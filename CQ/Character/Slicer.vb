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

    Dim faceSize As New Size(50, 30)

    <Extension>
    Public Iterator Function Faces(spirit As Bitmap) As IEnumerable(Of Bitmap)
        ' A B
        ' C D
        ' E F

        ' A
        Yield spirit.ImageCrop(New Point(width * 0.6, height * 0.1), faceSize)
        ' B
        Yield spirit.ImageCrop(New Point(width * 0.6 + faceSize.Width, height * 0.1), faceSize)
        ' C
        Yield spirit.ImageCrop(New Point(width * 0.6, height * 0.2), faceSize)
        ' D
        Yield spirit.ImageCrop(New Point(width * 0.6 + faceSize.Width, height * 0.2), faceSize)
        ' E
        Yield spirit.ImageCrop(New Point(width * 0.6, height * 0.3), faceSize)
        ' F
        Yield spirit.ImageCrop(New Point(width * 0.6 + faceSize.Width, height * 0.3), faceSize)
    End Function
End Module
