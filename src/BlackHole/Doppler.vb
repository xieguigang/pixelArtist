Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

    ''' <summary>
    ''' Relativistic Doppler shift, gravitational redshift and relativistic beaming for the
    ''' accretion disk. Given the disk material 3-velocity (beta, in geometric units where
    ''' c = 1) and the photon travel direction (emitter -&gt; observer), it returns:
    ''' <list type="bullet">
    '''   <item><description><c>Delta</c> - Doppler factor 1/(gamma(1 - beta.n)).</description></item>
    '''   <item><description><c>GravFactor</c> - sqrt(1 - Rs/r) gravitational redshift at radius r.</description></item>
    '''   <item><description><c>Boost</c> - relativistic beaming intensity factor delta^3.</description></item>
    ''' </list>
    ''' The observed blackbody temperature is T_emit * Delta * GravFactor, and the observed
    ''' intensity scales as Boost.
    ''' </summary>
    Public Class Doppler

        Public Shared Function Compute(photonDir As vec3, materialVel As vec3, radius As Double) As DopplerResult
            Dim speed = materialVel.Length()
            If speed >= 1.0 Then speed = 0.999
            If speed < 0 Then speed = 0

            Dim gamma = 1.0 / System.Math.Sqrt(1.0 - speed * speed)
            Dim beta = materialVel.Normalize().Multiply(speed)
            Dim n = photonDir.Normalize()

            ' beta . n  (n points emitter -> observer; moving toward observer => positive => blueshift)
            Dim cosA = vec3.Dot(beta, n)
            Dim delta = 1.0 / (gamma * (1.0 - cosA))

            Dim r = System.Math.Max(radius, 1.0001)
            Dim grav = System.Math.Sqrt(System.Math.Max(1.0 - 1.0 / r, 0.01))

            Dim boost = delta * delta * delta
            If Double.IsNaN(delta) OrElse Double.IsInfinity(delta) Then delta = 1.0
            If Double.IsNaN(boost) OrElse Double.IsInfinity(boost) Then boost = 1.0

            Return New DopplerResult With {
                .Delta = delta,
                .GravFactor = grav,
                .Boost = boost,
                .Approaching = cosA > 0
            }
        End Function

    End Class

    Public Structure DopplerResult
        Public Delta As Double
        Public GravFactor As Double
        Public Boost As Double
        Public Approaching As Boolean
    End Structure
