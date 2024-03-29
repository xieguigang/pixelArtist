﻿''' <summary>
''' Capture the input device actions, including mouse input and keyboard key pressing.
''' (捕捉动作的输入的设备，包括键盘和鼠标)
''' </summary>
Public Class Controller

    ''' <summary>
    ''' The gamepad input mappings, mapping the keyboard input as the internal actions.
    ''' (输入映射，将键盘按键映射为游戏内部的动作信号)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ControlMaps As ControlMaps
    Public Property Enable As Boolean = True

    Dim engine As WorldEngine

    Sub New(engine As WorldEngine)
        Me.engine = engine
        Me.ControlMaps = ControlMaps.DefaultMaps
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

        For Each x As CharacterModel In engine
            If x.GetPixels(pixel).Any(Function(r) r.Contains(xy)) Then
                Call engine.clicks(xy, x)
            End If
        Next
    End Sub
End Class