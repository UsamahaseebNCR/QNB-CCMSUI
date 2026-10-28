<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Administration.aspx.cs"
    EnableViewState="false" Inherits="CCMSUI.admin.Administration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>iJournal 3 Home</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script type="text/javascript">
        window.parent.window.contentPage = '';
       
    </script>
</head>
<body onload="hideProgressBar();">
    <div class="PageHeading">
        <div style="padding-top: 5px;">
            Please select your option
        </div>
    </div>
    <br />
    <div id="menus" style="padding-left: 10px">
        <div class="DIVHomePage" id="DivNoteSetType" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='NoteSetType.aspx'">
                        Note Set Type Setup<br />
                        <img alt="" src="../images/NOTE-SET-TYPE UPDATE.jpg" style="cursor: pointer" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivOrganization" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='organization.aspx'">
                        Organization Setup<br />
                        <img alt="" src="../images/icon_Bank.jpg" style="cursor: pointer" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivCiT" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='CIT.aspx'">
                        CIT<br />
                        <img alt="" src="../images/icon_CIT.jpg" style="cursor: pointer" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivVault" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='Vault.aspx'">
                        Vault<br />
                        <img alt="" src="../images/icon_CIT.jpg" style="cursor: pointer" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivATM" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='ATMs.aspx'">
                        ATMs Setup<br />
                        <img alt="" src="../images/icon_welcome_ATMManagement.jpg" style="cursor: pointer" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivUserManagement" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='Users.aspx'">
                        Users<br />
                        <img alt="" src="../images/icon_welcome_groupmanagement.jpg" style="cursor: pointer" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivGroups" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='GroupUser.aspx'">
                        Groups<br />
                        <img alt="" src="../images/icon_welcome_groups.jpg" style="cursor: pointer" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivAppConfig" runat="server">
            <table cellpadding="0" cellspacing="0" class="DIVHomePage" border="0">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/Configure.aspx'">
                        Application Configuration<br />
                        <img alt="" src="../images/icon_welcome_applicationconfiguration.jpg" style="cursor: pointer" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivScheduleDownload" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='../ccms/ScheduledDownload.aspx'">
                        Schedule ATM Counter Downloads<br />
                        <img alt="" src="../images/icon_welcome_filedownload.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivAuditLog" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/AuditLog.aspx'">
                        Activity Log<br />
                        <img alt="" src="../images/icon_ActivityLog.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivScheduleDailyFeed" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='../CCMS/ViewDailyFeed.aspx'">
                        Recreate Daily Feed<br />
                        <img alt="" src="../images/icon_dailyFeedFileScheduleS.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivAgentRemoteAccess" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='AgentRemoteAccess.htm'">
                        Agent Remote Access<br />
                        <img alt="" src="../images/icon_agentRemoteAccess.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="Div_AlertTemplate" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='AlertTemplate.aspx'">
                        Alert Template<br />
                        <img alt="" src="../images/icon_alertTemplate.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="Div1" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='../ccms/ConfigureDailyFeed.aspx'">
                        Configure Daily Feed Generation<br />
                        <img alt="" src="../images/icon_dailyFeedFileConfig.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="Div_ScheduleReport" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='../ccms/ScheduleReportGeneration.aspx'">
                        Schedule Report<br />
                        <img alt="" src="../images/icon_dailyFeedFileConfig.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="Div_FileType" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='../admin/FileType.aspx'">
                        File Type<br />
                        <img alt="" src="../images/icon_dailyFeedFileConfig.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivDirLookup" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/DirectoryLookupAndFileDownloader.aspx';">
                        Directory Lookup And File Downloader<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG15" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivImportFLM" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ImportFromFLMReport.aspx';">
                        Import Call Out Report By Bank<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG1" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivImportFLMMonitoring" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ImportFLMMonitoringRecords.aspx';">
                        Import Call Out Report By NCR<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG3" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivFITMapping" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/FITMapping.aspx';">
                        FIT Mapping<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG13" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivATMSettlement" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ImportATMSettlement.aspx';">
                        Import ATM Settlement<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG4" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivAddEditRule" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/RulesList.aspx';">
                        Rules Management<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG2" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivGLAccounts" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/GLAccounts.aspx';">
                        GL Accounts Management<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG5" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivInvoice" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/Invoices.aspx';">
                        Invoice Management<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG6" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivCheque" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/Cheques.aspx';">
                        Bulk Cash Management<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG7" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivInvestigation" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/InvestigationList.aspx';">
                        Investigation<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG8" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivDevices" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/Devices.aspx';">
                        Device Management<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG9" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivDeviceTemplate" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../ccms/DeviceTemplates.aspx';">
                        Device Template Management<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG10" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivAssignTemplate" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/AssignTemplates.aspx';">
                        Assign Templates<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG11" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivAlertTemplates" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/AlertTemplates.aspx';">
                        Alert Templates Management<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG12" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivImportGasperTickets" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ImportGasperTickets.aspx';">
                        Import Gasper Tickets<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG14" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivImportBWAC" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ImportBWAC.aspx';">
                        Import BWAC<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG16" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivImportQNBExcelSheet" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ImportQNBExcelSheetTemplate.aspx';">
                        Import QNB Excel Sheet<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG17" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivATMClassification" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/Classification.aspx';">
                        ATM Classification<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG18" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivImportCashOrders" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ImportCashOrders.aspx';">
                        Import Cash Orders<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG19" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivEventsManagement" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../EventsManagement.aspx';">
                        Events Management<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG20" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivSMSTemplate" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/SmsTemplate.aspx';">
                        SMS Template<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG21" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivSMSConfiguration" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/SmsConfiguration.aspx';">
                        SMS Configuration<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG22" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivExceptionHandlingCriteria" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ExceptionHandlingCriteria.aspx';">
                        Exception Handling Criteria<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG23" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivReportTemplate" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ReportTemplate.aspx';">
                        Report Template<br />
                        <img alt="" src="../images/icon_cashOrderDashboard.jpg" id="IMG24" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivApproveUser" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ApproveUser.aspx';">
                        Approve User<br />
                        <img alt="" src="../images/icon_welcome_groupmanagement.jpg" id="IMG25" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivApproveGroup" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ApproveGroup.aspx';">
                        Approve Group<br />
                        <img alt="" src="../images/icon_welcome_groups.jpg" id="IMG26" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivDashboardTemplate" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/DashboardTemplate.aspx';">
                        Dashboard Template<br />
                        <img alt="" src="../images/icon_welcome_groups.jpg" id="IMG27" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivPackageManagement" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/Package.aspx';">
                        <asp:Label ID="Label1" runat="server" Text="Package Management"></asp:Label>
                        <br />
                        <img alt="" src="../images/package.png" id="IMG52" width="50px" height="50px" />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivATMBandwidth" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                    width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.location='../admin/ATMBandwidth.aspx';">
                        <asp:Label ID="Label2" runat="server" Text="Bandwidth Management"></asp:Label>
                        <br />
                        <img alt="" src="../images/bandwidth.png" id="IMG28" width="50px" height="50px" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <form id="form1" runat="server">
    </form>
</body>
</html>
