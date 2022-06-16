Public Class PixelScreen : Implements IDisposable

    Public Property Resolution As Size = New Size(300, 400)

    Dim canvas As Action(Of PixelGraphics)

    Public Sub RequestFrame(paint As Action(Of PixelGraphics))
        Me.canvas = paint
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        If Not canvas Is Nothing Then
            Dim dev As PixelGraphics = PixelGraphics.FromGdiDevice(e.Graphics, Size)

            Call dev.SetScreenResolution(Resolution)
            Call dev.Clear(BackColor)
            Call canvas(dev)
        End If

        MyBase.OnPaint(e)
    End Sub

    Private Sub PixelScreen_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
    End Sub
End Class
