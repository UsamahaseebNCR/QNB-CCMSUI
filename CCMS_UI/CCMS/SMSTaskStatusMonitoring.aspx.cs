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
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Web.Caching;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using CCMSUI.CCMS;


namespace Avanza.iSuite.UI
{
    public partial class SMSTaskStatusMonitoring : BasePage
    {
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
           this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {

                if (!CheckRight((int)Permissions.ViewSMSTaskStatusMonitoring))
                    Response.Redirect("AccessDenied.aspx");
                // trFileTypes.Visible = true;
                //GridView_Files.Columns[3].Visible = true;

                ViewState["SortExpression"] = "creation_time";
                ViewState["sortDirection"] = " desc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                if (Session["msg"] != null)
                {
                    Literal_ok.Text = Session["msg"].ToString();
                    Session["msg"] = null;
                }

                if (Session["creationFromDate"] != null)
                    TextBox_FromDate.Text = Session["creationFromDate"].ToString();
                else
                    TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00";

                if (Session["creationToDate"] != null)
                    TextBox_ToDate.Text = Session["creationToDate"].ToString();
                else
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59";

                if (Session["endFromDate"] != null)
                    TextBox_EndTimeFromDate.Text = Session["endFromDate"].ToString();
                if (Session["endToDate"] != null)
                    TextBox_EndTimeToDate.Text = Session["endToDate"].ToString();

             

                //if (Request.Params["file_id"] != null)
                //    BinxdGrid(int.Parse(Request.Params["file_id"]));
                //else
                //BindGrid(null);
                BindGrid();
            }
        }
      


