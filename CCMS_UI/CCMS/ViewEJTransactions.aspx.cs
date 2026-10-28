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
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Web.Caching;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Text;
using ComponentArt.Web.UI;


namespace CCMSUI.CCMS
{
    public partial class ViewEJTransactions : BasePage
    {


        protected void GridView_Trxn_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "viewDetails")
            {
                Session["ej"] = e.CommandArgument.ToString();
                Session["d"] = null;
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
                if (Request.QueryString["q"] != null)
                    Label_heading.Text = "Dispute Management";
                else if (Request.QueryString["ej"] == "2")
                    Label_heading.Text = "Exception Handling (History)";

                Label_status.Visible = CheckBoxList_Status.Visible = Request.QueryString["q"] == null ? true : false;

                //if (!CheckRight((int)Permissions.TaskStatusMonitoringHistory, true))
                //    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["SortExpression"] = "trxn_datetime";
                ViewState["sortDirection"] = " desc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                //  BindGrid();
                Avanza.CCMS.DAL.TransactionType.TransactionTypeReader trxnTypeReader = Avanza.CCMS.DAL.TransactionType.ExecuteReader("transaction_type_id>0");
                while (trxnTypeReader.Read())
                {
                    DropDownList_TrxnType.Items.Add(new ListItem(trxnTypeReader.CurrentTransactionType.TransactionTypeName,
                                                                trxnTypeReader.CurrentTransactionType.TransactionTypeId.ToString()));

                }
                DropDownList_TrxnType.DataTextField = "transaction_type_name";
                DropDownList_TrxnType.DataValueField = "transaction_type_id";
                DropDownList_TrxnType.DataBind();

                trxnTypeReader.Close();
                Avanza.CCMS.DAL.Comment.CommentReader commentReader = Avanza.CCMS.DAL.Comment.ExecuteReader("comment_id>0");
                while (commentReader.Read())
                {
                    DropDownList_Comment.Items.Add(new ListItem(commentReader.CurrentComment.CommentText, commentReader.CurrentComment.CommentId.ToString()));
                }
                DropDownList_Comment.DataTextField = "comment_text";
                DropDownList_Comment.DataValueField = "comment_id";
                DropDownList_Comment.DataBind();
                commentReader.Close();

                if (System.Configuration.ConfigurationManager.AppSettings["bankID"] == "1")
                {
                    GridView_Trxn.Columns[15].Visible = false;
                    GridView_Trxn.Columns[16].Visible = false;
                    GridView_Trxn.Columns[17].Visible = false;
                    GridView_Trxn.Columns[18].Visible = false;
                }
                if (System.Configuration.ConfigurationManager.AppSettings["isHostTsnEnable"] == "false")
                {
                    GridView_Trxn.Columns[9].Visible = false;
                }
                //Edited By Muhammad Shaharyar Parekh
                if (System.Configuration.ConfigurationManager.AppSettings["IsBankNameEnable"] == "true")
                {
                    GridView_Trxn.Columns[1].Visible = true;
                    bank_name.Visible = true;
                }
            }
        }

        protected void GridView_Trxn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Trxn.PageIndex = e.NewPageIndex;
            BindGrid();
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
            BindGrid();
            //BindGridWithCache();
        }


        private void BindGridWithCache()
        {
            DataTable dtTrxn = (DataTable)Cache["dtTrxn"];
            dtTrxn.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
            this.GridView_Trxn.DataSource = dtTrxn;
            this.GridView_Trxn.DataBind();
            Literal_count.Text = dtTrxn.Rows.Count + " records.";

        }
        void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                ///<Summary>Edited by Ali Shah on 09th Oct, 2019
                ///To dynamically handling of cassettes more than 4
                EnableGridRespectiveColumns();

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = DateTime.Today;
                cmd.Parameters.Add("@Orderby", SqlDbType.VarChar);
                cmd.Parameters[2].Value = ViewState["orderby"];
                //edit by shariq on 21 oct    
                cmd.Parameters.Add("@conditions", SqlDbType.VarChar);
                cmd.Parameters[3].Value = GetFilter();
                cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                cmd.Parameters[4].Value = Session[SessionVars.SelectedATMs.ToString()];
                cmd.Parameters.Add("@type", SqlDbType.VarChar);
                cmd.Parameters[5].Value = Request.QueryString["ej"] == null ? "1" : Request.QueryString["ej"].ToString();


                cmd.CommandText = "GetViewParsedTransaction";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                StringBuilder builder = new StringBuilder();
                DataRow[] dataRow = dtTrxn.Select("mstate_id is not null and mstate_id <> 0");
                foreach (DataRow dr in dataRow)
                {
                    builder.Append(dr["mstate_id"] + ",");
                }
                builder.Append("-1");

                MState.MStateReader mStateReader = MState.ExecuteReader("mstate_id in (" + builder.ToString() + ")");
                while (mStateReader.Read())
                    DropDownList_MstateDesc.Items.Add(new ListItem(mStateReader.CurrentMState.MstateDesc, mStateReader.CurrentMState.MstateId.ToString()));

                //DropDownList_MstateDesc.DataBind();
                mStateReader.Close();
                Cache["dtTrxn"] = dtTrxn;
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
            TextBox_Dispensed1.Text = "";
            TextBox_Dispensed2.Text = "";
            TextBox_Dispensed3.Text = "";
            TextBox_Dispensed4.Text = "";
            TextBox_FromAmount.Text = "";
            TextBox_PAN.Text = "";
            TextBox_TSN.Text = "";
            TextBox_ToAmount.Text = "";
            DropDownList_TrxnType.SelectedIndex = 0;
            DropDownList_Comment.SelectedIndex = 0;
            DropDownList_MstateDesc.SelectedIndex = 0;
            foreach (ListItem item in CheckBoxList_Status.Items)
            {
                item.Selected = true;
            }


        }

        string GetFilter()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            if (TextBox_FromAmount.Text.Length > 0)
                filter += " and vEjParsedTransactions.amount >= " + int.Parse(TextBox_FromAmount.Text);

            if (TextBox_ToAmount.Text.Length > 0)
                filter += " and vEjParsedTransactions.amount <= " + int.Parse(TextBox_ToAmount.Text);

            if (TextBox_Dispensed1.Text.Length > 0)
                filter += " and notes_dispensed_type1 = " + int.Parse(TextBox_Dispensed1.Text);

            if (TextBox_Dispensed2.Text.Length > 0)
                filter += " and notes_dispensed_type2 = " + int.Parse(TextBox_Dispensed2.Text);

            if (TextBox_Dispensed3.Text.Length > 0)
                filter += " and notes_dispensed_type3 = " + int.Parse(TextBox_Dispensed3.Text);

            if (TextBox_Dispensed4.Text.Length > 0)
                filter += " and notes_dispensed_type4 = " + int.Parse(TextBox_Dispensed4.Text);

            if (TextBox_TSN.Text.Length > 0)
                filter += " and vEjParsedTransactions.tsn = '" + TextBox_TSN.Text + "'";
            //filter += " and tsn = " + int.Parse(TextBox_TSN.Text);

            if (TextBox_HostTSN.Text.Length > 0)
                filter += " and vEjParsedTransactions.host_tsn = '" + TextBox_HostTSN.Text + "'";

            if (TextBox_PAN.Text.Length > 0)
                filter += " and vEjParsedTransactions.pan like '%" + TextBox_PAN.Text + "%'";

            if (TextBox_bankName.Text.Length > 0)
                filter += " and vEjParsedTransactions.bank_name like '%" + TextBox_bankName.Text + "%'";

            if (CheckBoxList_Status.Visible)
            {
                StringBuilder builder = new StringBuilder();
                foreach (ListItem item in CheckBoxList_Status.Items)
                {
                    if (item.Selected)
                        builder.Append(item.Value + ",");
                }
                builder.Append("-1");
                //  if (CheckBoxList_Status.SelectedIndex > 0)
                filter += " and vEjParsedTransactions.status in (" + builder.ToString() + ")";
            }
            else
            {
                filter += " and vEjParsedTransactions.status in (1,2)";
            }

            if (DropDownList_TrxnType.SelectedIndex > 0)
                filter += " and transaction_type.transaction_type_id= " + DropDownList_TrxnType.SelectedItem.Value;

            if (DropDownList_Comment.SelectedIndex > 0)
                filter += " and comment.comment_id= " + DropDownList_Comment.SelectedItem.Value;

            if (DropDownList_MstateDesc.SelectedIndex > 0)
            {
                filter += " and vEjParsedTransactions.mstate_id = " + DropDownList_MstateDesc.SelectedItem.Value;
            }

            //Start Addition by NZT
            if (CheckBoxList_IsDisputed.Items[0].Selected)
                filter += " and is_disputed_transaction = 1";
            if (TextBox_referenceNo.Text.Length > 0)
                filter += " and  refrence_no like '%" + TextBox_referenceNo.Text + "%'";
            if (TextBox_customerID.Text.Length > 0)
                filter += " and  customer_id like '%" + TextBox_customerID.Text + "%'";
            if (TextBox_customerMobile.Text.Length > 0)
                filter += " and  customer_mobile_no like '%" + TextBox_customerMobile.Text + "%'";
            if (TextBox_customerTelephone.Text.Length > 0)
                filter += " and  customer_telephone like '%" + TextBox_customerTelephone.Text + "%'";
            //End Addition by NZT

            return filter;
        }
        protected void button_search_Click(object sender, EventArgs e)
        {
            try
            {
                if (TextBox_PAN.Text.Length > 0)
                    if (TextBox_PAN.Text.Length < 4)
                        throw new Exception("Please enter at least 4 characters for PAN search.");

                bool result = ValidateInputs();
                if (result == false)
                {
                    return;
                }
                else
                {
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }
        //Start Addition by NZT
        protected void GridView_Trxn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem).Row["sms_customer_detail_id"].ToString().Length == 0)
                    e.Row.FindControl("anchorInquiry").Visible = false;
                else
                    ((ImageButton)e.Row.FindControl("anchorInquiry")).Attributes.Add("onclick", "javascript:window.open('ViewCustomerDetail.aspx?id=" + ((DataRowView)e.Row.DataItem).Row["ej_parsed_transactions_id"] + "')");

                if (((DataRowView)e.Row.DataItem).Row["captured_transactions_id"].ToString().Length == 0)
                    e.Row.FindControl("anchorClose").Visible = false;
                else
                    ((ImageButton)e.Row.FindControl("anchorClose")).Attributes.Add("onclick", "javascript:openDialog(" + ((DataRowView)e.Row.DataItem).Row["captured_transactions_id"] + "," + ((DataRowView)e.Row.DataItem).Row["transaction_rule_id"] + ");return false;");

            }
        }
        //End Addition by NZT
        public bool ValidateInputs()
        {
            bool result = true;
            bool isPANContainSpecialChars = CheckSpecialChars(TextBox_PAN.Text);
            if (isPANContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;PAN contain invalid character</img> </li>";
                return result = false;
            }
            bool isTSNContainSpecialChars = CheckSpecialChars(TextBox_TSN.Text);
            if (isTSNContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;TSN contain invalid character</img> </li>";
                return result = false;
            }
            bool isFromAmountContainSpecialChars = CheckSpecialChars(TextBox_FromAmount.Text);
            if (isFromAmountContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;From Amount contain invalid character</img> </li>";
                return result = false;
            }
            bool isToAmountContainSpecialChars = CheckSpecialChars(TextBox_ToAmount.Text);
            if (isToAmountContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;To Amount contain invalid character</img> </li>";
                return result = false;
            }
            bool isCustomerIDContainSpecialChars = CheckSpecialChars(TextBox_customerID.Text);
            if (isCustomerIDContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Customer ID contain invalid character</img> </li>";
                return result = false;
            }
            bool isCustomerMobileContainSpecialChars = CheckSpecialChars(TextBox_customerMobile.Text);
            if (isCustomerMobileContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Customer Mobile Number contain invalid character</img> </li>";
                return result = false;
            }
            bool isCustomerTelephoneContainSpecialChars = CheckSpecialChars(TextBox_customerTelephone.Text);
            if (isCustomerTelephoneContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Customer Telephone contain invalid character</img> </li>";
                return result = false;
            }
            bool isRefrenceContainSpecialChars = CheckSpecialChars(TextBox_referenceNo.Text);
            if (isRefrenceContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Refrence Number contain invalid character</img> </li>";
                return result = false;
            }
            bool isHOSTTSNContainSpecialChars = CheckSpecialChars(TextBox_HostTSN.Text);
            if (isHOSTTSNContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Host TSN contain invalid character</img> </li>";
                return result = false;
            }
            bool isTextBox_Dispensed1ContainSpecialChars = CheckSpecialChars(TextBox_Dispensed1.Text);
            if (isTextBox_Dispensed1ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Dispensed 1 contain invalid character</img> </li>";
                return result = false;
            }
            bool isTextBox_Dispensed2ContainSpecialChars = CheckSpecialChars(TextBox_Dispensed2.Text);
            if (isTextBox_Dispensed2ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Dispensed 2 contain invalid character</img> </li>";
                return result = false;
            }
            bool isTextBox_Dispensed3ContainSpecialChars = CheckSpecialChars(TextBox_Dispensed3.Text);
            if (isTextBox_Dispensed3ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Dispensed 3 contain invalid character</img> </li>";
                return result = false;
            }
            bool isTextBox_Dispensed4ContainSpecialChars = CheckSpecialChars(TextBox_Dispensed4.Text);
            if (isTextBox_Dispensed4ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Dispensed 4 contain invalid character</img> </li>";
                return result = false;
            }
            return result;
        }

     

        protected void Button_ExportToExcel_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToExcel_Click");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = DateTime.Today;
                cmd.Parameters.Add("@Orderby", SqlDbType.VarChar);
                cmd.Parameters[2].Value = ViewState["orderby"];
                //edit by shariq on 21 oct    
                cmd.Parameters.Add("@conditions", SqlDbType.VarChar);
                cmd.Parameters[3].Value = GetFilter();
                cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                cmd.Parameters[4].Value = Session[SessionVars.SelectedATMs.ToString()];
                cmd.Parameters.Add("@type", SqlDbType.VarChar);
                cmd.Parameters[5].Value = Request.QueryString["ej"] == null ? "1" : Request.QueryString["ej"].ToString();


                cmd.CommandText = "GetViewParsedTransaction";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\ExportEjTransactions.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "EJ Cash Withdrawal";
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

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                cmd.Parameters[1].Value = DateTime.Today;
                cmd.Parameters.Add("@Orderby", SqlDbType.VarChar);
                cmd.Parameters[2].Value = ViewState["orderby"];
                //edit by shariq on 21 oct    
                cmd.Parameters.Add("@conditions", SqlDbType.VarChar);
                cmd.Parameters[3].Value = GetFilter();
                cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                cmd.Parameters[4].Value = Session[SessionVars.SelectedATMs.ToString()];
                cmd.Parameters.Add("@type", SqlDbType.VarChar);
                cmd.Parameters[5].Value = Request.QueryString["ej"] == null ? "1" : Request.QueryString["ej"].ToString();


                cmd.CommandText = "GetViewParsedTransaction";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\ExportEjTransactions.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "EJ Cash Withdrawal";
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

        //        private string GetQuery(string requestEj)
        //        {
        //                if (requestEj == "2")
        //                    return @"SELECT transaction_start_time,transaction_end_time,card_taken_time, vEjParsedTransactions.ej_parsed_transactions_id,outerATM.TITLE, outerATM.ip, trxn_datetime, vEjParsedTransactions.amount,vEjParsedTransactions.tsn,vEjParsedTransactions.host_tsn,vEjParsedTransactions.pan,notes_dispensed_type1,
        //                    notes_dispensed_type2,notes_dispensed_type3,notes_dispensed_type4,notes_dispensed_type5,notes_dispensed_type6,notes_dispensed_type7,case when vEjParsedTransactions.status =0 then 'Successful' 
        //			        when vEjParsedTransactions.status=1 then 'Failed'
        //			        when vEjParsedTransactions.status =2 then 'Suspicious' end status,comment_text,sms_customer_detail_id,captured_transactions.trxn_status,captured_transactions.amount_claimed customer_claim_amount,captured_transactions.comments user_comments,transaction_type_name,result,mstate_desc,sms_task.status alert_status,mstate.mstate_id,processing_datetime,notes_remaining_type1,notes_remaining_type2,notes_remaining_type3,notes_remaining_type4,posting_date,currency,
        //                    n.denomination_type_1_title, n.denomination_type_2_title, n.denomination_type_3_title, n.denomination_type_4_title,
        //                    n.denomination_type_5_title, n.denomination_type_6_title, n.denomination_type_7_title,vEjParsedTransactions.bank_name,captured_transactions_id,transaction_rule_id
        //                     from captured_transactions inner join vEjParsedTransactions 
        //                    on captured_transactions.transaction_id = vEjParsedTransactions.ej_parsed_transactions_id
        //                    inner join atm outerATM on vEjParsedTransactions.atm_id = outerATM.atm_id 
        //                    inner join note_set_type n ON outerATM.note_set_type_id = n.note_set_type_id
        //                    inner join transaction_type on transaction_type.transaction_type_id =vEjParsedTransactions.transaction_type_id
        //                    left join comment on comment.comment_id = vEjParsedTransactions.comment_id 
        //                    left join mstate on vEjParsedTransactions.mstate_id = mstate.mstate_id 
        //                    left join sms_customer_detail on sms_customer_detail.ej_transaction_id = vEjParsedTransactions.ej_parsed_transactions_id
        //                    left join sms_task on sms_task.sms_task_id = sms_customer_detail.sms_task_id"
        //                        // left join captured_transactions on captured_transactions.ej_parsed_transactions_id =vEjParsedTransactions.ej_parsed_transactions_id"
        //                + " where outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.IS_ACTIVE =1  " + GetFilter()
        //                    + " order by " + ViewState["orderby"];

        //                else
        //                    return @"SELECT transaction_start_time,transaction_end_time,card_taken_time, vEjParsedTransactions.ej_parsed_transactions_id,outerATM.TITLE, outerATM.ip, trxn_datetime, vEjParsedTransactions.amount,vEjParsedTransactions.tsn,vEjParsedTransactions.host_tsn,vEjParsedTransactions.pan,notes_dispensed_type1,
        //                    notes_dispensed_type2,notes_dispensed_type3,notes_dispensed_type4,notes_dispensed_type5,notes_dispensed_type6,notes_dispensed_type7,case when vEjParsedTransactions.status =0 then 'Successful' 
        //                    when vEjParsedTransactions.status=1 then 'Failed'
        //                    when vEjParsedTransactions.status =2 then 'Suspicious' end status,comment_text,sms_customer_detail_id,captured_transactions.trxn_status,captured_transactions.amount_claimed customer_claim_amount,captured_transactions.comments user_comments,transaction_type_name,result,mstate_desc,sms_task.status alert_status,mstate.mstate_id,processing_datetime,notes_remaining_type1,notes_remaining_type2,notes_remaining_type3,notes_remaining_type4,posting_date,currency,
        //                    n.denomination_type_1_title, n.denomination_type_2_title, n.denomination_type_3_title, n.denomination_type_4_title,
        //                    n.denomination_type_5_title, n.denomination_type_6_title, n.denomination_type_7_title,vEjParsedTransactions.bank_name,captured_transactions_id,transaction_rule_id
        //                    from vEjParsedTransactions inner join atm outerATM on vEjParsedTransactions.atm_id = outerATM.atm_id 
        //                    inner join note_set_type n ON outerATM.note_set_type_id = n.note_set_type_id
        //                    inner join transaction_type on transaction_type.transaction_type_id =vEjParsedTransactions.transaction_type_id
        //                    left join comment on comment.comment_id = vEjParsedTransactions.comment_id
        //                    left join mstate on vEjParsedTransactions.mstate_id = mstate.mstate_id 
        //                    left join sms_customer_detail on sms_customer_detail.ej_transaction_id = vEjParsedTransactions.ej_parsed_transactions_id
        //                    left join sms_task on sms_task.sms_task_id = sms_customer_detail.sms_task_id 
        //                    left join captured_transactions on captured_transactions.ej_parsed_transactions_id =vEjParsedTransactions.ej_parsed_transactions_id"
        //                + " where outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.IS_ACTIVE =1 " + GetFilter()
        //                + " order by " + ViewState["orderby"];


        //        }

        private void EnableGridRespectiveColumns()
        {
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);

            int columnIndexDispensed = -1;
            int columnIndexDenomination = -1;

            columnIndexDispensed = GetColumnIndexByName(this.GridView_Trxn, "Dispensed 5");
            columnIndexDenomination = GetColumnIndexByName(this.GridView_Trxn, "Type 5");


            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexDenomination >= 0)
                {
                    this.GridView_Trxn.Columns[columnIndexDenomination].Visible = true;
                    columnIndexDenomination += 2;
                }

                if (columnIndexDispensed >= 0)
                {
                    this.GridView_Trxn.Columns[columnIndexDispensed].Visible = true;
                    columnIndexDispensed += 2;
                }
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


                    //if (Convert.ToBoolean(e.Parameters[0]) == false && (capturedTrxn.IsLocked.HasValue && !capturedTrxn.IsLocked.Value))
                    if (Convert.ToBoolean(e.Parameters[0]) == false && ((capturedTrxn.IsLocked.HasValue && !capturedTrxn.IsLocked.Value) || capturedTrxn.UserId == ((AppUser)Session[SessionVars.user.ToString()]).UserId))
                    {
                        capturedTrxn.IsLocked = true;
                        capturedTrxn.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        capturedTrxn.LockedDatetime = DateTime.Now;
                        capturedTrxn.Save();

                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                        //cmd.CommandText = "select action_code from sms_transaction_type_detail where transaction_type_id = " + transactionTypeId + " and action_code<> '' order by sms_transaction_type_detail_id";
                        cmd.CommandText = "select action_code from sms_transaction_type_detail where transaction_type_id = " + transactionTypeId + " and action_code<> '' and screen_name = 'ViewEJTransactions' order by sms_transaction_type_detail_id";

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
                        builder.Append("<tr><td style=\"border:0;\"> Internal Team Comment:</td><td style=\"border:0;\"> <textarea rows=\"10\" cols=\"50\" id=\"txtInternalComment\"></textarea></td></tr>");
                        builder.Append("<tr><td style=\"border:0;\"><input type=\"button\" value=\"Save\" onclick=\"onCloseOfDialog();\" class=\"button\"></td>");
                        builder.Append("<td style=\"border:0;\"><input type=\"button\" value=\"Close\" class=\"button\" onclick=\"onClose();\"/></td></tr>");

                        e.Output.Write("<script> receiveCallBackExceptionFormResult('<table border=1 width=100%><tbody>" + builder.ToString() + "</tbody></table>');</script>");
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

                    }
                }
                if (Convert.ToBoolean(e.Parameters[0]) == true)
                    e.Output.Write("<script> window.location='ViewEJTransactions.aspx';</script>");
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
    }
}