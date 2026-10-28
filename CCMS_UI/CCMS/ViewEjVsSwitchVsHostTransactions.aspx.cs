using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using ComponentArt.Web.UI;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
//using Avanza.iSuite.DAL;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;

namespace CCMSUI.CCMS
{
    public partial class ViewEjVsSwitchVsHostTransactions : BasePage
    {
        string[] dateFormats = new string[] { "dd/MM/yyyy HH:mm","dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy h:mm:ss tt", "dd/M/yyyy h:mm:ss tt",
                                              "d/MM/yyyy HH:mm:ss", "d/MM/yyyy h:mm:ss tt", "d/M/yyyy h:mm:ss tt",
                                              "MM/dd/yyyy HH:mm:ss","MM/dd/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss tt",
                                              "M/dd/yyyy HH:mm:ss","M/dd/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss tt"};

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                if (!Page.IsPostBack)
                {
                    trSearchResults.Visible = false;

                    if (!CheckRight((int)Permissions.ViewReconData))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    ViewState["SortExpressionEJSwitch"] = "tsn";
                    ViewState["SortDirectionEJSwitch"] = " asc";
                    ViewState["SortExpressionEJBNASwitch"] = "seq";
                    ViewState["SortDirectionEJBNASwitch"] = " asc";

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    //ViewState["SortExpression"] = "creation_time";
                    //ViewState["sortDirection"] = " asc";
                    //ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                    //if (DropDownList_TransactionType.SelectedValue == "1")
                    //{
                    //    BindEJSwitchGrid(ViewState["SortExpressionEJSwitch"] + " " + ViewState["SortDirectionEJSwitch"]);
                    //}
                    //else if(DropDownList_TransactionType.SelectedValue == "2")
                    //{
                    //    BindEJBNASwitchGrid(ViewState["SortExpressionEJBNASwitch"] + " " + ViewState["SortDirectionEJBNASwitch"]);
                    //}

                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
            finally
            { task.EndTask(); }
        }

        private void CreateReconciledTrxnEntry(int batchID, decimal amount, string title, string tsn, DateTime trxnDatetime,
            SqlTransaction trxn, string pan, string source)
        {
            ReconciledTransactions reconciledTrxn = new ReconciledTransactions();
            reconciledTrxn.GeneratedAt = DateTime.Now;
            reconciledTrxn.BatchId = batchID;
            reconciledTrxn.Amount = amount;
            reconciledTrxn.Title = title;
            reconciledTrxn.Tsn = tsn;
            reconciledTrxn.TrxnDatetime = trxnDatetime;
            reconciledTrxn.Pan = pan;
            reconciledTrxn.Source = source;
            reconciledTrxn.Save(trxn.Connection, trxn);
        }

