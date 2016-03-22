Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.DataMining.Framework.QLearning
Imports Microsoft.VisualBasic.Serialization

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
        Call MyBase.New(New QState(game), Function(n) New QTable(n))
        Me.game = game
        game.GameOverCallback = Sub(engine)
                                    Call engine.Reset()
                                    Call Q.UpdateQvalue(GoalPenalty, _stat.Current)
                                    SyncLock Q
                                        Call New QModel(Q).GetJson.SaveTo(App.AppSystemTemp & $"/{Now.ToString.NormalizePathString}.json")
                                    End SyncLock
                                End Sub
    End Sub

    Protected Overrides Sub __init()

    End Sub

    Protected Overrides Sub __reset(i As Integer)

    End Sub

    Protected Overrides Sub __run(i As Integer)
        Dim pre = Distance(game.Snake.Location, game.food.Location)
        Dim index As Integer = Q.NextAction(_stat.Current)
        Dim action As Controls

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

        Call game.Invoke(action)

        Call _stat.SetState(_stat.GetNextState(action))
        Call _stat.Current.__DEBUG_ECHO
        Call Threading.Thread.Sleep(100)

        Dim now = Distance(game.Snake.Location, game.food.Location)

        If now < pre Then  ' 与前一个状态相比距离变小了，则奖励
            Call Q.UpdateQvalue(GoalRewards / 2, _stat.Current)
        Else
            Call Q.UpdateQvalue(GoalPenalty / 2, _stat.Current)
        End If
    End Sub
End Class
