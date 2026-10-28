using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;

using ComponentArt.Web.UI;
using System.Text;

namespace CCMSUI.admin
{
    public partial class AddEditGLAccounts : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.IsPostBack)
                return;
            Page.SetFocus(TextBox_GLAccountName);
            if (Request.QueryString["id"] == null)
            {
                if (!CheckRight((int)Permissions.CreateGLAccount))
                    Response.Redirect("AccessDenied.aspx");
            }
            else
            {
                ReconciliationGlAccount glAccount = null;
                if (!CheckRight((int)Permissions.EditGLAccount))
                    Response.Redirect("AccessDenied.aspx");
                if (Request.QueryString["id"] != null)
                {
                    glAccount = ReconciliationGlAccount.LoadReconciliationGlAccountByPk(int.Parse(Request.QueryString["id"]));
                    TextBox_OpeningBalance.Text = glAccount.OpeningBalance.ToString();
                    TextBox_Comments.Text = glAccount.UserComments;
                    TextBox_GLAccountName.Text = glAccount.ReconciliationGlAccountName;
                }
            }
        }

        protected void Button_SaveUpdateGLAccount_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddEditGLAccount");
            ReconciliationGlAccount glAccount = null;
            int id = 0;
            StringBuilder builder = new StringBuilder(
                "select count(reconciliation_gl_account_id) from reconciliation_gl_account where reconciliation_gl_account_name = '" + TextBox_GLAccountName.Text + "'");
            try
            {
                if (Request.QueryString["id"] != null)
                {
                    id = int.Parse(Request.QueryString["id"]);
                    builder.Append(" and reconciliation_gl_account_id <> " + id);

                    if ((int)ConnectionFactory.ExecuteScalar(builder.ToString()) == 0)
                    {
                        glAccount = ReconciliationGlAccount.LoadReconciliationGlAccountByPk(id);
                        glAccount.ReconciliationGlAccountName = TextBox_GLAccountName.Text;
                        glAccount.OpeningBalance = decimal.Parse(TextBox_OpeningBalance.Text);
                        glAccount.UserComments = TextBox_Comments.Text;
                        glAccount.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        glAccount.ModificationDatetime = DateTime.Now;
                    }
                    else
                        throw new Exception("GL Account Name already exists");
                }
                else
                {
                    if ((int)ConnectionFactory.ExecuteScalar(builder.ToString()) == 0)
                    {
                        glAccount = new ReconciliationGlAccount();
                        glAccount.ReconciliationGlAccountName = TextBox_GLAccountName.Text;
                        glAccount.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        glAccount.CreationTime = DateTime.Now;
                        glAccount.OpeningBalance = decimal.Parse(TextBox_OpeningBalance.Text);
                        glAccount.UserComments = TextBox_Comments.Text;
                    }
                    else
                        throw new Exception("GL Account Name already exists");
                }
                glAccount.Save();
                Response.Redirect("GLAccounts.aspx", false);
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>An error occured while saving GL Account." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {

                task.EndTask();
            }
        }


    }
}