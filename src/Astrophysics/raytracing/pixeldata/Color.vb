Namespace raytracing.pixeldata

    ''' <summary>
    ''' Color data in float type
    ''' </summary>
    Public Class Color

        Public Overridable ReadOnly Property Red As Single
        Public Overridable ReadOnly Property Green As Single
        Public Overridable ReadOnly Property Blue As Single

        Public Sub New(red As Single, green As Single, blue As Single)
            If red > 1.0F OrElse green > 1.0F OrElse blue > 1.0F Then
                Throw New ArgumentException("Color parameter(s) outside of expected range")
            End If

            If Single.IsNaN(red) OrElse Single.IsNaN(green) OrElse Single.IsNaN(blue) Then
                Throw New ArgumentException("One or more color parameters are NaN")
            End If

            _Red = red
            _Green = green
            _Blue = blue
        End Sub

        Public Overridable Function multiply(other As Color) As Color
            Return New Color(Red * other.Red, Green * other.Green, Blue * other.Blue)
        End Function

        Public Overridable Function multiply(brightness As Single) As Color
            brightness = System.Math.Min(1, brightness)
            Return New Color(Red * brightness, Green * brightness, Blue * brightness)
        End Function

        Public Overridable Function add(other As Color) As Color
            Return New Color(System.Math.Min(1, Red + other.Red), System.Math.Min(1, Green + other.Green), System.Math.Min(1, Blue + other.Blue))
        End Function

        Public Overridable Sub addSelf(other As Color)
            _Red = System.Math.Min(1, Red + other.Red)
            _Green = System.Math.Min(1, Green + other.Green)
            _Blue = System.Math.Min(1, Blue + other.Blue)
        End Sub

        Public Overridable Function add(brightness As Single) As Color
            Return New Color(System.Math.Min(1, Red + brightness), System.Math.Min(1, Green + brightness), System.Math.Min(1, Blue + brightness))
        End Function

        ' 0xFF000000 as a signed 32-bit integer (avoids OverflowException from a UInteger literal).
        Private Const ALPHA As Integer = -16777216

        Public Overridable ReadOnly Property RGB As Integer
            Get
                Dim redPart As Integer = CInt(Red * 255)
                Dim greenPart As Integer = CInt(Green * 255)
                Dim bluePart As Integer = CInt(Blue * 255)

                ' Shift bits to right place
                redPart = redPart << 16 And &HFF0000 'Shift red 16-bits and mask out other stuff
                greenPart = greenPart << 8 And &HFF00 'Shift Green 8-bits and mask out other stuff
                bluePart = bluePart And &HFF 'Mask out anything not blue.

                Return ALPHA Or redPart Or greenPart Or bluePart '100% Alpha. Bitwise OR everything together.
            End Get
        End Property

        ' https://en.wikipedia.org/wiki/Grayscale#Luma_coding_in_video_systems
        Public Overridable ReadOnly Property Luminance As Single
            Get
                Return Red * 0.2126F + Green * 0.7152F + Blue * 0.0722F
            End Get
        End Property

        Public Shared Function fromInt(argb As Integer) As Color
            Dim b = argb And &HFF
            Dim g = argb >> 8 And &HFF
            Dim r = argb >> 16 And &HFF

            Return New Color(r / 255.0F, g / 255.0F, b / 255.0F)
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
        Public Shared ReadOnly WHITE As Color = New Color(1.0F, 1.0F, 1.0F)
        Public Shared ReadOnly REDField As Color = New Color(1.0F, 0F, 0F)
        Public Shared ReadOnly GREENField As Color = New Color(0F, 1.0F, 0F)
        Public Shared ReadOnly BLUEField As Color = New Color(0F, 0F, 1.0F)
        Public Shared ReadOnly MAGENTA As Color = New Color(1.0F, 0.0F, 1.0F)
        Public Shared ReadOnly GRAY As Color = New Color(0.5F, 0.5F, 0.5F)
        Public Shared ReadOnly DARK_GRAY As Color = New Color(0.2F, 0.2F, 0.2F)
    End Class

End Namespace
