Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO

Namespace DDS

    <HideModuleName> Public Module Reader

        Const dwMagic$ = "DDS "

        Public Function ReadFile(path As String) As DDS.File
            Using dds As BinaryDataReader = path.OpenBinaryReader
                Dim dwMagic$ = dds.ReadString(4)

                If dwMagic <> Reader.dwMagic Then
                    Throw New InvalidDataException($"File {path} is not a DDS file!")
                End If

                Return New File With {
                    .dwMagic = Reader.dwMagic,
                    .header = dds.parseHeader,
                    .header10 = dds.parseHeader10(.header)
                }
            End Using
        End Function

        <Extension>
        Private Function parseHeader10(dds As BinaryDataReader, header As DDS_HEADER.Header) As DDS_HEADER_DXT10.HeaderDXT10
            If Not header.ddspf.dwFlags = DDS_PIXELFORMAT.dwFlags.DDPF_FOURCC OrElse header.ddspf.dwFourCC <> "DX10" Then
                Return Nothing
            End If

            Return New DDS_HEADER_DXT10.HeaderDXT10 With {
                .dxgiFormat = dds.ReadInt32,
                .resourceDimension = dds.ReadInt32,
                .miscFlag = dds.ReadUInt32,
                .arraySize = dds.ReadUInt32,
                .miscFlags2 = dds.ReadUInt32
            }
        End Function

        <Extension>
        Private Function parseHeader(dds As BinaryDataReader) As DDS_HEADER.Header
            Return New DDS_HEADER.Header With {
                .dwSize = dds.ReadInt32,
                .dwFlags = dds.ReadInt32,
                .dwHeight = dds.ReadInt32,
                .dwWidth = dds.ReadInt32,
                .dwPitchOrLinearSize = dds.ReadInt32,
                .dwDepth = dds.ReadInt32,
                .dwMipMapCount = dds.ReadInt32,
                .dwReserved1 = dds.ReadInt32s(11),
                .ddspf = New DDS_PIXELFORMAT.PixelFormat With {
                    .dwSize = dds.ReadInt32,
                    .dwFlags = dds.ReadInt32,
                    .dwFourCC = dds.ReadString(4),
                    .dwRGBBitCount = dds.ReadInt32,
                    .dwRBitMask = dds.ReadInt32,
                    .dwGBitMask = dds.ReadInt32,
                    .dwBBitMask = dds.ReadInt32,
                    .dwABitMask = dds.ReadInt32
                },
                .dwCaps = dds.ReadInt32,
                .dwCaps2 = dds.ReadInt32,
                .dwCaps3 = dds.ReadInt32,
                .dwCaps4 = dds.ReadInt32,
                .dwReserved2 = dds.ReadInt32
            }
        End Function
    End Module

    Public Class File

        Public dwMagic As String
        Public header As DDS_HEADER.Header

        ''' <summary>
        ''' If the DDS_PIXELFORMAT dwFlags is set to DDPF_FOURCC and dwFourCC is 
        ''' set to "DX10" an additional DDS_HEADER_DXT10 structure will be present 
        ''' to accommodate texture arrays or DXGI formats that cannot be expressed 
        ''' as an RGB pixel foramt such as floating point formats, sRGB formats 
        ''' etc. 
        ''' 
        ''' When the DDS_HEADER_DXT10 structure is present the entire data 
        ''' description will looks like this.
        ''' </summary>
        Public header10 As DDS_HEADER_DXT10.HeaderDXT10


    End Class
End Namespace

