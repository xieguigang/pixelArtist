﻿Class MainWindow

    Dim stage As Stage

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        stage = New Stage(Me, mainGrid)
        stage.Add(
            "1", {
            New Animation("idle", Png.idle, Nothing),
            New Animation("walk", Png.walk, New Offset With {.direction = OffsetDirections.left, .pixels = 90}),
            New Animation("highfive", Png.highfive, Nothing)
        })

        Me.Top = 0
        Me.Left = 0
        Me.Width = SystemParameters.WorkArea.Width
        Me.Height = SystemParameters.WorkArea.Height
        Me.Topmost = True
    End Sub

    Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        stage.Dispose()
    End Sub
End Class
