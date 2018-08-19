Imports System.Threading

Public Class Stage : Implements IDisposable

    Dim characters As New Dictionary(Of String, Character)
    Dim host As Window
    Dim mainGrid As Grid
    Dim random As New Random

    Sub New(host As Window, grid As Grid)
        Me.host = host
        Me.mainGrid = grid
    End Sub

    Public Sub Add(name$, characterAnimations As IEnumerable(Of Animation))
        Dim character As New Character(host, mainGrid, characterAnimations) With {
            .name = name,
            .rand = random
        }

        SyncLock characters
            characters.Add(name, character)
        End SyncLock

        Call New Thread(AddressOf character.Action).Start()
    End Sub

    Public Sub Remove(name As String)
        SyncLock characters
            If characters.ContainsKey(name) Then
                characters(name).Stop()
                characters.Remove(name)
            End If
        End SyncLock
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                For Each name As String In characters.Keys.ToArray
                    Call Remove(name)
                Next
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
