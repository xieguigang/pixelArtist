Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic

Public Class Snake : Inherits GraphicUnit

    Public Property Direction As Controls
        Get
            Return _direction
        End Get
        Set(value As Controls)
            Call __setDirection(value)
        End Set
    End Property

    Private Sub __setDirection(value As Controls)
        Dim pre = Direction

        Select Case value
            Case Controls.Down

                If pre = Controls.Up Then
                    Return
                End If

                dx = 0
                dy = 1
            Case Controls.Left

                If pre = Controls.Right Then
                    Return
                End If

                dx = -1
                dy = 0
            Case Controls.Right

                If pre = Controls.Left Then
                    Return
                End If

                dx = 1
                dy = 0
            Case Controls.Up

                If pre = Controls.Down Then
                    Return
                End If

                dx = 0
                dy = -1

            Case Else

                Dim dd As Controls = Controls.NotBind

                If value.HasFlag(Controls.Up) Then
                    dd = Controls.Up
                ElseIf value.HasFlag(Controls.Down) Then
                    dd = Controls.Down
                ElseIf value.HasFlag(Controls.Left) Then
                    dd = Controls.Left
                ElseIf value.HasFlag(Controls.Right) Then
                    dd = Controls.Right
                End If

                If dd = Controls.NotBind Then
                    Return
                Else
                    Call __setDirection(dd)
                End If
        End Select

        _direction = value
    End Sub

    Dim _direction As Controls
    Dim body As New List(Of Point)
    Dim size As Size = New Size(10, 10)

    Dim dx As Integer = 0, dy As Integer = 1

    Public Sub init()
        Call body.Clear()
        For i As Integer = 0 To 10
            Call body.Add(New Point(0, Location.Y - i * size.Height))
        Next
    End Sub

    Public ReadOnly Property Head As Rectangle
        Get
            Return New Rectangle(Location, size)
        End Get
    End Property

    Public Sub Append()
        Call body.Add(body.Last)
    End Sub

    ''' <summary>
    ''' 1-100
    ''' </summary>
    ''' <returns></returns>
    Public Property MoveSpeed As Double
        Get
            Return _speed * 100
        End Get
        Set(value As Double)
            _speed = value / 100
        End Set
    End Property

    Dim _speed As Double

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        SyncLock body
            For Each dot In body.Skip(1).ToArray
                Call g.FillRectangle(Brushes.Gray, New Rectangle(dot, size))
            Next
            Call g.FillRectangle(Brushes.Black, Head)

            Dim pre As Point = Location
            Location = New Point(Location.X + dx * size.Width * _speed, Location.Y + dy * size.Height * _speed)

            For i As Integer = 0 To body.Count - 1
                Dim tmp As Point = body(i)
                body(i) = pre
                pre = tmp
            Next
        End SyncLock
    End Sub

    ''' <summary>
    ''' 吃自己的身体？？
    ''' </summary>
    ''' <returns></returns>
    Public Function EatSelf() As Boolean
        SyncLock body
            Dim hd = Head

            For Each x In body.ToArray
                If hd.Contains(x) Then
                    Return True
                End If
            Next
        End SyncLock

        Return False
    End Function

    Protected Overrides Function __getSize() As Size
        Return size
    End Function
End Class
