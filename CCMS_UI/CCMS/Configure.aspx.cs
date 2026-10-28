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
using Avanza.iSuite.UI;
using System.Data.SqlClient;
using System.Text;
using Encryption;
using Newtonsoft.Json;

namespace CCMSUI.CCMS
{
    public partial class Configure : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");

            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ViewConfiguration, true))
                        Response.Redirect("AccessDenied.aspx");

                    Button_FIT.Visible = CheckRight((int)Permissions.UpdateFITMapping, true);

                    Button_Save.Visible = CheckRight((int)Permissions.ChangeConfiguration, true);

                    for (int i = 0; i < 24; i++)
                    {
                        //DropDownList_CashDataHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ExecHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    for (int i = 0; i < 60; i++)
                    {
                        //DropDownList_CashDataMin.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ExecMin.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    AppSetting objappsetting = AppSetting.LoadAppSetting("1=1");
                    if (objappsetting != null)
                    {
                        if (objappsetting.IsCipheredComm.HasValue)
                            CheckBox_IsCipheredComm.Checked = objappsetting.IsCipheredComm.Value;
                        else
                            objappsetting.IsCipheredComm = false;

                        TextBox_ServerIP.Text = objappsetting.ServerIp;
                        TextBox_ServerPort.Text = objappsetting.ServerPort.ToString();
                        TextBox_Port.Text = objappsetting.DefaltAtmPort.ToString();



                        TextBox_RefreshInterval.Text = objappsetting.RefreshInterval.ToString();
                        TextBox_CashDataLogLocation.Text = objappsetting.CashDataStoresLocation;
                        TextBox_HeartBeatInterval.Text = objappsetting.HeartBeatRefreshInterval.ToString();
                        //TextBox_NumberOfRetry.Text = objappsetting.MaxTries.ToString();
                        //Code added on 23/11/2011 by IK
                        TextBox_NoOfRetryCounterFile.Text = objappsetting.RetryCountCounterFile.ToString();
                        TextBox_NoOfRetryDateTimeSync.Text = objappsetting.RetryCountDatetimeSchedule.ToString();
                        TextBox_NoOfRetryRestart.Text = objappsetting.RetryCountRestartSchedule.ToString();
                        TextBox_NumberOfRetryCashOrderDownload.Text = objappsetting.RetryCountCashOrderDownload.ToString();
                        TextBox_NumberOfRetryCashOrderUpload.Text = objappsetting.RetryCountCashOrderUpload.ToString();
                        TextBox_NumberOfRetryConfUpload.Text = objappsetting.RetryCountConfUpload.ToString();
                        TextBox_NumberOfRetryDFFUpload.Text = objappsetting.RetryCountDffUpload.ToString();
                        //


                        DropDownList_ServicelogLevel.SelectedValue = objappsetting.ServiceLogLevel;
                        DropDownList_ExecMin.SelectedIndex = objappsetting.CashOrderExecutionTime.Minute;
                        DropDownList_ExecHour.SelectedIndex = objappsetting.CashOrderExecutionTime.Hour;
                        if (objappsetting.ParsingEnabled)
                            CheckBox_ParsingEnabled.Checked = objappsetting.ParsingEnabled;

                        TextBox_DashBoardRefreshInterval.Text = objappsetting.DashboardRefreshInterval.ToString();
                        TextBox_DataArchiveDatabase.Text = objappsetting.ArchivalDatabase;
                        if (objappsetting.ArchivalPassword != null)
                        {
                            TextBox_DataArchivePwd.Attributes.Add("value", objappsetting.ArchivalPassword);
                            TextBox_DataArchivePwd.Attributes.Add("onchange", "setDirtyFlagToUpdatePasswordInDb();");
                        }
                        TextBox_DataArchiveServer.Text = objappsetting.ArchivalServer;
                        TextBox_DataArchiveUsername.Text = objappsetting.ArchivalUsername;
                        DropDownList_MonthsToArchive.SelectedValue = objappsetting.ArchivalDays.ToString();
                        DropDown_AlertMonitoring.SelectedIndex = objappsetting.ThresholdForAlert.Value + 1;
                        DropDownList_FTPDashBoard.SelectedIndex = objappsetting.ThresholdForFtp.Value + 1;
                        DropDownList_TaskMonitoring.SelectedIndex = objappsetting.ThresholdForTask.Value + 1;
                        DropDownList_CashOrderMonitoring.SelectedIndex = objappsetting.ThresholdForCashorder.Value + 1;
                        TextBox_LogFilePath.Text = objappsetting.LogFilePath;

                        TextBox_TemporaryFolder.Text = objappsetting.TemporaryFolder;
                        TextBox_DownloadedFolderPath.Text = objappsetting.DownloadedFilePath;
                        if (objappsetting.IsGoogleMapEnabled.HasValue)
                            CheckBox_GMapEnabled.Checked = objappsetting.IsGoogleMapEnabled.Value;

                    }
                    GetServiceStatus();
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


        protected void Button_Save_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Save Configuration");
            SqlConnection conn = null;
            SqlTransaction trx = null;

            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                AppSetting objappsetting = AppSetting.LoadAppSetting("1=1");
                AppSetting oldObjappsetting = AppSetting.LoadAppSetting("1=1");
                if (objappsetting == null)
                    objappsetting = new AppSetting();
                bool isReqParamChanged = false;



                if (objappsetting.ServerIp != TextBox_ServerIP.Text)
                    isReqParamChanged = true;
                objappsetting.ServerIp = TextBox_ServerIP.Text;
                if (objappsetting.ServerPort != int.Parse(TextBox_ServerPort.Text))
                    isReqParamChanged = true;
                objappsetting.ServerPort = int.Parse(TextBox_ServerPort.Text);
                if (objappsetting.DefaltAtmPort != int.Parse(TextBox_Port.Text))
                    isReqParamChanged = true;
                if (TextBox_DashBoardRefreshInterval.Text.Length > 0)
                    objappsetting.DashboardRefreshInterval = int.Parse(TextBox_DashBoardRefreshInterval.Text);
                objappsetting.LogFilePath = TextBox_LogFilePath.Text;
                objappsetting.DefaltAtmPort = int.Parse(TextBox_Port.Text);
                objappsetting.RefreshInterval = int.Parse(TextBox_RefreshInterval.Text);
                objappsetting.CashDataStoresLocation = TextBox_CashDataLogLocation.Text;
                objappsetting.ServiceLogLevel = DropDownList_ServicelogLevel.SelectedValue;
                objappsetting.IsGoogleMapEnabled = CheckBox_GMapEnabled.Checked;

                if (objappsetting.IsCipheredComm.HasValue)
                {
                    if (objappsetting.IsCipheredComm.Value != CheckBox_IsCipheredComm.Checked)
                        isReqParamChanged = true;
                }

                objappsetting.IsCipheredComm = CheckBox_IsCipheredComm.Checked;



                objappsetting.TemporaryFolder = TextBox_TemporaryFolder.Text;
                objappsetting.DownloadedFilePath = TextBox_DownloadedFolderPath.Text;

                if (objappsetting.HeartBeatRefreshInterval != int.Parse(TextBox_HeartBeatInterval.Text))
                    isReqParamChanged = true;
                objappsetting.HeartBeatRefreshInterval = int.Parse(TextBox_HeartBeatInterval.Text);

                //objappsetting.MaxTries = int.Parse(TextBox_NumberOfRetry.Text);
                //Code added by IK on 23/11/2011
                objappsetting.RetryCountCounterFile = int.Parse(TextBox_NoOfRetryCounterFile.Text);
                objappsetting.RetryCountDatetimeSchedule = int.Parse(TextBox_NoOfRetryDateTimeSync.Text);
                objappsetting.RetryCountRestartSchedule = int.Parse(TextBox_NoOfRetryRestart.Text);
                objappsetting.RetryCountCashOrderDownload = int.Parse(TextBox_NumberOfRetryCashOrderDownload.Text);
                objappsetting.RetryCountCashOrderUpload = int.Parse(TextBox_NumberOfRetryCashOrderUpload.Text);
                objappsetting.RetryCountConfUpload = int.Parse(TextBox_NumberOfRetryConfUpload.Text);
                objappsetting.RetryCountDffUpload = int.Parse(TextBox_NumberOfRetryDFFUpload.Text);
                //

                //if (FileUpload_License.HasFile)
                //    if ((FileUpload_License.FileName.Contains(".lcn")) || (FileUpload_License.FileName.Contains(".LCN")))
                //    {
                //        objappsetting.LicenseKey = Encoding.ASCII.GetString(FileUpload_License.FileBytes);    
                //    }
                //    else
                //    {
                //        Literal_error.Text = "<li>" + "File Type is incorrect. Only  .lcn file allowed" + "</li>";
                //        return;
                //    }
                objappsetting.ParsingEnabled = CheckBox_ParsingEnabled.Checked;

                objappsetting.CashOrderExecutionTime = DateTime.Today.AddHours(double.Parse(DropDownList_ExecHour.SelectedItem.Value)).AddMinutes(double.Parse(DropDownList_ExecMin.SelectedItem.Value));
                objappsetting.ThresholdForAlert = int.Parse(DropDown_AlertMonitoring.SelectedItem.Value);
                objappsetting.ThresholdForFtp = int.Parse(DropDownList_FTPDashBoard.SelectedItem.Value);
                objappsetting.ThresholdForTask = int.Parse(DropDownList_TaskMonitoring.SelectedItem.Value);
                objappsetting.ThresholdForCashorder = int.Parse(DropDownList_CashOrderMonitoring.SelectedItem.Value);
                objappsetting.ArchivalDays = int.Parse(DropDownList_MonthsToArchive.SelectedItem.Value);
                objappsetting.ArchivalDatabase = TextBox_DataArchiveDatabase.Text;
                if (TextBox_DataArchivePwd.Text.Length > 0 && hidden_dirtyFlagForUserPassword.Value=="1")
                    objappsetting.ArchivalPassword = Cryptic.EncryptString(TextBox_DataArchivePwd.Text);
                objappsetting.ArchivalServer = TextBox_DataArchiveServer.Text;
                objappsetting.ArchivalUsername = TextBox_DataArchiveUsername.Text;


                objappsetting.Save();

                if (isReqParamChanged)
                {
                    Atm.AtmReader reader = Atm.ExecuteReader("is_active = 1");
                    while (reader.Read())
                    {
                        if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task where task_type_id=" + (int)EnumTaskType.Configuration + " and status in ('scheduled','uploadingDisconnected') and atm_id =" + reader.CurrentAtm.ATMId) == 0)
                        {
                            Task configTask = new Task();
                            configTask.ATMId = reader.CurrentAtm.ATMId;
                            configTask.BytesTransferred = 0;
                            configTask.CreatedBy = 1;
                            configTask.CreationTime = DateTime.Now;
                            configTask.Parsed = false;
                            configTask.RetryRemaining = (byte)reader.CurrentAtm.RetryCountConfUpload;
                            configTask.Status = UploadStates.scheduled.ToString();
                            configTask.TaskTypeId = (int)EnumTaskType.Configuration;
                            configTask.UploadTime = new DateTime(1900, 1, 1);
                            configTask.Save();
                        }
                    }
                }

                //Utility.BuildAuditLog("Application configuration modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ChangeConfiguration);
                editAuditLog(objappsetting, oldObjappsetting, trx);
                if (trx != null)
                {
                    trx.Commit();
                }
                try
                {
                    LogableTask.DefaultTraceLevel = (TraceLevel)Enum.Parse(typeof(TraceLevel), objappsetting.UILogLevel);
                }
                catch
                {
                    LogableTask.DefaultTraceLevel = TraceLevel.Info;
                    LogableTask.LogMonoActivityTask("GetTraceLevel", MethodBase.GetCurrentMethod(), TraceLevel.Error, "Failed to extract trace level from database");
                }
                Application[ApplicationVars.AppSettings.ToString()] = objappsetting;

                Literal_error.Text = "<li>" + "Configuration Set Successfully." + "</li>";
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        public void editAuditLog(AppSetting objCurrent, AppSetting objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Application configuration modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ChangeConfiguration, trx);
            }

        }

        private void GetServiceStatus()
        {
            try
            {
                string status = null;
                StringBuilder builder = new StringBuilder();
                string[] services = { "CurrencyMngServer", "CurrencyParser", "CCMSAlertDispatcher", "CCMSEJParser", "CCMSMonitor", "CCMSOrderDownloader", "CCMSTransactionMonitor", "CurrencyCutOver", "CurrencyReportSchedular", "CCMSVaultManager", "DailyFeedGenerator" };
                //string[] svcs = { "CurrencyMngServer", "CurrencyParser", "CCMSAlertDispatcher", "CCMSEJParser", "CCMSVaultManager", "CCMSOrderDownloader", "CCMSTransactionMonitor", "CurrencyCutOver", "CurrencyReportSchedular" };
                //string domain = System.Configuration.ConfigurationManager.AppSettings["domain"];
                //string username = System.Configuration.ConfigurationManager.AppSettings["username"];
                //string password = System.Configuration.ConfigurationManager.AppSettings["password"];
                //CCMSUI.SignIn.Impersonate impersonate = new SignIn.Impersonate();
                //if (impersonate.DoLogIn(domain, username, password))
                //{
                //    impersonate.DoImpersonate();

                for (int i = 0; i < services.Length; i++)
                {
                    try
                    {
                        CcmsServices ccmsServices = CcmsServices .LoadCcmsServices("name='"+services[i]+"'");
                        if (ccmsServices.ServiceStatus == "Running")
                            status = "<span style='color:green'><b>" + ccmsServices.ServiceStatus + "</b></span>";
                        else
                            status = "<span style='color:red'><b>" + ccmsServices.ServiceStatus + "</b></span>";
                        if (i == 0)
                            Literal_CurrencyMngServer_Status.Text = status;
                        else if (i == 1)
                            Literal_CurrencyParser_Status.Text = status;
                        else if (i == 2)
                            Literal_CCMSAlertDispatcher_Status.Text = status;
                        else if (i == 3)
                            Literal_CCMSEJParser_Status.Text = status;
                        else if (i == 4)
                            Literal_CCMSMonitor_Status.Text = status;
                        else if (i == 5)
                            Literal_CCMSOrderDownloader_Status.Text = status;
                        else if (i == 6)
                            Literal_CCMSTransactionMonitor_Status.Text = status;
                        else if (i == 7)
                            Literal_CurrencyCutOver_Status.Text = status;
                        else if (i == 8)
                            Literal_CurrencyReportSchedular_Status.Text = status;
                        else if (i == 9)
                            Literal_CCMSVaultManager_Status.Text = status;
                        else if (i == 10)
                            Literal_DailyFeedGenerator_Status.Text = status;

                    }
                    catch (Exception ex)
                    {
                        builder.Append(ex.Message + "<br/>");
                    }

                }
                if (builder.Length > 0)
                    Literal_error.Text = builder.ToString();
                //impersonate.Revert();

                //}
                //else
                //    Literal_error.Text = "unable to impersonate";


            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }
        }
        protected void Button_Click(object sender, EventArgs e)
        {
            try
            {
                string name = ((Button)sender).ID.ToString();
                string[] parts = name.Split('_');

                CcmsServices ccmsServices = CcmsServices.LoadCcmsServices("name='" + parts[1] + "'");
                if (ccmsServices != null)
                {
                    if (parts[2].ToLower() == "start")
                    {
                        ccmsServices.IsStartScheduled = true;
                        ccmsServices.Save();
                        Literal_error.Text = "Task scheduled to start selected service.";
                    }
                    else if (parts[2].ToLower() == "stop")
                    {
                        ccmsServices.IsStopScheduled = true;
                        ccmsServices.Save();
                        Literal_error.Text = "Task scheduled to stop selected service.";
                    }   
                }
                
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }
        }


        protected void Button_FIT_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectionFactory.ExecuteScalar(@"insert into fitmapping(pan_prefix,bank_name)
                                                select distinct substring(pan,1,6),substring(pan,1,6) from ej_parsed_transactions where pan != ' '
                                                and substring(pan,1,6) not in (select pan_prefix from fitmapping);insert into fitmapping(pan_prefix,bank_name)
                                                select distinct substring(pan,1,6),substring(pan,1,6)  from ej_captured_card where pan != ' '
                                                and substring(pan,1,6) not in (select pan_prefix from fitmapping)");

                Literal_error.Text = "<li>FIT updated successfully!</li>";
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
        }
        protected void CheckBox_PurgeDatabase_CheckChanged(object sender, EventArgs e)
        {
            if (CheckBox_PurgeDatabase.Checked)
            {
                TextBox_DataArchiveDatabase.Enabled = false;
                TextBox_DataArchiveServer.Enabled = false;
                TextBox_DataArchiveUsername.Enabled = false;
                TextBox_DataArchivePwd.Enabled = false;
            }
            else
            {
                TextBox_DataArchiveDatabase.Enabled = true;
                TextBox_DataArchiveServer.Enabled = true;
                TextBox_DataArchiveUsername.Enabled = true;
                TextBox_DataArchivePwd.Enabled = true;
            }


        }
    }

}