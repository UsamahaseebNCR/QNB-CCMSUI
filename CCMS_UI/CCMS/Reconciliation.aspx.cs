using System;
using System.Web.UI;
using Avanza.iSuite.UI;
//using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class Reconciliation : BasePage
    {

        protected void GridView_Batch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Batch.PageIndex = e.NewPageIndex;
            //BindGrid(null);
            BindGrid();
        }

        protected void GridView_Batch_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
            //if (ViewState["SortExpression"].ToString() == "creation_time")
            //    ViewState["SortExpression"] = "f.creation_time";

            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            ///BindGrid(null);
            BindGrid();
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewReconciliation, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                ViewState["SortExpression"] = "creation_time";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                //AppUser.AppUserReader userReader = AppUser.ExecuteReader("user_id > 1 order by user_login", AppUser.Columns.user_id | AppUser.Columns.user_login);
                //Dropdownlist_user.Items.Add(new ListItem("Any", "Any"));
                //Dropdownlist_user.Items.Add(new ListItem("Automatically", "1"));
                //while (userReader.Read())
                //    Dropdownlist_user.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                //userReader.Close();
                BindGrid();
            }
        }

        string GetFilter()
        {
            string filter = "";
            if ((Session[SessionVars.SelectedATMs.ToString()].ToString()) != string.Empty)
            {
                filter += "a.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()].ToString() + ") and ";
            }

            if (TextBox_FromDate.Text.Length > 0)
                filter += "r.creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103) and ";

            if (TextBox_ToDate.Text.Length > 0)
                filter += "r.creation_time<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and ";

            //if (Dropdownlist_user.SelectedIndex > 0)
            //    filter += "r.created_by = " + Dropdownlist_user.SelectedValue + " and ";

            if (DropDownList_status.SelectedIndex > 0)
                filter += "r.status = '" + DropDownList_status.SelectedValue + "' and ";

            if (filter.EndsWith("and "))
                filter = "where " + filter.Substring(0, filter.LastIndexOf("and "));
            return filter;
        }



        void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandText = @"select r.creation_time,a.title,transaction_start_date,transaction_end_date,acceptable_difference,status,reconciliation_batch_id,failure_reason,
                                    no_of_records_processed,no_of_records_reconciled,
                                    no_of_records_failed_to_reconciled,failure_reason,retry_count  from reconciliation_batch r inner join atm a on r.atm_id = a.ATM_id " + GetFilter()
                                 + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);
                GridView_Batch.DataSource = dtTrxn;
                GridView_Batch.DataBind();
                Literal_count.Text = dtTrxn.Rows.Count.ToString();
            }



            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }


        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }

        protected void button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {

        }
        protected void button_loadData_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("trxn_datetime");
            dt.Columns.Add("withdrawals");
            dt.Columns.Add("balance");


            //if (FileUpload_TrxnFromSwitch.HasFile)
            //{

            //    string fileContent = Encoding.ASCII.GetString(FileUpload_TrxnFromSwitch.FileBytes);
            //    string[] parts = fileContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (string part in parts)
            //    {
            //        string[] subParts = part.Split(',');
            //        DataRow dr = dt.NewRow();
            //        dr[0] = subParts[0];
            //        dr[1] = subParts[1];
            //        dr[2] = subParts[2];
            //        dr[3] = subParts[3];
            //        dt.Rows.Add(dr);
            //    }
            //    GridView1.DataSource = dt;
            //    GridView1.DataBind();
            // }
        }


        protected void GridView_Batch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            SqlCommand cmd = null;
            StringBuilder builder = new StringBuilder();
            int? batchID = null;
            try
            {
                task = LogableTask.NewTask("GridView_Batch_RowCommand");

                if (e.CommandName == "reExecute")
                {
                    batchID = int.Parse(e.CommandArgument.ToString());


                    builder.Append("delete reconciliation_transactions where reconciliation_batch_id=" + batchID + ";");

                    //builder.Append(@"update reconciliation_gl_account
                    //                set opening_balance = opening_balance+(
                    //                select isnull(sum(opening_balance-closing_balance),0)
                    //                from reconciliation_gl_account_summary
                    //                where reconciliation_batch_id = " + batchID + ");");

                    //builder.Append("delete reconciliation_gl_account_summary where reconciliation_batch_id=" + batchID + ";");
                    builder.Append("delete reconciled_transactions where batch_id=" + batchID + ";");

                    builder.Append("delete Recon_Batch_Info where Reconciliation_Batch_Id = " + batchID + ";");

                    builder.Append("update reconciliation_batch set status = 'Scheduled',reprocess_count = case when reprocess_count is null then 1 else reprocess_count+1 end,retry_count=10,auto_reconciled_amount=null,manual_reconciled_amount=null,dropped_transaction_amount = null,no_of_records_processed=0,no_of_records_reconciled=0,no_of_records_failed_to_reconciled=0 where reconciliation_batch_id=" + batchID + ";");

                    ConnectionFactory.ExecuteQuery(builder.ToString());

                    //using (SqlCommand cmd1 = ConnectionFactory.GetNewCommand(true))
                    //{
                    //    cmd1.CommandText = "BatchReprocess";
                    //    cmd1.CommandType = CommandType.StoredProcedure;
                    //    cmd1.Parameters.Add(new SqlParameter("@reconBatchId", SqlDbType.Int));
                    //    cmd1.Parameters[0].Value = batchID;
                    //    cmd1.ExecuteNonQuery();
                    //}
                    Literal_ok.Text = "Batch Rescheduled Successfully";
                    BindGrid();
                }
                else if (e.CommandName == "export")
                {
                    batchID = int.Parse(e.CommandArgument.ToString());

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandText = "select * from reconciled_transactions where batch_id=" + batchID;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    

                    foreach (DataRow dr in dt.Rows)
                    {
                        builder.Append(dr["title"] + ",");
                        builder.Append(dr["trxn_datetime"] + ",");
                        builder.Append(dr["pan"] + ",");
                        builder.Append(dr["tsn"] + ",");
                        builder.Append(dr["amount"] + ",");
                        //builder.Append(dr["ej_status"] + ",");
                        //builder.Append(dr["switch_status"] + ",");
                        //builder.Append(dr["host_status"] + ",");
                        //builder.Append(dr["recon_status"] + ",");
                        //builder.Append(dr["reason"] + ",");
                        builder.Append("\r\n");

                    }
                    string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\ReconciliationFile.csv";
                    File.WriteAllText(path, builder.ToString());
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=ReconciliationFile.csv");
                    Response.TransmitFile(path);
                    Response.Flush();
                    Response.Close();
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured while exporting reconciliation report. " + ex.Message + "</li>";

            }
            finally
            {

                task.EndTask();

            }

        }

    }
}