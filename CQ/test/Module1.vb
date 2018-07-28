Imports Testures

Module Module1

    Sub Main()
        Call ddsTest()
    End Sub

    Sub ddsTest()
        Dim file As DDS.File = DDS.ReadFile("C:\Users\Administrator\Downloads\UtinyRipper-master\Bins\Debug\Ripped\0000000000000000f000000000000000\Texture2D\Default-Particle.dds")

        Pause()
    End Sub


End Module
