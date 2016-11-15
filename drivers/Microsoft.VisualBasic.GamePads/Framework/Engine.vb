Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.Text

''' <summary>
''' 游戏引擎
''' </summary>
Public MustInherit Class GameEngine : Implements IDisposable
    Implements IEnumerable(Of GraphicUnit)
    Implements IObjectModel_Driver

    ''' <summary>
    ''' 基于GDI+的显示驱动模块
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DisplayDriver As GraphicDevice
    ''' <summary>
    ''' Controller device of the game play engine.(输入设备，包括鼠标与键盘的输入的捕捉)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ControlsMap As Controller
    ''' <summary>
    ''' 内部的显示输出设备
    ''' </summary>
    Protected Friend WithEvents _innerDevice As DisplayPort

    Protected ReadOnly _rnd As Random = New Random(100)

    Protected Score As IScore

    ''' <summary>
    ''' 图像的绘图区域
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property GraphicRegion As Rectangle

    Public Property GameOverCallback As Action(Of GameEngine)

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="Display">将输出的图像数据定向到这个输出设备之上</param>
    Sub New(Display As DisplayPort)
        Me._innerDevice = Display ' 有些模块是需要这个来触发事件的，所以这个必须要在第一个赋值，否则组件初始化的都是Nothing

        Me.ControlsMap = New Controller(Me)
        Me.DisplayDriver = New GraphicDevice(Me)
        Me.DisplayDriver.RefreshHz = 25

        Call GraphicsDeviceResize()  ' 默认是禁用自动调整大小的
        _GraphicRegion = New Rectangle(New Point, _innerDevice.Size) ' 初始化绘图设备的大小
    End Sub

    ''' <summary>
    ''' 游戏引擎是否处于运行状态
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Running As Boolean

    ''' <summary>
    ''' 随机事件是否发生
    ''' </summary>
    ''' <param name="n">1-7, 数字越小越容易发生</param>
    ''' <returns></returns>
    Protected Function __happens(n As Integer) As Boolean
        Dim a As Double = _rnd.NextDouble
        Dim b As Double = _rnd.NextDouble
        Dim c As Double = _rnd.NextDouble
        Dim i As Double = _rnd.NextDouble
        Dim j As Double = _rnd.NextDouble
        Dim x As Double = _rnd.NextDouble
        Dim y As Double = _rnd.NextDouble
        Dim z As Double = _rnd.NextDouble
        Dim array As Double() = {b, c, i, j, x, y, z}
        Dim pre As Double = a

        For Each x In array
            If pre > x Then
                pre = x
                n -= 1
                If n = 0 Then  ' 数字越小越容易发生
                    Return True
                End If
            Else
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' 启动游戏引擎。请注意，线程会被阻塞在这里
    ''' </summary>
    Public Function Run() As Integer Implements IObjectModel_Driver.Run
        If Running Then
            Return 1
        Else
            _Running = True
        End If

        Call Parallel.RunTask(AddressOf __displayUpdates)

        Do While Running
            Call Threading.Thread.Sleep(1)
            Try
                Call __worldReacts()
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        Loop

        Return 0
    End Function

    ''' <summary>
    ''' 图像更新
    ''' </summary>
    Private Sub __displayUpdates()
        Do While Running
            Call DisplayDriver.Updates()
            Call Threading.Thread.Sleep(DisplayDriver._sleep)
        Loop
    End Sub

    Public Sub Pause()
        _Running = False
        Call Threading.Thread.Sleep(100) ' 休眠一段时间，等待引擎之中的图像更新线程和控制线程的退出
    End Sub

    Public Sub Start()
        Call Parallel.RunTask(AddressOf Run)
    End Sub

    Public Sub Add(obj As GraphicUnit)
        SyncLock DisplayDriver._list
            Call DisplayDriver._list.Add(obj)
        End SyncLock
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="obj"></param>
    Public Sub Remove(obj As GraphicUnit, Optional throwEx As Boolean = False)
        If obj Is Nothing Then
            Dim ex As New Exception("GraphicUnit is nothing!")
            Call App.LogException(ex)

            If throwEx Then
                Throw ex
            End If
        Else
            SyncLock DisplayDriver._list
                If DisplayDriver._list.Contains(obj) Then
                    Call DisplayDriver._list.Remove(obj)
                End If
            End SyncLock
        End If
    End Sub

    ''' <summary>
    ''' 计算游戏内部世界的变化
    ''' </summary>
    Protected MustOverride Sub __worldReacts()
    ''' <summary>
    ''' 处理来自于键盘的输入
    ''' </summary>
    ''' <param name="control"></param>
    ''' <param name="raw"></param>
    Public MustOverride Sub Invoke(control As EngineParts.Controls, raw As Char)

    ''' <summary>
    ''' Null raw char
    ''' </summary>
    ''' <param name="control"></param>
    Public Sub Invoke(control As EngineParts.Controls)
        Call Invoke(control, ASCII.NUL)
    End Sub

    ''' <summary>
    ''' 初始化游戏引擎
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function Init() As Boolean
    ''' <summary>
    ''' 处理来自鼠标的输入事件
    ''' </summary>
    ''' <param name="pos"></param>
    ''' <param name="x"></param>
    Public MustOverride Sub ClickObject(pos As Point, x As GraphicUnit)
    ''' <summary>
    ''' 输出设备的绘图区域的大小发生了变化
    ''' </summary>
    Protected MustOverride Sub __GraphicsDeviceResize()

    Public MustOverride Sub __reset()
    Public MustOverride Sub __restart()

    Public Sub Restart()
        Call __restart()
        Call Me.DriverRun
    End Sub

    ''' <summary>
    ''' Usually call this method after game over and restart the game.
    ''' </summary>
    Public Sub Reset()
        Call __reset()
        Call Me.DriverRun
    End Sub

    Public Property DeviceResizeEnable As Boolean = False

    Private Sub GraphicsDeviceResize() Handles _innerDevice.Resize
        If DeviceResizeEnable Then
            _GraphicRegion = New Rectangle(New Point, _innerDevice.Size)
            Call __GraphicsDeviceResize()
        End If
    End Sub

    Public Iterator Function GetEnumerator() As IEnumerator(Of GraphicUnit) Implements IEnumerable(Of GraphicUnit).GetEnumerator
        For Each x In DisplayDriver._list.ToArray
            Yield x
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _Running = False
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
