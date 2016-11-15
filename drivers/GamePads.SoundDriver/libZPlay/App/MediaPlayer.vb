Imports System.Drawing
Imports libZPlay.InternalTypes

Namespace App

    Public Class MediaPlayer
        Implements IDisposable

        ReadOnly __api As ZPlay

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="libzplay">libzplay.dll的文件夹的位置，默认是<see cref="Microsoft.VisualBasic.App.HOME"/></param>
        Sub New(Optional libzplay As String = Nothing)
            If Not String.IsNullOrEmpty(libzplay) Then

            End If

            __api = New ZPlay
        End Sub

        ''' <summary>
        ''' Gets the playback position of the current media file
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CurrentPosition As TStreamTime
            Get
                Dim time As New TStreamTime
                Call __api.GetPosition(time)
                Return time
            End Get
        End Property

        Public ReadOnly Property ID3v2 As TID3InfoEx

        Public ReadOnly Property AlbumArt As Bitmap
            Get
                Return ID3v2.Picture.Bitmap
            End Get
        End Property

        Public ReadOnly Property PlaybackURI As String
        Public ReadOnly Property StreamInfo As TStreamInfo

        Public ReadOnly Property status As TStreamStatus
            Get
                Dim s As New TStreamStatus
                Call __api.GetStatus(s)
                Return s
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uri"></param>
        ''' <param name="format"></param>
        ''' <param name="autoStart"></param>
        ''' <returns></returns>
        ''' <remarks>除了一些需要实时进行更新的数据，所有的静态的信息都是在这里一次性的完成读取操作的</remarks>
        Public Function PlayBack(uri As String,
                                 Optional format As TStreamFormat = TStreamFormat.sfAutodetect,
                                 Optional autoStart As Boolean = False) As Boolean

            If __api.OpenFile(uri, TStreamFormat.sfAutodetect) Then
                ' 成功的话则开始获取文件的标签信息
                _ID3v2 = New TID3InfoEx
                _PlaybackURI = uri
                _StreamInfo = New TStreamInfo

                Call __api.LoadID3Ex(_ID3v2, True)
                Call __api.GetStreamInfo(_StreamInfo)

                If autoStart Then
                    Call Play()
                End If
            Else
                Return False
            End If

            Return True
        End Function

        Public Function Play() As TickEvent
            Call __api.StartPlayback()
            Return New TickEvent(Me)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call __api.StopPlayback()
                    Call GC.SuppressFinalize(__api)
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
End Namespace