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
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class ParamBNAEJATMSummary : Page
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
                dtGrid.Columns.Add("notes_accepted");
                dtGrid.Columns.Add("notes_rejected1");
                dtGrid.Columns.Add("notes_rejected2");
                dtGrid.Columns.Add("total_rejected");
                dtGrid.Columns.Add("acc_perc");
                dtGrid.Columns.Add("rej_perc");

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;


                string SQL = @"select atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,ej_parsed_bna_transaction.ej_parsed_bna_transaction_Id
                                    from ej_parsed_bna_transaction inner join atm on ej_parsed_bna_transaction.atm_id = atm.atm_id
                                    inner join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    ej_parsed_bna_transaction.ej_parsed_bna_transaction_id
                                     where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                             " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) and is_eligible=1 ";
               

                //string SQL = "select atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,dispute_status,status from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                //             " inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id  where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                //             " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) ";

                DataTable dt = GetDataTable(SQL);

                DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id", "title" });
                DataTable dtATMsTrxnDatetime = dt.DefaultView.ToTable(true, new string[] { "atm_id", "trxn_datetime" });
                int depositCount = 0;
                int rej1 = 0;
                int rej2 = 0;
                int total = 0;
                foreach (DataRow dr in dtATMs.Rows)
                {
                    DataRow[] drTrxnDates = dtATMsTrxnDatetime.Select("atm_id = " + dr["atm_id"]);

                    foreach (DataRow drInner in drTrxnDates)
                    {
                        rej1 = 0;
                        rej2 = 0;
                        total = 0;
                        depositCount = 0;

                        DataRow[] drAllTrxnForTheDay = dt.Select("atm_id = " + dr["atm_id"] + " and trxn_datetime='" + drInner["trxn_datetime"] + "' and status = 'Successful'");
                        if (drAllTrxnForTheDay.Length > 0)
                        {

                            DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                            foreach (DataRow drRow in dtBNATrxnIds.Rows)
                            {
                                
                                StringBuilder builder = new StringBuilder();
                                DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                                if (dr1.Length > 0)
                                {
                                    DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                    foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                    {
                                        DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                                        builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                        depositCount += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                                    }

                                }
                            }
                        }

                        DataRow newRow = dtGrid.NewRow();
                        newRow["title"] = dr["title"];
                        newRow["trxn_datetime"] = drInner["trxn_datetime"];
                        newRow["notes_accepted"] = depositCount;



                        drAllTrxnForTheDay = dt.Select("atm_id = " + dr["atm_id"] + " and trxn_datetime='" + drInner["trxn_datetime"] + "' and status = 'Failed' and dispute_status is not null");
                        if (drAllTrxnForTheDay.Length > 0)
                        {
                            DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                            foreach (DataRow drRow in dtBNATrxnIds.Rows)
                            {
                                
                                StringBuilder builder = new StringBuilder();
                                DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                                if (dr1.Length > 0)
                                {
                                    DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                    foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                    {
                                        DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                                        builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                        rej1 += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                                    }

                                }
                            }
                        }



                        newRow["notes_rejected1"] = rej1;

                        drAllTrxnForTheDay = dt.Select("atm_id = " + dr["atm_id"] + " and trxn_datetime='" + drInner["trxn_datetime"] + "' and status = 'Failed' and dispute_status is null");
                        if (drAllTrxnForTheDay.Length > 0)
                        {
                            DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                            foreach (DataRow drRow in dtBNATrxnIds.Rows)
                            {
                                
                                StringBuilder builder = new StringBuilder();
                                DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                                if (dr1.Length > 0)
                                {
                                    DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                    foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                    {
                                        DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                                        builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                        rej2 += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                                    }

                                }
                            }


                        }
                        newRow["notes_rejected2"] = rej2;
                        newRow["total_rejected"] = rej1+rej2;
                        total = rej1 + rej2 + depositCount;
                        if (total != 0)
                        {
                            newRow["acc_perc"] = Math.Round(((decimal)depositCount / total) * 100, 2);
                            newRow["rej_perc"] = Math.Round((((decimal)rej1 + rej2) / total) * 100, 2);
                        }
                        else
                        {
                            newRow["acc_perc"] = 0;
                            newRow["rej_perc"] = 0;
                        }
                        dtGrid.Rows.Add(newRow);

                    }
                    

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