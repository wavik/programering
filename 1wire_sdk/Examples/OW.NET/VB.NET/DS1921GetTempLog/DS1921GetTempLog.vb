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

Public Class TempLog
   Inherits System.Windows.Forms.Form
   ' a few global variables
   Dim CRLF As String = Environment.NewLine ' makes a carriage-return-linefeed string to begin a new line in a text box
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
   Friend WithEvents Get_Temp_Log_Button As System.Windows.Forms.Button
   Friend WithEvents ListBoxTempLog As System.Windows.Forms.ListBox
   Friend WithEvents TempLog_Clear_Button As System.Windows.Forms.Button
   Friend WithEvents StatusBar1 As System.Windows.Forms.StatusBar
   Friend WithEvents TempLog_MainMenu As System.Windows.Forms.MainMenu
   Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
   Friend WithEvents Save_To_File_MenuItem As System.Windows.Forms.MenuItem
   <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.components = New System.ComponentModel.Container
      Me.Get_Temp_Log_Button = New System.Windows.Forms.Button
      Me.ListBoxTempLog = New System.Windows.Forms.ListBox
      Me.TempLog_Clear_Button = New System.Windows.Forms.Button
      Me.StatusBar1 = New System.Windows.Forms.StatusBar
      Me.TempLog_MainMenu = New System.Windows.Forms.MainMenu(Me.components)
      Me.MenuItem1 = New System.Windows.Forms.MenuItem
      Me.Save_To_File_MenuItem = New System.Windows.Forms.MenuItem
      Me.SuspendLayout()
      '
      'Get_Temp_Log_Button
      '
      Me.Get_Temp_Log_Button.Location = New System.Drawing.Point(360, 80)
      Me.Get_Temp_Log_Button.Name = "Get_Temp_Log_Button"
      Me.Get_Temp_Log_Button.Size = New System.Drawing.Size(80, 48)
      Me.Get_Temp_Log_Button.TabIndex = 1
      Me.Get_Temp_Log_Button.Text = "&Get Temperature Log"
      '
      'ListBoxTempLog
      '
      Me.ListBoxTempLog.Location = New System.Drawing.Point(48, 32)
      Me.ListBoxTempLog.Name = "ListBoxTempLog"
      Me.ListBoxTempLog.Size = New System.Drawing.Size(296, 225)
      Me.ListBoxTempLog.TabIndex = 2
      '
      'TempLog_Clear_Button
      '
      Me.TempLog_Clear_Button.Location = New System.Drawing.Point(360, 160)
      Me.TempLog_Clear_Button.Name = "TempLog_Clear_Button"
      Me.TempLog_Clear_Button.Size = New System.Drawing.Size(80, 48)
      Me.TempLog_Clear_Button.TabIndex = 3
      Me.TempLog_Clear_Button.Text = "&Clear"
      '
      'StatusBar1
      '
      Me.StatusBar1.Location = New System.Drawing.Point(0, 288)
      Me.StatusBar1.Name = "StatusBar1"
      Me.StatusBar1.Size = New System.Drawing.Size(456, 22)
      Me.StatusBar1.TabIndex = 4
      Me.StatusBar1.Text = "Port:  "
      '
      'TempLog_MainMenu
      '
      Me.TempLog_MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1})
      '
      'MenuItem1
      '
      Me.MenuItem1.Index = 0
      Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.Save_To_File_MenuItem})
      Me.MenuItem1.Text = "&File"
      '
      'Save_To_File_MenuItem
      '
      Me.Save_To_File_MenuItem.Index = 0
      Me.Save_To_File_MenuItem.Text = "&Save To File"
      '
      'TempLog
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(456, 310)
      Me.Controls.Add(Me.StatusBar1)
      Me.Controls.Add(Me.TempLog_Clear_Button)
      Me.Controls.Add(Me.ListBoxTempLog)
      Me.Controls.Add(Me.Get_Temp_Log_Button)
      Me.Menu = Me.TempLog_MainMenu
      Me.Name = "TempLog"
      Me.Text = "DS1921 Get Temperature Log"
      Me.ResumeLayout(False)

   End Sub

