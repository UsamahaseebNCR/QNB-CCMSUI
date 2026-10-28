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

using Avanza.CCMS.DAL;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Avanza.iSuite.UI;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class SwitchDispensingReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.SwitchDispensingReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;

                    TextBox_FromDate.Text = DateTime.Today.ToString();
                    TextBox_ToDate.Text = DateTime.Now.ToString();
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
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime <= convert(datetime,'" + TextBox_ToDate.Text + "',103)";

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
                Region region = Region.LoadRegionByPk(id);
                dt.Rows[i][8] = region.RegionName;
                dt.Rows[i][5] = region.BankLogo;

            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                // Data Archiving Work
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                if (Request.QueryString["ej"] == "1")
                    cmd.CommandText = @"select title,location,amount denomination,count(ej_parsed_transactions_id) count
                                    from atm inner join vEjParsedTransactions
                                    on vEjParsedTransactions.atm_id = atm.ATM_id
                                    and amount > 0 and 
                                   + ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + buildSearchCriteria() + " group by title,location,amount";
                else
                    cmd.CommandText = @"select title,location,amount denomination,count(parsed_transaction_id) count
                                    from atm inner join vParsedTransaction
                                    on vParsedTransaction.atm_id = atm.ATM_id
                                    and amount > 0 and 
                                   + ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + buildSearchCriteria() + " group by title,location,amount";


                DsDispensing ds = new DsDispensing();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.Tables["DataTable1"]);
                if (Request.QueryString["ej"] == "1")
                    cmd.CommandText = @"select title,
notes_dispensed_type1 cash_dispensed1, notes_dispensed_type1 cash_dispensed2,notes_dispensed_type1 cash_dispensed3, notes_dispensed_type1 cash_dispensed4,
notes_dispensed_type5 cash_dispensed5,notes_dispensed_type6 cash_dispensed6,notes_dispensed_type7 cash_dispensed7,
amount,
notes_remaining_type1 cash_remaining1,notes_remaining_type1 cash_remaining2,notes_remaining_type1 cash_remaining3,notes_remaining_type1 cash_remaining4

                        from atm, vEjParsedTransactions, region  where atm.atm_id = vEjParsedTransactions.atm_id and atm.region_id = region.region_id and 
 + ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and status = 0 and amount is not null" + buildSearchCriteria();
                else
                    cmd.CommandText = @"select title,
cash_dispensed1, cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,cash_dispensed7,amount,
cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4

                        from atm, vParsedTransaction, region  where atm.atm_id = vParsedTransaction.atm_id and atm.region_id = region.region_id and 
 + ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + buildSearchCriteria();

                DataTable dt = new DataTable();
                //DSCashUtilization Ds = new DSCashUtilization();
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                DataTable dtDistinct = dt.DefaultView.ToTable(true, "amount");
                if (!CheckBox_ByATM.Checked)
                {
                    foreach (DataRow dr in dtDistinct.Rows)
                    {
                        string amount = dr[0].ToString();
                        DataRow[] drArray = dt.Select("amount=" + amount);
                        foreach (DataRow obj in drArray)
                        {
                            DataRow[] result = ds.Tables["DataTable2"].Select("cash_dispensed1=" + obj[1] + " and cash_dispensed2=" + obj[2] + " and cash_dispensed3=" + obj[3] +
                                " and cash_dispensed4=" + obj[4]);
                            if (result.Length == 0)
                            {
                                DataRow newRow = ds.Tables["DataTable2"].NewRow();
                                newRow["title"] = obj[0];
                                newRow["denomination"] = amount.Split(new char[] { '.', ',' })[0];
                                newRow["cash_dispensed1"] = obj[1];
                                newRow["cash_dispensed2"] = obj[2];
                                newRow["cash_dispensed3"] = obj[3];
                                newRow["cash_dispensed4"] = obj[4];
                                newRow["remaining1"] = obj[9];
                                newRow["remaining2"] = obj[10];
                                newRow["remaining3"] = obj[11];
                                newRow["remaining4"] = obj[12];
                                ds.Tables["DataTable2"].Rows.Add(newRow);
                            }
                        }
                    }
                }
                else
                {
                    DataTable dtDistinctATMs = dt.DefaultView.ToTable(true, "title");
                    foreach (DataRow drATM in dtDistinctATMs.Rows)
                    {
                        foreach (DataRow dr in dtDistinct.Rows)
                        {
                            string amount = dr[0].ToString();
                            DataRow[] drArray = dt.Select("amount=" + amount + " and title='" + drATM[0] + "'");
                            foreach (DataRow obj in drArray)
                            {
                                DataRow[] result = ds.Tables["DataTable2"].Select("cash_dispensed1=" + obj[1] + " and cash_dispensed2=" + obj[2] + " and cash_dispensed3=" + obj[3] +
                                    " and cash_dispensed4=" + obj[4] + " and title='" + drATM[0] + "'");
                                if (result.Length == 0)
                                {
                                    DataRow newRow = ds.Tables["DataTable2"].NewRow();
                                    newRow["title"] = obj[0];
                                    newRow["denomination"] = amount.Split(new char[] { '.', ',' })[0];
                                    newRow["cash_dispensed1"] = obj[1];
                                    newRow["cash_dispensed2"] = obj[2];
                                    newRow["cash_dispensed3"] = obj[3];
                                    newRow["cash_dispensed4"] = obj[4];
                                    newRow["remaining1"] = obj[9];
                                    newRow["remaining2"] = obj[10];
                                    newRow["remaining3"] = obj[11];
                                    newRow["remaining4"] = obj[12];
                                    ds.Tables["DataTable2"].Rows.Add(newRow);
                                }
                            }
                        }
                    }
                }
                Session["subReport"] = ds;

                //PopulateDataTable(Ds.Tables["Summary"]);

                //({Summary.replenishment_amount}-{Summary.return_amount})/{Summary.replenishment_amount}*100




                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables["DataTable2"]);

                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    string URL = null;
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    if (CheckBox_ByATM.Checked)
                        URL = "ReportPopup.aspx?Report=SwitchDispensingMainByATM.rpt&ReportTitle=Switch Dispensed Logic Report&FromDate=" + TextBox_FromDate.Text + "&ToDate=" + TextBox_ToDate.Text + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable2"].Rows.Count;
                    else
                        //    URL = "ReportPopup.aspx?Report=SwitchDispensingMain.rpt&ReportTitle=Switch Dispensed Logic Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable2"].Rows.Count;
                        URL = "ReportPopup.aspx?Report=SwitchDispensingMain.rpt&ReportTitle=Switch Dispensed Logic Report&FromDate=" + TextBox_FromDate.Text + "&ToDate=" + TextBox_ToDate.Text + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable2"].Rows.Count;

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
                                                          where report_name='SwitchDispensingReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='SwitchDispensingReport' and is_ej_enabled=" + ejEnabled + " and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "SwitchDispensingReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"SwitchDispensingMain.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Switch Dispensing Report";
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
                        Utility.BuildAuditLog("Schedule added for Switch Dispensing Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.SwitchDispensingReport);

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