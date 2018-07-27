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

        slicers += newSlicer

        Canvas.Children.Add(newSlicer)
        Canvas.SetLeft(newSlicer, 0)
        Canvas.SetTop(newSlicer, Margintop)

        Margintop += newSlicer.Height + 10
    End Sub

    Public Function GetSlicers() As Dictionary(Of String, rect)
        Dim checkDuplicated = slicers.GroupBy(Function(s) s.slicerNameString).Where(Function(sg) sg.Count > 1).Select(Function(sg) sg.Key).ToArray

        If checkDuplicated.Length > 0 Then

            MsgBox(checkDuplicated.GetJson, MsgBoxStyle.Critical, Title:="Duplicated Name found!!!")

            Return New Dictionary(Of String, rect)
        End If

        Return slicers _
            .ToDictionary(Function(s) s.slicerNameString,
                          Function(s)
                              Return New rect With {
                                .left = s.rect.left,
                                .top = s.rect.top,
                                .width = s.rect.width,
                                .height = s.rect.height
                              }
                          End Function)
    End Function
End Class
