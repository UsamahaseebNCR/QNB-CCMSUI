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
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;

using System.Text;
using Avanza.CCMS.DAL;

namespace CCMSUI.CCMS
{
    public partial class VaultSummary : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewVaultSummary))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = " transaction_date";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        
            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            if (!((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                     Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0))
            {
                //TextBox_ip.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).IP;
                Button_search_Click(this, null);
            }
            else
            {
                //TextBox_ip.Text = "";
            }
            Button_search_Click(null, null);


        }

        protected void GridView_VaultBalanceInvestigation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_VaultBalanceInvestigation.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_VaultBalanceInvestigation_Sorting(object sender, GridViewSortEventArgs e)
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

            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" and transaction_date >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) ");
            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(" and transaction_date <= Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) ");

            return builder.ToString();
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = null;
            DataTable dt = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                 StringBuilder builder = new StringBuilder();
                 cmd.CommandText = @"select title,opening_balance,closing_balance,bulk_cash,cash_delivered_to_atm,cash_returned_to_atm,denomination_name,transaction_date,generated_at
                                    from CCMS_vault_day_balance_summary inner join CCMS_vault on CCMS_vault.id = ccms_vault_day_balance_summary.vault_id where 1=1 " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                Literal_summary.Text = dt.Rows.Count + " records.";
                this.GridView_VaultBalanceInvestigation.DataSource = dt;
                this.GridView_VaultBalanceInvestigation.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        }
        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
      
        }


    }
}