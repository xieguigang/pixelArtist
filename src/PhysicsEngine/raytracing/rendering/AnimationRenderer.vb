Imports System.IO
Imports Microsoft.VisualBasic.Imaging.Drawing3D
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

        Public Shared Sub renderImageSequence(scene As Scene, outputDirectory As FileStream, outputWidth As Integer, outputHeight As Integer, frameCount As Integer, resolution As Single, postProcessing As Boolean)
            For frame = 0 To frameCount - 1
                Dim t = CSng(frame) / (frameCount - 1)
                Dim position = vec3.Lerp(firstPosition, secondPosition, t)
                Dim yaw = firstYaw + (secondYaw - firstYaw) * t
                Dim pitch = firstPitch + (secondPitch - firstPitch) * t

                Dim cam = scene.Camera
                cam.position = position
                cam.AngleY = yaw
                cam.AngleX = pitch
                Dim frameBuffer As BufferedImage = New BufferedImage(outputWidth, outputHeight, BufferedImage.TYPE_INT_RGB)

                ' Renderer.renderScenePostProcessed(scene, frameBuffer.Graphics, outputWidth, outputHeight, resolution);
                If postProcessing Then
                Else
                    ' Renderer.renderScene(scene, frameBuffer.Graphics, outputWidth, outputHeight, resolution);
                End If

                ' ImageIO.write(frameBuffer, "PNG", new File(outputDirectory, frame + ".png"));

                Console.WriteLine("Rendered frame " & frame.ToString() & "/" & (frameCount - 1).ToString())
            Next
        End Sub
    End Class

End Namespace
