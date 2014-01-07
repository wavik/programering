/*--------------------------------------------------------------------------
 | Copyright (C) 1992-2008 Dallas Semiconductor/MAXIM Corporation.
 | All rights Reserved. Printed in U.S.A.
 | This software is protected by copyright laws of
 | the United States and of foreign countries.
 | This material may also be protected by patent laws of the United States
 | and of foreign countries.
 | This software is furnished under a license agreement and/or a
 | nondisclosure agreement and may only be used or copied in accordance
 | with the terms of those agreements.
 | The mere transfer of this software does not imply any licenses
 | of trade secrets, proprietary technology, copyrights, patents,
 | trademarks, maskwork rights, or any other form of intellectual
 | property whatsoever. Dallas Semiconductor retains all ownership rights.
 |--------------------------------------------------------------------------*/

//
//  LOOPIT64.C - This is a demo program using the 64-bit TMEX DLL
//               'IBFS64.DLL' to loop and display the registration number of the 
//               iButtons on that port.
//
//  Compiler: Microsoft Visual Studio 2005
//  Version: 4.02
//

#include <stdio.h>
#include <conio.h>
#include <windows.h>

// function prototypes
void  UnLoadTMEX(void);
short LoadTMEX(void);

// globals
static FARPROC Get_Version, TMGetTypeVersion, TMEndSession;
static FARPROC TMSetup, TMNext, TMRom, ExtendedStartSession;
static FARPROC TMReadDefaultPort;
long (__fastcall *TMExtendedStartSession)(short,short,void *);

static HINSTANCE hInst;
unsigned char state_buf[5125];

//--------------------------------------------------------------------------
// Main of LOOPIT64
//
void main(int argc, char **argv)
{
   char buf[200];
   short flag,i,didsetup=0;
   short ROM[9];
   short PortNum,PortType;
   long SHandle;
        
   // load the TMEX driver and get pointers to functions 
   if (!LoadTMEX())
   {
      printf("ERROR, could not load IBFS64.DLL\n");
      exit(0);
   }

   // load the TMEX driver and get pointers to functions 
   TMReadDefaultPort(&PortNum, &PortType);
      
   // get the TMEX driver version
   printf("\nLOOPIT64 Version 4.02\n"
         "MAXIM Integrated Products\n"
         "Copyright (C) 1992-2008\n\n");
   printf("Port number: %d     Port type: %d\n",PortNum,PortType);
   Get_Version(buf);
   printf("Main Driver: %s\n",buf);
   printf("TYPE%d:",PortType);
   if ((short)TMGetTypeVersion(PortType,buf) < 0)
   {
      printf("\nNo Hardware Driver for this type found!\n");
      // Unload the TMEX driver
      UnLoadTMEX();
      exit(0);
   }
   printf(" %s\n\n\n",buf);

   // check the command line
   if (argc > 1)
      PortNum = atoi(argv[1]);

   // check for valid range of PortNum
   if ((PortNum < 1) || (PortNum > 15))
   {
      printf("ERROR, invalid port requested: %d\n",PortNum);
      exit(0);
   }

   // loop to display the rom numbers until key hit
   do
   {
      // get a session handle to the requested port
      SHandle = TMExtendedStartSession(PortNum,PortType,NULL);
      if (SHandle > 0)
      {
         // check to see if TMSetup has been done once
         if (!didsetup)
         {
            flag = (short)TMSetup(SHandle);
            if (flag == 1 || flag == 2)
            {
               printf("TMSetup complete %d\n",flag);
               didsetup = 1;
            }
            else
            {
               printf("ERROR doing setup %d\n",flag);
               break;
            }
         }
         // only get the next rom after setup complete
         else
         {
            flag = (short)TMNext(SHandle,(void far *)&state_buf[0]);
            if (flag > 0)
            {
               ROM[0] = 0;
               flag = (short)TMRom(SHandle,(void far *)&state_buf[0],(short far *)&ROM[0]);
               for (i = 7; i >= 0; i--)
                  printf("%02X ",ROM[i]);
               printf("\n");
            }
            else
               printf("end of search\n");
         }

         // close the opened session 
         TMEndSession(SHandle);
      }
   } 
   while (!_kbhit());

   // Unload the TMEX driver
   UnLoadTMEX();

   printf("LOOPIT64 end\n");
}


//--------------------------------------------------------------------------
// Load the TMEX driver and get a pointers to the functions
//
short LoadTMEX(void)
{
   // attempt to get a SHandle to the TMEX driver
   hInst = LoadLibrary(L"IBFS64.DLL");

   // get a pointer to the function needed by loopit64
   if (hInst != NULL)
   {
      ExtendedStartSession = GetProcAddress(hInst,"TMExtendedStartSession");
      TMEndSession = GetProcAddress(hInst,"TMEndSession");
      TMSetup = GetProcAddress(hInst,"TMSetup");
      TMNext = GetProcAddress(hInst,"TMNext");
      TMRom = GetProcAddress(hInst,"TMRom");
      Get_Version = GetProcAddress(hInst,"Get_Version");
      TMGetTypeVersion = GetProcAddress(hInst,"TMGetTypeVersion");
	   TMReadDefaultPort = GetProcAddress(hInst, "TMReadDefaultPort");
   
      // check to make sure got ALL of the functions needed
      if ((ExtendedStartSession == NULL) || (TMEndSession == NULL) ||
         (TMSetup == NULL) || (TMNext == NULL) ||
         (TMRom == NULL) || (Get_Version == NULL) ||
         (TMGetTypeVersion == NULL) || (TMReadDefaultPort == NULL))
      {
         printf("ERROR, could not get a pointer to all"
                " of the TMEX functions needed\n");
         return 0;
      }
      // get a function pointer that returns a long
      TMExtendedStartSession = (long (__fastcall *)(short,short,void *))ExtendedStartSession;

      return 1;
   }
   else
      return 0;
}


//--------------------------------------------------------------------------
// UnLoad the TMEX driver
//
void UnLoadTMEX(void)
{
   // release the TMEX driver
   FreeLibrary(hInst);
}

