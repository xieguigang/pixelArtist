Imports System.Drawing
Imports Microsoft.VisualBasic.DataVisualization.DataMining.Framework
Imports Microsoft.VisualBasic.GamePads.EngineParts
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.GamePads

Module LearnPlayGame

    ''' <summary>
    ''' inputs:
    ''' head location x,y   0,1
    ''' food location x,y   2,3
    ''' run direction d     4
    ''' distance to the wall:  
    ''' up, down, left, right  5,6,7,8
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

        Dim errControls As New CapacityQueue(Of NeuralNetwork.DataSet)(6) ' assume that the last 6 operation makes the game over

        For i As Integer = 0 To games - 1
            Do While Not game.GameOver
                Dim input As Double() = __inputs(game.Snake.Head.Location, game.food.Location, game.Snake.Direction, size)
                Dim out As Double() = learn.NeuronNetwork.Compute(input)

                Call out.GetJson.__DEBUG_ECHO

                game.Invoke(GetController(out))
                errControls += New NeuralNetwork.DataSet(input, out)

                Threading.Thread.Sleep(10)
            Loop
            ' AI snake dead then restart the game playing and continute learning
            Call game.Reset()
            Call $"Games {i}".__DEBUG_ECHO

            ' corrects errors
            For Each x In errControls
                Dim d As Controls = GetController(x.Targets)
                Dim head As New Point(x.Values(0), x.Values(1))
                Dim food As New Point(x.Values(2), x.Values(3))

                Dim c As Controls = head.Position(food)

                'up, down, left, right  5,6,7,8
                If x.Values(5) <= 2 Then
                    c = Controls.Down
                End If
                If x.Values(6) <= 2 Then
                    c = Controls.Up
                End If
                If x.Values(7) <= 2 Then
                    c = Controls.Right
                End If
                If x.Values(8) <= 2 Then
                    c = Controls.Left
                End If

                Call learn.Corrects(x, getOutput(c), False)
            Next

            Call learn.Train()
        Next
    End Sub

    Public Function getOutput(d As Controls) As Double()
        Dim up As Double = If(d.HasFlag(Controls.Up), 1, 0)
        Dim down As Double = If(d.HasFlag(Controls.Down), 1, 0)
        Dim left As Double = If(d.HasFlag(Controls.Left), 1, 0)
        Dim right As Double = If(d.HasFlag(Controls.Right), 1, 0)
        Return {up, down, left, right}
    End Function

    Private Function GetController(out As Double()) As Controls
        If out(0) > 0.5 Then
            Return Controls.Up
        ElseIf out(1) > 0.5 Then
            Return Controls.Down
        ElseIf out(2) > 0.5 Then
            Return Controls.Left
        ElseIf out(3) > 0.5 Then
            Return Controls.Right
        End If

        Return Controls.Left
    End Function

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
