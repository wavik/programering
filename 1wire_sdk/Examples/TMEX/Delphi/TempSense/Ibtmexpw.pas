{----------------------------------------------------------------------
 | Copyright (C) 1992-2002 Dallas Semiconductor/MAXIM Corporation.
 | All rights Reserved. Printed in U.S.A.  This software is 
 | protected by copyright laws of the United States and of foreign
 | countries.  This material may also be protected by patent laws 
 | of the United States and of foreign countries.  This software is 
 | furnished under a license agreement and/or a nondisclosure 
 | agreement and may only be used or copied in accordance with the 
 | terms of those agreements.  The mere transfer of this software 
 | does not imply any licenses of trade secrets, proprietary 
 | technology, copyrights, patents, trademarks, maskwork rights, 
 | or any other form of intellectual property whatsoever. Dallas 
 | Semiconductor retains all ownership rights.
 |---------------------------------------------------------------------

   The TMEX API functions.
}
Unit iBTMEXPW;

Interface

Type
   FileEntry = record
      name : Array[0..3] of Char;
      extension : Byte;
      startpage : Byte;
      numpages  : Byte;
      attribute : Byte;
      BitMap : Array[0..31] of Byte;
   end;
   PFileEntry = ^FileEntry;
   DirectoryPath = record
      NumEntries : Byte;
      Ref : Char;
      Entries : Array[0..9,0..3] of Char;
   end;
   PDirectoryPath = ^DirectoryPath;
   Specification = record
      features : Array[0..31] of Byte;
      description : Array[0..254] of Char;
   end;
 
Var
   StateBuf : Array[1..15360] of Byte;
   SHandle : Longint;
   CRC16 : Word;
   CRC8 :Byte;
Const
  { TMEX Error codes }
      XNO_DEVICE             = -1 ;
      XWRONG_TYPE            = -2 ;
      XFILE_READ_ERR         = -3 ;
      XBUFFER_TOO_SMALL      = -4 ;
      XHANDLE_NOT_AVAIL      = -5 ;
      XFILE_NOT_FOUND        = -6 ;
      XREPEAT_FILE           = -7 ;
      XHANDLE_NOT_USED       = -8 ;
      XFILE_WRITE_ONLY       = -9 ;
      XOUT_OF_SPACE          = -10 ;
      XFILE_WRITE_ERR        = -11 ;
      XFILE_READ_ONLY        = -12 ;
      XFUNC_NOT_SUP          = -13 ;
      XBAD_FILENAME          = -14 ;
      XCANT_DEL_READ_ONLY    = -15 ;
      XHANDLE_NOT_EXIST      = -16 ;
      XONE_WIRE_PORT_ERROR   = -17 ;
      XINVALID_DIRECTORY     = -18 ;
      XDIRECTORY_NOT_EMPTY   = -19 ;
      XUNABLE_TO_CREATE_DIR  = -20 ;
      XNO_PROGRAM_JOB        = -21 ;
      XPROGRAM_WRITE_PROTECT = -22 ;
      XNON_PROGRAM_PARTS     = -23 ;
      XADDFILE_TERMINATED    = -24 ;
      XTIMEOUT               = -25 ;
      XINVALID_ARGUMENT      = -26 ;
      XBAD_ACK               = -27 ;
      XINVALID_SESSION       = -200;
      XNO_BASDRV_FOUND       = -201;

{ Basic physical level error codes }
      BPORT_NOT_INITIALIZED  = -1 ;
      BPORT_NOT_EXIST        = -2 ;
      BNO_SUCH_FUNCTION      = -3 ;

{ Transport level error codes }
      TERROR_READ_WRITE      = -4 ;
      TBUFFER_TOO_SMALL      = -5 ;
      TDEVICE_TOO_SMALL      = -6 ;
      TNO_DEVICE             = -7 ;
      TBLOCK_TOO_BIG         = -8 ;
      TWRONG_TYPE            = -9 ;
      TPAGE_REDIRECTED       = -10;
      TPROGRAM_NOT_POSSIBLE  = -11;
      BCOM_FAILURE           = -12;
      BCOM_EVENT             = -13;
{ other }
      GENERAL_FAIL           = -2 ;

{Session Layer}
Function TMExtendedStartSession(P: SmallInt; T: SmallInt; Q: Pointer): LongInt; StdCall;
Function TMStartSession(N: SmallInt): SmallInt; StdCall;
Function TMValidSession(H: LongInt): SmallInt; StdCall;
Function TMEndSession(H: LongInt): SmallInt; StdCall;

