using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using Avanza.iSuite.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace CCMSUI.admin
{
    public partial class AddEditDashboard : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    Page.SetFocus(TextBox_Name);
                    if (Request.QueryString["id"] == null)
                    {
                        RadioButton_Active.SelectedIndex = 0;
                        TextBox_Refresh.Text = "60";
                    }
                    else
                    {
                        Avanza.CCMS.DAL.DashboardTemplate graph = Avanza.CCMS.DAL.DashboardTemplate.
                            LoadDashboardTemplateByPk(int.Parse(Request.QueryString["id"]));
                        TextBox_Name.Text = graph.Title;
                        TextBox_Refresh.Text = graph.RefreshDurationSec.ToString();
                        RadioButton_Active.SelectedIndex = graph.IsActive ? 0 : 1;
                    }
                }
                catch (Exception ex)
                {
                    Literal_error.Text = "<li>" + ex.Message + "</li>";
                }
            }
        }

        protected void Button_SaveUpdateTemplate_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddDashboardTemplate");
            SqlConnection conn = null;
            SqlTransaction trx = null;
            Avanza.CCMS.DAL.DashboardTemplate template;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                if (Request.QueryString["id"] != null)
                {
                    template = Avanza.CCMS.DAL.DashboardTemplate.LoadDashboardTemplateByPk(int.Parse(Request.QueryString["id"]));
                    template.ChangedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    template.ChangedAt = DateTime.Now;
                }
                else
                {
                    template = new Avanza.CCMS.DAL.DashboardTemplate();
                    template.CreatedAt = DateTime.Now;
                    template.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                }

                object res = ConnectionFactory.ExecuteScalar("select top 1 title from dashboard_template where title='" +
                    TextBox_Name.Text + "' "
                          + ((Request.QueryString["id"] != null) ? "and id != " + template.Id : ""));
                if (res != null)
                {
                    Literal_error.Text = "<li>This name is already defined for dashboard content '" + res + "'. Please, provide different dashboard content name.</li>";
                    return;
                }

                template.Title = TextBox_Name.Text;
                template.RefreshDurationSec = Convert.ToInt32(TextBox_Refresh.Text);
                template.IsActive = (RadioButton_Active.SelectedIndex == 0) ? true : false;
                template.Save(conn, trx);

                if (Request.QueryString["id"] == null)
                    Utility.BuildAuditLog(template.Title + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateCIT);
                else
                    Utility.BuildAuditLog(template.Title + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyCIT);

                if (trx != null)
                {
                    trx.Commit();
                }

                Response.Redirect("dashboardtemplate.aspx", false);
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }

                Literal_error.Text = "<li>An error occured while saving template." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }
    }
}