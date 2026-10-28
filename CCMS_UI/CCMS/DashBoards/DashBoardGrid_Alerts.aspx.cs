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

namespace CCMSUI.CCMS
{
    public partial class DashBoardGrid_Alerts : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewAlertDashBoard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                AlertType.AlertTypeReader alertTypeReader = AlertType.ExecuteReader("1=1");
                DropDownList_AlertType.Items.Add(new ListItem("Select"));
                while (alertTypeReader.Read())
                    DropDownList_AlertType.Items.Add(new ListItem(alertTypeReader.CurrentAlertType.AlertTypeName, alertTypeReader.CurrentAlertType.AlertTypeId.ToString()));
                alertTypeReader.Close();

                Session["SortExpression1"] = "generated_at ";
                Session["sortDirection1"] = "desc ";
                Session["SortExpression2"] = "generated_at ";
                Session["sortDirection2"] = "desc ";
                Session["SortExpression3"] = "generated_at ";
                Session["sortDirection3"] = "desc ";
                Session["SortExpression4"] = "generated_at ";
                Session["sortDirection4"] = "desc ";
                if (Session["Grid1"] != null)
                {

                    BindGrid(Session["SortExpression1"].ToString() + Session["sortDirection1"].ToString(), 1, Session["Grid1"].ToString());

                }
                if (Session["Grid2"] != null)
                {

                    BindGrid(Session["SortExpression2"].ToString() + Session["sortDirection2"].ToString(), 2, Session["Grid2"].ToString());
                }
                if (Session["Grid3"] != null)
                {

                    BindGrid(Session["SortExpression3"].ToString() + Session["sortDirection3"].ToString(), 3, Session["Grid3"].ToString());
                }
                if (Session["Grid4"] != null)
                {

                    BindGrid(Session["SortExpression4"].ToString() + Session["sortDirection4"].ToString(), 4, Session["Grid4"].ToString());
                }


