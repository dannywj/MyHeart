<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MessagePub.aspx.cs" Inherits="MyHeart.Heart.MessagePub" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>PubNewMessage</h2>
        <p>
            <input type="text" value="2013/10/12" runat="server" id="txtDate" />
            <asp:DropDownList ID="ddlWriter" runat="server">
                <asp:ListItem Text="JueJue" Value="juejue" Selected="True"></asp:ListItem>
                <asp:ListItem Text="GeGe" Value="gege"></asp:ListItem>
            </asp:DropDownList>
        </p>
        <div>
            <textarea style="width: 452px; height: 220px" id="txtContent" runat="server"></textarea>
        </div>
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
            <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
        </p>
    </form>
</body>
</html>
