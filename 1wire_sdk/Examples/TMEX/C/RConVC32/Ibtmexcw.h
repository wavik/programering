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
 * TMEX API header file - IBTMEXCW.H
 */

/* includes */
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <windows.h>

/* type defs */
typedef unsigned char  uchar;
typedef unsigned short ushort;
typedef unsigned long  ulong;

/* typedef structures sent back by TMEX routines */

/* FileEntry holds TMEX file info */
typedef struct      
{
   uchar name[4];
   uchar extension;
   uchar startpage;
   uchar numpages;
   uchar attrib;
   uchar bitmap[32];
} FileEntry;

/* DirectoryPath holds information on TMEX file directories */
typedef struct
{
   uchar NumEntries;      /* number of entries in path 0-10 */
   char  Ref;             /* reference character '\' or '.' */
   char  Entries[10][4];  /* sub-directory entry names */                                          
} DirectoryPath;

/* Specification holds information on 1-Wire adapters or 1-Wire devices */
typedef struct 
{
   unsigned short features[32];
   char description[255];
} Specification;

struct DirNumInfo // Holds info of each object in the Directory list
{  
   unsigned char Name[4];
   unsigned char Extension;
   char Attrib;
};

/* Error codes from TMEX functions */
#define    XNO_DEVICE             -1 
#define    XWRONG_TYPE            -2 
#define    XFILE_READ_ERR         -3 
#define    XBUFFER_TOO_SMALL      -4 
#define    XHANDLE_NOT_AVAIL      -5  
#define    XFILE_NOT_FOUND        -6 
#define    XREPEAT_FILE           -7 
#define    XHANDLE_NOT_USED       -8 
#define    XFILE_WRITE_ONLY       -9 
#define    XOUT_OF_SPACE          -10 
#define    XFILE_WRITE_ERR        -11 
#define    XFILE_READ_ONLY        -12 
#define    XFUNC_NOT_SUP          -13 
#define    XBAD_FILENAME          -14 
#define    XCANT_DEL_READ_ONLY    -15 
#define    XHANDLE_NOT_EXIST      -16 
#define    XONE_WIRE_PORT_ERROR   -17 
#define    XINVALID_DIRECTORY     -18 
#define    XDIRECTORY_NOT_EMPTY   -19 
#define    XUNABLE_TO_CREATE_DIR  -20 
#define    XNO_PROGRAM_JOB        -21 
#define    XPROGRAM_WRITE_PROTECT -22
#define    XNON_PROGRAM_PARTS     -23 
#define    XADDFILE_TERMINATED    -24 
#define    XTIMEOUT               -25 
#define    XINVALID_ARGUMENT      -26 
#define    XBAD_ACK               -27 
#define    XINVALID_SESSION       -200
#define    XNO_BASDRV_FOUND       -201

/* Basic physical level error codes  */
#define    BPORT_NOT_INITIALIZED   -1 
#define    BPORT_NOT_EXIST         -2 
#define    BNO_SUCH_FUNCTION       -3 

/* Transport level error codes  */
#define    TERROR_READ_WRITE       -4 
#define    TBUFFER_TOO_SMALL       -5 
#define    TDEVICE_TOO_SMALL       -6 
#define    TNO_DEVICE              -7 
#define    TBLOCK_TOO_BIG          -8 
#define    TWRONG_TYPE             -9 
#define    TPAGE_REDIRECTED        -10
#define    TPROGRAM_NOT_POSSIBLE   -11
#define    BCOM_FAILURE            -12
#define    BCOM_EVENT              -13

/* other  */
#define    GENERAL_FAIL            -2 
#define    BAD_ARGUMENT            50
#define    NO_DRIVERS              51
#define    KEY_ABORT               52
#define    OUT_MEMORY              53
#define    NORMAL_EXIT             0

#define    TRUE                    1
#define    FALSE                   0
#define    DIR_READ                1
#define    DIR_SET                 0
#define    DIR_REMOVE              1
#define    DIR_MAKE                0

// for TMOneWireLevel
#define    LEVEL_NORMAL            0
#define    LEVEL_STRONG_PULLUP     1
#define    LEVEL_BREAK             2
#define    LEVEL_PROGRAM           3  
#define    PRIMED_NONE             0
#define    PRIMED_BIT              1
#define    PRIMED_BYTE             2        
#define    LEVEL_READ              1
#define    LEVEL_SET               0


