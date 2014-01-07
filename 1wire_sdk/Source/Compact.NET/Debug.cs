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

namespace DalSemi
{
	/// <summary>
	/// Summary description for Debug.
	/// </summary>
   public class Debug
   {
      private static bool enabled = true;
      public static bool Enabled
      {
         set
         {
            Debug.enabled = value;
         }
         get
         {
            return Debug.output!=null && Debug.enabled;
         }
      }
      private static System.IO.TextWriter output = null;
      public static System.IO.TextWriter Output
      {
         set
         {
            Debug.output = value;
         }
         get
         {
            return Debug.output;
         }
      }

      public static void WriteLine()
      {
         if(Debug.Enabled)
         {
            Debug.output.WriteLine();
         }
      }

      public static void Write(Object o)
      {
         if(Debug.Enabled)
         {
            Debug.output.Write(o);
         }
      }

      public static void WriteLine(Object o)
      {
         if(Debug.Enabled)
         {
            Debug.output.WriteLine(o);
         }
      }

      public static void Write(String s)
      {
         if(Debug.Enabled)
         {
            Debug.output.Write(s);
         }
      }

      public static void WriteLine(String s)
      {
         if(Debug.Enabled)
         {
            Debug.output.WriteLine(s);
         }
      }

      public static void Write(String s, params object[] args)
      {
         if(Debug.Enabled)
         {
            Debug.output.Write(s, args);
         }
      }

      public static void WriteLine(String s, params object[] args)
      {
         if(Debug.Enabled)
         {
            Debug.output.WriteLine(s, args);
         }
      }

      public static void WriteLineHex(String lbl, byte[] data, int offset, int length)
      {
         if(Debug.Enabled)
         {
            Debug.output.Write(lbl);
            for(int i=0; i<length; i++)
            {
               Debug.output.Write(" ");
               Debug.output.Write(data[i].ToString("X2"));
            }
            Debug.output.WriteLine();
         }
      }

      public static void WriteInfo()
      {
         if(Debug.Enabled)
         {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            WriteLine("OS: " + Environment.OSVersion);
            WriteLine("Version: " + Environment.Version);
            WriteLine("Assembly: " + assembly.FullName);
         }
      }
   }
}
