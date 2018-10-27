Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.UnixBash

Module Program

    Sub Main()
        Call test()
    End Sub

    Sub test()
        Dim fragments As IEnumerable(Of Image) = (ls - l - r - "*.png" <= "E:\VB_GamePads\CQ\Atlas0Jigsaw\test").Select(AddressOf LoadImage)
        Dim atlas As Image = fragments.Assembling

        Call atlas.SaveAs("./test.png")
    End Sub
End Module
