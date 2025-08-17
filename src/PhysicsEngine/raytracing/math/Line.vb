Namespace raytracing.math
    Public Class Line
        Public pointA As Vector3
        Public pointB As Vector3

        Public Sub New(pointA As Vector3, pointB As Vector3)
            Me.pointA = pointA
            Me.pointB = pointB
        End Sub

        Public Overridable Function asRay() As Ray
            Return New Ray(pointA, pointB.subtract(pointA).normalize())
        End Function
    End Class

End Namespace
