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

   Main unit for DS2406/7 switch delphi demo.
 }
unit Swun32;

interface

uses
  SysUtils, WinTypes, WinProcs, Messages, Classes, Graphics, Controls,
  Forms, Dialogs, StdCtrls, iBTMEXPW, Menus;

type
  TSwitchForm = class(TForm)
    MainDriverLabel: TLabel;
    TypeDriverLabel: TLabel;
    ROMLabel: TLabel;
    FlipFlopLabel: TLabel;
    SensedLevelLabel: TLabel;
    ActivityLatchLabel: TLabel;
    ChannelAFFButton: TButton;
    ChannelBFFButton: TButton;
    ChannelASensedLevel: TLabel;
    ChannelBSensedLevel: TLabel;
    ChannelAActivityLatch: TLabel;
    ChannelBActivityLatch: TLabel;
    ChannelALabel: TLabel;
    ChannelBLabel: TLabel;
    MicroLanLabel: TLabel;
    MainMenu: TMainMenu;
    MenuExit: TMenuItem;
    MenuReadInfo: TMenuItem;
    MenuClearActivity: TMenuItem;
    ROMNumberLabel: TLabel;
    procedure FormCreate(Sender: TObject);
    procedure MenuExitClick(Sender: TObject);
    function FindFirstFamily(family: SmallInt; SHandle: LongInt; Var ROMStr: String): Boolean;
    procedure MenuReadInfoClick(Sender: TObject);
    function ReadSwitchInfo(ResetActivity: Boolean; Var Info: SmallInt): Boolean;
    Procedure UpdateSwitchInfo(Info: SmallInt);
    procedure MenuClearActivityClick(Sender: TObject);
    procedure ChannelAFFButtonClick(Sender: TObject);
    function  SetFlipFlop(ffstate: SmallInt): Boolean;
    procedure ChannelBFFButtonClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
    PortNum, PortType: SmallInt;
    CRC16 : Word;
  end;

var
  SwitchForm: TSwitchForm;

implementation

{$R *.DFM}

{-----------------------------------------------------------------
  Called when main window is created
}
procedure TSwitchForm.FormCreate(Sender: TObject);
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
      PortNum := 1;
      PortType := 1;
   end;
   { Read the versions of the TMEX drivers }
   Get_Version(@RetStr);
   MainDriverLabel.Caption := 'Main Driver: ' + StrPas(RetStr);
   TMGetTypeVersion(PortType,@RetStr);
   TypeDriverLabel.Caption := 'Type ' + IntToStr(PortType) + ': ' + StrPas(RetStr);
   MicroLanLabel.Caption := 'Port Number: ' + IntToStr(PortNum) + '   Port Type: ' + IntToStr(PortType);

   { Attempt to find a DS2407 on the MicroLan }
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
            { find the first DS2407 on the MicroLan }
            If (FindFirstFamily($12,SHandle,ROMStr)) Then
            Begin
               TMEndSession(SHandle);
               ROMNumberLabel.Caption := ' ' + ROMStr + ' ';
               Done := True;
            End
            Else
            Begin
               TMEndSession(SHandle);
               ShowMessage('Could not find a DS2407 on the MicroLan!');
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

   { read and display the switch information }
   If (ReadSwitchInfo(True,Info)) Then
      UpdateSwitchInfo(Info);

end;

{-----------------------------------------------------------------
  Find the first device with the family type 'family'
}
function TSwitchForm.FindFirstFamily(family: SmallInt;
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
  Exit
}
procedure TSwitchForm.MenuExitClick(Sender: TObject);
begin
   { end the application }
   Application.Terminate;
end;

{-----------------------------------------------------------------
  Read the DS2406/7 switch information byte
}
function TSwitchForm.ReadSwitchInfo(ResetActivity: Boolean; Var Info: SmallInt): Boolean;
Var
   flag : SmallInt;
   rt : Boolean;
   CRCByte : Byte;
