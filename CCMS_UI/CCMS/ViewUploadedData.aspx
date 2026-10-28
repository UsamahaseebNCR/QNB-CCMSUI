<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewUploadedData.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewUploadedData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - View Uplaoded Data</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">

        //window.parent.window.SetContentPage(window.location);

        function confirmATMDeletion(title) {
            return confirm('Are you sure you want to delete ATM ' + title + '?', 'iJournal');
        }</script>
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
                    View Uploaded Data
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
                                        <td style="height: 20px">
                                            Transaction Date (From)
                                        </td>
                                        <td style="height: 20px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                        </td>
                                        <td style="height: 20px">
                                            Transaction Date (To)
                                        </td>
                                        <td style="height: 20px">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            ATM ID
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_ATMID" runat="server" Width="148px" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                        <td>
                                            Card Number
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_PAN" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 20px">
                                            Customer Account No
                                        </td>
                                        <td style="height: 20px">
                                            <asp:TextBox ID="TextBox_AcctNo" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                        <td>
                                            Transaction Amount
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_Amount" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Transaction Sequence
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_TSN" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                        <td>
                                            Transaction Response
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_Response" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Source
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_Source" runat="server" CssClass="dropDown">
                                                <asp:ListItem Text="Host" Value="reconciliation_host_data"></asp:ListItem>
                                                <asp:ListItem Text="Switch" Value="reconciliation_switch_data"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align ="right">
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                                Width="71px" OnClick="Button_reset_Click" />&nbsp;
                                                <input type="button" value="Back" class="button" onclick="window.location='Reconciliation.aspx'" />
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
                    Selected Transactions -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_Trxn" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                        OnSorting="GridView_Trxn_Sorting" Width="100%" AllowPaging="True" OnPageIndexChanging="GridView_Trxn_PageIndexChanging"
                        PageSize="15">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="card_number" HeaderText="Card Number" SortExpression="card_number"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="customer_account_no" HeaderText="Account No." SortExpression="customer_account_no"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_date" HeaderText="Date" SortExpression="transaction_date"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_time" HeaderText="Time" SortExpression="transaction_time"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_amount" HeaderText="Amount" SortExpression="transaction_amount" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_currency" HeaderText="Currency" SortExpression="transaction_currency" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_sequence" HeaderText="Seq" SortExpression="transaction_sequence" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_type" HeaderText="Type" SortExpression="transaction_type" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_response" HeaderText="Response Code" SortExpression="transaction_response" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_settlement_date" HeaderText="Settlement Date" HeaderStyle-ForeColor="black"
                                SortExpression="transaction_settlement_date">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="card_network" HeaderText="Card Network" SortExpression="card_network" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="card_issuer" HeaderText="Card Issuer" SortExpression="card_issuer" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Transactions found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                    </asp:GridView>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
