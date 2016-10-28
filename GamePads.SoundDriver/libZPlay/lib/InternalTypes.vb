Imports System
Imports System.Runtime.InteropServices
Imports System.Drawing

Namespace InternalTypes

#Region "Structure and Enum"

    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TStreamInfo_Internal
        <FieldOffset(0)>
        Public SamplingRate As Integer
        <FieldOffset(4)>
        Public ChannelNumber As Integer
        <FieldOffset(8)>
        Public VBR As Boolean
        <FieldOffset(12)>
        Public Bitrate As Integer
        <FieldOffset(16)>
        Public Length As TStreamTime
        <FieldOffset(44)>
        Public Description As IntPtr
    End Structure

    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TWaveOutInfo_Internal
        <FieldOffset(0)>
        Public ManufacturerID As UInteger
        <FieldOffset(4)>
        Public ProductID As UInteger
        <FieldOffset(8)>
        Public DriverVersion As UInteger
        <FieldOffset(12)>
        Public Formats As UInteger
        <FieldOffset(16)>
        Public Channels As UInteger
        <FieldOffset(20)>
        Public Support As UInteger
        <FieldOffset(24)>
        Public ProductName As IntPtr
    End Structure

    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TWaveInInfo_Internal
        <FieldOffset(0)>
        Public ManufacturerID As UInteger
        <FieldOffset(4)>
        Public ProductID As UInteger
        <FieldOffset(8)>
        Public DriverVersion As UInteger
        <FieldOffset(12)>
        Public Formats As UInteger
        <FieldOffset(16)>
        Public Channels As UInteger
        <FieldOffset(20)>
        Public ProductName As IntPtr
    End Structure

    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TID3Info_Internal
        <FieldOffset(0)>
        Public Title As IntPtr
        <FieldOffset(4)>
        Public Artist As IntPtr
        <FieldOffset(8)>
        Public Album As IntPtr
        <FieldOffset(12)>
        Public Year As IntPtr
        <FieldOffset(16)>
        Public Comment As IntPtr
        <FieldOffset(20)>
        Public Track As IntPtr
        <FieldOffset(24)>
        Public Genre As IntPtr
    End Structure


    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TID3InfoEx_Internal
        <FieldOffset(0)>
        Public Title As IntPtr
        <FieldOffset(4)>
        Public Artist As IntPtr
        <FieldOffset(8)>
        Public Album As IntPtr
        <FieldOffset(12)>
        Public Year As IntPtr
        <FieldOffset(16)>
        Public Comment As IntPtr
        <FieldOffset(20)>
        Public Track As IntPtr
        <FieldOffset(24)>
        Public Genre As IntPtr
        <FieldOffset(28)>
        Public AlbumArtist As IntPtr
        <FieldOffset(32)>
        Public Composer As IntPtr
        <FieldOffset(36)>
        Public OriginalArtist As IntPtr
        <FieldOffset(40)>
        Public Copyright As IntPtr
        <FieldOffset(44)>
        Public URL As IntPtr
        <FieldOffset(48)>
        Public Encoder As IntPtr
        <FieldOffset(52)>
        Public Publisher As IntPtr
        <FieldOffset(56)>
        Public BPM As Integer
        <FieldOffset(60)>
        Public PicturePresent As Integer
        <FieldOffset(64)>
        Public CanDrawPicture As Integer
        <FieldOffset(68)>
        Public MIMEType As IntPtr
        <FieldOffset(72)>
        Public PictureType As Integer
        <FieldOffset(76)>
        Public Description As IntPtr
        <FieldOffset(80)>
        Public PictureData As IntPtr
        <FieldOffset(84)>
        Public PictureDataSize As Integer
        <FieldOffset(88)>
        Public hBitmap As IntPtr
        <FieldOffset(92)>
        Public Width As Integer
        <FieldOffset(96)>
        Public Height As Integer
        <FieldOffset(356)>
        Public reserved As IntPtr
    End Structure

    Public Delegate Function TCallbackFunc(objptr As UInteger, user_data As Integer, msg As TCallbackMessage, param1 As UInteger, param2 As UInteger) As Integer

    Public Enum TSettingID As Integer
        sidWaveBufferSize = 1
        sidAccurateLength = 2
        sidAccurateSeek = 3
        sidSamplerate = 4
        sidChannelNumber = 5
        sidBitPerSample = 6
        sidBigEndian = 7
    End Enum

    Public Enum TStreamFormat As Integer
        sfUnknown = 0
        sfMp3 = 1
        sfOgg = 2
        sfWav = 3
        sfPCM = 4
        sfFLAC = 5
        sfFLACOgg = 6
        sfAC3 = 7
        sfAacADTS = 8
        sfWaveIn = 9
        sfAutodetect = 1000
    End Enum


    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TStreamInfo
        <FieldOffset(0)>
        Public SamplingRate As Integer
        <FieldOffset(4)>
        Public ChannelNumber As Integer
        <FieldOffset(8)>
        Public VBR As Boolean
        <FieldOffset(12)>
        Public Bitrate As Integer
        <FieldOffset(16)>
        Public Length As TStreamTime
        <FieldOffset(44)>
        Public Description As String
    End Structure





    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TWaveOutInfo
        <FieldOffset(0)>
        Public ManufacturerID As UInteger
        <FieldOffset(4)>
        Public ProductID As UInteger
        <FieldOffset(8)>
        Public DriverVersion As UInteger
        <FieldOffset(12)>
        Public Formats As UInteger
        <FieldOffset(16)>
        Public Channels As UInteger
        <FieldOffset(20)>
        Public Support As UInteger
        <FieldOffset(24)>
        Public ProductName As String
    End Structure


    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TWaveInInfo
        <FieldOffset(0)>
        Public ManufacturerID As UInteger
        <FieldOffset(4)>
        Public ProductID As UInteger
        <FieldOffset(8)>
        Public DriverVersion As UInteger
        <FieldOffset(12)>
        Public Formats As UInteger
        <FieldOffset(16)>
        Public Channels As UInteger
        <FieldOffset(20)>
        Public ProductName As String
    End Structure


    Public Enum TFFTWindow As Integer
        fwRectangular = 1
        fwHamming
        fwHann
        fwCosine
        fwLanczos
        fwBartlett
        fwTriangular
        fwGauss
        fwBartlettHann
        fwBlackman
        fwNuttall
        fwBlackmanHarris
        fwBlackmanNuttall
        fwFlatTop
    End Enum




    Public Enum TTimeFormat As UInteger
        tfMillisecond = 1
        tfSecond = 2
        tfHMS = 4
        tfSamples = 8
    End Enum

    Public Enum TSeekMethod As Integer
        smFromBeginning = 1
        smFromEnd = 2
        smFromCurrentForward = 4
        smFromCurrentBackward = 8
    End Enum

    <StructLayout(LayoutKind.Explicit)>
    Public Structure TStreamLoadInfo
        <FieldOffset(0)>
        Public NumberOfBuffers As UInteger
        <FieldOffset(4)>
        Public NumberOfBytes As UInteger
    End Structure




    <StructLayout(LayoutKind.Explicit)>
    Public Structure TEchoEffect
        <FieldOffset(0)>
        Public nLeftDelay As Integer
        <FieldOffset(4)>
        Public nLeftSrcVolume As Integer
        <FieldOffset(8)>
        Public nLeftEchoVolume As Integer
        <FieldOffset(12)>
        Public nRightDelay As Integer
        <FieldOffset(16)>
        Public nRightSrcVolume As Integer
        <FieldOffset(20)>
        Public nRightEchoVolume As Integer
    End Structure

    Public Enum TID3Version As Integer
        id3Version1 = 1
        id3Version2 = 2
    End Enum


    Public Enum TFFTGraphHorizontalScale As Integer
        gsLogarithmic = 0
        gsLinear = 1
    End Enum

    Public Enum TFFTGraphParamID As Integer
        gpFFTPoints = 1
        gpGraphType
        gpWindow
        gpHorizontalScale
        gpSubgrid
        gpTransparency
        gpFrequencyScaleVisible
        gpDecibelScaleVisible
        gpFrequencyGridVisible
        gpDecibelGridVisible
        gpBgBitmapVisible
        gpBgBitmapHandle
        gpColor1
        gpColor2
        gpColor3
        gpColor4
        gpColor5
        gpColor6
        gpColor7
        gpColor8
        gpColor9
        gpColor10
        gpColor11
        gpColor12
        gpColor13
        gpColor14
        gpColor15
        gpColor16
    End Enum

    Public Enum TFFTGraphType As Integer
        gtLinesLeftOnTop = 0
        gtLinesRightOnTop
        gtAreaLeftOnTop
        gtAreaRightOnTop
        gtBarsLeftOnTop
        gtBarsRightOnTop
        gtSpectrum
    End Enum



    <StructLayout(LayoutKind.Explicit)>
    Public Structure TStreamStatus
        <FieldOffset(0)>
        Public fPlay As Boolean
        <FieldOffset(4)>
        Public fPause As Boolean
        <FieldOffset(8)>
        Public fEcho As Boolean
        <FieldOffset(12)>
        Public fEqualizer As Boolean
        <FieldOffset(16)>
        Public fVocalCut As Boolean
        <FieldOffset(20)>
        Public fSideCut As Boolean
        <FieldOffset(24)>
        Public fChannelMix As Boolean
        <FieldOffset(28)>
        Public fSlideVolume As Boolean
        <FieldOffset(32)>
        Public nLoop As Integer
        <FieldOffset(36)>
        Public fReverse As Boolean
        <FieldOffset(40)>
        Public nSongIndex As Integer
        <FieldOffset(44)>
        Public nSongsInQueue As Integer
    End Structure


    <StructLayout(LayoutKind.Explicit)>
    Public Structure TStreamHMSTime
        <FieldOffset(0)>
        Public hour As UInteger
        <FieldOffset(4)>
        Public minute As UInteger
        <FieldOffset(8)>
        Public second As UInteger
        <FieldOffset(12)>
        Public millisecond As UInteger
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Public Structure TStreamTime
        <FieldOffset(0)>
        Public sec As UInteger
        <FieldOffset(4)>
        Public ms As UInteger
        <FieldOffset(8)>
        Public samples As UInteger
        <FieldOffset(12)>
        Public hms As TStreamHMSTime
    End Structure


    <StructLayout(LayoutKind.Explicit, CharSet:=CharSet.Unicode)>
    Public Structure TID3Info
        <FieldOffset(0)>
        Public Title As String
        <FieldOffset(4)>
        Public Artist As String
        <FieldOffset(8)>
        Public Album As String
        <FieldOffset(12)>
        Public Year As String
        <FieldOffset(16)>
        Public Comment As String
        <FieldOffset(20)>
        Public Track As String
        <FieldOffset(24)>
        Public Genre As String
    End Structure


    Public Structure TID3Picture
        Public PicturePresent As Boolean
        Public PictureType As Integer
        Public Description As String
        Public Bitmap As Bitmap
        Public BitStream As System.IO.MemoryStream
    End Structure


    Public Structure TID3InfoEx
        Public Title As String
        Public Artist As String
        Public Album As String
        Public Year As String
        Public Comment As String
        Public Track As String
        Public Genre As String
        Public AlbumArtist As String
        Public Composer As String
        Public OriginalArtist As String
        Public Copyright As String
        Public URL As String
        Public Encoder As String
        Public Publisher As String
        Public BPM As Integer
        Public Picture As TID3Picture
    End Structure


    Public Enum TBPMDetectionMethod As Integer
        dmPeaks = 0
        dmAutoCorrelation
    End Enum


    Public Enum TFFTGraphSize As Integer
        FFTGraphMinWidth = 100
        FFTGraphMinHeight = 60
    End Enum

    Public Enum TWaveOutMapper As UInteger
        WaveOutWaveMapper = 4294967295
    End Enum

    Public Enum TWaveInMapper As UInteger
        WaveInWaveMapper = 4294967295
    End Enum

    Public Enum TCallbackMessage As Integer
        MsgStopAsync = 1
        MsgPlayAsync = 2
        MsgEnterLoopAsync = 4
        MsgExitLoopAsync = 8
        MsgEnterVolumeSlideAsync = 16
        MsgExitVolumeSlideAsync = 32
        MsgStreamBufferDoneAsync = 64
        MsgStreamNeedMoreDataAsync = 128
        MsgNextSongAsync = 256
        MsgStop = 65536
        MsgPlay = 131072
        MsgEnterLoop = 262144
        MsgExitLoop = 524288
        MsgEnterVolumeSlide = 1048576
        MsgExitVolumeSlide = 2097152
        MsgStreamBufferDone = 4194304
        MsgStreamNeedMoreData = 8388608
        MsgNextSong = 16777216
        MsgWaveBuffer = 33554432
    End Enum


    Public Enum TWaveOutFormat As UInteger
        format_invalid = 0
        format_11khz_8bit_mono = 1
        format_11khz_8bit_stereo = 2
        format_11khz_16bit_mono = 4
        format_11khz_16bit_stereo = 8

        format_22khz_8bit_mono = 16
        format_22khz_8bit_stereo = 32
        format_22khz_16bit_mono = 64
        format_22khz_16bit_stereo = 128

        format_44khz_8bit_mono = 256
        format_44khz_8bit_stereo = 512
        format_44khz_16bit_mono = 1024
        format_44khz_16bit_stereo = 2048
    End Enum

    Public Enum TWaveOutFunctionality As UInteger
        supportPitchControl = 1
        supportPlaybackRateControl = 2
        supportVolumeControl = 4
        supportSeparateLeftRightVolume = 8
        supportSync = 16
        supportSampleAccuratePosition = 32
        supportDirectSound = 6
    End Enum
#End Region
End Namespace