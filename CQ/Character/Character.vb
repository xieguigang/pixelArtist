Imports System.Drawing

Public Class Character

    Public Property emotions As Bitmap()
    Public Property head As Bitmap
    Public Property body As Bitmap
    Public Property leftHair As Bitmap
    Public Property rightHair As Bitmap
    Public Property foot As Bitmap
    Public Property frontHand As Bitmap
    Public Property backHand As Bitmap

    Sub New(spirit As Bitmap)
        emotions = spirit.Faces.ToArray
        head = spirit.Head
        body = spirit.Body
        foot = spirit.Foot
        frontHand = spirit.FrontHand
        backHand = spirit.BackHand

        With spirit.Hairs.ToArray
            leftHair = .GetValue(0)
            rightHair = .GetValue(1)
        End With
    End Sub

End Class
