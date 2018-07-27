Class MainWindow

    Private Sub viewer_MouseWheel(sender As Object, e As MouseWheelEventArgs) Handles viewer.MouseWheel
        Dim height# = viewer.Height
        Dim width# = viewer.Width
        Dim delta% = e.Delta

        height += delta
        width += delta

        viewer.Height = If(height < 150, 150, height)
        viewer.Width = If(width < 200, 200, width)
    End Sub
End Class
