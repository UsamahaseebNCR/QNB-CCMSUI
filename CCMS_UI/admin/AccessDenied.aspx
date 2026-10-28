<%@ Page Language="C#" AutoEventWireup="true" Codebehind="AccessDenied.aspx.cs" Inherits="Avanza.iSuite.UI.Info" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table width="100%">
                <tr>
                    <td style="text-align: center; font-size: larger; font-family: Arial">
                        <img src="../images/error.gif" id="image_error" alt=""/>
                         <asp:Literal ID="Literal_Information" runat="server" Text="You are not authorized to view this page"></asp:Literal>
                    </td>
                </tr>
                <tr>
                </tr>
                <tr>
                </tr>
                <tr>
                    <td>
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <a href="../Home.aspx">Go To Home Page</a>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
