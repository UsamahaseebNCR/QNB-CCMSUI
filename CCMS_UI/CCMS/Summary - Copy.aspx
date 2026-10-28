<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Summary.aspx.cs" Inherits="CCMSUI.CCMS.Summary"
    EnableSessionState="True" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Summary</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <table style="height: 100%" cellspacing="0" cellpadding="0" border="0" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2">
                Summary
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2" valign="top">
                <table width="100%" border="0">
                    <tr>
                        <td valign="top">
                            <asp:Literal ID="literal_script" runat="server"></asp:Literal>
                            <ComponentArt:TabStrip ID="TabStrip_Summary" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_Summary"
                                runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                <ItemLooks>
                                    <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                        LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                        LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                        HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                        RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                    <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                        LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                        RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                        RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                </ItemLooks>
                                <Tabs>
                                    <ComponentArt:TabStripTab ID="TabStripTab_RecentActivity" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Recent Activity">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_LowBalance" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text=" Low Balance">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_Transactions" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Transactions">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_Replenishments" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Replenishment">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_Deposit" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Deposit collection">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_Cheque" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Cheque collection">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_PurgeBinAlerts" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Purge Bin Alerts">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_PurgedCount" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Purged Transaction Count">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_CounterMismatch" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Counter Mismatch">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_DFF" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Daily Feed File Information">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_Communication" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATM Cash Level File Summary">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_Alerts" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Top 20 Alerts">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_DeadATMs" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Dead ATMs">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_DatetimeSync" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="DateTime Sync">
                                    </ComponentArt:TabStripTab>
                                </Tabs>
                            </ComponentArt:TabStrip>
                            <ComponentArt:MultiPage ID="MultiPage_Summary" CssClass="MultiPage" runat="server">
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_RecentActivity">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg;">
                                        <tr>
                                            <td id="tableHeaderRecentActivity" runat="server" style="background-color: #54b948;
                                                color: white; text-align: center">
                                                <b>Recent Activity At All ATMs</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" align="center">
                                                <table width="100%" border="0">
                                                    <tr align="left">
                                                        <td valign="top" style="width: 150px">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Button ID="RecentActivityMenu1" runat="server" OnClick="RecentActivityMenu1_Click"
                                                                            CssClass="button" Text="Today's Activity" Width="150px" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left">
                                                                        <asp:Button ID="RecentActivityMenu2" runat="server" OnClick="RecentActivityMenu2_Click"
                                                                            CssClass="button" Text="Yesterday's Activity" Width="150px" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left">
                                                                        <asp:Button ID="RecentActivityMenu3" runat="server" OnClick="RecentActivityMenu3_Click"
                                                                            CssClass="button" Text="Last Three Day's Activity" Width="150px" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left">
                                                                        <asp:Button ID="RecentActivityMenu4" runat="server" OnClick="RecentActivityMenu4_Click"
                                                                            CssClass="button" Text="Last Seven Day's Activity" Width="150px" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td valign="top">
                                                            <table width="100%" border="0">
                                                                <tr>
                                                                    <td align="left">
                                                                        <asp:Literal ID="Literal_DisplayGraph" runat="server"></asp:Literal>
                                                                        <div style="height: 250px; width: 100%; overflow: auto">
                                                                            <asp:GridView ID="GridView_RecentActivity" runat="server" Width="70%" AutoGenerateColumns="false"
                                                                                HorizontalAlign="Center">
                                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" HorizontalAlign="Center" />
                                                                                <Columns>
                                                                                    <asp:BoundField DataField="Task" HeaderText="Task" ItemStyle-HorizontalAlign="Center" />
                                                                                    <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-HorizontalAlign="Center" />
                                                                                    <asp:BoundField DataField="Count" HeaderText="Count" ItemStyle-HorizontalAlign="Center" />
                                                                                </Columns>
                                                                                <EmptyDataTemplate>
                                                                                    No records found
                                                                                </EmptyDataTemplate>
                                                                            </asp:GridView>
                                                                        </div>
                                                                    </td>
                                                                    <%--<td style="vertical-align: top; width: 30%">
                                                                            <div id="Div_RecentActivity" runat="server" style="vertical-align: top">
                                                                            </div>
                                                                        </td>--%>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_LowBalance">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>Low Balance</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <ComponentArt:TabStrip ID="TabStrip_LowBalance" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage2"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab4" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab5" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>
                                                <ComponentArt:MultiPage ID="MultiPage2" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView4">
                                                        <div style="height: 250px; width: 100%; overflow: auto" runat="server" id="divCurrentLowBalance">
                                                            <asp:GridView ID="GridView_LowBalance_CurrentDay" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                AllowSorting="true" OnSorting="GridView_LowBalance_Sorting" HorizontalAlign="Center">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="min_operating_balance" HeaderText="Minimum Operating Balance"
                                                                        ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                                                        DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                    <asp:BoundField DataField="type1" HeaderText="Type 1" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type2" HeaderText="Type 2" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type3" HeaderText="Type 3" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type4" HeaderText="Type 4" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="balance" HeaderText="Balance" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView5">
                                                        <div style="height: 250px; width: 100%; overflow: auto" runat="server" id="divPreviousLowBalance">
                                                            <asp:GridView ID="GridView_LowBalance_PreviousDay" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_LowBalance_Sorting">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="min_operating_balance" HeaderText="Minimum Operating Balance"
                                                                        ItemStyle-HorizontalAlign="Center" SortExpression="min_operating_balance" />
                                                                    <asp:BoundField DataField="generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                                                        DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                    <asp:BoundField DataField="type1" HeaderText="Type 1" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type2" HeaderText="Type 2" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type3" HeaderText="Type 3" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type4" HeaderText="Type 4" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="balance" HeaderText="Balance" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No previous records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                </ComponentArt:MultiPage>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_MostActiveATMs">
                                    <table width="90%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>ATMs Transactions</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <ComponentArt:TabStrip ID="TabStrip3" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_Trxn"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab_MostActiveATMs" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATMs Transactions">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab_ATMsWithoutTrxns" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATMs without transactions">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>
                                                <ComponentArt:MultiPage ID="MultiPage_Trxn" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView16">
                                                        <div style="height: 250px; width: 100%; overflow: auto" runat="server" id="div2">
                                                            <asp:GridView ID="GridView_TransactionATMs" runat="server" AutoGenerateColumns="false"
                                                                AllowSorting="true" OnSorting="GridView_TransactionATMs_Sorting">
                                                                <AlternatingRowStyle BackColor="#eff0ef" Width="100%" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="Title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="Title" />
                                                                    <asp:BoundField DataField="Count" HeaderText="Count" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="Count" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_ATMsWithoutTrxns">
                                                        <table align="left" width="75%" style="background: ../images/PatternHomeBG.jpg;">
                                                            <%--<tr>
                                                                <td style="background-color: #FFC73C; text-align: center">
                                                                    <b>ATMs without transactions</b>
                                                                </td>
                                                            </tr>--%>
                                                            <tr>
                                                                <td valign="top">
                                                                    <ComponentArt:TabStrip ID="TabStrip1" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                                        DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                                        DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage1"
                                                                        runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                                        ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                                        Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                                        <ItemLooks>
                                                                            <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                                                LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                                                LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                                                HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                                                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                                            <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                                                LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                                                RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                                                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                                        </ItemLooks>
                                                                        <Tabs>
                                                                            <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                AutoPostBackOnSelect="true" DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATMs with no transactions on current day">
                                                                            </ComponentArt:TabStripTab>
                                                                            <ComponentArt:TabStripTab ID="TabStripTab16" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATMs having threshold reached">
                                                                            </ComponentArt:TabStripTab>
                                                                            <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATMs having last transaction">
                                                                            </ComponentArt:TabStripTab>
                                                                            <ComponentArt:TabStripTab ID="TabStripTab3" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATMs having no last transaction">
                                                                            </ComponentArt:TabStripTab>
                                                                            <ComponentArt:TabStripTab ID="TabStripTab19" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATMs Threshold Hourly Analysis">
                                                                            </ComponentArt:TabStripTab>
                                                                        </Tabs>
                                                                    </ComponentArt:TabStrip>
                                                                    <ComponentArt:MultiPage ID="MultiPage1" CssClass="MultiPage" runat="server">
                                                                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1">
                                                                            <div style="overflow: auto; height: 300px; width: 350px; vertical-align: top; border: 0"
                                                                                id="divWithoutTrxn">
                                                                                <asp:GridView ID="GridView_CurrentDay" runat="server" AutoGenerateColumns="false"
                                                                                    Width="80%" OnRowDataBound="GridView_CurrentDay_RowDataBound" AllowSorting="true"
                                                                                    OnSorting="GridView_CurrentDay_Sorting">
                                                                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                                    <Columns>
                                                                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                            SortExpression="title" />
                                                                                        <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                                            SortExpression="last_heart_beat_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                                    </Columns>
                                                                                    <EmptyDataTemplate>
                                                                                        No records found
                                                                                    </EmptyDataTemplate>
                                                                                </asp:GridView>
                                                                            </div>
                                                                        </ComponentArt:PageView>
                                                                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView27">
                                                                            <div style="overflow: auto; height: 300px; vertical-align: top">
                                                                                <asp:GridView ID="GridView_thresholdReached" runat="server" Width="80%" AutoGenerateColumns="false"
                                                                                    OnRowDataBound="GridView_CurrentDay_RowDataBound" AllowSorting="true" OnSorting="GridView_ThresholdReached_Sorting">
                                                                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                                    <Columns>
                                                                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                            SortExpression="title" />
                                                                                        <asp:BoundField DataField="generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                                                                            DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                                        <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                                            DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_heart_beat_at" />
                                                                                        <asp:BoundField DataField="last_trxn_at" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                                                                            DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_trxn_at" />
                                                                                        <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"
                                                                                            SortExpression="amount" />
                                                                                    </Columns>
                                                                                    <EmptyDataTemplate>
                                                                                        No records found
                                                                                    </EmptyDataTemplate>
                                                                                </asp:GridView>
                                                                            </div>
                                                                        </ComponentArt:PageView>
                                                                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2">
                                                                            <div style="overflow: auto; height: 300px; vertical-align: top">
                                                                                <asp:GridView ID="GridView_AnyDay" runat="server" Width="80%" AutoGenerateColumns="false"
                                                                                    OnRowDataBound="GridView_CurrentDay_RowDataBound" AllowSorting="true" OnSorting="GridView_LastTrxn_Sorting">
                                                                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                                    <Columns>
                                                                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                            SortExpression="title" />
                                                                                        <asp:BoundField DataField="trxn_datetime" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                                                                            DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="trxn_datetime" />
                                                                                        <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                                            DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_heart_beat_at" />
                                                                                        <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"
                                                                                            SortExpression="amount" DataFormatString="{0:C}" HtmlEncode="false" />
                                                                                    </Columns>
                                                                                    <EmptyDataTemplate>
                                                                                        No records found
                                                                                    </EmptyDataTemplate>
                                                                                </asp:GridView>
                                                                            </div>
                                                                        </ComponentArt:PageView>
                                                                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView3">
                                                                            <div style="overflow: auto; height: 300px; width: 350px; vertical-align: top">
                                                                                <asp:GridView ID="GridView_noTrxn" runat="server" Width="80%" AutoGenerateColumns="false"
                                                                                    OnRowDataBound="GridView_CurrentDay_RowDataBound" AllowSorting="true" OnSorting="GridView_NoLastTrxn_Sorting">
                                                                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                                    <Columns>
                                                                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                            SortExpression="title" />
                                                                                        <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                                            DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_heart_beat_at" />
                                                                                    </Columns>
                                                                                    <EmptyDataTemplate>
                                                                                        No records found
                                                                                    </EmptyDataTemplate>
                                                                                </asp:GridView>
                                                                            </div>
                                                                        </ComponentArt:PageView>
                                                                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView28">
                                                                            <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                                                                <tr>
                                                                                    <td style="background-color: #54b948; color: white; text-align: center">
                                                                                        <b>ATMs Threshold Hourly Analysis</b>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td valign="top">
                                                                                        <ComponentArt:TabStrip ID="TabStrip6" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                                                            DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                                                            DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage11"
                                                                                            runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                                                            ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                                                            Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                                                            <ItemLooks>
                                                                                                <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                                                                    LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                                                                    LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                                                                    HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                                                                    RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                                                                <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                                                                    LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                                                                    RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                                                                    RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                                                            </ItemLooks>
                                                                                            <Tabs>
                                                                                                <ComponentArt:TabStripTab ID="TabStripTab20" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="1 Hour"
                                                                                                    AutoPostBackOnSelect="true">
                                                                                                </ComponentArt:TabStripTab>
                                                                                                <ComponentArt:TabStripTab ID="TabStripTab21" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="4 Hour"
                                                                                                    AutoPostBackOnSelect="true">
                                                                                                </ComponentArt:TabStripTab>
                                                                                                <ComponentArt:TabStripTab ID="TabStripTab22" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="8 Hour"
                                                                                                    AutoPostBackOnSelect="true">
                                                                                                </ComponentArt:TabStripTab>
                                                                                                <ComponentArt:TabStripTab ID="TabStripTab23" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="16 Hour"
                                                                                                    AutoPostBackOnSelect="true">
                                                                                                </ComponentArt:TabStripTab>
                                                                                            </Tabs>
                                                                                        </ComponentArt:TabStrip>
                                                                                        <ComponentArt:MultiPage ID="MultiPage11" CssClass="MultiPage" runat="server">
                                                                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView29">
                                                                                                <div style="height: 250px; width: 100%; overflow: auto" runat="server" id="div6">
                                                                                                    <asp:GridView ID="GridView_Threshold1" runat="server" Width="80%" AutoGenerateColumns="false"
                                                                                                        OnRowDataBound="GridView_CurrentDay_RowDataBound" AllowSorting="true" OnSorting="GridView_Threshold1_Sorting">
                                                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                                                        <Columns>
                                                                                                            <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                                                SortExpression="title" />
                                                                                                            <asp:BoundField DataField="generated_at" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                                                            <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_heart_beat_at" />
                                                                                                            <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"
                                                                                                                SortExpression="amount" />
                                                                                                        </Columns>
                                                                                                        <EmptyDataTemplate>
                                                                                                            No records found
                                                                                                        </EmptyDataTemplate>
                                                                                                    </asp:GridView>
                                                                                                </div>
                                                                                            </ComponentArt:PageView>
                                                                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView30">
                                                                                                <div style="height: 250px; width: 100%; overflow: auto" runat="server" id="div7">
                                                                                                    <asp:GridView ID="GridView_Threshold4" runat="server" Width="80%" AutoGenerateColumns="false"
                                                                                                        OnRowDataBound="GridView_CurrentDay_RowDataBound" AllowSorting="true" OnSorting="GridView_Threshold4_Sorting">
                                                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                                                        <Columns>
                                                                                                            <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                                                SortExpression="title" />
                                                                                                            <asp:BoundField DataField="generated_at" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                                                            <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_heart_beat_at" />
                                                                                                            <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"
                                                                                                                SortExpression="amount" />
                                                                                                        </Columns>
                                                                                                        <EmptyDataTemplate>
                                                                                                            No records found
                                                                                                        </EmptyDataTemplate>
                                                                                                    </asp:GridView>
                                                                                                </div>
                                                                                            </ComponentArt:PageView>
                                                                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView31">
                                                                                                <div style="height: 250px; width: 100%; overflow: auto" runat="server" id="div8">
                                                                                                    <asp:GridView ID="GridView_Threshold8" runat="server" Width="80%" AutoGenerateColumns="false"
                                                                                                        OnRowDataBound="GridView_CurrentDay_RowDataBound" AllowSorting="true" OnSorting="GridView_Threshold8_Sorting">
                                                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                                                        <Columns>
                                                                                                            <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                                                SortExpression="title" />
                                                                                                            <asp:BoundField DataField="generated_at" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                                                            <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_heart_beat_at" />
                                                                                                            <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"
                                                                                                                SortExpression="amount" />
                                                                                                        </Columns>
                                                                                                        <EmptyDataTemplate>
                                                                                                            No records found
                                                                                                        </EmptyDataTemplate>
                                                                                                    </asp:GridView>
                                                                                                </div>
                                                                                            </ComponentArt:PageView>
                                                                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView32">
                                                                                                <div style="height: 250px; width: 100%; overflow: auto" runat="server" id="div9">
                                                                                                    <asp:GridView ID="GridView_Threshold16" runat="server" Width="80%" AutoGenerateColumns="false"
                                                                                                        OnRowDataBound="GridView_CurrentDay_RowDataBound" AllowSorting="true" OnSorting="GridView_Threshold16_Sorting">
                                                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                                                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                                                        <Columns>
                                                                                                            <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                                                SortExpression="title" />
                                                                                                            <asp:BoundField DataField="generated_at" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                                                            <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_heart_beat_at" />
                                                                                                            <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"
                                                                                                                SortExpression="amount" />
                                                                                                        </Columns>
                                                                                                        <EmptyDataTemplate>
                                                                                                            No records found
                                                                                                        </EmptyDataTemplate>
                                                                                                    </asp:GridView>
                                                                                                </div>
                                                                                            </ComponentArt:PageView>
                                                                                        </ComponentArt:MultiPage>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </ComponentArt:PageView>
                                                                    </ComponentArt:MultiPage>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ComponentArt:PageView>
                                                </ComponentArt:MultiPage>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView21">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>Replenishments</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <ComponentArt:TabStrip ID="TabStrip5" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage10"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab17" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Suggested Replenishments"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab18" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Faulty"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>
                                                <ComponentArt:MultiPage ID="MultiPage10" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView25">
                                                        <asp:GridView ID="GridView_SuggestedRep" runat="server" Width="30%" AutoGenerateColumns="false"
                                                            HorizontalAlign="left" AllowSorting="true" OnSorting="GridView_SuggestedReplenishment_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            <Columns>
                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="title" />
                                                                <asp:BoundField DataField="cassette1_notes" HeaderText="Type 1" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="cassette1_notes" />
                                                                <asp:BoundField DataField="cassette2_notes" HeaderText="Type 2" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="cassette2_notes" />
                                                                <asp:BoundField DataField="cassette3_notes" HeaderText="Type 3" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="cassette3_notes" />
                                                                <asp:BoundField DataField="cassette4_notes" HeaderText="Type 4" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="cassette4_notes" />
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </ComponentArt:PageView>
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView26">
                                                        <ComponentArt:TabStrip ID="TabStrip7" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                            DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                            DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage30"
                                                            runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                            ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                            Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                            <ItemLooks>
                                                                <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                                    LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                                    LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                                    HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                                    RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                                <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                                    LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                                    RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                                    RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                            </ItemLooks>
                                                            <Tabs>
                                                                <ComponentArt:TabStripTab ID="TabStripTab24" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day"
                                                                    AutoPostBackOnSelect="true">
                                                                </ComponentArt:TabStripTab>
                                                                <ComponentArt:TabStripTab ID="TabStripTab25" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day"
                                                                    AutoPostBackOnSelect="true">
                                                                </ComponentArt:TabStripTab>
                                                            </Tabs>
                                                        </ComponentArt:TabStrip>
                                                        <ComponentArt:MultiPage ID="MultiPage30" CssClass="MultiPage" runat="server">
                                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView33">
                                                                <asp:GridView ID="GridView_FaultyRep1" runat="server" Width="30%" AutoGenerateColumns="false"
                                                                    HorizontalAlign="left" AllowSorting="true" OnSorting="GridView_FaultyReplenishment1_Sorting">
                                                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                            SortExpression="title" />
                                                                        <asp:BoundField DataField="generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                                                            SortExpression="generated_at" />
                                                                        <asp:BoundField DataField="alert_msg" HeaderText="Message" SortExpression="alert_msg" />
                                                                    </Columns>
                                                                    <EmptyDataTemplate>
                                                                        No records found
                                                                    </EmptyDataTemplate>
                                                                </asp:GridView>
                                                            </ComponentArt:PageView>
                                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView34">
                                                                <asp:GridView ID="GridView_FaultyRep" runat="server" Width="30%" AutoGenerateColumns="false"
                                                                    HorizontalAlign="left" AllowSorting="true" OnSorting="GridView_FaultyReplenishment_Sorting"
                                                                    OnRowDataBound="GridView_FaultyRep_RowDataBound">
                                                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                            SortExpression="title" />
                                                                        <asp:BoundField DataField="status1" HeaderText="Type 1" ItemStyle-HorizontalAlign="Center"
                                                                            SortExpression="status1" />
                                                                        <asp:BoundField DataField="status2" HeaderText="Type 2" ItemStyle-HorizontalAlign="Center"
                                                                            SortExpression="status2" />
                                                                        <asp:BoundField DataField="status3" HeaderText="Type 3" ItemStyle-HorizontalAlign="Center"
                                                                            SortExpression="status3" />
                                                                        <asp:BoundField DataField="status4" HeaderText="Type 4" ItemStyle-HorizontalAlign="Center"
                                                                            SortExpression="status4" />
                                                                        <asp:BoundField DataField="trxn_datetime" HeaderText="Last Processed Transaction"
                                                                            ItemStyle-HorizontalAlign="Center" SortExpression="trxn_datetime" />
                                                                    </Columns>
                                                                    <EmptyDataTemplate>
                                                                        No records found
                                                                    </EmptyDataTemplate>
                                                                </asp:GridView>
                                                            </ComponentArt:PageView>
                                                        </ComponentArt:MultiPage>
                                                    </ComponentArt:PageView>
                                                </ComponentArt:MultiPage>
                                                <div id="Div5" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_Deposit">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>ATM Deposit Collection</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <ComponentArt:TabStrip ID="TabStrip_DepositCollection" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage5"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab10" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab11" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>
                                                <ComponentArt:MultiPage ID="MultiPage5" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView10">
                                                        <div style="height: 250px; width: 100%; overflow: auto">
                                                            <asp:GridView ID="GridView_DepositCollection_CurrentDay" runat="server" Width="100%"
                                                                AutoGenerateColumns="false" HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_Deposit_Sorting">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="generated_at" HeaderText="Generated at" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="generated_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                    <asp:BoundField DataField="alert_msg" HeaderText="Message" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView11">
                                                        <div style="height: 250px; width: 100%; overflow: auto">
                                                            <asp:GridView ID="GridView_DepositCollection_PreviousDay" runat="server" Width="100%"
                                                                AutoGenerateColumns="false" HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_Deposit_Sorting">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="generated_at" HeaderText="Generated at" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="generated_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                    <asp:BoundField DataField="alert_msg" HeaderText="Message" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                </ComponentArt:MultiPage>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_Cheque">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>ATM Cheque Collection</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <ComponentArt:TabStrip ID="TabStrip_ATMChequeCollection" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage4"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab8" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab9" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>
                                                <ComponentArt:MultiPage ID="MultiPage4" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView8">
                                                        <div style="height: 250px; width: 100%; overflow: auto">
                                                            <asp:GridView ID="GridView_ChequeCollection_CurrentDay" runat="server" Width="100%"
                                                                AllowSorting="true" OnSorting="GridView_Cheque_Sorting" AutoGenerateColumns="false"
                                                                HorizontalAlign="Center">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="generated_at" HeaderText="Generated at" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="generated_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                    <asp:BoundField DataField="alert_msg" HeaderText="Message" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView9">
                                                        <div style="height: 250px; width: 100%; overflow: auto">
                                                            <asp:GridView ID="GridView_ChequeCollection_PreviousDay" runat="server" Width="100%"
                                                                AllowSorting="true" OnSorting="GridView_Cheque_Sorting" AutoGenerateColumns="false"
                                                                HorizontalAlign="Center">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="generated_at" HeaderText="Generated at" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="generated_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                    <asp:BoundField DataField="alert_msg" HeaderText="Message" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                </ComponentArt:MultiPage>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_PurgeBinAlerts">
                                    <table width="60%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>Purge Bin Alerts</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <ComponentArt:TabStrip ID="TabStripTab_PurgeBin" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage3"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab6" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab7" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>
                                                <ComponentArt:MultiPage ID="MultiPage3" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView6">
                                                        <div style="height: 250px; width: 100%; overflow: auto">
                                                            <asp:GridView ID="GridView_PurgeBinAlerts_CurrentDay" runat="server" Width="100%"
                                                                AllowSorting="true" AutoGenerateColumns="false" HorizontalAlign="Center" OnSorting="Gridview_purgebin_sorting">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="generated_at" HeaderText="Generation Time" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="generated_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                    <asp:BoundField DataField="type1" HeaderText="Cassette 1" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="threshold1" HeaderText="Threshold 1" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type2" HeaderText="Cassette 2" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="threshold2" HeaderText="Threshold 2" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type3" HeaderText="Cassette 3" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="threshold3" HeaderText="Threshold 3" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type4" HeaderText="Cassette 4" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="threshold4" HeaderText="Threshold 4" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView7">
                                                        <div style="height: 250px; width: 100%; overflow: auto">
                                                            <asp:GridView ID="GridView_PurgeBinAlert_PreviousDay" runat="server" Width="100%"
                                                                AutoGenerateColumns="false" HorizontalAlign="Center" AllowSorting="true" OnSorting="Gridview_purgebin_sorting">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="generated_at" HeaderText="Generation Time" ItemStyle-HorizontalAlign="Center"
                                                                        DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                    <asp:BoundField DataField="type1" HeaderText="Cassette 1" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="threshold1" HeaderText="Threshold 1" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type2" HeaderText="Cassette 2" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="threshold2" HeaderText="Threshold 2" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type3" HeaderText="Cassette 3" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="threshold3" HeaderText="Threshold 3" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="type4" HeaderText="Cassette 4" ItemStyle-HorizontalAlign="Center" />
                                                                    <asp:BoundField DataField="threshold4" HeaderText="Threshold 4" ItemStyle-HorizontalAlign="Center" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                </ComponentArt:MultiPage>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView19">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>Purged Transaction Count </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <div>
                                                    <b>Following Grid display number of transactions in which no notes dispensed.</b>
                                                    <asp:GridView ID="GridView_PurgedCounter" runat="server" Width="15%" AutoGenerateColumns="false"
                                                        AllowSorting="true" OnSorting="GridView_PurgedCounter_Sorting">
                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                        <Columns>
                                                            <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                SortExpression="title" HeaderStyle-ForeColor="Black" />
                                                            <asp:BoundField DataField="count" HeaderText="No. Of Transactions" ItemStyle-HorizontalAlign="Center"
                                                                SortExpression="count" HeaderStyle-ForeColor="Black" />
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            No records found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView22">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>Counter Mismatch</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <ComponentArt:TabStrip ID="TabStrip4" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage9"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab14" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab15" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>
                                                <ComponentArt:MultiPage ID="MultiPage9" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView23">
                                                        <asp:GridView ID="GridView_CounterMismatch_CurrentDay" runat="server" Width="100%"
                                                            AutoGenerateColumns="false" HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_CounterMismatch_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            <Columns>
                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="title" />
                                                                <asp:BoundField DataField="generated_at" HeaderText="Generation Time" ItemStyle-HorizontalAlign="Center"
                                                                    DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                <asp:BoundField DataField="alert_msg" HeaderText="Message" ItemStyle-HorizontalAlign="Center" />
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </ComponentArt:PageView>
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView24">
                                                        <asp:GridView ID="GridView_CounterMismatch_PreviousDay" runat="server" Width="100%"
                                                            AutoGenerateColumns="false" HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_CounterMismatch_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            <Columns>
                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="title" />
                                                                <asp:BoundField DataField="generated_at" HeaderText="Generation Time" ItemStyle-HorizontalAlign="Center"
                                                                    DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                <asp:BoundField DataField="alert_msg" HeaderText="Message" ItemStyle-HorizontalAlign="Center" />
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </ComponentArt:PageView>
                                                </ComponentArt:MultiPage>
                                                <div id="Div4" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView20">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg" border="0">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center" colspan="4">
                                                <b>Daily Feed File Information</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" style="width: 250px">
                                                <div style="height: 250px; width: 100%; overflow: auto">
                                                    <center>
                                                        <b>ATMs Sending Same Opening Balance</b><br />
                                                        <br />
                                                        <asp:GridView ID="GridView_SameOpeningBalanceAsOfYesterday" runat="server" AutoGenerateColumns="false"
                                                            AllowSorting="true" OnSorting="GridView_DFFInfo_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <Columns>
                                                                <asp:TemplateField SortExpression="title" HeaderText="Title" HeaderStyle-ForeColor="Black">
                                                                    <ItemTemplate>
                                                                        <img src="../images/icon_ATM.jpg" />
                                                                        <%#Eval("title") %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </center>
                                                </div>
                                            </td>
                                            <td valign="top" style="width: 250px">
                                                <div style="height: 250px; width: 100%; overflow: auto">
                                                    <center>
                                                        <b>ATMs Sending 2</b><br />
                                                        <br />
                                                        <asp:GridView ID="GridView_Sending2" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                                            OnSorting="GridView_DFFInfo_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <Columns>
                                                                <asp:TemplateField SortExpression="title" HeaderText="Title" HeaderStyle-ForeColor="Black">
                                                                    <ItemTemplate>
                                                                        <img src="../images/icon_ATM.jpg" />
                                                                        <%#Eval("title") %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </center>
                                                </div>
                                            </td>
                                            <td valign="top" style="width: 250px">
                                                <div style="height: 250px; width: 100%; overflow: auto">
                                                    <center>
                                                        <b>ATMs Not Sending Any Data</b><br />
                                                        <br />
                                                        <asp:GridView ID="GridView_NotInDFF" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                                                            OnSorting="GridView_DFFInfo_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <Columns>
                                                                <asp:TemplateField SortExpression="title" HeaderText="Title" HeaderStyle-ForeColor="Black">
                                                                    <ItemTemplate>
                                                                        <img src="../images/icon_ATM.jpg" />
                                                                        <%#Eval("title") %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </center>
                                                </div>
                                            </td>
                                            <td valign="top" style="width: 250px">
                                                <div style="height: 250px; width: 100%; overflow: auto">
                                                    <center>
                                                        <b>ATMs Having Inconsistent Data</b><br />
                                                        <br />
                                                        <asp:GridView ID="GridView_InconsistentData" runat="server" AutoGenerateColumns="false"
                                                            AllowSorting="true" OnSorting="GridView_DFFInfo_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <Columns>
                                                                <asp:TemplateField SortExpression="title" HeaderText="Title" HeaderStyle-ForeColor="Black">
                                                                    <ItemTemplate>
                                                                        <img src="../images/icon_ATM.jpg" />
                                                                        <%#Eval("title") %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </center>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_Communication">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>ATM Cash Level Download Summary</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <div>
                                                    <asp:GridView ID="GridView_DownloadSummary" runat="server" Width="15%" AutoGenerateColumns="false">
                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                        <Columns>
                                                            <asp:BoundField DataField="Activity" HeaderText="Activity" />
                                                            <asp:BoundField DataField="Count" HeaderText="Count" ItemStyle-HorizontalAlign="Right" />
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            No records found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_Alerts">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>Top 20 Alerts</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <ComponentArt:TabStrip ID="TabStrip2" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage6"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab12" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab13" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day"
                                                            AutoPostBackOnSelect="true">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>
                                                <ComponentArt:MultiPage ID="MultiPage6" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView12">
                                                        <asp:GridView ID="GridView_Alerts_CurrentDay" runat="server" Width="100%" AutoGenerateColumns="false"
                                                            HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_Alerts_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            <Columns>
                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="title" />
                                                                <asp:BoundField DataField="generated_at" HeaderText="Generation Time" ItemStyle-HorizontalAlign="Center"
                                                                    DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                <asp:BoundField DataField="alert_type_name" HeaderText="Alert Type" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="alert_type_name" />
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </ComponentArt:PageView>
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView13">
                                                        <asp:GridView ID="GridView_Alerts_PreviousDay" runat="server" Width="100%" AutoGenerateColumns="false"
                                                            HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_Alerts_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            <Columns>
                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="title" />
                                                                <asp:BoundField DataField="generated_at" HeaderText="Generation Time" ItemStyle-HorizontalAlign="Center"
                                                                    DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                                                                <asp:BoundField DataField="alert_type_name" HeaderText="Alert Type" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="alert_type_name" />
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </ComponentArt:PageView>
                                                </ComponentArt:MultiPage>
                                                <div id="Div_AlertsSummary" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView14">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>Dead ATMs</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <%--<ComponentArt:TabStrip ID="TabStrip3" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage6"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab15" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab16" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>--%>
                                                <ComponentArt:MultiPage ID="MultiPage7" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView15">
                                                        <div style="height: 330px; width: 100%; overflow: auto">
                                                            <asp:GridView ID="GridView_DeadATM" runat="server" Width="50%" AutoGenerateColumns="false"
                                                                HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_DeadATM_Sorting">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="last_heart_beat_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                    <%--<ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView16">
                                                        <asp:GridView ID="GridView_DeadATMPreviousDay" runat="server" Width="100%" AutoGenerateColumns="false"
                                                            HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_Alerts_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            <Columns>
                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="title" />
                                                                <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="last_heart_beat_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </ComponentArt:PageView>--%>
                                                </ComponentArt:MultiPage>
                                                <div id="Div1" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView17">
                                    <table width="100%" style="background: ../images/PatternHomeBG.jpg">
                                        <tr>
                                            <td style="background-color: #54b948; color: white; text-align: center">
                                                <b>DateTime Sync</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <%--<ComponentArt:TabStrip ID="TabStrip3" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage6"
                                                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                                    Width="100%" OnItemSelected="TabStrip_Summary_ItemSelected" AutoPostBackOnSelect="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                            LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                            LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                            HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                        <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                            LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                            RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                            RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                    </ItemLooks>
                                                    <Tabs>
                                                        <ComponentArt:TabStripTab ID="TabStripTab15" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Current Day">
                                                        </ComponentArt:TabStripTab>
                                                        <ComponentArt:TabStripTab ID="TabStripTab16" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Previous Day">
                                                        </ComponentArt:TabStripTab>
                                                    </Tabs>
                                                </ComponentArt:TabStrip>--%>
                                                <ComponentArt:MultiPage ID="MultiPage8" CssClass="MultiPage" runat="server">
                                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView18">
                                                        <div style="height: 330px; width: 100%; overflow: auto">
                                                            <asp:GridView ID="GridView_DatetimeSync" runat="server" Width="50%" AutoGenerateColumns="false"
                                                                HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_DatetimeSync_Sorting">
                                                                <AlternatingRowStyle BackColor="#eff0ef" />
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="title" />
                                                                    <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="status" />
                                                                    <asp:BoundField DataField="last_invoked" HeaderText="Last Invoked" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="last_invoked" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                    <asp:BoundField DataField="end_time" HeaderText="End Time" ItemStyle-HorizontalAlign="Center"
                                                                        SortExpression="end_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                </Columns>
                                                                <EmptyDataTemplate>
                                                                    No records found
                                                                </EmptyDataTemplate>
                                                            </asp:GridView>
                                                        </div>
                                                    </ComponentArt:PageView>
                                                    <%--<ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView16">
                                                        <asp:GridView ID="GridView_DeadATMPreviousDay" runat="server" Width="100%" AutoGenerateColumns="false"
                                                            HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_Alerts_Sorting">
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            <Columns>
                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="title" />
                                                                <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                                                    SortExpression="last_heart_beat_at" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </ComponentArt:PageView>--%>
                                                </ComponentArt:MultiPage>
                                                <div id="Div3" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                            </ComponentArt:MultiPage>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
