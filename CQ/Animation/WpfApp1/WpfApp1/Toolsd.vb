Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports WpfBitmap = System.Windows.Media.Imaging.BitmapImage

Module Toolsd

    <Extension>
    Public Function WpfBitmap(resource As MemoryStream) As WpfBitmap
        Dim bitmap = New WpfBitmap()
        bitmap.BeginInit()
        bitmap.StreamSource = resource
        bitmap.CacheOption = BitmapCacheOption.OnLoad
        bitmap.EndInit()
        bitmap.Freeze()

        Return bitmap
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="previous">前一个动画的最后一帧</param>
    ''' <param name="[next]">下一个动画的第一帧</param>
    ''' <returns></returns>
    Public Function OffsetRenderTest(previous As Animation, [next] As Animation) As WpfBitmap
        Dim canvas = New MemoryStream(My.Resources.blank.GetStreamBuffer)
        Dim Offset As Point = Offsets.Calculate(previous, [next])

        Dim DrawingVisual As New DrawingVisual()
        Dim DrawingContext = DrawingVisual.RenderOpen()
        Dim bgImage As WpfBitmap = previous.Last
        Dim currentImagePos As New Point(100, bgImage.Height + 100)

        Call DrawingContext.DrawImage(bgImage, New Rect(currentImagePos, New Size(bgImage.Width, bgImage.Height)))

        currentImagePos = New Point(currentImagePos.X - Offset.X, currentImagePos.Y - Offset.Y)
        bgImage = [next].First
        Call DrawingContext.DrawImage(bgImage, New Rect(currentImagePos, New Size(bgImage.Width, bgImage.Height)))

        Call DrawingContext.Close()

        Dim composeImage As New RenderTargetBitmap(500, 500, bgImage.DpiX, bgImage.DpiY, PixelFormats.Default)

        composeImage.Render(DrawingVisual)

        ' 定义一个JPG编码器
        Dim BitmapEncoder As New PngBitmapEncoder
        ' 加入第一帧
        BitmapEncoder.Frames.Add(BitmapFrame.Create(composeImage))

        Using ms As New MemoryStream
            BitmapEncoder.Save(ms)
            Return ms.WpfBitmap
        End Using
    End Function

End Module
