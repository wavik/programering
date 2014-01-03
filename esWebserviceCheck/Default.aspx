<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Kortdata:<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox><asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        <hr />
        CardRef:<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
        <br />
        ArtName:<asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
        <br />
        CardHist:<br />
        <asp:ListBox ID="ListBox1" runat="server"></asp:ListBox>
    </div>
    </form>
</body>
</html>
