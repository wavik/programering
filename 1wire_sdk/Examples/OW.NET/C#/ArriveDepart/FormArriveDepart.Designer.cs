namespace ArriveDepart
{
   partial class FormArriveDepart
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.textBoxResults = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // textBoxResults
         // 
         this.textBoxResults.Location = new System.Drawing.Point(12, 26);
         this.textBoxResults.Multiline = true;
         this.textBoxResults.Name = "textBoxResults";
         this.textBoxResults.Size = new System.Drawing.Size(263, 228);
         this.textBoxResults.TabIndex = 0;
         // 
         // FormArriveDepart
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(372, 273);
         this.Controls.Add(this.textBoxResults);
         this.Name = "FormArriveDepart";
         this.Text = "Arrive/Depart";
         this.Load += new System.EventHandler(this.Form1_Load);
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox textBoxResults;
   }
}

