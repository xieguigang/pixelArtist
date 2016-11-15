Imports libZPlay.App
Imports libZPlay.InternalTypes
Imports Microsoft.VisualBasic.Serialization

Public Class Form1
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.


        Dim play As New libZPlay.App.MediaPlayer

        Call play.PlayBack("E:\116. 宇多田光Beautiful World.flac")

        Me.BackgroundImage = play.AlbumArt

        ticks = play.Play()

    End Sub

    Private Sub ticks_Tick(sender As MediaPlayer, cur As TStreamTime, p As Double) Handles ticks.Tick
        Call Me.Invoke(Sub() Text = $"[{Math.Round(p * 100, 2)}%] " & cur.GetJson)
    End Sub

    Dim WithEvents ticks As TickEvent
End Class
