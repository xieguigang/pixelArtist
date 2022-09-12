Imports PixelArtist.Engine

''' <summary>
''' Environment state inputs
''' </summary>
''' <remarks>
''' vector size is 20
''' </remarks>
Public Structure GameControl : Implements ICloneable

    ''' <summary>
    ''' The relative position between the head of the snake and his food
    ''' </summary>
    ''' <remarks>
    ''' 4 bits
    ''' </remarks>
    Dim position As Controls
    ''' <summary>
    ''' The current movement direction of the snake.(蛇的当前运动方向)
    ''' </summary>
    ''' <remarks>
    ''' 8 bits
    ''' </remarks>
    Dim moveDIR As Controls

    ' 4 bits
    Dim wallTop As Byte
    Dim wallRight As Byte
    Dim wallBottom As Byte
    Dim wallLeft As Byte

    ' 4 bits
    Dim foodWallTop As Byte
    Dim foodWallRight As Byte
    Dim foodWallBottom As Byte
    Dim foodWallLeft As Byte

    ''' <summary>
    ''' hash key for the QTable
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return $"{CInt(position)},{vbTab}{CInt(moveDIR)} {wallTop}{wallRight}{wallBottom}{wallLeft} {foodWallTop}{foodWallRight}{foodWallBottom}{foodWallLeft}"
    End Function

    Public Function ToVector() As Double()
        Dim v As Double() = New Double(20 - 1) {}

        Select Case position
            Case Controls.Down : v(0) = 1
            Case Controls.Left : v(1) = 1
            Case Controls.Right : v(2) = 1
            Case Controls.Up : v(3) = 1
        End Select
        Select Case moveDIR
            Case Controls.Up : v(4) = 1
            Case Controls.Left : v(5) = 1
            Case Controls.Down : v(6) = 1
            Case Controls.Right : v(7) = 1
            Case Controls.Up Or Controls.Left : v(8) = 1
            Case Controls.Up Or Controls.Right : v(9) = 1
            Case Controls.Down Or Controls.Left : v(10) = 1
            Case Controls.Down Or Controls.Right : v(11) = 1
        End Select

        v(12) = wallTop
        v(13) = wallBottom
        v(14) = wallLeft
        v(15) = wallRight

        v(16) = foodWallTop
        v(17) = foodWallLeft
        v(18) = foodWallRight
        v(19) = foodWallBottom

        Return v
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Function Clone() As Object Implements ICloneable.Clone
        Return New GameControl With {
            .position = position,
            .moveDIR = moveDIR,
            .wallLeft = wallLeft,
            .wallBottom = wallBottom,
            .wallRight = wallRight,
            .wallTop = wallTop,
            .foodWallLeft = foodWallLeft,
            .foodWallBottom = foodWallBottom,
            .foodWallRight = foodWallRight,
            .foodWallTop = foodWallTop
        }
    End Function
End Structure
