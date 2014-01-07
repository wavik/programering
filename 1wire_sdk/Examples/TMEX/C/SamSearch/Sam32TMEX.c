//---------------------------------------------------------------------------
// Copyright (C) 2001 - 2002 Dallas Semiconductor/MAXIM Corporation, All Rights Reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL DALLAS SEMICONDUCTOR BE LIABLE FOR ANY CLAIM, DAMAGES 
// OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
// OTHER DEALINGS IN THE SOFTWARE.
// 
// Except as contained in this notice, the name of Dallas Semiconductor 
// shall not be used except as stated in the Dallas Semiconductor 
// Branding Policy. 
//---------------------------------------------------------------------------
//
//     Module Name: Sam32TMEX
//
//     Description: Replacement for SAM32MULTI but uses the TMEX drivers 
//                  instead of sauth
//
//        Filename: sam32tmex.c
//
// Dependant Files: adapter.h
//
//          Author: DS
//
//        Compiler: MSVC on Win32 x86 ONLY
//
//         Version: Ver 3.20
//
//         Created: 02/21/01
//   

// includes
#include <stdio.h>
#include <windows.h>
#include "adapter.h"
#include <time.h>

// typedefs
typedef unsigned char  uchar;
typedef unsigned short ushort;
typedef unsigned long  ulong;

// defines
#define VERSION_MAJOR         0
#define VERSION_MINOR         2

#define TRUE                  1
#define FALSE                 0

// session options
#define SESSION_INFINITE            0x0001
#define SESSION_RSRC_RELEASE        0x0002 
#define SESSION_NO_FORCE_REG_SPD    0x0004

// debug options
#define DEBUG_OFF             0
#define DEBUG_ERROR           1
#define DEBUG_IO              2
#define DEBUG_VERBOSE         3
#define DEBUG_ADAPTER         4
#define DEBUG_DETAIL          5

#define PLRG_INTEGER PLARGE_INTEGER
#define LRG_INTEGER LARGE_INTEGER

// external functions
static long (far pascal *TMExtendedStartSession)(short,short,void far *);
static short (far pascal *TMEndSession)(long);
static short (far pascal *TMValidSession)(long);
static short (far pascal *TMSetup)(long);
static short (far pascal *TMClose)(long);
static short (far pascal *TMTouchReset)(long);
static short (far pascal *TMTouchByte)(long, short);
static short (far pascal *TMTouchBit)(long, short);
static short (far pascal *TMOneWireCom)(long, short, short);
static short (far pascal *TMOneWireLevel)(long, short, short, short);
static short (far pascal *TMGetTypeVersion)(short, char *);
static short (far pascal *Get_Version)(char *);
static short (far pascal *TMBlockStream)(long, unsigned char *, short);
static short (far pascal *TMFirst)(long, uchar *);
static short (far pascal *TMNext)(long, uchar *);
static short (far pascal *TMAccess)(long, uchar *);
static short (far pascal *TMStrongAccess)(long, uchar *);
static short (far pascal *TMRom)(long, uchar *, short *);

// local exportable functions
ulong GetAccessAPIVersion();
uchar iBOverdriveOn(void);
void  iBOverdriveOff(void);
uchar iBDOWCheck(void);
uchar iBKeyOpen(void);
uchar iBKeyClose(void);
uchar iBFirst(void);
uchar iBNext(void);
uchar iBFastAccess(void);
uchar iBStrongAccess(void);
uchar iBAccess(void);
uchar * iBROMData(void);
uchar iBDataByte(uchar data);
uchar iBDataBlock(uchar *data, int count);
uchar iBSetup(uchar pn);
uchar iBDataBit(uchar bit);
uchar iBDOWReset();
uchar SetAdapterType(uchar Type, char *type);
uchar SetAdapterSpeed(ulong speed);
uchar SetAdapter5VTime(uchar time);
uchar Adapter5VPrime(void);
uchar Adapter5VCancel(void);
int   Adapter5VActivate(ulong ustimeout);
uchar iBStreamRS(uchar *arr,int length,int run);
uchar iBStream(uchar *arr,int length);
void iBReleaseResources();
void FastSleep(DWORD Delay);
void iBSetAutoReleaseResources(uchar);

