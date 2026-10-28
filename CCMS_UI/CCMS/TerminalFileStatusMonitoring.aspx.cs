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
using System.Collections.Generic;
using System.Linq;
using ceTe.DynamicPDF;

namespace CCMSUI.CCMS
{
    public partial class TerminalFileStatusMonitoring : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.TaskStatusMonitoringHistory, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");
                button_search.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(button_search, null) + ";");
                HyperLink_newDownload.Visible = CheckRight((int)Permissions.DownloadFileFromATM, true);
                LinkButton_ScheduleRestart.Visible = CheckRight((int)Permissions.ScheduleRestart, true);
                LinkButton_DateTimeSync.Visible = CheckRight((int)Permissions.ScheduleDateTimeSync, true);
                LinkButton_Configuration.Visible = CheckRight((int)Permissions.ScheduleConfiguration, true);
                LinkButton_HeartBeatConfiguration.Visible = CheckRight((int)Permissions.ScheduleHeartBeatConfiguration, true);
                LinkButton_BatchConf.Visible = CheckRight((int)Permissions.SchedulBatchConfiguration, true);
                LinkButton_ScheduleInventory.Visible = CheckRight((int)Permissions.ScheduleInventory, true);



                LinkButton_CaptureScreen.Visible = CheckRight((int)Permissions.CaptureScreen, true);
                LinkButton_GetRunningApp.Visible = CheckRight((int)Permissions.TaskbarRunningApps, true);
                HyperLink_StartService.Visible = CheckRight((int)Permissions.ServiceStart, true);
                HyperLink_StopService.Visible = CheckRight((int)Permissions.ServiceStop, true);
                LinkButton_GetServiceStatus.Visible = CheckRight((int)Permissions.ServiceStatus, true);
                LinkButton_initEJ.Visible = CheckRight((int)Permissions.ExecInitEJ, true);


                // trFileTypes.Visible = true;
                //GridView_Files.Columns[3].Visible = true;


                FileType.FileTypeReader fileTypeReader = FileType.ExecuteReader("1=1");
                DropDownList_fileType.Items.Add(new ListItem("All", "All"));
                while (fileTypeReader.Read())
                    DropDownList_fileType.Items.Add(new ListItem(fileTypeReader.CurrentFileType.FileTypeTitle, fileTypeReader.CurrentFileType.FileTypeId.ToString()));
                fileTypeReader.Close();

                TaskType.TaskTypeReader taskTypeReader = TaskType.ExecuteReader("task_type_id not in (2,3) order by task_type_name");
                Dropdownlist_taskType.Items.Add(new ListItem("All", "All"));
                while (taskTypeReader.Read())
                {

                    Dropdownlist_taskType.Items.Add(new ListItem(taskTypeReader.CurrentTaskType.TaskTypeName, taskTypeReader.CurrentTaskType.TaskTypeId.ToString()));
                }
                taskTypeReader.Close();
                if (Session["taskType"] != null)
                    Dropdownlist_taskType.SelectedValue = Session["taskType"].ToString();

                // user_id =1 is for system
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("user_id > 1 order by user_login", AppUser.Columns.user_id | AppUser.Columns.user_login);
                Dropdownlist_user.Items.Add(new ListItem("Any", "Any"));
                Dropdownlist_user.Items.Add(new ListItem("Automatically", "1"));
                while (userReader.Read())
                    Dropdownlist_user.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();

                if (Session["createdBy"] != null)
                    Dropdownlist_user.SelectedValue = Session["createdBy"].ToString();


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

                if (Session["fileType"] != null)
                   DropDownList_fileType.SelectedValue = Session["fileType"].ToString();



                FillComboWithFriendlyNames(DropDownList_status, "All");
                if (Session["statusFriendlyName"] != null)
                    DropDownList_status.SelectedValue = Session["statusFriendlyName"].ToString();
                //FillComboWithUpdateFriendlyNames(DropDownList_status, false);

                DropDownList_ATMType.Items.Add(new ListItem("Any", "Any"));


                AddEditATM.PopulateATMTypes(DropDownList_ATMType);
                if (Session["atmType"] != null)
                    DropDownList_ATMType.SelectedValue = Session["atmType"].ToString();

                if (this.Session["LoadGrid"] != null)
                    this.BindGrid(null);
                this.Session["LoadGrid"] = null;

                //if (Request.Params["file_id"] != null)
                //    BinxdGrid(int.Parse(Request.Params["file_id"]));
                //else
                //BindGrid(null);
            }
        }
        public static void FillComboWithFriendlyNames(DropDownList ddlStatus, string task)
        {
            //if (clearAll)
            SqlCommand cmd = null;
            try
            {
                ddlStatus.Items.Clear();
                //  if (!ddlStatus.Items.Contains(new ListItem("All", "All")))
                ddlStatus.Items.Add(new ListItem("All", "All"));
                Array downloadStates = Enum.GetValues(typeof(DownloadStates));
                Array uploadStates = Enum.GetValues(typeof(UploadStates));


                cmd = ConnectionFactory.GetNewCommand(true);

                if (task == "All")
                {
                    for (int counter = 0; counter < downloadStates.Length; counter++)
                    {
                        string tempString = downloadStates.GetValue(counter).ToString();
                        cmd.CommandText = "select dbo.GetStatusFriendlyName ('" + tempString + "') Status";
                        cmd.CommandType = CommandType.Text;
                        ddlStatus.Items.Add(cmd.ExecuteScalar().ToString());

                    }
                    for (int counter = 0; counter < uploadStates.Length; counter++)
                    {
                        string tempString = uploadStates.GetValue(counter).ToString();
                        cmd.CommandText = "select dbo.GetStatusFriendlyName ('" + tempString + "') Status";
                        cmd.CommandType = CommandType.Text;
                        string dbString = cmd.ExecuteScalar().ToString();
                        if (!ddlStatus.Items.Contains(new ListItem(dbString)))
                            ddlStatus.Items.Add(dbString);

                    }
                }
                else if (task == "Download")
                {
                    for (int counter = 0; counter < downloadStates.Length; counter++)
                    {
                        string tempString = downloadStates.GetValue(counter).ToString();
                        cmd.CommandText = "select dbo.GetStatusFriendlyName ('" + tempString + "') Status";
                        cmd.CommandType = CommandType.Text;
                        ddlStatus.Items.Add(cmd.ExecuteScalar().ToString());

                    }

                }
                else
                {
                    for (int counter = 0; counter < uploadStates.Length; counter++)
                    {
                        string tempString = uploadStates.GetValue(counter).ToString();
                        cmd.CommandText = "select dbo.GetStatusFriendlyName ('" + tempString + "') Status";
                        cmd.CommandType = CommandType.Text;
                        ddlStatus.Items.Add(cmd.ExecuteScalar().ToString());

                    }

                }

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("FillCombo", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }


        protected void GridView_Files_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Files.PageIndex = e.NewPageIndex;
            //BindGrid(null);
            BindGridWithCache();
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
            BindGridWithCache();
        }
        protected void SaveAsTXT_Click(object sender, ImageClickEventArgs e)
        {
            try
            {

                VTask task = VTask.LoadvTaskByPk(int.Parse(((ImageButton)sender).CommandArgument));
                FileType ftype = FileType.LoadFileTypeByPk(task.FileTypeId.Value);
                int uncompressedLength;
                byte[] uncompressedData = null;
                string ejFormattedData;
                if (ftype.IsEJLog)
                {
                    uncompressedData = CashDataViewer.GetUnCompressedBytes(task, ftype, out uncompressedLength);
                    Regex regex = new Regex(Utility.GetFormatRegex());
                    //Regex regex = new Regex("([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]D)|([(]1)|([(]C)" +
                    //    "|([[]040)|([[]000p)");
                    ejFormattedData = regex.Replace(CashDataViewer.GetDecodedString(task, ftype, out uncompressedLength), "");
                    if (bool.Parse(ConfigurationManager.AppSettings["UseASCIIEncodingForEj"]))
                        uncompressedData = System.Text.ASCIIEncoding.ASCII.GetBytes(ejFormattedData);
                    else if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["UseUnicodeForEj"]) &&
                        bool.Parse(ConfigurationManager.AppSettings["UseUnicodeForEj"]))
                        uncompressedData = Encoding.Unicode.GetBytes(ejFormattedData);
                    else
                        uncompressedData = System.Text.ASCIIEncoding.UTF8.GetBytes(ejFormattedData);
                }
                else if (task.FileTypeId == 3)
                {
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                    ejFormattedData = CashDataViewer.GetStringForFileType3(task, ftype, out uncompressedLength, appSetting);
                    uncompressedData = System.Text.ASCIIEncoding.ASCII.GetBytes(ejFormattedData);
                }


                string tempFilename = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\" + DateTime.Now.ToString("yyMMddHHmmss") + ".evt";
                FileStream writer = new FileStream(tempFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                writer.Write(uncompressedData, 0, uncompressedData.Length);
                writer.Close();
                Utility.BuildAuditLog("PDF File for task: " + task.TaskId.ToString() + " Downloaded", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewEjViewerWithOptions, "FileTypeId", "", task.FileTypeId.ToString());
                Utility.
                    BuildAuditLog("PDF File for task: " + task.TaskId.ToString() + " Downloaded", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewEjViewerWithOptions, "FilePathAtATM", "", task.FilePathAtATM);
                Utility.BuildAuditLog("PDF File for task: " + task.TaskId.ToString() + " Downloaded", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewEjViewerWithOptions, "ServerFilePath", "", task.ServerFilepath);

                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition", "attachment; filename=data.txt");
//                Response.AddHeader("Content-Length", uncompressedData.Length.ToString());
                Response.AddHeader("Content-Length", new FileInfo(tempFilename).Length.ToString());

                Response.TransmitFile(tempFilename);
                Response.Flush();
                Response.Close();
                this.Cache.Add(tempFilename, tempFilename, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                    new TimeSpan(0, 1, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(DeleteFile));
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
        }
        protected void GridView_Files_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem).Row["file_type_id"] != DBNull.Value)
                {
                    FileType fileType = FileType.LoadFileTypeByPk((int)((DataRowView)e.Row.DataItem).Row["file_type_id"]);

                    //    if (fileType != null)
                    //if (fileType.IsEJLog.Value)
                    if (((DataRowView)e.Row.DataItem).Row["status"].ToString() == "Completed"
                        || ((DataRowView)e.Row.DataItem).Row["status"].ToString() == "File Downloaded"
                        || ((DataRowView)e.Row.DataItem).Row["status"].ToString() == "Failed to parse"
                        || ((DataRowView)e.Row.DataItem).Row["status"].ToString() == "Parsing")
                    {
                        if (fileType.FileTypeId == 16) // Capture Screen
                        {
                            e.Row.FindControl("ImageButton_ViewFile").Visible = false;
                            e.Row.FindControl("ImageButton_SaveAsPDF").Visible = false;
                            e.Row.FindControl("ImageButton_SaveAsTXT").Visible = false;
                            e.Row.FindControl("ImageButton_Reparse").Visible = false;

                        }
                        //else if (fileType.FileTypeId == 2 || fileType.FileTypeId == 19) // EJ Files
                        //{
                        //   // ((HyperLink)e.Row.FindControl("hyperlink_view")).NavigateUrl = "EJViewer.aspx?file_id=" + ((DataRowView)e.Row.DataItem).Row["task_id"];
                        //    e.Row.FindControl("ImageButton_ViewFile").Visible = false;
                        //    //e.Row.FindControl("ImageButton_Reparse").Visible = false;
                        //}
                        //else
                        //{
                        //    //((HyperLink)e.Row.FindControl("hyperlink_view")).NavigateUrl = "CashDataViewer.aspx?file_id=" + ((DataRowView)e.Row.DataItem).Row["task_id"];
                        //    e.Row.FindControl("ImageButton_ViewFile").Visible = false;
                        //}
                    }
                    else
                    {
                        //e.Row.FindControl("hyperlink_view").Visible = false;
                        e.Row.FindControl("ImageButton_SaveAsTXT").Visible = false;
                        e.Row.FindControl("ImageButton_SaveAsPDF").Visible = false;
                        e.Row.FindControl("ImageButton_ViewFile").Visible = false;
                        e.Row.FindControl("ImageButton_Reparse").Visible = false;
                    }
                    //if (!(bool)((DataRowView)e.Row.DataItem).Row["parsed"])
                    //    e.Row.FindControl("ImageButton_reparse").Visible = false;
                }
                else
                {
                    //if (((DataRowView)e.Row.DataItem).Row["status"].ToString() == "")
                    //    ((DataRowView)e.Row.DataItem).Row["status"] = ((DataRowView)e.Row.DataItem).Row["status1"].ToString();

                    //e.Row.FindControl("hyperlink_view").Visible = false;
                    //e.Row.FindControl("ImageButton_reparse").Visible = false;
                    e.Row.FindControl("ImageButton_SaveAsPDF").Visible = false;
                    e.Row.FindControl("ImageButton_SaveAsTXT").Visible = false;
                    e.Row.FindControl("ImageButton_ViewFile").Visible = false;
                    e.Row.FindControl("ImageButton_Reparse").Visible = false;
                }
            }
        }
        protected void ViewFile_Click(object sender, ImageClickEventArgs e)
        {
            try
            {

                VTask task = VTask.LoadvTaskByPk(int.Parse(((ImageButton)sender).CommandArgument));
                if (task.FileTypeId == 16)
                {
                    int uncompressedLength;
                    byte[] data = CashDataViewer.GetUnCompressedBytes(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength);
                    MemoryStream memoryStream = new MemoryStream(data, 0, uncompressedLength);
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(memoryStream);
                    string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\CapturedWindow.bmp";
                    bitmap.Save(path);
                    //System.Diagnostics.Process.Start(path);
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + "CapturedWindow.bmp");
                    Response.WriteFile(path);
                    Response.Flush();
                    Response.Close();
                    this.Cache.Add(path, path, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 1, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(DeleteFile));
                }
                else if (task.FileTypeId == 2|| task.FileTypeId == 19)
                {
                    Session["task_id"] = task.TaskId;
                    Response.Redirect("EJViewer.aspx");
                }
                else
                {
                    Session["task_id"] = task.TaskId;
                    Response.Redirect("CashDataViewer.aspx");
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
        }



        protected void ImageButton_Reparse_Click(object sender, ImageClickEventArgs e)
        {
            SqlCommand cmd = null;
            try
            {
                int taskID = int.Parse(((ImageButton)sender).CommandArgument);
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandText = "[reparse]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@taskID", taskID);
                cmd.ExecuteNonQuery();
                BindGrid(null);
                Session[SessionVars.notice.ToString()] = "File scheduled for reparsing";

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("Reparse", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }



        protected void SaveAsPDF_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                VTask task = VTask.LoadvTaskByPk(int.Parse(((ImageButton)sender).CommandArgument));

                FileType fType = FileType.LoadFileTypeByPk(task.FileTypeId.Value);
                int uncompressedLength;
                byte[] uncompressedData = CashDataViewer.GetUnCompressedBytes(task, fType, out uncompressedLength);
                ceTe.DynamicPDF.Document doc = new ceTe.DynamicPDF.Document();

                //Regex regex = new Regex("([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]D)|([(]1)|([(]C)" +
                //    "|([[]040)|([[]000p)");
                Regex regex = new Regex(Utility.GetFormatRegex());
                string ejFormattedData = null;
                if (task.FileTypeId == 3)
                {

                    ejFormattedData = CashDataViewer.GetStringForFileType3(task, fType, out uncompressedLength, appSetting);
                }
                else
                    ejFormattedData = regex.Replace(CashDataViewer.GetDecodedString(task, fType, out uncompressedLength), "");


                Regex regex1 = new Regex(@"\d+aLUNO123a");
                Match match = regex1.Match(ejFormattedData);
                if (match.Success)
                    ejFormattedData = regex1.Replace(ejFormattedData, "").Replace('a', '\r');



                StringReader strReader = new StringReader(ejFormattedData);

                string line;
                //AppSetting appSetting = AppSetting.LoadAppSetting("1=1");
                ceTe.DynamicPDF.Page p;
                string pageText = "";

                int linecount = 0;
                while ((line = strReader.ReadLine()) != null)
                {
                    pageText += line + "\r";
                    linecount++;
                    if (linecount == 60)
                    {
                        linecount = 0;
                        p = new ceTe.DynamicPDF.Page(ceTe.DynamicPDF.PageSize.A4, ceTe.DynamicPDF.PageOrientation.Portrait, 25);
                        p.Elements.Add(new ceTe.DynamicPDF.PageElements.Label(pageText, 20, 20, 600, 700, ceTe.DynamicPDF.Font.TimesRoman, 10));
                        doc.Pages.Add(p);
                        pageText = "";
                    }
                }

                if (linecount > 0)
                {
                    linecount = 0;
                    p = new ceTe.DynamicPDF.Page(ceTe.DynamicPDF.PageSize.A4, ceTe.DynamicPDF.PageOrientation.Portrait, 25);
                    p.Elements.Add(new ceTe.DynamicPDF.PageElements.Label(pageText, 20, 20, 600, 700, ceTe.DynamicPDF.Font.TimesRoman, 10));
                    doc.Pages.Add(p);
                    pageText = "";
                }

                string tempFile = appSetting.TemporaryFolder + "\\" + DateTime.Now.ToString("MMddHHmms") + ".pdf";
                doc.Draw(tempFile);
                Utility.BuildAuditLog("PDF File for task: "+task.TaskId.ToString() +" Downloaded", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId,(int)Permissions.ViewEjViewerWithOptions, "FileTypeId", "",task.FileTypeId.ToString());
                Utility.BuildAuditLog("PDF File for task: " + task.TaskId.ToString() + " Downloaded", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewEjViewerWithOptions, "FilePathAtATM", "", task.FilePathAtATM);
                Utility.BuildAuditLog("PDF File for task: " + task.TaskId.ToString() + " Downloaded", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewEjViewerWithOptions, "ServerFilePath", "", task.ServerFilepath);
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fType.FileTypeTitle + ".pdf");
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Length", new FileInfo(tempFile).Length.ToString());
                Response.WriteFile(tempFile);
                Response.Flush();
                Response.Close();
                strReader.Close();
                this.Cache.Add(tempFile, tempFile, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                    new TimeSpan(0, 1, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(DeleteFile));

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
        private void BindGridWithCache()
        {
            DataTable dtTask = (DataTable)Cache["dtTasks"];
            dtTask.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
            this.GridView_Files.DataSource = dtTask;
            this.GridView_Files.DataBind();
            Literal_count.Text = dtTask.Rows.Count + " records.";

        }
        void BindGrid(int? file_id)
        {
            LogableTask task = LogableTask.NewTask("BindDownloadedFilesGrid");
            DataTable filterTasks = null;
            try
            {
                // Data Archiving Work By Wajahat
                Regex regex = new Regex(Utility.GetFormatRegex());

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                ///Edited by Ali Shah on 26Aug2020 on Izhar Khan's instruction
                ///Needs to remove the limit of top 1000 records.
                ///ADCB were not able to search by keyword because top 1000 records don't contain the keyword.
                //cmd.CommandText = @"SELECT top 1000 ATM.ATM_ID,end_time, atm_type,IP, TITLE, task_id,file_type.file_type_id, F.creation_time  ,task_type.task_type_name, F.bytes_transferred,  parsed,  download_time, dbo.GetStatusFriendlyName(status) status,"
                //+ " last_invoked, case when Len(substring(failure_reason,0,29))> 0 then substring(failure_reason,0,29)+'...' else '' end reason,file_type_title FROM ATM inner join vTask F ON ATM.ATM_ID = F.ATM_ID "
                //+ " inner join task_type on f.task_type_id = task_type.task_type_id left join file_type on file_type.file_type_id = F.file_type_id where "
                //+ (file_id.HasValue ? "task_id = " + file_id.Value : (" ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and atm.IS_ACTIVE =1 " + GetFilter()))

                //+ (DropDownList_fileType.SelectedItem.Value == "19" ? "order by F.creation_time " : " order by " + ViewState["orderby"]);

                cmd.CommandText = @"SELECT ATM.ATM_ID,end_time, atm_type,IP, TITLE, task_id,file_type.file_type_id, F.creation_time  ,task_type.task_type_name, F.bytes_transferred,  parsed,  download_time, dbo.GetStatusFriendlyName(status) status,"
                + " last_invoked, case when Len(substring(failure_reason,0,29))> 0 then substring(failure_reason,0,29)+'...' else '' end reason,file_type_title FROM ATM inner join vTask F ON ATM.ATM_ID = F.ATM_ID "
                + " inner join task_type on f.task_type_id = task_type.task_type_id left join file_type on file_type.file_type_id = F.file_type_id where "
                + (file_id.HasValue ? "task_id = " + file_id.Value : (" ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and atm.IS_ACTIVE =1 " + GetFilter()))

                + (DropDownList_fileType.SelectedItem.Value == "19" ? "order by F.creation_time " : " order by " + ViewState["orderby"]);
               
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTask = new DataTable();
                //dtTask = QueryModifier.TaskQuery.DataTableQueryModify(cmd.CommandText);
                adapter.Fill(dtTask);

                if (TextBox_SpecificWord.Text.Length > 0)
                {
                    filterTasks = dtTask.Clone();
                    FileType fType = null;
                    VTask existingTask = null;
                    int uncompressedLength;
                    string content = "";

                    for (int i = 0; i < dtTask.Rows.Count;i++ )
                    {
                        uncompressedLength = 0;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "Searching Keyword in Task Id: " + int.Parse(dtTask.Rows[i]["task_id"].ToString()));
                        existingTask = VTask.LoadvTaskByPk(int.Parse(dtTask.Rows[i]["task_id"].ToString()));
                        if (existingTask.FileTypeId.HasValue)
                        {
                            //FileType fType = FileType.LoadFileTypeByPk(existingTask.FileTypeId.Value);
                            if (fType == null || fType.FileTypeId != existingTask.FileTypeId.Value)
                                fType = FileType.LoadFileTypeByPk(existingTask.FileTypeId.Value);

                            //uncompressedLength = 0;
                            //byte[] uncompressedData = CashDataViewer.GetUnCompressedBytes(existingTask, fType, out uncompressedLength);

                            //Regex regex = new Regex("([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]D)|([(]1)|([(]C)" +
                            //    "|([[]040)|([[]000p)");
                            //Regex regex = new Regex(Utility.GetFormatRegex());

                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "Going to Fetch EJ");
                            content = regex.Replace(CashDataViewer.GetDecodedString(existingTask, fType, out uncompressedLength), "");
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "Going to check Keyword in EJ");
                            if (content.Contains(TextBox_SpecificWord.Text))
                            {
                                filterTasks.ImportRow(dtTask.Rows[i]);
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "Keyword Found inside EJ for Task Id: " + existingTask.TaskId);
                                //filterTasks.Rows.Add(dtTask.Rows[i].);
                                //dtTask.Rows.Remove(dtTask.Rows[i]);
                            }
                            
                        }
                    }
                    dtTask = filterTasks;
                }
                    
                
                Cache["dtTasks"] = dtTask;
                Literal_count.Text = dtTask.Rows.Count + " records.";
                this.GridView_Files.DataSource = dtTask;
                this.GridView_Files.DataBind();
                //Commented on 18/12/2014
                //if (DropDownList_fileType.SelectedItem.Value == "19")
                //{
                //    Button_MergeAllIntoTxt.Enabled = true;
                //    Button_MergeAllIntoPDF.Enabled = true;
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


        protected void Button_MergeAllIntoTxt_Click(object sender, EventArgs e)
        {
            LogableTask logableTask = LogableTask.NewTask(nameof(Button_MergeAllIntoTxt_Click));
            try
            {
                (string fileName, string str) = this.GetMergedDataForDownloading();
                if (fileName != null && str != null)
                {
                    this.Response.Clear();
                    this.Response.ContentType = "text/plain";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".txt");
                    this.Response.AddHeader("Content-Length", str.Length.ToString());
                    this.Response.Write(str);
                    this.Response.Flush();
                    this.Response.Close();
                }
                else
                    this.Literal_error.Text = "No Data Found";
            }
            catch (Exception ex)
            {
                this.Literal_error.Text = ex.Message;
                logableTask.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                logableTask.EndTask();
            }
        }

        protected void Button_MergeAllIntoPDF_Click(object sender, EventArgs e)
        {
            LogableTask logableTask = LogableTask.NewTask(nameof(Button_MergeAllIntoPDF_Click));
            try
            {
                if (this.Cache["dtTasks"] == null)
                    return;
                int linecount = 0;
                ceTe.DynamicPDF.Page p;
                
                string pageText = ""; 
                string line;
                AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                ceTe.DynamicPDF.Document doc = new ceTe.DynamicPDF.Document();
                (string fileName, string fileData) dataForDownloading = this.GetMergedDataForDownloading();
                string fileName = dataForDownloading.fileName;
                StringReader strReader = new StringReader(dataForDownloading.fileData);
                while ((line = strReader.ReadLine()) != null)
                {
                    pageText += line + "\r";
                    linecount++;
                    if (linecount == 60)
                    {
                        linecount = 0;
                        p = new ceTe.DynamicPDF.Page(ceTe.DynamicPDF.PageSize.A4, ceTe.DynamicPDF.PageOrientation.Portrait, 25);
                        p.Elements.Add(new ceTe.DynamicPDF.PageElements.Label(pageText, 20, 20, 600, 700, ceTe.DynamicPDF.Font.TimesRoman, 10));
                        doc.Pages.Add(p);
                        pageText = "";
                    }
                }

                if (linecount > 0)
                {
                    linecount = 0;
                    p = new ceTe.DynamicPDF.Page(ceTe.DynamicPDF.PageSize.A4, ceTe.DynamicPDF.PageOrientation.Portrait, 25);
                    p.Elements.Add(new ceTe.DynamicPDF.PageElements.Label(pageText, 20, 20, 600, 700, ceTe.DynamicPDF.Font.TimesRoman, 10));
                    doc.Pages.Add(p);
                    pageText = "";
                }


                string tempFile = appSetting.TemporaryFolder + "\\" + DateTime.Now.ToString("MMddHHmms") + ".pdf";
                doc.Draw(tempFile);


                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "attachment; filename=data.pdf");
                Response.AddHeader("Content-Length", new FileInfo(tempFile).Length.ToString());
                Response.TransmitFile(tempFile);
                Response.Flush();
                Response.Close();


            }
            catch (Exception ex)
            {
                this.Literal_error.Text = ex.Message;
                logableTask.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                logableTask.EndTask();
            }
        }
        protected void Button_MergeAllIntoPDF_Click_old(object sender, EventArgs e)
        {
            LogableTask task1 = LogableTask.NewTask("Button_MergeAllIntoPDF_Click");
            try
            {
                if (Cache["dtTasks"] != null)
                {
                    string line;
                    ceTe.DynamicPDF.Page p;
                    string pageText = "";
                    int linecount = 0;
                    ceTe.DynamicPDF.Document doc = new ceTe.DynamicPDF.Document();
                    StringBuilder ejFormattedData = new StringBuilder();
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                    DataTable dtTask = (DataTable)Cache["dtTasks"];
                    foreach (DataRow dr in dtTask.Rows)
                    {
                        VTask task = VTask.LoadvTaskByPk(int.Parse(dr["task_id"].ToString()));
                        if (task.FileTypeId.HasValue)
                        {
                            FileType fType = FileType.LoadFileTypeByPk(task.FileTypeId.Value);
                            int uncompressedLength;
                            byte[] uncompressedData = EjViewer.GetUnCompressedBytes(task, fType, out uncompressedLength);

                            //Regex regex = new Regex("([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]D)|([(]1)|([(]C)" +
                            //    "|([[]040)|([[]000p)");
                            Regex regex = new Regex(Utility.GetFormatRegex());

                            ejFormattedData.Append(regex.Replace(EjViewer.GetDecodedString(task, fType, out uncompressedLength), ""));
                        }
                    }

                    StringReader strReader = new StringReader(ejFormattedData.ToString());


                    while ((line = strReader.ReadLine()) != null)
                    {
                        pageText += line + "\r";
                        linecount++;
                        if (linecount == 60)
                        {
                            linecount = 0;
                            p = new ceTe.DynamicPDF.Page(ceTe.DynamicPDF.PageSize.A4, ceTe.DynamicPDF.PageOrientation.Portrait, 25);
                            p.Elements.Add(new ceTe.DynamicPDF.PageElements.Label(pageText, 20, 20, 600, 700, ceTe.DynamicPDF.Font.TimesRoman, 10));
                            doc.Pages.Add(p);
                            pageText = "";
                        }
                    }

                    if (linecount > 0)
                    {
                        linecount = 0;
                        p = new ceTe.DynamicPDF.Page(ceTe.DynamicPDF.PageSize.A4, ceTe.DynamicPDF.PageOrientation.Portrait, 25);
                        p.Elements.Add(new ceTe.DynamicPDF.PageElements.Label(pageText, 20, 20, 600, 700, ceTe.DynamicPDF.Font.TimesRoman, 10));
                        doc.Pages.Add(p);
                        pageText = "";
                    }


                    string tempFile = appSetting.TemporaryFolder + "\\" + DateTime.Now.ToString("MMddHHmms") + ".pdf";
                    doc.Draw(tempFile);


                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=data.pdf");
                    Response.AddHeader("Content-Length", new FileInfo(tempFile).Length.ToString());
                    Response.TransmitFile(tempFile);
                    Response.Flush();
                    Response.Close();
         
                    
                    
                    
                  //  Response.AddHeader("Content-Disposition", "attachment;filename=" + "ej.pdf");
                   // Response.WriteFile(tempFile);
                    //Response.Flush();
                    //Response.Close();
                    strReader.Close();
                    this.Cache.Add(tempFile, tempFile, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 1, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(DeleteFile));
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task1.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task1.EndTask(); }

        }
        protected void Button_MergeAllIntoTxt_Click_old(object sender, EventArgs e)
        {
            LogableTask task1 = LogableTask.NewTask("Button_MergeAllIntoTxt_Click");
            try
            {


                if (Cache["dtTasks"] != null)
                {
                    StringBuilder ejFormattedData = new StringBuilder();
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                    DataTable dtTask = (DataTable)Cache["dtTasks"];
                    //dtTask.DefaultView.Sort = "task_id";
                    foreach (DataRow dr in dtTask.Rows)
                    {
                        VTask task = VTask.LoadvTaskByPk(int.Parse(dr["task_id"].ToString()));
                        if (task.FileTypeId.HasValue) // This condition added on 18/12/2014 to merge counters and other files as well.
                        {
                            FileType fType = FileType.LoadFileTypeByPk(task.FileTypeId.Value);
                            int uncompressedLength;
                            byte[] uncompressedData = CashDataViewer.GetUnCompressedBytes(task, fType, out uncompressedLength);

                            //Regex regex = new Regex("([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]D)|([(]1)|([(]C)" +
                            //    "|([[]040)|([[]000p)");
                            Regex regex = new Regex(Utility.GetFormatRegex());

                            ejFormattedData.Append(regex.Replace(CashDataViewer.GetDecodedString(task, fType, out uncompressedLength), ""));
                        }
                    }


                    string tempFilename = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\" + DateTime.Now.ToString("yyMMddHHmmss") + ".evt";
                    File.WriteAllText(tempFilename, ejFormattedData.ToString());
                    //FileStream writer = new FileStream(tempFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    //writer.Write(uncompressedData, 0, uncompressedData.Length);
                    //writer.Close();



                    Response.Clear();
                    Response.ContentType = "text/plain";
                    Response.AddHeader("Content-Disposition", "attachment; filename=data.txt");
                    Response.AddHeader("Content-Length", ejFormattedData.Length.ToString());
                    Response.TransmitFile(tempFilename);
                    Response.Flush();
                    Response.Close();
         
                    
                    
                    
                    
                    //Response.Clear();
                    ////Response.ContentType = "application/octet-stream";
                    //Response.AddHeader("Content-Disposition", "attachment; filename=ej.txt");

                    //Response.TransmitFile(tempFilename);
                    //Response.Flush();
                    //Response.Close();
                    this.Cache.Add(tempFilename, tempFilename, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 1, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(DeleteFile));
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task1.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task1.EndTask(); }

        }
        private (string fileName, string fileData) GetMergedDataForDownloading()
        {
            if (this.Cache["dtTasks"] == null)
                return ((string)null, (string)null);
            DataTable source = (DataTable)this.Cache["dtTasks"];
            if (source.AsEnumerable().Where<DataRow>((System.Func<DataRow, bool>)(row => row["ATM_id"] != DBNull.Value)).Select<DataRow, string>((System.Func<DataRow, string>)(row => row["ATM_id"].ToString())).Distinct<string>().ToList<string>().Count != 1)
                throw new Exception("Error: Multiple ATM Selected. Expected only one.");
            string str1 = source.Rows[0]["title"].ToString();
            string str2 = source.Rows[0]["file_type_title"].ToString();
            Dictionary<int, List<int>> dictionary = source.AsEnumerable().Where<DataRow>((System.Func<DataRow, bool>)(row => row["creation_time"] != DBNull.Value && row["task_id"] != DBNull.Value)).GroupBy<DataRow, int>((System.Func<DataRow, int>)(row => Convert.ToDateTime(row["creation_time"]).Year)).OrderBy<IGrouping<int, DataRow>, int>((System.Func<IGrouping<int, DataRow>, int>)(g => g.Key)).ToDictionary<IGrouping<int, DataRow>, int, List<int>>((System.Func<IGrouping<int, DataRow>, int>)(g => g.Key), (System.Func<IGrouping<int, DataRow>, List<int>>)(g => g.Select<DataRow, int>((System.Func<DataRow, int>)(row => Convert.ToInt32(row["task_id"]))).ToList<int>()));
            StringBuilder stringBuilder = new StringBuilder();
            DataTable dataTable = new DataTable();
            foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
            {
                int key = keyValuePair.Key;
                string str3 = string.Join<int>(",", (IEnumerable<int>)keyValuePair.Value);
                using (SqlCommand newCommand = ConnectionFactory.GetNewCommand(true))
                {
                    newCommand.CommandType = CommandType.StoredProcedure;
                    newCommand.CommandText = "GetCashDataFiles";
                    string str4 = ConfigurationManager.AppSettings["keepOneCashDataStoreName"];
                    if (str4.Length == 0)
                        str4 = "CashDataStore_" + key.ToString();
                    newCommand.Parameters.AddWithValue("@DbName", (object)str4);
                    newCommand.Parameters.AddWithValue("@TaskIds", (object)str3);
                    using (SqlDataReader reader = newCommand.ExecuteReader())
                        dataTable.Load((IDataReader)reader);
                    newCommand.Connection.Close();
                }
                foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                {
                    string input = CashDataViewer.GetUnCompressedBytes((byte[])row["cash_data_file"], out int _).Replace("\0", "").TrimStart();
                    Regex regex = new Regex(Utility.GetFormatRegex());
                    stringBuilder.Append(regex.Replace(input, ""));
                }
            }
            string str5 = stringBuilder.ToString();
            return (str1 + "_" + str2 + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), str5);
        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00";
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59";
            DropDownList_ATMType.SelectedIndex = 0;
            DropDownList_status.SelectedIndex = 0;
            Dropdownlist_taskType.SelectedIndex = 0;
            Dropdownlist_user.SelectedIndex = 0;
            TextBox_EndTimeFromDate.Text = "";
            TextBox_EndTimeToDate.Text = "";
            TextBox_SpecificWord.Text = "";
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
            Session["LoadGrid"] = null;
        }
        string GetFilter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
            {
                filter += " and F.creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
                Session["creationFromDate"] = TextBox_FromDate.Text;
            }
            else
                filter += " and F.creation_time >= getdate() - 3";

            if (TextBox_ToDate.Text.Length > 0)
            {
                filter += " and F.creation_time <= convert(datetime,'" + TextBox_ToDate.Text + "',103)";
                Session["creationToDate"] = TextBox_ToDate.Text;
            }


            if (TextBox_EndTimeFromDate.Text.Length > 0)
            {
                filter += " and F.end_time >= convert(datetime,'" + TextBox_EndTimeFromDate.Text + " ',103)";
                Session["endFromDate"] = TextBox_FromDate.Text;
            }
            else
                Session["endFromDate"] = null;

            if (TextBox_EndTimeToDate.Text.Length > 0)
            {
                filter += " and F.end_time <= convert(datetime,'" + TextBox_EndTimeToDate.Text + " ',103)";
                Session["endToDate"] = TextBox_EndTimeToDate.Text;
            }
            else
                Session["endToDate"] = null;


            if (Dropdownlist_taskType.SelectedIndex > 0)
            {
                filter += " and F.task_type_id  = " + Dropdownlist_taskType.SelectedValue;
                Session["taskType"] = Dropdownlist_taskType.SelectedValue;
            }
            else
                Session["taskType"] = null;

            if (DropDownList_fileType.SelectedIndex > 0)
            {
                filter += " and F.file_type_id  = " + DropDownList_fileType.SelectedValue;
                Session["fileType"] = DropDownList_fileType.SelectedValue;
            }
            else
                Session["fileType"] = null;



            if (Dropdownlist_user.SelectedIndex > 0)
            {
                filter += " and F.created_by = " + Dropdownlist_user.SelectedValue;
                Session["createdBy"] = Dropdownlist_user.SelectedValue;
            }


            if (DropDownList_status.SelectedIndex > 0)
            {
                filter += " and dbo.GetStatusFriendlyName(F.status) = '" + DropDownList_status.SelectedValue + "'";
                Session["statusFriendlyName"] = DropDownList_status.SelectedValue;
            }

            if (DropDownList_ATMType.SelectedIndex > 0)
            {
                filter += " and atm_type = '" + DropDownList_ATMType.SelectedValue + "'";
                Session["atmType"] = DropDownList_ATMType.SelectedValue;
            }

            return filter;
        }
        protected void button_search_Click(object sender, EventArgs e)
        {
            try
            {
                bool result = ValidateInputs();
                if (result == false)
                {
                    return;
                }
                else
                {
                    BindGrid(null);
                }
            }
            catch (Exception ex)
            {

                Literal_error.Text = ex.Message;
            }
            
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }

        protected void Dropdownlist_taskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Dropdownlist_taskType.SelectedItem.Value != "All")
            {
                if (int.Parse(Dropdownlist_taskType.SelectedItem.Value) == (int)EnumTaskType.CashDataDownload)
                {
                    FillComboWithFriendlyNames(DropDownList_status, "Download");

                }
                else if (int.Parse(Dropdownlist_taskType.SelectedItem.Value) == (int)EnumTaskType.CashOrderUpload ||
                    int.Parse(Dropdownlist_taskType.SelectedItem.Value) == (int)EnumTaskType.Configuration)
                {
                    FillComboWithFriendlyNames(DropDownList_status, "Upload");

                }
            }
            else
            {
                // DropDownList_status.Items.Clear();
                FillComboWithFriendlyNames(DropDownList_status, "All");
                //    FillComboWithUpdateFriendlyNames(DropDownList_status, false);
                //
            }
            BindGrid(null);

        }
        protected void LinkButton_DateTimeSync_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("DateTimeSync");
            string selectedATMs = (string)Session[SessionVars.SelectedATMs.ToString()];
            string[] selectedATMsParts = selectedATMs.Split(',');
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                bool dateTimeSyncScheduled = false;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                for (int i = 0; i < selectedATMsParts.Length; i++)
                {
                    if (selectedATMsParts[i] == "-1")
                        continue;
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task with (nolock) where task_type_id=" + (int)EnumTaskType.DateTimeSync + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
                    {
                        Task restartTask = new Task();
                        restartTask.ATMId = int.Parse(selectedATMsParts[i]);
                        restartTask.BytesTransferred = 0;
                        restartTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        restartTask.CreationTime = DateTime.Now;
                        restartTask.Parsed = false;

                        restartTask.RetryRemaining = (byte)(Atm.LoadAtmByPk(restartTask.ATMId).RetryCountDatetimeSchedule);
                        restartTask.Status = UploadStates.scheduled.ToString();
                        restartTask.TaskTypeId = (int)EnumTaskType.DateTimeSync;
                        restartTask.UploadTime = new DateTime(1900, 1, 1);
                        restartTask.Save(conn, trx);
                        dateTimeSyncScheduled = true;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Datetime sync task schedule for ATM id:" + selectedATMsParts[i]);
                    }
                    else
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Datetime sync task already scheduled for ATM id:" + selectedATMsParts[i]);
                    }
                }
                trx.Commit();
                if (dateTimeSyncScheduled)
                    Session[SessionVars.notice.ToString()] = "DateTime Sync task scheduled successfully";
                else
                    Session[SessionVars.notice.ToString()] = "DateTime Sync task already scheduled for selected ATMs";
                BindGrid(null);
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


        protected void LinkButton_Configuration_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Configuration");
            string selectedATMs = (string)Session[SessionVars.SelectedATMs.ToString()];
            string[] selectedATMsParts = selectedATMs.Split(',');
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                bool confScheduled = false;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                for (int i = 0; i < selectedATMsParts.Length; i++)
                {
                    if (selectedATMsParts[i] == "-1")
                        continue;

                    // Data Archiving Work
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from vTask with (nolock) where task_type_id=" + (int)EnumTaskType.Configuration + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
                    {
                        Task restartTask = new Task();
                        restartTask.ATMId = int.Parse(selectedATMsParts[i]);
                        restartTask.BytesTransferred = 0;
                        restartTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        restartTask.CreationTime = DateTime.Now;
                        restartTask.Parsed = false;

                        restartTask.RetryRemaining = (byte)(Atm.LoadAtmByPk(restartTask.ATMId).RetryCountConfUpload);
                        restartTask.Status = UploadStates.scheduled.ToString();
                        restartTask.TaskTypeId = (int)EnumTaskType.Configuration;
                        restartTask.UploadTime = new DateTime(1900, 1, 1);
                        restartTask.Save(conn, trx);
                        confScheduled = true;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Configuration task schedule for ATM id:" + selectedATMsParts[i]);
                    }
                    else
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Configuration task already scheduled for ATM id:" + selectedATMsParts[i]);
                    }
                }
                trx.Commit();
                if (confScheduled)
                    Session[SessionVars.notice.ToString()] = "Configuration task scheduled successfully";
                else
                    Session[SessionVars.notice.ToString()] = "Configuration task already scheduled for selected ATMs";
                BindGrid(null);
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
        protected void LinkButton_HeartBeatConfiguration_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("HeartBeatConfiguration");
            string selectedATMs = (string)Session[SessionVars.SelectedATMs.ToString()];
            string[] selectedATMsParts = selectedATMs.Split(',');
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                bool heartBeatScheduled = false;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                for (int i = 0; i < selectedATMsParts.Length; i++)
                {
                    if (selectedATMsParts[i] == "-1")
                        continue;

                    // Data Archiving Work
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from vTask with (nolock) where task_type_id=" + (int)EnumTaskType.HeartbeatConfiguration + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
                    {
                        Task restartTask = new Task();
                        restartTask.ATMId = int.Parse(selectedATMsParts[i]);
                        restartTask.BytesTransferred = 0;
                        restartTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        restartTask.CreationTime = DateTime.Now;
                        restartTask.Parsed = false;

                        restartTask.RetryRemaining = (byte)(Atm.LoadAtmByPk(restartTask.ATMId).RetryCountConfUpload);
                        restartTask.Status = UploadStates.scheduled.ToString();
                        restartTask.TaskTypeId = (int)EnumTaskType.HeartbeatConfiguration;
                        restartTask.UploadTime = new DateTime(1900, 1, 1);
                        restartTask.Save(conn, trx);
                        heartBeatScheduled = true;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Heart Beat Configuration task schedule for ATM id:" + selectedATMsParts[i]);
                    }
                    else
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Heart Beat Configuration task already scheduled for ATM id:" + selectedATMsParts[i]);
                    }
                }
                trx.Commit();
                if (heartBeatScheduled)
                    Session[SessionVars.notice.ToString()] = "Heart Beat Configuration task scheduled successfully";
                else
                    Session[SessionVars.notice.ToString()] = "Heart Beat Configuration already scheduled for selected ATMs";
                BindGrid(null);
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
        protected void LinkButton_BatchConf_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("BatchConfiguration");
            string selectedATMs = (string)Session[SessionVars.SelectedATMs.ToString()];
            string[] selectedATMsParts = selectedATMs.Split(',');
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                bool batchScheduled = false;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                for (int i = 0; i < selectedATMsParts.Length; i++)
                {
                    if (selectedATMsParts[i] == "-1")
                        continue;

                    // Data Archiving Work
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from vTask with (nolock) where task_type_id=" + (int)EnumTaskType.BatchConfiguration + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
                    {
                        Task restartTask = new Task();
                        restartTask.ATMId = int.Parse(selectedATMsParts[i]);
                        restartTask.BytesTransferred = 0;
                        restartTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        restartTask.CreationTime = DateTime.Now;
                        restartTask.Parsed = false;

                        restartTask.RetryRemaining = (byte)(Atm.LoadAtmByPk(restartTask.ATMId).RetryCountConfUpload);
                        restartTask.Status = UploadStates.scheduled.ToString();
                        restartTask.TaskTypeId = (int)EnumTaskType.BatchConfiguration;
                        restartTask.UploadTime = new DateTime(1900, 1, 1);
                        restartTask.Save(conn, trx);
                        batchScheduled = true;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Batch Configuration task schedule for ATM id:" + selectedATMsParts[i]);
                    }
                    else
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Batch Configuration task already scheduled for ATM id:" + selectedATMsParts[i]);
                    }
                }
                trx.Commit();
                if (batchScheduled)
                    Session[SessionVars.notice.ToString()] = "Batch Configuration task scheduled successfully";
                else
                    Session[SessionVars.notice.ToString()] = "Batch Configuration already scheduled for selected ATMs";
                BindGrid(null);
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

        protected void LinkButton_ScheduleInventory_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("ScheduleInventory");
            string selectedATMs = (string)Session[SessionVars.SelectedATMs.ToString()];
            string[] selectedATMsParts = selectedATMs.Split(',');
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                bool inventoryScheduled = false;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                for (int i = 0; i < selectedATMsParts.Length; i++)
                {
                    if (selectedATMsParts[i] == "-1")
                        continue;

                    // Data Archivng Work
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from vTask with (nolock) where task_type_id=" + (int)EnumTaskType.Inventory + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
                    {
                        Task invTask = new Task();
                        invTask.ATMId = int.Parse(selectedATMsParts[i]);
                        invTask.BytesTransferred = 0;
                        invTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        invTask.CreationTime = DateTime.Now;
                        invTask.Parsed = false;
                        invTask.RetryRemaining = (byte)(Atm.LoadAtmByPk(invTask.ATMId).RetryCountRestartSchedule);
                        invTask.Status = UploadStates.scheduled.ToString();
                        invTask.TaskTypeId = (int)EnumTaskType.Inventory;
                        invTask.UploadTime = new DateTime(1900, 1, 1);
                        invTask.Save(conn, trx);
                        inventoryScheduled = true;
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Inventory task schedule for ATM id:" + selectedATMsParts[i]);
                    }
                    else
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Inventory task already scheduled for ATM id:" + selectedATMsParts[i]);
                    }
                }
                trx.Commit();
                if (inventoryScheduled)
                    Session[SessionVars.notice.ToString()] = "Inventory scheduled successfully";
                else
                    Session[SessionVars.notice.ToString()] = "Inventory already scheduled for selected ATMs";
                BindGrid(null);
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

                    //Data Archivng Work
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from vTask with (nolock) where task_type_id=" + (int)EnumTaskType.Restart + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
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
                BindGrid(null);
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

                    //Data Archivng Work
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from vTask with (nolock) where task_type_id=" + (int)taskTypeId + " and status in ('scheduled','uploadingDisconnected') and atm_id= " + selectedATMsParts[i]) == 0)
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
                    BindGrid(null);
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
       

        public bool ValidateInputs()
        {
            bool result = true;
            bool isEndTimeFromDateContainSpecialChars = CheckSpecialChars(TextBox_EndTimeFromDate.Text);
            if (isEndTimeFromDateContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;EndTimeFromDate contain invalid character</img> </li>";
                return result = false;
            }
            bool isEndTimeToDateContainSpecialChars = CheckSpecialChars(TextBox_EndTimeToDate.Text);
            if (isEndTimeToDateContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;EndTimeToDate contain invalid character</img> </li>";
                return result = false;
            }
            bool isFromDateContainSpecialChars = CheckSpecialChars(TextBox_FromDate.Text);
            if (isFromDateContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;From Date contain invalid character</img> </li>";
                return result = false;
            }
            //bool isSpecificWordContainSpecialChars = CheckSpecialChars(TextBox_SpecificWord.Text);
            //if (isSpecificWordContainSpecialChars == true)
            //{
            //    Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Specific Word contain invalid character</img> </li>";
            //    return result = false;
            //}
            bool isToDateContainSpecialChars = CheckSpecialChars(TextBox_ToDate.Text);
            if (isToDateContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;To Date contain invalid character</img> </li>";
                return result = false;
            }
            return result;
        }

    }
}