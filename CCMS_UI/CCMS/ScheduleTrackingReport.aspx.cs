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
    public partial class ScheduleTrackingReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;

                SqlDataReader downloadingScheduleReader = null;
                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
                try
                {
                    cmd.CommandText = @"select substring(convert(varchar, next_download_at, 108),0,6) as next_download_at,downloading_schedule_id
                                    from downloading_schedule";
                    downloadingScheduleReader = cmd.ExecuteReader();
                    schedulesList.DataSource = downloadingScheduleReader;
                    schedulesList.DataTextField = "next_download_at";
                    schedulesList.DataValueField = "downloading_schedule_id";
                    schedulesList.DataBind();
                }
                catch (Exception ex)
                {
                    Literal_errorMsg.Text = ex.Message;
                }
                finally
                {
                    if (downloadingScheduleReader!=null)
                        downloadingScheduleReader.Close();
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                }
            }
        }

        private string buildSearchCriteria()
        {
            string filter = " and (";
            bool scheduleSelected = false;
            foreach (ListItem Item in schedulesList.Items)
            {
                if (Item.Selected == true)
                {
                    filter += " downloading_schedule_id=" + Item.Value + " or";
                    scheduleSelected = true;
                }
            }
            if (scheduleSelected == false)
                throw new Exception("Please select atleast one shedule time");

            filter = filter.Substring(0, filter.Length - 2);
            filter += ")";
           
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and creation_time <= convert(datetime,'" + TextBox_ToDate.Text + "',103)";

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
                cmd.CommandText = @"select * from vScheduleTrackingReport 
                                    where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                                    + buildSearchCriteria();
                CCMSUI.CCMS.DsScheduleTrackingRpt Ds = new CCMSUI.CCMS.DsScheduleTrackingRpt();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["ScheduleTracking"]);
                
                PopulateDataTable(Ds.Tables["ScheduleTracking"]);
                
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=ScheduleTracking.rpt&ReportTitle=Schedule Tracking Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;
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
                region_id = int.Parse(dt.Rows[i][7].ToString()); //parent

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
                dt.Rows[i][5] = region.RegionName;
                dt.Rows[i][6] = region.BankLogo;
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
                                                          where report_name='ScheduleTrackingReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='ScheduleTrackingReport' and organization_id = " + orgID, trxn);
                      
                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "ScheduleTrackingReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"ScheduleTracking.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Schedule Tracking Report";
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