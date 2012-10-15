﻿Namespace Forms.ChartForm
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class GraphForm
        Inherits System.Windows.Forms.Form

        'Форма переопределяет dispose для очистки списка компонентов.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Является обязательной для конструктора форм Windows Forms
        Private components As System.ComponentModel.IContainer

        'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
        'Для ее изменения используйте конструктор форм Windows Form.  
        'Не изменяйте ее в редакторе исходного кода.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim ChartArea3 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
            Dim Legend3 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
            Me.TheStatusStrip = New System.Windows.Forms.StatusStrip()
            Me.StatusMessage = New System.Windows.Forms.ToolStripStatusLabel()
            Me.TheToolStrip = New System.Windows.Forms.ToolStrip()
            Me.ZoomAllButton = New System.Windows.Forms.ToolStripButton()
            Me.ZoomCustomButton = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
            Me.PortfolioTSSB = New System.Windows.Forms.ToolStripSplitButton()
            Me.portfolioSelectTSCB = New System.Windows.Forms.ToolStripComboBox()
            Me.CurvesToolStripSplitButton = New System.Windows.Forms.ToolStripSplitButton()
            Me.AddBondCurveTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
            Me.RubIRSTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.RubCCSTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.NDFTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
            Me.ShowLegendTSB = New System.Windows.Forms.ToolStripButton()
            Me.ShowLabelsTSB = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
            Me.AsTableTSB = New System.Windows.Forms.ToolStripButton()
            Me.GraphToolTip = New System.Windows.Forms.ToolTip(Me.components)
            Me.BondCMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.MainInfoLine1TSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.MainInfoLine2TSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.ExtInfoTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.ShowHistoryTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.RelatedQuoteTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.MainTableLayout = New System.Windows.Forms.TableLayoutPanel()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.DurLabel = New System.Windows.Forms.Label()
            Me.SpreadLabel = New System.Windows.Forms.Label()
            Me.DscrLabel = New System.Windows.Forms.Label()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.DatLabel = New System.Windows.Forms.Label()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.YldLabel = New System.Windows.Forms.Label()
            Me.ZSpreadLabel = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.LinkSpreadLabel = New System.Windows.Forms.LinkLabel()
            Me.ZSpreadLinkLabel = New System.Windows.Forms.LinkLabel()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.ASWLinkLabel = New System.Windows.Forms.LinkLabel()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.MatLabel = New System.Windows.Forms.Label()
            Me.ConvLabel = New System.Windows.Forms.Label()
            Me.TheChart = New System.Windows.Forms.DataVisualization.Charting.Chart()
            Me.ChartCMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.CopyToClipboardTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.HistoryCMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.RemoveHistoryTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.ResizePictureBox = New System.Windows.Forms.PictureBox()
            Me.MoneyCurveCMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.MMNameTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.DeleteMMCurveTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
            Me.BrokerTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.QuoteTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.FitTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.BootstrapTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator()
            Me.SelDateTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.SpreadCMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.DurConvCMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.ModifiedTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.MacaleyTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.EffectiveTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.BidAskCMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.RemoveBidAskTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.YAxisCMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.RelatedChartTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.BondDescriptionTSMI = New System.Windows.Forms.ToolStripMenuItem()
            Me.TheStatusStrip.SuspendLayout()
            Me.TheToolStrip.SuspendLayout()
            Me.BondCMS.SuspendLayout()
            Me.MainTableLayout.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            CType(Me.TheChart, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ChartCMS.SuspendLayout()
            Me.HistoryCMS.SuspendLayout()
            CType(Me.ResizePictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.MoneyCurveCMS.SuspendLayout()
            Me.DurConvCMS.SuspendLayout()
            Me.BidAskCMS.SuspendLayout()
            Me.SuspendLayout()
            '
            'TheStatusStrip
            '
            Me.TheStatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusMessage})
            Me.TheStatusStrip.Location = New System.Drawing.Point(0, 540)
            Me.TheStatusStrip.Name = "TheStatusStrip"
            Me.TheStatusStrip.Size = New System.Drawing.Size(784, 22)
            Me.TheStatusStrip.TabIndex = 0
            Me.TheStatusStrip.Text = "StatusStrip1"
            '
            'StatusMessage
            '
            Me.StatusMessage.Name = "StatusMessage"
            Me.StatusMessage.Size = New System.Drawing.Size(0, 17)
            '
            'TheToolStrip
            '
            Me.TheToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ZoomAllButton, Me.ZoomCustomButton, Me.ToolStripSeparator3, Me.PortfolioTSSB, Me.CurvesToolStripSplitButton, Me.ToolStripSeparator2, Me.ShowLegendTSB, Me.ShowLabelsTSB, Me.ToolStripSeparator6, Me.AsTableTSB})
            Me.TheToolStrip.Location = New System.Drawing.Point(0, 0)
            Me.TheToolStrip.Name = "TheToolStrip"
            Me.TheToolStrip.Size = New System.Drawing.Size(784, 25)
            Me.TheToolStrip.TabIndex = 1
            Me.TheToolStrip.Text = "Portfolio"
            '
            'ZoomAllButton
            '
            Me.ZoomAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ZoomAllButton.Image = Global.YieldMap.My.Resources.Resources.view_1_1
            Me.ZoomAllButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ZoomAllButton.Name = "ZoomAllButton"
            Me.ZoomAllButton.Size = New System.Drawing.Size(23, 22)
            Me.ZoomAllButton.Text = "Reset zoom"
            Me.ZoomAllButton.ToolTipText = "Reset zoom"
            '
            'ZoomCustomButton
            '
            Me.ZoomCustomButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ZoomCustomButton.Image = Global.YieldMap.My.Resources.Resources.fit_to_size
            Me.ZoomCustomButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ZoomCustomButton.Name = "ZoomCustomButton"
            Me.ZoomCustomButton.Size = New System.Drawing.Size(23, 22)
            Me.ZoomCustomButton.Text = "ToolStripButton4"
            Me.ZoomCustomButton.ToolTipText = "Resize"
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
            '
            'PortfolioTSSB
            '
            Me.PortfolioTSSB.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.portfolioSelectTSCB})
            Me.PortfolioTSSB.Image = Global.YieldMap.My.Resources.Resources.folder
            Me.PortfolioTSSB.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.PortfolioTSSB.Name = "PortfolioTSSB"
            Me.PortfolioTSSB.Size = New System.Drawing.Size(79, 22)
            Me.PortfolioTSSB.Text = "Portfolio"
            '
            'portfolioSelectTSCB
            '
            Me.portfolioSelectTSCB.Enabled = False
            Me.portfolioSelectTSCB.Name = "portfolioSelectTSCB"
            Me.portfolioSelectTSCB.Size = New System.Drawing.Size(121, 21)
            '
            'CurvesToolStripSplitButton
            '
            Me.CurvesToolStripSplitButton.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddBondCurveTSMI, Me.ToolStripSeparator9, Me.ToolStripMenuItem1})
            Me.CurvesToolStripSplitButton.Image = Global.YieldMap.My.Resources.Resources.graph_edge_curved
            Me.CurvesToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.CurvesToolStripSplitButton.Name = "CurvesToolStripSplitButton"
            Me.CurvesToolStripSplitButton.Size = New System.Drawing.Size(73, 22)
            Me.CurvesToolStripSplitButton.Text = "Curves"
            '
            'AddBondCurveTSMI
            '
            Me.AddBondCurveTSMI.Name = "AddBondCurveTSMI"
            Me.AddBondCurveTSMI.Size = New System.Drawing.Size(173, 22)
            Me.AddBondCurveTSMI.Text = "Add bond curve..."
            '
            'ToolStripSeparator9
            '
            Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
            Me.ToolStripSeparator9.Size = New System.Drawing.Size(170, 6)
            '
            'ToolStripMenuItem1
            '
            Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RubIRSTSMI, Me.RubCCSTSMI, Me.NDFTSMI})
            Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
            Me.ToolStripMenuItem1.Size = New System.Drawing.Size(173, 22)
            Me.ToolStripMenuItem1.Text = "IR curves"
            '
            'RubIRSTSMI
            '
            Me.RubIRSTSMI.Name = "RubIRSTSMI"
            Me.RubIRSTSMI.Size = New System.Drawing.Size(127, 22)
            Me.RubIRSTSMI.Text = "Rub IRS"
            '
            'RubCCSTSMI
            '
            Me.RubCCSTSMI.Name = "RubCCSTSMI"
            Me.RubCCSTSMI.Size = New System.Drawing.Size(127, 22)
            Me.RubCCSTSMI.Text = "Rub CCS"
            '
            'NDFTSMI
            '
            Me.NDFTSMI.Name = "NDFTSMI"
            Me.NDFTSMI.Size = New System.Drawing.Size(127, 22)
            Me.NDFTSMI.Text = "NDF"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'ShowLegendTSB
            '
            Me.ShowLegendTSB.CheckOnClick = True
            Me.ShowLegendTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ShowLegendTSB.Image = Global.YieldMap.My.Resources.Resources.text_marked
            Me.ShowLegendTSB.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ShowLegendTSB.Name = "ShowLegendTSB"
            Me.ShowLegendTSB.Size = New System.Drawing.Size(23, 22)
            Me.ShowLegendTSB.Text = "Show legend"
            '
            'ShowLabelsTSB
            '
            Me.ShowLabelsTSB.CheckOnClick = True
            Me.ShowLabelsTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.ShowLabelsTSB.Image = Global.YieldMap.My.Resources.Resources.Labels
            Me.ShowLabelsTSB.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ShowLabelsTSB.Name = "ShowLabelsTSB"
            Me.ShowLabelsTSB.Size = New System.Drawing.Size(23, 22)
            Me.ShowLabelsTSB.Text = "Show labels"
            '
            'ToolStripSeparator6
            '
            Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
            Me.ToolStripSeparator6.Size = New System.Drawing.Size(6, 25)
            '
            'AsTableTSB
            '
            Me.AsTableTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.AsTableTSB.Image = Global.YieldMap.My.Resources.Resources.table2_run_small
            Me.AsTableTSB.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.AsTableTSB.Name = "AsTableTSB"
            Me.AsTableTSB.Size = New System.Drawing.Size(23, 22)
            Me.AsTableTSB.Text = "AsTable"
            Me.AsTableTSB.ToolTipText = "Show as table"
            '
            'BondCMS
            '
            Me.BondCMS.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MainInfoLine1TSMI, Me.MainInfoLine2TSMI, Me.ExtInfoTSMI, Me.ShowHistoryTSMI, Me.ToolStripSeparator1, Me.BondDescriptionTSMI, Me.RelatedQuoteTSMI, Me.RelatedChartTSMI})
            Me.BondCMS.Name = "BondContextMenuStrip"
            Me.BondCMS.Size = New System.Drawing.Size(165, 186)
            '
            'MainInfoLine1TSMI
            '
            Me.MainInfoLine1TSMI.Name = "MainInfoLine1TSMI"
            Me.MainInfoLine1TSMI.Size = New System.Drawing.Size(164, 22)
            Me.MainInfoLine1TSMI.Text = "Description"
            '
            'MainInfoLine2TSMI
            '
            Me.MainInfoLine2TSMI.Name = "MainInfoLine2TSMI"
            Me.MainInfoLine2TSMI.Size = New System.Drawing.Size(164, 22)
            Me.MainInfoLine2TSMI.Text = "Current quote"
            '
            'ExtInfoTSMI
            '
            Me.ExtInfoTSMI.Enabled = False
            Me.ExtInfoTSMI.Name = "ExtInfoTSMI"
            Me.ExtInfoTSMI.Size = New System.Drawing.Size(164, 22)
            Me.ExtInfoTSMI.Text = "Loading info"
            '
            'ShowHistoryTSMI
            '
            Me.ShowHistoryTSMI.Name = "ShowHistoryTSMI"
            Me.ShowHistoryTSMI.Size = New System.Drawing.Size(164, 22)
            Me.ShowHistoryTSMI.Text = "Show history"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(161, 6)
            '
            'RelatedQuoteTSMI
            '
            Me.RelatedQuoteTSMI.Name = "RelatedQuoteTSMI"
            Me.RelatedQuoteTSMI.Size = New System.Drawing.Size(164, 22)
            Me.RelatedQuoteTSMI.Text = "Related quote"
            '
            'MainTableLayout
            '
            Me.MainTableLayout.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.MainTableLayout.ColumnCount = 1
            Me.MainTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.MainTableLayout.Controls.Add(Me.TableLayoutPanel1, 0, 0)
            Me.MainTableLayout.Controls.Add(Me.TheChart, 0, 1)
            Me.MainTableLayout.Location = New System.Drawing.Point(0, 28)
            Me.MainTableLayout.Name = "MainTableLayout"
            Me.MainTableLayout.RowCount = 2
            Me.MainTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.MainTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.MainTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.MainTableLayout.Size = New System.Drawing.Size(784, 509)
            Me.MainTableLayout.TabIndex = 4
            '
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TableLayoutPanel1.AutoSize = True
            Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.TableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control
            Me.TableLayoutPanel1.ColumnCount = 6
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel1.Controls.Add(Me.Label3, 2, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.DurLabel, 3, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.SpreadLabel, 5, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.DscrLabel, 1, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Label6, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.DatLabel, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Label1, 2, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.YldLabel, 3, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.ZSpreadLabel, 5, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.LinkSpreadLabel, 4, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ZSpreadLinkLabel, 4, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Label4, 2, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.ASWLinkLabel, 4, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.Label5, 0, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.MatLabel, 1, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.ConvLabel, 3, 2)
            Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 3
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanel1.Size = New System.Drawing.Size(778, 60)
            Me.TableLayoutPanel1.TabIndex = 4
            '
            'Label3
            '
            Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.Label3.Location = New System.Drawing.Point(256, 3)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(55, 13)
            Me.Label3.TabIndex = 0
            Me.Label3.Text = "Duration"
            '
            'DurLabel
            '
            Me.DurLabel.AutoSize = True
            Me.DurLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.DurLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.DurLabel.Location = New System.Drawing.Point(324, 3)
            Me.DurLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.DurLabel.Name = "DurLabel"
            Me.DurLabel.Size = New System.Drawing.Size(189, 14)
            Me.DurLabel.TabIndex = 0
            '
            'SpreadLabel
            '
            Me.SpreadLabel.AutoSize = True
            Me.SpreadLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SpreadLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.SpreadLabel.Location = New System.Drawing.Point(584, 3)
            Me.SpreadLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.SpreadLabel.Name = "SpreadLabel"
            Me.SpreadLabel.Size = New System.Drawing.Size(191, 14)
            Me.SpreadLabel.TabIndex = 0
            '
            'DscrLabel
            '
            Me.DscrLabel.AutoSize = True
            Me.DscrLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.DscrLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.DscrLabel.Location = New System.Drawing.Point(61, 23)
            Me.DscrLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.DscrLabel.Name = "DscrLabel"
            Me.DscrLabel.Size = New System.Drawing.Size(189, 14)
            Me.DscrLabel.TabIndex = 0
            '
            'Label6
            '
            Me.Label6.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.Label6.AutoSize = True
            Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.Label6.Location = New System.Drawing.Point(3, 3)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(34, 13)
            Me.Label6.TabIndex = 0
            Me.Label6.Text = "Date"
            '
            'DatLabel
            '
            Me.DatLabel.AutoSize = True
            Me.DatLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.DatLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.DatLabel.Location = New System.Drawing.Point(61, 3)
            Me.DatLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.DatLabel.Name = "DatLabel"
            Me.DatLabel.Size = New System.Drawing.Size(189, 14)
            Me.DatLabel.TabIndex = 0
            '
            'Label1
            '
            Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.Label1.Location = New System.Drawing.Point(256, 23)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(35, 13)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Yield"
            '
            'YldLabel
            '
            Me.YldLabel.AutoSize = True
            Me.YldLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.YldLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.YldLabel.Location = New System.Drawing.Point(324, 23)
            Me.YldLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.YldLabel.Name = "YldLabel"
            Me.YldLabel.Size = New System.Drawing.Size(189, 14)
            Me.YldLabel.TabIndex = 0
            '
            'ZSpreadLabel
            '
            Me.ZSpreadLabel.AutoSize = True
            Me.ZSpreadLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.ZSpreadLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.ZSpreadLabel.Location = New System.Drawing.Point(584, 23)
            Me.ZSpreadLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.ZSpreadLabel.Name = "ZSpreadLabel"
            Me.ZSpreadLabel.Size = New System.Drawing.Size(191, 14)
            Me.ZSpreadLabel.TabIndex = 0
            '
            'Label2
            '
            Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.Label2.AutoSize = True
            Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.Label2.Location = New System.Drawing.Point(3, 23)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(40, 13)
            Me.Label2.TabIndex = 0
            Me.Label2.Text = "Descr"
            '
            'LinkSpreadLabel
            '
            Me.LinkSpreadLabel.AutoSize = True
            Me.LinkSpreadLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.LinkSpreadLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
            Me.LinkSpreadLabel.LinkColor = System.Drawing.Color.Navy
            Me.LinkSpreadLabel.LinkVisited = True
            Me.LinkSpreadLabel.Location = New System.Drawing.Point(519, 3)
            Me.LinkSpreadLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
            Me.LinkSpreadLabel.Name = "LinkSpreadLabel"
            Me.LinkSpreadLabel.Size = New System.Drawing.Size(47, 13)
            Me.LinkSpreadLabel.TabIndex = 1
            Me.LinkSpreadLabel.TabStop = True
            Me.LinkSpreadLabel.Text = "Spread"
            Me.LinkSpreadLabel.VisitedLinkColor = System.Drawing.Color.Navy
            '
            'ZSpreadLinkLabel
            '
            Me.ZSpreadLinkLabel.AutoSize = True
            Me.ZSpreadLinkLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.ZSpreadLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
            Me.ZSpreadLinkLabel.LinkColor = System.Drawing.Color.Navy
            Me.ZSpreadLinkLabel.LinkVisited = True
            Me.ZSpreadLinkLabel.Location = New System.Drawing.Point(519, 23)
            Me.ZSpreadLinkLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
            Me.ZSpreadLinkLabel.Name = "ZSpreadLinkLabel"
            Me.ZSpreadLinkLabel.Size = New System.Drawing.Size(59, 13)
            Me.ZSpreadLinkLabel.TabIndex = 1
            Me.ZSpreadLinkLabel.TabStop = True
            Me.ZSpreadLinkLabel.Text = "Z-Spread"
            Me.ZSpreadLinkLabel.VisitedLinkColor = System.Drawing.Color.Navy
            '
            'Label4
            '
            Me.Label4.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.Label4.AutoSize = True
            Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.Label4.Location = New System.Drawing.Point(256, 43)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(62, 13)
            Me.Label4.TabIndex = 0
            Me.Label4.Text = "Convexity"
            '
            'ASWLinkLabel
            '
            Me.ASWLinkLabel.AutoSize = True
            Me.ASWLinkLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.ASWLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline
            Me.ASWLinkLabel.LinkColor = System.Drawing.Color.Navy
            Me.ASWLinkLabel.LinkVisited = True
            Me.ASWLinkLabel.Location = New System.Drawing.Point(519, 43)
            Me.ASWLinkLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
            Me.ASWLinkLabel.Name = "ASWLinkLabel"
            Me.ASWLinkLabel.Size = New System.Drawing.Size(35, 13)
            Me.ASWLinkLabel.TabIndex = 1
            Me.ASWLinkLabel.TabStop = True
            Me.ASWLinkLabel.Text = "ASW"
            Me.ASWLinkLabel.Visible = False
            Me.ASWLinkLabel.VisitedLinkColor = System.Drawing.Color.Navy
            '
            'Label5
            '
            Me.Label5.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.Label5.AutoSize = True
            Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.Label5.Location = New System.Drawing.Point(3, 43)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(52, 13)
            Me.Label5.TabIndex = 0
            Me.Label5.Text = "Maturity"
            '
            'MatLabel
            '
            Me.MatLabel.AutoSize = True
            Me.MatLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.MatLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.MatLabel.Location = New System.Drawing.Point(61, 43)
            Me.MatLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.MatLabel.Name = "MatLabel"
            Me.MatLabel.Size = New System.Drawing.Size(189, 14)
            Me.MatLabel.TabIndex = 2
            '
            'ConvLabel
            '
            Me.ConvLabel.AutoSize = True
            Me.ConvLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.ConvLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
            Me.ConvLabel.Location = New System.Drawing.Point(324, 43)
            Me.ConvLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.ConvLabel.Name = "ConvLabel"
            Me.ConvLabel.Size = New System.Drawing.Size(189, 14)
            Me.ConvLabel.TabIndex = 2
            '
            'TheChart
            '
            Me.TheChart.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            ChartArea3.Name = "ChartArea1"
            Me.TheChart.ChartAreas.Add(ChartArea3)
            Legend3.Enabled = False
            Legend3.Name = "Legend1"
            Me.TheChart.Legends.Add(Legend3)
            Me.TheChart.Location = New System.Drawing.Point(3, 69)
            Me.TheChart.Name = "TheChart"
            Me.TheChart.Size = New System.Drawing.Size(778, 437)
            Me.TheChart.TabIndex = 3
            Me.TheChart.Text = "Chart1"
            '
            'ChartCMS
            '
            Me.ChartCMS.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyToClipboardTSMI})
            Me.ChartCMS.Name = "ChartCMS"
            Me.ChartCMS.Size = New System.Drawing.Size(170, 26)
            '
            'CopyToClipboardTSMI
            '
            Me.CopyToClipboardTSMI.Name = "CopyToClipboardTSMI"
            Me.CopyToClipboardTSMI.Size = New System.Drawing.Size(169, 22)
            Me.CopyToClipboardTSMI.Text = "Copy to clipboard"
            '
            'HistoryCMS
            '
            Me.HistoryCMS.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RemoveHistoryTSMI})
            Me.HistoryCMS.Name = "HistoryCMS"
            Me.HistoryCMS.Size = New System.Drawing.Size(161, 26)
            '
            'RemoveHistoryTSMI
            '
            Me.RemoveHistoryTSMI.Name = "RemoveHistoryTSMI"
            Me.RemoveHistoryTSMI.Size = New System.Drawing.Size(160, 22)
            Me.RemoveHistoryTSMI.Text = "Remove history"
            '
            'ResizePictureBox
            '
            Me.ResizePictureBox.Location = New System.Drawing.Point(509, 400)
            Me.ResizePictureBox.Name = "ResizePictureBox"
            Me.ResizePictureBox.Size = New System.Drawing.Size(181, 106)
            Me.ResizePictureBox.TabIndex = 4
            Me.ResizePictureBox.TabStop = False
            Me.ResizePictureBox.Visible = False
            '
            'MoneyCurveCMS
            '
            Me.MoneyCurveCMS.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MMNameTSMI, Me.DeleteMMCurveTSMI, Me.ToolStripSeparator10, Me.BrokerTSMI, Me.QuoteTSMI, Me.FitTSMI, Me.BootstrapTSMI, Me.ToolStripSeparator11, Me.SelDateTSMI})
            Me.MoneyCurveCMS.Name = "MoneyCurveCMS"
            Me.MoneyCurveCMS.Size = New System.Drawing.Size(153, 192)
            '
            'MMNameTSMI
            '
            Me.MMNameTSMI.Enabled = False
            Me.MMNameTSMI.Name = "MMNameTSMI"
            Me.MMNameTSMI.Size = New System.Drawing.Size(152, 22)
            Me.MMNameTSMI.Text = "Name"
            '
            'DeleteMMCurveTSMI
            '
            Me.DeleteMMCurveTSMI.Name = "DeleteMMCurveTSMI"
            Me.DeleteMMCurveTSMI.Size = New System.Drawing.Size(152, 22)
            Me.DeleteMMCurveTSMI.Text = "Delete curve"
            '
            'ToolStripSeparator10
            '
            Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
            Me.ToolStripSeparator10.Size = New System.Drawing.Size(149, 6)
            '
            'BrokerTSMI
            '
            Me.BrokerTSMI.Name = "BrokerTSMI"
            Me.BrokerTSMI.Size = New System.Drawing.Size(152, 22)
            Me.BrokerTSMI.Text = "Broker"
            '
            'QuoteTSMI
            '
            Me.QuoteTSMI.Name = "QuoteTSMI"
            Me.QuoteTSMI.Size = New System.Drawing.Size(152, 22)
            Me.QuoteTSMI.Text = "Quote"
            '
            'FitTSMI
            '
            Me.FitTSMI.Name = "FitTSMI"
            Me.FitTSMI.Size = New System.Drawing.Size(152, 22)
            Me.FitTSMI.Text = "Interpolation"
            '
            'BootstrapTSMI
            '
            Me.BootstrapTSMI.CheckOnClick = True
            Me.BootstrapTSMI.Name = "BootstrapTSMI"
            Me.BootstrapTSMI.Size = New System.Drawing.Size(152, 22)
            Me.BootstrapTSMI.Text = "Bootstrap"
            '
            'ToolStripSeparator11
            '
            Me.ToolStripSeparator11.Name = "ToolStripSeparator11"
            Me.ToolStripSeparator11.Size = New System.Drawing.Size(149, 6)
            '
            'SelDateTSMI
            '
            Me.SelDateTSMI.Name = "SelDateTSMI"
            Me.SelDateTSMI.Size = New System.Drawing.Size(152, 22)
            Me.SelDateTSMI.Text = "Select date..."
            '
            'SpreadCMS
            '
            Me.SpreadCMS.Name = "SpreadCMS"
            Me.SpreadCMS.Size = New System.Drawing.Size(61, 4)
            '
            'DurConvCMS
            '
            Me.DurConvCMS.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ModifiedTSMI, Me.MacaleyTSMI, Me.EffectiveTSMI})
            Me.DurConvCMS.Name = "DurConvCMS"
            Me.DurConvCMS.Size = New System.Drawing.Size(129, 70)
            '
            'ModifiedTSMI
            '
            Me.ModifiedTSMI.Name = "ModifiedTSMI"
            Me.ModifiedTSMI.Size = New System.Drawing.Size(128, 22)
            Me.ModifiedTSMI.Text = "Modified"
            '
            'MacaleyTSMI
            '
            Me.MacaleyTSMI.Name = "MacaleyTSMI"
            Me.MacaleyTSMI.Size = New System.Drawing.Size(128, 22)
            Me.MacaleyTSMI.Text = "Macaley"
            '
            'EffectiveTSMI
            '
            Me.EffectiveTSMI.Name = "EffectiveTSMI"
            Me.EffectiveTSMI.Size = New System.Drawing.Size(128, 22)
            Me.EffectiveTSMI.Text = "Effective"
            '
            'BidAskCMS
            '
            Me.BidAskCMS.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RemoveBidAskTSMI})
            Me.BidAskCMS.Name = "BidAskCMS"
            Me.BidAskCMS.Size = New System.Drawing.Size(162, 26)
            '
            'RemoveBidAskTSMI
            '
            Me.RemoveBidAskTSMI.Name = "RemoveBidAskTSMI"
            Me.RemoveBidAskTSMI.Size = New System.Drawing.Size(161, 22)
            Me.RemoveBidAskTSMI.Text = "Remove bid-ask"
            '
            'YAxisCMS
            '
            Me.YAxisCMS.Name = "YAxisCMS"
            Me.YAxisCMS.Size = New System.Drawing.Size(61, 4)
            '
            'RelatedChartTSMI
            '
            Me.RelatedChartTSMI.Name = "RelatedChartTSMI"
            Me.RelatedChartTSMI.Size = New System.Drawing.Size(164, 22)
            Me.RelatedChartTSMI.Text = "Related chart"
            '
            'BondDescriptionTSMI
            '
            Me.BondDescriptionTSMI.Name = "BondDescriptionTSMI"
            Me.BondDescriptionTSMI.Size = New System.Drawing.Size(164, 22)
            Me.BondDescriptionTSMI.Text = "Bond description"
            '
            'GraphForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(784, 562)
            Me.Controls.Add(Me.ResizePictureBox)
            Me.Controls.Add(Me.MainTableLayout)
            Me.Controls.Add(Me.TheToolStrip)
            Me.Controls.Add(Me.TheStatusStrip)
            Me.MinimumSize = New System.Drawing.Size(400, 300)
            Me.Name = "GraphForm"
            Me.Text = "Yield map chart"
            Me.TheStatusStrip.ResumeLayout(False)
            Me.TheStatusStrip.PerformLayout()
            Me.TheToolStrip.ResumeLayout(False)
            Me.TheToolStrip.PerformLayout()
            Me.BondCMS.ResumeLayout(False)
            Me.MainTableLayout.ResumeLayout(False)
            Me.MainTableLayout.PerformLayout()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            CType(Me.TheChart, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ChartCMS.ResumeLayout(False)
            Me.HistoryCMS.ResumeLayout(False)
            CType(Me.ResizePictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.MoneyCurveCMS.ResumeLayout(False)
            Me.DurConvCMS.ResumeLayout(False)
            Me.BidAskCMS.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents TheStatusStrip As System.Windows.Forms.StatusStrip
        Friend WithEvents TheToolStrip As System.Windows.Forms.ToolStrip
        Friend WithEvents StatusMessage As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ZoomAllButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents ZoomCustomButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents GraphToolTip As System.Windows.Forms.ToolTip
        Friend WithEvents PortfolioTSSB As System.Windows.Forms.ToolStripSplitButton
        Friend WithEvents CurvesToolStripSplitButton As System.Windows.Forms.ToolStripSplitButton
        Friend WithEvents BondCMS As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents AddBondCurveTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ExtInfoTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents MainInfoLine2TSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ShowLabelsTSB As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents MainInfoLine1TSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ShowHistoryTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents portfolioSelectTSCB As System.Windows.Forms.ToolStripComboBox
        Friend WithEvents MainTableLayout As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents TheChart As System.Windows.Forms.DataVisualization.Charting.Chart
        Friend WithEvents ResizePictureBox As System.Windows.Forms.PictureBox
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents YldLabel As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents DurLabel As System.Windows.Forms.Label
        Friend WithEvents SpreadLabel As System.Windows.Forms.Label
        Friend WithEvents ZSpreadLabel As System.Windows.Forms.Label
        Friend WithEvents ChartCMS As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents CopyToClipboardTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents HistoryCMS As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents RemoveHistoryTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents DscrLabel As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents DatLabel As System.Windows.Forms.Label
        Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RubIRSTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents MoneyCurveCMS As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents MMNameTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents DeleteMMCurveTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents BrokerTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents QuoteTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents SelDateTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RubCCSTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents NDFTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents LinkSpreadLabel As System.Windows.Forms.LinkLabel
        Friend WithEvents SpreadCMS As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents ZSpreadLinkLabel As System.Windows.Forms.LinkLabel
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents ASWLinkLabel As System.Windows.Forms.LinkLabel
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents MatLabel As System.Windows.Forms.Label
        Friend WithEvents ConvLabel As System.Windows.Forms.Label
        Friend WithEvents DurConvCMS As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents ModifiedTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents MacaleyTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents EffectiveTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents AsTableTSB As System.Windows.Forms.ToolStripButton
        Friend WithEvents BidAskCMS As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents RemoveBidAskTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ShowLegendTSB As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents YAxisCMS As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents FitTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents BootstrapTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents RelatedQuoteTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents BondDescriptionTSMI As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RelatedChartTSMI As System.Windows.Forms.ToolStripMenuItem
    End Class
End Namespace