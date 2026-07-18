Imports System.Threading.Tasks
Imports gb = Astrophysics.raytracing.pixeldata.GaussianBlur
Imports pb = Astrophysics.raytracing.pixeldata.PixelBuffer
Imports pColor = Astrophysics.raytracing.pixeldata.Color
Imports pd = Astrophysics.raytracing.pixeldata.PixelData
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

''' <summary>
''' Ray-traces the black hole scene: for every pixel a camera ray is generated, traced
''' through curved spacetime (see <see cref="Geodesics"/>), and composited with the
''' accretion-disk emission (doppler shifted), the shadow, and the escaped star field.
''' A bloom pass is applied for the disk glow. Output is a <see cref="Bitmap"/> at the
''' requested (low) resolution; the UI stretches it to the canvas.
''' </summary>
Public Class BlackHoleRenderer

    Public Shared Function Render(model As BlackHoleModel, sky As Starfield, width As Integer, height As Integer, Optional token As Threading.CancellationToken = Nothing) As Bitmap
        width = System.Math.Max(1, width)
        height = System.Math.Max(1, height)

        Dim buf = New pb(width, height)
        Dim lanes = Geodesics.LANES
        Dim total = width * height
        Dim packetCount = CInt(System.Math.Ceiling(total / CDbl(lanes)))

        ' Trace photons in SIMD packets, parallelised across packets. Each packet holds
        ' LANES consecutive pixels so the per-packet writes keep good cache locality.
        Parallel.For(0, packetCount, Sub(pk)
                                     If token.IsCancellationRequested Then Return
                                     Dim baseIdx = pk * lanes
                                     Dim n = System.Math.Min(lanes, total - baseIdx)
                                     Dim origins(n - 1) As vec3
                                     Dim dirs(n - 1) As vec3
                                     Dim us(n - 1) As Single
                                     Dim vs(n - 1) As Single
                                     For l = 0 To n - 1
                                         Dim idx = baseIdx + l
                                         Dim x = idx Mod width
                                         Dim y = idx \ width
                                         Dim uv = NormCoords(x, y, width, height)
                                         Dim ray = model.GetRay(uv.u, uv.v)
                                         origins(l) = ray.Origin
                                         dirs(l) = ray.Direction
                                         us(l) = uv.u
                                         vs(l) = uv.v
                                     Next
                                     Dim results = Geodesics.TracePacket(origins, dirs, model)
                                     For l = 0 To n - 1
                                         Dim idx = baseIdx + l
                                         Dim x = idx Mod width
                                         Dim y = idx \ width
                                         buf.setPixel(x, y, ComputePixel(results(l), model, sky))
                                     Next
                                 End Sub)

        If token.IsCancellationRequested Then Return Nothing

        If model.BloomIntensity > 0 Then
            Dim emis = buf.clone()
            emis.filterByEmission(0.15F)
            Dim blur = New gb(emis)
            blur.blur(model.BloomRadius, 1)
            buf.add(blur.PixelBuffer.multiply(CSng(model.BloomIntensity)))
        End If

        Return ToBitmap(buf)
    End Function

    Private Shared Function NormCoords(x As Integer, y As Integer, w As Integer, h As Integer) As (u As Single, v As Single)
        Dim u, v As Single
        If w > h Then
            u = CSng((x - w / 2 + h / 2) / h * 2 - 1)
            v = CSng(-(y / h * 2 - 1))
        Else
            u = CSng(x / w * 2 - 1)
            v = CSng(-((y - h / 2 + w / 2) / w * 2 - 1))
        End If
        Return (u, v)
    End Function

    ''' <summary>
    ''' Composite one traced photon (already integrated by Geodesics.TracePacket) with the
    ''' accretion-disk emission (doppler shifted), the shadow, and the escaped star field.
    ''' </summary>
    Private Shared Function ComputePixel(result As PhotonResult, model As BlackHoleModel, sky As Starfield) As pd
        Dim color As pColor
        If result.Captured Then
            color = pColor.BLACK
        Else
            color = sky.GetColor(result.EscapeDir.Normalize())
        End If

        Dim emission As Single = 0.0F

        If model.DiskEnabled Then
            For i = 0 To result.DiskHits.Count - 1
                Dim c As pColor = Nothing
                Dim em As Single = 0.0F
                AccretionDisk.ComputeEmission(result.DiskHits(i), model, c, em)
                Dim alpha = If(i = 0, 1.0F, 0.6F)
                color = BlackBody.Safe(
                    color.Red * (1 - alpha) + c.Red * alpha,
                    color.Green * (1 - alpha) + c.Green * alpha,
                    color.Blue * (1 - alpha) + c.Blue * alpha)
                If em > emission Then emission = em
            Next
        End If

        Return New pd(color, 1.0F, emission)
    End Function

    Private Shared Function ToBitmap(buf As pb) As Bitmap
        Dim w = buf.Width
        Dim h = buf.Height
        Dim bmp As New Bitmap(w, h)

        For y = 0 To h - 1
            For x = 0 To w - 1
                Dim p = buf.getPixel(x, y)
                If p Is Nothing Then p = New pd(pColor.BLACK, 0, 0)
                Dim c = p.Color
                bmp.SetPixel(x, y, Color.FromArgb(
                    255,
                    Clamp255(c.Red),
                    Clamp255(c.Green),
                    Clamp255(c.Blue)))
            Next
        Next

        Return bmp
    End Function

        Private Shared Function Clamp255(v As Single) As Integer
            If Single.IsNaN(v) OrElse Single.IsInfinity(v) Then Return 0
            If v < 0 Then v = 0
            If v > 1 Then v = 1
            Return CInt(v * 255)
        End Function

End Class

