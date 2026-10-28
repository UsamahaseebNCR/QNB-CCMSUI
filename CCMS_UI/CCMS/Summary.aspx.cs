using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

using System.Reflection;
using System.Diagnostics;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Web.Configuration;
using ComponentArt.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Management;
using System.Security;



namespace CCMSUI.CCMS
{
    public partial class Summary : BasePage
    {
        public static object syncObject = new object();
        //protected void Page_PreInit(object sender, EventArgs e)
        //{
        //    this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        //}
        private void AddToDataTable(DataRow[] src, DataTable dest)
        {
            foreach (DataRow row in src)
            {
                if (!dest.Rows.Contains(row["parsed_transaction_id"]))
                {
                    DataRow dr = dest.NewRow();
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        dr[i] = row.ItemArray[i];
                    }
                    dest.Rows.Add(dr);
                }
            }
        }

        //        private void BindLowBalanceGrid(string order)
        //        {
        //            LogableTask task = LogableTask.NewTask("GetLowBalanceATMs");
        //            try
        //            {
        //                SqlCommand cmd = null;
        //                DataTable dt = new DataTable();
        //                cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //                cmd.CommandTimeout = 600;
        //                cmd.CommandText = @"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
        //                                  select title, min_operating_balance, generated_at,alert_text,resolved_at,
        //                                note_set_type.denomination_type_1 * cassette1_notes +
        //note_set_type.denomination_type_2 * cassette2_notes +
        //note_set_type.denomination_type_3 * cassette3_notes +
        //note_set_type.denomination_type_4 * cassette4_notes cbalance
        //
        //                                   from ccms_integrated_alert,atm outerATM,cash_position,note_set_type
        //                                   where ccms_integrated_alert.entity_id = outerATM.atm_id
        //                                    and cash_position.atm_id = outerATM.atm_id
        //                                    and note_set_type.note_set_type_id = outerATM.note_set_type_id
        //                                    and alert_type_id =20 and outerATM.is_active =1  " +
        //                                    " and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " +
        //                                   " and ((expiration_time>getdate() and resolved_at is not null) " +
        //                                        " or resolved_at is null) " +

        //                                   "  and last_trxn_at = (select max(last_trxn_at) from cash_position where atm_id = outerATM.atm_id)";


        //                new SqlDataAdapter(cmd).Fill(dt);
        //                DataRow[] currentDayLowBalancedRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
        //                DataRow[] previousDayLowBalancedRows = dt.Select("generated_at < #" + DateTime.Today + "#");
        //                DataTable dtLowBalanceCurrentDay = this.CreateDatatableFromDataRows(currentDayLowBalancedRows);
        //                dtLowBalanceCurrentDay.DefaultView.Sort = order;
        //                this.GridView_LowBalance_CurrentDay.DataSource = dtLowBalanceCurrentDay;
        //                this.GridView_LowBalance_CurrentDay.DataBind();
        //                DataTable dtLowBalancePreviousDay = this.CreateDatatableFromDataRows(previousDayLowBalancedRows);
        //                dtLowBalancePreviousDay.DefaultView.Sort = order;
        //                this.GridView_LowBalance_PreviousDay.DataSource = dtLowBalancePreviousDay;
        //                this.GridView_LowBalance_PreviousDay.DataBind();
        //            }
        //            catch (Exception ex)
        //            {
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //            }
        //            finally
        //            {
        //                task.EndTask();
        //            }
        //        }

