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
   Friend WithEvents SearchButton As System.Windows.Forms.Button
   Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
   Friend WithEvents Label1 As System.Windows.Forms.Label
   Friend WithEvents Label2 As System.Windows.Forms.Label
   Friend WithEvents ListBox2 As System.Windows.Forms.ListBox
   Friend WithEvents Label3 As System.Windows.Forms.Label
   Friend WithEvents Label4 As System.Windows.Forms.Label
   Friend WithEvents ListBox3 As System.Windows.Forms.ListBox
   Friend WithEvents Label5 As System.Windows.Forms.Label
   Friend WithEvents ListBox4 As System.Windows.Forms.ListBox
   Friend WithEvents Label6 As System.Windows.Forms.Label
   Friend WithEvents ListBox5 As System.Windows.Forms.ListBox
   Friend WithEvents Label7 As System.Windows.Forms.Label
   Friend WithEvents Label8 As System.Windows.Forms.Label
   Friend WithEvents Label9 As System.Windows.Forms.Label
   Friend WithEvents Label10 As System.Windows.Forms.Label
   Friend WithEvents Label11 As System.Windows.Forms.Label
   Friend WithEvents Button1 As System.Windows.Forms.Button
   Friend WithEvents StatusBar1 As System.Windows.Forms.StatusBar
   Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
   Friend WithEvents NumericUpDown2 As System.Windows.Forms.NumericUpDown
   Friend WithEvents NumericUpDown3 As System.Windows.Forms.NumericUpDown
   Friend WithEvents NumericUpDown4 As System.Windows.Forms.NumericUpDown
   Friend WithEvents NumericUpDown5 As System.Windows.Forms.NumericUpDown
   Friend WithEvents Label12 As System.Windows.Forms.Label
   Friend WithEvents NumericUpDown6 As System.Windows.Forms.NumericUpDown
   Friend WithEvents Label13 As System.Windows.Forms.Label
   Friend WithEvents Label14 As System.Windows.Forms.Label
   Friend WithEvents ListBox6 As System.Windows.Forms.ListBox
   Friend WithEvents Label15 As System.Windows.Forms.Label
   Friend WithEvents ListBox7 As System.Windows.Forms.ListBox
   <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.SearchButton = New System.Windows.Forms.Button
      Me.ListBox1 = New System.Windows.Forms.ListBox
      Me.Label1 = New System.Windows.Forms.Label
      Me.Label2 = New System.Windows.Forms.Label
      Me.ListBox2 = New System.Windows.Forms.ListBox
      Me.Label3 = New System.Windows.Forms.Label
      Me.Label4 = New System.Windows.Forms.Label
      Me.ListBox3 = New System.Windows.Forms.ListBox
      Me.Label5 = New System.Windows.Forms.Label
      Me.ListBox4 = New System.Windows.Forms.ListBox
      Me.Label6 = New System.Windows.Forms.Label
      Me.ListBox5 = New System.Windows.Forms.ListBox
      Me.Label7 = New System.Windows.Forms.Label
      Me.Label8 = New System.Windows.Forms.Label
      Me.Label9 = New System.Windows.Forms.Label
      Me.Label10 = New System.Windows.Forms.Label
      Me.Label11 = New System.Windows.Forms.Label
      Me.Button1 = New System.Windows.Forms.Button
      Me.StatusBar1 = New System.Windows.Forms.StatusBar
      Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown
      Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown
      Me.NumericUpDown3 = New System.Windows.Forms.NumericUpDown
      Me.NumericUpDown4 = New System.Windows.Forms.NumericUpDown
      Me.NumericUpDown5 = New System.Windows.Forms.NumericUpDown
      Me.Label12 = New System.Windows.Forms.Label
      Me.NumericUpDown6 = New System.Windows.Forms.NumericUpDown
      Me.Label13 = New System.Windows.Forms.Label
      Me.Label14 = New System.Windows.Forms.Label
      Me.ListBox6 = New System.Windows.Forms.ListBox
      Me.Label15 = New System.Windows.Forms.Label
      Me.ListBox7 = New System.Windows.Forms.ListBox
      CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.NumericUpDown5, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.NumericUpDown6, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'SearchButton
      '
      Me.SearchButton.BackColor = System.Drawing.SystemColors.Control
      Me.SearchButton.Location = New System.Drawing.Point(64, 240)
      Me.SearchButton.Name = "SearchButton"
      Me.SearchButton.TabIndex = 1
      Me.SearchButton.Text = "&Search"
      '
      'ListBox1
      '
      Me.ListBox1.Location = New System.Drawing.Point(8, 40)
      Me.ListBox1.Name = "ListBox1"
      Me.ListBox1.Size = New System.Drawing.Size(184, 186)
      Me.ListBox1.TabIndex = 2
      '
      'Label1
      '
      Me.Label1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label1.Location = New System.Drawing.Point(8, 8)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(120, 23)
      Me.Label1.TabIndex = 3
      Me.Label1.Text = "iButton Timer List"
      '
      'Label2
      '
      Me.Label2.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label2.Location = New System.Drawing.Point(232, 8)
      Me.Label2.Name = "Label2"
      Me.Label2.Size = New System.Drawing.Size(152, 23)
      Me.Label2.TabIndex = 4
      Me.Label2.Text = "Current Timer Values"
      '
      'ListBox2
      '
      Me.ListBox2.Location = New System.Drawing.Point(232, 96)
      Me.ListBox2.Name = "ListBox2"
      Me.ListBox2.Size = New System.Drawing.Size(32, 17)
      Me.ListBox2.TabIndex = 0
      '
      'Label3
      '
      Me.Label3.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label3.Location = New System.Drawing.Point(232, 80)
      Me.Label3.Name = "Label3"
      Me.Label3.Size = New System.Drawing.Size(36, 15)
      Me.Label3.TabIndex = 6
      Me.Label3.Text = "Hour"
      '
      'Label4
      '
      Me.Label4.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label4.Location = New System.Drawing.Point(280, 80)
      Me.Label4.Name = "Label4"
      Me.Label4.Size = New System.Drawing.Size(36, 15)
      Me.Label4.TabIndex = 7
      Me.Label4.Text = "Min"
      '
      'ListBox3
      '
      Me.ListBox3.Location = New System.Drawing.Point(280, 96)
      Me.ListBox3.Name = "ListBox3"
      Me.ListBox3.Size = New System.Drawing.Size(32, 17)
      Me.ListBox3.TabIndex = 0
      '
      'Label5
      '
      Me.Label5.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label5.Location = New System.Drawing.Point(328, 80)
      Me.Label5.Name = "Label5"
      Me.Label5.Size = New System.Drawing.Size(36, 15)
      Me.Label5.TabIndex = 9
      Me.Label5.Text = "Sec"
      '
      'ListBox4
      '
      Me.ListBox4.Location = New System.Drawing.Point(328, 96)
      Me.ListBox4.Name = "ListBox4"
      Me.ListBox4.Size = New System.Drawing.Size(32, 17)
      Me.ListBox4.TabIndex = 0
      '
      'Label6
      '
      Me.Label6.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label6.Location = New System.Drawing.Point(328, 32)
      Me.Label6.Name = "Label6"
      Me.Label6.Size = New System.Drawing.Size(36, 15)
      Me.Label6.TabIndex = 11
      Me.Label6.Text = "Year"
      '
      'ListBox5
      '
      Me.ListBox5.Location = New System.Drawing.Point(328, 48)
      Me.ListBox5.Name = "ListBox5"
      Me.ListBox5.Size = New System.Drawing.Size(40, 17)
      Me.ListBox5.TabIndex = 0
      '
      'Label7
      '
      Me.Label7.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label7.Location = New System.Drawing.Point(232, 192)
      Me.Label7.Name = "Label7"
      Me.Label7.Size = New System.Drawing.Size(36, 15)
      Me.Label7.TabIndex = 13
      Me.Label7.Text = "Hour"
      '
      'Label8
      '
      Me.Label8.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label8.Location = New System.Drawing.Point(280, 192)
      Me.Label8.Name = "Label8"
      Me.Label8.Size = New System.Drawing.Size(36, 15)
      Me.Label8.TabIndex = 18
      Me.Label8.Text = "Min"
      '
      'Label9
      '
      Me.Label9.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label9.Location = New System.Drawing.Point(328, 192)
      Me.Label9.Name = "Label9"
      Me.Label9.Size = New System.Drawing.Size(36, 15)
      Me.Label9.TabIndex = 19
      Me.Label9.Text = "Sec"
      '
      'Label10
      '
      Me.Label10.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label10.Location = New System.Drawing.Point(328, 144)
      Me.Label10.Name = "Label10"
      Me.Label10.Size = New System.Drawing.Size(36, 15)
      Me.Label10.TabIndex = 20
      Me.Label10.Text = "Year"
      '
      'Label11
      '
      Me.Label11.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label11.Location = New System.Drawing.Point(232, 120)
      Me.Label11.Name = "Label11"
      Me.Label11.Size = New System.Drawing.Size(152, 23)
      Me.Label11.TabIndex = 21
      Me.Label11.Text = "New Timer Values"
      '
      'Button1
      '
      Me.Button1.BackColor = System.Drawing.SystemColors.Control
      Me.Button1.Location = New System.Drawing.Point(264, 240)
      Me.Button1.Name = "Button1"
      Me.Button1.TabIndex = 9
      Me.Button1.Text = "&Update"
      '
      'StatusBar1
      '
      Me.StatusBar1.Location = New System.Drawing.Point(0, 288)
      Me.StatusBar1.Name = "StatusBar1"
      Me.StatusBar1.Size = New System.Drawing.Size(440, 16)
      Me.StatusBar1.TabIndex = 23
      '
      'NumericUpDown1
      '
      Me.NumericUpDown1.Location = New System.Drawing.Point(232, 208)
      Me.NumericUpDown1.Maximum = New Decimal(New Integer() {23, 0, 0, 0})
      Me.NumericUpDown1.Name = "NumericUpDown1"
      Me.NumericUpDown1.Size = New System.Drawing.Size(32, 20)
      Me.NumericUpDown1.TabIndex = 6
      '
      'NumericUpDown2
      '
      Me.NumericUpDown2.Location = New System.Drawing.Point(280, 208)
      Me.NumericUpDown2.Maximum = New Decimal(New Integer() {59, 0, 0, 0})
      Me.NumericUpDown2.Name = "NumericUpDown2"
      Me.NumericUpDown2.Size = New System.Drawing.Size(32, 20)
      Me.NumericUpDown2.TabIndex = 7
      '
      'NumericUpDown3
      '
      Me.NumericUpDown3.Location = New System.Drawing.Point(328, 208)
      Me.NumericUpDown3.Maximum = New Decimal(New Integer() {59, 0, 0, 0})
      Me.NumericUpDown3.Name = "NumericUpDown3"
      Me.NumericUpDown3.Size = New System.Drawing.Size(32, 20)
      Me.NumericUpDown3.TabIndex = 8
      '
      'NumericUpDown4
      '
      Me.NumericUpDown4.Location = New System.Drawing.Point(328, 160)
      Me.NumericUpDown4.Maximum = New Decimal(New Integer() {3000, 0, 0, 0})
      Me.NumericUpDown4.Minimum = New Decimal(New Integer() {1970, 0, 0, 0})
      Me.NumericUpDown4.Name = "NumericUpDown4"
      Me.NumericUpDown4.Size = New System.Drawing.Size(48, 20)
      Me.NumericUpDown4.TabIndex = 5
      Me.NumericUpDown4.Value = New Decimal(New Integer() {1970, 0, 0, 0})
      '
      'NumericUpDown5
      '
      Me.NumericUpDown5.Location = New System.Drawing.Point(232, 160)
      Me.NumericUpDown5.Maximum = New Decimal(New Integer() {12, 0, 0, 0})
      Me.NumericUpDown5.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
      Me.NumericUpDown5.Name = "NumericUpDown5"
      Me.NumericUpDown5.Size = New System.Drawing.Size(32, 20)
      Me.NumericUpDown5.TabIndex = 3
      Me.NumericUpDown5.Value = New Decimal(New Integer() {1, 0, 0, 0})
      '
      'Label12
      '
      Me.Label12.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label12.Location = New System.Drawing.Point(232, 144)
      Me.Label12.Name = "Label12"
      Me.Label12.Size = New System.Drawing.Size(36, 15)
      Me.Label12.TabIndex = 29
      Me.Label12.Text = "Mon"
      '
      'NumericUpDown6
      '
      Me.NumericUpDown6.Location = New System.Drawing.Point(280, 160)
      Me.NumericUpDown6.Maximum = New Decimal(New Integer() {31, 0, 0, 0})
      Me.NumericUpDown6.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
      Me.NumericUpDown6.Name = "NumericUpDown6"
      Me.NumericUpDown6.Size = New System.Drawing.Size(32, 20)
      Me.NumericUpDown6.TabIndex = 4
      Me.NumericUpDown6.Value = New Decimal(New Integer() {1, 0, 0, 0})
      '
      'Label13
      '
      Me.Label13.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label13.Location = New System.Drawing.Point(280, 145)
      Me.Label13.Name = "Label13"
      Me.Label13.Size = New System.Drawing.Size(36, 15)
      Me.Label13.TabIndex = 31
      Me.Label13.Text = "Day"
      '
      'Label14
      '
      Me.Label14.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label14.Location = New System.Drawing.Point(232, 32)
      Me.Label14.Name = "Label14"
      Me.Label14.Size = New System.Drawing.Size(36, 15)
      Me.Label14.TabIndex = 32
      Me.Label14.Text = "Mon"
      '
      'ListBox6
      '
      Me.ListBox6.Location = New System.Drawing.Point(232, 48)
      Me.ListBox6.Name = "ListBox6"
      Me.ListBox6.Size = New System.Drawing.Size(32, 17)
      Me.ListBox6.TabIndex = 0
      '
      'Label15
      '
      Me.Label15.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label15.Location = New System.Drawing.Point(280, 32)
      Me.Label15.Name = "Label15"
      Me.Label15.Size = New System.Drawing.Size(36, 15)
      Me.Label15.TabIndex = 34
      Me.Label15.Text = "Day"
      '
      'ListBox7
      '
      Me.ListBox7.Location = New System.Drawing.Point(280, 48)
      Me.ListBox7.Name = "ListBox7"
      Me.ListBox7.Size = New System.Drawing.Size(32, 17)
      Me.ListBox7.TabIndex = 0
      '
      'Form1
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(440, 304)
      Me.Controls.Add(Me.ListBox7)
      Me.Controls.Add(Me.Label15)
      Me.Controls.Add(Me.ListBox6)
      Me.Controls.Add(Me.Label14)
      Me.Controls.Add(Me.Label13)
      Me.Controls.Add(Me.NumericUpDown6)
      Me.Controls.Add(Me.Label12)
      Me.Controls.Add(Me.NumericUpDown5)
      Me.Controls.Add(Me.NumericUpDown4)
      Me.Controls.Add(Me.NumericUpDown3)
      Me.Controls.Add(Me.NumericUpDown2)
      Me.Controls.Add(Me.NumericUpDown1)
      Me.Controls.Add(Me.StatusBar1)
      Me.Controls.Add(Me.Button1)
      Me.Controls.Add(Me.Label11)
      Me.Controls.Add(Me.Label10)
      Me.Controls.Add(Me.Label9)
      Me.Controls.Add(Me.Label8)
      Me.Controls.Add(Me.Label7)
      Me.Controls.Add(Me.ListBox5)
      Me.Controls.Add(Me.Label6)
      Me.Controls.Add(Me.ListBox4)
      Me.Controls.Add(Me.Label5)
      Me.Controls.Add(Me.ListBox3)
      Me.Controls.Add(Me.Label4)
      Me.Controls.Add(Me.Label3)
      Me.Controls.Add(Me.ListBox2)
      Me.Controls.Add(Me.Label2)
      Me.Controls.Add(Me.Label1)
      Me.Controls.Add(Me.ListBox1)
      Me.Controls.Add(Me.SearchButton)
      Me.Name = "Form1"
      Me.Text = "Get 1-Wire Addresses VB.NET"
      CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.NumericUpDown5, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.NumericUpDown6, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)

   End Sub

