Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract

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

Public Class Dragon : Inherits GraphicUnit

    Dim images As New LoopArray(Of Image)({My.Resources.walk1, My.Resources.walk2})
    Dim size As Size

    Sub New()
        size = images.GET.Size
    End Sub

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        Dim res As Image = images.GET
        size = res.Size
        Call g.Gr_Device.DrawImageUnscaled(res, Location)
    End Sub

    Dim jumps As Boolean

    Public Sub Jump()
        If Not jumps Then
            jumps = True
        Else
            Return
        End If

        For i As Integer = 0 To 25
            Location = New Point(Location.X, Location.Y - 6)
            Threading.Thread.Sleep(30)
        Next
        For i As Integer = 0 To 25
            Location = New Point(Location.X, Location.Y + 6)
            Threading.Thread.Sleep(30)
        Next

        jumps = False
    End Sub

    Protected Overrides Function __getSize() As Size
        Return size
    End Function
End Class