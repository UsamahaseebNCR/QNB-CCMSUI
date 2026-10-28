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
using Avanza.iSuite.UI;
using System.Reflection;
using System.Diagnostics;
using System.Text;

using System.Data.SqlClient;
using Avanza.CCMS.DAL;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class TaskStatusReport : BasePage
    {
        private void BindNoteSetType()
        {
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && regionID != "1")
            {
                var dt = new DataTable();
                dt.Columns.Add("note_set_type_name");
                dt.Columns.Add("region_id");
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                //dt.Load((IDataReader)reader);
                while (reader.Read())
                {
                    //    DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                    dt.Rows.Add(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString());

                }
                if (dt.Rows.Count > 0)
                {
                    groupList.DataSource = dt;
                    groupList.DataTextField = "note_set_type_name";
                    groupList.DataValueField = "region_id";
                    groupList.DataBind();
                }
              
                reader.Close();
            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindNoteSetType();
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;

                TaskType.TaskTypeReader taskTypeReader = TaskType.ExecuteReader("task_type_id not in (2,3) order by task_type_name");
                Dropdownlist_taskType.Items.Add(new ListItem("All", "All"));
                while (taskTypeReader.Read())
                {
                    Dropdownlist_taskType.Items.Add(new ListItem(taskTypeReader.CurrentTaskType.TaskTypeName, taskTypeReader.CurrentTaskType.TaskTypeId.ToString()));
                }
                taskTypeReader.Close();
                // user_id =1 is for system
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("user_id > 1 order by user_login", AppUser.Columns.user_id | AppUser.Columns.user_login);
                Dropdownlist_user.Items.Add(new ListItem("Any", "Any"));
                Dropdownlist_user.Items.Add(new ListItem("Automatically", "1"));
                while (userReader.Read())
                    Dropdownlist_user.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();

                ViewState["SortExpression"] = "title";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                FillComboWithFriendlyNames(DropDownList_status, "All");

                DropDownList_ATMType.Items.Add(new ListItem("Any", "Any"));

                AddEditATM.PopulateATMTypes(DropDownList_ATMType);
                BindNoteSetType();
            }
        }

        public static void FillComboWithFriendlyNames(DropDownList ddlStatus, string task)
        {
            SqlCommand cmd = null;
            try
            {
                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(new ListItem("All", "All"));
                Array downloadStates = Enum.GetValues(typeof(DownloadStates));
                Array uploadStates = Enum.GetValues(typeof(UploadStates));

                cmd = ConnectionFactory.GetNewCommand(true);

                if (task == "All")
                {
                    for (int counter = 0; counter < downloadStates.Length; counter++)
                    {
                        string tempString = downloadStates.GetValue(counter).ToString();
                        cmd.CommandText = "select dbo.GetStatusFriendlyName ('" + tempString + "') Status";
                        cmd.CommandType = CommandType.Text;
                        ddlStatus.Items.Add(cmd.ExecuteScalar().ToString());
                    }
                    for (int counter = 0; counter < uploadStates.Length; counter++)
                    {
                        string tempString = uploadStates.GetValue(counter).ToString();
                        cmd.CommandText = "select dbo.GetStatusFriendlyName ('" + tempString + "') Status";
                        cmd.CommandType = CommandType.Text;
                        string dbString = cmd.ExecuteScalar().ToString();
                        if (!ddlStatus.Items.Contains(new ListItem(dbString)))
                            ddlStatus.Items.Add(dbString);
                    }
                }
                else if (task == "Download")
                {
                    for (int counter = 0; counter < downloadStates.Length; counter++)
                    {
                        string tempString = downloadStates.GetValue(counter).ToString();
                        cmd.CommandText = "select dbo.GetStatusFriendlyName ('" + tempString + "') Status";
                        cmd.CommandType = CommandType.Text;
                        ddlStatus.Items.Add(cmd.ExecuteScalar().ToString());
                    }
                }
                else
                {
                    for (int counter = 0; counter < uploadStates.Length; counter++)
                    {
                        string tempString = uploadStates.GetValue(counter).ToString();
                        cmd.CommandText = "select dbo.GetStatusFriendlyName ('" + tempString + "') Status";
                        cmd.CommandType = CommandType.Text;
                        ddlStatus.Items.Add(cmd.ExecuteScalar().ToString());

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

        private string buildSearchCriteria()
        {
            StringBuilder groupbuilder = new StringBuilder();
            foreach (ListItem item in groupList.Items)
            {
                if(item.Selected)
                {
                    if (groupbuilder.Length > 0)
                        groupbuilder.Append(",");
                    groupbuilder.Append(item.Value);
                }
                   
            }
          
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
            else
                filter += " and creation_time >= getdate() - 3";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and creation_time <= convert(datetime,'" + TextBox_ToDate.Text + "',103)";

            if (TextBox_EndTimeFromDate.Text.Length > 0)
                filter += " and end_time >= convert(datetime,'" + TextBox_EndTimeFromDate.Text + " ',103)";

            if (TextBox_EndTimeToDate.Text.Length > 0)
                filter += " and end_time <= convert(datetime,'" + TextBox_EndTimeToDate.Text + "',103)";

            if (groupbuilder.Length > 0)
                filter += " and note_set_type_id in ( " + groupbuilder.ToString() + " ) ";

            if (Dropdownlist_taskType.SelectedIndex > 0)
                filter += " and task_type_id  = " + Dropdownlist_taskType.SelectedValue;

            if (Dropdownlist_user.SelectedIndex > 0)
                filter += " and created_by = " + Dropdownlist_user.SelectedValue;

            if (DropDownList_status.SelectedIndex > 0)
                filter += " and status = '" + DropDownList_status.SelectedValue + "'";

            if (DropDownList_ATMType.SelectedIndex > 0)
                filter += " and atm_type = '" + DropDownList_ATMType.SelectedValue + "'";

            return filter;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;


                //                cmd.CommandText = @"SELECT top 1000 title,region.region_id,region.region_name,parent_region_id,ip,F.creation_time,F.bytes_transferred,
                //                                dbo.GetStatusFriendlyName(status) status,
                //                                task_type.task_type_name,
                //                                failure_reason reason,F.end_time,F.last_invoked 
                //                                FROM ATM inner join region on atm.region_id = region.region_id inner join task F ON ATM.ATM_ID = F.ATM_ID 
                //                                inner join task_type on f.task_type_id = task_type.task_type_id where "

                cmd.CommandText = @"SELECT * from vTaskStatusReport where "
                 + "atm_id in (" + Session[SessionVars.SelectedATMs.ToString()]
                 + ") and IS_ACTIVE =1 " + buildSearchCriteria()
                 + " order by " + ViewState["orderby"];
                CCMSUI.CCMS.DsTaskStatusRpt Ds = new CCMSUI.CCMS.DsTaskStatusRpt();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["TaskStatus"]);
                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables[1]);
                PopulateDataTable(Ds.Tables["TaskStatus"]);
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=TaskStatus.rpt&ReportTitle=Task Status Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

                    Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }
        }
        protected void Dropdownlist_taskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Dropdownlist_taskType.SelectedItem.Value != "All")
            {
                if (int.Parse(Dropdownlist_taskType.SelectedItem.Value) == (int)EnumTaskType.CashDataDownload)
                {
                    FillComboWithFriendlyNames(DropDownList_status, "Download");

                }
                else if (int.Parse(Dropdownlist_taskType.SelectedItem.Value) == (int)EnumTaskType.CashOrderUpload ||
                    int.Parse(Dropdownlist_taskType.SelectedItem.Value) == (int)EnumTaskType.Configuration)
                {
                    FillComboWithFriendlyNames(DropDownList_status, "Upload");
                }
            }
            else
            {
                // DropDownList_status.Items.Clear();
                FillComboWithFriendlyNames(DropDownList_status, "All");
                //    FillComboWithUpdateFriendlyNames(DropDownList_status, false);
                //
            }
            // BindGrid(null);

        }

        private void PopulateDataTable(DataTable dt)
        {
            int region_id = 0;
            int id = -1;

            DataTable dtRegions = new DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtRegions);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                region_id = int.Parse(dt.Rows[i][10].ToString()); //parent

                while (true)
                {
                    DataRow[] innerArray = dtRegions.Select("region_id=" + region_id);
                    if (bool.Parse(innerArray[0][2].ToString()))
                    {
                        id = int.Parse(innerArray[0][0].ToString());
                        break;
                    }
                    else
                        region_id = int.Parse(innerArray[0][1].ToString());

                }
                //organization id found
                Region region = Region.LoadRegionByPk(id);
                dt.Rows[i][7] = region.RegionName;
                dt.Rows[i][8] = region.BankLogo;

            }

        }
        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            ReportSchedule reportSchedule = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            try
            {
                if (Session["isOrgSelected"] != null)
                {
                    if (bool.Parse(Session["isOrgSelected"].ToString()))
                    {
                        cmd = ConnectionFactory.GetNewCommand(true);
                        trxn = cmd.Connection.BeginTransaction();


                        string orgID = Session["selectedOrgID"].ToString();
                        ConnectionFactory.ExecuteScalar(@"delete report_generation_schedule
                                                          where report_schedule_id in
                                                          (select  report_schedule_id 
                                                          from report_schedule 
                                                          where report_name='TaskStatusReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='TaskStatusReport' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "TaskStatusReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"TaskStatus.rpt");
                        reportSchedule.ReportReceipients = "";

                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Task Status Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = 1;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        reportSchedule.IsEjEnabled = false;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_errorMsg.Text = "";
                        Utility.BuildAuditLog("Schedule added for Task Status Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.TaskStatusReport);

                    }
                    else
                        Literal_errorMsg.Text = "Please select organization from treeview.";
                }
                else
                    Literal_errorMsg.Text = "Please select organization from treeview.";
            }

            catch (Exception ex)
            {
                if (trxn != null)
                    trxn.Rollback();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);

            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }
    }
}