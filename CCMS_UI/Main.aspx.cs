using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;

using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
//using LicenseVerifierLib;
using System.IO;
using CCMSUI;

namespace iSuite
{
    public partial class Main : BasePage
    {
        DataSet atmsDS;
        StringBuilder jsStr = new StringBuilder(5000);
        StringBuilder jsStrLocation = new StringBuilder(5000);
        bool isTreeviewNodeExpanded = System.Web.Configuration.WebConfigurationManager.AppSettings["isTreeviewNodeExpanded"] == "1" ? true : false;
        string traceOrganizationMCN = System.Web.Configuration.WebConfigurationManager.AppSettings["traceOrganizationMCN"];

        protected void Page_Load(object sender, EventArgs e)
        {
            //divWait.Visible = true;
            try
            {
                if (Callback_SessionEnd.CausedCallback
                ||
                Callback_Move.CausedCallback || Callback_RegionAdd.CausedCallback || Callback_RegionAdd.CausedCallback || Callback_Move.CausedCallback || Callback_nodeSelected.CausedCallback || Callback_multiNodeSelection.CausedCallback || CallBack_TreePanelWidthChange.CausedCallback)
                    return;

                if (Session[SessionVars.UserSettings.ToString()] == null)
                    return;

                if (Session["currentPage"] != null)
                    lastVisitedPage.Value = (string)Session["currentPage"];
                else
                {
                    //if (CheckRight((int)Permissions.ViewSummary))
                    lastVisitedPage.Value = "home.aspx";
                }


                if (Session[SessionVars.noticeForMain.ToString()] != null)
                {
                    Page.RegisterStartupScript("script", "<script>ShowSuccess('" + Session[SessionVars.noticeForMain.ToString()] + "')</script>");
                    Session[SessionVars.noticeForMain.ToString()] = null;
                }
                //Session[SessionVars.SelectedRegion.ToString()] = "1";

                Label_DateTime.Text = DateTime.Now.ToLongDateString();
                 Splitter1.Layouts[0].Panes[0].Width = ((UserSetting)(Session[SessionVars.UserSettings.ToString()])).TreePanelWidth;
                BuildTree();
                SelectDefaultOrganization();

                Callback_RegionAdd.Visible = CheckRight((int)Permissions.CreateRegion);

                if (!CheckRight((int)Permissions.ChangePassword))
                    tdChangePassword.InnerHtml = "";//.Visible = false;

                ComponentArt.Web.UI.MenuItem menuHome = new ComponentArt.Web.UI.MenuItem();

                menuHome.Text = "Home";
                menuHome.NavigateUrl = "~/home.aspx";
                menuHome.Target = "contentForm";
                menuHome.LookId = "TopItemLook";


                if (CheckRight((int)Permissions.ViewSummary))
                {
                    ComponentArt.Web.UI.MenuItem menuSummary = new ComponentArt.Web.UI.MenuItem();
                    menuSummary.Text = "Summary";
                    menuSummary.NavigateUrl = "~/ccms/Summary.aspx";
                    menuSummary.Target = "contentForm";

                    menuHome.Items.Add(menuSummary);

                    menuHome.Look.RightIconUrl = "arrow.gif";
                    menuHome.Look.RightIconWidth = 10;
                }
                //if (menuHome.Items.Count > 0)
                MenuHome.Items.Add(menuHome);

                if (CheckRight((int)Permissions.ViewExceptionHandlingCriteria) ||
                    CheckRight((int)Permissions.ViewSmsTemplate) ||
                    CheckRight((int)Permissions.ViewSmsConfiguration) ||
                    CheckRight((int)Permissions.ViewEventsManagement) ||
                    CheckRight((int)Permissions.ImportCashOrders) ||
                    CheckRight((int)Permissions.ViewATMClassification) ||
                    CheckRight((int)Permissions.ImportGasperTicket) ||
                    CheckRight((int)Permissions.ImportBWAC) ||
                    CheckRight((int)Permissions.ViewAlertTemplatesList) ||
                    CheckRight((int)Permissions.ViewInvestigation) ||
                    CheckRight((int)Permissions.ViewCheque) ||
                    CheckRight((int)Permissions.ViewInvoice) ||
                    CheckRight((int)Permissions.ViewGLAccount) ||
                    CheckRight((int)Permissions.AddEditRules) || CheckRight((int)Permissions.ViewFITMapping) || CheckRight((int)Permissions.ImportATMSettlement) ||
                    CheckRight((int)Permissions.ImportFLMRecord) || CheckRight((int)Permissions.DirectoryLookupAndFileDownloader) || CheckRight((int)Permissions.ViewFileType) || CheckRight((int)Permissions.ViewScheduleReports) || CheckRight((int)Permissions.ViewVault) || CheckRight((int)Permissions.ViewNoteSetType) || CheckRight((int)Permissions.ViewOrganization) || CheckRight((int)Permissions.ViewCIT) || CheckRight((int)Permissions.ViewATMsList) || CheckRight((int)Permissions.ViewUserList) || CheckRight((int)Permissions.ViewGroupList) || CheckRight((int)Permissions.ViewConfiguration) || CheckRight((int)Permissions.ScheduleFileDownload) || CheckRight((int)Permissions.ViewAuditLog) || CheckRight((int)Permissions.ViewDailyFeedSchedule) || CheckRight((int)Permissions.ViewAgentRemoteAccess) || CheckRight((int)Permissions.ViewAlertTemplate) || CheckRight((int)Permissions.ViewConfigureDailyFeed) ||
                    CheckRight((int)Permissions.ReportTemplate) || CheckRight((int)Permissions.DashboardTemplate) ||
                    CheckRight((int)Permissions.ApproveGroup) || CheckRight((int)Permissions.ApproveUser) ||
                    CheckRight((int)Permissions.ViewATMBandwidth) || CheckRight((int)Permissions.ViewPackageList))
                {
                    ComponentArt.Web.UI.MenuItem menuAdmin = new ComponentArt.Web.UI.MenuItem();
                    menuAdmin.Text = "Administration";
                    menuAdmin.NavigateUrl = "~/admin/administration.aspx";
                    menuAdmin.Target = "contentForm";

                    //MenuAdmin.Items.Add(menuAdmin);

                    if (CheckRight((int)Permissions.ViewNoteSetType))
                    {
                        ComponentArt.Web.UI.MenuItem menuNoteSetType = new ComponentArt.Web.UI.MenuItem();
                        menuNoteSetType.Text = "Note Set Type Setup";
                        menuNoteSetType.NavigateUrl = "~/admin/NoteSetType.aspx";
                        menuNoteSetType.Target = "contentForm";
                        menuAdmin.Items.Add(menuNoteSetType);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ViewOrganization))
                    {
                        ComponentArt.Web.UI.MenuItem menuOrganization = new ComponentArt.Web.UI.MenuItem();
                        menuOrganization.Text = "Organization Setup";
                        menuOrganization.NavigateUrl = "~/admin/organization.aspx";
                        menuOrganization.Target = "contentForm";

                        menuAdmin.Items.Add(menuOrganization);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ViewCIT))
                    {
                        ComponentArt.Web.UI.MenuItem menuCiT = new ComponentArt.Web.UI.MenuItem();
                        menuCiT.Text = "CiT";
                        menuCiT.NavigateUrl = "~/admin/cit.aspx";
                        menuCiT.Target = "contentForm";

                        menuAdmin.Items.Add(menuCiT);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ViewVault))
                    {
                        ComponentArt.Web.UI.MenuItem menuVault = new ComponentArt.Web.UI.MenuItem();
                        menuVault.Text = "Vault Setup";
                        menuVault.NavigateUrl = "~/admin/Vault.aspx";
                        menuVault.Target = "contentForm";
                        menuAdmin.Items.Add(menuVault);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ViewATMsList))
                    {
                        ComponentArt.Web.UI.MenuItem menuATM = new ComponentArt.Web.UI.MenuItem();
                        menuATM.Text = "ATMs Setup";
                        menuATM.NavigateUrl = "~/admin/ATMs.aspx";
                        menuATM.Target = "contentForm";
                        menuAdmin.Items.Add(menuATM);
                    }

                    if (CheckRight((int)Permissions.ViewUserList))
                    {
                        ComponentArt.Web.UI.MenuItem menuUser = new ComponentArt.Web.UI.MenuItem();
                        menuUser.Text = "Users";
                        menuUser.NavigateUrl = "~/admin/Users.aspx";
                        menuUser.Target = "contentForm";
                        menuAdmin.Items.Add(menuUser);
                    }

                    if (CheckRight((int)Permissions.ViewGroupList))
                    {
                        ComponentArt.Web.UI.MenuItem menuGroup = new ComponentArt.Web.UI.MenuItem();
                        menuGroup.Text = "Groups";
                        menuGroup.NavigateUrl = "~/admin/GroupUser.aspx";
                        menuGroup.Target = "contentForm";
                        menuAdmin.Items.Add(menuGroup);
                    }

                    if (CheckRight((int)Permissions.ViewConfiguration))
                    {
                        ComponentArt.Web.UI.MenuItem menuConfig = new ComponentArt.Web.UI.MenuItem();
                        menuConfig.Text = "Application Configuration";
                        menuConfig.NavigateUrl = "~/ccms/configure.aspx";
                        menuConfig.Target = "contentForm";
                        menuAdmin.Items.Add(menuConfig);
                    }
                    if (CheckRight((int)Permissions.ScheduleFileDownload))
                    {
                        ComponentArt.Web.UI.MenuItem menuFileDownload = new ComponentArt.Web.UI.MenuItem();
                        menuFileDownload.Text = "Schedule ATM Counter Downloads";
                        menuFileDownload.NavigateUrl = "~/ccms/ScheduledDownload.aspx";
                        menuFileDownload.Target = "contentForm";
                        menuAdmin.Items.Add(menuFileDownload);
                    }

                    if (CheckRight((int)Permissions.ViewScheduleReports))
                    {
                        ComponentArt.Web.UI.MenuItem menuFileDownload = new ComponentArt.Web.UI.MenuItem();
                        menuFileDownload.Text = "Schedule Report Generation";
                        menuFileDownload.NavigateUrl = "~/ccms/ScheduleReportGeneration.aspx";
                        menuFileDownload.Target = "contentForm";
                        menuAdmin.Items.Add(menuFileDownload);
                    }
                    if (CheckRight((int)Permissions.ViewAuditLog))
                    {
                        ComponentArt.Web.UI.MenuItem menuAudit = new ComponentArt.Web.UI.MenuItem();
                        menuAudit.Text = "Activity Log";
                        menuAudit.NavigateUrl = "~/ccms/AuditLog.aspx";
                        menuAudit.Target = "contentForm";
                        menuAdmin.Items.Add(menuAudit);
                    }

                    if (CheckRight((int)Permissions.ViewDailyFeedSchedule))
                    {
                        ComponentArt.Web.UI.MenuItem menuDailyFeedSchedule = new ComponentArt.Web.UI.MenuItem();
                        menuDailyFeedSchedule.Text = "Recreate Daily Feed";
                        menuDailyFeedSchedule.NavigateUrl = "~/ccms/ViewDailyFeed.aspx";
                        menuDailyFeedSchedule.Target = "contentForm";
                        menuAdmin.Items.Add(menuDailyFeedSchedule);
                    }

                    if (CheckRight((int)Permissions.ViewAgentRemoteAccess))
                    {
                        ComponentArt.Web.UI.MenuItem menuAgentRemoteAccess = new ComponentArt.Web.UI.MenuItem();
                        menuAgentRemoteAccess.Text = "Agent Remote Access";
                        menuAgentRemoteAccess.NavigateUrl = "~/admin/AgentRemoteAccess.htm";
                        menuAgentRemoteAccess.Target = "contentForm";
                        menuAdmin.Items.Add(menuAgentRemoteAccess);
                    }


                    if (CheckRight((int)Permissions.ViewAlertTemplate))
                    {
                        ComponentArt.Web.UI.MenuItem menuAlertTemplate = new ComponentArt.Web.UI.MenuItem();
                        menuAlertTemplate.Text = "Alert Template";
                        menuAlertTemplate.NavigateUrl = "~/admin/AlertTemplate.aspx";
                        menuAlertTemplate.Target = "contentForm";
                        menuAdmin.Items.Add(menuAlertTemplate);
                    }

