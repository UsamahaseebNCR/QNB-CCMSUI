using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using ComponentArt.Web.UI;
using Avanza.CCMS.DAL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using Avanza.iSuite.DAL;
//using Avanza.iSuite.DAL;

namespace CCMSUI.CCMS
{
    public partial class ViewEJBNATransactions : BasePage
    {
        protected void GridView_Trxn_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "viewDetails")
            {
                Session["ej"] = e.CommandArgument.ToString();
                Session["d"] = "1";
                Literal_error.Text = "<script>window.open('TransactionComparison.aspx','','location=0,status=1,resizable=yes,scrollbars=yes')</script>";
            }
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewPNCBNATransactions, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["SortExpression"] = "trxn_datetime";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                ///<Summary>
                ///Edited by Ali Shah on 16th Sep, 2018
                ///CR: To Add Filter to filter by Approval Status
                //TransactionType.TransactionTypeReader trxnTypeReader = Avanza.CCMS.DAL.TransactionType.ExecuteReader("transaction_type_id>0");

                //while (trxnTypeReader.Read())
                //{
                //    DropDownList_TrxnType.Items.Add(new ListItem(trxnTypeReader.CurrentTransactionType.TransactionTypeName,
                //                                                trxnTypeReader.CurrentTransactionType.TransactionTypeId.ToString()));

                //}
                //DropDownList_TrxnType.DataTextField = "transaction_type_name";
                //DropDownList_TrxnType.DataValueField = "transaction_type_id";
                //DropDownList_TrxnType.DataBind();

