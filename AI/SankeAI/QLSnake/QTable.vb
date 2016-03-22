Imports Microsoft.VisualBasic.DataMining.Framework.QLearning
Imports SankeAI

''' <summary>
''' 方向加下一个动作作为key
''' </summary>
Public Class QTable : Inherits QTable(Of GameControl)

    Sub New(range As Integer)
        Call MyBase.New(range)
    End Sub

    Protected Overrides Function __getMapString(map As GameControl) As String
        Return map.ToString
    End Function
End Class
