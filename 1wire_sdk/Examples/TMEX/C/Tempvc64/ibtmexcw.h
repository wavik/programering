/*---------------------------------------------------------------------------
* Copyright © 1992-2008 Maxim Integrated Products, All Rights Reserved.
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
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
* IN NO EVENT SHALL MAXIM INTEGRATED PRODUCTS BE LIABLE FOR ANY CLAIM, DAMAGES
* OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
* ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
* OTHER DEALINGS IN THE SOFTWARE.
*
* Except as contained in this notice, the name of Maxim Integrated Products
* shall not be used except as stated in the Maxim Integrated Products
* Branding Policy.
*---------------------------------------------------------------------------
* Version 4.01
*/

/* includes */
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <windows.h>
#ifndef _WIN32_WCE
#include <stdio.h>
#include <conio.h>
#include <dos.h>
#include <fcntl.h>
#include <io.h>
#include <time.h>
#endif

/* type defs */
typedef unsigned char uchar;
typedef unsigned short ushort;
typedef unsigned long ulong;

/* typedef structure sent back by TMEX routines */
typedef struct 
{
uchar name[4];
uchar extension;
uchar startpage;
uchar numpages;
uchar attrib;
uchar bitmap[32];
} FileEntry;

/* structure to hold directory path */
typedef struct
{
uchar NumEntries; /* number of entries in path 0-10 */
char Ref; /* reference character '\' or '.' */
char Entries[10][4]; /* sub-directory entry names */ 
} DirectoryPath;

/* Holds info of each object in the Directory list */
struct DirNumInfo 
{ 
unsigned char Name[4];
unsigned char Extension;
char Attrib;
};

/* structure to hold exportable device parameters by family code (3.11) */
typedef struct
{
short features[32];
char dscrptn[255];
} Specification;


/* Error codes from TMEX functions */
#define NO_DEVICE -1 
#define WRONG_TYPE -2 
#define FILE_READ_ERR -3 
#define BUFFER_TOO_SMALL -4 
#define HANDLE_NOT_AVAIL -5 
#define FILE_NOT_FOUND -6 
#define REPEAT_FILE -7 
#define HANDLE_NOT_USED -8 
#define FILE_WRITE_ONLY -9 
#define OUT_OF_SPACE -10 
#define FILE_WRITE_ERR -11 
#define TMFILE_READ_ONLY -12 
#define FUNC_NOT_SUP -13 
#define BAD_FILENAME -14 
#define CANT_DEL_READ_ONLY -15 
#define HANDLE_NOT_EXIST -16 
#define ONE_WIRE_PORT_ERROR -17 
#define INVALID_DIRECTORY -18 
#define DIRECTORY_NOT_EMPTY -19 
#define UNABLE_TO_CREATE_DIR -20 
#define NO_PROGRAM_JOB -21 
#define PROGRAM_WRITE_PROTECT -22
#define NON_PROGRAM_PARTS -23 
#define ADDFILE_TERMINATED -24 
#define TIMEOUT -25 
#define INVALID_ARGUMENT -26 
#define BAD_ACK -27 
#define INVALID_SESSION -200
#define NO_BASDRV_FOUND -201

/* Basic physical level error codes */
#define BPORT_NOT_INITIALIZED -1 
#define BPORT_NOT_EXIST -2 
#define BNO_SUCH_FUNCTION -3 

/* Transport level error codes */
#define TERROR_READ_WRITE -4 
#define TBUFFER_TOO_SMALL -5 
#define TDEVICE_TOO_SMALL -6 
#define TNO_DEVICE -7 
#define TBLOCK_TOO_BIG -8 
#define TWRONG_TYPE -9 
#define TPAGE_REDIRECTED -10
#define TPROGRAM_NOT_POSSIBLE -11
#define BCOM_FAILURE -12
#define BCOM_EVENT -13

/* for TMOneWireLevel */
#define LEVEL_NORMAL 0
#define LEVEL_STRONG_PULLUP 1
#define LEVEL_BREAK 2
#define LEVEL_PROGRAM 3 
#define PRIMED_NONE 0
#define PRIMED_BIT 1
#define PRIMED_BYTE 2 
#define LEVEL_READ 1
#define LEVEL_SET 0

/* misc */
#define GENERAL_FAIL -2 
#define BAD_ARGUMENT 50
#define NO_DRIVERS 51
#define KEY_ABORT 52
#define OUT_MEMORY 53
#define NORMAL_EXIT 0

