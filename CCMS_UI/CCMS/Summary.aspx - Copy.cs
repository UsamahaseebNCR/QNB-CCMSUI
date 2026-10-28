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
using Avanza.iSuite.DAL;
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



namespace CCMSUI.CCMS
{
    public partial class Summary : BasePage
    {
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

        private void BindLowBalanceGrid(string order)
        {
            LogableTask task = LogableTask.NewTask("GetLowBalanceATMs");
            try
            {
                SqlCommand cmd = null;
                DataTable dt = new DataTable();
                cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = @"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                                  select title, min_operating_balance, generated_at,alert_msg,resolve_at,
                                note_set_type.denomination_type_1 * cassette1_notes +
note_set_type.denomination_type_2 * cassette2_notes +
note_set_type.denomination_type_3 * cassette3_notes +
note_set_type.denomination_type_4 * cassette4_notes cbalance

                                   from atm_alert,atm outerATM,cash_position,note_set_type
                                   where atm_alert.atm_id = outerATM.atm_id
                                    and cash_position.atm_id = outerATM.atm_id
                                    and note_set_type.note_set_type_id = outerATM.note_set_type_id
                                    and alert_type_id =20 and outerATM.is_active =1 
                                    --and generated_at = ( select max(generated_at) from atm_alert  where alert_type_id =20 and atm_id = outerATM.atm_id )
                                    and last_trxn_at = (select max(last_trxn_at) from cash_position where atm_id = outerATM.atm_id)";
                new SqlDataAdapter(cmd).Fill(dt);
                DataRow[] currentDayLowBalancedRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
                DataRow[] previousDayLowBalancedRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                DataTable dtLowBalanceCurrentDay = this.CreateDatatableFromDataRows(currentDayLowBalancedRows);
                dtLowBalanceCurrentDay.DefaultView.Sort = order;
                this.GridView_LowBalance_CurrentDay.DataSource = dtLowBalanceCurrentDay;
                this.GridView_LowBalance_CurrentDay.DataBind();
                DataTable dtLowBalancePreviousDay = this.CreateDatatableFromDataRows(previousDayLowBalancedRows);
                dtLowBalancePreviousDay.DefaultView.Sort = order;
                this.GridView_LowBalance_PreviousDay.DataSource = dtLowBalancePreviousDay;
                this.GridView_LowBalance_PreviousDay.DataBind();
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
            SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
            cmd.CommandTimeout = 600;
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
                    builder.Append("<tr><td><b>Title</b></td></tr>");
                    string[] parts = e.Parameters[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in parts)
                    {
                        builder.Append("<tr><td>" + str + "</td></tr>");
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

        private void DisplayCallOutSummary()
        {
            LogableTask task = LogableTask.NewTask("DisplayCallOutSummary");
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("activity");
            dt.Columns.Add("count");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 600;

                cmd.CommandText = @"select COUNT(flm_monitoring_id) from flm_monitoring
                              where activity_datetime >=convert(datetime,'" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy") + "',103) and activity_datetime<=convert(datetime,'"
               + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103); select COUNT(flm_monitoring_id) from flm_monitoring  where time_of_call_to_bank is null  and activity_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and activity_datetime<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103);  select COUNT(flm_monitoring_id) from flm_monitoring  where time_of_solving is null and activity_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and activity_datetime<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103);  select max(activity_datetime) from flm_monitoring";
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    this.Label_CurrentMonthCalls.Text = reader[0].ToString();
                }
                reader.NextResult();
                while (reader.Read())
                {
                    this.Label_NotReportedToBank.Text = reader[0].ToString();
                }
                reader.NextResult();
                while (reader.Read())
                {
                    this.Label_PendingCalls.Text = reader[0].ToString();
                }
                reader.NextResult();
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value)
                        this.Label_LastActivityAt.Text = DateTime.Parse(reader[0].ToString()).ToString("dd/MM/yyyy HH:mm:ss");
                    else
                        this.Label_LastActivityAt.Text = "No records imported";
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
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

