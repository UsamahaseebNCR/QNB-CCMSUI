using Avanza.iSuite.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class ReconciliationMonitoring : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (!CheckRight((int)Permissions.ATMReconciliation))
                //    Response.Redirect("../admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                ViewState["SortExpression"] = "title";
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpressionCC"] = "title";
                ViewState["sortDirectionCC"] = " asc";

            }


        }
        private string buildSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();

            string fromDate = DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null).ToString("MM/dd/yyyy");
            string toDate = DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null).ToString("MM/dd/yyyy");

            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" '" + fromDate + "' , ");

            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(" '" + toDate + "'");

            return builder.ToString();
        }

        void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            try
            {
                cmd.CommandText = @"EXEC [GetTotalNotesReconcileData]  " + buildSearchCriteria();
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "loading data");
                adapter.Fill(dt);
                GridView_ReconciliationNotesDispensed.DataSource = dt;
                GridView_ReconciliationNotesDispensed.DataBind();
                Literal_CountsClearedCount.Text = dt.Rows.Count + " records.";
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

        protected void GridView_ReconciliationNotesDispensed_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (int.Parse(e.Row.Cells[8].Text) != 0 )
                {
                    e.Row.Cells[8].Font.Bold = true;
                    e.Row.Cells[8].BackColor = System.Drawing.Color.Red;

                }
                if (int.Parse(e.Row.Cells[9].Text) != 0)
                {
                    e.Row.Cells[9].Font.Bold = true;
                    e.Row.Cells[9].BackColor = System.Drawing.Color.Red;

                }
                if (int.Parse(e.Row.Cells[12].Text) != 0)
                {
                    e.Row.Cells[12].Font.Bold = true;
                    e.Row.Cells[12].BackColor = System.Drawing.Color.Red;

                }
                if (int.Parse(e.Row.Cells[13].Text) != 0)
                {
                    e.Row.Cells[13].Font.Bold = true;
                    e.Row.Cells[13].BackColor = System.Drawing.Color.Red;

                }
                if (int.Parse(e.Row.Cells[14].Text) != 0)
                {
                    e.Row.Cells[14].Font.Bold = true;
                    e.Row.Cells[14].BackColor = System.Drawing.Color.Red;

                }
                if (int.Parse(e.Row.Cells[15].Text) != 0)
                {
                    e.Row.Cells[15].Font.Bold = true;
                    e.Row.Cells[15].BackColor = System.Drawing.Color.Red;

                }

            }
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            btnSearch_Click(this, null);
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }
     



       
        protected void GridView_ReconciliationNotesDispensed_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpressionCC"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirectionCC"].ToString() == " asc")
                    ViewState["sortDirectionCC"] = " desc";
                else
                    ViewState["sortDirectionCC"] = " asc";
            }
            else
            {
                ViewState["SortExpressionCC"] = e.SortExpression;
                ViewState["sortDirectionCC"] = " asc";
            }
            BindGrid();
        }





        protected void GridView_ReconciliationNotesDispensed_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_ReconciliationNotesDispensed.PageIndex = e.NewPageIndex;
            BindGrid();
        }

    }
}