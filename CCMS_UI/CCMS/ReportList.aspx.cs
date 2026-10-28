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
using Avanza.CCMS.DAL;

namespace CCMSUI.CCMS
{
    public partial class ReportList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TabStrip_Operations.Tabs[1].Visible = CheckRight((int)Permissions.ViewCounterModule);
            TabStrip_Operations.Tabs[2].Visible = CheckRight((int)Permissions.ViewEjModule);
            
            TabStripTab_SoftwareDistribution.Visible = CheckRight((int)Permissions.ViewSoftwareDistributionModule);
            DIVSoftwareDistributionHistoryReport.Visible = CheckRight((int)Permissions.ViewUploadStatusReport);
            //if (System.Configuration.ConfigurationManager.AppSettings["isCounterOptionDisabled"] == "1")
            //    TabStrip_Operations.Tabs[1].Visible = false;
            //else if (System.Configuration.ConfigurationManager.AppSettings["isEJEnabled"] == "0")
            //    TabStrip_Operations.Tabs[2].Visible = false;

            DivPowerMonitoringReport.Visible = CheckRight((int)Permissions.ViewPowerMonitoringReport);
            //edited by shariq on 29 Aug 2015
            DivExceptionHandlingReport.Visible = CheckRight((int)Permissions.ViewExceptionHandlingReport);
           //*****
            DivMinNotesThresholdReport.Visible = CheckRight((int)Permissions.ViewMinNotesThresholdReport);
            DivReplenishmentRemainingNotesReport.Visible = CheckRight((int)Permissions.ViewReplenishedRemainingNotesReport);
            DIVCITCCMSCounterReport.Visible = CheckRight((int)Permissions.ViewBankSettlementReport);
            DIVBankSettlementReport.Visible = CheckRight((int)Permissions.ViewBankSettlementReport);
            DIVQNBBankSettlementReport.Visible = CheckRight((int)Permissions.ViewQNBBankSettlementReport);
            DIVCassetteFaultySummary.Visible = CheckRight((int)Permissions.CassetteFaultySummaryReport);
            DIVMeanTimeToIncidenceFailureAnalysis.Visible = CheckRight((int)Permissions.PNCMeanTimeToFailureAnalysisReport);
            DIVMeanTimeToFailureAnalysis.Visible = CheckRight((int)Permissions.PNCMeanTimeToFailureAnalysisReport);
            DIVPNCDepositDetail.Visible = CheckRight((int)Permissions.PNCDepositDetail);
            DIVDepositWeeklyAnalysis.Visible = CheckRight((int)Permissions.PNCWeeklyAnalysis);
            DIVPNCIncidentAnalysis.Visible = CheckRight((int)Permissions.PNCIncidentAnalysis);
            DIVPNCDepositSummary.Visible = CheckRight((int)Permissions.PNCDepositSummary);
            DIVMissingSettlement.Visible = CheckRight((int)Permissions.MissingSettlementReport);
            DIVLateSettlement.Visible = CheckRight((int)Permissions.LateSettlementReport);

            DIVCurrentCashPositionsReport.Visible = CheckRight((int)Permissions.ViewCashPositionReport);

            DIVVaultReport.Visible = CheckRight((int)Permissions.VaultReport);
            DIVRepExceptionReport.Visible = CheckRight((int)Permissions.ReplenishmentExceptionReport);
            DIVInvoiceSummary.Visible = CheckRight((int)Permissions.ViewInvoiceSummaryReport);
            DIVInvoice.Visible = CheckRight((int)Permissions.ViewInvoiceDetailReport);
            DIVGLSummary.Visible = CheckRight((int)Permissions.GLReport);
            DIVReconcilation.Visible = CheckRight((int)Permissions.ReconciliationReport);
            DIVFraudTrxn.Visible = CheckRight((int)Permissions.FraudTransactionsReport);
            DIVEJCashWithdrawalsReport.Visible = CheckRight((int)Permissions.EJCashWithdrawalReport);
            DIVEJRepReturnedReport.Visible = CheckRight((int)Permissions.EJReplenishmentReturnedReport);
            DIVRepReturnedReport.Visible = CheckRight((int)Permissions.ReplenishmentReturnedReport);

            DIVEJCashUtilization.Visible = CheckRight((int)Permissions.EJCashUtilizationReport);

