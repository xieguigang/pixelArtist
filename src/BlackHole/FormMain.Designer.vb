<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    ' Declared controls (accessed from FormMain.vb)
    Friend WithEvents picCanvas As System.Windows.Forms.PictureBox
    Friend WithEvents panelControls As System.Windows.Forms.Panel
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblModel As System.Windows.Forms.Label
    Friend WithEvents trkSpin As System.Windows.Forms.TrackBar
    Friend WithEvents lblSpin As System.Windows.Forms.Label
    Friend WithEvents chkDisk As System.Windows.Forms.CheckBox
    Friend WithEvents lblRes As System.Windows.Forms.Label
    Friend WithEvents trkRes As System.Windows.Forms.TrackBar
    Friend WithEvents lblResVal As System.Windows.Forms.Label
    Friend WithEvents lblBloom As System.Windows.Forms.Label
    Friend WithEvents trkBloom As System.Windows.Forms.TrackBar
    Friend WithEvents lblBloomVal As System.Windows.Forms.Label
    Friend WithEvents btnReset As System.Windows.Forms.Button
    Friend WithEvents lblHud As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
        Dim fontUI = New System.Drawing.Font("Segoe UI", 9.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Dim fontTitle = New System.Drawing.Font("Segoe UI", 15.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))

        Me.picCanvas = New System.Windows.Forms.PictureBox()
        Me.lblHud = New System.Windows.Forms.Label()
        Me.panelControls = New System.Windows.Forms.Panel()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.trkSpin = New System.Windows.Forms.TrackBar()
        Me.lblSpin = New System.Windows.Forms.Label()
        Me.chkDisk = New System.Windows.Forms.CheckBox()
        Me.lblRes = New System.Windows.Forms.Label()
        Me.trkRes = New System.Windows.Forms.TrackBar()
        Me.lblResVal = New System.Windows.Forms.Label()
        Me.lblBloom = New System.Windows.Forms.Label()
        Me.trkBloom = New System.Windows.Forms.TrackBar()
        Me.lblBloomVal = New System.Windows.Forms.Label()
        Me.btnReset = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()

        Me.panelControls.SuspendLayout()
        CType(Me.trkSpin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkRes, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkBloom, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()

        ' ---- picCanvas ----
        Me.picCanvas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picCanvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picCanvas.BackColor = System.Drawing.Color.Black
        Me.picCanvas.TabStop = True
        Me.picCanvas.Location = New System.Drawing.Point(0, 0)
        Me.picCanvas.Name = "picCanvas"
        Me.picCanvas.Size = New System.Drawing.Size(800, 420)
        Me.picCanvas.TabIndex = 0
        Me.picCanvas.Controls.Add(Me.lblHud)

        ' ---- lblHud (overlay on canvas, bottom-left) ----
        Me.lblHud.AutoSize = False
        Me.lblHud.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblHud.Height = 22
        Me.lblHud.Text = "FOV 60°  |  dist 14.0  |  a 0.000"
        Me.lblHud.ForeColor = System.Drawing.Color.FromArgb(54, 209, 220)
        Me.lblHud.BackColor = System.Drawing.Color.FromArgb(40, 5, 6, 10)
        Me.lblHud.Font = fontUI
        Me.lblHud.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblHud.Padding = New System.Windows.Forms.Padding(8, 0, 0, 0)

        ' ---- panelControls ----
        Me.panelControls.Dock = System.Windows.Forms.DockStyle.Right
        Me.panelControls.Width = 270
        Me.panelControls.BackColor = System.Drawing.Color.FromArgb(14, 17, 24)
        Me.panelControls.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.panelControls.Padding = New System.Windows.Forms.Padding(14, 12, 14, 12)
        Me.panelControls.Controls.Add(Me.btnReset)
        Me.panelControls.Controls.Add(Me.lblBloomVal)
        Me.panelControls.Controls.Add(Me.trkBloom)
        Me.panelControls.Controls.Add(Me.lblBloom)
        Me.panelControls.Controls.Add(Me.lblResVal)
        Me.panelControls.Controls.Add(Me.trkRes)
        Me.panelControls.Controls.Add(Me.lblRes)
        Me.panelControls.Controls.Add(Me.chkDisk)
        Me.panelControls.Controls.Add(Me.lblSpin)
        Me.panelControls.Controls.Add(Me.trkSpin)
        Me.panelControls.Controls.Add(Me.lblModel)
        Me.panelControls.Controls.Add(Me.lblTitle)

        ' ---- lblTitle ----
        Me.lblTitle.Text = "Black Hole Simulator"
        Me.lblTitle.ForeColor = System.Drawing.Color.FromArgb(255, 138, 30)
        Me.lblTitle.Font = fontTitle
        Me.lblTitle.Location = New System.Drawing.Point(14, 14)
        Me.lblTitle.Size = New System.Drawing.Size(242, 28)
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft

        ' ---- lblModel ----
        Me.lblModel.Text = "自旋参数 a (0 = 史瓦西 / 非旋转)"
        Me.lblModel.ForeColor = System.Drawing.Color.FromArgb(232, 236, 244)
        Me.lblModel.Font = fontUI
        Me.lblModel.Location = New System.Drawing.Point(14, 56)
        Me.lblModel.Size = New System.Drawing.Size(242, 18)

        ' ---- trkSpin ----
        Me.trkSpin.Location = New System.Drawing.Point(14, 76)
        Me.trkSpin.Size = New System.Drawing.Size(200, 28)
        Me.trkSpin.Minimum = 0
        Me.trkSpin.Maximum = 998
        Me.trkSpin.Value = 0
        Me.trkSpin.TickFrequency = 50
        Me.trkSpin.BackColor = System.Drawing.Color.FromArgb(14, 17, 24)
        Me.trkSpin.ForeColor = System.Drawing.Color.FromArgb(54, 209, 220)

        ' ---- lblSpin ----
        Me.lblSpin.Text = "0.000"
        Me.lblSpin.ForeColor = System.Drawing.Color.FromArgb(191, 216, 255)
        Me.lblSpin.Font = fontUI
        Me.lblSpin.Location = New System.Drawing.Point(218, 78)
        Me.lblSpin.Size = New System.Drawing.Size(40, 18)
        Me.lblSpin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft

        ' ---- chkDisk ----
        Me.chkDisk.Text = "吸积盘 Accretion Disk"
        Me.chkDisk.ForeColor = System.Drawing.Color.FromArgb(232, 236, 244)
        Me.chkDisk.Font = fontUI
        Me.chkDisk.Location = New System.Drawing.Point(14, 116)
        Me.chkDisk.Size = New System.Drawing.Size(242, 22)
        Me.chkDisk.Checked = True
        Me.chkDisk.BackColor = System.Drawing.Color.FromArgb(14, 17, 24)

        ' ---- lblRes ----
        Me.lblRes.Text = "渲染分辨率 Resolution"
        Me.lblRes.ForeColor = System.Drawing.Color.FromArgb(232, 236, 244)
        Me.lblRes.Font = fontUI
        Me.lblRes.Location = New System.Drawing.Point(14, 150)
        Me.lblRes.Size = New System.Drawing.Size(242, 18)

        ' ---- trkRes ----
        Me.trkRes.Location = New System.Drawing.Point(14, 170)
        Me.trkRes.Size = New System.Drawing.Size(200, 28)
        Me.trkRes.Minimum = 30
        Me.trkRes.Maximum = 100
        Me.trkRes.Value = 50
        Me.trkRes.TickFrequency = 10
        Me.trkRes.BackColor = System.Drawing.Color.FromArgb(14, 17, 24)
        Me.trkRes.ForeColor = System.Drawing.Color.FromArgb(54, 209, 220)

        ' ---- lblResVal ----
        Me.lblResVal.Text = "50%"
        Me.lblResVal.ForeColor = System.Drawing.Color.FromArgb(191, 216, 255)
        Me.lblResVal.Font = fontUI
        Me.lblResVal.Location = New System.Drawing.Point(218, 172)
        Me.lblResVal.Size = New System.Drawing.Size(40, 18)
        Me.lblResVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft

        ' ---- lblBloom ----
        Me.lblBloom.Text = "辉光强度 Bloom"
        Me.lblBloom.ForeColor = System.Drawing.Color.FromArgb(232, 236, 244)
        Me.lblBloom.Font = fontUI
        Me.lblBloom.Location = New System.Drawing.Point(14, 210)
        Me.lblBloom.Size = New System.Drawing.Size(242, 18)

        ' ---- trkBloom ----
        Me.trkBloom.Location = New System.Drawing.Point(14, 230)
        Me.trkBloom.Size = New System.Drawing.Size(200, 28)
        Me.trkBloom.Minimum = 0
        Me.trkBloom.Maximum = 150
        Me.trkBloom.Value = 70
        Me.trkBloom.TickFrequency = 10
        Me.trkBloom.BackColor = System.Drawing.Color.FromArgb(14, 17, 24)
        Me.trkBloom.ForeColor = System.Drawing.Color.FromArgb(54, 209, 220)

        ' ---- lblBloomVal ----
        Me.lblBloomVal.Text = "0.70"
        Me.lblBloomVal.ForeColor = System.Drawing.Color.FromArgb(191, 216, 255)
        Me.lblBloomVal.Font = fontUI
        Me.lblBloomVal.Location = New System.Drawing.Point(218, 232)
        Me.lblBloomVal.Size = New System.Drawing.Size(40, 18)
        Me.lblBloomVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft

        ' ---- btnReset ----
        Me.btnReset.Text = "重置视角 Reset View"
        Me.btnReset.ForeColor = System.Drawing.Color.FromArgb(5, 6, 10)
        Me.btnReset.BackColor = System.Drawing.Color.FromArgb(54, 209, 220)
        Me.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnReset.Font = New System.Drawing.Font("Segoe UI", 10.0F, System.Drawing.FontStyle.Bold)
        Me.btnReset.Location = New System.Drawing.Point(14, 280)
        Me.btnReset.Size = New System.Drawing.Size(242, 34)

        ' ---- lblStatus (bottom bar) ----
        Me.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblStatus.Height = 22
        Me.lblStatus.Text = "就绪"
        Me.lblStatus.ForeColor = System.Drawing.Color.FromArgb(138, 148, 166)
        Me.lblStatus.BackColor = System.Drawing.Color.FromArgb(10, 11, 18)
        Me.lblStatus.Font = fontUI
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblStatus.Padding = New System.Windows.Forms.Padding(8, 0, 0, 0)

        ' ---- FormMain ----
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(5, 6, 10)
        Me.ClientSize = New System.Drawing.Size(1070, 420)
        Me.Text = "Black Hole Simulator"
        Me.Controls.Add(Me.picCanvas)
        Me.Controls.Add(Me.panelControls)
        Me.Controls.Add(Me.lblStatus)

        Me.panelControls.ResumeLayout(False)
        Me.panelControls.PerformLayout()
        CType(Me.trkSpin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkRes, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkBloom, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

End Class
