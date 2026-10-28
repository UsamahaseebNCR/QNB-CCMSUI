using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;

namespace CCMSUI
{
    public partial class GraphList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            DivPNCMeanTimeForBNA.Visible = CheckRight((int)Permissions.ViewPNCMeanTimePerIncidentAndMediaForBNA);
            DivPNCMeanTimeForCPM.Visible = CheckRight((int)Permissions.ViewPNCMeanTimePerIncidentAndMediaForCPM);
            DivPNCMeanTimeForSDM.Visible = CheckRight((int)Permissions.ViewPNCMeanTimePerIncidentAndMediaForSDM);
            



            DivPNCDepositReportingForSDM.Visible = CheckRight((int)Permissions.ViewPNCMeanTimeForSDM);
            DivPNCMeanTimeToIncidentFailureAnalysisForBNA.Visible = CheckRight((int)Permissions.ViewPNCMeanTimeIncidentForBNA);
            DivPNCMeanTimeToIncidentFailureAnalysisForCPM.Visible = CheckRight((int)Permissions.ViewPNCMeanTimeIncidentForCPM);
            DivPNCMeanTimeToIncidentFailureAnalysisForSDM.Visible = CheckRight((int)Permissions.ViewPNCMeanTimeIncidentForSDM);


            DivPNCMeanTimeToFailureAnalysisForSDM.Visible = CheckRight((int)Permissions.ViewPNCMeanTimeForSDM);
            DivPNCMeanTimeToFailureAnalysisForBNA.Visible = CheckRight((int)Permissions.ViewPNCMeanTimeForBNA);
            DivPNCMeanTimeToFailureAnalysisForCPM.Visible = CheckRight((int)Permissions.ViewPNCMeanTimeForCPM);
            
            DivAvgResponseTimeAnalysis.Visible = CheckRight((int)Permissions.ViewAvgResponseTimeAnalysis);
            DivMeanTimeToFailureAnalysis.Visible = CheckRight((int)Permissions.ViewMeanTimeToFailureAnalysis);
            DivPNCDepositReportingForBNA.Visible = CheckRight((int)Permissions.ViewPNCAnalysisForBNA);
            DivPNCDepositReportingForCPM.Visible = CheckRight((int)Permissions.ViewPNCAnalysisForCPM);
            DivTop5IncidentAnalysis.Visible = CheckRight((int)Permissions.ViewPNCAnalysisForTop5Incidents);

            DivPerfDashboard.Visible = CheckRight((int)Permissions.ViewPerformanceDashboard);
            DivVaultUtil.Visible = CheckRight((int)Permissions.ViewVaultUtilizationAnalysisGraph);
            DivTransactionAnalysis.Visible = CheckRight((int)Permissions.ViewTransactionAnalysisGraph);
            DivTransactionAnalysisByAmount.Visible = CheckRight((int)Permissions.ViewTransactionAnalysisByAmountGraph);
            DivReplenishmentAnalysis.Visible = CheckRight((int)Permissions.ViewReplenishmentAnalysisGraph);
            DivCashUtilization.Visible = CheckRight((int)Permissions.ViewCashUtilizationAnalysisGraph);
            DivCallOutAnalysis.Visible = CheckRight((int)Permissions.ViewCallOutAnalysisGraph);
            DivDenomUtilizationAnalysis.Visible = CheckRight((int)Permissions.ViewDenominationUtilizationGraph);
            DivEJTrxnAnalysis.Visible = CheckRight((int)Permissions.ViewEJTransactionAnalysis);
            DivGoogleMaps.Visible = CheckRight((int)Permissions.ViewGoogleMaps);
            DivPackageAnalytics.Visible = CheckRight((int)Permissions.ViewPackageAnalytics);
            Session["isRunningForEj"] = null;
        }
    }
}