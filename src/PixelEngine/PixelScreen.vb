Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class PixelScreen : Implements IDisposable

    Public Property Resolution As Size = New Size(300, 400)

    Dim canvas As Action(Of PixelGraphics)
    Dim keyQueue As Char
    Dim mouseQueue As Point

    Public ReadOnly Property GetCommand As Char
        Get
            Dim buf = keyQueue
            keyQueue = Nothing
            Return buf
        End Get
    End Property

    Public ReadOnly Property GetClick As Point
        Get
            Dim buf = mouseQueue
            mouseQueue = Nothing
            Return buf
        End Get
    End Property

    Public ReadOnly Property Random As Point
        Get
            Return New Point(randf.NextInteger(Resolution.Width), randf.NextInteger(Resolution.Height))
        End Get
    End Property

    Public ReadOnly Property PixelSize As SizeF
        Get
            Dim size As Size = Me.Size

            Return New SizeF(
                width:=size.Width / Resolution.Width,
                height:=size.Height / Resolution.Height
            )
        End Get
    End Property

    Public Sub RequestFrame(paint As Action(Of PixelGraphics))
        Me.canvas = paint
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        If Not canvas Is Nothing Then
            Dim dev As PixelGraphics = PixelGraphics.FromGdiDevice(e.Graphics, Size)

            Call dev.SetScreenResolution(Resolution)
            Call dev.Clear(BackColor)
            Call canvas(dev)
        End If

        MyBase.OnPaint(e)
    End Sub

    Private Sub PixelScreen_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
    End Sub

    Public Sub CallKeyPress(key As Char)
        keyQueue = key
    End Sub

    Public Sub CallKeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        keyQueue = e.KeyChar
    End Sub

    Private Sub PixelScreen_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        mouseQueue = e.Location
    End Sub
End Class
