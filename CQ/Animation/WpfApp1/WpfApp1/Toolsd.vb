Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports WpfBitmap = System.Windows.Media.Imaging.BitmapImage

Module Toolsd

    <Extension>
    Public Function WpfBitmap(resource As MemoryStream) As WpfBitmap
        Dim bitmap = New WpfBitmap()
        bitmap.BeginInit()
        bitmap.StreamSource = resource
        bitmap.CacheOption = BitmapCacheOption.OnLoad
        bitmap.EndInit()
        bitmap.Freeze()

        Return bitmap
    End Function

    Public Function OffsetRenderTest(previous As WpfBitmap, [next] As WpfBitmap) As WpfBitmap
        Dim canvas = New MemoryStream(My.Resources.blank.GetStreamBuffer)
    End Function

End Module
