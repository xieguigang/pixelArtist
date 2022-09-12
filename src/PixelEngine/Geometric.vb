Imports System.Runtime.CompilerServices

Public Module Geometric

    ''' <summary>
    ''' <paramref name="b"/> position relatives to <paramref name="a"/>
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns>8 relative positions</returns>
    <Extension>
    Public Function Position(a As Point, b As Point, Optional max As Boolean = False) As Controls
        Dim v, h As Controls
        Dim dv, dh As Integer

        If b.X > a.X Then
            h = Controls.Right
        ElseIf b.X < a.X Then
            h = Controls.Left
        End If

        dh = Math.Abs(a.X - b.X)

        If b.Y > a.Y Then
            v = Controls.Down
        ElseIf b.Y < a.Y Then
            v = Controls.Up
        End If

        dv = Math.Abs(a.Y - b.Y)

        If max Then
            If dh > dv Then
                Return h
            Else
                Return v
            End If
        Else
            Return h Or v
        End If
    End Function

    Public Function PointTo(a As Point, b As Point, d As Controls, Optional max As Boolean = False) As Boolean
        Dim pos As Controls = Position(a, b, max)

        If pos.HasFlag(Controls.Down) <> d.HasFlag(Controls.Down) Then
            Return False
        End If
        If pos.HasFlag(Controls.Left) <> d.HasFlag(Controls.Left) Then
            Return False
        End If
        If pos.HasFlag(Controls.Right) <> d.HasFlag(Controls.Right) Then
            Return False
        End If
        If pos.HasFlag(Controls.Up) <> d.HasFlag(Controls.Up) Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 4 directions
    ''' </summary>
    ''' <param name="dx"></param>
    ''' <param name="dy"></param>
    ''' <returns></returns>
    Public Function Direction(dx As Integer, dy As Integer) As Controls
        If dx > 0 AndAlso dy = 0 Then
            Return Controls.Right
        ElseIf dx < 0 AndAlso dy = 0 Then
            Return Controls.Left
        ElseIf dx = 0 AndAlso dy > 0 Then
            Return Controls.Down
        ElseIf dx = 0 AndAlso dy < 0 Then
            Return Controls.Up
        Else
            Return Controls.NotBind
        End If
    End Function
End Module
