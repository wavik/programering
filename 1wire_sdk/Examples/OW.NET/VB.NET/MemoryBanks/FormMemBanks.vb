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

Public Class FormMemBanks
   Inherits System.Windows.Forms.Form
   Dim adapter As com.dalsemi.onewire.adapter.DSPortAdapter


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
   Friend WithEvents TextBoxResults As System.Windows.Forms.TextBox
   Friend WithEvents ButtonReadMemBanks As System.Windows.Forms.Button
   Friend WithEvents ButtonWriteMemBanks As System.Windows.Forms.Button
   <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.TextBoxResults = New System.Windows.Forms.TextBox
      Me.ButtonReadMemBanks = New System.Windows.Forms.Button
      Me.ButtonWriteMemBanks = New System.Windows.Forms.Button
      Me.SuspendLayout()
      '
      'TextBoxResults
      '
      Me.TextBoxResults.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.TextBoxResults.Location = New System.Drawing.Point(32, 32)
      Me.TextBoxResults.MaxLength = 128000
      Me.TextBoxResults.Multiline = True
      Me.TextBoxResults.Name = "TextBoxResults"
      Me.TextBoxResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
      Me.TextBoxResults.Size = New System.Drawing.Size(336, 208)
      Me.TextBoxResults.TabIndex = 0
      '
      'ButtonReadMemBanks
      '
      Me.ButtonReadMemBanks.Location = New System.Drawing.Point(376, 72)
      Me.ButtonReadMemBanks.Name = "ButtonReadMemBanks"
      Me.ButtonReadMemBanks.Size = New System.Drawing.Size(128, 40)
      Me.ButtonReadMemBanks.TabIndex = 1
      Me.ButtonReadMemBanks.Text = "&Read Memory Banks"
      '
      'ButtonWriteMemBanks
      '
      Me.ButtonWriteMemBanks.Location = New System.Drawing.Point(376, 144)
      Me.ButtonWriteMemBanks.Name = "ButtonWriteMemBanks"
      Me.ButtonWriteMemBanks.Size = New System.Drawing.Size(128, 72)
      Me.ButtonWriteMemBanks.TabIndex = 2
      Me.ButtonWriteMemBanks.Text = "&Write ""Testing 123"" to Each General Purpose  Memory Bank Page"
      '
      'FormMemBanks
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.AutoScroll = True
      Me.ClientSize = New System.Drawing.Size(512, 270)
      Me.Controls.Add(Me.ButtonWriteMemBanks)
      Me.Controls.Add(Me.ButtonReadMemBanks)
      Me.Controls.Add(Me.TextBoxResults)
      Me.Name = "FormMemBanks"
      Me.Text = "Memory Banks Read/Write for Main and Data Memory Banks"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub

