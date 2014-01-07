//---------------------------------------------------------------------------
#ifndef MdiDataH
#define MdiDataH
//---------------------------------------------------------------------------
#include <vcl\Classes.hpp>
#include <vcl\Controls.hpp>
#include <vcl\StdCtrls.hpp>
#include <vcl\Forms.hpp>
#include <vcl\ExtCtrls.hpp>
#include <Graphics.hpp>
//---------------------------------------------------------------------------
class TFileData : public TForm
{
__published:	// IDE-managed Components
    TPanel *Panel1;
    TButton *CancelButton;
    TButton *AcceptButton;
    TLabel *iButton;
    TPanel *Panel2;
    TMemo *Data;
	TImage *Image1;
    void __fastcall CancelButtonClick(TObject *Sender);
    void __fastcall AcceptButtonClick(TObject *Sender);
private:    // User declarations
    char* __fastcall WriteError(const int flag);
public:	    // User declarations
    __fastcall TFileData(TComponent* Owner);
    void __fastcall TFileData::EditFileData();
    void __fastcall TFileData::ReplaceFile();
};
//---------------------------------------------------------------------------
extern TFileData *FileData;
//---------------------------------------------------------------------------
#endif
