using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Text;

using Avanza.CCMS.DAL;
using System.Web.UI.MobileControls;
using System.Collections.Generic;
using System.IO;
using System;

namespace Avanza.iSuite.UI
{
    public partial class ApproveGroup : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (!CheckRight((int)Permissions.ApproveGroup))
                    Response.Redirect("AccessDenied.aspx");

               
                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                //AppUser currentUser = ((AppUser)Session[SessionVars.user.ToString()]);

                //String whereStatement = " status = 'Pending' ";

                //if (!currentUser.UserType.Equals("NCR"))
                //{
                //    whereStatement += String.Format(" and entity_type = '{0}' and created_by = {1}", currentUser.UserType, currentUser.UserId);
                //}

                //if (TextBox_groupname.Text != String.Empty)
                //{
                //    whereStatement += String.Format(" and group_name like '%{0}%' ", TextBox_groupname.Text);
                //}

                //Groups.GroupsReader groupsReader = Groups.ExecuteReader(whereStatement);

                StringBuilder commandText = new StringBuilder();
                //Commented by Danish
                //commandText.Append("select a.user_id, user_login, user_full_name, user_email, user_is_active, org_mcn from app_user a inner join user_organization u on u.user_id = a.user_id where a.user_id > 2");

                AppUser currentUser = (AppUser)Session[SessionVars.user.ToString()];
                DataTable dtGroups = PopulateGroupList(currentUser);

                DataTable dtGroups_Copy = new DataTable();
                //dtGroups_Copy.Columns.Add("user_id", typeof(int));
                dtGroups_Copy.Columns.Add("group_name", typeof(string));
                dtGroups_Copy.Columns.Add("Description", typeof(string));
                dtGroups_Copy.Columns.Add("Status", typeof(string));
                dtGroups_Copy.Columns.Add("Request_Mode", typeof(string));
                //dtUsers_Copy.Columns.Add("org_mcn", typeof(string));

                foreach (DataRow row in dtGroups.Rows)
                {
                    dtGroups_Copy.Rows.Add(new object[] { row["group_name"], row["Description"], row["Status"], row["Request_Mode"] });
                }

                Cache[CacheVars.dtGroups.ToString()] = dtGroups_Copy;

