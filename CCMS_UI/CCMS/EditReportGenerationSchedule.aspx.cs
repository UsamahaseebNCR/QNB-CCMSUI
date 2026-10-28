using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;
using Newtonsoft.Json;

namespace Avanza.iSuite.UI
{
    public partial class EditReportGenerationSchedule : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        DropDownList_Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    for (int i = 0; i < 60; i++)
                    {
                        DropDownList_Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    
                    if (Request.QueryString["report_schedule_id"] != null)
                    {
                        ReportSchedule reportSchedule = ReportSchedule.LoadReportScheduleByPk(int.Parse(Request.QueryString["report_schedule_id"]));
                        Label_ReportName.Text = reportSchedule.ReportName;
                        TextBox_ReportFriendlyName.Text = reportSchedule.ReportFriendlyName;
                        TextBox_PhysicalPath.Text = reportSchedule.ReportPhysicalPath;
						if (reportSchedule.CitId.HasValue)
                        {
                            CcmsCit cit = CcmsCit.LoadCcmsCitByPk(reportSchedule.CitId.Value);
                            Label_CIT.Text = cit.Name;
                        }								  
                        TextBox_ReportRecipients.Text = reportSchedule.ReportReceipients;
                        TextBox_TempPath.Text = reportSchedule.ReportTempPath;
                        
                        TextBox_RetryCount.Text = reportSchedule.RetryCount.ToString();
                        TextBox_ExportDataOlderThan.Text = reportSchedule.ReportDataAge.ToString();
                        if (reportSchedule.ReportExportType == 3)
                        {
                            CheckBoxList_ExportType.Items[0].Selected = true;
                            CheckBoxList_ExportType.Items[1].Selected = true;
                        }
                        else
                            CheckBoxList_ExportType.SelectedValue = reportSchedule.ReportExportType.ToString();

                        if (reportSchedule.ScheduleType)
                        {
                            RadioButtonList_ScheduleType.SelectedIndex = 1;
                            trRelative.Visible = false;
                            trAbsolute1.Visible = trAbsolute2.Visible = trAbsolute3.Visible = true;
                            ReportGenerationSchedule.ReportGenerationScheduleReader reader = ReportGenerationSchedule.ExecuteReader("report_schedule_id = " + reportSchedule.ReportScheduleId);
                            while (reader.Read())
                                Listbox_schedule.Items.Add(new ListItem(reader.CurrentReportGenerationSchedule.NextGenerationAt.Hour + ":" + reader.CurrentReportGenerationSchedule.NextGenerationAt.Minute));
                            reader.Close();

                        }
                        else
                        {
                            RadioButtonList_ScheduleType.SelectedIndex = 0;
                            trRelative.Visible = true;
                            TextBox_MinToScheduleAgain.Text = reportSchedule.MinutesToScheduleAgain.ToString();
                            trAbsolute1.Visible = trAbsolute2.Visible = trAbsolute3.Visible = false;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("LoadReportGenerationSchedule", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {

            }
        }


        protected void Button_AddSchedule_Click(object sender, EventArgs e)
        {
            
            string item = DropDownList_Hour.SelectedItem.Value + ":" + DropDownList_Min.SelectedItem.Value;
            if (!Listbox_schedule.Items.Contains(new ListItem(item)))
                Listbox_schedule.Items.Add(item);
        }

        protected void Button_RemoveSchedule_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i < Listbox_schedule.Items.Count; i++)
            {
                if (Listbox_schedule.Items[i].Selected)
                    Listbox_schedule.Items.Remove(Listbox_schedule.Items[i]);
            }
        }

