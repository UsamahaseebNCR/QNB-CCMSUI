using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.CCMS.DAL;
using System.Text.RegularExpressions;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class UserTask : BasePage
    {
        private void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            try
            {
                cmd.CommandTimeout = 100;
                //                cmd.CommandText = @"select title,user_full_name,status,user_task.creation_time,task_type_name,atm_settlement_id,user_task.user_task_id
                //                                    from user_task inner join task_type
                //                                    on user_task.task_type_id =task_type.task_type_id
                //                                    inner join atm on atm.atm_id = user_task.atm_id
                //                                    inner join app_user on user_task.created_by = app_user.user_id " +
                //                                    " inner join user_task_assignment on user_task_assignment.user_task_id = user_task.user_task_id " +
                //                                    this.buildSearchCriteria() + " Order by " + orderBy;

                cmd.CommandText = @"select title,user_full_name,status,user_task.creation_time,task_type_name,atm_settlement_id,user_task.user_task_id,user_task_assignment.user_id,app_user.user_id,task_desc,entity_id,entity_type
                                    from user_task inner join task_type on user_task.task_type_id =task_type.task_type_id
                                    left join atm on atm.atm_id = user_task.atm_id
                                    inner join app_user on user_task.created_by = app_user.user_id 
                                    inner join user_task_assignment on user_task_assignment.user_task_id = user_task.user_task_id " +
                                    this.buildSearchCriteria() + " Order by " + orderBy;


                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                this.GridView_ATMSettlement.DataSource = dt;
                this.GridView_ATMSettlement.DataBind();
                this.Literal_count.Text = dt.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_error.Text = "<li>An Error occured. " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindGrid(this.ViewState["SortExpression"].ToString() + this.ViewState["sortDirection"].ToString());
        }

        private string buildSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();
            string selectQuery = string.Empty;
            builder.Append(" where");
            if (this.TextBox_FromDate.Text.Length > 0)
            {
                builder.Append(" user_task.creation_time >= Convert(datetime,'" + this.TextBox_FromDate.Text + "',103) AND");
            }
            if (this.TextBox_ToDate.Text.Length > 0)
            {
                builder.Append("  user_task.creation_time <=Convert(datetime,'" + this.TextBox_ToDate.Text + " 23:59:59',103) AND");
            }
            if (DropDownList_TaskType.SelectedIndex > 0)
                builder.Append("  user_task.task_type_id = " + DropDownList_TaskType.SelectedItem.Value + " AND");

            //commented on 26/06/2013 --uncommented for NBE..
            if (DropDownList_TaskType.SelectedIndex == 1 || DropDownList_TaskType.SelectedIndex == 2)
                builder.Append(" user_task.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") AND");

            //Enable below for BNP so that to view vault settlements task.
            //Need to think over it.

            //if (Request.QueryString["a"]!=null)
            //    builder.Append(" user_task.atm_id ="+Request.QueryString["a"]+"  AND");

                //or  entity_id in ( " +
                //"                select vault_settlement_id from vault_settlement inner join vault_Atm on vault_settlement.vault_id = vault_Atm.vault_id " +
                //"                   where atm_id =" + Request.QueryString["a"] + ")) or  entity_id in ( " +
                //"                select id from vault_settlement inner join vault_Atm on vault_settlement.vault_id = vault_Atm.vault_id " +
                //"                   where atm_id =" + Request.QueryString["a"] + "))
            //else
            //    builder.Append(" user_task.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")  AND");
            //or entity_id in ( " +
                //"                select vault_settlement_id from vault_settlement inner join vault_Atm on vault_settlement.vault_id = vault_Atm.vault_id " +
                //"                   where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")))

            //if (DropDownList_Status.SelectedIndex==2)
            //    builder.Append(" status='Rejected' and (( user_task.created_by="+((AppUser)this.Session[SessionVars.user.ToString()]).UserId+") or (user_task_assignment.user_id="+ ((AppUser)this.Session[SessionVars.user.ToString()]).UserId+" and user_task.task_type_id=12)) AND");
            //else
            if (DropDownList_Status.SelectedIndex>0)
                builder.Append(" status='" + this.DropDownList_Status.SelectedItem.Value + "' AND");

            builder.Append(" user_task_assignment.user_id=" + ((AppUser)this.Session[SessionVars.user.ToString()]).UserId);

            selectQuery = builder.ToString().Trim();
            if (selectQuery.EndsWith("AND"))
            {
                selectQuery = selectQuery.Substring(0, selectQuery.LastIndexOf("AND"));
            }
            return selectQuery;
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            this.TextBox_FromDate.Text = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            this.TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        protected void GridView_ATMSettlement_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_ATMSettlement.PageIndex = e.NewPageIndex;
            this.BindGrid(this.ViewState["SortExpression"].ToString() + this.ViewState["sortDirection"].ToString());
        }

        protected void GridView_ATMSettlement_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            try
            {
                task = LogableTask.NewTask("assign links");
                Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(e.CommandArgument.ToString()));
                if (userTask != null)
                {
                    if (userTask.TaskTypeId == 11)
                    {
                        if (userTask.Status == ((ApprovalStatus)0).ToString())
                        {
                            if (userTask.CreatedBy == ((AppUser)this.Session[SessionVars.user.ToString()]).UserId
                                 && ((AppUser)this.Session[SessionVars.user.ToString()]).UserId != 2)
                            {
                                base.Response.Redirect("~/ccms/ShowSettlementData.aspx?sid=" + userTask.AtmSettlementId + "&e=0");
                            }
                            else
                            {
                                base.Response.Redirect("~/ccms/ShowSettlementData.aspx?sid=" + userTask.AtmSettlementId);
                            }
                        }
                        else if (userTask.Status == ((ApprovalStatus)2).ToString())
                        {
                            if (userTask.CreatedBy != ((AppUser)this.Session[SessionVars.user.ToString()]).UserId
                                && ((AppUser)this.Session[SessionVars.user.ToString()]).UserId != 2)
                            {
                                base.Response.Redirect("~/ccms/ShowSettlementData.aspx?sid=" + userTask.AtmSettlementId + "&e=0");
                            }
                            else
                            {
                                base.Response.Redirect("~/ccms/EditSettlementData.aspx?sid=" + userTask.AtmSettlementId);
                            }
                        }
                        else if (userTask.Status == ((ApprovalStatus)1).ToString())
                        {
                            base.Response.Redirect("~/ccms/ShowSettlementData.aspx?sid=" + userTask.AtmSettlementId + "&e=0");
                        }
                    }
                    else if (userTask.TaskTypeId == 12)
                    {
                        if (userTask.Status == ((ApprovalStatus)0).ToString())
                        {
                            base.Response.Redirect(string.Concat(new object[] { "~/ccms/FinalizeSuspectedReplenishments.aspx?aid=", userTask.AtmAlertId, "&tid=", userTask.UserTaskId, "&s=p" }));
                        }
                        else if (userTask.Status == ((ApprovalStatus)1).ToString())
                        {
                            base.Response.Redirect(string.Concat(new object[] { "~/ccms/FinalizeSuspectedReplenishments.aspx?aid=", userTask.AtmAlertId, "&tid=", userTask.UserTaskId, "&s=a" }));
                        }
                        else if (userTask.Status == ((ApprovalStatus)2).ToString())
                        {
                            base.Response.Redirect(string.Concat(new object[] { "~/ccms/FinalizeSuspectedReplenishments.aspx?aid=", userTask.AtmAlertId, "&tid=", userTask.UserTaskId, "&s=r" }));
                        }
                    }

                    else if (userTask.TaskTypeId == 14)
                    {
                        if (userTask.Status == ApprovalStatus.Pending.ToString())
                        {
                            Response.Redirect("~/admin/AddEditCheques.aspx?id=" + userTask.EntityId + "&s=0&tid= " + userTask.UserTaskId);
                        }
                        else if (userTask.Status == ApprovalStatus.Approved.ToString())
                        {
                            Response.Redirect("~/admin/AddEditCheques.aspx?id=" + userTask.EntityId + "&s=1&tid=" + userTask.UserTaskId);
                        }
                        else if (userTask.Status == ApprovalStatus.Rejected.ToString())
                        {
                            Response.Redirect("~/admin/AddEditCheques.aspx?id=" + userTask.EntityId + "&s=2&tid=" + userTask.UserTaskId);
                        }
                    }
                    else if (userTask.TaskTypeId == 16)
                    {
                        if (userTask.Status == ApprovalStatus.Pending.ToString())
                        {
                            Response.Redirect("~/admin/AddEditVaultAdj.aspx?id=" + userTask.EntityId + "&s=0&tid= " + userTask.UserTaskId);
                        }
                        else if (userTask.Status == ApprovalStatus.Approved.ToString())
                        {
                            Response.Redirect("~/admin/AddEditVaultAdj.aspx?id=" + userTask.EntityId + "&s=1&tid=" + userTask.UserTaskId);
                        }
                        else if (userTask.Status == ApprovalStatus.Rejected.ToString())
                        {
                            Response.Redirect("~/admin/AddEditVaultAdj.aspx?id=" + userTask.EntityId + "&s=2&tid=" + userTask.UserTaskId);
                        }
                    }
                    else if (userTask.TaskTypeId == 17)
                    {
                        if (userTask.Status == ApprovalStatus.Pending.ToString())
                        {
                            Response.Redirect("~/admin/VaultSettlement.aspx?id=" + userTask.EntityId + "&s=0&tid= " + userTask.UserTaskId);
                        }
                        else if (userTask.Status == ApprovalStatus.Approved.ToString())
                        {
                            Response.Redirect("~/admin/VaultSettlement.aspx?id=" + userTask.EntityId + "&s=1&tid=" + userTask.UserTaskId);
                        }
                        else if (userTask.Status == ApprovalStatus.Rejected.ToString())
                        {
                            Response.Redirect("~/admin/VaultSettlement.aspx?id=" + userTask.EntityId + "&s=2&tid=" + userTask.UserTaskId);
                        }
                    }
                    else if (userTask.TaskTypeId == 18)
                    {
                        if (userTask.Status == ApprovalStatus.Pending.ToString())
                        {
                            Response.Redirect("~/ccms/Investigation.aspx?id=" + userTask.EntityId + "&s=0&tid= " + userTask.UserTaskId);
                        }
                        else if (userTask.Status == ApprovalStatus.Closed.ToString())
                        {
                            Response.Redirect("~/ccms/Investigation.aspx?id=" + userTask.EntityId + "&s=1&tid=" + userTask.UserTaskId);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void GridView_ATMSettlement_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (this.ViewState["sortDirection"].ToString() == " asc")
                {
                    this.ViewState["sortDirection"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirection"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpression"] = e.SortExpression;
                this.ViewState["sortDirection"] = " asc";
            }
            this.BindGrid(this.ViewState["SortExpression"].ToString() + this.ViewState["sortDirection"].ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {

                if (!base.CheckRight((int)Permissions.UserTask))
                {
                    Response.Redirect("../admin/AccessDenied.aspx");
                }
                this.TextBox_FromDate.Text = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                this.TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.ViewState["sortDirection"] = " desc";
                this.ViewState["SortExpression"] = "creation_time";


                TaskType.TaskTypeReader reader = TaskType.ExecuteReader("task_type_id in (11,12,14,16)");
                while (reader.Read())
                {
                    DropDownList_TaskType.Items.Add(new ListItem(reader.CurrentTaskType.TaskTypeName, reader.CurrentTaskType.TaskTypeId.ToString()));
                }
                reader.Close();

                this.BindGrid(this.ViewState["SortExpression"].ToString() + this.ViewState["sortDirection"].ToString());
                this.GridView_ATMSettlement.Columns[4].Visible = base.CheckRight((int)Permissions.ApproveCCMSAndCiTSettlement);
                Session["settlementID"] = null;

            }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new EventHandler(this.refresh_ServerClick);
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            this.btnSearch_Click(this, null);
        }

    }
}