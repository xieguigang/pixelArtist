Public Class DisplayPort

    Public Property Engine As Engine

    Private Sub DisplayPort_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Call Engine.Free
    End Sub
End Class
