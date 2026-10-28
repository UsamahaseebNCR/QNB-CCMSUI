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
using System.Text;


namespace Avanza.iSuite.UI
{
    public partial class ExceptionHandlingCriteria : BasePage
    {
        StringBuilder builder = new StringBuilder();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewExceptionHandlingCriteria))
                    Response.Redirect("AccessDenied.aspx");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "message";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }


        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @" 
                                    select exception_handling_criteria_id,message,is_precondition,is_disputed,is_sms_task_required,
                                    case when transaction_type_id = 0 then 'Cash Deposit Transaction' 
	                                     when transaction_type_id = 1 then 'Cheque Deposit Transaction'
	                                     when transaction_type_id = 2 then 'Normal Transaction'
	                                     when transaction_type_id = 3 then 'Captured Card Transaction'
	                                      end transaction_type_name
                                    from exception_handling_criteria 
                                     where " + Filter() + " order by " + orderby + "";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtExceptionCriteria = new DataTable();
                adapter.Fill(dtExceptionCriteria);
                cmd.Connection.Close();
                this.GridView_ExceptionCriteria.DataSource = dtExceptionCriteria;
                this.GridView_ExceptionCriteria.DataBind();
                Literal_count.Text = dtExceptionCriteria.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }

        protected void GridView_AlertTemplate_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridView_ExceptionCriteria_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_ExceptionCriteria.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Criteria_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteTemplate") != null)
                ((ImageButton)e.Row.FindControl("Button_deleteTemplate")).Attributes.Add("onClick", "return confirm('Are you sure you want to delete message " + ((DataRowView)e.Row.DataItem).Row["message"] + "?','CCMS')");
        }

        protected void GridView_ExceptionCriteria_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteCriteria")
            {
                SqlCommand cmd = null;
                //SqlTransaction trx = null;
                try
                {
                    task = LogableTask.NewTask("delete Criteria");
                    cmd = ConnectionFactory.GetNewCommand(true);

                    //Avanza.CCMS.DAL.ExceptionHandlingCriteria exceptionCriteria = Avanza.CCMS.DAL.ExceptionHandlingCriteria.LoadExceptionHandlingCriteriaByPk(int.Parse(e.CommandArgument.ToString()));

                    //  trx = cmd.Connection.BeginTransaction();
                    cmd.CommandText = "DeleteExceptionCriteria";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@exceptionhandlingcriteriaid", SqlDbType.Int);
                    cmd.Parameters[0].Value = int.Parse(e.CommandArgument.ToString());
                    //cmd.Transaction = trx;
                    cmd.ExecuteNonQuery();
                    //trx.Commit();
                    // if (exceptionCriteria != null)
                    //{
                    Utility.BuildAuditLog("Exception Criteria: "+e.CommandArgument.ToString() + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteException );

                    //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                    Session[SessionVars.notice.ToString()] = "Selected Criteria was deleted!";
                    //}

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Criteria deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting Criteria. " + ex.Message + "</li>";
                    //trx.Rollback();
                }
                finally
                {
                    if (cmd != null)
                        if (cmd.Connection != null)
                            cmd.Connection.Close();

                    task.EndTask();

                }
            }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in CheckBox_Conditions.Items)
            {
                if (item.Selected)
                {
                    builder.Append(item.Text + ",");

                }
            }
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        string Filter()
        {
            string filter = "";
            string[] parts = builder.ToString().Split(',');


            filter += "   message like '%" + TextBox_message.Text + "%'";

            if (Array.IndexOf(parts, "PreCondition") > -1)
            {
                if (filter != "")
                    filter += " and ";
                filter += " is_precondition = 1";
            }
            if (Array.IndexOf(parts, "Disputed") > -1)
            {
                if (filter != "")
                    filter += " and ";
                filter += " is_disputed = 1";
            }
            if (Array.IndexOf(parts, "SMSTaskRequired") > -1)
            {
                if (filter != "")
                    filter += " and ";
                filter += " is_sms_task_required = 1";
            }



            return filter;
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_message.Text = "";

        }

        protected void GridView_SMSTemplate_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_SMSTemplate_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void Button_editCriteria_Click(object sender, CommandEventArgs e)
        {
            Session["exceptionCritID"]= e.CommandArgument.ToString();
            Response.Redirect("EditExceptionHandlingCriteria.aspx");
        }
        
    }
}