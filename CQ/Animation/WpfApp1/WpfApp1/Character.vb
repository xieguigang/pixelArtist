﻿''' <summary>
''' Character model
''' </summary>
Public Class Character

    Dim animations As Dictionary(Of String, Animation)
    Dim canvas As Image
    Dim keys$()

    Dim playbackQueue As New Queue(Of String)
    Dim previous As Animation
    Dim run As Boolean = False

    Public Property name As String
    Public Property rand As Random

    Sub New(host As Grid, animationList As IEnumerable(Of Animation))
        canvas = New Image
        host.Children.Add(canvas)
        animations = animationList.ToDictionary(Function(a) a.Name)
        keys = animations.Keys.ToArray
    End Sub

    Public Sub Pending(key As String)
        If animations.ContainsKey(key) Then
            SyncLock playbackQueue
                playbackQueue.Enqueue(key)
            End SyncLock
        End If
    End Sub

    Public Sub [Stop]()
        run = False
    End Sub

    Public Sub Action()

    End Sub

    Public Sub PlayNext()
        Dim animation As Animation

        If playbackQueue.Count > 0 Then
            animation = animations(playbackQueue.Dequeue)
        Else
            SyncLock rand
                animation = animations(keys(rand.Next(0, keys.Length)))
            End SyncLock
        End If

        animation.PlayBack(canvas, previous)
        previous = animation
    End Sub
End Class

