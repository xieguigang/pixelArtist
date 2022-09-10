Imports System.Runtime.CompilerServices
Imports PixelArtist.Engine

Public Class Food : Inherits CharacterModel

    Dim posX As Integer
    Dim posY As Integer

    Public ReadOnly Property Location As Point
        Get
            Return New Point(posX, posY)
        End Get
    End Property

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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Sub Draw(g As PixelGraphics)
        Call g.DrawPixel(posX, posY, Color.Red)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Iterator Function GetPixels(pixelScale As SizeF) As IEnumerable(Of Rectangle)
        Yield New Rectangle(posX, posY, pixelScale.Width, pixelScale.Height)
    End Function
End Class
