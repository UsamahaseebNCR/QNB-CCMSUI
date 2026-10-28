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
using System.Diagnostics;
using System.Reflection;
using Avanza.iSuite.UI;
using System.Data.SqlClient;


namespace CCMSUI.CCMS
{
    public partial class EditCashOrders : BasePage
    {
        Atm atm = null;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                Page.SetFocus(TextBox_Cassette1);

                if (Request.QueryString["cash_order_id"] == null)
                {
                    if (!CheckRight((int)Permissions.CreateCashOrders))
                        Response.Redirect("AccessDenied.aspx");

                    TextBox_CashOrderDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    TextBox_RepDate.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm:ss");

                    refresh_ServerClick(this, EventArgs.Empty);


                }
                else
                {
                    if (!CheckRight((int)Permissions.ModifyCashOrders))
                        Response.Redirect("AccessDenied.aspx");

                    Avanza.CCMS.DAL.CashOrders cashOrders = Avanza.CCMS.DAL.CashOrders.LoadCashOrdersByPk(int.Parse(Request.QueryString["cash_order_id"]));
                    TextBox_Cassette1.Text = cashOrders.Cassette1SuggestedNotes.ToString();

                    TextBox_Cassette2.Text = cashOrders.Cassette2SuggestedNotes.ToString();
                    TextBox_Cassette3.Text = cashOrders.Cassette3SuggestedNotes.ToString();
                    TextBox_Cassette4.Text = cashOrders.Cassette4SuggestedNotes.ToString();
                    TextBox_Cassette5.Text = cashOrders.Cassette5SuggestedNotes.ToString();
                    TextBox_Cassette6.Text = cashOrders.Cassette6SuggestedNotes.ToString();
                    TextBox_Cassette7.Text = cashOrders.Cassette7SuggestedNotes.ToString();
                    atm = Atm.LoadAtmByPk(cashOrders.AtmId);
                    Label_SelectedATM.Text = atm.Title;
                    DropDownList_RepType.SelectedIndex = cashOrders.CashOrderType == "RS" ? 0 : 1;
                    Panel_Denom.Visible = true;
                    NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                    if (noteSetType == null)
                        throw new Exception("Note Set Type does not exists");

                    Label_Denom1.Text = noteSetType.DenominationType1.Value.ToString();
                    Label_Denom2.Text = noteSetType.DenominationType2.Value.ToString();
                    Label_Denom3.Text = noteSetType.DenominationType3.Value.ToString();
                    Label_Denom4.Text = noteSetType.DenominationType4.Value.ToString();
                    Label_Denom5.Text = noteSetType.DenominationType5.Value.ToString();
                    Label_Denom6.Text = noteSetType.DenominationType6.Value.ToString();
                    Label_Denom7.Text = noteSetType.DenominationType7.Value.ToString();

                    if (cashOrders.ReplenishmentDatetime.HasValue)
                        TextBox_RepDate.Text = cashOrders.ReplenishmentDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss");

                    TextBox_CashOrderDate.Text = cashOrders.CashOrderDatetime.ToString("dd/MM/yyyy HH:mm:ss");
                    if (cashOrders.IsHold.Value)
                        CheckBox_HoldCashOrder.Checked = cashOrders.IsHold.Value;
                }
            }
        }


        public static void GenerateCCMSEvent(string eventId, string eventName, string eventType, string entityId, string entityType,
           string source, string destination, SqlTransaction trxn)
        {
            CcmsEvent ccmsEvent = new CcmsEvent();
            ccmsEvent.EventId = eventId;
            ccmsEvent.EventName = eventName;
            ccmsEvent.EventType = eventType;
            ccmsEvent.EntityId = entityId;
            ccmsEvent.EntityType = entityType;
            ccmsEvent.Sender = source;
            ccmsEvent.Recipient = destination;

            if (trxn != null)
                ccmsEvent.Save(trxn.Connection, trxn);
            else
                ccmsEvent.Save();

        }

        private void SaveCashOrdersInEmitacDatabase(Avanza.CCMS.DAL.CashOrders cashOrder, Avanza.CCMS.DAL.CashOrderMonitoring cashOrderMonitoring, Atm atm, LogableTask task, SqlConnection conn, SqlTransaction trxn)
        {
            CcmsOrder ccmsOrder = null;

            if (Request.QueryString["cash_order_id"] == null)
            {
                ccmsOrder = new CcmsOrder();
                ccmsOrder.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                ccmsOrder.CreatedOn = DateTime.Now;
                ccmsOrder.IsDeleted = false;
                ccmsOrder.IsValidated = false;
            }
            else
            {
                ccmsOrder = CcmsOrder.LoadCcmsOrder("order_number='" + cashOrder.OrderNumber + "'");
                ccmsOrder.ModifiedOn = DateTime.Now;
                ccmsOrder.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

            }

            //int organizationId = AddEditATM.GetOrganization(atm.RegionId);
            //Region region = Region.LoadRegionByPk(organizationId);
            //CcmsOrganization ccmsOrganization = CcmsOrganization.LoadCcmsOrganization("region_id=" + region.RegionId);
            //if (ccmsOrganization == null)
            //    throw new Exception("Organization Code does not exists,organization_internal_id : " + organizationId);
            CcmsAtmNoteSet ccmsATMNoteSet = CcmsAtmNoteSet.LoadCcmsAtmNoteSet("atm_id=" + atm.ATMId);
            if (ccmsATMNoteSet == null)
                throw new Exception("ATM Note Set is not defined");
            CcmsOrgNoteSet ccmsOrgNoteSet = CcmsOrgNoteSet.LoadCcmsOrgNoteSetByPk(ccmsATMNoteSet.OrgNoteSetId.Value);

            if (ccmsOrgNoteSet == null)
                throw new Exception("Organization note set does not exists");

            ccmsOrder.AtmId = atm.ATMId;
            ccmsOrder.OrderDate = cashOrder.CashOrderDatetime;
            ccmsOrder.OrderNumber = cashOrder.OrderNumber;
            ccmsOrder.Status = "Pending";
            ccmsOrder.Source = "NCR";

             
            ccmsOrder.Save(conn, trxn);

            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Order inserted in ccms_order table.");
            ConnectionFactory.ExecuteQuery("delete ccms_order_detail where order_id=" + ccmsOrder.Id);

            CcmsOrderDetail ccmsOrderDetail = null;
            NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
            if (noteSetType == null)
                throw new Exception("note set type does not exists " + atm.NoteSetTypeId);

            String[] noteSetTypeArray = {noteSetType.DenominationType1Title,
                                        noteSetType.DenominationType2Title,
                                        noteSetType.DenominationType3Title,
        noteSetType.DenominationType4Title,
        noteSetType.DenominationType5Title,
        noteSetType.DenominationType6Title,
        noteSetType.DenominationType7Title};

            int[] cashOrderSuggestedNotesArray = {cashOrder.Cassette1SuggestedNotes.Value,
                                                     cashOrder.Cassette2SuggestedNotes.Value,
                                                     cashOrder.Cassette3SuggestedNotes.Value,
                                                         cashOrder.Cassette4SuggestedNotes.Value,
                                                             cashOrder.Cassette5SuggestedNotes.Value,
                                                                 cashOrder.Cassette6SuggestedNotes.Value,
                                                                     cashOrder.Cassette7SuggestedNotes.Value};




            for (int i = 0; i < 6; i++)
            {
                if (cashOrderSuggestedNotesArray[i] == 0)
                    continue;

                ccmsOrderDetail = new CcmsOrderDetail();
                ccmsOrderDetail.DenominationName = noteSetTypeArray[i];
                ccmsOrderDetail.Quantity = cashOrderSuggestedNotesArray[i];
                ccmsOrderDetail.OrderId = ccmsOrder.Id;
                CcmsOrgNoteSetItem ccmsOrgNoteSetItem = CcmsOrgNoteSetItem.LoadCcmsOrgNoteSetItem("denomination_name='" + ccmsOrderDetail.DenominationName + "' and org_note_set_id =" + ccmsOrgNoteSet.Id);
                if (ccmsOrgNoteSetItem == null)
                    throw new Exception("Organization note set item does not exists");
                ccmsOrderDetail.NoteSetItemId = ccmsOrgNoteSetItem.Id;
                ccmsOrderDetail.Save(conn, trxn);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Order detail inserted in ccms_order_table table.");
            }
            if (atm.MinAmountForNormalDelivery.HasValue)
            {
                if (cashOrderMonitoring.CurrentOrderSuggestedAmount < atm.MinAmountForNormalDelivery.Value)
                {
                    GenerateCCMSEvent(
                    EventType.OrderBelowThreshold.ToString(),
                    EventType.OrderBelowThreshold.ToString(),
                    Event_Type.Warning.ToString(),
                    cashOrder.OrderNumber.ToString(),
                    EntityType.Order.ToString(),
                    Actors.OPTICash.ToString(),
                    Actors.CCMS.ToString(),
                    trxn);
                }
            }



        }

        protected void Button_SaveUpdateCashOrder_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("UpdateCashOrder");
            SqlTransaction trxn = null;
            SqlConnection conn = null;

            try
            {

                if (Label_SelectedATM.Text.Contains("Please, select an ATM"))
                    throw new Exception("ATM Not selected");

                Avanza.CCMS.DAL.CashOrders cashOrders;

                atm = Atm.LoadAtm("title='" + Label_SelectedATM.Text + "'");
                NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                if (noteSetType == null)
                    throw new Exception("Note Set Type does not exists");


                if (Request.QueryString["cash_order_id"] == null)
                {
                    cashOrders = new Avanza.CCMS.DAL.CashOrders();
                    //cashOrders.CashOrderDatetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    cashOrders.AtmId = atm.ATMId;
                    cashOrders.IsUploaded = false;
                    cashOrders.IsCancelled = false;
                    cashOrders.IsHold = false;
                    cashOrders.CreationTime = DateTime.Now;
                    cashOrders.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                
                    AppSetting appSettings = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]);
                    cashOrders.DispatchTime = new DateTime(DateTime.Now.Year,
    DateTime.Now.Month, DateTime.Now.Day, appSettings.CashOrderExecutionTime.Hour,
appSettings.CashOrderExecutionTime.Minute, appSettings.CashOrderExecutionTime.Second);
                }
                else
                {
                    cashOrders = Avanza.CCMS.DAL.CashOrders.LoadCashOrdersByPk(int.Parse(Request.QueryString["cash_order_id"]));
                    cashOrders.ModificationTime = DateTime.Now;
                    cashOrders.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                
                }
                cashOrders.ReplenishmentDatetime = DateTime.ParseExact(TextBox_RepDate.Text, "dd/MM/yyyy HH:mm:ss", null);
                cashOrders.CashOrderDatetime = DateTime.ParseExact(TextBox_CashOrderDate.Text, "dd/MM/yyyy HH:mm:ss", null);
                cashOrders.IsHold = CheckBox_HoldCashOrder.Checked;
                cashOrders.Cassette1Denomination = noteSetType.DenominationType1.Value;
                cashOrders.Cassette2Denomination = noteSetType.DenominationType2.Value;
                cashOrders.Cassette3Denomination = noteSetType.DenominationType3.Value;
                cashOrders.Cassette4Denomination = noteSetType.DenominationType4.Value;
                cashOrders.Cassette5Denomination = noteSetType.DenominationType5.Value;
                cashOrders.Cassette6Denomination = noteSetType.DenominationType6.Value;
                cashOrders.Cassette7Denomination = noteSetType.DenominationType7.Value;


                if (TextBox_Cassette1.Text.Length > 0)
                    cashOrders.Cassette1SuggestedNotes = int.Parse(TextBox_Cassette1.Text);
                else
                    cashOrders.Cassette1SuggestedNotes = 0;


                if (TextBox_Cassette2.Text.Length > 0)
                    cashOrders.Cassette2SuggestedNotes = int.Parse(TextBox_Cassette2.Text);
                else
                    cashOrders.Cassette2SuggestedNotes = 0;


                if (TextBox_Cassette3.Text.Length > 0)
                    cashOrders.Cassette3SuggestedNotes = int.Parse(TextBox_Cassette3.Text);
                else
                    cashOrders.Cassette3SuggestedNotes = 0;

                if (TextBox_Cassette4.Text.Length > 0)
                    cashOrders.Cassette4SuggestedNotes = int.Parse(TextBox_Cassette4.Text);
                else
                    cashOrders.Cassette4SuggestedNotes = 0;

                if (TextBox_Cassette5.Text.Length > 0)
                    cashOrders.Cassette5SuggestedNotes = int.Parse(TextBox_Cassette5.Text);
                else
                    cashOrders.Cassette5SuggestedNotes = 0;
                if (TextBox_Cassette6.Text.Length > 0)
                    cashOrders.Cassette6SuggestedNotes = int.Parse(TextBox_Cassette6.Text);
                else
                    cashOrders.Cassette6SuggestedNotes = 0;

                if (TextBox_Cassette7.Text.Length > 0)
                    cashOrders.Cassette7SuggestedNotes = int.Parse(TextBox_Cassette7.Text);
                else
                    cashOrders.Cassette7SuggestedNotes = 0;

                cashOrders.CashOrderType = DropDownList_RepType.SelectedIndex == 0 ? "RS" : "UR";
                cashOrders.OrderNumber = cashOrders.CashOrderType + "-" + atm.Title + "-" + cashOrders.CashOrderDatetime.ToString("ddMMyy");
                cashOrders.Cassette1RemainingNotes = cashOrders.Cassette1SuggestedNotes;
                cashOrders.Cassette2RemainingNotes = cashOrders.Cassette2SuggestedNotes;
                cashOrders.Cassette3RemainingNotes = cashOrders.Cassette3SuggestedNotes;
                cashOrders.Cassette4RemainingNotes = cashOrders.Cassette4SuggestedNotes;
                cashOrders.Cassette5RemainingNotes = cashOrders.Cassette5SuggestedNotes;
                cashOrders.Cassette6RemainingNotes = cashOrders.Cassette6SuggestedNotes;
                cashOrders.Cassette7RemainingNotes = cashOrders.Cassette7SuggestedNotes;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trxn = conn.BeginTransaction();

      
                cashOrders.Save(trxn.Connection, trxn);
                Task cashOrderUploadTask = null;
                if (Request.QueryString["cash_order_id"] == null)
                {

                    cashOrderUploadTask = new Task();
                    cashOrderUploadTask.ATMId = atm.ATMId;
                    cashOrderUploadTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    cashOrderUploadTask.CreationTime = DateTime.Now;
                    cashOrderUploadTask.UploadTime = new DateTime(1900, 1, 1);
                    cashOrderUploadTask.Parsed = false;
                    cashOrderUploadTask.Status = DownloadStates.scheduled.ToString();
                    cashOrderUploadTask.CashOrderId = cashOrders.CashOrderId;
                    cashOrderUploadTask.BytesTransferred = 0;
                    cashOrderUploadTask.TaskTypeId = (int)EnumTaskType.CashOrderUpload;
                    cashOrderUploadTask.RetryRemaining = (byte)(atm.RetryCountCashOrderUpload);
                    cashOrderUploadTask.Save(trxn.Connection, trxn);


                }

                if (atm == null)
                    atm = Atm.LoadAtmByPk(cashOrders.AtmId);

                Avanza.CCMS.DAL.CashOrderMonitoring cashOrderMonitoring = null;
                if (Request.QueryString["cash_order_id"] == null)
                {
                    cashOrderMonitoring = new Avanza.CCMS.DAL.CashOrderMonitoring();
                    cashOrderMonitoring.AtmId = atm.ATMId;
                    cashOrderMonitoring.CurrentOrderReceivedAt = DateTime.Now;
                    cashOrderMonitoring.CurrentOrderId = cashOrderUploadTask.CashOrderId.Value;
                }
                else
                    cashOrderMonitoring = Avanza.CCMS.DAL.CashOrderMonitoring.LoadCashOrderMonitoring("current_order_id=" + cashOrders.CashOrderId);
                
                cashOrderMonitoring.CashOrderDatetime = cashOrders.CashOrderDatetime;
                cashOrderMonitoring.ReplenishmentDatetime = cashOrders.ReplenishmentDatetime;
                                
                cashOrderMonitoring.CurrentOrderRemainingAmount = noteSetType.DenominationType1.Value * cashOrders.Cassette1SuggestedNotes.Value
                                + noteSetType.DenominationType2.Value * cashOrders.Cassette2SuggestedNotes.Value
                              + noteSetType.DenominationType3.Value * cashOrders.Cassette3SuggestedNotes.Value
                                + noteSetType.DenominationType4.Value * cashOrders.Cassette4SuggestedNotes.Value
                                 + noteSetType.DenominationType5.Value * cashOrders.Cassette5SuggestedNotes.Value
                                  + noteSetType.DenominationType6.Value * cashOrders.Cassette6SuggestedNotes.Value
                                    + noteSetType.DenominationType7.Value * cashOrders.Cassette7SuggestedNotes.Value;

                cashOrderMonitoring.CurrentOrderSuggestedAmount = cashOrderMonitoring.CurrentOrderRemainingAmount;
                cashOrderMonitoring.Save(trxn.Connection, trxn);
                //SaveCashOrdersInEmitacDatabase(cashOrders, cashOrderMonitoring, atm, task, conn, trxn);


                VaultAtm vaultAtm = VaultAtm.LoadVaultAtm("atm_id = " + cashOrders.AtmId);
                if (vaultAtm != null)
                {
                    VaultInfo vaultInfo = null;
                    if (Request.QueryString["cash_order_id"] != null)
                    {
                        CcmsTempVaultLedger tempVaultLedger = CcmsTempVaultLedger.LoadCcmsTempVaultLedger("order_id = " + cashOrders.CashOrderId);
                        if (tempVaultLedger != null)
                        {
                            vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, tempVaultLedger.LedgerAmount, cashOrders.AtmId, "Cr", "Vault adjusted because order cancelled.Order ref # " + cashOrders.OrderNumber, "Adjustment Credit", -1, trxn);
                            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                            cmd.CommandText = "select quantity,vault_note_type_id from ccms_temp_vault_ledger_detail where ledger_id = " + tempVaultLedger.Id;
                            System.Data.DataTable dt = new System.Data.DataTable();
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            adapter.Fill(dt);

                            foreach (DataRow dr in dt.Rows)
                            {
                                int Quantity = int.Parse(dr["quantity"].ToString());
                                CreateTempVaultLedgerDetail(vaultInfo, Quantity, int.Parse(dr["vault_note_type_id"].ToString()), "Cr", true, trxn);
                            }
                            ConnectionFactory.ExecuteQuery("delete general_alert where entity_id = " + tempVaultLedger.Id, trxn);
                        }
                    }

                    vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, cashOrderMonitoring.CurrentOrderRemainingAmount, cashOrders.AtmId, "Dr", "Vault deducted with amount " + cashOrderMonitoring.CurrentOrderRemainingAmount + " order ref # " + cashOrders.OrderNumber, "Outbound", cashOrders.CashOrderId, trxn);

                    if (cashOrders.Cassette1SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType1Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette1SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette2SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType2Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette2SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette3SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType3Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette3SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette4SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType4Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette4SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette5SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType5Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette5SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }

                    if (cashOrders.Cassette6SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType6Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette6SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }
                    if (cashOrders.Cassette7SuggestedNotes > 0)
                    {
                        CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType7Title + "'");
                        if (vaultNoteType != null)
                            CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette7SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                    }





                }
                else
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Vault is not defined for the atm =" + cashOrders.AtmId);

                trxn.Commit();
                if (Request.QueryString["cash_order_id"] == null)
                {

                    Utility.BuildAuditLog(string.Format("Cash order created for atm {0} for the day {1}", atm.Title, cashOrders.CashOrderDatetime.ToString("dd/MM/yyyy")), (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateCashOrders);
                    Session[SessionVars.notice.ToString()] = "Cash order created successfully.";
                }
                else
                {
                    Utility.BuildAuditLog(string.Format("Cash order modified for atm {0} for the day {1}", atm.Title, cashOrders.CashOrderDatetime.ToString("dd/MM/yyyy")), (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyCashOrders);
                    Session[SessionVars.notice.ToString()] = "Cash order updated successfully.";
                }

                Response.Redirect("cashorders.aspx", false);
            }
            catch (Exception ex)
            {
                if (trxn!=null)
                trxn.Rollback();
                Literal_error.Text = "<li>An error occured while saving cash order." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

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

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            if (Request.QueryString["cash_order_id"] == null)
            {
                if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
          Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                {
                    Label_SelectedATM.Text = "Please, select an ATM";
                    Panel_Denom.Visible = false;
                    Label_Denom1.Text = "";
                    Label_Denom2.Text = "";
                    Label_Denom3.Text = "";
                    Label_Denom4.Text = "";
                    Label_Denom5.Text = "";
                    Label_Denom6.Text = "";
                    Label_Denom7.Text = "";

                }
                else
                {
                    if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                    {
                        Label_SelectedATM.Text = "<li>ATM not selected.</li>";
                        Panel_Denom.Visible = false;
                        Label_Denom1.Text = "";
                        Label_Denom2.Text = "";
                        Label_Denom3.Text = "";
                        Label_Denom4.Text = "";
                        Label_Denom5.Text = "";
                        Label_Denom6.Text = "";
                        Label_Denom7.Text = "";
                    }
                    else
                    {
                        atm = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString());
                        Label_SelectedATM.Text = atm.Title;
                        Panel_Denom.Visible = true;
                        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                        if (noteSetType == null)
                            throw new Exception("Note Set Type does not exists");

                        Label_Denom1.Text = noteSetType.DenominationType1.Value.ToString();
                        Label_Denom2.Text = noteSetType.DenominationType2.Value.ToString();
                        Label_Denom3.Text = noteSetType.DenominationType3.Value.ToString();
                        Label_Denom4.Text = noteSetType.DenominationType4.Value.ToString();
                        Label_Denom5.Text = noteSetType.DenominationType5.Value.ToString();
                        Label_Denom6.Text = noteSetType.DenominationType6.Value.ToString();
                        Label_Denom7.Text = noteSetType.DenominationType7.Value.ToString();
                        Page.SetFocus(TextBox_Cassette1);
                    }
                }
            }
        }
    }
}