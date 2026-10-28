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

namespace CCMSUI.CCMS
{
    public partial class DenominationUtilizationReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.CashUtilizationReport))
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
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

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

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select region.region_name,region.region_id, parent_region_id, title, trxn_datetime rep_datetime, replenishment_amount,
                                    return_amount , isnull(cash_added1,0) cash_added1,isnull(cash_added2,0) cash_added2,isnull(cash_added3,0) cash_added3,isnull(cash_added4,0) cash_added4,
                                    isnull(return_type1,0) cash_return1,isnull(return_type2,0) cash_return2,isnull(return_type3,0) cash_return3, 
                                isnull(return_type4,0) cash_return4
                        from atm, summary, region  where atm.atm_id = summary.atm_id and atm.region_id = region.region_id and 
 + ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] +") and replenishment_amount > 0 " + buildSearchCriteria() + " order by title,trxn_datetime asc";
                DSCashUtilization Ds = new DSCashUtilization();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["Summary"]);

                PopulateDataTable(Ds.Tables["Summary"]);

                //({Summary.replenishment_amount}-{Summary.return_amount})/{Summary.replenishment_amount}*100
               

              

                //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                //adapter.Fill(Ds.Tables["DataTable2"]);

                if (Ds.Tables["Summary"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=DenomUtilization.rpt&ReportTitle=Denomination Utilization Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["Summary"].Rows.Count;

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