        private DataTable CreateDatatableFromDataRows(DataRow[] dr)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("min_operating_balance", typeof(decimal));
            dt.Columns.Add("generated_at", typeof(DateTime));
            dt.Columns.Add("type1", typeof(int));
            dt.Columns.Add("type2", typeof(int));
            dt.Columns.Add("type3", typeof(int));
            dt.Columns.Add("type4", typeof(int));
            dt.Columns.Add("balance", typeof(decimal));
            dt.Columns.Add("resolve_at", typeof(DateTime));
            dt.Columns.Add("cbalance", typeof(decimal));
            foreach (DataRow row in dr)
            {
                DataRow temp = dt.NewRow();
                temp[0] = row[0].ToString();
                temp[1] = decimal.Parse(row[1].ToString());
                temp[2] = DateTime.Parse(row[2].ToString());

                if (row[3].ToString().Length > 0)
                {
                    string[] parts = row[3].ToString().Split(new char[] { ',' });
                    temp["type1"] = parts[0];
                    temp["type2"] = parts[1];
                    temp["type3"] = parts[2];
                    temp["type4"] = parts[3];
                    temp["balance"] = parts[7];
                }
                if (row[4].ToString().Length > 0)
                    temp["resolve_at"] = DateTime.Parse(row[4].ToString());
                temp["cbalance"] = decimal.Parse(row[5].ToString());
                dt.Rows.Add(temp);
            }
            return dt;
        }

        private DataTable CreatePurgedDatatableFromDataRows(DataRow[] dr)
        {
            string[] ConfiguredCassettes = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(new char[] { ',' });
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("generated_at", typeof(DateTime));
            dt.Columns.Add("type1", typeof(int));
            dt.Columns.Add("type2", typeof(int));
            dt.Columns.Add("type3", typeof(int));
            dt.Columns.Add("type4", typeof(int));
            dt.Columns.Add("threshold1", typeof(int));
            dt.Columns.Add("threshold2", typeof(int));
            dt.Columns.Add("threshold3", typeof(int));
            dt.Columns.Add("threshold4", typeof(int));
            foreach (DataRow row in dr)
            {
                DataRow temp = dt.NewRow();
                temp[0] = row[0].ToString();
                temp[1] = DateTime.Parse(row[1].ToString());
                if (row[2].ToString().Length > 0)
                {
                    string[] parts = row[2].ToString().Split(new char[] { ',' });
                    if (ConfiguredCassettes[0] == "1")
                    {
                        temp["type1"] = parts[1];
                        temp["threshold1"] = parts[2];
                    }
                    if (ConfiguredCassettes[1] == "1")
                    {
                        temp["type2"] = parts[4];
                        temp["threshold2"] = parts[5];
                    }
                    if (ConfiguredCassettes[2] == "1")
                    {
                        temp["type3"] = parts[7];
                        temp["threshold3"] = parts[8];
                    }
                    if (ConfiguredCassettes[3] == "1")
                    {
                        temp["type4"] = parts[10];
                        temp["threshold4"] = parts[11];
                    }
                }
                dt.Rows.Add(temp);
            }
            return dt;
        }

        protected void DialogCallBack_Callback(object sender, CallBackEventArgs e)
        {
            Exception ex;
            LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
            StringBuilder builder = new StringBuilder();
            SqlCommand cmd = null;
            try
            {

                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 0;
                bool isATM = false;
                bool isOrg = false;
                if (e.Parameters[2].ToString() == "0")
                {
                    if (e.Parameters[1].ToString() == EntityType.ATM.ToString())
                    {
                        cmd.CommandText = "select alert_type,outerATM.title,outerATM.location,generated_at,resolved_at\r\n                                from ccms_integrated_alert inner join atm outerATM\r\n                                on ccms_integrated_alert.entity_id = outerATM.atm_id where outerATM.is_active = 1 \r\n                                and generated_at>=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + "',103)  and generated_at<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103)  and alert_type_id = " + e.Parameters[0];
                        builder.Append("<tr><td><b>Alert Type</td><td><b>Title</b></td><td><b>Location</b></td><td><b>Generated At</b></td><td><b>Resolved At</b></td></tr>");
                        isATM = true;
                    }
                    else if (e.Parameters[1].ToString() == EntityType.Organization.ToString())
                    {
                        cmd.CommandText = "select alert_type,outerRegion.region_name,generated_at\r\n                                from ccms_integrated_alert inner join region outerRegion\r\n                                on ccms_integrated_alert.entity_id = outerRegion.region_id where\r\n                                generated_at>=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + "',103)  and generated_at<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103)  and alert_type_id = " + e.Parameters[0] + " and generated_at = (select MAX(generated_at) from  ccms_integrated_alert where entity_id = outerRegion.region_id and alert_type_id = " + e.Parameters[0] + " and generated_at>=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + "',103)  and generated_at<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103))";
                        builder.Append("<tr><td><b>Alert Type</b></td><td><b>Organization</b></td><td><b>Generated At</b></td></tr>");
                        isOrg = true;
                    }
                    else
                    {
                        cmd.CommandText = "select alert_type,generated_at\r\n                                from ccms_integrated_alert \r\n                                where alert_type_id = " + e.Parameters[0] + " and generated_at>=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + "',103)  and generated_at<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103)";
                        builder.Append("<tr><td><b>Alert Type</b></td><td><b>Generated At</b></td></tr>");
                    }
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to fetch records from db");
                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "records fetched");
                        while (reader.Read())
                        {
                            if (isATM)
                            {
                                builder.Append(string.Concat(new object[] { "<tr><td width=10%>", reader[0], "</td><td width=10%>", reader[1], "</td><td width=20%>", reader[2], "</td><td width=30%>", DateTime.Parse(reader[3].ToString()).ToString("dd/MM/yyyy HH:mm:ss"), "</td><td width=30%>", (reader[4] != DBNull.Value ? DateTime.Parse(reader[4].ToString()).ToString("dd/MM/yyyy HH:mm:ss") : ""), "</td></tr>" }));
                            }
                            else if (isOrg)
                            {
                                builder.Append(string.Concat(new object[] { "<tr><td width=10%>", reader[0], "</td><td width=50%>", reader[1], "</td><td width=50%>", DateTime.Parse(reader[2].ToString()).ToString("dd/MM/yyyy HH:mm:ss"), "</td></tr>" }));
                            }
                            else
                            {
                                builder.Append(string.Concat(new object[] { "<tr><td width=10%>", reader[0], "</td><td width=90%>", DateTime.Parse(reader[1].ToString()).ToString("dd/MM/yyyy HH:mm:ss"), "</td></tr>" }));
                            }
                        }
                        reader.Close();
                        e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>');</script>");
                    }
                    catch (Exception exception1)
                    {
                        ex = exception1;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    finally
                    {
                        task.EndTask();
                    }
                }
                else
                {
                    try
                    {
                        List<SqlParameter> listParam = new List<SqlParameter>();
                                List<object> listValues = new List<object>();

                        DataTable dt = null;
                        StringBuilder builderATMTitles = new StringBuilder();
                        builder.Append("<tr><td><b>Title</b></td><td><b>Last Transaction</b></td><td><b>Last Task Status</b></td><td><b>Last Invoked</b></td></tr>");
                        string[] parts = e.Parameters[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string str in parts)
                            builderATMTitles.Append("'" + str + "',");
                        builderATMTitles.Append("'-1'");

                        ///<Summary>
                        ///Code edited by Ali Faiyaz Shah on 3rd Sep, 15   18:20
                        ///To show "Last Transaction Time of BNA and CPM Machines" on Summary.aspx
                        if (e.Parameters[0] == "4" || e.Parameters[0] == "5")
                        {
                            //BNA Machines Transactions
                            if (System.Configuration.ConfigurationManager.AppSettings["isEJEnabled"] == "1")
                            {
                                cmd.CommandText = " select title,max(trxn_datetime)  " +
                                                  " from atm  left join ej_parsed_bna_transaction on ej_parsed_bna_transaction.atm_id = atm.atm_id " +
                                                  " where title in (" + builderATMTitles.ToString() + ")" +
                                                  "  group by title ";
                            }
                            else
                            {
                                //cmd.CommandText = " select title,max(last_deposit_at)  " +
                                //                  " from atm  left join parsed_bna_counter  on parsed_bna_counter.atm_id = atm.atm_id " +
                                //                  " where title in (" + builderATMTitles.ToString() + ")" +
                                //                  "  group by title ";
                                listParam = new List<SqlParameter>();
                                 listValues = new List<object>();

                                listParam.Add(new SqlParameter("@Filter", SqlDbType.VarChar));
                                listValues.Add(builderATMTitles.ToString());

                                dt = GetDataTableFromStoredProcedure("GetBNALastTransaction", listParam, listValues);
                            }
                        }

                        else if (e.Parameters[0] == "6" || e.Parameters[0] == "7")
                        {
                            //Cheque Machinces Transaction
                            if (System.Configuration.ConfigurationManager.AppSettings["isEJEnabled"] == "1")
                                cmd.CommandText = " select title,max(trxn_datetime)  " +
                                                  " from atm  left join ej_parsed_cpm_transaction  on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                                                  " where title in (" + builderATMTitles.ToString() + ")" +
                                                  "  group by title ";

                            else
                            {

                                listParam = new List<SqlParameter>();
                                  listValues = new List<object>();

                                listParam.Add(new SqlParameter("@Filter", SqlDbType.VarChar));
                                listValues.Add(builderATMTitles.ToString());

                                dt = GetDataTableFromStoredProcedure("GetCPMLastTransaction", listParam, listValues);
                            }
                                //cmd.CommandText = " select title,max(deposit_at)  " +
                                //                  " from atm  left join parsed_cpm_counter  on parsed_cpm_counter.atm_id = atm.atm_id " +
                                //                  " where title in (" + builderATMTitles.ToString() + ")" +
                                //                  "  group by title ";

                        }

                        else
                        {
                            //BNA No Transacting
                            if (System.Configuration.ConfigurationManager.AppSettings["isEJEnabled"] == "1")
                                cmd.CommandText = " select title,max(trxn_datetime)  " +
                                                  " from atm  left join ej_parsed_transactions  on ej_parsed_transactions.atm_id = atm.atm_id " +
                                                  " where title in (" + builderATMTitles.ToString() + ")" +
                                                  "  group by title ";

                            else
                            {
                                listParam = new List<SqlParameter>();
                                listValues= new List<object>();

                                listParam.Add(new SqlParameter("@Filter", SqlDbType.VarChar));
                                listValues.Add(builderATMTitles.ToString());

                                dt = GetDataTableFromStoredProcedure("GetATMsLastTransaction", listParam, listValues);
                                     
                                //cmd.CommandText = " select title,max(trxn_datetime)  " +
                                //                  " from atm  left join parsed_transaction  on parsed_transaction.atm_id = atm.atm_id " +
                                //                  " where title in (" + builderATMTitles.ToString() + ")" +
                                //                  "  group by title ";

                            }

                        }
                        ///</Summary>

                        if (dt == null)
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            adapter.Fill(dt);
                        }

                        listParam = new List<SqlParameter>();
                        listValues = new List<object>();

                        listParam.Add(new SqlParameter("@Filter", SqlDbType.VarChar));
                        listValues.Add(builderATMTitles.ToString());

                        DataTable dt1 = GetDataTableFromStoredProcedure("GetLatestTaskStatus", listParam, listValues);

                        //cmd.CommandText = " select title, status,last_invoked " +
                        //                 "   from atm outerATM left join task on outerATM.atm_id = task.atm_id " +
                        //                 "   and outerATM.title  in (" + builderATMTitles + ")" +
                        //                 "  and task_id in (select MAX(task_id) from task where atm_id = outerATM.atm_id and last_invoked is not null) ";

                        //SqlDataAdapter adapter1 = new SqlDataAdapter(cmd);
                        //DataTable dt1 = new DataTable();
                        //adapter1.Fill(dt1);



                        foreach (string str in parts)
                        {
                            builder.Append("<tr><td>" + str + "</td><td>");
                            DataRow[] drArray = dt.Select("title='" + str + "'");
                            if (drArray.Length > 0)
                            {
                                if (drArray[0][1] != DBNull.Value)
                                    builder.Append(DateTime.Parse(drArray[0][1].ToString()));
                                else
                                    builder.Append("");
                            }
                            else
                                builder.Append("");


                            builder.Append("</td>");


                            drArray = dt1.Select("title='" + str + "'");
                            if (drArray.Length > 0)
                            {
                                builder.Append("<td>" + drArray[0][1] + "</td><td>" + drArray[0][2] + "</td></tr>");
                            }
                            else
                                builder.Append("<td></td><td></td></tr>");


                        }
                        e.Output.Write("<script> receiveCallBackResult1('<table border=1 width=100%>" + builder.ToString() + "</table>');</script>");
                    }
                    catch (Exception exception2)
                    {
                        ex = exception2;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    finally
                    {
                        task.EndTask();
                    }
                }
            }
            catch (Exception ex1)
            {
                LogableTask.LogMonoActivityTask("DialogCallBack_Callback", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex1);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }
        private DataTable GetDataTableFromStoredProcedure(string storedProcedureName, List<SqlParameter> sqlParameters, List<object> values )
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = storedProcedureName;
            cmd.CommandType = CommandType.StoredProcedure;

            for (int i=0;i< sqlParameters.Count;i++) 
            {
                cmd.Parameters.Add(sqlParameters[i]);
                cmd.Parameters[i].Value = values[i];
            }             
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        private SqlDataReader GetReaderFromStoredProcedure(string storedProcedureName, List<SqlParameter> sqlParameters, List<object> values)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = storedProcedureName;
            cmd.CommandType = CommandType.StoredProcedure;

            for (int i = 0; i < sqlParameters.Count; i++)
            {
                cmd.Parameters.Add(sqlParameters[i]);
                cmd.Parameters[i].Value = values[i];
            }
            return cmd.ExecuteReader();
         }
        private void DisplayCallOutSummary(ref SqlCommand cmd)
        {
            SqlDataReader reader = null;
            LogableTask task = LogableTask.NewTask("DisplayCallOutSummary");
            //SqlCommand cmd = null;
            //DataTable dt = new DataTable();
            //dt.Columns.Add("activity");
            //dt.Columns.Add("count");
            try
            {
                lock (syncObject)
                {
                    if (Cache["DisplayCallOutSummary"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        StringBuilder builder = new StringBuilder();
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        //if (cmd.Connection != null)
                        //    if (cmd.Connection.State != ConnectionState.Open)
                        //        cmd.Connection.Open();
                        cmd.CommandType = CommandType.Text;
                        //cmd = ConnectionFactory.GetNewCommand(true);
                        //cmd.CommandTimeout = 600;

                        cmd.CommandText = @"select COUNT(flm_monitoring_id) from flm_monitoring
                              where activity_datetime >=convert(datetime,'" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy") + "',103) and activity_datetime<=convert(datetime,'"
                       + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and ATM_ID_NO IN (SELECT TITLE FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")); select COUNT(flm_monitoring_id) from flm_monitoring  where time_of_call_to_bank is null  and activity_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and activity_datetime<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and ATM_ID_NO IN (SELECT TITLE FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "));  select COUNT(flm_monitoring_id) from flm_monitoring  where time_of_solving is null and activity_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and activity_datetime<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and ATM_ID_NO IN (SELECT TITLE FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "));  select max(activity_datetime) from flm_monitoring WHERE ATM_ID_NO IN (SELECT TITLE FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "))";
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            this.Label_CurrentMonthCalls.Text = reader[0].ToString();
                            builder.Append(Label_CurrentMonthCalls.Text + ";");
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            this.Label_NotReportedToBank.Text = reader[0].ToString();
                            builder.Append(Label_NotReportedToBank.Text + ";");
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            this.Label_PendingCalls.Text = reader[0].ToString();
                            builder.Append(Label_PendingCalls.Text + ";");
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            if (reader[0] != DBNull.Value)
                            {
                                this.Label_LastActivityAt.Text = DateTime.Parse(reader[0].ToString()).ToString("dd/MM/yyyy HH:mm:ss");
                                builder.Append(Label_LastActivityAt.Text + ";");
                            }
                            else
                                this.Label_LastActivityAt.Text = "No records imported";
                        }
                        reader.Close();
                        if (builder.Length > 0)
                            Cache.Insert("DisplayCallOutSummary", builder.ToString(), null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                    {
                        string val = (string)Cache["DisplayCallOutSummary"];

                        string[] parts = val.Split(';');
                        if (parts.Length > 0)
                        {
                            this.Label_CurrentMonthCalls.Text = parts[0];
                            this.Label_NotReportedToBank.Text = parts[1];
                            this.Label_PendingCalls.Text = parts[2];
                            this.Label_LastActivityAt.Text = parts[3];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                //Want to keep the connection open so that no need to open it again for other functions..
                //if ((cmd != null) && (cmd.Connection != null))
                //{
                //    cmd.Connection.Close();
                //}
                if (reader != null)
                    reader.Close();

                task.EndTask();
            }
        }

        private void DisplayCurrentLoggedInUser()
        {
            LogableTask task = LogableTask.NewTask("DisplayCurrentLoggedInUser");
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("loggedInAt", typeof(DateTime));
            dt.Columns.Add("lastRequestAt", typeof(DateTime));
            try
            {
                List<ActiveSessions> list = (List<ActiveSessions>)base.Application["ActiveSessions"];
                foreach (ActiveSessions session in list)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = session.userID;
                    dr[1] = session.loggedInAt;
                    dr[2] = session.lastRequestSentAt;
                    dt.Rows.Add(dr);
                }
                this.GridView_CurrentLoggedInUser.DataSource = dt;
                this.GridView_CurrentLoggedInUser.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        private void DisplayCurrentOrder(ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("DisplayCurrentOrder");
            DataTable dt = null;
            try
            {
                //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //cmd.CommandTimeout = 600;
                lock (syncObject)
                {
                    if (Cache["DisplayCurrentOrder"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {

                        cmd.CommandText = "GetCurrentOrderList";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                        cmd.Parameters[0].Value = DateTime.Today;
                        cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                        cmd.Parameters[1].Value = DateTime.Today;
                        cmd.Parameters.Add(new SqlParameter("UserId", SqlDbType.Int));
                        cmd.Parameters[2].Value = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        Cache.Insert("DisplayCurrentOrder", dt, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                        dt = (DataTable)Cache["DisplayCurrentOrder"];
                }

                this.GridView_CurrentOrders.DataSource = dt;
                this.GridView_CurrentOrders.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private void DisplayHaltedATMs(ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("DisplayHaltedATMs");
            DataTable dt = null;
            try
            {
                //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //cmd.CommandTimeout = 600;
                lock (syncObject)
                {
                    if (Cache["DisplayHaltedATMs"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        cmd.CommandText = "select title,atm_id from atm where is_dff_generation_halt =1 and is_active = 1 and atm_id in (select atm_id from user_atms where user_id=" + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId + ")";
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        Cache.Insert("DisplayHaltedATMs", dt, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                        dt = (DataTable)Cache["DisplayHaltedATMs"];
                }
                this.GridView_ATM.DataSource = dt;
                this.GridView_ATM.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private void DisplayMessage()
        {
            try
            {
                this.Page.Response.Write("<DIV ID='loadingScreen'><TABLE  WIDTH='100%' HEIGHT='100%' BORDER='0'><TR><TD valign = 'top'><font size='3' COLOR='black'>Loading... Please wait</font><p><IMG SRC='../images/loading.gif' BORDER='0'></p></TD></TR></TABLE></DIV>");
                this.Page.Response.Flush();
            }
            catch (Exception)
            {
            }
        }

        private void DisplaySummaryOfTodaysAlert(ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("DisplaySummaryOfTodaysAlert");
            DataTable dt1 = null;

            try
            {
                lock (syncObject)
                {
                    if (Cache["DisplaySummaryOfTodaysAlert"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        List<string> alertTypes = new List<string>();
                        string type = null;

                        int resolvedCount = 0;
                        int unResolvedCount = 0;


                        dt1 = new DataTable();
                        dt1.Columns.Add("task");
                        dt1.Columns.Add("resolved");
                        dt1.Columns.Add("unresolved");
                        dt1.Columns.Add("total");
                        dt1.Columns.Add("alert_type_id");
                        dt1.Columns.Add("entity_type");

                        //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                        //cmd.CommandTimeout = 600;
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        cmd.CommandText = "GetSummary";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                        cmd.Parameters[0].Value = Session[SessionVars.SelectedATMs.ToString()];
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            type = dr["type"].ToString();
                            if (!alertTypes.Contains(type))
                                alertTypes.Add(type);
                        }

                        for (int j = 0; j < alertTypes.Count; j++)
                        {
                            resolvedCount = 0;
                            unResolvedCount = 0;

                            DataRow[] drArray = dt.Select("type='" + alertTypes[j] + "'");
                            for (int k = 0; k < drArray.Length; k++)
                            {
                                if (drArray[k][4].ToString() == "1")
                                    resolvedCount = int.Parse(drArray[k][1].ToString());
                                else if (drArray[k][4].ToString() == "0")
                                    unResolvedCount = int.Parse(drArray[k][1].ToString());
                            }

                            DataRow dr = dt1.NewRow();
                            dr["task"] = drArray[0][0].ToString();


                            dr["resolved"] = resolvedCount;
                            dr["unresolved"] = unResolvedCount;
                            dr["total"] = resolvedCount + unResolvedCount;

                            dr["alert_type_id"] = drArray[0][2].ToString();
                            dr["entity_type"] = drArray[0][3].ToString();
                            dt1.Rows.Add(dr);

                        }
                        Cache.Insert("DisplaySummaryOfTodaysAlert", dt1, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                        dt1 = (DataTable)Cache["DisplaySummaryOfTodaysAlert"];
                }
                this.GridView_SummaryOfAlerts.DataSource = dt1;
                this.GridView_SummaryOfAlerts.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private void DisplayUserTask(ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("DisplayUserTask");
            DataTable dt = null;
            try
            {
                //cmd = ConnectionFactory.GetNewCommand(false);
                //cmd.CommandTimeout = 600;
                lock (syncObject)
                {
                    if (Cache["DisplayUserTask"] == null)
                    {
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        cmd.CommandText = "GetUserTask";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                        cmd.Parameters[0].Value = DateTime.Today;
                        cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                        cmd.Parameters[1].Value = DateTime.Today;
                        cmd.Parameters.Add(new SqlParameter("UserId", SqlDbType.Int));
                        cmd.Parameters[2].Value = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        Cache.Insert("DisplayUserTask", dt, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                        dt = (DataTable)Cache["DisplayUserTask"];
                }

                this.GridView_UserTasks.DataSource = dt;
                this.GridView_UserTasks.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }


        private string GetATMsByNoteSetTypeID(string id, ref SqlCommand cmd)
        {
            //|| ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode())
            //Atm.AtmReader reader = null;
            StringBuilder builder = new StringBuilder();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select atm_id from atm where note_set_type_id = " + id + " and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and is_active=1";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
                builder.Append(dr["atm_id"] + ",");
            builder.Append("-1");
            return builder.ToString();
        }


        private void GenerateRemainingNotesGraph(ref SqlCommand cmd)
        {
            // SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("DrawGraph");
            //SqlDataReader reader = null;

            try
            {
                Chart3.Series[0].Points.Clear();
                decimal totalRemaining1 = 0M;
                decimal totalRemaining2 = 0M;
                decimal totalRemaining3 = 0M;
                decimal totalRemaining4 = 0M;
                //cmd = ConnectionFactory.GetNewCommand(true);
                //cmd.CommandTimeout = 600;
                lock (syncObject)
                {
                    string nameToCheck = "GenerateRemainingNotesGraph" + DropDownList_NoteSetType.SelectedItem.Value + CheckBox_AnalysisOnEJ.Checked.ToString();
                    if (Cache[nameToCheck] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        string atms = GetATMsByNoteSetTypeID(DropDownList_NoteSetType.SelectedItem.Value, ref cmd);
                        if (CheckBox_AnalysisOnEJ.Checked)
                            cmd.CommandText = "GetEJRemainingNotes";
                        else
                            cmd.CommandText = "GetRemainingNotes";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("ForDate", SqlDbType.DateTime));
                        cmd.Parameters[0].Value = DateTime.Today;

                        cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                        cmd.Parameters[1].Value = atms;

                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Remaining Notes Procedure");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        //if (cmd.Connection.State != ConnectionState.Open)
                        //    cmd.Connection.Open();
                        //reader = cmd.ExecuteReader();
                        //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Procedure executed");

                        foreach (DataRow dr in dt.Rows)
                        //while (reader.Read())
                        {
                            if (dr[0] != DBNull.Value && dr[4] != DBNull.Value)
                                totalRemaining1 += decimal.Parse(dr[0].ToString()) - decimal.Parse(dr[4].ToString());
                            if (dr[1] != DBNull.Value && dr[5] != DBNull.Value)
                                totalRemaining2 += decimal.Parse(dr[1].ToString()) - decimal.Parse(dr[5].ToString());
                            if (dr[2] != DBNull.Value && dr[6] != DBNull.Value)
                                totalRemaining3 += decimal.Parse(dr[2].ToString()) - decimal.Parse(dr[6].ToString());
                            if (dr[3] != DBNull.Value && dr[7] != DBNull.Value)
                                totalRemaining4 += decimal.Parse(dr[3].ToString()) - decimal.Parse(dr[7].ToString());
                        }
                        //reader.Close();

                        Label_QueryAt.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        Cache.Insert(nameToCheck, totalRemaining1 + "|" + totalRemaining2 + "|" + totalRemaining3 + "|" + totalRemaining4
                            , null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                    {
                        string var = (string)Cache[nameToCheck];
                        string[] parts1 = var.Split('|');

                        totalRemaining1 = int.Parse(parts1[0]);
                        totalRemaining2 = int.Parse(parts1[1]);
                        totalRemaining3 = int.Parse(parts1[2]);
                        totalRemaining4 = int.Parse(parts1[3]);

                    }
                }




                NoteSetType noteSet = NoteSetType.LoadNoteSetTypeByPk(int.Parse(DropDownList_NoteSetType.SelectedItem.Value));
                //int[] validTypes = new int[4];
                //if (noteSet.DenominationType1Title.Length > 0)
                //    validTypes[0] = 1;
                //if (noteSet.DenominationType2Title.Length > 0)
                //    validTypes[1] = 1;
                //if (noteSet.DenominationType3Title.Length > 0)
                //    validTypes[2] = 1;
                //if (noteSet.DenominationType4Title.Length > 0)
                //    validTypes[3] = 1;

                string[] parts = { noteSet.DenominationType1Title, noteSet.DenominationType2Title, noteSet.DenominationType3Title, noteSet.DenominationType4Title };


                this.Chart3.Series["Default"].Points.AddXY(parts[0], new object[] { totalRemaining1 });
                this.Chart3.Series["Default"].Points.AddXY(parts[1], new object[] { totalRemaining2 });
                this.Chart3.Series["Default"].Points.AddXY(parts[2], new object[] { totalRemaining3 });
                this.Chart3.Series["Default"].Points.AddXY(parts[3], new object[] { totalRemaining4 });

                if (parts[0].Length > 0)
                {
                    this.Label_Denomination1.Text = parts[0];
                    this.Label_Quantity1.Text = totalRemaining1.ToString();
                    this.Label_TotalDenomination1.Text = (decimal.Parse(parts[0].Substring(3)) * totalRemaining1).ToString();
                }
                else
                    this.Label_TotalDenomination1.Text = "0";

                if (parts[1].Length > 0)
                {
                    this.Label_Denomination2.Text = parts[1];
                    this.Label_Quantity2.Text = totalRemaining2.ToString();
                    this.Label_TotalDenomination2.Text = (decimal.Parse(parts[1].Substring(3)) * totalRemaining2).ToString();
                }
                else
                    this.Label_TotalDenomination2.Text = "0";

                if (parts[2].Length > 0)
                {
                    this.Label_Denomination3.Text = parts[2];
                    this.Label_Quantity3.Text = totalRemaining3.ToString();
                    this.Label_TotalDenomination3.Text = (decimal.Parse(parts[2].Substring(3)) * totalRemaining3).ToString();
                }
                else
                    this.Label_TotalDenomination3.Text = "0";

                if (parts[3].Length > 0)
                {
                    this.Label_Denomination4.Text = parts[3];
                    this.Label_Quantity4.Text = totalRemaining4.ToString();
                    this.Label_TotalDenomination4.Text = (decimal.Parse(parts[3].Substring(3)) * totalRemaining4).ToString();
                }

                this.Label_TotalRemAmount.Text = (((decimal.Parse(this.Label_TotalDenomination1.Text) + decimal.Parse(this.Label_TotalDenomination2.Text)) + decimal.Parse(this.Label_TotalDenomination3.Text)) + decimal.Parse(this.Label_TotalDenomination4.Text)).ToString();

                this.Chart3.Series["Default"]["ShowMarkerLines"] = "true";
                this.Chart3.Series["Default"]["DrawingStyle"] = "Cylinder";
                this.Chart3.Series["Default"]["PointWidth"] = "0.8";

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                //if (reader != null)
                //    reader.Close();
                //if ((cmd != null) && (cmd.Connection != null))
                //{
                //    cmd.Connection.Close();
                //}
                task.EndTask();
            }

        }
        private void DrawGraph(ref SqlCommand cmd)
        {
            //SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("DrawGraph");
            DataTable dt = null;
            try
            {
                this.Chart2.Series[0].Points.Clear();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Draw Pie");
                this.DrawPie(ref cmd);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Pie created");
                //decimal totalRemaining1 = 0M;
                //decimal totalRemaining2 = 0M;
                //decimal totalRemaining3 = 0M;
                //decimal totalRemaining4 = 0M;
                //cmd = ConnectionFactory.GetNewCommand(true);
                //cmd.CommandTimeout = 600;
                //cmd.CommandText = "GetRemainingNotes";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue(new SqlParameter("ForDate", SqlDbType.DateTime));
                //cmd.Parameters[0].Value = DateTime.Today;

                //cmd.Parameters.AddWithValue(new SqlParameter("atmIDs", SqlDbType.VarChar));
                //cmd.Parameters[1].Value = GetATMsByNoteSetTypeID(DropDownList_NoteSetType.SelectedItem.Value);

                //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Remaining Notes Procedure");
                //SqlDataReader reader = cmd.ExecuteReader();
                //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Procedure executed");
                //while (reader.Read())
                //{
                //    totalRemaining1 += decimal.Parse(reader[0].ToString()) - decimal.Parse(reader[4].ToString());
                //    totalRemaining2 += decimal.Parse(reader[1].ToString()) - decimal.Parse(reader[5].ToString());
                //    totalRemaining3 += decimal.Parse(reader[2].ToString()) - decimal.Parse(reader[6].ToString());
                //    totalRemaining4 += decimal.Parse(reader[3].ToString()) - decimal.Parse(reader[7].ToString());
                //}
                //reader.Close();
                //Label_QueryAt.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                //NoteSetType noteSet = NoteSetType.LoadNoteSetTypeByPk(int.Parse(DropDownList_NoteSetType.SelectedItem.Value));
                //string[] parts = { noteSet.DenominationType1Title, noteSet.DenominationType2Title, noteSet.DenominationType3Title, noteSet.DenominationType4Title };
                //this.Chart3.Series["Default"].Points.AddXY(parts[0], new object[] { totalRemaining1 });
                //this.Chart3.Series["Default"].Points.AddXY(parts[1], new object[] { totalRemaining2 });
                //this.Chart3.Series["Default"].Points.AddXY(parts[2], new object[] { totalRemaining3 });
                //this.Chart3.Series["Default"].Points.AddXY(parts[3], new object[] { totalRemaining4 });
                //this.Label_Denomination1.Text = parts[0];
                //this.Label_Quantity1.Text = totalRemaining1.ToString();
                //this.Label_TotalDenomination1.Text = (decimal.Parse(parts[0].Substring(3)) * totalRemaining1).ToString();
                //this.Label_Denomination2.Text = parts[1];
                //this.Label_Quantity2.Text = totalRemaining2.ToString();
                //this.Label_TotalDenomination2.Text = (decimal.Parse(parts[1].Substring(3)) * totalRemaining2).ToString();
                //this.Label_Denomination3.Text = parts[2];
                //this.Label_Quantity3.Text = totalRemaining3.ToString();
                //this.Label_TotalDenomination3.Text = (decimal.Parse(parts[2].Substring(3)) * totalRemaining3).ToString();
                //this.Label_Denomination4.Text = parts[3];
                //this.Label_Quantity4.Text = totalRemaining4.ToString();
                //this.Label_TotalDenomination4.Text = (decimal.Parse(parts[3].Substring(3)) * totalRemaining4).ToString();
                //this.Label_TotalRemAmount.Text = (((decimal.Parse(this.Label_TotalDenomination1.Text) + decimal.Parse(this.Label_TotalDenomination2.Text)) + decimal.Parse(this.Label_TotalDenomination3.Text)) + decimal.Parse(this.Label_TotalDenomination4.Text)).ToString();
                //this.Chart3.Series["Default"]["ShowMarkerLines"] = "true";
                //this.Chart3.Series["Default"]["DrawingStyle"] = "Cylinder";
                //this.Chart3.Series["Default"]["PointWidth"] = "0.8";
                GenerateRemainingNotesGraph(ref cmd);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Trxn Count Procedure For Current Day");
                lock (syncObject)
                {
                    if (Cache["GetTrxnCount"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        cmd.CommandText = "GetTrxnCount";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                        cmd.Parameters[0].Value = DateTime.Today;
                        cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                        cmd.Parameters[1].Value = DateTime.Now;
                        //if (cmd.Connection.State != ConnectionState.Open)
                        //    cmd.Connection.Open();
                        this.Label_CurrentDayTrxnCount.Text = cmd.ExecuteScalar().ToString();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Trxn Count Procedure For Current Day executed successfully.");
                        Cache.Insert("GetTrxnCount", Label_CurrentDayTrxnCount.Text, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                        this.Label_CurrentDayTrxnCount.Text = (string)Cache["GetTrxnCount"];

                }

                lock (syncObject)
                {
                    if (Cache["GetTrxnCountForYesterday"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                        cmd.Parameters[0].Value = DateTime.Today.AddDays(-1.0);
                        cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                        cmd.Parameters[1].Value = DateTime.Now.AddDays(-1.0);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Trxn Count Procedure For yesterday");
                        this.Label_PreviousDayTrxnCount.Text = cmd.ExecuteScalar().ToString();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Trxn Count Procedure For yesterday executed successfully.");
                        Cache.Insert("GetTrxnCountForYesterday", Label_PreviousDayTrxnCount.Text, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                        this.Label_PreviousDayTrxnCount.Text = (string)Cache["GetTrxnCountForYesterday"];
                }

                lock (syncObject)
                {
                    if (Cache["GetFlmSummaryByCode"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        InitializeCommand(ref cmd);
                        cmd.CommandText = "GetFlmSummaryByCode";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                        cmd.Parameters[0].Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                        cmd.Parameters[1].Value = DateTime.Today;
                        cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                        cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute GetFlmSummaryByCode");
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        Cache.Insert("GetFlmSummaryByCode", dt, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                    {
                        dt = (DataTable)Cache["GetFlmSummaryByCode"];
                    }
                }

                foreach (DataRow dr in dt.Rows)
                    this.Chart2.Series["Default"].Points.AddXY(dr[0], dr[1]);


                this.Chart2.Series["Default"]["ShowMarkerLines"] = "true";
                this.Chart2.Series["Default"]["DrawingStyle"] = "Cylinder";
                this.Chart2.Series["Default"]["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                if ((cmd != null) && (cmd.Connection != null))
                {
                    cmd.Connection.Close();
                }
                task.EndTask();
            }
        }

        private void DrawNotesAcceptorGraph(ref SqlCommand cmd)
        {
            Chart_BNA.Series["Default"].Points.Clear();
            DataTable dt1 = null;
            StringBuilder builderTrxn = new StringBuilder();
            StringBuilder builderNoTrxn = new StringBuilder();
            int totalTransactingATM = 0;
            int totalNotesDepositMachines = 0;
            //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            //cmd.CommandTimeout = 600;

            lock (syncObject)
            {
                if (Cache["DrawNotesAcceptorGraph"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                {
                    InitializeCommand(ref cmd);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetBNAAtmsForNoteAcceptorGraph";
                    cmd.Parameters.AddWithValue("@FromDate", DateTime.Today.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@ToDate", DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59");
                    cmd.Parameters.AddWithValue("@AtmId", Session[SessionVars.SelectedATMs.ToString()]);
                    cmd.Parameters.AddWithValue("@UserId", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId);




                    //@"select distinct(title)
                    //                                from parsed_bna_counter inner join atm 
                    //                                on parsed_bna_counter.atm_id = atm.atm_id 
                    //                                inner join user_ATMs
                    //                                on user_ATMs.atm_id = parsed_bna_counter.atm_id
                    //                                where last_deposit_at>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) " +
                    //                                " and last_deposit_at<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)  " +
                    //                               "  and atm.is_active=1 and user_ATMs.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId +
                    //                                " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ";

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);



                    //            int totalTransactingATM = (int)ConnectionFactory.ExecuteScalar(@"select count(distinct(parsed_bna_counter.atm_id))
                    //                                from parsed_bna_counter inner join atm 
                    //                                on parsed_bna_counter.atm_id = atm.atm_id 
                    //                                inner join user_ATMs
                    //                                on user_ATMs.atm_id = parsed_bna_counter.atm_id
                    //                                where last_deposit_at>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) " +
                    //                                " and last_deposit_at<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)  " +
                    //                               "  and atm.is_active=1 and user_ATMs.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select title from atm where is_active=1 and is_cdm=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ";
                    dt1 = new DataTable();
                    adapter.Fill(dt1);
                    string title = null;
                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {
                        title = dt1.Rows[j][0].ToString();
                        DataRow[] drArray = dt.Select("title='" + title + "'");
                        if (drArray.Length == 0)
                            builderNoTrxn.Append(title + ",");
                        else
                        {
                            builderTrxn.Append(title + ",");
                            totalTransactingATM++;
                        }
                    }
                    Cache.Insert("DrawNotesAcceptorGraph", totalTransactingATM + "|" + builderNoTrxn.ToString() + "|" + builderTrxn.ToString(), null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    Cache["DrawNotesAcceptorGraphDt"] = dt1;
                }
                else
                {
                    string var = (string)Cache["DrawNotesAcceptorGraph"];
                    dt1 = (DataTable)Cache["DrawNotesAcceptorGraphDt"];
                    if (var != null)
                    {
                        string[] parts = var.Split('|');
                        totalTransactingATM = int.Parse(parts[0]);
                        builderNoTrxn.Append(parts[1]);
                        builderTrxn.Append(parts[2]);
                    }
                }
            }


            totalNotesDepositMachines = dt1.Rows.Count;
            //   int totalNotesDepositMachines = (int)ConnectionFactory.ExecuteScalar("select count(atm_id) from atm where is_active=1 and is_cdm=1");

            Chart_BNA.Series["Default"].Points.AddXY("Total Transacting ATMs", totalTransactingATM);
            int diff = totalNotesDepositMachines - totalTransactingATM;
            Chart_BNA.Series["Default"].Points.AddXY("No Transactions", diff);
            Chart_BNA.Series["Default"].ChartType = SeriesChartType.Pie;
            Chart_BNA.Series["Default"]["PieLabelStyle"] = "Outside";
            Chart_BNA.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart_BNA.Series[0]["PieDrawingStyle"] = "SoftEdge";
            Label_BNANoActivity.Text = diff.ToString();
            Label_BNATotalTrxn.Text = totalTransactingATM.ToString();
            Label_totalBNAAtms.Text = totalNotesDepositMachines.ToString();


            if (Label_BNANoActivity.Text != "0")
            {
                this.Label_BNANoActivity.Attributes.Add("onclick", "displayATM(4);");
                this.Label_BNANoActivity.Attributes.Add("style", "text-decoration:underline");
                hidden_BNANoTransactingATM.Value = builderNoTrxn.ToString();
            }
            if (Label_BNATotalTrxn.Text != "0")
            {
                this.Label_BNATotalTrxn.Attributes.Add("onclick", "displayATM(5);");
                this.Label_BNATotalTrxn.Attributes.Add("style", "text-decoration:underline");
                hidden_BNATransactingATM.Value = builderTrxn.ToString();
            }



        }
        private void DrawChequeDepositGraph(ref SqlCommand cmd)
        {
            Chart_ChequeDeposit.Series["Default"].Points.Clear();
            DataTable dt1 = null;
            StringBuilder builderTrxn = new StringBuilder();
            StringBuilder builderNoTrxn = new StringBuilder();
            int totalTransactingATM = 0;
            int totalChequeDepositMachines = 0;
            lock (syncObject)
            {
                if (Cache["DrawChequeDepositGraph"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                {

                    InitializeCommand(ref cmd);
                    cmd.Parameters.Clear();
                    //cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandText = @"select distinct(title) 
                                from parsed_cpm_counter inner join atm 
                                on parsed_cpm_counter.atm_id = atm.atm_id 
                                inner join user_ATMs
                                on user_ATMs.atm_id = parsed_cpm_counter.atm_id
                                where deposit_at>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) " +
                                        " and deposit_at<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)  " +
                                       "  and atm.is_active=1  " +
                                       " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                                        " and user_ATMs.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);



                    cmd.CommandText = "select title from atm where is_active=1 and is_ccdm=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ";
                    dt1 = new DataTable();
                    adapter.Fill(dt1);
                    string title = null;
                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {
                        title = dt1.Rows[j][0].ToString();
                        DataRow[] drArray = dt.Select("title='" + title + "'");
                        if (drArray.Length == 0)
                            builderNoTrxn.Append(title + ",");
                        else
                        {
                            builderTrxn.Append(title + ",");
                            totalTransactingATM++;
                        }
                    }
                    Cache.Insert("DrawChequeDepositGraph", totalTransactingATM + "|" + builderNoTrxn.ToString() + "|" + builderTrxn.ToString(), null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    Cache["DrawChequeDepositGraphDt"] = dt1;
                }
                else
                {
                    dt1 = (DataTable)Cache["DrawChequeDepositGraphDt"];
                    string var = (string)Cache["DrawChequeDepositGraph"];
                    if (var != null)
                    {
                        string[] parts = var.Split('|');
                        totalTransactingATM = int.Parse(parts[0]);
                        builderNoTrxn.Append(parts[1]);
                        builderTrxn.Append(parts[2]);
                    }
                }
            }
            totalChequeDepositMachines = dt1.Rows.Count;





            Chart_ChequeDeposit.Series["Default"].Points.AddXY("Total Transacting ATMs", totalTransactingATM);
            int diff = totalChequeDepositMachines - totalTransactingATM;
            Chart_ChequeDeposit.Series["Default"].Points.AddXY("No Transactions", diff);
            Chart_ChequeDeposit.Series["Default"].ChartType = SeriesChartType.Pie;
            Chart_ChequeDeposit.Series["Default"]["PieLabelStyle"] = "Outside";
            Chart_ChequeDeposit.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart_ChequeDeposit.Series[0]["PieDrawingStyle"] = "SoftEdge";

            Label_ChequeDepositMachinesCount.Text = totalChequeDepositMachines.ToString();
            Label_ChequeNoActivity.Text = diff.ToString();
            Label_ChequeTrxnCount.Text = totalTransactingATM.ToString();


            if (Label_ChequeNoActivity.Text != "0")
            {
                this.Label_ChequeNoActivity.Attributes.Add("onclick", "displayATM(6);");
                this.Label_ChequeNoActivity.Attributes.Add("style", "text-decoration:underline");
                hidden_ChequeNoTransactingATM.Value = builderNoTrxn.ToString();
            }
            if (Label_ChequeTrxnCount.Text != "0")
            {
                this.Label_ChequeTrxnCount.Attributes.Add("onclick", "displayATM(7);");
                this.Label_ChequeTrxnCount.Attributes.Add("style", "text-decoration:underline");
                hidden_ChequeTransactingATM.Value = builderTrxn.ToString();
            }

        }


        private void DisplayCITTasks()
        {
            DataTable dtTasks = new DataTable();
            dtTasks.Columns.Add("task");
            dtTasks.Columns.Add("count");

            DataRow dr = dtTasks.NewRow();
            dr[0] = "Replenishments Pending";
            dr[1] = (int)ConnectionFactory.ExecuteScalar("select count(cash_order_monitoring_id) from cash_order_monitoring where cash_order_datetime=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and current_order_executed_at is null and ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ");
            dtTasks.Rows.Add(dr);

            dr = dtTasks.NewRow();
            dr[0] = "Missing Settlements";
            dr[1] = (int)ConnectionFactory.ExecuteScalar("select count(atm_settlement_id) from atm_settlement where rep_datetime=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and GL_NO IN (SELECT GL_NUMBER FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "))");
            dtTasks.Rows.Add(dr);

            GridView_CITTasks.DataSource = dtTasks;
            GridView_CITTasks.DataBind();


        }

        private void DrawPie(ref SqlCommand cmd)
        {
            DataTable dt = null;
            SqlDataReader reader = null;
            this.Chart1.Series[0].Points.Clear();
            int inactivityCounter = 0;
            int faultyCounter = 0;
            int totalATMs = 0;
            int transactingCount = 0;
            int dispensingFailed = 0;
            StringBuilder builderForSQL = new StringBuilder();
            DataTable dtATMs = new DataTable();
            dtATMs.Columns.Add("title");
            DataTable dtInactivity = new DataTable();

            dtInactivity.Columns.Add("title");
            dtInactivity.PrimaryKey = new DataColumn[] { dtInactivity.Columns[0] };

            DataTable dtFaulty = dtInactivity.Clone();
            DataTable dtDispensing = dtInactivity.Clone();
            DataTable dtTransacting = dtInactivity.Clone();
            try
            {
                //lock (syncObject)
                //{
                //    if (Cache["DrawPieConsideringOtherParam"] == null)
                //    {
                InitializeCommand(ref cmd);
                cmd.Parameters.Clear();
                //if (cmd.Connection.State != ConnectionState.Open)
                //    cmd.Connection.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                StringBuilder builder = new StringBuilder();
                if (!CheckBox_AnalysisOnlyOnTransactingATM.Checked)
                {
                    lock (syncObject)
                    {
                        if (Cache["DrawPieConsideringOtherParam"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            ////Looking for faulty alerts
                            //cmd.CommandText = "select distinct(title) from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.ATMCassetteFaulty + " and resolve_at is null and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            cmd.CommandText = "[GetAtmsByAlertId]";
                            cmd.Parameters.AddWithValue("@alertType", (int)EnumAlertType.ATMCassetteFaulty);
                            cmd.Parameters.AddWithValue("@atmIds", Session[SessionVars.SelectedATMs.ToString()]);
                            cmd.Parameters.AddWithValue("@isDistinct", 1);

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            adapter.Fill(dt);
                            foreach (DataRow drTemp in dt.Rows)
                            {
                                if (!dtFaulty.Rows.Contains(drTemp[0]))
                                {
                                    builder.Append(drTemp[0] + ",");
                                    DataRow dr = dtFaulty.NewRow();
                                    dr[0] = drTemp[0];
                                    dtFaulty.Rows.Add(dr);
                                    faultyCounter++;
                                }
                            }


                            //reader = cmd.ExecuteReader();
                            //try
                            //{
                            //    while (reader.Read())
                            //    {
                            //        if (!dtFaulty.Rows.Contains(reader[0]))
                            //        {
                            //            builder.Append(reader[0] + ",");
                            //            DataRow dr = dtFaulty.NewRow();
                            //            dr[0] = reader[0];
                            //            dtFaulty.Rows.Add(dr);
                            //            faultyCounter++;
                            //        }
                            //    }
                            //}
                            //finally
                            //{
                            //    reader.Close();
                            //}
                            hidden_faulty.Value = builder.ToString();

                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            ////Looking for Inactivity Alert                
                            builder = new StringBuilder();
                            //cmd.CommandText = "select title from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id in (" + (int)EnumAlertType.ATMInactivityPeriodElapsed + "," + (int)EnumAlertType.ATMEJInactivityPeriodElapsed + ") and resolve_at is null and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";

                            cmd.Parameters.Clear();
                            cmd.CommandText = "[GetAtmsByAlertId]";
                            cmd.Parameters.AddWithValue("@alertType", (int)EnumAlertType.ATMInactivityPeriodElapsed + "," + (int)EnumAlertType.ATMEJInactivityPeriodElapsed);
                            cmd.Parameters.AddWithValue("@atmIds", Session[SessionVars.SelectedATMs.ToString()]);
                            cmd.Parameters.AddWithValue("@isDistinct", 0);
                            //cmd.Parameters.AddWithValue("@isDistinct", 0);
                            adapter = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            adapter.Fill(dt);
                            foreach (DataRow drTemp in dt.Rows)
                            {
                                if (!dtInactivity.Rows.Contains(drTemp[0]))
                                {
                                    builder.Append(drTemp[0] + ",");
                                    //DataRow dr = dtATMs.NewRow();
                                    DataRow dr = dtInactivity.NewRow();
                                    dr[0] = drTemp[0];
                                    dtInactivity.Rows.Add(dr);
                                    inactivityCounter++;
                                }
                            }

                            //try
                            //{
                            //    reader = cmd.ExecuteReader();
                            //    while (reader.Read())
                            //    {
                            //        if (!dtInactivity.Rows.Contains(reader[0]))
                            //        {
                            //            builder.Append(reader[0] + ",");
                            //            //DataRow dr = dtATMs.NewRow();
                            //            DataRow dr = dtInactivity.NewRow();
                            //            dr[0] = reader[0];
                            //            dtInactivity.Rows.Add(dr);
                            //            inactivityCounter++;
                            //        }
                            //    }
                            //}
                            //finally
                            //{
                            //    reader.Close();
                            //}
                            hidden_noActivity.Value = builder.ToString();

                            ///////////////////////////////////////////////
                            ////Looking for Dispensing Failed Alert                
                            builder = new StringBuilder();
                            //cmd.CommandText = "select title from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.CassetteDispensingFailed + " and resolve_at is null and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";

                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@alertType", (int)EnumAlertType.CassetteDispensingFailed);
                            cmd.Parameters.AddWithValue("@atmIds", Session[SessionVars.SelectedATMs.ToString()]);
                            cmd.Parameters.AddWithValue("@isDistinct", 0);
                            adapter = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            adapter.Fill(dt);
                            foreach (DataRow drTemp in dt.Rows)
                            {
                                if (!dtDispensing.Rows.Contains(drTemp[0]))
                                {
                                    builder.Append(drTemp[0] + ",");
                                    DataRow dr = dtDispensing.NewRow();
                                    dr[0] = drTemp[0];
                                    dtDispensing.Rows.Add(dr);
                                    dispensingFailed++;
                                }
                            }

                            //try
                            //{
                            //    reader = cmd.ExecuteReader();
                            //    while (reader.Read())
                            //    {
                            //        if (!dtDispensing.Rows.Contains(reader[0]))
                            //        {
                            //            builder.Append(reader[0] + ",");
                            //            DataRow dr = dtDispensing.NewRow();
                            //            dr[0] = reader[0];
                            //            dtDispensing.Rows.Add(dr);
                            //            dispensingFailed++;
                            //        }
                            //    }
                            //}
                            //finally
                            //{
                            //    reader.Close();
                            //}
                            hidden_dispensedFailed.Value = builder.ToString();


                            Cache.Insert("DrawPieConsideringOtherParam", faultyCounter + "|" + inactivityCounter + "|" + dispensingFailed + "|" +
                                hidden_faulty.Value + "|" + hidden_noActivity.Value + "|" + hidden_dispensedFailed.Value, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);

                            Cache.Insert("DrawPieConsideringOtherParamFaulty", dtFaulty, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                            Cache.Insert("DrawPieConsideringOtherParamDispensing", dtDispensing, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                            Cache.Insert("DrawPieConsideringOtherParamInactivity", dtInactivity, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);

                        }

                        else
                        {
                            string var = (string)Cache["DrawPieConsideringOtherParam"];
                            string[] parts = var.Split('|');
                            faultyCounter = int.Parse(parts[0]);
                            inactivityCounter = int.Parse(parts[1]);
                            dispensingFailed = int.Parse(parts[2]);
                            hidden_faulty.Value = parts[3];
                            hidden_noActivity.Value = parts[4];
                            hidden_dispensedFailed.Value = parts[5];
                            dtFaulty = (DataTable)Cache["DrawPieConsideringOtherParamFaulty"];
                            dtDispensing = (DataTable)Cache["DrawPieConsideringOtherParamDispensing"];
                            dtInactivity = (DataTable)Cache["DrawPieConsideringOtherParamInactivity"];


                        }
                    }


                }
                lock (syncObject)
                {
                    if (Cache["DrawPieConsideringTransactingParam"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select count(title) from atm where is_active=1 and is_atm=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                        totalATMs = (int)cmd.ExecuteScalar();

                        cmd.CommandText = "GetTransactingATMTitle";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("FromDate", DateTime.Today.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("ToDate", DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59");
                        cmd.Parameters.AddWithValue("atmIDs", Session[SessionVars.SelectedATMs.ToString()]);
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);

                        Cache.Insert("DrawPieConsideringTransactingParam", totalATMs, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                        Cache.Insert("DrawPieConsideringTransactingParamDt", dt, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);

                    }
                    else
                    {
                        totalATMs = (int)Cache["DrawPieConsideringTransactingParam"];
                        dt = (DataTable)Cache["DrawPieConsideringTransactingParamDt"];
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!CheckBox_AnalysisOnlyOnTransactingATM.Checked)
                        {
                            if (!dtDispensing.Rows.Contains(dr[0]) && !dtInactivity.Rows.Contains(dr[0]) && !dtFaulty.Rows.Contains(dr[0]))
                            {
                                builder.Append(dr[0] + ",");
                                builderForSQL.Append("'" + dr[0] + "',");
                                transactingCount++;
                            }
                        }
                        else
                        {
                            builder.Append(dr[0] + ",");
                            builderForSQL.Append("'" + dr[0] + "',");
                            transactingCount++;
                        }
                    }

                    //RevertToExecuteTextComamnds(ref cmd);
                    //cmd.CommandText = "select distinct(title) from ej_parsed_transactions,atm where atm.is_active = 1 and ej_parsed_transactions.atm_id = atm.atm_id and trxn_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and trxn_datetime<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59',103) and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                    //try
                    //{
                    //    reader = cmd.ExecuteReader();
                    //    while (reader.Read())
                    //    {
                    //        if (!dtDispensing.Rows.Contains(reader[0]) && !dtInactivity.Rows.Contains(reader[0]) && !dtFaulty.Rows.Contains(reader[0]))
                    //        {
                    //            builder.Append(reader[0] + ",");
                    //            transactingCount++;
                    //            builderForSQL.Append("'" + reader[0] + "',");
                    //        }
                    //    }
                    //}
                    //finally
                    //{
                    //    reader.Close();
                    //}
                }




                hidden_transacting.Value = builder.ToString();
                this.Label_TotalATMs.Text = totalATMs.ToString();


                this.Chart1.Series["Default"].Points.AddXY("Transacting ATMs", new object[] { transactingCount });
                this.Label_ATMsTrxn.Text = transactingCount.ToString();
                if (!CheckBox_AnalysisOnlyOnTransactingATM.Checked)
                {
                    trNoTransactionATMRow.Visible = false;
                    trNoActivityRow.Visible = true;
                    trDispensingFailedRow.Visible = true;
                    trCassetteFaultyRow.Visible = true;

                    this.Chart1.Series["Default"].Points.AddXY("ATMs Having Cassettes Faulty", new object[] { faultyCounter });
                    this.Chart1.Series["Default"].Points.AddXY("ATMs Having Inactivity Period Elapsed", new object[] { inactivityCounter });
                    this.Chart1.Series["Default"].Points.AddXY("ATMs Dispensing Failed", new object[] { dispensingFailed });
                    this.Label_CassettesFaulty.Text = faultyCounter.ToString();
                    this.Label_NoActivity.Text = inactivityCounter.ToString();
                    this.Label_ATMsDispensingFailed.Text = dispensingFailed.ToString();

                    if (dispensingFailed > 0)
                    {
                        this.Label_ATMsDispensingFailed.Attributes.Add("onclick", "displayATM(1);");
                        this.Label_ATMsDispensingFailed.Attributes.Add("style", "text-decoration:underline");
                    }
                    if (faultyCounter > 0)
                    {
                        this.Label_CassettesFaulty.Attributes.Add("onclick", "displayATM(2);");
                        this.Label_CassettesFaulty.Attributes.Add("style", "text-decoration:underline");
                    }
                    if (inactivityCounter > 0)
                    {
                        this.Label_NoActivity.Attributes.Add("onclick", "displayATM(3);");
                        this.Label_NoActivity.Attributes.Add("style", "text-decoration:underline");
                    }

                }
                else
                {
                    trNoActivityRow.Visible = false;
                    trNoTransactionATMRow.Visible = true;
                    trDispensingFailedRow.Visible = false;
                    trCassetteFaultyRow.Visible = false;

                    builder = new StringBuilder();
                    int noTransactingATM = totalATMs - transactingCount;
                    this.Chart1.Series["Default"].Points.AddXY("No Transacting ATMs", noTransactingATM);
                    this.Chart1.Series["Default"].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#EB1414");

                    this.Label_NonTransactingATM.Text = noTransactingATM.ToString();
                    if (noTransactingATM > 0)
                    {
                        this.Label_NonTransactingATM.Attributes.Add("onclick", "displayATM(8);");
                        this.Label_NonTransactingATM.Attributes.Add("style", "text-decoration:underline");
                    }
                    cmd.CommandType = CommandType.Text;
                    if (hidden_transacting.Value.Length > 0)
                        cmd.CommandText = "select title from atm where is_active=1 and is_atm=1 and title not in (" + builderForSQL.ToString() + "'-1') and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                    else
                        cmd.CommandText = "select title from atm where is_active=1 and is_atm=1 and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";

                    try
                    {
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            builder.Append(reader[0] + ",");
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                    hidden_noTransacting.Value = builder.ToString();

                }


                if (transactingCount > 0)
                {
                    this.Label_ATMsTrxn.Attributes.Add("onclick", "displayATM(0);");
                    this.Label_ATMsTrxn.Attributes.Add("style", "text-decoration:underline");
                }

                this.Chart1.Series["Default"].ChartType = SeriesChartType.Pie;
                this.Chart1.Series["Default"]["PieLabelStyle"] = "Outside";
                this.Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                this.Chart1.Series[0]["PieDrawingStyle"] = "SoftEdge";
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("DrawPie", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                //if (cmd != null)
                //    if (cmd.Connection != null)
                //        cmd.Connection.Close();
            }

        }

        private string FilterConfiguredCassettes()
        {
            StringBuilder builder = new StringBuilder();
            string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(new char[] { ',' });
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "1")
                {
                    builder.Append(" cassette" + (i + 1) + "_notes=0 or ");
                }
            }
            string filter = builder.ToString();
            return filter.Substring(0, filter.LastIndexOf("or"));
        }

        private DataTable GetAlertDatatable()
        {
            LogableTask task = LogableTask.NewTask("GetAlertDatatable");
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = null;
                cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = string.Concat(new object[] { "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select count(distinct(atm.atm_id)),alert_type_id from atm inner join ccms_integrated_alert  on atm.atm_id = ccms_integrated_alert.entity_id  inner join user_atms on user_atms.atm_id = atm.atm_id  where alert_type_id in (31,32,36) and generated_at >= convert(datetime, '", DateTime.Now.AddHours(-1.0).ToString("yyyy-MM-dd  HH:mm:ss"), "', 120)  and generated_at <= convert(datetime, '", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "', 120)  and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " group by alert_type_id " });
                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
            return dt;
        }

        //protected void GetAlertsHTML(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetAlertsHTML");
        //    SqlConnection conn = null;
        //    try
        //    {
        //        SqlCommand cmd = null;
        //        conn = ConnectionFactory.GetNewConnection();
        //        cmd = conn.CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = string.Concat(new object[] { "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select top 20 title, generated_at, alert_type_name from atm_alert,alert_type, atm, user_atms where atm_alert.alert_type_id = alert_type.alert_type_id and  atm_alert.atm_id = atm.atm_id and user_atms.atm_id = atm.atm_id and is_active = 1 and resolve_at is null and  generated_at >=convert(datetime,'", DateTime.Today.ToString("dd/MM/yyyy"), "',103) and generated_at <=convert(datetime,'", DateTime.Today.ToString("dd/MM/yyyy"), " 23:59:59',103) and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " order by ", order });
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        cmd.CommandText = string.Concat(new object[] { "select top 20 title, generated_at, alert_type_name from atm_alert, alert_type, atm, user_atms where atm_alert.alert_type_id = alert_type.alert_type_id and  atm_alert.atm_id = atm.atm_id and user_atms.atm_id = atm.atm_id and is_active = 1 and resolve_at is null and  generated_at<convert(datetime,'", DateTime.Today.ToString("dd/MM/yyyy"), "',103) and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " order by ", order });
        //        adapter = new SqlDataAdapter(cmd);
        //        DataTable dt1 = new DataTable();
        //        adapter.Fill(dt1);
        //        this.GridView_Alerts_CurrentDay.DataSource = dt;
        //        this.GridView_Alerts_CurrentDay.DataBind();
        //        this.GridView_Alerts_PreviousDay.DataSource = dt1;
        //        this.GridView_Alerts_PreviousDay.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //        if (conn != null)
        //            conn.Close();
        //    }
        //}

        //protected void GetATMsChequeColAlerts(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetATMsChequeColAlerts");
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select top 1000 title, generated_at, alert_msg from atm_alert, atm \r\n                                    where atm_alert.atm_id = atm.atm_id \r\n                                    and alert_type_id = 17 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")  order by " + order;
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        DataRow[] currentDayRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
        //        DataRow[] previousDayRows = dt.Select("generated_at < #" + DateTime.Today + "#");
        //        if (currentDayRows.Length > 0)
        //        {
        //            this.GridView_ChequeCollection_CurrentDay.DataSource = currentDayRows.CopyToDataTable<DataRow>();
        //        }
        //        else
        //        {
        //            this.GridView_ChequeCollection_CurrentDay.DataSource = new DataTable();
        //        }
        //        this.GridView_ChequeCollection_CurrentDay.DataBind();
        //        if (previousDayRows.Length > 0)
        //        {
        //            this.GridView_ChequeCollection_PreviousDay.DataSource = previousDayRows.CopyToDataTable<DataRow>();
        //        }
        //        else
        //        {
        //            this.GridView_ChequeCollection_PreviousDay.DataSource = new DataTable();
        //        }
        //        this.GridView_ChequeCollection_PreviousDay.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //protected void GetATMsDepositColAlerts(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetATMsDepositColAlerts");
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select top 1000 title, generated_at, alert_msg from atm_alert, atm \r\n                                    where atm_alert.atm_id = atm.atm_id \r\n                                    and alert_type_id = 18 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") order by " + order;
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        DataRow[] currentDayRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
        //        DataRow[] previousDayRows = dt.Select("generated_at < #" + DateTime.Today + "#");
        //        if (currentDayRows.Length > 0)
        //        {
        //            this.GridView_DepositCollection_CurrentDay.DataSource = currentDayRows.CopyToDataTable<DataRow>();
        //        }
        //        else
        //        {
        //            this.GridView_DepositCollection_CurrentDay.DataSource = new DataTable();
        //        }
        //        this.GridView_DepositCollection_CurrentDay.DataBind();
        //        if (previousDayRows.Length > 0)
        //        {
        //            this.GridView_DepositCollection_PreviousDay.DataSource = previousDayRows.CopyToDataTable<DataRow>();
        //        }
        //        else
        //        {
        //            this.GridView_DepositCollection_PreviousDay.DataSource = new DataTable();
        //        }
        //        this.GridView_DepositCollection_PreviousDay.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //        protected void GetATMsHavingLastTrxns(string order)
        //        {
        //            LogableTask task = LogableTask.NewTask("GetATMsHavingLastTrxns");
        //            SqlConnection conn = null;
        //            try
        //            {
        //                SqlCommand cmd = null;
        //                SqlDataReader reader = null;
        //                DataTable dt2 = new DataTable();
        //                dt2.Columns.Add("title");
        //                dt2.Columns.Add("trxn_datetime", typeof(DateTime));
        //                dt2.Columns.Add("amount");
        //                dt2.Columns.Add("last_heart_beat_at");
        //                dt2.Columns.Add("is_healthy", typeof(bool));
        //                conn = ConnectionFactory.GetNewConnection();
        //                conn.Open();
        //                cmd = conn.CreateCommand();
        //                cmd.CommandTimeout = 600;
        //                cmd.CommandText = @"select outerATM.title, trxn_datetime, amount ,outerATM.is_healthy,
        //                                    (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at
        //                                    from atm  outerATM inner join parsed_transaction
        //                                    on parsed_transaction.atm_id = outerATM.atm_id and outerATM.is_counter_enabled=1 and outerATM.is_active=1 
        //                                    and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
        //                                    " and trxn_datetime =  (select max(trxn_datetime) from parsed_transaction  " +
        //                                    " where atm_id = outerATM.atm_id) " +
        //                                    " where outerATM.atm_id in (select atm_id " +
        //                                    " from atm where is_active = 1 and atm_id not in (" +
        //                                    "select parsed_transaction.atm_id " +
        //                                    " from parsed_transaction , user_atms " +
        //                                    " where user_atms.atm_id = parsed_transaction.atm_id " +
        //                                    " and trxn_datetime >= convert(datetime, '" + DateTime.Today.ToString("yyyy-MM-dd") + "', 120)" +
        //                                    " and trxn_datetime <= convert(datetime, '" + DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd") + "', 120) " +
        //                                    " and user_atms.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId + " and amount > 0 " +
        //                                    " group by parsed_transaction.atm_id  having count(parsed_transaction_id)>0)) order by " + order;

        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "fetched");
        //                reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    DataRow dr = dt2.NewRow();
        //                    dr[0] = reader.GetString(0);
        //                    dr[1] = reader.GetDateTime(1);
        //                    dr[2] = reader.GetDecimal(2);
        //                    if (reader[4] != DBNull.Value)
        //                    {
        //                        dr[3] = reader.GetDateTime(4).ToString("dd/MM/yyyy HH:mm:ss");
        //                    }
        //                    else
        //                    {
        //                        dr[3] = "-";
        //                    }
        //                    dr[4] = reader.GetBoolean(3);
        //                    dt2.Rows.Add(dr);
        //                }
        //                reader.Close();
        //                //Read For Ej
        //                cmd.CommandText = @"select outerATM.title, trxn_datetime, amount ,outerATM.is_healthy," +
        //                                   " (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at " +
        //                                   " from atm  outerATM inner join ej_parsed_transactions " +
        //                                   " on ej_parsed_transactions.atm_id = outerATM.atm_id   " +
        //                                   " and outerATM.is_ej_enabled=1 and outerATM.is_active=1 and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " +
        //                                   " and trxn_datetime = (select max(trxn_datetime) " +
        //                                   " from ej_parsed_transactions " +
        //                                   " where atm_id = outerATM.atm_id) " +
        //                                   " where outerATM.atm_id in (select atm_id " +
        //                                   " from atm where is_active = 1 and atm_id not in ( " +
        //                                    " select ej_parsed_transactions.atm_id " +
        //                                    "from ej_parsed_transactions , user_atms " +
        //                                   " where user_atms.atm_id = ej_parsed_transactions.atm_id " +
        //                                   " and trxn_datetime >= convert(datetime, '" + DateTime.Today.ToString("yyyy-MM-dd") + "', 120)" +
        //                                   "  and trxn_datetime <= convert(datetime, '" + DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd") + "', 120) " +
        //                                   " and user_atms.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId + " and status= 0  " +
        //                                   "group by ej_parsed_transactions.atm_id  having count(ej_parsed_transactions_id)>0)) order by " + order;
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "fetched");
        //                reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    DataRow dr = dt2.NewRow();
        //                    dr[0] = reader.GetString(0);
        //                    dr[1] = reader.GetDateTime(1);
        //                    dr[2] = reader.GetDecimal(2);
        //                    if (reader[4] != DBNull.Value)
        //                    {
        //                        dr[3] = reader.GetDateTime(4).ToString("dd/MM/yyyy HH:mm:ss");
        //                    }
        //                    else
        //                    {
        //                        dr[3] = "-";
        //                    }
        //                    dr[4] = reader.GetBoolean(3);
        //                    dt2.Rows.Add(dr);
        //                }
        //                reader.Close();
        //                //

        //                this.GridView_AnyDay.DataSource = dt2;
        //                this.GridView_AnyDay.DataBind();
        //            }
        //            catch (Exception ex)
        //            {
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //            }
        //            finally
        //            {
        //                task.EndTask();
        //                if (conn != null)
        //                    conn.Close();

        //            }
        //        }

        //        protected void GetATMsHavingNoLastTrxns(string order)
        //        {
        //            LogableTask task = LogableTask.NewTask("GetATMsHavingNoLastTrxns");
        //            SqlConnection conn = null;
        //            try
        //            {
        //                SqlCommand cmd = null;
        //                conn = ConnectionFactory.GetNewConnection();
        //                conn.Open();
        //                cmd = conn.CreateCommand();
        //                cmd.CommandTimeout = 600;
        //                cmd.CommandText = @"select outerATM.title,outerATM.is_healthy, 
        //                                  (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at 
        //                                  from atm outerATM where atm_id in (select outerATM.atm_id    
        //                                  from user_ATMs inner join atm outerATM   
        //                                  on user_ATMs.ATM_id = outerATM.ATM_id  
        //                                  left join parsed_transaction  
        //                                  on user_ATMs.ATM_id = parsed_transaction.atm_id 
        //                                  where outerATM.is_active = 1 and outerATM.is_counter_enabled=1
        //                                  and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
        //                                  " and user_atms.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId +
        //                                  " group by outerATM.atm_id  " +
        //                                  " having COUNT(parsed_transaction.parsed_transaction_id)=0) order by " + order;

        //                DataTable dt = new DataTable();
        //                new SqlDataAdapter(cmd).Fill(dt);
        //                cmd.CommandText = @"select outerATM.title,outerATM.is_healthy, 
        //                                  (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at 
        //                                  from atm outerATM where atm_id in (select outerATM.atm_id    
        //                                  from user_ATMs inner join atm outerATM   
        //                                  on user_ATMs.ATM_id = outerATM.ATM_id  
        //                                  left join ej_parsed_transactions
        //                                  on user_ATMs.ATM_id = ej_parsed_transactions.atm_id 
        //                                  where outerATM.is_active = 1 and outerATM.is_ej_enabled=1
        //                                  and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
        //                                  " and user_atms.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId +
        //                                  " group by outerATM.atm_id  " +
        //                                  " having COUNT(ej_parsed_transactions.ej_parsed_transactions_id)=0) order by " + order;

        //                DataTable dtEj = new DataTable();
        //                new SqlDataAdapter(cmd).Fill(dtEj);


        //                dt.Merge(dtEj);
        //                dt.DefaultView.Sort = "title asc";
        //                this.GridView_noTrxn.DataSource = dt;
        //                this.GridView_noTrxn.DataBind();
        //            }
        //            catch (Exception ex)
        //            {
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //            }
        //            finally
        //            {
        //                task.EndTask();
        //                if (conn != null)
        //                    conn.Close();
        //            }
        //        }

        //protected void GetATMsHavingThresholdReached(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetATMsHavingThresholdReached");
        //    SqlConnection conn = null;
        //    SqlDataReader reader = null;
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("title");
        //    dt.Columns.Add("generated_at", typeof(DateTime));
        //    dt.Columns.Add("amount");
        //    dt.Columns.Add("last_heart_beat_at");
        //    dt.Columns.Add("is_healthy", typeof(bool));
        //    dt.Columns.Add("last_trxn_at", typeof(DateTime));
        //    try
        //    {
        //        conn = ConnectionFactory.GetNewConnection();
        //        conn.Open();
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = @"select title, generated_at, alert_msg,outerATM.atm_id,is_healthy," +
        //                           "(select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at,alert_type_id " +
        //                           " from atm_alert, atm outerATM " +
        //                           " where atm_alert.atm_id = outerATM.atm_id and resolve_at is null " +
        //                           " and alert_type_id in (31,40) and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
        //        reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            DataRow dr = dt.NewRow();
        //            dr[0] = reader.GetString(0);
        //            dr[1] = reader.GetDateTime(1);
        //            string msg = reader.GetString(2);
        //            if (msg.StartsWith("Last"))
        //            {
        //                //decimal amount = -1M;
        //                DateTime lastReportedTrxn = DateTime.ParseExact(msg.Replace("Last Reported Transaction ", ""), "dd/MM/yyyy HH:mm:ss", null);
        //                object result = null;
        //                if (reader[6].ToString() == "40")
        //                    result = ConnectionFactory.ExecuteScalar(string.Concat(new object[] { "select amount from ej_parsed_transactions where atm_id = ", reader.GetInt32(3), " and trxn_datetime=convert(datetime,'", lastReportedTrxn.ToString("dd/MM/yyyy HH:mm:ss"), "',103)" }));
        //                else
        //                    result = ConnectionFactory.ExecuteScalar(string.Concat(new object[] { "select amount from parsed_transaction where atm_id = ", reader.GetInt32(3), " and trxn_datetime=convert(datetime,'", lastReportedTrxn.ToString("dd/MM/yyyy HH:mm:ss"), "',103)" }));

        //                if (result != null)
        //                {
        //                    dr[2] = (decimal)result;
        //                }
        //                //dr[2] = amount;
        //                dr[5] = lastReportedTrxn;
        //            }
        //            if (reader[5] != DBNull.Value)
        //            {
        //                dr[3] = reader.GetDateTime(5).ToString("dd/MM/yyyy HH:mm:ss");
        //            }
        //            else
        //            {
        //                dr[3] = "-";
        //            }
        //            dr[4] = reader.GetBoolean(4);
        //            dt.Rows.Add(dr);
        //        }
        //        reader.Close();
        //        dt.DefaultView.Sort = order;
        //        this.GridView_thresholdReached.DataSource = dt;
        //        this.GridView_thresholdReached.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        if (conn != null)
        //            conn.Close();
        //        task.EndTask();
        //    }
        //}

        //        protected void GetATMsWithNoTrxnOnCurrentDay(string order)
        //        {
        //            LogableTask task = LogableTask.NewTask("GetATMsWithNoTrxnOnCurrentDay");
        //            SqlConnection conn = null;
        //            try
        //            {
        //                SqlCommand cmd = null;
        //                DataTable dt = new DataTable();
        //                DataTable dtEJ = new DataTable();
        //                dt.Columns.Add("title");
        //                dt.Columns.Add("last_heart_beat_at", typeof(DateTime));
        //                dt.Columns.Add("is_healthy", typeof(bool));
        //                conn = ConnectionFactory.GetNewConnection();
        //                conn.Open();
        //                cmd = conn.CreateCommand();
        //                cmd.CommandTimeout = 600;
        //                cmd.CommandText = @"select outerATM.title,outerATM.is_healthy,
        //                                   (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at
        //                                    from atm outerATM where is_active=1 and is_counter_enabled=1 and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
        //                                    " and outerATM.atm_id not in ( " +
        //                                    " select parsed_transaction.atm_id " +
        //                                    " from parsed_transaction , user_atms " +
        //                                    " where user_atms.atm_id = parsed_transaction.atm_id " +
        //                                    " and trxn_datetime >= convert(datetime, '" + DateTime.Today.ToString("yyyy-MM-dd") + "', 120) " +
        //                                    " and trxn_datetime <= convert(datetime, '" + DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd") + "', 120)  " +
        //                                    " and user_atms.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId +
        //                                    " and amount > 0  group by parsed_transaction.atm_id  having count(parsed_transaction_id)>0) order by " + order;
        //                new SqlDataAdapter(cmd).Fill(dt);

        //                cmd.CommandText = @"select outerATM.title,outerATM.is_healthy,
        //                                   (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at
        //                                   from atm outerATM where is_active=1 and is_ej_enabled=1 and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " +
        //                                   " and outerATM.atm_id not in ( " +
        //                                    " select ej_parsed_transactions.atm_id " +
        //                                    " from ej_parsed_transactions , user_atms " +
        //                                    " where user_atms.atm_id = ej_parsed_transactions.atm_id " +
        //                                    " and trxn_datetime >= convert(datetime, '" + DateTime.Today.ToString("yyyy-MM-dd") + "', 120) " +
        //                                    " and trxn_datetime <= convert(datetime,'" + DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd") + "', 120) " +
        //                                    " and user_atms.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId + " and is_active=1 and is_ej_enabled=1 and status = 0  group by ej_parsed_transactions.atm_id  having count(ej_parsed_transactions_id)>0) order by " + order;
        //                new SqlDataAdapter(cmd).Fill(dtEJ);
        //                dt.Merge(dtEJ);
        //                dt.DefaultView.Sort = "title asc";
        //                this.GridView_CurrentDay.DataSource = dt;
        //                this.GridView_CurrentDay.DataBind();
        //                Literal_TotalRec.Text = "Total Atms " + dt.Rows.Count;
        //            }
        //            catch (Exception ex)
        //            {
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //            }
        //            finally
        //            {
        //                task.EndTask();
        //                if (conn != null)
        //                    conn.Close();
        //            }
        //        }

        //protected void GetCounterMismatchAlerts(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetCounterMismatchAlerts");
        //    DataTable dtPreviousDay = null;
        //    DataTable dtCurrentDay = null;
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select top 1000 title, generated_at, alert_msg from atm_alert, atm \r\n                                    where atm_alert.atm_id = atm.atm_id and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and alert_type_id = 23 order by " + order;
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        DataRow[] currentDayRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
        //        DataRow[] previousDayRows = dt.Select("generated_at < #" + DateTime.Today + "#");
        //        if (currentDayRows.Length > 0)
        //        {
        //            dtCurrentDay = currentDayRows.CopyToDataTable<DataRow>();
        //            foreach (DataRow dr in dtCurrentDay.Rows)
        //            {
        //                dr[2] = CashDataViewer.FormatInformation(dr[2].ToString());
        //            }
        //            dtCurrentDay.DefaultView.Sort = order;
        //        }
        //        this.GridView_CounterMismatch_CurrentDay.DataSource = dtCurrentDay;
        //        this.GridView_CounterMismatch_CurrentDay.DataBind();
        //        if (previousDayRows.Length > 0)
        //        {
        //            dtPreviousDay = previousDayRows.CopyToDataTable<DataRow>();
        //            foreach (DataRow dr in dtPreviousDay.Rows)
        //            {
        //                dr[2] = CashDataViewer.FormatInformation(dr[2].ToString());
        //            }
        //            dtPreviousDay.DefaultView.Sort = order;
        //        }
        //        this.GridView_CounterMismatch_PreviousDay.DataSource = dtPreviousDay;
        //        this.GridView_CounterMismatch_PreviousDay.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //protected void GetCurrentFaultyRep(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetCurrentFaultyRep");
        //    //DataTable temp = null;
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        //cmd.CommandText = "select title,trxn_datetime,cash_dispensed1,cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,cash_dispensed7,parsed_transaction_id,atm.atm_id,cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4\r\n                                        from atm, parsed_transaction  where atm.atm_id = parsed_transaction.atm_id \r\n                                        and trxn_datetime >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  and trxn_datetime <= convert(datetime,'" + DateTime.Today.AddDays(1.0).ToString("dd/MM/yyyy") + " 23:59:59',103) order by trxn_datetime";
        //        cmd.CommandText = "select title,generated_at,alert_msg,standard_order_type1,standard_order_type2,standard_order_type3,standard_order_type4   from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.ATMCassetteFaulty + " and resolve_at is null and generated_at>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and generated_at<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ";

        //        DataTable dt = new DataTable();
        //        new SqlDataAdapter(cmd).Fill(dt);
        //        //if (dt.Rows.Count > 0)
        //        //{
        //        //    List<int> atmIDs = new List<int>();
        //        //    foreach (DataRow dr in dt.Rows)
        //        //    {
        //        //        int atmID = int.Parse(dr["atm_id"].ToString());
        //        //        if (!atmIDs.Contains(atmID))
        //        //        {
        //        //            atmIDs.Add(atmID);
        //        //        }
        //        //    }
        //        //    DataTable dtinner = new DataTable();
        //        //    dtinner.Columns.Add("title");
        //        //    dtinner.Columns.Add("status1");
        //        //    dtinner.Columns.Add("status2");
        //        //    dtinner.Columns.Add("status3");
        //        //    dtinner.Columns.Add("status4");
        //        //    dtinner.Columns.Add("trxn_datetime");
        //        //    dtinner.PrimaryKey = new DataColumn[] { dtinner.Columns["title"] };
        //        //    temp = dtinner.Clone();
        //        //    string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(new char[] { ',' });
        //        //    if (parts[0] == "1")
        //        //    {
        //        //        this.ProcessDataTable("cash_dispensed1", dt, dtinner, atmIDs, 1, "cash_remaining1");
        //        //    }
        //        //    if (parts[1] == "1")
        //        //    {
        //        //        this.ProcessDataTable("cash_dispensed2", dt, dtinner, atmIDs, 2, "cash_remaining2");
        //        //    }
        //        //    if (parts[2] == "1")
        //        //    {
        //        //        this.ProcessDataTable("cash_dispensed3", dt, dtinner, atmIDs, 3, "cash_remaining3");
        //        //    }
        //        //    if (parts[3] == "1")
        //        //    {
        //        //        this.ProcessDataTable("cash_dispensed4", dt, dtinner, atmIDs, 4, "cash_remaining4");
        //        //    }
        //        //    dt.Rows.Clear();
        //        //    foreach (DataRow dr in dtinner.Rows)
        //        //    {
        //        //        if ((((dr[1].ToString() == "Faulty") || (dr[2].ToString() == "Faulty")) || (dr[3].ToString() == "Faulty")) || (dr[4].ToString() == "Faulty"))
        //        //        {
        //        //            DataRow newRow = temp.NewRow();
        //        //            newRow[0] = dr[0];
        //        //            newRow[1] = dr[1];
        //        //            newRow[2] = dr[2];
        //        //            newRow[3] = dr[3];
        //        //            newRow[4] = dr[4];
        //        //            newRow[5] = dr[5];
        //        //            temp.Rows.Add(newRow);
        //        //        }
        //        //    }
        //        //    temp.DefaultView.Sort = order;
        //        //}
        //        dt.DefaultView.Sort = order;
        //        this.GridView_FaultyRep_Current.DataSource = dt;
        //        this.GridView_FaultyRep_Current.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //protected void GetDateTimeSyncTask(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetDateTimeSyncTask");
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;\r\n                                     select title,status,last_invoked,end_time\r\n                                        from task inner join atm \r\n                                        on task.atm_id = atm.atm_id\r\n                                        where task_type_id = 6\r\n  and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")   and task.creation_time >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and task.creation_time <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) order by " + order;
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        this.GridView_DatetimeSync.DataSource = dt;
        //        this.GridView_DatetimeSync.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //protected void GetDeadATMs(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetDeadATMs");
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;\r\n                                     select outerATM.title,(select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at \r\nfrom atm outerATM\r\nwhere is_active = 1 and ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and ATM_id not in (select ATM_id from heart_beat where heart_beat_received_at >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and heart_beat_received_at <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)) order by " + order;
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        this.GridView_DeadATM.DataSource = dt;
        //        this.GridView_DeadATM.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}


        protected void DrawDFFInfoGraph(ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("DrawDFFInfoGraph");
            UserOrganization.UserOrganizationReader userOrgReader = null;
            Atm.AtmReader reader = null;
            try
            {
                Label_org.Text = "";
                //int orgID = 0;
                this.Chart_DFFStatus.Series["Default"].Points.Clear();
                StringBuilder builderNoData = new StringBuilder();
                StringBuilder builderSending2 = new StringBuilder();
                StringBuilder builderSendingSameBalance = new StringBuilder();
                StringBuilder builderInconsistentData = new StringBuilder();
                StringBuilder temp = new StringBuilder();
                int atmSending2 = 0;
                int atmSendingSameBalance = 0;
                int atmInconsistentData = 0;
                int atmSendingNoData = 0;

                //if (Session["selectedOrgID"] != null)
                //    orgID = int.Parse(Session["selectedOrgID"].ToString());

                //Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegion("is_organization=1 and region_id<> 1 order by region_id");
                lock (syncObject)
                {
                    if (Cache["DrawDFFInfoGraph"] == null)
                    {

                        userOrgReader = UserOrganization.ExecuteReader("user_id = " + this.appUser.UserId);
                        StringBuilder builderOrg = new StringBuilder();
                        StringBuilder builderOrgName = new StringBuilder();
                        while (userOrgReader.Read())
                        {

                            Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegion("mcn = '" + userOrgReader.CurrentUserOrganization.OrgMcn + "'");
                            if (region == null) continue;
                            if (Label_org.Text.Length > 0)
                                Label_org.Text += ",";
                            Label_org.Text += region.RegionName;
                            builderOrg.Append(region.RegionId + ",");
                            builderOrgName.Append(region.RegionName + ",");
                            //null;
                            //DataRow dr;
                            string[] content = null;
                            string fileNameConvention = region.DffNamingConvention;// WebConfigurationManager.AppSettings["DFFFileNameConvention"];
                            if (fileNameConvention == null) continue;
                            fileNameConvention = fileNameConvention.Replace("[yyyyMMdd]", DateTime.Now.AddDays(-1.0).ToString("yyyyMMdd"));


                            //string regions = this.Session[SessionVars.SelectedRegions.ToString()].ToString();
                            //string[] regionsParts = regions.Split(',');
                            //int counter = 0;
                            //for (int j = 1; j < regionsParts.Length; j++)
                            //{
                            //    int regionID = int.Parse(regionsParts[j]);
                            //    region = Avanza.CCMS.DAL.Region.LoadRegionByPk(regionID);
                            //    if (!region.IsOrganization)
                            //    {
                            //        counter++;
                            //    }
                            //    else
                            //        break;
                            //}
                            //if (counter == regionsParts.Length - 1)
                            //    throw new Exception("Organization not selected.");

                            //if (regionID == 1)
                            //{
                            //    throw new Exception("Organization not selected.");
                            //}
                            //Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(regionID);
                            //if (!region.IsOrganization)
                            //{
                            //    throw new Exception("Organization not selected.");
                            //}
                            if (File.Exists(region.DailyFeedOutputFilePath + @"\PendingUpload\" + fileNameConvention))
                            {
                                content = File.ReadAllLines(region.DailyFeedOutputFilePath + @"\PendingUpload\" + fileNameConvention);
                            }
                            else if (File.Exists(region.DailyFeedOutputFilePath + @"\OutputArchive\" + fileNameConvention))
                            {
                                content = File.ReadAllLines(region.DailyFeedOutputFilePath + @"\OutputArchive\" + fileNameConvention);
                            }
                            if (content == null)
                            {
                                //throw new Exception("File does not exists." + fileNameConvention);
                                literal_script.Text += "File does not exists." + fileNameConvention;
                                continue;
                            }

                            DataTable dtDff = new DataTable();
                            dtDff.Columns.Add("title");
                            dtDff.Columns.Add("replenishmentAmount");
                            dtDff.Columns.Add("returnAmount");
                            dtDff.Columns.Add("withdrawalAmount");
                            dtDff.Columns.Add("closingBalance");

                            List<string> atmList = new List<string>();
                            string atmID = null;
                            string replenishmentAmount = null;
                            string returnAmount = null;
                            string withdrawalAmount = null;
                            string state = null;
                            string closingBalance = null;
                            SqlDataAdapter adapter = null;
                            for (int i = 1; i < (content.Length - 1); i++)
                            {
                                try
                                {
                                    atmID = content[i].Substring(8, 8);
                                    atmList.Add(atmID);
                                    DataRow newRow = dtDff.NewRow();
                                    newRow[0] = atmID;
                                    newRow[1] = replenishmentAmount = content[i].Substring(0x10, 9);
                                    newRow[2] = returnAmount = content[i].Substring(0x19, 9);
                                    newRow[3] = withdrawalAmount = content[i].Substring(0x22, 9);
                                    state = content[i].Substring(0x3d, 1);
                                    newRow[4] = closingBalance = content[i].Substring(0x34, 9);
                                    dtDff.Rows.Add(newRow);
                                    //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info,
                                    //    string.Format("atmID={0},Rep={1},Returns={2},Withdrawals={3},state={4},bal={5}",
                                    //                    atmID, replenishmentAmount, returnAmount, withdrawalAmount, state, closingBalance));

                                    if (state == "2")
                                    {
                                        atmSending2++;
                                        builderSending2.Append(atmID + ",");
                                    }
                                    else
                                    {
                                        temp.Append("'" + atmID + "',");

                                    }
                                }
                                catch (Exception ex)
                                {
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                                }

                            }
                            //No check for inconsistencies
                            if (temp.Length > 0)
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                InitializeCommand(ref cmd);
                                cmd.Parameters.Clear();
                                cmd.CommandText = "[GetSummaryRowByTitleAndDay]";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("FromDate", DateTime.Now.AddDays(-2.0).ToString("dd/MM/yyyy"));
                                cmd.Parameters.AddWithValue("ToDate", DateTime.Now.AddDays(-2.0).ToString("dd/MM/yyyy") + " 23:59:59");
                                cmd.Parameters.AddWithValue("Title", temp.ToString() + "'-1'");
                                adapter = new SqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);
                                foreach (DataRow dr in dt.Rows)
                                {
                                    //From file
                                    DataRow[] arr = dtDff.Select("title='" + dr["title"] + "'");
                                    if (arr.Length > 0)
                                    {
                                        if (decimal.Parse(arr[0]["closingBalance"].ToString()) == decimal.Parse(dr["closing_balance"].ToString()))
                                        {
                                            atmSendingSameBalance++;
                                            builderSendingSameBalance.Append(atmID + ",");
                                        }
                                        decimal inconsistentAmount = (((decimal.Parse(dr["closing_balance"].ToString()) + decimal.Parse(arr[0]["replenishmentAmount"].ToString())) - decimal.Parse(arr[0]["returnAmount"].ToString())) - decimal.Parse(arr[0]["withdrawalAmount"].ToString())) - decimal.Parse(arr[0]["closingBalance"].ToString());
                                        if (inconsistentAmount != 0M)
                                        {
                                            //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Concat(new object[] { "Opening Balance: ", summary.ClosingBalance, "\r\nReplenishment Amount: ", replenishmentAmount, "\r\nReturn Amount: ", returnAmount, "\r\nWithdrawal Amount: ", withdrawalAmount, "\r\nClosing Balance: ", closingBalance, "\r\nInconsistent Amount: ", inconsistentAmount }));
                                            atmInconsistentData++;
                                            builderInconsistentData.Append(atmID + ",");

                                        }
                                    }

                                }

                            }
                            //Atm atm = Atm.LoadAtm("title='" + atmID + "'");
                            //if (atm != null)
                            //{
                            //    Avanza.CCMS.DAL.Summary summary = Avanza.CCMS.DAL.Summary.LoadSummary(
                            //        string.Concat(new object[] { "atm_id=", atm.ATMId, " and trxn_datetime>=convert(datetime,'", DateTime.Now.AddDays(-2.0).ToString("dd/MM/yyyy"), "',103)  and trxn_datetime<=convert(datetime,'", DateTime.Now.AddDays(-2.0).ToString("dd/MM/yyyy"), " 23:59:59',103) " }));
                            //    if (summary != null)
                            //    {
                            //        if (decimal.Parse(closingBalance) == summary.ClosingBalance)
                            //        {
                            //            atmSendingSameBalance++;
                            //            builderSendingSameBalance.Append(atmID + ",");
                            //        }
                            //        decimal inconsistentAmount = (((summary.ClosingBalance + decimal.Parse(replenishmentAmount)) - decimal.Parse(returnAmount)) - decimal.Parse(withdrawalAmount)) - decimal.Parse(closingBalance);
                            //        if (inconsistentAmount != 0M)
                            //        {
                            //            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Concat(new object[] { "Opening Balance: ", summary.ClosingBalance, "\r\nReplenishment Amount: ", replenishmentAmount, "\r\nReturn Amount: ", returnAmount, "\r\nWithdrawal Amount: ", withdrawalAmount, "\r\nClosing Balance: ", closingBalance, "\r\nInconsistent Amount: ", inconsistentAmount }));
                            //            atmInconsistentData++;
                            //            builderInconsistentData.Append(atmID + ",");

                            //        }
                            //    }
                            //}

                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            InitializeCommand(ref cmd);
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "select title from atm where is_active=1 and exclude_dff=0";
                            adapter = new SqlDataAdapter(cmd);
                            DataTable dtTitles = new DataTable();
                            adapter.Fill(dtTitles);
                            foreach (DataRow dr in dtTitles.Rows)
                            {
                                //reader = Atm.ExecuteReader("exclude_dff=0");
                                //while (reader.Read())
                                //{
                                if (!atmList.Contains(dr[0].ToString()))
                                {
                                    atmSendingNoData++;
                                    builderNoData.Append(dr[0] + ",");
                                }
                            }
                            //reader.Close();
                        }
                        builderOrg.Append("-1");
                        Session["org"] = builderOrg.ToString();

                        userOrgReader.Close();
                        Cache.Insert("DrawDFFInfoGraph", atmSendingNoData + "|" + atmSending2 + "|" + atmSendingSameBalance + "|" + atmInconsistentData +
                            "|" + builderNoData.ToString() + "|" + builderSending2.ToString() + "|" + builderSendingSameBalance.ToString()
                            + "|" + builderInconsistentData.ToString() + "|" + builderOrg.ToString() + "|" + builderOrgName.ToString().Remove(builderOrgName.Length - 1, 1), null, DateTime.Now.AddMinutes(60), System.Web.Caching.Cache.NoSlidingExpiration);

                    }
                    else
                    {
                        string var = (string)Cache["DrawDFFInfoGraph"];
                        string[] parts = var.Split('|');
                        if (parts != null)
                        {
                            atmSendingNoData = int.Parse(parts[0]);
                            atmSending2 = int.Parse(parts[1]);
                            atmSendingSameBalance = int.Parse(parts[2]);
                            atmInconsistentData = int.Parse(parts[3]);
                            builderNoData.Append(parts[4]);
                            builderSending2.Append(parts[5]);
                            builderSendingSameBalance.Append(parts[6]);
                            builderInconsistentData.Append(parts[7]);
                            Session["org"] = parts[8];
                            Label_org.Text = parts[9];

                        }


                    }
                }

                this.Chart_DFFStatus.Series["Default"].Points.AddXY("ATMs Sending No Data", atmSendingNoData);
                this.Chart_DFFStatus.Series["Default"].Points.AddXY("ATMs Sending 2", atmSending2);
                this.Chart_DFFStatus.Series["Default"].Points.AddXY("ATMs Sending Same Balance", atmSendingSameBalance);
                this.Chart_DFFStatus.Series["Default"].Points.AddXY("ATMs having Inconsistent Data", atmInconsistentData);
                Label_NoData.Text = atmSendingNoData.ToString();
                Label_Sending2.Text = atmSending2.ToString();
                Label_SameBalance.Text = atmSendingSameBalance.ToString();
                Label_InconsistentData.Text = atmInconsistentData.ToString();
                Label_total.Text = (atmSendingNoData + atmSending2 + atmSendingSameBalance + atmInconsistentData).ToString();


                if (Label_NoData.Text != "0")
                {
                    this.Label_NoData.Attributes.Add("onclick", "displayDFFATM(0);");
                    this.Label_NoData.Attributes.Add("style", "text-decoration:underline");
                    hidden_NoData.Value = builderNoData.ToString();
                }
                if (Label_Sending2.Text != "0")
                {
                    this.Label_Sending2.Attributes.Add("onclick", "displayDFFATM(1);");
                    this.Label_Sending2.Attributes.Add("style", "text-decoration:underline");
                    hidden_Sending2.Value = builderSending2.ToString();
                }
                if (Label_SameBalance.Text != "0")
                {
                    this.Label_SameBalance.Attributes.Add("onclick", "displayDFFATM(2);");
                    this.Label_SameBalance.Attributes.Add("style", "text-decoration:underline");
                    hidden_SendingSameBalance.Value = builderSendingSameBalance.ToString();
                }
                if (Label_InconsistentData.Text != "0")
                {
                    this.Label_InconsistentData.Attributes.Add("onclick", "displayDFFATM(3);");
                    this.Label_InconsistentData.Attributes.Add("style", "text-decoration:underline");
                    hidden_InconsistentData.Value = builderInconsistentData.ToString();
                }

                this.Chart_DFFStatus.Series["Default"]["ShowMarkerLines"] = "true";
                this.Chart_DFFStatus.Series["Default"]["DrawingStyle"] = "Cylinder";
                this.Chart_DFFStatus.Series["Default"]["PointWidth"] = "0.8";

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                if (userOrgReader != null)
                    userOrgReader.Close();

                if (reader != null)
                    reader.Close();

                task.EndTask();
            }
        }

        protected void GetDFFInfo(string order)
        {
            LogableTask task = LogableTask.NewTask("GetDFFInfo");
            try
            {
                DataRow dr;
                string[] content = null;
                string fileNameConvention = WebConfigurationManager.AppSettings["DFFFileNameConvention"];
                fileNameConvention = fileNameConvention.Replace("[yyyyMMdd]", DateTime.Now.AddDays(-1.0).ToString("yyyyMMdd"));
                if (this.Session[SessionVars.isRegionSelected.ToString()] == null)
                {
                    throw new Exception("Organization not selected.");
                }
                int regionID = int.Parse(this.Session[SessionVars.SelectedRegion.ToString()].ToString());
                if (regionID == 1)
                {
                    throw new Exception("Organization not selected.");
                }
                Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(regionID);
                if (!region.IsOrganization)
                {
                    throw new Exception("Organization not selected.");
                }
                if (File.Exists(region.DailyFeedOutputFilePath + @"\PendingUpload\" + fileNameConvention))
                {
                    content = File.ReadAllLines(region.DailyFeedOutputFilePath + @"\PendingUpload\" + fileNameConvention);
                }
                else if (File.Exists(region.DailyFeedOutputFilePath + @"\OutputArchive\" + fileNameConvention))
                {
                    content = File.ReadAllLines(region.DailyFeedOutputFilePath + @"\OutputArchive\" + fileNameConvention);
                }
                if (content == null)
                {
                    throw new Exception("File does not exists." + fileNameConvention);
                }
                DataTable dtSending2 = new DataTable();
                dtSending2.Columns.Add("title");
                DataTable dtSendingNoData = new DataTable();
                dtSendingNoData.Columns.Add("title");
                DataTable dtSendingYesterdayBalance = new DataTable();
                dtSendingYesterdayBalance.Columns.Add("title");
                DataTable dtInconsistent = new DataTable();
                dtInconsistent.Columns.Add("title");
                List<string> atmList = new List<string>();
                string atmID = null;
                string replenishmentAmount = null;
                string returnAmount = null;
                string withdrawalAmount = null;
                string state = null;
                string closingBalance = null;
                for (int i = 1; i < (content.Length - 1); i++)
                {
                    atmID = content[i].Substring(8, 8);
                    atmList.Add(atmID);
                    replenishmentAmount = content[i].Substring(0x10, 9);
                    returnAmount = content[i].Substring(0x19, 9);
                    withdrawalAmount = content[i].Substring(0x22, 9);
                    state = content[i].Substring(0x3d, 1);
                    closingBalance = content[i].Substring(0x34, 9);
                    if (state == "2")
                    {
                        dr = dtSending2.NewRow();
                        dr[0] = atmID;
                        dtSending2.Rows.Add(dr);
                    }
                    else
                    {
                        Atm atm = Atm.LoadAtm("title='" + atmID + "'");
                        if (atm != null)
                        {
                            Avanza.CCMS.DAL.Summary summary = Avanza.CCMS.DAL.Summary.LoadSummary(string.Concat(new object[] { "atm_id=", atm.ATMId, " and trxn_datetime>=convert(datetime,'", DateTime.Now.AddDays(-2.0).ToString("dd/MM/yyyy"), "',103)  and trxn_datetime<=convert(datetime,'", DateTime.Now.AddDays(-2.0).ToString("dd/MM/yyyy"), " 23:59:59',103) " }));
                            if (summary != null)
                            {
                                if (decimal.Parse(closingBalance) == summary.ClosingBalance)
                                {
                                    dr = dtSendingYesterdayBalance.NewRow();
                                    dr[0] = atmID;
                                    dtSendingYesterdayBalance.Rows.Add(dr);
                                }
                                decimal inconsistentAmount = (((summary.ClosingBalance + decimal.Parse(replenishmentAmount)) - decimal.Parse(returnAmount)) - decimal.Parse(withdrawalAmount)) - decimal.Parse(closingBalance);
                                if (inconsistentAmount != 0M)
                                {
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Concat(new object[] { "Opening Balance: ", summary.ClosingBalance, "\r\nReplenishment Amount: ", replenishmentAmount, "\r\nReturn Amount: ", returnAmount, "\r\nWithdrawal Amount: ", withdrawalAmount, "\r\nClosing Balance: ", closingBalance, "\r\nInconsistent Amount: ", inconsistentAmount }));
                                    dr = dtInconsistent.NewRow();
                                    dr[0] = atmID;
                                    dtInconsistent.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }
                Atm.AtmReader reader = Atm.ExecuteReader("exclude_dff=0");
                while (reader.Read())
                {
                    if (!atmList.Contains(reader.CurrentAtm.Title))
                    {
                        dr = dtSendingNoData.NewRow();
                        dr[0] = reader.CurrentAtm.Title;
                        dtSendingNoData.Rows.Add(dr);
                    }
                }
                reader.Close();
                dtSendingNoData.DefaultView.Sort = order;
                //this.GridView_NotInDFF.DataSource = dtSendingNoData;
                //this.GridView_NotInDFF.DataBind();
                //dtSending2.DefaultView.Sort = order;
                //this.GridView_Sending2.DataSource = dtSending2;
                //this.GridView_Sending2.DataBind();
                //dtSendingYesterdayBalance.DefaultView.Sort = order;
                //this.GridView_SameOpeningBalanceAsOfYesterday.DataSource = dtSendingYesterdayBalance;
                //this.GridView_SameOpeningBalanceAsOfYesterday.DataBind();
                //dtInconsistent.DefaultView.Sort = order;
                //this.GridView_InconsistentData.DataSource = dtInconsistent;
                //this.GridView_InconsistentData.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        //        protected void GetFaultyRep(string order)
        //        {
        //            LogableTask task = LogableTask.NewTask("GetFaultyRep");
        //            //DataTable temp = null;
        //            try
        //            {
        //                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //                cmd.CommandTimeout = 600;
        //                // cmd.CommandText = "select title,trxn_datetime,cash_dispensed1,cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,cash_dispensed7,parsed_transaction_id,atm.atm_id,cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4\r\n                                        from atm, parsed_transaction  where atm.atm_id = parsed_transaction.atm_id \r\n                                        and trxn_datetime >= convert(datetime,'" + DateTime.Today.AddDays(-1.0).ToString("dd/MM/yyyy") + "',103)  and trxn_datetime <= convert(datetime,'" + DateTime.Today.AddDays(-1.0).ToString("dd/MM/yyyy") + " 23:59:59',103) order by trxn_datetime";

        //                cmd.CommandText = @"select title,generated_at,alert_msg,standard_order_type1,standard_order_type2,standard_order_type3,standard_order_type4  from atm_alert,atm where atm.is_active = 1 
        //                                    and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.ATMCassetteFaulty +
        //                                   " and resolve_at is null and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")  and generated_at<convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) ";
        //                DataTable dt = new DataTable();
        //                new SqlDataAdapter(cmd).Fill(dt);
        //                //if (dt.Rows.Count > 0)
        //                //{
        //                //    List<int> atmIDs = new List<int>();
        //                //    foreach (DataRow dr in dt.Rows)
        //                //    {
        //                //        int atmID = int.Parse(dr["atm_id"].ToString());
        //                //        if (!atmIDs.Contains(atmID))
        //                //        {
        //                //            atmIDs.Add(atmID);
        //                //        }
        //                //    }
        //                //    DataTable dtinner = new DataTable();
        //                //    dtinner.Columns.Add("title");
        //                //    dtinner.Columns.Add("status1");
        //                //    dtinner.Columns.Add("status2");
        //                //    dtinner.Columns.Add("status3");
        //                //    dtinner.Columns.Add("status4");
        //                //    dtinner.Columns.Add("trxn_datetime");
        //                //    dtinner.PrimaryKey = new DataColumn[] { dtinner.Columns["title"] };
        //                //    temp = dtinner.Clone();
        //                //    string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(new char[] { ',' });
        //                //    if (parts[0] == "1")
        //                //    {
        //                //        this.ProcessDataTable("cash_dispensed1", dt, dtinner, atmIDs, 1, "cash_remaining1");
        //                //    }
        //                //    if (parts[1] == "1")
        //                //    {
        //                //        this.ProcessDataTable("cash_dispensed2", dt, dtinner, atmIDs, 2, "cash_remaining2");
        //                //    }
        //                //    if (parts[2] == "1")
        //                //    {
        //                //        this.ProcessDataTable("cash_dispensed3", dt, dtinner, atmIDs, 3, "cash_remaining3");
        //                //    }
        //                //    if (parts[3] == "1")
        //                //    {
        //                //        this.ProcessDataTable("cash_dispensed4", dt, dtinner, atmIDs, 4, "cash_remaining4");
        //                //    }
        //                //    dt.Rows.Clear();
        //                //    foreach (DataRow dr in dtinner.Rows)
        //                //    {
        //                //        if ((((dr[1].ToString() == "Faulty") || (dr[2].ToString() == "Faulty")) || (dr[3].ToString() == "Faulty")) || (dr[4].ToString() == "Faulty"))
        //                //        {
        //                //            DataRow newRow = temp.NewRow();
        //                //            newRow[0] = dr[0];
        //                //            newRow[1] = dr[1];
        //                //            newRow[2] = dr[2];
        //                //            newRow[3] = dr[3];
        //                //            newRow[4] = dr[4];
        //                //            newRow[5] = dr[5];
        //                //            temp.Rows.Add(newRow);
        //                //        }
        //                //    }
        //                //    temp.DefaultView.Sort = order;
        //                //}
        //                dt.DefaultView.Sort = order;
        //                this.GridView_FaultyRep.DataSource = dt;
        //                this.GridView_FaultyRep.DataBind();
        //            }
        //            catch (Exception ex)
        //            {
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //            }
        //            finally
        //            {
        //                task.EndTask();
        //            }
        //        }

        //protected void GetLowBalanceATMs()
        //{
        //    LogableTask task = LogableTask.NewTask("GetLowBalanceATMs");
        //    try
        //    {
        //        this.BindLowBalanceGrid("title");
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        private string GetMapping(string columnName)
        {
            if (columnName == "cash_dispensed1")
            {
                return "Cassette1";
            }
            if (columnName == "cash_dispensed2")
            {
                return "Cassette2";
            }
            if (columnName == "cash_dispensed3")
            {
                return "Cassette3";
            }
            if (columnName == "cash_dispensed4")
            {
                return "Cassette4";
            }
            return "Mapping not found";
        }

        //protected void GetPurgeBinAlerts(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetPurgeBinAlerts");
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select title, generated_at, isnull(alert_msg,'') alert_msg\r\n                                    from atm_alert,atm outerATM\r\n                                    where atm_alert.atm_id = outerATM.atm_id \r\n                                    and alert_type_id =22 and outerATM.is_active =1 \r\n                                    and atm_alert_id in \r\n                                    (\r\n                                    select max(atm_alert_id) from atm_alert\r\n                                    where atm_id = outerATM.atm_id\r\n                                    and alert_type_id =22 and resolve_at is null\r\n                                    ) order by " + order;
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        DataRow[] currentDayPurgedBinRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
        //        DataRow[] previousDayPurgedBinRows = dt.Select("generated_at < #" + DateTime.Today + "#");
        //        DataTable dtPurgedBinCurrentDay = this.CreatePurgedDatatableFromDataRows(currentDayPurgedBinRows);
        //        dtPurgedBinCurrentDay.DefaultView.Sort = order;
        //        this.GridView_PurgeBinAlerts_CurrentDay.DataSource = dtPurgedBinCurrentDay;
        //        this.GridView_PurgeBinAlerts_CurrentDay.DataBind();
        //        DataTable dtPurgedBinPreviousDay = this.CreatePurgedDatatableFromDataRows(previousDayPurgedBinRows);
        //        dtPurgedBinPreviousDay.DefaultView.Sort = order;
        //        this.GridView_PurgeBinAlert_PreviousDay.DataSource = dtPurgedBinPreviousDay;
        //        this.GridView_PurgeBinAlert_PreviousDay.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //protected void GetPurgedCounterTrxn(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetPurgedCounterTrxn");
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;\r\n                                     select title,COUNT(parsed_transaction_id) count\r\n                                        from parsed_transaction inner join atm \r\n                                        on parsed_transaction.atm_id = atm.atm_id                                        where amount =0 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")                                         and trxn_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and trxn_datetime <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) group by title order by " + order;
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable();
        //        adapter.Fill(dt);
        //        dt.DefaultView.Sort = order;
        //        this.GridView_PurgedCounter.DataSource = dt;
        //        this.GridView_PurgedCounter.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        protected void GetRecentActivityHTML(int day, ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("GetRecentActivityHTML");
            try
            {
                //SqlCommand cmd = null;
                DataTable dt = new DataTable();
                if (cmd == null)
                    cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandText = string.Format("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select dbo.GetStatusFriendlyName(status) Status, count(1) Count, task_type.task_type_name Task from task, task_type, user_atms where task.task_type_id = task_type.task_type_id and user_atms.atm_id = task.atm_id and user_atms.user_id = {1} and task.creation_time > convert(datetime, '{0}', 120) and task.creation_time < convert(datetime, '{0} 23:59:59', 120) group by status, task_type.task_type_name \r\n                                                    union\r\n                                                    select dbo.GetFTPStatusFriendlyName(status), count(1) count, task_type.task_type_name from ftp_file_info, task_type, user_organization, region where ftp_file_info.task_type_id = task_type.task_type_id and ftp_file_info.region_id = region.region_id and user_organization.org_mcn = region.mcn and user_organization.user_id = {1} and ftp_file_info.creation_time > convert(datetime, '{0} 00:00:00', 120) and ftp_file_info.creation_time < convert(datetime, '{0} 23:59:59', 120) group by status, task_type.task_type_name order by task_type.task_type_name", DateTime.Today.AddDays((double)day).ToString("yyyy-MM-dd"), ((AppUser)this.Session[SessionVars.user.ToString()]).UserId);
                cmd.CommandTimeout = 600;
                new SqlDataAdapter(cmd).Fill(dt);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "fetched");
                this.GridView_RecentActivity.Visible = true;
                this.GridView_RecentActivity.DataSource = dt;
                this.GridView_RecentActivity.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        //protected void GetScheduleSummary()
        //{
        //    LogableTask task = LogableTask.NewTask("GetScheduleSummary");
        //    SqlConnection conn = null;
        //    try
        //    {
        //        SqlCommand cmd = null;
        //        SqlDataReader reader = null;
        //        DataTable dt = new DataTable();
        //        dt.Columns.Add("Activity");
        //        dt.Columns.Add("Count");
        //        conn = ConnectionFactory.GetNewConnection();
        //        conn.Open();
        //        cmd = conn.CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED";
        //        cmd.ExecuteNonQuery();
        //        cmd.CommandText = string.Concat(new object[] { "select count(*) \r\n                                    from scheduled_downloads, user_atms,atm \r\n                                    where user_atms.atm_id = scheduled_downloads.atm_id and user_atms.atm_id = atm.atm_id and atm.is_active = 1\r\n                                    and file_type_id = 1 and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, ";select count(*) \r\n                                    from task, user_atms, atm \r\n                                    where user_atms.atm_id = task.atm_id and atm.atm_id = atm.atm_id and atm.is_active = 1\r\n                                    and task.creation_time >= convert(datetime, '", DateTime.Today.ToString("yyyy-MM-dd"), "', 120)  and task.creation_time <= convert(datetime, '", DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd"), "', 120) and file_type_id = 1 and status in ('downloadedParsePending','downloadedParsing','completed') and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, ";select count(*) \r\n                                    from task, user_atms, atm  \r\n                                    where user_atms.atm_id = task.atm_id and user_atms.atm_id = atm.atm_id and atm.is_active = 1 \r\n                                    and task.creation_time >= convert(datetime, '", DateTime.Today.ToString("yyyy-MM-dd"), "', 120)  and task.creation_time <= convert(datetime, '", DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd"), "', 120)   and file_type_id = 1 and status in ('retriesExhausted','cancelled','unknownError')   and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, ";" });
        //        reader = cmd.ExecuteReader();
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "fetched");
        //        reader.Read();
        //        DataRow dr = dt.NewRow();
        //        dr[0] = "Scheduled";
        //        dr[1] = reader[0];
        //        dt.Rows.Add(dr);
        //        reader.NextResult();
        //        reader.Read();
        //        dr = dt.NewRow();
        //        dr[0] = "Downloaded";
        //        dr[1] = reader[0];
        //        dt.Rows.Add(dr);
        //        reader.NextResult();
        //        reader.Read();
        //        dr = dt.NewRow();
        //        dr[0] = "Failed";
        //        dr[1] = reader[0];
        //        dt.Rows.Add(dr);
        //        reader.Close();
        //        this.GridView_DownloadSummary.DataSource = dt;
        //        this.GridView_DownloadSummary.DataBind();
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "strings built");
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //        if (conn != null)
        //            conn.Close();
        //    }
        //}

        //protected void GetSuggestedReplenishments(string order)
        //{
        //    LogableTask task = LogableTask.NewTask("GetSuggestedReplenishments");
        //    try
        //    {
        //        SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select title, cassette1_notes,cassette2_notes,cassette3_notes,cassette4_notes,standard_order_type1,standard_order_type2,standard_order_type3,standard_order_type4 from cash_position inner join atm \r\n                                    on cash_position.atm_id = atm.atm_id \r\n                                    where  atm.is_active=1 and last_trxn_at >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  and last_trxn_at <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and (" + this.FilterConfiguredCassettes() + " ) and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")  order by " + order;
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        DataTable dtSuggestedRep = new DataTable();
        //        adapter.Fill(dtSuggestedRep);
        //        this.GridView_SuggestedRep.DataSource = dtSuggestedRep;
        //        this.GridView_SuggestedRep.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //private void GetThresholdOfHour(int hr, string orderby)
        //{
        //    LogableTask task = LogableTask.NewTask("GetThresholdOfHour");
        //    SqlConnection conn = null;
        //    SqlDataReader reader = null;
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("title");
        //    dt.Columns.Add("generated_at", typeof(DateTime));
        //    dt.Columns.Add("amount");
        //    dt.Columns.Add("last_heart_beat_at");
        //    dt.Columns.Add("is_healthy", typeof(bool));
        //    try
        //    {
        //        conn = ConnectionFactory.GetNewConnection();
        //        conn.Open();
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.CommandTimeout = 600;
        //        cmd.CommandText = @" select outerATM.atm_id, max(trxn_datetime) trxn_datetime," +
        //                           " (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at " +
        //                           " from atm outerATM left join parsed_transaction " +
        //                           " on parsed_transaction.atm_id = outerATM.atm_id " +
        //                           " where is_active = 1 and is_counter_enabled=1 " +
        //                           " and allowed_inactivity_period is not null " +
        //                           " and outerATM.ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
        //                           " group by outerATM.atm_id";
        //        reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            Atm atm = Atm.LoadAtmByPk(reader.GetInt32(0));
        //            if (reader[1] != DBNull.Value)
        //            {
        //                DateTime maxDateTime = reader.GetDateTime(1);
        //                if (maxDateTime.AddHours((double)hr) < DateTime.Now)
        //                {
        //                    ParsedTransaction parsedTransaction = ParsedTransaction.LoadParsedTransaction(string.Concat(new object[] { "trxn_datetime=convert(datetime,'", maxDateTime.ToString("dd/MM/yyyy HH:mm:ss"), "',103) and atm_id = ", atm.ATMId }));
        //                    DataRow dr = dt.NewRow();
        //                    dr[0] = atm.Title;
        //                    dr[1] = maxDateTime;
        //                    if (parsedTransaction != null)
        //                    {
        //                        dr[2] = parsedTransaction.Amount;
        //                    }
        //                    if (reader[2] != DBNull.Value)
        //                    {
        //                        dr[3] = reader.GetDateTime(2).ToString("dd/MM/yyyy HH:mm:ss");
        //                    }
        //                    else
        //                    {
        //                        dr[3] = "-";
        //                    }
        //                    dr[4] = atm.IsHealthy;
        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //        }
        //        reader.Close();


        //        //For EJ
        //        cmd.CommandText = @" select outerATM.atm_id, max(trxn_datetime) trxn_datetime," +
        //                           " (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at " +
        //                           " from atm outerATM left join ej_parsed_transactions " +
        //                           " on ej_parsed_transactions.atm_id = outerATM.atm_id " +
        //                           " where is_active = 1 and is_ej_enabled=1" +
        //                           " and allowed_inactivity_period is not null " +
        //                           " and outerATM.ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
        //                           " group by outerATM.atm_id";
        //        reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            Atm atm = Atm.LoadAtmByPk(reader.GetInt32(0));
        //            if (reader[1] != DBNull.Value)
        //            {
        //                DateTime maxDateTime = reader.GetDateTime(1);
        //                if (maxDateTime.AddHours((double)hr) < DateTime.Now)
        //                {
        //                    EjParsedTransactions parsedTransaction = EjParsedTransactions.LoadEjParsedTransactions(string.Concat(new object[] { "trxn_datetime=convert(datetime,'", maxDateTime.ToString("dd/MM/yyyy HH:mm:ss"), "',103) and atm_id = ", atm.ATMId }));
        //                    DataRow dr = dt.NewRow();
        //                    dr[0] = atm.Title;
        //                    dr[1] = maxDateTime;
        //                    if (parsedTransaction != null)
        //                    {
        //                        dr[2] = parsedTransaction.Amount;
        //                    }
        //                    if (reader[2] != DBNull.Value)
        //                    {
        //                        dr[3] = reader.GetDateTime(2).ToString("dd/MM/yyyy HH:mm:ss");
        //                    }
        //                    else
        //                    {
        //                        dr[3] = "-";
        //                    }
        //                    dr[4] = atm.IsHealthy;
        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //        }
        //        reader.Close();
        //        //

        //        dt.DefaultView.Sort = orderby;
        //        if (hr == 1)
        //        {
        //            this.GridView_Threshold1.DataSource = dt;
        //            this.GridView_Threshold1.DataBind();
        //        }
        //        else if (hr == 4)
        //        {
        //            this.GridView_Threshold4.DataSource = dt;
        //            this.GridView_Threshold4.DataBind();
        //        }
        //        else if (hr == 8)
        //        {
        //            this.GridView_Threshold8.DataSource = dt;
        //            this.GridView_Threshold8.DataBind();
        //        }
        //        else if (hr == 0x10)
        //        {
        //            this.GridView_Threshold16.DataSource = dt;
        //            this.GridView_Threshold16.DataBind();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //    }
        //    finally
        //    {
        //        if (conn != null)
        //            conn.Close();
        //        task.EndTask();
        //    }
        //}

        private void GetTop10LowTransactingATM(ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("GetTop10LowTransactingATM");
            DataTable dt = null;
            try
            {
                //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //cmd.CommandTimeout = 600;
                lock (syncObject)
                {
                    if (Cache["GetTop10LowTransactingATM"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        cmd.CommandText = "GetTop10ATMs";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("ForDate", SqlDbType.DateTime));
                        cmd.Parameters[0].Value = DateTime.Today;
                        cmd.Parameters.Add(new SqlParameter("order", SqlDbType.Int));
                        cmd.Parameters[1].Value = 0;
                        cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                        cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        Cache.Insert("GetTop10LowTransactingATM", dt, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                        dt = (DataTable)Cache["GetTop10LowTransactingATM"];
                }
                this.GridView_NoTransactingATM.DataSource = dt;
                this.GridView_NoTransactingATM.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private void GetTop10TransactingATM(ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("GetTop10TransactingATM");
            DataTable dt = null;
            try
            {
                //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //cmd.CommandTimeout = 600;
                lock (syncObject)
                {
                    if (Cache["GetTop10TransactingATM"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();
                        cmd.CommandText = "GetTop10ATMs";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("ForDate", SqlDbType.DateTime));
                        cmd.Parameters[0].Value = DateTime.Today;
                        cmd.Parameters.Add(new SqlParameter("order", SqlDbType.Int));
                        cmd.Parameters[1].Value = 1;
                        cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                        cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];


                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        Cache.Insert("GetTop10TransactingATM", dt, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                    else
                        dt = (DataTable)Cache["GetTop10TransactingATM"];
                }
                this.GridView_Transacting.DataSource = dt;
                this.GridView_Transacting.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        //        protected void GetTopATMHTML(string order)
        //        {
        //            LogableTask task = LogableTask.NewTask("GetTopATMHTML");
        //            try
        //            {
        //                SqlCommand cmd = null;
        //                DataTable dt = new DataTable();
        //                DataTable dtEj = new DataTable();
        //                cmd = ConnectionFactory.GetNewConnection().CreateCommand();
        //                cmd.CommandTimeout = 600;
        //                cmd.CommandText = string.Concat(new object[] { "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select atm.title Title,count(*) Count from atm inner join parsed_transaction  on atm.atm_id = parsed_transaction.atm_id  inner join user_atms on user_atms.atm_id = atm.atm_id  where trxn_datetime >= convert(datetime, '", DateTime.Today.ToString("yyyy-MM-dd"), "', 120)  and trxn_datetime <= convert(datetime, '", DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd"), "', 120)  and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " group by title order by ", order });
        //                new SqlDataAdapter(cmd).Fill(dt);
        //                cmd.CommandText = @"select atm.title Title,count(*) Count 
        //                                    from atm inner join ej_parsed_transactions  on atm.atm_id = ej_parsed_transactions.atm_id  
        //                                    inner join user_atms on user_atms.atm_id = atm.atm_id  
        //                                    where trxn_datetime >= convert(datetime, '" + DateTime.Today.ToString("yyyy-MM-dd") + "', 120)  " +
        //                                    " and trxn_datetime <= convert(datetime, '" + DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd") + "', 120) " +
        //                                    " and user_atms.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId + " group by title order by " + order;
        //                new SqlDataAdapter(cmd).Fill(dtEj);

        //                dt.Merge(dtEj);

        //                dt.DefaultView.Sort = "title asc";
        //                this.GridView_TransactionATMs.DataSource = dt;
        //                this.GridView_TransactionATMs.DataBind();
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "string built");
        //            }
        //            catch (Exception ex)
        //            {
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
        //            }
        //            finally
        //            {
        //                task.EndTask();
        //            }
        //        }

        private int GetTransactingATMsCount()
        {
            int transactingATM = 0;
            LogableTask task = LogableTask.NewTask("GetTransactingATMsCount");
            SqlConnection conn = null;
            try
            {
                SqlCommand cmd = null;
                DataTable dt = new DataTable();
                conn = ConnectionFactory.GetNewConnection();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetTransactingATM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Now.AddHours(-1.0);
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.Now;
                cmd.Parameters.Add(new SqlParameter("userID", SqlDbType.Int));
                cmd.Parameters[2].Value = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    transactingATM = (int)result;
                }

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
                transactingATM = -1;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                task.EndTask();
            }
            return transactingATM;
        }

        //protected void GridView_Alerts_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetAlertsHTML((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_Cheque_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetATMsChequeColAlerts((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_CounterMismatch_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetCounterMismatchAlerts((string)this.ViewState["orderby"]);
        //}
        static int count = 0;
        StringBuilder titlesToBlink = null;

        protected void Gridview1_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                titlesToBlink = new StringBuilder();
            }

            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                titlesToBlink.Append(e.Row.Cells[1].Text + ",");
                if (count % 3 == 0)
                {
                    ((System.Web.UI.HtmlControls.HtmlImage)e.Row.Cells[0].FindControl("orangebullet")).Visible = false;
                    ((System.Web.UI.HtmlControls.HtmlImage)e.Row.Cells[0].FindControl("greenbullet")).Visible = false;
                    //e.Row.Cells[0].
                }
                else if (count % 2 == 0)
                {

                    ((System.Web.UI.HtmlControls.HtmlImage)e.Row.Cells[0].FindControl("redbullet")).Visible = false;
                    ((System.Web.UI.HtmlControls.HtmlImage)e.Row.Cells[0].FindControl("greenbullet")).Visible = false;
                }

                else
                {
                    //    e.Row.Cells[0].BackColor = Color.Green;
                    ((System.Web.UI.HtmlControls.HtmlImage)e.Row.Cells[0].FindControl("redbullet")).Visible = false;
                    ((System.Web.UI.HtmlControls.HtmlImage)e.Row.Cells[0].FindControl("orangebullet")).Visible = false;
                    e.Row.CssClass = "cancel";
                    e.Row.Cells[9].Text = "";
                    e.Row.Cells[10].Text = "";

                }


                count++;
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                count = 0;
                hidden_toBlink.Value = titlesToBlink.ToString() + "-1";
            }

        }

        protected void GridView_CurrentDay_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowType == DataControlRowType.DataRow) && !Convert.ToBoolean(((DataRowView)e.Row.DataItem).Row["is_healthy"].ToString()))
            {
                e.Row.Cells[0].ForeColor = Color.Red;
            }
        }
        protected void GridView_LowBalance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((e.Row.RowType == DataControlRowType.DataRow) && ((DataRowView)e.Row.DataItem).Row["resolve_at"].ToString().Length > 0)
            {
                e.Row.Cells[0].BackColor = System.Drawing.ColorTranslator.FromHtml("#54b948");
                e.Row.Cells[0].ForeColor = Color.White;
            }
        }

        //protected void GridView_CurrentDay_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionCurrentDayNoTrxn"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionCurrentDayNoTrxn"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionCurrentDayNoTrxn"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionCurrentDayNoTrxn"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionCurrentDayNoTrxn"] = e.SortExpression;
        //        this.ViewState["sortDirectionCurrentDayNoTrxn"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionCurrentDayNoTrxn"] + " " + this.ViewState["sortDirectionCurrentDayNoTrxn"];
        //    this.GetATMsWithNoTrxnOnCurrentDay((string)this.ViewState["orderby"]);
        //}

        protected void GridView_CurrentOrder_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[1].Text == "Completed")
                {
                    e.Row.Cells[0].BackColor = Color.Green;
                    e.Row.Cells[0].ForeColor = Color.White;
                }
                else if (e.Row.Cells[1].Text.Contains("Suspect"))
                {
                    e.Row.Cells[0].BackColor = Color.Red;
                    e.Row.Cells[0].ForeColor = Color.White;
                }
            }
        }

        //protected void GridView_DatetimeSync_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetDateTimeSyncTask((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_DeadATM_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetDeadATMs((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_Deposit_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetATMsDepositColAlerts((string)this.ViewState["orderby"]);
        //}

        protected void GridView_DFFInfo_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirection"].ToString() == " asc")
                {
                    this.ViewState["sortDirection"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirection"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpression"] = e.SortExpression;
                this.ViewState["sortDirection"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
            this.GetDFFInfo((string)this.ViewState["orderby"]);
        }

        //protected void GridView_FaultyRep_Current_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionCurrentFaultyRep"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionCurrentFaultyRep"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionCurrentFaultyRep"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionCurrentFaultyRep"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionCurrentFaultyRep"] = e.SortExpression;
        //        this.ViewState["sortDirectionCurrentFaultyRep"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionCurrentFaultyRep"] + " " + this.ViewState["sortDirectionCurrentFaultyRep"];
        //    this.GetCurrentFaultyRep((string)this.ViewState["orderby"]);
        //}

        protected void GridView_FaultyRep_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(new char[] { ',' });
                int configuredCassettesCount = 0;
                int faultyCount = 0;
                if (parts[0] == "1")
                {
                    configuredCassettesCount++;
                    if (e.Row.Cells[1].Text == "Faulty")
                    {
                        faultyCount++;
                    }
                }
                if (parts[1] == "1")
                {
                    configuredCassettesCount++;
                    if (e.Row.Cells[2].Text == "Faulty")
                    {
                        faultyCount++;
                    }
                }
                if (parts[2] == "1")
                {
                    configuredCassettesCount++;
                    if (e.Row.Cells[3].Text == "Faulty")
                    {
                        faultyCount++;
                    }
                }
                if (parts[3] == "1")
                {
                    configuredCassettesCount++;
                    if (e.Row.Cells[4].Text == "Faulty")
                    {
                        faultyCount++;
                    }
                }
                e.Row.ForeColor = Color.White;
                if (faultyCount == configuredCassettesCount)
                {
                    e.Row.BackColor = Color.Red;
                }
                if (faultyCount == (configuredCassettesCount - 1))
                {
                    e.Row.BackColor = Color.Orange;
                }
                if (faultyCount == (configuredCassettesCount - 2))
                {
                    e.Row.BackColor = ColorTranslator.FromHtml("#54b948");
                }
            }
        }

        //protected void GridView_FaultyReplenishment_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionFaultyRep"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionFaultyRep"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionFaultyRep"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionFaultyRep"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionFaultyRep"] = e.SortExpression;
        //        this.ViewState["sortDirectionFaultyRep"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionFaultyRep"] + " " + this.ViewState["sortDirectionFaultyRep"];
        //    this.GetFaultyRep((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_LastTrxn_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionHavingLastTrxn"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionHavingLastTrxn"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionHavingLastTrxn"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionHavingLastTrxn"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionHavingLastTrxn"] = e.SortExpression;
        //        this.ViewState["sortDirectionHavingLastTrxn"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionHavingLastTrxn"] + " " + this.ViewState["sortDirectionHavingLastTrxn"];
        //    this.GetATMsHavingLastTrxns((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_LowBalance_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.BindLowBalanceGrid((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_NoLastTrxn_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionHavingNoLastTrxn"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionHavingNoLastTrxn"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionHavingNoLastTrxn"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionHavingNoLastTrxn"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionHavingNoLastTrxn"] = e.SortExpression;
        //        this.ViewState["sortDirectionHavingNoLastTrxn"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionHavingNoLastTrxn"] + " " + this.ViewState["sortDirectionHavingNoLastTrxn"];
        //    this.GetATMsHavingNoLastTrxns((string)this.ViewState["orderby"]);
        //}

        //protected void Gridview_purgebin_sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetPurgeBinAlerts((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_PurgedCounter_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetPurgedCounterTrxn((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_SuggestedReplenishment_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetSuggestedReplenishments((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_Threshold1_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionThreshold1"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionThreshold1"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionThreshold1"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionThreshold1"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionThreshold1"] = e.SortExpression;
        //        this.ViewState["sortDirectionThreshold1"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionThreshold1"] + " " + this.ViewState["sortDirectionThreshold1"];
        //    this.GetThresholdOfHour(1, (string)this.ViewState["orderby"]);
        //}

        //protected void GridView_Threshold16_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionThreshold16"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionThreshold16"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionThreshold16"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionThreshold16"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionThreshold16"] = e.SortExpression;
        //        this.ViewState["sortDirectionThreshold16"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionThreshold16"] + " " + this.ViewState["sortDirectionThreshold16"];
        //    this.GetThresholdOfHour(0x10, (string)this.ViewState["orderby"]);
        //}

        //protected void GridView_Threshold4_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionThreshold4"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionThreshold4"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionThreshold4"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionThreshold4"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionThreshold4"] = e.SortExpression;
        //        this.ViewState["sortDirectionThreshold4"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionThreshold4"] + " " + this.ViewState["sortDirectionThreshold4"];
        //    this.GetThresholdOfHour(4, (string)this.ViewState["orderby"]);
        //}

        //protected void GridView_Threshold8_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionThreshold8"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionThreshold8"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionThreshold8"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionThreshold8"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionThreshold8"] = e.SortExpression;
        //        this.ViewState["sortDirectionThreshold8"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionThreshold8"] + " " + this.ViewState["sortDirectionThreshold8"];
        //    this.GetThresholdOfHour(8, (string)this.ViewState["orderby"]);
        //}

        //protected void GridView_ThresholdReached_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpressionHavingThresholdReached"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirectionHavingThresholdReached"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirectionHavingThresholdReached"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirectionHavingThresholdReached"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpressionHavingThresholdReached"] = e.SortExpression;
        //        this.ViewState["sortDirectionHavingThresholdReached"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpressionHavingThresholdReached"] + " " + this.ViewState["sortDirectionHavingThresholdReached"];
        //    this.GetATMsHavingThresholdReached((string)this.ViewState["orderby"]);
        //}

        //protected void GridView_TransactionATMs_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GetTopATMHTML((string)this.ViewState["orderby"]);
        //}

        private bool isFaulty(DataTable dt, string columnName, string remainingNotesColumnName, string atmID)
        {
            bool output = false;
            object result = dt.Compute("sum(" + columnName + ")", "atm_id=" + atmID + " and " + remainingNotesColumnName + ">0");
            if (result != DBNull.Value)
            {
                if (int.Parse(result.ToString()) != 0)
                {
                    return output;
                }
                DataRow[] dr = dt.Select(columnName + "=0 and atm_id=" + atmID);
                if (dt.Select("atm_id=" + atmID).Length == dr.Length)
                {
                    output = true;
                }
            }
            return output;
        }

        private void BindNoteSetDropDownList()
        {
            NoteSetType.NoteSetTypeReader reader = null;
            try
            {
                int defaultNoteSetId = 0;

                if (Session["org"] != null)
                {
                    DropDownList_NoteSetType.Items.Clear();
                    reader = NoteSetType.ExecuteReader("region_id in (" + Session["org"] + ")");
                    while (reader.Read())
                    {
                        DropDownList_NoteSetType.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));

                        CcmsOrgNoteSet orgNoteSet = CcmsOrgNoteSet.LoadCcmsOrgNoteSet("name='" + reader.CurrentNoteSetType.NoteSetTypeName + "'");
                        if (orgNoteSet != null)
                            if (orgNoteSet.IsDefault.HasValue)
                                if (orgNoteSet.IsDefault.Value)
                                    defaultNoteSetId = reader.CurrentNoteSetType.NoteSetTypeId;


                    }
                    if (DropDownList_NoteSetType.Items.Count > 0 && defaultNoteSetId != 0)
                    {
                        DropDownList_NoteSetType.SelectedValue = defaultNoteSetId.ToString();
                    }
                }

            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }

        private void UpdateServiceStatus(System.Web.UI.WebControls.Image imageOK, System.Web.UI.WebControls.Image imageFailed, bool isWorking)
        {
            imageOK.Visible = isWorking;
            imageFailed.Visible = !imageOK.Visible;
        }

        static string GetServiceStatus(string host, string userName, string password, string serviceName)
        {
            string status = null;
            PermissionSet fullTrust = new PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            fullTrust.Demand();

            //ConnectionOptions connectoptions = new ConnectionOptions();
            //connectoptions.Username = userName;
            //connectoptions.Password = password;
            string ipAddress = host;
            //ManagementScope scope = new System.Management.ManagementScope(@"\\" + ipAddress + @"\root\cimv2");
            //scope.Options = connectoptions;
            SelectQuery query = new SelectQuery("select * from Win32_Service where name = '" + serviceName + "'");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementObject service in collection)
                {
                    if (service["Started"].Equals(true))
                    {
                        status = "Running";
                    }
                    else
                        status = service["Started"].ToString();
                }
                return status;
            }
        }
        private void GetServiceStatus(ref SqlCommand cmd)
        {
            try
            {
                string[] services = { "CurrencyMngServer", "CurrencyParser", "CCMSAlertDispatcher", "CCMSEJParser", "CCMSMonitor", "CCMSOrderDownloader", "CCMSTransactionMonitor", "CurrencyCutOver", "CurrencyReportSchedular", "CCMSVaultManager", "DailyFeedGenerator" };
                //string domain = System.Configuration.ConfigurationManager.AppSettings["domain"];
                //string username = System.Configuration.ConfigurationManager.AppSettings["username"];
                //string password = System.Configuration.ConfigurationManager.AppSettings["password"]; ;// Encryption.Cryptic.DecryptString(System.Configuration.ConfigurationManager.AppSettings["password"]);
                //SignIn.Impersonate impersonate = new SignIn.Impersonate();
                //if (impersonate.DoLogIn(domain, username, password))
                //{
                //    impersonate.DoImpersonate();

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();

                cmd.CommandText = "select * from ccms_services";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)

                //for (int i = 0; i < services.Length; i++)
                {
                    try
                    {
                        DataRow[] arr = dt.Select("name='" + services[i] + "'");
                        //CcmsServices sc = CcmsServices.LoadCcmsServices("name='" + services[i] + "'");


                        if (arr[0]["Service_Status"].ToString() == "Running")
                        {
                            if (i == 0)
                                UpdateServiceStatus(Image_CurrencyServer_Working, Image_CurrencyServer_NotWorking, true);
                            else if (i == 1)
                                UpdateServiceStatus(Image_CurrencyParser_Working, Image_CurrencyParser_NotWorking, true);
                            else if (i == 2)
                                UpdateServiceStatus(Image_AlertDispatcher_Working, Image_AlertDispatcher_NotWorking, true);
                            else if (i == 3)
                                UpdateServiceStatus(Image_EJParser_Working, Image_EJParser_NotWorking, true);
                            else if (i == 4)
                                UpdateServiceStatus(Image_Monitor_Working, Image_Monitor_NotWorking, true);
                            else if (i == 5)
                                UpdateServiceStatus(Image_OrderDownloader_Working, Image_OrderDownloader_NotWorking, true);
                            else if (i == 6)
                                UpdateServiceStatus(Image_TrxnMonitor_Working, Image_TrxnMonitor_NotWorking, true);
                            else if (i == 7)
                                UpdateServiceStatus(Image_CurrencyCutOver_Working, Image_CurrencyCutOver_NotWorking, true);
                            else if (i == 8)
                                UpdateServiceStatus(Image_ReportSchedular_Working, Image_ReportSchedular_NotWorking, true);
                            else if (i == 9)
                                UpdateServiceStatus(Image_CCMSVaultManager_Working, Image_CCMSVaultManager_NotWorking, true);
                            else if (i == 10)
                                UpdateServiceStatus(Image_DailyFeedGenerator_Working, Image_DailyFeedGenerator_NotWorking, true);
                        }
                        else
                        {
                            if (i == 0)
                                UpdateServiceStatus(Image_CurrencyServer_Working, Image_CurrencyServer_NotWorking, false);
                            else if (i == 1)
                                UpdateServiceStatus(Image_CurrencyParser_Working, Image_CurrencyParser_NotWorking, false);
                            else if (i == 2)
                                UpdateServiceStatus(Image_AlertDispatcher_Working, Image_AlertDispatcher_NotWorking, false);
                            else if (i == 3)
                                UpdateServiceStatus(Image_EJParser_Working, Image_EJParser_NotWorking, false);
                            else if (i == 4)
                                UpdateServiceStatus(Image_Monitor_Working, Image_Monitor_NotWorking, false);
                            else if (i == 5)
                                UpdateServiceStatus(Image_OrderDownloader_Working, Image_OrderDownloader_NotWorking, false);
                            else if (i == 6)
                                UpdateServiceStatus(Image_TrxnMonitor_Working, Image_TrxnMonitor_NotWorking, false);
                            else if (i == 7)
                                UpdateServiceStatus(Image_CurrencyCutOver_Working, Image_CurrencyCutOver_NotWorking, false);
                            else if (i == 8)
                                UpdateServiceStatus(Image_ReportSchedular_Working, Image_ReportSchedular_NotWorking, false);
                            else if (i == 9)
                                UpdateServiceStatus(Image_CCMSVaultManager_Working, Image_CCMSVaultManager_NotWorking, false);
                            else if (i == 10)
                                UpdateServiceStatus(Image_DailyFeedGenerator_Working, Image_DailyFeedGenerator_NotWorking, false);
                        }


                    }
                    catch (Exception ex)
                    {
                        LogableTask.LogMonoActivityTask("DisplayServiceStatus", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
                        if (i == 0)
                            UpdateServiceStatus(Image_CurrencyServer_Working, Image_CurrencyServer_NotWorking, false);
                        else if (i == 1)
                            UpdateServiceStatus(Image_CurrencyParser_Working, Image_CurrencyParser_NotWorking, false);
                        else if (i == 2)
                            UpdateServiceStatus(Image_AlertDispatcher_Working, Image_AlertDispatcher_NotWorking, false);
                        else if (i == 3)
                            UpdateServiceStatus(Image_EJParser_Working, Image_EJParser_NotWorking, false);
                        else if (i == 4)
                            UpdateServiceStatus(Image_Monitor_Working, Image_Monitor_NotWorking, false);
                        else if (i == 5)
                            UpdateServiceStatus(Image_OrderDownloader_Working, Image_OrderDownloader_NotWorking, false);
                        else if (i == 6)
                            UpdateServiceStatus(Image_TrxnMonitor_Working, Image_TrxnMonitor_NotWorking, false);
                        else if (i == 7)
                            UpdateServiceStatus(Image_CurrencyCutOver_Working, Image_CurrencyCutOver_NotWorking, false);
                        else if (i == 8)
                            UpdateServiceStatus(Image_ReportSchedular_Working, Image_ReportSchedular_NotWorking, false);
                        else if (i == 9)
                            UpdateServiceStatus(Image_CCMSVaultManager_Working, Image_CCMSVaultManager_NotWorking, false);
                        else if (i == 10)
                            UpdateServiceStatus(Image_DailyFeedGenerator_Working, Image_DailyFeedGenerator_NotWorking, false);
                    }

                    //                    }

                    //impersonate.Revert();

                }
                //else
                //{
                //    LogableTask.LogMonoActivityTask("DisplayServiceStatus", MethodBase.GetCurrentMethod(), TraceLevel.Info, "unable to get service status");

                //    UpdateServiceStatus(Image_CurrencyServer_Working, Image_CurrencyServer_NotWorking, false);

                //    UpdateServiceStatus(Image_CurrencyParser_Working, Image_CurrencyParser_NotWorking, false);

                //    UpdateServiceStatus(Image_AlertDispatcher_Working, Image_AlertDispatcher_NotWorking, false);

                //    UpdateServiceStatus(Image_EJParser_Working, Image_EJParser_NotWorking, false);

                //    UpdateServiceStatus(Image_Monitor_Working, Image_Monitor_NotWorking, false);

                //    UpdateServiceStatus(Image_OrderDownloader_Working, Image_OrderDownloader_NotWorking, false);

                //    UpdateServiceStatus(Image_TrxnMonitor_Working, Image_TrxnMonitor_NotWorking, false);

                //    UpdateServiceStatus(Image_CurrencyCutOver_Working, Image_CurrencyCutOver_NotWorking, false);

                //    UpdateServiceStatus(Image_ReportSchedular_Working, Image_ReportSchedular_NotWorking, false);
                //}


            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("DisplayServiceStatus", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
        }

        private void RevertToExecuteTextComamnds(ref SqlCommand cmd)
        {
            if (cmd != null)
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.Text;
            }
        }
        private void InitializeCommand(ref SqlCommand cmd)
        {
            if (cmd == null)
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 1200;
            }
        }
        private void UpdateHomePage()
        {
            LogableTask task = LogableTask.NewTask("UpdateHomePage");
            SqlCommand cmd = null;
            try
            {


                if (base.CheckRight((int)Permissions.ViewSummary))
                {
                    //Changed done on 25/11/2014
                    // int width = int.Parse(this.Session["width"].ToString()) - 100;
                    // this.container.Attributes.Add("style", "width:" + width.ToString() + "px");

                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to execute ReplenishmentsWithoutOrder at " + DateTime.Now.ToString());
                        this.ReplenishmentsWithoutOrder(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "ReplenishmentsWithoutOrder completed at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling GetTop10LowTransactingATM at " + DateTime.Now.ToString());
                        this.GetTop10LowTransactingATM(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed GetTop10LowTransactingATM at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling GetTop10TransactingATM at " + DateTime.Now.ToString());
                        this.GetTop10TransactingATM(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed GetTop10TransactingATM at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling DisplayHaltedATMs at " + DateTime.Now.ToString());
                        this.DisplayHaltedATMs(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayHaltedATMs at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplaySummaryOfTodaysAlert at " + DateTime.Now.ToString());
                        this.DisplaySummaryOfTodaysAlert(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplaySummaryOfTodaysAlert at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayUserTask at " + DateTime.Now.ToString());
                        this.DisplayUserTask(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayUserTask at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayCurrentOrder at " + DateTime.Now.ToString());
                        this.DisplayCurrentOrder(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayCurrentOrder at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayCallOutSummary at " + DateTime.Now.ToString());
                        this.DisplayCallOutSummary(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayCallOutSummary at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        this.DisplayCurrentLoggedInUser();
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        DrawChequeDepositGraph(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawChequeDepositGraph at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        DrawNotesAcceptorGraph(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawNotesAcceptorGraph at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        DisplayCITTasks();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayCITTasks at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        DrawDFFInfoGraph(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawDFFInfoGraph at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        BindNoteSetDropDownList();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DrawGraph at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        this.DrawGraph(ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawGraph at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }

                    try
                    {
                        RevertToExecuteTextComamnds(ref cmd);
                        GetRecentActivityHTML(0, ref cmd);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed GetRecentActivityHTML at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }





                    Cache["selectedATMIdentifier"] = Session[SessionVars.SelectedATMs.ToString()].GetHashCode();
                    GetServiceStatus(ref cmd);
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        protected void Button_Regenerate_Click(object sender, EventArgs e)
        {
            UpdateHomePage();
            Label_generatedAt.Text = "Home-Generation Completed At " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("SummaryPageLoad");
            try
            {
                Label_AnalysisOnEj.Visible = CheckBox_AnalysisOnEJ.Checked = CheckBox_AnalysisOnEJ.Visible = (System.Configuration.ConfigurationManager.AppSettings["isEJEnabled"] == "1" ? true : false);

                if (!this.Page.IsPostBack && !this.DialogCallBack.CausedCallback)
                {
                    Button_Regenerate.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(Button_Regenerate, null) + ";");

                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LoadHomePage"]) && bool.Parse(ConfigurationManager.AppSettings["LoadHomePage"]))
                        UpdateHomePage();
                    int count = 0;
                    //if (!base.CheckRight(0x6a))
                    //{
                    //    this.TabStripTab_RecentActivity.Visible = false;
                    //    count++;
                    //}
                    ////Changes done by IK on 14/12/2016
                    /////Should not be any call during page load
                    ////else
                    ////{
                    ////    this.RecentActivityMenu1_Click(null, null);
                    ////}
                    //if (!base.CheckRight(0x6b))
                    //{
                    //    this.TabStripTab_Communication.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0x6c))
                    //{
                    //    this.TabStripTab_Alerts.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0x6d))
                    //{
                    //    this.TabStripTab_MostActiveATMs.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(110))
                    //{
                    //    this.TabStripTab_ATMsWithoutTrxns.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0x6f))
                    //{
                    //    this.TabStripTab_Cheque.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0x70))
                    //{
                    //    this.TabStripTab_Deposit.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0x71))
                    //{
                    //    this.TabStripTab_LowBalance.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0xbbc))
                    //{
                    //    this.TabStripTab_PurgeBinAlerts.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0xbb8))
                    //{
                    //    this.TabStripTab_PurgedCount.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0xbb9))
                    //{
                    //    this.TabStripTab_DFF.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0xbcd))
                    //{
                    //    this.TabStripTab_CounterMismatch.Visible = false;
                    //    count++;
                    //}
                    //if (!base.CheckRight(0xbce))
                    //{
                    //    this.TabStripTab_Replenishments.Visible = false;
                    //    count++;
                    //}
                    //// 13 
                    //if (!CheckRight((int)Permissions.ViewDeadATMs))
                    //{
                    //    this.TabStripTab_DeadATMs.Visible = false;
                    //    count++;
                    //}
                    //if (!CheckRight((int)Permissions.ViewDateTimeSync))
                    //{
                    //    this.TabStripTab_DatetimeSync.Visible = false;
                    //    count++;
                    //}

                    // bool isAnyGroupTabVisible = true;

                    //if (!CheckRight((int)Permissions.ViewForcedReplenishments))
                    //{
                    //    TabStripTab17.Visible = false;
                    //    count++;
                    //    isAnyGroupTabVisible = false;
                    //}
                    //if (!CheckRight((int)Permissions.ViewFaultyReplenishments))
                    //{
                    //    TabStripTab18.Visible = false;
                    //    count++;
                    //    isAnyGroupTabVisible = false;
                    //}
                    //if (!CheckRight((int)Permissions.ViewSuggestedReplenishments))
                    //{
                    //    TabStripTab26.Visible = false;
                    //    count++;
                    //    isAnyGroupTabVisible = false;
                    //}

                    //if (!isAnyGroupTabVisible)
                    //    TabStripTab_Replenishments.Visible = false;

                    //isAnyGroupTabVisible = true;
                    //if (!CheckRight((int)Permissions.ViewATMsWithNoTransactionOnCurrentDay))
                    //{
                    //    TabStripTab1.Visible = false;
                    //    count++;
                    //    isAnyGroupTabVisible = false;
                    //}
                    //if (!CheckRight((int)Permissions.ViewATMsHavingThresholdReached))
                    //{
                    //    TabStripTab16.Visible = false;
                    //    count++;
                    //    isAnyGroupTabVisible = false;
                    //}
                    //if (!CheckRight((int)Permissions.ViewATMsHavingLastTransaction))
                    //{
                    //    TabStripTab2.Visible = false;
                    //    count++;
                    //    isAnyGroupTabVisible = false;
                    //}
                    //if (!CheckRight((int)Permissions.ViewATMsHavingNoLastTransaction))
                    //{
                    //    TabStripTab3.Visible = false;
                    //    count++;
                    //    isAnyGroupTabVisible = false;
                    //}
                    //if (!CheckRight((int)Permissions.ViewATMsThresholdHourlyAnalysis))
                    //{
                    //    TabStripTab19.Visible = false;
                    //    count++;
                    //    isAnyGroupTabVisible = false;
                    //}
                    //if (!isAnyGroupTabVisible)
                    //    TabStripTab_ATMsWithoutTrxns.Visible = false;

                    //if (count == 23)
                    //{
                    //    this.literal_script.Text = "<li>You do not have rights to view summary data.Please contact NCR administrator";
                    //}

                    //GetServiceStatus();
                }
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("Summary", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        private void ProcessDataTable(string columnName, DataTable dt2, DataTable dt, List<int> atmIDs, int index, string remainingNotesColumnName)
        {
            DataRow[] dr = null;
            foreach (int atmID in atmIDs)
            {
                DataRow drInner;
                DataRow[] drArray;
                object result = dt2.Compute("sum(" + columnName + ")", string.Concat(new object[] { "atm_id=", atmID, " and ", remainingNotesColumnName, ">0" }));
                if (result != DBNull.Value)
                {
                    if (int.Parse(result.ToString()) == 0)
                    {
                        dr = dt2.Select(columnName + "=0 and atm_id=" + atmID);
                        if (dt2.Select("atm_id=" + atmID).Length == dr.Length)
                        {
                            if (!dt.Rows.Contains(dr[0][0]))
                            {
                                drInner = dt.NewRow();
                                drInner[0] = dr[0][0];
                                drInner[index] = "Faulty";
                                drInner[5] = dr[dr.Length - 1][1].ToString();
                                dt.Rows.Add(drInner);
                            }
                            else
                            {
                                drArray = dt.Select("title='" + dr[0][0] + "'");
                                drArray[0][index] = "Faulty";
                                drArray[0][5] = dr[dr.Length - 1][1].ToString();
                            }
                        }
                    }
                    else
                    {
                        dr = dt2.Select("atm_id=" + atmID);
                        if (!dt.Rows.Contains(dr[0][0]))
                        {
                            drInner = dt.NewRow();
                            drInner[0] = dr[0][0];
                            drInner[index] = "Healthy";
                            drInner[5] = dr[dr.Length - 1][1].ToString();
                            dt.Rows.Add(drInner);
                        }
                        else
                        {
                            drArray = dt.Select("title='" + dr[0][0] + "'");
                            drArray[0][index] = "Healthy";
                            drArray[0][5] = dr[dr.Length - 1][1].ToString();
                        }
                    }
                }
                else
                {
                    dr = dt2.Select("atm_id=" + atmID);
                    if (!dt.Rows.Contains(dr[0][0]))
                    {
                        drInner = dt.NewRow();
                        drInner[0] = dr[0][0];
                        drInner[index] = "Empty";
                        drInner[5] = dr[dr.Length - 1][1].ToString();
                        dt.Rows.Add(drInner);
                    }
                    else
                    {
                        drArray = dt.Select("title='" + dr[0][0] + "'");
                        drArray[0][index] = "Empty";
                        drArray[0][5] = dr[dr.Length - 1][1].ToString();
                    }
                }
            }
        }

        protected void RecentActivityMenu1_Click(object sender, EventArgs e)
        {
            //this.GetRecentActivityHTML(0, null);
        }

        protected void RecentActivityMenu2_Click(object sender, EventArgs e)
        {
            //this.GetRecentActivityHTML(-1, null);
        }

        protected void RecentActivityMenu3_Click(object sender, EventArgs e)
        {
            //this.GetRecentActivityHTML(-2, null);
        }

        protected void RecentActivityMenu4_Click(object sender, EventArgs e)
        {
            //this.GetRecentActivityHTML(-6, null);
        }

        private void ReplenishmentsWithoutOrder(ref SqlCommand cmd)
        {
            LogableTask task = LogableTask.NewTask("ReplenishmentsWithoutOrder");
            DataTable dt = null;
            try
            {
                lock (syncObject)
                {
                    if (Cache["GetRepWithoutOrders"] == null || ((int)Cache["selectedATMIdentifier"] != Session[SessionVars.SelectedATMs.ToString()].GetHashCode()))
                    {
                        InitializeCommand(ref cmd);
                        cmd.Parameters.Clear();

                        cmd.CommandText = "GetRepWithoutOrders";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                        cmd.Parameters[0].Value = Session[SessionVars.SelectedATMs.ToString()];
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);
                        Cache.Insert("GetRepWithoutOrders", dt, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);

                    }
                    else
                        dt = (DataTable)Cache["GetRepWithoutOrders"];
                }

                this.GridView_Rep_Without_Order.DataSource = dt;
                this.GridView_Rep_Without_Order.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        //protected void TabStrip_Summary_ItemSelected(object sender, TabStripTabEventArgs e)
        //{
        //    this.ViewState["SortExpression"] = "title";
        //    this.ViewState["sortDirection"] = " asc";
        //    switch (e.Tab.ID)
        //    {
        //        case "TabStripTab28":
        //            DisplayOrders();
        //            break;

        //        case "TabStripTab_RecentActivity":
        //            this.RecentActivityMenu1_Click(null, null);
        //            break;

        //        case "TabStripTab_Alerts":
        //        case "TabStripTab12":
        //        case "TabStripTab13":
        //            this.GetAlertsHTML(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_Communication":
        //            this.GetScheduleSummary();
        //            break;

        //        case "TabStripTab_Transactions":
        //            this.GetTopATMHTML(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_MostActiveATMs":
        //            this.GetTopATMHTML(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_ATMsWithoutTrxns":
        //        case "TabStripTab1":
        //            this.ViewState["SortExpressionCurrentDayNoTrxn"] = "title";
        //            this.ViewState["sortDirectionCurrentDayNoTrxn"] = " asc";
        //            this.GetATMsWithNoTrxnOnCurrentDay(((string)this.ViewState["SortExpressionCurrentDayNoTrxn"]) + ((string)this.ViewState["sortDirectionCurrentDayNoTrxn"]));
        //            break;

        //        case "TabStripTab2":
        //            this.ViewState["SortExpressionHavingLastTrxn"] = "title";
        //            this.ViewState["sortDirectionHavingLastTrxn"] = " asc";
        //            this.GetATMsHavingLastTrxns(((string)this.ViewState["SortExpressionHavingLastTrxn"]) + ((string)this.ViewState["sortDirectionHavingLastTrxn"]));
        //            break;

        //        case "TabStripTab3":
        //            this.ViewState["SortExpressionHavingNoLastTrxn"] = "title";
        //            this.ViewState["sortDirectionHavingNoLastTrxn"] = " asc";
        //            this.GetATMsHavingNoLastTrxns(((string)this.ViewState["SortExpressionHavingNoLastTrxn"]) + ((string)this.ViewState["sortDirectionHavingNoLastTrxn"]));
        //            break;

        //        case "TabStripTab16":
        //            this.ViewState["SortExpressionHavingThresholdReached"] = "title";
        //            this.ViewState["sortDirectionHavingThresholdReached"] = " asc";
        //            this.GetATMsHavingThresholdReached(((string)this.ViewState["SortExpressionHavingThresholdReached"]) + ((string)this.ViewState["sortDirectionHavingThresholdReached"]));
        //            break;

        //        case "TabStripTab_Cheque":
        //        case "TabStripTab8":
        //        case "TabStripTab9":
        //            this.GetATMsChequeColAlerts(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_Deposit":
        //        case "TabStripTab10":
        //        case "TabStripTab11":
        //            this.GetATMsDepositColAlerts(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_LowBalance":
        //        case "TabStripTab4":
        //        case "TabStripTab5":
        //            this.BindLowBalanceGrid(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_PurgeBinAlerts":
        //        case "TabStripTab6":
        //        case "TabStripTab7":
        //            this.GetPurgeBinAlerts(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_DeadATMs":
        //            this.GetDeadATMs(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_DatetimeSync":
        //            this.GetDateTimeSyncTask(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_PurgedCount":
        //            this.GetPurgedCounterTrxn(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_DFF":
        //            this.GetDFFInfo(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_CounterMismatch":
        //        case "TabStripTab14":
        //        case "TabStripTab15":
        //            this.GetCounterMismatchAlerts(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab_Replenishments":
        //        case "TabStripTab17":
        //            this.GetSuggestedReplenishments(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
        //            break;

        //        case "TabStripTab18":
        //        case "TabStripTab24":
        //            this.ViewState["SortExpressionCurrentFaultyRep"] = "title";
        //            this.ViewState["sortDirectionCurrentFaultyRep"] = " asc";
        //            this.GetCurrentFaultyRep(((string)this.ViewState["SortExpressionCurrentFaultyRep"]) + ((string)this.ViewState["sortDirectionCurrentFaultyRep"]));
        //            break;

        //        case "TabStripTab25":
        //            this.ViewState["SortExpressionFaultyRep"] = "title";
        //            this.ViewState["sortDirectionFaultyRep"] = " asc";
        //            this.GetFaultyRep(((string)this.ViewState["SortExpressionFaultyRep"]) + ((string)this.ViewState["sortDirectionFaultyRep"]));
        //            break;

        //        case "TabStripTab20":
        //        case "TabStrip6":
        //        case "TabStripTab19":
        //            this.ViewState["SortExpressionThreshold1"] = "title";
        //            this.ViewState["sortDirectionThreshold1"] = " asc";
        //            this.GetThresholdOfHour(1, ((string)this.ViewState["SortExpressionThreshold1"]) + ((string)this.ViewState["sortDirectionThreshold1"]));
        //            break;

        //        case "TabStripTab21":
        //            this.ViewState["SortExpressionThreshold4"] = "title";
        //            this.ViewState["sortDirectionThreshold4"] = " asc";
        //            this.GetThresholdOfHour(4, ((string)this.ViewState["SortExpressionThreshold4"]) + ((string)this.ViewState["sortDirectionThreshold4"]));
        //            break;

        //        case "TabStripTab22":
        //            this.ViewState["SortExpressionThreshold8"] = "title";
        //            this.ViewState["sortDirectionThreshold8"] = " asc";
        //            this.GetThresholdOfHour(8, ((string)this.ViewState["SortExpressionThreshold8"]) + ((string)this.ViewState["sortDirectionThreshold8"]));
        //            break;

        //        case "TabStripTab23":
        //            this.ViewState["SortExpressionThreshold16"] = "title";
        //            this.ViewState["sortDirectionThreshold16"] = " asc";
        //            this.GetThresholdOfHour(0x10, ((string)this.ViewState["SortExpressionThreshold16"]) + ((string)this.ViewState["sortDirectionThreshold16"]));
        //            break;
        //        case "TabStripTab26":
        //            this.ViewState["SortExpressionThreshold16"] = "title";
        //            this.ViewState["sortDirectionThreshold16"] = " asc";
        //            this.BindForecastingGrid("title asc");

        //            break;
        //    }
        //}
        //private void BindForecastingGrid(string orderby)
        //{
        //    DataTable dt = GenerateForecastDataTable();
        //    dt.DefaultView.Sort = orderby;
        //    GridView1.DataSource = dt;
        //    GridView1.DataBind();


        //    DataTable dt1 = new DataTable();
        //    dt1.Columns.Add("title");
        //    dt1.Columns.Add("rep_datetime");
        //    dt1.Columns.Add("replenishment");
        //    dt1.Columns.Add("type1");
        //    dt1.Columns.Add("type2");
        //    dt1.Columns.Add("type3");
        //    dt1.Columns.Add("type4");
        //    dt1.Columns.Add("location");
        //    dt1.Columns.Add("balance");

        //    StringBuilder builder = new StringBuilder();
        //    int count = 0;


        //    foreach (DataRow dr in dt.Rows)
        //        builder.Append("'" + dr[1] + "',");

        //    Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1  and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and title not in (" + builder.ToString() + "'1')");
        //    while (atmReader.Read())
        //    {
        //        count++;
        //        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atmReader.CurrentAtm.NoteSetTypeId);

        //        DataRow dr = dt1.NewRow();
        //        dr[0] = atmReader.CurrentAtm.Title;
        //        dr[1] = DateTime.Now.DayOfWeek + "," + DateTime.Now.Day + "-" + DateTime.Now.ToString("MMM") + "-" + DateTime.Now.Year;
        //        dr[2] = 1500000;
        //        dr[3] = 2000;
        //        dr[4] = 4000;
        //        dr[5] = 2000;
        //        dr[6] = 0;
        //        dr[7] = atmReader.CurrentAtm.Location;
        //        CashPosition cashPosition = CashPosition.LoadCashPosition("cash_position_id in (select max(cash_position_id) from cash_position where atm_id=" + atmReader.CurrentAtm.ATMId + ")");
        //        if (cashPosition != null)
        //        {
        //            dr[8] = cashPosition.Cassette1Notes * noteSetType.DenominationType1 +
        //                cashPosition.Cassette2Notes * noteSetType.DenominationType2 +
        //                cashPosition.Cassette3Notes * noteSetType.DenominationType3 +
        //                cashPosition.Cassette4Notes * noteSetType.DenominationType4;
        //        }
        //        //if (count == 10)
        //        //    break;
        //        dt1.Rows.Add(dr);


        //    }
        //    atmReader.Close();
        //    GridView2.DataSource = dt1;
        //    GridView2.DataBind();




        //}
        private static decimal MakeNumberDivisibleBy50(decimal number)
        {
            while (number % 50 != 0)
                number++;

            return number;
        }
        private static decimal MakeNumberDivisibleBy500(decimal number)
        {
            while (number % 500 != 0)
                number++;

            return number;
        }
        private static decimal MakeNumberDivisibleBy100(decimal number)
        {
            while (number % 100 != 0)
                number++;

            return number;
        }


        private static decimal MakeNumberDivisibleBy10(decimal number)
        {
            while (number % 10 != 0)
                number++;

            return number;
        }
        //DataTable GenerateForecastDataTable()
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("title");
        //    dt.Columns.Add("rep_datetime");
        //    dt.Columns.Add("replenishment");
        //    dt.Columns.Add("type1");
        //    dt.Columns.Add("type2");
        //    dt.Columns.Add("type3");
        //    dt.Columns.Add("type4");
        //    dt.Columns.Add("location");
        //    dt.Columns.Add("balance");
        //    //dt.Columns.Add("date");

        //    // decimal standardOrderAmount = 50*1000+100*1000+500*2000;
        //    decimal openingBalance = 0;
        //    decimal rep = 0;
        //    decimal balance = 0;
        //    decimal preWithdrawals = 0;
        //    decimal returns = 0;

        //    decimal amountExpForType1 = 0;
        //    decimal amountExpForType2 = 0;
        //    decimal amountExpForType3 = 0;
        //    decimal amountExpForType4 = 0;
        //    decimal standardOrderAmount = 0;
        //    NoteSetType notesetType = null;
        //    int windowSize = 3;
        //    List<TransactionHistory> withdrawalHistory = null;
        //    List<decimal> forecastedWithdrawals = null;
        //    //List<decimal> withdrawals = new List<decimal>();
        //    List<DateTime> events = new List<DateTime>();
        //    //decimal forecastedWithdrawals = 0;
        //    TransactionHistory history = null;
        //    Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1 and is_active=1 and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ");
        //    while (atmReader.Read())
        //    {
        //        forecastedWithdrawals = new List<decimal>();
        //        withdrawalHistory = new List<TransactionHistory>();
        //        notesetType = NoteSetType.LoadNoteSetTypeByPk(atmReader.CurrentAtm.NoteSetTypeId);
        //        standardOrderAmount = (decimal)(notesetType.DenominationType1 * 2000 +
        //            notesetType.DenominationType2 * 4000 +
        //            notesetType.DenominationType3 * 2000);

        //        Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atmReader.CurrentAtm.ATMId + " and trxn_datetime >=convert(datetime,'" + DateTime.Now.AddMonths(-1).AddDays(-windowSize).ToString("dd/MM/yyyy") + "',103) and trxn_datetime<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + "',103) order by trxn_datetime");
        //        while (summaryReader.Read())
        //        {
        //            history = new TransactionHistory();
        //            history.trxnDatetime = summaryReader.CurrentSummary.TrxnDatetime;
        //            history.amount = summaryReader.CurrentSummary.Withdrawals;
        //            //withdrawals.Add(summaryReader.CurrentSummary.Withdrawals);
        //            withdrawalHistory.Add(history);
        //        }
        //        summaryReader.Close();

        //        if (withdrawalHistory.Count == 0)
        //            continue;

        //        //int[] dataPoints = { 119, 72, 113, 82, 82, 131, 111, 116, 89, 95, 88, 90 };
        //        int toLoop = withdrawalHistory.Count - windowSize;
        //        decimal sum = 0;
        //        bool added = false;
        //        int itemAdded = 0;
        //        DateTime temp = DateTime.MinValue;
        //        DateTime forecastedDay = DateTime.MinValue;

        //        int tempCount = 0;
        //        for (int j = 0; j < toLoop; j++)
        //        {
        //            tempCount = 0;
        //            sum = 0;
        //            added = false;
        //            itemAdded = 0;
        //            for (int k = j; k < (j + windowSize); k++)
        //            {
        //                if (tempCount == 0)
        //                {
        //                    temp = withdrawalHistory[k].trxnDatetime;
        //                    forecastedDay = temp.AddDays(windowSize);
        //                    tempCount++;
        //                }
        //                if (events.Contains(withdrawalHistory[k].trxnDatetime))
        //                {
        //                    for (int l = 0; l < withdrawalHistory.Count; l++)
        //                    {
        //                        if (withdrawalHistory[l].trxnDatetime == forecastedDay)
        //                        {
        //                            sum = withdrawalHistory[l].amount;
        //                            forecastedWithdrawals.Add(MakeNumberDivisibleBy50(sum));
        //                            added = true;
        //                            break;
        //                        }
        //                    }


        //                    //if (temp == withdrawalHistory[k].trxnDatetime) // Event Day
        //                    //{
        //                    //    sum = withdrawalHistory[k].amount;
        //                    //    forecastedWithdrawals.Add(MakeNumberDivisibleBy50(sum));
        //                    //    added = true;
        //                    //    break;
        //                    //}
        //                    //else
        //                    //{
        //                    //    if (forecastedWithdrawals.Count > 0)
        //                    //    {
        //                    //        forecastedWithdrawals.Add(forecastedWithdrawals[forecastedWithdrawals.Count - 1]);
        //                    //        added = true;
        //                    //        break;
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        forecastedWithdrawals.Add(MakeNumberDivisibleBy50(Math.Round(sum / itemAdded)));
        //                    //        added = true;
        //                    //        break;
        //                    //    }
        //                    //}
        //                }
        //                else
        //                {
        //                    sum += withdrawalHistory[k].amount;
        //                    itemAdded++;
        //                }

        //            }
        //            if (!added)
        //                forecastedWithdrawals.Add(MakeNumberDivisibleBy50(Math.Round(sum / windowSize)));
        //        }
        //        decimal expectedAmount = 0;
        //        for (int j = 0; j < forecastedWithdrawals.Count; j++)
        //        {
        //            expectedAmount = 0;
        //            preWithdrawals = 0;
        //            amountExpForType1 = 0;
        //            amountExpForType2 = 0;
        //            amountExpForType3 = 0;
        //            amountExpForType4 = 0;

        //            returns = 0;
        //            //Console.WriteLine("Day " + (j + 1 + windowSize) + " -->" + Math.Round(sum / windowSize));
        //            DataRow dr = dt.NewRow();
        //            DateTime forecastedDateTime = withdrawalHistory[j].trxnDatetime.AddDays(windowSize);//.AddYears(1);
        //            dr[1] = forecastedDateTime.DayOfWeek + "," + forecastedDateTime.Day + "-" + forecastedDateTime.ToString("MMM") + "-" + forecastedDateTime.Year;
        //            dr[0] = atmReader.CurrentAtm.Title;
        //            if (openingBalance == 0)
        //                openingBalance = standardOrderAmount;
        //            //dr[1] = openingBalance;


        //            if (openingBalance < forecastedWithdrawals[j])
        //            {
        //                decimal upperBound = 1; //Trips per week.
        //                if (j + 1 + upperBound < forecastedWithdrawals.Count)
        //                {
        //                    for (int k = 0; k < upperBound; k++)
        //                    {
        //                        expectedAmount += forecastedWithdrawals[k + j];
        //                    }
        //                    expectedAmount += forecastedWithdrawals[j + (int)upperBound] / 3;
        //                    rep = Math.Round(expectedAmount);
        //                    rep = MakeNumberDivisibleBy50(rep);
        //                    amountExpForType1 = Math.Round(rep * 0.05M);
        //                    amountExpForType2 = Math.Floor(rep * 0.1M);
        //                    amountExpForType3 = Math.Round(rep * 0.85M);
        //                    amountExpForType4 = Math.Floor(rep * 0);

        //                    amountExpForType1 = MakeNumberDivisibleBy50(amountExpForType1);
        //                    amountExpForType2 = MakeNumberDivisibleBy100(amountExpForType2);
        //                    amountExpForType3 = MakeNumberDivisibleBy500(amountExpForType3);
        //                    amountExpForType4 = 0;

        //                    if (amountExpForType1 + amountExpForType2 + amountExpForType3 > rep)
        //                    {
        //                        decimal diff = (amountExpForType1 + amountExpForType2 + amountExpForType3) - rep;

        //                        amountExpForType1 = amountExpForType1 - diff;
        //                    }


        //                }
        //                else
        //                    rep = standardOrderAmount;
        //                decimal notes = openingBalance / 50;


        //                returns = Math.Floor(notes / 2) * 50;
        //                preWithdrawals = Math.Round(notes / 2) * 50;
        //            }
        //            else
        //                rep = 0;

        //            //dr[1] = 
        //            dr[2] = rep;

        //            //dr[3] = preWithdrawals;
        //            //dr[4] = forecastedWithdrawals[j];
        //            //dr[5] = returns;
        //            if (rep > 0)
        //            {
        //                balance = rep - (forecastedWithdrawals[j] - preWithdrawals);
        //            }
        //            else
        //                balance = openingBalance - forecastedWithdrawals[j];
        //            openingBalance = Math.Round(balance);
        //            //dr[6] = openingBalance;
        //            dr[3] = Math.Round(amountExpForType1 / 50);
        //            dr[4] = Math.Round(amountExpForType2 / 100);
        //            dr[5] = Math.Round(amountExpForType3 / 500);
        //            dr[6] = amountExpForType4;
        //            dr[7] = atmReader.CurrentAtm.Location;
        //            dr[8] = MakeNumberDivisibleBy50(Math.Round((decimal)(decimal.Parse(dr[3].ToString()) * notesetType.DenominationType1 +
        //                decimal.Parse(dr[4].ToString()) * notesetType.DenominationType2 +
        //                decimal.Parse(dr[5].ToString()) * notesetType.DenominationType3) / 3));
        //            dr[1] = DateTime.Now.DayOfWeek + "," + DateTime.Now.Day + "-" + DateTime.Now.ToString("MMM") + "-" + DateTime.Now.Year;


        //            if (rep > 0)
        //            {
        //                //if (forecastedDateTime.Date == DateTime.Now.Date)
        //                //{
        //                dt.Rows.Add(dr);
        //                break;
        //                //                       }
        //            }



        //        }
        //    }
        //    atmReader.Close();
        //    return dt;
        //}


        //private void DisplayOrders()
        //{

        //    DataTable dt1 = new DataTable();
        //    dt1.Columns.Add("title");
        //    dt1.Columns.Add("rep_datetime");
        //    dt1.Columns.Add("replenishment");
        //    dt1.Columns.Add("type1");
        //    dt1.Columns.Add("type2");
        //    dt1.Columns.Add("type3");
        //    dt1.Columns.Add("type4");
        //    dt1.Columns.Add("location");
        //    dt1.Columns.Add("balance");

        //    StringBuilder builder = new StringBuilder();
        //    int count = 0;
        //    DataTable dt = null;
        //    if (Session["forcedOrders"] != null)
        //    {
        //        dt = (DataTable)Session["forcedOrders"];

        //        foreach (DataRow dr in dt.Rows)
        //            builder.Append("'" + dr[0] + "',");
        //    }
        //    else
        //        dt = new DataTable();

        //    GridView3.DataSource = dt;
        //    GridView3.DataBind();

        //    if (Session["faultyOrders"] != null)
        //    {
        //        dt = (DataTable)Session["faultyOrders"];
        //        foreach (DataRow dr in dt.Rows)
        //            builder.Append("'" + dr[0] + "',");
        //    }
        //    else
        //        dt = new DataTable();
        //    GridView4.DataSource = dt;
        //    GridView4.DataBind();

        //    dt = GenerateForecastDataTable();
        //    foreach (DataRow dr in dt.Rows)
        //        builder.Append("'" + dr[0] + "',");
        //    Session["normal"] = dt;
        //    GridView5.DataSource = dt;
        //    GridView5.DataBind();

        //    Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1 and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and title not in (" + builder.ToString() + "'1')");
        //    while (atmReader.Read())
        //    {
        //        count++;
        //        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atmReader.CurrentAtm.NoteSetTypeId);

        //        DataRow dr = dt1.NewRow();
        //        dr[0] = atmReader.CurrentAtm.Title;
        //        dr[1] = DateTime.Now.DayOfWeek + "," + DateTime.Now.Day + "-" + DateTime.Now.ToString("MMM") + "-" + DateTime.Now.Year;

        //        if (atmReader.CurrentAtm.StandardOrderType1.HasValue)
        //            dr[3] = atmReader.CurrentAtm.StandardOrderType1.Value;
        //        else
        //            dr[3] = 2000;

        //        if (atmReader.CurrentAtm.StandardOrderType2.HasValue)
        //            dr[4] = atmReader.CurrentAtm.StandardOrderType2.Value;
        //        else
        //            dr[4] = 2000;

        //        if (atmReader.CurrentAtm.StandardOrderType3.HasValue)
        //            dr[5] = atmReader.CurrentAtm.StandardOrderType3.Value;
        //        else
        //            dr[5] = 2000;

        //        if (atmReader.CurrentAtm.StandardOrderType4.HasValue)
        //            dr[6] = atmReader.CurrentAtm.StandardOrderType4.Value;
        //        else
        //            dr[6] = 2000;


        //        dr[2] = int.Parse(dr[3].ToString()) * noteSetType.DenominationType1 +
        //            int.Parse(dr[4].ToString()) * noteSetType.DenominationType2 +
        //            int.Parse(dr[5].ToString()) * noteSetType.DenominationType3 +
        //            int.Parse(dr[6].ToString()) * noteSetType.DenominationType4;

        //        dr[7] = atmReader.CurrentAtm.Location;
        //        CashPosition cashPosition = CashPosition.LoadCashPosition("cash_position_id in (select max(cash_position_id) from cash_position where atm_id=" + atmReader.CurrentAtm.ATMId + ")");
        //        if (cashPosition != null)
        //        {
        //            dr[8] = cashPosition.Cassette1Notes * noteSetType.DenominationType1 +
        //                cashPosition.Cassette2Notes * noteSetType.DenominationType2 +
        //                cashPosition.Cassette3Notes * noteSetType.DenominationType3 +
        //                cashPosition.Cassette4Notes * noteSetType.DenominationType4;
        //        }
        //        //if (count == 11)
        //        //    break;
        //        dt1.Rows.Add(dr);


        //    }
        //    atmReader.Close();
        //    GridView6.DataSource = dt1;
        //    GridView6.DataBind();
        //    Session["emergencyOrders"] = dt1;


        //}


        //protected void grdForcedOrders_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        //{
        //    this.GridView3.EditIndex = -1;
        //    if (Session["forcedOrders"] != null)
        //    {
        //        DataTable dt = (DataTable)Session["forcedOrders"];
        //        GridView3.DataSource = dt;
        //        GridView3.DataBind();
        //    }


        //}
        //protected void grdForcedOrders_RowEditing(object sender, GridViewEditEventArgs e)
        //{
        //    this.GridView3.EditIndex = e.NewEditIndex;
        //    if (Session["forcedOrders"] != null)
        //    {
        //        DataTable dt = (DataTable)Session["forcedOrders"];
        //        GridView3.DataSource = dt;
        //        GridView3.DataBind();
        //    }
        //}
        //protected void grdForcedOrders_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    DataTable sourceData = (DataTable)Session["forcedOrders"];
        //    sourceData.Rows[e.RowIndex].Delete();
        //    Session["forcedOrders"] = sourceData;
        //    GridView3.EditIndex = -1;
        //    GridView3.DataSource = sourceData;
        //    GridView3.DataBind();

        //}
        //protected void grdForcedOrders_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{
        //    var newvalues = e.NewValues;

        //    DataTable sourceData = (DataTable)Session["forcedOrders"];
        //    Atm atm = Atm.LoadAtm("title='" + sourceData.Rows[e.RowIndex][0].ToString() + "'");
        //    if (atm != null)
        //    {
        //        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
        //        if (noteSetType != null)
        //        {
        //            sourceData.Rows[e.RowIndex][2] = noteSetType.DenominationType1.Value * int.Parse(e.NewValues[0].ToString()) +
        //                noteSetType.DenominationType2.Value * int.Parse(e.NewValues[1].ToString()) + noteSetType.DenominationType3.Value * int.Parse(e.NewValues[2].ToString())
        //                + noteSetType.DenominationType4.Value * int.Parse(e.NewValues[3].ToString());

        //            sourceData.Rows[e.RowIndex][3] = e.NewValues[0];
        //            sourceData.Rows[e.RowIndex][4] = e.NewValues[1];
        //            sourceData.Rows[e.RowIndex][5] = e.NewValues[2];
        //            sourceData.Rows[e.RowIndex][6] = e.NewValues[3];



        //            GridView3.EditIndex = -1;
        //            GridView3.DataSource = sourceData;
        //            GridView3.DataBind();
        //            Session["forcedOrders"] = sourceData;
        //        }
        //    }
        //    // Update the grid
        //    //DisplayOrders();
        //}


        //protected void grdFaultyOrders_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        //{
        //    this.GridView4.EditIndex = -1;
        //    if (Session["faultyOrders"] != null)
        //    {
        //        DataTable dt = (DataTable)Session["faultyOrders"];
        //        GridView4.DataSource = dt;
        //        GridView4.DataBind();
        //    }


        //}
        //protected void grdFaultyOrders_RowEditing(object sender, GridViewEditEventArgs e)
        //{
        //    this.GridView4.EditIndex = e.NewEditIndex;
        //    if (Session["faultyOrders"] != null)
        //    {
        //        DataTable dt = (DataTable)Session["faultyOrders"];
        //        GridView4.DataSource = dt;
        //        GridView4.DataBind();
        //    }
        //}
        //protected void grdFaultyOrders_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    DataTable sourceData = (DataTable)Session["faultyOrders"];
        //    sourceData.Rows[e.RowIndex].Delete();
        //    Session["forcedOrders"] = sourceData;
        //    GridView4.EditIndex = -1;
        //    GridView4.DataSource = sourceData;
        //    GridView4.DataBind();

        //}
        //protected void grdFaultyOrders_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{
        //    var newvalues = e.NewValues;

        //    DataTable sourceData = (DataTable)Session["faultyOrders"];
        //    Atm atm = Atm.LoadAtm("title='" + sourceData.Rows[e.RowIndex][0].ToString() + "'");
        //    if (atm != null)
        //    {
        //        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
        //        if (noteSetType != null)
        //        {
        //            sourceData.Rows[e.RowIndex][2] = noteSetType.DenominationType1.Value * int.Parse(e.NewValues[0].ToString()) +
        //                noteSetType.DenominationType2.Value * int.Parse(e.NewValues[1].ToString()) + noteSetType.DenominationType3.Value * int.Parse(e.NewValues[2].ToString())
        //                + noteSetType.DenominationType4.Value * int.Parse(e.NewValues[3].ToString());

        //            sourceData.Rows[e.RowIndex][3] = e.NewValues[0];
        //            sourceData.Rows[e.RowIndex][4] = e.NewValues[1];
        //            sourceData.Rows[e.RowIndex][5] = e.NewValues[2];
        //            sourceData.Rows[e.RowIndex][6] = e.NewValues[3];



        //            GridView4.EditIndex = -1;
        //            GridView4.DataSource = sourceData;
        //            GridView4.DataBind();
        //            Session["faultyOrders"] = sourceData;
        //        }
        //    }
        //    // Update the grid
        //    //DisplayOrders();
        //}



        //protected void grdNormalOrders_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        //{
        //    this.GridView5.EditIndex = -1;
        //    if (Session["normal"] != null)
        //    {
        //        DataTable dt = (DataTable)Session["normal"];
        //        GridView5.DataSource = dt;
        //        GridView5.DataBind();
        //    }


        //}
        //protected void grdNormalOrders_RowEditing(object sender, GridViewEditEventArgs e)
        //{
        //    this.GridView5.EditIndex = e.NewEditIndex;
        //    if (Session["normal"] != null)
        //    {
        //        DataTable dt = (DataTable)Session["normal"];
        //        GridView5.DataSource = dt;
        //        GridView5.DataBind();
        //    }
        //}
        //protected void grdNormalOrders_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    DataTable sourceData = (DataTable)Session["normal"];
        //    sourceData.Rows[e.RowIndex].Delete();
        //    Session["normal"] = sourceData;
        //    GridView5.EditIndex = -1;
        //    GridView5.DataSource = sourceData;
        //    GridView5.DataBind();

        //}
        //protected void grdNormalOrders_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{
        //    var newvalues = e.NewValues;

        //    DataTable sourceData = (DataTable)Session["normal"];
        //    Atm atm = Atm.LoadAtm("title='" + sourceData.Rows[e.RowIndex][0].ToString() + "'");
        //    if (atm != null)
        //    {
        //        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
        //        if (noteSetType != null)
        //        {
        //            sourceData.Rows[e.RowIndex][2] = noteSetType.DenominationType1.Value * int.Parse(e.NewValues[0].ToString()) +
        //                noteSetType.DenominationType2.Value * int.Parse(e.NewValues[1].ToString()) + noteSetType.DenominationType3.Value * int.Parse(e.NewValues[2].ToString())
        //                + noteSetType.DenominationType4.Value * int.Parse(e.NewValues[3].ToString());

        //            sourceData.Rows[e.RowIndex][3] = e.NewValues[0];
        //            sourceData.Rows[e.RowIndex][4] = e.NewValues[1];
        //            sourceData.Rows[e.RowIndex][5] = e.NewValues[2];
        //            sourceData.Rows[e.RowIndex][6] = e.NewValues[3];



        //            GridView5.EditIndex = -1;
        //            GridView5.DataSource = sourceData;
        //            GridView5.DataBind();
        //            Session["faultyOrders"] = sourceData;
        //        }
        //    }
        //    // Update the grid
        //    //DisplayOrders();
        //}

        //protected void grdEmergencyOrders_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        //{
        //    this.GridView6.EditIndex = -1;
        //    DisplayOrders();
        //}
        //protected void grdEmergencyOrders_RowEditing(object sender, GridViewEditEventArgs e)
        //{
        //    this.GridView6.EditIndex = e.NewEditIndex;
        //    DisplayOrders();
        //}
        //protected void grdEmergencyOrders_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    DataTable sourceData = (DataTable)Session["emergencyOrders"];
        //    sourceData.Rows[e.RowIndex].Delete();
        //    Session["emergencyOrders"] = sourceData;
        //    GridView6.EditIndex = -1;
        //    GridView6.DataSource = sourceData;
        //    GridView6.DataBind();

        //}
        //protected void grdEmergencyOrders_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{
        //    var newvalues = e.NewValues;

        //    DataTable sourceData = (DataTable)Session["emergencyOrders"];
        //    Atm atm = Atm.LoadAtm("title='" + sourceData.Rows[e.RowIndex][0].ToString() + "'");
        //    if (atm != null)
        //    {
        //        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
        //        if (noteSetType != null)
        //        {
        //            sourceData.Rows[e.RowIndex][2] = noteSetType.DenominationType1.Value * int.Parse(e.NewValues[0].ToString()) +
        //                noteSetType.DenominationType2.Value * int.Parse(e.NewValues[1].ToString()) + noteSetType.DenominationType3.Value * int.Parse(e.NewValues[2].ToString())
        //                + noteSetType.DenominationType4.Value * int.Parse(e.NewValues[3].ToString());

        //            sourceData.Rows[e.RowIndex][3] = e.NewValues[0];
        //            sourceData.Rows[e.RowIndex][4] = e.NewValues[1];
        //            sourceData.Rows[e.RowIndex][5] = e.NewValues[2];
        //            sourceData.Rows[e.RowIndex][6] = e.NewValues[3];



        //            GridView6.EditIndex = -1;
        //            GridView6.DataSource = sourceData;
        //            GridView6.DataBind();
        //            Session["emergencyOrders"] = sourceData;
        //        }
        //    }
        //    // Update the grid
        //    //DisplayOrders();
        //}


        //protected void Button_CurrentDayFaultyForcedOrder_Click(object sender, EventArgs e)
        //{

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("title");
        //    dt.Columns.Add("rep_datetime");
        //    dt.Columns.Add("replenishment");
        //    dt.Columns.Add("type1");
        //    dt.Columns.Add("type2");
        //    dt.Columns.Add("type3");
        //    dt.Columns.Add("type4");
        //    dt.Columns.Add("location");
        //    dt.Columns.Add("balance");

        //    LogableTask task = LogableTask.NewTask("Button_CurrentDayFaultyForcedOrder_Click");
        //    int count = 0;
        //    try
        //    {
        //        foreach (GridViewRow row in GridView_FaultyRep_Current.Rows)
        //        {
        //            CheckBox chk = (CheckBox)row.FindControl("chkbx");
        //            if (chk.Checked)
        //            {
        //                string title = this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[1].Text;
        //                Atm atm = Atm.LoadAtm("title='" + title + "'");
        //                NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
        //                if (Session["faultyOrders"] != null)
        //                    dt = (DataTable)Session["faultyOrders"];
        //                DataRow dr = dt.NewRow();
        //                dr[0] = this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[1].Text;
        //                dr[1] = DateTime.Now.DayOfWeek + "," + DateTime.Now.Day + "-" + DateTime.Now.ToString("MMM") + "-" + DateTime.Now.Year;
        //                dr[2] = int.Parse(((TextBox)this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[4].FindControl("TextBox_StandardOrderType1")).Text) * noteSetType.DenominationType1 +
        //                    int.Parse(((TextBox)this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[5].FindControl("TextBox_StandardOrderType2")).Text) * noteSetType.DenominationType2 +
        //                    int.Parse(((TextBox)this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[6].FindControl("TextBox_StandardOrderType3")).Text) * noteSetType.DenominationType3 +
        //                    int.Parse(((TextBox)this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[7].FindControl("TextBox_StandardOrderType4")).Text) * noteSetType.DenominationType4;

        //                dr[3] = ((TextBox)this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[4].FindControl("TextBox_StandardOrderType1")).Text;
        //                dr[4] = ((TextBox)this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[5].FindControl("TextBox_StandardOrderType2")).Text;
        //                dr[5] = ((TextBox)this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[6].FindControl("TextBox_StandardOrderType3")).Text;
        //                dr[6] = ((TextBox)this.GridView_FaultyRep_Current.Rows[row.RowIndex].Cells[7].FindControl("TextBox_StandardOrderType4")).Text;
        //                dr[7] = atm.Location;
        //                CashPosition cashPosition = CashPosition.LoadCashPosition("cash_position_id in (select max(cash_position_id) from cash_position where atm_id=" + atm.ATMId + ")");
        //                if (cashPosition != null)
        //                {
        //                    dr[8] = cashPosition.Cassette1Notes * noteSetType.DenominationType1 +
        //                        cashPosition.Cassette2Notes * noteSetType.DenominationType2 +
        //                        cashPosition.Cassette3Notes * noteSetType.DenominationType3 +
        //                        cashPosition.Cassette4Notes * noteSetType.DenominationType4;
        //                }
        //                dt.Rows.Add(dr);
        //                count++;
        //            }
        //        }

        //        if (count > 0)
        //        {
        //            literal_script.Text = "<li>" + count + " Orders generated successfully!</li>";
        //            Session["faultyOrders"] = dt;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        literal_script.Text = "<li>" + ex.Message + "</li>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //protected void Button_PreviousDayFaultyForcedOrder_Click(object sender, EventArgs e)
        //{

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("title");
        //    dt.Columns.Add("rep_datetime");
        //    dt.Columns.Add("replenishment");
        //    dt.Columns.Add("type1");
        //    dt.Columns.Add("type2");
        //    dt.Columns.Add("type3");
        //    dt.Columns.Add("type4");
        //    dt.Columns.Add("location");
        //    dt.Columns.Add("balance");

        //    LogableTask task = LogableTask.NewTask("Button_PreviousDayFaultyForcedOrder_Click");
        //    int count = 0;
        //    try
        //    {
        //        foreach (GridViewRow row in GridView_FaultyRep.Rows)
        //        {
        //            CheckBox chk = (CheckBox)row.FindControl("chkbx");
        //            if (chk.Checked)
        //            {
        //                string title = this.GridView_FaultyRep.Rows[row.RowIndex].Cells[1].Text;
        //                Atm atm = Atm.LoadAtm("title='" + title + "'");
        //                NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
        //                if (Session["faultyOrders"] != null)
        //                    dt = (DataTable)Session["faultyOrders"];
        //                DataRow dr = dt.NewRow();
        //                dr[0] = this.GridView_FaultyRep.Rows[row.RowIndex].Cells[1].Text;
        //                dr[1] = DateTime.Now.DayOfWeek + "," + DateTime.Now.Day + "-" + DateTime.Now.ToString("MMM") + "-" + DateTime.Now.Year;
        //                dr[2] = int.Parse(((TextBox)this.GridView_FaultyRep.Rows[row.RowIndex].Cells[4].FindControl("TextBox_StandardOrderType1")).Text) * noteSetType.DenominationType1 +
        //                    int.Parse(((TextBox)this.GridView_FaultyRep.Rows[row.RowIndex].Cells[5].FindControl("TextBox_StandardOrderType2")).Text) * noteSetType.DenominationType2 +
        //                    int.Parse(((TextBox)this.GridView_FaultyRep.Rows[row.RowIndex].Cells[6].FindControl("TextBox_StandardOrderType3")).Text) * noteSetType.DenominationType3 +
        //                    int.Parse(((TextBox)this.GridView_FaultyRep.Rows[row.RowIndex].Cells[7].FindControl("TextBox_StandardOrderType4")).Text) * noteSetType.DenominationType4;

        //                dr[3] = ((TextBox)this.GridView_FaultyRep.Rows[row.RowIndex].Cells[4].FindControl("TextBox_StandardOrderType1")).Text;
        //                dr[4] = ((TextBox)this.GridView_FaultyRep.Rows[row.RowIndex].Cells[5].FindControl("TextBox_StandardOrderType2")).Text;
        //                dr[5] = ((TextBox)this.GridView_FaultyRep.Rows[row.RowIndex].Cells[6].FindControl("TextBox_StandardOrderType3")).Text;
        //                dr[6] = ((TextBox)this.GridView_FaultyRep.Rows[row.RowIndex].Cells[7].FindControl("TextBox_StandardOrderType4")).Text;
        //                dr[7] = atm.Location;
        //                CashPosition cashPosition = CashPosition.LoadCashPosition("cash_position_id in (select max(cash_position_id) from cash_position where atm_id=" + atm.ATMId + ")");
        //                if (cashPosition != null)
        //                {
        //                    dr[8] = cashPosition.Cassette1Notes * noteSetType.DenominationType1 +
        //                        cashPosition.Cassette2Notes * noteSetType.DenominationType2 +
        //                        cashPosition.Cassette3Notes * noteSetType.DenominationType3 +
        //                        cashPosition.Cassette4Notes * noteSetType.DenominationType4;
        //                }
        //                dt.Rows.Add(dr);
        //                count++;
        //            }
        //        }

        //        if (count > 0)
        //        {
        //            literal_script.Text = "<li>" + count + " Orders generated successfully!</li>";
        //            Session["faultyOrders"] = dt;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        literal_script.Text = "<li>" + ex.Message + "</li>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

        //protected void Button_GenerateForcedOrder_Click(object sender, EventArgs e)
        //{

        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("title");
        //    dt.Columns.Add("rep_datetime");
        //    dt.Columns.Add("replenishment");
        //    dt.Columns.Add("type1");
        //    dt.Columns.Add("type2");
        //    dt.Columns.Add("type3");
        //    dt.Columns.Add("type4");
        //    dt.Columns.Add("location");
        //    dt.Columns.Add("balance");

        //    LogableTask task = LogableTask.NewTask("Button_GenerateForcedOrder_Click");
        //    int count = 0;
        //    try
        //    {
        //        foreach (GridViewRow row in GridView_SuggestedRep.Rows)
        //        {
        //            CheckBox chk = (CheckBox)row.FindControl("chkbx");
        //            if (chk.Checked)
        //            {
        //                string title = this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[1].Text;
        //                Atm atm = Atm.LoadAtm("title='" + title + "'");
        //                NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
        //                DataRow dr = dt.NewRow();
        //                dr[0] = this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[1].Text;
        //                dr[1] = DateTime.Now.DayOfWeek + "," + DateTime.Now.Day + "-" + DateTime.Now.ToString("MMM") + "-" + DateTime.Now.Year;
        //                dr[2] = int.Parse(((TextBox)this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[6].FindControl("TextBox_StandardOrderType1")).Text) * noteSetType.DenominationType1 +
        //                    int.Parse(((TextBox)this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[7].FindControl("TextBox_StandardOrderType2")).Text) * noteSetType.DenominationType2 +
        //                    int.Parse(((TextBox)this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[8].FindControl("TextBox_StandardOrderType3")).Text) * noteSetType.DenominationType3 +
        //                    int.Parse(((TextBox)this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[9].FindControl("TextBox_StandardOrderType4")).Text) * noteSetType.DenominationType4;

        //                dr[3] = ((TextBox)this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[6].FindControl("TextBox_StandardOrderType1")).Text;
        //                dr[4] = ((TextBox)this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[7].FindControl("TextBox_StandardOrderType2")).Text;
        //                dr[5] = ((TextBox)this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[8].FindControl("TextBox_StandardOrderType3")).Text;
        //                dr[6] = ((TextBox)this.GridView_SuggestedRep.Rows[row.RowIndex].Cells[9].FindControl("TextBox_StandardOrderType4")).Text;
        //                dr[7] = atm.Location;
        //                CashPosition cashPosition = CashPosition.LoadCashPosition("cash_position_id in (select max(cash_position_id) from cash_position where atm_id=" + atm.ATMId + ")");
        //                if (cashPosition != null)
        //                {
        //                    dr[8] = cashPosition.Cassette1Notes * noteSetType.DenominationType1 +
        //                        cashPosition.Cassette2Notes * noteSetType.DenominationType2 +
        //                        cashPosition.Cassette3Notes * noteSetType.DenominationType3 +
        //                        cashPosition.Cassette4Notes * noteSetType.DenominationType4;
        //                }
        //                dt.Rows.Add(dr);
        //                count++;
        //            }
        //        }
        //        if (count > 0)
        //        {
        //            literal_script.Text = "<li>" + count + " Orders generated successfully!</li>";
        //            Session["forcedOrders"] = dt;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //        literal_script.Text = "<li>" + ex.Message + "</li>";
        //    }
        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}


        //protected void Button_GenerateOrderForAll_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = null;
        //    if (Session["forcedOrders"] != null)
        //    {
        //        dt = (DataTable)Session["forcedOrders"];
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            try
        //            {
        //                SaveCashOrders(dr["title"].ToString(), dr["type1"].ToString(), dr["type2"].ToString(), dr["type3"].ToString(), dr["type4"].ToString(),
        //                    dr["replenishment"].ToString(), "RS");

        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //    }
        //    if (Session["faultyOrders"] != null)
        //    {
        //        dt = (DataTable)Session["faultyOrders"];
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            try
        //            {
        //                SaveCashOrders(dr["title"].ToString(), dr["type1"].ToString(), dr["type2"].ToString(), dr["type3"].ToString(), dr["type4"].ToString(),
        //                     dr["replenishment"].ToString(), "RS");
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //    }
        //    if (Session["normal"] != null)
        //    {
        //        dt = (DataTable)Session["normal"];
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            try
        //            {
        //                SaveCashOrders(dr["title"].ToString(), dr["type1"].ToString(), dr["type2"].ToString(), dr["type3"].ToString(), dr["type4"].ToString(),
        //                     dr["replenishment"].ToString(), "RS");
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //    }

        //    if (Session["emergencyOrders"] != null)
        //    {
        //        dt = (DataTable)Session["emergencyOrders"];
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            try
        //            {
        //                SaveCashOrders(dr["title"].ToString(), dr["type1"].ToString(), dr["type2"].ToString(), dr["type3"].ToString(), dr["type4"].ToString(),
        //                     dr["replenishment"].ToString(), "UR");
        //            }
        //            catch (Exception ex)
        //            {
        //            }
        //        }
        //    }
        //    Session[SessionVars.notice.ToString()] = "Cash orders created successfully.";
        //    Session["forcedOrders"] = null;
        //    Session["faultyOrders"] = null;
        //    Session["normal"] = null;
        //    Session["emergencyOrders"] = null;
        //    Response.Redirect("~/ccms/cashorders.aspx");

        //}
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
            try
            {
                DrawDFFInfoGraph(ref cmd);
            }
            finally
            {
                if (cmd != null)
                    cmd.Connection.Close();
            }
            //button_search_Click(this, null);

        }
        protected void Dropdownlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
            try
            {
                GenerateRemainingNotesGraph(ref cmd);
            }
            finally
            {
                if (cmd != null)
                    cmd.Connection.Close();
            }
        }
        protected void Button_UpdateATM_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("EnableDFFGeneration");
            StringBuilder builder = new StringBuilder();

            try
            {
                foreach (GridViewRow row in GridView_ATM.Rows)
                {
                    string atm_id = this.GridView_ATM.DataKeys[row.DataItemIndex].Values["atm_id"].ToString();
                    CheckBox chk = (CheckBox)row.FindControl("chkbx");
                    if (chk.Checked) // means now it is set to 'scheduled'
                    {
                        builder.Append(atm_id + ",");
                    }
                }
                if (builder.Length > 0)
                {
                    builder.Append("-1");
                    ConnectionFactory.ExecuteQuery("update atm set is_dff_generation_halt=0 where atm_id in (" + builder.ToString() + ")");
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);


                    DisplayHaltedATMs(ref cmd);
                    literal_script.Text = "<script>parent.window.ShowSuccess('ATMs updated successfully!');</script>";

                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void SaveCashOrders(string title, string type1, string type2, string type3, string type4, string amount, string cashOrderType)
        {
            LogableTask task = LogableTask.NewTask("SaveCashOrders");
            SqlTransaction trxn = null;
            SqlConnection conn = null;
            Avanza.CCMS.DAL.CashOrders cashOrders;

            try
            {
                Atm atm = Atm.LoadAtm("title='" + title + "'");
                NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                if (noteSetType == null)
                    throw new Exception("Note Set Type does not exists");



                cashOrders = new Avanza.CCMS.DAL.CashOrders();
                cashOrders.CashOrderDatetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                cashOrders.AtmId = atm.ATMId;
                cashOrders.IsUploaded = false;
                cashOrders.IsCancelled = false;

                AppSetting appSettings = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]);
                cashOrders.DispatchTime = new DateTime(DateTime.Now.Year,
                                          DateTime.Now.Month, DateTime.Now.Day, appSettings.CashOrderExecutionTime.Hour,
                                          appSettings.CashOrderExecutionTime.Minute, appSettings.CashOrderExecutionTime.Second);



                cashOrders.Cassette1Denomination = noteSetType.DenominationType1.Value;
                cashOrders.Cassette2Denomination = noteSetType.DenominationType2.Value;
                cashOrders.Cassette3Denomination = noteSetType.DenominationType3.Value;
                cashOrders.Cassette4Denomination = noteSetType.DenominationType4.Value;
                cashOrders.Cassette5Denomination = noteSetType.DenominationType5.Value;
                cashOrders.Cassette6Denomination = noteSetType.DenominationType6.Value;
                cashOrders.Cassette7Denomination = noteSetType.DenominationType7.Value;


                if (type1.Length > 0)
                    cashOrders.Cassette1SuggestedNotes = int.Parse(type1);
                else
                    cashOrders.Cassette1SuggestedNotes = 0;


                if (type2.Length > 0)
                    cashOrders.Cassette2SuggestedNotes = int.Parse(type2);
                else
                    cashOrders.Cassette2SuggestedNotes = 0;


                if (type3.Length > 0)
                    cashOrders.Cassette3SuggestedNotes = int.Parse(type3);
                else
                    cashOrders.Cassette3SuggestedNotes = 0;

                if (type4.Length > 0)
                    cashOrders.Cassette4SuggestedNotes = int.Parse(type4);
                else
                    cashOrders.Cassette4SuggestedNotes = 0;


                cashOrders.Cassette5SuggestedNotes = 0;
                cashOrders.Cassette6SuggestedNotes = 0;
                cashOrders.Cassette7SuggestedNotes = 0;

                cashOrders.CashOrderType = cashOrderType;// DropDownList_RepType.SelectedIndex == 0 ? "RS" : "UR";
                cashOrders.OrderNumber = cashOrders.CashOrderType + "-" + atm.Title + "-" + cashOrders.CashOrderDatetime.ToString("ddMMyy");
                cashOrders.Cassette1RemainingNotes = cashOrders.Cassette1SuggestedNotes;
                cashOrders.Cassette2RemainingNotes = cashOrders.Cassette2SuggestedNotes;
                cashOrders.Cassette3RemainingNotes = cashOrders.Cassette3SuggestedNotes;
                cashOrders.Cassette4RemainingNotes = cashOrders.Cassette4SuggestedNotes;
                cashOrders.Cassette5RemainingNotes = cashOrders.Cassette5SuggestedNotes;
                cashOrders.Cassette6RemainingNotes = cashOrders.Cassette6SuggestedNotes;
                cashOrders.Cassette7RemainingNotes = cashOrders.Cassette7SuggestedNotes;
                cashOrders.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trxn = conn.BeginTransaction();

                Avanza.CCMS.DAL.CashOrders existingCashOrder = Avanza.CCMS.DAL.CashOrders.LoadCashOrders(string.Format("atm_id = {0} and cash_order_datetime=convert(datetime,'{1}',103)", atm.ATMId, DateTime.Now.ToString("dd/MM/yyyy")));
                if (existingCashOrder != null)
                {
                    existingCashOrder.IsCancelled = true;
                    Task existingTask = Task.LoadTask("cash_order_id = " + existingCashOrder.CashOrderId);
                    if (existingTask != null)
                    {
                        existingTask.Status = UploadStates.cancelledAnotherOrderPosted.ToString();
                        existingTask.Save(trxn.Connection, trxn);
                    }
                }


                cashOrders.Save(trxn.Connection, trxn);
                Task cashOrderUploadTask = new Task();
                cashOrderUploadTask.ATMId = atm.ATMId;
                cashOrderUploadTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                cashOrderUploadTask.CreationTime = DateTime.Now;
                cashOrderUploadTask.UploadTime = new DateTime(1900, 1, 1);
                cashOrderUploadTask.Parsed = false;
                cashOrderUploadTask.Status = DownloadStates.scheduled.ToString();
                cashOrderUploadTask.CashOrderId = cashOrders.CashOrderId;
                cashOrderUploadTask.BytesTransferred = 0;
                cashOrderUploadTask.TaskTypeId = (int)EnumTaskType.CashOrderUpload;
                cashOrderUploadTask.RetryRemaining = (byte)(atm.RetryCountCashOrderUpload);
                cashOrderUploadTask.Save(trxn.Connection, trxn);




                Avanza.CCMS.DAL.CashOrderMonitoring cashOrderMonitoring = new Avanza.CCMS.DAL.CashOrderMonitoring();
                cashOrderMonitoring.CashOrderDatetime = cashOrders.CashOrderDatetime;
                cashOrderMonitoring.AtmId = atm.ATMId;
                cashOrderMonitoring.CurrentOrderReceivedAt = DateTime.Now;
                cashOrderMonitoring.CurrentOrderId = cashOrderUploadTask.CashOrderId.Value;


                cashOrderMonitoring.CurrentOrderRemainingAmount = noteSetType.DenominationType1.Value * cashOrders.Cassette1SuggestedNotes.Value
                                + noteSetType.DenominationType2.Value * cashOrders.Cassette2SuggestedNotes.Value
                              + noteSetType.DenominationType3.Value * cashOrders.Cassette3SuggestedNotes.Value
                                + noteSetType.DenominationType4.Value * cashOrders.Cassette4SuggestedNotes.Value
                                 + noteSetType.DenominationType5.Value * cashOrders.Cassette5SuggestedNotes.Value
                                  + noteSetType.DenominationType6.Value * cashOrders.Cassette6SuggestedNotes.Value
                                    + noteSetType.DenominationType7.Value * cashOrders.Cassette7SuggestedNotes.Value;

                cashOrderMonitoring.CurrentOrderSuggestedAmount = cashOrderMonitoring.CurrentOrderRemainingAmount;
                cashOrderMonitoring.Save(trxn.Connection, trxn);
                //SaveCashOrdersInEmitacDatabase(cashOrders, cashOrderMonitoring, atm, task, conn, trxn);




                VaultAtm vaultAtm = VaultAtm.LoadVaultAtm("atm_id = " + cashOrders.AtmId);
                if (vaultAtm != null)
                {
                    VaultInfo vaultInfo = null;
                    if (existingCashOrder != null)
                    {
                        CcmsTempVaultLedger tempVaultLedger = CcmsTempVaultLedger.LoadCcmsTempVaultLedger("order_id = " + existingCashOrder.CashOrderId);
                        if (tempVaultLedger != null)
                        {
                            vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, tempVaultLedger.LedgerAmount, existingCashOrder.AtmId, "Cr", "Vault adjusted because order cancelled.Order ref # " + existingCashOrder.OrderNumber, "Adjustment Credit", -1, trxn);
                            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                            cmd.CommandText = "select quantity,vault_note_type_id from ccms_temp_vault_ledger_detail where ledger_id = " + tempVaultLedger.Id;
                            System.Data.DataTable dt = new System.Data.DataTable();
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            adapter.Fill(dt);

                            foreach (DataRow dr in dt.Rows)
                            {
                                int Quantity = int.Parse(dr["quantity"].ToString());
                                CreateTempVaultLedgerDetail(vaultInfo, Quantity, int.Parse(dr["vault_note_type_id"].ToString()), "Cr", true, trxn);
                            }
                            ConnectionFactory.ExecuteQuery("delete general_alert where entity_id = " + tempVaultLedger.Id, trxn);
                        }
                    }

                    vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, cashOrderMonitoring.CurrentOrderRemainingAmount, cashOrders.AtmId, "Dr", "Vault deducted with amount " + cashOrderMonitoring.CurrentOrderRemainingAmount + " order ref # " + cashOrders.OrderNumber, "Outbound", cashOrders.CashOrderId, trxn);

                    if (cashOrders.Cassette1SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType1Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette1SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette2SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType2Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette2SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette3SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType3Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette3SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette4SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType4Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette4SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette5SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType5Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette5SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }

                    if (cashOrders.Cassette6SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType6Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette6SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette7SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType7Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette7SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }





                }
                else
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Vault is not defined for the atm =" + cashOrders.AtmId);

                trxn.Commit();


                Utility.BuildAuditLog(string.Format("Cash order created for atm {0} for the day {1}", atm.Title, cashOrders.CashOrderDatetime.ToString("dd/MM/yyyy")), (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateCashOrders);




            }
            catch (Exception ex)
            {
                trxn.Rollback();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        public class VaultInfo
        {
            public int newVaultId = 0;
            public int existingVaultId = 0;
        }
        private VaultInfo CreateTempVaultLedger(int vaultId, decimal amount, int atmId, string transactionType, string desc, string vaultTrxnType, int orderId, SqlTransaction trxn)
        {
            VaultInfo vaultInfo = new VaultInfo();
            CcmsTempVaultLedger vaultLedger = new CcmsTempVaultLedger();
            vaultLedger.TransactionDate = DateTime.Now;
            vaultLedger.TransactionType = transactionType;
            vaultLedger.Description = desc;
            vaultLedger.LedgerAmount = amount;
            vaultLedger.PostedBy = 1;
            vaultLedger.VaultId = vaultId;
            vaultLedger.VaultTransactionType = vaultTrxnType;
            vaultLedger.AtmId = atmId;
            vaultLedger.OrderId = orderId;
            object result = ConnectionFactory.ExecuteScalar("select max(id) from ccms_temp_vault_ledger where vault_id = " + vaultId);
            if (result != DBNull.Value)
            {
                CcmsTempVaultLedger existingLedger = CcmsTempVaultLedger.LoadCcmsTempVaultLedgerByPk(int.Parse(result.ToString()));
                if (existingLedger != null)
                {
                    vaultInfo.existingVaultId = existingLedger.Id;
                    if (transactionType == "Cr")
                        vaultLedger.Balance = existingLedger.Balance + vaultLedger.LedgerAmount;
                    else
                        vaultLedger.Balance = existingLedger.Balance - vaultLedger.LedgerAmount;

                }

            }
            else
            {
                if (transactionType == "Cr")
                    vaultLedger.Balance = vaultLedger.LedgerAmount;
                else
                    vaultLedger.Balance = -vaultLedger.LedgerAmount;
            }

            vaultLedger.Save(trxn.Connection, trxn);
            vaultInfo.newVaultId = vaultLedger.Id;
            return vaultInfo;
        }

        private void CreateTempVaultLedgerDetail(VaultInfo vaultInfo, int qty, int vaultNoteTypeId, string trxnType, bool isContraEntry, SqlTransaction trxn)
        {
            CcmsTempVaultLedgerDetail vaultLedgerDetail = new CcmsTempVaultLedgerDetail();
            vaultLedgerDetail.LedgerId = vaultInfo.newVaultId;
            vaultLedgerDetail.Quantity = qty;
            vaultLedgerDetail.VaultNoteTypeId = vaultNoteTypeId;
            CcmsTempVaultLedgerDetail existingVaultLedgerDetail = CcmsTempVaultLedgerDetail.LoadCcmsTempVaultLedgerDetail(
                string.Format("ledger_id = {0} and vault_note_type_id={1}", vaultInfo.existingVaultId, vaultNoteTypeId));
            if (existingVaultLedgerDetail != null)
            {
                if (vaultLedgerDetail.Balance == null)
                    vaultLedgerDetail.Balance = 0;

                if (existingVaultLedgerDetail.Balance == null)
                    existingVaultLedgerDetail.Balance = 0;



                if (trxnType == "Cr")
                    vaultLedgerDetail.Balance = existingVaultLedgerDetail.Balance + qty;
                else if (trxnType == "Dr")
                    vaultLedgerDetail.Balance = existingVaultLedgerDetail.Balance - qty;
            }
            else
            {
                if (trxnType == "Cr")
                    vaultLedgerDetail.Balance = qty;
                else
                    vaultLedgerDetail.Balance = -qty;
            }
            if (!isContraEntry)
            {
                CcmsVaultNoteType ccmsVaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteTypeByPk(vaultNoteTypeId);
                if (vaultLedgerDetail.Balance <= ccmsVaultNoteType.ThrasholdQty)
                {
                    CcmsVault ccmsVault = CcmsVault.LoadCcmsVaultByPk(ccmsVaultNoteType.VaultId);
                    GenerateGeneralAlert((int)EnumAlertType.VaultLowBalanceThresholdReached, "Vault[" + ccmsVault.Title + "] Threshold reached for denomination " + ccmsVaultNoteType.DenominationName, vaultLedgerDetail.LedgerId, EntityType.VaultLedger, trxn, Event_Type.Warning);
                }
            }

            vaultLedgerDetail.Save(trxn.Connection, trxn);
        }
        public void GenerateGeneralAlert(int alertTypeID, string msg, int entityId, EntityType entityType, SqlTransaction trxn, Event_Type eventType)
        {
            LogableTask task = LogableTask.NewTask("GenerateGeneralAlert");
            try
            {

                GeneralAlert alert = new GeneralAlert();
                alert.GeneratedAt = DateTime.Now;
                alert.AlertTypeId = alertTypeID;
                alert.ExpirationTime = DateTime.Now.AddDays(((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).AlertExpirationTime.Value);

                //                alert.ExpirationTime = DateTime.Now.AddDays(int.Parse(ConfigurationManager.AppSettings["AlertExpirationTime"]));
                alert.AlertMsg = msg;
                alert.RetryRemaining = 10;
                alert.GenerateNotificationSent = false;
                alert.EntityId = entityId;
                alert.EntityType = entityType.ToString();
                alert.Save(trxn.Connection, trxn);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Alert added of type " + alertTypeID);
                GenerateIntegratedAlert(alertTypeID, msg, entityId, entityType,
                    eventType, null, trxn);
            }

            finally
            {
                task.EndTask();
            }
        }
        public void GenerateIntegratedAlert(int alertTypeID, string msg,
           int entityID, EntityType entityType, Event_Type eventType, int? ftpFileInfoId, SqlTransaction trxn)
        {
            LogableTask task = LogableTask.NewTask("GenerateIntegratedAlert");
            int orgID = -1;
            try
            {
                CcmsIntegratedAlert alert = new CcmsIntegratedAlert();
                alert.AlertTypeId = alertTypeID;
                alert.AlertType = AlertType.LoadAlertTypeByPk(alertTypeID).AlertTypeName;
                alert.EntityId = entityID;
                alert.EntityType = entityType.ToString();
                alert.AlertLevel = eventType.ToString();
                alert.AlertStatus = "Unread";
                alert.GeneratedAt = DateTime.Now;
                alert.AlertText = msg;
                alert.ExpirationTime = DateTime.Now.AddDays(((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).AlertExpirationTime.Value);
                alert.GenerateNotificationSent = false;
                alert.ResolveNotificationSent = false;
                if (alert.EntityType == EntityType.ATM.ToString())
                {
                    Atm atm = Atm.LoadAtmByPk(entityID);
                    alert.GenerateRetryRemaining = atm.RetryCountAlert;
                    alert.ResolveRetryRemaining = atm.RetryCountAlert;
                    orgID = GetOrganization(atm.RegionId);
                }
                else if (alert.EntityType == EntityType.Organization.ToString())
                {
                    alert.FtpFileInfoId = ftpFileInfoId;
                    FtpFileInfo ftpFileInfo = FtpFileInfo.LoadFtpFileInfoByPk(ftpFileInfoId.Value);
                    Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(ftpFileInfo.RegionId);
                    alert.GenerateRetryRemaining = region.RetryCountAlert; // add field in region table for this....
                    alert.ResolveRetryRemaining = region.RetryCountAlert;
                    orgID = FtpFileInfo.LoadFtpFileInfoByPk(ftpFileInfoId.Value).RegionId;
                }
                alert.ModuleType = "CURRENCY";
                alert.OrganizationId = orgID;

                if (trxn != null)
                    alert.Save(trxn.Connection, trxn);
                else
                    alert.Save();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Alert added of type " + alertTypeID);
            }

            finally
            {
                task.EndTask();
            }
        }
        public int GetOrganization(int region_id)
        {
            Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(region_id);
            if (region.IsOrganization)
            {
                return region.RegionId;
            }
            else
                return GetOrganization(region.ParentRegionId.Value);
        }

    }


}
