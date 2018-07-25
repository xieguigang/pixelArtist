Imports System.Text
Imports System.IO
Imports System.IO.Compression

''' <summary>
''' Reads FBX nodes from a binary stream
''' </summary>
Public Class FbxBinaryReader
    Inherits FbxBinary

    Private ReadOnly stream As BinaryReader
	Private ReadOnly errorLevel As ErrorLevel

	Private Delegate Function ReadPrimitive(reader As BinaryReader) As Object

    ''' <summary>
    ''' Creates a new reader
    ''' </summary>
    ''' <param name="stream">The stream to read from</param>
    ''' <param name="errorLevel">When to throw an <see cref="FbxException"/></param>
    ''' <exception cref="ArgumentException"><paramref name="stream"/> does
    ''' not support seeking</exception>
    Public Sub New(stream As Stream, Optional errorLevel As ErrorLevel = ErrorLevel.Checked)
        If stream Is Nothing Then
            Throw New ArgumentNullException(NameOf(stream))
        End If
        If Not stream.CanSeek Then
            Throw New ArgumentException("The stream must support seeking. Try reading the data into a buffer first")
        End If
        Me.stream = New BinaryReader(stream, Encoding.ASCII)
        Me.errorLevel = errorLevel
    End Sub

	' Reads a single property
	Private Function ReadProperty() As Object
		Dim dataType = ChrW(stream.ReadByte())
		Select Case dataType
			Case "Y"C
				Return stream.ReadInt16()
			Case "C"C
				Return ChrW(stream.ReadByte())
			Case "I"C
				Return stream.ReadInt32()
			Case "F"C
				Return stream.ReadSingle()
			Case "D"C
				Return stream.ReadDouble()
			Case "L"C
				Return stream.ReadInt64()
			Case "f"C
				Return ReadArray(Function(br) br.ReadSingle(), GetType(Single))
			Case "d"C
				Return ReadArray(Function(br) br.ReadDouble(), GetType(Double))
			Case "l"C
				Return ReadArray(Function(br) br.ReadInt64(), GetType(Long))
			Case "i"C
				Return ReadArray(Function(br) br.ReadInt32(), GetType(Integer))
			Case "b"C
				Return ReadArray(Function(br) br.ReadBoolean(), GetType(Boolean))
			Case "S"C
				Dim len = stream.ReadInt32()
				Dim str = If(len = 0, "", Encoding.ASCII.GetString(stream.ReadBytes(len)))
				' Convert \0\1 to '::' and reverse the tokens
				If str.Contains(binarySeparator) Then
                    Dim tokens = str.Split({binarySeparator}, StringSplitOptions.None)
                    Dim sb = New StringBuilder()
					Dim first As Boolean = True
					For i As Integer = tokens.Length - 1 To 0 Step -1
						If Not first Then
							sb.Append(asciiSeparator)
						End If
						sb.Append(tokens(i))
						first = False
					Next
					str = sb.ToString()
				End If
				Return str
			Case "R"C
				Return stream.ReadBytes(stream.ReadInt32())
			Case Else
				Throw New FbxException(stream.BaseStream.Position - 1, "Invalid property data type `" & dataType & "'")
		End Select
	End Function

	' Reads an array, decompressing it if required
	Private Function ReadArray(readPrimitive As ReadPrimitive, arrayType As Type) As Array
		Dim len = stream.ReadInt32()
		Dim encoding = stream.ReadInt32()
		Dim compressedLen = stream.ReadInt32()
		Dim ret = Array.CreateInstance(arrayType, len)
		Dim s = stream
		Dim endPos = stream.BaseStream.Position + compressedLen
		If encoding <> 0 Then
			If errorLevel >= ErrorLevel.Checked Then
				If encoding <> 1 Then
					Throw New FbxException(stream.BaseStream.Position - 1, "Invalid compression encoding (must be 0 or 1)")
				End If
				Dim cmf = stream.ReadByte()
				If (cmf And &Hf) <> 8 OrElse (cmf >> 4) > 7 Then
					Throw New FbxException(stream.BaseStream.Position - 1, "Invalid compression format " & cmf)
				End If
				Dim flg = stream.ReadByte()
				If errorLevel >= ErrorLevel.[Strict] AndAlso ((cmf << 8) + flg) Mod 31 <> 0 Then
					Throw New FbxException(stream.BaseStream.Position - 1, "Invalid compression FCHECK")
				End If
				If (flg And (1 << 5)) <> 0 Then
					Throw New FbxException(stream.BaseStream.Position - 1, "Invalid compression flags; dictionary not supported")
				End If
			Else
				stream.BaseStream.Position += 2
			End If
			Dim codec = New DeflateWithChecksum(stream.BaseStream, CompressionMode.Decompress)
			s = New BinaryReader(codec)
		End If
		Try
			For i As Integer = 0 To len - 1
				ret.SetValue(readPrimitive(s), i)
			Next
		Catch generatedExceptionName As InvalidDataException
			Throw New FbxException(stream.BaseStream.Position - 1, "Compressed data was malformed")
		End Try
		If encoding <> 0 Then
			If errorLevel >= ErrorLevel.Checked Then
				stream.BaseStream.Position = endPos - 4
				Dim checksumBytes = New Byte(4 - 1) {}
				stream.BaseStream.Read(checksumBytes, 0, checksumBytes.Length)
				Dim checksum As Integer = 0
				For i As Integer = 0 To checksumBytes.Length - 1
					checksum = (checksum << 8) + checksumBytes(i)
				Next
				If checksum <> DirectCast(s.BaseStream, DeflateWithChecksum).Checksum Then
					Throw New FbxException(stream.BaseStream.Position, "Compressed data has invalid checksum")
				End If
			Else
				stream.BaseStream.Position = endPos
			End If
		End If
		Return ret
	End Function

	''' <summary>
	''' Reads a single node.
	''' </summary>
	''' <remarks>
	''' This won't read the file header or footer, and as such will fail if the stream is a full FBX file
	''' </remarks>
	''' <returns>The node</returns>
	''' <exception cref="FbxException">The FBX data was malformed
	''' for the reader's error level</exception>
	Public Function ReadNode() As FbxNode
		Dim endOffset = stream.ReadInt32()
		Dim numProperties = stream.ReadInt32()
		Dim propertyListLen = stream.ReadInt32()
		Dim nameLen = stream.ReadByte()
		Dim name = If(nameLen = 0, "", Encoding.ASCII.GetString(stream.ReadBytes(nameLen)))

		If endOffset = 0 Then
			' The end offset should only be 0 in a null node
			If errorLevel >= ErrorLevel.Checked AndAlso (numProperties <> 0 OrElse propertyListLen <> 0 OrElse Not String.IsNullOrEmpty(name)) Then
				Throw New FbxException(stream.BaseStream.Position, "Invalid node; expected NULL record")
			End If
			Return Nothing
		End If

        Dim node = New FbxNode() With {
             .Name = name
        }

        Dim propertyEnd = stream.BaseStream.Position + propertyListLen
		' Read properties
		For i As Integer = 0 To numProperties - 1
			node.Properties.Add(ReadProperty())
		Next

		If errorLevel >= ErrorLevel.Checked AndAlso stream.BaseStream.Position <> propertyEnd Then
			Throw New FbxException(stream.BaseStream.Position, "Too many bytes in property list, end point is " & propertyEnd)
		End If

		' Read nested nodes
		Dim listLen = endOffset - stream.BaseStream.Position
		If errorLevel >= ErrorLevel.Checked AndAlso listLen < 0 Then
			Throw New FbxException(stream.BaseStream.Position, "Node has invalid end point")
		End If
		If listLen > 0 Then
			Dim nested As FbxNode
			Do
				nested = ReadNode()
				node.Nodes.Add(nested)
			Loop While nested IsNot Nothing
			If errorLevel >= ErrorLevel.Checked AndAlso stream.BaseStream.Position <> endOffset Then
				Throw New FbxException(stream.BaseStream.Position, "Too many bytes in node, end point is " & endOffset)
			End If
		End If
		Return node
	End Function

	''' <summary>
	''' Reads an FBX document from the stream
	''' </summary>
	''' <returns>The top-level node</returns>
	''' <exception cref="FbxException">The FBX data was malformed
	''' for the reader's error level</exception>
	Public Function Read() As FbxDocument
		' Read header
		Dim validHeader As Boolean = ReadHeader(stream.BaseStream)
		If errorLevel >= ErrorLevel.[Strict] AndAlso Not validHeader Then
			Throw New FbxException(stream.BaseStream.Position, "Invalid header string")
		End If
        Dim document = New FbxDocument() With {
             .Version = CType(stream.ReadInt32(), FbxVersion)
        }

        ' Read nodes
        Dim dataPos = stream.BaseStream.Position
		Dim nested As FbxNode
		Do
			nested = ReadNode()
			If nested IsNot Nothing Then
				document.Nodes.Add(nested)
			End If
		Loop While nested IsNot Nothing

		' Read footer code
		Dim footerCode = New Byte(footerCodeSize - 1) {}
		stream.BaseStream.Read(footerCode, 0, footerCode.Length)
		If errorLevel >= ErrorLevel.[Strict] Then
			Dim validCode = GenerateFooterCode(document)
			If Not CheckEqual(footerCode, validCode) Then
				Throw New FbxException(stream.BaseStream.Position - footerCodeSize, "Incorrect footer code")
			End If
		End If

		' Read footer extension
		dataPos = stream.BaseStream.Position
		Dim validFooterExtension = CheckFooter(stream, document.Version)
		If errorLevel >= ErrorLevel.[Strict] AndAlso Not validFooterExtension Then
			Throw New FbxException(dataPos, "Invalid footer")
		End If
		Return document
	End Function
End Class
