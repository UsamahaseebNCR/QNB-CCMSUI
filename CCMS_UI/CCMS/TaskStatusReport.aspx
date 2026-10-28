<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskStatusReport.aspx.cs"
    Inherits="CCMSUI.CCMS.TaskStatusReport" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Task Status Report</title>
    <link href="../content/iSuite.css" rel="stylesheet" />

   
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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server" autocomplete="off">
    <div>
        <table cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    Task Status Report
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
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td style="width: 120px; height: 17px">
                                            From(Creation Time) :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="datepicker" />
                                            
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                            ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                             ControlToValidate="TextBox_FromDate" ErrorMessage="Creation Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To(Creation Time) :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="datepicker" />
                                         
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                             ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                            ControlToValidate="TextBox_ToDate" ErrorMessage="Creation Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px; height: 17px">
                                            From(End Time) :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_EndTimeFromDate" Width="119px" CssClass="datepicker" />
                                           
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_EndTimeFromDate" ErrorMessage="End Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To(End Time) :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_EndTimeToDate" Width="119px" CssClass="datepicker" />
                                           
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_EndTimeToDate" ErrorMessage="End Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr id="trFileTypes" runat="server">
                                        <td>
                                            Task Types :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="Dropdownlist_taskType" runat="server" EnableViewState="true"
                                                AutoPostBack="true" OnSelectedIndexChanged="Dropdownlist_taskType_SelectedIndexChanged"
                                                CssClass="textbox" Width="144px">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            User :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="Dropdownlist_user" runat="server" Width="144px" CssClass="textbox">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Task Status :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_status" runat="server" EnableViewState="true"
                                                CssClass="textbox" Width="144px">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            ATM Type :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_ATMType" runat="server" Width="144px" CssClass="textbox">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Group:
                                        </td>
                                        <td colspan="8">
                                         <%--   <asp:DropDownList ID="DropDownList_NoteSetTypeID" runat="server" CssClass="textbox"
                                                AppendDataBoundItems="true" Width="125px">
                                                <asp:ListItem>Select</asp:ListItem>
                                            </asp:DropDownList>--%>
                                            <div style=" overflow:auto">
                                                    <asp:CheckBoxList ID="groupList"  runat="server" RepeatDirection="Horizontal"
                                                        RepeatColumns="4" required>
                                                    </asp:CheckBoxList>
                                                </div>
                                        </td>
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
