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
    public partial class CassetteFaultySummaryReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.CassetteFaultySummaryReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;


                    TextBox_fromdate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_toDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
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

        

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                cmd.CommandText = "GetCassetteFaultyMessages";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox_fromdate.Text,"dd/MM/yyyy",null);
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox_toDate.Text, "dd/MM/yyyy", null);
                /*TO DO ..organization is passed as int in stored procedure...Need to update this.*/
                cmd.Parameters.Add(new SqlParameter("orgID", SqlDbType.Int));
                cmd.Parameters[2].Value = 1000;
                cmd.Parameters.Add(new SqlParameter("isDeadATMExcluded", SqlDbType.Int));
                cmd.Parameters[3].Value = int.Parse(WebConfigurationManager.AppSettings["isDeadATMExcluded"]);
                cmd.Parameters.Add(new SqlParameter("isDateTimeExcluded", SqlDbType.Bit));
                cmd.Parameters[4].Value =0; 
           

                //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "param2 added");
                CCMSUI.CCMS.dsCassetteFaultySummary Ds = new CCMSUI.CCMS.dsCassetteFaultySummary();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables[0]);
                
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=CassetteFaultySummary.rpt&ReportTitle=Cassette Faulty Summary Report&FromDate=" + Utility.GetDateTime(TextBox_fromdate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_toDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count; 

                    //  Literal_ok.Text = "<script> try { window.open(\"" + URL + "\"); } catch { alert('Popup cannot be opened'); }</script>";
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
                region_id = int.Parse(dt.Rows[i][14].ToString()); //parent

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
                dt.Rows[i][11] = region.RegionName;
                dt.Rows[i][12] = region.BankLogo;

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
                                                          where report_name='CassetteFaultySummaryReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='CassetteFaultySummaryReport' and organization_id = " + orgID, trxn);
                        
                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "CassetteFaultySummaryReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"CassetteFaultySummary.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Cassette Faulty Summary Report";
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
                        Utility.BuildAuditLog("Schedule added for Cassette Faulty Summary Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CassetteFaultySummaryReport);
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