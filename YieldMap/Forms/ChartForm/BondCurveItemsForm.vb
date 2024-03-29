﻿Imports NLog
Imports Uitls
Imports YieldMap.Tools.Elements

Namespace Forms.ChartForm
    Public Class BondCurveItemsForm
        Private _curve As IChangeable
        Private Shared ReadOnly Logger As Logger = Logging.GetLogger(GetType(BondCurveItemsForm))

        Public Property Curve() As ICurve
            Get
                Return _curve
            End Get
            Set(ByVal value As ICurve)
                If _curve IsNot Nothing Then
                    RemoveHandler _curve.Cleared, AddressOf OnCurveCleared
                    RemoveHandler _curve.Updated, AddressOf OnCurveUpdated
                End If
                _curve = value
                If _curve IsNot Nothing Then
                    AddHandler _curve.Cleared, AddressOf OnCurveCleared
                    AddHandler _curve.Updated, AddressOf OnCurveUpdated
                    AddHandler _curve.UpdatedSpread, AddressOf OnCurveUpdatedSpread
                End If
            End Set
        End Property

        Private Sub OnBondRemoved(id As Long)
            Throw New NotImplementedException()
        End Sub

        Private Sub OnCurveUpdatedSpread(ByVal items As List(Of PointOfCurve), ByVal ord As IOrdinate)
            CurveUpdate()
        End Sub

        Private Sub CurveUpdate()
            GuiAsync(
                Sub()
                    If Curve IsNot Nothing Then
                        Dim currentPageName = MainTC.SelectedTab.Name
                        Dim curveSnapshot = Curve.Snapshot()
                        Dim els = curveSnapshot.EnabledElements
                        If Not els.Any Then
                            BondsDGV.DataSource = Nothing
                            MainTC.SelectedTab = CurrentTP
                            MainTC.TabPages.Remove(BondsTP)
                        Else
                            If TypeOf els.First Is BondCurveSnapshotElement Then
                                BondsDGV.DataSource = els.Cast(Of BondCurveSnapshotElement).ToList()
                            ElseIf TypeOf els.First Is SwapCurveSnapshotElement Then
                                BondsDGV.DataSource = els.Cast(Of SwapCurveSnapshotElement).ToList()
                            Else
                                BondsDGV.DataSource = els
                            End If
                        End If
                        CurrentDGV.DataSource = curveSnapshot.Current
                        FormulaTB.Text = Curve.Formula

                        For Each key In From k In curveSnapshot.Spreads.Keys
                            If curveSnapshot.Spreads(key).Any Then
                                CreatePage(key, curveSnapshot.Spreads(key))
                            Else
                                KillPage(key)
                            End If
                        Next
                        ResetEnabled()
                        OpenPage(currentPageName)
                    Else
                        BondsDGV.DataSource = Nothing
                        CurrentDGV.DataSource = Nothing
                        FormulaTB.Text = ""
                    End If
                End Sub)
        End Sub

        Private Sub OpenPage(ByVal nm As String)
            Dim i As Integer = 0
            Do
                If MainTC.TabPages(i).Name = nm Then
                    MainTC.SelectedIndex = i
                    Exit Do
                Else
                    i = i + 1
                End If
            Loop While i < MainTC.TabPages.Count
        End Sub

        Private Sub KillPage(ByVal key As IOrdinate)
            Dim i As Integer = 0
            Do
                If MainTC.TabPages(i).Tag IsNot Nothing AndAlso CType(MainTC.TabPages(i).Tag, OrdinateBase) = key Then
                    MainTC.TabPages.RemoveAt(i)
                Else
                    i = i + 1
                End If
            Loop While i < MainTC.TabPages.Count
        End Sub

        Private Sub CreatePage(key As IOrdinate, data As List(Of PointOfCurve))
            Try
                Dim i As Integer = 0
                Dim found As Boolean = False
                Do
                    If MainTC.TabPages(i).Tag IsNot Nothing AndAlso CType(MainTC.TabPages(i).Tag, OrdinateBase) = key Then
                        found = True
                        Exit Do
                    Else
                        i = i + 1
                    End If
                Loop While i < MainTC.TabPages.Count

                Dim dgvName = key.NameProperty + "_DGV"
                If Not found Then
                    Dim pg = New TabPage(key.DescrProperty) With {.Tag = key}
                    Dim dgv = New DataGridView With {.Name = dgvName}
                    dgv.DataSource = data
                    pg.Controls.Add(dgv)
                    dgv.Dock = DockStyle.Fill
                    MainTC.TabPages.Add(pg)
                Else
                    Dim pg = MainTC.TabPages(i)
                    Dim dgv = CType(pg.Controls(dgvName), DataGridView)
                    dgv.DataSource = data
                End If
            Catch ex As Exception
                Logger.ErrorException("Failed to add page", ex)
                Logger.Error("Exception = {0}", ex.ToString())
            End Try
        End Sub

        Private Sub ResetEnabled()
            AddItemsTSB.Enabled = MainTC.SelectedTab.Name = BondsTP.Name AndAlso _curve.DisabledElements.Any AndAlso Not Curve.IsSynthetic
            RemoveItemsTSB.Enabled = MainTC.SelectedTab.Name = BondsTP.Name AndAlso Not Curve.IsSynthetic
        End Sub

        Private Sub OnCurveUpdated(ByVal obj As List(Of PointOfCurve))
            CurveUpdate()
        End Sub

        Private Sub OnCurveCleared()
            Close()
        End Sub

        Private Sub BondCurveItemsForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
            Curve = Nothing
        End Sub

        Private Sub MainTC_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles MainTC.SelectedIndexChanged
            ResetEnabled()
        End Sub

        Private Sub AddItemsTSB_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddItemsTSB.Click
            Dim frm As New AddBondCurveItemsForm
            frm.Curve = Curve
            frm.ShowDialog()
        End Sub

        Private Sub RemoveItemsTSB_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveItemsTSB.Click
            If BondsDGV.SelectedRows.Count <= 0 Then Return
            If BondsDGV.Rows.Count <= 2 Then
                MessageBox.Show("There must be at least two points in curve", "Cannot remove point", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim elements = (From elem As DataGridViewRow In BondsDGV.SelectedRows Select CType(elem.DataBoundItem, BondCurveSnapshotElement).RIC).ToList()
            _curve.Disable(elements)
        End Sub

        Private Sub BondCurveItemsForm_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
            CurveUpdate()
        End Sub
    End Class
End Namespace