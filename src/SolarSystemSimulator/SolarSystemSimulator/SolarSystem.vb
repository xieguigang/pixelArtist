' ============================================================
' SolarSystem.vb - 太阳系模拟引擎
' ============================================================
' 管理所有天体的位置更新、彗星/小行星的随机生成
' ============================================================

Imports System.Collections.Generic

Namespace SolarSystemSimulator

    ''' <summary>
    ''' 太阳系模拟引擎
    ''' 负责管理所有天体、更新位置、生成随机天体
    ''' </summary>
    Public Class SolarSystemSimulation

        ' ==================== 天体集合 ====================

        ''' <summary>太阳</summary>
        Public Property Sun As CelestialBody

        ''' <summary>九大行星</summary>
        Public Property Planets As New List(Of CelestialBody)()

        ''' <summary>活跃的彗星列表</summary>
        Public Property Comets As New List(Of CometEntry)()

        ''' <summary>活跃的小行星列表</summary>
        Public Property Asteroids As New List(Of AsteroidEntry)()

        ''' <summary>所有天体的扁平列表（用于遍历渲染）</summary>
        Public ReadOnly Property AllBodies As List(Of CelestialBody)
            Get
                Dim list As New List(Of CelestialBody)()
                list.Add(Sun)
                For Each p In Planets
                    list.Add(p)
                    For Each m In p.Moons
                        list.Add(m)
                    Next
                Next
                For Each ce In Comets
                    list.Add(ce.Body)
                Next
                For Each ae In Asteroids
                    list.Add(ae.Body)
                Next
                Return list
            End Get
        End Property

        ''' <summary>太阳在银河系中的 X 坐标（光年）</summary>
        Public Property SunGalacticX As Double
            Get
                Return OrbitalMechanics.CalculateSunGalacticPosition(CurrentTime).X
            End Get
            Set(value As Double)
                _SunGalacticX = value
            End Set
        End Property

        ''' <summary>太阳在银河系中的 Y 坐标（光年）</summary>
        Public Property SunGalacticY As Double
            Get
                Return OrbitalMechanics.CalculateSunGalacticPosition(CurrentTime).Y
            End Get
            Set(value As Double)
                _SunGalacticY = value
            End Set
        End Property

        ' ==================== 时间管理 ====================

        ''' <summary>当前模拟时间（年，从 J2000.0 起算）</summary>
        Public Property CurrentTime As Double = 0.0

        ''' <summary>时间步长（年/帧）</summary>
        Public Property TimeStep As Double = 1.0 / 365.25  ' 默认每天一帧

        ''' <summary>时间加速倍率</summary>
        Public Property TimeScale As Double = 1.0

        ''' <summary>是否暂停</summary>
        Public Property Paused As Boolean = False

        ''' <summary>总帧数</summary>
        Public Property FrameCount As Long = 0

        ' ==================== 彗星/小行星管理 ====================

        ''' <summary>彗星/小行星的最大数量</summary>
        Public Property MaxComets As Integer = 5
        Public Property MaxAsteroids As Integer = 50

        ''' <summary>彗星生成间隔（帧数）</summary>
        Public Property CometSpawnInterval As Integer = 300

        ''' <summary>小行星生成间隔（帧数）</summary>
        Public Property AsteroidSpawnInterval As Integer = 30

        ''' <summary>彗星/小行星存活时间（帧数）</summary>
        Public Property CometLifetime As Integer = 2000
        Public Property AsteroidLifetime As Integer = 5000

        Private _cometTimer As Integer = 0
        Private _asteroidTimer As Integer = 0
        Private _rnd As New Random()

        ' ==================== 银河系视图数据 ====================

        ''' <summary>太阳在银河系中的当前位置（光年）</summary>
        Dim _SunGalacticX As Double = 0
        Dim _SunGalacticY As Double = 0

        ' ==================== 构造函数 ====================

        Public Sub New()
            Initialize()
        End Sub

        ''' <summary>重置模拟到初始状态</summary>
        Public Sub Reset()
            CurrentTime = 0
            FrameCount = 0
            Comets.Clear()
            Asteroids.Clear()
            Initialize()
        End Sub

        ''' <summary>初始化太阳系</summary>
        Public Sub Initialize()
            ' 创建太阳
            Sun = SolarSystemFactory.CreateSun()
            Sun.Position = New OrbitalState()

            ' 创建九大行星
            Planets = SolarSystemFactory.CreatePlanets()

            ' 预计算轨道路径
            For Each planet In Planets
                planet.UpdateOrbitPath()
                For Each moon In planet.Moons
                    moon.UpdateOrbitPath()
                Next
            Next

            ' 清空彗星和小行星
            Comets.Clear()
            Asteroids.Clear()

            ' 重置时间
            CurrentTime = 0.0
            FrameCount = 0
            _cometTimer = 0
            _asteroidTimer = 0

            ' 初始更新一次位置
            UpdatePositions()
        End Sub

        ' ==================== 核心更新 ====================

        ''' <summary>
        ''' 推进一帧 —— 更新所有天体位置（主循环调用）
        ''' </summary>
        Public Sub Update()
            StepForward()
        End Sub

        ''' <summary>
        ''' 推进一帧 —— 更新所有天体位置
        ''' </summary>
        Public Sub StepForward()
            If Paused Then Return

            ' 推进时间
            CurrentTime += TimeStep * TimeScale
            FrameCount += 1

            ' 更新所有天体位置
            UpdatePositions()

            ' 管理彗星和小行星
            ManageComets()
            ManageAsteroids()

            ' 更新太阳银河系位置
            Dim galPos = OrbitalMechanics.CalculateSunGalacticPosition(CurrentTime)
            SunGalacticX = galPos.X
            SunGalacticY = galPos.Y
        End Sub

        ''' <summary>更新所有天体的位置</summary>
        Private Sub UpdatePositions()
            ' 更新行星位置（相对于太阳）
            For Each planet In Planets
                If planet.Orbit IsNot Nothing Then
                    planet.Position = OrbitalMechanics.CalculateOrbitalState(planet.Orbit, CurrentTime)
                    planet.RecordTrail()

                    ' 更新卫星位置（相对于行星）
                    For Each moon In planet.Moons
                        If moon.Orbit IsNot Nothing Then
                            moon.Position = OrbitalMechanics.CalculateOrbitalState(moon.Orbit, CurrentTime)
                            moon.RecordTrail()
                        End If
                    Next
                End If
            Next

            ' 更新彗星位置
            For Each ce In Comets
                If ce.Body.Orbit IsNot Nothing Then
                    ce.Body.Position = OrbitalMechanics.CalculateOrbitalState(ce.Body.Orbit, CurrentTime)
                    ce.Body.RecordTrail()
                End If
            Next

            ' 更新小行星位置
            For Each ae In Asteroids
                If ae.Body.Orbit IsNot Nothing Then
                    ae.Body.Position = OrbitalMechanics.CalculateOrbitalState(ae.Body.Orbit, CurrentTime)
                    ae.Body.RecordTrail()
                End If
            Next
        End Sub

        ' ==================== 彗星管理 ====================

        Private Sub ManageComets()
            _cometTimer += 1

            ' 定期生成新彗星
            If _cometTimer >= CometSpawnInterval AndAlso Comets.Count < MaxComets Then
                Dim comet As CelestialBody = SolarSystemFactory.CreateRandomComet()
                comet.UpdateOrbitPath()
                Comets.Add(New CometEntry With {
                    .Body = comet,
                    .RemainingLife = CometLifetime
                })
                _cometTimer = 0
            End If

            ' 更新存活时间，移除过期彗星
            For i As Integer = Comets.Count - 1 To 0 Step -1
                Comets(i).RemainingLife -= 1
                If Comets(i).RemainingLife <= 0 Then
                    Comets.RemoveAt(i)
                End If
            Next
        End Sub

        ' ==================== 小行星管理 ====================

        Private Sub ManageAsteroids()
            _asteroidTimer += 1

            ' 定期生成新小行星
            If _asteroidTimer >= AsteroidSpawnInterval AndAlso Asteroids.Count < MaxAsteroids Then
                Dim asteroid As CelestialBody = SolarSystemFactory.CreateRandomAsteroid()
                asteroid.UpdateOrbitPath()
                Asteroids.Add(New AsteroidEntry With {
                    .Body = asteroid,
                    .RemainingLife = AsteroidLifetime
                })
                _asteroidTimer = 0
            End If

            ' 更新存活时间，移除过期小行星
            For i As Integer = Asteroids.Count - 1 To 0 Step -1
                Asteroids(i).RemainingLife -= 1
                If Asteroids(i).RemainingLife <= 0 Then
                    Asteroids.RemoveAt(i)
                End If
            Next
        End Sub

        ' ==================== 手动生成天体 ====================

        ''' <summary>手动生成一颗彗星</summary>
        Public Sub GenerateComet()
            Dim comet As CelestialBody = SolarSystemFactory.CreateRandomComet()
            comet.UpdateOrbitPath()
            Comets.Add(New CometEntry With {
                .Body = comet,
                .RemainingLife = CometLifetime
            })
        End Sub

        ''' <summary>手动生成一颗小行星</summary>
        Public Sub GenerateAsteroid()
            Dim asteroid As CelestialBody = SolarSystemFactory.CreateRandomAsteroid()
            asteroid.UpdateOrbitPath()
            Asteroids.Add(New AsteroidEntry With {
                .Body = asteroid,
                .RemainingLife = AsteroidLifetime
            })
        End Sub

        ' ==================== 时间控制 ====================

        ''' <summary>设置时间步长（年/帧）</summary>
        Public Sub SetTimeStep(daysPerFrame As Double)
            TimeStep = daysPerFrame / 365.25
        End Sub

        ''' <summary>获取当前日期字符串</summary>
        Public Function GetCurrentDateString() As String
            ' J2000.0 = 2000年1月1日 12:00 TT
            Dim baseDate As New DateTime(2000, 1, 1, 12, 0, 0)
            Dim currentDate As DateTime = baseDate.AddDays(CurrentTime * 365.25)
            Return currentDate.ToString("yyyy年MM月dd日")
        End Function

        ''' <summary>获取模拟信息摘要</summary>
        Public Function GetSimulationInfo() As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("=== 太阳系模拟信息 ===")
            sb.AppendLine(String.Format("模拟时间: {0}", GetCurrentDateString()))
            sb.AppendLine(String.Format("经过时间: {0:F2} 年", CurrentTime))
            sb.AppendLine(String.Format("时间步长: {0:F4} 年/帧", TimeStep * TimeScale))
            sb.AppendLine(String.Format("时间倍率: {0:F1}x", TimeScale))
            sb.AppendLine(String.Format("帧数: {0}", FrameCount))
            sb.AppendLine(String.Format("行星数: {0}", Planets.Count))
            sb.AppendLine(String.Format("活跃彗星: {0}/{1}", Comets.Count, MaxComets))
            sb.AppendLine(String.Format("活跃小行星: {0}/{1}", Asteroids.Count, MaxAsteroids))
            Dim totalMoons As Integer = 0
            For Each p In Planets
                totalMoons += p.Moons.Count
            Next
            sb.AppendLine(String.Format("卫星总数: {0}", totalMoons))
            Return sb.ToString()
        End Function

    End Class

    ''' <summary>彗星条目（含存活时间）</summary>
    Public Class CometEntry
        Public Property Body As CelestialBody
        Public Property RemainingLife As Integer = 2000
    End Class

    ''' <summary>小行星条目（含存活时间）</summary>
    Public Class AsteroidEntry
        Public Property Body As CelestialBody
        Public Property RemainingLife As Integer = 5000
    End Class

End Namespace
