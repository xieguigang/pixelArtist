Namespace  raytracing.pixeldata
    Public Class PixelData
        Private colorField As Color
        Private depthField As Single
        Private emissionField As Single

        Public Sub New(color As Color, depth As Single, emission As Single)
            colorField = color
            depthField = depth
            emissionField = emission
        End Sub

        Public Overridable ReadOnly Property Color As Color
            Get
                Return colorField
            End Get
        End Property

        Public Overridable ReadOnly Property Depth As Single
            Get
                Return depthField
            End Get
        End Property

        Public Overridable ReadOnly Property Emission As Single
            Get
                Return emissionField
            End Get
        End Property

        Public Overridable Sub add(other As PixelData)
            colorField.addSelf(other.colorField)
            depthField = (depthField + other.depthField) / 2F
            emissionField = emissionField + other.emissionField
        End Sub

        Public Overridable Sub multiply(brightness As Single)
            colorField = colorField.multiply(brightness)
        End Sub
    End Class

End Namespace
