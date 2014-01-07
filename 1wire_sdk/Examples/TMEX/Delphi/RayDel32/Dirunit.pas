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
unit Dirunit;

interface

uses
  SysUtils, WinTypes, WinProcs, Messages, Classes, Graphics, Controls,
  Forms, Dialogs, StdCtrls, ExtCtrls, fileunit,iBTMEXPW;

type
  TDirForm = class(TForm)
    RegListBox: TListBox;
    Panel1: TPanel;
    SelectButton: TButton;
    CancelButton: TButton;
    Image1: TImage;
    Label1: TLabel;
    procedure SelectButtonClick(Sender: TObject);
    procedure OnDblClick(Sender: TObject);
    procedure DirList;
    procedure CancelButtonClick(Sender: TObject);
  private
    { Private declarations }
    ReadBuf : array[0..2048] of byte;
  public
    { Public declarations }
     LineContent : string;
     Shandle : Longint;
  End;
  PRegNumInfo = ^RegNumInfo;       { pointer to reg number info }
  RegNumInfo = Record              { record of reg number info }
     ROM : array[0..8] of Smallint; { array of rom number }
     Age : Byte;                   { age of rom number in list }
  End;
  PRecordInfo = ^RecordInfo ;      { pointer to file info}
  RecordInfo = Record              { record of file info}
       {data for file}
      Name : array[0..3] of char;  {four character filename}
      Extension : Byte;            { extension number, range 0-99, 127}
      Startpage : char;            {page number where file starts}
      Numpages : char;             {number of pages occupied by file}
      Attrib : char;               {file/directory attribute}
      Bitmap: array[0..31]of char; {current bitmap of the device}
       {data for directory}
      NumEntries : byte;           {Number of entries of path}
      Ref : char;                  {Refernce signal}
      Entries : array[0..9] of array [0..3] of char; {Ten entries, each entry
                                                     has four characters}
  End;

  CdBuf = record                   {Record of directory info}
    NumEntries : byte;
    Ref        : char;
    Entries    : array[0..9]of array[0..3] of char;
  end;
  FileEntry = record               { Record of file info}
    Name : array[0..3] of char;
    Extension : Byte;
    Startpage : char;
    Numpages : char;
    Attrib : char;
    Bitmap : array[0..31] of char;
  end;

var
  DirForm: TDirForm;
Const
  Max = 2048;
implementation
   uses
     Raydelun;
{$R *.DFM}
procedure TDirForm.SelectButtonClick(Sender: TObject);
begin
  DirList;
end;
procedure TDirForm.OnDblClick(Sender: TObject);
begin
  DirList;
end;
procedure TDirForm.DirList;
  { This procedure is used to figure out the selected item is a file or a
    subdirectory and then print out the content of the selected file or list
    all files and subdirectories of the selected subdirectory}
var
  fentry2 : FileEntry;
  DirPath2 : CdBuf;
  PRecordObject2: PRecordInfo;
  PRegNumObject3: PRegNumInfo;
  ROM3 : array[0..8] of Smallint;
  linestr2, titlestr, ftitlestr, dtitlestr, dtitle : string;
  flag, result,i, j, k,nonflag, flag1, flag2  : Smallint;
  IndexRec, Filehandle, Len, ReadMax, dlen  : Smallint;
  Handle : Longint;
