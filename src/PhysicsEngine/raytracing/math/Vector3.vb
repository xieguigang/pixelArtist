Imports Microsoft.VisualBasic.Math
Imports System

Namespace  raytracing.math
    Public Class Vector3
        Private xField, yField, zField As Single

        Public Sub New(x As Single, y As Single, z As Single)
            If Single.IsNaN(x) OrElse Single.IsNaN(y) OrElse Single.IsNaN(z) Then
                Throw New ArgumentException("One or more parameters are NaN")
            End If

            xField = x
            yField = y
            zField = z
        End Sub

        Public Overridable Property X As Single
            Get
                Return xField
            End Get
            Set(value As Single)
                xField = value
            End Set
        End Property

        Public Overridable Property Y As Single
            Get
                Return yField
            End Get
            Set(value As Single)
                yField = value
            End Set
        End Property

        Public Overridable Property Z As Single
            Get
                Return zField
            End Get
            Set(value As Single)
                zField = value
            End Set
        End Property




        Public Overridable Function add(vec As Vector3) As Vector3
            Return New Vector3(xField + vec.xField, yField + vec.yField, zField + vec.zField)
        End Function

        Public Overridable Function subtract(vec As Vector3) As Vector3
            Return New Vector3(xField - vec.xField, yField - vec.yField, zField - vec.zField)
        End Function

        Public Overridable Function multiply(scalar As Single) As Vector3
            Return New Vector3(xField * scalar, yField * scalar, zField * scalar)
        End Function

        Public Overridable Function multiply(vec As Vector3) As Vector3
            Return New Vector3(xField * vec.xField, yField * vec.yField, zField * vec.zField)
        End Function

        Public Overridable Function divide(vec As Vector3) As Vector3
            Return New Vector3(xField / vec.xField, yField / vec.yField, zField / vec.zField)
        End Function

        Public Overridable Function length() As Single
            Return System.Math.Sqrt(xField * xField + yField * yField + zField * zField)
        End Function

        Public Overridable Function normalize() As Vector3
            Dim length As Single = Me.length()
            Return New Vector3(xField / length, yField / length, zField / length)
        End Function

        Public Overridable Function rotateYP(yaw As Single, pitch As Single) As Vector3
            ' Convert to radians
            Dim yawRads = ToRadians(yaw)
            Dim pitchRads = ToRadians(pitch)

            ' Step one: Rotate around X axis (pitch)
            Dim _y As Single = yField * System.Math.Cos(pitchRads) - zField * System.Math.Sin(pitchRads)
            Dim _z As Single = yField * System.Math.Sin(pitchRads) + zField * System.Math.Cos(pitchRads)

            ' Step two: Rotate around the Y axis (yaw)
            Dim _x As Single = xField * System.Math.Cos(yawRads) + _z * System.Math.Sin(yawRads)
            _z = CSng(-xField * System.Math.Sin(yawRads) + _z * System.Math.Cos(yawRads))

            Return New Vector3(_x, _y, _z)
        End Function

        ''' <summary>
        ''' Does the same as Vector3.add but changes the vector itself instead of returning a new one </summary>
        Public Overridable Sub translate(vec As Vector3)
            xField += vec.xField
            yField += vec.yField
            zField += vec.zField
        End Sub

        Public Shared Function distance(a As Vector3, b As Vector3) As Single
            Return System.Math.Sqrt(System.Math.Pow(a.xField - b.xField, 2) + System.Math.Pow(a.yField - b.yField, 2) + System.Math.Pow(a.zField - b.zField, 2))
        End Function

        Public Shared Function dot(a As Vector3, b As Vector3) As Single
            Return a.xField * b.xField + a.yField * b.yField + a.zField * b.zField
        End Function

        Public Shared Function lerp(a As Vector3, b As Vector3, t As Single) As Vector3
            Return a.add(b.subtract(a).multiply(t))
        End Function

        Public Function clone() As Vector3
            Return New Vector3(xField, yField, zField)
        End Function

        Public Overrides Function ToString() As String
            Return "Vector3{" & "x=" & xField.ToString() & ", y=" & yField.ToString() & ", z=" & zField.ToString() & "}"c.ToString()
        End Function

        Public Overridable Function toArray() As Single()
            Return New Single() {xField, yField, zField}
        End Function
    End Class

End Namespace
