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
    public partial class PNCMeanTimeToIncidentFailureAnalysis : BasePage
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
                //filter += " and dispute_status is null ";
                filter += " and (dispute_status is null or dispute_status = '000') ";
            }

            //if (CheckBoxList_Status.Items[0].Selected && CheckBoxList_Status.Items[1].Selected)
            //{
            //}
            //else if (CheckBoxList_Status.Items[0].Selected)
            //    filter += " and status = 'Successful'";
            //else if (CheckBoxList_Status.Items[1].Selected)
            //    filter += " and status = 'Failed'";

            if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1)
                filter += " and is_sdm = 1 ";
            else if (RadioButtonList_Module.SelectedIndex == 2)
                filter += " and is_cdm = 1 ";

            //if (DropDownList_Comment.SelectedIndex>0)
            //    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";



            filter += " and is_eligible=1  ";
            return filter;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.PNCMeanTimeToFailureAnalysisReport))
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

                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
                dtGrid.Columns.Add("counter", typeof(int));
                dtGrid.Columns.Add("type");
                cmd = ConnectionFactory.GetNewCommand(false);

                if (RadioButtonList_Module.SelectedIndex == 0 )
                {
                    List<int> parsedCPMTransactions = new List<int>();
                    //int count = 0;
                    int successfulChqDeposit = 0;
                    int id = 0;


                    string SQL = @"select atm.atm_id,* from ej_parsed_cpm_transaction 
                                  left join ej_parsed_cpm_transaction_detail 
                                  on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                                 " inner join atm on atm.atm_id = ej_parsed_cpm_transaction.atm_id " +
                        " where atm.is_active=1 " + GetFilter() + "  order by trxn_datetime ";
                    DataTable dt = GetDataTable(SQL);
                    DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id", "title" });
                    DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "atm_id", "trxn_datetime" });

                    foreach (DataRow drATM in dtATMs.Rows)
                    {
                        for (int i = 0; i < dtTrxns.Rows.Count; i++)
                        {
                            //DataRow[] drArray = dt.Select(
                            //    "trxn_datetime >= #" + DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()).ToString("MM/dd/yyyy") +
                            //    "# and trxn_datetime <= #" + DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()).ToString("MM/dd/yyyy") + " 23:59:59# and atm_id = " + drATM["atm_id"]);

                            DataRow[] drArray = dt.Select(
                                "trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] +"# and atm_id = " + drATM["atm_id"]);
                          
                            
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
                                        successfulChqDeposit++; // Successful cheque deposit count;
                                }
                                else if (dr["status"].ToString() == "Failed" && successfulChqDeposit > 0)
                                {
                                    DataRow newRow = dtGrid.NewRow();
                                    newRow["title"] = drATM["title"];
                                    newRow["trxn_datetime"] = dr["trxn_datetime"];
                                    newRow["counter"] = successfulChqDeposit;
                                    newRow["type"] = dr["comment"];
                                    dtGrid.Rows.Add(newRow);
                                    successfulChqDeposit = 0;
                                }
                            }
                        }

                    }


                    //ds = new dsMeanTimeToFailureAnalysis();
                    //ds.Tables[0].Merge(dtGrid);
                }
                else
                {

                    List<int> parsedBNATransactions = new List<int>();
                   // int count = 0;
                    int successfulChqDeposit = 0;
                    int id = 0;

                    string SQL = "select * from ej_parsed_bna_transaction left join ej_parsed_bna_transaction_detail on ej_parsed_bna_transaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                         " inner join atm on atm.atm_id = ej_parsed_bna_transaction.atm_id " +
                        " where atm.is_active=1 " + GetFilter() + "  order by trxn_datetime ";
                    DataTable dt = GetDataTable(SQL);
                    DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id", "title" });
                    DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "atm_id", "trxn_datetime" });
                    foreach (DataRow drATM in dtATMs.Rows)
                    {
                        for (int i = 0; i < dtTrxns.Rows.Count; i++)
                        {
                            //DataRow[] drArray = dt.Select(
                            //    "trxn_datetime >= #" + DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()).ToString("MM/dd/yyyy") +
                            //    "# and trxn_datetime <= #" + DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()).ToString("MM/dd/yyyy") + " 23:59:59# and atm_id = " + drATM["atm_id"]);

                            DataRow[] drArray = dt.Select(
                                "trxn_datetime = #" + DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()).ToString("MM/dd/yyyy HH:mm:ss")+ "# and atm_id = " + drATM["atm_id"]);
                            
                            
                            foreach (DataRow dr in drArray)
                            {
                                id = int.Parse(dr["ej_parsed_bna_transaction_id"].ToString());
                                if (parsedBNATransactions.Contains(id))
                                    continue;
                                else
                                    parsedBNATransactions.Add(id);

                                if (dr["status"].ToString() == "Successful")
                                {
                                    successfulChqDeposit++;
                                    //DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id = " + dr["ej_parsed_bna_transaction_id"]);

                                    //if (temp.Length > 0)
                                    //{
                                    //    DataTable dtUniqueNoteType = temp.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                    //    foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                    //    {
                                    //        DataRow[] innerTemp = dt.Select("ej_parsed_bna_transaction_id=" + dr["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"]);
                                    //        //builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                    //        successfulChqDeposit += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                                    //    }

                                    //}

                                    //if (temp != null)
                                    //{
                                    //    foreach (DataRow drTemp in temp)
                                    //        successfulChqDeposit += int.Parse(drTemp["notes_count"].ToString()); // Successful cheque deposit count;
                                    //}
                                }
                                else if (dr["status"].ToString() == "Failed" && successfulChqDeposit > 0)
                                {
                                    DataRow newRow = dtGrid.NewRow();
                                    newRow["title"] = drATM["title"];
                                    newRow["trxn_datetime"] = dr["trxn_datetime"];
                                    newRow["counter"] = successfulChqDeposit;
                                    newRow["type"] = dr["comment"];
                                    dtGrid.Rows.Add(newRow);
                                    successfulChqDeposit = 0;
                                }
                            }
                        }


                    }
                 
                }
                ds = new dsMeanTimeToFailureAnalysis();
                ds.Tables[0].Merge(dtGrid);

                //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "query ->"+cmd.CommandText);

                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.Fill(ds.Tables["DataTable1"]);
                ////SplitAlertMessageIntoColumns(ds.Tables["DataTable1"]);

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    string title = null;
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;

                    //if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
                        title = RadioButtonList_Module.SelectedItem.Text + " Mean Time To Failure Analysis";
                    //else
                     //   title = "BNA Weekly Analysis";
                    string URL = "ReportPopup.aspx?Report=PNCMeanTimeToFailureAnalysisRep.rpt&ReportTitle=" + title + "&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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