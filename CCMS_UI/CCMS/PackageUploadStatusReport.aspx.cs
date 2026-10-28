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
    public partial class PackageUploadStatusReport : BasePage
    {
         protected void refresh_ServerClick(object sender, EventArgs e)
        {
    //        BindNoteSetType();
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

               // TaskType.TaskTypeReader taskTypeReader = TaskType.ExecuteReader("task_type_id not in (2,3) order by task_type_name");
                 // user_id =1 is for system
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("user_id > 1 order by user_login", AppUser.Columns.user_id | AppUser.Columns.user_login);
                Dropdownlist_user.Items.Add(new ListItem("Any", "Any"));
                Dropdownlist_user.Items.Add(new ListItem("Automatically", "1"));
                while (userReader.Read())
                    Dropdownlist_user.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();


                Package.PackageReader packageReader = Package.ExecuteReader("1=1");
                DropDownList_Package.Items.Add(new ListItem("Any", "Any"));
                while (packageReader.Read())
                    DropDownList_Package.Items.Add(new ListItem(packageReader.CurrentPackage.PackageTitle, packageReader.CurrentPackage.PackageId.ToString()));
                packageReader.Close();
                ViewState["SortExpression"] = "package_title";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy")+" 00:00:00";
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
               // FillComboWithFriendlyNames(DropDownList_status, "All");

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
            StringBuilder selectQuery = new StringBuilder();
            if (DDLUploadStatus.SelectedIndex > 0)
            {
                selectQuery.Append(" upld.STATUS = '" + DDLUploadStatus.SelectedValue + "' AND ");
            }

            if (DropDownList_Package.SelectedIndex > 0)
            {
                selectQuery.Append(" upld.package_id = '" + DropDownList_Package.SelectedItem.Value + "' AND ");
            }

            //if (txtATMIP.Text.Length > 0)
            //    selectQuery.Append(" IP like '" + txtATMIP.Text + "%' AND ");

            if (TextBox_FromDate.Text.Length > 0)
                selectQuery.Append(@" upld.creation_time >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) AND ");

            if (TextBox_ToDate.Text.Length > 0)
                selectQuery.Append(@"  upld.creation_time <=Convert(datetime,'" + TextBox_ToDate.Text + "',103) AND ");


            if (TextBox_EndTimeFromDate.Text.Length > 0)
                selectQuery.Append(@" upld.end_time >= Convert(datetime,'" + TextBox_EndTimeFromDate.Text + "',103) AND ");

            if (TextBox_EndTimeToDate.Text.Length > 0)
                selectQuery.Append(@"  upld.end_time <=Convert(datetime,'" + TextBox_EndTimeToDate.Text + "',103) AND ");

            selectQuery.Append(@" ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") AND ");

            string query = selectQuery.ToString();
            query = query.Trim();

            if (query.EndsWith("AND"))
                query = query.Substring(0, query.LastIndexOf("AND"));
            if (query == "")
                return query;
            else
                return "where " + query;
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select atm.title as title, package_title, dbo.GetStatusFriendlyName(upld.status) status, upld.creation_time,appuser.user_login,
                                    upld.end_time,package.uploaded_bytes,package_physcial_path,retry_remaining,last_invoked,failure_reason,execution_time,upload_id,package_size, upld.uploaded_bytes bytes_uploaded
                        from UPLOAD upld inner join ATM atm 
                        on upld.atm_id = atm.atm_id inner join app_user appuser on
                        upld.created_by = appuser.user_id  inner join package on package.package_id = upld.package_id " + buildSearchCriteria() + "order by " + ViewState["orderby"];
            
                CCMSUI.CCMS.dsUpload Ds = new CCMSUI.CCMS.dsUpload();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables[0]);
                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables[1]);
                //PopulateDataTable(Ds.Tables["TaskStatus"]);
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=UploadStatusReport.rpt&ReportTitle=Package Distribution Status Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

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
                                                          where report_name='PackageDistributionReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='PackageDistributionReport' and organization_id = " + orgID, trxn);

                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "PackageDistributionReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"UploadStatusReport.rpt");
                        reportSchedule.ReportReceipients = "";
                        if(appSetting != null )
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Package Distribution Report";
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
                        Utility.BuildAuditLog("Schedule added for Package Distribution Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.TaskStatusReport);

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