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
        Me.SuspendLayout()
        '
        'PixelScreen1
        '
        Me.PixelScreen1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PixelScreen1.BackColor = System.Drawing.Color.White
        Me.PixelScreen1.Location = New System.Drawing.Point(12, 12)
        Me.PixelScreen1.Name = "PixelScreen1"
        Me.PixelScreen1.Resolution = New System.Drawing.Size(60, 80)
        Me.PixelScreen1.Size = New System.Drawing.Size(617, 883)
        Me.PixelScreen1.TabIndex = 0
        '
        'FormGame
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(641, 907)
        Me.Controls.Add(Me.PixelScreen1)
        Me.Name = "FormGame"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PixelScreen1 As PixelArtist.Engine.PixelScreen
End Class
