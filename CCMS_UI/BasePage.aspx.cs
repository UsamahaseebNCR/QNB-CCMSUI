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

using Avanza.CCMS.DAL;
using System.Collections.Generic;
using CCMSUI;
//using System.Web.Helpers;


namespace Avanza.iSuite.UI
{
    public partial class BasePage : System.Web.UI.Page
    {
        protected AppUser appUser;
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;
        //void Page_Load(object sender, EventArgs e)
        //{
        //    Response.Buffer = true;
        //    Response.ExpiresAbsolute = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
        //    Response.Expires = 0;
        //    Response.CacheControl = "no-cache";
        //}

        void Page_Init(object sender, EventArgs e)
        {
            
            ////Commented on 16 Dec 2016
            Response.Expires = -1;
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            
            if (Request.Headers["X-HTTP-METHOD"] != null)
            {
                throw new Exception("invalid request");
            }
            if (Request.Headers["X-HTTP-Method-Override"] != null)
            {
                throw new Exception("invalid request");
            }
            if (Request.Headers["X-METHOD-OVERRIDE"] != null)
            {
                throw new Exception("invalid request");
            }

            #region CSRF Tokenization

            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;
                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }
            Page.PreLoad += master_Page_PreLoad;
            #endregion


            //HttpCookie sameSiteCookie = new HttpCookie("sameSiteCookie");

            //// Set a value for the cookie
            //sameSiteCookie.Value = "sameSiteCookie";

            //// Set the secure flag, which Chrome's changes will require for SameSite none.
            //// Note this will also require you to be running on HTTPS
            //sameSiteCookie.Secure = true;

            //// Set the cookie to HTTP only which is good practice unless you really do need
            //// to access it client side in scripts.
            //sameSiteCookie.HttpOnly = true;

            //// Add the SameSite attribute, this will emit the attribute with a value of none.
            //// To not emit the attribute at all set the SameSite property to -1.
            //sameSiteCookie.SameSite = SameSiteMode.Strict;

            //// Add the cookie to the response cookie collection
            //Response.Cookies.Add(sameSiteCookie);

            ////Response.Cache.SetExpires(DateTime.Now);
            //Response.Cache.SetNoStore();

            // Response.Cache.SetCacheability(HttpCacheability.Public);
            // Response.Cache.SetValidUntilExpires(true);


            if (Request.Url.LocalPath.ToLower().Contains("atms.aspx"))
                Session["currentPage1"] = Request.Url.LocalPath;

            // this.Theme = "Default";
            // this.Theme = userSelectedTheme;
            if (!Request.Url.LocalPath.ToLower().Contains("main") && !HttpContext.Current.Request.FilePath.Contains("Dashboards"))
                Session["currentPage"] = Request.Url.LocalPath;
            if (Session[SessionVars.user.ToString()] == null)
            {
                if (HttpContext.Current.Request.FilePath.Contains("Dashboards"))
                    Response.Redirect("../../signin.aspx");

                else if (HttpContext.Current.Request.FilePath.Split('/').Length > 3)
                    Response.Redirect("../signin.aspx");
                else
                    Response.Redirect("signin.aspx");
                //Response.Write("<script> if (window.parent) window.parent.location ='../signin.aspx'; else window.location ='../signin.aspx'</script>");
                //Response.End();
            }
            appUser = (AppUser)Session[SessionVars.user.ToString()];
            if (appUser != null)
            {
                if (!appUser.LastRequestAt.HasValue || (DateTime.Now - appUser.LastRequestAt.Value).TotalMinutes > 2)
                {
                    appUser.LastRequestAt = DateTime.Now;
                    appUser.Save();
                }
            }
            try
            {
                if (form1 != null)
                {
                    //HtmlGenericControl htmlCtrl = new HtmlGenericControl("div");
                    //htmlCtrl.ID = "divCtrl";
                    //htmlCtrl.InnerHtml = AntiForgery.GetHtml().ToString();
                    //form1.Controls.Add(htmlCtrl);
                    form1.Attributes.Add("autocomplete", "off");

                    for (int i = 0; i < form1.Controls.Count; i++)
                    {
                        if (form1.Controls[i].GetType() == typeof(TextBox))
                        {

                            ((TextBox)form1.Controls[i]).Attributes.Add("onkeydown", "return checkForSQLInjection()");


                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }

            List<ActiveSessions> list = ((List<ActiveSessions>)Application["ActiveSessions"]);
            object obj = Application["sync"];
            lock (obj)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].userID == ((AppUser)Session[SessionVars.user.ToString()]).UserLogin.ToLower())
                        list[i].lastRequestSentAt = DateTime.Now;
                }
            }

