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
using System.Reflection;
using System.Diagnostics;

using Avanza.CCMS.DAL;


namespace Avanza.iSuite.UI
{
    public partial class ChangePassword : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ChangePassword))
                    Response.Redirect("AccessDenied.aspx");                
                
                Page.SetFocus(this.TextBox_CurrentPassword);
            }
        }

        protected void Button_ChangePassword_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("ChangePassword");
            bool checkpolicy = false;
            try
            {
                AppUser existingUser = AppUser.LoadAppUserByPk(((AppUser)Session[SessionVars.user.ToString()]).UserId);
                if (existingUser.UserPassword == Encryption.Cryptic.EncryptString(TextBox_CurrentPassword.Text))
                {
                    if (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).ApplyPasswordPolicy == true)
                    {
                        checkpolicy = AddEditUser.CheckPasswordPolicy(TextBox_NewPassword.Text, Literal_error);
                        if (checkpolicy == true)
                        {
                            existingUser.UserPassword = Encryption.Cryptic.EncryptString(TextBox_NewPassword.Text);
                            existingUser.Save();
                            Utility.BuildAuditLog(existingUser.UserFullName + " has changed his/her password successfully!", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ChangePassword);
                            Literal_Info.Text = "<li>Password changed successfully</li>";
                        }
                        else
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, Literal_error.ToString());
                            Literal_error.Text = "<li>Error: " + Literal_error + "</li>";
                        }

                    }
                    else
                    {
                        existingUser.UserPassword = Encryption.Cryptic.EncryptString(TextBox_NewPassword.Text);
                        existingUser.Save();
                        Utility.BuildAuditLog(existingUser.UserFullName + " has changed his/her password successfully!", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ChangePassword);
                        Literal_Info.Text = "<li>Password changed successfully</li>";
                       
                    }
                }
                else
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "Current password does not match with the logged in user");
                    Literal_error.Text = "<li>" + "Current password does not match with the logged in user" + "</li>";

                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>Error: " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }

        }
    }
}