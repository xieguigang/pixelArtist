Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.Imaging

Public Class Dinosaur : Inherits GraphicUnit

    Dim images As New LoopArray(Of Image)({My.Resources.walk1, My.Resources.walk2})
    Dim size As Size

    Sub New()
        size = images.GET.Size
    End Sub

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        Dim res As Image = images.GET
        size = res.Size
        Call g.Graphics.DrawImageUnscaled(res, Location)
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