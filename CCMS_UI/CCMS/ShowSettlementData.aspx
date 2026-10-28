<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowSettlementData.aspx.cs"
    Inherits="CCMSUI.CCMS.ShowSettlementData" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>EJ Log Viewer</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        function Checbox_Verification_Click() {
            if (document.getElementById('Checkbox_Verification').checked)
                document.getElementById('<%=TextBox_Inventory.ClientID%>').value = 'Check Balance';
            else
                document.getElementById('<%=TextBox_Inventory.ClientID%>').value = '';
        }
    </script>
</head>
<body style="margin-left: 0; margin-top: 0; margin-right: 0" onload="hideProgressBar();">
    <form id="frmEJLogViewer" method="post" runat="server">
    <table id="maintable" style="width: 100%">
        <tr>
            <td class="ConfirmationText" align="center" colspan="2">
                <%=Session[SessionVars.notice.ToString()]%>
                <%Session[SessionVars.notice.ToString()] = "";%>
                <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td class="PageHeading" style="vertical-align: middle">
                Settlement
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            Title
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_Title" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Location
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_ATMLocation" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            CIT Atm Title
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_ATMCITTitle" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            ATM Open Date
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_ATMOpenDate" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            ATM Open Time
                        </td>
                        <td>
                            <asp:Label ID="Label_ATMOpenTime" runat="server" Font-Bold="true"></asp:Label>
                            <asp:TextBox ID="TextBox_ATMOpenTimeCorrection" runat="server" Text="" Visible="false"
                                Width="30px" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            ATM Closed Date
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_ATMClosedDate" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            ATM Closed Time
                        </td>
                        <td>
                            <asp:Label ID="Label_ATMClosedTime" runat="server" Font-Bold="true"></asp:Label>
                            <asp:TextBox ID="TextBox_ATMClosedTimeCorrection" runat="server" Text="" Visible="false"
                                Width="30px" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            Verification Required
                        </td>
                        <td>
                        <input type="checkbox" id="Checkbox_Verification" onclick="Checbox_Verification_Click();" />

                      
                        </td>
                    </tr>

                    <%--                    <tr>
                        <td>
                            Seal Number
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_SealNo" runat="server"></asp:Label>
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
                    --%>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td valign="top">
                            <componentart:tabstrip id="TabStrip_Summary" cssclass="TopGroup" defaultitemlookid="DefaultTabLook"
                                defaultselecteditemlookid="SelectedTabLook" defaultdisableditemlookid="DisabledTabLook"
                                defaultgrouptabspacing="1px" imagesbaseurl="../images/Tabimages/" multipageid="MultiPage_Summary"
                                runat="server" scrolldownlookid="DefaultTabLook" scrollleftlookid="DefaultTabLook"
                                scrollrightlookid="DefaultTabLook" scrolluplookid="DefaultTabLook" topgrouptabspacing="1px"
                                width="100%">
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
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Counters" Width="100%">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_LowBalance" runat="server" ChildSelectedLook-LeftIconVisibility="Always" 
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Comments" Width="100%" Visible="False" >
                                    </ComponentArt:TabStripTab>

                                    <ComponentArt:TabStripTab ID="TabStripTab_OtherDetails" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Other Details" Width="100%">
                                    </ComponentArt:TabStripTab>
                                </Tabs>
                            </componentart:tabstrip>
                            <componentart:multipage id="MultiPage_Summary" cssclass="MultiPage" runat="server"
                                width="100%">
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_RecentActivity" Width="100%">
                                    <table runat="server" id="oldTable">
                                        <tr>
                                            <td valign="top">
                                                <span style="font-size: large;"><b>CCMS</b></span>
                                                <table border="0" width="36%" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td id="Td1" runat="server" style="background-color: #54b948; color: white; text-align: center"
                                                            colspan="6">
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
                                                    <tr>
                                                        <td style="background-color: Gray; color: White">
                                                            <b>
                                                                <asp:Label ID="Label_type1" runat="server" Text="Type 1"></asp:Label></b>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Added1" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Dispensed1" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Returned11" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:TextBox ID="Label_Remaining1" runat="server" CssClass="textbox" Width="70px"
                                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="background-color: Gray; color: White">
                                                            <b>
                                                                <asp:Label ID="Label_type2" runat="server" Text="Type 2"></asp:Label></b>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Added2" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Dispensed2" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Returned21" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:TextBox ID="Label_Remaining2" runat="server" CssClass="textbox" Width="70px"
                                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="background-color: Gray; color: White">
                                                            <b>
                                                                <asp:Label ID="Label_type3" runat="server" Text="Type 3"></asp:Label></b>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Added3" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Dispensed3" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Returned31" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:TextBox ID="Label_Remaining3" runat="server" CssClass="textbox" Width="70px"
                                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="background-color: Gray; color: White">
                                                            <b>
                                                                <asp:Label ID="Label_type4" runat="server" Text="Type 4"></asp:Label></b>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Added4" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Dispensed4" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:Label ID="Label_Returned41" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Pink">
                                                            <asp:TextBox ID="Label_Remaining4" runat="server" CssClass="textbox" Width="70px"
                                                                BackColor="Pink" BorderColor="Pink"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="background-color: Gray; color: White">
                                                            <b>Total Amount</b>
                                                        </td>
                                                        <td style="background-color: Gray; color: White">
                                                            <asp:Label ID="Label_TotalAdded" runat="server"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Gray">
                                                            <asp:Label ID="Label_TotalDispensed" runat="server" CssClass="textbox" Width="70px"
                                                                BackColor="Gray" ForeColor="White"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Gray">
                                                            <asp:Label ID="Label_TotalReturned" runat="server" CssClass="textbox" Width="70px"
                                                                BackColor="Gray" ForeColor="White"></asp:Label>
                                                        </td>
                                                        <td style="background-color: Gray">
                                                            <asp:Label ID="Label_TotalRemaining" runat="server" CssClass="textbox" Width="70px"
                                                                BackColor="Gray" ForeColor="White"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td valign="top">
                                                <table>
                                                    <tr>
                                                        <td valign="top">
                                                            <span style="font-size: large"><b>CIT</b></span>
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
                                                                <tr>
                                                                    <td style="background-color: Gray; color: White">
                                                                        <b>
                                                                            <asp:Label ID="Label_ctype1" runat="server" Text="Type 1"></asp:Label></b>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITAdded1Obs" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                            BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITDispensed11" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITReturned11" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITRejected11" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="background-color: Gray; color: White">
                                                                        <b>
                                                                            <asp:Label ID="Label_ctype2" runat="server" Text="Type 2"></asp:Label></b>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITAdded2Obs" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                            BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITDispensed2" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITReturned2" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITRejected21" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="background-color: Gray; color: White">
                                                                        <b>
                                                                            <asp:Label ID="Label_ctype3" runat="server" Text="Type 3"></asp:Label></b>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITAdded3Obs" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                            BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITDispensed3" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITReturned3" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITRejected31" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="background-color: Gray; color: White">
                                                                        <b>
                                                                            <asp:Label ID="Label_ctype4" runat="server" Text="Type 4"></asp:Label></b>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITAdded4Obs" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                            BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITDispensed4" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITReturned4" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Pink">
                                                                        <asp:Label ID="Label_CITRejected41" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="background-color: Gray; color: White">
                                                                        <b>Total Amount</b>
                                                                    </td>
                                                                    <td style="background-color: Gray">
                                                                        <asp:Label ID="Label_CITTotalAdded" runat="server" Width="70px" CssClass="textbox"
                                                                            BackColor="Gray" ForeColor="White"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Gray">
                                                                        <asp:Label ID="Label_CITTotalDispensed" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Gray" ForeColor="White"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Gray">
                                                                        <asp:Label ID="Label_CITTotalReturned" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Gray" ForeColor="White"></asp:Label>
                                                                    </td>
                                                                    <td style="background-color: Gray">
                                                                        <asp:Label ID="Label_CITTotalRejected" runat="server" CssClass="textbox" Width="70px"
                                                                            BackColor="Gray" ForeColor="White"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <div>
                                                                <table>
                                                                    <tr>
                                                                        <td style="font-size: large">
                                                                            ATM Status:
                                                                        </td>
                                                                        <td style="font-size: large">
                                                                            <b>
                                                                                <asp:Label ID="Label_CITATMStatus" runat="server"></asp:Label></b>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        
                                    </table>
                                   <table border="0" style="font-family: Comic Sans MS" width="70%" align="center">
            <tr>
                <td valign="top">
                    <table border="0" cellpadding="2" cellspacing="2" width="100%">
                        <tr>
                            <td colspan="2" align="right">
                                <b>Figures Found During The Replenishment</b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Loaded
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Left
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Rejected
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Returned
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                            <br />
                                <table cellpadding="2" cellspacing="0" border="1" style="text-align: center">
                                    <tr style="background-color:#218559; color:White">
                                        <td>
                                            <asp:Label ID="Label_CITAdded1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITAdded2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITAdded3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITAdded4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="background-color:#EBB035; color:White">
                                        <td>
                                            <asp:Label ID="Label_CITLeft1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITLeft2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITLeft3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITLeft4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="background-color:#06A2CB; color:White">
                                        <td>
                                            <asp:Label ID="Label_CITRejected1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITRejected2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITRejected3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITRejected4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="background-color:#DD1E2F; color:White">
                                        <td>
                                            <asp:Label ID="Label_Returned1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_Returned2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_Returned3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_Returned4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="center">
                                            <b>
                                            <asp:Label ID="Label_CITTotalLeftAmount" runat="server" Text="0"></asp:Label>
                                            
                                            </b>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                                <b>Different between physical and printed figures</b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            Difference
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table cellpadding="2" cellspacing="0" border="1" style="text-align: center">
                                    <tr>
                                        <td> 
                                           <asp:Label ID="Label_NoteSetType1" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                           <asp:Label ID="Label_NoteSetType2" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_NoteSetType3" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_NoteSetType4" runat="server"></asp:Label> L.E
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_DiffType1" runat="server" Text="0"></asp:Label>
                                                                                   </td>
                                        <td>
                                           <asp:Label ID="Label_DiffType2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffType3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffType4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table cellpadding="2" cellspacing="2">
                        <tr>
                            <td colspan="2" align="right">
                                <b>Figures as printed in the CCMS</b>
                            </td>
                        </tr>
                        <tr>
                            <td >
                                <table>
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                        <td>
                                            Total
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Left
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Dispensed
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Rejected
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Retract
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            adjusted
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Returned
                                        </td>
                                    </tr>--%>
                                </table>
                            </td>
                            <td>
                                <table cellpadding="2" cellspacing="0" border="1" style="text-align: center">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Width="100px"></asp:Label> 
                                        </td>
                                        
                                        <td>
                                            <asp:Label ID="Label_DiffNoteSetType1" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffNoteSetType2" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffNoteSetType3" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffNoteSetType4" runat="server"></asp:Label> L.E
                                        </td>
                                    </tr>
                                    <tr style="background-color:#218559; color:White">
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text="Total" ></asp:Label> 
                                        </td>
                                      
                                        <td>
                                            <asp:Label ID="Label_CCMSAdded1" runat="server" Text="0"></asp:Label>
                                            <asp:TextBox  ID="TextBox_CCMSAdded1Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>

                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSAdded2" runat="server" Text="0"></asp:Label>
                                           <asp:TextBox  ID="TextBox_CCMSAdded2Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSAdded3" runat="server" Text="0"></asp:Label>
                                           <asp:TextBox  ID="TextBox_CCMSAdded3Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                       </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSAdded4" runat="server" Text="0"></asp:Label>
                                            <asp:TextBox  ID="TextBox_CCMSAdded4Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                        </td>
                                    </tr>
                                    
                                    <tr style="background-color:#EBB035; color:White">
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text="Left" ></asp:Label> 
                                        </td>
                                      
                                        <td>
                                            <asp:Label ID="Label_CCMSLeft1" runat="server" Text="0"></asp:Label>
                                            <asp:TextBox  ID="TextBox_CCMSLeft1Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"> </asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSLeft2" runat="server" Text="0"></asp:Label>
                                            <asp:TextBox  ID="TextBox_CCMSLeft2Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSLeft3" runat="server" Text="0"></asp:Label>
                                            <asp:TextBox  ID="TextBox_CCMSLeft3Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSLeft4" runat="server" Text="0"></asp:Label>
                                             <asp:TextBox  ID="TextBox_CCMSLeft4Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text="Dispensed" ></asp:Label> 
                                        </td>
                                     
                                        <td>
                                            <asp:Label ID="Label_CCMSDispensed1" runat="server" Text="0"></asp:Label>
                                         <asp:TextBox  ID="TextBox_CCMSDispensed1Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"> </asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSDispensed2" runat="server" Text="0"></asp:Label>
                                            <asp:TextBox  ID="TextBox_CCMSDispensed2Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSDispensed3" runat="server" Text="0"></asp:Label>
                                        <asp:TextBox  ID="TextBox_CCMSDispensed3Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSDispensed4" runat="server" Text="0"></asp:Label>
                                        <asp:TextBox  ID="TextBox_CCMSDispensed4Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="background-color:#06A2CB; color:White" >
                                        <td>
                                            <asp:Label ID="Label5" runat="server" Text="Rejected" ></asp:Label> 
                                        </td>

                                        <td>
                                            <asp:Label ID="Label_CCMSRejected1" runat="server" Text="0"></asp:Label>
                                         <asp:TextBox  ID="TextBox_CCMSReject1Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"> </asp:TextBox>     
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSRejected2" runat="server" Text="0"></asp:Label>
                                            <asp:TextBox  ID="TextBox_CCMSReject2Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"> </asp:TextBox>     
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSRejected3" runat="server" Text="0"></asp:Label>
                                       <asp:TextBox  ID="TextBox_CCMSReject3Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>     
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSRejected4" runat="server" Text="0"></asp:Label>
                                            <asp:TextBox  ID="TextBox_CCMSReject4Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>     
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text="Retract" ></asp:Label> 
                                        </td>

                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                    </tr>
                                    <tr>
                                                                                <td>
                                            <asp:Label ID="Label7" runat="server" Text="Adjusted" ></asp:Label> 
                                        </td>

                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                    </tr>

                                                                        <tr style="background-color:#DD1E2F; color:White" >
                                                                                                                    <td>
                                            <asp:Label ID="Label8" runat="server" Text="Returned" ></asp:Label> 
                                        </td>

                                        <td>
                                            <asp:Label ID="Label_CCMSReturned1" runat="server" Text="0"></asp:Label>
                                  <%--          <asp:TextBox  ID="TextBox_CCMSReturned1Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>     
                                  --%>      </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSReturned2" runat="server" Text="0"></asp:Label>
                                    <%--    <asp:TextBox  ID="TextBox_CCMSReturned2Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"> </asp:TextBox>     
                                    --%>    </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSReturned3" runat="server" Text="0"></asp:Label>
                                      <%--  <asp:TextBox  ID="TextBox_CCMSReturned3Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>     
                                      --%>  </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSReturned4" runat="server" Text="0"></asp:Label>
                                      <%--  <asp:TextBox  ID="TextBox_CCMSReturned4Correction" runat="server" Text="" Visible ="false" Width="30px" CssClass="textbox"></asp:TextBox>     
                                      --%>  </td>
                                    </tr>
                                    <tr>
                                        <td colspan="5">
                                           
                                            <b>
                                            <asp:Label ID="Label_CCMSTotalLeftAmount" runat="server" Text="0"></asp:Label>
                                            </b>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Total Difference
                            </td>
                            <td style="border-style: solid; text-align: center">
                              <asp:Label ID="Label_DiffTotalAmount" runat="server" Text="0"></asp:Label>  
                            </td>
                        </tr>
                        <tr>
                            <td>
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
                                <table cellpadding="2" cellspacing="0" border="1" style="text-align: center" width="100%">
                                    <tr>
                                        <td colspan="4">
                                            <b>New Loading</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_notesAdded1" runat="server" Text="0"></asp:Label>
                                            </b>
                                        </td>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_notesAdded2" runat="server" Text="0"></asp:Label></b>
                                        </td>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_notesAdded3" runat="server" Text="0"></asp:Label></b>
                                        </td>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_notesAdded4" runat="server" Text="0"></asp:Label></b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>
                                                <asp:Label ID="Label_TotalLoading" runat="server" Text="0"></asp:Label></b>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1" Width="100%">
                                    <table width="100%" border="0">
                                        <tr>
                                            <td width="100%">
                                                <div style="height: 300px; overflow: auto; background-color:InfoBackground">
                                                    <asp:Literal ID="Literal_Comments" runat="server"></asp:Literal>
                                                    <%--<asp:Label ID="Label_Reason" runat="server" Height="117px" BackColor="Info"
                                                    Width="680px"></asp:Label>--%>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_NewComments" runat="server" Width="520px" TextMode="MultiLine"
                                                                CssClass="textbox" Height="100px"></asp:TextBox>
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



                                 <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2" Width="100%">
                                    <table width="100%" border="0">
                                        
                                        <tr>
                                            <td valign="top">
                                                <table width="50%" border="0"> 
                                                    <tr>
                                                        <td>
                                                            No. Of Replenishments
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_NoOfReplenishments" runat="server"  CssClass="textbox" Text="1"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator_NoOfRep" runat="server"
                                                             ControlToValidate="TextBox_NoOfReplenishments" ValidationExpression="\d+" ErrorMessage="*"></asp:RegularExpressionValidator>
                                                        </td>
                                                     
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Inventory
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_Inventory" runat="server"  CssClass="textbox" Width="200"></asp:TextBox>
                                                        </td>
                                                        
                                                    </tr>
                                                      <tr>
                                                        <td valign="top">
                                                            NCR Comments
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_NCRComments" runat="server"  CssClass="textbox" Width="200" Height="100"></asp:TextBox>
                                                        </td>
                                                        
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                            </componentart:multipage>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="Button_Approve" runat="server" OnClick="btnApprove_Click" CssClass="button"
                    Text="Approve" Width="71px" />&nbsp;
                <asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reject" Width="71px"
                    OnClick="Button_reset_Click" />
                <asp:Button ID="Button_Correction" runat="server" CssClass="button" Text="Correction"
                    Width="71px" OnClick="Button_Correction_Click" />
                <asp:Button ID="Button_Cancel" runat="server" Text="Cancel" CssClass="button" OnClick="Button_Cancel_Click" />
                <input type="button" value="Print Envelop" onclick="window.open('PrintEnvelop.aspx?s=<%=Request.QueryString["sid"]%>');"
                    class="button" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
