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

namespace CCMSUI.CCMS
{
    public partial class ScheduledDownload : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ScheduleFileDownload))
                        Response.Redirect("../admin/AccessDenied.aspx");
                    
                    Dropdownlist_FileType.Items.Add(new ListItem("All", "All"));
                    FileType.FileTypeReader fileTypes = FileType.ExecuteReader(" 1=1 order by file_type_title");
                    while (fileTypes.Read())
                    {
                        Dropdownlist_FileType.Items.Add(new ListItem(fileTypes.CurrentFileType.FileTypeTitle, fileTypes.CurrentFileType.FileTypeId.ToString()));
                        if (fileTypes.CurrentFileType.FileTypeId==1)
                            Dropdownlist_FileType.Items[Dropdownlist_FileType.Items.Count-1].Selected = true;
                    }
                    fileTypes.Close();
                                        
                    ViewState["SortExpression"] = "title";
                    ViewState["sortDirection"] = " asc";
                    BindGrid("temp.title,temp.file_type_id");
                }
                //BindGrid("temp.title,temp.file_type_id");
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
            finally
            { task.EndTask(); }
        }

        private void BindGrid(string sortOrder)
        {
            try
            {
                string searchCriteria = " temp.is_active = 1 and  temp.Atm_ID in (" + Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]) + ") ";

                if (Dropdownlist_FileType.SelectedValue != "All")
                    searchCriteria += " and  temp.file_type_id = " + Dropdownlist_FileType.SelectedValue;
                
                if (RadioScheduled.Checked)
                    searchCriteria += " and scheduled_downloads.file_type_id is Not null";

                if (RadioNotScheduled.Checked)
                    searchCriteria += " and scheduled_downloads.file_type_id is null";
                
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = "select temp.atm_id,temp.atm_type,temp.title,temp.file_type_id, temp.file_type_title,scheduled_downloads.file_type_id Scheduled_Files from ( " +
                    " select atm.atm_id ,atm.atm_type,atm.title,file_type.file_type_id,file_type.file_type_title,is_Active " +
                    " from atm ,file_type ) as temp " +
                    " left outer join scheduled_downloads " +
                    " on temp.atm_id = scheduled_downloads.atm_id  " +
                    " and scheduled_downloads.file_type_id = temp.file_type_id " 
                    + (searchCriteria.Length > 1 ? "where " : "") + searchCriteria + " order by " + sortOrder;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable DT = new DataTable();
                adapter.Fill(DT);
                DG_ScheduledJobs.DataSource = DT;
                DG_ScheduledJobs.DataBind();
            }
            catch (Exception ex)
            {
                LogableTask task = LogableTask.NewTask("BindGrid");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
        }
        protected void DG_ScheduledJobs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkbx = (CheckBox)e.Row.FindControl("chkbx");
                string chkboxFlag = this.DG_ScheduledJobs.DataKeys[e.Row.DataItemIndex].Values["Scheduled_files"].ToString();
                if (chkboxFlag != null && !chkboxFlag.Equals("")) // means the file is scheduled for download
                    chkbx.Checked = true;
            }
        }

        protected void DG_ScheduledJobs_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGrid("temp.title,temp.file_type_id");
        }

        protected void Schedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Schedule");
            SqlConnection SqlConn = ConnectionFactory.GetNewConnection();
            SqlConn.Open();
            SqlTransaction SqlTrx = SqlConn.BeginTransaction();
            SqlCommand cmd = SqlConn.CreateCommand();
            cmd.Transaction = SqlTrx;
            
            try
            {
                foreach (GridViewRow row in DG_ScheduledJobs.Rows)
                {
                    string atm_id = this.DG_ScheduledJobs.DataKeys[row.DataItemIndex].Values["atm_id"].ToString();
                    string file_type_id = this.DG_ScheduledJobs.DataKeys[row.DataItemIndex].Values["file_type_id"].ToString();

                    CheckBox chk = (CheckBox)row.FindControl("chkbx");

                    ScheduledDownloads objScheduledDwnlds = ScheduledDownloads.LoadScheduledDownloads("ATM_id =" + atm_id + " and file_type_id=" + file_type_id);

                    if (objScheduledDwnlds != null) // means already set to 'scheduled'
                    {
                        if (!chk.Checked) // means now it is set 'not to scheduled'
                        {
                            cmd.CommandText = "delete from scheduled_downloads where atm_id = " + atm_id + " and file_type_id = " + file_type_id;
                            cmd.ExecuteNonQuery();
                            Utility.BuildAuditLog("ScheduleDownload for ATM: " + atm_id + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ScheduleFileDownload, "Scheduled", "True", "False");
                        }
                    }
                    else // means 'Not Scheduled yet'
                    {
                        if (chk.Checked) // means now it is set to 'scheduled'
                        {
                            objScheduledDwnlds = new ScheduledDownloads();
                            objScheduledDwnlds.ATMId = int.Parse(atm_id);
                            objScheduledDwnlds.FileTypeId = int.Parse(file_type_id);
                            objScheduledDwnlds.Save(SqlConn, SqlTrx);
                            Utility.BuildAuditLog("ScheduleDownload for ATM: "+ atm_id+ " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ScheduleFileDownload, "Scheduled", "False", "True");
                        }
                    }
                }
                SqlTrx.Commit();
                                    

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "scheduling info saved");

                Literal_msg.Text = "<script>ShowSuccess('Schedule Successfully Updated.');</script>";
                  //Response.Write("<script>ShowSuccess('changes saved.');</script>");
            }
            catch (Exception ex)
            {
                SqlTrx.Rollback();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
                SqlConn.Close();
            }
        }

        protected void DG_ScheduledJobs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.DG_ScheduledJobs.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            ButtonSearch_Click(this, null);
        }
         
    }
}