#End Region

   Private Sub ButtonReadMemBanks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonReadMemBanks.Click
      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim mb As Object
      Dim mb_enum As java.util.Enumeration
      Dim memoryBanksFound As Integer
      Dim isfirst As Boolean
      Dim I As Integer
      Dim numOfChars As Integer
      Dim pageData As String
      Dim half_len As Integer
      Dim quarter_len As Integer

      ' get exclusive use of 1-Wire network
      adapter.beginExclusive(True)
      Try

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers
         TextBoxResults.AppendText("1-Wire List:" & Environment.NewLine)
         TextBoxResults.AppendText("==========================" & Environment.NewLine)
         memoryBanksFound = 0
         While owd_enum.hasMoreElements
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement
            ' Set owd to max possible speed with available adapter, allow fall back
            If (adapter.canOverdrive() And owd.getMaxSpeed() = com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_OVERDRIVE) Then
               owd.setSpeed(owd.getMaxSpeed(), True)
            End If
            ' retrieve OneWireAddress
            TextBoxResults.AppendText(Environment.NewLine & "Address = " & owd.getAddressAsString & Environment.NewLine)

            ' check to see if 1-Wire device supports Memory Banks, if so get address.
            mb_enum = owd.getMemoryBanks
            While mb_enum.hasMoreElements
               ' retrieve MemoryBanks
               memoryBanksFound = 1
               mb = mb_enum.nextElement
               TextBoxResults.AppendText(" Memory Bank:  " & (mb.getBankDescription) & Environment.NewLine)
               ' Now, let's read out the MemoryBanks'
               Dim readBuf(mb.getPageLength) As [SByte]
               Dim extraInfo(mb.getExtraInfoLength) As [SByte]
               TextBoxResults.AppendText("   PageLength:      " & mb.getPageLength & Environment.NewLine)
               TextBoxResults.AppendText("   ExtraInfoLength: " & mb.getExtraInfoLength & Environment.NewLine)

               isfirst = False
               ' loop to read all of the pages
               For I = 0 To mb.getNumberPages - 1
                  If mb.haveExtraInfo Then
                     mb.readPage(I, isfirst, readBuf, 0, extraInfo)
                  Else
                     mb.readPage(I, isfirst, readBuf, 0)
                  End If
                  pageData = com.dalsemi.onewire.utils.Convert.toHexString(readBuf)
                  numOfChars = mb.getPageLength * 2
                  ' format string to display nicely 16 chars at a time
                  If mb.getPageLength > 32 Then
                     quarter_len = numOfChars / 4
                     TextBoxResults.AppendText(" Page " & Format(I, "000") & ": " & pageData.Substring(0, quarter_len) & Environment.NewLine)
                     TextBoxResults.AppendText("           " & pageData.Substring(quarter_len, quarter_len) & Environment.NewLine)
                     TextBoxResults.AppendText("           " & pageData.Substring(quarter_len * 2, quarter_len) & Environment.NewLine)
                     TextBoxResults.AppendText("           " & pageData.Substring(quarter_len * 3, quarter_len) & Environment.NewLine)
                  End If
                  If (mb.getPageLength <= 32) And (mb.getPageLength > 16) Then
                     half_len = numOfChars / 2
                     TextBoxResults.AppendText(" Page " & Format(I, "000") & ": " & pageData.Substring(0, half_len) & Environment.NewLine)
                     TextBoxResults.AppendText("           " & pageData.Substring(half_len, half_len) & Environment.NewLine)
                  End If
                  If mb.getPageLength <= 16 Then
                     TextBoxResults.AppendText(" Page " & Format(I, "000") & ": " & pageData.Substring(0, numOfChars) & Environment.NewLine)
                  End If
                  If mb.haveExtraInfo Then  ' check for existence of Extra Info'
                     TextBoxResults.AppendText("           Extra: " & com.dalsemi.onewire.utils.Convert.toHexString(extraInfo) & Environment.NewLine)
                  End If
                  isfirst = True
               Next I
            End While ' End Memory Bank enumeration loop
         End While ' End OneWireContainer enumeration loop

         ' show "No Memory Banks Found" message
         If (memoryBanksFound = 0) Then
            TextBoxResults.AppendText(Environment.NewLine & "No 1-Wire devices with a Memory Bank found!" & Environment.NewLine)
         Else
            TextBoxResults.AppendText(Environment.NewLine & "Finished Reading..." & Environment.NewLine)
         End If


      Catch ex As Exception
         TextBoxResults.AppendText(Environment.NewLine & Environment.NewLine & "Error:  " & ex.ToString & Environment.NewLine)
      End Try

      ' end exclusive use of adapter
      adapter.endExclusive()

      TextBoxResults.Text = TextBoxResults.Text & Environment.NewLine & Environment.NewLine

   End Sub

   Private Sub FormMemBanks_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      'Add any initialization after the InitializeComponent() call
      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter
         ' print that we got an adapter
         TextBoxResults.AppendText(Environment.NewLine & "Adapter: " & adapter.getAdapterName & " Port: " & adapter.getPortName & Environment.NewLine & Environment.NewLine)
      Catch ex As Exception
         TextBoxResults.AppendText(Environment.NewLine & Environment.NewLine & "Error:  " & ex.ToString)
      End Try

   End Sub

   Private Sub ButtonWriteMemBanks_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonWriteMemBanks.Click

      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim mb As Object
      Dim mb_enum As java.util.Enumeration
      Dim memoryBanksFound As Integer
      Dim I As Integer


      ' write out message that we are attempting to write to the part
      TextBoxResults.AppendText(Environment.NewLine & "Attempting to write ''Testing 123'' to memory pages" & Environment.NewLine & Environment.NewLine)
      ' get exclusive use of 1-Wire network
      adapter.beginExclusive(True)
      Try
         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers
         TextBoxResults.AppendText("1-Wire List:" & Environment.NewLine)
         TextBoxResults.AppendText("========================" & Environment.NewLine)
         memoryBanksFound = 0
         While owd_enum.hasMoreElements
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement
            ' Set owd to max possible speed with available adapter, allow fall back
            If (adapter.canOverdrive() And owd.getMaxSpeed() = com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_OVERDRIVE) Then
               owd.setSpeed(owd.getMaxSpeed(), True)
            End If
            ' retrieve OneWireAddress
            TextBoxResults.AppendText("Address = " & owd.getAddressAsString & Environment.NewLine)
            ' check to see if 1-Wire device supports Memory Banks, if so get address.
            mb_enum = owd.getMemoryBanks
            While mb_enum.hasMoreElements
               ' retrieve MemoryBanks
               mb = mb_enum.nextElement

               memoryBanksFound = 1
               If (mb.isGeneralPurposeMemory) Then


                  TextBoxResults.AppendText(" Found a General Purpose MemoryBank, writing..." & Environment.NewLine)

                  ' Now, let's write to the General Purpose Memory Bank, page by page'
                  Dim writeBuf(11) As [SByte]

                  writeBuf(0) = Convert.ToSByte(Asc("T"))
                  writeBuf(1) = Convert.ToSByte(Asc("E"))
                  writeBuf(2) = Convert.ToSByte(Asc("S"))
                  writeBuf(3) = Convert.ToSByte(Asc("T"))
                  writeBuf(4) = Convert.ToSByte(Asc("I"))
                  writeBuf(5) = Convert.ToSByte(Asc("N"))
                  writeBuf(6) = Convert.ToSByte(Asc("G"))
                  writeBuf(7) = Convert.ToSByte(Asc(" "))
                  writeBuf(8) = Convert.ToSByte(Asc("1"))
                  writeBuf(9) = Convert.ToSByte(Asc("2"))
                  writeBuf(10) = Convert.ToSByte(Asc("3"))
                  ' if you need to write a byte that should be unsigned, say 0xFF, use the following method:
                  ' System.Buffer.SetByte(writeBuf, 10, &HFF).  This writes 0xFF to the array "writeBuf" at index 10,
                  ' even though the value of 0xFF would normally be out-of-range for an SByte.

                  Dim isfirst As Boolean
                  isfirst = False
                  ' loop to write all of the pages
                  ' check to see if this is an EPROM or other One Time Programmable Memory Bank
                  If (mb.needsProgramPulse And (Not adapter.canProgram)) Then
                     TextBoxResults.AppendText(" Adapter does not support EPROM writing" & Environment.NewLine)
                  Else
                     For I = 0 To mb.getNumberPages - 1
                        Call mb.write((I * mb.getPageLength), writeBuf, 0, 11)
                        'mb.writePagePacket(I, writeBuf, 0, 11)
                        isfirst = True
                     Next I
                  End If
               End If
                End While ' End Memory Bank enumeration loop
         End While ' End OneWireContainer enumeration loop

         ' show "No Memory Banks Found" message
         If (memoryBanksFound = 0) Then
            TextBoxResults.AppendText(Environment.NewLine & "No 1-Wire devices with a General Purpose Memory Bank found!" & Environment.NewLine)
         Else
            TextBoxResults.AppendText(Environment.NewLine & "Finished Writing..." & Environment.NewLine)
         End If
      Catch ex As Exception
         TextBoxResults.AppendText(Environment.NewLine & Environment.NewLine & "Error:  " & ex.ToString & Environment.NewLine)
      End Try

      ' end exclusive use of adapter
      adapter.endExclusive()

      TextBoxResults.AppendText(Environment.NewLine & Environment.NewLine)

   End Sub
End Class
