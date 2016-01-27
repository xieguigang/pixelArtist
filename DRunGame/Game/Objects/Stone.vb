Imports Microsoft.VisualBasic.GamePads.Abstract

Public Class Stone : Inherits GraphicUnit

    Dim size As New Size(30, 45)
    Dim res As Image

    Sub New()
        Dim x = size.Width * RandomDouble()
        Dim y = size.Height * RandomDouble()
        Dim width = size.Width * RandomDouble()
        Dim dots = 5 * RandomDouble()

        Dim gr = size.CreateGDIDevice(Color.Transparent)

        Call gr.Gr_Device.DrawLine(Pens.Black, New Point(x, y), New Point(x + width, y))

        For i As Integer = 0 To dots
            x = size.Width * RandomDouble()
            y = size.Height * RandomDouble()
            Call gr.Gr_Device.DrawLine(Pens.Black, New Point(x, y), New Point(x + 1, y))
        Next

        res = gr.ImageResource
    End Sub

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        Call g.Gr_Device.DrawImageUnscaled(res, Location)
        Location = New Point(Location.X - 3, Location.Y)
    End Sub

    Protected Overrides Function __getSize() As Size
        Return size
    End Function
End Class
