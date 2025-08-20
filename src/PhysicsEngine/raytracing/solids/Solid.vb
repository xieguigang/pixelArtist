Imports PhysicsEngine.raytracing.math
Imports PhysicsEngine.raytracing.pixeldata
Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.solids

    Public MustInherit Class Solid

        Public Overridable ReadOnly Property Position As Vector3
            Get
                Return _Position
            End Get
        End Property

        Public Overridable ReadOnly Property Color As Color
            Get
                Return _Color
            End Get
        End Property

        Public Overridable ReadOnly Property Reflectivity As Single
            Get
                Return _Reflectivity
            End Get
        End Property

        Public Overridable ReadOnly Property Emission As Single
            Get
                Return _Emission
            End Get
        End Property

        Protected _Position As Vector3
        Protected _Color As Color
        Protected _Reflectivity As Single
        Protected _Emission As Single

        Public Sub New(position As Vector3, color As Color, reflectivity As Single, emission As Single)
            _Position = position
            _Color = color
            _Reflectivity = reflectivity
            _Emission = emission
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ray"></param>
        ''' <returns>
        ''' nothing means no intersection, otherwise the intersection point of the ray with the solid.
        ''' </returns>
        Public MustOverride Function calculateIntersection(ray As Ray) As Vector3?
        Public MustOverride Function getNormalAt(point As Vector3) As Vector3

        Public Overridable Function getTextureColor(point As Vector3) As Color
            Return Color
        End Function

    End Class

End Namespace
