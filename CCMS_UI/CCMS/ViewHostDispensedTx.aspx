<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewHostDispensedTx.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewHostDispensedTx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <title>View Host Dispensed Transactions (History)</title>

    <link href="../content/iSuite.css" rel="stylesheet" />

    <script src="../content/boxover.js" type="text/javascript"></script>
    <script src="../content/shortcut.js" type="text/javascript"></script>

    <script src="../content/cal/popcalendar.js" type="text/javascript"></script>
    <link href="../content/cal/popcalendar.css" rel="stylesheet" type="text/css" />

</head>

<body onkeydown='checkForCtrlKey(window.event);'
    onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">

    <form id="form1" runat="server">

        <div>

            <table width="100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">

                <tr>

                    <td class="PageHeading" align="left">View Host Dispensed Transactions (History)
                    </td>

                </tr>

                <tr>
                    <td class="ValidationText" align="center" colspan="2">
                        <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    </td>
                </tr>

                <tr>
                    <td class="ConfirmationText" align="center" colspan="2">

                        <asp:ValidationSummary ID="valSummary" runat="server" />

                        <%=Session[SessionVars.notice.ToString()]%>
                        <%Session[SessionVars.notice.ToString()] = "";%>

                        <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>

                    </td>
                </tr>

                <tr style="background-color: white">
                    <td colspan="2" style="height: 20px; padding-left: 5px;">
                        <b>Search Filters</b>
                    </td>
                </tr>

                <tr>
                    <td colspan="2">

                        <table width="100%" class="SEARCHBOX" border="0">

                            <tr>

                                <td>

                                    <table id="search" width="90%" border="0">

                                        <tr>

                                            <td style="width: 130px">From :
                                            </td>

                                            <td style="width: 170px">

                                                <asp:TextBox ID="TextBox_FromDate"
                                                    runat="server"
                                                    CssClass="textbox"
                                                    Width="120px" />

                                                <img src="../images/IconCalendar.gif"
                                                    onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID%>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: pointer" />

                                            </td>

                                            <td style="width: 130px">To :
                                            </td>

                                            <td style="width: 170px">

                                                <asp:TextBox ID="TextBox_ToDate"
                                                    runat="server"
                                                    CssClass="textbox"
                                                    Width="120px" />

                                                <img src="../images/IconCalendar.gif"
                                                    onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID%>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: pointer" />

                                            </td>

                                        </tr>

                                        <tr>

                                            <td>ATM ID :</td>

                                            <td>
                                                <asp:TextBox ID="TextBox_ATMID"
                                                    runat="server"
                                                    CssClass="textbox" />
                                            </td>

                                            <td>Card Number :</td>

                                            <td>
                                                <asp:TextBox ID="TextBox_PAN"
                                                    runat="server"
                                                    CssClass="textbox" />
                                            </td>

                                        </tr>

                                        <tr>

                                            <td>Customer Account No :</td>

                                            <td>
                                                <asp:TextBox ID="TextBox_AcctNo"
                                                    runat="server"
                                                    CssClass="textbox" />
                                            </td>

                                            <td>Transaction Amount :</td>

                                            <td>
                                                <asp:TextBox ID="TextBox_Amount"
                                                    runat="server"
                                                    CssClass="textbox" />
                                            </td>

                                        </tr>

                                        <tr>

                                            <td>Transaction Sequence :</td>

                                            <td>
                                                <asp:TextBox ID="TextBox_TSN"
                                                    runat="server"
                                                    CssClass="textbox" />
                                            </td>

                                            <td>Transaction Response :</td>

                                            <td>
                                                <asp:TextBox ID="TextBox_Response"
                                                    runat="server"
                                                    CssClass="textbox" />
                                            </td>

                                        </tr>

                                        <tr>

                                            <td colspan="4" align="right" style="padding-right: 80px">

                                                <asp:Button ID="Button_search"
                                                    runat="server"
                                                    CssClass="button"
                                                    Text="Search"
                                                    OnClick="Button_search_Click" />

                                                &nbsp;

                                                <asp:Button ID="Button_reset"
                                                    runat="server"
                                                    CssClass="button"
                                                    Text="Reset"
                                                    OnClick="Button_reset_Click" />

                                                &nbsp;

                                                <asp:Button ID="Button_ExporttoCSV"
                                                    runat="server"
                                                    CssClass="button"
                                                    Text="Export"
                                                    OnClick="Button_ExporttoCSV_Click" />

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
                    <td colspan="2">&nbsp;</td>
                </tr>

                <tr>

                    <th colspan="2" align="left">Selected Transactions -
                        <asp:Literal ID="Literal_count"
                            runat="server"
                            EnableViewState="False"></asp:Literal>

                    </th>

                </tr>

                <tr>

                    <td colspan="2" valign="top">

                        <asp:GridView
                            ID="GridView_Trxn"
                            runat="server"
                            AutoGenerateColumns="False"
                            AllowPaging="true"
                            AllowSorting="true"
                            PageSize="25"
                            Width="100%"
                            OnSorting="GridView_Trxn_Sorting"
                            OnPageIndexChanging="GridView_Trxn_PageIndexChanging">

                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center" />

                            <EmptyDataTemplate>
                                <center>No Records Found</center>
                            </EmptyDataTemplate>

                            <Columns>

                                <asp:BoundField DataField="ATM ID" HeaderText="ATM ID" SortExpression="hd.atm_id">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Card Number" HeaderText="Card Number" SortExpression="hd.card_number">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Account No." HeaderText="Account No." SortExpression="hd.customer_account_no">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="hd.transaction_date">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Time" HeaderText="Time" SortExpression="hd.transaction_time">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Amount" HeaderText="Amount" SortExpression="hd.transaction_amount">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="hd.transaction_currency">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Seq" HeaderText="Seq" SortExpression="hd.transaction_sequence">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="hd.transaction_type">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Response Code" HeaderText="Response Code" SortExpression="hd.transaction_response">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Card Network" HeaderText="Card Network" SortExpression="hd.card_network">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Card Issuer" HeaderText="Card Issuer" SortExpression="hd.card_issuer">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>

                            </Columns>


                            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />

                            <PagerStyle BackColor="#999999"
                                ForeColor="Black"
                                HorizontalAlign="Left" />

                            <HeaderStyle Font-Bold="True" ForeColor="White" />

                        </asp:GridView>

                    </td>

                </tr>

            </table>

        </div>

        <input type="button" runat="server" id="refresh" style="display: none" />

    </form>

</body>
</html>
