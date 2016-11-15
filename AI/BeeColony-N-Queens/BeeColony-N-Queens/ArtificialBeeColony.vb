Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

' ArtificialBeeColony.java
' *
' * Solves the N-Queens puzzle using Artificial Bee Colony Algorithm.
' * Code inspired by the java code for abc algorith at artificial bee colony's website
' * Found at http://mf.erciyes.edu.tr/abc/.
' *
' * Special thanks to Professor Bahriye Basturk Akay for pointing me to abc's website for the source code
' * http://mf.erciyes.edu.tr/abc/software.htm
' *
' * @author: James M. Bayon-on
' * @version: 1.0
' 

Public Class ArtificialBeeColony
    'ABC PARAMETERS
    Public MAX_LENGTH As Integer 'The number of parameters of the problem to be optimized
    Public NP As Integer 'The number of total bees/colony size. employed + onlookers
    Public FOOD_NUMBER As Integer 'The number of food sources equals the half of the colony size
    Public LIMIT As Integer 'A food source which could not be improved through "limit" trials is abandoned by its employed bee
    Public MAX_EPOCH As Integer 'The number of cycles for foraging {a stopping criteria}
    Public MIN_SHUFFLE As Integer
    Public MAX_SHUFFLE As Integer

    Public rand As Random
    Public foodSources As List(Of Honey)
    Public solutions As List(Of Honey)
    Public gBest As Honey
    Public epoch As Integer


    '      sets the max epoch
    '     *
    '     * @return: new max epoch value
    Public Overridable Property MaxEpoch As Integer
        Set(ByVal newMaxEpoch As Integer)
            Me.MAX_EPOCH = newMaxEpoch
        End Set
        Get
            ' 	 gets the max epoch
            ' 	 *
            ' 	 * @return: max epoch
            ' 	  
            Return MAX_EPOCH
        End Get
    End Property

    ' 	 gets the population size
    ' 	 *
    ' 	 * @return: pop size
    ' 	  
    Public Overridable ReadOnly Property PopSize As Integer
        Get
            Return foodSources.Count
        End Get
    End Property

    ' 	 gets the start size
    ' 	 *
    ' 	 * @return: start size
    ' 	  
    Public Overridable ReadOnly Property StartSize As Integer
        Get
            Return NP
        End Get
    End Property

    ' 	 gets the number of food
    ' 	 *
    ' 	 * @return: food number
    ' 	  
    Public Overridable ReadOnly Property FoodNum As Double
        Get
            Return FOOD_NUMBER
        End Get
    End Property

    '     sets the limit for trials for all food sources
    '     *
    '     * @param: new trial limit
    '         



    ' 	 gets the min shuffle
    ' 	 *
    ' 	 * @return: min shuffle
    ' 	  
    Public Overridable ReadOnly Property ShuffleMin As Integer
        Get
            Return MIN_SHUFFLE
        End Get
    End Property

    ' 	 gets the max shuffle
    ' 	 *
    ' 	 * @return: max shuffle
    ' 	  
    Public Overridable ReadOnly Property ShuffleMax As Integer
        Get
            Return MAX_SHUFFLE
        End Get
    End Property

    '     Instantiates the artificial bee colony algorithm along with its parameters.
    '	 *
    '	 * @param: size of n queens
    '	 
    Public Sub New(ByVal n As Integer)
        MAX_LENGTH = n
        NP = 40 'pop size 20 to 40 or even 100
        FOOD_NUMBER = NP\2
        LIMIT = 50
        MAX_EPOCH = 1000
        MIN_SHUFFLE = 8
        MAX_SHUFFLE = 20
        gBest = Nothing
        epoch = 0
    End Sub

'     Starts the particle swarm optimization algorithm solving for n queens.
'	 *
'	 
    Public Overridable Function algorithm() As Boolean
        foodSources = New List(Of Honey)()
        solutions = New List(Of Honey)()
        rand = New Random()
        Dim done As Boolean = False
        epoch = 0

        initialize()
        memorizeBestFoodSource()

        Do While Not done
            If epoch < MAX_EPOCH Then
                If gBest.Conflicts = 0 Then
                    done = True
                End If
                sendEmployedBees()
                getFitness()
                calculateProbabilities()
                sendOnlookerBees()
                memorizeBestFoodSource()
                sendScoutBees()

                epoch += 1
                ' This is here simply to show the runtime status.
                Console.WriteLine("Epoch: " & epoch)
            Else
                done = True
            End If

        Loop

        If epoch = MAX_EPOCH Then
            Console.WriteLine("No Solution found")
            done = False
        End If

        Console.WriteLine("done.")
        Console.WriteLine("Completed " & epoch & " epochs.")

        For Each h As Honey In foodSources
            If h.Conflicts = 0 Then
                Console.WriteLine("SOLUTION")
                solutions.Add(h)
                printSolution(h)
                Console.WriteLine("conflicts:" & h.Conflicts)
            End If
        Next

        Return done
    End Function

