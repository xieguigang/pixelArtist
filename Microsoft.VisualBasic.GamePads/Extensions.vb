Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.GamePads.Abstract

Public Module Extensions

    <Extension> Public Sub Reset(ByRef score As IScore)
        If score.Score > score.Highest Then
            score.Highest = score.Score
        End If
        score.Score = 0
    End Sub
End Module
