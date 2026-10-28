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

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Avanza.iSuite.UI;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CCMSUI.CCMS
{
    public partial class TempRp1 : BasePage
    {
        ReportDocument rptDoc;

        //private readonly string reportDateFormat = System.Configuration.ConfigurationManager.AppSettings["reportDateFormat"];
        protected void Page_Unload(object sender, EventArgs e)
        {
            if (this.rptDoc != null)
         {
             this.rptDoc.Close();
             this.rptDoc.Dispose();
         }
     
        }
        protected void Page_Init(object sender, EventArgs e)
        {

           StringBuilder builder = new StringBuilder();
          

            string ReportName = (string)Request.Params["Report"];
            string ReportTitle = (string)Request.Params["ReportTitle"];
            string FromDate = HttpUtility.HtmlDecode((string)Request.Params["FromDate"]);
            string ToDate = HttpUtility.HtmlDecode((string)Request.Params["ToDate"]);
            string GeneratedBy = (string)Request.Params["GeneratedBy"];
            int total = int.Parse(Request.Params["total"]);
             builder.Append($"QS,From date: {Request.Params["FromDate"]},To Date:{Request.Params["ToDate"]}\r\n");

            builder.Append($"C,From date: {FromDate},To Date:{ToDate}");
           
            LogableTask.LogMonoActivityTask("", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info,builder.ToString());
            //LogableTask.LogMonoActivityTask("", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "To Date :" + ToDate);


            rptDoc = new ReportDocument();

            ParameterFields pFields = new ParameterFields();
            ParameterField pField = new ParameterField();
            ParameterDiscreteValue disVal = new ParameterDiscreteValue();
            ParameterRangeValue rVal = new ParameterRangeValue();


            rptDoc.Load(Server.MapPath("~/CCMS/" + ReportName));
           

            if (ReportName == "ATMWithoutWithdrawal.rpt"
              )
            {
                pField = new ParameterField();
                disVal = new ParameterDiscreteValue();
                pField.ParameterFieldName = "generatedFor";
                disVal.Value = DateTime.ParseExact(FromDate, "MM/dd/yyyy", null).ToString("dd/MM/yyyy");
                //disVal.Value = DateTime.ParseExact(FromDate, "MM/dd/yyyy", null);
                pField.CurrentValues.Add(disVal);
                               
                pFields.Add(pField);
                //Removed for NBE
                //pField = new ParameterField();
                //disVal = new ParameterDiscreteValue();
                //pField.ParameterFieldName = "total";
                //disVal.Value = total;
                //pField.CurrentValues.Add(disVal);
                //pFields.Add(pField);

                 //CrystalReportViewer1.ParameterFieldInfo = pFields;
            }
            
            else if (ReportName.Contains("BankSettlementsDR.rpt"))
            {
                pField = new ParameterField();
                disVal = new ParameterDiscreteValue();
                pField.ParameterFieldName = "ReportDate";
                disVal.Value = (string)Request.Params["rDate"];
                pField.CurrentValues.Add(disVal);
                pFields.Add(pField);

                pField = new ParameterField();
                disVal = new ParameterDiscreteValue();
                pField.ParameterFieldName = "opening_balance";
                disVal.Value = (string)Request.Params["balance"];
                pField.CurrentValues.Add(disVal);
                pFields.Add(pField);


                pField = new ParameterField();
                disVal = new ParameterDiscreteValue();
                pField.ParameterFieldName = "cash_Recv";
                disVal.Value = (string)Request.Params["cashRecv"];
                pField.CurrentValues.Add(disVal);
                pFields.Add(pField);


                pField = new ParameterField();
                disVal = new ParameterDiscreteValue();
                pField.ParameterFieldName = "cash_Ret";
                disVal.Value = (string)Request.Params["cashRet"];
                pField.CurrentValues.Add(disVal);
                pFields.Add(pField);

                pField = new ParameterField();
                disVal = new ParameterDiscreteValue();
                pField.ParameterFieldName = "title";
                disVal.Value = (string)Request.Params["title"];
                pField.CurrentValues.Add(disVal);
                pFields.Add(pField);

                
                CrystalReportViewer1.ParameterFieldInfo = pFields;
            }
            else
            {
                if ((string)Request.Params["Bypass"] == "0" || (string)Request.Params["Bypass"] == null)
                {


                    pField = new ParameterField();
                    disVal = new ParameterDiscreteValue();
                    pField.ParameterFieldName = "FromDate";
                    //disVal.Value = DateTime.Parse(FromDate);
                    disVal.Value = DateTime.ParseExact(FromDate, "MM/dd/yyyy", null);
                    
                    pField.CurrentValues.Add(disVal);
                    pFields.Add(pField);

                    pField = new ParameterField();
                    disVal = new ParameterDiscreteValue();
                    pField.ParameterFieldName = "ToDate";
                    // disVal.Value = DateTime.Parse(ToDate);

                    disVal.Value = DateTime.ParseExact(ToDate, "MM/dd/yyyy", null);

                    pField.CurrentValues.Add(disVal);
                    pFields.Add(pField);

                    pField = new ParameterField();
                    disVal = new ParameterDiscreteValue();
                    pField.ParameterFieldName = "GeneratedBy";
                    disVal.Value = GeneratedBy;
                    pField.CurrentValues.Add(disVal);
                    pFields.Add(pField);

                    pField = new ParameterField();
                    disVal = new ParameterDiscreteValue();
                    pField.ParameterFieldName = "total";
                    disVal.Value = total;
                    pField.CurrentValues.Add(disVal);
                    pFields.Add(pField);

                    if (ReportName.Contains("CashPositionsRpt.rpt") || ReportName.Contains("CashWithdrawals.rpt") || ReportName.Contains("ReplenishmentReturned.rpt") || ReportName.Contains("SuspiciousReplenishment.rpt"))
                    {
                        if (!ReportName.Contains("NoCashWithdrawals"))
                        {
                            pField = new ParameterField();
                            disVal = new ParameterDiscreteValue();
                            pField.ParameterFieldName = "suppressDetail";
                            disVal.Value = Request.Params["suppressDetail"];
                            pField.CurrentValues.Add(disVal);
                            pFields.Add(pField);

                        }
    
                    }

                    //pField = new ParameterField();
                    //disVal = new ParameterDiscreteValue();
                    //pField.ParameterFieldName = "suppressGroup";
                    //disVal.Value = true;
                    //pField.CurrentValues.Add(disVal);
                    //pFields.Add(pField);


                    if (ReportName == "PNCNotesDepWeeklyAnalysis.rpt")
                    {
                        pField = new ParameterField();
                        disVal = new ParameterDiscreteValue();
                        pField.ParameterFieldName = "accPerc";
                        disVal.Value = decimal.Parse(Request.Params["accPerc"]);
                        pField.CurrentValues.Add(disVal);
                        pFields.Add(pField);

                        pField = new ParameterField();
                        disVal = new ParameterDiscreteValue();
                        pField.ParameterFieldName = "rejPerc";
                        disVal.Value = decimal.Parse(Request.Params["rejPerc"]);
                        pField.CurrentValues.Add(disVal);
                        pFields.Add(pField);

                       
                    }
                    //else if (ReportName == "RepExceptionByTimeReport.rpt")
                    //{
                    //    pField = new ParameterField();
                    //    disVal = new ParameterDiscreteValue();
                    //    pField.ParameterFieldName = "ExecutionTimeLimit";
                    //    disVal.Value = (string)Request.Params["ExecutionTimeLimit"];
                    //    pField.CurrentValues.Add(disVal);
                    //    pFields.Add(pField);
                    //}

                }
            }



            rptDoc.SummaryInfo.ReportTitle = ReportTitle;
            rptDoc.SetDataSource((DataSet)Session[SessionVars.dtReportDataSet.ToString()]);

            if (rptDoc.Subreports.Count > 0 && Session["subReport"] != null)
                rptDoc.Subreports[0].SetDataSource((DataSet)Session["subReport"]);

            if (rptDoc.Subreports.Count == 2)
                rptDoc.Subreports[1].SetDataSource((DataSet)Session["subReport"]);

            CrystalReportViewer1.ReportSource = rptDoc;
            CrystalReportViewer1.DataBind();
            CrystalReportViewer1.ParameterFieldInfo = pFields;

            // rptDoc.Close();
            //rptDoc.Dispose();
            //GC.Collect();



        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}