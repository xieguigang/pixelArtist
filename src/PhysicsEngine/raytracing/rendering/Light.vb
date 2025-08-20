Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.rendering

    Public Class Light
        Private positionField As Vector3

        Public Sub New(position As Vector3)
            positionField = position
        End Sub

        Public Overridable Property Position As Vector3
            Get
                Return positionField
            End Get
            Set(value As Vector3)
                positionField = value
            End Set
        End Property

    End Class

End Namespace
