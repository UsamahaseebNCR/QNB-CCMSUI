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
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

using System.Text;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class ExceptionHandlingReport : BasePage
    {
        bool isBNADisputedAndBNAFailed = false;
        bool isBNAFailed = false;
        bool isBNADisputed = false;
        bool isCPMDisputedAndCPMFailed = false;
        bool isCPMDisputed = false;
        bool isCPMFailed = false;
        bool isCaptureCard = false;
        bool isParsedTransaction = false;
        /// <summary>
        /// page edited by shariq on 26aug15 for capture transaction report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewExceptionHandlingReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");
                //edit on 22 oct
                DropDownList_TransactionType.Items.Add(new ListItem("Any", "Any"));
                DropDownList_TransactionType.AppendDataBoundItems = true;
                DropDownList_ActionCode.Items.Add(new ListItem("Any", "Any"));
                DropDownList_ActionCode.AppendDataBoundItems = true;
                //***
                PopulateDropdownList("select transaction_type_id,transaction_type_name from transaction_type", DropDownList_TransactionType, "transaction_type_name", "transaction_type_id");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");


                //DropDownList_type.Items.Add("Any");
                //AlertType.AlertTypeReader alertTypeReader = AlertType.ExecuteReader("1=1");
                //while (alertTypeReader.Read())
                //    DropDownList_type.Items.Add(new ListItem(alertTypeReader.CurrentAlertType.AlertTypeName, alertTypeReader.CurrentAlertType.AlertTypeId.ToString()));
                //alertTypeReader.Close();



            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Expiration = null;
            if (isCheckExpired.Checked == true && CheckBoxList_Status.Text != "" || isCheckNonExpired.Checked == true && CheckBoxList_Status.Text != "")
            {
                if (isCheckNonExpired.Checked == true && isCheckExpired.Checked == true)
                    Expiration = "  ";

                else if (isCheckNonExpired.Checked == true)
                    Expiration = "and expiration_time is null ";

                else
                    Expiration = "and expiration_time is not null ";

                StringBuilder Checkboxbuilder = new StringBuilder();

                foreach (ListItem item in CheckBoxList_Status.Items)
                {
                    if (item.Selected)
                    {
                        Checkboxbuilder.Append(item.Text + ",");
                        // isCheck = true;
                    }
                }

                string[] parts = Checkboxbuilder.ToString().Split(',');
                LogableTask task = LogableTask.NewTask("Search");
                try
                {

                    AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 100;
                    //                cmd.CommandText = @"
                    //select region_name,region.region_id, parent_region_id, title, bank_logo,alert_type_name,generated_at,resolve_at,generate_at_retry_remaining,alert_msg alert_text,
                    //resolve_at_retry_remaining,generate_notification_sent,resolve_notification_sent,failure_reason,atm.location from 
                    //atm_alert,atm,alert_type,region
                    //where atm.atm_id = atm_alert.atm_id 
                    //and atm_alert.alert_type_id = alert_type.alert_type_id
                    //and atm.region_id = region.region_id
                    //                    and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"
                    //                                           + Filter();

                    //shariq
                    string condition= "";
                    string ejcapturecardCondition = "";
                    string ejparsedtranCondition = "";
                    StringBuilder builder = new StringBuilder();
                   
                    // edit on 22 oct
                    if (Array.IndexOf(parts, "BNA Disputed") > -1 && Array.IndexOf(parts, "BNA Failed") > -1)
                    {
                        isBNADisputedAndBNAFailed = true;
                        if (DropDownList_TransactionType.Text != "Any")
                            condition = "  and transaction_type_id = " + DropDownList_TransactionType.SelectedValue + "";

                    }
                    else if (Array.IndexOf(parts, "BNA Disputed") > -1)
                    {
                        isBNADisputed = true;
                        if (DropDownList_TransactionType.Text != "Any")
                            condition = " and transaction_type_id = " + DropDownList_TransactionType.SelectedValue + "";
                    }
                    else if (Array.IndexOf(parts, "BNA Failed") > -1)
                    {
                        isBNAFailed = true;
                        if (DropDownList_TransactionType.Text != "Any")
                            condition = "  and transaction_type_id = " + DropDownList_TransactionType.SelectedValue + "";
                    }
                    if (Array.IndexOf(parts, "CPM Disputed") > -1 && Array.IndexOf(parts, "CPM Failed") > -1)
                    {
                        isCPMDisputedAndCPMFailed = true;
                        if (DropDownList_TransactionType.Text != "Any")
                            condition = "  and transaction_type_id = " + DropDownList_TransactionType.SelectedValue + "";
                    }
                    else if (Array.IndexOf(parts, "CPM Disputed") > -1)
                    {
                        isCPMDisputed = true;
                        if (DropDownList_TransactionType.Text != "Any")
                            condition = "  and transaction_type_id = " + DropDownList_TransactionType.SelectedValue + "";
                    }
                    else if (Array.IndexOf(parts, "CPM Failed") > -1)
                    {
                        isCPMFailed = true;
                        if (DropDownList_TransactionType.Text != "Any")
                            condition = "  and transaction_type_id = " + DropDownList_TransactionType.SelectedValue + "";
                    }
                    if (Array.IndexOf(parts, "Captured Card") > -1)
                    {
                        isCaptureCard = true;
                        if (DropDownList_TransactionType.Text != "Any" && DropDownList_ActionCode.SelectedItem.Text != "Any")
                            ejcapturecardCondition = " and trxn_status = '" + DropDownList_ActionCode.SelectedItem.Text + "' ";//and transaction_type_id = 5
                    }
                    if (Array.IndexOf(parts, "Normal Transactions") > -1)
                    {
                        isParsedTransaction = true;
                        if (DropDownList_TransactionType.Text != "Any")
                            ejparsedtranCondition = " and  vEjParsedTransactions.transaction_type_id = " + DropDownList_TransactionType.SelectedValue + " ";
                    }
                    if (DropDownList_ActionCode.SelectedItem.Text != "Any")
                        condition += " and trxn_status = '" + DropDownList_ActionCode.SelectedItem.Text + "'";
                    //***
                    //*****************************************************************************************************
//                    if (Array.IndexOf(parts, "BNA Disputed") > -1 && Array.IndexOf(parts, "BNA Failed") > -1)
//                    {
//                        if (builder.Length > 0)
//                            builder.Append(" union ");

//                        builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,outerATM.TITLE,ra.region_name,ra.region_id, ra.parent_region_id, outerATM.ip,  trxn_datetime, amount_authorized amount,seq tsn,pan,status,' ' comment_text,'BNA FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_bna_transaction.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments,captured_transactions.amount TransactionAmount " +
//                                           " from captured_transactions inner join ej_parsed_bna_transaction " +
//                                           " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_bna_transaction.ej_parsed_bna_transaction_id " +
//                                           " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id " +
//                                           " inner join atm outerATM " +
//                                           " on ej_parsed_bna_transaction.atm_id = outerATM.atm_id inner join region ra on ra.region_id = outerATM.region_id " +
//                                           " where  outerATM.IS_ACTIVE =1 "
//+ " and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + Expiration + "");

//                    }
//                    else if (Array.IndexOf(parts, "BNA Disputed") > -1)
//                    {
//                        if (builder.Length > 0)
//                            builder.Append(" union ");

//                        builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,outerATM.TITLE,ra.region_name,ra.region_id, ra.parent_region_id, outerATM.ip,  trxn_datetime, amount_authorized amount,seq tsn,pan, status,' ' comment_text,'BNA FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_bna_transaction.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments,captured_transactions.amount TransactionAmount " +
//                                           " from captured_transactions inner join ej_parsed_bna_transaction " +
//                                           " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_bna_transaction.ej_parsed_bna_transaction_id " +
//                                           " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id " +
//                                           " inner join atm outerATM " +
//                                           " on ej_parsed_bna_transaction.atm_id = outerATM.atm_id inner join region ra on ra.region_id = outerATM.region_id " +
//                                           " where  outerATM.IS_ACTIVE =1  and is_disputed_transaction = 1"
//+ " and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + Expiration + "");

//                    }
//                    else if (Array.IndexOf(parts, "BNA Failed") > -1)
//                    {
//                        if (builder.Length > 0)
//                            builder.Append(" union ");

//                        builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,outerATM.TITLE,ra.region_name,ra.region_id, ra.parent_region_id, outerATM.ip,  trxn_datetime, amount_authorized amount,seq tsn,pan, status,' ' comment_text,'BNA FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_bna_transaction.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments,captured_transactions.amount TransactionAmount " +
//                                           " from captured_transactions inner join ej_parsed_bna_transaction " +
//                                           " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_bna_transaction.ej_parsed_bna_transaction_id " +
//                                           " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id " +
//                                           " inner join atm outerATM " +
//                                           " on ej_parsed_bna_transaction.atm_id = outerATM.atm_id inner join region ra on ra.region_id = outerATM.region_id " +
//                                           " where  outerATM.IS_ACTIVE =1  and status = 'Failed'"
//+ " and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + Expiration + "");

//                    }



//                    if (Array.IndexOf(parts, "CPM Disputed") > -1 && Array.IndexOf(parts, "CPM Failed") > -1)
//                    {
//                        if (builder.Length > 0)
//                            builder.Append(" union ");

//                        builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id,outerATM.TITLE,ra.region_name,ra.region_id, ra.parent_region_id, outerATM.ip,  trxn_datetime, captured_transactions.amount,seq tsn,pan,status,' ' comment_text,'CPM FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_cpm_transaction.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments,captured_transactions.amount TransactionAmount "
//                                         + " from captured_transactions inner join ej_parsed_cpm_transaction "
//                                         + " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id"
//                                         + " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id "
//                                         + " inner join atm outerATM "
//                                         + " on ej_parsed_cpm_transaction.atm_id = outerATM.atm_id inner join region ra on ra.region_id = outerATM.region_id "
//                                         + " where  outerATM.IS_ACTIVE =1 "
//+ " and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + Expiration + "");

//                    }
//                    else if (Array.IndexOf(parts, "CPM Disputed") > -1)
//                    {
//                        if (builder.Length > 0)
//                            builder.Append(" union ");

//                        builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id,outerATM.TITLE,ra.region_name,ra.region_id, ra.parent_region_id, outerATM.ip,  trxn_datetime, captured_transactions.amount,seq tsn,pan,status,' ' comment_text,'CPM FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_cpm_transaction.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments,captured_transactions.amount TransactionAmount "
//                                         + " from captured_transactions inner join ej_parsed_cpm_transaction "
//                                         + " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id"
//                                         + " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id "
//                                         + " inner join atm outerATM "
//                                         + " on ej_parsed_cpm_transaction.atm_id = outerATM.atm_id inner join region ra on ra.region_id = outerATM.region_id "
//                                         + " where  outerATM.IS_ACTIVE =1 and is_disputed_transaction = 1 "
//+ " and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + Expiration + "");

//                    }
//                    else if (Array.IndexOf(parts, "CPM Failed") > -1)
//                    {
//                        if (builder.Length > 0)
//                            builder.Append(" union ");

//                        builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id,outerATM.TITLE,ra.region_name,ra.region_id, ra.parent_region_id, outerATM.ip,  trxn_datetime, captured_transactions.amount,seq tsn,pan,status,' ' comment_text,'CPM FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_cpm_transaction.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments,captured_transactions.amount TransactionAmount "
//                                         + " from captured_transactions inner join ej_parsed_cpm_transaction "
//                                         + " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id"
//                                         + " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id "
//                                         + " inner join atm outerATM "
//                                         + " on ej_parsed_cpm_transaction.atm_id = outerATM.atm_id inner join region ra on ra.region_id = outerATM.region_id "
//                                         + " where  outerATM.IS_ACTIVE =1 and status = 'Failed' "
//                    + " and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + Expiration + "");

//                    }
//                    if (Array.IndexOf(parts, "Captured Card") > -1)
//                    {
//                        if (builder.Length > 0)
//                            builder.Append(" union ");

//                        builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_captured_card.ej_captured_card_id,outerATM.TITLE,ra.region_name,ra.region_id, ra.parent_region_id, outerATM.ip, capture_time trxn_datetime, captured_transactions.amount,tsn,pan,' ' status,reason comment_text,'CAPTURED CARD' transaction_type_name, transaction_rule_friendly_name,ej_captured_card.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments,captured_transactions.amount TransactionAmount " +
//                                           " from captured_transactions inner join ej_captured_card " +
//                                           " on captured_transactions.ej_captured_card_id = ej_captured_card.ej_captured_card_id " +
//                                           " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id " +
//                                           " inner join atm outerATM " +
//                                           " on ej_captured_card.atm_id = outerATM.atm_id inner join region ra on ra.region_id = outerATM.region_id  " +
//                                           " where  outerATM.IS_ACTIVE =1 "
//                           + " and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + Expiration + "");
//                    }



//                    if (builder.Length > 0)
//                        builder.Append(" union ");


//                    builder.Append(@"SELECT transaction_rule.transaction_rule_id,ej_parsed_transactions.ej_parsed_transactions_id,outerATM.TITLE,ra.region_name,ra.region_id, ra.parent_region_id, outerATM.ip, trxn_datetime, captured_transactions.amount,tsn,pan,case when status =0 then 'Successful' 
//			when status=1 then 'Failed'
//			when status =2 then 'Suspicious' end status,comment_text,transaction_type_name, transaction_rule_friendly_name,ej_parsed_transactions.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments,captured_transactions.amount TransactionAmount
//                     from captured_transactions inner join ej_parsed_transactions 
//                    on captured_transactions.transaction_id = ej_parsed_transactions.ej_parsed_transactions_id
//                    inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id
//                    inner join atm outerATM
//                    on ej_parsed_transactions.atm_id = outerATM.atm_id 
//                    inner join transaction_type on transaction_type.transaction_type_id =ej_parsed_transactions.transaction_type_id
//                    inner join comment on comment.comment_id = ej_parsed_transactions.comment_id inner join region ra on ra.region_id = outerATM.region_id "
//                    + " where  outerATM.IS_ACTIVE =1 "
//                    + " and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + Expiration + "");

//                    cmd.CommandText = builder.ToString();
                    //*******************************************************************************************
                    //shariq
                    //                if (DropDownList_type.Text == "Non Expired")
                    //                {
                    //                    cmd.CommandText = @" SELECT ej_captured_card.ej_captured_card_id,outerATM.TITLE,r.region_name, outerATM.ip, capture_time trxn_datetime,captured_transactions.amount,tsn,pan,' ' status,reason comment_text,'CAPTURED CARD' transaction_type_name, transaction_rule_friendly_name,ej_captured_card.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments  from  captured_transactions  inner join ej_captured_card  on captured_transactions.ej_captured_card_id = ej_captured_card.ej_captured_card_id  inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id  inner join atm outerATM  on ej_captured_card.atm_id = outerATM.atm_id inner join region r on r.region_id = outerATM.region_id   where  outerATM.IS_ACTIVE =1 and expiration_time is null and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + " 00:00:00',103)   and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)" +
                    //"  union SELECT ej_parsed_transactions.ej_parsed_transactions_id,outerATM.TITLE,ra.region_name, outerATM.ip, trxn_datetime,captured_transactions.amount,tsn,pan,case when status =0 then 'Successful' "
                    //+ "			when status=1 then 'Failed'"
                    //            + "when status =2 then 'Suspicious' end status,comment_text,transaction_type_name, transaction_rule_friendly_name,ej_parsed_transactions.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments"
                    //                   + "  from captured_transactions  inner join ej_parsed_transactions"
                    //                   + " on captured_transactions.transaction_id = ej_parsed_transactions.ej_parsed_transactions_id"
                    //                   + " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id"
                    //                   + " inner join atm outerATM"
                    //                   + " on ej_parsed_transactions.atm_id = outerATM.atm_id"
                    //                   + " inner join transaction_type on transaction_type.transaction_type_id =ej_parsed_transactions.transaction_type_id"
                    //                   + " inner join comment on comment.comment_id = ej_parsed_transactions.comment_id inner join region ra on ra.region_id = outerATM.region_id where  outerATM.IS_ACTIVE =1  and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + " 00:00:00',103)   and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and expiration_time is null order by trxn_datetime desc ";

                    //                }
                    //                else if (DropDownList_type.Text == "Expired")
                    //                {

                    //                    cmd.CommandText = @" SELECT ej_captured_card.ej_captured_card_id,outerATM.TITLE,r.region_name, outerATM.ip, capture_time trxn_datetime,captured_transactions.amount,tsn,pan,' ' status,reason comment_text,'CAPTURED CARD' transaction_type_name, transaction_rule_friendly_name,ej_captured_card.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments  from  captured_transactions  inner join ej_captured_card  on captured_transactions.ej_captured_card_id = ej_captured_card.ej_captured_card_id  inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id  inner join atm outerATM  on ej_captured_card.atm_id = outerATM.atm_id inner join region r on r.region_id = outerATM.region_id   where  outerATM.IS_ACTIVE =1 and expiration_time is not null and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + " 00:00:00',103)   and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)" +
                    //                    "  union SELECT ej_parsed_transactions.ej_parsed_transactions_id,outerATM.TITLE,ra.region_name, outerATM.ip, trxn_datetime,captured_transactions.amount,tsn,pan,case when status =0 then 'Successful' "
                    //                    + "			when status=1 then 'Failed'"
                    //                      + "when status =2 then 'Suspicious' end status,comment_text,transaction_type_name, transaction_rule_friendly_name,ej_parsed_transactions.task_id,captured_transactions_id,color,captured_transactions.trxn_status,captured_transactions.comments"
                    //                       + "  from captured_transactions  inner join ej_parsed_transactions"
                    //                       + " on captured_transactions.transaction_id = ej_parsed_transactions.ej_parsed_transactions_id"
                    //                       + " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id"
                    //                       + " inner join atm outerATM"
                    //                       + " on ej_parsed_transactions.atm_id = outerATM.atm_id"
                    //                       + " inner join transaction_type on transaction_type.transaction_type_id =ej_parsed_transactions.transaction_type_id"
                    //                       + " inner join comment on comment.comment_id = ej_parsed_transactions.comment_id inner join region ra on ra.region_id = outerATM.region_id where  outerATM.IS_ACTIVE =1  and captured_at >= convert(datetime,'" + TextBox_FromDate.Text + " 00:00:00',103)   and captured_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) and expiration_time is not null order by trxn_datetime desc ";
                    //                }


                    dsExceptionHandlingReport Ds = new dsExceptionHandlingReport();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = TextBox_FromDate.Text;
                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = TextBox_ToDate.Text;
                    cmd.Parameters.Add("@isBNADisputedAndBNAFailed", SqlDbType.Bit);
                    cmd.Parameters[2].Value = isBNADisputedAndBNAFailed;
                    cmd.Parameters.Add("@isBNAFailed", SqlDbType.Bit);
                    cmd.Parameters[3].Value = isBNAFailed;
                    cmd.Parameters.Add("@isBNADisputed", SqlDbType.Bit);
                    cmd.Parameters[4].Value = isBNADisputed;
                    cmd.Parameters.Add("@isCPMDisputedAndCPMFailed", SqlDbType.Bit);
                    cmd.Parameters[5].Value = isCPMDisputedAndCPMFailed;
                    cmd.Parameters.Add("@isCPMDisputed", SqlDbType.Bit);
                    cmd.Parameters[6].Value = isCPMDisputed;
                    cmd.Parameters.Add("@isCPMFailed", SqlDbType.Bit);
                    cmd.Parameters[7].Value = isCPMFailed;
                    cmd.Parameters.Add("@isCaptureCard", SqlDbType.Bit);
                    cmd.Parameters[8].Value = isCaptureCard;
                    cmd.Parameters.Add("@isParsedTransaction", SqlDbType.Bit);
                    cmd.Parameters[9].Value = isParsedTransaction;
                    cmd.Parameters.Add("@Orderby", SqlDbType.VarChar);
                    cmd.Parameters[10].Value = "trxn_datetime desc";
                    cmd.Parameters.Add("@expiration", SqlDbType.VarChar);
                    cmd.Parameters[11].Value = Expiration;
                    cmd.Parameters.Add("@conditions", SqlDbType.VarChar);
                    cmd.Parameters[12].Value = condition;
                    cmd.Parameters.Add("@ejcapturecardCondition", SqlDbType.VarChar);
                    cmd.Parameters[13].Value = ejcapturecardCondition;
                    cmd.Parameters.Add("@ejparsedtranCondition", SqlDbType.VarChar);
                    cmd.Parameters[14].Value = ejparsedtranCondition;
                    cmd.Parameters.Add("@isRunningReport", SqlDbType.Bit);
                    cmd.Parameters[15].Value = true;
                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[16].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString(); 

                    cmd.CommandText = "ExceptionHandling";
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(Ds.Tables["DataTable1"]);   //and expiration_time is null

                    PopulateDataTable(Ds.Tables["DataTable1"]);

                    //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                    //adapter.Fill(Ds.Tables["DataTable2"]);

                    if (Ds.Tables["DataTable1"].Rows.Count > 0)
                    {
                        Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                        string URL = "ReportPopup.aspx?Report=ExceptionHandlingReport.rpt&ReportTitle=Exception Handling Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count;

                        Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                    }
                    else
                        Literal_ok.Text = "<li>No record(s) found</li>";
                }

                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li> An error occurred while generating Report::" + ex.Message;
                }
                finally
                {
                    task.EndTask();
                }
            }
            else
                Literal_ok.Text = "<li>Please Select Type OR Transaction Type</li>";
        }
        private void PopulateDataTable(DataTable dt)
        {
            int region_id = 0;
            int id = -1;

            DataTable dtRegions = new DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region where parent_region_id is not null";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtRegions);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                region_id = int.Parse(dt.Rows[i][15].ToString()); //parent

                while (true)
                {
                    DataRow[] innerArray = dtRegions.Select("region_id=" + region_id);
                    if (bool.Parse(innerArray[0][2].ToString()))
                    {
                        id = int.Parse(innerArray[0][0].ToString());
                        break;
                    }
                    else
                        region_id = int.Parse(innerArray[0][1].ToString());

                }
                //organization id found
                Region region = Region.LoadRegionByPk(id);
                dt.Rows[i][16] = region.RegionName;
                // dt.Rows[i][4] = region.BankLogo;

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

     
        protected string Filter()
        {
            string filter = "";


            if (TextBox_FromDate.Text.Length > 0)
                filter += " and generated_at >= convert(datetime, '" + TextBox_FromDate.Text + " 00:00:00', 103) ";
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and generated_at <= convert(datetime, '" + TextBox_ToDate.Text + " 23:59:59', 103) ";
            //if (DropDownList_type.SelectedIndex > 0)
            //    filter += " and alert_type.alert_type_id = " + DropDownList_type.SelectedValue;


            return filter;
        }

        protected void DropDownList_TransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList_TransactionType.SelectedIndex>0)
                PopulateDropdownList("select sms_transaction_type_detail_id,action_code from sms_transaction_type_detail where transaction_type_id = '" + DropDownList_TransactionType.SelectedValue + "'", DropDownList_ActionCode, "action_code", "sms_transaction_type_detail_id");
      
        }
    }
}