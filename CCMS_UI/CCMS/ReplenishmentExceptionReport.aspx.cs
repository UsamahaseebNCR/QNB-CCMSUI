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
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using Avanza.CCMS.DAL;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class ReplenishmentExceptionReport : BasePage
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
                    //  DropDownList_N.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
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
                if (!CheckRight((int)Permissions.ReplenishmentExceptionReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;


                ViewState["SortExpression"] = "title";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Now.ToString();
                TextBox_ToDate.Text = DateTime.Now.ToString();
                BindNoteSetType();
               
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
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
                string filter = "";
                if (groupbuilder.Length > 0)
                    filter = " and atm.note_set_type_id in ( " + groupbuilder.ToString() + " ) ";
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;

                if (Request.QueryString["ej"] == "1")
                    cmd.CommandText = @"select notes_added_type1 cash_added1, notes_added_type2 cash_added2, notes_added_type3 cash_added3,notes_added_type4 cash_added4,
                                        0 cash_added5,0 cash_added6, 0 cash_added7, 1 is_swap, -1 cash_order_id,
                                        atm.atm_id,title,region.region_id,region.region_name,parent_region_id,ip,
                                        rep_datetime,'Normal' reason,
                                        notes_added_type1*note_set_type.denomination_type_1+notes_added_type2*denomination_type_2+notes_added_type3*denomination_type_3+notes_added_type4*denomination_type_4 total_rep,atm.location
                                        from ej_parsed_replenishments 
                                        inner join atm on ej_parsed_replenishments.atm_id = atm.atm_id 
                                        inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id 
                                        inner join region on atm.region_id = region.region_id 
                                        where rep_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                                        "and rep_datetime <= convert(datetime,'" + TextBox_ToDate.Text + "',103)  " +
                                        " and ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
                                        " and atm.IS_ACTIVE =1 ";

                else
                    cmd.CommandText = @"select cash_added1,cash_added2,cash_added3,cash_added4,
                                        atm.atm_id,title,r.region_id,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.parent_region_id,
                                        rep_datetime,
                                        cash_added1*note_set_type.denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+cash_added4*denomination_type_4+cash_added5*denomination_type_5
                                        +cash_added6*denomination_type_6+cash_added7*denomination_type_7 total_rep,atm.location,
                                        cassette1_suggested_notes,cassette2_suggested_notes,cassette3_suggested_notes,cassette4_suggested_notes,
                                        cash_order_datetime,
                                        cassette1_suggested_notes*note_set_type.denomination_type_1+
                                        cassette2_suggested_notes*note_set_type.denomination_type_2+
                                        cassette3_suggested_notes*note_set_type.denomination_type_3+
                                        cassette4_suggested_notes*note_set_type.denomination_type_4 suggested_amount 
                                        from replenishment 
                                        inner join atm on replenishment.atm_id = atm.atm_id 
                                        inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id 
                                        inner join region r on atm.region_id = r.region_id 
                                        inner join cash_orders on replenishment.cash_order_id = cash_orders.cash_order_id
                                        where rep_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " + filter +
                                        " and rep_datetime <= convert(datetime,'" + TextBox_ToDate.Text + "',103)  " +
                                        " and ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
                                        " and atm.IS_ACTIVE =1 and (convert(varchar,rep_datetime,103) <> convert(varchar,cash_order_datetime,103) "+
                                        " or "+
                                        " cash_added1*note_set_type.denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+cash_added4*denomination_type_4+cash_added5*denomination_type_5 " +
                                        " +cash_added6*denomination_type_6+cash_added7*denomination_type_7 " +
                                         "<> " + 
                                         " cassette1_suggested_notes*note_set_type.denomination_type_1+ " +
                                         " cassette2_suggested_notes*note_set_type.denomination_type_2+ " +
                                        " cassette3_suggested_notes*note_set_type.denomination_type_3+ " +
                                        " cassette4_suggested_notes*note_set_type.denomination_type_4)";


                CCMSUI.CCMS.DsSuspiciousReplenishmentRpt Ds = new CCMSUI.CCMS.DsSuspiciousReplenishmentRpt();


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["SuspiciousReplenishment"]);




                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables[1]);
                PopulateDataTable(Ds.Tables["SuspiciousReplenishment"]);



                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=RepExceptionReport.rpt&ReportTitle=Replenishment Exception Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

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
                region_id = int.Parse(dt.Rows[i][25].ToString()); //parent

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
                dt.Rows[i][5] = region.RegionName;
                dt.Rows[i][6] = region.BankLogo;

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
                                                          where report_name='ReplenishmentSummaryReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='ReplenishmentSummaryReport' and is_ej_enabled=" + ejEnabled + " and organization_id = " + orgID, trxn);
                       
                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "ReplenishmentSummaryReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"SuspiciousReplenishment.rpt");
                        reportSchedule.ReportReceipients = "";
                        reportSchedule.ReportTempPath = @"c:\ccmsLiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Replenishment Summary Report";
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
                        Utility.BuildAuditLog("Schedule added for Replenishment Exception Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ReplenishmentExceptionReport);
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