/*--------------------------------------------------------------------------
 | Copyright (C) 1992 - 2002 Dallas Semiconductor/MAXIM Corporation.
 | All rights Reserved. Printed in U.S.A.
 | This software is protected by copyright laws of
 | the United States and of foreign countries.
 | This material may also be protected by patent laws of the United States
 | and of foreign countries.
 | This software is furnished under a license agreement and/or a
 | nondisclosure agreement and may only be used or copied in accordance
 | with the terms of those agreements.
 | The mere transfer of this software does not imply any licenses
 | of trade secrets, proprietary technology, copyrights, patents,
 | trademarks, maskwork rights, or any other form of intellectual
 | property whatsoever. Dallas Semiconductor retains all ownership rights.
 |--------------------------------------------------------------------------*/
//---------------------------------------------------------------------------
#include <vcl\vcl.h>
#pragma hdrstop

#include "MdiData.h"
#include "MdiFiles.h"
#include "MdiDev.h"
#include "Globals.h"
//---------------------------------------------------------------------------
#pragma resource "*.dfm"
TFileData *FileData;
//---------------------------------------------------------------------------
__fastcall TFileData::TFileData(TComponent* Owner)
    : TForm(Owner)
{
	FileData->Data->Text = ""; // Clear text editor
    FileData->Caption = ""; // Clear caption
}
//---------------------------------------------------------------------------
void __fastcall TFileData::CancelButtonClick(TObject *Sender)
{
    FileData->Hide();
    DirList->GetFileList();

}
//---------------------------------------------------------------------------
void __fastcall TFileData::EditFileData()
{
    unsigned char Buf[7140];
    AnsiString filestr;
// Start a session with a valid SHandle
    SHandle = -1;
    while (SHandle <= 0)
    {
        SHandle = TMExtendedStartSession(PortNum, PortType, 0);
    }
// Set the selected ROM and current directory
    TMRom(SHandle, state_buffer, ROM);
    TMChangeDirectory(SHandle, state_buffer, 0, &Path);
// Try to open the file
    int hndl = TMOpenFile(SHandle, state_buffer, SFile);
// File is open: read contents
    if (hndl >= 0)
    {
        FileData->Show();
    // Create form caption
        AnsiString tempname = "";
        if (Path.NumEntries != 0) tempname = '\\';
        for (int k=0;k<4;k++)
            tempname = tempname + char(SFile->name[k]);
        if (int(SFile->extension) <10)
            filestr = tempname + "." + "00" + SFile->extension;
        else if (int(SFile->extension) <100)
		    filestr = tempname + "." + "0" + SFile->extension;
        else
            filestr = tempname + "." + SFile->extension;
        FileData->Caption = devicestr + pathstr + filestr;
       	int length = TMReadFile(SHandle, state_buffer, (short)hndl, Buf, 7140);
        TMCloseFile(SHandle, state_buffer, (short)hndl);
    // Valid read: display file content
        if (length >= 0)
        {
          	AnsiString contentstr = "";
            for (int i=0; i<length; i++)
               	contentstr = contentstr + char(Buf[i]);
            FileData->Data->Text = contentstr;
            OriginalText = FileData->Data->Text;
        }
    }
    else // Error opening file
        ShowMessage("!Error occurred reading the file!");
// Release session
    TMEndSession(SHandle);
}
//---------------------------------------------------------------------------
void __fastcall TFileData::AcceptButtonClick(TObject *Sender)
{
// Compare original file with current contents and save new data
    if (!(FileData->Data->Text == OriginalText))
    {
    // Prompt a message box with yes and no buttons places as the top window
        int flag = MessageBox(0,"Saving will replace original file. Save?","",
          MB_YESNO + MB_TOPMOST);
        if (flag == 6) // User chose 'Yes'
            ReplaceFile();
    }
// Close edit window
//    else CancelButtonClick(Sender);
}
//---------------------------------------------------------------------------
void __fastcall TFileData::ReplaceFile()
{
    int JobReady = 0, result = false, flag = false, error = 0, error2 = 0;
    short handle;
    short max;
    unsigned char Buf[7140];

// Turn timer off or might lose session
    DeviceList->Timer1->Enabled = false;
// Start a session with a valid SHandle
    SHandle = -1;
    while (SHandle <= 0)
    {
        SHandle = TMExtendedStartSession(PortNum, PortType, 0);
    }
// Set the selected ROM and current directory
    TMRom(SHandle, state_buffer, ROM);
    TMChangeDirectory(SHandle, state_buffer, 0, &Path);
// Try to start a program job with the current device
    flag = TMCreateProgramJob(SHandle, state_buffer);
    if (flag == 1) JobReady = true; // Successfully started a program job: EPROM
    flag = TMDeleteFile(SHandle, state_buffer, SFile);
// Successful delete or file already deleted
    if ((flag == 1) || (flag == -6))
    {
    // Create the file to write
        handle = TMCreateFile(SHandle, state_buffer, &max, SFile);
        if (handle >= 0) // File successfully created
        {
            int length = FileData->Data->GetTextLen();
            if (max >= length) // There must be enough memory
            {
            	FileData->Data->SelectAll();
            	FileData->Data->GetSelTextBuf(Buf, 7140);
                flag = TMWriteFile(SHandle, state_buffer, handle, Buf, length);
                if (flag == length) // File successfully written
                    result = true;
                else error2 = MessageBox(0,WriteError(-100),"ERROR",
                  MB_OKCANCEL + MB_TOPMOST);
            }
            else
        		error2 = MessageBox(0,WriteError(-10),"ERROR",
                  MB_OKCANCEL + MB_TOPMOST);
        }
    }
// Unsuccessfule delete
    else error = MessageBox(0,WriteError(flag),"ERROR",MB_OKCANCEL + MB_TOPMOST);
// Check if need to finish program job: EPROM wrote to buffer
    if ((JobReady == true) && (result == true))
    {
        flag = 0;
        while (flag != 1) // Continue until done or user aborts
        {
            flag = TMDoProgramJob(SHandle, state_buffer);
            if (flag < 0)
            {
            // Prompt a message box for errors and let user cancel or continue
            	error = MessageBox(0,WriteError(flag),"ERROR",
                  MB_OKCANCEL + MB_TOPMOST);
                if (error == 2) flag = 1; // User chose to cancel operation
            }
        }
    }
// No error occurred or operation was completed after error occurred
	if ((error2 == 0 && error == 1) || (error == 0 && error2 == 0))
    {
    	FileData->Caption = "";
    	FileData->Hide();
        DirList->GetFileList();
    }
// Clear buffer on EPROM
    else if (JobReady == true)
        TMCreateProgramJob(SHandle, state_buffer);
// Release session
    TMEndSession(SHandle);
// Turn timer back on
    DeviceList->Timer1->Enabled = true;
// Note: if an error did occur and the user chose to cancel the operation,
// text editor remains open with the unsaved text.
}
//---------------------------------------------------------------------------
char* __fastcall TFileData::WriteError(const int flag)
{ 	char* message = "";
    if (flag == -1) message = "No device found!";
    else if (flag == -10) message = "Not enough memory on device!";
    else if (flag == -13) message = "Function not supported!";
    else if (flag == -15) message = "Cannot delete a read only file!";
    else if (flag == -22) message = "Device cannot be changed!";
    else if (flag==-23) message = "There are non-EPROM devices present!";
    else if (flag == -200) message = "Invalid session!";
    else message = "Error trying to write to device!";
	return message;
}
//---------------------------------------------------------------------------
