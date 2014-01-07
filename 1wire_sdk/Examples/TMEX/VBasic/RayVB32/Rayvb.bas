Attribute VB_Name = "RAYVB1"
'//--------------------------------------------------------------------------
'// Copyright (C) 1992-2002 Dallas Semiconductor/MAXIM Corporation.
'// All rights Reserved. Printed in U.S.A.
'// This software is protected by copyright laws of
'// the United States and of foreign countries.
'// This material may also be protected by patent laws of the United States
'// and of foreign countries.
'// This software is furnished under a license agreement and/or a
'// nondisclosure agreement and may only be used or copied in accordance
'// with the terms of those agreements.
'// The mere transfer of this software does not imply any licenses
'// of trade secrets, proprietary technology, copyrights, patents,
'// trademarks, maskwork rights, or any other form of intellectual
'// property whatsoever. Dallas Semiconductor retains all ownership rights.
'//--------------------------------------------------------------------------
' Version 4.0
'

Option Explicit

'// Global Variable Declaration

'// session handle
Global SHandle As Long
'// data state space for DLL
Global state_buffer(15360) As Byte
'// port selection
Global PortNum As Integer
Global PortType As Integer
'// debounce array
Global Debounce(100) As Integer
'// save selected values
Global SelectROM(8) As Integer
Global SelectFile(6) As Byte
'// directory of the current opened device
Global DirBuf(256) As Byte
'// data to save
Global SaveEditData As String
'// flag to indicate if TMSetup has been run
Global SetupDone As Integer
'// max debounce value
Global MaxDbnc As Integer





'ByVal ID_buf$
' Change the directory from the current working directory to the
' sub-directory name given in RS.  RS can be a back reference '..'.
'
Sub ChangeDirectory(RS As String)
    Dim flag As Integer
    Dim Dmmy As Integer
    Dim TDir(6) As Byte
    Dim I As Integer
    
    '// start a session
    SHandle = TMExtendedStartSession(PortNum, PortType, vbNullString)
    If (SHandle > 0) Then
        '// set the selected rom
        flag = TMRom(SHandle, state_buffer(0), SelectROM(0))
        '// first change the directory to the correct current directory
        '// need to do this because background task of listing roms has
        '// reset the current directory
        flag = TMChangeDirectory(SHandle, state_buffer(0), 0, DirBuf(0))
        '// change the directory to the selected sub directory
        TDir(0) = 1
        TDir(1) = Asc(".")
        For I = 1 To 4
          TDir(I + 1) = Asc(Mid$(RS, I, 1))
        Next I
        flag = TMChangeDirectory(SHandle, state_buffer(0), 0, TDir(0))
        Dmmy = TMEndSession(SHandle)
        If (flag > 0) Then
            '// attempt to get the file list of the choosen device
            '// recreate the select string
            RS = ""
            For flag = 7 To 0 Step -1
                If (SelectROM(flag) < 16) Then
                   RS = RS + "0"
                End If
                RS = RS + Hex$(SelectROM(flag))
            Next flag
            GetFileList RS
        End If
    End If
End Sub

Sub EditFileData(RS As String)
    Dim I, J, hndl, ln, flag, rslt As Integer
    Dim Buf(7140) As Byte
    Dim tstr As String
    
    '// set result flag
    rslt = False

    '// record the selected file
    For I = 0 To 3
       SelectFile(I) = Asc(Mid$(RS, I + 1, 1))
    Next I
    SelectFile(4) = Val(Right$(RS, 3))

    '// start a session
    SHandle = TMExtendedStartSession(PortNum, PortType, vbNullString)

    '// if SHandle valid then try to read the file to edit
    If (SHandle > 0) Then
        '// set the selected rom
        flag = TMRom(SHandle, state_buffer(0), SelectROM(0))
        '// first change the directory to the correct current directory
        '// need to do this because background task of listing roms has
        '// reset the current directory
        flag = TMChangeDirectory(SHandle, state_buffer(0), 0, DirBuf(0))
        '// try to open the file
        hndl = TMOpenFile(SHandle, state_buffer(0), SelectFile(0))
        If (hndl >= 0) Then
            '// file is open, so read
            ln = TMReadFile(SHandle, state_buffer(0), hndl, Buf(0), 7140)
            flag = TMCloseFile(SHandle, state_buffer(0), hndl)
            If (ln >= 0) Then
                '// valid read so display and edit
                tstr = ""
                For I = 0 To ln - 1
                    tstr = tstr + Chr$(Buf(I))
                Next I
                FileData.EditData.Text = tstr
                SaveEditData = FileData.EditData.Text
                '// construct a title for the form
                I = 1
                While (Mid$(RS, I, 1) <> " ")
                    I = I + 1
                Wend
                FileData.Caption = Left$(RS, I - 1) + "." + Right$(RS, 3) + "    " + DirList.Caption
                FileData.Left = DirList.Left + 1000
                FileData.Top = DirList.Top + 1000
                FileData.Show
                rslt = True
            End If
        End If
        '// close the opened session
        flag = TMEndSession(SHandle)
    Else
        MsgBox "Could not get permission to use the 1-Wire", 16, "RAYVB"
    End If

    '// check result flag
    If (Not rslt) Then
        MsgBox "Could not read data file", 16, "RAYVB"
    End If