#define TRUE 1
#define FALSE 0
#define DIR_READ 1
#define DIR_SET 0
#define DIR_REMOVE 1
#define DIR_MAKE 0

/* for specification */
#define NOMEM 0
#define NVRAM 1
#define EPROM1 2
#define EPROM2 3
#define EPROM3 4
#define EEPROM1 5
#define MNVRAM 6
#define EEPROM2 7
#define NVRAM2 8
#define NVRAM3 9 
#define FTR_REG_PAGES 0
#define FTR_REG_LEN 1
#define FTR_STAT_PAGES 2
#define FTR_STAT_LEN 3
#define FTR_MAX_COM 4 
#define FTR_MEM_TYPE 5

// session
extern long __fastcall TMExtendedStartSession(short, short, void far *);
extern short __fastcall TMValidSession(long); 
extern short __fastcall TMEndSession(long); 
extern short __fastcall Get_Version(char far *); 
// file_operations
extern short __fastcall TMFirstFile(long, void far *, FileEntry far *); 
extern short __fastcall TMNextFile(long, void far *, FileEntry far *);
extern short __fastcall TMOpenFile(long, void far *, FileEntry far *);
extern short __fastcall TMCreateFile(long, void far *, short far *, FileEntry far *);
extern short __fastcall TMCloseFile(long, void far *, short);
extern short __fastcall TMReadFile(long, void far *, short, uchar far *, short);
extern short __fastcall TMWriteFile(long, void far *, short, uchar far *, short);
extern short __fastcall TMDeleteFile(long, void far *, FileEntry far *);
extern short __fastcall TMFormat(long, void far *);
extern short __fastcall TMAttribute(long, void far *, short, FileEntry far *);
extern short __fastcall TMReNameFile(long, void far *, short, FileEntry far *); 
extern short __fastcall TMChangeDirectory(long, void far *, short, DirectoryPath far *); 
extern short __fastcall TMDirectoryMR(long, void far *, short, FileEntry far *);
extern short __fastcall TMCreateProgramJob(long, void far *);
extern short __fastcall TMDoProgramJob(long, void far *);
extern short __fastcall TMWriteAddFile(long, void far *, short, short, short, uchar far *, short);
extern short __fastcall TMTerminateAddFile(long, void far *, FileEntry far *);
extern short __fastcall TMGetFamilySpec(long, void far *, Specification *); 
// transport
extern short __fastcall TMReadPacket(long, void far *, short, uchar far *, short);
extern short __fastcall TMWritePacket(long, void far *, short, uchar far *, short);
extern short __fastcall TMBlockIO(long, uchar far *, short);
extern short __fastcall TMExtendedReadPage(long, void far *, short, uchar far *, short);
extern short __fastcall TMProgramByte(long, void far *, short, short, short, short far *, short);
extern short __fastcall TMProgramBlock(long, void far *, uchar far *, short, short, short far *); /* (3.11) */
extern long __fastcall TMCRC(short, uchar far *, ushort, short); 
// network
extern short __fastcall TMSkipFamily(long, void far *); 
extern short __fastcall TMFamilySearchSetup(long, void far *, short); 
extern short __fastcall TMFirst(long, void far *);
extern short __fastcall TMNext(long, void far *);
extern short __fastcall TMAccess(long, void far *);
extern short __fastcall TMOverAccess(long, void far *);
extern short __fastcall TMStrongAccess(long, void far *);
extern short __fastcall TMStrongAlarmAccess(long, void far *);
extern short __fastcall TMRom(long, void far *, short far *);
extern short __fastcall TMFirstAlarm(long, void far *);
extern short __fastcall TMNextAlarm(long, void far *); 
extern short __fastcall TMAutoOverDrive(long, void far *, short); 
extern short __fastcall TMSearch(short, short, short, short); 
// hardware_specific
extern short __fastcall TMSetup(long);
extern short __fastcall TMTouchByte(long, short);
extern short __fastcall TMTouchReset(long);
extern short __fastcall TMTouchBit(long, short);
extern short __fastcall TMClose(long);
extern short __fastcall TMProgramPulse(long); 
extern short __fastcall TMOneWireCom(long, short, short); 
extern short __fastcall TMOneWireLevel(long, short, short, short); 
extern short __fastcall TMGetTypeVersion(short,char far *); 
extern short __fastcall TMBlockStream(long, uchar far *, short);
extern short __fastcall TMGetAdapterSpec(long, Specification far *); 
extern short __fastcall TMReadDefaultPort(short far *, short far *); 

