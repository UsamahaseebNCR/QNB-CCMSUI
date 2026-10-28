<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashPositionVsDFFDetail.aspx.cs"
    Inherits="CCMSUI.CCMS.CashPositionVsDFFDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table  style="font-family:Arial; font-size:small">
            <tr>
                <td >
                    <b>Top 10 Replenishments</b>
                    <asp:GridView ID="GridView_Replenishment" runat="server" CellPadding="4" ForeColor="Black"
                        GridLines="Vertical" AutoGenerateColumns="False" BackColor="White" 
                        BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px">
                        <AlternatingRowStyle BackColor="White" />
                        <FooterStyle BackColor="#CCCC99" />
                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                        <RowStyle BackColor="#F7F7DE" />
                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#FBFBF2" />
                        <SortedAscendingHeaderStyle BackColor="#848384" />
                        <SortedDescendingCellStyle BackColor="#EAEAD3" />
                        <SortedDescendingHeaderStyle BackColor="#575357" />
                        <Columns>
                            <asp:BoundField DataField="cash_added1" HeaderText="Type 1" />
                            <asp:BoundField DataField="cash_added2" HeaderText="Type 2" />
                            <asp:BoundField DataField="cash_added3" HeaderText="Type 3" />
                            <asp:BoundField DataField="cash_added4" HeaderText="Type 4" />
                            <asp:BoundField DataField="rep_status" HeaderText="Status" />
                            <asp:BoundField DataField="rep_datetime" HeaderText="Datetime" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="is_swap" HeaderText="Is Swap" />
                            <asp:BoundField DataField="generated_at" HeaderText="Generated At" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        </Columns>
                    </asp:GridView>
                </td>

                <td valign="top">
                <b>Test Cash</b>
                    <asp:GridView ID="GridView_TestCash" runat="server" CellPadding="4" ForeColor="#333333"
                        GridLines="None" AutoGenerateColumns="false">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                        <Columns>
                            <asp:BoundField DataField="test_Cash_datetime" HeaderText="Datetime" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="cash_purged1" HeaderText="Type 1" />
                            <asp:BoundField DataField="cash_purged2" HeaderText="Type 2" />
                            <asp:BoundField DataField="cash_purged3" HeaderText="Type 3" />
                            <asp:BoundField DataField="cash_purged4" HeaderText="Type 4" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <b>Top 10 Cash Positions</b>
                    <asp:GridView ID="GridView_CashPosition" runat="server" CellPadding="4" ForeColor="#333333"
                        GridLines="None" AutoGenerateColumns="False">
                        <AlternatingRowStyle BackColor="White" />
                        <EditRowStyle BackColor="#7C6F57" />
                        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#E3EAEB" />
                        <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#F8FAFA" />
                        <SortedAscendingHeaderStyle BackColor="#246B61" />
                        <SortedDescendingCellStyle BackColor="#D4DFE1" />
                        <SortedDescendingHeaderStyle BackColor="#15524A" />
                        <Columns>
                            <asp:BoundField DataField="cassette1_notes" HeaderText="Type 1" />
                            <asp:BoundField DataField="cassette2_notes" HeaderText="Type 2" />
                            <asp:BoundField DataField="cassette3_notes" HeaderText="Type 3" />
                            <asp:BoundField DataField="cassette4_notes" HeaderText="Type 4" />
                            <asp:BoundField DataField="last_trxn_at" HeaderText="Last Transaction At" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                            <asp:BoundField DataField="purge_cassette1_notes" HeaderText="Type 1" />
                            <asp:BoundField DataField="purge_cassette2_notes" HeaderText="Type 2" />
                            <asp:BoundField DataField="purge_cassette3_notes" HeaderText="Type 3" />
                            <asp:BoundField DataField="purge_cassette4_notes" HeaderText="Type 4" />
                        </Columns>
                    </asp:GridView>
                </td>
                <td valign="top">
                    <b>Pending User Task</b>
                    <asp:GridView ID="GridView_UserTasks" runat="server" CellPadding="3" 
                        AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" 
                        BorderStyle="None" BorderWidth="1px">
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                        <RowStyle ForeColor="#000066" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                        
                        <Columns>
                            <asp:BoundField DataField="creation_time" HeaderText="Task Creation Time" DataFormatString="{0:dd/MM/yyyy HH:mm}" 
                             ItemStyle-VerticalAlign="Top">
<ItemStyle VerticalAlign="Top"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="status" HeaderText="Status"  
                                ItemStyle-VerticalAlign="Top">
<ItemStyle VerticalAlign="Top"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="generated_at" HeaderText="Generated At" 
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-VerticalAlign="Top">
<ItemStyle VerticalAlign="Top"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="alert_msg" HeaderText="Message" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
