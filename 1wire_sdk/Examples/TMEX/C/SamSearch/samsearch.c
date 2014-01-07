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
//  SamSearch.c - This utility uses the TMEX wrapper for the Software 
//                Authorization API to list the devices found
//
//  Compiler:  Microsoft Visual C 
//  Version:   1.00
//

#include "sauth95.h"
#include <stdio.h>

//--------------------------------------------------------------------------
// Main of SamSearch
//
void main(int argc, char **argv)
{
   uchar *rom_ptr, search_rslt;
   int i;

   // header
   printf("\nSam32TMEX Demo using TMEX Software Authorization API wrapper\n\n");

   // verify driver
   if (!iBDOWCheck())
   {
      printf("iBDOWCheck failed, could not load driver\n");
      exit(1);
   }

   // select the port 
   if (!SetAdapterType(SERIAL_DS2480, "COM1"))
   {
      printf("SetAdapterType failed, could not select DS2480/COM1\n");
      exit(2);
   }

   // setup the port
   if (!iBSetup(1))
   {
      printf("iBSetup failed, could not setup DS2480/COM1\n");
      exit(2);
   }
  
   // get control of the 1-Wire
   if (iBKeyOpen())
   {
      // get a pointer to the 1-Wire Net Address 
      rom_ptr = iBROMData();
 
      // search 
      iBOverdriveOff();

      // find the first device
      search_rslt = iBFirst();

      // loop while devices still being discovered
      while (search_rslt)
      {
         // print the 1-Wire Net Address
         for (i = 7; i >= 0; i--)
            printf("%02X",rom_ptr[i]);
         printf("\n");

         // find the 'next' device
         search_rslt = iBNext();
      }

      // release the 1-Wire
      iBKeyClose();
   }
   else
   {
      printf("iBKeyOpen failed, could not select 1-Wire\n");
      exit(3);
   }

   printf("\nDone\n");
   exit(0);
}
