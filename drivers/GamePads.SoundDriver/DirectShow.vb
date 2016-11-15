Imports QuartzTypeLib

Namespace DirectShow

    ''' <summary>
    ''' DirectShow组件的抽象接口，整个播放器的核心部件
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DirectShow : Implements IDisposable

        Public ReadOnly Property MediaControl As IMediaControl
        Public ReadOnly Property MediaPosition As IMediaPosition
        Public ReadOnly Property BasicAudio As IBasicAudio
        Public ReadOnly Property url As String

        Public Sub Seek(position As Double)
            MediaPosition.CurrentPosition = position
        End Sub

        Public Function RenderFile(path As String) As DirectShow
            Try
                Call [Stop]()
                Call __renderFile(path)
            Catch ex As Exception
                Throw New Exception(path, ex)
            End Try

            Return Me
        End Function

        Private Sub __renderFile(path As String)
            Me._MediaControl = New FilgraphManager
            Me._MediaControl.RenderFile(path)
            Me._BasicAudio = MediaControl
            Me._MediaPosition = MediaControl
            Me._url = path
        End Sub

        Public ReadOnly Property Duration As Double
            Get
                Return MediaPosition.Duration
            End Get
        End Property

        Public Sub Dispose() Implements IDisposable.Dispose
            On Error Resume Next

            Call [Stop]()

            Me._MediaPosition = Nothing
            Me._BasicAudio = Nothing
            Me._MediaControl = Nothing
        End Sub

        Public Overrides Function ToString() As String
            Return url
        End Function

        Public Function IsNull() As Boolean
            Return (BasicAudio Is Nothing OrElse MediaControl Is Nothing OrElse MediaPosition Is Nothing)
        End Function

        Public Sub [Stop]()
            On Error Resume Next
            MediaControl.Stop()
        End Sub

        Public Sub Pause()
            On Error Resume Next
            MediaControl.Pause()
        End Sub

        Public Sub Play()
            On Error Resume Next
            MediaControl.Run()
        End Sub

        Public ReadOnly Property State As Long
            Get
                Dim TimeOut As Long, s As Long = 0
                MediaControl.GetState(TimeOut, s)
                Return s
            End Get
        End Property
    End Class

    ''' <summary>
    ''' DirectShow driver
    ''' </summary>
    Public Module WinMM

        ''' <summary>
        ''' 将数字转化为mm:ss的时间格式
        ''' </summary>
        ''' <param name="intTime"></param>
        ''' <returns></returns>
        Public Function Int2_strTime(intTime As Integer) As String
            Dim mm As Integer = Int(intTime \ 60)
            Dim ss As Integer = intTime Mod 60

            Return mm.ToString + ":" + Format(ss, "00").ToString
        End Function

        Public Sub PlaySound(filename As String)
            filename = FileIO.FileSystem.GetFileInfo(filename).FullName

            Try
                Dim Device As New DirectShow
                Call Device.RenderFile(filename)
                Call Device.Play()
            Catch ex As Exception
                ex = New Exception(filename, ex)
                Call MsgBox(ex.ToString, MsgBoxStyle.Critical, "Sound Driver Error!")
            End Try
        End Sub
    End Module
End Namespace