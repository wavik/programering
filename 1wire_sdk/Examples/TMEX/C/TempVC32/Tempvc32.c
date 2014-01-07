/*--------------------------------------------------------------------------
 * Copyright (C) 1992-2002 Dallas Semiconductor Corporation. 
 * All rights Reserved. Printed in U.S.A.
 * This software is protected by copyright laws of
 * the United States and of foreign countries.
 * This material may also be protected by patent laws of the United States 
 * and of foreign countries.
 * This software is furnished under a license agreement and/or a
 * nondisclosure agreement and may only be used or copied in accordance
 * with the terms of those agreements.
 * The mere transfer of this software does not imply any licenses
 * of trade secrets, proprietary technology, copyrights, patents,
 * trademarks, maskwork rights, or any other form of intellectual
 * property whatsoever. Dallas Semiconductor retains all ownership rights.
 *--------------------------------------------------------------------------
 *
 *  TEMPVC32.C - This utility uses TMEX to view a read the temperature from 
 *               a DS1920/DS1820. It requires the TMEX
 *               drivers to be present.
 *
 * Compiler:  Microsoft Visual C 6.0
 * Version: 4.00
 *
 */

#define TMEXUTIL

#include "iBTMEXCW.H"
#include <stdio.h>   

/* Local Function Prototypes */
short FindFirstFamily(short, long);
short ReadTempScratch(long, uchar *);
short ReadTemperature(long, float far *);
void Exit_Prog(char *, short);

/* Global Variables to hold list information */
short PortType, PortNum;
uchar state_buffer[5120];
short CRC8;                   

/*----------------------------------------------------------------------
 *  This is the Main routine for TEMPVC.
 */
void main(short argc, char **argv)
{
   short printmsg=1;
   char tstr[80];       
   long session_handle;                         
   float current_temp;

   /* check arguments to see if request instruction with '?' or too many */
   if ((argc > 2) && (argv[1][0] == '?' || argv[1][1] == '?'))
   {
       printf("\nusage: tempvc32 <PortNum> \n"
              "  - read the temperature from a DS1920/DS1820 iButton\n"
              "  - <optional> argument 1 specifies the port number\n"
              "  - version 4.00\n");
       Exit_Prog("",BAD_ARGUMENT);
   }

   /* read the default PortNum and PortType */
   PortNum = 1;
   PortType = 5;
   if (TMReadDefaultPort(&PortNum, &PortType) < 1)
   {
     Exit_Prog("ERROR, Could not read the default PortNum and PortType from registry!\n",
               NO_DRIVERS);
   }

   /* check argument to see if provided port number */
   if (argc > 1)
       PortNum = atoi(argv[1]);

   /* check to see that indicated PortNum is  <= 15 */
   if (PortNum > 15)
     Exit_Prog("ERROR, Indicated PortNum is not valid!\n",
               -XONE_WIRE_PORT_ERROR);

   /* print a header */  
   printf("\nTEMPVC32 Version 4.00\n"
         "Dallas Semiconductor/MAXIM Corporation\n"
         "Copyright (C) 1992-2002\n\n");
   printf("Port number: %d    Port type: %d\n",PortNum,PortType);
   Get_Version(tstr);
   printf("Main Driver: %s\n",tstr);
   printf("Type%d:",PortType);
   if (TMGetTypeVersion(PortType,tstr) < 0)
      Exit_Prog("No Hardware Driver for this type found!",NO_DRIVERS);
   printf(" %s\n\n\n",tstr);

   // loop to get a session handle and to a temperature conversion */
   for (;;)
   {
      /* attempt to get a session */
      session_handle = TMExtendedStartSession(PortNum,PortType,NULL);
      if (session_handle > 0)  
      {
         /* setup the 1-Wire network */
         if (TMSetup(session_handle) == 1)
         {
            printf("Searching for a DS1920/DS1820...\n\n");
            
            /* look for the first temperature iButton (family type 0x10) */
            if (FindFirstFamily(0x10,session_handle))
            {             
               /* read the temperature of the found iButton */
               if (ReadTemperature(session_handle,&current_temp) == 1)                    
                  printf("Temperature: %6.1fC (%6.1fF)\n",current_temp,current_temp * 9 / 5 + 32);                                   
               /* failed to read the temperature */
               else
                  printf("Coversion failed!");
   
               MessageBeep(MB_ICONEXCLAMATION);
               break;
            }      
            /* Temperature iButton not on 1-Wire network */
            else   
            {
               printf("DS1920/DS1820 not found on 1-Wire network!"); 
               break;
            }
         }   
         /* 1-Wire network port not valid */
         else   
         {
            printf("1-Wire network setup failed!"); 
            break;
         }
      }
      else if (session_handle == 0)
      {                            
         /* check if need to print the waiting message */
         if (printmsg)
         {  
            printmsg = FALSE;
            printf("\nWaiting to get access to the 1-Wire network\n");
         }
      }
      else if (session_handle < 0)   
      {
         MessageBeep(MB_ICONEXCLAMATION);
         printf("No Hardware Driver for this type found!");
         break;
      }
   }

   /* end the session if one is open */
   TMEndSession(session_handle);
}


