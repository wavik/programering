<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormSecureKey
   Inherits System.Windows.Forms.Form

   'Form overrides dispose to clean up the component list.
   <System.Diagnostics.DebuggerNonUserCode()> _
   Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      Try
         If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
         End If
      Finally
         MyBase.Dispose(disposing)
      End Try
   End Sub

   'Required by the Windows Form Designer
   Private components As System.ComponentModel.IContainer

   'NOTE: The following procedure is required by the Windows Form Designer
   'It can be modified using the Windows Form Designer.  
   'Do not modify it using the code editor.
   <System.Diagnostics.DebuggerStepThrough()> _
   Private Sub InitializeComponent()
      Me.ResultsTextBox = New System.Windows.Forms.TextBox
      Me.SetAllDS1991Passwords = New System.Windows.Forms.Button
      Me.WriteDS1991Button = New System.Windows.Forms.Button
      Me.ReadDS1991Button = New System.Windows.Forms.Button
      Me.SuspendLayout()
      '
      'ResultsTextBox
      '
      Me.ResultsTextBox.Location = New System.Drawing.Point(26, 40)
      Me.ResultsTextBox.Multiline = True
      Me.ResultsTextBox.Name = "ResultsTextBox"
      Me.ResultsTextBox.Size = New System.Drawing.Size(268, 219)
      Me.ResultsTextBox.TabIndex = 0
      '
      'SetAllDS1991Passwords
      '
      Me.SetAllDS1991Passwords.Location = New System.Drawing.Point(315, 62)
      Me.SetAllDS1991Passwords.Name = "SetAllDS1991Passwords"
      Me.SetAllDS1991Passwords.Size = New System.Drawing.Size(120, 41)
      Me.SetAllDS1991Passwords.TabIndex = 1
      Me.SetAllDS1991Passwords.Text = "&Find and Set All DS1991 Passwords"
      Me.SetAllDS1991Passwords.UseVisualStyleBackColor = True
      '
      'WriteDS1991Button
      '
      Me.WriteDS1991Button.Location = New System.Drawing.Point(315, 128)
      Me.WriteDS1991Button.Name = "WriteDS1991Button"
      Me.WriteDS1991Button.Size = New System.Drawing.Size(119, 36)
      Me.WriteDS1991Button.TabIndex = 2
      Me.WriteDS1991Button.Text = "&Write Secure Subkeys"
      Me.WriteDS1991Button.UseVisualStyleBackColor = True
      '
      'ReadDS1991Button
      '
      Me.ReadDS1991Button.Location = New System.Drawing.Point(314, 186)
      Me.ReadDS1991Button.Name = "ReadDS1991Button"
      Me.ReadDS1991Button.Size = New System.Drawing.Size(120, 38)
      Me.ReadDS1991Button.TabIndex = 3
      Me.ReadDS1991Button.Text = "&Read Secure Subkeys"
      Me.ReadDS1991Button.UseVisualStyleBackColor = True
      '
      'FormSecureKey
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(447, 293)
      Me.Controls.Add(Me.ReadDS1991Button)
      Me.Controls.Add(Me.WriteDS1991Button)
      Me.Controls.Add(Me.SetAllDS1991Passwords)
      Me.Controls.Add(Me.ResultsTextBox)
      Me.Name = "FormSecureKey"
      Me.Text = "Secure Key DS1991 VB.NET Demo"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Friend WithEvents ResultsTextBox As System.Windows.Forms.TextBox
   Friend WithEvents SetAllDS1991Passwords As System.Windows.Forms.Button
   Friend WithEvents WriteDS1991Button As System.Windows.Forms.Button
   Friend WithEvents ReadDS1991Button As System.Windows.Forms.Button

End Class
