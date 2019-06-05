Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.BitmapImage

Public Module Slicer

    Const width = 256
    Const height = 256

    <Extension>
    Public Function Head(spirit As Bitmap) As Bitmap
        Return spirit.ImageCrop(New Point(width * 0.3, height * 0.3), New Size(width * 0.1, height * 0.1))
    End Function
End Module
