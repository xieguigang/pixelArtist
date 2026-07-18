' ============================================================
' Simulation.vb - 三维小球物理引擎
' ============================================================
' 在「盒子局部坐标系」[0, BoxSize]^3 中计算全部物理：
'   - 半隐式欧拉积分；
'   - 球-盒六个内面：逐轴反射（乘 -restitution 损耗能量）+ 切向摩擦衰减；
'   - 球-球：冲量法弹性碰撞（乘 restitution 损耗能量）+ 重叠位置分离。
'
' 重力在「世界系」始终指向窗口底部 (0,-1,0)。盒子本身只被 Camera 旋转（无平移），
' 因此把世界向下向量用与 Camera 旋转「逆序」的 Euler 旋转（角度取负）变换到
' 盒子局部系，即得到随盒子旋转而改变的局部重力方向 —— 旋转盒子即改变小球受力。
'
' 复用 framework\gr\physics 的 Vector3（三维向量）与 PhysicsMaterial（摩擦/恢复系数）。

Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Physics

Public Class Simulation

    ''' <summary>盒子局部坐标系边长（中心在 BoxSize/2）</summary>
    Public BoxSize As Double = 200.0

    ''' <summary>世界系重力强度（局部系再乘以方向向量）</summary>
    Public gravityStrength As Double = 700.0

    ''' <summary>恢复系数（球-墙、球-球共用），&lt;1 表示每次碰撞损耗能量</summary>
    Public restitution As Double = 0.85

    ''' <summary>切向摩擦衰减系数（0=无摩擦，越大摩擦越强）</summary>
    Public friction As Double = 0.04

    ''' <summary>固定积分步长（秒）</summary>
    Public dt As Double = 1.0 / 60.0

    ''' <summary>小球集合</summary>
    Public balls As New List(Of Ball)

    ''' <summary>每帧由 Camera 逆旋转 (0,-1,0) 得到的盒子局部重力向量</summary>
    Public gravityLocal As Vector3

    ''' <summary>用于初始化的小球半径</summary>
    Public ballRadius As Double = 12.0

    ''' <summary>滑动最大速率，用于颜色谱自适应（避免过早饱和）</summary>
    Public maxSpeed As Double = 1.0

    Private _pendingCount As Integer = 15

    Sub New(Optional count As Integer = 15)
        _pendingCount = count
        Rebuild(count)
    End Sub

    ''' <summary>
    ''' 重建小球集合（数量变化或重置时调用）。
    ''' </summary>
    Public Sub Rebuild(count As Integer)
        balls.Clear()
        _pendingCount = Math.Max(1, count)

        ' 球数过多时缩小半径，保证放得下
        Dim r = ballRadius
        If _pendingCount > 30 Then
            r = ballRadius * 0.7
        End If

        For i As Integer = 1 To _pendingCount
            balls.Add(Ball.RandomBall(BoxSize, r, balls))
        Next

        maxSpeed = 1.0
    End Sub

    ''' <summary>
    ''' 把世界系向下向量 (0,-1,0) 用与 Camera Euler 旋转逆序（先 Z、后 Y、后 X，
    ''' 角度取负）的旋转变换到盒子局部坐标系。
    ''' </summary>
    Private Function ComputeGravityLocal(camera As Camera) As Vector3
        ' 世界向下（屏幕向下为正 Y 屏幕系；这里用局部系约定：屏幕下方为局部 -Y 的反向，
        ' 直接取 (0,-1,0) 作为世界向下，逆旋转后即随盒子朝向变化）
        Dim v = New Vector3(0, -1, 0)

        ' 逆序 = 先撤销 Z，再撤销 Y，再撤销 X；角度取负
        v = RotateZ(v, -camera.AngleZ)
        v = RotateY(v, -camera.AngleY)
        v = RotateX(v, -camera.AngleX)

        Return v * gravityStrength
    End Function

    Private Shared Function RotateX(v As Vector3, angleDeg As Double) As Vector3
        Dim rad = angleDeg * Math.PI / 180
        Dim c = Math.Cos(rad), s = Math.Sin(rad)
        Dim y = v.y * c - v.z * s
        Dim z = v.y * s + v.z * c
        Return New Vector3(v.x, y, z)
    End Function

    Private Shared Function RotateY(v As Vector3, angleDeg As Double) As Vector3
        Dim rad = angleDeg * Math.PI / 180
        Dim c = Math.Cos(rad), s = Math.Sin(rad)
        Dim x = v.x * c + v.z * s
        Dim z = -v.x * s + v.z * c
        Return New Vector3(x, v.y, z)
    End Function

    Private Shared Function RotateZ(v As Vector3, angleDeg As Double) As Vector3
        Dim rad = angleDeg * Math.PI / 180
        Dim c = Math.Cos(rad), s = Math.Sin(rad)
        Dim x = v.x * c - v.y * s
        Dim y = v.x * s + v.y * c
        Return New Vector3(x, y, v.z)
    End Function

    ''' <summary>
    ''' 推进一个固定步长：重算局部重力 → 半隐式欧拉积分 → 球-墙碰撞 → 球-球碰撞。
    ''' </summary>
    Public Sub [Step](camera As Camera)
        gravityLocal = ComputeGravityLocal(camera)

        ' 1. 半隐式欧拉：先更新速度，再用新速度更新位置
        For Each b In balls
            b.velocity = b.velocity + gravityLocal * dt
            b.position = b.position + b.velocity * dt
        Next

        ' 2. 球-墙碰撞（逐轴）
        ResolveWalls()

        ' 3. 球-球碰撞（O(N^2)，球数较小足够）
        ResolveBalls()

        ' 4. 更新自适应最大速率
        Dim curMax = 1.0
        For Each b In balls
            If b.Speed > curMax Then curMax = b.Speed
        Next
        ' 缓慢跟踪，避免瞬态抖动导致色谱闪烁
        maxSpeed = Math.Max(maxSpeed * 0.995, curMax)
    End Sub

    Private Sub ResolveWalls()
        Dim lo = ballRadiusMin()
        Dim hi = BoxSize - lo

        For Each b In balls
            ' ---- X 轴 ----
            If b.position.x < lo Then
                b.position.x = lo
                If b.velocity.x < 0 Then
                    b.velocity.x = -b.velocity.x * restitution
                    b.velocity.y *= (1 - friction)
                    b.velocity.z *= (1 - friction)
                End If
            ElseIf b.position.x > hi Then
                b.position.x = hi
                If b.velocity.x > 0 Then
                    b.velocity.x = -b.velocity.x * restitution
                    b.velocity.y *= (1 - friction)
                    b.velocity.z *= (1 - friction)
                End If
            End If

            ' ---- Y 轴 ----
            If b.position.y < lo Then
                b.position.y = lo
                If b.velocity.y < 0 Then
                    b.velocity.y = -b.velocity.y * restitution
                    b.velocity.x *= (1 - friction)
                    b.velocity.z *= (1 - friction)
                End If
            ElseIf b.position.y > hi Then
                b.position.y = hi
                If b.velocity.y > 0 Then
                    b.velocity.y = -b.velocity.y * restitution
                    b.velocity.x *= (1 - friction)
                    b.velocity.z *= (1 - friction)
                End If
            End If

            ' ---- Z 轴 ----
            If b.position.z < lo Then
                b.position.z = lo
                If b.velocity.z < 0 Then
                    b.velocity.z = -b.velocity.z * restitution
                    b.velocity.x *= (1 - friction)
                    b.velocity.y *= (1 - friction)
                End If
            ElseIf b.position.z > hi Then
                b.position.z = hi
                If b.velocity.z > 0 Then
                    b.velocity.z = -b.velocity.z * restitution
                    b.velocity.x *= (1 - friction)
                    b.velocity.y *= (1 - friction)
                End If
            End If
        Next
    End Sub

    Private Sub ResolveBalls()
        For i As Integer = 0 To balls.Count - 1
            For j As Integer = i + 1 To balls.Count - 1
                Dim a = balls(i), b = balls(j)
                Dim delta = a.position - b.position
                Dim dist = delta.Magnitude
                Dim minDist = a.radius + b.radius

                If dist > 0 AndAlso dist < minDist Then
                    ' 碰撞法线（由 b 指向 a）
                    Dim n = delta * (1.0 / dist)
                    Dim relVel = a.velocity - b.velocity
                    Dim velAlongNormal = Vector3.Dot(relVel, n)

                    ' 已在分离则不处理
                    If velAlongNormal > 0 Then Continue For

                    Dim invA = 1.0 / a.mass
                    Dim invB = 1.0 / b.mass
                    Dim e = restitution

                    ' 冲量标量
                    Dim jImp = -(1 + e) * velAlongNormal / (invA + invB)

                    Dim imp = n * jImp
                    a.velocity = a.velocity + imp * invA
                    b.velocity = b.velocity - imp * invB

                    ' 位置分离（按质量反比平分重叠）
                    Dim overlap = minDist - dist
                    Dim totalInv = invA + invB
                    Dim corrA = n * (overlap * (invA / totalInv))
                    Dim corrB = n * (overlap * (invB / totalInv))
                    a.position = a.position + corrA
                    b.position = b.position - corrB
                End If
            Next
        Next
    End Sub

    ''' <summary>系统中最小的半径，用于墙边界钳位</summary>
    Private Function ballRadiusMin() As Double
        Dim m = Double.MaxValue
        For Each b In balls
            If b.radius < m Then m = b.radius
        Next
        Return If(m = Double.MaxValue, ballRadius, m)
    End Function

    ''' <summary>系统当前总动能（用于状态栏显示）</summary>
    Public ReadOnly Property TotalKineticEnergy As Double
        Get
            Dim ke = 0.0
            For Each b In balls
                ke += 0.5 * b.mass * b.velocity.Magnitude ^ 2
            Next
            Return ke
        End Get
    End Property

End Class
