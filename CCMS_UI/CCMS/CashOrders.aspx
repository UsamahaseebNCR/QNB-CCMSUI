<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashOrders.aspx.cs" Inherits="CCMSUI.CCMS.CashOrders" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/isuite.css" rel="stylesheet" />
    <title>iSuite - ATMs</title>
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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Cash Orders
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
                <td style="height: 20px; padding-left: 5px; width: 100%;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2" style="width: 100%">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td>
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                          
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                          
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 20px">
                                            Replenishment Type :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_repType" runat="server" Style="width: 150px" CssClass="textbox">
                                                <asp:ListItem Selected="True" Value="Any">Any</asp:ListItem>
                                                <asp:ListItem Value="RS">RS</asp:ListItem>
                                                <asp:ListItem Value="UR">UR</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td style="height: 20px">
                                            Created by :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_creator" runat="server" Width="150px" CssClass="dropDown">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 20px">
                                            Status :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_OrderStatus" runat="server" Width="150px" CssClass="dropDown">
                                                <asp:ListItem>Any</asp:ListItem>
                                                <asp:ListItem>Cancelled</asp:ListItem>
                                                <asp:ListItem>Normal</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td colspan="2">
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />
                                            <asp:Button ID="Button_ExportCashOrder" runat="server" Text="Export Cash Order" CssClass="button"
                                                OnClick="Button_ExportCashOrder_Click" Visible="false" />
                                                 &nbsp;
                                                <asp:Button ID="Button_Export" runat="server" CssClass="button" 
                                                Text="Export Order" onclick="Button_ExportOrder_Click"  />
                                            <asp:HyperLink ID="HyperLink_CreateCashOrders" runat="server" CssClass="Link" NavigateUrl="EditCashOrders.aspx">Create Cash Orders ... </asp:HyperLink>
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
                <th colspan="2" style="width: 100%" align="left">
                    Selected Scheduled Cash Orders -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_CashOrders" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CashOrders_Sorting" Width="100%" AllowPaging="True"
                        OnPageIndexChanging="GridView_CashOrders_PageIndexChanging" PageSize="15" OnRowDataBound="GridView_CashOrders_RowDataBound"
                        DataKeyNames="cash_order_id" OnRowCommand="GridView_CashOrders_RowCommand">
                        <RowStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <%--<asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>--%>
                            <asp:BoundField DataField="cash_order_type" HeaderText="Rep Type" SortExpression="cash_order_type"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                             <asp:BoundField DataField="creation_time" HeaderText="Creation Date" SortExpression="creation_time"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HtmlEncode="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_order_datetime" HeaderText="Order Date" SortExpression="cash_order_datetime"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HtmlEncode="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                             <asp:BoundField DataField="replenishment_datetime" HeaderText="Repl Date" SortExpression="replenishment_datetime"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HtmlEncode="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="order_number" HeaderText="Order Number" SortExpression="order_number"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette1_denomination" HeaderText="Den 1" SortExpression="Cassette1_denomination"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette1_suggested_notes" HeaderText="Sugg 1" SortExpression="Cassette1_suggested_notes"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette2_denomination" HeaderText="Den 2" SortExpression="Cassette2_denomination"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette2_suggested_notes" HeaderText="Sugg 2" SortExpression="Cassette2_suggested_notes"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette3_denomination" HeaderText="Den 3" SortExpression="Cassette3_denomination"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette3_suggested_notes" HeaderText="Sugg 3" SortExpression="Cassette3_suggested_notes"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette4_denomination" HeaderText="Den 4" SortExpression="Cassette4_denomination"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette4_suggested_notes" HeaderText="Sugg 4" SortExpression="Cassette4_suggested_notes"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette5_denomination" HeaderText="Den 5" SortExpression="Cassette5_denomination"
                                Visible="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette5_suggested_notes" HeaderText="Sugg 5" SortExpression="Cassette5_suggested_notes"
                                Visible="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette6_denomination" HeaderText="Den 6" SortExpression="Cassette6_denomination"
                                Visible="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette6_suggested_notes" HeaderText="Sugg 6" SortExpression="Cassette6_suggested_notes"
                                Visible="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette7_denomination" HeaderText="Den 7" SortExpression="Cassette7_denomination"
                                Visible="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cassette7_suggested_notes" HeaderText="Sugg 7" SortExpression="Cassette7_suggested_notes"
                                Visible="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <%--<asp:BoundField DataField="sum" HeaderText="Sum" SortExpression="sum" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>--%>
                            <asp:BoundField DataField="sum_text" HtmlEncode="false" HeaderText="Sum" SortExpression="sum" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                             <asp:CheckBoxField DataField="is_hold" HeaderText="Hold">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:CheckBoxField>
                            
                            <asp:TemplateField HeaderText="Edit" HeaderStyle-ForeColor="black">
                                <ItemTemplate>
                                    
                                    <asp:HyperLink ID="HyperLink_EditCashOrders" runat="server" ImageUrl="../images/Edit.gif"
                                        NavigateUrl='<%#String.Format("EditCashOrders.aspx?cash_order_id={0}",Eval("cash_order_id"))%>' >
                                    </asp:HyperLink>
                                    
                                    <%--<a href='EditCashOrders.aspx?cash_order_id=<%# Eval("cash_order_id") %>' id="editLink" runat="server"> 
                                            <img alt="Click to edit cash order" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>--%>
                                    <%--<asp:ImageButton ID="ImageButton_EditCashOrders" runat="server" 
                                        ImageUrl ="../images/Edit.gif"
                                        <a href='EditCashOrders.aspx?cash_order_id=<%# Eval("cash_order_id") %>'>
                                            <img alt="Click to edit cash order" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>--%>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cancel">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="cancelCashOrder" CommandArgument='<%# Eval("cash_order_id") %>'
                                        ToolTip="Click to cancel order" ImageUrl="../images/Delete.gif" ID="Button_CancelCashOrder"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Hold" Visible="false">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="holdCashOrder" CommandArgument='<%# Eval("cash_order_id") %>'
                                        ToolTip="Click to hold order" ImageUrl="../images/Delete.gif" ID="Button_HoldCashOrder"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <%-- <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />--%>
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
