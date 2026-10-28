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
    public partial class DashBoard_Summary : BasePage
    {
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
               + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and ATM_ID_NO IN (SELECT TITLE FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")); select COUNT(flm_monitoring_id) from flm_monitoring  where time_of_call_to_bank is null  and activity_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and activity_datetime<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and ATM_ID_NO IN (SELECT TITLE FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "));  select COUNT(flm_monitoring_id) from flm_monitoring  where time_of_solving is null and activity_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and activity_datetime<=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and ATM_ID_NO IN (SELECT TITLE FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "));  select max(activity_datetime) from flm_monitoring WHERE ATM_ID_NO IN (SELECT TITLE FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "))";
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
                cmd.CommandText = "select title,atm_id from atm where is_dff_generation_halt =1 and is_active = 1 and atm_id in (select atm_id from user_atms where user_id=" + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId + ")";
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

            int resolvedCount = 0;
            int unResolvedCount = 0;
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


        private string GetATMsByNoteSetTypeID(string id)
        {
            StringBuilder builder = new StringBuilder();
            Atm.AtmReader reader = Atm.ExecuteReader("note_set_type_id = " + id);
            while (reader.Read())
                builder.Append(reader.CurrentAtm.ATMId + ",");
            builder.Append("-1");
            reader.Close();
            return builder.ToString();
        }


        private void GenerateRemainingNotesGraph()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("DrawGraph");
            try
            {
                Chart3.Series[0].Points.Clear();
                decimal totalRemaining1 = 0M;
                decimal totalRemaining2 = 0M;
                decimal totalRemaining3 = 0M;
                decimal totalRemaining4 = 0M;
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 600;
                if (CheckBox_AnalysisOnEJ.Checked)
                    cmd.CommandText = "GetEJRemainingNotes";
                else
                    cmd.CommandText = "GetRemainingNotes";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("ForDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today;

                cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                cmd.Parameters[1].Value = GetATMsByNoteSetTypeID(DropDownList_NoteSetType.SelectedItem.Value);

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Remaining Notes Procedure");
                SqlDataReader reader = cmd.ExecuteReader();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Procedure executed");
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value && reader[4] != DBNull.Value)
                        totalRemaining1 += decimal.Parse(reader[0].ToString()) - decimal.Parse(reader[4].ToString());
                    if (reader[1] != DBNull.Value && reader[5] != DBNull.Value)
                        totalRemaining2 += decimal.Parse(reader[1].ToString()) - decimal.Parse(reader[5].ToString());
                    if (reader[2] != DBNull.Value && reader[6] != DBNull.Value)
                        totalRemaining3 += decimal.Parse(reader[2].ToString()) - decimal.Parse(reader[6].ToString());
                    if (reader[3] != DBNull.Value && reader[7] != DBNull.Value)
                        totalRemaining4 += decimal.Parse(reader[3].ToString()) - decimal.Parse(reader[7].ToString());
                }
                reader.Close();
                Label_QueryAt.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

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
                if ((cmd != null) && (cmd.Connection != null))
                {
                    cmd.Connection.Close();
                }
                task.EndTask();
            }

        }
        private void DrawGraph()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("DrawGraph");

            try
            {
                this.Chart2.Series[0].Points.Clear();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Draw Pie");
                this.DrawPie();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Pie created");
                //decimal totalRemaining1 = 0M;
                //decimal totalRemaining2 = 0M;
                //decimal totalRemaining3 = 0M;
                //decimal totalRemaining4 = 0M;
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 600;
                //cmd.CommandText = "GetRemainingNotes";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Add(new SqlParameter("ForDate", SqlDbType.DateTime));
                //cmd.Parameters[0].Value = DateTime.Today;

                //cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
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
                GenerateRemainingNotesGraph();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute Trxn Count Procedure For Current Day");
                cmd.CommandText = "GetTrxnCount";
                cmd.CommandType = CommandType.StoredProcedure;
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
                cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Execute GetFlmSummaryByCode");
                SqlDataReader reader = cmd.ExecuteReader();
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
            Chart_BNA.Series["Default"].Points.Clear();
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
                               "  and atm.is_active=1 and user_ATMs.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId +
                                " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ";

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

            cmd.CommandText = "select title from atm where is_active=1 and is_cdm=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ";
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
            Chart_ChequeDeposit.Series["Default"].Points.Clear();
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
                               "  and atm.is_active=1  " +
                               " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                                " and user_ATMs.user_id = " + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);



            cmd.CommandText = "select title from atm where is_active=1 and is_ccdm=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ";
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
            dr[1] = (int)ConnectionFactory.ExecuteScalar("select count(cash_order_monitoring_id) from cash_order_monitoring where cash_order_datetime=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and current_order_executed_at is null and ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") ");
            dtTasks.Rows.Add(dr);

            dr = dtTasks.NewRow();
            dr[0] = "Missing Settlements";
            dr[1] = (int)ConnectionFactory.ExecuteScalar("select count(atm_settlement_id) from atm_settlement where rep_datetime=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and GL_NO IN (SELECT GL_NUMBER FROM ATM WHERE ATM_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "))");
            dtTasks.Rows.Add(dr);

            GridView_CITTasks.DataSource = dtTasks;
            GridView_CITTasks.DataBind();


        }

        private void DrawPie()
        {
            this.Chart1.Series[0].Points.Clear();
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
                cmd.CommandText = "select distinct(title) from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.ATMCassetteFaulty + " and resolve_at is null and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
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
                cmd.CommandText = "select title from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id in (" + (int)EnumAlertType.ATMInactivityPeriodElapsed + "," + (int)EnumAlertType.ATMEJInactivityPeriodElapsed + ") and resolve_at is null and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
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
                cmd.CommandText = "select title from atm_alert,atm where atm.is_active = 1 and atm_alert.atm_id = atm.atm_id and alert_type_id=" + (int)EnumAlertType.CassetteDispensingFailed + " and resolve_at is null and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
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
                cmd.CommandText = "select count(title) from atm where is_active=1 and is_atm=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                totalATMs = (int)cmd.ExecuteScalar();

                cmd.CommandText = "select distinct(title) from parsed_transaction,atm where atm.is_active = 1 and parsed_transaction.atm_id = atm.atm_id and trxn_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and trxn_datetime<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59',103) and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                builder = new StringBuilder();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!dtDispensing.Rows.Contains(reader[0]) && !dtInactivity.Rows.Contains(reader[0]) && !dtFaulty.Rows.Contains(reader[0]))
                    {
                        builder.Append(reader[0] + ",");
                        transactingCount++;
                    }
                }
                reader.Close();
                //Doing for EJ
                cmd.CommandText = "select distinct(title) from ej_parsed_transactions,atm where atm.is_active = 1 and ej_parsed_transactions.atm_id = atm.atm_id and trxn_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and trxn_datetime<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59',103) and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
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

       

        protected void DrawDFFInfoGraph()
        {
            LogableTask task = LogableTask.NewTask("DrawDFFInfoGraph");
            try
            {
                Label_org.Text = "";
                //int orgID = 0;
                this.Chart_DFFStatus.Series["Default"].Points.Clear();
                StringBuilder builderNoData = new StringBuilder();
                StringBuilder builderSending2 = new StringBuilder();
                StringBuilder builderSendingSameBalance = new StringBuilder();
                StringBuilder builderInconsistentData = new StringBuilder();
                int atmSending2 = 0;
                int atmSendingSameBalance = 0;
                int atmInconsistentData = 0;
                int atmSendingNoData = 0;

                //if (Session["selectedOrgID"] != null)
                //    orgID = int.Parse(Session["selectedOrgID"].ToString());

                //Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegion("is_organization=1 and region_id<> 1 order by region_id");
                UserOrganization.UserOrganizationReader userOrgReader = UserOrganization.ExecuteReader("user_id = " + this.appUser.UserId);
                StringBuilder builderOrg = new StringBuilder();
                while (userOrgReader.Read())
                {

                    Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegion("mcn = '" + userOrgReader.CurrentUserOrganization.OrgMcn + "'");
                    if (region == null) continue;
                    if (Label_org.Text.Length > 0)
                        Label_org.Text += ",";
                    Label_org.Text += region.RegionName;
                    builderOrg.Append(region.RegionId + ",");
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


                    List<string> atmList = new List<string>();
                    string atmID = null;
                    string replenishmentAmount = null;
                    string returnAmount = null;
                    string withdrawalAmount = null;
                    string state = null;
                    string closingBalance = null;
                    for (int i = 1; i < (content.Length - 1); i++)
                    {
                        try
                        {
                            atmID = content[i].Substring(8, 8);
                            atmList.Add(atmID);
                            replenishmentAmount = content[i].Substring(0x10, 9);
                            returnAmount = content[i].Substring(0x19, 9);
                            withdrawalAmount = content[i].Substring(0x22, 9);
                            state = content[i].Substring(0x3d, 1);
                            closingBalance = content[i].Substring(0x34, 9);

                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info,
                                string.Format("atmID={0},Rep={1},Returns={2},Withdrawals={3},state={4},bal={5}",
                                                atmID, replenishmentAmount, returnAmount, withdrawalAmount, state, closingBalance));

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
                        catch (Exception ex)
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
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
                }
                builderOrg.Append("-1");
                Session["org"] = builderOrg.ToString();

                userOrgReader.Close();

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

      

        protected void GetRecentActivityHTML(int day)
        {
            LogableTask task = LogableTask.NewTask("GetRecentActivityHTML");
            try
            {
                SqlCommand cmd = null;
                DataTable dt = new DataTable();
                cmd = ConnectionFactory.GetNewConnection().CreateCommand();
                cmd.CommandText = string.Format("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;select dbo.GetStatusFriendlyName(status) Status, count(1) Count, task_type.task_type_name Task from task, task_type, user_atms where task.task_type_id = task_type.task_type_id and user_atms.atm_id = task.atm_id and user_atms.user_id = {1} and task.creation_time > convert(datetime, '{0}', 120) and task.creation_time < convert(datetime, '{0} 23:59:59', 120) group by status, task_type.task_type_name \r\n                                                    union\r\n                                                    select dbo.GetFTPStatusFriendlyName(status), count(1) count, task_type.task_type_name from ftp_file_info, task_type, user_organization, region where ftp_file_info.task_type_id = task_type.task_type_id and ftp_file_info.region_id = region.region_id and user_organization.org_mcn = region.mcn and user_organization.user_id = {1} and ftp_file_info.creation_time > convert(datetime, '{0} 00:00:00', 120) and ftp_file_info.creation_time < convert(datetime, '{0} 23:59:59', 120) group by status, task_type.task_type_name order by task_type.task_type_name", DateTime.Today.AddDays((double)day).ToString("yyyy-MM-dd"), ((AppUser)this.Session[SessionVars.user.ToString()]).UserId);
                dt = QueryModifier.TaskQuery.DataTableQueryModify(cmd.CommandText);
                cmd.CommandTimeout = 600;
                //new SqlDataAdapter(cmd).Fill(dt);
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
                cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
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
                cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];


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
            if (Session["org"] != null)
            {
                DropDownList_NoteSetType.Items.Clear();
                NoteSetType.NoteSetTypeReader reader = NoteSetType.ExecuteReader("region_id in (" + Session["org"] + ")");
                while (reader.Read())
                    DropDownList_NoteSetType.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
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
        private void GetServiceStatus()
        {
            try
            {
                string[] services = { "CurrencyMngServer", "CurrencyParser", "CCMSAlertDispatcher", "CCMSEJParser", "CCMSMonitor", "CCMSOrderDownloader", "CCMSTransactionMonitor", "CurrencyCutOver", "CurrencyReportSchedular" };
                //string domain = System.Configuration.ConfigurationManager.AppSettings["domain"];
                //string username = System.Configuration.ConfigurationManager.AppSettings["username"];
                //string password = System.Configuration.ConfigurationManager.AppSettings["password"]; ;// Encryption.Cryptic.DecryptString(System.Configuration.ConfigurationManager.AppSettings["password"]);
                //SignIn.Impersonate impersonate = new SignIn.Impersonate();
                //if (impersonate.DoLogIn(domain, username, password))
                //{
                //    impersonate.DoImpersonate();

                for (int i = 0; i < services.Length; i++)
                {
                    try
                    {
                        CcmsServices sc = CcmsServices.LoadCcmsServices("name='" + services[i] + "'");

                        if (sc.ServiceStatus == "Running")
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


        private void UpdateHomePage()
        {
            LogableTask task = LogableTask.NewTask("UpdateHomePage");
            try
            {
                if (base.CheckRight((int)Permissions.ViewSummary))
                {
                    int width = int.Parse(this.Session["width"].ToString()) - 100;


                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to execute ReplenishmentsWithoutOrder at " + DateTime.Now.ToString());
                        this.ReplenishmentsWithoutOrder();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "ReplenishmentsWithoutOrder completed at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling GetTop10LowTransactingATM at " + DateTime.Now.ToString());
                        this.GetTop10LowTransactingATM();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed GetTop10LowTransactingATM at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling GetTop10TransactingATM at " + DateTime.Now.ToString());
                        this.GetTop10TransactingATM();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed GetTop10TransactingATM at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Calling DisplayHaltedATMs at " + DateTime.Now.ToString());
                        this.DisplayHaltedATMs();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayHaltedATMs at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplaySummaryOfTodaysAlert at " + DateTime.Now.ToString());
                        this.DisplaySummaryOfTodaysAlert();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplaySummaryOfTodaysAlert at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayUserTask at " + DateTime.Now.ToString());
                        this.DisplayUserTask();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayUserTask at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayCurrentOrder at " + DateTime.Now.ToString());
                        this.DisplayCurrentOrder();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DisplayCurrentOrder at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "calling DisplayCallOutSummary at " + DateTime.Now.ToString());
                        this.DisplayCallOutSummary();
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
                        DrawChequeDepositGraph();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawChequeDepositGraph at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }
                    try
                    {
                        DrawNotesAcceptorGraph();
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
                        DrawDFFInfoGraph();
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
                        this.DrawGraph();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "completed DrawGraph at " + DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    }


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

        protected void Button_Regenerate_Click(object sender, EventArgs e)
        {
            UpdateHomePage();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("SummaryPageLoad");
            try
            {
                if (!this.Page.IsPostBack && !this.DialogCallBack.CausedCallback)
                {
                    UpdateHomePage();
                    GetServiceStatus();
                    metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                    Page.RegisterStartupScript("script", "<script>function initDashboard() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} initDashboard();</script>");
                
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
        protected void DialogCallBack_Callback(object sender, CallBackEventArgs e)
        {
            Exception ex;
            LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
            StringBuilder builder = new StringBuilder();
            SqlCommand cmd = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
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
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();

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

  

        private void ReplenishmentsWithoutOrder()
        {
            LogableTask task = LogableTask.NewTask("ReplenishmentsWithoutOrder");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetRepWithoutOrders";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                cmd.Parameters[0].Value = Session[SessionVars.SelectedATMs.ToString()];
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

             
        
       

      


      
      
       


     
     
      



   
       
     
     


       
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            DrawDFFInfoGraph();
            //button_search_Click(this, null);
        }
        protected void Dropdownlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenerateRemainingNotesGraph();
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
                    DisplayHaltedATMs();
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

     
    

    }


}