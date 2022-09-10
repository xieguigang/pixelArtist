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

    Public Sub CallCommand()
        Dim screen = engine.screen
        Dim key As Char = screen.Invoke(Function() screen.GetCommand)
        Dim action = ControlMaps.GetMapAction(key)

        Call engine.controls(action, key)
    End Sub

    Private Sub _innerDevice_MouseClick(sender As Object, e As MouseEventArgs) Handles _innerDevice.MouseClick
        If Not Enable Then
            Return
        End If

        Dim xy As Point = e.Location

        Call engine _
            .Select(Function(x) __invokeClick(x, xy)) _
            .ToArray
    End Sub

    Private Function __invokeClick(x As GraphicUnit, pos As Point) As Boolean
        If x.Region.Contains(pos) Then
            Call _clickObject.BeginInvoke(pos, x, Nothing, Nothing)
        End If

        Return True
    End Function
End Class