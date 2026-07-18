Imports System.Numerics
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

''' <summary>
''' A single disk crossing recorded along a photon's path.
''' </summary>
Public Structure DiskHit
    ''' <summary>Position of the crossing in the embedding Cartesian frame (Rs = 1 units).</summary>
    Public Position As vec3
    ''' <summary>Direction the photon travels at the hit (emitter -> observer), used for the Doppler factor.</summary>
    Public PhotonDir As vec3
    ''' <summary>Radial distance from the spin axis in the disk plane.</summary>
    Public DiskRadius As Double
End Structure

''' <summary>
''' Structured outcome of tracing one photon through curved spacetime.
''' </summary>
Public Structure PhotonResult
    Public Captured As Boolean
    Public Escaped As Boolean
    Public DiskHits As List(Of DiskHit)
    Public EscapeDir As vec3
End Structure

''' <summary>
''' Geodesic (null) integrator for Schwarzschild (a = 0) and Kerr (a &gt; 0) black holes.
''' All lengths are expressed in Schwarzschild-radius units (Rs = 1), so the mass
''' parameter is M = 0.5. The spin handed to <see cref="Trace"/> is the dimensionless
''' Kerr spin chi = a/M in [0, 0.998]; the geometric spin used internally is a_geom = chi * M.
''' The visualisation embedding uses the spin axis aligned with the world +Y axis, so the
''' accretion disk lives in the XZ plane (y = 0).
''' </summary>
Public Class Geodesics

    ' Geometric units: Rs = 2M = 1  ->  M = 0.5
    Private Const M As Double = 0.5

    ''' <summary>
    ''' Trace a photon launched from <paramref name="origin"/> (embedding Cartesian, Rs = 1)
    ''' along unit <paramref name="direction"/>. Dispatches to Schwarzschild or Kerr depending
    ''' on the model spin.
    ''' </summary>
    Public Shared Function Trace(origin As vec3, direction As vec3, model As BlackHoleModel) As PhotonResult
        If model.Spin < 0.001 Then
            Return TraceSchwarzschild(origin, direction, model)
        Else
            Return TraceKerr(origin, direction, model)
        End If
    End Function

#Region "Schwarzschild (exact, Cartesian RK4)"

    Private Shared Function SchwarzschildAccel(p As vec3, h2 As Double) As vec3
        Dim r2 = p.X * p.X + p.Y * p.Y + p.Z * p.Z
        Dim r = System.Math.Sqrt(r2)
        If r < 0.000000001 Then Return New vec3(0, 0, 0)
        ' a = -1.5 * h^2 * r_vec / |r|^5   (Rs = 1, M = 0.5 => 3M = 1.5)
        Dim inv = -1.5 * h2 / (r2 * r2 * r)
        Return New vec3(p.X * inv, p.Y * inv, p.Z * inv)
    End Function

    Private Shared Sub RK4Schwarzschild(ByRef p As vec3, ByRef v As vec3, h2 As Double, dt As Double)
        Dim k1p = v
        Dim k1v = SchwarzschildAccel(p, h2)

        Dim p2 = p.Add(k1p.Multiply(dt / 2))
        Dim v2 = v.Add(k1v.Multiply(dt / 2))
        Dim k2p = v2
        Dim k2v = SchwarzschildAccel(p2, h2)

        Dim p3 = p.Add(k2p.Multiply(dt / 2))
        Dim v3 = v.Add(k2v.Multiply(dt / 2))
        Dim k3p = v3
        Dim k3v = SchwarzschildAccel(p3, h2)

        Dim p4 = p.Add(k3p.Multiply(dt))
        Dim v4 = v.Add(k3v.Multiply(dt))
        Dim k4p = v4
        Dim k4v = SchwarzschildAccel(p4, h2)

        p = p.Add((k1p.Add(k2p.Multiply(2)).Add(k3p.Multiply(2)).Add(k4p)).Multiply(dt / 6))
        v = v.Add((k1v.Add(k2v.Multiply(2)).Add(k3v.Multiply(2)).Add(k4v)).Multiply(dt / 6))
    End Sub

    Private Shared Function TraceSchwarzschild(origin As vec3, direction As vec3, model As BlackHoleModel) As PhotonResult
        Dim res As New PhotonResult With {
            .DiskHits = New List(Of DiskHit)(),
            .Captured = False,
            .Escaped = False,
            .EscapeDir = New vec3(0, 0, 1)
        }

        Dim p = origin
        Dim v = direction.Normalize()
        Dim h = vec3.Cross(p, v)
        Dim h2 = vec3.Dot(h, h)

        Dim dt = model.StepSize
        Dim maxSteps = model.MaxSteps
        Dim Rmax = model.EscapeRadius
        Dim Rs = 1.0

        Dim prev = p

        For i = 0 To maxSteps - 1
            Dim r = p.Length()
            If r < Rs Then
                res.Captured = True
                Return res
            End If
            If r > Rmax Then
                res.Escaped = True
                res.EscapeDir = v.Normalize()
                Return res
            End If
            ' Early-out: once clearly outbound beyond the strong-lensing region, the
            ' trajectory asymptotes and we can sample the background immediately.
            If r > 16 AndAlso vec3.Dot(v, p) > 0 Then
                res.Escaped = True
                res.EscapeDir = v.Normalize()
                Return res
            End If

            Dim np = p
            Dim nv = v
            RK4Schwarzschild(np, nv, h2, dt)

            ' Disk lives in the XZ plane (y = 0). Detect a crossing of the equatorial plane.
            If (prev.Y < 0 AndAlso np.Y >= 0) OrElse (prev.Y > 0 AndAlso np.Y <= 0) Then
                Dim t = prev.Y / (prev.Y - np.Y)
                Dim hit = vec3.Lerp(prev, np, CSng(t))
                Dim rad = System.Math.Sqrt(hit.X * hit.X + hit.Z * hit.Z)
                If rad >= model.DiskInner AndAlso rad <= model.DiskOuter Then
                    res.DiskHits.Add(New DiskHit With {
                        .Position = hit,
                        .PhotonDir = nv.Normalize(),
                        .DiskRadius = rad
                    })
                End If
            End If

            prev = np
            p = np
            v = nv
        Next

        res.Escaped = True
        res.EscapeDir = v.Normalize()
        Return res
    End Function

