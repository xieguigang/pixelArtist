Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices

''' <summary>
''' Writes an FBX document to a binary stream
''' </summary>
Public Class FbxBinaryWriter
	Inherits FbxBinary
	Private ReadOnly output As Stream
	Private ReadOnly memory As MemoryStream
	Private ReadOnly stream As BinaryWriter

	ReadOnly nodePath As New Stack(Of String)()

	''' <summary>
	''' The minimum size of an array in bytes before it is compressed
	''' </summary>
	Public CompressionThreshold As Integer = 1024

	''' <summary>
	''' Creates a new writer
	''' </summary>
	''' <param name="stream"></param>
	Public Sub New(stream As Stream)
		If stream Is Nothing Then
			Throw New ArgumentNullException(nameof(stream))
		End If
		output = stream
		' Wrap in a memory stream to guarantee seeking
		memory = New MemoryStream()
		Me.stream = New BinaryWriter(memory, Encoding.ASCII)
	End Sub

	Private Delegate Sub PropertyWriter(sw As BinaryWriter, obj As Object)

	Private Structure WriterInfo
		Public ReadOnly id As Char
		Public ReadOnly writer As PropertyWriter

		Public Sub New(id As Char, writer As PropertyWriter)
			Me.id = id
			Me.writer = writer
		End Sub
	End Structure

    ' null elements indicate arrays - they are checked again with their element type
    Private Shared ReadOnly writePropertyActions As New Dictionary(Of Type, WriterInfo)() From {
        {GetType(Integer), New WriterInfo("I"c, Sub(sw, obj) sw.Write(CInt(obj)))},
        {GetType(Short), New WriterInfo("Y"c, Sub(sw, obj) sw.Write(CShort(obj)))},
        {GetType(Long), New WriterInfo("L"c, Sub(sw, obj) sw.Write(CLng(obj)))},
        {GetType(Single), New WriterInfo("F"c, Sub(sw, obj) sw.Write(CSng(obj)))},
        {GetType(Double), New WriterInfo("D"c, Sub(sw, obj) sw.Write(CDbl(obj)))},
        {GetType(Boolean), New WriterInfo("C"c, Sub(sw, obj) sw.Write(CByte(AscW(CChar(obj)))))},
        {GetType(Byte()), New WriterInfo("R"c, AddressOf WriteRaw)},
        {GetType(String), New WriterInfo("S"c, AddressOf WriteString)},
        {GetType(Integer()), New WriterInfo("i"c, Nothing)},
        {GetType(Long()), New WriterInfo("l"c, Nothing)},
        {GetType(Single()), New WriterInfo("f"c, Nothing)},
        {GetType(Double()), New WriterInfo("d"c, Nothing)},
        {GetType(Boolean()), New WriterInfo("b"c, Nothing)}
    }

    Private Shared Sub WriteRaw(stream As BinaryWriter, obj As Object)
		Dim bytes = DirectCast(obj, Byte())
		stream.Write(bytes.Length)
		stream.Write(bytes)
	End Sub

	Private Shared Sub WriteString(stream As BinaryWriter, obj As Object)
		Dim str = obj.ToString()
		' Replace "::" with \0\1 and reverse the tokens
		If str.Contains(asciiSeparator) Then
            Dim tokens = str.Split({asciiSeparator}, StringSplitOptions.None)
            Dim sb = New StringBuilder()
			Dim first As Boolean = True
			For i As Integer = tokens.Length - 1 To 0 Step -1
				If Not first Then
					sb.Append(binarySeparator)
				End If
				sb.Append(tokens(i))
				first = False
			Next
			str = sb.ToString()
		End If
		Dim bytes = Encoding.ASCII.GetBytes(str)
		stream.Write(bytes.Length)
		stream.Write(bytes)
	End Sub

	Private Sub WriteArray(array As Array, elementType As Type, writer As PropertyWriter)
		stream.Write(array.Length)

		Dim size = array.Length * Marshal.SizeOf(elementType)
		Dim compress As Boolean = size >= CompressionThreshold
		stream.Write(If(compress, 1, 0))

		Dim sw = stream
		Dim codec As DeflateWithChecksum = Nothing

		Dim compressLengthPos = stream.BaseStream.Position
		stream.Write(0)
		' Placeholder compressed length
		Dim dataStart = stream.BaseStream.Position
		If compress Then
			stream.Write(New Byte() {&H58, &H85}, 0, 2)
			' Header bytes for DeflateStream settings
			codec = New DeflateWithChecksum(stream.BaseStream, CompressionMode.Compress, True)
			sw = New BinaryWriter(codec)
		End If
        For Each obj In array
            writer(sw, obj)
        Next
        If compress Then
			codec.Close()
			' This is important - otherwise bytes can be incorrect
			Dim checksum = codec.Checksum
			Dim bytes As Byte() = {CByte((checksum >> 24) And &Hff), CByte((checksum >> 16) And &Hff), CByte((checksum >> 8) And &Hff), CByte(checksum And &Hff)}
			stream.Write(bytes)
		End If

		' Now we can write the compressed data length, since we know the size
		If compress Then
			Dim dataEnd = stream.BaseStream.Position
			stream.BaseStream.Position = compressLengthPos
			stream.Write(CInt(dataEnd - dataStart))
			stream.BaseStream.Position = dataEnd
		End If
	End Sub

	Private Sub WriteProperty(obj As Object, id As Integer)
		If obj Is Nothing Then
			Return
		End If
        Dim writerInfo As WriterInfo = Nothing
        If Not writePropertyActions.TryGetValue(obj.[GetType](), writerInfo) Then
			Throw New FbxException(nodePath, id, "Invalid property type " & Convert.ToString(obj.[GetType]()))
		End If
		stream.Write(CByte(AscW(writerInfo.id)))
		' ReSharper disable once AssignNullToNotNullAttribute
		If writerInfo.writer Is Nothing Then
			' Array type
			Dim elementType = obj.[GetType]().GetElementType()
			WriteArray(DirectCast(obj, Array), elementType, writePropertyActions(elementType).writer)
		Else
			writerInfo.writer(stream, obj)
		End If
	End Sub

	' Data for a null node
	Shared ReadOnly nullData As Byte() = New Byte(12) {}

	' Writes a single document to the buffer
	Private Sub WriteNode(node As FbxNode)
		If node Is Nothing Then
			stream.BaseStream.Write(nullData, 0, nullData.Length)
		Else
			nodePath.Push(If(node.Name, ""))
			Dim name = If(String.IsNullOrEmpty(node.Name), Nothing, Encoding.ASCII.GetBytes(node.Name))
			If name IsNot Nothing AndAlso name.Length > Byte.MaxValue Then
				Throw New FbxException(stream.BaseStream.Position, "Node name is too long")
			End If

			' Header
			Dim endOffsetPos = stream.BaseStream.Position
			stream.Write(0)
			' End offset placeholder
			stream.Write(node.Properties.Count)
			Dim propertyLengthPos = stream.BaseStream.Position
			stream.Write(0)
			' Property length placeholder
			stream.Write(CByte(If(name Is Nothing, 0, name.Length)))
			If name IsNot Nothing Then
				stream.Write(name)
			End If

			' Write properties and length
			Dim propertyBegin = stream.BaseStream.Position
			For i As Integer = 0 To node.Properties.Count - 1
				WriteProperty(node.Properties(i), i)
			Next
			Dim propertyEnd = stream.BaseStream.Position
			stream.BaseStream.Position = propertyLengthPos
			stream.Write(CInt(propertyEnd - propertyBegin))
			stream.BaseStream.Position = propertyEnd

			' Write child nodes
			If node.Nodes.Count > 0 Then
                For Each n As FbxNode In node.Nodes
                    If n Is Nothing Then
                        Continue For
                    End If
                    WriteNode(n)
                Next
                WriteNode(Nothing)
			End If

			' Write end offset
			Dim dataEnd = stream.BaseStream.Position
			stream.BaseStream.Position = endOffsetPos
			stream.Write(CInt(dataEnd))
			stream.BaseStream.Position = dataEnd

			nodePath.Pop()
		End If
	End Sub

	''' <summary>
	''' Writes an FBX file to the output
	''' </summary>
	''' <param name="document"></param>
	Public Sub Write(document As FbxDocument)
		stream.BaseStream.Position = 0
		WriteHeader(stream.BaseStream)
		stream.Write(CInt(document.Version))
		' TODO: Do we write a top level node or not? Maybe check the version?
		nodePath.Clear()
        For Each node As FbxNode In document.Nodes
            WriteNode(node)
        Next
        WriteNode(Nothing)
		stream.Write(GenerateFooterCode(document))
		WriteFooter(stream, CInt(document.Version))
		output.Write(memory.GetBuffer(), 0, CInt(memory.Position))
	End Sub
End Class
