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

using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using Avanza.iSuite.UI;
using System.DirectoryServices;
using System.Threading;
using System.Linq;

namespace CCMSUI
{
    public partial class AddActiveDirectoryUsers : BasePage
    {
        DataTable dtAppUser = new DataTable();

        private void DisplayMessage()
        {
            this.Page.Response.Write("<DIV ID='loadingScreen'><TABLE  WIDTH='100%' HEIGHT='100%' BORDER='0'><TR><TD valign = 'top'><font size='3' COLOR='black'>Loading... Please wait</font><p><IMG SRC='../images/loading.gif' BORDER='0'></p></TD></TR></TABLE></DIV>");
            this.Page.Response.Flush();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                if (!Page.IsPostBack)
                {
                    //if (Session[SessionVars.CSVUsers.ToString()] != null)
                    //{
                    //    if (!CheckRight((int)Permissions.ImportUsersFromCSV))
                    //        Response.Redirect("AccessDenied.aspx");
                    //}
                    //else
                    //    
                    if (!CheckRight((int)Permissions.AddActiveDirectoryUsers))
                            Response.Redirect("AccessDenied.aspx");

                    Thread thread = new Thread(new ThreadStart(DisplayMessage));
                    thread.Start();
                    TextBox_userId.Text = (string)Session[SessionVars.ADSearch.ToString()];
                    BindGrid("user_name asc");
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
            finally
            { task.EndTask(); }
        }

        protected void DG_ADUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (dtAppUser.Select("user_login = '" + this.DG_ADUsers.DataKeys[e.Row.DataItemIndex].Values["user_id"].ToString().Replace("'","''") + "'").Length > 0)
                    ((CheckBox)e.Row.FindControl("chkbx")).Enabled = false;
            }
        }


        public static DataTable GetADTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("user_id", typeof(string));
            dt.Columns.Add("user_email", typeof(string));
            dt.Columns.Add("user_name", typeof(string));
            return dt;

        }
        private DataTable GetActiveDirectoryUsers()
        {

            DataTable dtActiveDirectoryUsers = null;
            if (System.Web.HttpContext.Current.Cache["activeDirectoryUsers"] == null)
            {
                dtActiveDirectoryUsers = GetADTableSchema();
                //"LDAP://OU=EMEA,OU=User Accounts,DC=corp,DC=ncr,DC=com"
                DirectoryEntry de = new DirectoryEntry(System.Configuration.ConfigurationManager.AppSettings["LDAPPath"], (string)Session[SessionVars.ADUserName.ToString()], (string)Session[SessionVars.ADPassword.ToString()], AuthenticationTypes.Secure);
                object o = de.NativeObject;

                DirectorySearcher searcher = new DirectorySearcher(de);
                searcher.SizeLimit = System.Int32.MaxValue;
                searcher.PageSize = System.Int32.MaxValue;
 
                de.Children.SchemaFilter.Add("user");

                //searcher.SearchScope = SearchScope.OneLevel;
                searcher.PropertiesToLoad.Add("SAMAccountName");
                searcher.PropertiesToLoad.Add("userPrincipalName");
                searcher.PropertiesToLoad.Add("cn");
                SearchResultCollection results = searcher.FindAll();
                foreach (SearchResult result in results)
                {

                    if (result.Properties.Contains("SAMAccountName")
                        && result.Properties.Contains("userPrincipalName") &&
                        result.Properties.Contains("cn"))
                    {
                        DataRow dr = dtActiveDirectoryUsers.NewRow();
                        dr[0] = result.Properties["SAMAccountName"][0];
                        dr[1] = result.Properties["userPrincipalName"][0];
                        dr[2] = result.Properties["cn"][0];

                        dtActiveDirectoryUsers.Rows.Add(dr);
                    }
                }

                System.Web.HttpContext.Current.Cache.Add("activeDirectoryUsers", dtActiveDirectoryUsers, null, DateTime.MaxValue, new TimeSpan(0, 30, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            else
                dtActiveDirectoryUsers = (DataTable)System.Web.HttpContext.Current.Cache["activeDirectoryUsers"];
            return dtActiveDirectoryUsers.Select(
                string.Format("user_id LIKE '%{0}%'", TextBox_userId.Text)).Any() ? dtActiveDirectoryUsers.Select(
                string.Format("user_id LIKE '%{0}%'", TextBox_userId.Text)).CopyToDataTable() : GetADTableSchema();
        }

        private void BindGrid(string order)
        {
            DataTable dtActiveDirectoryUsers=null;
            //Load Active Directory Users
            //if (Request.QueryString["fromCSV"] == null)
            //{
            //    
                string[] parts = ((string)Session[SessionVars.ADUserName.ToString()]).Split('\\');
                SignIn.Impersonate impersonate = (SignIn.Impersonate)Session["impersonate"];
                //impersonate.DoLogIn(

                if (!(impersonate.DoImpersonate()))
                    throw new Exception("unable to impersonate");

                dtActiveDirectoryUsers = GetActiveDirectoryUsers();
                impersonate.Revert();
            //}
            //else
            //    dtActiveDirectoryUsers = (DataTable)Session[SessionVars.CSVUsers.ToString()];

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select user_login from app_user";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtAppUser);
            dtActiveDirectoryUsers.DefaultView.Sort = order;
            DG_ADUsers.DataSource = dtActiveDirectoryUsers;
            DG_ADUsers.DataBind();

            if (dtActiveDirectoryUsers.Rows.Count == 0)
            {
                DivChoices1.Visible = false;
                DivChoices2.Visible = false;
                throw new Exception("No users found in the active directory!");
            }
        }

        protected void DG_ADUsers_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            BindGrid("user_name asc");
        }

        protected void Import_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Import AD Users");
            DataTable dtADUsers = GetADTableSchema();
            try
            {
                foreach (GridViewRow row in DG_ADUsers.Rows)
                {
                    if (((CheckBox)row.FindControl("chkbx")).Checked && ((CheckBox)row.FindControl("chkbx")).Enabled)
                    {
                        DataRow dr = dtADUsers.NewRow();
                        dr[0] = this.DG_ADUsers.DataKeys[row.DataItemIndex].Values["user_id"].ToString(); ;
                        dr[1] = this.DG_ADUsers.DataKeys[row.DataItemIndex].Values["user_email"].ToString(); ;
                        dr[2] = this.DG_ADUsers.DataKeys[row.DataItemIndex].Values["user_name"].ToString(); ;
                        dtADUsers.Rows.Add(dr);
                    }
                }
                Session[SessionVars.dtAppUser.ToString()] = dtADUsers;
                ((SignIn.Impersonate)Session["impersonate"]).ShutDownHandle();
                //Session[SessionVars.CSVUsers.ToString()] = null; // to indicate that users are uploading from AD
               // if (Request.QueryString["fromCSV"] == null)
                    Response.Redirect("AssignATMsToADUsers.aspx");
               // else
               //     Response.Redirect("AssignATMsToADUsers.aspx?fromCSV=1");
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            try
            {
                BindGrid("user_name asc");
            }
            catch (Exception ex)
            {
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
        }
    }
}