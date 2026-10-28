<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditATMClassification.aspx.cs"
    Inherits="CCMSUI.admin.AddEditATMClassification" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit ATM Classification</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["file_type_id"] == null ? "Add a New File Type" : "Edit File Type"%>
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
                <table id="inner" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px">
                    <tr>
                        <td>
                            Classification Name
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_title" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_title" runat="server" ControlToValidate="TextBox_title"
                                ErrorMessage="Please enter name for ATM Classification.">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                   
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateClassification" runat="server" Text="Save" CssClass="button"
                                Width="60px" OnClick="Button_SaveUpdateClassification_Click" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='Classification.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <br />
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
