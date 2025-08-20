Namespace raytracing.pixeldata

    Public Class PixelData

        Public Overridable ReadOnly Property Color As Color
        Public Overridable ReadOnly Property Depth As Single
        Public Overridable ReadOnly Property Emission As Single

        Public Sub New(color As Color, depth As Single, emission As Single)
            _Color = color
            _Depth = depth
            _Emission = emission
        End Sub

        Public Overridable Sub add(other As PixelData)
            _Color.addSelf(other.Color)
            _Depth = (Depth + other.Depth) / 2.0F
            _Emission = Emission + other.Emission
        End Sub

        Public Overridable Sub multiply(brightness As Single)
            _Color = Color.multiply(brightness)
        End Sub
    End Class

End Namespace
