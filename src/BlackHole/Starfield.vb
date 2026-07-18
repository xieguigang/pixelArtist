Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports vec3 = Microsoft.VisualBasic.Imaging.Drawing3D.Point3D
Imports Astrophysics.raytracing.rendering
Imports pColor = Astrophysics.raytracing.pixeldata.Color



    ''' <summary>
    ''' Background sky: a procedural star field (deterministic) with a faint nebula gradient.
    ''' If a skybox texture path is supplied and loads successfully, the skybox is sampled
    ''' instead. Used for photons that escape the black hole.
    ''' </summary>
    Public Class Starfield

        Private Class Star
            Public Dir As vec3
            Public Mag As Double
            Public Col As pColor
        End Class

        Private stars As New List(Of Star)()
        Private skybox As Skybox = Nothing

        Public Sub New(Optional starCount As Integer = 450, Optional skyboxPath As String = Nothing)
            Dim rnd = New System.Random(1337)
            For i = 0 To starCount - 1
                ' Uniform point on the unit sphere.
                Dim z = rnd.NextDouble() * 2 - 1
                Dim phi = rnd.NextDouble() * 2 * System.Math.PI
                Dim s = System.Math.Sqrt(1 - z * z)
                Dim dir = New vec3(s * System.Math.Cos(phi), z, s * System.Math.Sin(phi))

                ' Magnitude: many faint stars, few bright ones.
                Dim mag = System.Math.Pow(rnd.NextDouble(), 3.0) * 0.9 + 0.05
                ' Slight colour temperature variation.
                Dim kelvin = 3500 + rnd.NextDouble() * 7000
                Dim col = BlackBody.Color(kelvin)

                stars.Add(New Star With {.Dir = dir, .Mag = mag, .Col = col})
            Next

            If Not String.IsNullOrEmpty(skyboxPath) Then
                Try
                    skybox = New Skybox(skyboxPath)
                Catch
                    skybox = Nothing
                End Try
            End If
        End Sub

        ''' <summary>Sample the background in the given (unit) escape direction.</summary>
        Public Function GetColor(dir As vec3) As pColor
            If skybox IsNot Nothing Then
                Return skybox.getColor(dir)
            End If

            Dim d = dir.Normalize()
            Dim r = 0.012F
            Dim g = 0.016F
            Dim b = 0.03F

            ' Faint nebula band along the galactic equator (y ~ 0).
            Dim band = System.Math.Exp(-(d.Y * d.Y) / 0.05)
            r += CSng(0.02 * band)
            g += CSng(0.015 * band)
            b += CSng(0.035 * band)

            For Each st In stars
                Dim c = vec3.Dot(d, st.Dir)
                If c > 0.99995 Then
                    Dim k = CSng((c - 0.99995) / 0.00005)
                    k = System.Math.Min(1, k)
                    r += CSng(st.Col.Red * st.Mag * k)
                    g += CSng(st.Col.Green * st.Mag * k)
                    b += CSng(st.Col.Blue * st.Mag * k)
                End If
            Next

            Return BlackBody.Safe(r, g, b)
        End Function

    End Class

