/*---------------------------------------------------------------------------
 * Copyright (C) 2010 Maxim Integrated Products, All Rights Reserved.
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
 * TMEX.cs -- This module is a P/Invoke module into TMEX.  It uses delegates to dynamically 
 * switch between x86 TMEX and x64 TMEX.  It does not encompass every TMEX function, but 
 * only a subset.  Note that it does not currently support any 1-Wire file operations.
 * 
 * Version -- 1.0.0
 * 
 * Usage -- From your x86 or x64 TMEX program, just instantiate a TMEX object and then 
 * use the methods.  Instantiate like:  
 * 
 * TMEX tmex = new TMEX();
 * 
 * Then use the associated methods:
 * tmex.TMExtendedStartSession(...);
 * 
 * Please note that a different class is available for useful TMEX constants.  You do not 
 * need to instantiate the class -- just call the variables.  For example:
 * 
 * TMEX_Constants.FEATURE_OVERDRIVE
 */

using System;
using System.Runtime.InteropServices;


// delegates for TMEX
//---------------------------------------------------------------------------
// TMEX - Session
public delegate int TMExtendedStartSession(short portNum, short portType, ref int sessionOptions);
public delegate int TMStartSession(short i1);
public delegate short TMValidSession(int sessionHandle);
public delegate short TMEndSession(int sessionHandle);
// TMEX - Network
public delegate short TMFirst(int sessionHandle, byte[] stateBuffer);
public delegate short TMNext(int sessionHandle, byte[] stateBuffer);
public delegate short TMAccess(int sessionHandle, byte[] stateBuffer);
public delegate short TMStrongAccess(int sessionHandle, byte[] stateBuffer);
public delegate short TMStrongAlarmAccess(int sessionHandle, byte[] stateBuffer);
public delegate short TMOverAccess(int sessionHandle, byte[] stateBuffer);
public delegate short TMRom(int sessionHandle, byte[] stateBuffer, short[] ROM);
public delegate short TMSearch(int sessionHandle, byte[] stateBuffer, short doResetFlag, 
                               short skipResetOnSearchFlag, short searchCommand);
// TMEX - Transport
public delegate short TMBlockIO(int sessionHandle, byte[] dataBlock, short len);
// TMEX - Hardware Specific
public delegate short TMSetup(int sessionHandle);
public delegate short TMTouchReset(int sessionHandle);
public delegate short TMTouchByte(int sessionHandle, short byteValue);
public delegate short TMTouchBit(int sessionHandle, short bitValue);
public delegate short TMProgramPulse(int sessionHandle);
public delegate short TMClose(int sessionHandle);
public delegate short TMOneWireCom(int sessionHandle, short command, short argument);
public delegate short TMOneWireLevel(int sessionHandle, short command, short argument, short changeCondition);
public delegate short TMGetTypeVersion(int portType, System.Text.StringBuilder sbuff);
public delegate short Get_Version(System.Text.StringBuilder sbuff);
public delegate short TMBlockStream(int sessionHandle, byte[] dataBlock, short len);
public delegate short TMGetAdapterSpec(int sessionHandle, byte[] adapterSpec);
public delegate short TMReadDefaultPort(ref short portTypeRef, ref short portNumRef);

public class TMEX
{
   // TMEX - Session

   /// <summary>int TMExtendedStartSession(short portNum, short portType, ref int sessionOptions)
   /// <para>request a 1-Wire network port</para>
   /// </summary>
   public TMExtendedStartSession TMExtendedStartSession;

   /// <summary>int TMStartSession(short i1)
   /// <para>(old format) request a 1-Wire network port</para>
   /// </summary>
   public TMStartSession TMStartSession;

   /// <summary>short TMValidSession(int sessionHandle)
   /// <para>check to see if the 1-Wire network port session is still valid</para>
   /// </summary>
   public TMValidSession TMValidSession;

   /// <summary>short TMEndSession(int sessionHandle)
   /// <para>relinquish a 1-Wire network port</para>
   /// </summary>
   public TMEndSession TMEndSession;

   // TMEX - Network

   /// <summary>short TMFirst(int sessionHandle, byte[] stateBuffer)
   /// <para>find the first device on a 1-Wire network</para>
   /// </summary>
   public TMFirst TMFirst;

