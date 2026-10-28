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
    public partial class ParamPNCMeanTimeToFailureAnalysisForBNA : Page
    {
        int id = 0;
        ReportSchedule reportSchedule = null;
        private void DrawGraph(string from, string to, int businessRule, int nonBusinessRule,string filter)
        {
            List<int> parsedBNATransactions = new List<int>();
            int count = 0;
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

            string SQL = "select * from ej_parsed_bna_transaction left join ej_parsed_bna_transaction_detail on ej_parsed_bna_transaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
               " inner join atm on atm.atm_id = ej_parsed_bna_transaction.atm_id " +
               " where atm.is_Active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
               " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + filter + "  and is_eligible=1  order by trxn_datetime ";

            DataTable dt = GetDataTable(SQL);
            DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
            for (int i = 0; i < dtTrxns.Rows.Count; i++)
            {
                count++;
                //DataRow[] previousDayLowBalancedRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                DataRow[] drArray = dt.Select("trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] + "#");
                foreach (DataRow dr in drArray)
                {
                    id = int.Parse(dr["ej_parsed_bna_transaction_id"].ToString());
                    if (parsedBNATransactions.Contains(id))
                        continue;
                    else
                        parsedBNATransactions.Add(id);

                    if (dr["status"].ToString() == "Successful")
                    {
                        DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id = " + dr["ej_parsed_bna_transaction_id"]);
                        if (temp.Length > 0)
                        {
                            DataTable dtUniqueNoteType = temp.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                            foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                            {
                                DataRow[] innerTemp = dt.Select("ej_parsed_bna_transaction_id=" + dr["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"]);
                                //builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                successfulChqDeposit += int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString());
                            }

                        }

                        //if (temp != null)
                        //{
                        //    foreach (DataRow drTemp in temp)
                        //        successfulChqDeposit += int.Parse(drTemp["notes_count"].ToString()); // Successful cheque deposit count;
                        //}
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
                                Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = successfulChqDeposit + " notes deposited before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                            }
                            else
                                Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), 0);

                            successfulChqDeposit = 0;
                        }
                        else
                        {   //Non Business Rules
                            //DataRow newRow = dtNonBusinessRules.NewRow();
                            //newRow[0] = DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString());
                            //newRow[1] = successfulChqDeposit;
                            //dtNonBusinessRules.Rows.Add(newRow);

                            if (nonBusinessRule==1)
                            {
                                Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit);
                                Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[1].Points.Count - 1].ToolTip = successfulChqDeposit + " notes deposited before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                            }
                            else
                                Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), 0);


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
            //}

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
            string filter = null;
            string fromDate = Request.Params["from"];
            string toDate = Request.Params["to"];
            id = int.Parse(Request.Params["id"]);
            //string type = Request.Params["type"];
            //http://localhost:82/CCMSInt/GraphicalAnalysis/Reports/ParamPNCMeanTimeToFailureAnalysisForBNA.aspx?from=05/05/2000&to=12/05/2014&type=3

            reportSchedule = ReportSchedule.LoadReportScheduleByPk(id);
            if (reportSchedule.ReportName.ToLower().Contains("sdm"))
                filter += " and is_sdm = 1 ";
            else
                filter += " and is_cdm = 1 ";
        
            if (reportSchedule.CriteriaId.HasValue)
            {
                if (reportSchedule.CriteriaId.Value == 1)
                {
                    filter += " and dispute_status is not null ";
                    DrawGraph(fromDate, toDate, 1, 0, filter);
                }
                else if (reportSchedule.CriteriaId.Value == 2)
                {
                    filter += " and (dispute_status is null or dispute_status = '000') ";
                    DrawGraph(fromDate, toDate, 0, 1, filter);
                }
            }
            else
                DrawGraph(fromDate, toDate, 1, 1, filter);
            int reportTaskId = int.Parse(Request.Params["tid"]);
            ReportTask reportTask = ReportTask.LoadReportTaskByPk(reportTaskId);
            Literal_Title.Text = reportSchedule.ReportFriendlyName;
            Label_currentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Label_scheduleDate.Text = reportTask.ScheduleDate.ToString("dd/MM/yyyy"); 
         
        }
    }


}