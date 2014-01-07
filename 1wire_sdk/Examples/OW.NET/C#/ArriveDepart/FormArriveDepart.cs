/*---------------------------------------------------------------------------
 * Copyright (C) 2011 Maxim Integrated Products, All Rights Reserved.
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
 *
 *
 * Arrive/Depart program showing how to use the 1-Wire API's ability to sense 
 * arrivals and departurs of 1-Wire / iButton devices on the 1-Wire network.  It 
 * uses 
 * 
 * version 0.0.1
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using com.dalsemi.onewire;
using com.dalsemi.onewire.adapter;
using com.dalsemi.onewire.container;
using com.dalsemi.onewire.application.monitor;
using System.Windows.Forms;

namespace ArriveDepart
{
   // this form implements "DeviceMonitorEventListener"
   public partial class FormArriveDepart : Form, DeviceMonitorEventListener

   {
      DSPortAdapter adapter = OneWireAccessProvider.getDefaultAdapter();
      DeviceMonitor dMonitor;
      java.lang.Thread searchThread;

      // This delegate enables asynchronous calls for setting
      // the text property on a TextBox control.
      delegate void SetTextCallback(string text);

      public FormArriveDepart()
      {
         InitializeComponent();
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         try
         {
            // get exclusive use of adapter
            adapter.beginExclusive(true);

            // clear any previous search restrictions
            adapter.setSearchAllDevices();
            adapter.targetAllFamilies();
            adapter.setSpeed(DSPortAdapter.SPEED_REGULAR);
            adapter.endExclusive();

            // Monitor of the network
            dMonitor = new DeviceMonitor(adapter);
            dMonitor.setDoAlarmSearch(false);
            
            // setup event listener (should point to this form)
            dMonitor.addDeviceMonitorEventListener(this);

            // start the monitor in a java thread
            searchThread = new java.lang.Thread(dMonitor);
            searchThread.start();
         }
         catch (Exception exc)
         {
            textBoxResults.AppendText("General exception occurred: " + exc.ToString());
         }
      }

      // The following 3 methods implement the J# interface "DeviceMonitorEventListener" from the 1-Wire API (J#).  
      // These methods are: deviceArrival, deviceDeparture, and networkException.
      //

      // Departure method for the 1-Wire API's DeviceMonitorEventListener interface
      public void deviceDeparture(DeviceMonitorEvent devt)
      {
         int i = 0;
         for (i = 0; i < devt.getDeviceCount(); i++)
         {
            SetText("Departing: " + devt.getAddressAsStringAt(i) + Environment.NewLine);
         }
      }


      // Arrival method for the 1-Wire API's DeviceMonitorEventListener interface
      public void deviceArrival(DeviceMonitorEvent devt)
      {
         int i = 0;
         for (i = 0; i < devt.getDeviceCount(); i++)
         {
            SetText("Arriving: " + devt.getAddressAsStringAt(i) + Environment.NewLine);
         }
      }

      // exception for DeviceMonitor
      public void networkException(DeviceMonitorException dexc)
      {
         SetText("1-Wire network exception occurred: " + Environment.NewLine);
      }

      // This method demonstrates a pattern for making thread-safe
      // calls on a Windows Forms control. 
      //
      // If the calling thread is different from the thread that
      // created the TextBox control, this method creates a
      // SetTextCallback and calls itself asynchronously using the
      // Invoke method.
      //
      // If the calling thread is the same as the thread that created
      // the TextBox control, the Text property is set directly. 

      private void SetText(string text)
      {
         // InvokeRequired required compares the thread ID of the
         // calling thread to the thread ID of the creating thread.
         // If these threads are different, it returns true.
         if (this.textBoxResults.InvokeRequired)
         {
            SetTextCallback d = new SetTextCallback(SetText);
            this.Invoke(d, new object[] { text });
         }
         else
         {
            this.textBoxResults.AppendText(text);
         }
      }

      private void Form1_FormClosing(object sender, FormClosingEventArgs e)
      {
         // end the 1-Wire network Device Monitor
         if (dMonitor.isMonitorRunning())
         {
            SetText("Killing deviceMonitor" + Environment.NewLine);
            dMonitor.killMonitor();
         }
         // cleanup after ourselves by destroying the thread
         if (searchThread.isAlive())
         {
            searchThread.destroy();
         }
      }
   }
}