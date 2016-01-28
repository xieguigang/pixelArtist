Imports Microsoft.VisualBasic.GamePads.Abstract

Namespace Commons

    Public Class Score : Inherits GraphicUnit
        Implements IScore

        Public Property Highest As Integer Implements IScore.Highest
        Public Property Current As Integer Implements IScore.Score

        ReadOnly Font As New Font("Tahoma", 16, FontStyle.Bold)

        Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
            Dim s As String = "HI  " & ZeroFill(Highest, 5)
            Dim sz = g.Gr_Device.MeasureString(s, Font)
            Call g.Gr_Device.DrawString(s, Font, Brushes.Gray, Location)
            Call g.Gr_Device.DrawString(ZeroFill(Current, 5), Font, Brushes.Black, New Point(Location.X + 10 + sz.Width, Location.Y))
        End Sub

        Protected Overrides Function __getSize() As Size
            Return New Size
        End Function
    End Class
End Namespace