                this.GridView_approveGroups.DataSource = dtGroups_Copy;
                this.GridView_approveGroups.DataBind();
                //groupsReader.Close();
            }
            catch (Exception ex)
            { LogableTask.LogMonoActivityTask("BindGroupsGrid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
        }

        private DataTable PopulateGroupList(AppUser currentUser)
        {
            DataTable dtGroupList = new DataTable();
            String commandText = String.Empty;
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

            commandText = @"select *, CASE WHEN is_added = 1 THEN 'Add Request' WHEN is_deleted = 1 THEN 'Delete Request' WHEN is_editied = 1 THEN 'Edit Request' ELSE NULL END AS Request_Mode 
                                            from groups 
                                            where status = 'Pending'";
            if (currentUser.UserId != 2)
                commandText += string.Format(" AND group_id IN (SELECT group_id FROM groups_approver where created_by != {0})", currentUser.UserId);

            if (!currentUser.UserType.Equals("NCR"))
            {
                commandText = commandText + String.Format(" and entity_type = '{0}' and created_by = {1}", currentUser.UserType, currentUser.UserId);
            }

            if (TextBox_groupname.Text != String.Empty)
            {
                commandText = commandText + String.Format(" and group_name like '%{0}%' ", TextBox_groupname.Text);
            }
            

            cmd.CommandText = commandText;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtGroupList);


            return dtGroupList;
        }

        protected void GridView_approveGroups_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trx = null;
            conn = ConnectionFactory.GetNewConnection();
            //Groups groups = Groups.LoadGroupsByPk(int.Parse(e.CommandArgument.ToString()));
            //GroupsApprover approveGroup = GroupsApprover.LoadAppUseApproverrByGroupName(groups.GroupName);
            try
            {
                if (e.CommandName == "ViewGroup")
                {
                    LogableTask task = null;

                    try
                    {
                        task = LogableTask.NewTask("View Group");
                        Groups _group = Groups.LoadGroupsByGroupName(e.CommandArgument.ToString());
                        GroupsApprover group = GroupsApprover.LoadGroupsApprover(string.Format("group_id={0}", _group.GroupId));
                        Session["group"] = group.GroupName;
                        Response.Redirect("AddEditGroup.aspx",false);
                    }
                    catch (Exception ex)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                        Literal_error.Text = "<li>An Error occured while view group. " + ex.Message + "</li>";

                    }
                    finally
                    {
                        task.EndTask();
                    }
                }
            }
            catch( Exception ex)
            {
                Literal_error.Text = "<li>An Error occured while view group. " + ex.Message + "</li>";
            }
            //    if (e.CommandName == "ApproveRequest")
            //    {
            //        if (groups.IsDeleted == true)
            //        {
            //            cmd = ConnectionFactory.GetNewCommand(true);
            //            trx = cmd.Connection.BeginTransaction();
            //            cmd.Transaction = trx;

            //            cmd.CommandText = "Delete_group";
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            cmd.Parameters.Add("@group_id", SqlDbType.Int);
            //            cmd.Parameters[0].Value = int.Parse(e.CommandArgument.ToString());
            //            cmd.ExecuteNonQuery();
            //        }
            //        if (groups.IsEditied == true)
            //        {
            //            groups.GroupName = approveGroup.GroupName;
            //            groups.Description = approveGroup.Description;
            //            groups.EntityType = approveGroup.EntityType;
            //            groups.CreatedBy = approveGroup.CreatedBy;
            //            groups.OrganizationId = approveGroup.OrganizationId;
            //            groups.GroupEmail = approveGroup.GroupEmail;
            //            groups.SendIndividualAlert = approveGroup.SendIndividualAlert;
            //            groups.IsAdded = false;
            //            groups.IsDeleted = false;
            //            groups.IsEditied = false;
            //            groups.Status = "Approved";
            //            groups.Save();

            //            ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights where group_id = " + groups.GroupId);
            //            ConnectionFactory.ExecuteQuery("insert into Group_rights select * from Group_rights_approver where group_id = " + groups.GroupId);
            //            ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights_approver where group_id = " + groups.GroupId);
            //            ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_name = '" + groups.GroupName + "'");

            //        }
            //        if (groups.IsAdded == true)
            //        {
            //            groups.IsAdded = false;
            //            groups.IsDeleted = false;
            //            groups.IsEditied = false;
            //            groups.Status = "Approved";
            //            groups.Save();

            //            ConnectionFactory.ExecuteQuery("insert into Group_rights select * from Group_rights_approver where group_id = " + groups.GroupId);
            //            ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights_approver where group_id = " + groups.GroupId);
            //        }
            //        Literal_ok.Text = "<li>User Approval Accepted!";
                                       
            //    }
            //    else if (e.CommandName == "DeclineRequest")
            //    {
            //        if (groups.IsDeleted == true)
            //        {
            //            groups.Status = "Approved";
            //            groups.IsDeleted = false;
            //            groups.Save();
            //        }
            //        if (groups.IsAdded == true)
            //        {
            //            ConnectionFactory.ExecuteQuery("delete from Groups where group_id  = '" + groups.GroupId + "'");
            //            ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_name = '" + groups.GroupName + "'");
            //            ConnectionFactory.ExecuteQuery("delete from Group_rights_approver where group_id  = '" + groups.GroupId + "'");
            //        }
            //        if (groups.IsEditied == true)
            //        {
            //            groups.Status = "Approved";
            //            groups.IsEditied = false;
            //            groups.Save();

            //            ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_name = '" + groups.GroupName + "'");
            //            ConnectionFactory.ExecuteQuery("delete from Group_rights_approver where group_id  = '" + groups.GroupId + "'");
            //        }
            //        Literal_ok.Text = "<li>User Approval Decline!";
            //    }

            //    if (trx != null)
            //        trx.Commit();

            //    BindGrid();
            //}
            //catch (Exception ex)
            //{
            //    if (trx != null)
            //        trx.Rollback();
            //    Literal_error.Text = "<li>An error occured while deleting group! " + ex.Message + "</li>";
            //    LogableTask.LogMonoActivityTask("Delete group", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            //}
            //finally { if (cmd != null) cmd.Connection.Close(); }
        }

       
       
        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_groupname.Text = "";

        }
       
    }
}