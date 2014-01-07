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
   Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
   Friend WithEvents Button_Delete_Selected As System.Windows.Forms.Button
   Dim nodNew As TreeNode


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
   Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
   Friend WithEvents Label1 As System.Windows.Forms.Label
   Friend WithEvents Label2 As System.Windows.Forms.Label
   Friend WithEvents Button_Format As System.Windows.Forms.Button
   Friend WithEvents StatusBar1 As System.Windows.Forms.StatusBar
   Friend WithEvents Button_Search As System.Windows.Forms.Button
   Friend WithEvents Button_Read_Dir As System.Windows.Forms.Button
   Friend WithEvents Button_Make_New_Directory As System.Windows.Forms.Button
   Friend WithEvents Button_Read_File As System.Windows.Forms.Button
   Friend WithEvents Label3 As System.Windows.Forms.Label
   Friend WithEvents Label4 As System.Windows.Forms.Label
   Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
   Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
   Friend WithEvents Label5 As System.Windows.Forms.Label
   Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
   Friend WithEvents RichTextBox2 As System.Windows.Forms.RichTextBox
   Friend WithEvents Button_Create_New_File As System.Windows.Forms.Button
   Friend WithEvents Label6 As System.Windows.Forms.Label
   <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      Me.ListBox1 = New System.Windows.Forms.ListBox
      Me.Label1 = New System.Windows.Forms.Label
      Me.Label2 = New System.Windows.Forms.Label
      Me.Button_Format = New System.Windows.Forms.Button
      Me.StatusBar1 = New System.Windows.Forms.StatusBar
      Me.Button_Search = New System.Windows.Forms.Button
      Me.Button_Read_Dir = New System.Windows.Forms.Button
      Me.Button_Make_New_Directory = New System.Windows.Forms.Button
      Me.Button_Read_File = New System.Windows.Forms.Button
      Me.Label3 = New System.Windows.Forms.Label
      Me.Label4 = New System.Windows.Forms.Label
      Me.TreeView1 = New System.Windows.Forms.TreeView
      Me.TextBox1 = New System.Windows.Forms.TextBox
      Me.Label5 = New System.Windows.Forms.Label
      Me.RichTextBox1 = New System.Windows.Forms.RichTextBox
      Me.RichTextBox2 = New System.Windows.Forms.RichTextBox
      Me.Button_Create_New_File = New System.Windows.Forms.Button
      Me.Label6 = New System.Windows.Forms.Label
      Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown
      Me.Button_Delete_Selected = New System.Windows.Forms.Button
      CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'ListBox1
      '
      Me.ListBox1.Location = New System.Drawing.Point(24, 32)
      Me.ListBox1.Name = "ListBox1"
      Me.ListBox1.Size = New System.Drawing.Size(238, 108)
      Me.ListBox1.TabIndex = 3
      '
      'Label1
      '
      Me.Label1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label1.Location = New System.Drawing.Point(24, 8)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(120, 23)
      Me.Label1.TabIndex = 4
      Me.Label1.Text = "iButton List"
      '
      'Label2
      '
      Me.Label2.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label2.Location = New System.Drawing.Point(24, 160)
      Me.Label2.Name = "Label2"
      Me.Label2.Size = New System.Drawing.Size(120, 23)
      Me.Label2.TabIndex = 6
      Me.Label2.Text = "Directory/File List"
      '
      'Button_Format
      '
      Me.Button_Format.BackColor = System.Drawing.SystemColors.Control
      Me.Button_Format.Location = New System.Drawing.Point(302, 70)
      Me.Button_Format.Name = "Button_Format"
      Me.Button_Format.Size = New System.Drawing.Size(75, 23)
      Me.Button_Format.TabIndex = 10
      Me.Button_Format.Text = "&Format"
      Me.Button_Format.UseVisualStyleBackColor = False
      '
      'StatusBar1
      '
      Me.StatusBar1.Location = New System.Drawing.Point(0, 484)
      Me.StatusBar1.Name = "StatusBar1"
      Me.StatusBar1.Size = New System.Drawing.Size(748, 16)
      Me.StatusBar1.TabIndex = 24
      '
      'Button_Search
      '
      Me.Button_Search.BackColor = System.Drawing.SystemColors.Control
      Me.Button_Search.Location = New System.Drawing.Point(302, 32)
      Me.Button_Search.Name = "Button_Search"
      Me.Button_Search.Size = New System.Drawing.Size(75, 23)
      Me.Button_Search.TabIndex = 25
      Me.Button_Search.Text = "&Search"
      Me.Button_Search.UseVisualStyleBackColor = False
      '
      'Button_Read_Dir
      '
      Me.Button_Read_Dir.BackColor = System.Drawing.SystemColors.Control
      Me.Button_Read_Dir.Location = New System.Drawing.Point(302, 117)
      Me.Button_Read_Dir.Name = "Button_Read_Dir"
      Me.Button_Read_Dir.Size = New System.Drawing.Size(75, 23)
      Me.Button_Read_Dir.TabIndex = 26
      Me.Button_Read_Dir.Text = "&Read Dir"
      Me.Button_Read_Dir.UseVisualStyleBackColor = False
      '
      'Button_Make_New_Directory
      '
      Me.Button_Make_New_Directory.BackColor = System.Drawing.SystemColors.Control
      Me.Button_Make_New_Directory.Location = New System.Drawing.Point(283, 184)
      Me.Button_Make_New_Directory.Name = "Button_Make_New_Directory"
      Me.Button_Make_New_Directory.Size = New System.Drawing.Size(120, 23)
      Me.Button_Make_New_Directory.TabIndex = 27
      Me.Button_Make_New_Directory.Text = "&Make New Directory"
      Me.Button_Make_New_Directory.UseVisualStyleBackColor = False
      '
      'Button_Read_File
      '
      Me.Button_Read_File.BackColor = System.Drawing.SystemColors.Control
      Me.Button_Read_File.Location = New System.Drawing.Point(283, 292)
      Me.Button_Read_File.Name = "Button_Read_File"
      Me.Button_Read_File.Size = New System.Drawing.Size(120, 23)
      Me.Button_Read_File.TabIndex = 29
      Me.Button_Read_File.Text = "&Read File"
      Me.Button_Read_File.UseVisualStyleBackColor = False
      '
      'Label3
      '
      Me.Label3.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label3.Location = New System.Drawing.Point(426, 158)
      Me.Label3.Name = "Label3"
      Me.Label3.Size = New System.Drawing.Size(120, 23)
      Me.Label3.TabIndex = 34
      Me.Label3.Text = "Hex File View"
      '
      'Label4
      '
      Me.Label4.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label4.Location = New System.Drawing.Point(426, 326)
      Me.Label4.Name = "Label4"
      Me.Label4.Size = New System.Drawing.Size(120, 23)
      Me.Label4.TabIndex = 35
      Me.Label4.Text = "Text File View"
      '
      'TreeView1
      '
      Me.TreeView1.Location = New System.Drawing.Point(24, 184)
      Me.TreeView1.Name = "TreeView1"
      Me.TreeView1.Size = New System.Drawing.Size(238, 288)
      Me.TreeView1.TabIndex = 36
      '
      'TextBox1
      '
      Me.TextBox1.Location = New System.Drawing.Point(429, 32)
      Me.TextBox1.Name = "TextBox1"
      Me.TextBox1.Size = New System.Drawing.Size(104, 20)
      Me.TextBox1.TabIndex = 37
      '
      'Label5
      '
      Me.Label5.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label5.Location = New System.Drawing.Point(426, 9)
      Me.Label5.Name = "Label5"
      Me.Label5.Size = New System.Drawing.Size(128, 23)
      Me.Label5.TabIndex = 38
      Me.Label5.Text = "New Dir/File Name"
      '
      'RichTextBox1
      '
      Me.RichTextBox1.Location = New System.Drawing.Point(429, 184)
      Me.RichTextBox1.Name = "RichTextBox1"
      Me.RichTextBox1.Size = New System.Drawing.Size(297, 120)
      Me.RichTextBox1.TabIndex = 43
      Me.RichTextBox1.Text = ""
      '
      'RichTextBox2
      '
      Me.RichTextBox2.Location = New System.Drawing.Point(429, 352)
      Me.RichTextBox2.Name = "RichTextBox2"
      Me.RichTextBox2.Size = New System.Drawing.Size(297, 120)
      Me.RichTextBox2.TabIndex = 44
      Me.RichTextBox2.Text = ""
      '
      'Button_Create_New_File
      '
      Me.Button_Create_New_File.Location = New System.Drawing.Point(283, 237)
      Me.Button_Create_New_File.Name = "Button_Create_New_File"
      Me.Button_Create_New_File.Size = New System.Drawing.Size(120, 23)
      Me.Button_Create_New_File.TabIndex = 45
      Me.Button_Create_New_File.Text = "&Create New File"
      '
      'Label6
      '
      Me.Label6.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Label6.Location = New System.Drawing.Point(560, 8)
      Me.Label6.Name = "Label6"
      Me.Label6.Size = New System.Drawing.Size(128, 23)
      Me.Label6.TabIndex = 47
      Me.Label6.Text = "File Extension"
      '
      'NumericUpDown1
      '
      Me.NumericUpDown1.Location = New System.Drawing.Point(563, 32)
      Me.NumericUpDown1.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
      Me.NumericUpDown1.Name = "NumericUpDown1"
      Me.NumericUpDown1.Size = New System.Drawing.Size(80, 20)
      Me.NumericUpDown1.TabIndex = 38
      '
      'Button_Delete_Selected
      '
      Me.Button_Delete_Selected.Location = New System.Drawing.Point(283, 352)
      Me.Button_Delete_Selected.Name = "Button_Delete_Selected"
      Me.Button_Delete_Selected.Size = New System.Drawing.Size(120, 23)
      Me.Button_Delete_Selected.TabIndex = 49
      Me.Button_Delete_Selected.Text = "&Delete Selected"
      '
      'Form1
      '
      Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
      Me.ClientSize = New System.Drawing.Size(748, 500)
      Me.Controls.Add(Me.Button_Delete_Selected)
      Me.Controls.Add(Me.NumericUpDown1)
      Me.Controls.Add(Me.Label6)
      Me.Controls.Add(Me.Button_Create_New_File)
      Me.Controls.Add(Me.RichTextBox2)
      Me.Controls.Add(Me.RichTextBox1)
      Me.Controls.Add(Me.Label5)
      Me.Controls.Add(Me.TextBox1)
      Me.Controls.Add(Me.TreeView1)
      Me.Controls.Add(Me.Label4)
      Me.Controls.Add(Me.Label3)
      Me.Controls.Add(Me.Button_Read_File)
      Me.Controls.Add(Me.Button_Make_New_Directory)
      Me.Controls.Add(Me.Button_Read_Dir)
      Me.Controls.Add(Me.Button_Search)
      Me.Controls.Add(Me.StatusBar1)
      Me.Controls.Add(Me.Button_Format)
      Me.Controls.Add(Me.Label2)
      Me.Controls.Add(Me.Label1)
      Me.Controls.Add(Me.ListBox1)
      Me.Name = "Form1"
      Me.Text = "1-Wire File Structure Viewer Application"
      CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub

