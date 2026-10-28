<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParamEJCPMTransactions.aspx.cs"
    Inherits="CCMSUI.CCMS.ParamEJCPMTransactions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script src="../../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <script type="text/javascript">
        //window.parent.window.SetContentPage( window.location);
    </script>
    <style type="text/css">
        .style1
        {
            height: 17px;
            width: 130px;
        }
        .style3
        {
            width: 141px;
        }
        .style4
        {
            height: 17px;
            width: 141px;
        }
    </style>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="center">
                    CPM Transactions
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
                        BorderWidth="1px" CellPadding="3"  Width="100%" >
                        <EmptyDataTemplate>
                            <center>
                                No Records Found</center>
                        </EmptyDataTemplate>
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" SortExpression="title"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="trxn_datetime" HeaderText="DateTime" SortExpression="trxn_datetime"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" ItemStyle-Width="140px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="seq" HeaderText="TSN" SortExpression="seq">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="account_type" HeaderText="Account Type" SortExpression="account_type" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="pan" HeaderText="PAN" SortExpression="pan" ItemStyle-Width="140px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="result" HeaderText="Result" SortExpression="result">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="deposit_amount" HeaderText="Amount" SortExpression="deposit_amount" ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dispute_status" HeaderText="Dispute Status" SortExpression="dispute_status" ItemStyle-Width="50px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" ItemStyle-Width="80px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="comment" HeaderText="Comment" SortExpression="comment" ItemStyle-HorizontalAlign="Left">
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
   
    </form>
</body>
</html>
