Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices

Public Module PngResource

    Public Function idle() As IEnumerable(Of MemoryStream)
        Return loadResource(NameOf(My.Resources.idle))
    End Function

    Public Function walk() As IEnumerable(Of MemoryStream)
        Return loadResource(NameOf(My.Resources.walk))
    End Function

    Private Iterator Function loadResource(<CallerMemberName> Optional name$ = Nothing) As IEnumerable(Of MemoryStream)
        Dim data As Byte() = My.Resources.ResourceManager.GetObject(name)

        Using zip As New ZipArchive(New MemoryStream(data))
            For Each entry In zip.Entries.OrderBy(Function(e) e.Name)
                Using png As Stream = entry.Open
                    Dim ms As New MemoryStream()
                    Call png.CopyTo(ms)
                    Yield ms
                End Using
            Next
        End Using
    End Function
End Module
