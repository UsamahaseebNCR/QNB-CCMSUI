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
using Avanza.CCMS.DAL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace CCMSUI.CCMS
{
    public partial class ViewEJCPMTransactions : BasePage
    {
        protected void GridView_Trxn_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "viewDetails")
            {
                Session["ej"] = e.CommandArgument.ToString();
                Session["d"] = "2";
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
                if (!CheckRight((int)Permissions.ViewPNCCPMTransactions, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["SortExpression"] = "trxn_datetime";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                if (System.Configuration.ConfigurationManager.AppSettings["isRunningForPNC"] == "0")
                {
                    tdAnalysisOn.Visible = false;
                    tdAnalysisOnCheckBoxList.Visible = false;
                }
                if (System.Configuration.ConfigurationManager.AppSettings["isHostTsnEnable"] == "false")
                {
                    GridView_Trxn.Columns[6].Visible = false;
                }
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
                int depositCount = 0;
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("bank_name");
                dtGrid.Columns.Add("trxn_datetime", typeof(DateTime));
                dtGrid.Columns.Add("seq");
                dtGrid.Columns.Add("account_type");
                dtGrid.Columns.Add("pan");
                dtGrid.Columns.Add("deposit_amount");
                dtGrid.Columns.Add("result");
                dtGrid.Columns.Add("dispute_status");
                dtGrid.Columns.Add("status");
                dtGrid.Columns.Add("comment");
                dtGrid.Columns.Add("check_amount");
                dtGrid.Columns.Add("detail");
                dtGrid.Columns.Add("deposit_count");
                dtGrid.Columns.Add("ej_parsed_cpm_transaction_id");
                dtGrid.Columns.Add("account_no");
                dtGrid.Columns.Add("host_tsn");
                dtGrid.Columns.Add("micr");
                dtGrid.Columns.Add("sms_customer_detail_id");
                dtGrid.Columns.Add("trxn_status");
                dtGrid.Columns.Add("customer_claim_amount");
                dtGrid.Columns.Add("user_comments");
                dtGrid.Columns.Add("alert_status");



                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                //edit by shariq for the purpose of customer search
                // if (TextBox_referenceNo.Text.Length > 0 || TextBox_customerMobile.Text.Length > 0 || TextBox_customerID.Text.Length > 0 || TextBox_customerTelephone.Text.Length > 0)
                //{
                cmd.CommandText = GetQuery();

                //  }
                //                else
                //                {
                //                    cmd.CommandText = @"select title,trxn_datetime,seq,account_type,pan,deposit_amount,result,dispute_status,status,comment,check_amount,ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id,account_no,host_tsn,micr
                //                                    from ej_parsed_cpm_transaction inner join atm 
                //                                    on ej_parsed_cpm_transaction.atm_id = atm.atm_id
                //                                    left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id = 
                //                                    ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id 
                //                                    where atm.is_active=1 and (is_ccdm = 1 or is_sdm = 1)
                //                                    and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
                //                                        + " order by " + ViewState["orderby"];
                //                }
                //*****
                //                cmd.CommandText = @"select title,trxn_datetime,seq,account_type,pan,deposit_amount,result,dispute_status,status,comment
                //                                    from ej_parsed_cpm_transaction inner join atm 
                //                                    on ej_parsed_cpm_transaction.atm_id = atm.atm_id where atm.is_active=1 
                //                                    and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
                //                + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                //Bind Comment Combo
                //DropDownList_Comment.Items.Clear();
                //DropDownList_Comment.Items.Add("Any");
                //DataTable comments = dtTrxn.DefaultView.ToTable(true, new string[] { "comment" });
                ////DataRow[] dataRow = dtTrxn.Select("comment<>''");
                //foreach (DataRow dr in comments.Rows)
                //{
                //    if (dr["comment"].ToString().Length>0)
                //    DropDownList_Comment.Items.Add(dr["comment"].ToString());
                //}
                //
                DataTable dtCPMTrxnIds = dtTrxn.DefaultView.ToTable(true, new string[] { "ej_parsed_cpm_transaction_id" });
                foreach (DataRow drRow in dtCPMTrxnIds.Rows)
                {
                    depositCount = 0;
                    StringBuilder builder = new StringBuilder();

                    DataRow[] dr = dtTrxn.Select("ej_parsed_cpm_transaction_id=" + drRow["ej_parsed_cpm_transaction_id"]);
                    if (dr.Length > 0)
                    {
                        //    DataTable dtUniqueNoteType = dr.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                        foreach (DataRow drDepositedCheque in dr)
                        {
                            //DataRow[] temp = dtTrxn.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"]);
                            if (drDepositedCheque["check_amount"].ToString().Length > 0)
                            {
                                builder.Append(drDepositedCheque["check_amount"].ToString() + "<br/>");
                                depositCount++;
                            }
                        }

                    }
                    DataRow drNewRow = dtGrid.NewRow();
                    drNewRow["title"] = dr[0]["title"];
                    drNewRow["bank_name"] = dr[0]["bank_name"];
                    drNewRow["trxn_datetime"] = dr[0]["trxn_datetime"];
                    drNewRow["seq"] = dr[0]["seq"];
                    drNewRow["account_type"] = dr[0]["account_type"];
                    drNewRow["pan"] = dr[0]["pan"];
                    drNewRow["deposit_amount"] = dr[0]["deposit_amount"];
                    drNewRow["result"] = dr[0]["result"];
                    drNewRow["dispute_status"] = dr[0]["dispute_status"];
                    drNewRow["status"] = dr[0]["status"];
                    drNewRow["comment"] = dr[0]["comment"];
                    drNewRow["detail"] = builder.ToString();
                    drNewRow["check_amount"] = dr[0]["check_amount"];
                    drNewRow["deposit_count"] = depositCount;
                    drNewRow["ej_parsed_cpm_transaction_id"] = dr[0]["ej_parsed_cpm_transaction_id"];
                    drNewRow["account_no"] = dr[0]["account_no"];
                    drNewRow["host_tsn"] = dr[0]["host_tsn"];
                    drNewRow["micr"] = dr[0]["micr"];
                    drNewRow["sms_customer_detail_id"] = dr[0]["sms_customer_detail_id"];

                    drNewRow["trxn_status"] = dr[0]["trxn_status"];
                    drNewRow["customer_claim_amount"] = dr[0]["amount"];
                    drNewRow["user_comments"] = dr[0]["user_comments"];
                    drNewRow["alert_status"] = dr[0]["alert_status"];
                    dtGrid.Rows.Add(drNewRow);

                }

                Literal_count.Text = dtGrid.Rows.Count + " records.";
                this.GridView_Trxn.DataSource = dtGrid;
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
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            if (TextBox_FromAmount.Text.Length > 0)
                filter += " and deposit_amount >= " + int.Parse(TextBox_FromAmount.Text);

            if (TextBox_ToAmount.Text.Length > 0)
                filter += " and deposit_amount <= " + int.Parse(TextBox_ToAmount.Text);


            if (TextBox_TSN.Text.Length > 0)
                filter += " and seq = '" + TextBox_TSN.Text + "'";

            if (TextBox_HostTSN.Text.Length > 0)
                filter += " and host_tsn = '" + TextBox_HostTSN.Text + "'";
            //filter += " and tsn = " + int.Parse(TextBox_TSN.Text);

            if (TextBox_PAN.Text.Length > 0)
                filter += " and vEjParsedCPMtransaction.pan like '%" + TextBox_PAN.Text + "%'";


            if (TextBox_bankName.Text.Length > 0)
                filter += " and vEjParsedCPMtransaction.bank_name like '%" + TextBox_bankName.Text + "%'";

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

            if (CheckBoxList_IsDisputed.Items[0].Selected)
                filter += " and is_disputed_transaction = 1";

            if (CheckBoxList_Status.Items[0].Selected && CheckBoxList_Status.Items[1].Selected)
            {
            }
            else if (CheckBoxList_Status.Items[0].Selected)
                filter += " and vEjParsedCPMtransaction.status = 'Successful'";
            else if (CheckBoxList_Status.Items[1].Selected)
                filter += " and vEjParsedCPMtransaction.status = 'Failed'";

            //if (DropDownList_Comment.SelectedIndex > 0)
            //    filter += " and comment = '" + DropDownList_Comment.SelectedItem.Text + "'";
            //edit by shariq for the purpose of customer search
            if (TextBox_referenceNo.Text.Length > 0)
                filter += " and  refrence_no like '%" + TextBox_referenceNo.Text + "%'";

            if (TextBox_customerTelephone.Text.Length > 0)
                filter += " and  customer_telephone like '%" + TextBox_customerTelephone.Text + "%'";

            if (TextBox_customerMobile.Text.Length > 0)
                filter += " and  customer_mobile_no like '%" + TextBox_customerMobile.Text + "%'";

            if (TextBox_customerID.Text.Length > 0)
                filter += " and  customer_id like '%" + TextBox_customerID.Text + "%'";
            //****
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
        protected void GridView_Trxn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem).Row.ItemArray[18].ToString().Length == 0)
                    e.Row.FindControl("anchorInquiry").Visible = false;
                else
                    ((ImageButton)e.Row.FindControl("anchorInquiry")).Attributes.Add("onclick", "javascript:window.open('ViewCustomerDetail.aspx?id=" + ((DataRowView)e.Row.DataItem).Row.ItemArray[13] + "')");
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
            bool isRefrenceContainSpecialChars = CheckSpecialChars(TextBox_referenceNo.Text);
            if (isRefrenceContainSpecialChars == true)
            {
                throw new Exception("Refrence Number contain invalid character");

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
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\EjCPMTransactions.rpt");
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
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\EjCPMTransactions.rpt");
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

        private string GetQuery()
        {
            return @"select title,vEjParsedCPMtransaction.bank_name,trxn_datetime,seq,account_type,vEjParsedCPMtransaction.pan,deposit_amount,result,dispute_status,vEjParsedCPMtransaction.status,comment,check_amount,vEjParsedCPMtransaction.ej_parsed_cpm_transaction_id,vEjParsedCPMtransaction.account_no,host_tsn,micr,refrence_no,customer_id,customer_telephone,customer_mobile_no,sms_customer_detail_id,captured_transactions.trxn_status,captured_transactions.comments user_comments,captured_transactions.amount ,sms_task.status alert_status 
                                    from vEjParsedCPMtransaction inner join atm on vEjParsedCPMtransaction.atm_id = atm.atm_id
                                    left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id = vEjParsedCPMtransaction.ej_parsed_cpm_transaction_id
									left join captured_transactions on captured_transactions.ej_parsed_cpm_transactions_id =vEjParsedCPMtransaction.ej_parsed_cpm_transaction_id 
                                    left join sms_task on sms_task.captured_transaction_id=captured_transactions.captured_transactions_id and sms_task.is_eligible=1
									left join sms_customer_detail on sms_customer_detail.sms_task_id = sms_task.sms_task_id                                  
                                    where atm.is_active=1 and (is_ccdm = 1 or is_sdm = 1) 
                                    and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
                                        + " order by " + ViewState["orderby"];

        }
   

    }
}