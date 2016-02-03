Namespace Abstract

    Public MustInherit Class EngineParts

        Public ReadOnly Property Engine As GameEngine

        Sub New(engine As GameEngine)
            Me.Engine = engine
        End Sub
    End Class
End Namespace