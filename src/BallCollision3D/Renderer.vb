' ============================================================
' Renderer.vb - 三维场景绘制
' ============================================================
' 仅使用 Camera 平台无关的数学方法（Rotate / Project）把盒子局部坐标
' 变换到屏幕二维，再用 GDI+ 自绘：
'   - 盒子：12 条边框线条（发光青蓝）；
'   - 小球：按深度由远到近排序绘制的径向渐变实心圆；
'   - 小球颜色：速度大小经 Designer 连续色谱映射得到。
'
' 不调用 Surface.Draw / Camera.Draw（位于 #If WINDOWS 分支，且自绘更可控）。

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing3D

Public Class Renderer

    ' 盒子线框的 12 条边（8 顶点按 0..7 索引）
    Private Shared ReadOnly edges As Integer(,) = {
        {0, 1}, {1, 3}, {3, 2}, {2, 0},   ' 底面
        {4, 5}, {5, 7}, {7, 6}, {6, 4},   ' 顶面
        {0, 4}, {1, 5}, {2, 6}, {3, 7}    ' 立柱
    }

    ''' <summary>当前连续色谱（256 色），由 Designer.GetColors 生成</summary>
    Public palette As Color() = Designer.GetColors("turbo", 256)

    ''' <summary>重新设置调色板名称并重建色谱</summary>
    Public Sub SetPalette(term As String)
        Try
            palette = Designer.GetColors(term, 256)
        Catch
            palette = Designer.GetColors("turbo", 256)
        End Try
    End Sub

    ''' <summary>
    ''' 根据速度大小从色谱取色。
    ''' </summary>
    Private Function SpeedColor(speed As Double, maxSpeed As Double) As Color
        Dim t = If(maxSpeed <= 0, 0, speed / maxSpeed)
        If t < 0 Then t = 0
        If t > 1 Then t = 1
        Dim idx = CInt(t * (palette.Length - 1))
        If idx < 0 Then idx = 0
        If idx >= palette.Length Then idx = palette.Length - 1
        Return palette(idx)
    End Function

    ''' <summary>
    ''' 绘制一帧：盒子线框 + 小球（按深度排序）。
    ''' </summary>
    Public Sub Render(g As Graphics, sim As Simulation, camera As Camera)
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.Clear(Color.FromArgb(10, 14, 26))

        ' 线框盒子必须与物理碰撞空间一致：局部系 [0, BoxSize]^3，
        ' 这样旋转/投影后线框正好包裹住在此范围内活动的小球。
        Dim s = sim.BoxSize
        Dim corners = New Point3D() {
            New Point3D(0, 0, 0),
            New Point3D(s, 0, 0),
            New Point3D(s, s, 0),
            New Point3D(0, s, 0),
            New Point3D(0, 0, s),
            New Point3D(s, 0, s),
            New Point3D(s, s, s),
            New Point3D(0, s, s)
        }

        ' 旋转 + 投影盒子顶点
        Dim rc = camera.Rotate(corners)
        Dim pc = camera.Project(rc)

        ' ---- 画盒子线框（发光青蓝）----
        Using pen As New Pen(Color.FromArgb(120, 31, 182, 255), 2.0F)
            For i As Integer = 0 To edges.GetLength(0) - 1
                Dim a = pc(edges(i, 0))
                Dim b = pc(edges(i, 1))
                g.DrawLine(pen, CSng(a.X), CSng(a.Y), CSng(b.X), CSng(b.Y))
            Next
        End Using

        ' ---- 投影小球并按深度排序（远→近）----
        Dim proj = New List(Of BallProj)
        For Each ball In sim.balls
            Dim c = camera.Rotate(New Point3D(ball.position.x, ball.position.y, ball.position.z))
            Dim p = camera.Project(c)
            Dim r2d = ball.radius * camera.FieldOfView / (camera.ViewDistance + p.Z)
            If r2d < 1 Then r2d = 1
            proj.Add(New BallProj With {
                .x = CSng(p.X),
                .y = CSng(p.Y),
                .r = CSng(r2d),
                .z = p.Z,
                .color = SpeedColor(ball.Speed, sim.maxSpeed)
            })
        Next

        proj.Sort(Function(a, b) b.z.CompareTo(a.z))

        ' ---- 由远到近绘制小球 ----
        For Each bp In proj
            DrawBall(g, bp)
        Next
    End Sub

    Private Sub DrawBall(g As Graphics, bp As BallProj)
        ' 外发光
        Using glow As New SolidBrush(Color.FromArgb(40, bp.color.R, bp.color.G, bp.color.B))
            g.FillEllipse(glow, bp.x - bp.r * 1.35F, bp.y - bp.r * 1.35F, bp.r * 2.7F, bp.r * 2.7F)
        End Using

        ' 径向渐变实心圆（中心亮、边缘暗，营造立体感）
        Using ellipsePath As New GraphicsPath()
            ellipsePath.AddEllipse(bp.x - bp.r, bp.y - bp.r, bp.r * 2, bp.r * 2)
            Using pgb As New PathGradientBrush(ellipsePath)
                pgb.CenterColor = Color.FromArgb(255,
                    CByte(Math.Min(255, bp.color.R + 90)),
                    CByte(Math.Min(255, bp.color.G + 90)),
                    CByte(Math.Min(255, bp.color.B + 90)))
                pgb.SurroundColors = {bp.color}

                g.FillEllipse(pgb, bp.x - bp.r, bp.y - bp.r, bp.r * 2, bp.r * 2)
            End Using
        End Using

        ' 高光点
        Using hl As New SolidBrush(Color.FromArgb(180, 255, 255, 255))
            g.FillEllipse(hl, bp.x - bp.r * 0.35F, bp.y - bp.r * 0.4F, bp.r * 0.4F, bp.r * 0.3F)
        End Using
    End Sub

    ''' <summary>小球投影数据（用于排序绘制）</summary>
    Private Class BallProj
        Public x As Single
        Public y As Single
        Public r As Single
        Public z As Double
        Public color As Color
    End Class

End Class
