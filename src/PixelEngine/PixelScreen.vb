Public Class PixelScreen : Implements IDisposable

    Public Property Resolution As Size

    Dim canvas As Action(Of PixelGraphics)

    Public Sub RequestFrame(paint As Action(Of PixelGraphics))
        Me.canvas = paint
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim dev As PixelGraphics = PixelGraphics.FromGdiDevice(e.Graphics, Size)

        Call dev.SetScreenResolution(Resolution)
        Call dev.Clear(BackColor)
        Call canvas(dev)

        MyBase.OnPaint(e)
    End Sub

End Class
