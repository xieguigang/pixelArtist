Namespace EngineParts

    Public Class ControlMaps

        Public ReadOnly Property Maps As Dictionary(Of Char, Controls) =
        New Dictionary(Of Char, Controls)

        ''' <summary>
        ''' 进行按键绑定
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="action"></param>
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
            If Not Maps.ContainsKey(key) Then
                Return Controls.NotBind
            Else
                Return Maps(key)
            End If
        End Function

        Public Shared Function DefaultMaps(ByRef maps As ControlMaps) As ControlMaps
            Call maps.BindMapping("w"c, Controls.Up)
            Call maps.BindMapping("s"c, Controls.Down)
            Call maps.BindMapping("a"c, Controls.Left)
            Call maps.BindMapping("d"c, Controls.Right)
            Call maps.BindMapping(" "c, Controls.Jumps)
            Call maps.BindMapping("1"c, Controls.Fire)
            Call maps.BindMapping("`"c, Controls.Menu)
            Call maps.BindMapping("p"c, Controls.Pause)

            Return maps
        End Function
    End Class
End Namespace