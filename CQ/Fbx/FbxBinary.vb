Imports System.Collections.Generic
Imports System.IO
Imports System.Text

''' <summary>
''' Base class for binary stream wrappers
''' </summary>
Public MustInherit Class FbxBinary

    ' Header string, found at the top of all compliant files
    Private Shared ReadOnly headerString As Byte() = Encoding.ASCII.GetBytes("Kaydara FBX Binary  " & vbNullChar & ChrW(26) & vbNullChar)

    ' This data was entirely calculated by me, honest. Turns out it works, fancy that!
    Private Shared ReadOnly sourceId As Byte() = {&H58, &HAB, &HA9, &HF0, &H6C, &HA2,
        &HD8, &H3F, &H4D, &H47, &H49, &HA3,
        &HB4, &HB2, &HE7, &H3D}
    Private Shared ReadOnly key As Byte() = {&HE2, &H4F, &H7B, &H5F, &HCD, &HE4,
        &HC8, &H6D, &HDB, &HD8, &HFB, &HD7,
        &H40, &H58, &HC6, &H78}
    ' This wasn't - it just appears at the end of every compliant file
    Private Shared ReadOnly extension As Byte() = {&HF8, &H5A, &H8C, &H6A, &HDE, &HF5,
        &HD9, &H7E, &HEC, &HE9, &HC, &HE3,
        &H75, &H8F, &H29, &HB}

    ' Number of null bytes between the footer code and the version
    Private Const footerZeroes1 As Integer = 20
    ' Number of null bytes between the footer version and extension code
    Private Const footerZeroes2 As Integer = 120

    ''' <summary>
    ''' The size of the footer code
    ''' </summary>
    Protected Const footerCodeSize As Integer = 16

    ''' <summary>
    ''' The namespace separator in the binary format (remember to reverse the identifiers)
    ''' </summary>
    Protected Const binarySeparator As String = vbNullChar & ChrW(1)

    ''' <summary>
    ''' The namespace separator in the ASCII format and in object data
    ''' </summary>
    Protected Const asciiSeparator As String = "::"

    ''' <summary>
    ''' Checks if the first part of 'data' matches 'original'
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="original"></param>
    ''' <returns><c>true</c> if it does, otherwise <c>false</c></returns>
    Protected Shared Function CheckEqual(data As Byte(), original As Byte()) As Boolean
        For i As Integer = 0 To original.Length - 1
            If data(i) <> original(i) Then
                Return False
            End If
        Next
        Return True
    End Function

    ''' <summary>
    ''' Writes the FBX header string
    ''' </summary>
    ''' <param name="stream"></param>
    Protected Shared Sub WriteHeader(stream As Stream)
        stream.Write(headerString, 0, headerString.Length)
    End Sub

    ''' <summary>
    ''' Reads the FBX header string
    ''' </summary>
    ''' <param name="stream"></param>
    ''' <returns><c>true</c> if it's compliant</returns>
    Protected Shared Function ReadHeader(stream As Stream) As Boolean
        Dim buf = New Byte(headerString.Length - 1) {}
        stream.Read(buf, 0, buf.Length)
        Return CheckEqual(buf, headerString)
    End Function

    ' Turns out this is the algorithm they use to generate the footer. Who knew!
    Private Shared Sub Encrypt(a As Byte(), b As Byte())
        Dim c As Byte = 64
        For i As Integer = 0 To footerCodeSize - 1
            a(i) = CByte(a(i) Xor CByte(c Xor b(i)))
            c = a(i)
        Next
    End Sub

    Const timePath1 As String = "FBXHeaderExtension"
    Const timePath2 As String = "CreationTimeStamp"
    Shared ReadOnly timePath As New Stack(Of String)({timePath1, timePath2})

    ' Gets a single timestamp component
    Private Shared Function GetTimestampVar(timestamp As FbxNode, element As String) As Integer
        Dim elementNode = timestamp(element)
        If elementNode IsNot Nothing AndAlso elementNode.Properties.Count > 0 Then
            Dim prop = elementNode.Properties(0)
            If TypeOf prop Is Integer OrElse TypeOf prop Is Long Then
                Return CInt(prop)
            End If
        End If
        Throw New FbxException(timePath, -1, "Timestamp has no " & element)
    End Function

    ''' <summary>
    ''' Generates the unique footer code based on the document's timestamp
    ''' </summary>
    ''' <param name="document"></param>
    ''' <returns>A 16-byte code</returns>
    Protected Shared Function GenerateFooterCode(document As FbxNodeList) As Byte()
        Dim timestamp = document.GetRelative(timePath1 & "/" & timePath2)
        If timestamp Is Nothing Then
            Throw New FbxException(timePath, -1, "No creation timestamp")
        End If
        Try
            Return GenerateFooterCode(GetTimestampVar(timestamp, "Year"), GetTimestampVar(timestamp, "Month"), GetTimestampVar(timestamp, "Day"), GetTimestampVar(timestamp, "Hour"), GetTimestampVar(timestamp, "Minute"), GetTimestampVar(timestamp, "Second"),
                GetTimestampVar(timestamp, "Millisecond"))
        Catch generatedExceptionName As ArgumentOutOfRangeException
            Throw New FbxException(timePath, -1, "Invalid timestamp")
        End Try
    End Function

    ''' <summary>
    ''' Generates a unique footer code based on a timestamp
    ''' </summary>
    ''' <param name="year"></param>
    ''' <param name="month"></param>
    ''' <param name="day"></param>
    ''' <param name="hour"></param>
    ''' <param name="minute"></param>
    ''' <param name="second"></param>
    ''' <param name="millisecond"></param>
    ''' <returns>A 16-byte code</returns>
    Protected Shared Function GenerateFooterCode(year As Integer, month As Integer, day As Integer, hour As Integer, minute As Integer, second As Integer,
        millisecond As Integer) As Byte()
        If year < 0 OrElse year > 9999 Then
            Throw New ArgumentOutOfRangeException(NameOf(year))
        End If
        If month < 0 OrElse month > 12 Then
            Throw New ArgumentOutOfRangeException(NameOf(month))
        End If
        If day < 0 OrElse day > 31 Then
            Throw New ArgumentOutOfRangeException(NameOf(day))
        End If
        If hour < 0 OrElse hour >= 24 Then
            Throw New ArgumentOutOfRangeException(NameOf(hour))
        End If
        If minute < 0 OrElse minute >= 60 Then
            Throw New ArgumentOutOfRangeException(NameOf(minute))
        End If
        If second < 0 OrElse second >= 60 Then
            Throw New ArgumentOutOfRangeException(NameOf(second))
        End If
        If millisecond < 0 OrElse millisecond >= 1000 Then
            Throw New ArgumentOutOfRangeException(NameOf(millisecond))
        End If

        Dim str = DirectCast(sourceId.Clone(), Byte())
        Dim mangledTime = "{second:00}{month:00}{hour:00}{day:00}{(millisecond/10):00}{year:0000}{minute:00}"
        Dim mangledBytes = Encoding.ASCII.GetBytes(mangledTime)
        Encrypt(str, mangledBytes)
        Encrypt(str, key)
        Encrypt(str, mangledBytes)
        Return str
    End Function

    ''' <summary>
    ''' Writes the FBX footer extension (NB - not the unique footer code)
    ''' </summary>
    ''' <param name="stream"></param>
    ''' <param name="version"></param>
    Protected Sub WriteFooter(stream As BinaryWriter, version As Integer)
        Dim zeroes = New Byte(Math.Max(footerZeroes1, footerZeroes2) - 1) {}
        stream.Write(zeroes, 0, footerZeroes1)
        stream.Write(version)
        stream.Write(zeroes, 0, footerZeroes2)
        stream.Write(extension, 0, extension.Length)
    End Sub

    Private Shared Function AllZero(array As Byte()) As Boolean
        For Each b As Byte In array
            If b <> 0 Then
                Return False
            End If
        Next
        Return True
    End Function

    ''' <summary>
    ''' Reads and checks the FBX footer extension (NB - not the unique footer code)
    ''' </summary>
    ''' <param name="stream"></param>
    ''' <param name="version"></param>
    ''' <returns><c>true</c> if it's compliant</returns>
    Protected Function CheckFooter(stream As BinaryReader, version As FbxVersion) As Boolean
        Dim buffer = New Byte(Math.Max(footerZeroes1, footerZeroes2) - 1) {}
        stream.Read(buffer, 0, footerZeroes1)
        Dim correct As Boolean = AllZero(buffer)
        Dim readVersion = stream.ReadInt32()
        correct = correct And (readVersion = CInt(version))
        stream.Read(buffer, 0, footerZeroes2)
        correct = correct And AllZero(buffer)
        stream.Read(buffer, 0, extension.Length)
        correct = correct And CheckEqual(buffer, extension)
        Return correct
    End Function
End Class
