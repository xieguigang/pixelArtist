Imports System

' Honey.java
' *
' * Honey class used by ArtificialBeeColony.java
' * Also known as food source.
' * Contains the positions of the queens in a solution as well as its conflicts, trials, fitness and selection probability. 
' * Base code at http://mf.erciyes.edu.tr/abc/
' *
' * @author: James M. Bayon-on
' * @version: 1.3
' 

Public Class Honey
    Implements IComparable(Of Honey)

    Dim MAX_LENGTH As Integer
    ''' <summary>
    ''' solution or placement of queens
    ''' </summary>
    Dim nectar As Integer()

    '     Gets the conflicts of the Honey.
    '	 *
    '	 * @return: number of conflicts of the honey
    '	 
    Public Overridable Property Conflicts As Integer

    '     Sets the conflicts of the honey.
    '	 *
    '	 * @param: new number of conflicts
    '	 

    '     Gets the selection probability of the honey.
    '	 *
    '	 * @return: selection probability of the honey
    '	 
    Public Overridable Property SelectionProbability As Double

    '     sets the selection probability of the honey.
    '	 *
    '	 * @param: new selection probability of the honey
    '	 

    '     Gets the fitness of a honey.
    '	 *
    '	 * @return: fitness of honey
    '	 
    Public Overridable Property Fitness As Double
    '	 Gets the number of trials of a solution.
    '	 *
    '	 * @return: number of trials
    '	 
    Public Overridable Property Trials As Integer

    '     Sets the number of trials of a solution.
    '	 *
    '	 * @param: new number of trials
    '	 

    '     Gets the max length.
    '	 *
    '	 * @return: max length
    '	 
    Public Overridable ReadOnly Property MaxLength As Integer
        Get
            Return MAX_LENGTH
        End Get
    End Property

    '     Instantiate a Honey.
    '     *
    '     * @param: size of n
    '     
    Public Sub New(ByVal size As Integer)
        Me.MAX_LENGTH = size
        Me.nectar = New Integer(MAX_LENGTH - 1){}
        Me.conflicts = 0
        Me.trials = 0
        Me.fitness = 0.0
        Me.selectionProbability = 0.0
        initNectar()
    End Sub

    '     Compares two Honeys.
    '	 *
    '	 * @param: a Honey to compare with
    '	 	
    Public Overridable Function compareTo(ByVal h As Honey) As Integer Implements IComparable(Of Honey).CompareTo
        Return Me.Conflicts - h.Conflicts
    End Function

    '     Initializes the Honey into diagonal queens.
    '	 *
    '	 
    Public Overridable Sub initNectar()
        For i As Integer = 0 To MAX_LENGTH - 1 'initialize the solution to 1... n
            nectar(i) = i
        Next
    End Sub

'     Computes the conflicts in the nxn board.
'	 *
'	 
    Public Overridable Sub computeConflicts() 'compute the number of conflicts to calculate fitness
        Dim board As String()() = MAT(Of String)(MAX_LENGTH, MAX_LENGTH)
        Dim x As Integer = 0 'row
        Dim y As Integer = 0 'column
        Dim tempx As Integer = 0
        Dim tempy As Integer = 0

        Dim dx As Integer() = New Integer() {-1, 1, -1, 1} 'to check for diagonal
        Dim dy As Integer() = New Integer() {-1, 1, 1, -1} 'paired with dx to check for diagonal

        Dim done As Boolean = False 'used to check is checking fo diagonal is out of bounds
        Dim conflicts__ As Integer = 0 'number of conflicts found

        board = clearBoard(board) 'clears the board into empty strings
        board = plotQueens(board) 'plots the Q in the board

        ' Walk through each of the Queens and compute the number of conflicts.
        For i As Integer = 0 To MAX_LENGTH - 1
            x = i
            y = Me.nectar(i) 'will result to no horizontal and vertical conflicts because it will result to diagonal

            ' Check diagonals.
            For j As Integer = 0 To 3 'because of dx and dy where there are 4 directions for diagonal searching for conflicts
                tempx = x
                tempy = y ' store coordinate in temp
                done = False

                Do While Not done
                    tempx += dx(j)
                    tempy += dy(j)

                    If (tempx < 0 OrElse tempx >= MAX_LENGTH) OrElse (tempy < 0 OrElse tempy >= MAX_LENGTH) Then 'if exceeds board
                        done = True
                    Else
                        If board(tempx)(tempy).Equals("Q") Then
                            conflicts__ += 1
                        End If
                    End If
                Loop
            Next
        Next

        Me.conflicts = conflicts__ 'set conflicts of this chromosome

    End Sub

'     Plots the queens in the board.
'	 *
'	 * @param: a nxn board
'	 
    Public Overridable Function plotQueens(ByVal board As String()()) As String()()
        For i As Integer = 0 To MAX_LENGTH - 1
            board(i)(Me.nectar(i)) = "Q"
        Next
        Return board
    End Function

'     Clears the board.
'	 *
'	 * @param: a nxn board
'	 
    Public Overridable Function clearBoard(ByVal board As String()()) As String()()
        ' Clear the board.
        For i As Integer = 0 To MAX_LENGTH - 1
            For j As Integer = 0 To MAX_LENGTH - 1
                board(i)(j) = ""
            Next
        Next
        Return board
    End Function

    '     Sets the fitness of the honey.
    '	 *
    '	 * @param: new fitness
    '	 

    '     Gets the data on a specified index.
    '	 *
    '	 * @param: index of data
    '	 * @return: position of queen
    '	 
    Public Overridable Function getNectar(ByVal index As Integer) As Integer
        Return nectar(index)
    End Function

'     Gets the index on a specified data.
'	 *
'	 * @param: index of data
'	 * @return: position of queen
'	 
    Public Overridable Function getIndex(ByVal value As Integer) As Integer
        Dim k As Integer = 0
        Do While k < MAX_LENGTH
            If nectar(k) = value Then
                Exit Do
            End If
            k += 1
        Loop
        Return k
    End Function

    '     Sets the data on a specified index.
    '	 *
    '	 * @param: index of data
    '	 * @param: new position of queen
    '	 
    Public Overridable Sub setNectar(ByVal index As Integer, ByVal value As Integer)
        Me.nectar(index) = value
    End Sub
End Class
