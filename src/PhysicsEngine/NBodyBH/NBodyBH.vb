


Imports System.Drawing
Imports System.IO

''' <summary>
''' NBodyBH.java
''' 
''' Reads in a universe of N bodies from stdin, and performs an
''' N-Body simulation in O(N log N) using the Barnes-Hut algorithm.
''' 
''' Compilation:  javac NBodyBH.java
''' Execution:    java NBodyBH <inputs/>
''' </summary>
Public Class NBodyBH

    Public Shared Sub Main(args As String())

        ' for reading from stdin
        Dim console As StreamReader = Nothing

        Const dt = 0.1 ' time quantum
        Dim N As Integer = Integer.Parse(console.ReadLine()) ' number of particles
        Dim radius As Double = Double.Parse(console.ReadLine()) ' radius of universe

        ' turn on animation mode and rescale coordinate system
        ' StdDraw.show(0);
        'StdDraw.setXscale(-radius, +radius);
        'StdDraw.setYscale(-radius, +radius);

        ' read in and initialize bodies
        Dim bodies = New Body(N - 1) {} ' array of N bodies
        For i = 0 To N - 1
            Dim line = console.ReadLine()
            Dim tl = line.Split(New Char() {" "c, ChrW(9)}, StringSplitOptions.RemoveEmptyEntries)

            Dim px = Double.Parse(tl(0))
            Dim py = Double.Parse(tl(1))
            Dim vx = Double.Parse(tl(2))
            Dim vy = Double.Parse(tl(3))
            Dim mass = Double.Parse(tl(4))
            Dim red = Integer.Parse(tl(5))
            Dim green = Integer.Parse(tl(6))
            Dim blue = Integer.Parse(tl(7))
            Dim color As Color = Color.FromArgb(red, green, blue)
            bodies(i) = New Body(px, py, vx, vy, mass, color)
        Next


        ' simulate the universe
        Dim t = 0.0

        While True

            Dim quad As Quad = New Quad(0, 0, radius * 2)
            Dim tree As BHTree = New BHTree(quad)

            ' build the Barnes-Hut tree
            For i = 0 To N - 1
                If bodies(i).in(quad) Then
                    tree.insert(bodies(i))
                End If
            Next

            ' update the forces, positions, velocities, and accelerations
            For i = 0 To N - 1
                bodies(i).resetForce()
                tree.updateForce(bodies(i))
                bodies(i).update(dt)
            Next

            ' draw the bodies
            '  StdDraw.clear(StdDraw.BLACK);
            For i = 0 To N - 1
                bodies(i).draw()
            Next

            t = t + dt

            ' StdDraw.show(10);
        End While
    End Sub
End Class
