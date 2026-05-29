' ============================================================
' OrbitalMechanics.vb - 轨道力学计算引擎
' ============================================================
' 基于开普勒定律和牛顿万有引力定律，使用纯 VB.NET 数学函数库
' 实现所有天体的轨道位置和速度计算。
'
' 核心算法：
'   1. 开普勒方程求解（牛顿迭代法）：M = E - e·sin(E)
'   2. 偏近点角 → 真近点角转换
'   3. 轨道平面坐标 → 三维空间坐标旋转
'   4. 双曲线轨道求解（用于彗星）
' ============================================================

''' <summary>
''' 轨道根数 —— 描述一个天体轨道的六个经典参数
''' </summary>
Public Class OrbitalElements

    ''' <summary>半长轴 a（AU）—— 椭圆长轴的一半</summary>
    Public Property SemiMajorAxis As Double

    ''' <summary>离心率 e（0=圆，0&lt;e&lt;1=椭圆，e=1=抛物线，e&gt;1=双曲线）</summary>
    Public Property Eccentricity As Double

    ''' <summary>轨道倾角 i（弧度）—— 轨道平面与参考平面的夹角</summary>
    Public Property Inclination As Double

    ''' <summary>升交点经度 Ω（弧度）—— 升交点方向在参考平面上的角度</summary>
    Public Property LongitudeOfAscendingNode As Double

    ''' <summary>近日点幅角 ω（弧度）—— 从升交点到近日点的角度</summary>
    Public Property ArgumentOfPerihelion As Double

    ''' <summary>历元时刻的平近点角 M₀（弧度）</summary>
    Public Property MeanAnomalyAtEpoch As Double

    ''' <summary>轨道周期 T（年）</summary>
    Public Property Period As Double

    ''' <summary>天体实际半径（千米），用于绘制</summary>
    Public Property BodyRadiusKm As Double = 0

    ''' <summary>历元时刻（年），默认 J2000.0</summary>
    Public Property Epoch As Double = 2000.0

    ''' <summary>计算半短轴 b = a·√(1-e²)</summary>
    Public ReadOnly Property SemiMinorAxis As Double
        Get
            Return SemiMajorAxis * Math.Sqrt(Math.Max(0, 1 - Eccentricity * Eccentricity))
        End Get
    End Property

    ''' <summary>计算近拱点距离 r_p = a(1-e)</summary>
    Public ReadOnly Property PeriapsisDistance As Double
        Get
            Return SemiMajorAxis * (1 - Eccentricity)
        End Get
    End Property

    ''' <summary>计算远拱点距离 r_a = a(1+e)</summary>
    Public ReadOnly Property ApoapsisDistance As Double
        Get
            Return SemiMajorAxis * (1 + Eccentricity)
        End Get
    End Property

    ''' <summary>是否为椭圆轨道</summary>
    Public ReadOnly Property IsElliptical As Boolean
        Get
            Return Eccentricity >= 0 AndAlso Eccentricity < 1
        End Get
    End Property

    ''' <summary>是否为双曲线轨道</summary>
    Public ReadOnly Property IsHyperbolic As Boolean
        Get
            Return Eccentricity > 1
        End Get
    End Property

End Class