Begin
   { default return }
   rt := False;
   { loop to get a session }
   Repeat
      { get a session }
      SHandle := TMExtendedStartSession(PortNum,PortType,nil);
      If (SHandle > 0) Then
      Begin
         If (TMAccess(SHandle,@Statebuf) = 1) Then
         Begin
            { reset the CRC16 generator }
            CRC16 := 0;

            { channel access command }
            flag := TMTouchByte(SHandle, $F5);
            CRCByte := $F5;
            CRC16 := TMCRC(1, @CRCByte, 0, 1);

            { control bytes }
            If (ResetActivity) Then
               flag := TMTouchByte(SHandle,$D5)
            Else
               flag := TMTouchByte(SHandle,$55);

            CRCByte := Byte(flag);
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            flag := TMTouchByte(SHandle,$FF);
            CRCByte := $FF;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            { read the info byte }
            Info := TMTouchByte(SHandle,$FF);
            CRCByte := Info;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);


            { read a dummy read byte and CRC16 }
            flag := TMTouchByte(SHandle, $FF);
            CRCByte := flag;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            flag := TMTouchByte(SHandle, $FF);
            CRCByte := flag;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            flag := TMTouchByte(SHandle, $FF);
            CRCByte := flag;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            { check result }
            If (CRC16 = $B001) Then
               rt := True;
         End;

         { close the opened session }
         TMEndSession(SHandle);
         Break;
      End;

      { release control back to Windows }
      Application.ProcessMessages;
   Until (1 <> 1);

   { set return value }
   ReadSwitchInfo := rt;
End;

{-----------------------------------------------------------------
  Update the screen with the switch information
}
Procedure TSwitchForm.UpdateSwitchInfo(Info: SmallInt);
Begin
   { get the number of channels }
   If ((info And $40) = $40) Then
   Begin
      { make channel B stuff visible }
      ChannelBLabel.Visible := True;
      ChannelBFFButton.Visible := True;
      ChannelBSensedLevel.Visible := True;
      ChannelBActivityLatch.Visible := True;
   End;
   { make channel A Stuff visible }
   ChannelALabel.Visible := True;
   ChannelAFFButton.Visible := True;
   ChannelASensedLevel.Visible := True;
   ChannelAActivityLatch.Visible := True;

   { channel A flip flop }
   If ((info And $01) <> 0) Then
      ChannelAFFButton.Caption := 'OFF'
   Else
      ChannelAFFButton.Caption := 'ON';

   { channel B flip flop }
   If ((info And $02) <> 0) Then
      ChannelBFFButton.Caption := 'OFF'
   Else
      ChannelBFFButton.Caption := 'ON';

   { channel A sensed level }
   If ((info And $04) <> 0) Then
      ChannelASensedLevel.Caption := 'HIGH'
   Else
      ChannelASensedLevel.Caption := 'LOW';

   { channel B sensed level }
   If ((info And $08) <> 0) Then
      ChannelBSensedLevel.Caption := 'HIGH'
   Else
      ChannelBSensedLevel.Caption := 'LOW';

   { channel A activity latch }
   If ((info And $10) <> 0) Then
      ChannelAActivityLatch.Caption := 'SET'
   Else
      ChannelAActivityLatch.Caption := 'CLEAR';

   { channel B activity latch }
   If ((info And $20) <> 0) Then
      ChannelBActivityLatch.Caption := 'SET'
   Else
      ChannelBActivityLatch.Caption := 'CLEAR';

   { beep to indicate dislay has been updated }
   MessageBeep(MB_OK);
End;

{-----------------------------------------------------------------
  Set the flip-flop(s) on the switch
}
function TSwitchForm.SetFlipFlop(ffstate: SmallInt): Boolean;
Var
   flag : SmallInt;
   rt : Boolean;
   CRCByte : Byte;
