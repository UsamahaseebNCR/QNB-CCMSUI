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


namespace Avanza.iSuite.UI
{
    public partial class ViewGLSummary : BasePage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                //if (!CheckRight((int)Permissions.ViewGLAccount))
                //    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "reconciliation_gl_account_name";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        string Filter()
        {
            string filter = "";
            if (TextBox_GLAccountName.Text.Length > 0)
                filter += " and reconciliation_gl_account_name like '%" + TextBox_GLAccountName.Text + "%'";

            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT reconciliation_gl_account_name,reconciliation_gl_account_summary.opening_balance,closing_balance,title from 
reconciliation_gl_account_summary inner join atm on atm.atm_id =reconciliation_gl_account_summary.atm_id 
inner join reconciliation_gl_account on reconciliation_gl_account.reconciliation_gl_account_id = reconciliation_gl_account_summary.reconciliation_gl_account_id where  reconciliation_batch_id =  " + Request.QueryString["r"] + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtGLAccounts = new DataTable();
                adapter.Fill(dtGLAccounts);
                cmd.Connection.Close();
                this.GridView_GLSummary.DataSource = dtGLAccounts; ;
                this.GridView_GLSummary.DataBind();
                Literal_count.Text = dtGLAccounts.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = ex.Message;
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_GLSummary_Sorting(object sender, GridViewSortEventArgs e)
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



        protected void GridView_GLSummary_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_GLSummary.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_GLAccountName.Text = "";
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


    }
}