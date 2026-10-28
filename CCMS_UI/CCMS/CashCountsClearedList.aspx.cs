using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Text;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS
{
    public partial class CashCountsClearedList : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewATMSettlement))
                    Response.Redirect("../admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "clearing_datetime";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        private string buildSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();
            string selectQuery = String.Empty;
            builder.Append(" where is_active = 1 AND");

            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" clearing_datetime >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) AND");

            if (TextBox_ToDate.Text.Length > 0)
                builder.Append("  clearing_datetime <=Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) AND");


            builder.Append("  atm.atm_id in  ("
                    + Session[SessionVars.SelectedATMs.ToString()] + ") AND");

                //builder.Append("  gl_no in (select gl_number from atm where atm_id in  ("
                //    + Session[SessionVars.SelectedATMs.ToString()] + ")) AND");
 
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
                cmd.CommandText = @"select * from dispensed inner join atm on atm.atm_id= dispensed.atm_id " + buildSearchCriteria() + " Order by " + orderBy;


                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                GridView_CashCountsCleared.DataSource = dt;
                GridView_CashCountsCleared.DataBind();
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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
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


        protected void GridView_CashCountsCleared_Sorting(object sender, GridViewSortEventArgs e)
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




        protected void GridView_CashCountsCleared_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_CashCountsCleared.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }
    }
}