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
    public partial class DashboardGraphTemplate : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["id"] == null)
                    HyperLink_newGraph.NavigateUrl = string.Format("../admin/AddEditDashboardGraph.aspx");
                else
                    HyperLink_newGraph.NavigateUrl = string.Format("../admin/AddEditDashboardGraph.aspx?dashboard_id={0}", 
                        Convert.ToInt32(Request.QueryString["id"]));

                Avanza.CCMS.DAL.DashboardTemplate.DashboardTemplateReader templateReader =
                        Avanza.CCMS.DAL.DashboardTemplate.ExecuteReader("is_active = 1");
                DropDownList_dashboard.Items.Add(new ListItem("Select", ""));
                while (templateReader.Read())
                {
                    DropDownList_dashboard.Items.Add(new ListItem(templateReader.CurrentDashboardTemplate.Title.ToString(),
                        templateReader.CurrentDashboardTemplate.Id.ToString()));

                }
                templateReader.Close();

                DropDownList_dashboard.SelectedValue = Convert.ToInt32(Request.QueryString["id"]).ToString();

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "name";
            }

            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                int dashboardId = string.IsNullOrEmpty(DropDownList_dashboard.SelectedValue) ? 0 : Convert.ToInt32(DropDownList_dashboard.SelectedValue);

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = string.Format(@"SELECT dg.id, dt.title, dg.name, dg.type, dg.row_number, dg.is_active 
                                    FROM dashboard_graph dg
                                    INNER JOIN dashboard_template dt ON dg.dashboard_id = dt.id
                                    WHERE (0 = {0} OR dashboard_id = {1})
                                    AND dt.is_active = 1
                                    ORDER BY {2}", dashboardId, dashboardId, orderby);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtDashboards = new DataTable();
                adapter.Fill(dtDashboards);
                cmd.Connection.Close();
                this.GridView_Templates.DataSource = dtDashboards; ;
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
            if (e.CommandName == "editDashboard")
            {
                if (Request.QueryString["i"] == null)
                    Response.Redirect("AddEditDashboard.aspx?id=" + e.CommandArgument);
                else
                    Response.Redirect("AddEditDashboard.aspx?id=" + e.CommandArgument + "&i=1");
            }
        }

        protected void GridView_Templates_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Templates.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Templates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteDashboard") != null)
                ((ImageButton)e.Row.FindControl("Button_deleteDashboard")).Attributes.Add("onClick", "return confirm('Are you sure you want to delete Dashboard " + ((DataRowView)e.Row.DataItem).Row["Title"] + "?','CCMS')");
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }
    }
}