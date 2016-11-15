Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        DisplayPort1.Dock = DockStyle.Fill
        DisplayPort1.Engine = New Engine(DisplayPort1)
        DisplayPort1.Engine.Init()
        DisplayPort1.Engine.DriverRun
    End Sub
End Class
