Imports PhysicsEngine.raytracing.math
Imports PhysicsEngine.raytracing.solids

Namespace raytracing.rendering

    Public Class Scene
        Private cameraField As Camera
        Private lightField As Light
        Private solids As List(Of Solid)
        Private skyboxField As Skybox

        Public Sub New()
            solids = New List(Of Solid)()
            cameraField = New Camera()
            lightField = New Light(New Vector3(-1, 2, -1))
            skyboxField = New Skybox("Sky.jpg")
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
                If hitPos IsNot Nothing AndAlso (closestHit Is Nothing OrElse Vector3.distance(closestHit.Position, ray.Origin) > Vector3.distance(hitPos, ray.Origin)) Then
                    closestHit = New RayHit(ray, solid, hitPos)
                End If
            Next
            Return closestHit
        End Function

        Public Overridable ReadOnly Property Camera As Camera
            Get
                Return cameraField
            End Get
        End Property

        Public Overridable ReadOnly Property Light As Light
            Get
                Return lightField
            End Get
        End Property

        Public Overridable ReadOnly Property Skybox As Skybox
            Get
                Return skyboxField
            End Get
        End Property
    End Class

End Namespace