                //trxnTypeReader.Close();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                //cmd.CommandText = "select sms_transaction_type_detail_id, action_code from sms_transaction_type_detail where transaction_type_id = 17 and action_code<> '' order by sms_transaction_type_detail_id";
                cmd.CommandText = "select distinct action_code from sms_transaction_type_detail where transaction_type_id in (select transaction_type_id from transaction_type where transaction_type_name like '%Cash%') and action_code<> ''";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    ddl_ApprovalStatus.Items.Add(new ListItem(dr["action_code"].ToString()));
                }

                if (System.Configuration.ConfigurationManager.AppSettings["isRunningForPNC"] == "0")
                {
                    GridView_Trxn.Columns[8].Visible = false;
                    // GridView_Trxn.Columns[11].Visible = false;
                    GridView_Trxn.Columns[15].Visible = false;
                    GridView_Trxn.Columns[16].Visible = false;
                    GridView_Trxn.Columns[17].Visible = false;
                    GridView_Trxn.Columns[18].Visible = false;
                    //GridView_Trxn.Columns[14].HeaderText = "Response";
                    //GridView_Trxn.Columns[15].HeaderText = "View Trxn";
                    tdAnalysisOn.Visible = false;
                    tdAnalysisOnCheckBoxList.Visible = false;
                }
                if (System.Configuration.ConfigurationManager.AppSettings["isHostTsnEnable"] == "false")
                {
                    GridView_Trxn.Columns[10].Visible = false;
                }
                //else if (System.Configuration.ConfigurationManager.AppSettings["bankID"] == "1")
                //{
                //    GridView_Trxn.Columns[7].Visible = false;
                //    GridView_Trxn.Columns[8].Visible = false;
                //    GridView_Trxn.Columns[15].Visible = false;
                //}
                //Edited By Muhammad Shaharyar Parekh
                if (System.Configuration.ConfigurationManager.AppSettings["IsBankNameEnable"] == "true")
                {
                    GridView_Trxn.Columns[1].Visible = true;
                    bank_name.Visible = true;
                }

                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
        }

        protected void GridView_Trxn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Trxn.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            //BindGridWithCache();
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
            //if (ViewState["SortExpression"].ToString() == "creation_time")
            //    ViewState["SortExpression"] = "f.creation_time";

            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            //BindGridWithCache();
        }

        void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = GetQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);
                Literal_count.Text = dtTrxn.Rows.Count + " records.";
                this.GridView_Trxn.DataSource = dtTrxn;
                this.GridView_Trxn.DataBind();
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_FromAmount.Text = "";
            TextBox_ToAmount.Text = "";
            TextBox_TSN.Text = "";
            TextBox_PAN.Text = "";
        }

        string GetFilter()
        {
            string temp = null;
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            if (TextBox_FromAmount.Text.Length > 0)
                filter += " and amount_authorized >= " + int.Parse(TextBox_FromAmount.Text);

            if (TextBox_ToAmount.Text.Length > 0)
                filter += " and amount_authorized <= " + int.Parse(TextBox_ToAmount.Text);

            if (TextBox_TSN.Text.Length > 0)
                filter += " and vEjParsedBNATransaction.seq = '" + TextBox_TSN.Text + "'";

            if (TextBox_HostTSN.Text.Length > 0)
                filter += " and vEjParsedBNATransaction.host_tsn = '" + TextBox_HostTSN.Text + "'";

            if (TextBox_PAN.Text.Length > 0)
                filter += " and vEjParsedBNATransaction.pan like '%" + TextBox_PAN.Text + "%'";

            if (TextBox_Response.Text.Length > 0)
                filter += " and comment like '%" + TextBox_Response.Text + "%'";

            if (TextBox_bankName.Text.Length > 0)
                filter += " and vEjParsedBNATransaction.bank_name like '%" + TextBox_bankName.Text + "%'";

            if (CheckBoxList_DisputeStatus.Items[0].Selected && CheckBoxList_DisputeStatus.Items[1].Selected)
            {
            }
            else if (CheckBoxList_DisputeStatus.Items[0].Selected)
            {
                filter += " and dispute_status is not null ";
            }
            else if (CheckBoxList_DisputeStatus.Items[1].Selected)
            {
                filter += " and dispute_status is null ";
            }

            if (CheckBoxList_Status.Items[0].Selected)
                temp += "'Successful',";
            if (CheckBoxList_Status.Items[1].Selected)
                temp += "'Failed',";
            if (CheckBoxList_Status.Items[2].Selected)
                temp += "'Suspicious',";

            if (temp != null)
            {
                filter += " and vEjParsedBNATransaction.status in (" + temp + "'-1')";
            }

            if (CheckBoxList_IsDisputed.Items[0].Selected)
                filter += " and is_disputed_transaction = 1";

            //if (DropDownList_Comment.SelectedIndex>0)
            //    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";
            //edit by shariq for the purpose of customer search
            //if (TextBox_referenceNo.Text.Length > 0 || TextBox_customerMobile.Text.Length > 0 || TextBox_customerID.Text.Length > 0 || TextBox_customerTelephone.Text.Length > 0)
            //{
            //    filter += "and  refrence_no like '%" + TextBox_referenceNo.Text + "%' and customer_id like '%" + TextBox_customerID.Text + "%' and customer_telephone like '%" + TextBox_customerTelephone.Text + "%' and customer_mobile_no like '%" + TextBox_customerMobile.Text + "%'";        
            //}

            if (TextBox_referenceNo.Text.Length > 0)
                filter += " and  refrence_no like '%" + TextBox_referenceNo.Text + "%'";

            if (TextBox_customerTelephone.Text.Length > 0)
                filter += " and  customer_telephone like '%" + TextBox_customerTelephone.Text + "%'";

            if (TextBox_customerMobile.Text.Length > 0)
                filter += " and  customer_mobile_no like '%" + TextBox_customerMobile.Text + "%'";

            if (TextBox_customerID.Text.Length > 0)
                filter += " and  sms_customer_detail.customer_id like '%" + TextBox_customerID.Text + "%'";

            if (TextBox_AccountNo.Text.Length > 0)
                filter += " and  vEjParsedBNATransaction.account_no like '%" + TextBox_AccountNo.Text + "%'";

            ///<Summary>
            ///Edited by Ali Shah on 16th Sep, 2018
            ///
            if (ddl_ApprovalStatus.SelectedIndex > 0)
                filter += " and captured_transactions.trxn_status = '" + ddl_ApprovalStatus.SelectedItem.Text + "'";

            //and customer_id like '%" + TextBox_customerID.Text + "%' and customer_telephone like '%" + TextBox_customerTelephone.Text + "%' and customer_mobile_no like '%" + TextBox_customerMobile.Text + "%'";

            return filter;
        }

        protected void button_search_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateInputs();
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                if (ex.Message.ToLower().Contains("invalid"))
                    throw;
            }
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }

        protected void DialogCallBackCustomerDetail_Callback(object sender, CallBackEventArgs e)
        {
            //SmsCustomerDetail customer = SmsCustomerDetail.LoadSmsCustomerDetailByPk(int.Parse(capturedTrxnID));
            StringBuilder builder = new StringBuilder();
            try
            {
                Avanza.CCMS.DAL.SmsCustomerDetail customer = Avanza.CCMS.DAL.SmsCustomerDetail.LoadSmsCustomerDetailByPk(e.Parameter[0]);

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @" select bnatransaction.seq,smstask.refrence_no,customerdetail.customer_name,customerdetail.customer_telephone,customerdetail.customer_mobile_no
                                                    ,customerdetail.customer_id from ej_parsed_bna_transaction bnatransaction inner join captured_transactions capturetransaction on capturetransaction.ej_parsed_bna_transactions_id = bnatransaction.ej_parsed_bna_transaction_id 
                                                    inner join sms_task smstask on smstask.captured_transaction_id = capturetransaction.captured_transactions_id
                                                    inner join sms_customer_detail customerdetail on smstask.sms_task_id = customerdetail.sms_task_id where bnatransaction.seq = '" + e.Parameter[0] + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmd.Connection.Close();
                builder.Append("<tr><td>Customer Name:</td><td>" + customer.CustomerName + "</td></tr>");
                builder.Append("<tr><td>Customer Telephone:</td><td>" + customer.CustomerTelephone + "</td></tr>");
                builder.Append("<tr><td>Customer Mobile No.:</td><td>" + customer.CustomerMobileNo + "</td></tr>");
                builder.Append("<tr><td>Customer Email:</td><td>" + customer.CustomerEmail + "</td></tr>");
                builder.Append("<tr><td>Customer Category:</td><td>" + customer.CustomerCategory + "</td></tr>");
                builder.Append("<tr><td>Available Balance:</td><td>" + customer.AvailableBalance + "</td></tr>");
                builder.Append("<tr><td>RM Name:</td><td>" + customer.RmName + "</td></tr>");
                builder.Append("<tr><td>RM Contact:</td><td>" + customer.RmContact + "</td></tr>");
                builder.Append("<tr><td>RM Email:</td><td>" + customer.RmEmail + "</td></tr>");
                e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>');</script>");
            }
            catch (Exception ex)
            {
            }
        }

        protected void GridView_Trxn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem).Row["sms_customer_detail_id"].ToString().Length == 0)
                    e.Row.FindControl("anchorInquiry").Visible = false;
                else
                    ((ImageButton)e.Row.FindControl("anchorInquiry")).Attributes.Add("onclick", "javascript:window.open('ViewCustomerDetail.aspx?id=" + ((DataRowView)e.Row.DataItem).Row["ej_parsed_bna_transaction_id"] + "')");

                if (((DataRowView)e.Row.DataItem).Row["captured_transactions_id"].ToString().Length == 0)
                    e.Row.FindControl("anchorClose").Visible = false;
                else
                    ((ImageButton)e.Row.FindControl("anchorClose")).Attributes.Add("onclick", "javascript:openDialog(" + ((DataRowView)e.Row.DataItem).Row["captured_transactions_id"] + "," + ((DataRowView)e.Row.DataItem).Row["transaction_rule_id"] + ");return false;");

            }


        }

        public void ValidateInputs()
        {
            bool isPANContainSpecialChars = CheckSpecialChars(TextBox_PAN.Text);
            if (isPANContainSpecialChars == true)
            {
                throw new Exception("PAN contain invalid character");
            }

            bool isTSNContainSpecialChars = CheckSpecialChars(TextBox_TSN.Text);
            if (isTSNContainSpecialChars == true)
            {
                throw new Exception("TSN contain invalid character");
            }

            bool isFromAmountContainSpecialChars = CheckSpecialChars(TextBox_FromAmount.Text);
            if (isFromAmountContainSpecialChars == true)
            {
                throw new Exception("From Amount contain invalid character");
            }

            bool isToAmountContainSpecialChars = CheckSpecialChars(TextBox_ToAmount.Text);
            if (isToAmountContainSpecialChars == true)
            {
                throw new Exception("To Amount contain invalid character");
            }

            bool isResponseContainSpecialChars = CheckSpecialChars(TextBox_Response.Text);
            if (isResponseContainSpecialChars == true)
            {
                throw new Exception("Response contain invalid character");

            }
            bool isAccountNoContainSpecialChars = CheckSpecialChars(TextBox_AccountNo.Text);
            if (isAccountNoContainSpecialChars == true)
            {
                throw new Exception("Account Number contain invalid character");
            }

            bool isReferenceNoContainSpecialChars = CheckSpecialChars(TextBox_referenceNo.Text);
            if (isReferenceNoContainSpecialChars == true)
            {
                throw new Exception("Reference Number contain invalid character");
            }

            bool isCustomerIDContainSpecialChars = CheckSpecialChars(TextBox_customerID.Text);
            if (isCustomerIDContainSpecialChars == true)
            {
                throw new Exception("Customer ID contain invalid character");
            }

            bool isCustomerMobileContainSpecialChars = CheckSpecialChars(TextBox_customerMobile.Text);
            if (isCustomerMobileContainSpecialChars == true)
            {
                throw new Exception("Customer Mobile Number contain invalid character");
            }

            bool isCustomerTelephoneContainSpecialChars = CheckSpecialChars(TextBox_customerTelephone.Text);
            if (isCustomerTelephoneContainSpecialChars == true)
            {
                throw new Exception("Customer Telephone contain invalid character");
            }

            bool isHOSTTSNContainSpecialChars = CheckSpecialChars(TextBox_HostTSN.Text);
            if (isHOSTTSNContainSpecialChars == true)
            {
                throw new Exception("Host TSN contain invalid character");
            }
        }


        protected void Button_ExportToExcel_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToExcel_Click");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = GetQuery();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\EjBnaTransactions.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "EJ Cash Deposit";
                rptDoc.SetParameterValue("FromDate", DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null));
                rptDoc.SetParameterValue("ToDate", DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null));
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dtTrxn.Rows.Count);
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\EjTransactions_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
                rptDoc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.ExcelRecord, path);
                rptDoc.Close();
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(path));
                Response.AddHeader("Content-Length", new System.IO.FileInfo(path).Length.ToString());
                Response.BufferOutput = false;
                Response.WriteFile(path);
                Response.Flush();
                Response.Close();
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

        protected void Button_ExportToPDF_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToPDF_Click");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = GetQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\EjBnaTransactions.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "EJ Cash Deposit";
                rptDoc.SetParameterValue("FromDate", DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null));
                rptDoc.SetParameterValue("ToDate", DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null));
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dtTrxn.Rows.Count);
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\EjTransactions_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";
                rptDoc.ExportToDisk(ExportFormatType.PortableDocFormat, path);
                rptDoc.Close();
                Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(path));
                Response.WriteFile(path);
                Response.Flush();
                Response.Close();
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


        protected void CallBack1_Callback(object sender, CallBackEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
            EjParsedBnaTransaction objEjParsedBNA = null;
            EjParsedCpmTransaction objEjParsedCPM = null;
            EjParsedTransactions objEjParsed = null;
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
                    if (!capturedTrxn.IsLocked.HasValue)
                        capturedTrxn.IsLocked = false;

                    //e.Parameters[7] --> Unlock on Cancel Case
                    if (Convert.ToBoolean(e.Parameters[0]) == true && Convert.ToBoolean(e.Parameters[7]) == true)
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

                            capturedTrxn.InternalTeamComment = e.Parameters[6];
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
                        }

                        else if (capturedTrxn.EjParsedCpmTransactionsId != null)
                        {
                            objEjParsedCPM = EjParsedCpmTransaction.LoadEjParsedCpmTransactionByPk(Convert.ToInt32(capturedTrxn.EjParsedCpmTransactionsId));
                            transactionTypeId = objEjParsedCPM.TransactionTypeId;
                            atmID = objEjParsedCPM.AtmId;
                            PAN = objEjParsedCPM.Pan;
                            accountNo = objEjParsedCPM.AccountNo;
                            tsn = objEjParsedCPM.Seq;
                        }
                        else if (capturedTrxn.EjParsedTransactionsId != null)
                        {
                            objEjParsed = EjParsedTransactions.LoadEjParsedTransactionsByPk(Convert.ToInt32(capturedTrxn.EjParsedTransactionsId));
                            transactionTypeId = objEjParsed.TransactionTypeId;
                            atmID = objEjParsed.AtmId.Value;

                            PAN = objEjParsed.Pan;
                            //accountNo = objEjParsed.AccountNo;
                            tsn = objEjParsed.Tsn;
                        }
                        else if (capturedTrxn.EjCapturedCardId != null)
                        {
                            transactionTypeId = 5;
                        }

                    }
                    else//IsLocked is True
                    {
                        AppUser screenLockedBy = AppUser.LoadAppUserByPk(capturedTrxn.UserId.Value);
                        //e.Output.Write("<script> receiveCallBackExceptionFormResult('<div border=1 width=100%>The transaction is locked. Please contact your administrator.</div>');</script>");
                        e.Output.Write("<script> receiveCallBackExceptionFormResult('<div border=1 width=100%>The transaction is locked because it is updating by " + screenLockedBy.UserLogin + ".</div>');</script>");
                    }

                    //if(Convert.ToBoolean(e.Parameters[0]) == false && (capturedTrxn.IsLocked.HasValue && !capturedTrxn.IsLocked.Value))
                    //if (Convert.ToBoolean(e.Parameters[0]) == false)
                    if (Convert.ToBoolean(e.Parameters[0]) == false && ((capturedTrxn.IsLocked.HasValue && !capturedTrxn.IsLocked.Value) || capturedTrxn.UserId == ((AppUser)Session[SessionVars.user.ToString()]).UserId))
                    {
                        //if(capturedTrxn.IsLocked.HasValue && !capturedTrxn.IsLocked.Value)
                        {
                            capturedTrxn.IsLocked = true;
                            capturedTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                            capturedTrxn.LockedDatetime = DateTime.Now;
                            capturedTrxn.Save();

                            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                            //cmd.CommandText = "select action_code from sms_transaction_type_detail where transaction_type_id = " + transactionTypeId + " and action_code<> '' order by sms_transaction_type_detail_id";
                            cmd.CommandText = "select action_code from sms_transaction_type_detail where action_code<> '' and screen_name = 'ViewEJBNATransactions' order by sms_transaction_type_detail_id";

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
                            //builder.Append("<tr id=\"AmountRow\"><td style=\"border:0;\">Modified By:</td><td style=\"border:0;\"><input id=\"txtModifiedBy\" value=\"" + appUser.UserLogin +"\" type=\"text\" disabled/></td></tr>");
                            builder.Append("<tr id=\"AmountRow\"><td style=\"border:0;\">Modified By:</td><td style=\"border:0;\"><label id=\"txtModifiedBy\">" + appUser.UserLogin + "</label></td></tr>");
                            builder.Append("<tr id=\"AmountRow\"><td style=\"border:0;\">Amount Claimed:</td><td style=\"border:0;\"><input id=\"txtAmountClaimed\" type=\"text\" value=\"" + capturedTrxn.AmountClaimed.ToString() + "\" disabled/></td></tr>");
                            builder.Append("<tr id=\"AmountRow\"><td style=\"border:0;\">Amount Credited:</td><td style=\"border:0;\"><input id=\"txtAmountCredited\" type=\"text\" value=\"" + capturedTrxn.AmountCredited.ToString() + "\"/></td></tr>");
                            builder.Append("<tr><td style=\"border:0;\"> Closure Comment:</td><td style=\"border:0;\"> <textarea rows=\"10\" cols=\"50\" id=\"txtUsrComment\" readonly>" + capturedTrxn.Comments + "</textarea></td></tr>");
                            builder.Append("<tr><td style=\"border:0;\"> Internal Team Comment:</td><td style=\"border:0;\"> <textarea rows=\"10\" cols=\"50\" id=\"txtInternalComment\">" + capturedTrxn.InternalTeamComment + "</textarea></td></tr>");
                            builder.Append("<tr><td style=\"border:0;\"><input type=\"button\" value=\"Save\" onclick=\"onCloseOfDialog();\" class=\"button\"></td>");
                            builder.Append("<td style=\"border:0;\"><input type=\"button\" value=\"Close\" class=\"button\" onclick=\"onClose();\"/></td></tr>");

                            e.Output.Write("<script> receiveCallBackExceptionFormResult('<table border=1 width=100%><tbody>" + builder.ToString() + "</tbody></table>');</script>");
                        }
                    }
                    else
                    {
                        if (capturedTrxn.IsLocked.Value && capturedTrxn.UserId != ((AppUser)Session[SessionVars.user.ToString()]).UserId)
                            return;
                        capturedTrxn.LockedDatetime = null;
                        capturedTrxn.IsLocked = false;
                        capturedTrxn.Save();
                        SmsTaskHelper.CheckConfiguration(transactionTypeId, null, capturedTrxn.CapturedTransactionsId, null, null, capturedTrxn.TrxnStatus, atmID, capturedTrxn.UserId.Value, PAN, accountNo, tsn, capturedTrxn.TaskId.Value, null);
                        //****
                        //SmsTaskHelper.CheckConfiguration(transactionTypeId, null, capturedTrxn.CapturedTransactionsId, null, null,capturedTrxn.TrxnStatus, null);

                        #region CommentAuditLogs //Audit Logs entered by internal team
                        CommentAuditLogs commentAuditLogs = new CommentAuditLogs();

                        commentAuditLogs.CommentText = capturedTrxn.InternalTeamComment;
                        commentAuditLogs.CommentEnteredAt = DateTime.Now;
                        commentAuditLogs.UserId = capturedTrxn.UserId;
                        commentAuditLogs.CapturedTransactionsId = capturedTrxn.CapturedTransactionsId;
                        commentAuditLogs.TrxnStatus = capturedTrxn.TrxnStatus;

                        commentAuditLogs.Save();
                        #endregion
                    }
                }
                if (Convert.ToBoolean(e.Parameters[0]) == true)
                    e.Output.Write("<script> window.location='ViewEJBNATransactions.aspx';</script>");
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }

            finally
            {
                task.EndTask();
            }

        }

        private string GetQuery()
        {
            return @" 


SELECT title,trxn_datetime,seq,account_type,vEjParsedBNATransaction.pan,vEjParsedBNATransaction.amount_authorized,dispute_status,vEjParsedBNATransaction.status,comment,
 vEjParsedBNATransaction.ej_parsed_bna_transaction_Id,transaction_start_time,transaction_end_time,
card_taken_time,vEjParsedBNATransaction.account_no,host_tsn,posting_date,currency,refrence_no,customer_name,customer_telephone,customer_mobile_no,
sms_customer_detail_id,captured_transactions.trxn_status,captured_transactions.comments user_comments,captured_transactions.amount_claimed customer_claim_amount,transaction_type_name,sms_task.status alert_status,captured_transactions_id,captured_transactions.transaction_rule_id transaction_rule_id,transaction_rule_friendly_name error_type,
vEjParsedBNATransaction.eida_name,vEjParsedBNATransaction.eida_number,vEjParsedBNATransaction.eida_dob,vEjParsedBNATransaction.eida_nationality,vEjParsedBNATransaction.eida_expiry ,vEjParsedBNATransaction.mobile_number ,
detail = STUFF((
          SELECT '<br>' + CONVERT(varchar(max), T.Currency) + CONVERT(varchar(max), T.note_type) + ' * ' + CONVERT(varchar(max), T.notes_count) + ' = ' + CONVERT(varchar(max), T.note_type * T.notes_count) + ' '
          FROM ej_parsed_bna_transaction_detail T
          WHERE vEjParsedBNATransaction.ej_parsed_bna_transaction_id = T.ej_parsed_bna_transaction_id
          FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 4, ''),
		  total =  (
          SELECT sum(T.note_type * T.notes_count) 
          FROM ej_parsed_bna_transaction_detail T
          WHERE vEjParsedBNATransaction.ej_parsed_bna_transaction_id = T.ej_parsed_bna_transaction_id
          ),
		  curr = STUFF((
          SELECT '<br>' + CONVERT(varchar(max), T.Currency)  + ' '
          FROM ej_parsed_bna_transaction_detail T
          WHERE vEjParsedBNATransaction.ej_parsed_bna_transaction_id = T.ej_parsed_bna_transaction_id
          FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 4, ''),
(SELECT SUM(t.notes_count) FROM ej_parsed_bna_transaction_detail T
WHERE vEjParsedBNATransaction.ej_parsed_bna_transaction_id = T.ej_parsed_bna_transaction_id) deposit_count , vEjParsedBNATransaction.bank_name , captured_transactions.internal_team_comment
FROM vEjParsedBNATransaction 
INNER JOIN atm on vEjParsedBNATransaction.atm_id = atm.atm_id AND atm.is_active = 1 AND (is_cdm=1 or is_sdm = 1)
LEFT JOIN captured_transactions on captured_transactions.ej_parsed_bna_transactions_id =vEjParsedBNATransaction.ej_parsed_bna_transaction_id
LEFT JOIN sms_task on sms_task.captured_transaction_id = captured_transactions.captured_transactions_id  and sms_task.is_eligible=1
LEFT JOIN sms_customer_detail on sms_customer_detail.sms_task_id = sms_task.sms_task_id
LEFT JOIN transaction_type on vEjParsedBNATransaction.transaction_type_id = transaction_type.transaction_type_id
LEFT JOIN transaction_rule on captured_transactions.transaction_rule_id = transaction_rule.transaction_rule_id" +
                                     " WHERE   atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
                                     + " order by " + ViewState["orderby"];
        }
    }
}