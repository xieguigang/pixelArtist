Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math
Imports PhysicsEngine.raytracing.math
Imports PhysicsEngine.raytracing.pixeldata
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush

Namespace raytracing.rendering

    Public Class Renderer
        Private Const GLOBAL_ILLUMINATION As Single = 0.3F
        Private Const SKY_EMISSION As Single = 0.5F
        Private Const MAX_REFLECTION_BOUNCES As Integer = 5
        Private Const SHOW_SKYBOX As Boolean = True

        Public Shared bloomIntensity As Single = 0.5F
        Public Shared bloomRadius As Integer = 10

        ''' <summary>
        ''' Renders the scene to a Pixel buffer </summary>
        ''' <param name="scene"> The scene to Render </param>
        ''' <param name="width"> The width of the desired output </param>
        ''' <param name="height"> The height of the desired output </param>
        ''' <returns> The rendered PixelBuffer </returns>
        Public Shared Function renderScene(scene As Scene, width As Integer, height As Integer) As PixelBuffer
            Dim pixelBuffer As PixelBuffer = New PixelBuffer(width, height)

            For X As Integer = 0 To width - 1
                For Y As Integer = 0 To height - 1
                    Dim screenUV = getNormalizedScreenCoordinates(X, Y, width, height)

                    pixelBuffer.setPixel(X, Y, computePixelInfo(scene, screenUV(0), screenUV(1)))
                Next
            Next

            Return pixelBuffer
        End Function

        ''' <summary>
        ''' Renders the scene to a Graphics object </summary>
        ''' <param name="scene"> The scene to Render </param>
        ''' <param name="width"> The width of the desired output </param>
        ''' <param name="height"> The height of the desired output </param>
        ''' <param name="resolution"> (Floating point greater than 0 and lower or equal to 1) Controls the number of rays traced. (1 = Every pixel is ray-traced) </param>
        Public Shared Sub renderScene(scene As Scene, gfx As IGraphics, width As Integer, height As Integer, resolution As Single)
            Dim blockSize As Integer = 1 / resolution

            Dim x = 0

            While x < width
                Dim y = 0

                While y < height
                    Dim uv = getNormalizedScreenCoordinates(x, y, width, height)
                    Dim pixelData = computePixelInfo(scene, uv(0), uv(1))

                    Dim c = Drawing.Color.FromArgb(CInt(pixelData.Color.Red) * 255, CInt(pixelData.Color.Green) * 255, CInt(pixelData.Color.Blue) * 255)
                    gfx.FillRectangle(New SolidBrush(c), x, y, blockSize, blockSize)
                    y += blockSize
                End While

                x += blockSize
            End While
        End Sub

        ''' <summary>
        ''' Same as the above but applies Post-Processing effects before drawing. </summary>
        Public Shared Sub renderScenePostProcessed(scene As Scene, gfx As IGraphics, width As Integer, height As Integer, resolution As Single)
            Dim bufferWidth As Integer = System.Math.Round(width * resolution + 0.49F, MidpointRounding.AwayFromZero)
            Dim bufferHeight As Integer = System.Math.Round(height * resolution + 0.49F, MidpointRounding.AwayFromZero)
            Dim pixelBuffer = renderScene(scene, bufferWidth, bufferHeight)

            Dim emissivePixels As PixelBuffer = pixelBuffer.clone() ' The width of this buffer has to remain constant to keep the blur factor the same for all sizes
            emissivePixels.filterByEmission(0.1F)
            Dim blur As GaussianBlur = New GaussianBlur(emissivePixels)
            blur.blur(bloomRadius, 1)
            pixelBuffer.add(blur.PixelBuffer.multiply(bloomIntensity).resize(bufferWidth, bufferHeight, True))

            Dim blockSize = 1 / resolution
            For X As Integer = 0 To bufferWidth - 1
                For Y As Integer = 0 To bufferHeight - 1
                    Dim pixel = pixelBuffer.getPixel(X, Y)
                    Dim c = Drawing.Color.FromArgb(CInt(pixel.Color.Red) * 255, CInt(pixel.Color.Green) * 255, CInt(pixel.Color.Blue) * 255)
                    gfx.FillRectangle(New SolidBrush(c), CInt(X * blockSize), CInt(Y * blockSize), CInt(blockSize) + 1, CInt(blockSize) + 1)
                Next
            Next

        End Sub

        ''' <summary>
        ''' Get OpenGL style screen coordinates where (0,0) is the center of the screen from x, y coordinates that start from the top-left corner of the screen </summary>
        Public Shared Function getNormalizedScreenCoordinates(x As Integer, y As Integer, width As Integer, height As Integer) As Single()
            Dim u As Single = 0, v As Single = 0
            If width > height Then
                u = CSng(x - width / 2 + height / 2) / height * 2 - 1
                v = -(CSng(y) / height * 2 - 1)
            Else
                u = CSng(x) / width * 2 - 1
                v = -(CSng(y - height / 2 + width / 2) / width * 2 - 1)
            End If

            Return New Single() {u, v}
        End Function

        Public Shared Function computePixelInfo(scene As Scene, u As Single, v As Single) As pixeldata.PixelData
            Dim eyePos As Vector3 = New Vector3(0, 0, -1 / System.Math.Tan(ToRadians(scene.Camera.FOV / 2)))
            Dim cam = scene.Camera

            Dim rayDir As Vector3 = (New Vector3(u, v, 0)).subtract(eyePos).normalize().rotateYP(cam.Yaw, cam.Pitch)
            Dim hit As RayHit = scene.raycast(New Ray(eyePos.add(cam.Position), rayDir))
            If hit IsNot Nothing Then
                Return computePixelInfoAtHit(scene, hit, MAX_REFLECTION_BOUNCES)
            ElseIf SHOW_SKYBOX Then
                Dim sbColor = scene.Skybox.getColor(rayDir)
                Return New pixeldata.PixelData(sbColor, Single.PositiveInfinity, sbColor.Luminance * SKY_EMISSION)
            Else
                Return New pixeldata.PixelData(Color.BLACK, Single.PositiveInfinity, 0)
            End If
        End Function

        Private Shared Function computePixelInfoAtHit(scene As Scene, hit As RayHit, recursionLimit As Integer) As pixeldata.PixelData
            Dim hitPos = hit.Position
            Dim rayDir = hit.Ray.Direction
            Dim hitSolid = hit.Solid
            Dim hitColor = hitSolid.getTextureColor(hitPos.subtract(hitSolid.Position))
            Dim brightness = getDiffuseBrightness(scene, hit)
            Dim specularBrightness = getSpecularBrightness(scene, hit)
            Dim reflectivity = hitSolid.Reflectivity
            Dim emission = hitSolid.Emission

            Dim reflection As pixeldata.PixelData
            Dim reflectionVector = rayDir.subtract(hit.Normal.multiply(2 * Vector3.dot(rayDir, hit.Normal)))
            Dim reflectionRayOrigin = hitPos.add(reflectionVector.multiply(0.001F)) ' Add a little to avoid hitting the same solid again
            Dim reflectionHit As RayHit = If(recursionLimit > 0, scene.raycast(New Ray(reflectionRayOrigin, reflectionVector)), Nothing)
            If reflectionHit IsNot Nothing Then
                reflection = computePixelInfoAtHit(scene, reflectionHit, recursionLimit - 1)
            Else
                Dim sbColor = scene.Skybox.getColor(reflectionVector)
                reflection = New pixeldata.PixelData(sbColor, Single.PositiveInfinity, sbColor.Luminance * SKY_EMISSION)
            End If

            Dim pixelColor = Color.lerp(hitColor, reflection.Color, reflectivity).multiply(brightness).add(specularBrightness).add(hitColor.multiply(emission)).add(reflection.Color.multiply(reflection.Emission * reflectivity)) ' Indirect illumination

            Return New pixeldata.PixelData(pixelColor, Vector3.distance(scene.Camera.Position, hitPos), System.Math.Min(1, emission + reflection.Emission * reflectivity + specularBrightness))
        End Function

        Private Shared Function getDiffuseBrightness(scene As Scene, hit As RayHit) As Single
            Dim sceneLight = scene.Light

            ' Raytrace to light to check if something blocks the light
            Dim lightBlocker As RayHit = scene.raycast(New Ray(sceneLight.Position, hit.Position.subtract(sceneLight.Position).normalize()))
            If lightBlocker IsNot Nothing AndAlso lightBlocker.Solid IsNot hit.Solid Then
                Return GLOBAL_ILLUMINATION ' GOBAL_ILLUMINATION = Minimum brightness
            Else
                Return System.Math.Max(GLOBAL_ILLUMINATION, System.Math.Min(1, Vector3.dot(hit.Normal, sceneLight.Position.subtract(hit.Position))))
            End If
        End Function

        Private Shared Function getSpecularBrightness(scene As Scene, hit As RayHit) As Single
            Dim hitPos = hit.Position
            Dim cameraDirection As Vector3 = scene.Camera.Position.subtract(hitPos).normalize()
            Dim lightDirection As Vector3 = hitPos.subtract(scene.Light.Position).normalize()
            Dim lightReflectionVector = lightDirection.subtract(hit.Normal.multiply(2 * Vector3.dot(lightDirection, hit.Normal)))

            Dim specularFactor = System.Math.Max(0, System.Math.Min(1, Vector3.dot(lightReflectionVector, cameraDirection)))
            Return CSng(System.Math.Pow(specularFactor, 2)) * hit.Solid.Reflectivity
        End Function
    End Class

End Namespace
