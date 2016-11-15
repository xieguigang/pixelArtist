Imports System.Runtime.InteropServices

Namespace libZPlay

    ''' <summary>
    ''' libzplay.dll interface
    ''' </summary>
    Public Module API

        <DllImport("libzplay.dll", EntryPoint:="zplay_CreateZPlay", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_CreateZPlay() As UInteger
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_DestroyZPlay", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_DestroyZPlay(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetSettings", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetSettings(objptr As UInteger, nSettingID As Integer, value As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetSettings", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetSettings(objptr As UInteger, nSettingID As Integer) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_GetError", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetError(objptr As UInteger) As IntPtr
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetErrorW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_GetErrorW(objptr As UInteger) As IntPtr
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetVersion", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetVersion(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetFileFormat", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetFileFormat(objptr As UInteger, <MarshalAs(UnmanagedType.LPStr)> pchFileName As String) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetFileFormatW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_GetFileFormatW(objptr As UInteger, <MarshalAs(UnmanagedType.LPWStr)> pchFileName As String) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_OpenFile", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_OpenFile(objptr As UInteger, <MarshalAs(UnmanagedType.LPStr)> sFileName As String, nFormat As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_AddFile", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_AddFile(objptr As UInteger, <MarshalAs(UnmanagedType.LPStr)> sFileName As String, nFormat As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_OpenFileW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_OpenFileW(objptr As UInteger, <MarshalAs(UnmanagedType.LPWStr)> sFileName As String, nFormat As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_AddFileW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_AddFileW(objptr As UInteger, <MarshalAs(UnmanagedType.LPWStr)> sFileName As String, nFormat As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_OpenStream", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_OpenStream(objptr As UInteger, fBuffered As Integer, fManaged As Integer, <[In]()> sMemStream() As Byte, nStreamSize As UInteger, nFormat As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_PushDataToStream", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_PushDataToStream(objptr As UInteger, <[In]()> sMemNewData() As Byte, nNewDataize As UInteger) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_Close", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_Close(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_Play", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_Play(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_Stop", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_Stop(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_Pause", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_Pause(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_Resume", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_Resume(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_IsStreamDataFree", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_IsStreamDataFree(objptr As UInteger, <[In]()> sMemNewData() As Byte) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetDynamicStreamLoad", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Sub zplay_GetDynamicStreamLoad(objptr As UInteger, ByRef pStreamLoadInfo As InternalTypes.TStreamLoadInfo)
        End Sub

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetPosition", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Sub zplay_GetPosition(objptr As UInteger, ByRef pTime As InternalTypes.TStreamTime)
        End Sub

        <DllImport("libzplay.dll", EntryPoint:="zplay_PlayLoop", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_PlayLoop(objptr As UInteger, fFormatStartTime As Integer, ByRef pStartTime As InternalTypes.TStreamTime, fFormatEndTime As Integer, ByRef pEndTime As InternalTypes.TStreamTime, nNumOfCycles As UInteger, fContinuePlaying As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_Seek", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_Seek(objptr As UInteger, fFormat As InternalTypes.TTimeFormat, ByRef pTime As InternalTypes.TStreamTime, nMoveMethod As InternalTypes.TSeekMethod) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_ReverseMode", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_ReverseMode(objptr As UInteger, fEnable As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetMasterVolume", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetMasterVolume(objptr As UInteger, nLeftVolume As Integer, nRightVolume As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetPlayerVolume", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetPlayerVolume(objptr As UInteger, nLeftVolume As Integer, nRightVolume As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetMasterVolume", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Sub zplay_GetMasterVolume(objptr As UInteger, ByRef nLeftVolume As Integer, ByRef nRightVolume As Integer)
        End Sub

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetPlayerVolume", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Sub zplay_GetPlayerVolume(objptr As UInteger, ByRef nLeftVolume As Integer, ByRef nRightVolume As Integer)
        End Sub

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetBitrate", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetBitrate(objptr As UInteger, fAverage As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetStatus", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Sub zplay_GetStatus(objptr As UInteger, ByRef pStatus As InternalTypes.TStreamStatus)
        End Sub

        <DllImport("libzplay.dll", EntryPoint:="zplay_MixChannels", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_MixChannels(objptr As UInteger, fEnable As Integer, nLeftPercent As UInteger, nRightPercent As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetVUData", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Sub zplay_GetVUData(objptr As UInteger, ByRef pnLeftChannel As Integer, ByRef pnRightChannel As Integer)
        End Sub

        <DllImport("libzplay.dll", EntryPoint:="zplay_SlideVolume", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SlideVolume(objptr As UInteger, fFormatStart As InternalTypes.TTimeFormat, ByRef pTimeStart As InternalTypes.TStreamTime, nStartVolumeLeft As Integer, nStartVolumeRight As Integer, fFormatEnd As InternalTypes.TTimeFormat, ByRef pTimeEnd As InternalTypes.TStreamTime, nEndVolumeLeft As Integer, nEndVolumeRight As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_EnableEqualizer", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_EnableEqualizer(objptr As UInteger, fEnable As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetEqualizerPoints", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetEqualizerPoints(objptr As UInteger, <[In]()> pnFreqPoint() As Integer, nNumOfPoints As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetEqualizerPoints", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetEqualizerPoints(objptr As UInteger, <[In](), Out()> pnFreqPoint() As Integer, nNumOfPoints As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetEqualizerParam", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetEqualizerParam(objptr As UInteger, nPreAmpGain As Integer, <[In]()> pnBandGain() As Integer, nNumberOfBands As Integer) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_GetEqualizerParam", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetEqualizerParam(objptr As UInteger, ByRef nPreAmpGain As Integer, <[In](), Out()> pnBandGain() As Integer, nNumberOfBands As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetEqualizerPreampGain", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetEqualizerPreampGain(objptr As UInteger, nGain As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetEqualizerPreampGain", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetEqualizerPreampGain(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetEqualizerBandGain", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetEqualizerBandGain(objptr As UInteger, nBandIndex As Integer, nGain As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetEqualizerBandGain", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetEqualizerBandGain(objptr As UInteger, nBandIndex As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_EnableEcho", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_EnableEcho(objptr As UInteger, fEnable As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_StereoCut", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_StereoCut(objptr As UInteger, fEnable As Integer, fOutputCenter As Integer, fBassToSides As Integer) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_SetEchoParam", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetEchoParam(objptr As UInteger, <[In]()> pEchoEffect() As InternalTypes.TEchoEffect, nNumberOfEffects As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetEchoParam", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetEchoParam(objptr As UInteger, <[In](), Out()> pEchoEffect() As InternalTypes.TEchoEffect, nNumberOfEffects As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetFFTData", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetFFTData(objptr As UInteger, nFFTPoints As Integer, nFFTWindow As Integer, ByRef pnHarmonicNumber As Integer, <[In](), Out()> pnHarmonicFreq() As Integer, <[In](), Out()> pnLeftAmplitude() As Integer, <[In](), Out()> pnRightAmplitude() As Integer, <[In](), Out()> pnLeftPhase() As Integer, <[In](), Out()> pnRightPhase() As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetRate", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetRate(objptr As UInteger, nRate As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetRate", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetRate(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetPitch", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetPitch(objptr As UInteger, nPitch As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetPitch", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetPitch(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetTempo", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetTempo(objptr As UInteger, nTempo As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetTempo", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetTempo(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_DrawFFTGraphOnHDC", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_DrawFFTGraphOnHDC(objptr As UInteger, hdc As System.IntPtr, nX As Integer, nY As Integer, nWidth As Integer, nHeight As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_DrawFFTGraphOnHWND", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_DrawFFTGraphOnHWND(objptr As UInteger, hwnd As System.IntPtr, nX As Integer, nY As Integer, nWidth As Integer, nHeight As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetFFTGraphParam", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetFFTGraphParam(objptr As UInteger, nParamID As Integer, nParamValue As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetFFTGraphParam", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetFFTGraphParam(objptr As UInteger, nParamID As Integer) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_LoadID3W", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_LoadID3W(objptr As UInteger, nId3Version As Integer, ByRef pId3Info As InternalTypes.TID3Info_Internal) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_LoadFileID3W", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_LoadFileID3W(objptr As UInteger, <MarshalAs(UnmanagedType.LPWStr)> pchFileName As String, nFormat As Integer, nId3Version As Integer, ByRef pId3Info As InternalTypes.TID3Info_Internal) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_DetectBPM", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_DetectBPM(objptr As UInteger, nMethod As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_DetectFileBPM", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_DetectFileBPM(objptr As UInteger, <MarshalAs(UnmanagedType.LPStr)> sFileName As String, nFormat As Integer, nMethod As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_DetectFileBPMW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_DetectFileBPMW(objptr As UInteger, <MarshalAs(UnmanagedType.LPWStr)> sFileName As String, nFormat As Integer, nMethod As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_SetCallbackFunc", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetCallbackFunc(objptr As UInteger, <MarshalAs(UnmanagedType.FunctionPtr)> pCallbackFunc As InternalTypes.TCallbackFunc, nMessage As InternalTypes.TCallbackMessage, user_data As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_EnumerateWaveIn", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_EnumerateWaveIn(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetWaveInInfoW", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_GetWaveInInfoW(objptr As UInteger, nIndex As UInteger, ByRef pWaveInInfo As InternalTypes.TWaveInInfo_Internal) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_SetWaveInDevice", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetWaveInDevice(objptr As UInteger, nIndex As UInteger) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_EnumerateWaveOut", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_EnumerateWaveOut(objptr As UInteger) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_GetWaveOutInfoW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_GetWaveOutInfoW(objptr As UInteger, nIndex As UInteger, ByRef pWaveOutInfo As InternalTypes.TWaveOutInfo_Internal) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_SetWaveOutDevice", ExactSpelling:=True, CharSet:=CharSet.Ansi, SetLastError:=True)>
        Public Function zplay_SetWaveOutDevice(objptr As UInteger, nIndex As UInteger) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_GetStreamInfoW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_GetStreamInfoW(objptr As UInteger, ByRef pInfo As InternalTypes.TStreamInfo_Internal) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_LoadID3ExW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_LoadID3ExW(objptr As UInteger, ByRef pInfo As InternalTypes.TID3InfoEx_Internal, fDecodeEmbededPicture As Integer) As Integer
        End Function

        <DllImport("libzplay.dll", EntryPoint:="zplay_LoadFileID3ExW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_LoadFileID3ExW(objptr As UInteger, <MarshalAs(UnmanagedType.LPWStr)> sFileName As String, nFormat As Integer, ByRef pInfo As InternalTypes.TID3InfoEx_Internal, fDecodeEmbededPicture As Integer) As Integer
        End Function


        <DllImport("libzplay.dll", EntryPoint:="zplay_SetWaveOutFileW", ExactSpelling:=True, CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function zplay_SetWaveOutFileW(objptr As UInteger, <MarshalAs(UnmanagedType.LPWStr)> sFileName As String, nFormat As Integer, fOutputToSoundcard As Integer) As Integer
        End Function
    End Module
End Namespace