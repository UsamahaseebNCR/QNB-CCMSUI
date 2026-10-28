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
using System.Data.SqlClient;
using Avanza.iSuite.DAL;
using Encryption;
using Avanza.CCMS.DAL;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CCMSUI
{
    public class ActiveSessions
    {
        public string userID;
        public DateTime lastRequestSentAt;
        public DateTime loggedInAt;
        public string machineName;

    }

    public partial class SignIn : System.Web.UI.Page
    {



        string progID = System.Configuration.ConfigurationSettings.AppSettings["ProgId"];
        string isSecurityWebServiceEnabled = System.Configuration.ConfigurationSettings.AppSettings["isSecurityWebServiceEnabled"];

        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_PROVIDER_DEFAULT = 0;

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern int RevertToSelf();

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern int ImpersonateLoggedOnUser(IntPtr hToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        override protected void OnInit(EventArgs e)
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            ImageButton_SignIn_Click(this, null);
        }
        [Serializable]
        public class Impersonate 
        {
            IntPtr token;

            //public IntPtr GetToken()
            //{
            //    return token;
            //}
            public void Revert()
            {
                RevertToSelf();
            }

            public bool DoLogIn(string domain, string username, string password)
            {
                if (LogonUser(username, domain, password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, out token) > 0)
                    return true;
                else
                    return false;
            }

            public void ShutDownHandle()
            {
                CloseHandle(token);
            }

            public bool DoImpersonate()
            {
                try
                {
                    ImpersonateLoggedOnUser(token);
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Page.SetFocus(TextBox_signin);


                Response.Cookies.Remove("ASP.NET_SessionId");
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId",
                RandomString.Generate(24, 24)));

                //TextBox_password.Attributes.Add("onkeydown", "return checkForSQLInjection()");
                //TextBox_signin.Attributes.Add("onkeydown", "return checkForSQLInjection()");


                if (Application[ApplicationVars.initError.ToString()] != null)
                    Literal_ErrMsg.Text = "<script>alert('" + ((string)Application[ApplicationVars.initError.ToString()]).Replace("\r\n", "").Replace("\\", "\\\\").Replace("'", "\\'") + "');</script>";
                trConfirmPassword.Visible = false;
                ViewState["attempts"] = 0;
                if (isSecurityWebServiceEnabled == "0")
                    ImageButton_RequestChangePassword.Visible = false;

            }



        }


        private string GetUserFriendlyMessageAgainstCode(string code)
        {
            string msg = "<img src='images/icon_err.gif'>&nbsp;";
            if (code == "#0")
                msg += "System is included confidential databases";
            else if (code == "#1")
                msg += "User group suspended";
            else if (code == "#2")
                msg += "User is prohibited from dealing on the system";
            else if (code == "#3")
                msg += "Password Error";
            else if (code == "#4")
                msg += "Validity of user access to the system ended";
            else if (code == "#5")
                msg += "User does not exist confidential databases";
            else if (code == "#6")
                msg += "User must change the password when you first use";
            else if (code == "#7")
                msg += "User must change the password for the passage of 60 days";
            else if (code == "#8")
                msg += "User has been shut down for the passage of more than 60 days without the use of the system";
            else if (code == "#9")
                msg += "Password length should be greater than or equal to 8";
            else if (code == "#10")
                msg += "Password similar to one of the last four passwords";
            else if (code == "#11")
                msg += "Password should contain letters and numbers";
            else if (code == "#12")
                msg += "User has been suspended to repeat the wrong number of login attempts";
            else if (code == "#13")
                msg += "Username / Password contains one of the words banned use";
            else
                msg += "Unknown error code returned "+code;
            return msg;
        }

        private bool ValidateWithNBESecurityModule(int id)
        {
            LogableTask task = LogableTask.NewTask("ValidateWithNBESecurityModule");
            bool result = false;
            string stopUserResponse = null;
            string[] parts = null;
            try
            {
                task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "Going to create object");
                SecurityService securityService = new SecurityService();
                if (securityService == null)
                    throw new Exception("Unable to create service object");
                task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "object created");
                
                if (id == 0)
                {
                    parts = securityService.getGroup(TextBox_signin.Text, TextBox_password.Text, progID);
                    task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "return from getGroup call");
                }
                else
                {
                    parts = securityService.getGroup1(TextBox_signin.Text, TextBox_password.Text, progID);
                    task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "return from getGroup1 call");
                }


                if (parts.Length > 0)
                {
                    if (parts[0].StartsWith("#"))
                    {
                        if (ViewState["attempts"].ToString() == "5")
                        {
                            task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "attempts value " + ViewState["attempts"]+", Now going to suspend it");
                            stopUserResponse = securityService.StopUser(TextBox_signin.Text, progID);
                            task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "Response from StopUser Call " +stopUserResponse);
                            if (stopUserResponse == "1")
                            {
                                Literal_ErrMsg.Text = "User account suspended!";
                            }
                        }
                        else
                        {
                            ViewState["attempts"] = int.Parse(ViewState["attempts"].ToString()) + 1;
                            task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "attempts value " + ViewState["attempts"]);
                            Literal_ErrMsg.Text = GetUserFriendlyMessageAgainstCode(parts[0].Substring(0, 2));
                        }
                    }
                    else
                    {
                        if (parts[1].StartsWith("#"))
                        {
                            Literal_ErrMsg.Text = GetUserFriendlyMessageAgainstCode(parts[1].Substring(0, 2));
                        }
                        else
                        {
                            result = true;
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                throw;
            }
            finally
            {
                task.EndTask();
            }
            return result;
        }


        protected void ImageButton_SignIn_Click(object sender, ImageClickEventArgs e)
        {
            SqlCommand cmd = null;
            AppUser user = null;
            bool isValidUser = true;
            try
            {
                Session["width"] = screenWidth.Value;
                Session["height"] = screenHeight.Value;
                cmd = ConnectionFactory.GetNewCommand(true);


                if (isSecurityWebServiceEnabled == "1")
                    isValidUser = ValidateWithNBESecurityModule(0);
                else
                    ImageButton_RequestChangePassword.Visible = false;

                if (isValidUser)
                {
                    user = AppUser.LoadAppUser(" user_is_Active = 1 and USER_LOGIN='" + TextBox_signin.Text + "'");

                    if (user != null)
                    {
                        if (user.IsActiveDirectoryUser)
                        {
                            Impersonate impersonate = new Impersonate();
                            bool isLogin = impersonate.DoLogIn( ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).ActiveDirectoryDomain, TextBox_signin.Text, TextBox_password.Text);
                            if (isLogin)
                                impersonate.ShutDownHandle();
                            else
                                throw new Exception("User Id or Password is wrong!");
                        }
                        else if (!(user.UserPassword == Cryptic.EncryptString(TextBox_password.Text)))
                            throw new Exception("User Id or Password is wrong!");


                        List<ActiveSessions> list = ((List<ActiveSessions>)Application["ActiveSessions"]);
                        object obj = Application["sync"];
                        lock (obj)
                        {
                            IsUserAlreadyLoggedIn(list, TextBox_signin.Text.ToLower());
                            ActiveSessions newActiveSession = new ActiveSessions();
                            newActiveSession.userID = TextBox_signin.Text.ToLower();
                            newActiveSession.lastRequestSentAt = DateTime.Now;
                            newActiveSession.loggedInAt = DateTime.Now;
                            newActiveSession.machineName = Environment.MachineName;
                            
                            if (list == null)
                                list = new List<ActiveSessions>();
                            list.Add(newActiveSession);
                            Application["ActiveSessions"] = list;
                        }



                        //                    }

                        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT distinct(NAME), RIGHTS.RIGHT_ID FROM GROUP_USERS, GROUP_RIGHTS, RIGHTS " +
                                          "WHERE GROUP_USERS.USER_ID = " + user.UserId +
                                          " AND GROUP_USERS.GROUP_ID = GROUP_RIGHTS.GROUP_ID AND GROUP_RIGHTS.RIGHT_ID = RIGHTS.RIGHT_ID ";
                        DataTable dtUserRights = new DataTable();
                        dtUserRights.Columns.Add("NAME", typeof(string));
                        dtUserRights.Columns.Add("RIGHT_ID", typeof(int));
                        dtUserRights.Columns["RIGHT_ID"].Unique = true;
                        dtUserRights.PrimaryKey = new DataColumn[] { dtUserRights.Columns["RIGHT_ID"] };
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                            dtUserRights.Rows.Add(new object[] { reader[0].ToString().Replace(" ", ""), reader[1] });
                        reader.Close();

                        UserSetting usersettings = UserSetting.LoadUserSetting("user_id =" + user.UserId);
                        if (usersettings == null)
                        {
                            usersettings = new UserSetting();
                            usersettings.TreePanelWidth = 150;
                            usersettings.UserId = user.UserId;
                            usersettings.Save();
                        }

                        UserATMs.UserATMsReader userATMsReader = UserATMs.ExecuteReader("user_id = " + user.UserId);
                        StringBuilder builder = new StringBuilder(500);
                        if (userATMsReader.Read())
                            builder.Append(userATMsReader.CurrentUserATMs.ATMId.ToString());
                        while (userATMsReader.Read())
                            builder.Append("," + userATMsReader.CurrentUserATMs.ATMId);
                        userATMsReader.Close();

                        cmd.CommandText = "select org_mcn from user_organization where user_id = " + user.UserId;
                        DataTable dtUserOrgs = new DataTable();
                        dtUserOrgs.Columns.Add("MCN", typeof(string));
                        dtUserOrgs.Columns["MCN"].Unique = true;
                        dtUserOrgs.PrimaryKey = new DataColumn[] { dtUserOrgs.Columns["MCN"] };
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                            dtUserOrgs.Rows.Add(new object[] { reader[0] });
                        reader.Close();

                        Session[SessionVars.user.ToString()] = user;
                        Session[SessionVars.dtUserRights.ToString()] = dtUserRights;
                        Session[SessionVars.UserSettings.ToString()] = usersettings;
                        if (builder.Length > 0)
                            Session[SessionVars.SelectedATMs.ToString()] = builder.ToString();
                        Session[SessionVars.dtUserOrgs.ToString()] = dtUserOrgs;

                        Utility.BuildAuditLog("User successfully logged in", (int)user.UserId, (int)Permissions.Login);
                        Response.Redirect("Main.aspx", false);
                    }
                    else
                    {
                        Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;User Id or Password is wrong</img> </li>";
                    }
                }

            }
            catch (Exception ex)
            {
                Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;" + ex.Message + "</img>";
                LogableTask.LogMonoActivityTask("Login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd!=null)
                    if (cmd.Connection!=null)
                cmd.Connection.Close();
                trConfirmPassword.Visible = false;


            }

        }

        private void IsUserAlreadyLoggedIn(List<ActiveSessions> list, string userID)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].userID == userID)
                    {
                        if (list[i].lastRequestSentAt.AddMinutes(20) >= DateTime.Now && list[i].machineName !=Environment.MachineName)
                        {
                            throw new Exception("User with the same id already logged in");
                        }
                        else
                        {
                            ActiveSessions activeSession = list[i];
                            list.Remove(activeSession);
                        }
                    }
                }
            }
        }
        protected void ImageButton_Reset_Click(object sender, ImageClickEventArgs e)
        {
            TextBox_password.Text = "";
            TextBox_signin.Text = "";
            Literal_ErrMsg.Text = "";

            ImageButton_RequestChangePassword.ImageUrl = "images/key_info.png";
            Session["isConfirmPasswordVisible"] = null;
            Label_Info.Text = "Use your user id and password to login";
            Label_Password.Text = "Password:";
            trConfirmPassword.Visible = false;
            ImageButton_Login.Enabled = true;
            Page.SetFocus(TextBox_signin);
        }

        protected void ImageButton_RequestChangePassword_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Session["isConfirmPasswordVisible"] == null)
                {

                    //Get Response from webservice and then do the following.
                    //string[] parts = securityService.getGroup1(TextBox_signin.Text, TextBox_password.Text, progID);
                    try
                    {
                        bool isValidUser = ValidateWithNBESecurityModule(1);
                        if (isValidUser)
                        {
                            ImageButton_RequestChangePassword.ImageUrl = "images/ChangePwd.gif";
                            Label_Info.Text = "Enter your user id, New and Confirm password";
                            Label_Password.Text = "New Password:";
                            trConfirmPassword.Visible = true;
                            Session["isConfirmPasswordVisible"] = 1;
                            ImageButton_Login.Enabled = false;

                        }
                    }

                    catch (Exception ex)
                    {
                        trConfirmPassword.Visible = false;
                        throw;
                    }

                }
                else
                {
                    bool validated = true;
                    if (TextBox_password.Text != TextBox_ConfirmPassword.Text)
                    {
                        Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;Password and Confirm Password does not match.";
                        validated = false;
                    }
                    else if (TextBox_password.Text.Length < 8)
                    {
                        Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;Password cannot be less than 8 characters.";
                        validated = false;
                    }
                    if (validated)
                    {
                        SecurityService securityService = new SecurityService();
                        string[] parts = securityService.getGroup2(TextBox_signin.Text, TextBox_password.Text, progID);
                        if (parts.Length > 0)
                        {
                            if (parts[0].StartsWith("#"))
                            {
                                ViewState["attempts"] = int.Parse(ViewState["attempts"].ToString()) + 1;
                                Literal_ErrMsg.Text = GetUserFriendlyMessageAgainstCode(parts[0].Substring(0, 2));
                            }
                            else
                            {
                                string result = securityService.UpdPass(TextBox_signin.Text, TextBox_password.Text, progID);
                                if (result == "1")
                                {
                                    AppUser user = AppUser.LoadAppUser(" user_is_Active = 1 and USER_LOGIN='" + TextBox_signin.Text + "'");
                                    user.UserPassword = Cryptic.EncryptString(TextBox_password.Text);
                                    user.Save();
                                    ImageButton_Reset_Click(this, null);
                                    Literal_ErrMsg.Text = "<img src='images/working.png'>&nbsp;Password updated successfully.";


                                }
                                else
                                {
                                    Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;An error occured while updating password.";
                                }

                            }
                        }
                    }
                    else
                    {
                        ImageButton_Login.Enabled = false;
                    }

                    //ecurityObj.getGroup2(UserName.Text, NewPassword.Text, ProgId)

                    //Submit..


                }
            }
            catch (Exception ex)
            {
                Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;" + ex.Message;
                LogableTask.LogMonoActivityTask("ValidatingUser", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, ex);
            }
        }


    }
}