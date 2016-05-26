﻿Imports System.Drawing
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

        Public Function PlayBack(uri As String, Optional format As TStreamFormat = TStreamFormat.sfAutodetect) As Boolean
            If __api.OpenFile(uri, TStreamFormat.sfAutodetect) Then
                ' 成功的话则开始获取文件的标签信息
                _ID3v2 = New TID3InfoEx
                Call __api.LoadID3Ex(_ID3v2, True)
            Else
                Return False
            End If

            Return True
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