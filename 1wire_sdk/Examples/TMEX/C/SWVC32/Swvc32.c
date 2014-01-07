/*--------------------------------------------------------------------------
 * Copyright (C) 1992 - 2002 Dallas Semiconductor/MAXIM Corporation. 
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
 *  SWVC32.C - This utility uses TMEX to view and read the switches from 
 *             a DS2406. It requires the TMEX
 *             drivers to be present.
 *
 *  Compiler:  Microsoft Visual C 6.00
 *  Version: 4.00
 *
 */

#define TMEXUTIL

#include "iBTMEXCW.H"
#include <stdio.h>
#include <conio.h>   

/* Local Function Prototypes */
short FindFirstFamily(short, long);
void Exit_Prog(char *, short);
short ReadSwitchInfo(long,short);
void PrintSwitchInfo(short, short far *);
short DisplaySwitchInfo(short,short far *);
short WhichFlipFlop(short);
short SetFlipFlop(short);

/* Global Variables to hold list information */
short PortType, PortNum;
uchar state_buffer[5120];
ushort CRC16;

/*----------------------------------------------------------------------
 *  This is the Main routine for SWVC32.
 */
void main(short argc, char **argv)
{
   short printmsg=1,done,found,channels,ClearActivity=0;
   char tstr[80],ch;       
   long session_handle;                         

   /* check arguments to see if request instruction with '?' or too many */
   if ((argc > 2) && (argv[1][0] == '?' || argv[1][1] == '?'))
   {
       printf("\nusage: swvc32 <PortNum> \n"
              "  - read/set a DS2406 addressable switch\n"
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
   printf("\nSWVC Version 4.00\n"
         "Dallas Semiconductor/MAXIM Corporation\n"
         "Copyright (C) 1992-2002\n\n");
   printf("Port number: %d    Port type: %d\n",PortNum,PortType);
   Get_Version(tstr);
   printf("Main Driver: %s\n",tstr);
   printf("Type%d:",PortType);
   if (TMGetTypeVersion(PortType,tstr) < 0)
      Exit_Prog("No Hardware Driver for this type found!",NO_DRIVERS);
   printf(" %s\n\n\n",tstr);

   /* loop to get a session handle find a switch */
   found = FALSE;
   for (;;)
   {
      /* attempt to get a session */
      session_handle = TMExtendedStartSession(PortNum,PortType,NULL);
      if (session_handle > 0)  
      {
         /* setup the 1-Wire network */
         if (TMSetup(session_handle) == 1)
         {
            printf("Searching for a DS2406...\n\n");
            
            /* look for the first switch iButton (family type 0x12) */
            if (FindFirstFamily(0x12,session_handle))
            {
               found = TRUE;
               break;
            }
            /* Temperature iButton not on 1-Wire network */
            else   
            {
               printf("DS2406 not found on 1-Wire network!"); 
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
            printf("\nWaiting to get access to the 1-Wire network\n\n");
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

   /* continue if a DS2406 was found */
   if (found)                          
   {   
      /* loop while not done */
      done = FALSE;
      do
      {  
         /* always display info first */
         if (DisplaySwitchInfo(ClearActivity,&channels))
         {                              
            /* print menu */
            printf("\n\n(1) Display the switch Info\n"
                 "(2) Clear activity Latches\n"
                 "(3) Set Flip Flop(s) on switch\n"
                 "(4) Quit\n"
                 "Select a Number:");
            ch = getche();
            printf("\n\n");
            
            /* do something from the menu selection */             
            ClearActivity = FALSE;
            switch(ch)
            {        
               case '1': 
                  break;
               case '2':
                  ClearActivity = TRUE;
                  break;
               case '3':
                  if (SetFlipFlop(WhichFlipFlop(channels)) != 1)
                     done = TRUE;         
                  break;
               case '4': case 'q': case 'Q':
                  done = TRUE;  
                  break;                             
               default:
                  MessageBeep(MB_ICONEXCLAMATION);
            };
         }
      }
      while (!done);
   }
}


/*----------------------------------------------------------------------
 * Set the flip flops to the 'ffstate' .
 */                                                          
short SetFlipFlop(short ffstate)
{    
   short printmsg=1,flag,rt=0,cnt=0,done=0;
   long session_handle;
   unsigned char CRCByte;

   // loop to get a session handle and to a temperature conversion */
   do
   {
      /* attempt to get a session */
      session_handle = TMExtendedStartSession(PortNum,PortType,NULL);
      if (session_handle > 0)  
      {
         /* access and verify it is there */
         if (TMAccess(session_handle,&state_buffer) == 1) 
         {
            /* reset CRC */
            CRC16 = 0;
         
            /* write status command */
            flag = TMTouchByte(session_handle,0x55);
			CRCByte = 0x55;
            CRC16 = TMCRC(1, &CRCByte, 0, 1);

         
            /* send address */                 
            TMTouchByte(session_handle,0x07);
			CRCByte = 0x07;
            CRC16 = TMCRC(1, &CRCByte, CRC16, 1);
			
            TMTouchByte(session_handle,0x00);
			CRCByte = 0x00;
            CRC16 = TMCRC(1, &CRCByte, CRC16, 1);			
            
            /* write that status */
            TMTouchByte(session_handle,ffstate);
			CRCByte = (unsigned char) ffstate;
            CRC16 = TMCRC(1, &CRCByte, CRC16, 1);
			
         
            /* crc */
            flag = TMTouchByte(session_handle, 0xFF);
			CRCByte = (unsigned char) flag;
            CRC16 = TMCRC(1, &CRCByte, CRC16, 1);

            flag = TMTouchByte(session_handle, 0xFF);
		    CRCByte = (unsigned char) flag;
            CRC16 = TMCRC(1, &CRCByte, CRC16, 1);

         
            /* check result */
            if (CRC16 == 0xB001)   
               done = TRUE; 
            else
               cnt++;
         }
                                
         /* end the session if one is open */
         TMEndSession(session_handle);
      }
      else if (session_handle == 0)
      {                            
         /* check if need to print the waiting message */
         if (printmsg)
         {  
            printmsg = FALSE;
            printf("\nWaiting to get access to the 1-Wire network\n\n");
         }
      }
   }
   while (!done && (cnt < 10));

   return done;                  
}


/*----------------------------------------------------------------------
 * Read the switch information byte.
 */                                                          
short ReadSwitchInfo(long session_handle, short ClearActivity)
{
   short flag,rt=-1;
   unsigned char CRCByte;
   
   /* access and verify it is there */
   flag = TMAccess(session_handle,&state_buffer);
   if (flag == 1) 
   {
      /* reset CRC */
      CRC16 = 0;
   
      /* channel access command */
      flag = TMTouchByte(session_handle,0xF5);
      CRCByte = 0xF5;
      CRC16 = TMCRC(1, &CRCByte, CRC16, 1);
   
      /* control bytes */                 
      if (ClearActivity)
      {
         TMTouchByte(session_handle,0xD5);
         CRCByte = 0xD5;
	     CRC16 = TMCRC(1, &CRCByte, CRC16, 1);
		 
      }
      else
      {
         TMTouchByte(session_handle,0x55);   
         CRCByte = 0x55;
	     CRC16 = TMCRC(1, &CRCByte, CRC16, 1);		 
      }

      TMTouchByte(session_handle,0xFF);
      CRCByte = 0xFF;
      CRC16 = TMCRC(1, &CRCByte, CRC16, 1);

   
      /* read the info byte */
      rt = TMTouchByte(session_handle,0xFF);
      CRCByte = (unsigned char) rt;
	  CRC16 = TMCRC(1, &CRCByte, CRC16, 1);
   
      /* read a dummy read byte and CRC16 */
      CRCByte = (unsigned char) TMTouchByte(session_handle,0xFF);
	  CRC16 = TMCRC(1, &CRCByte, CRC16, 1);

      CRCByte = (unsigned char) TMTouchByte(session_handle,0xFF);
	  CRC16 = TMCRC(1, &CRCByte, CRC16, 1);

      CRCByte = (unsigned char) TMTouchByte(session_handle,0xFF);
	  CRC16 = TMCRC(1, &CRCByte, CRC16, 1);

   
      /* check result */
      if (CRC16 != 0xB001) 
         rt = -1;
   }
   else
      rt = -1;

   return rt;
}


/*----------------------------------------------------------------------
 * Print the switch information .
 */                                                          
short DisplaySwitchInfo(short ClearActivity, short far *channels)
{    
   short printmsg=1,flag,rt=0;
   long session_handle;                         

   // loop to get a session handle and to a temperature conversion */
   for (;;)
   {
      /* attempt to get a session */
      session_handle = TMExtendedStartSession(PortNum,PortType,NULL);
      if (session_handle > 0)  
      {
         flag = ReadSwitchInfo(session_handle,ClearActivity);
         if (flag >= 0)
         {
            PrintSwitchInfo(flag,channels);
            rt = 1;
         }
                                
         /* end the session if one is open */
         TMEndSession(session_handle);
         MessageBeep(MB_ICONEXCLAMATION);
         break;
      }
      else if (session_handle == 0)
      {                            
         /* check if need to print the waiting message */
         if (printmsg)
         {  
            printmsg = FALSE;
            printf("\nWaiting to get access to the 1-Wire network\n\n");
         }
      }
   }

   return rt;
}


/*----------------------------------------------------------------------
 * Print the switch information .
 */                                                          
void PrintSwitchInfo(short info, short far *channels)
{          
   short i;
   
   /* check number of channels on switch */
   if (info & 0x40)
   {                                       
      /* two */
      printf("     Channel              A           B\n");
      printf("     ------------------------------------\n");   
      *channels = 2;
   }
   else
   {                                       
      /* one */
      printf("     Channel              A  \n");
      printf("     ------------------------\n"); 
      *channels = 1;
   }
   
   /* flip flop */
   printf("     Flip Flop       ");
   for (i = 0; i < *channels; i++)  
   {
      if (info & (0x01 << i))
         printf("    OFF     ");
      else              
         printf("    ON      ");
   }

   /* sensed level */
   printf("\n     Sensed Level    ");
   for (i = 0; i < *channels; i++)  
   {
      if (info & (0x04 << i))
         printf("    HIGH    ");
      else      
         printf("    LOW     ");
   }

   /* sensed level */
   printf("\n     Activity Latch  ");
   for (i = 0; i < *channels; i++)  
   {
      if (info & (0x10 << i))
         printf("    SET     ");
      else      
         printf("    CLEAR   ");
   }
}


/*----------------------------------------------------------------------
 * Prompt for the state to put the flip flop(s) in
 */                                                          
short WhichFlipFlop(short channels)
{
   short rt = 0,i;
   
   printf("\n");                                                
   for (i = 0; i < channels; i++)                         
   {                       
      printf("Channel %c Flip Flop (1 set, 0 clear):",'A' + i);
      if (getche() == '0')
         rt |= (0x01 << i);      
      printf("\n\n");
   }

   return (rt << 5) | 0x1F;
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


