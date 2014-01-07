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

   Main unit for Raydelph.
}
unit Raydelun;

interface

uses
  SysUtils, WinTypes, WinProcs, Messages, Classes, Graphics, Controls,
  Forms, Dialogs, StdCtrls, ExtCtrls, iBTMEXPW, dirunit;

type
  TRayDephMain = class(TForm)
    RegListBox: TListBox;
    Panel1: TPanel;
    SelectButton: TButton;
    ExitButton: TButton;
    TMEXVersionLabel: TLabel;
    Label1: TLabel;
    TMTypeVersion: TLabel;
    Image1: TImage;
    procedure FormCreate(Sender: TObject);
    procedure ExitButtonClick(Sender: TObject);
    procedure SelectButtonClick(Sender: TObject);
    procedure TouchMemoryList;
    procedure OnDblClick(Sender: TObject);
  private
    { Private declarations }
    Procedure IdleAction(Sender: TObject; Var Done: Boolean);
  public
    { Public declarations }
    PortNum: Smallint;
    PortType: Smallint;
    SetupDone: Boolean;
    IndexNum  : Smallint;
  end;

  { types for object connected to registration number }
  PRegNumInfo = ^RegNumInfo;       { pointer to reg number info }
  RegNumInfo = Record              { record of reg number info }
     ROM : array[0..8] of Smallint; { array of rom number }
     Age : Byte;                   { age of rom number in list }
  End;

  {Type for object connected to the files or directories}
   PRecordInfo = ^RecordInfo ;     { pointer to File info}
   RecordInfo = Record             { record of file info}
        {data of file}
      Name : array[0..3] of char;  {four character filename}
      Extension : Byte;            { extension number, range 0-99, 127}
      Startpage : char;            {page number where file starts}
      Numpages : char;             {number of pages occupied by file}
      Attrib : char;               {file/directory attribute}
      Bitmap: array[0..31]of char; {current bitmap of the device}
          { data of directory}
      NumEntries : byte;
      Ref : char;
      Entries : array[0..9]of array[0..3] of char;
  End;
  FileEntry = Record               {record for file}
      Name : array[0..3] of char;  {four character filename}
      Extension : Byte;            { extension number, range 0-99, 127}
      Startpage : char;            {page number where file starts}
      Numpages : char;             {number of pages occupied by file}
      Attrib : char;               {file/directory attribute}
      Bitmap: array[0..31]of char; {current bitmap of the device}
  End;
  CdBuf = record                   {record of directory path}
    NumEntries : byte;
    Ref : char;
    Entries :array[0..9]of array[0..3]of char;
  end;
var
  RayDephMain: TRayDephMain;

Const
  PERSISTENCE = 30;

implementation

{$R *.DFM}

{-----------------------------------------------------------------}
{ Executed once at start of application }
{ (like main) }
{}
procedure TRayDephMain.FormCreate(Sender: TObject);
Var
   ztbuf : array[0..200] of Char;
   Typebuf : array[0..200] of Char;
   i,k ,RetValue: Smallint;
   RetStr : array[0..200] of Char;
begin
   { set default values }
  SetupDone := FALSE;           { TNSetup not done yet }
  {Take the values of Type and Number of Port}
  RetValue := TMReadDefaultPort(@PortNum, @PortType);
  if (RetValue  < 1) then
   begin
    ShowMessage('Please set port first');
    Halt;
   end
  else
   begin
    { read the tmex version and type version}
    Get_Version(@ztbuf);
    TMEXVersionLabel.Caption :=  StrPas(ztbuf);
    TMGetTypeVersion(PortType,@Typebuf);
    TMTypeVersion.Caption := StrPas(Typebuf);
    IndexNum := 0;
    { set up idle action routine }
    Application.OnIdle := IdleAction;
   end;
end;


{-----------------------------------------------------------------}
{ Idle routine, called whenever WINDOWS is not busy }
{}
Procedure TRayDephMain.IdleAction(Sender: TObject; Var Done: Boolean);
Var
   flag,i,K, j, result : Smallint;
   ROM : array[0..8] of Smallint;   { current TM rom }
   tstr: String;
   PRegNumObject : PRegNumInfo;    { pointer to reg info object }
