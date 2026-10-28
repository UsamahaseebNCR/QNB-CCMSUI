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
using Avanza.CCMS.DAL;
//using Avanza.iSuite.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using Avanza.iSuite.UI;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace CCMSUI.CCMS
{
    public partial class ScheduleReconBatch : BasePage
    {
    
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ScheduleFileDownload))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_AcceptableDifference.Text = "0.0";

                }
                //BindGrid("temp.title,temp.file_type_id");
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Label_errorMsg.Text = "<li>Error! " + ex.Message + "</li>";
            }
            finally
            { task.EndTask(); }
        }



        protected void ButtonSchedule_Click(object sender, EventArgs e)
        {

            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlTransaction trxn = null;
            ReconciliationBatch batch = null;
            List<int> batchID = new List<int>();
            bool trxnCommitted = false;
            try
            {
                conn.Open();
                trxn = conn.BeginTransaction();
                string ATM_id = Session[SessionVars.SelectedATMs.ToString()].ToString();
                if (ATM_id.Contains(","))
                {
                    string[] ATMs = ATM_id.Split(',');
                    
                    for (int i = 1; i <= ATMs.Length - 1; i++)
                    {
                        batch = new ReconciliationBatch();
                        batch.TransactionStartDate = DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null);
                        batch.TransactionEndDate = DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null); // yahan se change start karna ha
                        if (TextBox_AcceptableDifference.Text.Length > 0)
                            batch.AcceptableDifference = decimal.Parse(TextBox_AcceptableDifference.Text);
                        else
                            batch.AcceptableDifference = 0;
                        batch.CreationTime = DateTime.Now;
                        batch.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        batch.RetryCount = 10;
                        batch.Status = "Scheduled";
                        batch.AtmId = int.Parse(ATMs[i].ToString());
                        batch.AcceptableDifferenceType = DropDownList_Diff.SelectedItem.Text;
                        batch.Save(trxn.Connection, trxn);
                        batchID.Add(batch.ReconciliationBatchId);
                    }
                }
                else
                {
                    batch = new ReconciliationBatch();
                    batch.TransactionStartDate = DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null);
                    batch.TransactionEndDate = DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null); // yahan se change start karna ha
                    if (TextBox_AcceptableDifference.Text.Length > 0)
                        batch.AcceptableDifference = decimal.Parse(TextBox_AcceptableDifference.Text);
                    else
                        batch.AcceptableDifference = 0;
                    batch.CreationTime = DateTime.Now;
                    batch.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    batch.RetryCount = 10;
                    batch.Status = "Scheduled";
                    batch.AtmId = int.Parse(ATM_id);
                    batch.AcceptableDifferenceType = DropDownList_Diff.SelectedItem.Text;
                    batch.Save(trxn.Connection, trxn);
                }
                
                
                if (FileUpload_SwitchData.HasFile)
                {
                    foreach (int _batchID in batchID)
                    {
                        string data = Encoding.ASCII.GetString(FileUpload_SwitchData.FileBytes);
                        string[] parts = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in parts)
                        {
                            string[] subParts = part.Split(',');
                            ReconciliationSwitchData switchData = new ReconciliationSwitchData();
                            ReconciliationBnaSwitchData BNAswitchData = new ReconciliationBnaSwitchData();
                            if (subParts[8] == "Cash Withdrawal")
                            {
                                switchData.ReconciliationBatchId = _batchID;
                                switchData.AtmId = subParts[0];
                                switchData.CardNumber = subParts[1];
                                switchData.CustomerAccountNo = subParts[2];
                                switchData.TransactionDate = subParts[3];
                                switchData.TransactionTime = subParts[4];
                                switchData.TransactionAmount = decimal.Parse(subParts[5]);
                                switchData.TransactionCurrency = subParts[6];
                                switchData.TransactionSequence = subParts[7];
                                switchData.TransactionType = subParts[8];
                                switchData.TransactionResponse = subParts[9];
                                switchData.TransactionSettlementDate = DateTime.ParseExact(subParts[10], "dd/MM/yyyy", null);
                                switchData.CardNetwork = subParts[11];
                                switchData.CardIssuer = subParts[12];
                                switchData.Save(trxn.Connection, trxn);
                            }
                            else if (subParts[8] == "Cash Deposite")
                            {
                                BNAswitchData.ReconciliationBatchId = _batchID;
                                BNAswitchData.AtmId = subParts[0];
                                BNAswitchData.CardNumber = subParts[1];
                                BNAswitchData.CustomerAccountNo = subParts[2];
                                BNAswitchData.TransactionDate = subParts[3];
                                BNAswitchData.TransactionTime = subParts[4];
                                BNAswitchData.TransactionAmount = decimal.Parse(subParts[5]);
                                BNAswitchData.TransactionCurrency = subParts[6];
                                BNAswitchData.TransactionSequence = subParts[7];
                                BNAswitchData.TransactionType = subParts[8];
                                BNAswitchData.TransactionResponse = subParts[9];
                                BNAswitchData.TransactionSettlementDate = DateTime.ParseExact(subParts[10], "dd/MM/yyyy", null);
                                BNAswitchData.CardNetwork = subParts[11];
                                BNAswitchData.CardIssuer = subParts[12];
                                BNAswitchData.Save(trxn.Connection, trxn);
                            }

                        }
                    }
                    
                }
                if (FileUpload_GLData.HasFile)
                {
                    foreach (int _batchID in batchID)
                    {
                        string data = Encoding.ASCII.GetString(FileUpload_GLData.FileBytes);
                        string[] parts = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in parts)
                        {
                            string[] subParts = part.Split(',');
                            ReconciliationHostData hostData = new ReconciliationHostData();
                            ReconciliationBnaHostData BNAhostData = new ReconciliationBnaHostData();
                            if (subParts[8] == "Cash Withdrawal")
                            {
                                hostData.ReconciliationBatchId = _batchID;
                                hostData.AtmId = subParts[0];
                                hostData.CardNumber = subParts[1];
                                hostData.CustomerAccountNo = subParts[2];
                                hostData.TransactionDate = subParts[3];
                                hostData.TransactionTime = subParts[4];
                                hostData.TransactionAmount = decimal.Parse(subParts[5]);
                                hostData.TransactionCurrency = subParts[6];
                                hostData.TransactionSequence = subParts[7];
                                hostData.TransactionType = subParts[8];
                                hostData.TransactionResponse = subParts[9];
                                hostData.TransactionSettlementDate = DateTime.ParseExact(subParts[10], "dd/MM/yyyy", null);
                                hostData.CardNetwork = subParts[11];
                                hostData.CardIssuer = subParts[12];
                                hostData.Save(trxn.Connection, trxn);
                            }
                            else if (subParts[8] == "Cash Deposite")
                            {
                                BNAhostData.ReconciliationBatchId = _batchID;
                                BNAhostData.AtmId = subParts[0];
                                BNAhostData.CardNumber = subParts[1];
                                BNAhostData.CustomerAccountNo = subParts[2];
                                BNAhostData.TransactionDate = subParts[3];
                                BNAhostData.TransactionTime = subParts[4];
                                BNAhostData.TransactionAmount = decimal.Parse(subParts[5]);
                                BNAhostData.TransactionCurrency = subParts[6];
                                BNAhostData.TransactionSequence = subParts[7];
                                BNAhostData.TransactionType = subParts[8];
                                BNAhostData.TransactionResponse = subParts[9];
                                BNAhostData.TransactionSettlementDate = DateTime.ParseExact(subParts[10], "dd/MM/yyyy", null);
                                BNAhostData.CardNetwork = subParts[11];
                                BNAhostData.CardIssuer = subParts[12];
                                BNAhostData.Save(trxn.Connection, trxn);
                            }

                        }
                    }
                }
                trxn.Commit();
                trxnCommitted = true;
                Session[SessionVars.notice.ToString()] = "Reconciliation Batch Scheduled Successfully!";
                Response.Redirect("Reconciliation.aspx");
            }
            catch (Exception ex)
            {
                if (!trxnCommitted)
                    trxn.Rollback();
                LogableTask.LogMonoActivityTask("ButtonSchedule_Click", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = ex.Message;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }


        }
    }
}