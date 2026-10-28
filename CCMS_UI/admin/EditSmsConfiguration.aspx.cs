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
    public partial class EditSmsConfiguration : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            try
            {

                if (!Page.IsPostBack)
                {
                    if (Request.QueryString["configuration_id"] != null)
                        Session["configuration_id"] = Request.QueryString["configuration_id"];
                    else
                        Session["configuration_id"] = null;



                    PopulateAlertType();
                    PopulateDropdownList("select transaction_type_id,transaction_type_name from transaction_type", DropDownList_TransactionType, "transaction_type_name", "transaction_type_id");

                    PopulateDropdownList("select ccms_sms_template_id,ccms_sms_template_name from ccms_sms_template", DropDownList_TemplateName, "ccms_sms_template_name", "ccms_sms_template_id");

                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = @"select 
                                            s.* 
                                    ,(case  when s.alert_type_id is null  then 'Customers' else 'Staff' end) as AlertType,st.transaction_type_id" +

                                            " from sms_template_configuration s left outer join sms_transaction_type_detail st" +
                                            " on st.sms_transaction_type_detail_id = s.sms_transaction_type_detail_id" +

                                " where sms_template_configuration_id='" + (string)Session["configuration_id"] + "'";
                    reader = cmd.ExecuteReader();
                    int transactiontype = 0;
                    if (reader.Read())
                    {
                        DropDownList_TransactionType.SelectedValue = reader[7].ToString();

                        if (reader[6].ToString() == "Staff")
                        {
                            DropDownList_AlertType.SelectedValue = reader[5].ToString();
                            DropDownList_Alert.SelectedValue = "Staff";
                            DropDownList_Alert_SelectedIndexChanged(this, null);


                        }
                        else
                        {
                            DropDownList_TransactionType_SelectedIndexChanged(DropDownList_TransactionType, new EventArgs());
                            DropDownList_Alert.SelectedValue = "Customers";
                            DropDownList_Alert_SelectedIndexChanged(this, null);
                            DropDownList_ActionCode.SelectedValue = reader[2].ToString();
                        }
                        DropDownList_TemplateName.SelectedValue = reader[3].ToString();
                        DropDownList_Channel.SelectedValue = reader[4].ToString();
                        transactiontype = int.Parse(reader[2].ToString());
                        if (reader[1].ToString() == "None")
                        {
                            DropDownList_Status.SelectedValue = "0";
                        }
                        else if (reader[1].ToString() == "Failed")
                        {
                            DropDownList_Status.SelectedValue = "3";
                        }
                        else if (reader[1].ToString() == "Suspicious")
                        {
                            DropDownList_Status.SelectedValue = "2";
                        }
                        else if (reader[1].ToString() == "Successful")
                        {
                            DropDownList_Status.SelectedValue = "1";
                        }

                        if (DropDownList_Status.SelectedIndex > 0)
                            DropDownList_ActionCode.Enabled = false;

                        if (DropDownList_ActionCode.SelectedIndex > 0)
                            DropDownList_Status.Enabled = false;

                        //                        reader.Close();
                        //                        cmd.CommandText = @"select t.transaction_type_name from sms_transaction_type_detail st inner join
                        //                                        transaction_type t on t.transaction_type_id = st.transaction_type_id where
                        //                                        st.sms_transaction_type_detail_id = '" + transactiontype + "'";
                        //                        reader = cmd.ExecuteReader();
                        //                        if (reader.Read())
                        //                        {
                        //                            DropDownList_TransactionType = 
                        //                        }
                    }
                    else
                        DropDownList_Alert_SelectedIndexChanged(this, null);

                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("LoadSMSTemplates", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }

        }
        private void PopulateDropdownList(string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

            cmd.CommandText = query;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dtAlertType = new DataTable();

            adapter.Fill(dtAlertType);

            ddl.DataSource = dtAlertType;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
        }

  
  
        protected void Button_SaveUpdateEditAlertTemplate_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("SaveUpdateSmsConfiguration");
            try
            {
                SmsTemplateConfiguration smsTransaction;
                if (Request.QueryString["configuration_id"] != null)
                {
                    long selectedAlertTypeId = long.Parse(Session["configuration_id"].ToString());
                    smsTransaction = SmsTemplateConfiguration.LoadSmsTemplateConfigurationByPk(int.Parse(selectedAlertTypeId.ToString()));
                }
                else
                    smsTransaction = new SmsTemplateConfiguration();

                if (smsTransaction != null)
                {
                    if (DropDownList_Alert.SelectedValue != "Staff")
                    {
                        if (DropDownList_ActionCode.Text.Length > 0)
                            smsTransaction.SmsTransactionTypeDetailId = int.Parse(DropDownList_ActionCode.SelectedValue);

                        if ((int)ConnectionFactory.ExecuteScalar("select count(*) from Sms_Template_Configuration where Sms_Transaction_Type_Detail_Id=" + smsTransaction.SmsTransactionTypeDetailId + " and sms_template_configuration_id<>" + smsTransaction.SmsTemplateConfigurationId) > 0)
                            throw new Exception("Template configuration already defined for customer alert.");

                        if (DropDownList_Status.Enabled)
                        {
                            if (DropDownList_Status.SelectedIndex > 0)
                                smsTransaction.Status = DropDownList_Status.SelectedItem.Text;
                            else
                                throw new Exception("Please select transaction status for customer alert.");
                        }
                        else
                            smsTransaction.Status = DropDownList_Status.Items[0].Text;
                    }
                    else
                    {
                        smsTransaction.AlertTypeId = int.Parse(DropDownList_AlertType.SelectedValue);
                        if ((int)ConnectionFactory.ExecuteScalar("select count(*) from Sms_Template_Configuration where alert_type_id=" + smsTransaction.AlertTypeId + " and sms_template_configuration_id<>" + smsTransaction.SmsTemplateConfigurationId) > 0)
                            throw new Exception("Template configuration already defined for staff alert.");
                    }

                    if (DropDownList_TemplateName.Text.Length > 0)
                        smsTransaction.TemplateId = int.Parse(DropDownList_TemplateName.SelectedValue);

                    if (DropDownList_Channel.Text.Length > 0)
                        smsTransaction.ChannelId = int.Parse(DropDownList_Channel.SelectedValue);
                    smsTransaction.Save();
                }

                Literal_error.Text = "<li>" + "Sms Configuration Set Successfully." + "</li>";
                // Utility.BuildAuditLog(Label_Template.Text + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyAlertTemplate);

                Session[SessionVars.notice.ToString()] = "Configuration updated successfully.";
                Response.Redirect("SmsConfiguration.aspx", false);
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>An error occured while saving Sms Template Configuration." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }


        protected void DropDownList_Status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList_Status.SelectedIndex > 0)
                DropDownList_ActionCode.Enabled = false;
            else
                DropDownList_ActionCode.Enabled = true;
        }


        protected void DropDownList_ActionCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList_ActionCode.SelectedIndex > 0)
                DropDownList_Status.Enabled = false;
            else
                DropDownList_Status.Enabled = true;
        }
        protected void DropDownList_TransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDropdownList("select sms_transaction_type_detail_id,action_code from sms_transaction_type_detail where transaction_type_id = '" + DropDownList_TransactionType.SelectedValue + "'", DropDownList_ActionCode, "action_code", "sms_transaction_type_detail_id");
        }

        protected void DropDownList_Alert_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList_Alert.SelectedValue == "Staff")
            {
                Label_Select.Enabled = true;
                DropDownList_AlertType.Enabled = true;
                DropDownList_TransactionType.Enabled = false;
                DropDownList_ActionCode.Enabled = false;
                DropDownList_Status.Enabled = false;
                /*DropDownList_AlertType.Enabled = true;
                            DropDownList_TransactionType.Enabled = false;
                            DropDownList_ActionCode.Enabled = false;*/

            }
            if (DropDownList_Alert.SelectedValue == "Customers")
            {
                Label_Select.Enabled = false;
                DropDownList_AlertType.Enabled = false;
                DropDownList_TransactionType.Enabled = true;
                DropDownList_ActionCode.Enabled = true;
                DropDownList_Status.Enabled = true;
                /*DropDownList_AlertType.Enabled = false;
                            DropDownList_TransactionType.Enabled = true;
                            DropDownList_ActionCode.Enabled = true;*/
            }

        }
        private void PopulateAlertType()
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);


            //            cmd.CommandText = @"select ccms_sms_template_alert_type_id,alert.alert_type_id,alert.alert_type_name from ccms_sms_template
            //                               inner join alert_type alert on alert_type_id = ccms_sms_template_alert_type_id ";

            cmd.CommandText = @" select alert_type_id,alert_type_name from alert_type";


            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dtAlertType = new DataTable();

            adapter.Fill(dtAlertType);

            DropDownList_AlertType.DataSource = dtAlertType;
            DropDownList_AlertType.DataTextField = "alert_type_name";
            DropDownList_AlertType.DataValueField = "alert_type_id";
            DropDownList_AlertType.DataBind();
        }
    }
}