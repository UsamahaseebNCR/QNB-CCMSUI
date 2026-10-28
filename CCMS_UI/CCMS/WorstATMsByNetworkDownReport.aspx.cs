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
using Microsoft.Win32;

namespace CCMSUI.CCMS
{
    public partial class WorstATMsByNetworkDownReport : BasePage
    {
        string jpsDBName = System.Web.Configuration.WebConfigurationManager.AppSettings["jpsDBName"];

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ViewPowerNetworkDownReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    //if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    //    Button_GenerateReportSchedule.Enabled = false;


                    TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00";
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59";
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



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //SqlCommand cmd = GetCmdObject();
                StringBuilder builder = new StringBuilder();


                SqlConnection conn = ConnectionFactory.GetNewConnection();
                conn.Open();

                cmd = conn.CreateCommand();
                cmd.CommandText = "select title,city,location from atm where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                    builder.Append("'" + dr["title"] + "',");
                builder.Append("'-1'");

                conn.ChangeDatabase(jpsDBName);

                
                cmd.CommandTimeout = 100;
                cmd.CommandText = "GetWorstAtmsDownByCategory";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy HH:mm:ss", null);
                cmd.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy HH:mm:ss", null);
                /*TO DO ..organization is passed as int in stored procedure...Need to update this.*/
                cmd.Parameters.Add(new SqlParameter("@Category", SqlDbType.VarChar));
                cmd.Parameters[2].Value = "Network";
                cmd.Parameters.Add(new SqlParameter("@Percent", SqlDbType.Int));
                cmd.Parameters[3].Value = int.Parse(txtBoxCriticalAtmsInPercent.Text);
                /*TO DO ..organization is passed as int in stored procedure...Need to update this.*/
                cmd.Parameters.Add(new SqlParameter("@Offset", SqlDbType.Int));
                cmd.Parameters[4].Value = 5;
                //cmd.Parameters.Add(new SqlParameter("isDeadATMExcluded", SqlDbType.Int));
                //cmd.Parameters[3].Value = int.Parse(WebConfigurationManager.AppSettings["isDeadATMExcluded"]);
                //cmd.Parameters.Add(new SqlParameter("isDateTimeExcluded", SqlDbType.Bit));
                //cmd.Parameters[4].Value = 0;

                DataSet ds1 = new DataSet();
                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd);
                adapter1.Fill(ds1);

                foreach (DataRow dr in ds1.Tables[0].Rows)
                {
                    DataRow[] drArray = dt.Select("title = '" + dr["host"] + "'");
                    if (drArray.Length > 0)
                    {
                        dr["city"] = drArray[0]["city"];
                        dr["address"] = drArray[0]["location"];
                    }

                }
                //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "param2 added");
                CCMSUI.CCMS.DataSetPowerNetworkDown Ds = new CCMSUI.CCMS.DataSetPowerNetworkDown();
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.Fill(Ds);
                Ds.dtPowerNetworkDown.Merge(ds1.Tables[0]);
                Ds.dtTotalPowerNetworkDown.Merge(ds1.Tables[1]);


                

                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session["subReport"] = Ds.Tables[1].DataSet;
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=PowerNetworkDownRpt.rpt&ReportTitle=Worst Atms By Network Down Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["dtPowerNetworkDown"].Rows.Count;

                    //  Literal_ok.Text = "<script> try { window.open(\"" + URL + "\"); } catch { alert('Popup cannot be opened'); }</script>";
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
                region_id = int.Parse(dt.Rows[i][14].ToString()); //parent

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
                dt.Rows[i][11] = region.RegionName;
                dt.Rows[i][12] = region.BankLogo;

            }

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
                                                          where report_name='CassetteFaultySummaryReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='CassetteFaultySummaryReport' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "CassetteFaultySummaryReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"CassetteFaultySummary.rpt");
                        reportSchedule.ReportReceipients = "";
                        reportSchedule.ReportTempPath = @"c:\ccmsLiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Cassette Faulty Summary Report";
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
                        Utility.BuildAuditLog("Schedule added for Cassette Faulty Summary Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CassetteFaultySummaryReport);
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

        private SqlCommand GetCmdObject()
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            string connectionStr = (string)Registry.LocalMachine.OpenSubKey("SOFTWARE\\NCR\\JPS").GetValue("ConnectionString", "");
            connectionStr = Encryption.Cryptic.DecryptString(connectionStr);
            conn = new SqlConnection(connectionStr);
            conn.Open();
            cmd = conn.CreateCommand();

            return cmd;
        }

    }

}