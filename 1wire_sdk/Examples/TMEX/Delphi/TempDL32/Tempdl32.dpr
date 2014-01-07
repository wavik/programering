program Tempdl32;

uses
  Forms,
  Tempmain in 'TEMPMAIN.PAS' {Form1},
  iBTMEXPW in 'iBTMEXPW.pas';

{$R *.RES}

begin
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.
