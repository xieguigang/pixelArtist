Public Class DisplayPort

    Public Property Engine As Engine

    Private Sub DisplayPort_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        If Not Engine Is Nothing Then
            Call Engine.Free
        End If
    End Sub
End Class
