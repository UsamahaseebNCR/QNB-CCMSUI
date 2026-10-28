using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Avanza.iSuite.UI;

namespace CCMSUI.admin
{
    public partial class Administration : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CheckRight((int)Permissions.ViewATMBandwidth))
                 DivATMBandwidth.Visible = false;
 
            if (!CheckRight((int)Permissions.ViewPackageList))
                DivPackageManagement.Visible = false;

            //edited by shariq 5oct
            if (!CheckRight((int)Permissions.ViewExceptionHandlingCriteria))
                DivExceptionHandlingCriteria.Visible = false;

            if (!CheckRight((int)Permissions.ViewSmsTemplate))
                DivSMSTemplate.Visible = false;

            //edited by shariq 10sep
            if (!CheckRight((int)Permissions.ViewSmsConfiguration))
                DivSMSConfiguration.Visible = false;

                
            if (!CheckRight((int)Permissions.ViewEventsManagement))
                DivEventsManagement.Visible = false;
 
            if (!CheckRight((int)Permissions.ImportCashOrders))
                DivImportCashOrders.Visible = false;
 

            if (!CheckRight((int)Permissions.ViewATMClassification))
                DivATMClassification.Visible = false;
 
            if (!CheckRight((int)Permissions.ImportGasperTicket))
                DivImportGasperTickets.Visible = false;



            if (!CheckRight((int)Permissions.ImportBWAC))
                DivImportBWAC.Visible = false;

            if (!CheckRight((int)Permissions.ImportQNBExcelSheetTemplate))
                DivImportQNBExcelSheet.Visible = false;



            if (!CheckRight((int)Permissions.AssignTemplate))
                DivAlertTemplates.Visible = false;

            if (!CheckRight((int)Permissions.AssignTemplate))
                DivAssignTemplate.Visible = false;

            if (!CheckRight((int)Permissions.DeviceTemplate))
                DivDeviceTemplate.Visible = false;

            if (!CheckRight((int)Permissions.Devices))
                DivDevices.Visible = false;

            if (!CheckRight((int)Permissions.ViewAlertTemplatesList))
                DivAlertTemplates.Visible = false;
            if (!CheckRight((int)Permissions.ViewInvestigation))
                DivInvestigation.Visible = false;

            if (!CheckRight((int)Permissions.ViewCheque))
                DivCheque.Visible = false;

            if (!CheckRight((int)Permissions.ViewInvoice))
                DivInvoice.Visible = false;

            if (!CheckRight((int)Permissions.ViewGLAccount))
                DivGLAccounts.Visible = false;

            if (!CheckRight((int)Permissions.ViewRulesList))
                DivAddEditRule.Visible = false;

            if (!CheckRight((int)Permissions.ImportATMSettlement))
                DivATMSettlement.Visible = false;

            if (!CheckRight((int)Permissions.ViewFITMapping))
                DivFITMapping.Visible = false;

            if (!CheckRight((int)Permissions.ImportFLMRecord))
                DivImportFLMMonitoring.Visible = DivImportFLM.Visible = false;

            if (!CheckRight((int)Permissions.DirectoryLookupAndFileDownloader))
                DivDirLookup.Visible = false;

            if (!CheckRight((int)Permissions.ViewFileType))
                Div_FileType.Visible = false;

            if (!CheckRight((int)Permissions.ViewScheduleReports))
                Div_ScheduleReport.Visible = false;

            if (!CheckRight((int)Permissions.ViewVault))
                DivVault.Visible = false;

            if (!CheckRight((int)Permissions.ViewNoteSetType))
                DivNoteSetType.Visible = false;


            if (!CheckRight((int)Permissions.ViewOrganization))
                DivOrganization.Visible = false;

            if (!CheckRight((int)Permissions.ViewCIT))
                DivCiT.Visible = false;

            if (!CheckRight((int)Permissions.ViewATMsList))
                DivATM.Visible = false;

            if (!CheckRight((int)Permissions.ViewUserList))
                DivUserManagement.Visible = false;

            if (!CheckRight((int)Permissions.ViewGroupList))
                DivGroups.Visible = false;

            if (!CheckRight((int)Permissions.ChangeConfiguration))
                DivAppConfig.Visible = false;

            if (!CheckRight((int)Permissions.ScheduleFileDownload))
                DivScheduleDownload.Visible = false;

            if (!CheckRight((int)Permissions.ViewAuditLog))
                DivAuditLog.Visible = false;

            if (!CheckRight((int)Permissions.ViewDailyFeedSchedule))
                DivScheduleDailyFeed.Visible = false;

            if (!CheckRight((int)Permissions.ViewAgentRemoteAccess))
                DivAgentRemoteAccess.Visible = false;

            if (!CheckRight((int)Permissions.ViewAlertTemplate))
                Div_AlertTemplate.Visible = false;

            if (!CheckRight((int)Permissions.ViewConfigureDailyFeed))
                Div1.Visible = false;

            if (!CheckRight((int)Permissions.ReportTemplate))
                DivReportTemplate.Visible = false;

            if (!CheckRight((int)Permissions.DashboardTemplate))
                DivDashboardTemplate.Visible = false;

            if (!CheckRight((int)Permissions.ApproveUser))
                DivApproveUser.Visible = false;

            if (!CheckRight((int)Permissions.ApproveGroup))
                DivApproveGroup.Visible = false;
        }
    }
}