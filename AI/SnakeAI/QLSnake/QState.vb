Imports Microsoft.VisualBasic.DataMining.QLearning
Imports Microsoft.VisualBasic.GamePads
Imports Microsoft.VisualBasic.GamePads.EngineParts

''' <summary>
''' Environment state inputs
''' </summary>
Public Structure GameControl : Implements ICloneable

    ''' <summary>
    ''' The relative position between the head of the snake and his food
    ''' </summary>
    Dim position As Controls
    ''' <summary>
    ''' The current movement direction of the snake.(蛇的当前运动方向)
    ''' </summary>
    Dim moveDIR As Controls

    ''' <summary>
    ''' hash key for the QTable
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return $"{CInt(position)},{vbTab}{CInt(moveDIR)}"
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New GameControl With {
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
    ''' The position relationship of the snake head and his food consists of the current environment state
    ''' </summary>
    ''' <param name="action">当前的动作</param>
    ''' <returns></returns>
    Public Overrides Function GetNextState(action As Integer) As GameControl
        Dim pos As Controls = Position(game.Snake.Location, game.food.Location, False)
        Dim stat = New GameControl With {
            .moveDIR = game.Snake.Direction,
            .position = pos
        }  ' 当前的动作加上当前的状态构成q-learn里面的一个状态
        Return stat
    End Function
End Class
