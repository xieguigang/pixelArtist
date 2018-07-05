Imports System.Drawing

''' <summary>
''' 
''' </summary>
Public Class Skeleton

    Public Property head As PointF
    Public Property face As PointF
    Public Property body As PointF
    Public Property hair_left As PointF
    Public Property hair_right As PointF
    Public Property hair_back As PointF
    Public Property left_hand As PointF
    Public Property right_hand As PointF
    Public Property foot_left As PointF
    Public Property foot_right As PointF

End Class

''' <summary>
''' 人物的图片材质数据结构
''' </summary>
Public Class Textures

    Public Property Emotions As Dictionary(Of Emotions, Bitmap)
    Public Property hair_left As Bitmap
    Public Property hair_right As Bitmap
    Public Property hair_back As Bitmap
    Public Property head As Bitmap
    Public Property body As Bitmap
    Public Property left_hand As Bitmap
    Public Property right_hand As Bitmap
    Public Property foot As Bitmap

End Class

Public Enum Skeletons
    head
    face
    body
    hair_left
    hair_right
    ''' <summary>
    ''' 两只脚好像是用同一个素材的
    ''' </summary>
    foot
    left_hand
    right_hand
End Enum