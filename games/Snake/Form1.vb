Imports Microsoft.VisualBasic.GamePads

Public Class Form1

    Public ReadOnly Property GameEngine As GameEngine

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Call App.Exit(0)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim display As New DisplayPort
        Call Me.Controls.Add(display)
        display.Dock = DockStyle.Fill
        display.Engine = New GameEngine(display)
        display.Engine.Init()
        _GameEngine = display.Engine
        Call Microsoft.VisualBasic.Parallel.RunTask(AddressOf display.Engine.Run)
    End Sub
End Class
