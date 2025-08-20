Imports PhysicsEngine.raytracing.math
Imports PhysicsEngine.raytracing.pixeldata
Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.solids

    Public Class Plane : Inherits Solid

        Private checkerPattern As Boolean

        Public Sub New(height As Single, color As Color, checkerPattern As Boolean, reflectivity As Single, emission As Single)
            MyBase.New(New Vector3(0, height, 0), color, reflectivity, emission)
            Me.checkerPattern = checkerPattern
        End Sub

        Public Overrides Function calculateIntersection(ray As Ray) As Vector3?
            Dim t = -(ray.Origin.Y - _Position.Y) / ray.Direction.Y
            If t > 0 AndAlso Single.IsFinite(t) Then
                Return ray.Origin.add(ray.Direction.multiply(t))
            End If

            Return Nothing
        End Function

        Public Overrides Function getNormalAt(point As Vector3) As Vector3
            Return New Vector3(0, 1, 0)
        End Function

        Public Overrides Function getTextureColor(point As Vector3) As Color
            If checkerPattern Then
                ' in first or third quadrant of the checkerplane
                If point.X > 0 And point.Z > 0 OrElse point.X < 0 And point.Z < 0 Then
                    If CInt(point.X) Mod 2 = 0 Xor CInt(point.Z) Mod 2 <> 0 Then
                        Return Color.GRAY
                    Else
                        Return Color.DARK_GRAY
                    End If
                Else
                    ' in second or fourth quadrant of the checkerplane
                    If CInt(point.X) Mod 2 = 0 Xor CInt(point.Z) Mod 2 <> 0 Then
                        Return Color.DARK_GRAY
                    Else
                        Return Color.GRAY
                    End If
                End If
            Else
                Return MyBase.Color
            End If
        End Function

    End Class

End Namespace
