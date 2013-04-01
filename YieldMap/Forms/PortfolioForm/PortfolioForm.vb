﻿Imports DbManager
Imports DbManager.Bonds
Imports System.Runtime.InteropServices
Imports NLog
Imports Uitls

Namespace Forms.PortfolioForm

    Public Class PortfolioForm
        Private Shared ReadOnly PortfolioManager As IPortfolioManager = DbManager.PortfolioManager.Instance()
        Private Shared ReadOnly Logger As Logger = Logging.GetLogger(GetType(PortfolioForm))
        Private WithEvents _loader As IBondsLoader = BondsLoader.Instance

        Private _dragNode As TreeNode
        Private _flag As Boolean

        Private _currentItem As Portfolio
        Private Property CurrentItem As Portfolio
            Get
                Return _currentItem
            End Get
            Set(ByVal value As Portfolio)
                _currentItem = value
                RefreshPortfolioData()
            End Set
        End Property


        Private Sub PortfolioForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            BondsTableView.DataSource = _loader.GetBondsTable()
            If PortfolioTree.ImageList Is Nothing Then
                PortfolioTree.ImageList = New ImageList
                PortfolioTree.ImageList.Images.Add("folder", My.Resources.folder)
                PortfolioTree.ImageList.Images.Add("portfolio", My.Resources.briefcase)
            End If
            RefreshPortfolioTree()
            RefreshChainsLists()
            RefreshFieldsList()
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

#Region "Portfolio TAB"

        'Private Shared ReadOnly ErrorMessages As New List(Of String)


        Private Sub PortSourcesCheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ChainsCB.CheckedChanged, ListsCB.CheckedChanged
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
                Dim descr = PortfolioManager.GetPortfolioStructure(CurrentItem.Id)
                PortfolioChainsListsGrid.DataSource = descr.Sources(
                    If(ChainsCB.Checked, PortfolioStructure.Chain, 0) Or
                    If(ListsCB.Checked, PortfolioStructure.List, 0))

                PortfolioItemsGrid.DataSource = descr.Rics(AllRB.Checked)
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
                Dim portSelected  = CurrentItem IsNot Nothing AndAlso Not CurrentItem.IsFolder
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
                PortTreeCM.Tag = Nothing
                PortTreeCM.Show(PortfolioTree, e.Location)
            End If
        End Sub

        Private Sub AddToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddToolStripMenuItem.Click
            Dim node = TryCast(PortTreeCM.Tag, TreeNode)

            Dim adder As New AddPortfolioForm With {
                .EditMode = False,
                .ItIsPortfolio = True
            }

            Dim theId As String

            If node IsNot Nothing Then
                PortfolioTree.SelectedNode = node
                Dim descr = TryCast(node.Tag, Portfolio)
                If descr Is Nothing Then Return
                theId = descr.Id
            End If

            If adder.ShowDialog() = DialogResult.OK Then
                Dim newId As Long
                If adder.ItsFolder.Checked Then
                    newId = PortfolioManager.AddFolder(adder.NewName.Text, theId)
                Else
                    newId = PortfolioManager.AddPortfolio(adder.NewName.Text, theId)
                End If
                RefreshPortfolioTree(newId)
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

            Dim item = CType(PortfolioChainsListsGrid.SelectedRows(0).DataBoundItem, PortfolioSource)
            If item Is Nothing Then Exit Sub
            Try
                CurrentItem.DeleteSource(item)
                RefreshPortfolioData()
            Catch ex As Exception
                Logger.ErrorException("Failed to delete selected source", ex)
                Logger.Error("Exception = {0}", ex.ToString())
                MessageBox.Show("Failed to delete selected source", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
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

            If a.ShowDialog() = DialogResult.OK Then
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
            AddItemsButton.Enabled = Not ChainsButton.Checked
            DeleteItemsButton.Enabled = Not ChainsButton.Checked
            For Each col As DataGridViewColumn In ChainsListsGrid.Columns
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            Next
            If selectedSource IsNot Nothing Then
                ' todo select it
            End If
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
                PortfolioManager.UpdateSource(frm.Src)
                RefreshChainsLists(frm.Src)
            End If
        End Sub

        Private Sub DeleteCLButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteCLButton.Click, DeleteCLTSMI.Click
            If ChainsListsGrid.SelectedRows.Count <> 1 Then
                MessageBox.Show("Please select chain or list to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim selectedItem = CType(ChainsListsGrid.SelectedRows.Item(0).DataBoundItem, Source)
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
            Dim elem = ChainsListsGrid.Rows(e.RowIndex).DataBoundItem
            Dim chain As Source = TryCast(elem, Source)
            If chain Is Nothing Then Return
            If e.Button = MouseButtons.Left Then
                ChainListItemsGrid.DataSource = chain.GetDefaultRicsView()
                For Each col As DataGridViewColumn In ChainListItemsGrid.Columns
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                Next
            Else
                ChainsListsCMS.Show(ChainsListsGrid, e.Location)
            End If
        End Sub

        Private Sub ReloadChainButtin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ReloadChainButton.Click, ReloadCLTSMI.Click
            If ChainsListsGrid.SelectedRows.Count <> 1 Then
                MessageBox.Show("Please select chain or list to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim selectedItem = CType(ChainsListsGrid.SelectedRows.Item(0).DataBoundItem, Source)
            If Not TypeOf selectedItem Is Chain Then Return

            _loader.LoadChain(CType(selectedItem, Chain).ChainRic)
        End Sub

        Private Sub BondsLoaderFinished(ByVal evt As ProgressEvent) Handles _loader.Progress
            Logger.Info("Got message {0}", evt.Msg)
            If evt.Log.Success() Then

                GuiAsync(AddressOf RefreshChainsLists)
            ElseIf evt.Log.Failed() Then
                MessageBox.Show(evt.Log.Entries.Select(Function(item) item.Msg).Aggregate(Function(str, item) str + Environment.NewLine + item),
                                "Errors occured", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                Dim item = TryCast(FieldLayoutsListBox.Items(FieldLayoutsListBox.SelectedIndex), Fields)
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
    End Class
End Namespace