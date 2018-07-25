Imports System.IO
Imports System.IO.Compression

''' <summary>
''' A wrapper for DeflateStream that calculates the Adler32 checksum of the payload
''' </summary>
Public Class DeflateWithChecksum
    Inherits DeflateStream

    Const modAdler As Integer = 65521

    Dim checksumA As UInteger
    Dim checksumB As UInteger

    ''' <summary>
    ''' Gets the Adler32 checksum at the current point in the stream
    ''' </summary>
    Public ReadOnly Property Checksum() As Integer
        Get
            checksumA = checksumA Mod modAdler
            checksumB = checksumB Mod modAdler
            Return CInt((checksumB << 16) Or checksumA)
        End Get
    End Property

    ''' <inheritdoc />
    Public Sub New(stream As Stream, mode As CompressionMode)
        MyBase.New(stream, mode)
        ResetChecksum()
    End Sub

    ''' <inheritdoc />
    Public Sub New(stream As Stream, mode As CompressionMode, leaveOpen As Boolean)
        MyBase.New(stream, mode, leaveOpen)
        ResetChecksum()
    End Sub

    ' Efficiently extends the checksum with the given buffer
    Private Sub CalcChecksum(array As Byte(), offset As Integer, count As Integer)
        checksumA = checksumA Mod modAdler
        checksumB = checksumB Mod modAdler
        Dim i As Integer = offset, c As Integer = 0
        While i < (offset + count)
            checksumA += array(i)
            checksumB += checksumA
            If c > 4000 Then
                ' This is about how many iterations it takes for B to reach IntMax
                checksumA = checksumA Mod modAdler
                checksumB = checksumB Mod modAdler
                c = 0
            End If
            i += 1
            c += 1
        End While
    End Sub

    ''' <inheritdoc />
    Public Overrides Sub Write(array As Byte(), offset As Integer, count As Integer)
        MyBase.Write(array, offset, count)
        CalcChecksum(array, offset, count)
    End Sub

    ''' <inheritdoc />
    Public Overrides Function Read(array As Byte(), offset As Integer, count As Integer) As Integer
        Dim ret = MyBase.Read(array, offset, count)
        CalcChecksum(array, offset, count)
        Return ret
    End Function

    ''' <summary>
    ''' Initializes the checksum values
    ''' </summary>
    Public Sub ResetChecksum()
        checksumA = 1
        checksumB = 0
    End Sub
End Class
