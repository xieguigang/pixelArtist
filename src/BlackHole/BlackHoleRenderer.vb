Imports System.Drawing
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D
Imports pColor = Astrophysics.raytracing.pixeldata.Color
Imports pb = Astrophysics.raytracing.pixeldata.PixelBuffer
Imports pd = Astrophysics.raytracing.pixeldata.PixelData
Imports gb = Astrophysics.raytracing.pixeldata.GaussianBlur



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

            Parallel.For(0, height, Sub(y)
                                        If token.IsCancellationRequested Then Return
                                        For x = 0 To width - 1
                                            Dim uv = NormCoords(x, y, width, height)
                                            buf.setPixel(x, y, ComputePixel(model, sky, uv.u, uv.v))
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

        Private Shared Function ComputePixel(model As BlackHoleModel, sky As Starfield, u As Single, v As Single) As pd
            Dim ray = model.GetRay(u, v)
            Dim res = Geodesics.Trace(ray.Origin, ray.Direction, model)

            Dim color As pColor
            If res.Captured Then
                color = pColor.BLACK
            Else
                color = sky.GetColor(res.EscapeDir.Normalize())
            End If

            Dim emission As Single = 0.0F

            If model.DiskEnabled Then
                For i = 0 To res.DiskHits.Count - 1
                    Dim c As pColor = Nothing
                    Dim em As Single = 0.0F
                    AccretionDisk.ComputeEmission(res.DiskHits(i), model, c, em)
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
            Dim i = CInt(v * 255)
            If i < 0 Then i = 0
            If i > 255 Then i = 255
            Return i
        End Function

    End Class

