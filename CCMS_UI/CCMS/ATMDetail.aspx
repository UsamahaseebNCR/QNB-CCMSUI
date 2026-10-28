<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ATMDetail.aspx.cs" Inherits="CCMSUI.CCMS.ATMDetail" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register TagPrefix="DBWC" Namespace="DBauer.Web.UI.WebControls" Assembly="DBauer.Web.UI.WebControls.HierarGrid" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - ATM Detail</title>
    <script src="../Content/js/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script src="../content/diQuery-collapsiblePanel.js" type="text/javascript"></script>
    <link href="../content/diQuery-collapsiblePanel.css" rel="stylesheet" type="text/css" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $(".collapsibleContainer").collapsiblePanel();
        });
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table class="mainbox" cellspacing="0" cellpadding="0">
            <tr>
                <td class="PageHeading" align="left">
                    ATM Device Status
                </td>
            </tr>
            <tr>
                <td class="ValidationText">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <asp:ValidationSummary ID="ValidationSummary_ATM" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="ConfirmationTextClass">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr style="width: 100%">
                <td>
                    <table width="100%">
                        <tr>
                            <td>
                                <ComponentArt:TabStrip ID="TabStrip_ATMDetails" ScrollingEnabled="true" ScrollLeftLookId="ScrollItem"
                                    EnableTheming="true" ScrollRightLookId="ScrollItem" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_Operations"
                                    Width="100%" runat="server">
                                    <ItemLooks>
                                        <%-- <ComponentArt:ItemLook LookId="ScrollItem" CssClass="ScrollItem" HoverCssClass="ScrollItemHover"
                                                LabelPaddingLeft="5" LabelPaddingRight="5" LabelPaddingTop="0" LabelPaddingBottom="0" />--%>
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
                                        <ComponentArt:TabStripTab ID="TabStripTab_General" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="General" Width="100%">
                                        </ComponentArt:TabStripTab>
                                        <ComponentArt:TabStripTab ID="TabStripTab_CCMS" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Device" Width="100%">
                                        </ComponentArt:TabStripTab>
                                        <ComponentArt:TabStripTab ID="TabStripTab_EJ" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Cash Counters" Width="100%">
                                        </ComponentArt:TabStripTab>
                                        <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Supplies" Width="100%">
                                        </ComponentArt:TabStripTab>
                                    </Tabs>
                                </ComponentArt:TabStrip>
                                <ComponentArt:MultiPage ID="MultiPage_Operations" CssClass="MultiPage" runat="server"
                                    Width="100%">
                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_RecentActivity"
                                        Width="100%">
                                        <table style="width: 100%; padding: 0px; margin: 0px;" class="SEARCHBOX">
                                            <tr>
                                                <td>
                                                    <table style="width: 100%">
                                                        <tr>
                                                            <td style="width: 15%;">
                                                                Title
                                                            </td>
                                                            <td style="width: 35%">
                                                                <asp:Label ID="Label_Title" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 15%;">
                                                                IP Address
                                                            </td>
                                                            <td style="width: 35%">
                                                                <asp:Label ID="Label_IP" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Type
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_Type" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Region
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_Region" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Aggregate State
                                                            </td>
                                                            <td>
                                                                <asp:Image ID="img_aggregate" runat="server" ImageUrl="" Style="margin-left: 0px" />&nbsp;
                                                                <asp:Label ID="Label_AggregateState" runat="server" Font-Bold="true"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Current Mode
                                                            </td>
                                                            <td>
                                                                <asp:Image ID="Img_mode" runat="server" ImageUrl="~/images/offline1.jpg" Style="margin-left: 0px"
                                                                    Height="16px" Width="16px" />&nbsp;
                                                                <asp:Label ID="Label_CurrentMode" runat="server" Font-Bold="true" Style="text-align: center"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Calculated State
                                                            </td>
                                                            <td>
                                                                <asp:Image ID="img_calcstate" runat="server" ImageUrl="" Style="margin-left: 0px" />&nbsp;
                                                                <asp:Label ID="Label_CalcState" runat="server" Font-Bold="true"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Supplies
                                                            </td>
                                                            <td>
                                                                <asp:Image ID="img_supply" runat="server" ImageUrl="" Style="margin-left: 0px" Height="16px" />&nbsp;
                                                                <asp:Label ID="Label_SupplyState" runat="server" Font-Bold="true"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Last Scan Time
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_LastScanTime" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Last Boot Time
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_LastBootTime" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Discovered On
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_DiscoveredOn" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Last Current Mode
                                                            </td>
                                                            <td>
                                                                <asp:Image ID="img_last_currentmode" runat="server" ImageUrl="" Style="margin-left: 0px"
                                                                    Height="16px" />&nbsp;
                                                                <asp:Label ID="Label_Last_CurrentMode" runat="server" Font-Bold="True" Style="text-align: center"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </ComponentArt:PageView>
                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1" Width="100%">
                                        <div class="collapsibleContainer" title='Devices'>
                                            <table cellspacing="0" cellpadding="0">
                                                <tr style="visibility: visible">
                                                    <td>
                                                        Show devices with status :
                                                        <asp:RadioButton ID="rdball" runat="server" Text=" All " AutoPostBack="True" Checked="true"
                                                            GroupName="dStatus" OnCheckedChanged="rdball_CheckedChanged" />
                                                        <asp:RadioButton ID="Rdbwarning" runat="server" Text=" Warning or Fatal  " AutoPostBack="True"
                                                            GroupName="dStatus" OnCheckedChanged="Rdbwarning_CheckedChanged" />
                                                        <asp:RadioButton ID="rdbfatal" runat="server" Text=" Fatal " AutoPostBack="True"
                                                            GroupName="dStatus" OnCheckedChanged="rdbfatal_CheckedChanged" />
                                                    </td>
                                                </tr>
                                                <tr style="height: 10px;">
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr style="width: 100%">
                                                    <td>
                                                        <DBWC:HierarGrid ID="Grid_Devices" runat="server" AutoGenerateColumns="False" Width="100%"
                                                            AllowSorting="true" OnSortCommand="GridView_Sorting" AllowPaging="True" OnPageIndexChanged="GridView_PageIndexChanging"
                                                            PageSize="40" BorderStyle="None" ShowHeader="True" ShowFooter="false" TemplateCachingBase="Tablename"
                                                            LoadControlMode="UserControl" TemplateDataMode="Table" OnTemplateSelection="Grid_Devices_TemplateSelection"
                                                            OnItemDataBound="Grid_Devices_ItemDataBound" HeaderStyle-CssClass="HeadingCell"
                                                            HeaderStyle-HorizontalAlign="Center">
                                                            <AlternatingItemStyle BackColor="#eff0ef" />
                                                            <%--<HeaderStyle CssClass="GridHeadingCellClass" ForeColor="#ffffff" HorizontalAlign="Center" />--%>
                                                            <PagerStyle CssClass="GridFooterCellClass" Position="Bottom" Mode="NumericPages"
                                                                PrevPageText="<" NextPageText=">" />
                                                            <AlternatingItemStyle BackColor="#eff0ef" />
                                                            <Columns>
                                                                <asp:BoundColumn DataField="device_id" HeaderText="Id" Visible="false" ItemStyle-Width="5px">
                                                                    <ItemStyle Width="5px"></ItemStyle>
                                                                </asp:BoundColumn>
                                                                <asp:BoundColumn DataField="device_name" HeaderText="Device Name" SortExpression="device_name"
                                                                    ItemStyle-Width="300px"></asp:BoundColumn>
                                                                <asp:BoundColumn DataField="vendor_name" HeaderText="Vendor" SortExpression="vendor_name"
                                                                    Visible="false" ItemStyle-Width="0px"></asp:BoundColumn>
                                                                <asp:BoundColumn DataField="device_state_str" HeaderText="Status" SortExpression="device_state_str"
                                                                    ItemStyle-Width="100px"></asp:BoundColumn>
                                                                <asp:TemplateColumn ItemStyle-Width="15px">
                                                                    <ItemTemplate>
                                                                        <%--<div class="<%#DataBinder.Eval(Container.DataItem, "device_state_str").ToString().Replace(" ","-").ToLower()%>">--%>
                                                                        <div>
                                                                            <%--<asp:Image runat="server" ImageUrl='<%#DataBinder.Eval(Container.DataItem, "device_state_str").ToString().Replace(" ","-").ToLower().Equals(string.Empty)?"../images/"+"default.gif":"../images/"+DataBinder.Eval(Container.DataItem, "device_state_str").ToString().Replace(" ","-").ToLower()+".gif"%>' />--%>
                                                                            <asp:Image ID="Image1" runat="server" />
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="bold" />
                                                                </asp:TemplateColumn>
                                                                <asp:BoundColumn DataField="device_state_desc" HeaderText="Description" ItemStyle-Width="505px">
                                                                </asp:BoundColumn>
                                                            </Columns>
                                                            <HeaderStyle CssClass="HeadingCell1" />
                                                        </DBWC:HierarGrid>
                                                    </td>
                                                </tr>
                                                <tr style="height: 10px;">
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </ComponentArt:PageView>
                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2" Width="100%">
                                        <div class="collapsibleContainer" title='Cash Status'>
                                            <table class="PageLayoutClass" cellspacing="0" cellpadding="0">
                                                <tr style="width: 100%">
                                                    <td>
                                                        <asp:GridView ID="GridView_CashState" runat="server" AutoGenerateColumns="False"
                                                            Width="100%" ShowFooter="True" OnRowCreated="GridView_CashState_RowCreated" OnRowDataBound="GridView_CashState_RowDataBound">
                                                            <HeaderStyle CssClass="HeadingCell" />
                                                            <%--<HeaderStyle CssClass="GridHeadingCellClass" ForeColor="#ffffff" />--%>
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <EmptyDataTemplate>
                                                                No data found!</EmptyDataTemplate>
                                                            <PagerStyle CssClass="GridFooterCellClass" />
                                                            <Columns>
                                                                <asp:BoundField DataField="cassette_name" HeaderText="Cassette" SortExpression="cassette_name"
                                                                    ItemStyle-Width="22%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                                <asp:BoundField DataField="code" HeaderText="Currency" SortExpression="code" ItemStyle-Width="15%"
                                                                    HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                                <asp:BoundField DataField="currency_denomination" HeaderText="Denomination" SortExpression="currency_denomination"
                                                                    ItemStyle-Width="12%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                                <asp:BoundField DataField="notes_remaining" HeaderText="Remaining" SortExpression="notes_remaining"
                                                                    ItemStyle-Width="12%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                                <asp:BoundField DataField="notes_rejected" HeaderText="Rejected" SortExpression="notes_rejected"
                                                                    ItemStyle-Width="12%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                                <asp:BoundField DataField="notes_dispensed" HeaderText="Dispensed" SortExpression="notes_dispensed"
                                                                    ItemStyle-Width="12%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                                <asp:BoundField DataField="notes_jammed" HeaderText="Jammed" SortExpression="notes_jammed"
                                                                    ItemStyle-Width="12%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr style="height: 10px;">
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </ComponentArt:PageView>
                                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView3" Width="100%">
                                        <div class="collapsibleContainer" title='Supply Status'>
                                            <table class="PageLayoutClass" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td>
                                                        <asp:GridView ID="GridView_SupplyState" runat="server" AutoGenerateColumns="False"
                                                            Width="100%" >
                                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                                            <HeaderStyle CssClass="HeadingCell" />
                                                            <%--<HeaderStyle CssClass="GridHeadingCellClass" ForeColor="#ffffff" />--%>
                                                            <EmptyDataTemplate>
                                                                No data found!</EmptyDataTemplate>
                                                            <PagerStyle CssClass="GridFooterCellClass" />
                                                            <Columns>
                                                                <asp:BoundField DataField="device_name" HeaderText="Supply Name" SortExpression="device_name"
                                                                    ItemStyle-Width="22%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                                <asp:BoundField DataField="device_state" HeaderText="Status" SortExpression="device_state"
                                                                    ItemStyle-Width="22%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                                <asp:BoundField DataField="device_state_desc" HeaderText="Description" SortExpression="device_state_desc"
                                                                    ItemStyle-Width="22%" HeaderStyle-CssClass="HeadingCell1"></asp:BoundField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr style="height: 10px;">
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </ComponentArt:PageView>
                                </ComponentArt:MultiPage>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="height: 10px;">
                <td>
                </td>
            </tr>
        </table>
        <table class="PageLayoutClass" cellspacing="0" cellpadding="0">
            <tr>
                <td class="SearchButtonsPaneClass">
                    <asp:Button ID="Button_back" runat="server" CssClass="ButtonClass" Text="Back" OnClick="Button_back_Click" />
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" name="refresh" style="width: 0px;
        height: 0px; visibility: hidden" />
    </form>
</body>
</html>
