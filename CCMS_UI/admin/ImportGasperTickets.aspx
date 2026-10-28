<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportGasperTickets.aspx.cs"
    Inherits="CCMSUI.admin.ImportGasperTickets" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>Import FLM Records</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" type="text/javascript">
        function setCheckbox(selected) {
            var inputs = document.getElementsByTagName('input');

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox")
                    inputs[i].checked = selected;
            }
        }
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    Import Gasper Tickets
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center" colspan="2">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center" colspan="2">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="inner" border="0" cellpadding="2" cellspacing="1" width="100%">
                        
                       
                        <tr>
                            <td>
                                Excel Worksheet Name
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_SheetName" runat="server" Text="OOS Tickets" CssClass="textbox"></asp:TextBox>
                            </td>
                        </tr>
                      

                        <tr>
                            <td>
                                File Path *
                            </td>
                            <td>
                                <asp:FileUpload ID="FileUpload_imac" runat="server" CssClass="textbox" Width="375px" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator_imac" runat="server"
                                    ControlToValidate="FileUpload_imac" ValidationExpression=".*\.xlsx?">Only an xls file is allowed</asp:RegularExpressionValidator>
                                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator_imac" runat="server" ControlToValidate="FileUpload_imac">Please specify a file to upload</asp:RequiredFieldValidator>--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Button ID="Button_Submit" runat="server" Text="Upload" CssClass="button" OnClick="Button_Submit_Click"
                                    Width="100px" CausesValidation="true" />
                                <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location='administration.aspx'"
                                    class="button" style="width: 100px" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    
    </form>
</body>
</html>
