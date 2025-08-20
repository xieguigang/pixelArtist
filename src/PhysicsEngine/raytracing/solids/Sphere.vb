Imports PhysicsEngine.raytracing.math
Imports PhysicsEngine.raytracing.pixeldata
Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.solids

    Public Class Sphere
        Inherits Solid
        Private radius As Single

        Public Sub New(position As Vector3, radius As Single, color As Color, reflectivity As Single, emission As Single)
            MyBase.New(position, color, reflectivity, emission)
            Me.radius = radius
        End Sub

        Public Overrides Function calculateIntersection(ray As Ray) As Vector3
            Dim t = Vector3.dot(positionField.subtract(ray.Origin), ray.Direction)
            Dim p = ray.Origin.add(ray.Direction.multiply(t))

            Dim y As Single = positionField.subtract(p).length()
            If y < radius Then
                Dim x As Single = System.Math.Sqrt(radius * radius - y * y)
                Dim t1 = t - x
                If t1 > 0 Then
                    Return ray.Origin.add(ray.Direction.multiply(t1))
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        Public Overrides Function getNormalAt(point As Vector3) As Vector3
            Return point.subtract(positionField).normalize()
        End Function
    End Class

End Namespace
