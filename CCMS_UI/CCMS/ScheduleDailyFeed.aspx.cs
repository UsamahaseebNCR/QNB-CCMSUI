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
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class ScheduleDailyFeed : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SqlCommand cmd = null;
                try
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "select distinct mcn from region where is_active =1 and is_organization = 1 and region_id > 1";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ListBox_Organization.Items.Add(new ListItem(reader[0].ToString(), reader[0].ToString()));
                    }
                    reader.Close();
                    TextBox_FromDate.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (cmd != null)
                        if (cmd.Connection != null)
                            cmd.Connection.Close();

                }



            }
        }

        protected void button_create_schedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("DFSchedule");
            try
            {
                int scheduleCount = 0;
                for (int i = 0; i < ListBox_Organization.Items.Count; i++)
                {
                    if (ListBox_Organization.Items[i].Selected)
                    {
                        Avanza.CCMS.DAL.DailyFeedSchedule dailyFeedSchedule = new Avanza.CCMS.DAL.DailyFeedSchedule();
                        dailyFeedSchedule.DateFrom = DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null);
                        dailyFeedSchedule.DateTo = DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null);
                        dailyFeedSchedule.Mcn = ListBox_Organization.Items[i].Value;
                        dailyFeedSchedule.IsExecuted = false;
                        dailyFeedSchedule.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        dailyFeedSchedule.RetryCount = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).RetryCountDffUpload;
                        dailyFeedSchedule.CreationTime = DateTime.Now;
                        dailyFeedSchedule.DeleteCurrentData = CheckBox_DeleteExistingData.Checked;
                        dailyFeedSchedule.EnableDffGeneration = CheckBox_EnableDFFGeneration.Checked;

                        dailyFeedSchedule.Save();
                        scheduleCount++;
                        Utility.BuildAuditLog("DFF Schedule created for org mcn:" + dailyFeedSchedule.Mcn, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateDailyFeedSchedule);
                    }
                }
                if (scheduleCount == 0)
                {
                    Literal_ok.Text = "Organization not selected";
                }
                else
                {
                    Session[SessionVars.notice.ToString()] = scheduleCount + " schedules created successfully<script>window.location='ViewDailyFeed.aspx'</script>";
                }
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

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

        }
    }
}