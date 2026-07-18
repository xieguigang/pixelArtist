Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports System.Threading.Tasks

Namespace raytracing.pixeldata

    ''' <summary>
    ''' The PixelBuffer is an easy way to store PixelData. 
    ''' It holds color, depth and emission information 
    ''' about every pixel.
    ''' </summary>
    Public Class PixelBuffer

        Dim pixels As PixelData()()

        Public Overridable ReadOnly Property Width As Integer
        Public Overridable ReadOnly Property Height As Integer

        Public Sub New(width As Integer, height As Integer)
            _Width = width
            _Height = height

            pixels = RectangularArray.Matrix(Of PixelData)(width, height)
        End Sub

        Public Overridable Sub setPixel(x As Integer, y As Integer, pixelData As PixelData)
            pixels(x)(y) = pixelData
        End Sub

        Public Overridable Function getPixel(x As Integer, y As Integer) As PixelData
            Return pixels(x)(y)
        End Function

        Public Overridable Sub filterByEmission(minEmission As Single)
            Parallel.For(0, pixels.Length, Sub(i)
                                            For j = 0 To pixels(i).Length - 1
                                                Dim pxl = pixels(i)(j)
                                                If pxl IsNot Nothing AndAlso pxl.Emission < minEmission Then
                                                    pixels(i)(j) = New PixelData(Color.BLACK, pxl.Depth, pxl.Emission)
                                                End If
                                            Next
                                        End Sub)
        End Sub

        ''' <summary>
        ''' Changes will be applied to the buffer itself </summary>
        Public Overridable Function add(other As PixelBuffer) As PixelBuffer
            Parallel.For(0, pixels.Length, Sub(i)
                                            For j = 0 To pixels(i).Length - 1
                                                Dim pxl = pixels(i)(j)
                                                Dim otherPxl = other.pixels(i)(j)
                                                If pxl IsNot Nothing AndAlso otherPxl IsNot Nothing Then
                                                    pixels(i)(j).add(otherPxl)
                                                End If
                                            Next
                                        End Sub)

            Return Me
        End Function

        ''' <summary>
        ''' Changes will be applied to the buffer itself </summary>
        Public Overridable Function multiply(brightness As Single) As PixelBuffer
            Parallel.For(0, pixels.Length, Sub(i)
                                            For j = 0 To pixels(i).Length - 1
                                                pixels(i)(j).multiply(brightness)
                                            Next
                                        End Sub)

            Return Me
        End Function

        Public Overridable Function resize(newWidth As Integer, newHeight As Integer, linear As Boolean) As PixelBuffer
            Dim copy As PixelBuffer = New PixelBuffer(newWidth, newHeight)
            For i = 0 To newWidth - 1
                For j = 0 To newHeight - 1
                    If linear Then
                        copy.pixels(i)(j) = sampleBilinear(CSng(i) / newWidth * _Width, CSng(j) / newHeight * _Height)
                    Else
                        Dim sx = CInt(System.Math.Round(CSng(i) / newWidth * _Width))
                        Dim sy = CInt(System.Math.Round(CSng(j) / newHeight * _Height))
                        sx = System.Math.Max(0, System.Math.Min(_Width - 1, sx))
                        sy = System.Math.Max(0, System.Math.Min(_Height - 1, sy))
                        copy.pixels(i)(j) = pixels(sx)(sy)
                    End If
                Next
            Next
            Return copy
        End Function

        Private Function sampleBilinear(fx As Single, fy As Single) As PixelData
            Dim x0 = System.Math.Max(0, CInt(System.Math.Floor(fx)))
            Dim y0 = System.Math.Max(0, CInt(System.Math.Floor(fy)))
            Dim x1 = System.Math.Min(_Width - 1, x0 + 1)
            Dim y1 = System.Math.Min(_Height - 1, y0 + 1)
            Dim tx = fx - x0
            Dim ty = fy - y0

            Dim p00 = If(pixels(x0)(y0), New PixelData(Color.BLACK, 0, 0))
            Dim p10 = If(pixels(x1)(y0), New PixelData(Color.BLACK, 0, 0))
            Dim p01 = If(pixels(x0)(y1), New PixelData(Color.BLACK, 0, 0))
            Dim p11 = If(pixels(x1)(y1), New PixelData(Color.BLACK, 0, 0))

            ' Bilinear blend of the color; depth/emission are carried from the
            ' nearest sample to avoid mixing +Infinity (sky) depths.
            Dim top = Color.lerp(p00.Color, p10.Color, tx)
            Dim bottom = Color.lerp(p01.Color, p11.Color, tx)
            Dim blended = Color.lerp(top, bottom, ty)

            Return New PixelData(blended, p00.Depth, p00.Emission)
        End Function

        Public Overridable Sub countEmptyPixels()
            Dim emptyPixels = 0
            For i = 0 To _Width - 1
                For j = 0 To _Height - 1
                    If pixels(i)(j) Is Nothing Then
                        emptyPixels += 1
                    End If
                Next
            Next
            Console.WriteLine("Found " & emptyPixels.ToString() & " empty pixels.")
        End Sub

        Public Function clone() As PixelBuffer
            Dim lClone As New PixelBuffer(_Width, _Height)

            Parallel.For(0, _Width, Sub(i)
                                        For j As Integer = 0 To _Height - 1
                                            Dim p = pixels(i)(j)
                                            If p IsNot Nothing Then
                                                lClone.pixels(i)(j) = New PixelData(p.Color, p.Depth, p.Emission)
                                            End If
                                        Next
                                    End Sub)

            Return lClone
        End Function
    End Class

End Namespace
