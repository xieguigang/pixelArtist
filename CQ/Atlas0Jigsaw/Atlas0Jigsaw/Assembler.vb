Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.BitmapImage

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
    Public Iterator Function Split(spirit As Image, Optional delta# = 10, Optional continues% = 30) As IEnumerable(Of Image)
        Dim Xslices As New List(Of Integer)
        Dim Yslices As New List(Of Integer)

        Using buffer As BitmapBuffer = BitmapBuffer.FromImage(spirit)

            ' 首先进行纵向扫描，即沿着X轴进行垂直扫描
            For x As Integer = 0 To buffer.Width - 2
                Dim n As New List(Of Integer)

                For y As Integer = 0 To buffer.Height - 1
                    Dim p As Color = buffer.GetPixel(x, y)
                    Dim pNext As Color = buffer.GetPixel(x + 1, y)

                    If Assembler.Delta(p, pNext) > delta Then
                        Call n.Add(y)
                    End If
                Next

                Dim regions = n.Split().ToArray
                Dim regionSlicer = regions _
                    .Where(Function(sec) sec.Length >= continues) _
                    .ToArray

                If regionSlicer.Length > 0 Then
                    Xslices.Add(x)

                    For Each region In regionSlicer
                        Yslices.Add(region.Min)
                        Yslices.Add(region.Max)
                    Next
                End If
            Next
        End Using

        ' 得到切割线之后，进行区域切割

    End Function

    <Extension>
    Private Iterator Function Split(n As IEnumerable(Of Integer), Optional delta% = 10) As IEnumerable(Of IntRange)
        Dim pre As Integer = n.First
        Dim min% = pre

        For Each x As Integer In n.Skip(1)
            If x - pre <= delta Then
                ' do nothing
            Else
                ' 断开了
                Yield New IntRange(min, pre)
                min = x
            End If

            pre = x
        Next
    End Function

    Public Function Delta(p1 As Color, p2 As Color) As Double
        Dim r# = CInt(p1.R) - p2.R
        Dim g# = CInt(p1.G) - p2.G
        Dim b# = CInt(p1.B) - p2.B
        Dim a# = CInt(p1.A) - p2.A

        Return {r, g, b, a} _
            .Select(AddressOf Math.Abs) _
            .Average
    End Function
End Module
