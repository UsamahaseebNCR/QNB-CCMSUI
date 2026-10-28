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
    public partial class CardCapturedReport : BasePage
    {
        bool isMultiInstitution = System.Web.Configuration.WebConfigurationManager.AppSettings["isMultiInstitution"] == "1" ? true : false;

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ViewCardCaptureReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");

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

        private string buildSearchCriteria()
        {
            StringBuilder selectQuery = new StringBuilder();

            if ((TextBox_FromDate.Text.Length > 0))
                selectQuery.Append(@" capture_time >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) AND");

            if ((TextBox_ToDate.Text.Length > 0))
                selectQuery.Append(@" capture_time <=Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) AND");


            if (TextBox_TSN.Text.Length > 0)
                selectQuery.Append(" tsn = '" + TextBox_TSN.Text + "' AND");

            if (TextBox_PAN.Text.Length > 0)
                selectQuery.Append(" PAN like '%" + TextBox_PAN.Text + "%' AND");


            string ATMIds = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);
            selectQuery.Append(@" atm.atm_id in (" + ATMIds + ") AND");

            string query = selectQuery.ToString();
            
            if (query.EndsWith("AND"))
                query = " where " + query.Substring(0, query.LastIndexOf("AND"));
            return query;
            /*else
                return " Where " + query;*/
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                Literal_errorMsg.Text = string.Empty;
                ValidateInput();
                

                    AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 100;
                    cmd.CommandText = @"select title,pan,tsn,capture_time captured_at,atm.location,region.region_id,parent_region_id,region_name,reason
                                    from ej_captured_card inner join atm 
                                    on ej_captured_card.atm_id = atm.atm_id 
                                    inner join region on atm.region_id = region.region_id
                                    " + buildSearchCriteria();
                    CCMSUI.CCMS.dsCardCaptured Ds = new CCMSUI.CCMS.dsCardCaptured();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(Ds.Tables[0]);
                    //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                    //adapter.Fill(Ds.Tables[1]);

                    PopulateDataTable(Ds.Tables[0]);

                    /*    AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                        SqlCommand new_cmd = ConnectionFactory.GetNewCommand(true);
                        new_cmd.CommandText = "select den_value from denomination where den_name in ('dispense_type1','dispense_type2','dispense_type3','dispense_type4')";
                        SqlDataAdapter adap = new SqlDataAdapter(new_cmd);
                        DataTable dt = new DataTable();
                        adap.Fill(dt);*/

                    if (Ds.Tables[0].Rows.Count > 0)
                    {
                        Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                        string URL = "ReportPopup.aspx?Report=CardCaptureRpt.rpt&ReportTitle=Card Capture Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

                        //  Literal_ok.Text = "<script> try { window.open(\"" + URL + "\"); } catch { alert('Popup cannot be opened'); }</script>";
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
            Region region = null;

            DataTable dtRegions = new DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region where parent_region_id is not null";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtRegions);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                region_id = int.Parse(dt.Rows[i][7].ToString()); //parent

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
                if (isMultiInstitution)
                {
                    region = Region.LoadRegionByPk(id);
                    dt.Rows[i][8] = region.RegionName;
                }
                else
                {
                    if (region == null)
                        region = Region.LoadRegionByPk(id);

                    dt.Rows[i][8] = region.RegionName;
                }
               // dt.Rows[i][12] = region.BankLogo;

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
                                                          where report_name='CardCaptureReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='CardCaptureReport' and organization_id = " + orgID, trxn);
                        
                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "CardCaptureReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"CardCaptureRpt.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Card Capture Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = 1;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        reportSchedule.IsEjEnabled = false;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_errorMsg.Text = "";
                        Utility.BuildAuditLog("Schedule added for Card Capture Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewCardCaptureReport);
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
       
        public void ValidateInput()
        {
            
            bool isTSNContainSpecialChars = CheckSpecialChars(TextBox_TSN.Text);
            if (isTSNContainSpecialChars == true)
            {
               throw new Exception("TSN contain invalid character");
                
            }
            bool isPANContainSpecialChars = CheckSpecialChars(TextBox_PAN.Text);
            if (isPANContainSpecialChars == true)
            {
               throw new Exception("PAN contain invalid character");
                
            }
            
        }

    }

}