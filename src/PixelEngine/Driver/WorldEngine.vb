Imports Microsoft.VisualBasic.Parallel.Tasks

Public Class WorldEngine : Implements IDisposable

    Friend ReadOnly controls As Controller.FireCommand

    Dim graphics As Action(Of PixelGraphics)
    Dim graphicsLoop As UpdateThread
    Dim gameLoop As UpdateThread

    Friend screen As PixelScreen
    Friend controller As Controller

    Private disposedValue As Boolean

    Sub New(graphics As Action(Of PixelGraphics), controls As Controller.FireCommand, fps As Integer, worldSpeed As Integer)
        Dim ms As Double = 1000 / fps

        Me.graphics = graphics
        Me.controls = controls
        Me.controller = New Controller(Me)
        Me.graphicsLoop = New UpdateThread(ms, AddressOf PaintFrame)
        Me.gameLoop = New UpdateThread(worldSpeed, AddressOf CallActionCommand)
    End Sub

    Private Sub CallActionCommand()
        If Not screen Is Nothing Then
            Call controller.CallCommand()
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
End Class
