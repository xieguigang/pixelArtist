Imports Microsoft.VisualBasic.Parallel.Tasks

Public Class WorldEngine
    Implements IEnumerable(Of CharacterModel)
    Implements IDisposable

    Friend ReadOnly controls As Controller.FireCommand
    Friend ReadOnly clicks As Controller.ClickObject

    Dim graphics As Action(Of PixelGraphics)
    Dim graphicsLoop As UpdateThread
    Dim gameLoop As UpdateThread
    Dim models As New List(Of CharacterModel)

    Friend screen As PixelScreen
    Friend controller As Controller

    Private disposedValue As Boolean

    Sub New(graphics As Action(Of PixelGraphics), controls As Controller.FireCommand,
            Optional mouseClick As Controller.ClickObject = Nothing,
            Optional fps As Integer = 30,
            Optional worldSpeed As Integer = 100)

        Dim ms As Double = 1000 / fps

        If controls Is Nothing Then
            Throw New NullReferenceException("Keyboard or GamePad controller can not be missing!")
        End If

        Me.graphics = graphics
        Me.controls = controls
        Me.clicks = mouseClick
        Me.controller = New Controller(Me)
        Me.graphicsLoop = New UpdateThread(ms, AddressOf PaintFrame)
        Me.gameLoop = New UpdateThread(worldSpeed, AddressOf CallActionCommand)
    End Sub

    Private Sub CallActionCommand()
        If Not screen Is Nothing Then
            controller.CallCommand()

            If Not clicks Is Nothing Then
                Call controller.CallModelClick()
            End If
        End If
    End Sub

    Public Function LoadScreenDevice(screen As PixelScreen) As WorldEngine
        Me.screen = screen
        Return Me
    End Function

    Public Sub PaintFrame()
        SyncLock screen
            If Not screen.IsDisposed Then
                Call screen.Invoke(Sub() Call screen.RequestFrame(graphics))
            End If
        End SyncLock
    End Sub

    Public Sub Run()
        Call graphicsLoop.Start()
        Call gameLoop.Start()
    End Sub

    Public Sub Pause()
        Call graphicsLoop.Stop()
        Call gameLoop.Stop()
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call gameLoop.Dispose()
                Call graphicsLoop.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

    Public Iterator Function GetEnumerator() As IEnumerator(Of CharacterModel) Implements IEnumerable(Of CharacterModel).GetEnumerator
        Yield IEnumerable_GetEnumerator()
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        For Each obj As CharacterModel In models
            Yield obj
        Next
    End Function
End Class
