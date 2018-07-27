Public Class SlicerInput

    Public flush As Action

    Private Sub ViewRectangle_Click(sender As Object, e As RoutedEventArgs) Handles ViewRectangle.Click
        If Not flush Is Nothing Then
            Call flush()
        End If
    End Sub
End Class
