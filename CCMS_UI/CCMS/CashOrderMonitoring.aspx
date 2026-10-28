<%@ Page Language="C#" AutoEventWireup="true" Codebehind="CashOrderMonitoring.aspx.cs"
    Inherits="CCMSUI.CCMS.CashOrderMonitoring" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/iSuite.css" rel="stylesheet" />
    <title>CCMS - Cash Order Monitoring</title>

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
                        Cash Order Monitoring ( History )</td>
                </tr>
                <tr>
                    <td class="ValidationText" align="center">
                        <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal></td>
                </tr>
                <tr>
                    <td class="ConfirmationText" align="center">
                        <asp:ValidationSummary ID="valSummary" runat="server" />
                        <%=Session[SessionVars.notice.ToString()]%>
                        <%Session[SessionVars.notice.ToString()] = "";%>
                        <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></td>
                </tr>
                <tr align="left" style="background-color: WHITE">
                    <td style="height: 20px; padding-left: 5px;" colspan="2">
                        <b>Search Filters</b></td>
                </tr>
                <tr align="left">
                    <td colspan="2">
                        <table width="100%" class="SEARCHBOX" border="0">
                            <tr>
                                <td>
                                    <table id="search" width="90%" border="0">
                                        <tr>
                                            <td align="left">
                                                Order Date :</td>
                                            <td>
                                                <asp:TextBox ID="TextBox_Date" runat="server"  Width="119px" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                                <asp:RequiredFieldValidator ID="requiredfieldvalidator_Date" runat="server" ControlToValidate="TextBox_Date"
                                                    ErrorMessage="Please enter Date." Text="*"></asp:RequiredFieldValidator>
                                               
                                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_Date" ErrorMessage="Given Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 20px">
                                                Status :</td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="DropDownList_status" runat="server" Style="width: 150px" CssClass="textbox">
                                                    <asp:ListItem Selected="True" Value="Any">Any</asp:ListItem>
                                                    <asp:ListItem Value="0">Completely Executed</asp:ListItem>
                                                    <asp:ListItem Value="1">Partially Executed</asp:ListItem>
                                                    <asp:ListItem Value="2">Pending</asp:ListItem>
                                                    <asp:ListItem Value="3">Over Replenished</asp:ListItem>
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
                                                <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                    Text="Search" Width="71px" />
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
                        Cash Order Listing
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                       <asp:GridView ID="GridView_CashOrderMonitoring" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CashOrderMonitoring_Sorting" Font-Size="Smaller"
                        Width="100%" OnRowDataBound="GridView_CashOrders_RowDataBound">
                        <HeaderStyle ForeColor="white" />
                        <RowStyle HorizontalAlign="Center" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                           <%-- <asp:BoundField DataField="last_order_received_at" HeaderText="Last Order Received"
                                SortExpression="last_order_received_at" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                HtmlEncode="false" ItemStyle-Width="100px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="last_order_delivered_at" HeaderText="Last Order Delivered"
                                SortExpression="last_order_delivered_at" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                HtmlEncode="false" ItemStyle-Width="100px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="last_order_executed_at" HeaderText="Last Order Executed"
                                SortExpression="last_order_executed_at" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                HtmlEncode="false" ItemStyle-Width="100px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="last_order_remaining_amount" HeaderText="Last Order Remaining Amount"
                                SortExpression="last_order_remaining_amount" HtmlEncode="False">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="last_order_suggested_amount" HeaderText="Last Order Suggested Amount"
                                SortExpression="last_order_suggested_amount">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="last_order_status" HeaderText="Last Order Status" SortExpression="last_order_status">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>--%>


                            <%--<asp:BoundField DataField="current_cash_positions_remaining_amount" HeaderText="Current Cash Position Amount"
                                SortExpression="remaining_amount">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>--%>
                            <%--<asp:TemplateField>
                                <ItemTemplate>
                                   <asp:Image ID="Image_OrderCancelled" runat="server" 
                                    ImageUrl="../../images/alert.jpg" Visible="false"/>
                                                                   </ItemTemplate>
                            </asp:TemplateField>--%>
                          
                            <asp:BoundField DataField="current_order_received_at" HeaderText="Creation Time"
                                SortExpression="current_order_received_at" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                HtmlEncode="false" ItemStyle-Width="100px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                           <%-- <asp:BoundField DataField="current_order_delivered_at" HeaderText="Current Order Delivered"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="current_order_delivered_at"
                                ItemStyle-Width="100px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>--%>
                            <asp:BoundField DataField="current_order_executed_at" HeaderText="Execution Time"
                                SortExpression="last_order_executed_at" DataFormatString="{0:dd/MM/yy HH:mm:ss}" ItemStyle-Width="100px"
                                HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="current_order_suggested_amount" HeaderText="Suggested Amount"
                                HtmlEncode="False"  SortExpression="current_order_suggested_amount">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="current_order_remaining_amount" HeaderText="Remaining Amount"
                                SortExpression="current_order_remaining_amount"  
                                HtmlEncode="False">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:TemplateField>
                             <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                <HeaderTemplate>
                                    Current Order Status
                                </HeaderTemplate>
                                <ItemTemplate>
                                    
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:BoundField DataField="current_order_status" HeaderText="Current Order Status"
                                SortExpression="current_order_status">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>--%>
                        </Columns>
                    </asp:GridView>

                    </td>
                </tr>
                <%--    <tr>
                    <th align="left">
                        Total Executed -
                        <asp:Literal ID="Literal_executed" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <th align="left">
                        Total Partially Executed -
                        <asp:Literal ID="Literal_partial" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                 <tr>
                    <th align="left">
                        Total Pending Orders -
                        <asp:Literal ID="Literal_not" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <th align="left">
                        Total Over Consumed Orders -
                        <asp:Literal ID="Literal_over" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>--%>
            </table>
        </div>
        <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
