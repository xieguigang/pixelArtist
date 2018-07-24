Imports Fbx

Module Program

    Sub Main()
        ' Read a file
        Dim documentNode = FbxIO.ReadBinary(App.CommandLine.Name)
        ' Update a property
        documentNode("Creator").Value = App.AssemblyName

        ' Preview the file in the console
        Call New FbxAsciiWriter(Console.OpenStandardOutput()).Write(documentNode)
    End Sub
End Module
