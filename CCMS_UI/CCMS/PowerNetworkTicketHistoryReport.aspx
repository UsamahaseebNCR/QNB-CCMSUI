<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PowerNetworkTicketHistoryReport.aspx.cs" Inherits="CCMSUI.CCMS.PowerNetworkTicketHistoryReport" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS - Power and Network Ticket History Report</title>
    <link href="../content/isuite.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript"></script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Power And Network Ticket History Report
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px; width: 935px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2" style="width: 100%">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td>
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="textboxNormal" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />
                                            <%--<asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>--%>
                                            <%--Below change is done to consider Time part also..Done by Ali Shah on 24 Feb, 16--%>
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                            ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d[ ](0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])"
                                            ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td>
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="textboxNormal" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG2" />
                                            <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>--%>

                                            <%--Below change is done to consider Time part also..Done by Ali Shah on 24 Feb, 16--%>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d[ ](0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        </tr>
                                       <%--<tr>
                                        <td>
                                            Only Powerdown Events : 
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="CheckBox_PowerOnly" runat="server" AutoPostBack="false" 
                                                oncheckedchanged="CheckBox_PowerOnly_CheckedChanged"/>
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td></td>
                                        <td style="padding-top: 20PX;">
                                            <asp:Button ID="btnSearch" runat="server" Width="130px" Text="Generate Report" CssClass="button"
                                                OnClick="btnSearch_Click"></asp:Button>&nbsp;
                                            <input id="ButtonCancel" class="button" onclick="window.location='reportList.aspx'"
                                                style="width: 52px" type="button" value="Close" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <img src="../images/Icon_search.png" alt="" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