End Sub

Sub GetFileList(RS As String)
    Dim I, J, flag, rslt As Integer
    Dim FileStr(50) As Byte
    Dim TmpDir As String
    Dim tstr As String

    '// set result flag
    rslt = False

    '// record the selected file rom
    For I = 0 To 7
        SelectROM(I) = Val("&H" + Mid$(RS, (7 - I) * 2 + 1, 2))
    Next I

    '// clear the Directory list
    Do While (DirList.FileList.ListCount > 0)
        DirList.FileList.RemoveItem 0
    Loop

    '// start a session
    SHandle = TMExtendedStartSession(PortNum, PortType, vbNullString)

    '// if SHandle valid then try to read directory
    If (SHandle > 0) Then
        '// set the selected rom
        flag = TMRom(SHandle, state_buffer(0), SelectROM(0))

        '// read the directory to see if need to add back references
        flag = TMChangeDirectory(SHandle, state_buffer(0), 1, DirBuf(0))

        '// set the title of the filelist to include rom number and current directory
        TmpDir = Str$(PortNum) + ":\"
        For I = 1 To DirBuf(0)
            For J = 1 To 4
                If (DirBuf(J + 1 + (I - 1) * 4) <> &H20) Then
                    TmpDir = TmpDir + Chr$(DirBuf(J + 1 + (I - 1) * 4))
                Else
                    Exit For
                End If
            Next J
            TmpDir = TmpDir + "\"
        Next I
        DirList.Caption = RS + "  " + TmpDir

        '// get first file
        rslt = TMFirstFile(SHandle, state_buffer(0), FileStr(0))
        Do While (rslt >= 1)
            '// check to see if file or subdirectory
            If (FileStr(4) = &H7F) Then
                '// check to see if directory is hidden
                If (FileStr(7) <> 2) Then
                    tstr = ""
                    For J = 0 To 3
                        tstr = tstr + Chr$(FileStr(J))
                    Next J
                    '// decide where to place the new directory so that all directories at top
                    DirList.FileList.AddItem (tstr + "  <DIR>")
                End If
            Else
                tstr = ""
                For J = 0 To 3
                    tstr = tstr + Chr$(FileStr(J))
                Next J
            
                DirList.FileList.AddItem (tstr + "   " + Format$(FileStr(4), "000"))
            End If
            rslt = TMNextFile(SHandle, state_buffer(0), FileStr(0))
        Loop
    
        '// end the open session
        flag = TMEndSession(SHandle)
    End If

    '// hide the file edit window
    FileData.Hide
    
    '// size and display the directory list box if rslt ok
    If (rslt >= 0) Then
        DirList.FileList.ListIndex = DirList.FileList.ListCount - 1
        DirList.Left = RegNum.Left + 1000
        DirList.Top = RegNum.Top + 1000
        DirList.FileList.Height = 2895
        DirList.Show
    Else
        DirList.Hide
        MsgBox "Could not read file directory", 16, "RAYVB"
    End If
End Sub

