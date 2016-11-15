<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.DisplayPort1 = New Microsoft.VisualBasic.GamePads.DisplayPort()
        Me.SuspendLayout()
        '
        'DisplayPort1
        '
        Me.DisplayPort1.AutoValidate = System.Windows.Forms.AutoValidate.Disable
        Me.DisplayPort1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DisplayPort1.Engine = Nothing
        Me.DisplayPort1.Location = New System.Drawing.Point(0, 0)
        Me.DisplayPort1.Name = "DisplayPort1"
        Me.DisplayPort1.Size = New System.Drawing.Size(654, 440)
        Me.DisplayPort1.TabIndex = 0
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(654, 440)
        Me.Controls.Add(Me.DisplayPort1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DisplayPort1 As GamePads.DisplayPort
End Class
