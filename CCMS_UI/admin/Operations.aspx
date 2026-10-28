<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Operations.aspx.cs" Inherits="CCMSUI.admin.Operations"
    EnableViewState="false" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';


    </script>
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <div class="PageHeading">
            <div style="padding-top: 5px;">
                Please select your option
            </div>
        </div>
        <br />
        <div class="cart-tabstrip-outer-wrapper" id="container" runat="server">
            <div class="cart-tabstrip-inner-wrapper">
                <ComponentArt:TabStrip ID="TabStrip_Operations" ScrollingEnabled="true" ScrollLeftLookId="ScrollItem"
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
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="General">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_CCMS" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Currency And Cash Management System">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_EJ" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Electronic Journal">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_Distribution" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Software Distribution">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_PowerMonitoring" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Power Monitoring">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Reconciliation">
                        </ComponentArt:TabStripTab>
                    </Tabs>
                </ComponentArt:TabStrip>
                <ComponentArt:MultiPage ID="MultiPage_Operations" CssClass="MultiPage" runat="server"
                    Width="100%">
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_RecentActivity"
                        Width="100%">
                        <div class="DIVHomePage" id="DivAgentMonitoring" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/AgentMonitoring.aspx'">
                                        Alert Monitoring ( History )<br />
                                        <img alt="" src="../images/Icon_AgentMonitoring.jpg" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivTerminalFileStatusMonitoring" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/TerminalFileStatusMonitoring.aspx'">
                                        Task Status Monitoring ( History )<br />
                                        <img alt="" src="../images/Icon_taskStatusMonitoring.jpg" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivFtpStatus" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/FtpFileTransferStatus.aspx'">
                                        Order Download And Daily Feed Status ( History )<br />
                                        <img alt="" src="../images/Icon_fileTransferStatus.jpg" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCashOrderMonitoring" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/CashOrderMonitoring.aspx'">
                                        Cash Order Monitoring ( History )<br />
                                        <img alt="" src="../images/Icon_cashOrderMonitoring.jpg" id="IMG3" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivAlertDashBoard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/Dashboards/DashBoard_Alerts.aspx';">
                                        Alert Dashboard<br />
                                        <img alt="" src="../images/icon_alertDashboard.jpg" id="IMG1" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivFTPDashBoard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/DashBoardGrid_FTP.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        Order Download And Daily Feed Status Dashboard<br />
                                        <img alt="" src="../images/icon_ftpDashBoard.jpg" id="IMG2" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivTaskDashBoard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/Dashboards/DashBoard_Task.aspx';">
                                        Task Dashboard<br />
                                        <img alt="" src="../images/icon_taskSchedule.jpg" id="IMG5" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCashOrderDashBoard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/DashBoardGrid_CashOrderMonitoring.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        Cash Order Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG6" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivDepositMonitoring" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/Dashboards/DashBoard_Deposite.aspx';">
                                        BNA & CPM Deposit Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG10" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivReconciliation" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/EJ_transaction.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/Reconciliation.aspx';">
                                        Reconciliation Batch (History)<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG211" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                         &nbsp;
                        <div class="DIVHomePage" id="Div1" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/EJ_transaction.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewUploadedData.aspx';">
                                        Switch/Host Transactions (History)<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG21" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivViewCashOrders" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/cashorders.aspx';">
                                        Cash Orders ( History )<br />
                                        <img alt="" src="../images/icon_viewCashOrders.jpg" id="IMG8" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivMinThreshold" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/DashBoardGrid_MinimumThreshold.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        Minimum Threshold Dashboard<br />
                                        <img alt="" src="../images/icon_alertTemplate.jpg" id="IMG12" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivVaultSummaryDashboard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/DashBoardGrid_VaultSummary.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        Vault Balance Dashboard<br />
                                        <img alt="" src="../images/icon_alertTemplate.jpg" id="IMG17" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        
                        &nbsp;
                        <div class="DIVHomePage" id="DivReportTaskMonitoring" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/reporttask.aspx';">
                                        Report Task Monitoring (History)<br />
                                        <img alt="" src="../images/reports.jpg" id="IMG13" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivViewCallOut" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/CallOutList.aspx';">
                                        Call Out (History)<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG22" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivViewATMSettlement" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ATMSettlementList.aspx';">
                                        View ATM Settlements ( History )<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG20" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCallOutSummary" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/CallOutSummary.aspx';">
                                        Call Out Summary<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG23" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivTrxnHourlyAnalysis" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewTransactionsHourlyAnalysis.aspx';">
                                        Transactions Hourly Analysis<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG24" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivUserTask" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/userTask.aspx';">
                                        <br />
                                        User Task ( History )<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG25" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="Div_ATMTrxnAnalysis" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ATMTransactionAnalysis.aspx';">
                                        <br />
                                        ATM Transaction Analysis
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG26" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivForecast" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ForecastingList.aspx';">
                                        <br />
                                        Forecast<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG27" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivDisputeManagement" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewEJTransactions.aspx?q=1';">
                                        <br />
                                        Dispute Management<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG28" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivVaultLedgerHistory" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/VaultLedger.aspx';">
                                        <br />
                                        Vault Ledger ( History )<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG29" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivVaultAdjustments" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../admin/VaultAdjustments.aspx';">
                                        <br />
                                        Post Vault Adjustments<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG32" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp; &nbsp;
                        <div class="DIVHomePage" id="DivVaultBalanceInvestigation" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/VaultBalanceInvestigation.aspx';">
                                        <br />
                                        Vault Balance Investigation<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG30" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="DIVHomePage" id="DivVaultSummary" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/VaultSummary.aspx';">
                                        <br />
                                        Vault Summary<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG31" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivATMNetworkInfo" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../admin/ATMNetworkInfo.aspx';">
                                        <br />
                                        ATM Network Info<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG33" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                       
                        &nbsp;
                        <div class="DIVHomePage" id="DivSummaryDashboard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/DashBoard_Summary.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        Summary Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG19" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivViewATMInventory" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='ATMs.aspx?i=1';">
                                        View ATM Inventory<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG45" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivExecDashboard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/ExecDashboard.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        Executive Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG46" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivTransactionMonDashboard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/TransactionDashboard.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        Transaction Monitoring Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG47" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCashDashboard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/CashDashboard.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        Cash Management Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG48" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        &nbsp;
                          <div class="DIVHomePage" id="DivReconciliationMonitoring" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/ATMReconciliation.aspx'";">
                                         ATM Reconciliation<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG465" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                         &nbsp;
                          <div class="DIVHomePage" id="Div2" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/ATMReconciliationDataFeedFiles.aspx'";">
                                         Reconciliation Data Feed Files<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG4615" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                          <div class="DIVHomePage" id="Div3" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/ATMNotesDispense.aspx'";">
                                         ATM Notes Dispensed<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG1465" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                         &nbsp;
                        &nbsp;
  <div class="DIVHomePage" id="Div4" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
        <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
            width: 158px">
            <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/TransactionsSummary.aspx'";">
                 ATM Transaction Summary<br />
                <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG1465" />
            </td>
        </tr>
    </table>
