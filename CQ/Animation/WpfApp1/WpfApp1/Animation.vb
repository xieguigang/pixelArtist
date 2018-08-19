Imports System.IO
Imports System.Threading

''' <summary>
''' Animation playback controls
''' </summary>
Public Class Animation

    Dim frames As BitmapImage()
    Dim sleep%
    Dim run As Boolean
    Dim size As Size

    Public ReadOnly Property Name As String

    Sub New(aniName$, res As IEnumerable(Of MemoryStream), Optional rate% = 24)
        frames = res _
            .Select(Function(m)
                        Dim bitmap = New BitmapImage()
                        bitmap.BeginInit()
                        bitmap.StreamSource = m
                        bitmap.CacheOption = BitmapCacheOption.OnLoad
                        bitmap.EndInit()
                        bitmap.Freeze()

                        Return bitmap
                    End Function) _
            .ToArray
        sleep = 1000 / rate
        size = New Size(frames(0).Width, frames(0).Height)
        Name = aniName
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

        location.Top -= dh
        canvas.Margin = location
    End Sub

    Public Sub PlayOn(canvas As Image)
        run = True
        ensureHorizontal(canvas)
        canvas.Width = size.Width
        canvas.Height = size.Height

        For Each frame As BitmapImage In frames
            canvas.Dispatcher.Invoke(Sub() canvas.Source = frame)
            Thread.Sleep(sleep)
        Next
    End Sub
End Class
