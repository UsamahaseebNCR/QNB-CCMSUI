<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditGLAccounts.aspx.cs"
    Inherits="CCMSUI.admin.AddEditGLAccounts" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit GL Accounts</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["id"] == null?"Add a New GL Account ":"Edit GL Account"%>
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server"></asp:ValidationSummary>
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td>
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width="100%" style="padding-left: 10px">
                    <tr style="background-color: #eff0ef;">
                        <td style="width: 15%">
                            GL Account Name<span style="color:Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_GLAccountName" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator1" runat="server" ControlToValidate="TextBox_GLAccountName"
                                ErrorMessage="Please enter GL Account Name" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Opening Balance<span style="color:Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_OpeningBalance" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Title" runat="server" ControlToValidate="TextBox_OpeningBalance"
                                ErrorMessage="Please enter Opening Balance." Text="*"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator_OpeningBal" runat="server"
                                ControlToValidate="TextBox_OpeningBalance" ValidationExpression="\d*(\.\d{2})?" ErrorMessage="Invalid value entered for opening balance field"
                                Text="*">
                            </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef">
                        <td valign="top">
                            Comments
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Comments" runat="server" CssClass="textbox" TextMode="MultiLine" Width="300" Height="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateGLAccount" runat="server" Text="Save" OnClick="Button_SaveUpdateGLAccount_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='GLAccounts.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <br />
    </form>
</body>
</html>