        private void DisplayCurrentOrder()
        {
            LogableTask task = LogableTask.NewTask("DisplayCurrentOrder");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetCurrentOrderList";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter("UserId", SqlDbType.Int));
                cmd.Parameters[2].Value = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
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

        private void DisplayHaltedATMs()
        {
            LogableTask task = LogableTask.NewTask("DisplayHaltedATMs");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "select title from atm where is_dff_generation_halt =1 and is_active = 1 and atm_id in (select atm_id from user_atms where user_id=" + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId + ")";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
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

        private void DisplaySummaryOfTodaysAlert()
        {
            LogableTask task = LogableTask.NewTask("DisplaySummaryOfTodaysAlert");
            List<string> alertTypes = new List<string>();
            string type = null;
            
            int resolvedCount=0;
            int unResolvedCount=0;
            try
            {
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("task");
                dt1.Columns.Add("resolved");
                dt1.Columns.Add("unresolved");
                dt1.Columns.Add("total");
                dt1.Columns.Add("alert_type_id");
                dt1.Columns.Add("entity_type");
 
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetSummary";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                
                foreach (DataRow dr  in dt.Rows)
                {
                    type=dr["type"].ToString();
                    if (!alertTypes.Contains(type))
                        alertTypes.Add(type);
                }

                for (int j = 0; j < alertTypes.Count; j++)
                {
                     resolvedCount=0;
                    unResolvedCount=0;

                    DataRow[] drArray =  dt.Select("type='"+alertTypes[j]+"'");
                    for (int k = 0; k < drArray.Length; k++)
                    {
                        if (drArray[k][4].ToString() == "1")
                            resolvedCount = int.Parse(drArray[k][1].ToString());
                        else if (drArray[k][4].ToString() == "0")
                            unResolvedCount = int.Parse(drArray[k][1].ToString());
                    }
                    
                    DataRow dr =  dt1.NewRow();
                    dr["task"] = drArray[0][0].ToString();


                    dr["resolved"] = resolvedCount;
                    dr["unresolved"] = unResolvedCount;
                    dr["total"] = resolvedCount + unResolvedCount;
                    
                    dr["alert_type_id"] = drArray[0][2].ToString();
                    dr["entity_type"] = drArray[0][3].ToString();
                    dt1.Rows.Add(dr);
 
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

        private void DisplayUserTask()
        {
            LogableTask task = LogableTask.NewTask("DisplayUserTask");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetUserTask";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter("UserId", SqlDbType.Int));
                cmd.Parameters[2].Value = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
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

        private void DrawGraph()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("DrawGraph");
            try
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Draw Pie");
                this.DrawPie();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Pie created");
                decimal totalRemaining1 = 0M;
                decimal totalRemaining2 = 0M;
                decimal totalRemaining3 = 0M;
                decimal totalRemaining4 = 0M;
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetRemainingNotes";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("ForDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Remaining Notes Procedure");
                SqlDataReader reader = cmd.ExecuteReader();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Procedure executed");
                while (reader.Read())
                {
                    totalRemaining1 += decimal.Parse(reader[0].ToString()) - decimal.Parse(reader[4].ToString());
                    totalRemaining2 += decimal.Parse(reader[1].ToString()) - decimal.Parse(reader[5].ToString());
                    totalRemaining3 += decimal.Parse(reader[2].ToString()) - decimal.Parse(reader[6].ToString());
                    totalRemaining4 += decimal.Parse(reader[3].ToString()) - decimal.Parse(reader[7].ToString());
                }
                reader.Close();
                Label_QueryAt.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettesDenomination"].Split(new char[] { ',' });
                this.Chart3.Series["Default"].Points.AddXY(parts[0], new object[] { totalRemaining1 });
                this.Chart3.Series["Default"].Points.AddXY(parts[1], new object[] { totalRemaining2 });
                this.Chart3.Series["Default"].Points.AddXY(parts[2], new object[] { totalRemaining3 });
                this.Chart3.Series["Default"].Points.AddXY(parts[3], new object[] { totalRemaining4 });
                this.Label_Denomination1.Text = parts[0];
                this.Label_Quantity1.Text = totalRemaining1.ToString();
                this.Label_TotalDenomination1.Text = (decimal.Parse(parts[0].Substring(3)) * totalRemaining1).ToString();
                this.Label_Denomination2.Text = parts[1];
                this.Label_Quantity2.Text = totalRemaining2.ToString();
                this.Label_TotalDenomination2.Text = (decimal.Parse(parts[1].Substring(3)) * totalRemaining2).ToString();
                this.Label_Denomination3.Text = parts[2];
                this.Label_Quantity3.Text = totalRemaining3.ToString();
                this.Label_TotalDenomination3.Text = (decimal.Parse(parts[2].Substring(3)) * totalRemaining3).ToString();
                this.Label_Denomination4.Text = parts[3];
                this.Label_Quantity4.Text = totalRemaining4.ToString();
                this.Label_TotalDenomination4.Text = (decimal.Parse(parts[3].Substring(3)) * totalRemaining4).ToString();
                this.Label_TotalRemAmount.Text = (((decimal.Parse(this.Label_TotalDenomination1.Text) + decimal.Parse(this.Label_TotalDenomination2.Text)) + decimal.Parse(this.Label_TotalDenomination3.Text)) + decimal.Parse(this.Label_TotalDenomination4.Text)).ToString();
                this.Chart3.Series["Default"]["ShowMarkerLines"] = "true";
                this.Chart3.Series["Default"]["DrawingStyle"] = "Cylinder";
                this.Chart3.Series["Default"]["PointWidth"] = "0.8";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Trxn Count Procedure For Current Day");
                cmd.CommandText = "GetTrxnCount";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.Now;
                this.Label_CurrentDayTrxnCount.Text = cmd.ExecuteScalar().ToString();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Trxn Count Procedure For Current Day executed successfully.");
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today.AddDays(-1.0);
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.Now.AddDays(-1.0);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Trxn Count Procedure For yesterday");
                this.Label_PreviousDayTrxnCount.Text = cmd.ExecuteScalar().ToString();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Trxn Count Procedure For yesterday");
                cmd.CommandText = "GetFlmSummaryByCode";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.Today;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute GetFlmSummaryByCode");
                reader = cmd.ExecuteReader();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "GetFlmSummaryByCode executed successfully");
                while (reader.Read())
                {
                    this.Chart2.Series["Default"].Points.AddXY(reader[0], new object[] { reader[1] });
                }
                reader.Close();
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

        private void DrawNotesAcceptorGraph()
        {
            StringBuilder builderTrxn = new StringBuilder();
            StringBuilder builderNoTrxn = new StringBuilder();
            int totalTransactingATM = 0;
            int totalNotesDepositMachines = 0;
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select distinct(title)
                                from parsed_bna_counter inner join atm 
                                on parsed_bna_counter.atm_id = atm.atm_id 
                                inner join user_ATMs
                                on user_ATMs.atm_id = parsed_bna_counter.atm_id
                                where last_deposit_at>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) " +
                                " and last_deposit_at<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)  " +
                               "  and atm.is_active=1 and user_ATMs.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;

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

            cmd.CommandText = "select title from atm where is_active=1 and is_cdm=1";
            DataTable dt1 = new DataTable();
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
        private void DrawChequeDepositGraph()
        {
            StringBuilder builderTrxn = new StringBuilder();
            StringBuilder builderNoTrxn = new StringBuilder();
            int totalTransactingATM = 0;
            int totalChequeDepositMachines = 0;
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select distinct(title) 
                                from parsed_cpm_counter inner join atm 
                                on parsed_cpm_counter.atm_id = atm.atm_id 
                                inner join user_ATMs
                                on user_ATMs.atm_id = parsed_cpm_counter.atm_id
                                where deposit_at>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) " +
                                " and deposit_at<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)  " +
                               "  and atm.is_active=1 and user_ATMs.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);



            cmd.CommandText = "select title from atm where is_active=1 and is_ccdm=1";
            DataTable dt1 = new DataTable();
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
            dr[1] = (int)ConnectionFactory.ExecuteScalar("select count(cash_order_monitoring_id) from cash_order_monitoring where cash_order_datetime=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and current_order_executed_at is null");
            dtTasks.Rows.Add(dr);

            dr = dtTasks.NewRow();
            dr[0] = "Missing Settlements";
            dr[1] = (int)ConnectionFactory.ExecuteScalar("select count(atm_settlement_id) from atm_settlement where rep_datetime=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) ");
            dtTasks.Rows.Add(dr);

            GridView_CITTasks.DataSource = dtTasks;
            GridView_CITTasks.DataBind();


        }

        private void DrawPie()
        {
            //DataRow drInner1;
            //bool baseFound = false;
            int inactivityCounter = 0;
            int faultyCounter = 0;
            //int purgeCounter = 0;
            //int purgeTrxnCount = 0;
            int totalATMs = 0;
            int transactingCount = 0;
            int dispensingFailed = 0;
            //SqlDataAdapter adapter = null;
            SqlCommand cmd = null;
            //DateTime fromHour;
            //DateTime toHour;

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
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 600;
                StringBuilder builder = new StringBuilder();
                cmd.CommandText = "select distinct(title) from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.ATMCassetteFaulty + " and resolve_at is null";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!dtFaulty.Rows.Contains(reader[0]))
                    {
                        builder.Append(reader[0] + ",");
                        //DataRow dr = dtATMs.NewRow();
                        DataRow dr = dtFaulty.NewRow();
                        dr[0] = reader[0];
                        dtFaulty.Rows.Add(dr);
                        faultyCounter++;
                    }
                }
                reader.Close();
                hidden_faulty.Value = builder.ToString();

                builder = new StringBuilder();
                cmd.CommandText = "select title from atm_alert,atm where atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.ATMInactivityPeriodElapsed + " and resolve_at is null";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!dtInactivity.Rows.Contains(reader[0]))
                    {
                        builder.Append(reader[0] + ",");
                        //DataRow dr = dtATMs.NewRow();
                        DataRow dr = dtInactivity.NewRow();
                        dr[0] = reader[0];
                        dtInactivity.Rows.Add(dr);
                        inactivityCounter++;
                    }
                }
                reader.Close();
                hidden_noActivity.Value = builder.ToString();

                builder = new StringBuilder();
                cmd.CommandText = "select title from atm_alert,atm where atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.CassetteDispensingFailed + " and resolve_at is null";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!dtDispensing.Rows.Contains(reader[0]))
                    {
                        builder.Append(reader[0] + ",");
                        DataRow dr = dtDispensing.NewRow();
                        dr[0] = reader[0];
                        dtDispensing.Rows.Add(dr);
                        dispensingFailed++;
                    }
                }
                reader.Close();
                hidden_dispensedFailed.Value = builder.ToString();


                //cmd.CommandText = "GetTransactions";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                //fromHour = DateTime.Now.AddHours(-1.0);
                //cmd.Parameters[0].Value = fromHour;
                //cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                //toHour = DateTime.Now;
                //cmd.Parameters[1].Value = toHour;
                //cmd.Parameters.Add(new SqlParameter("userID", SqlDbType.Int));
                //cmd.Parameters[2].Value = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                //Chart1.Titles[0].Text = " ATM Status (Last 1 Hour, "+
                // fromHour.Hour.ToString().PadLeft(2, '0') + ":" + fromHour.Minute.ToString().PadLeft(2, '0') + "-" +
                //    toHour.Hour.ToString().PadLeft(2, '0') + ":" + toHour.Minute.ToString().PadLeft(2, '0')+")";

                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //DataTable dt = new DataTable();
                //adapter.Fill(dt);
                cmd.CommandText = "select count(title) from atm where is_active=1 and is_atm=1";
                totalATMs = (int)cmd.ExecuteScalar();

                cmd.CommandText = "select distinct(title) from parsed_transaction,atm where parsed_transaction.atm_id = atm.atm_id and trxn_datetime>=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + "',103) and trxn_datetime<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59',103)";
                builder = new StringBuilder();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    //DataRow[] drArray = dtATMs.Select("title='" + reader[0] + "'");
                    //if (drArray.Length == 0)
                    //{
                    //    builder.Append(reader[0] + ",");
                    //    transactingCount++;
                    //}

                    if (!dtDispensing.Rows.Contains(reader[0]) && !dtInactivity.Rows.Contains(reader[0]) && !dtFaulty.Rows.Contains(reader[0]))
                    {
                        builder.Append(reader[0] + ",");
                        transactingCount++;
                    }
                }
                reader.Close();
                hidden_transacting.Value = builder.ToString();
                //cmd.Parameters.Clear();
                //cmd.CommandType = CommandType.Text;
                //adapter = new SqlDataAdapter(cmd);
                //DataTable dtATM = new DataTable();
                //adapter.Fill(dtATM);
                //int totalActiveATMs = dtATM.DefaultView.ToTable(true, new string[] { "title" }).Rows.Count;
                //this.Label_TotalATMs.Text = totalActiveATMs.ToString();
                this.Label_TotalATMs.Text = totalATMs.ToString();


