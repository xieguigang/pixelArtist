Imports System.ComponentModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Interpolation
Imports DashStyle = System.Drawing.Drawing2D.DashStyle

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
        Dim theme As New Theme With {.padding = "padding: 200px 500px 300px 300px;"}
        Dim app As New LinePlot2D({line}, theme, fill:=True, fillPie:=True, Splines.B_Spline) With {
            .legendTitle = "Score",
            .main = "Q-Learning AI Game Score",
            .xlabel = "Iteration",
            .ylabel = "Game Score"
        }
        Dim image = app.Plot("2500,1600").AsGDIImage

        PictureBox1.BackgroundImage = image.CTypeImage
    End Sub

    Private Sub FormPlotViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        WindowState = FormWindowState.Minimized
    End Sub
End Class