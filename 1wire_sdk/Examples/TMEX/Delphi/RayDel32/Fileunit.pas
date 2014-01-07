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
                                                                               }
unit Fileunit;

interface

uses
  SysUtils, WinTypes, WinProcs, Messages, Classes, Graphics, Controls,
  Forms, Dialogs, StdCtrls, ExtCtrls, iBTMEXPW, Upunit;

type
  TFileForm = class(TForm)
    Panel1: TPanel;
    AcceptButton: TButton;
    CancelButton: TButton;
    FileMemo: TMemo;
    Label1: TLabel;
    Image1: TImage;
    procedure AcceptButtonClick(Sender: TObject);
    procedure CancelButtonClick(Sender: TObject);
    procedure FileMemoChange(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
     changeflag : boolean;
     Buffer : PChar;
     Size : Byte;
  end;

var
  FileForm: TFileForm;

implementation

{$R *.DFM}
uses
    Dirunit;

procedure TFileForm.AcceptButtonClick(Sender: TObject);
   { This procedure is used to compare the content of the file before and after
      editing and show the update-form to choice }
var
  i, ln: Smallint;
  WriteBuf2 : string;
begin
   size := FileMemo.GetTextLen;
   Inc(Size);
   GetMem(Buffer, Size);
   FileMemo.GetTextBuf(Buffer,Size);
   if (changeflag) then
      UpdateForm.Show
   else
       Visible := False;

end;

procedure TFileForm.CancelButtonClick(Sender: TObject);
begin
  Visible := False;
end;

procedure TFileForm.FileMemoChange(Sender: TObject);
begin
  changeflag := True;
end;

end.
