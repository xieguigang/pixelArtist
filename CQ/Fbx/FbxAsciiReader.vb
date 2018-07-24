Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language

''' <summary>
''' Reads FBX nodes from a text stream
''' </summary>
Public Class FbxAsciiReader
    Private ReadOnly stream As Stream
    Private ReadOnly errorLevel As ErrorLevel

    Private line As Integer = 1
    Private column As Integer = 1

    ''' <summary>
    ''' Creates a new reader
    ''' </summary>
    ''' <param name="stream"></param>
    ''' <param name="errorLevel"></param>
    Public Sub New(stream As Stream, Optional errorLevel As ErrorLevel = ErrorLevel.Checked)
        If stream Is Nothing Then
            Throw New ArgumentNullException(NameOf(stream))
        End If
        Me.stream = stream
        Me.errorLevel = errorLevel
    End Sub

    ''' <summary>
    ''' The maximum array size that will be allocated
    ''' </summary>
    ''' <remarks>
    ''' If you trust the source, you can expand this value as necessary.
    ''' Malformed files could cause large amounts of memory to be allocated
    ''' and slow or crash the system as a result.
    ''' </remarks>
    Public MaxArrayLength As Integer = (1 << 24)

    ' We read bytes a lot, so we should make a more efficient method here
    ' (The normal one makes a new byte array each time)

    ReadOnly singleChar As Byte() = New Byte(0) {}
    Private prevChar As System.Nullable(Of Char)
    Private endStream As Boolean
    Private wasCr As Boolean

    ' Reads a char, allows peeking and checks for end of stream
    Private Function ReadChar() As Char
        If prevChar IsNot Nothing Then
            Dim c = prevChar.Value
            prevChar = Nothing
            Return c
        End If
        If stream.Read(singleChar, 0, 1) < 1 Then
            endStream = True
            Return ControlChars.NullChar
        End If
        Dim ch = ChrW(singleChar(0))
        ' Handle line and column numbers here;
        ' This isn't terribly accurate, but good enough for diagnostics
        If ch = ControlChars.Cr Then
            wasCr = True
            line += 1
            column = 0
        Else
            If ch = ControlChars.Lf AndAlso Not wasCr Then
                line += 1
                column = 0
            End If
            wasCr = False
        End If
        column += 1
        Return ch
    End Function

    ' Checks if a character is valid in a real number
    Private Shared Function IsDigit(c As Char, first As Boolean) As Boolean
        If Char.IsDigit(c) Then
            Return True
        End If
        Select Case c
            Case "-"c, "+"c
                Return True
            Case "."c, "e"c, "E"c, "X"c, "x"c
                Return Not first
        End Select
        Return False
    End Function

    Private Shared Function IsLineEnd(c As Char) As Boolean
        Return c = ControlChars.Cr OrElse c = ControlChars.Lf
    End Function

    ' Token to mark the end of the stream
    Private Class EndOfStream
        Public Overrides Function ToString() As String
            Return "end of stream"
        End Function
    End Class

    ' Wrapper around a string to mark it as an identifier
    ' (as opposed to a string literal)
    Private Class Identifier
        Public ReadOnly [String] As String

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim id = TryCast(obj, Identifier)
            If id IsNot Nothing Then
                Return [String] = id.[String]
            End If
            Return False
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return If(([String] Is Nothing), 0, [String].GetHashCode())
        End Function

        Public Sub New(str As String)
            [String] = str
        End Sub

        Public Overrides Function ToString() As String
            Return [String] & ":"
        End Function
    End Class

    Private prevTokenSingle As Object

    ' Reads a single token, allows peeking
    ' Can return 'null' for a comment or whitespace
    Private Function ReadTokenSingle() As Object
        If prevTokenSingle IsNot Nothing Then
            Dim ret = prevTokenSingle
            prevTokenSingle = Nothing
            Return ret
        End If
        Dim c = ReadChar()
        If endStream Then
            Return New EndOfStream()
        End If
        Select Case c
            Case ";"c
                ' Comments
                While Not IsLineEnd(ReadChar()) AndAlso Not endStream
                End While
                ' Skip a line
                Return Nothing
            ' Operators
            Case "{"c, "}"c, "*"c, ":"c, ","c
                Return c
            Case """"c
                ' String literal
                Dim sb1 = New StringBuilder()
                Dim ci As Value(Of Char) = """"c

                While (ci = ReadChar()) <> """"c
                    If endStream Then
                        Throw New FbxException(line, column, "Unexpected end of stream; expecting end quote")
                    End If
                    sb1.Append(ci)
                End While
                Return sb1.ToString()
            Case Else
                If Char.IsWhiteSpace(c) Then
                    ' Merge whitespace
                    Dim ci As Value(Of Char) = """"c

                    While Char.IsWhiteSpace(ci = ReadChar()) AndAlso Not endStream
                    End While
                    If Not endStream Then
                        prevChar = ci
                    End If
                    Return Nothing
                End If
                If IsDigit(c, True) Then
                    ' Number
                    Dim sb2 = New StringBuilder()
                    Do
                        sb2.Append(c)
                        c = ReadChar()
                    Loop While IsDigit(c, False) AndAlso Not endStream
                    If Not endStream Then
                        prevChar = c
                    End If
                    Dim str = sb2.ToString()
                    If str.Contains(".") Then
                        If str.Split("."c, "e"c, "E"c)(1).Length > 6 Then
                            Dim d As Double
                            If Not Double.TryParse(str, d) Then
                                Throw New FbxException(line, column, "Invalid number")
                            End If
                            Return d
                        Else
                            Dim f As Single
                            If Not Single.TryParse(str, f) Then
                                Throw New FbxException(line, column, "Invalid number")
                            End If
                            Return f
                        End If
                    End If
                    Dim l As Long
                    If Not Long.TryParse(str, l) Then
                        Throw New FbxException(line, column, "Invalid integer")
                    End If
                    ' Check size and return the smallest possible
                    If l >= Byte.MinValue AndAlso l <= Byte.MaxValue Then
                        Return CByte(l)
                    End If
                    If l >= Integer.MinValue AndAlso l <= Integer.MaxValue Then
                        Return CInt(l)
                    End If
                    Return l
                End If
                If Char.IsLetter(c) OrElse c = "_"c Then
                    ' Identifier
                    Dim sb3 = New StringBuilder()
                    Do
                        sb3.Append(c)
                        c = ReadChar()
                    Loop While (Char.IsLetterOrDigit(c) OrElse c = "_"c) AndAlso Not endStream
                    If Not endStream Then
                        prevChar = c
                    End If
                    Return New Identifier(sb3.ToString())
                End If
                Exit Select
        End Select
        Throw New FbxException(line, column, "Unknown character " & c)
    End Function

    Private prevToken As Object

    ' Use a loop rather than recursion to prevent stack overflow
    ' Here we can also merge string+colon into an identifier,
    ' returning single-character bare strings (for C-type properties)
    Private Function ReadToken() As Object
        Dim ret As Object
        If prevToken IsNot Nothing Then
            ret = prevToken
            prevToken = Nothing
            Return ret
        End If
        Do
            ret = ReadTokenSingle()
        Loop While ret Is Nothing
        Dim id = TryCast(ret, Identifier)
        If id IsNot Nothing Then
            Dim colon As Object
            Do
                colon = ReadTokenSingle()
            Loop While colon Is Nothing
            If Not ":"c.Equals(colon) Then
                If id.[String].Length > 1 Then
                    Throw New FbxException(line, column, "Unexpected '" & colon & "', expected ':' or a single-char literal")
                End If
                ret = id.[String](0)
                prevTokenSingle = colon
            End If
        End If
        Return ret
    End Function

    Private Sub ExpectToken(token As Object)
        Dim t = ReadToken()
        If Not token.Equals(t) Then
            Throw New FbxException(line, column, "Unexpected '" & t & "', expected " & token)
        End If
    End Sub

    Private Enum ArrayType
        [Byte] = 0
        Int = 1
        [Long] = 2
        Float = 3
        [Double] = 4
    End Enum

    Private Function ReadArray() As Array
        ' Read array length and header
        Dim len = ReadToken()
        Dim l As Long
        If TypeOf len Is Long Then
            l = CLng(len)
        ElseIf TypeOf len Is Integer Then
            l = CInt(len)
        ElseIf TypeOf len Is Byte Then
            l = CByte(len)
        Else
            Throw New FbxException(line, column, "Unexpected '" & len & "', expected an integer")
        End If
        If l < 0 Then
            Throw New FbxException(line, column, "Invalid array length " & l)
        End If
        If l > MaxArrayLength Then
            Throw New FbxException(line, column, "Array length " & l & " higher than permitted maximum " & MaxArrayLength)
        End If
        ExpectToken("{"c)
        ExpectToken(New Identifier("a"))
        Dim array = New Double(l - 1) {}

        ' Read array elements
        Dim expectComma As Boolean = False
        Dim token As Object
        Dim tokenReader As New Value(Of Object)(Nothing)
        Dim arrayType__1 = ArrayType.[Byte]
        Dim pos As Long = 0


        While Not "}"c.Equals(tokenReader = ReadToken())
            token = tokenReader.Value

            If expectComma Then
                If Not ","c.Equals(token) Then
                    Throw New FbxException(line, column, "Unexpected '" & token & "', expected ','")
                End If
                expectComma = False
                Continue While
            End If
            If pos >= array.Length Then
                If errorLevel >= ErrorLevel.Checked Then
                    Throw New FbxException(line, column, "Too many elements in array")
                End If
                Continue While
            End If

            ' Add element to the array, checking for the maximum
            ' size of any one element.
            ' (I'm not sure if this is the 'correct' way to do it, but it's the only
            ' logical one given the nature of the ASCII format)
            Dim d As Double
            If TypeOf token Is Byte Then
                d = CByte(token)
            ElseIf TypeOf token Is Integer Then
                d = CInt(token)
                If arrayType__1 < ArrayType.Int Then
                    arrayType__1 = ArrayType.Int
                End If
            ElseIf TypeOf token Is Long Then
                d = CLng(token)
                If arrayType__1 < ArrayType.[Long] Then
                    arrayType__1 = ArrayType.[Long]
                End If
            ElseIf TypeOf token Is Single Then
                d = CSng(token)
                ' A long can't be accurately represented by a float
                arrayType__1 = If(arrayType__1 < ArrayType.[Long], ArrayType.Float, ArrayType.[Double])
            ElseIf TypeOf token Is Double Then
                d = CDbl(token)
                If arrayType__1 < ArrayType.[Double] Then
                    arrayType__1 = ArrayType.[Double]
                End If
            Else
                Throw New FbxException(line, column, "Unexpected '" & token & "', expected a number")
            End If
            array(System.Math.Max(System.Threading.Interlocked.Increment(pos), pos - 1)) = d
            expectComma = True
        End While
        If pos < array.Length AndAlso errorLevel >= ErrorLevel.Checked Then
            Throw New FbxException(line, column, "Too few elements in array - expected " + (array.Length - pos) & " more")
        End If

        ' Convert the array to the smallest type we can see
        Dim ret As Array
        Select Case arrayType__1
            Case ArrayType.[Byte]
                Dim bArray = New Byte(array.Length - 1) {}
                For i As Integer = 0 To bArray.Length - 1
                    bArray(i) = CByte(array(i))
                Next
                ret = bArray
                Exit Select
            Case ArrayType.Int
                Dim iArray = New Integer(array.Length - 1) {}
                For i As Integer = 0 To iArray.Length - 1
                    iArray(i) = CInt(array(i))
                Next
                ret = iArray
                Exit Select
            Case ArrayType.[Long]
                Dim lArray = New Long(array.Length - 1) {}
                For i As Integer = 0 To lArray.Length - 1
                    lArray(i) = CLng(array(i))
                Next
                ret = lArray
                Exit Select
            Case ArrayType.Float
                Dim fArray = New Single(array.Length - 1) {}
                For i As Integer = 0 To fArray.Length - 1
                    fArray(i) = CLng(array(i))
                Next
                ret = fArray
                Exit Select
            Case Else
                ret = array
                Exit Select
        End Select
        Return ret
    End Function

    ''' <summary>
    ''' Reads the next node from the stream
    ''' </summary>
    ''' <returns>The read node, or <c>null</c></returns>
    Public Function ReadNode() As FbxNode
        Dim first = ReadToken()
        Dim id = TryCast(first, Identifier)
        If id Is Nothing Then
            If TypeOf first Is EndOfStream Then
                Return Nothing
            End If
            Throw New FbxException(line, column, "Unexpected '" & first & "', expected an identifier")
        End If
        Dim node = New FbxNode() With {
            .Name = id.[String]
        }

        ' Read properties
        Dim token As Object = Nothing
        Dim tokenReader As New Value(Of Object)(Nothing)
        Dim expectComma As Boolean = False
        While Not "{"c.Equals(tokenReader = ReadToken()) AndAlso Not (TypeOf tokenReader.Value Is Identifier) AndAlso Not "}"c.Equals(tokenReader.Value)
            token = tokenReader.Value

            If expectComma Then
                If Not ","c.Equals(token) Then
                    Throw New FbxException(line, column, "Unexpected '" & token & "', expected a ','")
                End If
                expectComma = False
                Continue While
            End If
            If TypeOf token Is Char Then
                Dim c = CChar(token)
                Select Case c
                    Case "*"c
                        token = ReadArray()
                        Exit Select
                    Case "}"c, ":"c, ","c
                        Throw New FbxException(line, column, "Unexpected '" & c & "' in property list")
                End Select
            End If
            node.Properties.Add(token)
            ' The final comma before the open brace isn't required
            expectComma = True
        End While
        ' TODO: Merge property list into an array as necessary
        ' Now we're either at an open brace, close brace or a new node
        If TypeOf token Is Identifier OrElse "}"c.Equals(token) Then
            prevToken = token
            Return node
        End If
        ' The while loop can't end unless we're at an open brace, so we can continue right on
        Dim endBrace As New Value(Of Object)(Nothing)

        While Not "}"c.Equals(endBrace = ReadToken())
            prevToken = endBrace.Value
            ' If it's not an end brace, the next node will need it
            node.Nodes.Add(ReadNode())
        End While
        If node.Nodes.Count < 1 Then
            ' If there's an open brace, we want that to be preserved
            node.Nodes.Add(Nothing)
        End If
        Return node
    End Function

    ''' <summary>
    ''' Reads a full document from the stream
    ''' </summary>
    ''' <returns>The complete document object</returns>
    Public Function Read() As FbxDocument
        Dim ret = New FbxDocument()

        ' Read version string
        Const versionString As String = "; FBX (\d)\.(\d)\.(\d) project file"
        Dim c As Value(Of Char) = " "c
        While Char.IsWhiteSpace(c = ReadChar()) AndAlso Not endStream
        End While
        ' Skip whitespace
        Dim hasVersionString As Boolean = False

        If c.Value = ";"c Then
            Dim sb = New StringBuilder()
            Do
                sb.Append(c.Value)
            Loop While Not IsLineEnd(c = ReadChar()) AndAlso Not endStream
            Dim match = Regex.Match(sb.ToString(), versionString)
            hasVersionString = match.Success
            If hasVersionString Then
                ret.Version = DirectCast(Integer.Parse(match.Groups(1).Value) * 1000 + Integer.Parse(match.Groups(2).Value) * 100 + Integer.Parse(match.Groups(3).Value) * 10, FbxVersion)
            End If
        End If
        If Not hasVersionString AndAlso errorLevel >= ErrorLevel.[Strict] Then
            Throw New FbxException(line, column, "Invalid version string; first line must match """ & versionString & """")
        End If
        Dim node As New Value(Of FbxNode)
        While (node = ReadNode()) IsNot Nothing
            ret.Nodes.Add(node)
        End While
        Return ret
    End Function

End Class
