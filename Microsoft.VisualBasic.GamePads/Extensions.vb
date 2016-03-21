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
    <Extension> Public Function Position(a As Point, b As Point) As Controls
        Dim result As Controls = Controls.NotBind

        If b.X > a.X Then
            result = result Or Controls.Right
        ElseIf b.X < a.X Then
            result = result Or Controls.Left
        End If

        If b.Y > a.Y Then
            result = result Or Controls.Down
        ElseIf b.Y < a.Y Then
            result = result Or Controls.Up
        End If

        Return result
    End Function
End Module
