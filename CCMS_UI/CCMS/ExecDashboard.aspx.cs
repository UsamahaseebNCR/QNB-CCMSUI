
using Avanza.iSuite.UI;
using ComponentArt.Web.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class ExecDashboard : BasePage
    {
        private static int TransactionSuccessful = 0;
        private static string TransactionSuccessfulString = "Successful";
        private static DateTime FromDate = DateTime.Today;
        private static DateTime ToDate = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["fromDate"].ToString()))
                    DateTime.TryParse(Request.QueryString["fromDate"].ToString(), out FromDate);

                if (!string.IsNullOrEmpty(Request.QueryString["toDate"].ToString()))
                    DateTime.TryParse(Request.QueryString["toDate"].ToString(), out ToDate);

                BindGrid();
            }
        }

        private void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                FillAtmCdmAvailaibility();
                FillAtmTransactionInfo();
                FillTopCharts();
                FillAtmPowerDownCount();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private void FillTop3Cdms()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;

            try
            {
                    List<int> data = new List<int>();
                    List<string> labels = new List<string>();

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetTopNCDMs";
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = FromDate;
                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = ToDate;
                    cmd.Parameters.Add("@order", SqlDbType.Int);
                    cmd.Parameters[2].Value = 1;
                    cmd.Parameters.Add("@atmIds", SqlDbType.VarChar);
                    cmd.Parameters[3].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                    cmd.Parameters.Add("@limit", SqlDbType.VarChar);
                    cmd.Parameters[4].Value = 3;
                    adapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        data.Add(Convert.ToInt32(dr["total"]));
                        labels.Add(dr["title"].ToString());
                    }

                    string top3CdmScript = "top3CdmData = " + new JavaScriptSerializer().Serialize(data) + "; " +
                    "top3CdmLabel = " + new JavaScriptSerializer().Serialize(labels) + "; loadTop3CdmChart();";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "top3CdmScriptKey", top3CdmScript, true);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }
        }

        private void FillTop3Atms()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;

            try
            {
                
                    List<int> data = new List<int>();
                    List<string> labels = new List<string>();

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetTopNATMsByEjTransaction";
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = FromDate;
                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = ToDate;
                    cmd.Parameters.Add("@order", SqlDbType.Int);
                    cmd.Parameters[2].Value = 1;
                    cmd.Parameters.Add("@atmIds", SqlDbType.VarChar);
                    cmd.Parameters[3].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                    cmd.Parameters.Add("@limit", SqlDbType.Int);
                    cmd.Parameters[4].Value = 3; 
                    adapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        data.Add(Convert.ToInt32(dr["total"]));
                        labels.Add(dr["title"].ToString());
                    }

                    string top3AtmScript = "top3AtmData = " + new JavaScriptSerializer().Serialize(data) + "; " +
                    "top3AtmLabel = " + new JavaScriptSerializer().Serialize(labels) + "; loadTop3AtmChart();";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "top3AtmScriptKey", top3AtmScript, true);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }
        }

        private void FillTop3Cpms()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;

            try
            {
                
                    List<int> data = new List<int>();
                    List<string> labels = new List<string>();

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetTopNCPMs";
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = FromDate;
                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = ToDate;
                    cmd.Parameters.Add("@order", SqlDbType.Int);
                    cmd.Parameters[2].Value = 1;
                    cmd.Parameters.Add("@atmIds", SqlDbType.VarChar);
                    cmd.Parameters[3].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                    cmd.Parameters.Add("@limit", SqlDbType.Int);
                    cmd.Parameters[4].Value = 3; 
                    adapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        data.Add(Convert.ToInt32(dr["total"]));
                        labels.Add(dr["title"].ToString());
                    }

                    string top3CpmScript = "top3CpmData = " + new JavaScriptSerializer().Serialize(data) + "; " +
                    "top3CpmLabel = " + new JavaScriptSerializer().Serialize(labels) + "; loadTop3CpmChart();";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "top3CpmScriptKey", top3CpmScript, true);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }
        }

        private void FillTopCharts()
        {
            try
            {
                FillTop3Atms();
                FillTop3Cdms();
                FillTop3Cpms();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillAtmCdmAvailaibility()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;
            LogableTask task = LogableTask.NewTask("FillAtmCdmAvailaibility");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTotalAvailabilityByDateAndAtm";
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = FromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = ToDate;
                cmd.Parameters.Add("@atmIDs", SqlDbType.VarChar);
                cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                double atmAvail = 0;
                double cdmAvail = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    //ATM
                    if (dr["atm_type"].ToString() == "0")
                    {
                        atmAvail = Convert.ToDouble(dr["average"]);
                        AtmAvailabilityAgreedSla.InnerText = dr["agreed_sla"].ToString().Contains("%") ? dr["agreed_sla"].ToString() : dr["agreed_sla"].ToString() + "%";
                    }
                    else if (dr["atm_type"].ToString() == "1")
                    {
                        cdmAvail = Convert.ToDouble(dr["average"]);
                        CdmAvailabilityAgreedSla.InnerText = dr["agreed_sla"].ToString().Contains("%") ? dr["agreed_sla"].ToString() : dr["agreed_sla"].ToString() + "%";
                    }
                }

                string atmScript = @"$('#progress-circle-atm').percircle({
                                            progressBarColor: '#008F9B',
                                            text: '" + (atmAvail > 0 ? Convert.ToInt64(Math.Floor(atmAvail)) : 0) + @"%',
                                            percent: " + (atmAvail > 0 ? Convert.ToInt64(Math.Floor(atmAvail)) : 0.1) + 
                                       " });" +
                                   @"$('#progress-circle-cdm').percircle({
                                            progressBarColor: '#008F9B',
                                            text: '" + (cdmAvail > 0 ? Convert.ToInt64(Math.Floor(cdmAvail)) : 0) + @"%',
                                            percent: " + (cdmAvail > 0 ? Convert.ToInt64(Math.Floor(cdmAvail)) : 0.1) + 
                                       " });";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "atmScriptKey", atmScript, true);
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();

            }
        }

        private void FillAtmTransactionInfo()
        {
            object res = null;
            try
            {
                //Today
                res = ConnectionFactory.ExecuteScalar(string.Format("SELECT COUNT(*) FROM vEjParsedTransactions t, transaction_type tt WHERE t.transaction_type_id = tt.transaction_type_id AND (tt.transaction_type_name LIKE '%withdraw%' OR tt.transaction_type_name LIKE '%WTD%') AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = {2} and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")", 
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessful));
                WithdrawalTransactionToday.InnerText = res.ToString();

                res = ConnectionFactory.ExecuteScalar(string.Format("SELECT COUNT(*) FROM vEjParsedBNATransaction where trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = '{2}' and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")", 
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessfulString));
                CashDepositTransactionToday.InnerText = res.ToString();

                res = ConnectionFactory.ExecuteScalar(string.Format("SELECT COUNT(*) FROM vEjParsedCPMtransaction where trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = '{2}' and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")", 
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessfulString));
                CpmTransactionToday.InnerText = res.ToString();

                //Balance Inquiry
                res = ConnectionFactory.ExecuteScalar(
                    string.Format("SELECT COUNT(*) FROM vEjParsedTransactions t, transaction_type tt WHERE t.transaction_type_id = tt.transaction_type_id AND tt.transaction_type_name LIKE '%bal%' AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = {2} and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")",
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessful));
                BalanceInquiryToday.InnerText = res.ToString();

                res = ConnectionFactory.ExecuteScalar(
                    string.Format(@"SELECT COUNT(*) FROM (
                                        SELECT trxn_datetime FROM vEjParsedTransactions WHERE is_cardless = 1 AND status =0 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + 
                                        ") UNION SELECT trxn_datetime FROM vEjParsedBNATransaction WHERE is_cardless = 1 AND status = 'Successful' and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + 
                                         ") UNION SELECT trxn_datetime FROM vEjParsedCPMtransaction WHERE is_cardless = 1 AND status = 'Successful' and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + 
                                         ")) Transactions WHERE trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103)", FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy")));
                CardlessTransactionsCount.InnerText = res.ToString();

                res = ConnectionFactory.ExecuteScalar(
                    string.Format(@"SELECT COUNT(*) FROM vEjParsedTransactions t, transaction_type tt  
                                    WHERE t.transaction_type_id = tt.transaction_type_id 
                                    AND ISNULL(is_cardless, 0) = 0 AND status =0
                                    AND t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + 
                                    ") AND tt.transaction_type_name NOT LIKE '%withdraw%' AND tt.transaction_type_name NOT LIKE '%bal%' AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103)", 
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy")));
                OtherTransactionsCount.InnerText = res.ToString();
            }
            catch (Exception ex)
            {
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
            }
        }

        private void FillAtmPowerDownCount()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;

            try
            {
                object totalAtms = ConnectionFactory.ExecuteScalar("SELECT COUNT(*) FROM atm WHERE is_active = 1");
                TotalAtmCount.InnerText = totalAtms.ToString();
                if (Convert.ToInt32(totalAtms) > 0)
                {
                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummaryByDateAndAtm";
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = FromDate;
                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = ToDate;
                    cmd.Parameters.Add("@atmIDs", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                    cmd.Parameters.Add("@alertType", SqlDbType.Int);
                    cmd.Parameters[3].Value = 68;
                    adapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    int atmDown = 0;
                    foreach (DataRow dr in dt.Rows)
                        atmDown += Convert.ToInt32(dr["total"]);

                    string powerDownScript = @"$('#progress-circle-atm-down').percircle({
                                            progressBarColor: '#D40C0C',
                                            text: '" + (atmDown > 0 ? Convert.ToInt32(atmDown) : 0) + @"',
                                            percent: " + (atmDown > 0 ? (atmDown * 100) / Convert.ToInt32(totalAtms) : 0.1) +
                                           " });";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "powerDownScriptKey", powerDownScript, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();

            }
        }

        /* 
         Ajax Methods for Page
         */
         [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetWithdrawalTransactionDetail()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder tableRows = new StringBuilder();
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetWithdrawalTransactionDetail");
            try
            {
                DateTime todayShortDate = DateTime.Today;
                string todayShortDatString = DateTime.Today.ToShortDateString();

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT trxn_datetime, amount, title 
                                                FROM vEjParsedTransactions t, transaction_type tt, atm 
                                                WHERE t.transaction_type_id = tt.transaction_type_id AND t.atm_id = atm.ATM_id
                                                AND (tt.transaction_type_name LIKE '%withdraw%' OR tt.transaction_type_name LIKE '%WTD%')
                                                AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = {2}",
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessful);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    tableRows.Append("<tr><td>" + dr["title"] + "</td><td>" + dr["trxn_datetime"] + "</td><td>" + dr["amount"] + "</td></tr>");
                }

                int total = dt.Rows.Count;
                object mtdTotal = ConnectionFactory.ExecuteScalar(string.Format("SELECT COUNT(*) FROM vEjParsedTransactions t, transaction_type tt WHERE t.transaction_type_id = tt.transaction_type_id AND tt.transaction_type_name LIKE '%withdraw%' AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1} 23:59:59',103) AND status = {2}"
                    , new DateTime(todayShortDate.Year, todayShortDate.Month, 1).ToString("dd/MM/yyyy"), todayShortDatString, TransactionSuccessful));
                object onUs = ConnectionFactory.ExecuteScalar(string.Format(@"select COUNT(*) from vEjParsedTransactions t 
                                    INNER JOIN transaction_type tt ON t.transaction_type_id = tt.transaction_type_id AND (tt.transaction_type_name LIKE '%withdraw%' OR tt.transaction_type_name LIKE '%WTD%')
                                    INNER JOIN FITMapping fm 
                                    ON SUBSTRING (t.pan, 1, 6)  = fm.PAN_prefix AND t.pan IS NOT NULL AND fm.on_us = 1
                                    WHERE trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = {2} and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")",
                                        FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessful));
                object mtdOnUs = ConnectionFactory.ExecuteScalar(string.Format(@"select COUNT(*) from vEjParsedTransactions t 
                                    INNER JOIN transaction_type tt ON t.transaction_type_id = tt.transaction_type_id AND tt.transaction_type_name LIKE '%withdraw%'
                                    INNER JOIN FITMapping fm 
                                    ON SUBSTRING (t.pan, 1, 6)  = fm.PAN_prefix AND t.pan IS NOT NULL AND fm.on_us = 1
                                    WHERE trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = {2} and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")",
                                        FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessful));

                int offUs = Convert.ToInt32(total) - Convert.ToInt32(onUs);
                int mtdOffUs = Convert.ToInt32(mtdTotal) - Convert.ToInt32(mtdOnUs);

                sb.Append(string.Format(@"
                    <div class='row'>
                        <div class='col-sm-4'>
                            <div class='row m-t-xs'>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Today</h5>
                                    <h1 class='no-margins'>{0}</h1>
                                </div>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>MTD</h5>
                                    <h1 class='no-margins'>{1}</h1>
                                </div>
                            </div>
                            <table class='table small m-t-sm'>
                                <tbody>
                                <tr>
                                    <td><strong>{2}</strong> On Us</td>
                                    <td><strong>{3}</strong> On Us</td>
                                </tr>
                                <tr>
                                    <td><strong>{4}</strong> Off Us</td>
                                    <td><strong>{5}</strong> Off Us</td>
                                </tr>
                                </tbody>
                            </table>
                        </div>
                        <div class='col-sm-8'>
                            <table id='modalTable' class='table table-striped table-bordered'>
                                <thead><tr><th>Machine ID</th><th>Date Time</th><th>Amount</th></tr></thead>
                                <tbody>{6}</tbody>
                            </table>
                        </div>
                    </div>", total, mtdTotal, onUs, mtdOnUs, offUs, mtdOffUs, tableRows.ToString()));

                return sb.ToString();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
                task.EndTask();
            }
        }

         [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetCashDepositTransactionDetail()
        {
            StringBuilder sb = new StringBuilder();
            SqlCommand cmd = null;
            StringBuilder tableRows = new StringBuilder();
            LogableTask task = LogableTask.NewTask("GetCashDepositTransactionDetail");
            try
            {
                DateTime todayShortDate = DateTime.Today;
                string todayShortDatString = DateTime.Today.ToShortDateString();
                int monthsToCheckBack = 6;

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT trxn_datetime, amount_authorized AS amount, title 
                                                FROM vEjParsedBNATransaction t, transaction_type tt, atm 
                                                WHERE t.transaction_type_id = tt.transaction_type_id AND t.atm_id = atm.ATM_id
                                                AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = '{2}' and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")",
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessfulString);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    tableRows.Append("<tr><td>" + dr["title"] + "</td><td>" + dr["trxn_datetime"] + "</td><td>" + dr["amount"] + "</td></tr>");
                }

                int total = dt.Rows.Count;
                
                object mtdTotal = ConnectionFactory.ExecuteScalar(string.Format("SELECT COUNT(*) FROM vEjParsedBNATransaction where trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = '{2}' and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")", 
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessfulString));

                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT Month(trxn_datetime) m, Year(trxn_datetime) y, COUNT(*) total FROM vEjParsedBNATransaction 
                                        where trxn_datetime >= convert(datetime,'{0}',103)      
                                        and trxn_datetime <= convert(datetime,'{1}',103)    
                                        AND status='{2}'
                                        AND atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + 
                                        ") group by Month(trxn_datetime), Year(trxn_datetime) ORDER BY y, m",
                                            FromDate.ToString("dd/MM/yyyy"),
                                            ToDate.ToString("dd/MM/yyyy"), todayShortDatString, TransactionSuccessfulString);
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(Convert.ToInt32(dr["total"]));
                    labels.Add(string.Format("{0}/{1}", dr["m"].ToString(), dr["y"].ToString()));
                }

                sb.Append(string.Format(@"
                    <div class='row'>
                        <div class='col-sm-6'>
                            <div class='row m-t-xs'>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Today</h5>
                                    <h1 class='no-margins'>{0}</h1>
                                </div>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Current Month</h5>
                                    <h1 class='no-margins'>{1}</h1>
                                </div>
                            </div>
                            <div class='row m-t-xs'>
                                <canvas id='modalChart' height='114'></canvas>
                            </div>
                        </div>
                        <div class='col-sm-6'>
                            <table id='modalTable' class='table table-striped table-bordered'>
                                <thead><tr><th>Machine ID</th><th>Date Time</th><th>Amount</th></tr></thead>
                                <tbody>{2}</tbody>
                            </table>
                        </div>
                    </div>", total, mtdTotal, tableRows.ToString()));

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
                result.html = sb.ToString();
                return result;
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }
        }

         [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetCPMTransactionDetail()
        {
            StringBuilder sb = new StringBuilder();
            SqlCommand cmd = null;
            StringBuilder tableRows = new StringBuilder();
            LogableTask task = LogableTask.NewTask("GetCPMTransactionDetail");
            try
            {
                DateTime todayShortDate = DateTime.Today;
                string todayShortDatString = DateTime.Today.ToShortDateString();
                int monthsToCheckBack = 6;

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT trxn_datetime, deposit_amount AS amount, title 
                                                FROM vEjParsedCPMTransaction t, atm 
                                                WHERE t.atm_id = atm.ATM_id
                                                AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = '{2}' and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")",
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessfulString);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    tableRows.Append("<tr><td>" + dr["title"] + "</td><td>" + dr["trxn_datetime"] + "</td><td>" + dr["amount"] + "</td></tr>");
                }

                int total = dt.Rows.Count;
                object mtdTotal = ConnectionFactory.ExecuteScalar(string.Format("SELECT COUNT(*) FROM vEjParsedCPMTransaction where trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = '{2}' and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")", 
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessfulString));

                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT Month(trxn_datetime) m, Year(trxn_datetime) y, COUNT(*) total FROM vEjParsedCPMTransaction 
                                        where trxn_datetime >= convert(datetime,'{0}',103)      
                                        and trxn_datetime <= convert(datetime,'{1}',103)   
                                        AND status = '{2}'
                                        and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + 
                                        ") group by Month(trxn_datetime), Year(trxn_datetime) ORDER BY y, m",
                                            FromDate.ToString("dd/MM/yyyy"),
                                            ToDate.ToString("dd/MM/yyyy"), todayShortDatString, TransactionSuccessfulString);
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(Convert.ToInt32(dr["total"]));
                    labels.Add(string.Format("{0}/{1}", dr["m"].ToString(), dr["y"].ToString()));
                }

                sb.Append(string.Format(@"
                    <div class='row'>                        
                        <div class='col-sm-6'>
                            <div class='row m-t-xs'>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Today</h5>
                                    <h1 class='no-margins'>{0}</h1>
                                </div>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Current Month</h5>
                                    <h1 class='no-margins'>{1}</h1>
                                </div>
                            </div>
                            <div class='row m-t-xs'>
                                <canvas id='modalChart' height='114'></canvas>
                            </div>
                        </div>
                        <div class='col-sm-6'>
                            <table id='modalTable' class='table table-striped table-bordered'>
                                <thead><tr><th>Machine ID</th><th>Date Time</th><th>Amount</th></tr></thead>
                                <tbody>{2}</tbody>
                            </table>
                        </div>
                    </div>", total, mtdTotal, tableRows.ToString()));

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
                result.html = sb.ToString();
                return result;
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }

        }

         [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetBalanceInquiryDetail()
        {
            StringBuilder sb = new StringBuilder();
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetBalanceInquiryDetail");
            try
            {
                DateTime todayShortDate = DateTime.Today;
                string todayShortDatString = DateTime.Today.ToShortDateString();
                int monthsToCheckBack = 6;

                object total = ConnectionFactory.ExecuteScalar(
                    string.Format(@"SELECT COUNT(*) FROM vEjParsedTransactions t, transaction_type tt WHERE t.transaction_type_id = tt.transaction_type_id AND tt.transaction_type_name LIKE '%bal%' AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = {2} and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")",
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessful));
                
                object mtdTotal = ConnectionFactory.ExecuteScalar(
                    string.Format("SELECT COUNT(*) FROM vEjParsedTransactions t, transaction_type tt WHERE t.transaction_type_id = tt.transaction_type_id AND tt.transaction_type_name LIKE '%bal%' AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) AND status = {2} and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")",
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), TransactionSuccessful));

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT Month(trxn_datetime) m, Year(trxn_datetime) y, COUNT(*) total FROM vEjParsedTransactions t, transaction_type tt
                                        where t.transaction_type_id = tt.transaction_type_id AND tt.transaction_type_name LIKE '%bal%'
                                        AND trxn_datetime >= convert(datetime,'{0}',103)
                                        and trxn_datetime <= convert(datetime,'{1}',103)
                                        AND status = {2}
                                        and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") group by Month(trxn_datetime), Year(trxn_datetime) ORDER BY y, m",
                                            FromDate.ToString("dd/MM/yyyy"),
                                            ToDate.ToString("dd/MM/yyyy"), todayShortDatString, TransactionSuccessful);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(Convert.ToInt32(dr["total"]));
                    labels.Add(string.Format("{0}/{1}", dr["m"].ToString(), dr["y"].ToString()));
                }

                sb.Append(string.Format(@"<div class='col-sm-12'>
                            <div class='row m-t-xs'>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Today</h5>
                                    <h1 class='no-margins'>{0}</h1>
                                </div>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Current Month</h5>
                                    <h1 class='no-margins'>{1}</h1>
                                </div>
                            </div>
                            <div class='row m-t-xs'>
                                <canvas id='modalChart' height='114'></canvas>
                            </div>
                        </div>", total, mtdTotal));

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
                result.html = sb.ToString();
                return result;
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }
        }

         [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetCardlessTransactionDetail()
        {
            StringBuilder sb = new StringBuilder();
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetCardlessTransactionDetail");
            try
            {
                DateTime todayShortDate = DateTime.Today;
                string todayShortDatString = DateTime.Today.ToShortDateString();
                int monthsToCheckBack = 6;

                object total = ConnectionFactory.ExecuteScalar(
                    string.Format(@"SELECT COUNT(*) FROM (
                                        SELECT trxn_datetime, is_cardless FROM vEjParsedTransactions WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") UNION SELECT trxn_datetime, is_cardless FROM vEjParsedBNATransaction WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") UNION  SELECT trxn_datetime, is_cardless FROM vEjParsedCPMtransaction WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")) Transactions WHERE trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103)", FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy")));
                
                object mtdTotal = ConnectionFactory.ExecuteScalar(
                    string.Format(@"SELECT COUNT(*) FROM (
                                        SELECT trxn_datetime, is_cardless FROM vEjParsedTransactions WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") UNION  SELECT trxn_datetime, is_cardless FROM vEjParsedBNATransaction WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") UNION  SELECT trxn_datetime, is_cardless FROM vEjParsedCPMtransaction WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")) Transactions WHERE trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103)", FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy")));

                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT Month(trxn_datetime) m, Year(trxn_datetime) y, COUNT(*) total FROM (
                                            SELECT trxn_datetime FROM vEjParsedTransactions WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") UNION ALL  SELECT trxn_datetime FROM vEjParsedBNATransaction WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") UNION ALL  SELECT trxn_datetime FROM vEjParsedCPMtransaction WHERE is_cardless = 1 and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")) Transactions  WHERE trxn_datetime >= convert(datetime,'{0}',103) AND trxn_datetime <= convert(datetime,'{1}',103)  group by Month(trxn_datetime), Year(trxn_datetime)ORDER BY y, m",
                                            FromDate.ToString("dd/MM/yyyy"),
                                            ToDate.ToString("dd/MM/yyyy"), todayShortDatString);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(Convert.ToInt32(dr["total"]));
                    labels.Add(string.Format("{0}/{1}", dr["m"].ToString(), dr["y"].ToString()));
                }

                sb.Append(string.Format(@"<div class='col-sm-12'>
                            <div class='row m-t-xs'>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Today</h5>
                                    <h1 class='no-margins'>{0}</h1>
                                </div>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Current Month</h5>
                                    <h1 class='no-margins'>{1}</h1>
                                </div>
                            </div>
                            <div class='row m-t-xs'>
                                <canvas id='modalChart' height='114'></canvas>
                            </div>
                        </div>", total, mtdTotal));

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
                result.html = sb.ToString();
                return result;
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }

        }

         [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetOtherTransactionDetail()
        {
            StringBuilder sb = new StringBuilder();
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetOtherTransactionDetail");
            try
            {
                DateTime todayShortDate = DateTime.Today;
                string todayShortDatString = DateTime.Today.ToShortDateString();
                int monthsToCheckBack = 6;

                object total = ConnectionFactory.ExecuteScalar(
                    string.Format(@"SELECT COUNT(*) FROM vEjParsedTransactions t, transaction_type tt  
                                    WHERE t.transaction_type_id = tt.transaction_type_id 
                                    AND ISNULL(is_cardless, 0) = 0 AND status =0
                                    AND tt.transaction_type_name NOT LIKE '%withdraw%'
                                    AND tt.transaction_type_name NOT LIKE '%bal%'
                                    AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")", 
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy")));
                
                object mtdTotal = ConnectionFactory.ExecuteScalar(
                    string.Format(@"SELECT COUNT(*) FROM vEjParsedTransactions t, transaction_type tt  
                                    WHERE t.transaction_type_id = tt.transaction_type_id 
                                    AND ISNULL(is_cardless, 0) = 0 AND status =0
                                    AND tt.transaction_type_name NOT LIKE '%withdraw%'
                                    AND tt.transaction_type_name NOT LIKE '%bal%'
                                    AND trxn_datetime BETWEEN convert(datetime,'{0}',103) AND convert(datetime,'{1}',103) and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")", 
                    FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy")));

                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT CAST(YEAR(trxn_datetime) AS VARCHAR(4)) + '-' + right('00' + CAST(MONTH(trxn_datetime) AS VARCHAR(2)), 2) AS MonthYear, transaction_type_name, COUNT(*) total FROM (
                                                    SELECT t.transaction_type_id, tt.transaction_type_name, trxn_datetime FROM vEjParsedTransactions t, transaction_type tt 
                                                    WHERE t.transaction_type_id = tt.transaction_type_id 
                                                    AND ISNULL(is_cardless, 0) = 0 AND status =0
                                                    AND tt.transaction_type_name NOT LIKE '%withdraw%'
                                                    AND tt.transaction_type_name NOT LIKE '%bal%'
                                                    and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() +
                                                    ")) Transactions where trxn_datetime >= convert(datetime,'{0}',103) and trxn_datetime <= convert(datetime,'{1}',103) group by CAST(YEAR(trxn_datetime) AS VARCHAR(4)) + '-' + right('00' + CAST(MONTH(trxn_datetime) AS VARCHAR(2)), 2), transaction_type_name ORDER BY MonthYear",
                                FromDate.ToString("dd/MM/yyyy"),
                                ToDate.ToString("dd/MM/yyyy"));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<string> labels = new List<string>();

                var data = dt.AsEnumerable().Select(x => new { 
                    MonthYear = x["MonthYear"].ToString(), 
                    transaction_type_name = x["transaction_type_name"].ToString(), 
                    total = Convert.ToInt32(x["total"]) 
                }).ToList();

                var otherTransactionDetailGraphList = new List<OtherTransactionDetailGraph>();
                List<string> transactionNames = data.Select(x => x.transaction_type_name).Distinct().ToList();
                labels = data.Select(x => x.MonthYear.ToString()).Distinct().ToList(); ;
                var random = new Random();

                foreach (string name in transactionNames)
                {
                    OtherTransactionDetailGraph otherTransactionDetailGraph = new OtherTransactionDetailGraph();
                    otherTransactionDetailGraph.label = name;
                    otherTransactionDetailGraph.data = new List<int>();
                    otherTransactionDetailGraph.backgroundColor = String.Format("#{0:X6}", random.Next(0x1000000));

                    foreach (string row in labels)
                        otherTransactionDetailGraph.data.Add(
                            data.Where(x => x.transaction_type_name == name && x.MonthYear == row)
                            .Sum(x => x.total == null ? 0 : x.total));

                    otherTransactionDetailGraphList.Add(otherTransactionDetailGraph);
                }

                sb.Append(string.Format(@"<div class='col-sm-12'>
                            <div class='row m-t-xs'>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Today</h5>
                                    <h1 class='no-margins'>{0}</h1>
                                </div>
                                <div class='col-md-6'>
                                    <h5 class='m-b-xs'>Current Month</h5>
                                    <h1 class='no-margins'>{1}</h1>
                                </div>
                            </div>
                            <div class='row m-t-xs'>
                                <canvas id='modalChart' height='114'></canvas>
                            </div>
                        </div>", total, mtdTotal));

                dynamic result = new ExpandoObject();
                result.dataSet = otherTransactionDetailGraphList;
                result.chartLabel = labels;
                result.html = sb.ToString();
                return result;
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetAtmCdmAvailabilityDetail(int type = 0)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetTotalAvailabilityByMonth");
            try
            {
                double mtd = 0;
                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandTimeout = 500;
                cmd.CommandText = "GetTotalAvailabilityByMonth";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@startdate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = FromDate;
                cmd.Parameters.Add("@enddate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = ToDate;
                cmd.Parameters.Add("@atm_type", SqlDbType.Int);
                cmd.Parameters[2].Value = type;
                cmd.Parameters.Add("@atmIDs", SqlDbType.VarChar);
                cmd.Parameters[3].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString(); ;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<double> data = new List<double>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToInt32(dr["rMonth"].ToString()) == DateTime.Today.Month && 
                        Convert.ToInt32(dr["rYear"].ToString()) == DateTime.Today.Year)
                        mtd = Math.Floor(Convert.ToDouble(dr["average"]));
                    data.Add(Math.Floor(Convert.ToDouble(dr["average"])));
                    labels.Add(string.Format("{0}/{1}", dr["rMonth"].ToString(), dr["rYear"].ToString()));
                }

                builder.Append(string.Format(@"<div class='col-sm-12'>
                            <div class='row m-t-xs'>
                                <div class='col-md-12'>
                                    <h5 class='m-b-xs'>Current Month</h5>
                                    <h1 class='no-margins'>{0}%</h1>
                                </div>
                            </div>
                            <div class='row m-t-xs'>
                                <canvas id='modalChart' height='114'></canvas>
                            </div>
                        </div>", mtd));

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
                result.html = builder.ToString();
                return result;
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }
        }

         [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetAtmDownDetail()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder tableRows = new StringBuilder();
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetAtmDownDetail");

            try
            {
                DateTime todayShortDate = DateTime.Today;
                string todayShortDatString = DateTime.Today.ToShortDateString();

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTMDandMachineDownBeyondNHoursDetailByAlertType";
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = FromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = ToDate;
                cmd.Parameters.Add("@atmIDs", SqlDbType.VarChar);
                cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                cmd.Parameters.Add("@alertType", SqlDbType.Int);
                cmd.Parameters[3].Value = 68;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                bool isExecuted = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime tokenValue = DateTime.ParseExact(dt.Rows[i]["token_value"].ToString(), "dd/MM/yyyy HH:mm:ss", null);
                    DateTime currentDate = DateTime.Now;

                    int hours = Convert.ToInt32((currentDate - tokenValue).TotalHours);
                    dt.Rows[i]["duration"] = hours;
                    if ((i + 1) < dt.Rows.Count)
                    {
                        if (dt.Rows[i + 1]["token_name"].ToString() == "TicketNo")
                        {
                            dt.Rows[i]["TicketNo"] = dt.Rows[i + 1]["token_value"];
                            isExecuted = true;
                        }
                        else
                            isExecuted = false;
                    }

                    tableRows.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"] + "</td><td>" + dt.Rows[i]["mail_subject"] + "</td><td>" + dt.Rows[i]["token_value"] + "</td><td>" + dt.Rows[i]["duration"] + "</td><td>" + dt.Rows[i]["TicketNo"] + "</td></tr>");
                    if (isExecuted == true)
                        i++;
                }

                sb.Append(string.Format(@"<table id='modalTable' class='table table-striped table-bordered'>
                                    <thead><tr><th>Machine ID</th><th>Location</th><th>Description</th><th>Start Time</th><th>Duration(Hours)</th><th>Ticket No</th></tr></thead>
                                    <tbody>{0}</tbody>
                                </table>", tableRows.ToString()));

                return sb.ToString();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }
        }

         [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static List<CashOutAtm> GetCashOutAtms(){
            SqlCommand cmd = null;
            List<CashOutAtm> result = new List<CashOutAtm>();
            LogableTask task = LogableTask.NewTask("GetCashOutAtms");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"SELECT atm.ATM_id, generated_at, title, longitude, latitude, location  
                                        FROM ( 
                                            SELECT atm_id, MAX(generated_at) generated_at 
                                            FROM atm_alert where resolve_at IS NULL AND alert_type_id = 21
                                            GROUP BY atm_id ) atm_alert 
                                        INNER JOIN atm ON atm_alert.atm_id = atm.ATM_id AND atm.is_active = 1 and atm.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    result.Add(new CashOutAtm()
                    {
                        Title = dr["Title"].ToString(),
                        Lat = Convert.ToDouble(dr["latitude"]),
                        Long = Convert.ToDouble(dr["longitude"]),
                        Location = dr["location"].ToString(),
                        AlertGenerateDate = Convert.ToDateTime(dr["generated_at"]),
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
            }
             
        }
    }

    public class CashOutAtm {
        public string Title { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string Location { get; set; }
        public DateTime AlertGenerateDate { get; set; }
        public string AlertGenerateDateString { get { return AlertGenerateDate.ToShortDateString(); } }
    }

    public class OtherTransactionDetailGraph
    {
        //Naming same as Chart.JS dataset
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public List<int> data { get; set; }
    }
}