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
    TempSense : This utility uses TMEX to read and view temperatures from
                DS1920/DS1820 devices. This is a multi-threaded program in that
                an external thread is created to perform 1-Wire communication during
                program startup.  This frees the main GUI thread to perform updates
                on Windows/mouse events and keeps the app from "freezing" when
                during calls to "Sleep".  It requires the 32-Bit Windows TMEX drivers
                to be present.
    Compiler : Borland Delphi 5.0
                                        }

unit TempSenseUnit;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs, SensorThreadUnit,
  StdCtrls, iBTMEXPW;

type
  TTempSenseForm = class(TForm)
    ResultsMemo: TMemo;
    procedure FormCreate(Sender: TObject);

  private
    { Private declarations }
    procedure SensorReadingDone(var AMessage : TMessage); message WM_SensorReadingDone; // Message to be sent back from thread when its done

  public
    { Public declarations }
    MySensorThread: TSensorThread;   { Thread to do 1-Wire }
  end;

var
  TempSenseForm: TTempSenseForm;

implementation

{$R *.DFM}


{----------------------------------------------------------------------------}
{ Receives a message from MySensorThread that a reading has been completed,
{ so get and plot the reading.
}
procedure TTempSenseForm.SensorReadingDone(var AMessage: TMessage); // update form
var
   DegreesC:  Real;
   TempString:  String;
   RomNumString: String;
Begin
   TempString := '';
   RomNumString := '';
   If ((MySensorThread <> nil) And (MySensorThread.ThreadID = cardinal(AMessage.WParam))) Then
   Begin
      DegreesC := MySensorThread.getTemperature();
      TempString := FormatFloat('0.00',DegreesC) + '°C';
      RomNumString := MySensorThread.getRomNumber();
      ResultsMemo.Lines.Add(RomNumString + '   ' + TempString);
   End;
End;

{----------------------------------------------------------------------------}
{ The FormCreate even also creates/initializes/starts 1-Wire thread.
}
procedure TTempSenseForm.FormCreate(Sender: TObject);
var
   RetValue: Integer;
begin
   { Create Sensor Thread here }
   MySensorThread := TSensorThread.Create(True); { Start Thread initially suspended }
   MySensorThread.ParentFormHandle := WindowHandle; { Pass Handle to thread }
   MySensorThread.DidSetup := false;
   MySensorThread.FreeOnTerminate := true;
   { Get default port num and port type and put them in thread variables}
   RetValue := TMReadDefaultPort(@MySensorThread.PortNum, @MySensorThread.PortType);
   if (RetValue  < 1) then
   begin
      MySensorThread.PortNum :=1;
      MySensorThread.PortType :=6;
   end;
   MySensorThread.Resume;  { Resume thread from initial suspended state }

end;

End.
