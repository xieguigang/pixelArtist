


Module Program

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine, AddressOf CLI.RunFresh)
    End Function
End Module

