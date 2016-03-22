Imports Microsoft.VisualBasic.DataMining.Framework.QLearning
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.GamePads

Public Structure GameControl : Implements ICloneable

    Dim action As Controls
    Dim position As Controls

    Public Overrides Function ToString() As String
        Return $"{CInt(position)} --> {CInt(action)}"
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New GameControl With {
            .action = action,
            .position = position
        }
    End Function
End Structure

Public Class QState : Inherits QState(Of GameControl)

    Dim game As Snake.GameEngine

    Sub New(game As Snake.GameEngine)
        Me.game = game
    End Sub

    Public Overrides Function GetNextState(action As Integer) As GameControl
        Dim pos As Controls = Position(game.Snake.Location, game.food.Location, False)
        Dim stat = New GameControl With {.action = game.Snake.Direction, .position = pos}
        Return stat
    End Function
End Class
