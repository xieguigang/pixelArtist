Imports System.ComponentModel
Imports Microsoft.VisualBasic.Text
Imports PixelArtist.Engine

Public Class FormGame

    Dim game As WorldEngine
    Dim snake As New Snake(New Point(10, 10), 10)

    Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles Me.Load
        game = New WorldEngine(AddressOf Render, AddressOf Run, fps:=30, worldSpeed:=1000)
        game.LoadScreenDevice(PixelScreen1)
        game.Run()

        Me.DoubleBuffered = True
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
    End Sub

    Private Sub Render(g As PixelGraphics)
        Call snake.Draw(g)
    End Sub

    Private Sub FormGame_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub FormGame_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        Call PixelScreen1.CallKeyPress(sender, e)
    End Sub
End Class
