'Imports System.Drawing
'Imports Microsoft.VisualBasic.DataMining.Framework
'Imports Microsoft.VisualBasic.GamePads.EngineParts
'Imports Microsoft.VisualBasic.Parallel
'Imports Microsoft.VisualBasic
'Imports Microsoft.VisualBasic.Serialization
'Imports Microsoft.VisualBasic.ComponentModel.Collection
'Imports Microsoft.VisualBasic.ComponentModel.DataStructures
'Imports Microsoft.VisualBasic.GamePads
'Imports Microsoft.VisualBasic.Linq

'Module LearnPlayGame

'    ''' <summary>
'    ''' inputs:
'    ''' head location x,y   0,1
'    ''' food location x,y   2,3
'    ''' run direction d     4
'    ''' distance to the wall:
'    ''' up, down, left, right  5,6,7,8
'    '''
'    ''' outputs is the key control
'    ''' up, down, left, right
'    ''' </summary>
'    ReadOnly AI As New NeuralNetwork.Network(9, 15, 4)
'    ReadOnly game As Snake.GameEngine

'    Sub New()
'        Dim game = New Snake.Form1
'        Call RunTask(AddressOf game.ShowDialog)
'        Call Threading.Thread.Sleep(2000)
'        LearnPlayGame.game = game.GameEngine
'        game.GameEngine.ControlsMap.Enable = False
'    End Sub

'    ''' <summary>
'    ''' AI learn to play a snake game by itself, not a human teaches him
'    ''' </summary>
'    Public Sub LearnPlay(games As Integer)
'        Dim learn As New NeuralNetwork.TrainingUtils(AI)
'        Dim size = game.DisplayDriver.DeviceSize
'        Dim rand As New Random(Now.Second + Now.Hour + Now.Minute)

'        ' init control
'        Call learn.Add(__inputs(New Point(0, 0), New Point(0, 100), Controls.Left, size), {0.0R, 1.0R, 0R, 0R})
'        Call learn.Add(__inputs(New Point(0, 100), New Point(0, 0), Controls.Left, size), {1.0R, .0R, 0R, 0R})
'        Call learn.Add(__inputs(New Point(100, 0), New Point(0, 0), Controls.Left, size), {0.0R, .0R, 1.0R, 0R})
'        Call learn.Add(__inputs(New Point(0, 0), New Point(100, 0), Controls.Left, size), {0.0R, .0R, 0R, 1.0R})
'        Call learn.Train()

'        '   Dim errControls As New CapacityQueue(Of NeuralNetwork.DataSet)(6) ' assume that the last 6 operation makes the game over
'        Dim list As New List(Of NeuralNetwork.DataSet)
'        Dim la As New LoopArray(Of Controls)({Controls.Down, Controls.Left, Controls.Right, Controls.Up})

'        game.ScoreCallback = Sub(food As Point)
'                                 Dim LQuery = (From x In list Let foodX As Point = New Point(x.Values(2), x.Values(3)) Where food = foodX Select x).ToArray
'                                 For Each x In LQuery
'                                     Call learn.Corrects(x, getOutput(GetController(x.Targets)), False)
'                                 Next
'                                 Call "Learning.....".__DEBUG_ECHO
'                                 Call learn.Train()
'                             End Sub

'        For i As Integer = 0 To games - 1

'            game.Invoke(la.GET)

'            Do While Not game.GameOver


'                Threading.Thread.Sleep(10)

'                Dim input As Double() = __inputs(game.Snake.Head.Location, game.food.Location, game.Snake.Direction, size)
'                Dim out As Double() = learn.NeuronNetwork.Compute(input)
'                Dim c = GetController(out)

'                game.Invoke(c)
'                list += New NeuralNetwork.DataSet(input, out)

'            Loop

'            Call list.Clear()
'            Call learn.XP.__DEBUG_ECHO

'            ' AI snake dead then restart the game playing and continute learning
'            Call game.Reset()
'            Call $"Games {i}".__DEBUG_ECHO
'        Next
'    End Sub


'    Public Function getOutput(d As Controls) As Double()
'        Dim up As Double = If(d.HasFlag(Controls.Up), 1, 0)
'        Dim down As Double = If(d.HasFlag(Controls.Down), 1, 0)
'        Dim left As Double = If(d.HasFlag(Controls.Left), 1, 0)
'        Dim right As Double = If(d.HasFlag(Controls.Right), 1, 0)
'        Return {up, down, left, right}
'    End Function

'    Private Function GetController(out As Double()) As Controls
'        'Dim result As Controls = Controls.NotBind

'        'If out(0) > 0.5 Then
'        '    result = result Or Controls.Up
'        'End If
'        'If out(1) > 0.5 Then
'        '    result = result Or Controls.Down
'        'End If
'        'If out(2) > 0.5 Then
'        '    result = result Or Controls.Left
'        'End If
'        'If out(3) > 0.5 Then
'        '    result = result Or Controls.Right
'        'End If

'        'Return result

'        Dim ind As Integer = out.MaxInd


'        Select Case ind
'            Case 0
'                Call Console.Write("↑")
'                Return Controls.Up
'            Case 1
'                Call Console.Write("↓")
'                Return Controls.Down
'            Case 2
'                Call Console.Write("←")
'                Return Controls.Left
'            Case 3
'                Call Console.Write("→")
'                Return Controls.Right
'        End Select

'        Return Controls.NotBind
'    End Function

'    Private Function __inputs(head As Point, food As Point, direct As Controls, size As Size) As Double()
'        Dim p As New List(Of Double)
'        p += {CDbl(head.X), CDbl(head.Y)}
'        p += {CDbl(food.X), CDbl(food.Y)}
'        p += direct

'        Dim dup As Double = head.Y
'        Dim ddown As Double = size.Height - head.Y
'        Dim dleft As Double = head.X
'        Dim dright As Double = size.Width - head.X

'        p += {dup, ddown, dleft, dright}

'        Return p
'    End Function
'End Module
