' ============================================================
' Program.vb - 程序入口
' ============================================================

Imports System.Windows.Forms

Public Module Program

    <STAThread>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New MainForm())
    End Sub

End Module
