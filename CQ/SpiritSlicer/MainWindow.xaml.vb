Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Win32

Class MainWindow

    Private Sub viewer_MouseWheel(sender As Object, e As MouseWheelEventArgs) Handles viewer.MouseWheel
        Dim height# = viewer.Height
        Dim width# = viewer.Width
        Dim delta% = -e.Delta

        height += delta
        width += delta

        viewer.Height = If(height < 150, 150, height)
        viewer.Width = If(width < 200, 200, width)
    End Sub

    Dim imageFile$
    Dim slicers As RectangleInputsWindow

    ''' <summary>
    ''' 打开精灵图
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenSpiritImage_Click(sender As Object, e As RoutedEventArgs) Handles OpenSpiritImage.Click
        Dim openFileDialog As New OpenFileDialog()

        If (openFileDialog.ShowDialog() = True) Then
            viewer.Source = New System.Windows.Media.Imaging.BitmapImage(New Uri(openFileDialog.FileName))
            imageFile = openFileDialog.FileName

            Dim win2 As New RectangleInputsWindow With {.flush = AddressOf showRectangles}
            win2.Show()
            slicers = win2
        End If
    End Sub

    Sub showRectangles()
        Using g As Graphics2D = imageFile.LoadImage.CreateCanvas2D(directAccess:=True)
            For Each rect In slicers.GetSlicers
                Dim r As New System.Drawing.RectangleF With {
                    .X = rect.Value.left,
                    .Y = rect.Value.top,
                    .Width = rect.Value.width,
                    .Height = rect.Value.height
                }

                Call g.DrawRectangle(New System.Drawing.Pen(System.Drawing.Color.Red, 5), r)
            Next

            Dim temp = App.GetAppSysTempFile("." & imageFile.ExtensionSuffix, App.PID)

            Call g.Save(temp, ImageFormats.Png)
            viewer.Source = New System.Windows.Media.Imaging.BitmapImage(New Uri(temp))
        End Using
    End Sub

    ''' <summary>
    ''' 保存精灵图切割矩阵集合
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SaveSlicerJson_Click(sender As Object, e As RoutedEventArgs) Handles SaveSlicerJson.Click
        If Not slicers Is Nothing Then
            Call slicers.GetSlicers.GetJson.SaveTo(imageFile.TrimSuffix & ".slicers.json")
        End If
    End Sub

    Dim mousePosition As Point
    Dim capture As Boolean

    Private Sub Canvas_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles Canvas.MouseLeftButtonDown
        mousePosition = e.GetPosition(Canvas)
        capture = True

        Panel.SetZIndex(viewer, 1)
    End Sub

    Private Sub Canvas_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles Canvas.MouseLeftButtonUp
        capture = False
        Panel.SetZIndex(viewer, 0)
    End Sub

    Private Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs) Handles Canvas.MouseMove
        If capture Then
            Dim position = e.GetPosition(Canvas)
            Dim offset = position - mousePosition

            mousePosition = position

            Canvas.SetLeft(viewer, Canvas.GetLeft(viewer) + offset.X)
            Canvas.SetTop(viewer, Canvas.GetTop(viewer) + offset.Y)
        End If
    End Sub
End Class
