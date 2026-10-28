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
    public partial class ServiceRequestTask : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                    Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                    Label_atm.Text = "Please, select an ATM";
                else
                {
                    if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                    {
                        Literal_error.Text = "<li>ATM not selected.</li>";
                        Button_Schedule.Enabled = false;
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



        protected void Button_Schedule_Click(object sender, EventArgs e)
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
                    int taskTypeId = int.Parse(Request.QueryString["t"]);
                    Atm atm = Atm.LoadAtmByPk(int.Parse(Session[SessionVars.SelectedATMs.ToString()].ToString()));
                    Task newTask = new Task();
                    newTask.ATMId = atm.ATMId;
                    newTask.BytesTransferred = 0;
                    newTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    newTask.CreationTime = DateTime.Now;
                    newTask.UploadTime= new DateTime(1900, 1, 1);
                    newTask.TaskInfo = Textbox_ServiceName.Text;
                    //newTask.FileTypeId = fileTypeId;
                    newTask.Parsed = false;
                    newTask.RetryRemaining = (byte)atm.RetryCountCounterFile;//(byte)((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).MaxTries;
                    newTask.Status = DownloadStates.scheduled.ToString();
                    newTask.TaskTypeId = taskTypeId;
                    newTask.Save();
                    //Atm atm = Atm.LoadAtmByPk((int)newTask.ATMId);
                    //if (atm!=null)
                    string msg = "";
                    if (taskTypeId == 21)
                        msg = "Task scheduled for Starting \""+Textbox_ServiceName.Text+"\" on "+atm.Title;
                    else
                        msg = "Task scheduled for Stopping \"" + Textbox_ServiceName.Text + "\" on " + atm.Title;

                    Utility.BuildAuditLog(msg, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, taskTypeId == 21 ? (int)Permissions.ServiceStart : (int)Permissions.ServiceStop);
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "task saved. id=" + newTask.TaskId);
                    Session["msg"] = "<li>"+msg+".</li>";
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