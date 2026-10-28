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
    public partial class PostReplenishment : BasePage
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
                Page.SetFocus(TextBox_RepDate);

                if (Request.QueryString["replenishment_id"] == null)
                {
                    if (!CheckRight((int)Permissions.PostReplenishment))
                        Response.Redirect("AccessDenied.aspx");

                    if (Request.QueryString["summary_id"] != null)
                    {
                        Avanza.CCMS.DAL.Summary summary = Avanza.CCMS.DAL.Summary.LoadSummaryByPk(int.Parse(Request.QueryString["summary_id"]));
                        if (summary != null)
                        {
                            TextBox_RepDate.Text = summary.TrxnDatetime.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                    }
                    
                    refresh_ServerClick(this, EventArgs.Empty);


                }
                else
                {
                    if (!CheckRight((int)Permissions.EditReplenishment))
                        Response.Redirect("AccessDenied.aspx");

                    Avanza.CCMS.DAL.Replenishment rep = Avanza.CCMS.DAL.Replenishment.LoadReplenishmentByPk(int.Parse(Request.QueryString["replenishment_id"]));
                    TextBox_Cassette1.Text = rep.CashAdded1.ToString();
                    TextBox_Cassette2.Text = rep.CashAdded2.ToString();
                    TextBox_Cassette3.Text = rep.CashAdded3.ToString();
                    TextBox_Cassette4.Text = rep.CashAdded4.ToString();
                    CheckBox_IsSwap.Checked = rep.IsSwap;
                    TextBox_RepDate.Text = rep.RepDatetime.ToString("dd/MM/yyyy HH:mm:ss");
                    TextBox_Reason.Text = rep.Reason;
                    atm = Atm.LoadAtmByPk(rep.AtmId);
                    Label_SelectedATM.Text = atm.Title;

                    NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                    if (noteSetType == null)
                        throw new Exception("Note Set Type does not exists");

                    Label_Denom1.Text = noteSetType.DenominationType1.Value.ToString();
                    Label_Denom2.Text = noteSetType.DenominationType2.Value.ToString();
                    Label_Denom3.Text = noteSetType.DenominationType3.Value.ToString();
                    Label_Denom4.Text = noteSetType.DenominationType4.Value.ToString();
                    //Label_Denom5.Text = noteSetType.DenominationType5.Value.ToString();
                    //Label_Denom6.Text = noteSetType.DenominationType6.Value.ToString();
                    //Label_Denom7.Text = noteSetType.DenominationType7.Value.ToString();

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

        protected void Button_SaveUpdate_Click(object sender, EventArgs e)
        {
            SqlConnection conn =  null;
            SqlTransaction trxn =null;
            LogableTask task = LogableTask.NewTask("UpdateCashOrder");
            DateTime replenishmentDateTime;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trxn = conn.BeginTransaction();

                if (Label_SelectedATM.Text.Contains("Please, select an ATM"))
                    throw new Exception("ATM Not selected");

                replenishmentDateTime = DateTime.ParseExact(TextBox_RepDate.Text, "dd/MM/yyyy HH:mm:ss", null);
                if (replenishmentDateTime>DateTime.Now)
                    throw new Exception ("Cannot post replenishments for future date " + replenishmentDateTime.ToString("dd/MM/yyyy HH:mm:ss"));

                       
                Avanza.CCMS.DAL.Replenishment rep;

                atm = Atm.LoadAtm("title='" + Label_SelectedATM.Text + "'");
                NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                if (noteSetType == null)
                    throw new Exception("Note Set Type does not exists");


                if (Request.QueryString["replenishment_id"] == null)
                {
                    rep = new Avanza.CCMS.DAL.Replenishment();
                    rep.GeneratedAt = DateTime.Now;
                    rep.GeneratedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;                    
                }
                else
                {
                    rep = Avanza.CCMS.DAL.Replenishment.LoadReplenishmentByPk(int.Parse(Request.QueryString["replenishment_id"]));
                    rep.ModifiedDatetime = DateTime.Now;
                    rep.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;


                    //ConnectionFactory.ExecuteQuery("insert into replenishmentHistory select * from replenishment where replenishment_id = " + rep.ReplenishmentId);

                    //ReplenishmentHistory replenishmentHistory = new ReplenishmentHistory();
                    //replenishmentHistory.CashAdded1 = rep.CashAdded1;
                    //replenishmentHistory.CashAdded2 = rep.CashAdded2;
                    //replenishmentHistory.CashAdded3 = rep.CashAdded3;
                    //replenishmentHistory.CashAdded4 = rep.CashAdded4;
                    //replenishmentHistory.CashAdded5 = rep.CashAdded5;
                    //replenishmentHistory.CashAdded6 = rep.CashAdded6;
                    //replenishmentHistory.CashAdded7 = rep.CashAdded7;
                    //replenishmentHistory.AtmId = rep.AtmId;
                    //replenishmentHistory.CashOrderId = rep.CashOrderId;
                    //replenishmentHistory.GeneratedAt = rep.GeneratedAt;
                    //replenishmentHistory.IsSwap = rep.IsSwap;
                    //replenishmentHistory.IsUpdated = rep.IsUpdated;
                    //replenishmentHistory.ModifiedBy = rep.ModifiedBy;
                    //replenishmentHistory.ModifiedDatetime = rep.ModifiedDatetime;
                    //replenishmentHistory.RepDatetime = rep.RepDatetime;
                    //replenishmentHistory.ReplenishmentId = rep.ReplenishmentId;
                    //replenishmentHistory.RepStatus = rep.RepStatus;
                    //replenishmentHistory.TaskId = rep.TaskId;
                    //replenishmentHistory.GeneratedAt = rep.GeneratedAt;
                    //replenishmentHistory.GeneratedBy = rep.GeneratedBy;
                    //replenishmentHistory.Reason = rep.Reason;
                    //replenishmentHistory.Save();
                }

                

                if (TextBox_Cassette1.Text.Length > 0)
                    rep.CashAdded1 = int.Parse(TextBox_Cassette1.Text);
                else
                    rep.CashAdded1 = 0;


                if (TextBox_Cassette2.Text.Length > 0)
                    rep.CashAdded2 = int.Parse(TextBox_Cassette2.Text);
                else
                    rep.CashAdded2 = 0;


                if (TextBox_Cassette3.Text.Length > 0)
                    rep.CashAdded3 = int.Parse(TextBox_Cassette3.Text);
                else
                    rep.CashAdded3 = 0;

                if (TextBox_Cassette4.Text.Length > 0)
                    rep.CashAdded4 = int.Parse(TextBox_Cassette4.Text);
                else
                    rep.CashAdded4 = 0;

                rep.RepDatetime = replenishmentDateTime;
                rep.RepStatus = "Normal";
                rep.IsSwap = CheckBox_IsSwap.Checked;
                rep.AtmId = atm.ATMId;
                rep.Reason = TextBox_Reason.Text;
                rep.Save(trxn.Connection, trxn);
                
                Avanza.CCMS.DAL.CashOrders cashOrder = Avanza.CCMS.DAL.CashOrders.LoadCashOrders("atm_id=" + rep.AtmId +
                                " and replenishment_datetime>=convert(datetime,'" + rep.RepDatetime.ToString("dd/MM/yyyy") + "',103) " +
                                " and replenishment_datetime<=convert(datetime,'" + rep.RepDatetime.ToString("dd/MM/yyyy") + " 23:59:59',103) ");


                if (cashOrder == null)
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "cash order not found for atm : " + rep.AtmId
                         + " for the date " + rep.RepDatetime.ToString());

                else //if (cashOrder != null)
                {
                    if (rep.CashAdded1 != cashOrder.Cassette1SuggestedNotes ||
                        rep.CashAdded2 != cashOrder.Cassette2SuggestedNotes ||
                        rep.CashAdded3 != cashOrder.Cassette3SuggestedNotes ||
                        rep.CashAdded4 != cashOrder.Cassette4SuggestedNotes ||
                        rep.CashAdded5 != cashOrder.Cassette5SuggestedNotes ||
                        rep.CashAdded6 != cashOrder.Cassette6SuggestedNotes ||
                        rep.CashAdded7 != cashOrder.Cassette7SuggestedNotes)
                    {
                       GenerateIntegratedAlert((int)EnumAlertType.SuspiciousReplenishment,
                            rep.CashAdded1 +
                            "|" + rep.CashAdded2 + "|" + rep.CashAdded3 + "|" + rep.CashAdded4 + "|" +
                            rep.CashAdded5 + "|" + rep.CashAdded6 + "|" + rep.CashAdded7 + "|" +
                            cashOrder.Cassette1SuggestedNotes + "|" + cashOrder.Cassette2SuggestedNotes + "|" +
                            cashOrder.Cassette3SuggestedNotes + "|" + cashOrder.Cassette4SuggestedNotes + "|" +
                            cashOrder.Cassette5SuggestedNotes + "|" + cashOrder.Cassette6SuggestedNotes + "|" +
                            cashOrder.Cassette7SuggestedNotes,rep.AtmId, EntityType.ATM,Event_Type.Warning,null,trxn);

                        //GenerateTerminalAlert(ATM.ATMId, (int)EnumAlertType.SuspiciousReplenishment



                          //  , trxn, Event_Type.Information, taskID, replenishment.ReplenishmentId, "Replenishment");
                        //GenerateCCMSEvent(EventType.SuspiciousReplenishment.ToString(),
                        //    EventType.SuspiciousReplenishment.ToString(), Event_Type.Warning.ToString(), cashOrder.OrderNumber, EntityType.Order.ToString(), Actors.ATM.ToString(), Actors.CCMS.ToString(), trxn, null);
                    }
                    // GenerateTerminalAlert(ATMID, (int)EnumAlertType.ATMCounterMismatch, "ATM Counter Mismatched", trxn);
                    int remainingNotesInCassette1 = cashOrder.Cassette1SuggestedNotes.Value - rep.CashAdded1;
                    int remainingNotesInCassette2 = cashOrder.Cassette2SuggestedNotes.Value - rep.CashAdded2;
                    int remainingNotesInCassette3 = cashOrder.Cassette3SuggestedNotes.Value - rep.CashAdded3;
                    int remainingNotesInCassette4 = cashOrder.Cassette4SuggestedNotes.Value - rep.CashAdded4;
                    int remainingNotesInCassette5 = cashOrder.Cassette5SuggestedNotes.Value - rep.CashAdded5;
                    int remainingNotesInCassette6 = cashOrder.Cassette6SuggestedNotes.Value - rep.CashAdded6;
                    int remainingNotesInCassette7 = cashOrder.Cassette7SuggestedNotes.Value - rep.CashAdded7;

                    cashOrder.Cassette1RemainingNotes = (remainingNotesInCassette1 != cashOrder.Cassette1RemainingNotes ? remainingNotesInCassette1 : cashOrder.Cassette1RemainingNotes);
                    cashOrder.Cassette2RemainingNotes = (remainingNotesInCassette2 != cashOrder.Cassette2RemainingNotes ? remainingNotesInCassette2 : cashOrder.Cassette2RemainingNotes);
                    cashOrder.Cassette3RemainingNotes = (remainingNotesInCassette3 != cashOrder.Cassette3RemainingNotes ? remainingNotesInCassette3 : cashOrder.Cassette3RemainingNotes);
                    cashOrder.Cassette4RemainingNotes = (remainingNotesInCassette4 != cashOrder.Cassette4RemainingNotes ? remainingNotesInCassette4 : cashOrder.Cassette4RemainingNotes);
                    cashOrder.Cassette5RemainingNotes = (remainingNotesInCassette5 != cashOrder.Cassette5RemainingNotes ? remainingNotesInCassette5 : cashOrder.Cassette5RemainingNotes);
                    cashOrder.Cassette6RemainingNotes = (remainingNotesInCassette6 != cashOrder.Cassette6RemainingNotes ? remainingNotesInCassette6 : cashOrder.Cassette6RemainingNotes);
                    cashOrder.Cassette7RemainingNotes = (remainingNotesInCassette7 != cashOrder.Cassette7RemainingNotes ? remainingNotesInCassette7 : cashOrder.Cassette7RemainingNotes);
                    cashOrder.LastReplenishmentAt = rep.RepDatetime;
                    cashOrder.Save(trxn.Connection, trxn);

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "cash order updated for atm : " + rep.AtmId);


                    //CashOrderMonitoring cashOrderMonitoring = CashOrderMonitoring.LoadCashOrderMonitoring("current_order_id=" + cashOrder.CashOrderId);
                    Avanza.CCMS.DAL.CashOrderMonitoring cashOrderMonitoring = (cashOrder.CashOrderId > 0 ?
                        Avanza.CCMS.DAL.CashOrderMonitoring.LoadCashOrderMonitoring("current_order_id=" + cashOrder.CashOrderId)
                        : Avanza.CCMS.DAL.CashOrderMonitoring.LoadCashOrderMonitoring("atm_id=" + rep.AtmId +
                        " and replenishment_datetime>=convert(datetime,'" + rep.RepDatetime.ToString("dd/MM/yyyy") + "',103) " +
                        " and replenishment_datetime<=convert(datetime,'" + rep.RepDatetime.ToString("dd/MM/yyyy") + " 23:59:59',103) "));

                    if (cashOrderMonitoring == null)
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "cash order monitoring not found for atm : " + rep.AtmId
                             + " for the date " + rep.RepDatetime.ToString());

                    else //if (cashOrderMonitoring != null)
                    {
                        cashOrderMonitoring.CurrentOrderRemainingAmount = (decimal)(cashOrder.Cassette1RemainingNotes * noteSetType.DenominationType1 +
                            cashOrder.Cassette2RemainingNotes * noteSetType.DenominationType2 + cashOrder.Cassette3RemainingNotes * noteSetType.DenominationType3 +
                            cashOrder.Cassette4RemainingNotes * noteSetType.DenominationType4 + cashOrder.Cassette5RemainingNotes * noteSetType.DenominationType5 +
                            cashOrder.Cassette6RemainingNotes * noteSetType.DenominationType6 + cashOrder.Cassette7RemainingNotes * noteSetType.DenominationType7);
                        cashOrderMonitoring.CurrentOrderExecutedAt = rep.RepDatetime;
                        cashOrderMonitoring.Save(trxn.Connection, trxn);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "cash order monitoring updated for atm : " + rep.AtmId);


                    }

                }
                trxn.Commit();
                if (Request.QueryString["replenishment_id"] == null)
                {
                    Utility.BuildAuditLog(string.Format("Replenishment posted  for atm {0} for the day {1}", atm.Title, rep.RepDatetime.ToString("dd/MM/yyyy")), (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.PostReplenishment);
                    Session[SessionVars.notice.ToString()] = "Replenishment created successfully.";
                }
                else
                {
                    Utility.BuildAuditLog(string.Format("Cash order modified for atm {0} for the day {1}", atm.Title, rep.RepDatetime.ToString("dd/MM/yyyy")), (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditReplenishment);
                    Session[SessionVars.notice.ToString()] = "Replenishment updated successfully.";
                }

                Response.Redirect("CCMSReplenishments.aspx", false);
            }
            catch (Exception ex)
            {
                trxn.Rollback();
                // trxn.Rollback();
                Literal_error.Text = "<li>An error occured while saving replenishment." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn!=null)
                    conn.Close();

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
                    //Label_Denom5.Text = "";
                    //Label_Denom6.Text = "";
                    //Label_Denom7.Text = "";

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
                        //Label_Denom5.Text = "";
                        //Label_Denom6.Text = "";
                        //Label_Denom7.Text = "";
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
                        //Label_Denom5.Text = noteSetType.DenominationType5.Value.ToString();
                        //Label_Denom6.Text = noteSetType.DenominationType6.Value.ToString();
                        //Label_Denom7.Text = noteSetType.DenominationType7.Value.ToString();
                        //
                        Page.SetFocus(TextBox_RepDate);
                    }
                }
            }
        }
    }
}