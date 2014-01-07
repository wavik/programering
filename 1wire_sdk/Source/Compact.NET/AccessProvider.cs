/*---------------------------------------------------------------------------
 * Copyright (C) 2004 Maxim Integrated Products, All Rights Reserved.
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
using DalSemi.OneWire.Adapter;

namespace DalSemi.OneWire
{
	public class AccessProvider
	{
      public static PortAdapter GetAdapter(string adapterName)
      {
         if(adapterName.Equals("DS9097U"))
         {
            return GetAppropriateSerialAdapter();
         }
         else if (adapterName.Equals("DS9097U-X"))
         {
             return new SerialAdapterX();
         }
         else if (adapterName.Equals("{DS9097U}") || adapterName.Equals("{DS9097U_DS9480}"))
         {
            return new TMEXLibAdapter(TMEXPortType.SerialPort);
         }
         else if(adapterName.Equals("{DS9490}"))
         {
            return new TMEXLibAdapter(TMEXPortType.USBPort);
         }
         else if(adapterName.Equals("{DS9097}"))
         {
            return new TMEXLibAdapter(TMEXPortType.PassiveSerialPort);
         }
         else if(adapterName.Equals("{DS1410E}"))
         {
            return new TMEXLibAdapter(TMEXPortType.ParallelPort);
         }
         else
         {
            throw new AdapterException("Bad adapter name: " + adapterName);
         }
      }

      public static PortAdapter GetAdapter(string adapterName, string portname)
      {
         PortAdapter adapter = GetAdapter(adapterName);
         if(!adapter.OpenPort(portname))
         {
            throw new AdapterException("Failed to open port: " + adapterName + ", " + portname);
         }
         return adapter;
      }

      public static PortAdapter DefaultAdapter
      {
         get
         {
            return GetAdapter("DS9097U", "COM1");
         }
      }

      private static PortAdapter GetAppropriateSerialAdapter()
      {
          try
          {
              SerialAdapter sa = new SerialAdapter();
              // access port names to verify that the proper serial port api exists
              System.Collections.IList list = sa.PortNames;
              return sa;
          }
          catch
          {
              try
              {
                  SerialAdapterX sax = new SerialAdapterX();
                  // access port names to verify that the proper serial port api exists
                  System.Collections.IList list = sax.PortNames;
                  return sax;
              }
              catch
              {
              }
          }
          throw new AdapterException("Cannot load pure serial adapter");
      }

      public static System.Collections.IList GetAllAdapters()
      {
         System.Collections.ArrayList al = new System.Collections.ArrayList();

         try
         {
             al.Add(GetAppropriateSerialAdapter());
         }
         catch { }
         
         try
         {
            al.Add(new TMEXLibAdapter(TMEXPortType.SerialPort));
         }
         catch{}
         
         try
         {
            al.Add(new TMEXLibAdapter(TMEXPortType.PassiveSerialPort));
         }
         catch{}
         
         try
         {
            al.Add(new TMEXLibAdapter(TMEXPortType.ParallelPort));
         }
         catch{}
         
         try
         {
            al.Add(new TMEXLibAdapter(TMEXPortType.USBPort));
         }
         catch{}

         return al;
      }
	}
}
