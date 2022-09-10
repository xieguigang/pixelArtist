Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.MachineLearning.QLearning
Imports Microsoft.VisualBasic.MachineLearning.QLearning.DataModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports PixelArtist.Engine

''' <summary>
''' The Q-Learning AI engine of this snake game
''' </summary>
Public Class QL_AI : Inherits QLearning(Of GameControl)

    Dim _snakeGame As SnakeGameEngine
    Dim _score As Integer

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
            If _score < _snakeGame.score Then
                _score = _snakeGame.score
                Return True
            Else
                Return False
            End If
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
        Me._snakeGame.GameOverCallback = AddressOf __gameOver
        Me._snakeGame.CrossBodyEnable = True
    End Sub

    Private Sub __gameOver(engine As SnakeGameEngine)
        Call engine.GameReset()
        Call Q.UpdateQvalue(GoalPenalty, _stat.Current)

        _score = 0

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

        Dim index As Integer = Q.NextAction(_stat.Current)    ' Get action index based on the current environment inputs 
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

        Call Threading.Thread.Sleep(300)

        ' Calculate the distance between the snake head and the target food
        Dim now = _snakeGame.snake.Head.Distance(_snakeGame.food.Location)

        If now < pre Then ' If the distance result of the new action compare with the previous action is getting smaller, then rewards the AI 
            ' 与前一个状态相比距离变小了，则奖励
            Call Q.UpdateQvalue(GoalRewards, preAction)
            Call Console.WriteLine("+")
        Else
            ' Else if larger the distance, penalty
            Call Q.UpdateQvalue(GoalPenalty / 2, preAction)
        End If

        If Not _snakeGame.Running Then
            Call _snakeGame.GameReset()  ' if game over then restart the game
        End If
    End Sub

    Protected Overrides Sub initialize()

    End Sub

    Protected Overrides Sub reset(i As Integer)

    End Sub
End Class