// local static functions
static void UnLoadTMEX(void);
static int  LoadTMEX(void);
static int  BlockStartSession();
static void checkROMBuffer();
static void updateROMBuffer();
static void MyInt64Add(PLRG_INTEGER pA, PLRG_INTEGER pB);
static BOOL MyInt64LessThanOrEqualTo(PLRG_INTEGER pA, PLRG_INTEGER pB);
static int  CheckDebug();
static void StopDebug();
static int dprintf(char *format, ...);

// globals
static HINSTANCE hTMEXInst;
static uchar state_buf[5125];
static uchar driverROM[8];
static uchar userROM[8];
static short PortNum=1,PortType=2;
static long SHandle;
static int TMEXDriverLoaded=FALSE;
static int SetupDone=FALSE;
static int PendingOverdriveOff=FALSE;
static int ShortCut=FALSE;
static ulong SessionFlags=SESSION_INFINITE | SESSION_NO_FORCE_REG_SPD;
static int DebugLevel=0;
static FILE *DFile;

const char version[] = "SA to TMEX interface 3.20";


//--------------------------------------------------------------------------
// Set all devices to overdrive
//
uchar iBOverdriveOn(void)
{
   if (!TMEXDriverLoaded)
      return FALSE;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBOverdriveOn\n",SHandle);

   if (PendingOverdriveOff)
      PendingOverdriveOff = FALSE;

   // if not already in overdrive
   if (TMOneWireCom(SHandle,1,0) != 1)
   {
      // not using short cut
      ShortCut = FALSE;

      // first set to normal speed
      if (TMOneWireCom(SHandle,0,0) == 0)
      {
         // Put all overdrive parts into overdrive mode. 
         if (!iBDOWReset())
            return FALSE;

         iBDataByte(0x3C);
   
         return (TMOneWireCom(SHandle,0,1) == 1);
      }
   }
   else 
   { 
      // using short cut
      ShortCut = TRUE;
      return TRUE;
   }

   if (DebugLevel != 0)
      dprintf("__Sam32TMEX_%04X_iBOverdriveOn failed\n",SHandle);

   return FALSE;
}

//--------------------------------------------------------------------------
// Set to regular speed
//
void iBOverdriveOff(void)
{
   if (!TMEXDriverLoaded)
      return;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBOverdriveOff\n",SHandle);

   // don't really turn off but remember it is pending
   PendingOverdriveOff = TRUE;
}

//--------------------------------------------------------------------------
// Description: Open a handle to the device. This function
//              must be used in all applications.
//              The device is specified using SetPortType().
//              - Checks if driver is loaded
// Returns:     TRUE on success, FALSE on failure
//
uchar iBDOWCheck(void) 
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBDOWCheck\n",SHandle);

   if (!TMEXDriverLoaded)
      TMEXDriverLoaded = LoadTMEX();

   return TMEXDriverLoaded;
}

//--------------------------------------------------------------------------
// Open a session on the current adapter
//
uchar iBKeyOpen(void)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBKeyOpen\n",SHandle);

   if (!TMEXDriverLoaded)
      return FALSE;

   if (BlockStartSession())
   {
      if (!SetupDone)
      {
         if (TMSetup(SHandle) == 1)
         {
            SetupDone = TRUE;
         }
         else
         {
            iBKeyClose();
            return FALSE;
         }
      }

      return TRUE;
   }

   return FALSE;
}

//--------------------------------------------------------------------------
// Close the session
//
uchar iBKeyClose(void)
{
   short rslt;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBKeyClose\n",SHandle);

   rslt = TMEndSession(SHandle);
   SHandle = 0;

   return (rslt == 1);
}

//--------------------------------------------------------------------------
// Find first device and update rom buffer
//
uchar iBFirst(void)
{
   if (!TMEXDriverLoaded)
      return FALSE;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBFirst\n",SHandle);

   if (PendingOverdriveOff)
   {
      TMOneWireCom(SHandle,0,0);
      PendingOverdriveOff = FALSE;
   }

   if (TMFirst(SHandle,state_buf) > 0)
   {
      updateROMBuffer();
      return TRUE;
   }

   return FALSE;
}

//--------------------------------------------------------------------------
// Find next device and update rom buffer
//
uchar iBNext(void)
{
   if (!TMEXDriverLoaded)
      return FALSE;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBNext\n",SHandle);

   if (PendingOverdriveOff)
   {
      TMOneWireCom(SHandle,0,0);
      PendingOverdriveOff = FALSE;
   }

   if (TMNext(SHandle,state_buf) > 0)
   {
      updateROMBuffer();
      return TRUE;
   }

   return FALSE;
}

