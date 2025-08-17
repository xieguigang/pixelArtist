Namespace  raytracing.pixeldata
    Public Class GaussianBlur
        Private kernel As Single()
        Private pixelBufferField As PixelBuffer
        Private width, height As Integer

        Public Sub New(pixelBuffer As PixelBuffer)
            pixelBufferField = pixelBuffer
            width = pixelBuffer.Width
            height = pixelBuffer.Height

            ' Kernel is currently hardcoded with the help of http://dev.theomader.com/gaussian-kernel-calculator/
            kernel = New Single() {0.0093F, 0.028002F, 0.065984F, 0.121703F, 0.175713F, 0.198596F, 0.175713F, 0.121703F, 0.065984F, 0.028002F, 0.0093F}
        End Sub

        Public Overridable Sub blurHorizontally(radius As Integer)
            Dim result As PixelBuffer = New PixelBuffer(width, height)
            For y = 0 To height - 1
                For x = 0 To width - 1
                    Dim blurredColor As Color = New Color(0, 0, 0)
                    Dim originalPixel = pixelBufferField.getPixel(x, y)
                    For i = -radius To radius
                        Dim kernelMultiplier = kernel((i + radius) / (radius * 2F) * (kernel.Length - 1))
                        If x + i >= 0 AndAlso x + i < width Then
                            Dim pixel = pixelBufferField.getPixel(x + i, y)
                            If pixel IsNot Nothing Then
                                blurredColor.addSelf(pixel.Color.multiply(kernelMultiplier))
                            End If
                        End If
                    Next

                    result.setPixel(x, y, New PixelData(blurredColor, originalPixel.Depth, originalPixel.Emission))
                Next
            Next
            pixelBufferField = result
        End Sub

        Public Overridable Sub blurVertically(radius As Integer)
            Dim result As PixelBuffer = New PixelBuffer(width, height)
            For x = 0 To width - 1
                For y = 0 To height - 1
                    Dim blurredColor As Color = New Color(0, 0, 0)
                    Dim originalPixel = pixelBufferField.getPixel(x, y)
                    For i = -radius To radius
                        Dim kernelMultiplier = kernel((i + radius) / (radius * 2F) * (kernel.Length - 1))
                        If y + i >= 0 AndAlso y + i < height Then
                            Dim pixel = pixelBufferField.getPixel(x, y + i)
                            If pixel IsNot Nothing Then
                                blurredColor.addSelf(pixel.Color.multiply(kernelMultiplier))
                            End If
                        End If
                    Next

                    result.setPixel(x, y, New PixelData(blurredColor, originalPixel.Depth, originalPixel.Emission))
                Next
            Next
            pixelBufferField = result
        End Sub

        Public Overridable Sub blur(radius As Integer, iterations As Integer)
            For i = 0 To iterations - 1
                blurHorizontally(radius)
                blurVertically(radius)
            Next
        End Sub

        Public Overridable ReadOnly Property PixelBuffer As PixelBuffer
            Get
                Return pixelBufferField
            End Get
        End Property

        ' Currently unused due to kernel being hardcoded
        'private float gaussianDistribution(float x, float sigma) {
        '    return (float) (1/Math.sqrt(2*Math.PI*sigma*sigma)*Math.exp(-(x*x)/(2*sigma*sigma))); // https://en.wikipedia.org/wiki/Gaussian_blur
        '}

    End Class

End Namespace
