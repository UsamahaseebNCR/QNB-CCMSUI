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

namespace CCMSUI.CCMS
{
    public partial class FTPFileMonitoringReport : BasePage
    {
        private string GetFriendlyName(string systemFriendly)
        {
            switch (systemFriendly)
            {
                case "downloaded": return "Downloaded";
                case "scheduled": return "Scheduled";
                case "completed": return "Completed";
                case "failed": return "Failed";
                case "completedWithExceptions":
                    return "Completed With Exceptions";
                default: return "unKnown";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.FTPFileMonitoringReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    Array downloadStates = Enum.GetValues(typeof(FTPDownloadStatus));

                    DropDownList_status.Items.Add("Any");
                    for (int counter = 0; counter < downloadStates.Length; counter++)
                    {
                        string tempString = downloadStates.GetValue(counter).ToString();
                        string friendlyName = GetFriendlyName(tempString);
                        DropDownList_status.Items.Add(new ListItem(friendlyName, friendlyName));
                    }
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        private string buildSearchCriteria()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += "  creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103) and ";
         
            if (TextBox_ToDate.Text.Length > 0)
                filter += "  creation_time <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and ";

            if (TextBox_EndTimeFromDate.Text.Length > 0)
                filter += " end_time >= convert(datetime,'" + TextBox_EndTimeFromDate.Text + " ',103) and ";

            if (TextBox_EndTimeToDate.Text.Length > 0)
                filter += " end_time <= convert(datetime,'" + TextBox_EndTimeToDate.Text + " 23:59:59',103) and ";
            
            if (DropDownList_status.SelectedIndex > 0)
                filter += " status = '" + DropDownList_status.SelectedItem.Value + "' and ";

            if (DropDownList_TaskType.SelectedIndex > 0)
                filter += " task_type_id in (" + DropDownList_TaskType.SelectedItem.Value + ") and ";

            if (filter.EndsWith("and "))
                return " where " + filter.Substring(0, filter.LastIndexOf("and "));
            else
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
                cmd.CommandText = @"select * from vOrderDownloadAndDailyFeedReport " + buildSearchCriteria();


//                cmd.CommandText = @"select failure_reason, region.region_id, region.region_name, parent_region_id, F.creation_time creation_time, dbo.GetFTPStatusFriendlyName(status) status, task_type_name, f.end_time, f.last_invoked_at, f.retry_count
//                                    from ftp_file_info f, region, task_type t
//                                    where f.region_id = region.region_id
//                                    and f.task_type_id = t.task_type_id " + buildSearchCriteria();
                CCMSUI.CCMS.DsFTPFileMonitoringRpt Ds = new CCMSUI.CCMS.DsFTPFileMonitoringRpt();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["FTPFileMonitoring"]);
                
                PopulateDataTable(Ds.Tables["FTPFileMonitoring"]);
                
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=FTPFileMonitoring.rpt&ReportTitle=Order Download and Daily Feed Status Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;
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
                dt.Rows[i][9] = region.RegionName;
                dt.Rows[i][5] = region.BankLogo;
            }
        }
    }
}