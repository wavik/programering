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
    TEMPDL32 : This utility uses TMEX to view a read the temperature from
             a DS1920/DS1820. It requires the 32-Bit Windows TMEX drivers
             to be present.
    Compiler : Borland Delphi 5.0
                                        }

unit Tempmain;

interface

uses
  SysUtils, WinTypes, WinProcs, Messages, Classes, Graphics, Controls,
  Forms, Dialogs, iBTMEXPW, StdCtrls, ExtCtrls;

type
  TForm1 = class(TForm)
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Image1: TImage;
    Label5: TLabel;
    procedure FormCreate(Sender: TObject);
    procedure FindFirstFamily(family : smallint; SHandle : Longint);
    procedure ReadTemperature(session_handle : Longint);
  private
    { Private declarations }
  public
    { Public declarations }
     SHandle : longint;
     StateBuf : array[0..5120] of smallint;
     CRC8 : Word;
     Done : boolean;
  end;
  Const
  LEVEL_SET  = 1;
  LEVEL_STRONG_PULL_UP = $01;
  PRIMED_BYTE =  2;
  PRIMED_NONE =  0;
  LEVEL_NORMAL =  $00;
var
  Form1: TForm1;

implementation

{$R *.DFM}

procedure TForm1.FormCreate(Sender: TObject);
Var
   ztbuf : array[0..200] of Char;
   Typebuf : array [0..200] of Char;
   i,k,RetValue : smallint;
   RetStr : array[0..200] of Char;
   SetupDone: Boolean;
   PortNum, PortType : smallint;

begin
  SetupDone := FALSE;           { TMSetup not done yet }
  Label4.Caption := '';
  {Read default Port Number and Port Type from registry}
  RetValue := TMReadDefaultPort(@PortNum, @PortType);
  if (RetValue < 1) then
   begin
    ShowMessage('Please set port first');
    Halt;
   end
  else
   begin
    { read the tmex version and type version}
    Get_Version(@ztbuf);
    Label1.Caption :=  StrPas(ztbuf);
    TMGetTypeVersion(PortType,@Typebuf);
    Label2.Caption := StrPas(Typebuf);
    {attemp to get a session }
    Done := False;
    Repeat
      SHandle := TMExtendedStartSession(PortNum,PortType,NIL);
      If (SHandle > 0) Then
       begin
         if (TMSetup(SHandle) = 1) then
           {The device that will be found is Temperature Device DS1920/DS1820,
            so Family Type is set to $10}
            FindFirstFamily($10,SHandle)
         else
           begin
             TMEndSession(SHandle);
             ShowMessage('Fail to setup MicroLan!');
             Halt;
           end;
       end
      else
       begin
        if (SHandle < 0 ) then
         Begin
           ShowMessage('The Default Port Type does not have a driver !');
           Halt;
         end;
       end;
      {Release control back to window}
      Application.ProcessMessages;
    until (Done);
   end;
end;

procedure TForm1.FindFirstFamily(family : smallint; SHandle: longint);
var
 i , flag : smallint;
 romstr : string;
 rom : array[0..8] of smallint;
begin
 {Set up to find the first device with the family 'family'}
 if (TMFamilySearchSetup(SHandle,@stateBuf,family ) = 1 ) then
  begin
    {Get first device}
    if (TMNext(SHandle, @stateBuf)= 1) then
     begin
      {Read the rom number by setting rom[0] to 0 for reading and using TMRom}
      rom[0] := 0;
      TMRom(SHandle,@stateBuf,@rom);
      {Check if correct type}
      romstr := '';
      if ((family and $7F )= (rom[0] and $7F)) then
       begin
        for i := 7 downto 0 do
          romstr := romstr + IntToHex(ROM[i],2);
        Label3.caption := 'Serial ROM ID :  ' + romstr;
        ReadTemperature(SHandle);
       end
      else
       begin
         ShowMessage('There is no Temperature Device on the port');
         halt;
       end;
     end
    else
     begin
       TMEndSession(SHandle);
       ShowMessage('There is no Temperature Device on the port');
       halt;
     end;
  end
 else
  begin
    TMEndSession(SHandle);
    ShowMessage('There is no Temperature Device on the port');
    halt;
  end;
end;

procedure TForm1.ReadTemperature(session_handle : longint);
var
 tsht, i, tmp1 : smallint;
 cr,cpc, tmpf,tmp : Extended;
 rbuf : array[0..9] of smallint ;
 st : longint;
 CRCByte : Byte;
begin
  tmp := 0.00;
 {access the device}
 if (TMAccess(session_handle,@StateBuf)= 1) then
  begin
   {Send the recall E2 command (by setting $B8 to outbyte in TMTouchByte)
    make sure Scratch is correct}
   TMTouchByte(session_handle, $B8);
   {Send the start T command }
   if (TMAccess(session_handle,@StateBuf) = 1) then
    begin
     {Prepare the strong pullup after next TMTouchByte}
     TMOneWireLevel(session_handle,LEVEL_SET,LEVEL_STRONG_PULL_UP, PRIMED_BYTE);
     {Send the conversion command (by setting $44 to outbyte in TMTouchByte)}
     TMTouchByte(session_handle, $44);
     {Sleep for a second}
     st := GetTickCount + 1000;
     While (GetTickCount < st) do
        TMValidSession(Session_handle);
     {Disable the strong pullup}
     TMOneWireLevel(session_handle, LEVEL_SET,LEVEL_NORMAL,PRIMED_NONE);
     {verify conversion is complete by setting $01 to outbit in TMTouchBit and
      check the return value with 1}
     if (TMTouchBit(session_handle,$01) = $01) then
      begin
       {Access device}
       If (TMAccess(session_handle,@StateBuf) = 1 ) then
        begin
         {Send read scratch command by setting $BE to outbyte in TMTouchByte}
         TMTouchByte(session_handle,$BE);
         CRC8 := 0;
         {Read scratch (setting $FF to outbyte in TMTouchByte) and check crc for
           each byte}
         for i := 0 to 7 do
          begin
            rbuf[i]:= TMTouchByte(session_handle, $FF);
            CRCByte := Byte(rbuf[i]); { the byte to run through CRC8 routine }
            CRC8 := TMCRC(1, @CRCByte, CRC8, 0);
          end;
         {Check crc}
         CRCByte := Byte(TMTouchByte(session_handle, $FF)); { the byte to run through CRC8 routine }
         CRC8 := TMCRC(1, @CRCByte, CRC8, 0);
         if ( CRC8 = 0 ) then
          begin
            {Calculate the temperarure}
             tsht := rbuf[0];
             if ((rbuf[1] and $01)= 1) then
               tsht := tsht or (-256);
             tmp1 := Round((tsht)/2);
             tmp := tmp1;
             cr := rbuf[6];
             cpc := rbuf [7];
             if (rbuf[7] <> 0) then
               tmp := tmp - (0.25) + (cpc-cr)/cpc;
             tmpf := (tmp * 9 )/5 + 32;
             label4.caption := 'Current temp   :  ' + FormatFloat('0.0',tmp) +
                               ' C or ' + FormatFloat('0.0', tmpf) + ' F';
             MessageBeep(0);
             TMEndSession(session_handle);
             Done := True;
          end;
        end;
      end;
    end;
  end;
end;

end.
