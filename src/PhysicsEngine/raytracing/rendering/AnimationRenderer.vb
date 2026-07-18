Imports System.IO
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Astrophysics.raytracing.pixeldata
Imports BufferedImage = Microsoft.VisualBasic.Imaging.BitmapImage.BitmapBuffer
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.rendering

    Public Class AnimationRenderer

        Private Shared firstPosition As vec3
        Private Shared firstYaw, firstPitch As Single

        Private Shared secondPosition As vec3
        Private Shared secondYaw, secondPitch As Single

        Public Shared Sub captureFirstPosition(camera As Camera)
            firstPosition = camera.position.Clone()
            firstYaw = camera.AngleY
            firstPitch = camera.AngleX
        End Sub

        Public Shared Sub captureSecondPosition(camera As Camera)
            secondPosition = camera.position.Clone()
            secondYaw = camera.AngleY
            secondPitch = camera.AngleX
        End Sub

        ''' <summary>
        ''' Renders an image sequence by interpolating the camera between the
        ''' captured first and second positions and writes one PNG per frame
        ''' into <paramref name="outputDirectory"/>.
        ''' </summary>
        Public Shared Sub renderImageSequence(scene As Scene, outputDirectory As String, outputWidth As Integer, outputHeight As Integer, frameCount As Integer, resolution As Single, postProcessing As Boolean)
            If Not Directory.Exists(outputDirectory) Then
                Directory.CreateDirectory(outputDirectory)
            End If

            For frame = 0 To frameCount - 1
                Dim t = If(frameCount > 1, CSng(frame) / (frameCount - 1), 0.0F)
                Dim position = vec3.Lerp(firstPosition, secondPosition, t)
                Dim yaw = firstYaw + (secondYaw - firstYaw) * t
                Dim pitch = firstPitch + (secondPitch - firstPitch) * t

                Dim cam = scene.Camera
                cam.position = position
                cam.AngleY = yaw
                cam.AngleX = pitch

                ' Render at a reduced internal resolution for speed, then upscale
                ' to the requested output size with bilinear interpolation.
                Dim bufferWidth = System.Math.Max(1, CInt(System.Math.Round(outputWidth * resolution)))
                Dim bufferHeight = System.Math.Max(1, CInt(System.Math.Round(outputHeight * resolution)))

                Dim pixelBuffer = renderToBuffer(scene, bufferWidth, bufferHeight, postProcessing)
                If bufferWidth <> outputWidth OrElse bufferHeight <> outputHeight Then
                    pixelBuffer = pixelBuffer.resize(outputWidth, outputHeight, True)
                End If

                Dim frameBuffer As BufferedImage = New BufferedImage(outputWidth, outputHeight, BufferedImage.TYPE_INT_RGB)
                For x = 0 To outputWidth - 1
                    For y = 0 To outputHeight - 1
                        Dim p = pixelBuffer.getPixel(x, y)
                        If p IsNot Nothing Then
                            frameBuffer.SetPixel(x, y, Drawing.Color.FromArgb(p.Color.RGB))
                        End If
                    Next
                Next

                Dim filePath = Path.Combine(outputDirectory, frame.ToString("D4") & ".png")
                frameBuffer.Save(filePath)

                Console.WriteLine("Rendered frame " & frame.ToString() & "/" & (frameCount - 1).ToString() & " -> " & filePath)
            Next
        End Sub

        ''' <summary>
        ''' Renders a single frame to a PixelBuffer, optionally applying the
        ''' bloom post-processing pipeline (reusing Renderer's bloom settings).
        ''' </summary>
        Private Shared Function renderToBuffer(scene As Scene, width As Integer, height As Integer, postProcessing As Boolean) As PixelBuffer
            Dim pixelBuffer = Renderer.renderScene(scene, width, height)

            If postProcessing Then
                Dim emissivePixels As PixelBuffer = pixelBuffer.clone()
                emissivePixels.filterByEmission(0.1F)

                Dim blur As GaussianBlur = New GaussianBlur(emissivePixels)
                blur.blur(Renderer.bloomRadius, 1)

                pixelBuffer.add(blur.PixelBuffer.multiply(Renderer.bloomIntensity).resize(width, height, True))
            End If

            Return pixelBuffer
        End Function
    End Class

End Namespace
