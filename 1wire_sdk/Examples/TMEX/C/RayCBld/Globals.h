/*---------------------------------------------------------------------
 * Copyright (C) 1992-2002 Dallas Semiconductor/MAXIM Corporation.
 * All rights Reserved. Printed in U.S.A.  This software is 
 * protected by copyright laws of the United States and of foreign
 * countries.  This material may also be protected by patent laws 
 * of the United States and of foreign countries.  This software is 
 * furnished under a license agreement and/or a nondisclosure 
 * agreement and may only be used or copied in accordance with the 
 * terms of those agreements.  The mere transfer of this software 
 * does not imply any licenses of trade secrets, proprietary 
 * technology, copyrights, patents, trademarks, maskwork rights, 
 * or any other form of intellectual property whatsoever. Dallas 
 * Semiconductor retains all ownership rights.
 *---------------------------------------------------------------------
 */

/* Global Variables for TMEX routines and to hold list information*/

const int MaxDbnc=1;
#ifdef TMEX
    short PortNum, PortType, SetupDone; // Initialized once when program is opened
    long SHandle; // Global for the session handles excluding the timer
    short ROM[8]; // Global that holds the current rom number
    unsigned char state_buffer[15360];
    AnsiString devicestr, pathstr, tstr, OriginalText; // Globals for form captions
    DirectoryPath Path; // Global that holds current path info
    FileEntry* SFile; // Used when selecting an item(file or dir) from the file list
#else
    extern short PortNum, PortType, SetupDone;
    extern long SHandle;
    extern short ROM[8];
    extern unsigned char state_buffer[15360];
    extern AnsiString devicestr, pathstr, tstr, OriginalText;
    extern DirectoryPath Path;
    extern FileEntry* SFile;
#endif

