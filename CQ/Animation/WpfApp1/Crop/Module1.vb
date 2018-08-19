Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports OCR

Module Module1

    Sub Main()
        Call GetType(Module1).RunCLI(App.CommandLine)
    End Sub

    <ExportAPI("/do")>
    <Usage("/do /in <directory> [/out <directory>]")>
    Public Function doCrop(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or ([in].TrimDIR & ".crop")
        Dim projections As New List(Of (X As Vector, Y As Vector))

        ' 首先扫描所有帧，得到边界
        For Each png As String In ls - l - r - "*.png" <= [in]
            Dim frame As Image = png.LoadImage
            projections += (frame.Projection(True, Color.Transparent), frame.Projection(False, Color.Transparent))
        Next

        Dim width = projections.First.X.Length
        Dim height = projections.First.Y.Length

        Dim top = projections.Select(Function(f) f.Y.TakeWhile(Function(p) p = 0R).Count).Min
        Dim left = projections.Select(Function(f) f.X.TakeWhile(Function(p) p = 0R).Count).Min
        Dim right = width - projections.Select(Function(f) f.X.Reverse.TakeWhile(Function(p) p = 0R).Count).Min
        Dim bottom = height - projections.Select(Function(f) f.Y.Reverse.TakeWhile(Function(p) p = 0R).Count).Min
        Dim rect As New RectangleF(left, top, right - left, bottom - top)

        For Each png As String In ls - l - r - "*.png" <= [in]
            Dim frame As Image = png.LoadImage
            Call New Bitmap(frame).ImageCrop(rect).SaveAs($"{out}/{png.BaseName}.png", ImageFormats.Png)
        Next

        Return 0
    End Function
End Module
