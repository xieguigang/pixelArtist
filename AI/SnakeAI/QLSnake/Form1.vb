Imports System.Timers
Imports Microsoft.VisualBasic.DataMining.Framework.QLearning.DataModel
Imports Microsoft.VisualBasic.Linq

Public Class Form1

    Public Table As IQTable

    Dim WithEvents timer As New Timers.Timer

    Sub UpdateTableView()
        Dim views As String() = Table.Table.Values.ToArray(Function(x) x.ToString)
        Call Me.Invoke(Sub() TextBox1.Text = vbCrLf & views.JoinBy(vbCrLf))
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        timer.Interval = 500
        Call Threading.Thread.Sleep(2000)
        Call timer.Start()
    End Sub

    Private Sub timer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles timer.Elapsed
        Call UpdateTableView()
    End Sub
End Class