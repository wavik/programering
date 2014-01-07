/*--------------------------------------------------------------------------
 * Copyright (C) 1992-2002 Dallas Semiconductor/MAXIM Corporation. 
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
 * property whatsoever. Dallas Semiconductor/MAXIM retains all ownership rights.
 *--------------------------------------------------------------------------
 *
 *  TIMEVC32.C - This utility uses TMEX to read the time from 
 *               a DS1994. It requires the TMEX
 *               drivers to be present.
 *
 * Compiler:  Microsoft Visual C 6.0
 * Version: 4.00
 *
 */

#define TMEXUTIL

#include "iBTMEXCW.H"
#include <stdio.h>  

/* type structure to holde time/date */
typedef struct          
{
     ushort  frsecond;
     ushort  second;
     ushort  minute;
     ushort  hour;
     ushort  day;
     ushort  month;
     ushort  year;
} timedate;

/* Local Function Prototypes */
short FindFirstFamily(short, long);
void Exit_Prog(char *, short);
short ReadTime(long, uchar far *);
void ConvertTime(timedate far *, uchar far *);
ulong uchar_to_bin(int, uchar far *);

/* Global Variables to hold list information */
short PortType, PortNum;
uchar state_buffer[5120];

/*----------------------------------------------------------------------
 *  This is the Main routine for SWVC.
 */
void main(short argc, char **argv)
{
   short printmsg=1,ClearActivity=0;
   char tstr[80];       
   long session_handle;       
   timedate td;                  

   /* check arguments to see if request instruction with '?' or too many */
   if ((argc > 2) && (argv[1][0] == '?' || argv[1][1] == '?'))
   {
       printf("\nusage: timevc32 <PortNum> \n"
              "  - read the time from a DS1994 \n"
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
   printf("\nTIMEVC32 Version 4.00\n"
         "Dallas Semiconductor/MAXIM Corporation\n"
         "Copyright (C) 1992-2002\n\n");
   printf("Port number: %d    Port type: %d\n",PortNum,PortType);
   Get_Version(tstr);
   printf("Main Driver: %s\n",tstr);
   printf("Type%d:",PortType);
   if (TMGetTypeVersion(PortType,tstr) < 0)
      Exit_Prog("No Hardware Driver for this type found!",NO_DRIVERS);
   printf(" %s\n\n\n",tstr);

   // loop to get a session handle and read the time */
   for (;;)
   {
      /* attempt to get a session */
      session_handle = TMExtendedStartSession(PortNum,PortType,NULL);
      if (session_handle > 0)  
      {
         /* setup the 1-Wire network */
         if (TMSetup(session_handle) == 1)
         {
            printf("Searching for a DS1994...\n\n");
            
            /* look for the first time iButton (family type 0x04) */
            if (FindFirstFamily(0x04,session_handle))
            {
               /* read the time register */
               if (ReadTime(session_handle,tstr) == 1)
               {
                  ConvertTime(&td,&tstr[0]);
                  printf(" %02d/%02d/%02d  %2d:%02d:%02d.%02d\n\n",
                         td.month,td.day,td.year,td.hour,td.minute,
                         td.second,td.frsecond); 
                  break;
               }
               else
               {
                  printf("Error reading DS1994 time register!"); 
                  break;
               }
            }
            /* Temperature iButton not on 1-Wire network */
            else   
            {
               printf("DS1994 not found on 1-Wire network!"); 
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
 * Read the time into the buffer buf.
 */                                                          
short ReadTime(long session_handle, uchar far *buf)
{    
   short printmsg=1,ch,cnt=0,change,i;   

   /* loop to read the time value (loop until two read the same) */
   do
   {
      change = TRUE;
      /* access and verify it is there */
      if (TMAccess(session_handle,&state_buffer) == 1) 
      {
         /* send read memory command */
         TMTouchByte(session_handle,0xF0);
         /* send address */                 
         TMTouchByte(session_handle,0x02);
         TMTouchByte(session_handle,0x02);
               
         /* loop to read in the time register */
         change = FALSE;
         for (i = 0; i < 5; i++)
         {
            ch = TMTouchByte(session_handle,0xFF);
            if (ch != buf[i])
            {         
               /* don't count change in fractional seconds as a change */
               if (i != 0)
                  change = TRUE;         
               buf[i] = (uchar)ch; 
            }     
         }
      }
   }
   while (change && (++cnt < 5));

   return !change;
}


/*--------------------------------------------------------------------------
 * Take a 5 byte long string and convert it into a timedata structure.
 */
static int dm[] = { 0,0,31,59,90,120,151,181,212,243,273,304,334,365 };

void ConvertTime(timedate far *td, uchar far *ptr)
{
   short tmp,i,j;
   ulong x,y;
   
   /* get value of number of seconds */
   x = uchar_to_bin(4,&ptr[1]);
   
   /* get the hundredths of a second */
   tmp = (99*(ptr[0]+2))/255;
   
   td->frsecond = tmp;
   
   /* check to make sure date is not over 2020 */
   if (x > 0x5FDD4280L)
      x = 0;
   
   y = x/60;  td->second = (ushort)(x-60*y);
   x = y/60;  td->minute = (ushort)(y-60*x);
   y = x/24;  td->hour   = (ushort)(x-24*y);
   x = 4*(y+731);  td->year = (ushort)(x/1461);
   i = (int)((x-1461*(ulong)(td->year))/4);  td->month = 13;
   
   do
   {
      td->month -= 1;
      tmp = (td->month > 2) && ((td->year & 3)==0) ? 1 : 0;
      j = dm[td->month]+tmp;
   
   } while (i < j);
   
   td->day = i-j+1;
   
   /* slight adjustment to algorithm */
   if (td->day == 0) td->day = 1;
   
   td->year = (td->year < 32)  ? td->year + 68 : td->year - 32;
}


/*--------------------------------------------------------------------------
 * UCHAR_TO_BIN converts a binary uchar string 'str' into a ulong return
 * number.  'num' indicates the length of the binary uchar string.
 */
ulong uchar_to_bin(int num, uchar far *str)
{
   short i;
   ulong l = 0;
      
   for (i = (num-1); i >= 0; i--)
      l = (l << 8) | (int)(str[i]);
      
   return l;
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

                               
