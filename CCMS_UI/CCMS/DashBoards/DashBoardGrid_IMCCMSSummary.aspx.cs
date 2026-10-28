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
using System.Text;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using ceTe.DynamicPDF;
using System.Threading;
using ComponentArt.Web.UI;
using System.Globalization;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_IMCCMSSummary : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DialogCallBackCustomerDetail.CausedCallback)
                return;
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewIMCCMSDashboard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "total ";
                ViewState["sortDirection"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
        }

        private void BindGrid(string orderBy)
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = null;
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                this.Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(this.metaRefresh.Content) / 60) + "\");} init();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetCashPositionSummary";

                //cmd.Parameters.Add("@Date", SqlDbType.VarChar);
                //cmd.Parameters.Add("@NoteSetTypeId", SqlDbType.Int);
                //cmd.Parameters.Add("@MinNotesAlertExists", SqlDbType.Int);
                //cmd.Parameters.Add("@OrderBy", SqlDbType.VarChar);
                //cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);

                //cmd.Parameters[0].Value = DateTime.Today.ToString("dd/MM/yyyy");

                //if (Request.QueryString["n"] != null)
                //    cmd.Parameters[1].Value = int.Parse(Request.QueryString["n"]);
                //else
                //    cmd.Parameters[1].Value = 0;

                //if (Request.QueryString["f"] != null)
                //    cmd.Parameters[2].Value = int.Parse(Request.QueryString["f"]);
                //else
                //    cmd.Parameters[2].Value = 0;

                //cmd.Parameters[3].Value = orderBy;
                //cmd.Parameters[4].Value = "";


                //cmd.CommandText = "select top 1000 outerATM.atm_id,outerATM.title title, note_set_type.denomination_type_1, " +
                //    " note_set_type.denomination_type_2, note_set_type.denomination_type_3,note_set_type.denomination_type_4 " +
                //" , note_set_type.denomination_type_5, note_set_type.denomination_type_6, note_set_type.denomination_type_7, " +
                //" (select max(trxn_datetime) from parsed_transaction where atm_id = outerATM.atm_id and amount>0 )last_trxn_at, " +
                //" p.cassette1_notes, p.cassette2_notes, " +
                // " p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes, " +
                //" p.cassette7_notes, note_set_type.denomination_type_1 * p.cassette1_notes +  " +
                //" note_set_type.denomination_type_2 * p.cassette2_notes + note_set_type.denomination_type_3 * p.cassette3_notes  " +
                //" + note_set_type.denomination_type_4 * p.cassette4_notes + note_set_type.denomination_type_5 * p.cassette5_notes + " +
                //" note_set_type.denomination_type_6 * p.cassette6_notes + note_set_type.denomination_type_7 * p.cassette7_notes total ," +
                //" (p.purge_cassette1_notes+p.purge_cassette2_notes+p.purge_cassette3_notes+p.purge_cassette4_notes+p.purge_cassette5_notes+" +
                //" p.purge_cassette6_notes+p.purge_cassette7_notes) purged_counts," +
                //" (p.purge_cassette1_notes*note_set_type.denomination_type_1+p.purge_cassette2_notes*note_set_type.denomination_type_2+" +
                //" p.purge_cassette3_notes*note_set_type.denomination_type_3+p.purge_cassette4_notes*note_set_type.denomination_type_4 " +
                //" +p.purge_cassette5_notes*note_set_type.denomination_type_5+p.purge_cassette6_notes*note_set_type.denomination_type_6 " +
                //" +p.purge_cassette7_notes*note_set_type.denomination_type_7) purged_amount,( select max(rep_datetime) from replenishment where atm_id = outerATM.atm_id )last_replenished_at, " +
                //" ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenished_at,  " +
                //" ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id   and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount from atm outerATM , cash_position p ,note_set_type where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 " +

                //" and outerATM.exclude_dff = 0 and outerATM.note_set_type_id = note_set_type.note_set_type_id " +
                //(Request.QueryString["n"] != null ? " and outerATM.note_set_type_id = " + Request.QueryString["n"] : "") +
                //" and last_trxn_at = (select max(last_trxn_at)from cash_position where cash_position.atm_id = outerATM.atm_id)  order by " + orderBy;

                //commented by shariq for remove cash Position 
                //string condition;
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //DataTable dt = new DataTable();
                //adapter.Fill(dt);
                //DataTable dtnew = dt.Clone();
                //for (int i = 0; i <= 1; i++)
                //{
                //    if (i == 0)
                //        condition = "Offsite";
                //    else
                //        condition = "Onsite";

                //    DataRow[] rows = dt.Select("region_name like '%" + condition + "%'");
                //    DataRow dr = dt.NewRow();

                //    double total = 0;
                //    int type1note = 0;
                //    double type1val = 0;
                //    int type2note = 0;
                //    double type2val = 0;
                //    int type3note = 0;
                //    double type3val = 0;
                //    int type4note = 0;
                //    double type4val = 0;
                //    for (int k = 0; k < rows.Length; k++)
                //    {

                //        total += (Convert.ToDouble(rows[k]["type1_Val_Sum"]) + Convert.ToDouble(rows[k]["type2_Val_Sum"]) + Convert.ToDouble(rows[k]["type3_Val_Sum"]) + Convert.ToDouble(rows[k]["type4_Val_Sum"])) / 1000000;
                //        type1note += Convert.ToInt32(rows[k]["type1_notes_Sum"]);
                //        type1val += Convert.ToDouble(rows[k]["type1_Val_Sum"]) / 1000000;
                //        type2note += Convert.ToInt32(rows[k]["type2_notes_Sum"]);
                //        type2val += Convert.ToDouble(rows[k]["type2_Val_Sum"]) / 1000000;
                //        type3note += Convert.ToInt32(rows[k]["type3_notes_Sum"]);
                //        type3val += Convert.ToDouble(rows[k]["type3_Val_Sum"]) / 1000000;
                //        type4note += Convert.ToInt32(rows[k]["type4_notes_Sum"]);
                //        type4val += Convert.ToDouble(rows[k]["type4_Val_Sum"]) / 1000000;
                //    }
                //    dr["region_name"] = condition + " ATMs";
                //    dr["total"] = total;
                //    dr["type1_notes_Sum"] = type1note;
                //    dr["type1_Val_Sum"] = type1val;
                //    dr["type2_notes_Sum"] = type2note;
                //    dr["type2_Val_Sum"] = type2val;
                //    dr["type3_notes_Sum"] = type3note;
                //    dr["type3_Val_Sum"] = type3val;
                //    dr["type4_notes_Sum"] = type4note;
                //    dr["type4_Val_Sum"] = type4val;
                //    dtnew.Rows.Add(dr.ItemArray);
                //}
                //StringBuilder builder = new StringBuilder();
                //foreach (DataRow item in dtnew.Rows)
                //{
                //    builder.Append("<tr><td style=\"text-align: center;\">" + item.ItemArray[0] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[1] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[2] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[3] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[4] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[5] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[6] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[7] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[8] + "</td>");
                //    builder.Append("<td style=\"text-align: right;\">" + item.ItemArray[9] + "</td></tr>");
                //}
                //builder.Append("<tr><td style=\"text-align: center;font-weight:bold\">Total</td>");
                //builder.Append("<td style=\"text-align: right;font-weight:bold\">" + (Convert.ToDouble(dtnew.Rows[0]["total"]) + Convert.ToDouble(dtnew.Rows[1]["total"])) + "</td>");

                //builder.Append("<td colspan=\"2\" style=\"text-align: center; font-weight:bold\">" + (Convert.ToDouble(dtnew.Rows[0]["type1_Val_Sum"]) + Convert.ToDouble(dtnew.Rows[1]["type1_Val_Sum"])) + "</td>");
                //builder.Append("<td colspan=\"2\" style=\"text-align: center;font-weight:bold\">" + (Convert.ToDouble(dtnew.Rows[0]["type2_Val_Sum"]) + Convert.ToDouble(dtnew.Rows[1]["type2_Val_Sum"])) + "</td>");
                //builder.Append("<td colspan=\"2\" style=\"text-align: center;font-weight:bold\">" + (Convert.ToDouble(dtnew.Rows[0]["type3_Val_Sum"]) + Convert.ToDouble(dtnew.Rows[1]["type3_Val_Sum"])) + "</td>");
                //builder.Append("<td colspan=\"2\" style=\"text-align: center;font-weight:bold\">" + (Convert.ToDouble(dtnew.Rows[0]["type4_Val_Sum"]) + Convert.ToDouble(dtnew.Rows[1]["type4_Val_Sum"])) + "</td></tr>");

                //tableCashPosition.InnerHtml = builder.ToString();
                //******************************************


                cmd.CommandText = "NoTransactionMonitoringSummary";
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);


                GridView_NoTrxnMonitoring.DataSource = dt;
                GridView_NoTrxnMonitoring.DataBind();

                //commented for deadAtm 
                //cmd.CommandText = "GetDeadAtms";
                //adapter = new SqlDataAdapter(cmd);
                //dt = new DataTable();
                //adapter.Fill(dt);
                //GridView_DeadATMs.DataSource = dt;
                //GridView_DeadATMs.DataBind();


                cmd.CommandText = "GetCashMonitoringSummary";
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                GridView_CashMonitoring.DataSource = dt;
                GridView_CashMonitoring.DataBind();


                cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummary";
                cmd.Parameters.Add("@alertType", SqlDbType.Int);
                cmd.Parameters[0].Value = 67;
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);


                GridView_TMDMonitoring.DataSource = dt;
                GridView_TMDMonitoring.DataBind();

                cmd.Parameters.Clear();
                //cmd = null;
                //cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummary";
                cmd.Parameters.Add("@alertType", SqlDbType.Int);
                cmd.Parameters[0].Value = 68;
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                GridView_MachineDown.DataSource = dt;
                GridView_MachineDown.DataBind();


                cmd.Parameters.Clear();
                //cmd = null;
                //cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTotalAvailability";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@date", SqlDbType.DateTime);
                cmd.Parameters[0].Value = DateTime.Now.AddDays(-1);
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                GridView_MachinesAvailability.DataSource = dt;
                GridView_MachinesAvailability.DataBind();

                //added on 21 march 2016
                cmd.Parameters.Clear();

                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetMinOperatingBalance";
                cmd.CommandType = CommandType.StoredProcedure;

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                GridView_MachineMinOperatingBal.DataSource = dt;
                GridView_MachineMinOperatingBal.DataBind();

                cmd.Parameters.Clear();
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetNoReplenishmentsForMorethan10Days";
                cmd.CommandType = CommandType.StoredProcedure;

                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                GridView_MachineNotReplenished.DataSource = dt;
                GridView_MachineNotReplenished.DataBind();

                Literal_AddHeading.Text = "IM CCMS Summary Dashboard";

                //*************added by shariq on 9/2/2016
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("interface1");
                dtGrid.Columns.Add("interface2");
                dtGrid.Columns.Add("interface3");
                dtGrid.Columns.Add("interface4");
                dtGrid.Columns.Add("interface5");
                dtGrid.Columns.Add("sensor1");
                dtGrid.Columns.Add("sensor2");
                dtGrid.Columns.Add("sensor3");
                dtGrid.Columns.Add("sensor4");
                dtGrid.Columns.Add("sensor5");
                dtGrid.Columns.Add("sensor6");
                dtGrid.Columns.Add("sensor7");
                dtGrid.Columns.Add("sensor8");
                dtGrid.Columns.Add("sensor9");
                dtGrid.Columns.Add("sensor10");
                dtGrid.Columns.Add("sensor11");
                dtGrid.Columns.Add("sensor12");
                dtGrid.Columns.Add("ccmsagent_last_reported_heartbeat");
                dtGrid.Columns.Add("ccmsservicemanager_last_reported_heartbeat");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                int idx = 1;
                cmd.CommandText = @"select name from sensor_oid_mapping ";

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //  GridView_PowerMonitoring.Columns[5 + idx].HeaderText = reader[0].ToString();
                    idx++;
                }
                reader.Close();



                // StringBuilder builder = new StringBuilder();
                cmd.CommandText = "select atm_id,title,ccmsagent_last_reported_heartbeat,ccmsservicemanager_last_reported_heartbeat from atm where is_active=1";
                dt = new DataTable();
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);


                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newRow = dtGrid.NewRow();
                    newRow["title"] = dr["title"];
                    cmd.CommandText = @"select sensor_oid_mapping.sensor_oid_mapping_id,name,val 
                                               from sensor_current_status inner join sensor_oid_mapping 
                                               on sensor_oid_mapping.sensor_oid_mapping_id=sensor_current_status.sensor_oid_mapping_id where atm_id = " + dr["atm_id"];

                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        newRow[int.Parse(reader["sensor_oid_mapping_id"].ToString()) + 5] = reader["val"];
                    }
                    reader.Close();
                    idx = 1;
                    cmd.CommandText = @" select interface_friendly_name,ip,interface_status from atm_interface_current_status inner join atm_interface_info
                                               on atm_interface_current_status.atm_interface_info_id =atm_interface_info.atm_interface_info_id
                                               where atm_interface_current_status.atm_id = " + dr["atm_id"];

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        newRow[idx] = reader["interface_friendly_name"] + ";" + reader["ip"] + ";" + reader["interface_status"];
                        idx++;
                    }
                    reader.Close();

                    newRow["ccmsservicemanager_last_reported_heartbeat"] = dr["ccmsservicemanager_last_reported_heartbeat"];
                    newRow["ccmsagent_last_reported_heartbeat"] = dr["ccmsagent_last_reported_heartbeat"];
                    dtGrid.Rows.Add(newRow);
                }
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {

                    string[] interfaceInfo = { dtGrid.Rows[i][1].ToString(), dtGrid.Rows[i][2].ToString(), dtGrid.Rows[i][3].ToString(), dtGrid.Rows[i][4].ToString(), dtGrid.Rows[i][5].ToString() };
                    for (int j = 0; j < interfaceInfo.Length; j++)
                    {
                        if (interfaceInfo[j] != "")
                        {
                            string[] parts = interfaceInfo[j].Split(';');

                            if (parts[2] == "0")
                            {
                                UpdateCounter(j.ToString());
                                ViewState[j.ToString()] += dtGrid.Rows[i][0].ToString() + ";";
                            }
                            else
                            {
                                UpdateCounter("u" + j.ToString());
                            }
                        }
                    }
                    //if (dtGrid.Rows[i][13].ToString() != "")
                    //{
                    //    if (int.Parse(dtGrid.Rows[i][13].ToString()) < 5)
                    //    {
                    //        UpdateCounter("powerDown");
                    //    }
                    //    else
                    //    {
                    //        UpdateCounter("powerUp");
                    //    }
                    //}


                    if (i == dtGrid.Rows.Count - 1)
                    {
                        //Generating pie chart.
                        StringBuilder builder1 = new StringBuilder();
                        int powerUp = -1;
                        int powerDown = -1;
                        double perc = 0;
                        if (Session["powerUp"] != null)
                            powerUp = (int)Session["powerUp"];

                        if (Session["powerDown"] != null)
                            powerDown = (int)Session["powerDown"];

                        if (powerDown <= 0)
                        {
                            perc = 100;

                        }
                        else if (powerUp <= 0)
                        {
                            perc = 0;

                        }
                        else
                        {
                            perc = Math.Round((double)powerUp / (powerUp + powerDown) * 100, 2);

                        }


                        ////Generating bar chart.
                        GenerateProgressBarContent("0", ref builder1, "ATM IP");

                        GenerateProgressBarContent("1", ref builder1, "Gateway");

                        GenerateProgressBarContent("2", ref builder1, "Router");

                        GenerateProgressBarContent("3", ref builder1, "Branch");
                        GenerateProgressBarContent("4", ref builder1, "DVR");

                        ChartBar.Titles[0].Text = "Interface Summary";
                        ChartBar.Series["Default"]["ShowMarkerLines"] = "true";
                        ChartBar.Series["Default"]["DrawingStyle"] = "Cylinder";
                        ChartBar.Series["Default"]["PointWidth"] = "0.8";

                        string[] colorArr = { "#d8422D", "#ffba04", "#1d71ef", "#01A05b", "#d8422D" };
                        //for (int k = 0; k < ChartBar.Series["Default"].Points.Count; k++)
                        for (int k = 0; k < 5; k++)
                        {
                            if (ViewState[k.ToString()] != null)
                            {
                                ChartBar.Series["Default"].Points[k].Color = System.Drawing.ColorTranslator.FromHtml(colorArr[k % 5]);
                                ChartBar.Series["Default"].Points[k].Url = "javascript:modalWindow('" + ViewState[k.ToString()] + "','chartBar');";
                            }
                            // ChartBar.Series["Default"].Points[k].ToolTip = "sdfd";
                        }
                        Session["powerUp"] = null;
                        Session["powerDown"] = null;
                        Session["0"] = null;
                        Session["1"] = null;
                        Session["2"] = null;
                        Session["3"] = null;
                        Session["4"] = null;
                        Session["u0"] = null;
                        Session["u1"] = null;
                        Session["u2"] = null;
                        Session["u3"] = null;
                        Session["u4"] = null;
                    }
                }
                //*************added by shariq on 9/2/2016

            }


            //            try
            //            {
            //                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
            //                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

            //                cmd = ConnectionFactory.GetNewCommand(false);
            //                cmd.CommandTimeout = 500;
            //                //                cmd.CommandText = @"select top 1000 atm.title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
            //                //                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
            //                //                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
            //                //                                    p.last_trxn_at, p.cassette1_notes, p.cassette2_notes,
            //                //                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
            //                //                                    p.cassette7_notes, task.last_invoked ,
            //                //note_set_type.denomination_type_1 * p.cassette1_notes + 
            //                //note_set_type.denomination_type_2 * p.cassette2_notes + 
            //                //note_set_type.denomination_type_3 * p.cassette3_notes + 
            //                //note_set_type.denomination_type_4 * p.cassette4_notes + 
            //                //note_set_type.denomination_type_5 * p.cassette5_notes + 
            //                //note_set_type.denomination_type_6 * p.cassette6_notes + 
            //                //note_set_type.denomination_type_7 * p.cassette7_notes total 
            //                //                                    from atm inner join cash_position p on atm.ATM_id = p.atm_id inner join task on p.task_id = task.task_id
            //                //                                    inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id where atm.IS_ACTIVE = 1 order by atm.title";


            //                cmd.CommandText = @"select top 1000 outerATM.atm_id,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
            //                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
            //                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
            //                                    p.last_trxn_at, p.cassette1_notes, p.cassette2_notes,
            //                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
            //                                    p.cassette7_notes, task.last_invoked ,
            //note_set_type.denomination_type_1 * p.cassette1_notes + 
            //note_set_type.denomination_type_2 * p.cassette2_notes + 
            //note_set_type.denomination_type_3 * p.cassette3_notes + 
            //note_set_type.denomination_type_4 * p.cassette4_notes + 
            //note_set_type.denomination_type_5 * p.cassette5_notes + 
            //note_set_type.denomination_type_6 * p.cassette6_notes + 
            //note_set_type.denomination_type_7 * p.cassette7_notes total ,
            //(p.purge_cassette1_notes+p.purge_cassette2_notes+p.purge_cassette3_notes+p.purge_cassette4_notes+p.purge_cassette5_notes+
            //p.purge_cassette6_notes+p.purge_cassette7_notes) purged_counts,
            //
            //(p.purge_cassette1_notes*note_set_type.denomination_type_1+p.purge_cassette2_notes*note_set_type.denomination_type_2+
            //p.purge_cassette3_notes*note_set_type.denomination_type_3+p.purge_cassette4_notes*note_set_type.denomination_type_4
            //+p.purge_cassette5_notes*note_set_type.denomination_type_5+p.purge_cassette6_notes*note_set_type.denomination_type_6
            //+p.purge_cassette7_notes*note_set_type.denomination_type_7) purged_amount,
            //( select max(rep_datetime) from replenishment where atm_id = outerATM.atm_id )last_replenished_at,
            //
            //( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    
            //and cash_order_datetime>=convert(datetime,'"+DateTime.Today.ToString("dd/MM/yyyy")+"',103))next_replenished_at, " +

            //" ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
            //" and cash_order_datetime>=convert(datetime,'"+DateTime.Today.ToString("dd/MM/yyyy")+"',103))amount"+

            //" from atm outerATM , cash_position p ,task,note_set_type " +
            //"where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 and outerATM.exclude_dff = 0 " +
            //"and p.task_id = task.task_id " +
            //"and outerATM.note_set_type_id = note_set_type.note_set_type_id " +
            //"and last_trxn_at = (select max(last_trxn_at)" +
            //"from cash_position " +
            //"where cash_position.atm_id = outerATM.atm_id)  order by " + orderBy;

            //                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //                DataTable dt = new DataTable();
            //                adapter.Fill(dt);

            //                GridView_CashPositions.DataSource = dt;
            //                GridView_CashPositions.DataBind();
            //            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
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
        private void GenerateProgressBarContent(string key, ref StringBuilder builder, string friendlyName)
        {
            string progressBarClass = "";
            double val = 0;
            int interfaceUp = -1;
            int interfaceDown = -1;

            if (Session[key] != null)
            {
                interfaceDown = (int)Session[key];
                ChartBar.Series["Default"].Points.AddXY(friendlyName, interfaceDown);
            }
            else
                ChartBar.Series["Default"].Points.AddXY(friendlyName, 0);

            if (Session["u" + key] != null)
                interfaceUp = (int)Session["u" + key];

            if (interfaceDown == -1 && interfaceUp == -1)
                val = 0;
            else if (interfaceDown == -1)
                val = 0;
            else if (interfaceUp == -1)
                val = 100;
            else
                val = Math.Round((double)interfaceDown / (interfaceDown + interfaceUp) * 100, 2);

            if (val > 50)
                progressBarClass = "progress-bar-danger";
            else if (val < 25)
                progressBarClass = "progress-bar-success";
            else
                progressBarClass = "progress-bar-warning";

            builder.Append(" <b>" + friendlyName + " Down %</b> " +
         " <div class=\"progress\"> " +
         "  <div class=\"progress-bar " + progressBarClass + "\" role=\"progressbar\" aria-valuenow=\"100\" " +
         " aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: " + val + "%;\">" +
         "    <span style=\"color: black; font-weight:bold\">" + val + "%</span></div>" +
          " </div> ");


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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

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
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"].ToString().Replace("'","") + "</td><td>" + dt.Rows[i]["generated_at"] + "</td><td>" + dt.Rows[i]["hoursdiff"] + "</td></tr>");
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
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"].ToString().Replace("'", "") + "</td><td>" + dt.Rows[i]["lasttrxn"] + "</td><td>" + dt.Rows[i]["inactivitycount"] + "</td><td>" + dt.Rows[i]["inactivityduration"] + "</td></tr>");//<td>" + dt.Rows[i]["operationalHours"] + "</td>
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
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"].ToString().Replace("'", "") + "</td><td>" + dt.Rows[i]["token_value"] + "</td><td>" + dt.Rows[i]["duration"] + "</td><td>" + dt.Rows[i]["TicketNo"] + "</td></tr>");
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
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"].ToString().Replace("'", "") + "</td><td>" + dt.Rows[i]["token_value"] + "</td><td>" + dt.Rows[i]["duration"] + "</td><td>" + dt.Rows[i]["TicketNo"] + "</td></tr>");
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
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"].ToString().Replace("'", "") + "</td><td>" + dt.Rows[i]["min_operating_balance"] + "</td><td>" + dt.Rows[i]["currentBalance"] + "</td><td>" + dt.Rows[i]["rep_datetime"] + "</td><td>" + dt.Rows[i]["next_replenished_at"] + "</td></tr>");
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
                            builder.Append("<tr><td>" + dt.Rows[i]["title"] + "</td><td>" + dt.Rows[i]["location"].ToString().Replace("'", "") + "</td><td>" + dt.Rows[i]["total"] + "</td><td>" + dt.Rows[i]["last_replenished_at"] + "</td><td>" + dt.Rows[i]["next_replenished_at"] + "</td></tr>");
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


    }

}