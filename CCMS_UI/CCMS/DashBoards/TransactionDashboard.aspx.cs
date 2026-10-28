
using Avanza.iSuite.UI;
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
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class TransactionDashboard : BasePage
    {
        private static int TransactionTypeWithdrawal = 3;
        private static int TransactionSuccessful = 0;
        public static DateTime toDate;
        public static DateTime fromDate;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["to"] != null)
                {
                    string _toDate = Request.QueryString["to"];
                    toDate = DateTime.ParseExact(_toDate, "dd/MM/yyyy HH:mm:ss", null);
                }
                else
                {
                    toDate = DateTime.Now;
                }
                if (Request.QueryString["from"] != null)
                {
                    string _fromDate = Request.QueryString["from"];
                    fromDate = DateTime.ParseExact(_fromDate, "dd/MM/yyyy HH:mm:ss", null);
                }
                else
                {
                    fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                }
                BindGrid();
            }
        }

        private void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
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

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetHourlyTransactionSummary()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetHourlyTransactionSummary");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = "EJTransactionVersusHour";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = fromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = toDate;
                cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(Convert.ToInt32(dr[0]));
                    labels.Add(dr["hr"].ToString());
                }

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;

                var WithdrawalTransactionToday = ConnectionFactory.ExecuteScalar(string.Format("SELECT COUNT(*) FROM vEjParsedTransactions where trxn_datetime BETWEEN '{0}' AND '{1} 23:59:59'",
                    DateTime.Today.ToShortDateString(), DateTime.Today.ToShortDateString()));

                result.WithdrawalTransactionToday = WithdrawalTransactionToday;

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
        public static dynamic OnUsVsOffUsTransactionSummary()
        {
            StringBuilder sb = new StringBuilder();
            LogableTask task = LogableTask.NewTask("OnUsVsOffUsTransactionSummary");
            try
            {
                DateTime todayShortDate = DateTime.Today;
                string todayShortDatString = DateTime.Today.ToShortDateString();

                object total = ConnectionFactory.ExecuteScalar(string.Format("SELECT COUNT(*) FROM vEjParsedTransactions where trxn_datetime BETWEEN '{0}' AND '{1}'",
                    fromDate, toDate));

                object onUs = ConnectionFactory.ExecuteScalar(string.Format(@"select COUNT(*) from vEjParsedTransactions t 
                                    INNER JOIN FITMapping fm 
                                    ON SUBSTRING (t.pan, 1, 6)  = fm.PAN_prefix AND t.pan IS NOT NULL AND fm.on_us = 1
                                    WHERE trxn_datetime BETWEEN '{0}' AND '{1}' and t.atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ")",
                                        fromDate, toDate));
                int offUs = Convert.ToInt32(total) - Convert.ToInt32(onUs);

                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                data.Add(Convert.ToInt32(onUs)); labels.Add("On Us");
                data.Add(offUs); labels.Add("Off Us");

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
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
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetNoTransactionSummary()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("NoTransactionMonitoringSummaryByDateAndATM");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = "NoTransactionMonitoringSummaryByDateAndATM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = fromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = toDate;
                cmd.Parameters.Add("@atmIDs", SqlDbType.VarChar);
                cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(Convert.ToInt32(dr[1]));
                    labels.Add(dr[0].ToString());
                }

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
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
        public static dynamic GetTopTransactingAtms()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;
            LogableTask task = LogableTask.NewTask("GetTopTransactingAtms");
            List<int> data = new List<int>();
            List<string> labels = new List<string>();
            try
            {

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTopNATMsByEjTransaction";
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = fromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = toDate;
                cmd.Parameters.Add("@order", SqlDbType.Int);
                cmd.Parameters[2].Value = 1;
                cmd.Parameters.Add("@atmIds", SqlDbType.VarChar);
                cmd.Parameters[3].Value =  HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                cmd.Parameters.Add("@limit", SqlDbType.Int);
                cmd.Parameters[4].Value = 5;
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(Convert.ToInt32(dr["total"]));
                    labels.Add(dr["title"].ToString());
                }

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
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
        public static dynamic GetTransactionsByStatus()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;
            LogableTask task = LogableTask.NewTask("GetTransactionsByStatus");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(
                    @"SELECT status, COUNT(*) as total FROM vEjParsedTransactions
                        WHERE trxn_datetime BETWEEN '{0}' AND '{1}' and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") GROUP BY status", fromDate, toDate);
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    labels.Add(((Avanza.CCMS.DAL.TransactionStatus)dr["status"]).ToString());
                    data.Add(Convert.ToInt32(dr["total"]));
                }

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
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
        public static dynamic GetTransactionsByType()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;
            LogableTask task = LogableTask.NewTask("GetTransactionsByType");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"SELECT transaction_type_name, COUNT(*) as total 
                                    FROM (
	                                    SELECT transaction_type_id, trxn_datetime, atm_id FROM vEjParsedTransactions WHERE status = 0
	                                    UNION ALL
	                                    SELECT transaction_type_id, trxn_datetime, atm_id FROM vEjParsedBNATransaction WHERE status = 'Successful'
	                                    UNION ALL
	                                    SELECT transaction_type_id, trxn_datetime, atm_id FROM vEjParsedCPMtransaction WHERE status = 'Successful'
                                    ) A, transaction_type
                                    WHERE A.transaction_type_id = transaction_type.transaction_type_id AND trxn_datetime BETWEEN '{0}' AND '{1}' and atm_id in (" + HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString() + ") GROUP BY transaction_type_name", fromDate, toDate);
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                List<int> data = new List<int>();
                List<string> labels = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    labels.Add((dr["transaction_type_name"]).ToString());
                    data.Add(Convert.ToInt32(dr["total"]));
                }

                dynamic result = new ExpandoObject();
                result.chartData = data;
                result.chartLabel = labels;
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
        public static string GetNoTransactionAtmDetail()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetNoTransactionMonitoringDetailsByDateANDAtm");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = "GetNoTransactionMonitoringDetailsByDateANDAtm";
                cmd.Parameters.Add("@ForDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add("@order", SqlDbType.Int);
                cmd.Parameters[1].Value = 1;
                cmd.Parameters.Add("@atmIds", SqlDbType.VarChar);
                cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                cmd.Parameters[3].Value = 31;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                StringBuilder sb = new StringBuilder();
                StringBuilder tableRows = new StringBuilder();

                foreach (DataRow dr in dt.Rows)
                {
                    tableRows.Append(string.Format(@"<tr>
                                        <td>{0}</td>
                                        <td>{1}</td>
                                        <td>{2}</td>
                                        <td>{3}</td>
                                        <td>No Transaction Monitoring</td>           
                                    </tr>",
                                          dr["title"],
                                          dr["location"],
                                          dr["lasttrxn"],
                                          GetInactivityPeriodCount((DateTime)dr["generated_at"], Convert.ToInt32(dr["inactivityduration"]))));
                }

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                cmd.Parameters[0].Value = 38;

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    tableRows.Append(string.Format(@"<tr>
                                        <td>{0}</td>
                                        <td>{1}</td>
                                        <td>{2}</td>
                                        <td>{3}</td>
                                        <td>BNA Inactivity Period Elapsed</td>           
                                    </tr>",
                                          dr["title"],
                                          dr["location"],
                                          dr["lasttrxn"],
                                          GetInactivityPeriodCount((DateTime)dr["generated_at"], Convert.ToInt32(dr["inactivityduration"]))));
                }

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                cmd.Parameters[0].Value = 39;

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    tableRows.Append(string.Format(@"<tr>
                                        <td>{0}</td>
                                        <td>{1}</td>
                                        <td>{2}</td>
                                        <td>{3}</td>
                                        <td>Cheque Inactivity Period Elapsed</td>           
                                    </tr>",
                                          dr["title"],
                                          dr["location"],
                                          dr["lasttrxn"],
                                          GetInactivityPeriodCount((DateTime)dr["generated_at"], Convert.ToInt32(dr["inactivityduration"]))));
                }

                sb.Append(string.Format(@"<div class='col-sm-12'>
                            <div class='row m-t-xs'>
                                <table id='modalTable' class='table table-striped table-bordered'>
                                    <thead>
                                        <tr>
                                            <th>Title</th>
                                            <th>Location</th>
                                            <th>Last Transaction At</th>
                                            <th>Inactivity Duration</th>
                                            <th>Type</th>
                                        </tr>
                                    </thead>
                                    <tbody>{0}</tbody>
                                </table>
                            </div>
                        </div>", tableRows.ToString()));

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

        public static int GetInactivityPeriodCount(DateTime generationTime, int inactivityPeriod)
        {

            DateTime startTime = generationTime;
            DateTime endTime = DateTime.Now; //DateTime.Now.AddSeconds( 75 );
            TimeSpan span = endTime.Subtract(startTime);
            int hours = (span.Days * 24) + span.Hours;
            hours = (inactivityPeriod / 3600) == 0 ? 0 : hours / (inactivityPeriod / 3600);
            return hours + 1;
        }
    }
}