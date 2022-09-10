Imports Microsoft.VisualBasic.Text
''' <summary>
''' 键盘对游戏内部动作的映射绑定
''' </summary>
Public Class ControlMaps

    ''' <summary>
    ''' 键盘按键绑定方案
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Maps As New Dictionary(Of Char, Controls)

    ''' <summary>
    ''' 进行按键绑定
    ''' </summary>
    ''' <param name="key">按键，小写字母</param>
    ''' <param name="action">所需要进行绑定的输入信号</param>
    Public Sub BindMapping(key As Char, action As Controls)
        If Maps.ContainsKey(key) Then
            Call Maps.Remove(key)
        End If
        Call Maps.Add(key, action)
    End Sub

    ''' <summary>
    ''' 得到用户输入的动作
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    Public Function GetMapAction(key As Char) As Controls
        If key = ASCII.NUL Then
            Return Controls.NotBind
        End If
        If Not Maps.ContainsKey(key) Then
            Return Controls.NotBind
        Else
            Return Maps(key)
        End If
    End Function

    ''' <summary>
    ''' 默认的按键绑定方案
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    Public Shared Function DefaultMaps(Optional ByRef maps As ControlMaps = Nothing) As ControlMaps
        If maps Is Nothing Then
            maps = New ControlMaps
        End If

        Call maps.BindMapping("w"c, Controls.Up)
        Call maps.BindMapping("s"c, Controls.Down)
        Call maps.BindMapping("a"c, Controls.Left)
        Call maps.BindMapping("d"c, Controls.Right)
        Call maps.BindMapping(" "c, Controls.Jumps)
        Call maps.BindMapping("1"c, Controls.Fire)
        Call maps.BindMapping("`"c, Controls.Menu)
        Call maps.BindMapping("p"c, Controls.Pause)
        Call maps.BindMapping(vbCrLf, Controls.Ok)

        Return maps
    End Function
End Class