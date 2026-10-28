using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using Avanza.iSuite.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.admin
{
    public partial class AddEditTemplate : BasePage
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
                    }
                    else
                    {
                        Avanza.CCMS.DAL.ReportTemplate reportTemplate = Avanza.CCMS.DAL.ReportTemplate.LoadReportTemplateByPk(int.Parse(Request.QueryString["id"]));
                        TextBox_Name.Text = reportTemplate.ReportName;
                        TextBox_Query.Text = reportTemplate.ReportQuery.Replace('"', '\''); ;
                        RadioButton_Active.SelectedIndex = reportTemplate.IsActive ? 0 : 1;
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
            LogableTask task = LogableTask.NewTask("AddTemplate");
            SqlConnection conn = null;
            SqlTransaction trx = null;
            Avanza.CCMS.DAL.ReportTemplate reportTemplate;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                if (Request.QueryString["id"] != null)
                {
                    reportTemplate = Avanza.CCMS.DAL.ReportTemplate.LoadReportTemplateByPk(int.Parse(Request.QueryString["id"]));
                    reportTemplate.ChangedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    reportTemplate.ChangedDate = DateTime.Now;
                }
                else
                {
                    reportTemplate = new Avanza.CCMS.DAL.ReportTemplate();
                    reportTemplate.CreateDate = DateTime.Now;
                    reportTemplate.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                }

                object res = ConnectionFactory.ExecuteScalar("select top 1 report_name from report_template where report_name='" + 
                    TextBox_Name.Text + "' " + ((Request.QueryString["id"] != null) ? "and report_id != " + 
                    reportTemplate.ReportId : ""));
                if (res != null)
                {
                    Literal_error.Text = "<li>This name is already defined for report '" + res + "'. Please, provide different report name.</li>";
                    return;
                }

                var command = TextBox_Query.Text.Split(';')[0];
                if (!IsValidQuery(command))
                    throw new Exception("Only query expression is allowed");

                reportTemplate.ReportName = TextBox_Name.Text;
                reportTemplate.ReportQuery = command.Replace('\'', '"');
                reportTemplate.IsActive = (RadioButton_Active.SelectedIndex == 0) ? true : false;
                reportTemplate.Save(conn, trx);

                if (Request.QueryString["id"] == null)
                    Utility.BuildAuditLog(reportTemplate.ReportName + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateCIT);
                else
                    Utility.BuildAuditLog(reportTemplate.ReportName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyCIT);

                if (trx != null)
                {
                    trx.Commit();
                }

                Response.Redirect("reporttemplate.aspx", false);
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }

                Literal_error.Text = "<li>An error occured while saving report name." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        protected void Button_PreviewQuery_Click(object sender, EventArgs e)
        {
            try
            {
                if (TextBox_Query.Text.Length > 0)
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    var command = TextBox_Query.Text.Split(';')[0];
                    
                    if (!IsValidQuery(command))
                        throw new Exception("Only query expression is allowed");
                    
                    cmd.CommandText = command;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    GridView_Result.DataSource = dt;
                    GridView_Result.DataBind();
                    Literal_error.Text = "";
                    preview.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }
        }

        private bool IsValidQuery(string command)
        {
            string[] blockKeyWords = new string[] { "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER" };
            foreach (string x in blockKeyWords)
            {
                if (command.Contains(x))
                {
                    return false;
                }
            }

            return true;
        }
    }
}