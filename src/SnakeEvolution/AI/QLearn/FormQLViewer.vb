Imports System.ComponentModel
Imports System.Timers
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.QLearning
Imports Microsoft.VisualBasic.MachineLearning.QLearning.DataModel
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class FormQLViewer

    Public Table As QTable(Of GameControl)

    Dim WithEvents timer As New Timers.Timer

    Sub UpdateTableView()
        Try
            SyncLock Table.Table
                Dim views As String() = Table.Table.Values _
                    .Select(Function(x) x.ToString) _
                    .ToArray

                Call Me.Invoke(Sub() TextBox1.Text = views.JoinBy(vbCrLf))
            End SyncLock
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        timer.Interval = 500
        Call Threading.Thread.Sleep(2000)
        Call timer.Start()
    End Sub

    Private Sub timer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles timer.Elapsed
        Call UpdateTableView()
    End Sub

    <STAThreadAttribute>
    Private Sub SaveCurrentMatrixToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveCurrentMatrixToolStripMenuItem.Click
        Call New QModel(Table).GetJson(True).SaveTo(App.HOME & "/snake_QL_AI.json")
    End Sub

    Private Sub FormQLViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
    End Sub
End Class