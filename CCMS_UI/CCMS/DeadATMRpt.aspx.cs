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
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class DeadATMRpt : BasePage
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
                if (!CheckRight((int)Permissions.DeadATMsReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");
 
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
                region_id = int.Parse(dt.Rows[i][9].ToString()); //parent


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
                dt.Rows[i][8] = region.RegionName;
                //dt.Rows[i][4] = region.BankLogo;

            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                string filter = "";
                if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                    filter += " and outerATM.note_set_type_id = " + DropDownList_NoteSetTypeID.SelectedItem.Value;

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;


                if (RadioButtonList_DeadCriteria.SelectedIndex == 0)
                    // Data Archiving Work
                    cmd.CommandText = @"select outerATM.location,outerATM.title,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,(select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) heart_beat_received_at ,outerATM.last_ping_executed_at,outerATM.last_ping_status,outerATM.last_telnet_executed_at,last_telnet_status,r.parent_region_id,r.region_id
                                        from atm outerATM inner join region r on outerATM.region_id = r.region_id
                                        where outerATM.is_active = 1 and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                                              + " and ATM_id not in (select ATM_id from vHeartBeat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                            "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " +
                                            " and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" + " ) " + filter;
                else
                {
                    if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                        filter += " and outerATM.atm_id in (select ATM_id from vHeartBeat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                             "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103))";

                    cmd.CommandText = @"select outerATM.location,outerATM.title,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,(select max(heart_beat_received_at) from vHeartBeat where atm_id = outerATM.atm_id) heart_beat_received_at ,outerATM.last_ping_executed_at,outerATM.last_ping_status,outerATM.last_telnet_executed_at,last_telnet_status,r.parent_region_id,r.region_id
                                        from atm outerATM inner join region r on outerATM.region_id = r.region_id
                                        where outerATM.is_active = 1 and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                                              + " and outerATM.atm_id in (select atm_id from vTask where creation_time >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                            "creation_time <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and status = 'downloadingDisconnected'" +
                                            " and atm_id = outerATM.atm_id  ) " + filter;

                }
                DataSetDeadAtm ds = new DataSetDeadAtm();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.Tables[0]);

                PopulateDataTable(ds.Tables["dtDeadATMs"]);

                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables["DataTable2"]);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=DeadATMsReport.rpt&ReportTitle=Dead ATMs Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["dtDeadATMs"].Rows.Count;
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
         protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            
            

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
                                                          where report_name='DeadATMsReport' and organization_id = " + orgID + "); " +

                                                          " delete report_schedule where report_name='DeadATMsReport' and organization_id = " + orgID, trxn);

                         reportSchedule = new ReportSchedule();
                         reportSchedule.ReportName = "DeadATMsReport";
                         reportSchedule.ReportPhysicalPath = Server.MapPath(@"DeadATMsReport.rpt");
                         reportSchedule.ReportReceipients = "";
                         AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                         if (appSetting != null)
                             reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                         else
                             reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                         reportSchedule.RetryCount = 5;
                         reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                         reportSchedule.ReportFriendlyName = "Dead ATMs Report";
                         reportSchedule.MinutesToScheduleAgain = 1440;
                         reportSchedule.ReportExportType = 3;
                         reportSchedule.ReportDataAge = 1;
                         reportSchedule.ScheduleType = false;
                         reportSchedule.OrganizationId = int.Parse(orgID);
                         reportSchedule.Save(trxn.Connection, trxn);
                         trxn.Commit();
                         Literal_ok.Text = "Schedule added successfully";
                         Literal_error.Text = "";
                         Utility.BuildAuditLog("Schedule added for Dead ATMs report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.LowBalanceReport);
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