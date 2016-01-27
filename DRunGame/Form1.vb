Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        DisplayPort1.Engine = New DianosolaRun(DisplayPort1)
        DisplayPort1.Engine.Init()
        Call Microsoft.VisualBasic.Parallel.Run(AddressOf DisplayPort1.Engine.Run)
    End Sub
End Class