                    if (CheckRight((int)Permissions.ViewConfigureDailyFeed))
                    {
                        ComponentArt.Web.UI.MenuItem menuConfDailyFeed = new ComponentArt.Web.UI.MenuItem();
                        menuConfDailyFeed.Text = "Configure Daily Feed Generation";
                        menuConfDailyFeed.NavigateUrl = "~/ccms/ConfigureDailyFeed.aspx";
                        menuConfDailyFeed.Target = "contentForm";
                        menuAdmin.Items.Add(menuConfDailyFeed);
                    }
                    if (CheckRight((int)Permissions.ViewFileType))
                    {
                        ComponentArt.Web.UI.MenuItem menuFileType = new ComponentArt.Web.UI.MenuItem();
                        menuFileType.Text = "File Type Setup";
                        menuFileType.NavigateUrl = "~/admin/FileType.aspx";
                        menuFileType.Target = "contentForm";

                        menuAdmin.Items.Add(menuFileType);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.DirectoryLookupAndFileDownloader))
                    {
                        ComponentArt.Web.UI.MenuItem menuDirectoryLookupAndFileDownloader = new ComponentArt.Web.UI.MenuItem();
                        menuDirectoryLookupAndFileDownloader.Text = "Directory Lookup And File Downloader";
                        menuDirectoryLookupAndFileDownloader.NavigateUrl = "~/admin/DirectoryLookupAndFileDownloader.aspx";
                        menuDirectoryLookupAndFileDownloader.Target = "contentForm";

                        menuAdmin.Items.Add(menuDirectoryLookupAndFileDownloader);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }
                    if (CheckRight((int)Permissions.ImportATMSettlement))
                    {
                        ComponentArt.Web.UI.MenuItem menuImportATMSettlement = new ComponentArt.Web.UI.MenuItem();
                        menuImportATMSettlement.Text = "Import ATM Settlement";
                        menuImportATMSettlement.NavigateUrl = "~/admin/ImportATMSettlement.aspx";
                        menuImportATMSettlement.Target = "contentForm";

                        menuAdmin.Items.Add(menuImportATMSettlement);
                    }

                    if (CheckRight((int)Permissions.ImportFLMRecord))
                    {
                        ComponentArt.Web.UI.MenuItem menuImportFLM = new ComponentArt.Web.UI.MenuItem();
                        menuImportFLM.Text = "Import Call Out Report By Bank";
                        menuImportFLM.NavigateUrl = "~/admin/ImportFromFLMReport.aspx";
                        menuImportFLM.Target = "contentForm";

                        menuAdmin.Items.Add(menuImportFLM);

                        ComponentArt.Web.UI.MenuItem menuImportFLMMonitoring = new ComponentArt.Web.UI.MenuItem();
                        menuImportFLMMonitoring.Text = "Import Call Out Report By NCR";
                        menuImportFLMMonitoring.NavigateUrl = "~/admin/ImportFLMMonitoringRecords.aspx";
                        menuImportFLMMonitoring.Target = "contentForm";
                        menuAdmin.Items.Add(menuImportFLMMonitoring);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }
                    if (CheckRight((int)Permissions.ViewFITMapping))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "FIT Mapping";
                        menu.NavigateUrl = "~/admin/FITMapping.aspx";
                        menu.Target = "contentForm";

                        menuAdmin.Items.Add(menu);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }
                    if (CheckRight((int)Permissions.ViewRulesList))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Rules Management";
                        menu.NavigateUrl = "~/admin/RulesList.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ViewGLAccount))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "GL Accounts Management";
                        menu.NavigateUrl = "~/admin/GLAccounts.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewInvoice))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Invoice Management";
                        menu.NavigateUrl = "~/admin/Invoices.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewCheque))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Bulk Cash Management";
                        menu.NavigateUrl = "~/admin/Cheques.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewInvestigation))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Investigation";
                        menu.NavigateUrl = "~/ccms/InvestigationList.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewAlertTemplatesList))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Alert Templates Management";
                        menu.NavigateUrl = "~/admin/AlertTemplates.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ImportBWAC))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Import BWAC";
                        menu.NavigateUrl = "~/admin/ImportBWAC.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ImportQNBExcelSheetTemplate))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Import QNB Excel Sheet";
                        menu.NavigateUrl = "~/admin/ImportQNBExcelSheetTemplate.aspx";
                        menu.Target = "contentForm";

                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ImportGasperTicket))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Import Gasper Tickets";
                        menu.NavigateUrl = "~/admin/ImportGasperTickets.aspx";
                        menu.Target = "contentForm";

                        menuAdmin.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewATMClassification))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "ATM Classification";
                        menu.NavigateUrl = "~/admin/Classification.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ImportCashOrders))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Import Cash Orders";
                        menu.NavigateUrl = "~/admin/ImportCashOrders.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewEventsManagement))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Events Management";
                        menu.NavigateUrl = "~/EventsManagement.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewSmsTemplate))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "SMS Template";
                        menu.NavigateUrl = "~/admin/SmsTemplate.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }
                    //******
                    ///edited by shariq 10sep 2015
                    if (CheckRight((int)Permissions.ViewSmsConfiguration))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "SMS Configuration";
                        menu.NavigateUrl = "~/admin/SmsConfiguration.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    ///edited by shariq 5 oct 2015
                    if (CheckRight((int)Permissions.ViewExceptionHandlingCriteria))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Exception Handling Criteria";
                        menu.NavigateUrl = "~/admin/ExceptionHandlingCriteria.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ApproveUser))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Approve User";
                        menu.NavigateUrl = "~/admin/ApproveUser.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ApproveGroup))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Approve Group";
                        menu.NavigateUrl = "~/admin/ApproveGroup.aspx";
                        menu.Target = "contentForm";
                        menuAdmin.Items.Add(menu);
                    }

                    //*****************************************************//
                    //Name : Aadil Damani
                    //Modified Date  : 25/05/2018
                    //Reason : Custom Report and Dashboard Admin Tab for ADIB
                    //*****************************************************//

                    if (CheckRight((int)Permissions.ReportTemplate))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt1 = new ComponentArt.Web.UI.MenuItem();
                        menuRpt1.Text = "Report Templates";
                        menuRpt1.NavigateUrl = "~/admin/ReportTemplate.aspx";
                        menuRpt1.Target = "contentForm";
                        menuAdmin.Items.Add(menuRpt1);
                    }

                    if (CheckRight((int)Permissions.DashboardTemplate))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt2 = new ComponentArt.Web.UI.MenuItem();
                        menuRpt2.Text = "Dashboard Templates";
                        menuRpt2.NavigateUrl = "~/admin/DashboardTemplate.aspx";
                        menuRpt2.Target = "contentForm";
                        menuAdmin.Items.Add(menuRpt2);
                    }
                    if (CheckRight((int)Permissions.ViewPackageList))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt2 = new ComponentArt.Web.UI.MenuItem();
                        menuRpt2.Text = "Package Management";
                        menuRpt2.NavigateUrl = "~/admin/Package.aspx";
                        menuRpt2.Target = "contentForm";
                        menuAdmin.Items.Add(menuRpt2);
                    }
                    if (CheckRight((int)Permissions.ViewATMBandwidth))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt2 = new ComponentArt.Web.UI.MenuItem();
                        menuRpt2.Text = "Bandwidth Management";
                        menuRpt2.NavigateUrl = "~/admin/ATMBandwidth.aspx";
                        menuRpt2.Target = "contentForm";
                        menuAdmin.Items.Add(menuRpt2);
                    }

                    menuAdmin.LookId = "TopItemLook";
                    menuAdmin.Look.RightIconUrl = "arrow.gif";
                    menuAdmin.Look.RightIconWidth = 10;
                    if (menuAdmin.Items.Count > 0)
                    {
                        //menuHome.Items[0].Items.Add(menuAdmin);
                        MenuAdmin.Items.Add(menuAdmin);
                        //Look-RightIconUrl="arrow.gif" Look-RightIconWidth="10"
                    }

                    //menuItem = new MenuItem("Administration", "Administration");
                    //menuItem.NavigateUrl = "~/admin/administration.aspx";
                    //menuMoveTo.ChildItems.Add(menuItem);
                }

                if (CheckRight((int)Permissions.ViewSMSTaskStatusMonitoring) ||
                    CheckRight((int)Permissions.ViewEjViewerWithOptions) ||
                    CheckRight((int)Permissions.ViewATMInventory) ||
                    CheckRight((int)Permissions.PrepareCashOrders) ||
                    CheckRight((int)Permissions.ATMNetworkInfo) ||
                    CheckRight((int)Permissions.ViewVaultSummary) ||
                    CheckRight((int)Permissions.VaultBalanceInvestigation) ||
                    CheckRight((int)Permissions.ViewLedger) ||
                    CheckRight((int)Permissions.ViewFraudTransactions) ||
                    CheckRight((int)Permissions.ViewReconciliation) ||
                    CheckRight((int)Permissions.ViewEJCashPositions) ||
                    CheckRight((int)Permissions.ViewEJCashPositionsDashBoard) ||
                    CheckRight((int)Permissions.ViewVaultSummaryDashboard) ||
                    CheckRight((int)Permissions.ViewDisputeManagement) ||
                    CheckRight((int)Permissions.ViewATMTransactionForecast) ||
                    CheckRight((int)Permissions.ATMTransactionAnalysis) ||
                    CheckRight((int)Permissions.UserTask) ||
                    CheckRight((int)Permissions.ViewTransactionsHourlyAnalysis) || CheckRight((int)Permissions.ViewEJBNATransactions) ||
                    CheckRight((int)Permissions.ViewCPMTransactions) || CheckRight((int)Permissions.ViewBNATransactions) ||
                    CheckRight((int)Permissions.ViewPNCBNATransactions) ||
                    CheckRight((int)Permissions.ViewCCMSReplenishment) ||
                    CheckRight((int)Permissions.ViewPNCCPMTransactions) ||
                    CheckRight((int)Permissions.ViewCallOutSummary) || CheckRight((int)Permissions.ViewCallOutList) || CheckRight((int)Permissions.ViewATMSettlement) ||
                    CheckRight((int)Permissions.ViewCardCaptured) || CheckRight((int)Permissions.ViewEJReplenishments) || CheckRight((int)Permissions.EJBalanceInvestigation) ||
                    CheckRight((int)Permissions.ViewEJTransactions) || CheckRight((int)Permissions.ViewCashPositionVsDFF) || CheckRight((int)Permissions.ViewReportTask) || CheckRight((int)Permissions.ViewMinThresholdDashboard) || CheckRight((int)Permissions.ViewCashPositions) || CheckRight((int)Permissions.ViewDepositDashboard) || CheckRight((int)Permissions.AlertMonitoringHistory) || CheckRight((int)Permissions.TaskStatusMonitoringHistory) || CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatus) || CheckRight((int)Permissions.ViewCashOrderMonitoringHistory) || CheckRight((int)Permissions.ViewOutOfBalanceInvestigation) || CheckRight((int)Permissions.ViewAlertDashBoard) || CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatusDashBoard) || CheckRight((int)Permissions.ViewTaskDashBoard)
                    || CheckRight((int)Permissions.ViewIMCCMSDashboard) || CheckRight((int)Permissions.ViewCashOrderDashBoard) || CheckRight((int)Permissions.ViewTransactions) || CheckRight((int)Permissions.ViewCashOrders)
                    || CheckRight((int)Permissions.UploadResource) || CheckRight((int)Permissions.UploadStatus)
                    || CheckRight((int)Permissions.ViewPowerMonitoringDashboard) || CheckRight((int)Permissions.ViewSensorMonitoringHistory)
                    || CheckRight((int)Permissions.ViewLinkMonitoringHistory) || CheckRight((int)Permissions.ViewGasperTicket) || CheckRight((int)Permissions.ViewBWACCashPositions)
                    )
                {
                    ComponentArt.Web.UI.MenuItem menuOperations = new ComponentArt.Web.UI.MenuItem();
                    menuOperations.Text = "Operations";
                    menuOperations.NavigateUrl = "~/admin/operations.aspx";
                    menuOperations.Target = "contentForm";


                    ComponentArt.Web.UI.MenuItem menuDashBoard = new ComponentArt.Web.UI.MenuItem();
                    menuDashBoard.Text = "DashBoards";
                    menuDashBoard.LookId = "TopItemLook";
                    menuDashBoard.Look.RightIconUrl = "arrow.gif";
                    menuDashBoard.Look.RightIconWidth = 10;


                    ComponentArt.Web.UI.MenuItem menuGeneral = new ComponentArt.Web.UI.MenuItem();
                    menuGeneral.Text = "General";
                    menuGeneral.LookId = "TopItemLook";
                    menuGeneral.Look.RightIconUrl = "arrow.gif";
                    menuGeneral.Look.RightIconWidth = 10;

                    ComponentArt.Web.UI.MenuItem menuCCMS = new ComponentArt.Web.UI.MenuItem();
                    menuCCMS.Text = "Currency And Cash Management System";
                    menuCCMS.LookId = "TopItemLook";
                    menuCCMS.Look.RightIconUrl = "arrow.gif";
                    menuCCMS.Look.RightIconWidth = 10;


                    ComponentArt.Web.UI.MenuItem menuEJ = new ComponentArt.Web.UI.MenuItem();
                    menuEJ.Text = "Electronic Journal";
                    menuEJ.LookId = "TopItemLook";
                    menuEJ.Look.RightIconUrl = "arrow.gif";
                    menuEJ.Look.RightIconWidth = 10;


                    ComponentArt.Web.UI.MenuItem menuSoftwareDistribution = new ComponentArt.Web.UI.MenuItem();
                    menuSoftwareDistribution.Text = "Software Distribution";
                    menuSoftwareDistribution.LookId = "TopItemLook";
                    menuSoftwareDistribution.Look.RightIconUrl = "arrow.gif";
                    menuSoftwareDistribution.Look.RightIconWidth = 10;

                    ComponentArt.Web.UI.MenuItem menuPowerMonitoring = new ComponentArt.Web.UI.MenuItem();
                    menuPowerMonitoring.Text = "Power Monitoring";
                    menuPowerMonitoring.LookId = "TopItemLook";
                    menuPowerMonitoring.Look.RightIconUrl = "arrow.gif";
                    menuPowerMonitoring.Look.RightIconWidth = 10;

                    ComponentArt.Web.UI.MenuItem menuReconciliation = new ComponentArt.Web.UI.MenuItem();
                    menuReconciliation.Text = "Reconciliation";
                    menuReconciliation.LookId = "TopItemLook";
                    menuReconciliation.Look.RightIconUrl = "arrow.gif";
                    menuReconciliation.Look.RightIconWidth = 10;



                    if (CheckRight((int)Permissions.ViewPowerMonitoringDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        string friendlyName = System.Web.Configuration.WebConfigurationManager.AppSettings["powerMonitoringAlias"];
                        if (friendlyName.Length == 0)
                            menu.Text = "Power Monitoring Dashboard";
                        else
                            menu.Text = friendlyName + " Dashboard";

                        menu.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_PowerMonitoring.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";

                        //  menu.NavigateUrl = "~/ccms/UploadResource.aspx";
                        menu.Target = "contentForm";
                        menuPowerMonitoring.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewReconciliation))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Reconciliation Batch";
                        menu.NavigateUrl = "~/ccms/Reconciliation.aspx";
                        menu.Target = "contentForm";
                        menuReconciliation.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewSwitchDispenseTrxn))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "View Switch Dispensed Transactions";
                        menu.NavigateUrl = "~/CCMS/ViewSwitchDispensedTx.aspx";
                        menu.Target = "contentForm";
                        menuReconciliation.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewSwitchBnaTrxn))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "View Switch Deposit Transactions";
                        menu.NavigateUrl = "~/CCMS/ViewSwitchDepositTx.aspx";
                        menu.Target = "contentForm";
                        menuReconciliation.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewHostDispenseTrxn))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "View Host Dispensed Transactions";
                        menu.NavigateUrl = "~/CCMS/ViewHostDispensedTx.aspx";
                        menu.Target = "contentForm";
                        menuReconciliation.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewHostBnaTrxn))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = " View Host Deposit Transactions";
                        menu.NavigateUrl = "~/CCMS/ViewHostDepositTx.aspx";
                        menu.Target = "contentForm";
                        menuReconciliation.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewReconFileImportHistory))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Bulk Import History";
                        menu.NavigateUrl = "~/CCMS/ATMReconciliationDataFeedFiles.aspx";
                        menu.Target = "contentForm";
                        menuReconciliation.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewSensorMonitoringHistory))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Sensor Monitoring ( History )";
                        menu.NavigateUrl = "~/ccms/SensorMonitoring.aspx";
                        menu.Target = "contentForm";
                        menuPowerMonitoring.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewLinkMonitoringHistory))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Link Monitoring ( History )";
                        menu.NavigateUrl = "~/ccms/LinkMonitoring.aspx";
                        menu.Target = "contentForm";
                        menuPowerMonitoring.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewGasperTicket))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Gasper Ticket ( History )";
                        menu.NavigateUrl = "~/ccms/GasperTicketHistory.aspx";
                        menu.Target = "contentForm";
                        menuPowerMonitoring.Items.Add(menu);
                    }



                    if (CheckRight((int)Permissions.UploadResource))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Package Scheduling";
                        menu.NavigateUrl = "~/ccms/UploadResource.aspx";
                        menu.Target = "contentForm";
                        menuSoftwareDistribution.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.UploadStatus))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Package Distribution Status ( History )";
                        menu.NavigateUrl = "~/ccms/UploadStatus.aspx";
                        menu.Target = "contentForm";
                        menuSoftwareDistribution.Items.Add(menu);
                    }


                    if (CheckRight((int)Permissions.AlertMonitoringHistory))
                    {
                        ComponentArt.Web.UI.MenuItem menuAgentMonitoring = new ComponentArt.Web.UI.MenuItem();
                        menuAgentMonitoring.Text = "Alert Monitoring ( History )";
                        menuAgentMonitoring.NavigateUrl = "~/ccms/AgentMonitoring.aspx";
                        menuAgentMonitoring.Target = "contentForm";
                        menuGeneral.Items.Add(menuAgentMonitoring);
                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.TaskStatusMonitoringHistory))
                    {
                        ComponentArt.Web.UI.MenuItem menuTerminalMonitoring = new ComponentArt.Web.UI.MenuItem();
                        menuTerminalMonitoring.Text = "Task Status Monitoring ( History )";
                        menuTerminalMonitoring.NavigateUrl = "~/ccms/TerminalFileStatusMonitoring.aspx";
                        menuTerminalMonitoring.Target = "contentForm";

                        //menuTerminalMonitoring.Look.LeftIconUrl = "~/images/Icon_taskStatusMonitoring.jpg";
                        //menuTerminalMonitoring.Look.LeftIconWidth = 50;
                        //menuTerminalMonitoring.Look.LeftIconHeight = 50;


                        menuGeneral.Items.Add(menuTerminalMonitoring);
                    }
                    if (CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatus))
                    {
                        ComponentArt.Web.UI.MenuItem menuOrderStatus = new ComponentArt.Web.UI.MenuItem();
                        menuOrderStatus.Text = "Order Download And Daily Feed Status ( History )";
                        menuOrderStatus.NavigateUrl = "~/ccms/FTPFileTransferStatus.aspx";
                        menuOrderStatus.Target = "contentForm";
                        //menuOrderStatus.Look.LeftIconUrl = "~/images/Icon_fileTransferStatus.jpg";
                        //menuOrderStatus.Look.LeftIconWidth = 50;
                        //menuOrderStatus.Look.LeftIconHeight = 50;

                        menuGeneral.Items.Add(menuOrderStatus);
                    }
                    if (CheckRight((int)Permissions.ViewCashOrderMonitoringHistory))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashOrder = new ComponentArt.Web.UI.MenuItem();
                        menuCashOrder.Text = "Cash Order Monitoring ( History )";
                        menuCashOrder.NavigateUrl = "~/ccms/CashOrderMonitoring.aspx";
                        menuCashOrder.Target = "contentForm";
                        menuGeneral.Items.Add(menuCashOrder);
                    }
                    if (CheckRight((int)Permissions.ViewLedger))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashOrder = new ComponentArt.Web.UI.MenuItem();
                        menuCashOrder.Text = "Vault Ledger ( History )";
                        menuCashOrder.NavigateUrl = "~/ccms/VaultLedger.aspx";
                        menuCashOrder.Target = "contentForm";
                        menuGeneral.Items.Add(menuCashOrder);
                    }

                    if (CheckRight((int)Permissions.ViewOutOfBalanceInvestigation))
                    {
                        ComponentArt.Web.UI.MenuItem menuBalance = new ComponentArt.Web.UI.MenuItem();
                        menuBalance.Text = "Balance Investigation (History)";
                        menuBalance.NavigateUrl = "~/ccms/OutOFBalanceInvestigation.aspx";
                        menuBalance.Target = "contentForm";
                        menuCCMS.Items.Add(menuBalance);
                    }
                    if (CheckRight((int)Permissions.ViewCashPositionVsDFF))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashPosVsDFF = new ComponentArt.Web.UI.MenuItem();
                        menuCashPosVsDFF.Text = "Cash Position Vs DFF (History)";
                        menuCashPosVsDFF.NavigateUrl = "~/CCMS/Dashboards/DashBoardGrid_CashPosVsDff.aspx";
                        //menuAlertDashBoard.NavigateUrl = "~/ccms/OpenDashBoard.aspx";
                        menuCashPosVsDFF.Target = "contentForm";
                        menuCCMS.Items.Add(menuCashPosVsDFF);
                    }

                    if (CheckRight((int)Permissions.ViewAlertDashBoard))
                    {
                        ComponentArt.Web.UI.MenuItem menuAlertDashBoard = new ComponentArt.Web.UI.MenuItem();
                        menuAlertDashBoard.Text = "Alert DashBoard";
                        //menuAlertDashBoard.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_Alerts.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        //menuAlertDashBoard.NavigateUrl = "~/ccms/Dashboards/DashBoard_Home.aspx?pageID=1";
                        menuAlertDashBoard.NavigateUrl = "~/ccms/Dashboards/DashBoard_Alerts.aspx";
                        //Session["pageID"] = "1";
                        //menuAlertDashBoard.NavigateUrl = "~/ccms/OpenDashBoard.aspx";
                        menuAlertDashBoard.Target = "contentForm";
                        //menuOperations.Items.Add(menuAlertDashBoard);
                        menuDashBoard.Items.Add(menuAlertDashBoard);
                    }

                    if (CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatusDashBoard))
                    {
                        ComponentArt.Web.UI.MenuItem menuFTPDashBoard = new ComponentArt.Web.UI.MenuItem();
                        menuFTPDashBoard.Text = "Order Download And Daily Feed Status DashBoard";
                        menuFTPDashBoard.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_FTP.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        //menuAlertDashBoard.NavigateUrl = "~/ccms/OpenDashBoard.aspx";
                        //menuAlertDashBoard.Target = "contentForm";
                        menuDashBoard.Items.Add(menuFTPDashBoard);

                    }

                    if (CheckRight((int)Permissions.ViewTaskDashBoard))
                    {
                        ComponentArt.Web.UI.MenuItem menuTaskDashBoard = new ComponentArt.Web.UI.MenuItem();
                        menuTaskDashBoard.Text = "Task DashBoard";
                        //menuTaskDashBoard.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_TAsks.aspx','','location=0,status=1,resizable=yes, scrollbars=yes');";
                        //menuTaskDashBoard.NavigateUrl = "~/ccms/Dashboards/DashBoard_Home.aspx?pageID=6";
                        menuTaskDashBoard.NavigateUrl = "~/ccms/Dashboards/DashBoard_Task.aspx";
                        //Session["pageID"] = "6";
                        menuTaskDashBoard.Target = "contentForm";
                        menuDashBoard.Items.Add(menuTaskDashBoard);

                    }

                    if (CheckRight((int)Permissions.ViewCashOrderMonitoringHistory))
                    {
                        ComponentArt.Web.UI.MenuItem menuViewCashOrder = new ComponentArt.Web.UI.MenuItem();
                        menuViewCashOrder.Text = "Cash Order DashBoard";
                        menuViewCashOrder.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_CashOrderMonitoring.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        //menuAlertDashBoard.NavigateUrl = "~/ccms/OpenDashBoard.aspx";
                        //menuViewCashOrder.Target = "contentForm";
                        menuDashBoard.Items.Add(menuViewCashOrder);
                    }
                    if (CheckRight((int)Permissions.ViewCashPositionsDashBoard))
                    {
                        ComponentArt.Web.UI.MenuItem menuViewCashPos = new ComponentArt.Web.UI.MenuItem();
                        menuViewCashPos.Text = "Cash Positions DashBoard";
                        //menuViewCashOrder.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_CashPositions.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        //menuViewCashPos.NavigateUrl = "~/ccms/Dashboards/DashBoard_Home.aspx?pageID=3";
                        menuViewCashPos.NavigateUrl = "~/ccms/Dashboards/DashBoard_CashPositions.aspx";
                        //Session["pageID"] = "3";
                        menuViewCashPos.Target = "contentForm";
                        menuDashBoard.Items.Add(menuViewCashPos);

                    }
                    if (CheckRight((int)Permissions.ViewBWACCashPositions))
                    {
                        ComponentArt.Web.UI.MenuItem menuViewCashPos = new ComponentArt.Web.UI.MenuItem();
                        menuViewCashPos.Text = "BWAC Cash Positions DashBoard";
                        //menuViewCashOrder.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_CashPositions.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        //menuViewCashPos.NavigateUrl = "~/ccms/Dashboards/DashBoard_Home.aspx?pageID=7";
                        menuViewCashPos.NavigateUrl = "~/ccms/Dashboards/DashBoard_BWACCashPosition.aspx";
                        Session["pageID"] = "7";
                        menuViewCashPos.Target = "contentForm";
                        menuDashBoard.Items.Add(menuViewCashPos);

                    }



                    if (CheckRight((int)Permissions.ViewEJCashPositionsDashBoard))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "EJ Cash Positions DashBoard";
                        menu.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_EJCashPositions.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        //menuAlertDashBoard.NavigateUrl = "~/ccms/OpenDashBoard.aspx";
                        //menuViewCashOrder.Target = "contentForm";
                        menuDashBoard.Items.Add(menu);

                    }
                    if (CheckRight((int)Permissions.ViewDepositDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem menuDepositDashboard = new ComponentArt.Web.UI.MenuItem();
                        //menuDepositDashboard.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_Deposits.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        menuDepositDashboard.Text = "BNA & CPM Monitoring";
                        //menuDepositDashboard.NavigateUrl = "~/ccms/Dashboards/DashBoard_Home.aspx?pageID=4";
                        menuDepositDashboard.NavigateUrl = "~/ccms/Dashboards/DashBoard_Deposite.aspx";
                        //Session["pageID"] = "4"; //deposite
                        //menuDepositDashboard.NavigateUrl = "~/ccms/dashboards/DashBoardGrid_Home.aspx?pageID=4";
                        menuDepositDashboard.Target = "contentForm";
                        menuDashBoard.Items.Add(menuDepositDashboard);

                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ViewVaultSummaryDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem menuDashboard = new ComponentArt.Web.UI.MenuItem();
                        menuDashboard.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_VaultSummary.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        menuDashboard.Text = "Vault Balance Monitoring";
                        //menuDepositDashboard.NavigateUrl = "~/ccms/dashboards/DashBoardGrid_Deposits.aspx";
                        menuDashboard.Target = "contentForm";
                        menuDashBoard.Items.Add(menuDashboard);

                        //MenuAdmin.Items.Add(menuAdmin);
                    }
                    if (CheckRight((int)Permissions.ViewIMCCMSDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem menuDashboard = new ComponentArt.Web.UI.MenuItem();
                        menuDashboard.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_IMCCMSSummary.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        menuDashboard.Text = "IM CCMS Summary Dashboard";
                        //menuDepositDashboard.NavigateUrl = "~/ccms/dashboards/DashBoardGrid_Deposits.aspx";
                        menuDashboard.Target = "contentForm";
                        menuDashBoard.Items.Add(menuDashboard);

                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ViewCommsVsPowerMonitoringDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem menuDashboard = new ComponentArt.Web.UI.MenuItem();
                        menuDashboard.ClientSideCommand = "window.open('CCMS/Dashboards/Dashboard_PowerCommsDashboard.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        menuDashboard.Text = "Comms Vs Power Monitoring Dashboard";
                        //menuDepositDashboard.NavigateUrl = "~/ccms/dashboards/DashBoardGrid_Deposits.aspx";
                        menuDashboard.Target = "contentForm";
                        menuDashBoard.Items.Add(menuDashboard);

                        //MenuAdmin.Items.Add(menuAdmin);
                    }

                    if (CheckRight((int)Permissions.ViewCCMSReplenishment))
                    {
                        ComponentArt.Web.UI.MenuItem menuTrxn = new ComponentArt.Web.UI.MenuItem();
                        menuTrxn.Text = "Replenishments (History)";
                        menuTrxn.NavigateUrl = "~/ccms/CCMSReplenishments.aspx";
                        menuTrxn.Target = "contentForm";
                        menuCCMS.Items.Add(menuTrxn);
                    }

                    if (CheckRight((int)Permissions.ViewTransactions))
                    {
                        ComponentArt.Web.UI.MenuItem menuTrxn = new ComponentArt.Web.UI.MenuItem();
                        menuTrxn.Text = "Withdrawal Transactions (History)";
                        menuTrxn.NavigateUrl = "~/ccms/ViewTransactions.aspx";
                        menuTrxn.Target = "contentForm";
                        menuCCMS.Items.Add(menuTrxn);
                    }

                    if (CheckRight((int)Permissions.ViewEJTransactions))
                    {
                        ComponentArt.Web.UI.MenuItem menuTrxn = new ComponentArt.Web.UI.MenuItem();
                        menuTrxn.Text = "EJ Withdrawal Transactions (History)";
                        menuTrxn.NavigateUrl = "~/ccms/ViewEJTransactions.aspx";
                        menuTrxn.Target = "contentForm";
                        menuEJ.Items.Add(menuTrxn);
                    }
                    if (CheckRight((int)Permissions.ViewFraudTransactions))
                    {
                        ComponentArt.Web.UI.MenuItem menuTrxn = new ComponentArt.Web.UI.MenuItem();
                        menuTrxn.Text = "Exception handling (History)";
                        menuTrxn.NavigateUrl = "~/ccms/ViewEJTransactions.aspx?ej=2";
                        menuTrxn.Target = "contentForm";
                        menuEJ.Items.Add(menuTrxn);
                    }

                    if (CheckRight((int)Permissions.ViewEjViewerWithOptions))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "EJViewer";
                        menu.NavigateUrl = "~/CCMS/EjViewerWithOptions.aspx";
                        menu.Target = "contentForm";
                        menuEJ.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewCashOrders))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashOrders = new ComponentArt.Web.UI.MenuItem();
                        menuCashOrders.Text = "Cash Orders (History)";
                        menuCashOrders.NavigateUrl = "~/ccms/CashOrders.aspx";
                        menuCashOrders.Target = "contentForm";
                        menuCCMS.Items.Add(menuCashOrders);
                    }

                    if (CheckRight((int)Permissions.ViewCashPositions))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashPositions = new ComponentArt.Web.UI.MenuItem();
                        menuCashPositions.Text = "Cash Positions (History)";
                        menuCashPositions.NavigateUrl = "~/ccms/CashPositions.aspx";
                        menuCashPositions.Target = "contentForm";
                        menuCCMS.Items.Add(menuCashPositions);
                    }

                    if (CheckRight((int)Permissions.ViewEJCashPositions))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashPositions = new ComponentArt.Web.UI.MenuItem();
                        menuCashPositions.Text = "EJ Cash Positions (History)";
                        menuCashPositions.NavigateUrl = "~/ccms/CashPositions.aspx?ej=1";
                        menuCashPositions.Target = "contentForm";
                        menuEJ.Items.Add(menuCashPositions);
                    }

                    if (CheckRight((int)Permissions.ViewFraudTransactionsDashBoard))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Exception Handling DashBoard";
                        //menu.NavigateUrl = "~/ccms/Dashboards/DashBoard_Home.aspx?pageID=8";
                        menu.NavigateUrl = "~/ccms/Dashboards/DashBoard_ExceptionHandling.aspx";
                        //Session["pageID"] = "8";
                        menu.Target = "contentForm";
                        menuDashBoard.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewReconciliation))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Reconciliation Batch (History)";
                        menu.NavigateUrl = "~/ccms/Reconciliation.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewMinThresholdDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem menuMinThreshold = new ComponentArt.Web.UI.MenuItem();
                        menuMinThreshold.Text = "Minimum Threshold DashBoard";
                        menuMinThreshold.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_MinimumThreshold.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        menuDashBoard.Items.Add(menuMinThreshold);
                    }

                    if (CheckRight((int)Permissions.ViewPowerMonitoringDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem menuMinThreshold = new ComponentArt.Web.UI.MenuItem();
                        string friendlyName = System.Web.Configuration.WebConfigurationManager.AppSettings["powerMonitoringAlias"];
                        if (friendlyName.Length == 0)
                            menuMinThreshold.Text = "Power Monitoring Dashboard";
                        else
                            menuMinThreshold.Text = friendlyName + " Dashboard";
                        menuMinThreshold.ClientSideCommand = "window.open('CCMS/Dashboards/DashBoardGrid_PowerMonitoring.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        menuDashBoard.Items.Add(menuMinThreshold);
                    }

                    //*****************************************************//
                    //Name : Aadil Damani
                    //Modified Date  : 12/07/2018
                    //Reason : Executive Dashboard
                    //*****************************************************//
                    if (CheckRight((int)Permissions.ExecutiveDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem execDashMenu = new ComponentArt.Web.UI.MenuItem();
                        execDashMenu.Text = "Executive Dashboard";
                        //execDashMenu.ClientSideCommand = "window.open('CCMS/ExecDashboard.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        execDashMenu.NavigateUrl = "~/ccms/DashBoards/Dashboard_Executive.aspx";
                        execDashMenu.Target = "contentForm";
                        menuDashBoard.Items.Add(execDashMenu);
                    }

                    if (CheckRight((int)Permissions.TransactionMonitoringDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem transDashboard = new ComponentArt.Web.UI.MenuItem();
                        transDashboard.Text = "Transaction Monitoring Dashboard";
                        transDashboard.NavigateUrl = "~/ccms/DashBoards/Dashboard_Transaction.aspx";
                        //transDashboard.ClientSideCommand = "window.open('CCMS/DashBoards/TransactionDashboard.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        transDashboard.Target = "contentForm";
                        menuDashBoard.Items.Add(transDashboard);
                    }

                    if (CheckRight((int)Permissions.CashManagementDashboard))
                    {
                        ComponentArt.Web.UI.MenuItem cashDashboard = new ComponentArt.Web.UI.MenuItem();
                        cashDashboard.Text = "Cash Management Dashboard";
                        cashDashboard.NavigateUrl = "~/ccms/DashBoards/Dashboard_Cash.aspx";
                        //cashDashboard.ClientSideCommand = "window.open('CCMS/DashBoards/CashDashboard.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');";
                        cashDashboard.Target = "contentForm";
                        menuDashBoard.Items.Add(cashDashboard);
                    }

                    if (CheckRight((int)Permissions.ViewReportTask))
                    {
                        ComponentArt.Web.UI.MenuItem menuFileDownload = new ComponentArt.Web.UI.MenuItem();
                        menuFileDownload.Text = "Report Task Monitoring (History)";
                        menuFileDownload.NavigateUrl = "~/admin/ReportTask.aspx";
                        menuFileDownload.Target = "contentForm";
                        menuGeneral.Items.Add(menuFileDownload);
                    }
                    if (CheckRight((int)Permissions.ViewATMInventory))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "View ATM Inventory";
                        menu.NavigateUrl = "~/admin/Atms.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewCardCaptured))
                    {
                        ComponentArt.Web.UI.MenuItem menuCardCaptured = new ComponentArt.Web.UI.MenuItem();
                        menuCardCaptured.Text = "Cards Captured (History)";
                        menuCardCaptured.NavigateUrl = "~/ccms/cardcaptured.aspx";
                        menuCardCaptured.Target = "contentForm";
                        menuEJ.Items.Add(menuCardCaptured);
                    }

                    if (CheckRight((int)Permissions.ViewEJReplenishments))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "EJ Replenishments (History)";
                        menu.NavigateUrl = "~/ccms/EJReplenishments.aspx";
                        menu.Target = "contentForm";
                        menuEJ.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.EJBalanceInvestigation))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "EJ Balance Investigation (History)";
                        menu.NavigateUrl = "~/ccms/EJBalanceInvestigation.aspx";
                        menu.Target = "contentForm";
                        menuEJ.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewPNCBNATransactions))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "EJ BNA Transactions ( History )";
                        menu.NavigateUrl = "~/ccms/ViewEJBNATransactions.aspx";
                        menu.Target = "contentForm";

                        menuEJ.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewPNCCPMTransactions))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "EJ CPM Transactions ( History )";
                        menu.NavigateUrl = "~/ccms/ViewEJCPMTransactions.aspx";
                        menu.Target = "contentForm";

                        menuEJ.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewCallOutList))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Call Out (History)";
                        menu.NavigateUrl = "~/ccms/CallOutList.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewATMSettlement))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "ATM Settlements (History)";
                        menu.NavigateUrl = "~/ccms/ATMSettlementList.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewCallOutSummary))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Call Out Summary";
                        menu.NavigateUrl = "~/ccms/CallOutSummary.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewBNATransactions))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "BNA Transactions (History)";
                        menu.NavigateUrl = "~/ccms/ViewBNATransactions.aspx";
                        menu.Target = "contentForm";
                        menuCCMS.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewCPMTransactions))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "CPM Transactions (History)";
                        menu.NavigateUrl = "~/ccms/ViewCPMTransactions.aspx";
                        menu.Target = "contentForm";
                        menuCCMS.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewTransactionsHourlyAnalysis))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Transactions Hourly Analysis";
                        menu.NavigateUrl = "~/ccms/ViewTransactionsHourlyAnalysis.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.PrepareCashOrders))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Prepare Cash Orders";
                        menu.NavigateUrl = "~/ccms/PrepareCashOrders.aspx";
                        menu.Target = "contentForm";
                        menuCCMS.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewEJBNATransactions))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "EJ BNA Transactions (History)";
                        menu.NavigateUrl = "~/ccms/EJBNATransactions.aspx";
                        menu.Target = "contentForm";
                        menuEJ.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.UserTask))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "User Task (History)";
                        menu.NavigateUrl = "~/ccms/UserTask.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ATMTransactionAnalysis))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "ATM Transaction Analysis";
                        menu.NavigateUrl = "~/ccms/ATMTransactionAnalysis.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewATMTransactionForecast))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Forecast";
                        menu.NavigateUrl = "~/ccms/ForecastingList.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewDisputeManagement))
                    {
                        ComponentArt.Web.UI.MenuItem menuTrxn = new ComponentArt.Web.UI.MenuItem();
                        menuTrxn.Text = "Dispute Management";
                        menuTrxn.NavigateUrl = "~/ccms/ViewEJTransactions.aspx?q=1";
                        menuTrxn.Target = "contentForm";
                        menuGeneral.Items.Add(menuTrxn);
                    }
                    if (CheckRight((int)Permissions.VaultBalanceInvestigation))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Vault Balance Investigation";
                        menu.NavigateUrl = "~/ccms/VaultBalanceInvestigation.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewVaultSummary))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Vault Summary";
                        menu.NavigateUrl = "~/ccms/VaultSummary.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewVaultAdjustment))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Post Vault Adjustments";
                        menu.NavigateUrl = "~/admin/VaultAdjustments.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ATMNetworkInfo))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "ATM Network Info";
                        menu.NavigateUrl = "~/admin/ATMNetworkInfo.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }

                    if (CheckRight((int)Permissions.ViewSMSTaskStatusMonitoring))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "SMS Task Status Monitoring(History)";
                        menu.NavigateUrl = "~/CCMS/SMSTaskStatusMonitoring.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }

                    //Edited by Ali Shah on 10th Dec, 15
                    //To add 'Invoice Deduction Sheet Generation' feature in CCMS
                    if (CheckRight((int)Permissions.CalculateInvoiceDeductions))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Calculate Invoice Deductions";
                        menu.NavigateUrl = "~/CCMS/CalculateInvoiceDeductions.aspx";
                        menu.Target = "contentForm";
                        menuGeneral.Items.Add(menu);
                    }

                    menuOperations.LookId = "TopItemLook";
                    menuOperations.Look.RightIconUrl = "arrow.gif";
                    menuOperations.Look.RightIconWidth = 10;

                    if (menuGeneral.Items.Count > 0)
                        menuOperations.Items.Add(menuGeneral);
                    if (menuCCMS.Items.Count > 0 && CheckRight((int)Permissions.ViewCounterModule))
                        menuOperations.Items.Add(menuCCMS);
                    if (menuEJ.Items.Count > 0 && CheckRight((int)Permissions.ViewEjModule))
                        menuOperations.Items.Add(menuEJ);


                    if (menuDashBoard.Items.Count > 0)
                        menuOperations.Items.Add(menuDashBoard);

                    if (menuSoftwareDistribution.Items.Count > 0 && CheckRight((int)Permissions.ViewSoftwareDistributionModule))
                        menuOperations.Items.Add(menuSoftwareDistribution);

                    if (menuPowerMonitoring.Items.Count > 0 && CheckRight((int)Permissions.ViewPowerMonitoringModule))
                        menuOperations.Items.Add(menuPowerMonitoring);

                    if (menuReconciliation.Items.Count > 0 && CheckRight((int)Permissions.ViewReconciliationModule))
                        menuOperations.Items.Add(menuReconciliation);

                    if (menuOperations.Items.Count > 0)
                        MenuOperations.Items.Add(menuOperations);
                }

                //
                if (CheckRight((int)Permissions.ViewPowerMonitoringReport) ||
                    CheckRight((int)Permissions.ViewExceptionHandlingReport) ||
                    CheckRight((int)Permissions.ViewQNBBankSettlementReport) ||
                    CheckRight((int)Permissions.ViewBankSettlementReport) ||
                    CheckRight((int)Permissions.ViewCCMSAndCiTSettlement) ||
                    CheckRight((int)Permissions.PNCMeanTimeToFailureAnalysisReport) ||
                    CheckRight((int)Permissions.PNCDepositDetail) ||
                    CheckRight((int)Permissions.PNCWeeklyAnalysis) ||
                    CheckRight((int)Permissions.PNCIncidentAnalysis) ||
                    CheckRight((int)Permissions.PNCDepositSummary) ||
                    CheckRight((int)Permissions.ViewCashPositionReport) ||
                    CheckRight((int)Permissions.LateSettlementReport) ||
                    CheckRight((int)Permissions.MissingSettlementReport) ||
                    CheckRight((int)Permissions.VaultReport) ||
                    CheckRight((int)Permissions.ReplenishmentExceptionReport) ||
                    CheckRight((int)Permissions.ViewInvoiceSummaryReport) ||
                    CheckRight((int)Permissions.ViewInvoiceDetailReport) ||
                    CheckRight((int)Permissions.GLReport) ||
                    CheckRight((int)Permissions.ReconciliationReport) ||
                    CheckRight((int)Permissions.FraudTransactionsReport) ||
                    CheckRight((int)Permissions.EJCashWithdrawalReport) ||
                    CheckRight((int)Permissions.EJReplenishmentReturnedReport) ||
                    CheckRight((int)Permissions.EJCashUtilizationReport) ||
                    CheckRight((int)Permissions.EJReplenishmentReport) ||
                    CheckRight((int)Permissions.EJSwitchDispensingReport) ||
                    CheckRight((int)Permissions.EJATMWithoutTransaction24Hour) ||
                    CheckRight((int)Permissions.EJCassetteDispensingReport) ||
                    CheckRight((int)Permissions.ReplenishmentReturnedReport) ||
                    CheckRight((int)Permissions.CassetteFaultySummaryReport) ||
                    CheckRight((int)Permissions.ViewReplenishedRemainingNotesReport) ||
                    CheckRight((int)Permissions.ViewMinNotesThresholdReport) ||
                    CheckRight((int)Permissions.CallOutReport) ||
                    CheckRight((int)Permissions.MemberBankTransactionReport) || CheckRight((int)Permissions.ViewCardCaptureReport) ||
                    CheckRight((int)Permissions.CassetteDispensingReport) || CheckRight((int)Permissions.PurgeBinAlertsReport) || CheckRight((int)Permissions.LowBalanceReport) || CheckRight((int)Permissions.ATMWithoutTransaction24Hour) || CheckRight((int)Permissions.DeadATMsReport) || CheckRight((int)Permissions.ATMSummaryReport) || CheckRight((int)Permissions.SwitchDispensingReport) || CheckRight((int)Permissions.DenominationUtilizationReport) || CheckRight((int)Permissions.OutOfCashReport) || CheckRight((int)Permissions.NoCashWithdrawalReport) || CheckRight((int)Permissions.AlertMonitoringReport) || CheckRight((int)Permissions.CashWithdrawalReport) || CheckRight((int)Permissions.CPMCounterReport) || CheckRight((int)Permissions.BNACounterReport) || CheckRight((int)Permissions.CashOrderReport) || CheckRight((int)Permissions.FTPFileMonitoringReport) || CheckRight((int)Permissions.TaskStatusReport) || CheckRight((int)Permissions.TerminalDowntimeReport) || CheckRight((int)Permissions.ScheduleTrackingReport) || CheckRight((int)Permissions.SuspiciousReplenishmentReport)
                    || CheckRight((int)Permissions.CashUtilizationReport) ||
                    CheckRight((int)Permissions.ReplenishmentExceptionWithoutOrderReport) ||
                CheckRight((int)Permissions.ReplenishmentExceptionByAmountReport) ||
                CheckRight((int)Permissions.ReplenishmentExceptionByTimeReport)
                    || CheckRight((int)Permissions.ViewSoftwareDistributionModule))
                {
                    ComponentArt.Web.UI.MenuItem menuReports = new ComponentArt.Web.UI.MenuItem();
                    menuReports.Text = "Reports";
                    menuReports.NavigateUrl = "~/ccms/reportlist.aspx";
                    menuReports.Target = "contentForm";
                    menuReports.LookId = "TopItemLook";
                    menuReports.Look.RightIconUrl = "arrow.gif";
                    menuReports.Look.RightIconWidth = 10;


                    ComponentArt.Web.UI.MenuItem menuGeneral = new ComponentArt.Web.UI.MenuItem();
                    menuGeneral.Text = "General";
                    menuGeneral.NavigateUrl = "~/ccms/reportlist.aspx";
                    menuGeneral.Target = "contentForm";
                    menuGeneral.LookId = "TopItemLook";
                    menuGeneral.Look.RightIconUrl = "arrow.gif";
                    menuGeneral.Look.RightIconWidth = 10;

                    ComponentArt.Web.UI.MenuItem menuCCMS = new ComponentArt.Web.UI.MenuItem();
                    menuCCMS.Text = "Currency And Cash Management System";
                    menuCCMS.NavigateUrl = "~/ccms/reportlist.aspx";
                    menuCCMS.Target = "contentForm";
                    menuCCMS.LookId = "TopItemLook";
                    menuCCMS.Look.RightIconUrl = "arrow.gif";
                    menuCCMS.Look.RightIconWidth = 10;

                    ComponentArt.Web.UI.MenuItem menuEJ = new ComponentArt.Web.UI.MenuItem();
                    menuEJ.Text = "Electonic Journal";
                    menuEJ.NavigateUrl = "~/ccms/reportlist.aspx";
                    menuEJ.Target = "contentForm";
                    menuEJ.LookId = "TopItemLook";
                    menuEJ.Look.RightIconUrl = "arrow.gif";
                    menuEJ.Look.RightIconWidth = 10;

                    ComponentArt.Web.UI.MenuItem menuSD = new ComponentArt.Web.UI.MenuItem();
                    menuSD.Text = "Software Distribution";
                    menuSD.NavigateUrl = "~/ccms/reportlist.aspx";
                    menuSD.Target = "contentForm";
                    menuSD.LookId = "TopItemLook";
                    menuSD.Look.RightIconUrl = "arrow.gif";
                    menuSD.Look.RightIconWidth = 10;

                    if (CheckRight((int)Permissions.ViewReplenishedRemainingNotesReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Replenishment Remaining Notes Report";
                        menuRpt.NavigateUrl = "~/ccms/ReplenishmentRemainingNotesReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRpt);
                    }

                    if (CheckRight((int)Permissions.ViewMinNotesThresholdReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Min Notes Threshold Report";
                        menuRpt.NavigateUrl = "~/ccms/MinNotesThresholdReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRpt);
                    }

                    if (CheckRight((int)Permissions.CassetteDispensingReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Cassette Dispensing Report";
                        menuRpt.NavigateUrl = "~/ccms/CassetteDispensingReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.EJCassetteDispensingReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "EJ Cassette Dispensing Report";
                        menuRpt.NavigateUrl = "~/ccms/CassetteDispensingReport.aspx?ej=1";
                        menuRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.PurgeBinAlertsReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Purge Bin Alerts Report";
                        menuRpt.NavigateUrl = "~/ccms/PurgeBinAlertsReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.LowBalanceReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Low Balance";
                        menuRpt.NavigateUrl = "~/ccms/LowBalanceRpt.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.ATMWithoutTransaction24Hour))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "ATM Without Withdrawal 24 Hour";
                        menuRpt.NavigateUrl = "~/ccms/ATMWithoutTransaction24Hour.aspx";
                        menuRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.EJATMWithoutTransaction24Hour))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "EJ ATM Without Withdrawal 24 Hour";
                        menuRpt.NavigateUrl = "~/ccms/ATMWithoutTransaction24Hour.aspx?ej=1";
                        menuRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.DeadATMsReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuDeadATMsRpt = new ComponentArt.Web.UI.MenuItem();
                        menuDeadATMsRpt.Text = "Dead ATMs Report";
                        menuDeadATMsRpt.NavigateUrl = "~/ccms/DeadATMRpt.aspx";
                        menuDeadATMsRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuDeadATMsRpt);
                    }
                    if (CheckRight((int)Permissions.ATMSummaryReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuATMSummaryRpt = new ComponentArt.Web.UI.MenuItem();
                        menuATMSummaryRpt.Text = "ATM Summary Report";
                        menuATMSummaryRpt.NavigateUrl = "~/ccms/ATMSummaryReport.aspx";
                        menuATMSummaryRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuATMSummaryRpt);
                    }
                    if (CheckRight((int)Permissions.SwitchDispensingReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuSwitchDispensingRpt = new ComponentArt.Web.UI.MenuItem();
                        menuSwitchDispensingRpt.Text = "Switch Dispensed Logic Report";
                        menuSwitchDispensingRpt.NavigateUrl = "~/ccms/SwitchDispensingReport.aspx";
                        menuSwitchDispensingRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuSwitchDispensingRpt);
                    }
                    if (CheckRight((int)Permissions.EJSwitchDispensingReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuSwitchDispensingRpt = new ComponentArt.Web.UI.MenuItem();
                        menuSwitchDispensingRpt.Text = "EJ Switch Dispensed Logic Report";
                        menuSwitchDispensingRpt.NavigateUrl = "~/ccms/SwitchDispensingReport.aspx?ej=1";
                        menuSwitchDispensingRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuSwitchDispensingRpt);
                    }


                    if (CheckRight((int)Permissions.DenominationUtilizationReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuDenomUtilRpt = new ComponentArt.Web.UI.MenuItem();
                        menuDenomUtilRpt.Text = "Denomination Utilization Report";
                        menuDenomUtilRpt.NavigateUrl = "~/ccms/DenominationUtilizationReport.aspx";
                        menuDenomUtilRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuDenomUtilRpt);
                    }
                    if (CheckRight((int)Permissions.CashOrderReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashOrderRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashOrderRpt.Text = "Cash Order Report";
                        menuCashOrderRpt.NavigateUrl = "~/ccms/CashOrderReport.aspx";
                        menuCashOrderRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuCashOrderRpt);
                    }
                    if (CheckRight((int)Permissions.FTPFileMonitoringReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuFtpRpt = new ComponentArt.Web.UI.MenuItem();
                        menuFtpRpt.Text = "Order Download And Daily Feed Status Report";
                        menuFtpRpt.NavigateUrl = "~/ccms/FTPFileMonitoringReport.aspx";
                        menuFtpRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuFtpRpt);
                    }
                    if (CheckRight((int)Permissions.TaskStatusReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuTaskStatusRpt = new ComponentArt.Web.UI.MenuItem();
                        menuTaskStatusRpt.Text = "Task Status Report";
                        menuTaskStatusRpt.NavigateUrl = "~/ccms/TaskStatusReport.aspx";
                        menuTaskStatusRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuTaskStatusRpt);
                    }
                    if (CheckRight((int)Permissions.TerminalDowntimeReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuDowntimeRpt = new ComponentArt.Web.UI.MenuItem();
                        menuDowntimeRpt.Text = "Terminal Downtime Report";
                        menuDowntimeRpt.NavigateUrl = "~/ccms/TerminalDowntimeReport.aspx";
                        menuDowntimeRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuDowntimeRpt);
                    }

                    if (CheckRight((int)Permissions.ViewPowerMonitoringReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Power Monitoring Report";
                        menuRpt.NavigateUrl = "~/ccms/PowerMonitoringReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }

                    if (CheckRight((int)Permissions.ScheduleTrackingReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuScheduleRpt = new ComponentArt.Web.UI.MenuItem();
                        menuScheduleRpt.Text = "Schedule Tracking Report";
                        menuScheduleRpt.NavigateUrl = "~/ccms/ScheduleTrackingReport.aspx";
                        menuScheduleRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuScheduleRpt);
                    }

                    if (CheckRight((int)Permissions.ReconciliationReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Reconciliation Report";
                        menuRpt.NavigateUrl = "~/ccms/ReconciliationReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.GLReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "GL Summary Report";
                        menuRpt.NavigateUrl = "~/ccms/GLReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.ViewInvoiceDetailReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Invoice Detail Report";
                        menuRpt.NavigateUrl = "~/ccms/InvoiceDetailReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.ViewInvoiceSummaryReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Invoice Summary Report";
                        menuRpt.NavigateUrl = "~/ccms/InvoiceSummaryReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }

                    if (CheckRight((int)Permissions.SuspiciousReplenishmentReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Replenishment Report";
                        menuRepRpt.NavigateUrl = "~/ccms/SuspiciousReplenishmentReport.aspx";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }


                    if (CheckRight((int)Permissions.ViewCashPositionReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Cash Positions Report";
                        menuRepRpt.NavigateUrl = "~/ccms/CurrentCashPositionsReport.aspx";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }


                    if (CheckRight((int)Permissions.ReplenishmentExceptionReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Replenishment Exception Report";
                        menuRepRpt.NavigateUrl = "~/ccms/ReplenishmentExceptionReport.aspx";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }

                    if (CheckRight((int)Permissions.ReplenishmentExceptionWithoutOrderReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Replenishment Exception Report - Not Ordered By Bank";
                        menuRepRpt.NavigateUrl = "~/ccms/ReplenishmentExceptionWithoutOrderReport.aspx";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }

                    if (CheckRight((int)Permissions.ReplenishmentExceptionByAmountReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Replenishment Exception Report - By Amount";
                        menuRepRpt.NavigateUrl = "~/ccms/ReplenishmentExceptionByAmountReport.aspx";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }

                    if (CheckRight((int)Permissions.ReplenishmentExceptionByTimeReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Replenishment Exception Report - By Time";
                        menuRepRpt.NavigateUrl = "~/ccms/ReplenishmentExceptionByTimeReport.aspx";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }
                    if (CheckRight((int)Permissions.VaultReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Vault Transaction Report";
                        menuRepRpt.NavigateUrl = "~/ccms/VaultReport.aspx";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }
                    if (CheckRight((int)Permissions.MissingSettlementReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Missing Settlement Report";
                        menuRepRpt.NavigateUrl = "~/ccms/SettlementReport.aspx?t=1";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }
                    if (CheckRight((int)Permissions.LateSettlementReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "Late Settlement Report";
                        menuRepRpt.NavigateUrl = "~/ccms/SettlementReport.aspx?t=2";
                        menuRepRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRepRpt);
                    }



                    if (CheckRight((int)Permissions.EJReplenishmentReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRepRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRepRpt.Text = "EJ Replenishment Report";
                        //menuRepRpt.NavigateUrl = "~/ccms/SuspiciousReplenishmentReport.aspx?ej=1";
                        menuRepRpt.NavigateUrl = "~/ccms/SuspiciousEJReplenishmentReport.aspx";
                        menuRepRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuRepRpt);
                    }



                    if (CheckRight((int)Permissions.CashUtilizationReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "Cash Utilization Report";
                        menuCashRpt.NavigateUrl = "~/ccms/CashUtilizationReport.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuCashRpt);
                    }

                    if (CheckRight((int)Permissions.ViewBankSettlementReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "Bank Settlement Report";
                        menuCashRpt.NavigateUrl = "~/ccms/BankSettlementReport.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuCashRpt);
                    }



                    if (CheckRight((int)Permissions.ViewCCMSAndCiTSettlement))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "CIT CCMS Report";
                        menuCashRpt.NavigateUrl = "~/ccms/CITCCMSCounterReport.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuCashRpt);
                    }


                    if (CheckRight((int)Permissions.EJCashUtilizationReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "EJ Cash Utilization Report";
                        menuCashRpt.NavigateUrl = "~/ccms/CashUtilizationReport.aspx?ej=1";
                        menuCashRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuCashRpt);
                    }

                    if (CheckRight((int)Permissions.PNCDepositSummary))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "PNC Deposit Summary";
                        menuCashRpt.NavigateUrl = "~/ccms/PNCDepositSummary.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuCashRpt);
                    }

                    if (CheckRight((int)Permissions.PNCIncidentAnalysis))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "PNC Incident Analysis";
                        menuCashRpt.NavigateUrl = "~/ccms/PNCIncidentAnalysis.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuCashRpt);
                    }
                    if (CheckRight((int)Permissions.PNCWeeklyAnalysis))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "PNC Deposits Weekly Analysis";
                        menuCashRpt.NavigateUrl = "~/ccms/PNCDepositWeeklyAnalysis.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuCashRpt);
                    }



                    if (CheckRight((int)Permissions.PNCDepositDetail))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "Deposits Detail";
                        menuCashRpt.NavigateUrl = "~/ccms/PNCDepositDetail.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuCashRpt);
                    }

                    if (CheckRight((int)Permissions.PNCMeanTimeToFailureAnalysisReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "PNC Mean Time to Faiure Analysis";
                        menuCashRpt.NavigateUrl = "~/ccms/PNCMeanTimeToFailureAnalysis.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuCashRpt);

                        ComponentArt.Web.UI.MenuItem menuCashRpt1 = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt1.Text = "PNC Mean Time to Incident Faiure Analysis";
                        menuCashRpt1.NavigateUrl = "~/ccms/PNCMeanTimeToIncidentFailureAnalysis.aspx";
                        menuCashRpt1.Target = "contentForm";
                        menuEJ.Items.Add(menuCashRpt1);
                    }


                    // CheckRight((int)Permissions.PNCMeanTimeToFailureAnalysisReport)


                    if (CheckRight((int)Permissions.BNACounterReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuBNACounterRpt = new ComponentArt.Web.UI.MenuItem();
                        menuBNACounterRpt.Text = "BNA Counter Report";
                        menuBNACounterRpt.NavigateUrl = "~/ccms/BNACounterReport.aspx";
                        menuBNACounterRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuBNACounterRpt);
                    }

                    if (CheckRight((int)Permissions.CPMCounterReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCPMCounterRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCPMCounterRpt.Text = "CPM Counter Report";
                        menuCPMCounterRpt.NavigateUrl = "~/ccms/CPMCounterReport.aspx";
                        menuCPMCounterRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuCPMCounterRpt);
                    }

                    if (CheckRight((int)Permissions.ViewCPMCountClearedReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCPMCounterRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCPMCounterRpt.Text = "CPM Count Cleared Report";
                        menuCPMCounterRpt.NavigateUrl = "~/ccms/CPMCountClearedReport.aspx";
                        menuCPMCounterRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuCPMCounterRpt);
                    }


                    if (CheckRight((int)Permissions.CashWithdrawalReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashWithdrawalRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashWithdrawalRpt.Text = "Cash Withdrawal Report";
                        menuCashWithdrawalRpt.NavigateUrl = "~/ccms/CashWithdrawalsReport.aspx";
                        menuCashWithdrawalRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuCashWithdrawalRpt);
                    }

                    if (CheckRight((int)Permissions.FraudTransactionsReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashWithdrawalRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashWithdrawalRpt.Text = "Exception Handling Report";
                        menuCashWithdrawalRpt.NavigateUrl = "~/ccms/CashWithdrawalsReport.aspx?ej=2";
                        menuCashWithdrawalRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuCashWithdrawalRpt);
                    }

                    if (CheckRight((int)Permissions.EJCashWithdrawalReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashWithdrawalRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashWithdrawalRpt.Text = "EJ Cash Withdrawal Report";
                        menuCashWithdrawalRpt.NavigateUrl = "~/ccms/CashWithdrawalsReport.aspx?ej=1";
                        menuCashWithdrawalRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuCashWithdrawalRpt);
                    }

                    if (CheckRight((int)Permissions.AlertMonitoringReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuAlertMonitoringRpt = new ComponentArt.Web.UI.MenuItem();
                        menuAlertMonitoringRpt.Text = "Alert Monitoring Report";
                        menuAlertMonitoringRpt.NavigateUrl = "~/ccms/AlertReport.aspx";
                        menuAlertMonitoringRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuAlertMonitoringRpt);
                    }

                    if (CheckRight((int)Permissions.NoCashWithdrawalReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuNoCashWithdrawalRpt = new ComponentArt.Web.UI.MenuItem();
                        menuNoCashWithdrawalRpt.Text = "No Cash Withdrawal Report";
                        menuNoCashWithdrawalRpt.NavigateUrl = "~/ccms/NoCashWithdrawalReport.aspx";
                        menuNoCashWithdrawalRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuNoCashWithdrawalRpt);
                    }

                    if (CheckRight((int)Permissions.OutOfCashReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuOutOfCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuOutOfCashRpt.Text = "Out Of Cash Report";
                        menuOutOfCashRpt.NavigateUrl = "~/ccms/newOutOfCash.aspx";
                        menuOutOfCashRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuOutOfCashRpt);
                    }
                    if (CheckRight((int)Permissions.MemberBankTransactionReport))
                    {
                        ComponentArt.Web.UI.MenuItem menu = new ComponentArt.Web.UI.MenuItem();
                        menu.Text = "Member Bank Transaction Report";
                        menu.NavigateUrl = "~/ccms/MemberBankTransactionReport.aspx";
                        menu.Target = "contentForm";
                        menuEJ.Items.Add(menu);
                    }
                    if (CheckRight((int)Permissions.ViewCardCaptureReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Card Capture Report";
                        menuRpt.NavigateUrl = "~/ccms/CardCapturedReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuRpt);
                    }
                    if (CheckRight((int)Permissions.ViewQNBBankSettlementReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuCashRpt = new ComponentArt.Web.UI.MenuItem();
                        menuCashRpt.Text = "QNB Bank Settlement Report";
                        menuCashRpt.NavigateUrl = "~/ccms/QNBBankSettlementReport.aspx";
                        menuCashRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuCashRpt);
                    }
                    if (CheckRight((int)Permissions.ViewCallOutList))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Call Out Report";
                        menuRpt.NavigateUrl = "~/ccms/CallOutReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }

                    if (CheckRight((int)Permissions.ReplenishmentReturnedReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Replenishment Returned Report";
                        menuRpt.NavigateUrl = "~/ccms/ReplenishmentReturnedReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuCCMS.Items.Add(menuRpt);
                    }

                    if (CheckRight((int)Permissions.EJReplenishmentReturnedReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "EJ Replenishment Returned Report";
                        menuRpt.NavigateUrl = "~/ccms/ReplenishmentReturnedReport.aspx?ej=1";
                        menuRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuRpt);
                    }

                    if (CheckRight((int)Permissions.ViewExceptionHandlingReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Exception Handling Report";
                        menuRpt.NavigateUrl = "~/ccms/ExceptionHandlingReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuEJ.Items.Add(menuRpt);
                    }

                    //*****************************************************//
                    //Name : Aadil Damani
                    //Modified Date  : 22/05/2018
                    //Reason : Custom Report menu for ADIB
                    //*****************************************************//
                    //if (CheckRight((int)Permissions.CustomReports))
                    //{
                    //    ReportTemplate.ReportTemplateReader reportTemplateReader = ReportTemplate.ExecuteReader("is_active=1");
                    //    ComponentArt.Web.UI.MenuItem menuCustomReport = new ComponentArt.Web.UI.MenuItem();
                    //    menuCustomReport.Text = "Custom Report";
                    //    menuCustomReport.LookId = "TopItemLook";
                    //    menuCustomReport.Look.RightIconUrl = "arrow.gif";
                    //    menuCustomReport.Look.RightIconWidth = 10;
                    //    bool addCustomReportOption = false;
                    //    while (reportTemplateReader.Read())
                    //    {
                    //        addCustomReportOption = true;
                    //        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                    //        menuRpt.Text = reportTemplateReader.CurrentReportTemplate.ReportName;
                    //        menuRpt.NavigateUrl = string.Format("~/ccms/CustomReport.aspx?id={0}",
                    //            reportTemplateReader.CurrentReportTemplate.ReportId);
                    //        menuRpt.Target = "contentForm";
                    //        menuCustomReport.Items.Add(menuRpt);
                    //    }
                    //    reportTemplateReader.Close();
                    //    if (addCustomReportOption)
                    //        menuReports.Items.Add(menuCustomReport);
                    //}

                    if (CheckRight((int)Permissions.CassetteFaultySummaryReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Cassette Faulty Summary Report";
                        menuRpt.NavigateUrl = "~/ccms/CassetteFaultySummaryReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuGeneral.Items.Add(menuRpt);
                    }

                    if (CheckRight((int)Permissions.ViewUploadStatusReport))
                    {
                        ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                        menuRpt.Text = "Package Distribution History Report";
                        menuRpt.NavigateUrl = "~/ccms/PackageUploadStatusReport.aspx";
                        menuRpt.Target = "contentForm";
                        menuSD.Items.Add(menuRpt);
                    }

                    menuReports.LookId = "TopItemLook";
                    menuReports.Look.RightIconUrl = "arrow.gif";
                    menuReports.Look.RightIconWidth = 10;

                    if (menuGeneral.Items.Count > 0)
                        menuReports.Items.Add(menuGeneral);
                    if (menuCCMS.Items.Count > 0 && CheckRight((int)Permissions.ViewCounterModule))
                        menuReports.Items.Add(menuCCMS);
                    if (menuEJ.Items.Count > 0 && CheckRight((int)Permissions.ViewEjModule))
                        menuReports.Items.Add(menuEJ);
                    if (menuSD.Items.Count > 0 && CheckRight((int)Permissions.ViewSoftwareDistributionModule))
                        menuReports.Items.Add(menuSD);

                    if (menuReports.Items.Count > 0)
                        MenuReports.Items.Add(menuReports);
                }

                ComponentArt.Web.UI.MenuItem menuOthers = new ComponentArt.Web.UI.MenuItem();
                menuOthers.Text = "Others";
                menuOthers.NavigateUrl = "~/admin/other.aspx";
                menuOthers.Target = "contentForm";
                menuOthers.LookId = "TopItemLook";


                if (CheckRight((int)Permissions.ViewGraphList))
                {
                    ComponentArt.Web.UI.MenuItem menuGraphList = new ComponentArt.Web.UI.MenuItem();
                    menuGraphList.Text = "Graphs";
                    menuGraphList.NavigateUrl = "~/admin/other.aspx";
                    menuGraphList.Target = "contentForm";
                    menuOthers.Items.Add(menuGraphList);
                }

                //*****************************************************//
                //Name : Aadil Damani
                //Modified Date  : 25/05/2018
                //Reason : Custom Dashboard Tab for ADIB
                //*****************************************************//
                object res = ConnectionFactory.ExecuteScalar(@"SELECT COUNT(*) 
                                                        FROM dashboard_template 
                                                        WHERE is_active = 1 AND id in (
                                                                SELECT dashboard_id FROM dashboard_graph WHERE is_active = 1   
                                                                        )");
                if (res != null && (int)res > 0)
                {
                    ComponentArt.Web.UI.MenuItem menuCustomDashboard = new ComponentArt.Web.UI.MenuItem();
                    menuCustomDashboard.Text = "Dashboard";
                    menuCustomDashboard.NavigateUrl = "~/ccms/dashboardlist.aspx";
                    menuCustomDashboard.Target = "contentForm";
                    menuOthers.Items.Add(menuCustomDashboard);
                }


                if (menuOthers.Items.Count > 0)
                {
                    menuOthers.Look.RightIconUrl = "arrow.gif";
                    menuOthers.Look.RightIconWidth = 10;
                    MenuOthers.Items.Add(menuOthers);
                }


                AppUser appUser = ((AppUser)Session[SessionVars.user.ToString()]);
                Label_LoggedInUser.Text = appUser.UserFullName;
                if (appUser.UserLastLoginTime != null)
                    Label_LastLoggedInAt.Text = appUser.UserLastLoginTime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                //divWait.Visible = false;

                appUser.UserLastLoginTime = DateTime.Now;
                appUser.Save();
            }
            catch (Exception ex)
            {

                LogableTask task = LogableTask.NewTask("Main");
                task.Log(MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                task.EndTask();
            }

        }
        private void BuildTree()
        {
            try
            {
                bool allowEditRegion = CheckRight((int)Permissions.ModifyRegion);
                bool allowEditATM = CheckRight((int)Permissions.ModifyATM);
                bool noRegion = true;

                jsStr.Append("<script>var ary = [");
                jsStrLocation.Append("<script>var aryLocation = [");
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select title, atm_id, region_id,location from atm where is_active = 1 and atm_id in (select atm_id from user_atms where user_id = " + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ") order by title;" +
                                   "select region_name, region_id, parent_region_id, is_organization, mcn from region where is_active = 1 order by region_name";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                atmsDS = new DataSet();
                adapter.Fill(atmsDS);
                atmsDS.Relations.Add("ATMRegionRelation", atmsDS.Tables[1].Columns["region_id"], atmsDS.Tables[0].Columns["region_id"]);
                atmsDS.Relations.Add("RegionRegionRelation", atmsDS.Tables[1].Columns["parent_region_id"], atmsDS.Tables[1].Columns["region_id"], false);

                foreach (DataRow dbRow in atmsDS.Tables[1].Rows)
                {
                    if (dbRow.IsNull("parent_region_id"))
                    {
                        ComponentArt.Web.UI.TreeViewNode newNode = CreateRegionNode(dbRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
                        newNode.EditingEnabled = newNode.DraggingEnabled = allowEditRegion;
                        ATMTreeView.Nodes.Add(newNode);
                        jsStr.Append("['" + newNode.ID + "','" + newNode.Text + "'],");
                        PopulateSubTree(dbRow, newNode, allowEditRegion, allowEditATM, dbRow["mcn"].ToString());
                        noRegion = false;
                    }
                }
                foreach (DataRow dbRow in atmsDS.Tables[0].Rows)
                {
                    if (dbRow.IsNull("region_id"))
                    {
                        ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(dbRow["title"].ToString(), (int)dbRow["atm_id"], dbRow["location"].ToString());
                        childNode.EditingEnabled = childNode.DraggingEnabled = allowEditATM;
                        childNode.Nodes.Add(childNode);
                        jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                        if (dbRow["location"].ToString().Length > 0)
                            jsStrLocation.Append("['" + childNode.ID + "','" + dbRow["location"].ToString().Replace("'", "&#39;") + "'],");
                    }
                }

                jsStr[jsStr.Length - 1] = ']';
                jsStr.Append("</script>");
                jsStrLocation[jsStrLocation.Length - 1] = ']';
                jsStrLocation.Append("</script>");

                if (atmsDS.Tables[1].Rows.Count == 0)
                {
                    literal_jsArray.Text = "<script>var ary = new Array();var aryLocation = new Array(); </script>";
                }
                else
                {
                    literal_jsArray.Text = jsStr.ToString() + " " + jsStrLocation.ToString();
                }

                if (noRegion)
                    literal_jsArray.Text = "<script>var ary = new Array();var aryLocation = new Array(); </script>";
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("BuildTree");
                task.Log(MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                task.EndTask();
            }

        }

        private void PopulateSubTree(DataRow parentRegionRow, ComponentArt.Web.UI.TreeViewNode node, bool allowEditingRegion, bool allowEditingATM, string parentMCN)
        {
            foreach (DataRow childRow in atmsDS.Tables[1].Select("parent_region_id=" + parentRegionRow["region_id"]))
            {
                DataTable dt = (DataTable)Session[SessionVars.dtUserOrgs.ToString()];

                if (bool.Parse(childRow["is_organization"].ToString()))
                {
                    if (dt.Rows.Contains(childRow["mcn"].ToString()) || dt.Rows.Contains(parentMCN))
                    {
                        ComponentArt.Web.UI.TreeViewNode childNode = CreateRegionNode(childRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)childRow["region_id"], bool.Parse(childRow["is_organization"].ToString()));
                        node.Nodes.Add(childNode);
                        childNode.EditingEnabled = childNode.DraggingEnabled = allowEditingRegion;
                        jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                        PopulateSubTree(childRow, childNode, allowEditingRegion, allowEditingATM, parentMCN);
                    }
                }
                else
                {
                    string regionMCN = TraceOrganizationMCN(int.Parse(childRow["region_id"].ToString()));

                    if (dt.Rows.Contains(regionMCN) || dt.Rows.Contains(parentMCN))
                    {
                        ComponentArt.Web.UI.TreeViewNode childNode = CreateRegionNode(childRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)childRow["region_id"], bool.Parse(childRow["is_organization"].ToString()));
                        node.Nodes.Add(childNode);
                        childNode.EditingEnabled = childNode.DraggingEnabled = allowEditingRegion;
                        jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                        PopulateSubTree(childRow, childNode, allowEditingRegion, allowEditingATM, parentMCN);
                    }
                }
            }
            int atmCount = 0;
            foreach (DataRow childRow in parentRegionRow.GetChildRows("ATMRegionRelation"))
            {
                atmCount++;
                ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(childRow["title"].ToString(), (int)childRow["atm_id"], childRow["location"].ToString());
                node.Nodes.Add(childNode);
                childNode.EditingEnabled = childNode.DraggingEnabled = allowEditingATM;
                jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                if (childRow["location"].ToString().Length > 0)
                    jsStrLocation.Append("['" + childNode.ID + "','" + childRow["location"].ToString().Replace("'", "&#39;") + "'],");

                //jsStrLocation.Append("['" + childNode.ID + "','" + childRow["location"].ToString() + "'],");


            }
            if (atmCount > 0)
                node.ToolTip = "Total No. Of Atms = " + atmCount;
        }

        private ComponentArt.Web.UI.TreeViewNode CreateRegionNode(string text, bool expanded, int id, bool isOrganization)
        {
            ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();
            node.Text = text;
            node.ID = "r" + id;
            if (isOrganization)
            {
                node.ImageUrl = "icon_organization.png";
                if (node.ID == "r1")
                    node.DroppingEnabled = false;
                else
                    node.DroppingEnabled = true;

            }
            else
                node.ImageUrl = "icon_ATMRegionList.gif";

            node.Expanded = expanded;

            return node;
        }

        private ComponentArt.Web.UI.TreeViewNode CreateATMNode(string text, int id, string location)
        {
            ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();

            node.Text = text;
            node.ID = "a" + id;
            node.ImageUrl = "icon_atm.jpg";
            node.DroppingEnabled = false;
            node.ToolTip = location;

            return node;
        }
        //Problematic function.....

        protected string TraceOrganizationMCN(int region_id)
        {
            if (traceOrganizationMCN.Length == 0)
            {
                if (Region.LoadRegionByPk(region_id).IsOrganization == true)
                    return Region.LoadRegionByPk(region_id).MCN;
                else
                    return TraceOrganizationMCN(Region.LoadRegionByPk(region_id).ParentRegionId.Value);
            }
            else
                return traceOrganizationMCN;
        }

        protected void PanelWidthChanged(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            try
            {
                UserSetting userSettings = Session[SessionVars.UserSettings.ToString()] as UserSetting;
                userSettings.TreePanelWidth = int.Parse(e.Parameter);
                userSettings.Save();
            }
            catch
            {
            }
        }
        protected void Callback_SessionEnd_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            List<ActiveSessions> list = ((List<ActiveSessions>)Application["ActiveSessions"]);
            AppUser appUser = null;
            //Change on 02_dec_2014
            //if (list != null) 

            if (list != null && Session[SessionVars.user.ToString()] != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    appUser = ((AppUser)Session[SessionVars.user.ToString()]);
                    if (list[i].userID == appUser.UserLogin.ToLower())
                    {
                        ActiveSessions activeSession = list[i];
                        object obj = Application["sync"];
                        if (obj != null)
                        {
                            lock (obj)
                            {
                                list.Remove(activeSession);
                            }
                        }
                        Utility.BuildAuditLog("User successfully logged off", appUser.UserId, (int)Permissions.Login);
                        //Write log off
                    }
                }
            }

            //Cache.Remove(Session["username"].ToString());
            Session.Abandon();
            Session.RemoveAll();
        }
        protected void Callback_RegionAdd_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            try
            {
                if (!bool.Parse(Session[SessionVars.isRegionSelected.ToString()].ToString()) || e.Parameters[1] == "null")
                {
                    throw new Exception("Cannot add this region");
                }

                if (Session[SessionVars.SelectedRegion.ToString()].ToString() == "1")
                {
                    throw new Exception("Add Organization before going to add region");
                }

                Region newRegion = new Region();
                newRegion.RegionName = e.Parameters[0];
                newRegion.IsOrganization = false;
                newRegion.IsActive = true;
                newRegion.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                newRegion.CreationTime = DateTime.Now;

                if (e.Parameters[0] == null || e.Parameters[0].Equals(""))
                    throw new Exception("Blank Region Cannot be added");

                if (e.Parameters[1] == "null")//root
                    newRegion.Save();
                else if (e.Parameters[1][0] == 'r') // a region is selected, add new as its child
                {
                    newRegion.ParentRegionId = int.Parse(e.Parameters[1].Substring(1));
                    newRegion.Save();
                }
                else
                    throw new Exception("unable to determine parent node");

                Utility.BuildAuditLog(newRegion.RegionName + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateRegion);
                //e.Output.Write(@" <div id='confirm-dialog'><a href='javascript: void(0)' class='confirm' style=""text-decoration:underline"">Add a region</a></div>");
                e.Output.Write(@"<a href='javascript: void(0)' onclick='Dialog_AddRegion.Show()' id='AddRegion'>Add a Region</a>");

                if (!CheckRight((int)Permissions.ModifyRegion))
                    e.Output.Write("<script>AddNode('" + e.Parameters[0] + "','" + e.Parameters[1] + "','" + newRegion.RegionId + "',false,false); document.getElementById('NewRegionName').value='' </script>");
                else
                    e.Output.Write("<script>AddNode('" + e.Parameters[0] + "','" + e.Parameters[1] + "','" + newRegion.RegionId + "',true,false); document.getElementById('NewRegionName').value='' </script>");
            }
            catch (Exception ex)
            {
                e.Output.Write("<script>alert('An Error Occured! " + ex.Message + "');" + @"document.getElementById('NewRegionName').value=''</script><a href='javascript: void(0)' onclick='Dialog_AddRegion.Show()' id='AddRegion'>Add a Region</a>");

                //                e.Output.Write("<script>alert('An Error Occured! " + ex.Message + "');</script>");
                LogableTask task = LogableTask.NewTask("AddRegion");
                task.Log(MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                task.EndTask();
            }
        }

        protected void MoveNode(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            try
            {
                if (e.Parameters[0][0] == 'a' && e.Parameters[1][0] == 'r')
                {
                    if (Region.LoadRegionByPk(int.Parse(e.Parameters[1].Substring(1))).IsOrganization == false)
                    {
                        Atm atm = Atm.LoadAtm("atm_id=" + e.Parameters[0].Substring(1));
                        atm.RegionId = int.Parse(e.Parameters[1].Substring(1));
                        atm.Save();


                        int organization_id = AddEditATM.GetOrganization(atm.RegionId);
                        ConnectionFactory.ExecuteScalar("delete mcn_atms where atm_id = " + atm.ATMId);
                        McnAtms region_atm = new McnAtms();
                        region_atm.AtmId = atm.ATMId;
                        region_atm.Mcn = Region.LoadRegionByPk(organization_id).MCN;
                        region_atm.RegionId = organization_id;
                        region_atm.Save();


                        e.Output.Write("<script>ShowSuccess('ATM moved to new location')</script>");
                    }
                    else
                        throw new Exception("Cannot move atm in an organization");
                }
                else if (e.Parameters[0][0] == 'r' && e.Parameters[1][0] == 'r')
                {
                    Region org = Region.LoadRegion(Region.Columns.region_id.ToString() + "=" + e.Parameters[1].Substring(1));
                    Region region = Region.LoadRegion(Region.Columns.region_id.ToString() + "=" + e.Parameters[0].Substring(1));
                    region.ParentRegionId = int.Parse(e.Parameters[1].Substring(1));
                    //region.MCN = org.MCN;
                    region.Save();

                    Atm.AtmReader reader = Atm.ExecuteReader("REGION_ID = " + region.RegionId);
                    while (reader.Read())
                    {
                        ConnectionFactory.ExecuteScalar("delete mcn_atms where atm_id = " + reader.CurrentAtm.ATMId);
                        //McnAtms mcnAtm = McnAtms.LoadMcnAtms("REGION_ID = " + org.RegionId + " and atm_id = "+reader.CurrentAtm.ATMId);
                        //if (mcnAtm == null)
                        //{
                        McnAtms mcnAtm = new McnAtms();
                        mcnAtm.RegionId = org.RegionId;
                        mcnAtm.AtmId = reader.CurrentAtm.ATMId;
                        // }
                        mcnAtm.Mcn = org.MCN;
                        mcnAtm.Save();

                    }
                    reader.Close();
                    //int organization_id = AddEditATM.GetOrganization(atm.RegionId);
                    //ConnectionFactory.ExecuteScalar("delete mcn_atms where region_id = " + atm.ATMId);



                    if (region.IsOrganization)
                        e.Output.Write("<script>ShowSuccess('Organization moved to new location')</script>");
                    else
                        e.Output.Write("<script>ShowSuccess('Region moved to new location')</script>");
                }
                else
                    throw new ArgumentOutOfRangeException("e.parameters");
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("MoveNode");
                task.Log(MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                task.EndTask();
                e.Output.Write("<script>ShowError('Move operation failed! Detail: " + ex.Message + "')</script>");
            }
        }

        protected void Callback_Rename_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            try
            {
                if (e.Parameters[0][0] == 'a')
                {
                    if (e.Parameters[2] != e.Parameters[1]) // if the new name is different from the previous name
                    {
                        Atm tempATM = Atm.LoadAtm("atm_id=" + e.Parameters[0].Substring(1) + " and atm_id <>" + e.Parameter.Substring(1));
                        if (tempATM != null)
                        {
                            e.Output.Write("<script>window.parent.RenameNode('" + e.Parameters[2] + "'," + e.Parameter + ")</script>");
                            throw new Exception("ATM already exists with the same name provided");
                        }

                        Atm atm = Atm.LoadAtm("atm_id=" + e.Parameters[0].Substring(1));
                        //atm.AtmFriendlyName = atm.Title = e.Parameters[1];
                        atm.Title = e.Parameters[1];

                        atm.Save();
                        e.Output.Write("<script>ShowSuccess('ATM name changed from " + e.Parameters[2] + " to " + e.Parameters[1] + ".')</script>");
                    }
                }
                else if (e.Parameters[0][0] == 'r')
                {
                    if (e.Parameters[2] != e.Parameters[1]) // if the new name is different from the previous name
                    {
                        //Change done on 06 Jan 2014....To allow creating region with the same name..
                        //Region TempRegion = Region.LoadRegion(Region.Columns.region_name.ToString() + "='" + e.Parameters[1] + "' and region_id<>" + e.Parameters[0].Substring(1));
                        //if (TempRegion != null)
                        //{
                        //    e.Output.Write("<script>window.parent.RenameNode('" + e.Parameters[2] + "','" + e.Parameter + "')</script>");
                        //    throw new Exception("Region already exists with the same name provided");
                        //}

                        Region region = Region.LoadRegion("region_id=" + e.Parameters[0].Substring(1));
                        region.RegionName = e.Parameters[1];
                        region.Save();

                        Utility.BuildAuditLog(region.RegionName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyRegion);
                        if (region.IsOrganization)
                            e.Output.Write("<script>ShowSuccess('Organization name changed from " + e.Parameters[2] + " to " + e.Parameters[1] + ".')</script>");
                        else
                            e.Output.Write("<script>ShowSuccess('Region name changed from " + e.Parameters[2] + " to " + e.Parameters[1] + ".')</script>");
                    }
                }
                else
                    throw new ArgumentOutOfRangeException();
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("RenameNode");
                task.Log(MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                task.EndTask();
                e.Output.Write("<script>ShowError('Rename operation failed! Detail: " + ex.Message + "')</script>");
            }
        }

        private void SelectDefaultOrganization()
        {
            int count = 0;
            Session[SessionVars.isRegionSelected.ToString()] = true;
            Session[SessionVars.SelectedRegion.ToString()] = ConnectionFactory.ExecuteScalar("select top 1 region_id from region where is_organization=1 and region_id<>1 and is_active=1 order by region_id");
            Session["isOrgSelected"] = true;
            Session["selectedOrgID"] = Session[SessionVars.SelectedRegion.ToString()];
            Queue<int> regionQue = new Queue<int>(10);
            regionQue.Enqueue((int)Session[SessionVars.SelectedRegion.ToString()]);
            string selectedATMs = "-1";
            string inactiveATMs = "-1";
            string selectedRegions = regionQue.Peek().ToString();

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            DataSet ds = new DataSet();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select atm_id,is_active,region_id from atm where atm_id in (select atm_id from user_ATMs where user_id = " + this.appUser.UserId + ")" +
                              "select org_mcn from user_organization where user_id = " + this.appUser.UserId;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            while (regionQue.Count > 0)
            {
                /*
      cmd.CommandText = @"select title, atm_id, region_id,location from atm where is_active = 1 and atm_id in (select atm_id from user_atms where user_id = " + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ") order by title;" +
                  "select region_name, region_id, parent_region_id, is_organization, mcn from region where is_active = 1 order by region_name";
     
                 */
                DataRow[] drArray = ds.Tables[0].Select("region_id=" + regionQue.Peek());
                foreach (DataRow dr in drArray)
                {
                    if (bool.Parse(dr[1].ToString()))
                        selectedATMs += "," + dr[0];
                    else
                        inactiveATMs += "," + dr[0];
                }
                //Atm.AtmReader atmReader = Atm.ExecuteReader("region_id=" + regionQue.Peek() + " and atm_id in (select atm_id from user_ATMs where user_id = " + this.appUser.UserId + ")");
                //while (atmReader.Read())
                //{
                //    if (atmReader.CurrentAtm.IsActive)
                //        selectedATMs += "," + atmReader.CurrentAtm.ATMId.ToString();
                //    else
                //        inactiveATMs += "," + atmReader.CurrentAtm.ATMId.ToString();
                //}
                //atmReader.Close();

                //UserOrganization userOrg = UserOrganization.LoadUserOrganization("user_id = " + this.appUser.UserId);
                DataRow[] drRegion = atmsDS.Tables[1].Select("parent_region_id=" + regionQue.Peek());
                foreach (DataRow dr in drRegion)
                {
                    regionQue.Enqueue(int.Parse(dr["region_id"].ToString()));
                    selectedRegions += "," + dr["region_id"];
                    if (count == 0)
                    {
                        if (dr["mcn"].ToString() == ds.Tables[1].Rows[0]["org_mcn"].ToString())
                        {
                            if (bool.Parse(dr["is_organization"].ToString()))
                            {
                                Session["isOrgSelected"] = true;
                                Session["selectedOrgID"] = dr["region_id"];
                                count++;
                            }
                            else
                                Session["isOrgSelected"] = false;
                        }
                    }
                }
                //Region.RegionReader regionReader = Region.ExecuteReader("parent_region_id=" + regionQue.Peek());
                //while (regionReader.Read())
                //{
                //    regionQue.Enqueue(regionReader.CurrentRegion.RegionId);
                //    selectedRegions += "," + regionReader.CurrentRegion.RegionId;
                //    if (count == 0)
                //    {
                //        if (regionReader.CurrentRegion.MCN == ds.Tables[1].Rows[0]["org_mcn"])
                //        {
                //            if (regionReader.CurrentRegion.IsOrganization)
                //            {
                //                Session["isOrgSelected"] = true;
                //                Session["selectedOrgID"] = regionReader.CurrentRegion.RegionId;
                //                count++;
                //            }
                //            else
                //                Session["isOrgSelected"] = false;
                //        }
                //    }

                //}
                //regionReader.Close();

                regionQue.Dequeue();
            }

            Session[SessionVars.SelectedATMs.ToString()] = selectedATMs;
            Session[SessionVars.SelectedRegions.ToString()] = selectedRegions;
            Session[SessionVars.InactiveATMs.ToString()] = inactiveATMs;
            Literal_script.Text = "<script>SelectionCompleted('r1')</script>";

        }
        protected void Callback_nodeSelected_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            try
            {
                if (e.Parameter[0] == 'a')
                {
                    Session[SessionVars.SelectedATMs.ToString()] = e.Parameter.Substring(1);
                    Session[SessionVars.isRegionSelected.ToString()] = false;
                    Session["isOrgSelected"] = false;
                }
                else if (e.Parameter[0] == 'r')
                {
                    Session[SessionVars.isRegionSelected.ToString()] = true;
                    Session[SessionVars.SelectedRegion.ToString()] = e.Parameter.Substring(1);
                    Region region = Region.LoadRegionByPk(int.Parse(e.Parameter.Substring(1)));
                    if (region.IsOrganization)
                    {
                        Session["isOrgSelected"] = true;
                        Session["selectedOrgID"] = region.RegionId;
                    }
                    else
                        Session["isOrgSelected"] = false;


                    Queue<int> regionQue = new Queue<int>(10);

                    regionQue.Enqueue(int.Parse(e.Parameter.Substring(1)));
                    string selectedATMs = "-1";
                    string inactiveATMs = "-1";
                    string selectedRegions = regionQue.Peek().ToString();

                    while (regionQue.Count > 0)
                    {
                        Atm.AtmReader atmReader = Atm.ExecuteReader("region_id=" + regionQue.Peek() + " and atm_id in (select atm_id from user_ATMs where user_id = " + this.appUser.UserId + ")");
                        while (atmReader.Read())
                        {
                            if (atmReader.CurrentAtm.IsActive)
                                selectedATMs += "," + atmReader.CurrentAtm.ATMId.ToString();
                            else
                                inactiveATMs += "," + atmReader.CurrentAtm.ATMId.ToString();
                        }
                        atmReader.Close();

                        Region.RegionReader regionReader = Region.ExecuteReader("parent_region_id=" + regionQue.Peek(), Region.Columns.region_id);
                        while (regionReader.Read())
                        {
                            regionQue.Enqueue(regionReader.CurrentRegion.RegionId);
                            selectedRegions += "," + regionReader.CurrentRegion.RegionId;
                        }
                        regionReader.Close();

                        regionQue.Dequeue();
                    }

                    Session[SessionVars.SelectedATMs.ToString()] = selectedATMs;
                    Session[SessionVars.SelectedRegions.ToString()] = selectedRegions;
                    Session[SessionVars.InactiveATMs.ToString()] = inactiveATMs;

                }
                else
                    throw new ArgumentOutOfRangeException();

                e.Output.Write("<script>SelectionCompleted('" + e.Parameter + "')</script>");
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("SelectNode");
                task.Log(MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                task.EndTask();
                e.Output.Write("<script>ShowError('Selection could not be changed! Detail: " + ex.Message + "')</script>");
            }
        }

        protected void Callback_multiNodeSelection_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            try
            {
                Session[SessionVars.isRegionSelected.ToString()] = false;
                e.Parameter = e.Parameter.TrimEnd(',');
                string[] ids = e.Parameter.Split(',');
                string selectedATMs = "-1";
                Queue<int> regionQue = new Queue<int>(10);

                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i][0] == 'a')
                        selectedATMs += ids[i].Substring(1);
                    else if (ids[i][0] == 'r')
                    {
                        regionQue.Enqueue(int.Parse(ids[i].Substring(1)));

                        while (regionQue.Count > 0)
                        {
                            Atm.AtmReader atmReader = Atm.ExecuteReader("region_id=" + regionQue.Peek() + " and atm_id in (select atm_id from user_ATMs where user_id = " + this.appUser.UserId + ")");
                            while (atmReader.Read())
                                selectedATMs += "," + atmReader.CurrentAtm.ATMId.ToString();
                            atmReader.Close();

                            Region.RegionReader regionReader = Region.ExecuteReader("parent_region_id=" + regionQue.Peek(), Region.Columns.region_id);
                            while (regionReader.Read())
                                regionQue.Enqueue(regionReader.CurrentRegion.RegionId);
                            regionReader.Close();

                            regionQue.Dequeue();
                        }
                    }
                }

                Session[SessionVars.SelectedATMs.ToString()] = selectedATMs;
                e.Output.Write("<script>MutilNodeSelectionCompleted(new Array('" + e.Parameter.Replace(",", "','") + "'))</script>");
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("SelectMultipleNode");
                task.Log(MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                task.EndTask();
                e.Output.Write("<script>ShowError('Selection could not be changed! Detail: " + ex.Message + "')</script>");
            }
        }

        protected void Callback_deleteNode_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            try
            {
                Region region = Region.LoadRegionByPk(int.Parse(e.Parameter.Substring(1)));

                if (region.IsOrganization)
                {

                    if (e.Parameter.StartsWith("r") && CheckRight((int)Permissions.DeleteOrganization))
                    {

                        SqlConnection conn = ConnectionFactory.GetNewConnection();
                        conn.Open();
                        SqlTransaction trxn = conn.BeginTransaction();

                        if ((int)ConnectionFactory.ExecuteScalar("select count(region_id) from region  " +
    " where  parent_region_id = " + region.RegionId.ToString()) != 0)
                            throw new Exception("Cannot delete selected organization as it has one or more child objects");
                        //int id = region.RegionId;
                        try
                        {
                            ConnectionFactory.ExecuteQuery("delete region where region_id = " + region.RegionId + ";update ccms_organization set is_deleted = 1 where region_id=" + region.RegionId + ";delete mcn_atms where mcn='" + region.MCN + "';delete region_receipients where region_id =" + region.RegionId + ";delete region_cit where region_id =" + region.RegionId, trxn);
                            trxn.Commit();
                        }
                        catch (Exception ex)
                        {
                            trxn.Rollback();
                            throw;
                        }
                        finally
                        {
                            if (conn != null)
                                conn.Close();
                        }
                        //region.Delete();

                        if ((int)ConnectionFactory.ExecuteScalar("select count(region_id) from region  " +
    " where  mcn = '" + region.MCN + "'") == 0)
                        {

                            DataTable dt = ((DataTable)Session[SessionVars.dtUserOrgs.ToString()]);
                            if (dt != null)
                                dt.Rows.Remove(dt.Select("MCN = '" + region.MCN + "'")[0]);
                        }

                        e.Output.Write("<script>ATMTreeView.findNodeById('" + e.Parameter + "').remove(); ShowSuccess('Organization deleted.')</script>");

                        Utility.BuildAuditLog(region.RegionName + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteOrganization);

                    }
                    else
                        throw new Exception("You do not have permission to delete Organization");
                }
                else
                {
                    if (e.Parameter.StartsWith("r") && CheckRight((int)Permissions.DeleteRegion))
                    {

                        if ((int)ConnectionFactory.ExecuteScalar("select count(atm_id) from atm  " +
    " where  region_id = " + region.RegionId.ToString()) != 0)
                            throw new Exception("Cannot delete selected region as it has one or more active/inactive ATMs");

                        Region.DeleteRegions("region_id = " + e.Parameter.Substring(1));
                        e.Output.Write("<script>ATMTreeView.findNodeById('" + e.Parameter + "').remove(); ShowSuccess('Region deleted.')</script>");
                        Utility.BuildAuditLog(region.RegionName + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteRegion);

                    }
                    else
                        throw new Exception("You do not have permission to delete region");
                }
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("delete node");
                task.Log(MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                task.EndTask();

                e.Output.Write("<script>ShowError('Node could not be deleted! Detail: " + ex.Message + "')</script>");
            }
        }
        protected void ImageButton_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Abandon();
                Session.RemoveAll();
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("LogOff", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
            }

            Response.Redirect("signin.aspx");
        }


    }
}