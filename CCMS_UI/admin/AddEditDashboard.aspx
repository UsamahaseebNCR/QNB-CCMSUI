<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditDashboard.aspx.cs" Inherits="CCMSUI.admin.AddEditDashboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit Template</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%" >
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["id"] == null?"Add New Template ":"Edit Template"%>
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
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
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width ="100%" style="padding-left:10px">
                    <tr style="background-color: #eff0ef">
                        <td style="width:15%">
                            Title <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Name" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Name" runat="server" ControlToValidate="TextBox_Name"
                                ErrorMessage="Please enter chart Name" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:15%">
                            Refresh Span(Seconds) <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Refresh" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Refresh" runat="server" ControlToValidate="TextBox_Refresh"
                                ErrorMessage="Please enter refresh span" Text="*"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                ControlToValidate="TextBox_Refresh" runat="server"
                                ErrorMessage="Only Numbers allowed"
                                Text="*"
                                ValidationExpression="\d+">
                            </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef">
                        <td>
                            Active
                        </td>
                        <td>
                            <asp:RadioButtonList ID="RadioButton_Active" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1">Active</asp:ListItem>
                                <asp:ListItem Value="0">Inactive</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateTemplate" runat="server" Text="Save" OnClick="Button_SaveUpdateTemplate_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location = 'dashboardtemplate.aspx'"
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