<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EJReplenishments.aspx.cs"
    Inherits="CCMSUI.CCMS.EJReplenishments" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
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
                    EJ Replenishments
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
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="155px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
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
                    Selected Cash Added -
                    <asp:Literal ID="Literal_CashAddedCount" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2" style="text-align: center">
                    <asp:GridView ID="GridView_CashAdded" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CashAdded_Sorting" Width="100%" AllowPaging="True"
                        OnPageIndexChanging="GridView_CashAdded_PageIndexChanging" HeaderStyle-ForeColor="Black"
                        PageSize="25">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="location" HeaderText="Location" SortExpression="location"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="rep_datetime" HeaderText="Replenished At" SortExpression="rep_datetime"
                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                            <asp:BoundField DataField="notes_added_type1" HeaderText="Type 1" SortExpression="notes_added_type1"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_added_type2" HeaderText="Type 2" SortExpression="notes_added_type2"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_added_type3" HeaderText="Type 3" SortExpression="notes_added_type3"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_added_type4" HeaderText="Type 4" SortExpression="notes_added_type4"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_added_type5" HeaderText="Type 5" SortExpression="notes_added_type5" Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_added_type6" HeaderText="Type 6" SortExpression="notes_added_type6" Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_added_type7" HeaderText="Type 7" SortExpression="notes_added_type7" Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="last_tsn" HeaderText="Last TSN" SortExpression="tsn" Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                        </Columns>
                        <EmptyDataTemplate>
                            No Record(s) Found
                        </EmptyDataTemplate>
                    </asp:GridView>
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Counts Cleared -
                    <asp:Literal ID="Literal_CountsClearedCount" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2" style="text-align: center">
                    <asp:GridView ID="GridView_CountsCleared" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CountsCleared_Sorting" Width="100%" AllowPaging="True"
                        OnPageIndexChanging="GridView_CountsCleared_PageIndexChanging" HeaderStyle-ForeColor="Black"
                        PageSize="25">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="location" HeaderText="Location" SortExpression="location"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="clearing_datetime" HeaderText="Counts Cleared At" SortExpression="clearing_datetime"
                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                            <asp:BoundField DataField="notes_dispensed_type1" HeaderText="Dispensed Type 1" SortExpression="notes_dispensed_type1"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_dispensed_type2" HeaderText="Dispensed Type 2" SortExpression="notes_dispensed_type2"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_dispensed_type3" HeaderText="Dispensed Type 3" SortExpression="notes_dispensed_type3"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_dispensed_type4" HeaderText="Dispensed Type 4" SortExpression="notes_dispensed_type4"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_dispensed_type5" HeaderText="Dispensed Type 5" SortExpression="notes_dispensed_type5"  Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_dispensed_type6" HeaderText="Dispensed Type 6" SortExpression="notes_dispensed_type6"  Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_dispensed_type7" HeaderText="Dispensed Type 7" SortExpression="notes_dispensed_type7"  Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_remaining_type1" HeaderText="Remaining Type 1" SortExpression="notes_remaining_type1"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_remaining_type2" HeaderText="Remaining Type 2" SortExpression="notes_remaining_type2"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_remaining_type3" HeaderText="Remaining Type 3" SortExpression="notes_remaining_type3"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_remaining_type4" HeaderText="Remaining Type 4" SortExpression="notes_remaining_type4"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_remaining_type5" HeaderText="Remaining Type 5" SortExpression="notes_remaining_type5"  Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_remaining_type6" HeaderText="Remaining Type 6" SortExpression="notes_remaining_type6"  Visible="false"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="notes_remaining_type7" HeaderText="Remaining Type 7" SortExpression="notes_remaining_type7"  Visible="false"
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
