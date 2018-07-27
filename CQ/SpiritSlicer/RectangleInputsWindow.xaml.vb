Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class RectangleInputsWindow

    Dim Margintop% = 5
    Dim slicers As New List(Of SlicerInput)

    Public flush As Action

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim newSlicer As New SlicerInput With {
            .Height = 80,
            .HorizontalAlignment = HorizontalAlignment.Stretch,
            .flush = Me.flush
        }

        slicers += New SlicerInput

        Canvas.Children.Add(newSlicer)
        Canvas.SetLeft(newSlicer, 0)
        Canvas.SetTop(newSlicer, Margintop)

        Margintop += newSlicer.Height + 10
    End Sub

    Public Function GetSlicers() As Dictionary(Of String, rect)
        Dim checkDuplicated = slicers.GroupBy(Function(s) s.sliceName.Text).Where(Function(sg) sg.Count > 1).Select(Function(sg) sg.Key).ToArray

        If checkDuplicated.Length > 0 Then

            MsgBox(checkDuplicated.GetJson, MsgBoxStyle.Critical, Title:="Duplicated Name found!!!")

            Return New Dictionary(Of String, rect)
        End If

        Return slicers _
            .ToDictionary(Function(s) s.sliceName.Text,
                          Function(s)
                              Return New rect With {
                                .left = Val(s.rLeft.Text),
                                .top = Val(s.rTop.Text),
                                .width = Val(s.rWidth.Text),
                                .height = Val(s.rHeight.Text)
                              }
                          End Function)
    End Function
End Class
