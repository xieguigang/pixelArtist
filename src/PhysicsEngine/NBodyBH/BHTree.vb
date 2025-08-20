''' <summary>
''' BHTree.java
''' 
''' Represents a quadtree for the Barnes-Hut algorithm.
''' 
''' Dependencies: Body.java Quad.java
''' 
''' @author chindesaurus
''' @version 1.00 
''' </summary>

Public Class BHTree

    ' threshold value
    Private ReadOnly Theta As Double = 0.5

    Private body As Body ' body or aggregate body stored in this node
    Private quad As Quad ' square region that the tree represents
    Private NW As BHTree ' tree representing northwest quadrant
    Private NE As BHTree ' tree representing northeast quadrant
    Private SW As BHTree ' tree representing southwest quadrant
    Private SE As BHTree ' tree representing southeast quadrant

    ''' <summary>
    ''' Constructor: creates a new Barnes-Hut tree with no bodies. 
    ''' Each BHTree represents a quadrant and an aggregate body 
    ''' that represents all bodies inside the quadrant.
    ''' </summary>
    ''' <paramname="q"> the quadrant this node is contained within </param>
    Public Sub New(q As Quad)
        quad = q
        body = Nothing
        NW = Nothing
        NE = Nothing
        SW = Nothing
        SE = Nothing
    End Sub


    ''' <summary>
    ''' Adds the Body b to the invoking Barnes-Hut tree.
    ''' </summary>
    Public Overridable Sub insert(b As Body)

        ' if this node does not contain a body, put the new body b here
        If body Is Nothing Then
            body = b
            Return
        End If

        ' internal node
        If Not External Then
            ' update the center-of-mass and total mass
            body = body.plus(b)

            ' recursively insert Body b into the appropriate quadrant

            ' external node
            putBody(b)
        Else
            ' subdivide the region further by creating four children
            NW = New BHTree(quad.NW())
            NE = New BHTree(quad.NE())
            SE = New BHTree(quad.SE())
            SW = New BHTree(quad.SW())

            ' recursively insert both this body and Body b into the appropriate quadrant
            putBody(body)
            putBody(b)

            ' update the center-of-mass and total mass
            body = body.plus(b)
        End If
    End Sub


    ''' <summary>
    ''' Inserts a body into the appropriate quadrant.
    ''' </summary>
    Private Sub putBody(b As Body)
        If b.in(quad.NW()) Then
            NW.insert(b)
        ElseIf b.in(quad.NE()) Then
            NE.insert(b)
        ElseIf b.in(quad.SE()) Then
            SE.insert(b)
        ElseIf b.in(quad.SW()) Then
            SW.insert(b)
        End If
    End Sub


    ''' <summary>
    ''' Returns true iff this tree node is external.
    ''' </summary>
    Private ReadOnly Property External As Boolean
        Get
            ' a node is external iff all four children are null
            Return NW Is Nothing AndAlso NE Is Nothing AndAlso SW Is Nothing AndAlso SE Is Nothing
        End Get
    End Property


    ''' <summary>
    ''' Approximates the net force acting on Body b from all bodies
    ''' in the invoking Barnes-Hut tree, and updates b's force accordingly.
    ''' </summary>
    Public Overridable Sub updateForce(b As Body)

        If body Is Nothing OrElse b.Equals(body) Then
            Return
        End If

        ' if the current node is external, update net force acting on b
        If External Then

            ' for internal nodes
            b.addForce(body)
        Else

            ' width of region represented by internal node
            Dim s As Double = quad.length()

            ' distance between Body b and this node's center-of-mass
            Dim d = body.distanceTo(b)

            ' compare ratio (s / d) to threshold value Theta
            If s / d < Theta Then

                ' recurse on each of current node's children
                b.addForce(body) ' b is far away
            Else
                NW.updateForce(b)
                NE.updateForce(b)
                SW.updateForce(b)
                SE.updateForce(b)
            End If
        End If
    End Sub


    ''' <summary>
    ''' Returns a string representation of the Barnes-Hut tree
    ''' in which spaces represent external nodes, and asterisks
    ''' represent internal nodes.
    ''' </summary>
    ''' <returns> a string representation of this quadtree </returns>
    Public Overrides Function ToString() As String
        If External Then
            Return " " & body.ToString() & vbLf
        Else
            Return "*" & body.ToString() & vbLf & NW.ToString() & NE.ToString() & SW.ToString() & SE.ToString()
        End If
    End Function
End Class
