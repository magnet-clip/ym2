﻿Imports System.ComponentModel
Imports DbManager
Imports DbManager.Bonds
Imports System.Runtime.InteropServices
Imports System.Globalization
Imports NLog
Imports ReutersData
Imports Uitls

Namespace Forms.PortfolioForm
    Public Class PortfolioForm
        Private Class ProgressShower
            Private _closed As Boolean = True
            Private _frm As New ProgressForm

            Public ReadOnly Property IsClosed() As Boolean
                Get
                    Return _closed
                End Get
            End Property

            Public Sub Revitalize()
                If Not _closed Then _frm.Close()
                _frm = New ProgressForm()
                AddHandler _frm.Closed, Sub() _closed = True
                _closed = False
                _frm.Show()
            End Sub

            Public Sub LogMessage(ByVal msg As String)
                _frm.LogMessage(msg)
            End Sub
        End Class

        Private Class ProgressForm
            Inherits Form

            Private ReadOnly _listBox As New ListBox With {
                .Location = New Point(20, 20),
                .Size = New Point(280, 180)
            }

            Public Sub LogMessage(ByVal msg As String)
                GuiAsync(Sub() _listBox.Items.Add(msg))
            End Sub

            Private WithEvents _close As New Button With {
                .Location = New Point(20, 210),
                .Size = New Point(80, 25),
                .Text = "Close"
            }

            Public Sub New()
                Dim sz = New Point(320, 290)
                With Me
                    .Size = sz
                    .MaximumSize = sz
                    .MinimumSize = sz
                    .Text = "Load progress"
                    .Controls.Add(_listBox)
                    .Controls.Add(_close)
                    .CancelButton = _close
                End With
            End Sub

            Private Sub CloseClick(ByVal sender As Object, ByVal e As EventArgs) Handles _close.Click
                Close()
            End Sub
        End Class

        Private Shared ReadOnly PortfolioManager As IPortfolioManager = DbManager.PortfolioManager.Instance()
        Private Shared ReadOnly Logger As Logger = Logging.GetLogger(GetType(PortfolioForm))
        Private WithEvents _loader As IBondsLoader = BondsLoader.Instance

        Private _dragNode As TreeNode
        Private _flag As Boolean

        Private _currentItem As Portfolio
        Private _currentBond As CustomBondSrc
        Private _locked As Boolean

        Private Property CurrentItem As Portfolio
            Get
                Return _currentItem
            End Get
            Set(ByVal value As Portfolio)
                _currentItem = value
                RefreshPortfolioData()
            End Set
        End Property

        Private Enum CMSSource
            CustomBond
            CouponSchedule
            AmortSchedule
            OptionList
        End Enum

        Private _customBondChanged As Boolean
        Private WithEvents _progressForm As New ProgressShower

        Friend Property CustomBondChanged() As Boolean
            Get
                Return _customBondChanged
            End Get
            Set(ByVal value As Boolean)
                CustomBondsPage.Text = "Custom bond" + If(value, " *", "")
                _customBondChanged = value
            End Set
        End Property

