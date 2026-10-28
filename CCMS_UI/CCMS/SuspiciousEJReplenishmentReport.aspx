<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SuspiciousEJReplenishmentReport.aspx.cs" Inherits="CCMSUI.CCMS.SuspiciousEJReplenishmentReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Suspicious Replenishment Report</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
   <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <link href="../content/Scripts/ui-lightness/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.8.17.custom.css" rel="stylesheet"
        type="text/css" />
    <link href="../content/Scripts/ui-lightness/ui-time-picker.css" rel="stylesheet"
        type="text/css" />

    <script type="text/javascript" src="../content/js/jquery.js"></script>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script src="../content/Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.datepicker.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery-ui-timepicker-addon.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datetimepicker({
                dateFormat: 'dd/mm/yy',
                timeFormat: 'hh:mm:ss',
                defaultDate: -1,
                minDate: '-2m',
                maxDate: '+2d',
                hourGrid: 4,
                minuteGrid: 10,

                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });

    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server" autocomplete="off">
    <div>
        <table cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    EJ Replenishment Report
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" colspan="2">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="False"
                        ShowSummary="True" CssClass="ValidationText"></asp:ValidationSummary>
                </td>
            </tr>
            <tr style="height: 20px">
                <td align="center" colspan="2">
                    <div class="ValidationText">
                        <asp:Literal ID="Literal_errorMsg" runat="server"></asp:Literal></div>
                    <div class="ConfirmationText">
                        <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></div>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="90%" border="0">
                                    <tr>
                                        <td style="width: 120px; height: 17px">
                                            From Date :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="datepicker" />
                                           
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                               ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                  ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 120px; height: 17px">
                                            &nbsp;To Date :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="datepicker" />
                                          
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                               ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                 ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 99px; height: 17px">
                                            Status :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:DropDownList ID="DropDownList_status" runat="server" EnableViewState="true"
                                                CssClass="textbox" Width="144px">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            Group:
                                        </td>
                                          <td colspan="8">
                                                <%--   <asp:DropDownList ID="DropDownList_NoteSetTypeID" runat="server" CssClass="textbox"
                                                AppendDataBoundItems="true" Width="125px">
                                                <asp:ListItem>Select</asp:ListItem>
                                            </asp:DropDownList>--%>
                                                <div style="overflow: auto; background-color:InfoBackground">
                                                    <asp:CheckBoxList ID="groupList" runat="server" RepeatDirection="Horizontal"
                                                        RepeatColumns="4" required>
                                                    </asp:CheckBoxList>
                                                </div>
                                            </td>
                                    </tr>

                                     <tr style="padding-top: 5px"  id="reportType" runat="server">
                                        <td>
                                            Report Type :
                                        </td>
                                        <td >
                                            <asp:RadioButtonList RepeatDirection="Horizontal" ID="RadioButtonList_Replenishment"
                                                runat="server" AutoPostBack="true" Width="250px" >
                                                <asp:ListItem Text="Detail Report" Selected="True"></asp:ListItem>
                                                 <asp:ListItem Text="Summary Report "></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        </tr>

                                    <tr style="padding-top: 5px">
                                    </tr>
                                    <tr>
                                        <td colspan="6" align="right" style="padding-top: 20PX;">
                                            <asp:Button ID="btnSearch" runat="server" Width="130px" Text="Generate Report" CssClass="button"
                                                OnClick="btnSearch_Click"></asp:Button>&nbsp;
                                            <asp:Button ID="Button_GenerateReportSchedule" runat="server" Width="160px" Text="Generate Report Schedule"
                                                CssClass="button" OnClick="Button_GenerateReportSchedule_Click" OnClientClick="return confirm('Are you sure you want to delete existing schedule and add new schedule for this report?');"></asp:Button>&nbsp;
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
                                            <img src="../images/Icon_search.png" />
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
