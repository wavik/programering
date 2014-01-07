/*---------------------------------------------------------------------------
 * Copyright (C) 2005-2008 Maxim Integrated Products, All Rights Reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY,  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL MAXIM INTEGRATED PRODUCTS BE LIABLE FOR ANY CLAIM, DAMAGES
 * OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * Except as contained in this notice, the name of Maxim Integrated Products
 * shall not be used except as stated in the Maxim Integrated Products
 * Branding Policy.
 *---------------------------------------------------------------------------
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using Microsoft.Win32;
using DalSemi.OneWire.Adapter;
using DalSemi.OneWire;

namespace OneWireIO
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class OneWireIO : System.Windows.Forms.Form
	{
      private System.Windows.Forms.ComboBox deviceAddressList;
      private System.Windows.Forms.Button onewireSearchButton;
      private System.Windows.Forms.Button resetMatchROMButton;
      private System.Windows.Forms.Button resetODMatchROMButton;
      private System.Windows.Forms.Button resetSkipROMbutton;
      private System.Windows.Forms.Button resetODSkipROMButton;

      private PortAdapter adapter = null;
      private System.Windows.Forms.StatusBar StatusBar;
      private System.Windows.Forms.StatusBarPanel AdapterStatus;
      private System.Windows.Forms.StatusBarPanel Status;
      private System.Windows.Forms.Button touchByteButton;
      private System.Windows.Forms.RichTextBox oneWireIOTextBox;
      private System.Windows.Forms.NumericUpDown readBytesCount;
      private System.Windows.Forms.Button readBytesButton;
      private System.Windows.Forms.Button resetTouchBytesButton;
      private System.Windows.Forms.Button odResetTouchBytes;
      private System.Windows.Forms.Button clearTextButton;
      private System.Windows.Forms.ComboBox bytesTextBox;
      private System.Windows.Forms.MainMenu mainMenu1;
      private System.Windows.Forms.MenuItem menuItem1;
      private System.Windows.Forms.MenuItem menuItem2;
      private System.Windows.Forms.MenuItem menuItem3;
      private System.Windows.Forms.MenuItem menuItem4;
      private System.Windows.Forms.MenuItem menuItem5;
      private System.Windows.Forms.MenuItem menuItem6;
      private System.Windows.Forms.MenuItem menuItem7;
      private System.Windows.Forms.MenuItem menuItem8;
      private System.Windows.Forms.MenuItem menuItem9;
      private System.Windows.Forms.MenuItem menuItem10;
      private System.Windows.Forms.MenuItem menuItem11;
      private System.Windows.Forms.MenuItem menuItem12;
      private System.Windows.Forms.MenuItem menuItem13;
      private System.Windows.Forms.MenuItem menuItem14;
      private System.Windows.Forms.MenuItem menuItem15;
      private System.Windows.Forms.MenuItem menuItem16;
      private System.Windows.Forms.MenuItem menuItem17;
      private System.Windows.Forms.MenuItem menuItem18;
      private System.Windows.Forms.MenuItem menuItem19;
      private System.Windows.Forms.MenuItem menuItem20;
      private System.Windows.Forms.MenuItem menuItem21;
      private System.Windows.Forms.MenuItem menuItem22;
      private System.Windows.Forms.MenuItem menuItem23;
      private IContainer components;
      private System.Windows.Forms.Button resetButton;
      private System.Windows.Forms.Button odResetButton;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.MenuItem menuItem24;
      private System.Windows.Forms.MenuItem menuItem25;
      private System.Windows.Forms.MenuItem menuItem26;
      private System.Windows.Forms.MenuItem menuItem27;

      private RegistryKey key;
      private System.Windows.Forms.Button oneBitButton;
      private System.Windows.Forms.Button zeroBitButton;
      private System.Windows.Forms.GroupBox searchGroup;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.GroupBox groupBox4;
      private System.Windows.Forms.GroupBox groupBox5;
      private System.Windows.Forms.Button strongPullupButton;
      private System.Windows.Forms.Button setPowerNormalButton;
      private System.Windows.Forms.RadioButton startAfterBitRadioButton;
      private System.Windows.Forms.RadioButton startAfterByteRadioButton;
      private System.Windows.Forms.MenuItem menuItem28;
      private System.Windows.Forms.MenuItem menuItem29;
      private System.Windows.Forms.MenuItem menuItem30;
      private System.Windows.Forms.MenuItem menuItem31;
      private System.Windows.Forms.MenuItem menuItem32;
      private MenuItem menuItem33;
      private MenuItem menuItemStandardSpeed;
      private MenuItem menuItemOverdriveSpeed;
      private OpenFileDialog openFileDialog;

		public OneWireIO()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

         openFileDialog = new OpenFileDialog();
         openFileDialog.CheckFileExists = true;
         openFileDialog.Multiselect = false;
         openFileDialog.Title = "Select Script";

         string adapterName = null, adapterPort = null;
         key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Maxim Integrated Products\\OneWireAPI.NET\\owio", true);
         if(key!=null)
         {
            adapterName = (string)key.GetValue("adapterName", null);
            adapterPort = (string)key.GetValue("adapterPort", null);
            string mru;
            for(int i = 0; (mru=(string)key.GetValue("MRU" + i)) != null; i++)
               bytesTextBox.Items.Add(mru);
         }

         try
         {
            if(adapterName!=null && adapterPort!=null)
               setAdapter(adapterName, adapterPort);//OneWireAccessProvider.getAdapter(adapterName, adapterPort);
            //else
            //   adapter = OneWireAccessProvider.getDefaultAdapter();
            //AdapterStatus.Text = adapter.ToString();
            //adapter.BeginExclusive(true);
         }
         catch(Exception ex)
         {
            addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            AdapterStatus.Text = "Not Loaded";
            adapter = null;
         }
      }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
         if(key==null)
         {
            key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Maxim Integrated Products\\OneWireAPI.NET\\owio");
         }

         if(adapter!=null)
         {
            try
            {
               key.SetValue("adapterName", adapter.AdapterName);
               key.SetValue("adapterPort", adapter.PortName);
               adapter.EndExclusive();
               adapter.Dispose();
               adapter = null;
            }
            catch
            {}
         }

         for(int i=0; i<bytesTextBox.Items.Count; i++)
            key.SetValue("MRU"+i, bytesTextBox.Items[i].ToString());

         key.Close();

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
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OneWireIO));
         this.deviceAddressList = new System.Windows.Forms.ComboBox();
         this.onewireSearchButton = new System.Windows.Forms.Button();
         this.resetMatchROMButton = new System.Windows.Forms.Button();
         this.resetODMatchROMButton = new System.Windows.Forms.Button();
         this.resetSkipROMbutton = new System.Windows.Forms.Button();
         this.resetODSkipROMButton = new System.Windows.Forms.Button();
         this.StatusBar = new System.Windows.Forms.StatusBar();
         this.AdapterStatus = new System.Windows.Forms.StatusBarPanel();
         this.Status = new System.Windows.Forms.StatusBarPanel();
         this.touchByteButton = new System.Windows.Forms.Button();
         this.oneWireIOTextBox = new System.Windows.Forms.RichTextBox();
         this.readBytesCount = new System.Windows.Forms.NumericUpDown();
         this.readBytesButton = new System.Windows.Forms.Button();
         this.resetTouchBytesButton = new System.Windows.Forms.Button();
         this.odResetTouchBytes = new System.Windows.Forms.Button();
         this.clearTextButton = new System.Windows.Forms.Button();
         this.bytesTextBox = new System.Windows.Forms.ComboBox();
         this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
         this.menuItem1 = new System.Windows.Forms.MenuItem();
         this.menuItem2 = new System.Windows.Forms.MenuItem();
         this.menuItem3 = new System.Windows.Forms.MenuItem();
         this.menuItem4 = new System.Windows.Forms.MenuItem();
         this.menuItem5 = new System.Windows.Forms.MenuItem();
         this.menuItem6 = new System.Windows.Forms.MenuItem();
         this.menuItem7 = new System.Windows.Forms.MenuItem();
         this.menuItem8 = new System.Windows.Forms.MenuItem();
         this.menuItem9 = new System.Windows.Forms.MenuItem();
         this.menuItem10 = new System.Windows.Forms.MenuItem();
         this.menuItem11 = new System.Windows.Forms.MenuItem();
         this.menuItem12 = new System.Windows.Forms.MenuItem();
         this.menuItem13 = new System.Windows.Forms.MenuItem();
         this.menuItem14 = new System.Windows.Forms.MenuItem();
         this.menuItem15 = new System.Windows.Forms.MenuItem();
         this.menuItem16 = new System.Windows.Forms.MenuItem();
         this.menuItem17 = new System.Windows.Forms.MenuItem();
         this.menuItem18 = new System.Windows.Forms.MenuItem();
         this.menuItem19 = new System.Windows.Forms.MenuItem();
         this.menuItem20 = new System.Windows.Forms.MenuItem();
         this.menuItem21 = new System.Windows.Forms.MenuItem();
         this.menuItem22 = new System.Windows.Forms.MenuItem();
         this.menuItem23 = new System.Windows.Forms.MenuItem();
         this.menuItem28 = new System.Windows.Forms.MenuItem();
         this.menuItem29 = new System.Windows.Forms.MenuItem();
         this.menuItem30 = new System.Windows.Forms.MenuItem();
         this.menuItem31 = new System.Windows.Forms.MenuItem();
         this.menuItem32 = new System.Windows.Forms.MenuItem();
         this.menuItem24 = new System.Windows.Forms.MenuItem();
         this.menuItem25 = new System.Windows.Forms.MenuItem();
         this.menuItem26 = new System.Windows.Forms.MenuItem();
         this.menuItem27 = new System.Windows.Forms.MenuItem();
         this.menuItem33 = new System.Windows.Forms.MenuItem();
         this.menuItemStandardSpeed = new System.Windows.Forms.MenuItem();
         this.menuItemOverdriveSpeed = new System.Windows.Forms.MenuItem();
         this.resetButton = new System.Windows.Forms.Button();
         this.odResetButton = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.oneBitButton = new System.Windows.Forms.Button();
         this.zeroBitButton = new System.Windows.Forms.Button();
         this.searchGroup = new System.Windows.Forms.GroupBox();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.groupBox4 = new System.Windows.Forms.GroupBox();
         this.groupBox5 = new System.Windows.Forms.GroupBox();
         this.startAfterByteRadioButton = new System.Windows.Forms.RadioButton();
         this.startAfterBitRadioButton = new System.Windows.Forms.RadioButton();
         this.setPowerNormalButton = new System.Windows.Forms.Button();
         this.strongPullupButton = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.AdapterStatus)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Status)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.readBytesCount)).BeginInit();
         this.searchGroup.SuspendLayout();
         this.groupBox5.SuspendLayout();
         this.SuspendLayout();
         // 
         // deviceAddressList
         // 
         this.deviceAddressList.Font = new System.Drawing.Font("Courier New", 9F);
         this.deviceAddressList.Location = new System.Drawing.Point(136, 8);
         this.deviceAddressList.Name = "deviceAddressList";
         this.deviceAddressList.Size = new System.Drawing.Size(272, 23);
         this.deviceAddressList.TabIndex = 1;
         // 
         // onewireSearchButton
         // 
         this.onewireSearchButton.Location = new System.Drawing.Point(16, 8);
         this.onewireSearchButton.Name = "onewireSearchButton";
         this.onewireSearchButton.Size = new System.Drawing.Size(112, 24);
         this.onewireSearchButton.TabIndex = 0;
         this.onewireSearchButton.Text = "1-Wire Search";
         this.onewireSearchButton.Click += new System.EventHandler(this.onewireSearchButton_Click);
         // 
         // resetMatchROMButton
         // 
         this.resetMatchROMButton.Location = new System.Drawing.Point(264, 48);
         this.resetMatchROMButton.Name = "resetMatchROMButton";
         this.resetMatchROMButton.Size = new System.Drawing.Size(136, 24);
         this.resetMatchROMButton.TabIndex = 4;
         this.resetMatchROMButton.Text = "Reset - Match ROM";
         this.resetMatchROMButton.Click += new System.EventHandler(this.resetMatchROMButton_Click);
         // 
         // resetODMatchROMButton
         // 
         this.resetODMatchROMButton.Location = new System.Drawing.Point(264, 80);
         this.resetODMatchROMButton.Name = "resetODMatchROMButton";
         this.resetODMatchROMButton.Size = new System.Drawing.Size(136, 24);
         this.resetODMatchROMButton.TabIndex = 5;
         this.resetODMatchROMButton.Text = "Reset - OD Match ROM";
         this.resetODMatchROMButton.Click += new System.EventHandler(this.resetODMatchROMButton_Click);
         // 
         // resetSkipROMbutton
         // 
         this.resetSkipROMbutton.Location = new System.Drawing.Point(104, 48);
         this.resetSkipROMbutton.Name = "resetSkipROMbutton";
         this.resetSkipROMbutton.Size = new System.Drawing.Size(136, 24);
         this.resetSkipROMbutton.TabIndex = 6;
         this.resetSkipROMbutton.Text = "Reset - Skip ROM";
         this.resetSkipROMbutton.Click += new System.EventHandler(this.resetSkipROMbutton_Click);
         // 
         // resetODSkipROMButton
         // 
         this.resetODSkipROMButton.Location = new System.Drawing.Point(104, 80);
         this.resetODSkipROMButton.Name = "resetODSkipROMButton";
         this.resetODSkipROMButton.Size = new System.Drawing.Size(136, 24);
         this.resetODSkipROMButton.TabIndex = 7;
         this.resetODSkipROMButton.Text = "Reset - OD Skip ROM";
         this.resetODSkipROMButton.Click += new System.EventHandler(this.resetODSkipROMButton_Click);
         // 
         // StatusBar
         // 
         this.StatusBar.Location = new System.Drawing.Point(0, 491);
         this.StatusBar.Name = "StatusBar";
         this.StatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.AdapterStatus,
            this.Status});
         this.StatusBar.ShowPanels = true;
         this.StatusBar.Size = new System.Drawing.Size(424, 22);
         this.StatusBar.TabIndex = 7;
         this.StatusBar.Text = "Status";
         // 
         // AdapterStatus
         // 
         this.AdapterStatus.Name = "AdapterStatus";
         this.AdapterStatus.Text = "Adapter Not Loaded";
         this.AdapterStatus.Width = 200;
         // 
         // Status
         // 
         this.Status.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
         this.Status.Name = "Status";
         this.Status.Text = "Status";
         this.Status.Width = 207;
         // 
         // touchByteButton
         // 
         this.touchByteButton.Location = new System.Drawing.Point(24, 160);
         this.touchByteButton.Name = "touchByteButton";
         this.touchByteButton.Size = new System.Drawing.Size(96, 24);
         this.touchByteButton.TabIndex = 9;
         this.touchByteButton.Text = "Write Bytes";
         this.touchByteButton.Click += new System.EventHandler(this.touchByteButton_Click);
         // 
         // oneWireIOTextBox
         // 
         this.oneWireIOTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.oneWireIOTextBox.Font = new System.Drawing.Font("Courier New", 9F);
         this.oneWireIOTextBox.Location = new System.Drawing.Point(8, 296);
         this.oneWireIOTextBox.Name = "oneWireIOTextBox";
         this.oneWireIOTextBox.Size = new System.Drawing.Size(408, 160);
         this.oneWireIOTextBox.TabIndex = 0;
         this.oneWireIOTextBox.TabStop = false;
         this.oneWireIOTextBox.Text = "";
         // 
         // readBytesCount
         // 
         this.readBytesCount.Font = new System.Drawing.Font("Courier New", 9F);
         this.readBytesCount.Location = new System.Drawing.Point(104, 208);
         this.readBytesCount.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
         this.readBytesCount.Name = "readBytesCount";
         this.readBytesCount.Size = new System.Drawing.Size(48, 21);
         this.readBytesCount.TabIndex = 12;
         this.readBytesCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         this.readBytesCount.Value = new decimal(new int[] {
            34,
            0,
            0,
            0});
         // 
         // readBytesButton
         // 
         this.readBytesButton.Location = new System.Drawing.Point(160, 208);
         this.readBytesButton.Name = "readBytesButton";
         this.readBytesButton.Size = new System.Drawing.Size(88, 24);
         this.readBytesButton.TabIndex = 13;
         this.readBytesButton.Text = "Read Bytes";
         this.readBytesButton.Click += new System.EventHandler(this.readBytesButton_Click);
         // 
         // resetTouchBytesButton
         // 
         this.resetTouchBytesButton.Location = new System.Drawing.Point(128, 160);
         this.resetTouchBytesButton.Name = "resetTouchBytesButton";
         this.resetTouchBytesButton.Size = new System.Drawing.Size(136, 24);
         this.resetTouchBytesButton.TabIndex = 10;
         this.resetTouchBytesButton.Text = "Reset - Write Bytes";
         this.resetTouchBytesButton.Click += new System.EventHandler(this.resetTouchBytesButton_Click);
         // 
         // odResetTouchBytes
         // 
         this.odResetTouchBytes.Location = new System.Drawing.Point(272, 160);
         this.odResetTouchBytes.Name = "odResetTouchBytes";
         this.odResetTouchBytes.Size = new System.Drawing.Size(136, 24);
         this.odResetTouchBytes.TabIndex = 11;
         this.odResetTouchBytes.Text = "OD Reset - Write Bytes";
         this.odResetTouchBytes.Click += new System.EventHandler(this.odResetTouchBytes_Click);
         // 
         // clearTextButton
         // 
         this.clearTextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.clearTextButton.Location = new System.Drawing.Point(96, 465);
         this.clearTextButton.Name = "clearTextButton";
         this.clearTextButton.Size = new System.Drawing.Size(216, 24);
         this.clearTextButton.TabIndex = 0;
         this.clearTextButton.TabStop = false;
         this.clearTextButton.Text = "Clear 1-Wire Transaction Text";
         this.clearTextButton.Click += new System.EventHandler(this.clearTextButton_Click);
         // 
         // bytesTextBox
         // 
         this.bytesTextBox.Font = new System.Drawing.Font("Courier New", 9F);
         this.bytesTextBox.Location = new System.Drawing.Point(16, 128);
         this.bytesTextBox.MaxDropDownItems = 10;
         this.bytesTextBox.Name = "bytesTextBox";
         this.bytesTextBox.Size = new System.Drawing.Size(392, 23);
         this.bytesTextBox.TabIndex = 8;
         this.bytesTextBox.Text = "33 FF FF FF FF FF FF FF FF";
         // 
         // mainMenu1
         // 
         this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem24,
            this.menuItem33});
         // 
         // menuItem1
         // 
         this.menuItem1.Index = 0;
         this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.menuItem3});
         this.menuItem1.Text = "File";
         // 
         // menuItem2
         // 
         this.menuItem2.Index = 0;
         this.menuItem2.Text = "Close";
         this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
         // 
         // menuItem3
         // 
         this.menuItem3.Index = 1;
         this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4,
            this.menuItem9,
            this.menuItem14,
            this.menuItem19,
            this.menuItem28});
         this.menuItem3.Text = "Pick Adapter";
         // 
         // menuItem4
         // 
         this.menuItem4.Index = 0;
         this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem5,
            this.menuItem6,
            this.menuItem7,
            this.menuItem8});
         this.menuItem4.Text = "{DS9097U}";
         // 
         // menuItem5
         // 
         this.menuItem5.Index = 0;
         this.menuItem5.Text = "COM1";
         this.menuItem5.Click += new System.EventHandler(this.DS9097U_Click);
         // 
         // menuItem6
         // 
         this.menuItem6.Index = 1;
         this.menuItem6.Text = "COM2";
         this.menuItem6.Click += new System.EventHandler(this.DS9097U_Click);
         // 
         // menuItem7
         // 
         this.menuItem7.Index = 2;
         this.menuItem7.Text = "COM3";
         this.menuItem7.Click += new System.EventHandler(this.DS9097U_Click);
         // 
         // menuItem8
         // 
         this.menuItem8.Index = 3;
         this.menuItem8.Text = "COM4";
         this.menuItem8.Click += new System.EventHandler(this.DS9097U_Click);
         // 
         // menuItem9
         // 
         this.menuItem9.Index = 1;
         this.menuItem9.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem10,
            this.menuItem11,
            this.menuItem12,
            this.menuItem13});
         this.menuItem9.Text = "{DS1410E}";
         // 
         // menuItem10
         // 
         this.menuItem10.Index = 0;
         this.menuItem10.Text = "LPT1";
         this.menuItem10.Click += new System.EventHandler(this.DS1410E_Click);
         // 
         // menuItem11
         // 
         this.menuItem11.Index = 1;
         this.menuItem11.Text = "LPT2";
         this.menuItem11.Click += new System.EventHandler(this.DS1410E_Click);
         // 
         // menuItem12
         // 
         this.menuItem12.Index = 2;
         this.menuItem12.Text = "LPT3";
         this.menuItem12.Click += new System.EventHandler(this.DS1410E_Click);
         // 
         // menuItem13
         // 
         this.menuItem13.Index = 3;
         this.menuItem13.Text = "LPT4";
         this.menuItem13.Click += new System.EventHandler(this.DS1410E_Click);
         // 
         // menuItem14
         // 
         this.menuItem14.Index = 2;
         this.menuItem14.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem15,
            this.menuItem16,
            this.menuItem17,
            this.menuItem18});
         this.menuItem14.Text = "{DS9490}";
         // 
         // menuItem15
         // 
         this.menuItem15.Index = 0;
         this.menuItem15.Text = "USB1";
         this.menuItem15.Click += new System.EventHandler(this.DS9490_Click);
         // 
         // menuItem16
         // 
         this.menuItem16.Index = 1;
         this.menuItem16.Text = "USB2";
         this.menuItem16.Click += new System.EventHandler(this.DS9490_Click);
         // 
         // menuItem17
         // 
         this.menuItem17.Index = 2;
         this.menuItem17.Text = "USB3";
         this.menuItem17.Click += new System.EventHandler(this.DS9490_Click);
         // 
         // menuItem18
         // 
         this.menuItem18.Index = 3;
         this.menuItem18.Text = "USB4";
         this.menuItem18.Click += new System.EventHandler(this.DS9490_Click);
         // 
         // menuItem19
         // 
         this.menuItem19.Index = 3;
         this.menuItem19.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem20,
            this.menuItem21,
            this.menuItem22,
            this.menuItem23});
         this.menuItem19.Text = "{DS9097E}";
         // 
         // menuItem20
         // 
         this.menuItem20.Index = 0;
         this.menuItem20.Text = "COM1";
         this.menuItem20.Click += new System.EventHandler(this.DS9097E_Click);
         // 
         // menuItem21
         // 
         this.menuItem21.Index = 1;
         this.menuItem21.Text = "COM2";
         this.menuItem21.Click += new System.EventHandler(this.DS9097E_Click);
         // 
         // menuItem22
         // 
         this.menuItem22.Index = 2;
         this.menuItem22.Text = "COM3";
         this.menuItem22.Click += new System.EventHandler(this.DS9097E_Click);
         // 
         // menuItem23
         // 
         this.menuItem23.Index = 3;
         this.menuItem23.Text = "COM4";
         this.menuItem23.Click += new System.EventHandler(this.DS9097E_Click);
         // 
         // menuItem28
         // 
         this.menuItem28.Index = 4;
         this.menuItem28.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem29,
            this.menuItem30,
            this.menuItem31,
            this.menuItem32});
         this.menuItem28.Text = "DS9097U";
         // 
         // menuItem29
         // 
         this.menuItem29.Index = 0;
         this.menuItem29.Text = "COM1";
         this.menuItem29.Click += new System.EventHandler(this.pureDS9097U_Click);
         // 
         // menuItem30
         // 
         this.menuItem30.Index = 1;
         this.menuItem30.Text = "COM2";
         this.menuItem30.Click += new System.EventHandler(this.pureDS9097U_Click);
         // 
         // menuItem31
         // 
         this.menuItem31.Index = 2;
         this.menuItem31.Text = "COM3";
         this.menuItem31.Click += new System.EventHandler(this.pureDS9097U_Click);
         // 
         // menuItem32
         // 
         this.menuItem32.Index = 3;
         this.menuItem32.Text = "COM4";
         this.menuItem32.Click += new System.EventHandler(this.pureDS9097U_Click);
         // 
         // menuItem24
         // 
         this.menuItem24.Index = 1;
         this.menuItem24.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem25,
            this.menuItem26,
            this.menuItem27});
         this.menuItem24.Text = "Script";
         // 
         // menuItem25
         // 
         this.menuItem25.Index = 0;
         this.menuItem25.Text = "Execute with Reset";
         this.menuItem25.Click += new System.EventHandler(this.menuItem25_Click);
         // 
         // menuItem26
         // 
         this.menuItem26.Index = 1;
         this.menuItem26.Text = "Execute with Reset - Match ROM";
         this.menuItem26.Click += new System.EventHandler(this.menuItem26_Click);
         // 
         // menuItem27
         // 
         this.menuItem27.Index = 2;
         this.menuItem27.Text = "Execute with Reset - Skip ROM";
         this.menuItem27.Click += new System.EventHandler(this.menuItem27_Click);
         // 
         // menuItem33
         // 
         this.menuItem33.Index = 2;
         this.menuItem33.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemStandardSpeed,
            this.menuItemOverdriveSpeed});
         this.menuItem33.Text = "Speed";
         // 
         // menuItemStandardSpeed
         // 
         this.menuItemStandardSpeed.Index = 0;
         this.menuItemStandardSpeed.Text = "Standard";
         this.menuItemStandardSpeed.Click += new System.EventHandler(this.menuItemStandardSpeed_Click);
         // 
         // menuItemOverdriveSpeed
         // 
         this.menuItemOverdriveSpeed.Index = 1;
         this.menuItemOverdriveSpeed.Text = "Overdrive";
         this.menuItemOverdriveSpeed.Click += new System.EventHandler(this.menuItemOverdriveSpeed_Click);
         // 
         // resetButton
         // 
         this.resetButton.Location = new System.Drawing.Point(16, 48);
         this.resetButton.Name = "resetButton";
         this.resetButton.Size = new System.Drawing.Size(80, 24);
         this.resetButton.TabIndex = 2;
         this.resetButton.Text = "STD Reset";
         this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
         // 
         // odResetButton
         // 
         this.odResetButton.Location = new System.Drawing.Point(16, 80);
         this.odResetButton.Name = "odResetButton";
         this.odResetButton.Size = new System.Drawing.Size(80, 24);
         this.odResetButton.TabIndex = 3;
         this.odResetButton.Text = "OD Reset";
         this.odResetButton.Click += new System.EventHandler(this.odResetButton_Click);
         // 
         // label1
         // 
         this.label1.Location = new System.Drawing.Point(16, 208);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(80, 24);
         this.label1.TabIndex = 18;
         this.label1.Text = "Number of Bytes to Read";
         this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // oneBitButton
         // 
         this.oneBitButton.Location = new System.Drawing.Point(272, 208);
         this.oneBitButton.Name = "oneBitButton";
         this.oneBitButton.Size = new System.Drawing.Size(56, 24);
         this.oneBitButton.TabIndex = 19;
         this.oneBitButton.Text = "One Bit";
         this.oneBitButton.Click += new System.EventHandler(this.oneBitButton_Click);
         // 
         // zeroBitButton
         // 
         this.zeroBitButton.Location = new System.Drawing.Point(344, 208);
         this.zeroBitButton.Name = "zeroBitButton";
         this.zeroBitButton.Size = new System.Drawing.Size(56, 24);
         this.zeroBitButton.TabIndex = 20;
         this.zeroBitButton.Text = "Zero Bit";
         this.zeroBitButton.Click += new System.EventHandler(this.zeroBitButton_Click);
         // 
         // searchGroup
         // 
         this.searchGroup.Controls.Add(this.resetMatchROMButton);
         this.searchGroup.Controls.Add(this.resetODMatchROMButton);
         this.searchGroup.Controls.Add(this.resetSkipROMbutton);
         this.searchGroup.Controls.Add(this.resetODSkipROMButton);
         this.searchGroup.Controls.Add(this.groupBox1);
         this.searchGroup.Location = new System.Drawing.Point(8, 0);
         this.searchGroup.Name = "searchGroup";
         this.searchGroup.Size = new System.Drawing.Size(408, 112);
         this.searchGroup.TabIndex = 21;
         this.searchGroup.TabStop = false;
         // 
         // groupBox1
         // 
         this.groupBox1.Location = new System.Drawing.Point(0, 32);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(248, 80);
         this.groupBox1.TabIndex = 22;
         this.groupBox1.TabStop = false;
         // 
         // groupBox2
         // 
         this.groupBox2.Location = new System.Drawing.Point(8, 112);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(408, 80);
         this.groupBox2.TabIndex = 22;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Writing";
         // 
         // groupBox3
         // 
         this.groupBox3.Location = new System.Drawing.Point(8, 192);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(248, 48);
         this.groupBox3.TabIndex = 23;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "Reading";
         // 
         // groupBox4
         // 
         this.groupBox4.Location = new System.Drawing.Point(256, 192);
         this.groupBox4.Name = "groupBox4";
         this.groupBox4.Size = new System.Drawing.Size(160, 48);
         this.groupBox4.TabIndex = 24;
         this.groupBox4.TabStop = false;
         this.groupBox4.Text = "Bit-Bang";
         // 
         // groupBox5
         // 
         this.groupBox5.Controls.Add(this.startAfterByteRadioButton);
         this.groupBox5.Controls.Add(this.startAfterBitRadioButton);
         this.groupBox5.Controls.Add(this.setPowerNormalButton);
         this.groupBox5.Controls.Add(this.strongPullupButton);
         this.groupBox5.Location = new System.Drawing.Point(8, 240);
         this.groupBox5.Name = "groupBox5";
         this.groupBox5.Size = new System.Drawing.Size(408, 56);
         this.groupBox5.TabIndex = 25;
         this.groupBox5.TabStop = false;
         this.groupBox5.Text = "Power Delivery";
         // 
         // startAfterByteRadioButton
         // 
         this.startAfterByteRadioButton.Checked = true;
         this.startAfterByteRadioButton.Location = new System.Drawing.Point(8, 16);
         this.startAfterByteRadioButton.Name = "startAfterByteRadioButton";
         this.startAfterByteRadioButton.Size = new System.Drawing.Size(128, 16);
         this.startAfterByteRadioButton.TabIndex = 4;
         this.startAfterByteRadioButton.TabStop = true;
         this.startAfterByteRadioButton.Text = "Start After Next Byte";
         this.startAfterByteRadioButton.CheckedChanged += new System.EventHandler(this.startAfterByteRadioButton_CheckedChanged);
         // 
         // startAfterBitRadioButton
         // 
         this.startAfterBitRadioButton.Location = new System.Drawing.Point(8, 32);
         this.startAfterBitRadioButton.Name = "startAfterBitRadioButton";
         this.startAfterBitRadioButton.Size = new System.Drawing.Size(120, 16);
         this.startAfterBitRadioButton.TabIndex = 3;
         this.startAfterBitRadioButton.Text = "Start After Next Bit";
         this.startAfterBitRadioButton.CheckedChanged += new System.EventHandler(this.startAfterBitRadioButton_CheckedChanged);
         // 
         // setPowerNormalButton
         // 
         this.setPowerNormalButton.Location = new System.Drawing.Point(288, 16);
         this.setPowerNormalButton.Name = "setPowerNormalButton";
         this.setPowerNormalButton.Size = new System.Drawing.Size(112, 24);
         this.setPowerNormalButton.TabIndex = 2;
         this.setPowerNormalButton.Text = "Set Power Normal";
         this.setPowerNormalButton.Click += new System.EventHandler(this.setPowerNormalButton_Click);
         // 
         // strongPullupButton
         // 
         this.strongPullupButton.Location = new System.Drawing.Point(144, 16);
         this.strongPullupButton.Name = "strongPullupButton";
         this.strongPullupButton.Size = new System.Drawing.Size(120, 24);
         this.strongPullupButton.TabIndex = 1;
         this.strongPullupButton.Text = "Start Strong-Pullup";
         this.strongPullupButton.Click += new System.EventHandler(this.strongPullupButton_Click);
         // 
         // OneWireIO
         // 
         this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
         this.ClientSize = new System.Drawing.Size(424, 513);
         this.Controls.Add(this.zeroBitButton);
         this.Controls.Add(this.oneBitButton);
         this.Controls.Add(this.readBytesCount);
         this.Controls.Add(this.odResetButton);
         this.Controls.Add(this.resetButton);
         this.Controls.Add(this.bytesTextBox);
         this.Controls.Add(this.clearTextButton);
         this.Controls.Add(this.odResetTouchBytes);
         this.Controls.Add(this.resetTouchBytesButton);
         this.Controls.Add(this.readBytesButton);
         this.Controls.Add(this.oneWireIOTextBox);
         this.Controls.Add(this.touchByteButton);
         this.Controls.Add(this.StatusBar);
         this.Controls.Add(this.onewireSearchButton);
         this.Controls.Add(this.deviceAddressList);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.groupBox3);
         this.Controls.Add(this.groupBox4);
         this.Controls.Add(this.searchGroup);
         this.Controls.Add(this.groupBox5);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Menu = this.mainMenu1;
         this.MinimumSize = new System.Drawing.Size(424, 450);
         this.Name = "OneWireIO";
         this.Text = "1-Wire I/O v2.60";
         ((System.ComponentModel.ISupportInitialize)(this.AdapterStatus)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Status)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.readBytesCount)).EndInit();
         this.searchGroup.ResumeLayout(false);
         this.groupBox5.ResumeLayout(false);
         this.ResumeLayout(false);

      }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new OneWireIO());
		}

      private void resetODMatchROMButton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               byte[] address;
               //if(deviceAddressList.SelectedItem!=null)
               //   address = ((OneWireContainer)deviceAddressList.SelectedItem).getAddress();
               //else
               if(deviceAddressList.Text.Length>0)
               {
                  string s = deviceAddressList.Text;
                  //if(s.IndexOf(" ")>0) // LSB first
                     address = FromHex(s);
                  //else // MSB first
                  //   address = Address.toByteArray(s);
               }
               else
               {
                  addText("No device or device address indicated", Color.Red);
                  return;
               }

               addText("Reset: " + adapter.Speed + " - " + adapter.Reset(), Color.Blue);

               adapter.PutByte(0x69); // OD Match ROM cmd
               adapter.Speed = OWSpeed.SPEED_OVERDRIVE;
               adapter.DataBlock(address, 0, 8);

               addText("OD Match ROM: " + ToHex(address,0 ,8), Color.Blue);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void resetMatchROMButton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               byte[] address;
               //if(deviceAddressList.SelectedItem!=null)
               //   address = ((OneWireContainer)deviceAddressList.SelectedItem).getAddress();
               //else 
               if(deviceAddressList.Text.Length>0)
               {
                  string s = deviceAddressList.Text;
                  //if(s.IndexOf(" ")>0) // LSB first
                     address = FromHex(s);
                  //else // MSB first
                  //   address = Address.toByteArray(s);
               }
               else
               {
                  addText("No device or device address indicated", Color.Red);
                  return;
               }

               //adapter.Speed = OWSpeed.SPEED_REGULAR;

               addText("Reset: " + adapter.Speed + " - " + adapter.Reset(), Color.Blue);

               adapter.PutByte(0x55); // Match ROM cmd
               adapter.DataBlock(address, 0, 8);

               addText("Match ROM: " + ToHex(address, 0, 8), Color.Blue);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void resetSkipROMbutton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               //adapter.Speed = OWSpeed.SPEED_REGULAR;

               addText("Reset: " + adapter.Speed + " - " + adapter.Reset(), Color.Blue);

               adapter.PutByte(0xCC); // Skip ROM cmd

               addText("Skip ROM", Color.Blue);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void resetODSkipROMButton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               addText("Reset: " + adapter.Speed + " - " + adapter.Reset(), Color.Blue);

               adapter.PutByte(0x3C); // OD Skip ROM cmd
               adapter.Speed = OWSpeed.SPEED_OVERDRIVE;

               addText("OD Skip ROM", Color.Blue);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void onewireSearchButton_Click(object sender, System.EventArgs e)
      {
         deviceAddressList.Items.Clear();
         if(adapter!=null)
         {
            try
            {
               adapter.Speed = OWSpeed.SPEED_REGULAR;
               adapter.TargetAllFamilies();
               byte[] address = new byte[8];
               if(adapter.GetFirstDevice(address, 0))
               {
                  do
                  {
                     deviceAddressList.Items.Add(ToHex(address, 0, 8));
                  }
                  while(adapter.GetNextDevice(address, 0));
               }
               if(deviceAddressList.Items.Count>0)
                  deviceAddressList.SelectedIndex = 0;
               addText("1-Wire Search Complete", Color.Blue);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void touchByteButton_Click(object sender, System.EventArgs e)
      {
         bytesTextBox.Text = bytesTextBox.Text.Trim();
         if(!bytesTextBox.Items.Contains(bytesTextBox.Text))
            bytesTextBox.Items.Insert(0, bytesTextBox.Text);
         if(adapter!=null)
         {
            try
            {
               if(bytesTextBox.Text.Length>0)
               {
                  byte[] block = FromHex(bytesTextBox.Text);
               
                  addText("Send: " + adapter.Speed, Color.Green);
                  addText(ToHex(block, 0, block.Length), Color.Green);
                  
                  adapter.DataBlock(block, 0, block.Length);
                  
                  addText("Recv: " + adapter.Speed, Color.Black);
                  addText(ToHex(block, 0, block.Length), Color.Black);
               }
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void resetTouchBytesButton_Click(object sender, System.EventArgs e)
      {
         bytesTextBox.Text = bytesTextBox.Text.Trim();
         if(!bytesTextBox.Items.Contains(bytesTextBox.Text))
            bytesTextBox.Items.Insert(0, bytesTextBox.Text);
         if(adapter!=null)
         {
            try
            {
               adapter.Speed = OWSpeed.SPEED_REGULAR;

               addText("Reset: " + adapter.Speed + " - " + adapter.Reset(), Color.Blue);
               
               if(bytesTextBox.Text.Length>0)
               {
                  byte[] block = FromHex(bytesTextBox.Text);
                  
                  addText("Send: " + adapter.Speed, Color.Green);
                  addText(ToHex(block, 0, block.Length), Color.Green);
                  
                  adapter.DataBlock(block, 0, block.Length);
                  
                  addText("Recv: " + adapter.Speed, Color.Black);
                  addText(ToHex(block, 0, block.Length), Color.Black);
               }
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void odResetTouchBytes_Click(object sender, System.EventArgs e)
      {
         bytesTextBox.Text = bytesTextBox.Text.Trim();
         if(!bytesTextBox.Items.Contains(bytesTextBox.Text))
            bytesTextBox.Items.Insert(0, bytesTextBox.Text);
         if(adapter!=null)
         {
            try
            {
               adapter.Speed = OWSpeed.SPEED_OVERDRIVE;

               addText("Reset: " + adapter.Speed + " - " + adapter.Reset(), Color.Blue);
               
               byte[] block = FromHex(bytesTextBox.Text);
               
               addText("Send: " + adapter.Speed, Color.Green);
               addText(ToHex(block, 0, block.Length), Color.Green);
               
               adapter.DataBlock(block, 0, block.Length);
               
               addText("Recv: " + adapter.Speed, Color.Black);
               addText(ToHex(block, 0, block.Length), Color.Black);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void readBytesButton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               byte[] block = new byte[(int)readBytesCount.Value];
               for(int i=0; i<block.Length; i++)
                  block[i] = 0xFF;
               
               addText("Reading " + block.Length + " bytes: " + adapter.Speed, Color.Green);

               adapter.DataBlock(block, 0, block.Length);
               
               addText("Recv: " + adapter.Speed, Color.Black);
               addText(ToHex(block, 0, block.Length), Color.Black);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void addText(string text, Color col)
      {
         try
         {
            oneWireIOTextBox.SelectionColor = col;
            oneWireIOTextBox.AppendText(text + "\r\n");
            //oneWireIOTextBox.SelectionStart = oneWireIOTextBox.Text.Length - 1;
            oneWireIOTextBox.Focus();
            oneWireIOTextBox.ScrollToCaret();
            bytesTextBox.Focus();
            bytesTextBox.SelectAll();
         }
         catch(Exception e)
         {
            Console.Out.WriteLine(e);
         }
      }

      private void clearTextButton_Click(object sender, System.EventArgs e)
      {
         oneWireIOTextBox.Clear();
      }

      private void pureDS9097U_Click(object sender, System.EventArgs e)
      {
         setAdapter("DS9097U", ((MenuItem)sender).Text);
      }
      private void DS9097U_Click(object sender, System.EventArgs e)
      {
         setAdapter("{DS9097U}", ((MenuItem)sender).Text);
      }
      private void DS1410E_Click(object sender, System.EventArgs e)
      {
         setAdapter("{DS1410E}", ((MenuItem)sender).Text);
      }
      private void DS9490_Click(object sender, System.EventArgs e)
      {
         setAdapter("{DS9490}", ((MenuItem)sender).Text);
      }
      private void DS9097E_Click(object sender, System.EventArgs e)
      {
         setAdapter("{DS9097E}", ((MenuItem)sender).Text);
      }

      private void setAdapter(string adapterName, string portName)
      {
         try
         {
            if(adapter!=null)
            {
               adapter.EndExclusive();
               adapter.Dispose();
               adapter = null;
            }

            adapter = AccessProvider.GetAdapter(adapterName, portName);

            if(adapter!=null)
            {
               AdapterStatus.Text = adapter.ToString();
               adapter.BeginExclusive(true);

               /*adapter.Speed = OWSpeed.SPEED_REGULAR;
               adapter.Reset();
               adapter.PutByte(0x3C);
               adapter.Speed = OWSpeed.SPEED_OVERDRIVE;
               adapter.PutByte(0x3C);

               adapter.Speed = OWSpeed.SPEED_REGULAR;
               adapter.Reset();
               adapter.PutByte(0x3C);
               adapter.PutByte(0x3C);

               adapter.Speed = OWSpeed.SPEED_REGULAR;
               adapter.Reset();
               adapter.PutByte(0x3C);
               adapter.Speed = OWSpeed.SPEED_OVERDRIVE;
                */

               adapter.SpeedChangeEvent += adapter_SpeedChanged;
            }

            deviceAddressList.Items.Clear();
            deviceAddressList.Text = "";
         }
         catch(Exception ex)
         {
            Console.WriteLine("Failed to set adapter: " + ex.Message);
            Console.WriteLine("StackTrace = " + ex.StackTrace);
            addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            AdapterStatus.Text = "Not Loaded";
            if(adapter!=null)
               adapter.Dispose();
            adapter = null;
         }		
      }

      private void menuItem2_Click(object sender, System.EventArgs e)
      {
         this.Close();
      }

      private void resetButton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               adapter.Speed = OWSpeed.SPEED_REGULAR;
               adapter.Reset();

               addText("Reset: " + adapter.Speed + " - " + adapter.Reset(), Color.Blue);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void odResetButton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               adapter.Speed = OWSpeed.SPEED_OVERDRIVE;

               addText("Reset: " + adapter.Speed + " - " + adapter.Reset(), Color.Blue);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void zeroBitButton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               adapter.PutBit(false);
               addText("Wrote: 0 Read: 0 Speed: " + adapter.Speed, Color.Black);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void oneBitButton_Click(object sender, System.EventArgs e)
      {
         if(adapter!=null)
         {
            try
            {
               bool getBit = adapter.GetBit();;
               addText("Wrote: 1 Read: " + (getBit?1:0) + " Speed: " + adapter.Speed, Color.Black);
            }
            catch(Exception ex)
            {
               addText("Failure: " + ex.Message + "\n" + ex.StackTrace + "\n" + ((ex.InnerException!=null)?ex.InnerException.Message:""), Color.Red);
            }
         }
         else
            addText("No Adapter", Color.Red);
      }

      private void menuItem25_Click(object sender, System.EventArgs e)
      {
         executeScript(sender, e, 0);
      }

      private void menuItem26_Click(object sender, System.EventArgs e)
      {
         executeScript(sender, e, 1);
      }

      private void menuItem27_Click(object sender, System.EventArgs e)
      {
         executeScript(sender, e, 2);
      }

      private void executeScript(object sender, System.EventArgs e, int prefixAction)
      {
         if (openFileDialog.ShowDialog() == DialogResult.OK)
         {
            Stream myStream;
            if ((myStream = openFileDialog.OpenFile()) != null)
            {
               // Insert code to read the stream here.
               StreamReader reader = new StreamReader(myStream);
               EventArgs ea = new EventArgs();
               for (String line = reader.ReadLine(); line != null; line = reader.ReadLine())
               {
                  if (line.StartsWith("DELAY "))
                  {
                     System.Threading.Thread.Sleep(Int32.Parse(line.Substring(6)));
                  }
                  else
                  {
                     if (line.StartsWith("CONT "))
                        line = line.Substring(5);
                     else if (prefixAction == 0)
                        if (menuItemOverdriveSpeed.Checked)
                           odResetButton_Click(this, e);
                        else
                           resetButton_Click(this, ea);
                     else if (prefixAction == 1)
                        resetMatchROMButton_Click(this, ea);
                     else if (prefixAction == 2)
                        resetSkipROMbutton_Click(this, ea);
                     if (line.Contains(" 1 ") || line.Contains(" 0 "))
                     {
                        string[] bits = line.Split(' ');
                        for (int i = 0; i < bits.Length; i++)
                           if (bits[i] == "1")
                              oneBitButton_Click(this, ea);
                           else if (bits[i] == "0")
                              zeroBitButton_Click(this, ea);
                     }
                     else
                     {
                        bytesTextBox.Text = line;
                        touchByteButton_Click(this, ea);
                     }
                  }
               }
               myStream.Close();
            }
         }
      }

      private void startAfterByteRadioButton_CheckedChanged(object sender, System.EventArgs e)
      {
         startAfterBitRadioButton.Checked = !startAfterByteRadioButton.Checked;
      }

      private void startAfterBitRadioButton_CheckedChanged(object sender, System.EventArgs e)
      {
         startAfterByteRadioButton.Checked = !startAfterBitRadioButton.Checked;
      }

      private void strongPullupButton_Click(object sender, System.EventArgs e)
      {
         if(startAfterByteRadioButton.Checked)
            adapter.StartPowerDelivery(DalSemi.OneWire.Adapter.OWPowerStart.CONDITION_AFTER_BYTE);
         else
            adapter.StartPowerDelivery(DalSemi.OneWire.Adapter.OWPowerStart.CONDITION_AFTER_BYTE);
      }

      private void setPowerNormalButton_Click(object sender, System.EventArgs e)
      {
         adapter.SetPowerNormal();
      }


      private static string ToHex(byte[] buff, int off, int len)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder(buff.Length*3);
         sb.Append(buff[off].ToString("X2"));
         for(int i=1; i<len; i++)
         {
            sb.Append(" ");
            sb.Append(buff[off+i].ToString("X2"));
         }
         return sb.ToString();
      }

      private static byte[] FromHex(string s)
      {
         s = System.Text.RegularExpressions.Regex.Replace(s.ToUpper(), "[^0-9A-F]", "");
         byte[] b = new byte[s.Length / 2];
         for (int i = 0; i < s.Length; i += 2)
            b[i / 2] = byte.Parse(s.Substring(i, 2), 
               System.Globalization.NumberStyles.AllowHexSpecifier);

         return b;
      }

      private void menuItemStandardSpeed_Click(object sender, EventArgs e)
      {
         adapter.Speed = OWSpeed.SPEED_REGULAR;
      }

      private void menuItemOverdriveSpeed_Click(object sender, EventArgs e)
      {
         adapter.Speed = OWSpeed.SPEED_OVERDRIVE;
      }

      private void adapter_SpeedChanged(object sender, SpeedChangeEventInfo info)
      {
         addText("Change Speed From: " + info.OldSpeed + " To: " + info.NewSpeed, Color.Black);
         menuItemOverdriveSpeed.Checked = info.NewSpeed == OWSpeed.SPEED_OVERDRIVE;
         menuItemStandardSpeed.Checked = info.NewSpeed == OWSpeed.SPEED_REGULAR;
      }
   }
}
