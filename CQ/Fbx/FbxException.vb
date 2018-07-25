Imports System.Collections.Generic

''' <summary>
''' An error with the FBX data input
''' </summary>
Public Class FbxException
    Inherits Exception

    ''' <summary>
    ''' An error at a binary stream offset
    ''' </summary>
    ''' <param name="position"></param>
    ''' <param name="message"></param>
    Public Sub New(position As Long, message As String)
        MyBase.New($"{message}, near offset {position}")
    End Sub

	''' <summary>
	''' An error in a text file
	''' </summary>
	''' <param name="line"></param>
	''' <param name="column"></param>
	''' <param name="message"></param>
	Public Sub New(line As Integer, column As Integer, message As String)
        MyBase.New($"{message}, near line {line} column {column}")
    End Sub

	''' <summary>
	''' An error in a node object
	''' </summary>
	''' <param name="nodePath"></param>
	''' <param name="propertyID"></param>
	''' <param name="message"></param>
	Public Sub New(nodePath As Stack(Of String), propertyID As Integer, message As String)
        MyBase.New(message & ", at " & String.Join("/", nodePath.ToArray()) & (If(propertyID < 0, "", $"[{propertyID}]")))
    End Sub
End Class