#End Region

   Private Sub Get_Temp_Log_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Get_Temp_Log_Button.Click

      Dim state As Object, log As [SByte]()
      Dim i As Integer

      ' We already have the adapter from initialization code in the 
      ' Form "load" event below...

      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim owd21 As com.dalsemi.onewire.container.OneWireContainer21

      ' time variables
      Dim time_stamp As java.util.Calendar
      Dim time As Long
      Dim sample_rate As Integer
      Dim newDate As java.util.Date

      Try
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' setup adapter to search for DS1921 Thermochrons
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)
         adapter.targetFamily(&H21)
         adapter.setSearchAllDevices()

         ' Get all device containers
         owd_enum = adapter.getAllDeviceContainers
         ListBoxTempLog.Items.Add("DS1921 List:")
         ' enumerate through all the DS1921s found (with Java-style enumeration)
         While owd_enum.hasMoreElements()
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement()
            owd21 = owd
            ' Set owd21 to max possible speed with available adapter, allow fall back
            If (adapter.canOverdrive() And owd21.getMaxSpeed() = com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_OVERDRIVE) Then
               owd21.setSpeed(owd21.getMaxSpeed(), True)
            End If

            state = owd21.readDevice()
            ListBoxTempLog.Items.Add("")
            ListBoxTempLog.Items.Add("Address = " & owd21.getAddressAsString & CRLF)
            ListBoxTempLog.Items.Add("  Name = " & owd21.getName & CRLF)
            ListBoxTempLog.Items.Add("  ==== Temp Log Download in °C ===" & CRLF)

            log = owd21.getTemperatureLog(state)
            time_stamp = owd21.getMissionTimeStamp(state)
            time = time_stamp.getTime().getTime() + owd21.getFirstLogOffset(state)
            sample_rate = owd21.getSampleRate(state)

            If log.Length > 0 Then
               For i = 0 To (log.Length - 1)
                  newDate = New java.util.Date(time)
                  ListBoxTempLog.Items.Add("  " & newDate.ToString & " " & owd21.decodeTemperature(log(i)) & " °C" & CRLF)
                  time += sample_rate * 60 * 1000
               Next
            End If

         End While
         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()
      Catch ex As Exception
         ListBoxTempLog.Items.Add("")
         ListBoxTempLog.Items.Add("")
         ListBoxTempLog.Items.Add("Error:  " & ex.ToString & CRLF)
      End Try

   End Sub

   Private Sub TempLog_Clear_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TempLog_Clear_Button.Click
      ListBoxTempLog.Items.Clear()
   End Sub

   ' Saves list box items to a file through a "Save Dialog"
   Private Sub Save_To_File_MenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save_To_File_MenuItem.Click
      ' Displays a SaveFileDialog so the user can save the ListBox results

      Dim SaveDialog As SaveFileDialog = New SaveFileDialog()
      Dim SW As System.IO.StreamWriter, i As Integer, Temp As String
      Try
         SaveDialog.InitialDirectory = Application.StartupPath
         SaveDialog.Title = "Save the Temperature Log Text File"
         SaveDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
         SaveDialog.FilterIndex = 1

         If SaveDialog.ShowDialog() = DialogResult.OK Then
            SW = IO.File.CreateText(SaveDialog.FileName.ToString)
            For i = 0 To ListBoxTempLog.Items.Count - 1
               Temp = ListBoxTempLog.Items.Item(i)
               SW.Write(Temp)
            Next
            SW.Flush()
            SW.Close()
         End If
      Catch ex As Exception
         ListBoxTempLog.Items.Add("")
         ListBoxTempLog.Items.Add("")
         ListBoxTempLog.Items.Add("Error:  " & ex.ToString & CRLF)
      End Try

   End Sub

   Private Sub TempLog_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
      ' free adapter
      adapter.freePort()
   End Sub

   ' Form "load" event
   Private Sub TempLog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter

         ' print that we got an adapter
         ListBoxTempLog.Items.Add("")
         ListBoxTempLog.Items.Add("Adapter: " & adapter.getAdapterName & " Port: " & adapter.getPortName)
         StatusBar1.Text = "Adapter: " & adapter.getAdapterName() & " Port: " & adapter.getPortName() & " "
         ListBoxTempLog.Items.Add("")
         ListBoxTempLog.Items.Add("")

      Catch ex As Exception
         ListBoxTempLog.Items.Add("")
         ListBoxTempLog.Items.Add("")
         ListBoxTempLog.Items.Add("Error:  " & ex.ToString)
      End Try
   End Sub
End Class
