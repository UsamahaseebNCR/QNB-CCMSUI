<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ScheduleTrackingReport.aspx.cs"
    Inherits="CCMSUI.CCMS.ScheduleTrackingReport" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Schedule Tracking Report</title>
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

            $('.datepicker').datetimepicker({ dateFormat: 'dd/mm/yy',

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

    <script type="text/javascript" language="javascript">
        function validate()
        {
         
         for (i =0;i<document.getElementById('ListBox_ScheduleTime').options.length;i++)
         {
             if (document.getElementById('ListBox_ScheduleTime').options[i].selected)
                 return true;
                
                
         }
          document.getElementById('divMsg').innerHTML = 'Please select schedules time.';
          return false;
        }
    
    </script>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server" autocomplete="off">
        <div>
            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                <tr>
                    <td class="PageHeading" align="left" colspan="2">
                        Schedule Tracking Report</td>
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
                        <div class="ValidationText" id="divMsg">
                            <asp:Literal ID="Literal_errorMsg" runat="server"></asp:Literal></div>
                        <div class="ConfirmationText">
                            <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></div>
                    </td>
                </tr>
                <tr align="left" style="background-color: WHITE">
                    <td style="height: 20px; padding-left: 5px;" colspan="2">
                        <b>Search Filters</b></td>
                </tr>
                <tr align="left">
                    <td colspan="2">
                        <table width="100%" class="SEARCHBOX" border="0">
                            <tr>
                                <td>
                                    <table id="search" width="100%" border="0">
                                        <tr>
                                            <td style=" height: 17px">
                                                From Date :
                                            </td>
                                            <td style=" height: 17px">
                                                <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="datepicker" />
                                               
                                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                   ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                 ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style=" height: 17px">
                                                To Date :
                                            </td>
                                            <td style="height: 17px">
                                                <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="datepicker" />
                                               
                                                <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                   ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                 ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td  valign="top">
                                                Schedule Time :
                                            </td>
                                            <td valign="top" colspan="3">
                                                <div style="height: 150px; overflow:auto">
                                                    <asp:CheckBoxList ID="schedulesList" runat="server" RepeatDirection="Horizontal"
                                                        RepeatColumns="8">
                                                    </asp:CheckBoxList>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" align="right" style="padding-top: 10px;">
                                                <div>
                                                    <input type="button" id="button_checkAll" value="Check All" class="button" onclick="setCheck(true)"
                                                        style="width: 70px" />
                                                    &nbsp;<input type="button" id="button_uncheckAll" value="Uncheck All" class="button"
                                                        onclick="setCheck(false)" style="width: 90px" />
                                                    <asp:Button ID="btnSearch" runat="server" Width="130px" Text="Generate Report" CssClass="button"
                                                        OnClick="btnSearch_Click"></asp:Button>&nbsp;
                                                             <asp:Button ID="Button_GenerateReportSchedule" runat="server" Width="160px" Text="Generate Report Schedule" CssClass="button"
                                                OnClick="Button_GenerateReportSchedule_Click" OnClientClick="return confirm('Are you sure you want to delete existing schedule and add new schedule for this report?');"></asp:Button>&nbsp;
                                                    <input id="ButtonCancel" class="button" onclick="window.location='reportList.aspx'"
                                                        style="width: 52px" type="button" value="Close" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td valign="top">
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
    </form>
</body>
</html>
