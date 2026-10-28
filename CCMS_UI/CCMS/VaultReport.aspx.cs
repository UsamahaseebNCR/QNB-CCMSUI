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

namespace CCMSUI.CCMS
{
    public partial class VaultReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                  if (!CheckRight((int)Permissions.VaultReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");


                ViewState["SortExpression"] = "title";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Now.ToString();
                TextBox_ToDate.Text = DateTime.Now.ToString();


            }
        }

        private string Filter()
        {
            string temp = "";
            StringBuilder builder = new StringBuilder();
            foreach (ListItem item in CheckBoxList_TrxnType.Items)
            {
                builder.Append("'" + item.Text + "',");
            }
            if (builder.Length > 0)
            {
                temp = " and vault_transaction_type in (" + builder.Append("'Total'").ToString() + ")";

            }
            return temp;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;

                cmd.CommandText = @"select title,ledger_date,vault_transaction_type,total_quantity quantity, denomination_name denomination,total_value total,region_name organization,name
from ccms_vault_day_balance inner join  ccms_vault on ccms_vault_day_balance.vault_id = ccms_vault.id 
inner join region on region.region_id = ccms_vault.organization_id 
inner join ccms_Cit on ccms_cit.id = ccms_vault.cit_id
where ccms_vault.is_deleted = 0 and ledger_date >= convert(datetime,'" + TextBox_FromDate.Text + "',103) and ledger_date <= convert(datetime,'" + TextBox_ToDate.Text + "',103)" + Filter();

                CCMSUI.CCMS.dsVault Ds = new CCMSUI.CCMS.dsVault();


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables[0]);
                //PopulateDataTable(Ds.Tables[0]);



                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=VaultBalanceReport.rpt&ReportTitle=Vault Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

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
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
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
                        Utility.BuildAuditLog("Schedule added for Vault Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.VaultReport);

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