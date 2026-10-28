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
using System.Reflection;
using System.Diagnostics;

using Avanza.CCMS.DAL;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace Avanza.iSuite.UI
{
    public partial class ApproveUser : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ApproveUser))
                    Response.Redirect("AccessDenied.aspx");

                BindGrid();
                Session["u"] = null;
            }
        }

        private void BindGrid()
        {
            try
            {
                StringBuilder commandText = new StringBuilder();
                //Commented by Danish
                //commandText.Append("select a.user_id, user_login, user_full_name, user_email, user_is_active, org_mcn from app_user a inner join user_organization u on u.user_id = a.user_id where a.user_id > 2");

                AppUser currentUser = (AppUser)Session[SessionVars.user.ToString()];
                DataTable dtUsers = PopulateUserList(currentUser);

                DataTable dtUsers_Copy = new DataTable();
                dtUsers_Copy.Columns.Add("user_id", typeof(int));
                dtUsers_Copy.Columns.Add("user_login", typeof(string));
                dtUsers_Copy.Columns.Add("user_full_name", typeof(string));
                dtUsers_Copy.Columns.Add("user_email", typeof(string));
                dtUsers_Copy.Columns.Add("Request_Mode", typeof(string));
                dtUsers_Copy.Columns.Add("approval_status", typeof(string));
                //dtUsers_Copy.Columns.Add("org_mcn", typeof(string));

                foreach (DataRow row in dtUsers.Rows)
                {
                    dtUsers_Copy.Rows.Add(new object[] { row["user_id"], row["user_login"], row["user_full_name"], row["user_email"], row["Request_Mode"], row["approval_status"] });
                }

                Cache[CacheVars.dtUsers.ToString()] = dtUsers_Copy;

                GridView_approveusers.DataSource = dtUsers_Copy;
                GridView_approveusers.DataBind();
                Literal_count.Text = dtUsers_Copy.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("bind user grid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

        private DataTable PopulateUserList(AppUser currentUser)
        {
            DataTable dtUserList = new DataTable();
            String commandText = String.Empty;
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

            commandText = @"select *,CASE WHEN is_added = 1 THEN 'Add Request' WHEN is_deleted = 1 THEN 'Delete Request' WHEN is_editied = 1 THEN 'Edit Request' ELSE NULL END AS Request_Mode 
                            from app_user 
                            where approval_status = 'Pending'";

            if (currentUser.UserId != 2)
                commandText += string.Format(" AND ((user_created_by != {0} AND user_modified_by IS NULL) OR (user_modified_by IS NOT NULL AND user_modified_by != {1}))", currentUser.UserId, currentUser.UserId);

            if (TextBox_userId.Text != String.Empty)
                commandText = commandText + String.Format(" and user_login like '%{0}%'", TextBox_userId.Text);

            if (TextBox_fullname.Text != String.Empty)
                commandText = commandText + String.Format(" and user_full_name like '%{0}%'", TextBox_fullname.Text);

            commandText += " order by user_login";

            cmd.CommandText = commandText;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtUserList);


            return dtUserList;
        }

        public SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;

                return (SortDirection)ViewState["sortDirection"];
            }
            set
            {
                ViewState["sortDirection"] = value;
            }
        }

        protected void GridView_approveusers_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = (DataTable)Cache[CacheVars.dtUsers.ToString()];
            DataView dv = new DataView(dt);
            GridViewSortDirection = (GridViewSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
            dv.Sort = e.SortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            GridView_approveusers.DataSource = dv;
            GridView_approveusers.DataBind();
        }

        protected void GridView_approveusers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trx = null;
            conn = ConnectionFactory.GetNewConnection();
            try
            {
                //AppUser _user = AppUser.LoadAppUserByPk(int.Parse(e.CommandArgument.ToString()));
                //AppUserApprover userApprover = AppUserApprover.LoadAppUseApproverrByuserLogin(_user.UserLogin);

                if (e.CommandName == "ViewUser")
                {
                    LogableTask task = null;

                    try
                    {
                        task = LogableTask.NewTask("View User");

                        AppUser _user = AppUser.LoadAppUser("user_login = '"+e.CommandArgument.ToString()+"'");
                        if (_user.IsDeleted == true && _user.ApprovalStatus == "Pending")
                        {
                            conn.Open();
                            trx = conn.BeginTransaction();
                            AppUserApprover userApprover = new AppUserApprover();
                            userApprover.UserFullName = _user.UserFullName.Replace("'", "''");
                            userApprover.CitId = _user.CitId;
                            userApprover.EmployeeManagerId = _user.EmployeeManagerId;
                            userApprover.IsActiveDirectoryUser = _user.IsActiveDirectoryUser;
                            userApprover.ManagerId = _user.ManagerId;
                            userApprover.MobileNumber = _user.MobileNumber;
                            userApprover.RetryAttempt = _user.RetryAttempt;
                            userApprover.UserCreatedBy = _user.UserCreatedBy;
                            userApprover.UserCreationTime = _user.UserCreationTime;
                            userApprover.UserEmail = _user.UserEmail;
                            userApprover.UserLastLoginTime = _user.UserLastLoginTime;
                            userApprover.UserLogin = _user.UserLogin;
                            userApprover.UserModificationTime = _user.UserModificationTime;
                            userApprover.UserModifiedBy = _user.UserModifiedBy;
                            userApprover.UserPassword = _user.UserPassword;
                            userApprover.UserType = _user.UserType;
                            userApprover.ApprovalStatus = "Pending";
                            userApprover.IsAdded = false;
                            userApprover.IsDeleted = true;
                            userApprover.IsEditied = false;
                            userApprover.UserIsActive = true;
                            userApprover.Save(conn, trx);
                            trx.Commit();
                            conn.Close();

                            Session["userApprover"] = userApprover.UserLogin;
                        }
                        else
                        {
                            AppUserApprover userApprover = AppUserApprover.LoadAppUseApproverrByuserLogin(e.CommandArgument.ToString());
                            Session["userApprover"] = userApprover.UserLogin;
                        }
                        Session["u"] = null;
                        Response.Redirect("AddEditUser.aspx", false);
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                        Literal_error.Text = "<li>An Error occured while view User. " + ex.Message + "</li>";

                    }
                    finally
                    {
                        task.EndTask();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //conn.Open();
            //trx = conn.BeginTransaction();

            //if (e.CommandName == "ApproveRequest")
            //{
            //    if (_user.IsAdded == true)
            //    {
            //        _user.ApprovalStatus = "Approved";
            //        _user.IsAdded = false;
            //        _user.UserIsActive = true;
            //        _user.Save(conn, trx);



            //        ConnectionFactory.ExecuteQuery("insert into group_users select * from group_users_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);

            //        ConnectionFactory.ExecuteQuery("insert into user_ATMs select * from user_ATMs_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);

            //        ConnectionFactory.ExecuteQuery("insert into user_organization (user_id, org_mcn) select user_id, org_mcn from user_organization_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);

            //        ConnectionFactory.ExecuteQuery("insert into ccms_user_cit (user_id,cit_id) select user_id,cit_id from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);

            //        ConnectionFactory.ExecuteQuery("insert into ccms_alert_notification (alert_type_id,organization_id,user_id) select alert_type_id,organization_id,user_id from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
            //    }
            //    if (_user.IsDeleted == true)
            //    {
            //        SqlCommand cmd = null;
            //        //AppUser user = AppUser.LoadAppUserByPk(int.Parse(e.CommandArgument.ToString()));
            //        cmd = ConnectionFactory.GetNewCommand(true);
            //        trx = cmd.Connection.BeginTransaction();
            //        cmd.CommandText = "DeleteUser";
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.Add("@user_id", SqlDbType.Int);
            //        cmd.Parameters[0].Value = int.Parse(e.CommandArgument.ToString());
            //        cmd.Transaction = trx;
            //        cmd.ExecuteNonQuery();
            //        //ConnectionFactory.ExecuteQuery("delete from App_user where user_id  = " + e.CommandArgument);
            //    }
            //    if (_user.IsEditied == true)
            //    {
            //        _user.UserFullName = userApprover.UserFullName;
            //        _user.CitId = userApprover.CitId;
            //        _user.EmployeeManagerId = userApprover.EmployeeManagerId;
            //        _user.IsActiveDirectoryUser = userApprover.IsActiveDirectoryUser;
            //        _user.ManagerId = userApprover.ManagerId;
            //        _user.MobileNumber = userApprover.MobileNumber;
            //        _user.RetryAttempt = userApprover.RetryAttempt;
            //        _user.UserCreatedBy = userApprover.UserCreatedBy;
            //        _user.UserCreationTime = userApprover.UserCreationTime;
            //        _user.UserEmail = userApprover.UserEmail;
            //        _user.UserLastLoginTime = userApprover.UserLastLoginTime;
            //        _user.UserLogin = userApprover.UserLogin;
            //        _user.UserModificationTime = userApprover.UserModificationTime;
            //        _user.UserModifiedBy = userApprover.UserModifiedBy;
            //        _user.UserPassword = userApprover.UserPassword;
            //        _user.UserType = userApprover.UserType;
            //        _user.ApprovalStatus = "Approved";
            //        _user.IsAdded = false;
            //        _user.IsDeleted = false;
            //        _user.IsEditied = false;
            //        _user.UserIsActive = true;
            //        _user.Save(conn, trx);


            //        ConnectionFactory.ExecuteQuery("DELETE FROM group_users where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("insert into group_users select * from group_users_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);

            //        ConnectionFactory.ExecuteQuery("delete from user_ATMs where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("insert into user_ATMs select * from user_ATMs_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);

            //        ConnectionFactory.ExecuteQuery("delete from user_organization where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("insert into user_organization (user_id, org_mcn) select user_id, org_mcn from user_organization_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);

            //        ConnectionFactory.ExecuteQuery("delete from ccms_user_cit where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("insert into ccms_user_cit(user_id,cit_id) select user_id,cit_id from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);

            //        ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("insert into ccms_alert_notification (alert_type_id,organization_id,user_id) select alert_type_id,organization_id,user_id from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
            //    }
            //    Literal_error.Text = "<li>User Approval Accepted!";
            //}
            //else if (e.CommandName == "DeclineRequest")
            //{
            //    if (_user.IsAdded == true)
            //    {
            //        ConnectionFactory.ExecuteQuery("delete from app_user where user_login  = '" + _user.UserLogin + "'");
            //        ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login  = '" + _user.UserLogin + "'");
            //        ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
            //    }
            //    if (_user.IsDeleted == true)
            //    {
            //        _user.ApprovalStatus = "Approved";
            //        _user.IsDeleted = false;
            //        _user.UserIsActive = true;
            //        _user.Save(conn, trx);
            //    }
            //    if (_user.IsEditied == true)
            //    {
            //        _user.ApprovalStatus = "Approved";
            //        _user.IsEditied = false;
            //        _user.UserIsActive = true;
            //        _user.Save(conn, trx);
            //        ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login  = '" + _user.UserLogin + "'");
            //        ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
            //        ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
            //    }
            //    Literal_error.Text = "<li>User Approval Decline!";
            //}

            //if (trx != null)
            //    trx.Commit();

            //BindGrid();
            //}
            //catch (Exception ex)
            //{
            //    if (trx != null)
            //        trx.Rollback();
            //    LogableTask task = LogableTask.NewTask("GridView_approveusers_RowCommand");
            //    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            //    task.EndTask();
            //}
            //finally
            //{
            //    if (conn != null && conn.State == ConnectionState.Open)
            //    {
            //        conn.Close();
            //    }
            //}

        }



        protected void GridView_approveusers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DataTable dt = (DataTable)Cache[CacheVars.dtUsers.ToString()];
            DataView dv = new DataView(dt);

            if (GridViewSortExpression != "")
                dv.Sort = GridViewSortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");

            this.GridView_approveusers.DataSource = dv;
            this.GridView_approveusers.PageIndex = e.NewPageIndex;
            this.GridView_approveusers.DataBind();
        }

        private string GridViewSortExpression
        {
            get
            {
                return ViewState["SortExpression"] as string ?? string.Empty;
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_fullname.Text = "";
            TextBox_userId.Text = "";
        }



    }
}