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
using System.Text;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using ceTe.DynamicPDF;
using System.Threading;
using ComponentArt.Web.UI;
using System.Globalization;
using System.Collections.Generic;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_IMCCMSSummaryMobileView : BasePage
    {
        static SqlCommand cmd = null;
        static SqlDataAdapter adapter = null;
        static DataTable dt = null;
        static testing clsObj;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            // if (this.DialogCallBackCustomerDetail.CausedCallback)
            //    return;
            if (!IsPostBack)
            {
                //if (!CheckRight((int)Permissions.ViewIMCCMSDashboard, true))
                //    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "total ";
                ViewState["sortDirection"] = "asc ";
                //   BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
        }

      
        private void UpdateCounter(string key)
        {
            if (Session[key] == null)
                Session[key] = 1;
            else
                Session[key] = (int)Session[key] + 1;
        }
        protected void GridView_CashPositions_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
          //  BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        }

        protected void GridView_CashPositions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    Image img = (Image)e.Row.FindControl("img");
            //    string atm_id = this.GridView_CashPositions.DataKeys[e.Row.DataItemIndex].Values["atm_id"].ToString();
            //    Atm atm = Atm.LoadAtmByPk(int.Parse(atm_id));
            //    if (!atm.IsHealthy)
            //        e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
            //    if (atm.MinOperatingBalance == null)
            //    {
            //        img.Attributes.Add("src", "../../images/help.jpg");
            //        img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Minimum operating balance is not defined]");
            //    }
            //    else if (decimal.Parse(e.Row.Cells[18].Text) <= atm.MinOperatingBalance)
            //    {
            //        if (decimal.Parse(e.Row.Cells[18].Text) <= 50000)
            //        {
            //            img.Attributes.Add("src", "../../images/attention_now.jpg");

            //            img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Current balance is less than equal to 50K.Minimum Operating balance (" + atm.MinOperatingBalance + ")]");
            //        }
            //        else
            //        {
            //            img.Attributes.Add("src", "../../images/attention_soon.jpg");

            //            img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Current balance is less then min operating balance.Minimum operating balance (" + atm.MinOperatingBalance + ")]");
            //        }
            //    }
            //    else
            //    {
            //        img.Attributes.Add("src", "../../images/healthy.jpg");

            //        img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[ATM is in healthy state.Minimum operating balance (" + atm.MinOperatingBalance + ")]");
            //    }

            //    if (Request.QueryString["f"] == "1")
            //    {
            //        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            //        cmd.CommandText = "select * from atm_alert where alert_type_id in (46,47,48,49) and resolve_at is null and atm_id = " + atm.ATMId;
            //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //        DataTable dt = new DataTable();
            //        adapter.Fill(dt);

            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            if (int.Parse(dr["alert_type_id"].ToString()) == 46)
            //            {
            //                e.Row.Cells[5].ForeColor = System.Drawing.Color.Yellow;

            //            }
            //            else if (int.Parse(dr["alert_type_id"].ToString()) == 47)
            //            {
            //                e.Row.Cells[7].ForeColor = System.Drawing.Color.Yellow;
            //            }
            //            else if (int.Parse(dr["alert_type_id"].ToString()) == 48)
            //            {
            //                e.Row.Cells[9].ForeColor = System.Drawing.Color.Yellow;
            //            }
            //            else if (int.Parse(dr["alert_type_id"].ToString()) == 49)
            //            {
            //                e.Row.Cells[11].ForeColor = System.Drawing.Color.Yellow;
            //            }

            //        }

            //    }
            //}

        }
        public static string parseValueIntoCurrency(decimal number)
        {
            // set currency format
            string curCulture = Thread.CurrentThread.CurrentCulture.ToString();
            System.Globalization.NumberFormatInfo currencyFormat = new
                System.Globalization.CultureInfo(curCulture).NumberFormat;

            currencyFormat.CurrencyNegativePattern = 1;
            currencyFormat.CurrencySymbol = "";
            return number.ToString("c", currencyFormat);
        }

        protected void GridView_MachinesAvailability_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
        protected void DialogCallBackDetail_Callback(object sender, CallBackEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
            int cssValue = 0;
            try
            {
                StringBuilder builder = new StringBuilder();
                if (e.Parameters[1] != "chartBar")
                {
                    // modalwindowserver.InnerHtml = "<p>adafdfdsfdsfdsfdsfdsfdsfdsf</p>";

                    int val = Convert.ToInt32(e.Parameters[0]);
                    int type = Convert.ToInt32(e.Parameters[1]);
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (type == 0)
                    {
                        cmd.CommandText = "GetTotalAvailabilityDetail";
                        cmd.Parameters.Add("@atm_type", SqlDbType.Int);
                        cmd.Parameters[0].Value = val;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Address</th><th scope=\"col\">Inservice</th><th scope=\"col\">OOS</th><th scope=\"col\">Downtime</th><th scope=\"col\">Supervisor</th><th scope=\"col\">Cash Out</th> <th scope=\"col\">Comm</th><th scope=\"col\">FLM P1</th><th scope=\"col\">Host Down</th><th scope=\"col\">SLM P1</th></tr>");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["address"] + "</td><td>" + dt.Rows[i]["in_service"] + "</td><td>" + dt.Rows[i]["out_of_service"] + "</td><td>" + dt.Rows[i]["total_downtime"] + "</td><td>" + dt.Rows[i]["in_supervisor"] + "</td><td>" + dt.Rows[i]["cash_out"] + "</td><td>" + dt.Rows[i]["comms_failure"] + "</td><td>" + dt.Rows[i]["flm_p1"] + "</td><td>" + dt.Rows[i]["host_down"] + "</td><td>" + dt.Rows[i]["slm_p1"] + "</td></tr>");
                        }
                        builder.Append("</tbody></table>");


                    }
                    else if (type == 1)
                    {
                        cmd.CommandText = "GetDeadAtmsDetails";
                        cmd.Parameters.Add("@type", SqlDbType.Int);
                        cmd.Parameters[0].Value = val;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        builder = new StringBuilder();
                        builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Generated Time</th><th scope=\"col\">Duration</th></tr>");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"] + "</td><td>" + dt.Rows[i]["generated_at"] + "</td><td>" + dt.Rows[i]["hoursdiff"] + "</td></tr>");
                        }
                        builder.Append("</tbody></table>");



                    }
                    else if (type == 2)
                    {
                        cmd.CommandText = "GetNoTransactionMonitoringDetails";
                        cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                        cmd.Parameters[0].Value = val;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i][1].ToString() != "" && dt.Rows[i][5].ToString() != "")
                            {
                                int inactivityCount = GetInactivityPeriodCount((DateTime)dt.Rows[i][1], (int)dt.Rows[i][5]);
                                dt.Rows[i][2] = inactivityCount;
                            }
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.Text;
                            //cmd = null;
                            //cmd = ConnectionFactory.GetNewCommand(false);
                            //-------------commented to remove operational hours------------
                            //                        cmd.CommandText = @"select case operational_day_id when '1' then 'S' when '2' then 'M' when '3' then 'T'
                            //                                when '4' then 'W' when '5' then 'T' when '6' then 'F' when '7' then 'S'
                            //                                 end as day,* from operational_hour where atm_id = " + dt.Rows[i]["atm_id"];
                            //                        adapter = new SqlDataAdapter(cmd);
                            //                        DataTable dtnew = new DataTable();
                            //                        adapter.Fill(dtnew);
                            //                        if (dtnew.Rows.Count > 0)
                            //                        {
                            //                            builder = new StringBuilder();
                            //                            for (int k = 0; k < dtnew.Rows.Count; k++)
                            //                            {
                            //                                builder.Append("(" + dtnew.Rows[k]["operational_hour_from"] + " to " + dtnew.Rows[k]["operational_hour_to"] + "  " + dtnew.Rows[k]["day"] + "  )<br/>");
                            //                            }
                            //                            dt.Rows[i]["operationalHours"] = builder.ToString();
                            //                        }
                            //----------commented to remove operational hours------------
                        }
                        builder = new StringBuilder();
                        builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Last Transaction</th><th scope=\"col\">Occurrence</th><th scope=\"col\">No Transaction Duration</th></tr>");//<th scope=\"col\">Operational Hours</th>
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"] + "</td><td>" + dt.Rows[i]["lasttrxn"] + "</td><td>" + dt.Rows[i]["inactivitycount"] + "</td><td>" + dt.Rows[i]["inactivityduration"] + "</td></tr>");//<td>" + dt.Rows[i]["operationalHours"] + "</td>
                        }
                        builder.Append("</tbody></table>");



                    }
                    else if (type == 3)
                    {
                        cmd.CommandText = "GetCashMonitoringSummaryDetail";
                        //cmd.Parameters.Add("@type", SqlDbType.Int);
                        //cmd.Parameters[0].Value = val;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        builder = new StringBuilder();
                        //builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Comm</th><th scope=\"col\">Host Down</th><th scope=\"col\">FLM</th><th scope=\"col\">Cash Out</th></tr>");
                        //for (int i = 0; i < dt.Rows.Count; i++)
                        //{
                        //    builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["address"] + "</td><td>" + dt.Rows[i]["comms_failure"] + "</td><td>" + dt.Rows[i]["host_down"] + "</td><td>" + dt.Rows[i]["flm_p1"] + "</td><td>" + dt.Rows[i]["cash_out"] + "</td></tr>");
                        //}
                        builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Address</th><th scope=\"col\">Inservice</th><th scope=\"col\">OOS</th><th scope=\"col\">Downtime</th><th scope=\"col\">Supervisor</th><th scope=\"col\">Cash Out</th> <th scope=\"col\">Comm</th><th scope=\"col\">FLM P1</th><th scope=\"col\">Host Down</th><th scope=\"col\">SLM P1</th><th scope=\"col\">Date</th></tr>");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["address"] + "</td><td>" + dt.Rows[i]["in_service"] + "</td><td>" + dt.Rows[i]["out_of_service"] + "</td><td>" + dt.Rows[i]["total_downtime"] + "</td><td>" + dt.Rows[i]["in_supervisor"] + "</td><td>" + dt.Rows[i]["cash_out"] + "</td><td>" + dt.Rows[i]["comms_failure"] + "</td><td>" + dt.Rows[i]["flm_p1"] + "</td><td>" + dt.Rows[i]["host_down"] + "</td><td>" + dt.Rows[i]["slm_p1"] + "</td><td>" + dt.Rows[i]["report_date"] + "</td></tr>");
                        }

                        builder.Append("</tbody></table>");


                    }
                    else if (type == 4)
                    {
                        //cmd = null;
                        //cmd = ConnectionFactory.GetNewCommand(false);
                        cmd.CommandTimeout = 500;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummaryDetail";
                        cmd.Parameters.Add("@ID", SqlDbType.Int);
                        cmd.Parameters[0].Value = val;
                        cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                        cmd.Parameters[1].Value = 67;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        bool isExecuted = false;
                        builder = new StringBuilder();
                        builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Start Time</th><th scope=\"col\">Duration(Hours)</th><th scope=\"col\">Ticket No</th></tr>");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            // string format = "dd/MM/yyyy HH:mm:ss";
                            DateTime tokenValue = DateTime.ParseExact(dt.Rows[i]["token_value"].ToString(), "dd/MM/yyyy HH:mm:ss", null);
                            // DateTime tokenValue = Convert.ToDateTime(dt.Rows[i]["token_value"].ToString());
                            // DateTime tokenValue = DateTime.Now.AddDays(-1);
                            DateTime currentDate = DateTime.Now;
                            // DataTable dtnew =  dt.DefaultView.ToTable(true, "title");
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
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"] + "</td><td>" + dt.Rows[i]["token_value"] + "</td><td>" + dt.Rows[i]["duration"] + "</td><td>" + dt.Rows[i]["TicketNo"] + "</td></tr>");
                            if (isExecuted == true)
                                i++;
                        }

                        builder.Append("</tbody></table>");


                    }
                    else if (type == 5)
                    {
                        cmd.Parameters.Clear();
                        //cmd = null;
                        //cmd = ConnectionFactory.GetNewCommand(false);
                        cmd.CommandTimeout = 500;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummaryDetail";
                        cmd.Parameters.Add("@ID", SqlDbType.Int);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "val " + val);
                        cmd.Parameters[0].Value = val;
                        cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                        cmd.Parameters[1].Value = 68;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "dt count " + dt.Rows.Count);

                        bool isExecuted = false;
                        builder = new StringBuilder();
                        builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Start Time</th><th scope=\"col\">Duration(Hours)</th><th scope=\"col\">Ticket No</th></tr>");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "i val " + i);

                            LogableTask.LogMonoActivityTask("dd", MethodBase.GetCurrentMethod(), TraceLevel.Info, dt.Rows[i]["token_value"].ToString());
                            //  string format = "dd/MM/yyyy hh:mm:ss";
                            DateTime tokenValue = DateTime.ParseExact(dt.Rows[i]["token_value"].ToString(), "dd/MM/yyyy HH:mm:ss", null);

                            //   DateTime d1 = Convert.ToDateTime(dt.Rows[i]["token_value"].ToString());
                            //   DateTime tokenValue = DateTime.Now.AddDays(-1);
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

                            LogableTask.LogMonoActivityTask("dd", MethodBase.GetCurrentMethod(), TraceLevel.Info, "append");
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"] + "</td><td>" + dt.Rows[i]["token_value"] + "</td><td>" + dt.Rows[i]["duration"] + "</td><td>" + dt.Rows[i]["TicketNo"] + "</td></tr>");
                            if (isExecuted == true)
                                i++;
                        }
                        builder.Append("</tbody></table>");


                    }
                    //added on 21 march 2016
                    else if (type == 6)
                    {
                        cmd.CommandText = "GetMinOperatingBalanceDetail";
                        //cmd.Parameters.Add("@type", SqlDbType.Int);
                        //cmd.Parameters[0].Value = val;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        builder = new StringBuilder();

                        builder.Append("     <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Min Threshold</th><th scope=\"col\">Current Cash Position</th><th scope=\"col\">Last Replenishment Date</th><th scope=\"col\">Next Replenishment Date</th></tr>");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"] + "</td><td>" + dt.Rows[i]["min_operating_balance"] + "</td><td>" + dt.Rows[i]["currentBalance"] + "</td><td>" + dt.Rows[i]["rep_datetime"] + "</td><td>" + dt.Rows[i]["next_replenished_at"] + "</td></tr>");
                        }

                        builder.Append("</tbody></table>");
                        // cssValue = 1;

                    }
                    else if (type == 7)
                    {
                        cmd.CommandText = "GetNoReplenishmentsForMorethan10DaysDetail";
                        //cmd.Parameters.Add("@type", SqlDbType.Int);
                        //cmd.Parameters[0].Value = val;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        builder = new StringBuilder();

                        builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th><th scope=\"col\">Location</th><th scope=\"col\">Current Cash Position</th><th scope=\"col\">Last Replenishment Date</th><th scope=\"col\">Next Replenishment Date</th></tr>");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"] + "</td><td>" + dt.Rows[i]["total"] + "</td><td>" + dt.Rows[i]["last_replenished_at"] + "</td><td>" + dt.Rows[i]["next_replenished_at"] + "</td></tr>");
                        }

                        builder.Append("</tbody></table>");


                    }
                    e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>','" + cssValue + "');</script>");

                }
                else
                {
                    string[] splitString = e.Parameters[0].ToString().Split(';');
                    builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">Machine ID</th></tr>");
                    for (int i = 0; i < splitString.Length; i++)
                    {
                        builder.Append("<tr><td>" + splitString[i] + "</td></tr>");
                    }
                    builder.Append("</tbody></table>");
                    e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>','1');</script>");

                }
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("DisplayDetail", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }
        public int GetInactivityPeriodCount(DateTime generationTime, int inactivityPeriod)
        {

            DateTime startTime = generationTime;
            DateTime endTime = DateTime.Now; //DateTime.Now.AddSeconds( 75 );
            TimeSpan span = endTime.Subtract(startTime);
            int hours = (span.Days * 24) + span.Hours;
            hours /= (inactivityPeriod / 3600);


            return hours + 1;
        }

        [System.Web.Services.WebMethod]
        //[ScriptMethod(UseHttpGet = true)]
        public static object Search()
        {
            string jsonString = string.Empty;
            try
            {
                object obj;
                clsObj = new testing();

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTotalAvailability";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@date", SqlDbType.DateTime);
                cmd.Parameters[0].Value = DateTime.Now.AddDays(-1);
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        obj = new { category = item[0], agreed_sla = item[1], average = item[2], atm_type = item[3], cumulative_percent = item[4] };
                        clsObj.getTotalAvailability.Add(obj);
                    }
                }
                cmd.Parameters.Clear();
                cmd.CommandText = "GetCashMonitoringSummary";
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        obj = new { title = item[0], agreed_sla = item[1], cashout = item[2] };
                        clsObj.getCashMonitoringSummary.Add(obj);
                    }
                }
                cmd.CommandText = "NoTransactionMonitoringSummary";
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        obj = new { alert_type_name = item[0], total = item[1], alert_type_id = item[2] };
                        clsObj.getNoTransactionMonitoring.Add(obj);
                    }
                }
                cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummary";
                cmd.Parameters.Add("@alertType", SqlDbType.Int);
                cmd.Parameters[0].Value = 68;
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        obj = new { total = item[0], subject = item[1], incident_id = item[2] };
                        clsObj.getTMDandMachineDownSummary.Add(obj);
                    }
                }
                cmd.Parameters.Clear();
                cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummary";
                cmd.Parameters.Add("@alertType", SqlDbType.Int);
                cmd.Parameters[0].Value = 67;
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        obj = new { total = item[0], subject = item[1], incident_id = item[2] };
                        clsObj.getTMDMonitoringSummary.Add(obj);
                    }
                }
                cmd.Parameters.Clear();
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetNoReplenishmentsForMorethan10Days";
                cmd.CommandType = CommandType.StoredProcedure;
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        obj = new { total = item[0] };
                        clsObj.getNoReplenishmentsForMorethan10Days.Add(obj);
                    }
                }
                cmd.Parameters.Clear();
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetMinOperatingBalance";
                cmd.CommandType = CommandType.StoredProcedure;
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        obj = new { total = item[0] };
                        clsObj.getMinOperatingBalance.Add(obj);
                    }
                }
            }

            catch (Exception ex)
            {

            }
            return clsObj;
        }
        public static string ConvertDataTabletoString(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);

        }
    }
}