</div>
 &nbsp;
                        <div class="DIVHomePage" id="DivSMSTaskStatusMonitoring" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/SMSTaskStatusMonitoring.aspx'";">
                                        SMS Task Status Monitoring(History)<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG46" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                         <div class="DIVHomePage" id="DivCalculateInvoiceDeductions" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/CalculateInvoiceDeductions.aspx'";">
                                        Calculate Invoice Deductions<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG48" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView21">
                        <div class="DIVHomePage" id="DivOutOfBalanceInvestigation" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/OutOfBalanceInvestigation.aspx'">Balance Investigation<br />
                                        <img alt="" src="../images/icon_outOfBalanceInvestigat.jpg" id="IMG4" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCashPositionsDashBoard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/Dashboards/DashBoard_CashPositions.aspx';">Cash Positions Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG9" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivViewTransactions" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewTransactions.aspx';">Withdrawal Transactions (History)<br />
                                        <img alt="" src="../images/icon_viewTransactions.jpg" id="IMG7" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCashPositions" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/cashpositions.aspx';">Cash Positions ( History )<br />
                                        <img alt="" src="../images/icon_viewCashOrders.jpg" id="IMG11" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCashPositionVsDFF" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/Dashboards/DashBoardGrid_CashPosVsDFF.aspx';">Cash Positions Vs DFF (History)<br />
                                        <img alt="" src="../images/Cash-Order.jpg" id="IMG14" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivViewBNATransactions" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewBNATransactions.aspx';">
                                        <br />
                                        BNA Transactions ( History )<br />
                                        <img alt="" src="../images/Cash-Order.jpg" id="IMG34" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivViewCPMTransactions" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewCPMTransactions.aspx';">
                                        <br />
                                        CPM Transactions ( History )<br />
                                        <img alt="" src="../images/Cash-Order.jpg" id="IMG35" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivATMDetail" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/deposit_transaction.jpg); height: 100px; width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ATMDetail.aspx';">
                                        <br />
                                        ATM Device Status<br />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCountsClearedHistory" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/deposit_transaction.jpg); height: 100px; width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/CashCountsClearedList.aspx';">
                                        <br />
                                        Cash Counts Cleared History<br />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCCMSReplenishments" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/deposit_transaction.jpg); height: 100px; width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/CCMSReplenishments.aspx';">
                                        <br />
                                        Replenishments<br />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        &nbsp;

 
            <div class="DIVHomePage" id="Div5" runat="server">
                <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                    <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                        <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/ReplenishmentReturn.aspx'">Replenishments Return<br />
                            <img alt="" src="../images/repReturn.png" />
                        </td>
                    </tr>
                </table>
            </div>
                        &nbsp;               
                        


                        <div class="DIVHomePage" id="DivPrepareCashOrders" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/PrepareCashOrders.aspx'">Prepare Cash Orders<br />
                                        <img alt="" src="../images/Icon_taskStatusMonitoring.jpg" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivViewIMCCMSDashboard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/DashBoardGrid_IMCCMSSummary.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">IM CCMS Summary Dashboard<br />
                                        <img alt="" src="../images/icon_alertTemplate.jpg" id="IMG47" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_LowBalance"
                        Width="100%">
                        <div class="DIVHomePage" id="DivEJCashPositionsDashBoard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/DashBoardGrid_EJCashPositions.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        EJ Cash Positions Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG15" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivBWACCashPos" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/Dashboards/DashBoard_BWACCashPosition.aspx';">
                                        BWAC Cash Positions Dashboard<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG43" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="DIVHomePage" id="DivEJTrxn" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewEJTransactions.aspx';">
                                        EJ Withdrawal Transactions (History)<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG36" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivEJCashPositions" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/cashpositions.aspx?ej=1';">
                                        EJ Cash Positions ( History )<br />
                                        <img alt="" src="../images/icon_viewCashOrders.jpg" id="IMG16" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivCardCaptured" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage" style="background-image: url(../images/capture_card.jpg);">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/CardCaptured.aspx';">
                                        <br />
                                        Cards Captured (History)
                                        <img alt="" src="../images/icon_viewCashOrders.jpg" id="IMG37" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivEJRep" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/EJReplenishments.aspx';">
                                        <br />
                                        EJ Replenishments (History)<br />
                                        <img alt="" src="../images/icon_viewCashOrders.jpg" id="IMG38" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivEJBalanceInvestigation" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/EJBalanceInvestigation.aspx';">
                                        EJ Balance Investigation (History)<br />
                                        <img alt="" src="../images/icon_outOfBalanceInvestigat.jpg" id="IMG18" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivEJBNATrxn" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/deposit_transaction.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/EJBNATransactions.aspx';">
                                        <br />
                                        EJ BNA Transactions ( History )
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivFraudTrxnDashboard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/Dashboards/DashBoard_ExceptionHandling.aspx';">
                                        Exception handling DashBoard<br />
                                        <img alt="" src="../images/icon_outOfBalanceInvestigat.jpg" id="IMG39" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivFraudTransactions" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewEJTransactions.aspx?ej=2';">
                                        Exception Handling ( History )<br />
                                        <img alt="" src="../images/icon_outOfBalanceInvestigat.jpg" id="IMG40" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivPNCBNATrxn" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewEJBNATransactions.aspx';">
                                        EJ BNA Transactions ( History )<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG41" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivPNCCPMTrxn" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewEJCPMTransactions.aspx';">
                                        EJ CPM Transactions ( History )<br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG42" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivEjCashCountsCleared" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/deposit_transaction.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/CashCountsClearedList.aspx';">
                                        <br />
                                        Cash Counts Cleared History<br />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <%-- added by shariq on 4 march --%>
                        &nbsp;
                        <div class="DIVHomePage" id="DivEjviewerWithOptions" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/EjViewerWithOptions.aspx'";">
                                        EjViewer <br />
                                     
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG49" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_Distribution"
                        Width="100%">
                        
                        

                          <div class="DIVHomePage" id="DivUploadResource" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/uploadResource.aspx';">
                                        <asp:Label ID="Label2" runat="server" Text="Package Scheduling"></asp:Label>
                                        <br />
                                        <img alt="" src="../images/package_scheduling.jpg" id="IMG53" width="60px" height="60px" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                         
                             &nbsp;

                          <div class="DIVHomePage" id="DivUploadStatus" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/uploadstatus.aspx';">
                                        <asp:Label ID="Label3" runat="server" Text="Package Distribution Status"></asp:Label>
                                        <br />
                                        <img alt="" src="../images/package_status.jpg" id="IMG54" width="60px" height="60px" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                         
                          
                                              <%-- &nbsp;
                        <div class="DIVHomePage" id="Div1" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/deposit_transaction.jpg); height: 100px;
                                    width: 158px">
                                    <td valign="top" style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/UploadStatus.aspx';">
                                        <br />
                                        Package Distribution Analytics<br />
                                    </td>
                                </tr>
                            </table>
                        </div>--%>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_PowerMonitoring"
                        Width="100%">
                        &nbsp;
                        <div class="DIVHomePage" id="DivPowerMonitoingDashboard" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="window.open('../CCMS/Dashboards/DashBoardGrid_PowerMonitoring.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                        <asp:Label ID="Label_PowerMonitoring" runat="server" Text="Power Monitoring Dashboard"></asp:Label>
                                        <br />
                                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG44" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivSensorMonitoringHistory" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/SensorMonitoring.aspx'">
                                        Sensor Monitoring ( History )<br />
                                        <img alt="" src="../images/Icon_taskStatusMonitoring.jpg" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivLinkMonitoringHistory" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/LinkMonitoring.aspx'">
                                        Link Monitoring( History )<br />
                                        <img alt="" src="../images/Icon_taskStatusMonitoring.jpg" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        &nbsp;
                        <div class="DIVHomePage" id="DivGasperTicketsHistory" runat="server">
                            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px">
                                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/GasperTicketHistory.aspx'">
                                        Gasper Ticket ( History )<br />
                                        <img alt="" src="../images/Icon_taskStatusMonitoring.jpg" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_Reconciliation"
                        Width="100%">
                        &nbsp;
    <div class="DIVHomePage" id="Div6" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
            <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/Reconciliation.aspx';">
                    <img alt="" src="../images/batch.png" /><br />
                    Reconciliation Batch
                </td>
            </tr>
        </table>
    </div>

                        &nbsp;
    <div class="DIVHomePage" id="DivViewSwitchTx" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
            <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewSwitchDispensedTx.aspx';">
                    <img alt="" src="../images/switchdistx.png" id="IMG8" /><br />
                    Switch Dispensed Transactions
                </td>
            </tr>
        </table>
    </div>

                        &nbsp;
    <div class="DIVHomePage" id="DivViewSwitchBnaTx" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
            <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewSwitchDepositTx.aspx';">
                    <img alt="" src="../images/switchdeptx.png" id="IMG8" /><br />
                    Switch Deposit Transactions
                </td>
            </tr>
        </table>
    </div>

                        &nbsp;
    <div class="DIVHomePage" id="DivViewHostTx" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
            <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewHostDispensedTx.aspx';">
                    <img alt="" src="../images/hostTx.png" id="IMG8" /><br />
                    Host Dispensed Transactions
                </td>
            </tr>
        </table>
    </div>

                        &nbsp;
    <div class="DIVHomePage" id="DivViewHostBnaTx" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
            <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewHostDepositTx.aspx';">
                    <img alt="" src="../images/tx.png" id="IMG8" /><br />
                    Host Deposit Transactions
                </td>
            </tr>
        </table>
    </div>
                        &nbsp;
    <div class="DIVHomePage" id="DivCCMSEjSwitch" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
            <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                <td style="width: 158px; cursor: pointer" onclick="window.location='../CCMS/ViewEjVsSwitchVsHostTransactionsOptimised.aspx';">
                    <img alt="" src="../images/recon.png" id="IMG48" /><br />
                    Ej Vs Switch Vs Host Reconciliation
                </td>
            </tr>
        </table>
    </div>
                        &nbsp;
    <div class="DIVHomePage" id="DivBulkImportHistory" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
            <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                <td style="width: 158px; cursor: pointer" onclick=" location.href='../CCMS/ATMReconciliationDataFeedFiles.aspx'">
                    <img alt="" src="../images/file-import.png" id="IMG48" /><br />
                    Switch/Host Import History
                </td>
            </tr>
        </table>
    </div>

                    </ComponentArt:PageView>
                </ComponentArt:MultiPage>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
