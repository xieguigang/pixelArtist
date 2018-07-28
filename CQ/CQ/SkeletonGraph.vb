Imports System.Drawing
Imports Microsoft.VisualBasic.Data.GraphTheory

Public Class SkeletonGraph : Inherits Graph(Of SkeletonNode, Edge(Of SkeletonNode), SkeletonGraph)

End Class

Public Class SkeletonNode : Inherits Vertex
    Public Property Texture As Bitmap
End Class