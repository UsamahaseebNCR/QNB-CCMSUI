using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS
{
    public partial class ParamCPMEJATMSummary : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fromDate = Request.Params["from"];
            string toDate = Request.Params["to"];

            //http://localhost:82/CCMSInt/GraphicalAnalysis/Reports/ParamEJBNATransactions.aspx?from=05/05/2000&to=12/05/2014
            BindGrid(fromDate, toDate);

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

        void BindGrid(string fromDate, string toDate)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("trxn_datetime");
                dtGrid.Columns.Add("cheques_accepted");
                dtGrid.Columns.Add("cheques_rejected1");
                dtGrid.Columns.Add("cheques_rejected2");
                dtGrid.Columns.Add("total_rejected");
                dtGrid.Columns.Add("acc_perc");
                dtGrid.Columns.Add("rej_perc");

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                string SQL = "select atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,dispute_status,status from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                             " inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id  where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                             " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) and is_eligible=1 ";

                DataTable dt = GetDataTable(SQL);

                DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id", "title" });
                DataTable dtATMsTrxnDatetime = dt.DefaultView.ToTable(true, new string[] { "atm_id", "trxn_datetime" });

                foreach (DataRow dr in dtATMs.Rows)
                {
                    DataRow[] drTrxnDates = dtATMsTrxnDatetime.Select("atm_id = " + dr["atm_id"]);

                    foreach (DataRow drInner in drTrxnDates)
                    {
                        DataRow[] drBusinessRuleReject = dt.Select("trxn_datetime='" + drInner["trxn_datetime"] + "' and  atm_id = " + dr["atm_id"] + " and dispute_status <>'000' and dispute_status is not null and status = 'Failed'");
                        DataRow[] drNonBusinessRuleReject = dt.Select("trxn_datetime='" + drInner["trxn_datetime"] + "' and atm_id = " + dr["atm_id"] + " and dispute_status is null and status = 'Failed'");
                        DataRow[] drAccepted = dt.Select("trxn_datetime='" + drInner["trxn_datetime"] + "' and atm_id = " + dr["atm_id"] + " and dispute_status ='000' and status = 'Successful'");

                        int totalChecks = drBusinessRuleReject.Length + drNonBusinessRuleReject.Length + drAccepted.Length;
                        DataRow newRow = dtGrid.NewRow();
                        newRow["title"] = dr["title"];
                        newRow["trxn_datetime"] = drInner["trxn_datetime"];
                        newRow["cheques_accepted"] = drAccepted.Length;
                        newRow["cheques_rejected1"] = drBusinessRuleReject.Length;
                        newRow["cheques_rejected2"] = drNonBusinessRuleReject.Length;
                        newRow["total_rejected"] = drBusinessRuleReject.Length + drNonBusinessRuleReject.Length;
                        if (totalChecks != 0)
                        {
                            newRow["acc_perc"] = Math.Round(((decimal)drAccepted.Length / totalChecks) * 100, 2);
                            newRow["rej_perc"] = Math.Round((((decimal)drBusinessRuleReject.Length + drNonBusinessRuleReject.Length) / totalChecks) * 100, 2);
                        }
                        else
                        {
                            newRow["acc_perc"] = 0;
                            newRow["rej_perc"] = 0;
                        }

                            dtGrid.Rows.Add(newRow);

                    }
                    //if (totalChecks > 0)
                    //    Label_AccPerc.Text = Math.Round((decimal)drAccepted.Length / totalChecks * 100).ToString();
                    //else
                    //    Label_AccPerc.Text = "0";


                }



                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //DataTable dtTrxn = new DataTable();
                //adapter.Fill(dtTrxn);
                ////Literal_count.Text = dtTrxn.Rows.Count + " records.";
                this.GridView_Trxn.DataSource = dtGrid;
                this.GridView_Trxn.DataBind();
            }
            catch (Exception ex)
            {
                
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }




    }
}