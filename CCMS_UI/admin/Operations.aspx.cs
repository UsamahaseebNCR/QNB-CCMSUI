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
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

namespace CCMSUI.admin
{
    public partial class Operations : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                TabStrip_Operations.Tabs[1].Visible = CheckRight((int)Permissions.ViewCounterModule);
                TabStrip_Operations.Tabs[2].Visible = CheckRight((int)Permissions.ViewEjModule);
                TabStrip_Operations.Tabs[3].Visible = CheckRight((int)Permissions.ViewSoftwareDistributionModule);
                TabStrip_Operations.Tabs[4].Visible = CheckRight((int)Permissions.ViewPowerMonitoringModule);


                DivCalculateInvoiceDeductions.Visible = CheckRight((int)Permissions.CalculateInvoiceDeductions);

                DivViewIMCCMSDashboard.Visible = CheckRight((int)Permissions.ViewIMCCMSDashboard);
                DivPrepareCashOrders.Visible = CheckRight((int)Permissions.PrepareCashOrders);
                DivUploadResource.Visible = CheckRight((int)Permissions.UploadResource);
                DivUploadStatus.Visible = CheckRight((int)Permissions.UploadStatus);
                DivCCMSReplenishments.Visible = CheckRight((int)Permissions.ViewCCMSReplenishment);
                DivPowerMonitoingDashboard.Visible = CheckRight((int)Permissions.ViewPowerMonitoringDashboard);

                DivSensorMonitoringHistory.Visible = CheckRight((int)Permissions.ViewSensorMonitoringHistory);
                DivLinkMonitoringHistory.Visible = CheckRight((int)Permissions.ViewLinkMonitoringHistory);
                DivGasperTicketsHistory.Visible = CheckRight((int)Permissions.ViewGasperTicket);


                if (!CheckRight((int)Permissions.ViewSMSTaskStatusMonitoring))
                    DivSMSTaskStatusMonitoring.Visible = false;
                //Added by shariq on 4march
                if (!CheckRight((int)Permissions.ViewEjViewerWithOptions))
                    DivEjviewerWithOptions.Visible = false;


                if (!CheckRight((int)Permissions.ViewBWACCashPositions))
                    DivBWACCashPos.Visible = false;

                if (!CheckRight((int)Permissions.ViewATMDeviceStatus))
                    DivATMDetail.Visible = false;
                if (!CheckRight((int)Permissions.ViewCountsClearedHistory))
                    DivCountsClearedHistory.Visible = false;

                if (!CheckRight((int)Permissions.ViewEJCountsClearedHistory))
                    DivEjCashCountsCleared.Visible = false;




                if (!CheckRight((int)Permissions.ViewSummaryDashboard))
                    DivSummaryDashboard.Visible = false;


                if (!CheckRight((int)Permissions.ViewPNCBNATransactions))
                    DivPNCBNATrxn.Visible = false;
                if (!CheckRight((int)Permissions.ViewPNCCPMTransactions))
                    DivPNCCPMTrxn.Visible = false;


                if (!CheckRight((int)Permissions.ATMNetworkInfo))
                    DivATMNetworkInfo.Visible = false;
                if (!CheckRight((int)Permissions.ViewVaultAdjustment))
                    DivVaultAdjustments.Visible = false;
                if (!CheckRight((int)Permissions.ViewVaultSummaryDashboard))
                    DivVaultSummaryDashboard.Visible = false;
                if (!CheckRight((int)Permissions.ViewVaultSummary))
                    DivVaultSummary.Visible = false;

                if (!CheckRight((int)Permissions.VaultBalanceInvestigation))
                    DivVaultBalanceInvestigation.Visible = false;

                if (!CheckRight((int)Permissions.ViewLedger))
                    DivVaultLedgerHistory.Visible = false;

                if (!CheckRight((int)Permissions.ViewFraudTransactions))
                    DivFraudTransactions.Visible = false;

                if (!CheckRight((int)Permissions.ViewFraudTransactionsDashBoard, false))
                    DivFraudTrxnDashboard.Visible = false;

                if (!CheckRight((int)Permissions.ViewReconciliation, false))
                    DivReconciliation.Visible = false;

