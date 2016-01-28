Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.Commons
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.GamePads.SoundDriver

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
        If sanek.Head.IntersectsWith(food.Region) Then
            Call Me.Remove(food)
            Call AddFood()
            sanek.Append()
            Call Beep()
        End If
    End Sub

    Protected Overrides Sub GraphicsDeviceResize()

    End Sub

    Dim food As Food

    Private Sub AddFood()
        food = New Food With {.Location = New Point(GraphicRegion.Width * RandomDouble(), GraphicRegion.Height * RandomDouble())}
        Call Me.Add(food)
        Score.Score += 1
    End Sub

    Public Overrides Function Init() As Boolean
        Call ControlMaps.DefaultMaps(Me.ControlsMap.ControlMaps)

        sanek.Location = Me.GraphicRegion.GetCenter


        Dim score As New Score With {.Location = New Point(10, 10)}

        Call Me.Add(score)

        Me.Score = score

        Call sanek.init()
        Call Me.Add(sanek)
        Call AddFood()

        Call WinMM.PlaySound("./background.mp3")

        Return True
    End Function
End Class