#End Region

#Region "Kerr (Carter constants, first-order system)"

    Friend Structure BLState
        Public r As Double
        Public th As Double
        Public ph As Double
    End Structure

    ''' <summary>Convert embedding Cartesian (spin axis = Y) to Boyer-Lindquist (r, theta, phi).</summary>
    Friend Shared Sub EmbeddingToBL(x As Double, y As Double, z As Double, a As Double, ByRef r As Double, ByRef th As Double, ByRef ph As Double)
        Dim S2 = x * x + y * y + z * z
        Dim u = 0.5 * ((S2 - a * a) + System.Math.Sqrt(System.Math.Max(0, (S2 - a * a) * (S2 - a * a) + 4 * a * a * y * y)))
        r = System.Math.Sqrt(System.Math.Max(0, u))
        Dim cosignThh = If(r > 0.000000001, y / r, 0)
        cosignThh = System.Math.Max(-1, System.Math.Min(1, cosignThh))
        th = System.Math.Acos(cosignThh)
        ph = System.Math.Atan2(z, x)
    End Sub

    ''' <summary>Convert Boyer-Lindquist (r, theta, phi) to embedding Cartesian (spin axis = Y).</summary>
    Friend Shared Function BLToEmbedding(st As BLState, a As Double) As vec3
        Dim rho = System.Math.Sqrt(st.r * st.r + a * a)
        Dim sinTh = System.Math.Sin(st.th)
        Dim cosTh = System.Math.Cos(st.th)
        Dim x = rho * sinTh * System.Math.Cos(st.ph)
        Dim z = rho * sinTh * System.Math.Sin(st.ph)
        Dim y = st.r * cosTh
        Return New vec3(x, y, z)
    End Function

    ''' <summary>Solve J * (rdot, thdot, phidot) = dir for the initial Boyer-Lindquist velocity.</summary>
    Private Shared Sub CartesianVelToBL(pos As vec3, dir As vec3, r0 As Double, th0 As Double, ph0 As Double, a As Double, ByRef rdot As Double, ByRef thdot As Double, ByRef phidot As Double)
        Dim rho = System.Math.Sqrt(r0 * r0 + a * a)
        Dim rhoR = r0 / rho
        Dim sinTh = System.Math.Sin(th0)
        Dim cosTh = System.Math.Cos(th0)
        Dim cosPh = System.Math.Cos(ph0)
        Dim sinPh = System.Math.Sin(ph0)

        Dim jac(,) As Double = {
            {rhoR * sinTh * cosPh, rho * cosTh * cosPh, -rho * sinTh * sinPh},
            {cosTh, -r0 * sinTh, 0},
            {rhoR * sinTh * sinPh, rho * cosTh * sinPh, rho * sinTh * cosPh}
        }
        Dim b() As Double = {dir.X, dir.Y, dir.Z}
        Dim x() As Double = {0, 0, 0}
        Solve3x3(jac, b, x)
        rdot = x(0)
        thdot = x(1)
        phidot = x(2)
    End Sub

    Private Shared Sub Solve3x3(A(,) As Double, b() As Double, ByRef x() As Double)
        Dim n = 3
        Dim m(,) As Double = {
            {A(0, 0), A(0, 1), A(0, 2), b(0)},
            {A(1, 0), A(1, 1), A(1, 2), b(1)},
            {A(2, 0), A(2, 1), A(2, 2), b(2)}
        }
        For i = 0 To n - 1
            Dim piv = i
            For k = i + 1 To n - 1
                If System.Math.Abs(m(k, i)) > System.Math.Abs(m(piv, i)) Then piv = k
            Next
            If piv <> i Then
                For c = 0 To n Step 1
                    Dim tmp = m(i, c) : m(i, c) = m(piv, c) : m(piv, c) = tmp
                Next
            End If
            Dim d = m(i, i)
            If System.Math.Abs(d) < 0.000000000001 Then d = 0.000000000001
            For c = i To n Step 1 : m(i, c) /= d : Next
            For k = 0 To n - 1
                If k <> i Then
                    Dim f = m(k, i)
                    For c = i To n Step 1 : m(k, c) -= f * m(i, c) : Next
                End If
            Next
        Next
        x = {m(0, n), m(1, n), m(2, n)}
    End Sub

    Private Shared Function KerrR(r As Double, a As Double, L As Double, Q As Double) As Double
        Dim Delta = r * r - 2 * M * r + a * a
        Dim T = (r * r + a * a) - a * L
        Return T * T - Delta * (Q + (a - L) * (a - L))
    End Function

    Private Shared Function KerrTheta(th As Double, a As Double, L As Double, Q As Double) As Double
        Dim s2 = System.Math.Sin(th)
        s2 = s2 * s2
        Dim c2 = System.Math.Cos(th)
        c2 = c2 * c2
        If s2 < 0.000000000001 Then s2 = 0.000000000001
        Return Q - c2 * (a * a - L * L / s2)
    End Function

    ''' <summary>
    ''' Solve the conserved quantities (Lz, Q) from the initial (r, theta) speeds.
    ''' Energy is normalised to E = 1. The correct angular-momentum root is selected by
    ''' matching the predicted dphi/dlambda to the Jacobian-derived phidot.
    ''' </summary>
    Friend Shared Sub SolveConserved(r0 As Double, th0 As Double, a As Double, rdot As Double, thdot As Double, phidot As Double, ByRef L As Double, ByRef Q As Double)
        Dim s2 = System.Math.Sin(th0) : s2 = s2 * s2
        Dim c2 = System.Math.Cos(th0) : c2 = c2 * c2
        If s2 < 0.000000000001 Then s2 = 0.000000000001

        Dim Sigma0 = r0 * r0 + a * a * c2
        Dim Delta0 = r0 * r0 - 2 * M * r0 + a * a
        Dim A0 = r0 * r0 + a * a
        Dim rhsignR = (rdot * rdot) * Sigma0 * Sigma0
        Dim rhsignTh = (thdot * thdot) * Sigma0 * Sigma0

        ' From Theta:  Q = rhsignTh + c2*(a^2 - L^2/s2)
        ' Substitute into R to obtain a quadratic in L.
        Dim K = (s2 - c2) / s2
        Dim qc2 = a * a - Delta0 * K
        Dim qc1 = -2 * a * (A0 - Delta0)
        Dim qc0 = A0 * A0 - Delta0 * (rhsignTh + a * a * (1 + c2)) - rhsignR

        Dim disc = qc1 * qc1 - 4 * qc2 * qc0
        If disc < 0 Then disc = 0
        Dim sq = System.Math.Sqrt(disc)
        Dim L1 = If(qc2 <> 0, (-qc1 + sq) / (2 * qc2), -qc0 / qc1)
        Dim L2 = If(qc2 <> 0, (-qc1 - sq) / (2 * qc2), L1)

        ' Choose the root whose predicted dphi/dlambda matches the Jacobian phidot.
        Dim bestL = L1
        Dim bestErr = Double.MaxValue
        For Each cand In {L1, L2}
            Dim Qc = rhsignTh + c2 * (a * a - cand * cand / s2)
            Dim T = A0 - a * cand
            Dim dph = (-(a - cand / s2) + a * T / Delta0) / Sigma0
            Dim err = System.Math.Abs(dph - phidot)
            If err < bestErr Then bestErr = err : bestL = cand : Q = Qc
        Next
        L = bestL
    End Sub

    Private Shared Sub KerrDeriv(st As BLState, a As Double, L As Double, Q As Double, signR As Double, signTh As Double, ByRef dr As Double, ByRef dth As Double, ByRef dph As Double)
        Dim r = st.r
        Dim th = st.th
        Dim s2 = System.Math.Sin(th) : s2 = s2 * s2
        Dim c2 = System.Math.Cos(th) : c2 = c2 * c2
        If s2 < 0.000000000001 Then s2 = 0.000000000001
        Dim Sigma = r * r + a * a * c2
        Dim Delta = r * r - 2 * M * r + a * a
        If System.Math.Abs(Delta) < 0.000000001 Then Delta = 0.000000001
        If System.Math.Abs(Sigma) < 0.000000001 Then Sigma = 0.000000001
        Dim T = (r * r + a * a) - a * L
        Dim radR = T * T - Delta * (Q + (a - L) * (a - L))
        Dim thetaR = Q - c2 * (a * a - L * L / s2)
        dr = signR * System.Math.Sqrt(System.Math.Max(radR, 0)) / Sigma
        dth = signTh * System.Math.Sqrt(System.Math.Max(thetaR, 0)) / Sigma
        dph = (-(a - L / s2) + a * T / Delta) / Sigma
    End Sub

    Private Shared Sub RK4Kerr(current As BLState, a As Double, L As Double, Q As Double, signR As Double, signTh As Double, dt As Double, ByRef nextState As BLState)
        Dim k1r, k1t, k1p As Double
        KerrDeriv(current, a, L, Q, signR, signTh, k1r, k1t, k1p)
        Dim s2 As New BLState With {.r = current.r + k1r * dt / 2, .th = current.th + k1t * dt / 2, .ph = current.ph + k1p * dt / 2}
        Dim k2r, k2t, k2p As Double
        KerrDeriv(s2, a, L, Q, signR, signTh, k2r, k2t, k2p)
        Dim s3 As New BLState With {.r = current.r + k2r * dt / 2, .th = current.th + k2t * dt / 2, .ph = current.ph + k2p * dt / 2}
        Dim k3r, k3t, k3p As Double
        KerrDeriv(s3, a, L, Q, signR, signTh, k3r, k3t, k3p)
        Dim s4 As New BLState With {.r = current.r + k3r * dt, .th = current.th + k3t * dt, .ph = current.ph + k3p * dt}
        Dim k4r, k4t, k4p As Double
        KerrDeriv(s4, a, L, Q, signR, signTh, k4r, k4t, k4p)

        nextState.r = current.r + dt / 6 * (k1r + 2 * k2r + 2 * k3r + k4r)
        nextState.th = current.th + dt / 6 * (k1t + 2 * k2t + 2 * k3t + k4t)
        nextState.ph = current.ph + dt / 6 * (k1p + 2 * k2p + 2 * k3p + k4p)
    End Sub

    Private Shared Function TraceKerr(origin As vec3, direction As vec3, model As BlackHoleModel) As PhotonResult
        Dim res As New PhotonResult With {
            .DiskHits = New List(Of DiskHit)(),
            .Captured = False,
            .Escaped = False,
            .EscapeDir = New vec3(0, 0, 1)
        }

        Dim a = model.Spin * M   ' dimensionless spin -> geometric spin
        Dim dt = model.StepSize * 0.8
        Dim maxSteps = model.MaxSteps * 2
        Dim Rmax = model.EscapeRadius
        Dim rHor = M + System.Math.Sqrt(System.Math.Max(0, M * M - a * a))

        Dim r0, th0, ph0 As Double
        EmbeddingToBL(origin.X, origin.Y, origin.Z, a, r0, th0, ph0)

        Dim rdot, thdot, phidot As Double
        CartesianVelToBL(origin, direction.Normalize(), r0, th0, ph0, a, rdot, thdot, phidot)

        Dim L, Q As Double
        SolveConserved(r0, th0, a, rdot, thdot, phidot, L, Q)
        If Double.IsNaN(L) OrElse Double.IsNaN(Q) OrElse Double.IsInfinity(L) OrElse Double.IsInfinity(Q) Then
            L = 0 : Q = 0
        End If

        Dim signR = If(rdot >= 0, 1.0, -1.0)
        Dim signTh = If(thdot >= 0, 1.0, -1.0)

        Dim st As New BLState With {.r = r0, .th = th0, .ph = ph0}
        Dim prevEmb = BLToEmbedding(st, a)

        For i = 0 To maxSteps - 1
            If Double.IsNaN(st.r) OrElse Double.IsInfinity(st.r) Then Exit For
            If st.r <= rHor Then
                res.Captured = True
                Return res
            End If
            If st.r > Rmax Then
                res.Escaped = True
                res.EscapeDir = EscapeDirFrom(st, prevEmb, a)
                Return res
            End If
            ' Early-out: once clearly outbound beyond the strong-lensing region, the
            ' trajectory asymptotes and we can sample the background immediately.
            If st.r > 16 AndAlso signR > 0 Then
                res.Escaped = True
                res.EscapeDir = EscapeDirFrom(st, prevEmb, a)
                Return res
            End If

            Dim ns As New BLState
            RK4Kerr(st, a, L, Q, signR, signTh, dt, ns)

            ' Turning-point handling. The radial/theta equations use dr = sign * sqrt(R)/Sigma,
            ' so inside the classically-forbidden region (R < 0) the step would be imaginary.
            ' When a step overshoots a turning point we reflect: keep the coordinate at its
            ' pre-step value and reverse the direction, instead of freezing at dr = 0.
            If KerrR(ns.r, a, L, Q) < 0 Then
                ns = New BLState With {.r = st.r, .th = st.th, .ph = st.ph}
                signR = -signR
            End If
            If KerrTheta(ns.th, a, L, Q) < 0 Then
                ns.th = st.th
                signTh = -signTh
            End If

            Dim curEmb = BLToEmbedding(ns, a)

            If (prevEmb.Y < 0 AndAlso curEmb.Y >= 0) OrElse (prevEmb.Y > 0 AndAlso curEmb.Y <= 0) Then
                Dim t = prevEmb.Y / (prevEmb.Y - curEmb.Y)
                Dim hit = vec3.Lerp(prevEmb, curEmb, CSng(t))
                Dim rad = System.Math.Sqrt(hit.X * hit.X + hit.Z * hit.Z)
                If rad >= model.DiskInner AndAlso rad <= model.DiskOuter Then
                    Dim dir = curEmb.Subtract(prevEmb).Normalize()
                    If dir.Length() < 0.000001 Then dir = direction.Normalize()
                    res.DiskHits.Add(New DiskHit With {
                        .Position = hit,
                        .PhotonDir = dir,
                        .DiskRadius = rad
                    })
                End If
            End If

            prevEmb = curEmb
            st = ns
        Next

        ' Ran out of steps: treat as escaped with the current (asymptotic) direction.
        res.Escaped = True
        res.EscapeDir = EscapeDirFrom(st, prevEmb, a)
        Return res
    End Function

    Private Shared Function EscapeDirFrom(st As BLState, prevEmb As vec3, a As Double) As vec3
        Dim ed = (BLToEmbedding(st, a).Subtract(prevEmb)).Normalize()
        If Double.IsNaN(ed.X) OrElse Double.IsNaN(ed.Y) OrElse Double.IsNaN(ed.Z) OrElse ed.Length() < 0.000001 Then
            ed = New vec3(0, 0, 1)
        End If
        Return ed
    End Function

