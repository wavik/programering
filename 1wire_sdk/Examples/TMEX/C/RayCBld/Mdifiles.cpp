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

#include "MdiFiles.h"
#include "MdiDev.h"
#include "MdiData.h"
#include "Globals.h"
//---------------------------------------------------------------------------
#pragma resource "*.dfm"
TDirList *DirList;
//---------------------------------------------------------------------------
__fastcall TDirList::TDirList(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
AnsiString __fastcall TDirList::GetPathString(const DirectoryPath Path)
{
    AnsiString pstring = "";
    for (int i=0; i<Path.NumEntries; i++) // Convert each sub-directory
    {
        if (i != 0) pstring = pstring + '\\';
        for (int j=0; j<4; j++) // Convert each letter of the directory name
        {
            if (Path.Entries[i][j] == ' ') break; // Remove spaces
            pstring = pstring + Path.Entries[i][j];
        }
    }
    return pstring; // Return the path as one long string
}
//---------------------------------------------------------------------------
void __fastcall TDirList::GetFileList()
{
    AnsiString filestr="";
    short flag,cnt=0;
    FileEntry Flname;
    DirNumInfo* DirNumObject;
// Start a session with a valid SHandle
    SHandle = -1;
    while (SHandle <= 0)
    {
        SHandle = TMExtendedStartSession(PortNum, PortType, 0);
    }
// Clear the list box
   	DirList->FileList->Items->Clear();
    TMRom(SHandle, state_buffer, ROM); // Read current device ROM
    flag = TMChangeDirectory(SHandle, state_buffer, 0, &Path); // Set path
    if (flag < 0) return;
// Get first file in list
    flag = TMFirstFile(SHandle, state_buffer, &Flname);
    while (flag>=1) // still have files in list
    {
    // Check to see if NOT a hidden file
        if (!(Flname.extension == 0x7F && Flname.attrib == 0x02))
        {
        // Convert the file/directory name to a string
           	AnsiString tempname = "";
           	for (int k=0;k<4;k++)
                tempname = tempname + char(Flname.name[k]);
            if (Flname.extension == 127) // A directory
                filestr = tempname + "      " + "<Dir>";
            else if (int(Flname.extension) <10)
                filestr = tempname + "       " + "00" + Flname.extension;
            else if (int(Flname.extension) <100)
                filestr = tempname + "       " + "0" + Flname.extension;
            else
                filestr = tempname + "       " + Flname.extension;
        // Record the specs in the file object
            DirNumObject = new DirNumInfo;
            for (int i=0;i<4;i++)
                DirNumObject->Name[i] = Flname.name[i];
            DirNumObject->Extension = Flname.extension;
            DirNumObject->Attrib = Flname.attrib;
            TMChangeDirectory(SHandle, state_buffer, 1, &Path); // Read path
        // Convert the path to a string for the title caption
            pathstr = "";
            pathstr = GetPathString(Path);
            DirList->Caption = devicestr + pathstr;
        // Add the string and object to the list box
            DirList->FileList->Items->AddObject(filestr,(TObject*)DirNumObject);
        }
        if (++cnt >= 10) break; // Increment file counter
    // Get next file
        flag = TMNextFile(SHandle, state_buffer ,&Flname);
    }
    DirList->Show();
// Release the session
    TMEndSession(SHandle);
}
//---------------------------------------------------------------------------
void __fastcall TDirList::SelectButtonClick(TObject *Sender)
{
// Check to see if one is not selected
    if (DirList->FileList->ItemIndex < 0) return;
// Find selected object
    short IndexNum = (short)DirList->FileList->ItemIndex;
    SFile = (FileEntry*)DirList->FileList->Items->Objects[IndexNum];
// Check to see if it is a subdirectory
    if (SFile->extension == 127)
    {
    // Set new directory and read file list
	    Path.NumEntries = 1;
	    Path.Ref = '.';
	    for (int i=0; i<4; i++)
    	    Path.Entries[0][i] = SFile->name[i];
	    GetFileList();
    }
// If not a sub-directory, show file contents for editing
    else
	    FileData->EditFileData();
}
//---------------------------------------------------------------------------
void __fastcall TDirList::OnDblClick(TObject *Sender)
{
// Find selected object
    short IndexNum = (short)DirList->FileList->ItemIndex;
    SFile = (FileEntry*)DirList->FileList->Items->Objects[IndexNum];
// Check to see if it is a subdirectory
    if (SFile->extension == 127)
    {
    // Set new directory and read file list
	    Path.NumEntries = 1;
	    Path.Ref = '.';
	    for (int i=0; i<4; i++)
		    Path.Entries[0][i] = SFile->name[i];
	    GetFileList();
    }
// If not a sub-directory, show file contents for editing
    else
	    FileData->EditFileData();
}
//---------------------------------------------------------------------------
void __fastcall TDirList::CancelButtonClick(TObject *Sender)
{
    pathstr = ""; // Clear global
    DirList->Caption = ""; // Clear caption
    DirList->Hide();
}
//---------------------------------------------------------------------------

