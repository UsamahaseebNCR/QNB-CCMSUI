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
    public partial class AlertTemplate : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewAlertTemplate))
                    Response.Redirect("AccessDenied.aspx");

                if (!CheckRight((int)Permissions.ModifyAlertTemplate))
                    GridView_AlertTemplate.Columns[2].Visible = false;


                BindGrid();
            }

        }


        private void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select alert_type_id,alert_type_name,alert_additional_text,alert_default_text,send_email_notification,open_ticket_in_gasper,tpa_code,tpa_value
                                    from alert_type";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtAlertTemplate = new DataTable();
                adapter.Fill(dtAlertTemplate);
                cmd.Connection.Close();
                this.GridView_AlertTemplate.DataSource = dtAlertTemplate; ;
                this.GridView_AlertTemplate.DataBind();
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

        protected void GridView_AlertTemplate_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editAlertType")
            {
                LogableTask task = null;

                try
                {
                    task = LogableTask.NewTask("edit Alert");

                    Session["alert_type_id"] = e.CommandArgument.ToString();
                    Response.Redirect("EditAlertTemplate.aspx");
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while Edit Alert. " + ex.Message + "</li>";

                }
                finally
                {
                    task.EndTask();
                }
            }
        }

        protected void GridView_AlertTemplate_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_AlertTemplate.PageIndex = e.NewPageIndex;
            BindGrid();
        }
    }
}