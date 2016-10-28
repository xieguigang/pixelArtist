Imports System
Imports System.Collections.Generic
Imports System.IO

' Writer.java
' *
' * Class that contains a string list to be written in a log file.
' *
' * @author: James M. Bayon-on
' * @version: 1.3
' 


Public Class Writer
    Private list As List(Of String)

    '	 Instantiates the writer class.
    '	 *
    '	 
    Public Sub New()
        list = New List(Of String)()
    End Sub

    '	 Accepts a string to add to the string list in the writer class.
    '	 *
    '	 * @param: a line string to write into the log
    '	 
    Public Overridable Sub add(ByVal line As String)
        list.Add(line)
    End Sub

    '	 Accepts a Honey and converts the content solution into strings then adds it to the string list.
    '	 *
    '	 * @param: a Honey to write into the log
    '	 
    Public Overridable Sub add(ByVal h As Honey)
        Dim n As Integer = h.MaxLength
        Dim board As String()() = MAT(Of String)(n, n)

        clearBoard(board, n)

        For x As Integer = 0 To n - 1
            board(x)(h.getNectar(x)) = "Q"
        Next

        printBoard(board, n)
    End Sub

    '	 Clears a 2D string board with empty string.
    '	 *
    '	 * @param: a 2D string board
    '	 * @param: length of n
    '	 
    Public Overridable Sub clearBoard(ByVal board As String()(), ByVal n As Integer)
        ' Clear the board.
        For x As Integer = 0 To n - 1
            For y As Integer = 0 To n - 1
                board(x)(y) = ""
            Next
        Next
    End Sub

    '	 Replaces the position of the queens with Q in the string board and a dot for indexes with no queens.
    '	 *
    '	 * @param: a 2D string board
    '	 * @param: length of n
    '	 
    Public Overridable Sub printBoard(ByVal board As String()(), ByVal n As Integer)
        ' Display the board.
        For y As Integer = 0 To n - 1
            Dim temp As String = ""
            For x As Integer = 0 To n - 1
                If board(x)(y) = "Q" Then
                    temp &= "Q "
                Else
                    temp &= ". "
                End If
            Next
            list.Add(temp)
        Next
    End Sub

    '	 Writes the string list into a log file.
    '	 *
    '	 * @param: a string filename
    '	 
    Public Overridable Sub writeFile(ByVal filename As String)
        Try
            Dim fw As New FileStream(filename, FileMode.OpenOrCreate)
            Dim bw As New StreamWriter(fw)

            For i As Integer = 0 To list.Count - 1
                bw.write(list(i))
                bw.WriteLine()
                bw.flush()
            Next

            bw.close()
        Catch e As IOException
            Console.WriteLine("Writing failed")
        End Try

    End Sub
End Class