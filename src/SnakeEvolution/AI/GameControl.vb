Imports PixelArtist.Engine

''' <summary>
''' Environment state inputs
''' </summary>
Public Structure GameControl : Implements ICloneable

    ''' <summary>
    ''' The relative position between the head of the snake and his food
    ''' </summary>
    Dim position As Controls
    ''' <summary>
    ''' The current movement direction of the snake.(蛇的当前运动方向)
    ''' </summary>
    Dim moveDIR As Controls

    Dim wallTop As Byte
    Dim wallRight As Byte
    Dim wallBottom As Byte
    Dim wallLeft As Byte

    ''' <summary>
    ''' hash key for the QTable
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return $"{CInt(position)},{vbTab}{CInt(moveDIR)} {wallTop}{wallRight}{wallBottom}{wallLeft}"
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New GameControl With {
            .position = position,
            .moveDIR = moveDIR,
            .wallLeft = wallLeft,
            .wallBottom = wallBottom,
            .wallRight = wallRight,
            .wallTop = wallTop
        }
    End Function
End Structure