   /// <summary>short TMNext(int sessionHandle, byte[] stateBuffer)
   /// <para>find the next device on a 1-Wire network</para>
   /// </summary>
   public TMNext TMNext;

   /// <summary>short TMNext(int sessionHandle, byte[] stateBuffer)
   /// <para>select the current device</para>
   /// </summary>
   public TMAccess TMAccess;

   /// <summary>short TMStrongAccess(int sessionHandle, byte[] stateBuffer)
   /// <para>verify that the current device is still there and select</para>
   /// </summary>
   public TMStrongAccess TMStrongAccess;

   /// <summary>short TMStrongAlarmAccess(int sessionHandle, byte[] stateBuffer)
   /// <para>same as TMStrongAccess except it must be alarming</para>
   /// </summary>
   public TMStrongAlarmAccess TMStrongAlarmAccess;

   /// <summary>short TMOverAccess(int sessionHandle, byte[] stateBuffer)
   /// <para>select the current device and switch it into overdrive speed</para>
   /// </summary>
   public TMOverAccess TMOverAccess;

   /// <summary>short TMRom(int sessionHandle, byte[] stateBuffer, short[] ROM)
   /// <para>read the current device ROM or set the ROM for the next select</para>
   /// </summary>
   public TMRom TMRom;

   /// <summary>short TMSearch(int sessionHandle, byte[] stateBuffer, short doResetFlag, short skipResetOnSearchFlag, short searchCommand)
   /// <para>performs general search for 1-Wire devices</para>
   /// </summary>
   public TMSearch TMSearch;

   // TMEX - Transport

   /// <summary>short TMBlockIO(int sessionHandle, byte[] dataBlock, short len)
   /// <para>transfer a block to the 1-Wire network</para>
   /// </summary>
   public TMBlockIO TMBlockIO;

   // TMEX - Hardware Specific

   /// <summary>short TMSetup(int sessionHandle)
   /// <para>verify the port exists</para>
   /// </summary>
   public TMSetup TMSetup;

   /// <summary>short TMTouchReset(int sessionHandle)
   /// <para>reset 1-Wire devices on 1-Wire network</para>
   /// </summary>
   public TMTouchReset TMTouchReset;

   /// <summary>short TMTouchByte(int sessionHandle, short byteValue)
   /// <para>one byte communication to 1-Wire network</para>
   /// </summary>
   public TMTouchByte TMTouchByte;

   /// <summary>short TMTouchBit(int sessionHandle, short bitValue)
   /// <para>one bit communication to 1-Wire network</para>
   /// </summary>
   public TMTouchBit TMTouchBit;

   /// <summary>short TMProgramPulse(int sessionHandle)
   /// <para>send a programming pulse to 1-Wire network</para>
   /// </summary>
   public TMProgramPulse TMProgramPulse;

   /// <summary>short TMClose(int sessionHandle)
   /// <para>power down a port (not always applicable)</para>
   /// </summary>
   public TMClose TMClose;

   /// <summary>short TMOneWireCom(int sessionHandle, short command, short argument)
   /// <para>set the 1-Wire communication speed</para>
   /// </summary>
   public TMOneWireCom TMOneWireCom;

   /// <summary>short TMOneWireLevel(int sessionHandle, short command, short argument, short changeCondition)
   /// <para>set the 1-Wire communication level</para>
   /// </summary>
   public TMOneWireLevel TMOneWireLevel;

   /// <summary>short TMGetTypeVersion(int portType, System.Text.StringBuilder sbuff)
   /// <para>read the version string of the hardware specific driver</para>
   /// </summary>
   public TMGetTypeVersion TMGetTypeVersion;

   /// <summary>short Get_Version(System.Text.StringBuilder sbuff)
   /// <para>read the version string of the main TMEX driver</para>
   /// </summary>
   public Get_Version Get_Version;

   /// <summary>short TMBlockStream(int sessionHandle, byte[] dataBlock, short len)
   /// <para>transfer a stream to the 1-Wire network</para>
   /// </summary>
   public TMBlockStream TMBlockStream;