            DIVEjReplenishment.Visible = CheckRight((int)Permissions.EJReplenishmentReport);
            DIVEJSwitchDispensing.Visible = CheckRight((int)Permissions.EJSwitchDispensingReport);
            DIVEjATMWithoutTrxn.Visible = CheckRight((int)Permissions.EJATMWithoutTransaction24Hour);
            DIVEjCassetteDispensing.Visible = CheckRight((int)Permissions.EJCassetteDispensingReport);
            DIVCallOut.Visible = CheckRight((int)Permissions.CallOutReport);
            DIVCardCaptureReport.Visible = CheckRight((int)Permissions.ViewCardCaptureReport);
            DIVMemberBankTransactionReport.Visible = CheckRight((int)Permissions.MemberBankTransactionReport);
            DIVCassetteDispensing.Visible = CheckRight((int)Permissions.CassetteDispensingReport);
            DIVPurgeBinAlerts.Visible = CheckRight((int)Permissions.PurgeBinAlertsReport);
            DIVATMWithoutTrxn.Visible = CheckRight((int)Permissions.ATMWithoutTransaction24Hour);
            DIVLowBalance.Visible = CheckRight((int)Permissions.LowBalanceReport);
            DIVDeadATMs.Visible = CheckRight((int)Permissions.DeadATMsReport);
            DIVATMSummary.Visible = CheckRight((int)Permissions.ATMSummaryReport);
            DIVSwitchDispensing.Visible = CheckRight((int)Permissions.SwitchDispensingReport);
            DivCashOrderReport.Visible = CheckRight((int)Permissions.CashOrderReport);
            DivOrderDownloadDFFReport.Visible = CheckRight((int)Permissions.FTPFileMonitoringReport);
            DivTaskStatusReport.Visible = CheckRight((int)Permissions.TaskStatusReport);
            DivTerminalDowntimeReport.Visible = CheckRight((int)Permissions.TerminalDowntimeReport);
            DIVScheduleTrackingReport.Visible = CheckRight((int)Permissions.ScheduleTrackingReport);
            DIVReplenishment.Visible = CheckRight((int)Permissions.SuspiciousReplenishmentReport);
            DIVCashUtilization.Visible = CheckRight((int)Permissions.CashUtilizationReport);
            DIVBNACounterReport.Visible = CheckRight((int)Permissions.BNACounterReport);
            DIVCPMCounterReport.Visible = CheckRight((int)Permissions.CPMCounterReport);
            DIVCashWithdrawalsReport.Visible = CheckRight((int)Permissions.CashWithdrawalReport);
            DIV_AlertMonitoring.Visible = CheckRight((int)Permissions.AlertMonitoringReport);
            //DIVNoCashWithdrawals.Visible = CheckRight((int)Permissions.NoCashWithdrawalReport);
            DIVOutOfCash.Visible = CheckRight((int)Permissions.OutOfCashReport);
            DIVDenominationUtlization.Visible = CheckRight((int)Permissions.DenominationUtilizationReport);

            //DivPowerNetworkDownReport.Visible = CheckRight((int)Permissions.ViewPowerNetworkDownReport);
            DIVRepExceptionByAmountReport.Visible = CheckRight((int)Permissions.ReplenishmentExceptionByAmountReport);
            DIVRepExceptionByTimeReport.Visible = CheckRight((int)Permissions.ReplenishmentExceptionByTimeReport);
            DIVRepExceptionWithoutOrderReport.Visible = CheckRight((int)Permissions.ReplenishmentExceptionWithoutOrderReport);

            DIVCPMCountClearedReport.Visible = CheckRight((int)Permissions.ViewCPMCountClearedReport);

            if (CheckRight((int)Permissions.CustomReports))
            {
                ReportTemplate.ReportTemplateReader reportTemplateReader = ReportTemplate.ExecuteReader("is_active=1");
                TabStripTab_Custom.Visible = false;
                string customReportComponent = string.Empty;
                while (reportTemplateReader.Read())
                {
                    customReportComponent = string.Empty;
                    TabStripTab_Custom.Visible = true;
                    customReportComponent += string.Format(@"<div class='DIVHomePage'>
                            <table cellpadding='0' cellspacing='0' border='0'>
                                <tr style='background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                                    width: 158px;'>
                                    <td style='width: 158px; cursor: pointer; height: 100px;' onclick=window.open('CustomReport.aspx?id={0}')>
                                       {1}<br />
                                        <img alt='' src='../images/CPM-Counter.jpg' style='cursor: pointer' />
                                    </td>
                                </tr>
                            </table>
                        </div>", reportTemplateReader.CurrentReportTemplate.ReportId, reportTemplateReader.CurrentReportTemplate.ReportName);
                    //ComponentArt.Web.UI.MenuItem menuRpt = new ComponentArt.Web.UI.MenuItem();
                    //menuRpt.Text = reportTemplateReader.CurrentReportTemplate.ReportName;
                    //menuRpt.NavigateUrl = string.Format("~/ccms/CustomReport.aspx?id={0}",
                    //    reportTemplateReader.CurrentReportTemplate.ReportId);
                    //menuRpt.Target = "contentForm";
                    //menuCustomReport.Items.Add(menuRpt);
                    customReportComponents.InnerHtml += customReportComponent;
                }
                reportTemplateReader.Close();
            }
        }
    }
}