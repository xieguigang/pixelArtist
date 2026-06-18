Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Windows.Controls

' ============================================================
'  贪吃蛇像素游戏 - VB.NET WinForms 实现
'  - 超大地图 (200x200)，窗口仅显示一部分，相机跟随玩家
'  - 三类食物：常规 / 活动 / 超级
'  - 障碍物、AI 蛇、蛇身切割、全局导航小地图
' ============================================================

' ===== 主窗体 =====
Public Class GameForm : Inherits Form

    ' ---------- 计时器 ----------
    Dim WithEvents gameTimer As New Timer() With {
        .Interval = 100   ' 100ms = 10 FPS
    }
    Dim game As Game
    Dim render As Render

    ' ============================================================
    '  构造函数
    ' ============================================================
    Public Sub New()
        Call InitializeComponent()
    End Sub

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.DoubleBuffered = True
        Me.ClientSize = New Size(Game.ViewCols * Game.CellSize, Game.ViewRows * Game.CellSize)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "贪吃蛇像素游戏"
        Me.BackColor = Render.BackgroundColor
        Me.KeyPreview = True
    End Sub

    Private Sub GameForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        game = New Game()
        render = New Render(Me, game)

        Call game.InitGame(render)
        Call render.UpdateCamera()
        Call gameTimer.Start()
    End Sub

    ' ============================================================
    '  游戏主循环 (Timer Tick)
    ' ============================================================
    Private Sub GameTimer_Tick(sender As Object, e As EventArgs) Handles gameTimer.Tick
        If game.gameOver OrElse game.paused Then
            Me.Invalidate()
        Else
            ' 1. 玩家蛇移动
            game.playerSnake.Move()
            game.CheckPlayerCollisions()

            If game.gameOver Then
                Me.Invalidate()
            Else
                game.GameTick()
            End If
        End If



        ' 9. 更新相机
        render.UpdateCamera()

        ' 10. 渲染
        Me.Invalidate()
    End Sub

    Private Sub RestartGame()
        game.InitGame(render)
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        game.SaveHighScore()
        MyBase.OnFormClosing(e)
    End Sub

    ' ============================================================
    '  键盘输入
    ' ============================================================
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If game.gameOver Then
            If keyData = Keys.R Or keyData = Keys.Space Then
                RestartGame()
            End If
            Return MyBase.ProcessCmdKey(msg, keyData)
        End If

        Select Case keyData
            Case Keys.Up, Keys.W
                If game.playerSnake.Direction.Y <> 1 Then
                    game.playerSnake.Direction = New Point(0, -1)
                End If
            Case Keys.Down, Keys.S
                If game.playerSnake.Direction.Y <> -1 Then
                    game.playerSnake.Direction = New Point(0, 1)
                End If
            Case Keys.Left, Keys.A
                If game.playerSnake.Direction.X <> 1 Then
                    game.playerSnake.Direction = New Point(-1, 0)
                End If
            Case Keys.Right, Keys.D
                If game.playerSnake.Direction.X <> -1 Then
                    game.playerSnake.Direction = New Point(1, 0)
                End If
            Case Keys.P
                game.paused = Not game.paused
        End Select

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ' ============================================================
    '  渲染
    ' ============================================================
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.None
        ' 清屏
        g.Clear(Render.BackgroundColor)
        render.Draw(g)
    End Sub
End Class

