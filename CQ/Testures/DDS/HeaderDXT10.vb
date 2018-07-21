Imports UINT = System.UInt32

Namespace DDS.DDS_HEADER_DXT10

    'typedef struct {
    '  DXGI_FORMAT              dxgiFormat;
    '  D3D10_RESOURCE_DIMENSION resourceDimension;
    '  UINT                     miscFlag;
    '  UINT                     arraySize;
    '  UINT                     miscFlags2;
    '} DDS_HEADER_DXT10;

    ''' <summary>
    ''' DDS header extension to handle resource arrays, DXGI pixel formats 
    ''' that don't map to the legacy Microsoft DirectDraw pixel format 
    ''' structures, and additional metadata.
    ''' </summary>
    Public Class HeaderDXT10

#Region "typedef struct {"

        ''' <summary>
        ''' The surface pixel format (see DXGI_FORMAT).
        ''' </summary>
        Public dxgiFormat As DXGI_FORMAT
        ''' <summary>
        ''' Identifies the type of resource. The following values for this 
        ''' member are a subset of the values in the D3D10_RESOURCE_DIMENSION 
        ''' or D3D11_RESOURCE_DIMENSION enumeration
        ''' </summary>
        Public resourceDimension As D3D10_RESOURCE_DIMENSION
        ''' <summary>
        ''' Identifies other, less common options for resources. The following 
        ''' value for this member is a subset of the values in the 
        ''' D3D10_RESOURCE_MISC_FLAG or D3D11_RESOURCE_MISC_FLAG enumeration
        ''' </summary>
        Public miscFlag As UINT
        ''' <summary>
        ''' The number of elements in the array.
        '''
        ''' + For a 2D texture that Is also a cube-map texture, this number represents 
        ''' the number of cubes. This number Is the same as the number in the NumCubes 
        ''' member of D3D10_TEXCUBE_ARRAY_SRV1 Or D3D11_TEXCUBE_ARRAY_SRV). In this 
        ''' case, the DDS file contains arraySize*6 2D textures. For more information 
        ''' about this case, see the miscFlag description.
        ''' 
        ''' + For a 3D texture, you must set this number To 1.
        ''' </summary>
        Public arraySize As UINT
        ''' <summary>
        ''' Contains additional metadata (formerly was reserved). The lower 3 bits 
        ''' indicate the alpha mode of the associated resource. The upper 29 bits are 
        ''' reserved and are typically 0.
        ''' </summary>
        Public miscFlags2 As UINT
