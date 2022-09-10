Imports System.ComponentModel

Public Class FormGame

    Friend game As SnakeGameEngine

    Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
        Me.game = New SnakeGameEngine(Me)
    End Sub

    Private Sub FormGame_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub FormGame_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        Call PixelScreen1.CallKeyPress(sender, e)
    End Sub
End Class