                if (!CheckRight((int)Permissions.ViewEJCashPositions, false))
                    DivEJCashPositions.Visible = false;
                if (!CheckRight((int)Permissions.ViewEJCashPositionsDashBoard, false))
                    DivEJCashPositionsDashBoard.Visible = false;
                if (!CheckRight((int)Permissions.ViewDisputeManagement))
                    DivDisputeManagement.Visible = false;
                if (!CheckRight((int)Permissions.UserTask))
                    DivUserTask.Visible = false;
                if (!CheckRight((int)Permissions.ViewATMTransactionForecast))
                    DivForecast.Visible = false;
                if (!CheckRight((int)Permissions.ATMTransactionAnalysis))
                    Div_ATMTrxnAnalysis.Visible = false;
                if (!CheckRight((int)Permissions.ViewEJBNATransactions))
                    DivEJBNATrxn.Visible = false;
                if (!CheckRight((int)Permissions.ViewTransactionsHourlyAnalysis))
                    DivTrxnHourlyAnalysis.Visible = false;
                if (!CheckRight((int)Permissions.ViewBNATransactions))
                    DivViewBNATransactions.Visible = false;
                if (!CheckRight((int)Permissions.ViewCPMTransactions))
                    DivViewCPMTransactions.Visible = false;
                if (!CheckRight((int)Permissions.ViewCallOutSummary))
                    DivCallOutSummary.Visible = false;
                if (!CheckRight((int)Permissions.ViewATMSettlement))
                    DivViewATMSettlement.Visible = false;

                if (!CheckRight((int)Permissions.ViewCallOutList))
                    DivViewCallOut.Visible = false;
                if (!CheckRight((int)Permissions.EJBalanceInvestigation))
                    DivEJBalanceInvestigation.Visible = false;
                if (!CheckRight((int)Permissions.ViewEJReplenishments))
                    DivEJRep.Visible = false;

                if (!CheckRight((int)Permissions.ViewATMInventory))
                    DivViewATMInventory.Visible = false;



                if (!CheckRight((int)Permissions.ViewCardCaptured))
                    DivCardCaptured.Visible = false;

                if (!CheckRight((int)Permissions.ViewEJTransactions))
                    DivEJTrxn.Visible = false;

                if (!CheckRight((int)Permissions.ViewCashPositionVsDFF))
                    DivCashPositionVsDFF.Visible = false;

                if (!CheckRight((int)Permissions.ViewReportTask))
                    DivReportTaskMonitoring.Visible = false;

                if (!CheckRight((int)Permissions.ViewDepositDashboard, false))
                    DivDepositMonitoring.Visible = false;

                if (!CheckRight((int)Permissions.ViewMinThresholdDashboard, false))
                    DivMinThreshold.Visible = false;

                if (!CheckRight((int)Permissions.AlertMonitoringHistory, false))
                    DivAgentMonitoring.Visible = false;

                if (!CheckRight((int)Permissions.TaskStatusMonitoringHistory, false))
                    DivTerminalFileStatusMonitoring.Visible = false;

                if (!CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatus, false))
                    DivFtpStatus.Visible = false;

                if (!CheckRight((int)Permissions.ViewCashOrderMonitoringHistory, false))
                    DivCashOrderMonitoring.Visible = false;

                if (!CheckRight((int)Permissions.ViewOutOfBalanceInvestigation, false))
                    DivOutOfBalanceInvestigation.Visible = false;

                if (!CheckRight((int)Permissions.ViewAlertDashBoard, false))
                    DivAlertDashBoard.Visible = false;

                if (!CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatusDashBoard, false))
                    DivFTPDashBoard.Visible = false;

                if (!CheckRight((int)Permissions.ViewTaskDashBoard, false))
                    DivTaskDashBoard.Visible = false;

                if (!CheckRight((int)Permissions.ViewCashOrderDashBoard, false))
                    DivCashOrderDashBoard.Visible = false;

                if (!CheckRight((int)Permissions.ViewCashPositionsDashBoard, false))
                    DivCashPositionsDashBoard.Visible = false;

                if (!CheckRight((int)Permissions.ViewTransactions, false))
                    DivViewTransactions.Visible = false;

                if (!CheckRight((int)Permissions.ViewCashOrders, false))
                    DivViewCashOrders.Visible = false;


                if (!CheckRight((int)Permissions.ViewCashPositions, false))
                    DivCashPositions.Visible = false;

                if (!CheckRight((int)Permissions.ExecutiveDashboard))
                    DivExecDashboard.Visible = false;

                if (!CheckRight((int)Permissions.TransactionMonitoringDashboard))
                    DivTransactionMonDashboard.Visible = false;

                if (!CheckRight((int)Permissions.CashManagementDashboard))
                    DivCashDashboard.Visible = false;


                string friendlyName = System.Web.Configuration.WebConfigurationManager.AppSettings["powerMonitoringAlias"];
                if (friendlyName.Length > 0)
                    Label_PowerMonitoring.Text = friendlyName + " Dashboard";

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("Operation", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }
    }
}