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
using Avanza.iSuite.DAL;
using Avanza.CCMS.DAL;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Avanza.iSuite.UI;
using System.Web.Configuration;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class BNACounterReport : BasePage
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
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.BNACounterReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    BindNoteSetType();
           
                    
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
        protected void CheckBox_Current_CheckedChanged(object sender, EventArgs e)
        {

            TextBox_FromDate.Enabled = (!TextBox_FromDate.Enabled);
            TextBox_ToDate.Enabled = (!TextBox_ToDate.Enabled);

        }

        private string buildSearchCriteria()
        {
            string filter = "";
            if (!CheckBox_Current.Checked)
            {
                if (TextBox_FromDate.Text.Length > 0)
                    filter += " and last_bna_deposit_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) ";

                if (TextBox_ToDate.Text.Length > 0)
                    filter += " and last_bna_deposit_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";
            }
            else
            {
                filter += " and last_bna_deposit_at in (select max(last_bna_deposit_at) from vDepositPosition where atm_id = deposited_notes.atm_id)";
            }

            if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                filter += " and note_set_type_id = " + DropDownList_NoteSetTypeID.SelectedItem.Value;

            if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                filter += " and ATM_id in (select ATM_id from heart_beat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                     "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103))";

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
                region_id = int.Parse(dt.Rows[i][8].ToString()); //parent

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
                dt.Rows[i][10] = region.BankLogo;

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

                if (RadioButtonList_ReportType.SelectedIndex == 1)
                    cmd.CommandText = @"select convert(varchar,last_bna_deposit_at,3) last_bna_deposit_at,region_name,region_id, parent_region_id, title,location,
                    sum(bna_cassette1) bna_cassette1,sum(bna_cassette2) bna_cassette2,sum(bna_cassette3) bna_cassette3,sum(bna_cassette4) bna_cassette4,sum(bna_cassette5) bna_cassette5,count(atm_id) total_trxn 
                    ,denomination_type_1,denomination_type_2,denomination_type_3,denomination_type_4,sum(total) total from vDepositPosition as deposited_notes
               
                    where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                                               + buildSearchCriteria() + " group by convert(varchar,last_bna_deposit_at,3),region_name,region_id,parent_region_id,title,location,denomination_type_1,denomination_type_2,denomination_type_3,denomination_type_4 ";
                else

                    cmd.CommandText = @"select region_name,region_id, parent_region_id, title, bank_logo,location,
                    bna_cassette1,bna_cassette2,bna_cassette3,bna_cassette4,bna_cassette5,last_bna_deposit_at
                     from vDepositPosition as deposited_notes
                    where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                                               + buildSearchCriteria();
                DsBNACounter Ds = new DsBNACounter();
                dsBNACountsClearedReport dsBNACounterCleared = null;
                if (RadioButtonList_ReportType.SelectedIndex == 1)
                    dsBNACounterCleared = new dsBNACountsClearedReport();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(Ds.Tables["BNACounter"]);

                PopulateDataTable(Ds.Tables["BNACounter"]);

                if (RadioButtonList_ReportType.SelectedIndex == 1)
                {
                    //*******************************************************************************************************************************************************************************
                    //Change done on 23/11/2014
                    //Desc : There is no need for 'counts_cleared_at_date_only' column
                    //*******************************************************************************************************************************************************************************
                    string filter = "";
                    if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                        filter += " and atm.atm_id in (select ATM_id from heart_beat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                             "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103))";


                    cmd.CommandText = @"select title,outerBNA.counts_cleared_at 
               from bna_counts_cleared outerBNA,atm where outerBNA.atm_id = atm.atm_id ";
                    if (!CheckBox_Current.Checked)
                        cmd.CommandText += " and outerBNA.counts_cleared_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                        " and outerBNA.counts_cleared_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) ";
                    else
                    {
                        cmd.CommandText += " and outerBNA.counts_cleared_at in (select max(counts_cleared_at) from bna_counts_cleared where atm_id =outerBNA.atm_id )";
                    }


                    cmd.CommandText += " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" + filter;
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dsBNACounterCleared.Tables[0]);
                    Session["subReport"] = dsBNACounterCleared;

                }





                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables["DataTable2"]);

                if (Ds.Tables["BNACounter"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = null;
                    if (RadioButtonList_ReportType.SelectedIndex == 1)
                        URL = "ReportPopup.aspx?Report=BNACounterSummaryRpt.rpt&ReportTitle=BNA Counter Summary Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["BNACounter"].Rows.Count;
                    else
                        URL = "ReportPopup.aspx?Report=BNACounterRpt.rpt&ReportTitle=BNA Counter Detail Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["BNACounter"].Rows.Count;

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
                reportName = "BNACounterDetailReport";
                friendlyName = "BNA Counter Detail Report";
                rptName = "BNACounterRpt.rpt";

            }
            else
            {
                reportName = "BNACounterSummaryReport";
                friendlyName = "BNA Counter Summary Report";
                rptName = "BNACounterSummaryRpt.rpt";
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
                        reportSchedule.ApplicableNoteSetType = GetNoteSetTypeId();
                        if (reportSchedule.ApplicableNoteSetType == "-1")
                            reportSchedule.ApplicableNoteSetType = null;        

                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_errorMsg.Text = "";
                        Utility.BuildAuditLog("Schedule added for " + friendlyName, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.BNACounterReport);

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