#End Region


   Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchButton.Click
      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim i As Integer

      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter

         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         StatusBar1.Text = "Processing...."
         StatusBar1.Text.ToString()

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers

         ListBox1.ClearSelected()
         ListBox1.Items.Clear()

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         '
         While owd_enum.hasMoreElements()
            StatusBar1.Text = " "
            StatusBar1.Text.ToString()

            ' retrieve OneWireContainer
            owd = owd_enum.nextElement()

            If TypeOf owd Is com.dalsemi.onewire.container.ClockContainer Then
               ListBox1.Items.Add(owd.getAddressAsString)
            End If

         End While

         For i = 0 To (ListBox1.Items.Count - 1)
            ListBox1.Items.Item(i).ToString()
         Next

         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()
         adapter.freePort()
      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try

   End Sub


   Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

   End Sub

   Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim dt As java.util.Date
      Dim cc As com.dalsemi.onewire.container.ClockContainer
      Dim ows As com.dalsemi.onewire.container.OneWireSensor
      Dim msec As Long
      Dim mon As Integer
      Dim yr As Integer

      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter

         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         '
         owd = adapter.getDeviceContainer(ListBox1.Items.Item(ListBox1.SelectedIndex).ToString())
         cc = CType(owd, com.dalsemi.onewire.container.ClockContainer)
         ows = CType(owd, com.dalsemi.onewire.container.OneWireSensor)

         msec = cc.getClock(ows.readDevice)
         dt = New java.util.Date(msec)

         'Dim result As java.util.Calendar
         'result = java.util.Calendar.getInstance()
         'result.set(java.util.Calendar.YEAR, com.dalsemi.onewire.utils.Convert.toInt(state, 4, 1))
         'result.set(java.util.Calendar.MONTH, com.dalsemi.onewire.utils.Convert.toInt(state, 5, 1) - 1)
         'result.set(java.util.Calendar.DATE, com.dalsemi.onewire.utils.Convert.toInt(state, 6, 1))
         'result.set(java.util.Calendar.AM_PM, java.util.Calendar.AM)
         'result.set(java.util.Calendar.HOUR, com.dalsemi.onewire.utils.Convert.toInt(state, 2, 1))
         'result.set(java.util.Calendar.MINUTE, com.dalsemi.onewire.utils.Convert.toInt(state, 1, 1))
         'result.set(java.util.Calendar.SECOND, com.dalsemi.onewire.utils.Convert.toInt(state, 0, 1))
         'Dim dt2 As java.util.Date
         'dt2 = result.getTime()
         'Dim msec2 As Long
         'msec2 = dt2.getTime()

         'StatusBar1.Text = "hour: " + state(2).ToString + " or " + dt.getHours().ToString() + " ms=" + msec.ToString() + " dt=" + dt.ToString()
         'StatusBar1.Text.ToString()

         ListBox2.ClearSelected()
         ListBox2.Items.Clear()
         ListBox2.Items.Add(dt.getHours.ToString)
         ListBox2.Items.Item(0).ToString()

         ListBox3.ClearSelected()
         ListBox3.Items.Clear()
         ListBox3.Items.Add(dt.getMinutes.ToString)
         ListBox3.Items.Item(0).ToString()

         ListBox4.ClearSelected()
         ListBox4.Items.Clear()
         ListBox4.Items.Add(dt.getSeconds.ToString)
         ListBox4.Items.Item(0).ToString()

         yr = dt.getYear + 1900

         ListBox5.ClearSelected()
         ListBox5.Items.Clear()
         ListBox5.Items.Add(yr.ToString)
         ListBox5.Items.Item(0).ToString()

         mon = dt.getMonth + 1

         ListBox6.ClearSelected()
         ListBox6.Items.Clear()
         ListBox6.Items.Add(mon.ToString)
         ListBox6.Items.Item(0).ToString()

         ListBox7.ClearSelected()
         ListBox7.Items.Clear()
         ListBox7.Items.Add(dt.getDate.ToString)
         ListBox7.Items.Item(0).ToString()

         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()
         adapter.freePort()

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try

   End Sub

   Private Sub StatusBar1_PanelClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.StatusBarPanelClickEventArgs) Handles StatusBar1.PanelClick

   End Sub

   Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

   End Sub

   Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim dtold As java.util.Date
      Dim dtnew As java.util.Date
      Dim cc As com.dalsemi.onewire.container.ClockContainer
      Dim ows As com.dalsemi.onewire.container.OneWireSensor
      Dim msec As Long
      Dim state(32) As System.SByte

      Dim j As Integer

      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter

         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         owd = adapter.getDeviceContainer(ListBox1.Items.Item(ListBox1.SelectedIndex).ToString())
         cc = CType(owd, com.dalsemi.onewire.container.ClockContainer)
         ows = CType(owd, com.dalsemi.onewire.container.OneWireSensor)

         msec = cc.getClock(ows.readDevice)

         dtold = New java.util.Date(msec)

         dtnew = New java.util.Date

         j = Integer.Parse(NumericUpDown1.Value.ToString)
         dtnew.setHours(j)

         j = Integer.Parse(NumericUpDown2.Value.ToString)
         dtnew.setMinutes(j)

         j = Integer.Parse(NumericUpDown3.Value.ToString)
         dtnew.setSeconds(j)

         j = Integer.Parse(NumericUpDown4.Value.ToString)
         j = j - 1900
         dtnew.setYear(j)

         j = Integer.Parse(NumericUpDown5.Value.ToString)
         j = j - 1
         dtnew.setMonth(j)

         j = Integer.Parse(NumericUpDown6.Value.ToString)
         dtnew.setDate(j)

         state = ows.readDevice

         cc.setClock(dtnew.getTime(), state)
         ows.writeDevice(state)

         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()
         adapter.freePort()

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try

   End Sub

   Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged

   End Sub
End Class
