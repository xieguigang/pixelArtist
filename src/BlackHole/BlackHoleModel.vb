Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

''' <summary>
''' Central parameter store for the black-hole simulator. Holds the metric parameters
''' (spin, disk geometry), integration settings and the camera state, and exposes the
''' camera ray generation used by the renderer (reusing the Astrophysics ray-tracer
''' camera convention: eye at <c>(0,0,-1/tan(FOV/2))</c> rotated by yaw/pitch).
''' </summary>
Public Class BlackHoleModel

    ' ---- Metric / scene parameters ----
    ''' <summary>Dimensionless Kerr spin chi = a/M in [0, 0.998]. 0 => Schwarzschild.</summary>
    Public Property Spin As Double = 0.0
    ''' <summary>Inner disk radius in Schwarzschild-radius units (default 3 ~ ISCO).</summary>
    Public Property DiskInner As Double = 3.0
    ''' <summary>Outer disk radius in Schwarzschild-radius units.</summary>
    Public Property DiskOuter As Double = 12.0
    Public Property DiskEnabled As Boolean = True
    ''' <summary>Overall disk emission brightness multiplier.</summary>
    Public Property DiskBrightness As Double = 1.0
    ''' <summary>Disk temperature (Kelvin) at the inner edge, driving the blackbody colour.</summary>
    Public Property DiskTempInner As Double = 11000.0

    ' ---- Integration parameters ----
    Public Property StepSize As Double = 0.1
    Public Property MaxSteps As Integer = 2000
    ''' <summary>Radius (Rs units) beyond which a photon is considered to have escaped.</summary>
    Public Property EscapeRadius As Double = 60.0

    ' ---- Rendering ----
    ''' <summary>Fraction of the canvas used for the ray-traced buffer (0.3 - 1.0).</summary>
    Public Property RenderScale As Double = 0.5
    Public Property BloomRadius As Integer = 4
    Public Property BloomIntensity As Double = 0.7
    Public Property FieldOfViewDeg As Double = 60.0

    ' ---- Camera ----
    Public Property Position As vec3 = New vec3(0, 2.0, -14.0)
    ''' <summary>Pitch (degrees).</summary>
    Public Property AngleX As Double = 0.0
    ''' <summary>Yaw (degrees).</summary>
    Public Property AngleY As Double = 0.0

    ' ---- Camera basis helpers ----
    Private Function EyeZ() As Double
        Dim half = FieldOfViewDeg * System.Math.PI / 180.0 / 2.0
        Return -1.0 / System.Math.Tan(half)
    End Function

    Public Structure CameraRay
        Public Origin As vec3
        Public Direction As vec3
    End Structure

    ''' <summary>Generate a world-space camera ray for normalised screen coordinates (u, v) in [-1, 1].</summary>
    Public Function GetRay(u As Single, v As Single) As CameraRay
        Dim eye = New vec3(0, 0, EyeZ())
        Dim dir = (New vec3(u, v, 0)).Subtract(eye).Normalize().RotateYawPitch(CSng(AngleY), CSng(AngleX))
        Return New CameraRay With {.Origin = eye.Add(Position), .Direction = dir}
    End Function

    ''' <summary>Forward / right / up unit vectors of the current camera, for panning.</summary>
    Public Sub ViewBasis(ByRef forward As vec3, ByRef right As vec3, ByRef up As vec3)
        Dim eye = New vec3(0, 0, EyeZ())
        forward = (New vec3(0, 0, 0)).Subtract(eye).Normalize().RotateYawPitch(CSng(AngleY), CSng(AngleX))
        Dim worldUp = New vec3(0, 1, 0)
        right = vec3.Cross(forward, worldUp)
        If right.Length() < 0.000001 Then right = New vec3(1, 0, 0)
        right = right.Normalize()
        up = vec3.Cross(right, forward).Normalize()
    End Sub

    ' ---- Camera controls ----
    Public Sub Rotate(dPitch As Double, dYaw As Double)
        AngleX = System.Math.Max(-89, System.Math.Min(89, AngleX + dPitch))
        AngleY = AngleY + dYaw
    End Sub

    Public Sub Zoom(factor As Double)
        Dim p = Position.Multiply(factor)
        Dim d = p.Length()
        If d > 2.2 AndAlso d < 500 Then Position = p
    End Sub

    Public Sub Pan(dx As Double, dy As Double)
        Dim f, r, u As vec3
        ViewBasis(f, r, u)
        Dim scale = Position.Length() * 0.0015
        Position = Position.Add(r.Multiply(dx * scale)).Add(u.Multiply(dy * scale))
    End Sub

    Public Sub ResetView()
        Position = New vec3(0, 2.0, -14.0)
        AngleX = 0.0
        AngleY = 0.0
    End Sub

End Class
