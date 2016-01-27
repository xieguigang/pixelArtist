Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public MustInherit Class Engine : Implements IDisposable
    Implements IEnumerable(Of GraphicUnit)

    ''' <summary>
    ''' 基于GDI+的显示驱动模块
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DisplayDriver As GraphicDevice
    Public ReadOnly Property ControlsMap As Controller

    Protected Friend WithEvents _innerDevice As DisplayPort

    Protected ReadOnly _rnd As Random = New Random(100)

    Protected Score As IScore

    Public ReadOnly Property GraphicRegion As Size
        Get
            Return _innerDevice.Size
        End Get
    End Property

    Sub New(Display As DisplayPort)
        Me._innerDevice = Display ' 有些模块是需要这个来触发事件的，所以这个必须要在第一个赋值，否则组件初始化的都是Nothing

        Me.ControlsMap = New Controller(Me)
        Me.DisplayDriver = New GraphicDevice(Me)
        Me.DisplayDriver.RefreshHz = 25
    End Sub

    Public ReadOnly Property Running As Boolean

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

    Public Sub Run()
        _Running = True
        Call Microsoft.VisualBasic.Parallel.Run(AddressOf __displayUpdates)

        Do While Running
            Call Threading.Thread.Sleep(1)
            Call __worldReacts()
        Loop
    End Sub

    Private Sub __displayUpdates()
        Do While Running
            Call DisplayDriver.Updates()
            Call Threading.Thread.Sleep(DisplayDriver._sleep)
        Loop
    End Sub

    Sub Add(obj As GraphicUnit)
        Call DisplayDriver._list.Add(obj)
    End Sub

    Sub Remove(obj As GraphicUnit)
        Call DisplayDriver._list.Remove(obj)
    End Sub

    Protected MustOverride Sub __worldReacts()
    Public MustOverride Sub Invoke(control As EngineParts.Controls, raw As Char)
    Public MustOverride Function Init() As Boolean

    Public MustOverride Sub ClickObject(pos As Point, x As GraphicUnit)

    Protected MustOverride Sub GraphicsDeviceResize() Handles _innerDevice.Resize

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
