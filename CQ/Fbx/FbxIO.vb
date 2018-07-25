Imports System.IO

''' <summary>
''' Static read and write methods
''' </summary>
''' <remarks>
''' IO is an acronym
''' ReSharper disable once InconsistentNaming
''' </remarks>
Public Module FbxIO

    ''' <summary>
    ''' Reads a binary FBX file
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns>The top level document node</returns>
    Public Function ReadBinary(path As String) As FbxDocument
        If path Is Nothing Then
            Throw New ArgumentNullException(NameOf(path))
        End If
        Using stream = New FileStream(path, FileMode.Open)
            Dim reader = New FbxBinaryReader(stream)
            Return reader.Read()
        End Using
    End Function

    ''' <summary>
    ''' Reads an ASCII FBX file
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns>The top level document node</returns>
    Public Function ReadAscii(path As String) As FbxDocument
        If path Is Nothing Then
            Throw New ArgumentNullException(NameOf(path))
        End If
        Using stream = New FileStream(path, FileMode.Open)
            Dim reader = New FbxAsciiReader(stream)
            Return reader.Read()
        End Using
    End Function

    ''' <summary>
    ''' Writes an FBX document
    ''' </summary>
    ''' <param name="document">The top level document node</param>
    ''' <param name="path"></param>
    Public Sub WriteBinary(document As FbxDocument, path As String)
        If path Is Nothing Then
            Throw New ArgumentNullException(NameOf(path))
        End If
        Using stream = New FileStream(path, FileMode.Create)
            Dim writer = New FbxBinaryWriter(stream)
            writer.Write(document)
        End Using
    End Sub

    ''' <summary>
    ''' Writes an FBX document
    ''' </summary>
    ''' <param name="document">The top level document node</param>
    ''' <param name="path"></param>
    Public Sub WriteAscii(document As FbxDocument, path As String)
        If path Is Nothing Then
            Throw New ArgumentNullException(NameOf(path))
        End If
        Using stream = New FileStream(path, FileMode.Create)
            Dim writer = New FbxAsciiWriter(stream)
            writer.Write(document)
        End Using
    End Sub
End Module
