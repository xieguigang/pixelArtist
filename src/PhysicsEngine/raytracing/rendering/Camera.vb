Imports PhysicsEngine.raytracing.math

Namespace raytracing.rendering

    Public Class Camera

        Public Sub New()
            Position = New Vector3(0, 0, 0)
            Yaw = 0
            Pitch = 0
            FOV = 60
        End Sub

        Public Overridable Property Position As Vector3

        Public Overridable Property Yaw As Single

        Public Overridable Property Pitch As Single

        Public Overridable Sub translate(vec As Vector3)
            Position = Position.translate(vec)
        End Sub

        ''' <summary>
        ''' field Of Vision
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property FOV As Single

    End Class

End Namespace
