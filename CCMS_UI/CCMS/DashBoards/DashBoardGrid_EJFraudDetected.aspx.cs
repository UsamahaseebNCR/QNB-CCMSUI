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
using System.Data.SqlClient;

using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using ceTe.DynamicPDF;
using ComponentArt.Web.UI;
using Avanza.iSuite.DAL;
//using Avanza.iSuite.DAL;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_EJFraudDetected : BasePage
    {
       
        bool isBNADisputedAndBNAFailed = false;
        bool isBNAFailed = false;
        bool isBNADisputed = false;
        bool isCPMDisputedAndCPMFailed = false;
        bool isCPMDisputed = false;
        bool isCPMFailed = false;
        bool isCaptureCard = false;
        bool isParsedTransaction = false;

     
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.CallBack1.CausedCallback || this.DialogCallBackCustomerDetail.CausedCallback)
                return;
            //if (this.DialogCallBack.CausedCallback || this.DialogCallBackCustomerDetail.CausedCallback)
            //    return;

            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewFraudTransactionsDashBoard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                

                ViewState["SortExpression"] = "trxn_datetime ";
                ViewState["sortDirection"] = "desc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                if (System.Configuration.ConfigurationManager.AppSettings["IsBankNameEnable"] == "false")
                {
                    GridView_Trxn.Columns[2].Visible = false;
                }

            }
            
        }

        protected void GridView_Trxn_Sorting(object sender, GridViewSortEventArgs e)
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

            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            BindGrid(ViewState["SortExpression"] + " " + ViewState["sortDirection"]);
            //BindGridWithCache();
        }
        /// <summary>
        /// queries edited by shariq on 26 aug15 transaction_rule_id inserted in select queries and cpmdisputed cpm failed queries
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns>lots of changes in GetSQL method</returns>

        private string GetSQL(string orderBy)
        {
           
            StringBuilder builder = new StringBuilder();
            if (Request.QueryString["c"] != null)
            {
                //builder.Append("");
                Session["change"] = Request.QueryString["c"].ToString();
            }
            //else
            //{

//                string[] parts = Convert.ToString(Session["change"]).Split(',');
//                if (Array.IndexOf(parts, "BNA Disputed") > -1 && Array.IndexOf(parts, "BNA Failed") > -1)
//                {
//                    if (builder.Length > 0)
//                        builder.Append(" union ");
                   
//                    builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,outerATM.TITLE, outerATM.ip,  trxn_datetime, amount_authorized amount,seq tsn,pan,status,comment comment_text,'BNA FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_bna_transaction.task_id,captured_transactions_id,color,account_no " +
//                                       " from captured_transactions inner join ej_parsed_bna_transaction " +
//                                       " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_bna_transaction.ej_parsed_bna_transaction_id " +
//                                       " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id " +
//                                       " inner join atm outerATM " +
//                                       " on ej_parsed_bna_transaction.atm_id = outerATM.atm_id " +
//                                       " where  outerATM.IS_ACTIVE =1 "
//                                     + " and captured_at >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  " +
//                                       " and captured_at <= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and expiration_time is null  ");

//                }
//                else if (Array.IndexOf(parts, "BNA Disputed") > -1)
//                {
//                    if (builder.Length > 0)
//                        builder.Append(" union ");

//                    builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,outerATM.TITLE, outerATM.ip,  trxn_datetime, amount_authorized amount,seq tsn,pan, status,comment comment_text,'BNA FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_bna_transaction.task_id,captured_transactions_id,color,account_no " +
//                                       " from captured_transactions inner join ej_parsed_bna_transaction " +
//                                       " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_bna_transaction.ej_parsed_bna_transaction_id " +
//                                       " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id " +
//                                       " inner join atm outerATM " +
//                                       " on ej_parsed_bna_transaction.atm_id = outerATM.atm_id " +
//                                       " where  outerATM.IS_ACTIVE =1  and is_disputed_transaction = 1"
//                                     + " and captured_at >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  " +
//                                       " and captured_at <= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and expiration_time is null  ");

//                }
//                else if (Array.IndexOf(parts, "BNA Failed") > -1)
//                {
//                    if (builder.Length > 0)
//                        builder.Append(" union ");

//                    builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,outerATM.TITLE, outerATM.ip,  trxn_datetime, amount_authorized amount,seq tsn,pan, status,comment comment_text,'BNA FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_bna_transaction.task_id,captured_transactions_id,color,account_no " +
//                                       " from captured_transactions inner join ej_parsed_bna_transaction " +
//                                       " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_bna_transaction.ej_parsed_bna_transaction_id " +
//                                       " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id " +
//                                       " inner join atm outerATM " +
//                                       " on ej_parsed_bna_transaction.atm_id = outerATM.atm_id " +
//                                       " where  outerATM.IS_ACTIVE =1  and status = 'Failed'"
//                                     + " and captured_at >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  " +
//                                       " and captured_at <= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and expiration_time is null  ");

//                }



//                if (Array.IndexOf(parts, "CPM Disputed") > -1 && Array.IndexOf(parts, "CPM Failed") > -1)
//                {
//                    if (builder.Length > 0)
//                        builder.Append(" union ");

//                    builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id,outerATM.TITLE, outerATM.ip,  trxn_datetime, captured_transactions.amount,seq tsn,pan,status,comment comment_text,'CPM FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_cpm_transaction.task_id,captured_transactions_id,color,account_no "
//                                     + " from captured_transactions inner join ej_parsed_cpm_transaction "
//                                     + " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id" 
//                                     + " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id "
//                                     + " inner join atm outerATM "
//                                     + " on ej_parsed_cpm_transaction.atm_id = outerATM.atm_id" 
//                                     + " where  outerATM.IS_ACTIVE =1 "
//                                     + " and captured_at >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  " +
//                                       " and captured_at <= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and expiration_time is null  ");

//                }
//                else if (Array.IndexOf(parts, "CPM Disputed") > -1)
//                {
//                    if (builder.Length > 0)
//                        builder.Append(" union ");

//                    builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id,outerATM.TITLE, outerATM.ip,  trxn_datetime, captured_transactions.amount,seq tsn,pan,status,comment comment_text,'CPM FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_cpm_transaction.task_id,captured_transactions_id,color,account_no "
//                                     + " from captured_transactions inner join ej_parsed_cpm_transaction "
//                                     + " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id"
//                                     + " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id "
//                                     + " inner join atm outerATM "
//                                     + " on ej_parsed_cpm_transaction.atm_id = outerATM.atm_id"
//                                     + " where  outerATM.IS_ACTIVE =1 and is_disputed_transaction = 1 "
//                                     + " and captured_at >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  " +
//                                       " and captured_at <= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and expiration_time is null  ");

//                }
//                else if (Array.IndexOf(parts, "CPM Failed") > -1)
//                {
//                    if (builder.Length > 0)
//                        builder.Append(" union ");

//                    builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id,outerATM.TITLE, outerATM.ip,  trxn_datetime, captured_transactions.amount,seq tsn,pan,status,comment comment_text,'CPM FAILED TRXN' transaction_type_name, transaction_rule_friendly_name,ej_parsed_cpm_transaction.task_id,captured_transactions_id,color,account_no "
//                                     + " from captured_transactions inner join ej_parsed_cpm_transaction "
//                                     + " on captured_transactions.ej_parsed_bna_transactions_id = ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id"
//                                     + " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id "
//                                     + " inner join atm outerATM "
//                                     + " on ej_parsed_cpm_transaction.atm_id = outerATM.atm_id"
//                                     + " where  outerATM.IS_ACTIVE =1 and status = 'Failed' "
//                                     + " and captured_at >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  " +
//                                       " and captured_at <= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and expiration_time is null  ");

//                }
//                if (Array.IndexOf(parts, "Captured Card") > -1)
//                {
//                    if (builder.Length > 0)
//                         builder.Append(" union ");

//                    builder.Append(" SELECT transaction_rule.transaction_rule_id,ej_captured_card.ej_captured_card_id,outerATM.TITLE, outerATM.ip, capture_time trxn_datetime, captured_transactions.amount,tsn,pan,' ' status,reason comment_text,'CAPTURED CARD' transaction_type_name, transaction_rule_friendly_name,ej_captured_card.task_id,captured_transactions_id,color,' ' account_no " +
//                                       " from captured_transactions inner join ej_captured_card " +
//                                       " on captured_transactions.ej_captured_card_id = ej_captured_card.ej_captured_card_id " +
//                                       " inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id " +
//                                       " inner join atm outerATM " +
//                                       " on ej_captured_card.atm_id = outerATM.atm_id " +
//                                       " where  outerATM.IS_ACTIVE =1 "
//                                      + " and captured_at >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  " +
//                                        " and captured_at <= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and expiration_time is null");
//                }


//                if (Array.IndexOf(parts, "Normal Transactions") > -1)
//                {


//                    if (builder.Length > 0)
//                        builder.Append(" union ");


//                    builder.Append(@"SELECT transaction_rule.transaction_rule_id,ej_parsed_transactions.ej_parsed_transactions_id,outerATM.TITLE, outerATM.ip, trxn_datetime, captured_transactions.amount,tsn,pan,case when status =0 then 'Successful' 
//			when status=1 then 'Failed'
//			when status =2 then 'Suspicious' end status,comment_text,transaction_type_name, transaction_rule_friendly_name,ej_parsed_transactions.task_id,captured_transactions_id,color,' ' account_no
//                     from captured_transactions inner join ej_parsed_transactions 
//                    on captured_transactions.transaction_id = ej_parsed_transactions.ej_parsed_transactions_id
//                    inner join transaction_rule on transaction_rule.transaction_rule_id =captured_transactions.transaction_rule_id
//                    inner join atm outerATM
//                    on ej_parsed_transactions.atm_id = outerATM.atm_id 
//                    inner join transaction_type on transaction_type.transaction_type_id =ej_parsed_transactions.transaction_type_id
//                    inner join comment on comment.comment_id = ej_parsed_transactions.comment_id"
//                    + " where  outerATM.IS_ACTIVE =1 "
//                    + " and captured_at >= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)  " +
//                        " and captured_at <= convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) and expiration_time is null");

//                }
//            //}
            
            
            string[] parts = Convert.ToString(Session["change"]).Split(',');
            if (Array.IndexOf(parts, "BNA Disputed") > -1 && Array.IndexOf(parts, "BNA Failed") > -1)
                isBNADisputedAndBNAFailed = true;

            else if (Array.IndexOf(parts, "BNA Disputed") > -1)
                isBNADisputed = true;

            else if (Array.IndexOf(parts, "BNA Failed") > -1)
                isBNAFailed = true;

            if (Array.IndexOf(parts, "CPM Disputed") > -1 && Array.IndexOf(parts, "CPM Failed") > -1)
                isCPMDisputedAndCPMFailed = true;

            else if (Array.IndexOf(parts, "CPM Disputed") > -1)
                isCPMDisputed = true;

            else if (Array.IndexOf(parts, "CPM Failed") > -1)
                isCPMFailed = true;

            if (Array.IndexOf(parts, "Captured Card") > -1)
                isCaptureCard = true;

            if (Array.IndexOf(parts, "Normal Transactions") > -1)
                isParsedTransaction = true;


            //******

            
            builder.Append(" order by " + orderBy);
            return builder.ToString();
        }
        //*******
        private void BindGrid(string orderBy)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                //outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and
                this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                this.Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(this.metaRefresh.Content) / 60) + "\");} init();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = GetSQL(orderBy);
                    
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = DateTime.Today;
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
                cmd.Parameters[10].Value = orderBy;
                //edit by shariq on 21 oct    
                cmd.Parameters.Add("@expiration", SqlDbType.VarChar);
                cmd.Parameters[11].Value = " and expiration_time is null ";
                cmd.Parameters.Add("@conditions", SqlDbType.VarChar);
                cmd.Parameters[12].Value = "";
                cmd.Parameters.Add("@ejcapturecardCondition", SqlDbType.VarChar);
                cmd.Parameters[13].Value = "";
                cmd.Parameters.Add("@ejparsedtranCondition", SqlDbType.VarChar);
                cmd.Parameters[14].Value = " and is_disputed_transaction = 1 ";
                cmd.Parameters.Add("@isRunningReport", SqlDbType.Bit);
                cmd.Parameters[15].Value = false;
                cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                cmd.Parameters[16].Value = HttpContext.Current.Session[SessionVars.SelectedATMs.ToString()].ToString(); 
                //*******
                cmd.CommandText = "ExceptionHandling";
                
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                this.GridView_Trxn.DataSource = dt;
                this.GridView_Trxn.DataBind();
                Literal_AddHeading.Text = "NCR Exception Handling DashBoard - Total No.Of Records : " + dt.Rows.Count;
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();

                task.EndTask();
            }
        }
        protected void GridView_CashPositions_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        }

        protected void GridView_Trxn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string colorCode = ((DataRowView)e.Row.DataItem).Row.ItemArray[17].ToString();
                if (colorCode.Length>0)
                //if (e.Row.Cells[0].Text.Length > 0)
                    e.Row.Cells[0].BackColor = System.Drawing.ColorTranslator.FromHtml(colorCode);




                if (((DataRowView)e.Row.DataItem).Row.ItemArray[22].ToString().Length == 0)
                    e.Row.FindControl("anchorInquiry").Visible = false;
                else
                    ((ImageButton)e.Row.FindControl("anchorInquiry")).Attributes.Add("onclick", "javascript:openDialogCustomerDetail(" + ((DataRowView)e.Row.DataItem).Row.ItemArray[16] + ");return false;");



                
                //  if ( e.Row.DataItem
                //    TransactionRule rule = TransactionRule.LoadTransactionRule("transaction_rule_friendly_name='" + e.Row.Cells[2].Text + "'");
            //    if (rule != null)
            //    {
            //        if (rule.Color != null)
            //            if (rule.Color.Length > 0)
            //                e.Row.Cells[0].BackColor = System.Drawing.ColorTranslator.FromHtml(rule.Color);
            //    }
            //    if (e.Row.Cells[12].Text == "Successful")
            //    {
            //        e.Row.Cells[12].BackColor = System.Drawing.Color.Green;
            //        e.Row.Cells[12].ForeColor = System.Drawing.Color.White;
            //    }
            //    else if (e.Row.Cells[12].Text == "Failed")
            //    {
            //        e.Row.Cells[12].BackColor = System.Drawing.Color.Red;
            //        e.Row.Cells[12].ForeColor = System.Drawing.Color.White;
            //    }


            }

        }
        //protected void DialogCallBack_Callback(object sender, CallBackEventArgs e)
        //{
        //    //LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
        //    // SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
        //    // cmd.CommandTimeout = 600;
        //    LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
        //    EjParsedBnaTransaction objEjParsedBNA = null;
        //    EjParsedCpmTransaction objEjParsedCPM = null;
        //    EjParsedTransactions objEjParsed = null;
        //    int? transactionTypeId = null;
        //    int atmID = -1;
        //    string PAN = "";
        //    string accountNo = "";
        //    string tsn = "";
        //    try
        //    {
        //        CapturedTransactions capturedTrxn = CapturedTransactions.LoadCapturedTransactionsByPk(int.Parse(e.Parameters[0]));
        //        if (capturedTrxn != null)
        //        {
        //            capturedTrxn.ExpirationTime = DateTime.Now;
        //            capturedTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
        //            capturedTrxn.Comments = e.Parameters[1];
        //            capturedTrxn.TrxnStatus = e.Parameters[2];

        //            //editing done by shariq 01Sep2015***********
        //            if (e.Parameters[3] != "")
        //                capturedTrxn.Amount = Convert.ToDecimal(e.Parameters[3]);
                    
        //            //***********
                  
        //            if (capturedTrxn.EjParsedBnaTransactionsId != null)
        //            {
        //                objEjParsedBNA = EjParsedBnaTransaction.LoadEjParsedBnaTransactionByPk(Convert.ToInt32(capturedTrxn.EjParsedBnaTransactionsId));
        //                transactionTypeId = objEjParsedBNA.TransactionTypeId;
        //                atmID = objEjParsedBNA.AtmId;
        //                PAN = objEjParsedBNA.Pan;
        //                accountNo = objEjParsedBNA.AccountNo;
        //                tsn = objEjParsedBNA.Seq;
        //            }

        //            else if (capturedTrxn.EjParsedCpmTransactionsId != null)
        //            {
        //                objEjParsedCPM = EjParsedCpmTransaction.LoadEjParsedCpmTransactionByPk(Convert.ToInt32(capturedTrxn.EjParsedCpmTransactionsId));
        //                transactionTypeId = objEjParsedCPM.TransactionTypeId;
        //                atmID = objEjParsedCPM.AtmId;
        //                PAN = objEjParsedCPM.Pan;
        //                accountNo = objEjParsedCPM.AccountNo;
        //                tsn = objEjParsedCPM.Seq;
        //            }
        //            else if (capturedTrxn.EjParsedTransactionsId != null)
        //            {
        //                objEjParsed = EjParsedTransactions.LoadEjParsedTransactionsByPk(Convert.ToInt32(capturedTrxn.EjParsedTransactionsId));
        //                transactionTypeId = objEjParsed.TransactionTypeId;
        //                atmID = objEjParsed.AtmId.Value;

        //                PAN = objEjParsed.Pan;
        //                //accountNo = objEjParsed.AccountNo;
        //                tsn = objEjParsed.Tsn;
        //            }
        //            capturedTrxn.Save();
        //            SmsTaskHelper.CheckConfiguration(transactionTypeId, null, capturedTrxn.CapturedTransactionsId, null, null,capturedTrxn.TrxnStatus,atmID,capturedTrxn.UserId.Value,PAN,accountNo,tsn,capturedTrxn.TaskId.Value, null);
        //        }
        //        e.Output.Write("<script> window.location='DashBoardGrid_EJFraudDetected.aspx';</script>");
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
        //    }

        //    finally
        //    {
        //        task.EndTask();
        //    }
        //}

             //Added by Ali Shah on 28th Sep, 2015
        //To provide feature of Customer Detail Button

        protected void CallBack1_Callback(object sender, CallBackEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
            EjParsedBnaTransaction objEjParsedBNA = null;
            EjParsedCpmTransaction objEjParsedCPM = null;
            EjParsedTransactions objEjParsed = null;
            string bankName = null;
            int? transactionTypeId = null;
            int atmID = -1;
            string PAN = "";
            string accountNo = "";
            string tsn = "";

            try
            {
                CapturedTransactions capturedTrxn = CapturedTransactions.LoadCapturedTransactionsByPk(int.Parse(e.Parameters[1]));
                if (capturedTrxn != null)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Close: Captured Transaction Found");
                    if (!capturedTrxn.IsLocked.HasValue)
                        capturedTrxn.IsLocked = false;


                    //e.Parameters[6] --> Unlock on Cancel Case
                    if (Convert.ToBoolean(e.Parameters[0]) == true && Convert.ToBoolean(e.Parameters[6]) == true)
                    {
                        if (capturedTrxn.IsLocked.Value && capturedTrxn.UserId == ((AppUser)Session[SessionVars.user.ToString()]).UserId)
                        {
                            capturedTrxn.LockedDatetime = null;
                            capturedTrxn.IsLocked = false;
                            capturedTrxn.Save();
                            return;
                        }
                    }

                    if (capturedTrxn.UserId == null || capturedTrxn.UserId == ((AppUser)Session[SessionVars.user.ToString()]).UserId)
                    {
                        if (Convert.ToBoolean(e.Parameters[0]) == true)
                        {
                            capturedTrxn.ExpirationTime = DateTime.Now;
                            capturedTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                            capturedTrxn.Comments = e.Parameters[2];
                            capturedTrxn.TrxnStatus = e.Parameters[3];

                            //editing done by shariq 01Sep2015***********
                            if (e.Parameters[4] != "")
                                capturedTrxn.AmountClaimed = Convert.ToDecimal(e.Parameters[4]);

                            if (e.Parameters[5] != "")
                                capturedTrxn.AmountCredited = Convert.ToDecimal(e.Parameters[5]);
                        }
                    }
                    if ((capturedTrxn.IsLocked.HasValue && !capturedTrxn.IsLocked.Value) || capturedTrxn.UserId == ((AppUser)Session[SessionVars.user.ToString()]).UserId)
                    {
                        if (capturedTrxn.EjParsedBnaTransactionsId != null)
                        {
                            objEjParsedBNA = EjParsedBnaTransaction.LoadEjParsedBnaTransactionByPk(Convert.ToInt32(capturedTrxn.EjParsedBnaTransactionsId));
                            transactionTypeId = objEjParsedBNA.TransactionTypeId;
                            atmID = objEjParsedBNA.AtmId;
                            PAN = objEjParsedBNA.Pan;
                            accountNo = objEjParsedBNA.AccountNo;
                            tsn = objEjParsedBNA.Seq;
                            bankName = objEjParsedBNA.BankName;
                        }

                        else if (capturedTrxn.EjParsedCpmTransactionsId != null)
                        {
                            objEjParsedCPM = EjParsedCpmTransaction.LoadEjParsedCpmTransactionByPk(Convert.ToInt32(capturedTrxn.EjParsedCpmTransactionsId));
                            transactionTypeId = objEjParsedCPM.TransactionTypeId;
                            atmID = objEjParsedCPM.AtmId;
                            PAN = objEjParsedCPM.Pan;
                            accountNo = objEjParsedCPM.AccountNo;
                            tsn = objEjParsedCPM.Seq;
                            bankName = objEjParsedCPM.BankName;
                        }
                        else if (capturedTrxn.EjParsedTransactionsId != null)
                        {
                            objEjParsed = EjParsedTransactions.LoadEjParsedTransactionsByPk(Convert.ToInt32(capturedTrxn.EjParsedTransactionsId));
                            transactionTypeId = objEjParsed.TransactionTypeId;
                            atmID = objEjParsed.AtmId.Value;

                            PAN = objEjParsed.Pan;
                            //accountNo = objEjParsed.AccountNo;
                            tsn = objEjParsed.Tsn;
                            bankName = objEjParsed.BankName;
                        }
                        else if (capturedTrxn.EjCapturedCardId != null)
                        {
                            transactionTypeId = 5;
                        }
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Close: Transaction Transaction Found");
                    }
                    else//IsLocked is True
                    {
                        AppUser screenLockedBy = AppUser.LoadAppUserByPk(capturedTrxn.UserId.Value);
                        //e.Output.Write("<script> receiveCallBackExceptionFormResult('<div border=1 width=100%>The transaction is locked. Please contact your administrator.</div>');</script>");
                        e.Output.Write("<script> receiveCallBackExceptionFormResult('<div border=1 width=100%>The transaction is locked because it is updating by " + screenLockedBy.UserLogin + ".</div>');</script>");
                    }


                    //if (Convert.ToBoolean(e.Parameters[0]) == false && (capturedTrxn.IsLocked.HasValue && !capturedTrxn.IsLocked.Value))
                    if (Convert.ToBoolean(e.Parameters[0]) == false && ((capturedTrxn.IsLocked.HasValue && !capturedTrxn.IsLocked.Value) || capturedTrxn.UserId == ((AppUser)Session[SessionVars.user.ToString()]).UserId))
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Close: Creating Form");
                        capturedTrxn.IsLocked = true;
                        capturedTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        capturedTrxn.LockedDatetime = DateTime.Now;
                        capturedTrxn.Save();

                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                        //cmd.CommandText = "select action_code from sms_transaction_type_detail where transaction_type_id = " + transactionTypeId + " and action_code<> '' order by sms_transaction_type_detail_id";
                        cmd.CommandText = "select action_code from sms_transaction_type_detail where transaction_type_id = " + transactionTypeId + " and action_code<> '' and screen_name = 'DashBoardGrid_EJFraudDetected' order by sms_transaction_type_detail_id";

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();

                        adapter.Fill(dt);

                        AppUser appUser = AppUser.LoadAppUserByPk(((AppUser)Session[SessionVars.user.ToString()]).UserId);

                        StringBuilder builder = new StringBuilder();
                        builder.Append("<tr><td style=\"border:0;\">Status:</td><td style=\"border:0;\"> <select id=\"ddlStatus\" style=\"visibility: visible;\">");
                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            builder.Append("<option value=\"" + i + "\">" + dr["action_code"].ToString() + "</option>");
                            i++;
                        }
                        builder.Append("</select></td></tr>");
                        builder.Append("<tr id=\"AmountRow\"><td style=\"border:0;\">Modified By:</td><td style=\"border:0;\"><label id=\"txtModifiedBy\">" + appUser.UserLogin + "</label></td></tr>");
                        builder.Append("<tr id=\"AmountRow\"><td style=\"border:0;\">Amount Claimed:</td><td style=\"border:0;\"><input id=\"txtAmountClaimed\" type=\"text\"/></td></tr>");
                        builder.Append("<tr id=\"AmountRow\"><td style=\"border:0;\">Amount Credited:</td><td style=\"border:0;\"><input id=\"txtAmountCredited\" type=\"text\" disabled/></td></tr>");
                        builder.Append("<tr><td style=\"border:0;\"> Please enter Comments to close this issue.:</td><td style=\"border:0;\"> <textarea rows=\"10\" cols=\"50\" id=\"txtUsrComment\"></textarea></td></tr>");

                        builder.Append("<tr><td style=\"border:0;\"><input type=\"button\" value=\"Save\" onclick=\"onCloseOfDialog();\" class=\"button\"></td>");
                        builder.Append("<td style=\"border:0;\"><input type=\"button\" value=\"Close\" class=\"button\" onclick=\"onClose();\"/></td></tr>");
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Close: Form Created");
                        e.Output.Write("<script> receiveCallBackExceptionFormResult('<table border=1 width=100%><tbody>" + builder.ToString() + "</tbody></table>');</script>");
                    }
                    else
                    {
                        if (capturedTrxn.IsLocked.Value && capturedTrxn.UserId != ((AppUser)Session[SessionVars.user.ToString()]).UserId)
                            return;
                        capturedTrxn.LockedDatetime = null;
                        capturedTrxn.IsLocked = false;
                        capturedTrxn.Save();
                        SmsTaskHelper.CheckConfiguration(transactionTypeId, null, capturedTrxn.CapturedTransactionsId, null, null, capturedTrxn.TrxnStatus, atmID, capturedTrxn.UserId.Value, PAN, accountNo, tsn, capturedTrxn.TaskId.Value, null, bankName);
                        //****
                        //SmsTaskHelper.CheckConfiguration(transactionTypeId, null, capturedTrxn.CapturedTransactionsId, null, null,capturedTrxn.TrxnStatus, null);

                    }
                }
                if (Convert.ToBoolean(e.Parameters[0]) == true)
                    e.Output.Write("<script> window.location='DashBoardGrid_EJFraudDetected.aspx';</script>");
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
        
        protected void DialogCallBackCustomerDetail_Callback(object sender, CallBackEventArgs e)
        {
            //SmsCustomerDetail customer = SmsCustomerDetail.LoadSmsCustomerDetailByPk(int.Parse(capturedTrxnID));
            StringBuilder builder = new StringBuilder();
            try
            {
                SmsTask smsTask = SmsTask.LoadSmsTask("captured_transaction_id = " + e.Parameters[0]);
                if (smsTask != null)
                {

                    SmsCustomerDetail smsCustomerDetail = SmsCustomerDetail.LoadSmsCustomerDetail("sms_task_id = " + smsTask.SmsTaskId);
                    if (smsCustomerDetail != null)
                    {
                        
                        builder.Append("<tr><td>Customer Name:</td><td>" + smsCustomerDetail.CustomerName + "</td></tr>");
                        builder.Append("<tr><td>Customer Telephone:</td><td>" + smsCustomerDetail.CustomerTelephone + "</td></tr>");
                        builder.Append("<tr><td>Customer Mobile No.:</td><td>" + smsCustomerDetail.CustomerMobileNo + "</td></tr>");
                        builder.Append("<tr><td>Customer Email:</td><td>" + smsCustomerDetail.CustomerEmail + "</td></tr>");
                        builder.Append("<tr><td>Customer Category:</td><td>" + smsCustomerDetail.CustomerCategory + "</td></tr>");
                        builder.Append("<tr><td>Available Balance:</td><td>" + smsCustomerDetail.AvailableBalance + "</td></tr>");
                        builder.Append("<tr><td>RM Name:</td><td>" + smsCustomerDetail.RmName + "</td></tr>");
                        builder.Append("<tr><td>RM Contact:</td><td>" + smsCustomerDetail.RmContact + "</td></tr>");
                        builder.Append("<tr><td>RM Email:</td><td>" + smsCustomerDetail.RmEmail + "</td></tr>");

                        builder.Append("<tr><td>TSN:</td><td>" + smsTask.Tsn + "</td></tr>");
                        builder.Append("<tr><td>ATM ID:</td><td>" + Atm.LoadAtmByPk(smsTask.AtmId).Title + "</td></tr>");



                        e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>');</script>");
                    }
                }
            }
            catch(Exception ex)
            {
                LogableTask.LogMonoActivityTask("ViewInquiry", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            
        
        }
        private void GenerateLockedDialog(object sender, CallBackEventArgs e)
        {

            e.Output.Write("<script> receiveCallBackExceptionFormResult('<div border=1 width=100%>The transaction is locked. Please contact your administrator.</div>');</script>");
        }
    }

}