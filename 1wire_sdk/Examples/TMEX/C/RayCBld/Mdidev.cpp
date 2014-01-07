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
#define TMEX

#include "MdiDev.h"
#include "MdiFiles.h"
#include "Globals.h"
//---------------------------------------------------------------------------
#pragma resource "*.dfm"
TDeviceList *DeviceList;
//---------------------------------------------------------------------------
__fastcall TDeviceList::TDeviceList(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TDeviceList::ExitButtonClick(TObject *Sender)
{
    Close();
}
//-----------------------------------------------------------------------
void __fastcall TDeviceList::FormCreate(TObject *Sender)
{
    // Set port number and type, etc.
    char tstr[100];
    PortType = 5;  // Default DS9097U setting
    PortNum = 1;   // COM 1

    // Read the default PortNum and PortType from the registry

    if (TMReadDefaultPort(&PortNum, &PortType) < 1)
    {
       // if not successfully read, then do the following
     	ShowMessage("Could not read the port number and type from registry!");
     	DeviceList->ExitButtonClick(Sender);
    }
    // Check to see that the PortNum is valid
    if (PortNum > 15)
    {
    	ShowMessage("Error: Invalid port number!");
        DeviceList->ExitButtonClick(Sender);
    }
// Show the version information in the text boxes on the Device List form
   	if (Get_Version(tstr) == 1) // Version of main driver
        DeviceList->Label1->Caption = AnsiString(tstr);
   	if (TMGetTypeVersion(PortType,tstr) == 1) // Hardware specific driver
      	DeviceList->Label2->Caption = AnsiString(tstr);
    SetupDone = false; // Initialize variable
}
//---------------------------------------------------------------------------
void __fastcall TDeviceList::Timer1Timer(TObject *Sender)
{
// Declarations
    short search, flag, ROMtemp[8];
    AnsiString tstr;
    long TimerHandle=-1;
    RegNumInfo* RegNumObject;
// Create title of form
    DeviceList->Caption = "Registration Number on Port " + IntToStr(PortNum);
// Start a session with a valid TimerHandle
    while (TimerHandle <= 0)
    {
        TimerHandle = TMExtendedStartSession(PortNum, PortType, 0);
    }
// Check to see if setup needs to be done
    if (!SetupDone)
	{
        TMSetup(TimerHandle);
        SetupDone = true;
    }
// Look for the 'next' device on the 1-wire
    search = TMNext(TimerHandle, state_buffer);
    if (search==1) // There is a next device
    {
    // Device found: get its ROM code
        ROMtemp[0] = 0;
        TMRom(TimerHandle, state_buffer, ROMtemp);
        tstr="";
        for (int i=0;i<8;i++)
            tstr = tstr + IntToHex(ROMtemp[7-i],2);
	// Check for ROM in list
        flag = (short) DeviceList->RegList->Items->IndexOf(tstr);
        if (flag>=0) // Device already on list
            RegNumObject = (RegNumInfo*)DeviceList->RegList->Items->Objects[flag];
    // Device not on list: create it
        else
        {
            RegNumObject = new RegNumInfo;
            for (int j=0;j<8;j++)
                RegNumObject->ROM[j] = ROMtemp[j];
            DeviceList->RegList->Items->AddObject(tstr,(TObject*)RegNumObject);
        }
        RegNumObject->age = MaxDbnc;
    }
// No more devices found
    else
    {
        int k=0;
        while (k < DeviceList->RegList->Items->Count)
        {
            RegNumObject = (RegNumInfo*)(DeviceList->RegList->Items->Objects[k]);
            if (RegNumObject->age>0)
            {
                RegNumObject->age = RegNumObject->age - 1;
                k++;
            }
            else
                DeviceList->RegList->Items->Delete(k);
        }
    }
// End current session
    TMEndSession(TimerHandle);
}
//---------------------------------------------------------------------------
void __fastcall TDeviceList::SelectButtonClick(TObject *Sender)
{
// Check to see if a device is chosen
    if (DeviceList->RegList->ItemIndex >= 0)
    {
    // Find selected object
        RegNumInfo* RegNumObject;
        short temp = (short) DeviceList->RegList->ItemIndex;
        RegNumObject = (RegNumInfo*)DeviceList->RegList->Items->Objects[temp];
        for (int i=0;i<8;i++)
            ROM[i] = RegNumObject->ROM[i];
    // Initialize path parameters to root
        Path.NumEntries = 0;
        Path.Ref = '\\';
        devicestr = "";
        devicestr = DeviceList->RegList->Items->Strings[temp] + "  "
          + IntToStr(PortNum) + ":\\";
    // Obtain and display the file list of the selected device
        DirList->GetFileList();
    }
}
//---------------------------------------------------------------------------
void __fastcall TDeviceList::OnDblClick(TObject *Sender)
{
// Find selected object
    RegNumInfo* RegNumObject;
    short temp = (short) DeviceList->RegList->ItemIndex;
    RegNumObject = (RegNumInfo*)DeviceList->RegList->Items->Objects[temp];
    for (int i=0;i<8;i++)
        ROM[i] = RegNumObject->ROM[i];
// Initialize path parameters to root
    Path.NumEntries = 0;
    Path.Ref = '\\';
    devicestr = "";
    int IndexNum = DeviceList->RegList->ItemIndex;
    devicestr = DeviceList->RegList->Items->Strings[IndexNum] + "  "
      + IntToStr(PortNum) + ":\\";
// Obtain and display the file list of the selected device
    DirList->GetFileList();
}
//---------------------------------------------------------------------------
