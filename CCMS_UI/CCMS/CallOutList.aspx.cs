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
using System.Drawing;

namespace CCMSUI.CCMS
{
    public partial class CallOutList : BasePage
    {
        protected void GridView_CallOut_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string strTimeOfCallToBank = ((DataRowView)e.Row.DataItem).Row["time_of_call_to_bank"].ToString();
                string strTimeOfReachingSite = ((DataRowView)e.Row.DataItem).Row["time_of_reaching_site"].ToString();
                string strTimeOfSolving = ((DataRowView)e.Row.DataItem).Row["time_of_solving"].ToString();
                string strTimeOfInfo = ((DataRowView)e.Row.DataItem).Row["time_of_info"].ToString();
                if ((((strTimeOfCallToBank.Length > 0) && (strTimeOfReachingSite.Length > 0)) && (strTimeOfSolving.Length > 0)) && (strTimeOfInfo.Length > 0))
                {
                    DateTime timeOfCallToBank = DateTime.Parse(strTimeOfCallToBank);
                    DateTime timeOfReachingSite = DateTime.Parse(strTimeOfReachingSite);
                    DateTime timeOfSolving = DateTime.Parse(strTimeOfSolving);
                    if (((DateTime.Parse(strTimeOfInfo) >= timeOfReachingSite) || (timeOfReachingSite >= timeOfSolving)) || (timeOfSolving > timeOfCallToBank))
                    {
                        e.Row.BackColor = ColorTranslator.FromHtml("#EF3039");
                    }
                }
                else if (strTimeOfCallToBank.Length == 0)
                {
                    e.Row.BackColor = Color.Yellow;
                }
            }
             
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCallOutList))
                    Response.Redirect("../admin/AccessDenied.aspx");

                if (Session["callOutFromDate"] != null)
                    TextBox_FromDate.Text = Session["callOutFromDate"].ToString();
                else
                    TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                if (Session["callOutToDate"] != null)
                    TextBox_ToDate.Text = Session["callOutToDate"].ToString();
                else
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = "activity_datetime";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
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
            if (Request.QueryString["l"] != null)
                builder.Append(@"  atm_location='" + HttpUtility.UrlDecode(Request.QueryString["l"]) + "' AND");
            if (Request.QueryString["c"] != null)
                builder.Append(@"  code='" + HttpUtility.UrlDecode(Request.QueryString["c"]) + "' AND");
            if (Request.QueryString["p"] != null)
                builder.Append(@"  actual_problem_found='" + HttpUtility.UrlDecode(Request.QueryString["p"]) + "' AND");

            selectQuery = builder.ToString();
            selectQuery = selectQuery.Trim();
            if (selectQuery.EndsWith("AND"))
                selectQuery = selectQuery.Substring(0, selectQuery.LastIndexOf("AND"));
            return selectQuery;
        }

        void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            try
            {
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select * from flm_monitoring " + buildSearchCriteria() + " Order by " + orderBy;


                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                GridView_CallOut.DataSource = dt;
                GridView_CallOut.DataBind();
                Literal_count.Text = dt.Rows.Count + " records.";
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
            for (int j = 0; j < CheckBoxList_DisplayColumns.Items.Count; j++)
                GridView_CallOut.Columns[j].Visible = true;

            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            for (int j = 0; j < CheckBoxList_DisplayColumns.Items.Count; j++)
            {
                if (!CheckBoxList_DisplayColumns.Items[j].Selected)
                    GridView_CallOut.Columns[j].Visible = false;



            }

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


        protected void GridView_CallOut_Sorting(object sender, GridViewSortEventArgs e)
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




        protected void GridView_CallOut_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_CallOut.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }
    }
}