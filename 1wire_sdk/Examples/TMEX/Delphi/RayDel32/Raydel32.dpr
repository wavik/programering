{--------------------------------------------------------------------------
 | Copyright (C) 1992-2002 Dallas Semiconductor Corporation.
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
 |--------------------------------------------------------------------------
    Version 3.10
 }
                                                                             
program raydel32;

uses
  Forms,
  raydelun in 'raydelun.pas' {RayDephMain},
  dirunit in 'dirunit.pas' {DirForm},
  fileunit in 'fileunit.pas' {FileForm},
  upunit in 'upunit.pas' {UpdateForm},
  iBTMEXPW in 'iBTMEXPW.pas';

{$R *.RES}

begin
  Application.CreateForm(TRayDephMain, RayDephMain);
  Application.CreateForm(TDirForm, DirForm);
  Application.CreateForm(TFileForm, FileForm);
  Application.CreateForm(TUpdateForm, UpdateForm);
  Application.Run;
end.