   /// <summary>short TMGetAdapterSpec(int sessionHandle, byte[] adapterSpec)
   /// <para>returns 1-Wire adapter feature-set information</para>
   /// </summary>
   public TMGetAdapterSpec TMGetAdapterSpec;

   /// <summary>short TMReadDefaultPort(ref short portTypeRef, ref short portNumRef)
   /// <para>reads the default 1-Wire port and type from the system registry</para>
   /// </summary>
   public TMReadDefaultPort TMReadDefaultPort;

   public TMEX()
   {
      if (IntPtr.Size == 4) // if IntPtr == 4, then we have detected x86 platform, else it is x64
      {
         // x86 Instantiation
         //System.Windows.Forms.MessageBox.Show("x86");
         // TMEX - Session
         TMExtendedStartSession = new TMExtendedStartSession(TMEX32.TMExtendedStartSession);
         TMStartSession = new TMStartSession(TMEX32.TMStartSession);
         TMValidSession = new TMValidSession(TMEX32.TMValidSession);
         TMEndSession = new TMEndSession(TMEX32.TMEndSession);
         // TMEX - Network
         TMFirst = new TMFirst(TMEX32.TMFirst);
         TMNext = new TMNext(TMEX32.TMNext);
         TMAccess = new TMAccess(TMEX32.TMAccess);
         TMStrongAccess = new TMStrongAccess(TMEX32.TMStrongAccess);
         TMStrongAlarmAccess = new TMStrongAlarmAccess(TMEX32.TMStrongAlarmAccess);
         TMOverAccess = new TMOverAccess(TMEX32.TMOverAccess);
         TMRom = new TMRom(TMEX32.TMRom);
         TMSearch = new TMSearch(TMEX32.TMSearch);
         // TMEX - Transport
         TMBlockIO = new TMBlockIO(TMEX32.TMBlockIO);
         // TMEX - Hardware Specific
         TMSetup = new TMSetup(TMEX32.TMSetup);
         TMTouchReset = new TMTouchReset(TMEX32.TMTouchReset);
         TMTouchByte = new TMTouchByte(TMEX32.TMTouchByte);
         TMTouchBit = new TMTouchBit(TMEX32.TMTouchBit);
         TMProgramPulse = new TMProgramPulse(TMEX32.TMProgramPulse);
         TMClose = new TMClose(TMEX32.TMClose);
         TMOneWireCom = new TMOneWireCom(TMEX32.TMOneWireCom);
         TMOneWireLevel = new TMOneWireLevel(TMEX32.TMOneWireLevel);
         TMGetTypeVersion = new TMGetTypeVersion(TMEX32.TMGetTypeVersion);
         Get_Version = new Get_Version(TMEX32.Get_Version);
         TMBlockStream = new TMBlockStream(TMEX32.TMBlockStream);
         TMGetAdapterSpec = new TMGetAdapterSpec(TMEX32.TMGetAdapterSpec);
         TMReadDefaultPort = new TMReadDefaultPort(TMEX32.TMReadDefaultPort);
      }
      else
      {
         // x64 Instantiation
         //System.Windows.Forms.MessageBox.Show("x64");
         // TMEX - Session
         //TMExtendedStartSession = new tmextendedstartsession(TMEX64.TMExtendedStartSession);
         TMExtendedStartSession = new TMExtendedStartSession(TMEX64.TMExtendedStartSession);
         TMStartSession = new TMStartSession(TMEX64.TMStartSession);
         TMValidSession = new TMValidSession(TMEX64.TMValidSession);
         TMEndSession = new TMEndSession(TMEX64.TMEndSession);
         // TMEX - Network
         TMFirst = new TMFirst(TMEX64.TMFirst);
         TMNext = new TMNext(TMEX64.TMNext);
         TMAccess = new TMAccess(TMEX64.TMAccess);
         TMStrongAccess = new TMStrongAccess(TMEX64.TMStrongAccess);
         TMStrongAlarmAccess = new TMStrongAlarmAccess(TMEX64.TMStrongAlarmAccess);
         TMOverAccess = new TMOverAccess(TMEX64.TMOverAccess);
         TMRom = new TMRom(TMEX64.TMRom);
         TMSearch = new TMSearch(TMEX64.TMSearch);
         // TMEX - Transport
         TMBlockIO = new TMBlockIO(TMEX64.TMBlockIO);
         // TMEX - Hardware Specific
         TMSetup = new TMSetup(TMEX64.TMSetup);
         TMTouchReset = new TMTouchReset(TMEX64.TMTouchReset);
         TMTouchByte = new TMTouchByte(TMEX64.TMTouchByte);
         TMTouchBit = new TMTouchBit(TMEX64.TMTouchBit);
         TMProgramPulse = new TMProgramPulse(TMEX64.TMProgramPulse);
         TMClose = new TMClose(TMEX64.TMClose);
         TMOneWireCom = new TMOneWireCom(TMEX64.TMOneWireCom);
         TMOneWireLevel = new TMOneWireLevel(TMEX64.TMOneWireLevel);
         TMGetTypeVersion = new TMGetTypeVersion(TMEX64.TMGetTypeVersion);
         Get_Version = new Get_Version(TMEX64.Get_Version);
         TMBlockStream = new TMBlockStream(TMEX64.TMBlockStream);
         TMGetAdapterSpec = new TMGetAdapterSpec(TMEX64.TMGetAdapterSpec);
         TMReadDefaultPort = new TMReadDefaultPort(TMEX64.TMReadDefaultPort);
      }
   }
   public bool[] getFeaturesFromSpecification(byte[] adapterSpec)
   {
      bool[] features = new bool[32];
      for (int i = 0; i < 32; i++)
         features[i] = (adapterSpec[i * 2] > 0) || (adapterSpec[i * 2 + 1] > 0);
      return features;
   }

