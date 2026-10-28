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
using System.Data.SqlClient;
using Avanza.iSuite.UI;

namespace CCMSUI
{
    public partial class Home : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewSummary))
                    DivSummary.Visible = false;

                if (!CheckRight((int)Permissions.ViewOrganization) &&
                !CheckRight((int)Permissions.ViewCIT) &&
                !CheckRight((int)Permissions.ViewATMsList) &&
                !CheckRight((int)Permissions.ViewUserList) &&
                !CheckRight((int)Permissions.ViewGroupList) &&
                !CheckRight((int)Permissions.ViewConfiguration) &&
                !CheckRight((int)Permissions.ScheduleFileDownload) &&
                !CheckRight((int)Permissions.ViewAuditLog) &&
                !CheckRight((int)Permissions.ViewDailyFeedSchedule) &&
                !CheckRight((int)Permissions.ViewAgentRemoteAccess) && 
                !CheckRight((int)Permissions.ApproveUser) &&
                !CheckRight((int)Permissions.ApproveGroup))
                    DivAdmin.Visible = false;



                if (!CheckRight((int)Permissions.AlertMonitoringHistory, false) &&
                !CheckRight((int)Permissions.TaskStatusMonitoringHistory, false) &&
                !CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatus, false) &&
                !CheckRight((int)Permissions.ViewCashOrderMonitoringHistory, false) &&
                !CheckRight((int)Permissions.ViewOutOfBalanceInvestigation, false) &&
                !CheckRight((int)Permissions.ViewAlertDashBoard, false) &&
                !CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatus, false) &&
                !CheckRight((int)Permissions.ViewTaskDashBoard, false) &&
                !CheckRight((int)Permissions.ViewCashOrderDashBoard, false) &&
                !CheckRight((int)Permissions.ViewTransactions, false) &&
                !CheckRight((int)Permissions.ViewCashOrders, false))
                    DivOperations.Visible = false;



                if (!CheckRight((int)Permissions.CashOrderReport) && !CheckRight((int)Permissions.FTPFileMonitoringReport) && !CheckRight((int)Permissions.TaskStatusReport) && !CheckRight((int)Permissions.TerminalDowntimeReport) && !CheckRight((int)Permissions.ScheduleTrackingReport) && !CheckRight((int)Permissions.SuspiciousReplenishmentReport))
                    DivReports.Visible = false;


                if (!CheckRight((int)Permissions.ViewGraphList))
                    DivOthers.Visible = false;

            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session.Abandon();
        }


    }
}