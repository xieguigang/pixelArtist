Imports System.ComponentModel
Imports PixelArtist.Engine

Public Class FormGame

    Dim game As WorldEngine
    Dim snake As New Snake(New Point(10, 10), 10)
    Dim food As Food
    Dim hi As Integer
    Dim score As Integer

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

    Private Sub Run(action As Controls, c As Char)
        If action <> PixelArtist.Engine.Controls.NotBind Then
            Select Case action
                Case PixelArtist.Engine.Controls.Up : Call snake.Move(0, -1)
                Case PixelArtist.Engine.Controls.Down : Call snake.Move(0, 1)
                Case PixelArtist.Engine.Controls.Left : Call snake.Move(-1, 0)
                Case PixelArtist.Engine.Controls.Right : Call snake.Move(1, 0)
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

            score += 1

            If hi < score Then
                hi = score
            End If
        End If

        ' reach the wall
        If snake.Head.X < 0 OrElse
            snake.Head.Y < 0 OrElse
            snake.Head.X > PixelScreen1.Resolution.Width OrElse
            snake.Head.Y > PixelScreen1.Resolution.Height Then

            ' snake.Move(0, 0)
            Call GameSet()
        End If
    End Sub

    Private Sub Render(g As PixelGraphics)
        Call snake.Draw(g)
        Call food.Draw(g)
        Call g.DrawString($"Score: {score}; HI: {hi}", Color.Blue, FormGame.DefaultFont, 10, 10)
    End Sub

    Private Sub GameReset()
        score = 0
        snake = New Snake(New Point(10, 10), 10)

        Call PutFood()
        Call game.Run()
    End Sub

    Private Sub GameSet()
        Call snake.Move(0, 0)
        Call game.Pause()
    End Sub

    Private Sub FormGame_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub FormGame_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        Call PixelScreen1.CallKeyPress(sender, e)
    End Sub
End Class
