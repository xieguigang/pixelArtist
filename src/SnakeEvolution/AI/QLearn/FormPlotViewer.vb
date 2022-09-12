Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Interpolation

Public Class FormPlotViewer

    Public Sub PlotScore(scores As IEnumerable(Of Double))
        Dim line As New SerialData With {
            .color = Color.Blue,
            .lineType = DashStyle.Solid,
            .pointSize = 16,
            .shape = LegendStyles.Circle,
            .title = "Score",
            .width = 8,
            .pts = scores _
                .Select(Function(d, i) New PointData(i, d)) _
                .ToArray
        }
        Dim theme As New Theme With {.padding = "padding: 200px 300px 200px 200px;"}
        Dim app As New LinePlot2D({line}, theme, fill:=True, fillPie:=True, Splines.B_Spline) With {
            .legendTitle = "Score",
            .main = "Q-Learning AI Game Score",
            .xlabel = "Iteration",
            .ylabel = "Game Score"
        }
        Dim image = app.Plot("1600,1200").AsGDIImage

        PictureBox1.BackgroundImage = image
    End Sub
End Class