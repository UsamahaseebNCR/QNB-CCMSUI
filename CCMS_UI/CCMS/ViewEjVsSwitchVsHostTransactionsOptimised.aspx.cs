using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using ComponentArt.Web.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;

namespace CCMSUI.CCMS
{
    public partial class ViewEjVsSwitchVsHostTransactionsOptimised : BasePage
    {
        private const string DispenseSelect = @"
SELECT rt.reconciliation_transactions_id, rt.reason, rt.status,
       ej.ej_parsed_transactions_id AS ej_id, ej.trxn_datetime AS ej_dt, ej.pan AS ej_pan,
       ej.tsn AS ej_seq, ej.amount AS ej_amt, ej.account_no AS ej_acct,
       ej.status AS ej_status, ej.result AS ej_result, a.title AS ej_title,
       sw.reconciliation_switch_data_id AS sw_id, sw.transaction_date AS sw_date, sw.transaction_time AS sw_time,
       sw.card_number AS sw_pan, sw.transaction_sequence AS sw_seq, sw.transaction_amount AS sw_amt,
       sw.transaction_response AS sw_resp, sw.atm_id AS sw_atm, sw.customer_account_no AS sw_acct,
       hs.reconciliation_host_data_id AS hs_id, hs.transaction_date AS hs_date, hs.transaction_time AS hs_time,
       hs.card_number AS hs_pan, hs.transaction_sequence AS hs_seq, hs.transaction_amount AS hs_amt,
       hs.transaction_response AS hs_resp, hs.atm_id AS hs_atm, hs.customer_account_no AS hs_acct
FROM vreconciliationtransactions rt
LEFT JOIN vejparsedtransactions ej ON ej.ej_parsed_transactions_id = rt.ej_parsed_transactions_id
LEFT JOIN atm a ON a.atm_id = ej.atm_id
LEFT JOIN vreconciliationswitchdata sw ON sw.reconciliation_switch_data_id = rt.reconciliation_switch_data_id
LEFT JOIN vreconciliationhostdata   hs ON hs.reconciliation_host_data_id   = rt.reconciliation_host_data_id";

        private const string BnaSelect = @"
SELECT rt.reconciliation_transactions_id, rt.reason, rt.status,
       ej.ej_parsed_bna_transaction_id AS ej_id, ej.trxn_datetime AS ej_dt, ej.pan AS ej_pan,
       ej.seq AS ej_seq, ej.amount_authorized AS ej_amt, ej.account_no AS ej_acct,
       ej.status AS ej_status, CAST(NULL AS varchar(10)) AS ej_result, a.title AS ej_title,
       sw.reconciliation_bna_switch_data_id AS sw_id, sw.transaction_date AS sw_date, sw.transaction_time AS sw_time,
       sw.card_number AS sw_pan, sw.transaction_sequence AS sw_seq, sw.transaction_amount AS sw_amt,
       sw.transaction_response AS sw_resp, sw.atm_id AS sw_atm, sw.customer_account_no AS sw_acct,
       hs.reconciliation_bna_host_data_id AS hs_id, hs.transaction_date AS hs_date, hs.transaction_time AS hs_time,
       hs.card_number AS hs_pan, hs.transaction_sequence AS hs_seq, hs.transaction_amount AS hs_amt,
       hs.transaction_response AS hs_resp, hs.atm_id AS hs_atm, hs.customer_account_no AS hs_acct
FROM vreconciliationtransactions rt
LEFT JOIN vejparsedbnatransaction ej ON ej.ej_parsed_bna_transaction_id = rt.ej_parsed_bna_transaction_id
LEFT JOIN atm a ON a.atm_id = ej.atm_id
LEFT JOIN vreconciliationbnaswitchdata sw ON sw.reconciliation_bna_switch_data_id = rt.reconciliation_bna_switch_data_id
LEFT JOIN vreconciliationbnahostdata   hs ON hs.reconciliation_bna_host_data_id   = rt.reconciliation_bna_host_data_id";

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

        private DataTable FetchReconRows(bool bna)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandTimeout = 300;

            var where = new StringBuilder(" WHERE rt.comparison_type = @Type");
            cmd.Parameters.AddWithValue("@Type", bna ? 2 : 1);

            string status = DropDownList_TransactionStatus.SelectedValue;
            if (status != "All")
            {
                where.Append(" AND rt.status = @Status");
                cmd.Parameters.AddWithValue("@Status", status);

                string disSusOn = DropDownList_DisSusOn.SelectedValue;
                if (status != "Matched" && disSusOn != "All")
                {
                    where.Append(" AND rt.reason LIKE @Reason");
                    cmd.Parameters.AddWithValue("@Reason", "%" + disSusOn + "%");
                }
            }

            // Batch sub-filter (ATMs + date range) - sargable and parameterized
            var batch = new StringBuilder(
                "SELECT b.reconciliation_batch_id FROM reconciliation_batch b WHERE 1=1");

            string atmCsv = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);
            if (!string.IsNullOrWhiteSpace(atmCsv))
            {
                var ids = new List<int>();
                foreach (string s in atmCsv.Split(','))
                {
                    int id;
                    if (int.TryParse(s.Trim(), out id)) ids.Add(id);   // ints only => injection-safe
                }
                if (ids.Count > 0)
                    batch.Append(" AND b.atm_id IN (" + string.Join(",", ids) + ")");
            }

