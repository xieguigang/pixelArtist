Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.Commons
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.GamePads.SoundDriver

Public Class GameEngine : Inherits GamePads.GameEngine

    Public ReadOnly Property Snake As Snake
        Get
            Return _snake
        End Get
    End Property

    Dim _snake As Snake = New Snake

    Sub New(device As DisplayPort)
        Call MyBase.New(device)
    End Sub

    Public Overrides Sub ClickObject(pos As Point, x As GraphicUnit)
        If TypeOf x Is Button Then
            Call Reset()
        End If
    End Sub

    Public Overrides Sub Invoke(control As Controls, raw As Char)
        Select Case control
            Case Controls.Up, Controls.Right, Controls.Left, Controls.Down
                _snake.Direction = control
            Case Controls.Ok
                Call Restart()
            Case Controls.Pause
                Call Pause()
            Case Controls.Menu
        End Select
    End Sub

    Public Property GameOver As Boolean = False

    Public ReadOnly Property EatFood As Boolean
        Get
            Return _snake.Head.IntersectsWith(food.Region)
        End Get
    End Property

    Public Property PauseEnable As Boolean = True

    Protected Overrides Sub __worldReacts()
        If EatFood Then
            If Not ScoreCallback Is Nothing Then
                Call ScoreCallback()(food.Location)
            End If

            Call Me.Remove(food)
            Call AddFood()
            _snake.Append()
            Call Beep()
        End If

        If Not GraphicRegion.Contains(_snake.Head.Location) OrElse
            _snake.EatSelf Then            ' 撞墙或者吃掉自己的身体，Game Over

            If PauseEnable Then
                Call Pause()
            End If

            Using g As GDIPlusDeviceHandle = _innerDevice.BackgroundGraphics
                SyncLock My.Resources.Restart
                    Dim l As Point = New Point((g.Width - My.Resources.Restart.Width) / 2, (g.Height - My.Resources.Restart.Height) / 2)
                    Dim button As New Button(My.Resources.Restart) With {.Location = l}

                    Call Me.Add(button)
                    Call button.Draw(g)
                End SyncLock

                GameOver = True

                If Not GameOverCallback Is Nothing Then
                    Call GameOverCallback()(Me)
                End If

                _innerDevice.BackgroundImage = g.ImageResource
            End Using
        End If
    End Sub

    Protected Overrides Sub __GraphicsDeviceResize()

    End Sub

    Public ReadOnly Property food As Food

    Public Property ScoreCallback As Action(Of Point)

    Private Sub AddFood()
        _food = New Food With {.Location = New Point(GraphicRegion.Width * RandomDouble(), GraphicRegion.Height * RandomDouble())}
        Call Me.Add(food)
        Score.Score += 1
    End Sub

    Dim sound As New libZPlay.ZPlay

    Public Overrides Function Init() As Boolean
        Call ControlMaps.DefaultMaps(Me.ControlsMap.ControlMaps)

        _snake.Location = Me.GraphicRegion.Center
        _snake.MoveSpeed = 105

        Dim score As New Score With {.Location = New Point(10, 10)}

        Call Me.Add(score)

        Me.Score = score

        Call _snake.init()
        Call Me.Add(_snake)
        Call AddFood()

        Try
            Call My.Resources.background.FlushStream(App.HOME & "/background.mp3")
        Catch ex As Exception

        End Try

        Call sound.OpenFile(App.HOME & "/background.mp3")
        Call sound.StartPlayback()

        Return True
    End Function

    Public Overrides Sub __reset()
        _snake.Location = Me.GraphicRegion.Center
        Call _snake.init()

        For Each x In (From o In Me Where TypeOf o Is Food Select o)
            Call Me.Remove(x)
        Next

        Call AddFood()

        Call Me.Remove((From x In Me Where TypeOf x Is Button Select x).FirstOrDefault)
        Call Score.Reset

        GameOver = False
    End Sub

    Public Overrides Sub __restart()

    End Sub
End Class
