Class MainWindow

    Dim animations As Animation() = {
        New Animation(Png.PngResource.idle),
        New Animation(Png.PngResource.walk)
    }

    Dim idle As Animation
    Dim mouse As MoveDragHelper

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        idle = New Animation(Png.PngResource.Idle)
        mouse = New MoveDragHelper(Display, Me)

        idle.PlayBack(Me.Display)

        Me.Top = 0
        Me.Left = 0
        Me.Width = SystemParameters.WorkArea.Width
        Me.Height = SystemParameters.WorkArea.Height
        Me.Topmost = True
    End Sub

    Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        idle.Stop()
    End Sub
End Class
