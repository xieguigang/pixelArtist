Public Class DisplayPort

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ErrorImage = My.Resources.ErrorImage
    End Sub

    Public Property Engine As GameEngine

    Private Sub DisplayPort_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        If Not Engine Is Nothing Then
            Call Engine.Free
        End If
    End Sub
End Class
