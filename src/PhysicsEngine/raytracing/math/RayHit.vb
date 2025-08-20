Imports PhysicsEngine.raytracing.solids
Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.math

    Public Class RayHit

        Public Overridable ReadOnly Property Ray As Ray
        Public Overridable ReadOnly Property Solid As Solid
        Public Overridable ReadOnly Property Position As Vector3
        Public Overridable ReadOnly Property Normal As Vector3

        Public Sub New(ray As Ray, hitSolid As Solid, hitPos As Vector3)
            _Ray = ray
            _Solid = hitSolid
            _Position = hitPos
            _Normal = hitSolid.getNormalAt(hitPos)
        End Sub

    End Class
End Namespace
