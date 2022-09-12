Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork
Imports PixelArtist.Engine
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class ANNLearnSnakeAI

    Dim core As Network
    Dim game As SnakeGameEngine
    Dim stat As QState

    Sub New(game As SnakeGameEngine)
        Me.game = game
        Me.core = New Network(
            inputSize:=20,
            hiddenSize:={8, 6, 5},
            outputSize:=4,
            learnRate:=0.1,
            momentum:=0.9
        )
        Me.stat = New QState(game)
    End Sub

    Public Sub Run()
        For i As Integer = 0 To Integer.MaxValue
            Call Run(i)
        Next
    End Sub

    Private Sub Run(i As Integer)
        ' Get environment state as input
        Dim pre = game.snake.Head.Distance(game.food.Location)

        Call stat.SetState(stat.GetNextState(Nothing))

        Dim v As Double() = stat.Current.ToVector
        Dim output = core.ForwardPropagate(v, parallel:=True)
        Dim controls = output.Select(Function(n) If(n.Value < 0.5, 0, n.Value)).ToArray
        Dim index = which.Max(controls)
        Dim action As Controls = PixelArtist.Engine.Controls.NotBind

        If controls.Any(Function(c) c > 0) Then
            ' Gamepad action decodes
            Select Case index
                Case 0
                    action = PixelArtist.Engine.Controls.Up
                Case 1
                    action = PixelArtist.Engine.Controls.Down
                Case 2
                    action = PixelArtist.Engine.Controls.Left
                Case 3
                    action = PixelArtist.Engine.Controls.Right
                Case Else
                    action = PixelArtist.Engine.Controls.NotBind
            End Select
        End If

        ' QL_AI press the button on the gamepad
        Call game.Invoke(action)
        ' 20220910
        '
        ' wait for the action to take effects
        '
        ' the sleep time should be nearby the 
        ' word reactor its update time
        ' or the AI can not be learn due to the
        ' reason of world is already been updated
        ' too many loops when we test the condition
        ' below
        Call Threading.Thread.Sleep(game.game.worldSpeed + 10)

        ' Calculate the distance between the snake head and the target food
        Dim now = game.snake.Head.Distance(game.food.Location)

        If now <= pre Then
            ' If the distance result of the new action compare with the previous
            ' action is getting smaller, then rewards the AI 
            ' 与前一个状态相比距离变小了，则奖励
            controls = output.Select(Function(n, j) If(j = index, 1.0, 0.0)).ToArray
            core.BackPropagate(controls, parallel:=True)
        Else
            ' Else if larger the distance, penalty
            ' just press a random bottom on the game pad?
            controls = New Double(4 - 1) {}
            controls(randf.NextInteger(4)) = 1
            core.BackPropagate(controls, parallel:=True)
        End If

        If Not game.Running Then
            Call gameOver()
        End If
    End Sub

    Private Sub gameOver()
        ' if game over then restart the game
        Call game.GameReset()
        game.snake.Move(1, 0)
    End Sub
End Class
