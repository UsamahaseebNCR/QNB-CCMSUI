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

namespace CCMSUI.CCMS
{
    public partial class SettlementReport : BasePage
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
                dt.Rows[i][5] = region.RegionName;
               // dt.Rows[i][4] = region.BankLogo;

            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            string URL =null;
            try
            {
                
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                cmd = ConnectionFactory.GetNewCommand(false);
                if (Request.QueryString["t"]=="1")
                    cmd.CommandText = @"select title,atm.location,replenishment.rep_datetime,upload_datetime settlement_datetime,region_name,region.region_id, parent_region_id
                                    from atm  inner join  replenishment  on atm.atm_id = replenishment.atm_id 
                                    inner join region on region.region_id = atm.region_id
                                    left join atm_settlement on atm_settlement.rep_datetime =replenishment.rep_datetime 
                                    and  atm_settlement.atm_no = atm.cit_atm_title 
                                    where replenishment.rep_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                                    " and replenishment.rep_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and upload_datetime is null";
                else
                    cmd.CommandText = @"select title,atm.location,replenishment.rep_datetime,upload_datetime settlement_datetime,region_name,region.region_id, parent_region_id
                                    from atm  inner join  replenishment  on atm.atm_id = replenishment.atm_id 
                                    inner join region on region.region_id = atm.region_id                                    
                                    inner join atm_settlement on atm_settlement.rep_datetime =replenishment.rep_datetime 
                                    and  atm_settlement.atm_no = atm.cit_atm_title 
                                    where replenishment.rep_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                                    " and replenishment.rep_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and convert(datetime,replenishment.rep_datetime,103)<>convert(datetime,upload_datetime,103)";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dsSettlement ds = new dsSettlement();
                adapter.Fill(ds.Tables[0]);
                PopulateDataTable(ds.Tables["DataTable1"]);
                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    if (Request.QueryString["t"]=="1")
                         URL = "ReportPopup.aspx?Report=SettlementRpt.rpt&ReportTitle=Missing Settlements Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
                    else
                        URL = "ReportPopup.aspx?Report=SettlementRpt.rpt&ReportTitle=Late Settlements Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
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
            //                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
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