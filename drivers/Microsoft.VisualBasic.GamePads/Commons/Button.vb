Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.Imaging

Namespace Commons

    Public Class Button : Inherits GraphicUnit

        Dim UI As Image

        Sub New(ui As Image)
            SyncLock ui
                Me.UI = DirectCast(New Bitmap(ui).Clone, Bitmap)
            End SyncLock
        End Sub

        Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
            SyncLock g
                SyncLock UI
                    Call g.Graphics.DrawImageUnscaled(UI, Location)
                End SyncLock
            End SyncLock
        End Sub

        Protected Overrides Function __getSize() As Size
            Return UI.Size
        End Function
    End Class
End Namespace