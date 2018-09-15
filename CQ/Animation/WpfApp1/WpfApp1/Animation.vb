Imports System.IO
Imports System.Threading
Imports OCR
Imports gdi = System.Drawing

''' <summary>
''' Animation playback controls
''' </summary>
Public Class Animation : Implements IEnumerable(Of BitmapImage)

    Dim frames As BitmapImage()
    Dim sleep%
    Dim run As Boolean
    Dim size As Size

    Public ReadOnly Property Name As String
    Public ReadOnly Property FirstFrameRectangle As Thickness
    Public ReadOnly Property LastFrameRectangle As Thickness

    Sub New(aniName$, res As IEnumerable(Of MemoryStream), Optional rate% = 24)
        Dim resources As MemoryStream() = res.ToArray
        Dim first As gdi.Image = New gdi.Bitmap(resources(Scan0))
        Dim last As gdi.Image = New gdi.Bitmap(resources.Last())

        frames = resources _
            .Select(Function(m)
                        Return m.WpfBitmap
                    End Function) _
            .ToArray
        sleep = 1000 / rate
        size = New Size(frames(0).Width, frames(0).Height)
        Name = aniName

        FirstFrameRectangle = Offsets.CalcRectangle(first.Projection(True, gdi.Color.Transparent), first.Projection(False, gdi.Color.Transparent))
        LastFrameRectangle = Offsets.CalcRectangle(last.Projection(True, gdi.Color.Transparent), last.Projection(False, gdi.Color.Transparent))
    End Sub

    Public Sub [Stop]()
        run = False
    End Sub

    Private Sub waitStop()
        Do While run
            Thread.Sleep(10)
        Loop
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="canvas"></param>
    ''' <param name="wait">必须要前一个动作完成了才可以执行下一个动画</param>
    Public Sub PlayBack(canvas As Image, Optional wait As Animation = Nothing)
        If Not wait Is Nothing Then
            Call wait.waitStop()
        End If

        run = True
        ensureHorizontal(canvas)
        canvas.Width = size.Width
        canvas.Height = size.Height

        Call New Thread(Sub()
                            Do While run
                                For Each frame As BitmapImage In frames
                                    canvas.Dispatcher.Invoke(Sub() canvas.Source = frame)
                                    Thread.Sleep(sleep)
                                Next
                            Loop
                        End Sub).Start()
    End Sub

    ''' <summary>
    ''' 因为帧的大小是不一致的，所以调整大小的时候水平位置可能会发生位移
    ''' 在这里自动计算偏移量并纠正位移错误
    ''' 
    ''' 这个函数假设画布的水平位置已经被确认好了
    ''' </summary>
    Private Sub ensureHorizontal(canvas As Image)
        Dim bottom = canvas.Margin.Bottom
        Dim dh = canvas.Height - size.Height
        Dim location = canvas.Margin

        location.Top += dh
        canvas.Margin = location
    End Sub

    Public Sub PlayOn(canvas As Image, offset As Point)
        run = True
        canvas.Dispatcher _
              .Invoke(Sub()
                          Dim location As Thickness = canvas.Margin

                          ' ensureHorizontal(canvas)
                          canvas.Width = size.Width
                          canvas.Height = size.Height

                          location.Top -= offset.Y
                          location.Left -= offset.X

                          canvas.Margin = location
                      End Sub)

        For Each frame As BitmapImage In frames
            canvas.Dispatcher.Invoke(Sub() canvas.Source = frame)
            Thread.Sleep(sleep)
        Next
    End Sub

    Public Iterator Function GetEnumerator() As IEnumerator(Of BitmapImage) Implements IEnumerable(Of BitmapImage).GetEnumerator
        For Each frame As BitmapImage In frames
            Yield frame
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class
