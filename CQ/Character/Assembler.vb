Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging

Public Module Assembler

    <Extension>
    Public Function DrawNormal(visual As Character) As Bitmap
        Using g As Graphics2D = (200, 200).CreateGDIDevice(filled:=Color.Transparent)

            Call g.DrawImageUnscaled(visual.leftHair, New Point(75, 70))

            Call g.DrawImageUnscaled(visual.head, New Point(52, 10))
            Call g.DrawImageUnscaled(visual.backHand, New Point(134, 110))
            Call g.DrawImageUnscaled(visual.body, New Point(108, 105))
            Call g.DrawImageUnscaled(visual.foot, New Point(117, 130))
            Call g.DrawImageUnscaled(visual.foot, New Point(127, 130))
            Call g.DrawImageUnscaled(visual.frontHand, New Point(110, 110))

            Return g.ImageResource
        End Using
    End Function
End Module
