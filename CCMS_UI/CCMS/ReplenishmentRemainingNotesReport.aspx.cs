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
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using Avanza.iSuite.UI;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class ReplenishmentRemainingNotesReport : BasePage
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                DataSet Ds = null;
                AppUser objUser = (AppUser)this.Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                if (Request.QueryString["ej"] == "1")
                    cmd.CommandText = string.Concat(new object[] { "select ej_summary.atm_id,region.region_name,region.region_id, parent_region_id, title,trxn_datetime,\r\ncash_added1,cash_added2,cash_added3,cash_added4,return_type1,return_type2,return_type3,return_type4 ,replenishment_amount total_cashAdded,return_amount total_returned,atm.location \r\n                      from atm, ej_summary, region  where atm.atm_id = ej_summary.atm_id and atm.region_id = region.region_id and replenishment_amount>0  and ATM.atm_id in (", this.Session[SessionVars.SelectedATMs.ToString()], ") ", this.buildSearchCriteria() });
                else
                {
                    cmd.CommandText = "GetReplenishmentRemainingNotes";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AtmId", SqlDbType.VarChar));
                    cmd.Parameters[0].Value = Session[SessionVars.SelectedATMs.ToString()];
                    cmd.Parameters.Add(new SqlParameter("@NoteSetTypeId", SqlDbType.Int));
                    if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                        cmd.Parameters[1].Value = int.Parse(DropDownList_NoteSetTypeID.SelectedItem.Value);
                    else
                        cmd.Parameters[1].Value = 0;
                    //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    //DataTable dt = new DataTable();
                    //adapter.Fill(dt);

                }
                Ds = new dsReplenishmentRemainingNotes();
                new SqlDataAdapter(cmd).Fill(Ds.Tables[0]);
                this.PopulateDataTable(Ds.Tables[0]);
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    string URL = null;
                    this.Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    URL = string.Concat(new object[] { "ReportPopup.aspx?report=ReplenishmentRemainingNotes.rpt&ReportTitle=Current Replenishment Remaining Notes Report&FromDate=", Utility.GetDateTime(this.TextBox_FromDate.Text), "&ToDate=", Utility.GetDateTime(this.TextBox_ToDate.Text), "&GeneratedBy=", objUser.UserLogin, "&total=", Ds.Tables[0].Rows.Count });
                    this.Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                {
                    this.Literal_ok.Text = "<li>No record(s) found</li>";
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_errorMsg.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }
        }

        private string buildSearchCriteria()
        {
            string filter = "";
            if (this.TextBox_FromDate.Text.Length > 0)
            {
                filter = filter + " and trxn_datetime >= convert(datetime,'" + this.TextBox_FromDate.Text + "',103)";
            }
            if (this.TextBox_ToDate.Text.Length > 0)
            {
                filter += filter + " and trxn_datetime <= convert(datetime,'" + this.TextBox_ToDate.Text + " 23:59:59',103)";
            }
            if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                filter += " and note_set_type_id = " + DropDownList_NoteSetTypeID.SelectedItem.Value;
            return filter;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!this.Page.IsPostBack)
                {
                    if (!base.CheckRight((int)Permissions.ViewReplenishedRemainingNotesReport))
                    {
                        base.Response.Redirect("../admin/AccessDenied.aspx");
                    }
                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;

                    this.TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    this.TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
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
                //dt.Rows[i][2] = region.BankLogo;

            }

        }
        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            ReportSchedule reportSchedule = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            int ejEnabled = 0;
            try
            {
                if (Request.QueryString["ej"] == "1")
                    ejEnabled = 1;
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
                                                          where report_name='ReplenishmentRemainingNotesReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='ReplenishmentRemainingNotesReport' and is_ej_enabled=" + ejEnabled + " and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "ReplenishmentRemainingNotesReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"ReplenishmentRemainingNotes.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Current Replenishment Remaining Notes Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = 1;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        if (ejEnabled == 1)
                            reportSchedule.IsEjEnabled = true;
                        else
                            reportSchedule.IsEjEnabled = false;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_errorMsg.Text = "";
                        Utility.BuildAuditLog("Schedule added for Replenishment Remaining Notes Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewReplenishedRemainingNotesReport);
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