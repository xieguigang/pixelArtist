Public Class Form1
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.


        Dim play As New libZPlay.App.MediaPlayer

        Call play.PlayBack("E:\116. 宇多田光Beautiful World.flac")

        Me.BackgroundImage = play.AlbumArt

        Call play.Play()

    End Sub
End Class
