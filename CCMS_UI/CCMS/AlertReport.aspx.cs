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
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

using System.Text;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class AlertReport : BasePage
    {


        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.AlertMonitoringReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;


                

                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


                DropDownList_type.Items.Add("Any");
                AlertType.AlertTypeReader alertTypeReader = AlertType.ExecuteReader("1=1");
                while (alertTypeReader.Read())
                    DropDownList_type.Items.Add(new ListItem(alertTypeReader.CurrentAlertType.AlertTypeName, alertTypeReader.CurrentAlertType.AlertTypeId.ToString()));
                alertTypeReader.Close();



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
                region_id = int.Parse(dt.Rows[i][3].ToString()); //parent

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
                dt.Rows[i][1] = region.RegionName;
                dt.Rows[i][4] = region.BankLogo;

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
                cmd.CommandText = @"
select region_name,region.region_id, parent_region_id, title, bank_logo,alert_type_name,generated_at,resolve_at,generate_at_retry_remaining,alert_msg alert_text,
resolve_at_retry_remaining,generate_notification_sent,resolve_notification_sent,failure_reason,atm.location from 
vAtmAlert,atm,alert_type,region
where atm.atm_id = vAtmAlert.atm_id 
and vAtmAlert.alert_type_id = alert_type.alert_type_id
and atm.region_id = region.region_id
                    and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                                           + Filter();

                dsAlerts Ds = new dsAlerts();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["DataTable1"]);

                PopulateDataTable(Ds.Tables["DataTable1"]);

                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables["DataTable2"]);

                if (Ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=Alerts.rpt&ReportTitle=Alert Monitoring Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count;

                    Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }
        }
        protected string Filter()
        {
            string filter = "";
            

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and generated_at >= convert(datetime, '" + TextBox_FromDate.Text + "', 103) ";
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and generated_at <= convert(datetime, '" + TextBox_ToDate.Text + "', 103) ";
            if (DropDownList_type.SelectedIndex > 0)
                filter += " and alert_type.alert_type_id = " + DropDownList_type.SelectedValue;


            return filter;
        }


        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            if (DropDownList_type.Items.Count > 0)
            {
                DropDownList_type.SelectedIndex = 0;
                
            }

        }


        protected string SpaceCreator(string alertTypeName)
        {
            StringBuilder builder = new StringBuilder(100);
            builder.Append(alertTypeName[0]);

            for (int counter = 1; counter < alertTypeName.Length; counter++)
            {
                if (char.IsUpper(alertTypeName, counter) && char.IsLower(alertTypeName, counter - 1))
                    builder.Append(" " + alertTypeName[counter]);
                else
                    builder.Append(alertTypeName[counter]);
            }

            return builder.ToString();
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
                                                          where report_name='AlertsReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='AlertsReport' and organization_id = " + orgID, trxn);
              
                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "AlertsReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"Alerts.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Alerts Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = 1;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        reportSchedule.IsEjEnabled = false;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_error.Text = "";
                        Utility.BuildAuditLog("Schedule added for Alert Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.AlertMonitoringReport);
                    }
                    else
                        Literal_error.Text = "Please select organization from treeview.";
                }
                else
                    Literal_error.Text = "Please select organization from treeview.";
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