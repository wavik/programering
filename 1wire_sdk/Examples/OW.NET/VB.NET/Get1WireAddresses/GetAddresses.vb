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

Public Class Form1

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
    Friend WithEvents ResultsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents SearchButton As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.SearchButton = New System.Windows.Forms.Button
        Me.ResultsTextBox = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'SearchButton
        '
        Me.SearchButton.BackColor = System.Drawing.SystemColors.Control
        Me.SearchButton.Location = New System.Drawing.Point(336, 112)
        Me.SearchButton.Name = "SearchButton"
        Me.SearchButton.TabIndex = 0
        Me.SearchButton.Text = "&Search"
        '
        'ResultsTextBox
        '
        Me.ResultsTextBox.Location = New System.Drawing.Point(24, 24)
        Me.ResultsTextBox.Multiline = True
        Me.ResultsTextBox.Name = "ResultsTextBox"
        Me.ResultsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.ResultsTextBox.Size = New System.Drawing.Size(296, 200)
        Me.ResultsTextBox.TabIndex = 1
        Me.ResultsTextBox.Text = ""
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(424, 266)
        Me.Controls.Add(Me.ResultsTextBox)
        Me.Controls.Add(Me.SearchButton)
        Me.Name = "Form1"
        Me.Text = "Get 1-Wire Addresses VB.NET"
        Me.ResumeLayout(False)

    End Sub

#End Region

   Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchButton.Click
      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer

      Try
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers
         ResultsTextBox.AppendText(Environment.NewLine() & "1-Wire List:" & Environment.NewLine())
         ResultsTextBox.AppendText("==========================" & Environment.NewLine())

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         '
         While owd_enum.hasMoreElements()
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement()
            ' retrieve OneWireAddress
            ResultsTextBox.AppendText(Environment.NewLine())
            ResultsTextBox.AppendText("Address = " & owd.getAddressAsString & Environment.NewLine())
            ResultsTextBox.AppendText("Description = " & owd.getDescription & Environment.NewLine())
         End While
         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()
      Catch ex As Exception
         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine() & "Error:  " & ex.ToString)
      End Try

   End Sub

   Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