//--------------------------------------------------------------------------
// Perform a 'fast' or MATCH access.
//
uchar iBFastAccess(void)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBFastAccess\n",SHandle);

   // if doing the overdrive shortcut, then use strong access
   if (ShortCut)
      return iBStrongAccess(); 
   else
      return iBAccess(); 
}

//--------------------------------------------------------------------------
// Perform a 'fast' or MATCH access.
//
uchar iBAccess(void)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBAccess\n",SHandle);

   if (PendingOverdriveOff)
   {
      TMOneWireCom(SHandle,0,0);
      PendingOverdriveOff = FALSE;
   }

   // check to make sure state_buf is in sync with userROM
   checkROMBuffer();

   return (TMAccess(SHandle,state_buf) == 1); 
}

//--------------------------------------------------------------------------
// Perform a 'strong' or SEARCH access
//
uchar iBStrongAccess(void)
{
   short rslt;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBStrongAccess\n",SHandle);

   if (!TMEXDriverLoaded)
      return FALSE;

   if (PendingOverdriveOff)
   {
      TMOneWireCom(SHandle,0,0);
      PendingOverdriveOff = FALSE;
   }

   // check to make sure state_buf is in sync with userROM
   checkROMBuffer();

   rslt = TMStrongAccess(SHandle,state_buf);

   // check if device was present, do something special if in overdrive
   if ((rslt != 1) && (TMOneWireCom(SHandle,1,0) != 0))
   {
      // set back to standard speed
      TMOneWireCom(SHandle,0,0); 
      // attempt to get back to overdrive and verify device present
      iBOverdriveOn();
      rslt = TMStrongAccess(SHandle,state_buf);
   }

   return (rslt == 1);
}

//--------------------------------------------------------------------------
// Get pointer to the rom array
//
uchar * iBROMData(void)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBROMData\n",SHandle);

   return userROM;
}

//--------------------------------------------------------------------------
// Do 1 byte of communication
//
uchar iBDataByte(uchar data)
{
   short rslt;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBDataByte(%02X)\n",SHandle,data);

   if (!TMEXDriverLoaded)
      return 0;

   rslt = TMTouchByte(SHandle,data);
   if (rslt >= 0)
      return (uchar)rslt;
   else
      return 0;
}

//--------------------------------------------------------------------------
// Block I/O command. Transmit data in blocks of size PAR_BLOCK_SIZE. 
// Return received bytes in the same buffer.
//
uchar iBDataBlock(uchar *data, int count)
{
   if (!TMEXDriverLoaded)
      return FALSE;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBDataBlock, len %d\n",SHandle,count);

   return (TMBlockStream(SHandle,data,(short)count) == (short)count);
}

//--------------------------------------------------------------------------
// Assign the port number
//
uchar iBSetup(uchar pn)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBSetup port %d\n",SHandle,pn);

   // if current adapter type is serial then ignore
   if (PortType == 5)
      return TRUE;

   // check for invalid port number
   if ((pn <= 0) || (pn > 15))
   {
      // set to invalid number so iBKeyOpen will fail
      PortNum = -1;
      SetupDone = FALSE;
      driverROM[0] = 0;
      return FALSE;
   }

   // check if type is new
   if (PortNum != pn)
   {
      SetupDone = FALSE;
      driverROM[0] = 0;
      PortNum = pn;
   }

   return TRUE;
}

//--------------------------------------------------------------------------
// Perform 1-Wire bit
//
uchar iBDataBit(uchar bit)
{
   short rslt;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBDataBit, value %d\n",SHandle,bit);

   if (!TMEXDriverLoaded)
      return FALSE;

   rslt = TMTouchBit(SHandle,bit);
   if (rslt >= 0)
      return (uchar)rslt;
   else
      return 0;
}

//--------------------------------------------------------------------------
// Perform 1-Wire reset
//
uchar iBDOWReset() 
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBDOWReset\n",SHandle);

   if (!TMEXDriverLoaded)
      return FALSE;

   return (TMTouchReset(SHandle) == 1);
}

