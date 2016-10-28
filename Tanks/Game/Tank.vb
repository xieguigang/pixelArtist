Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.Imaging

Public Class Tank : Inherits GraphicUnit

    Dim model As Image
    Dim direct As Controls

    Public Property Direction As Controls
        Get
            Return direct
        End Get
        Set(value As Controls)
            If value <> direct Then  ' 转向
                Select Case value
                    Case Controls.Down
                        model = My.Resources.user_down
                    Case Controls.Left
                        model = My.Resources.user_left
                    Case Controls.Right
                        model = My.Resources.user_right
                    Case Controls.Up
                        model = My.Resources.user_up
                End Select
                direct = value
            Else  ' 前进

                Select Case direct
                    Case Controls.Up
                        Location = New Point(Location.X, Location.Y - d)
                    Case Controls.Right
                        Location = New Point(Location.X + d, Location.Y)
                    Case Controls.Left
                        Location = New Point(Location.X - d, Location.Y)
                    Case Controls.Down
                        Location = New Point(Location.X, Location.Y + d)
                End Select

            End If
        End Set
    End Property

    Dim d As Integer = 10

    Sub New()
        Direction = Controls.Up
        model = My.Resources.user_up
    End Sub

    Public Overrides Sub Draw(ByRef g As GDIPlusDeviceHandle)
        Call g.DrawImageUnscaled(model, Location)
    End Sub

    Protected Overrides Function __getSize() As Size
        Return model.Size
    End Function
End Class
