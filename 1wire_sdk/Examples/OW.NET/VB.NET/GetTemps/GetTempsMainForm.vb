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

Public Class GetTempsForm
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
      Me.Text = "Get Temps"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub

#End Region

   Private Sub GetTempsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetTempsButton.Click
      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim state As Object
      Dim tc As com.dalsemi.onewire.container.TemperatureContainer

      Try
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' Get all device containers
         owd_enum = adapter.getAllDeviceContainers
         ResultsTextBox.AppendText(Environment.NewLine() & "1-Wire List:" & Environment.NewLine())
         ResultsTextBox.AppendText("==========================" & Environment.NewLine())

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)

         While owd_enum.hasMoreElements()
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement()
            ' check to see if 1-Wire device supports temperatures, if so get address and temp.
            If TypeOf owd Is com.dalsemi.onewire.container.TemperatureContainer Then
               ' cast the OneWireContainer to TemperatureContainer
               tc = DirectCast(owd, com.dalsemi.onewire.container.TemperatureContainer)
               ' read the device
               state = tc.readDevice
               ' extract the temperature from previous read
               tc.doTemperatureConvert(state)
               ' retrieve OneWireAddress
               ResultsTextBox.AppendText(Environment.NewLine())
               ResultsTextBox.AppendText("Address = " & owd.getAddressAsString & Environment.NewLine())
               ResultsTextBox.AppendText("Name = " & owd.getName & Environment.NewLine())
               ResultsTextBox.AppendText("Description = " & owd.getDescription & Environment.NewLine())
               ResultsTextBox.AppendText("Temperature = " & Math.Round(tc.getTemperature(state), 5) & " °C" & Environment.NewLine())
            End If

         End While
         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()
      Catch ex As Exception
         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine() & "Error:  " & ex.ToString & Environment.NewLine())
      End Try
   End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

   Private Sub GetTempsForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter
         ' print that we got an adapter
         ResultsTextBox.AppendText(Environment.NewLine() & "Adapter: " & adapter.getAdapterName & " Port: " & adapter.getPortName & Environment.NewLine() & Environment.NewLine())
      Catch ex As Exception
         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine() & "Error:  " & ex.ToString)
      End Try
   End Sub
End Class
