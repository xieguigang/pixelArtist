Imports System.Drawing
Imports System.IO
Imports Astrophysics.raytracing.pixeldata
Imports BufferedImage = Microsoft.VisualBasic.Imaging.BitmapImage.BitmapBuffer
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.rendering


    Public Class Skybox
        Private sphereImage As BufferedImage
        Private loadedField As Boolean

        Public Sub New(resourceName As String)
            load(resourceName)
        End Sub

        Private Sub load(resourceName As String)
            If String.IsNullOrEmpty(resourceName) Then
                sphereImage = Nothing
                loadedField = False
                Return
            End If

            Try
                sphereImage = BufferedImage.FromImage(Image.FromFile(resourceName))
                loadedField = True
            Catch e As Exception
                sphereImage = Nothing
                loadedField = False
                Console.WriteLine("Failed to load skybox '" & resourceName & "': " & e.Message)
            End Try
        End Sub

        Public Overridable Function getColor(d As vec3) As Color
            If sphereImage Is Nothing Then
                Return defaultSkyColor(d)
            End If

            ' Convert Unit vector to texture coordinates (Wikipedia UV Unwrapping page)
            Dim u As Single = 0.5 + System.Math.Atan2(d.Z, d.X) / (2 * System.Math.PI)
            Dim v As Single = 0.5 - System.Math.Asin(d.Y) / System.Math.PI

            Try
                Dim px = CInt(System.Math.Round(u * (sphereImage.Width - 1)))
                Dim py = CInt(System.Math.Round(v * (sphereImage.Height - 1)))
                px = System.Math.Max(0, System.Math.Min(sphereImage.Width - 1, px))
                py = System.Math.Max(0, System.Math.Min(sphereImage.Height - 1, py))

                Return sphereImage.GetPixel(px, py)
            Catch e As Exception
                Console.WriteLine(e.ToString())
                Return defaultSkyColor(d)
            End Try
        End Function

        ' Safe fallback so a missing/unloaded skybox never throws.
        Private Function defaultSkyColor(d As vec3) As Color
            Dim t As Single = System.Math.Max(0, System.Math.Min(1, d.Y * 0.5F + 0.5F))
            Return Color.lerp(New Color(0.1F, 0.2F, 0.4F), New Color(0.5F, 0.7F, 1.0F), t)
        End Function

        Public Overridable ReadOnly Property Loaded As Boolean
            Get
                Return loadedField
            End Get
        End Property

        Public Overridable Sub reload(resourceName As String)
            load(resourceName)
        End Sub

        Public Overridable Sub reloadFromFile(file As FileStream)
            If file IsNot Nothing Then
                load(file.Name)
            End If
        End Sub
    End Class

End Namespace