Begin
   { Create the title of the ListBox}
   RayDephMain.Caption := 'Registration Number on Port ' + IntToStr(PortNum);
   { attempt to start a session on port }
   SHandle := TMExtendedStartSession(PortNum,PortType,Nil);
   { session successfull }
   If (SHandle > 0) Then
   Begin
      { check to see if TMSetup has been done }
      IF (Not SetupDone) Then
      Begin
         flag := TMSetup(SHandle);
         If ((flag = 1) Or (flag = 2)) Then
            SetupDone := TRUE;
      End;
      { find the next part on the 1-Wire }
      If (SetupDone) Then
      Begin
        result := TMNext(SHandle,@StateBuf);
         { found a Touch Memory if flag is 1 }
        if( result = 1 )then
         Begin
            {got a part so get the ROM }
            ROM[0] := 0;          { zero first to indicate read rom }
            TMRom(SHandle,@StateBuf,@ROM);
            { construct the string from the Smallint array }
            tstr := '';
            For i := 7 downto 0 do
            Begin
               tstr := tstr + IntToHex(ROM[i],2);
            End;
            { loop for the reg number in the listbox }
            flag := RegListBox.Items.IndexOf(tstr);
            If (flag >= 0) Then
               { Registration number already in list }
               { so get a pointer to it }
               PRegNumObject := PRegNumInfo(RegListBox.Items.Objects[flag])
            Else
            Begin
               { Reg number is new so create a new object }
               { to attatch to the list }
               PRegNumObject := New(PRegNumInfo);
               { set the rom in the object }
               For i := 0 to 7 do
                  PRegNumObject^.ROM[i] := ROM[i];
               { add the string and object to the listbox }
               RegListBox.Items.AddObject(tstr, Pointer(PRegNumObject));
               { beep to indicate a new registration number }
               MessageBeep(0);
            End;
            { update the persistence age of reg number }
            PRegNumObject^.Age := PERSISTENCE;
           end
           { else no more Touch Memory on this pass so age }
        else
          begin
         { loop through all of the items }
            K := 0;
            While K < RegListBox.Items.Count do
            Begin
               { get a pointer to the current object }
               PRegNumObject := PRegNumInfo(RegListBox.Items.Objects[K]);
               { check its age }
               If (PRegNumObject^.Age > 0) then
               Begin
                  { age ok so decrement and go to next item }
                  Dec(PRegNumObject^.Age);
                  Inc(K);
               End
               else
               Begin
                  { age not ok so delete from list }
                  Dispose(PRegNumObject);
                  RegListBox.Items.Delete(K);
                  { beep to indicate a device is gone }
                  MessageBeep(0);
               End;
            End;
        end;
       end;
         { end the session }
      TMEndSession(SHandle);
   End;
   { not done so IdleAction is called again }
   Done := False;
End;


{-----------------------------------------------------------------}
{ Exit button }
{}
procedure TRayDephMain.ExitButtonClick(Sender: TObject);
begin
   Halt;
end;

{-----------------------------------------------------------------}
{ temporary procedure, does nothing }
{}
procedure TRayDephMain.SelectButtonClick(Sender: TObject);
begin
  TouchMemoryList;
end;

procedure TRayDephMain.OnDblClick(Sender: TObject);
begin
  TouchMemoryList;
end;
procedure TRayDephMain.TouchMemoryList;
  {This procedure is used to list all directories and files that are contained
   in the selected touch memory}
var
  flag, j, k, result : Smallint;
  DirPath : Cdbuf;
  fentry : FileEntry;
  PRecordObject : PRecordInfo;
  linestr, titlestr : String;
  ROM2 : array[0..8] of Smallint; { current TM rom}
  PRegNumObject2 : PRegNumInfo;  {pointer to reg info object}
  Handle : Longint;
