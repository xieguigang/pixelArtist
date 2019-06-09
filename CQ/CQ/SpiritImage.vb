Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Language.Default

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
            Return New Textures With {
                .body = texture.ImageCrop(rect:=head),
                .head = texture.ImageCrop(rect:=head),
                .foot = texture.ImageCrop(rect:=foot),
                .hair_back = texture.ImageCrop(rect:=hair_back),
                .hair_left = texture.ImageCrop(rect:=hair_left),
                .hair_right = texture.ImageCrop(rect:=hair_right),
                .left_hand = texture.ImageCrop(rect:=left_hand),
                .right_hand = texture.ImageCrop(rect:=right_hand),
                .Emotions = Emotions _
                    .ToDictionary(Function(t) t.Key,
                                  Function(t)
                                      Return texture.ImageCrop(rect:=t.Value)
                                  End Function)
            }
        End SyncLock
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function DefaultSlicer() As [Default](Of SpiritImage)
        Return New SpiritImage With {
            .body = New RectangleF(),
            .foot = New RectangleF(),
            .hair_back = New RectangleF(),
            .hair_left = New RectangleF(),
            .hair_right = New RectangleF(),
            .head = New RectangleF(),
            .left_hand = New RectangleF(),
            .right_hand = New RectangleF(),
            .Emotions = New Dictionary(Of Emotions, RectangleF) From {
                {CQ.Emotions.terrified, New RectangleF},
                {CQ.Emotions.smile, New RectangleF},
                {CQ.Emotions.boring, New RectangleF},
                {CQ.Emotions.normal, New RectangleF},
                {CQ.Emotions.serious, New RectangleF},
                {CQ.Emotions.angry, New RectangleF}
            }
        }
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