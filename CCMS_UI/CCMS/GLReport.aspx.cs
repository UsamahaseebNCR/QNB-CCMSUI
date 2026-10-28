using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data;
using System.Data.SqlClient;
using Avanza.iSuite.DAL;
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class GLReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.GLReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            }
        }

        //private string Filter()
        //{
        //    StringBuilder builder = new StringBuilder();
        //    if (DropDownList_Status.SelectedIndex > 0)
        //        builder.Append("and status = '" + DropDownList_Status.SelectedItem.Text + "'");

        //    /*if (dr["is_reconciled_manually"] == DBNull.Value && bool.Parse(dr["is_reconciled"].ToString()))
        //                    dataRow["mode"] = "Auto";
        //                else
        //                {
        //                    if (dr["is_reconciled_manually"] != DBNull.Value)
        //                    {
        //                        dataRow["mode"] = "Manual";
        //                    }
        //                }*/
        //    if (DropDownList_Mode.SelectedIndex ==1)
        //        builder.Append("and is_reconciled_manually is null and is_reconciled =1");
        //    else if (DropDownList_Mode.SelectedIndex == 2)
        //        builder.Append("and is_reconciled_manually is not null");

        //    return builder.ToString();
        // }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            
            try
            {
                
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select reconciliation_gl_account_name,reconciliation_gl_account_summary.opening_balance,closing_balance,reconciliation_gl_account_summary.creation_time,title
                                    from reconciliation_gl_account_summary inner join reconciliation_gl_account
                                    on  reconciliation_gl_account_summary.reconciliation_gl_account_id =  reconciliation_gl_account.reconciliation_gl_account_id 
                                    inner join atm on reconciliation_gl_account_summary.atm_id = atm.atm_id 
                                    where reconciliation_gl_account_summary.creation_time>=convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                                    " and reconciliation_gl_account_summary.creation_time<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) ";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dsGLSummary ds = new dsGLSummary();
                adapter.Fill(ds.Tables[0]);
                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=GLSummaryReport.rpt&ReportTitle=GL Summary Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        protected void CheckBox_Current_CheckedChanged(object sender, EventArgs e)
        {

            TextBox_FromDate.Enabled = (!TextBox_FromDate.Enabled);
            TextBox_ToDate.Enabled = (!TextBox_ToDate.Enabled);

        }
        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            //            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            //            ReportSchedule reportSchedule = null;
            //            SqlCommand cmd = null;
            //            SqlTransaction trxn = null;
            //            int reportAge = CheckBox_Current.Checked ? 0 : 1;
            //            try
            //            {
            //                if (Session["isOrgSelected"] != null)
            //                {
            //                    if (bool.Parse(Session["isOrgSelected"].ToString()))
            //                    {
            //                        cmd = ConnectionFactory.GetNewCommand(true);
            //                        trxn = cmd.Connection.BeginTransaction();


            //                        string orgID = Session["selectedOrgID"].ToString();
            //                        ConnectionFactory.ExecuteScalar(@"delete report_generation_schedule
            //                                                          where report_schedule_id in
            //                                                          (select  report_schedule_id 
            //                                                          from report_schedule 
            //                                                          where report_name='LowBalanceReport' and organization_id = " + orgID + "); " +

            //                                                         " delete report_schedule where report_name='LowBalanceReport' and report_data_age=" + reportAge + " and organization_id = " + orgID, trxn);

            //                        reportSchedule = new ReportSchedule();
            //                        reportSchedule.ReportName = "LowBalanceReport";
            //                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"lowbalancereport.rpt");
            //                        reportSchedule.ReportReceipients = "";
            //                        reportSchedule.ReportTempPath = @"c:\ccmsLiveData\temp\";
            //                        reportSchedule.RetryCount = 5;
            //                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
            //                        reportSchedule.ReportFriendlyName = "Low Balance Report";
            //                        reportSchedule.MinutesToScheduleAgain = 1440;
            //                        reportSchedule.ReportExportType = 3;
            //                        reportSchedule.ReportDataAge = reportAge;
            //                        reportSchedule.ScheduleType = false;
            //                        reportSchedule.OrganizationId = int.Parse(orgID);
            //                        reportSchedule.IsEjEnabled = false;
            //                        reportSchedule.Save(trxn.Connection, trxn);
            //                        trxn.Commit();
            //                        Literal_ok.Text = "Schedule added successfully";
            //                        Literal_error.Text = "";
            //                    }
            //                    else
            //                        Literal_error.Text = "Please select organization from treeview.";
            //                }
            //                else
            //                    Literal_error.Text = "Please select organization from treeview.";
            //            }

            //            catch (Exception ex)
            //            {
            //                if (trxn != null)
            //                    trxn.Rollback();
            //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);

            //            }
            //            finally
            //            {
            //                task.EndTask();
            //                if (cmd != null)
            //                    if (cmd.Connection != null)
            //                        cmd.Connection.Close();
            //            }
        }
     

       


       

    }
}