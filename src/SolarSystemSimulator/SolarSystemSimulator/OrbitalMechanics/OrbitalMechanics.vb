Imports System.Drawing

''' <summary>
''' 轨道力学计算器 —— 纯数学函数实现
''' 所有计算基于开普勒定律和牛顿万有引力定律
''' </summary>
Public Module OrbitalMechanics

    ' ==================== 常量 ====================

    ''' <summary>天文单位（千米）</summary>
    Public Const AU_KM As Double = 149597870.7

    ''' <summary>光年（千米）</summary>
    Public Const LIGHT_YEAR_KM As Double = 9460730472580.8

    ''' <summary>秒差距（千米）</summary>
    Public Const PARSEC_KM As Double = 30856775814913.7

    ''' <summary>1年（天）</summary>
    Public Const YEAR_DAYS As Double = 365.25

    ''' <summary>太阳引力参数 GM☉（AU³/年²）—— 由开普勒第三定律推导</summary>
    ''' GM = 4π²a³/T²，取 a=1AU, T=1年 → GM = 4π²
    Public Const GM_SUN As Double = 4.0 * Math.PI * Math.PI

    ''' <summary>太阳绕银河系中心公转半径（光年）</summary>
    Public Const SUN_GALACTIC_RADIUS_LY As Double = 26660.0

    ''' <summary>太阳绕银河系中心公转周期（百万年）</summary>
    Public Const SUN_GALACTIC_PERIOD_MYR As Double = 225.0

    ''' <summary>太阳绕银河系中心公转速度（千米/秒）</summary>
    Public Const SUN_GALACTIC_VELOCITY_KMS As Double = 220.0

    ' ==================== 开普勒方程求解 ====================

    ''' <summary>
    ''' 求解开普勒方程 M = E - e·sin(E)（牛顿迭代法）
    ''' 适用于椭圆轨道（0 ≤ e &lt; 1）
    ''' </summary>
    ''' <param name="M">平近点角（弧度）</param>
    ''' <param name="e">离心率</param>
    ''' <param name="tolerance">收敛精度（弧度）</param>
    ''' <returns>偏近点角 E（弧度）</returns>
    Public Function SolveKeplerEquation(M As Double, e As Double,
                                        Optional tolerance As Double = 0.000000000001) As Double
        ' 将 M 归一化到 [0, 2π)
        M = M Mod (2.0 * Math.PI)
        If M < 0 Then M += 2.0 * Math.PI

        ' 初始猜测：E₀ = M（低离心率时收敛快）
        Dim E0 As Double = M

        ' 牛顿迭代法：E_{n+1} = E_n - f(E_n)/f'(E_n)
        ' f(E) = E - e·sin(E) - M
        ' f'(E) = 1 - e·cos(E)
        For iter As Integer = 0 To 200
            Dim f As Double = E0 - e * Math.Sin(E0) - M
            Dim fPrime As Double = 1.0 - e * Math.Cos(E0)

            ' 防止除零
            If Math.Abs(fPrime) < 0.000000000000001 Then
                fPrime = 0.000000000000001
            End If

            Dim delta As Double = f / fPrime
            E0 -= delta

            ' 收敛判断
            If Math.Abs(delta) < tolerance Then Exit For
        Next

        Return E0
    End Function

    ''' <summary>
    ''' 求解双曲线开普勒方程 M = e·sinh(H) - H（牛顿迭代法）
    ''' 适用于双曲线轨道（e > 1）
    ''' </summary>
    ''' <param name="M">平近点角（弧度）</param>
    ''' <param name="e">离心率（> 1）</param>
    ''' <returns>双曲近点角 H</returns>
    Public Function SolveKeplerHyperbolic(M As Double, e As Double,
                                          Optional tolerance As Double = 0.000000000001) As Double
        ' 初始猜测
        Dim H As Double = Math.Log(2.0 * Math.Abs(M) / e + 1.8)

        For iter As Integer = 0 To 200
            Dim f As Double = e * Math.Sinh(H) - H - M
            Dim fPrime As Double = e * Math.Cosh(H) - 1.0

            If Math.Abs(fPrime) < 0.000000000000001 Then
                fPrime = 0.000000000000001
            End If

            Dim delta As Double = f / fPrime
            H -= delta

            If Math.Abs(delta) < tolerance Then Exit For
        Next

        Return H
    End Function

    ' ==================== 角度转换 ====================

    ''' <summary>
    ''' 偏近点角 E → 真近点角 θ
    ''' tan(θ/2) = √((1+e)/(1-e)) · tan(E/2)
    ''' </summary>
    Public Function EccentricToTrueAnomaly(Et As Double, e As Double) As Double
        Dim halfTheta As Double = Math.Atan2(
            Math.Sqrt(1.0 + e) * Math.Sin(Et / 2.0),
            Math.Sqrt(1.0 - e) * Math.Cos(Et / 2.0))
        Return 2.0 * halfTheta
    End Function

    ''' <summary>
    ''' 双曲近点角 H → 真近点角 θ（双曲线轨道）
    ''' tan(θ/2) = √((e+1)/(e-1)) · tanh(H/2)
    ''' </summary>
    Public Function HyperbolicToTrueAnomaly(H As Double, e As Double) As Double
        Dim halfTheta As Double = Math.Atan2(
            Math.Sqrt(e + 1.0) * Math.Sinh(H / 2.0),
            Math.Sqrt(e - 1.0) * Math.Cosh(H / 2.0))
        Return 2.0 * halfTheta
    End Function

    ' ==================== 轨道位置计算 ====================

    ''' <summary>
    ''' 根据轨道根数和时刻，计算天体的三维位置和速度
    ''' 这是整个轨道力学计算的核心函数
    ''' </summary>
    ''' <param name="elements">轨道根数</param>
    ''' <param name="timeYears">从历元起算的时间（年）</param>
    ''' <returns>天体的轨道状态（位置和速度）</returns>
    Public Function CalculateOrbitalState(elements As OrbitalElements,
                                           timeYears As Double) As OrbitalState
        Dim e As Double = elements.Eccentricity
        Dim a As Double = elements.SemiMajorAxis
        Dim i As Double = elements.Inclination
        Dim omega As Double = elements.LongitudeOfAscendingNode   ' Ω
        Dim w As Double = elements.ArgumentOfPerihelion           ' ω

        ' ---------- 第1步：计算平近点角 M ----------
        ' M(t) = M₀ + n·t，其中 n = 2π/T 为平均角速度
        Dim n As Double = 2.0 * Math.PI / elements.Period
        Dim M As Double = elements.MeanAnomalyAtEpoch + n * timeYears

        ' ---------- 第2步：求解偏近点角 E ----------
        Dim trueAnomaly As Double
        Dim r As Double

        If e < 1.0 Then
            ' 椭圆轨道
            Dim Ek As Double = SolveKeplerEquation(M, e)
            trueAnomaly = EccentricToTrueAnomaly(Ek, e)
            ' 轨道半径 r = a(1 - e·cos(E))
            r = a * (1.0 - e * Math.Cos(Ek))
        ElseIf e = 1.0 Then
            ' 抛物线轨道（近似处理）
            trueAnomaly = M
            r = a * 2.0 / (1.0 + Math.Cos(trueAnomaly))
        Else
            ' 双曲线轨道
            Dim H As Double = SolveKeplerHyperbolic(M, e)
            trueAnomaly = HyperbolicToTrueAnomaly(H, e)
            ' r = a(1 - e·cosh(H))，注意双曲线 a 为负
            r = Math.Abs(a) * (e * Math.Cosh(H) - 1.0)
        End If

        ' ---------- 第3步：计算轨道平面内的坐标 ----------
        ' 在轨道平面内，以焦点（中心天体）为原点
        Dim xOrbit As Double = r * Math.Cos(trueAnomaly)
        Dim yOrbit As Double = r * Math.Sin(trueAnomaly)

        ' ---------- 第4步：旋转到三维空间 ----------
        ' 依次绕三个轴旋转：ω（近日点幅角）、i（倾角）、Ω（升交点经度）
        '
        ' 旋转矩阵 R = Rz(-Ω) · Rx(-i) · Rz(-ω)
        '
        ' 展开后：
        '   X = (cosΩ·cosω - sinΩ·sinω·cosi)·xOrbit + (-cosΩ·sinω - sinΩ·cosω·cosi)·yOrbit
        '   Y = (sinΩ·cosω + cosΩ·sinω·cosi)·xOrbit + (-sinΩ·sinω + cosΩ·cosω·cosi)·yOrbit
        '   Z = (sinω·sini)·xOrbit + (cosω·sini)·yOrbit

        Dim cosW As Double = Math.Cos(w)
        Dim sinW As Double = Math.Sin(w)
        Dim cosI As Double = Math.Cos(i)
        Dim sinI As Double = Math.Sin(i)
        Dim cosO As Double = Math.Cos(omega)
        Dim sinO As Double = Math.Sin(omega)

        Dim X As Double = (cosO * cosW - sinO * sinW * cosI) * xOrbit +
                          (-cosO * sinW - sinO * cosW * cosI) * yOrbit
        Dim Y As Double = (sinO * cosW + cosO * sinW * cosI) * xOrbit +
                          (-sinO * sinW + cosO * cosW * cosI) * yOrbit
        Dim Z As Double = (sinW * sinI) * xOrbit +
                          (cosW * sinI) * yOrbit

        ' ---------- 第5步：计算速度 ----------
        ' 活力公式（vis-viva）：v² = GM(2/r - 1/a)
        Dim vSquared As Double
        If e < 1.0 Then
            vSquared = GM_SUN * (2.0 / r - 1.0 / a)
        Else
            vSquared = GM_SUN * (2.0 / r + 1.0 / Math.Abs(a))
        End If
        vSquared = Math.Max(0, vSquared)
        Dim v As Double = Math.Sqrt(vSquared)

        ' 速度方向：垂直于径向，在轨道平面内
        ' 简化处理：速度方向与位置方向垂直分量
        Dim flightPathAngle As Double
        If e < 1.0 Then
            ' 椭圆轨道飞行路径角
            flightPathAngle = Math.Atan2(e * Math.Sin(trueAnomaly),
                                          1.0 + e * Math.Cos(trueAnomaly))
        Else
            flightPathAngle = Math.Atan2(e * Math.Sin(trueAnomaly),
                                          1.0 + e * Math.Cos(trueAnomaly))
        End If

        ' 速度在轨道平面内的分量
        Dim vr As Double = v * Math.Sin(flightPathAngle)   ' 径向速度
        Dim vt As Double = v * Math.Cos(flightPathAngle)   ' 切向速度

        ' 将速度旋转到三维空间（与位置使用相同的旋转矩阵）
        Dim vxOrbit As Double = vr * Math.Cos(trueAnomaly) - vt * Math.Sin(trueAnomaly)
        Dim vyOrbit As Double = vr * Math.Sin(trueAnomaly) + vt * Math.Cos(trueAnomaly)

        Dim VX As Double = (cosO * cosW - sinO * sinW * cosI) * vxOrbit +
                           (-cosO * sinW - sinO * cosW * cosI) * vyOrbit
        Dim VY As Double = (sinO * cosW + cosO * sinW * cosI) * vxOrbit +
                           (-sinO * sinW + cosO * cosW * cosI) * vyOrbit
        Dim VZ As Double = (sinW * sinI) * vxOrbit +
                           (cosW * sinI) * vyOrbit

        Return New OrbitalState With {
            .X = X, .Y = Y, .Z = Z,
            .VX = VX, .VY = VY, .VZ = VZ
        }
    End Function

    ' ==================== 轨道路径计算（用于绘制轨道线） ====================

    ''' <summary>
    ''' 计算一个完整轨道的路径点（用于绘制轨道椭圆）
    ''' </summary>
    ''' <param name="elements">轨道根数</param>
    ''' <param name="numPoints">采样点数</param>
    ''' <returns>轨道上的点列表（X, Y 坐标，AU）</returns>
    Public Function ComputeOrbitalPath(elements As OrbitalElements,
                                        Optional numPoints As Integer = 360) As List(Of PointF)
        Dim points As New List(Of PointF)()

        If elements.IsElliptical Then
            ' 椭圆轨道：均匀采样真近点角
            For k As Integer = 0 To numPoints - 1
                Dim theta As Double = 2.0 * Math.PI * k / numPoints
                Dim r As Double = elements.SemiMajorAxis * (1.0 - elements.Eccentricity * elements.Eccentricity) /
                                  (1.0 + elements.Eccentricity * Math.Cos(theta))

                ' 轨道平面坐标
                Dim xOrb As Double = r * Math.Cos(theta)
                Dim yOrb As Double = r * Math.Sin(theta)

                ' 旋转到三维（只取 X, Y 用于二维显示）
                Dim cosW As Double = Math.Cos(elements.ArgumentOfPerihelion)
                Dim sinW As Double = Math.Sin(elements.ArgumentOfPerihelion)
                Dim cosI As Double = Math.Cos(elements.Inclination)
                Dim sinI As Double = Math.Sin(elements.Inclination)
                Dim cosO As Double = Math.Cos(elements.LongitudeOfAscendingNode)
                Dim sinO As Double = Math.Sin(elements.LongitudeOfAscendingNode)

                Dim X As Double = (cosO * cosW - sinO * sinW * cosI) * xOrb +
                                  (-cosO * sinW - sinO * cosW * cosI) * yOrb
                Dim Y As Double = (sinO * cosW + cosO * sinW * cosI) * xOrb +
                                  (-sinO * sinW + cosO * cosW * cosI) * yOrb

                points.Add(New PointF(CSng(X), CSng(Y)))
            Next
        ElseIf elements.IsHyperbolic Then
            ' 双曲线轨道：只绘制近日点附近的部分
            Dim maxTheta As Double = Math.Acos(-1.0 / elements.Eccentricity) * 0.95
            For k As Integer = 0 To numPoints - 1
                Dim theta As Double = -maxTheta + 2.0 * maxTheta * k / (numPoints - 1)
                Dim denom As Double = 1.0 + elements.Eccentricity * Math.Cos(theta)
                If denom <= 0.01 Then Continue For

                Dim r As Double = Math.Abs(elements.SemiMajorAxis) *
                                  (elements.Eccentricity * elements.Eccentricity - 1.0) / denom
                If r > 200 Then Continue For ' 限制绘制范围

                Dim xOrb As Double = r * Math.Cos(theta)
                Dim yOrb As Double = r * Math.Sin(theta)

                Dim cosW As Double = Math.Cos(elements.ArgumentOfPerihelion)
                Dim sinW As Double = Math.Sin(elements.ArgumentOfPerihelion)
                Dim cosI As Double = Math.Cos(elements.Inclination)
                Dim sinI As Double = Math.Sin(elements.Inclination)
                Dim cosO As Double = Math.Cos(elements.LongitudeOfAscendingNode)
                Dim sinO As Double = Math.Sin(elements.LongitudeOfAscendingNode)

                Dim X As Double = (cosO * cosW - sinO * sinW * cosI) * xOrb +
                                  (-cosO * sinW - sinO * cosW * cosI) * yOrb
                Dim Y As Double = (sinO * cosW + cosO * sinW * cosI) * xOrb +
                                  (-sinO * sinW + cosO * cosW * cosI) * yOrb

                points.Add(New PointF(CSng(X), CSng(Y)))
            Next
        End If

        Return points
    End Function

    ' ==================== 太阳银河系公转 ====================

    ''' <summary>
    ''' 计算太阳在银河系中的位置
    ''' 太阳绕银河系中心做近似圆周运动
    ''' </summary>
    ''' <param name="timeYears">从 J2000.0 起算的时间（年）</param>
    ''' <returns>太阳相对于银河系中心的位置（光年）</returns>
    Public Function CalculateSunGalacticPosition(timeYears As Double) As (X As Double, Y As Double)
        ' 银河系公转周期：225 百万年 = 225,000,000 年
        Dim galacticPeriod As Double = SUN_GALACTIC_PERIOD_MYR * 1000000.0
        ' 角速度
        Dim omega As Double = 2.0 * Math.PI / galacticPeriod
        ' 当前角度
        Dim angle As Double = omega * timeYears

        ' 简化为圆轨道
        Dim x As Double = SUN_GALACTIC_RADIUS_LY * Math.Cos(angle)
        Dim y As Double = SUN_GALACTIC_RADIUS_LY * Math.Sin(angle)

        Return (x, y)
    End Function

    ' ==================== 辅助函数 ====================

    ''' <summary>角度转弧度</summary>
    Public Function DegToRad(deg As Double) As Double
        Return deg * Math.PI / 180.0
    End Function

    ''' <summary>弧度转角度</summary>
    Public Function RadToDeg(rad As Double) As Double
        Return rad * 180.0 / Math.PI
    End Function

    ''' <summary>开普勒第三定律：由半长轴计算周期（年）</summary>
    ''' <param name="a">半长轴（AU）</param>
    Public Function CalculatePeriod(a As Double) As Double
        ' T² = a³（以 AU 和年为单位）
        Return Math.Sqrt(a * a * a)
    End Function

    ''' <summary>由半长轴和离心率计算轨道周长（近似，AU）</summary>
    Public Function CalculateOrbitalCircumference(a As Double, e As Double) As Double
        ' Ramanujan 近似公式
        Dim h As Double = ((a - a * (1 - e)) * (a - a * (1 - e)) +
                           (a * (1 + e) - a) * (a * (1 + e) - a)) /
                          ((a - a * (1 - e)) + (a * (1 + e) - a) + 0.0001)
        Return Math.PI * (3.0 * (a + a * (1 - e)) - Math.Sqrt((3.0 * a + a * (1 - e)) * (a + 3.0 * a * (1 - e))))
    End Function

End Module
