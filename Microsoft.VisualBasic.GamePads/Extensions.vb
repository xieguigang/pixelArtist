Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.GamePads.Abstract
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public Module Extensions

    <Extension> Public Sub Reset(ByRef score As IScore)
        If score.Score > score.Highest Then
            score.Highest = score.Score
        End If
        score.Score = 0
    End Sub

    ''' <summary>
    ''' <paramref name="b"/> position relatives to <paramref name="a"/>
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    <Extension> Public Function Position(a As Point, b As Point, Optional max As Boolean = False) As EngineParts.Controls
        Dim v, h As EngineParts.Controls
        Dim dv, dh As Integer

        If b.X > a.X Then
            h = EngineParts.Controls.Right
        ElseIf b.X < a.X Then
            h = EngineParts.Controls.Left
        End If

        dh = Math.Abs(a.X - b.X)

        If b.Y > a.Y Then
            v = EngineParts.Controls.Down
        ElseIf b.Y < a.Y Then
            v = EngineParts.Controls.Up
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

    Public Function PointTo(a As Point, b As Point, d As EngineParts.Controls, Optional max As Boolean = False) As Boolean
        Dim pos As EngineParts.Controls = Position(a, b, max)

        If pos.HasFlag(EngineParts.Controls.Down) <> d.HasFlag(EngineParts.Controls.Down) Then
            Return False
        End If
        If pos.HasFlag(EngineParts.Controls.Left) <> d.HasFlag(EngineParts.Controls.Left) Then
            Return False
        End If
        If pos.HasFlag(EngineParts.Controls.Right) <> d.HasFlag(EngineParts.Controls.Right) Then
            Return False
        End If
        If pos.HasFlag(EngineParts.Controls.Up) <> d.HasFlag(EngineParts.Controls.Up) Then
            Return False
        End If

        Return True
    End Function
End Module
