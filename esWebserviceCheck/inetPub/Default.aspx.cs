using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WS;

public partial class _Default : System.Web.UI.Page 
{
    
        
protected void Page_Load(object sender, EventArgs e)
    {}

protected void Button1_Click(object sender, EventArgs e)
{
    WSLogin wsl = new WSLogin();
    ESBookingWebService ws = wsl.TalkToWS();
    //var artikelinfo = ws.LookupCard(TextBox1.Text);
    //TextBox2.Text=ws.
    TextBox2.Text = ws.DecryptCardData(TextBox1.Text).ToString();

    WS.CardHistory[] cardhist = ws.GetCardHistory(TextBox1.Text);
    //ESBookingWebService ws = new 


}
}
public class WSLogin
{
    public SecureHeader CreatSecureHeader()
    {
        SecureHeader sh = new SecureHeader();
        sh.Username = "entry";
        sh.Password = "yrtne";
        return sh;

    }

    public ESBookingWebService TalkToWS()
    {
        ESBookingWebService ws_temp = new ESBookingWebService();
        ws_temp.SecureHeaderValue = CreatSecureHeader();
        //ws_temp.Url = "http://www17.goteborg.se/gbgco/ESBookingWebService.asmx";
        ws_temp.Url = "http://148.160.19.213/esservice/ESBookingWebService.asmx";
        return ws_temp;
    }
}