{ File Operations Layer }
Function TMFirstFile(H: LongInt; P: Pointer; F: Pointer): SmallInt; StdCall;
Function TMNextFile(H: LongInt; P: Pointer; F: Pointer): SmallInt; StdCall;
Function TMOpenFile(H: LongInt; P: Pointer; F: Pointer): SmallInt; StdCall;
Function TMCreateFile(H: LongInt; P: Pointer; L: Pointer; F: Pointer): SmallInt; StdCall;
Function TMCloseFile(H: LongInt; P: Pointer; G: SmallInt): SmallInt; StdCall;
Function TMReadFile(H: LongInt; P: Pointer; G: SmallInt; F: Pointer; M: SmallInt): SmallInt; StdCall;
Function TMWriteFile(H: LongInt; P: Pointer; G: SmallInt; F: Pointer; M: SmallInt): SmallInt; StdCall;
Function TMDeleteFile(H: LongInt; P: Pointer; F: Pointer): SmallInt; StdCall;
Function TMFormat(H: LongInt; P: Pointer): SmallInt; StdCall;
Function TMAttribute(H: LongInt; P: Pointer; A: SmallInt; F: Pointer): SmallInt; StdCall;
Function TMReNameFile(H: LongInt; P: Pointer; A: SmallInt; F: Pointer): SmallInt; StdCall;
Function TMChangeDirectory(H: LongInt; P: Pointer; N: SmallInt; F: Pointer): SmallInt; StdCall;
Function TMDirectoryMR(H: LongInt; P: Pointer; N: SmallInt; F: Pointer): SmallInt; StdCall;
Function TMCreateProgramJob(H: LongInt; P: Pointer): SmallInt; StdCall;
Function TMDoProgramJob(H: LongInt; P: Pointer): SmallInt; StdCall;
Function TMWriteAddFile(H: LongInt; P: Pointer; G,O,S: SmallInt; F: Pointer; M: SmallInt): SmallInt; StdCall;
Function TMTerminateAddFile(H: Longint; P: Pointer; F: Pointer): Smallint; StdCall;
Function TMGetFamilySpec(H: Longint; P: Pointer; F: Pointer): Smallint; StdCall;
Function Get_Version(V: Pointer): SmallInt; StdCall;

{ Transport Layer }
Function TMReadPacket(H: LongInt; P: Pointer; G: SmallInt; D: Pointer; M: SmallInt): SmallInt; StdCall;
Function TMWritePacket(H: LongInt; P: Pointer; G: SmallInt; D: Pointer; L: SmallInt): SmallInt; StdCall;
Function TMBlockIO(H: Longint; P: Pointer; T: Pointer; N: Smallint ): Smallint; StdCall;
Function TMExtendedReadPage(H: LongInt; P: Pointer; G: SmallInt; D: Pointer; T: SmallInt): SmallInt; StdCall;
Function TMProgramByte(H: Longint; P: Pointer; M: Smallint; I : Pointer; N: Smallint; T:Pointer; Z: SmallInt): Smallint; StdCall;
Function TMProgramBlock(H: Longint; P: Pointer; W: Pointer; L: Smallint; A: SmallInt; B: Pointer): Smallint; StdCall;
Function TMCRC(L: SmallInt; P: Pointer; S: Word; T: Smallint): Smallint; StdCall;

{ Network Layer }
Function TMFirst(H: LongInt; P: Pointer): SmallInt; StdCall;
Function TMNext(H: LongInt; P: Pointer): SmallInt; StdCall;
Function TMAccess(H: LongInt; P: Pointer): SmallInt; StdCall;
Function TMStrongAccess(H: LongInt; P: Pointer): SmallInt; StdCall;
Function TMStrongAlarmAccess(H: LongInt; P: Pointer): SmallInt; StdCall;
Function TMOverAccess (H: Longint; P: Pointer): Smallint; StdCall;
Function TMRom(H: Longint; P: Pointer; D: Pointer): Smallint; StdCall;
Function TMFirstAlarm(H: Longint; P: Pointer): Smallint; StdCall;
Function TMNextAlarm (H: Longint; P: Pointer): Smallint; StdCall;
Function TMFamilySearchSetup(H: Longint; P: Pointer; G: Smallint): Smallint; StdCall;
Function TMSkipFamily (H: Longint; P: Pointer): Smallint; StdCall;
Function TMAutoOverDrive(H: LongInt; P: Pointer; M: SmallInt): SmallInt; StdCall;
Function TMSearch(H: LongInt; P: Pointer; R: SmallInt; PR: SmallInt; SC: SmallInt): SmallInt; StdCall;