        protected void GridView_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem).Row["reason"].ToString().Contains("missing")) { }
                //e.Row.Cells[8].Visible = false;
            }

        }


        //protected void Button_Drop_Click(object sender, EventArgs e)
        //{
        //    LogableTask task = LogableTask.NewTask("Button_Drop_Click");
        //    SqlConnection SqlConn = ConnectionFactory.GetNewConnection();
        //    SqlConn.Open();
        //    SqlTransaction SqlTrx = SqlConn.BeginTransaction();
        //    SqlCommand cmd = SqlConn.CreateCommand();
        //    cmd.Transaction = SqlTrx;

        //    try
        //    {
        //        foreach (GridViewRow row in GridView_EJSwitch.Rows)
        //        {
        //            string reconciliation_id = this.GridView_EJSwitch.DataKeys[row.DataItemIndex].Values["reconciliation_transactions_id"].ToString();
        //            CheckBox chk = (CheckBox)row.FindControl("chkbx");
        //            if (chk.Checked) // means now it is set to 'scheduled'
        //            {
        //                ReconciliationTransactions reconTrxn = ReconciliationTransactions.LoadReconciliationTransactionsByPk(int.Parse(reconciliation_id));
        //                if (reconTrxn != null)
        //                {
        //                    reconTrxn.UpdatedAt = DateTime.Now;
        //                    reconTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
        //                    reconTrxn.IsReconciledManually = false;
        //                    reconTrxn.Status = "Dropped";

        //                    //ReconciliationBatch reconBatch = ReconciliationBatch.LoadReconciliationBatchByPk(reconTrxn.ReconciliationBatchId);
        //                    //if (reconBatch != null)
        //                    //{
        //                    //    reconBatch.NoOfRecordsReconciled++;
        //                    //    reconBatch.NoOfRecordsFailedToReconciled--;
        //                    //    reconBatch.Save(SqlTrx.Connection, SqlTrx);
        //                    //}
        //                    reconTrxn.Save(SqlTrx.Connection, SqlTrx);
        //                }
        //            }
        //        }
        //        foreach (GridViewRow row in GridView_SwitchHost.Rows)
        //        {
        //            string reconciliation_id = this.GridView_SwitchHost.DataKeys[row.DataItemIndex].Values["reconciliation_transactions_id"].ToString();
        //            CheckBox chk = (CheckBox)row.FindControl("chkbx");
        //            if (chk.Checked) // means now it is set to 'scheduled'
        //            {
        //                ReconciliationTransactions reconTrxn = ReconciliationTransactions.LoadReconciliationTransactionsByPk(int.Parse(reconciliation_id));
        //                if (reconTrxn != null)
        //                {
        //                    reconTrxn.UpdatedAt = DateTime.Now;
        //                    reconTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
        //                    reconTrxn.IsReconciledManually = false;
        //                    reconTrxn.Status = "Dropped";

        //                    //ReconciliationBatch reconBatch = ReconciliationBatch.LoadReconciliationBatchByPk(reconTrxn.ReconciliationBatchId);
        //                    //if (reconBatch != null)
        //                    //{
        //                    //    reconBatch.NoOfRecordsReconciled++;
        //                    //    reconBatch.NoOfRecordsFailedToReconciled--;
        //                    //    reconBatch.Save(SqlTrx.Connection, SqlTrx);
        //                    //}
        //                    reconTrxn.Save(SqlTrx.Connection, SqlTrx);
        //                }
        //            }
        //        }

        //        SqlTrx.Commit();
        //        BindEJSwitchGrid(ViewState["SortExpressionEJSwitch"] + " " + ViewState["SortDirectionEJSwitch"]);
        //        BindEJBNASwitchGrid(ViewState["SortExpressionEJBNASwitch"] + " " + ViewState["SortDirectionEJBNASwitch"]);
        //        //Utility.BuildAuditLog("ScheduleDownload", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ScheduleFileDownload);

        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "trxn Dropped Successfully");
        //        Literal_ok.Text = "<script>ShowSuccess('trxn Dropped Successfully.');</script>";
        //        //Response.Write("<script>ShowSuccess('changes saved.');</script>");
        //    }
        //    catch (Exception ex)
        //    {
        //        SqlTrx.Rollback();
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //        SqlConn.Close();
        //    }

        //}

        //protected void Button_Reconcile_Click(object sender, EventArgs e)
        //{
        //    LogableTask task = LogableTask.NewTask("Button_Reconcile_Click");
        //    SqlConnection SqlConn = ConnectionFactory.GetNewConnection();
        //    SqlConn.Open();
        //    SqlTransaction SqlTrx = SqlConn.BeginTransaction();
        //    SqlCommand cmd = SqlConn.CreateCommand();
        //    cmd.Transaction = SqlTrx;

        //    try
        //    {
        //        foreach (GridViewRow row in GridView_EJSwitch.Rows)
        //        {
        //            string reconciliation_id = this.GridView_EJSwitch.DataKeys[row.DataItemIndex].Values["reconciliation_transactions_id"].ToString();
        //            CheckBox chk = (CheckBox)row.FindControl("chkbx");
        //            if (chk.Checked) // means now it is set to 'scheduled'
        //            {
        //                ReconciliationTransactions reconTrxn = ReconciliationTransactions.LoadReconciliationTransactionsByPk(int.Parse(reconciliation_id));
        //                if (reconTrxn != null)
        //                {
        //                    reconTrxn.UpdatedAt = DateTime.Now;
        //                    reconTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
        //                    reconTrxn.IsReconciledManually = true;
        //                    ReconciliationBatch reconBatch = ReconciliationBatch.LoadReconciliationBatchByPk(reconTrxn.ReconciliationBatchId);
        //                    if (reconBatch != null)
        //                    {
        //                        reconBatch.NoOfRecordsReconciled++;
        //                        reconBatch.NoOfRecordsFailedToReconciled--;
        //                        reconBatch.Save(SqlTrx.Connection, SqlTrx);
        //                    }
        //                    reconTrxn.Save(SqlTrx.Connection, SqlTrx);
        //                }
        //            }
        //        }

        //        SqlTrx.Commit();
        //        BindEJSwitchGrid(ViewState["SortExpressionEJSwitch"] + " " + ViewState["SortDirectionEJSwitch"]);
        //        BindEJBNASwitchGrid(ViewState["SortExpressionEJBNASwitch"] + " " + ViewState["SortDirectionEJBNASwitch"]);
        //        //Utility.BuildAuditLog("ScheduleDownload", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ScheduleFileDownload);

        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "trxn Reconciled Forcefully");
        //        Literal_ok.Text = "<script>ShowSuccess('trxn Reconciled Forcefully.');</script>";
        //        //Response.Write("<script>ShowSuccess('changes saved.');</script>");
        //    }
        //    catch (Exception ex)
        //    {
        //        SqlTrx.Rollback();
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //        SqlConn.Close();
        //    }
        //}

        private void BindEJSwitchGrid(string orderBy)
        {
            try
            {
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
                dtGrid.Columns.Add("PAN");
                dtGrid.Columns.Add("tsn");
                //dtGrid.Columns.Add("amount");
                dtGrid.Columns.Add("ej_amount");
                dtGrid.Columns.Add("switch_amount");
                dtGrid.Columns.Add("host_amount");
                dtGrid.Columns.Add("reconciliation_transactions_id");
                //dtGrid.Columns.Add("is_on_ej", typeof(bool));
                //dtGrid.Columns.Add("is_on_switch", typeof(bool));
                dtGrid.Columns.Add("reason");
                dtGrid.Columns.Add("status");
                dtGrid.Columns.Add("status_on_ej");
                dtGrid.Columns.Add("status_on_switch");
                dtGrid.Columns.Add("status_on_host");
                dtGrid.Columns.Add("accountno");

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                string typeFilter = "", statusFilter = "";

                if (DropDownList_TransactionType.SelectedValue != "All")
                {
                    typeFilter = " comparison_type = @Type and";
                }
                if (DropDownList_TransactionStatus.SelectedValue != "All")
                {
                    if (DropDownList_TransactionStatus.SelectedValue != "Matched")
                    {
                        if (DropDownList_DisSusOn.SelectedValue != "All")
                        {
                            statusFilter = $" status = @Status and reason like '%{DropDownList_DisSusOn.SelectedValue}%' and";
                        }
                        else
                        {
                            statusFilter = " status = @Status and";
                        }
                    }
                    else
                    {
                        statusFilter = " status = @Status and";

                    }
                }

                cmd.CommandText =
                    @"select * from vreconciliationtransactions where " + typeFilter + statusFilter +
                    @" reconciliation_batch_id in (select reconciliation_batch_id from reconciliation_batch " + GetFilter() + @")";

                if (DropDownList_TransactionStatus.SelectedValue != "All")
                {
                    cmd.Parameters.AddWithValue("@Status", DropDownList_TransactionStatus.SelectedValue);
                }

                if (DropDownList_TransactionType.SelectedValue != "All")
                {
                    cmd.Parameters.AddWithValue("@Type", DropDownList_TransactionType.SelectedValue);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable DT = new DataTable();
                adapter.Fill(DT);
                foreach (DataRow dr in DT.Rows)
                {
                    if (dr["ej_parsed_transactions_id"].ToString() != "0" && dr["ej_parsed_transactions_id"].ToString() != "")
                    {
                        VEjParsedTransactions ejTrxn = VEjParsedTransactions.LoadVEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                        if (ejTrxn != null)
                        {
                            DataRow dataRow = dtGrid.NewRow();
                            dataRow["trxn_datetime"] = ejTrxn.TrxnDatetime;
                            dataRow["PAN"] = ejTrxn.Pan;
                            dataRow["tsn"] = ejTrxn.Tsn;
                            dataRow["ej_amount"] = ejTrxn.Amount;
                            dataRow["title"] = Atm.LoadAtmByPk(ejTrxn.AtmId.Value).Title;
                            dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                            //dataRow["is_on_ej"] = true;
                            dataRow["reason"] = dr["reason"].ToString();
                            dataRow["status"] = dr["status"];
                            dataRow["accountno"] = ejTrxn.AccountNo;

                            dataRow["status_on_ej"] = GetStatusFriendlyName(ejTrxn.Status.Value) + "(" + ejTrxn.Result + ")";

                            if (dr["reconciliation_switch_data_id"].ToString() != null && dr["reconciliation_switch_data_id"] != DBNull.Value && dr["reconciliation_switch_data_id"].ToString() != "" && dr["reconciliation_switch_data_id"].ToString() != "0")
                            {
                                //ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                                //if (switchData != null)
                                //    dataRow["status_on_switch"] = switchData.TransactionResponse;

                                DataTable DTSwitch = GetDTData("select * from Vreconciliationswitchdata where reconciliation_switch_data_id=" + dr["reconciliation_switch_data_id"].ToString());
                                if (DTSwitch.Rows.Count > 0)
                                {
                                    dataRow["status_on_switch"] = DTSwitch.Rows[0]["transaction_response"].ToString();
                                    dataRow["switch_amount"] = DTSwitch.Rows[0]["transaction_amount"];
                                }

                            }


                            if (dr["reconciliation_host_data_id"].ToString() != null && dr["reconciliation_host_data_id"] != DBNull.Value && dr["reconciliation_host_data_id"].ToString() != "" && dr["reconciliation_host_data_id"].ToString() != "0")
                            {
                                ///var s = dr["reconciliation_host_data_id"].ToString();
                                //ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_host_data_id"].ToString()));
                                //if (hostData != null)
                                //    dataRow["status_on_host"] = hostData.TransactionResponse;
                                DataTable DTHost = GetDTData("select * from Vreconciliationhostdata where reconciliation_host_data_id=" + dr["reconciliation_host_data_id"].ToString());
                                if (DTHost.Rows.Count > 0)
                                {
                                    dataRow["status_on_host"] = DTHost.Rows[0]["transaction_response"].ToString();
                                    dataRow["host_amount"] = DTHost.Rows[0]["transaction_amount"];
                                }
                            }

                            dtGrid.Rows.Add(dataRow);
                        }
                    }
                    else
                    {
                        if (dr["reconciliation_switch_data_id"].ToString() != null && dr["reconciliation_switch_data_id"] != DBNull.Value && dr["reconciliation_switch_data_id"].ToString() != "" && dr["reconciliation_switch_data_id"].ToString() != "0")
                        {
                            //ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                            //if (switchData != null)
                            DataTable DTSwitch = GetDTData("select * from VreconciliationSwitchdata where reconciliation_switch_data_id=" + dr["reconciliation_switch_data_id"].ToString());

                            if (DTSwitch.Rows.Count > 0)
                            {
                                DataRow dataRow = dtGrid.NewRow();

                                DateTime transactionDateTime;
                                //var dateParts = DateTime.ParseExact(DTSwitch.Rows[0]["transaction_date"].ToString(), "yyyyMMdd", null);
                                //string[] timeParts = DTSwitch.Rows[0]["transaction_time"].ToString().Split(':');
                                //transactionDateTime = new DateTime(dateParts.Year, dateParts.Month, dateParts.Day, int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse("00"));


                                DateTime datePart = DateTime.ParseExact(
                                DTSwitch.Rows[0]["transaction_date"].ToString(),
                                "dd/MM/yyyy hh:mm:ss tt",
                                CultureInfo.InvariantCulture);

                                int totalSeconds = int.Parse(DTSwitch.Rows[0]["transaction_time"].ToString());

                                transactionDateTime = datePart.Date.AddSeconds(totalSeconds);

                                //DateTime.TryParseExact(switchData.TransactionDate + " " + switchData.TransactionTime, dateFormats, null, System.Globalization.DateTimeStyles.None, out transactionDateTime);
                                dataRow["trxn_datetime"] = transactionDateTime;

                                //dataRow["trxn_datetime"] = switchData.TransactionDate + " " + switchData.TransactionTime;
                                dataRow["PAN"] = DTSwitch.Rows[0]["card_number"].ToString();
                                dataRow["tsn"] = DTSwitch.Rows[0]["transaction_sequence"].ToString();
                                dataRow["switch_amount"] = DTSwitch.Rows[0]["transaction_amount"].ToString();
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                dataRow["status_on_switch"] = DTSwitch.Rows[0]["transaction_response"].ToString();
                                Atm atm = Atm.LoadAtm("title = '" + DTSwitch.Rows[0]["atm_id"].ToString() + "'");
                                if (atm != null)
                                    dataRow["title"] = atm.Title;
                                //dataRow["is_on_switch"] = true;
                                //if (int.Parse(dr["ej_parsed_transactions_id"].ToString()) > 0)
                                //    dataRow["is_on_ej"] = true;
                                dataRow["reason"] = dr["reason"].ToString();
                                if (dr["reason"] != DBNull.Value)
                                {
                                    if (dr["reason"].ToString().ToLower().Contains("mismatch"))
                                    {
                                        if (dr["ej_parsed_transactions_id"].ToString() != "0" && dr["ej_parsed_transactions_id"].ToString() != "")
                                        {
                                            VEjParsedTransactions ejTrxn = VEjParsedTransactions.LoadVEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                                            dataRow["ej_amount"] = ejTrxn.Amount;
                                            dataRow["status_on_ej"] = GetStatusFriendlyName(ejTrxn.Status.Value) + "(" + ejTrxn.Result + ")";
                                        }
                                    }
                                }
                                dataRow["status"] = dr["status"];
                                dataRow["accountno"] = DTSwitch.Rows[0]["customer_account_no"].ToString();

                                if (dr["reconciliation_host_data_id"].ToString() != null && dr["reconciliation_host_data_id"] != DBNull.Value && dr["reconciliation_host_data_id"].ToString() != "" && dr["reconciliation_host_data_id"].ToString() != "0")
                                {
                                    //ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_host_data_id"].ToString()));
                                    //if (hostData != null)
                                    //    dataRow["status_on_host"] = hostData.TransactionResponse;

                                    DataTable DTHost = GetDTData("select * from Vreconciliationhostdata where reconciliation_host_data_id=" + dr["reconciliation_host_data_id"].ToString());
                                    if (DTHost.Rows.Count > 0)
                                    {
                                        dataRow["status_on_host"] = DTHost.Rows[0]["transaction_response"].ToString();
                                        dataRow["host_amount"] = DTHost.Rows[0]["transaction_amount"];
                                    }
                                }

                                dtGrid.Rows.Add(dataRow);
                            }
                        }
                        else if (dr["reconciliation_host_data_id"].ToString() != null && dr["reconciliation_host_data_id"] != DBNull.Value && dr["reconciliation_host_data_id"].ToString() != "" && dr["reconciliation_host_data_id"].ToString() != "0")
                        {
                            //ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_host_data_id"].ToString()));
                            //if (hostData != null)
                            DataTable DTHost = GetDTData("select * from Vreconciliationhostdata where reconciliation_host_data_id=" + dr["reconciliation_host_data_id"].ToString());
                            if (DTHost.Rows.Count > 0)
                            {
                                DataRow dataRow = dtGrid.NewRow();

                                DateTime transactionDateTime;

                                DateTime datePart = DateTime.ParseExact(
                                DTHost.Rows[0]["transaction_date"].ToString(),
                                "dd/MM/yyyy hh:mm:ss tt",
                                CultureInfo.InvariantCulture);

                                int totalSeconds = int.Parse(DTHost.Rows[0]["transaction_time"].ToString());

                                transactionDateTime = datePart.Date.AddSeconds(totalSeconds);

                                dataRow["trxn_datetime"] = transactionDateTime;

                                //dataRow["trxn_datetime"] = switchData.TransactionDate + " " + switchData.TransactionTime;
                                dataRow["PAN"] = DTHost.Rows[0]["card_number"].ToString();
                                dataRow["tsn"] = DTHost.Rows[0]["transaction_sequence"].ToString();
                                dataRow["host_amount"] = DTHost.Rows[0]["transaction_amount"].ToString();
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                dataRow["status_on_host"] = DTHost.Rows[0]["transaction_response"].ToString();
                                Atm atm = Atm.LoadAtm("title = '" + DTHost.Rows[0]["atm_id"].ToString() + "'");
                                if (atm != null)
                                    dataRow["title"] = atm.Title;
                                //dataRow["is_on_switch"] = true;
                                //if (int.Parse(dr["ej_parsed_transactions_id"].ToString()) > 0)
                                //    dataRow["is_on_ej"] = true;
                                dataRow["reason"] = dr["reason"].ToString();
                                if (dr["reason"] != DBNull.Value)
                                {
                                    if (dr["reason"].ToString().ToLower().Contains("mismatch"))
                                    {
                                        if (dr["ej_parsed_transactions_id"].ToString() != "0" && dr["ej_parsed_transactions_id"].ToString() != "")
                                        {
                                            VEjParsedTransactions ejTrxn = VEjParsedTransactions.LoadVEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                                            dataRow["ej_amount"] = ejTrxn.Amount;
                                            dataRow["status_on_ej"] = GetStatusFriendlyName(ejTrxn.Status.Value) + "(" + ejTrxn.Result + ")";
                                        }
                                    }
                                }
                                dataRow["status"] = dr["status"];
                                dataRow["accountno"] = DTHost.Rows[0]["customer_account_no"].ToString();

                                if (dr["reconciliation_switch_data_id"].ToString() != null && dr["reconciliation_switch_data_id"] != DBNull.Value && dr["reconciliation_switch_data_id"].ToString() != "" && dr["reconciliation_switch_data_id"].ToString() != "0")
                                {
                                    //ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                                    //if (switchData != null)
                                    DataTable DTSwitch = GetDTData("select * from VreconciliationSwitchdata where reconciliation_switch_data_id=" + dr["reconciliation_switch_data_id"].ToString());
                                    if (DTSwitch.Rows.Count > 0)
                                    {
                                        dataRow["status_on_switch"] = DTSwitch.Rows[0]["transaction_response"].ToString();
                                        dataRow["switch_amount"] = DTSwitch.Rows[0]["transaction_amount"];
                                    }
                                }

                                dtGrid.Rows.Add(dataRow);
                            }
                        }

                    }
                }
                //GridView_EJSwitch.DataSource = dtGrid.DefaultView;
                //GridView_EJSwitch.DataBind();
                //Literal_count.Text = dtGrid.Rows.Count.ToString();

                //bool hasData = dtGrid.Rows.Count > 0;
                //trSearchResults.Visible = hasData;

                dtGrid.DefaultView.Sort = orderBy;

                GridView_EJSwitch.DataSource = dtGrid.DefaultView;
                GridView_EJSwitch.DataBind();

                Literal_count.Text = dtGrid.Rows.Count.ToString();

                bool hasData = dtGrid.Rows.Count > 0;

                divDispense.Visible = hasData;
                divDeposit.Visible = false;
                trSearchResults.Visible = hasData;

            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("BindEJSwitchGrid");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
        }

        private void BindEJBNASwitchGrid(string orderBy)
        {
            try
            {
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
                dtGrid.Columns.Add("PAN");
                dtGrid.Columns.Add("seq");
                dtGrid.Columns.Add("ej_amount");
                dtGrid.Columns.Add("switch_amount");
                dtGrid.Columns.Add("host_amount");
                dtGrid.Columns.Add("reconciliation_transactions_id");
                //dtGrid.Columns.Add("is_on_ej", typeof(bool));
                //dtGrid.Columns.Add("is_on_switch", typeof(bool));
                dtGrid.Columns.Add("reason");
                dtGrid.Columns.Add("status");
                dtGrid.Columns.Add("status_on_ej");
                dtGrid.Columns.Add("status_on_switch");
                dtGrid.Columns.Add("status_on_host");
                dtGrid.Columns.Add("accountno");

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                string typeFilter = "", statusFilter = "";

                if (DropDownList_TransactionType.SelectedValue != "All")
                {
                    typeFilter = " comparison_type = @Type and";
                }
                if (DropDownList_TransactionStatus.SelectedValue != "All")
                {
                    if (DropDownList_TransactionStatus.SelectedValue != "Matched")
                    {
                        if (DropDownList_DisSusOn.SelectedValue != "All")
                        {
                            statusFilter = $" status = @Status and reason like '%{DropDownList_DisSusOn.SelectedValue}%' and";
                        }
                        else
                        {
                            statusFilter = " status = @Status and";
                        }
                    }
                    else
                    {
                        statusFilter = " status = @Status and";

                    }
                }

                cmd.CommandText =
                    @"select * from vreconciliationtransactions where " + typeFilter + statusFilter +
                    @" reconciliation_batch_id in (select reconciliation_batch_id from reconciliation_batch " + GetFilter() + @")";

                if (DropDownList_TransactionStatus.SelectedValue != "All")
                {
                    cmd.Parameters.AddWithValue("@Status", DropDownList_TransactionStatus.SelectedValue);
                }

                if (DropDownList_TransactionType.SelectedValue != "All")
                {
                    cmd.Parameters.AddWithValue("@Type", DropDownList_TransactionType.SelectedValue);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable DT = new DataTable();
                adapter.Fill(DT);
                foreach (DataRow dr in DT.Rows)
                {
                    if (dr["ej_parsed_bna_transaction_id"].ToString() != "0" && dr["ej_parsed_bna_transaction_id"].ToString() != "" && dr["ej_parsed_bna_transaction_id"] != DBNull.Value && dr["ej_parsed_bna_transaction_id"].ToString() != string.Empty)
                    {
                        VEjParsedBNATransaction ejBNATrxn = VEjParsedBNATransaction.LoadVEjParsedBNATransactionsByPk(int.Parse(dr["ej_parsed_bna_transaction_id"].ToString()));
                        if (ejBNATrxn != null)
                        {
                            DataRow dataRow = dtGrid.NewRow();
                            dataRow["trxn_datetime"] = ejBNATrxn.TrxnDatetime;
                            dataRow["PAN"] = ejBNATrxn.Pan;
                            dataRow["seq"] = ejBNATrxn.Seq;
                            dataRow["ej_amount"] = ejBNATrxn.AmountAuthorized;
                            dataRow["title"] = Atm.LoadAtmByPk(ejBNATrxn.AtmId).Title;
                            dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                            //dataRow["is_on_ej"] = true;
                            dataRow["reason"] = dr["reason"].ToString();
                            dataRow["status"] = dr["status"];
                            dataRow["accountno"] = ejBNATrxn.AccountNo;

                            dataRow["status_on_ej"] = ejBNATrxn.Status;

                            if (dr["reconciliation_bna_switch_data_id"].ToString() != "0" && dr["reconciliation_bna_switch_data_id"].ToString() != "" && dr["reconciliation_bna_switch_data_id"] != DBNull.Value && dr["reconciliation_bna_switch_data_id"].ToString() != string.Empty)
                            {
                                //ReconciliationBnaSwitchData switchData = ReconciliationBnaSwitchData.LoadReconciliationBnaSwitchDataByPk(int.Parse(dr["reconciliation_bna_switch_data_id"].ToString()));
                                //if (switchData != null)
                                DataTable DTSwitchBNA = GetDTData("select * from Vreconciliationbnaswitchdata where reconciliation_bna_switch_data_id=" + dr["reconciliation_bna_switch_data_id"].ToString());
                                if (DTSwitchBNA.Rows.Count > 0)
                                {
                                    dataRow["status_on_switch"] = DTSwitchBNA.Rows[0]["transaction_response"].ToString();
                                    dataRow["switch_amount"] = DTSwitchBNA.Rows[0]["transaction_amount"];
                                }
                            }

                            if (dr["reconciliation_bna_host_data_id"].ToString() != "0" && dr["reconciliation_bna_host_data_id"].ToString() != "" && dr["reconciliation_bna_host_data_id"] != DBNull.Value && dr["reconciliation_bna_host_data_id"].ToString() != string.Empty)
                            {
                                //ReconciliationBnaHostData hostData = ReconciliationBnaHostData.LoadReconciliationBnaHostDataByPk(int.Parse(dr["reconciliation_bna_host_data_id"].ToString()));
                                //if (hostData != null)
                                DataTable DTHostBNA = GetDTData("select * from Vreconciliationbnahostdata where reconciliation_bna_host_data_id=" + dr["reconciliation_bna_host_data_id"].ToString());
                                if (DTHostBNA.Rows.Count > 0)
                                {
                                    dataRow["status_on_host"] = DTHostBNA.Rows[0]["transaction_response"].ToString();
                                    dataRow["host_amount"] = DTHostBNA.Rows[0]["transaction_amount"];
                                }
                            }
                            dtGrid.Rows.Add(dataRow);
                        }
                    }
                    else
                    {
                        if (dr["reconciliation_bna_switch_data_id"].ToString() != "0" && dr["reconciliation_bna_switch_data_id"].ToString() != "" && dr["reconciliation_bna_switch_data_id"] != DBNull.Value && dr["reconciliation_bna_switch_data_id"].ToString() != string.Empty)
                        {
                            //ReconciliationBnaSwitchData switchData = ReconciliationBnaSwitchData.LoadReconciliationBnaSwitchDataByPk(int.Parse(dr["reconciliation_bna_switch_data_id"].ToString()));
                            //if (switchData != null)
                            DataTable DTSwitchBNA = GetDTData("select * from Vreconciliationbnaswitchdata where reconciliation_bna_switch_data_id=" + dr["reconciliation_bna_switch_data_id"].ToString());
                            if (DTSwitchBNA.Rows.Count > 0)
                            {
                                DataRow dataRow = dtGrid.NewRow();

                                DateTime transactionDateTime;
                                DateTime datePart = DateTime.ParseExact(
                                DTSwitchBNA.Rows[0]["transaction_date"].ToString(),
                                "dd/MM/yyyy hh:mm:ss tt",
                                CultureInfo.InvariantCulture);

                                int totalSeconds = int.Parse(DTSwitchBNA.Rows[0]["transaction_time"].ToString());

                                transactionDateTime = datePart.Date.AddSeconds(totalSeconds);
                                dataRow["trxn_datetime"] = transactionDateTime;
                                dataRow["PAN"] = DTSwitchBNA.Rows[0]["card_number"].ToString();
                                dataRow["seq"] = DTSwitchBNA.Rows[0]["transaction_sequence"].ToString();
                                dataRow["switch_amount"] = DTSwitchBNA.Rows[0]["transaction_amount"].ToString();
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                dataRow["status_on_switch"] = DTSwitchBNA.Rows[0]["transaction_response"].ToString();
                                Atm atm = Atm.LoadAtm("title = '" + DTSwitchBNA.Rows[0]["atm_id"].ToString() + "'");
                                if (atm != null)
                                    dataRow["title"] = atm.Title;
                                dataRow["reason"] = dr["reason"].ToString();
                                if (dr["reason"] != DBNull.Value)
                                {
                                    if (dr["reason"].ToString().ToLower().Contains("mismatch"))
                                    {
                                        if (dr["ej_parsed_bna_transaction_id"].ToString() != "0" && dr["ej_parsed_bna_transaction_id"].ToString() != "")
                                        {
                                            VEjParsedBNATransaction ejBNATrxn = VEjParsedBNATransaction.LoadVEjParsedBNATransactionsByPk(int.Parse(dr["ej_parsed_bna_transaction_id"].ToString()));
                                            dataRow["ej_amount"] = ejBNATrxn.AmountAuthorized;
                                            dataRow["status_on_ej"] = ejBNATrxn.Status;
                                        }
                                    }
                                }
                                dataRow["status"] = dr["status"];
                                dataRow["accountno"] = DTSwitchBNA.Rows[0]["customer_account_no"].ToString();

                                if (dr["reconciliation_bna_host_data_id"].ToString() != "0" && dr["reconciliation_bna_host_data_id"].ToString() != "" && dr["reconciliation_bna_host_data_id"] != DBNull.Value && dr["reconciliation_bna_host_data_id"].ToString() != string.Empty)
                                {
                                    //ReconciliationBnaHostData hostData = ReconciliationBnaHostData.LoadReconciliationBnaHostDataByPk(int.Parse(dr["reconciliation_bna_host_data_id"].ToString()));
                                    //if (hostData != null)
                                    DataTable DTHostBNA = GetDTData("select * from Vreconciliationbnahostdata where reconciliation_bna_host_data_id=" + dr["reconciliation_bna_host_data_id"].ToString());
                                    if (DTHostBNA.Rows.Count > 0)
                                    {
                                        dataRow["status_on_host"] = DTHostBNA.Rows[0]["transaction_response"].ToString();
                                        dataRow["host_amount"] = DTHostBNA.Rows[0]["transaction_amount"];
                                    }
                                }
                                dtGrid.Rows.Add(dataRow);
                            }
                        }
                        else if (dr["reconciliation_bna_host_data_id"].ToString() != "0" && dr["reconciliation_bna_host_data_id"].ToString() != "" && dr["reconciliation_bna_host_data_id"] != DBNull.Value && dr["reconciliation_bna_host_data_id"].ToString() != string.Empty)
                        {
                            //ReconciliationBnaHostData hostData = ReconciliationBnaHostData.LoadReconciliationBnaHostDataByPk(int.Parse(dr["reconciliation_bna_host_data_id"].ToString()));
                            //if (hostData != null)
                            DataTable DTHostBNA = GetDTData("select * from Vreconciliationbnahostdata where reconciliation_bna_host_data_id=" + dr["reconciliation_bna_host_data_id"].ToString());
                            if (DTHostBNA.Rows.Count > 0)
                            {
                                DataRow dataRow = dtGrid.NewRow();

                                DateTime transactionDateTime;

                                DateTime datePart = DateTime.ParseExact(
                                DTHostBNA.Rows[0]["transaction_date"].ToString(),
                                "dd/MM/yyyy hh:mm:ss tt",
                                CultureInfo.InvariantCulture);

                                int totalSeconds = int.Parse(DTHostBNA.Rows[0]["transaction_time"].ToString());

                                transactionDateTime = datePart.Date.AddSeconds(totalSeconds);

                                dataRow["trxn_datetime"] = transactionDateTime;

                                //dataRow["trxn_datetime"] = switchData.TransactionDate + " " + switchData.TransactionTime;
                                dataRow["PAN"] = DTHostBNA.Rows[0]["card_number"].ToString();
                                dataRow["seq"] = DTHostBNA.Rows[0]["transaction_sequence"].ToString();
                                dataRow["host_amount"] = DTHostBNA.Rows[0]["transaction_amount"].ToString();
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                dataRow["status_on_host"] = DTHostBNA.Rows[0]["transaction_response"].ToString();
                                Atm atm = Atm.LoadAtm("title = '" + DTHostBNA.Rows[0]["atm_id"].ToString() + "'");
                                if (atm != null)
                                    dataRow["title"] = atm.Title;
                                //dataRow["is_on_switch"] = true;
                                //if (int.Parse(dr["ej_parsed_transactions_id"].ToString()) > 0)
                                //    dataRow["is_on_ej"] = true;
                                dataRow["reason"] = dr["reason"].ToString();
                                if (dr["reason"] != DBNull.Value)
                                {
                                    if (dr["reason"].ToString().ToLower().Contains("mismatch"))
                                    {
                                        if (dr["ej_parsed_bna_transaction_id"].ToString() != "0" && dr["ej_parsed_bna_transaction_id"].ToString() != "")
                                        {
                                            VEjParsedBNATransaction ejBNATrxn = VEjParsedBNATransaction.LoadVEjParsedBNATransactionsByPk(int.Parse(dr["ej_parsed_bna_transaction_id"].ToString()));
                                            dataRow["ej_amount"] = ejBNATrxn.AmountAuthorized;
                                            dataRow["status_on_ej"] = ejBNATrxn.Status;
                                        }
                                    }
                                }
                                dataRow["status"] = dr["status"];
                                dataRow["accountno"] = DTHostBNA.Rows[0]["customer_account_no"].ToString();
                                if (dr["reconciliation_bna_switch_data_id"].ToString() != "0" && dr["reconciliation_bna_switch_data_id"].ToString() != "" && dr["reconciliation_bna_switch_data_id"] != DBNull.Value && dr["reconciliation_bna_switch_data_id"].ToString() != string.Empty)
                                {
                                    //ReconciliationBnaSwitchData switchData = ReconciliationBnaSwitchData.LoadReconciliationBnaSwitchDataByPk(int.Parse(dr["reconciliation_bna_switch_data_id"].ToString()));
                                    //if (switchData != null)
                                    DataTable DTSwitchBNA = GetDTData("select * from Vreconciliationbnaswitchdata where reconciliation_bna_switch_data_id=" + dr["reconciliation_bna_switch_data_id"].ToString());
                                    if (DTSwitchBNA.Rows.Count > 0)
                                    {
                                        dataRow["status_on_switch"] = DTSwitchBNA.Rows[0]["transaction_response"].ToString();
                                        dataRow["switch_amount"] = DTSwitchBNA.Rows[0]["transaction_amount"];
                                    }
                                }
                                dtGrid.Rows.Add(dataRow);
                            }
                        }
                    }
                }
                dtGrid.DefaultView.Sort = orderBy;

                GridView_EJBNASwitch.DataSource = dtGrid.DefaultView;
                GridView_EJBNASwitch.DataBind();

                Literal_count.Text = dtGrid.Rows.Count.ToString();

                bool hasData = dtGrid.Rows.Count > 0;

                divDispense.Visible = false;
                divDeposit.Visible = hasData;
                trSearchResults.Visible = hasData;
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("BindEJBNASwitchGrid");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
        }

        protected void GridView_EJSwitch_Sorting(object sender, GridViewSortEventArgs e)
        {

            if (ViewState["SortExpressionEJSwitch"].ToString() == e.SortExpression)
            {
                if (ViewState["SortDirectionEJSwitch"].ToString() == " asc")
                    ViewState["SortDirectionEJSwitch"] = " desc";
                else
                    ViewState["SortDirectionEJSwitch"] = " asc";
            }
            else
            {
                ViewState["SortExpressionEJSwitch"] = e.SortExpression;
                ViewState["SortDirectionEJSwitch"] = " asc";
            }
            BindEJSwitchGrid(ViewState["SortExpressionEJSwitch"] + " " + ViewState["SortDirectionEJSwitch"]);
        }

        protected void GridView_EJBNASwitch_Sorting(object sender, GridViewSortEventArgs e)
        {

            if (ViewState["SortExpressionEJBNASwitch"].ToString() == e.SortExpression)
            {
                if (ViewState["SortDirectionEJBNASwitch"].ToString() == " asc")
                    ViewState["SortDirectionEJBNASwitch"] = " desc";
                else
                    ViewState["SortDirectionEJBNASwitch"] = " asc";
            }
            else
            {
                ViewState["SortExpressionEJBNASwitch"] = e.SortExpression;
                ViewState["SortDirectionEJBNASwitch"] = " asc";
            }
            BindEJBNASwitchGrid(ViewState["SortExpressionEJBNASwitch"] + " " + ViewState["SortDirectionEJBNASwitch"]);
        }

        protected void GridView_EJSwitch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_EJSwitch.PageIndex = e.NewPageIndex;
            BindEJSwitchGrid(ViewState["SortExpressionEJSwitch"] + " " + ViewState["SortDirectionEJSwitch"]);
        }
        protected void GridView_EJBNASwitch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_EJBNASwitch.PageIndex = e.NewPageIndex;

            BindEJBNASwitchGrid(
                ViewState["SortExpressionEJBNASwitch"] + " " +
                ViewState["SortDirectionEJBNASwitch"]);
        }

        private void CreateGLAccountSummary(int batchID, int atmID, decimal amount, SqlTransaction trxn)
        {
            Atm atm = Atm.LoadAtmByPk(atmID);
            ReconciliationGlAccountSummary summary = new ReconciliationGlAccountSummary();
            summary.CreationTime = DateTime.Now;
            summary.AtmId = atm.ATMId;
            summary.ReconciliationBatchId = batchID;
            if (atm.GlNumber != null)
            {
                if (atm.GlNumber.Length > 0)
                {
                    ReconciliationGlAccount GLAccount = ReconciliationGlAccount.LoadReconciliationGlAccount("reconciliation_gl_account_name='" + atm.GlNumber + "'");
                    if (GLAccount != null)
                    {
                        summary.OpeningBalance = GLAccount.OpeningBalance;
                        summary.ClosingBalance = summary.OpeningBalance - amount;
                        summary.ReconciliationGlAccountId = GLAccount.ReconciliationGlAccountId;
                        GLAccount.OpeningBalance = summary.ClosingBalance;
                        GLAccount.Save(trxn.Connection, trxn);
                    }
                }
            }
            summary.Save(trxn.Connection, trxn);
        }

        protected void DialogCallBack_Callback(object sender, CallBackEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
            SqlTransaction dbTrxn = null;
            SqlCommand cmd = null;
            try
            {
                ReconciliationTransactions reconTrxn = ReconciliationTransactions.LoadReconciliationTransactionsByPk(int.Parse(e.Parameters[0]));
                if (reconTrxn != null)
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    //cmd.Connection.Open();
                    dbTrxn = cmd.Connection.BeginTransaction();

                    reconTrxn.UpdatedAt = DateTime.Now;
                    reconTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    ReconciliationBatch batch = ReconciliationBatch.LoadReconciliationBatchByPk(reconTrxn.ReconciliationBatchId);
                    if (e.Parameters[2] == "1")
                    {
                        reconTrxn.IsReconciledManually = true;
                        reconTrxn.Status = "Matched";
                        if (batch.ManualReconciledAmount == null)
                            batch.ManualReconciledAmount = 0;
                        batch.ManualReconciledAmount += decimal.Parse(e.Parameters[3]);
                        batch.NoOfRecordsFailedToReconciled--;
                        if ((int)ConnectionFactory.ExecuteScalar("select reconciliation_host_data_id from reconciliation_transactions where " +
                            " reconciliation_switch_data_id = " + reconTrxn.ReconciliationSwitchDataId + " and comparison_type=1 and reconciliation_batch_id=" + batch.ReconciliationBatchId) > 0)
                        {
                            batch.NoOfRecordsReconciled++;


                            if (e.Parameters[4] == "1") // EJ Vs Switch
                            {
                                EjParsedTransactions ejTrxn = EjParsedTransactions.LoadEjParsedTransactionsByPk(reconTrxn.EjParsedTransactionsId);
                                Atm atm = Atm.LoadAtmByPk(ejTrxn.AtmId.Value);
                                CreateGLAccountSummary(reconTrxn.ReconciliationBatchId, atm.ATMId, batch.ManualReconciledAmount.Value, dbTrxn);
                                CreateReconciledTrxnEntry(reconTrxn.ReconciliationBatchId, ejTrxn.Amount.Value, atm.Title, ejTrxn.Tsn, ejTrxn.TrxnDatetime.Value, dbTrxn, ejTrxn.Pan, "EJ");
                            }
                            //else if (e.Parameters[4] == "2") // Switch Vs Host
                            //{
                            //    ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(reconTrxn.ReconciliationSwitchDataId);
                            //    Atm atm = Atm.LoadAtm("title = '" + switchData.AtmId + "'");
                            //    CreateGLAccountSummary(reconTrxn.ReconciliationBatchId, atm.ATMId, batch.ManualReconciledAmount.Value, dbTrxn);

                            //    string date = switchData.TransactionDate;
                            //    string time = switchData.TransactionTime;

                            //    string[] dateParts = date.Split('/');
                            //    string[] timeParts = time.Split(':');
                            //    DateTime dateTime = new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]), int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));

                            //    CreateReconciledTrxnEntry(reconTrxn.ReconciliationBatchId, switchData.TransactionAmount.Value, switchData.AtmId, switchData.TransactionSequence,
                            //         dateTime, dbTrxn,switchData.pa);
                            //}
                            else if (e.Parameters[4] == "3")  //CCMS Vs Switch
                            {
                                ParsedTransaction ccmsTrxn = ParsedTransaction.LoadParsedTransactionByPk(reconTrxn.ParsedTransactionsId);
                                Atm atm = Atm.LoadAtmByPk(ccmsTrxn.AtmId);
                                CreateGLAccountSummary(reconTrxn.ReconciliationBatchId, atm.ATMId, batch.ManualReconciledAmount.Value, dbTrxn);
                                CreateReconciledTrxnEntry(reconTrxn.ReconciliationBatchId, ccmsTrxn.Amount, atm.Title, ccmsTrxn.Tsn, ccmsTrxn.TrxnDatetime, dbTrxn, ccmsTrxn.Pan, "CCMS");
                            }



                        }

                    }
                    else
                    {
                        reconTrxn.IsReconciledManually = false;
                        reconTrxn.Status = "Dropped";
                        if (batch.DroppedTransactionAmount == null)
                            batch.DroppedTransactionAmount = 0;

                        batch.DroppedTransactionAmount += decimal.Parse(e.Parameters[3]);
                        batch.NoOfRecordsFailedToReconciled--;

                    }
                    reconTrxn.UserComments = e.Parameters[1];
                    reconTrxn.Save(dbTrxn.Connection, dbTrxn);
                    batch.Save(dbTrxn.Connection, dbTrxn);
                    dbTrxn.Commit();
                }
                //e.Output.Write("<script>ShowSuccess('Transaction Dropped Successfully')</script>");
                e.Output.Write("<script>window.location = 'ViewReconTransactions.aspx?i='+document.getElementById('batchID').value;ShowSuccess('Transaction " + reconTrxn.Status + " Successfully') </script>");
            }
            catch (Exception ex)
            {
                dbTrxn.Rollback();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
                Literal_error.Text = ex.Message;
            }

            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask();
            }


        }
        private string GetStatusFriendlyName(int status)
        {
            string name = "";
            switch (status)
            {
                case 1:
                    name = "Failed";
                    break;
                case 0:
                    name = "Successful";
                    break;
                case 2:
                    name = "Suspicious";
                    break;

                default:
                    break;
            }
            return name;

        }

        protected void Button_ExportToExcel_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToExcel_Click");
            try
            {
                // Bindgrid();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=EJ_Switch_Host Dispense.csv");
                Response.Charset = "";
                Response.ContentType = "application/text";
                //GridView_EJSwitch.AllowPaging = false;
                //GridView_EJSwitch.DataBind();
                StringBuilder columnbind = new StringBuilder();
                for (int k = 0; k < GridView_EJSwitch.Columns.Count; k++)
                {
                    columnbind.Append(GridView_EJSwitch.Columns[k].HeaderText + ',');
                }
                columnbind.Append("\r\n");
                for (int i = 0; i < GridView_EJSwitch.Rows.Count; i++)
                {
                    for (int k = 0; k < GridView_EJSwitch.Columns.Count; k++)
                    {
                        columnbind.Append(GridView_EJSwitch.Rows[i].Cells[k].Text.Replace("&nbsp;", "") + ',');
                    }
                    columnbind.Append("\r\n");
                }
                Response.Output.Write(columnbind.ToString());
                Response.AppendHeader("Set-Cookie", "fileDownload=true; path=/");
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void Button_ExportToExcelBNA_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToExcelBNA_Click");
            try
            {
                // Bindgrid();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=EJ_Switch_Host BNA.csv");
                Response.Charset = "";
                Response.ContentType = "application/text";
                //GridView_EJSwitch.AllowPaging = false;
                //GridView_EJSwitch.DataBind();
                StringBuilder columnbind = new StringBuilder();
                for (int k = 0; k < GridView_EJBNASwitch.Columns.Count; k++)
                {
                    columnbind.Append(GridView_EJBNASwitch.Columns[k].HeaderText + ',');
                }
                columnbind.Append("\r\n");
                for (int i = 0; i < GridView_EJBNASwitch.Rows.Count; i++)
                {
                    for (int k = 0; k < GridView_EJBNASwitch.Columns.Count; k++)
                    {
                        columnbind.Append(GridView_EJBNASwitch.Rows[i].Cells[k].Text.Replace("&nbsp;", "") + ',');
                    }
                    columnbind.Append("\r\n");
                }
                Response.Output.Write(columnbind.ToString());
                Response.AppendHeader("Set-Cookie", "fileDownload=true; path=/");
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        DataTable GetDTData(string query)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = query;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable DT = new DataTable();
            adapter.Fill(DT);

            return DT;
        }
        private string GetFilter()
        {
            string filter = "";

            if (!string.IsNullOrEmpty(Session[SessionVars.SelectedATMs.ToString()].ToString()))
            {
                filter += " atm_id in (" +
                          Session[SessionVars.SelectedATMs.ToString()] +
                          ") and ";
            }

            if (!string.IsNullOrWhiteSpace(TextBox_FromDate.Text))
            {
                filter +=
                    " transaction_start_date >= convert(datetime,'"
                    + TextBox_FromDate.Text +
                    "',103) and ";
            }

            if (!string.IsNullOrWhiteSpace(TextBox_ToDate.Text))
            {
                filter +=
                    " transaction_start_date <= convert(datetime,'"
                    + TextBox_ToDate.Text +
                    " 23:59:59',103) and ";
            }

            if (filter.EndsWith("and "))
            {
                filter = "where " +
                         filter.Substring(0, filter.LastIndexOf("and "));
            }

            return filter;
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }

        //protected void button_search_Click(object sender, EventArgs e)
        //{
        //    if (DropDownList_TransactionType.SelectedValue == "1")
        //    {
        //        BindEJSwitchGrid(ViewState["SortExpressionEJSwitch"] + " " + ViewState["SortDirectionEJSwitch"]);
        //    }
        //    else if (DropDownList_TransactionType.SelectedValue == "2")
        //    {
        //        BindEJBNASwitchGrid(ViewState["SortExpressionEJBNASwitch"] + " " + ViewState["SortDirectionEJBNASwitch"]);
        //    }
        //}
        protected void button_search_Click(object sender, EventArgs e)
        {
            divDispense.Visible = false;
            divDeposit.Visible = false;
            trSearchResults.Visible = false;

            if (DropDownList_TransactionType.SelectedValue == "1")
            {
                BindEJSwitchGrid(
                    ViewState["SortExpressionEJSwitch"] + " " +
                    ViewState["SortDirectionEJSwitch"]);
            }
            else if (DropDownList_TransactionType.SelectedValue == "2")
            {
                BindEJBNASwitchGrid(
                    ViewState["SortExpressionEJBNASwitch"] + " " +
                    ViewState["SortDirectionEJBNASwitch"]);
            }
            else
            {
                Label_errorMsg.Text = "<li>Error! " + "Select Transaction Type" + "</li>";
            }
        }


        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            DropDownList_TransactionStatus.SelectedValue = "All";

            BindEJSwitchGrid(ViewState["SortExpressionEJSwitch"] + " " + ViewState["SortDirectionEJSwitch"]);
            BindEJBNASwitchGrid(
                ViewState["SortExpressionEJBNASwitch"] + " " +
                ViewState["SortDirectionEJBNASwitch"]);

        }

    }
}
