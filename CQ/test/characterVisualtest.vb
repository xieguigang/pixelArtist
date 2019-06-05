Imports System.Drawing
Imports Character
Imports Microsoft.VisualBasic.Imaging

Module characterVisualtest
    Sub Main()
        Call sliceImages("E:\VB_GamePads\CQ\pr_6_1\texture_cos_wi_17_20.png", "E:\VB_GamePads\CQ\pr_6_1\slices")

    End Sub

    Sub sliceImages(source As String, output$)
        Dim texture As Bitmap = source.LoadImage

        Call texture.Head.SaveAs($"{output}/{source.BaseName}/head.png")
    End Sub
End Module
