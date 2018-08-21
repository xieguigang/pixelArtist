Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module Offsets

    ''' <summary>
    ''' 使用前一个动画的最后一帧以及下一个动画的第一帧来计算出偏移量
    ''' </summary>
    ''' <param name="previous"></param>
    ''' <param name="[next]"></param>
    ''' <returns></returns>
    Public Function Calculate(previous As Animation, [next] As Animation) As Point
        Dim previousLast = previous.LastFrameRectangle
        Dim nextFirst = [next].FirstFrameRectangle
        Dim dx = nextFirst.Left - previousLast.Left
        Dim dy = nextFirst.Top - previousLast.Top

        Return New Point(dx, dy)
    End Function

    Public Function CalcRectangle(X As Vector, Y As Vector) As Thickness
        Dim left = X.TakeWhile(Function(p) p = 0R).Count
        Dim top = Y.TakeWhile(Function(p) p = 0R).Count
        Dim right = X.Length - X.Reverse.TakeWhile(Function(p) p = 0R).Count
        Dim bottom = Y.Length - Y.Reverse.TakeWhile(Function(p) p = 0R).Count

        Return New Thickness(left, top, right, bottom)
    End Function
End Module