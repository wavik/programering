program TempSense;

uses
  Forms,
  TempSenseUnit in 'TempSenseUnit.pas' {TempSenseForm},
  SensorThreadUnit in 'SensorThreadUnit.pas',
  iBTMEXPW in 'Ibtmexpw.pas';

{$R *.RES}

begin
  Application.Initialize;
  Application.CreateForm(TTempSenseForm, TempSenseForm);
  Application.Run;
end.
