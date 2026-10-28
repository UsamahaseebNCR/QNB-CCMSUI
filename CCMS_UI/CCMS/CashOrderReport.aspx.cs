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
using System.Collections.Generic;

namespace CCMSUI.CCMS
{
    public partial class CashOrderReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.CashOrderReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;


                    TextBox_fromdate.Text = DateTime.Today.ToString();
                    TextBox_toDate.Text = DateTime.Today.ToString();
                    AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                    DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                    while (userReader.Read())
                        DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                    userReader.Close();
                    CcmsCit.CcmsCitReader ccmsCit = CcmsCit.ExecuteReader("is_active = 1");
                    ListBox_CIT_Selection.Items.Add(new ListItem("Atms having no CIT", "0"));
                    while (ccmsCit.Read())
                        ListBox_CIT_Selection.Items.Add(new ListItem(ccmsCit.CurrentCcmsCit.Name, ccmsCit.CurrentCcmsCit.Id.ToString()));
                    ccmsCit.Close();
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

            if ((TextBox_fromdate.Text.Length > 0))
                selectQuery.Append(@" cash_order_datetime >= Convert(datetime,'" + TextBox_fromdate.Text + "',103) AND ");

            if ((TextBox_toDate.Text.Length > 0))
                selectQuery.Append(@" cash_order_datetime <=Convert(datetime,'" + TextBox_toDate.Text + "',103) AND ");


            if (DropDownList_creator.SelectedIndex > 0)
                selectQuery.Append(@" user_id = " + DropDownList_creator.SelectedValue + " AND ");

            if (DropDownList_repType.SelectedIndex > 0)
                selectQuery.Append(" cash_order_type= '" + DropDownList_repType.SelectedValue + "' and ");


            string ATMIds = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);
            selectQuery.Append(@" atm_id in (" + ATMIds + ") AND ");

            string query = selectQuery.ToString();
            query = query.Trim();

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

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select * from vCashOrderReport " + buildSearchCriteria();
                CCMSUI.CCMS.DsCashOrderRpt Ds = new CCMSUI.CCMS.DsCashOrderRpt();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["CashOrder"]);
                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables[1]);

                PopulateDataTable(Ds.Tables["CashOrder"]);

                /*    AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                    SqlCommand new_cmd = ConnectionFactory.GetNewCommand(true);
                    new_cmd.CommandText = "select den_value from denomination where den_name in ('dispense_type1','dispense_type2','dispense_type3','dispense_type4')";
                    SqlDataAdapter adap = new SqlDataAdapter(new_cmd);
                    DataTable dt = new DataTable();
                    adap.Fill(dt);*/

                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=CashOrder.rpt&ReportTitle=Cash Order Report&FromDate=" + Utility.GetDateTime(TextBox_fromdate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_toDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

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
                                                          where report_name like 'CashOrderReport%' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name like  'CashOrderReport%' and organization_id = " + orgID, trxn);

                        List<int> cit = new List<int>();
                        foreach (ListItem item in ListBox_CIT_Selection.Items)
                        {
                            if (item.Selected)
                            {
                                if (item.Value == "0")
                                {
                                    reportSchedule = new ReportSchedule();
                                    reportSchedule.ReportName = "CashOrderReport";
                                    reportSchedule.ReportPhysicalPath = Server.MapPath(@"CashOrder.rpt");
                                    reportSchedule.ReportReceipients = "";
                                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                                    if (appSetting != null)
                                        reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                                    else
                                        reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                                    reportSchedule.RetryCount = 5;
                                    reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                                    reportSchedule.ReportFriendlyName = "Cash Order Report";
                                    reportSchedule.MinutesToScheduleAgain = 1440;
                                    reportSchedule.ReportExportType = 3;
                                    reportSchedule.ReportDataAge = 1;
                                    reportSchedule.ScheduleType = false;
                                    reportSchedule.OrganizationId = int.Parse(orgID);
                                    reportSchedule.IsEjEnabled = false;
                                    reportSchedule.Save(trxn.Connection, trxn);
                                }
                                else
                                    cit.Add(Convert.ToInt32(item.Value));
                            }
                        }
                        {
                            Avanza.CCMS.DAL.CcmsCit.CcmsCitReader reader = Avanza.CCMS.DAL.CcmsCit.ExecuteReader(
                                "is_active = 1 AND id in (" + string.Join<int>(",", cit) + ")"
                                );
                            while (reader.Read())
                            {
                                reportSchedule = new ReportSchedule();
                                reportSchedule.ReportName = "CashOrderReport";
                                reportSchedule.ReportPhysicalPath = Server.MapPath(@"CashOrder.rpt");
                                reportSchedule.ReportReceipients = "";
                                AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                                if (appSetting != null)
                                    reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                                else
                                    reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                                reportSchedule.RetryCount = 5;
                                reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                                reportSchedule.ReportFriendlyName = "Cash Order Report";
                                reportSchedule.MinutesToScheduleAgain = 1440;
                                reportSchedule.ReportExportType = 3;
                                reportSchedule.ReportDataAge = 1;
                                reportSchedule.ScheduleType = false;
                                reportSchedule.OrganizationId = int.Parse(orgID);
                                reportSchedule.IsEjEnabled = false;
                                reportSchedule.CitId = reader.CurrentCcmsCit.Id;
                                reportSchedule.Save(trxn.Connection, trxn);
                            }
                            reader.Close();
                        }

                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_errorMsg.Text = "";
                        Utility.BuildAuditLog("Schedule added for Cash Order Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CashOrderReport);
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