'     Sends the employed bees to optimize the solution
'	 *
'	 
    Public Overridable Sub sendEmployedBees()
        Dim neighborBeeIndex As Integer = 0
        Dim currentBee As Honey = Nothing
        Dim neighborBee As Honey = Nothing

        For i As Integer = 0 To FOOD_NUMBER - 1
            'A randomly chosen solution is used in producing a mutant solution of the solution i
            'neighborBee = getRandomNumber(0, Food_Number-1);
            neighborBeeIndex = getExclusiveRandomNumber(FOOD_NUMBER-1, i)
            currentBee = foodSources(i)
            neighborBee = foodSources(neighborBeeIndex)
            sendToWork(currentBee, neighborBee)
        Next
    End Sub

'     Sends the onlooker bees to optimize the solution. Onlooker bees work on the best solutions from the employed bees. best solutions have high selection probability. 
'	 *
'	 
    Public Overridable Sub sendOnlookerBees()
        Dim i As Integer = 0
        Dim t As Integer = 0
        Dim neighborBeeIndex As Integer = 0
        Dim currentBee As Honey = Nothing
        Dim neighborBee As Honey = Nothing

        Do While t < FOOD_NUMBER
            currentBee = foodSources(i)
            If rand.NextDouble() < currentBee.SelectionProbability Then
                t += 1
                neighborBeeIndex = getExclusiveRandomNumber(FOOD_NUMBER-1, i)
                neighborBee = foodSources(neighborBeeIndex)
                sendToWork(currentBee, neighborBee)
            End If
            i += 1
            If i = FOOD_NUMBER Then
                i = 0
            End If
        Loop
    End Sub

'	 The optimization part of the algorithm. improves the currentbee by choosing a random neighbor bee. the changes is a randomly generated number of times to try and improve the current solution.
'	 *
'	 * @param: the currently selected bee
'	 * @param: a randomly selected neighbor bee
'	 * @param: the number of times to try and improve the solution
'	 
    Public Overridable Sub sendToWork(ByVal currentBee As Honey, ByVal neighborBee As Honey)
        Dim newValue As Integer = 0
        Dim tempValue As Integer = 0
        Dim tempIndex As Integer = 0
        Dim prevConflicts As Integer = 0
        Dim currConflicts As Integer = 0
        Dim parameterToChange As Integer = 0

        'get number of conflicts
        prevConflicts = currentBee.Conflicts

        'The parameter to be changed is determined randomly
        parameterToChange = getRandomNumber(0, MAX_LENGTH-1)

'        v_{ij}=x_{ij}+\phi_{ij}*(x_{kj}-x_{ij}) 
'        solution[param2change]=Foods[i][param2change]+(Foods[i][param2change]-Foods[neighbour][param2change])*(r-0.5)*2;
'        
        tempValue = currentBee.getNectar(parameterToChange)
        newValue = CInt(Fix(tempValue+(tempValue - neighborBee.getNectar(parameterToChange))*(rand.NextDouble()-0.5)*2))

        'trap the value within upper bound and lower bound limits
        If newValue < 0 Then
            newValue = 0
        End If
        If newValue > MAX_LENGTH-1 Then
            newValue = MAX_LENGTH-1
        End If

        'get the index of the new value
        tempIndex = currentBee.getIndex(newValue)

        'swap
        currentBee.setNectar(parameterToChange, newValue)
        currentBee.setNectar(tempIndex, tempValue)
        currentBee.computeConflicts()
        currConflicts = currentBee.Conflicts

        'greedy selection
        If prevConflicts < currConflicts Then 'No improvement
            currentBee.setNectar(parameterToChange, tempValue)
            currentBee.setNectar(tempIndex, newValue)
            currentBee.computeConflicts()
            currentBee.Trials = currentBee.Trials + 1 'improved solution
        Else
            currentBee.Trials = 0
        End If

    End Sub

'     Finds food sources which have been abandoned/reached the limit.
'     * Scout bees will generate a totally random solution from the existing and it will also reset its trials back to zero.
'     *
'     
    Public Overridable Sub sendScoutBees()
        Dim currentBee As Honey = Nothing
        Dim shuffles As Integer = 0

        For i As Integer = 0 To FOOD_NUMBER - 1
            currentBee = foodSources(i)
            If currentBee.Trials >= LIMIT Then
                shuffles = getRandomNumber(MIN_SHUFFLE, MAX_SHUFFLE)
                For j As Integer = 0 To shuffles - 1
                    randomlyArrange(i)
                Next
                currentBee.computeConflicts()
                currentBee.Trials = 0

            End If
        Next
    End Sub

