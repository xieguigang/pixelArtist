Imports PixelArtist.Engine

Public Class FormGame

    Dim game As WorldEngine
    Dim snake As New Snake(New Point(100, 100), 10)

    Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles Me.Load
        game = New WorldEngine(AddressOf Render, AddressOf Run, fps:=30)
        game.LoadScreenDevice(PixelScreen1)
        game.Run()

        Me.DoubleBuffered = True
    End Sub

    Private Sub Run()

    End Sub

    Private Sub Render(g As PixelGraphics)
        Call snake.Draw(g)
    End Sub
End Class
