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
    public partial class newOutOfCash : BasePage
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
                while (reader.Read())
                {
                    // DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
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
                if (!CheckRight((int)Permissions.OutOfCashReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;


                TextBox_FromDate.Text = DateTime.Today.ToString();
                TextBox_ToDate.Text = DateTime.Now.ToString();
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
                string[] parts = dr["alert_msg"].ToString().Split(',');
                dr["type1"] = parts[0];
                dr["type2"] = parts[1];
                dr["type3"] = parts[2];
                dr["type4"] = parts[3];
                dr["balance"] = parts[7];
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            try
            {
                StringBuilder groupbuilder = new StringBuilder();
                foreach (ListItem item in groupList.Items)
                {
                    if (item.Selected)
                    {
                        if (groupbuilder.Length > 0)
                            groupbuilder.Append(",");
                        groupbuilder.Append(item.Value);
                    }

                }
                string filter = null;
                if (groupbuilder.Length > 0)
                    filter = " and outerATM.note_set_type_id in ( " + groupbuilder.ToString() + " ) ";

                if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                    filter += " and outerATM.atm_id in (select ATM_id from heart_beat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                         "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + "',103))";
     
                // Data Archiving Work
                cmd = ConnectionFactory.GetNewCommand(false);
                if (CheckBox_Current.Checked)
                    cmd.CommandText = @"
                                select ( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id, r.parent_region_id, outerATM.title, bank_logo,alert_type_name,atm_alert.generated_at,alert_msg,outerATM.location,
cash_added1,cash_added2,cash_added3,cash_added4,rep_datetime,(Select dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,
				  note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4,
				  cassette1_notes, cassette2_notes, cassette3_notes, cassette4_notes) from vCashPosition,atm , note_set_type
where vCashPosition.atm_id = atm.atm_id and atm.note_set_type_id = note_set_type.note_set_type_id and vCashPosition.atm_id = outerATM.atm_id
and last_trxn_at = (select max(last_trxn_at) from vCashPosition where atm_id = outerATM.atm_id)) currentBalance

                                from    
                                atm_alert,atm outerATM,alert_type,region r ,vReplenishment
                                where outerATM.atm_id = atm_alert.atm_id 
                                and outerATM.is_active=1
                                and atm_alert.alert_type_id = alert_type.alert_type_id
                                and outerATM.region_id = r.region_id
and vReplenishment.atm_id = outerATM.ATM_id 
                                and rep_datetime in (select max(rep_datetime) from vReplenishment
													where rep_datetime <=atm_alert.generated_at
                                                    and atm_id =atm_alert.atm_id )													                                
and atm_alert.generated_at in (select max(generated_at) from atm_alert where atm_id = outerATM.atm_id and alert_type_id = 21 and resolve_at is null) " +
                                 " and alert_type.alert_type_id = 21 and resolve_at is null "+
                                 " and (select note_set_type.denomination_type_1 * cassette1_notes + " +
                                " note_set_type.denomination_type_2 * cassette2_notes + " +
                                " note_set_type.denomination_type_3 * cassette3_notes + " +
                                " note_set_type.denomination_type_4 * cassette4_notes from vCashPosition,atm , note_set_type " +
                                " where vCashPosition.atm_id = atm.atm_id and atm.note_set_type_id = note_set_type.note_set_type_id and vCashPosition.atm_id = outerATM.atm_id " +
                                " and last_trxn_at = (select max(last_trxn_at) from vCashPosition where atm_id = outerATM.atm_id)) <= outerATM.out_of_cash_threshold" + 
                                
                        " and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + filter;
                                 

                else


                    cmd.CommandText = @"
                                select ( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id, r.parent_region_id, title, bank_logo,alert_type_name,atm_alert.generated_at,alert_msg,outerATM.location,
cash_added1,cash_added2,cash_added3,cash_added4,rep_datetime,(Select dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,
				  note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4,
				  cassette1_notes, cassette2_notes, cassette3_notes, cassette4_notes) from vCashPosition,atm , note_set_type
where vCashPosition.atm_id = atm.atm_id and atm.note_set_type_id = note_set_type.note_set_type_id and vCashPosition.atm_id = outerATM.atm_id
and last_trxn_at = (select max(last_trxn_at) from vCashPosition where atm_id = outerATM.atm_id)) currentBalance_text
                                from    
                                atm_alert,atm outerATM,alert_type,region r,vReplenishment
                                where outerATM.atm_id = atm_alert.atm_id 
                                and atm_alert.alert_type_id = alert_type.alert_type_id
                                and outerATM.region_id = r.region_id
and vReplenishment.atm_id = outerATM.ATM_id 
                                and rep_datetime in (select max(rep_datetime) from vReplenishment
													where rep_datetime <=atm_alert.generated_at
                                                    and atm_id = atm_alert.atm_id)													
                                and atm_alert.generated_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                                    "and atm_alert.generated_at <= convert(datetime,'" + TextBox_ToDate.Text + "',103)  " +
                                    " and alert_type.alert_type_id = 21 and resolve_at is null  "+
                                     " and (select note_set_type.denomination_type_1 * cassette1_notes + " +
                                " note_set_type.denomination_type_2 * cassette2_notes + " +
                                " note_set_type.denomination_type_3 * cassette3_notes + " +
                                " note_set_type.denomination_type_4 * cassette4_notes from vCashPosition,atm , note_set_type " +
                                " where vCashPosition.atm_id = atm.atm_id and atm.note_set_type_id = note_set_type.note_set_type_id and vCashPosition.atm_id = outerATM.atm_id " +
                                " and last_trxn_at = (select max(last_trxn_at) from vCashPosition where atm_id = outerATM.atm_id)) <= outerATM.out_of_cash_threshold" + 
                                
                                    " and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + filter;
                
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "query ->" + cmd.CommandText);

                DataSet ds = new dsAlerts();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.Tables["DataTable1"]);
                SplitAlertMessageIntoColumns(ds.Tables["DataTable1"]);
               
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=LowBalanceReport.rpt&ReportTitle=Out Of Cash Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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
            TextBox_FromDate.Text = DateTime.Now.ToString();
            TextBox_ToDate.Text = DateTime.Now.ToString();
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
            string reportName = null;
            int reportAge = CheckBox_Current.Checked ? 0 : 1;
            reportName = reportAge == 0 ? "CurrentOutOfCashReport" : "OutOfCashReport";
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

                                                         " delete report_schedule where report_name='" + reportName + "' and report_data_age=" + reportAge + " and organization_id = " + orgID, trxn);
    
                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = reportName;
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"lowbalancereport.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Out Of Cash Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = reportAge;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        reportSchedule.IsEjEnabled = false;
                        reportSchedule.ApplicableNoteSetType = GetNoteSetTypeId();
                        if (reportSchedule.ApplicableNoteSetType == "-1")
                            reportSchedule.ApplicableNoteSetType = null;                        
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_error.Text = "";
                        Utility.BuildAuditLog("Schedule added for Out Of Cash Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.OutOfCashReport);

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
        private string GetNoteSetTypeId()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ListItem chk in groupList.Items)
            {
                if (chk.Selected)
                    builder.Append(chk.Value + ",");
            }
            builder.Append("-1");
            return builder.ToString();
        }
    }
}