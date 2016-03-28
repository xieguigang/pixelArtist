Imports Microsoft.VisualBasic.GamePads.Abstract

Namespace EngineParts

    Public Class GraphicDevice : Inherits Abstract.EngineParts

        Protected Friend ReadOnly _list As New List(Of GraphicUnit)

        ''' <summary>
        ''' Graphics refresh rate, Hz.(图像的刷新频率，单位为Hz)
        ''' </summary>
        ''' <returns></returns>
        Public Property RefreshHz As Integer
            Get
                Return 1000 / _sleep
            End Get
            Set(value As Integer)
                _sleep = 1000 / value
            End Set
        End Property

        Friend _sleep As Integer

        ''' <summary>
        ''' The size of the graphics region
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property DeviceSize As Size
            Get
                SyncLock Engine._innerDevice
                    Return Engine._innerDevice.Size
                End SyncLock
            End Get
        End Property

        Sub New(engine As GameEngine)
            Call MyBase.New(engine)
        End Sub

        Public Sub Updates()
            Using g As GDIPlusDeviceHandle = DeviceSize.CreateGDIDevice
                Dim source As GraphicUnit()

                SyncLock _list
                    source = _list.ToArray
                End SyncLock

                For Each x As GraphicUnit In source
                    SyncLock x
                        Call x.Draw(g)
                    End SyncLock
                Next

                Engine._innerDevice.BackgroundImage = g.ImageResource
            End Using
        End Sub
    End Class
End Namespace