
' ===== 蛇类 (玩家蛇和AI蛇共用) =====
Public Class Snake
    Public Property Body As New List(Of Point) ' 蛇身, Body(0)为蛇头
    Public Property Direction As Point          ' 移动方向
    Public Property IsPlayer As Boolean         ' 是否为玩家控制
    Public Property PendingGrowth As Integer = 0 ' 待增长的节数
    Public Property BodyColor As Color
    Public Property HeadColor As Color
    Public Property Alive As Boolean = True

    Public ReadOnly Property Length As Integer
        Get
            Return Body.Count
        End Get
    End Property

    Public ReadOnly Property Head As Point
        Get
            Return Body(0)
        End Get
    End Property

    ' 移动一步: 头部前进, 尾部删除 (除非有待增长)
    Public Sub Move()
        Dim newHead As New Point(Head.X + Direction.X, Head.Y + Direction.Y)
        Body.Insert(0, newHead)
        If PendingGrowth > 0 Then
            PendingGrowth -= 1
        Else
            Body.RemoveAt(Body.Count - 1)
        End If
    End Sub

    ' 增加待增长节数
    Public Sub Grow(amount As Integer)
        PendingGrowth += amount
    End Sub

    ' 检查某点是否在蛇身上
    Public Function ContainsBody(pos As Point, Optional skipHead As Boolean = False) As Boolean
        Dim startIdx As Integer = If(skipHead, 1, 0)
        For i As Integer = startIdx To Body.Count - 1
            If Body(i) = pos Then Return True
        Next
        Return False
    End Function
End Class