Begin
   { default return }
   rt := False;
   { loop to get a session }
   Repeat
      { get a session }
      SHandle := TMExtendedStartSession(PortNum,PortType,nil);
      If (SHandle > 0) Then
      Begin
         If (TMAccess(SHandle,@Statebuf) = 1) Then
         Begin
            { reset the CRC generator }
            CRC16 := 0;

            { write status command }
            flag := TMTouchByte(SHandle,$55);
            CRCByte := $55;
            CRC16 := TMCRC(1, @CRCByte, 0, 1);

            { send address }
            flag := TMTouchByte(SHandle,$07);
            CRCByte := $07;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            flag := TMTouchByte(SHandle,$00);
            CRCByte := $00;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            { write the status }
            flag := TMTouchByte(SHandle,ffstate);
            CRCByte := flag;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            { CRC16 }
            flag := TMTouchByte(SHandle, $FF);
            CRCByte := flag;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            flag := TMTouchByte(SHandle, $FF);
            CRCByte := flag;
            CRC16 := TMCRC(1, @CRCByte, CRC16, 1);

            { check result }
            If (CRC16 = $B001) Then
               rt := True;
         End;

         { close the opened session }
         TMEndSession(SHandle);
         Break;
      End;

      { release control back to Windows }
      Application.ProcessMessages;

   Until (1 <> 1);

   { set the return value }
   SetFlipFlop := rt;
End;

{-----------------------------------------------------------------
  Read switch info (don't clear the activity latches)
}
procedure TSwitchForm.MenuReadInfoClick(Sender: TObject);
Var
   Info : SmallInt;
begin
   If (ReadSwitchInfo(False,Info)) Then
      UpdateSwitchInfo(Info)
   Else
      ShowMessage('Error reading the switch information!');
end;

{-----------------------------------------------------------------
  Read switch info (clear the activity latches)
}
procedure TSwitchForm.MenuClearActivityClick(Sender: TObject);
Var
   Info : SmallInt;
begin
   If (ReadSwitchInfo(True,Info)) Then
      UpdateSwitchInfo(Info)
   Else
      ShowMessage('Error reading the switch information!');
end;

{-----------------------------------------------------------------
  Toggle the state of FlipFlop Channel A
}
procedure TSwitchForm.ChannelAFFButtonClick(Sender: TObject);
Var
   ffstate: SmallInt;
   Info : SmallInt;
begin
   ffstate := 0;
   { create the byte to write to the flip flop }
   If (ChannelAFFButton.Caption = 'ON') Then
      ffstate := ffstate Or $01;
   If (ChannelBFFButton.Caption = 'OFF') Then
      ffstate := ffstate Or $02;
   ffstate := (ffstate shl 5) Or $1F;

   { set the flip flop state and the read it back }
   If (SetFlipFlop(ffstate)) Then
   Begin
      If (ReadSwitchInfo(False,Info)) Then
         UpdateSwitchInfo(Info)
      Else
         ShowMessage('Error reading the switch information!');
   End
   Else
      ShowMessage('Error setting the switch flip flop(s)!');
end;

{-----------------------------------------------------------------
  Toggle the state of FlipFlop Channel B
}
procedure TSwitchForm.ChannelBFFButtonClick(Sender: TObject);
Var
   ffstate: SmallInt;
   Info : SmallInt;
begin
   ffstate := 0;
   { create the byte to write to the flip flop }
   If (ChannelBFFButton.Caption = 'ON') Then
      ffstate := ffstate Or $02;
   If (ChannelAFFButton.Caption = 'OFF') Then
      ffstate := ffstate Or $01;
   ffstate := (ffstate shl 5) Or $1F;

   { set the flip flop state and the read it back }
   If (SetFlipFlop(ffstate)) Then
   Begin
      If (ReadSwitchInfo(False,Info)) Then
         UpdateSwitchInfo(Info)
      Else
         ShowMessage('Error reading the switch information!');
   End
   Else
      ShowMessage('Error setting the switch flip flop(s)!');
end;

end.
