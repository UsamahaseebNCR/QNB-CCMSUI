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

using Avanza.CCMS.DAL;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Avanza.iSuite.UI;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class CPMCounterReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.CPMCounterReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    //BindNoteSetType();
                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;
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
            if (!CheckBox_Current.Checked)
            {
                if (TextBox_FromDate.Text.Length > 0)
                    filter += " and deposit_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) ";


                if (TextBox_ToDate.Text.Length > 0)
                    filter += " and deposit_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";
            }
            else
            {
                filter += " and deposit_at in (select max(deposit_at) from parsed_cpm_counter where atm_id = outerCPM.atm_id)";
            }

            return filter;
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
                dt.Rows[i][6] = region.RegionName;
                dt.Rows[i][9] = region.BankLogo;

            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                // Data Archiving Work
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                if (RadioButtonList_ReportType.SelectedIndex == 1)
                    cmd.CommandText = @"select convert(varchar,deposit_at,3) last_cpm_deposit_at,region_name,region.region_id, parent_region_id, title,                   
                    sum(bin1) cpm_pkt_1,sum(bin2) cpm_pkt_2,sum(bin3) cpm_pkt_3,sum(bin4) cpm_pkt_4, atm.location from vParsedCPMCounter outerCPM
inner join atm on  outerCPM.atm_id = atm.atm_id inner join region on atm.region_id = region.region_id " +
                   " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                                               + buildSearchCriteria() + " group by convert(varchar,deposit_at,3),region_name,region.region_id, parent_region_id, title,atm.location";
                else

                    cmd.CommandText = @"select region_name,region.region_id, parent_region_id, title, bank_logo,
                    
                    bin1 cpm_pkt_1,bin2 cpm_pkt_2,bin3 cpm_pkt_3,bin4 cpm_pkt_4,deposit_at last_cpm_deposit_at, atm.location from vParsedCPMCounter outerCPM
inner join atm on  outerCPM.atm_id = atm.atm_id inner join region on atm.region_id = region.region_id 
                    where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                                               + buildSearchCriteria();

                DsCPMCounter Ds = new DsCPMCounter();
                //dsCPMCountsClearedReport dsCPMCounterCleared = null;
                //if (RadioButtonList_ReportType.SelectedIndex == 1)
                //    dsCPMCounterCleared = new dsCPMCountsClearedReport();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["CPMCounter"]);

                PopulateDataTable(Ds.Tables["CPMCounter"]);
                if (RadioButtonList_ReportType.SelectedIndex == 1)
                {
                    cmd.CommandText = @"select title,outerCPM.counts_cleared_at 
               from vCpmCountsCleared outerCPM,atm where outerCPM.atm_id = atm.atm_id ";
                    if (!CheckBox_Current.Checked)
                        cmd.CommandText += " and outerCPM.counts_cleared_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                        " and outerCPM.counts_cleared_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) ";
                    else
                    {
                        cmd.CommandText += " and outerCPM.counts_cleared_at in (select max(counts_cleared_at) from vCpmCountsCleared where atm_id =outerCPM.atm_id )";
                    }


                    cmd.CommandText += " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                    adapter = new SqlDataAdapter(cmd);
                    //adapter.Fill(dsCPMCounterCleared.Tables[0]);
                    //Session["subReport"] = dsCPMCounterCleared;

                }

                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables["DataTable2"]);

                if (Ds.Tables["CPMCounter"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = null;
                    if (RadioButtonList_ReportType.SelectedIndex == 1)
                        URL = "ReportPopup.aspx?Report=CPMCounterSummaryRpt.rpt&ReportTitle=CPM Counter Summary Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["CPMCounter"].Rows.Count;
                    else
                        URL = "ReportPopup.aspx?Report=CPMCounterRpt.rpt&ReportTitle=CPM Counter Detail Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["CPMCounter"].Rows.Count;

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
        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            ReportSchedule reportSchedule = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            string reportName = "";
            string friendlyName = "";
            string rptName = "";
            if (RadioButtonList_ReportType.SelectedIndex == 0)
            {
                reportName = "CPMCounterDetailReport";
                friendlyName = "CPM Counter Detail Report";
                rptName = "CPMCounterRpt.rpt";
            }

            else
            {
                reportName = "CPMCounterSummaryReport";
                friendlyName = "CPM Counter Summary Report";
                rptName = "CPMCounterSummaryRpt.rpt";
            }
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
                                                          where report_name='" + reportName + "' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='" + reportName + "' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = reportName;
                        reportSchedule.ReportPhysicalPath = Server.MapPath(rptName);
                        reportSchedule.ReportReceipients = "";
                        reportSchedule.ReportTempPath = @"c:\ccmsLiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = friendlyName;
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
                        Utility.BuildAuditLog("Schedule added for CPM Counter Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CPMCounterReport);
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

        protected void CheckBox_Current_CheckedChanged(object sender, EventArgs e)
        {

            TextBox_FromDate.Enabled = (!TextBox_FromDate.Enabled);
            TextBox_ToDate.Enabled = (!TextBox_ToDate.Enabled);

        }
    }
}