#ifdef __cplusplus
   extern "C" {

// Session Layer
__declspec(dllimport) long pascal  TMExtendedStartSession(short, short, void *);
__declspec(dllimport) long pascal  TMStartSession(short);
__declspec(dllimport) short pascal TMValidSession(long);
__declspec(dllimport) short pascal TMEndSession(long);

// File Operations Layer
__declspec(dllimport) short pascal TMFirstFile(long, void *, FileEntry *);
__declspec(dllimport) short pascal TMNextFile(long, void *, FileEntry *);
__declspec(dllimport) short pascal TMOpenFile(long, void *, FileEntry *);
__declspec(dllimport) short pascal TMCreateFile(long, void *, short *, FileEntry *);
__declspec(dllimport) short pascal TMCloseFile(long, void *, short);
__declspec(dllimport) short pascal TMReadFile(long, void *, short, unsigned char*, short);
__declspec(dllimport) short pascal TMWriteFile(long, void *, short, unsigned char *, short);
__declspec(dllimport) short pascal TMDeleteFile(long, void *, FileEntry *);
__declspec(dllimport) short pascal TMFormat(long, void *);
__declspec(dllimport) short pascal TMAttribute(long, void *, short, FileEntry *);
__declspec(dllimport) short pascal TMReNameFile(long, void *, short, FileEntry *);
__declspec(dllimport) short pascal TMChangeDirectory(long, void *, short, DirectoryPath *);
__declspec(dllimport) short pascal TMDirectoryMR(long, void *, short, FileEntry *);
__declspec(dllimport) short pascal TMCreateProgramJob(long, void *);
__declspec(dllimport) short pascal TMDoProgramJob(long, void *);
__declspec(dllimport) short pascal TMWriteAddFile(long, void *, short, short, short, unsigned char *, short);
__declspec(dllimport) short pascal TMTerminateAddFile(long, void *, FileEntry *);
__declspec(dllimport) short pascal TMGetFamilySpec(long, void *, Specification *);
__declspec(dllimport) short pascal Get_Version(char *);
   
// Transport Layer
__declspec(dllimport) short pascal TMReadPacket(long, void *, short, unsigned char *, short);
__declspec(dllimport) short pascal TMWritePacket(long, void *, short, unsigned char *, short);
__declspec(dllimport) short pascal TMBlockIO(long, unsigned char far *, short);
__declspec(dllimport) short pascal TMExtendedReadPage(long, void *, short, unsigned char *, short);
__declspec(dllimport) short pascal TMProgramByte(long, void *, short, short, short, short *, short);
__declspec(dllimport) short pascal TMProgramBlock(long, void *, unsigned char *, short, short, short *);
__declspec(dllimport) short pascal TMCRC(short, unsigned char *, unsigned short, short);

// Network Layer
__declspec(dllimport) short pascal TMFirst(long, void *);
__declspec(dllimport) short pascal TMNext(long, void *);
__declspec(dllimport) short pascal TMAccess(long, void *);
__declspec(dllimport) short pascal TMStrongAccess(long, void *);
__declspec(dllimport) short pascal TMStrongAlarmAccess(long, void *);
__declspec(dllimport) short pascal TMOverAccess(long, void *);
__declspec(dllimport) short pascal TMRom(long, void *, short *);
__declspec(dllimport) short pascal TMFirstAlarm(long, void *);
__declspec(dllimport) short pascal TMNextAlarm(long, void *);
__declspec(dllimport) short pascal TMFamilySearchSetup(long, void *, short);
__declspec(dllimport) short pascal TMSkipFamily(long, void *);
__declspec(dllimport) short pascal TMAutoOverDrive(long, void *, short);
__declspec(dllimport) short pascal TMSearch(long, void *, short, short, short);


// Hardware Specific Layer
__declspec(dllimport) short pascal TMSetup(long);
__declspec(dllimport) short pascal TMTouchByte(long, short);
__declspec(dllimport) short pascal TMTouchReset(long);
__declspec(dllimport) short pascal TMTouchBit(long, short);
__declspec(dllimport) short pascal TMProgramPulse(long);
__declspec(dllimport) short pascal TMOneWireLevel(long, short, short, short);
__declspec(dllimport) short pascal TMOneWireCom(long, short, short);
__declspec(dllimport) short pascal TMClose(long);
__declspec(dllimport) short pascal TMGetTypeVersion(short, char *);
__declspec(dllimport) short pascal TMBlockStream(long, unsigned char far *, short);
__declspec(dllimport) short pascal TMReadDefaultPort(short *, short *);
__declspec(dllimport) short pascal TMGetAdapterSpec(long, void *, Specification *);

}
#else

