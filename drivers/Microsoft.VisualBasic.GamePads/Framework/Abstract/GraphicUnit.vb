Imports Microsoft.VisualBasic.Imaging

Namespace Abstract

    Public MustInherit Class GraphicUnit

        Public Property Location As Point

        Public ReadOnly Property OutBounds As Boolean
            Get
                Return Region.Right < 0
            End Get
        End Property

        Public ReadOnly Property Region As Rectangle
            Get
                Return New Rectangle(Location, __getSize)
            End Get
        End Property

        Public Sub OffsetHeight()
            Location = New Point(Location.X, Location.Y - __getSize.Height)
        End Sub

        Protected MustOverride Function __getSize() As Size

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="size">Graphics device size</param>
        Public MustOverride Sub Draw(ByRef g As Graphics, size As Size)
    End Class
End Namespace