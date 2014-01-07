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
unit Upunit;

interface

uses
  SysUtils, WinTypes, WinProcs, Messages, Classes, Graphics, Controls,
  Forms, Dialogs, StdCtrls, ExtCtrls, iBTMEXPW;

type
  TUpdateForm = class(TForm)
    YesButton: TButton;
    NoButton: TButton;
    CancelButton: TButton;
    Image1: TImage;
    Label1: TLabel;
    Label2: TLabel;
    procedure NoButtonClick(Sender: TObject);
    procedure CancelButtonClick(Sender: TObject);
    procedure YesButtonClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }

  end;
  PRegNumInfo = ^RegNumInfo;        { pointer to reg number info }
  RegNumInfo = Record               { record of reg number info }
     ROM : array[0..8] of Smallint;  { array of rom number }
     Age : Byte;                    { age of rom number in list }
  End;
  PRecordInfo = ^RecordInfo ;        { pointer to reg number info}
  RecordInfo = Record                { record of reg number info}
       {data for file}
      Name : array[0..3] of char;    {four character filename}
      Extension : Byte;              { extension number, range 0-99, 127}
      Startpage : char;              {page number where file starts}
      Numpages : char;               {number of pages occupied by file}
      Attrib : char;                 {file/directory attribute}
      Bitmap: array[0..31]of char;   {current bitmap of the device}
       {data for directory}
      NumEntries : byte;
      Ref : char;
      Entries : array[0..9] of array [0..3] of char;
  End;
  CdBuf = record
    NumEntries : byte;
    Ref        : char;
    Entries    : array[0..9]of array[0..3] of char;
  end;
  FileEntry = record
    Name : array[0..3] of char;
    Extension : Byte;
    Startpage : char;
    Numpages : char;
    Attrib : char;
    Bitmap : array[0..31] of char;
  end;
  var
  UpdateForm: TUpdateForm;

implementation

{$R *.DFM}
uses
   Dirunit, Raydelun, Fileunit;

procedure TUpdateForm.NoButtonClick(Sender: TObject);
begin
  FileForm.Visible := False;
  Visible := False;
end;

procedure TUpdateForm.CancelButtonClick(Sender: TObject);
begin
  FileForm.Visible := False ;
  Visible := False;
end;

procedure TUpdateForm.YesButtonClick(Sender: TObject);
  { This procedure is used to update the file. Go to the directory that
    contains the file, then delete the file, create the new one with the
    same name and write the new content to it}
var
  fentry3 : FileEntry;
  DirPath3 : CdBuf;
  PRecordObject3: PRecordInfo;
  PRegNumObject4: PRegNumInfo;
  ROM4 : array[0..8] of Smallint;
  i ,j, result, filehandle, ln, Jobflag,DoJobflag,Closeflag : Smallint;
  IndexRec ,flag, Count: Smallint;
  MaxWrite :Smallint;
  SHandle, Handle : longint;
  Writeflag : boolean;
