using Avanza.iSuite.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI
{
    public partial class TransactionsSummary : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewOutOfBalanceInvestigation))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "trxn_datetime";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                refresh_ServerClick(this, EventArgs.Empty);

            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(null, null);
        }

        protected void GridView_DispensedSummary_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_DispensedSummary.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_DepositSummary_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_DepositSummary.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_DispensedSummary_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_DepositSummary_Sorting(object sender, GridViewSortEventArgs e)
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


        string Filter()
        {
            StringBuilder builder = new StringBuilder();

            if (Session[SessionVars.SelectedATMs.ToString()] != null)
                builder.Append(" and atm1.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")");

            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" and trxn_datetime >= Convert(datetime,'" + TextBox_FromDate.Text + @"',103) ");
            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(" and trxn_datetime <= Convert(datetime,'" + TextBox_ToDate.Text + @" 23:59:59',103) ");

            return builder.ToString();
        }

        protected void GridView_DispensedSummary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           

        }
        protected void GridView_DepositSummary_RowDataBound(object sender, GridViewRowEventArgs e)
        {
          

        }
        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = null;
            DataTable dt = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);

                StringBuilder builder = new StringBuilder();
                // Data Archiving Work
                //                cmd.CommandText = @"select summary_id,title,trxn_datetime,closing_balance,withdrawals, pre_withdrawals,replenishment_amount,return_amount,isnull(opening_balance,0) opening_balance,isnull((
                //select note_set_type.denomination_type_1* cassette1_notes +note_set_type.denomination_type_2* cassette2_notes +
                //note_set_type.denomination_type_3* cassette3_notes +note_set_type.denomination_type_4* cassette4_notes 
                //from vCashPosition inner join atm on atm.ATM_id = vCashPosition.atm_id
                //inner join note_set_type on note_set_type.note_set_type_id = atm.note_set_type_id 
                //where atm.atm_id = Atm1.ATM_id and last_trxn_at >=outerSummary.trxn_datetime and last_trxn_at<=
                //CONVERT(datetime,CONVERT(varchar,outerSummary.trxn_datetime,103)+ ' 23:59:59',103)
                //),0)cash_pos_balance
                //                    from summary outerSummary,   atm Atm1
                //                    where outerSummary.atm_id = Atm1.atm_id " + Filter() + " order by " + orderby;

                BindDepositGrid();
                BindDispensedGrid();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                if (ex.Message.ToLower().Contains("invalid"))
                    throw;

            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        private void BindDispensedGrid()
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
            cmd.CommandText = "select atm_id[title], sum(cash_dispensed1)[cash_dispensed1],sum(cash_dispensed2)[cash_dispensed2],sum(cash_dispensed3)[cash_dispensed3],sum(cash_dispensed4)[cash_dispensed4] from parsed_transaction group by atm_id";




            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            literal_dispensedSummaryCount.Text = dt.Rows.Count + " records.";
            this.GridView_DispensedSummary.DataSource = dt;
            this.GridView_DispensedSummary.DataBind();
        }

        private void BindDepositGrid()
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
            cmd.CommandText = "select atm_id[title], sum(amount_authorized)[amount] from ej_parsed_bna_transaction group by atm_id";




            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            literal_depositSummaryCount.Text = dt.Rows.Count + " records.";
            this.GridView_DepositSummary.DataSource = dt;
            this.GridView_DepositSummary.DataBind();
        }
        protected void Button_search_Click(object sender, EventArgs e)
        {
            try
            {

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }


        }
        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());


        }
        public static string parseValueIntoCurrency(decimal number)
        {
            // set currency format
            string curCulture = Thread.CurrentThread.CurrentCulture.ToString();
            System.Globalization.NumberFormatInfo currencyFormat = new
                System.Globalization.CultureInfo(curCulture).NumberFormat;

            currencyFormat.CurrencyNegativePattern = 1;
            currencyFormat.CurrencySymbol = "";
            return number.ToString("c", currencyFormat);
        }

    }
}