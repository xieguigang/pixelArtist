Imports PixelArtist.Engine

Public Class Snake

    Dim bodyX As List(Of Integer)
    Dim bodyY As List(Of Integer)

    Sub New(head As Point, len As Integer)
        bodyX = New List(Of Integer)
        bodyY = New List(Of Integer)

        bodyX.Add(head.X)
        bodyY.Add(head.Y)

        For i As Integer = 1 To len
            bodyX.Add(head.X + i)
            bodyY.Add(head.Y)
        Next
    End Sub

    Public Sub Draw(g As PixelGraphics)
        For i As Integer = 0 To bodyX.Count - 1
            Call g.DrawPixel(bodyX(i), bodyY(i))
        Next
    End Sub

End Class
