Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.DataMining.Framework.QLearning

Public Class QLAI : Inherits QLearning(Of GameControl)

    Dim game As Snake.GameEngine

    Public Overrides ReadOnly Property ActionRange As Integer
        Get
            Return 4
        End Get
    End Property

    Public Overrides ReadOnly Property GoalReached As Boolean
        Get
            Return game.food.Region.Contains(game.Snake.Location)
        End Get
    End Property

    Sub New(game As Snake.GameEngine)
        Call MyBase.New(New QState(game))
        Me.game = game
        game.GameOverCallback = Sub(engine)
                                    Call engine.Reset()
                                End Sub
    End Sub

    Protected Overrides Sub __init()

    End Sub

    Protected Overrides Sub __reset(i As Integer)

    End Sub

    Protected Overrides Sub __run(Q As QTable(Of GameControl), i As Integer)
        Dim action = CType(Q.NextAction(_stat.Current), Controls)
        Call game.Invoke(action)
        Call _stat.SetState(_stat.GetNextState(action))
        Call _stat.Current.__DEBUG_ECHO
        Call Threading.Thread.Sleep(15)
    End Sub
End Class
