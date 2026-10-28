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

namespace CCMSUI.CCMS
{
    public partial class ViewDailyFeed : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (!CheckRight((int)Permissions.ViewDailyFeedSchedule))
                    Response.Redirect("../admin/AccessDenied.aspx");

                if (!CheckRight((int)Permissions.DeleteDailyFeedSchedule))
                    GridView_Schedules.Columns[6].Visible = false;



                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "mcn";
                HyperLink_newSchedules.Visible = CheckRight((int)Permissions.CreateDailyFeedSchedule);
               
             

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
        }
        protected void button_search_Click(object sender, EventArgs e)
        { BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString()); }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

        }
        string GetFilter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += "  daily_feed_schedule.creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103) and ";

            if (TextBox_ToDate.Text.Length > 0)
                filter += "  daily_feed_schedule.creation_time<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and ";

            if (filter.Length > 0)
                return "where " + filter.Substring(0, filter.LastIndexOf("and "));
            else
                return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select daily_feed_schedule_id,mcn,date_from,date_to,is_executed, retry_count,failure_reason
from daily_feed_schedule " + GetFilter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtATMs = new DataTable();
                adapter.Fill(dtATMs);
                cmd.Connection.Close();
                this.GridView_Schedules.DataSource = dtATMs; ;
                this.GridView_Schedules.DataBind();
                Literal_count.Text = dtATMs.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }


        

        protected void GridView_Schedules_Sorting(object sender, GridViewSortEventArgs e)
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


        protected void GridView_Schedules_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Schedules.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Schedules_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteSchedule") != null)


                ((ImageButton)e.Row.FindControl("Button_deleteSchedule")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete schedule for mcn : " + ((DataRowView)e.Row.DataItem).Row["mcn"] + "?','CCMS')");
        }

        protected void GridView_Schedules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteDailyFeedSchedule")
            {
                SqlCommand cmd = null;
                SqlTransaction trx = null;
                try
                {
                    task = LogableTask.NewTask("deleteDailyFeedSchedule");
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "delete daily_feed_schedule where daily_feed_schedule_id = " + e.CommandArgument.ToString();
                    cmd.ExecuteNonQuery();
                    
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "DFF Schedule deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                    Utility.BuildAuditLog("DFF Schedule deleted for org mcn: "+e.CommandArgument.ToString(), (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteDailyFeedSchedule);

                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting daily feed schedules. " + ex.Message + "</li>";
                    
                }
                finally
                {
                    cmd.Connection.Close();
                    task.EndTask();

                }
            }
        }

    }
}