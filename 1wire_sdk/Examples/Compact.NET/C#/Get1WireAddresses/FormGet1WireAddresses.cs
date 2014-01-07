using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
// using the OneWireLinkLayer
using DalSemi.OneWire.Adapter;
using DalSemi.OneWire;

namespace Get1WireAddresses
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FormGet1WireAddresses : System.Windows.Forms.Form
	{
		// Declare port adapter
		public PortAdapter adapter = null;

		private System.Windows.Forms.TextBox textBoxResults;
		private System.Windows.Forms.Button buttonSearch;
		private System.Windows.Forms.StatusBar statusBarAdapter;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormGet1WireAddresses()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
	            
		}
	

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.textBoxResults = new System.Windows.Forms.TextBox();
         this.buttonSearch = new System.Windows.Forms.Button();
         this.statusBarAdapter = new System.Windows.Forms.StatusBar();
         this.SuspendLayout();
         // 
         // textBoxResults
         // 
         this.textBoxResults.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.textBoxResults.Location = new System.Drawing.Point(24, 32);
         this.textBoxResults.Multiline = true;
         this.textBoxResults.Name = "textBoxResults";
         this.textBoxResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.textBoxResults.Size = new System.Drawing.Size(256, 192);
         this.textBoxResults.TabIndex = 0;
         // 
         // buttonSearch
         // 
         this.buttonSearch.Location = new System.Drawing.Point(296, 96);
         this.buttonSearch.Name = "buttonSearch";
         this.buttonSearch.Size = new System.Drawing.Size(64, 32);
         this.buttonSearch.TabIndex = 1;
         this.buttonSearch.Text = "&Search";
         this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
         // 
         // statusBarAdapter
         // 
         this.statusBarAdapter.Location = new System.Drawing.Point(0, 244);
         this.statusBarAdapter.Name = "statusBarAdapter";
         this.statusBarAdapter.Size = new System.Drawing.Size(376, 22);
         this.statusBarAdapter.TabIndex = 2;
         // 
         // FormGet1WireAddresses
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(376, 266);
         this.Controls.Add(this.statusBarAdapter);
         this.Controls.Add(this.buttonSearch);
         this.Controls.Add(this.textBoxResults);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
         this.MaximizeBox = false;
         this.Name = "FormGet1WireAddresses";
         this.Text = "Get 1-Wire Addresses";
         this.Load += new System.EventHandler(this.FormGet1WireAddresses_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FormGet1WireAddresses());
		}


		private void buttonSearch_Click(object sender, System.EventArgs e)
		{
			if (adapter != null)
			{
				try
				{
					// get exclusive use of resource
					adapter.BeginExclusive(true);
					// clear any previous search restrictions
					adapter.SetSearchAllDevices();
					adapter.TargetAllFamilies();
					adapter.Speed = OWSpeed.SPEED_REGULAR;
					// print header to text box
                    textBoxResults.AppendText(Environment.NewLine + "1-Wire List:" + Environment.NewLine);
                    textBoxResults.AppendText("================="+ Environment.NewLine);
                    // get 1-Wire Addresses
					byte[] address = new byte[8];
					// get the first 1-Wire device's address
					// keep in mind the first device is not necessarily the first 
					// device physically located on the network.
					if(adapter.GetFirstDevice(address, 0))
					{
						do  // get subsequent 1-Wire device addresses
						{
							textBoxResults.AppendText(Print1WireHexAddress(address) + Environment.NewLine);
						}
						while(adapter.GetNextDevice(address, 0));
					}
					// end exclusive use of resource
					adapter.EndExclusive();
				}
				catch (Exception ex)
				{
					textBoxResults.AppendText("Error: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""));
					statusBarAdapter.Text = "Not Loaded";
					adapter.EndExclusive();
				}
			}
		}
		private static string Print1WireHexAddress(byte[] buff)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(buff.Length*3);
			for(int i=7; i>-1; i--)
			{
				sb.Append(buff[i].ToString("X2"));
			}
			return sb.ToString();
		}

      private void FormGet1WireAddresses_Load(object sender, EventArgs e)
      {
         try
         {
            // AccessProvider defaults to the COM port
            adapter = AccessProvider.DefaultAdapter;
            //if you want another adapter, such as the DS9490, see this call
            //adapter = AccessProvider.GetAdapter("{DS9490}", "USB1");
            statusBarAdapter.Text = adapter.ToString();
         }
         catch (Exception ex)
         {
            textBoxResults.AppendText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException != null) ? ex.InnerException.Message : ""));
            statusBarAdapter.Text = "Not Loaded";
            adapter = null;
         }
      }
	}
}
