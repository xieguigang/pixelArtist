Imports System.Drawing
Imports System.Runtime.CompilerServices

Public Module Assembler

    ''' <summary>
    ''' 将碎片拼接为一个完整的图片
    ''' </summary>
    ''' <param name="puzzle"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Assembling(puzzle As IEnumerable(Of Image)) As Image

    End Function

    ''' <summary>
    ''' 将精灵图进行自动切割
    ''' </summary>
    ''' <param name="spirit"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Split(spirit As Image) As IEnumerable(Of Image)

    End Function

    Public Function Delta(p1 As Color, p2 As Color) As Double
        Dim r# = p1.R - p2.R
        Dim g# = p1.G - p2.G
        Dim b# = p1.B - p2.B
        Dim a# = p1.A - p2.A

        Return {r, g, b, a} _
            .Select(AddressOf Math.Abs) _
            .Average
    End Function
End Module