// Session Layer
extern long  far pascal TMExtendedStartSession(short, short, void far *);
extern long  far pascal TMStartSession(short);
extern short far pascal TMValidSession(long); 
extern short far pascal TMEndSession(long); 

// File Operations Layer
extern short far pascal TMFirstFile(long, void far *, FileEntry far *); 
extern short far pascal TMNextFile(long, void far *, FileEntry far *);
extern short far pascal TMOpenFile(long, void far *, FileEntry far *);
extern short far pascal TMCreateFile(long, void far *, short far *, FileEntry far *);
extern short far pascal TMCloseFile(long, void far *, short);
extern short far pascal TMReadFile(long, void far *, short, uchar far *, short);
extern short far pascal TMWriteFile(long, void far *, short, uchar far *, short);
extern short far pascal TMDeleteFile(long, void far *, FileEntry far *);
extern short far pascal TMFormat(long, void far *);
extern short far pascal TMAttribute(long, void far *, short, FileEntry far *);
extern short far pascal TMReNameFile(long, void far *, short, FileEntry far *); 
extern short far pascal TMChangeDirectory(long, void far *, short, DirectoryPath far *); 
extern short far pascal TMDirectoryMR(long, void far *, short, FileEntry far *);
extern short far pascal TMCreateProgramJob(long, void far *);
extern short far pascal TMDoProgramJob(long, void far *);
extern short far pascal TMWriteAddFile(long, void far *, short, short, short, uchar far *, short);
extern short far pascal TMTerminateAddFile(long, void far *, FileEntry far *);
extern short far pascal TMGetFamilySpec(long, void *, Specification *);
extern short far pascal Get_Version(char far *);

// Transport Layer
extern short far pascal TMReadPacket(long, void far *, short, uchar far *, short);
extern short far pascal TMWritePacket(long, void far *, short, uchar far *, short);
extern short far pascal TMBlockIO(long, uchar far *, short);
extern short far pascal TMExtendedReadPage(long, void far *, short, uchar far *, short);
extern short far pascal TMProgramByte(long, void far *, short, short, short, short far *, short);
extern short far pascal TMProgramBlock(long, void far *, unsigned char far *, short, short, short far *);
extern short far pascal TMCRC(short, unsigned char far *, unsigned short, short);

// Network Layer
extern short far pascal TMSkipFamily(long, void far *); 
extern short far pascal TMFamilySearchSetup(long, void far *, short); 
extern short far pascal TMFirst(long, void far *);
extern short far pascal TMNext(long, void far *);
extern short far pascal TMAccess(long, void far *);
extern short far pascal TMOverAccess(long, void far *);
extern short far pascal TMStrongAccess(long, void far *);
extern short far pascal TMStrongAlarmAccess(long, void far *);
extern short far pascal TMRom(long, void far *, short far *);
extern short far pascal TMFirstAlarm(long, void far *);
extern short far pascal TMNextAlarm(long, void far *);  
extern short far pascal TMAutoOverDrive(long, void far *, short);
extern short far pascal TMSearch(long, void far *, short, short, short);
  
// Hardware Specific Layer
extern short far pascal TMSetup(long);
extern short far pascal TMTouchByte(long, short);
extern short far pascal TMTouchReset(long);
extern short far pascal TMTouchBit(long, short);
extern short far pascal TMProgramPulse(long); 
extern short far pascal TMOneWireLevel(long, short, short, short); 
extern short far pascal TMOneWireCom(long, short, short); 
extern short far pascal TMClose(long);
extern short far pascal TMGetTypeVersion(short,char far *); 
extern short far pascal TMBlockStream(long, uchar far *, short);
extern short far pascal TMReadDefaultPort(short far *, short far *); 
extern short far pascal TMGetAdapterSpec(long, void far *, Specification far *);

#endif

