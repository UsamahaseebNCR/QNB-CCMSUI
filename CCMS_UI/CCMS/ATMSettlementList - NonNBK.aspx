<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ATMSettlementList.aspx.cs"
    Inherits="CCMSUI.CCMS.ATMSettlementList" %>

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
                    ATM Settlements
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
                    <asp:GridView ID="GridView_ATMSettlement" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_ATMSettlement_Sorting" Width="100%" AllowPaging="True"
                        OnPageIndexChanging="GridView_ATMSettlement_PageIndexChanging" OnRowCommand="GridView_ATMSettlement_RowCommand" HeaderStyle-ForeColor="Black"
                        OnRowDataBound="GridView_ATMSettlement_RowDataBound"
                        PageSize="25">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title"
                                HeaderStyle-CssClass="HeadingCell" />

                            <asp:BoundField DataField="atm_site_and_number" HeaderText="Site" SortExpression="atm_site"
                                HeaderStyle-CssClass="HeadingCell"  />
                                
                          
                            <asp:BoundField DataField="gl_no" HeaderText="GL#" SortExpression="gl_no" HeaderStyle-CssClass="HeadingCell" Visible="false" />
                            <asp:BoundField DataField="rep_datetime" HeaderText="Replenished At" SortExpression="rep_datetime"
                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="seal_number" HeaderText="Seal#" HeaderStyle-CssClass="HeadingCell" Visible ="false"
                                SortExpression="seal_number" />
                            <asp:BoundField DataField="cash_rep_denomination_type1" HeaderText="Type 1" SortExpression="cash_rep_denomination_type1"
                                HeaderStyle-CssClass="HeadingCell" ItemStyle-BackColor="Chocolate" ItemStyle-ForeColor="White"/>
                            <asp:BoundField DataField="cash_rep_counters_type1" HeaderText="Added" SortExpression="cash_rep_counters_type1"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_dispensed_counters_type1" HeaderText="Dispensed" Visible ="false"
                                SortExpression="cash_dispensed_counters_type1" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_rem_counters_type1" HeaderText="Remaining" SortExpression="cash_rem_counters_type1"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_rejected_counters_type1" HeaderText="Rejected" SortExpression="cash_rejected_counters_type1"
                                HeaderStyle-CssClass="HeadingCell" Visible ="false"/>
                            <asp:BoundField DataField="cash_rep_denomination_type2" HeaderText="Type 2" SortExpression="cash_rep_denomination_type2"
                                HeaderStyle-CssClass="HeadingCell" ItemStyle-BackColor="Info"/>
                            <asp:BoundField DataField="cash_rep_counters_type2" HeaderText="Added" SortExpression="cash_rep_counters_type2"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_dispensed_counters_type2" HeaderText="Dispensed" Visible ="false"
                                SortExpression="cash_dispensed_counters_type2" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_rem_counters_type2" HeaderText="Remaining" SortExpression="cash_rem_counters_type2"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_rejected_counters_type2" HeaderText="Rejected" SortExpression="cash_rejected_counters_type2"
                                HeaderStyle-CssClass="HeadingCell" Visible ="false"/>
                            <asp:BoundField DataField="cash_rep_denomination_type3" HeaderText="Type 3" SortExpression="cash_rep_denomination_type3"
                                HeaderStyle-CssClass="HeadingCell" ItemStyle-BackColor="gray" ItemStyle-ForeColor="White"/>
                            <asp:BoundField DataField="cash_rep_counters_type3" HeaderText="Added" SortExpression="cash_rep_counters_type3"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_dispensed_counters_type3" HeaderText="Dispensed" Visible ="false"
                                SortExpression="cash_dispensed_counters_type3" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_rem_counters_type3" HeaderText="Remaining" SortExpression="cash_rem_counters_type3"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_rejected_counters_type3" HeaderText="Rejected" SortExpression="cash_rejected_counters_type3"
                                HeaderStyle-CssClass="HeadingCell"  Visible ="false"/>

                                <asp:BoundField DataField="cash_rep_denomination_type4" HeaderText="Type 4" SortExpression="cash_rep_denomination_type4"
                                HeaderStyle-CssClass="HeadingCell" ItemStyle-BackColor="Brown" ItemStyle-ForeColor="White"/>
                            <asp:BoundField DataField="cash_rep_counters_type4" HeaderText="Added" SortExpression="cash_rep_counters_type4"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_dispensed_counters_type4" HeaderText="Dispensed" Visible ="false"
                                SortExpression="cash_dispensed_counters_type4" HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_rem_counters_type4" HeaderText="Remaining" SortExpression="cash_rem_counters_type4"
                                HeaderStyle-CssClass="HeadingCell" />
                            <asp:BoundField DataField="cash_rejected_counters_type4" HeaderText="Rejected" SortExpression="cash_rejected_counters_type4"
                                HeaderStyle-CssClass="HeadingCell" Visible ="false"/>
                                <%--<asp:BoundField DataField="total_replenished" HeaderText="Replenished" SortExpression="total_replenished"
                                HeaderStyle-CssClass="HeadingCell"  />--%>
                            <asp:BoundField DataField="total_Replenishment_text" HeaderText="Replenished" SortExpression="total_replenished_text"
                                HeaderStyle-CssClass="HeadingCell"  />
                                <%--<asp:BoundField DataField="total_rejected" HeaderText="Returned" SortExpression="total_rejected"
                                HeaderStyle-CssClass="HeadingCell"  />--%>
                            <asp:BoundField DataField="total_rejected_text" HeaderText="Rejected" SortExpression="total_rejected_text"
                                HeaderStyle-CssClass="HeadingCell"  />
                            <asp:BoundField DataField="total_remaining_text" HeaderText="Returned" SortExpression="total_remaining_text"
                                HeaderStyle-CssClass="HeadingCell"  />
                                
                                 <asp:TemplateField HeaderText="Edit" Visible ="false">
                                <ItemTemplate>
                                    <a href='../admin/EditATMSettlement.aspx?id=<%# Eval("atm_settlement_id") %>'>
                                        <img alt="Click to edit cit" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="deleteATMSettlement" CommandArgument='<%# Eval("atm_settlement_id") %>'
                                            ToolTip="Click to delete atm settlement" ImageUrl="../images/Delete.gif" ID="Button_deleteATMSettlement"
                                            CssClass="ShowHand" />
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
