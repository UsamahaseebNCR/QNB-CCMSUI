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
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using Avanza.CCMS.DAL;

using ComponentArt.Web.UI;

namespace Avanza.iSuite.UI
{
    public partial class EditSmsTemplate : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!Page.IsPostBack)
                {
                    if (Request.QueryString["template_id"] != null)
                    {
                        CcmsSmsTemplate ccmsSmsTemplate = CcmsSmsTemplate.LoadCcmsSmsTemplateByPk(int.Parse(Request.QueryString["template_id"]));
                        if (ccmsSmsTemplate != null)
                        {
                            TextBox_TemplateName.Text = ccmsSmsTemplate.CcmsSmsTemplateName;
                            TextBox_TemplateMessage.Text = ccmsSmsTemplate.CcmsSmsTemplateMessage;
                            TextBox_TemplateID.Text = ccmsSmsTemplate.TibcoTemplateId.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("LoadSMSTemplates", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }

        }


        protected void Button_SaveUpdateEditAlertTemplate_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("SaveUpdateSmsTemplate");
            ///SqlConnection conn = null;
            //SqlCommand cmd = null;
            //SqlTransaction trx = null;
            //AppUser loggedInUser = (AppUser)Session[SessionVars.user.ToString()];

            try
            {
                //conn = ConnectionFactory.GetNewConnection();
                //conn.Open();
                //trx = conn.BeginTransaction();


                CcmsSmsTemplate smsTemplate;
                if (Request.QueryString["template_id"] != null)
                {
                    //long selectedAlertTypeId = long.Parse(Session["alert_type_id"].ToString());
                    smsTemplate = CcmsSmsTemplate.LoadCcmsSmsTemplateByPk(int.Parse(Request.QueryString["template_id"]));
                    smsTemplate.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    smsTemplate.ModificationTime = DateTime.Now;
                }
                else
                {

                    smsTemplate = new CcmsSmsTemplate();
                    //smsTemplate.CcmsSmsTemplateAlertTypeId = int.Parse(DropDownList_AlertType.SelectedValue);
                    smsTemplate.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    smsTemplate.CreationTime = DateTime.Now;

                }
                //if (smsTemplate != null)
                //{


                smsTemplate.CcmsSmsTemplateName = TextBox_TemplateName.Text;
                smsTemplate.CcmsSmsTemplateMessage = TextBox_TemplateMessage.Text;
                smsTemplate.TibcoTemplateId = TextBox_TemplateID.Text;
                smsTemplate.Save();

                //  }

                //if (userSelected)
                //    SQL = "update alert_type set alert_additional_text='" + TextBox_additional.Text + "',send_email_notification=1 where alert_type_id=" + selectedAlertTypeId;
                //else
                //    SQL = "update alert_type set alert_additional_text='" + TextBox_additional.Text + "',send_email_notification=null where alert_type_id=" + selectedAlertTypeId;
                //ConnectionFactory.ExecuteQuery(SQL, trx);


                Literal_error.Text = "<li>" + "Sms Templates Set Successfully." + "</li>";
                // Utility.BuildAuditLog(Label_Template.Text + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyAlertTemplate);

                Session[SessionVars.notice.ToString()] = "Template updated successfully.";
                Response.Redirect("SmsTemplate.aspx", false);

            

            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>An error occured while saving Sms Template." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                task.EndTask();
             
            }
        }
    }
}