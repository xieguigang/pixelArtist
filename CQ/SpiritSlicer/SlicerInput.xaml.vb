Public Class SlicerInput

    Public flush As Action
    Public rect As New rect
    Public slicerNameString$

    Private Sub ViewRectangle_Click(sender As Object, e As RoutedEventArgs) Handles ViewRectangle.Click
        If Not flush Is Nothing Then
            Call flush()
        End If
    End Sub

    Private Sub rHeight_TextChanged(sender As Object, e As TextChangedEventArgs) Handles rHeight.TextChanged
        rect.height = Val(DirectCast(sender, TextBox).Text)
    End Sub

    Private Sub sliceName_TextChanged(sender As Object, e As TextChangedEventArgs) Handles sliceName.TextChanged
        slicerNameString = DirectCast(sender, TextBox).Text
    End Sub

    Private Sub rLeft_TextChanged(sender As Object, e As TextChangedEventArgs) Handles rLeft.TextChanged
        rect.left = Val(DirectCast(sender, TextBox).Text)
    End Sub

    Private Sub rTop_TextChanged(sender As Object, e As TextChangedEventArgs) Handles rTop.TextChanged
        rect.top = Val(DirectCast(sender, TextBox).Text)
    End Sub

    Private Sub rWidth_TextChanged(sender As Object, e As TextChangedEventArgs) Handles rWidth.TextChanged
        rect.width = Val(DirectCast(sender, TextBox).Text)
    End Sub
End Class
