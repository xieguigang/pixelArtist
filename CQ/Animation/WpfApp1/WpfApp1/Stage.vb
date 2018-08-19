Imports System.Threading

Public Class Stage

    Dim characters As New Dictionary(Of String, Character)
    Dim host As Grid
    Dim random As New Random

    Public Sub Add(name$, characterAnimations As IEnumerable(Of Animation))
        Dim character As New Character(host, characterAnimations) With {
            .name = name,
            .rand = random
        }

        SyncLock characters
            characters.Add(name, character)
        End SyncLock

        Call New Thread(AddressOf character.Action).Start()
    End Sub

    Public Sub Remove(name As String)
        SyncLock characters
            If characters.ContainsKey(name) Then
                characters(name).Stop()
                characters.Remove(name)
            End If
        End SyncLock
    End Sub
End Class
