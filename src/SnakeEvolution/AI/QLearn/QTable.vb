Imports Microsoft.VisualBasic.MachineLearning.QLearning
Imports Microsoft.VisualBasic.MachineLearning.QLearning.DataModel

''' <summary>
''' 方向加下一个动作作为key
''' </summary>
Public Class QTable : Inherits QTable(Of GameControl)

    Sub New(range As Integer)
        Call MyBase.New(range)
    End Sub

    Sub New(model As QModel)
        Call MyBase.New(model)
    End Sub

    Protected Overrides Function MapToString(map As GameControl) As String
        Return map.ToString
    End Function
End Class