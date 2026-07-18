Imports Astrophysics.raytracing.solids
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.math

    Public Class RayHit

        Public Overridable ReadOnly Property Ray As Ray
        Public Overridable ReadOnly Property Solid As Solid
        Public Overridable ReadOnly Property Position As vec3
        Public Overridable ReadOnly Property Normal As vec3

        Public Sub New(ray As Ray, hitSolid As Solid, hitPos As vec3)
            _Ray = ray
            _Solid = hitSolid
            _Position = hitPos
            _Normal = hitSolid.getNormalAt(hitPos)
        End Sub

    End Class
End Namespace
