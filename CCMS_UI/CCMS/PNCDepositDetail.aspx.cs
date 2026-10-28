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
    public partial class PNCDepositDetail : BasePage
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

        string GetFilter()
        {
            string filter = "";

            filter = " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";


            //if (TextBox_FromDate.Text.Length > 0)
            //    filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            //if (TextBox_ToDate.Text.Length > 0)
            //    filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            //if (TextBox_FromAmount.Text.Length > 0)
            //{
            //    if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
            //        filter += " and deposit_amount >= " + int.Parse(TextBox_FromAmount.Text);
            //    else
            //        filter += " and amount_authorized >= " + int.Parse(TextBox_FromAmount.Text);
            //}
            //if (TextBox_ToAmount.Text.Length > 0)
            //{
            //    if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
            //        filter += " and deposit_amount <= " + int.Parse(TextBox_FromAmount.Text);
            //    else
            //        filter += " and amount_authorized <= " + int.Parse(TextBox_FromAmount.Text);

            //    //filter += " and amount_authorized <= " + int.Parse(TextBox_ToAmount.Text);
            //}

            //if (TextBox_TSN.Text.Length > 0)
            //    filter += " and seq = '" + TextBox_TSN.Text + "'";
            ////filter += " and tsn = " + int.Parse(TextBox_TSN.Text);

            //if (TextBox_PAN.Text.Length > 0)
            //    filter += " and pan like '%" + TextBox_PAN.Text + "%'";

            //if (CheckBoxList_DisputeStatus.Items[0].Selected && CheckBoxList_DisputeStatus.Items[1].Selected)
            //{
            //}
            //else if (CheckBoxList_DisputeStatus.Items[0].Selected)
            //{
            //    filter += " and dispute_status is not null ";
            //}
            //else if (CheckBoxList_DisputeStatus.Items[1].Selected)
            //{
            //    filter += " and dispute_status is null ";
            //}

            //if (CheckBoxList_Status.Items[0].Selected && CheckBoxList_Status.Items[1].Selected)
            //{
            //}
            //else if (CheckBoxList_Status.Items[0].Selected)
            //    filter += " and status = 'Successful'";
            //else if (CheckBoxList_Status.Items[1].Selected)
            //    filter += " and status = 'Failed'";

            //if (RadioButtonList_Module.SelectedIndex == 0)
            //    filter += " and (is_ccdm = 1 or is_sdm = 1) ";
            //else if (RadioButtonList_Module.SelectedIndex == 1)
            //    filter += " and is_ccdm = 1 ";
            //else if (RadioButtonList_Module.SelectedIndex == 2)
            //    filter += " and is_sdm = 1 ";
            //else if (RadioButtonList_Module.SelectedIndex == 3)
            //    filter += " and is_cdm = 1 ";

            ////if (DropDownList_Comment.SelectedIndex>0)
            ////    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";

            //filter += " and is_eligible=1  ";
            //return filter;
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            if (TextBox_FromAmount.Text.Length > 0)
            {
                if (RadioButtonList_Module.SelectedIndex == 0)
                    filter += " and deposit_amount >= " + int.Parse(TextBox_FromAmount.Text);
                else if (RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
                    filter += " and amount_authorized >= " + int.Parse(TextBox_FromAmount.Text);
            }
            if (TextBox_ToAmount.Text.Length > 0)
            {
                if (RadioButtonList_Module.SelectedIndex == 0)
                    filter += " and deposit_amount <= " + int.Parse(TextBox_FromAmount.Text);
                else if (RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
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
                filter += " and is_sdm = 1 ";
            else if (RadioButtonList_Module.SelectedIndex == 2)
                filter += " and is_cdm = 1 ";
            else if (RadioButtonList_Module.SelectedIndex == 3)
                filter += " and is_ccdm = 1 ";
            //if (DropDownList_Comment.SelectedIndex>0)
            //    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";



            filter += " and is_eligible=1  ";
            return filter;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.PNCDepositDetail))
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
                if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 3)
                {
                    int depositCount = 0;
                    DataTable dtGrid = new DataTable();
                    dtGrid.Columns.Add("title");
                    dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
                    dtGrid.Columns.Add("tsn");
                    dtGrid.Columns.Add("account_type");
                    dtGrid.Columns.Add("pan");
                    dtGrid.Columns.Add("deposit_amount");
                    dtGrid.Columns.Add("result");
                    dtGrid.Columns.Add("dispute_status");
                    dtGrid.Columns.Add("status");
                    dtGrid.Columns.Add("comment");
                    dtGrid.Columns.Add("amount_authorized", typeof(decimal));
                    dtGrid.Columns.Add("detail");
                    dtGrid.Columns.Add("deposit_count",typeof(int));



                    
                    cmd.CommandTimeout = 500;
                    // Data Archiving Work
                    cmd.CommandText = @"select title,trxn_datetime,seq,account_type,pan,deposit_amount,result,dispute_status,status,comment,check_amount,vEjParsedCPMtransaction.ej_parsed_cpm_transaction_id 
                                    from vEjParsedCPMtransaction inner join atm 
                                    on vEjParsedCPMtransaction.atm_id = atm.atm_id
                                    left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id = 
                                    vEjParsedCPMtransaction.ej_parsed_cpm_transaction_id 
                                    where atm.is_active=1 " + GetFilter();
                                     

                    //                cmd.CommandText = @"select title,trxn_datetime,seq,account_type,pan,deposit_amount,result,dispute_status,status,comment
                    //                                    from ej_parsed_cpm_transaction inner join atm 
                    //                                    on ej_parsed_cpm_transaction.atm_id = atm.atm_id where atm.is_active=1 
                    //                                    and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
                    //                + " order by " + ViewState["orderby"];

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dtTrxn = new DataTable();
                    adapter.Fill(dtTrxn);

                    //Bind Comment Combo
                    //DropDownList_Comment.Items.Clear();
                    //DropDownList_Comment.Items.Add("Any");
                    //DataTable comments = dtTrxn.DefaultView.ToTable(true, new string[] { "comment" });
                    ////DataRow[] dataRow = dtTrxn.Select("comment<>''");
                    //foreach (DataRow dr in comments.Rows)
                    //{
                    //    if (dr["comment"].ToString().Length>0)
                    //    DropDownList_Comment.Items.Add(dr["comment"].ToString());
                    //}
                    //
                    DataTable dtCPMTrxnIds = dtTrxn.DefaultView.ToTable(true, new string[] { "ej_parsed_cpm_transaction_id" });
                    foreach (DataRow drRow in dtCPMTrxnIds.Rows)
                    {
                        depositCount = 0;
                        StringBuilder builder = new StringBuilder();
                        DataRow[] dr = dtTrxn.Select("ej_parsed_cpm_transaction_id=" + drRow["ej_parsed_cpm_transaction_id"]);
                        if (dr.Length > 0)
                        {
                            //    DataTable dtUniqueNoteType = dr.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                            foreach (DataRow drDepositedCheque in dr)
                            {
                                //DataRow[] temp = dtTrxn.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"]);
                                builder.Append(drDepositedCheque["check_amount"].ToString() + "    ");
                                depositCount++;
                            }

                        }
                        DataRow drNewRow = dtGrid.NewRow();
                        drNewRow["title"] = dr[0]["title"];
                        drNewRow["trxn_datetime"] = dr[0]["trxn_datetime"];
                        drNewRow["tsn"] = dr[0]["seq"];
                        drNewRow["account_type"] = dr[0]["account_type"];
                        drNewRow["pan"] = dr[0]["pan"];
                        drNewRow["deposit_amount"] = dr[0]["deposit_amount"];
                        drNewRow["result"] = dr[0]["result"];
                        drNewRow["dispute_status"] = dr[0]["dispute_status"];
                        drNewRow["status"] = dr[0]["status"];
                        drNewRow["comment"] = dr[0]["comment"];
                        drNewRow["detail"] = builder.ToString();
                        drNewRow["amount_authorized"] = dr[0]["deposit_amount"];
                        drNewRow["deposit_count"] = depositCount;

                        dtGrid.Rows.Add(drNewRow);

                    }


                    ds = null;// new DsPNCChequeDepSummary();
                    ds.Tables[0].Merge(dtGrid);
                }
                else
                {


                    int depositCount = 0;
                    DataTable dtGrid = new DataTable();
                    dtGrid.Columns.Add("title");
                    dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
                    dtGrid.Columns.Add("tsn");
                    dtGrid.Columns.Add("account_type");
                    dtGrid.Columns.Add("pan");
                    dtGrid.Columns.Add("amount_authorized", typeof(decimal));
                    dtGrid.Columns.Add("dispute_status");
                    dtGrid.Columns.Add("status");
                    dtGrid.Columns.Add("comment");
                    dtGrid.Columns.Add("detail");
                    dtGrid.Columns.Add("deposit_count", typeof(int));




                    cmd.CommandTimeout = 500;

                    //                cmd.CommandText = @"select title,trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment
                    //                                    from ej_parsed_bna_transaction inner join atm 
                    //                                    on ej_parsed_bna_transaction.atm_id = atm.atm_id where atm.is_active=1 
                    //                                    and  atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
                    //                                     + " order by " + ViewState["orderby"];

                    // Data Archiving Work
                    cmd.CommandText = @"select title,trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,vEjParsedBNATransaction.ej_parsed_bna_transaction_Id
                                    from vEjParsedBNATransaction inner join atm on vEjParsedBNATransaction.atm_id = atm.atm_id
                                    left join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    vEjParsedBNATransaction.ej_parsed_bna_transaction_id
                                     where atm.is_active=1 " + GetFilter();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dtTrxn = new DataTable();
                    adapter.Fill(dtTrxn);

                    //Bind Comment Combo
                    //DropDownList_Comment.Items.Clear();
                    //DropDownList_Comment.Items.Add("Any");
                    //DataTable comments = dtTrxn.DefaultView.ToTable(true, new string[] { "comment" });
                    ////DataRow[] dataRow = dtTrxn.Select("comment<>''");
                    //foreach (DataRow dr in comments.Rows)
                    //{
                    //    if (dr["comment"].ToString().Length > 0)
                    //        DropDownList_Comment.Items.Add(dr["comment"].ToString());
                    //}
                    //


                    DataTable dtBNATrxnIds = dtTrxn.DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });
                    foreach (DataRow drRow in dtBNATrxnIds.Rows)
                    {
                     
                        depositCount = 0;
                        StringBuilder builder = new StringBuilder();
                        
                        DataRow[] dr = dtTrxn.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                        if (dr.Length > 0)
                        {
                            LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "p2");
                            DataTable dtUniqueNoteType = dr.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                            foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                            {
                                if (drUniqueNoteType["note_type"].ToString() == "")
                                    continue;
                                LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "p1");
                                DataRow[] temp = dtTrxn.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"]);
                                builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + " ");
                                depositCount += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                            }

                        }
                        DataRow drNewRow = dtGrid.NewRow();
                        drNewRow["title"] = dr[0]["title"];
                        drNewRow["trxn_datetime"] = dr[0]["trxn_datetime"];
                        drNewRow["tsn"] = dr[0]["seq"];
                        drNewRow["account_type"] = dr[0]["account_type"];
                        drNewRow["pan"] = dr[0]["pan"];
                        drNewRow["amount_authorized"] = dr[0]["amount_authorized"];
                        drNewRow["dispute_status"] = dr[0]["dispute_status"];
                        drNewRow["status"] = dr[0]["status"];
                        drNewRow["comment"] = dr[0]["comment"];
                        drNewRow["deposit_count"] = depositCount;
                        drNewRow["detail"] = builder.ToString();

                        dtGrid.Rows.Add(drNewRow);

                    }







                    ds = new dsPNCNoteDepDetail();
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
                    if (RadioButtonList_Module.SelectedIndex == 0)
                        URL = "ReportPopup.aspx?Report=PNCCheckDepDetail.rpt&ReportTitle=Check Deposit Detail&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
                    else
                        URL = "ReportPopup.aspx?Report=PNCNotesDepDetail.rpt&ReportTitle=Notes Deposit Detail&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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

    }
}