' ============================================================
' MainForm.vb - 主窗体
' ============================================================
' 太阳系模拟器的主界面，包含：
'   - 左侧控制面板（视图切换、时间控制、显示选项、天体列表）
'   - 右侧全屏画布（GDI+ 渲染）
'   - 底部状态栏
' ============================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports SolarSystemSimulator.SolarSystemSimulator



Public Class MainForm
    Inherits Form

    ' ==================== UI 控件 ====================

    Private WithEvents canvas As PictureBox
    Private pnlControl As Panel
    Private grpView As GroupBox
    Private btnGalactic As Button
    Private btnSolarSystem As Button
    Private btnInnerSystem As Button
    Private btnPlanetFocus As Button
    Private cmbPlanet As ComboBox

    Private grpTime As GroupBox
    Private btnPause As Button
    Private btnSlower As Button
    Private btnFaster As Button
    Private btnReset As Button
    Private lblSpeed As Label
    Private lblDate As Label
    Private lblElapsed As Label
    Private trkSpeed As TrackBar

    Private grpDisplay As GroupBox
    Private chkOrbits As CheckBox
    Private chkTrails As CheckBox
    Private chkLabels As CheckBox
    Private chkGrid As CheckBox
    Private chkSaturnRings As CheckBox
    Private chkCometTails As CheckBox

    Private grpComets As GroupBox
    Private btnAddComet As Button
    Private btnAddAsteroid As Button
    Private btnClearComets As Button
    Private lblCometCount As Label

    Private grpInfo As GroupBox
    Private txtInfo As TextBox

    Private statusStrip As StatusStrip
    Private toolStripStatusLabel1 As ToolStripStatusLabel
    Private toolStripStatusLabel2 As ToolStripStatusLabel

    ' ==================== 模拟引擎 ====================

    Private _sim As SolarSystemSimulation
    Private _camera As Camera
    Private _renderer As SolarSystemRenderer
    Private _animTimer As Timer
    Private _isPaused As Boolean = False
    Private _isDragging As Boolean = False
    Private _lastMousePos As Point

    ' ==================== 构造函数 ====================

    Public Sub New()
        InitializeComponent()
        InitializeSimulation()
    End Sub

    ' ==================== UI 初始化 ====================

    Private Sub InitializeComponent()
        Me.Text = "☀ 太阳系模拟器 - Solar System Simulator"
        Me.Size = New Size(1400, 900)
        Me.MinimumSize = New Size(1000, 700)
        Me.BackColor = Color.FromArgb(10, 10, 30)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Font = New Font("Microsoft YaHei", 9)

        ' 画布
        canvas = New PictureBox()
        canvas.Dock = DockStyle.Fill
        canvas.BackColor = Color.Black
        ' canvas.DoubleBuffered = True

        ' 控制面板
        pnlControl = New Panel()
        pnlControl.Width = 260
        pnlControl.Dock = DockStyle.Left
        pnlControl.BackColor = Color.FromArgb(20, 20, 45)
        pnlControl.Padding = New Padding(8)

        ' ---- 视图切换 ----
        grpView = New GroupBox()
        grpView.Text = "🔭 视图模式"
        grpView.ForeColor = Color.FromArgb(180, 200, 255)
        grpView.Dock = DockStyle.Top
        grpView.Height = 170
        grpView.Padding = New Padding(6)

        btnGalactic = New Button() With {.Text = "🌌 银河系视图", .Dock = DockStyle.Top, .Height = 30, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 40, 70), .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 3)}
        btnSolarSystem = New Button() With {.Text = "☀ 太阳系全景", .Dock = DockStyle.Top, .Height = 30, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(50, 50, 80), .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 3)}
        btnInnerSystem = New Button() With {.Text = "🪐 内太阳系", .Dock = DockStyle.Top, .Height = 30, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 40, 70), .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 3)}
        btnPlanetFocus = New Button() With {.Text = "🔍 行星聚焦", .Dock = DockStyle.Top, .Height = 30, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 40, 70), .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 3)}
        btnPlanetFocus.Visible = False

        cmbPlanet = New ComboBox() With {.Dock = DockStyle.Top, .Height = 26, .DropDownStyle = ComboBoxStyle.DropDownList, .BackColor = Color.FromArgb(30, 30, 60), .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 3)}
        cmbPlanet.Items.AddRange(New Object() {"水星", "金星", "地球", "火星", "木星", "土星", "天王星", "海王星", "冥王星"})
        cmbPlanet.SelectedIndex = 2  ' 默认地球

        grpView.Controls.AddRange(New Control() {cmbPlanet, btnPlanetFocus, btnInnerSystem, btnSolarSystem, btnGalactic})

        ' ---- 时间控制 ----
        grpTime = New GroupBox()
        grpTime.Text = "⏱ 时间控制"
        grpTime.ForeColor = Color.FromArgb(180, 200, 255)
        grpTime.Dock = DockStyle.Top
        grpTime.Height = 180
        grpTime.Padding = New Padding(6)
        grpTime.Margin = New Padding(0, 6, 0, 0)

        lblDate = New Label() With {.Text = "日期: 2000-01-01", .Dock = DockStyle.Top, .Height = 20, .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 2)}
        lblElapsed = New Label() With {.Text = "经过: 0.00 年", .Dock = DockStyle.Top, .Height = 20, .ForeColor = Color.FromArgb(180, 180, 200), .Margin = New Padding(0, 0, 0, 2)}
        lblSpeed = New Label() With {.Text = "速度: 1.0x (1年/秒)", .Dock = DockStyle.Top, .Height = 20, .ForeColor = Color.FromArgb(180, 180, 200), .Margin = New Padding(0, 0, 0, 4)}

        trkSpeed = New TrackBar() With {.Dock = DockStyle.Top, .Height = 35, .Minimum = -20, .Maximum = 60, .Value = 0, .TickFrequency = 10, .Margin = New Padding(0, 0, 0, 4)}
        AddHandler trkSpeed.Scroll, AddressOf trkSpeed_Scroll

        Dim timeBtnPanel As New Panel() With {.Dock = DockStyle.Top, .Height = 32, .Margin = New Padding(0, 0, 0, 3)}
        btnSlower = New Button() With {.Text = "⏪", .Location = New Point(0, 0), .Size = New Size(55, 28), .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 40, 70), .ForeColor = Color.White}
        btnPause = New Button() With {.Text = "⏸ 暂停", .Location = New Point(60, 0), .Size = New Size(70, 28), .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(60, 60, 90), .ForeColor = Color.White}
        btnFaster = New Button() With {.Text = "⏩", .Location = New Point(135, 0), .Size = New Size(55, 28), .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 40, 70), .ForeColor = Color.White}
        btnReset = New Button() With {.Text = "⏮ 重置", .Location = New Point(195, 0), .Size = New Size(55, 28), .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 40, 70), .ForeColor = Color.White}
        timeBtnPanel.Controls.AddRange(New Control() {btnSlower, btnPause, btnFaster, btnReset})

        grpTime.Controls.AddRange(New Control() {timeBtnPanel, trkSpeed, lblSpeed, lblElapsed, lblDate})

        ' ---- 显示选项 ----
        grpDisplay = New GroupBox()
        grpDisplay.Text = "🎨 显示选项"
        grpDisplay.ForeColor = Color.FromArgb(180, 200, 255)
        grpDisplay.Dock = DockStyle.Top
        grpDisplay.Height = 155
        grpDisplay.Padding = New Padding(6)
        grpDisplay.Margin = New Padding(0, 6, 0, 0)

        chkOrbits = New CheckBox() With {.Text = "显示轨道线", .Checked = True, .Dock = DockStyle.Top, .Height = 22, .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 1)}
        chkTrails = New CheckBox() With {.Text = "显示运动轨迹", .Checked = True, .Dock = DockStyle.Top, .Height = 22, .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 1)}
        chkLabels = New CheckBox() With {.Text = "显示名称标签", .Checked = True, .Dock = DockStyle.Top, .Height = 22, .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 1)}
        chkGrid = New CheckBox() With {.Text = "显示网格", .Checked = False, .Dock = DockStyle.Top, .Height = 22, .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 1)}
        chkSaturnRings = New CheckBox() With {.Text = "土星光环", .Checked = True, .Dock = DockStyle.Top, .Height = 22, .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 1)}
        chkCometTails = New CheckBox() With {.Text = "彗星尾巴", .Checked = True, .Dock = DockStyle.Top, .Height = 22, .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 1)}

        grpDisplay.Controls.AddRange(New Control() {chkCometTails, chkSaturnRings, chkGrid, chkLabels, chkTrails, chkOrbits})

        ' ---- 彗星/小行星 ----
        grpComets = New GroupBox()
        grpComets.Text = "☄ 彗星与小行星"
        grpComets.ForeColor = Color.FromArgb(180, 200, 255)
        grpComets.Dock = DockStyle.Top
        grpComets.Height = 110
        grpComets.Padding = New Padding(6)
        grpComets.Margin = New Padding(0, 6, 0, 0)

        btnAddComet = New Button() With {.Text = "🌠 生成彗星", .Dock = DockStyle.Top, .Height = 28, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 40, 70), .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 3)}
        btnAddAsteroid = New Button() With {.Text = "🪨 生成小行星", .Dock = DockStyle.Top, .Height = 28, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 40, 70), .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 3)}
        btnClearComets = New Button() With {.Text = "🗑 清除全部", .Dock = DockStyle.Top, .Height = 28, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(60, 30, 30), .ForeColor = Color.White, .Margin = New Padding(0, 0, 0, 3)}
        lblCometCount = New Label() With {.Text = "彗星: 0  小行星: 0", .Dock = DockStyle.Top, .Height = 20, .ForeColor = Color.FromArgb(180, 180, 200)}

        grpComets.Controls.AddRange(New Control() {lblCometCount, btnClearComets, btnAddAsteroid, btnAddComet})

        ' ---- 信息面板 ----
        grpInfo = New GroupBox()
        grpInfo.Text = "📋 天体信息"
        grpInfo.ForeColor = Color.FromArgb(180, 200, 255)
        grpInfo.Dock = DockStyle.Fill
        grpInfo.Padding = New Padding(6)
        grpInfo.Margin = New Padding(0, 6, 0, 0)

        txtInfo = New TextBox() With {.Dock = DockStyle.Fill, .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .BackColor = Color.FromArgb(15, 15, 35), .ForeColor = Color.FromArgb(200, 200, 220), .BorderStyle = BorderStyle.None, .Font = New Font("Consolas", 8)}

        grpInfo.Controls.Add(txtInfo)

        ' 组装控制面板
        pnlControl.Controls.Add(grpInfo)
        pnlControl.Controls.Add(grpComets)
        pnlControl.Controls.Add(grpDisplay)
        pnlControl.Controls.Add(grpTime)
        pnlControl.Controls.Add(grpView)

        ' 状态栏
        statusStrip = New StatusStrip()
        statusStrip.BackColor = Color.FromArgb(20, 20, 45)
        toolStripStatusLabel1 = New ToolStripStatusLabel("就绪") With {.ForeColor = Color.FromArgb(180, 180, 200)}
        toolStripStatusLabel2 = New ToolStripStatusLabel("") With {.ForeColor = Color.FromArgb(150, 150, 170), .Alignment = ToolStripItemAlignment.Right}
        statusStrip.Items.AddRange(New ToolStripItem() {toolStripStatusLabel1, toolStripStatusLabel2})

        ' 主窗体布局
        Me.Controls.Add(canvas)
        Me.Controls.Add(pnlControl)
        Me.Controls.Add(statusStrip)

        ' 事件绑定
        AddHandler canvas.Paint, AddressOf canvas_Paint
        AddHandler canvas.MouseWheel, AddressOf canvas_MouseWheel
        AddHandler canvas.MouseDown, AddressOf canvas_MouseDown
        AddHandler canvas.MouseMove, AddressOf canvas_MouseMove
        AddHandler canvas.MouseUp, AddressOf canvas_MouseUp
        AddHandler canvas.Resize, AddressOf canvas_Resize
        AddHandler btnGalactic.Click, AddressOf btnGalactic_Click
        AddHandler btnSolarSystem.Click, AddressOf btnSolarSystem_Click
        AddHandler btnInnerSystem.Click, AddressOf btnInnerSystem_Click
        AddHandler btnPlanetFocus.Click, AddressOf btnPlanetFocus_Click
        AddHandler btnPause.Click, AddressOf btnPause_Click
        AddHandler btnSlower.Click, AddressOf btnSlower_Click
        AddHandler btnFaster.Click, AddressOf btnFaster_Click
        AddHandler btnReset.Click, AddressOf btnReset_Click
        AddHandler btnAddComet.Click, AddressOf btnAddComet_Click
        AddHandler btnAddAsteroid.Click, AddressOf btnAddAsteroid_Click
        AddHandler btnClearComets.Click, AddressOf btnClearComets_Click
        AddHandler chkOrbits.CheckedChanged, AddressOf DisplayOptionChanged
        AddHandler chkTrails.CheckedChanged, AddressOf DisplayOptionChanged
        AddHandler chkLabels.CheckedChanged, AddressOf DisplayOptionChanged
        AddHandler chkGrid.CheckedChanged, AddressOf DisplayOptionChanged
        AddHandler chkSaturnRings.CheckedChanged, AddressOf DisplayOptionChanged
        AddHandler chkCometTails.CheckedChanged, AddressOf DisplayOptionChanged
        AddHandler Me.KeyDown, AddressOf MainForm_KeyDown
    End Sub

    ' ==================== 模拟初始化 ====================

    Private Sub InitializeSimulation()
        ' 创建模拟引擎
        _sim = New SolarSystemSimulation()
        _sim.Initialize()

        ' 创建相机
        _camera = New Camera()
        _camera.CanvasWidth = canvas.Width
        _camera.CanvasHeight = canvas.Height
        _camera.SwitchToSolarSystemView()

        ' 创建渲染器
        _renderer = New SolarSystemRenderer()

        ' 创建动画定时器（~30 FPS）
        _animTimer = New Timer()
        _animTimer.Interval = 33
        AddHandler _animTimer.Tick, AddressOf AnimationTick
        _animTimer.Start()

        ' 初始更新信息
        UpdateInfoPanel()
    End Sub

    ' ==================== 动画循环 ====================

    Private Sub AnimationTick(sender As Object, e As EventArgs)
        If Not _isPaused Then
            ' 更新模拟
            _sim.Update()

            ' 更新行星聚焦视图
            If _camera.Mode = ViewMode.PlanetFocus Then
                _camera.UpdateFocusPosition()
            End If
        End If

        ' 更新 UI
        lblDate.Text = String.Format("日期: {0}", _sim.GetCurrentDateString())
        lblElapsed.Text = String.Format("经过: {0:F2} 年", _sim.CurrentTime)
        lblSpeed.Text = String.Format("速度: {0:F1}x ({1:F2}年/秒)", _sim.TimeScale, _sim.TimeStep * _sim.TimeScale * 30)
        lblCometCount.Text = String.Format("彗星: {0}  小行星: {1}", _sim.Comets.Count, _sim.Asteroids.Count)

        ' 更新状态栏
        toolStripStatusLabel1.Text = String.Format("帧: {0}  |  天体总数: {1}",
            _sim.FrameCount, _sim.AllBodies.Count)
        toolStripStatusLabel2.Text = _camera.GetViewDescription()

        ' 刷新画布
        canvas.Invalidate()
    End Sub

    ' ==================== 画布渲染 ====================

    Private Sub canvas_Paint(sender As Object, e As PaintEventArgs)
        If _sim Is Nothing OrElse _renderer Is Nothing Then Return

        _camera.CanvasWidth = canvas.Width
        _camera.CanvasHeight = canvas.Height

        _renderer.Render(e.Graphics, _sim, _camera)
    End Sub

    ' ==================== 鼠标交互 ====================

    Private Sub canvas_MouseWheel(sender As Object, e As MouseEventArgs)
        _camera.HandleMouseWheel(e)
    End Sub

    Private Sub canvas_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            _isDragging = True
            _lastMousePos = e.Location
            canvas.Cursor = Cursors.Hand
        End If
    End Sub

    Private Sub canvas_MouseMove(sender As Object, e As MouseEventArgs)
        If _isDragging Then
            Dim dx As Single = e.X - _lastMousePos.X
            Dim dy As Single = e.Y - _lastMousePos.Y
            _camera.Pan(dx, dy)
            _lastMousePos = e.Location
        End If
    End Sub

    Private Sub canvas_MouseUp(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            _isDragging = False
            canvas.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub canvas_Resize(sender As Object, e As EventArgs)
        _camera.CanvasWidth = canvas.Width
        _camera.CanvasHeight = canvas.Height
    End Sub

    ' ==================== 视图切换 ====================

    Private Sub btnGalactic_Click(sender As Object, e As EventArgs)
        _camera.SwitchToGalacticView()
    End Sub

    Private Sub btnSolarSystem_Click(sender As Object, e As EventArgs)
        _camera.SwitchToSolarSystemView()
    End Sub

    Private Sub btnInnerSystem_Click(sender As Object, e As EventArgs)
        _camera.SwitchToInnerSolarSystemView()
    End Sub

    Private Sub btnPlanetFocus_Click(sender As Object, e As EventArgs)
        Dim idx As Integer = cmbPlanet.SelectedIndex
        If idx >= 0 AndAlso idx < _sim.Planets.Count Then
            _camera.SwitchToPlanetFocus(_sim.Planets(idx))
            UpdateInfoPanel()
        End If
    End Sub

    ' ==================== 时间控制 ====================

    Private Sub btnPause_Click(sender As Object, e As EventArgs)
        _isPaused = Not _isPaused
        btnPause.Text = If(_isPaused, "▶ 继续", "⏸ 暂停")
        btnPause.BackColor = If(_isPaused, Color.FromArgb(80, 60, 30), Color.FromArgb(60, 60, 90))
    End Sub

    Private Sub btnSlower_Click(sender As Object, e As EventArgs)
        _sim.TimeScale = Math.Max(0.01, _sim.TimeScale / 2)
        trkSpeed.Value = CInt(Math.Log(_sim.TimeScale) / Math.Log(2) * 10)
    End Sub

    Private Sub btnFaster_Click(sender As Object, e As EventArgs)
        _sim.TimeScale = Math.Min(1000000, _sim.TimeScale * 2)
        trkSpeed.Value = CInt(Math.Log(_sim.TimeScale) / Math.Log(2) * 10)
    End Sub

    Private Sub trkSpeed_Scroll(sender As Object, e As EventArgs)
        _sim.TimeScale = Math.Pow(2, trkSpeed.Value / 10.0)
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs)
        _sim.Reset()
        _isPaused = False
        btnPause.Text = "⏸ 暂停"
        btnPause.BackColor = Color.FromArgb(60, 60, 90)
        _camera.SwitchToSolarSystemView()
    End Sub

    ' ==================== 彗星/小行星 ====================

    Private Sub btnAddComet_Click(sender As Object, e As EventArgs)
        _sim.GenerateComet()
        UpdateInfoPanel()
    End Sub

    Private Sub btnAddAsteroid_Click(sender As Object, e As EventArgs)
        _sim.GenerateAsteroid()
        UpdateInfoPanel()
    End Sub

    Private Sub btnClearComets_Click(sender As Object, e As EventArgs)
        _sim.Comets.Clear()
        _sim.Asteroids.Clear()
    End Sub

    ' ==================== 显示选项 ====================

    Private Sub DisplayOptionChanged(sender As Object, e As EventArgs)
        _renderer.ShowOrbits = chkOrbits.Checked
        _renderer.ShowTrails = chkTrails.Checked
        _renderer.ShowLabels = chkLabels.Checked
        _renderer.ShowGrid = chkGrid.Checked
        _renderer.ShowSaturnRings = chkSaturnRings.Checked
        _renderer.ShowCometTails = chkCometTails.Checked
    End Sub

    ' ==================== 键盘快捷键 ====================

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Space
                btnPause_Click(Nothing, Nothing)
            Case Keys.Add, Keys.Oemplus
                btnFaster_Click(Nothing, Nothing)
            Case Keys.Subtract, Keys.OemMinus
                btnSlower_Click(Nothing, Nothing)
            Case Keys.R
                btnReset_Click(Nothing, Nothing)
            Case Keys.D1
                btnGalactic_Click(Nothing, Nothing)
            Case Keys.D2
                btnSolarSystem_Click(Nothing, Nothing)
            Case Keys.D3
                btnInnerSystem_Click(Nothing, Nothing)
            Case Keys.D4
                btnPlanetFocus_Click(Nothing, Nothing)
            Case Keys.C
                btnAddComet_Click(Nothing, Nothing)
            Case Keys.A
                btnAddAsteroid_Click(Nothing, Nothing)
        End Select
    End Sub

    ' ==================== 信息面板更新 ====================

    Private Sub UpdateInfoPanel()
        If _sim Is Nothing Then Return

        Dim sb As New System.Text.StringBuilder()
        sb.AppendLine("═══ 太阳系天体 ═══")
        sb.AppendLine()

        ' 太阳
        sb.AppendLine(String.Format("☉ {0} ({1})", _sim.Sun.Name, _sim.Sun.NameEn))
        sb.AppendLine(String.Format("  质量: {0:E3} kg", _sim.Sun.MassKg))
        sb.AppendLine(String.Format("  半径: {0:N0} km", _sim.Sun.RadiusKm))
        sb.AppendLine()

        ' 行星
        For Each planet In _sim.Planets
            sb.AppendLine(String.Format("● {0} ({1})", planet.Name, planet.NameEn))
            sb.AppendLine(String.Format("  质量: {0:E3} kg", planet.MassKg))
            sb.AppendLine(String.Format("  半径: {0:N0} km", planet.RadiusKm))
            If planet.Orbit IsNot Nothing Then
                sb.AppendLine(String.Format("  轨道半长轴: {0:F3} AU", planet.Orbit.SemiMajorAxis))
                sb.AppendLine(String.Format("  轨道离心率: {0:F4}", planet.Orbit.Eccentricity))
                sb.AppendLine(String.Format("  公转周期: {0:F3} 年", planet.Orbit.Period))
                sb.AppendLine(String.Format("  轨道倾角: {0:F2}°", planet.Orbit.Inclination * 180 / Math.PI))
            End If
            If planet.Moons.Count > 0 Then
                sb.AppendLine(String.Format("  卫星数: {0}", planet.Moons.Count))
                For Each moon In planet.Moons
                    sb.AppendLine(String.Format("    ○ {0} ({1:F0}km, T={2:F2}d)",
                        moon.Name, moon.RadiusKm, moon.Orbit.Period * 365.25))
                Next
            End If
            sb.AppendLine()
        Next

        txtInfo.Text = sb.ToString()
    End Sub

    ' ==================== 窗体关闭 ====================

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        If _animTimer IsNot Nothing Then
            _animTimer.Stop()
            _animTimer.Dispose()
        End If
        MyBase.OnFormClosed(e)
    End Sub

End Class


