<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VaultSettlement.aspx.cs"
    Inherits="CCMSUI.admin.VaultSettlement" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit Vault Adjustments</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
  
    <script type="text/javascript">
        window.parent.window.contentPage = '';

        
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <input type="hidden" id="amt" runat="server" />
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["id"] == null?"Vault Settlement ":"Edit Vault Settlement"%>
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="SaveCheque">
                </asp:ValidationSummary>
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <div id="msg">
                </div>
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        
        <tr>
            <td>
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width="100%" style="padding-left: 10px">
                <tr>
            <td>
                Vault ID : 
            </td>
            <td>
                <asp:Label ID="Label_VaultId" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                Summary Date: 
            </td>
            <td>
                <asp:Label ID="Label_SummaryDate" runat="server"></asp:Label>
            </td>
        </tr>
                <tr>
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
                        <td valign="top">
                            <asp:GridView ID="GridView_CITVaultBalances" runat="server" AutoGenerateColumns="False"
                               Width="100%"
                             
                                HeaderStyle-ForeColor="#ffffff" PageSize="15">
                                <AlternatingRowStyle BackColor="#eff0ef" />
                                <RowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="title" HeaderText="ATM">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="vault_summary_date" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="denomination_name" HeaderText="Denomination" ItemStyle-HorizontalAlign="Left">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="opening_balance" HeaderText="Opening Balance" ItemStyle-HorizontalAlign="Right">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cash_recieved_from_bank" HeaderText="Bulk Cash" ItemStyle-HorizontalAlign="Right">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cash_delivered_to_atm" HeaderText="Cash delivered to ATM"
                                        ItemStyle-HorizontalAlign="Right">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cash_returned_from_atm" HeaderText="Cash returned from ATM"
                                        ItemStyle-HorizontalAlign="Right">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="closing_balance" HeaderText="Closing Balance" ItemStyle-HorizontalAlign="Right">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="generated_at" HeaderText="Generated At" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                    </asp:BoundField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No Records Found!</EmptyDataTemplate>
                                <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
                            </asp:GridView>
                        </td><td>
                    &nbsp;
                </td>
                        <td style="vertical-align: top">
                            <asp:GridView ID="GridView_VaultBalanceInvestigation" runat="server" AutoGenerateColumns="True"
                                Width="100%" HeaderStyle-CssClass="HeadingCell" OnRowDataBound="GridView_VaultBalanceInvestigation_RowDataBound"
                                HeaderStyle-ForeColor="#ffffff">
                                <AlternatingRowStyle BackColor="#eff0ef" />
                                <RowStyle HorizontalAlign="Left" />
                                <EmptyDataTemplate>
                                    No Records Found!</EmptyDataTemplate>
                                <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr id="trAddEditVaultAdj" runat="server">
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='VaultAdjustments.aspx'"
                                class="button" />
                        </td>
                    </tr>
                    <tr id="trApproveRejectVaultAdj" runat="server">
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_ApproveVaultAdj" runat="server" Text="Approve" OnClick="Button_ApproveVaultAdj_Click"
                                CssClass="button" Width="60px" ValidationGroup="SaveCheque" />&nbsp;
                            <asp:Button ID="Button_RejectVaultAdj" runat="server" Text="Reject" OnClick="Button_Reject_Click"
                                CssClass="button" Width="60px" ValidationGroup="SaveCheque" />&nbsp;
                            <input id="Button2" type="button" value="Cancel" onclick="window.location ='../ccms/usertask.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <br />
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