//--------------------------------------------------------------------------
// Set the desired adapter type
//
uchar SetAdapterType(uchar Type, char *type)
{
   short newPortType;
   uchar pn;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_SetAdapterType, type=%d, string=%s\n",
             SHandle,Type,type);

   switch(Type)
   {
      case SERIAL_DS2480:
         newPortType = 5;
         // make sure that there is a string 
         if (type == NULL)
            return FALSE;
         // blindly extract the port number from the string
         pn = type[3] - 0x30;   
         // check for invalid port number
         if ((pn <= 0) || (pn > 15))
         {
            // set to invalid number so iBKeyOpen will fail
            PortNum = -1;
            SetupDone = FALSE;
            driverROM[0] = 0;
            return FALSE;
         }
         // check if type is new
         if (PortNum != pn)
         {
            SetupDone = FALSE;
            driverROM[0] = 0;
            PortNum = pn;
         }
         break;
      case PARALLEL_DS1481:
         newPortType = 2;
         break;
      case SERIAL_DS1413:
         newPortType = 1;
         break;
      case USB_DS2490:
         newPortType = 6;
         break;
      default:
         newPortType = Type;
         break;
   }

   // check if type is new
   if (newPortType != PortType)
   {
      SetupDone = FALSE;
      driverROM[0] = 0;
      PortType = newPortType;
   }

   return TRUE;
}

//--------------------------------------------------------------------------
// Description: Set the Speed of a Serial 2480 adapter.  Does nothing if
//              some other adapter.
// Returns:     TRUE on success, FALSE on failure
//
uchar SetAdapterSpeed(ulong speed)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_SetAdapterSpeed %d, do nothing\n",SHandle,speed);

   // handled by TMEX driver, so noop
   return TRUE; 
}

//--------------------------------------------------------------------------
//  Set 5V time, only accept infinite time
//
uchar SetAdapter5VTime(uchar time)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_SetAdapter5VTime to %d\n",SHandle,time);

   return (time == PARMSET_infinite);
}

//--------------------------------------------------------------------------
// Prime for 5V after next bit
//
uchar Adapter5VPrime(void)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_Adapter5VPrime\n",SHandle);

   if (!TMEXDriverLoaded)
      return FALSE;

   return (TMOneWireLevel(SHandle,0,1,1) >= 0);
}

//--------------------------------------------------------------------------
// Turn off strong 5V
//
uchar Adapter5VCancel(void)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_Adapter5VCancel\n",SHandle);

   if (!TMEXDriverLoaded)
      return FALSE;

   return (TMOneWireLevel(SHandle,0,0,0) >= 0);
}

//--------------------------------------------------------------------------
// Manual 5V 
//
int Adapter5VActivate(ulong ustimeout)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_Adapter5VActivate for time %d\n",SHandle,ustimeout);

   if (!TMEXDriverLoaded)
      return FALSE;

   if (TMOneWireLevel(SHandle,0,1,0) >= 0)
   {
      Sleep(ustimeout/1000 + 1);
      TMOneWireLevel(SHandle,0,0,0);
      return TRUE;
   }

   return FALSE;
}

//--------------------------------------------------------------------------
// Select and send a block or a release sequence (run=1).
//
uchar iBStreamRS(uchar *arr,int length,int run)
{
   int AccessGood = FALSE;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBStreamRS, run %d, len %d\n",SHandle,run,length);

   if (!TMEXDriverLoaded)
      return FALSE;

   if (iBAccess())
   {
      if (iBDataBlock(arr, length))
      {
         if (run == 1)
         {
            Adapter5VPrime();
            return (iBDataBit(1) == 1);
         }

         return TRUE;
      }
   }

   return FALSE;
}

//--------------------------------------------------------------------------
// Select and send a block to the current device
//
uchar iBStream(uchar *arr,int length)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_iBStream, len %d\n",SHandle,length);

   if (!TMEXDriverLoaded)
      return FALSE;

   if (iBAccess())
      return iBDataBlock(arr, length);

   return FALSE;
}

//--------------------------------------------------------------------------
// Get the version of the Access Layer.
//
ulong GetAccessAPIVersion()
{
  return (((unsigned long)VERSION_MINOR)<<16) + VERSION_MAJOR;
}

//--------------------------------------------------------------------------
// Set the auto-relase resource option.
//
// auto_release : 1 (TRUE), resources will be release after each iBKeyClose
//                0 (TRUE), resources may be kept between sessions
//
void iBSetAutoReleaseResources(uchar auto_release)
{
   if (auto_release)
      SessionFlags=SESSION_INFINITE | SESSION_NO_FORCE_REG_SPD | SESSION_RSRC_RELEASE;
   else
      SessionFlags=SESSION_INFINITE | SESSION_NO_FORCE_REG_SPD;
}

