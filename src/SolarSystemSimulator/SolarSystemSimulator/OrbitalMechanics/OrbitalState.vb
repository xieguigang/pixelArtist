''' <summary>
''' 三维空间中的位置和速度
''' </summary>
Public Structure OrbitalState

    ''' <summary>位置 X（AU）</summary>
    Public X As Double

    ''' <summary>位置 Y（AU）</summary>
    Public Y As Double

    ''' <summary>位置 Z（AU）</summary>
    Public Z As Double

    ''' <summary>速度 VX（AU/年）</summary>
    Public VX As Double

    ''' <summary>速度 VY（AU/年）</summary>
    Public VY As Double

    ''' <summary>速度 VZ（AU/年）</summary>
    Public VZ As Double

    ''' <summary>到中心天体的距离（AU）</summary>
    Public ReadOnly Property Distance As Double
        Get
            Return Math.Sqrt(X * X + Y * Y + Z * Z)
        End Get
    End Property

    ''' <summary>速度大小（AU/年）</summary>
    Public ReadOnly Property Speed As Double
        Get
            Return Math.Sqrt(VX * VX + VY * VY + VZ * VZ)
        End Get
    End Property

End Structure

