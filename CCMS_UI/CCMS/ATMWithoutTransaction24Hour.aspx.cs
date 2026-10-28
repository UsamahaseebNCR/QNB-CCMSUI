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
    public partial class ATMWithoutTransaction24Hour : BasePage
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
                if (!CheckRight((int)Permissions.ATMWithoutTransaction24Hour))
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            try
            {
                string filter = "";

                if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                    filter = " and atm.note_set_type_id = " + DropDownList_NoteSetTypeID.SelectedItem.Value;
                //Change done on 5dec2016 by ik on soliman request

                //if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                //    filter += " and ATM_id in (select ATM_id from heart_beat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                //                         "heart_beat_received_at <=convert(datetime,'" + TextBox_FromDate.Text + " 23:59:59',103))";
           
                StringBuilder builder = new StringBuilder();
                cmd = ConnectionFactory.GetNewCommand(false);
                if (Request.QueryString["ej"] == "1")
                    cmd.CommandText = "select atm_id from atm where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and is_ej_enabled=1 and  atm_id not in (select ATM_id from ej_parsed_transactions where trxn_datetime >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                        " trxn_datetime <=convert(datetime,'" + TextBox_FromDate.Text + " 23:59:59',103) " +
                                           " and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")) " + filter;
                else
                    cmd.CommandText = "select atm_id from atm where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and  atm_id not in (select ATM_id from parsed_transaction where trxn_datetime >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                        " trxn_datetime <=convert(datetime,'" + TextBox_FromDate.Text + " 23:59:59',103) " +
                                           " and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and amount>0 ) " + filter;
                //and is_counter_enabled=1 
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    builder.Append(reader.GetInt32(0) + ",");
                }
                builder.Append("-1");

                reader.Close();

                // Data Archiving Work

                string atmWithoutTrxnToday = builder.ToString();
                string[] parts = atmWithoutTrxnToday.Split(',');
                if (Request.QueryString["ej"] == "1")
                    cmd.CommandText = @"select outerATM.atm_id,outerATM.title,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,trxn_datetime last_trxn_at,amount,outerATM.location,outerATM.allowed_inactivity_period
                                    from atm outerATM left join vEjParsedTransactions on outerATM.atm_id =vEjParsedTransactions.atm_id 
                                    inner join region r on  outerATM.region_id = r.region_id 
                                    where outerATM.is_active = 1 and outerATM.atm_id in (" + atmWithoutTrxnToday + ")" +
                                    " and trxn_datetime in (select max(trxn_datetime) from vEjParsedTransactions " +
                                                                                 " where vEjParsedTransactions.atm_id= outerATM.atm_id)";
                else
                    cmd.CommandText = @"select outerATM.atm_id,outerATM.title,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,trxn_datetime last_trxn_at,amount,outerATM.location,outerATM.allowed_inactivity_period,description
                                    from atm outerATM left join vParsedTransaction on outerATM.atm_id =vParsedTransaction.atm_id 
                                    inner join region r on  outerATM.region_id = r.region_id 
                                    where outerATM.is_active = 1 and outerATM.atm_id in (" + atmWithoutTrxnToday + ")" +
                                    " and trxn_datetime in (select max(trxn_datetime) from vParsedTransaction " +
                                                                                 " where vParsedTransaction.atm_id= outerATM.atm_id and amount>0)";
                dsNoCashWithdrawals ds = new dsNoCashWithdrawals();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.Tables["DataTable1"]);
                builder = new StringBuilder();

                for (int i = 0; i < parts.Length; i++)
                {
                    DataRow[] dr = ds.Tables["DataTable1"].Select("atm_id=" + parts[i]);
                    if (dr.Length == 0)
                    {
                        builder.Append(parts[i] + ",");
                    }
                }
                builder.Append("-1");
                cmd.CommandText = "select title,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,atm.location,allowed_inactivity_period from atm inner join region r on atm.region_id =r.region_id  where atm_id in (" + builder.ToString() + ")";
                DataTable temp = new DataTable();
                adapter.Fill(temp);
                ds.Tables["DataTable1"].Merge(temp);

                ds.Tables["DataTable1"].DefaultView.Sort = "title asc";





                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                 

                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=ATMWithoutWithdrawal.rpt&ReportTitle=No Withdrawal&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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
                                                          where report_name='ATMWithoutTrxn24Hour' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='ATMWithoutTrxn24Hour' and is_ej_enabled="+ejEnabled+" and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "ATMWithoutTrxn24Hour";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"atmwithoutwithdrawal.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "ATM With No Withdrawal during current day";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = 1;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        if (ejEnabled==1)
                            reportSchedule.IsEjEnabled = true;
                        else
                            reportSchedule.IsEjEnabled = false;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_error.Text = "";
                        Utility.BuildAuditLog("Schedule added for ATM With No Withdrawal during current day", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ATMWithoutTransaction24Hour);
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