Imports Microsoft.VisualBasic.GamePads

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim display As New DisplayPort
        Call Me.Controls.Add(display)
        display.Dock = DockStyle.Fill
        display.Engine = New GameEngine(display)
        display.Engine.Init()
        Call Microsoft.VisualBasic.Parallel.RunTask(AddressOf display.Engine.Run)
    End Sub
End Class
