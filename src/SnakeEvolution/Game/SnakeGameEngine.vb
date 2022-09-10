Imports PixelArtist.Engine

Public Class SnakeGameEngine

    Dim game As WorldEngine
    Dim hi As Integer

    Friend score As Integer
    Friend snake As New Snake(New Point(10, 10), 10)
    Friend food As Food

    Friend GameOverCallback As Action(Of SnakeGameEngine)

    Public Property CrossBodyEnable As Boolean
    Public ReadOnly Property Running As Boolean
        Get
            Return game.running
        End Get
    End Property

    Sub New(host As FormGame)
        game = New WorldEngine(AddressOf Render, AddressOf Run, fps:=30, worldSpeed:=100)
        game.LoadScreenDevice(host.PixelScreen1)
        game.Run()

        Call PutFood()
    End Sub

    Private Sub PutFood()
        food = New Food(game.screen.Random)
    End Sub

    Public Sub Invoke(action As Controls)
        Call Run(action, Nothing)
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

        Dim res As Size = game.screen.Resolution

        ' reach the wall
        If snake.Head.X < 0 OrElse
            snake.Head.Y < 0 OrElse
            snake.Head.X > res.Width OrElse
            snake.Head.Y > res.Height Then

            ' snake.Move(0, 0)
            Call GameSet()
        End If
    End Sub

    Private Sub Render(g As PixelGraphics)
        Call snake.Draw(g)
        Call food.Draw(g)
        Call g.DrawString($"Score: {score}; HI: {hi}", Color.Blue, FormGame.DefaultFont, 10, 10)
    End Sub

    Public Sub GameReset()
        score = 0
        snake = New Snake(New Point(10, 10), 10)

        Call PutFood()
        Call game.Run()
    End Sub

    Private Sub GameSet()
        Call snake.Move(0, 0)
        Call game.Pause()
    End Sub
End Class
