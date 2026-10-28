using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

namespace Avanza.iSuite.UI
{
    public partial class ScheduleReportGeneration : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }


        private string GetSelectedOrg()
        {
            string selectedOrg = null;
                
            if (Session["selectedOrgID"] != null)
            {
                selectedOrg = Session["selectedOrgID"].ToString();
            }
            else
            {
                string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
                if (regionID != null)
                {
                    Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
                    if (region.IsOrganization)
                    {
                        if (region.RegionId == 1)
                            selectedOrg = (string)Session["org"];
                        else
                            selectedOrg = region.RegionId.ToString();
                    }
                }
            }
            return selectedOrg;
         }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindGrid(GetSelectedOrg());
            

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewScheduleReports))
                    Response.Redirect("AccessDenied.aspx");

                if (!CheckRight((int)Permissions.EditScheduleReports))
                    GridView_ReportSchedule.Columns[2].Visible = false;
                if (!CheckRight((int)Permissions.DeleteReportSchedule))
                    GridView_ReportSchedule.Columns[3].Visible = false;

                BindGrid(GetSelectedOrg());
            }
        }

        private void BindGrid(string selectedOrgIDs)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                if (selectedOrgIDs == null)
                    return;
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
               cmd.CommandText = @"SELECT report_schedule_id, ccms_cit.name as cit_name, report_name, report_friendly_name,region_name FROM report_schedule"+
                    " INNER JOIN region ON report_schedule.organization_id = region.region_id " +
                    " LEFT JOIN ccms_cit ON report_schedule.cit_id = ccms_cit.id WHERE region.region_id in  (" + selectedOrgIDs + ")";										 
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtReport = new DataTable();
                adapter.Fill(dtReport);
                cmd.Connection.Close();
                this.GridView_ReportSchedule.DataSource = dtReport;
                this.GridView_ReportSchedule.DataBind();
                Literal_count.Text = dtReport.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }
        protected void GridView_ReportSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteSchedule") != null)
                ((ImageButton)e.Row.FindControl("Button_deleteSchedule")).Attributes.Add("onClick",
                  "return confirm('Are you sure you want to delete Schedule for the report :" + ((DataRowView)e.Row.DataItem).Row["report_friendly_name"] + "?','CCMS')");


        }
        protected void GridView_ReportSchedule_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteSchedule")
            {
                SqlCommand cmd = null;
                SqlTransaction trx = null;
                try
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    task = LogableTask.NewTask("delete Schedule");
                    trx = cmd.Connection.BeginTransaction();
                    int scheduleID = int.Parse(e.CommandArgument.ToString());
                    ConnectionFactory.ExecuteQuery("delete report_task where report_schedule_id = " + scheduleID, trx);
                    ConnectionFactory.ExecuteQuery("delete report_schedule where report_schedule_id = " + scheduleID, trx);
                    trx.Commit();
                    Utility.BuildAuditLog("Schedule[" + scheduleID + "] deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteReportSchedule);
                    Session[SessionVars.notice.ToString()] = "Selected schedule deleted!";
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "schedule deleted");
                    BindGrid(GetSelectedOrg());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting schedule. " + ex.Message + "</li>";
                    if (trx != null)
                        trx.Rollback();
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