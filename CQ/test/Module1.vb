Imports Testures

Module Module1

    Sub Main()
        Call scanGraph()
        Call ddsTest()
    End Sub

    Sub ddsTest()
        Dim file As DDS.File = DDS.ReadFile("C:\Users\Administrator\Downloads\UtinyRipper-master\Bins\Debug\Ripped\0000000000000000f000000000000000\Texture2D\Default-Particle.dds")

        Pause()
    End Sub

    Sub scanGraph()
        Dim yamlDoc = Microsoft.VisualBasic.MIME.text.yaml.Grammar.YamlParser.Load("D:\OneDrive\lilo.18580\export\AnimationClip\AnimationClip\idle2.anim")

        Pause()
    End Sub
End Module
