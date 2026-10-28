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

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_PowerMonitoring : BasePage
    {
        string[] colorArr = { "#d8422D", "#ffba04", "#1d71ef", "#01A05b", "#d8422D" };
        string friendlyName = System.Web.Configuration.WebConfigurationManager.AppSettings["powerMonitoringAlias"];
        //Variables added by Ali Shah on 6th April, 2016
        //Sorting Column in Communication Dashboard
        string interface1Name = "ATM IP";
        string interface2Name = "Gateway";
        string interface3Name = "Hotline";


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCashPositionsDashBoard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "title ";
                ViewState["sortDirection"] = " asc";
                if (friendlyName.Length > 0)
                    Label_Title.Text = friendlyName + " Dashboard";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
        }

        private void BindGrid(string orderBy)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                // int idx = 0;
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

                //Edited by Ali Shah on 6th April, 2016
                //Column Sorting in Communication Dashboard
                dtGrid.Columns.Add("Interface1Status");
                dtGrid.Columns.Add("Interface2Status");
                dtGrid.Columns.Add("Interface3Status");

                this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                this.Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(this.metaRefresh.Content) / 60) + "\");} init();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = "GetCashPositions";
                //cmd.Parameters.Add("@Date", SqlDbType.VarChar);
                //cmd.Parameters.Add("@NoteSetTypeId", SqlDbType.Int);
                //cmd.Parameters.Add("@OrderBy", SqlDbType.VarChar);
                //cmd.Parameters[0].Value = DateTime.Today.ToString("dd/MM/yyyy");

                //if (Request.QueryString["n"] != null)
                //    cmd.Parameters[1].Value = int.Parse(Request.QueryString["n"]);
                //else
                //    cmd.Parameters[1].Value = 0;

                //cmd.Parameters[2].Value = orderBy;


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
                int idx = 1;
                cmd.CommandText = @"select name from sensor_oid_mapping ";

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GridView_PowerMonitoring.Columns[5 + idx].HeaderText = reader[0].ToString();
                    idx++;
                }
                reader.Close();



                StringBuilder builder = new StringBuilder();
                cmd.CommandText = "select atm_id,title,ccmsagent_last_reported_heartbeat,ccmsservicemanager_last_reported_heartbeat from atm where is_active=1";
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
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
                        //Edited by Ali Shah on 6th April, 2016
                        //Sorting Column in Communicatin Dashboard
                        if (reader["interface_friendly_name"].ToString().Contains(interface2Name)) newRow["Interface2Status"] = reader["interface_status"];
                        else if (reader["interface_friendly_name"].ToString().Contains(interface3Name)) newRow["Interface3Status"] = reader["interface_status"];
                        else newRow["Interface1Status"] = reader["interface_status"];
                        idx++;
                    }
                    reader.Close();

                    newRow["ccmsservicemanager_last_reported_heartbeat"] = dr["ccmsservicemanager_last_reported_heartbeat"];
                    newRow["ccmsagent_last_reported_heartbeat"] = dr["ccmsagent_last_reported_heartbeat"];
                    dtGrid.Rows.Add(newRow);
                }

                if (friendlyName.Length > 0)
                {
                    Literal_AddHeading.Text = "NCR " + friendlyName + " DashBoard - Total No.Of ATMs : " + dtGrid.Rows.Count;
                    GridView_PowerMonitoring.Columns[2].HeaderText = "Router";
                    GridView_PowerMonitoring.Columns[3].HeaderText = "Hotline";
                }
                else
                    Literal_AddHeading.Text = "NCR Power Monitoring DashBoard - Total No.Of ATMs : " + dtGrid.Rows.Count;

                //
                //this.GridView_PowerMonitoring.DataSource = dtGrid;
                DataView dv = dtGrid.DefaultView;
                dv.Sort = orderBy;
                this.GridView_PowerMonitoring.DataSource = dv.ToTable();

                this.GridView_PowerMonitoring.DataBind();
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

