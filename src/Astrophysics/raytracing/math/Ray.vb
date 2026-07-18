Imports Microsoft.VisualBasic.Imaging.Drawing3D.Models
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.math

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' https://github.com/carl-vbn/pure-java-raytracer
    ''' </remarks>
    Public Class Ray

        Public Overridable ReadOnly Property Origin As vec3
        Public Overridable ReadOnly Property Direction As vec3

        Public Sub New(origin As vec3, direction As vec3)
            _Origin = origin

            If direction.Length() <> 1 Then
                direction = direction.Normalize()
            End If
            _Direction = direction
        End Sub

        Public Overridable Function asLine(length As Single) As Line3D
            Return New Line3D(Origin, Origin.add(Direction.multiply(length)))
        End Function

    End Class

End Namespace
