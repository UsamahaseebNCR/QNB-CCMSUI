using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data;
using System.Text;

using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

namespace CCMSUI.admin
{
    public partial class FITMapping : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewFITMapping))
                    Response.Redirect("AccessDenied.aspx");

                if (!CheckRight((int)Permissions.DeleteFITMapping))
                    GridView_FITMapping.Columns[3].Visible = false;
                if (!CheckRight((int)Permissions.EditFITMapping))
                    GridView_FITMapping.Columns[2].Visible = false;


                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "pan_prefix";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
        }
        public static DataTable MergeFIT(DataTable dtFIT)
        {
            DataTable newDataTable = new DataTable();
            newDataTable.Columns.Add("pan_prefix", typeof(string));
            newDataTable.Columns.Add("bank_name", typeof(string));
            newDataTable.Columns["bank_name"].Unique = true;
            newDataTable.PrimaryKey = new DataColumn[] { newDataTable.Columns["bank_name"] };

            string value = String.Empty;

            if (dtFIT.Rows.Count > 0)
            {
                for (int counter = 0; counter < dtFIT.Rows.Count; counter++)
                {
                    StringBuilder builder = new StringBuilder();
                    DataRow[] dataRow = newDataTable.Select("bank_name='" + dtFIT.Rows[counter].ItemArray[1].ToString() + "'");
                    if (dataRow.Length == 0)
                        newDataTable.Rows.Add(new object[] { dtFIT.Rows[counter].ItemArray[0].ToString(),
                        dtFIT.Rows[counter].ItemArray[1].ToString() });
                    else
                        dataRow[0].ItemArray = new object[] { dataRow[0].ItemArray[0] + ", " + dtFIT.Rows[counter].ItemArray[0].ToString() };
                }
            }
            return newDataTable;
        }
        private void BindGrid(string orderby)
        {
            try
            {

                StringBuilder builder = new StringBuilder();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                builder.Append(@"select PAN_prefix, bank_name from fitmapping  where");

                if (TextBox_PANPrefix.Text.Length > 0)
                    builder.Append(" PAN_prefix like '" + TextBox_PANPrefix.Text + "%' AND");

                string query = builder.ToString();
                query = query.Trim();

                if (query.EndsWith("AND"))
                    query = query.Substring(0, query.LastIndexOf("AND"));
                if (query.EndsWith("where"))
                    query = query.Substring(0, query.LastIndexOf("where"));
                query += " order by " + orderby;

                cmd.CommandText = query;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataTable dtFIT = new DataTable();
                adapter.Fill(dtFIT);

                DataTable fit = MergeFIT(dtFIT);


                this.GridView_FITMapping.DataSource = fit;
                this.GridView_FITMapping.DataBind();
                Literal_count.Text = fit.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                LogableTask.LogMonoActivityTask("bind FIT grid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

        protected void GridView_FITMapping_Sorting(object sender, GridViewSortEventArgs e)
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
        protected void GridView_FITMapping_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            try
            {
                if (e.CommandName == "deleteFIT")
                {
                    LogableTask task = null;
                    SqlCommand cmd = null;
                    SqlTransaction trx = null;
                    try
                    {
                        task = LogableTask.NewTask("delete FIT");
                        ConnectionFactory.ExecuteScalar("delete fitmapping where pan_prefix in ('" + e.CommandArgument.ToString() + "')");
                        Utility.BuildAuditLog("selected fit " + e.CommandArgument.ToString() + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteFITMapping);
                        Literal_ok.Text = "<li>Selected FIT deleted!</li>";
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "FIT deleted");
                        BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                        Literal_error.Text = "<li>An Error occured while deleting FIT. " + ex.Message + "</li>";
                        trx.Rollback();
                    }
                    finally
                    {
                        cmd.Connection.Close();
                        task.EndTask();

                    }
                }
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("GridView_FIT_RowCommand");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
            }
        }

        protected void GridView_FITMapping_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    if (e.Row.FindControl("imgbtn_delete") != null)
                        ((ImageButton)e.Row.FindControl("imgbtn_delete")).Attributes.Add("onClick",
                            "return confirm('Are you sure you want to delete FIT \"" + ((DataRowView)e.Row.DataItem).Row["pan_prefix"] + "\" ?','CCMS')");
                }
                /*       if (e.Row.RowType == DataControlRowType.DataRow)
                       {
                           ((TextBox)e.Row.FindControl("PAN_prefix")).Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                       }*/

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("GridView_FIT_RowDataBound", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = ex.Message;
            }
        }
        /* protected void GridView_RowCreated(object sender, GridViewRowEventArgs e)
         {

             if (e.Row.RowType == DataControlRowType.DataRow)
             {
                 ((TextBox)e.Row.FindControl("PAN_prefix")).Width = new Unit(100);
             }

         }*/
        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_PANPrefix.Text = "";
        }


        protected void Button_Search_Click(object sender, EventArgs e)
        {
           
                bool isPANContainSpecialChars = CheckSpecialChars(TextBox_PANPrefix.Text);
                if (isPANContainSpecialChars == true)
                {
                    throw new Exception ("PAN Prefix contain invalid character");
                    //return;
                }
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            
           
        }
        protected void GridView_FITMapping_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            this.GridView_FITMapping.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }
       
    }
}