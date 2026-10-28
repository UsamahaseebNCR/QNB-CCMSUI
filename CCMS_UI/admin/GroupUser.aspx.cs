using System;
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

namespace Avanza.iSuite.UI
{
    public partial class GroupUser : BasePage
    {
        string enablemakeachecker = System.Web.Configuration.WebConfigurationManager.AppSettings["enablemakeachecker"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !DialogCallBack.CausedCallback)
            {
                if (!CheckRight((int)Permissions.ViewGroupList))
                    Response.Redirect("AccessDenied.aspx");

                //if (!CheckRight((int)Permissions.ModifyGroup))
                //    GridView_groups.Columns[2].Visible = false;

                if (!CheckRight((int)Permissions.DeleteGroup))
                    GridView_groups.Columns[3].Visible = false;

                if (!CheckRight((int)Permissions.CreateGroup))
                    this.HyperLink_newGroup.Visible = false;
                
                BindGrid();
                if(enablemakeachecker=="false")
                {
                    GridView_groups.Columns[2].Visible = false;
                }
            }
        }

        private void BindGrid()
        {
            try
            {
                AppUser currentUser = ((AppUser)Session[SessionVars.user.ToString()]);

                String whereStatement = " 1=1";

                if (!currentUser.UserType.Equals("NCR"))
                {
                    whereStatement += String.Format(" and entity_type = '{0}' and created_by = {1}", currentUser.UserType, currentUser.UserId);
                }

                if (TextBox_groupname.Text != String.Empty)
                {
                    whereStatement += String.Format(" and group_name like '%{0}%' ", TextBox_groupname.Text);
                }

                 Groups.GroupsReader groupsReader = Groups.ExecuteReader(whereStatement);

                this.GridView_groups.DataSource = groupsReader;
                this.GridView_groups.DataBind();
                groupsReader.Close();
            }
            catch (Exception ex)
            { LogableTask.LogMonoActivityTask("BindGroupsGrid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
        }
        
        protected void GridView_groups_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("DeleteGroup");
            SqlTransaction trx = null;
            SqlCommand cmd = null;
            try
            {
                if (e.CommandName == "deleteGroup")
                {
                    Groups groups = Groups.LoadGroupsByPk(int.Parse(e.CommandArgument.ToString()));
                    ConnectionFactory.ExecuteQuery("update groups set is_deleted = 1, Status = 'Pending' where group_id  = " + e.CommandArgument);

                    if(enablemakeachecker=="false")
                    {
                        cmd = ConnectionFactory.GetNewCommand(true);
                        trx = cmd.Connection.BeginTransaction();
                        cmd.Transaction = trx;

                        cmd.CommandText = "Delete_group";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@group_id", SqlDbType.Int);
                        cmd.Parameters[0].Value = groups.GroupId;
                        cmd.ExecuteNonQuery();
                        trx.Commit();
                        Literal_ok.Text = "<li>Group has been deleted successfully";
                        Utility.BuildAuditLog("Group [" + groups.GroupName + "] has been deleted successfully", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteGroup);
                    }
                    else
                    {
                        GroupsApprover groupApprover = new GroupsApprover();
                        groupApprover.GroupName = groups.GroupName;
                        groupApprover.GroupId = groups.GroupId;
                        groupApprover.Description = groups.Description;
                        groupApprover.EntityType = groups.EntityType;
                        groupApprover.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        groupApprover.OrganizationId = groups.OrganizationId;
                        groupApprover.GroupEmail = groups.GroupEmail;
                        groupApprover.SendIndividualAlert = groups.SendIndividualAlert;
                        groupApprover.IsAdded = false;
                        groupApprover.IsDeleted = true;
                        groupApprover.IsEditied = false;
                        groupApprover.Status = "Pending";
                        groupApprover.Save();

                        cmd = ConnectionFactory.GetNewCommand(true);
                        cmd.CommandText = "delete from group_rights_approver where group_id = " + groups.GroupId;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT INTO group_rights_approver SELECT * FROM group_rights WHERE group_id = " + groups.GroupId;
                        cmd.ExecuteNonQuery();

                        cmd.Connection.Close();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "rights for the group added");

                        Literal_ok.Text = "<li>Selected group deletion request has been sent for approval!";
                        Utility.BuildAuditLog("Request to delete group [" + groups.GroupName + "] has been sent for approval", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteGroup);
                    }
                    
                    
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
            SqlCommand cmd = null;
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "select count( app_user.user_id) from app_user inner join group_users " +
                       "on group_users.user_id = app_user.user_id where group_id = " + ((Groups)(e.Row.DataItem)).GroupId + " and user_is_active = 1 and app_user.user_id > 1";

                    if ((int)cmd.ExecuteScalar() == 0)
                        e.Row.FindControl("ShowHideUsersIcon").Visible = false;


                    cmd.CommandText = "select count(name) from rights inner join group_rights " +
               "on rights.right_id = group_rights.right_id where group_id = " + ((Groups)(e.Row.DataItem)).GroupId;
                    //cmd.Connection.Open();
                    if ((int)cmd.ExecuteScalar() == 0)
                        e.Row.FindControl("ShowHideRightsIcon").Visible = false;

                    List<int> list = new List<int>(3);
                    cmd.CommandText = "select group_id from group_users where user_id=" + (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        list.Add(reader.GetInt32(0));
                    reader.Close();


                    if (((Groups)(e.Row.DataItem)).GroupId == 1 || list.Contains(((Groups)(e.Row.DataItem)).GroupId) || (enablemakeachecker == "true" && ((Groups)(e.Row.DataItem)).Status == "Pending"))
                        ((ImageButton)e.Row.FindControl("Button_deleteUserGroup")).Visible = false;
                    else
                        ((ImageButton)e.Row.FindControl("Button_deleteUserGroup")).Attributes.Add("onclick", "javascript:window.event.returnValue=confirm('Are you sure you want to delete " + e.Row.Cells[0].Text + " Group ?');");

                    if (CheckRight((int)Permissions.ModifyGroup))
                    {
                        //  cmd.CommandText = "select group_id from group_users where user_id=" + (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        // int groupId = (int)cmd.ExecuteScalar();

                        if ((!list.Contains(((Groups)e.Row.DataItem).GroupId) && ((Groups)(e.Row.DataItem)).GroupId != 1) || list.Contains(1))
                        {
                            if (enablemakeachecker == "false" || ((Groups)(e.Row.DataItem)).Status != "Pending")
                            {
                                ((HyperLink)e.Row.Cells[0].FindControl("HyperLink_GroupName")).NavigateUrl = "AddEditGroup.aspx?group_id=" + ((Groups)(e.Row.DataItem)).GroupId;
                            }
                        }
                    }
                    
                    cmd.Connection.Close();
                }

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("GridView_groups_RowDataBound", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();


            }
        }

        protected void DialogCallBack_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            SqlCommand cmd = null;
           try
            {
                string[] chars = new string[] { "select", "and", "drop", "create", "update", "delete", "from", "or", ".", "%" };
                foreach (string x in chars)
                {
                    if (e.Parameters[0].Contains(x))
                    {
                        throw new Exception("Invalid Value");
                    }
                    if (e.Parameters[1].Contains(x))
                    {
                        throw new Exception("Invalid Value");
                    }
                    if (e.Parameters[2].Contains(x))
                    {
                        throw new Exception("Invalid Value");
                    }
                }

                cmd = ConnectionFactory.GetNewCommand(true);

                if (e.Parameters[1] == "1")
                    cmd.CommandText = "select app_user.user_id,user_full_name,user_is_active from app_user inner join group_users " +
                               "on group_users.user_id = app_user.user_id where group_id = " + e.Parameters[0] + " and user_is_active = 1 and app_user.user_id > 1";
                else
                    cmd.CommandText = "select name from rights inner join group_rights " +
                       "on rights.right_id = group_rights.right_id where group_id = " + e.Parameters[0];

                SqlDataReader reader = cmd.ExecuteReader();


                builder.Append("<table>");

                byte counter = 0;

                while (reader.Read())
                {
                    if (counter == 0)
                        builder.Append("<tr>");

                    if (e.Parameters[2] == "1")
                    {
                        string value = Encryption.Cryptic.EncryptString(reader[0].ToString() +","+reader[1].ToString());
                        builder.Append("<td><a href = AddEditUser.aspx?u=" + value + ">" + HttpUtility.JavaScriptStringEncode(reader[1].ToString()) + "</a></td>");
                    }
                    else

                        builder.Append("<td><li>" + reader[0].ToString() + "&nbsp;&nbsp;&nbsp;</li></td>");
                    // style='width: 50px'
                    counter++;
                    if (counter == 3) counter = 0;
                }

                builder.Append("</table>");
                e.Output.Write("<script> receiveCallBackResult('" + builder.ToString() + "');</script>");
            }
            //catch (Exception ex)
            //{
            //    LogableTask.LogMonoActivityTask("Err", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            //    Literal_error.Text = ex.Message;
            //    if (ex.Message.ToLower().Contains("invalid"))
            //        throw;
            //}
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();


            }

        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
        
        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_groupname.Text = "";

        }
        
        protected void Button_ExportToPDF_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToPDF_Click");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"

select group_name,description,user_full_name,name
 from groups inner join group_rights
 on groups.group_id = group_rights.group_id
 inner join rights on rights.right_id = group_rights.right_id
 inner join app_user on user_id = groups.created_by
  order by group_name";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\GroupList.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "Group List";
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dtTrxn.Rows.Count);
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\GroupList_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";
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
                cmd.CommandText = @"

select group_name,description,user_full_name,name
 from groups inner join group_rights
 on groups.group_id = group_rights.group_id
 inner join rights on rights.right_id = group_rights.right_id
 inner join app_user on user_id = groups.created_by
  order by group_name";


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                CrystalDecisions.CrystalReports.Engine.ReportDocument rptDoc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\GroupList.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "Group List";
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dtTrxn.Rows.Count);
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\GroupList_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
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