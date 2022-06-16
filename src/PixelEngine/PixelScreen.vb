Public Class PixelScreen : Implements IDisposable

    Public Property Resolution As Size

    Public Function RequestFrame() As PixelGraphics
        Dim g As Graphics = Me.CreateGraphics
        Dim frame As PixelGraphics = PixelGraphics.FromGdiDevice(g, Size)

        Call g.Clear(BackColor)
        Call frame.SetScreenResolution(Resolution.Width, Resolution.Height)
        Call g.Flush()

        Return frame
    End Function

End Class
