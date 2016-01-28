Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public Class Snake : Inherits GraphicUnit

    Public Property Direction As Controls

    Dim body As New List(Of Point)
    Dim size As Size = New Size(10, 10)

    Sub New()
        For i As Integer = 0 To 5
            Call body.Add(New Point(0, i - 1))
        Next
    End Sub

    Public ReadOnly Property Head As Rectangle
        Get
            Return New Rectangle(Location, New Size(15, 15))
        End Get
    End Property

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        For Each dot In body.Skip(1)
            dot = New Point(Location.X + dot.X * size.Width, Location.Y + dot.Y * size.Height)
            Call g.FillRectangle(Brushes.Gray, New Rectangle(dot, size))
        Next
        Call g.FillRectangle(Brushes.Black, Head)

        Select Case Direction
            Case Controls.Down

            Case Controls.Left
            Case Controls.Right
            Case Controls.Up
        End Select
    End Sub

    Protected Overrides Function __getSize() As Size
        Return size
    End Function
End Class
