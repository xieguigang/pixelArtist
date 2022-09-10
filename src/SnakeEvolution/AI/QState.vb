Imports Microsoft.VisualBasic.MachineLearning.QLearning
Imports PixelArtist.Engine

''' <summary>
''' Q state generator
''' </summary>
Public Class QState : Inherits QState(Of GameControl)

    Dim game As SnakeGameEngine

    Sub New(game As SnakeGameEngine)
        Me.game = game
    End Sub

    ''' <summary>
    ''' The position relationship of the snake head and his food consists of the current environment state
    ''' </summary>
    ''' <param name="action">当前的动作</param>
    ''' <returns></returns>
    Public Overrides Function GetNextState(action As Integer) As GameControl
        Dim headPos As Point = game.snake.Head
        Dim pos As Controls = Position(headPos, game.food.Location, False)
        Dim rect = game.game.screen.Resolution
        Dim top = headPos.Y < rect.Height / 2
        Dim left = headPos.X < rect.Width / 2
        Dim right = headPos.X > rect.Width / 2
        Dim bottom = headPos.Y > rect.Height / 2
        ' 当前的动作加上当前的状态构成q-learn里面的一个状态
        Dim stat As New GameControl With {
            .moveDIR = Direction(game.snake.speedX, game.snake.speedY),
            .position = pos,
            .wallBottom = If(bottom, 1, 0),
            .wallLeft = If(left, 1, 0),
            .wallRight = If(right, 1, 0),
            .wallTop = If(top, 1, 0)
        }

        Return stat
    End Function
End Class