        protected void Button_SaveUpdateEditReportGenerationSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("SaveUpdateReportGenerationSchedule");
            //SqlCommand cmd = null;
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            try
            {
                if (!ValidateForm())
                {
                    return;
                }
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trxn = conn.BeginTransaction();

                ReportSchedule reportSchedule = new ReportSchedule();
                ReportSchedule oldreportSchedule = new ReportSchedule();
                if (Request.QueryString["report_schedule_id"] != null)
                    reportSchedule = ReportSchedule.LoadReportScheduleByPk(int.Parse(Request.QueryString["report_schedule_id"].ToString()));
                    oldreportSchedule = ReportSchedule.LoadReportScheduleByPk(int.Parse(Request.QueryString["report_schedule_id"].ToString()));

                reportSchedule.ReportName = Label_ReportName.Text;
                reportSchedule.ReportFriendlyName = TextBox_ReportFriendlyName.Text;
                reportSchedule.ReportPhysicalPath = TextBox_PhysicalPath.Text;
                reportSchedule.ReportReceipients = TextBox_ReportRecipients.Text;
                reportSchedule.ReportTempPath = TextBox_TempPath.Text;
                reportSchedule.RetryCount = int.Parse(TextBox_RetryCount.Text);
                if (RadioButtonList_ScheduleType.SelectedIndex == 0)
                {
                    reportSchedule.MinutesToScheduleAgain = int.Parse(TextBox_MinToScheduleAgain.Text);
                    reportSchedule.ScheduleType = false;
                }
                else
                {
                    reportSchedule.MinutesToScheduleAgain = -1;
                    reportSchedule.ScheduleType = true;
                }

                reportSchedule.ReportDataAge = int.Parse(TextBox_ExportDataOlderThan.Text);
                reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                int count = 0;
                foreach (ListItem items in CheckBoxList_ExportType.Items)
                {
                    if (items.Selected)
                    {
                        reportSchedule.ReportExportType = byte.Parse(items.Value);
                        count++;
                    }
                }
                if (count == 2)
                    reportSchedule.ReportExportType = 3;
                reportSchedule.Save(conn, trxn);

                ConnectionFactory.ExecuteScalar("delete report_generation_schedule where report_schedule_id = " + reportSchedule.ReportScheduleId);
                if (RadioButtonList_ScheduleType.SelectedIndex == 1)
                {
                    for (int i = 0; i < Listbox_schedule.Items.Count; i++)
                    {
                        string[] parts = Listbox_schedule.Items[i].Value.Split(':');
                        ReportGenerationSchedule newSchedule = new ReportGenerationSchedule(DateTime.Today.AddHours(double.Parse(parts[0])).AddMinutes(double.Parse(parts[1])), reportSchedule.ReportScheduleId);
                        newSchedule.Save(conn, trxn);
                    }
                }

                
                Literal_error.Text = "<li>" + "Report Generation Schedule Saved Successfully." + "</li>";
                //Utility.BuildAuditLog(Label_ReportName.Text + " schedule modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditScheduleReports);
                editAuditLog(reportSchedule, oldreportSchedule, trxn);
                trxn.Commit();
                Session[SessionVars.notice.ToString()] = "Schedule for report [" + reportSchedule.ReportName + "] updated successfully.";
                Response.Redirect("ScheduleReportGeneration.aspx", false);


            }
            catch (Exception ex)
            {
                trxn.Rollback();
                Literal_error.Text = "<li>An error occured while saving Report Generation Schedule." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                task.EndTask();
                if (conn != null)
                    conn.Close();

            }
        }
        public void editAuditLog(ReportSchedule objCurrent, ReportSchedule objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Report " + objPrevious.ReportName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditScheduleReports, trx);
            }

        }
        private bool ValidateForm()
        {
            bool rValue = true;

            if (String.IsNullOrEmpty(TextBox_ReportFriendlyName.Text))
            {
                Literal_error.Text = "<li>" + "Please provide report friendly name." + "</li>";
                return false;
            }

            if (String.IsNullOrEmpty(TextBox_PhysicalPath.Text))
            {
                Literal_error.Text = "<li>" + "Please provide report physical path." + "</li>";
                return false;
            }

            if (String.IsNullOrEmpty(TextBox_ReportRecipients.Text))
            {
                Literal_error.Text = "<li>" + "Please provide report recipients email addresses." + "</li>";
                return false;
            }

            if (String.IsNullOrEmpty(TextBox_TempPath.Text))
            {
                Literal_error.Text = "<li>" + "Please provide report temp path." + "</li>";
                return false;
            }

            if (String.IsNullOrEmpty(TextBox_RetryCount.Text))
            {
                Literal_error.Text = "<li>" + "Please provide retry count." + "</li>";
                return false;
            }
            int count = 0;
            foreach (ListItem items in CheckBoxList_ExportType.Items)
            {
                if (items.Selected)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                Literal_error.Text = "<li>" + "Please select export type." + "</li>";
                return false;
            }
            return rValue;
        }

        protected void RadioButtonList_ScheduleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList_ScheduleType.SelectedIndex == 0)
            {
                trAbsolute1.Visible = trAbsolute2.Visible = trAbsolute3.Visible = false;
                trRelative.Visible = true;
            }
            else
            {
                trAbsolute1.Visible = trAbsolute2.Visible = trAbsolute3.Visible = true;
                trRelative.Visible = false;

            }

        }
    }
}