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
using System.Reflection;
using System.Diagnostics;
using Avanza.iSuite.UI;
using System.IO;
using System.Data.OleDb;

using System.Data.SqlClient;
using System.Text;
using System.Runtime.InteropServices;


namespace CCMSUI.admin
{
    public partial class ImportCashOrders : BasePage
    {
        string[] dateFormats = new string[] { "M/d/yyyy", "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy", "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy" };
        public class VaultInfo
        {
            public int newVaultId = 0;
            public int existingVaultId = 0;
        }
        private VaultInfo CreateTempVaultLedger(int vaultId, decimal amount, int atmId, string transactionType, string desc, string vaultTrxnType, int orderId, SqlTransaction trxn)
        {
            VaultInfo vaultInfo = new VaultInfo();
            CcmsTempVaultLedger vaultLedger = new CcmsTempVaultLedger();
            vaultLedger.TransactionDate = DateTime.Now;
            vaultLedger.TransactionType = transactionType;
            vaultLedger.Description = desc;
            vaultLedger.LedgerAmount = amount;
            vaultLedger.PostedBy = 1;
            vaultLedger.VaultId = vaultId;
            vaultLedger.VaultTransactionType = vaultTrxnType;
            vaultLedger.AtmId = atmId;
            vaultLedger.OrderId = orderId;
            object result = ConnectionFactory.ExecuteScalar("select max(id) from ccms_temp_vault_ledger where vault_id = " + vaultId);
            if (result != DBNull.Value)
            {
                CcmsTempVaultLedger existingLedger = CcmsTempVaultLedger.LoadCcmsTempVaultLedgerByPk(int.Parse(result.ToString()));
                if (existingLedger != null)
                {
                    vaultInfo.existingVaultId = existingLedger.Id;
                    if (transactionType == "Cr")
                        vaultLedger.Balance = existingLedger.Balance + vaultLedger.LedgerAmount;
                    else
                        vaultLedger.Balance = existingLedger.Balance - vaultLedger.LedgerAmount;

                }

            }
            else
            {
                if (transactionType == "Cr")
                    vaultLedger.Balance = vaultLedger.LedgerAmount;
                else
                    vaultLedger.Balance = -vaultLedger.LedgerAmount;
            }

            vaultLedger.Save(trxn.Connection, trxn);
            vaultInfo.newVaultId = vaultLedger.Id;
            return vaultInfo;
        }

