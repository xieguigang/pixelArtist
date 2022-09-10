Imports PixelArtist.Engine

Public Class Food : Inherits CharacterModel

    Dim posX As Integer
    Dim posY As Integer

    Sub New(x As Integer, y As Integer)
        posX = x
        posY = y
    End Sub

    Sub New(pos As Point)
        Call Me.New(pos.X, pos.Y)
    End Sub

    Public Function Check(snake As Snake) As Boolean
        Dim snakeHead = snake.Head

        Return posX = snakeHead.X AndAlso posY = snakeHead.Y
    End Function

    Public Overrides Sub Draw(g As PixelGraphics)
        Call g.DrawPixel(posX, posY, Color.Red)
    End Sub
End Class
