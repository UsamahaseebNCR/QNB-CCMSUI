<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditFITMapping.aspx.cs"
    Inherits="CCMSUI.admin.EditFITMapping" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Edit FIT Mapping</title>
    <link rel="stylesheet" href="../content/iSuite.css" />
    <script type="text/javascript">
        window.parent.window.contentPage = window.location;           
    </script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
        <tr>
            <td class="PageHeading" align="left" colspan="2">
                &nbsp;Edit FIT Mapping
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" style="height: 15px" colspan="2">
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="ValidationText" align="center" colspan="2" style="height: 20px">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                <asp:ValidationSummary ID="ValidationSummary_err" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center" class="ValidationText" style="height: 13px; width: 82px;">
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px; width: 72px; height: 212px;" valign="top">
                FIT :
            </td>
            <td style="height: 212px; width: 860px;">
                <div style="overflow: scroll; height: 200px; width: 800px">
                    <asp:CheckBoxList ID="CheckBoxList_FIT" runat="server" RepeatColumns="8">
                    </asp:CheckBoxList>
                </div>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px; height: 19px; width: 82px;">
                Bank Name :
            </td>
            <td style="height: 19px; width: 860px;">
                <asp:TextBox ID="TextBox_BankName" runat="server" CssClass="textbox">
                </asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator_BankName" runat="server" ControlToValidate="TextBox_BankName"
                    ErrorMessage="Please enter Bank name">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px; height: 19px; width: 82px;">
                On Us :
            </td>
            <td style="height: 19px; width: 860px;">
                <asp:CheckBox ID="CheckBox_OnUs" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="padding-left: 100px; height: 20px;" colspan="2">
                <asp:Button ID="Button_Save" runat="server" CssClass="button" OnClick="Button_Save_Click"
                    Text="Save" />&nbsp;
                <input type="button" class="button" value="Cancel" onclick="history.go(-1)" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
