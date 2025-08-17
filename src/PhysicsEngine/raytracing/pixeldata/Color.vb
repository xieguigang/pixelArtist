Imports System
Imports System.Collections.Generic

Namespace  raytracing.pixeldata

    Public Class Color
        Private redField1 As Single
        Private greenField1 As Single
        Private blueField1 As Single

        Public Sub New(red As Single, green As Single, blue As Single)
            If red > 1F OrElse green > 1F OrElse blue > 1F Then
                Throw New ArgumentException("Color parameter(s) outside of expected range")
            End If

            If Single.IsNaN(red) OrElse Single.IsNaN(green) OrElse Single.IsNaN(blue) Then
                Throw New ArgumentException("One or more color parameters are NaN")
            End If

            redField1 = red
            greenField1 = green
            blueField1 = blue
        End Sub

        Public Overridable ReadOnly Property Red As Single
            Get
                Return redField1
            End Get
        End Property

        Public Overridable ReadOnly Property Green As Single
            Get
                Return greenField1
            End Get
        End Property

        Public Overridable ReadOnly Property Blue As Single
            Get
                Return blueField1
            End Get
        End Property

        Public Overridable Function multiply(other As Color) As Color
            Return New Color(redField1 * other.redField1, greenField1 * other.greenField1, blueField1 * other.blueField1)
        End Function

        Public Overridable Function multiply(brightness As Single) As Color
            brightness = System.Math.Min(1, brightness)
            Return New Color(redField1 * brightness, greenField1 * brightness, blueField1 * brightness)
        End Function

        Public Overridable Function add(other As Color) As Color
            Return New Color(System.Math.Min(1, redField1 + other.redField1), System.Math.Min(1, greenField1 + other.greenField1), System.Math.Min(1, blueField1 + other.blueField1))
        End Function

        Public Overridable Sub addSelf(other As Color)
            redField1 = System.Math.Min(1, redField1 + other.redField1)
            greenField1 = System.Math.Min(1, greenField1 + other.greenField1)
            blueField1 = System.Math.Min(1, blueField1 + other.blueField1)
        End Sub

        Public Overridable Function add(brightness As Single) As Color
            Return New Color(System.Math.Min(1, redField1 + brightness), System.Math.Min(1, greenField1 + brightness), System.Math.Min(1, blueField1 + brightness))
        End Function

        Public Overridable ReadOnly Property RGB As Integer
            Get
                Dim redPart As Integer = redField1 * 255
                Dim greenPart As Integer = greenField1 * 255
                Dim bluePart As Integer = blueField1 * 255

                ' Shift bits to right place
                redPart = redPart << 16 And &H00FF0000 'Shift red 16-bits and mask out other stuff
                greenPart = greenPart << 8 And &H0000FF00 'Shift Green 8-bits and mask out other stuff
                bluePart = bluePart And &H000000FF 'Mask out anything not blue.

                Return CInt(&HFF000000UI) Or redPart Or greenPart Or bluePart '0xFF000000 for 100% Alpha. Bitwise OR everything together.
            End Get
        End Property

        ' https://en.wikipedia.org/wiki/Grayscale#Luma_coding_in_video_systems
        Public Overridable ReadOnly Property Luminance As Single
            Get
                Return redField1 * 0.2126F + greenField1 * 0.7152F + blueField1 * 0.0722F
            End Get
        End Property

        Public Shared Function fromInt(argb As Integer) As Color
            Dim b = argb And &HFF
            Dim g = argb >> 8 And &HFF
            Dim r = argb >> 16 And &HFF

            Return New Color(r / 255F, g / 255F, b / 255F)
        End Function

        Public Shared Widening Operator CType(c As Drawing.Color) As Color
            Return New Color(c.R / 255, c.G / 255, c.B / 255)
        End Operator

        Public Shared Function average(colors As ICollection(Of Color)) As Color
            Dim rSum As Single = 0
            Dim gSum As Single = 0
            Dim bSum As Single = 0

            For Each col In colors
                rSum += col.Red
                gSum += col.Green
                bSum += col.Blue
            Next


            Dim colorCount = colors.Count
            Return New Color(rSum / colorCount, gSum / colorCount, bSum / colorCount)
        End Function

        Public Shared Function average(colors As IList(Of Color), weights As IList(Of Single)) As Color
            If colors.Count <> weights.Count Then
                Throw New ArgumentException("Specified color count does not match weight count.")
            End If

            Dim rSum As Single = 0
            Dim gSum As Single = 0
            Dim bSum As Single = 0
            Dim weightSum As Single = 0

            For i = 0 To colors.Count - 1
                Dim col = colors(i)
                Dim weight = weights(i)
                rSum += col.Red * weight
                gSum += col.Green * weight
                bSum += col.Blue * weight
                weightSum += weight
            Next

            Return New Color(rSum / weightSum, gSum / weightSum, bSum / weightSum)
        End Function

        Public Shared Function average(ParamArray colors As Color()) As Color
            Dim rSum As Single = 0
            Dim gSum As Single = 0
            Dim bSum As Single = 0

            For Each col In colors
                rSum += col.Red
                gSum += col.Green
                bSum += col.Blue
            Next

            Dim colorCount = colors.Length
            Return New Color(rSum / colorCount, gSum / colorCount, bSum / colorCount)
        End Function

        Private Shared Function lerp(a As Single, b As Single, t As Single) As Single
            Return a + t * (b - a)
        End Function

        Public Shared Function lerp(a As Color, b As Color, t As Single) As Color
            Return New Color(lerp(a.Red, b.Red, t), lerp(a.Green, b.Green, t), lerp(a.Blue, b.Blue, t))
        End Function

        Public Shared ReadOnly BLACK As Color = New Color(0F, 0F, 0F)
        Public Shared ReadOnly WHITE As Color = New Color(1F, 1F, 1F)
        Public Shared ReadOnly REDField As Color = New Color(1F, 0F, 0F)
        Public Shared ReadOnly GREENField As Color = New Color(0F, 1F, 0F)
        Public Shared ReadOnly BLUEField As Color = New Color(0F, 0F, 1F)
        Public Shared ReadOnly MAGENTA As Color = New Color(1.0F, 0.0F, 1.0F)
        Public Shared ReadOnly GRAY As Color = New Color(0.5F, 0.5F, 0.5F)
        Public Shared ReadOnly DARK_GRAY As Color = New Color(0.2F, 0.2F, 0.2F)
    End Class

End Namespace
