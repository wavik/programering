//---------------------------------------------------------------------------
#ifndef MdiFilesH
#define MdiFilesH
//---------------------------------------------------------------------------
#include <vcl\Classes.hpp>
#include <vcl\Controls.hpp>
#include <vcl\StdCtrls.hpp>
#include <vcl\Forms.hpp>
#include <vcl\ExtCtrls.hpp>
#include "iBTMEXCW.h"
#include <Graphics.hpp>
//---------------------------------------------------------------------------
//---------------------------------------------------------------------------
class TDirList : public TForm
{
__published:    // IDE-managed Components
    TPanel *Panel1;
    TButton *CancelButton;
    TButton *SelectButton;
    TLabel *Label1;
    TListBox *FileList;
	TImage *Image1;
    void __fastcall SelectButtonClick(TObject *Sender);
    void __fastcall OnDblClick(TObject *Sender);
    void __fastcall CancelButtonClick(TObject *Sender);
private:    // User declarations
    AnsiString __fastcall GetPathString(const DirectoryPath Path);
public:	    // User declarations
    __fastcall TDirList(TComponent* Owner);
    void __fastcall GetFileList();
};
//---------------------------------------------------------------------------
extern TDirList *DirList;
//---------------------------------------------------------------------------
#endif
