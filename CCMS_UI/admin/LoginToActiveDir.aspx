<%@ Page Language="C#" AutoEventWireup="true" Codebehind="LoginToActiveDir.aspx.cs"
    Inherits="CCMSUI.LoginToActiveDir" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login to Active Directory</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onload="setPageURL('admin/LoginToActiveDir.aspx')">
    <form id="form1" runat="server">
        <div>
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <th align="left" colspan="2" style="height: 20px">
                        Domain And Credentials for Active Directory</th>
                </tr>
                <tr>
                    <td >
                        &nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center; color:Red">
                        <asp:Literal ID="Literal_Msg" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center">
                        <asp:ValidationSummary ID="ValidationSummary_LoginPage" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 145px">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td style="width: 145px">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td style="width: 145px">
                        &nbsp;Domain :
                    </td>
                    <td>
                        <asp:TextBox ID="TextBox_Domain" runat="server" CssClass="textbox" Width="196px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_Domain"
                         ErrorMessage="Please enter Domain" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td style="width: 145px">
                        &nbsp;User Name :
                    </td>
                    <td>
                        <asp:TextBox ID="TextBox_UserName" runat="server" CssClass="textbox" Width="196px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator_Username" runat="server" ControlToValidate="TextBox_UserName"
                         ErrorMessage="Please enter Login ID" SetFocusOnError="true">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td style="width: 145px">
                        &nbsp;Password :
                    </td>
                    <td>
                        <asp:TextBox ID="TextBox_Password" runat="server" CssClass="textbox" TextMode="Password"
                            Width="196px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator_Password" runat="server"
                         ControlToValidate="TextBox_Password"  SetFocusOnError="true" ErrorMessage ="Please enter Password">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td style="width: 145px">
                        &nbsp;Search Name(Optional) :
                    </td>
                    <td>
                        <asp:TextBox ID="TextBox_Search" runat="server" CssClass="textbox" Width="196px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 145px">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td style="width: 145px">
                    </td>
                    <td>
                        <asp:Button ID="Button_Next" runat="server" CssClass="button" OnClick="Button_Next_Click"
                            Text="Login" Width="56px" />
                        <input type="button" value="Close" class="button" onclick="javascript:history.go(-1);" />                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