//                GridView_PowerMonitoring.DataSource = dt;
            //                GridView_PowerMonitoring.DataBind();
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
                        cmd.Connection.Close();

                task.EndTask();
            }
        }
        protected void GridView_PowerMonitoring_Sorting(object sender, GridViewSortEventArgs e)
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

        private void UpdateCounter(string key)
        {
            if (Session[key] == null)
                Session[key] = 1;
            else
                Session[key] = (int)Session[key] + 1;
        }
        protected void GridView_PowerMonitoring_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string[] interfaceInfo = { e.Row.Cells[1].Text, e.Row.Cells[2].Text, e.Row.Cells[3].Text, e.Row.Cells[4].Text, e.Row.Cells[5].Text };
                for (int i = 0; i < interfaceInfo.Length; i++)
                {
                    if (interfaceInfo[i] != "&nbsp;")
                    {
                        string[] parts = interfaceInfo[i].Split(';');

                        if (parts[2] == "0")
                        {
                            //e.Row.Cells[i + 1].BackColor = System.Drawing.Color.Red;
                            e.Row.Cells[i + 1].Text = "<img src='../../images/interfaceNotWorking.png' width='30px' height='30px'/>" + parts[1];
                            UpdateCounter(i.ToString());
                        }
                        else
                        {
                            //    e.Row.Cells[i + 1].BackColor = System.Drawing.Color.Green;
                            e.Row.Cells[i + 1].Text = "<img src='../../images/interfaceWorking.png' width='30px' height='30px'/>" + parts[1];
                            UpdateCounter("u" + i.ToString());
                        }
                    }
                }
                if (e.Row.Cells[13].Text != "&nbsp;")
                {
                    if (int.Parse(e.Row.Cells[13].Text) < 5)
                    {
                        //e.Row.Cells[12].BackColor = System.Drawing.Color.Red;
                        e.Row.Cells[13].Text = "<img src='../../images/powerdown.png' width='30px' height='30px'/>";
                        UpdateCounter("powerDown");
                    }
                    else
                    {
                        //e.Row.Cells[12].BackColor = System.Drawing.Color.Green;
                        e.Row.Cells[13].Text = "<img src='../../images/Powerup.jpg' width='30px' height='30px'/>";
                        UpdateCounter("powerUp");
                    }
                }

                // Image img = (Image)e.Row.FindControl("img");
                //string atm_id = this.GridView_PowerMonitoring.DataKeys[e.Row.DataItemIndex].Values["atm_id"].ToString();
                // Atm atm = Atm.LoadAtmByPk(int.Parse(atm_id));
                //if (!atm.IsHealthy)
                //    e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                //if (atm.MinOperatingBalance == null)
                //{
                //    img.Attributes.Add("src", "../../images/help.jpg");
                //    img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Minimum operating balance is not defined]");
                //}
                //else if (decimal.Parse(e.Row.Cells[18].Text) <= atm.MinOperatingBalance)
                //{
                //    if (decimal.Parse(e.Row.Cells[18].Text) <= 50000)
                //    {
                //        img.Attributes.Add("src", "../../images/attention_now.jpg");

                //        img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Current balance is less than equal to 50K.Minimum Operating balance (" + atm.MinOperatingBalance + ")]");
                //    }
                //    else
                //    {
                //        img.Attributes.Add("src", "../../images/attention_soon.jpg");

                //        img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Current balance is less then min operating balance.Minimum operating balance (" + atm.MinOperatingBalance + ")]");
                //    }
                //}
                //else
                //{
                //    img.Attributes.Add("src", "../../images/healthy.jpg");

                //    img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[ATM is in healthy state.Minimum operating balance (" + atm.MinOperatingBalance + ")]");
                //}


            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                //Generating pie chart.
                StringBuilder builder = new StringBuilder();
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
                    ChartPie.Series["Default"].Points.AddXY("PowerDown", perc);
                    ChartPie.Series["Default"].Points.AddXY("PowerUp", 100 - perc);
                    ChartPie.Series["Default"].Points[1].Color = System.Drawing.Color.Green;
                        ChartPie.Series["Default"].Points[0].Color = System.Drawing.Color.Red;

                }
                else if (powerUp <= 0)
                {
                    perc = 0;
                    ChartPie.Series["Default"].Points.AddXY("PowerUp", perc);
                    ChartPie.Series["Default"].Points.AddXY("PowerDown", 100 - perc);
                    ChartPie.Series["Default"].Points[0].Color = System.Drawing.Color.Green;
                        ChartPie.Series["Default"].Points[1].Color = System.Drawing.Color.Red;


                }
                else
                {
                    perc = Math.Round((double)powerUp / (powerUp + powerDown) * 100, 2);
                    ChartPie.Series["Default"].Points.AddXY("PowerUp", perc);
                    ChartPie.Series["Default"].Points.AddXY("PowerDown", 100 - perc);
                    ChartPie.Series["Default"].Points[0].Color = System.Drawing.Color.Green;
                        ChartPie.Series["Default"].Points[1].Color = System.Drawing.Color.Red;


                }
                //ChartPie.Series["Default"].Points.AddXY("PowerUp", perc);
                //ChartPie.Series["Default"].Points.AddXY("PowerDown", 100 - perc);
                //ChartPie.Series["Default"].Points[0].Color = System.Drawing.Color.Green;
                //ChartPie.Series["Default"].Points[1].Color = System.Drawing.Color.Red;

                ChartPie.Series["Default"]["PieLabelStyle"] = "Outside";
                ChartPie.Titles[0].Text = "Power Summary";
                ChartPie.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                ChartPie.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ChartPie.Legends[0].Enabled = true;
                //Generating bar chart.
                if (friendlyName.Length > 0)
                    ChartPie.Visible = false;
                GenerateProgressBarContent("0", ref builder, "ATM IP");
                //if (Session["0"] != null)
                //{
                //    interface1Down = (int)Session["0"];
                //    ChartBar.Series["Default"].Points.AddXY("Interface1", interface1Down);
                //}

                //if (Session["u0"] != null)
                //    interface1Up = (int)Session["u0"];

                //if (interface1Down == -1 && interface1Up == -1)
                //    val = 0;
                //else if (interface1Down == -1)
                //    val = 0;
                //else if (interface1Up == -1)
                //    val = 100;
                //else
                //    val = Math.Round((double)interface1Down / (interface1Down + interface1Up) * 100, 2);

                //if (val > 50)
                //    progressBarClass = "progress-bar-danger";
                //else
                //    progressBarClass = "progress-bar-success";

                //    builder.Append(" <b>Interface 1 Down %</b> " +
                // " <div class=\"progress\"> " +
                // "  <div class=\"progress-bar " + progressBarClass + "\" role=\"progressbar\" aria-valuenow=\"100\" " +
                // " aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: " + val + "%;\">" +
                // "    <span style=\"color: black; font-weight:bold\">" + val + "%</span></div>" +
                //  " </div> ");
                if (friendlyName.Length > 0)
                {
                    GenerateProgressBarContent("1", ref builder, "Router");
                    GenerateProgressBarContent("2", ref builder, "Hotline");
                }
                else
                {
                    GenerateProgressBarContent("1", ref builder, "Gateway");
                    GenerateProgressBarContent("2", ref builder, "Router");
                }
                //   if (Session["2"] != null)
                //   {
                //       interface3Down = (int)Session["2"];
                //       ChartBar.Series["Default"].Points.AddXY("Interface3", interface3Down);
                //   }
                //   if (Session["u2"] != null)
                //       interface3Up = (int)Session["u2"];

                //   if (interface3Down == -1 && interface3Up == -1)
                //       val = 0;
                //   else if (interface3Down == -1)
                //       val = 0;
                //   else if (interface3Up == -1)
                //       val = 100;
                //   else
                //       val = Math.Round((double)interface3Down / (interface3Down + interface3Up) * 100, 2);

                //   if (val > 50)
                //       progressBarClass = "progress-bar-danger";
                //   else
                //       progressBarClass = "progress-bar-success";

                //   builder.Append(" <b>Interface 3 Down %</b> " +
                //" <div class=\"progress\"> " +
                //"  <div class=\"progress-bar " + progressBarClass + "\" role=\"progressbar\" aria-valuenow=\"100\" " +
                //" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: " + val + "%;\">" +
                //"    <span style=\"color: black; font-weight:bold\">" + val + "%</span></div>" +
                // " </div> ");
                GenerateProgressBarContent("3", ref builder, "Branch");
                GenerateProgressBarContent("4", ref builder, "DVR");


                //   if (Session["3"] != null)
                //   {
                //       interface4Down = (int)Session["3"];
                //       ChartBar.Series["Default"].Points.AddXY("Interface4", interface4Down);
                //   }
                //   if (Session["u3"] != null)
                //       interface4Up = (int)Session["u3"];

                //   if (interface4Down == -1 && interface4Up == -1)
                //       val = 0;
                //   else if (interface4Down == -1)
                //       val = 0;
                //   else if (interface4Up == -1)
                //       val = 100;
                //   else
                //       val = Math.Round((double)interface4Down / (interface4Down + interface4Up) * 100, 2);

                //   if (val > 50)
                //       progressBarClass = "progress-bar-danger";
                //   else
                //       progressBarClass = "progress-bar-success";

                //   builder.Append(" <b>Interface 4 Down %</b> " +
                //" <div class=\"progress\"> " +
                //"  <div class=\"progress-bar " + progressBarClass + "\" role=\"progressbar\" aria-valuenow=\"100\" " +
                //" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: " + val + "%;\">" +
                //"    <span style=\"color: black; font-weight:bold\">" + val + "%</span></div>" +
                // " </div> ");


                ChartBar.Titles[0].Text = "Interface Summary";
                ChartBar.Series["Default"]["ShowMarkerLines"] = "true";
                ChartBar.Series["Default"]["DrawingStyle"] = "Cylinder";
                ChartBar.Series["Default"]["PointWidth"] = "0.8";


                for (int i = 0; i < ChartBar.Series["Default"].Points.Count; i++)
                    ChartBar.Series["Default"].Points[i].Color = System.Drawing.ColorTranslator.FromHtml(colorArr[i % 5]);


                Literal_progressBar.Text = builder.ToString();

                //Generating progress bar.


                //<div class="progress">
                //    <div class="progress-bar progress-bar-success" role="progressbar" aria-valuenow="60"
                //        aria-valuemin="10" aria-valuemax="100" style="width: 100%;">
                //        <span style="color: black;font-weight:bold">100%</span></div>
                //</div>
                //<b>Interface 3 Down %</b>
                //<div class="progress">
                //    <div class="progress-bar progress-bar-danger" role="progressbar" aria-valuenow="60"
                //        aria-valuemin="0" aria-valuemax="100" style="width: 100%;">
                //        <span style="color: black;font-weight:bold">100%</span></div>
                //</div>
                //<b>Interface 4 Down %</b>
                //<div class="progress">
                //    <div class="progress-bar progress-bar-warning" role="progressbar" aria-valuenow="60"
                //        aria-valuemin="0" aria-valuemax="100" style="width: 0%;">
                //        <span style="color: black;font-weight:bold">0%</span></div>
                //</div>");

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
    }

}