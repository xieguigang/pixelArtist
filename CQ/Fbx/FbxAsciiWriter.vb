Imports System.Text
Imports System.IO
Imports System.Collections.Generic

''' <summary>
''' Writes an FBX document in a text format
''' </summary>
Public Class FbxAsciiWriter

    Private ReadOnly stream As Stream

    ''' <summary>
    ''' Creates a new reader
    ''' </summary>
    ''' <param name="stream"></param>
    Public Sub New(stream As Stream)
        If stream Is Nothing Then
            Throw New ArgumentNullException(NameOf(stream))
        End If
        Me.stream = stream
    End Sub

    ''' <summary>
    ''' The maximum line length in characters when outputting arrays
    ''' </summary>
    ''' <remarks>
    ''' Lines might end up being a few characters longer than this, visibly and otherwise,
    ''' so don't rely on it as a hard limit in code!
    ''' </remarks>
    Public ReadOnly Property MaxLineLength As Integer = 260

    ReadOnly nodePath As New Stack(Of String)()

    ' Adds the given node text to the string
    Private Sub BuildString(node As FbxNode, sb As StringBuilder, writeArrayLength As Boolean, Optional indentLevel As Integer = 0)
        nodePath.Push(If(node.Name, ""))
        Dim lineStart As Integer = sb.Length
        ' Write identifier
        For i As Integer = 0 To indentLevel - 1
            sb.Append(ControlChars.Tab)
        Next
        sb.Append(node.Name).Append(":"c)

        ' Write properties
        Dim first = True
        For j As Integer = 0 To node.Properties.Count - 1
            Dim p = node.Properties(j)
            If p Is Nothing Then
                Continue For
            End If
            If Not first Then
                sb.Append(","c)
            End If
            sb.Append(" "c)
            If TypeOf p Is String Then
                sb.Append(""""c).Append(p).Append(""""c)
            ElseIf TypeOf p Is Array Then
                Dim array = DirectCast(p, Array)
                Dim elementType = p.[GetType]().GetElementType()
                ' ReSharper disable once PossibleNullReferenceException
                ' We know it's an array, so we don't need to check for null
                If array.Rank <> 1 OrElse Not elementType.IsPrimitive Then
                    Throw New FbxException(nodePath, j, "Invalid array type " & Convert.ToString(p.[GetType]()))
                End If
                If writeArrayLength Then
                    sb.Append("*"c).Append(array.Length).Append(" {" & vbLf)
                    lineStart = sb.Length
                    For i As Integer = -1 To indentLevel - 1
                        sb.Append(ControlChars.Tab)
                    Next
                    sb.Append("a: ")
                End If
                Dim pFirst As Boolean = True
                For Each v In DirectCast(p, Array)
                    If Not pFirst Then
                        sb.Append(","c)
                    End If
                    Dim vstr = v.ToString()
                    If (sb.Length - lineStart) + vstr.Length >= MaxLineLength Then
                        sb.Append(ControlChars.Lf)
                        lineStart = sb.Length
                    End If
                    sb.Append(vstr)
                    pFirst = False
                Next
                If writeArrayLength Then
                    sb.Append(ControlChars.Lf)
                    For i As Integer = 0 To indentLevel - 1
                        sb.Append(ControlChars.Tab)
                    Next
                    sb.Append("}"c)
                End If
            ElseIf TypeOf p Is Char Then
                sb.Append(CChar(p))
            ElseIf p.[GetType]().IsPrimitive AndAlso TypeOf p Is IFormattable Then
                sb.Append(p)
            Else
                Throw New FbxException(nodePath, j, "Invalid property type " & Convert.ToString(p.[GetType]()))
            End If
            first = False
        Next

        ' Write child nodes
        If node.Nodes.Count > 0 Then
            sb.Append(" {" & vbLf)
            For Each n As FbxNode In node.Nodes
                If n Is Nothing Then
                    Continue For
                End If
                BuildString(n, sb, writeArrayLength, indentLevel + 1)
            Next
            For i As Integer = 0 To indentLevel - 1
                sb.Append(ControlChars.Tab)
            Next
            sb.Append("}"c)
        End If
        sb.Append(ControlChars.Lf)

        nodePath.Pop()
    End Sub

    ''' <summary>
    ''' Writes an FBX document to the stream
    ''' </summary>
    ''' <param name="document"></param>
    ''' <remarks>
    ''' ASCII FBX files have no header or footer, so you can call this multiple times
    ''' </remarks>
    Public Sub Write(document As FbxDocument)
        If document Is Nothing Then
            Throw New ArgumentNullException(NameOf(document))
        End If
        Dim sb = New StringBuilder()

        ' Write version header (a comment, but required for many importers)
        Dim vMajor = CInt(document.Version) \ 1000
        Dim vMinor = (CInt(document.Version) Mod 1000) \ 100
        Dim vRev = (CInt(document.Version) Mod 100) \ 10
        sb.Append("; FBX {vMajor}.{vMinor}.{vRev} project file" & vbLf & vbLf)

        nodePath.Clear()
        For Each n As FbxNode In document.Nodes
            If n Is Nothing Then
                Continue For
            End If
            BuildString(n, sb, document.Version >= FbxVersion.v7_1)
            sb.Append(ControlChars.Lf)
        Next
        Dim b = Encoding.ASCII.GetBytes(sb.ToString())
        stream.Write(b, 0, b.Length)
    End Sub
End Class
