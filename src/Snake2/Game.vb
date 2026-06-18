Imports System.IO

Public Class Game

    ' ---------- 常量 ----------
    Public Const CellSize As Integer = 20              ' 每格像素大小
    Public Const MapCols As Integer = 200              ' 地图列数 (超大地图)
    Public Const MapRows As Integer = 200              ' 地图行数
    Public Const ViewCols As Integer = 40              ' 可视列数
    Public Const ViewRows As Integer = 30              ' 可视行数
    Public Const MiniMapSize As Integer = 150          ' 小地图尺寸
    Public Const MaxAISnakes As Integer = 50           ' AI蛇数量上限(性能保护)
    Public Const SuperFoodDurationSec As Integer = 60  ' 超级食物存在秒数
    Public Const SuperFoodSpawnIntervalTicks As Integer = 300 ' 超级食物生成间隔(tick)
    Public Const ObstacleCount As Integer = 300        ' 障碍物数量

    ' ---------- 游戏状态 ----------
    Friend playerSnake As Snake
    Friend aiSnakes As New List(Of Snake)
    Friend foods As New List(Of Food)
    Friend obstacles As New HashSet(Of Point)
    Friend score As Integer = 0
    Friend highScore As Integer = 0
    Friend gameOver As Boolean = False
    Friend paused As Boolean = False
    Friend rand As New Random()
    Friend superFoodTimer As Integer = 0

    Dim render As Render

    ' ============================================================
    '  游戏初始化
    ' ============================================================
    Public Sub InitGame(render As Render)
        Me.render = render

        ' 创建玩家蛇 (地图中央, 长度3, 向右移动)
        playerSnake = New Snake()
        playerSnake.IsPlayer = True
        playerSnake.BodyColor = Render.PlayerBodyColor
        playerSnake.HeadColor = Render.PlayerHeadColor
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

        Call LoadHighScore()
    End Sub

    Public Sub GameTick()
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
    End Sub

    ' ============================================================
    '  游戏结束 / 重启
    ' ============================================================
    Public Sub EndGame()
        gameOver = True
        If score > highScore Then
            highScore = score
            SaveHighScore()
        End If
    End Sub

    ' ============================================================
    '  最高分读写
    ' ============================================================
    Public Sub LoadHighScore()
        Try
            Dim path As String = Application.StartupPath & "/" & "snake_highscore.txt"
            If File.Exists(path) Then
                highScore = Integer.Parse(File.ReadAllText(path).Trim())
            End If
        Catch
            highScore = 0
        End Try
    End Sub

    Public Sub SaveHighScore()
        Try
            Dim path As String = Application.StartupPath & "/" & "snake_highscore.txt"
            File.WriteAllText(path, highScore.ToString())
        Catch
        End Try
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
            Dim x As Integer = render.cameraX + rand.Next(ViewCols)
            Dim y As Integer = render.cameraY + rand.Next(ViewRows)
            If x < 0 OrElse x >= MapCols OrElse y < 0 OrElse y >= MapRows Then Continue While
            Dim p As New Point(x, y)
            If IsCellFree(p) Then
                Dim f As New Food()
                f.Position = p
                f.Type = FoodType.Regular
                f.Color = Render.RegularFoodColor
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
        f.Color = Render.MovingFoodColor
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
        f.Color = Render.SuperFoodColor
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
    '  碰撞检测
    ' ============================================================

    ' 玩家蛇碰撞检测
    Public Sub CheckPlayerCollisions()
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
        newSnake.BodyColor = Render.AIBodyColor
        newSnake.HeadColor = Render.AIHeadColor
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
                f.Color = Render.RegularFoodColor
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

End Class