            //if (Request.Cookies["__RequestVerificationToken"] != null)
            //    AntiForgery.Validate();

        }
        void Page_OnInit(object sender, EventArgs e)
        {
            if (CheckForSessionTimeout())
            {
                if (Page.Request.IsAuthenticated)
                {
                    FormsAuthentication.SignOut();
                }
                NavigateToLoginPage(true);
            }
        }
        public void NavigateToLoginPage(bool TimeOutHasOccurred)
        {
            Response.Redirect("~/SignIn.aspx?mode=timeout");
        }

        /// <summary>
        /// if dtUserRights is not in sessin then redirects to login page, else gives the result of  
        /// Session[SessionVars.dtUserRights.ToString()]).Rows.Contains
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        protected bool CheckRight(int right)
        {
            DataTable dtRights = (DataTable)Session[SessionVars.dtUserRights.ToString()];
            if (dtRights == null)
            {
                Response.Redirect("../signin.aspx");
                //Response.Write("<script> if (window.parent) window.parent.location ='../signin.aspx'; else window.location ='../signin.aspx'</script>");
                //Response.End();
            }

            Permissions currentPermission = (Permissions)Enum.ToObject(typeof(Permissions), right);
            String permissionName = currentPermission.ToString();

            bool hasRight = false;

            hasRight = dtRights.Select(String.Format("NAME ='{0}'", permissionName)).Length > 0;

            return hasRight;
        }

        protected bool CheckRight(int right, bool fromImmediateChildDir)
        {
            DataTable dtRights = (DataTable)Session[SessionVars.dtUserRights.ToString()];
            if (dtRights == null)
            {
                if (Request.Url.ToString().Contains("DashBoards"))
                    Response.Redirect("../../signin.aspx");
                else if (fromImmediateChildDir)
                    Response.Redirect("../signin.aspx");
                //Response.Write("<script> if (window.parent) window.parent.location ='../signin.aspx'; else window.location ='../signin.aspx'</script>");
                else
                    Response.Redirect("signin.aspx");
                //Response.Write("<script> if (window.parent) window.parent.location ='signin.aspx'; else window.location ='CCMS/signin.aspx'</script>");

            }

            Permissions currentPermission = (Permissions)Enum.ToObject(typeof(Permissions), right);
            String permissionName = currentPermission.ToString();

            bool hasRight = false;

            hasRight = dtRights.Select(String.Format("NAME ='{0}'", permissionName)).Length > 0;

            return hasRight;
        }
        private static string userSelectedTheme;
        public static string UserSelectedTheme
        {
            get { return userSelectedTheme; }
            set { userSelectedTheme = value; }
        }

        protected int GetColumnIndexByName(GridView grid, string name)
        {
            foreach (DataControlField col in grid.Columns)
            {
                if (col.HeaderText.ToLower().Trim() == name.ToLower().Trim())
                {
                    return grid.Columns.IndexOf(col);
                }
            }

            return -1;
        }

        protected int ValidateNoOfCassettes(string pKeyValue)
        {
            const int maxValue = 7;
            const int minValueToHandle = 4;
            int noOfCassettes = 4;
            if (pKeyValue != null)
            {
                ///<Summary>
                ///Edited by Ali Shah
                ///We will only have to adjust our webforms for NoOfCassettes 4,5,6 or 7.
                if (int.TryParse(pKeyValue, out noOfCassettes))
                    noOfCassettes = (noOfCassettes >= minValueToHandle && noOfCassettes <= maxValue) ? noOfCassettes : 4;
            }
            return noOfCassettes;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue ||
                   (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        public bool CheckForSessionTimeout()
        {
            if (Context.Session != null && Context.Session.IsNewSession)
            {
                string cookieHeader = Page.Request.Headers["Cookie"];
                if ((null != cookieHeader) && (cookieHeader.IndexOf("ASP.NET_SessionId") >= 0))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        protected bool CheckSpecialChars(string str)
        {
            // Create  a string array and add the special characters you want to remove
            string[] chars = new string[] { ",", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", "|", "[", "]", "=", "\r", "\n" };
            bool result = false;
            //Iterate the number of times based on the String array length.
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    result = true;
                    break;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
    }
}