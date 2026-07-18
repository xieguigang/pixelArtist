Namespace raytracing

    Module Extensions

        <Extension>
        Public Function asRay(line As Line3D) As Ray
            Return New Ray(line.a, line.b.subtract(line.a).normalize())
        End Function
    End Module
End Namespace