   public System.String getDescriptionFromSpecification(byte[] adapterSpec)
   {
      int i;
      // find null terminator for string
      for (i = 64; i < 319; i++)
         if (adapterSpec[i] == 0)
            break;
      return new System.String(
         System.Text.UTF8Encoding.UTF8.GetChars(adapterSpec), 64, i - 64);
   }
}
public static class TMEX_Constants
{
   /// <summary>feature indexes into adapterSpecFeatures array </summary>
   public const int FEATURE_OVERDRIVE = 0;
   public const int FEATURE_POWER = 1;
   public const int FEATURE_PROGRAM = 2;
   public const int FEATURE_FLEX = 3;
   public const int FEATURE_BREAK = 4;
   /// <summary>Speed settings for TMOneWireCOM </summary>
   public const short TIME_NORMAL = 0;
   public const short TIME_OVERDRV = 1;
   public const short TIME_RELAXED = 2;
   /// <summary>for TMOneWireLevel</summary>
   public const short LEVEL_NORMAL = 0;
   public const short LEVEL_STRONG_PULLUP = 1;
   public const short LEVEL_BREAK = 2;
   public const short LEVEL_PROGRAM = 3;
   public const short PRIMED_NONE = 0;
   public const short PRIMED_BIT = 1;
   public const short PRIMED_BYTE = 2;
   public const short LEVEL_READ = 1;
   public const short LEVEL_SET = 0;
   /// <summary>session options </summary>
   public const int SESSION_INFINITE = 1;
   public const int SESSION_RSRC_RELEASE = 2;
}
public static class TMEX32
{
   // TMEX - Session
   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMExtendedStartSession")]
   public static extern int TMExtendedStartSession(short portNum, short portType, ref int sessionOptions);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMStartSession")]
   public static extern int TMStartSession(short i1);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMValidSession")]
   public static extern short TMValidSession(int sessionHandle);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMEndSession")]
   public static extern short TMEndSession(int sessionHandle);

   // TMEX - Transport
   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMBlockIO")]
   public static extern short TMBlockIO(int sessionHandle, byte[] dataBlock, short len);

