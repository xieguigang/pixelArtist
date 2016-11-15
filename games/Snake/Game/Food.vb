Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.Imaging

Public Class Food : Inherits GraphicUnit

    Dim size As New Size(15, 15)

    Public Overrides Sub Draw(ByRef g As Graphics, rect As Size)
        Call g.FillRectangle(Brushes.Red, Me.Region)
    End Sub

    Protected Overrides Function __getSize() As Size
        Return size
    End Function
End Class
