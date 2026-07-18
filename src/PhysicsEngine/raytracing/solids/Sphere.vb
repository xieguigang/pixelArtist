Imports Astrophysics.raytracing.math
Imports Astrophysics.raytracing.pixeldata
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.solids

    Public Class Sphere : Inherits Solid

        Private radius As Single

        Public Sub New(position As vec3, radius As Single, color As Color, reflectivity As Single, emission As Single)
            MyBase.New(position, color, reflectivity, emission)
            Me.radius = radius
        End Sub

        Public Overrides Function calculateIntersection(ray As Ray) As vec3?
            Dim t = vec3.Dot(_Position.Subtract(ray.Origin), ray.Direction)
            Dim p = ray.Origin.Add(ray.Direction.Multiply(t))

            Dim y As Single = _Position.Subtract(p).Length()
            If y < radius Then
                Dim x As Single = System.Math.Sqrt(radius * radius - y * y)
                Dim t1 = t - x
                If t1 > 0 Then
                    Return ray.Origin.Add(ray.Direction.Multiply(t1))
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        Public Overrides Function getNormalAt(point As vec3) As vec3
            Return point.Subtract(_Position).Normalize()
        End Function
    End Class

End Namespace
