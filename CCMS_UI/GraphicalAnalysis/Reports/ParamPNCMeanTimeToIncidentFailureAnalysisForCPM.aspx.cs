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

using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Text;

namespace CCMSUI
{
    public partial class ParamPNCMeanTimeToIncidentFailureAnalysisForCPM : Page
    {
        
        private void DrawGraph(string from, string to,int businessRule, int nonBusinessRule,string filter)
        {
            List<int> parsedCPMTransactions = new List<int>();
            int count= 0;
            int successfulChqDeposit = 0;
            int id = 0;
            //bool series1DataPointAdded = false;
            //bool series2DataPointAdded = false;
            Chart_MeanTimeToFailure1.Series[0].Points.Clear();
            Chart_MeanTimeToFailure1.Series[1].Points.Clear();

            //DataTable dtNonBusinessRules = new DataTable();
            //dtNonBusinessRules.Columns.Add("trxn_datetime", typeof(DateTime));
            //dtNonBusinessRules.Columns.Add("successfulTrxnCount");
            
            //DataTable dtBusinessRuleError = dtNonBusinessRules.Clone();



            string SQL = "select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
           " inner join atm on atm.atm_id = ej_parsed_cpm_transaction.atm_id where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
           " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) and is_eligible=1 " + filter+ " order by trxn_datetime ";
       

            //string SQL = "select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
            //    " inner join atm on atm.atm_id = ej_parsed_cpm_transaction.atm_id where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
            //    " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103)  order by trxn_datetime ";
            DataTable dt = GetDataTable(SQL);
            DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
            for (int i = 0; i < dtTrxns.Rows.Count; i++)
            {
                count++;
                //DataRow[] previousDayLowBalancedRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                DataRow[] drArray = dt.Select("trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] + "#");
                foreach (DataRow dr in drArray)
                {
                    id = int.Parse(dr["ej_parsed_cpm_transaction_id"].ToString());
                    if (parsedCPMTransactions.Contains(id))
                        continue;
                    else
                        parsedCPMTransactions.Add(id);

                    if (dr["status"].ToString() == "Successful")
                    {
                        //DataRow[] temp = dt.Select("ej_parsed_cpm_transaction_id = " + dr["ej_parsed_cpm_transaction_id"]);
                        //if (temp != null)
                            //successfulChqDeposit += temp.Length; // Successful cheque deposit count;
                        successfulChqDeposit++;
                    }
                    else if (dr["status"].ToString() == "Failed" && successfulChqDeposit > 0)
                    {
                        if (dr["dispute_status"] != DBNull.Value) // Business Rules
                        {
                            //DataRow newRow = dtBusinessRuleError.NewRow();
                            //newRow[0] = DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString());
                            //newRow[1] = successfulChqDeposit;
                            //dtBusinessRuleError.Rows.Add(newRow);
                            if (businessRule==1)
                            {
                                Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit);
                                Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = successfulChqDeposit + " transactions occurred before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                            }
                            else
                                Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), 0);

                            successfulChqDeposit = 0;
                        }
                        else
                        {
                            if (nonBusinessRule==1)
                            {
                                Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit);
                                Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[1].Points.Count - 1].ToolTip = successfulChqDeposit + " transactions occurred before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                            }
                            else
                                Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), 0);
                            //Non Business Rules
                            //DataRow newRow = dtNonBusinessRules.NewRow();
                            //newRow[0] = DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString());
                            //newRow[1] = successfulChqDeposit;
                            //dtNonBusinessRules.Rows.Add(newRow);
                            //
                            successfulChqDeposit = 0;
                        }
                    }
                }
            }

            //int max = dtBusinessRuleError.Rows.Count > dtNonBusinessRules.Rows.Count ? dtBusinessRuleError.Rows.Count : dtNonBusinessRules.Rows.Count;
            //int sample = int.Parse(TextBox_Instance.Text);
            //for (int i = 0; i < max; i++)
            //{
            //    if (i < dtBusinessRuleError.Rows.Count)
            //    {
            //        if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
            //        {
            //            Chart_MeanTimeToFailure1.Series[0].Points.AddXY("Instance " + (i + 1), dtBusinessRuleError.Rows[i]["successfulTrxnCount"]);
            //            Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = "Successful Transactions[" + dtBusinessRuleError.Rows[i]["successfulTrxnCount"] + "]"; 
            //        }
            //        else
            //            Chart_MeanTimeToFailure1.Series[0].Points.AddXY("Instance " + (i + 1), 0);
            //    }
            //    if (i < dtNonBusinessRules.Rows.Count)
            //    {
            //        if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            //        {
            //            Chart_MeanTimeToFailure1.Series[1].Points.AddXY("Instance " + (i + 1), dtNonBusinessRules.Rows[i]["successfulTrxnCount"]);
            //            Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[1].Points.Count - 1].ToolTip = "Successful Transactions[" + dtNonBusinessRules.Rows[i]["successfulTrxnCount"] + "]"; 
            //        }
            //        else
            //            Chart_MeanTimeToFailure1.Series[1].Points.AddXY("Instance " + (i + 1), 0);
            //    }
            //    if (i == sample)
            //        break;
            //             }

            Chart_MeanTimeToFailure1.ResetAutoValues();
        }
 
        private DataTable GetDataTable(string SQL)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = SQL;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            string filter = "";
            string fromDate = Request.Params["from"];
            string toDate = Request.Params["to"];
            int id = int.Parse(Request.Params["id"]);
            ReportSchedule reportSchedule = ReportSchedule.LoadReportScheduleByPk(id);
            //http://localhost:82/CCMSInt/GraphicalAnalysis/Reports/ParamPNCMeanTimeToFailureAnalysisForCPM.aspx?from=05/05/2000&to=12/05/2014&type=0


            if (reportSchedule.CriteriaId.HasValue)
            {
                if (reportSchedule.CriteriaId.Value == 1)
                {
                    filter += " and dispute_status is not null ";
                    DrawGraph(fromDate, toDate, 1, 0,filter);
                }
                else if (reportSchedule.CriteriaId.Value == 2)
                {
                    filter += " and (dispute_status is null or dispute_status = '000') ";
                    DrawGraph(fromDate, toDate, 0, 1,filter);
                }
             }
            else
                DrawGraph(fromDate, toDate, 1, 1,filter);
            int reportTaskId = int.Parse(Request.Params["tid"]);
            ReportTask reportTask = ReportTask.LoadReportTaskByPk(reportTaskId);
            Literal_Title.Text = reportSchedule.ReportFriendlyName;
            Label_currentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Label_scheduleDate.Text = reportTask.ScheduleDate.ToString("dd/MM/yyyy"); 
                   
            //if (type=="1")
            //    DrawGraph(fromDate, toDate, 1,0);
            //else if (type == "2")
            //    DrawGraph(fromDate, toDate, 0, 1);
            //else if (type == "3" || type == null)
            //    DrawGraph(fromDate, toDate, 1, 1);

        }
    }


}