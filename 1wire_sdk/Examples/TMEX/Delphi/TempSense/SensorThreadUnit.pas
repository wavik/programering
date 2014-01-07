{--------------------------------------------------------------------------
 | Copyright (C) 1992-2005 Dallas Semiconductor Corporation.
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

   SensorThread Unit
}
unit SensorThreadUnit;

interface

uses
  Classes, Graphics, ComCtrls, Extctrls, Controls, Forms, StdCtrls, Windows,
  Sysutils, Messages, Menus, iBTMEXPW;

const
   { thread messages }
   WM_SensorReadingDone = WM_User + 8;
   WM_TerminateThreads = WM_User + 11;
   { Delay Constant }
   DelayBetweenLoops = 500;  { Delay between modes in milliseconds }
   { Strong pull-up constants }
   LEVEL_SET  = 1;
   LEVEL_STRONG_PULL_UP = $01;
   PRIMED_BYTE =  2;
   PRIMED_NONE =  0;
   LEVEL_NORMAL =  $00;
   

type
  TSensorThread = class(TThread)

  private
    { Private declarations }
    Function ReadTemperature(session_handle : longint): Extended;

  protected
    { Protected declarations }
    procedure Execute; override;

  published
    { Will use superclass constructor/destructor }

  public
    { Public declarations }
    ParentFormHandle : Hwnd;             { Reference to Sensor Form's window handle }
    Temperature : Real;
    PortType,PortNum: SmallInt;          { Port Number and Type }
    StateBuf : Array[1..15360] of Byte;  { TMEX state buffer }
    SHandle : LongInt;                   { TMEX Session handle }
    ROM : array[0..8] of SmallInt;       { array of rom number }
    DidSetup : Boolean;                  { flag if did setup (3.10) }
    RomNum : String;
    Function getTemperature : Real;
    Function getRomNumber : String;

  end;

implementation



{----------------------------------------------------------------------------}
{ Execute is the main method of the thread.
}
procedure TSensorThread.Execute;
var
   i: Integer;
begin
   While (Terminated = False) Do     { While thread is not terminated... }
   Begin
      { attempt to get a session }
      SHandle := TMExtendedStartSession(PortNum,PortType,nil);
      If (SHandle > 0) Then
      begin
         If (not DidSetup) Then
         Begin
            TMSetup(SHandle);
            DidSetup := true;
         End;
         // Get first device
         while (TMNext(SHandle, @StateBuf) = 1) do
         begin
            // Read the rom number by setting rom[0] to 0 for reading and using TMRom
            ROM[0] := 0;
            TMRom(SHandle,@StateBuf,@ROM);
            // Check if correct type
            if ROM[0] = $10 then
            begin
               Temperature := ReadTemperature(SHandle);
               RomNum := '';
               For i := 7 downto 0 do
               Begin
                  RomNum := RomNum + IntToHex(ROM[i],2);
               End;

               // Tell Form that Sensor Reading is now Done by passing a message to it.
               PostMessage(ParentFormHandle,WM_SensorReadingDone,ThreadID,0);
            end;
         end;
      end;
      TMEndSession(SHandle);
      Windows.Sleep(DelayBetweenLoops);
   end;
end;

{----------------------------------------------------------------------------}
{ Gets Temperature from 1-Wire part (DS18S20/DS1920)
}
Function TSensorThread.ReadTemperature(session_handle : longint): Extended;
var
 tsht, i, tmp1 : smallint;
 cr,cpc,tmp : Extended;
 rbuf : array[0..9] of smallint;
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
     Windows.Sleep(1000);
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

          end;
        end;
      end;
    end;
  end;
  ReadTemperature := tmp;
end;

{----------------------------------------------------------------------------}
{ Method to get temperature from TempSenseForm
}
Function TSensorThread.getTemperature : Real;
Begin
   getTemperature := Temperature;
End;

{----------------------------------------------------------------------------}
{ Method to get Rom Number (1-Wire Net address) from TempSenseForm
}
Function TSensorThread.getRomNumber : String;
Begin
   getRomNumber := RomNum;
End;

end.
