Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.rendering

    Public Class Light

        Public Overridable Property Position As Vector3

        Public Sub New(position As Vector3)
            _Position = position
        End Sub

        Public Overrides Function ToString() As String
            Return $"light {Position}"
        End Function

    End Class

End Namespace
