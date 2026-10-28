<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinalizeSuspectedReplenishments.aspx.cs"
    Inherits="CCMSUI.CCMS.FinalizeSuspectedReplenishments" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Finalize Suspected Replenishments</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">

    </script>
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <%--<script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript"></script>--%>
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.core.js"></script>
    <script src="../content/js/jquery.js" type="text/javascript"></script>
    <script src="../content/Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.datepicker.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery-ui-timepicker-addon.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.core.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datetimepicker({ dateFormat: 'dd/mm/yy',

                dateFormat: 'dd/mm/yy',
                timeFormat: 'hh:mm:ss',
                defaultDate: -1,
                minDate: '-2m',
                maxDate: '+2d',
                hourGrid: 4,
                minuteGrid: 10,

                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });
    
    </script>
    <style type="text/css">
        .style1
        {
            width: 949px;
        }
        .style4
        {
            height: 27px;
        }
        .style5
        {
            width: 54px;
            height: 27px;
        }
        .style6
        {
            height: 25px;
        }
        .style7
        {
            height: 29px;
        }
        .style8
        {
            width: 54px;
            height: 29px;
        }
        .style9
        {
            height: 30px;
        }
        .style10
        {
            width: 54px;
            height: 30px;
        }
        .style11
        {
            height: 192px;
        }
        .style12
        {
            width: 79px;
            height: 27px;
        }
        .style13
        {
            width: 79px;
            height: 29px;
        }
        .style14
        {
            width: 79px;
            height: 30px;
        }
        .style16
        {
            height: 18px;
        }
        .style17
        {
            height: 17px;
        }
        .style18
        {
            height: 18px;
            width: 174px;
        }
        .style19
        {
            height: 17px;
            width: 174px;
        }
    </style>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    Finalize Suspected Replenishments
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center" colspan="2">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center" colspan="2">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label_Notes" runat="server" ForeColor="Green" Font-Bold="true"></asp:Label>
                </td>
            </tr>
            <tr align="left">
                <td class="style1">
                    <componentart:tabstrip id="TabStrip1" scrollingenabled="true" scrollleftlookid="ScrollItem"
                        enabletheming="true" scrollrightlookid="ScrollItem" cssclass="TopGroup" defaultitemlookid="DefaultTabLook"
                        defaultselecteditemlookid="SelectedTabLook" defaultdisableditemlookid="DisabledTabLook"
                        defaultgrouptabspacing="1px" imagesbaseurl="../images/Tabimages/" multipageid="MultiPage1"
                        width="100%" runat="server">
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
                            <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Mismatch">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Counter File ">
                            </ComponentArt:TabStripTab>
                        </Tabs>
                    </componentart:tabstrip>
                    <componentart:multipage id="MultiPage1" cssclass="MultiPage" runat="server" width="100%">
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1" Width="100%">
                            <table width="100%">
                                <tr>
                                    <td valign="top">
                                        <table width="60%">
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                ATM
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_Title" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Event Date
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_date" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Replenishment Date
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TextBox_RepDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <table id="tblServiceStateAndCurentModeInfo" runat="server" width="100%" cellpadding="0"
                                                                    cellspacing="0" border="0">
                                                                    <tr>
                                                                        <td>
                                                                            Service State
                                                                        </td>
                                                                        <td class="style16">
                                                                            <asp:Label ID="Label_ServiceState" runat="server"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="style19">
                                                                            Current Mode
                                                                        </td>
                                                                        <td class="style17">
                                                                            <asp:Label ID="Label_CurrentMode" runat="server"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Replenishment Type
                                                            </td>
                                                            <td>
                                                                <asp:RadioButtonList ID="RadioButtonList_RepType" runat="server" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="1">Add</asp:ListItem>
                                                                    <asp:ListItem Value="0" Selected="True">Swap</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Resolve Out Of Cash Alert
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="CheckBox_ResolveOutOfCashAlert" runat="server" Checked="true" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Resolve Low Balance Alert
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="CheckBox_LowBalanceReport" runat="server" Checked="true" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Resolve Purge Bin Alert
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="CheckBox_PurgeBinThreshold" runat="server" Checked="true" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Forcefully overwrite counters
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="CheckBox_Override" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <ComponentArt:TabStrip ID="TabStrip_Summary" ScrollingEnabled="true" ScrollLeftLookId="ScrollItem"
                                                        EnableTheming="true" ScrollRightLookId="ScrollItem" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                                        DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                                        DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_Summary"
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
                                                            <ComponentArt:TabStripTab ID="TabStripTab_RecentActivity" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="System">
                                                            </ComponentArt:TabStripTab>
                                                            <ComponentArt:TabStripTab ID="TabStripTab_Replenishments" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="User">
                                                            </ComponentArt:TabStripTab>
                                                        </Tabs>
                                                    </ComponentArt:TabStrip>
                                                    <ComponentArt:MultiPage ID="MultiPage_Summary" CssClass="MultiPage" runat="server"
                                                        Width="100%">
                                                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_OldCounters"
                                                            Width="100%">
                                                            <table width="90%">
                                                                <tr>
                                                                    <td class="style11">
                                                                        <table border="0" width="50%" cellspacing="0" cellpadding="0">
                                                                            <tr>
                                                                                <td id="tableHeaderRecentActivity" runat="server" style="background-color: #54b948;
                                                                                    color: white; text-align: center" colspan="6" class="style6">
                                                                                    <b>
                                                                                        <asp:Label ID="Label_Counters" runat="server" Text="Last Counters Snapshot"></asp:Label>
                                                                                    </b>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: #54b948; color: White" class="style12">
                                                                                    <b>Denomination Type</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" class="style5" align="center">
                                                                                    <b>Total</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" class="style5" align="center">
                                                                                    <b>Remaining</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" width="70px" class="style4" align="center">
                                                                                    <b>Dispensed</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: white" width="70px" class="style4" align="center">
                                                                                    <b>Purged</b>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style12" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label_ctype1" runat="server" Text="Type 1"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_LastTotalType1" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_Remaining1" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:Label ID="Label_Dispensed1" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:Label ID="Label_Purged1" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style12" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label_ctype2" runat="server" Text="Type 2"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_LastTotalType2" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_Remaining2" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:Label ID="Label_Dispensed2" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:Label ID="Label_Purged2" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style13" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label_ctype3" runat="server" Text="Type 3"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_LastTotalType3" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style8" align="right">
                                                                                    <asp:Label ID="Label_Remaining3" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style7" align="right">
                                                                                    <asp:Label ID="Label_Dispensed3" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style7" align="right">
                                                                                    <asp:Label ID="Label_Purged3" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style14" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label_ctype4" runat="server" Text="Type 4"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_LastTotalType4" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style10" align="right">
                                                                                    <asp:Label ID="Label_Remaining4" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style9" align="right">
                                                                                    <asp:Label ID="Label_Dispensed4" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style9" align="right">
                                                                                    <asp:Label ID="Label_Purged4" runat="server" CssClass="textbox" Width="70px" BackColor="Pink"
                                                                                        BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td>
                                                                        <table border="0" width="50%" cellspacing="0" cellpadding="0" id="currentCounterSnapshot"
                                                                            runat="server">
                                                                            <tr>
                                                                                <td id="Td1" runat="server" style="background-color: #54b948; color: white; text-align: center"
                                                                                    colspan="6" class="style6">
                                                                                    <b>Current Counters Snapshot</b>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: #54b948; width: 20px; color: White" class="style12"
                                                                                    align="center">
                                                                                    <b>Denomination Type</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" class="style5" align="center">
                                                                                    <b>Total</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" class="style5" align="center">
                                                                                    <b>Remaining</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" width="70px" class="style4" align="center">
                                                                                    <b>Dispensed</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: white" width="70px" class="style4" align="center">
                                                                                    <b>Purged</b>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style12" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label1" runat="server" Text="Type 1"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_CurrentTotalType1" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_currentRemaining1" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:Label ID="Label_currentDispensed1" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:Label ID="Label_currentPurged1" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style12" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label2" runat="server" Text="Type 2"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_CurrentTotalType2" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_currentRemaining2" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:Label ID="Label_currentDispensed2" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:Label ID="Label_currentPurged2" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style13" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label3" runat="server" Text="Type 3"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_CurrentTotalType3" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style8" align="right">
                                                                                    <asp:Label ID="Label_currentRemaining3" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style7" align="right">
                                                                                    <asp:Label ID="Label_currentDispensed3" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style7" align="right">
                                                                                    <asp:Label ID="Label_currentPurged3" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style14" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label4" runat="server" Text="Type 4"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:Label ID="Label_CurrentTotalType4" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style10" align="right">
                                                                                    <asp:Label ID="Label_currentRemaining4" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style9" align="right">
                                                                                    <asp:Label ID="Label_currentDispensed4" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style9" align="right">
                                                                                    <asp:Label ID="Label_currentPurged4" runat="server" CssClass="textbox" Width="70px"
                                                                                        BackColor="Pink" BorderColor="Pink"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </ComponentArt:PageView>
                                                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_RecentActivity"
                                                            Width="100%">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <table border="0" width="50%" cellspacing="0" cellpadding="0" id="Table1" runat="server">
                                                                            <tr>
                                                                                <td id="Td2" runat="server" style="background-color: #54b948; color: white; text-align: center"
                                                                                    colspan="6" class="style6">
                                                                                    <b>New Counters</b>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: #54b948; width: 20px; color: White" class="style12"
                                                                                    align="center">
                                                                                    <b>Denomination Type</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" class="style5" align="center">
                                                                                    <b>Total</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" class="style5" align="center">
                                                                                    <b>Remaining</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: White" width="70px" class="style4" align="center">
                                                                                    <b>Dispensed</b>
                                                                                </td>
                                                                                <td style="background-color: #54b948; color: white" width="70px" class="style4" align="center">
                                                                                    <b>Purged</b>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style12" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label5" runat="server" Text="Type 1"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:TextBox ID="TextBox_Total1" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:TextBox ID="TextBox_Rem1" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:TextBox ID="TextBox_Dis1" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:TextBox ID="TextBox_Purg1" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style12" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label10" runat="server" Text="Type 2"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:TextBox ID="TextBox_Total2" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:TextBox ID="TextBox_Rem2" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:TextBox ID="TextBox_Dis2" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:TextBox ID="TextBox_Purg2" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style13" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label15" runat="server" Text="Type 3"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:TextBox ID="TextBox_Total3" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:TextBox ID="TextBox_Rem3" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:TextBox ID="TextBox_Dis3" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:TextBox ID="TextBox_Purg3" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="background-color: Gray; color: White" class="style14" align="center">
                                                                                    <b>
                                                                                        <asp:Label ID="Label20" runat="server" Text="Type 4"></asp:Label></b>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:TextBox ID="TextBox_Total4" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style5" align="right">
                                                                                    <asp:TextBox ID="TextBox_Rem4" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:TextBox ID="TextBox_Dis4" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="background-color: Pink" class="style4" align="right">
                                                                                    <asp:TextBox ID="TextBox_Purg4" runat="server" CssClss="textbox" Width="70px"></asp:TextBox>
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
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="Button_Approve" runat="server" Text="Approve" CssClass="button" OnClick="Button_Approve_Click" />
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="Button_Decline" runat="server" Text="Reject" CssClass="button" OnClick="Button_Decline_Click" />
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="Button_Cancel" runat="server" Text="Cancel" CssClass="button" OnClick="Button_Cancel_Click" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td valign="top">
                                        <table width="100%">
                                            <tr>
                                                <td align="center" style="font-weight: bold; font-size: large">
                                                    <b>Replenishment</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div style="overflow: auto; height: 200px">
                                                        <asp:GridView runat="server" ID="GridView_rep" AutoGenerateColumns="False" BorderStyle="Solid"
                                                            BorderWidth="1px" CellPadding="3" PageSize="15" Width="100%" AllowSorting="true"
                                                            EnableViewState="false">
                                                            <EmptyDataTemplate>
                                                                <center>
                                                                    No Records Found</center>
                                                            </EmptyDataTemplate>
                                                            <RowStyle HorizontalAlign="Center" />
                                                            <Columns>
                                                                <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" ItemStyle-HorizontalAlign="Center">
                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="rep_datetime" HeaderText="DateTime" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                                                    HtmlEncode="false">
                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="cash_added1" HeaderText="Added 1">
                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="cash_added2" HeaderText="Added 2">
                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="cash_added3" HeaderText="Added 3">
                                                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="cash_added4" HeaderText="Added 4">
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
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" style="font-weight: bold; font-size: large">
                                                    <b>Top 5 Pre Withdrawals </b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:GridView runat="server" ID="GridView_PreWithdrawals" AutoGenerateColumns="False"
                                                        BorderStyle="Solid" BorderWidth="1px" CellPadding="3" PageSize="15" Width="100%">
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
                                                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="cash_remaining1" HeaderText="Remaining 1" SortExpression="cash_remaining1">
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="cash_remaining2" HeaderText="Remaining 2" SortExpression="cash_remaining2">
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="cash_remaining3" HeaderText="Remaining 3" SortExpression="cash_remaining3">
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="cash_remaining4" HeaderText="Remaining 4" SortExpression="cash_remaining4">
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
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" style="font-weight: bold; font-size: large">
                                                    <b>Top 5 Post Withdrawals </b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:GridView runat="server" ID="GridView_PostWithdrawals" AutoGenerateColumns="False"
                                                        BorderStyle="Solid" BorderWidth="1px" CellPadding="3" PageSize="15" Width="100%">
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
                                                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="cash_remaining1" HeaderText="Remaining 1" SortExpression="cash_remaining1">
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="cash_remaining2" HeaderText="Remaining 2" SortExpression="cash_remaining2">
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="cash_remaining3" HeaderText="Remaining 3" SortExpression="cash_remaining3">
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="cash_remaining4" HeaderText="Remaining 4" SortExpression="cash_remaining4">
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
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2" Width="100%">
                            <table>
                                <tr>
                                    <td>
                                        Load On Demand
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="CheckBox_LoadOnDemandCounterFile" runat="server" OnCheckedChanged="LoadCounterFileOnDemand_Click" AutoPostBack="true" />
                                    </td>
                                    <td>
                                     <asp:GridView runat="server" ID="GridView_CounterFile" BorderStyle="Solid" EnableViewState="false"
                                BorderWidth="1px" CellPadding="3" PageSize="15" Width="100%">
                            </asp:GridView>   
                                    </td>
                                </tr>
                            </table>
                            
                        </ComponentArt:PageView>
                    </componentart:multipage>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
