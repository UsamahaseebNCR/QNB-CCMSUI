<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ATMReconciliation.aspx.cs" Inherits="CCMSUI.CCMS.ReconciliationMonitoring" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ATM Reconciliation</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    ATM Reconciliation
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
                                            From (Clearing Date Time):
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="155px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To (Clearing Date Time):
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="155px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
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
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="btnSearch_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
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
            <tr>
                <td align="left">
                    &nbsp; &nbsp; &nbsp;&nbsp;
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" align="left">
                    ATM Reconciliation -
                    <asp:Literal ID="Literal_CountsClearedCount" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2" style="text-align: center">
                    <asp:GridView ID="GridView_ReconciliationNotesDispensed" runat="server" AutoGenerateColumns="False" onrowdatabound="GridView_ReconciliationNotesDispensed_RowDataBound"
                        AllowSorting="true" OnSorting="GridView_ReconciliationNotesDispensed_Sorting" Width="100%" AllowPaging="True"
                        OnPageIndexChanging="GridView_ReconciliationNotesDispensed_PageIndexChanging" HeaderStyle-ForeColor="Black"
                        PageSize="25">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                       <Columns>
                            <asp:BoundField DataField="atm_title" HeaderText="ATM TITLE" SortExpression="atm_title" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="replishment_date" HeaderText="REPLISHMENT DATE" SortExpression="replishment_date"
                                HeaderStyle-CssClass="HeadingCell" />
                               <asp:BoundField DataField="clearing_date" HeaderText="CLEARING DATE" SortExpression="clearing_date" 
                                HeaderStyle-CssClass="HeadingCell" />
                              <asp:BoundField DataField="total_notes_dispensed" HeaderText="TOTAL AMOUNT DISPENSED" SortExpression="total_notes_dispensed"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="atm_dispensed" HeaderText="ATM DISPENSED" SortExpression="atm_dispensed"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="atm_remaining" HeaderText="ATM REMAINING" SortExpression="atm_remaining"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="switch_dispensed" HeaderText="SWITCH DISPENSED" SortExpression="switch_dispensed"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="switch_remaining" HeaderText="SWITCH REMAINING" SortExpression="switch_remaining"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="ATM_SWITCH_DISPENSED_DIFF" HeaderText="DIFF ATM & SWITCH DISPENSED" SortExpression="ATM_SWITCH_DISPENSED_DIFF"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="ATM_SWITCH_REMAINING_DIFF" HeaderText="DIFF ATM & SWITCH REMAINING" SortExpression="ATM_SWITCH_REMAINING_DIFF"  
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="host_dispensed" HeaderText="HOST DISPENSED" SortExpression="host_dispensed"  
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="host_remaining" HeaderText="HOST REMAINING" SortExpression="host_remaining"  
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="ATM_HOST_DISPENSED_DIFF" HeaderText="DIFF ATM & HOST DISPENSED" SortExpression="ATM_HOST_DISPENSED_DIFF"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="ATM_HOST_REMAINING_DIFF" HeaderText="DIFF ATM & HOST REMAINING" SortExpression="ATM_HOST_REMAINING_DIFF"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="SWITCH_HOST_DISPENSED_DIFF" HeaderText="DIFF SWITCH & HOST DISPENSED" SortExpression="SWITCH_HOST_DISPENSED_DIFF"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="SWITCH_HOST_REMAINING_DIFF" HeaderText="DIFF SWITCH & HOST REMAINING" SortExpression="SWITCH_HOST_REMAINING_DIFF"
                                HeaderStyle-CssClass="HeadingCell" />
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