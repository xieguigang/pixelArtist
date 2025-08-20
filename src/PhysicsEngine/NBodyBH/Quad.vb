''' <summary>
''' Quad.java
''' 
''' Represents quadrants for the Barnes-Hut algorithm. 
''' 
''' Dependencies: StdDraw.java
''' 
''' @author chindesaurus
''' @version 1.00
''' </summary>

Public Class Quad

    Private xmid As Double
    Private ymid As Double
    'JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
    Private length_Conflict As Double

    ''' <summary>
    ''' Constructor: creates a new Quad with the given 
    ''' parameters (assume it is square).
    ''' </summary>
    ''' <paramname="xmid">   x-coordinate of center of quadrant </param>
    ''' <paramname="ymid">   y-coordinate of center of quadrant </param>
    ''' <paramname="length"> the side length of the quadrant </param>
    Public Sub New(xmid As Double, ymid As Double, length As Double)
        Me.xmid = xmid
        Me.ymid = ymid
        length_Conflict = length
    End Sub

    ''' <summary>
    ''' Returns the length of one side of the square quadrant.
    ''' </summary>
    ''' <returns> side length of the quadrant </returns>
    Public Overridable Function length() As Double
        Return length_Conflict
    End Function

    ''' <summary>
    ''' Does this quadrant contain (x, y)?
    ''' </summary>
    ''' <paramname="x"> x-coordinate of point to test </param>
    ''' <paramname="y"> y-coordinate of point to test </param>
    ''' <returns>  true if quadrant contains (x, y), else false </returns>
    Public Overridable Function contains(x As Double, y As Double) As Boolean
        Dim halfLen = length_Conflict / 2.0
        Return x <= xmid + halfLen AndAlso x >= xmid - halfLen AndAlso y <= ymid + halfLen AndAlso y >= ymid - halfLen
    End Function

    ''' <summary>
    ''' Returns a new object that represents the northwest quadrant.
    ''' </summary>
    ''' <returns> the northwest quadrant of this Quad </returns>
    Public Overridable Function NW() As Quad
        Dim x = xmid - length_Conflict / 4.0
        Dim y = ymid + length_Conflict / 4.0
        Dim len = length_Conflict / 2.0
        Dim lNW As Quad = New Quad(x, y, len)
        Return lNW
    End Function

    ''' <summary>
    ''' Returns a new object that represents the northeast quadrant.
    ''' </summary>
    ''' <returns> the northeast quadrant of this Quad </returns>
    Public Overridable Function NE() As Quad
        Dim x = xmid + length_Conflict / 4.0
        Dim y = ymid + length_Conflict / 4.0
        Dim len = length_Conflict / 2.0
        Dim lNE As Quad = New Quad(x, y, len)
        Return lNE
    End Function

    ''' <summary>
    ''' Returns a new object that represents the southwest quadrant.
    ''' </summary>
    ''' <returns> the southwest quadrant of this Quad </returns>
    Public Overridable Function SW() As Quad
        Dim x = xmid - length_Conflict / 4.0
        Dim y = ymid - length_Conflict / 4.0
        Dim len = length_Conflict / 2.0
        Dim lSW As Quad = New Quad(x, y, len)
        Return lSW
    End Function

    ''' <summary>
    ''' Returns a new object that represents the southeast quadrant.
    ''' </summary>
    ''' <returns> the southeast quadrant of this Quad </returns>
    Public Overridable Function SE() As Quad
        Dim x = xmid + length_Conflict / 4.0
        Dim y = ymid - length_Conflict / 4.0
        Dim len = length_Conflict / 2.0
        Dim lSE As Quad = New Quad(x, y, len)
        Return lSE
    End Function

    ''' <summary>
    ''' Draws an unfilled rectangle that represents the quadrant.
    ''' </summary>
    Public Overridable Sub draw()
        ' StdDraw.rectangle(xmid, ymid, length_Conflict / 2.0, length_Conflict / 2.0);
    End Sub

    ''' <summary>
    ''' Returns a string representation of this quadrant for debugging.
    ''' </summary>
    ''' <returns> string representation of this quadrant </returns>
    Public Overrides Function ToString() As String
        Dim ret = vbLf
        For row As Integer = 0 To length_Conflict - 1
            For col As Integer = 0 To length_Conflict - 1
                If row = 0 OrElse col = 0 OrElse row = length_Conflict - 1 OrElse col = length_Conflict - 1 Then
                    ret += "*"
                Else
                    ret += " "
                End If
            Next
            ret += vbLf
        Next
        Return ret
    End Function
End Class
