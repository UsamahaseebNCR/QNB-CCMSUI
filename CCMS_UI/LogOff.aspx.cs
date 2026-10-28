using Avanza.CCMS.DAL;
using System;
using System.Web;

namespace CCMSUI
{
    public partial class LogOff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            AppUser appUser = ((AppUser)Session[SessionVars.user.ToString()]);
            if (appUser != null)
            {
                appUser.IsCurrentlyLoggedIn = false;
                appUser.Save();
                Utility.BuildAuditLog("User successfully logged off", appUser.UserId, (int)Permissions.Login);

            }


            //List<ActiveSessions> list = ((List<ActiveSessions>)Application["ActiveSessions"]);
            //AppUser appUser = null;
            ////Change on 02_dec_2014
            ////if (list != null) 

            //if (list != null && Session[SessionVars.user.ToString()]!=null)
            //{
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        appUser = ((AppUser)Session[SessionVars.user.ToString()]);
            //        if (list[i].userID == appUser.UserLogin.ToLower())
            //        {
            //            LogableTask.LogMonoActivityTask("", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "user exists in list "+ list[i].userID);

            //            ActiveSessions activeSession = list[i];
            //            object obj = Application["sync"];
            //            if (obj != null)
            //            {
            //                lock (obj)
            //                {
            //                    Application.Lock();
            //                    list.Remove(activeSession);
            //                    Application["ActiveSessions"] = list;
            //                    Application.UnLock();
            //                    LogableTask.LogMonoActivityTask("", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "user removed from  list " + appUser.UserId);


            //                }
            //            }
            //            //Write log off
            //        }
            //    }
            //}

            Session.Abandon();
            Session.RemoveAll();
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            Response.Redirect("signin.aspx");
        }
    }
}