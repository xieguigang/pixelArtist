Imports PhysicsEngine.raytracing.math
Imports PhysicsEngine.raytracing.pixeldata
Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.solids

    ' Adapted from:
    ' http://ray-tracing-conept.blogspot.com/2015/01/ray-box-intersection-and-normal.html
    Public Class Box : Inherits Solid

        Private min, max As Vector3

        Public Sub New(position As Vector3, scale As Vector3, color As Color, reflectivity As Single, emission As Single)
            MyBase.New(position, color, reflectivity, emission)
            max = position.add(scale.multiply(0.5F))
            min = position.subtract(scale.multiply(0.5F))
        End Sub

        Public Overrides Function calculateIntersection(ray As Ray) As Vector3?
            Dim t1, t2, temp As Single, tnear = Single.NegativeInfinity, tfar = Single.PositiveInfinity
            Dim intersectFlag = True
            Dim rayDirection As Single() = ray.Direction.ToArray()
            Dim rayOrigin As Single() = ray.Origin.ToArray()
            Dim b1 As Single() = min.ToArray()
            Dim b2 As Single() = max.ToArray()

            For i = 0 To 2
                If rayDirection(i) = 0 Then
                    If rayOrigin(i) < b1(i) OrElse rayOrigin(i) > b2(i) Then
                        intersectFlag = False
                    End If
                Else
                    t1 = (b1(i) - rayOrigin(i)) / rayDirection(i)
                    t2 = (b2(i) - rayOrigin(i)) / rayDirection(i)
                    If t1 > t2 Then
                        temp = t1
                        t1 = t2
                        t2 = temp
                    End If
                    If t1 > tnear Then
                        tnear = t1
                    End If
                    If t2 < tfar Then
                        tfar = t2
                    End If
                    If tnear > tfar Then
                        intersectFlag = False
                    End If
                    If tfar < 0 Then
                        intersectFlag = False
                    End If
                End If
            Next
            If intersectFlag Then
                Return ray.Origin.add(ray.Direction.multiply(tnear))
            Else
                Return Nothing
            End If
        End Function


        Public Overridable Function contains(point As Vector3) As Boolean
            Return point.X >= min.X AndAlso point.Y >= min.Y AndAlso point.Z >= min.Z AndAlso point.X <= max.X AndAlso point.Y <= max.Y AndAlso point.Z <= max.Z
        End Function

        Public Overrides Function getNormalAt(point As Vector3) As Vector3
            Dim direction As Single() = point.subtract(_Position).ToArray()
            Dim biggestValue = Single.NaN

            For i = 0 To 2
                If Single.IsNaN(biggestValue) OrElse biggestValue < System.Math.Abs(direction(i)) Then
                    biggestValue = System.Math.Abs(direction(i))
                End If
            Next

            If biggestValue = 0 Then
                Return New Vector3(0, 0, 0)
            Else
                For i = 0 To 2
                    If System.Math.Abs(direction(i)) = biggestValue Then
                        Dim normal = New Single() {0, 0, 0}
                        normal(i) = If(direction(i) > 0, 1, -1)

                        Return New Vector3(normal(0), normal(1), normal(2))
                    End If
                Next
            End If

            Return New Vector3(0, 0, 0)
        End Function
    End Class

End Namespace
