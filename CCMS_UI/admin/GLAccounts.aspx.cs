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
    public partial class GLAccounts : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (!CheckRight((int)Permissions.ViewGLAccount))
                    Response.Redirect("~/admin/AccessDenied.aspx");
                    

                if (!CheckRight((int)Permissions.EditGLAccount))
                    GridView_GLAccounts.Columns[4].Visible = false;

                if (!CheckRight((int)Permissions.DeleteGLAccount))
                    GridView_GLAccounts.Columns[5].Visible = false;


                HyperLink_newGLAccount.Visible = CheckRight((int)Permissions.CreateGLAccount);

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "reconciliation_gl_account_name";
                
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
            
        }
        string Filter()
        {
            string filter = "";
            if (TextBox_GLAccountName.Text.Length > 0)
                filter += " where reconciliation_gl_account_name like '%" + TextBox_GLAccountName.Text + "%'";

            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT reconciliation_gl_account_id,reconciliation_gl_account_name,opening_balance,creation_time,created_by,user_comments from reconciliation_gl_account  " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtGLAccounts = new DataTable();
                adapter.Fill(dtGLAccounts);
                cmd.Connection.Close();
                this.GridView_GLAccounts.DataSource = dtGLAccounts; ;
                this.GridView_GLAccounts.DataBind();
                Literal_count.Text = dtGLAccounts.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }

        protected void GridView_GLAccounts_Sorting(object sender, GridViewSortEventArgs e)
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


        protected void GridView_GLAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteGLAccount")
            {
                SqlCommand cmd = null;
                
                try
                {
                    task = LogableTask.NewTask("delete GL Account");
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "delete reconciliation_gl_account where reconciliation_gl_account_id =" + e.CommandArgument.ToString();
                    cmd.ExecuteNonQuery();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "GL Account deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting GL Account. " + ex.Message + "</li>";
                 }
                finally
                {
                    cmd.Connection.Close();
                    task.EndTask();

                }
            }
        }

        protected void GridView_GLAccounts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_GLAccounts.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_GLAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteGLAccount") != null)


                ((ImageButton)e.Row.FindControl("Button_deleteGLAccount")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete GL Account " + ((DataRowView)e.Row.DataItem).Row["reconciliation_gl_account_name"] + "?','CCMS')");


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