Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D
Imports pColor = Astrophysics.raytracing.pixeldata.Color



    ''' <summary>
    ''' Thin Shakura-Sunyaev style accretion disk in the equatorial (XZ) plane. Provides the
    ''' blackbody colour (temperature falling as r^-3/4), the relativistic orbital gas velocity
    ''' (Kerr frame-dragging included) and the doppler-shifted emission at a disk crossing.
    ''' </summary>
    Public Class AccretionDisk

        Private Const M As Double = 0.5   ' Rs = 2M = 1

        ''' <summary>3-velocity (beta, geometric units) of the gas at a disk crossing.</summary>
        Public Shared Function MaterialVelocity(hit As DiskHit, model As BlackHoleModel) As vec3
            Dim rad = System.Math.Max(hit.DiskRadius, 1.001)
            Dim aGeom = model.Spin * M
            ' Bardeen circular-orbit angular velocity (prograde).
            Dim Omega = System.Math.Sqrt(M) / (System.Math.Pow(rad, 1.5) + aGeom * System.Math.Sqrt(M))
            Dim v = Omega * rad
            If v >= 1.0 Then v = 0.999
            ' Prograde rotation about +Y: v = (0, Omega, 0) x (x,0,z) = (Omega*z, 0, -Omega*x)
            Dim x = hit.Position.X
            Dim z = hit.Position.Z
            Return New vec3(Omega * z, 0, -Omega * x).Normalize().Multiply(v)
        End Function

        ''' <summary>Compute the doppler-shifted, gravitationally-redshifted disk emission at a hit.</summary>
        Public Shared Sub ComputeEmission(hit As DiskHit, model As BlackHoleModel, ByRef color As pColor, ByRef emission As Single)
            Dim rad = hit.DiskRadius
            Dim tEmit = model.DiskTempInner * System.Math.Pow(model.DiskInner / rad, 0.75)

            Dim vel = MaterialVelocity(hit, model)
            Dim dop = Doppler.Compute(hit.PhotonDir, vel, rad)

            Dim tObs = tEmit * dop.Delta * dop.GravFactor
            tObs = System.Math.Max(500, System.Math.Min(40000, tObs))

            Dim baseCol = BlackBody.Color(tObs)

            Dim brightness = System.Math.Pow(model.DiskInner / rad, 2.0)
            Dim intensity = dop.Boost * brightness * model.DiskBrightness

            color = BlackBody.Safe(baseCol.Red * intensity, baseCol.Green * intensity, baseCol.Blue * intensity)
            emission = color.Luminance
        End Sub

    End Class

    ''' <summary>
    ''' Planckian-locus (blackbody) colour helper plus a NaN/overflow-safe colour factory.
    ''' </summary>
    Public Class BlackBody

        ''' <summary>Blackbody RGB (each channel 0..1) for a temperature in Kelvin (Tanner Helland approximation).</summary>
        Public Shared Function Color(kelvin As Double) As pColor
            Dim temp = kelvin / 100.0
            Dim r, g, b As Double

            If temp <= 66 Then
                r = 255
            Else
                r = 329.698727446 * System.Math.Pow(temp - 60, -0.1332047592)
            End If

            If temp <= 66 Then
                g = 99.4708025861 * System.Math.Log(temp) - 161.1195681661
            Else
                g = 288.1221695283 * System.Math.Pow(temp - 60, -0.0755148492)
            End If

            If temp >= 66 Then
                b = 255
            ElseIf temp <= 19 Then
                b = 0
            Else
                b = 138.5177312231 * System.Math.Log(temp - 10) - 305.0447927307
            End If

            r = Clamp01(r / 255.0)
            g = Clamp01(g / 255.0)
            b = Clamp01(b / 255.0)
            Return Safe(r, g, b)
        End Function

        Private Shared Function Clamp01(x As Double) As Double
            If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then Return 0
            If x < 0 Then Return 0
            If x > 1 Then Return 1
            Return x
        End Function

        ''' <summary>Build a colour clamping every channel to [0,1] and guarding against NaN/Infinity.</summary>
        Public Shared Function Safe(r As Double, g As Double, b As Double) As pColor
            r = Clamp01(r) : g = Clamp01(g) : b = Clamp01(b)
            Return New pColor(CSng(r), CSng(g), CSng(b))
        End Function

    End Class

