using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data;
using System.Data.SqlClient;

using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class PNCDepositSummary : BasePage
    {
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
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.PNCDepositSummary))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                
            }
        }
      
       
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            DataSet ds = null;
            try
            {
               
                cmd = ConnectionFactory.GetNewCommand(false);
                if (RadioButtonList_Module.SelectedIndex == 0 )
                {
                    DataTable dtGrid = new DataTable();
                    dtGrid.Columns.Add("title");
                    dtGrid.Columns.Add("trxn_datetime",typeof(DateTime));
                    dtGrid.Columns.Add("cheques_accepted",typeof(int));
                    dtGrid.Columns.Add("cheques_rejected1", typeof(int));
                    dtGrid.Columns.Add("cheques_rejected2", typeof(int));
                    dtGrid.Columns.Add("total_rejected", typeof(int));
                    dtGrid.Columns.Add("acc_perc", typeof(decimal));
                    dtGrid.Columns.Add("rej_perc", typeof(decimal));

//                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;

                    string SQL = "select atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,dispute_status,status from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                                 " inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id  where atm.is_active=1 " + GetFilter();
                    
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
                    ds = null;// new DsPNCChequeDepSummary();
                    ds.Tables[0].Merge(dtGrid);
                }
                
               
                
                
                else
                {
                    DataTable dtGrid = new DataTable();
                    dtGrid.Columns.Add("title");
                    dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
                    dtGrid.Columns.Add("notes_accepted", typeof(int));
                    dtGrid.Columns.Add("notes_rejected1", typeof(int));
                    dtGrid.Columns.Add("notes_rejected2", typeof(int));
                    dtGrid.Columns.Add("total_rejected", typeof(int));
                    dtGrid.Columns.Add("acc_perc", typeof(decimal));
                    dtGrid.Columns.Add("rej_perc", typeof(decimal));

                    //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;

                    // Data Archiving Work
                    string SQL = @"select atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,vEjParsedBNATransaction.ej_parsed_bna_transaction_Id
                                    from vEjParsedBNATransaction inner join atm on vEjParsedBNATransaction.atm_id = atm.atm_id
                                    inner join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    vEjParsedBNATransaction.ej_parsed_bna_transaction_id
                                     where atm.is_active=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" + GetFilter();


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
                            newRow["total_rejected"] = rej1 + rej2;
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
                    ds = new DsPNCNoteDepSummary();
                    ds.Tables[0].Merge(dtGrid);
                }
                
                //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "query ->"+cmd.CommandText);
                
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.Fill(ds.Tables["DataTable1"]);
                ////SplitAlertMessageIntoColumns(ds.Tables["DataTable1"]);

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    string URL = null;
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    if (RadioButtonList_Module.SelectedIndex == 0 )
                        URL = "ReportPopup.aspx?Report=PNCChequeDepSummary.rpt&ReportTitle=Cheques Deposit Summary&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
                    else if (RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
                        URL = "ReportPopup.aspx?Report=PNCNotesDepSummary.rpt&ReportTitle=Notes Deposit Summary&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
                    Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask();
            }
        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        protected void CheckBox_Current_CheckedChanged(object sender, EventArgs e)
        {

            TextBox_FromDate.Enabled = (!TextBox_FromDate.Enabled);
            TextBox_ToDate.Enabled = (!TextBox_ToDate.Enabled);

        }
        string GetFilter()
        {
            string filter = "";

            filter = " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";

           
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            if (TextBox_FromAmount.Text.Length > 0)
            {
                if (RadioButtonList_Module.SelectedIndex == 0 )
                    filter += " and deposit_amount >= " + int.Parse(TextBox_FromAmount.Text);
                else if ( RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
                    filter += " and amount_authorized >= " + int.Parse(TextBox_FromAmount.Text);                    
            }
            if (TextBox_ToAmount.Text.Length > 0)
            {
                if (RadioButtonList_Module.SelectedIndex == 0 )
                    filter += " and deposit_amount <= " + int.Parse(TextBox_FromAmount.Text);
                else if ( RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
                    filter += " and amount_authorized <= " + int.Parse(TextBox_FromAmount.Text);                    

                //filter += " and amount_authorized <= " + int.Parse(TextBox_ToAmount.Text);
            }

            if (TextBox_TSN.Text.Length > 0)
                filter += " and seq = '" + TextBox_TSN.Text + "'";
            //filter += " and tsn = " + int.Parse(TextBox_TSN.Text);

            if (TextBox_PAN.Text.Length > 0)
                filter += " and pan like '%" + TextBox_PAN.Text + "%'";

            if (CheckBoxList_DisputeStatus.Items[0].Selected && CheckBoxList_DisputeStatus.Items[1].Selected)
            {
            }
            else if (CheckBoxList_DisputeStatus.Items[0].Selected)
            {
                filter += " and dispute_status is not null ";
            }
            else if (CheckBoxList_DisputeStatus.Items[1].Selected)
            {
                //filter += " and dispute_status is null ";
                filter += " and (dispute_status is null or dispute_status = '000') ";
            }

            if (CheckBoxList_Status.Items[0].Selected && CheckBoxList_Status.Items[1].Selected)
            {
            }
            else if (CheckBoxList_Status.Items[0].Selected)
                filter += " and status = 'Successful'";
            else if (CheckBoxList_Status.Items[1].Selected)
                filter += " and status = 'Failed'";

            if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1)
                filter+=" and is_sdm = 1 ";
            else if (RadioButtonList_Module.SelectedIndex==2)
                filter += " and is_cdm = 1 ";

            //if (DropDownList_Comment.SelectedIndex>0)
            //    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";



            filter += " and is_eligible=1  ";
            return filter;
        }


      
    }
}