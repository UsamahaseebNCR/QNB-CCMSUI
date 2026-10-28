<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_VaultSummary.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_VaultSummary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>NCR Vault Balance Monitoring DashBoard</title>
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
                            Task DashBoard
                            <img src="../../images/close.jpg" onclick="window.close();" style="cursor: hand"
                                height="18px" width="43px" alt="Click to close this window" />
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
                <td style="text-align: center">
                    <h1>
                        Virtual Vault Balance</h1>
                </td>
                <td>
                    &nbsp;
                </td>
                <td style="text-align: center">
                    <h1>
                        Vault Balance</h1>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top">
                    <asp:GridView ID="GridView_TempVaultBalanceInvestigation" runat="server" AutoGenerateColumns="True"
                        Width="100%" HeaderStyle-CssClass="HeadingCell" OnRowDataBound="GridView_VaultBalanceInvestigation_RowDataBound"
                        HeaderStyle-ForeColor="#ffffff">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Left" />
                        <%--<Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ledger_date" HeaderText="Date" SortExpression="ledger_date">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="vault_transaction_type" HeaderText="Vault Transaction Type" SortExpression="vault_transaction_type">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_type" HeaderText="Transaction Type" SortExpression="transaction_type">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="denomination_name" HeaderText="Denomination" SortExpression="denomination_name">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="total_quantity" HeaderText="Quantity" SortExpression="total_quantity">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="total_value" HeaderText="Value" SortExpression="total_value">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                        </Columns>--%>
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
                    </asp:GridView>
                </td>
                <td>
                    &nbsp;
                </td>
                <td style="vertical-align: top">
                    <asp:GridView ID="GridView_VaultBalanceInvestigation" runat="server" AutoGenerateColumns="True"
                        Width="100%"  
                        HeaderStyle-CssClass="HeadingCell" OnRowDataBound="GridView_VaultBalanceInvestigation_RowDataBound"
                        HeaderStyle-ForeColor="#ffffff" >
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Left" />
                        <%--<Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ledger_date" HeaderText="Date" SortExpression="ledger_date">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="vault_transaction_type" HeaderText="Vault Transaction Type" SortExpression="vault_transaction_type">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_type" HeaderText="Transaction Type" SortExpression="transaction_type">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="denomination_name" HeaderText="Denomination" SortExpression="denomination_name">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="total_quantity" HeaderText="Quantity" SortExpression="total_quantity">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="total_value" HeaderText="Value" SortExpression="total_value">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                        </Columns>--%>
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
                    </asp:GridView>
                </td>
            </tr>
         <%--   <tr>
                <td style="text-align: center">
                    <h1>
                        CIT</h1>
                </td>
                <td>
                    &nbsp;
                </td>
                <td style="text-align: center">
                    <h1>
                        Master Vault</h1>
                </td>
            </tr>
            <tr>
                <td valign="top"  >
                    <asp:GridView ID="GridView_CITVaultBalances" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CITVaultBalances_Sorting" Width="100%"
                        AllowPaging="True" OnPageIndexChanging="GridView_CITVaultBalances_PageIndexChanging"
                        HeaderStyle-ForeColor="#ffffff" PageSize="15">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" >
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="vault_summary_date" HeaderText="Date" 
                                DataFormatString="{0:dd/MM/yyyy}">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="denomination_name" HeaderText="Denomination"
                                ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="opening_balance" HeaderText="Opening Balance" 
                                ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_recieved_from_bank" HeaderText="Bulk Cash" 
                                ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_delivered_to_atm" HeaderText="Cash delivered to ATM"
                                ItemStyle-HorizontalAlign="Right" >
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_returned_from_atm" HeaderText="Cash returned from ATM"
                                ItemStyle-HorizontalAlign="Right" >
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="closing_balance" HeaderText="Closing Balance" 
                                ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="generated_at" HeaderText="Generated At" 
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
                    </asp:GridView>
                </td>
                <td>
                    &nbsp;
                </td>
                <td style="vertical-align: top">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="True" Width="100%"
                        AllowPaging="True" OnPageIndexChanging="GridView_VaultBalanceInvestigation_PageIndexChanging"
                        HeaderStyle-CssClass="HeadingCell" OnRowDataBound="GridView_VaultBalanceInvestigation_RowDataBound"
                        HeaderStyle-ForeColor="#ffffff" PageSize="15">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Left" />
                        <%--<Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ledger_date" HeaderText="Date" SortExpression="ledger_date">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="vault_transaction_type" HeaderText="Vault Transaction Type" SortExpression="vault_transaction_type">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_type" HeaderText="Transaction Type" SortExpression="transaction_type">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="denomination_name" HeaderText="Denomination" SortExpression="denomination_name">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="total_quantity" HeaderText="Quantity" SortExpression="total_quantity">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="total_value" HeaderText="Value" SortExpression="total_value">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                        </Columns>--
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
                    </asp:GridView>
                </td>
            </tr>--%>
        </table>
    </div>
    </form>
</body>
</html>
