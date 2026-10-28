using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Avanza.iSuite.DAL;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS
{
    public partial class Investigation : BasePage
    {
        protected void Button_AddNewComments_click(object sender, EventArgs e)
        {
            if (this.TextBox_NewComments.Text.Trim().Length > 0)
            {
                Avanza.CCMS.DAL.UserTask task = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(Request.QueryString["tid"]));
                UserTaskComments comments = new UserTaskComments();
                comments.Comments = this.TextBox_NewComments.Text;
                comments.UserTaskId = task.UserTaskId;
                comments.CreationTime = DateTime.Now;
                comments.UserId = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                comments.Save();
                this.TextBox_NewComments.Text = "";
            }
            this.ReloadComments();
        }

        private void ReloadComments()
        {
            if (Request.QueryString["tid"] != null)
            {
                Avanza.CCMS.DAL.UserTask task = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(Request.QueryString["tid"]));
                UserTaskComments.UserTaskCommentsReader reader = UserTaskComments.ExecuteReader(" user_task_id=" + task.UserTaskId);
                StringBuilder builder = new StringBuilder();
                while (reader.Read())
                {
                    builder.Append("<div style='width:100%'><table style='width:100%' border='0'><tr><td style='width:10%'><b>" + AppUser.LoadAppUserByPk(reader.CurrentUserTaskComments.UserId).UserFullName + ":</b></td><td style='width:80%'>" + reader.CurrentUserTaskComments.Comments + "</td><td align='right' width='10%'>" + reader.CurrentUserTaskComments.CreationTime.ToString("dd/MM/yyyy HH:mm") + "</td></table></div><br/>");
                }
                reader.Close();
                this.Literal_Comments.Text = builder.ToString();
            }
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    Avanza.CCMS.DAL.Investigation investigation = Avanza.CCMS.DAL.Investigation.LoadInvestigationByPk(int.Parse(Request.QueryString["id"]));
                    if (investigation != null)
                    {
                        TextBox_OrderNo.Text = investigation.OrderNumber;
                        TextBox_Summary.Text = investigation.InvestigationSummary;
                        DropDownList_Reason.SelectedIndex = investigation.InvestigationReasonId;
                        DropDownList_Status.SelectedIndex = investigation.InvestigationStatus;
                        Atm atm = Atm.LoadAtmByPk(investigation.AtmId);
                        if (atm != null)
                        {
                            Label_atm.Text = atm.Title;
                            Response.Write("<script>window.parent.SelectNode('a" + investigation.AtmId + "');</script>");
                        }


                        this.ReloadComments();
                        TabStripTab_Comments.Visible = true;
                        TextBox_OrderNo.Enabled = false;
                        if (DropDownList_Status.SelectedIndex == 1)
                            Button_AddNew.Enabled = false;

                    }
                    Button_Submit.Text = "Update Investigation";
                    if (DropDownList_Status.SelectedIndex == 1)
                        Button_Submit.Enabled = false;
                }
                else
                {
                    refresh_ServerClick(this, null);
                    TabStripTab_Comments.Visible = false;
                    Button_Submit.Text = "Create Investigation";
                    DropDownList_Status.Enabled = false;
                }

            }

        }
        public static void CreateUserAndAssignmentTasks(int createdBy, string taskDesc, int entityId, int taskTypeId, SqlTransaction trxn)
        {

            List<int> usersTasks = new List<int>();

            Avanza.CCMS.DAL.UserTask userTask = new Avanza.CCMS.DAL.UserTask();
            userTask.CreatedBy = createdBy;
            userTask.CreationTime = DateTime.Now;
            userTask.Status = ApprovalStatus.Pending.ToString();
            userTask.TaskTypeId = taskTypeId;
            userTask.TaskDesc = taskDesc;
            userTask.EntityType = "Investigation";
            userTask.EntityId = entityId;
            //userTask.AtmId = atm.ATMId;
            userTask.Save(trxn.Connection, trxn);

            //User that can view this task..
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select user_id , employee_manager_id from app_user
                                   where employee_manager_id is not null";

            DataTable dtUser = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtUser);
            int id = userTask.CreatedBy;
            while (true)
            {
                DataRow[] dataRow = dtUser.Select("user_id = " + id);
                if (dataRow.Length > 0)
                {
                    //Create task Assignment
                    id = int.Parse(dataRow[0]["employee_manager_id"].ToString());
                    UserTaskAssignment taskAssignment = new UserTaskAssignment();
                    taskAssignment.UserTaskId = userTask.UserTaskId;
                    taskAssignment.UserId = id;
                    taskAssignment.Save(trxn.Connection, trxn);
                    usersTasks.Add(id);
                }
                else
                    break;
            }
            if (!usersTasks.Contains(2))
            {
                UserTaskAssignment taskAssignment = new UserTaskAssignment();
                taskAssignment.UserTaskId = userTask.UserTaskId;
                taskAssignment.UserId = 2;
                taskAssignment.Save(trxn.Connection, trxn);
            }
        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddInvestigation");
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trxn = conn.BeginTransaction();

                Avanza.CCMS.DAL.Investigation investigation = null;

                if (Request.QueryString["id"] != null)
                {
                    if ((int)ConnectionFactory.ExecuteScalar(@"select count(investigation_id) from investigation
                                                              where order_number = '" + TextBox_OrderNo.Text +
                                                                "' and investigation_id <> " + Request.QueryString["id"]) == 0)
                    {
                        investigation = Avanza.CCMS.DAL.Investigation.LoadInvestigationByPk(int.Parse(Request.QueryString["id"]));
                        if (Request.QueryString["s"] == null)
                        {
                            investigation.ModificationTime = DateTime.Now;
                            investigation.ModifiedBy = ((Avanza.CCMS.DAL.AppUser)Session[SessionVars.user.ToString()]).UserId;
                        }
                    }
                    else
                        throw new Exception("Investigation already opened for given order number.Please enter another order number.");
                }
                else
                {
                    if (Request.QueryString["s"] == null)
                    {
                        if ((int)ConnectionFactory.ExecuteScalar(@"select count(investigation_id) from investigation
                                                              where order_number = '" + TextBox_OrderNo.Text + "'") == 0)
                        {
                            investigation = new Avanza.CCMS.DAL.Investigation();
                            investigation.CreationTime = DateTime.Now;
                            investigation.UserId = ((Avanza.CCMS.DAL.AppUser)Session[SessionVars.user.ToString()]).UserId;
                        }
                        else
                            throw new Exception("Investigation already opened for given order number.Please enter another order number.");
                    }
                }
                investigation.OrderNumber = TextBox_OrderNo.Text;
                investigation.InvestigationStatus = DropDownList_Status.SelectedIndex;
                investigation.InvestigationSummary = TextBox_Summary.Text;
                investigation.InvestigationReasonId = DropDownList_Reason.SelectedIndex;

                investigation.AtmId = int.Parse(Session[SessionVars.SelectedATMs.ToString()].ToString());
                investigation.Save(trxn.Connection, trxn);
                if (Request.QueryString["e"] == null && Request.QueryString["tid"] == null)
                {
                    //int userTaskId = int.Parse(Request.QueryString["tid"]);
                    //ConnectionFactory.ExecuteQuery("delete user_task_comments where user_task_id = "+userTaskId+"; delete user_task_assignment where user_task_id = " + userTaskId+";delete user_task where user_task_id = " + userTaskId,trxn);
                    CreateUserAndAssignmentTasks(((AppUser)Session[SessionVars.user.ToString()]).UserId,
                       "Investigation opened against Atm[" + Atm.LoadAtmByPk(investigation.AtmId).Title + "]",
                       investigation.InvestigationId, (int)EnumTaskType.UpdateInvestigation, trxn);
                }
                if (Request.QueryString["tid"] != null)
                {
                    if (DropDownList_Status.SelectedIndex == 1)
                    {
                        Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(Request.QueryString["tid"]));
                        userTask.Status = ApprovalStatus.Closed.ToString();
                        userTask.Save(trxn.Connection, trxn);
                        investigation.ResolutionTime = DateTime.Now;
                        investigation.ResolvedBy = ((Avanza.CCMS.DAL.AppUser)Session[SessionVars.user.ToString()]).UserId;
                        investigation.Save(trxn.Connection, trxn);
                    }
                }


                trxn.Commit();
                if (Request.QueryString["e"] == null && Request.QueryString["tid"] == null)
                    Utility.BuildAuditLog("Investigation " + investigation.InvestigationId + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateInvestigation);
                else
                    Utility.BuildAuditLog("Investigation " + investigation.InvestigationId + " edited", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditInvestigation);

                if (Request.QueryString["s"] == null )
                    Response.Redirect("InvestigationList.aspx");
                else
                    Response.Redirect("UserTask.aspx");
            }
            catch (Exception ex)
            {
                trxn.Rollback();
                Literal_error.Text = "<li>An error occured while saving investigation." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        //<input type="button" value="Close" class="button" onclick="window.location='InvestigationList.aspx'" />


        protected void Button_Close_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["s"] == null)
                Response.Redirect("InvestigationList.aspx");
            else
                Response.Redirect("usertask.aspx");
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                        Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                Label_atm.Text = "Please, select an ATM";
            else
            {
                if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                {
                    Literal_error.Text = "<li>ATM not selected.</li>";

                }
                else
                    Label_atm.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
            }
        }
    }
}