Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.MachineLearning.QLearning
Imports Microsoft.VisualBasic.MachineLearning.QLearning.DataModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports PixelArtist.Engine
Imports stdNum = System.Math

''' <summary>
''' The Q-Learning AI engine of this snake game
''' </summary>
Public Class QLearningSnakeAI : Inherits QLearning(Of GameControl)

    Dim _snakeGame As SnakeGameEngine
    Dim dist0 As Double

    ''' <summary>
    ''' Only 4 direction output: ``UP, DOWN, LEFT, RIGHT arrows``
    ''' </summary>
    ''' <returns></returns>
    Public Overrides ReadOnly Property ActionRange As Integer
        Get
            Return 4
        End Get
    End Property

    Public Overrides ReadOnly Property GoalReached As Boolean
        Get
            Dim d = _snakeGame.snake.Head.Distance(_snakeGame.food.Location)
            Dim test = d < dist0

            dist0 = d

            Return test
        End Get
    End Property

    Private Shared Function __getQTable(model As QModel) As Func(Of Integer, QTable)
        If model Is Nothing Then
            Return Function(n) New QTable(n)
        Else
            Return Function(n) New QTable(model)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="game"></param>
    ''' <param name="model">
    ''' Using the exists AI training result to initialize this engine
    ''' </param>
    Sub New(game As SnakeGameEngine, Optional model As QModel = Nothing)
        Call MyBase.New(New QState(game), __getQTable(model))

        Me._snakeGame = game
        Me._snakeGame.CrossBodyEnable = True
    End Sub

    Private Sub gameOver()
        ' if game over then restart the game
        Call _snakeGame.GameReset()
        ' the snake is dead after the current action
        ' add penalty
        Call Q.UpdateQvalue(GoalPenalty, _stat.Current)

        SyncLock Q
            Call New QModel(Q).GetJson.SaveTo(App.AppSystemTemp & $"/{Now.ToString.NormalizePathString}.json")
        End SyncLock
    End Sub

    ''' <summary>
    ''' QL AI logic
    ''' </summary>
    ''' <param name="i"></param>
    Protected Overrides Sub run(i As Integer)
        ' Get environment state as input
        Dim pre = _snakeGame.snake.Head.Distance(_snakeGame.food.Location)

        Call _stat.SetState(_stat.GetNextState(Nothing))

        ' Get action index based on the current environment inputs 
        Dim index As Integer = Q.NextAction(_stat.Current)
        Dim preAction = _stat.Current
        Dim action As Controls

        ' Gamepad action decodes
        Select Case index
            Case 0
                action = Controls.Up
            Case 1
                action = Controls.Down
            Case 2
                action = Controls.Left
            Case 3
                action = Controls.Right
            Case Else
                action = Controls.NotBind
        End Select

        ' QL_AI press the button on the gamepad
        Call _snakeGame.Invoke(action)
        Call _stat.Current.__DEBUG_ECHO

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
        Call Threading.Thread.Sleep(_snakeGame.game.worldSpeed * 1.125)

        ' Calculate the distance between the snake head and the target food
        Dim now = _snakeGame.snake.Head.Distance(_snakeGame.food.Location)

        If now < pre Then
            ' If the distance result of the new action compare with the previous
            ' action is getting smaller, then rewards the AI 
            ' 与前一个状态相比距离变小了，则奖励
            Call Q.UpdateQvalue(GoalRewards, preAction)
            Call Console.WriteLine("+")
        Else
            ' Else if larger the distance, penalty
            Call Q.UpdateQvalue(GoalPenalty / 2, preAction)
        End If

        If Not _snakeGame.Running Then
            Call gameOver()
        End If
    End Sub

    Protected Overrides Sub initialize()

    End Sub

    Protected Overrides Sub reset(i As Integer)

    End Sub
End Class