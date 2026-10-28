using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.admin
{
    public partial class ReportTask : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewReportTask, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["SortExpression"] = "creation_time";
                ViewState["sortDirection"] = " desc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                if (Session["msg"] != null)
                {
                    Literal_ok.Text = Session["msg"].ToString();
                    Session["msg"] = null;
                }

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                DropDownList_status.SelectedValue = "All";


                BindGrid();

            }
        }

        private void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindReportTaskGrid");
            try
            { /* case 
			                                    when len(failure_reason) > 0 then 
				                                    substring(failure_reason, 0, 29) + '...' 
			                                    else 
				                                    '' 
		                                    end failure_reason*/
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select report_task_id, report_task.report_schedule_id, file_path_attachment, report_task.retry_count, 
		                                    failure_reason, 
		                                    creation_time, last_invoked_at, status, report_name 
                                    from report_task 
                                    inner join report_schedule on report_task.report_schedule_id = report_schedule.report_schedule_id 
                                    where 1=1 " + GetFilter()
                                    + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTasks = new DataTable();
                adapter.Fill(dtTasks);
                Cache["dtTasks"] = dtTasks;
                Literal_count.Text = dtTasks.Rows.Count + " records.";
                this.GridView_Tasks.DataSource = dtTasks;
                this.GridView_Tasks.DataBind();

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        private string GetFilter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
            {
                filter += " and creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
                //Session["creationFromDate"] = TextBox_FromDate.Text;
            }
            else
                filter += " and creation_time >= getdate() - 3";

            if (TextBox_ToDate.Text.Length > 0)
            {
                filter += " and creation_time <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";
                //Session["creationToDate"] = TextBox_ToDate.Text;
            }

            if (DropDownList_status.SelectedIndex > 0)
            {
                filter += " and status = '" + DropDownList_status.SelectedValue + "'";
                //Session["taskStatus"] = DropDownList_status.SelectedValue;
            }

            return filter;
        }

        protected void button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            DropDownList_status.SelectedIndex = 0;
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }

        protected void GridView_Tasks_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Tasks.PageIndex = e.NewPageIndex;
            BindGridWithCache();
        }

        protected void GridView_Tasks_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
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

            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            BindGridWithCache();
        }

        private void BindGridWithCache()
        {
            DataTable dtTask = (DataTable)Cache["dtTasks"];
            dtTask.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
            this.GridView_Tasks.DataSource = dtTask;
            this.GridView_Tasks.DataBind();
            Literal_count.Text = dtTask.Rows.Count + " records.";

        }
    }
}