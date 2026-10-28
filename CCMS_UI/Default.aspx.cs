using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using Avanza.CCMS.DAL;
using System.Data;
using System.Text;

namespace CCMSUI
{
    public partial class Default : System.Web.UI.Page
    {
        string userID = null;
        string loginURL = System.Web.Configuration.WebConfigurationManager.AppSettings["securityServiceWebURL"];

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            DoLogin();
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.Cookies.Remove("ASP.NET_SessionId");
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId",
                RandomString.Generate(24, 24)));
                Literal_Script.Text = "<script>call();</script>";
            }

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
                userID = Request.QueryString["UserID"];
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
        protected void DoLogin()
        {
            SqlCommand cmd = null;
            AppUser user = null;
            bool isValidUser = true;
            string[] parts = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                isValidUser = ValidateWithNBESecurityModule();

                if (isValidUser)
                {
                    user = AppUser.LoadAppUser(" user_is_Active = 1 and USER_LOGIN='" +userID + "'");

                    if (user != null)
                    {
                        user.UserLastLoginTime = DateTime.Now;
                        user.Save();
                        
                        List<ActiveSessions> list = ((List<ActiveSessions>)Application["ActiveSessions"]);
                        object obj = Application["sync"];
                        lock (obj)
                        {
                            IsUserAlreadyLoggedIn(list, userID.ToLower());
                            ActiveSessions newActiveSession = new ActiveSessions();
                            newActiveSession.userID = userID.ToLower();
                            newActiveSession.lastRequestSentAt = DateTime.Now;
                            newActiveSession.loggedInAt = DateTime.Now;
                            newActiveSession.sessionId = Response.Cookies["ASP.NET_SessionId"].Value;

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
                        Response.Redirect(loginURL);
                        //Literal_ErrMsg.Text = "<img src='images/icon_err.gif'>&nbsp;User Id or Password is wrong</img> </li>";
                    }
                }

            }
            catch (Exception ex)
            {
                Response.Redirect(loginURL);
                
                //LogableTask.LogMonoActivityTask("Login", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                //trConfirmPassword.Visible = false;


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
                        if (list[i].lastRequestSentAt.AddMinutes(20) >= DateTime.Now && list[i].sessionId != Request.Cookies["ASP.NET_SessionId"].Value)
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


    }
}