program swdl32;

uses
  Forms,
  Swun32 in 'Swun32.pas' {SwitchForm},
  iBTMEXPW in 'iBTMEXPW.pas';

{$R *.RES}

begin
  Application.Initialize;
  Application.CreateForm(TSwitchForm, SwitchForm);
  Application.Run;
end.