//--------------------------------------------------------------------------
// Release all resources for this layer.  Note that 'iBDOWCheck' would need
// to be called again to do 1-Wire.
//
void iBReleaseResources()
{
   short types[] = { 1, 2, 5, 6 };
   short num_ports[] = { 4, 3, 4, 15 };
   short port, adapter;

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_iBReleaseResources\n");

   if (TMEXDriverLoaded)
   {
      // free session if currently in one
      TMEndSession(SHandle);

      // attempt to loop through all adapters/ports and call TMClose
      // to clean up any resources 
      for (adapter = 0; adapter < 4; adapter++)
      {
         for (port = 1; port <= num_ports[adapter]; port++)
         {
            SHandle = TMExtendedStartSession(port,types[adapter],&SessionFlags);
            if (SHandle > 0)
            {
               TMClose(SHandle);
               TMEndSession(SHandle);
            }
         }
      }
   
      // unload the drivers
      UnLoadTMEX();
   }

   // reset globals
   SetupDone=FALSE;
   PendingOverdriveOff=FALSE;
   ShortCut=FALSE;
   driverROM[0] = 0;
}

//--------------------------------------------------------------------------
// Block waiting for a TMEX session.  Timout is 60 seconds
// Return: TRUE session aquired
//         FALSE timeout
//
static int BlockStartSession()
{
   ulong timeout = GetTickCount() + 60000;

   //????????????????????????
   //???SessionFlags = SESSION_INFINITE | SESSION_NO_FORCE_REG_SPD | SESSION_RSRC_RELEASE;
   //????????????????????????

   do
   {
      SHandle = TMExtendedStartSession(PortNum,PortType,&SessionFlags);

      // check for error in type (no DLL) or invalid PortNum
      if (SHandle < 0)
         return FALSE;
   } 
   while ((SHandle <= 0) && (timeout > GetTickCount()));

   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_%04X_BlockStartSession took %ld ms\n",SHandle, 60000l - (timeout - GetTickCount())); 

   return (SHandle > 0);
}

//--------------------------------------------------------------------------
// Check to make sure state_buf is in sync with userROM 
//
static void checkROMBuffer()
{
   short ROM[8];
   int i,j;

   for (i = 0; i < 8; i++)
   {
      if (userROM[i] != driverROM[i])
      {
         if (DebugLevel > 2)
            dprintf("__Sam32TMEX_%04X_CheckROM, rom was changed by user\n",SHandle);

         // ROM has been changed by user
         for (j = 0; j < 8; j++)
         {
            ROM[j] = userROM[j];
            driverROM[j] = userROM[j];
         }

         // set ROM in state_buf
         TMRom(SHandle,state_buf,&ROM[0]);

         return;
      }
   }
}

//--------------------------------------------------------------------------
// Sync the userROM with state_buf 
//
static void updateROMBuffer()
{
   short ROM[8];
   int i;

   // get ROM in state_buf
   ROM[0] = 0;
   TMRom(SHandle,state_buf,&ROM[0]);

   for (i = 0; i < 8; i++)
   {
      userROM[i] = (uchar)ROM[i];
      driverROM[i] = (uchar)ROM[i];
   }
}

