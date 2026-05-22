' ============================================================
' Renderer.vb - GDI+ 渲染引擎
' ============================================================
' 使用 System.Drawing (GDI+) 绘制太阳系所有天体和视觉效果
' ============================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

Namespace SolarSystemSimulator

    ''' <summary>
    ''' 太阳系渲染器
    ''' </summary>
    Public Class SolarSystemRenderer

        ' ==================== 配置 ====================

        Public Property ShowOrbits As Boolean = True
        Public Property ShowTrails As Boolean = True
        Public Property ShowLabels As Boolean = True
        Public Property ShowBackgroundStars As Boolean = True
        Public Property ShowGrid As Boolean = False
        Public Property ShowInfoPanel As Boolean = True
        Public Property ShowScaleBar As Boolean = True
        Public Property ShowSaturnRings As Boolean = True
        Public Property ShowCometTails As Boolean = True

        ' ==================== 内部状态 ====================

        Private _bgStars As List(Of StarPoint)
        Private _rng As New Random(42)
        Private _fontSmall As Font
        Private _fontMedium As Font
        Private _fontLarge As Font
        Private _fontTitle As Font

        ' ==================== 构造函数 ====================

        Public Sub New()
            _fontSmall = New Font("Microsoft YaHei", 8, FontStyle.Regular)
            _fontMedium = New Font("Microsoft YaHei", 10, FontStyle.Regular)
            _fontLarge = New Font("Microsoft YaHei", 12, FontStyle.Bold)
            _fontTitle = New Font("Microsoft YaHei", 14, FontStyle.Bold)
            GenerateBackgroundStars(800)
        End Sub

        ' ==================== 主渲染方法 ====================

        ''' <summary>渲染整个场景</summary>
        Public Sub Render(g As Graphics, sim As SolarSystemSimulation, cam As Camera)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit
            g.CompositingQuality = CompositingQuality.HighQuality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic

            ' 1. 背景
            DrawBackground(g, cam)

            ' 2. 根据视图模式渲染
            If cam.Mode = ViewMode.Galactic Then
                DrawGalacticView(g, sim, cam)
            Else
                DrawSolarSystemView(g, sim, cam)
            End If

            ' 3. UI 覆盖层
            If ShowInfoPanel Then DrawInfoPanel(g, sim, cam)
            If ShowScaleBar Then DrawScaleBar(g, cam)
        End Sub

        ' ==================== 背景 ====================

        Private Sub DrawBackground(g As Graphics, cam As Camera)
            ' 深空渐变
            Using bgBrush As New LinearGradientBrush(
                New Rectangle(0, 0, cam.CanvasWidth, cam.CanvasHeight),
                Color.FromArgb(5, 5, 20), Color.FromArgb(2, 2, 12),
                LinearGradientMode.Vertical)
                g.FillRectangle(bgBrush, 0, 0, cam.CanvasWidth, cam.CanvasHeight)
            End Using

            ' 背景星空
            If ShowBackgroundStars AndAlso _bgStars IsNot Nothing Then
                For Each star In _bgStars
                    Dim parallax As Double = 0.02
                    Dim sx As Single = CSng(star.X * cam.CanvasWidth - cam.CenterX * parallax * 0.001) Mod cam.CanvasWidth
                    Dim sy As Single = CSng(star.Y * cam.CanvasHeight - cam.CenterY * parallax * 0.001) Mod cam.CanvasHeight
                    If sx < 0 Then sx += cam.CanvasWidth
                    If sy < 0 Then sy += cam.CanvasHeight

                    Dim alpha As Integer = CInt(star.Brightness * 255)
                    Using brush As New SolidBrush(Color.FromArgb(alpha, alpha, alpha))
                        g.FillEllipse(brush, sx - star.Size / 2, sy - star.Size / 2, star.Size, star.Size)
                    End Using
                Next
            End If
        End Sub

        Private Sub GenerateBackgroundStars(count As Integer)
            _bgStars = New List(Of StarPoint)(count)
            For i As Integer = 0 To count - 1
                _bgStars.Add(New StarPoint() With {
                    .X = _rng.NextDouble(), .Y = _rng.NextDouble(),
                    .Brightness = 0.2 + _rng.NextDouble() * 0.8,
                    .Size = CSng(0.5 + _rng.NextDouble() * 1.5)
                })
            Next
        End Sub

        ' ==================== 银河系视图 ====================

        Private Sub DrawGalacticView(g As Graphics, sim As SolarSystemSimulation, cam As Camera)
            Dim gcScreen As PointF = cam.WorldToScreen(0, 0)

            ' 银河系中心光晕
            DrawGlow(g, gcScreen, 40, Color.FromArgb(60, 50, 20), Color.FromArgb(0, 0, 0, 0))
            Using centerBrush As New SolidBrush(Color.FromArgb(255, 200, 100))
                g.FillEllipse(centerBrush, gcScreen.X - 6, gcScreen.Y - 6, 12, 12)
            End Using

            ' 太阳银河系轨道
            Dim orbitR As Double = cam.WorldDistanceToPixels(OrbitalMechanics.SUN_GALACTIC_RADIUS_LY)
            If orbitR > 5 AndAlso orbitR < 100000 Then
                Using orbitPen As New Pen(Color.FromArgb(40, 40, 80), 1)
                    orbitPen.DashStyle = DashStyle.Dash
                    g.DrawEllipse(orbitPen,
                        gcScreen.X - CSng(orbitR), gcScreen.Y - CSng(orbitR),
                        CSng(orbitR * 2), CSng(orbitR * 2))
                End Using
            End If

            ' 太阳位置
            Dim sunScr As PointF = cam.WorldToScreen(sim.SunGalacticX, sim.SunGalacticY)
            DrawGlow(g, sunScr, 15, Color.FromArgb(100, 255, 200, 50), Color.FromArgb(0, 0, 0, 0))
            Using sunBrush As New SolidBrush(Color.FromArgb(255, 255, 100))
                g.FillEllipse(sunBrush, sunScr.X - 3, sunScr.Y - 3, 6, 6)
            End Using

            ' 标签
            Using lbl As New SolidBrush(Color.White)
                g.DrawString("银河系中心", _fontSmall, lbl, gcScreen.X + 12, gcScreen.Y - 6)
                g.DrawString("☉ 太阳", _fontSmall, lbl, sunScr.X + 8, sunScr.Y - 6)
            End Using

            ' 信息
            Using info As New SolidBrush(Color.FromArgb(200, 200, 200))
                g.DrawString(String.Format(
                    "太阳距银河系中心: {0:N0} 光年" & vbCrLf &
                    "银河系公转周期: ~2.25亿年" & vbCrLf &
                    "公转速度: ~220 km/s",
                    OrbitalMechanics.SUN_GALACTIC_RADIUS_LY), _fontMedium, info, 10, 10)
            End Using
        End Sub

        ' ==================== 太阳系视图 ====================

        Private Sub DrawSolarSystemView(g As Graphics, sim As SolarSystemSimulation, cam As Camera)
            If ShowGrid Then DrawGrid(g, cam)

            ' 轨道线
            If ShowOrbits Then
                For Each planet In sim.Planets
                    If planet.ShowOrbit AndAlso planet.OrbitPath IsNot Nothing Then
                        DrawOrbitPath(g, planet.OrbitPath, planet.Color, cam, 0.3F)
                    End If
                Next
                For Each ce In sim.Comets
                    If ce.Body.OrbitPath IsNot Nothing Then
                        DrawOrbitPath(g, ce.Body.OrbitPath, ce.Body.Color, cam, 0.2F)
                    End If
                Next
            End If

            ' 运动轨迹
            If ShowTrails Then
                For Each planet In sim.Planets
                    DrawTrail(g, planet, cam)
                Next
                For Each ce In sim.Comets
                    DrawTrail(g, ce.Body, cam)
                Next
            End If

            ' 太阳
            DrawSun(g, sim.Sun, cam)

            ' 行星
            For Each planet In sim.Planets
                DrawPlanet(g, planet, cam)
            Next

            ' 小行星
            For Each ae In sim.Asteroids
                DrawSmallBody(g, ae.Body, cam, 1.0F)
            Next

            ' 彗星
            For Each ce In sim.Comets
                DrawComet(g, ce.Body, sim.Sun, cam)
            Next

            ' 卫星（仅在行星聚焦模式或高缩放时显示）
            If cam.Mode = ViewMode.PlanetFocus OrElse cam.Zoom > 5000 Then
                For Each planet In sim.Planets
                    If cam.IsVisible(planet.Position.X, planet.Position.Y, 500) Then
                        ' 卫星轨道
                        If ShowOrbits Then
                            For Each moon In planet.Moons
                                If moon.OrbitPath IsNot Nothing Then
                                    DrawMoonOrbitPath(g, moon, planet, cam)
                                End If
                            Next
                        End If
                        ' 卫星本体
                        For Each moon In planet.Moons
                            DrawMoon(g, moon, planet, cam)
                        Next
                    End If
                Next
            End If

            ' 标签
            If ShowLabels Then
                For Each planet In sim.Planets
                    If planet.ShowLabel Then
                        DrawLabel(g, planet.Name, planet.Position.X, planet.Position.Y, planet.Color, cam, _fontSmall)
                    End If
                Next
                For Each ce In sim.Comets
                    If ce.Body.ShowLabel Then
                        DrawLabel(g, ce.Body.Name, ce.Body.Position.X, ce.Body.Position.Y, ce.Body.Color, cam, _fontSmall)
                    End If
                Next
            End If
        End Sub

        ' ==================== 天体绘制 ====================

        ''' <summary>绘制太阳（带光晕）</summary>
        Private Sub DrawSun(g As Graphics, sun As CelestialBody, cam As Camera)
            Dim sp As PointF = cam.WorldToScreen(sun.Position.X, sun.Position.Y)
            Dim baseSize As Single = GetDisplaySize(sun, cam)

            ' 多层光晕
            DrawGlow(g, sp, baseSize * 5, Color.FromArgb(12, 255, 200, 50), Color.FromArgb(0, 0, 0, 0))
            DrawGlow(g, sp, baseSize * 3, Color.FromArgb(30, 255, 200, 50), Color.FromArgb(0, 0, 0, 0))
            DrawGlow(g, sp, baseSize * 1.8, Color.FromArgb(60, 255, 220, 80), Color.FromArgb(0, 0, 0, 0))

            ' 太阳本体
            DrawGlow(g, sp, baseSize, Color.FromArgb(255, 255, 240, 100), Color.FromArgb(255, 255, 180, 30))

            If ShowLabels Then
                DrawLabel(g, sun.Name, sun.Position.X, sun.Position.Y, sun.Color, cam, _fontMedium)
            End If
        End Sub

        ''' <summary>绘制行星</summary>
        Private Sub DrawPlanet(g As Graphics, planet As CelestialBody, cam As Camera)
            Dim sp As PointF = cam.WorldToScreen(planet.Position.X, planet.Position.Y)
            If Not IsOnScreen(sp, 100, cam) Then Return

            Dim size As Single = GetDisplaySize(planet, cam)

            ' 行星本体
            Using bodyBrush As New SolidBrush(planet.Color)
                g.FillEllipse(bodyBrush, sp.X - size, sp.Y - size, size * 2, size * 2)
            End Using

            ' 高光
            If size > 3 Then
                DrawGlow(g, New PointF(sp.X - size * 0.3F, sp.Y - size * 0.3F),
                         size * 0.6F, Color.FromArgb(80, 255, 255, 255), Color.FromArgb(0, 0, 0, 0))
            End If

            ' 土星光环
            If ShowSaturnRings AndAlso planet.NameEn = "Saturn" Then
                DrawSaturnRings(g, sp, size)
            End If
        End Sub

        ''' <summary>绘制土星光环</summary>
        Private Sub DrawSaturnRings(g As Graphics, center As PointF, planetSize As Single)
            Dim ringInner As Single = planetSize * 1.4F
            Dim ringOuter As Single = planetSize * 2.3F
            Dim ringWidth As Single = ringOuter - ringInner

            ' 外环
            Using ringPen As New Pen(Color.FromArgb(100, 210, 190, 140), ringWidth * 0.6F)
                g.DrawEllipse(ringPen,
                    center.X - ringOuter, center.Y - ringOuter * 0.3F,
                    ringOuter * 2, ringOuter * 0.6F)
            End Using
            ' 内环
            Using ringPen2 As New Pen(Color.FromArgb(70, 190, 170, 120), ringWidth * 0.4F)
                g.DrawEllipse(ringPen2,
                    center.X - ringInner * 1.1F, center.Y - ringInner * 1.1F * 0.3F,
                    ringInner * 1.1F * 2, ringInner * 1.1F * 0.6F)
            End Using
        End Sub

        ''' <summary>绘制卫星</summary>
        Private Sub DrawMoon(g As Graphics, moon As CelestialBody, parent As CelestialBody, cam As Camera)
            ' 卫星绝对位置 = 行星位置 + 卫星相对位置
            Dim absX As Double = parent.Position.X + moon.Position.X
            Dim absY As Double = parent.Position.Y + moon.Position.Y
            Dim sp As PointF = cam.WorldToScreen(absX, absY)
            If Not IsOnScreen(sp, 50, cam) Then Return

            Dim size As Single = Math.Max(GetDisplaySize(moon, cam), 2)

            Using bodyBrush As New SolidBrush(moon.Color)
                g.FillEllipse(bodyBrush, sp.X - size, sp.Y - size, size * 2, size * 2)
            End Using

            If ShowLabels AndAlso moon.ShowLabel Then
                DrawLabel(g, moon.Name, absX, absY, moon.Color, cam, _fontSmall)
            End If
        End Sub

        ''' <summary>绘制卫星轨道（相对于父天体偏移）</summary>
        Private Sub DrawMoonOrbitPath(g As Graphics, moon As CelestialBody, parent As CelestialBody, cam As Camera)
            If moon.OrbitPath Is Nothing OrElse moon.OrbitPath.Count < 2 Then Return

            Dim parentScr As PointF = cam.WorldToScreen(parent.Position.X, parent.Position.Y)
            Dim scale As Single = CSng(cam.Zoom)

            Using orbitPen As New Pen(Color.FromArgb(40, moon.Color), 0.5F)
                orbitPen.DashStyle = DashStyle.Dot
                Dim points(moon.OrbitPath.Count - 1) As PointF
                For i As Integer = 0 To moon.OrbitPath.Count - 1
                    points(i) = New PointF(
                        parentScr.X + moon.OrbitPath(i).X * scale,
                        parentScr.Y - moon.OrbitPath(i).Y * scale)
                Next
                g.DrawLines(orbitPen, points)
            End Using
        End Sub

        ''' <summary>绘制彗星（带尾巴）</summary>
        Private Sub DrawComet(g As Graphics, comet As CelestialBody, sun As CelestialBody, cam As Camera)
            Dim sp As PointF = cam.WorldToScreen(comet.Position.X, comet.Position.Y)
            If Not IsOnScreen(sp, 200, cam) Then Return

            Dim size As Single = Math.Max(GetDisplaySize(comet, cam), 2)

            ' 彗发
            DrawGlow(g, sp, size * 4, Color.FromArgb(50, comet.Color), Color.FromArgb(0, 0, 0, 0))

            ' 核心
            Using coreBrush As New SolidBrush(Color.White)
                g.FillEllipse(coreBrush, sp.X - size, sp.Y - size, size * 2, size * 2)
            End Using

            ' 彗尾
            If ShowCometTails Then
                DrawCometTail(g, comet, sun, sp, cam)
            End If
        End Sub

        ''' <summary>绘制彗尾（离子尾 + 尘埃尾）</summary>
        Private Sub DrawCometTail(g As Graphics, comet As CelestialBody, sun As CelestialBody, sp As PointF, cam As Camera)
            Dim dx As Double = comet.Position.X - sun.Position.X
            Dim dy As Double = comet.Position.Y - sun.Position.Y
            Dim dist As Double = Math.Sqrt(dx * dx + dy * dy)
            If dist < 0.001 Then Return

            Dim nx As Double = dx / dist
            Dim ny As Double = dy / dist

            ' 尾巴长度：距太阳越近越长
            Dim tailLen As Single = CSng(Math.Min(250, 80.0 / (dist * 0.05 + 0.01)))
            tailLen = Math.Max(tailLen, 20)

            ' 离子尾（直线，蓝色）
            Dim ionEnd As New PointF(CSng(sp.X + nx * tailLen), CSng(sp.Y - ny * tailLen))
            Using ionPen As New Pen(Color.FromArgb(80, 100, 180, 255), 2)
                ionPen.EndCap = LineCap.Round
                g.DrawLine(ionPen, sp, ionEnd)
            End Using
            Using ionPen2 As New Pen(Color.FromArgb(40, 100, 180, 255), 4)
                ionPen2.EndCap = LineCap.Round
                g.DrawLine(ionPen2, sp, ionEnd)
            End Using

            ' 尘埃尾（弯曲，黄白色）
            Dim dustAngle As Double = 0.3
            Dim dustLen As Single = tailLen * 0.7F
            Dim dustEnd As New PointF(
                CSng(sp.X + (nx * Math.Cos(dustAngle) - ny * Math.Sin(dustAngle)) * dustLen),
                CSng(sp.Y - (ny * Math.Cos(dustAngle) + nx * Math.Sin(dustAngle)) * dustLen))
            Using dustPen As New Pen(Color.FromArgb(50, 200, 180, 150), 3)
                dustPen.EndCap = LineCap.Round
                g.DrawLine(dustPen, sp, dustEnd)
            End Using
        End Sub

        ''' <summary>绘制小天体（小行星等）</summary>
        Private Sub DrawSmallBody(g As Graphics, body As CelestialBody, cam As Camera, minSize As Single)
            Dim sp As PointF = cam.WorldToScreen(body.Position.X, body.Position.Y)
            If Not IsOnScreen(sp, 10, cam) Then Return

            Dim size As Single = Math.Max(GetDisplaySize(body, cam), minSize)
            Using bodyBrush As New SolidBrush(body.Color)
                g.FillEllipse(bodyBrush, sp.X - size, sp.Y - size, size * 2, size * 2)
            End Using
        End Sub

        ' ==================== 轨道和轨迹 ====================

        ''' <summary>绘制轨道路径</summary>
        Private Sub DrawOrbitPath(g As Graphics, path As List(Of PointF), color As Color, cam As Camera, alpha As Single)
            If path Is Nothing OrElse path.Count < 2 Then Return

            Dim screenPts(path.Count - 1) As PointF
            For i As Integer = 0 To path.Count - 1
                screenPts(i) = cam.WorldToScreen(path(i).X, path(i).Y)
            Next

            Using orbitPen As New Pen(Color.FromArgb(CInt(alpha * 255), color), 1)
                orbitPen.DashStyle = DashStyle.Dash
                g.DrawLines(orbitPen, screenPts)
            End Using
        End Sub

        ''' <summary>绘制运动轨迹（渐变透明度）</summary>
        Private Sub DrawTrail(g As Graphics, body As CelestialBody, cam As Camera)
            If body.Trail Is Nothing OrElse body.Trail.Count < 2 Then Return

            Dim count As Integer = body.Trail.Count
            Dim skip As Integer = Math.Max(1, count \ 300)

            For i As Integer = skip To count - 1
                Dim alpha As Single = CSng(i / count) * 0.6F
                Dim p1 As PointF = cam.WorldToScreen(body.Trail(i - skip).X, body.Trail(i - skip).Y)
                Dim p2 As PointF = cam.WorldToScreen(body.Trail(i).X, body.Trail(i).Y)

                If Not IsOnScreen(p1, 50, cam) AndAlso Not IsOnScreen(p2, 50, cam) Then Continue For

                Using trailPen As New Pen(Color.FromArgb(CInt(alpha * 255), body.Color), 1.5F)
                    g.DrawLine(trailPen, p1, p2)
                End Using
            Next
        End Sub

        ' ==================== 标签 ====================

        Private Sub DrawLabel(g As Graphics, text As String, worldX As Double, worldY As Double,
                              color As Color, cam As Camera, font As Font)
            Dim sp As PointF = cam.WorldToScreen(worldX, worldY)
            If Not IsOnScreen(sp, 100, cam) Then Return

            Dim textSize As SizeF = g.MeasureString(text, font)
            Dim lx As Single = sp.X + 8
            Dim ly As Single = sp.Y - 6

            Using bgBrush As New SolidBrush(Color.FromArgb(140, 10, 10, 30))
                g.FillRectangle(bgBrush, lx - 2, ly - 1, textSize.Width + 4, textSize.Height + 2)
            End Using
            Using textBrush As New SolidBrush(color)
                g.DrawString(text, font, textBrush, lx, ly)
            End Using
        End Sub

        ' ==================== UI 覆盖层 ====================

        Private Sub DrawInfoPanel(g As Graphics, sim As SolarSystemSimulation, cam As Camera)
            Dim px As Integer = 10, py As Integer = 10
            Dim pw As Integer = 300, ph As Integer = 140

            Using panelBrush As New SolidBrush(Color.FromArgb(180, 5, 5, 20))
                g.FillRectangle(panelBrush, px, py, pw, ph)
            End Using
            Using borderPen As New Pen(Color.FromArgb(80, 100, 150), 1)
                g.DrawRectangle(borderPen, px, py, pw, ph)
            End Using

            Dim y As Integer = py + 8
            Using titleBrush As New SolidBrush(Color.FromArgb(200, 220, 255))
                g.DrawString("☀ 太阳系模拟器", _fontTitle, titleBrush, px + 10, y)
            End Using
            y += 26

            Using infoBrush As New SolidBrush(Color.FromArgb(200, 200, 220))
                g.DrawString(String.Format("日期: {0}", sim.GetCurrentDateString()), _fontMedium, infoBrush, px + 10, y) : y += 18
                g.DrawString(String.Format("经过: {0:F2} 年  |  速度: {1:F0}x", sim.CurrentTime, sim.TimeScale), _fontMedium, infoBrush, px + 10, y) : y += 18
                g.DrawString(String.Format("视图: {0}", cam.GetViewDescription()), _fontSmall, infoBrush, px + 10, y) : y += 16
                g.DrawString(String.Format("彗星: {0}  小行星: {1}", sim.Comets.Count, sim.Asteroids.Count), _fontSmall, infoBrush, px + 10, y)
            End Using
        End Sub

        Private Sub DrawScaleBar(g As Graphics, cam As Camera)
            Dim bx As Integer = cam.CanvasWidth - 220
            Dim by As Integer = cam.CanvasHeight - 30
            Dim bw As Single = 120

            Dim distAU As Double = bw / cam.Zoom
            Dim distStr As String
            If distAU >= 1.0 Then
                distStr = String.Format("{0:F1} AU", distAU)
            ElseIf distAU >= 0.001 Then
                distStr = String.Format("{0:F0} 万km", distAU * 149597870.7 / 10000)
            Else
                distStr = String.Format("{0:F0} km", distAU * 149597870.7)
            End If

            Using barPen As New Pen(Color.FromArgb(180, 180, 200), 2)
                g.DrawLine(barPen, bx, by, bx + bw, by)
                g.DrawLine(barPen, bx, by - 4, bx, by + 4)
                g.DrawLine(barPen, bx + bw, by - 4, bx + bw, by + 4)
            End Using
            Using textBrush As New SolidBrush(Color.FromArgb(180, 180, 200))
                g.DrawString(distStr, _fontSmall, textBrush, bx + bw / 2 - 25, by - 18)
            End Using
        End Sub

        Private Sub DrawGrid(g As Graphics, cam As Camera)
            Dim range = cam.GetViewRange()
            Dim gridStep As Double = 1.0
            If range.WidthAU > 100 Then gridStep = 10
            If range.WidthAU > 1000 Then gridStep = 100
            If range.WidthAU < 1 Then gridStep = 0.1
            If range.WidthAU < 0.01 Then gridStep = 0.001

            Dim startX As Double = Math.Floor((cam.CenterX - range.WidthAU / 2) / gridStep) * gridStep
            Dim startY As Double = Math.Floor((cam.CenterY - range.HeightAU / 2) / gridStep) * gridStep
            Dim endX As Double = cam.CenterX + range.WidthAU / 2
            Dim endY As Double = cam.CenterY + range.HeightAU / 2

            Using gridPen As New Pen(Color.FromArgb(20, 40, 60), 1)
                Dim x As Double = startX
                While x <= endX
                    Dim s1 As PointF = cam.WorldToScreen(x, startY)
                    Dim s2 As PointF = cam.WorldToScreen(x, endY)
                    g.DrawLine(gridPen, s1, s2)
                    x += gridStep
                End While
                Dim y As Double = startY
                While y <= endY
                    Dim s1 As PointF = cam.WorldToScreen(startX, y)
                    Dim s2 As PointF = cam.WorldToScreen(endX, y)
                    g.DrawLine(gridPen, s1, s2)
                    y += gridStep
                End While
            End Using
        End Sub

        ' ==================== 辅助方法 ====================

        ''' <summary>绘制径向光晕（使用 PathGradientBrush）</summary>
        Private Sub DrawGlow(g As Graphics, center As PointF, radius As Single, centerColor As Color, edgeColor As Color)
            If radius < 1 Then Return
            Try
                Dim rect As New RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2)
                Dim path As New GraphicsPath()
                path.AddEllipse(rect)
                Using pgb As New PathGradientBrush(path)
                    pgb.CenterPoint = center
                    pgb.CenterColor = centerColor
                    pgb.SurroundColors = New Color() {edgeColor}
                    g.FillEllipse(pgb, rect)
                End Using
            Catch
                ' 如果半径太小则忽略
            End Try
        End Sub

        ''' <summary>计算天体显示大小</summary>
        Private Function GetDisplaySize(body As CelestialBody, cam As Camera) As Single
            Dim baseSize As Single = 4.0F * CSng(body.DisplayScale)

            If cam.Zoom > 100000 Then
                Dim radiusAU As Double = body.RadiusKm / 149597870.7
                baseSize = CSng(Math.Max(3, radiusAU * cam.Zoom * body.DisplayScale))
            ElseIf cam.Zoom > 100 Then
                baseSize = CSng(3 + Math.Log10(cam.Zoom / 100) * 3) * CSng(body.DisplayScale)
            End If

            Return Math.Max(2, Math.Min(baseSize, 50))
        End Function

        ''' <summary>判断屏幕坐标是否在画布范围内</summary>
        Private Function IsOnScreen(sp As PointF, margin As Single, cam As Camera) As Boolean
            Return sp.X >= -margin AndAlso sp.X <= cam.CanvasWidth + margin AndAlso
                   sp.Y >= -margin AndAlso sp.Y <= cam.CanvasHeight + margin
        End Function

    End Class

    ''' <summary>背景星星</summary>
    Public Class StarPoint
        Public Property X As Double
        Public Property Y As Double
        Public Property Brightness As Double
        Public Property Size As Single
    End Class

End Namespace
