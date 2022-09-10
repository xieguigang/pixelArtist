<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormGame
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PixelScreen1 = New PixelArtist.Engine.PixelScreen()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.GameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlayByAIToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PixelScreen1
        '
        Me.PixelScreen1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PixelScreen1.BackColor = System.Drawing.Color.White
        Me.PixelScreen1.Location = New System.Drawing.Point(12, 50)
        Me.PixelScreen1.Name = "PixelScreen1"
        Me.PixelScreen1.Resolution = New System.Drawing.Size(60, 80)
        Me.PixelScreen1.Size = New System.Drawing.Size(617, 845)
        Me.PixelScreen1.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GameToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(641, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'GameToolStripMenuItem
        '
        Me.GameToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PlayByAIToolStripMenuItem})
        Me.GameToolStripMenuItem.Name = "GameToolStripMenuItem"
        Me.GameToolStripMenuItem.Size = New System.Drawing.Size(50, 20)
        Me.GameToolStripMenuItem.Text = "Game"
        '
        'PlayByAIToolStripMenuItem
        '
        Me.PlayByAIToolStripMenuItem.CheckOnClick = True
        Me.PlayByAIToolStripMenuItem.Name = "PlayByAIToolStripMenuItem"
        Me.PlayByAIToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.PlayByAIToolStripMenuItem.Text = "Play By AI"
        '
        'FormGame
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(641, 907)
        Me.Controls.Add(Me.PixelScreen1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FormGame"
        Me.Text = "Form1"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PixelScreen1 As PixelArtist.Engine.PixelScreen
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents GameToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PlayByAIToolStripMenuItem As ToolStripMenuItem
End Class