        protected void GridView_Files_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Files.PageIndex = e.NewPageIndex;
            //BindGrid(null);
            //BindGridWithCache();
            BindGrid();
        }

        protected void GridView_Files_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
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
            //if (ViewState["SortExpression"].ToString() == "creation_time")
            //    ViewState["SortExpression"] = "f.creation_time";

            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            ///BindGrid(null);
           // BindGridWithCache();
            BindGrid();
        }
        
       
        //private void BindGridWithCache()
        //{
        //    DataTable dtTask = (DataTable)Cache["dtTasks"];
        //    dtTask.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
        //    this.GridView_Files.DataSource = dtTask;
        //    this.GridView_Files.DataBind();
        //    Literal_count.Text = dtTask.Rows.Count + " records.";

        //}
        void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = "select user_full_name,sms_service_ref_no,title,transaction_friendly_name,refrence_no,last_invoked_at,status,retry_remaining,sms_task.creation_time,failure_reason,end_time,pan,account_no,tsn from sms_task inner join atm on sms_task.atm_id = atm.atm_id inner join app_user on app_user.user_id = sms_task.user_id  where " + GetFilter() + " order by " + ViewState["orderby"] + "";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTask = new DataTable();
                adapter.Fill(dtTask);

              //  dtTask = filterTasks;
             
                Literal_count.Text = dtTask.Rows.Count + " records.";
                this.GridView_Files.DataSource = dtTask;
                this.GridView_Files.DataBind();
                //Commented on 18/12/2014
                //if (DropDownList_fileType.SelectedItem.Value == "19")
                //{
                //    Button_MergeAllIntoTxt.Enabled = true;
                //    Button_MergeAllIntoPDF.Enabled = true;6
                //}
                //else
                //{
                //    Button_MergeAllIntoTxt.Enabled = false;
                //    Button_MergeAllIntoPDF.Enabled = false;
                //}

            }



            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }



       
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00";
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59";
           
            TextBox_EndTimeFromDate.Text = "";
            TextBox_EndTimeToDate.Text = "";
            
            Session["fileType"] = null;
            Session["taskType"] = null;
            Session["createdBy"] = null;
            Session["msg"] = null;
            Session["creationFromDate"] = null;
            Session["creationToDate"] = null;
            Session["endFromDate"] = null;
            Session["endToDate"] = null;
            Session["statusFriendlyName"] = null;
            Session["atmType"] = null;
        }
        string GetFilter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += "  sms_task.creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
    
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and sms_task.creation_time <= convert(datetime,'" + TextBox_ToDate.Text + "',103)";


            if (TextBox_EndTimeFromDate.Text.Length > 0)
                filter += " and end_time >= convert(datetime,'" + TextBox_EndTimeFromDate.Text + " ',103)";

            if (TextBox_EndTimeToDate.Text.Length > 0)
                filter += " and end_time <= convert(datetime,'" + TextBox_EndTimeToDate.Text + " ',103)";


            if (DropDownList_Status.SelectedIndex > 0)
                filter += " and status = '" + DropDownList_Status.SelectedItem.Text + "'";

            if (DropDownList_AlertType.SelectedIndex > 0)
            {
                if (DropDownList_AlertType.SelectedIndex == 1)
                    filter += " and captured_transaction_id is not null";
                else
                    filter += " and atm_alert_id is not null";
            }
            filter += " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";
          
            return filter;
        }
        protected void button_search_Click(object sender, EventArgs e)
        {
            BindGrid();
           // BindGrid(null);
        }
      




        protected void LinkButton_ScheduleRestart_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("ScheduleRestart");
            string selectedATMs = (string)Session[SessionVars.SelectedATMs.ToString()];
            string[] selectedATMsParts = selectedATMs.Split(',');
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                bool restartScheduled = false;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                for (int i = 0; i < selectedATMsParts.Length; i++)
                {
                    if (selectedATMsParts[i] == "-1")
                        continue;
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task with (nolock) where task_type_id=" + (int)EnumTaskType.Restart + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
                    {
                        Task restartTask = new Task();
                        restartTask.ATMId = int.Parse(selectedATMsParts[i]);
                        restartTask.BytesTransferred = 0;
                        restartTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        restartTask.CreationTime = DateTime.Now;
                        restartTask.Parsed = false;
                        restartTask.RetryRemaining = (byte)(Atm.LoadAtmByPk(restartTask.ATMId).RetryCountRestartSchedule);
                        restartTask.Status = UploadStates.scheduled.ToString();
                        restartTask.TaskTypeId = (int)EnumTaskType.Restart;
                        restartTask.UploadTime = new DateTime(1900, 1, 1);
                        restartTask.Save(conn, trx);
                        restartScheduled = true;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Restart task schedule for ATM id:" + selectedATMsParts[i]);
                    }
                    else
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Restart task already scheduled for ATM id:" + selectedATMsParts[i]);
                    }
                }
                trx.Commit();
                if (restartScheduled)
                    Session[SessionVars.notice.ToString()] = "Restart scheduled successfully";
                else
                    Session[SessionVars.notice.ToString()] = "Restart already scheduled for selected ATMs";
                //BindGrid(null);
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

                if (trx != null)
                    trx.Rollback();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                try
                {
                    task.EndTask();
                }
                catch (Exception ex)
                {
                }
            }

        }

        private void CreateTask(EnumTaskType taskTypeId, int retryRemaining)
        {
            LogableTask task = LogableTask.NewTask("CreateTask");
            string selectedATMs = (string)Session[SessionVars.SelectedATMs.ToString()];
            string[] selectedATMsParts = selectedATMs.Split(',');
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                bool scheduled = false;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                for (int i = 0; i < selectedATMsParts.Length; i++)
                {
                    if (selectedATMsParts[i] == "-1")
                        continue;
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task with (nolock) where task_type_id=" + (int)taskTypeId + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
                    {
                        Task restartTask = new Task();
                        restartTask.ATMId = int.Parse(selectedATMsParts[i]);
                        restartTask.BytesTransferred = 0;
                        restartTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        restartTask.CreationTime = DateTime.Now;
                        restartTask.Parsed = false;
                        restartTask.RetryRemaining = (byte)(retryRemaining);
                        restartTask.Status = UploadStates.scheduled.ToString();
                        restartTask.TaskTypeId = (int)taskTypeId;
                        restartTask.UploadTime = new DateTime(1900, 1, 1);
                        restartTask.Save(conn, trx);
                        scheduled = true;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Restart task schedule for ATM id:" + selectedATMsParts[i]);
                    }
                    else
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Restart task already scheduled for ATM id:" + selectedATMsParts[i]);
                    }
                }
                trx.Commit();
                if (scheduled)
                {
                    Session[SessionVars.notice.ToString()] = taskTypeId + " scheduled successfully";
                   // BindGrid(null);
                }
                else
                    Session[SessionVars.notice.ToString()] = taskTypeId + " already scheduled for selected ATMs";

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

                if (trx != null)
                    trx.Rollback();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                try
                {
                    task.EndTask();
                }
                catch (Exception ex)
                {
                }
            }

        }

        protected void LinkButton_initEj_Click(object sender, EventArgs e)
        {
            CreateTask(EnumTaskType.ExecuteInitEj, 10);
        }
        protected void LinkButton_ServiceStatus_Click(object sender, EventArgs e)
        {
            CreateTask(EnumTaskType.GetRunningServices, 10);
        }
        protected void LinkButton_StopService_Click(object sender, EventArgs e)
        {
            CreateTask(EnumTaskType.StopService, 10);

        }
        protected void LinkButton_StartService_Click(object sender, EventArgs e)
        {
            CreateTask(EnumTaskType.StartService, 10);

        }
        protected void LinkButton_GetRunningApp_Click(object sender, EventArgs e)
        {
            CreateTask(EnumTaskType.GetApplicationName, 10);

        }

        protected void LinkButton_CaptureScreen_Click(object sender, EventArgs e)
        {
            CreateTask(EnumTaskType.CaptureScreen, 10);

        }

        protected void GridView_Files_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void GridView_Files_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}