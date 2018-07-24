Imports System.Collections.Generic

''' <summary>
''' Represents a node in an FBX file
''' </summary>
Public Class FbxNode
	Inherits FbxNodeList
	''' <summary>
	''' The node name, which is often a class type
	''' </summary>
	''' <remarks>
	''' The name must be smaller than 256 characters to be written to a binary stream
	''' </remarks>
	Public Name As String

	''' <summary>
	''' The list of properties associated with the node
	''' </summary>
	''' <remarks>
	''' Supported types are primitives (apart from byte and char),arrays of primitives, and strings
	''' </remarks>
	Public Properties As New List(Of Object)()

	''' <summary>
	''' The first property element
	''' </summary>
	Public Property Value() As Object
		Get
			Return If(Properties.Count < 1, Nothing, Properties(0))
		End Get
		Set
			If Properties.Count < 1 Then
				Properties.Add(value)
			Else
				Properties(0) = value
			End If
		End Set
	End Property

	''' <summary>
	''' Whether the node is empty of data
	''' </summary>
	Public Function IsEmpty() As Boolean
		Return String.IsNullOrEmpty(Name) AndAlso Properties.Count = 0 AndAlso Nodes.Count = 0
	End Function
End Class
