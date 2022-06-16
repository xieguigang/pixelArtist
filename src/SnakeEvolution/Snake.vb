Imports PixelArtist.Engine

Public Class Snake

    Dim bodyX As List(Of Integer)
    Dim bodyY As List(Of Integer)
    Dim speedX As Integer
    Dim speedY As Integer

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

    Public Sub Move()
        Call Move(speedX, speedY)
    End Sub

    Public Sub Move(dx As Integer, dy As Integer)
        Dim xi As Integer = bodyX(0)
        Dim yi As Integer = bodyY(0)

        speedX = dx
        speedY = dy
        bodyX(0) += dx
        bodyY(0) += dy

        For i As Integer = 1 To bodyX.Count - 1
            Dim tx = bodyX(i)
            Dim ty = bodyY(i)

            bodyX(i) = xi
            bodyY(i) = yi
            xi = tx
            yi = ty
        Next
    End Sub

    Public Sub Draw(g As PixelGraphics)
        For i As Integer = 0 To bodyX.Count - 1
            Call g.DrawPixel(bodyX(i), bodyY(i))
        Next
    End Sub

End Class
