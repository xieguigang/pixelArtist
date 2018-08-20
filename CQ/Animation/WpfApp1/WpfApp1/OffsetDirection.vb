Public Enum OffsetDirections
    top
    bottom
    left
    right
End Enum

Public Class Offset

    Public Property direction As OffsetDirections
    Public Property pixels As Integer

    Public Sub DoOffset(canvas As Image)
        Dim location = canvas.Margin

        Select Case direction
            Case OffsetDirections.left
                location.Left -= pixels
            Case OffsetDirections.right
                location.Left += pixels
            Case OffsetDirections.top
                location.Top -= pixels
            Case OffsetDirections.bottom
                location.Top += pixels
            Case Else
                Throw New NotImplementedException
        End Select

        canvas.Margin = location
    End Sub
End Class