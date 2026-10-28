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
    public partial class User : BasePage
    {
        string enablemakeachecker = System.Web.Configuration.WebConfigurationManager.AppSettings["enablemakeachecker"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewUserList))
                    Response.Redirect("AccessDenied.aspx");

                if (!CheckRight((int)Permissions.ModifyUser))
                {
                    GridView_users.Columns[6].Visible = false;
                    GridView_users.Columns[4].Visible = false;
                }

                if (!CheckRight((int)Permissions.DeleteUser))
                    GridView_users.Columns[5].Visible = false;

                if (!CheckRight((int)Permissions.CreateUser))
                    HyperLink_newUser.Visible = false;

                if (!CheckRight((int)Permissions.AddActiveDirectoryUsers))
                    HyperLink_AddADUser.Visible = false;
                BindGrid();

                if(enablemakeachecker=="false")
                {
                    GridView_users.Columns[3].Visible = false;
                }
                Session["u"] = null;
                Session["userApprover"] = null;
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
                dtUsers_Copy.Columns.Add("user_is_active", typeof(bool));
                dtUsers_Copy.Columns.Add("approval_status", typeof(string));
                //dtUsers_Copy.Columns.Add("org_mcn", typeof(string));

                foreach (DataRow row in dtUsers.Rows)
                {
                    dtUsers_Copy.Rows.Add(new object[] { row["user_id"], row["user_login"], row["user_full_name"], row["user_email"], row["user_is_active"], row["approval_status"] });
                }

                Cache[CacheVars.dtUsers.ToString()] = dtUsers_Copy;

                GridView_users.DataSource = dtUsers_Copy;
                GridView_users.DataBind();
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

            if (currentUser.UserType.Equals("NCR"))
            {
                if (currentUser.UserId.Equals(2))
                {
                    commandText = "select * from app_user where 1=1";

                    if (TextBox_userId.Text != String.Empty)
                        commandText = commandText + String.Format(" and user_login like '%{0}%'", TextBox_userId.Text);

                    if (TextBox_fullname.Text != String.Empty)
                        commandText = commandText + String.Format(" and user_full_name like '%{0}%'", TextBox_fullname.Text);

                    commandText += " order by user_login";
                }
                else
                {
                    commandText = "GetUserListForNCRUser";
                    cmd.Parameters.Add("@userId", SqlDbType.BigInt).Value = currentUser.UserId;
                    cmd.Parameters.Add("@userName", SqlDbType.VarChar).Value = String.Format("%{0}%", TextBox_userId.Text);
                    cmd.Parameters.Add("@userFullName", SqlDbType.VarChar).Value = String.Format("%{0}%", TextBox_fullname.Text);
                    cmd.CommandType = CommandType.StoredProcedure;
                }
            }
            else if (currentUser.UserType.Equals("BANK"))
            {
                commandText = String.Format(@"select 
	                                app_user.*
                                from 
	                                app_user inner join ccms_user_organization on app_user.user_id = ccms_user_organization.user_id
                                where ccms_user_organization.organization_id in (select organization_id from ccms_user_organization where user_id = {0})", currentUser.UserId);

                if (TextBox_userId.Text != String.Empty)
                    commandText = commandText + String.Format(" and user_login like '%{0}%'", TextBox_userId.Text);

                if (TextBox_fullname.Text != String.Empty)
                    commandText = commandText + String.Format(" and user_full_name like '%{0}%'", TextBox_fullname.Text);
            }
            else if (currentUser.UserType.Equals("CIT"))
            {
                commandText = String.Format(@"select 
	                            app_user.*
                            from	
	                            app_user inner join ccms_user_cit on app_user.user_id = ccms_user_cit.user_id
                            where ccms_user_cit.cit_id in (select cit_id from ccms_user_cit where user_id = {0})", currentUser.UserId);

                if (TextBox_userId.Text != String.Empty)
                    commandText = commandText + String.Format(" and user_login like '%{0}%'", TextBox_userId.Text);

                if (TextBox_fullname.Text != String.Empty)
                    commandText = commandText + String.Format(" and user_full_name like '%{0}%'", TextBox_fullname.Text);
            }

            cmd.CommandText = commandText;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtUserList);

            //    cmd.Connection.Close();

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

        protected void GridView_users_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = (DataTable)Cache[CacheVars.dtUsers.ToString()];
            DataView dv = new DataView(dt);
            GridViewSortDirection = (GridViewSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
            dv.Sort = e.SortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            GridView_users.DataSource = dv;
            GridView_users.DataBind();
        }

        protected void GridView_users_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "deactivateUser")
                {
                    AppUser user = AppUser.LoadAppUserByPk(int.Parse(e.CommandArgument.ToString()));
                    ConnectionFactory.ExecuteQuery("update App_user set user_is_active =0 where user_id  = " + e.CommandArgument);
                    Literal_error.Text = "<li>User de-activated!";            
                    Utility.BuildAuditLog("User [" + user.UserFullName.Replace("'", "''") + "] deactivated", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeactivateUser);
                }
                else if (e.CommandName == "activateUser")
                {
                    AppUser user = AppUser.LoadAppUserByPk(int.Parse(e.CommandArgument.ToString()));
                    ConnectionFactory.ExecuteQuery("update App_user set user_is_active =1 where user_id  = " + e.CommandArgument);
                    Literal_error.Text = "<li>User activated!";
                    Utility.BuildAuditLog("User [" + user.UserFullName.Replace("'", "''") + "] Activated", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ActivateUser);

                }
                else if (e.CommandName == "editUser")
                {
                    LogableTask task = null;

                    try
                    {
                        task = LogableTask.NewTask("edit user");

                        AppUser user = AppUser.LoadAppUserByPk(int.Parse(e.CommandArgument.ToString()));
                        Session["u"] = user.UserId;
                        Session["userApprover"] = null;
                        Response.Redirect("AddEditUser.aspx", false);
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                        Literal_error.Text = "<li>An Error occured while Edit User. " + ex.Message + "</li>";

                    }
                    finally
                    {
                        task.EndTask();
                    }
                }
                else if (e.CommandName == "deleteUser")
                {
                    SqlCommand cmd = null;
                    SqlTransaction trx = null;
                    LogableTask task = LogableTask.NewTask("delete user");
                    try
                    {
                        AppUser user = AppUser.LoadAppUserByPk(int.Parse(e.CommandArgument.ToString()));
                        //if (user.ApprovalStatus == "Decline")
                        //{
                        //    cmd = ConnectionFactory.GetNewCommand(true);
                        //    trx = cmd.Connection.BeginTransaction();
                        //    cmd.CommandText = "DeleteUser";
                        //    cmd.CommandType = CommandType.StoredProcedure;
                        //    cmd.Parameters.Add("@user_id", SqlDbType.Int);
                        //    cmd.Parameters[0].Value = int.Parse(e.CommandArgument.ToString());
                        //    cmd.Transaction = trx;
                        //    cmd.ExecuteNonQuery();


                        //    ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login  = '" + user.UserLogin + "'");
                        //    ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + user.UserId, trx);
                        //    ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + user.UserId, trx);
                        //    ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + user.UserId, trx);
                        //    ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + user.UserId, trx);
                        //    ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + user.UserId, trx);

                        //    trx.Commit();
                        //}
                        //else
                        //{
                        if (enablemakeachecker=="false")
                        {
                            //AppUser user = AppUser.LoadAppUserByPk(int.Parse(e.CommandArgument.ToString()));
                            cmd = ConnectionFactory.GetNewCommand(true);
                            trx = cmd.Connection.BeginTransaction();
                            cmd.CommandText = "DeleteUser";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@user_id", SqlDbType.Int);
                            cmd.Parameters[0].Value = user.UserId;
                            cmd.Transaction = trx;
                            cmd.ExecuteNonQuery();

                            //ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login  = '" + user.UserLogin + "'");
                            trx.Commit();
                            BindGrid();
                            Literal_ok.Text = "<li>Selected user deleted</li>";
                            Utility.BuildAuditLog("User [" + user.UserFullName.Replace("'", "''") + "] has been deleted successfully", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteUser);
                        }
                        else
                        {
                            ConnectionFactory.ExecuteQuery(string.Format("update App_user set is_deleted = 1, approval_status = 'Pending', user_modified_by = {0}, user_modification_time= '{1}' where user_id  = {2}", ((AppUser)Session[SessionVars.user.ToString()]).UserId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), user.UserId));
                            Literal_ok.Text = "<li>Selected user deletion request has been sent for approval!</li>";
                            
                            Utility.BuildAuditLog("Request to delete User [" + user.UserFullName.Replace("'", "''") + "] has been sent for approval", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                        Literal_error.Text = "<li>An Error occured while deleting User. " + ex.Message + "</li>";
                        //trx.Rollback();
                    }
                    finally
                    {
                        //cmd.Connection.Close();
                        task.EndTask();
                    }
                }

                BindGrid();
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("GridView_users_RowCommand");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
            }
        }

        protected void GridView_users_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                // changes by Wajahat 2-04-2018
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDeactivate = (ImageButton)e.Row.FindControl("imgbtn_deActivate");
                    ImageButton btnActivate = (ImageButton)e.Row.FindControl("imgbtn_activate");
                    ImageButton btnEditUser = (ImageButton)e.Row.FindControl("imgbtn_edituser");
                    ImageButton btnDeleteUser = (ImageButton)e.Row.FindControl("imgbtn_delete");

                    if ((bool)((DataRowView)e.Row.DataItem)["user_is_active"])
                    {
                        btnActivate.Visible = false;
                        btnDeactivate.ToolTip = "Click to de-activate user";
                        btnDeactivate.ImageUrl = "../images/deactivate.jpg";
                        btnDeactivate.OnClientClick = "return confirm('Are you sure you want to de-activate User " + ((DataRowView)e.Row.DataItem).Row["user_login"] + "?','CCMS')";
                    }
                    else
                    {
                        ((HyperLink)e.Row.Cells[0].FindControl("HyperLink_UserId")).NavigateUrl = "";
                        btnDeactivate.Visible = false;
                        btnActivate.ImageUrl = "../images/activate.jpg";
                        btnActivate.ToolTip = "Click to activate user";
                        btnActivate.OnClientClick = "return confirm('Are you sure you want to activate User " + ((DataRowView)e.Row.DataItem).Row["user_login"] + "?','CCMS')";
                    }
                    if ((int)((DataRowView)e.Row.DataItem)["user_id"] < 2)
                    {
                        e.Row.Cells[3].Visible = false;
                        e.Row.Cells[4].Visible = false;
                        e.Row.Cells[5].Visible = false;
                    }
                    AppUser user = ((AppUser)Session[SessionVars.user.ToString()]);
                    if (CheckRight((int)Permissions.ModifyUser) && (bool)((DataRowView)e.Row.DataItem)["user_is_active"])
                    {
                        if (((int)((DataRowView)e.Row.DataItem)["user_id"] != user.UserId && (int)((DataRowView)e.Row.DataItem)["user_id"] > 2))
                        {
                            if (e.Row.FindControl("btn_edituser") != null)
                            {
                                btnEditUser.Visible = true;
                            }
                        }
                    }
                    if (enablemakeachecker == "true")
                    {
                        if (((DataRowView)e.Row.DataItem)["approval_status"] != DBNull.Value)
                        {
                            if ((string)((DataRowView)e.Row.DataItem)["approval_status"] == "Pending")
                            {
                                btnDeleteUser.Visible = false;
                                btnActivate.Visible = false;
                                btnDeactivate.Visible = false;
                                btnEditUser.Visible = false;
                            }
                        }
                    }
                    
                    

                    //if (CheckRight((int)Permissions.ModifyUser) && (bool)((DataRowView)e.Row.DataItem)["user_is_active"])
                    //{
                    //    //AppUser user = ((AppUser)Session[SessionVars.user.ToString()]);

                    //    if (((int)((DataRowView)e.Row.DataItem)["user_id"] != user.UserId && (int)((DataRowView)e.Row.DataItem)["user_id"] > 2))
                    //    {
                    //        //Session.Add("u", (int)((DataRowView)e.Row.DataItem)["user_id"]);
                    //        //((HyperLink)e.Row.Cells[0].FindControl("HyperLink_UserId")).NavigateUrl = "AddEditUser.aspx";
                    //        string u_id = Encryption.Cryptic.EncryptString(((DataRowView)e.Row.DataItem)["user_id"].ToString());
                    //        ((HyperLink)e.Row.Cells[0].FindControl("HyperLink_UserId")).NavigateUrl = "AddEditUser.aspx?u=" + u_id;
                    //    }
                    //    //if (e.Row.FindControl("btn_EditUser") != null)
                    //    //     Button chkRow = (e.Row.Cells[2].FindControl("cbSelect") as Button);
                    //    //(e.Row.Cells[2].FindControl("btn_EditUser");
                    //}


                    if (((int)((DataRowView)e.Row.DataItem)["user_id"] != user.UserId && (int)((DataRowView)e.Row.DataItem)["user_id"] > 2))
                    {
                        if (e.Row.FindControl("imgbtn_delete") != null)
                            ((ImageButton)e.Row.FindControl("imgbtn_delete")).Attributes.Add("onClick", "return confirm('Are you sure you want to delete User " + ((DataRowView)e.Row.DataItem).Row["user_login"] + "?','CCMS')");
                    }
                    else
                    {
                        btnDeleteUser.Visible = false;
                        btnActivate.Visible = false;
                        btnDeactivate.Visible = false;
                        btnEditUser.Visible = false;

                    }
                }
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("GridView_users_RowDataBound", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

        protected void GridView_users_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DataTable dt = (DataTable)Cache[CacheVars.dtUsers.ToString()];
            DataView dv = new DataView(dt);

            if (GridViewSortExpression != "")
                dv.Sort = GridViewSortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");

            this.GridView_users.DataSource = dv;
            this.GridView_users.PageIndex = e.NewPageIndex;
            this.GridView_users.DataBind();
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
        protected void Button_ExportToPDF_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToPDF_Click");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select user_login,user_full_name,user_creation_time, 
                                    case user_is_active when  1 then 'Yes' else 'No' end  user_is_active
                                    ,user_email,user_type,user_modification_time, user_last_login_time,
                                    user_group = STUFF((
	                                    SELECT ',' + g.group_name
	                                    FROM groups g, group_users gu
	                                    WHERE g.group_id = gu.group_id and app_user.user_id = gu.user_id
	                                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')
	                                    from app_user
                                    order by user_full_name";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\UserList.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "User List";
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dtTrxn.Rows.Count);
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\UserList" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";
                rptDoc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path);
                rptDoc.Close();
                Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(path));
                Response.WriteFile(path);
                Response.Flush();
                Response.Close();

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }


        }


        protected void Button_ExportToExcel_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToExcel_Click");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select user_login,user_full_name,user_creation_time, 
                                    case user_is_active when  1 then 'Yes' else 'No' end  user_is_active
                                    ,user_email,user_type,user_modification_time, user_last_login_time,
                                    user_group = STUFF((
	                                    SELECT ',' + g.group_name
	                                    FROM groups g, group_users gu
	                                    WHERE g.group_id = gu.group_id and app_user.user_id = gu.user_id
	                                    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')
	                                    from app_user
                                    order by user_full_name";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\UserList.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "User List";
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dtTrxn.Rows.Count);
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\UserList" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
                rptDoc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelRecord, path);
                rptDoc.Close();
                Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(path));
                Response.WriteFile(path);
                Response.Flush();
                Response.Close();

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }

        }
    }
}
