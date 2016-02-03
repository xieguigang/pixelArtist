Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.Commons
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public Class DinosaurRun : Inherits GameEngine

    ''' <summary>
    ''' 地平线的位置
    ''' </summary>
    Dim horrizon As Integer = 350
    Dim _dinosaur As New Dinosaur With {.Location = New Point(100, horrizon)}
    Dim _lstCactus As New List(Of Cactus)
    Dim _lands As GroundLand

    Sub New(Display As DisplayPort)
        Call MyBase.New(Display)
    End Sub

    Public Overrides Sub Invoke(control As Controls, raw As Char)
        Select Case control
            Case Controls.Jumps : Call _dinosaur.Jump()
        End Select
    End Sub

    Protected Overrides Sub __worldReacts()
        If __happens(5) Then
            Dim cactus As New Cactus With {.Location = New Point(Me.GraphicRegion.Width, horrizon)}
            Call Me.Add(cactus)
            Call _lstCactus.Add(cactus)
            Call cactus.OffsetHeight()
        End If

        If __happens(4) Then
            Dim cl As New Cloud With {.Location = New Point(Me.GraphicRegion.Width, (horrizon - 30) * _rnd.NextDouble)}
            Call Me.Add(cl)
        End If

        If __happens(5) Then
            Dim st As New Stone With {.Location = New Point(Me.GraphicRegion.Width, horrizon + 2)}
            Call Me.Add(st)
        End If

        For Each x In _lstCactus
            If _dinosaur.Region.IntersectsWith(x.Region) Then
                Call Score.Reset
            End If
        Next

        Score.Score += 1

        For Each x In Me
            If x.OutBounds Then
                Call Me.Remove(x)
                Call x.Free
            End If
        Next
    End Sub

    Protected Overrides Sub __GraphicsDeviceResize()

    End Sub

    Public Overrides Function Init() As Boolean
        Call ControlMaps.DefaultMaps(Me.ControlsMap.ControlMaps)
        Me._lands = New GroundLand(Function() Me.GraphicRegion.Width) With {.Location = New Point(0, horrizon + 1)}
        Call Me.Add(_lands)

        Dim score As New Score With {.Location = New Point(10, 10)}

        Call Me.Add(score)
        Call Me.Add(_dinosaur)
        Call _dinosaur.OffsetHeight()

        Me.Score = score
        Me.DisplayDriver.RefreshHz = 60
        Return True
    End Function

    Public Overrides Sub ClickObject(pos As Point, x As GraphicUnit)
        MsgBox(x.ToString)
    End Sub

    Public Overrides Sub __reset()
    End Sub

    Public Overrides Sub __restart()

    End Sub
End Class
