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

        Public MustOverride Sub Draw(ByRef g As GDIPlusDeviceHandle)
    End Class
End Namespace