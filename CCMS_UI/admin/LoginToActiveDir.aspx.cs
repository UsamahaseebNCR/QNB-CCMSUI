using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Configuration;
using System.Web.UI;

namespace CCMSUI
{
    public partial class LoginToActiveDir : BasePage
    {
        int loginAttempt = int.Parse(WebConfigurationManager.AppSettings["loginAttempt"]);
        int timeDuration = int.Parse(WebConfigurationManager.AppSettings["reAttemptTime"]);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                if (!CheckRight((int)Permissions.AddActiveDirectoryUsers))
                    Response.Redirect("AccessDenied.aspx");



                if (System.Configuration.ConfigurationSettings.AppSettings["readDomainFromAppSetting"] == "1")
                {
                    AppSetting appsetting = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]);
                    TextBox_Domain.Text = appsetting.ActiveDirectoryDomain;
                }

                Page.SetFocus(this.TextBox_Domain);
            }
        }
        protected void Button_Next_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Login To Active Directory");

            try
            {
                if (Session["loginAttempt"] == null)
                    Session["loginAttempt"] = 0;
                if ((int)Session["loginAttempt"] >= loginAttempt)
                {
                    Session["attemptExhaustTime"] = DateTime.Now;
                    Session["loginAttempt"] = 0;
                }
                if (Session["attemptExhaustTime"] != null && (((DateTime)Session["attemptExhaustTime"]).AddMinutes(timeDuration) >= DateTime.Now))
                {
                    Response.StatusCode = 429;
                    throw new Exception("Login Attemps Exhausted. Try again after few minutes");
                }
                Session["loginAttempt"] = (int)Session["loginAttempt"] + 1;
                CCMSUI.SignIn.Impersonate impersonate = new SignIn.Impersonate();

                if (!impersonate.DoLogIn(TextBox_Domain.Text, TextBox_UserName.Text, TextBox_Password.Text))
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, Marshal.GetLastWin32Error().ToString());
                    Response.StatusCode = 401;
                    //Response.End();
                    throw new Exception("User Id or Password is wrong!");
                }

                Session["impersonate"] = impersonate;
                Avanza.CCMS.DAL.AppSetting appSettings = (Avanza.CCMS.DAL.AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                appSettings.ActiveDirectoryDomain = TextBox_Domain.Text;
                appSettings.Save();
                Session[SessionVars.ADUserName.ToString()] = TextBox_Domain.Text + "\\" + TextBox_UserName.Text;
                Session[SessionVars.ADPassword.ToString()] = TextBox_Password.Text;
                Session[SessionVars.ADSearch.ToString()] = TextBox_Search.Text;
                Response.Redirect("AddActiveDirectoryUsers.aspx", false);

            }
            catch (Exception ex)
            {
                Literal_Msg.Text = "<li style='text-align:center;color:red'>" + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }

        }

        protected void TextBox_Domain_TextChanged(object sender, EventArgs e)
        {

        }


    }
}