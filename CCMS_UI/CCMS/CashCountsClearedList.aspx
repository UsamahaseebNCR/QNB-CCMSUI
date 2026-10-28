<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashCountsClearedList.aspx.cs"
    Inherits="CCMSUI.CCMS.CashCountsClearedList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CashCountsClearedList</title>
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
                    Cash Counts Cleared List
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
                    Selected Settlements -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2" style="text-align: center">
                    <asp:GridView ID="GridView_CashCountsCleared" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CashCountsCleared_Sorting" Width="100%" AllowPaging="True"
                        OnPageIndexChanging="GridView_CashCountsCleared_PageIndexChanging" HeaderStyle-ForeColor="Black"
                        PageSize="25">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title"
                                HeaderStyle-CssClass="HeadingCell" />

                            <asp:BoundField DataField="cash_remaining1" HeaderText="Rem1" SortExpression="cash_remaining1"
                                HeaderStyle-CssClass="HeadingCell"  />
                             <asp:BoundField DataField="cash_remaining2" HeaderText="Rem2" SortExpression="cash_remaining2" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_remaining3" HeaderText="Rem3" SortExpression="cash_remaining3" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_remaining4" HeaderText="Rem4" SortExpression="cash_remaining4" HeaderStyle-CssClass="HeadingCell" />
                            
                            <asp:BoundField DataField="cash_dispensed1" HeaderText="Dis1" SortExpression="cash_dispensed1"
                                HeaderStyle-CssClass="HeadingCell"  />
                             <asp:BoundField DataField="cash_dispensed2" HeaderText="Dis2" SortExpression="cash_dispensed2" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_dispensed3" HeaderText="Dis3" SortExpression="cash_dispensed3" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_dispensed4" HeaderText="Dis4" SortExpression="cash_dispensed4" HeaderStyle-CssClass="HeadingCell" />
                            

                            <asp:BoundField DataField="cash_purged1" HeaderText="Purg1" SortExpression="cash_purged1"
                                HeaderStyle-CssClass="HeadingCell"  />
                             <asp:BoundField DataField="cash_purged2" HeaderText="Purg2" SortExpression="cash_purged2" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_purged3" HeaderText="Purg3" SortExpression="cash_purged3" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_purged4" HeaderText="Purg4" SortExpression="cash_purged4" HeaderStyle-CssClass="HeadingCell" />

                            
                            <asp:BoundField DataField="clearing_datetime" HeaderText="Clearing Datetime" SortExpression="clearing_datetime"
                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                
                                 <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <a href='../admin/EditATMSettlement.aspx?id=<%# Eval("dispensed_id") %>'>
                                        <img alt="Click to edit cit" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField> 
                        </Columns>
                        <EmptyDataTemplate>
                            No Record(s) Found
                        </EmptyDataTemplate>
                    </asp:GridView>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
