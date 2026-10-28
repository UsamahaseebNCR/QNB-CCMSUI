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
using System.Text;

using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace CCMSUI.CCMS
{
    public partial class AuditLog : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewAuditLog))
                {
                    Response.StatusCode = 401;
                    Response.Redirect("../admin/AccessDenied.aspx");
                }

                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                DropDownList_User.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    DropDownList_User.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();


                DropDownList_Rights.Items.Add(new ListItem("Any", "Any"));
                Array permissions = Enum.GetValues(typeof(Permissions));

                //26*permission length 
                for (int outer = 'A'; outer < 'Z'; outer++)
                    for (int inner = 0; inner < permissions.Length; inner++)
                    {
                        if (permissions.GetValue(inner).ToString().ToLower().Contains("report") ||
                            permissions.GetValue(inner).ToString().ToLower().Contains("view"))
                            continue;
                        if (permissions.GetValue(inner).ToString()[0] == (char)outer)
                            DropDownList_Rights.Items.Add(new ListItem(spaceCreator(permissions.GetValue(inner).ToString()), ((int)permissions.GetValue(inner)).ToString()));
                    }


                TextBox_FromDate.Text = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                BindGrid();

            }
        }
        public string spaceCreator(string permissionName)
        {
            StringBuilder builder = new StringBuilder(100);


            builder.Append(permissionName[0]);

            for (int counter = 1; counter < permissionName.Length; counter++)
            {
                if (char.IsUpper(permissionName, counter) && char.IsLower(permissionName, counter - 1))
                    builder.Append(" " + permissionName[counter]);
                else
                    builder.Append(permissionName[counter]);
            }

            //string updatedPermissionName = builder.ToString();
            //if (updatedPermissionName.IndexOf("i Suite") >= 0)
            //    updatedPermissionName = updatedPermissionName.Replace("i Suite", " iSuite");
            //else if (updatedPermissionName.IndexOf("i Journal") >= 0)
            //    updatedPermissionName = updatedPermissionName.Replace("i Journal", " iJournal");
            //else if (updatedPermissionName.IndexOf("i Cam") >= 0)
            //    updatedPermissionName = updatedPermissionName.Replace("i Cam", " iCam");

            return builder.ToString(); //updatedPermissionName;
        }
        private void BindGrid()
        {
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select activity_time,message,u.user_full_name,ip_address,field_name,old_value,new_value
                                 from audit_log,app_user u 
                                where audit_log.user_id = u.user_id" + BuildCriteria() +" order by activity_time desc" ;// exclude system user
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataTable dtAuditLog = new DataTable();
                adapter.Fill(dtAuditLog);

                Cache[CacheVars.dtAuditLog.ToString()] = dtAuditLog;
                cmd.Connection.Close();
                Literal_count.Text = dtAuditLog.Rows.Count + " records.";
                this.GridView_AuditLog.DataSource = dtAuditLog;
                this.GridView_AuditLog.DataBind();

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("bind audit log grid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

        public SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;

                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected void GridView_AuditLog_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = (DataTable)Cache[CacheVars.dtAuditLog.ToString()];
            DataView dv = new DataView(dt);
            GridViewSortDirection = (GridViewSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
            dv.Sort = e.SortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            GridView_AuditLog.DataSource = dv;
            GridView_AuditLog.DataBind();

        }
        protected void GridView_AuditLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            DataTable dt = (DataTable)Cache[CacheVars.dtAuditLog.ToString()];
            DataView dv = new DataView(dt);
            if (GridViewSortExpression != "")
                dv.Sort = GridViewSortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            this.GridView_AuditLog.DataSource = dv;

            this.GridView_AuditLog.PageIndex = e.NewPageIndex;
            this.GridView_AuditLog.DataBind();

        }
        private string GridViewSortExpression
        {
            get
            { return ViewState["SortExpression"] as string ?? string.Empty; }
            set
            { ViewState["SortExpression"] = value; }
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            DropDownList_User.SelectedIndex = 0;

        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        public string BuildCriteria()
        {
            StringBuilder builder = new StringBuilder();
            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" and activity_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)");
            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(" and activity_time <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59',103)");
            if (DropDownList_User.SelectedIndex > 0)
                builder.Append(" and u.user_id = " + DropDownList_User.SelectedValue);
            if (DropDownList_Rights.SelectedIndex > 0)
                builder.Append(" and right_id = " + DropDownList_Rights.SelectedValue);


            return builder.ToString();

        }

        protected void Button_ExportToPDF_Click(object sender, EventArgs e)
        {
            LogableTask logableTask = LogableTask.NewTask("Button_ExportToPDF_Click");
            try
            {
                SqlCommand newCommand = ConnectionFactory.GetNewCommand(false);
                newCommand.CommandTimeout = 500;
                newCommand.CommandText = @"select activity_time,message,u.user_full_name
                                            from audit_log,app_user u
                                            where audit_log.user_id = u.user_id" + this.BuildCriteria() + " order by activity_time desc";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(newCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                ReportDocument reportDocument = new ReportDocument();
                reportDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\AuditLogRpt.rpt");
                reportDocument.SetDataSource(dataTable);
                reportDocument.SummaryInfo.ReportTitle = "Activity Log";
                reportDocument.SetParameterValue("GeneratedBy", (object)((AppUser)this.Session[SessionVars.user.ToString()]).UserLogin);
                reportDocument.SetParameterValue("total", (object)dataTable.Rows.Count);
                string str = ((AppSetting)this.Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\ActivityLog" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";
                reportDocument.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, str);
                reportDocument.Close();
                this.Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(str));
                this.Response.WriteFile(str);
                this.Response.Flush();
                this.Response.Close();
            }
            catch (Exception ex)
            {
                this.Literal_error.Text = ex.Message;
                logableTask.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                logableTask.EndTask();
            }
        }

        protected void Button_ExportToExcel_Click(object sender, EventArgs e)
        {
            LogableTask logableTask = LogableTask.NewTask("Button_ExportToExcel_Click");
            try
            {
                SqlCommand newCommand = ConnectionFactory.GetNewCommand(false);
                newCommand.CommandTimeout = 500;
                newCommand.CommandText = "select activity_time,message,u.user_full_name from audit_log,app_user u where audit_log.user_id = u.user_id" + this.BuildCriteria() + " order by activity_time desc";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(newCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                ReportDocument reportDocument = new ReportDocument();
                reportDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\AuditLogRpt.rpt");
                reportDocument.SetDataSource(dataTable);
                reportDocument.SummaryInfo.ReportTitle = "Activity Log";
                reportDocument.SetParameterValue("GeneratedBy", (object)((AppUser)this.Session[SessionVars.user.ToString()]).UserLogin);
                reportDocument.SetParameterValue("total", (object)dataTable.Rows.Count);
                string str = ((AppSetting)this.Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\ActivityLog" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
                reportDocument.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelRecord, str);
                reportDocument.Close();
                this.Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(str));
                this.Response.WriteFile(str);
                this.Response.Flush();
                this.Response.Close();
            }
            catch (Exception ex)
            {
                this.Literal_error.Text = ex.Message;
                logableTask.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                logableTask.EndTask();
            }
        }
    }
}