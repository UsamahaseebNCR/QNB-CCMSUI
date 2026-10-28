using Avanza.iSuite.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Web.UI;

namespace CCMSUI.CCMS
{
    public partial class ViewReconTransactions : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                if (!Page.IsPostBack)
                {

                    if (!CheckRight((int)Permissions.ViewReconData))
                        Response.Redirect("../admin/AccessDenied.aspx");
                    if (Request.QueryString["i"] != null)
                    {
                        batchID.Value = Request.QueryString["i"].ToString();
                    }

                    BindSummaryWithdrawalGrid();
                    BindSummaryDepositGrid();

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

        private void BindSummaryWithdrawalGrid()
        {
            try
            {
                DataTable dtGrid = new DataTable();

                dtGrid.Columns.Add("atm_Id");
                dtGrid.Columns.Add("reconciliation_Batch_Id");
                dtGrid.Columns.Add("processing_DateTime");
                dtGrid.Columns.Add("tran_Date");

                dtGrid.Columns.Add("disputed_Amount");

                dtGrid.Columns.Add("Disputed_OnUs_EJAmount");
                dtGrid.Columns.Add("Disputed_OnUs_SwitchAmount");
                dtGrid.Columns.Add("Disputed_OnUs_HostAmount");

                dtGrid.Columns.Add("Disputed_OnUs_Transactions");
                dtGrid.Columns.Add("Disputed_OnUs_EJ_Transactions");
                dtGrid.Columns.Add("Disputed_OnUs_Switch_Transactions");
                dtGrid.Columns.Add("Disputed_OnUs_Host_Transactions");

                DataTable DTBatch = GetDTData("select r.*, a.title from vReconBatchInfo r left join atm a on r.atm_id = a.atm_id where Reconciliation_Batch_Id = " + Request.QueryString["i"]);

                if (DTBatch.Rows.Count > 0)
                {
                    DataRow source = DTBatch.Rows[0];
                    DataRow dataRow = dtGrid.NewRow();

                    dataRow["atm_Id"] = source["title"];
                    dataRow["reconciliation_Batch_Id"] = source["reconciliation_Batch_Id"];
                    dataRow["processing_DateTime"] = ((DateTime)source["processing_DateTime"])
                                    .ToString("dd/MM/yyyy HH:mm:ss");
                    dataRow["tran_Date"] = ((DateTime)source["tran_Date"])
                            .ToString("dd/MM/yyyy HH:mm:ss");
                    dataRow["disputed_Amount"] = source["disputed_Amount"];
                    dataRow["Disputed_OnUs_EJAmount"] = source["Disputed_OnUs_EJAmount"];
                    dataRow["Disputed_OnUs_SwitchAmount"] = source["Disputed_OnUs_SwitchAmount"];
                    dataRow["Disputed_OnUs_HostAmount"] = source["Disputed_OnUs_HostAmount"];
                    dataRow["Disputed_OnUs_Transactions"] = source["Disputed_OnUs_Transactions"];
                    dataRow["Disputed_OnUs_EJ_Transactions"] = source["Disputed_OnUs_EJ_Transactions"];
                    dataRow["Disputed_OnUs_Switch_Transactions"] = source["Disputed_OnUs_Switch_Transactions"];
                    dataRow["Disputed_OnUs_Host_Transactions"] = source["Disputed_OnUs_Host_Transactions"];

                    dtGrid.Rows.Add(dataRow);
                }

                RepeaterWithdrawal.DataSource = dtGrid.DefaultView;
                RepeaterWithdrawal.DataBind();
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("BindSummaryWithdrawalGrid");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
            }
        }

