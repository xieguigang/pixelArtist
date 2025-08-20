Imports Microsoft.VisualBasic.Imaging.Drawing3D.Models
Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.math

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' https://github.com/carl-vbn/pure-java-raytracer
    ''' </remarks>
    Public Class Ray

        Private originField As Vector3
        Private directionField As Vector3

        Public Sub New(origin As Vector3, direction As Vector3)
            originField = origin

            If direction.length() <> 1 Then
                direction = direction.normalize()
            End If
            directionField = direction
        End Sub

        Public Overridable Function asLine(length As Single) As Line3D
            Return New Line3D(originField, originField.add(directionField.multiply(length)))
        End Function

        Public Overridable ReadOnly Property Origin As Vector3
            Get
                Return originField
            End Get
        End Property

        Public Overridable ReadOnly Property Direction As Vector3
            Get
                Return directionField
            End Get
        End Property
    End Class

End Namespace
