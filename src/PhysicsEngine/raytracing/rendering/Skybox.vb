Imports System.IO
Imports PhysicsEngine.raytracing.pixeldata
Imports BufferedImage = Microsoft.VisualBasic.Imaging.BitmapImage.BitmapBuffer
Imports Vector3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D

Namespace raytracing.rendering


    Public Class Skybox
        Private sphereImage As BufferedImage
        Private loadedField As Boolean

        Public Sub New(resourceName As String)
        End Sub

        Public Overridable Function getColor(d As Vector3) As Color
            ' Convert Unit vector to texture coordinates (Wikipedia UV Unwrapping page)
            Dim u As Single = 0.5 + System.Math.Atan2(d.Z, d.X) / (2 * System.Math.PI)
            Dim v As Single = 0.5 - System.Math.Asin(d.Y) / System.Math.PI

            Try
                Return sphereImage.GetPixel(u * (sphereImage.Width - 1), v * (sphereImage.Height - 1))
            Catch e As Exception
                Console.WriteLine("U: " & u.ToString() & " V: " & v.ToString())
                Console.WriteLine(e.ToString())
                Console.Write(e.StackTrace)

                Return Color.MAGENTA
            End Try
        End Function

        Public Overridable ReadOnly Property Loaded As Boolean
            Get
                Return loadedField
            End Get
        End Property

        Public Overridable Sub reload(resourceName As String)
            sphereImage = New BufferedImage(2, 2, BufferedImage.TYPE_INT_RGB)
            loadedField = False
        End Sub


        Public Overridable Sub reloadFromFile(file As FileStream)
            sphereImage = New BufferedImage(2, 2, BufferedImage.TYPE_INT_RGB)
            loadedField = False
        End Sub
    End Class

End Namespace
