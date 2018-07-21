Imports Testures.DDS.DDS_PIXELFORMAT
Imports DWORD = System.Int32

Namespace DDS.DDS_HEADER

    'typedef struct {
    '  DWORD           dwSize;
    '  DWORD           dwFlags;
    '  DWORD           dwHeight;
    '  DWORD           dwWidth;
    '  DWORD           dwPitchOrLinearSize;
    '  DWORD           dwDepth;
    '  DWORD           dwMipMapCount;
    '  DWORD           dwReserved1[11];
    '  DDS_PIXELFORMAT ddspf;
    '  DWORD           dwCaps;
    '  DWORD           dwCaps2;
    '  DWORD           dwCaps3;
    '  DWORD           dwCaps4;
    '  DWORD           dwReserved2;
    '} DDS_HEADER;

    ''' <summary>
    ''' Describes a DDS file header.
    ''' 
    ''' > https://docs.microsoft.com/en-us/windows/desktop/direct3ddds/dds-header
    ''' </summary>
    Public Class Header

#Region "typedef struct {"

        ''' <summary>
        ''' Size of structure. This member must be set to 124.
        ''' </summary>
        Public dwSize As DWORD
        ''' <summary>
        ''' Flags to indicate which members contain valid data.
        ''' </summary>
        Public dwFlags As dwFlags
        ''' <summary>
        ''' Surface height (in pixels).
        ''' </summary>
        Public dwHeight As DWORD
        ''' <summary>
        ''' Surface width (in pixels).
        ''' </summary>
        Public dwWidth As DWORD
        ''' <summary>
        ''' The pitch or number of bytes per scan line in an uncompressed texture; 
        ''' the total number of bytes in the top level texture for a compressed 
        ''' texture. For information about how to compute the pitch, see the DDS 
        ''' File Layout section of the Programming Guide for DDS.
        ''' </summary>
        Public dwPitchOrLinearSize As DWORD
        ''' <summary>
        ''' Depth of a volume texture (in pixels), otherwise unused.
        ''' </summary>
        Public dwDepth As DWORD
        ''' <summary>
        ''' Number of mipmap levels, otherwise unused.
        ''' </summary>
        Public dwMipMapCount As DWORD
        ''' <summary>
        ''' dwReserved1[11] Unused.
        ''' </summary>
        Public dwReserved1 As DWORD()
        ''' <summary>
        ''' The pixel format (see DDS_PIXELFORMAT).
        ''' </summary>
        Public ddspf As PixelFormat
        ''' <summary>
        ''' Specifies the complexity of the surfaces stored.
        ''' </summary>
        Public dwCaps As dwCaps
        ''' <summary>
        ''' Additional detail about the surfaces stored.
        ''' </summary>
        Public dwCaps2 As dwCaps2
        Public dwCaps3 As DWORD
        Public dwCaps4 As DWORD
        Public dwReserved2 As DWORD
#End Region

    End Class

    ''' <summary>
    ''' Additional detail about the surfaces stored.
    ''' </summary>
    Public Enum dwCaps2 As DWORD
        ''' <summary>
        ''' Required For a cube map.
        ''' </summary>
        DDSCAPS2_CUBEMAP = &H200
        ''' <summary>
        ''' Required When these surfaces are stored In a cube map.
        ''' </summary>
        DDSCAPS2_CUBEMAP_POSITIVEX = &H400
        ''' <summary>
        ''' Required When these surfaces are stored In a cube map.
        ''' </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEX = &H800
        ''' <summary>
        ''' Required When these surfaces are stored In a cube map.
        ''' </summary>
        DDSCAPS2_CUBEMAP_POSITIVEY = &H1000
        ''' <summary>
        ''' Required When these surfaces are stored In a cube map.
        ''' </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEY = &H2000
        ''' <summary>
        ''' Required When these surfaces are stored In a cube map.
        ''' </summary>
        DDSCAPS2_CUBEMAP_POSITIVEZ = &H4000
        ''' <summary>
        ''' Required When these surfaces are stored In a cube map.
        ''' </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEZ = &H8000
        ''' <summary>
        ''' Required For a volume texture.
        ''' </summary>
        DDSCAPS2_VOLUME = &H200000
    End Enum

    Public Enum dwCaps As DWORD
        ''' <summary>
        ''' Optional; must be used On any file that contains more than one surface 
        ''' (a mipmap, a cubic environment map, Or mipmapped volume texture).	
        ''' </summary>
        DDSCAPS_COMPLEX = &H8
        ''' <summary>
        ''' Optional; should be used For a mipmap.
        ''' </summary>
        DDSCAPS_MIPMAP = &H400000
        ''' <summary>
        ''' Required
        ''' </summary>
        DDSCAPS_TEXTURE = &H1000
    End Enum

    ''' <summary>
    ''' Flags to indicate which members contain valid data.
    ''' </summary>
    Public Enum dwFlags As DWORD
        ''' <summary>
        ''' Required In every .dds file.	
        ''' </summary>
        DDSD_CAPS = &H1
        ''' <summary>
        ''' Required In every .dds file.	
        ''' </summary>
        DDSD_HEIGHT = &H2
        ''' <summary>
        ''' Required In every .dds file.	
        ''' </summary>
        DDSD_WIDTH = &H4
        ''' <summary>
        ''' Required When pitch Is provided For an uncompressed texture.	
        ''' </summary>
        DDSD_PITCH = &H8
        ''' <summary>
        ''' Required In every .dds file.	
        ''' </summary>
        DDSD_PIXELFORMAT = &H1000
        ''' <summary>
        ''' Required In a mipmapped texture.	
        ''' </summary>
        DDSD_MIPMAPCOUNT = &H20000
        ''' <summary>
        ''' Required When pitch Is provided For a compressed texture.	
        ''' </summary>
        DDSD_LINEARSIZE = &H80000
        ''' <summary>
        ''' Required In a depth texture.	
        ''' </summary>
        DDSD_DEPTH = &H800000
    End Enum
End Namespace