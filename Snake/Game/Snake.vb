Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic

Public Class Snake : Inherits GraphicUnit

    Public Property Direction As GamePads.EngineParts.Controls
        Get
            Return _direction
        End Get
        Set(value As GamePads.EngineParts.Controls)
            Call __setDirection(value)
        End Set
    End Property

    Private Sub __setDirection(value As GamePads.EngineParts.Controls)
        Dim pre = Direction

        Select Case value
            Case GamePads.EngineParts.Controls.Down

                If pre = GamePads.EngineParts.Controls.Up Then
                    Return
                End If

                dx = 0
                dy = 1
            Case GamePads.EngineParts.Controls.Left

                If pre = GamePads.EngineParts.Controls.Right Then
                    Return
                End If

                dx = -1
                dy = 0
            Case GamePads.EngineParts.Controls.Right

                If pre = GamePads.EngineParts.Controls.Left Then
                    Return
                End If

                dx = 1
                dy = 0
            Case GamePads.EngineParts.Controls.Up

                If pre = GamePads.EngineParts.Controls.Down Then
                    Return
                End If

                dx = 0
                dy = -1

            Case Else

                Dim dd As GamePads.EngineParts.Controls = GamePads.EngineParts.Controls.NotBind

                If value.HasFlag(GamePads.EngineParts.Controls.Up) Then
                    dd = GamePads.EngineParts.Controls.Up
                ElseIf value.HasFlag(GamePads.EngineParts.Controls.Down) Then
                    dd = GamePads.EngineParts.Controls.Down
                ElseIf value.HasFlag(GamePads.EngineParts.Controls.Left) Then
                    dd = GamePads.EngineParts.Controls.Left
                ElseIf value.HasFlag(GamePads.EngineParts.Controls.Right) Then
                    dd = GamePads.EngineParts.Controls.Right
                End If

                If dd = GamePads.EngineParts.Controls.NotBind Then
                    Return
                Else
                    Call __setDirection(dd)
                End If
        End Select

        _direction = value
    End Sub

    Dim _direction As GamePads.EngineParts.Controls
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
        SyncLock body
            Call body.Add(body.Last)
        End SyncLock
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
