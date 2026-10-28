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

using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;

namespace CCMSUI.admin
{
    public partial class VaultSettlement : BasePage
    {
        DataTable dt = null;


        private void BindGridCIT(DateTime vaultDt, int vaultId)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select title, denomination_name,opening_balance,cash_recieved_from_bank,cash_delivered_to_atm,cash_returned_from_atm,closing_balance,generated_at,vault_summary_date
                                    from vault_settlement inner join CCMS_vault on CCMS_vault.id = vault_settlement.vault_id
                                    inner join vault_settlement_detail on vault_settlement.vault_settlement_id =vault_settlement_detail.vault_settlement_id
                                    and vault_summary_date >=convert(datetime,'" + vaultDt.ToString("dd/MM/yyyy")+ "',103)  " +
                                    " and vault_summary_date <=convert(datetime,'" + vaultDt.ToString("dd/MM/yyyy HH:mm:ss")+ "',103) and id = "+vaultId;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtMonitoring = new DataTable();
                adapter.Fill(dtMonitoring);
                GridView_CITVaultBalances.DataSource = dtMonitoring;
                GridView_CITVaultBalances.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";

            }
            finally
            {
                task.EndTask();

            }
        }

        private void BindGrid(DateTime vaultDt, int vaultId)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = null;
            DataTable dt = null;
            try
            {
                bool isTitlePrinted = false;

                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("Title");
                dtGrid.Columns.Add("Date");
                dtGrid.Columns.Add("Type");
                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandText = @"select title,ledger_date,vault_transaction_type,transaction_type,denomination_name,total_quantity,total_value, outerVaultBalance.vault_id vaultID
from CCMS_vault_day_balance outerVaultBalance inner join CCMS_vault on CCMS_vault.id = outerVaultBalance.vault_id 
where CCMS_vault.id = " + vaultId + " and  ledger_date in (select max(ledger_date) from CCMS_vault_day_balance " + 
" where vault_transaction_Type = outerVaultBalance.vault_transaction_type " +
" and vault_id = outerVaultBalance.vault_id and ledger_date<= convert(datetime,'" + vaultDt.ToString("dd/MM/yyyy") + "',103))";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                DataTable dtDenom = dt.DefaultView.ToTable(true, new string[] { "denomination_name" });
                foreach (DataRow dr in dtDenom.Rows)
                    dtGrid.Columns.Add(dr[0].ToString());
                dtGrid.Columns.Add("Total");
                dtGrid.Columns.Add("Value");

                DataTable dtVaultIDs = dt.DefaultView.ToTable(true, new string[] { "vaultID" });

                foreach (DataRow dr in dtVaultIDs.Rows)
                {
                    isTitlePrinted = false;
                    //Inbound...
                    int total = 0;
                    int totalVal = 0;
                    DataRow newRow = dtGrid.NewRow();
                    newRow["type"] = "Inbound Bulk";
                    DataRow[] result = dt.Select("vault_transaction_type='Bulk Cash' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        newRow["Title"] = result[0]["title"];
                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                        isTitlePrinted = true;

                    }
                    dtGrid.Rows.Add(newRow);
                    dtGrid.Rows.Add(dtGrid.NewRow());

                    //Inbound...
                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Inbound (Residual)";
                    result = dt.Select("vault_transaction_type='Inbound' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);


                    //Outbound...
                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Outbound (Replenishment)";
                    result = dt.Select("vault_transaction_type='Outbound' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);

                    //
                    dtGrid.Rows.Add(dtGrid.NewRow());
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Net";
                    dtGrid.Rows.Add(newRow);
                    dtGrid.Rows.Add(dtGrid.NewRow());

                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Net Adjustments (+)";
                    result = dt.Select("vault_transaction_type='Adjustment Credit' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);

                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Net Adjustments (-)";
                    result = dt.Select("vault_transaction_type='Adjustment Debit' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);


                    dtGrid.Rows.Add(dtGrid.NewRow());
                    //Inbound...
                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Total Available";
                    result = dt.Select("vault_transaction_type='Total' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);

                    //newRow = dtGrid.NewRow();
                    //newRow["type"] = "Total Available";
                    //for (int i = dt.Rows.Count - 4; i < dt.Rows.Count ; i++)
                    //{
                    //    newRow[i + 1] = dt.Rows[i]["total_quantity"];
                    //    total += int.Parse(dt.Rows[i]["total_quantity"].ToString());
                    //}
                    //newRow["total"] = total;
                    //dtGrid.Rows.Add(newRow);
                    //Literal_summary.Text = dt.Rows.Count + " records.";
                }
                this.GridView_VaultBalanceInvestigation.DataSource = dtGrid;
                this.GridView_VaultBalanceInvestigation.DataBind();


            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.IsPostBack)
                return;
 

            if (!CheckRight((int)Permissions.ApproveVaultSettlement))
                Response.Redirect("AccessDenied.aspx");



            if (Request.QueryString["id"] != null)
            {
                Avanza.CCMS.DAL.VaultSettlement vaultSettlement = Avanza.CCMS.DAL.VaultSettlement.LoadVaultSettlementByPk(int.Parse(Request.QueryString["id"]));
                if (vaultSettlement != null)
                {
                    BindGridCIT(vaultSettlement.VaultSummaryDate, vaultSettlement.VaultId);
                    BindGrid(vaultSettlement.VaultSummaryDate, vaultSettlement.VaultId);
                
                }
                

                if (Request.QueryString["s"] != null)
                {
                    trAddEditVaultAdj.Visible = false;
                    trApproveRejectVaultAdj.Visible = true;


                    if (Request.QueryString["s"].ToString() == "1")
                    {
                        Button_ApproveVaultAdj.Enabled = false;
                        Button_RejectVaultAdj.Enabled = false;
                    }
                    else if (Request.QueryString["s"].ToString() == "2")
                    {
                        Button_ApproveVaultAdj.Enabled = false;
                        Button_RejectVaultAdj.Enabled = false;
                    }
                }
                else
                {
                    trApproveRejectVaultAdj.Visible = false;
                    trAddEditVaultAdj.Visible = true;
                }
            }
        }


       


        //public class VaultInfo
        //{
        //    public int newVaultId = 0;
        //    public int existingVaultId = 0;
        //}


        //private VaultInfo CreateVaultLedger(int vaultId, decimal amount, int id, SqlTransaction trxn, string trxnType)
        //{
        //    VaultInfo vaultInfo = new VaultInfo();
        //    CcmsVaultLedger vaultLedger = new CcmsVaultLedger();
        //    vaultLedger.TransactionDate = DateTime.Now;
        //    vaultLedger.TransactionType = trxnType;
        //    vaultLedger.Description = "Adjustment of amount " + amount + " posted on Vault";
        //    vaultLedger.LedgerAmount = amount;
        //    vaultLedger.PostedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
        //    vaultLedger.VaultId = vaultId;
        //    if (trxnType == "Cr")
        //        vaultLedger.VaultTransactionType = "Adjustment Credit";
        //    else
        //        vaultLedger.VaultTransactionType = "Adjustment Debit";
        //    vaultLedger.VaultAdjustmentId = id;

        //    object result = ConnectionFactory.ExecuteScalar("select max(id) from ccms_vault_ledger where vault_id = " + vaultId);
        //    if (result != DBNull.Value)
        //    {
        //        CcmsVaultLedger existingLedger = CcmsVaultLedger.LoadCcmsVaultLedgerByPk(int.Parse(result.ToString()));
        //        if (existingLedger != null)
        //        {
        //            if (trxnType == "Cr")
        //                vaultLedger.Balance = existingLedger.Balance + vaultLedger.LedgerAmount;
        //            else
        //                vaultLedger.Balance = existingLedger.Balance - vaultLedger.LedgerAmount;
        //            vaultInfo.existingVaultId = existingLedger.Id;
        //        }
        //    }
        //    else
        //        vaultLedger.Balance = vaultLedger.LedgerAmount;

        //    vaultLedger.Save(trxn.Connection, trxn);
        //    vaultInfo.newVaultId = vaultLedger.Id;
        //    return vaultInfo;
        //}

        //private void CreateVaultLedgerDetail(VaultInfo vaultInfo, int qty, int vaultNoteTypeId, string trxnType, SqlTransaction trxn)
        //{
        //    CcmsVaultLedgerDetail vaultLedgerDetail = new CcmsVaultLedgerDetail();
        //    vaultLedgerDetail.LedgerId = vaultInfo.newVaultId;
        //    vaultLedgerDetail.Quantity = qty;
        //    vaultLedgerDetail.VaultNoteTypeId = vaultNoteTypeId;

        //    CcmsVaultLedgerDetail existingVaultLedgerDetail = CcmsVaultLedgerDetail.LoadCcmsVaultLedgerDetail(
        //        string.Format("ledger_id = {0} and vault_note_type_id={1}", vaultInfo.existingVaultId, vaultNoteTypeId));
        //    if (existingVaultLedgerDetail != null)
        //    {
        //        if (vaultLedgerDetail.Balance == null)
        //            vaultLedgerDetail.Balance = 0;

        //        if (existingVaultLedgerDetail.Balance == null)
        //            existingVaultLedgerDetail.Balance = 0;



        //        if (trxnType == "Cr")
        //            vaultLedgerDetail.Balance = existingVaultLedgerDetail.Balance + qty;
        //        else if (trxnType == "Dr")
        //            vaultLedgerDetail.Balance = existingVaultLedgerDetail.Balance - qty;
        //    }
        //    else
        //    {
        //        if (trxnType == "Cr")
        //            vaultLedgerDetail.Balance = qty;
        //        else
        //            vaultLedgerDetail.Balance = -qty;
        //    }
        //    vaultLedgerDetail.Save(trxn.Connection, trxn);
        //}

        protected void Button_ApproveVaultAdj_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            conn = ConnectionFactory.GetNewConnection();
            try
            {
//                conn.Open();
//                trxn = conn.BeginTransaction();

//                CcmsVaultAdjustments vaultAdj = CcmsVaultAdjustments.LoadCcmsVaultAdjustmentsByPk(int.Parse(Request.QueryString["id"]));
//                VaultInfo vaultInfo = CreateVaultLedger(vaultAdj.VaultId, vaultAdj.Amount, vaultAdj.Id, trxn, vaultAdj.TrxnType);

//                DataTable dtVaultNotesDetail = new DataTable();
//                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
//                cmd.CommandText = "select id, denomination_id, denomination_name from ccms_vault_note_type where vault_id = " + vaultAdj.VaultId;
//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                adapter.Fill(dtVaultNotesDetail);


//                foreach (GridViewRow row in GridView_VaultNoteDetail.Rows)
//                {
//                    string type = row.Cells[0].Text;
//                    HtmlInputText quantity = (HtmlInputText)row.FindControl("TextBox_Quantity");
//                    DataRow[] dr = dtVaultNotesDetail.Select("denomination_name='" + type + "'");
//                    CreateVaultLedgerDetail(vaultInfo, int.Parse(quantity.Value), int.Parse(dr[0]["id"].ToString()), vaultAdj.TrxnType, trxn);
//                }

//                //Do not enter in temp vault....Forgot reason thats uncommenting it..
//                ConnectionFactory.ExecuteQuery(string.Format(@"insert into ccms_temp_vault_ledger(
//                                                  [transaction_date] ,[description] ,[transaction_type] ,[ledger_amount] ,[balance] ,[posted_by]
//                                                  ,[vault_id] ,[vault_transaction_type] ,[cheque_id] ,[atm_id]) 
//                                                    select [transaction_date] ,[description] ,[transaction_type] ,[ledger_amount] ,[balance] ,[posted_by]
//                                                  ,[vault_id] ,[vault_transaction_type] ,[cheque_id] ,[atm_id] from ccms_vault_ledger
//                                                   where id = {0}; insert into ccms_temp_vault_ledger_detail(
//                                                   [ledger_id],[quantity] ,[vault_note_type_id],balance) 
//                                                    select scope_identity(),[quantity] ,[vault_note_type_id],balance from ccms_vault_ledger_detail
//                                                    where ledger_id = {0}", vaultInfo.newVaultId), trxn);

//                Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(base.Request.QueryString["tid"]));
//                userTask.Status = ApprovalStatus.Approved.ToString();
//                userTask.Save(trxn.Connection, trxn);
//                trxn.Commit();
//                Utility.BuildAuditLog("Vault Adjustment Approved " + CcmsVault.LoadCcmsVaultByPk(vaultAdj.VaultId).Title + " approved", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ApproveVaultAdjustment);
//                Response.Redirect("~/ccms/usertask.aspx");
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                LogableTask.LogMonoActivityTask("ApproveVaultAdjustment", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

        }
        protected void Button_Reject_Click(object sender, EventArgs e)
        {
            Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(base.Request.QueryString["tid"]));
            userTask.Status = ApprovalStatus.Rejected.ToString();
            userTask.Save();
            base.Response.Redirect("~/ccms/UserTask.aspx");
        }

      


        private DataTable GetGridDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("type");
            dt.Columns.Add("quantity");
            dt.Columns.Add("vault_adjustment_id");
            return dt;
        }

        protected void GridView_VaultBalanceInvestigation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[2].Text.Contains("Inbound"))
                {
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#BF4b4f");
                }
                else if (e.Row.Cells[2].Text.Contains("Outbound"))
                {
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#527fb8");
                }
                else if (e.Row.Cells[2].Text == "Net")
                {
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#8766a5");
                }
                else if (e.Row.Cells[2].Text.Contains("Total"))
                {
                    e.Row.Cells[2].Style.Add("font-weight", "bold");
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#FCb441");
                }


            }
        }
       


    }
}