Public Class Controller

    ReadOnly game As Game

    Sub New(game As Game)
        Me.game = game
    End Sub

    Public Sub GameReset(keyData As Keys)
        If keyData = Keys.R Or keyData = Keys.Space Then
            Call game.RestartGame()
        End If
    End Sub

    Public Sub GameControl(keyData As Keys)
        Select Case keyData
            Case Keys.Up, Keys.W
                If game.playerSnake.Direction.Y <> 1 Then
                    game.playerSnake.Direction = New Point(0, -1)
                End If
            Case Keys.Down, Keys.S
                If game.playerSnake.Direction.Y <> -1 Then
                    game.playerSnake.Direction = New Point(0, 1)
                End If
            Case Keys.Left, Keys.A
                If game.playerSnake.Direction.X <> 1 Then
                    game.playerSnake.Direction = New Point(-1, 0)
                End If
            Case Keys.Right, Keys.D
                If game.playerSnake.Direction.X <> -1 Then
                    game.playerSnake.Direction = New Point(1, 0)
                End If
            Case Keys.P
                game.paused = Not game.paused
        End Select
    End Sub
End Class
