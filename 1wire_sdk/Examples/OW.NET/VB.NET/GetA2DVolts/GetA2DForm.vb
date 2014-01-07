'-------------------------------------------------------------------------
'Copyright (C) 2008 Maxim Integrated Products, All Rights Reserved.
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

Public Class GetA2DForm
   Inherits System.Windows.Forms.Form
   Dim adapter As com.dalsemi.onewire.adapter.DSPortAdapter


#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

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
    Friend WithEvents ButtonGetVolts As System.Windows.Forms.Button
    Friend WithEvents ResultsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents StatusBar1 As System.Windows.Forms.StatusBar
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.ButtonGetVolts = New System.Windows.Forms.Button
      Me.ResultsTextBox = New System.Windows.Forms.TextBox
      Me.StatusBar1 = New System.Windows.Forms.StatusBar
      Me.SuspendLayout()
      '
      'ButtonGetVolts
      '
      Me.ButtonGetVolts.Location = New System.Drawing.Point(296, 104)
      Me.ButtonGetVolts.Name = "ButtonGetVolts"
      Me.ButtonGetVolts.Size = New System.Drawing.Size(72, 32)
      Me.ButtonGetVolts.TabIndex = 0
      Me.ButtonGetVolts.Text = "&Get Volts"
      '
      'ResultsTextBox
      '
      Me.ResultsTextBox.AcceptsReturn = True
      Me.ResultsTextBox.Location = New System.Drawing.Point(48, 24)
      Me.ResultsTextBox.Multiline = True
      Me.ResultsTextBox.Name = "ResultsTextBox"
      Me.ResultsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
      Me.ResultsTextBox.Size = New System.Drawing.Size(232, 216)
      Me.ResultsTextBox.TabIndex = 1
      '
      'StatusBar1
      '
      Me.StatusBar1.Location = New System.Drawing.Point(0, 272)
      Me.StatusBar1.Name = "StatusBar1"
      Me.StatusBar1.Size = New System.Drawing.Size(392, 22)
      Me.StatusBar1.TabIndex = 2
      '
      'GetA2DForm
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(392, 294)
      Me.Controls.Add(Me.StatusBar1)
      Me.Controls.Add(Me.ResultsTextBox)
      Me.Controls.Add(Me.ButtonGetVolts)
      Me.Name = "GetA2DForm"
      Me.Text = "Get A to D Voltages"
      Me.ResumeLayout(False)

   End Sub

#End Region

    Private Sub ButtonGetVolts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonGetVolts.Click
        ' This program retrieves all 1-Wire Analog-To-Digital devices found on a 1-Wire 
        ' network and lists their addresses and voltage readings.

      Dim deviceFound As Integer
        Dim voltages() As Double
        Dim numOfADChannels As Integer
      Dim channelCount As Integer
      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim state As Object
      Dim adc As com.dalsemi.onewire.container.ADContainer
      Dim tc As com.dalsemi.onewire.container.TemperatureContainer

        Try
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers
         ResultsTextBox.AppendText("1-Wire Analog-To-Digital Device List:" & Environment.NewLine())
         deviceFound = 0
         While owd_enum.hasMoreElements
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement()
            ' check to see if 1-Wire device supports temperatures, if so get address and temp.
            If TypeOf owd Is com.dalsemi.onewire.container.ADContainer Then
               ' cast the OneWireContainer to ADContainer
               adc = DirectCast(owd, com.dalsemi.onewire.container.ADContainer)
               deviceFound = 1
               channelCount = 0
               ' read the device
               state = adc.readDevice
               ' read the voltages
               ResultsTextBox.AppendText(Environment.NewLine() & "Address = " & owd.getAddressAsString & Environment.NewLine())
               ' ResultsTextBox.AppendText("Description = " & owd.getDescription & Environment.NewLine())
               ResultsTextBox.AppendText("        " & "Part Number = " & owd.getName() & Environment.NewLine())
               ' cycle through the different A2D channels
                    numOfADChannels = adc.getNumberADChannels
                    ReDim voltages(numOfADChannels)
               For channelCount = 0 To (numOfADChannels - 1)
                  adc.doADConvert(channelCount, state)
                  voltages(channelCount) = adc.getADVoltage(channelCount, state)
                  ResultsTextBox.AppendText("        " & "Channel: " & channelCount & "   Voltage:  " & voltages(channelCount) & " Volts" & Environment.NewLine())
               Next
               If TypeOf owd Is com.dalsemi.onewire.container.TemperatureContainer Then
                  ' cast the OneWireContainer to TemperatureContainer
                  tc = DirectCast(owd, com.dalsemi.onewire.container.TemperatureContainer)
                  ' read the device
                  state = tc.readDevice
                  ' extract the temperature from previous read
                  tc.doTemperatureConvert(state)
                  ResultsTextBox.AppendText("        " & "Temperature = " & Math.Round(tc.getTemperature(state), 1) & " °C" & Environment.NewLine())
               End If
               ResultsTextBox.AppendText(Environment.NewLine())
            End If
         End While

            ' show temperature list
         If (deviceFound = 0) Then ResultsTextBox.AppendText(Environment.NewLine() & "No 1-Wire Analog-To-Digital devices found!")

            ' end exclusive use of adapter
            adapter.endExclusive()

            ' free port used by adapter
            'adapter.freePort()
        Catch ex As Exception
            StatusBar1.Text = StatusBar1.Text & "  Error:  " & ex.ToString()
        End Try

    End Sub

   Private Sub GetA2DForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter

         ' print that we got an adapter
         StatusBar1.Text = "Adapter: " & adapter.getAdapterName & " Port: " & adapter.getPortName
      Catch ex As Exception
         StatusBar1.Text = StatusBar1.Text & "  Error:  " & ex.ToString()
      End Try

   End Sub
End Class
