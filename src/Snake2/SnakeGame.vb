Imports System.Drawing.Drawing2D
Imports System.IO

' ============================================================
'  贪吃蛇像素游戏 - VB.NET WinForms 实现
'  - 超大地图 (200x200)，窗口仅显示一部分，相机跟随玩家
'  - 三类食物：常规 / 活动 / 超级
'  - 障碍物、AI 蛇、蛇身切割、全局导航小地图
' ============================================================

' ===== 主窗体 =====
Public Class GameForm : Inherits Form

    ' ---------- 计时器 ----------
    Dim gameTimer As New Timer()
    Dim game As Game
    Dim render As Render

    ' ============================================================
    '  构造函数
    ' ============================================================
    Public Sub New()
        Me.DoubleBuffered = True
        Me.ClientSize = New Size(ViewCols * CellSize, ViewRows * CellSize)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "贪吃蛇像素游戏 - VB.NET WinForms"
        Me.BackColor = BackgroundColor
        Me.KeyPreview = True

        AddHandler gameTimer.Tick, AddressOf GameTimer_Tick
        gameTimer.Interval = 100  ' 100ms = 10 FPS

        LoadHighScore()
        InitGame()
        gameTimer.Start()
    End Sub

    ' ============================================================
    '  游戏主循环 (Timer Tick)
    ' ============================================================
    Private Sub GameTimer_Tick(sender As Object, e As EventArgs)
        If gameOver OrElse paused Then
            Me.Invalidate()
            Return
        End If

        ' 1. 玩家蛇移动
        playerSnake.Move()
        CheckPlayerCollisions()
        If gameOver Then
            Me.Invalidate()
            Return
        End If

        ' 2. AI蛇移动
        For i As Integer = aiSnakes.Count - 1 To 0 Step -1
            Dim s = aiSnakes(i)
            If s.Alive Then
                UpdateAISnake(s)
                CheckAICollisions(s)
            End If
        Next
        aiSnakes.RemoveAll(Function(s) Not s.Alive)

        ' 3. 活动食物移动
        UpdateMovingFoods()

        ' 4. 活动食物碰撞检测 (切割蛇身)
        CheckMovingFoodCollisions()

        ' 5. 移除过期超级食物
        foods.RemoveAll(Function(f) f.IsExpired)

        ' 6. 玩家与AI蛇碰撞检测
        CheckPlayerAICollisions()

        ' 7. 补充食物
        Dim regularCount = CountFoodOfType(FoodType.Regular)
        While regularCount < 3
            Dim before = foods.Count
            SpawnRegularFood()
            If foods.Count = before Then Exit While
            regularCount = CountFoodOfType(FoodType.Regular)
        End While

        Dim movingCount = CountFoodOfType(FoodType.Moving)

        Const maxMoveFood As Integer = 300

        While movingCount < maxMoveFood
            Dim before = foods.Count
            SpawnMovingFood()
            If foods.Count = before Then Exit While
            movingCount = CountFoodOfType(FoodType.Moving)
        End While

        ' 8. 超级食物生成
        superFoodTimer += 1
        If superFoodTimer >= SuperFoodSpawnIntervalTicks AndAlso Not HasSuperFood() Then
            SpawnSuperFood()
            superFoodTimer = 0
        End If

        ' 9. 更新相机
        UpdateCamera()

        ' 10. 渲染
        Me.Invalidate()
    End Sub

    ' ============================================================
    '  相机
    ' ============================================================
    Private Sub UpdateCamera()
        cameraX = playerSnake.Head.X - ViewCols \ 2
        cameraY = playerSnake.Head.Y - ViewRows \ 2

        If cameraX < 0 Then cameraX = 0
        If cameraY < 0 Then cameraY = 0
        If cameraX > MapCols - ViewCols Then cameraX = MapCols - ViewCols
        If cameraY > MapRows - ViewRows Then cameraY = MapRows - ViewRows
    End Sub

    ' ============================================================
    '  游戏结束 / 重启
    ' ============================================================
    Private Sub EndGame()
        gameOver = True
        If score > highScore Then
            highScore = score
            SaveHighScore()
        End If
    End Sub

    Private Sub RestartGame()
        InitGame()
    End Sub

    ' ============================================================
    '  最高分读写
    ' ============================================================
    Private Sub LoadHighScore()
        Try
            Dim path As String = Application.StartupPath & "/" & "snake_highscore.txt"
            If File.Exists(path) Then
                highScore = Integer.Parse(File.ReadAllText(path).Trim())
            End If
        Catch
            highScore = 0
        End Try
    End Sub

    Private Sub SaveHighScore()
        Try
            Dim path As String = Application.StartupPath & "/" & "snake_highscore.txt"
            File.WriteAllText(path, highScore.ToString())
        Catch
        End Try
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        SaveHighScore()
        MyBase.OnFormClosing(e)
    End Sub

    ' ============================================================
    '  键盘输入
    ' ============================================================
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If gameOver Then
            If keyData = Keys.R Or keyData = Keys.Space Then
                RestartGame()
            End If
            Return MyBase.ProcessCmdKey(msg, keyData)
        End If

        Select Case keyData
            Case Keys.Up, Keys.W
                If playerSnake.Direction.Y <> 1 Then
                    playerSnake.Direction = New Point(0, -1)
                End If
            Case Keys.Down, Keys.S
                If playerSnake.Direction.Y <> -1 Then
                    playerSnake.Direction = New Point(0, 1)
                End If
            Case Keys.Left, Keys.A
                If playerSnake.Direction.X <> 1 Then
                    playerSnake.Direction = New Point(-1, 0)
                End If
            Case Keys.Right, Keys.D
                If playerSnake.Direction.X <> -1 Then
                    playerSnake.Direction = New Point(1, 0)
                End If
            Case Keys.P
                paused = Not paused
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
        g.Clear(BackgroundColor)

        ' 绘制网格
        Using pen As New Pen(GridColor)
            For x As Integer = 0 To ViewCols
                g.DrawLine(pen, x * CellSize, 0, x * CellSize, ViewRows * CellSize)
            Next
            For y As Integer = 0 To ViewRows
                g.DrawLine(pen, 0, y * CellSize, ViewCols * CellSize, y * CellSize)
            Next
        End Using

        ' 绘制障碍物
        For Each obs In obstacles
            If IsVisible(obs) Then
                g.FillRectangle(New SolidBrush(ObstacleColor),
                    (obs.X - cameraX) * CellSize, (obs.Y - cameraY) * CellSize, CellSize, CellSize)
            End If
        Next

        ' 绘制食物
        For Each f In foods
            If IsVisible(f.Position) Then
                DrawFood(g, f)
            End If
        Next

        ' 绘制AI蛇
        For Each s In aiSnakes
            DrawSnake(g, s)
        Next

        ' 绘制玩家蛇
        DrawSnake(g, playerSnake)

        ' 绘制小地图
        DrawMiniMap(g)

        ' 绘制HUD
        DrawHUD(g)

        ' 绘制游戏结束/暂停画面
        If gameOver Then
            DrawGameOver(g)
        ElseIf paused Then
            DrawPaused(g)
        End If
    End Sub



End Class

