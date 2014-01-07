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

Public Class FormSecureKey
   Dim adapter As com.dalsemi.onewire.adapter.DSPortAdapter
   ' keep in mind SByte arrays in VB.NET are padded by 1 byte
   Dim passwordBuf(7) As SByte
   Dim idBuf(7) As SByte
   Dim writeBuf(10) As SByte
   Dim subkeyBuf(63) As SByte

   Private Sub FormSecureKey_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter

         ' print that we got an adapter
         ResultsTextBox.AppendText(Environment.NewLine() & "Adapter: " & adapter.getAdapterName & " Port: " & adapter.getPortName & Environment.NewLine() & Environment.NewLine())

         passwordBuf(0) = Asc("p")
         passwordBuf(1) = Asc("a")
         passwordBuf(2) = Asc("s")
         passwordBuf(3) = Asc("s")
         passwordBuf(4) = Asc("w")
         passwordBuf(5) = Asc("o")
         passwordBuf(6) = Asc("r")
         passwordBuf(7) = Asc("d")

         ' set idBuf to ascii string "ds1991id" -- w/o quotes 
         idBuf(0) = Asc("d")
         idBuf(1) = Asc("s")
         idBuf(2) = Asc("1")
         idBuf(3) = Asc("9")
         idBuf(4) = Asc("9")
         idBuf(5) = Asc("1")
         idBuf(6) = Asc("i")
         idBuf(7) = Asc("d")

         ' set writeBuf to ascii string "testing 123" -- w/o quotes 
         writeBuf(0) = Asc("t")
         writeBuf(1) = Asc("e")
         writeBuf(2) = Asc("s")
         writeBuf(3) = Asc("t")
         writeBuf(4) = Asc("i")
         writeBuf(5) = Asc("n")
         writeBuf(6) = Asc("g")
         writeBuf(7) = Asc(" ")
         writeBuf(8) = Asc("1")
         writeBuf(9) = Asc("2")
         writeBuf(10) = Asc("3")

      Catch ex As Exception
         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine() & "Error:  " & ex.ToString)
      End Try
   End Sub

   Private Sub WriteDS1991Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WriteDS1991Button.Click
      ' Clicking this button retrieves all 1-Wire parts found on a 1-Wire
      ' network and if a DS1991 is found, then attempts to write to it.
      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim owd02 As com.dalsemi.onewire.container.OneWireContainer02
      Dim I As Integer

      Try
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers
         ResultsTextBox.AppendText("1-Wire List:  " & Environment.NewLine())
         ResultsTextBox.AppendText("==========================" & Environment.NewLine())
         While owd_enum.hasMoreElements
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement
            ' retrieve OneWireAddress
            ResultsTextBox.AppendText(Environment.NewLine())
            ResultsTextBox.AppendText("Address = " & owd.getAddressAsString & Environment.NewLine())
            ' check to see if 1-Wire device is a DS1991'
            If owd.getName = "DS1991" Then
               ResultsTextBox.AppendText("Part Number: " & owd.getName & Environment.NewLine())
               ' cast the OneWireContainer to OneWireContainer02
               owd02 = DirectCast(owd, com.dalsemi.onewire.container.OneWireContainer02)
               For I = 0 To 2  ' writes to subkey'
                  Call owd02.writeSubkey(I, 16, passwordBuf, writeBuf)
               Next I
               ResultsTextBox.AppendText("Writing 'testing 123' to each subkey: SUCCESS!" & Environment.NewLine())
            End If

         End While ' End OneWireContainer enumeration loop


         ' end exclusive use of adapter
         adapter.endExclusive()

         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine())

      Catch ex As Exception
         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine() & "Error:  " & ex.ToString)
      End Try

   End Sub

   Private Sub SetAllDS1991Passwords_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SetAllDS1991Passwords.Click
      ' Clicking this button retrieves all 1-Wire parts found on a 1-Wire
      ' network and lists their addresses and sets all 1991 ids and passwords.

      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim owd02 As com.dalsemi.onewire.container.OneWireContainer02
      Dim I As Integer
      Dim J As Integer
      Dim oldidBuf(7) As SByte 'VB.NET will pad an extra SByte

      ' initialize subkeyBuf to zeroes
      For I = 0 To 63
         subkeyBuf(I) = 0
      Next I
      ' initialize oldidBuf to zeroes
      For I = 0 To 7
         oldidBuf(I) = 0
      Next I

      Try
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers
         ResultsTextBox.AppendText("1-Wire List:  " & Environment.NewLine())
         ResultsTextBox.AppendText("==========================" & Environment.NewLine())
         While owd_enum.hasMoreElements
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement
            ' retrieve OneWireAddress
            ResultsTextBox.AppendText(Environment.NewLine())
            ResultsTextBox.AppendText("Address = " & owd.getAddressAsString & Environment.NewLine())
            ' check to see if 1-Wire device is a DS1991'
            If owd.getName = "DS1991" Then
               ResultsTextBox.AppendText("Part Number: " & owd.getName & Environment.NewLine())
               ' cast the OneWireContainer to OneWireContainer02
               owd02 = DirectCast(owd, com.dalsemi.onewire.container.OneWireContainer02)
               For I = 0 To 2  ' writes new password and id for each subkey'
                  ' first get the old id from reading the subkey
                  For J = 0 To 7
                     ResultsTextBox.AppendText(passwordBuf(J) & " ")
                  Next J
                  ResultsTextBox.AppendText(Environment.NewLine())
                  ResultsTextBox.AppendText("password length = " & passwordBuf.Length() & Environment.NewLine())
                  subkeyBuf = owd02.readSubkey(I, passwordBuf)
                  For J = 0 To 7
                     oldidBuf(J) = subkeyBuf(J)
                  Next J
                  owd02.writePassword(I, oldidBuf, idBuf, passwordBuf)
               Next I
               ResultsTextBox.AppendText("Writing new id and password: SUCCESS!" & Environment.NewLine())
            End If

         End While ' End OneWireContainer enumeration loop


         ' end exclusive use of adapter
         adapter.endExclusive()

         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine())

      Catch ex As Exception
         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine() & "Error:  " & ex.ToString)
      End Try
   End Sub

   Private Sub ReadDS1991Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReadDS1991Button.Click
      ' Clicking this button retrieves all 1-Wire parts found on a 1-Wire
      ' network and lists their addresses and prints out their subkeys if
      ' they are DS1991s.

      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim owd02 As com.dalsemi.onewire.container.OneWireContainer02
      Dim I As Integer
      Dim J As Integer

      Try
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers
         ResultsTextBox.AppendText("1-Wire List:  " & Environment.NewLine())
         ResultsTextBox.AppendText("==========================" & Environment.NewLine())
         While owd_enum.hasMoreElements
            ' retrieve OneWireContainer
            owd = owd_enum.nextElement
            ' retrieve OneWireAddress
            ResultsTextBox.AppendText(Environment.NewLine())
            ResultsTextBox.AppendText("Address = " & owd.getAddressAsString & Environment.NewLine())
            ' check to see if 1-Wire device is a DS1991'
            If owd.getName = "DS1991" Then
               ResultsTextBox.AppendText("Part Number: " & owd.getName & Environment.NewLine())
               ' cast the OneWireContainer to OneWireContainer02
               owd02 = DirectCast(owd, com.dalsemi.onewire.container.OneWireContainer02)

               For I = 0 To 2  ' writes new password and id for each subkey'
                  subkeyBuf = owd02.readSubkey(I, passwordBuf)
                  ResultsTextBox.AppendText(" Subkey " & I & ": ")
                  For J = 0 To 47
                     ' to convert to ascii characters --> Chr(subkeyBuf.getAt(J))
                     ResultsTextBox.AppendText(subkeyBuf(J) & " ")
                  Next J
                  ResultsTextBox.AppendText(Environment.NewLine())
               Next I
            End If
         End While ' End OneWireContainer enumeration loop


         ' end exclusive use of adapter
         adapter.endExclusive()

         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine())

      Catch ex As Exception
         ResultsTextBox.AppendText(Environment.NewLine() & Environment.NewLine() & "Error:  " & ex.ToString)
      End Try
   End Sub
End Class
