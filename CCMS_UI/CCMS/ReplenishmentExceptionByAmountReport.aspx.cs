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
    public partial class ReplenishmentExceptionByAmountReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ReplenishmentExceptionWithoutOrderReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                //if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                //    Button_GenerateReportSchedule.Enabled = false;

                ViewState["SortExpression"] = "title";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
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
                cmd.CommandText = @"select cash_added1,cash_added2,cash_added3,cash_added4,
                                    atm.atm_id,title,r.region_id,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.parent_region_id,
                                    rep_datetime,
                                    cash_added1*note_set_type.denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+cash_added4*denomination_type_4+cash_added5*denomination_type_5
                                    +cash_added6*denomination_type_6+cash_added7*denomination_type_7 total_rep,atm.location,
                                    cassette1_suggested_notes,cassette2_suggested_notes,cassette3_suggested_notes,cassette4_suggested_notes,
                                    cash_order_datetime,
                                    cassette1_suggested_notes*note_set_type.denomination_type_1+
                                    cassette2_suggested_notes*note_set_type.denomination_type_2+
                                    cassette3_suggested_notes*note_set_type.denomination_type_3+
                                    cassette4_suggested_notes*note_set_type.denomination_type_4 suggested_amount 
                                    from replenishment 
                                    inner join atm on replenishment.atm_id = atm.atm_id 
                                    inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id 
                                    inner join region r on atm.region_id = r.region_id 
                                    inner join cash_orders on replenishment.cash_order_id = cash_orders.cash_order_id
                                    where rep_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103) " + 
                                    " and rep_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " +
                                    " and ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")" +
                                    " and atm.IS_ACTIVE =1 and " +
                                    " (cash_added1*note_set_type.denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+cash_added4*denomination_type_4+cash_added5*denomination_type_5 " +
                                    " +cash_added6*denomination_type_6+cash_added7*denomination_type_7 " +
                                        "<> " +
                                        " cassette1_suggested_notes*note_set_type.denomination_type_1+ " +
                                        " cassette2_suggested_notes*note_set_type.denomination_type_2+ " +
                                    " cassette3_suggested_notes*note_set_type.denomination_type_3+ " +
                                    " cassette4_suggested_notes*note_set_type.denomination_type_4)";

                CCMSUI.CCMS.DsSuspiciousReplenishmentRpt Ds = new CCMSUI.CCMS.DsSuspiciousReplenishmentRpt();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(Ds.Tables["SuspiciousReplenishment"]);
                PopulateDataTable(Ds.Tables["SuspiciousReplenishment"]);

                if (Ds.Tables[0].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=RepExceptionByAmountReport.rpt&ReportTitle=Replenishment Exception Report - By Amount&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

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
                region_id = int.Parse(dt.Rows[i][26].ToString()); //parent

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
                Region region = Region.LoadRegionByPk(id);
                dt.Rows[i][5] = region.RegionName;
                dt.Rows[i][6] = region.BankLogo;
            }
        }
    }
}