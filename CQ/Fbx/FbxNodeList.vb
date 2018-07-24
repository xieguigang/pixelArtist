Imports System.Collections.Generic

''' <summary>
''' Base class for nodes and documents
''' </summary>
Public MustInherit Class FbxNodeList
	''' <summary>
	''' The list of child/nested nodes
	''' </summary>
	''' <remarks>
	''' A list with one or more null elements is treated differently than an empty list,
	''' and represented differently in all FBX output files.
	''' </remarks>
	Public Nodes As New List(Of FbxNode)()

	''' <summary>
	''' Gets a named child node
	''' </summary>
	''' <param name="name"></param>
	''' <returns>The child node, or null</returns>
	Public Default ReadOnly Property Item(name As String) As FbxNode
		Get
			Return Nodes.Find(Function(n) n IsNot Nothing AndAlso n.Name = name)
		End Get
	End Property

	''' <summary>
	''' Gets a child node, using a '/' separated path
	''' </summary>
	''' <param name="path"></param>
	''' <returns>The child node, or null</returns>
	Public Function GetRelative(path As String) As FbxNode
		Dim tokens = path.Split("/"C)
		Dim n As FbxNodeList = Me
        For Each t As String In tokens
            If t = "" Then
                Continue For
            End If
            n = n(t)
            If n Is Nothing Then
                Exit For
            End If
        Next
        Return TryCast(n, FbxNode)
	End Function
End Class
