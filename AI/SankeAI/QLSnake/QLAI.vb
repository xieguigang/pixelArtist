Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.DataMining.Framework.QLearning
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.DataMining.Framework.QLearning.DataModel

Public Class QLAI : Inherits QLearning(Of GameControl)

    Dim game As Snake.GameEngine

    ''' <summary>
    ''' Only 4 direction output: UP, DOWN, LEFT, RIGHT arrows
    ''' </summary>
    ''' <returns></returns>
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

    Public ReadOnly Property QTable As QTable
        Get
            Return Q
        End Get
    End Property

    Sub New(game As Snake.GameEngine, model As QModel)
        Call MyBase.New(New QState(game),
                        [If](Of Func(Of Integer, QTable))(model Is Nothing,
                                                          Function(n) New QTable(n),
                                                          Function(n) New QTable(model)))
        Me.game = game
        game.GameOverCallback = Sub(engine)
                                    Call engine.Reset()
                                    Call Q.UpdateQvalue(GoalPenalty, _stat.Current)
                                    SyncLock Q
                                        '    Call New QModel(Q).GetJson.SaveTo(App.AppSystemTemp & $"/{Now.ToString.NormalizePathString}.json")
                                    End SyncLock

                                    ' Call dump.Dump(Q, nnn.MoveNext)
                                    '  Call dump.Save(App.AppSystemTemp & "/QLearning.Csv")
                                End Sub
    End Sub

    Dim nnn As Integer

    Protected Overrides Sub __init()

    End Sub

    Dim dump As New QTableDump

    Protected Overrides Sub __reset(i As Integer)

    End Sub

    Protected Overrides Sub __run(i As Integer)
        Dim pre = Distance(game.Snake.Location, game.food.Location)
        Call _stat.SetState(_stat.GetNextState(Nothing))
        Dim index As Integer = Q.NextAction(_stat.Current)
        Dim preAction = _stat.Current
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


        Call _stat.Current.__DEBUG_ECHO
        Call Threading.Thread.Sleep(100)

        Dim now = Distance(game.Snake.Location, game.food.Location)

        If now < pre Then  ' 与前一个状态相比距离变小了，则奖励
            Call Q.UpdateQvalue(GoalRewards / 2, preAction)
            Call Console.WriteLine("+")
        Else
            Call Q.UpdateQvalue(GoalPenalty / 2, preAction)
        End If
    End Sub
End Class
