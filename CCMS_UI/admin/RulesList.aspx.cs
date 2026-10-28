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

using Avanza.CCMS.DAL;


namespace Avanza.iSuite.UI
{
    public partial class RulesList : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewRulesList))
                    Response.Redirect("AccessDenied.aspx");

                if (!CheckRight((int)Permissions.EditRulesList))
                    GridView_Rules.Columns[4].Visible = false;

                if (!CheckRight((int)Permissions.DeactivateRules))
                    GridView_Rules.Columns[5].Visible = false;

                ViewState["SortExpression"] = "creation_time ";
                ViewState["sortDirection"] = "desc";
                BindGrid();
            }

        }


        private void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select * from transaction_rule " + (TextBox_RuleFriendlyName.Text.Length > 0 ? "where transaction_rule_friendly_name like '%" + TextBox_RuleFriendlyName.Text + "%'" : "") +
                                  " order by " + ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtRules = new DataTable();
                adapter.Fill(dtRules);
                cmd.Connection.Close();
                this.GridView_Rules.DataSource = dtRules; ;
                this.GridView_Rules.DataBind();
                Literal_count.Text = dtRules.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = ex.Message;
            }
            finally
            { task.EndTask(); }
        }


        protected void GridView_Rules_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Rules.PageIndex = e.NewPageIndex;
            BindGrid();
        }
        protected void GridView_Rules_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGrid();
        }
        protected void button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_RuleFriendlyName.Text = "";
        }
    }
}