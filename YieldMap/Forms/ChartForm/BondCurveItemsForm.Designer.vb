﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BondCurveItemsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Dim MainToolStrip As System.Windows.Forms.ToolStrip
        Dim MainTC As System.Windows.Forms.TabControl
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BondCurveItemsForm))
        Me.BondsTP = New System.Windows.Forms.TabPage()
        Me.CurrentTP = New System.Windows.Forms.TabPage()
        Me.RemoveItemsTSB = New System.Windows.Forms.ToolStripButton()
        Me.AddItemsTSB = New System.Windows.Forms.ToolStripButton()
        Me.BondsDGV = New System.Windows.Forms.DataGridView()
        Me.CurrentDGV = New System.Windows.Forms.DataGridView()
        TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        MainToolStrip = New System.Windows.Forms.ToolStrip()
        MainTC = New System.Windows.Forms.TabControl()
        TableLayoutPanel1.SuspendLayout()
        MainToolStrip.SuspendLayout()
        MainTC.SuspendLayout()
        Me.BondsTP.SuspendLayout()
        Me.CurrentTP.SuspendLayout()
        CType(Me.BondsDGV, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CurrentDGV, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        TableLayoutPanel1.ColumnCount = 1
        TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        TableLayoutPanel1.Controls.Add(MainToolStrip, 0, 0)
        TableLayoutPanel1.Controls.Add(MainTC, 0, 1)
        TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 2
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        TableLayoutPanel1.Size = New System.Drawing.Size(673, 502)
        TableLayoutPanel1.TabIndex = 0
        '
        'MainToolStrip
        '
        MainToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddItemsTSB, Me.RemoveItemsTSB})
        MainToolStrip.Location = New System.Drawing.Point(0, 0)
        MainToolStrip.Name = "MainToolStrip"
        MainToolStrip.Size = New System.Drawing.Size(673, 25)
        MainToolStrip.TabIndex = 0
        MainToolStrip.Text = "MainToolStrip"
        '
        'MainTC
        '
        MainTC.Controls.Add(Me.BondsTP)
        MainTC.Controls.Add(Me.CurrentTP)
        MainTC.Dock = System.Windows.Forms.DockStyle.Fill
        MainTC.Location = New System.Drawing.Point(3, 28)
        MainTC.Name = "MainTC"
        MainTC.SelectedIndex = 0
        MainTC.Size = New System.Drawing.Size(667, 471)
        MainTC.TabIndex = 1
        '
        'BondsTP
        '
        Me.BondsTP.Controls.Add(Me.BondsDGV)
        Me.BondsTP.Location = New System.Drawing.Point(4, 22)
        Me.BondsTP.Name = "BondsTP"
        Me.BondsTP.Padding = New System.Windows.Forms.Padding(3)
        Me.BondsTP.Size = New System.Drawing.Size(659, 445)
        Me.BondsTP.TabIndex = 0
        Me.BondsTP.Text = "Bonds"
        Me.BondsTP.UseVisualStyleBackColor = True
        '
        'CurrentTP
        '
        Me.CurrentTP.Controls.Add(Me.CurrentDGV)
        Me.CurrentTP.Location = New System.Drawing.Point(4, 22)
        Me.CurrentTP.Name = "CurrentTP"
        Me.CurrentTP.Padding = New System.Windows.Forms.Padding(3)
        Me.CurrentTP.Size = New System.Drawing.Size(659, 445)
        Me.CurrentTP.TabIndex = 1
        Me.CurrentTP.Text = "Current representation"
        Me.CurrentTP.UseVisualStyleBackColor = True
        '
        'RemoveItemsTSB
        '
        Me.RemoveItemsTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.RemoveItemsTSB.Enabled = False
        Me.RemoveItemsTSB.Image = CType(resources.GetObject("RemoveItemsTSB.Image"), System.Drawing.Image)
        Me.RemoveItemsTSB.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.RemoveItemsTSB.Name = "RemoveItemsTSB"
        Me.RemoveItemsTSB.Size = New System.Drawing.Size(133, 22)
        Me.RemoveItemsTSB.Text = "Remove selected items..."
        '
        'AddItemsTSB
        '
        Me.AddItemsTSB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.AddItemsTSB.Enabled = False
        Me.AddItemsTSB.Image = CType(resources.GetObject("AddItemsTSB.Image"), System.Drawing.Image)
        Me.AddItemsTSB.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.AddItemsTSB.Name = "AddItemsTSB"
        Me.AddItemsTSB.Size = New System.Drawing.Size(70, 22)
        Me.AddItemsTSB.Text = "Add items..."
        '
        'BondsDGV
        '
        Me.BondsDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.BondsDGV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BondsDGV.Location = New System.Drawing.Point(3, 3)
        Me.BondsDGV.Name = "BondsDGV"
        Me.BondsDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.BondsDGV.Size = New System.Drawing.Size(653, 439)
        Me.BondsDGV.TabIndex = 0
        '
        'CurrentDGV
        '
        Me.CurrentDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.CurrentDGV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CurrentDGV.Location = New System.Drawing.Point(3, 3)
        Me.CurrentDGV.Name = "CurrentDGV"
        Me.CurrentDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.CurrentDGV.Size = New System.Drawing.Size(653, 439)
        Me.CurrentDGV.TabIndex = 0
        '
        'BondCurveItemsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(673, 502)
        Me.Controls.Add(TableLayoutPanel1)
        Me.Name = "BondCurveItemsForm"
        Me.Text = "Bond curve items"
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        MainToolStrip.ResumeLayout(False)
        MainToolStrip.PerformLayout()
        MainTC.ResumeLayout(False)
        Me.BondsTP.ResumeLayout(False)
        Me.CurrentTP.ResumeLayout(False)
        CType(Me.BondsDGV, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CurrentDGV, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents AddItemsTSB As System.Windows.Forms.ToolStripButton
    Friend WithEvents RemoveItemsTSB As System.Windows.Forms.ToolStripButton
    Friend WithEvents BondsTP As System.Windows.Forms.TabPage
    Friend WithEvents CurrentTP As System.Windows.Forms.TabPage
    Friend WithEvents BondsDGV As System.Windows.Forms.DataGridView
    Friend WithEvents CurrentDGV As System.Windows.Forms.DataGridView
End Class
