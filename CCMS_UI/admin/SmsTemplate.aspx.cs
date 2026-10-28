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
    public partial class SmsTemplate : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewSmsTemplate))
                    Response.Redirect("AccessDenied.aspx");

                //if (!CheckRight((int)Permissions.ModifyAlertTemplate))
                //  GridView_AlertTemplate.Columns[2].Visible = false;


                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "ccms_sms_template_name";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }


        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @" select ccms_sms_template_id,ccms_sms_template_name,ccms_sms_template_message,user_login,tibco_template_id
                                    from ccms_sms_template inner join app_user on 
                                    created_by = user_id where " + Filter() + " order by " + orderby + "";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtAlertTemplate = new DataTable();
                adapter.Fill(dtAlertTemplate);
                cmd.Connection.Close();
                this.GridView_SMSTemplate.DataSource = dtAlertTemplate; ;
                this.GridView_SMSTemplate.DataBind();
                Literal_count.Text = dtAlertTemplate.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }

        protected void GridView_AlertTemplate_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridView_AlertTemplate_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_SMSTemplate.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_SMSTemplate_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "editTemplate")
            {
                //if (Request.QueryString["i"] == null)
                //    Response.Redirect("AddEditATM.aspx?atm_id="+e.CommandArgument);
                //else
                //    Response.Redirect("AddEditATM.aspx?atm_id=" + e.CommandArgument+"&i=1");

            }
            else if (e.CommandName == "deleteTemplate")
            {
                SqlCommand cmd = null;
                SqlTransaction trx = null;
                try
                {
                    task = LogableTask.NewTask("delete Template");
                    cmd = ConnectionFactory.GetNewCommand(true);

                    CcmsSmsTemplate sms = CcmsSmsTemplate.LoadCcmsSmsTemplateByPk(int.Parse(e.CommandArgument.ToString()));

                    trx = cmd.Connection.BeginTransaction();
                    cmd.CommandText = "DeleteSMSTemplate";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@template_id", SqlDbType.Int);
                    cmd.Parameters[0].Value = int.Parse(e.CommandArgument.ToString());
                    cmd.Transaction = trx;
                    cmd.ExecuteNonQuery();
                    trx.Commit();
                    if (sms != null)
                    {
                        // Utility.BuildAuditLog(atm.Title + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteATM);

                        //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                        Session[SessionVars.notice.ToString()] = "Selected Template was deleted!<script>window.parent.DeleteATM(" + sms.CcmsSmsTemplateId + ")</script>";
                    }

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Template deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting Template. " + ex.Message + "</li>";
                    trx.Rollback();
                }
                finally
                {
                    cmd.Connection.Close();
                    task.EndTask();

                }
            }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        string Filter()
        {
            string filter = "";

            filter += "  ccms_sms_template_name like '%" + TextBox_template.Text + "%' and ccms_sms_template_message like '%" + TextBox_message.Text + "%'";



            return filter;
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_message.Text = "";
            TextBox_template.Text = "";
        }


        protected void GridView_SMSTemplate_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteTemplate") != null)
                ((ImageButton)e.Row.FindControl("Button_deleteTemplate")).Attributes.Add("onClick", "return confirm('Are you sure you want to delete SMS Template " + ((DataRowView)e.Row.DataItem).Row["ccms_sms_template_name"] + "?','CCMS')");
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
    }
}