        private void CreateTempVaultLedgerDetail(VaultInfo vaultInfo, int qty, int vaultNoteTypeId, string trxnType, bool isContraEntry, SqlTransaction trxn)
        {
            CcmsTempVaultLedgerDetail vaultLedgerDetail = new CcmsTempVaultLedgerDetail();
            vaultLedgerDetail.LedgerId = vaultInfo.newVaultId;
            vaultLedgerDetail.Quantity = qty;
            vaultLedgerDetail.VaultNoteTypeId = vaultNoteTypeId;
            CcmsTempVaultLedgerDetail existingVaultLedgerDetail = CcmsTempVaultLedgerDetail.LoadCcmsTempVaultLedgerDetail(
                string.Format("ledger_id = {0} and vault_note_type_id={1}", vaultInfo.existingVaultId, vaultNoteTypeId));
            if (existingVaultLedgerDetail != null)
            {
                if (vaultLedgerDetail.Balance == null)
                    vaultLedgerDetail.Balance = 0;

                if (existingVaultLedgerDetail.Balance == null)
                    existingVaultLedgerDetail.Balance = 0;



                if (trxnType == "Cr")
                    vaultLedgerDetail.Balance = existingVaultLedgerDetail.Balance + qty;
                else if (trxnType == "Dr")
                    vaultLedgerDetail.Balance = existingVaultLedgerDetail.Balance - qty;
            }
            else
            {
                if (trxnType == "Cr")
                    vaultLedgerDetail.Balance = qty;
                else
                    vaultLedgerDetail.Balance = -qty;
            }
            if (!isContraEntry)
            {
                CcmsVaultNoteType ccmsVaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteTypeByPk(vaultNoteTypeId);
                if (vaultLedgerDetail.Balance <= ccmsVaultNoteType.ThrasholdQty)
                {
                    CcmsVault ccmsVault = CcmsVault.LoadCcmsVaultByPk(ccmsVaultNoteType.VaultId);
                    GenerateGeneralAlert((int)EnumAlertType.VaultLowBalanceThresholdReached, "Vault[" + ccmsVault.Title + "] Threshold reached for denomination " + ccmsVaultNoteType.DenominationName, vaultLedgerDetail.LedgerId, EntityType.VaultLedger, trxn, Event_Type.Warning);
                }
            }

            vaultLedgerDetail.Save(trxn.Connection, trxn);
        }
        public void GenerateGeneralAlert(int alertTypeID, string msg, int entityId, EntityType entityType, SqlTransaction trxn, Event_Type eventType)
        {
            LogableTask task = LogableTask.NewTask("GenerateGeneralAlert");
            try
            {

                GeneralAlert alert = new GeneralAlert();
                alert.GeneratedAt = DateTime.Now;
                alert.AlertTypeId = alertTypeID;
                alert.ExpirationTime = DateTime.Now.AddDays(((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).AlertExpirationTime.Value);

                //                alert.ExpirationTime = DateTime.Now.AddDays(int.Parse(ConfigurationManager.AppSettings["AlertExpirationTime"]));
                alert.AlertMsg = msg;
                alert.RetryRemaining = 10;
                alert.GenerateNotificationSent = false;
                alert.EntityId = entityId;
                alert.EntityType = entityType.ToString();
                alert.Save(trxn.Connection, trxn);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Alert added of type " + alertTypeID);
                GenerateIntegratedAlert(alertTypeID, msg, entityId, entityType,
                    eventType, null, trxn);
            }

            finally
            {
                task.EndTask();
            }
        }
        public void GenerateIntegratedAlert(int alertTypeID, string msg,
           int entityID, EntityType entityType, Event_Type eventType, int? ftpFileInfoId, SqlTransaction trxn)
        {
            LogableTask task = LogableTask.NewTask("GenerateIntegratedAlert");
            int orgID = -1;
            try
            {
                CcmsIntegratedAlert alert = new CcmsIntegratedAlert();
                alert.AlertTypeId = alertTypeID;
                alert.AlertType = AlertType.LoadAlertTypeByPk(alertTypeID).AlertTypeName;
                alert.EntityId = entityID;
                alert.EntityType = entityType.ToString();
                alert.AlertLevel = eventType.ToString();
                alert.AlertStatus = "Unread";
                alert.GeneratedAt = DateTime.Now;
                alert.AlertText = msg;
                alert.ExpirationTime = DateTime.Now.AddDays(((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).AlertExpirationTime.Value);
                alert.GenerateNotificationSent = false;
                alert.ResolveNotificationSent = false;
                if (alert.EntityType == EntityType.ATM.ToString())
                {
                    Atm atm = Atm.LoadAtmByPk(entityID);
                    alert.GenerateRetryRemaining = atm.RetryCountAlert;
                    alert.ResolveRetryRemaining = atm.RetryCountAlert;
                    orgID = GetOrganization(atm.RegionId);
                }
                else if (alert.EntityType == EntityType.Organization.ToString())
                {
                    alert.FtpFileInfoId = ftpFileInfoId;
                    FtpFileInfo ftpFileInfo = FtpFileInfo.LoadFtpFileInfoByPk(ftpFileInfoId.Value);
                    Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(ftpFileInfo.RegionId);
                    alert.GenerateRetryRemaining = region.RetryCountAlert; // add field in region table for this....
                    alert.ResolveRetryRemaining = region.RetryCountAlert;
                    orgID = FtpFileInfo.LoadFtpFileInfoByPk(ftpFileInfoId.Value).RegionId;
                }
                alert.ModuleType = "CURRENCY";
                alert.OrganizationId = orgID;

                if (trxn != null)
                    alert.Save(trxn.Connection, trxn);
                else
                    alert.Save();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Alert added of type " + alertTypeID);
            }

            finally
            {
                task.EndTask();
            }
        }
        public int GetOrganization(int region_id)
        {
            Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(region_id);
            if (region.IsOrganization)
            {
                return region.RegionId;
            }
            else
                return GetOrganization(region.ParentRegionId.Value);
        }

        //protected void Page_PreInit(object sender, EventArgs e)
        //{
        //    this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        //}


        //protected void refresh_ServerClick(object sender, EventArgs e)
        //{
        //    string regionID = Session[SessionVars.SelectedRegions.ToString()].ToString();
        //    if (regionID != null /*&& !regionID.Contains(",")*/
        //        && !regionID.StartsWith("1,"))
        //    {

        //        Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
        //        if (region.IsOrganization)
        //        {
        //            Label_SelectedOrganization.Text = region.RegionName;
        //            Session[SessionVars.SelectedRegions.ToString()] = region.RegionId.ToString();
        //        }
        //        DropDownList_Process.Items.Clear();
        //        OrganizationSettlementsProcess.OrganizationSettlementsProcessReader reader = OrganizationSettlementsProcess.ExecuteReader("organization_id=" + region.RegionId);
        //        while (reader.Read())
        //        {
        //            SettlementProcess process = SettlementProcess.LoadSettlementProcessByPk(reader.CurrentOrganizationSettlementsProcess.SettlementProcessId.Value);
        //            DropDownList_Process.Items.Add(new ListItem(process.SettlementProcessName, process.SettlementProcessId.ToString()));
        //        }
        //        DropDownList_Process.DataBind();
        //        if (DropDownList_Process.Items.Count>0)
        //            DropDownList_Process_Idx_Chg(this, null);
        //    }
        //    else
        //        Label_SelectedOrganization.Text = "";

        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");

           
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ImportCashOrders))
                        Response.Redirect("AccessDenied.aspx");
                    //refresh_ServerClick(this, null);
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An error occured while loading page [" + ex.Message + "]</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_Submit_Click");

