Imports System.Numerics
Imports System.Collections.Generic
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

''' <summary>
''' SIMD "photon-packet" geodesic integrator.
'''
''' A single photon cannot be vectorised along its own time axis (each RK4 step
''' depends on the previous one), but N independent photons share the same RK4
''' formula at every step, so their state vectors can be packed into
''' System.Numerics.Vector(Of Double) lanes and integrated in lock-step.
'''
''' LANES = Vector(Of Double).Count (4 on AVX2, 8 on AVX-512). The hot RK4 loop
''' therefore runs ~LANES times fewer vector instructions for the same number of
''' photons. Per-lane termination (capture / escape / disk crossing / Kerr
''' turning-point reflection) is handled with bit masks, never scalar branches
''' inside the arithmetic.
'''
''' Schwarzschild and Kerr paths are both implemented here. The conserved
''' initial values (h2 / L / Q / signR / signTh) are computed once per lane with
''' the scalar helpers in Geodesics (they run only once and are a tiny fraction
''' of the cost), then packed into vectors for the integration loop.
''' </summary>
Public Class GeodesicsPacket

    Private Const M As Double = 0.5
    Public Shared ReadOnly LANES As Integer = Vector(Of Double).Count

    ' ============================ public dispatch ============================
    Public Shared Function TracePacket(origins() As vec3, dirs() As vec3, model As BlackHoleModel) As PhotonResult()
        Dim n = origins.Length
        If model.Spin < 0.001 Then
            Return TraceSchwarzschildPacket(origins, dirs, n, model)
        Else
            Return TraceKerrPacket(origins, dirs, n, model)
        End If
    End Function

    ' ============================ helpers ============================
    Private Shared Function NormalizeLane(x As Double, y As Double, z As Double) As vec3
        Dim l = Math.Sqrt(x * x + y * y + z * z)
        If Double.IsNaN(l) OrElse l < 0.000001 Then Return New vec3(0, 0, 1)
        Return New vec3(x / l, y / l, z / l)
    End Function

    ' ============================ Schwarzschild ============================
    Private Shared Function TraceSchwarzschildPacket(origins() As vec3, dirs() As vec3, n As Integer, model As BlackHoleModel) As PhotonResult()
        Dim L = LANES
        Dim oX(L - 1) As Double, oY(L - 1) As Double, oZ(L - 1) As Double
        Dim dX(L - 1) As Double, dY(L - 1) As Double, dZ(L - 1) As Double
        Dim act(L - 1) As Double
        For i = 0 To L - 1
            If i < n Then
                Dim p = origins(i)
                Dim dv = dirs(i).Normalize()
                oX(i) = p.X : oY(i) = p.Y : oZ(i) = p.Z
                dX(i) = dv.X : dY(i) = dv.Y : dZ(i) = dv.Z
                act(i) = 1.0
            Else
                oX(i) = 0 : oY(i) = 0 : oZ(i) = 0
                dX(i) = 0 : dY(i) = 0 : dZ(i) = 0
                act(i) = 0.0
            End If
        Next

        Dim px = New Vector(Of Double)(oX)
        Dim py = New Vector(Of Double)(oY)
        Dim pz = New Vector(Of Double)(oZ)
        Dim vx = New Vector(Of Double)(dX)
        Dim vy = New Vector(Of Double)(dY)
        Dim vz = New Vector(Of Double)(dZ)
        Dim active = New Vector(Of Double)(act)

        ' conserved angular momentum per lane: h = p x v, h2 = |h|^2
        Dim hx = vy * pz - vz * py
        Dim hy = vz * px - vx * pz
        Dim hz = vx * py - vy * px
        Dim h2 = hx * hx + hy * hy + hz * hz

        Dim dt = model.StepSize
        Dim maxSteps = model.MaxSteps
        Dim Rmax = model.EscapeRadius
        Dim Rs = 1.0

        Dim prevX = px, prevY = py, prevZ = pz

        Dim results(n - 1) As PhotonResult
        For i = 0 To n - 1
            results(i) = New PhotonResult With {.DiskHits = New List(Of DiskHit)(), .Captured = False, .Escaped = False, .EscapeDir = New vec3(0, 0, 1)}
        Next

        Dim one = Vector(Of Double).One
        Dim zero = Vector(Of Double).Zero
        Dim RsVec = New Vector(Of Double)(Rs)
        Dim RmaxVec = New Vector(Of Double)(Rmax)
        Dim R16 = New Vector(Of Double)(16.0)
        Dim tiny = New Vector(Of Double)(0.000000001)
        Dim hdt = dt / 2.0
        Dim inv6 = dt / 6.0

        For step = 0 To maxSteps - 1
            ' ---- termination on current position (active lanes only) ----
            Dim r2 = px * px + py * py + pz * pz
            Dim r = Vector.Sqrt(r2)
            Dim dotVP = vx * px + vy * py + vz * pz
            Dim cmpCap = Vector.LessThan(r, RsVec)
            Dim cmpEscR = Vector.GreaterThan(r, RmaxVec)
            Dim cmpEarly = Vector.BitwiseAnd(Vector.GreaterThan(r, R16), Vector.GreaterThan(dotVP, zero))
            Dim cmpTerm = Vector.BitwiseOr(cmpCap, Vector.BitwiseOr(cmpEscR, cmpEarly))
            Dim capBit = Vector.ConditionalSelect(cmpCap, one, zero)
            Dim termBit = Vector.ConditionalSelect(cmpTerm, one, zero) * active

            For i = 0 To L - 1
                If termBit(i) = 1.0 Then
                    If capBit(i) = 1.0 Then
                        results(i).Captured = True
                    Else
                        results(i).Escaped = True
                        results(i).EscapeDir = NormalizeLane(vx(i), vy(i), vz(i))
                    End If
                End If
            Next

            Dim activeAfter = active - termBit

            ' ---- RK4 step (vectorised) ----
            Dim a1x, a1y, a1z As Vector(Of Double)
            AccelS(px, py, pz, h2, activeAfter, a1x, a1y, a1z)
            Dim p2x = px + vx * hdt, p2y = py + vy * hdt, p2z = pz + vz * hdt
            Dim v2x = vx + a1x * hdt, v2y = vy + a1y * hdt, v2z = vz + a1z * hdt

            Dim a2x, a2y, a2z As Vector(Of Double)
            AccelS(p2x, p2y, p2z, h2, activeAfter, a2x, a2y, a2z)
            Dim p3x = px + v2x * hdt, p3y = py + v2y * hdt, p3z = pz + v2z * hdt
            Dim v3x = vx + a2x * hdt, v3y = vy + a2y * hdt, v3z = vz + a2z * hdt

            Dim a3x, a3y, a3z As Vector(Of Double)
            AccelS(p3x, p3y, p3z, h2, activeAfter, a3x, a3y, a3z)
            Dim p4x = px + v3x * dt, p4y = py + v3y * dt, p4z = pz + v3z * dt
            Dim v4x = vx + a3x * dt, v4y = vy + a3y * dt, v4z = vz + a3z * dt

            Dim a4x, a4y, a4z As Vector(Of Double)
            AccelS(p4x, p4y, p4z, h2, activeAfter, a4x, a4y, a4z)

            Dim npx = px + (vx + 2 * v2x + 2 * v3x + v4x) * inv6 * activeAfter
            Dim npy = py + (vy + 2 * v2y + 2 * v3y + v4y) * inv6 * activeAfter
            Dim npz = pz + (vz + 2 * v2z + 2 * v3z + v4z) * inv6 * activeAfter
            Dim nvx = vx + (a1x + 2 * a2x + 2 * a3x + a4x) * inv6 * activeAfter
            Dim nvy = vy + (a1y + 2 * a2y + 2 * a3y + a4y) * inv6 * activeAfter
            Dim nvz = vz + (a1z + 2 * a2z + 2 * a3z + a4z) * inv6 * activeAfter

            ' ---- disk-plane crossing for active lanes ----
            For i = 0 To L - 1
                If activeAfter(i) = 1.0 AndAlso i < n Then
                    Dim yPrev = prevY(i)
                    Dim yNew = npy(i)
                    If (yPrev < 0 AndAlso yNew >= 0) OrElse (yPrev > 0 AndAlso yNew <= 0) Then
                        Dim t = yPrev / (yPrev - yNew)
                        Dim hxx = prevX(i) + t * (npx(i) - prevX(i))
                        Dim hyy = prevY(i) + t * (npy(i) - prevY(i))
                        Dim hzz = prevZ(i) + t * (npz(i) - prevZ(i))
                        Dim rad = Math.Sqrt(hxx * hxx + hzz * hzz)
                        If rad >= model.DiskInner AndAlso rad <= model.DiskOuter Then
                            results(i).DiskHits.Add(New DiskHit With {
                                .Position = New vec3(hxx, hyy, hzz),
                                .PhotonDir = NormalizeLane(nvx(i), nvy(i), nvz(i)),
                                .DiskRadius = rad})
                        End If
                    End If
                End If
            Next

            prevX = npx : prevY = npy : prevZ = npz
            px = npx : py = npy : pz = npz
            vx = nvx : vy = nvy : vz = nvz
            active = activeAfter

            If Vector.EqualsAll(active, zero) Then Exit For
        Next

        ' ---- lanes still active after maxSteps -> escaped ----
        For i = 0 To n - 1
            If active(i) = 1.0 Then
                results(i).Escaped = True
                results(i).EscapeDir = NormalizeLane(vx(i), vy(i), vz(i))
            End If
        Next

        Return results
    End Function

    Private Shared Sub AccelS(px As Vector(Of Double), py As Vector(Of Double), pz As Vector(Of Double), h2 As Vector(Of Double), active As Vector(Of Double),
                               ByRef ax As Vector(Of Double), ByRef ay As Vector(Of Double), ByRef az As Vector(Of Double))
        Dim r2 = px * px + py * py + pz * pz
        Dim r = Vector.Sqrt(r2)
        Dim denom = r2 * r2 * r
        Dim inv = (-1.5) * h2 / denom
        ' guard r ~ 0 to avoid division by zero (matches scalar: r < 1e-9 -> 0)
        inv = Vector.ConditionalSelect(Vector.LessThan(r2, New Vector(Of Double)(0.000000001)), Vector(Of Double).Zero, inv)
        ax = px * inv * active
        ay = py * inv * active
        az = pz * inv * active
    End Sub

    ' ============================ Kerr ============================
    Private Shared Function TraceKerrPacket(origins() As vec3, dirs() As vec3, n As Integer, model As BlackHoleModel) As PhotonResult()
        Dim L = LANES
        Dim aGeom = model.Spin * M
        Dim dt = model.StepSize * 0.8
        Dim maxSteps = model.MaxSteps * 2
        Dim Rmax = model.EscapeRadius
        Dim rHor = M + Math.Sqrt(Math.Max(0, M * M - aGeom * aGeom))

        Dim oR(L - 1) As Double, oTh(L - 1) As Double, oPh(L - 1) As Double
        Dim Lv(L - 1) As Double, Qv(L - 1) As Double, sR(L - 1) As Double, sTh(L - 1) As Double
        Dim act(L - 1) As Double
        Dim pEx(L - 1) As Double, pEy(L - 1) As Double, pEz(L - 1) As Double

        For i = 0 To L - 1
            If i < n Then
                Dim o = origins(i)
                Dim dv = dirs(i).Normalize()
                Dim r0, th0, ph0 As Double
                EmbeddingToBL(o.X, o.Y, o.Z, aGeom, r0, th0, ph0)
                Dim rdot, thdot, phidot As Double
                CartesianVelToBL(o, dv, r0, th0, ph0, aGeom, rdot, thdot, phidot)
                Dim Lc, Qc As Double
                SolveConserved(r0, th0, aGeom, rdot, thdot, phidot, Lc, Qc)
                If Double.IsNaN(Lc) OrElse Double.IsInfinity(Lc) Then Lc = 0
                If Double.IsNaN(Qc) OrElse Double.IsInfinity(Qc) Then Qc = 0
                oR(i) = r0 : oTh(i) = th0 : oPh(i) = ph0
                Lv(i) = Lc : Qv(i) = Qc
                sR(i) = If(rdot >= 0, 1.0, -1.0)
                sTh(i) = If(thdot >= 0, 1.0, -1.0)
                act(i) = 1.0
                Dim e = BLToEmbedding(New BLState With {.r = r0, .th = th0, .ph = ph0}, aGeom)
                pEx(i) = e.X : pEy(i) = e.Y : pEz(i) = e.Z
            Else
                oR(i) = rHor : oTh(i) = Math.PI / 2 : oPh(i) = 0
                Lv(i) = 0 : Qv(i) = 0 : sR(i) = 1 : sTh(i) = 1
                act(i) = 0.0
                pEx(i) = 0 : pEy(i) = 0 : pEz(i) = 0
            End If
        Next

        Dim r = New Vector(Of Double)(oR)
        Dim th = New Vector(Of Double)(oTh)
        Dim ph = New Vector(Of Double)(oPh)
        Dim Lvec = New Vector(Of Double)(Lv)
        Dim Qvec = New Vector(Of Double)(Qv)
        Dim signR = New Vector(Of Double)(sR)
        Dim signTh = New Vector(Of Double)(sTh)
        Dim active = New Vector(Of Double)(act)
        Dim prevEx = New Vector(Of Double)(pEx)
        Dim prevEy = New Vector(Of Double)(pEy)
        Dim prevEz = New Vector(Of Double)(pEz)

        Dim results(n - 1) As PhotonResult
        For i = 0 To n - 1
            results(i) = New PhotonResult With {.DiskHits = New List(Of DiskHit)(), .Captured = False, .Escaped = False, .EscapeDir = New vec3(0, 0, 1)}
        Next

        Dim one = Vector(Of Double).One
        Dim zero = Vector(Of Double).Zero
        Dim rHorVec = New Vector(Of Double)(rHor)
        Dim RmaxVec = New Vector(Of Double)(Rmax)
        Dim R16 = New Vector(Of Double)(16.0)
        Dim infVec = New Vector(Of Double)(Double.PositiveInfinity)
        Dim hdt = dt / 2.0
        Dim inv6 = dt / 6.0

        For step = 0 To maxSteps - 1
            ' current embedding of the state (used for escape direction + disk crossing base)
            Dim csX, csY, csZ As Vector(Of Double)
            BLToEmbV(r, th, ph, aGeom, csX, csY, csZ)

            ' ---- termination on current state (active lanes only) ----
            Dim cmpCap = Vector.LessThanOrEqual(r, rHorVec)
            Dim cmpEscR = Vector.GreaterThan(r, RmaxVec)
            Dim cmpEarly = Vector.BitwiseAnd(Vector.GreaterThan(r, R16), Vector.GreaterThan(signR, zero))
            Dim notNanBit = Vector.ConditionalSelect(Vector.Equals(r, r), one, zero)
            Dim isNanBit = one - notNanBit
            Dim isInfBit = Vector.ConditionalSelect(Vector.Equals(Vector.Abs(r), infVec), one, zero)
            Dim cmpBad = Vector.BitwiseOr(isNanBit, isInfBit)
            Dim cmpTerm = Vector.BitwiseOr(cmpCap, Vector.BitwiseOr(cmpEscR, Vector.BitwiseOr(cmpEarly, cmpBad)))
            Dim capBit = Vector.ConditionalSelect(cmpCap, one, zero)
            Dim termBit = Vector.ConditionalSelect(cmpTerm, one, zero) * active

            For i = 0 To L - 1
                If termBit(i) = 1.0 Then
                    If capBit(i) = 1.0 Then
                        results(i).Captured = True
                    Else
                        results(i).Escaped = True
                        results(i).EscapeDir = NormalizeLane(csX(i) - prevEx(i), csY(i) - prevEy(i), csZ(i) - prevEz(i))
                    End If
                End If
            Next

            Dim activeAfter = active - termBit

            ' ---- RK4 step (vectorised) ----
            Dim dr1, dth1, dph1 As Vector(Of Double)
            KerrDerivV(r, th, ph, aGeom, Lvec, Qvec, signR, signTh, activeAfter, dr1, dth1, dph1)
            Dim r2r = r + dr1 * hdt, th2 = th + dth1 * hdt, ph2 = ph + dph1 * hdt
            Dim dr2, dth2, dph2 As Vector(Of Double)
            KerrDerivV(r2r, th2, ph2, aGeom, Lvec, Qvec, signR, signTh, activeAfter, dr2, dth2, dph2)
            Dim r3r = r + dr2 * hdt, th3 = th + dth2 * hdt, ph3 = ph + dph2 * hdt
            Dim dr3, dth3, dph3 As Vector(Of Double)
            KerrDerivV(r3r, th3, ph3, aGeom, Lvec, Qvec, signR, signTh, activeAfter, dr3, dth3, dph3)
            Dim r4r = r + dr3 * dt, th4 = th + dth3 * dt, ph4 = ph + dph3 * dt
            Dim dr4, dth4, dph4 As Vector(Of Double)
            KerrDerivV(r4r, th4, ph4, aGeom, Lvec, Qvec, signR, signTh, activeAfter, dr4, dth4, dph4)

            Dim ns_r = r + (dr1 + 2 * dr2 + 2 * dr3 + dr4) * inv6 * activeAfter
            Dim ns_th = th + (dth1 + 2 * dth2 + 2 * dth3 + dth4) * inv6 * activeAfter
            Dim ns_ph = ph + (dph1 + 2 * dph2 + 2 * dph3 + dph4) * inv6 * activeAfter

            ' ---- radial / polar turning-point reflection (per lane) ----
            Dim kR = KerrRV(ns_r, aGeom, Lvec, Qvec)
            Dim refR = Vector.ConditionalSelect(Vector.LessThan(kR, zero), one, zero) * activeAfter
            ns_r = Vector.ConditionalSelect(Vector.Equals(refR, one), r, ns_r)
            signR = signR - 2 * refR * signR

            Dim kT = KerrThetaV(ns_th, aGeom, Lvec, Qvec)
            Dim refT = Vector.ConditionalSelect(Vector.LessThan(kT, zero), one, zero) * activeAfter
            ns_th = Vector.ConditionalSelect(Vector.Equals(refT, one), th, ns_th)
            signTh = signTh - 2 * refT * signTh

            ' embedding of the new state
            Dim ceX, ceY, ceZ As Vector(Of Double)
            BLToEmbV(ns_r, ns_th, ns_ph, aGeom, ceX, ceY, ceZ)

            ' ---- disk-plane crossing for active lanes ----
            For i = 0 To L - 1
                If activeAfter(i) = 1.0 AndAlso i < n Then
                    Dim yPrev = prevEy(i)
                    Dim yNew = ceY(i)
                    If (yPrev < 0 AndAlso yNew >= 0) OrElse (yPrev > 0 AndAlso yNew <= 0) Then
                        Dim t = yPrev / (yPrev - yNew)
                        Dim hxx = prevEx(i) + t * (ceX(i) - prevEx(i))
                        Dim hyy = prevEy(i) + t * (ceY(i) - prevEy(i))
                        Dim hzz = prevEz(i) + t * (ceZ(i) - prevEz(i))
                        Dim rad = Math.Sqrt(hxx * hxx + hzz * hzz)
                        If rad >= model.DiskInner AndAlso rad <= model.DiskOuter Then
                            results(i).DiskHits.Add(New DiskHit With {
                                .Position = New vec3(hxx, hyy, hzz),
                                .PhotonDir = NormalizeLane(ceX(i) - prevEx(i), ceY(i) - prevEy(i), ceZ(i) - prevEz(i)),
                                .DiskRadius = rad})
                        End If
                    End If
                End If
            Next

            prevEx = ceX : prevEy = ceY : prevEz = ceZ
            r = ns_r : th = ns_th : ph = ns_ph
            active = activeAfter

            If Vector.EqualsAll(active, zero) Then Exit For
        Next

        ' ---- lanes still active after maxSteps -> escaped ----
        Dim csX2, csY2, csZ2 As Vector(Of Double)
        BLToEmbV(r, th, ph, aGeom, csX2, csY2, csZ2)
        For i = 0 To n - 1
            If active(i) = 1.0 Then
                results(i).Escaped = True
                results(i).EscapeDir = NormalizeLane(csX2(i) - prevEx(i), csY2(i) - prevEy(i), csZ2(i) - prevEz(i))
            End If
        Next

        Return results
    End Function

    Private Shared Sub KerrDerivV(r As Vector(Of Double), th As Vector(Of Double), ph As Vector(Of Double), a As Double,
                                   L As Vector(Of Double), Q As Vector(Of Double), signR As Vector(Of Double), signTh As Vector(Of Double), active As Vector(Of Double),
                                   ByRef dr As Vector(Of Double), ByRef dth As Vector(Of Double), ByRef dph As Vector(Of Double))
        Dim sc = Vector.SinCos(th)
        Dim s = sc.Item1
        Dim c = sc.Item2
        Dim s2 = s * s
        Dim c2 = c * c
        Dim Sigma = r * r + a * a * c2
        Dim Delta = r * r - 2 * M * r + a * a
        Dim T = (r * r + a * a) - a * L
        Dim radR = T * T - Delta * (Q + (a - L) * (a - L))
        ' guard s2 ~ 0 to avoid division by zero (matches scalar s2 clamp)
        Dim sGuard = Vector.GreaterThan(s2, New Vector(Of Double)(0.0000001))
        Dim fullThetaR = Q - c2 * (a * a - L * L / s2)
        Dim thetaR = Vector.ConditionalSelect(sGuard, fullThetaR, Q)
        dr = signR * Vector.Sqrt(radR) / Sigma
        dth = signTh * Vector.Sqrt(thetaR) / Sigma
        dph = (-(a - L / s2) + a * T / Delta) / Sigma
        dr = dr * active
        dth = dth * active
        dph = dph * active
    End Sub

    Private Shared Sub BLToEmbV(r As Vector(Of Double), th As Vector(Of Double), ph As Vector(Of Double), a As Double,
                                ByRef ex As Vector(Of Double), ByRef ey As Vector(Of Double), ByRef ez As Vector(Of Double))
        Dim rho = Vector.Sqrt(r * r + a * a)
        Dim sc = Vector.SinCos(th)
        Dim sinTh = sc.Item1, cosTh = sc.Item2
        Dim scp = Vector.SinCos(ph)
        Dim sinPh = scp.Item1, cosPh = scp.Item2
        ex = rho * sinTh * cosPh
        ez = rho * sinTh * sinPh
        ey = r * cosTh
    End Sub

    Private Shared Function KerrRV(r As Vector(Of Double), a As Double, L As Vector(Of Double), Q As Vector(Of Double)) As Vector(Of Double)
        Dim Delta = r * r - 2 * M * r + a * a
        Dim T = (r * r + a * a) - a * L
        Return T * T - Delta * (Q + (a - L) * (a - L))
    End Function

    Private Shared Function KerrThetaV(th As Vector(Of Double), a As Double, L As Vector(Of Double), Q As Vector(Of Double)) As Vector(Of Double)
        Dim sc = Vector.SinCos(th)
        Dim s = sc.Item1 : Dim s2 = s * s
        Dim c = sc.Item2 : Dim c2 = c * c
        Return Q - c2 * (a * a - L * L / s2)
    End Function

End Class
