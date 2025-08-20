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

        Public Overridable ReadOnly Property Origin As Vector3
        Public Overridable ReadOnly Property Direction As Vector3

        Public Sub New(origin As Vector3, direction As Vector3)
            _Origin = origin

            If direction.length() <> 1 Then
                direction = direction.normalize()
            End If
            _Direction = direction
        End Sub

        Public Overridable Function asLine(length As Single) As Line3D
            Return New Line3D(Origin, Origin.add(Direction.multiply(length)))
        End Function

    End Class

End Namespace
