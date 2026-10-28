
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
    public partial class CashDashboard : BasePage
    {
        public static DateTime toDate;
        public static DateTime fromDate;
        

        protected void Page_Load(object sender, EventArgs e)
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
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static int GetMachinesNotReplenishedSummary()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetMachinesNotReplenishedSummary");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetNoReplenishmentsForMorethan10Days";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return Convert.ToInt32(dt.Rows[0][0]);
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
        public static dynamic GetCashOutSummary()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetCashMonitoringSummaryByDateAndAtm");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetCashMonitoringSummaryByDateAndAtm";
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = fromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = toDate;
                cmd.Parameters.Add("@atmIDs", SqlDbType.VarChar);
                cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dynamic result = new ExpandoObject();
                result.cashOut = 0;
                result.sla = 0;
                if (dt.Rows.Count > 0)
                {
                    result.cashOut = dt.Rows[0][2];
                    result.sla = dt.Rows[0][1];
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

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public static dynamic GetReplenishmentsByDay()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetReplenishmentsByDay");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Replenishments";
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
                    labels.Add(dr[1].ToString());
                    data.Add(Convert.ToInt32(dr[0]));
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

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetMachinesNotReplenishedDetail()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetMachinesNotReplenishedDetail");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetNoReplenishmentsForMorethan10DaysDetail";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                var builder = new StringBuilder();
                builder.Append("<table id='modalTable' class=\"table table-striped table-bordered\"><thead><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Current Cash Position</th><th scope=\"col\">Last Replenishment Date</th><th scope=\"col\">Next Replenishment Date</th></tr></thead><tbody>");
                foreach (DataRow dr in dt.Rows)
                {
                    builder.Append("<tr><td>" + dr["title"] + "</td><td>" + dr["location"].ToString().Replace("'", "") + "</td><td>" + dr["total"] + "</td><td>" + dr["last_replenished_at"] + "</td><td>" + dr["next_replenished_at"] + "</td></tr>");
                }

                builder.Append("</tbody></table>");
                return builder.ToString();
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
        public static string GetCashOutDetail()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetCashOutDetail");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetCashMonitoringSummaryDetail";
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = fromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = toDate;
                cmd.Parameters.Add("@atmIDs", SqlDbType.VarChar);
                cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                var builder = new StringBuilder();
                builder.Append("<table id='modalTable' class='table table-striped table-bordered'><thead><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Address</th><th scope=\"col\">Inservice</th><th scope=\"col\">OOS</th><th scope=\"col\">Downtime</th><th scope=\"col\">Supervisor</th><th scope=\"col\">Cash Out</th> <th scope=\"col\">Comm</th><th scope=\"col\">FLM P1</th><th scope=\"col\">Host Down</th><th scope=\"col\">SLM P1</th><th scope=\"col\">Date</th></tr></thead><tbody>");
                foreach (DataRow dr in dt.Rows)
                {
                    builder.Append("<tr><td>" + dr["title"] + "</td><td>" + dr["address"] + "</td><td>" + dr["in_service"] + "</td><td>" + dr["out_of_service"] + "</td><td>" + dr["total_downtime"] + "</td><td>" + dr["in_supervisor"] + "</td><td>" + dr["cash_out"] + "</td><td>" + dr["comms_failure"] + "</td><td>" + dr["flm_p1"] + "</td><td>" + dr["host_down"] + "</td><td>" + dr["slm_p1"] + "</td><td>" + dr["report_date"] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
                return builder.ToString();
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


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetMinOperatingBalanceDetail()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetMinOperatingBalanceDetail");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetMinOperatingBalanceDetail";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                var builder = new StringBuilder();
                builder.Append("<table id='tblLowBalance' class='table table-striped table-bordered'><thead><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Min Threshold</th><th scope=\"col\">Current Cash Position</th><th scope=\"col\">Last Replenishment Date</th><th scope=\"col\">Next Replenishment Date</th></tr></thead><tbody>");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"].ToString().Replace("'", "") + "</td><td>" + dt.Rows[i]["min_operating_balance"] + "</td><td>" + dt.Rows[i]["currentBalance"] + "</td><td>" + dt.Rows[i]["rep_datetime"] + "</td><td>" + dt.Rows[i]["next_replenished_at"] + "</td></tr>");
                }

                return builder.ToString();
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

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public static string GetReplenishmentsByDayDetail()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("GetReplenishmentsByDayDetail");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ReplenishmentsDetail";
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = fromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = toDate;
                cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                cmd.Parameters[2].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                var builder = new StringBuilder();
                builder.Append("<table id='modalTable' class='table table-striped table-bordered'><thead><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">WeekDay</th><th scope=\"col\">Replenishment Date Time</th><th scope=\"col\">Count</th></tr></thead><tbody>");
                foreach (DataRow dr in dt.Rows)
                {
                    builder.Append("<tr><td>" + dr["Title"] + "</td><td>" + dr["WeekDay"] + "</td><td>" + dr["ReplenishedDatetime"] + "</td><td>" + dr["Count"] + "</td></tr>");
                }

                return builder.ToString();
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
}