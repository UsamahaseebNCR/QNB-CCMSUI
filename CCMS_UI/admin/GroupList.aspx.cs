using System;
using System.Data; 
using System.Web; 
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using Avanza.iSuite.DAL;
using Avanza.CCMS.DAL;


namespace Avanza.iSuite.UI
{
    public partial class GroupList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewGroupList))
                    Response.Redirect("~/aspx/AccessDenied.aspx");
                                
                if (!CheckRight((int) Permissions.ModifyGroup))
                    GridView_groups.Columns[0].Visible = false;
                 
                if (!CheckRight((int) Permissions.DeleteGroup))
                    GridView_groups.Columns[2].Visible = false;

                if (!CheckRight((int)Permissions.CreateGroup))
                    this.HyperLink_newGroup.Visible = false;

                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                Groups.GroupsReader groupsReader = Groups.ExecuteReader("user_id >1"// exclude system user
                +(TextBox_userId.Text.Length > 1 ? " and user_login like '" + TextBox_userId.Text + "%'" : ""));
                this.GridView_groups.DataSource = groupsReader;
                this.GridView_groups.DataBind();
                groupsReader.Close();
            }
            catch (Exception ex)
            { LogableTask.LogMonoActivityTask("BindGroupsGrid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
        }
        protected void GridView_groups_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            SqlTransaction trx = null;
            SqlCommand cmd = null;
            try
            {
                if (e.CommandName == "deleteGroup")
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trx = cmd.Connection.BeginTransaction();
                    cmd.Transaction = trx;

                    cmd.CommandText = "Delete_group";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@group_id", SqlDbType.Int);
                    cmd.Parameters[0].Value = int.Parse(e.CommandArgument.ToString());
                    cmd.ExecuteNonQuery();
                    trx.Commit();
                    Literal_ok.Text = "<li>Selected group deleted!";
                    Utility.BuildAuditLog("Group deleted successfully",((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteGroup);
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();
                Literal_error.Text = "<li>An error occured while deleting group! " + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("Delete group", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally { if (cmd != null) cmd.Connection.Close(); }
        }

        protected void GridView_groups_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    Repeater repeater = (Repeater)e.Row.FindControl("userList");
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandText = "select app_user.user_id,user_full_name,user_is_active from app_user inner join group_users " +
                       "on group_users.user_id = app_user.user_id where group_id = " + ((Groups)(e.Row.DataItem)).GroupId;
                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    repeater.DataSource = dt;
                    repeater.DataBind();
                    repeater = (Repeater)e.Row.FindControl("rightList");
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "select name from rights inner join group_rights " +
                       "on rights.right_id = group_rights.right_id where group_id = " + ((Groups)(e.Row.DataItem)).GroupId.ToString();
                    dt = new DataTable();
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    repeater.DataSource = dt;
                    repeater.DataBind();
                    ((ImageButton)e.Row.FindControl("Button_deleteUserGroup")).Attributes.Add("onclick", "javascript:window.event.returnValue=confirm('Are you sure you want to delete " + e.Row.Cells[0].Text + " Group ?');");
                }
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("GridView_groups_RowDataBound", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

       

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_userId.Text = "";
          
        }
     
    }
}