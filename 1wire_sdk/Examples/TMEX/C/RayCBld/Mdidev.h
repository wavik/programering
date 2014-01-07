//---------------------------------------------------------------------------
#ifndef MdiDevicesH
#define MdiDevicesH
//---------------------------------------------------------------------------
#include <vcl\Classes.hpp>
#include <vcl\Controls.hpp>
#include <vcl\StdCtrls.hpp>
#include <vcl\Forms.hpp>
#include <vcl\ExtCtrls.hpp>
#include <vcl\Buttons.hpp>
#include <vcl\Menus.hpp>
#include <vcl\DB.hpp>
#include <vcl\Dialogs.hpp>
#include <vcl\DBCtrls.hpp>
#include <Graphics.hpp>
//---------------------------------------------------------------------------
struct RegNumInfo // Holds info of each object in the Device list
{   short ROM[8];
    int age;
};
//---------------------------------------------------------------------------
class TDeviceList : public TForm
{
__published:	// IDE-managed Components
    TPanel *Panel1;
    TPanel *Panel2;
    TLabel *Logo;
	TLabel *Label1; // Main version info
	TLabel *Label2; // Hardware specific version info
	TLabel *Label3;
    TButton *ExitButton;
    TButton *SelectButton;
    TListBox *RegList;
    TTimer *Timer1;
	TImage *Image1;
    void __fastcall ExitButtonClick(TObject *Sender);
    void __fastcall SelectButtonClick(TObject *Sender);
    void __fastcall Timer1Timer(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall OnDblClick(TObject *Sender);

	
private:    // User declarations
public:	    // User declarations
    __fastcall TDeviceList(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern TDeviceList *DeviceList;
//---------------------------------------------------------------------------
#endif
