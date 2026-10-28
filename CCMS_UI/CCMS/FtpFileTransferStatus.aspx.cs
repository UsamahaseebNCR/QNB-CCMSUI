using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using System.IO;
using System.Web.Caching;

namespace CCMSUI.CCMS
{
    public partial class FtpFileTransferStatus : BasePage
    {
        private string GetFriendlyName(string systemFriendly)
        {
            switch (systemFriendly)
            {
                case "downloaded":
                    return "Downloaded";
                case "scheduled":
                    return "Scheduled";
                case "completed":
                    return "Completed";
                case "failed":
                    return "Failed";
                case "completedWithExceptions":
                    return "Completed With Exceptions";
                default:
                    return "Unknown";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = "creation_time";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                Array downloadStates = Enum.GetValues(typeof(FTPDownloadStatus));
                DropDownList_status.Items.Add("Any");
                for (int counter = 0; counter < downloadStates.Length; counter++)
                {
                    string tempString = downloadStates.GetValue(counter).ToString();
                    DropDownList_status.Items.Add(new ListItem(GetFriendlyName(tempString), tempString));
                }

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                
                
               
                
            }
        }

        protected string Filter()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and F.creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
            else
                filter += " and F.creation_time >= getdate() - 3";
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and F.creation_time <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";
            if (TextBox_EndTimeFromDate.Text.Length > 0)
                filter += " and F.end_time >= convert(datetime,'" + TextBox_EndTimeFromDate.Text + " ',103)";
            if (TextBox_EndTimeToDate.Text.Length > 0)
                filter += " and F.end_time <= convert(datetime,'" + TextBox_EndTimeToDate.Text + " 23:59:59',103)";
            if (DropDownList_status.SelectedIndex > 0)
                filter += " and F.status = '" + DropDownList_status.SelectedItem.Value + "'";
            if (DropDownList_TaskType.SelectedIndex > 0)
                filter += " and f.task_type_id in (" + DropDownList_TaskType.SelectedItem.Value + ")";

            return filter;
        }

        private void BindGridWithCache()
        {
            DataTable dtFtp = (DataTable)Cache["dtFtp"];
            dtFtp.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
            GridView_FileTransferStatus.DataSource = dtFtp;
            GridView_FileTransferStatus.DataBind();
            Literal_count.Text = dtFtp.Rows.Count + " records.";
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select f.ftp_file_info_id, f.end_time, f.creation_time creation_time, dbo.GetFTPStatusFriendlyName(status) status, task_type_name, region_name, case when Len(substring(failure_reason, 0, 29)) > 0 then substring(failure_reason, 0, 29)+'...' else '' end reason
                                    from ftp_file_info f, task_type t, region 
                                    where f.region_id = region.region_id 
                                    and f.task_type_id = t.task_type_id" + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtFTPStatus = new DataTable();
                adapter.Fill(dtFTPStatus);

                Cache["dtFtp"] = dtFTPStatus;
                GridView_FileTransferStatus.DataSource = dtFTPStatus;
                GridView_FileTransferStatus.DataBind();

                Literal_count.Text = dtFTPStatus.Rows.Count + " records.";
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

        protected void GridView_FileTransferStatus_Sorting(object sender, GridViewSortEventArgs e)
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

            BindGridWithCache();
        }

        protected void GridView_FileTransferStatus_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_FileTransferStatus.PageIndex = e.NewPageIndex;
            BindGridWithCache();
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_EndTimeFromDate.Text = "";
            TextBox_EndTimeToDate.Text = "";
            DropDownList_TaskType.SelectedIndex = 0;
            DropDownList_status.SelectedIndex = 0;
        }

        protected void SaveAsTXT_Click(object sender, ImageClickEventArgs e)
        {
            string tempFilename=null;
            Region organization = null;
            try
            {
                FtpFileInfo fileInfo = FtpFileInfo.LoadFtpFileInfoByPk(int.Parse(((ImageButton)sender).CommandArgument));
                if (fileInfo.TaskTypeId == (int)EnumTaskType.DailyFeedUpload)
                {
                    if (File.Exists(fileInfo.FtpFilename))
                        tempFilename = fileInfo.FtpFilename;
                    else
                        tempFilename = fileInfo.FtpFilename.Replace("PendingUpload", "OutputArchive");
                }
                else
                {
                    organization = Region.LoadRegionByPk(fileInfo.RegionId);
                    tempFilename = organization.CashOrderDownloadedFilePath + "\\" + fileInfo.FtpFilename;
                }

                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(tempFilename));
                Response.TransmitFile(tempFilename);
                Response.Flush();
                Response.Close();
                //Cache.Add(tempFilename, tempFilename, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(DeleteFile));
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
        }

        static void DeleteFile(string key, object val, CacheItemRemovedReason reas)
        {
            File.Delete(key);
        }

        protected void GridView_Files_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem).Row["ftp_file_info_id"] != DBNull.Value)
                {
                    int ftpFileInfoId = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["ftp_file_info_id"]);

                    if (((DataRowView)e.Row.DataItem).Row["status"].ToString() != "Scheduled")
                        ((HyperLink)e.Row.FindControl("hyperlink_view")).NavigateUrl = "FtpFileTransferViewer.aspx?file_id=" + ftpFileInfoId;
                    else
                    {
                        e.Row.FindControl("hyperlink_view").Visible = false;
                        e.Row.FindControl("ImageButton_SaveAsTXT").Visible = false;
                    }
                }
                else
                {
                    e.Row.FindControl("hyperlink_view").Visible = false;
                    e.Row.FindControl("ImageButton_SaveAsTXT").Visible = false;
                }
            }
        }
    }
}