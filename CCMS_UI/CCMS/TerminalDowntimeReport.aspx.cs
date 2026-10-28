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
    public partial class TerminalDowntimeReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.TerminalDowntimeReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;


                ViewState["SortExpression"] = "title";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                DropDownList_status.Items.Clear();
                DropDownList_status.Items.Add(new ListItem("All", "All"));
                DropDownList_status.Items.Add(new ListItem("Down", "Down"));
                DropDownList_status.Items.Add(new ListItem("Up", "Up"));

            }
        }


        /*    public static void FillComboWithFriendlyNames(DropDownList ddlStatus, string task)
            {
                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(new ListItem("All", "All"));
                Array downloadStates = Enum.GetValues(typeof(DownloadStates));
                Array uploadStates = Enum.GetValues(typeof(UploadStates));

                SqlCommand cmd = ConnectionFactory.GetNewCommand(true);

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
            }*/

        private string buildSearchCriteria()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and generated_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  ";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and generated_at <= convert(datetime,'" + TextBox_ToDate.Text + "',103)  ";

            if (DropDownList_status.SelectedIndex > 0)
                filter += " and status = '" + DropDownList_status.SelectedValue + "'  ";



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
                cmd.CommandText = @"select * from vTerminalDowntimeReport "
                 + "where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()]
                 + ") and IS_ACTIVE =1 " + buildSearchCriteria();

                CCMSUI.CCMS.DsTerminalDowntimeRpt Ds = new CCMSUI.CCMS.DsTerminalDowntimeRpt();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["TerminalDowntime"]);
                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables[1]);
                PopulateDataTable(Ds.Tables["TerminalDowntime"]);
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=TerminalDowntime.rpt&ReportTitle=Terminal Downtime Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

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
            cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region where parent_region_id is not null";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtRegions);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                region_id = int.Parse(dt.Rows[i][9].ToString()); //parent

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
                dt.Rows[i][6] = region.RegionName;
                dt.Rows[i][7] = region.BankLogo;

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
                                                          where report_name='TerminalDowntimeReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='TerminalDowntimeReport' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "TerminalDowntimeReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"TerminalDowntime.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Terminal Downtime Report";
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
                        Utility.BuildAuditLog("Schedule added for Terminal Downtime Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.TerminalDowntimeReport);

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