                //BindGrid(Session["SortExpression"].ToString() + Session["sortDirection"].ToString());
                //BindGrid();
            }

        }


        private string Filter()
        {
            AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

            StringBuilder builder = new StringBuilder();
            int alertType = Convert.ToInt32(DropDownList_AlertType.SelectedValue);

            if (TextBox_FromDate.Text.Length > 0 && TextBox_ToDate.Text.Length > 0)
                builder.Append(" and generated_at >= convert(datetime, '" + TextBox_FromDate.Text + "', 103) and generated_at <= convert(datetime, '" + TextBox_ToDate.Text + " 23:59:59', 103)");
            else
            {
                if (appSetting.ThresholdForAlert != -1)
                    builder.Append("and generated_at >=convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForAlert.Value).ToString("dd/MM/yyyy") + "',103) " +
                           " and generated_at <=convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForAlert.Value).ToString("dd/MM/yyyy") + " 23:59:59',103) ");
            }

            if (DropDownList_AlertType.SelectedIndex > 0)
            {
                if (DropDownList_AlertType.SelectedValue == "5")
                    builder.Append(" and alert_type.alert_type_id in (5,6) ");
                else
                    builder.Append(" and alert_type.alert_type_id = " + DropDownList_AlertType.SelectedValue + "  ");
            }
             
            if (Request.QueryString["n"]!=null)
                builder.Append(" and atm.note_set_type_id = " + Request.QueryString["n"]);

            builder.Append(" and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")");

            return builder.ToString();
        }
        private string BindGrid(string orderBy, int gridNumber, string query)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                Page.RegisterStartupScript("script", "<script>function initDashboard() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} initDashboard();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                if (query.Length == 0)
                {
                    if (DropDownList_AlertType.SelectedValue == "5")
                    {
                        cmd.CommandText = @"select 'TCP/IP Connection Down' alert_type_name,generated_at, title,ip,region.region_name, alert_msg from atm,region,atm_alert,alert_type where atm_alert.alert_type_id =alert_type.alert_type_id and atm.region_id = region.region_id  and atm.atm_id = atm_alert.atm_id " +
                            "  and (expiration_time is null or expiration_time >=getdate()) " + Filter() + " order by " + orderBy;
                    }
                    else
                        cmd.CommandText = @"select alert_type_name,generated_at, title,ip,region.region_name, alert_msg from atm,region,atm_alert,alert_type where atm_alert.alert_type_id =alert_type.alert_type_id and atm.region_id = region.region_id  and atm.atm_id = atm_alert.atm_id " +
                            "  and (expiration_time is null or expiration_time >=getdate()) " + Filter() + " order by " + orderBy;
                }
                else
                    cmd.CommandText = query;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtAgentMonitoring = new DataTable();
                adapter.Fill(dtAgentMonitoring);
                if (gridNumber == 1)
                {
                    GridView_Grid1.DataSource = dtAgentMonitoring;
                    GridView_Grid1.DataBind();
                }
                else if (gridNumber == 2)
                {
                    GridView_Grid2.DataSource = dtAgentMonitoring;
                    GridView_Grid2.DataBind();
                }

                else if (gridNumber == 3)
                {
                    GridView_Grid3.DataSource = dtAgentMonitoring;
                    GridView_Grid3.DataBind();
                }
                else if (gridNumber == 4)
                {
                    GridView_Grid4.DataSource = dtAgentMonitoring;
                    GridView_Grid4.DataBind();
                }

            }
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
            return cmd.CommandText;

        }

        protected void GridView_Grid1_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Session["SortExpression1"].ToString() == e.SortExpression)
            {
                if (Session["sortDirection1"].ToString() == " asc")
                    Session["sortDirection1"] = " desc";
                else
                    Session["sortDirection1"] = " asc";
            }
            else
            {
                Session["SortExpression1"] = e.SortExpression;
                Session["sortDirection1"] = " asc";
            }
            BindGrid(Session["SortExpression1"].ToString() + Session["sortDirection1"].ToString(), 1, "");

        }
        //
        protected void GridView_Grid2_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Session["SortExpression2"].ToString() == e.SortExpression)
            {
                if (Session["sortDirection2"].ToString() == " asc")
                    Session["sortDirection2"] = " desc";
                else
                    Session["sortDirection2"] = " asc";
            }
            else
            {
                Session["SortExpression2"] = e.SortExpression;
                Session["sortDirection2"] = " asc";
            }
            BindGrid(Session["SortExpression2"].ToString() + Session["sortDirection2"].ToString(), 2, "");

        }
        protected void GridView_Grid3_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Session["SortExpression3"].ToString() == e.SortExpression)
            {
                if (Session["sortDirection3"].ToString() == " asc")
                    Session["sortDirection3"] = " desc";
                else
                    Session["sortDirection3"] = " asc";
            }
            else
            {
                Session["SortExpression3"] = e.SortExpression;
                Session["sortDirection3"] = " asc";
            }
            BindGrid(Session["SortExpression3"].ToString() + Session["sortDirection3"].ToString(), 3, "");

        }
        //
        protected void GridView_Grid4_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Session["SortExpression4"].ToString() == e.SortExpression)
            {
                if (Session["sortDirection4"].ToString() == " asc")
                    Session["sortDirection4"] = " desc";
                else
                    Session["sortDirection4"] = " asc";
            }
            else
            {
                Session["SortExpression4"] = e.SortExpression;
                Session["sortDirection4"] = " asc";
            }
            BindGrid(Session["SortExpression4"].ToString() + Session["sortDirection4"].ToString(), 4, "");

        }


        private void UpdateDashboard()
        {
            if (DropDownList_Grid.SelectedItem.Text == "Grid1")
            {
                Session["Grid1"] = BindGrid(Session["SortExpression1"].ToString() + Session["sortDirection1"].ToString(), 1, "");
                if (Session["Grid2"] != null)
                    BindGrid(Session["SortExpression2"].ToString() + Session["sortDirection2"].ToString(), 2, Session["Grid2"].ToString());
                if (Session["Grid3"] != null)
                    BindGrid(Session["SortExpression3"].ToString() + Session["sortDirection3"].ToString(), 3, Session["Grid3"].ToString());
                if (Session["Grid4"] != null)
                    BindGrid(Session["SortExpression4"].ToString() + Session["sortDirection4"].ToString(), 4, Session["Grid4"].ToString());
            }

            if (DropDownList_Grid.SelectedItem.Text == "Grid2")
            {
                Session["Grid2"] = BindGrid(Session["SortExpression2"].ToString() + Session["sortDirection2"].ToString(), 2, "");
                if (Session["Grid1"] != null)
                    BindGrid(Session["SortExpression1"].ToString() + Session["sortDirection1"].ToString(), 1, Session["Grid1"].ToString());
                if (Session["Grid3"] != null)
                    BindGrid(Session["SortExpression3"].ToString() + Session["sortDirection3"].ToString(), 3, Session["Grid3"].ToString());
                if (Session["Grid4"] != null)
                    BindGrid(Session["SortExpression4"].ToString() + Session["sortDirection4"].ToString(), 4, Session["Grid4"].ToString());

            }
            if (DropDownList_Grid.SelectedItem.Text == "Grid3")
            {
                Session["Grid3"] = BindGrid(Session["SortExpression3"].ToString() + Session["sortDirection3"].ToString(), 3, "");
                if (Session["Grid1"] != null)
                    BindGrid(Session["SortExpression1"].ToString() + Session["sortDirection1"].ToString(), 1, Session["Grid1"].ToString());
                if (Session["Grid2"] != null)
                    BindGrid(Session["SortExpression2"].ToString() + Session["sortDirection2"].ToString(), 2, Session["Grid2"].ToString());
                if (Session["Grid4"] != null)
                    BindGrid(Session["SortExpression4"].ToString() + Session["sortDirection4"].ToString(), 4, Session["Grid4"].ToString());
            }
            if (DropDownList_Grid.SelectedItem.Text == "Grid4")
            {
                Session["Grid4"] = BindGrid(Session["SortExpression4"].ToString() + Session["sortDirection4"].ToString(), 4, "");
                if (Session["Grid1"] != null)
                    BindGrid(Session["SortExpression1"].ToString() + Session["sortDirection1"].ToString(), 1, Session["Grid1"].ToString());
                if (Session["Grid2"] != null)
                    BindGrid(Session["SortExpression2"].ToString() + Session["sortDirection2"].ToString(), 2, Session["Grid2"].ToString());
                if (Session["Grid3"] != null)
                    BindGrid(Session["SortExpression3"].ToString() + Session["sortDirection3"].ToString(), 3, Session["Grid3"].ToString());

            }




        }
        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            UpdateDashboard();
        }


    }
}