        private void BindSummaryDepositGrid()
        {
            try
            {
                DataTable dtGrid = new DataTable();

                dtGrid.Columns.Add("atm_Id");
                dtGrid.Columns.Add("reconciliation_Batch_Id");
                dtGrid.Columns.Add("processing_DateTime");
                dtGrid.Columns.Add("tran_Date");

                dtGrid.Columns.Add("Disputed_Amount_BNA");

                dtGrid.Columns.Add("Disputed_BNA_EJAmount");
                dtGrid.Columns.Add("Disputed_BNA_SwitchAmount");
                dtGrid.Columns.Add("Disputed_BNA_HostAmount");

                dtGrid.Columns.Add("Disputed_BNA_Transactions");
                dtGrid.Columns.Add("Disputed_BNA_EJ_Transactions");
                dtGrid.Columns.Add("Disputed_BNA_Switch_Transactions");
                dtGrid.Columns.Add("Disputed_BNA_Host_Transactions");

                DataTable DTBatch = GetDTData("select r.*, a.title from vReconBatchInfo r left join atm a on r.atm_id = a.atm_id where Reconciliation_Batch_Id = " + Request.QueryString["i"]);

                if (DTBatch.Rows.Count > 0)
                {
                    DataRow source = DTBatch.Rows[0];
                    DataRow dataRow = dtGrid.NewRow();

                    dataRow["atm_Id"] = source["title"];
                    dataRow["reconciliation_Batch_Id"] = source["reconciliation_Batch_Id"];
                    dataRow["processing_DateTime"] =
                        ((DateTime)source["processing_DateTime"])
                        .ToString("dd/MM/yyyy HH:mm:ss");

                    dataRow["tran_Date"] =
                        ((DateTime)source["tran_Date"])
                        .ToString("dd/MM/yyyy HH:mm:ss");

                    dataRow["Disputed_Amount_BNA"] = source["Disputed_Amount_BNA"];

                    dataRow["Disputed_BNA_EJAmount"] = source["Disputed_BNA_EJAmount"];
                    dataRow["Disputed_BNA_SwitchAmount"] = source["Disputed_BNA_SwitchAmount"];
                    dataRow["Disputed_BNA_HostAmount"] = source["Disputed_BNA_HostAmount"];

                    dataRow["Disputed_BNA_Transactions"] = source["Disputed_BNA_Transactions"];
                    dataRow["Disputed_BNA_EJ_Transactions"] = source["Disputed_BNA_EJ_Transactions"];
                    dataRow["Disputed_BNA_Switch_Transactions"] = source["Disputed_BNA_Switch_Transactions"];
                    dataRow["Disputed_BNA_Host_Transactions"] = source["Disputed_BNA_Host_Transactions"];

                    dtGrid.Rows.Add(dataRow);
                }

                RepeaterDeposit.DataSource = dtGrid.DefaultView;
                RepeaterDeposit.DataBind();
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("BindSummaryDepositGrid");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
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

        // ---------------------------------------------------------------------
        // Display-only helpers used by the dashboard markup (.aspx).
        // They never touch the database and never throw: if a value cannot be
        // parsed, the raw text is shown instead.
        // ---------------------------------------------------------------------

        private const string EmptyValue = "\u2014"; // em dash

        /// <summary>Amounts: thousands separators, 2 decimals (e.g. 1,250,000.00).</summary>
        protected string FormatAmount(object value)
        {
            return FormatNumber(value, "N2");
        }

        /// <summary>Transaction counts: thousands separators, no decimals (e.g. 1,250).</summary>
        protected string FormatCount(object value)
        {
            return FormatNumber(value, "N0");
        }

        /// <summary>
        /// tran_Date is stored as "dd/MM/yyyy HH:mm:ss" text by the bind methods;
        /// the tile shows the date part only (matches the old {0:dd/MM/yyyy} intent).
        /// </summary>
        protected string FormatTranDate(object value)
        {
            if (value == null || value == DBNull.Value)
                return EmptyValue;

            string text = Convert.ToString(value);
            DateTime parsed;
            if (DateTime.TryParseExact(text, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out parsed))
                return parsed.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            return text;
        }

        private static string FormatNumber(object value, string format)
        {
            if (value == null || value == DBNull.Value)
                return EmptyValue;

            string text = Convert.ToString(value);
            if (string.IsNullOrEmpty(text))
                return EmptyValue;

            decimal number;
            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out number))
                return number.ToString(format, CultureInfo.CurrentCulture);

            return text;
        }
    }
}
