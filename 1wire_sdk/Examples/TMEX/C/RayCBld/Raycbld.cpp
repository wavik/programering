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

//---------------------------------------------------------------------------
USERES("Raycbld.res");
USEFORM("MdiFiles.cpp", DirList);
USEFORM("MdiDev.cpp", DeviceList);
USEFORM("MdiData.cpp", FileData);
USELIB("ibfs32.lib");
//---------------------------------------------------------------------------
WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
	try
	{
		Application->Initialize();
		Application->CreateForm(__classid(TDeviceList), &DeviceList);
                 Application->CreateForm(__classid(TDirList), &DirList);
                 Application->CreateForm(__classid(TFileData), &FileData);
                 Application->Run();
	}
	catch (Exception &exception)
	{
		Application->ShowException(&exception);
	}
	return 0;
}
//---------------------------------------------------------------------------