/*----------------------------------------------------------------------
 * Find the first device with a particular family
 */
short FindFirstFamily(short family, long session_handle)
{
   short j;
   short ROM[9];
   
   /* set up to find the first device with family 'family */
   if (TMFamilySearchSetup(session_handle,&state_buffer,family) == 1)
   { 
      /* get first device in list with the specified family */
      if (TMNext(session_handle,state_buffer) == 1)
      {
         /* read the rom number */
         ROM[0] = 0;
         TMRom(session_handle,state_buffer,ROM);   
         
         /* check if correct type */
         if ((family & 0x7F) == (ROM[0] & 0x7F))
         {
            printf("Serial ROM ID: ");  
            for (j = 7; j >= 0; j--)
               printf("%02X",ROM[j]);
            printf("\n\n");
            return 1;
         }        
      }
   }
     
   /* failed to find device of that type */
   return 0;                              
}


/*----------------------------------------------------------------------
 * Get a list of the files on the 'num' device in the ROM list.
 */                                                          
short ReadTemperature(long session_handle, float far *current_temp)
{
   short flag,tsht;
   float tmp,cr,cpc;
   uchar rbuf[10];       
   unsigned long st;
                         
   /* access the device */
   if (TMAccess(session_handle,&state_buffer) != 1)
      return 0;
         
   /* send the Recall E2 command make sure Scratch is correct */
   TMTouchByte(session_handle,0xB8);

   /* send the start T command */
   if (TMAccess(session_handle,&state_buffer) != 1)
      return 0;
   
   /* prepare the strong pullup after next TMTouchByte */
   TMOneWireLevel(session_handle,LEVEL_SET,LEVEL_STRONG_PULLUP,PRIMED_BYTE);
         
   /* send the temperature conversion command */
   TMTouchByte(session_handle,0x44);
         
   /* sleep for a second */
   st = GetTickCount() + 1000; 
   while (GetTickCount() < st)
      TMValidSession(session_handle);

   /* disable the strong pull-up */
   TMOneWireLevel(session_handle,LEVEL_SET,LEVEL_NORMAL,PRIMED_NONE);
   
   /* verify conversion is complete */
   if (TMTouchBit(session_handle,0x01) != 0x01)
      return -100; 
                         
   /* read the scratchpad */
   flag = ReadTempScratch(session_handle,rbuf);
   if (flag != 1)
      return 0;
         
   /* calculate the temperature  */  
   tsht = (rbuf[0] & 0xFE); // according to datasheet, truncate the 0.5 degree Celsius bit (least significant bit)
   if (rbuf[1] & 0x01)
      tsht |= -256;
   tmp = (float)(tsht/2);
   cr = rbuf[6];
   cpc = rbuf[7];
   if (rbuf[7] == 0)
      return 0;   
   else
      tmp = tmp - (float)0.25 + (cpc - cr)/cpc;
   
   *current_temp = tmp;
         
   return 1;   
}


/*------------------------------------------------------------------------
 * Read the scratch of a DS18/1920.
 */
short ReadTempScratch(long session_handle, uchar *buf)
{
   short i;
   unsigned char CRCByte;
                         
   /* access device */
   if (TMAccess(session_handle,&state_buffer) != 1)
      return 0;
         
   /* send read scratch command */
   TMTouchByte(session_handle,0xBE);
                              
   /* zero the dowcrc */                          
   CRC8 = 0;
   
   /* read scratch */
   for (i = 0; i < 8; i++)   
   {
      buf[i] = (unsigned char) TMTouchByte(session_handle,(short)0xFF);
      //dowcrc(buf[i]);
      CRC8 = TMCRC(1, &buf[i], CRC8, 0); /* perform CRC on result */

   }
   
   /* check crc */
   CRCByte = (unsigned char) TMTouchByte(session_handle, (short) 0xFF);
   if (TMCRC(1, &CRCByte, CRC8, 0) != 0)
      return 0;
      
   return 1;   
}                   


/*--------------------------------------------------------------------------
 *  Prints a message, beeps and exits the program.
 */
void Exit_Prog(char *msg, short errcd)
{
     /* print the message left justified */
     printf("\r%s",msg);

     /*  error beep if return code is not 0 */
     if (errcd != NORMAL_EXIT)
        MessageBeep(MB_ICONEXCLAMATION);

     /*  return error code passed */
     exit(errcd);
}