#Region "Common items"
        Private Sub PortfolioForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Initialize()
        End Sub

        Private Sub Initialize()
            BondsTableView.DataSource = _loader.GetBondsTable()
            If PortfolioTree.ImageList Is Nothing Then
                PortfolioTree.ImageList = New ImageList
                PortfolioTree.ImageList.Images.Add("folder", My.Resources.folder)
                PortfolioTree.ImageList.Images.Add("portfolio", My.Resources.briefcase)
            End If
            RefreshPortfolioTree()
            RefreshChainsLists()
            RefreshFieldsList()
            RefreshCustomBondList()
            'RefreshChainCurves()

            For Each clr In Utils.GetColorList()
                CustomBondColorCB.Items.Add(clr)
            Next
        End Sub

        Private Shared Sub ColorCellFormatting(ByVal sender As Object, ByVal e As DataGridViewCellFormattingEventArgs) Handles PortfolioChainsListsGrid.CellFormatting, PortfolioItemsGrid.CellFormatting, ChainsListsGrid.CellFormatting
            Dim dgv = TryCast(sender, DataGridView)
            If dgv Is Nothing Then Return
            If dgv.Columns(e.ColumnIndex).DataPropertyName = "Color" Then
                Dim theColor As KnownColor
                If TypeOf e.Value Is String AndAlso KnownColor.TryParse(e.Value, theColor) Then
                    e.CellStyle.BackColor = Color.FromKnownColor(theColor)
                    e.CellStyle.ForeColor = Color.FromKnownColor(theColor)
                End If
            End If
        End Sub

        Private Sub PortfolioForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
            If CustomBondChanged Then
                Select Case MessageBox.Show("Would you like to save changes in custom bond?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                    Case DialogResult.Yes : SaveCustomBond()
                    Case DialogResult.Cancel : e.Cancel = True
                End Select
            End If
        End Sub

#End Region

#Region "Menu"
        Private Sub CloseToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CloseToolStripMenuItem.Click
            Close()
        End Sub

        Private Sub OpenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OpenToolStripMenuItem.Click
            Dim a As New OpenFileDialog With {
                .AddExtension = True,
                .CheckFileExists = True,
                .InitialDirectory = Utils.GetMyPath(),
                .DefaultExt = "xml",
                .Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
            }
            If a.ShowDialog() = DialogResult.OK Then
                Try
                    PortfolioManager.SelectConfigFile(a.FileName)
                    Initialize()
                    _progressForm.Revitalize()
                    _loader.Initialize()

                    ' todo some notification to charts!!!!
                    ' todo next todo is support for RICS and no RICS tablez

                Catch ex As Exception
                    PortfolioManager.SelectDefaultConfigFile()
                End Try
            End If
        End Sub


#End Region

#Region "Portfolio TAB"
        Private Sub PortSourcesCheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ChainsCB.CheckedChanged, ListsCB.CheckedChanged, CustomBondsCB.CheckedChanged, RegularBondsCB.CheckedChanged
            RefreshPortfolioData()
        End Sub

        Private Sub PortItemsCheckChanged(ByVal sender As Object, ByVal e As EventArgs) Handles AllRB.CheckedChanged, SeparateRB.CheckedChanged
            RefreshPortfolioData()
        End Sub

        Private Sub RefreshPortfolioData()
            If CurrentItem Is Nothing Then Return
            If CurrentItem.IsFolder Then
                PortfolioChainsListsGrid.Columns.Clear()
                PortfolioChainsListsGrid.Rows.Clear()
                PortfolioItemsGrid.Columns.Clear()
                PortfolioItemsGrid.Rows.Clear()
            Else
                Cursor = Cursors.WaitCursor
                Dim descr = PortfolioManager.GetPortfolioStructure(CurrentItem.Id)

                PortfolioChainsListsGrid.DataSource = descr.Sources(
                    If(ChainsCB.Checked, PortfolioStructure.Chain, 0) Or
                    If(ListsCB.Checked, PortfolioStructure.List, 0) Or
                    If(CustomBondsCB.Checked, PortfolioStructure.CustomBond, 0) Or
                    If(RegularBondsCB.Checked, PortfolioStructure.RegularBond, 0)
                )

                PortfolioItemsGrid.DataSource = descr.Rics(AllRB.Checked)
                Cursor = Cursors.Default
            End If
        End Sub

        Private Sub RefreshPortfolioTree(Optional ByVal selId As Long = -1)
            Dim selectedNodeId As String
            If selId = -1 Then
                If PortfolioTree.SelectedNode IsNot Nothing Then
                    Dim descr = CType(PortfolioTree.SelectedNode.Tag, Portfolio)
                    selectedNodeId = descr.Id
                End If
            Else
                selectedNodeId = selId
            End If

            PortfolioTree.Nodes.Clear()

            If PortfolioManager.PortfoliosValid() Then
                PortfolioTree.BeginUpdate()
                Dim newSelNode = AddPortfoliosByFolder("", selectedNodeId)
                If newSelNode IsNot Nothing Then
                    newSelNode.EnsureVisible()
                    PortfolioTree.SelectedNode = newSelNode
                End If
                PortfolioTree.EndUpdate()
            Else
                MessageBox.Show("Portfolios are corrupted, unable to show", "Portfolios...", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            End If

        End Sub

        Private Function AddPortfoliosByFolder(ByVal theId As String, ByVal selId As String, Optional ByVal whereTo As TreeNode = Nothing) As TreeNode
            Dim portMan As PortfolioManager = PortfolioManager
            Dim res As TreeNode = If(theId = selId, whereTo, Nothing)
            Dim descrs = portMan.GetPortfoliosByFolder(theId)

            For Each descr In descrs
                Dim img = If(descr.IsFolder, "folder", "portfolio")
                Dim newNode As TreeNode
                If whereTo IsNot Nothing Then
                    newNode = whereTo.Nodes.Add(descr.Id, descr.Name, img, img)
                Else
                    newNode = PortfolioTree.Nodes.Add(descr.Id, descr.Name, img, img)
                End If
                newNode.Tag = descr
                If descr.IsFolder Then
                    Dim tmp = AddPortfoliosByFolder(descr.Id, selId, newNode)
                    If res Is Nothing AndAlso tmp IsNot Nothing Then res = tmp
                Else
                    If descr.Id = selId Then res = newNode
                End If
            Next
            Return res
        End Function

        Private Sub PortfolioTree_DblClick(ByVal sender As Object, ByVal e As EventArgs) Handles PortfolioTree.DoubleClick
            Dim mea = TryCast(e, MouseEventArgs)
            Dim node = PortfolioTree.GetNodeAt(mea.Location)
            If node Is Nothing Then Return
            DoRename(node)
        End Sub

        Private Sub RenameToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RenameToolStripMenuItem.Click
            Dim node = CType(PortTreeCM.Tag, TreeNode)
            If node Is Nothing Then Return
            DoRename(node)
        End Sub

        Private Sub DoRename(ByVal node As TreeNode)
            PortfolioTree.SelectedNode = node

            Dim adder = New AddPortfolioForm
            adder.EditMode = True
            adder.NewName.Text = node.Text
            adder.ItIsPortfolio = Not CType(node.Tag, Portfolio).IsFolder

            If adder.ShowDialog() = DialogResult.OK Then
                Dim portDescr As Portfolio
                portDescr = CType(node.Tag, Portfolio)
                If portDescr Is Nothing Then
                    MessageBox.Show("Failed to update name", "Name edit", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    If portDescr.IsFolder Then
                        PortfolioManager.SetFolderName(portDescr.Id, adder.NewName.Text)
                    Else
                        PortfolioManager.SetPortfolioName(portDescr.Id, adder.NewName.Text)
                    End If
                    RefreshPortfolioTree()
                End If
            End If
        End Sub

        Private Sub PortfolioTree_NodeMouseClick(ByVal sender As Object, ByVal e As TreeNodeMouseClickEventArgs) Handles PortfolioTree.NodeMouseClick
            If e.Button = MouseButtons.Right Then
                PortTreeCM.Tag = e.Node
                PortTreeCM.Show(PortfolioTree, e.Location)
            Else
                Dim temp = TryCast(e.Node.Tag, Portfolio)
                CurrentItem = If(temp IsNot Nothing AndAlso Not temp.IsFolder, temp, Nothing)
                Dim portSelected = CurrentItem IsNot Nothing AndAlso Not CurrentItem.IsFolder
                AddChainListButton.Enabled = portSelected
                RemoveChainListButton.Enabled = portSelected
                EditChainListButton.Enabled = portSelected
            End If
            _flag = False
        End Sub

        Private Sub PortfolioTree_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PortfolioTree.MouseUp
            If Not _flag Then
                _flag = True
                Return
            End If
            If e.Button = MouseButtons.Right Then
                PortTreeCM.Tag = PortfolioTree.SelectedNode
                PortTreeCM.Show(PortfolioTree, e.Location)
            End If
        End Sub

        Private Sub PortTreeCM_Opening(sender As Object, e As CancelEventArgs) Handles PortTreeCM.Opening
            RenameToolStripMenuItem.Enabled = PortTreeCM.Tag IsNot Nothing
            DeleteToolStripMenuItem.Enabled = PortTreeCM.Tag IsNot Nothing
        End Sub

        Private Sub AddToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddToolStripMenuItem.Click
            Dim theId As String
            Dim node = TryCast(PortTreeCM.Tag, TreeNode)
            If node Is Nothing Then
                theId = ""
            Else
                PortfolioTree.SelectedNode = node
                Dim descr = TryCast(node.Tag, Portfolio)
                If descr Is Nothing Then Return
                theId = descr.Id
            End If


            Dim adder As New AddPortfolioForm With {
             .EditMode = False,
             .ItIsPortfolio = True
            }
            If adder.ShowDialog() = DialogResult.OK Then
                Dim newId As Long
                Try
                    If adder.ItsFolder.Checked Then
                        newId = PortfolioManager.AddFolder(adder.NewName.Text, theId)
                    Else
                        newId = PortfolioManager.AddPortfolio(adder.NewName.Text, theId)
                    End If
                Catch ex As PortfolioException
                    MessageBox.Show("Can not add item into portfolio, please select a folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Finally
                    RefreshPortfolioTree(newId)
                End Try

            End If
        End Sub

        Private Sub DeleteToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteToolStripMenuItem.Click
            Dim node = TryCast(PortTreeCM.Tag, TreeNode)
            If node Is Nothing Then Return

            Dim descr = TryCast(node.Tag, Portfolio)
            If descr Is Nothing Then Return
            If MessageBox.Show("Are you sure you would like to delete an item permanently?", "Delete...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                If descr.IsFolder Then
                    PortfolioManager.DeleteFolder(descr.Id)
                Else
                    PortfolioManager.DeletePortfolio(descr.Id)
                End If
                RefreshPortfolioTree()
            End If
        End Sub

        Private Sub PortfolioTree_ItemDrag(ByVal sender As Object, ByVal e As ItemDragEventArgs) Handles PortfolioTree.ItemDrag
            _dragNode = e.Item
            DoDragDrop(e.Item, DragDropEffects.Copy Or DragDropEffects.Move)
        End Sub

        <DllImport("user32.dll")>
        Private Shared Function GetKeyState(ByVal key As Keys) As Short
        End Function

        Private Sub PortfolioTree_DragOver(ByVal sender As Object, ByVal e As DragEventArgs) Handles PortfolioTree.DragOver
            Dim pos = PortfolioTree.PointToClient(New Point(e.X, e.Y))
            Dim node = PortfolioTree.GetNodeAt(pos)
            Dim copy = GetKeyState(Keys.ControlKey) < 0
            If node Is Nothing Then
                e.Effect = If(copy, DragDropEffects.Copy, DragDropEffects.Move)
            Else
                node.Expand()
                Dim descr = TryCast(node.Tag, Portfolio)
                If IsChildOf(_dragNode, node) OrElse (descr IsNot Nothing AndAlso Not descr.IsFolder) Then
                    e.Effect = DragDropEffects.None
                Else
                    e.Effect = If(copy, DragDropEffects.Copy, DragDropEffects.Move)
                End If
            End If
        End Sub

        Private Shared Function IsChildOf(ByVal ofWhat As TreeNode, ByVal who As TreeNode) As Boolean
            If ofWhat Is Nothing Then Return False
            Do
                If ofWhat.Equals(who) Then Return True
                If ofWhat.Equals(who.Parent) Then Return True
                If who.Parent IsNot Nothing AndAlso TypeOf who.Parent Is TreeNode Then
                    who = who.Parent
                Else
                    Return False
                End If
            Loop
        End Function

        Private Sub PortfolioTree_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles PortfolioTree.DragDrop
            Dim dragDescr = TryCast(_dragNode.Tag, Portfolio)
            If dragDescr Is Nothing Then Return
            Dim copy = GetKeyState(Keys.ControlKey) < 0

            Dim pos = PortfolioTree.PointToClient(New Point(e.X, e.Y))
            Dim node = PortfolioTree.GetNodeAt(pos)
            Dim resId As String
            If node Is Nothing Then
                If copy Then
                    resId = PortfolioManager.CopyItemToTop(dragDescr.Id)
                Else
                    resId = PortfolioManager.MoveItemToTop(dragDescr.Id)
                End If
            Else
                Dim descr = TryCast(node.Tag, Portfolio)
                If descr Is Nothing OrElse Not descr.IsFolder Then Return
                If copy Then
                    resId = PortfolioManager.CopyItemToFolder(dragDescr.Id, descr.Id)
                Else
                    resId = PortfolioManager.MoveItemToFolder(dragDescr.Id, descr.Id)
                End If
            End If
            RefreshPortfolioTree(resId)
        End Sub

        Private Sub AddChainListButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddChainListButton.Click
            Dim a As New AddPortfolioSource
            If a.ShowDialog() = DialogResult.OK Then
                CurrentItem.AddSource(a.Data.Src, a.Data.CustomName, a.Data.CustomColor, a.Data.Condition, a.Data.Include)
                RefreshPortfolioData()
            End If
        End Sub

        Private Sub RemoveChainListButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveChainListButton.Click
            If PortfolioChainsListsGrid.SelectedRows.Count <= 0 Then
                MessageBox.Show("Please select an item to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If
            Dim srcToDelete = (From item As DataGridViewRow In PortfolioChainsListsGrid.SelectedRows
                               Where item.DataBoundItem IsNot Nothing
                               Let elem = TryCast(item.DataBoundItem, PortfolioSource)
                               Where elem IsNot Nothing
                               Select elem).ToList()
            For Each item In srcToDelete
                Try
                    CurrentItem.DeleteSource(item)
                    RefreshPortfolioData()
                Catch ex As Exception
                    Logger.ErrorException("Failed to delete selected source", ex)
                    Logger.Error("Exception = {0}", ex.ToString())
                    MessageBox.Show("Failed to delete selected source", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Next
        End Sub

        Private Sub EditChainListButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles EditChainListButton.Click
            If PortfolioChainsListsGrid.SelectedRows.Count <= 0 Then
                MessageBox.Show("Please select an item to edit", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If
            Dim item = CType(PortfolioChainsListsGrid.SelectedRows(0).DataBoundItem, PortfolioSource)
            If item Is Nothing Then Exit Sub
            Dim a As New EditPortfolioSource(item.Source)
            a.CustomName = item.Name
            a.CustomColor = item.Color
            a.Condition = item.Condition
            a.Include = item.Included

            If a.ShowDialog() = DialogResult.OK Then
                If a.Data.Src Is Nothing Then Return
                CurrentItem.UpdateSource(item, a.Data.Src, a.Data.CustomName, a.Data.CustomColor, a.Data.Condition, a.Data.Include)
                RefreshPortfolioData()
            End If
        End Sub
#End Region

#Region "Data TAB"
        Private Sub TableChooserList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TableChooserList.SelectedIndexChanged
            RefreshDataGrid()
        End Sub

        Private Sub RefreshDataGrid()
            If TableChooserList.SelectedIndex >= 0 Then
                Select Case TableChooserList.Items(TableChooserList.SelectedIndex).ToString()
                    Case "Bonds" : BondsTableView.DataSource = _loader.GetBondsTable()
                    Case "Coupons" : BondsTableView.DataSource = _loader.GetCouponsTable()
                    Case "FRNs" : BondsTableView.DataSource = _loader.GetFRNsTable()
                    Case "Issue ratings" : BondsTableView.DataSource = _loader.GetIssueRatingsTable()
                    Case "Issuer ratings" : BondsTableView.DataSource = _loader.GetIssuerRatingsTable()
                    Case "Rics" : BondsTableView.DataSource = _loader.GetAllRicsTable()
                    Case Else
                        BondsTableView.DataSource = Nothing
                End Select
            End If
        End Sub

        Private Sub CleanupDataButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CleanupDataButton.Click
            _loader.ClearTables()
            RefreshDataGrid()
        End Sub


        Private Sub ReloadDataButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ReloadDataButton.Click
            _loader.ClearTables()
            _progressForm.Revitalize()
            _loader.Initialize()
            RefreshDataGrid()
        End Sub
#End Region

#Region "Chains and lists TAB"
        Private Shared Sub OnCellBeginEdit(ByVal sender As Object, ByVal e As DataGridViewCellCancelEventArgs) Handles ChainsListsGrid.CellBeginEdit, ChainListItemsGrid.CellBeginEdit
            e.Cancel = True
        End Sub

        Private Sub ChainsListCheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ChainsButton.CheckedChanged
            Logger.Debug("Refresh...")

            ChainListItemsGrid.DataSource = Nothing
            ChainListItemsGrid.Rows.Clear()
            ChainListItemsGrid.Columns.Clear()

            RefreshChainsLists()
        End Sub

        Private Sub RefreshChainsLists(Optional ByVal selectedSource As Source = Nothing)
            ChainsListsGrid.DataSource = If(ChainsButton.Checked, PortfolioManager.ChainsView, PortfolioManager.UserListsView)
            For Each col As DataGridViewColumn In ChainsListsGrid.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            Next

            If selectedSource Is Nothing Then Return

            ChainsListsGrid.ClearSelection()
            For Each row As DataGridViewRow In (From rw As DataGridViewRow In ChainsListsGrid.Rows
                                               Where rw.DataBoundItem IsNot Nothing AndAlso rw.DataBoundItem.Equals(selectedSource))
                row.Selected = True
                Exit For
            Next
        End Sub


        Private Sub ChainsListsGrid_SelectionChanged(sender As Object, e As EventArgs) Handles ChainsListsGrid.SelectionChanged
            If ChainsListsGrid.SelectedRows.Count <= 0 Then Return
            Dim item = ChainsListsGrid.SelectedRows(0).DataBoundItem
            If item Is Nothing Then Return
            AddItemsButton.Enabled = TypeOf item Is UserListSrc And Not TypeOf item Is UserQuerySrc
            DeleteItemsButton.Enabled = AddItemsButton.Enabled
        End Sub

        Private Sub AddCLButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddCLButton.Click, AddCLTSMI.Click
            Dim frm As New AddEditChainList
            If frm.ShowDialog() = DialogResult.OK Then RefreshChainsLists(frm.SaveSource())
        End Sub

        Private Sub EditCLButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles EditCLButton.Click, EditCLTSMI.Click
            If ChainsListsGrid.SelectedRows.Count <> 1 Then
                MessageBox.Show("Please select chain or list to edit", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim selectedItem = CType(ChainsListsGrid.SelectedRows.Item(0).DataBoundItem, Source)
            Dim frm As New AddEditChainList
            frm.Src = selectedItem
            If frm.ShowDialog() = DialogResult.OK Then
                Dim tmp = frm.Src
                PortfolioManager.UpdateSource(tmp)
                RefreshChainsLists(tmp)
            End If
        End Sub

        Private Sub DeleteCLButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteCLButton.Click, DeleteCLTSMI.Click
            If ChainsListsGrid.SelectedRows.Count <> 1 Then
                MessageBox.Show("Please select chain or list to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim selectedItem = CType(ChainsListsGrid.SelectedRows.Item(0).DataBoundItem, Source)

            If TypeOf selectedItem Is ChainSrc Then
                Dim queries = PortfolioManager.GetQueriesByChain(selectedItem)
                If queries.Any Then
                    Dim aList = String.Join(Environment.NewLine, (From q In queries Select q.Value))
                    MessageBox.Show("There are the following queries using this chain. Please delete them first." + Environment.NewLine + aList, "Action required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return
                End If
            End If

            Dim list = PortfolioManager.GetPortfoliosBySource(selectedItem)
            If Not list.Any OrElse (
                   MessageBox.Show(
                       String.Format("There are {0} portfolios using this item, are you sure you'd like to delete it?", list.Count),
                       "Please confirm",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question) = DialogResult.Yes) Then
                PortfolioManager.DeleteSource(selectedItem)
                RefreshChainsLists()
            End If
        End Sub

        Private Sub ChainsListsGrid_CellMouseClick(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles ChainsListsGrid.CellMouseClick
            If e.RowIndex < 0 Then Return
            Dim elem = ChainsListsGrid.Rows(e.RowIndex).DataBoundItem
            Dim chain As Source = TryCast(elem, Source)
            If chain Is Nothing Then Return
            If e.Button = MouseButtons.Left Then
                RefreshChainListItemGrid(chain)
            Else
                ChainsListsCMS.Show(ChainsListsGrid, e.Location)
            End If
        End Sub

        Private Sub ReloadChainButtin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ReloadChainButton.Click, ReloadCLTSMI.Click
            If ChainsListsGrid.SelectedRows.Count = 0 Then
                MessageBox.Show("Please select one or more chains to reload", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim items = (From row As DataGridViewRow In ChainsListsGrid.SelectedRows
                         Where TypeOf row.DataBoundItem Is ChainSrc
                         Select CType(row.DataBoundItem, ChainSrc).ChainRic).ToList()

            If Not items.Any Then
                MessageBox.Show("Please select one or more chains to reload", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            _progressForm.Revitalize()
            _loader.LoadChains(items)
        End Sub

        Private Sub BondsLoaderFinished(ByVal evt As ProgressEvent) Handles _loader.Progress
            Logger.Info("Got message {0}", evt.Msg)
            If _progressForm.IsClosed Then
                If evt.Log.Success() Then
                    MessageBox.Show("Data updated successfully", "Success")
                    GuiAsync(AddressOf RefreshChainsLists)
                ElseIf evt.Log.Failed() Then
                    MessageBox.Show(evt.Log.Entries.Select(Function(item) item.Msg).Aggregate(Function(str, item) str + Environment.NewLine + item),
                                    "Errors occured", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                _progressForm.LogMessage(evt.Msg)
            End If
        End Sub

        Private Sub AddItemsButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddItemsButton.Click
            If ChainsButton.Checked Then Return
            If ChainsListsGrid.SelectedRows.Count <= 0 Then Return

            Dim elem = ChainsListsGrid.SelectedRows.Item(0).DataBoundItem
            Dim src As UserListSrc = TryCast(elem, UserListSrc)
            If src Is Nothing Then Return

            Dim a As New BondSelectorForm
            If a.ShowDialog() = DialogResult.OK Then
                src.AddItems(a.SelectedRICs)
                RefreshChainListItemGrid(src)
            End If
        End Sub

        Private Sub RefreshChainListItemGrid(ByVal src As Source)
            Cursor = Cursors.WaitCursor
            ChainListItemsGrid.DataSource = src.GetDefaultRicsView()
            Cursor = Cursors.Default
            For Each col As DataGridViewColumn In ChainListItemsGrid.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            Next
        End Sub

        Private Sub DeleteItemsButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteItemsButton.Click
            If ChainsButton.Checked Then Return
            If ChainsListsGrid.SelectedRows.Count <= 0 Then Return

            Dim elem = ChainsListsGrid.SelectedRows.Item(0).DataBoundItem
            Dim src As UserListSrc = TryCast(elem, UserListSrc)
            If src Is Nothing Then Return

            Dim rics = (From row As DataGridViewRow In ChainListItemsGrid.SelectedRows
                        Let descr = TryCast(row.DataBoundItem, BondMetadata)
                        Where descr IsNot Nothing
                        Select descr.Ric).ToList()
            If rics.Any Then
                src.RemoveItems(rics)
                RefreshChainListItemGrid(src)
            End If
        End Sub
#End Region

#Region "Fields TAB"
        Private Sub RefreshFieldsList()
            FieldsListBox.DataSource = PortfolioManager.GetFieldLayouts
            RefreshFieldLayoutsList()
        End Sub

        Private Sub FieldsListBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles FieldsListBox.SelectedIndexChanged
            RefreshFieldLayoutsList()
        End Sub

        Private Sub RefreshFieldLayoutsList()
            If FieldsListBox.SelectedIndex < 0 Then Return

            Dim elem = TryCast(FieldsListBox.Items(FieldsListBox.SelectedIndex), IdName(Of String))
            If elem Is Nothing Then Return

            FieldLayoutsListBox.DataSource = New FieldSet(elem.Id).AsDataSource
            FieldLayoutsListBox.SelectedIndex = -1
            FieldsGrid.DataSource = Nothing
        End Sub

        Private Sub FieldLayoutsListBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles FieldLayoutsListBox.SelectedIndexChanged
            RefreshFieldsGrid()
        End Sub

        Private Sub RefreshFieldsGrid()
            If FieldLayoutsListBox.SelectedIndex < 0 Then
                FieldsGrid.DataSource = Nothing
            Else
                Dim item = TryCast(FieldLayoutsListBox.Items(FieldLayoutsListBox.SelectedIndex), FieldsDescription)
                If item Is Nothing Then
                    FieldsGrid.DataSource = Nothing
                Else
                    FieldsGrid.DataSource = item.AsDataSource()
                End If

            End If
        End Sub

        Private Sub FieldsGrid_CellEndEdit(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles FieldsGrid.CellEndEdit
            Dim elem = TryCast(FieldsGrid.Rows(e.RowIndex).DataBoundItem, FieldDescription)
            If elem Is Nothing Then Return
            elem.Value = FieldsGrid.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
        End Sub

#End Region

#Region "Custom bond TAB"
        Private Function GetSource(ByVal sender As Object) As CMSSource?
            If ReferenceEquals(sender, CustomBondsList) Then Return CMSSource.CustomBond
            If ReferenceEquals(sender, CouponScheduleDGV) Then Return CMSSource.CouponSchedule
            If ReferenceEquals(sender, AmortScheduleDGV) Then Return CMSSource.AmortSchedule
            If ReferenceEquals(sender, OptionsDGV) Then Return CMSSource.OptionList
            Return Nothing
        End Function

        Private Sub CustomBondsList_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) _
            Handles CustomBondsList.MouseClick, OptionsDGV.MouseClick, CouponScheduleDGV.Click, AmortScheduleDGV.Click

            If e.Button <> MouseButtons.Right Then Return

            Dim src = GetSource(sender)

            If src Is Nothing Then Return
            RenameCustomBondTSMI.Visible = (src = CMSSource.CustomBond)

            CustomBondListCMS.Tag = sender
            CustomBondListCMS.Show(sender, e.Location)

            CustomBondChanged = False
        End Sub

        Private Sub RefreshCustomBondList(Optional ByVal toselect As CustomBondSrc = Nothing, Optional ByVal furtherRefresh As Boolean = True)
            CustomBondsList.DataSource = PortfolioManager.CustomBondsView()
            If toselect IsNot Nothing Then
                Dim x = (From row As DataGridViewRow In CustomBondsList.Rows
                         Where CType(row.DataBoundItem, CustomBondSrc).ID = toselect.ID
                         Select row.Index).ToList()
                If x.Any Then CustomBondsList.Rows(x.First).Selected = True
            End If
            If Not furtherRefresh Then Exit Sub
            If CustomBondsList.Rows.Count > 0 Then
                EnableEverythingComplete()
                RefreshBondView()
            Else
                DisableEverythingComplete()
            End If
        End Sub

        Private Sub CustomBondsList_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CustomBondsList.SelectionChanged
            If CustomBondsList.SelectedRows.Count = 0 Then
                EnableEverythingComplete()
            Else
                If _currentBond IsNot Nothing Then PortfolioManager.UpdateSource(_currentBond)
                _currentBond = CustomBondsList.SelectedRows(0).DataBoundItem
                RefreshBondView()
            End If
            CustomBondChanged = False
        End Sub

        Private Sub RandomColorButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RandomColorButton.Click
            CustomBondColorCB.SelectedIndex = New Random().NextDouble() * (CustomBondColorCB.Items.Count - 1)
            If _currentBond Is Nothing Then Return
            _currentBond.Color = CustomBondColorCB.SelectedItem
            CustomBondChanged = True
        End Sub

        Private Sub ColorComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CustomBondColorCB.SelectedIndexChanged
            If CustomBondColorCB.SelectedIndex < 0 Then
                CustomBondColorPB.BackColor = Color.White
            Else
                CustomBondColorPB.BackColor = Color.FromName(CustomBondColorCB.SelectedItem)
            End If
            CustomBondChanged = True
        End Sub

        Private Sub CustomColorCB_DrawItem(ByVal sender As Object, ByVal e As DrawItemEventArgs) Handles CustomBondColorCB.DrawItem
            Dim g As Graphics = e.Graphics
            Dim r As Rectangle = e.Bounds
            If e.Index > 0 Then
                Dim txt As String = CustomBondColorCB.Items(e.Index)
                g.DrawString(txt, CustomBondColorCB.Font, Brushes.Black, r.X, r.Top)
                Dim m = g.MeasureString(txt, CustomBondColorCB.Font)
                Dim c As Color = Color.FromName(txt)
                g.FillRectangle(New SolidBrush(c), r.X + m.Width + 10, r.Y + 2, r.Width - m.Width - 15, r.Height - 6)
                g.DrawRectangle(New Pen(New SolidBrush(Color.Black)), r.X + m.Width + 10, r.Y + 2, r.Width - m.Width - 15, r.Height - 6)
            End If
        End Sub

        Private Sub RefreshBondView()
            If _currentBond Is Nothing Then
                DisableEverythingComplete()
                Return
            End If

            LockEverything()
            EditManCB.Checked = False
            Dim bondStructure = _currentBond.Struct

            CustomBondColorCB.SelectedItem = _currentBond.Color
            MaturityDTP.Value = If(_currentBond.Maturity.HasValue, _currentBond.Maturity, Date.Today)
            FrequencyCB.SelectedItem = bondStructure.Frequency

            FixedRateTB.Text = String.Format(CultureInfo.InvariantCulture, "{0:F4}", bondStructure.GetFixedRate())
            CouponScheduleDGV.DataSource = bondStructure.GetCouponsList()

            PerpetualCB.Checked = bondStructure.IsPerpetual
            MaturityDTP.Enabled = Not bondStructure.IsPerpetual
            AnnuityCB.Checked = bondStructure.IsAnnuity

            AmortScheduleDGV.DataSource = bondStructure.GetAmortizationSchedule()

            OptionsDGV.DataSource = bondStructure.GetEmbeddedOptions()
            OtherRulesML.Text = bondStructure.ToString()

            If bondStructure.IssueDate IsNot Nothing AndAlso bondStructure.IssueDate <> "" Then
                UnspecifiedIssueDateCB.Checked = False
                IssueDateDTP.Enabled = True
                IssueDateDTP.Value = ReutersDate.ReutersToDate(bondStructure.IssueDate)
            Else
                UnspecifiedIssueDateCB.Checked = True
                IssueDateDTP.Enabled = False
            End If

            UnlockEverything()
            RecalculateCashFlows()
        End Sub

        Private Sub UnlockEverything()
            _locked = False
        End Sub

        Private Sub LockEverything()
            _locked = True
        End Sub

        Private Sub RecalculateCashFlows()
            If _locked Then Return
            Dim bondStructure = _currentBond.Struct
            OtherRulesML.Text = bondStructure.ToString()
            If MainForm.MainForm.Connected Then
                MessagesTB.Text = ""
                bondStructure.SetBondModule(Eikon.Sdk.CreateAdxBondModule())
                Try
                    CashFlowsDGV.DataSource = bondStructure.GetCashFlows(If(_currentBond.Maturity.HasValue, ReutersDate.DateToReuters(_currentBond.Maturity), ""), _currentBond.CurrentCouponRate)
                Catch ex As Exception
                    MessagesTB.Text = ex.Message
                End Try
            Else
                MessagesTB.Text = "Not connected to Eikon platform"
            End If
        End Sub

        Private Sub UnspecifiedIssueDateCB_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles UnspecifiedIssueDateCB.CheckedChanged
            If _locked Then Return
            IssueDateDTP.Enabled = Not UnspecifiedIssueDateCB.Checked
            If _currentBond Is Nothing Then Return
            If UnspecifiedIssueDateCB.Checked Then
                _currentBond.Struct.IssueDate = ""
            Else
                _currentBond.Struct.IssueDate = ReutersDate.DateToReuters(IssueDateDTP.Value)
            End If
            RecalculateCashFlows()
            CustomBondChanged = True
        End Sub

        Private Sub MaturityDTP_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles MaturityDTP.ValueChanged
            If _locked Then Return
            If _currentBond Is Nothing Then Return
            _currentBond.Struct.ReimbursementType = ""
            _currentBond.Maturity = MaturityDTP.Value
            RecalculateCashFlows()
            CustomBondChanged = True
        End Sub

        Private Sub IssueDTP_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles IssueDateDTP.ValueChanged
            If _locked Then Return
            If _currentBond Is Nothing Then Return
            _currentBond.Struct.IssueDate = ReutersDate.DateToReuters(IssueDateDTP.Value)
            RecalculateCashFlows()
            CustomBondChanged = True
        End Sub


        Private Sub AnnuityCB_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles AnnuityCB.CheckedChanged
            If _locked Then Return
            If AnnuityCB.Checked Then
                If PerpetualCB.Checked Then PerpetualCB.Checked = False
                _currentBond.Struct.ReimbursementType = "C"
            Else
                _currentBond.Struct.ReimbursementType = ""
            End If
            RecalculateCashFlows()
            CustomBondChanged = True
        End Sub

        Private Sub PerpetualCB_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles PerpetualCB.CheckedChanged
            If _locked Then Return
            If _currentBond Is Nothing Then Return
            MaturityDTP.Enabled = Not PerpetualCB.Checked
            If PerpetualCB.Checked Then
                AnnuityCB.Checked = False
                _currentBond.Maturity = Nothing
                _currentBond.Struct.ReimbursementType = "P"
            Else
                _currentBond.Maturity = MaturityDTP.Value
                _currentBond.Struct.ReimbursementType = ""
            End If
            RecalculateCashFlows()
            CustomBondChanged = True
        End Sub

        Private Sub FrequencyCB_SelectedValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles FrequencyCB.SelectedValueChanged
            If _locked Then Return
            If _currentBond Is Nothing Then Return
            _currentBond.Struct.Frequency = FrequencyCB.SelectedItem
            RecalculateCashFlows()
            CustomBondChanged = True
        End Sub

        Private Sub FixedRateTB_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles FixedRateTB.TextChanged
            If _locked Then Return
            If _currentBond Is Nothing Then Return
            Dim res As Single
            If Not Single.TryParse(FixedRateTB.Text, NumberStyles.Any, CultureInfo.InvariantCulture, res) Then
                FixedRateTB.ForeColor = Color.Red
                Return
            Else
                FixedRateTB.ForeColor = Color.Black
            End If
            _currentBond.CurrentCouponRate = res

            _currentBond.Struct.Rate = FixedRateTB.Text
            RecalculateCashFlows()
            CustomBondChanged = True
        End Sub

        Private Sub RenameCustomBondTSMI_Click(sender As Object, e As EventArgs) Handles RenameCustomBondTSMI.Click
            If _currentBond Is Nothing Then Return
            Dim newName = InputBox("Please enter new bond name", "Rename custom bond", _currentBond.Name)
            If newName <> "" Then
                _currentBond.Name = newName
            End If
            CustomBondChanged = True
        End Sub


        Private Sub AddNewCustomBondTSMI_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddNewCustomBondTSMI.Click
            Dim src = GetSource(CustomBondListCMS.Tag)
            If src Is Nothing Then Return
            Select Case src
                Case CMSSource.CustomBond
                    Dim frm As New Form With {
                        .Size = New Point(300, 200),
                        .MaximumSize = New Point(300, 200),
                        .Text = "Create new custom bond"
                    }

                    frm.Controls.Add(New Label With {.Text = "Name", .Location = New Point(20, 20), .Width = 60})
                    frm.Controls.Add(New Label With {.Text = "Code", .Location = New Point(20, 60), .Width = 60})
                    Dim nameTb = New TextBox With {.Location = New Point(80, 20), .Width = 200}
                    frm.Controls.Add(nameTb)
                    Dim descrTb = New TextBox With {.Location = New Point(80, 60), .Width = 200}
                    frm.Controls.Add(descrTb)
                    Dim btnOk As New Button With {.Text = "Ok", .Location = New Point(20, 100), .DialogResult = DialogResult.OK}
                    AddHandler btnOk.Click, Sub() frm.Close()
                    frm.Controls.Add(btnOk)
                    Dim btnCancel As New Button With {.Text = "Cancel", .Location = New Point(120, 100), .DialogResult = DialogResult.Cancel}
                    AddHandler btnCancel.Click, Sub() frm.Close()
                    frm.Controls.Add(btnCancel)
                    frm.CancelButton = btnCancel
                    frm.AcceptButton = btnOk

                    If frm.ShowDialog() = DialogResult.OK Then
                        SaveCustomBond()
                        Const struct = "ACC:A5 IC:L1 CLDR:RUS_FI SETTLE:0WD  CFADJ:NO DMC:FOLLOWING EMC:LASTDAY PX:CLEAN REFDATE:MATURITY"
                        _currentBond = New CustomBondSrc(Color.Gray.Name, nameTb.Text, descrTb.Text, struct,
                                                      ReutersDate.DateToReuters(Date.Today.AddYears(1)), 0.1)
                        PortfolioManager.AddSource(_currentBond)
                        RefreshCustomBondList()
                    End If
                Case CMSSource.CouponSchedule
                    Dim frm As New Form With {
                        .Size = New Point(300, 200),
                        .MaximumSize = New Point(300, 200),
                        .Text = "Add another coupon rate"
                    }

                    frm.Controls.Add(New Label With {.Text = "Since date", .Location = New Point(20, 20), .Width = 60})
                    frm.Controls.Add(New Label With {.Text = "Coupon rate", .Location = New Point(20, 60), .Width = 60})
                    Dim sinceDateDtp = New DateTimePicker With {
                        .Location = New Point(80, 20),
                        .Width = 200,
                        .Format = DateTimePickerFormat.Custom,
                        .CustomFormat = "dd/MMM/yyyy"
                    }
                    frm.Controls.Add(sinceDateDtp)
                    Dim newRateTb = New TextBox With {.Location = New Point(80, 60), .Width = 200}
                    frm.Controls.Add(newRateTb)
                    Dim btnOk As New Button With {.Text = "Ok", .Location = New Point(20, 100), .DialogResult = DialogResult.OK}
                    AddHandler btnOk.Click, Sub() frm.Close()
                    frm.Controls.Add(btnOk)
                    Dim btnCancel As New Button With {.Text = "Cancel", .Location = New Point(120, 100), .DialogResult = DialogResult.Cancel}
                    AddHandler btnCancel.Click, Sub() frm.Close()
                    frm.Controls.Add(btnCancel)
                    frm.CancelButton = btnCancel
                    frm.AcceptButton = btnOk

                    If frm.ShowDialog() = DialogResult.OK Then
                        Dim triedRes As Single

                        If Not Single.TryParse(newRateTb.Text, NumberStyles.Any, CultureInfo.InvariantCulture, triedRes) Then
                            MessageBox.Show(String.Format("Coupon value {0} is not numeric", newRateTb.Text),
                                            "Please try once again", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            _currentBond.Struct.StepCouponPattern.Add(tuple.Create(sinceDateDtp.Value, triedRes))
                            CustomBondChanged = True
                            RefreshBondView()
                            RecalculateCashFlows()
                        End If
                    End If
                Case CMSSource.AmortSchedule
                    Dim frm As New Form With {
                        .Size = New Point(300, 200),
                        .MaximumSize = New Point(300, 200),
                        .Text = "Add partial redemption"
                    }
                    frm.Controls.Add(New Label With {.Text = "Date", .Location = New Point(20, 20), .Width = 60})
                    frm.Controls.Add(New Label With {.Text = "Amount", .Location = New Point(20, 60), .Width = 60})
                    Dim dateDtp = New DateTimePicker With {
                        .Location = New Point(80, 20),
                        .Width = 200,
                        .Format = DateTimePickerFormat.Custom,
                        .CustomFormat = "dd/MMM/yyyy"
                    }
                    frm.Controls.Add(dateDtp)
                    Dim amountTb = New TextBox With {.Location = New Point(80, 60), .Width = 200}
                    frm.Controls.Add(amountTb)
                    Dim btnOk As New Button With {.Text = "Ok", .Location = New Point(20, 100), .DialogResult = DialogResult.OK}
                    AddHandler btnOk.Click, Sub() frm.Close()
                    frm.Controls.Add(btnOk)
                    Dim btnCancel As New Button With {.Text = "Cancel", .Location = New Point(120, 100), .DialogResult = DialogResult.Cancel}
                    AddHandler btnCancel.Click, Sub() frm.Close()
                    frm.Controls.Add(btnCancel)
                    frm.CancelButton = btnCancel
                    frm.AcceptButton = btnOk

                    If frm.ShowDialog() = DialogResult.OK Then
                        Dim triedRes As Single
                        If Not Single.TryParse(amountTb.Text, NumberStyles.Any, CultureInfo.InvariantCulture, triedRes) Then
                            MessageBox.Show(String.Format("Redemption value {0} is not numeric", amountTb.Text),
                                            "Please try once again", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            _currentBond.Struct.AmortPattern.Add(tuple.Create(dateDtp.Value, triedRes))
                            CustomBondChanged = True
                            RefreshBondView()
                            RecalculateCashFlows()
                        End If
                    End If
                Case CMSSource.OptionList
                    Dim frm As New Form With {
                        .Text = "Add embdedded option",
                        .Size = New Point(400, 200),
                        .MaximumSize = New Point(400, 200)
                    }

                    frm.Controls.Add(New Label With {.Text = "Option start date", .Location = New Point(20, 20), .Width = 120})
                    frm.Controls.Add(New Label With {.Text = "Option end date", .Location = New Point(20, 45), .Width = 120})
                    frm.Controls.Add(New Label With {.Text = "Strike price", .Location = New Point(20, 70), .Width = 120})
                    frm.Controls.Add(New Label With {.Text = "Option type", .Location = New Point(20, 95), .Width = 120})

                    Dim startDateDtp = New DateTimePicker With {
                        .Location = New Point(140, 20),
                        .Width = 200,
                        .Format = DateTimePickerFormat.Custom,
                        .CustomFormat = "dd/MMM/yyyy"
                    }
                    frm.Controls.Add(startDateDtp)

                    Dim endDateDtp = New DateTimePicker With {
                        .Location = New Point(140, 45),
                        .Width = 200,
                        .Format = DateTimePickerFormat.Custom,
                        .CustomFormat = "dd/MMM/yyyy"
                    }
                    frm.Controls.Add(endDateDtp)

                    Dim priceTb = New TextBox With {.Location = New Point(140, 70), .Width = 200}
                    frm.Controls.Add(priceTb)

                    Dim optionTypeCb = New ComboBox With {
                        .Location = New Point(140, 95),
                        .Width = 200
                    }
                    optionTypeCb.Items.Add("Call")
                    optionTypeCb.Items.Add("Put")
                    optionTypeCb.SelectedIndex = 1
                    frm.Controls.Add(optionTypeCb)

                    Dim btnOk As New Button With {.Text = "Ok", .Location = New Point(20, 140), .DialogResult = DialogResult.OK}
                    AddHandler btnOk.Click, Sub() frm.Close()
                    frm.Controls.Add(btnOk)
                    Dim btnCancel As New Button With {.Text = "Cancel", .Location = New Point(120, 140), .DialogResult = DialogResult.Cancel}
                    AddHandler btnCancel.Click, Sub() frm.Close()
                    frm.Controls.Add(btnCancel)
                    frm.CancelButton = btnCancel
                    frm.AcceptButton = btnOk

                    If frm.ShowDialog() = DialogResult.OK Then
                        Dim triedRes As Single

                        If Not Single.TryParse(priceTb.Text, NumberStyles.Any, CultureInfo.InvariantCulture, triedRes) Then
                            MessageBox.Show(String.Format("Strike price {0} is not numeric", priceTb.Text),
                                            "Please try once again", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            If optionTypeCb.SelectedItem.ToString() = "Call" Then
                                _currentBond.Struct.CallPattern.Add(tuple.Create(startDateDtp.Value, endDateDtp.Value, triedRes))
                            Else
                                _currentBond.Struct.PutPattern.Add(tuple.Create(startDateDtp.Value, endDateDtp.Value, triedRes))
                            End If
                            CustomBondChanged = True
                            RefreshBondView()
                            RecalculateCashFlows()
                        End If
                    End If
            End Select
        End Sub

        Private Sub SaveCustomBond()
            If _currentBond IsNot Nothing AndAlso CustomBondChanged Then
                PortfolioManager.UpdateSource(_currentBond)
                CustomBondChanged = False
            End If
        End Sub

        Private Sub DeleteCustomBondTSMI_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteCustomBondTSMI.Click
            If _currentBond Is Nothing Then Return

            Dim src = GetSource(CustomBondListCMS.Tag)
            If src Is Nothing Then Return
            Select Case src
                Case CMSSource.CustomBond
                    If CustomBondsList.SelectedRows.Count <= 0 Then
                        MessageBox.Show("Please select custom bond to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                    Dim item = CType(CustomBondsList.SelectedRows(0).DataBoundItem, CustomBondSrc)
                    PortfolioManager.DeleteSource(item)
                    RefreshCustomBondList()
                Case CMSSource.AmortSchedule
                    If AmortScheduleDGV.SelectedRows.Count <= 0 Then
                        MessageBox.Show("Please select amortization item to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                    Dim item = CType(AmortScheduleDGV.SelectedRows(0).DataBoundItem, ReutersBondStructure.AmortizationDescription)
                    _currentBond.Struct.DeleteAmortizationItem(item)
                    CustomBondChanged = True
                Case CMSSource.CouponSchedule
                    If CouponScheduleDGV.SelectedRows.Count <= 0 Then
                        MessageBox.Show("Please select coupon to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                    Dim item = CType(CouponScheduleDGV.SelectedRows(0).DataBoundItem, ReutersBondStructure.CouponDescription)
                    _currentBond.Struct.DeleteCouponItem(item)
                    CustomBondChanged = True
                Case CMSSource.OptionList
                    If OptionsDGV.SelectedRows.Count <= 0 Then
                        MessageBox.Show("Please select option to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If
                    Dim item = CType(OptionsDGV.SelectedRows(0).DataBoundItem, ReutersBondStructure.EmbdeddedOptionDescription)
                    _currentBond.Struct.DeleteOptionItem(item)
                    CustomBondChanged = True
            End Select

            RefreshBondView()
            RecalculateCashFlows()
        End Sub

        Private Sub EditManCB_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles EditManCB.CheckedChanged
            If EditManCB.Checked Then
                OtherRulesML.ReadOnly = False
                OtherRulesML.Select()
                LockEverything()
                DisableEveryThing()
            Else
                CustomBondChanged = True
                _currentBond.Struct.Load(OtherRulesML.Text)
                EnableEveryThing()
                OtherRulesML.ReadOnly = True
                UnlockEverything()
                RefreshBondView()
                RecalculateCashFlows()
            End If
        End Sub

        Private Sub DisableEverythingComplete()
            AmortScheduleDGV.DataSource = Nothing
            CouponScheduleDGV.DataSource = Nothing
            OptionsDGV.DataSource = Nothing
            DoToggle(False, True)
        End Sub

        Private Sub EnableEverythingComplete()
            DoToggle(True, True)
        End Sub

        Private Sub EnableEveryThing()
            DoToggle(True)
        End Sub

        Private Sub DoToggle(ByVal b As Boolean, Optional ByVal complete As Boolean = False)
            For Each cntrl As Control In From elem As Control In BondsSC.Panel2.Controls
                                         Where complete OrElse Not {OtherRulesML.Name, EditManCB.Name}.Contains(elem.Name)
                cntrl.Enabled = b
            Next
        End Sub

        Private Sub DisableEveryThing()
            DoToggle(False)
        End Sub
#End Region

#Region "Chain curves TAB"
        'Private Sub RefreshChainCurves()
        '    For Each clr In Utils.GetColorList()
        '        ChainCurveColorCB.Items.Add(clr)
        '    Next
        '    ChainCurveFieldSetCB.DataSource = PortfolioManager.GetFieldLayouts
        '    ChainCurvesDGV.DataSource = PortfolioManager.CurveChainsView
        '    ClearChainCurveFields()
        'End Sub

        'Private Sub ChainCurveRandomColorButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        '    ChainCurveColorCB.SelectedIndex = New Random().NextDouble() * (ChainCurveColorCB.Items.Count - 1)
        '    ChainCurveColorPB.BackColor = Color.FromName(ChainCurveColorCB.SelectedItem)
        'End Sub

        'Private Sub CustomBondColorCB_DrawItem(ByVal sender As Object, ByVal e As DrawItemEventArgs)
        '    Dim g As Graphics = e.Graphics
        '    Dim r As Rectangle = e.Bounds
        '    If e.Index > 0 Then
        '        Dim txt As String = ChainCurveColorCB.Items(e.Index)
        '        g.DrawString(txt, ChainCurveColorCB.Font, Brushes.Black, r.X, r.Top)
        '        Dim m = g.MeasureString(txt, CustomBondColorCB.Font)
        '        Dim c As Color = Color.FromName(txt)
        '        g.FillRectangle(New SolidBrush(c), r.X + m.Width + 10, r.Y + 2, r.Width - m.Width - 15, r.Height - 6)
        '        g.DrawRectangle(New Pen(New SolidBrush(Color.Black)), r.X + m.Width + 10, r.Y + 2, r.Width - m.Width - 15, r.Height - 6)
        '    End If
        'End Sub

        'Private Sub ChainCurveColorCB_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CustomBondColorCB.SelectedIndexChanged
        '    If ChainCurveColorCB.SelectedIndex < 0 Then
        '        ChainCurveColorPB.BackColor = Color.White
        '    Else
        '        ChainCurveColorPB.BackColor = Color.FromName(ChainCurveColorCB.SelectedItem)
        '    End If
        'End Sub

        'Private Enum ChainCurveMode
        '    NewCc
        '    EditCc
        'End Enum
        'Private _ccMode As ChainCurveMode = ChainCurveMode.NewCc
        'Private _currentChainCurveId As String

        'Private Class ChainCurveDescr
        '    Public ReadOnly Id As String
        '    Public ReadOnly Ric As String
        '    Public ReadOnly Name As String
        '    Public ReadOnly Color As String
        '    Public ReadOnly Skip As String
        '    Public ReadOnly Pattern As String
        '    Public ReadOnly FsId As String

        '    Public Sub New(ByVal color As String, ByVal fsId As String, ByVal id As String, ByVal name As String, ByVal ric As String, ByVal skip As String, ByVal pattern As String)
        '        Me.Color = color
        '        Me.FsId = fsId
        '        Me.Id = id
        '        Me.Name = name
        '        Me.Ric = ric
        '        Me.Skip = skip
        '        Me.Pattern = pattern
        '    End Sub
        'End Class

        'Private _tmp As ChainCurveDescr

        'Private Sub AddNewChainCurveButton_Click(sender As Object, e As EventArgs)
        '    ErrProv.Clear()
        '    _ccMode = ChainCurveMode.NewCc
        '    _currentChainCurveId = ""
        '    ClearChainCurveFields()
        '    For i = 0 To ChainCurvesDGV.Rows.Count - 1
        '        ChainCurvesDGV.Rows(i).Selected = False
        '    Next
        'End Sub

        'Private Sub ClearChainCurveFields()
        '    ChainCurveRicTB.Text = ""
        '    ChainCurveNameTB.Text = ""
        '    ChainCurveColorCB.SelectedIndex = -1
        '    ChainCurvePatternTB.Text = ""
        '    ChainCurveSkipTB.Text = ""
        '    ChainCurveFieldSetCB.SelectedIndex = -1
        'End Sub

        'Private Sub FillChainCurveFields(ByVal chainCrv As ChainCurveSrc)
        '    ChainCurveRicTB.Text = chainCrv.Ric
        '    ChainCurveNameTB.Text = chainCrv.Name
        '    ChainCurveColorCB.SelectedItem = chainCrv.Color
        '    ChainCurvePatternTB.Text = chainCrv.Pattern
        '    ChainCurveSkipTB.Text = chainCrv.Skip

        '    Dim i = 0
        '    Do
        '        Dim item = CType(ChainCurveFieldSetCB.Items(i), IdName(Of String))
        '        If item.Id = chainCrv.FieldSetId Then
        '            ChainCurveFieldSetCB.SelectedIndex = i
        '            Exit Do
        '        End If
        '        i += 1
        '    Loop While i < ChainCurveFieldSetCB.Items.Count
        'End Sub

        'Private Sub EditChainCurveButton_Click(sender As Object, e As EventArgs)
        '    ErrProv.Clear()
        '    If ChainCurvesDGV.SelectedRows.Count = 0 Then
        '        MessageBox.Show("Please select chain curve to edit", "Edit chain curve", MessageBoxButtons.OK, MessageBoxIcon.Information)
        '        Return
        '    End If
        '    Dim chainCrv = CType(ChainCurvesDGV.SelectedRows(0).DataBoundItem, ChainCurveSrc)
        '    _currentChainCurveId = chainCrv.ID
        '    FillChainCurveFields(chainCrv)
        '    _ccMode = ChainCurveMode.EditCc
        'End Sub

        'Private Sub ChainCurvesDGV_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        '    If e.RowIndex < 0 Then Return
        '    Dim chainCrv = CType(ChainCurvesDGV.Rows(e.RowIndex).DataBoundItem, ChainCurveSrc)
        '    _currentChainCurveId = chainCrv.ID
        '    FillChainCurveFields(chainCrv)
        'End Sub

        'Private Sub SaveChainCurveChangesButton_Click(sender As Object, e As EventArgs)
        '    If ValidToSave() Then
        '        If _ccMode = ChainCurveMode.EditCc Then
        '            Dim chainCurveSrc = GetCurrentSource()
        '            PortfolioManager.UpdateSource(chainCurveSrc)
        '        Else
        '            Dim chainCurveSrc = GetCurrentSourceNew()
        '            PortfolioManager.AddSource(chainCurveSrc)
        '            _ccMode = ChainCurveMode.EditCc
        '            _currentChainCurveId = chainCurveSrc.ID
        '        End If

        '        StoreCurrentFields()
        '        RefreshChainCurves()
        '        RestoreCurrentFields()
        '        ErrProv.Clear()
        '    End If
        'End Sub

        'Private Function ValidToSave() As Boolean
        '    ErrProv.Clear()
        '    If ChainCurveRicTB.Text = "" Then
        '        ErrProv.SetError(ChainCurveRicTB, "Please enter ric")
        '        Return False
        '    End If
        '    If ChainCurveNameTB.Text = "" Then
        '        ErrProv.SetError(ChainCurveNameTB, "Please enter name")
        '        Return False
        '    End If
        '    If ChainCurveColorCB.SelectedIndex < 0 Then
        '        ErrProv.SetError(ChainCurveColorCB, "Please select color")
        '        Return False
        '    End If
        '    If ChainCurvePatternTB.Text = "" Then
        '        ErrProv.SetError(ChainCurvePatternTB, "Please enter pattern")
        '        Return False
        '    End If
        '    If ChainCurveFieldSetCB.SelectedIndex < 0 Then
        '        ErrProv.SetError(ChainCurveFieldSetCB, "Please select field set")
        '        Return False
        '    End If
        '    Return True
        'End Function

        'Private Function GetCurrentSource() As ChainCurveSrc
        '    Return New ChainCurveSrc(_currentChainCurveId,
        '                             ChainCurveColorCB.SelectedItem,
        '                             ChainCurveNameTB.Text,
        '                             ChainCurvePatternTB.Text,
        '                             ChainCurveRicTB.Text,
        '                             ChainCurveSkipTB.Text,
        '                             New FieldSet(CType(ChainCurveFieldSetCB.SelectedValue, IdName(Of String)).Id))
        'End Function

        'Private Function GetCurrentSourceNew() As ChainCurveSrc
        '    Return New ChainCurveSrc(ChainCurveColorCB.SelectedItem,
        '                             ChainCurveNameTB.Text,
        '                             ChainCurvePatternTB.Text,
        '                             ChainCurveRicTB.Text,
        '                             ChainCurveSkipTB.Text,
        '                             New FieldSet(CType(ChainCurveFieldSetCB.SelectedValue, IdName(Of String)).Id))
        'End Function

        'Private Sub DeleteChainCurveButton_Click(sender As Object, e As EventArgs)
        '    If ChainCurvesDGV.SelectedRows.Count = 0 Then
        '        MessageBox.Show("Please select chain curve to delete", "Delete chain curve", MessageBoxButtons.OK, MessageBoxIcon.Information)
        '    ElseIf MessageBox.Show("Are you sure you would like to delete the curve?", "Delete chain curve", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
        '        Dim chainCrv = CType(ChainCurvesDGV.SelectedRows(0).DataBoundItem, ChainCurveSrc)
        '        PortfolioManager.DeleteSource(chainCrv)

        '        StoreCurrentFields()
        '        RefreshChainCurves()
        '        RestoreCurrentFields()

        '        If chainCrv.ID = _currentChainCurveId Then
        '            _ccMode = ChainCurveMode.NewCc
        '            _currentChainCurveId = ""
        '        End If
        '    End If
        'End Sub

        'Private Sub RestoreCurrentFields()
        '    If _tmp Is Nothing Then Return
        '    ChainCurveRicTB.Text = _tmp.Ric
        '    ChainCurveNameTB.Text = _tmp.Name
        '    ChainCurvePatternTB.Text = _tmp.Pattern
        '    ChainCurveSkipTB.Text = _tmp.Skip

        '    If _tmp.Color IsNot Nothing Then ChainCurveColorCB.SelectedItem = _tmp.Color

        '    If _tmp.FsId IsNot Nothing Then
        '        Dim i = 0
        '        Do
        '            Dim item = CType(ChainCurveFieldSetCB.Items(i), IdName(Of String))
        '            If item.Id = _tmp.FsId Then
        '                ChainCurveFieldSetCB.SelectedIndex = i
        '                Exit Do
        '            End If
        '            i += 1
        '        Loop While i < ChainCurveFieldSetCB.Items.Count
        '    End If

        '    If _tmp.Id IsNot Nothing AndAlso ChainCurvesDGV.Rows.Count > 0 Then
        '        _currentChainCurveId = _tmp.Id
        '        Dim i = 0
        '        Do
        '            Dim item = CType(ChainCurvesDGV.Rows(i).DataBoundItem, ChainCurveSrc)
        '            If item.ID = _tmp.Id Then
        '                ChainCurvesDGV.Rows(i).Selected = True
        '            Else
        '                ChainCurvesDGV.Rows(i).Selected = False
        '            End If
        '            i += 1
        '        Loop While i < ChainCurvesDGV.Rows.Count
        '    End If
        '    _tmp = Nothing
        'End Sub

        'Private Sub StoreCurrentFields()
        '    _tmp = New ChainCurveDescr(ChainCurveColorCB.SelectedItem,
        '                               If(ChainCurveFieldSetCB.SelectedValue IsNot Nothing,
        '                                  CType(ChainCurveFieldSetCB.SelectedValue, IdName(Of String)).Id,
        '                                  Nothing),
        '                               _currentChainCurveId,
        '                               ChainCurveNameTB.Text,
        '                               ChainCurveRicTB.Text,
        '                               ChainCurveSkipTB.Text,
        '                               ChainCurvePatternTB.Text)
        'End Sub

        'Private Sub AddItemTagButton_Click(sender As Object, e As EventArgs)
        '    ChainCurvePatternTB.Text += "(?<term>.+?)"
        'End Sub
#End Region

    End Class
End Namespace