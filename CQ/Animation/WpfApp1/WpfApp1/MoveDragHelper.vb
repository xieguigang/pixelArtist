Public Class MoveDragHelper

    Dim WithEvents display As Image

    Dim mouseX As Double
    Dim mouseY As Double
    Dim offset As Point
    Dim this As Window

    Sub New(display As Image, this As Window)
        Me.display = display
        Me.this = this
    End Sub

    ''' <summary>
    ''' Get the Position of Window so that it will set margin from this window
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Display_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles display.MouseLeftButtonUp
        With e.GetPosition(this)
            mouseX = .X
            mouseY = .Y
        End With

        offset = e.GetPosition(display)
    End Sub

    Private Sub Display_MouseMove(sender As Object, e As MouseEventArgs) Handles display.MouseMove
        If (e.LeftButton <> MouseButtonState.Pressed) Then
            Return
        Else
            ' Capture the mouse for border
            e.MouseDevice.Capture(display)
        End If

        Dim current = e.GetPosition(this)
        Dim dx = -mouseX + current.X
        Dim dy = -mouseY + current.Y
        Dim location = display.Margin

        location.Left += dx - offset.X
        location.Top += dy - offset.Y
        display.Margin = location

        mouseX = location.Left
        mouseY = location.Top
    End Sub

    Private Sub Display_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles display.MouseUp
        e.MouseDevice.Capture(Nothing)
    End Sub
End Class
