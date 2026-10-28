<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_CashOrderMonitoring.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_CashOrderMonitoring" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>NCR Cash Order Monitoring DashBoard</title>
     <script src="../content/boxover.js" type="text/javascript" language="javascript">
    </script>

    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
    <script type="text/javascript">
        function ShowDialog(title, fileStatus) {
            Dialog_ShowListOfFiles.Show();
            DialogCallBack_ShowListOfFiles.callback(title, fileStatus);
        }

        var mins;
        var secs;

        function cd(minutes) {
            mins = 1 * m(minutes); // change minutes here
            secs = 0 + s(":01"); // change seconds here (always add an additional second to your total)
            redo();
        }

        function m(obj) {
            for (var i = 0; i < obj.length; i++) {
                if (obj.substring(i, i + 1) == ":")
                    break;
            }

            return (obj.substring(0, i));
        }

        function s(obj) {
            for (var i = 0; i < obj.length; i++) {
                if (obj.substring(i, i + 1) == ":")
                    break;
            }

            return (obj.substring(i + 1, obj.length));
        }

        function dis(mins, secs) {
            var disp;
            if (mins <= 9) {
                disp = " 0";
            } else {
                disp = " ";
            }
            disp += mins + ":";
            if (secs <= 9) {
                disp += "0" + secs;
            } else {
                disp += secs;
            }
            return (disp);
        }

        function redo() {
            secs--;
            if (secs == -1) {
                secs = 59;
                mins--;
            }
            window.status = "Count Down " + dis(mins, secs); // setup additional displays here.
            if ((mins == 0) && (secs == 0)) {
                //window.alert("Time is up. Press OK to continue."); // change timeout message as required
                // window.location = "yourpage.htm" // redirects to specified page once timer ends and ok button is pressed
            } else {
                cd = setTimeout("redo()", 1000);
            }
        }




        function ToggleOptionView() {
            var optionsPanel = document.getElementById('optionsPanel');
            if (optionsPanel.style.display == 'none') {
                optionsPanel.style.display = 'block';
                document.getElementById('viewButton').value = 'Hide Options';

            }
            else {
                optionsPanel.style.display = 'none';
                document.getElementById('viewButton').value = 'Show Options';
            }
        }
    </script>
</head>
<body style="margin: 0; background-image: url(../../images/dashboard_bg.jpg)">
    <form id="form1" runat="server">
    <div>
        <%--<table cellpadding="0" cellspacing="0" border="0" width="100%" style="margin: 0px">
                <tr>
                    <td style="width: 10px; height: 23px;" valign="top">
                        <img src="../../images/box_lefttop.jpg" width="10px" height="23px" /></td>
                    <td width="100%" style="background-image: url(../../images/box_tophorizontal.jpg);
                        color: White;">
                        
                      <div style="text-align: right">
                        
                        
                       Cash Order Monitoring DashBoard
                            <img src="../../images/close.jpg" onclick="window.close();" style="cursor: hand"
                                height="18px" width="43px" alt="Click to close this window"/>
                       
                        </div>
                    </td>
                    <td style="width: 10px; height: 23px;" valign="top">
                        <img src="../../images/box_righttop.jpg" width="10px" height="23px" /></td>
                </tr>
                <tr valign="top">
                    <td style="width: 10px" valign="top" height="2px">
                        <img src="../../images/box_leftvertical_10px.jpg" width="10px" height="2px" /></td>
                    <td style="width: 100%" valign="top" height="2px">
                        <img src="../../images/menulinebottom.jpg" width="100%" height="2px" /></td>
                    <td style="width: 10px" valign="top" height="2px">
                        <img src="../../images/box_rightvertical_10px.jpg" width="10px" height="2px" /></td>
                </tr>
            </table>--%>
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <%--<td style="width:50%; text-align:center; font-size:large">
                    <b>Previous Replenishments</b>
                </td>--%>
                <td style="width:50%; text-align:center;font-size:large">
                    <b>Current/Next Replenishments</b>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top" colspan="2">
                    <asp:GridView ID="GridView_CashOrder" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CashOrder_Sorting" Font-Size="Smaller"
                        Width="100%" OnRowDataBound="GridView_CashOrders_RowDataBound" >
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
                          <%--  <asp:BoundField DataField="current_order_suggested_amount_text" HeaderText="Suggested Amount"
                                HtmlEncode="False"  SortExpression="current_order_suggested_amount">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="current_order_remaining_amount_text" HeaderText="Remaining Amount"
                                SortExpression="current_order_remaining_amount"  
                                HtmlEncode="False">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>--%>
                            <asp:BoundField DataField="tsn" HeaderText="Last TSN"
                                SortExpression="tsn"  
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
                <%--  <td>&nbsp;&nbsp;</td>
                    <td style="vertical-align: top">
                        <asp:GridView ID="GridView2" runat="server" 
                            AutoGenerateColumns="False" Font-Size="Smaller" Width="100%">
                             <RowStyle  HorizontalAlign="Center"/>
                            <EmptyDataTemplate>
                                <center>
                                No Rows Found</center>
                            </EmptyDataTemplate>
                            
                            <Columns>
                                <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="last_order_received_at" HeaderText="Last Order Received" SortExpression="last_order_received_at"
                                 DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="last_order_delivered_at" HeaderText="Last Order Delivered"  SortExpression="last_order_delivered_at"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                    <HeaderStyle  CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="last_order_executed_at" HeaderText="Last Order Executed" SortExpression="last_order_executed_at" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="last_order_remaining_amount" HeaderText="Last Order Remaining Amount"  
                                SortExpression="last_order_remaining_amount">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="current_order_received_at" HeaderText="Current Order Received" SortExpression="current_order_received_at" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="current_order_delivered_at" HeaderText="Current Order Delivered" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                SortExpression="current_order_delivered_at">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="current_order_executed_at" HeaderText="Current Order Executed" SortExpression="last_order_executed_at" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>


                                                                  <asp:BoundField DataField="current_order_suggested_amount" HeaderText="Current Order Suggested Amount" 
                                 SortExpression="current_order_suggested_amount">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>


                                 <asp:BoundField DataField="current_order_remaining_amount" HeaderText="Current Order Remaining Amount" 
                                 SortExpression="current_order_remaining_amount">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </td>--%>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
