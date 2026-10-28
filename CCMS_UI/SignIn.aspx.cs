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

using Avanza.CCMS.DAL;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;
using ClientsideEncryption;
using Encryption;
using System.Reflection;
using Avanza.iSuite.UI;
using Lextm.SharpSnmpLib.Security;

namespace CCMSUI
{
    public delegate void SetCounter(int x);

    public class ActiveSessions
    {
        public string userID;
        public DateTime lastRequestSentAt;
        public DateTime loggedInAt;
        public string sessionId;
        public string browserLocalIdentifier;
        //public string responseCookie;

    }

    public partial class SignIn : BasePage
    {

        string loginURL = System.Web.Configuration.WebConfigurationManager.AppSettings["securityServiceWebURL"];
        string readDomainFromAppSetting = System.Configuration.ConfigurationSettings.AppSettings["readDomainFromAppSetting"];
        string progID = System.Configuration.ConfigurationSettings.AppSettings["ProgId"];
        string isSecurityWebServiceEnabled = System.Configuration.ConfigurationSettings.AppSettings["isSecurityWebServiceEnabled"];
        string CheckSignInAttempts = System.Configuration.ConfigurationSettings.AppSettings["CheckSignInAttempts"];
        string RetryAttemptCheck = System.Configuration.ConfigurationSettings.AppSettings["RetryAttemptCheck"];
        string IsEncodeSignIn = System.Configuration.ConfigurationSettings.AppSettings["IsEncodeSignIn"];
        string Domain = null;

        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_PROVIDER_DEFAULT = 0;

        const int ERROR_PASSWORD_MUST_CHANGE = 1907;
        const int ERROR_LOGON_FAILURE = 1326;
        const int ERROR_ACCOUNT_DISABLED = 1331;
        const int ERROR_ACCOUNT_LOCKED_OUT = 1909;      //It gives this error if the account is locked, REGARDLESS OF WHETHER VALID CREDENTIALS WERE PROVIDED!!!
        const int ERROR_ACCOUNT_EXPIRED = 1793;
        const int ERROR_PASSWORD_EXPIRED = 1330;

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
            Button_SignIn_Click(this, null);
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
                int loginType = string.IsNullOrEmpty(ConfigurationManager.AppSettings["LDAPLogonType"]) ? LOGON32_LOGON_NETWORK :
                    int.Parse(ConfigurationManager.AppSettings["LDAPLogonType"]);

                int loginProvider = string.IsNullOrEmpty(ConfigurationManager.AppSettings["LDAPLogonProvider"]) ?
                    LOGON32_PROVIDER_DEFAULT :
                    int.Parse(ConfigurationManager.AppSettings["LDAPLogonProvider"]);

                if (LogonUser(username, domain, password, loginType, loginProvider, out token) > 0)
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
            //Due to IE issue in BISB
            if (string.IsNullOrEmpty(IsEncodeSignIn) || !bool.Parse(IsEncodeSignIn))
                Button_SignIn.OnClientClick = "";

            if (!IsPostBack)
            {
                Page.SetFocus(TextBox_signin);

                //if (CheckSignInAttempts == "1")
                //{
                //    //Response.Cookies["RetryAttempts"].Value = RetryAttemptCheck;
                //    ////Response.Cookies["RetryAttempts"].Expires = DateTime.Now.AddMinutes(15);
                //    //Response.Cookies["RetryAttempts"].HttpOnly = true;
                //}
                if (Session[SessionVars.user.ToString()] == null)
                {
                    Response.Cookies.Remove("ASP.NET_SessionId");
                    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId",
                    RandomString.Generate(24, 24)));
                    Response.Cookies["ASP.NET_SessionId"].HttpOnly = true;
                }


                if (isSecurityWebServiceEnabled == "1")
                    Response.Redirect(loginURL);

                //TextBox_password.Attributes.Add("onkeydown", "return checkForSQLInjection()");
                //TextBox_signin.Attributes.Add("onkeydown", "return checkForSQLInjection()");