#End Region

    End Class

    ''' <summary>
    ''' Resource data formats, including fully-typed and typeless formats. A list of 
    ''' modifiers at the bottom of the page more fully describes each format type.
    ''' 
    ''' > https://docs.microsoft.com/en-us/windows/desktop/api/dxgiformat/ne-dxgiformat-dxgi_format
    ''' </summary>
    Public Enum DXGI_FORMAT
        DXGI_FORMAT_UNKNOWN
        DXGI_FORMAT_R32G32B32A32_TYPELESS
        DXGI_FORMAT_R32G32B32A32_FLOAT
        DXGI_FORMAT_R32G32B32A32_UINT
        DXGI_FORMAT_R32G32B32A32_SINT
        DXGI_FORMAT_R32G32B32_TYPELESS
        DXGI_FORMAT_R32G32B32_FLOAT
        DXGI_FORMAT_R32G32B32_UINT
        DXGI_FORMAT_R32G32B32_SINT
        DXGI_FORMAT_R16G16B16A16_TYPELESS
        DXGI_FORMAT_R16G16B16A16_FLOAT
        DXGI_FORMAT_R16G16B16A16_UNORM
        DXGI_FORMAT_R16G16B16A16_UINT
        DXGI_FORMAT_R16G16B16A16_SNORM
        DXGI_FORMAT_R16G16B16A16_SINT
        DXGI_FORMAT_R32G32_TYPELESS
        DXGI_FORMAT_R32G32_FLOAT
        DXGI_FORMAT_R32G32_UINT
        DXGI_FORMAT_R32G32_SINT
        DXGI_FORMAT_R32G8X24_TYPELESS
        DXGI_FORMAT_D32_FLOAT_S8X24_UINT
        DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS
        DXGI_FORMAT_X32_TYPELESS_G8X24_UINT
        DXGI_FORMAT_R10G10B10A2_TYPELESS
        DXGI_FORMAT_R10G10B10A2_UNORM
        DXGI_FORMAT_R10G10B10A2_UINT
        DXGI_FORMAT_R11G11B10_FLOAT
        DXGI_FORMAT_R8G8B8A8_TYPELESS
        DXGI_FORMAT_R8G8B8A8_UNORM
        DXGI_FORMAT_R8G8B8A8_UNORM_SRGB
        DXGI_FORMAT_R8G8B8A8_UINT
        DXGI_FORMAT_R8G8B8A8_SNORM
        DXGI_FORMAT_R8G8B8A8_SINT
        DXGI_FORMAT_R16G16_TYPELESS
        DXGI_FORMAT_R16G16_FLOAT
        DXGI_FORMAT_R16G16_UNORM
        DXGI_FORMAT_R16G16_UINT
        DXGI_FORMAT_R16G16_SNORM
        DXGI_FORMAT_R16G16_SINT
        DXGI_FORMAT_R32_TYPELESS
        DXGI_FORMAT_D32_FLOAT
        DXGI_FORMAT_R32_FLOAT
        DXGI_FORMAT_R32_UINT
        DXGI_FORMAT_R32_SINT
        DXGI_FORMAT_R24G8_TYPELESS
        DXGI_FORMAT_D24_UNORM_S8_UINT
        DXGI_FORMAT_R24_UNORM_X8_TYPELESS
        DXGI_FORMAT_X24_TYPELESS_G8_UINT
        DXGI_FORMAT_R8G8_TYPELESS
        DXGI_FORMAT_R8G8_UNORM
        DXGI_FORMAT_R8G8_UINT
        DXGI_FORMAT_R8G8_SNORM
        DXGI_FORMAT_R8G8_SINT
        DXGI_FORMAT_R16_TYPELESS
        DXGI_FORMAT_R16_FLOAT
        DXGI_FORMAT_D16_UNORM
        DXGI_FORMAT_R16_UNORM
        DXGI_FORMAT_R16_UINT
        DXGI_FORMAT_R16_SNORM
        DXGI_FORMAT_R16_SINT
        DXGI_FORMAT_R8_TYPELESS
        DXGI_FORMAT_R8_UNORM
        DXGI_FORMAT_R8_UINT
        DXGI_FORMAT_R8_SNORM
        DXGI_FORMAT_R8_SINT
        DXGI_FORMAT_A8_UNORM
        DXGI_FORMAT_R1_UNORM
        DXGI_FORMAT_R9G9B9E5_SHAREDEXP
        DXGI_FORMAT_R8G8_B8G8_UNORM
        DXGI_FORMAT_G8R8_G8B8_UNORM
        DXGI_FORMAT_BC1_TYPELESS
        DXGI_FORMAT_BC1_UNORM
        DXGI_FORMAT_BC1_UNORM_SRGB
        DXGI_FORMAT_BC2_TYPELESS
        DXGI_FORMAT_BC2_UNORM
        DXGI_FORMAT_BC2_UNORM_SRGB
        DXGI_FORMAT_BC3_TYPELESS
        DXGI_FORMAT_BC3_UNORM
        DXGI_FORMAT_BC3_UNORM_SRGB
        DXGI_FORMAT_BC4_TYPELESS
        DXGI_FORMAT_BC4_UNORM
        DXGI_FORMAT_BC4_SNORM
        DXGI_FORMAT_BC5_TYPELESS
        DXGI_FORMAT_BC5_UNORM
        DXGI_FORMAT_BC5_SNORM
        DXGI_FORMAT_B5G6R5_UNORM
        DXGI_FORMAT_B5G5R5A1_UNORM
        DXGI_FORMAT_B8G8R8A8_UNORM
        DXGI_FORMAT_B8G8R8X8_UNORM
        DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM
        DXGI_FORMAT_B8G8R8A8_TYPELESS
        DXGI_FORMAT_B8G8R8A8_UNORM_SRGB
        DXGI_FORMAT_B8G8R8X8_TYPELESS
        DXGI_FORMAT_B8G8R8X8_UNORM_SRGB
        DXGI_FORMAT_BC6H_TYPELESS
        DXGI_FORMAT_BC6H_UF16
        DXGI_FORMAT_BC6H_SF16
        DXGI_FORMAT_BC7_TYPELESS
        DXGI_FORMAT_BC7_UNORM
        DXGI_FORMAT_BC7_UNORM_SRGB
        DXGI_FORMAT_AYUV
        DXGI_FORMAT_Y410
        DXGI_FORMAT_Y416
        DXGI_FORMAT_NV12
        DXGI_FORMAT_P010
        DXGI_FORMAT_P016
        DXGI_FORMAT_420_OPAQUE
        DXGI_FORMAT_YUY2
        DXGI_FORMAT_Y210
        DXGI_FORMAT_Y216
        DXGI_FORMAT_NV11
        DXGI_FORMAT_AI44
        DXGI_FORMAT_IA44
        DXGI_FORMAT_P8
        DXGI_FORMAT_A8P8
        DXGI_FORMAT_B4G4R4A4_UNORM
        DXGI_FORMAT_P208
        DXGI_FORMAT_V208
        DXGI_FORMAT_V408
        DXGI_FORMAT_FORCE_UINT
    End Enum

    Public Enum D3D10_RESOURCE_DIMENSION
        ''' <summary>
        ''' Resource is of unknown type.
        ''' </summary>
        D3D10_RESOURCE_DIMENSION_UNKNOWN
        ''' <summary>
        ''' Resource is a buffer.
        ''' </summary>
        D3D10_RESOURCE_DIMENSION_BUFFER
        ''' <summary>
        ''' Resource is a 1D texture.
        ''' </summary>
        D3D10_RESOURCE_DIMENSION_TEXTURE1D
        ''' <summary>
        ''' Resource is a 2D texture.
        ''' </summary>
        D3D10_RESOURCE_DIMENSION_TEXTURE2D
        ''' <summary>
        ''' Resource is a 3D texture.
        ''' </summary>
        D3D10_RESOURCE_DIMENSION_TEXTURE3D
    End Enum
End Namespace

