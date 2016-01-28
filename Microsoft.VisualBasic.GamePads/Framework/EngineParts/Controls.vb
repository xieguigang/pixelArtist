Namespace EngineParts

    Public Enum Controls As Integer

        ''' <summary>
        ''' 按键未绑定
        ''' </summary>
        NotBind = -100

#Region "Direction Controls"
        Up = 8
        Down = 2
        Left = 4
        Right = 6
#End Region

#Region "Game Controls"
        ''' <summary>
        ''' 游戏暂停
        ''' </summary>
        Pause = 100
        ''' <summary>
        ''' 确认
        ''' </summary>
        Ok
        ''' <summary>
        ''' 取消
        ''' </summary>
        Cancel
        ''' <summary>
        ''' 呼出菜单
        ''' </summary>
        Menu
#End Region

#Region "Game Actions"
        Fire = 1000
        Jumps
#End Region
    End Enum
End Namespace