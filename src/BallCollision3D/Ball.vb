' ============================================================
' Ball.vb - 三维小球模型
' ============================================================
' 小球的所有物理状态保存在「盒子局部坐标系」中：
'   盒子局部坐标范围 [0, BoxSize]^3，中心在 (BoxSize/2, BoxSize/2, BoxSize/2)。
' 渲染时再由 Camera 旋转/投影到屏幕。
'
' 复用 framework\gr\physics 的 Vector3 作为三维向量原语。

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Physics

Public Class Ball

    ''' <summary>小球球心在盒子局部坐标系中的位置</summary>
    Public position As Vector3

    ''' <summary>小球在盒子局部坐标系中的速度向量</summary>
    Public velocity As Vector3

    ''' <summary>小球半径（局部坐标单位）</summary>
    Public radius As Double

    ''' <summary>小球质量（与半径立方成正比，正比于密度）</summary>
    Public mass As Double

    ''' <summary>当前显示颜色（由速度映射到连续色谱后写入）</summary>
    Public color As Color

    ''' <summary>用于初始随机布点的随机源</summary>
    Private Shared ReadOnly rnd As New Random()

    Sub New(Optional position As Vector3 = Nothing,
            Optional velocity As Vector3 = Nothing,
            Optional radius As Double = 10.0,
            Optional mass As Double = 1.0)
        Me.position = If(position, New Vector3(0, 0, 0))
        Me.velocity = If(velocity, New Vector3(0, 0, 0))
        Me.radius = radius
        Me.mass = mass
        Me.color = Color.White
    End Sub

    ''' <summary>
    ''' 当前速度大小（标量速率）。用于映射到颜色谱。
    ''' </summary>
    Public ReadOnly Property Speed As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return velocity.Magnitude
        End Get
    End Property

    ''' <summary>
    ''' 在给定盒子尺寸内随机生成一个不重叠的小球。
    ''' </summary>
    ''' <param name="boxSize">盒子局部边长</param>
    ''' <param name="radius">小球半径</param>
    ''' <param name="existing">已存在的小球，用于做两两重叠剔除</param>
    Public Shared Function RandomBall(boxSize As Double, radius As Double, existing As IEnumerable(Of Ball)) As Ball
        Dim half = radius * 1.05
        Dim min = half
        Dim max = boxSize - half
        Dim pos As Vector3
        Dim ok As Boolean
        Dim attempt As Integer = 0

        Do
            pos = New Vector3(
                rnd.NextDouble() * (max - min) + min,
                rnd.NextDouble() * (max - min) + min,
                rnd.NextDouble() * (max - min) + min
            )

            ok = True
            For Each b In existing
                If (pos - b.position).Magnitude < (radius + b.radius) * 1.02 Then
                    ok = False
                    Exit For
                End If
            Next

            attempt += 1
            ' 最多尝试 40 次，避免极端拥挤时死循环
            If attempt > 40 Then ok = True
        Loop While Not ok

        ' 初始赋予一个较小的随机初速度
        Dim v = New Vector3(
            (rnd.NextDouble() - 0.5) * 120,
            (rnd.NextDouble() - 0.5) * 120,
            (rnd.NextDouble() - 0.5) * 120
        )

        ' 质量正比于体积（密度取 1）
        Dim mass = 4.0 / 3.0 * Math.PI * radius * radius * radius

        Return New Ball(pos, v, radius, mass)
    End Function

End Class
