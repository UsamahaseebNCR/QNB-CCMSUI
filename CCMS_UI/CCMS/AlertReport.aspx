<%@ Page Language="C#" AutoEventWireup="true" Codebehind="AlertReport.aspx.cs" Inherits="CCMSUI.CCMS.AlertReport"
Culture="fr-FR"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS - Alert Report</title>

    <script src="../content/boxover.js" type="text/javascript" language="javascript">
    </script>

    <link href="../content/isuite.css" rel="stylesheet" />
   
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script src="../content/DateTimePicker/jquery.min.js"></script>
    <link href="../content/DateTimePicker/jquery.datetimepicker.min.css" rel="stylesheet" />
    <script src="../content/DateTimePicker/jquery.datetimepicker.full.min.js"></script>


<script>
    $(function () {
        $(".datepicker").datetimepicker({
            format: 'd/m/Y H:i:s', // format for both date and time
            step: 15 // time step interval
        });
    });
</script>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                        Alert Monitoring Report</td>
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
                                                From :</td>
                                            <td>
                                                <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" />
                                             
                                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                      ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                 ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style="width: 99px; height: 17px">
                                                To :</td>
                                            <td>
                                                <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" />
                                             
                                                <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                        ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                               ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                Type :</td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="DropDownList_type" runat="server" CssClass="textboxNormal"
                                                    Width="280px" >
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr style="padding-top: 5px">
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
                                <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <img src="../images/Icon_search.png" alt="" /></td>
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
