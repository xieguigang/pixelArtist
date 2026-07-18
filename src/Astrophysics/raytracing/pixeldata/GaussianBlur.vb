Imports System.Threading.Tasks

Namespace  raytracing.pixeldata
    Public Class GaussianBlur

        Private kernel As Single()
        Private width, height As Integer

        Public Overridable ReadOnly Property PixelBuffer As PixelBuffer

        Public Sub New(pixelBuffer As PixelBuffer)
            _PixelBuffer = pixelBuffer
            width = pixelBuffer.Width
            height = pixelBuffer.Height

            ' Default kernel matches the historical radius of 5.
            kernel = buildKernel(5)
        End Sub

        ''' <summary>
        ''' Builds a normalized 1D Gaussian kernel of length 2 * radius + 1.
        ''' The kernel is generated on demand so it always matches the requested
        ''' blur radius (the previous fixed 11-tap kernel only worked for radius 5).
        ''' </summary>
        Private Shared Function buildKernel(radius As Integer) As Single()
            Dim size = radius * 2 + 1
            Dim kernel = New Single(size - 1) {}
            Dim sigma = radius / 2.0F
            Dim twoSigmaSq = 2.0F * sigma * sigma
            Dim sum As Single = 0

            For i = -radius To radius
                Dim w = CSng(System.Math.Exp(-(i * i) / twoSigmaSq))
                kernel(i + radius) = w
                sum += w
            Next

            ' Normalize so the weights sum to 1 (keeps brightness constant after blur).
            For i = 0 To kernel.Length - 1
                kernel(i) /= sum
            Next

            Return kernel
        End Function

        Public Overridable Sub blurHorizontally(radius As Integer)
            kernel = buildKernel(radius)

            Dim result As PixelBuffer = New PixelBuffer(width, height)
            Parallel.For(0, height, Sub(y)
                                        For x = 0 To width - 1
                                            Dim blurredColor As Color = New Color(0, 0, 0)
                                            Dim originalPixel = PixelBuffer.getPixel(x, y)
                                            For i = -radius To radius
                                                Dim sampleX = x + i
                                                ' Clamp to the edge so the kernel stays normalized at borders.
                                                If sampleX < 0 Then sampleX = 0
                                                If sampleX >= width Then sampleX = width - 1

                                                Dim pixel = PixelBuffer.getPixel(sampleX, y)
                                                If pixel IsNot Nothing Then
                                                    blurredColor.addSelf(pixel.Color.multiply(kernel(i + radius)))
                                                End If
                                            Next

                                            result.setPixel(x, y, New PixelData(blurredColor, originalPixel.Depth, originalPixel.Emission))
                                        Next
                                    End Sub)
            _PixelBuffer = result
        End Sub

        Public Overridable Sub blurVertically(radius As Integer)
            kernel = buildKernel(radius)

            Dim result As PixelBuffer = New PixelBuffer(width, height)
            Parallel.For(0, width, Sub(x)
                                        For y = 0 To height - 1
                                            Dim blurredColor As Color = New Color(0, 0, 0)
                                            Dim originalPixel = PixelBuffer.getPixel(x, y)
                                            For i = -radius To radius
                                                Dim sampleY = y + i
                                                ' Clamp to the edge so the kernel stays normalized at borders.
                                                If sampleY < 0 Then sampleY = 0
                                                If sampleY >= height Then sampleY = height - 1

                                                Dim pixel = PixelBuffer.getPixel(x, sampleY)
                                                If pixel IsNot Nothing Then
                                                    blurredColor.addSelf(pixel.Color.multiply(kernel(i + radius)))
                                                End If
                                            Next

                                            result.setPixel(x, y, New PixelData(blurredColor, originalPixel.Depth, originalPixel.Emission))
                                        Next
                                    End Sub)
            _PixelBuffer = result
        End Sub

        Public Overridable Sub blur(radius As Integer, iterations As Integer)
            For i = 0 To iterations - 1
                blurHorizontally(radius)
                blurVertically(radius)
            Next
        End Sub

    End Class

End Namespace
