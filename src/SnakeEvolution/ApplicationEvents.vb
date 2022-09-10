Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.MachineLearning.QLearning.DataModel
Imports Microsoft.VisualBasic.Parallel

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.

    ' **NEW** ApplyApplicationDefaults: Raised when the application queries default values to be set for the application.

    ' Example:
    ' Private Sub MyApplication_ApplyApplicationDefaults(sender As Object, e As ApplyApplicationDefaultsEventArgs) Handles Me.ApplyApplicationDefaults
    '
    '   ' Setting the application-wide default Font:
    '   e.Font = New Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular)
    '
    '   ' Setting the HighDpiMode for the Application:
    '   e.HighDpiMode = HighDpiMode.PerMonitorV2
    '
    '   ' If a splash dialog is used, this sets the minimum display time:
    '   e.MinimumSplashScreenDisplayTime = 4000
    ' End Sub

    Partial Friend Class MyApplication

        Public Overloads Shared Function Run(ai As QModel) As Integer
            Dim game = New FormGame

            Call RunTask(AddressOf game.ShowDialog)

            Do While game.game Is Nothing OrElse game.game.Running = False
                Threading.Thread.Sleep(1)
            Loop

            Dim q As New QLearningSnakeAI(game.game, ai)
            game.game.ControlsMap.Enable = False

            Call RunTask(AddressOf New FormQLViewer With {.Table = q.Q}.ShowDialog)
            Call q.RunLearningLoop(Integer.MaxValue)
            Return 0
        End Function

        Public Shared Function RunFresh() As Integer
            Return Run(ai:=Nothing)
        End Function
    End Class
End Namespace
