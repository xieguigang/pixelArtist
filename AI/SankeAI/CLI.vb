Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataMining.Framework.QLearning.DataModel
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization

Module CLI

    <ExportAPI("/start", Usage:="/start /load <ai_path.json/xml> [/xml]")>
    Public Function Start(args As CommandLine.CommandLine) As Integer
        Dim load As String = args("/load")
        Dim isXml As Boolean = args.GetBoolean("/xml")
        Dim model As QModel

        If isXml Then
            model = load.LoadXml(Of QModel)
        Else
            model = load.ReadAllText.LoadObject(Of QModel)
        End If

        Return Run(model)
    End Function

    Public Function Run(ai As QModel) As Integer
        Dim game = New Snake.Form1
        game.InitCallback = Sub()
                                Dim q As New QL_AI(game.GameEngine, ai)
                                game.GameEngine.ControlsMap.Enable = False
                                game.GameEngine.PauseEnable = False

                                Call RunTask(AddressOf New Form1 With {.Table = q.Q}.ShowDialog)
                                Call RunTask(Sub() q.RunLearningLoop(Integer.MaxValue))
                            End Sub
        Call game.ShowDialog()

        Return 0
    End Function

    Public Function RunFresh() As Integer
        Return Run(Nothing)
    End Function
End Module
