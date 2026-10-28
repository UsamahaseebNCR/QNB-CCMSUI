<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallOutList.aspx.cs" Inherits="CCMSUI.CCMS.CallOutList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript"></script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Call Out Report List
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
                                        <td style="width: 120px; height: 17px">
                                            From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="155px" CssClass="textbox" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="155px" CssClass="textbox" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG2" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            Display Columns
                                        </td>
                                        <td colspan="3">
                                            <asp:CheckBoxList ID="CheckBoxList_DisplayColumns" runat="server" RepeatDirection="Horizontal"
                                                RepeatColumns="5">
                                                <asp:ListItem Text="Activity Datetime" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Ticket Number" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Log" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Type Of Call" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Reported By" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="ATM ID" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Location" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Type Of Problem" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Code" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Info Passed To" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Time Of Info" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Time Of Reaching Site" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Time Of Solving" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Attended By" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Reject Receipt Counter Type 1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Reject Receipt Counter Type 2" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Reject Receipt Counter Type 3" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Physical Reject Counter Type 1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Physcial Reject Counter Type 2" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Physcial Reject Counter Type 3" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Actual Problem" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Action Taken" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="BNA Jammed Counter" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="TE Bag No. & Count" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Time Of Call To Bank" Selected="True"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="btnSearch_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                                Width="71px" OnClick="Button_reset_Click" />&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top">
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
            <tr>
                <td align="left">
                    &nbsp; &nbsp; &nbsp;&nbsp;
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Call Outs -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2" style="text-align: center">
                    <asp:GridView ID="GridView_CallOut" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                        OnSorting="GridView_CallOut_Sorting" Width="100%" AllowPaging="True" OnPageIndexChanging="GridView_CallOut_PageIndexChanging"
                        HeaderStyle-ForeColor="Black" PageSize="25" OnRowDataBound="GridView_CallOut_RowDataBound">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="activity_datetime" HeaderText="Activity Datetime" SortExpression="activity_datetime"
                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="ticket_number" HeaderText="Ticket No." SortExpression="ticket_number"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="log" HeaderText="Log" SortExpression="log" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="type_of_Call" HeaderText="Type Of Call" SortExpression="type_of_Call"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="call_reported_by" HeaderText="Reported By" SortExpression="call_reported_by"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="atm_id_no" HeaderText="ATM ID No." HeaderStyle-CssClass="HeadingCell"
                                SortExpression="atm_id_no" />
                            <asp:BoundField DataField="atm_location" HeaderText="Location" SortExpression="atm_location"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="type_of_problem" HeaderText="Problem" SortExpression="type_of_problem"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="code" HeaderText="Code" SortExpression="code" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="info_passed_to" HeaderText="Passed To" SortExpression="info_passed_to"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="time_of_info" HeaderText="Time Of Info" SortExpression="time_of_info"
                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="time_of_reaching_site" HeaderText="Time Of Reaching Site"
                                SortExpression="time_of_reaching_site" HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="time_of_solving" HeaderText="Time Of Solving" SortExpression="time_of_solving"
                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="attended_by" HeaderText="Attended By" SortExpression="attended_by"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="reject_receipt_counter_type1" HeaderText="Reject 1" SortExpression="reject_receipt_counter_type1"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="reject_receipt_counter_type2" HeaderText="Reject 2" SortExpression="reject_receipt_counter_type2"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="reject_receipt_counter_type3" HeaderText="Reject 3" SortExpression="reject_receipt_counter_type3"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="physical_reject_counter_type1" HeaderText="Physial Reject 1"
                                SortExpression="physical_reject_counter_type1" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="physical_reject_counter_type2" HeaderText="Physial Reject 2"
                                SortExpression="physical_reject_counter_type2" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="physical_reject_counter_type3" HeaderText="Physial Reject 3"
                                SortExpression="physical_reject_counter_type3" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="actual_problem_found" HeaderText="Actual Problem" SortExpression="actual_problem_found"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="action_taken" HeaderText="Action Taken" SortExpression="action_taken"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="bna_jammed_counter" HeaderText="BNA Jammed Counter" SortExpression="bna_jammed_counter"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="te_bag_no_and_count" HeaderText="TE Bag No. & Count" SortExpression="te_bag_no_and_count"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="time_of_call_to_bank" HeaderText="Time Of Call To Bank"
                                SortExpression="time_of_call_to_bank" HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        </Columns>
                        <EmptyDataTemplate>
                            No Record(s) Found
                        </EmptyDataTemplate>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
