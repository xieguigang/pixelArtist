Imports DWORD = System.Int32

Namespace DDS.DDS_PIXELFORMAT

    'struct DDS_PIXELFORMAT {
    '  DWORD dwSize;
    '  DWORD dwFlags;
    '  DWORD dwFourCC;
    '  DWORD dwRGBBitCount;
    '  DWORD dwRBitMask;
    '  DWORD dwGBitMask;
    '  DWORD dwBBitMask;
    '  DWORD dwABitMask;
    '};

    ''' <summary>
    ''' Surface pixel format.
    ''' 
    ''' > https://docs.microsoft.com/en-us/windows/desktop/direct3ddds/dds-pixelformat
    ''' </summary>
    ''' <remarks>
    ''' To store DXGI formats such as floating-point data, use a dwFlags of DDPF_FOURCC and set 
    ''' dwFourCC to 'D','X','1','0'. Use the DDS_HEADER_DXT10 extension header to store the DXGI 
    ''' format in the dxgiFormat member.
    ''' 
    ''' Note that there are non-standard variants Of DDS files where dwFlags has DDPF_FOURCC And 
    ''' the dwFourCC value Is Set directly To a D3DFORMAT Or DXGI_FORMAT enumeration value. It 
    ''' Is Not possible To disambiguate the D3DFORMAT versus DXGI_FORMAT values Using this 
    ''' non-standard scheme, so the DX10 extension header Is recommended instead.
    ''' </remarks>
    Public Class PixelFormat

#Region "struct DDS_PIXELFORMAT {"

        ''' <summary>
        ''' Structure size; set to 32 (bytes).
        ''' </summary>
        Public dwSize As DWORD
        Public dwFlags As dwFlags
        ''' <summary>
        ''' Four-character codes for specifying compressed or custom formats. Possible 
        ''' values include: DXT1, DXT2, DXT3, DXT4, or DXT5. A FourCC of DX10 indicates 
        ''' the prescense of the DDS_HEADER_DXT10 extended header, and the dxgiFormat 
        ''' member of that structure indicates the true format. When using a four-character 
        ''' code, dwFlags must include DDPF_FOURCC.
        ''' </summary>
        Public dwFourCC As DWORD
        ''' <summary>
        ''' Number of bits in an RGB (possibly including alpha) format. Valid when dwFlags 
        ''' includes DDPF_RGB, DDPF_LUMINANCE, or DDPF_YUV.
        ''' </summary>
        Public dwRGBBitCount As DWORD
        ''' <summary>
        ''' Red (or lumiannce or Y) mask for reading color data. For instance, given the 
        ''' A8R8G8B8 format, the red mask would be 0x00ff0000.
        ''' </summary>
        Public dwRBitMask As DWORD
        ''' <summary>
        ''' Green (or U) mask for reading color data. For instance, given the A8R8G8B8 format, 
        ''' the green mask would be 0x0000ff00.
        ''' </summary>
        Public dwGBitMask As DWORD
        ''' <summary>
        ''' Blue (or V) mask for reading color data. For instance, given the A8R8G8B8 format, 
        ''' the blue mask would be 0x000000ff.
        ''' </summary>
        Public dwBBitMask As DWORD
        ''' <summary>
        ''' Alpha mask for reading alpha data. dwFlags must include DDPF_ALPHAPIXELS or DDPF_ALPHA. 
        ''' For instance, given the A8R8G8B8 format, the alpha mask would be 0xff000000.
        ''' </summary>
        Public dwABitMask As DWORD
#End Region

    End Class

    ''' <summary>
    ''' Values which indicate what type of data is in the surface.
    ''' </summary>
    Public Enum dwFlags As DWORD

        ''' <summary>
        ''' Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
        ''' </summary>
        DDPF_ALPHAPIXELS = &H1
        ''' <summary>
        ''' Used In some older DDS files For alpha channel only uncompressed data 
        ''' (dwRGBBitCount contains the alpha channel bitcount; dwABitMask 
        ''' contains valid data)
        ''' </summary>
        DDPF_ALPHA = &H2
        ''' <summary>
        ''' Texture contains compressed RGB data; dwFourCC contains valid data.
        ''' </summary>
        DDPF_FOURCC = &H4
        ''' <summary>
        ''' Texture contains uncompressed RGB data; dwRGBBitCount And the RGB masks 
        ''' (dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
        ''' </summary>
        DDPF_RGB = &H40
        ''' <summary>
        ''' Used In some older DDS files For YUV uncompressed data (dwRGBBitCount 
        ''' contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask 
        ''' contains the U mask, dwBBitMask contains the V mask)
        ''' </summary>
        DDPF_YUV = &H200
        ''' <summary>
        ''' Used In some older DDS files For Single channel color uncompressed data 
        ''' (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains 
        ''' the channel mask). Can be combined With DDPF_ALPHAPIXELS For a two channel 
        ''' DDS file.
        ''' </summary>
        DDPF_LUMINANCE = &H200
    End Enum
End Namespace