begin
  titlestr := '';
  ftitlestr := '';
  dtitlestr := '';
  nonflag := 0;
    {get Rom number of the selected Touch Memory to create the title and update Statebuf}
  flag :=  RayDephMain.RegListBox.ItemIndex;
  titlestr := RayDephMain.RegListBox.Items[flag];
  titlestr := titlestr + '  ' + IntToStr(RayDephMain.PortNum) + ':';
  PRegNumObject3 := PRegNumInfo(RayDephMain.RegListBox.Items.Objects[flag]);
  for i := 0 to 7 do
    ROM3[i] := PRegNumObject3^.ROM[i];
  TMRom(SHandle,@StateBuf,@ROM3);
     {get the selected item to create the title of the file form and get
     informations about the current directory path and the file from the
          selected item}
  IndexRec := RegListBox.ItemIndex;
  linestr2 := RegListBox.Items[IndexRec];
    {flag1 and flag2 are used to figure out the selected item is '.   <Dir>' or
      '..    <Dir>'}
  flag1 := 0;
  flag2 := 0;
  if (linestr2 = '.     <Dir>') then
    flag1 := 1;
  if (linestr2 = '..    <Dir>') then
    flag2 := 1;
  PRecordObject2 := PRecordInfo(RegListBox.Items.Objects[IndexRec]);
     {data for selected file }
  for i := 0 to 3 do
     fentry2.Name[i] := PRecordObject2^.Name[i];
  fentry2.Extension := PRecordObject2^.Extension;
  fentry2.Startpage := PRecordObject2^.Startpage;
  fentry2.Numpages := PRecordObject2^.Numpages;
  fentry2.Attrib := PRecordObject2^.Attrib;
  for i := 0 to 31 do
     fentry2.Bitmap[i] := PRecordObject2^.Bitmap[i];
  for i := 0 to 3 do
    begin
      if (fentry2.Name[i]<> ' ') then
          ftitlestr :=  ftitlestr + fentry2.Name[i];
    end;
  ftitlestr := ftitlestr + '.' + IntToStr(fentry2.Extension);
      {data for directory}
  DirPath2.NumEntries := PRecordObject2^.NumEntries;
  DirPath2.Ref := PRecordObject2^.Ref;
  for i := 0 to 9 do
    begin
     for j := 0 to 3 do
        DirPath2.Entries[i][j]:= PRecordObject2^.Entries[i][j];
     if (DirPath2.Entries[i][0] <> ' ') then
     dtitlestr := dtitlestr + '\' + DirPath2.Entries[i];
    end;
   if (dtitlestr <> '') then
    begin
     If (dtitlestr[2] = '.') then
       Delete(dtitlestr,1,3);
     end;
    { attempt to start a session on port}
  SHandle := TMExtendedStartSession(RayDephMain.PortNum,RayDephMain.PortType,Nil);
  Handle := SHandle;
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
       TMRom(Handle,@StateBuf,@ROM3);
        {Change to the current directory}
       Flag := TMChangeDirectory(Handle,@StateBuf,0,@DirPath2);
       If (flag = 1) then
        begin
          If (fentry2.Extension = 127) then
          { the selected file is a subdirectory }
             begin
                { provide informations of subdirectory for DirPath2 }
                DirPath2.NumEntries := 1;
                DirPath2.Ref := '.';
                for j := 0 to 3 do
                   DirPath2.Entries[0][j] := fentry2.Name[j];
                for i:= 1 to 9 do
                      for j := 0 to 3 do
                         DirPath2.Entries[i][j] := ' ' ;
                  {set to the selected subdirectory}
               flag := TMChangeDirectory(Handle,@StateBuf,0,@DirPath2);
               if (flag = 1) then
                 begin
                   { add the name of the subdirectory to the directory path and
                      prepare the title for the Listbox}
                   DirForm.Caption := '';
                   for i := 0 to (DirPath2.NumEntries - 1) do
                     dtitlestr := dtitlestr + '\' + DirPath2.Entries[i];
                   dlen := Length(dtitlestr);
                   if (flag1 = 1) then
                    begin
                     Delete( dtitlestr,(dlen - 4),5);
                     if (dtitlestr[2] = '.') then
                        Delete(dtitlestr,1,5);
                    end;
                   if (flag2 = 1) then
                    Delete(dtitlestr, (dlen - 9),10);
                   dtitle := '';
                   dlen := Length(dtitlestr);
                   for i :=1 to dlen do
                     begin
                       if (dtitlestr[i] <> ' ') then
                         dtitle := dtitle + dtitlestr[i];
                      end;
                   {ShowMessage('titlestr : '+titlestr +' Dtitle : '+dtitle);  }
                   DirForm.Caption := titlestr  + dtitle;

                   {clear the directory list and its attached objects}
                   j := 0;
                   while j < DirForm.RegListBox.Items.Count do
                      begin
                     PRecordObject2 := PRecordInfo(RegListBox.Items.Objects[j]);
                       Dispose(PRecordObject2);
                       RegListBox.Items.Delete(j);
                       end;
                   linestr2 := '';
                   Show;
                   result := TMFirstFile(Handle,@StateBuf,@fentry2);
                    { Read all file entries of the subdirectory and create an
                      object for each entry}
                   while (result > 0) do
                    Begin
                      if (fentry2.Extension = 127)then
                       linestr2 := fentry2.Name + '  ' + '<Dir>'
                      Else
                       case fentry2.Extension of
    0..9: linestr2 := fentry2.Name + '   ' + '00' + IntToStr(fentry2.Extension);
    10..99:linestr2 := fentry2.Name + '   ' + '0' + IntToStr(fentry2.Extension);
    100..126:linestr2 := fentry2.Name + '   ' + IntToStr(fentry2.Extension);
                       end;
                      {set data to the file object}
                      PRecordObject2 := New(PRecordInfo);
                      for j:= 0 to 3 do
                        PRecordObject2^.Name[j] := fentry2.Name[j];
                      PRecordObject2^.Extension := fentry2.Extension;
                      PRecordObject2^.Startpage := fentry2.Startpage;
                      PRecordObject2^.Numpages := fentry2.Numpages;
                      PRecordObject2^.Attrib := fentry2.Attrib;
                      for j := 0 to 31 do
                      PRecordObject2^.Bitmap[j] := fentry2.Bitmap[j];
                      {set data to the directory object}
                      flag :=TMChangeDirectory(Handle,@StateBuf,1,@DirPath2);
                      If (flag =1) then
                        begin
                          PRecordObject2^.NumEntries := DirPath2.NumEntries;
                          PRecordObject2^.Ref := DirPath2.Ref;
                          for j := 0 to 9 do
                             for k := 0 to 3 do
                       PRecordObject2^.Entries[j][k] := DirPath2.Entries[j][k];
                        end
                      else
                         ShowMessage('error in reading the current directory');
                        {add the string and object to the listbox}
                  RegListBox.Items.AddObject(linestr2, Pointer(PRecordObject2));
                      result := TMNextFile(Handle,@StateBuf,@fentry2);
                    End; {end of while}
                end {end of if (change to subdirectory)}
              else
                ShowMessage('eror in setting to sub directory');
           end
          else
           begin
             { the selected file is a file, prepare informations for the title
                of the FileForm}
               FileForm.Caption := '';
               dlen := Length(dtitlestr);
               if (flag1 = 1) then
                   Delete( dtitlestr,(dlen - 4),5);
               if (flag2 = 1) then
                   Delete(dtitlestr, (dlen - 9),10);
               if (dtitlestr <> '') then
                begin
                 if (dtitlestr[2] = '.') then
                   Delete(dtitlestr,1,5);
                end;
               dtitle := '';
               dlen := Length(dtitlestr);
               for i :=1 to dlen do
                  begin
                    if (dtitlestr[i] <> ' ') then
                      dtitle := dtitle + dtitlestr[i];
                  end;
               FileForm.Caption := ftitlestr + '  ' + titlestr + dtitle;
               { clear ReadBuf, LineContent and FileForm before containing data}
               for i := 0 to Max do
                 ReadBuf[i] := 0;
               LineContent := '';
               FileForm.Show;
               j := 0;
               while j < FileForm.FileMemo.Lines.Count do
                  FileForm.FileMemo.Lines.Delete(j);
                 {  try to open file and read it}
               Filehandle := TMOpenFile(Handle,@StateBuf,@fentry2);
               if (Filehandle >= 0) then
                begin
                 Len := TMReadFile(Handle, @StateBuf, Filehandle,@ReadBuf,Max);
                  {close the file}
                 flag := TMCloseFile(Handle,@StateBuf, Filehandle);
                 { Check each character whether it is a non-printable character,
                   replace it by '.' and show the warning message. If it is CR
                   or LF or printable chareacter, copy it to the LineContent}
                 if (Len >= 0) then
                   begin
                    k := 0;
                    while k < Len  do
                      begin
                        if (( ReadBuf[k] < 32 )Or (ReadBuf[k] > 126)) then
                           begin
                             if ((ReadBuf[k] = 10) Or (ReadBuf[k] = 13)) then
                               LineContent := LineContent + Chr(ReadBuf[k])
                             else
                                begin
                                   LineContent := LineContent + '.';
                                   nonflag := 1;
                                 end;
                             end
                        else
                          LineContent := LineContent + Chr(ReadBuf[k ]);
                        k:= k + 1 ;
                      end; {end of while}
                    FileForm.FileMemo.Lines.Add(LineContent);
                    if (nonflag = 1) then
    ShowMessage('Warning the file contains at least 1 non-printable character');
                   end {end of len}
                 else
                  ShowMessage('can not read the file');
                end
               else
                  ShowMessage('can not open the file');
          end ;    {end of if (extension)}
         end  {end of if (setting to the current directory)}
       else
           ShowMessage('can not change to the current directory');
       end;    {end of if (set )}
       TMEndSession(Handle);
    end;   {end of if (SHandle}
  FileForm.changeflag := False;
end;

procedure TDirForm.CancelButtonClick(Sender: TObject);
begin
   Visible := False;
end;

end.