'// Routine that is executed first
'
Sub Main()
    Dim ps, ln As Integer
    Dim Buf As String * 150
    Dim NBuf As String
    Dim cstring(150) As Byte
    Dim I As Integer
    Dim RetValue As Integer
    
    '// read the Registry to get the default PortNum and PortType
    '// set defaults
    MaxDbnc = 3
    '// read Registry for the PortNum and PortType
    RetValue = TMReadDefaultPort(PortNum, PortType)
    If (RetValue < 1) Then
        PortNum = 1  '// COM 1
        PortType = 5 '// DS9097U style adapter (DS2480B based)
    End If
    
    '// check the command line arguments
    ln = Len(Command$)
    ps = InStr(1, Command$, " ")
    If (ln > 0) Then
        If (ln = 1) Then
            PortNum = Val(Command$)
        ElseIf ((ln > 3) And (ps = 2)) Then
            PortNum = Val(Left$(Command$, ps))
            MaxDbnc = Val(Right$(Command$, ln - ps))
        Else
            MsgBox "Error in command line argument!", 16, "RAYVB"
        End If
    End If

    '// get the main and hardware driver version
    I = Get_Version(Buf)
    NBuf = Left$(Buf, InStr(1, Buf, Chr$(0), 1) - 1)
    RegNum.DriverLab.Caption = NBuf
    If (TMGetTypeVersion(PortType, Buf) > 0) Then
        NBuf = Left$(Buf, InStr(1, Buf, Chr$(0), 1) - 1)
        RegNum.DriverLab.Caption = RegNum.DriverLab.Caption + Chr$(&HD) + NBuf
    End If
        
    RegNum.Caption = "Registration Numbers on Port " + Str$(PortNum) + " Type " + Str$(PortType)
    
    '// size and display the registration number box
    RegNum.Left = 1000
    RegNum.Top = 1000
    RegNum.RegList.Height = 2895
    RegNum.Show
End Sub

'// Replace the file 'SaveFile' with the data in
'// 'FileData.EditData.Txt' field.
'
Sub ReplaceFile()
    Dim flag, I, J, hndl, ln, rslt  As Integer
    Dim mln As Integer
    Dim Epr As Integer
    Dim ans As Integer
    Dim Buf(7140) As Byte
    
    '// result flag
    rslt = False
    Epr = False

    '// start a session
    SHandle = TMExtendedStartSession(PortNum, PortType, vbNullString)

    '// if SHandle valid then try to read the file to edit
    If (SHandle > 0) Then
        '// set the selected rom
        flag = TMRom(SHandle, state_buffer(0), SelectROM(0))
        '// first change the directory to the correct current directory
        '// need to do this because background task of listing roms has
        '// reset the current directory
        flag = TMChangeDirectory(SHandle, state_buffer(0), 0, DirBuf(0))

        '// check for an program job
        flag = TMCreateProgramJob(SHandle, state_buffer(0))
        If (flag = 1) Then Epr = True

        '// delete the existing file
        flag = TMDeleteFile(SHandle, state_buffer(0), SelectFile(0))
        If ((flag = 1) Or (flag = -6)) Then
            '// success or delete of file already deleted
            '// create the file to write
            hndl = TMCreateFile(SHandle, state_buffer(0), mln, SelectFile(0))
            If (hndl >= 0) Then
                '// success in create
                ln = Len(FileData.EditData.Text)
                For I = 1 To ln
                    Buf(I - 1) = Asc(Mid$(FileData.EditData.Text, I, 1))
                Next I
                flag = TMWriteFile(SHandle, state_buffer(0), hndl, Buf(0), ln)
                If (flag = ln) Then
                    '// success with write
                    FileData.Hide
                    rslt = True
                End If
            End If
        End If

        '// check to see if need to finish program job
        If (Epr = True And rslt = True) Then
            '// loop trying to write the device
            '// until done or a user abort
            Do
                flag = TMDoProgramJob(SHandle, state_buffer(0))
                If (flag < 0) Then
                    If (flag = -22) Then
                        ans = MsgBox("Device is written such that updating is impossible.", 53, "RAYVB")
                    ElseIf (flag = -23) Then
                        ans = MsgBox("Program device can not be written with non-program devices on the 1-Wire.", 53, "RAYVB")
                    ElseIf (flag = -1) Then
                        ans = MsgBox("Device not found, replace and press retry!", 53, "RAYVB")
                    ElseIf (flag = -200) Then
                        SHandle = TMExtendedStartSession(PortNum, PortType, vbNullString)
                    ElseIf (flag = -13) Then
                        ans = MsgBox("Can not be program with this hardware or software configuration!", 53, "RAYVB")
                    Else
                        ans = MsgBox("Unknown error! " + Str$(flag), 53, "RAYVB")
                    End If
        
                    '// check result of message box
                    If (ans = 2) Then  '// Cancel
                        rslt = False
                        '// reset the program job
                        flag = TMCreateProgramJob(SHandle, state_buffer(0))
                        Exit Do
                    End If
                End If
            Loop Until (flag = 1)
        End If

        '// end the session
        flag = TMEndSession(SHandle)
    End If

    '// check result flag
    If (Not rslt) Then
        MsgBox "Could not write data file", 16, "RAYVB"
    End If
End Sub

