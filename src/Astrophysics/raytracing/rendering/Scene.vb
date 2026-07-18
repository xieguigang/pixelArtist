Imports Astrophysics.raytracing.math
Imports Astrophysics.raytracing.solids
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.rendering

    Public Class Scene

        ReadOnly solids As List(Of Solid)

        Public Overridable ReadOnly Property Camera As Camera
        Public Overridable ReadOnly Property Light As Light
        Public Overridable ReadOnly Property Skybox As Skybox

        Public Sub New()
            solids = New List(Of Solid)()
            Camera = New Camera()
            Light = New Light(New vec3(-1, 2, -1))
            Skybox = New Skybox("Sky.jpg")
        End Sub

        Public Overridable Sub addSolid(solid As Solid)
            solids.Add(solid)
        End Sub

        Public Overridable Sub clearSolids()
            solids.Clear()
        End Sub

        Public Overridable Function raycast(ray As Ray) As RayHit
            Dim closestHit As RayHit = Nothing
            For Each solid In solids
                If solid Is Nothing Then
                    Continue For
                End If

                Dim hitPos = solid.calculateIntersection(ray)
                If hitPos IsNot Nothing AndAlso (closestHit Is Nothing OrElse vec3.Distance(closestHit.Position, ray.Origin) > vec3.Distance(hitPos, ray.Origin)) Then
                    closestHit = New RayHit(ray, solid, hitPos)
                End If
            Next
            Return closestHit
        End Function

    End Class

End Namespace
