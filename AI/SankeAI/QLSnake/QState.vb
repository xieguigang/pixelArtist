Imports Microsoft.VisualBasic.DataMining.Framework.QLearning
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.GamePads

Public Structure GameControl : Implements ICloneable

    Dim action As Controls
    Dim position As Controls
    ''' <summary>
    ''' 蛇的当前运动方向
    ''' </summary>
    Dim moveDIR As Controls

    Public Overrides Function ToString() As String
        Return $"{CInt(position)}, {CInt(moveDIR)} --> {CInt(action)}"
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New GameControl With {
            .action = action,
            .position = position,
            .moveDIR = moveDIR
        }
    End Function
End Structure

Public Class QState : Inherits QState(Of GameControl)

    Dim game As Snake.GameEngine

    Sub New(game As Snake.GameEngine)
        Me.game = game
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="action">当前的动作</param>
    ''' <returns></returns>
    Public Overrides Function GetNextState(action As Integer) As GameControl
        Dim pos As Controls = Position(game.Snake.Location, game.food.Location, False)
        Dim stat = New GameControl With {.action = action, .moveDIR = game.Snake.Direction, .position = pos}  ' 当前的动作加上当前的状态构成q-learn里面的一个状态
        Return stat
    End Function
End Class