            try
            {
                if (FileUpload_imac.HasFile)
                {
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                    string fileName = "";
                    if ((FileUpload_imac.PostedFile.FileName.Contains(".xlsx") || (FileUpload_imac.PostedFile.FileName.Contains(".xls"))))
                    {
                        if (FileUpload_imac.PostedFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || FileUpload_imac.PostedFile.ContentType == "application/octet-stream")
                        {

                            fileName = appSetting.TemporaryFolder + "\\" + FileUpload_imac.FileName;

                            File.WriteAllBytes(fileName, FileUpload_imac.FileBytes);

                            DataSet ds = GetDataSetFromSheet(fileName, TextBox_SheetName.Text);


                            //File.WriteAllBytes(appSetting.TemporaryFolder + "\\IMACFile_Copy.xls", FileUpload_imac.FileBytes);

                            //SaveSettlement(GetDataSetFromSheet(fileName, TextBox_SheetName.Text));
                            SaveCashOrders(fileName, TextBox_SheetName.Text);


                            File.Delete(fileName);

                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Cash orders uploaded successfully");
                            Session[SessionVars.noticeForMain.ToString()] = "Cash orders uploaded successfully";
                        }
                        else
                            throw new Exception("invalid file type uploaded on server");

                    }
                    else
                    {
                        Literal_error.Text = "<li>Please only select .xlsx and xls the cash order file to upload in the system</li>";
                    }

                }
                else
                    Literal_error.Text = "<li>Please select the cash order file to upload in the system</li>";

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An error occured while importing cash orders [" + ex.Message + "]</li>";
                if (ex.Message.Contains("invalid"))
                    throw;
            }
            finally
            {
                task.EndTask();
            }
        }

