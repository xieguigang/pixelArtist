Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq

Module Program

    Sub Main()
        Call test()
    End Sub

    Sub testSlice()
        Dim raw As Image = "E:\VB_GamePads\CQ\Atlas0Jigsaw\atlas0 #039450.png".LoadImage
        Dim slices As Image() = Assembler.Split(raw).ToArray

        For Each fragment In slices.SeqIterator
            Call fragment.value.SaveAs($"./{fragment.i}.png")
        Next
    End Sub

    Sub test()
        Dim fragments As IEnumerable(Of Image) = (ls - l - r - "*.png" <= "E:\VB_GamePads\CQ\Atlas0Jigsaw\test").Select(AddressOf LoadImage)
        Dim atlas As Image = fragments.Assembling

        Call atlas.SaveAs("./test.png")
    End Sub
End Module
