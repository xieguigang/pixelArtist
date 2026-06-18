' ===== 食物类型枚举 =====
Public Enum FoodType
    Regular   ' 常规食物
    Moving    ' 活动食物
    Super     ' 超级食物
End Enum

' ===== 食物类 =====
Public Class Food
    Public Property Position As Point
    Public Property Type As FoodType
    Public Property Velocity As Point          ' 活动食物的速度方向
    Public Property Speed As Integer = 1       ' 活动食物的速度 (1=快, 2=慢)
    Public Property MoveCounter As Integer = 0 ' 移动计数器
    Public Property SpawnTime As DateTime      ' 超级食物的生成时间
    Public Property Color As Color

    ' 超级食物是否已过期 (最多存在60秒)
    Public ReadOnly Property IsExpired As Boolean
        Get
            Return Type = FoodType.Super AndAlso (DateTime.Now - SpawnTime).TotalSeconds >= 60
        End Get
    End Property
End Class