   // TMEX - Network
   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMFirst")]
   public static extern short TMFirst(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMNext")]
   public static extern short TMNext(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMAccess")]
   public static extern short TMAccess(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMStrongAccess")]
   public static extern short TMStrongAccess(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMStrongAlarmAccess")]
   public static extern short TMStrongAlarmAccess(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMOverAccess")]
   public static extern short TMOverAccess(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMRom")]
   public static extern short TMRom(int sessionHandle, byte[] stateBuffer, short[] ROM);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMSearch")]
   public static extern short TMSearch(int sessionHandle, byte[] stateBuffer, short doResetFlag, short skipResetOnSearchFlag, short searchCommand);

   // TMEX - Hardware Specific
   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMSetup")]
   public static extern short TMSetup(int sessionHandle);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMTouchReset")]
   public static extern short TMTouchReset(int sessionHandle);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMTouchByte")]
   public static extern short TMTouchByte(int sessionHandle, short byteValue);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMTouchBit")]
   public static extern short TMTouchBit(int sessionHandle, short bitValue);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMProgramPulse")]
   public static extern short TMProgramPulse(int sessionHandle);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMClose")]
   public static extern short TMClose(int sessionHandle);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMOneWireCom")]
   public static extern short TMOneWireCom(int sessionHandle, short command, short argument);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMOneWireLevel")]
   public static extern short TMOneWireLevel(int sessionHandle, short command, short argument, short changeCondition);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMGetTypeVersion")]
   public static extern short TMGetTypeVersion(int portType, System.Text.StringBuilder sbuff);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "Get_Version")]
   public static extern short Get_Version(System.Text.StringBuilder sbuff);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMBlockStream")]
   public static extern short TMBlockStream(int sessionHandle, byte[] dataBlock, short len);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMGetAdapterSpec")]
   public static extern short TMGetAdapterSpec(int sessionHandle, byte[] adapterSpec);

   [DllImportAttribute("IBFS32.dll", EntryPoint = "TMReadDefaultPort")]
   public static extern short TMReadDefaultPort(ref short portTypeRef, ref short portNumRef);
}

public static class TMEX64
{
   // TMEX - Session
   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMExtendedStartSession")]
   public static extern int TMExtendedStartSession(short portNum, short portType, ref int sessionOptions);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMStartSession")]
   public static extern int TMStartSession(short i1);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMValidSession")]
   public static extern short TMValidSession(int sessionHandle);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMEndSession")]
   public static extern short TMEndSession(int sessionHandle);

   // TMEX - Transport
   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMBlockIO")]
   public static extern short TMBlockIO(int sessionHandle, byte[] dataBlock, short len);

   // TMEX - Network
   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMFirst")]
   public static extern short TMFirst(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMNext")]
   public static extern short TMNext(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMAccess")]
   public static extern short TMAccess(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMStrongAccess")]
   public static extern short TMStrongAccess(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMStrongAlarmAccess")]
   public static extern short TMStrongAlarmAccess(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMOverAccess")]
   public static extern short TMOverAccess(int sessionHandle, byte[] stateBuffer);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMRom")]
   public static extern short TMRom(int sessionHandle, byte[] stateBuffer, short[] ROM);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMSearch")]
   public static extern short TMSearch(int sessionHandle, byte[] stateBuffer, short doResetFlag, short skipResetOnSearchFlag, short searchCommand);

   // TMEX - Hardware Specific
   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMSetup")]
   public static extern short TMSetup(int sessionHandle);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMTouchReset")]
   public static extern short TMTouchReset(int sessionHandle);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMTouchByte")]
   public static extern short TMTouchByte(int sessionHandle, short byteValue);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMTouchBit")]
   public static extern short TMTouchBit(int sessionHandle, short bitValue);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMProgramPulse")]
   public static extern short TMProgramPulse(int sessionHandle);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMClose")]
   public static extern short TMClose(int sessionHandle);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMOneWireCom")]
   public static extern short TMOneWireCom(int sessionHandle, short command, short argument);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMOneWireLevel")]
   public static extern short TMOneWireLevel(int sessionHandle, short command, short argument, short changeCondition);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMGetTypeVersion")]
   public static extern short TMGetTypeVersion(int portType, System.Text.StringBuilder sbuff);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "Get_Version")]
   public static extern short Get_Version(System.Text.StringBuilder sbuff);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMBlockStream")]
   public static extern short TMBlockStream(int sessionHandle, byte[] dataBlock, short len);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMGetAdapterSpec")]
   public static extern short TMGetAdapterSpec(int sessionHandle, byte[] adapterSpec);

   [DllImportAttribute("IBFS64.dll", EntryPoint = "TMReadDefaultPort")]
   public static extern short TMReadDefaultPort(ref short portTypeRef, ref short portNumRef);
}