<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberBankTransactionReport.aspx.cs"
    Inherits="CCMSUI.CCMS.MemberBankTransactionReport" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS - Member Bank Transaction Report</title>
    <script src="../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/isuite.css" rel="stylesheet" />
  
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>

        <link href="../content/DatePicker/jquery-ui.css" rel="stylesheet" />
    <script src="../content/DatePicker/jquery-3.7.1.js"></script>
    <script src="../content/DatePicker/jquery-ui.js"></script>

<script>
    $(function () {
        $(".datepicker").datepicker({
            dateFormat: "dd/mm/yy",
            showOn: "button",
            buttonImage: "../images/calendar.gif",
            buttonImageOnly: true,
            buttonText: "Select date",
            showButtonPanel: true
        });
    });
</script>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Member Bank Transaction Report
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
                                            <asp:TextBox runat="server" ID="TextBox_FromDate"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            TSN(From) :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_TSN" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_TSN" ErrorMessage="TSN(From) is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td>
                                            TSN(To) :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToTSN" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_ToTSN" ErrorMessage="TSN(To) is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 151px" valign="top">
                                            FIT :
                                        </td>
                                        <td align="left" colspan="5">
                                            <div style="overflow: auto; height: 200px; width: 450px">
                                                <asp:CheckBoxList ID="CheckBoxList_FIT" runat="server" RepeatColumns="7">
                                                </asp:CheckBoxList>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Status:
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:CheckBoxList ID="CheckBoxList_Status" runat="server" CssClass="dropDown" RepeatColumns="3"
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem Value="0" Selected="True">Successful</asp:ListItem>
                                                <asp:ListItem Value="2" Selected="True">Suspicious</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Failed</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 151px" valign="top">
                                            Transaction Type :
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:CheckBoxList ID="TrxTypeList" runat="server" RepeatColumns="3">
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
            <%--                        <Summary>
            Code edited by Ali Faiyaz Shah on 4th Sep, 15   12:35
            To include "Filter by comment" feature while generating MemberBankTransactionReport--%>
                                    <tr>
                                        <td class="style4">
                                            Comment:
                                        </td>
                                        <td style="width: 190px; height: 17px" colspan="3">
                                            <asp:DropDownList ID="DropDownList_Comment" runat="server" AppendDataBoundItems="true"
                                                CssClass="dropDown">
                                                <asp:ListItem>Any</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <%--</Summary> --%>
                                    <tr style="padding-top: 5px">
                                        <td>
                                        </td>
                                        <td>
                                            <input type="button" id="button_checkAll" value="Check All" class="button" onclick="setCheck(true)" />
                                            <input type="button" id="button_uncheckAll" value="Uncheck All" class="button" onclick="setCheck(false)" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="6" align="right" style="padding-top: 20PX;">
                                            <asp:Button ID="btnSearch" runat="server" Width="130px" Text="Generate Report" CssClass="button"
                                                OnClick="btnSearch_Click"></asp:Button>&nbsp;
                                                <asp:Button ID="Button_GenerateReportSchedule" runat="server" Width="160px" Text="Generate Report Schedule" CssClass="button"
                                                OnClick="Button_GenerateReportSchedule_Click" OnClientClick="return confirm('Are you sure you want to delete existing schedule and add new schedule for this report?');"></asp:Button>&nbsp;
                                            <input id="ButtonCancel" class="button" onclick="window.location='reportList.aspx'"
                                                style="width: 52px" type="button" value="Close" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top">
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
    </form>
</body>
</html>
