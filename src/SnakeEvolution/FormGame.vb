Imports System.ComponentModel
Imports PixelArtist.Engine

Public Class FormGame

    Dim game As WorldEngine
    Dim snake As New Snake(New Point(100, 100), 10)

    Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles Me.Load
        game = New WorldEngine(AddressOf Render, AddressOf Run, fps:=30, worldSpeed:=1000)
        game.LoadScreenDevice(PixelScreen1)
        game.Run()

        Me.DoubleBuffered = True
    End Sub

    Private Sub Run()
        Call snake.Move(1, 0)
    End Sub

    Private Sub Render(g As PixelGraphics)
        Call snake.Draw(g)
    End Sub

    Private Sub FormGame_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub
End Class
