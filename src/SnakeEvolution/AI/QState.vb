Imports Microsoft.VisualBasic.MachineLearning.QLearning
Imports PixelArtist.Engine

''' <summary>
''' Q state generator
''' </summary>
Public Class QState : Inherits QState(Of GameControl)

    Dim game As SnakeGameEngine

    Public Overrides ReadOnly Iterator Property stateFeatures As IEnumerable(Of String)
        Get
            Yield "position"
            Yield "move_dir"
            Yield "wall_top"
            Yield "wall_bottom"
            Yield "wall_left"
            Yield "wall_right"
            Yield "food_top"
            Yield "food_left"
            Yield "food_right"
            Yield "food_bottom"
        End Get
    End Property

    Public Overrides ReadOnly Iterator Property QValueNames As IEnumerable(Of String)
        Get
            Yield "up"
            Yield "down"
            Yield "left"
            Yield "right"
        End Get
    End Property

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
        Dim foodPos As Point = game.food.Location
        Dim pos As Controls = Position(headPos, game.food.Location, False)
        Dim rect = game.game.screen.Resolution
        Dim top = headPos.Y < rect.Height / 2
        Dim left = headPos.X < rect.Width / 2
        Dim right = headPos.X > rect.Width / 2
        Dim bottom = headPos.Y > rect.Height / 2
        Dim foodtop = foodPos.Y < rect.Height / 2
        Dim foodleft = foodPos.X < rect.Width / 2
        Dim foodright = foodPos.X > rect.Width / 2
        Dim foodbottom = foodPos.Y > rect.Height / 2

        ' 当前的动作加上当前的状态构成q-learn里面的一个状态
        Dim stat As New GameControl With {
            .moveDIR = Direction(game.snake.speedX, game.snake.speedY),
            .position = pos,
            .wallBottom = If(bottom, 1, 0),
            .wallLeft = If(left, 1, 0),
            .wallRight = If(right, 1, 0),
            .wallTop = If(top, 1, 0),
            .foodWallTop = If(foodtop, 1, 0),
            .foodWallBottom = If(foodbottom, 1, 0),
            .foodWallLeft = If(foodleft, 1, 0),
            .foodWallRight = If(foodright, 1, 0)
        }

        Return stat
    End Function

    Public Overrides Function ExtractStateVector(stat As Object) As Double()
        Dim obj As GameControl = stat
        Dim vec As Double() = New Double(10 - 1) {}

        vec(0) = obj.position
        vec(1) = obj.moveDIR
        vec(2) = obj.wallTop
        vec(3) = obj.wallBottom
        vec(4) = obj.wallLeft
        vec(5) = obj.wallRight
        vec(6) = obj.foodWallTop
        vec(7) = obj.foodWallLeft
        vec(8) = obj.foodWallRight
        vec(9) = obj.foodWallBottom

        Return vec
    End Function
End Class