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

   Main unit for the DS1994 Delphi example
}
unit Timeun32;

interface

uses
  SysUtils, WinTypes, WinProcs, Messages, Classes, Graphics, Controls,
  Forms, Dialogs, Menus, StdCtrls, iBTMEXPW, ExtCtrls;

type
  TTimeForm = class(TForm)
    MainDriverLabel: TLabel;
    TypeDriverLabel: TLabel;
    MicroLanLabel: TLabel;
    ROMLabel: TLabel;
    ROMNumberLabel: TLabel;
    TimeLabel: TLabel;
    MainMenu: TMainMenu;
    MenuExit: TMenuItem;
    ClockTimer: TTimer;
    procedure MenuExitClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    function  FindFirstFamily(family: SmallInt; SHandle: LongInt; Var ROMStr: String): Boolean;
    Function  Convert_TimeDate(TimeBuf : PChar) : TDateTime;
    Procedure DisplayTime(TimeReg : TDateTime);
    function  ReadTimeReg(TimeBuf: PChar): Boolean;
    Function  GetLong(LongBuf : PChar) : LongInt;
    procedure ClockTimerTimer(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
    PortNum, PortType: SmallInt;
    TimeBuf : array[0..5] of Byte; { array of timer buffer }
  end;

var
  TimeForm: TTimeForm;

implementation

{$R *.DFM}

{-----------------------------------------------------------------
  Called when main window is created
}
procedure TTimeForm.FormCreate(Sender: TObject);
Var
   RetStr : array[0..200] of Char;
   ROMStr : String;
   Done : Boolean;
   Info,RetValue : SmallInt;
begin
   { Read the Registry for the Port Number and Port Type }
   RetValue := TMReadDefaultPort(@PortNum, @PortType);
   if (RetValue < 1) then
   begin
      PortNum := 1;  // COM 1
      PortType := 5; // DS9097U adapter type
   end;

   { Read the versions of the TMEX drivers }
   Get_Version(@RetStr);
   MainDriverLabel.Caption := 'Main Driver: ' + StrPas(RetStr);
   TMGetTypeVersion(PortType,@RetStr);
   TypeDriverLabel.Caption := 'Type' + IntToStr(PortType) + ': ' + StrPas(RetStr);
   MicroLanLabel.Caption := 'Port Number: ' + IntToStr(PortNum) + '   Port Type: ' + IntToStr(PortType);

   { Attempt to find a DS1994 on the MicroLan }
   { loop to get a session }
   Done := False;
   Repeat
      { get a session }
      SHandle := TMExtendedStartSession(PortNum,PortType,nil);
      If (SHandle > 0) Then
      Begin
         { to a TMSetup on the MicroLan }
         If (TMSetup(SHandle) = 1) Then
         Begin
            { find the first DS1994 on the MicroLan }
            If (FindFirstFamily($04,SHandle,ROMStr)) Then
            Begin
               TMEndSession(SHandle);
               ROMNumberLabel.Caption := ' ' + ROMStr + ' ';
               Done := True;
            End
            Else
            Begin
               TMEndSession(SHandle);
               ShowMessage('Could not find a DS1994 on the MicroLan!');
               Halt;
            End;
         End
         Else
         Begin
            TMEndSession(SHandle);
            ShowMessage('Failed to setup MicroLan!');
            Halt;
         End;
      End
      { check for invalid type  }
      Else If (SHandle < 0) Then
      Begin
         ShowMessage('The default Port Type does not have a driver!');
         Halt;
      End;

      { release control back to Windows }
      Application.ProcessMessages;

   Until (Done);

   { read and display the time }
   If (ReadTimeReg(@TimeBuf)) Then
      DisplayTime(Convert_TimeDate(@TimeBuf))
   Else
   Begin
      ShowMessage('Error reading the time register!');
      Halt;
   End;

   { start the clock timer }
   ClockTimer.Enabled := True;
end;

{-----------------------------------------------------------------
   Display the time/date
}
Procedure TTimeForm.DisplayTime(TimeReg : TDateTime);
Var
   Year,Month,Day,Hour,Min,Sec,MSec: Word;
Begin
   DecodeTime(TimeReg,Hour,Min,Sec,MSec);
   DecodeDate(TimeReg,Year,Month,Day);
   TimeLabel.Caption := Format(' %.2d/%.2d/%.4d  %.2d:%.2d:%.2d.%.2d ',
          [Month,Day,Year,Hour,Min,Sec,MSec div 10]);
   MessageBeep(MB_OK);
End;


{-----------------------------------------------------------------
  Find the first device with the family type 'family'
}
function TTimeForm.FindFirstFamily(family: SmallInt;
             SHandle: LongInt; Var ROMStr: String): Boolean;
Var
   j : SmallInt;
   rt : Boolean;
   ROM : Array [0..8] of SmallInt;
Begin
   { default return }
   rt := False;
   { set up to find the first device with family family }
   If (TMFamilySearchSetup(SHandle,@Statebuf,family) = 1) Then
   Begin
      { get first device in list with the specified family }
      If (TMNext(SHandle,@Statebuf) = 1) Then
      Begin
         { read the rom number }
         ROM[0] := 0;
         TMRom(SHandle,@Statebuf,@ROM);

         { check if correct type }
         If ((family And $7F) = (ROM[0] And $7F)) Then
         Begin
            ROMStr := '';
            For j := 7 downto 0 do
               ROMStr := ROMStr + Format('%.2X',[ROM[j]]);
            rt := True;
         End;
      End;
   End;

   { set return value }
   FindFirstFamily := rt;
End;

{-----------------------------------------------------------------
  Read the DS1994 time register
}
function TTimeForm.ReadTimeReg(TimeBuf: PChar): Boolean;
Var
   j,cnt : SmallInt;
   Change : Boolean;
   ch : Char;
Begin
   { loop to get a session }
   Repeat
      { get a session }
      SHandle := TMExtendedStartSession(PortNum,PortType,nil);
      If (SHandle > 0) Then
      Begin
         { loop to read time register }
         cnt := 0;
         Change := True;
         Repeat
            If (TMStrongAccess(SHandle,@Statebuf) = 1) Then
            Begin
               TMTouchByte(SHandle,$F0); { read ram command }
               TMTouchByte(SHandle,$02); { address 1 }
               TMTouchByte(SHandle,$02); { address 2 }
               Change := False;
               For j := 0 to 4 Do
               Begin
                  ch := Char(TMTouchByte(SHandle,$FF));
                  If (TimeBuf[j] <> ch) Then
                  Begin
                     If (j <> 0) Then
                        Change := TRUE;
                     TimeBuf[j] := ch;
                  End;
               End;
            End;
            { count tries }
            Inc(cnt);
         Until ((Not Change) Or (cnt > 4));

         { close the opened session }
         TMEndSession(SHandle);
         Break;
      End;

      { release control back to Windows }
      Application.ProcessMessages;
   Until (1 <> 1);

   { set return value }
   ReadTimeReg := Not Change;
End;

{----------------------------------------------------------------------------}
{ Convert the buffer to a time/date structure
}
Function TTimeForm.Convert_TimeDate(TimeBuf : PChar) : TDateTime;
Const
   DM : Array[1..13] of Word =
      (0,31,59,90,120,151,181,212,243,273,304,334,365);
Var
   frsecond,second,minute,hour,day,month,year : Word;
   x,y : LongInt;
   tmp,i,j : SmallInt;
begin
   { convert the array of bytes to a long }
   x := GetLong(@TimeBuf[1]);

   { calculate the fractions of a second }
   frsecond := Round(LongInt(TimeBuf[0]) * 1000.0 / 256.0);

   { check for date over 2020 }
   If (x > $5FDD4280) Then
      x := 0;

   Try
      y := x div 60;
      second := Word(x - 60 * y);
      x := y div 60;
      minute := Word(y - 60 * x);
      y := x div 24;
      hour := Word(x - 24 * y);
      x := 4 * (y + 731);
      year := Word(x div 1461);
      i := Word((x - 1461 * (LongInt(year))) div 4);
      month := 1 + (i + 1) div 30;
      If ((month > 2) And ((year And $03) = 0)) Then
         tmp := 1
      Else
         tmp := 0;
      If (month > 13) Then
         month := 1;
      j := dm[month] + tmp;
      If (i < j) Then
      Begin
         Dec(month);
         If ((month > 2) And ((year And $03) = 0)) Then
            tmp := 1
         Else
            tmp := 0;
         j := dm[month] + tmp;
      End;
      day := i - j + 1;
      If (year < 32) Then
         year := year + 68 + 1900
      Else
         year := year - 32 + 2000;
   Except
      frsecond := 0;
      second := 0;
      minute := 0;
      hour := 0;
      day := 0;
      month := 0;
      year := 0;
   End;

   { now convert to the pascal format }
   Try
      Convert_TimeDate := EncodeDate(year,month,day) +
                          EncodeTime(hour,minute,second,frsecond);
   Except
      Convert_TimeDate := StrToDateTime('01/01/70 01:00:00');
   End;
End;

{----------------------------------------------------------------------------}
{ convert a 4 byte buffer to a long SmallInt
}
Function TTimeForm.GetLong(LongBuf : PChar) : LongInt;
Var
   x : LongInt;
   i : SmallInt;
Begin
   x := 0;
   For i := 3 downto 0 Do
   Begin
      x := x shl 8;
      x := x Or LongInt(LongBuf[i]);
   End;
   GetLong := x;
End;

{-----------------------------------------------------------------
  Exit
}
procedure TTimeForm.MenuExitClick(Sender: TObject);
begin
   { end the program }
   Application.Terminate;
end;

{-----------------------------------------------------------------
  Read Time once a second
}
procedure TTimeForm.ClockTimerTimer(Sender: TObject);
begin
   If (ReadTimeReg(@TimeBuf)) Then
      DisplayTime(Convert_TimeDate(@TimeBuf))
   Else
      TimeLabel.Caption := ' No Device ';
end;

end.
