Imports Microsoft.VisualBasic.GamePads.Abstract

Public Class Food : Inherits GraphicUnit

    Dim size As New Size(15, 15)

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        Call g.Gr_Device.FillRectangle(Brushes.Red, Me.Region)
    End Sub

    Protected Overrides Function __getSize() As Size
        Return size
    End Function
End Class
