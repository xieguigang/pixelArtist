' ============================================================
' CelestialBodies.vb - 天体类定义 + 太阳系真实天文数据
' ============================================================
' 包含所有天体的类定义，以及九大行星及其主要卫星的真实轨道数据。
' 数据来源：NASA JPL 行星事实表
' ============================================================

Imports System.Collections.Generic
Imports System.Drawing

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

