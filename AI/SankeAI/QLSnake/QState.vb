Imports Microsoft.VisualBasic.DataMining.Framework.QLearning
Imports Microsoft.VisualBasic.GamePads.EngineParts

Public Structure GameControl : Implements ICloneable

    Dim action As Controls

    Public Overrides Function ToString() As String
        Return action.Description
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New GameControl With {
            .action = action
        }
    End Function
End Structure

Public Class QState : Inherits QState(Of GameControl)

    Public Overrides Function GetNextState(action As Integer) As GameControl

    End Function
End Class
