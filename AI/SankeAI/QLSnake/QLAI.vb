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
        Call MyBase.New(New QState)
        Me.game = game
    End Sub

    Protected Overrides Sub __init()

    End Sub

    Protected Overrides Sub __reset(i As Integer)

    End Sub

    Protected Overrides Sub __run(Q As QTable(Of GameControl), i As Integer)
        Dim action = Q.NextAction(_stat.Current)
        Call game.Invoke(CType(action, Controls))
    End Sub
End Class
