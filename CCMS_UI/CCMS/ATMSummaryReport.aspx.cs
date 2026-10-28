using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;

using System.Data;
using System.Web.Configuration;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class ATMSummaryReport : BasePage
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
                    //  DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
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
                if (!CheckRight((int)Permissions.ATMSummaryReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;


                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();
                DropDownList_ATMType.Items.Add(new ListItem("Any", "Any"));
                AddEditATM.PopulateATMTypes(DropDownList_ATMType);
                BindNoteSetType();
            }

        }
        string Filter()
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

            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and atm.creation_time >= convert(datetime, '" + TextBox_FromDate.Text + "', 103) ";
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and atm.creation_time <= convert(datetime, '" + TextBox_ToDate.Text + "', 103) ";
            if (DropDownList_creator.SelectedIndex > 0)
                filter += " and atm.created_by = " + DropDownList_creator.SelectedValue;
            if (DropDownList_ATMType.SelectedIndex > 0)
                filter += " and atm.atm_type = '" + DropDownList_ATMType.SelectedValue + "'";

            if (groupbuilder.Length > 0)
                filter += " and atm.note_set_type_id in ( " + groupbuilder.ToString() + " ) ";
            if (DropDownList_status.SelectedValue == "1")
                filter += " and atm.is_active =1 and atm.atm_id in ("
                        + Session[SessionVars.SelectedATMs.ToString()] + ")";
            else if (DropDownList_status.SelectedValue == "0")
            {
                filter += " and atm.is_active = 0 and atm.atm_id in (select atm_id from user_ATMS where user_id=" + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ")";
            }
            else
            {
                filter += " and atm.atm_id in (select atm_id from user_ATMS where user_id=" + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ")";
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
                region_id = int.Parse(dt.Rows[i][6].ToString()); //parent

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
                dt.Rows[i][8] = region.BankLogo;

            }

        }
        protected void Button_search_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"
select atm.title, atm.ip,atm.location,atm.is_active status,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id, r.parent_region_id, bank_logo,atm.min_operating_balance,note_set_type_name
from atm,region r ,note_set_type
where  atm.region_id = r.region_id and atm.note_set_type_id = note_set_type.note_set_type_id  " + Filter();

                dsATMSummary Ds = new dsATMSummary();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["DataTable1"]);

                PopulateDataTable(Ds.Tables["DataTable1"]);

                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables["DataTable2"]);

                if (Ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=ATMSummaryRpt.rpt&ReportTitle=ATM Summary Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count;

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

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            DropDownList_creator.SelectedIndex = 0;
            DropDownList_status.SelectedIndex = 0;
            DropDownList_ATMType.SelectedIndex = 0;
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
                                                          where report_name='ATMSummaryReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='ATMSummaryReport' and organization_id = " + orgID, trxn);
                       
                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "ATMSummaryReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"ATMSummaryRpt.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "ATM Summary Report";
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
                        Utility.BuildAuditLog("Schedule added for ATM summary report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ATMSummaryReport);

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