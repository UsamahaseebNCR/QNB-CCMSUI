<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Investigation.aspx.cs"
    Inherits="CCMSUI.CCMS.Investigation" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/theme.css" rel="stylesheet" />
    <link href="../content/tabStyle.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td class="PageHeading" style="vertical-align: middle">
                 <%=Request.QueryString["e"]==null?"Create Investigation":"View/Update Investigation" %>   
                </td>
            </tr>
            <tr style="height: 20px">
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <ComponentArt:TabStrip ID="TabStrip_Summary" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                        DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                        DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_Summary"
                        runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                        ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                        Width="100%">
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
                            <ComponentArt:TabStripTab ID="TabStripTab_InvestigationDetails" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Investigation"
                                Width="100%">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab_Comments" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Comments">
                            </ComponentArt:TabStripTab>
                        </Tabs>
                    </ComponentArt:TabStrip>
                    <ComponentArt:MultiPage ID="MultiPage_Summary" CssClass="MultiPage" runat="server"
                        Width="100%">
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_RecentActivity" 
                            Width="100%">
                            <table width="100%">
                                <tr>
                                    <td>
                                        Selected ATM
                                    </td>
                                    <td>
                                        <asp:Label ID="Label_atm" runat="server" EnableViewState="False" Font-Bold="true"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Order Number
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_OrderNo" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Investigation Reason
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DropDownList_Reason" runat="server" CssClass="dropDown">
                                            <asp:ListItem Text="Order was missed"></asp:ListItem>
                                            <asp:ListItem Text="Order was replenished delayed/earlier"></asp:ListItem>
                                            <asp:ListItem Text="Replenished mode was different"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Status
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DropDownList_Status" runat="server" CssClass="dropDown">
                                            <asp:ListItem Text="Open"></asp:ListItem>
                                            <asp:ListItem Text="Closed"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                
                                <tr>
                                    <td valign="top">
                                        Investigation Summary
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_Summary" runat="server" TextMode="MultiLine" Width="500"
                                            Height="70" CssClass="textbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Button ID="Button_Submit" Text="Update Investigation" runat="server" CssClass="button"
                                            OnClick="Button_Submit_Click" />
                                        &nbsp;
                                        <asp:Button ID="Button_Close" Text="Close" runat="server" CssClass="button"
                                            OnClick="Button_Close_Click" />
                                        
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1">
                            <table width="100%" border="0">
                                <tr>
                                    <td width="100%">
                                        <div style="height: 300px; overflow: auto">
                                            <asp:Literal ID="Literal_Comments" runat="server"></asp:Literal>
                                            <%--<asp:Label ID="Label_Reason" runat="server" Height="117px" BackColor="Info"
                                                    Width="680px"></asp:Label>--%>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="TextBox_NewComments" runat="server" Width="520px" TextMode="MultiLine"
                                                        CssClass="textbox" Height="150px"></asp:TextBox>
                                                </td>
                                                <td valign="top">
                                                    <asp:Button ID="Button_AddNew" runat="server" Text="Add New Comments" OnClick="Button_AddNewComments_click"
                                                        CssClass="button" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                    </ComponentArt:MultiPage>
                </td>
            </tr>
        </table>
        <input type="button" runat="server" id="refresh" style="wdth: 0px; height: 0px; visibility: hidden" />
    </div>
    </form>
</body>
</html>