begin
  {Get the rom number of the selected Touch Memory}
  flag :=  RayDephMain.RegListBox.ItemIndex;
  PRegNumObject4 := PRegNumInfo(RayDephMain.RegListBox.Items.Objects[flag]);
  for i := 0 to 7 do
    ROM4[i] := PRegNumObject4^.ROM[i];
  TMRom(SHandle,@StateBuf,@ROM4);
     {get the current directory path and get the selected file}
  IndexRec := DirForm.RegListBox.ItemIndex;
  PRecordObject3 := PRecordInfo(DirForm.RegListBox.Items.Objects[IndexRec]);
     {data for selected file }
  for i := 0 to 3 do
     fentry3.Name[i] := PRecordObject3^.Name[i];
  fentry3.Extension := PRecordObject3^.Extension;
  fentry3.Startpage := PRecordObject3^.Startpage;
  fentry3.Numpages := PRecordObject3^.Numpages;
  fentry3.Attrib := PRecordObject3^.Attrib;
  for i := 0 to 31 do
     fentry3.Bitmap[i] := PRecordObject3^.Bitmap[i];
    {data for directory}
  DirPath3.NumEntries := PRecordObject3^.NumEntries;
  DirPath3.Ref := PRecordObject3^.Ref;
  for i := 0 to 9 do
     for j := 0 to 3 do
        DirPath3.Entries[i][j]:= PRecordObject3^.Entries[i][j];
    { attempt to start a session on port}
  SHandle := TMExtendedStartSession(RayDephMain.PortNum,RayDephMain.PortType,Nil);
  Handle := SHandle;
  Writeflag := True;
  If (Handle >0) then
   begin
    If(Not RayDephMain.SetupDone) then
     Begin
      flag := TMSetup(Handle);
      If((flag = 1) Or (flag = 2)) then
          RayDephMain.SetupDone := True;
     end;
    If(RayDephMain.SetupDone) then
     Begin
      TMRom(Handle,@StateBuf,@ROM4);
        {Change to the current directory}
      Flag := TMChangeDirectory(Handle,@StateBuf,0,@DirPath3);
      if (flag = 1) then
       begin
        Jobflag := TMCreateProgramJob(Handle,@StateBuf);
        if ((Jobflag = 1) Or (Jobflag = -2)) then
         begin
          result := TMDeleteFile(Handle,@StateBuf,@fentry3);
          if ((result = 1) Or (result = -6 ) Or (result >= 0) ) then
           begin
            filehandle :=
                    TMCreateFile(Handle,@StateBuf,@MaxWrite,@fentry3);
            if (filehandle >= 0) then
             begin
              ln := Length(FileForm.FileMemo.Text);
              result :=
                TMWriteFile(Handle,@StateBuf,filehandle,FileForm.Buffer,ln);
              if (result <> -10) then
               begin
                  if ((result > 0) Or (result = 0 )) then
                   begin
                    {Closeflag := TMCloseFile(Handle,@StateBuf,filehandle);
                    if (Closeflag = 1) then
                      begin    }
                       if (Jobflag = 1) then
                        begin
                          DoJobflag := -1;
                          Count := 0;
                          while DoJobflag < 1 do
                           begin
                            DoJobflag := TMDoProgramJob(Handle,@StateBuf);
                            Case DoJobflag of
                              -23 : begin
                                    ShowMessage('There are non-EPROM parts on 1-wire port.'
                                              + '  Please move them out of the Network');
                                    DoJobflag := 1;
                                    end;
                              -10 : begin
                                    ShowMessage('There are not enough room on device to write');
                                    TMEndSession(Handle);
                                    SHandle := TMExtendedStartSession(RayDephMain.PortNum,RayDephMain.PortType,Nil);
                                    TMCreateProgramJob(SHandle,@StateBuf);
                                    DoJobflag := 1;
                                    end;
                            else
                              begin
                                if (Count >= 5) then
                                 begin
                                  ShowMessage('There is something wrong in hardware');
                                  DoJobflag := 1;
                                 end
                                else
                                  Count := Count + 1;
                              end;
                            end;
                           end;
                        end;
                     { end;
                    else
                       ShowMessage('Error in close file ' + IntToStr(Closeflag)); }
                   end
                  else
                   ShowMessage(' error in writing the file' + IntToStr(result));
                 end
                else
                 begin
                  Closeflag := TMCloseFile(Handle,@StateBuf,filehandle);
                  if (Closeflag <> 1)then
                     ShowMessage('Error in close file ' + IntToStr(Closeflag))
                  else
                   begin
                     ShowMessage(' There is not enough space to write. Please '
                                 + 'make it shorter');
                     Writeflag := False;
                   end;
                 end;
               end
              else
                  ShowMessage('error in creating the file' + IntToStr(filehandle));
              end
           else
               ShowMessage('error in deleting a file'  + IntToStr(result));
         end
        else
          ShowMessage('Error in creating TMProgramJob : '+ IntToStr(Jobflag));
       end {end of if(flag change ddirectory)}
      else
          ShowMessage(' error in changing to the current directory');
       end; {end of if (set up)}
      TMEndSession(Handle);
      if Writeflag then
       FileForm.Visible := False;
      DirForm.Visible := False; 
      Visible := False;
  end;
  FileForm.changeflag := False;
end;
end.
