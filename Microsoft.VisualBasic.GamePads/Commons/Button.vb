Imports Microsoft.VisualBasic.GamePads.Abstract

Namespace Commons

    Public Class Button : Inherits GraphicUnit

        Dim UI As Image

        Sub New(ui As Image)
            Me.UI = ui
        End Sub

        Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
            Call g.Gr_Device.DrawImageUnscaled(UI, Location)
        End Sub

        Protected Overrides Function __getSize() As Size
            Return UI.Size
        End Function
    End Class
End Namespace