using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Text;
using System.Data.SqlClient;
using Avanza.iSuite.DAL;
using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS
{
    public partial class CallOutSummary : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCallOutSummary))
                    Response.Redirect("../admin/AccessDenied.aspx");

                if (Session["callOutFromDate"] != null)
                    TextBox_FromDate.Text = Session["callOutFromDate"].ToString();
                else
                    TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                if (Session["callOutToDate"] != null)
                    TextBox_ToDate.Text = Session["callOutToDate"].ToString();
                else
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "atm_location";
                ViewState["sortDirection1"] = " asc";
                ViewState["SortExpression1"] = "code";
                ViewState["sortDirection2"] = " asc";
                ViewState["SortExpression2"] = "actual_problem_found";
                BindGridByLocation(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                BindGridByCode(ViewState["SortExpression1"].ToString() + ViewState["sortDirection1"].ToString());
                BindGridByIssue(ViewState["SortExpression2"].ToString() + ViewState["sortDirection2"].ToString());


            }

        }
        private string buildSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();
            string selectQuery = String.Empty;
            builder.Append(" where");



            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" activity_datetime >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) AND");

            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(@"  activity_datetime <=Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) AND");


            selectQuery = builder.ToString();
            selectQuery = selectQuery.Trim();
            if (selectQuery.EndsWith("AND"))
                selectQuery = selectQuery.Substring(0, selectQuery.LastIndexOf("AND"));
            return selectQuery;
        }

        void BindGridByLocation(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGridByLocation");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            try
            {
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select atm_location, count(flm_monitoring_id) total from flm_monitoring " + buildSearchCriteria() + " group by atm_location Order by " + orderBy;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                GridView_CallOutSummary.DataSource = dt;
                GridView_CallOutSummary.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured. " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
        void BindGridByIssue(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGridByIssue");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            try
            {
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select actual_problem_found,atm_id_no, count(flm_monitoring_id) total from flm_monitoring " + buildSearchCriteria() + " group by actual_problem_found,atm_id_no having count(flm_monitoring_id)>3 Order by " + orderBy;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                GridView_problemSummary.DataSource = dt;
                GridView_problemSummary.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured. " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
        void BindGridByCode(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            try
            {
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select code, count(flm_monitoring_id) total from flm_monitoring " + buildSearchCriteria() + " group by code Order by " + orderBy;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                GridView_CodeSummary.DataSource = dt;
                GridView_CodeSummary.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured. " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGridByLocation(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            BindGridByCode(ViewState["SortExpression1"].ToString() + ViewState["sortDirection1"].ToString());
            Session["callOutFromDate"] = TextBox_FromDate.Text;
            Session["callOutToDate"] = TextBox_ToDate.Text;

        }


        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            btnSearch_Click(this, null);
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

        }
        protected void GridView_ProblemSummary_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression2"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection2"].ToString() == " asc")
                    ViewState["sortDirection2"] = " desc";
                else
                    ViewState["sortDirection2"] = " asc";
            }
            else
            {
                ViewState["SortExpression2"] = e.SortExpression;
                ViewState["sortDirection2"] = " asc";
            }
            BindGridByIssue(ViewState["SortExpression2"].ToString() + ViewState["sortDirection2"].ToString());
        }


        protected void GridView_CallOutSummary_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGridByLocation(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_CodeSummary_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression1"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection1"].ToString() == " asc")
                    ViewState["sortDirection1"] = " desc";
                else
                    ViewState["sortDirection1"] = " asc";
            }
            else
            {
                ViewState["SortExpression1"] = e.SortExpression;
                ViewState["sortDirection1"] = " asc";
            }
            BindGridByCode(ViewState["SortExpression1"].ToString() + ViewState["sortDirection1"].ToString());
        }

        protected void GridView_CallOutSummary_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Sort"))
            {
                if (ViewState["SortExpression"] != null)
                {
                    if (this.ViewState["SortExpression"].ToString() == e.CommandArgument.ToString())
                    {
                        if (ViewState["sortDirection"].ToString() == " asc")
                            ViewState["sortDirection"] = " desc";
                        else
                            ViewState["sortDirection"] = " asc";
                    }
                    else
                    {
                        ViewState["sortDirection"] = " asc";
                        ViewState["SortExpression"] = e.CommandArgument.ToString();
                    }
                }
                else
                {
                    ViewState["SortExpression"] = e.CommandArgument.ToString();
                    ViewState["sortDirection"] = " asc";
                }
            }

            //Re Bind The Grid to reflect the Sort Order
            BindGridByLocation(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());


        }




        //protected void GridView_CallOut_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    this.GridView_CallOutSummary.PageIndex = e.NewPageIndex;
        //    BindGridByLocation(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        //}
    }
}