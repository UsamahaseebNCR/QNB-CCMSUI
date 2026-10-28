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

using System.Reflection;
using System.Diagnostics;
using iSuite;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

namespace CCMSUI.CCMS
{
    public partial class newDownload : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    FileType.FileTypeReader reader = FileType.ExecuteReader("1=1");
                    while (reader.Read())
                        DropDownList_fileType.Items.Add(new ListItem(reader.CurrentFileType.FileTypeTitle, reader.CurrentFileType.FileTypeId.ToString()));
                    reader.Close();
                    DropDownList_fileType.SelectedIndex = 0;
                }
                if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                    Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                    Label_atm.Text = "Please, select an ATM";
                else
                {
                    if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                    {
                        Literal_error.Text = "<li>ATM not selected.</li>";
                        Button_Download.Enabled = false;
                    }
                    else
                        Label_atm.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask task = LogableTask.NewTask("new download");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
            }


        }



        protected void Button_Download_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("new download");
            try
            {
                if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                {
                    Label_atm.Text = "Please, select an ATM";
                    Literal_error.Text = "<li>please select an ATM from the tree.</li>";
                    return;
                }
                else
                {
                    if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1
                        || Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                    {
                        Literal_error.Text = "<li>please select an ATM from the tree.</li>";
                        return;
                    }
                    Atm atm = Atm.LoadAtmByPk(int.Parse(Session[SessionVars.SelectedATMs.ToString()].ToString()));
                    Task newTask = new Task();
                    newTask.ATMId = atm.ATMId;
                    newTask.BytesTransferred = 0;
                    newTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    newTask.CreationTime = DateTime.Now;
                    newTask.DownloadTime = new DateTime(1900, 1, 1);
                    newTask.FileTypeId = int.Parse(DropDownList_fileType.SelectedValue);
                    newTask.Parsed = false;
                    newTask.RetryRemaining = (byte)atm.RetryCountCounterFile;//(byte)((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).MaxTries;
                    newTask.Status = DownloadStates.scheduled.ToString();
                    newTask.TaskTypeId = 1;
                    newTask.Save();
                    //Atm atm = Atm.LoadAtmByPk((int)newTask.ATMId);
                    //if (atm!=null)
                    Utility.BuildAuditLog(atm.Title + " scheduled for file downloading", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DownloadFileFromATM);
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "task saved. id=" + newTask.TaskId);
                    Session["msg"] = "<li> File scheduled for downloading.</li>";
                    Response.Redirect("TerminalFileStatusMonitoring.aspx", false);
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {

        }
    }
}