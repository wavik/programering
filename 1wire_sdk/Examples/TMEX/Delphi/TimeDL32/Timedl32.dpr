program timedl32;

uses
  Forms,
  Timeun32 in 'timeun32.pas' {TimeForm},
  iBTMEXPW in 'iBTMEXPW.pas';

{$R *.RES}

begin
  Application.Initialize;
  Application.CreateForm(TTimeForm, TimeForm);
  Application.Run;
end.
