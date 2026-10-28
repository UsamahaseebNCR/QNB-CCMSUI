using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;
using System.Data.SqlClient;
using Avanza.iSuite.DAL;

namespace CCMSUI.CCMS
{
    public partial class CallOutReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.CallOutReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
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
                filter += " and deposit_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) ";


            if (TextBox_ToDate.Text.Length > 0)
                filter += " and deposit_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";


            return filter;
        }
        //private void PopulateDataTable(DataTable dt)
        //{
        //    int region_id = 0;
        //    int id = -1;

        //    DataTable dtRegions = new DataTable();
        //    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
        //    cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region where parent_region_id is not null";
        //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //    adapter.Fill(dtRegions);



        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        region_id = int.Parse(dt.Rows[i][7].ToString()); //parent

        //        while (true)
        //        {
        //            DataRow[] innerArray = dtRegions.Select("region_id=" + region_id);
        //            if (bool.Parse(innerArray[0][2].ToString()))
        //            {
        //                id = int.Parse(innerArray[0][0].ToString());
        //                break;
        //            }
        //            else
        //                region_id = int.Parse(innerArray[0][1].ToString());

        //        }
        //        //organization id found
        //        Region region = Region.LoadRegionByPk(id);
        //        dt.Rows[i][6] = region.RegionName;
        //        dt.Rows[i][9] = region.BankLogo;

        //    }

        //}
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;


                cmd.CommandText = @"select outerATM.title,outerATM.location, count(flm_monitoring_id) total_callout
,
(select count(replenishment_id) from replenishment 
                                    where (rep_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " + 
" and rep_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " + 
" and atm_id = outerATM.atm_id)) total_rep,  " +
" (select count(parsed_transaction_id) from parsed_transaction  "+
                                 "   where (trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " + 
" and trxn_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " + 
" and atm_id = outerATM.atm_id)) total_trxn  " +

" from atm outerATM inner join flm_monitoring " + 
" on flm_monitoring.atm_id_no = outerATM.cit_atm_title " + 
" where (activity_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " + 
" and activity_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)) " +
" group by title,location,outerATM.atm_id";

                DSCallOutSummary Ds = new DSCallOutSummary();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["DataTable1"]);


                cmd.CommandText = @"select title,location, type_of_problem,code,activity_datetime,time_of_info,time_of_reaching_site,time_of_solving
                                    from flm_monitoring inner join atm 
                                    on flm_monitoring.atm_id_no = atm.cit_atm_title
                                    where activity_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                " and activity_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " +
                " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
             
                
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["callout"]);

                
                cmd.CommandText = @"select title,location,rep_datetime,cash_added1,cash_added2,cash_added3,cash_added4
                                    from replenishment inner join atm on replenishment.atm_id = atm.atm_id 
                                    where rep_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " +
                " and rep_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " +
                " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["replenishment"]);

                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables["DataTable2"]);

                if (Ds.Tables["callout"].Rows.Count > 0 || Ds.Tables["replenishment"].Rows.Count>0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=CallOutSummaryReport.rpt&ReportTitle=Call Out Summary Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=1" ;                    
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
    }
}