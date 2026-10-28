<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSettlementData.aspx.cs"
    Inherits="CCMSUI.CCMS.EditSettlementData" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>EJ Log Viewer</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        function updateTotal(denom1, denom2, denom3, denom4, counter) {
            
            if (counter == 1) {
                document.getElementById('<%=Label_CITTotalAdded.ClientID %>').innerHTML =
            document.getElementById('<%=TextBox_CITAdded1.ClientID %>').value * denom1 +
            document.getElementById('<%=TextBox_CITAdded2.ClientID %>').value * denom2 +
            document.getElementById('<%=TextBox_CITAdded3.ClientID %>').value * denom3 +
            document.getElementById('<%=TextBox_CITAdded4.ClientID %>').value * denom4;
            
            }
            else if (counter == 2) {
                document.getElementById('<%=Label_CITTotalDispensed.ClientID %>').innerHTML =
            document.getElementById('<%=TextBox_CITDispensed1.ClientID %>').value * denom1 +
            document.getElementById('<%=TextBox_CITDispensed2.ClientID %>').value * denom2 +
            document.getElementById('<%=TextBox_CITDispensed3.ClientID %>').value * denom3 +
            document.getElementById('<%=TextBox_CITDispensed4.ClientID %>').value * denom4;

            }
            else if (counter == 3) {
                document.getElementById('<%=Label_CITTotalReturned.ClientID %>').innerHTML =
            document.getElementById('<%=TextBox_CITReturned1.ClientID %>').value * denom1 +
            document.getElementById('<%=TextBox_CITReturned2.ClientID %>').value * denom2 +
            document.getElementById('<%=TextBox_CITReturned3.ClientID %>').value * denom3 +
            document.getElementById('<%=TextBox_CITReturned4.ClientID %>').value * denom4;

            }

            else if (counter == 4) {
                document.getElementById('<%=Label_CITTotalRejected.ClientID %>').innerHTML =
            document.getElementById('<%=TextBox_CITRejected1.ClientID %>').value * denom1 +
            document.getElementById('<%=TextBox_CITRejected2.ClientID %>').value * denom2 +
            document.getElementById('<%=TextBox_CITRejected3.ClientID %>').value * denom3 +
            document.getElementById('<%=TextBox_CITRejected4.ClientID %>').value * denom4;

            }


        }
    </script>
</head>
<body style="margin-left: 0; margin-top: 0; margin-right: 0" onload="hideProgressBar();">
    <form id="frmEJLogViewer" method="post" runat="server">
    <table id="maintable" style="width: 100%">
        <tr>
            <td class="PageHeading" style="vertical-align: middle">
                Edit Settlement
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            ATM Site And Number
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_ATMSiteAndNumber" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    
                    <tr>
                        <td>
                            Replenishment Date
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_ReplenishmentDate" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            GL Number
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_GLNo" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Seal Number
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_SealNo" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" cellpadding="0" cellspacing="0">
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
                                    <ComponentArt:TabStripTab ID="TabStripTab_RecentActivity" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Counters">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_LowBalance" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Comments">
                                    </ComponentArt:TabStripTab>
                                </Tabs>
                            </ComponentArt:TabStrip>
                            <ComponentArt:MultiPage ID="MultiPage_Summary" CssClass="MultiPage" runat="server">
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_RecentActivity">
                                    <table border="0" width="36%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td id="tableHeaderRecentActivity" runat="server" style="background-color: #54b948;
                                                color: white; text-align: center" colspan="6">
                                                <b>Replenishment Details</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: #54b948; color: White" width="140px">
                                                <b>Currency Denomination Type</b>
                                            </td>
                                            <td style="background-color: #54b948; color: White" width="70px">
                                                <b>Replenished<br />
                                                    Quantity</b>
                                                    
                                            </td>
                                            <td style="background-color: #54b948; color: White" width="70px">
                                                <b>Dispensed<br />
                                                    Quantity</b>
                                            </td>
                                            <td style="background-color: #54b948; color: White" width="70px">
                                                <b>Returned<br />
                                                    Quantity</b>
                                            </td>
                                            <td style="background-color: #54b948; color: white" width="70px">
                                                <b>Rejected<br />
                                                    Quantity</b>
                                            </td>
                                        </tr>
                                        <tr  >
                                            <td  style="background-color: Gray; color: White">
                                                <b>
                                                    <asp:Label ID="Label_ctype1" runat="server" Text="Type 1"></asp:Label></b>
                                            </td>
                                            <td style="background-color:Pink" >
                                                <asp:TextBox ID="TextBox_CITAdded1" runat="server" CssClass="textbox" Width="70px"
                                                 BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITDispensed1" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITReturned1" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td >
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITRejected1" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td  style="background-color: Gray; color: White">
                                                <b>
                                                    <asp:Label ID="Label_ctype2" runat="server" Text="Type 2"></asp:Label></b>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITAdded2" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITDispensed2" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITReturned2" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITRejected2" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td  style="background-color: Gray; color: White">
                                                <b>
                                                    <asp:Label ID="Label_ctype3" runat="server" Text="Type 3"></asp:Label></b>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITAdded3" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITDispensed3" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITReturned3" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITRejected3" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td  style="background-color: Gray; color: White">
                                                <b>
                                                    <asp:Label ID="Label_ctype4" runat="server" Text="Type 4"></asp:Label></b>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITAdded4" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITDispensed4" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITReturned4" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                            <td style="background-color:Pink">
                                                <asp:TextBox ID="TextBox_CITRejected4" runat="server" CssClass="textbox" Width="70px"
                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td  style="background-color: Gray; color: White">
                                                <b>Total Amount</b>
                                            </td>
                                            <td style="background-color:Gray">
                                                <asp:Label ID="Label_CITTotalAdded" runat="server"  Width="70px"
                                                    CssClass="textbox" BackColor="Gray" ForeColor="White"></asp:Label>
                                            </td>
                                            <td style="background-color:Gray">
                                                <asp:Label ID="Label_CITTotalDispensed" runat="server"  CssClass="textbox"
                                                    Width="70px" BackColor="Gray" ForeColor="White"></asp:Label>
                                            </td>
                                            <td style="background-color:Gray">
                                                <asp:Label ID="Label_CITTotalReturned" runat="server"  CssClass="textbox"
                                                    Width="70px" BackColor="Gray" ForeColor="White"></asp:Label>
                                            </td>
                                            <td style="background-color:Gray">
                                                <asp:Label ID="Label_CITTotalRejected" runat="server"  CssClass="textbox"
                                                    Width="70px" BackColor="Gray" ForeColor="White"></asp:Label>
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
                    <tr>
                        <td>
                            <asp:Button ID="Button_Update" runat="server" OnClick="Button_Update_Click" CssClass="button"
                                Text="Update" Width="71px" />&nbsp;
                            <asp:Button ID="Button_cancel" runat="server" CssClass="button" Text="Cancel" Width="71px"
                                OnClick="Button_cancel_click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
