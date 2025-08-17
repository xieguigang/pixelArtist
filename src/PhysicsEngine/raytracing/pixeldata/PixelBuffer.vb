Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace  raytracing.pixeldata
    ''' <summary>
    ''' The PixelBuffer is an easy way to store PixelData. It holds color, depth and emission information about every pixel </summary>
    Public Class PixelBuffer
        Private pixels As PixelData()()
        Private widthField, heightField As Integer

        Public Sub New(width As Integer, height As Integer)
            widthField = width
            heightField = height

            pixels = RectangularArray.Matrix(Of PixelData)(width, height)
        End Sub

        Public Overridable Sub setPixel(x As Integer, y As Integer, pixelData As PixelData)
            pixels(x)(y) = pixelData
        End Sub

        Public Overridable Function getPixel(x As Integer, y As Integer) As PixelData
            Return pixels(x)(y)
        End Function

        Public Overridable Sub filterByEmission(minEmission As Single)
            For i = 0 To pixels.Length - 1
                For j = 0 To pixels(i).Length - 1
                    Dim pxl = pixels(i)(j)
                    If pxl IsNot Nothing AndAlso pxl.Emission < minEmission Then
                        pixels(i)(j) = New PixelData(Color.BLACK, pxl.Depth, pxl.Emission)
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Changes will be applied to the buffer itself </summary>
        Public Overridable Function add(other As PixelBuffer) As PixelBuffer
            For i = 0 To pixels.Length - 1
                For j = 0 To pixels(i).Length - 1
                    Dim pxl = pixels(i)(j)
                    Dim otherPxl = other.pixels(i)(j)
                    If pxl IsNot Nothing AndAlso otherPxl IsNot Nothing Then
                        'float brightnessB4 = pixels[i][j].getColor().getLuminance();
                        pixels(i)(j).add(otherPxl)
                    End If
                Next
            Next

            Return Me
        End Function

        ''' <summary>
        ''' Changes will be applied to the buffer itself </summary>
        Public Overridable Function multiply(brightness As Single) As PixelBuffer
            For i = 0 To pixels.Length - 1
                For j = 0 To pixels(i).Length - 1
                    pixels(i)(j).multiply(brightness)
                Next
            Next

            Return Me
        End Function

        Public Overridable Function resize(newWidth As Integer, newHeight As Integer, linear As Boolean) As PixelBuffer ' Linear resizing isn't actually implemented yet.
            Dim copy As PixelBuffer = New PixelBuffer(newWidth, newHeight)
            For i = 0 To newWidth - 1
                For j = 0 To newHeight - 1
                    copy.pixels(i)(j) = pixels(CSng(i) / newWidth * widthField)(CSng(j) / newHeight * heightField)
                Next
            Next
            Return copy
        End Function

        Public Overridable ReadOnly Property Width As Integer
            Get
                Return widthField
            End Get
        End Property

        Public Overridable ReadOnly Property Height As Integer
            Get
                Return heightField
            End Get
        End Property

        Public Overridable Sub countEmptyPixels()
            Dim emptyPixels = 0
            For i = 0 To pixels.Length - 1
                If pixels(i) Is Nothing Then
                    emptyPixels += 1
                End If
            Next
            Console.WriteLine("Found " & emptyPixels.ToString() & " empty pixels.")
        End Sub

        Public Function clone() As PixelBuffer
            Dim lClone As PixelBuffer = New PixelBuffer(widthField, heightField)
            For i = 0 To pixels.Length - 1
                Array.Copy(pixels(i), 0, lClone.pixels(i), 0, pixels(i).Length)
            Next
            Return lClone
        End Function
    End Class

End Namespace
