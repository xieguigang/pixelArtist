Imports System.ComponentModel
Imports Microsoft.VisualBasic.Data.Plots
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Q-Learning 训练得分曲线查看器。
'''
''' 原先使用 ChartPlots 的 LinePlot2D + CSS 主题渲染，现在改用 DataPlot 的
''' 折线图（<see cref="LinePlot"/>）：样条平滑 + 填充由 <see cref="AreaPlot"/> 承担，
''' 主题改为强类型的 <see cref="PlotTheme"/>。
''' </summary>
Public Class FormPlotViewer

    Public Sub PlotScore(scores As IEnumerable(Of Double))
        Dim data = scores.SafeQuery.ToArray
        Dim line As New Series With {
            .Name = "Score",
            .Color = Color.Blue,
            .MarkerShape = MarkerShape.None,
            .PointSize = 16,
            .X = Enumerable.Range(0, data.Length).Select(Function(i) CDbl(i)).ToArray,
            .Y = data
        }

        Using area As New AreaPlot(2500, 1600, PlotTheme.Light())
            area.Title = "Q-Learning AI Game Score"
            area.XLabel = "Iteration"
            area.YLabel = "Game Score"
            area.Smooth = True

            area.Plot({line}.ToList())

            Dim image = area.AsGraphicsData().AsGDIImage

            PictureBox1.BackgroundImage = image.CTypeGdiImage
        End Using
    End Sub

    Private Sub FormPlotViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        WindowState = FormWindowState.Minimized
    End Sub
End Class
