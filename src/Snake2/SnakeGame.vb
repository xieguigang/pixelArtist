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

    ' ---------- 常量 ----------
    Private Const CellSize As Integer = 20              ' 每格像素大小
    Private Const MapCols As Integer = 200              ' 地图列数 (超大地图)
    Private Const MapRows As Integer = 200              ' 地图行数
    Private Const ViewCols As Integer = 40              ' 可视列数
    Private Const ViewRows As Integer = 30              ' 可视行数
    Private Const MiniMapSize As Integer = 150          ' 小地图尺寸
    Private Const MaxAISnakes As Integer = 50           ' AI蛇数量上限(性能保护)
    Private Const SuperFoodDurationSec As Integer = 60  ' 超级食物存在秒数
    Private Const SuperFoodSpawnIntervalTicks As Integer = 300 ' 超级食物生成间隔(tick)
    Private Const ObstacleCount As Integer = 300        ' 障碍物数量

    ' ---------- 颜色 ----------
    Private ReadOnly PlayerBodyColor As Color = Color.LimeGreen
    Private ReadOnly PlayerHeadColor As Color = Color.DarkGreen
    Private ReadOnly AIBodyColor As Color = Color.Orange
    Private ReadOnly AIHeadColor As Color = Color.DarkOrange
    Private ReadOnly RegularFoodColor As Color = Color.Gold
    Private ReadOnly MovingFoodColor As Color = Color.Cyan
    Private ReadOnly SuperFoodColor As Color = Color.Magenta
    Private ReadOnly ObstacleColor As Color = Color.DimGray
    Private ReadOnly BackgroundColor As Color = Color.Black
    Private ReadOnly GridColor As Color = Color.FromArgb(25, 25, 25)

    ' ---------- 游戏状态 ----------
    Private playerSnake As Snake
    Private aiSnakes As New List(Of Snake)
    Private foods As New List(Of Food)
    Private obstacles As New HashSet(Of Point)
    Private score As Integer = 0
    Private highScore As Integer = 0
    Private gameOver As Boolean = False
    Private paused As Boolean = False
    Private rand As New Random()
    Private superFoodTimer As Integer = 0

    ' ---------- 相机 ----------
    Private cameraX As Integer = 0
    Private cameraY As Integer = 0

    ' ---------- 计时器 ----------
    Private gameTimer As New Timer()

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
    '  游戏初始化
    ' ============================================================
    Private Sub InitGame()
        ' 创建玩家蛇 (地图中央, 长度3, 向右移动)
        playerSnake = New Snake()
        playerSnake.IsPlayer = True
        playerSnake.BodyColor = PlayerBodyColor
        playerSnake.HeadColor = PlayerHeadColor
        playerSnake.Direction = New Point(1, 0)
        playerSnake.PendingGrowth = 0
        playerSnake.Alive = True

        Dim cx As Integer = MapCols \ 2
        Dim cy As Integer = MapRows \ 2
        playerSnake.Body.Add(New Point(cx, cy))
        playerSnake.Body.Add(New Point(cx - 1, cy))
        playerSnake.Body.Add(New Point(cx - 2, cy))

        aiSnakes.Clear()
        foods.Clear()
        obstacles.Clear()
        score = 0
        gameOver = False
        paused = False
        superFoodTimer = 0

        ' 生成障碍物
        GenerateObstacles()

        ' 生成初始食物
        For i As Integer = 1 To 3
            SpawnRegularFood()
        Next
        For i As Integer = 1 To 5
            SpawnMovingFood()
        Next

        UpdateCamera()
    End Sub

    ' 生成障碍物 (避开玩家出生区域)
    Private Sub GenerateObstacles()
        Dim attempts As Integer = 0
        While obstacles.Count < ObstacleCount AndAlso attempts < ObstacleCount * 10
            attempts += 1
            Dim x As Integer = rand.Next(MapCols)
            Dim y As Integer = rand.Next(MapRows)
            Dim p As New Point(x, y)

            ' 不在玩家出生区域附近生成
            Dim cx As Integer = MapCols \ 2
            Dim cy As Integer = MapRows \ 2
            If Math.Abs(x - cx) < 15 AndAlso Math.Abs(y - cy) < 15 Then
                Continue While
            End If

            obstacles.Add(p)
        End While
    End Sub

    ' ============================================================
    '  食物生成
    ' ============================================================

    ' 生成常规食物 (在当前可视区域内)
    Private Sub SpawnRegularFood()
        Dim attempts As Integer = 0
        While attempts < 200
            attempts += 1
            Dim x As Integer = cameraX + rand.Next(ViewCols)
            Dim y As Integer = cameraY + rand.Next(ViewRows)
            If x < 0 OrElse x >= MapCols OrElse y < 0 OrElse y >= MapRows Then Continue While
            Dim p As New Point(x, y)
            If IsCellFree(p) Then
                Dim f As New Food()
                f.Position = p
                f.Type = FoodType.Regular
                f.Color = RegularFoodColor
                foods.Add(f)
                Return
            End If
        End While
    End Sub

    ' 生成活动食物 (全地图随机, 有不同速度)
    Private Sub SpawnMovingFood()
        Dim p As Point = FindFreeCell()
        If p.X < 0 Then Return
        Dim f As New Food()
        f.Position = p
        f.Type = FoodType.Moving
        f.Color = MovingFoodColor
        Dim dirs() As Point = {New Point(1, 0), New Point(-1, 0), New Point(0, 1), New Point(0, -1)}
        f.Velocity = dirs(rand.Next(4))
        f.Speed = rand.Next(3, 5)  ' 1=快(每tick移动), 2=慢(每2tick移动)
        f.MoveCounter = 0
        foods.Add(f)
    End Sub

    ' 生成超级食物 (全地图随机, 最多存在60秒)
    Private Sub SpawnSuperFood()
        Dim p As Point = FindFreeCell()
        If p.X < 0 Then Return
        Dim f As New Food()
        f.Position = p
        f.Type = FoodType.Super
        f.Color = SuperFoodColor
        f.SpawnTime = DateTime.Now
        foods.Add(f)
    End Sub

    ' 在全地图找一个空闲格子
    Private Function FindFreeCell() As Point
        Dim attempts As Integer = 0
        While attempts < 500
            attempts += 1
            Dim x As Integer = rand.Next(MapCols)
            Dim y As Integer = rand.Next(MapRows)
            Dim p As New Point(x, y)
            If IsCellFree(p) Then Return p
        End While
        Return New Point(-1, -1)
    End Function

    ' 检查格子是否空闲 (无障碍物/蛇身/食物)
    Private Function IsCellFree(p As Point) As Boolean
        If p.X < 0 OrElse p.X >= MapCols OrElse p.Y < 0 OrElse p.Y >= MapRows Then Return False
        If obstacles.Contains(p) Then Return False
        If playerSnake.ContainsBody(p) Then Return False
        For Each s In aiSnakes
            If s.ContainsBody(p) Then Return False
        Next
        For Each f In foods
            If f.Position = p Then Return False
        Next
        Return True
    End Function

    ' 统计指定类型食物数量
    Private Function CountFoodOfType(type As FoodType) As Integer
        Dim count As Integer = 0
        For Each f In foods
            If f.Type = type Then count += 1
        Next
        Return count
    End Function

    ' 是否存在超级食物
    Private Function HasSuperFood() As Boolean
        For Each f In foods
            If f.Type = FoodType.Super Then Return True
        Next
        Return False
    End Function

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
    '  碰撞检测
    ' ============================================================

    ' 玩家蛇碰撞检测
    Private Sub CheckPlayerCollisions()
        Dim head = playerSnake.Head

        ' 撞墙
        If head.X < 0 OrElse head.X >= MapCols OrElse head.Y < 0 OrElse head.Y >= MapRows Then
            EndGame()
            Return
        End If

        ' 撞障碍物
        If obstacles.Contains(head) Then
            EndGame()
            Return
        End If

        ' 撞自身
        For i As Integer = 1 To playerSnake.Body.Count - 1
            If playerSnake.Body(i) = head Then
                EndGame()
                Return
            End If
        Next

        ' 吃食物
        For i As Integer = foods.Count - 1 To 0 Step -1
            Dim f = foods(i)
            If f.Position = head Then
                Select Case f.Type
                    Case FoodType.Regular
                        playerSnake.Grow(1)
                        score += 1
                    Case FoodType.Moving
                        playerSnake.Grow(5)
                        score += 5
                    Case FoodType.Super
                        playerSnake.Grow(10)
                        score += 30
                End Select
                foods.RemoveAt(i)
            End If
        Next
    End Sub

    ' AI蛇碰撞检测
    Private Sub CheckAICollisions(snake As Snake)
        Dim head = snake.Head

        ' 撞墙 -> 转化为常规食物
        If head.X < 0 OrElse head.X >= MapCols OrElse head.Y < 0 OrElse head.Y >= MapRows Then
            ConvertAIToFood(snake)
            snake.Alive = False
            Return
        End If

        ' 撞障碍物 -> 转化为常规食物
        If obstacles.Contains(head) Then
            ConvertAIToFood(snake)
            snake.Alive = False
            Return
        End If

        ' 撞自身 -> 转化为常规食物
        For i As Integer = 1 To snake.Body.Count - 1
            If snake.Body(i) = head Then
                ConvertAIToFood(snake)
                snake.Alive = False
                Return
            End If
        Next

        ' 吃食物 (AI吃食物只增长长度, 不加分)
        For i As Integer = foods.Count - 1 To 0 Step -1
            Dim f = foods(i)
            If f.Position = head Then
                Select Case f.Type
                    Case FoodType.Regular
                        snake.Grow(1)
                    Case FoodType.Moving
                        snake.Grow(5)
                    Case FoodType.Super
                        snake.Grow(10)
                End Select
                foods.RemoveAt(i)
            End If
        Next
    End Sub

    ' 玩家与AI蛇碰撞检测
    Private Sub CheckPlayerAICollisions()
        ' 玩家蛇头碰到AI蛇 -> 玩家吃掉AI (不管长度, 增加对应长度, 不加分)
        Dim playerHead = playerSnake.Head
        For i As Integer = aiSnakes.Count - 1 To 0 Step -1
            Dim ai = aiSnakes(i)
            For j As Integer = 0 To ai.Body.Count - 1
                If ai.Body(j) = playerHead Then
                    playerSnake.Grow(ai.Body.Count)
                    aiSnakes.RemoveAt(i)
                    Exit For
                End If
            Next
        Next

        ' AI蛇头碰到玩家蛇身 -> 比较长度
        For i As Integer = aiSnakes.Count - 1 To 0 Step -1
            Dim ai = aiSnakes(i)
            Dim aiHead = ai.Head
            For j As Integer = 1 To playerSnake.Body.Count - 1
                If playerSnake.Body(j) = aiHead Then
                    If ai.Body.Count >= playerSnake.Body.Count Then
                        ' AI长于或等于玩家 -> 游戏结束
                        EndGame()
                        Return
                    Else
                        ' AI短于玩家 -> AI被消灭 (无法威胁玩家)
                        aiSnakes.RemoveAt(i)
                        Exit For
                    End If
                End If
            Next
        Next
    End Sub

    ' 活动食物与蛇身碰撞检测 (切割蛇身)
    Private Sub CheckMovingFoodCollisions()
        Dim i As Integer = 0
        While i < foods.Count
            Dim f = foods(i)
            If f.Type <> FoodType.Moving Then
                i += 1
                Continue While
            End If

            ' 活动食物碰到玩家蛇头 -> 玩家吃掉 (因为食物移动后可能正好在蛇头位置)
            If f.Position = playerSnake.Head Then
                playerSnake.Grow(5)
                score += 5
                foods.RemoveAt(i)
                Continue While
            End If

            ' 活动食物碰到AI蛇头 -> AI吃掉
            Dim eatenByAI As Boolean = False
            For k As Integer = aiSnakes.Count - 1 To 0 Step -1
                If aiSnakes(k).Head = f.Position Then
                    aiSnakes(k).Grow(5)
                    foods.RemoveAt(i)
                    eatenByAI = True
                    Exit For
                End If
            Next
            If eatenByAI Then Continue While

            ' 活动食物碰到玩家蛇身(非头部) -> 切割玩家蛇
            Dim cut As Boolean = False
            For j As Integer = 1 To playerSnake.Body.Count - 1
                If playerSnake.Body(j) = f.Position Then
                    Dim newSnake = CutSnake(playerSnake, j)
                    If newSnake IsNot Nothing AndAlso aiSnakes.Count < MaxAISnakes Then
                        aiSnakes.Add(newSnake)
                    End If
                    foods.RemoveAt(i)
                    cut = True
                    Exit For
                End If
            Next
            If cut Then Continue While

            ' 活动食物碰到AI蛇身(非头部) -> 切割AI蛇 (产生新的AI蛇)
            For k As Integer = aiSnakes.Count - 1 To 0 Step -1
                Dim ai = aiSnakes(k)
                For j As Integer = 1 To ai.Body.Count - 1
                    If ai.Body(j) = f.Position Then
                        Dim newSnake = CutSnake(ai, j)
                        If newSnake IsNot Nothing AndAlso aiSnakes.Count < MaxAISnakes Then
                            aiSnakes.Add(newSnake)
                        End If
                        foods.RemoveAt(i)
                        cut = True
                        Exit For
                    End If
                Next
                If cut Then Exit For
            Next

            If Not cut Then i += 1
        End While
    End Sub

    ' 切割蛇身: 从hitIndex处切断, 头部部分保留原控制者, 尾部部分变为AI蛇
    Private Function CutSnake(snake As Snake, hitIndex As Integer) As Snake
        ' hitIndex 是被活动食物碰到的蛇身段索引
        ' 头部部分: Body(0..hitIndex) - 保留原控制者
        ' 尾部部分: Body(hitIndex+1..end) - 变为AI蛇
        If hitIndex >= snake.Body.Count - 1 Then Return Nothing

        ' 计算尾部新蛇的方向 (沿着原蛇身方向继续)
        Dim tailDir As Point
        tailDir = New Point(snake.Body(hitIndex).X - snake.Body(hitIndex + 1).X,
                            snake.Body(hitIndex).Y - snake.Body(hitIndex + 1).Y)
        If tailDir.X = 0 AndAlso tailDir.Y = 0 Then
            Dim dirs() As Point = {New Point(1, 0), New Point(-1, 0), New Point(0, 1), New Point(0, -1)}
            tailDir = dirs(rand.Next(4))
        End If

        ' 提取尾部
        Dim tailPart As New List(Of Point)
        For idx As Integer = hitIndex + 1 To snake.Body.Count - 1
            tailPart.Add(snake.Body(idx))
        Next

        ' 从原蛇移除尾部
        While snake.Body.Count > hitIndex + 1
            snake.Body.RemoveAt(snake.Body.Count - 1)
        End While

        ' 创建新的AI蛇
        Dim newSnake As New Snake()
        newSnake.Body = tailPart
        newSnake.IsPlayer = False
        newSnake.BodyColor = AIBodyColor
        newSnake.HeadColor = AIHeadColor
        newSnake.Direction = tailDir
        newSnake.Alive = True

        Return newSnake
    End Function

    ' AI蛇撞障碍物后转化为常规食物
    Private Sub ConvertAIToFood(snake As Snake)
        Dim head = snake.Head
        If head.X >= 0 AndAlso head.X < MapCols AndAlso head.Y >= 0 AndAlso head.Y < MapRows Then
            If Not obstacles.Contains(head) AndAlso Not IsOnAnyFood(head) AndAlso Not IsOnAnySnake(head) Then
                Dim f As New Food()
                f.Position = head
                f.Type = FoodType.Regular
                f.Color = RegularFoodColor
                foods.Add(f)
            End If
        End If
    End Sub

    Private Function IsOnAnyFood(p As Point) As Boolean
        For Each f In foods
            If f.Position = p Then Return True
        Next
        Return False
    End Function

    Private Function IsOnAnySnake(p As Point) As Boolean
        If playerSnake.ContainsBody(p) Then Return True
        For Each s In aiSnakes
            If s.ContainsBody(p) Then Return True
        Next
        Return False
    End Function

    ' ============================================================
    '  AI蛇行为
    ' ============================================================
    Private Sub UpdateAISnake(snake As Snake)
        Dim possibleDirs As New List(Of Point) From {
            New Point(1, 0), New Point(-1, 0), New Point(0, 1), New Point(0, -1)
        }

        ' 不能反向
        Dim reverseDir As New Point(-snake.Direction.X, -snake.Direction.Y)
        possibleDirs.Remove(reverseDir)

        ' 随机打乱
        For i As Integer = possibleDirs.Count - 1 To 1 Step -1
            Dim j As Integer = rand.Next(i + 1)
            Dim temp = possibleDirs(i)
            possibleDirs(i) = possibleDirs(j)
            possibleDirs(j) = temp
        Next

        ' 优先保持当前方向
        possibleDirs.Insert(0, snake.Direction)

        ' 寻找有效方向
        Dim chosenDir As Point = snake.Direction
        For Each d In possibleDirs
            Dim nextPos As New Point(snake.Head.X + d.X, snake.Head.Y + d.Y)
            If IsValidAIPosition(nextPos, snake) Then
                chosenDir = d
                Exit For
            End If
        Next

        snake.Direction = chosenDir
        snake.Move()
    End Sub

    ' 检查AI蛇下一步位置是否有效
    Private Function IsValidAIPosition(pos As Point, snake As Snake) As Boolean
        ' 撞墙
        If pos.X < 0 OrElse pos.X >= MapCols OrElse pos.Y < 0 OrElse pos.Y >= MapRows Then
            Return False
        End If

        ' 撞障碍物
        If obstacles.Contains(pos) Then
            Return False
        End If

        ' 撞自身 (尾部会移走, 所以排除最后一段)
        For i As Integer = 0 To snake.Body.Count - 2
            If snake.Body(i) = pos Then
                Return False
            End If
        Next

        ' 撞其他AI蛇
        For Each other In aiSnakes
            If other IsNot snake Then
                For i As Integer = 0 To other.Body.Count - 1
                    If other.Body(i) = pos Then
                        Return False
                    End If
                Next
            End If
        Next

        ' 撞玩家蛇头 -> 总是避开 (碰到蛇头会被玩家吃掉)
        If playerSnake.Head = pos Then
            Return False
        End If

        ' 撞玩家蛇身 -> 若AI短于玩家则避开, 若长于则可以攻击
        If snake.Body.Count < playerSnake.Body.Count Then
            For i As Integer = 1 To playerSnake.Body.Count - 1
                If playerSnake.Body(i) = pos Then
                    Return False
                End If
            Next
        End If

        Return True
    End Function

    ' ============================================================
    '  活动食物移动
    ' ============================================================
    Private Sub UpdateMovingFoods()
        For Each f In foods
            If f.Type <> FoodType.Moving Then Continue For

            f.MoveCounter += 1
            If f.MoveCounter < f.Speed Then Continue For
            f.MoveCounter = 0

            ' 移动
            Dim newPos As New Point(f.Position.X + f.Velocity.X, f.Position.Y + f.Velocity.Y)

            ' 撞墙反弹
            If newPos.X < 0 Then
                newPos.X = 0
                f.Velocity = New Point(-f.Velocity.X, f.Velocity.Y)
            ElseIf newPos.X >= MapCols Then
                newPos.X = MapCols - 1
                f.Velocity = New Point(-f.Velocity.X, f.Velocity.Y)
            End If
            If newPos.Y < 0 Then
                newPos.Y = 0
                f.Velocity = New Point(f.Velocity.X, -f.Velocity.Y)
            ElseIf newPos.Y >= MapRows Then
                newPos.Y = MapRows - 1
                f.Velocity = New Point(f.Velocity.X, -f.Velocity.Y)
            End If

            ' 撞障碍物反弹
            If obstacles.Contains(newPos) Then
                f.Velocity = New Point(-f.Velocity.X, -f.Velocity.Y)
                newPos = New Point(f.Position.X + f.Velocity.X, f.Position.Y + f.Velocity.Y)
                If obstacles.Contains(newPos) OrElse newPos.X < 0 OrElse newPos.X >= MapCols OrElse newPos.Y < 0 OrElse newPos.Y >= MapRows Then
                    newPos = f.Position
                    Dim dirs() As Point = {New Point(1, 0), New Point(-1, 0), New Point(0, 1), New Point(0, -1)}
                    f.Velocity = dirs(rand.Next(4))
                End If
            End If

            f.Position = newPos

            ' 随机改变方向
            If rand.NextDouble() < 0.05 Then
                Dim dirs() As Point = {New Point(1, 0), New Point(-1, 0), New Point(0, 1), New Point(0, -1)}
                f.Velocity = dirs(rand.Next(4))
            End If
        Next
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

    ' 判断格子是否在可视区域内
    Private Function IsVisible(p As Point) As Boolean
        Return p.X >= cameraX AndAlso p.X < cameraX + ViewCols AndAlso
               p.Y >= cameraY AndAlso p.Y < cameraY + ViewRows
    End Function

    ' 绘制食物
    Private Sub DrawFood(g As Graphics, f As Food)
        Dim x As Integer = (f.Position.X - cameraX) * CellSize
        Dim y As Integer = (f.Position.Y - cameraY) * CellSize

        Select Case f.Type
            Case FoodType.Regular
                ' 常规食物: 金色圆形
                g.FillEllipse(New SolidBrush(f.Color), x + 2, y + 2, CellSize - 4, CellSize - 4)
            Case FoodType.Moving
                ' 活动食物: 青色方块
                g.FillRectangle(New SolidBrush(f.Color), x + 3, y + 3, CellSize - 6, CellSize - 6)
            Case FoodType.Super
                ' 超级食物: 紫红色, 带脉冲效果和白色外圈
                Dim pulse As Integer = CInt(2 + 2 * Math.Sin(DateTime.Now.Millisecond / 100.0))
                g.FillEllipse(New SolidBrush(f.Color), x - pulse, y - pulse, CellSize + pulse * 2, CellSize + pulse * 2)
                g.DrawEllipse(Pens.White, x - pulse, y - pulse, CellSize + pulse * 2, CellSize + pulse * 2)
        End Select
    End Sub

    ' 绘制蛇
    Private Sub DrawSnake(g As Graphics, s As Snake)
        ' 从尾部向头部绘制, 头部最后绘制
        For i As Integer = s.Body.Count - 1 To 0 Step -1
            Dim seg = s.Body(i)
            If Not IsVisible(seg) Then Continue For

            Dim x As Integer = (seg.X - cameraX) * CellSize
            Dim y As Integer = (seg.Y - cameraY) * CellSize

            If i = 0 Then
                ' 蛇头
                g.FillRectangle(New SolidBrush(s.HeadColor), x, y, CellSize, CellSize)
                ' 眼睛
                g.FillEllipse(Brushes.White, x + 4, y + 4, 4, 4)
                g.FillEllipse(Brushes.White, x + CellSize - 8, y + 4, 4, 4)
                g.FillEllipse(Brushes.Black, x + 5, y + 5, 2, 2)
                g.FillEllipse(Brushes.Black, x + CellSize - 7, y + 5, 2, 2)
            Else
                ' 蛇身
                g.FillRectangle(New SolidBrush(s.BodyColor), x + 1, y + 1, CellSize - 2, CellSize - 2)
            End If
        Next
    End Sub

    ' 绘制小地图 (全局导航)
    Private Sub DrawMiniMap(g As Graphics)
        Dim mmX As Integer = Me.ClientSize.Width - MiniMapSize - 10
        Dim mmY As Integer = 10
        Dim scaleX As Single = CSng(MiniMapSize / MapCols)
        Dim scaleY As Single = CSng(MiniMapSize / MapRows)

        ' 背景
        g.FillRectangle(New SolidBrush(Color.FromArgb(200, 0, 0, 0)), mmX, mmY, MiniMapSize, MiniMapSize)
        g.DrawRectangle(Pens.White, mmX, mmY, MiniMapSize, MiniMapSize)

        ' 标题
        Using font As New Font("Arial", 8, FontStyle.Bold)
            g.DrawString("Global Map", font, Brushes.White, mmX, mmY - 14)
        End Using

        ' 绘制障碍物
        For Each obs In obstacles
            g.FillRectangle(Brushes.DimGray, mmX + obs.X * scaleX, mmY + obs.Y * scaleY,
                            Math.Max(1, scaleX), Math.Max(1, scaleY))
        Next

        ' 绘制食物
        For Each f In foods
            Dim fx As Single = mmX + f.Position.X * scaleX
            Dim fy As Single = mmY + f.Position.Y * scaleY
            Select Case f.Type
                Case FoodType.Super
                    ' 超级食物高亮显示 (大圆点 + 白色外圈)
                    g.FillEllipse(Brushes.Magenta, fx - 2, fy - 2, 6, 6)
                    g.DrawEllipse(Pens.White, fx - 3, fy - 3, 8, 8)
                Case FoodType.Moving
                    g.FillRectangle(Brushes.Cyan, fx, fy, 2, 2)
                Case FoodType.Regular
                    g.FillRectangle(Brushes.Gold, fx, fy, 2, 2)
            End Select
        Next

        ' 绘制AI蛇
        For Each s In aiSnakes
            For Each seg In s.Body
                g.FillRectangle(Brushes.Orange, mmX + seg.X * scaleX, mmY + seg.Y * scaleY, 1, 1)
            Next
        Next

        ' 绘制玩家蛇 (绿色, 稍大)
        For Each seg In playerSnake.Body
            g.FillRectangle(Brushes.LimeGreen, mmX + seg.X * scaleX, mmY + seg.Y * scaleY, 2, 2)
        Next

        ' 绘制相机视口矩形
        g.DrawRectangle(Pens.White, mmX + cameraX * scaleX, mmY + cameraY * scaleY,
                        ViewCols * scaleX, ViewRows * scaleY)
    End Sub

    ' 绘制HUD (分数/长度/图例等)
    Private Sub DrawHUD(g As Graphics)
        Using font As New Font("Arial", 12, FontStyle.Bold),
              smallFont As New Font("Arial", 9)

            ' 分数信息 (左上)
            g.DrawString($"Score: {score}", font, Brushes.White, 10, 10)
            g.DrawString($"High Score: {highScore}", font, Brushes.Yellow, 10, 30)
            g.DrawString($"Length: {playerSnake.Body.Count}", font, Brushes.LimeGreen, 10, 50)
            g.DrawString($"AI Snakes: {aiSnakes.Count}", font, Brushes.Orange, 10, 70)

            ' 超级食物倒计时
            Dim superFood As Food = Nothing
            For Each f In foods
                If f.Type = FoodType.Super Then
                    superFood = f
                    Exit For
                End If
            Next
            If superFood IsNot Nothing Then
                Dim remaining As Integer = SuperFoodDurationSec - CInt(Math.Floor((DateTime.Now - superFood.SpawnTime).TotalSeconds))
                If remaining < 0 Then remaining = 0
                g.DrawString($"Super Food: {remaining}s", font, Brushes.Magenta, 10, 90)
            End If

            ' 图例 (左下)
            Dim legendY As Integer = Me.ClientSize.Height - 130
            g.DrawString("Legend:", smallFont, Brushes.White, 10, legendY)
            g.FillRectangle(New SolidBrush(PlayerBodyColor), 10, legendY + 18, 12, 12)
            g.DrawString("Player Snake", smallFont, Brushes.White, 28, legendY + 18)
            g.FillRectangle(New SolidBrush(AIBodyColor), 10, legendY + 34, 12, 12)
            g.DrawString("AI Snake", smallFont, Brushes.White, 28, legendY + 34)
            g.FillEllipse(New SolidBrush(RegularFoodColor), 10, legendY + 50, 12, 12)
            g.DrawString("Regular Food (+1 len, +1 score)", smallFont, Brushes.White, 28, legendY + 50)
            g.FillRectangle(New SolidBrush(MovingFoodColor), 10, legendY + 66, 12, 12)
            g.DrawString("Moving Food (+5 len, +5 score)", smallFont, Brushes.White, 28, legendY + 66)
            g.FillEllipse(New SolidBrush(SuperFoodColor), 10, legendY + 82, 12, 12)
            g.DrawString("Super Food (+10 len, +30 score)", smallFont, Brushes.White, 28, legendY + 82)
            g.FillRectangle(New SolidBrush(ObstacleColor), 10, legendY + 98, 12, 12)
            g.DrawString("Obstacle", smallFont, Brushes.White, 28, legendY + 98)
        End Using

        ' 操作提示 (右下)
        Using font As New Font("Arial", 9)
            Dim hint As String = "Arrow/WASD: Move   P: Pause   R: Restart"
            Dim sz = g.MeasureString(hint, font)
            g.DrawString(hint, font, Brushes.LightGray,
                         Me.ClientSize.Width - sz.Width - 10,
                         Me.ClientSize.Height - sz.Height - 10)
        End Using
    End Sub

    ' 绘制游戏结束画面
    Private Sub DrawGameOver(g As Graphics)
        ' 半透明遮罩
        g.FillRectangle(New SolidBrush(Color.FromArgb(180, 0, 0, 0)),
                        0, 0, Me.ClientSize.Width, Me.ClientSize.Height)

        Using titleFont As New Font("Arial", 28, FontStyle.Bold),
              infoFont As New Font("Arial", 16, FontStyle.Bold),
              hintFont As New Font("Arial", 12)

            Dim cx As Single = Me.ClientSize.Width / 2
            Dim cy As Single = Me.ClientSize.Height / 2

            Dim s1 = g.MeasureString("GAME OVER", titleFont)
            g.DrawString("GAME OVER", titleFont, Brushes.Red, cx - s1.Width / 2, cy - 80)

            Dim s2 = g.MeasureString($"Score: {score}", infoFont)
            g.DrawString($"Score: {score}", infoFont, Brushes.White, cx - s2.Width / 2, cy - 20)

            Dim s3 = g.MeasureString($"High Score: {highScore}", infoFont)
            g.DrawString($"High Score: {highScore}", infoFont, Brushes.Yellow, cx - s3.Width / 2, cy + 10)

            Dim s4 = g.MeasureString("Press R or Space to Restart", hintFont)
            g.DrawString("Press R or Space to Restart", hintFont, Brushes.LightGray,
                         cx - s4.Width / 2, cy + 50)
        End Using
    End Sub

    ' 绘制暂停画面
    Private Sub DrawPaused(g As Graphics)
        g.FillRectangle(New SolidBrush(Color.FromArgb(150, 0, 0, 0)),
                        0, 0, Me.ClientSize.Width, Me.ClientSize.Height)
        Using font As New Font("Arial", 24, FontStyle.Bold)
            Dim s = g.MeasureString("PAUSED", font)
            g.DrawString("PAUSED", font, Brushes.White,
                         (Me.ClientSize.Width - s.Width) / 2,
                         (Me.ClientSize.Height - s.Height) / 2)
        End Using
    End Sub

End Class

