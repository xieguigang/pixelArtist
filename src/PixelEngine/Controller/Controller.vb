''' <summary>
''' Capture the input device actions, including mouse input and keyboard key pressing.
''' (捕捉动作的输入的设备，包括键盘和鼠标)
''' </summary>
Public Class Controller

    ''' <summary>
    ''' The gamepad input mappings, mapping the keyboard input as the internal actions.
    ''' (输入映射，将键盘按键映射为游戏内部的动作信号)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ControlMaps As New ControlMaps
    Public Property Enable As Boolean = True

    Dim engine As WorldEngine

    Sub New(engine As WorldEngine)
        Me.engine = engine
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="action">
    ''' the evaluated mapped command action
    ''' </param>
    ''' <param name="key">
    ''' the raw key char from the keyboard
    ''' </param>
    Public Delegate Sub FireCommand(action As Controls, key As Char)
    Public Delegate Sub ClickObject(pos As Point, obj As CharacterModel)

    Public Sub CallCommand()
        Dim screen = engine.screen
        Dim key As Char = screen.Invoke(Function() screen.GetCommand)
        Dim action = ControlMaps.GetMapAction(key)

        Call engine.controls(action, key)
    End Sub

    Public Sub CallModelClick()
        Dim screen = engine.screen
        Dim pixel As SizeF = screen.PixelSize
        Dim xy As Point = screen.Invoke(Function() screen.GetClick)

        Call engine _
           .Select(Function(x) __invokeClick(x, xy)) _
           .ToArray
    End Sub

    Private Function __invokeClick(x As CharacterModel, pos As Point, pixel As SizeF) As Boolean
        If x.GetPixels(pixel).Any(Function(r) r.Contains(pos)) Then
            Call ClickObject(pos, x)
        End If

        Return True
    End Function
End Class