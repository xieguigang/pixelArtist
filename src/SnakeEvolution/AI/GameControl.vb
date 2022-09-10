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

    ''' <summary>
    ''' hash key for the QTable
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return $"{CInt(position)},{vbTab}{CInt(moveDIR)}"
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New GameControl With {
            .position = position,
            .moveDIR = moveDIR
        }
    End Function
End Structure