//--------------------------------------------------------------------------
// Load the TMEX driver and get a pointers to the functions
//
static int LoadTMEX(void)
{
   // check on debuging
   CheckDebug();

   // attempt to get a SHandle to the TMEX driver
   hTMEXInst = LoadLibrary("IBFS32.DLL");

   // get a pointer to the function needed by loopit32
   if (hTMEXInst != NULL)
   {
      TMExtendedStartSession = (long (far pascal *)(short,short,void far *))GetProcAddress(hTMEXInst,"TMExtendedStartSession");
      TMEndSession = (short (far pascal *)(long))GetProcAddress(hTMEXInst,"TMEndSession");
      TMValidSession = (short (far pascal *)(long))GetProcAddress(hTMEXInst,"TMValidSession");
      TMSetup = (short (far pascal *)(long))GetProcAddress(hTMEXInst,"TMSetup");
      TMClose = (short (far pascal *)(long))GetProcAddress(hTMEXInst,"TMClose");
      TMNext = (short (far pascal *)(long, uchar *))GetProcAddress(hTMEXInst,"TMNext");
      TMFirst = (short (far pascal *)(long, uchar *))GetProcAddress(hTMEXInst,"TMFirst");
      TMRom = (short (far pascal *)(long, uchar *, short *))GetProcAddress(hTMEXInst,"TMRom");
      TMAccess = (short (far pascal *)(long, uchar *))GetProcAddress(hTMEXInst,"TMAccess");
      TMStrongAccess = (short (far pascal *)(long, uchar *))GetProcAddress(hTMEXInst,"TMStrongAccess");
      TMBlockStream = (short (far pascal *)(long, unsigned char *, short))GetProcAddress(hTMEXInst,"TMBlockStream");
      TMTouchBit = (short (far pascal *)(long, short))GetProcAddress(hTMEXInst,"TMTouchBit");
      TMTouchByte = (short (far pascal *)(long, short))GetProcAddress(hTMEXInst,"TMTouchByte");
      TMTouchReset = (short (far pascal *)(long))GetProcAddress(hTMEXInst,"TMTouchReset");
      TMOneWireCom = (short (far pascal *)(long, short, short))GetProcAddress(hTMEXInst,"TMOneWireCom");
      TMOneWireLevel = (short (far pascal *)(long, short, short, short))GetProcAddress(hTMEXInst,"TMOneWireLevel");
      Get_Version = (short (far pascal *)(char *))GetProcAddress(hTMEXInst,"Get_Version");
      TMGetTypeVersion = (short (far pascal *)(short, char *))GetProcAddress(hTMEXInst,"TMGetTypeVersion");
   
      // check to make sure got ALL of the functions needed
      if ((TMExtendedStartSession == NULL) || (TMEndSession == NULL) || 
         (TMSetup == NULL) || (TMNext == NULL) || (TMValidSession == NULL) ||
         (TMRom == NULL) || (TMFirst == NULL) || (TMAccess == NULL) ||
         (TMStrongAccess == NULL) || (TMBlockStream == NULL) ||
         (TMTouchBit == NULL) || (TMTouchByte == NULL) || (TMTouchReset == NULL) ||
         (TMOneWireCom == NULL) || (TMOneWireLevel == NULL) || (Get_Version == NULL) ||
         (TMGetTypeVersion == NULL) || (TMClose == NULL))
      {
         UnLoadTMEX();
         return FALSE;
      }

      if (DebugLevel > 2)
         dprintf("__Sam32TMEX_LoadTMEX, driver loaded\n");

      return TRUE;
   }
   else
   {
      if (DebugLevel != 0)
         dprintf("__Sam32TMEX_LoadTMEX, driver NOT loaded\n");

      return FALSE;
   }
}

//--------------------------------------------------------------------------
// UnLoad the TMEX driver
//
static void UnLoadTMEX(void)
{
   if (DebugLevel > 2)
      dprintf("__Sam32TMEX_UnloadTMEX\n");

   // release the TMEX driver
   FreeLibrary(hTMEXInst);
   TMEXDriverLoaded=FALSE;

   // stop debug output to file
   StopDebug();
}

//--------------------------------------------------------------------------
// Peform a fast sleep, delay in us
//
void FastSleep(DWORD Delay)
{
   LRG_INTEGER CountsPerSec, CurrentCount, FinalCount, DelayCounts;
   BOOL          UseOldSleep = TRUE;
   short valid;

   if (DebugLevel > 1)
      dprintf("__Sam32TMEX_%04X_FastSleep, for %d ms\n",SHandle,Delay);

   if (QueryPerformanceFrequency(&CountsPerSec))
   {
      if (CountsPerSec.HighPart == 0L)
      {
         DelayCounts.HighPart = 0;
         DelayCounts.LowPart  = (CountsPerSec.LowPart / 1000L) * Delay;
         
         if (DelayCounts.LowPart)
         {
            /* Get current count value */
            QueryPerformanceCounter(&FinalCount);
            /* FinalCount += DelayCounts */
            MyInt64Add(&FinalCount, &DelayCounts);
            
            do
            {
               // feed session while doing sleep
               valid = TMValidSession(SHandle);
               if (valid != 1)
               {
                  if (DebugLevel != 0)
                     dprintf("__Sam32TMEX_%04X_FastSleep, "
                            "Session became invalid in fastsleep\n");
               }

               SleepEx(1, FALSE);
               QueryPerformanceCounter(&CurrentCount);   
            }
            while (MyInt64LessThanOrEqualTo(&CurrentCount, &FinalCount));
            
            UseOldSleep = FALSE;
         }
      }
   }
   
   if (UseOldSleep)
   {
      /* Use regular sleep if hardware doesn't support a performance counter */
      SleepEx(Delay, FALSE);
   }
   return;
}