                //DataTable dtATMID = dt.DefaultView.ToTable(true, new string[] { "atm_id", "title" });
                //if (dtATMID.Rows.Count == 0)
                //{
                //    inactivityCounter = totalActiveATMs;
                //    dtInactivity = dtATM;
                //}
                //foreach (DataRow dr in dtATMID.Rows)
                //{
                //    DataRow drInner;
                //    baseFound = false;
                //    DataRow[] drArray = dt.Select("atm_id=" + dr[0]);
                //    for (int k = 0; k < drArray.Length; k++)
                //    {
                //        if (!baseFound)
                //        {
                //            if (decimal.Parse(drArray[k]["amount"].ToString()) == 0M)
                //            {
                //                purgeTrxnCount++;
                //                baseFound = true;
                //            }
                //        }
                //        else if (decimal.Parse(drArray[k]["amount"].ToString()) == 0M)
                //        {
                //            purgeTrxnCount++;
                //            if (purgeTrxnCount == 3)
                //            {
                //                break;
                //            }
                //        }
                //        else
                //        {
                //            purgeTrxnCount = 0;
                //            baseFound = false;
                //        }
                //    }
                //    if (purgeTrxnCount == 3)
                //    {
                //        purgeCounter++;
                //        drInner = dtDispensing.NewRow();
                //        drInner[0] = dr[1];
                //        dtDispensing.Rows.Add(drInner);
                //        purgeTrxnCount = 0;
                //    }
                //    else
                //    {
                //        drArray = dt.Select("atm_id=" + dr[0]);
                //        string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(new char[] { ',' });
                //        if ((parts[0] == "1") && this.isFaulty(dt, "cash_dispensed1", "cash_remaining1", dr[0].ToString()))
                //        {
                //            faultyCounter++;
                //            drInner = dtFaulty.NewRow();
                //            drInner[0] = dr[1];
                //            dtFaulty.Rows.Add(drInner);
                //        }
                //        else if ((parts[1] == "1") && this.isFaulty(dt, "cash_dispensed2", "cash_remaining2", dr[0].ToString()))
                //        {
                //            faultyCounter++;
                //            drInner = dtFaulty.NewRow();
                //            drInner[0] = dr[1];
                //            dtFaulty.Rows.Add(drInner);
                //        }
                //        else if ((parts[2] == "1") && this.isFaulty(dt, "cash_dispensed3", "cash_remaining3", dr[0].ToString()))
                //        {
                //            faultyCounter++;
                //            drInner = dtFaulty.NewRow();
                //            drInner[0] = dr[1];
                //            dtFaulty.Rows.Add(drInner);
                //        }
                //        else if ((parts[3] == "1") && this.isFaulty(dt, "cash_dispensed4", "cash_remaining4", dr[0].ToString()))
                //        {
                //            faultyCounter++;
                //            drInner = dtFaulty.NewRow();
                //            drInner[0] = dr[1];
                //            dtFaulty.Rows.Add(drInner);
                //        }
                //        else
                //        {
                //            transactingCount++;
                //            drInner1 = dtTransacting.NewRow();
                //            drInner1[0] = dr[1];
                //            dtTransacting.Rows.Add(drInner1);
                //        }
                //    }
                //}
                //int total = ((transactingCount + faultyCounter) + purgeCounter) + inactivityCounter;
                //int len1 = 0;
                //int len2 = 0;
                //int len3 = 0;
                //int len4 = 0;
                //if (total != totalActiveATMs)
                //{
                //    inactivityCounter += totalActiveATMs - total;
                //    foreach (DataRow dr in dtATM.Rows)
                //    {
                //        len1 = dtFaulty.Select("title='" + dr[0] + "'").Length;
                //        len2 = dtDispensing.Select("title='" + dr[0] + "'").Length;
                //        len3 = dtTransacting.Select("title='" + dr[0] + "'").Length;
                //        len4 = dtInactivity.Select("title='" + dr[0] + "'").Length;
                //        if ((((len1 == 0) && (len2 == 0)) && (len3 == 0)) && (len4 == 0))
                //        {
                //            drInner1 = dtInactivity.NewRow();
                //            drInner1[0] = dr[0];
                //            dtInactivity.Rows.Add(drInner1);
                //        }
                //    }
                //}
                //StringBuilder builder = new StringBuilder();
                //foreach (DataRow dr in dtTransacting.Rows)
                //{
                //    builder.Append(dr[0] + ",");
                //}
                //this.hidden_transacting.Value = builder.ToString();
                //builder = new StringBuilder();
                //foreach (DataRow dr in dtFaulty.Rows)
                //{
                //    builder.Append(dr[0] + ",");
                //}
                //this.hidden_faulty.Value = builder.ToString();
                //builder = new StringBuilder();
                //foreach (DataRow dr in dtDispensing.Rows)
                //{
                //    builder.Append(dr[0] + ",");
                //}
                //this.hidden_dispensedFailed.Value = builder.ToString();
                //builder = new StringBuilder();
                //foreach (DataRow dr in dtInactivity.Rows)
                //{
                //    builder.Append(dr[0] + ",");
                //}
                //this.hidden_noActivity.Value = builder.ToString();
                this.Chart1.Series["Default"].Points.AddXY("Transacting ATMs", new object[] { transactingCount });
                this.Chart1.Series["Default"].Points.AddXY("ATMs Having Cassettes Faulty", new object[] { faultyCounter });
                this.Chart1.Series["Default"].Points.AddXY("ATMs Having Inactivity Period Elapsed", new object[] { inactivityCounter });
                this.Chart1.Series["Default"].Points.AddXY("ATMs Dispensing Failed", new object[] { dispensingFailed });
                this.Label_ATMsTrxn.Text = transactingCount.ToString();
                this.Label_CassettesFaulty.Text = faultyCounter.ToString();
                this.Label_NoActivity.Text = inactivityCounter.ToString();
                this.Label_ATMsDispensingFailed.Text = dispensingFailed.ToString();
                if (transactingCount > 0)
                {
                    this.Label_ATMsTrxn.Attributes.Add("onclick", "displayATM(0);");
                    this.Label_ATMsTrxn.Attributes.Add("style", "text-decoration:underline");
                }
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
                this.Chart1.Series["Default"].ChartType = SeriesChartType.Pie;
                this.Chart1.Series["Default"]["PieLabelStyle"] = "Outside";
                this.Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                this.Chart1.Series[0]["PieDrawingStyle"] = "SoftEdge";
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
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

