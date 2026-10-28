up<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ATMSummaryReport.aspx.cs"
    Inherits="CCMSUI.CCMS.ATMSummaryReport" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CCMS - ATM Summary Report</title>
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
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    ATM Summary Report
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
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
                                        <td>
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" />
                                           
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                    ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" />
                                          
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                     ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 20px">
                                            Created by :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_creator" runat="server" Width="150px" CssClass="textbox">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="height: 20px">
                                            Status :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_status" runat="server" Style="width: 150px" CssClass="textbox">
                                                <asp:ListItem Selected="True" Value="1">Active</asp:ListItem>
                                                <asp:ListItem Value="0">InActive</asp:ListItem>
                                                <asp:ListItem Value="0,1">All</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            ATM Type :&nbsp;
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_ATMType" runat="server" Width="151px" CssClass="textbox">
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
                                            <div style=" overflow:auto">
                                                    <asp:CheckBoxList ID="groupList"  runat="server" RepeatDirection="Horizontal"
                                                        RepeatColumns="4" required>
                                                    </asp:CheckBoxList>
                                                </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Generate Report" Width="130px" />
                                            &nbsp;
                                            <asp:Button ID="Button_GenerateReportSchedule" runat="server" Width="160px" Text="Generate Report Schedule" CssClass="button"
                                                OnClick="Button_GenerateReportSchedule_Click" OnClientClick="return confirm('Are you sure you want to delete existing schedule and add new schedule for this report?');"></asp:Button>&nbsp;
                                            <asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                                Width="71px" OnClick="Button_reset_Click" />&nbsp;
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
