Imports PhysicsEngine.raytracing.math

Namespace raytracing.rendering

    Public Class Camera
        Private positionField As Vector3
        Private yawField As Single
        Private pitchField As Single
        Private fieldOfVision As Single

        Public Sub New()
            positionField = New Vector3(0, 0, 0)
            yawField = 0
            pitchField = 0
            fieldOfVision = 60
        End Sub

        Public Overridable Property Position As Vector3
            Get
                Return positionField
            End Get
            Set(value As Vector3)
                positionField = value
            End Set
        End Property


        Public Overridable Property Yaw As Single
            Get
                Return yawField
            End Get
            Set(value As Single)
                yawField = value
            End Set
        End Property


        Public Overridable Property Pitch As Single
            Get
                Return pitchField
            End Get
            Set(value As Single)
                pitchField = value
            End Set
        End Property


        Public Overridable Sub translate(vec As Vector3)
            positionField.translate(vec)
        End Sub

        Public Overridable Property FOV As Single
            Set(value As Single)
                fieldOfVision = value
            End Set
            Get
                Return fieldOfVision
            End Get
        End Property

    End Class

End Namespace
