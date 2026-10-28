<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewBNATransactions.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewBNATransactions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    View BNA Transactions
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
                                    <tr id="trTransactionRow" runat="server">
                                        <td class="style4">
                                            From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="datepicker" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                 ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td class="style1">
                                            To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="datepicker" />
                                           <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                 ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="right" style="padding-right: 80px">
                                            <asp:Button CssClass="button" ID="button_search" runat="server" Text="Search" OnClick="button_search_Click" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />
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
                    Selected Transactions -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView runat="server" ID="GridView_Trxn" AutoGenerateColumns="False" BorderStyle="Solid"
                        BorderWidth="1px" CellPadding="3" PageSize="25" Width="100%" AllowPaging="true"
                        AllowSorting="true" OnPageIndexChanging="GridView_Trxn_PageIndexChanging" 
                        OnSorting="GridView_Trxn_Sorting">
                        <EmptyDataTemplate>
                            <center>
                                No Records Found</center>
                        </EmptyDataTemplate>
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" SortExpression="title" ItemStyle-Width="20px"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="last_bna_deposit_at" HeaderText="DateTime" SortExpression="last_bna_deposit_at" ItemStyle-Width="20px"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="bna_cassette1" HeaderText="Cassette1" SortExpression="bna_cassette1" ItemStyle-HorizontalAlign="Right"  ItemStyle-Width="20px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                          <asp:TemplateField>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black"  Width="20px"  />
                           <HeaderTemplate>
                                Denomination Detail
                           </HeaderTemplate>
                           <ItemTemplate>
                                <table>
                                <%#Eval("cassette1_denomination_detail")%>
                                </table>
                                
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            </asp:TemplateField>
                           <%-- <asp:BoundField DataField="cassette1_denomination_detail" HeaderText="Cassette1 Detail" SortExpression="bna_cassette1" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>--%>
                            <asp:BoundField DataField="bna_cassette2" HeaderText="Cassette2" SortExpression="bna_cassette2" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                           <asp:TemplateField>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px"/>
                           <HeaderTemplate>
                                Denomination Detail
                           </HeaderTemplate>
                           <ItemTemplate>
                                <table>
                                <%#Eval("cassette2_denomination_detail")%>
                                </table>
                             </ItemTemplate>
                              <ItemStyle HorizontalAlign="Right" />
                             </asp:TemplateField>
                            <asp:BoundField DataField="bna_cassette3" HeaderText="Cassette3" SortExpression="bna_cassette3" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                           <asp:TemplateField>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px"/>
                           <HeaderTemplate>

                                Denomination Detail
                           </HeaderTemplate>
                           <ItemTemplate>
                                <table>
                                <%#Eval("cassette3_denomination_detail")%>
                                </table>
                                
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="bna_cassette4" HeaderText="Cassette4" SortExpression="bna_cassette4" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                           <asp:TemplateField>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px"/>
                           <HeaderTemplate>

                                Denomination Detail
                           </HeaderTemplate>
                           <ItemTemplate>
                                <table>
                                <%#Eval("cassette4_denomination_detail")%>
                                </table>
                                
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="bna_cassette5" HeaderText="Cassette5" SortExpression="bna_cassette5" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:TemplateField>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px"/>
                           <HeaderTemplate>

                                Denomination Detail
                           </HeaderTemplate>
                           <ItemTemplate>
                                <table>
                                <%#Eval("purge_denomination_detail")%>
                                </table>
                                
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="total" HeaderText="Total" SortExpression="total" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20px" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                        <%--<FooterStyle BackColor="#CCCCCC" />--%>
                        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                        <%--<PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Left" />--%>
                        <HeaderStyle Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
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
