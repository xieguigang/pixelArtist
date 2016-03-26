Imports Microsoft.VisualBasic.Parallel


Module Program

    Sub Main()

        Dim game = New Snake.Form1
        Call RunTask(AddressOf game.ShowDialog)
        Call Threading.Thread.Sleep(2000)

        Dim q As New QLAI(game.GameEngine)
        game.GameEngine.ControlsMap.Enable = False

        Call q.RunLearningLoop(Integer.MaxValue)

        Pause()
    End Sub

End Module
