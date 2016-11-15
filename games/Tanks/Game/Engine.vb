Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public Class Engine : Inherits GameEngine

    Sub New(display As DisplayPort)
        Call MyBase.New(display)
    End Sub

    Public Overrides Sub __reset()

    End Sub

    Public Overrides Sub __restart()

    End Sub

    Public Overrides Sub ClickObject(pos As Point, x As GraphicUnit)

    End Sub

    Public Overrides Sub Invoke(control As Controls, raw As Char)
        Select Case control
            Case Controls.Up, Controls.Down, Controls.Left, Controls.Right
                user.Direction = control
        End Select
    End Sub

    Protected Overrides Sub __GraphicsDeviceResize()

    End Sub

    Dim user As Tank

    Protected Overrides Sub __worldReacts()

    End Sub

    Public Overrides Function Init() As Boolean
        Call ControlMaps.DefaultMaps(Me.ControlsMap.ControlMaps)

        user = New Tank With {.Location = New Point(100, 100)}
        Call Me.Add(user)

        Return True
    End Function
End Class
