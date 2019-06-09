Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.BitmapImage

Public Module Slicer

    Const width = 256
    Const height = 256

    <Extension>
    Public Function Head(spirit As Bitmap) As Bitmap
        Return spirit.ImageCrop(New Point(width * 0.225, height * 0.38), New Size(width * 0.5, height * 0.4))
    End Function

    Dim faceSize As New Size(40, 25)

    <Extension>
    Public Iterator Function Faces(spirit As Bitmap) As IEnumerable(Of Bitmap)
        ' A B
        ' C D
        ' E F
        Dim x, x1, y As Integer

        y = height * 0.01
        x = width * 0.64
        x1 = x + faceSize.Width + 3

        ' A
        Yield spirit.ImageCrop(New Point(x, y), faceSize)
        ' B
        Yield spirit.ImageCrop(New Point(x1, y), faceSize)

        y = height * 0.14

        ' C
        Yield spirit.ImageCrop(New Point(x, y), faceSize)
        ' D
        Yield spirit.ImageCrop(New Point(x1, y), faceSize)

        y = height * 0.26

        ' E
        Yield spirit.ImageCrop(New Point(x, y), faceSize)
        ' F
        Yield spirit.ImageCrop(New Point(x1, y), faceSize)
    End Function

    <Extension>
    Public Function Body(spirit As Bitmap) As Bitmap
        Return spirit.ImageCrop(New Point(width * 0.54, height * 0.833), New Size(width * 0.15, height * 0.1))
    End Function

    <Extension>
    Public Iterator Function Hairs(spirit As Bitmap) As IEnumerable(Of Bitmap)
        ' left
        Yield spirit.ImageCrop(New Point(width * 0.09, height * 0.1), New Size(width * 0.3, height * 0.25))

        ' right
        Yield spirit.ImageCrop(New Point(width * 0.35, height * 0.1), New Size(width * 0.25, height * 0.25))
    End Function

    <Extension>
    Public Function Foot(spirit As Bitmap) As Bitmap
        Return spirit.ImageCrop(New Point(width * 0.94, height * 0.885), New Size(width * 0.03, height * 0.055))
    End Function

    <Extension>
    Public Function FrontHand(spirit As Bitmap) As Bitmap
        Return spirit.ImageCrop(New Point(width * 0.16, height * 0.885), New Size(width * 0.05, height * 0.055))
    End Function

    <Extension>
    Public Function BackHand(spirit As Bitmap) As Bitmap
        Return spirit.ImageCrop(New Point(width * 0.04, height * 0.89), New Size(width * 0.04, height * 0.05))
    End Function
End Module
