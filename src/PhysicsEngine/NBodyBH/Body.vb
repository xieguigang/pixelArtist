Imports System
Imports System.Drawing

''' <summary>
''' Body.java
''' 
''' Represents a Body (a point mass) and its position, 
''' velocity, mass, color, and the net force acting upon it.
''' 
''' @author chindesaurus
''' @version 1.00
''' </summary>

Public Class Body

    ' gravitational constant
    Private Const G As Double = 6.67e-11

    Private rx, ry As Double ' position
    Private vx, vy As Double ' velocity
    Private fx, fy As Double ' force
    Private mass As Double ' mass
    Private color As Color ' color

    ''' <summary>
    ''' Constructor: creates and initializes a new Body.
    ''' </summary>
    ''' <paramname="rx">    the x-position of this new body </param>
    ''' <paramname="ry">    the y-position of this new body </param>
    ''' <paramname="vx">    the x-velocity of this new body </param>
    ''' <paramname="vy">    the y-velocity of this new body </param>
    ''' <paramname="mass">  the mass of this new body </param>
    ''' <paramname="color"> the color of this new body (RGB) </param>
    Public Sub New(rx As Double, ry As Double, vx As Double, vy As Double, mass As Double, color As Color)
        Me.rx = rx
        Me.ry = ry
        Me.vx = vx
        Me.vy = vy
        Me.mass = mass
        Me.color = color
    End Sub

    ''' <summary>
    ''' Updates the velocity and position of the invoking Body
    ''' using leapfrom method, with timestep dt.
    ''' </summary>
    ''' <paramname="dt"> the timestep for this simulation </param>
    Public Overridable Sub update(dt As Double)
        vx += dt * fx / mass
        vy += dt * fy / mass
        rx += dt * vx
        ry += dt * vy
    End Sub

    ''' <summary>
    ''' Returns the Euclidean distance between the invoking Body and b.
    ''' </summary>
    ''' <paramname="b"> the body from which to determine the distance </param>
    ''' <returns>  the distance between this and Body b </returns>
    Public Overridable Function distanceTo(b As Body) As Double
        Dim dx = rx - b.rx
        Dim dy = ry - b.ry
        Return Math.Sqrt(dx * dx + dy * dy)
    End Function

    ''' <summary>
    ''' Resets the force (both x- and y-components) of the invoking Body to 0.
    ''' </summary>
    Public Overridable Sub resetForce()
        fx = 0.0
        fy = 0.0
    End Sub

    ''' <summary>
    ''' Computes the net force acting between the invoking body and b, and
    ''' adds this to the net force acting on the invoking Body.
    ''' </summary>
    ''' <paramname="b"> the body whose net force on this body to calculate </param>
    Public Overridable Sub addForce(b As Body)
        Dim a = Me
        Dim EPS = 3E4 ' softening parameter
        Dim dx = b.rx - a.rx
        Dim dy = b.ry - a.ry
        Dim dist = Math.Sqrt(dx * dx + dy * dy)
        Dim F = G * a.mass * b.mass / (dist * dist + EPS * EPS)
        a.fx += F * dx / dist
        a.fy += F * dy / dist
    End Sub

    ''' <summary>
    ''' Draws the invoking Body. 
    ''' </summary>
    Public Overridable Sub draw()
        'StdDraw.PenColor = color;
        'StdDraw.point(rx, ry);
    End Sub

    ''' <summary>
    ''' Returns a string representation of this body formatted nicely.
    ''' </summary>
    ''' <returns> a formatted string containing this body's x- and y- positions,
    '''         velocities, and mass </returns>
    Public Overrides Function ToString() As String
        Return String.Format("{0,10:E3} {1,10:E3} {2,10:E3} {3,10:E3} {4,10:E3}", rx, ry, vx, vy, mass)
    End Function

    ''' <summary>
    ''' Returns true if the body is in quadrant q, else false.
    ''' </summary>
    ''' <paramname="q"> the Quad to check </param>
    ''' <returns>  true iff body is in Quad q, else false </returns>
    Public Overridable Function [in](q As Quad) As Boolean
        Return q.contains(rx, ry)
    End Function

    ''' <summary>
    ''' Returns a new Body object that represents the center-of-mass
    ''' of the invoking body and b.
    ''' </summary>
    ''' <paramname="b"> the body to aggregate with this Body </param>
    ''' <returns>  a Body object representing an aggregate of this 
    '''          and b, having this and b's center of gravity and
    '''          combined mass </returns>
    Public Overridable Function plus(b As Body) As Body
        Dim a = Me

        Dim m = a.mass + b.mass
        Dim x = (a.rx * a.mass + b.rx * b.mass) / m
        Dim y = (a.ry * a.mass + b.ry * b.mass) / m

        Return New Body(x, y, a.vx, b.vx, m, a.color)
    End Function
End Class
