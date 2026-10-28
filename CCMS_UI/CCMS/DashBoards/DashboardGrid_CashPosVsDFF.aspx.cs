using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;
using Avanza.CCMS.DAL;

using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashboardGrid_CashPosVsDFF : BasePage
    {
        int matched = 0;
        int posSuspect = 0;
        int negSuspect = 0;
        int swapMatched = 0;
        int swapPosSuspect = 0;
        int swapNegSuspect = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCashPositionVsDFF, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "title ";
                ViewState["sortDirection"] = "asc ";
                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                TextBox_FromDate.Text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            }
        }
        private void BindGrid(string orderBy)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");
            DateTime date = DateTime.ParseExact(TextBox_FromDate.Text,"dd/MM/yyyy",null);
            try
            {
                

                //metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                //Page.RegisterStartupScript("script", "<script>function initDashboard() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} initDashboard();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = string.Format(@"select top 2000 outerATM.atm_id,outerATM.title title, p.last_trxn_at,
( select max(rep_datetime) from replenishment where atm_id = outerATM.atm_id 
and rep_datetime<=convert(datetime,'{0} 23:59:59',103) )last_replenished_at, 
note_set_type.denomination_type_1 * (p.cassette1_notes + isnull(p.purge_cassette1_notes,0)) +
note_set_type.denomination_type_2 * (p.cassette2_notes + isnull(p.purge_cassette2_notes,0)) +
note_set_type.denomination_type_3 * (p.cassette3_notes + isnull(p.purge_cassette3_notes,0)) +
note_set_type.denomination_type_4 * (p.cassette4_notes + isnull(p.purge_cassette4_notes,0)) +
note_set_type.denomination_type_5 * (p.cassette5_notes + isnull(p.purge_cassette5_notes,0)) +
note_set_type.denomination_type_6 * (p.cassette6_notes + isnull(p.purge_cassette6_notes,0)) +
note_set_type.denomination_type_7 * (p.cassette7_notes + isnull(p.purge_cassette7_notes,0))  total,

(select closing_balance from summary where atm_id = outerATM.atm_id 
and trxn_datetime>=convert(datetime,CONVERT(varchar,p.last_trxn_at,103),103) and trxn_datetime<=convert(datetime,CONVERT(varchar,p.last_trxn_at,103) + ' 23:59:59',103)  ) closing_balance,

((select closing_balance from summary where atm_id = outerATM.atm_id 
and trxn_datetime>=convert(datetime,CONVERT(varchar,p.last_trxn_at,103),103) and trxn_datetime<=convert(datetime,CONVERT(varchar,p.last_trxn_at,103) + ' 23:59:59',103)  )
-
(note_set_type.denomination_type_1 * (p.cassette1_notes + isnull(p.purge_cassette1_notes,0)) +
note_set_type.denomination_type_2 * (p.cassette2_notes + isnull(p.purge_cassette2_notes,0)) +
note_set_type.denomination_type_3 * (p.cassette3_notes + isnull(p.purge_cassette3_notes,0)) +
note_set_type.denomination_type_4 * (p.cassette4_notes + isnull(p.purge_cassette4_notes,0)) +
note_set_type.denomination_type_5 * (p.cassette5_notes + isnull(p.purge_cassette5_notes,0)) +
note_set_type.denomination_type_6 * (p.cassette6_notes + isnull(p.purge_cassette6_notes,0)) +
note_set_type.denomination_type_7 * (p.cassette7_notes + isnull(p.purge_cassette7_notes,0)))) diff,

(
select sum(isnull(cash_purged1,0)) * note_set_type.denomination_type_1+
sum(isnull(cash_purged2,0))  * note_set_type.denomination_type_2+
sum(isnull(cash_purged3,0))  * note_set_type.denomination_type_3+
sum(isnull(cash_purged4,0))  * note_set_type.denomination_type_4+
sum(isnull(cash_purged5,0))  * note_set_type.denomination_type_5+
sum(isnull(cash_purged6,0)) * note_set_type.denomination_type_6+
sum(isnull(cash_purged7,0))  * note_set_type.denomination_type_7
 from test_cash_purged_notes where atm_id = outerATM.atm_id 
and test_cash_datetime>=(select max(rep_datetime) from replenishment where atm_id = outerATM.atm_id  
and rep_datetime<=convert(datetime,'{0} 23:59:59',103))  and test_cash_datetime<=convert(datetime,CONVERT(varchar,p.last_trxn_at,103) + ' 23:59:59',103)  ) test_cash_amount,case is_swap_default_replenishment when 0 then 'Add' when 1 then 'Swap' else 'N/A' end isSwap


 from atm outerATM , cash_position p,note_set_type
where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 and outerATM.exclude_dff = 0 
and outerATM.note_set_type_id = note_set_type.note_set_type_id 
and p.last_trxn_at >=convert(datetime,'" + date.ToString("dd/MM/yyyy") + "',103) and " +
" p.last_trxn_at <=convert(datetime,'" + date.ToString("dd/MM/yyyy") + " 23:59:59',103) " +
" and p.cash_position_id in (select max(cash_position_id) " +
 " from cash_position  where last_trxn_at >=convert(datetime,'" + date.ToString("dd/MM/yyyy") + "',103) and " +
" last_trxn_at <=convert(datetime,'" + date.ToString("dd/MM/yyyy") + " 23:59:59',103) " +
 " and cash_position.atm_id = outerATM.atm_id)  order by " + orderBy,date.ToString("dd/MM/yyyy"));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                dt.Columns.Add("title");
                dt.Columns.Add("last_trxn_at", typeof(DateTime));
                dt.Columns.Add("total", typeof(decimal));
                dt.Columns.Add("closing_balance", typeof(decimal));
                dt.Columns.Add("diff", typeof(decimal));
                dt.Columns.Add("test_cash_amount", typeof(decimal));
                adapter.Fill(dt);


                GridView_CashPositionVsDFF.DataSource = dt;
                GridView_CashPositionVsDFF.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();

                task.EndTask();
            }
        }
        protected void GridView_CashPositionVsDFF_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        }
        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        protected void GridView_CashPositionVsDFF_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //decimal diff = 0;
                //diff = decimal.Parse(e.Row.Cells[3].Text) - decimal.Parse(e.Row.Cells[2].Text);
                //e.Row.Cells[4].Text = diff.ToString();
                decimal diff = 0;
                decimal testCashAmount = 0;
                decimal total = 0;
                bool isConvertSuccessful1 = decimal.TryParse(e.Row.Cells[6].Text, out diff);
                bool isConvertSuccessful2 = decimal.TryParse(e.Row.Cells[7].Text, out testCashAmount);
                total = diff - testCashAmount;

                if (isConvertSuccessful1)
                {
                    if (total > 0)
                    {
                        if (e.Row.Cells[1].Text == "Swap")
                            swapPosSuspect++;

                        posSuspect++;
                        e.Row.BackColor = System.Drawing.Color.YellowGreen;
                        for (int i = 0; i < e.Row.Cells.Count; i++)
                            e.Row.Cells[i].ForeColor = System.Drawing.Color.White;
                    }
                    else if (total == 0)
                    {
                        matched++;
                        if (e.Row.Cells[1].Text == "Swap")
                            swapMatched++;
                    }
                    else
                    {
                        if (e.Row.Cells[1].Text == "Swap")
                            swapNegSuspect++;

                        negSuspect++;
                        e.Row.BackColor = System.Drawing.Color.Brown;
                        for (int i = 0; i < e.Row.Cells.Count; i++)
                            e.Row.Cells[i].ForeColor = System.Drawing.Color.White;
                        
                    }
                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                int total = matched + posSuspect + negSuspect;
                int totalSwap = swapMatched +swapPosSuspect + swapNegSuspect;

                if (total != 0)
                {
                    Label_Matched.Text = Math.Round((double)matched / total * 100, 2).ToString() + "%";
                    Label_PosSuspect.Text = Math.Round((double)posSuspect / total * 100, 2).ToString() + "%"; ;
                    Label_NegSuspect.Text = Math.Round((double)negSuspect / total * 100, 2).ToString() + "%"; ;
                }
                if (totalSwap != 0)
                {
                    Label_SwapMatched.Text = Math.Round((double)swapMatched / totalSwap * 100, 2).ToString() + "%";
                    Label_SwapPosSuspect.Text = Math.Round((double)swapPosSuspect / totalSwap * 100, 2).ToString() + "%"; ;
                    Label_SwapNegSuspect.Text = Math.Round((double)swapNegSuspect / totalSwap * 100, 2).ToString() + "%"; ;
                }
            }

        }
    }
}