#End Region

   ' Searches the 1-Wire and generates a list of the iButtons on the 1-Wire
   Private Sub Button_Search_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Search.Click
      Dim owd_enum As java.util.Enumeration
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim i As Integer

      Try
         StatusBar1.Text = "Processing...."
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         owd_enum = adapter.getAllDeviceContainers

         StatusBar1.Text = ""

         ListBox1.ClearSelected()
         ListBox1.Items.Clear()

         ' enumerate through all the 1-Wire devices found (with Java-style enumeration)
         '
         While owd_enum.hasMoreElements()
            StatusBar1.Text = " "
            StatusBar1.Text.ToString()

            ' retrieve OneWireContainer
            owd = owd_enum.nextElement()

            ListBox1.Items.Add(owd.getAddressAsString)
         End While

         For i = 0 To (ListBox1.Items.Count - 1)
            ListBox1.Items.Item(i).ToString()
         Next

         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()
         ListBox1.SelectedIndex() = 0
         StatusBar1.Text = "Search:  Done"

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try

   End Sub

   '  Formats the selected iButton
   Private Sub Button_Format_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Format.Click
      Dim owf As com.dalsemi.onewire.application.file.OWFile
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim owfd As com.dalsemi.onewire.application.file.OWFileDescriptor
      Dim done As Boolean

      Try
         StatusBar1.Text = "Processing...."
         If (ListBox1.Items.Count > 0 And ListBox1.SelectedIndex > -1) Then

            ' get exclusive use of 1-Wire network
            adapter.beginExclusive(True)

            ' clear any previous search restrictions
            adapter.setSearchAllDevices()
            adapter.targetAllFamilies()
            adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

            '
            owd = adapter.getDeviceContainer(ListBox1.Items.Item(ListBox1.SelectedIndex).ToString())
            StatusBar1.Text = ""

            ' create a 1-Wire file at root
            owf = New com.dalsemi.onewire.application.file.OWFile(owd, "")
            owf.format()

            ' get 1-Wire File descriptor to flush to device
            owfd = owf.getFD()

            done = False

            ' loop until sync is successful
            While (Not done)
               Try
                  owfd.sync()
                  done = True
               Catch ex As Exception
                  done = False
               End Try
            End While
            ' close the 1-Wire fil
            owfd.close()

            ' end exclusive use of 1-Wire net adapter
            adapter.endExclusive()

            ' read the directory
            readDirectory()
            StatusBar1.Text = "Format:  Done"
         Else
            MessageBox.Show("Please select a device first.")
         End If

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try

   End Sub

   ' This reads the directory of the selected iButton and displays a tree view of it.
   Private Sub readDirectory()
      Dim owf As com.dalsemi.onewire.application.file.OWFile
      Dim owftemp As com.dalsemi.onewire.application.file.OWFile
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim owfd As com.dalsemi.onewire.application.file.OWFileDescriptor
      Dim string_array() As String
      Dim length As Integer
      Dim i As Integer

      Try
         StatusBar1.Text = "Processing...."
         If (ListBox1.Items.Count > 0 And ListBox1.SelectedIndex > -1) Then

            ' get exclusive use of 1-Wire network
            adapter.beginExclusive(True)

            ' clear any previous search restrictions
            adapter.setSearchAllDevices()
            adapter.targetAllFamilies()
            adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)

            '
            owd = adapter.getDeviceContainer(ListBox1.Items.Item(ListBox1.SelectedIndex).ToString())
            StatusBar1.Text = ""

            ' create a 1-Wire file at root
            owf = New com.dalsemi.onewire.application.file.OWFile(owd, "")

            ' get 1-Wire File descriptor to flush to device
            owfd = owf.getFD()
            TreeView1.Nodes.Clear()

            nodNew = TreeView1.Nodes.Add("/")

            string_array = owfd.list

            length = string_array.Length()

            For i = 0 To length - 1
               owftemp = New com.dalsemi.onewire.application.file.OWFile(owd, string_array(i))
               If (owftemp.isDirectory) Then
                  nodNew.Nodes.Add(ReadDir(owftemp, owd))
               Else
                  nodNew.Nodes.Add(string_array(i))
                  owftemp.close()
               End If
            Next

            owf.close()

            nodNew.ExpandAll()

            ' end exclusive use of 1-Wire net adapter
            adapter.endExclusive()
            StatusBar1.Text = "Read Directory:  Done"

            ' select TreeView so operations can take place on it
            TreeView1.Select()
         Else
            MessageBox.Show("Please select a device first.")
         End If

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try
   End Sub
   '  Read directory button click event
   Private Sub Button_Read_Dir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Read_Dir.Click
      readDirectory()
   End Sub

   '  This makes a new directory in the directory selected in the TreeNode
   Private Sub Button_Make_New_Directory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Make_New_Directory.Click
      Dim owf As com.dalsemi.onewire.application.file.OWFile
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim temp_string As String
      Dim add_string As String

      Try
         temp_string = ""

         If (ListBox1.SelectedIndex > -1) Then
            If (TextBox1.Text.ToString().Length > 4) Then
               MessageBox.Show("Error:  Dir/File name is over 4 chars long.  Please re-name.")
            End If

            StatusBar1.Text = "Processing...."

            ' get exclusive use of 1-Wire network
            adapter.beginExclusive(True)

            ' clear any previous search restrictions
            adapter.setSearchAllDevices()
            adapter.targetAllFamilies()
            adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)
            '
            owd = adapter.getDeviceContainer(ListBox1.Items.Item(ListBox1.SelectedIndex).ToString())
            StatusBar1.Text = ""

            temp_string = TreeView1.SelectedNode.FullPath.ToString
            temp_string = temp_string.Replace("\", "")
            temp_string = temp_string.Remove(0, 1)
            add_string = "/" & TextBox1.Text.ToString
            temp_string = String.Concat(temp_string, add_string)
            temp_string = temp_string.ToUpper()

            ' create a 1-Wire dir 
            owf = New com.dalsemi.onewire.application.file.OWFile(owd, temp_string)

            owf.mkdir()

            ' close the 1-Wire file
            owf.close()

            ' end exclusive use of 1-Wire net adapter
            adapter.endExclusive()
            TextBox1.Clear()

            ' read the directory
            readDirectory()
            StatusBar1.Text = "Make New Directory:  Done"

         Else
            MessageBox.Show("Error:  Make sure you select a 1-Wire/iButton device and a directory item first.")
         End If

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try

   End Sub

   '  This button click event reads a file and displays it as Hex or ASCII text
   Private Sub Button_Read_File_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Read_File.Click
      Dim owf As com.dalsemi.onewire.application.file.OWFile
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim temp_string As String
      Dim length As Integer
      Dim readBuff(1024) As System.SByte
      Dim fis As com.dalsemi.onewire.application.file.OWFileInputStream
      Dim i As Integer

      Try
         If (ListBox1.SelectedIndex < 0) Then
            MessageBox.Show("Error:  Make sure you select a 1-Wire/iButton device and a File/directory item first.")
            Exit Sub
         End If
         StatusBar1.Text = "Processing...."
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)
         '
         StatusBar1.Text = ""
         owd = adapter.getDeviceContainer(ListBox1.Items.Item(ListBox1.SelectedIndex).ToString())

         ' make a file path from the Tree View
         ' an example "tree view" path is:  "/\/DIR1\/DIR2\HELP.1"
         ' we want to transform it into "/DIR1/DIR2/HELP.1"
         temp_string = TreeView1.SelectedNode.FullPath.ToString
         ' replace the path "separators" with forward slashes to get valid 1-Wire file path
         temp_string = temp_string.Replace("/\", "/")
         temp_string = temp_string.Replace("//", "/")
         temp_string = temp_string.Replace("\/", "/")
         temp_string = temp_string.Replace("\", "/")

         ' instantiate a 1-Wire dir object
         owf = New com.dalsemi.onewire.application.file.OWFile(owd, temp_string)

         ' if what we are attempting to read is a directory, give error message and exit subroutine
         If owf.isDirectory Then
            StatusBar1.Text = "Error:  " & temp_string & " Is not a File"
            StatusBar1.Text.ToString()
            MessageBox.Show("Error:  " & temp_string & " Is not a File")
            owf.close()
            Exit Sub
            ' if what we are reading can't be read, error out and exit subroutine
         ElseIf Not owf.canRead Then
            StatusBar1.Text = "Error:  " & temp_string & " Can not be read"
            StatusBar1.Text.ToString()
            MessageBox.Show("Error:  " & temp_string & " Can not be read")
            owf.close()
            Exit Sub
         Else
            fis = New com.dalsemi.onewire.application.file.OWFileInputStream(owd, owf.getAbsolutePath())

            ' read the file
            length = fis.read(readBuff)
            fis.close()

            ' clear hex view text box
            RichTextBox1.Clear()

            ' write file contents in hex to hex view text box one byte at a time
            For i = 0 To length - 1
               RichTextBox1.AppendText(com.dalsemi.onewire.utils.Convert.toHexString(readBuff(i)) & " ")
            Next

            temp_string = SByteToAscString(readBuff)
            RichTextBox2.Clear()
            RichTextBox2.AppendText(temp_string)
         End If

         owf.close()

         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()

         StatusBar1.Text = "Read File:  Done"

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try

   End Sub

   ' Called by readDirectory() above
   Private Function ReadDir(ByVal node As com.dalsemi.onewire.application.file.OWFile, ByVal owd As com.dalsemi.onewire.container.OneWireContainer) As TreeNode
      Dim addNode As New System.Windows.Forms.TreeNode("/" & node.getName)
      Dim owftemp As com.dalsemi.onewire.application.file.OWFile
      Dim length As Integer
      Dim string_array() As String
      Dim i As Integer

      string_array = node.list
      length = string_array.Length()

      If length > 0 Then
         For i = 0 To length - 1
            If node.getAbsolutePath().Equals("/") Then
               owftemp = New com.dalsemi.onewire.application.file.OWFile(owd, node.getAbsolutePath() & string_array(i))
            Else
               owftemp = New com.dalsemi.onewire.application.file.OWFile(owd, node.getAbsolutePath() & "/" & string_array(i))
            End If

            If (owftemp.isDirectory) Then
               addNode.Nodes.Add(ReadDir(owftemp, owd))
            Else
               addNode.Nodes.Add(string_array(i))
               owftemp.close()

            End If
         Next
      End If

      node.close()

      Return addNode
   End Function

   ' Create a new file
   Private Sub Button_Create_New_File_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Create_New_File.Click
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim temp_string As String
      Dim fileNameString As String
      Dim extString As String
      Dim lengthHex As Integer
      Dim lengthChar As Integer
      Dim writeBuff(1024) As System.SByte
      Dim fos As com.dalsemi.onewire.application.file.OWFileOutputStream
      Dim owfd As com.dalsemi.onewire.application.file.OWFileDescriptor
      Dim done As Boolean
      Dim loop_retries As Integer

      Try
         If (TextBox1.Text.ToString().Length > 4) Then
            MessageBox.Show("Error:  Dir/File name is over 4 chars long.  Please re-name.")
            Exit Sub
         End If
         If (ListBox1.SelectedIndex < 0) Then
            MessageBox.Show("Error:  Make sure you select a 1-Wire/iButton device and a directory item first.")
            Exit Sub
         End If

         StatusBar1.Text = "Processing...."
         done = False
         loop_retries = 0

         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)
         '
         owd = adapter.getDeviceContainer(ListBox1.Items.Item(ListBox1.SelectedIndex).ToString())
         StatusBar1.Text = ""

         temp_string = TreeView1.SelectedNode.FullPath.ToString
         temp_string = temp_string.Replace("\", "")
         temp_string = temp_string.Remove(0, 1)
         fileNameString = TextBox1.Text()
         fileNameString = fileNameString.Replace(" ", "")
         ' retrieve file extension from NumericUpDown box
         extString = NumericUpDown1.Value.ToString
         temp_string = temp_string & "/" & fileNameString & "." & extString

         lengthHex = RichTextBox1.Text.Length
         lengthChar = RichTextBox2.Text.Length

         If lengthHex = 0 And lengthChar = 0 Then
            StatusBar1.Text = "Error:  No file data in Hex or Char Text Boxes"
            StatusBar1.Text.ToString()
            Exit Sub
         End If
         If lengthHex > 0 And lengthChar > 0 Then
            StatusBar1.Text = "Error:  Only data in Hex or Char Text Boxes not both"
            StatusBar1.Text.ToString()
            Exit Sub
         End If

         ' Create a hex file if the hex file text box has information in it
         If lengthHex > 0 Then
            ' create a 1-Wire file
            fos = New com.dalsemi.onewire.application.file.OWFileOutputStream(owd, temp_string)
            ' write the data (in a byte array writeBuff).  Grab data from RichTextBox1
            writeBuff = com.dalsemi.onewire.utils.Convert.toByteArray(RichTextBox1.Text())
            fos.write(writeBuff)
            'get 1-Wire File descriptor to flush to device
            owfd = fos.getFD()

            ' loop until sync is successful -- see documentation of OWFileOutputStream
            Do Until (done Or loop_retries = 20)
               Try
                  owfd.sync()
                  done = True
               Catch ex As com.dalsemi.onewire.application.file.OWSyncFailedException
                  done = False
               End Try
               loop_retries = loop_retries + 1
            Loop
            fos.close()
            ' show error message if file not created
            If done = False Then
               MessageBox.Show("File not created")
               Exit Sub
            End If

            ' Create an ASCII file if the ASCII file text box has information in it
         ElseIf lengthChar > 0 Then
            ' create a 1-Wire file
            fos = New com.dalsemi.onewire.application.file.OWFileOutputStream(owd, temp_string)
            ' write the data (in a byte array writeBuff).  Grab data from RichTextBox2 and convert from ASCII
            writeBuff = AscStringToSByte(RichTextBox2.Text())
            ' writeBuff now has an extra hex 00 at the end (like a C String), so let's remove it
            Array.Resize(writeBuff, writeBuff.Length - 1)

            fos.write(writeBuff)
            'get 1-Wire File descriptor to flush to device
            owfd = fos.getFD()

            ' loop until sync is successful -- see documentation of OWFileOutputStream
            done = False
            loop_retries = 0
            Do Until (done Or loop_retries = 20)
               Try
                  owfd.sync()
                  done = True
               Catch ex As com.dalsemi.onewire.application.file.OWSyncFailedException
                  done = False
               End Try
               loop_retries = loop_retries + 1
            Loop
            fos.close()
            ' show error message if file not created
            If done = False Then
               MessageBox.Show("File not created")
               Exit Sub
            End If
         End If

         RichTextBox1.Clear()
         RichTextBox2.Clear()

         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()

         ' read the directory
         readDirectory()
         StatusBar1.Text = "Create New File:  Done"

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try

   End Sub

   Private Function AscStringToSByte(ByVal str As String) As [SByte]()
      Dim bytes(str.Length) As [SByte]
      Dim i As Integer
      For i = 0 To UBound(bytes) - 1
         bytes(i) = Convert.ToSByte(str.Chars(i))
      Next
      AscStringToSByte = bytes
   End Function

   Private Function SByteToAscString(ByRef data() As [SByte]) As String
      Dim i As Integer
      Dim str As String
      Dim charToAdd As Short
      str = ""
      For i = 0 To data.Length - 1
         charToAdd = Convert.ToInt16(data(i))

         If (charToAdd < 127 And charToAdd > 31) Then
            str = str & Chr(charToAdd)
         Else
            Exit For
         End If
      Next
      If (str.Length = 0) Then
         str = "No Ascii Data Found"
      End If
      Return str
   End Function

   Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
      adapter.freePort()
   End Sub

   Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Try
         ' get the default adapter
         adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter
         ' print that we got an adapter
         StatusBar1.Text = "Adapter: " & adapter.getAdapterName & " Port: " & adapter.getPortName
      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
      End Try
   End Sub

   Private Sub Button_Delete_Selected_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Delete_Selected.Click
      Dim owf As com.dalsemi.onewire.application.file.OWFile
      Dim owd As com.dalsemi.onewire.container.OneWireContainer
      Dim temp_string As String

      Try
         If (ListBox1.SelectedIndex < 0) Then
            MessageBox.Show("Error:  Make sure you select a 1-Wire/iButton device and a File/directory item first.")
            Exit Sub
         End If

         ' make a file path from the Tree View
         ' an example "tree view" path is:  "/\/DIR1\/DIR2\HELP.1"
         ' we want to transform it into "/DIR1/DIR2/HELP.1"
         temp_string = TreeView1.SelectedNode.FullPath.ToString
         ' replace the path "separators" with forward slashes to get valid 1-Wire file path
         temp_string = temp_string.Replace("/\", "/")
         temp_string = temp_string.Replace("//", "/")
         temp_string = temp_string.Replace("\/", "/")
         temp_string = temp_string.Replace("\", "/")

         ' display error message when attempting to delete the root directory
         If (temp_string.Equals("/")) Then
            MessageBox.Show("Error:  You cannot delete the root directory.")
            Exit Sub
         End If

         StatusBar1.Text = "Processing...."
         ' get exclusive use of 1-Wire network
         adapter.beginExclusive(True)

         ' clear any previous search restrictions
         adapter.setSearchAllDevices()
         adapter.targetAllFamilies()
         adapter.setSpeed(com.dalsemi.onewire.adapter.DSPortAdapter.SPEED_REGULAR)
         '

         owd = adapter.getDeviceContainer(ListBox1.Items.Item(ListBox1.SelectedIndex).ToString())

         ' instantiate a 1-Wire dir object
         owf = New com.dalsemi.onewire.application.file.OWFile(owd, temp_string)
         If Not owf.delete() Then
            owf.close()
            MessageBox.Show("Error:  Could not delete item. If deleting a directory, make sure it is empty before deleting.")
            adapter.endExclusive()
            StatusBar1.Text = "Delete Selected Item:  Done"
            Exit Sub
         End If

         owf.close()

         ' end exclusive use of 1-Wire net adapter
         adapter.endExclusive()

         ' read directory
         readDirectory()

         StatusBar1.Text = "Delete Selected Item:  Done"

      Catch ex As Exception
         StatusBar1.Text = "Error:  " & ex.ToString
         StatusBar1.Text.ToString()
      End Try
   End Sub
End Class
