Imports System.Drawing
Imports Microsoft.VisualBasic.DataVisualization.DataMining.Framework
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization

Module LearnPlayGame

    ''' <summary>
    ''' inputs:
    ''' head location x,y
    ''' food location x,y
    ''' run direction d
    ''' distance to the wall:
    ''' up, down, left, right
    ''' 
    ''' outputs is the key control
    ''' up, down, left, right
    ''' </summary>
    ReadOnly AI As New NeuralNetwork.Network(9, 5, 4)
    ReadOnly game As Snake.GameEngine

    Sub New()
        Dim game = New Snake.Form1
        Call RunTask(AddressOf game.ShowDialog)
        Call Threading.Thread.Sleep(2000)
        LearnPlayGame.game = game.GameEngine
        game.GameEngine.ControlsMap.Enable = False
    End Sub

    ''' <summary>
    ''' AI learn to play a snake game by itself, not a human teaches him
    ''' </summary>
    Public Sub LearnPlay(games As Integer)
        Dim learn As New NeuralNetwork.TrainingUtils(AI)
        Dim size = game.DisplayDriver.DeviceSize
        Dim rand As New Random(Now.Second + Now.Hour + Now.Minute)

        ' init randonm control
        Call learn.Add(__inputs(New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), Controls.Up, size), {1.0R, 0R, 0R, 0R})
        Call learn.Add(__inputs(New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), Controls.Right, size), {0R, 0R, 0R, 1.0R})
        Call learn.Add(__inputs(New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), Controls.Left, size), {0R, 0R, 1.0R, .0R})
        Call learn.Add(__inputs(New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), Controls.Down, size), {0R, 1.0R, 0R, 0R})
        Call learn.Add(__inputs(New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), New Point(rand.Next(0, size.Width), rand.Next(0, size.Height)), Controls.Left, size), {0R, 0R, 0R, 1.0R})
        Call learn.Train()
        Call "Init network with randomize inputs...".__DEBUG_ECHO

        For i As Integer = 0 To games - 1
            Do While Not game.GameOver
                Dim input As Double() = __inputs(game.Snake.Head.Location, game.food.Location, game.Snake.Direction, size)
                Dim out As Double() = learn.NeuronNetwork.Compute(input)

                Call out.GetJson.__DEBUG_ECHO

                If out(0) > 0.5 Then
                    Call game.Invoke(Controls.Up)
                ElseIf out(1) > 0.5 Then
                    Call game.Invoke(Controls.Down)
                ElseIf out(2) > 0.5 Then
                    Call game.Invoke(Controls.Left)
                ElseIf out(3) > 0.5 Then
                    Call game.Invoke(Controls.Right)
                End If

                Threading.Thread.Sleep(10)
            Loop
            ' AI snake dead then restart the game playing and continute learning
            Call game.Reset()
            Call $"Games {i}".__DEBUG_ECHO
        Next
    End Sub

    Private Function __inputs(head As Point, food As Point, direct As Controls, size As Size) As Double()
        Dim p As New List(Of Double)
        p += {CDbl(head.X), CDbl(head.Y)}
        p += {CDbl(food.X), CDbl(food.Y)}
        p += direct

        Dim dup As Double = head.Y
        Dim ddown As Double = size.Height - head.Y
        Dim dleft As Double = head.X
        Dim dright As Double = size.Width - head.X

        p += {dup, ddown, dleft, dright}

        Return p
    End Function
End Module