//--------------------------------------------------------------------------
// Int64 <= 
//
static BOOL MyInt64LessThanOrEqualTo(PLRG_INTEGER pA, PLRG_INTEGER pB)
{
   BOOL CompRes = FALSE;
   
   if (pA->HighPart < pB->HighPart)
      CompRes = TRUE;
   else if ((pA->HighPart == pB->HighPart) && (pA->LowPart <= pB->LowPart))
      CompRes = TRUE;
   
   return CompRes;
}

//--------------------------------------------------------------------------
// Int64 add
//
static void MyInt64Add(PLRG_INTEGER pA, PLRG_INTEGER pB)
{
   BYTE c = 0;
   
   if ((pA->LowPart != 0L) && (pB->LowPart != 0L))
      c = ((pA->LowPart + pB->LowPart) < pA->LowPart) ? 1 : 0;
   
   pA->LowPart  += pB->LowPart;
   pA->HighPart += pB->HighPart + c;
   return;
}

//--------------------------------------------------------------------------
// Read the registry to see if we are doing debug
//
static int CheckDebug()
{
   HKEY GlobKey;
   DWORD dwType,size;
   char ReturnString[256];
   char DebugFileName[256];
   time_t tlong; 
   struct tm *tstruct;

   // check if already doing debug
   if (DebugLevel != 0)
      return DebugLevel;

   // read the default PortNum and PortType from the registry 
   if (RegOpenKeyEx(HKEY_LOCAL_MACHINE,
         "Software\\Dallas Semiconductor\\iButton TMEX",0,
         KEY_READ,&GlobKey) == ERROR_SUCCESS)
   {
      // attempt to read the PortNum 
      size = 255;
      if (RegQueryValueEx(GlobKey,"DebugLevel",NULL,&dwType,
          &ReturnString[0],&size) == ERROR_SUCCESS)
      {
         if ((dwType == REG_SZ) && (size >= 1))
         {
            DebugLevel = atoi(ReturnString);
            if ((DebugLevel < 0) || (DebugLevel > 9))
            {
               fprintf(stderr,"ERROR, bad debug level found %s, should be a number!\n",ReturnString);
               DebugLevel = 0;
            }
         }
      }

      if (DebugLevel != 0)
      {
         // attempt to read the PortNum 
         sprintf(DebugFileName,"c:\\onewirelog.txt");
         size = 255;
         if (RegQueryValueEx(GlobKey,"DebugFile",NULL,&dwType,
             &DebugFileName[0],&size) == ERROR_SUCCESS)
         {
            if ((dwType != REG_SZ) || (size <= 0))
               sprintf(DebugFileName,"c:\\onewirelog.txt");
         }
      }
      
      // close the key
      RegCloseKey(GlobKey);
   }

   // if doing debug then open the file for writing
   if (DebugLevel == 0)
      return 0;

   DFile = fopen(DebugFileName,"a+");
   if(DFile == NULL)
   {
      fprintf(stderr,"ERROR, Could not open debug file!\n");
      DebugLevel = 0;
   }
   else
   {
      time(&tlong);
      tstruct = localtime(&tlong); 
      dprintf("\n__Sam32TMEX>> opened log level %d at"
          " %02d/%02d/%04d %02d:%02d:%02d by process %04X\n",DebugLevel,
          tstruct->tm_mon + 1,tstruct->tm_mday,tstruct->tm_year + 1900, 
          tstruct->tm_hour,tstruct->tm_min,tstruct->tm_sec,(DWORD)GetCurrentProcessId());
      dprintf("__Sam32TMEX>> driver version: %s\n",version);
   }
      
   return DebugLevel;
}

//--------------------------------------------------------------------------
// Stop Debug 
//
static void StopDebug()
{
   if (DebugLevel != 0)
   {
      fflush(DFile);
      fclose(DFile);
      DebugLevel = 0;
   }
}

//--------------------------------------------------------------------------
// Debug printf to opened file DFile with a force flush 
//
// Return: number of characters printed
//
int dprintf(char *format, ...)
{
   int rt;
   va_list ap;

   va_start(ap, format);
   rt = vfprintf(DFile,format,ap); 
   va_end(ap);

   fflush(DFile);

   return rt;
}

