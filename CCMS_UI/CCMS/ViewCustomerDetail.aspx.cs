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
using Avanza.iSuite.DAL;
//using Avanza.iSuite.DAL;

namespace CCMSUI.CCMS
{
    public partial class ViewCustomerDetail : BasePage
    {
        //static Regex formattingChars = new Regex(@"([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]\D)|([(]\d)|([(]C)|([[]040)|([[]000p)|(/)|(q[(]I)|(q[(]1)|([(]7)|([(]>)|()|([(]>)|([(]C)|([(]1)|([(]D)|(\+)");

        //int transactionId;
        //int checkValue;
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!IsPostBack)
                {
                    SmsCustomerDetail smsCustomerDetail = SmsCustomerDetail.LoadSmsCustomerDetail("ej_transaction_id = " + Request.QueryString["id"].ToString());
                    if (smsCustomerDetail != null)
                    {
                        Label_refNo.Text = SmsTask.LoadSmsTaskByPk(smsCustomerDetail.SmsTaskId).RefrenceNo;
                        Label_CustomerName.Text = smsCustomerDetail.CustomerName;
                        Label_CustomerTelephone.Text = smsCustomerDetail.CustomerTelephone;
                        Label_MobileNo.Text = smsCustomerDetail.CustomerMobileNo;

                        Label_CustomerID.Text = smsCustomerDetail.CustomerId;
                        Label_CustomerEmail.Text = smsCustomerDetail.CustomerEmail;
                        Label_CustomerCategory.Text = smsCustomerDetail.CustomerCategory;
                        Label_AvailableBalance.Text = smsCustomerDetail.AvailableBalance.ToString();
                        Label_RMName.Text = smsCustomerDetail.RmEmail;
                        Label_RMContact.Text = smsCustomerDetail.RmContact;
                        Label_RMEmail.Text = smsCustomerDetail.RmEmail;
                    }
                }
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("CustomerDetailErr", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }

        }
        protected void Button_ViewChunk_Click(object sender, EventArgs e)
        {

        }

        protected void Button_ViewEJ_Click(object sender, EventArgs e)
        {

        }
        private void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {


                //                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //                if (checkValue == 0)
                //                {
                //                    //bnatransaction.seq,smstask.refrence_no,customerdetail.customer_name,customerdetail.customer_telephone,customerdetail.customer_mobile_no    ,customerdetail.customer_id
                //                    cmd.CommandText = @" select *  from ej_parsed_bna_transaction bnatransaction inner join captured_transactions capturetransaction on capturetransaction.ej_parsed_bna_transactions_id = bnatransaction.ej_parsed_bna_transaction_id 
                //                                    inner join sms_task smstask on smstask.captured_transaction_id = capturetransaction.captured_transactions_id
                //                                    inner join sms_customer_detail customerdetail on smstask.sms_task_id = customerdetail.sms_task_id where bnatransaction.ej_parsed_bna_transaction_id = '" + transactionId + "'  ";
                //                }
                //                else
                //                {//cpmtransaction.seq,smstask.refrence_no,customerdetail.customer_name,customerdetail.customer_telephone,customerdetail.customer_mobile_no   ,customerdetail.customer_id
                //                    cmd.CommandText = @"     select * from ej_parsed_cpm_transaction cpmtransaction inner join captured_transactions capturetransaction on capturetransaction.ej_parsed_cpm_transactions_id = cpmtransaction.ej_parsed_cpm_transaction_id 
                //                                    inner join sms_task smstask on smstask.captured_transaction_id = capturetransaction.captured_transactions_id
                //                                    inner join sms_customer_detail customerdetail on smstask.sms_task_id = customerdetail.sms_task_id where cpmtransaction.ej_parsed_cpm_transaction_id = '" + transactionId + "'  ";

                //                }
                //                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //                DataTable dt = new DataTable();
                //                adapter.Fill(dt);
                //                cmd.Connection.Close();









                // this.GridView_SMSTemplate.DataSource = dtAlertTemplate;
                // this.GridView_SMSTemplate.DataBind();
                // Literal_count.Text = dtAlertTemplate.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }

    }
}