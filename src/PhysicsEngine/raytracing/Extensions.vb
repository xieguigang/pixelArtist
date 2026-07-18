Imports System.Runtime.CompilerServices
Imports Astrophysics.raytracing.math
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Models

Namespace raytracing

    <HideModuleName>
    Module Extensions

        <Extension>
        Public Function asRay(line As Line3D) As Ray
            Return New Ray(line.a, line.b.Subtract(line.a).Normalize())
        End Function
    End Module
End Namespace