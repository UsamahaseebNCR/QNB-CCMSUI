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
    public partial class MinNotesThresholdReport : BasePage
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
                if (!CheckRight((int)Permissions.LowBalanceReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
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
        private void SplitAlertMessageIntoColumns(DataTable dataTable)
        {
            foreach (DataRow dr in dataTable.Rows)
            {
                string[] parts = dr["alert_msg"].ToString().Split('|');
                dr["type1"] = parts[0];
                dr["type2"] = parts[1];
                dr["type3"] = parts[2];
                dr["type4"] = parts[3];
                dr["balance"] = parts[4];
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            try
            {
                string filter = null;
                if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                    filter = " and outerATM.note_set_type_id = " + DropDownList_NoteSetTypeID.SelectedItem.Value;

                if (DropDownList_TicketStatus.SelectedIndex > 0)
                {
                    if (DropDownList_TicketStatus.SelectedIndex == 1)
                    {
                        filter += " and resolve_at is null";
                    }
                    else if (DropDownList_TicketStatus.SelectedIndex == 2)
                    {
                        filter += " and resolve_at is not null";
                    }
                }

                cmd = ConnectionFactory.GetNewCommand(false);
                //                if (CheckBox_Current.Checked)
                //                    cmd.CommandText = @"
                //                                
                //                                select ( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id, r.parent_region_id, outerATM.title, bank_logo,alert_type_name,atm_alert.generated_at,alert_msg,
                //                                cash_added1,cash_added2,cash_added3,cash_added4,rep_datetime,outerATM.location,(select note_set_type.denomination_type_1 * cassette1_notes +
                //                                note_set_type.denomination_type_2 * cassette2_notes +
                //                                note_set_type.denomination_type_3 * cassette3_notes +
                //                                note_set_type.denomination_type_4 * cassette4_notes from cash_position,atm , note_set_type
                //                                where cash_position.atm_id = atm.atm_id and atm.note_set_type_id = note_set_type.note_set_type_id and cash_position.atm_id = outerATM.atm_id
                //                                and last_trxn_at = (select max(last_trxn_at) from cash_position where atm_id = outerATM.atm_id)) currentBalance
                //                                from 
                //                                atm_alert,atm outerATM,alert_type,region r,replenishment
                //                                where outerATM.atm_id = atm_alert.atm_id
                //                                and outerATM.is_active=1
                //                                and atm_alert.alert_type_id = alert_type.alert_type_id
                //                                and outerATM.region_id = r.region_id
                //                                and replenishment.atm_id = outerATM.ATM_id 
                //                                and rep_datetime in (select max(rep_datetime) from replenishment
                //								where rep_datetime <=atm_alert.generated_at
                //                                and atm_id =atm_alert.atm_id )													
                //                                and atm_alert.generated_at in (select max(generated_at) from atm_alert where atm_id = outerATM.atm_id and alert_type_id = 20 and resolve_at is null) " +
                //                                " and alert_type.alert_type_id = 20 and resolve_at is null " +
                //                                " and (select note_set_type.denomination_type_1 * cassette1_notes + " +
                //                                " note_set_type.denomination_type_2 * cassette2_notes + " +
                //                                " note_set_type.denomination_type_3 * cassette3_notes + " + 
                //                                " note_set_type.denomination_type_4 * cassette4_notes from cash_position,atm , note_set_type " + 
                //                                " where cash_position.atm_id = atm.atm_id and atm.note_set_type_id = note_set_type.note_set_type_id and cash_position.atm_id = outerATM.atm_id " +
                //                                " and last_trxn_at = (select max(last_trxn_at) from cash_position where atm_id = outerATM.atm_id)) <= outerATM.min_operating_balance" +
                //                                " and (select note_set_type.denomination_type_1 * cassette1_notes + " +
                //                                " note_set_type.denomination_type_2 * cassette2_notes + " +
                //                                " note_set_type.denomination_type_3 * cassette3_notes + " +
                //                                " note_set_type.denomination_type_4 * cassette4_notes from cash_position,atm , note_set_type " +
                //                                " where cash_position.atm_id = atm.atm_id and atm.note_set_type_id = note_set_type.note_set_type_id and cash_position.atm_id = outerATM.atm_id " +
                //                                " and last_trxn_at = (select max(last_trxn_at) from cash_position where atm_id = outerATM.atm_id)) > outerATM.out_of_cash_threshold" + 
                //                                " and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") "+filter;
                //                else

                // Data Archivng Work
                cmd.CommandText = @"
                                
                                select ( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id, r.parent_region_id, title, bank_logo,alert_type_name,vAtmAlert.generated_at,alert_msg,resolve_at,
                                outerATM.location  from 
                                vAtmAlert,atm outerATM,alert_type,region r
                                where outerATM.atm_id = vAtmAlert.atm_id                                
                                and vAtmAlert.alert_type_id = alert_type.alert_type_id
                                and outerATM.region_id = r.region_id
                                
                                and vAtmAlert.generated_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                            "and vAtmAlert.generated_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " +
                            " and alert_type.alert_type_id in (46,47,48,49) and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + filter;

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "query ->" + cmd.CommandText);
                DataSet ds = new dsAlerts();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.Tables["DataTable1"]);
                SplitAlertMessageIntoColumns(ds.Tables["DataTable1"]);
                PopulateDataTable(ds.Tables["DataTable1"]);
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=MinNotesThresholdRpt.rpt&ReportTitle=Min Notes Threshold Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        protected void CheckBox_Current_CheckedChanged(object sender, EventArgs e)
        {

            TextBox_FromDate.Enabled = (!TextBox_FromDate.Enabled);
            TextBox_ToDate.Enabled = (!TextBox_ToDate.Enabled);

        }
        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            ReportSchedule reportSchedule = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            int reportAge = 1;//CheckBox_Current.Checked ? 0 : 1;
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
                                                          where report_name='MinNotesThresholdReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='MinNotesThresholdReport' and report_data_age=" + reportAge + " and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "MinNotesThresholdReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"MinNotesThresholdRpt.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Min Notes Threshold Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = reportAge;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        reportSchedule.IsEjEnabled = false;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_error.Text = "";
                        Utility.BuildAuditLog("Schedule added for Min Notes Threshold Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.LowBalanceReport);
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