            DateTime from, to;
            if (!string.IsNullOrWhiteSpace(TextBox_FromDate.Text))
            {
                if (!DateTime.TryParseExact(TextBox_FromDate.Text.Trim(), "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out from))
                    throw new Exception("From Date is invalid.");
                batch.Append(" AND b.transaction_start_date >= @From");
                cmd.Parameters.Add("@From", SqlDbType.DateTime).Value = from;
            }
            if (!string.IsNullOrWhiteSpace(TextBox_ToDate.Text))
            {
                if (!DateTime.TryParseExact(TextBox_ToDate.Text.Trim(), "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out to))
                    throw new Exception("To Date is invalid.");
                batch.Append(" AND b.transaction_start_date < @To");
                cmd.Parameters.Add("@To", SqlDbType.DateTime).Value = to.AddDays(1);
            }

            where.Append(" AND rt.reconciliation_batch_id IN (" + batch + ")");
            cmd.CommandText = (bna ? BnaSelect : DispenseSelect) + where;

            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                adapter.Fill(dt);
            return dt;
        }

        private static object ParseTxnDateTime(object dateObj, object timeObj)
        {
            DateTime d;
            if (dateObj == DBNull.Value ||
                !DateTime.TryParseExact(dateObj.ToString(), "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return DBNull.Value;

            int secs;
            if (timeObj != DBNull.Value && int.TryParse(timeObj.ToString(), out secs))
                d = d.Date.AddSeconds(secs);
            return d;
        }

        private DataTable BuildGridTable(bool bna)
        {
            string seqCol = bna ? "seq" : "tsn";

            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("title");
            dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
            dtGrid.Columns.Add("PAN");
            dtGrid.Columns.Add(seqCol);
            dtGrid.Columns.Add("ej_amount", typeof(decimal));
            dtGrid.Columns.Add("switch_amount", typeof(decimal));
            dtGrid.Columns.Add("host_amount", typeof(decimal));
            dtGrid.Columns.Add("reconciliation_transactions_id");
            dtGrid.Columns.Add("reason");
            dtGrid.Columns.Add("status");
            dtGrid.Columns.Add("status_on_ej");
            dtGrid.Columns.Add("status_on_switch");
            dtGrid.Columns.Add("status_on_host");
            dtGrid.Columns.Add("accountno");

            DataTable raw = FetchReconRows(bna);
            dtGrid.BeginLoadData();

            foreach (DataRow r in raw.Rows)
            {
                bool hasEj = r["ej_id"] != DBNull.Value;
                bool hasSw = r["sw_id"] != DBNull.Value;
                bool hasHs = r["hs_id"] != DBNull.Value;
                if (!hasEj && !hasSw && !hasHs) continue;

                DataRow o = dtGrid.NewRow();
                o["reconciliation_transactions_id"] = r["reconciliation_transactions_id"];
                o["reason"] = r["reason"];
                o["status"] = r["status"];

                // Primary source for the identifying columns: EJ > Switch > Host
                if (hasEj)
                {
                    o["trxn_datetime"] = r["ej_dt"];
                    o["PAN"] = r["ej_pan"];
                    o[seqCol] = r["ej_seq"];
                    o["ej_amount"] = r["ej_amt"];
                    o["title"] = r["ej_title"];
                    o["accountno"] = r["ej_acct"];

                    if (bna)
                        o["status_on_ej"] = r["ej_status"];
                    else if (r["ej_status"] != DBNull.Value)
                        o["status_on_ej"] = GetStatusFriendlyName(Convert.ToInt32(r["ej_status"]))
                                            + "(" + r["ej_result"] + ")";
                }
                else
                {
                    string p = hasSw ? "sw_" : "hs_";
                    o["trxn_datetime"] = ParseTxnDateTime(r[p + "date"], r[p + "time"]);
                    o["PAN"] = r[p + "pan"];
                    o[seqCol] = r[p + "seq"];
                    o["title"] = r[p + "atm"];          // atm_id on switch/host holds the ATM title
                    o["accountno"] = r[p + "acct"];
                }

                if (hasSw) { o["status_on_switch"] = r["sw_resp"]; o["switch_amount"] = r["sw_amt"]; }
                if (hasHs) { o["status_on_host"] = r["hs_resp"]; o["host_amount"] = r["hs_amt"]; }

                dtGrid.Rows.Add(o);
            }
            dtGrid.EndLoadData();
            return dtGrid;
        }

        private void BindEJSwitchGrid(string orderBy)
        {
            try
            {
                DataTable dtGrid = BuildGridTable(false);
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
                DataTable dtGrid = BuildGridTable(true);
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

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            DropDownList_TransactionType.SelectedIndex = 0;
            DropDownList_TransactionStatus.SelectedValue = "All";
            DropDownList_DisSusOn.SelectedValue = "All";

            divDispense.Visible = false;
            divDeposit.Visible = false;
            trSearchResults.Visible = false;
            Literal_count.Text = "0";
        }


        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////


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


    }
}
