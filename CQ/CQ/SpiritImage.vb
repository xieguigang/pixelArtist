Imports System.Drawing

''' <summary>
''' 精灵图模型，使用这个类来对人物的模型精灵图进行和切割操作
''' </summary>
Public Class SpiritImage

    Public Property Emotions As Dictionary(Of Emotions, RectangleF)
    Public Property hair_left As RectangleF
    Public Property hair_right As RectangleF
    Public Property hair_back As RectangleF
    Public Property head As RectangleF
    Public Property body As RectangleF
    Public Property left_hand As RectangleF
    Public Property right_hand As RectangleF
    Public Property foot As RectangleF

    Public Function Slice(texture As Bitmap) As Textures
        SyncLock texture

        End SyncLock
    End Function
End Class

Public Enum Emotions As Integer
    ''' <summary>
    ''' 左上
    ''' </summary>
    terrified
    ''' <summary>
    ''' 右上
    ''' </summary>
    smile
    ''' <summary>
    ''' 左中
    ''' </summary>
    boring
    ''' <summary>
    ''' 右中
    ''' </summary>
    normal
    ''' <summary>
    ''' 左下
    ''' </summary>
    serious
    ''' <summary>
    ''' 右下
    ''' </summary>
    angry
End Enum