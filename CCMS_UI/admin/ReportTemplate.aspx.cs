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
using Avanza.iSuite.UI;

namespace CCMSUI.admin
{
    public partial class ReportTemplate : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "report_name";
            }

            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT report_id, report_name, is_active 
                                    FROM report_template 
                                    ORDER BY " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtATMs = new DataTable();
                adapter.Fill(dtATMs);
                cmd.Connection.Close();
                this.GridView_Templates.DataSource = dtATMs; ;
                this.GridView_Templates.DataBind();
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Templates_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_Templates_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editTemplate")
            {
                if (Request.QueryString["i"] == null)
                    Response.Redirect("AddEditATM.aspx?atm_id=" + e.CommandArgument);
                else
                    Response.Redirect("AddEditATM.aspx?atm_id=" + e.CommandArgument + "&i=1");
            }
        }

        protected void GridView_Templates_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Templates.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Templates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteATM") != null)
                ((ImageButton)e.Row.FindControl("Button_deleteATM")).Attributes.Add("onClick", "return confirm('Are you sure you want to delete ATM " + ((DataRowView)e.Row.DataItem).Row["Title"] + "?','CCMS')");
        }
    }
}