'	 Sets the fitness of each solution based on its conflicts
'	 *
'	 
    Public Overridable Sub getFitness()
        ' Lowest errors = 100%, Highest errors = 0%
        Dim thisFood As Honey = Nothing
        Dim bestScore As Double = 0.0
        Dim worstScore As Double = 0.0

        ' The worst score would be the one with the highest energy, best would be lowest.
        worstScore = foodSources.Max.Conflicts

        ' Convert to a weighted percentage.
        bestScore = worstScore - foodSources.Min.Conflicts

        For i As Integer = 0 To FOOD_NUMBER - 1
            thisFood = foodSources(i)
            thisFood.Fitness = (worstScore - thisFood.Conflicts) * 100.0 / bestScore
        Next
    End Sub

'     Sets the selection probability of each solution. the higher the fitness the greater the probability 
'	 *
'	  
    Public Overridable Sub calculateProbabilities()
        Dim thisFood As Honey = Nothing
        Dim maxfit As Double = foodSources(0).Fitness

        For i As Integer = 1 To FOOD_NUMBER - 1
            thisFood = foodSources(i)
            If thisFood.Fitness > maxfit Then
                maxfit = thisFood.Fitness
            End If
        Next

        For j As Integer = 0 To FOOD_NUMBER - 1
            thisFood = foodSources(j)
            thisFood.SelectionProbability = (0.9*(thisFood.Fitness/maxfit))+0.1
        Next
    End Sub

'     Initializes all of the solutions' placement of queens in ramdom positions.
'	 *
'	  
    Public Overridable Sub initialize()
        Dim newFoodIndex As Integer = 0
        Dim shuffles As Integer = 0

        For i As Integer = 0 To FOOD_NUMBER - 1
            Dim newHoney As New Honey(MAX_LENGTH)

            foodSources.Add(newHoney)
            newFoodIndex = foodSources.IndexOf(newHoney)

            shuffles = getRandomNumber(MIN_SHUFFLE, MAX_SHUFFLE)

            For j As Integer = 0 To shuffles - 1
                randomlyArrange(newFoodIndex)
            Next

            foodSources(newFoodIndex).computeConflicts()
        Next ' i
    End Sub

'     Gets a random number in the range of the parameters
'	 *
'	 * @param: the minimum random number
'	 * @param: the maximum random number
'	 * @return: random number
'	  
    Public Overridable Function getRandomNumber(ByVal low As Integer, ByVal high As Integer) As Integer
        Return CInt(Fix(Math.Round((high - low) * rand.NextDouble() + low)))
    End Function

'     Gets a random number with the exception of the parameter
'	 *
'	 * @param: the maximum random number
'	 * @param: number to to be chosen
'	 * @return: random number
'	  
    Public Overridable Function getExclusiveRandomNumber(ByVal high As Integer, ByVal except As Integer) As Integer
        Dim done As Boolean = False
        Dim getRand As Integer = 0

        Do While Not done
            getRand = rand.Next(high)
            If getRand <> except Then
                done = True
            End If
        Loop

        Return getRand
    End Function

'     Changes a position of the queens in a particle by swapping a randomly selected position
'	 *
'	 * @param: index of the solution
'	  
    Public Overridable Sub randomlyArrange(ByVal index As Integer)
        Dim positionA As Integer = getRandomNumber(0, MAX_LENGTH - 1)
        Dim positionB As Integer = getExclusiveRandomNumber(MAX_LENGTH - 1, positionA)
        Dim thisHoney As Honey = foodSources(index)
        Dim temp As Integer = thisHoney.getNectar(positionA)
        thisHoney.setNectar(positionA, thisHoney.getNectar(positionB))
        thisHoney.setNectar(positionB, temp)
    End Sub

'     Memorizes the best solution
'	 *
'	  
    Public Overridable Sub memorizeBestFoodSource()
        gBest = foodSources.Min
    End Sub

    '     Prints the nxn board with the queens
    '	 *
    '	 * @param: a chromosome
    '	  
    Public Overridable Sub printSolution(ByVal solution As Honey)
        Dim board As String()() = MAT(Of String)(MAX_LENGTH, MAX_LENGTH)

        ' Clear the board.
        For x As Integer = 0 To MAX_LENGTH - 1
            For y As Integer = 0 To MAX_LENGTH - 1
                board(x)(y) = ""
            Next
        Next

        For x As Integer = 0 To MAX_LENGTH - 1
            board(x)(solution.getNectar(x)) = "Q"
        Next

        ' Display the board.
        Console.WriteLine("Board:")
        For y As Integer = 0 To MAX_LENGTH - 1
            For x As Integer = 0 To MAX_LENGTH - 1
                If board(x)(y) = "Q" Then
                    Console.Write("Q ")
                Else
                    Console.Write(". ")
                End If
            Next
            Console.Write(vbLf)
        Next
    End Sub
End Class