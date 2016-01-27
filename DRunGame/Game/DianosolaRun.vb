Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public Class DianosolaRun : Inherits Engine

    ''' <summary>
    ''' 地平线的位置
    ''' </summary>
    Dim horrizon As Integer = 350

    Dim draon As New Dragon With {.Location = New Point(100, horrizon)}
    Dim xrzhs As New List(Of xrzh)
    Dim ground As Earth

    Sub New(Display As DisplayPort)
        Call MyBase.New(Display)
    End Sub

    Public Overrides Sub Invoke(control As Controls, raw As Char)
        Select Case control
            Case Controls.Jumps : Call draon.Jump()
        End Select
    End Sub

    Protected Overrides Sub __worldReacts()
        If __happens(5) Then
            Dim zrz As New xrzh With {.Location = New Point(Me.GraphicRegion.Width, horrizon)}
            Call Me.Add(zrz)
            Call xrzhs.Add(zrz)
            Call zrz.OffsetHeight()
        End If

        If __happens(4) Then
            Dim cl As New Cloud With {.Location = New Point(Me.GraphicRegion.Width, (horrizon - 30) * _rnd.NextDouble)}
            Call Me.Add(cl)
        End If

        If __happens(5) Then
            Dim st As New Stone With {.Location = New Point(Me.GraphicRegion.Width, horrizon + 2)}
            Call Me.Add(st)
        End If

        For Each x In xrzhs
            If draon.Region.IntersectsWith(x.Region) Then
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

    Protected Overrides Sub GraphicsDeviceResize()

    End Sub

    Public Overrides Function Init() As Boolean
        Call ControlMaps.DefaultMaps(Me.ControlsMap.ControlMaps)
        Me.ground = New Earth(Function() Me.GraphicRegion.Width) With {.Location = New Point(0, horrizon + 1)}
        Call Me.Add(ground)

        Dim score As New Score With {.Location = New Point(10, 10)}

        Call Me.Add(score)
        Call Me.Add(draon)
        Call draon.OffsetHeight()

        Me.Score = score
        Me.DisplayDriver.RefreshHz = 60
        Return True
    End Function

    Public Overrides Sub ClickObject(pos As Point, x As GraphicUnit)
        MsgBox(x.ToString)
    End Sub
End Class
