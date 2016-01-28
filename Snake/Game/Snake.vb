Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public Class Snake : Inherits GraphicUnit

    Public Property Direction As Controls
        Get
            Return _direction
        End Get
        Set(value As Controls)
            _direction = value

            Select Case Direction
                Case Controls.Down
                    dx = 0
                    dy = 1
                Case Controls.Left
                    dx = -1
                    dy = 0
                Case Controls.Right
                    dx = 1
                    dy = 0
                Case Controls.Up
                    dx = 0
                    dy = -1
            End Select
        End Set
    End Property

    Dim _direction As Controls
    Dim body As New List(Of Point)
    Dim size As Size = New Size(10, 10)

    Dim dx As Integer = 0, dy As Integer = 1

    Public Sub init()
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

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        SyncLock body
            For Each dot In body.Skip(1).ToArray
                Call g.FillRectangle(Brushes.Gray, New Rectangle(dot, size))
            Next
            Call g.FillRectangle(Brushes.Black, Head)

            Dim pre As Point = Location
            Location = New Point(Location.X + dx * size.Width, Location.Y + dy * size.Height)

            For i As Integer = 0 To body.Count - 1
                Dim tmp As Point = body(i)
                body(i) = pre
                pre = tmp
            Next
        End SyncLock
    End Sub

    Protected Overrides Function __getSize() As Size
        Return size
    End Function
End Class
