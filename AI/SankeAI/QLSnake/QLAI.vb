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
        Call Threading.Thread.Sleep(50)
    End Sub
End Class
