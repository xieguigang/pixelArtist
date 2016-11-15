Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataMining.QLearning.DataModel
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' CLI not working on the Form thread, not sure why??
''' </summary>
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
        Dim game = New Snake.FormGameDisplay

        Call RunTask(AddressOf game.ShowDialog)

        Do While game.GameEngine Is Nothing OrElse game.GameEngine.Running = False
            Threading.Thread.Sleep(1)
        Loop

        Dim q As New QL_AI(game.GameEngine, ai)
        game.GameEngine.ControlsMap.Enable = False

        Call RunTask(AddressOf New FormQLViewer With {.Table = q.Q}.ShowDialog)
        Call q.RunLearningLoop(Integer.MaxValue)
        Return 0
    End Function

    Public Function RunFresh() As Integer
        Return Run(Nothing)
    End Function
End Module