        /// <summary>
        /// Returns the set of data from columns that are selected by the user
        /// </summary>
        /// <param name="filepath">Complete path where IMAC sheet resides</param>
        /// <returns></returns>
        protected static DataSet GetDataSetFromSheet(string filepath, string sheetName)
        {
            OleDbCommand command = null;
            OleDbDataAdapter adapter = null;
            DataSet dsSheet = null;

            try
            {
                if (filepath.Contains("xlsx"))
                    command = new OleDbCommand(@"select * from [" + sheetName + "$]", new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;\""));
                else
                    command = new OleDbCommand(@"select *  from [" + sheetName + "$]", new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=\"Excel 8.0;\""));
                adapter = new OleDbDataAdapter(command);
                dsSheet = new DataSet();
                adapter.Fill(dsSheet);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read data from excel sheet. " + ex.Message);
            }
            finally
            {
                if (adapter != null)
                    adapter.Dispose();


            }
            return dsSheet;
        }

        /// <summary>
        /// Reads each row of IMAC dataset and adds, changes or removes the ATM
        /// </summary>
        /// <param name="ds">Dataset containing data of selected columns from IMAC sheet</param>
        protected void SaveCashOrders(string filePath, string sheetName)
        {
            LogableTask task = LogableTask.NewTask("SaveCashOrders");

            SqlCommand cmd = null;
            SqlTransaction transaction = null;
            string isRunningForADCB = System.Configuration.ConfigurationManager.AppSettings["isRunningForADCB"];
            try
            {
                //string[] parts = dsSheet.Tables[0].Rows[6][3].ToString().Split('#');

                //if (DropDownList_Process.SelectedItem.Value == "2")
                //{
                //    StringBuilder builder = new StringBuilder();
                //    Settlements.Settlements settlements = new Settlements.Settlements();
                //    int noOfSettlementsImported = settlements.ImportSettlements(filePath, sheetName, TextBox_VaultBalancingSheetName.Text, ((AppUser)Session[SessionVars.user.ToString()]).UserId,builder);
                //    if (noOfSettlementsImported > 0)
                //    {
                //        Session[SessionVars.notice.ToString()] = noOfSettlementsImported + " ATM Settlements imported Successfully!";
                //        Response.Redirect("~/ccms/ATMSettlementList.aspx");

                //    }
                //    else
                //        Literal_ok.Text = "Settlements alreadys exists for all the ATMs.";
                //} //For QIB use the below code.
                //else if (DropDownList_Process.SelectedItem.Value == "4")
                //{
                int importCount = 0;
                DateTime dateResult;
                CashOrders cashOrders = null;
                string atmTitle = null;
                DataSet result = GetDataSetFromSheet(filePath, sheetName);
                if (result.Tables[0].Rows.Count > 0)
                {
                    string tempDatetime = result.Tables[0].Rows[0][10].ToString();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "header date extracted : " + tempDatetime);

                    cmd = ConnectionFactory.GetNewCommand(true);
                    transaction = cmd.Connection.BeginTransaction();

                    for (int j = 5; j < result.Tables[0].Rows.Count; j++)
                    {
                        if (isRunningForADCB == "1")
                            atmTitle = "ADB0" + result.Tables[0].Rows[j][1];
                        else
                            atmTitle = result.Tables[0].Rows[j][1].ToString();

                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Processing atm : " + atmTitle);
                        Atm atm = Atm.LoadAtm("title ='" + atmTitle + "'");
                        if (atm != null)
                        {
                            cashOrders = CashOrders.LoadCashOrders("atm_id = " + atm.ATMId + "and cash_order_datetime=convert(datetime,'" + tempDatetime + "',101) and is_cancelled=0");
                            if (cashOrders == null)
                            {
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Cash order does not exists so creating new order ");
                                cashOrders = new CashOrders();
                                DateTime.TryParseExact(tempDatetime, dateFormats, null, System.Globalization.DateTimeStyles.None, out dateResult);
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "header date converted to  : " + dateResult);

                                cashOrders.CashOrderDatetime = dateResult;
                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                //Changes done on 15/Nov to read from current row instead of header date
                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                                if (result.Tables[0].Rows[j][10].ToString().Length == 0)
                                    //if (result.Tables[0].Rows[0][10].ToString().Length == 0)
                                    cashOrders.ReplenishmentDatetime = cashOrders.CashOrderDatetime.AddDays(1);
                                else
                                {
                                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    //Changes done on 15/Nov to read from current row instead of header date
                                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    if (result.Tables[0].Rows[j][11].ToString().Length > 0)
                                        //if (result.Tables[0].Rows[0][11].ToString().Length > 0)
                                        cashOrders.ReplenishmentDatetime = DateTime.ParseExact(result.Tables[0].Rows[j][10].ToString() + result.Tables[0].Rows[j][11].ToString(), "dd-MMMHH:mm", null);
                                    else
                                        cashOrders.ReplenishmentDatetime = DateTime.ParseExact(result.Tables[0].Rows[j][10].ToString(), "dd-MMM", null);

                                }
                                cashOrders.AtmId = atm.ATMId;
                                cashOrders.IsUploaded = false;
                                cashOrders.IsCancelled = false;
                                cashOrders.IsHold = false;
                                cashOrders.CreationTime = DateTime.Now;
                                cashOrders.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

                                AppSetting appSettings = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]);
                                cashOrders.DispatchTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, appSettings.CashOrderExecutionTime.Hour,
                                                          appSettings.CashOrderExecutionTime.Minute, appSettings.CashOrderExecutionTime.Second);


                                Avanza.CCMS.DAL.NoteSetType noteSet = Avanza.CCMS.DAL.NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);

                                cashOrders.Cassette1Denomination = noteSet.DenominationType1.Value;
                                cashOrders.Cassette2Denomination = noteSet.DenominationType2.Value;
                                cashOrders.Cassette3Denomination = noteSet.DenominationType3.Value;
                                cashOrders.Cassette4Denomination = noteSet.DenominationType4.Value;
                                cashOrders.Cassette5Denomination = noteSet.DenominationType5.Value;
                                cashOrders.Cassette6Denomination = noteSet.DenominationType6.Value;
                                cashOrders.Cassette7Denomination = noteSet.DenominationType7.Value;


                                if (noteSet.DenominationType1.Value != 0)
                                    cashOrders.Cassette1SuggestedNotes = int.Parse(result.Tables[0].Rows[j][4].ToString().Replace(",", "")) / noteSet.DenominationType1.Value;
                                else
                                    cashOrders.Cassette1SuggestedNotes = 0;

                                if (noteSet.DenominationType2.Value != 0)
                                    cashOrders.Cassette2SuggestedNotes = int.Parse(result.Tables[0].Rows[j][5].ToString().Replace(",", "")) / noteSet.DenominationType2.Value;
                                else
                                    cashOrders.Cassette2SuggestedNotes = 0;

                                if (noteSet.DenominationType3.Value != 0)
                                    cashOrders.Cassette3SuggestedNotes = int.Parse(result.Tables[0].Rows[j][6].ToString().Replace(",", "")) / noteSet.DenominationType3.Value;
                                else
                                    cashOrders.Cassette3SuggestedNotes = 0;

                                if (noteSet.DenominationType4.Value != 0)
                                    cashOrders.Cassette4SuggestedNotes = int.Parse(result.Tables[0].Rows[j][7].ToString().Replace(",", "")) / noteSet.DenominationType4.Value;
                                else
                                    cashOrders.Cassette4SuggestedNotes = 0;
                                cashOrders.Cassette5SuggestedNotes = 0;
                                cashOrders.Cassette6SuggestedNotes = 0;
                                cashOrders.Cassette7SuggestedNotes = 0;

                                cashOrders.CashOrderType = "RS";
                                cashOrders.OrderNumber = cashOrders.CashOrderType + "-" + atm.Title + "-" + cashOrders.CashOrderDatetime.ToString("ddMMyy");
                                cashOrders.Cassette1RemainingNotes = cashOrders.Cassette1SuggestedNotes;
                                cashOrders.Cassette2RemainingNotes = cashOrders.Cassette2SuggestedNotes;
                                cashOrders.Cassette3RemainingNotes = cashOrders.Cassette3SuggestedNotes;
                                cashOrders.Cassette4RemainingNotes = cashOrders.Cassette4SuggestedNotes;
                                cashOrders.Cassette5RemainingNotes = cashOrders.Cassette5SuggestedNotes;
                                cashOrders.Cassette6RemainingNotes = cashOrders.Cassette6SuggestedNotes;
                                cashOrders.Cassette7RemainingNotes = cashOrders.Cassette7SuggestedNotes;
                                cashOrders.Save(transaction.Connection, transaction);


                                CashOrderMonitoring cashOrderMonitoring = new Avanza.CCMS.DAL.CashOrderMonitoring();
                                cashOrderMonitoring.CashOrderDatetime = cashOrders.CashOrderDatetime;
                                cashOrderMonitoring.ReplenishmentDatetime = cashOrders.ReplenishmentDatetime;
                                cashOrderMonitoring.AtmId = atm.ATMId;
                                cashOrderMonitoring.CurrentOrderReceivedAt = DateTime.Now;
                                cashOrderMonitoring.CurrentOrderId = cashOrders.CashOrderId;


                                cashOrderMonitoring.CurrentOrderRemainingAmount = noteSet.DenominationType1.Value * cashOrders.Cassette1SuggestedNotes.Value
                               + noteSet.DenominationType2.Value * cashOrders.Cassette2SuggestedNotes.Value
                             + noteSet.DenominationType3.Value * cashOrders.Cassette3SuggestedNotes.Value
                               + noteSet.DenominationType4.Value * cashOrders.Cassette4SuggestedNotes.Value
                                + noteSet.DenominationType5.Value * cashOrders.Cassette5SuggestedNotes.Value
                                 + noteSet.DenominationType6.Value * cashOrders.Cassette6SuggestedNotes.Value
                                   + noteSet.DenominationType7.Value * cashOrders.Cassette7SuggestedNotes.Value;

                                cashOrderMonitoring.CurrentOrderSuggestedAmount = cashOrderMonitoring.CurrentOrderRemainingAmount;
                                cashOrderMonitoring.Save(transaction.Connection, transaction);

                                VaultAtm vaultAtm = VaultAtm.LoadVaultAtm("atm_id = " + cashOrders.AtmId);
                                if (vaultAtm != null)
                                {
                                    VaultInfo vaultInfo = null;
                                    if (Request.QueryString["cash_order_id"] != null)
                                    {
                                        CcmsTempVaultLedger tempVaultLedger = CcmsTempVaultLedger.LoadCcmsTempVaultLedger("order_id = " + cashOrders.CashOrderId);
                                        if (tempVaultLedger != null)
                                        {
                                            vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, tempVaultLedger.LedgerAmount, cashOrders.AtmId, "Cr", "Vault adjusted because order cancelled.Order ref # " + cashOrders.OrderNumber, "Adjustment Credit", -1, transaction);
                                            SqlCommand cmd1 = ConnectionFactory.GetNewCommand(false);
                                            cmd1.CommandText = "select quantity,vault_note_type_id from ccms_temp_vault_ledger_detail where ledger_id = " + tempVaultLedger.Id;
                                            System.Data.DataTable dt = new System.Data.DataTable();
                                            SqlDataAdapter adapter = new SqlDataAdapter(cmd1);
                                            adapter.Fill(dt);

                                            foreach (DataRow dr in dt.Rows)
                                            {
                                                int Quantity = int.Parse(dr["quantity"].ToString());
                                                CreateTempVaultLedgerDetail(vaultInfo, Quantity, int.Parse(dr["vault_note_type_id"].ToString()), "Cr", true, transaction);
                                            }
                                            ConnectionFactory.ExecuteQuery("delete general_alert where entity_id = " + tempVaultLedger.Id, transaction);
                                        }
                                    }

                                    vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, cashOrderMonitoring.CurrentOrderRemainingAmount, cashOrders.AtmId, "Dr", "Vault deducted with amount " + cashOrderMonitoring.CurrentOrderRemainingAmount + " order ref # " + cashOrders.OrderNumber, "Outbound", cashOrders.CashOrderId, transaction);

                                    if (cashOrders.Cassette1SuggestedNotes > 0)
                                    {
                                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSet.DenominationType1Title + "'");
                                        if (vaultNoteType != null)
                                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette1SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, transaction);
                                    }
                                    if (cashOrders.Cassette2SuggestedNotes > 0)
                                    {
                                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSet.DenominationType2Title + "'");
                                        if (vaultNoteType != null)
                                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette2SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, transaction);
                                    }
                                    if (cashOrders.Cassette3SuggestedNotes > 0)
                                    {
                                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSet.DenominationType3Title + "'");
                                        if (vaultNoteType != null)
                                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette3SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, transaction);
                                    }
                                    if (cashOrders.Cassette4SuggestedNotes > 0)
                                    {
                                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSet.DenominationType4Title + "'");
                                        if (vaultNoteType != null)
                                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette4SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, transaction);
                                    }
                                    if (cashOrders.Cassette5SuggestedNotes > 0)
                                    {
                                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSet.DenominationType5Title + "'");
                                        if (vaultNoteType != null)
                                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette5SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, transaction);
                                    }

                                    if (cashOrders.Cassette6SuggestedNotes > 0)
                                    {
                                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSet.DenominationType6Title + "'");
                                        if (vaultNoteType != null)
                                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette6SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, transaction);
                                    }
                                    if (cashOrders.Cassette7SuggestedNotes > 0)
                                    {
                                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSet.DenominationType7Title + "'");
                                        if (vaultNoteType != null)
                                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette7SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, transaction);
                                    }
                                }
                                else
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Vault is not defined for the atm =" + cashOrders.AtmId);


                                Utility.BuildAuditLog(string.Format("Cash order created for atm {0} for the day {1}", atm.Title, cashOrders.CashOrderDatetime.ToString("dd/MM/yyyy")), (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateCashOrders);
                                importCount++;
                            }
                            else
                            {
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Cash order already exists so ignoring this order");
                            }


                        }
                        else
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "atm does not exists:" + atmTitle);

                    }
                    transaction.Commit();
                    if (importCount > 0)
                        Literal_ok.Text = "Cash orders Imported successfully!";
                    else
                        Literal_ok.Text = "No Cash order Imported";
                }
                else
                    Literal_error.Text = "No rows found";

                //                } //For QIB use the below code.
                //else
                //{
                //    AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlement(
                //        string.Format("gl_no='{0}' and rep_datetime=convert(datetime,'{1}',103) and atm_site_and_number='{2}'",
                //        dsSheet.Tables[0].Rows[7][2], DateTime.ParseExact(dsSheet.Tables[0].Rows[8][3].ToString(), "dd.MM.yyyy", null).ToString("dd/MM/yyyy HH:mm:ss"),
                //        dsSheet.Tables[0].Rows[6][3].ToString()));

                //    if (atmSettlement == null)
                //    {
                //        connection = ConnectionFactory.GetNewConnection();
                //        connection.Open();
                //        transaction = connection.BeginTransaction();
                //        DateTime dt = DateTime.Now;
                //        atmSettlement = new AtmSettlement();
                //        atmSettlement.AtmSiteAndNumber = dsSheet.Tables[0].Rows[6][3].ToString();
                //        //atmSettlement.AtmNumber = parts[1];
                //        atmSettlement.GlNo = dsSheet.Tables[0].Rows[7][2].ToString();
                //        atmSettlement.RepDatetime = DateTime.ParseExact(dsSheet.Tables[0].Rows[8][3].ToString(), "dd.MM.yyyy", null);
                //        atmSettlement.SealNumber = dsSheet.Tables[0].Rows[9][2].ToString();



                //        atmSettlement.CashRepDenominationType1 = int.Parse(dsSheet.Tables[0].Rows[12][1].ToString());
                //        atmSettlement.CashRepCountersType1 = int.Parse(dsSheet.Tables[0].Rows[12][2].ToString());
                //        atmSettlement.CashRepDenominationType2 = int.Parse(dsSheet.Tables[0].Rows[13][1].ToString());
                //        atmSettlement.CashRepCountersType2 = int.Parse(dsSheet.Tables[0].Rows[13][2].ToString());
                //        atmSettlement.CashRepDenominationType3 = int.Parse(dsSheet.Tables[0].Rows[14][1].ToString());
                //        atmSettlement.CashRepCountersType3 = int.Parse(dsSheet.Tables[0].Rows[14][2].ToString());

                //        atmSettlement.CashReturnDenominationType1 = int.Parse(dsSheet.Tables[0].Rows[12][5].ToString());
                //        atmSettlement.CashReturnCountersType1 = int.Parse(dsSheet.Tables[0].Rows[12][6].ToString());
                //        atmSettlement.CashReturnDenominationType2 = int.Parse(dsSheet.Tables[0].Rows[13][5].ToString());
                //        atmSettlement.CashReturnCountersType2 = int.Parse(dsSheet.Tables[0].Rows[13][6].ToString());
                //        atmSettlement.CashReturnDenominationType3 = int.Parse(dsSheet.Tables[0].Rows[14][5].ToString());
                //        atmSettlement.CashReturnCountersType3 = int.Parse(dsSheet.Tables[0].Rows[14][6].ToString());

                //        atmSettlement.CashDispensedDenominationType1 = int.Parse(dsSheet.Tables[0].Rows[20][1].ToString());
                //        atmSettlement.CashDispensedCountersType1 = int.Parse(dsSheet.Tables[0].Rows[20][2].ToString());
                //        atmSettlement.CashDispensedDenominationType2 = int.Parse(dsSheet.Tables[0].Rows[21][1].ToString());
                //        atmSettlement.CashDispensedCountersType2 = int.Parse(dsSheet.Tables[0].Rows[21][2].ToString());
                //        atmSettlement.CashDispensedDenominationType3 = int.Parse(dsSheet.Tables[0].Rows[22][1].ToString());
                //        atmSettlement.CashDispensedCountersType3 = int.Parse(dsSheet.Tables[0].Rows[22][2].ToString());

                //        atmSettlement.CashRejectedDenominationType1 = int.Parse(dsSheet.Tables[0].Rows[20][5].ToString());
                //        atmSettlement.CashRejectedCountersType1 = int.Parse(dsSheet.Tables[0].Rows[20][6].ToString());
                //        atmSettlement.CashRejectedDenominationType2 = int.Parse(dsSheet.Tables[0].Rows[21][5].ToString());
                //        atmSettlement.CashRejectedCountersType2 = int.Parse(dsSheet.Tables[0].Rows[21][6].ToString());
                //        atmSettlement.CashRejectedDenominationType3 = int.Parse(dsSheet.Tables[0].Rows[22][5].ToString());
                //        atmSettlement.CashRejectedCountersType3 = int.Parse(dsSheet.Tables[0].Rows[22][6].ToString());

                //        atmSettlement.UploadDatetime = DateTime.Now;
                //        atmSettlement.UploadedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

                //        atmSettlement.Save(transaction.Connection, transaction);



                //        Atm atm = Atm.LoadAtm("gl_number='" + atmSettlement.GlNo + "'");
                //        if (atm != null)
                //        {
                //            UserTask userTask = new UserTask();
                //            userTask.AtmSettlementId = atmSettlement.AtmSettlementId;
                //            userTask.CreatedBy = atmSettlement.UploadedBy.Value;
                //            userTask.CreationTime = DateTime.Now;
                //            userTask.Status = ApprovalStatus.Pending.ToString();
                //            userTask.TaskTypeId = (int)EnumTaskType.ApproveSettlement;
                //            userTask.AtmId = atm.ATMId;
                //            userTask.Save(transaction.Connection, transaction);



                //            UserATMs.UserATMsReader reader = UserATMs.ExecuteReader("atm_id = " + atm.ATMId);
                //            try
                //            {
                //                while (reader.Read())
                //                {


                //                    UserTaskAssignment taskAssignment = new UserTaskAssignment();
                //                    taskAssignment.UserTaskId = userTask.UserTaskId;
                //                    taskAssignment.UserId = reader.CurrentUserATMs.UserId;
                //                    taskAssignment.Save(transaction.Connection, transaction);

                //                }
                //            }
                //            finally
                //            {
                //                if (reader != null)
                //                    reader.Close();
                //            }

                //        }
                //        else
                //        {
                //            UserTask userTask = new UserTask();
                //            userTask.AtmSettlementId = atmSettlement.AtmSettlementId;
                //            userTask.CreatedBy = atmSettlement.UploadedBy.Value;
                //            userTask.CreationTime = DateTime.Now;
                //            userTask.Status = ApprovalStatus.Pending.ToString();
                //            userTask.TaskTypeId = (int)EnumTaskType.ApproveSettlement;
                //            userTask.Save(transaction.Connection, transaction);

                //            UserTaskAssignment taskAssignment = new UserTaskAssignment();
                //            taskAssignment.UserTaskId = userTask.UserTaskId;
                //            taskAssignment.UserId = 2;
                //            taskAssignment.Save(transaction.Connection, transaction);
                //        }

                //        if (transaction != null)
                //            transaction.Commit();

                //        Literal_ok.Text = "ATM Settlement Imported Successfully.";
                //    }
                //    else
                //    {
                //        Literal_error.Text = "ATM Settlement already exists.";
                //    }
                //}
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                //LogableTask.LogMonoActivityTask("test", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex.Message + " " + ex.StackTrace);
                throw;// new Exception("Unable to save ATMS. " + ex.Message);
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Determines if the necessary columns (ChangeType, ATM-ID, Region ID, IP Address and ID4) in the row are valid
        /// </summary>
        /// <param name="row">Row of the dataset from IMAC sheet</param>
        /// <returns></returns>


        protected bool isValid(object value)
        {
            if (value == null)
                return false;
            if (value.ToString().Replace(" ", "").Equals(String.Empty))
                return false;
            return true;
        }


    }
}