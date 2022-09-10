Public MustInherit Class CharacterModel

    ''' <summary>
    ''' do model rendering on the screen device
    ''' </summary>
    ''' <param name="g"></param>
    Public MustOverride Sub Draw(g As PixelGraphics)
    Public MustOverride Function GetPixels(pixelScale As SizeF) As IEnumerable(Of Rectangle)

End Class
