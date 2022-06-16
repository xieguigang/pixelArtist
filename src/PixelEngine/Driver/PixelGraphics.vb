Imports Microsoft.VisualBasic.Imaging

Public Class PixelGraphics

    ReadOnly driver As IGraphics

    ''' <summary>
    ''' the size value of a pixel
    ''' </summary>
    Dim _pixel As SizeF

    ''' <summary>
    ''' the pixel screen resolution 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Resolution As Size
    Public Property PixelColor As Color = Color.Black

    Sub New(driver As IGraphics)
        Me.driver = driver
    End Sub

    Public Sub Clear(backColor As Color)
        Call driver.Clear(backColor)
    End Sub

    Public Function SetScreenResolution(res As Size) As PixelGraphics
        _Resolution = res
        _pixel = New SizeF(driver.Size.Width / Resolution.Width, driver.Size.Height / Resolution.Height)

        Return Me
    End Function

    Public Function SetScreenResolution(width As Integer, height As Integer) As PixelGraphics
        Return SetScreenResolution(New Size(width, height))
    End Function

    Public Sub DrawPixel(x As Integer, y As Integer, Optional color As Color = Nothing)
        Dim pixel As New RectangleF(
            x * _pixel.Width,
            y * _pixel.Height,
            _pixel.Width,
            _pixel.Height
        )

        If color.IsEmpty Then
            color = PixelColor
        End If

        Call driver.FillRectangle(New SolidBrush(color), rect:=pixel)
    End Sub

    Public Shared Function FromGdiDevice(dev As Graphics, canvas As Size) As PixelGraphics
        Return New PixelGraphics(New Graphics2D(dev, canvas))
    End Function
End Class
