'-------------------------------------------------------------------------
'Copyright (C) 2005 - 2008 Maxim Integrated Products, All Rights Reserved.
'
'Permission is hereby granted, free of charge, to any person obtaining a
'copy of this software and associated documentation files (the "Software"),
'to deal in the Software without restriction, including without limitation
'the rights to use, copy, modify, merge, publish, distribute, sublicense,
'and/or sell copies of the Software, and to permit persons to whom the
'Software is furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included
'in all copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
'OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
'MERCHANTABILITY,  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
'IN NO EVENT SHALL MAXIM INTEGRATED PRODUCTS BE LIABLE FOR ANY CLAIM, DAMAGES
'OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
'ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
'OTHER DEALINGS IN THE SOFTWARE.
'
'Except as contained in this notice, the name of Maxim Integrated Products
'shall not be used except as stated in the Maxim Integrated Products
'Branding Policy.
'

Public Class Form1
    Inherits System.Windows.Forms.Form
    Dim adapter As Object
    Dim mc As com.dalsemi.onewire.container.MissionContainer
    Dim owd As com.dalsemi.onewire.container.OneWireContainer
    Dim MinutesDelay As Long




#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents RolloverCheck As System.Windows.Forms.CheckBox
    Friend WithEvents TemperatureLogCheck As System.Windows.Forms.CheckBox
    Friend WithEvents HumidityLogCheck As System.Windows.Forms.CheckBox
    Friend WithEvents TempLowResRadio As System.Windows.Forms.RadioButton
    Friend WithEvents TempHighResRadio As System.Windows.Forms.RadioButton
    Friend WithEvents HumidityGroup As System.Windows.Forms.GroupBox
    Friend WithEvents HumidityHighResRadio As System.Windows.Forms.RadioButton
    Friend WithEvents HumidityLowResRadio As System.Windows.Forms.RadioButton
    Friend WithEvents CalculateTimer As System.Windows.Forms.Timer
    Friend WithEvents Label As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents DelayLabel As System.Windows.Forms.Label
    Friend WithEvents DisplayTimeDateLabel As System.Windows.Forms.Label
    Friend WithEvents StartMissionButton As System.Windows.Forms.Button
    Friend WithEvents DeviceROMLabel As System.Windows.Forms.Label
    Friend WithEvents DeviceTypeLabel As System.Windows.Forms.Label
    Friend WithEvents StatusBar As System.Windows.Forms.StatusBar
    Friend WithEvents StatusAdapter As System.Windows.Forms.StatusBarPanel
    Friend WithEvents StatusPort As System.Windows.Forms.StatusBarPanel
    Friend WithEvents StatusMsg As System.Windows.Forms.StatusBarPanel
    Friend WithEvents TemperatureGroup As System.Windows.Forms.GroupBox
    Friend WithEvents StopMissionButton As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents MinuteCombo As System.Windows.Forms.ComboBox
    Friend WithEvents HourCombo As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents DatePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents SampleRateBox As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents MinuteLabel As System.Windows.Forms.Label
    Friend WithEvents TemperatureLogLabel As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.components = New System.ComponentModel.Container
      Me.RolloverCheck = New System.Windows.Forms.CheckBox
      Me.TemperatureLogCheck = New System.Windows.Forms.CheckBox
      Me.HumidityLogCheck = New System.Windows.Forms.CheckBox
      Me.TemperatureGroup = New System.Windows.Forms.GroupBox
      Me.TempHighResRadio = New System.Windows.Forms.RadioButton
      Me.TempLowResRadio = New System.Windows.Forms.RadioButton
      Me.HumidityGroup = New System.Windows.Forms.GroupBox
      Me.HumidityHighResRadio = New System.Windows.Forms.RadioButton
      Me.HumidityLowResRadio = New System.Windows.Forms.RadioButton
      Me.Label = New System.Windows.Forms.Label
      Me.DelayLabel = New System.Windows.Forms.Label
      Me.CalculateTimer = New System.Windows.Forms.Timer(Me.components)
      Me.Label5 = New System.Windows.Forms.Label
      Me.DisplayTimeDateLabel = New System.Windows.Forms.Label
      Me.MinuteLabel = New System.Windows.Forms.Label
      Me.StartMissionButton = New System.Windows.Forms.Button
      Me.DeviceROMLabel = New System.Windows.Forms.Label
      Me.DeviceTypeLabel = New System.Windows.Forms.Label
      Me.StatusBar = New System.Windows.Forms.StatusBar
      Me.StatusAdapter = New System.Windows.Forms.StatusBarPanel
      Me.StatusPort = New System.Windows.Forms.StatusBarPanel
      Me.StatusMsg = New System.Windows.Forms.StatusBarPanel
      Me.StopMissionButton = New System.Windows.Forms.Button
      Me.GroupBox1 = New System.Windows.Forms.GroupBox
      Me.Label4 = New System.Windows.Forms.Label
      Me.Label3 = New System.Windows.Forms.Label
      Me.MinuteCombo = New System.Windows.Forms.ComboBox
      Me.HourCombo = New System.Windows.Forms.ComboBox
      Me.Label2 = New System.Windows.Forms.Label
      Me.DatePicker = New System.Windows.Forms.DateTimePicker
      Me.Label1 = New System.Windows.Forms.Label
      Me.SampleRateBox = New System.Windows.Forms.TextBox
      Me.Label6 = New System.Windows.Forms.Label
      Me.Label8 = New System.Windows.Forms.Label
      Me.TemperatureLogLabel = New System.Windows.Forms.Label
      Me.TemperatureGroup.SuspendLayout()
      Me.HumidityGroup.SuspendLayout()
      CType(Me.StatusAdapter, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.StatusPort, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.StatusMsg, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.GroupBox1.SuspendLayout()
      Me.SuspendLayout()
      '
      'RolloverCheck
      '
      Me.RolloverCheck.Checked = True
      Me.RolloverCheck.CheckState = System.Windows.Forms.CheckState.Checked
      Me.RolloverCheck.Location = New System.Drawing.Point(48, 40)
      Me.RolloverCheck.Name = "RolloverCheck"
      Me.RolloverCheck.Size = New System.Drawing.Size(152, 16)
      Me.RolloverCheck.TabIndex = 0
      Me.RolloverCheck.Text = "Rollover Enabled"
      Me.RolloverCheck.Visible = False
      '
      'TemperatureLogCheck
      '
      Me.TemperatureLogCheck.Checked = True
      Me.TemperatureLogCheck.CheckState = System.Windows.Forms.CheckState.Checked
      Me.TemperatureLogCheck.Location = New System.Drawing.Point(48, 56)
      Me.TemperatureLogCheck.Name = "TemperatureLogCheck"
      Me.TemperatureLogCheck.Size = New System.Drawing.Size(16, 24)
      Me.TemperatureLogCheck.TabIndex = 1
      Me.TemperatureLogCheck.Visible = False
      '
      'HumidityLogCheck
      '
      Me.HumidityLogCheck.Checked = True
      Me.HumidityLogCheck.CheckState = System.Windows.Forms.CheckState.Checked
      Me.HumidityLogCheck.Location = New System.Drawing.Point(48, 120)
      Me.HumidityLogCheck.Name = "HumidityLogCheck"
      Me.HumidityLogCheck.Size = New System.Drawing.Size(184, 24)
      Me.HumidityLogCheck.TabIndex = 2
      Me.HumidityLogCheck.Text = "Humidity Logging Enabled"
      Me.HumidityLogCheck.Visible = False
      '
      'TemperatureGroup
      '
      Me.TemperatureGroup.Controls.Add(Me.TempHighResRadio)
      Me.TemperatureGroup.Controls.Add(Me.TempLowResRadio)
      Me.TemperatureGroup.Location = New System.Drawing.Point(232, 64)
      Me.TemperatureGroup.Name = "TemperatureGroup"
      Me.TemperatureGroup.Size = New System.Drawing.Size(152, 64)
      Me.TemperatureGroup.TabIndex = 3
      Me.TemperatureGroup.TabStop = False
      Me.TemperatureGroup.Text = "Temperature Resolution"
      Me.TemperatureGroup.Visible = False
      '
      'TempHighResRadio
      '
      Me.TempHighResRadio.Location = New System.Drawing.Point(16, 40)
      Me.TempHighResRadio.Name = "TempHighResRadio"
      Me.TempHighResRadio.Size = New System.Drawing.Size(112, 16)
      Me.TempHighResRadio.TabIndex = 1
      Me.TempHighResRadio.Text = "0.0625"
      '
      'TempLowResRadio
      '
      Me.TempLowResRadio.Checked = True
      Me.TempLowResRadio.Location = New System.Drawing.Point(16, 24)
      Me.TempLowResRadio.Name = "TempLowResRadio"
      Me.TempLowResRadio.Size = New System.Drawing.Size(112, 16)
      Me.TempLowResRadio.TabIndex = 0
      Me.TempLowResRadio.TabStop = True
      Me.TempLowResRadio.Text = "0.5"
      '
      'HumidityGroup
      '
      Me.HumidityGroup.Controls.Add(Me.HumidityHighResRadio)
      Me.HumidityGroup.Controls.Add(Me.HumidityLowResRadio)
      Me.HumidityGroup.Location = New System.Drawing.Point(232, 128)
      Me.HumidityGroup.Name = "HumidityGroup"
      Me.HumidityGroup.Size = New System.Drawing.Size(152, 64)
      Me.HumidityGroup.TabIndex = 4
      Me.HumidityGroup.TabStop = False
      Me.HumidityGroup.Text = "Humidity Resolution"
      Me.HumidityGroup.Visible = False
      '
      'HumidityHighResRadio
      '
      Me.HumidityHighResRadio.Location = New System.Drawing.Point(16, 40)
      Me.HumidityHighResRadio.Name = "HumidityHighResRadio"
      Me.HumidityHighResRadio.Size = New System.Drawing.Size(112, 16)
      Me.HumidityHighResRadio.TabIndex = 1
      Me.HumidityHighResRadio.Text = "0.125"
      '
      'HumidityLowResRadio
      '
      Me.HumidityLowResRadio.Checked = True
      Me.HumidityLowResRadio.Location = New System.Drawing.Point(16, 24)
      Me.HumidityLowResRadio.Name = "HumidityLowResRadio"
      Me.HumidityLowResRadio.Size = New System.Drawing.Size(112, 16)
      Me.HumidityLowResRadio.TabIndex = 0
      Me.HumidityLowResRadio.TabStop = True
      Me.HumidityLowResRadio.Text = "0.5"
      '
      'Label
      '
      Me.Label.Location = New System.Drawing.Point(48, 360)
      Me.Label.Name = "Label"
      Me.Label.Size = New System.Drawing.Size(120, 24)
      Me.Label.TabIndex = 10
      Me.Label.Text = "Calculated Start Delay"
      '
      'DelayLabel
      '
      Me.DelayLabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText
      Me.DelayLabel.Location = New System.Drawing.Point(176, 360)
      Me.DelayLabel.Name = "DelayLabel"
      Me.DelayLabel.Size = New System.Drawing.Size(144, 16)
      Me.DelayLabel.TabIndex = 11
      Me.DelayLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter
      '
      'CalculateTimer
      '
      Me.CalculateTimer.Enabled = True
      Me.CalculateTimer.Interval = 1000
      '
      'Label5
      '
      Me.Label5.Location = New System.Drawing.Point(48, 336)
      Me.Label5.Name = "Label5"
      Me.Label5.Size = New System.Drawing.Size(128, 16)
      Me.Label5.TabIndex = 14
      Me.Label5.Text = "Mission Start Date/Time"
      '
      'DisplayTimeDateLabel
      '
      Me.DisplayTimeDateLabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText
      Me.DisplayTimeDateLabel.Location = New System.Drawing.Point(176, 336)
      Me.DisplayTimeDateLabel.Name = "DisplayTimeDateLabel"
      Me.DisplayTimeDateLabel.Size = New System.Drawing.Size(144, 16)
      Me.DisplayTimeDateLabel.TabIndex = 15
      Me.DisplayTimeDateLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter
      '
      'MinuteLabel
      '
      Me.MinuteLabel.Location = New System.Drawing.Point(320, 360)
      Me.MinuteLabel.Name = "MinuteLabel"
      Me.MinuteLabel.Size = New System.Drawing.Size(80, 16)
      Me.MinuteLabel.TabIndex = 16
      Me.MinuteLabel.Text = "Minutes"
      '
      'StartMissionButton
      '
      Me.StartMissionButton.Enabled = False
      Me.StartMissionButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.StartMissionButton.Location = New System.Drawing.Point(224, 392)
      Me.StartMissionButton.Name = "StartMissionButton"
      Me.StartMissionButton.Size = New System.Drawing.Size(144, 24)
      Me.StartMissionButton.TabIndex = 17
      Me.StartMissionButton.Text = "Start Mission"
      '
      'DeviceROMLabel
      '
      Me.DeviceROMLabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText
      Me.DeviceROMLabel.Location = New System.Drawing.Point(160, 16)
      Me.DeviceROMLabel.Name = "DeviceROMLabel"
      Me.DeviceROMLabel.Size = New System.Drawing.Size(144, 16)
      Me.DeviceROMLabel.TabIndex = 19
      Me.DeviceROMLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter
      '
      'DeviceTypeLabel
      '
      Me.DeviceTypeLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.DeviceTypeLabel.Location = New System.Drawing.Point(32, 16)
      Me.DeviceTypeLabel.Name = "DeviceTypeLabel"
      Me.DeviceTypeLabel.Size = New System.Drawing.Size(120, 16)
      Me.DeviceTypeLabel.TabIndex = 18
      Me.DeviceTypeLabel.Text = "Waiting for Device "
      Me.DeviceTypeLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
      '
      'StatusBar
      '
      Me.StatusBar.Location = New System.Drawing.Point(0, 424)
      Me.StatusBar.Name = "StatusBar"
      Me.StatusBar.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.StatusAdapter, Me.StatusPort, Me.StatusMsg})
      Me.StatusBar.ShowPanels = True
      Me.StatusBar.Size = New System.Drawing.Size(408, 24)
      Me.StatusBar.TabIndex = 20
      '
      'StatusAdapter
      '
      Me.StatusAdapter.Name = "StatusAdapter"
      Me.StatusAdapter.Width = 120
      '
      'StatusPort
      '
      Me.StatusPort.Name = "StatusPort"
      Me.StatusPort.Width = 80
      '
      'StatusMsg
      '
      Me.StatusMsg.Name = "StatusMsg"
      Me.StatusMsg.Width = 200
      '
      'StopMissionButton
      '
      Me.StopMissionButton.Enabled = False
      Me.StopMissionButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.StopMissionButton.Location = New System.Drawing.Point(40, 392)
      Me.StopMissionButton.Name = "StopMissionButton"
      Me.StopMissionButton.Size = New System.Drawing.Size(144, 24)
      Me.StopMissionButton.TabIndex = 21
      Me.StopMissionButton.Text = "Stop Mission"
      '
      'GroupBox1
      '
      Me.GroupBox1.Controls.Add(Me.Label4)
      Me.GroupBox1.Controls.Add(Me.Label3)
      Me.GroupBox1.Controls.Add(Me.MinuteCombo)
      Me.GroupBox1.Controls.Add(Me.HourCombo)
      Me.GroupBox1.Controls.Add(Me.Label2)
      Me.GroupBox1.Controls.Add(Me.DatePicker)
      Me.GroupBox1.Controls.Add(Me.Label1)
      Me.GroupBox1.Location = New System.Drawing.Point(40, 240)
      Me.GroupBox1.Name = "GroupBox1"
      Me.GroupBox1.Size = New System.Drawing.Size(344, 88)
      Me.GroupBox1.TabIndex = 22
      Me.GroupBox1.TabStop = False
      Me.GroupBox1.Text = "Mission Start Date Selection"
      '
      'Label4
      '
      Me.Label4.Location = New System.Drawing.Point(232, 52)
      Me.Label4.Name = "Label4"
      Me.Label4.Size = New System.Drawing.Size(40, 16)
      Me.Label4.TabIndex = 20
      Me.Label4.Text = "Minute"
      '
      'Label3
      '
      Me.Label3.Location = New System.Drawing.Point(136, 52)
      Me.Label3.Name = "Label3"
      Me.Label3.Size = New System.Drawing.Size(32, 16)
      Me.Label3.TabIndex = 19
      Me.Label3.Text = "Hour"
      '
      'MinuteCombo
      '
      Me.MinuteCombo.Items.AddRange(New Object() {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"})
      Me.MinuteCombo.Location = New System.Drawing.Point(272, 44)
      Me.MinuteCombo.Name = "MinuteCombo"
      Me.MinuteCombo.Size = New System.Drawing.Size(64, 21)
      Me.MinuteCombo.TabIndex = 18
      Me.MinuteCombo.Text = "Minute"
      '
      'HourCombo
      '
      Me.HourCombo.Items.AddRange(New Object() {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"})
      Me.HourCombo.Location = New System.Drawing.Point(168, 44)
      Me.HourCombo.Name = "HourCombo"
      Me.HourCombo.Size = New System.Drawing.Size(56, 21)
      Me.HourCombo.TabIndex = 17
      Me.HourCombo.Text = "Hour"
      '
      'Label2
      '
      Me.Label2.Location = New System.Drawing.Point(16, 52)
      Me.Label2.Name = "Label2"
      Me.Label2.Size = New System.Drawing.Size(112, 16)
      Me.Label2.TabIndex = 16
      Me.Label2.Text = "Mission Start Time"
      '
      'DatePicker
      '
      Me.DatePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
      Me.DatePicker.Location = New System.Drawing.Point(136, 20)
      Me.DatePicker.Name = "DatePicker"
      Me.DatePicker.Size = New System.Drawing.Size(96, 20)
      Me.DatePicker.TabIndex = 15
      '
      'Label1
      '
      Me.Label1.Location = New System.Drawing.Point(16, 24)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(112, 16)
      Me.Label1.TabIndex = 14
      Me.Label1.Text = "Mission Start Date"
      '
      'SampleRateBox
      '
      Me.SampleRateBox.Location = New System.Drawing.Point(136, 200)
      Me.SampleRateBox.Name = "SampleRateBox"
      Me.SampleRateBox.Size = New System.Drawing.Size(80, 20)
      Me.SampleRateBox.TabIndex = 23
      Me.SampleRateBox.Text = "600"
      '
      'Label6
      '
      Me.Label6.Location = New System.Drawing.Point(48, 208)
      Me.Label6.Name = "Label6"
      Me.Label6.Size = New System.Drawing.Size(88, 16)
      Me.Label6.TabIndex = 24
      Me.Label6.Text = "Sample Rate "
      '
      'Label8
      '
      Me.Label8.Location = New System.Drawing.Point(216, 208)
      Me.Label8.Name = "Label8"
      Me.Label8.Size = New System.Drawing.Size(80, 16)
      Me.Label8.TabIndex = 25
      Me.Label8.Text = "Seconds"
      '
      'TemperatureLogLabel
      '
      Me.TemperatureLogLabel.Location = New System.Drawing.Point(64, 56)
      Me.TemperatureLogLabel.Name = "TemperatureLogLabel"
      Me.TemperatureLogLabel.Size = New System.Drawing.Size(152, 24)
      Me.TemperatureLogLabel.TabIndex = 26
      Me.TemperatureLogLabel.Text = "Log"
      Me.TemperatureLogLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
      '
      'Form1
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(408, 448)
      Me.Controls.Add(Me.TemperatureLogLabel)
      Me.Controls.Add(Me.Label8)
      Me.Controls.Add(Me.Label6)
      Me.Controls.Add(Me.SampleRateBox)
      Me.Controls.Add(Me.GroupBox1)
      Me.Controls.Add(Me.StopMissionButton)
      Me.Controls.Add(Me.StatusBar)
      Me.Controls.Add(Me.DeviceROMLabel)
      Me.Controls.Add(Me.DeviceTypeLabel)
      Me.Controls.Add(Me.StartMissionButton)
      Me.Controls.Add(Me.MinuteLabel)
      Me.Controls.Add(Me.DisplayTimeDateLabel)
      Me.Controls.Add(Me.Label5)
      Me.Controls.Add(Me.DelayLabel)
      Me.Controls.Add(Me.Label)
      Me.Controls.Add(Me.HumidityGroup)
      Me.Controls.Add(Me.TemperatureGroup)
      Me.Controls.Add(Me.HumidityLogCheck)
      Me.Controls.Add(Me.TemperatureLogCheck)
      Me.Controls.Add(Me.RolloverCheck)
      Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
      Me.Name = "Form1"
      Me.Text = "Mission Start Delay Demo"
      Me.TemperatureGroup.ResumeLayout(False)
      Me.HumidityGroup.ResumeLayout(False)
      CType(Me.StatusAdapter, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.StatusPort, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.StatusMsg, System.ComponentModel.ISupportInitialize).EndInit()
      Me.GroupBox1.ResumeLayout(False)
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub

#End Region


    Private Sub CalculateTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles CalculateTimer.Tick
        Dim DNow As DateTime = DateTime.Now

        ' Calculate the Delay
        If (HourCombo.SelectedIndex = -1) Or (MinuteCombo.SelectedIndex = -1) Then
            DisplayTimeDateLabel.Text = "Not Set Yet"
            MinutesDelay = -1
        Else
            Dim DDif As New System.DateTime(DatePicker.Value.Year, DatePicker.Value.Month, DatePicker.Value.Day, HourCombo.SelectedIndex, MinuteCombo.SelectedIndex, 0, 0)
            DisplayTimeDateLabel.Text = DDif.ToString

            ' calculate the delay
            MinutesDelay = CLng((DDif.Ticks - DNow.Ticks) / 10000000) ' Seconds
            MinutesDelay = CLng((MinutesDelay + 30) / 60) ' Minutes

            If (DNow.Ticks > DDif.Ticks) Then
                DelayLabel.Text = "Date selected is in the past "
                MinutesDelay = -1
            Else
                DelayLabel.Text = Str(MinutesDelay)
            End If
        End If

        ' Check for device
        Dim state As Object
        Dim channels As Integer
        Dim res() As Double

        Try
            ' get exclusive use of 1-Wire network
            adapter.beginExclusive(True)

            ' Only update screen if old device is not present
            If ((Not owd.isPresent()) Or (Not RolloverCheck.Visible)) Then
                ' clear any previous search restrictions
                adapter.setSearchAllDevices()
                adapter.targetAllFamilies()
                adapter.targetFamily(&H41)
                adapter.setSpeed(adapter.SPEED_REGULAR)

                ' Check if 41 family code device available
                If adapter.findFirstDevice Then
                    ' retrieve OneWireContainer
                    owd = adapter.getFirstDeviceContainer
                    ' check to see if 1-Wire device supports MissionContainer, if so get address 
                    If TypeOf owd Is com.dalsemi.onewire.container.MissionContainer Then
                        ' type cast to MissionContainer
                        mc = DirectCast(owd, com.dalsemi.onewire.container.MissionContainer)
                        ' read status
                        state = mc.readDevice
                        ' display device ID
                        DeviceROMLabel.Text = owd.getAddressAsString
                        RolloverCheck.Visible = True

                        ' See the channel option labels based on device type
                        channels = mc.getNumberMissionChannels()
                        If (channels >= 1) Then
                            TemperatureLogLabel.Text = mc.getMissionLabel(0) & " Logging Enable"
                            TemperatureGroup.Text = mc.getMissionLabel(0) & " Resolution"
                            res = mc.getMissionResolutions(0)
                            TempLowResRadio.Text = Str(res(0))
                            TempHighResRadio.Text = Str(res(1))
                            TemperatureGroup.Visible = True
                            TemperatureLogCheck.Visible = True
                            TemperatureLogLabel.Visible = True
                        End If

                        If (channels = 2) Then
                            DeviceTypeLabel.Text = "DS1923 "
                            HumidityLogCheck.Text = mc.getMissionLabel(1) & " Logging Enable"
                            HumidityGroup.Text = mc.getMissionLabel(1) & " Resolution"
                            res = mc.getMissionResolutions(1)
                            HumidityLowResRadio.Text = Str(res(0))
                            HumidityHighResRadio.Text = Str(res(1))
                            HumidityGroup.Visible = True
                            HumidityLogCheck.Visible = True
                            TemperatureLogCheck.Enabled = True
                        Else
                            TemperatureLogCheck.Checked = True
                            TemperatureLogCheck.Enabled = False
                            DeviceTypeLabel.Text = "DS1922L/T "
                        End If

                        ' check mission state
                        StopMissionButton.Enabled = mc.isMissionRunning
                    Else
                        StatusMsg.Text = "Unexpected Device Type"
                    End If
                Else
                    ' Device not present so clear screen
                    DeviceTypeLabel.Text = "Waiting for device"
                    DeviceROMLabel.Text = ""
                    TemperatureGroup.Visible = False
                    TemperatureLogCheck.Visible = False
                    TemperatureLogLabel.Visible = False
                    HumidityGroup.Visible = False
                    HumidityLogCheck.Visible = False
                    RolloverCheck.Visible = False
                    StopMissionButton.Enabled = False
                    StartMissionButton.Enabled = False
                    StatusMsg.Text = ""
                End If

            Else
                ' same device present, check button state
                ' Enabled start button only if mission disabled and delay set
                If ((Not StopMissionButton.Enabled) And (MinutesDelay <> -1)) Then
                    StartMissionButton.Enabled = True
                Else
                    StartMissionButton.Enabled = False
                End If
            End If

            ' end exclusive use of 1-Wire net adapter
            adapter.endExclusive()
        Catch ex As Exception
            StatusMsg.Text = ex.ToString
        End Try

    End Sub



    Private Sub StartMissionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StartMissionButton.Click
        Dim enables() As Boolean = {False, False}
        Dim state As Object
        Dim res() As Double
        Dim rsn As Double

        Try
            ' get exclusive use of 1-Wire network
            adapter.beginExclusive(True)

            ' read status
            state = mc.readDevice

            ' set channel resolutions and enables based on selections
            enables(0) = TemperatureLogCheck.Checked
            If (TemperatureLogCheck.Checked) Then
                res = mc.getMissionResolutions(0)
                If (TempLowResRadio.Checked) Then
                    rsn = res(0)
                Else
                    rsn = res(1)
                End If
                mc.setMissionResolution(0, rsn)
            End If

            If (HumidityLogCheck.Visible) Then
                enables(1) = HumidityLogCheck.Checked
                If (HumidityLogCheck.Checked) Then
                    res = mc.getMissionResolutions(1)
                    If (HumidityLowResRadio.Checked) Then
                        rsn = res(0)
                    Else
                        rsn = res(1)
                    End If
                    mc.setMissionResolution(1, rsn)
                End If
            Else
                enables(1) = False
            End If

            ' start the mission 
            StartMissionButton.Enabled = False
            mc.startNewMission(Val(SampleRateBox.Text), MinutesDelay, RolloverCheck.Checked, True, enables)
            ' force timer thread to read-read device to set buttons correctly
            owd = New com.dalsemi.onewire.container.OneWireContainer(adapter, "5500000000000055")

            ' end exclusive use of 1-Wire net adapter
            adapter.endExclusive()
        Catch ex As Exception
            StatusMsg.Text = ex.ToString
        End Try
    End Sub

    Private Sub StopMissionButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StopMissionButton.Click
        Try
            ' get exclusive use of 1-Wire network
            adapter.beginExclusive(True)

            ' attempt to stop mission
            StopMissionButton.Enabled = False
            mc.stopMission()

            ' force timer thread to read-read device to set buttons correctly
            owd = New com.dalsemi.onewire.container.OneWireContainer(adapter, "5500000000000055")

            ' end exclusive use of 1-Wire net adapter
            adapter.endExclusive()
        Catch ex As Exception
            StatusMsg.Text = ex.ToString
        End Try

    End Sub

   Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter
         ' initialize adapter to proper settings

         ' print that we got an adapter
         StatusAdapter.Text = "Adapter: " & adapter.getAdapterName
         StatusPort.Text = "Port: " & adapter.getPortName
         ' setup dummy container
         owd = New com.dalsemi.onewire.container.OneWireContainer(adapter, "5500000000000055")
      Catch ex As Exception
         StatusMsg.Text = "Error:  " & ex.ToString
      End Try

   End Sub
End Class
