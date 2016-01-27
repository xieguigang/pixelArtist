Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract

''' <summary>
''' 仙人掌
''' </summary>
Public Class Cactus : Inherits GraphicUnit

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        Call g.Gr_Device.DrawImageUnscaled(My.Resources.xrzh, Location)
        Location = New Point(Location.X - 5, Location.Y)
    End Sub

    Protected Overrides Function __getSize() As Size
        Return My.Resources.xrzh.Size
    End Function
End Class
