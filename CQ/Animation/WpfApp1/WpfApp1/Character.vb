''' <summary>
''' Character model
''' </summary>
Public Class Character : Implements IDisposable

    Dim animations As Dictionary(Of String, Animation)
    Dim canvas As Image
    Dim stage As Grid
    Dim keys$()

    Dim playbackQueue As New Queue(Of String)
    Dim previous As Animation
    Dim run As Boolean = False
    Dim mouse As MoveDragHelper

    Public Property name As String
    Public Property rand As Random

    Sub New(win As Window, host As Grid, animationList As IEnumerable(Of Animation))
        canvas = New Image With {
            .Width = 100,
            .Height = 100,
            .Margin = New Thickness(100, 300, 200, 500)
        }
        stage = host
        host.Children.Add(canvas)
        mouse = New MoveDragHelper(canvas, win)
        animations = animationList.ToDictionary(Function(a) a.Name)
        keys = animations.Keys.ToArray
    End Sub

    Public Sub Pending(key As String)
        If animations.ContainsKey(key) Then
            SyncLock playbackQueue
                playbackQueue.Enqueue(key)
            End SyncLock
        End If
    End Sub

    Public Sub [Stop]()
        run = False
    End Sub

    Public Sub Action()
        run = True

        Do While run
            Call PlayNext()
        Loop
    End Sub

    Public Sub PlayNext()
        Dim animation As Animation
        Dim offset As Point

        If playbackQueue.Count > 0 Then
            animation = animations(playbackQueue.Dequeue)
        Else
            SyncLock rand
                animation = animations(keys(rand.Next(0, keys.Length)))
            End SyncLock
        End If

        If Not previous Is Nothing Then
            offset = Offsets.Calculate(previous, [next]:=animation)
        Else
            offset = New Point
        End If

        previous = animation
        animation.PlayOn(canvas, offset)
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Call [Stop]()
                Call stage.Children.Remove(canvas)
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
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

