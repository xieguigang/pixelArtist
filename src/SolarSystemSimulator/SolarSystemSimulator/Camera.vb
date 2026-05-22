' ============================================================
' Camera.vb - 相机/视图系统
' ============================================================
' 实现缩放、平移、多视图模式切换
' 支持从银河系尺度到行星卫星尺度的连续缩放
' ============================================================

Imports System.Drawing
Imports System.Windows.Forms

Namespace SolarSystemSimulator

    ''' <summary>
    ''' 视图模式
    ''' </summary>
    Public Enum ViewMode
        ''' <summary>银河系视图 —— 太阳绕银河系中心运动</summary>
        Galactic
        ''' <summary>太阳系全景 —— 显示所有行星</summary>
        SolarSystem
        ''' <summary>内太阳系 —— 水星到火星</summary>
        InnerSolarSystem
        ''' <summary>行星聚焦 —— 聚焦某颗行星及其卫星</summary>
        PlanetFocus
    End Enum

    ''' <summary>
    ''' 相机系统 —— 控制视图的缩放、平移和目标
    ''' </summary>
    Public Class Camera

        ' ==================== 视图参数 ====================

        ''' <summary>当前视图模式</summary>
        Public Property Mode As ViewMode = ViewMode.SolarSystem

        ''' <summary>视图中心 X（AU）</summary>
        Public Property CenterX As Double = 0.0

        ''' <summary>视图中心 Y（AU）</summary>
        Public Property CenterY As Double = 0.0

        ''' <summary>缩放级别（像素/AU）</summary>
        Public Property Zoom As Double = 15.0

        ''' <summary>最小缩放</summary>
        Public Property MinZoom As Double = 0.0001

        ''' <summary>最大缩放</summary>
        Public Property MaxZoom As Double = 500000.0

        ''' <summary>聚焦的行星（PlanetFocus 模式下使用）</summary>
        Public Property FocusPlanet As CelestialBody = Nothing

        ''' <summary>画布宽度</summary>
        Public Property CanvasWidth As Integer = 800

        ''' <summary>画布高度</summary>
        Public Property CanvasHeight As Integer = 600

        ''' <summary>视图倾斜角（弧度，0=俯视，π/2=侧视）</summary>
        Public Property TiltAngle As Double = 0.3

        ' ==================== 预设缩放级别 ====================

        ''' <summary>银河系视图缩放</summary>
        Public Shared ReadOnly GalacticZoom As Double = 0.00015

        ''' <summary>太阳系全景缩放</summary>
        Public Shared ReadOnly SolarSystemZoom As Double = 12.0

        ''' <summary>内太阳系缩放</summary>
        Public Shared ReadOnly InnerSolarSystemZoom As Double = 80.0

        ''' <summary>行星聚焦缩放</summary>
        Public Shared ReadOnly PlanetFocusZoom As Double = 200000.0

        ' ==================== 坐标变换 ====================

        ''' <summary>
        ''' 世界坐标（AU）→ 屏幕坐标（像素）
        ''' </summary>
        Public Function WorldToScreen(worldX As Double, worldY As Double) As PointF
            ' 相对于视图中心的偏移
            Dim dx As Double = worldX - CenterX
            Dim dy As Double = worldY - CenterY

            ' 应用倾斜（将 Z 轴投影到 Y 轴，产生伪3D效果）
            ' 这里简化处理，只用 X 和 Y
            Dim screenX As Single = CSng(CanvasWidth / 2.0 + dx * Zoom)
            Dim screenY As Single = CSng(CanvasHeight / 2.0 - dy * Zoom)  ' Y轴翻转

            Return New PointF(screenX, screenY)
        End Function

        ''' <summary>
        ''' 屏幕坐标（像素）→ 世界坐标（AU）
        ''' </summary>
        Public Function ScreenToWorld(screenX As Single, screenY As Single) As (X As Double, Y As Double)
            Dim worldX As Double = (screenX - CanvasWidth / 2.0) / Zoom + CenterX
            Dim worldY As Double = -(screenY - CanvasHeight / 2.0) / Zoom + CenterY
            Return (worldX, worldY)
        End Function

        ''' <summary>
        ''' 世界坐标中的距离（AU）→ 屏幕上的像素距离
        ''' </summary>
        Public Function WorldDistanceToPixels(distanceAU As Double) As Double
            Return distanceAU * Zoom
        End Function

        ''' <summary>
        ''' 判断一个世界坐标点是否在屏幕可见范围内
        ''' </summary>
        Public Function IsVisible(worldX As Double, worldY As Double, margin As Double) As Boolean
            Dim sp As PointF = WorldToScreen(worldX, worldY)
            Return sp.X >= -margin AndAlso sp.X <= CanvasWidth + margin AndAlso
                   sp.Y >= -margin AndAlso sp.Y <= CanvasHeight + margin
        End Function

        ' ==================== 视图切换 ====================

        ''' <summary>切换到银河系视图</summary>
        Public Sub SwitchToGalacticView()
            Mode = ViewMode.Galactic
            CenterX = 0
            CenterY = 0
            Zoom = GalacticZoom
        End Sub

        ''' <summary>切换到太阳系全景视图</summary>
        Public Sub SwitchToSolarSystemView()
            Mode = ViewMode.SolarSystem
            CenterX = 0
            CenterY = 0
            Zoom = SolarSystemZoom
        End Sub

        ''' <summary>切换到内太阳系视图</summary>
        Public Sub SwitchToInnerSolarSystemView()
            Mode = ViewMode.InnerSolarSystem
            CenterX = 0
            CenterY = 0
            Zoom = InnerSolarSystemZoom
        End Sub

        ''' <summary>切换到行星聚焦视图</summary>
        Public Sub SwitchToPlanetFocus(planet As CelestialBody)
            Mode = ViewMode.PlanetFocus
            FocusPlanet = planet
            Zoom = PlanetFocusZoom
        End Sub

        ''' <summary>更新行星聚焦视图的中心位置</summary>
        Public Sub UpdateFocusPosition()
            If Mode = ViewMode.PlanetFocus AndAlso FocusPlanet IsNot Nothing Then
                CenterX = FocusPlanet.Position.X
                CenterY = FocusPlanet.Position.Y
            End If
        End Sub

        ' ==================== 缩放和平移 ====================

        ''' <summary>以屏幕某点为中心缩放</summary>
        Public Sub ZoomAt(screenX As Single, screenY As Single, factor As Double)
            ' 记录缩放前鼠标指向的世界坐标
            Dim worldBefore = ScreenToWorld(screenX, screenY)

            ' 应用缩放
            Zoom *= factor
            Zoom = Math.Max(MinZoom, Math.Min(MaxZoom, Zoom))

            ' 调整中心使鼠标指向的世界坐标不变
            Dim worldAfter = ScreenToWorld(screenX, screenY)
            CenterX += worldBefore.X - worldAfter.X
            CenterY += worldBefore.Y - worldAfter.Y
        End Sub

        ''' <summary>平移视图</summary>
        Public Sub Pan(deltaScreenX As Single, deltaScreenY As Single)
            CenterX -= deltaScreenX / Zoom
            CenterY += deltaScreenY / Zoom
        End Sub

        ''' <summary>鼠标滚轮缩放</summary>
        Public Sub HandleMouseWheel(e As MouseEventArgs)
            Dim factor As Double = If(e.Delta > 0, 1.15, 1.0 / 1.15)
            ZoomAt(e.X, e.Y, factor)
        End Sub

        ' ==================== 辅助方法 ====================

        ''' <summary>获取当前视图的描述</summary>
        Public Function GetViewDescription() As String
            Dim desc As String = ""
            Select Case Mode
                Case ViewMode.Galactic
                    desc = "银河系视图"
                Case ViewMode.SolarSystem
                    desc = "太阳系全景"
                Case ViewMode.InnerSolarSystem
                    desc = "内太阳系"
                Case ViewMode.PlanetFocus
                    If FocusPlanet IsNot Nothing Then
                        desc = "聚焦: " & FocusPlanet.Name
                    Else
                        desc = "行星聚焦"
                    End If
            End Select
            desc &= String.Format("  缩放: {0:F1} px/AU", Zoom)
            Return desc
        End Function

        ''' <summary>获取视野范围（AU）</summary>
        Public Function GetViewRange() As (WidthAU As Double, HeightAU As Double)
            Dim wAU As Double = CanvasWidth / Zoom
            Dim hAU As Double = CanvasHeight / Zoom
            Return (wAU, hAU)
        End Function

    End Class

End Namespace