                if (Application[ApplicationVars.initError.ToString()] != null)
                    Literal_ErrMsg.Text = "<script>alert('" + ((string)Application[ApplicationVars.initError.ToString()]).Replace("\r\n", "").Replace("\\", "\\\\").Replace("'", "\\'") + "');</script>";
                trConfirmPassword.Visible = false;
                ViewState["attempts"] = 0;
                // if (isSecurityWebServiceEnabled == "0")
                //    ImageButton_RequestChangePassword.Visible = false;
                Literal_ErrMsg.Text = HttpContext.Current.User.Identity.Name;
            }

            //if (Request.Cookies["RetryAttempts"] == null)
            //{
            //    Response.Cookies["RetryAttempts"].Value = RetryAttemptCheck;
            //    //Response.Cookies["RetryAttempts"].Expires = DateTime.Now.AddMinutes(15);
            //}

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
                msg += "Unknown error code returned " + code;
            return msg;
        }

        private bool ValidateWithNBESecurityModule()
        {
            LogableTask task = LogableTask.NewTask("ValidateWithNBESecurityModule");
            bool result = false;
            try
            {
                task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "Going to create object");
                Service securityService = new Service();
                if (securityService == null)
                    throw new Exception("Unable to create service object");
                task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "object created");

                string UID = Request.QueryString["UID"];
                string userID = Request.QueryString["UserID"];
                string applicationID = Request.QueryString["ApplicationID"];
                string groupID = Request.QueryString["GroupID"];
                string ticketID = Request.QueryString["TicketID"];
                string key = Request.QueryString["Key"];


                string webApplicationID = System.Web.Configuration.WebConfigurationManager.AppSettings["ApplicationID"];
                string webGroupID1 = System.Web.Configuration.WebConfigurationManager.AppSettings["Group1"];
                string webGroupID2 = System.Web.Configuration.WebConfigurationManager.AppSettings["Group2"];
                string webGroupID3 = System.Web.Configuration.WebConfigurationManager.AppSettings["Group3"];
                string webGroupID4 = System.Web.Configuration.WebConfigurationManager.AppSettings["Group4"];
                string webGroupID5 = System.Web.Configuration.WebConfigurationManager.AppSettings["Group5"];

                if (securityService.CheckTicket(decimal.Parse(ticketID), int.Parse(groupID), int.Parse(applicationID), int.Parse(UID), userID, key))
                {
                    LogableTask.LogMonoActivityTask("login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "login on security web service successful");
                    result = true;
                }
                else
                    LogableTask.LogMonoActivityTask("login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "login on security web service failed");


                //if (id == 0)
                //{
                //    parts = securityService.che.getGroup(TextBox_signin.Text, TextBox_password.Text, progID);
                //    task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "return from getGroup call");
                //}
                //else
                //{
                //    parts = securityService.getGroup1(TextBox_signin.Text, TextBox_password.Text, progID);
                //    task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "return from getGroup1 call");
                //}


                //if (parts.Length > 0)
                //{
                //    if (parts[0].StartsWith("#"))
                //    {
                //        if (ViewState["attempts"].ToString() == "5")
                //        {
                //            task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "attempts value " + ViewState["attempts"] + ", Now going to suspend it");
                //            stopUserResponse = securityService.StopUser(TextBox_signin.Text, progID);
                //            task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "Response from StopUser Call " + stopUserResponse);
                //            if (stopUserResponse == "1")
                //            {
                //                Literal_ErrMsg.Text = "User account suspended!";
                //            }
                //        }
                //        else
                //        {
                //            ViewState["attempts"] = int.Parse(ViewState["attempts"].ToString()) + 1;
                //            task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "attempts value " + ViewState["attempts"]);
                //            Literal_ErrMsg.Text = GetUserFriendlyMessageAgainstCode(parts[0].Substring(0, 2));
                //        }
                //    }
                //    else
                //    {
                //        if (parts[1].StartsWith("#"))
                //        {
                //            Literal_ErrMsg.Text = GetUserFriendlyMessageAgainstCode(parts[1].Substring(0, 2));
                //        }
                //        else
                //        {
                //            result = true;
                //        }
                //    }


                //}
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

        protected void Button_SignIn_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = null;
            AppUser user = null;
            bool isValidUser = true;
            string[] parts = null;

            try
            {
                Session["width"] = screenWidth.Value;
                Session["height"] = screenHeight.Value;
                cmd = ConnectionFactory.GetNewCommand(true);
                bool isUsernameContainSpecialChars = CheckSpecialChars(TextBox_signin.Text);
                //Session["username"] = TextBox_signin.Text;
                //string sKey = TextBox_signin.Text;
                //string sUser = Convert.ToString(Cache[sKey]);
                //if (sUser == null || sUser == String.Empty)
                //{
                //    TimeSpan SessTimeOut = new TimeSpan(0, 0, HttpContext.Current.Session.Timeout, 0, 0);

                //    HttpContext.Current.Cache.Insert(sKey, sKey, null, DateTime.MaxValue, SessTimeOut,

                //    System.Web.Caching.CacheItemPriority.NotRemovable, null);

                //    Session["username"] = sKey;
                //}
                //else
                //{
                //    throw new Exception("User already logged in!");
                //    return;
                //}
                var password = !string.IsNullOrEmpty(IsEncodeSignIn) && bool.Parse(IsEncodeSignIn) ? AESEncrytDecry.DecryptStringAES(xpass.Value) : TextBox_password.Text;
                if ((isUsernameContainSpecialChars == false))
                {
                    //if ((Request.Cookies["RetryAttempts"].Value.ToString() != "0"))
                    //{
                    //bool username = CheckSpecialChars(TextBox_signin.Text);
                    //bool Password = CheckSpecialChars(TextBox_password.Text);

                    if (isSecurityWebServiceEnabled == "1")
                        isValidUser = ValidateWithNBESecurityModule();
                    //else
                    //    ImageButton_RequestChangePassword.Visible = false;

                    if (isValidUser)
                    {
                        if (TextBox_signin.Text.IndexOf("\\") > -1)
                            user = AppUser.LoadAppUser(" USER_LOGIN='" + TextBox_signin.Text.Split('\\')[1] + "'");
                        else
                            user = AppUser.LoadAppUser(" USER_LOGIN='" + TextBox_signin.Text + "'");

                        if (user == null)
                        {
                            throw new Exception("User Id or Password is wrong!");
                        }
                        else if (!user.IsActiveDirectoryUser && !(user.UserPassword == Encryption.Cryptic.EncryptString(password)))
                        {
                            user.RetryAttempt = user.RetryAttempt - 1;
                            if (user.RetryAttempt == 0)
                            {
                                user.UserModificationTime = DateTime.Now;
                                user.UserIsActive = false;
                                user.RetryAttempt = int.Parse(RetryAttemptCheck);
                            }
                            user.Save();
                            //AttemptReset();
                            throw new Exception("User Id or Password is wrong!");
                        }
                        else if (!user.UserIsActive)
                        {
                            throw new Exception("Your account is inactive. Please contact administrator");
                        }
                        else
                        {
                            //user.UserLastLoginTime = DateTime.Now;

                            user.Save();
                            if (user.IsActiveDirectoryUser)
                            {
                                if (readDomainFromAppSetting == "1")
                                {
                                    if (!TextBox_signin.Text.Contains(((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).ActiveDirectoryDomain))
                                    {
                                        TextBox_signin.Text = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).ActiveDirectoryDomain + "\\" + TextBox_signin.Text;
                                    }
                                }
                                parts = TextBox_signin.Text.Split('\\');

                                if (parts.Length > 1)
                                {
                                    //The IF condition is added for BisB
                                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["useDirectoryEntryForLogin"]) && bool.Parse(ConfigurationManager.AppSettings["useDirectoryEntryForLogin"]))
                                    {
                                        DirectoryEntry de = new DirectoryEntry(string.Format("LDAP://{0}", parts[0]), parts[1], password, AuthenticationTypes.Secure);
                                        object o = de.NativeObject;
                                    }
                                    else
                                    {
                                        Impersonate impersonate = new Impersonate();
                                        //bool isLogin = impersonate.DoLogIn(((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).ActiveDirectoryDomain, TextBox_signin.Text, TextBox_password.Text);
                                        bool isLogin = impersonate.DoLogIn(parts[0], parts[1], password);
                                        if (isLogin)
                                            impersonate.ShutDownHandle();
                                        else
                                        {
                                            // AttemptReset();
                                            throw new Exception(GetAdErrorMsg(Marshal.GetLastWin32Error()));
                                            //throw new Exception("User Id or Password is wrong!");
                                        }
                                    }
                                }
                                else
                                {
                                    // AttemptReset();
                                    throw new Exception("Please enter domain with User Id!");
                                }
                            }
                            user.RetryAttempt = int.Parse(RetryAttemptCheck);
                            user.Save();

                            if (!user.IsCurrentlyLoggedIn.HasValue)
                            {
                                user.IsCurrentlyLoggedIn = false;
                                user.LastRequestAt = DateTime.MinValue;
                            }
                            List<ActiveSessions> list = ((List<ActiveSessions>)Application["ActiveSessions"]);

                            //&& user.LastRequestAt.Value.AddMinutes(30) <=DateTime.Now

                            LogableTask.LogMonoActivityTask("Login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info,
                                        $"Browser Value {_t.Value.ToLower()} & User Login: {user.UserLogin.ToLower()} & Is Currently Login: {user.IsCurrentlyLoggedIn.Value}");
                            if (user.IsCurrentlyLoggedIn.Value && !IsUserLoginAgainAfterSignInUsingSameBrowser(list, user.UserLogin, _t.Value))
                            {
                                if (!(DateTime.Now > user.LastRequestAt.Value.AddMinutes(Session.Timeout)))
                                {
                                    LogableTask.LogMonoActivityTask("Login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info,
                                        $"user last request at[{user.LastRequestAt.Value.ToString("dd/MM/yyyy HH:mm:ss")}], Session timeout[{Session.Timeout}]");

                                    throw new Exception("User with the same id already logged in");
                                }
                            }
                            else
                            {
                                user.IsCurrentlyLoggedIn = true;
                                user.LastRequestAt = DateTime.Now;
                                user.Save();
                            }
                            //List<ActiveSessions> list = ((List<ActiveSessions>)Application["ActiveSessions"]);
                            object obj = Application["sync"];
                            lock (obj)
                            {
                                int idx = -1;
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].userID == user.UserLogin)
                                    {
                                        idx = i;
                                        break;
                                    }
                                }
                                //IsUserAlreadyLoggedIn(list, TextBox_signin.Text.ToLower());
                                if (idx == -1)
                                {
                                    ActiveSessions newActiveSession = new ActiveSessions();
                                    newActiveSession.userID = user.UserLogin.ToLower();
                                    newActiveSession.lastRequestSentAt = DateTime.Now;
                                    newActiveSession.loggedInAt = DateTime.Now;
                                    //newActiveSession.sessionId = Request.Cookies["ASP.NET_SessionId"].Value;
                                    newActiveSession.browserLocalIdentifier = _t.Value;
                                    //LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, newActiveSession.sessionId + "\r\n" + Response.Cookies["ASP.NET_SessionId"].Value);
                                    //  if (list == null)
                                    //     list = new List<ActiveSessions>();
                                    list.Add(newActiveSession);
                                }
                                else
                                {
                                    list[idx].browserLocalIdentifier = _t.Value;

                                    list[idx].lastRequestSentAt = DateTime.Now;
                                    list[idx].loggedInAt = DateTime.Now;
                                }

                                Application.Lock();
                                Application["ActiveSessions"] = list;
                                Application.UnLock();

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
                            Application[SessionVars.user.ToString()] = user;
                            Session[SessionVars.dtUserRights.ToString()] = dtUserRights;
                            Session[SessionVars.UserSettings.ToString()] = usersettings;
                            if (builder.Length > 0)
                                Session[SessionVars.SelectedATMs.ToString()] = builder.ToString();
                            Session[SessionVars.dtUserOrgs.ToString()] = dtUserOrgs;

                            Utility.BuildAuditLog("User successfully logged in", (int)user.UserId, (int)Permissions.Login);
                            Response.Redirect("Main.aspx", false);
                            //Server.Transfer("Main.aspx");
                        }
                    }

                    //}
                    //else
                    //{
                    //    user = AppUser.LoadAppUser(" USER_LOGIN='" + TextBox_signin.Text + "'");
                    //    if (user != null)
                    //    {
                    //        int? RetryAttempt = user.RetryAttempt;
                    //        RetryAttempt--;
                    //        user.RetryAttempt = RetryAttempt;
                    //        if (RetryAttempt == 0)
                    //        {
                    //            user.UserModificationTime = DateTime.Now;
                    //            user.UserIsActive = false;
                    //            user.RetryAttempt = int.Parse(RetryAttemptCheck);
                    //        }
                    //        user.Save();
                    //        Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;You are not allowed to login because of consecutive retry attempts.Please contact admin for further assistance</img> </li>";
                    //    }
                    //    else
                    //    {
                    //        Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;You are not allowed to login because of consecutive retry attempts.Please contact admin for further assistance</img> </li>";
                    //    }


                    //}
                }
                else
                {
                    Literal_ErrMsg.Text = "<img src='../images/icon_err.gif'>&nbsp;User Id or Password is wrong</img> </li>";
                }

            }
            catch (Exception ex)
            {
                Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;" + ex.Message + "</img>";
                LogableTask.LogMonoActivityTask("Login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                trConfirmPassword.Visible = false;


            }

        }

        private bool IsUserLoginAgainAfterSignInUsingSameBrowser(List<ActiveSessions> list, string userID, string localBrowserIdentifier)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    //string msg = $"user in list={list[i].userID}\r\ncurrent user={userID}\r\nsession id in list={list[i].sessionId}\r\nresponse ={Response.Cookies["ASP.NET_SessionId"].Value}";
                    LogableTask.LogMonoActivityTask("Login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info,
            $"User ID: {list[i].userID.ToLower()}  & Browser Identifier: {list[i].browserLocalIdentifier.ToLower()}");

                    //LogableTask.LogMonoActivityTask("", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, msg);
                    if (list[i].userID.ToLower() == userID.ToLower() && list[i].browserLocalIdentifier.ToLower() == localBrowserIdentifier.ToLower())
                    {
                        LogableTask.LogMonoActivityTask("Login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info,
           $"Returning True");
                        return true;
                        //if (list[i].lastRequestSentAt.AddMinutes(20) >= DateTime.Now && list[i].machineName != Environment.MachineName)
                        //{
                        //    throw new Exception("User with the same id already logged in");
                        //}
                        //else
                        //{
                        //    ActiveSessions activeSession = list[i];
                        //    list.Remove(activeSession);
                        //}
                    }
                    //else
                    //{
                    //ActiveSessions activeSession = list[i];
                    //list.Remove(activeSession);

                    //}
                }
            }

            LogableTask.LogMonoActivityTask("Login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info,
            $"Returning False");
            return false;

        }

        //private void IsUserAlreadyLoggedIn(List<ActiveSessions> list, string userID)
        //{
        //    if (list != null)
        //    {
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            if (list[i].userID == userID && list[i].lastRequestSentAt.AddMinutes(HttpContext.Current.Session.Timeout) >= DateTime.Now)
        //            {
        //                if (list[i].lastRequestSentAt.AddMinutes(20) >= DateTime.Now && list[i].machineName != Environment.MachineName)
        //                {
        //                    throw new Exception("User with the same id already logged in");
        //                }
        //                else
        //                {
        //                    ActiveSessions activeSession = list[i];
        //                    list.Remove(activeSession);
        //                }
        //            }
        //        }
        //    }
        //}
        protected void ImageButton_Reset_Click(object sender, EventArgs e)
        {
            TextBox_password.Text = "";
            TextBox_signin.Text = "";
            Literal_ErrMsg.Text = "";

            //ImageButton_RequestChangePassword.ImageUrl = "images/key_info.png";
            Session["isConfirmPasswordVisible"] = null;
            Label_Info.Text = "Please enter your user id and password to login";
            Label_Password.Text = "Password:";
            trConfirmPassword.Visible = false;
            Button_SignIn.Enabled = true;
            Page.SetFocus(TextBox_signin);
        }

        //protected void ImageButton_RequestChangePassword_Click(object sender, ImageClickEventArgs e)
        //{
        //    try
        //    {
        //        if (Session["isConfirmPasswordVisible"] == null)
        //        {

        //            //Get Response from webservice and then do the following.
        //            //string[] parts = securityService.getGroup1(TextBox_signin.Text, TextBox_password.Text, progID);
        //            try
        //            {
        //                bool isValidUser = ValidateWithNBESecurityModule(1);
        //                if (isValidUser)
        //                {
        //                    ImageButton_RequestChangePassword.ImageUrl = "images/ChangePwd.gif";
        //                    Label_Info.Text = "Enter your user id, New and Confirm password";
        //                    Label_Password.Text = "New Password:";
        //                    trConfirmPassword.Visible = true;
        //                    Session["isConfirmPasswordVisible"] = 1;
        //                    Button_SignIn.Enabled = false;

        //                }
        //            }

        //            catch (Exception ex)
        //            {
        //                trConfirmPassword.Visible = false;
        //                throw;
        //            }

        //        }
        //        else
        //        {
        //            bool validated = true;
        //            if (TextBox_password.Text != TextBox_ConfirmPassword.Text)
        //            {
        //                Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;Password and Confirm Password does not match.";
        //                validated = false;
        //            }
        //            else if (TextBox_password.Text.Length < 8)
        //            {
        //                Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;Password cannot be less than 8 characters.";
        //                validated = false;
        //            }
        //            if (validated)
        //            {
        //                SecurityService securityService = new SecurityService();
        //                string[] parts = securityService.getGroup2(TextBox_signin.Text, TextBox_password.Text, progID);
        //                if (parts.Length > 0)
        //                {
        //                    if (parts[0].StartsWith("#"))
        //                    {
        //                        ViewState["attempts"] = int.Parse(ViewState["attempts"].ToString()) + 1;
        //                        Literal_ErrMsg.Text = GetUserFriendlyMessageAgainstCode(parts[0].Substring(0, 2));
        //                    }
        //                    else
        //                    {
        //                        string result = securityService.UpdPass(TextBox_signin.Text, TextBox_password.Text, progID);
        //                        if (result == "1")
        //                        {
        //                            AppUser user = AppUser.LoadAppUser(" user_is_Active = 1 and USER_LOGIN='" + TextBox_signin.Text + "'");
        //                            user.UserPassword = Cryptic.EncryptString(TextBox_password.Text);
        //                            user.Save();
        //                            ImageButton_Reset_Click(this, null);
        //                            Literal_ErrMsg.Text = "<img src='images/working.png'>&nbsp;Password updated successfully.";


        //                        }
        //                        else
        //                        {
        //                            Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;An error occured while updating password.";
        //                        }

        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Button_SignIn.Enabled = false;
        //            }

        //            //ecurityObj.getGroup2(UserName.Text, NewPassword.Text, ProgId)

        //            //Submit..


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;" + ex.Message;
        //        LogableTask.LogMonoActivityTask("ValidatingUser", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, ex);
        //    }
        //}


        private string GetAdErrorMsg(int errorCode)
        {
            switch (errorCode)
            {
                case ERROR_ACCOUNT_LOCKED_OUT:
                    return "Account is locked. Please contract administrator";
                case ERROR_LOGON_FAILURE:
                    return "User Id or Password is wrong!";
                case ERROR_ACCOUNT_DISABLED:
                    return "Account is disabled. Please contact administrator";
                case ERROR_ACCOUNT_EXPIRED:
                    return "Account is expired. Please contact administrator";
                case ERROR_PASSWORD_MUST_CHANGE:
                case ERROR_PASSWORD_EXPIRED:
                    return "Password is expired. Please contact administrator";
                default:
                    return string.Format("Error Code: {0}. Contact administrator for detail.", errorCode);
            }
        }

        //public void AttemptReset()
        //{
        //    int tempAttempt = int.Parse(Request.Cookies["RetryAttempts"].Value.ToString());
        //    tempAttempt--;
        //    Response.Cookies["RetryAttempts"].Value = tempAttempt.ToString();
        //}

    }
}