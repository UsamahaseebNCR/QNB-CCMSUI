<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GraphList.aspx.cs" Inherits="CCMSUI.GraphList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Graph List</title>
    <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="content/iSuite.css" rel="stylesheet" />
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table id="table2" cellspacing="0" cellpadding="0" width="100%" border="0" style="padding-left: 10px">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    Graph List
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" colspan="2">
                </td>
            </tr>
            <tr>
                <td style="height: 20px">
                    <div class="DIVHomePage" id="DivTransactionAnalysis" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='TransactionAnalysis.aspx'">
                                    Transactions Analysis<br />
                                    <img alt="" src="images/icon_reports.jpg" style="cursor: pointer" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    &nbsp;
                    <div class="DIVHomePage" id="DivTransactionAnalysisByAmount" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='TransactionAnalysisByAmount.aspx'">
                                    Transactions Analysis By Amount<br />
                                    <img alt="" src="images/icon_reports.jpg" style="cursor: pointer" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <%--<asp:HyperLink ID="HyperLink1" runat="server" CssClass="Link" NavigateUrl="TransactionAnalysis.aspx">Transactions Analysis</asp:HyperLink>--%>
                    &nbsp;
                    <div class="DIVHomePage" id="DivReplenishmentAnalysis" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='ReplenishmentAnalysis.aspx'">
                                    Replensihments Analysis<br />
                                    <img alt="" src="images/NOTE-SET-TYPE UPDATE.jpg" style="cursor: pointer" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    &nbsp;
                    <div class="DIVHomePage" id="DivCashUtilization" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='CashUtilizationAnalysis.aspx'">
                                    Cash Utlization Analysis<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    &nbsp;
                    <div class="DIVHomePage" id="DivCallOutAnalysis" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='CallOutAnalysis.aspx'">
                                    Call Out Analysis<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    &nbsp;
                    <div class="DIVHomePage" id="DivDenomUtilizationAnalysis" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='DenominationUtilizationAnalysis.aspx'">
                                    Denomination Utilization Analysis<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    &nbsp;
                    <div class="DIVHomePage" id="DivEJTrxnAnalysis" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='TransactionAnalysis.aspx?ej=1'">
                                    EJ Transactions Analysis<br />
                                    <img alt="" src="images/icon_reports.jpg" style="cursor: pointer" />
                                </td>
                            </tr>
                        </table>
                    </div>
                     &nbsp;
                    <div class="DIVHomePage" id="DivGoogleMaps" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                              <td style="width: 158px; cursor: pointer" onclick="window.open('GoogleMaps.aspx','','location=0,status=1,resizable=yes,scrollbars=yes');">
                                    Google Maps<br />
                                    <img alt="" src="images/gmap.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    &nbsp;
                    <div class="DIVHomePage" id="DivVaultUtil" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='VaultUtilizationAnalysis.aspx'">
                                    Vault Utilization Analysis<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>  &nbsp;
                    <div class="DIVHomePage" id="DivPerfDashboard" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='PerformanceDashboard.aspx'">
                                    Performance Dashboard<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    &nbsp;
                    <div class="DIVHomePage" id="DivPNCDepositReportingForBNA" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='PNCDepositReportingForBNA.aspx'">
                                    PNC Deposit Reporting For BNA<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                                        &nbsp;
                    <div class="DIVHomePage" id="DivPNCDepositReportingForCPM" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='PNCDepositReportingForCPM.aspx'">
                                    PNC Deposit Reporting For CPM<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                               &nbsp;
                    <div class="DIVHomePage" id="DivPNCDepositReportingForSDM" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='PNCDepositReportingForSDM.aspx'">
                                    PNC Deposit Reporting For SDM<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                                        &nbsp;
                    <div class="DIVHomePage" id="DivTop5IncidentAnalysis" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='PNCDepositReportingTop5Analysis.aspx'">
                                    PNC Deposit Reporting Analysis For Top 5 Incidents<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                                    &nbsp;
                    <div class="DIVHomePage" id="DivMeanTimeToFailureAnalysis" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/MeanTimeToFailureAnalysis.aspx'">
                                    Mean Time To Failure Analysis<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                                    &nbsp;
                    <div class="DIVHomePage" id="DivAvgResponseTimeAnalysis" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/AvgResponseTimeAnalysis.aspx'">
                                    Average Response Time Analysis<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>

                                        &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeToFailureAnalysisForBNA" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeToFailureAnalysisForBNA.aspx'">
                                    PNC MeanTime To Failure Analysis For BNA<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                

                                    &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeToFailureAnalysisForCPM" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeToFailureAnalysisForCPM.aspx'">
                                    PNC MeanTime To Failure Analysis For CPM<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>

                                   &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeToFailureAnalysisForSDM" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeToFailureAnalysisForSDM.aspx'">
                                    PNC MeanTime To Failure Analysis For SDM<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                     
                                           &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeToIncidentFailureAnalysisForBNA" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeToIncidentFailureAnalysisForBNA.aspx'">
                                    PNC MeanTime To Incident Failure Analysis For BNA<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                

                                    &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeToIncidentFailureAnalysisForCPM" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeToIncidentFailureAnalysisForCPM.aspx'">
                                    PNC MeanTime To Incident Failure Analysis For CPM<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                   &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeToIncidentFailureAnalysisForSDM" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeToIncidentFailureAnalysisForSDM.aspx'">
                                    PNC MeanTime To Incident Failure Analysis For SDM<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeForBNA" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeAnalysisForBNA.aspx'">
                                    PNC MeanTime To Failure Analysis For Incident And Media For BNA<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeForCPM" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeAnalysisForCPM.aspx'">
                                    PNC MeanTime To Failure Analysis For Incident And Media For  CPM<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                                    &nbsp;
                    <div class="DIVHomePage" id="DivPNCMeanTimeForSDM" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PNCMeanTimeAnalysisForSDM.aspx'">
                                    PNC MeanTime To Failure Analysis For Incident And Media For  SDM<br />
                                    <img alt="" src="images/Cash-Utilization-Report.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="DIVHomePage" id="DivPackageAnalytics" runat="server">
                        <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                            <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                                <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='GraphicalAnalysis/PackageDistributionAnalytics.aspx'">
                                    View Package Distribution Analytics<br />
                                    <img alt="" src="images/package_status.jpg" style="cursor: pointer; width:77px; height:61px" />
                                </td>
                            </tr>
                        </table>
                    </div>

                
                </td>

            </tr>
        </table>
    </div>
    </form>
</body>
</html>