begin
  linestr := '';
  titlestr := '';
  { get Rom number of the selected Touch Memory from the list of Touch Memories to create
     the title of the directory list and update the stateBuf}
  IndexNum := RegListBox.ItemIndex;
  titlestr := RegListBox.Items[IndexNum] + '  ' + IntToStr(PortNum) + ':\';
  DirForm.Caption := '';
  DirForm.Caption := titlestr;
  PRegNumObject2 := PRegNumInfo(RegListBox.Items.Objects[IndexNum]);
  for j := 0 to 7 do
    ROM2[j] := PRegNumObject2^.ROM[j];
 TMRom(SHandle,@StateBuf,@ROM2); {Transfer the registration number of
                                  the selected Touch Memory to the StateBuf}
  { Clear the ListBox and its attached objects}
  j := 0;
  while j < DirForm.RegListBox.Items.Count do
   begin
     PRecordObject := PRecordInfo(DirForm.RegListBox.Items.Objects[j]);
     Dispose(PRecordObject);
     DirForm.RegListBox.Items.Delete(j);
   end;
   { attempt to check if the session has been established on port and take
     the session handle}
  SHandle := TMExtendedStartSession(PortNum,PortType,Nil);
  Handle := SHandle;
  If (SHandle > 0) then
   { The session has been established on the port, so check flag SetupDone first
    to verify the port exists }
    Begin
     If(Not SetupDone) then
     { Call TMSetup. This function must be called before any other TMEX functions.
     No other TMEX API calls will operate correctly until the MicroLan has been
     called with this function  }
       Begin
        flag := TMSetup(SHandle);
        If((flag = 1) Or (flag = 2)) then
          SetupDone := True;
    end;
   If(SetupDone) then
    {Call TMRom to read the current device ROM}
      Begin
       TMRom(SHandle,@StateBuf,@ROM2);
        { clear fentry and DirPath}
       DirPath.NumEntries := 0;
       DirPath.Ref := ' ';
       for j := 0 to 9 do
         for k := 0 to 3 do
           DirPath.Entries[j][k]:= ' ';
       for j:=0 to 3 do
        fentry.Name[j] := ' ';
       fentry.Extension := 0;
       fentry.Startpage := ' ';
       fentry.Numpages := ' ';
       fentry.Attrib := ' ';
       for j := 0 to 31 do
        fentry.Bitmap[j]:= ' ';
      { Read all file entries in the selected Touch Memory , create an object
        for each file entry and attach these objects to the list of files
        and directories of this Touch Memory  }
      flag := TMChangeDirectory(SHandle,@StateBuf,0,@DirPath);
      if (flag = 1 ) then
       begin
        result := TMFirstFile(SHandle,@StateBuf,@fentry);
        if ((result = 0 )Or (result <0)) then
           ShowMessage('Could not read directory')
        else
         begin
          DirForm.Show;
          while (result > 0) do
           Begin
             {provide informations for a line of the ListBox. If the extension
              is 127 the entry is a directory entry.}
             if (fentry.Extension = 127)then
             linestr := fentry.Name + '  ' + '<Dir>'
             Else
               case fentry.Extension of
     0..9: linestr := fentry.Name + '   ' + '00' + IntToStr(fentry.Extension);
     10..99:linestr := fentry.Name + '   ' + '0' + IntToStr(fentry.Extension);
     100..126:linestr := fentry.Name + '   ' + IntToStr(fentry.Extension);
                end;
               {set data to the file object}
             PRecordObject := New(PRecordInfo);
             for j:= 0 to 3 do
                PRecordObject^.Name[j] := fentry.Name[j];
             PRecordObject^.Extension := fentry.Extension;
             PRecordObject^.Startpage := fentry.Startpage;
             PRecordObject^.Numpages := fentry.Numpages;
             PRecordObject^.Attrib := fentry.Attrib;
             for j := 0 to 31 do
                 PRecordObject^.Bitmap[j] := fentry.Bitmap[j];
                  {set data to the directory object}
             flag :=TMChangeDirectory(SHandle,@StateBuf,1,@DirPath);
             If (flag =1) then
                begin
                  PRecordObject^.NumEntries := DirPath.NumEntries;
                  PRecordObject^.Ref := DirPath.Ref;
                  for j := 0 to 9 do
                     for k := 0 to 3 do
                        PRecordObject^.Entries[j][k] := DirPath.Entries[j][k];
                 end
             else
                ShowMessage('error in reading the current directory');
                {add the string and object to the listbox}
            DirForm.RegListBox.Items.AddObject(linestr, Pointer(PRecordObject));
            result := TMNextFile(SHandle,@StateBuf,@fentry);
           End;
         end;
        end
      else
        ShowMessage('Could not open the directory');
      TMEndSession(SHandle);
  End;

end;
end;
end.
