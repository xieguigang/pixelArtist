' ============================================================
' MainForm.vb - 主窗体
' ============================================================
' 三维盒子内小球碰撞模拟主界面：
'   - 右侧全屏画布（PictureBox + GDI+ 渲染）
'   - 左侧深色玻璃质感控制面板（数量/重力/弹性/摩擦/调色板/暂停/重置）
'   - 底部状态栏（FPS / 小球数 / 调色板 / 总动能）
'   - 左键旋转盒子、滚轮缩放、右键平移

Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Imaging.Drawing3D

Public Class MainForm : Inherits Form

    ' 双缓冲画布，消除重绘闪烁
    Private Class BufferedBox : Inherits PictureBox
        Sub New()
            Me.DoubleBuffered = True
        End Sub
    End Class

    ' ==================== UI 控件 ====================

    Private WithEvents canvas As BufferedBox
    Private pnlControl As Panel

    Private grpParams As GroupBox
    Private lblCount As Label
    Private numCount As NumericUpDown
    Private lblGravity As Label
    Private trkGravity As TrackBar
    Private lblGravityVal As Label
    Private lblRestitution As Label
    Private trkRestitution As TrackBar
    Private lblRestitutionVal As Label
    Private lblFriction As Label
    Private trkFriction As TrackBar
    Private lblFrictionVal As Label

    Private grpDisplay As GroupBox
    Private lblPalette As Label
    Private cmbPalette As ComboBox
    Private btnPause As Button
    Private btnReset As Button

    Private grpHint As GroupBox
    Private lblHint As Label

    Private statusStrip As StatusStrip
    Private tssFps As ToolStripStatusLabel
    Private tssInfo As ToolStripStatusLabel
    Private tssEnergy As ToolStripStatusLabel

    ' ==================== 引擎与状态 ====================

    Private _sim As Simulation
    Private _camera As Camera
    Private _renderer As Renderer
    Private _animTimer As Timer

    Private _isPaused As Boolean = False
    Private _dragMode As Integer = 0   ' 0=无, 1=左键旋转, 2=右键平移
    Private _lastMouse As Point
    Private _frameCount As Long = 0
    Private _fps As Double = 0
    Private _fpsWatch As Stopwatch

    ' 可用调色板
    Private Shared ReadOnly Palettes As String() = {
        "turbo", "jet", "viridis", "viridis:magma",
        "viridis:plasma", "viridis:inferno", "rainbow", "viridis:rocket"
    }

    ' ==================== 构造函数 ====================

    Public Sub New()
        InitializeComponent()
        InitializeEngine()
    End Sub

    ' ==================== UI 初始化 ====================

    Private Sub InitializeComponent()
        Me.Text = "三维小球碰撞模拟 - Ball Collision 3D"
        Me.Size = New Size(1280, 820)
        Me.MinimumSize = New Size(900, 620)
        Me.BackColor = Color.FromArgb(10, 14, 26)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Font = New Font("Microsoft YaHei", 9)

        ' 画布
        canvas = New BufferedBox()
        canvas.Dock = DockStyle.Fill
        canvas.BackColor = Color.FromArgb(10, 14, 26)

        ' 控制面板
        pnlControl = New Panel()
        pnlControl.Width = 250
        pnlControl.Dock = DockStyle.Left
        pnlControl.BackColor = Color.FromArgb(18, 24, 41)
        pnlControl.Padding = New Padding(10)

        ' ---- 参数组 ----
        grpParams = New GroupBox()
        grpParams.Text = "模拟参数"
        grpParams.ForeColor = Color.FromArgb(159, 179, 200)
        grpParams.Dock = DockStyle.Top
        grpParams.Height = 270
        grpParams.Padding = New Padding(8)
        grpParams.Font = New Font("Microsoft YaHei", 9)

        lblCount = New Label() With {.Text = "小球数量", .Dock = DockStyle.Top, .Height = 20, .ForeColor = Color.FromArgb(230, 240, 255)}
        numCount = New NumericUpDown() With {.Dock = DockStyle.Top, .Height = 24, .Minimum = 1, .Maximum = 9999, .Value = 500}
        numCount.BackColor = Color.FromArgb(30, 38, 60)
        numCount.ForeColor = Color.White

        lblGravity = New Label() With {.Text = "重力强度", .Dock = DockStyle.Top, .Height = 18, .ForeColor = Color.FromArgb(230, 240, 255), .Margin = New Padding(0, 6, 0, 0)}
        trkGravity = New TrackBar() With {.Dock = DockStyle.Top, .Height = 30, .Minimum = 0, .Maximum = 1500, .Value = 700, .TickFrequency = 100}
        lblGravityVal = New Label() With {.Text = "700", .Dock = DockStyle.Top, .Height = 16, .ForeColor = Color.FromArgb(159, 179, 200)}

        lblRestitution = New Label() With {.Text = "恢复系数 (弹性)", .Dock = DockStyle.Top, .Height = 18, .ForeColor = Color.FromArgb(230, 240, 255), .Margin = New Padding(0, 6, 0, 0)}
        trkRestitution = New TrackBar() With {.Dock = DockStyle.Top, .Height = 30, .Minimum = 0, .Maximum = 100, .Value = 85, .TickFrequency = 5}
        lblRestitutionVal = New Label() With {.Text = "0.85", .Dock = DockStyle.Top, .Height = 16, .ForeColor = Color.FromArgb(159, 179, 200)}

        lblFriction = New Label() With {.Text = "摩擦系数", .Dock = DockStyle.Top, .Height = 18, .ForeColor = Color.FromArgb(230, 240, 255), .Margin = New Padding(0, 6, 0, 0)}
        trkFriction = New TrackBar() With {.Dock = DockStyle.Top, .Height = 30, .Minimum = 0, .Maximum = 100, .Value = 4, .TickFrequency = 5}
        lblFrictionVal = New Label() With {.Text = "0.04", .Dock = DockStyle.Top, .Height = 16, .ForeColor = Color.FromArgb(159, 179, 200)}

        grpParams.Controls.AddRange(New Control() {
            lblFrictionVal, trkFriction, lblFriction,
            lblRestitutionVal, trkRestitution, lblRestitution,
            lblGravityVal, trkGravity, lblGravity,
            numCount, lblCount
        })

        ' ---- 显示组 ----
        grpDisplay = New GroupBox()
        grpDisplay.Text = "显示与操作"
        grpDisplay.ForeColor = Color.FromArgb(159, 179, 200)
        grpDisplay.Dock = DockStyle.Top
        grpDisplay.Height = 160
        grpDisplay.Padding = New Padding(8)
        grpDisplay.Font = New Font("Microsoft YaHei", 9)
        grpDisplay.Margin = New Padding(0, 8, 0, 0)

        lblPalette = New Label() With {.Text = "速度色谱", .Dock = DockStyle.Top, .Height = 20, .ForeColor = Color.FromArgb(230, 240, 255)}
        cmbPalette = New ComboBox() With {.Dock = DockStyle.Top, .Height = 24, .DropDownStyle = ComboBoxStyle.DropDownList}
        cmbPalette.Items.AddRange(Palettes)
        cmbPalette.SelectedIndex = 0
        cmbPalette.BackColor = Color.FromArgb(30, 38, 60)
        cmbPalette.ForeColor = Color.White

        btnPause = New Button() With {.Text = "⏸ 暂停", .Dock = DockStyle.Top, .Height = 30, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(60, 60, 100), .ForeColor = Color.White, .Margin = New Padding(0, 8, 0, 0)}
        btnReset = New Button() With {.Text = "⟳ 重置", .Dock = DockStyle.Top, .Height = 30, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(40, 80, 70), .ForeColor = Color.White, .Margin = New Padding(0, 6, 0, 0)}

        grpDisplay.Controls.AddRange(New Control() {btnReset, btnPause, cmbPalette, lblPalette})

        ' ---- 提示组 ----
        grpHint = New GroupBox()
        grpHint.Text = "交互提示"
        grpHint.ForeColor = Color.FromArgb(159, 179, 200)
        grpHint.Dock = DockStyle.Fill
        grpHint.Padding = New Padding(8)
        grpHint.Font = New Font("Microsoft YaHei", 9)
        grpHint.Margin = New Padding(0, 8, 0, 0)

        lblHint = New Label() With {
            .Text = "🖱 左键拖拽：旋转盒子" & vbCrLf &
                     "🔍 滚轮：缩放视角" & vbCrLf &
                     "✋ 右键拖拽：平移场景",
            .Dock = DockStyle.Fill,
            .ForeColor = Color.FromArgb(159, 179, 200)
        }
        grpHint.Controls.Add(lblHint)

        pnlControl.Controls.Add(grpHint)
        pnlControl.Controls.Add(grpDisplay)
        pnlControl.Controls.Add(grpParams)

        ' 状态栏
        statusStrip = New StatusStrip()
        statusStrip.BackColor = Color.FromArgb(18, 24, 41)
        statusStrip.ForeColor = Color.FromArgb(159, 179, 200)
        tssFps = New ToolStripStatusLabel("FPS: --") With {.Font = New Font("Consolas", 9)}
        tssInfo = New ToolStripStatusLabel("") With {.Font = New Font("Consolas", 9)}
        tssEnergy = New ToolStripStatusLabel("") With {.Font = New Font("Consolas", 9), .Alignment = ToolStripItemAlignment.Right}
        statusStrip.Items.AddRange(New ToolStripItem() {tssFps, tssInfo, tssEnergy})

        ' 主布局
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
        AddHandler numCount.ValueChanged, AddressOf numCount_ValueChanged
        AddHandler trkGravity.Scroll, AddressOf trkGravity_Scroll
        AddHandler trkRestitution.Scroll, AddressOf trkRestitution_Scroll
        AddHandler trkFriction.Scroll, AddressOf trkFriction_Scroll
        AddHandler cmbPalette.SelectedIndexChanged, AddressOf cmbPalette_SelectedIndexChanged
        AddHandler btnPause.Click, AddressOf btnPause_Click
        AddHandler btnReset.Click, AddressOf btnReset_Click
        AddHandler Me.KeyDown, AddressOf MainForm_KeyDown
    End Sub

    ' ==================== 引擎初始化 ====================

    Private Sub InitializeEngine()
        _sim = New Simulation(15)
        _camera = New Camera()
        _camera.AngleX = 28
        _camera.AngleY = -32
        _camera.AngleZ = 0
        _camera.ViewDistance = 520
        _camera.FieldOfView = 512
        _camera.Screen = New Size(canvas.Width, canvas.Height)
        _camera.Offset = New PointF(0, 0)

        _renderer = New Renderer()
        _renderer.SetPalette(Palettes(0))

        _fpsWatch = New Stopwatch()
        _fpsWatch.Start()

        _animTimer = New Timer()
        _animTimer.Interval = 33
        AddHandler _animTimer.Tick, AddressOf AnimationTick
        _animTimer.Start()
    End Sub

    ' ==================== 动画循环 ====================

    Private Sub AnimationTick(sender As Object, e As EventArgs)
        If Not _isPaused Then
            ' 固定步长推进物理
            _sim.Step(_camera)
        End If

        _frameCount += 1
        If _fpsWatch.ElapsedMilliseconds >= 500 Then
            _fps = _frameCount / (_fpsWatch.ElapsedMilliseconds / 1000.0)
            _frameCount = 0
            _fpsWatch.Restart()
            UpdateStatus()
        End If

        canvas.Invalidate()
    End Sub

    Private Sub UpdateStatus()
        tssFps.Text = String.Format("FPS: {0:F1}", _fps)
        tssInfo.Text = String.Format("小球: {0}  |  重力: {1:F0}  弹性: {2:F2}  摩擦: {3:F2}  [{4}]",
            _sim.balls.Count, _sim.gravityStrength, _sim.restitution, _sim.friction,
            Palettes(cmbPalette.SelectedIndex))
        tssEnergy.Text = String.Format("动能: {0:E3}", _sim.TotalKineticEnergy)
    End Sub

    ' ==================== 画布渲染 ====================

    Private Sub canvas_Paint(sender As Object, e As PaintEventArgs)
        If _sim Is Nothing OrElse _renderer Is Nothing Then Return
        _camera.Screen = New Size(canvas.Width, canvas.Height)
        _renderer.Render(e.Graphics, _sim, _camera)
    End Sub

    ' ==================== 鼠标交互 ====================

    Private Sub canvas_MouseWheel(sender As Object, e As MouseEventArgs)
        ' 滚轮缩放视角（视距）
        Dim nd = _camera.ViewDistance - e.Delta * 0.5F
        If nd < 120 Then nd = 120
        If nd > 1400 Then nd = 1400
        _camera.ViewDistance = nd
    End Sub

    Private Sub canvas_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            _dragMode = 1
            _lastMouse = e.Location
            canvas.Cursor = Cursors.Hand
        ElseIf e.Button = MouseButtons.Right Then
            _dragMode = 2
            _lastMouse = e.Location
            canvas.Cursor = Cursors.SizeAll
        End If
    End Sub

    Private Sub canvas_MouseMove(sender As Object, e As MouseEventArgs)
        If _dragMode = 1 Then
            Dim dx = e.X - _lastMouse.X
            Dim dy = e.Y - _lastMouse.Y
            _camera.AngleY += dx * 0.4F
            _camera.AngleX += dy * 0.4F
            ' AngleX 限制在 ±89° 避免翻转
            If _camera.AngleX > 89 Then _camera.AngleX = 89
            If _camera.AngleX < -89 Then _camera.AngleX = -89
            _lastMouse = e.Location
        ElseIf _dragMode = 2 Then
            Dim dx = e.X - _lastMouse.X
            Dim dy = e.Y - _lastMouse.Y
            _camera.Offset = New PointF(_camera.Offset.X + dx, _camera.Offset.Y + dy)
            _lastMouse = e.Location
        End If
    End Sub

    Private Sub canvas_MouseUp(sender As Object, e As MouseEventArgs)
        _dragMode = 0
        canvas.Cursor = Cursors.Default
    End Sub

    Private Sub canvas_Resize(sender As Object, e As EventArgs)
        _camera.Screen = New Size(canvas.Width, canvas.Height)
    End Sub

    ' ==================== 控制面板 ====================

    Private Sub numCount_ValueChanged(sender As Object, e As EventArgs)
        _sim.Rebuild(CInt(numCount.Value))
        UpdateStatus()
    End Sub

    Private Sub trkGravity_Scroll(sender As Object, e As EventArgs)
        _sim.gravityStrength = trkGravity.Value
        lblGravityVal.Text = trkGravity.Value.ToString
        UpdateStatus()
    End Sub

    Private Sub trkRestitution_Scroll(sender As Object, e As EventArgs)
        _sim.restitution = trkRestitution.Value / 100.0
        lblRestitutionVal.Text = (_sim.restitution).ToString("F2")
        UpdateStatus()
    End Sub

    Private Sub trkFriction_Scroll(sender As Object, e As EventArgs)
        _sim.friction = trkFriction.Value / 100.0
        lblFrictionVal.Text = (_sim.friction).ToString("F2")
        UpdateStatus()
    End Sub

    Private Sub cmbPalette_SelectedIndexChanged(sender As Object, e As EventArgs)
        _renderer.SetPalette(Palettes(cmbPalette.SelectedIndex))
        UpdateStatus()
    End Sub

    Private Sub btnPause_Click(sender As Object, e As EventArgs)
        _isPaused = Not _isPaused
        btnPause.Text = If(_isPaused, "▶ 继续", "⏸ 暂停")
        btnPause.BackColor = If(_isPaused, Color.FromArgb(120, 60, 60), Color.FromArgb(60, 60, 100))
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs)
        _sim.Rebuild(CInt(numCount.Value))
        _camera.AngleX = 28
        _camera.AngleY = -32
        _camera.AngleZ = 0
        _camera.ViewDistance = 520
        _camera.Offset = New PointF(0, 0)
        _isPaused = False
        btnPause.Text = "⏸ 暂停"
        btnPause.BackColor = Color.FromArgb(60, 60, 100)
        UpdateStatus()
    End Sub

    ' ==================== 键盘快捷键 ====================

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Space
                btnPause_Click(Nothing, Nothing)
            Case Keys.R
                btnReset_Click(Nothing, Nothing)
        End Select
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
