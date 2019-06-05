Imports System.Drawing
Imports Character
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Module characterVisualtest
    Sub Main()
        Call sliceImages("E:\VB_GamePads\CQ\pr_6_1\texture_cos_wi_17_20.png", "E:\VB_GamePads\CQ\pr_6_1\slices")
        Call sliceImages("E:\VB_GamePads\CQ\pr_6_1\texture_pr_6_1.png", "E:\VB_GamePads\CQ\pr_6_1\slices")
    End Sub

    Sub sliceImages(source As String, output$)
        Dim texture As Bitmap = source.LoadImage

        Call texture.Head.SaveAs($"{output}/{source.BaseName}/head.png")

        For Each face As SeqValue(Of Bitmap) In texture.Faces.SeqIterator
            Call face.value.SaveAs($"{output}/{source.BaseName}/face_{face.i}.png")
        Next
    End Sub
End Module
