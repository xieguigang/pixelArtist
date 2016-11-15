Imports Microsoft.VisualBasic.DataMining.QLearning.DataModel
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

    Sub Main()
        Dim model As QModel = Nothing

        If Not String.IsNullOrEmpty(App.Command) Then
            Dim args = App.CommandLine
            Dim load As String = args("/load")
            Dim isXml As Boolean = args.GetBoolean("/xml")

            If isXml Then
                model = load.LoadXml(Of QModel)
            Else
                model = load.ReadAllText.LoadObject(Of QModel)
            End If
        End If

        Call FreshLearning(model)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model">
    ''' If the QL_AI data is nothing, then fresh learning
    ''' </param>
    Private Sub FreshLearning(model As QModel)
        Dim game As New Snake.FormGameDisplay

        Call RunTask(AddressOf game.ShowDialog)
        Call Threading.Thread.Sleep(2000)

        Dim q As New QL_AI(game.GameEngine, model)

        game.GameEngine.ControlsMap.Enable = False

        Call RunTask(
            AddressOf New FormQLViewer With {
                .Table = q.Q
            }.ShowDialog)
        Call q.RunLearningLoop(Integer.MaxValue)
    End Sub
End Module
