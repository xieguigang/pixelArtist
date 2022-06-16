Imports System.ComponentModel
Imports Microsoft.VisualBasic.Text
Imports PixelArtist.Engine

Public Class FormGame

    Dim game As WorldEngine
    Dim snake As New Snake(New Point(10, 10), 10)
    Dim food As Food

    Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles Me.Load
        game = New WorldEngine(AddressOf Render, AddressOf Run, fps:=30, worldSpeed:=100)
        game.LoadScreenDevice(PixelScreen1)
        game.Run()

        Call PutFood()

        Me.DoubleBuffered = True
    End Sub

    Private Sub PutFood()
        food = New Food(PixelScreen1.Random)
    End Sub

    Private Sub Run(c As Char)
        If c <> ASCII.NUL Then
            Select Case c
                Case "w" : Call snake.Move(0, -1)
                Case "s" : Call snake.Move(0, 1)
                Case "a" : Call snake.Move(-1, 0)
                Case "d" : Call snake.Move(1, 0)
                Case Else
                    ' do nothing
                    Call snake.Move()
            End Select
        Else
            Call snake.Move()
        End If

        If food.Check(snake) Then
            Call PutFood()
            Call snake.Extend()
        End If

        If snake.Head.X < 0 OrElse
            snake.Head.Y < 0 OrElse
            snake.Head.X > PixelScreen1.Resolution.Width OrElse
            snake.Head.Y > PixelScreen1.Resolution.Height Then

            snake.Move(0, 0)
        End If
    End Sub

    Private Sub Render(g As PixelGraphics)
        Call snake.Draw(g)
        Call food.Draw(g)
    End Sub

    Private Sub FormGame_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub FormGame_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        Call PixelScreen1.CallKeyPress(sender, e)
    End Sub
End Class
