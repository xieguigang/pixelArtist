Imports Microsoft.VisualBasic.DataMining.Framework.QLearning
Imports SankeAI

Public Class QTable : Inherits QTable(Of GameControl)

    Sub New(range As Integer)
        Call MyBase.New(range)
    End Sub

    Protected Overrides Function __getMapString(map As GameControl) As String
        Return CInt(map.action).ToString
    End Function
End Class