#End Region

#Region "Packet tracing (SIMD) dispatch"

    ''' <summary>
    ''' When True (default) the renderer uses the SIMD photon-packet integrator in
    ''' GeodesicsPacket. Set to False to fall back to the scalar Trace for A/B
    ''' correctness comparison (the two implementations should agree within
    ''' floating-point tolerance).
    ''' </summary>
    Public Shared Property UsePacketTracing As Boolean = True

    ''' <summary>Number of photons integrated in lock-step per SIMD packet (Vector(Of Double).Count).</summary>
    Public Shared ReadOnly Property LANES As Integer = Vector(Of Double).Count

    ''' <summary>
    ''' Trace a packet of photons. origins/dirs must have the same length (1..LANES).
    ''' Returns a PhotonResult per input ray.
    ''' </summary>
    Public Shared Function TracePacket(origins() As vec3, dirs() As vec3, model As BlackHoleModel) As PhotonResult()
        If UsePacketTracing Then
            Return GeodesicsPacket.TracePacket(origins, dirs, model)
        Else
            Dim n = origins.Length
            Dim res(n - 1) As PhotonResult
            For i = 0 To n - 1
                res(i) = Trace(origins(i), dirs(i), model)
            Next
            Return res
        End If
    End Function

#End Region

End Class
