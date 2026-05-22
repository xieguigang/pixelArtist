' ============================================================
' CelestialBodies.vb - 天体类定义 + 太阳系真实天文数据
' ============================================================
' 包含所有天体的类定义，以及九大行星及其主要卫星的真实轨道数据。
' 数据来源：NASA JPL 行星事实表
' ============================================================

Imports System.Collections.Generic
Imports System.Drawing

Namespace SolarSystemSimulator

    ' ==================== 天体类型枚举 ====================

    Public Enum BodyType
        Star
        Planet
        Moon
        Comet
        Asteroid
    End Enum

    ' ==================== 天体基类 ====================

    ''' <summary>
    ''' 天体基类 —— 所有天体（恒星、行星、卫星、彗星、小行星）的公共属性
    ''' </summary>
    Public Class CelestialBody

        ''' <summary>名称</summary>
        Public Property Name As String = ""

        ''' <summary>英文名</summary>
        Public Property NameEn As String = ""

        ''' <summary>天体类型</summary>
        Public Property Type As BodyType = BodyType.Planet

        ''' <summary>质量（千克）</summary>
        Public Property MassKg As Double = 0

        ''' <summary>实际半径（千米）</summary>
        Public Property RadiusKm As Double = 0

        ''' <summary>轨道根数</summary>
        Public Property Orbit As OrbitalElements = Nothing

        ''' <summary>显示颜色</summary>
        Public Property Color As Color = Color.White

        ''' <summary>当前三维位置（AU，相对于轨道中心天体）</summary>
        Public Property Position As OrbitalState

        ''' <summary>卫星列表（仅行星有）</summary>
        Public Property Moons As New List(Of CelestialBody)()

        ''' <summary>父天体（仅卫星有）</summary>
        Public Property Parent As CelestialBody = Nothing

        ''' <summary>轨道路径缓存（用于绘制轨道线）</summary>
        Public Property OrbitPath As List(Of PointF) = Nothing

        ''' <summary>运动轨迹（最近的位置记录，用于绘制尾迹）</summary>
        Public Property Trail As New List(Of PointF)()
        Public Property MaxTrailLength As Integer = 800

        ''' <summary>是否显示</summary>
        Public Property Visible As Boolean = True

        ''' <summary>是否显示轨道</summary>
        Public Property ShowOrbit As Boolean = True

        ''' <summary>是否显示名称标签</summary>
        Public Property ShowLabel As Boolean = True

        ''' <summary>显示大小倍率（用于调整视觉效果）</summary>
        Public Property DisplayScale As Double = 1.0

        ''' <summary>描述信息</summary>
        Public Property Description As String = ""

        ''' <summary>绝对位置（AU，相对于太阳）—— 卫星需要加上父天体位置</summary>
        Public ReadOnly Property AbsolutePosition As OrbitalState
            Get
                If Parent IsNot Nothing Then
                    Return New OrbitalState With {
                        .X = Parent.Position.X + Position.X,
                        .Y = Parent.Position.Y + Position.Y,
                        .Z = Parent.Position.Z + Position.Z,
                        .VX = Parent.Position.VX + Position.VX,
                        .VY = Parent.Position.VY + Position.VY,
                        .VZ = Parent.Position.VZ + Position.VZ
                    }
                Else
                    Return Position
                End If
            End Get
        End Property

        Public Property Period As Double

        ''' <summary>更新轨道路径缓存</summary>
        Public Sub UpdateOrbitPath()
            If Orbit IsNot Nothing Then
                OrbitPath = OrbitalMechanics.ComputeOrbitalPath(Orbit)
            End If
        End Sub

        ''' <summary>记录当前位置到轨迹</summary>
        Public Sub RecordTrail()
            Dim absPos As OrbitalState = AbsolutePosition
            Trail.Add(New PointF(CSng(absPos.X), CSng(absPos.Y)))
            If Trail.Count > MaxTrailLength Then
                Trail.RemoveAt(0)
            End If
        End Sub

        ''' <summary>清除轨迹</summary>
        Public Sub ClearTrail()
            Trail.Clear()
        End Sub

        ''' <summary>获取信息摘要</summary>
        Public Function GetInfo() As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("【" & Name & "】")
            If NameEn <> "" Then sb.AppendLine("  英文名: " & NameEn)
            sb.AppendLine("  类型: " & Type.ToString())
            If MassKg > 0 Then sb.AppendLine(String.Format("  质量: {0:E3} kg", MassKg))
            If RadiusKm > 0 Then sb.AppendLine(String.Format("  半径: {0:F0} km", RadiusKm))
            If Orbit IsNot Nothing Then
                sb.AppendLine(String.Format("  半长轴: {0:F3} AU", Orbit.SemiMajorAxis))
                sb.AppendLine(String.Format("  离心率: {0:F4}", Orbit.Eccentricity))
                sb.AppendLine(String.Format("  倾角: {0:F2}°", OrbitalMechanics.RadToDeg(Orbit.Inclination)))
                sb.AppendLine(String.Format("  周期: {0:F3} 年", Orbit.Period))
                sb.AppendLine(String.Format("  距中心: {0:F3} AU", Position.Distance))
            End If
            Return sb.ToString()
        End Function

    End Class

    ' ==================== 太阳系数据工厂 ====================

    ''' <summary>
    ''' 创建完整的太阳系模型，包含真实天文数据
    ''' 所有轨道数据基于 J2000.0 历元
    ''' </summary>
    Public Module SolarSystemFactory

        ''' <summary>创建太阳</summary>
        Public Function CreateSun() As CelestialBody
            Dim sun As New CelestialBody()
            sun.Name = "太阳"
            sun.NameEn = "Sun"
            sun.Type = BodyType.Star
            sun.MassKg = 1.989E+30
            sun.RadiusKm = 696340
            sun.Color = Color.FromArgb(255, 255, 200, 50)
            sun.DisplayScale = 3.0
            sun.Description = "太阳系的中心恒星，G2V型主序星"
            sun.Position = New OrbitalState()
            Return sun
        End Function

        ''' <summary>创建九大行星及其卫星</summary>
        Public Function CreatePlanets() As List(Of CelestialBody)
            Dim planets As New List(Of CelestialBody)()

            ' ========== 水星 Mercury ==========
            Dim mercury As New CelestialBody()
            mercury.Name = "水星"
            mercury.NameEn = "Mercury"
            mercury.Type = BodyType.Planet
            mercury.MassKg = 3.301E+23
            mercury.RadiusKm = 2439.7
            mercury.Color = Color.FromArgb(200, 180, 180)
            mercury.DisplayScale = 0.6
            mercury.Description = "距太阳最近的行星，无卫星"
            mercury.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 0.38710,
                .Eccentricity = 0.20563,
                .Inclination = OrbitalMechanics.DegToRad(7.005),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(48.331),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(29.124),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(174.796),
                .Period = 0.24085,
                .BodyRadiusKm = 2439.7
            }
            planets.Add(mercury)

            ' ========== 金星 Venus ==========
            Dim venus As New CelestialBody()
            venus.Name = "金星"
            venus.NameEn = "Venus"
            venus.Type = BodyType.Planet
            venus.MassKg = 4.867E+24
            venus.RadiusKm = 6051.8
            venus.Color = Color.FromArgb(230, 200, 150)
            venus.DisplayScale = 0.9
            venus.Description = "太阳系最热的行星，无卫星"
            venus.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 0.72333,
                .Eccentricity = 0.00677,
                .Inclination = OrbitalMechanics.DegToRad(3.395),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(76.680),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(54.884),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(50.115),
                .Period = 0.61520,
                .BodyRadiusKm = 6051.8
            }
            planets.Add(venus)

            ' ========== 地球 Earth ==========
            Dim earth As New CelestialBody()
            earth.Name = "地球"
            earth.NameEn = "Earth"
            earth.Type = BodyType.Planet
            earth.MassKg = 5.972E+24
            earth.RadiusKm = 6371.0
            earth.Color = Color.FromArgb(70, 130, 230)
            earth.DisplayScale = 0.9
            earth.Description = "我们的家园，拥有1颗天然卫星"
            earth.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 1.00000,
                .Eccentricity = 0.01671,
                .Inclination = OrbitalMechanics.DegToRad(0.0),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(-11.261),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(114.208),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(358.617),
                .Period = 1.00000,
                .BodyRadiusKm = 6371.0
            }
            ' 地球的卫星：月球
            earth.Moons.Add(CreateMoon("月球", "Moon", 7.342E+22, 1737.4,
                                       0.00257, 0.0549, 5.145, 27.322, Color.FromArgb(200, 200, 200), earth))
            planets.Add(earth)

            ' ========== 火星 Mars ==========
            Dim mars As New CelestialBody()
            mars.Name = "火星"
            mars.NameEn = "Mars"
            mars.Type = BodyType.Planet
            mars.MassKg = 6.417E+23
            mars.RadiusKm = 3389.5
            mars.Color = Color.FromArgb(193, 68, 14)
            mars.DisplayScale = 0.7
            mars.Description = "红色星球，拥有2颗卫星"
            mars.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 1.52368,
                .Eccentricity = 0.09341,
                .Inclination = OrbitalMechanics.DegToRad(1.850),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(49.558),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(286.502),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(19.373),
                .Period = 1.88082,
                .BodyRadiusKm = 3389.5
            }
            ' 火星的卫星
            mars.Moons.Add(CreateMoon("火卫一", "Phobos", 1.0659E+16, 11.267,
                                      0.0000627, 0.0151, 1.093, 0.3189, Color.FromArgb(160, 140, 120), mars))
            mars.Moons.Add(CreateMoon("火卫二", "Deimos", 1.4762E+15, 6.2,
                                      0.000157, 0.0002, 0.93, 1.263, Color.FromArgb(150, 130, 110), mars))
            planets.Add(mars)

            ' ========== 木星 Jupiter ==========
            Dim jupiter As New CelestialBody()
            jupiter.Name = "木星"
            jupiter.NameEn = "Jupiter"
            jupiter.Type = BodyType.Planet
            jupiter.MassKg = 1.898E+27
            jupiter.RadiusKm = 69911
            jupiter.Color = Color.FromArgb(200, 139, 58)
            jupiter.DisplayScale = 1.8
            jupiter.Description = "太阳系最大的行星，拥有95颗已知卫星"
            jupiter.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 5.20260,
                .Eccentricity = 0.04849,
                .Inclination = OrbitalMechanics.DegToRad(1.303),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(100.464),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(273.867),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(20.020),
                .Period = 11.8620,
                .BodyRadiusKm = 69911
            }
            ' 木星的四大伽利略卫星
            jupiter.Moons.Add(CreateMoon("木卫一", "Io", 8.932E+22, 1821.6,
                                         0.00282, 0.0041, 0.05, 0.00485, Color.FromArgb(230, 210, 80), jupiter))
            jupiter.Moons.Add(CreateMoon("木卫二", "Europa", 4.800E+22, 1560.8,
                                         0.00449, 0.009, 0.47, 0.00972, Color.FromArgb(180, 170, 150), jupiter))
            jupiter.Moons.Add(CreateMoon("木卫三", "Ganymede", 1.482E+23, 2634.1,
                                         0.00716, 0.0013, 0.21, 0.01959, Color.FromArgb(170, 160, 140), jupiter))
            jupiter.Moons.Add(CreateMoon("木卫四", "Callisto", 1.076E+23, 2410.3,
                                         0.01259, 0.0074, 0.19, 0.04570, Color.FromArgb(130, 120, 110), jupiter))
            planets.Add(jupiter)

            ' ========== 土星 Saturn ==========
            Dim saturn As New CelestialBody()
            saturn.Name = "土星"
            saturn.NameEn = "Saturn"
            saturn.Type = BodyType.Planet
            saturn.MassKg = 5.683E+26
            saturn.RadiusKm = 58232
            saturn.Color = Color.FromArgb(232, 209, 145)
            saturn.DisplayScale = 1.6
            saturn.Description = "以壮观光环闻名的行星，拥有146颗已知卫星"
            saturn.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 9.55491,
                .Eccentricity = 0.05551,
                .Inclination = OrbitalMechanics.DegToRad(2.489),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(113.665),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(339.392),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(317.020),
                .Period = 29.4571,
                .BodyRadiusKm = 58232
            }
            ' 土星的主要卫星
            saturn.Moons.Add(CreateMoon("土卫一", "Mimas", 3.749E+19, 198.2,
                                        0.00124, 0.0196, 1.53, 0.000942, Color.FromArgb(180, 180, 180), saturn))
            saturn.Moons.Add(CreateMoon("土卫二", "Enceladus", 1.080E+20, 252.1,
                                        0.00159, 0.0047, 0.02, 0.000375, Color.FromArgb(220, 230, 240), saturn))
            saturn.Moons.Add(CreateMoon("土卫三", "Tethys", 6.174E+20, 531.1,
                                        0.00197, 0.0001, 1.12, 0.000517, Color.FromArgb(200, 200, 210), saturn))
            saturn.Moons.Add(CreateMoon("土卫四", "Dione", 1.095E+21, 561.4,
                                        0.00252, 0.0022, 0.02, 0.000749, Color.FromArgb(190, 190, 200), saturn))
            saturn.Moons.Add(CreateMoon("土卫五", "Rhea", 2.307E+21, 763.8,
                                        0.00352, 0.001, 0.35, 0.001237, Color.FromArgb(180, 180, 190), saturn))
            saturn.Moons.Add(CreateMoon("土卫六", "Titan", 1.345E+23, 2574.7,
                                        0.00817, 0.0288, 0.34, 0.004366, Color.FromArgb(210, 180, 100), saturn))
            saturn.Moons.Add(CreateMoon("土卫八", "Iapetus", 1.806E+21, 734.5,
                                        0.02380, 0.0286, 7.57, 0.002173, Color.FromArgb(160, 150, 140), saturn))
            planets.Add(saturn)

            ' ========== 天王星 Uranus ==========
            Dim uranus As New CelestialBody()
            uranus.Name = "天王星"
            uranus.NameEn = "Uranus"
            uranus.Type = BodyType.Planet
            uranus.MassKg = 8.681E+25
            uranus.RadiusKm = 25362
            uranus.Color = Color.FromArgb(115, 194, 208)
            uranus.DisplayScale = 1.3
            uranus.Description = "侧躺旋转的冰巨星，拥有27颗已知卫星"
            uranus.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 19.2184,
                .Eccentricity = 0.04638,
                .Inclination = OrbitalMechanics.DegToRad(0.773),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(74.006),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(96.998),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(142.238),
                .Period = 84.0205,
                .BodyRadiusKm = 25362
            }
            ' 天王星的主要卫星
            uranus.Moons.Add(CreateMoon("天卫三", "Miranda", 6.59E+19, 235.8,
                                        0.000865, 0.0013, 4.34, 0.000387, Color.FromArgb(170, 170, 180), uranus))
            uranus.Moons.Add(CreateMoon("天卫一", "Ariel", 1.35E+21, 578.9,
                                        0.00127, 0.0012, 0.26, 0.000690, Color.FromArgb(190, 195, 200), uranus))
            uranus.Moons.Add(CreateMoon("天卫二", "Umbriel", 1.17E+21, 584.7,
                                        0.00178, 0.0039, 0.13, 0.001135, Color.FromArgb(140, 140, 150), uranus))
            uranus.Moons.Add(CreateMoon("天卫四", "Titania", 3.53E+21, 788.4,
                                        0.00291, 0.0011, 0.08, 0.002385, Color.FromArgb(180, 180, 190), uranus))
            uranus.Moons.Add(CreateMoon("天卫五", "Oberon", 3.01E+21, 761.4,
                                        0.00390, 0.0014, 0.07, 0.003688, Color.FromArgb(170, 165, 160), uranus))
            planets.Add(uranus)

            ' ========== 海王星 Neptune ==========
            Dim neptune As New CelestialBody()
            neptune.Name = "海王星"
            neptune.NameEn = "Neptune"
            neptune.Type = BodyType.Planet
            neptune.MassKg = 1.024E+26
            neptune.RadiusKm = 24622
            neptune.Color = Color.FromArgb(63, 84, 186)
            neptune.DisplayScale = 1.3
            neptune.Description = "太阳系最远的行星，拥有16颗已知卫星"
            neptune.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 30.1104,
                .Eccentricity = 0.00859,
                .Inclination = OrbitalMechanics.DegToRad(1.770),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(131.784),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(276.336),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(256.228),
                .Period = 164.800,
                .BodyRadiusKm = 24622
            }
            ' 海王星的主要卫星
            neptune.Moons.Add(CreateMoon("海卫一", "Triton", 2.14E+22, 1353.4,
                                         0.00237, 0.000016, 156.885, 0.0000161, Color.FromArgb(180, 200, 220), neptune))
            planets.Add(neptune)

            ' ========== 冥王星 Pluto（第九大行星） ==========
            Dim pluto As New CelestialBody()
            pluto.Name = "冥王星"
            pluto.NameEn = "Pluto"
            pluto.Type = BodyType.Planet
            pluto.MassKg = 1.303E+22
            pluto.RadiusKm = 1188.3
            pluto.Color = Color.FromArgb(210, 180, 140)
            pluto.DisplayScale = 0.5
            pluto.Description = "曾经的第九大行星，2006年重新分类为矮行星"
            pluto.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = 39.4821,
                .Eccentricity = 0.24882,
                .Inclination = OrbitalMechanics.DegToRad(17.16),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(110.299),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(113.834),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(14.53),
                .Period = 247.940,
                .BodyRadiusKm = 1188.3
            }
            ' 冥王星的卫星
            pluto.Moons.Add(CreateMoon("冥卫一", "Charon", 1.586E+21, 606.0,
                                       0.000130, 0.0022, 0.08, 0.0000175, Color.FromArgb(170, 160, 150), pluto))
            planets.Add(pluto)

            Return planets
        End Function

        ''' <summary>
        ''' 创建卫星的辅助方法
        ''' </summary>
        ''' <param name="name">中文名</param>
        ''' <param name="nameEn">英文名</param>
        ''' <param name="mass">质量（kg）</param>
        ''' <param name="radius">半径（km）</param>
        ''' <param name="semiMajorAxisAU">轨道半长轴（AU）</param>
        ''' <param name="ecc">离心率</param>
        ''' <param name="inclDeg">轨道倾角（度）</param>
        ''' <param name="periodYears">轨道周期（年）</param>
        ''' <param name="color">显示颜色</param>
        ''' <param name="parent">父天体</param>
        Private Function CreateMoon(name As String, nameEn As String, mass As Double,
                                    radius As Double, semiMajorAxisAU As Double,
                                    ecc As Double, inclDeg As Double, periodYears As Double,
                                    color As Color, parent As CelestialBody) As CelestialBody
            Dim moon As New CelestialBody()
            moon.Name = name
            moon.NameEn = nameEn
            moon.Type = BodyType.Moon
            moon.MassKg = mass
            moon.RadiusKm = radius
            moon.Color = color
            moon.DisplayScale = 0.4
            moon.Parent = parent
            moon.ShowLabel = False
            moon.MaxTrailLength = 200

            moon.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = semiMajorAxisAU,
                .Eccentricity = ecc,
                .Inclination = OrbitalMechanics.DegToRad(inclDeg),
                .LongitudeOfAscendingNode = 0,
                .ArgumentOfPerihelion = 0,
                .MeanAnomalyAtEpoch = 0,
                .Period = Math.Max(periodYears, 0.0001),
                .BodyRadiusKm = radius
            }

            Return moon
        End Function

        ''' <summary>
        ''' 随机生成一颗彗星
        ''' 彗星通常具有高离心率的椭圆轨道或双曲线轨道
        ''' </summary>
        Public Function CreateRandomComet(Optional seed As Integer = -1) As CelestialBody
            Dim rnd As New Random()
            If seed >= 0 Then rnd = New Random(seed)

            Dim comet As New CelestialBody()
            comet.Type = BodyType.Comet

            ' 随机彗星名称
            Dim cometNames As String() = {"哈雷", "海尔-波普", "百武", "麦克诺特", "洛夫乔伊",
                                           "艾森", "恩克", "坦普尔", "威斯塔", "池谷-张",
                                           "NEOWISE", "泽尔纳", "博雷利", "斯威夫特-塔特尔",
                                           "阿兰德-罗兰", "池谷-张", "关-莱恩斯", "德阿雷斯特"}
            Dim idx As Integer = rnd.Next(cometNames.Length)
            comet.Name = "彗星 " & cometNames(idx)
            comet.NameEn = "Comet " & cometNames(idx)

            ' 彗星轨道参数：高离心率
            Dim e As Double = 0.85 + rnd.NextDouble() * 0.6  ' 0.85 ~ 1.45
            Dim a As Double
            If e < 1.0 Then
                ' 椭圆轨道
                a = 5.0 + rnd.NextDouble() * 200.0  ' 5 ~ 205 AU
                comet.Period = OrbitalMechanics.CalculatePeriod(a)
            Else
                ' 双曲线轨道
                a = -(10.0 + rnd.NextDouble() * 100.0)  ' 负的半长轴
                comet.Period = 999.0  ' 双曲线轨道无周期
            End If

            comet.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = a,
                .Eccentricity = e,
                .Inclination = OrbitalMechanics.DegToRad(rnd.NextDouble() * 180),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(rnd.NextDouble() * 360),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(rnd.NextDouble() * 360),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(rnd.NextDouble() * 360),
                .Period = If(e < 1.0, comet.Period, 999.0),
                .BodyRadiusKm = 1.0 + rnd.NextDouble() * 10.0
            }

            comet.Color = Color.FromArgb(150 + CInt(rnd.NextDouble() * 105),
                                          200 + CInt(rnd.NextDouble() * 55),
                                          255)
            comet.DisplayScale = 0.5
            comet.MaxTrailLength = 1500
            comet.Description = String.Format("随机生成的彗星，e={0:F3}, a={1:F1}AU", e, a)

            Return comet
        End Function

        ''' <summary>
        ''' 随机生成一颗小行星
        ''' 小行星主要分布在火星和木星之间的小行星带（2.2~3.2 AU）
        ''' </summary>
        Public Function CreateRandomAsteroid(Optional seed As Integer = -1) As CelestialBody
            Dim rnd As New Random()
            If seed >= 0 Then rnd = New Random(seed)

            Dim asteroid As New CelestialBody()
            asteroid.Type = BodyType.Asteroid

            ' 小行星带分布
            Dim a As Double = 2.2 + rnd.NextDouble() * 1.0  ' 2.2 ~ 3.2 AU
            Dim e As Double = rnd.NextDouble() * 0.3        ' 0 ~ 0.3
            Dim incl As Double = rnd.NextDouble() * 20.0    ' 0 ~ 20°

            asteroid.Orbit = New OrbitalElements() With {
                .SemiMajorAxis = a,
                .Eccentricity = e,
                .Inclination = OrbitalMechanics.DegToRad(incl),
                .LongitudeOfAscendingNode = OrbitalMechanics.DegToRad(rnd.NextDouble() * 360),
                .ArgumentOfPerihelion = OrbitalMechanics.DegToRad(rnd.NextDouble() * 360),
                .MeanAnomalyAtEpoch = OrbitalMechanics.DegToRad(rnd.NextDouble() * 360),
                .Period = OrbitalMechanics.CalculatePeriod(a),
                .BodyRadiusKm = 0.5 + rnd.NextDouble() * 500.0
            }

            asteroid.Color = Color.FromArgb(140 + CInt(rnd.NextDouble() * 60),
                                             130 + CInt(rnd.NextDouble() * 50),
                                             120 + CInt(rnd.NextDouble() * 40))
            asteroid.DisplayScale = 0.3
            asteroid.MaxTrailLength = 400
            asteroid.ShowLabel = False
            asteroid.Name = "小行星 #" & rnd.Next(1000, 9999)
            asteroid.NameEn = "Asteroid #" & rnd.Next(1000, 9999)
            asteroid.Description = String.Format("小行星带中的小行星，a={0:F2}AU", a)

            Return asteroid
        End Function

    End Module

End Namespace
