Imports Microsoft.VisualBasic.MachineLearning.QLearning
Imports PixelArtist.Engine

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
        Dim stat As New GameControl With {
            .moveDIR = game.Snake.Direction,
            .position = pos
        }  ' 当前的动作加上当前的状态构成q-learn里面的一个状态
        Return stat
    End Function
End Class