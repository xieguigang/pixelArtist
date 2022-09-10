Imports System.ComponentModel
Imports System.Threading
Imports Microsoft.VisualBasic.Parallel

Public Class FormGame

    Friend game As SnakeGameEngine

    Private Sub FormGame_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
        Me.game = New SnakeGameEngine(Me)
        Me.game.GameReset()
    End Sub

    Private Sub FormGame_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub FormGame_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        Call PixelScreen1.CallKeyPress(sender, e)
    End Sub

    Private Sub PlayByAIToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayByAIToolStripMenuItem.Click
        Call New Thread(
            Sub()
                Dim q As New QLearningSnakeAI(game)
                game.ControlsMap.Enable = False

                Call RunTask(AddressOf New FormQLViewer With {.Table = q.Q}.ShowDialog)
                Call q.RunLearningLoop(Integer.MaxValue)
            End Sub).Start()
    End Sub
End Class
