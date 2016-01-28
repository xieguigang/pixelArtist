Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public Class GameEngine : Inherits Engine

    Dim sanek As Snake = New Snake

    Sub New(device As DisplayPort)
        Call MyBase.New(device)
    End Sub

    Public Overrides Sub ClickObject(pos As Point, x As GraphicUnit)

    End Sub

    Public Overrides Sub Invoke(control As Controls, raw As Char)
        Select Case control
            Case Controls.Up, Controls.Right, Controls.Left, Controls.Down
                sanek.Direction = control
            Case Controls.Fire
            Case Controls.Pause
            Case Controls.Menu
        End Select
    End Sub

    Protected Overrides Sub __worldReacts()

    End Sub

    Protected Overrides Sub GraphicsDeviceResize()

    End Sub

    Public Overrides Function Init() As Boolean
        Call ControlMaps.DefaultMaps(Me.ControlsMap.ControlMaps)

        Call Me.Add(sanek)

        Return True
    End Function
End Class
