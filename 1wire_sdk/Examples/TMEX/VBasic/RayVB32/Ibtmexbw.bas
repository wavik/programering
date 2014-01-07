Attribute VB_Name = "TMEXLIB"
'//---------------------------------------------------------------------
'// Copyright (C) 1992-2002 Dallas Semiconductor/MAXIM Corporation.
'// All rights Reserved. Printed in U.S.A.  This software is
'// protected by copyright laws of the United States and of foreign
'// countries.  This material may also be protected by patent laws
'// of the United States and of foreign countries.  This software is
'// furnished under a license agreement and/or a nondisclosure
'// agreement and may only be used or copied in accordance with the
'// terms of those agreements.  The mere transfer of this software
'// does not imply any licenses of trade secrets, proprietary
'// technology, copyrights, patents, trademarks, maskwork rights,
'// or any other form of intellectual property whatsoever. Dallas
'// Semiconductor retains all ownership rights.
'//---------------------------------------------------------------------

' External DLL function declarations

'// Session Layer functions
Declare Function TMExtendedStartSession Lib "IBFS32.DLL" (ByVal PortNum As Integer, ByVal PortType As Integer, ByVal Reserved As Any) As Long
Declare Function TMValidSession Lib "IBFS32.DLL" (ByVal session_handle As Long) As Integer
Declare Function TMEndSession Lib "IBFS32.DLL" (ByVal session_handle As Long) As Integer

'// File Operations Layer functions
Declare Function TMFirstFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, fentry As Byte) As Integer
Declare Function TMNextFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, fentry As Byte) As Integer
Declare Function TMOpenFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, fentry As Byte) As Integer
Declare Function TMCreateFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, maxwrite As Integer, fentry As Byte) As Integer
Declare Function TMCloseFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal file_handle As Integer) As Integer
Declare Function TMReadFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal file_handle As Integer, read_buffer As Byte, ByVal max_read As Integer) As Integer
Declare Function TMWriteFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal file_handle As Integer, write_buffer As Byte, ByVal num_write As Integer) As Integer
Declare Function TMDeleteFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, fentry As Byte) As Integer
Declare Function TMFormat Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMAttribute Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal attrib As Integer, fentry As Byte) As Integer
Declare Function TMReNameFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal file_handle As Integer, fentry As Byte) As Integer
Declare Function TMChangeDirectory Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal operation As Integer, cd_buf As Byte) As Integer
Declare Function TMDirectoryMR Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal operation As Integer, fentry As Byte) As Integer
Declare Function TMCreateProgramJob Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMDoProgramJob Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMWriteAddFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal operation As Integer, ByVal offset As Integer, write_buffer As Byte, ByVal num_write As Integer) As Integer
Declare Function TMTerminateAddFile Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, fentry As Byte) As Integer
Declare Function TMGetFamilySpec Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, FamSpec As Byte) As Integer
Declare Function Get_Version Lib "IBFS32.DLL" (ByVal ID_buf$) As Integer

'// Transport Layer functions
Declare Function TMReadPacket Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal StartPg As Integer, ReadBuf As Byte, ByVal MaxRead As Integer) As Integer
Declare Function TMWritePacket Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal StartPg As Integer, WriteBuf As Byte, ByVal Writelen As Integer) As Integer
Declare Function TMBlockIO Lib "IBFS32.DLL" (ByVal session_handle As Long, tran_buffer As Byte, ByVal num_tran As Integer) As Integer
Declare Function TMExtendedReadPage Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal StartPg As Integer, ReadBuf As Byte, ByVal MSpace As Integer) As Integer
Declare Function TMProgramByte Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal WRByte As Integer, ByVal Addr As Integer, ByVal MSpace As Integer, Bits As Integer, ByVal Zeros As Integer) As Integer
Declare Function TMProgramBlock Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, WriteBuf As Byte, ByVal Length As Integer, ByVal Address As Integer, Bits As Integer) As Integer
Declare Function TMCRC Lib "IBFS32.DLL" (ByVal Length As Integer, Buf As Byte, ByVal Seed As Integer, ByVal CRCType As Integer) As Integer

'// Network Layer functions
Declare Function TMFirst Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMNext Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMAccess Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMStrongAccess Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMStrongAlarmAccess Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMOverAccess Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMRom Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ROM As Integer) As Integer
Declare Function TMFirstAlarm Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMNextAlarm Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMFamilySearchSetup Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ByVal family_type As Integer) As Integer
Declare Function TMSkipFamily Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte) As Integer
Declare Function TMAutoOverDrive Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, Mode As Integer) As Integer
Declare Function TMSearch Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, ResetSearch As Integer, ByVal PerformReset As Integer, ByVal SrchCmd As Integer) As Integer

'// Hardware Specific Layer functions
Declare Function TMSetup Lib "IBFS32.DLL" (ByVal session_handle As Long) As Integer
Declare Function TMTouchByte Lib "IBFS32.DLL" (ByVal session_handle As Long, ByVal outbyte As Integer) As Integer
Declare Function TMTouchReset Lib "IBFS32.DLL" (ByVal session_handle As Long) As Integer
Declare Function TMTouchBit Lib "IBFS32.DLL" (ByVal session_handle As Long, ByVal outbit As Integer) As Integer
Declare Function TMProgramPulse Lib "IBFS32.DLL" (ByVal session_handle As Long) As Integer
Declare Function TMOneWireLevel Lib "IBFS32.DLL" (ByVal session_handle As Long, ByVal operation As Integer, ByVal LevelMode As Integer, ByVal primed As Integer) As Integer
Declare Function TMOneWireCom Lib "IBFS32.DLL" (ByVal session_handle As Long, ByVal operation As Integer, ByVal TimeMode As Integer) As Integer
Declare Function TMClose Lib "IBFS32.DLL" (ByVal session_handle As Long) As Integer
Declare Function TMGetTypeVersion Lib "IBFS32.DLL" (ByVal HSType As Integer, ByVal ID_buf$) As Integer
Declare Function TMBlockStream Lib "IBFS32.DLL" (ByVal session_handle As Long, tran_buffer As Byte, ByVal num_tran As Integer) As Integer
Declare Function TMReadDefaultPort Lib "IBFS32.DLL" (PortNum As Integer, PortType As Integer) As Integer
Declare Function TMGetAdapterSpec Lib "IBFS32.DLL" (ByVal session_handle As Long, state_buffer As Byte, AdapterSpec As Byte) As Integer


