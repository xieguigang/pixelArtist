Imports PhysicsEngine.raytracing.math
Imports PhysicsEngine.raytracing.pixeldata

Namespace raytracing.solids

    Public MustInherit Class Solid
        Protected Friend positionField As Vector3
        Protected Friend colorField As Color
        Protected Friend reflectivityField As Single
        Protected Friend emissionField As Single

        Public Sub New(position As Vector3, color As Color, reflectivity As Single, emission As Single)
            positionField = position
            colorField = color
            reflectivityField = reflectivity
            emissionField = emission
        End Sub

        Public MustOverride Function calculateIntersection(ray As Ray) As Vector3
        Public MustOverride Function getNormalAt(point As Vector3) As Vector3

        Public Overridable ReadOnly Property Position As Vector3
            Get
                Return positionField
            End Get
        End Property

        Public Overridable ReadOnly Property Color As Color
            Get
                Return colorField
            End Get
        End Property

        Public Overridable Function getTextureColor(point As Vector3) As Color
            Return Color
        End Function

        Public Overridable ReadOnly Property Reflectivity As Single
            Get
                Return reflectivityField
            End Get
        End Property

        Public Overridable ReadOnly Property Emission As Single
            Get
                Return emissionField
            End Get
        End Property
    End Class

End Namespace
