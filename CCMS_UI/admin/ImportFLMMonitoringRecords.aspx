<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportFLMMonitoringRecords.aspx.cs" Inherits="CCMSUI.admin.ImportFLMMonitoringRecords" %>

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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    Import Call Out Report By NCR
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
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                File Path *
                            </td>
                            <td>
                                <asp:FileUpload ID="FileUpload_imac" runat="server" CssClass="textbox" Width="375px" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator_imac" runat="server"
                                    ControlToValidate="FileUpload_imac" ValidationExpression=".*\.xls">Only an xls file is allowed</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_imac" runat="server" ControlToValidate="FileUpload_imac">Please specify a file to upload</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                Columns
                            </td>
                            <td>
                                <asp:CheckBoxList ID="CheckBoxList_columns" runat="server" CssClass="textbox" Width="100%"
                                    AutoPostBack="false" RepeatColumns="5" RepeatDirection="Horizontal" RepeatLayout="Table">
                                    <%--<asp:ListItem Text="Date" Value="Date" Selected="True"></asp:ListItem>--%>
                                    <asp:ListItem Text="Log" Value="log" Selected="True"></asp:ListItem>
                                    <%--<asp:ListItem Text="Time" Value="time" Selected="True"></asp:ListItem>--%>
                                    <asp:ListItem Text="Call Reported By" Value="Call Reported By" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="NCR Ticket Number" Value="NCR TICKET NUMBER" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Type Of Call" Value="Type Of Call" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="ATM ID No." Value="ATM ID No#" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="ATM Location" Value="ATM LOCATION" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Type Of Problem" Value="TYPE OF PROBLEM" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Code" Value="CODE" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Info Passed To" Value="INFO PASSED TO" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Time Of Info" Value="Time Of Info" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Time Of Reaching Site" Value="Time Of Reaching Site" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Time Of Solving" Value="Time Of Solving" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Attended By" Value="Attended By" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Reject Receipt Counter Type 1" Value="Reject Receipt Counter Type 1"
                                        Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Reject Receipt Counter Type 2" Value="Reject Receipt Counter Type 2"
                                        Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Reject Receipt Counter Type 3" Value="Reject Receipt Counter Type 3"
                                        Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Physical Reject Counter Type 1" Value="Physical Reject Counter Type 1"
                                        Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Physical Reject Counter Type 2" Value="Physical Reject Counter Type 2"
                                        Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Physical Reject Counter Type 3" Value="Physical Reject Counter Type 3"
                                        Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Actual Problem Found" Value="Actual Problem Found" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Action Taken" Value="Action Taken" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="BNA(CDM)Jammed Counter" Value="BNA(CDM)Jammed Counter" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="TE.Bag No. & Count" Value="TE#BAG NUMBER & COUNT" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Time Of Call To Bank" Value="Time Of Call To Bank" Selected="True"></asp:ListItem>
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td align="right">
                                <input id="Button_CheckAll" type="button" value="Check All" onclick="setCheckbox(true)"
                                    class="button" style="width: 90px" />
                                <input id="Button_UncheckAll" type="button" value="Uncheck All" onclick="setCheckbox(false)"
                                    class="button" style="width: 90px" />
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
