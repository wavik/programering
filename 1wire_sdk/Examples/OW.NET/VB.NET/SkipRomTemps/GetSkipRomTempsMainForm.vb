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

Public Class GetTempsForm
   Inherits System.Windows.Forms.Form
   Dim CRLF As New String(Environment.NewLine()) ' makes a carriage-return-linefeed string to begin a new line in a text box
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
    Friend WithEvents GetTempsButton As System.Windows.Forms.Button
    Friend WithEvents ResultsTextBox As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.ResultsTextBox = New System.Windows.Forms.TextBox
      Me.GetTempsButton = New System.Windows.Forms.Button
      Me.SuspendLayout()
      '
      'ResultsTextBox
      '
      Me.ResultsTextBox.Location = New System.Drawing.Point(40, 32)
      Me.ResultsTextBox.Multiline = True
      Me.ResultsTextBox.Name = "ResultsTextBox"
      Me.ResultsTextBox.Size = New System.Drawing.Size(296, 240)
      Me.ResultsTextBox.TabIndex = 0
      '
      'GetTempsButton
      '
      Me.GetTempsButton.Location = New System.Drawing.Point(352, 128)
      Me.GetTempsButton.Name = "GetTempsButton"
      Me.GetTempsButton.Size = New System.Drawing.Size(80, 32)
      Me.GetTempsButton.TabIndex = 1
      Me.GetTempsButton.Text = "&Get Temps"
      '
      'GetTempsForm
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(448, 318)
      Me.Controls.Add(Me.GetTempsButton)
      Me.Controls.Add(Me.ResultsTextBox)
      Me.Name = "GetTempsForm"
      Me.Text = "Get SkipRom Temps"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub

#End Region

    Private Sub GetTempsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetTempsButton.Click
        Dim owd_enum As java.util.Enumeration
        Dim owd As com.dalsemi.onewire.container.OneWireContainer
        Dim owd10 As com.dalsemi.onewire.container.OneWireContainer10
        Dim state As Object

        Try
            ' get exclusive use of 1-Wire network
            adapter.beginExclusive(True)

            ' clear any previous search restrictions
            ' setup adapter to search for DS1920s
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)
            adapter.targetFamily(&H10)
            adapter.Reset()
            adapter.setSearchAllDevices()

            ' Do SkipRom and then Temp Convert
            ' This assumes not externally powered
            adapter.reset()
            adapter.putByte(&HCC) ' issue skip rom command to broadcast the temp conversion to all 1-Wire devices.

            ' if you have EXTERNALLY-POWERED DS18S20s, then comment out the following 4 lines
            If adapter.canDeliverPower() Then ' if the adapter can deliver power, do so now
            adapter.setPowerDuration(com.dalsemi.onewire.adapter.DSPortAdapter.DELIVERY_INFINITE) ' sets duration to where we can turn it off manually later
            adapter.startPowerDelivery(com.dalsemi.onewire.adapter.DSPortAdapter.CONDITION_AFTER_BYTE) ' starts power deliver after next putByte
            End If
            ' End of Parasite-power code

            adapter.putByte(68)  ' issue Temp convert command (broadcastin to all 1-Wire devices)
         System.Threading.Thread.Sleep(1000) ' Feel free to change this number based on type of thermometer and resolution

            ' if you have EXTERNALLY-POWERED DS18S20s, then comment out the following line
            adapter.setPowerNormal() ' Return to normal 1-Wire line levels
            ' End of Parasite-power code

            adapter.reset()

            ' Get all device containers and read results
            owd_enum = adapter.getAllDeviceContainers
            ResultsTextBox.AppendText(CRLF & "1-Wire List:" & CRLF)
            ResultsTextBox.AppendText("==========================" & CRLF)

            ' enumerate through all the 1-Wire devices found (with Java-style enumeration)

            While owd_enum.hasMoreElements()
                ' retrieve OneWireContainer
                owd = owd_enum.nextElement()
                owd10 = owd
                ' read the device
                state = owd10.readDevice
                ' retrieve OneWireAddress
                ResultsTextBox.AppendText(CRLF)
                ResultsTextBox.AppendText("Address = " & owd10.getAddressAsString & CRLF)
                ResultsTextBox.AppendText("Name = " & owd10.getName & CRLF)
                ResultsTextBox.AppendText("Description = " & owd10.getDescription & CRLF)
                ResultsTextBox.AppendText("Temperature = " & Math.Round(owd10.getTemperature(state), 5) & " °C" & CRLF)
            End While
            ' end exclusive use of 1-Wire net adapter
            adapter.endExclusive()
        Catch ex As Exception
            ResultsTextBox.AppendText(CRLF & CRLF & "Error:  " & ex.ToString & CRLF)
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

   Private Sub GetTempsForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter
         ' initialize adapter to proper settings

         ' print that we got an adapter
         ResultsTextBox.AppendText(CRLF & "Adapter: " & adapter.getAdapterName & " Port: " & adapter.getPortName & CRLF & CRLF)
      Catch ex As Exception
         ResultsTextBox.AppendText(CRLF & CRLF & "Error:  " & ex.ToString)
      End Try
   End Sub
End Class
