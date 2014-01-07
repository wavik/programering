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
 * property whatsoever. Dallas Semiconductor retains all ownership rights.
 *--------------------------------------------------------------------------
 *
 * RCONVC32.C - This utility uses TMEX to view a list of devices on the
 *              1-wire, view a list of files on a device  and permit the
 *              files to be edited. It requires the TMEX
 *              drivers to be present.
 *
 * Compiler:  Microsoft Visual C 6.0
 * Externals: iBTMEXCW.H
 * Version: 4.00
 *
 */

#define TMEXUTIL

#include "iBTMEXCW.H"
#include <stdio.h> 
#include <conio.h> 

/* Local Function Prototypes */
short GetROMList(void);
short GetFILEList(short);
void editFILE(short);
void ProgramDevice(short, short);  
long WaitForSession(short, short);
void Exit_Prog(char *, short);

/* Global Variables to hold list information */
uchar RomList[10][9];
FileEntry FileList[10];
short PortType, PortNum;
uchar state_buffer[15360];
long SHandle;

/*----------------------------------------------------------------------
 *  This is the Main routine for RCONVC32.
 */
void main(short argc, char **argv)
{
   short flag,State=0,printmsg=1,listnum=0,currentrom=0,i;
   char ch,tstr[80];       
   DirectoryPath path;

   /* check arguments to see if request instruction with '?' or too many */
   if ((argc > 2) && (argv[1][0] == '?' || argv[1][1] == '?'))
   {
       printf("\nusage: rconvc32 <PortNum> \n"
              "  - view and replace files on multi-drop iButtons\n"
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
   printf("\nRCONVC32 Version 4.00\n"
         "Dallas Semiconductor Corporation\n"
         "Copyright (C) 1992-2002\n\n");
   printf("Port number: %d    Port type: %d\n",PortNum,PortType);
   Get_Version(tstr);
   printf("Main Driver: %s\n",tstr);
   printf("Type%d:",PortType);
   if (TMGetTypeVersion(PortType,tstr) < 0)
      Exit_Prog("No Hardware Driver for this type found!",NO_DRIVERS);
   printf(" %s\n\n\n",tstr);

   /* run setup on the indicated PortNum to see if it is valid */
   /* wait for a session on 1-Wire network */
   SHandle = WaitForSession(PortNum, PortType);
   flag = TMSetup(SHandle);
   TMEndSession(SHandle);
   if (flag != 1 && flag != 2)
     Exit_Prog("ERROR, Indicated PortNum does not exist!\n",
               -XONE_WIRE_PORT_ERROR);

   /* provide warning if shorted */
   if (flag == 2)
     printf("WARNING, The OneWire is shorted on setup.\n");

   /* loop to look for key strokes */
   for (;;)
   {
      /* check for message printing */
      if (printmsg)
      {
         if (State == 1)
            printf("\n>>> Press a number to view the file "
                   "list of a DEVICE.\n");
         else if (State == 2)
            printf("\n>>> Press a number to choose a FILE to "
                   "view or replace.\n");
         printf(">>> Press \'L\' to LIST current devices on 1-Wire."
                " (ESCAPE to quit)\n");
         printmsg = FALSE;
      }

      /* get a key */     
      ch = getch();
      if (kbhit())
         ch = getch();

      /* decide what to do with key */
      if (ch == 'l' || ch == 'L')
      {
         /* look for a list of devices on 1-wire */
         listnum = GetROMList();
         if (!listnum)
         {
              printf("\n\n>>> No devices found on 1-Wire.\n");
              Beep(300,600);
         }
         else
              State = 1;    
         printmsg = TRUE;
      }
      else if (State > 0 && ch >= '0' && ch < ('0'+listnum))
      {
         /* a valid choice in a rom list or file list */
         if (State == 1)
         {
            /* look for a list of files */
            listnum = GetFILEList((short)(ch - '0'));
            if (!listnum)
            {
                printf("\n\n>>> No file list.\n");
                Beep(300,600);
                State = 0;
            }
            else
                State = 2;
            currentrom = ch - '0';     
            printmsg = TRUE;
         }
         else
         {      
            /* check to see if the file picked is a sub-directory */
            if (FileList[ch - '0'].extension == 0x7F)
            {
               /* set the new directory and re-read file list */
               path.NumEntries = 1;
               path.Ref = '.';                  
               for (i = 0; i < 4; i++)
                  path.Entries[0][i] = FileList[ch-'0'].name[i];
               /* wait for a session on 1-Wire network */
               SHandle = WaitForSession(PortNum, PortType);
               TMChangeDirectory(SHandle,state_buffer,DIR_SET,&path);   
               TMEndSession(SHandle);
               listnum = GetFILEList(currentrom);
               if (!listnum)
               {
                   printf("\n\n>>> No file list.\n");
                   Beep(300,600);
                   State = 0;
               }
               else
                   State = 2;
               printmsg = TRUE;
            }
            else
            {
               /* edit the file of choice */
               editFILE((short)(ch - '0'));
               State = 0;
               printmsg = TRUE;
            }
         }
      }
      else if (ch == 0x1B)  /* escape to end */
         break;
      else
         Beep(300,600);  /* not a valid key */
   }

   /* close the 1-Wire and exit the program (1.10) */
   Exit_Prog("",NORMAL_EXIT);
}


/*----------------------------------------------------------------------
 * Get a list of devices on the 1-Wire network and display them.
 */
short GetROMList(void)
{
   short i,j,cnt=0,flag;
   short ROM[9];

   /* wait for a session on 1-Wire network */
   SHandle = WaitForSession(PortNum, PortType);

   /* get first device in list */
   flag = TMFirst(SHandle,state_buffer);
   /* loop while still have devices in list */
   while (flag == 1)
   {
      /* record the rom number in the list */
      ROM[0] = 0;
      TMRom(SHandle,state_buffer,ROM);
      for (i = 0; i < 8; i++)
         RomList[cnt][i] = (uchar)ROM[i];

      /* increment number of devices found */
      if (++cnt >= 10) 
         break;

      /* get next device in list */
      flag = TMNext(SHandle,state_buffer);
   }

   /* release the session */
   TMEndSession(SHandle);

   /* loop to print the list */
   printf("\n");
   for (i = 0; i < cnt; i++)  
   {
      printf("     %d)  ",i);
      for (j = 7; j >= 0; j--)
         printf("%02X",RomList[i][j]);
      printf("\n");
   }

   return cnt;
}


/*----------------------------------------------------------------------
 * Get a list of the files on the 'num' device in the ROM list.
 */
short GetFILEList(short num)
{
   short i,j,cnt=0,flag;
   short ROM[9];
   FileEntry Flname;
   DirectoryPath path;

   /* wait for a session on 1-Wire network */
   SHandle = WaitForSession(PortNum, PortType);

   /* set the rom to the one selected */
   for (i = 0; i < 8; i++)
      ROM[i] = RomList[num][i];     
   TMRom(SHandle,state_buffer,ROM);
   
   /* get the current directory */
   flag = TMChangeDirectory(SHandle,state_buffer,DIR_READ,&path); 
   if (flag < 0)
     return 0;

   /* get first file in list */
   flag = TMFirstFile(SHandle,state_buffer,&Flname);
   /* loop while still have files in list */
   while (flag >= 1)
   {
       /* check to see if it is not hidden directory */
       if (!(Flname.extension == 0x7F && Flname.attrib == 0x02))
       {
            /* record the file name in the list */
            FileList[cnt] = Flname;
         
            /* increment number of files found */
            if (++cnt >= 10) break;
       }

       /* get next file in list */
       flag = TMNextFile(SHandle,state_buffer,&Flname);
   }

   /* release the session */
   TMEndSession(SHandle);

   /* print the directory */  
   if (cnt)
   {                
       printf("\nDirectory of %d:\\",PortNum);
       for (i = 0; i < path.NumEntries; i++)  
       {      
           if (i != 0)
              printf("\\");
           for (j = 0; j < 4; j++)
           {
              if (path.Entries[i][j] == ' ')
                 break;
              printf("%c",path.Entries[i][j]);         
           }   
       }
       printf("\n\n");
   }

   /* loop to print the list */
   for (i = 0; i < cnt; i++)    
   {
      /* check to see if it is a sub-directory */
      if (FileList[i].extension == 0x7F)
         printf("     %d)  %.4s    <DIR>\n",i,FileList[i].name);
      else
         printf("     %d)  %.4s     %03d\n",i,FileList[i].name,
                 FileList[i].extension);
   }

   return cnt;
}


/*----------------------------------------------------------------------
 * View and replace the contents of the file chosen.
 */
void editFILE(short num)
{
   short epjob,hndl,i,len,maxlen,flag;
   uchar buf[7140];
   char ch;

   /* wait for a session on 1-Wire network */
   SHandle = WaitForSession(PortNum, PortType);

   /* read the file */
   hndl = TMOpenFile(SHandle,state_buffer,&FileList[num]);
   len = TMReadFile(SHandle,state_buffer,hndl,buf,7140);
   TMCloseFile(SHandle,state_buffer,hndl);

   /* release the session */
   TMEndSession(SHandle);

   /* check results and print */
   if ((hndl >= 0) && (len >= 0))
   {
       buf[len] = 0; /* zero terminate the string */
       printf("\n>>> File contents:\n%s\n>>> End of file\n",(char *)buf);
   }
   else
   {
      printf("\n\n>>> Error reading file.\n");
      Beep(300,600);
      return;
   }

   /* prompt to see if want to replace the file */
   printf("\n>>> Replace the file? (Y or N) ");
   ch = getch();
   if (ch != 'Y' && ch != 'y')
   {
       printf("\n\n\n");
       return;
   }

   /* prompt for the new file */
   printf("Y\n\n>>> Type in the new file, press 'END' when done. (ESCAPE to quit)\n");

   /* loop to get the file */
   len = 0;
   for (;;)
   {
       /* get a key */
       ch = getch();
       if (kbhit())
       {
            ch = getch();
            /* check for 'END' */
            if (ch == 'O')
               break;
       }

       /* check the key */
       if (ch == 0x08)
       {
            if (len > 0)
            {
                 /* got a valid backspace */
                 len--;
                 printf("\x008 \x008");
            }
            else
               Beep(300,600);
       }
       else if (ch == 0x1B)
       {
            /* escape key */
            Exit_Prog("",NORMAL_EXIT);
       }
       else
       {
            /* valid key */
            /* echo the char */
            if (ch == '\r')  /* special case for cr */
            {
               printf("\n");
               buf[len++] = ch;
               ch = 0x0A;
            }
            else
               printf("%c",ch);
            /* record the char */
            buf[len++] = ch;
       }
   }

   /* wait for a session on 1-Wire network */
   SHandle = WaitForSession(PortNum, PortType);

   /* attempt to create an program job on the chance this needed */
   epjob = TMCreateProgramJob(SHandle,state_buffer);

   /* replace the file */
   /* loop up to 5 times to attempt to replace the file */
   for (i = 0; i < 5; i++)
   {                                   
      flag = TMDeleteFile(SHandle,state_buffer,&FileList[num]);
      if (flag == 1 || flag == XFILE_NOT_FOUND)
      {
         /* successful delete or file already deleted */
         hndl = TMCreateFile(SHandle,state_buffer,&maxlen,&FileList[num]);

         /* check if file handle was valid */
         if (hndl < 0)
         {
              printf("\n\n>>> Error creating file! (%d)\n",hndl);
              Beep(300,600);
              /* release the session */
              TMEndSession(SHandle);
              return;
         }

         /* check if not enough room */
         if (maxlen < len)
         {
              TMCloseFile(SHandle,state_buffer,hndl);
              printf("\n\n>>> Error, not enough room for file!\n");
              Beep(300,600);
              /* release the session */
              TMEndSession(SHandle);
              return;
         }

         /* write the file */
         flag = TMWriteFile(SHandle,state_buffer,hndl,buf,len);
         TMCloseFile(SHandle,state_buffer,hndl);

         /* check results */
         if (flag == len)  
            break;        /* success */
      }
   }

   /* release the session */
   TMEndSession(SHandle);

   /* check to see if successfull or 5 attempts failed */ 
   if (i == 5)
   {
      /* must have failed 5 times */
      printf("\n\n>>> Error writing file!\n");
      Beep(300,600);
   }
   else
   {
      /* do the program job if it was started */
      if (epjob == TRUE)
         ProgramDevice(PortNum,PortType);
      
      Beep(800,200);
      printf("\n\n\n");
   }
}


/*----------------------------------------------------------------------
 * Call the function TMDoProgramJob until it is successful or it is 
 * aborted by the user.
 */                       
void ProgramDevice(short PortNum, short PortType)  
{          
   short flag;
   char ch;
   
   /* wait for a session on 1-Wire network */
   SHandle = WaitForSession(PortNum, PortType);

   /* loop to write program device */
   for (;;)
   {
      /* attempt to write */
      flag = TMDoProgramJob(SHandle,state_buffer);
      
      /* check result */
      if (flag == TRUE)
         break;          /* success */
      
      /* an error has occurred */
      Beep(300,600);
      printf("\n"); 
      printf("\n");
      if (flag == XNO_DEVICE)
         printf("Device not present!\n");   
      else if (flag == XNON_PROGRAM_PARTS)
         printf("WARNING, Non-program parts are on the 1-wire.\n");
      else if (flag == XFILE_READ_ERR)
         printf("ERROR reading Touch Memory possibly due to poor"
                " contact\n");
      else if (flag == XPROGRAM_WRITE_PROTECT) 
         printf("ERROR, device is programmed in a way that it can not"
                " be updated\n");
      else if (flag == XOUT_OF_SPACE)
         printf("ERROR, device is out of space!\n");
      else if (flag == XFUNC_NOT_SUP)
         printf("ERROR, program function is not supported by"
                " software or hardware!\n");
                
      /* request an abort or retry */
      printf("\n");
      printf("Press escape to ABORT operation or ANY other key"
             " to RETRY.\n");
      printf("WARNING, aborting a device program may leave the"
             " device unusable!\n");
      ch = getch();
      if (ch == 0x1B)  /* check for escape */ 
      {  
         /* reset the current program job */
         TMCreateProgramJob(SHandle,state_buffer);
         /* release the session */
         TMEndSession(SHandle);
         Exit_Prog("Aborting TMEX utility\n",KEY_ABORT);
      }
   }

   /* release the session */
   TMEndSession(SHandle);
}                     


/*----------------------------------------------------------------------
 *  Wait for a session on the 1-Wire network to become available (up to 10s)
 */
long WaitForSession(short portnum, short porttype)
{
   long session_handle;
   unsigned long stmp;

   stmp = GetTickCount() + 10000;

   /* loop to get a session */
   do
   {
      /* get a session to the port */
      session_handle = TMExtendedStartSession(portnum,porttype,NULL);
      if (session_handle == 0)
         Sleep(1);
      else if (session_handle < 0)
         Exit_Prog("No Hardware Driver for this type found!",NO_DRIVERS);
   }
   while ((session_handle == 0) && (GetTickCount() < stmp));

   if (session_handle == 0)
      Exit_Prog("ERROR, Could not get a sesson on the 1-Wire network!\n",
               -XONE_WIRE_PORT_ERROR);

   return session_handle;
}

/*--------------------------------------------------------------------------
 *  Prints a message, beeps and exits the program.
 */
void Exit_Prog(char *msg, short errcd)
{
     short i;

     /*  check to see if any characters to read */
     for (i = 0; i < 2; i++)
         if (kbhit()) getch();

     /* print the message left justified */
     printf("\r%s",msg);

     /*  error beep if return code is not 0 */
     if (errcd != NORMAL_EXIT)
        Beep(300,600);

     /*  return error code passed */
     exit(errcd);
}
