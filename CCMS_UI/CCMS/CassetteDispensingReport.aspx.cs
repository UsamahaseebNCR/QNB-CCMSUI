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
using System.Configuration;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class CassetteDispensingReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.CassetteDispensingReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;


                TextBox_FromDate.Text = DateTime.Now.ToString();

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
        private void ProcessDataTable(string columnName, DataSet ds, DataTable dt, List<int> atmIDs)
        {
            DataRow[] dr = null;
            foreach (int atmID in atmIDs)
            {
                object result = ds.Tables["DataTable1"].Compute("sum(" + columnName + ")", "atm_id=" + atmID);
                if (result != DBNull.Value)
                {
                    int val = int.Parse(result.ToString());
                    if (val == 0)
                    {
                        dr = ds.Tables["DataTable1"].Select(columnName + "=0 and atm_id=" + atmID);
                        int atmTotalRecords = ds.Tables["DataTable1"].Select("atm_id=" + atmID).Length;
                        if (atmTotalRecords == dr.Length)
                            AddToDataTable(dr, dt);
                    }
                }
            }
        }
        private void AddToDataTable(DataRow[] src, DataTable dest)
        {
            foreach (DataRow row in src)
            {
                if (!dest.Rows.Contains(row["parsed_transaction_id"]))
                {
                    DataRow dr = dest.NewRow();
                    for (int i = 0; i < row.ItemArray.Length; i++)
                        dr[i] = row.ItemArray[i];

                    dest.Rows.Add(dr);
                }

            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;




            try
            {
                string[] supportedTypes = ConfigurationManager.AppSettings["ConfiguredCassettes"].Split(',');
                cmd = ConnectionFactory.GetNewCommand(false);
                if (Request.QueryString["ej"] == "1")
                    cmd.CommandText = @"select region.region_name,region.region_id, parent_region_id, title,trxn_datetime,
                                       notes_dispensed_type1 cash_dispensed1,notes_dispensed_type2 cash_dispensed2,notes_dispensed_type3 cash_dispensed3,
                                       notes_dispensed_type4 cash_dispensed4,amount,ej_parsed_transactions_id parsed_transaction_id,atm.atm_id,
                                        notes_remaining_type1 cash_remaining1,notes_remaining_type2 cash_remaining2,notes_remaining_type3 cash_remaining3,
                                        notes_remaining_type4 cash_remaining4,atm.location
                                        from atm, ej_parsed_transactions, region  where atm.atm_id = ej_parsed_transactions.atm_id and atm.region_id = region.region_id
                                        and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                                      " and trxn_datetime <= convert(datetime,'" + TextBox_FromDate.Text + "',103)  "+
                                       " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                else

                    cmd.CommandText = @"select region.region_name,region.region_id, parent_region_id, title,trxn_datetime,
                                        cash_dispensed1, cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,cash_dispensed7,amount,parsed_transaction_id,atm.atm_id,
                                        cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4,atm.location
                                        from atm, parsed_transaction, region  where atm.atm_id = parsed_transaction.atm_id and atm.region_id = region.region_id
                                        and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                                      " and trxn_datetime <= convert(datetime,'" + TextBox_FromDate.Text + "',103)  "+
                                      " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                DataSet ds = new dsCashWithdrawals();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds.Tables["DataTable1"]);
                //DataColumn dc = new DataColumn();
                //dc.ColumnName = "parsed_transaction_id";
                //dc.DataType = typeof(int);
                //DataColumn[] dcArray = new DataColumn[1];
                //dcArray[0] = dc;
                //ds.Tables["DataTable1"].PrimaryKey = dcArray;
                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {

                    List<int> atmIDs = new List<int>();
                    foreach (DataRow dr in ds.Tables["DataTable1"].Rows)
                    {
                        int atmID = int.Parse(dr["atm_id"].ToString());
                        if (!atmIDs.Contains(atmID))
                            atmIDs.Add(atmID);
                    }

                    DataSet temp = new DataSet();
                    DataTable dt = new DataTable();
                    dt = ds.Tables["DataTable1"].Clone();
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["parsed_transaction_id"] };

                    if (supportedTypes[0] == "1")
                        ProcessDataTable("cash_dispensed1", ds, dt, atmIDs);
                    if (supportedTypes[1] == "1")
                        ProcessDataTable("cash_dispensed2", ds, dt, atmIDs);
                    if (supportedTypes[2] == "1")
                        ProcessDataTable("cash_dispensed3", ds, dt, atmIDs);
                    if (supportedTypes[3] == "1")
                        ProcessDataTable("cash_dispensed4", ds, dt, atmIDs);


                    temp.Tables.Add(dt);
                    ds.Tables["DataTable1"].Rows.Clear();
                    ds = temp;
                }
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=CashWithdrawals.rpt&ReportTitle=Cassette Dispensing Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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
            TextBox_FromDate.Text = DateTime.Now.ToString();
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
                                                          where report_name='CassetteDispensingReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='CassetteDispensingReport' and is_ej_enabled=" + ejEnabled + " and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "CassetteDispensingReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"CashWithdrawals.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Cassette Dispensing Report";
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
                        Literal_error.Text = "";
                        Utility.BuildAuditLog("Schedule added for Cassette Dispensing Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CassetteDispensingReport);
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