        protected void GetAlertsHTML(string order)
        {
            LogableTask task = LogableTask.NewTask("GetAlertsHTML");
            SqlConnection conn = null;
            try
            {
                SqlCommand cmd = null;
                conn = ConnectionFactory.GetNewConnection();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = string.Concat(new object[] { "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select top 20 title, generated_at, alert_type_name from atm_alert,alert_type, atm, user_atms where atm_alert.alert_type_id = alert_type.alert_type_id and  atm_alert.atm_id = atm.atm_id and user_atms.atm_id = atm.atm_id and is_active = 1 and resolve_at is null and  generated_at >=convert(datetime,'", DateTime.Today.ToString("dd/MM/yyyy"), "',103) and generated_at <=convert(datetime,'", DateTime.Today.ToString("dd/MM/yyyy"), " 23:59:59',103) and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " order by ", order });
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmd.CommandText = string.Concat(new object[] { "select top 20 title, generated_at, alert_type_name from atm_alert, alert_type, atm, user_atms where atm_alert.alert_type_id = alert_type.alert_type_id and  atm_alert.atm_id = atm.atm_id and user_atms.atm_id = atm.atm_id and is_active = 1 and resolve_at is null and  generated_at<convert(datetime,'", DateTime.Today.ToString("dd/MM/yyyy"), "',103) and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " order by ", order });
                adapter = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable();
                adapter.Fill(dt1);
                this.GridView_Alerts_CurrentDay.DataSource = dt;
                this.GridView_Alerts_CurrentDay.DataBind();
                this.GridView_Alerts_PreviousDay.DataSource = dt1;
                this.GridView_Alerts_PreviousDay.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
                if ((conn != null) && (conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
            }
        }

        protected void GetATMsChequeColAlerts(string order)
        {
            LogableTask task = LogableTask.NewTask("GetATMsChequeColAlerts");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select top 1000 title, generated_at, alert_msg from atm_alert, atm \r\n                                    where atm_alert.atm_id = atm.atm_id \r\n                                    and alert_type_id = 17 order by " + order;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DataRow[] currentDayRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
                DataRow[] previousDayRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                if (currentDayRows.Length > 0)
                {
                    this.GridView_ChequeCollection_CurrentDay.DataSource = currentDayRows.CopyToDataTable<DataRow>();
                }
                else
                {
                    this.GridView_ChequeCollection_CurrentDay.DataSource = new DataTable();
                }
                this.GridView_ChequeCollection_CurrentDay.DataBind();
                if (previousDayRows.Length > 0)
                {
                    this.GridView_ChequeCollection_PreviousDay.DataSource = previousDayRows.CopyToDataTable<DataRow>();
                }
                else
                {
                    this.GridView_ChequeCollection_PreviousDay.DataSource = new DataTable();
                }
                this.GridView_ChequeCollection_PreviousDay.DataBind();
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

        protected void GetATMsDepositColAlerts(string order)
        {
            LogableTask task = LogableTask.NewTask("GetATMsDepositColAlerts");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select top 1000 title, generated_at, alert_msg from atm_alert, atm \r\n                                    where atm_alert.atm_id = atm.atm_id \r\n                                    and alert_type_id = 18 order by " + order;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DataRow[] currentDayRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
                DataRow[] previousDayRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                if (currentDayRows.Length > 0)
                {
                    this.GridView_DepositCollection_CurrentDay.DataSource = currentDayRows.CopyToDataTable<DataRow>();
                }
                else
                {
                    this.GridView_DepositCollection_CurrentDay.DataSource = new DataTable();
                }
                this.GridView_DepositCollection_CurrentDay.DataBind();
                if (previousDayRows.Length > 0)
                {
                    this.GridView_DepositCollection_PreviousDay.DataSource = previousDayRows.CopyToDataTable<DataRow>();
                }
                else
                {
                    this.GridView_DepositCollection_PreviousDay.DataSource = new DataTable();
                }
                this.GridView_DepositCollection_PreviousDay.DataBind();
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

        protected void GetATMsHavingLastTrxns(string order)
        {
            LogableTask task = LogableTask.NewTask("GetATMsHavingLastTrxns");
            SqlConnection conn = null;
            try
            {
                SqlCommand cmd = null;
                SqlDataReader reader = null;
                DataTable dt2 = new DataTable();
                dt2.Columns.Add("title");
                dt2.Columns.Add("trxn_datetime", typeof(DateTime));
                dt2.Columns.Add("amount");
                dt2.Columns.Add("last_heart_beat_at");
                dt2.Columns.Add("is_healthy", typeof(bool));
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = string.Concat(new object[] { "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select outerATM.title, trxn_datetime, amount ,outerATM.is_healthy,\r\n                                (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at\r\n\r\n\t                                from atm  outerATM inner join parsed_transaction\r\n\t                                on parsed_transaction.atm_id = outerATM.atm_id \r\n\t                                and trxn_datetime =  \r\n\t                                (\r\n\t                                select max(trxn_datetime)\r\n\t                                from parsed_transaction \r\n\r\n\t                                where atm_id = outerATM.atm_id)\r\n                                     where outerATM.atm_id in (select atm_id \r\n                                    from atm where is_active = 1 and atm_id not in (\r\n                                     select parsed_transaction.atm_id\r\n                                    from parsed_transaction , user_atms\r\n                                    where user_atms.atm_id = parsed_transaction.atm_id \r\n                                    and trxn_datetime >= convert(datetime, '", DateTime.Today.ToString("yyyy-MM-dd"), "', 120)  and trxn_datetime <= convert(datetime, '", DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd"), "', 120)   and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " and amount > 0  group by parsed_transaction.atm_id  having count(parsed_transaction_id)>0)) order by ", order });
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "fetched");
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DataRow dr = dt2.NewRow();
                    dr[0] = reader.GetString(0);
                    dr[1] = reader.GetDateTime(1);
                    dr[2] = reader.GetDecimal(2);
                    if (reader[4] != DBNull.Value)
                    {
                        dr[3] = reader.GetDateTime(4).ToString("dd/MM/yyyy HH:mm:ss");
                    }
                    else
                    {
                        dr[3] = "-";
                    }
                    dr[4] = reader.GetBoolean(3);
                    dt2.Rows.Add(dr);
                }
                reader.Close();
                this.GridView_AnyDay.DataSource = dt2;
                this.GridView_AnyDay.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
                if ((conn != null) && (conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
            }
        }

        protected void GetATMsHavingNoLastTrxns(string order)
        {
            LogableTask task = LogableTask.NewTask("GetATMsHavingNoLastTrxns");
            SqlConnection conn = null;
            try
            {
                SqlCommand cmd = null;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = string.Concat(new object[] { "select outerATM.title,outerATM.is_healthy,  (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at from atm outerATM where atm_id in (select outerATM.atm_id    from user_ATMs inner join atm outerATM   on user_ATMs.ATM_id = outerATM.ATM_id   left join parsed_transaction   on user_ATMs.ATM_id = parsed_transaction.atm_id   where outerATM.is_active = 1 and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " group by outerATM.atm_id  having COUNT(parsed_transaction.parsed_transaction_id)=0) order by ", order });
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                this.GridView_noTrxn.DataSource = dt;
                this.GridView_noTrxn.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
                if ((conn != null) && (conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
            }
        }

        protected void GetATMsHavingThresholdReached(string order)
        {
            LogableTask task = LogableTask.NewTask("GetATMsHavingThresholdReached");
            SqlConnection conn = null;
            SqlDataReader reader = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("generated_at", typeof(DateTime));
            dt.Columns.Add("amount");
            dt.Columns.Add("last_heart_beat_at");
            dt.Columns.Add("is_healthy", typeof(bool));
            dt.Columns.Add("last_trxn_at", typeof(DateTime));
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select title, generated_at, alert_msg,outerATM.atm_id,is_healthy,\r\n                                (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at\r\n                                     from atm_alert, atm outerATM\r\n                                     where atm_alert.atm_id = outerATM.atm_id                                      \r\n                                     and alert_type_id = 31 ";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = reader.GetString(0);
                    dr[1] = reader.GetDateTime(1);
                    string msg = reader.GetString(2);
                    if (msg.StartsWith("Last"))
                    {
                        decimal amount = -1M;
                        DateTime lastReportedTrxn = DateTime.ParseExact(msg.Replace("Last Reported Transaction ", ""), "dd/MM/yyyy HH:mm:ss", null);
                        object result = ConnectionFactory.ExecuteScalar(string.Concat(new object[] { "select amount from parsed_transaction where atm_id = ", reader.GetInt32(3), " and trxn_datetime=convert(datetime,'", lastReportedTrxn.ToString("dd/MM/yyyy HH:mm:ss"), "',103)" }));
                        if (result != null)
                        {
                            amount = (decimal)result;
                        }
                        dr[2] = amount;
                        dr[5] = lastReportedTrxn;
                    }
                    if (reader[5] != DBNull.Value)
                    {
                        dr[3] = reader.GetDateTime(5).ToString("dd/MM/yyyy HH:mm:ss");
                    }
                    else
                    {
                        dr[3] = "-";
                    }
                    dr[4] = reader.GetBoolean(4);
                    dt.Rows.Add(dr);
                }
                reader.Close();
                dt.DefaultView.Sort = order;
                this.GridView_thresholdReached.DataSource = dt;
                this.GridView_thresholdReached.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                if ((conn != null) && (conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
                task.EndTask();
            }
        }

        protected void GetATMsWithNoTrxnOnCurrentDay(string order)
        {
            LogableTask task = LogableTask.NewTask("GetATMsWithNoTrxnOnCurrentDay");
            SqlConnection conn = null;
            try
            {
                SqlCommand cmd = null;
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("title");
                dt1.Columns.Add("last_heart_beat_at");
                dt1.Columns.Add("is_healthy", typeof(bool));
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = string.Concat(new object[] { "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select outerATM.title,outerATM.is_healthy,\r\n                                (select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at\r\n\t                                from atm outerATM where is_active=1 and outerATM.atm_id not in (\r\n                                     select parsed_transaction.atm_id\r\n                                    from parsed_transaction , user_atms\r\n                                    where user_atms.atm_id = parsed_transaction.atm_id \r\n                                    and trxn_datetime >= convert(datetime, '", DateTime.Today.ToString("yyyy-MM-dd"), "', 120)  and trxn_datetime <= convert(datetime, '", DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd"), "', 120)   and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " and amount > 0  group by parsed_transaction.atm_id  having count(parsed_transaction_id)>0) order by ", order });
                new SqlDataAdapter(cmd).Fill(dt1);
                this.GridView_CurrentDay.DataSource = dt1;
                this.GridView_CurrentDay.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
                if ((conn != null) && (conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
            }
        }

        protected void GetCounterMismatchAlerts(string order)
        {
            LogableTask task = LogableTask.NewTask("GetCounterMismatchAlerts");
            DataTable dtPreviousDay = null;
            DataTable dtCurrentDay = null;
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select top 1000 title, generated_at, alert_msg from atm_alert, atm \r\n                                    where atm_alert.atm_id = atm.atm_id \r\n                                    and alert_type_id = 23 order by " + order;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DataRow[] currentDayRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
                DataRow[] previousDayRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                if (currentDayRows.Length > 0)
                {
                    dtCurrentDay = currentDayRows.CopyToDataTable<DataRow>();
                    foreach (DataRow dr in dtCurrentDay.Rows)
                    {
                        dr[2] = CashDataViewer.FormatInformation(dr[2].ToString());
                    }
                    dtCurrentDay.DefaultView.Sort = order;
                }
                this.GridView_CounterMismatch_CurrentDay.DataSource = dtCurrentDay;
                this.GridView_CounterMismatch_CurrentDay.DataBind();
                if (previousDayRows.Length > 0)
                {
                    dtPreviousDay = previousDayRows.CopyToDataTable<DataRow>();
                    foreach (DataRow dr in dtPreviousDay.Rows)
                    {
                        dr[2] = CashDataViewer.FormatInformation(dr[2].ToString());
                    }
                    dtPreviousDay.DefaultView.Sort = order;
                }
                this.GridView_CounterMismatch_PreviousDay.DataSource = dtPreviousDay;
                this.GridView_CounterMismatch_PreviousDay.DataBind();
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

        protected void GetCurrentFaultyRep(string order)
        {
            LogableTask task = LogableTask.NewTask("GetCurrentFaultyRep");
            //DataTable temp = null;
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                //cmd.CommandText = "select title,trxn_datetime,cash_dispensed1,cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,cash_dispensed7,parsed_transaction_id,atm.atm_id,cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4\r\n                                        from atm, parsed_transaction  where atm.atm_id = parsed_transaction.atm_id \r\n                                        and trxn_datetime >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  and trxn_datetime <= convert(datetime,'" + DateTime.Today.AddDays(1.0).ToString("dd/MM/yyyy") + " 23:59:59',103) order by trxn_datetime";
                cmd.CommandText = "select title,generated_at,alert_msg from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.ATMCassetteFaulty + " and resolve_at is null and generated_at>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and generated_at<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) ";

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                //if (dt.Rows.Count > 0)
                //{
                //    List<int> atmIDs = new List<int>();
                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        int atmID = int.Parse(dr["atm_id"].ToString());
                //        if (!atmIDs.Contains(atmID))
                //        {
                //            atmIDs.Add(atmID);
                //        }
                //    }
                //    DataTable dtinner = new DataTable();
                //    dtinner.Columns.Add("title");
                //    dtinner.Columns.Add("status1");
                //    dtinner.Columns.Add("status2");
                //    dtinner.Columns.Add("status3");
                //    dtinner.Columns.Add("status4");
                //    dtinner.Columns.Add("trxn_datetime");
                //    dtinner.PrimaryKey = new DataColumn[] { dtinner.Columns["title"] };
                //    temp = dtinner.Clone();
                //    string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(new char[] { ',' });
                //    if (parts[0] == "1")
                //    {
                //        this.ProcessDataTable("cash_dispensed1", dt, dtinner, atmIDs, 1, "cash_remaining1");
                //    }
                //    if (parts[1] == "1")
                //    {
                //        this.ProcessDataTable("cash_dispensed2", dt, dtinner, atmIDs, 2, "cash_remaining2");
                //    }
                //    if (parts[2] == "1")
                //    {
                //        this.ProcessDataTable("cash_dispensed3", dt, dtinner, atmIDs, 3, "cash_remaining3");
                //    }
                //    if (parts[3] == "1")
                //    {
                //        this.ProcessDataTable("cash_dispensed4", dt, dtinner, atmIDs, 4, "cash_remaining4");
                //    }
                //    dt.Rows.Clear();
                //    foreach (DataRow dr in dtinner.Rows)
                //    {
                //        if ((((dr[1].ToString() == "Faulty") || (dr[2].ToString() == "Faulty")) || (dr[3].ToString() == "Faulty")) || (dr[4].ToString() == "Faulty"))
                //        {
                //            DataRow newRow = temp.NewRow();
                //            newRow[0] = dr[0];
                //            newRow[1] = dr[1];
                //            newRow[2] = dr[2];
                //            newRow[3] = dr[3];
                //            newRow[4] = dr[4];
                //            newRow[5] = dr[5];
                //            temp.Rows.Add(newRow);
                //        }
                //    }
                //    temp.DefaultView.Sort = order;
                //}
                dt.DefaultView.Sort = order;
                this.GridView_FaultyRep_Current.DataSource = dt;
                this.GridView_FaultyRep_Current.DataBind();
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

        protected void GetDateTimeSyncTask(string order)
        {
            LogableTask task = LogableTask.NewTask("GetDateTimeSyncTask");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;\r\n                                     select title,status,last_invoked,end_time\r\n                                        from task inner join atm \r\n                                        on task.atm_id = atm.atm_id\r\n                                        where task_type_id = 6\r\n                                        and task.creation_time >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and task.creation_time <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) order by " + order;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                this.GridView_DatetimeSync.DataSource = dt;
                this.GridView_DatetimeSync.DataBind();
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

        protected void GetDeadATMs(string order)
        {
            LogableTask task = LogableTask.NewTask("GetDeadATMs");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;\r\n                                     select outerATM.title,(select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at \r\nfrom atm outerATM\r\nwhere is_active = 1 and ATM_id not in (select ATM_id from heart_beat where heart_beat_received_at >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and heart_beat_received_at <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)) order by " + order;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                this.GridView_DeadATM.DataSource = dt;
                this.GridView_DeadATM.DataBind();
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


        protected void DrawDFFInfoGraph()
        {
            LogableTask task = LogableTask.NewTask("DrawDFFInfoGraph");
            try
            {
                StringBuilder builderNoData = new StringBuilder();
                StringBuilder builderSending2 = new StringBuilder();
                StringBuilder builderSendingSameBalance = new StringBuilder();
                StringBuilder builderInconsistentData = new StringBuilder();

                Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegion("is_organization=1 and region_id<> 1 order by region_id");
                Label_org.Text = region.RegionName;
                //null;
                //DataRow dr;
                string[] content = null;
                string fileNameConvention = WebConfigurationManager.AppSettings["DFFFileNameConvention"];
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
                    throw new Exception("File does not exists." + fileNameConvention);
                }
                int atmSending2 = 0;
                int atmSendingSameBalance = 0;
                int atmInconsistentData = 0;
                int atmSendingNoData = 0;

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
                        atmSending2++;
                        builderSending2.Append(atmID + ",");
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
                                    atmSendingSameBalance++;
                                    builderSendingSameBalance.Append(atmID + ",");
                                }
                                decimal inconsistentAmount = (((summary.ClosingBalance + decimal.Parse(replenishmentAmount)) - decimal.Parse(returnAmount)) - decimal.Parse(withdrawalAmount)) - decimal.Parse(closingBalance);
                                if (inconsistentAmount != 0M)
                                {
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Concat(new object[] { "Opening Balance: ", summary.ClosingBalance, "\r\nReplenishment Amount: ", replenishmentAmount, "\r\nReturn Amount: ", returnAmount, "\r\nWithdrawal Amount: ", withdrawalAmount, "\r\nClosing Balance: ", closingBalance, "\r\nInconsistent Amount: ", inconsistentAmount }));
                                    atmInconsistentData++;
                                    builderInconsistentData.Append(atmID + ",");

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
                        atmSendingNoData++;
                        builderNoData.Append(reader.CurrentAtm.Title + ",");
                    }
                }
                reader.Close();


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
                this.GridView_NotInDFF.DataSource = dtSendingNoData;
                this.GridView_NotInDFF.DataBind();
                dtSending2.DefaultView.Sort = order;
                this.GridView_Sending2.DataSource = dtSending2;
                this.GridView_Sending2.DataBind();
                dtSendingYesterdayBalance.DefaultView.Sort = order;
                this.GridView_SameOpeningBalanceAsOfYesterday.DataSource = dtSendingYesterdayBalance;
                this.GridView_SameOpeningBalanceAsOfYesterday.DataBind();
                dtInconsistent.DefaultView.Sort = order;
                this.GridView_InconsistentData.DataSource = dtInconsistent;
                this.GridView_InconsistentData.DataBind();
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

        protected void GetFaultyRep(string order)
        {
            LogableTask task = LogableTask.NewTask("GetFaultyRep");
            //DataTable temp = null;
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
               // cmd.CommandText = "select title,trxn_datetime,cash_dispensed1,cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,cash_dispensed7,parsed_transaction_id,atm.atm_id,cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4\r\n                                        from atm, parsed_transaction  where atm.atm_id = parsed_transaction.atm_id \r\n                                        and trxn_datetime >= convert(datetime,'" + DateTime.Today.AddDays(-1.0).ToString("dd/MM/yyyy") + "',103)  and trxn_datetime <= convert(datetime,'" + DateTime.Today.AddDays(-1.0).ToString("dd/MM/yyyy") + " 23:59:59',103) order by trxn_datetime";

                cmd.CommandText = "select title,generated_at,alert_msg from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.ATMCassetteFaulty + " and resolve_at is null and generated_at<convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) ";
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                //if (dt.Rows.Count > 0)
                //{
                //    List<int> atmIDs = new List<int>();
                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        int atmID = int.Parse(dr["atm_id"].ToString());
                //        if (!atmIDs.Contains(atmID))
                //        {
                //            atmIDs.Add(atmID);
                //        }
                //    }
                //    DataTable dtinner = new DataTable();
                //    dtinner.Columns.Add("title");
                //    dtinner.Columns.Add("status1");
                //    dtinner.Columns.Add("status2");
                //    dtinner.Columns.Add("status3");
                //    dtinner.Columns.Add("status4");
                //    dtinner.Columns.Add("trxn_datetime");
                //    dtinner.PrimaryKey = new DataColumn[] { dtinner.Columns["title"] };
                //    temp = dtinner.Clone();
                //    string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(new char[] { ',' });
                //    if (parts[0] == "1")
                //    {
                //        this.ProcessDataTable("cash_dispensed1", dt, dtinner, atmIDs, 1, "cash_remaining1");
                //    }
                //    if (parts[1] == "1")
                //    {
                //        this.ProcessDataTable("cash_dispensed2", dt, dtinner, atmIDs, 2, "cash_remaining2");
                //    }
                //    if (parts[2] == "1")
                //    {
                //        this.ProcessDataTable("cash_dispensed3", dt, dtinner, atmIDs, 3, "cash_remaining3");
                //    }
                //    if (parts[3] == "1")
                //    {
                //        this.ProcessDataTable("cash_dispensed4", dt, dtinner, atmIDs, 4, "cash_remaining4");
                //    }
                //    dt.Rows.Clear();
                //    foreach (DataRow dr in dtinner.Rows)
                //    {
                //        if ((((dr[1].ToString() == "Faulty") || (dr[2].ToString() == "Faulty")) || (dr[3].ToString() == "Faulty")) || (dr[4].ToString() == "Faulty"))
                //        {
                //            DataRow newRow = temp.NewRow();
                //            newRow[0] = dr[0];
                //            newRow[1] = dr[1];
                //            newRow[2] = dr[2];
                //            newRow[3] = dr[3];
                //            newRow[4] = dr[4];
                //            newRow[5] = dr[5];
                //            temp.Rows.Add(newRow);
                //        }
                //    }
                //    temp.DefaultView.Sort = order;
                //}
                dt.DefaultView.Sort = order;
                this.GridView_FaultyRep.DataSource = dt;
                this.GridView_FaultyRep.DataBind();
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

        protected void GetLowBalanceATMs()
        {
            LogableTask task = LogableTask.NewTask("GetLowBalanceATMs");
            try
            {
                this.BindLowBalanceGrid("title");
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

        protected void GetPurgeBinAlerts(string order)
        {
            LogableTask task = LogableTask.NewTask("GetPurgeBinAlerts");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select title, generated_at, isnull(alert_msg,'') alert_msg\r\n                                    from atm_alert,atm outerATM\r\n                                    where atm_alert.atm_id = outerATM.atm_id \r\n                                    and alert_type_id =22 and outerATM.is_active =1 \r\n                                    and atm_alert_id in \r\n                                    (\r\n                                    select max(atm_alert_id) from atm_alert\r\n                                    where atm_id = outerATM.atm_id\r\n                                    and alert_type_id =22 and resolve_at is null\r\n                                    ) order by " + order;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DataRow[] currentDayPurgedBinRows = dt.Select(string.Concat(new object[] { "generated_at >=#", DateTime.Today, "# and  generated_at <=#", DateTime.Today.AddDays(1.0), "#" }));
                DataRow[] previousDayPurgedBinRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                DataTable dtPurgedBinCurrentDay = this.CreatePurgedDatatableFromDataRows(currentDayPurgedBinRows);
                dtPurgedBinCurrentDay.DefaultView.Sort = order;
                this.GridView_PurgeBinAlerts_CurrentDay.DataSource = dtPurgedBinCurrentDay;
                this.GridView_PurgeBinAlerts_CurrentDay.DataBind();
                DataTable dtPurgedBinPreviousDay = this.CreatePurgedDatatableFromDataRows(previousDayPurgedBinRows);
                dtPurgedBinPreviousDay.DefaultView.Sort = order;
                this.GridView_PurgeBinAlert_PreviousDay.DataSource = dtPurgedBinPreviousDay;
                this.GridView_PurgeBinAlert_PreviousDay.DataBind();
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

        protected void GetPurgedCounterTrxn(string order)
        {
            LogableTask task = LogableTask.NewTask("GetPurgedCounterTrxn");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;\r\n                                     select title,COUNT(parsed_transaction_id) count\r\n                                        from parsed_transaction inner join atm \r\n                                        on parsed_transaction.atm_id = atm.atm_id\r\n                                        where amount =0\r\n                                        and trxn_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and trxn_datetime <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) group by title order by " + order;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dt.DefaultView.Sort = order;
                this.GridView_PurgedCounter.DataSource = dt;
                this.GridView_PurgedCounter.DataBind();
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

        protected void GetRecentActivityHTML(int day)
        {
            LogableTask task = LogableTask.NewTask("GetRecentActivityHTML");
            try
            {
                SqlCommand cmd = null;
                DataTable dt = new DataTable();
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

        protected void GetScheduleSummary()
        {
            LogableTask task = LogableTask.NewTask("GetScheduleSummary");
            SqlConnection conn = null;
            try
            {
                SqlCommand cmd = null;
                SqlDataReader reader = null;
                DataTable dt = new DataTable();
                dt.Columns.Add("Activity");
                dt.Columns.Add("Count");
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED";
                cmd.ExecuteNonQuery();
                cmd.CommandText = string.Concat(new object[] { "select count(*) \r\n                                    from scheduled_downloads, user_atms,atm \r\n                                    where user_atms.atm_id = scheduled_downloads.atm_id and user_atms.atm_id = atm.atm_id and atm.is_active = 1\r\n                                    and file_type_id = 1 and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, ";select count(*) \r\n                                    from task, user_atms, atm \r\n                                    where user_atms.atm_id = task.atm_id and atm.atm_id = atm.atm_id and atm.is_active = 1\r\n                                    and task.creation_time >= convert(datetime, '", DateTime.Today.ToString("yyyy-MM-dd"), "', 120)  and task.creation_time <= convert(datetime, '", DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd"), "', 120) and file_type_id = 1 and status in ('downloadedParsePending','downloadedParsing','completed') and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, ";select count(*) \r\n                                    from task, user_atms, atm  \r\n                                    where user_atms.atm_id = task.atm_id and user_atms.atm_id = atm.atm_id and atm.is_active = 1 \r\n                                    and task.creation_time >= convert(datetime, '", DateTime.Today.ToString("yyyy-MM-dd"), "', 120)  and task.creation_time <= convert(datetime, '", DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd"), "', 120)   and file_type_id = 1 and status in ('retriesExhausted','cancelled','unknownError')   and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, ";" });
                reader = cmd.ExecuteReader();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "fetched");
                reader.Read();
                DataRow dr = dt.NewRow();
                dr[0] = "Scheduled";
                dr[1] = reader[0];
                dt.Rows.Add(dr);
                reader.NextResult();
                reader.Read();
                dr = dt.NewRow();
                dr[0] = "Downloaded";
                dr[1] = reader[0];
                dt.Rows.Add(dr);
                reader.NextResult();
                reader.Read();
                dr = dt.NewRow();
                dr[0] = "Failed";
                dr[1] = reader[0];
                dt.Rows.Add(dr);
                reader.Close();
                this.GridView_DownloadSummary.DataSource = dt;
                this.GridView_DownloadSummary.DataBind();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "strings built");
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
                if ((conn != null) && (conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
            }
        }

        protected void GetSuggestedReplenishments(string order)
        {
            LogableTask task = LogableTask.NewTask("GetSuggestedReplenishments");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select title, cassette1_notes,cassette2_notes,cassette3_notes,cassette4_notes from cash_position inner join atm \r\n                                    on cash_position.atm_id = atm.atm_id \r\n                                    where  atm.is_active=1 and last_trxn_at >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  and last_trxn_at <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and (" + this.FilterConfiguredCassettes() + " ) order by " + order;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtSuggestedRep = new DataTable();
                adapter.Fill(dtSuggestedRep);
                this.GridView_SuggestedRep.DataSource = dtSuggestedRep;
                this.GridView_SuggestedRep.DataBind();
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

        private void GetThresholdOfHour(int hr, string orderby)
        {
            LogableTask task = LogableTask.NewTask("GetThresholdOfHour");
            SqlConnection conn = null;
            SqlDataReader reader = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("generated_at", typeof(DateTime));
            dt.Columns.Add("amount");
            dt.Columns.Add("last_heart_beat_at");
            dt.Columns.Add("is_healthy", typeof(bool));
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = "\r\n                                        select outerATM.atm_id, max(trxn_datetime) trxn_datetime,\r\n(select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at\r\n                                        from atm outerATM left join parsed_transaction\r\n                                        on parsed_transaction.atm_id = outerATM.atm_id \r\n                                        where is_active = 1 and allowed_inactivity_period is not null\r\n                                        group by outerATM.atm_id                                        ";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Atm atm = Atm.LoadAtmByPk(reader.GetInt32(0));
                    if (reader[1] != DBNull.Value)
                    {
                        DateTime maxDateTime = reader.GetDateTime(1);
                        if (maxDateTime.AddHours((double)hr) < DateTime.Now)
                        {
                            ParsedTransaction parsedTransaction = ParsedTransaction.LoadParsedTransaction(string.Concat(new object[] { "trxn_datetime=convert(datetime,'", maxDateTime.ToString("dd/MM/yyyy HH:mm:ss"), "',103) and atm_id = ", atm.ATMId }));
                            DataRow dr = dt.NewRow();
                            dr[0] = atm.Title;
                            dr[1] = maxDateTime;
                            if (parsedTransaction != null)
                            {
                                dr[2] = parsedTransaction.Amount;
                            }
                            if (reader[2] != DBNull.Value)
                            {
                                dr[3] = reader.GetDateTime(2).ToString("dd/MM/yyyy HH:mm:ss");
                            }
                            else
                            {
                                dr[3] = "-";
                            }
                            dr[4] = atm.IsHealthy;
                            dt.Rows.Add(dr);
                        }
                    }
                }
                reader.Close();
                dt.DefaultView.Sort = orderby;
                if (hr == 1)
                {
                    this.GridView_Threshold1.DataSource = dt;
                    this.GridView_Threshold1.DataBind();
                }
                else if (hr == 4)
                {
                    this.GridView_Threshold4.DataSource = dt;
                    this.GridView_Threshold4.DataBind();
                }
                else if (hr == 8)
                {
                    this.GridView_Threshold8.DataSource = dt;
                    this.GridView_Threshold8.DataBind();
                }
                else if (hr == 0x10)
                {
                    this.GridView_Threshold16.DataSource = dt;
                    this.GridView_Threshold16.DataBind();
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.literal_script.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                if ((conn != null) && (conn.State == ConnectionState.Open))
                {
                    conn.Close();
                }
                task.EndTask();
            }
        }

        private void GetTop10LowTransactingATM()
        {
            LogableTask task = LogableTask.NewTask("GetTop10LowTransactingATM");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetTop10ATMs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("ForDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter("order", SqlDbType.Int));
                cmd.Parameters[1].Value = 0;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
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

        private void GetTop10TransactingATM()
        {
            LogableTask task = LogableTask.NewTask("GetTop10TransactingATM");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetTop10ATMs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("ForDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter("order", SqlDbType.Int));
                cmd.Parameters[1].Value = 1;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
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

        protected void GetTopATMHTML(string order)
        {
            LogableTask task = LogableTask.NewTask("GetTopATMHTML");
            try
            {
                SqlCommand cmd = null;
                DataTable dt = new DataTable();
                cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = string.Concat(new object[] { "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select atm.title Title,count(*) Count from atm inner join parsed_transaction  on atm.atm_id = parsed_transaction.atm_id  inner join user_atms on user_atms.atm_id = atm.atm_id  where trxn_datetime >= convert(datetime, '", DateTime.Today.ToString("yyyy-MM-dd"), "', 120)  and trxn_datetime <= convert(datetime, '", DateTime.Today.AddDays(1.0).ToString("yyyy-MM-dd"), "', 120)  and user_atms.user_id = ", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, " group by title order by ", order });
                new SqlDataAdapter(cmd).Fill(dt);
                this.GridView_TransactionATMs.DataSource = dt;
                this.GridView_TransactionATMs.DataBind();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "string built");
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

        protected void GridView_Alerts_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetAlertsHTML((string)this.ViewState["orderby"]);
        }

        protected void GridView_Cheque_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetATMsChequeColAlerts((string)this.ViewState["orderby"]);
        }

        protected void GridView_CounterMismatch_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetCounterMismatchAlerts((string)this.ViewState["orderby"]);
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

        protected void GridView_CurrentDay_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionCurrentDayNoTrxn"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionCurrentDayNoTrxn"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionCurrentDayNoTrxn"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionCurrentDayNoTrxn"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionCurrentDayNoTrxn"] = e.SortExpression;
                this.ViewState["sortDirectionCurrentDayNoTrxn"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionCurrentDayNoTrxn"] + " " + this.ViewState["sortDirectionCurrentDayNoTrxn"];
            this.GetATMsWithNoTrxnOnCurrentDay((string)this.ViewState["orderby"]);
        }

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

        protected void GridView_DatetimeSync_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetDateTimeSyncTask((string)this.ViewState["orderby"]);
        }

        protected void GridView_DeadATM_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetDeadATMs((string)this.ViewState["orderby"]);
        }

        protected void GridView_Deposit_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetATMsDepositColAlerts((string)this.ViewState["orderby"]);
        }

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

        protected void GridView_FaultyRep_Current_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionCurrentFaultyRep"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionCurrentFaultyRep"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionCurrentFaultyRep"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionCurrentFaultyRep"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionCurrentFaultyRep"] = e.SortExpression;
                this.ViewState["sortDirectionCurrentFaultyRep"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionCurrentFaultyRep"] + " " + this.ViewState["sortDirectionCurrentFaultyRep"];
            this.GetCurrentFaultyRep((string)this.ViewState["orderby"]);
        }

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

        protected void GridView_FaultyReplenishment_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionFaultyRep"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionFaultyRep"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionFaultyRep"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionFaultyRep"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionFaultyRep"] = e.SortExpression;
                this.ViewState["sortDirectionFaultyRep"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionFaultyRep"] + " " + this.ViewState["sortDirectionFaultyRep"];
            this.GetFaultyRep((string)this.ViewState["orderby"]);
        }

        protected void GridView_LastTrxn_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionHavingLastTrxn"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionHavingLastTrxn"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionHavingLastTrxn"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionHavingLastTrxn"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionHavingLastTrxn"] = e.SortExpression;
                this.ViewState["sortDirectionHavingLastTrxn"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionHavingLastTrxn"] + " " + this.ViewState["sortDirectionHavingLastTrxn"];
            this.GetATMsHavingLastTrxns((string)this.ViewState["orderby"]);
        }

        protected void GridView_LowBalance_Sorting(object sender, GridViewSortEventArgs e)
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
            this.BindLowBalanceGrid((string)this.ViewState["orderby"]);
        }

        protected void GridView_NoLastTrxn_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionHavingNoLastTrxn"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionHavingNoLastTrxn"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionHavingNoLastTrxn"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionHavingNoLastTrxn"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionHavingNoLastTrxn"] = e.SortExpression;
                this.ViewState["sortDirectionHavingNoLastTrxn"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionHavingNoLastTrxn"] + " " + this.ViewState["sortDirectionHavingNoLastTrxn"];
            this.GetATMsHavingNoLastTrxns((string)this.ViewState["orderby"]);
        }

        protected void Gridview_purgebin_sorting(object sender, GridViewSortEventArgs e)
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
            this.GetPurgeBinAlerts((string)this.ViewState["orderby"]);
        }

        protected void GridView_PurgedCounter_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetPurgedCounterTrxn((string)this.ViewState["orderby"]);
        }

        protected void GridView_SuggestedReplenishment_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetSuggestedReplenishments((string)this.ViewState["orderby"]);
        }

        protected void GridView_Threshold1_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionThreshold1"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionThreshold1"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionThreshold1"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionThreshold1"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionThreshold1"] = e.SortExpression;
                this.ViewState["sortDirectionThreshold1"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionThreshold1"] + " " + this.ViewState["sortDirectionThreshold1"];
            this.GetThresholdOfHour(1, (string)this.ViewState["orderby"]);
        }

        protected void GridView_Threshold16_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionThreshold16"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionThreshold16"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionThreshold16"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionThreshold16"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionThreshold16"] = e.SortExpression;
                this.ViewState["sortDirectionThreshold16"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionThreshold16"] + " " + this.ViewState["sortDirectionThreshold16"];
            this.GetThresholdOfHour(0x10, (string)this.ViewState["orderby"]);
        }

        protected void GridView_Threshold4_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionThreshold4"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionThreshold4"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionThreshold4"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionThreshold4"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionThreshold4"] = e.SortExpression;
                this.ViewState["sortDirectionThreshold4"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionThreshold4"] + " " + this.ViewState["sortDirectionThreshold4"];
            this.GetThresholdOfHour(4, (string)this.ViewState["orderby"]);
        }

        protected void GridView_Threshold8_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionThreshold8"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionThreshold8"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionThreshold8"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionThreshold8"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionThreshold8"] = e.SortExpression;
                this.ViewState["sortDirectionThreshold8"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionThreshold8"] + " " + this.ViewState["sortDirectionThreshold8"];
            this.GetThresholdOfHour(8, (string)this.ViewState["orderby"]);
        }

        protected void GridView_ThresholdReached_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpressionHavingThresholdReached"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirectionHavingThresholdReached"].ToString() == " asc")
                {
                    this.ViewState["sortDirectionHavingThresholdReached"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirectionHavingThresholdReached"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpressionHavingThresholdReached"] = e.SortExpression;
                this.ViewState["sortDirectionHavingThresholdReached"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpressionHavingThresholdReached"] + " " + this.ViewState["sortDirectionHavingThresholdReached"];
            this.GetATMsHavingThresholdReached((string)this.ViewState["orderby"]);
        }

        protected void GridView_TransactionATMs_Sorting(object sender, GridViewSortEventArgs e)
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
            this.GetTopATMHTML((string)this.ViewState["orderby"]);
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("SummaryPageLoad");
            try
            {
                if (!this.Page.IsPostBack && !this.DialogCallBack.CausedCallback)
                {
                    int width = int.Parse(this.Session["width"].ToString()) - 100;
                    this.container.Attributes.Add("style", "width:" + width.ToString() + "px");
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to execute ReplenishmentsWithoutOrder at " + DateTime.Now.ToString());
                    this.ReplenishmentsWithoutOrder();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "ReplenishmentsWithoutOrder completed at " + DateTime.Now.ToString());
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling GetTop10LowTransactingATM at " + DateTime.Now.ToString());
                    this.GetTop10LowTransactingATM();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed GetTop10LowTransactingATM at " + DateTime.Now.ToString());
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling GetTop10TransactingATM at " + DateTime.Now.ToString());
                    this.GetTop10TransactingATM();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed GetTop10TransactingATM at " + DateTime.Now.ToString());
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling DisplayHaltedATMs at " + DateTime.Now.ToString());
                    this.DisplayHaltedATMs();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayHaltedATMs at " + DateTime.Now.ToString());
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplaySummaryOfTodaysAlert at " + DateTime.Now.ToString());
                    this.DisplaySummaryOfTodaysAlert();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplaySummaryOfTodaysAlert at " + DateTime.Now.ToString());
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DrawGraph at " + DateTime.Now.ToString());
                    this.DrawGraph();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawGraph at " + DateTime.Now.ToString());
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayUserTask at " + DateTime.Now.ToString());
                    this.DisplayUserTask();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayUserTask at " + DateTime.Now.ToString());
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayCurrentOrder at " + DateTime.Now.ToString());
                    this.DisplayCurrentOrder();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayCurrentOrder at " + DateTime.Now.ToString());
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayCallOutSummary at " + DateTime.Now.ToString());
                    this.DisplayCallOutSummary();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayCallOutSummary at " + DateTime.Now.ToString());
                    this.DisplayCurrentLoggedInUser();
                    DrawChequeDepositGraph();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawChequeDepositGraph at " + DateTime.Now.ToString());
                    DrawNotesAcceptorGraph();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawNotesAcceptorGraph at " + DateTime.Now.ToString());
                    DisplayCITTasks();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayCITTasks at " + DateTime.Now.ToString());
                    DrawDFFInfoGraph();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawDFFInfoGraph at " + DateTime.Now.ToString());
                    int count = 0;
                    if (!base.CheckRight(0x6a))
                    {
                        this.TabStripTab_RecentActivity.Visible = false;
                        count++;
                    }
                    else
                    {
                        this.RecentActivityMenu1_Click(null, null);
                    }
                    if (!base.CheckRight(0x6b))
                    {
                        this.TabStripTab_Communication.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0x6c))
                    {
                        this.TabStripTab_Alerts.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0x6d))
                    {
                        this.TabStripTab_MostActiveATMs.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(110))
                    {
                        this.TabStripTab_ATMsWithoutTrxns.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0x6f))
                    {
                        this.TabStripTab_Cheque.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0x70))
                    {
                        this.TabStripTab_Deposit.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0x71))
                    {
                        this.TabStripTab_LowBalance.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0xbbc))
                    {
                        this.TabStripTab_PurgeBinAlerts.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0xbb8))
                    {
                        this.TabStripTab_PurgedCount.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0xbb9))
                    {
                        this.TabStripTab_DFF.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0xbcd))
                    {
                        this.TabStripTab_CounterMismatch.Visible = false;
                        count++;
                    }
                    if (!base.CheckRight(0xbce))
                    {
                        this.TabStripTab_Replenishments.Visible = false;
                        count++;
                    }
                    if (count == 13)
                    {
                        this.literal_script.Text = "<li>You do not have rights to view summary data.Please contact NCR administrator";
                    }
                    Label_generatedAt.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
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
            this.GetRecentActivityHTML(0);
        }

        protected void RecentActivityMenu2_Click(object sender, EventArgs e)
        {
            this.GetRecentActivityHTML(-1);
        }

        protected void RecentActivityMenu3_Click(object sender, EventArgs e)
        {
            this.GetRecentActivityHTML(-2);
        }

        protected void RecentActivityMenu4_Click(object sender, EventArgs e)
        {
            this.GetRecentActivityHTML(-6);
        }

        private void ReplenishmentsWithoutOrder()
        {
            LogableTask task = LogableTask.NewTask("ReplenishmentsWithoutOrder");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetRepWithoutOrders";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
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

        protected void TabStrip_Summary_ItemSelected(object sender, TabStripTabEventArgs e)
        {
            this.ViewState["SortExpression"] = "title";
            this.ViewState["sortDirection"] = " asc";
            switch (e.Tab.ID)
            {
                case "TabStripTab_RecentActivity":
                    this.RecentActivityMenu1_Click(null, null);
                    break;

                case "TabStripTab_Alerts":
                case "TabStripTab12":
                case "TabStripTab13":
                    this.GetAlertsHTML(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_Communication":
                    this.GetScheduleSummary();
                    break;

                case "TabStripTab_Transactions":
                    this.GetTopATMHTML(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_MostActiveATMs":
                    this.GetTopATMHTML(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_ATMsWithoutTrxns":
                case "TabStripTab1":
                    this.ViewState["SortExpressionCurrentDayNoTrxn"] = "title";
                    this.ViewState["sortDirectionCurrentDayNoTrxn"] = " asc";
                    this.GetATMsWithNoTrxnOnCurrentDay(((string)this.ViewState["SortExpressionCurrentDayNoTrxn"]) + ((string)this.ViewState["sortDirectionCurrentDayNoTrxn"]));
                    break;

                case "TabStripTab2":
                    this.ViewState["SortExpressionHavingLastTrxn"] = "title";
                    this.ViewState["sortDirectionHavingLastTrxn"] = " asc";
                    this.GetATMsHavingLastTrxns(((string)this.ViewState["SortExpressionHavingLastTrxn"]) + ((string)this.ViewState["sortDirectionHavingLastTrxn"]));
                    break;

                case "TabStripTab3":
                    this.ViewState["SortExpressionHavingNoLastTrxn"] = "title";
                    this.ViewState["sortDirectionHavingNoLastTrxn"] = " asc";
                    this.GetATMsHavingNoLastTrxns(((string)this.ViewState["SortExpressionHavingNoLastTrxn"]) + ((string)this.ViewState["sortDirectionHavingNoLastTrxn"]));
                    break;

                case "TabStripTab16":
                    this.ViewState["SortExpressionHavingThresholdReached"] = "title";
                    this.ViewState["sortDirectionHavingThresholdReached"] = " asc";
                    this.GetATMsHavingThresholdReached(((string)this.ViewState["SortExpressionHavingThresholdReached"]) + ((string)this.ViewState["sortDirectionHavingThresholdReached"]));
                    break;

                case "TabStripTab_Cheque":
                case "TabStripTab8":
                case "TabStripTab9":
                    this.GetATMsChequeColAlerts(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_Deposit":
                case "TabStripTab10":
                case "TabStripTab11":
                    this.GetATMsDepositColAlerts(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_LowBalance":
                case "TabStripTab4":
                case "TabStripTab5":
                    this.BindLowBalanceGrid(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_PurgeBinAlerts":
                case "TabStripTab6":
                case "TabStripTab7":
                    this.GetPurgeBinAlerts(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_DeadATMs":
                    this.GetDeadATMs(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_DatetimeSync":
                    this.GetDateTimeSyncTask(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_PurgedCount":
                    this.GetPurgedCounterTrxn(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_DFF":
                    this.GetDFFInfo(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_CounterMismatch":
                case "TabStripTab14":
                case "TabStripTab15":
                    this.GetCounterMismatchAlerts(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab_Replenishments":
                case "TabStripTab17":
                    this.GetSuggestedReplenishments(((string)this.ViewState["SortExpression"]) + ((string)this.ViewState["sortDirection"]));
                    break;

                case "TabStripTab18":
                case "TabStripTab24":
                    this.ViewState["SortExpressionCurrentFaultyRep"] = "title";
                    this.ViewState["sortDirectionCurrentFaultyRep"] = " asc";
                    this.GetCurrentFaultyRep(((string)this.ViewState["SortExpressionCurrentFaultyRep"]) + ((string)this.ViewState["sortDirectionCurrentFaultyRep"]));
                    break;

                case "TabStripTab25":
                    this.ViewState["SortExpressionFaultyRep"] = "title";
                    this.ViewState["sortDirectionFaultyRep"] = " asc";
                    this.GetFaultyRep(((string)this.ViewState["SortExpressionFaultyRep"]) + ((string)this.ViewState["sortDirectionFaultyRep"]));
                    break;

                case "TabStripTab20":
                case "TabStrip6":
                case "TabStripTab19":
                    this.ViewState["SortExpressionThreshold1"] = "title";
                    this.ViewState["sortDirectionThreshold1"] = " asc";
                    this.GetThresholdOfHour(1, ((string)this.ViewState["SortExpressionThreshold1"]) + ((string)this.ViewState["sortDirectionThreshold1"]));
                    break;

                case "TabStripTab21":
                    this.ViewState["SortExpressionThreshold4"] = "title";
                    this.ViewState["sortDirectionThreshold4"] = " asc";
                    this.GetThresholdOfHour(4, ((string)this.ViewState["SortExpressionThreshold4"]) + ((string)this.ViewState["sortDirectionThreshold4"]));
                    break;

                case "TabStripTab22":
                    this.ViewState["SortExpressionThreshold8"] = "title";
                    this.ViewState["sortDirectionThreshold8"] = " asc";
                    this.GetThresholdOfHour(8, ((string)this.ViewState["SortExpressionThreshold8"]) + ((string)this.ViewState["sortDirectionThreshold8"]));
                    break;

                case "TabStripTab23":
                    this.ViewState["SortExpressionThreshold16"] = "title";
                    this.ViewState["sortDirectionThreshold16"] = " asc";
                    this.GetThresholdOfHour(0x10, ((string)this.ViewState["SortExpressionThreshold16"]) + ((string)this.ViewState["sortDirectionThreshold16"]));
                    break;
            }
        }
    }


}