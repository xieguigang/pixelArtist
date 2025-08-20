Imports PhysicsEngine.raytracing.solids
Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.math

    Public Class RayHit
        Private rayField As Ray
        Private hitSolid As Solid
        Private hitPos As Vector3
        Private normalField As Vector3

        Public Sub New(ray As Ray, hitSolid As Solid, hitPos As Vector3)
            rayField = ray
            Me.hitSolid = hitSolid
            Me.hitPos = hitPos
            normalField = hitSolid.getNormalAt(hitPos)
        End Sub

        Public Overridable ReadOnly Property Ray As Ray
            Get
                Return rayField
            End Get
        End Property

        Public Overridable ReadOnly Property Solid As Solid
            Get
                Return hitSolid
            End Get
        End Property

        Public Overridable ReadOnly Property Position As Vector3
            Get
                Return hitPos
            End Get
        End Property

        Public Overridable ReadOnly Property Normal As Vector3
            Get
                Return normalField
            End Get
        End Property
    End Class

End Namespace
