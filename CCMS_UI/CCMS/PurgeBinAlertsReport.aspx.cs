using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data;
using System.Data.SqlClient;

using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class PurgeBinAlertsReport : BasePage
    {
        private void BindNoteSetType()
        {
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && regionID != "1")
            {
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                while (reader.Read())
                {
                    DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
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
                if (!CheckRight((int)Permissions.PurgeBinAlertsReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;


                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                BindNoteSetType();
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
        private void SplitAlertMessageIntoColumnsForPurgeThreshold(DataTable dataTable)
        {
            // string[] ConfiguredCassettes = System.Configuration.ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(',');
            foreach (DataRow dr in dataTable.Rows)
            {
                //1,72,12, 2,47,12, 3,0,12, 4,17,14
                string[] parts = dr["alert_msg"].ToString().Split(',');
                for (int i = 0; i < parts.Length; i = i + 3)
                {
                    dr["type" + parts[i]] = parts[i + 1];
                    dr["threshold" + parts[i]] = parts[i + 2];
                }


                //if (parts[0] == "1")
                //{
                //    dr["type1"] = parts[1];
                //    dr["threshold1"] = parts[2];
                //}
                //if (parts[1] == "1")
                //{
                //    dr["type2"] = parts[4];
                //    dr["threshold2"] = parts[5];
                //}
                //if (parts[2] == "1")
                //{
                //    dr["type3"] = parts[7];
                //    dr["threshold3"] = parts[8];
                //}
                //if (parts[3] == "1")
                //{
                //    dr["type4"] = parts[10];
                //    dr["threshold4"] = parts[11];
                //}
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            try
            {
                // Data Archiving Work
                string filter = "";
                if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                    filter += " and atm.note_set_type_id = " + DropDownList_NoteSetTypeID.SelectedItem.Value;

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"
                                select ( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id, r.parent_region_id, title, bank_logo,alert_type_name,generated_at,alert_msg,atm.location from 
                                vAtmAlert,atm,alert_type,region r
                                where atm.atm_id = vAtmAlert.atm_id 
                                and vAtmAlert.alert_type_id = alert_type.alert_type_id
                                and atm.region_id = r.region_id
                                and vAtmAlert.generated_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                                "and vAtmAlert.generated_at <= convert(datetime,'" + TextBox_FromDate.Text + " 23:59:59',103)  " +
                                " and alert_type.alert_type_id = 22 " +
                                " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + filter;

                DataSet ds = new dsAlerts();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.Tables["DataTable1"]);
                //1,72,12, 2,47,12, 3,0,12, 4,17,14
                SplitAlertMessageIntoColumnsForPurgeThreshold(ds.Tables["DataTable1"]);

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=PurgeBinThresholdReport.rpt&ReportTitle=Purge Bin Threshold Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask();
            }
        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
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
                                                          where report_name='PurgeBinAlertsReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='PurgeBinAlertsReport' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "PurgeBinAlertsReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"PurgeBinThresholdReport.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Purge Bin Alerts Report";
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
                        Utility.BuildAuditLog("Schedule added for Purge Bin Alerts Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.PurgeBinAlertsReport);
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