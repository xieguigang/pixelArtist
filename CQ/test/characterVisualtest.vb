Imports System.Drawing
Imports Character
Imports Microsoft.VisualBasic.Imaging

Module characterVisualtest
    Sub Main()
        Dim texture As Bitmap = "E:\VB_GamePads\CQ\pr_6_1\texture_cos_wi_17_20.png".LoadImage

        Call texture.Head.SaveAs("./head.png")

    End Sub
End Module