{ Hardware Specific Layer }
Function TMSetup(H: LongInt): SmallInt; StdCall;
Function TMTouchByte(H: LongInt; B: SmallInt): SmallInt; StdCall;
Function TMTouchReset(H: LongInt): SmallInt; StdCall;
Function TMTouchBit(H: LongInt; B: SmallInt): SmallInt; StdCall;
Function TMProgramPulse(H: Longint): Smallint; StdCall;
Function TMOneWireLevel (H: Longint; O: SmallInt; L: SmallInt; P: SmallInt): SmallInt; StdCall;
Function TMOneWireCom (H: Longint; O: SmallInt; T: SmallInt): SmallInt; StdCall;
Function TMClose (H: Longint): Smallint; StdCall;
Function TMGetTypeVersion(N: SmallInt; V: Pointer): SmallInt; StdCall;
Function TMBlockStream(H: LongInt; P: Pointer; G: SmallInt): SmallInt; StdCall;
Function TMReadDefaultPort(P: Pointer; T: Pointer): SmallInt; StdCall;
Function TMGetAdapterSpec(H: LongInt; P: Pointer; S: Pointer): SmallInt; StdCall;


Implementation


{ Session Layer }
Function TMExtendedStartSession; External 'IBFS32.DLL';
Function TMStartSession; External 'IBFS32.DLL';
Function TMValidSession; External 'IBFS32.DLL';
Function TMEndSession; External 'IBFS32.DLL';

{ File Operations Layer }
Function TMFirstFile; External 'IBFS32.DLL';
Function TMNextFile; External 'IBFS32.DLL';
Function TMOpenFile; External 'IBFS32.DLL';
Function TMCreateFile; External 'IBFS32.DLL';
Function TMCloseFile; External 'IBFS32.DLL';
Function TMReadFile; External 'IBFS32.DLL';
Function TMWriteFile; External 'IBFS32.DLL';
Function TMDeleteFile; External 'IBFS32.DLL';
Function TMFormat; External 'IBFS32.DLL';
Function TMAttribute; External 'IBFS32.DLL';
Function TMReNameFile; External 'IBFS32.DLL';
Function TMChangeDirectory; External 'IBFS32.DLL';
Function TMDirectoryMR; External 'IBFS32.DLL';
Function TMCreateProgramJob; External 'IBFS32.DLL';
Function TMDoProgramJob; External 'IBFS32.DLL';
Function TMWriteAddFile; External 'IBFS32.DLL';
Function TMTerminateAddFile; External 'IBFS32.DLL';
Function TMGetFamilySpec; External 'IBFS32.DLL';
Function Get_Version; External 'IBFS32.DLL';

{ Transport Layer }
Function TMReadPacket; External 'IBFS32.DLL';
Function TMWritePacket; External 'IBFS32.DLL';
Function TMBlockIO; External 'IBFS32.DLL';
Function TMExtendedReadPage; External 'IBFS32.DLL';
Function TMProgramByte; External 'IBFS32.DLL';
Function TMProgramBlock; External 'IBFS32.DLL';
Function TMCRC; External 'IBFS32.DLL';

{ Network Layer }
Function TMFirst; External 'IBFS32.DLL';
Function TMNext; External 'IBFS32.DLL';
Function TMAccess; External 'IBFS32.DLL';
Function TMStrongAccess; External 'IBFS32.DLL';
Function TMStrongAlarmAccess; External 'IBFS32.DLL';
Function TMOverAccess; External 'IBFS32.DLL';
Function TMRom; External 'IBFS32.DLL';
Function TMFirstAlarm; External 'IBFS32.DLL';
Function TMNextAlarm; External 'IBFS32.DLL';
Function TMFamilySearchSetup; External 'IBFS32.DLL';
Function TMSkipFamily; External 'IBFS32.DLL';
Function TMAutoOverDrive; External 'IBFS32.DLL';
Function TMSearch; External 'IBFS32.DLL';

{ Hardware Specific Layer }
Function TMSetup; External 'IBFS32.DLL';
Function TMTouchByte; External 'IBFS32.DLL';
Function TMTouchReset; External 'IBFS32.DLL';
Function TMTouchBit; External 'IBFS32.DLL';
Function TMProgramPulse; External 'IBFS32.DLL';
Function TMOneWireLevel; External 'IBFS32.DLL';
Function TMOneWireCom; External 'IBFS32.DLL';
Function TMClose; External 'IBFS32.DLL';
Function TMGetTypeVersion; External 'IBFS32.DLL';
Function TMBlockStream; External 'IBFS32.DLL';
Function TMReadDefaultPort; External 'IBFS32.DLL';
Function TMGetAdapterSpec; External 'IBFS32.DLL';


End.





