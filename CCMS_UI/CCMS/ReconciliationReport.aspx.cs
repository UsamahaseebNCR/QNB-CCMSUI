using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data;
using System.Data.SqlClient;
//using Avanza.iSuite.DAL;
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class ReconciliationReport : BasePage
    {
        string[] dateFormats = new string[] { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy h:mm:ss tt", "dd/M/yyyy h:mm:ss tt",
                                              "d/MM/yyyy HH:mm:ss", "d/MM/yyyy h:mm:ss tt", "d/M/yyyy h:mm:ss tt",
                                              "MM/dd/yyyy HH:mm:ss","MM/dd/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss tt",
                                              "M/dd/yyyy HH:mm:ss","M/dd/yyyy h:mm:ss tt","M/dd/yyyy h:mm:ss tt"};

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ReconciliationReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            }
        }

        private string Filter()
        {
            StringBuilder builder = new StringBuilder();
            //if (DropDownList_Status.SelectedIndex > 0)
            builder.Append("and status = '" + DropDownList_Status.SelectedItem.Text + "'");

            /*if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                            dataRow["mode"] = "Auto";
                        else
                        {
                            if (dr["is_reconciled_manually"] != DBNull.Value)
                            {
                                dataRow["mode"] = "Manual";
                            }
                        }*/
            //if (DropDownList_Mode.SelectedIndex == 1)
            //    builder.Append("and is_reconciled_manually is null and is_reconciled =1");
            //else if (DropDownList_Mode.SelectedIndex == 2)
            //    builder.Append("and is_reconciled_manually is not null");

            return builder.ToString();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            DataSet ds = null;

            try
            {
                //DataSet ds = new dsReconciliation();
                //cmd = ConnectionFactory.GetNewCommand(false);
                //cmd.CommandText = @"select * from reconciliation_transactions where generated_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and generated_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) ";
                //DataTable dt = new DataTable();
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.Fill(dt);
                //DataRow newRow = null;
                //foreach (DataRow dr in dt.Rows)
                //{

                //    dataCaptured = false;
                //    if (dr["reconciliation_switch_data_id"].ToString() != "0")
                //    {

                //        if (!dataCaptured)
                //        {
                //            ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                //            newRow = ds.Tables[0].NewRow();
                //            newRow["title"] = switchData.AtmId;

                //            string date = switchData.TransactionDate;
                //            string time = switchData.TransactionTime;
                //            string[] dateParts = date.Split('/');
                //            string[] timeParts = time.Split(':');
                //            DateTime dateTime = new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]), int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));

                //            newRow["trxn_datetime"] = dateTime;
                //            newRow["tsn"] = switchData.TransactionSequence;
                //            newRow["amount"] = switchData.TransactionAmount;
                //            newRow["status"] = dr["status"];
                //            if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                //                newRow["mode"] = "Auto";
                //            else
                //            {
                //                if (dr["is_reconciled_manually"] != DBNull.Value)
                //                {
                //                    newRow["mode"] = "Manual";
                //                }
                //            }
                //            newRow["reason"] = dr["reason"];
                //            dataCaptured = true;
                //        }

                //        newRow["onSwitch"] = "True";

                //        //reportDt.Rows.Add(newRow);
                //    }
                //    if (dr["reconciliation_host_data_id"].ToString() != "0")
                //    {
                //        if (!dataCaptured)
                //        {
                //            ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                //            newRow = ds.Tables[0].NewRow();
                //            newRow["title"] = hostData.AtmId;

                //            string date = hostData.TransactionDate;
                //            string time = hostData.TransactionTime;
                //            string[] dateParts = date.Split('/');
                //            string[] timeParts = time.Split(':');
                //            DateTime dateTime = new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]), int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));

                //            newRow["trxn_datetime"] = dateTime;
                //            newRow["tsn"] = hostData.TransactionSequence;
                //            newRow["amount"] = hostData.TransactionAmount;
                //            newRow["status"] = dr["status"];

                //            if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                //                newRow["mode"] = "Auto";
                //            else
                //            {
                //                if (dr["is_reconciled_manually"] != DBNull.Value)
                //                {
                //                    newRow["mode"] = "Manual";
                //                }
                //            }
                //            newRow["reason"] = dr["reason"];
                //            dataCaptured = true;
                //        }

                //        newRow["onHost"] = "True";


                //        //reportDt.Rows.Add(newRow);
                //    }

                //    if (dr["ej_parsed_transactions_id"].ToString() != "0")
                //    {
                //        if (!dataCaptured)
                //        {
                //            EjParsedTransactions ejTrxn = EjParsedTransactions.LoadEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                //            newRow = ds.Tables[0].NewRow();
                //            newRow["title"] = Atm.LoadAtmByPk(ejTrxn.AtmId.Value).Title;
                //            newRow["trxn_datetime"] = ejTrxn.TrxnDatetime;
                //            newRow["tsn"] = ejTrxn.Tsn;
                //            newRow["amount"] = ejTrxn.Amount;
                //            newRow["status"] = dr["status"];
                //            if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                //                newRow["mode"] = "Auto";
                //            else
                //            {
                //                if (dr["is_reconciled_manually"] != DBNull.Value)
                //                {
                //                    newRow["mode"] = "Manual";
                //                }
                //            }
                //            newRow["reason"] = dr["reason"];
                //            dataCaptured = true;
                //        }

                //        newRow["onEJ"] = "True";

                //        //reportDt.Rows.Add(newRow);
                //    }
                //    if (dr["parsed_transactions_id"].ToString() != "0")
                //    {
                //        if (!dataCaptured)
                //        {
                //            ParsedTransaction ccmsTrxn = ParsedTransaction.LoadParsedTransactionByPk(int.Parse(dr["parsed_transactions_id"].ToString()));
                //            newRow = ds.Tables[0].NewRow();
                //            newRow["title"] = Atm.LoadAtmByPk(ccmsTrxn.AtmId).Title;
                //            newRow["trxn_datetime"] = ccmsTrxn.TrxnDatetime;
                //            newRow["tsn"] = ccmsTrxn.Tsn;
                //            newRow["amount"] = ccmsTrxn.Amount;
                //            newRow["status"] = dr["status"];
                //            if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                //                newRow["mode"] = "Auto";
                //            else
                //            {
                //                if (dr["is_reconciled_manually"] != DBNull.Value)
                //                {
                //                    newRow["mode"] = "Manual";
                //                }
                //            }
                //            newRow["reason"] = dr["reason"];
                //            dataCaptured = true;
                //        }

                //        newRow["onCCMS"] = "True";


                //    }
                //    ds.Tables[0].Rows.Add(newRow);
                //}



                //adapter.Fill(ds.Tables["DataTable1"]);
                //SplitAlertMessageIntoColumns(ds.Tables["DataTable1"]);

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];

                //if (RadioButtonList_ReportType.SelectedIndex == 4)
                //    ds = GetDataSetEJSwitchGrid();
                //else if (RadioButtonList_ReportType.SelectedIndex == 5)
                //    ds = GetDataSetSwitchHostGrid();
                //else
                ds = GetDataSetReconBatchGrid();

                ////else if (RadioButtonList_ReportType.SelectedIndex == 2)
                ////    ds = GetDataSetCCMSSwitchGrid();
                ////else if (RadioButtonList_ReportType.SelectedIndex == 3)
                ////    ds = GetDataSetCCMSEjGrid();

                string reportName = "ReconReport.rpt";
                //if (RadioButtonList_ReportType.SelectedIndex == 0)
                //    reportName = "ReconReport_Switch.rpt";
                //else if (RadioButtonList_ReportType.SelectedIndex == 1)
                //    reportName = "ReconReport_Host.rpt";
                //else if (RadioButtonList_ReportType.SelectedIndex == 2)
                //    reportName = "ReconReport_EJ.rpt";
                //else if (RadioButtonList_ReportType.SelectedIndex == 3)
                reportName = "ReconReport_BatchSummary.rpt";

                //if (RadioButtonList_ReportType.SelectedIndex <= 3)
                //{
                if (ds.Tables["DataTable_ReconBatch"].Rows.Count > 0)
                {
                    string BatchIDs = "";
                    foreach (DataRow row in ds.Tables["DataTable_ReconBatch"].Rows)
                    {
                        if (BatchIDs != "")
                            BatchIDs += ", ";

                        BatchIDs += row["Reconciliation_Batch_Id"].ToString();
                    }

                    DataTable dtDispense = BindEJSwitchGrid("", BatchIDs);
                    foreach (DataRow dr in dtDispense.Rows)
                    {
                        if (dr["onus"].ToString() == "1")
                        {//onus
                            DataRow dataRow = ds.Tables[1].NewRow();
                            dataRow["title"] = dr["title"];
                            dataRow["trxn_datetime"] = dr["trxn_datetime"];
                            dataRow["PAN"] = dr["PAN"];
                            dataRow["tsn"] = dr["tsn"];
                            dataRow["amount"] = dr["amount"];
                            dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                            dataRow["reason"] = dr["reason"];
                            dataRow["status"] = dr["status"];
                            dataRow["status_on_ej"] = dr["status_on_ej"];
                            dataRow["status_on_switch"] = dr["status_on_switch"];
                            dataRow["status_on_host"] = dr["status_on_host"];
                            dataRow["accountno"] = dr["accountno"];
                            dataRow["transaction_no"] = dr["transaction_no"];
                            dataRow["Organization"] = "GBK";
                            dataRow["action"] = dr["action"];
                            dataRow["card_issuer"] = dr["card_issuer"];
                            dataRow["card_acquirer"] = dr["card_acquirer"];

                            ds.Tables[1].Rows.Add(dataRow);
                        }
                        else
                        {
                            if (dr["onus"].ToString() == "999999" || dr["PAN"].ToString().StartsWith("999999"))
                            {//cardless

                                DataRow dataRow = ds.Tables[4].NewRow();
                                dataRow["title"] = dr["title"];
                                dataRow["trxn_datetime"] = dr["trxn_datetime"];
                                dataRow["PAN"] = dr["PAN"];
                                dataRow["tsn"] = dr["tsn"];
                                dataRow["amount"] = dr["amount"];
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                dataRow["reason"] = dr["reason"];
                                dataRow["status"] = dr["status"];
                                dataRow["status_on_ej"] = dr["status_on_ej"];
                                dataRow["status_on_switch"] = dr["status_on_switch"];
                                dataRow["status_on_host"] = dr["status_on_host"];
                                dataRow["accountno"] = dr["accountno"];
                                dataRow["transaction_no"] = dr["transaction_no"];
                                dataRow["Organization"] = "GBK";
                                dataRow["action"] = dr["action"];
                                dataRow["card_issuer"] = dr["card_issuer"];
                                dataRow["card_acquirer"] = dr["card_acquirer"];

                                ds.Tables[4].Rows.Add(dataRow);
                            }
                            else
                            {//offus
                                DataRow dataRow = ds.Tables[3].NewRow();
                                dataRow["title"] = dr["title"];
                                dataRow["trxn_datetime"] = dr["trxn_datetime"];
                                dataRow["PAN"] = dr["PAN"];
                                dataRow["tsn"] = dr["tsn"];
                                dataRow["amount"] = dr["amount"];
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                dataRow["reason"] = dr["reason"];
                                dataRow["status"] = dr["status"];
                                dataRow["status_on_ej"] = dr["status_on_ej"];
                                dataRow["status_on_switch"] = dr["status_on_switch"];
                                dataRow["status_on_host"] = dr["status_on_host"];
                                dataRow["accountno"] = dr["accountno"];
                                dataRow["transaction_no"] = dr["transaction_no"];
                                dataRow["Organization"] = "GBK";
                                dataRow["action"] = dr["action"];
                                dataRow["card_issuer"] = dr["card_issuer"];
                                dataRow["card_acquirer"] = dr["card_acquirer"];

                                ds.Tables[3].Rows.Add(dataRow);
                            }
                        }


                    }

                    DataTable dtDeposit = BindEJBNASwitchGrid("", BatchIDs);
                    foreach (DataRow dr in dtDeposit.Rows)
                    {
                        DataRow dataRow = ds.Tables[2].NewRow();

                        dataRow["title"] = dr["title"];
                        dataRow["trxn_datetime"] = dr["trxn_datetime"];
                        dataRow["PAN"] = dr["PAN"];
                        dataRow["tsn"] = dr["tsn"];
                        dataRow["amount"] = dr["amount"];
                        dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                        dataRow["reason"] = dr["reason"];
                        dataRow["status"] = dr["status"];
                        dataRow["status_on_ej"] = dr["status_on_ej"];
                        dataRow["status_on_switch"] = dr["status_on_switch"];
                        dataRow["status_on_host"] = dr["status_on_host"];
                        dataRow["accountno"] = dr["accountno"];
                        dataRow["transaction_no"] = dr["transaction_no"];
                        dataRow["Organization"] = "GBK";
                        dataRow["action"] = dr["action"];
                        dataRow["card_issuer"] = dr["card_issuer"];
                        dataRow["card_acquirer"] = dr["card_acquirer"];

                        ds.Tables[2].Rows.Add(dataRow);
                    }


                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=" + reportName + "&ReportTitle=" + RadioButtonList_ReportType.SelectedItem.Text +
                        "&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) +
                        "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable_ReconBatch"].Rows.Count;
                    Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
                //}
                //else
                //{
                //    if (ds.Tables["DataTable1"].Rows.Count > 0)
                //    {
                //        Session[SessionVars.dtReportDataSet.ToString()] = ds;
                //        string URL = "ReportPopup.aspx?Report=" + reportName + "&ReportTitle=" + RadioButtonList_ReportType.SelectedItem.Text +
                //            "&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) +
                //            "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
                //        Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                //    }
                //    else
                //        Literal_ok.Text = "<li>No record(s) found</li>";
                //}
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
        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            //            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            //            ReportSchedule reportSchedule = null;
            //            SqlCommand cmd = null;
            //            SqlTransaction trxn = null;
            //            int reportAge = CheckBox_Current.Checked ? 0 : 1;
            //            try
            //            {
            //                if (Session["isOrgSelected"] != null)
            //                {
            //                    if (bool.Parse(Session["isOrgSelected"].ToString()))
            //                    {
            //                        cmd = ConnectionFactory.GetNewCommand(true);
            //                        trxn = cmd.Connection.BeginTransaction();


            //                        string orgID = Session["selectedOrgID"].ToString();
            //                        ConnectionFactory.ExecuteScalar(@"delete report_generation_schedule
            //                                                          where report_schedule_id in
            //                                                          (select  report_schedule_id 
            //                                                          from report_schedule 
            //                                                          where report_name='LowBalanceReport' and organization_id = " + orgID + "); " +

            //                                                         " delete report_schedule where report_name='LowBalanceReport' and report_data_age=" + reportAge + " and organization_id = " + orgID, trxn);

            //                        reportSchedule = new ReportSchedule();
            //                        reportSchedule.ReportName = "LowBalanceReport";
            //                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"lowbalancereport.rpt");
            //                        reportSchedule.ReportReceipients = "";
            //                        reportSchedule.ReportTempPath = @"c:\ccmsLiveData\temp\";
            //                        reportSchedule.RetryCount = 5;
            //                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
            //                        reportSchedule.ReportFriendlyName = "Low Balance Report";
            //                        reportSchedule.MinutesToScheduleAgain = 1440;
            //                        reportSchedule.ReportExportType = 3;
            //                        reportSchedule.ReportDataAge = reportAge;
            //                        reportSchedule.ScheduleType = false;
            //                        reportSchedule.OrganizationId = int.Parse(orgID);
            //                        reportSchedule.IsEjEnabled = false;
            //                        reportSchedule.Save(trxn.Connection, trxn);
            //                        trxn.Commit();
            //                        Literal_ok.Text = "Schedule added successfully";
            //                        Literal_error.Text = "";
            //                    }
            //                    else
            //                        Literal_error.Text = "Please select organization from treeview.";
            //                }
            //                else
            //                    Literal_error.Text = "Please select organization from treeview.";
            //            }

            //            catch (Exception ex)
            //            {
            //                if (trxn != null)
            //                    trxn.Rollback();
            //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);

            //            }
            //            finally
            //            {
            //                task.EndTask();
            //                if (cmd != null)
            //                    if (cmd.Connection != null)
            //                        cmd.Connection.Close();
            //            }
        }
        //private DataSet GetDataSetCCMSEjGrid()
        //{
        //    DataSet ds = new dsReconciliation();
        //    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
        //    cmd.CommandText = "select * from reconciliation_transactions where generated_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and generated_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and   comparison_type=4  " + Filter();
        //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //    DataTable DT = new DataTable();
        //    adapter.Fill(DT);
        //    foreach (DataRow dr in DT.Rows)
        //    {
        //        if (dr["ej_parsed_transactions_id"].ToString() == "0")
        //        {
        //            ParsedTransaction trxn = ParsedTransaction.LoadParsedTransactionByPk(int.Parse(dr["parsed_transactions_id"].ToString()));
        //            if (trxn != null)
        //            {
        //                DataRow dataRow = ds.Tables[0].NewRow();
        //                dataRow["trxn_datetime"] = trxn.TrxnDatetime;
        //                dataRow["PAN"] = trxn.Pan;
        //                dataRow["tsn"] = trxn.Tsn;
        //                dataRow["amount"] = trxn.Amount;
        //                dataRow["title"] = Atm.LoadAtmByPk(trxn.AtmId).Title;
        //                //dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
        //                //dataRow["is_on_ej"] = true;
        //                dataRow["reason"] = dr["reason"];
        //                dataRow["status"] = dr["status"];
        //                if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
        //                    dataRow["mode"] = "Auto";
        //                else
        //                {
        //                    if (dr["is_reconciled_manually"] != DBNull.Value)
        //                    {
        //                        dataRow["mode"] = "Manual";
        //                    }
        //                }
        //                ds.Tables[0].Rows.Add(dataRow);
        //            }
        //        }
        //        else
        //        {
        //            EjParsedTransactions ejTrxn = EjParsedTransactions.LoadEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
        //            if (ejTrxn != null)
        //            {
        //                DataRow dataRow = ds.Tables[0].NewRow();
        //                dataRow["trxn_datetime"] = ejTrxn.TrxnDatetime;
        //                dataRow["PAN"] = ejTrxn.Pan;
        //                dataRow["tsn"] = ejTrxn.Tsn;
        //                dataRow["amount"] = ejTrxn.Amount;
        //                //dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
        //                dataRow["title"] = Atm.LoadAtmByPk(ejTrxn.AtmId.Value).Title;
        //                //Atm atm = Atm.LoadAtm("title = '" + ejTrxn.AtmId + "'");
        //                //if (atm != null)
        //                //    dataRow["title"] = atm.Title;
        //                //dataRow["is_on_switch"] = true;
        //                //if (int.Parse(dr["ej_parsed_transactions_id"].ToString()) > 0)
        //                //    dataRow["is_on_ej"] = true;
        //                dataRow["reason"] = dr["reason"];
        //                if (dr["reason"] != DBNull.Value)
        //                {
        //                    if (dr["reason"].ToString().ToLower().Contains("mismatch"))
        //                    {
        //                        ParsedTransaction trxn = ParsedTransaction.LoadParsedTransactionByPk(int.Parse(dr["parsed_transactions_id"].ToString()));
        //                        dataRow["amount"] = trxn.Amount;
        //                    }
        //                }
        //                dataRow["status"] = dr["status"];
        //                if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
        //                    dataRow["mode"] = "Auto";
        //                else
        //                {
        //                    if (dr["is_reconciled_manually"] != DBNull.Value)
        //                    {
        //                        dataRow["mode"] = "Manual";
        //                    }
        //                }
        //                ds.Tables[0].Rows.Add(dataRow);
        //            }
        //        }
        //    }

        //    return ds;
        //}

        //private DataSet GetDataSetCCMSSwitchGrid()
        //{

        //    DataSet ds = new dsReconciliation();
        //    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
        //    cmd.CommandText = "select * from reconciliation_transactions where generated_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and generated_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and   comparison_type=3  " + Filter();
        //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //    DataTable DT = new DataTable();
        //    adapter.Fill(DT);
        //    foreach (DataRow dr in DT.Rows)
        //    {
        //        if (dr["parsed_transactions_id"].ToString() != "0")
        //        {
        //            ParsedTransaction trxn = ParsedTransaction.LoadParsedTransactionByPk(int.Parse(dr["parsed_transactions_id"].ToString()));
        //            if (trxn != null)
        //            {
        //                DataRow dataRow = ds.Tables[0].NewRow();
        //                dataRow["trxn_datetime"] = trxn.TrxnDatetime;
        //                dataRow["PAN"] = trxn.Pan;
        //                dataRow["tsn"] = trxn.Tsn;
        //                dataRow["amount"] = trxn.Amount;
        //                dataRow["title"] = Atm.LoadAtmByPk(trxn.AtmId).Title;
        //                //dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
        //                //dataRow["is_on_ej"] = true;
        //                dataRow["reason"] = dr["reason"];
        //                dataRow["status"] = dr["status"];
        //                if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
        //                    dataRow["mode"] = "Auto";
        //                else
        //                {
        //                    if (dr["is_reconciled_manually"] != DBNull.Value)
        //                    {
        //                        dataRow["mode"] = "Manual";
        //                    }
        //                }
        //                ds.Tables[0].Rows.Add(dataRow);
        //            }
        //        }
        //        else
        //        {
        //            ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
        //            if (switchData != null)
        //            {
        //                DataRow dataRow = ds.Tables[0].NewRow();
        //                dataRow["trxn_datetime"] = switchData.TransactionDate + " " + switchData.TransactionTime;
        //                dataRow["PAN"] = switchData.CardNumber;
        //                dataRow["tsn"] = switchData.TransactionSequence;
        //                dataRow["amount"] = switchData.TransactionAmount;
        //                //dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
        //                Atm atm = Atm.LoadAtm("title = '" + switchData.AtmId + "'");
        //                if (atm != null)
        //                    dataRow["title"] = atm.Title;
        //                //dataRow["is_on_switch"] = true;
        //                //if (int.Parse(dr["ej_parsed_transactions_id"].ToString()) > 0)
        //                //    dataRow["is_on_ej"] = true;
        //                dataRow["reason"] = dr["reason"];
        //                if (dr["reason"] != DBNull.Value)
        //                {
        //                    if (dr["reason"].ToString().ToLower().Contains("mismatch"))
        //                    {
        //                        ParsedTransaction trxn = ParsedTransaction.LoadParsedTransactionByPk(int.Parse(dr["parsed_transactions_id"].ToString()));
        //                        dataRow["amount"] = trxn.Amount;
        //                    }
        //                }
        //                dataRow["status"] = dr["status"];
        //                if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
        //                    dataRow["mode"] = "Auto";
        //                else
        //                {
        //                    if (dr["is_reconciled_manually"] != DBNull.Value)
        //                    {
        //                        dataRow["mode"] = "Manual";
        //                    }
        //                }
        //                ds.Tables[0].Rows.Add(dataRow);
        //            }
        //        }
        //    }

        //    return ds;
        //}


        private DataSet GetDataSetEJSwitchGrid()
        {
            DataSet ds = new dsReconciliation();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select * from reconciliation_transactions where generated_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and generated_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and   comparison_type=2 " + Filter();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable DT = new DataTable();
            adapter.Fill(DT);
            foreach (DataRow dr in DT.Rows)
            {
                if (dr["ej_parsed_transactions_id"].ToString() != "0")
                {
                    EjParsedTransactions ejTrxn = EjParsedTransactions.LoadEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                    if (ejTrxn != null)
                    {
                        DataRow dataRow = ds.Tables[0].NewRow();
                        dataRow["trxn_datetime"] = ejTrxn.TrxnDatetime;
                        dataRow["PAN"] = ejTrxn.Pan;
                        dataRow["tsn"] = ejTrxn.Tsn;
                        dataRow["amount"] = ejTrxn.Amount;
                        dataRow["title"] = Atm.LoadAtmByPk(ejTrxn.AtmId.Value).Title;
                        //dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                        //dataRow["is_on_ej"] = true;
                        dataRow["reason"] = dr["reason"];
                        dataRow["status"] = dr["status"];
                        if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                            dataRow["mode"] = "Auto";
                        else
                        {
                            if (dr["is_reconciled_manually"] != DBNull.Value)
                            {
                                dataRow["mode"] = "Manual";
                            }
                        }
                        ds.Tables[0].Rows.Add(dataRow);
                    }
                }
                else
                {
                    ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                    if (switchData != null)
                    {
                        DataRow dataRow = ds.Tables[0].NewRow();
                        var dateParts = DateTime.ParseExact(switchData.TransactionDate, "yyyyMMdd", null); ;
                        string[] timeParts = switchData.TransactionTime.Split(':');
                        DateTime dateTime = new DateTime(dateParts.Year, dateParts.Month, dateParts.Day, int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse("00"));
                        dataRow["trxn_datetime"] = dateTime;
                        dataRow["PAN"] = switchData.CardNumber;
                        dataRow["tsn"] = switchData.TransactionSequence;
                        dataRow["amount"] = switchData.TransactionAmount;
                        //dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                        Atm atm = Atm.LoadAtm("title = '" + switchData.AtmId + "'");
                        if (atm != null)
                            dataRow["title"] = atm.Title;
                        //dataRow["is_on_switch"] = true;
                        //if (int.Parse(dr["ej_parsed_transactions_id"].ToString()) > 0)
                        //    dataRow["is_on_ej"] = true;
                        dataRow["reason"] = dr["reason"];
                        if (dr["reason"] != DBNull.Value)
                        {
                            if (dr["reason"].ToString().ToLower().Contains("mismatch"))
                            {
                                EjParsedTransactions ejTrxn = EjParsedTransactions.LoadEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                                dataRow["amount"] = ejTrxn.Amount;
                            }
                        }
                        dataRow["status"] = dr["status"];
                        if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                            dataRow["mode"] = "Auto";
                        else
                        {
                            if (dr["is_reconciled_manually"] != DBNull.Value)
                            {
                                dataRow["mode"] = "Manual";
                            }
                        }
                        ds.Tables[0].Rows.Add(dataRow);
                    }
                }

            }
            return ds;
        }

        private DataSet GetDataSetSwitchHostGrid()
        {
            DataSet ds = new dsReconciliation();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select * from reconciliation_transactions where generated_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and generated_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and   comparison_type=1 " + Filter();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable DT = new DataTable();
            adapter.Fill(DT);
            foreach (DataRow dr in DT.Rows)
            {
                if (dr["reconciliation_switch_data_id"].ToString() == "0")
                {
                    ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_host_data_id"].ToString()));
                    if (hostData != null)
                    {
                        DataRow dataRow = ds.Tables[0].NewRow();
                        var dateParts = DateTime.ParseExact(hostData.TransactionDate, "yyyyMMdd", null); ;
                        string[] timeParts = hostData.TransactionTime.Split(':');
                        DateTime dateTime = new DateTime(dateParts.Year, dateParts.Month, dateParts.Day, int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse("00"));
                        dataRow["trxn_datetime"] = dateTime;
                        dataRow["PAN"] = hostData.CardNumber;
                        dataRow["tsn"] = hostData.TransactionSequence;
                        dataRow["amount"] = hostData.TransactionAmount;
                        Atm atm = Atm.LoadAtm("title = '" + hostData.AtmId + "'");
                        if (atm != null)
                            dataRow["title"] = atm.Title;
                        //dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                        //dataRow["is_on_host"] = true;
                        dataRow["reason"] = dr["reason"];
                        dataRow["status"] = dr["status"];
                        if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                            dataRow["mode"] = "Auto";
                        else
                        {
                            if (dr["is_reconciled_manually"] != DBNull.Value)
                            {
                                dataRow["mode"] = "Manual";
                            }
                        }
                        ds.Tables[0].Rows.Add(dataRow);
                    }
                }
                else
                {
                    ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                    if (switchData != null)
                    {
                        DataRow dataRow = ds.Tables[0].NewRow();
                        var dateParts = DateTime.ParseExact(switchData.TransactionDate, "yyyyMMdd", null); ;
                        string[] timeParts = switchData.TransactionTime.Split(':');
                        DateTime dateTime = new DateTime(dateParts.Year, dateParts.Month, dateParts.Day, int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse("00"));
                        dataRow["trxn_datetime"] = dateTime;
                        dataRow["PAN"] = switchData.CardNumber;
                        string _tsn = switchData.TransactionSequence.Substring(switchData.TransactionSequence.Length - 5);
                        dataRow["tsn"] = _tsn;
                        //dataRow["tsn"] = switchData.TransactionSequence;
                        dataRow["amount"] = switchData.TransactionAmount;
                        Atm atm = Atm.LoadAtm("title = '" + switchData.AtmId + "'");
                        if (atm != null)
                            dataRow["title"] = atm.Title;
                        //dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                        //dataRow["is_on_switch"] = true;
                        //if (int.Parse(dr["reconciliation_host_data_id"].ToString()) > 0)
                        //    dataRow["is_on_host"] = true;
                        dataRow["reason"] = dr["reason"];
                        if (dr["reason"] != DBNull.Value)
                        {
                            if (dr["reason"].ToString().ToLower().Contains("mismatch"))
                            {
                                ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_host_data_id"].ToString()));
                                dataRow["amount"] = hostData.TransactionAmount;
                            }
                        }
                        dataRow["status"] = dr["status"];
                        if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
                            dataRow["mode"] = "Auto";
                        else
                        {
                            if (dr["is_reconciled_manually"] != DBNull.Value)
                            {
                                dataRow["mode"] = "Manual";
                            }
                        }
                        ds.Tables[0].Rows.Add(dataRow);
                    }
                }

            }
            return ds;
        }

        private DataSet GetDataSetReconBatchGrid()
        {
            DataSet ds = new dsReconciliationBatch();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "[GetReconciliationBatchinfo] '" + Utility.GetDateTime(TextBox_FromDate.Text) + "', '" + Utility.GetDateTime(TextBox_ToDate.Text) + "' ;";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable DT = new DataTable();
            adapter.Fill(DT);
            foreach (DataRow dr in DT.Rows)
            {

                DataRow dataRow = ds.Tables[0].NewRow();

                dataRow["title"] = dr["title"];
                dataRow["Id"] = dr["Id"];
                dataRow["Reconciliation_Batch_Id"] = dr["Reconciliation_Batch_Id"];
                dataRow["Switch_On_Us_Transactions"] = dr["Switch_On_Us_Transactions"];
                dataRow["Host_On_Us_Transactions"] = dr["Host_On_Us_Transactions"];
                dataRow["EJ_On_Us_Transactions"] = dr["EJ_On_Us_Transactions"];
                dataRow["Switch_Off_Us_Transactions"] = dr["Switch_Off_Us_Transactions"];
                dataRow["Host_Off_Us_Transactions"] = dr["Host_Off_Us_Transactions"];
                dataRow["EJ_Off_Us_Transactions"] = dr["EJ_Off_Us_Transactions"];
                dataRow["Switch_Cardless_Transactions"] = dr["Switch_Cardless_Transactions"];
                dataRow["EJ_Cardless_Transactions"] = dr["EJ_Cardless_Transactions"];
                dataRow["Host_Cardless_Transactions"] = dr["Host_Cardless_Transactions"];
                dataRow["Disputed_Amount"] = dr["Disputed_Amount"];
                dataRow["Switch_On_Us_BNA_Transactions"] = dr["Switch_On_Us_BNA_Transactions"];
                dataRow["Host_On_Us_BNA_Transactions"] = dr["Host_On_Us_BNA_Transactions"];
                dataRow["EJ_On_Us_BNA_Transactions"] = dr["EJ_On_Us_BNA_Transactions"];
                dataRow["Atm_Id"] = dr["Atm_Id"];
                dataRow["Processing_DateTime"] = dr["Processing_DateTime"];
                dataRow["Switch_Amount_OnUs"] = dr["Switch_Amount_OnUs"];
                dataRow["Host_Amount_OnUs"] = dr["Host_Amount_OnUs"];
                dataRow["EJ_Amount_OnUs"] = dr["EJ_Amount_OnUs"];
                dataRow["Switch_Amount_OffUs"] = dr["Switch_Amount_OffUs"];
                dataRow["EJ_Amount_OffUs"] = dr["EJ_Amount_OffUs"];
                dataRow["Host_Amount_OffUs"] = dr["Host_Amount_OffUs"];
                dataRow["Tran_Date"] = dr["Tran_Date"];
                dataRow["Switch_Amount_Cardless"] = dr["Switch_Amount_Cardless"];
                dataRow["EJ_Amount_Cardless"] = dr["EJ_Amount_Cardless"];
                dataRow["Host_Amount_Cardless"] = dr["Host_Amount_Cardless"];
                dataRow["Switch_Amount_BNA"] = dr["Switch_Amount_BNA"];
                dataRow["Host_Amount_BNA"] = dr["Host_Amount_BNA"];
                dataRow["EJ_Amount_BNA"] = dr["EJ_Amount_BNA"];
                dataRow["Disputed_Amount_BNA"] = dr["Disputed_Amount_BNA"];
                dataRow["Disputed_Amount_OffUs"] = dr["Disputed_Amount_OffUs"];
                dataRow["Disputed_Amount_Cardless"] = dr["Disputed_Amount_Cardless"];
                dataRow["Organization"] = "GBK";
                dataRow["Disputed_OnUs_HostAmount"] = dr["Disputed_OnUs_HostAmount"];
                dataRow["Disputed_OnUs_SwitchAmount"] = dr["Disputed_OnUs_SwitchAmount"];
                dataRow["Disputed_OnUs_EJAmount"] = dr["Disputed_OnUs_EJAmount"];
                dataRow["Disputed_OffUs_SwitchAmount"] = dr["Disputed_OffUs_SwitchAmount"];
                dataRow["Disputed_OffUs_EJAmount"] = dr["Disputed_OffUs_EJAmount"];
                dataRow["Disputed_BNA_HostAmount"] = dr["Disputed_BNA_HostAmount"];
                dataRow["Disputed_BNA_SwitchAmount"] = dr["Disputed_BNA_SwitchAmount"];
                dataRow["Disputed_BNA_EJAmount"] = dr["Disputed_BNA_EJAmount"];
                dataRow["Disputed_Cardless_HostAmount"] = dr["Disputed_Cardless_HostAmount"];
                dataRow["Disputed_Cardless_SwitchAmount"] = dr["Disputed_Cardless_SwitchAmount"];
                dataRow["Disputed_Cardless_EJAmount"] = dr["Disputed_Cardless_EJAmount"];

                dataRow["Disputed_OnUs_Host_Transactions"] = dr["Disputed_OnUs_Host_Transactions"];
                dataRow["Disputed_OnUs_Switch_Transactions"] = dr["Disputed_OnUs_Switch_Transactions"];
                dataRow["Disputed_OnUs_EJ_Transactions"] = dr["Disputed_OnUs_EJ_Transactions"];
                dataRow["Disputed_OffUs_Switch_Transactions"] = dr["Disputed_OffUs_Switch_Transactions"];
                dataRow["Disputed_OffUs_EJ_Transactions"] = dr["Disputed_OffUs_EJ_Transactions"];
                dataRow["Disputed_BNA_Host_Transactions"] = dr["Disputed_BNA_Host_Transactions"];
                dataRow["Disputed_BNA_Switch_Transactions"] = dr["Disputed_BNA_Switch_Transactions"];
                dataRow["Disputed_BNA_EJ_Transactions"] = dr["Disputed_BNA_EJ_Transactions"];
                dataRow["Disputed_Cardless_Host_Transactions"] = dr["Disputed_Cardless_Host_Transactions"];
                dataRow["Disputed_Cardless_Switch_Transactions"] = dr["Disputed_Cardless_Switch_Transactions"];
                dataRow["Disputed_Cardless_EJ_Transactions"] = dr["Disputed_Cardless_EJ_Transactions"];

                dataRow["Disputed_OnUs_Transactions"] = dr["Disputed_OnUs_Transactions"];
                dataRow["Disputed_BNA_Transactions"] = dr["Disputed_BNA_Transactions"];
                dataRow["Disputed_OffUs_Transactions"] = dr["Disputed_OffUs_Transactions"];
                dataRow["Disputed_Cardless_Transactions"] = dr["Disputed_Cardless_Transactions"];

                dataRow["Total_Off_Us_Transactions"] = dr["Total_Off_Us_Transactions"];
                dataRow["Total_Amount_OffUs"] = dr["Total_Amount_OffUs"];
                dataRow["KNET_Off_Us_Transactions"] = dr["KNET_Off_Us_Transactions"];
                dataRow["KNET_Amount_OffUs"] = dr["KNET_Amount_OffUs"];
                dataRow["GCC_Off_Us_Transactions"] = dr["GCC_Off_Us_Transactions"];
                dataRow["GCC_Amount_OffUs"] = dr["GCC_Amount_OffUs"];
                dataRow["Disputed_OffUs_Host_Transactions"] = dr["Disputed_OffUs_Host_Transactions"];
                dataRow["Disputed_OffUs_HostAmount"] = dr["Disputed_OffUs_HostAmount"];
                dataRow["Disputed_OffUs_KNET_Transactions"] = dr["Disputed_OffUs_KNET_Transactions"];
                dataRow["Disputed_OffUs_KNETAmount"] = dr["Disputed_OffUs_KNETAmount"];
                dataRow["Disputed_OffUs_GCC_Transactions"] = dr["Disputed_OffUs_GCC_Transactions"];
                dataRow["Disputed_OffUs_GCCAmount"] = dr["Disputed_OffUs_GCCAmount"];
                dataRow["Host_Tellerx_Transactions"] = dr["Host_Tellerx_Transactions"];
                dataRow["Host_Amount_Tellerx"] = dr["Host_Amount_Tellerx"];
                dataRow["Host_Tellerx_BNATransactions"] = dr["Host_Tellerx_BNATransactions"];
                dataRow["Host_AmountBNA_Tellerx"] = dr["Host_AmountBNA_Tellerx"];

                ds.Tables[0].Rows.Add(dataRow);



            }
            return ds;
        }

        private DataTable BindEJSwitchGrid(string orderBy, string reconciliation_batch_ids)
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("title");
            dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
            dtGrid.Columns.Add("PAN");
            dtGrid.Columns.Add("tsn");
            dtGrid.Columns.Add("amount");
            dtGrid.Columns.Add("reconciliation_transactions_id");
            //dtGrid.Columns.Add("is_on_ej", typeof(bool));
            //dtGrid.Columns.Add("is_on_switch", typeof(bool));
            dtGrid.Columns.Add("reason");
            dtGrid.Columns.Add("status");
            dtGrid.Columns.Add("status_on_ej");
            dtGrid.Columns.Add("status_on_switch");
            dtGrid.Columns.Add("status_on_host");
            dtGrid.Columns.Add("accountno");
            dtGrid.Columns.Add("transaction_no");
            dtGrid.Columns.Add("onus");
            dtGrid.Columns.Add("action");
            dtGrid.Columns.Add("card_issuer");
            dtGrid.Columns.Add("card_acquirer");

            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //if (DropDownList_Status.SelectedIndex > 0)
                cmd.CommandText = "select * from reconciliation_transactions where is_reconciled = 0  and is_reconciled_manually is null and comparison_type in (2,1,0) ";
                //else
                //    cmd.CommandText = "select * from reconciliation_transactions where  comparison_type in (2,1,0)";

                if (reconciliation_batch_ids != "")
                    cmd.CommandText += "and reconciliation_batch_id in (" + reconciliation_batch_ids.Trim() + ")";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable DT = new DataTable();
                adapter.Fill(DT);
                int totalCountONUS = 0, totalCountOFFUS = 0, totalCountCARDLESS = 0;
                decimal totalAmountONUS = 0, totalAmountOFFUS = 0, totalAmountCARDLESS = 0;

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
                            dataRow["amount"] = ejTrxn.Amount;
                            dataRow["title"] = Atm.LoadAtmByPk(ejTrxn.AtmId.Value).Title;
                            dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                            //dataRow["is_on_ej"] = true;
                            dataRow["reason"] = dr["reason"].ToString();
                            dataRow["status"] = dr["status"];
                            dataRow["accountno"] = ejTrxn.AccountNo;


                            dataRow["status_on_ej"] = GetStatusFriendlyName(ejTrxn.Status.Value) + "(" + ejTrxn.Result + ")";

                            if (dr["reconciliation_switch_data_id"].ToString() != null && dr["reconciliation_switch_data_id"] != DBNull.Value && dr["reconciliation_switch_data_id"].ToString() != "")
                            {
                                ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                                if (switchData != null)
                                    dataRow["status_on_switch"] = switchData.TransactionResponse;
                            }

                            string offus_cardIssuer = "---";
                            if (dr["reconciliation_host_data_id"].ToString() != null && dr["reconciliation_host_data_id"] != DBNull.Value && dr["reconciliation_host_data_id"].ToString() != "")
                            {
                                var s = dr["reconciliation_host_data_id"].ToString();
                                ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_host_data_id"].ToString()));
                                if (hostData != null)
                                {
                                    dataRow["status_on_host"] = hostData.TransactionResponse;
                                    dataRow["transaction_no"] = MappingAtmTxns.LoadMappingAtmTxns("small_description='" + hostData.TransactionType + "'").TransactionNo;
                                    offus_cardIssuer = hostData.CardIssuer;
                                }
                            }

                            FITMapping OnUSCards = FITMapping.LoadFITMapping("on_us = 1 and PAN_prefix= " + ejTrxn.Pan.Substring(0, 6));

                            if (OnUSCards == null)
                            {//cardless also comes here
                                if (ejTrxn.Pan.Substring(0, 6) == "999999")
                                {
                                    dataRow["onus"] = 999999;
                                    totalCountCARDLESS++;
                                    totalAmountCARDLESS += Convert.ToDecimal(dataRow["amount"]);

                                    dataRow["card_issuer"] = "GBK";
                                    dataRow["card_acquirer"] = "GBK";
                                }
                                else
                                {
                                    dataRow["onus"] = 0;
                                    totalCountOFFUS++;
                                    totalAmountOFFUS += Convert.ToDecimal(dataRow["amount"]);

                                    dataRow["card_issuer"] = offus_cardIssuer;
                                    dataRow["card_acquirer"] = "GBK";
                                }
                                dataRow["action"] = GetReconciliationAction(ejTrxn.Status.Value.ToString(), dataRow["status_on_switch"].ToString());

                            }
                            else
                            {
                                dataRow["onus"] = 1;
                                totalCountONUS++;
                                totalAmountONUS += Convert.ToDecimal(dataRow["amount"]);
                                dataRow["action"] = GetReconciliationAction(ejTrxn.Status.Value.ToString(), dataRow["status_on_switch"].ToString(), dataRow["status_on_host"].ToString());

                                dataRow["card_issuer"] = "GBK";
                                dataRow["card_acquirer"] = "GBK";
                            }

                            dtGrid.Rows.Add(dataRow);

                        }
                    }
                    else
                    {

                        if (dr["reconciliation_host_data_id"].ToString() != null && dr["reconciliation_host_data_id"] != DBNull.Value && dr["reconciliation_host_data_id"].ToString() != "" && dr["reconciliation_host_data_id"].ToString() != "0")
                        {
                            ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_host_data_id"].ToString()));
                            if (hostData != null)
                            {
                                DataRow dataRow = dtGrid.NewRow();

                                DateTime transactionDateTime;
                                var dateParts = DateTime.ParseExact(hostData.TransactionDate, "yyyyMMdd", null);
                                string[] timeParts = hostData.TransactionTime.Split(':');
                                transactionDateTime = new DateTime(dateParts.Year, dateParts.Month, dateParts.Day, int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse("00"));

                                //DateTime.TryParseExact(switchData.TransactionDate + " " + switchData.TransactionTime, dateFormats, null, System.Globalization.DateTimeStyles.None, out transactionDateTime);
                                dataRow["trxn_datetime"] = transactionDateTime;

                                //dataRow["trxn_datetime"] = switchData.TransactionDate + " " + switchData.TransactionTime;
                                dataRow["PAN"] = hostData.CardNumber;
                                dataRow["tsn"] = hostData.TransactionSequence;
                                dataRow["amount"] = hostData.TransactionAmount;
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                dataRow["status_on_host"] = hostData.TransactionResponse;
                                dataRow["transaction_no"] = MappingAtmTxns.LoadMappingAtmTxns("small_description='" + hostData.TransactionType + "'").TransactionNo;
                                Atm atm = Atm.LoadAtm("title = '" + hostData.AtmId + "'");
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
                                        EjParsedTransactions ejTrxn = EjParsedTransactions.LoadEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                                        dataRow["amount"] = ejTrxn.Amount;
                                    }
                                }
                                dataRow["status"] = dr["status"];
                                dataRow["accountno"] = hostData.CustomerAccountNo;


                                if (dr["reconciliation_switch_data_id"].ToString() != null && dr["reconciliation_switch_data_id"] != DBNull.Value && dr["reconciliation_switch_data_id"].ToString() != "" && dr["reconciliation_switch_data_id"].ToString() != "0")
                                {
                                    ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                                    if (switchData != null)
                                        dataRow["status_on_switch"] = switchData.TransactionResponse;
                                }

                                FITMapping OnUSCards = FITMapping.LoadFITMapping("on_us = 1 and PAN_prefix= " + hostData.CardNumber.Substring(0, 6));

                                if (OnUSCards == null)
                                {//cardless also comes here
                                    if (hostData.CardNumber.Substring(0, 6) == "999999")
                                    {
                                        dataRow["onus"] = 999999;
                                        totalCountCARDLESS++;
                                        totalAmountCARDLESS += Convert.ToDecimal(dataRow["amount"]);

                                        dataRow["card_issuer"] = hostData.CardIssuer;
                                        dataRow["card_acquirer"] = hostData.CardIssuer;
                                    }
                                    else
                                    {
                                        dataRow["onus"] = 0;
                                        totalCountOFFUS++;
                                        totalAmountOFFUS += Convert.ToDecimal(dataRow["amount"]);

                                        dataRow["card_issuer"] = hostData.CardIssuer;
                                        dataRow["card_acquirer"] = hostData.CardIssuer;
                                    }
                                    dataRow["action"] = GetReconciliationAction("1", dataRow["status_on_switch"].ToString());
                                }
                                else
                                {
                                    dataRow["onus"] = 1;
                                    totalCountONUS++;
                                    totalAmountONUS += Convert.ToDecimal(dataRow["amount"]);
                                    dataRow["action"] = GetReconciliationAction("1", dataRow["status_on_switch"].ToString(), dataRow["status_on_host"].ToString());

                                    dataRow["card_issuer"] = hostData.CardIssuer;
                                    dataRow["card_acquirer"] = hostData.CardIssuer;
                                }

                                dtGrid.Rows.Add(dataRow);

                            }
                        }
                        else if (dr["reconciliation_switch_data_id"].ToString() != null && dr["reconciliation_switch_data_id"] != DBNull.Value && dr["reconciliation_switch_data_id"].ToString() != "" && dr["reconciliation_switch_data_id"].ToString() != "0")
                        {
                            ReconciliationSwitchData switchData = ReconciliationSwitchData.LoadReconciliationSwitchDataByPk(int.Parse(dr["reconciliation_switch_data_id"].ToString()));
                            if (switchData != null)
                            {
                                DataRow dataRow = dtGrid.NewRow();

                                DateTime transactionDateTime;
                                var dateParts = DateTime.ParseExact(switchData.TransactionDate, "yyyyMMdd", null);
                                string[] timeParts = switchData.TransactionTime.Split(':');
                                transactionDateTime = new DateTime(dateParts.Year, dateParts.Month, dateParts.Day, int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse("00"));

                                //DateTime.TryParseExact(switchData.TransactionDate + " " + switchData.TransactionTime, dateFormats, null, System.Globalization.DateTimeStyles.None, out transactionDateTime);
                                dataRow["trxn_datetime"] = transactionDateTime;

                                //dataRow["trxn_datetime"] = switchData.TransactionDate + " " + switchData.TransactionTime;
                                dataRow["PAN"] = switchData.CardNumber;
                                dataRow["tsn"] = switchData.TransactionSequence;
                                dataRow["amount"] = switchData.TransactionAmount;
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                dataRow["status_on_switch"] = switchData.TransactionResponse;
                                Atm atm = Atm.LoadAtm("title = '" + switchData.AtmId + "'");
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
                                        EjParsedTransactions ejTrxn = EjParsedTransactions.LoadEjParsedTransactionsByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                                        dataRow["amount"] = ejTrxn.Amount;
                                    }
                                }
                                dataRow["status"] = dr["status"];
                                dataRow["accountno"] = switchData.CustomerAccountNo;

                                string offus_cardIssuer = "---";
                                if (dr["reconciliation_host_data_id"].ToString() != null && dr["reconciliation_host_data_id"] != DBNull.Value && dr["reconciliation_host_data_id"].ToString() != "")
                                {
                                    ReconciliationHostData hostData = ReconciliationHostData.LoadReconciliationHostDataByPk(int.Parse(dr["reconciliation_host_data_id"].ToString()));
                                    if (hostData != null)
                                    {
                                        dataRow["status_on_host"] = hostData.TransactionResponse;
                                        dataRow["transaction_no"] = MappingAtmTxns.LoadMappingAtmTxns("small_description='" + hostData.TransactionType + "'").TransactionNo;
                                        offus_cardIssuer = hostData.CardIssuer;
                                    }
                                }

                                FITMapping OnUSCards = FITMapping.LoadFITMapping("on_us = 1 and PAN_prefix= " + switchData.CardNumber.Substring(0, 6));

                                if (OnUSCards == null)
                                {//cardless also comes here
                                    if (switchData.CardNumber.Substring(0, 6) == "999999")
                                    {
                                        dataRow["onus"] = 999999;
                                        totalCountCARDLESS++;
                                        totalAmountCARDLESS += Convert.ToDecimal(dataRow["amount"]);

                                        dataRow["card_issuer"] = "GBK";
                                        dataRow["card_acquirer"] = "GBK";
                                    }
                                    else
                                    {
                                        dataRow["onus"] = 0;
                                        totalCountOFFUS++;
                                        totalAmountOFFUS += Convert.ToDecimal(dataRow["amount"]);

                                        dataRow["card_issuer"] = offus_cardIssuer;
                                        dataRow["card_acquirer"] = "GBK";
                                    }
                                    dataRow["action"] = GetReconciliationAction("1", dataRow["status_on_switch"].ToString());
                                }
                                else
                                {
                                    dataRow["onus"] = 1;
                                    totalCountONUS++;
                                    totalAmountONUS += Convert.ToDecimal(dataRow["amount"]);
                                    dataRow["action"] = GetReconciliationAction("1", dataRow["status_on_switch"].ToString(), dataRow["status_on_host"].ToString());

                                    dataRow["card_issuer"] = "GBK";
                                    dataRow["card_acquirer"] = "GBK";
                                }

                                dtGrid.Rows.Add(dataRow);

                            }
                        }
                    }
                }
                dtGrid.DefaultView.Sort = orderBy;

                DataRow dataRowTotalONUS = dtGrid.NewRow();
                dataRowTotalONUS["title"] = "Totals :" + totalCountONUS;
                dataRowTotalONUS["onus"] = "1";
                dataRowTotalONUS["transaction_no"] = "Amount :";
                dataRowTotalONUS["amount"] = totalAmountONUS;
                //dataRowTotalONUS["status_on_ej"] = "Total Count";
                //dataRowTotalONUS["status_on_switch"] = totalCountONUS;
                dtGrid.Rows.Add(dataRowTotalONUS);

                DataRow dataRowTotalOFFUS = dtGrid.NewRow();
                dataRowTotalOFFUS["title"] = "Totals :" + totalCountOFFUS;
                dataRowTotalOFFUS["onus"] = "0";
                dataRowTotalOFFUS["transaction_no"] = "Amount :";
                dataRowTotalOFFUS["amount"] = totalAmountOFFUS;
                //dataRowTotalOFFUS["status_on_ej"] = "Total Count";
                //dataRowTotalOFFUS["status_on_switch"] = totalCountOFFUS;
                dtGrid.Rows.Add(dataRowTotalOFFUS);

                DataRow dataRowTotalCARDLESS = dtGrid.NewRow();
                dataRowTotalCARDLESS["title"] = "Totals :" + totalCountCARDLESS;
                dataRowTotalCARDLESS["onus"] = "999999";
                dataRowTotalCARDLESS["transaction_no"] = "Amount :";
                dataRowTotalCARDLESS["amount"] = totalAmountCARDLESS;
                //dataRowTotalCARDLESS["status_on_ej"] = "Total Count";
                //dataRowTotalCARDLESS["status_on_switch"] = totalCountCARDLESS;
                dtGrid.Rows.Add(dataRowTotalCARDLESS);

                //GridView_EJSwitch.DataSource = dtGrid.DefaultView;
                //GridView_EJSwitch.DataBind();
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("BindEJSwitchGrid");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
                // Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
            return dtGrid;
        }

        private DataTable BindEJBNASwitchGrid(string orderBy, string reconciliation_batch_ids)
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("title");
            dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
            dtGrid.Columns.Add("PAN");
            dtGrid.Columns.Add("tsn");
            dtGrid.Columns.Add("amount", typeof(decimal));
            dtGrid.Columns.Add("reconciliation_transactions_id");
            //dtGrid.Columns.Add("is_on_ej", typeof(bool));
            //dtGrid.Columns.Add("is_on_switch", typeof(bool));
            dtGrid.Columns.Add("reason");
            dtGrid.Columns.Add("status");
            dtGrid.Columns.Add("accountno");
            dtGrid.Columns.Add("transaction_no");

            dtGrid.Columns.Add("status_on_ej");
            dtGrid.Columns.Add("status_on_switch");
            dtGrid.Columns.Add("status_on_host");
            dtGrid.Columns.Add("action");
            dtGrid.Columns.Add("card_issuer");
            dtGrid.Columns.Add("card_acquirer");

            try
            {

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //if (DropDownList_Status.SelectedIndex > 0)
                cmd.CommandText = "select * from reconciliation_transactions where is_reconciled=0  and is_reconciled_manually is null and comparison_type in (4,5,6) ";
                //else
                //    cmd.CommandText = "select * from reconciliation_transactions where comparison_type in (4,5,6)";

                if (reconciliation_batch_ids != "")
                    cmd.CommandText += "and reconciliation_batch_id in (" + reconciliation_batch_ids.Trim() + ")";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable DT = new DataTable();
                adapter.Fill(DT);
                int totalCount = 0;
                decimal totalAmount = 0;

                foreach (DataRow dr in DT.Rows)
                {
                    if ((dr["ej_parsed_bna_transaction_id"].ToString() != "0") && (dr["ej_parsed_bna_transaction_id"].ToString() != ""))
                    {
                        VEjParsedBNATransaction ejBNATrxn = VEjParsedBNATransaction.LoadVEjParsedBNATransactionsByPk(int.Parse(dr["ej_parsed_bna_transaction_id"].ToString()));
                        if (ejBNATrxn != null)
                        {
                            DataRow dataRow = dtGrid.NewRow();
                            dataRow["trxn_datetime"] = ejBNATrxn.TrxnDatetime;
                            dataRow["PAN"] = ejBNATrxn.Pan;
                            dataRow["tsn"] = ejBNATrxn.Seq;
                            dataRow["amount"] = ejBNATrxn.AmountAuthorized;
                            dataRow["title"] = Atm.LoadAtmByPk(ejBNATrxn.AtmId).Title;
                            dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                            //dataRow["is_on_ej"] = true;
                            dataRow["reason"] = dr["reason"].ToString();
                            dataRow["status"] = dr["status"];
                            dataRow["accountno"] = ejBNATrxn.AccountNo;

                            dataRow["status_on_ej"] = ejBNATrxn.Status;

                            if (dr["reconciliation_bna_switch_data_id"].ToString() != "0" && dr["reconciliation_bna_switch_data_id"] != DBNull.Value && dr["reconciliation_bna_switch_data_id"].ToString() != string.Empty)
                            {
                                ReconciliationBnaSwitchData switchData = ReconciliationBnaSwitchData.LoadReconciliationBnaSwitchDataByPk(int.Parse(dr["reconciliation_bna_switch_data_id"].ToString()));
                                if (switchData != null)
                                    dataRow["status_on_switch"] = switchData.TransactionResponse;
                            }

                            if (dr["reconciliation_bna_host_data_id"].ToString() != "0" && dr["reconciliation_bna_host_data_id"] != DBNull.Value && dr["reconciliation_bna_host_data_id"].ToString() != string.Empty)
                            {
                                ReconciliationBnaHostData hostData = ReconciliationBnaHostData.LoadReconciliationBnaHostDataByPk(int.Parse(dr["reconciliation_bna_host_data_id"].ToString()));
                                if (hostData != null)
                                {
                                    dataRow["status_on_host"] = hostData.TransactionResponse;
                                    dataRow["transaction_no"] = MappingAtmTxns.LoadMappingAtmTxns("small_description='" + hostData.TransactionType + "'").TransactionNo;
                                }
                            }
                            dataRow["action"] = GetReconciliationAction_BNA(ejBNATrxn.Status, dataRow["status_on_switch"].ToString(), dataRow["status_on_host"].ToString());
                            dataRow["card_issuer"] = "GBK";
                            dataRow["card_acquirer"] = "GBK";

                            dtGrid.Rows.Add(dataRow);
                            totalCount++;
                            totalAmount += Convert.ToDecimal(dataRow["amount"]);
                        }
                    }
                    else
                    {
                        if (dr["reconciliation_bna_switch_data_id"].ToString() != "0" && dr["reconciliation_bna_switch_data_id"] != DBNull.Value && dr["reconciliation_bna_switch_data_id"].ToString() != string.Empty)
                        {
                            ReconciliationBnaSwitchData switchData = ReconciliationBnaSwitchData.LoadReconciliationBnaSwitchDataByPk(int.Parse(dr["reconciliation_bna_switch_data_id"].ToString()));
                            if (switchData != null)
                            {
                                DataRow dataRow = dtGrid.NewRow();

                                DateTime transactionDateTime;
                                DateTime.TryParseExact(switchData.TransactionDate + " " + switchData.TransactionTime, dateFormats, null, System.Globalization.DateTimeStyles.None, out transactionDateTime);
                                dataRow["trxn_datetime"] = transactionDateTime;

                                //dataRow["trxn_datetime"] = switchData.TransactionDate + " " + switchData.TransactionTime;
                                dataRow["PAN"] = switchData.CardNumber;
                                dataRow["tsn"] = switchData.TransactionSequence;
                                dataRow["amount"] = switchData.TransactionAmount;
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                Atm atm = Atm.LoadAtm("title = '" + switchData.AtmId + "'");
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
                                        EjParsedBnaTransaction ejBNATrxn = EjParsedBnaTransaction.LoadEjParsedBnaTransactionByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                                        dataRow["amount"] = ejBNATrxn.AmountAuthorized;
                                    }
                                }
                                dataRow["status"] = dr["status"];
                                dataRow["accountno"] = switchData.CustomerAccountNo;

                                dataRow["status_on_switch"] = switchData.TransactionResponse;
                                if (dr["reconciliation_bna_host_data_id"].ToString() != "0" && dr["reconciliation_bna_host_data_id"] != DBNull.Value && dr["reconciliation_bna_host_data_id"].ToString() != string.Empty)
                                {
                                    ReconciliationBnaHostData hostData = ReconciliationBnaHostData.LoadReconciliationBnaHostDataByPk(int.Parse(dr["reconciliation_bna_host_data_id"].ToString()));
                                    if (hostData != null)
                                    {
                                        dataRow["status_on_host"] = hostData.TransactionResponse;
                                        dataRow["transaction_no"] = MappingAtmTxns.LoadMappingAtmTxns("small_description='" + hostData.TransactionType + "'").TransactionNo;
                                    }
                                }

                                dataRow["action"] = GetReconciliationAction_BNA("1", dataRow["status_on_switch"].ToString(), dataRow["status_on_host"].ToString());

                                dtGrid.Rows.Add(dataRow);
                                totalCount++;
                                totalAmount += Convert.ToDecimal(dataRow["amount"]);
                            }
                        }
                        else if (dr["reconciliation_bna_host_data_id"].ToString() != "0" && dr["reconciliation_bna_host_data_id"] != DBNull.Value && dr["reconciliation_bna_host_data_id"].ToString() != string.Empty)
                        {
                            ReconciliationBnaHostData hostData = ReconciliationBnaHostData.LoadReconciliationBnaHostDataByPk(int.Parse(dr["reconciliation_bna_host_data_id"].ToString()));
                            if (hostData != null)
                            {
                                DataRow dataRow = dtGrid.NewRow();

                                DateTime transactionDateTime;
                                DateTime.TryParseExact(hostData.TransactionDate + " " + hostData.TransactionTime, dateFormats, null, System.Globalization.DateTimeStyles.None, out transactionDateTime);
                                dataRow["trxn_datetime"] = transactionDateTime;

                                //dataRow["trxn_datetime"] = switchData.TransactionDate + " " + switchData.TransactionTime;
                                dataRow["PAN"] = hostData.CardNumber;
                                dataRow["tsn"] = hostData.TransactionSequence;
                                dataRow["amount"] = hostData.TransactionAmount;
                                dataRow["reconciliation_transactions_id"] = dr["reconciliation_transactions_id"];
                                Atm atm = Atm.LoadAtm("title = '" + hostData.AtmId + "'");
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
                                        EjParsedBnaTransaction ejBNATrxn = EjParsedBnaTransaction.LoadEjParsedBnaTransactionByPk(int.Parse(dr["ej_parsed_transactions_id"].ToString()));
                                        dataRow["amount"] = ejBNATrxn.AmountAuthorized;
                                    }
                                }
                                dataRow["status"] = dr["status"];
                                dataRow["accountno"] = hostData.CustomerAccountNo;

                                dataRow["status_on_host"] = hostData.TransactionResponse;
                                dataRow["transaction_no"] = MappingAtmTxns.LoadMappingAtmTxns("small_description='" + hostData.TransactionType + "'").TransactionNo;

                                if (dr["reconciliation_bna_switch_data_id"].ToString() != "0" && dr["reconciliation_bna_switch_data_id"] != DBNull.Value && dr["reconciliation_bna_switch_data_id"].ToString() != string.Empty)
                                {
                                    ReconciliationBnaSwitchData switchData = ReconciliationBnaSwitchData.LoadReconciliationBnaSwitchDataByPk(int.Parse(dr["reconciliation_bna_switch_data_id"].ToString()));
                                    if (switchData != null)
                                    {
                                        dataRow["status_on_switch"] = switchData.TransactionResponse;
                                    }
                                }

                                dataRow["action"] = GetReconciliationAction_BNA("1", dataRow["status_on_switch"].ToString(), dataRow["status_on_host"].ToString());
                                dataRow["card_issuer"] = "GBK";
                                dataRow["card_acquirer"] = "GBK";

                                dtGrid.Rows.Add(dataRow);
                                totalCount++;
                                totalAmount += Convert.ToDecimal(dataRow["amount"]);
                            }
                        }

                    }
                }
                dtGrid.DefaultView.Sort = orderBy;

                DataRow dataRowTotal = dtGrid.NewRow();
                dataRowTotal["title"] = "Totals :" + totalCount;
                dataRowTotal["transaction_no"] = "Amount :";
                dataRowTotal["amount"] = totalAmount;
                //dataRowTotal["status_on_ej"] = "Total Count";
                //dataRowTotal["status_on_switch"] = totalCount;
                dtGrid.Rows.Add(dataRowTotal);


                //GridView_EJBNASwitch.DataSource = dtGrid.DefaultView;
                //GridView_EJBNASwitch.DataBind();
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("BindEJBNASwitchGrid");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
                // Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
            return dtGrid;
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

        private string GetReconciliationAction(string ejStatus, string switchStatus, string hostStatus)
        {
            //LogableTask task = LogableTask.NewTask("GetReconciliationAction");
            //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "ej:" + ejStatus + ",Switch:" + switchStatus
            //    + ",host:" + hostStatus);
            //task.EndTask();

            if (hostStatus == "000")
                hostStatus = "0000";//change OFFUS host status same as ONUS host status

            string action = "";
            if (ejStatus == "0" && switchStatus == "000" && hostStatus != "0000")
                action = "Credit GL";
            else if (ejStatus == "0" && switchStatus != "000" && hostStatus != "0000")
                action = "Credit GL";
            else if (ejStatus != "0" && switchStatus != "000" && hostStatus == "0000")
                action = "Debit GL";
            else if (ejStatus != "0" && switchStatus == "000" && hostStatus == "0000")
                action = "Debit GL";

            return action;
        }

        private string GetReconciliationAction(string ejStatus, string switchStatus)
        {
            //LogableTask task = LogableTask.NewTask("GetReconciliationAction1");
            //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "ej:" + ejStatus + ",Switch:" + switchStatus);
            //task.EndTask();

            string action = "";
            if (ejStatus == "0" && switchStatus != "000")
                action = "Credit GL";
            else if (ejStatus != "0" && switchStatus == "000")
                action = "Debit GL";

            return action;
        }

        private string GetReconciliationAction_BNA(string ejStatus, string switchStatus, string hostStatus)
        {
            string action = "";
            if (ejStatus == "Successful" && switchStatus == "000" && hostStatus != "0000")
                action = "Debit GL";
            else if (ejStatus == "Successful" && switchStatus != "000" && hostStatus != "0000")
                action = "Debit GL";
            else if (ejStatus != "Successful" && switchStatus != "000" && hostStatus == "0000")
                action = "Credit GL";
            else if (ejStatus != "Successful" && switchStatus == "000" && hostStatus == "0000")
                action = "Credit GL";

            return action;
        }

    }
}