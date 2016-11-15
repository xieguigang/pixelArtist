Imports Microsoft.VisualBasic.GamePads.Abstract

Namespace EngineParts

    ''' <summary>
    ''' Capture the input device actions, including mouse input and keyboard key pressing.
    ''' (捕捉动作的输入的设备，包括键盘和鼠标)
    ''' </summary>
    Public Class Controller : Inherits Abstract.EngineParts

        Dim WithEvents _innerDevice As DisplayPort
        Dim _actionCallback As Action(Of Controls, Char)
        Dim _clickObject As Action(Of Point, GraphicUnit)

        ''' <summary>
        ''' The gamepad input mappings, mapping the keyboard input as the internal actions.
        ''' (输入映射，将键盘按键映射为游戏内部的动作信号)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ControlMaps As ControlMaps = New ControlMaps
        Public Property Enable As Boolean = True

        Sub New(engine As GameEngine)
            Call MyBase.New(engine)

            _innerDevice = engine._innerDevice
            _actionCallback = AddressOf engine.Invoke
            _clickObject = AddressOf engine.ClickObject
        End Sub

        Private Sub _innerDevice_KeyPress(sender As Object, e As KeyPressEventArgs) Handles _innerDevice.KeyPress
            If Not Enable Then
                Return
            End If

            Dim key As Char = e.KeyChar
            Dim action = ControlMaps.GetMapAction(key)
            Call _actionCallback.BeginInvoke(action, key, Nothing, Nothing)
        End Sub

        Private Sub _innerDevice_MouseClick(sender As Object, e As MouseEventArgs) Handles _innerDevice.MouseClick
            If Not Enable Then
                Return
            End If

            Dim xy As Point = e.Location

            Call Engine _
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
End Namespace