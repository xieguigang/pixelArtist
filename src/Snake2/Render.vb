Public Class Render

    ' ---------- 颜色 ----------
    Friend Shared ReadOnly PlayerBodyColor As Color = Color.LimeGreen
    Friend Shared ReadOnly PlayerHeadColor As Color = Color.DarkGreen
    Friend Shared ReadOnly AIBodyColor As Color = Color.Orange
    Friend Shared ReadOnly AIHeadColor As Color = Color.DarkOrange
    Friend Shared ReadOnly RegularFoodColor As Color = Color.Gold
    Friend Shared ReadOnly MovingFoodColor As Color = Color.Cyan
    Friend Shared ReadOnly SuperFoodColor As Color = Color.Magenta
    Friend Shared ReadOnly ObstacleColor As Color = Color.DimGray
    Friend Shared ReadOnly BackgroundColor As Color = Color.Black
    Friend Shared ReadOnly GridColor As Color = Color.FromArgb(25, 25, 25)

    ' ---------- 相机 ----------
    Friend cameraX As Integer = 0
    Friend cameraY As Integer = 0

    ReadOnly canvas As Form
    ReadOnly game As Game

    Public ReadOnly Property ClientSize As Size
        Get
            Return canvas.ClientSize
        End Get
    End Property

    Sub New(canvas As Form, game As Game)
        Me.game = game
        Me.canvas = canvas
    End Sub

    Public Sub Draw(g As Graphics)
        ' 绘制网格
        Using pen As New Pen(Render.GridColor)
            For x As Integer = 0 To Game.ViewCols
                g.DrawLine(pen, x * Game.CellSize, 0, x * Game.CellSize, Game.ViewRows * Game.CellSize)
            Next
            For y As Integer = 0 To Game.ViewRows
                g.DrawLine(pen, 0, y * Game.CellSize, Game.ViewCols * Game.CellSize, y * Game.CellSize)
            Next
        End Using

        ' 绘制障碍物
        For Each obs In game.obstacles
            If IsVisible(obs) Then
                g.FillRectangle(New SolidBrush(ObstacleColor),
                    (obs.X - cameraX) * Game.CellSize, (obs.Y - cameraY) * Game.CellSize, Game.CellSize, Game.CellSize)
            End If
        Next

        ' 绘制食物
        For Each f In game.foods
            If IsVisible(f.Position) Then
                DrawFood(g, f)
            End If
        Next

        ' 绘制AI蛇
        For Each s In game.aiSnakes
            DrawSnake(g, s)
        Next

        ' 绘制玩家蛇
        DrawSnake(g, game.playerSnake)

        ' 绘制小地图
        DrawMiniMap(g)

        ' 绘制HUD
        DrawHUD(g)

        ' 绘制游戏结束/暂停画面
        If game.gameOver Then
            DrawGameOver(g)
        ElseIf game.paused Then
            DrawPaused(g)
        End If
    End Sub

    ' 判断格子是否在可视区域内
    Private Function IsVisible(p As Point) As Boolean
        Return p.X >= cameraX AndAlso p.X < cameraX + Game.ViewCols AndAlso
               p.Y >= cameraY AndAlso p.Y < cameraY + Game.ViewRows
    End Function

    ' ============================================================
    '  相机
    ' ============================================================
    Public Sub UpdateCamera()
        cameraX = game.playerSnake.Head.X - Game.ViewCols \ 2
        cameraY = game.playerSnake.Head.Y - Game.ViewRows \ 2

        If cameraX < 0 Then cameraX = 0
        If cameraY < 0 Then cameraY = 0
        If cameraX > Game.MapCols - Game.ViewCols Then cameraX = Game.MapCols - Game.ViewCols
        If cameraY > Game.MapRows - Game.ViewRows Then cameraY = Game.MapRows - Game.ViewRows
    End Sub

    ' 绘制食物
    Private Sub DrawFood(g As Graphics, f As Food)
        Dim x As Integer = (f.Position.X - cameraX) * Game.CellSize
        Dim y As Integer = (f.Position.Y - cameraY) * Game.CellSize

        Select Case f.Type
            Case FoodType.Regular
                ' 常规食物: 金色圆形
                g.FillEllipse(New SolidBrush(f.Color), x + 2, y + 2, Game.CellSize - 4, Game.CellSize - 4)
            Case FoodType.Moving
                ' 活动食物: 青色方块
                g.FillRectangle(New SolidBrush(f.Color), x + 3, y + 3, Game.CellSize - 6, Game.CellSize - 6)
            Case FoodType.Super
                ' 超级食物: 紫红色, 带脉冲效果和白色外圈
                Dim pulse As Integer = CInt(2 + 2 * Math.Sin(DateTime.Now.Millisecond / 100.0))
                g.FillEllipse(New SolidBrush(f.Color), x - pulse, y - pulse, Game.CellSize + pulse * 2, Game.CellSize + pulse * 2)
                g.DrawEllipse(Pens.White, x - pulse, y - pulse, Game.CellSize + pulse * 2, Game.CellSize + pulse * 2)
        End Select
    End Sub

    ' 绘制蛇
    Private Sub DrawSnake(g As Graphics, s As Snake)
        ' 从尾部向头部绘制, 头部最后绘制
        For i As Integer = s.Body.Count - 1 To 0 Step -1
            Dim seg = s.Body(i)
            If Not IsVisible(seg) Then Continue For

            Dim x As Integer = (seg.X - cameraX) * Game.CellSize
            Dim y As Integer = (seg.Y - cameraY) * Game.CellSize

            If i = 0 Then
                ' 蛇头
                g.FillRectangle(New SolidBrush(s.HeadColor), x, y, Game.CellSize, Game.CellSize)
                ' 眼睛
                g.FillEllipse(Brushes.White, x + 4, y + 4, 4, 4)
                g.FillEllipse(Brushes.White, x + Game.CellSize - 8, y + 4, 4, 4)
                g.FillEllipse(Brushes.Black, x + 5, y + 5, 2, 2)
                g.FillEllipse(Brushes.Black, x + Game.CellSize - 7, y + 5, 2, 2)
            Else
                ' 蛇身
                g.FillRectangle(New SolidBrush(s.BodyColor), x + 1, y + 1, Game.CellSize - 2, Game.CellSize - 2)
            End If
        Next
    End Sub

    ' 绘制小地图 (全局导航)
    Private Sub DrawMiniMap(g As Graphics)
        Dim mmX As Integer = Me.ClientSize.Width - Game.MiniMapSize - 10
        Dim mmY As Integer = 10
        Dim scaleX As Single = CSng(Game.MiniMapSize / Game.MapCols)
        Dim scaleY As Single = CSng(Game.MiniMapSize / Game.MapRows)

        ' 背景
        g.FillRectangle(New SolidBrush(Color.FromArgb(200, 0, 0, 0)), mmX, mmY, Game.MiniMapSize, Game.MiniMapSize)
        g.DrawRectangle(Pens.White, mmX, mmY, Game.MiniMapSize, Game.MiniMapSize)

        ' 标题
        Using font As New Font("Arial", 8, FontStyle.Bold)
            g.DrawString("Global Map", font, Brushes.White, mmX, mmY - 14)
        End Using

        ' 绘制障碍物
        For Each obs In game.obstacles
            g.FillRectangle(Brushes.DimGray, mmX + obs.X * scaleX, mmY + obs.Y * scaleY,
                            Math.Max(1, scaleX), Math.Max(1, scaleY))
        Next

        ' 绘制食物
        For Each f In game.foods
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
        For Each s In game.aiSnakes
            For Each seg In s.Body
                g.FillRectangle(Brushes.Orange, mmX + seg.X * scaleX, mmY + seg.Y * scaleY, 1, 1)
            Next
        Next

        ' 绘制玩家蛇 (绿色, 稍大)
        For Each seg In game.playerSnake.Body
            g.FillRectangle(Brushes.LimeGreen, mmX + seg.X * scaleX, mmY + seg.Y * scaleY, 2, 2)
        Next

        ' 绘制相机视口矩形
        g.DrawRectangle(Pens.White, mmX + cameraX * scaleX, mmY + cameraY * scaleY,
                        Game.ViewCols * scaleX, Game.ViewRows * scaleY)
    End Sub

    ' 绘制HUD (分数/长度/图例等)
    Private Sub DrawHUD(g As Graphics)
        Using font As New Font("Arial", 12, FontStyle.Bold),
              smallFont As New Font("Arial", 9)

            ' 分数信息 (左上)
            g.DrawString($"Score: {game.score}", font, Brushes.White, 10, 10)
            g.DrawString($"High Score: {game.highScore}", font, Brushes.Yellow, 10, 30)
            g.DrawString($"Length: {game.playerSnake.Body.Count}", font, Brushes.LimeGreen, 10, 50)
            g.DrawString($"AI Snakes: {game.aiSnakes.Count}", font, Brushes.Orange, 10, 70)

            ' 超级食物倒计时
            Dim superFood As Food = Nothing
            For Each f In game.foods
                If f.Type = FoodType.Super Then
                    superFood = f
                    Exit For
                End If
            Next
            If superFood IsNot Nothing Then
                Dim remaining As Integer = Game.SuperFoodDurationSec - CInt(Math.Floor((DateTime.Now - superFood.SpawnTime).TotalSeconds))
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

            Dim s2 = g.MeasureString($"Score: {game.score}", infoFont)
            g.DrawString($"Score: {game.score}", infoFont, Brushes.White, cx - s2.Width / 2, cy - 20)

            Dim s3 = g.MeasureString($"High Score: {game.highScore}", infoFont)
            g.DrawString($"High Score: {game.highScore}", infoFont, Brushes.Yellow, cx - s3.Width / 2, cy + 10)

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
