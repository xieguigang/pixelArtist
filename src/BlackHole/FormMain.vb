Imports System.Threading

Public Class FormMain

    Private model As New BlackHoleModel()
    Private sky As New Starfield()
    Private cts As CancellationTokenSource

    Private isRendering As Boolean = False
    Private pending As Boolean = False

    Private dragging As Boolean = False
    Private lastX As Integer = 0
    Private lastY As Integer = 0

    Private Sub FormMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        UpdateHud(0)
        RequestRender()
    End Sub

#Region "Render scheduling (serialised, cancellable)"

    Private Sub RequestRender()
        If isRendering Then
            pending = True
            Return
        End If
        StartRender()
    End Sub

    Private Sub StartRender()
        isRendering = True
        If cts IsNot Nothing Then cts.Cancel()
        cts = New CancellationTokenSource()
        Dim token = cts.Token
        Dim m = model
        Dim sf = sky

        Dim w = CInt(picCanvas.ClientSize.Width * m.RenderScale)
        Dim h = CInt(picCanvas.ClientSize.Height * m.RenderScale)
        If w < 1 Then w = 1
        If h < 1 Then h = 1

        Dim sw = Stopwatch.StartNew()

        Task.Run(Sub()
                     Dim bmp As Bitmap = Nothing
                     Try
                         bmp = BlackHoleRenderer.Render(m, sf, w, h, token)
                     Catch ex As OperationCanceledException
                     Catch ex As Exception
                     Finally
                         If bmp IsNot Nothing Then
                             Dim dt = sw.ElapsedMilliseconds
                             Try
                                 picCanvas.Invoke(Sub()
                                                      Dim old = picCanvas.Image
                                                      picCanvas.Image = bmp
                                                      If old IsNot Nothing AndAlso old IsNot bmp Then old.Dispose()
                                                      UpdateHud(dt)
                                                  End Sub)
                             Catch
                                 bmp.Dispose()
                             End Try
                         End If
                         isRendering = False
                         If pending Then
                             pending = False
                             StartRender()
                         End If
                     End Try
                 End Sub)
    End Sub

#End Region

#Region "HUD / status"

    Private Sub UpdateHud(renderMs As Long)
        lblHud.Text = String.Format("FOV {0:0}°  |  dist {1:F1}  |  a {2:F3}",
                                    model.FieldOfViewDeg, model.Position.Length(), model.Spin)
        Dim kind = If(model.Spin < 0.001, "史瓦西 Schwarzschild", "克尔 Kerr")
        lblStatus.Text = String.Format("cam ({0:F1}, {1:F1}, {2:F1})  pitch {3:F1}° yaw {4:F1}°  render {5} ms  [{6}]",
                                       model.Position.X, model.Position.Y, model.Position.Z,
                                       model.AngleX, model.AngleY, renderMs, kind)
    End Sub

#End Region

#Region "Mouse interaction"

    Private Sub picCanvas_MouseDown(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseDown
        dragging = True
        lastX = e.X
        lastY = e.Y
        picCanvas.Focus()
    End Sub

    Private Sub picCanvas_MouseMove(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseMove
        If Not dragging Then Return
        Dim dx = e.X - lastX
        Dim dy = e.Y - lastY
        lastX = e.X
        lastY = e.Y

        If (e.Button And MouseButtons.Left) = MouseButtons.Left Then
            ' Rotate view (left drag): horizontal -> yaw, vertical -> pitch.
            model.Rotate(dy * 0.3, dx * 0.3)
            RequestRender()
        ElseIf (e.Button And MouseButtons.Right) = MouseButtons.Right Then
            ' Pan observation position (right drag).
            model.Pan(dx, -dy)
            RequestRender()
        End If
    End Sub

    Private Sub picCanvas_MouseUp(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseUp
        dragging = False
    End Sub

    Private Sub picCanvas_MouseWheel(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseWheel
        ' Wheel zooms the observation distance.
        model.Zoom(If(e.Delta > 0, 0.9, 1.1))
        RequestRender()
    End Sub

#End Region

#Region "Control events"

    Private Sub trkSpin_Scroll(sender As Object, e As EventArgs) Handles trkSpin.Scroll
        model.Spin = trkSpin.Value / 1000.0
        lblSpin.Text = model.Spin.ToString("F3")
        RequestRender()
    End Sub

    Private Sub chkDisk_CheckedChanged(sender As Object, e As EventArgs) Handles chkDisk.CheckedChanged
        model.DiskEnabled = chkDisk.Checked
        RequestRender()
    End Sub

    Private Sub trkRes_Scroll(sender As Object, e As EventArgs) Handles trkRes.Scroll
        model.RenderScale = trkRes.Value / 100.0
        lblResVal.Text = trkRes.Value & "%"
        RequestRender()
    End Sub

    Private Sub trkBloom_Scroll(sender As Object, e As EventArgs) Handles trkBloom.Scroll
        model.BloomIntensity = trkBloom.Value / 100.0
        lblBloomVal.Text = model.BloomIntensity.ToString("F2")
        RequestRender()
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        model.ResetView()
        RequestRender()
    End Sub

    Private Sub FormMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        RequestRender()
    End Sub

#End Region

End Class
