Imports System

' TesterABC.java
' *
' * Runs ArtificialBeeColony.java and logs the results into a file using Writer.java.
' * ABC testing setup is according to pass/fail criteria
' * Pass criteria - 50 success
' * Fail criteria - 100 failures
' *
' * @author: James M. Bayon-on
' * @version: 1.3
' 

Public Class TesterABC
    Friend logWriter As Writer
    Friend abc As ArtificialBeeColony
    Friend MAX_RUN As Integer
    Friend MAX_LENGTH As Integer
    Friend runtimes As Long()

'     Instantiates the TesterABC class
'     *
'     
    Public Sub New()
        logWriter = New Writer()
        MAX_RUN = 50
        runtimes = New Long(MAX_RUN - 1){}
    End Sub

'     Test method accepts the N/max length, and parameters mutation rate and max epoch to set for the ABC accordingly.
'     *
'     * @param: max length/n
'     * @param: trial limit for ABC
'     * @param: max epoch for ABC
'     
    Public Overridable Sub test(ByVal maxLength As Integer, ByVal trialLimit As Integer, ByVal maxEpoch As Integer)
        MAX_LENGTH = maxLength
        abc = New ArtificialBeeColony(MAX_LENGTH) 'instantiate and define abc here
        abc.Limit = trialLimit
        abc.MaxEpoch = maxEpoch
        Dim testStart As Long = App.NanoTime()
        Dim filepath As String = "ABC-N" & MAX_LENGTH & "-" & trialLimit & "-" & maxEpoch & ".txt"
        Dim startTime As Long = 0
        Dim endTime As Long = 0
        Dim totalTime As Long = 0
        Dim fail As Integer = 0
        Dim success As Integer = 0

        logParameters()

        Dim i As Integer = 0
        Do While i < MAX_RUN 'run 50 sucess to pass passing criteria
            startTime = Now.Ticks
            If abc.algorithm() Then
                endTime = Now.Ticks
                totalTime = endTime - startTime

                Console.WriteLine("Done")
                Console.WriteLine("run " & (i+1))
                Console.WriteLine("time in nanoseconds: " & totalTime)
                Console.WriteLine("Success!")

                runtimes(i) = totalTime
                i += 1
                success += 1

                'write to log
                logWriter.add(CStr("Run: " & i))
                logWriter.add(CStr("Runtime in nanoseconds: " & totalTime))
                logWriter.add(CStr("Found at epoch: " & abc.Epoch))
                logWriter.add(CStr("Population size: " & abc.PopSize))
                logWriter.add("")

                For Each h As Honey In abc.Solutions 'write solutions to log file
                    logWriter.add(h)
                    logWriter.add("")
                Next 'count failures for failing criteria
            Else
                fail += 1
                Console.WriteLine("Fail!")
            End If

            If fail >= 100 Then
                Console.WriteLine("Cannot find solution with these params")
                Exit Do
            End If
            startTime = 0 'reset time
            endTime = 0
            totalTime = 0
        Loop

        Console.WriteLine("Number of Success: " & success)
        Console.WriteLine("Number of failures: " & fail)
        logWriter.add("Runtime summary")
        logWriter.add("")

        For x As Integer = 0 To runtimes.Length - 1 'print runtime summary
            logWriter.add(Convert.ToString(runtimes(x)))
        Next

        Dim testEnd As Long = Now.Ticks
        logWriter.add(Convert.ToString(testStart))
        logWriter.add(Convert.ToString(testEnd))
        logWriter.add(Convert.ToString(testEnd - testStart))


        logWriter.writeFile(filepath)
        printRuntimes()
    End Sub

'     Converts the parameters of ABC to string and adds it to the string list in the writer class
'     *
'     
    Public Overridable Sub logParameters()
        logWriter.add("Artificial Bee Colony Algorithm")
        logWriter.add("Parameters")
        logWriter.add(CStr("MAX_LENGTH/N: " & MAX_LENGTH))
        logWriter.add(CStr("STARTING_POPULATION: " & abc.StartSize))
        logWriter.add(CStr("MAX_EPOCHS: " & abc.MaxEpoch))
        logWriter.add(CStr("FOOD_NUMBER: " & abc.FoodNum))
        logWriter.add(CStr("TRIAL_LIMIT: " & abc.Limit))
        logWriter.add(CStr("MINIMUM_SHUFFLES: " & abc.ShuffleMin))
        logWriter.add(CStr("MAXIMUM_SHUFFLES: " & abc.ShuffleMax))
        logWriter.add("")
    End Sub

'     Prints the runtime summary in the console
'     *
'     
    Public Overridable Sub printRuntimes()
        For Each x As Long In runtimes
            Console.WriteLine("run with time " & x & " nanoseconds")
        Next
    End Sub

    Public Shared Sub Main(ByVal args As String())
        Call New TesterABC().test(4, 50, 1000)
        Call New TesterABC().test(8, 50, 1000)
        Call New TesterABC().test(12, 50, 1000)
        Call New TesterABC().test(16, 50, 1000)
        Call New TesterABC().test(20, 50, 1000)
    End Sub
End Class
