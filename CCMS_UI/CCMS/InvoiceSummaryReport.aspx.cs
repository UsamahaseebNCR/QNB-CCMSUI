using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Avanza.iSuite.DAL;
using Avanza.CCMS.DAL;
using System.Text;
using CCMSUI.CCMS;


namespace Avanza.iSuite.UI
{
    public partial class InvoiceSummaryReport : BasePage
    {

        private void PopulateInvoiceStatus()
        {
            DropDownList_Status.Items.Clear();
            DropDownList_Status.Items.Add("Select");
            CcmsInvoiceStatus.CcmsInvoiceStatusReader reader = CcmsInvoiceStatus.ExecuteReader("1=1");
            while (reader.Read())
                DropDownList_Status.Items.Add(new ListItem(reader.CurrentCcmsInvoiceStatus.Name, reader.CurrentCcmsInvoiceStatus.Id.ToString()));
            reader.Close();

        }

        private void PopulateCIT()
        {
            DropDownList_CIT.Items.Add(new ListItem("Any", "Any"));
            CcmsCit.CcmsCitReader CITReader = CcmsCit.ExecuteReader("1=1");
            while (CITReader.Read())
            {
                DropDownList_CIT.Items.Add(new ListItem(CITReader.CurrentCcmsCit.Name, CITReader.CurrentCcmsCit.Id.ToString()));
            }
            CITReader.Close();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewInvoiceSummaryReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                PopulateInvoiceStatus();
                PopulateCIT();
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
            try
            {
                StringBuilder builder = new StringBuilder();
                string invoiceIds = null;
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select name,remarks,ccms_invoice.code,ccms_invoice.created_on,region_name,status,cheque_date,cheque_number,ccms_invoice.id invoice_id,ccms_invoice.amount,ccms_invoice.invoice_date
                                    from ccms_invoice 
                                    inner join ccms_cit on ccms_invoice.cit_id = ccms_cit.id 
                                    inner join region on organization_id  = region.region_id " + Filter();

                dsInvoice Ds = new dsInvoice();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["DataTable1"]);

                if (Ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=InvoiceSummaryRpt.rpt&ReportTitle=Invoice Summary Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count;

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
        protected string Filter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and ccms_invoice.created_on >= convert(datetime, '" + TextBox_FromDate.Text + "', 103) ";
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and ccms_invoice.created_on <= convert(datetime, '" + TextBox_ToDate.Text + " 23:59:59', 103) ";
            if (DropDownList_Status.SelectedIndex > 0)
                filter += " and status = '" + DropDownList_Status.SelectedItem.Text + "'";
            if (DropDownList_CIT.SelectedIndex > 0)
                filter += " and ccms_cit.id =" + DropDownList_CIT.SelectedItem.Value;
            return filter;
        }


        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            if (DropDownList_Status.Items.Count > 0)
                DropDownList_Status.SelectedIndex = 0;
        }


        protected string SpaceCreator(string alertTypeName)
        {
            StringBuilder builder = new StringBuilder(100);
            builder.Append(alertTypeName[0]);

            for (int counter = 1; counter < alertTypeName.Length; counter++)
            {
                if (char.IsUpper(alertTypeName, counter) && char.IsLower(alertTypeName, counter - 1))
                    builder.Append(" " + alertTypeName[counter]);
                else
                    builder.Append(alertTypeName[counter]);
            }

            return builder.ToString();
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
                                                          where report_name='AlertsReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='AlertsReport' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "AlertsReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"Alerts.rpt");
                        reportSchedule.ReportReceipients = "";
                        reportSchedule.ReportTempPath = @"c:\ccmsLiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Alerts Report";
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