
''' <summary>
''' The display output
''' </summary>
Public Class DisplayPort

    Public Property Engine As GameEngine

    Friend __updateDriver As Action(Of Graphics, Size)

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Call __updateDriver(e.Graphics, Size)
    End Sub

    Private Sub DisplayPort_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        If Not Engine Is Nothing Then
            Call Engine.Free
        End If
    End Sub
End Class
