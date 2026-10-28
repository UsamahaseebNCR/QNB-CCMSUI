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
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using Avanza.iSuite.UI;
using System.Text;
using System.Collections.Generic;

namespace CCMSUI.CCMS
{
    public partial class ShowSettlementData : BasePage
    {

        protected void Button_Correction_Click(object sender, EventArgs e)
        {
            SqlTransaction trxn = null;
            SqlCommand cmd = null;
            try
            {
                if (Button_Correction.Text == "Update")
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trxn = cmd.Connection.BeginTransaction();
                    cmd.Transaction = trxn;

                    Button_Correction.Text = "Correction";
                    DateTime atmRepDateTime = DateTime.ParseExact(Label_ATMClosedDate.Text + " " + Label_ATMClosedTime.Text, "dd-MMM-yy HH:mm", null);
                    Atm atm = Atm.LoadAtm("title='" + Label_Title.Text + "'");
                    NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                    SummaryCorrectionData summaryCorrectionData = SummaryCorrectionData.LoadSummaryCorrectionData("atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmRepDateTime.ToString("dd/MM/yyyy") + "',103) and rep_datetime<=convert(datetime,'" + atmRepDateTime.ToString("dd/MM/yyyy") + " 23:59:59',103) ");
                    if (summaryCorrectionData != null)
                    {
                        SummaryCorrectionDataHistory summaryCorrectionDataHistory = new SummaryCorrectionDataHistory();
                        summaryCorrectionDataHistory.AtmId = summaryCorrectionData.AtmId;

                        summaryCorrectionDataHistory.NotesRemaining1 = summaryCorrectionData.NotesRemaining1;
                        summaryCorrectionDataHistory.NotesRemaining2 = summaryCorrectionData.NotesRemaining2;
                        summaryCorrectionDataHistory.NotesRemaining3 = summaryCorrectionData.NotesRemaining3;
                        summaryCorrectionDataHistory.NotesRemaining4 = summaryCorrectionData.NotesRemaining4;

                        summaryCorrectionDataHistory.NotesDispensed1 = summaryCorrectionData.NotesDispensed1;
                        summaryCorrectionDataHistory.NotesDispensed2 = summaryCorrectionData.NotesDispensed2;
                        summaryCorrectionDataHistory.NotesDispensed3 = summaryCorrectionData.NotesDispensed3;
                        summaryCorrectionDataHistory.NotesDispensed4 = summaryCorrectionData.NotesDispensed4;

                        summaryCorrectionDataHistory.NotesRejected1 = summaryCorrectionData.NotesRejected1;
                        summaryCorrectionDataHistory.NotesRejected2 = summaryCorrectionData.NotesRejected2;
                        summaryCorrectionDataHistory.NotesRejected3 = summaryCorrectionData.NotesRejected3;
                        summaryCorrectionDataHistory.NotesRejected4 = summaryCorrectionData.NotesRejected4;

                        summaryCorrectionDataHistory.AtmId = atm.ATMId;
                        summaryCorrectionDataHistory.ModifiedDatetime = summaryCorrectionData.ModifiedDatetime;
                        summaryCorrectionDataHistory.ModifiedBy = summaryCorrectionData.ModifiedBy;
                        summaryCorrectionDataHistory.RepDatetime = summaryCorrectionData.RepDatetime;
                        summaryCorrectionDataHistory.AtmSettlementId = summaryCorrectionData.AtmSettlementId;
                        summaryCorrectionDataHistory.Save(trxn.Connection, trxn);
                    }
                    if (summaryCorrectionData == null)
                        summaryCorrectionData = new SummaryCorrectionData();
                    summaryCorrectionData.NotesRemaining1 = int.Parse(TextBox_CCMSLeft1Correction.Text);
                    summaryCorrectionData.NotesRemaining2 = int.Parse(TextBox_CCMSLeft2Correction.Text);
                    summaryCorrectionData.NotesRemaining3 = int.Parse(TextBox_CCMSLeft3Correction.Text);
                    summaryCorrectionData.NotesRemaining4 = int.Parse(TextBox_CCMSLeft4Correction.Text);

                    summaryCorrectionData.NotesDispensed1 = int.Parse(TextBox_CCMSDispensed1Correction.Text);
                    summaryCorrectionData.NotesDispensed2 = int.Parse(TextBox_CCMSDispensed2Correction.Text);
                    summaryCorrectionData.NotesDispensed3 = int.Parse(TextBox_CCMSDispensed3Correction.Text);
                    summaryCorrectionData.NotesDispensed4 = int.Parse(TextBox_CCMSDispensed4Correction.Text);

                    summaryCorrectionData.NotesRejected1 = int.Parse(TextBox_CCMSReject1Correction.Text);
                    summaryCorrectionData.NotesRejected2 = int.Parse(TextBox_CCMSReject2Correction.Text);
                    summaryCorrectionData.NotesRejected3 = int.Parse(TextBox_CCMSReject3Correction.Text);
                    summaryCorrectionData.NotesRejected4 = int.Parse(TextBox_CCMSReject4Correction.Text);


                    summaryCorrectionData.AtmId = atm.ATMId;
                    summaryCorrectionData.ModifiedDatetime = DateTime.Now;
                    summaryCorrectionData.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    summaryCorrectionData.RepDatetime = atmRepDateTime;
                    summaryCorrectionData.AtmSettlementId = int.Parse(Request.QueryString["sid"]);
                    summaryCorrectionData.Save(trxn.Connection, trxn);




                    if (Session["atmSettlement"] != null)
                    {
                        AtmSettlement atmSettlement = (AtmSettlement)Session["atmSettlement"];
                        string[] parts = TextBox_ATMClosedTimeCorrection.Text.Split(':');
                        if (parts.Length == 2)
                        {
                            if (atmSettlement.RepDatetime.Value.Hour != int.Parse(parts[0]) && atmSettlement.RepDatetime.Value.Minute != int.Parse(parts[1]))
                            {
                                atmSettlement.RepDatetime = new DateTime(atmSettlement.RepDatetime.Value.Year, atmSettlement.RepDatetime.Value.Month, atmSettlement.RepDatetime.Value.Day, int.Parse(parts[0]), int.Parse(parts[1]), 0);
                                atmSettlement.Save(trxn.Connection, trxn);
                            }

                            else if (atmSettlement.RepDatetime.Value.Hour != int.Parse(parts[0]))
                            {
                                atmSettlement.RepDatetime = new DateTime(atmSettlement.RepDatetime.Value.Year, atmSettlement.RepDatetime.Value.Month, atmSettlement.RepDatetime.Value.Day, int.Parse(parts[0]), atmSettlement.RepDatetime.Value.Minute, 0);
                                atmSettlement.Save(trxn.Connection, trxn);
                            }
                            else if (atmSettlement.RepDatetime.Value.Minute != int.Parse(parts[1]))
                            {
                                atmSettlement.RepDatetime = new DateTime(atmSettlement.RepDatetime.Value.Year, atmSettlement.RepDatetime.Value.Month, atmSettlement.RepDatetime.Value.Day, atmSettlement.RepDatetime.Value.Hour, int.Parse(parts[1]), 0);
                                atmSettlement.Save(trxn.Connection, trxn);
                            }

                        }

                        if (Session["lastSettlement"] != null)
                        {
                            AtmSettlement lastSettlement = (AtmSettlement)Session["lastSettlement"];
                            parts = TextBox_ATMOpenTimeCorrection.Text.Split(':');
                            if (parts.Length == 2)
                            {
                                if (lastSettlement.RepDatetime.Value.Hour != int.Parse(parts[0]) && lastSettlement.RepDatetime.Value.Minute != int.Parse(parts[1]))
                                {
                                    lastSettlement.RepDatetime = new DateTime(lastSettlement.RepDatetime.Value.Year, lastSettlement.RepDatetime.Value.Month, lastSettlement.RepDatetime.Value.Day, int.Parse(parts[0]), int.Parse(parts[1]), 0);
                                    lastSettlement.Save(trxn.Connection, trxn);
                                }
                                else if (lastSettlement.RepDatetime.Value.Hour != int.Parse(parts[0]))
                                {
                                    lastSettlement.RepDatetime = new DateTime(lastSettlement.RepDatetime.Value.Year, lastSettlement.RepDatetime.Value.Month, lastSettlement.RepDatetime.Value.Day, int.Parse(parts[0]), lastSettlement.RepDatetime.Value.Minute, 0);
                                    lastSettlement.Save(trxn.Connection, trxn);
                                }
                                else if (lastSettlement.RepDatetime.Value.Minute != int.Parse(parts[1]))
                                {
                                    lastSettlement.RepDatetime = new DateTime(lastSettlement.RepDatetime.Value.Year, lastSettlement.RepDatetime.Value.Month, lastSettlement.RepDatetime.Value.Day, lastSettlement.RepDatetime.Value.Hour, int.Parse(parts[1]), 0);
                                    lastSettlement.Save(trxn.Connection, trxn);
                                }
                            }
                        }
                        if (Session["replenishment"] != null)
                        {
                            //bool isCounterChanged = false;
                            Replenishment replenishment = (Replenishment)Session["replenishment"];
                            if (replenishment != null)
                            {
                                if (int.Parse(TextBox_CCMSAdded1Correction.Text) != replenishment.CashAdded1
                                     || int.Parse(TextBox_CCMSAdded2Correction.Text) != replenishment.CashAdded2
                                    || int.Parse(TextBox_CCMSAdded3Correction.Text) != replenishment.CashAdded3
                                    || int.Parse(TextBox_CCMSAdded4Correction.Text) != replenishment.CashAdded4)
                                //isCounterChanged = true;

                                //if (isCounterChanged)
                                {

                                    ReplenishmentHistory replenishmentHistory = new ReplenishmentHistory();
                                    replenishmentHistory.CashAdded1 = replenishment.CashAdded1;
                                    replenishmentHistory.CashAdded2 = replenishment.CashAdded2;
                                    replenishmentHistory.CashAdded3 = replenishment.CashAdded3;
                                    replenishmentHistory.CashAdded4 = replenishment.CashAdded4;
                                    replenishmentHistory.CashAdded5 = replenishment.CashAdded5;
                                    replenishmentHistory.CashAdded6 = replenishment.CashAdded6;
                                    replenishmentHistory.CashAdded7 = replenishment.CashAdded7;
                                    replenishmentHistory.AtmId = replenishment.AtmId;
                                    replenishmentHistory.CashOrderId = replenishment.CashOrderId;
                                    replenishmentHistory.GeneratedAt = replenishment.GeneratedAt;
                                    replenishmentHistory.IsSwap = replenishment.IsSwap;
                                    replenishmentHistory.IsUpdated = replenishment.IsUpdated;
                                    replenishmentHistory.ModifiedBy = replenishment.ModifiedBy;
                                    replenishmentHistory.ModifiedDatetime = replenishment.ModifiedDatetime;
                                    replenishmentHistory.RepDatetime = replenishment.RepDatetime;
                                    replenishmentHistory.ReplenishmentId = replenishment.ReplenishmentId;
                                    replenishmentHistory.RepStatus = replenishment.RepStatus;
                                    replenishmentHistory.TaskId = replenishment.TaskId;
                                    replenishmentHistory.Save(trxn.Connection, trxn);



                                    replenishment.CashAdded1 = int.Parse(TextBox_CCMSAdded1Correction.Text);
                                    replenishment.CashAdded2 = int.Parse(TextBox_CCMSAdded2Correction.Text);
                                    replenishment.CashAdded3 = int.Parse(TextBox_CCMSAdded3Correction.Text);
                                    replenishment.CashAdded4 = int.Parse(TextBox_CCMSAdded4Correction.Text);
                                    replenishment.ModifiedDatetime = DateTime.Now;
                                    replenishment.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                    replenishment.Save(trxn.Connection, trxn);
                                }
                            }
                        }
                    }
                    trxn.Commit();


                    Label_CCMSLeft1.Visible = true;
                    TextBox_CCMSLeft1Correction.Visible = false;
                    Label_CCMSLeft1.Text = TextBox_CCMSLeft1Correction.Text;

                    Label_CCMSLeft2.Visible = true;
                    TextBox_CCMSLeft2Correction.Visible = false;
                    Label_CCMSLeft2.Text = TextBox_CCMSLeft2Correction.Text;

                    Label_CCMSLeft3.Visible = true;
                    TextBox_CCMSLeft3Correction.Visible = false;
                    Label_CCMSLeft3.Text = TextBox_CCMSLeft3Correction.Text;

                    Label_CCMSLeft4.Visible = true;
                    TextBox_CCMSLeft4Correction.Visible = false;
                    Label_CCMSLeft4.Text = TextBox_CCMSLeft4Correction.Text;

                    ////Dispensed
                    Label_CCMSDispensed1.Visible = true;
                    TextBox_CCMSDispensed1Correction.Visible = false;
                    Label_CCMSDispensed1.Text = TextBox_CCMSDispensed1Correction.Text;

                    Label_CCMSDispensed2.Visible = true;
                    TextBox_CCMSDispensed2Correction.Visible = false;
                    Label_CCMSDispensed2.Text = TextBox_CCMSDispensed2Correction.Text;

                    Label_CCMSDispensed3.Visible = true;
                    TextBox_CCMSDispensed3Correction.Visible = false;
                    Label_CCMSDispensed3.Text = TextBox_CCMSDispensed3Correction.Text;

                    Label_CCMSDispensed4.Visible = true;
                    TextBox_CCMSDispensed4Correction.Visible = false;
                    Label_CCMSDispensed4.Text = TextBox_CCMSDispensed4Correction.Text;


                    Label_CCMSRejected1.Visible = true;
                    TextBox_CCMSReject1Correction.Visible = false;
                    Label_CCMSRejected1.Text = TextBox_CCMSReject1Correction.Text;

                    Label_CCMSRejected2.Visible = true;
                    TextBox_CCMSReject2Correction.Visible = false;
                    Label_CCMSRejected2.Text = TextBox_CCMSReject2Correction.Text;

                    Label_CCMSRejected3.Visible = true;
                    TextBox_CCMSReject3Correction.Visible = false;
                    Label_CCMSRejected3.Text = TextBox_CCMSReject3Correction.Text;

                    Label_CCMSRejected4.Visible = true;
                    TextBox_CCMSReject4Correction.Visible = false;
                    Label_CCMSRejected4.Text = TextBox_CCMSReject4Correction.Text;

                    Label_CCMSAdded1.Visible = true;
                    TextBox_CCMSAdded1Correction.Visible = false;
                    Label_CCMSAdded1.Text = TextBox_CCMSAdded1Correction.Text;

                    Label_CCMSAdded2.Visible = true;
                    TextBox_CCMSAdded2Correction.Visible = false;
                    Label_CCMSAdded2.Text = TextBox_CCMSAdded2Correction.Text;

                    Label_CCMSAdded3.Visible = true;
                    TextBox_CCMSAdded3Correction.Visible = false;
                    Label_CCMSAdded3.Text = TextBox_CCMSAdded3Correction.Text;

                    Label_CCMSAdded4.Visible = true;
                    TextBox_CCMSAdded4Correction.Visible = false;
                    Label_CCMSAdded4.Text = TextBox_CCMSAdded4Correction.Text;



                    Label_ATMOpenTime.Visible = true;
                    TextBox_ATMOpenTimeCorrection.Visible = false;
                    Label_ATMOpenTime.Text = TextBox_ATMOpenTimeCorrection.Text;

                    Label_ATMClosedTime.Visible = true;
                    TextBox_ATMClosedTimeCorrection.Visible = false;
                    Label_ATMClosedTime.Text = TextBox_ATMClosedTimeCorrection.Text;



                    Label_CCMSReturned1.Text = (int.Parse(Label_CCMSLeft1.Text) + int.Parse(Label_CCMSRejected1.Text)).ToString();
                    Label_CCMSReturned2.Text = (int.Parse(Label_CCMSLeft2.Text) + int.Parse(Label_CCMSRejected2.Text)).ToString();
                    Label_CCMSReturned3.Text = (int.Parse(Label_CCMSLeft3.Text) + int.Parse(Label_CCMSRejected3.Text)).ToString();
                    Label_CCMSReturned4.Text = (int.Parse(Label_CCMSLeft4.Text) + int.Parse(Label_CCMSRejected4.Text)).ToString();

                    Label_CCMSTotalLeftAmount.Text = (int.Parse(Label_CCMSReturned1.Text) * noteSetType.DenominationType1 +
                                                int.Parse(Label_CCMSReturned2.Text) * noteSetType.DenominationType2 +
                                                int.Parse(Label_CCMSReturned3.Text) * noteSetType.DenominationType3 +
                                                int.Parse(Label_CCMSReturned4.Text) * noteSetType.DenominationType4).ToString();


                    Label_DiffType1.Text = (int.Parse(Label_Returned1.Text) - int.Parse(Label_CCMSReturned1.Text)).ToString();
                    Label_DiffType2.Text = (int.Parse(Label_Returned2.Text) - int.Parse(Label_CCMSReturned2.Text)).ToString();
                    Label_DiffType3.Text = (int.Parse(Label_Returned3.Text) - int.Parse(Label_CCMSReturned3.Text)).ToString();
                    Label_DiffType4.Text = (int.Parse(Label_Returned4.Text) - int.Parse(Label_CCMSReturned4.Text)).ToString();

                    Label_DiffTotalAmount.Text = (int.Parse(Label_DiffType1.Text) * noteSetType.DenominationType1 +
                        int.Parse(Label_DiffType2.Text) * noteSetType.DenominationType2 +
                        int.Parse(Label_DiffType3.Text) * noteSetType.DenominationType3 +
                        int.Parse(Label_DiffType4.Text) * noteSetType.DenominationType4).ToString();
                }
                else
                {
                    Button_Correction.Text = "Update";
                    //Left
                    Label_CCMSLeft1.Visible = false;
                    TextBox_CCMSLeft1Correction.Visible = true;
                    TextBox_CCMSLeft1Correction.Text = Label_CCMSLeft1.Text;

                    Label_CCMSLeft2.Visible = false;
                    TextBox_CCMSLeft2Correction.Visible = true;
                    TextBox_CCMSLeft2Correction.Text = Label_CCMSLeft2.Text;

                    Label_CCMSLeft3.Visible = false;
                    TextBox_CCMSLeft3Correction.Visible = true;
                    TextBox_CCMSLeft3Correction.Text = Label_CCMSLeft3.Text;

                    Label_CCMSLeft4.Visible = false;
                    TextBox_CCMSLeft4Correction.Visible = true;
                    TextBox_CCMSLeft4Correction.Text = Label_CCMSLeft4.Text;

                    ////Dispensed
                    Label_CCMSDispensed1.Visible = false;
                    TextBox_CCMSDispensed1Correction.Visible = true;
                    TextBox_CCMSDispensed1Correction.Text = Label_CCMSDispensed1.Text;

                    Label_CCMSDispensed2.Visible = false;
                    TextBox_CCMSDispensed2Correction.Visible = true;
                    TextBox_CCMSDispensed2Correction.Text = Label_CCMSDispensed2.Text;

                    Label_CCMSDispensed3.Visible = false;
                    TextBox_CCMSDispensed3Correction.Visible = true;
                    TextBox_CCMSDispensed3Correction.Text = Label_CCMSDispensed3.Text;

                    Label_CCMSDispensed4.Visible = false;
                    TextBox_CCMSDispensed4Correction.Visible = true;
                    TextBox_CCMSDispensed4Correction.Text = Label_CCMSDispensed4.Text;
                    //Returned
                    //Label_CCMSReturned1.Visible = false;
                    //TextBox_CCMSReturned1Correction.Visible = true;
                    //TextBox_CCMSReturned1Correction.Text = Label_CCMSReturned1.Text;

                    //Label_CCMSReturned2.Visible = false;
                    //TextBox_CCMSReturned2Correction.Visible = true;
                    //TextBox_CCMSReturned2Correction.Text = Label_CCMSReturned2.Text;            

                    //Label_CCMSReturned3.Visible = false;
                    //TextBox_CCMSReturned3Correction.Visible = true;
                    //TextBox_CCMSReturned3Correction.Text = Label_CCMSReturned3.Text;

                    //Label_CCMSReturned4.Visible = false;
                    //TextBox_CCMSReturned4Correction.Visible = true;
                    //TextBox_CCMSReturned4Correction.Text = Label_CCMSReturned4.Text;
                    ////Rejected
                    Label_CCMSRejected1.Visible = false;
                    TextBox_CCMSReject1Correction.Visible = true;
                    TextBox_CCMSReject1Correction.Text = Label_CCMSRejected1.Text;

                    Label_CCMSRejected2.Visible = false;
                    TextBox_CCMSReject2Correction.Visible = true;
                    TextBox_CCMSReject2Correction.Text = Label_CCMSRejected2.Text;

                    Label_CCMSRejected3.Visible = false;
                    TextBox_CCMSReject3Correction.Visible = true;
                    TextBox_CCMSReject3Correction.Text = Label_CCMSRejected3.Text;

                    Label_CCMSRejected4.Visible = false;
                    TextBox_CCMSReject4Correction.Visible = true;
                    TextBox_CCMSReject4Correction.Text = Label_CCMSRejected4.Text;


                    //Last Replenishment Added Counters
                    Label_CCMSAdded1.Visible = false;
                    TextBox_CCMSAdded1Correction.Visible = true;
                    TextBox_CCMSAdded1Correction.Text = Label_CCMSAdded1.Text;

                    Label_CCMSAdded2.Visible = false;
                    TextBox_CCMSAdded2Correction.Visible = true;
                    TextBox_CCMSAdded2Correction.Text = Label_CCMSAdded2.Text;

                    Label_CCMSAdded3.Visible = false;
                    TextBox_CCMSAdded3Correction.Visible = true;
                    TextBox_CCMSAdded3Correction.Text = Label_CCMSAdded3.Text;

                    Label_CCMSAdded4.Visible = false;
                    TextBox_CCMSAdded4Correction.Visible = true;
                    TextBox_CCMSAdded4Correction.Text = Label_CCMSAdded4.Text;

                    Label_ATMOpenTime.Visible = false;
                    TextBox_ATMOpenTimeCorrection.Visible = true;
                    TextBox_ATMOpenTimeCorrection.Text = Label_ATMOpenTime.Text;

                    Label_ATMClosedTime.Visible = false;
                    TextBox_ATMClosedTimeCorrection.Visible = true;
                    TextBox_ATMClosedTimeCorrection.Text = Label_ATMClosedTime.Text;

                }
            }
            catch (Exception ex)
            {
                Literal_ok.Text = ex.Message;
                trxn.Rollback();
                LogableTask.LogMonoActivityTask("UpdateCounters", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTask("atm_settlement_id=" + base.Request.QueryString["sid"]);
            decimal amount = 0;
            decimal residualAmount = 0;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            bool isTrxnCommit = false;
            if (userTask != null)
            {
                try
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trxn = cmd.Connection.BeginTransaction();
                    cmd.Transaction = trxn;
                    List<CcmsVaultLedgerDetail> listVaultLedgerDetail = new List<CcmsVaultLedgerDetail>();
                    List<CcmsVaultLedgerDetail> listAddCountsVaultLedgerDetail = new List<CcmsVaultLedgerDetail>();
                    CcmsVaultLedgerDetail ccmsVaultLedgerDetail = null;
                    CcmsVaultLedgerDetail ccmsAddCountsVaultLedgerDetail = null;
                    CcmsVaultNoteType ccmsVaultNoteType = null;

                    Atm atm = Atm.LoadAtmByPk(userTask.AtmId.Value);
                    VaultAtm vaultAtm = VaultAtm.LoadVaultAtm("atm_id =" + atm.ATMId);
                    if (vaultAtm != null)
                    {
                        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                        string[] denominations = { noteSetType.DenominationType1Title,noteSetType.DenominationType2Title,noteSetType.DenominationType3Title,
                        noteSetType.DenominationType4Title,noteSetType.DenominationType5Title,noteSetType.DenominationType6Title,noteSetType.DenominationType7Title};

                        for (int k = 0; k < denominations.Length; k++)
                        {
                            if (denominations[k] != string.Empty)
                            {
                                ccmsVaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + denominations[k] + "'");
                                ccmsVaultLedgerDetail = new CcmsVaultLedgerDetail();
                                ccmsAddCountsVaultLedgerDetail = new CcmsVaultLedgerDetail();
                                ccmsVaultLedgerDetail.VaultNoteTypeId = ccmsVaultNoteType.Id;
                                ccmsAddCountsVaultLedgerDetail.VaultNoteTypeId = ccmsVaultNoteType.Id;


                                if (k == 0)
                                {
                                    if (ccmsVaultLedgerDetail != null)
                                        ccmsVaultLedgerDetail.Quantity = int.Parse(Label_CCMSReturned1.Text);

                                    if (ccmsAddCountsVaultLedgerDetail != null)
                                        ccmsAddCountsVaultLedgerDetail.Quantity = int.Parse(Label_notesAdded1.Text);

                                }
                                else if (k == 1)
                                {
                                    if (ccmsVaultLedgerDetail != null)
                                        ccmsVaultLedgerDetail.Quantity = int.Parse(Label_CCMSReturned2.Text);

                                    if (ccmsAddCountsVaultLedgerDetail != null)
                                        ccmsAddCountsVaultLedgerDetail.Quantity = int.Parse(Label_notesAdded2.Text);
                                }
                                else if (k == 2)
                                {

                                    if (ccmsVaultLedgerDetail != null)
                                        ccmsVaultLedgerDetail.Quantity = int.Parse(Label_CCMSReturned3.Text);

                                    if (ccmsAddCountsVaultLedgerDetail != null)
                                        ccmsAddCountsVaultLedgerDetail.Quantity = int.Parse(Label_notesAdded3.Text);

                                }
                                else if (k == 3)
                                {
                                    if (ccmsVaultLedgerDetail != null)
                                        ccmsVaultLedgerDetail.Quantity = int.Parse(Label_CCMSReturned4.Text);

                                    if (ccmsAddCountsVaultLedgerDetail != null)
                                        ccmsAddCountsVaultLedgerDetail.Quantity = int.Parse(Label_notesAdded4.Text);
                                }



                                if (ccmsAddCountsVaultLedgerDetail != null)
                                    amount += (decimal)(ccmsAddCountsVaultLedgerDetail.Quantity * int.Parse(denominations[k].Substring(3)));

                                listVaultLedgerDetail.Add(ccmsVaultLedgerDetail);
                                listAddCountsVaultLedgerDetail.Add(ccmsAddCountsVaultLedgerDetail);

                            }
                        }
                        residualAmount = decimal.Parse(Label_CCMSTotalLeftAmount.Text);
                        //int vaultLedgerId = 0;
                        VaultInfo vaultInfo = CreateVaultLedger(vaultAtm.VaultId, residualAmount, vaultAtm.AtmId, "Cr", "Amount " + residualAmount + " Returned from replenishment", "Inbound", trxn);
                        for (int l = 0; l < listVaultLedgerDetail.Count; l++)
                            CreateVaultLedgerDetail(vaultInfo, listVaultLedgerDetail[l].Quantity, listVaultLedgerDetail[l].VaultNoteTypeId, "Cr", trxn);


                        vaultInfo = CreateVaultLedger(vaultAtm.VaultId, amount, vaultAtm.AtmId, "Dr", "Amount " + amount + " deducted from vault", "Outbound", trxn);
                        for (int l = 0; l < listAddCountsVaultLedgerDetail.Count; l++)
                        {
                            CreateVaultLedgerDetail(vaultInfo, listAddCountsVaultLedgerDetail[l].Quantity, listAddCountsVaultLedgerDetail[l].VaultNoteTypeId, "Dr", trxn);
                        }


                    }
                    AtmSettlementOtherDetails atmSettlementOtherDetails = new AtmSettlementOtherDetails();
                    atmSettlementOtherDetails.AtmSettlementId = int.Parse(Request.QueryString["sid"]);
                    atmSettlementOtherDetails.Comments = TextBox_NCRComments.Text;
                    atmSettlementOtherDetails.NoOfReplenishment = int.Parse(TextBox_NoOfReplenishments.Text);
                    atmSettlementOtherDetails.Inventory = TextBox_Inventory.Text;
                    atmSettlementOtherDetails.Save(trxn.Connection, trxn);

                    userTask.Status = ((ApprovalStatus)1).ToString();
                    userTask.ResolutionTime = DateTime.Now;
                    userTask.ResolvedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    userTask.Save(trxn.Connection, trxn);
                    trxn.Commit();
                    isTrxnCommit = true;
                    this.Session[SessionVars.notice.ToString()] = "Settlement Approved Successfully";
                    base.Response.Redirect("~/ccms/UserTask.aspx");

                }
                catch (Exception ex)
                {
                    Literal_ok.Text = ex.Message;
                    LogableTask.LogMonoActivityTask("ApproveSettlement", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    if (!isTrxnCommit)
                        trxn.Rollback();
                }
                finally
                {
                    if (cmd != null)
                        if (cmd.Connection != null)
                            cmd.Connection.Close();
                }
            }

        }
        private VaultInfo CreateVaultLedger(int vaultId, decimal amount, int atmId, string transactionType, string desc, string vaultTrxnType, SqlTransaction trxn)
        {
            VaultInfo vaultInfo = new VaultInfo();
            CcmsVaultLedger vaultLedger = new CcmsVaultLedger();
            vaultLedger.TransactionDate = DateTime.Now;
            vaultLedger.TransactionType = transactionType;
            vaultLedger.Description = desc;
            vaultLedger.LedgerAmount = amount;
            vaultLedger.PostedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
            vaultLedger.VaultId = vaultId;
            vaultLedger.VaultTransactionType = vaultTrxnType;
            vaultLedger.AtmId = atmId;
            vaultLedger.AtmSettlementId = int.Parse(Request.QueryString["sid"]);
            object result = ConnectionFactory.ExecuteScalar("select max(id) from ccms_vault_ledger where vault_id = " + vaultId);
            if (result != DBNull.Value)
            {
                CcmsVaultLedger existingLedger = CcmsVaultLedger.LoadCcmsVaultLedgerByPk(int.Parse(result.ToString()));
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
                vaultLedger.Balance = vaultLedger.LedgerAmount;

            vaultLedger.Save(trxn.Connection, trxn);
            vaultInfo.newVaultId = vaultLedger.Id;
            return vaultInfo;
        }

        public class VaultInfo
        {
            public int newVaultId = 0;
            public int existingVaultId = 0;
        }
        private void CreateVaultLedgerDetail(VaultInfo vaultInfo, int qty, int vaultNoteTypeId, string trxnType, SqlTransaction trxn)
        {
            CcmsVaultLedgerDetail vaultLedgerDetail = new CcmsVaultLedgerDetail();
            vaultLedgerDetail.LedgerId = vaultInfo.newVaultId;
            vaultLedgerDetail.Quantity = qty;
            vaultLedgerDetail.VaultNoteTypeId = vaultNoteTypeId;
            CcmsVaultLedgerDetail existingVaultLedgerDetail = CcmsVaultLedgerDetail.LoadCcmsVaultLedgerDetail(
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
            vaultLedgerDetail.Save(trxn.Connection, trxn);
        }


        protected void Button_AddNewComments_click(object sender, EventArgs e)
        {
            if (this.TextBox_NewComments.Text.Trim().Length > 0)
            {
                AtmSettlement settlement = AtmSettlement.LoadAtmSettlementByPk(int.Parse(Session["settlementID"].ToString()));
                Avanza.CCMS.DAL.UserTask task = Avanza.CCMS.DAL.UserTask.LoadUserTask("atm_settlement_id=" + settlement.AtmSettlementId);
                UserTaskComments comments = new UserTaskComments();
                comments.Comments = this.TextBox_NewComments.Text;
                comments.UserTaskId = task.UserTaskId;
                comments.CreationTime = DateTime.Now;
                comments.UserId = ((AppUser)this.Session[SessionVars.user.ToString()]).UserId;
                comments.Save();
                this.TextBox_NewComments.Text = "";
            }
            this.ReloadComments(int.Parse(Session["settlementID"].ToString()));
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("~/ccms/UserTask.aspx");
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTask("atm_settlement_id=" + base.Request.QueryString["sid"]);
            if (userTask != null)
            {
                userTask.Status = (((ApprovalStatus)2).ToString());
                userTask.Save();
                this.Session[SessionVars.notice.ToString()] = "Settlement Rejected Successfully";
                base.Response.Redirect("~/ccms/UserTask.aspx");
            }
        }

        private void PopulateGrid(AtmSettlement atmSettlement, Avanza.CCMS.DAL.NoteSetType noteSetType)
        {
            //Find denomination Index based on net set type. Note set type values = 500 
            int index = 0;
            int[] noteSetTypeArray = new int[] {
                (noteSetType.DenominationType1Title.Length>0?int.Parse(noteSetType.DenominationType1Title.Substring(3)):0),
                (noteSetType.DenominationType2Title.Length>0?int.Parse(noteSetType.DenominationType2Title.Substring(3)):0),
                (noteSetType.DenominationType3Title.Length>0?int.Parse(noteSetType.DenominationType3Title.Substring(3)):0),
                (noteSetType.DenominationType4Title.Length>0?int.Parse(noteSetType.DenominationType4Title.Substring(3)):0)
             };

            int[] settlementNoteSetTypeArray = new int[] {
                (atmSettlement.CashRepDenominationType1.HasValue?atmSettlement.CashRepDenominationType1.Value:0),
                (atmSettlement.CashRepDenominationType2.HasValue?atmSettlement.CashRepDenominationType2.Value:0),
                (atmSettlement.CashRepDenominationType3.HasValue?atmSettlement.CashRepDenominationType3.Value:0),
                (atmSettlement.CashRepDenominationType4.HasValue?atmSettlement.CashRepDenominationType4.Value:0),
             };

            //Sort settlement array as per note set type.



            Label[] lblArray = new Label[] {
            Label_CITAdded1,Label_CITDispensed11,Label_CITReturned11,Label_CITRejected1,
            Label_CITAdded2,Label_CITDispensed2,Label_CITReturned2,Label_CITRejected2,
            Label_CITAdded3,Label_CITDispensed3,Label_CITReturned3,Label_CITRejected3,
            Label_CITAdded4,Label_CITDispensed4,Label_CITReturned4,Label_CITRejected4
             };

            double added = 0;
            double dispensed = 0;
            double returned = 0;
            double rejected = 0;


            for (int i = 0; i < noteSetTypeArray.Length; i++)
            {
                if (noteSetTypeArray[i] == 0) continue;
                int j = i * 4;

                index = Array.IndexOf(settlementNoteSetTypeArray, noteSetTypeArray[i]);
                if (index == 0)
                {
                    if (atmSettlement.CashRepCountersType1.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashRepCountersType1.Value.ToString();
                        added += atmSettlement.CashRepCountersType1.Value * noteSetTypeArray[i];

                    }
                    else
                        j++;

                    if (atmSettlement.CashDispensedCountersType1.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashDispensedCountersType1.Value.ToString();
                        dispensed += atmSettlement.CashDispensedCountersType1.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;
                    if (atmSettlement.CashReturnCountersType1.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashReturnCountersType1.Value.ToString();
                        returned += atmSettlement.CashReturnCountersType1.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashRejectedCountersType1.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashRejectedCountersType1.Value.ToString();
                        rejected += atmSettlement.CashRejectedCountersType1.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;


                }
                else if (index == 1)
                {
                    if (atmSettlement.CashRepCountersType2.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashRepCountersType2.Value.ToString();
                        added += atmSettlement.CashRepCountersType2.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashDispensedCountersType2.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashDispensedCountersType2.Value.ToString();
                        dispensed += atmSettlement.CashDispensedCountersType2.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashReturnCountersType2.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashReturnCountersType2.Value.ToString();
                        returned += atmSettlement.CashReturnCountersType2.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashRejectedCountersType2.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashRejectedCountersType2.Value.ToString();
                        rejected += atmSettlement.CashRejectedCountersType2.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;


                }
                else if (index == 2)
                {
                    if (atmSettlement.CashRepCountersType3.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashRepCountersType3.Value.ToString();
                        added += atmSettlement.CashRepCountersType3.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashDispensedCountersType3.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashDispensedCountersType3.Value.ToString();
                        dispensed += atmSettlement.CashDispensedCountersType3.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashReturnCountersType3.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashReturnCountersType3.Value.ToString();
                        returned += atmSettlement.CashReturnCountersType3.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashRejectedCountersType3.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashRejectedCountersType3.Value.ToString();
                        rejected += atmSettlement.CashRejectedCountersType3.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                }
                else if (index == 3)
                {
                    if (atmSettlement.CashRepCountersType4.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashRepCountersType4.Value.ToString();
                        added += atmSettlement.CashRepCountersType4.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;
                    if (atmSettlement.CashDispensedCountersType4.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashDispensedCountersType4.Value.ToString();
                        dispensed += atmSettlement.CashDispensedCountersType4.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashReturnCountersType4.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashReturnCountersType4.Value.ToString();
                        returned += atmSettlement.CashReturnCountersType4.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                    if (atmSettlement.CashRejectedCountersType4.HasValue)
                    {
                        lblArray[j++].Text = atmSettlement.CashRejectedCountersType4.Value.ToString();
                        rejected += atmSettlement.CashRejectedCountersType4.Value * noteSetTypeArray[i];
                    }
                    else
                        j++;

                }
                Label_CITTotalAdded.Text = added.ToString();
                Label_CITTotalDispensed.Text = dispensed.ToString();
                Label_CITTotalRejected.Text = rejected.ToString();
                Label_CITTotalReturned.Text = returned.ToString();

                if (added - dispensed < returned + rejected)
                    Label_CITATMStatus.Text = "EXCESS";
                else if (added - dispensed > returned + rejected)
                    Label_CITATMStatus.Text = "SHORT";
                else
                    Label_CITATMStatus.Text = "BALANCED";


            }


        }


        //        private void GetEJDispensedAmount(int atmID, NoteSetType noteSetType)
        //        {
        //            SqlConnection conn = ConnectionFactory.GetNewConnection();

        //            try
        //            {
        //                conn.Open();
        //                SqlCommand cmd = conn.CreateCommand();
        //                cmd.CommandText = "select max(rep_datetime) from ej_parsed_replenishments where atm_id=" + atmID + " and rep_datetime<convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).AddDays(-1).ToString("dd/MM/yyyy") + "',103)";
        //                object repDate = cmd.ExecuteScalar();
        //                if (repDate != DBNull.Value)
        //                {

        //                    cmd.CommandText = "select max(rep_datetime) from ej_parsed_replenishments where atm_id=" + atmID +
        //                        " and rep_datetime>=convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy") + "',103)" +
        //                        " and rep_datetime<=convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy") + " 23:59:59',103)";

        //                    object currentRepDate = cmd.ExecuteScalar();

        //                    cmd.CommandText = @"select sum(notes_dispensed_type1),
        //                                                           sum(notes_dispensed_type2),
        //                                                           sum(notes_dispensed_type3),
        //                                                           sum(notes_dispensed_type4)
        //                                            from ej_parsed_transactions 
        //                                            where ej_parsed_transactions.atm_id = " + atmID +
        //                                        "and trxn_datetime>=convert(datetime,'" + DateTime.Parse(repDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss") + "',103) " +
        //                                        " and trxn_datetime<=convert(datetime,'" + DateTime.Parse(currentRepDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss") + "',103) ";


        //                    SqlDataReader reader = cmd.ExecuteReader();
        //                    while (reader.Read())
        //                    {
        //                        //Label_EJSummaryDispensed1.Text = reader[0].ToString();
        //                        //Label_EJSummaryDispensed2.Text = reader[1].ToString();
        //                        //Label_EJSummaryDispensed3.Text = reader[2].ToString();
        //                        //Label_EJSummaryDispensed4.Text = reader[3].ToString();

        //                        //Label_EJSummaryTotalDispensed.Text = (int.Parse(reader[0].ToString()) * noteSetType.DenominationType1 +
        //                        //    int.Parse(reader[1].ToString()) * noteSetType.DenominationType2 +
        //                        //    int.Parse(reader[2].ToString()) * noteSetType.DenominationType3 +
        //                        //    int.Parse(reader[3].ToString()) * noteSetType.DenominationType4).ToString();
        //                    }
        //                    reader.Close();

        //                }
        //            }
        //            finally
        //            {
        //                if (conn != null)
        //                    conn.Close();
        //            }

        //        }



        //        private void GetDispensedAmount(int atmID, NoteSetType noteSetType)
        //        {
        //            SqlConnection conn = ConnectionFactory.GetNewConnection();

        //            try
        //            {
        //                conn.Open();
        //                SqlCommand cmd = conn.CreateCommand();
        //                cmd.CommandText = "select max(rep_datetime) from replenishment where atm_id=" + atmID + " and rep_datetime<convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).AddDays(-1).ToString("dd/MM/yyyy") + "',103)";
        //                object repDate = cmd.ExecuteScalar();
        //                if (repDate != DBNull.Value)
        //                {

        //                    cmd.CommandText = "select max(rep_datetime) from replenishment where atm_id=" + atmID +
        //                        " and rep_datetime>=convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy") + "',103)" +
        //                        " and rep_datetime<=convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy") + " 23:59:59',103)";

        //                    object currentRepDate = cmd.ExecuteScalar();

        //                    cmd.CommandText = @"select sum(cash_dispensed1),
        //                                                           sum(cash_dispensed2),
        //                                                           sum(cash_dispensed3),
        //                                                           sum(cash_dispensed4)
        //                                            from parsed_transaction 
        //                                            where parsed_transaction.atm_id = " + atmID +
        //                                        "and trxn_datetime>=convert(datetime,'" + DateTime.Parse(repDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss") + "',103) " +
        //                                        " and trxn_datetime<=convert(datetime,'" + DateTime.Parse(currentRepDate.ToString()).ToString("dd/MM/yyyy HH:mm:ss") + "',103) ";


        //                    SqlDataReader reader = cmd.ExecuteReader();
        //                    while (reader.Read())
        //                    {
        //                        Label_Dispensed1.Text = reader[0].ToString();
        //                        Label_Dispensed2.Text = reader[1].ToString();
        //                        Label_Dispensed3.Text = reader[2].ToString();
        //                        Label_Dispensed4.Text = reader[3].ToString();

        //                        Label_TotalDispensed.Text = (int.Parse(reader[0].ToString()) * noteSetType.DenominationType1 +
        //                            int.Parse(reader[1].ToString()) * noteSetType.DenominationType2 +
        //                            int.Parse(reader[2].ToString()) * noteSetType.DenominationType3 +
        //                            int.Parse(reader[3].ToString()) * noteSetType.DenominationType4).ToString();
        //                    }
        //                    reader.Close();

        //                }
        //            }
        //            finally
        //            {
        //                if (conn != null)
        //                    conn.Close();
        //            }

        //        }

        protected void Page_Load(object sender, EventArgs e)
        {


            if (!base.IsPostBack)
            {
                LogableTask task = LogableTask.NewTask("Page_Load");
                Session["lastSettlement"] = null;
                Session["replenishment"] = null;
                Session["atmSettlement"] = null;
                object isFuncEnabled = Request.QueryString["e"];
                object settlementID = Request.QueryString["sid"];
                if (!base.CheckRight(0xbed))
                {
                    base.Response.Redirect("../admin/AccessDenied.aspx");
                }

                if (settlementID != null)
                {
                    Session["isFuncEnabled"] = isFuncEnabled;
                    Session["settlementID"] = settlementID;
                }
                else
                {
                    isFuncEnabled = Session["isFuncEnabled"];
                    settlementID = Session["settlementID"];
                }
                if (isFuncEnabled != null)
                {
                    if (isFuncEnabled.ToString() == "0")
                    {
                        this.Button_Approve.Enabled = false;
                        this.Button_reset.Enabled = false;
                        Button_AddNew.Enabled = false;
                        Button_Correction.Enabled = false;
                    }
                }

                int atmSettlementID = int.Parse(settlementID.ToString());
                this.ReloadComments(atmSettlementID);

                try
                {
                    int testCashPurgedBin1 = 0;
                    int testCashPurgedBin2 = 0;
                    int testCashPurgedBin3 = 0;
                    int testCashPurgedBin4 = 0;

                    int CassettePurgedBin1 = 0;
                    int CassettePurgedBin2 = 0;
                    int CassettePurgedBin3 = 0;
                    int CassettePurgedBin4 = 0;


                    int notesPurgedInSupBeforeRep1 = 0;
                    int notesPurgedInSupBeforeRep2 = 0;
                    int notesPurgedInSupBeforeRep3 = 0;
                    int notesPurgedInSupBeforeRep4 = 0;


                    oldTable.Visible = false;
                    AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlementByPk(atmSettlementID);
                    if (atmSettlement != null)
                    {
                        Session["atmSettlement"] = atmSettlement;
                        Atm atm = Atm.LoadAtm("cit_atm_title = '" + atmSettlement.AtmNo.Value + "'");
                      
                      //  Atm atm = Atm.LoadAtm("cit_atm_title = '" + atmSettlement.AtmNo.Value.ToString().PadLeft(8,'0') + "'");
                        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                        Label_DiffNoteSetType1.Text = Label_NoteSetType1.Text = noteSetType.DenominationType1.Value.ToString();
                        Label_DiffNoteSetType2.Text = Label_NoteSetType2.Text = noteSetType.DenominationType2.Value.ToString();
                        Label_DiffNoteSetType3.Text = Label_NoteSetType3.Text = noteSetType.DenominationType3.Value.ToString();
                        Label_DiffNoteSetType4.Text = Label_NoteSetType4.Text = noteSetType.DenominationType4.Value.ToString();

                        Label_Title.Text = atm.Title;
                        Label_ATMCITTitle.Text = atm.CitAtmTitle;
                        Label_ATMLocation.Text = atm.Location;
                        //CIT counters
                        Label_TotalAdded.Text = atmSettlement.TotalReplenished.ToString();
                        /////////////////////////////New Loading////////////////////////////////////
                        if (atmSettlement.CashRepCountersType1.HasValue)
                            Label_notesAdded1.Text = atmSettlement.CashRepCountersType1.ToString();
                        else
                            Label_notesAdded1.Text = "0";
                        if (atmSettlement.CashRepCountersType2.HasValue)
                            Label_notesAdded2.Text = atmSettlement.CashRepCountersType2.ToString();
                        else
                            Label_notesAdded2.Text = "0";
                        if (atmSettlement.CashRepCountersType3.HasValue)
                            Label_notesAdded3.Text = atmSettlement.CashRepCountersType3.ToString();
                        else
                            Label_notesAdded3.Text = "0";
                        if (atmSettlement.CashRepCountersType4.HasValue)
                            Label_notesAdded4.Text = atmSettlement.CashRepCountersType4.ToString();
                        else
                            Label_notesAdded4.Text = "0";
                        /////////////////////Remaining in Cassettes
                        if (atmSettlement.CashRemCountersType1.HasValue)
                            Label_CITLeft1.Text = atmSettlement.CashRemCountersType1.Value.ToString();
                        else
                            Label_CITLeft1.Text = "0";

                        if (atmSettlement.CashRemCountersType2.HasValue)
                            Label_CITLeft2.Text = atmSettlement.CashRemCountersType2.Value.ToString();
                        else
                            Label_CITLeft2.Text = "0";

                        if (atmSettlement.CashRemCountersType3.HasValue)
                            Label_CITLeft3.Text = atmSettlement.CashRemCountersType3.Value.ToString();
                        else
                            Label_CITLeft3.Text = "0";

                        if (atmSettlement.CashRemCountersType4.HasValue)
                            Label_CITLeft4.Text = atmSettlement.CashRemCountersType4.Value.ToString();
                        else
                            Label_CITLeft4.Text = "0";
                        /////////////////////Returned

                        //////////////////////Rejected
                        if (atmSettlement.CashRejectedCountersType1.HasValue)
                            Label_CITRejected1.Text = atmSettlement.CashRejectedCountersType1.ToString();
                        else
                            Label_CITRejected1.Text = "0";
                        if (atmSettlement.CashRejectedCountersType2.HasValue)
                            Label_CITRejected2.Text = atmSettlement.CashRejectedCountersType2.ToString();
                        else
                            Label_CITRejected2.Text = "0";

                        if (atmSettlement.CashRejectedCountersType3.HasValue)
                            Label_CITRejected3.Text = atmSettlement.CashRejectedCountersType3.ToString();
                        else
                            Label_CITRejected3.Text = "0";

                        if (atmSettlement.CashRejectedCountersType4.HasValue)
                            Label_CITRejected4.Text = atmSettlement.CashRejectedCountersType4.ToString();
                        else
                            Label_CITRejected4.Text = "0";



                        Label_Returned1.Text = (int.Parse(Label_CITLeft1.Text) + int.Parse(Label_CITRejected1.Text)).ToString();
                        Label_Returned2.Text = (int.Parse(Label_CITLeft2.Text) + int.Parse(Label_CITRejected2.Text)).ToString();
                        Label_Returned3.Text = (int.Parse(Label_CITLeft3.Text) + int.Parse(Label_CITRejected3.Text)).ToString();
                        Label_Returned4.Text = (int.Parse(Label_CITLeft4.Text) + int.Parse(Label_CITRejected4.Text)).ToString();




                        Label_CITTotalLeftAmount.Text = (int.Parse(Label_Returned1.Text) * noteSetType.DenominationType1 +
                            int.Parse(Label_Returned2.Text) * noteSetType.DenominationType2 +
                            int.Parse(Label_Returned3.Text) * noteSetType.DenominationType3 +
                            int.Parse(Label_Returned4.Text) * noteSetType.DenominationType4).ToString();
                        //At the time of new loading..
                        Label_ATMClosedDate.Text = atmSettlement.RepDatetime.Value.ToString("dd-MMM-yy");
                        Label_ATMClosedTime.Text = atmSettlement.RepDatetime.Value.ToString("HH:mm");


                        AtmSettlement lastSettlement = AtmSettlement.LoadAtmSettlement(@"atm_no = '" + atmSettlement.AtmNo + "' and rep_datetime >= convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + "',103) " +
                                                                                        " and rep_datetime <= convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + " 23:59:59',103) ");
                        if (lastSettlement != null)
                        {
                            Label_ATMOpenDate.Text = lastSettlement.RepDatetime.Value.ToString("dd-MMM-yy");
                            Label_ATMOpenTime.Text = lastSettlement.RepDatetime.Value.ToString("HH:mm");
                            Label_CITAdded1.Text = lastSettlement.CashRepCountersType1.ToString();
                            Label_CITAdded2.Text = lastSettlement.CashRepCountersType2.ToString();
                            Label_CITAdded3.Text = lastSettlement.CashRepCountersType3.ToString();
                            Label_CITAdded4.Text = lastSettlement.CashRepCountersType4.ToString();
                            Session["lastSettlement"] = lastSettlement;
                        }

                        if (atm.IsEjEnabled)
                        {
                            LogableTask.LogMonoActivityTask("CheckTime", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy"));
                            Dispensed dispensed = Dispensed.LoadDispensed("atm_id=" + atm.ATMId + " and clearing_datetime=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + "',103)");
                            if (dispensed != null)
                            {
                                LogableTask.LogMonoActivityTask("CheckTime", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, "dispensed id= " + dispensed.DispensedId);
                                Label_CCMSAdded1.Text = (dispensed.CashDispensed1 + dispensed.CashRemaining1 + dispensed.CashPurged1).ToString();
                                Label_CCMSAdded2.Text = (dispensed.CashDispensed2 + dispensed.CashRemaining2 + dispensed.CashPurged2).ToString();
                                Label_CCMSAdded3.Text = (dispensed.CashDispensed3 + dispensed.CashRemaining3 + dispensed.CashPurged3).ToString();
                                Label_CCMSAdded4.Text = (dispensed.CashDispensed4 + dispensed.CashRemaining4 + dispensed.CashPurged4).ToString();
                                SummaryCorrectionData summaryCorrectionData = SummaryCorrectionData.LoadSummaryCorrectionData("atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + "',103) and rep_datetime<=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + " 23:59:59',103)   ");
                                if (summaryCorrectionData != null)
                                {
                                    Label_CCMSReturned1.Text = (summaryCorrectionData.NotesRejected1 + summaryCorrectionData.NotesRemaining1).ToString();
                                    Label_CCMSReturned2.Text = (summaryCorrectionData.NotesRejected2 + summaryCorrectionData.NotesRemaining2).ToString();
                                    Label_CCMSReturned3.Text = (summaryCorrectionData.NotesRejected3 + summaryCorrectionData.NotesRemaining3).ToString();
                                    Label_CCMSReturned4.Text = (summaryCorrectionData.NotesRejected4 + summaryCorrectionData.NotesRemaining4).ToString();
                                }
                                else
                                {

                                    Label_CCMSReturned1.Text = (dispensed.CashPurged1 + dispensed.CashRemaining1).ToString();
                                    Label_CCMSReturned2.Text = (dispensed.CashPurged2 + dispensed.CashRemaining2).ToString();
                                    Label_CCMSReturned3.Text = (dispensed.CashPurged3 + dispensed.CashRemaining3).ToString();
                                    Label_CCMSReturned4.Text = (dispensed.CashPurged4 + dispensed.CashRemaining4).ToString();
                                }

                                if (summaryCorrectionData != null)
                                {
                                    Label_CCMSRejected1.Text = summaryCorrectionData.NotesRejected1.ToString();
                                    Label_CCMSRejected2.Text = summaryCorrectionData.NotesRejected2.ToString();
                                    Label_CCMSRejected3.Text = summaryCorrectionData.NotesRejected3.ToString();
                                    Label_CCMSRejected4.Text = summaryCorrectionData.NotesRejected4.ToString();
                                }
                                else
                                {
                                    Label_CCMSRejected1.Text = dispensed.CashPurged1.ToString();
                                    Label_CCMSRejected2.Text = dispensed.CashPurged2.ToString();
                                    Label_CCMSRejected3.Text = dispensed.CashPurged3.ToString();
                                    Label_CCMSRejected4.Text = dispensed.CashPurged4.ToString();

                                }
                                if (summaryCorrectionData != null)
                                {
                                    Label_CCMSLeft1.Text = summaryCorrectionData.NotesRemaining1.ToString();
                                    Label_CCMSLeft2.Text = summaryCorrectionData.NotesRemaining2.ToString();
                                    Label_CCMSLeft3.Text = summaryCorrectionData.NotesRemaining3.ToString();
                                    Label_CCMSLeft4.Text = summaryCorrectionData.NotesRemaining4.ToString();
                                }
                                else
                                {
                                    Label_CCMSLeft1.Text = dispensed.CashRemaining1.ToString();
                                    Label_CCMSLeft2.Text = dispensed.CashRemaining2.ToString();
                                    Label_CCMSLeft3.Text = dispensed.CashRemaining3.ToString();
                                    Label_CCMSLeft4.Text = dispensed.CashRemaining4.ToString();
                                }
                                if (summaryCorrectionData != null)
                                {
                                    Label_CCMSDispensed1.Text = summaryCorrectionData.NotesDispensed1.ToString();
                                    Label_CCMSDispensed2.Text = summaryCorrectionData.NotesDispensed2.ToString();
                                    Label_CCMSDispensed3.Text = summaryCorrectionData.NotesDispensed3.ToString();
                                    Label_CCMSDispensed4.Text = summaryCorrectionData.NotesDispensed4.ToString();
                                }
                                else
                                {
                                    Label_CCMSDispensed1.Text = dispensed.CashDispensed1.ToString();
                                    Label_CCMSDispensed2.Text = dispensed.CashDispensed2.ToString();
                                    Label_CCMSDispensed3.Text = dispensed.CashDispensed3.ToString();
                                    Label_CCMSDispensed4.Text = dispensed.CashDispensed4.ToString();

                                }


                                Label_CCMSTotalLeftAmount.Text = (int.Parse(Label_CCMSReturned1.Text) * noteSetType.DenominationType1 +
                                            int.Parse(Label_CCMSReturned2.Text) * noteSetType.DenominationType2 +
                                            int.Parse(Label_CCMSReturned3.Text) * noteSetType.DenominationType3 +
                                            int.Parse(Label_CCMSReturned4.Text) * noteSetType.DenominationType4).ToString();


                                Label_DiffType1.Text = (int.Parse(Label_Returned1.Text) - int.Parse(Label_CCMSReturned1.Text)).ToString();
                                Label_DiffType2.Text = (int.Parse(Label_Returned2.Text) - int.Parse(Label_CCMSReturned2.Text)).ToString();
                                Label_DiffType3.Text = (int.Parse(Label_Returned3.Text) - int.Parse(Label_CCMSReturned3.Text)).ToString();
                                Label_DiffType4.Text = (int.Parse(Label_Returned4.Text) - int.Parse(Label_CCMSReturned4.Text)).ToString();

                                Label_DiffTotalAmount.Text = (int.Parse(Label_DiffType1.Text) * noteSetType.DenominationType1 +
                                    int.Parse(Label_DiffType2.Text) * noteSetType.DenominationType2 +
                                    int.Parse(Label_DiffType3.Text) * noteSetType.DenominationType3 +
                                    int.Parse(Label_DiffType4.Text) * noteSetType.DenominationType4).ToString();

                            }
                            else
                            {
                                LogableTask.LogMonoActivityTask("CheckTime", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, "no data for clearing counters so reading correction data");

                                SummaryCorrectionData summaryCorrectionData = SummaryCorrectionData.LoadSummaryCorrectionData("atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + "',103) and rep_datetime<=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + " 23:59:59',103)   ");
                                if (summaryCorrectionData != null)
                                {

                                    Label_CCMSReturned1.Text = (summaryCorrectionData.NotesRejected1 + summaryCorrectionData.NotesRemaining1).ToString();
                                    Label_CCMSReturned2.Text = (summaryCorrectionData.NotesRejected2 + summaryCorrectionData.NotesRemaining2).ToString();
                                    Label_CCMSReturned3.Text = (summaryCorrectionData.NotesRejected3 + summaryCorrectionData.NotesRemaining3).ToString();
                                    Label_CCMSReturned4.Text = (summaryCorrectionData.NotesRejected4 + summaryCorrectionData.NotesRemaining4).ToString();
                                    Label_CCMSRejected1.Text = summaryCorrectionData.NotesRejected1.ToString();
                                    Label_CCMSRejected2.Text = summaryCorrectionData.NotesRejected2.ToString();
                                    Label_CCMSRejected3.Text = summaryCorrectionData.NotesRejected3.ToString();
                                    Label_CCMSRejected4.Text = summaryCorrectionData.NotesRejected4.ToString();
                                    Label_CCMSRejected1.Text = summaryCorrectionData.NotesRejected1.ToString();
                                    Label_CCMSRejected2.Text = summaryCorrectionData.NotesRejected2.ToString();
                                    Label_CCMSRejected3.Text = summaryCorrectionData.NotesRejected3.ToString();
                                    Label_CCMSRejected4.Text = summaryCorrectionData.NotesRejected4.ToString();
                                    Label_CCMSDispensed1.Text = summaryCorrectionData.NotesDispensed1.ToString();
                                    Label_CCMSDispensed2.Text = summaryCorrectionData.NotesDispensed2.ToString();
                                    Label_CCMSDispensed3.Text = summaryCorrectionData.NotesDispensed3.ToString();
                                    Label_CCMSDispensed4.Text = summaryCorrectionData.NotesDispensed4.ToString();
                                    Label_CCMSTotalLeftAmount.Text = (int.Parse(Label_CCMSReturned1.Text) * noteSetType.DenominationType1 +
                                           int.Parse(Label_CCMSReturned2.Text) * noteSetType.DenominationType2 +
                                           int.Parse(Label_CCMSReturned3.Text) * noteSetType.DenominationType3 +
                                           int.Parse(Label_CCMSReturned4.Text) * noteSetType.DenominationType4).ToString();


                                    Label_DiffType1.Text = (int.Parse(Label_Returned1.Text) - int.Parse(Label_CCMSReturned1.Text)).ToString();
                                    Label_DiffType2.Text = (int.Parse(Label_Returned2.Text) - int.Parse(Label_CCMSReturned2.Text)).ToString();
                                    Label_DiffType3.Text = (int.Parse(Label_Returned3.Text) - int.Parse(Label_CCMSReturned3.Text)).ToString();
                                    Label_DiffType4.Text = (int.Parse(Label_Returned4.Text) - int.Parse(Label_CCMSReturned4.Text)).ToString();

                                    Label_DiffTotalAmount.Text = (int.Parse(Label_DiffType1.Text) * noteSetType.DenominationType1 +
                                        int.Parse(Label_DiffType2.Text) * noteSetType.DenominationType2 +
                                        int.Parse(Label_DiffType3.Text) * noteSetType.DenominationType3 +
                                        int.Parse(Label_DiffType4.Text) * noteSetType.DenominationType4).ToString();
                                }
                                else
                                    LogableTask.LogMonoActivityTask("CheckTime", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, "no correction data found");

                            }
                            //string SQL = "atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + "',103)  " +
                            //       " and rep_datetime<=convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + " 23:59:59',103) order by rep_datetime desc";
                            //LogableTask.LogMonoActivityTask("CheckSQL", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, SQL);
                            //Avanza.CCMS.DAL.EjParsedReplenishments replenishment = Avanza.CCMS.DAL.EjParsedReplenishments.LoadEjParsedReplenishments(SQL);
                            //if (replenishment != null)
                            //{
                            //    Label_CCMSAdded1.Text = replenishment.NotesAddedType1.ToString();

                            //    Label_CCMSAdded2.Text = replenishment.NotesAddedType2.ToString();

                            //    Label_CCMSAdded3.Text = replenishment.NotesAddedType3.ToString();

                            //    Label_CCMSAdded4.Text = replenishment.NotesAddedType4.ToString();


                            //    EjSummary ejSummary = EjSummary.LoadEjSummary(string.Format("atm_id = " + atm.ATMId +
                            //                                                  " and trxn_datetime=convert(datetime,'{0}',103)", atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy")));
                            //    if (ejSummary != null)
                            //    {
                            //        if (ejSummary.ReturnType1.HasValue)
                            //            Label_CCMSReturned1.Text = ejSummary.ReturnType1.Value.ToString();

                            //        if (ejSummary.ReturnType2.HasValue)
                            //            Label_CCMSReturned2.Text = ejSummary.ReturnType2.Value.ToString();

                            //        if (ejSummary.ReturnType3.HasValue)
                            //            Label_CCMSReturned3.Text = ejSummary.ReturnType3.Value.ToString();

                            //        if (ejSummary.ReturnType4.HasValue)
                            //            Label_CCMSReturned4.Text = ejSummary.ReturnType4.Value.ToString();


                            //        if (ejSummary.NotesRemainingType1.HasValue)
                            //            Label_CCMSLeft1.Text = ejSummary.NotesRemainingType1.ToString();

                            //        if (ejSummary.NotesRemainingType2.HasValue)
                            //            Label_CCMSLeft2.Text = ejSummary.NotesRemainingType2.ToString();

                            //        if (ejSummary.NotesRemainingType3.HasValue)
                            //            Label_CCMSLeft3.Text = ejSummary.NotesRemainingType3.ToString();

                            //        if (ejSummary.NotesRemainingType4.HasValue)
                            //            Label_CCMSLeft4.Text = ejSummary.NotesRemainingType4.ToString();


                            //        Label_CCMSDispensed1.Text = (replenishment.NotesAddedType1 - int.Parse(Label_CCMSReturned1.Text)).ToString();
                            //        Label_CCMSDispensed2.Text = (replenishment.NotesAddedType2 - int.Parse(Label_CCMSReturned2.Text)).ToString();
                            //        Label_CCMSDispensed3.Text = (replenishment.NotesAddedType3 - int.Parse(Label_CCMSReturned3.Text)).ToString();
                            //        Label_CCMSDispensed4.Text = (replenishment.NotesAddedType4 - int.Parse(Label_CCMSReturned4.Text)).ToString();



                            //        Label_CCMSTotalLeftAmount.Text = (int.Parse(Label_CCMSReturned1.Text) * noteSetType.DenominationType1 +
                            //                    int.Parse(Label_CCMSReturned2.Text) * noteSetType.DenominationType2 +
                            //                    int.Parse(Label_CCMSReturned3.Text) * noteSetType.DenominationType3 +
                            //                    int.Parse(Label_CCMSReturned4.Text) * noteSetType.DenominationType4).ToString();


                            //        Label_DiffType1.Text = (int.Parse(Label_Returned1.Text) - int.Parse(Label_CCMSReturned1.Text)).ToString();
                            //        Label_DiffType2.Text = (int.Parse(Label_Returned2.Text) - int.Parse(Label_CCMSReturned2.Text)).ToString();
                            //        Label_DiffType3.Text = (int.Parse(Label_Returned3.Text) - int.Parse(Label_CCMSReturned3.Text)).ToString();
                            //        Label_DiffType4.Text = (int.Parse(Label_Returned4.Text) - int.Parse(Label_CCMSReturned4.Text)).ToString();

                            //        Label_DiffTotalAmount.Text = (int.Parse(Label_DiffType1.Text) * noteSetType.DenominationType1 +
                            //            int.Parse(Label_DiffType2.Text) * noteSetType.DenominationType2 +
                            //            int.Parse(Label_DiffType3.Text) * noteSetType.DenominationType3 +
                            //            int.Parse(Label_DiffType4.Text) * noteSetType.DenominationType4).ToString();

                            //    }


                            //}
                        }
                        else
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "ej enabled is false so reading data from counters file transactions");
                            string SQL = "atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + "',103)  " +
                                " and rep_datetime<=convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + " 23:59:59',103) order by rep_datetime desc";
                            LogableTask.LogMonoActivityTask("CheckSQL", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, SQL);
                            Avanza.CCMS.DAL.Replenishment replenishment = Avanza.CCMS.DAL.Replenishment.LoadReplenishment(SQL
                                 );
                            if (replenishment != null)
                            {
                                Session["replenishment"] = replenishment;

                                Label_CCMSAdded1.Text = replenishment.CashAdded1.ToString();

                                Label_CCMSAdded2.Text = replenishment.CashAdded2.ToString();

                                Label_CCMSAdded3.Text = replenishment.CashAdded3.ToString();

                                Label_CCMSAdded4.Text = replenishment.CashAdded4.ToString();

                                SQL = "atm_id = " + atm.ATMId + " and rep_datetime = (select max(rep_datetime) from replenishment where rep_datetime >convert(datetime,'" + replenishment.RepDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) and rep_datetime <convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + " 23:59:59',103) and atm_id = " + atm.ATMId + ") ";
                                LogableTask.LogMonoActivityTask("CheckSQL", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, SQL);
                                Replenishment latestRep = Replenishment.LoadReplenishment(SQL);

                                if (latestRep != null)
                                {
                                    SQL = "atm_id = " + atm.ATMId + " and trxn_datetime=(select max(trxn_datetime) from parsed_transaction where atm_id = " + atm.ATMId + " and trxn_datetime<convert(datetime,'" + latestRep.RepDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103))";
                                    LogableTask.LogMonoActivityTask("CheckSQL", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, SQL);
                                    ParsedTransaction parsedTrxn = ParsedTransaction.LoadParsedTransaction(SQL);
                                    if (parsedTrxn != null)
                                    {

                                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                                        cmd.CommandText = @"select isnull(sum(cash_purged1),0),isnull(sum(cash_purged2),0),isnull(sum(cash_purged3),0),isnull(sum(cash_purged4),0)  
                                                    from test_cash_purged_notes 
                                                    where atm_id = " + atm.ATMId + " and test_cash_datetime>=convert(datetime,'" + replenishment.RepDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) " +
                                                          " and test_cash_datetime<convert(datetime,'" + parsedTrxn.TrxnDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) ";
                                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                        DataTable dt = new DataTable();
                                        adapter.Fill(dt);
                                        if (dt.Rows.Count > 0)
                                        {
                                            testCashPurgedBin1 = int.Parse(dt.Rows[0][0].ToString());
                                            testCashPurgedBin2 = int.Parse(dt.Rows[0][1].ToString());
                                            testCashPurgedBin3 = int.Parse(dt.Rows[0][2].ToString());
                                            testCashPurgedBin4 = int.Parse(dt.Rows[0][3].ToString());
                                        }


                                        cmd.CommandText = @"select isnull(sum(cash_purged1),0),isnull(sum(cash_purged2),0),isnull(sum(cash_purged3),0),isnull(sum(cash_purged4),0)
                                                    from parsed_transaction 
                                                    where atm_id = " + atm.ATMId + " and trxn_datetime>=convert(datetime,'" + replenishment.RepDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) " +
                                                          " and trxn_datetime<=convert(datetime,'" + parsedTrxn.TrxnDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) ";
                                        adapter = new SqlDataAdapter(cmd);
                                        dt = new DataTable();
                                        adapter.Fill(dt);
                                        if (dt.Rows.Count > 0)
                                        {
                                            CassettePurgedBin1 = int.Parse(dt.Rows[0][0].ToString());
                                            CassettePurgedBin2 = int.Parse(dt.Rows[0][1].ToString());
                                            CassettePurgedBin3 = int.Parse(dt.Rows[0][2].ToString());
                                            CassettePurgedBin4 = int.Parse(dt.Rows[0][3].ToString());
                                        }

                                        CcmsAtmLog ccmsAtmLog = CcmsAtmLog.LoadCcmsAtmLog("atm_id = " + atm.ATMId + " and event_occured_at>=convert(datetime,'" + parsedTrxn.TrxnDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) " +
                                                          " and event_occured_at<=convert(datetime,'" + latestRep.RepDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) and event_name = 'AddCash' and event_info like '%-%'");
                                        if (ccmsAtmLog != null)
                                        {
                                            LogableTask.LogMonoActivityTask("CCMSAtmLog", MethodBase.GetCurrentMethod(), TraceLevel.Info, "Log id = " + ccmsAtmLog.Id);
                                            string[] parts = ccmsAtmLog.EventInfo.Split('|');
                                            if (parts.Length > 0)
                                            {
                                                notesPurgedInSupBeforeRep1 = int.Parse(parts[7]);
                                                if (notesPurgedInSupBeforeRep1 > 0)
                                                    notesPurgedInSupBeforeRep1 = 0;
                                                notesPurgedInSupBeforeRep2 = int.Parse(parts[8]);
                                                if (notesPurgedInSupBeforeRep2 > 0)
                                                    notesPurgedInSupBeforeRep2 = 0;
                                                notesPurgedInSupBeforeRep3 = int.Parse(parts[9]);
                                                if (notesPurgedInSupBeforeRep3 > 0)
                                                    notesPurgedInSupBeforeRep3 = 0;
                                                notesPurgedInSupBeforeRep4 = int.Parse(parts[10]);
                                                if (notesPurgedInSupBeforeRep4 > 0)
                                                    notesPurgedInSupBeforeRep4 = 0;
                                            }

                                        }



                                        SummaryCorrectionData summaryCorrectionData = SummaryCorrectionData.LoadSummaryCorrectionData("atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + "',103) and rep_datetime<=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + " 23:59:59',103)   ");
                                        if (summaryCorrectionData != null)
                                        {
                                            Label_CCMSLeft1.Text = summaryCorrectionData.NotesRemaining1.ToString();
                                            Label_CCMSLeft2.Text = summaryCorrectionData.NotesRemaining2.ToString();
                                            Label_CCMSLeft3.Text = summaryCorrectionData.NotesRemaining3.ToString();
                                            Label_CCMSLeft4.Text = summaryCorrectionData.NotesRemaining4.ToString();
                                        }
                                        else
                                        {

                                            Label_CCMSLeft1.Text = (parsedTrxn.CashRemaining1 - parsedTrxn.CashDispensed1 - parsedTrxn.CashPurged1 + notesPurgedInSupBeforeRep1).ToString();
                                            if (int.Parse(Label_CCMSLeft1.Text) < 0)
                                                Label_CCMSLeft1.Text = "0";
                                            Label_CCMSLeft2.Text = (parsedTrxn.CashRemaining2 - parsedTrxn.CashDispensed2 - parsedTrxn.CashPurged2 + notesPurgedInSupBeforeRep2).ToString();
                                            if (int.Parse(Label_CCMSLeft2.Text) < 0)
                                                Label_CCMSLeft2.Text = "0";
                                            Label_CCMSLeft3.Text = (parsedTrxn.CashRemaining3 - parsedTrxn.CashDispensed3 - parsedTrxn.CashPurged3 + notesPurgedInSupBeforeRep3).ToString();
                                            if (int.Parse(Label_CCMSLeft3.Text) < 0)
                                                Label_CCMSLeft3.Text = "0";
                                            Label_CCMSLeft4.Text = (parsedTrxn.CashRemaining4 - parsedTrxn.CashDispensed4 - parsedTrxn.CashPurged4 + notesPurgedInSupBeforeRep4).ToString();
                                            if (int.Parse(Label_CCMSLeft4.Text) < 0)
                                                Label_CCMSLeft4.Text = "0";

                                        }
                                        if (summaryCorrectionData != null)
                                        {
                                            Label_CCMSReturned1.Text = (summaryCorrectionData.NotesRejected1 + summaryCorrectionData.NotesRemaining1).ToString();
                                            Label_CCMSReturned2.Text = (summaryCorrectionData.NotesRejected2 + summaryCorrectionData.NotesRemaining2).ToString();
                                            Label_CCMSReturned3.Text = (summaryCorrectionData.NotesRejected3 + summaryCorrectionData.NotesRemaining3).ToString();
                                            Label_CCMSReturned4.Text = (summaryCorrectionData.NotesRejected4 + summaryCorrectionData.NotesRemaining4).ToString();
                                        }
                                        else
                                        {
                                            Label_CCMSReturned1.Text = (int.Parse(Label_CCMSLeft1.Text) + CassettePurgedBin1 + testCashPurgedBin1).ToString();
                                            Label_CCMSReturned2.Text = (int.Parse(Label_CCMSLeft2.Text) + CassettePurgedBin2 + testCashPurgedBin2).ToString();
                                            Label_CCMSReturned3.Text = (int.Parse(Label_CCMSLeft3.Text) + CassettePurgedBin3 + testCashPurgedBin3).ToString();
                                            Label_CCMSReturned4.Text = (int.Parse(Label_CCMSLeft4.Text) + CassettePurgedBin4 + testCashPurgedBin4).ToString();
                                        }
                                        if (summaryCorrectionData != null)
                                        {
                                            Label_CCMSRejected1.Text = summaryCorrectionData.NotesRejected1.ToString();
                                            Label_CCMSRejected2.Text = summaryCorrectionData.NotesRejected2.ToString();
                                            Label_CCMSRejected3.Text = summaryCorrectionData.NotesRejected3.ToString();
                                            Label_CCMSRejected4.Text = summaryCorrectionData.NotesRejected4.ToString();
                                        }
                                        else
                                        {
                                            Label_CCMSRejected1.Text = (CassettePurgedBin1 + testCashPurgedBin1).ToString();
                                            Label_CCMSRejected2.Text = (CassettePurgedBin2 + testCashPurgedBin2).ToString();
                                            Label_CCMSRejected3.Text = (CassettePurgedBin3 + testCashPurgedBin3).ToString();
                                            Label_CCMSRejected4.Text = (CassettePurgedBin4 + testCashPurgedBin4).ToString();
                                        }
                                        //Label_CCMSDispensed1.Text = (replenishment.CashAdded1 - int.Parse(Label_CCMSLeft1.Text) + parsedTrxn.CashPurged1).ToString();
                                        //Label_CCMSDispensed2.Text = (replenishment.CashAdded2 - int.Parse(Label_CCMSLeft2.Text) + parsedTrxn.CashPurged2).ToString();
                                        //Label_CCMSDispensed3.Text = (replenishment.CashAdded3 - int.Parse(Label_CCMSLeft3.Text) + parsedTrxn.CashPurged3).ToString();
                                        //Label_CCMSDispensed4.Text = (replenishment.CashAdded4 - int.Parse(Label_CCMSLeft4.Text) + parsedTrxn.CashPurged4).ToString();
                                        if (summaryCorrectionData != null)
                                        {
                                            Label_CCMSDispensed1.Text = summaryCorrectionData.NotesDispensed1.ToString();
                                            Label_CCMSDispensed2.Text = summaryCorrectionData.NotesDispensed2.ToString();
                                            Label_CCMSDispensed3.Text = summaryCorrectionData.NotesDispensed3.ToString();
                                            Label_CCMSDispensed4.Text = summaryCorrectionData.NotesDispensed4.ToString();

                                        }
                                        else
                                        {
                                            Label_CCMSDispensed1.Text =
                                                (replenishment.CashAdded1 - int.Parse(Label_CCMSReturned1.Text) + Math.Abs(notesPurgedInSupBeforeRep1)).ToString();
                                            Label_CCMSDispensed2.Text = (replenishment.CashAdded2 - int.Parse(Label_CCMSReturned2.Text) + Math.Abs(notesPurgedInSupBeforeRep2)).ToString();
                                            Label_CCMSDispensed3.Text = (replenishment.CashAdded3 - int.Parse(Label_CCMSReturned3.Text) + Math.Abs(notesPurgedInSupBeforeRep3)).ToString();
                                            Label_CCMSDispensed4.Text = (replenishment.CashAdded4 - int.Parse(Label_CCMSReturned4.Text) + Math.Abs(notesPurgedInSupBeforeRep4)).ToString();

                                        }






                                        Label_CCMSTotalLeftAmount.Text = (int.Parse(Label_CCMSReturned1.Text) * noteSetType.DenominationType1 +
                                            int.Parse(Label_CCMSReturned2.Text) * noteSetType.DenominationType2 +
                                            int.Parse(Label_CCMSReturned3.Text) * noteSetType.DenominationType3 +
                                            int.Parse(Label_CCMSReturned4.Text) * noteSetType.DenominationType4).ToString();


                                        Label_DiffType1.Text = (int.Parse(Label_Returned1.Text) - int.Parse(Label_CCMSReturned1.Text)).ToString();
                                        Label_DiffType2.Text = (int.Parse(Label_Returned2.Text) - int.Parse(Label_CCMSReturned2.Text)).ToString();
                                        Label_DiffType3.Text = (int.Parse(Label_Returned3.Text) - int.Parse(Label_CCMSReturned3.Text)).ToString();
                                        Label_DiffType4.Text = (int.Parse(Label_Returned4.Text) - int.Parse(Label_CCMSReturned4.Text)).ToString();

                                        Label_DiffTotalAmount.Text = (int.Parse(Label_DiffType1.Text) * noteSetType.DenominationType1 +
                                            int.Parse(Label_DiffType2.Text) * noteSetType.DenominationType2 +
                                            int.Parse(Label_DiffType3.Text) * noteSetType.DenominationType3 +
                                            int.Parse(Label_DiffType4.Text) * noteSetType.DenominationType4).ToString();
                                    }
                                    else
                                    {
                                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "parsed transaction not found");
                                        SummaryCorrectionData summaryCorrectionData = SummaryCorrectionData.LoadSummaryCorrectionData("atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + "',103) and rep_datetime<=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + " 23:59:59',103)   ");
                                        if (summaryCorrectionData != null)
                                        {
                                            Label_CCMSLeft1.Text = summaryCorrectionData.NotesRemaining1.ToString();
                                            Label_CCMSLeft2.Text = summaryCorrectionData.NotesRemaining2.ToString();
                                            Label_CCMSLeft3.Text = summaryCorrectionData.NotesRemaining3.ToString();
                                            Label_CCMSLeft4.Text = summaryCorrectionData.NotesRemaining4.ToString();

                                            Label_CCMSReturned1.Text = (summaryCorrectionData.NotesRejected1 + summaryCorrectionData.NotesRemaining1).ToString();
                                            Label_CCMSReturned2.Text = (summaryCorrectionData.NotesRejected2 + summaryCorrectionData.NotesRemaining2).ToString();
                                            Label_CCMSReturned3.Text = (summaryCorrectionData.NotesRejected3 + summaryCorrectionData.NotesRemaining3).ToString();
                                            Label_CCMSReturned4.Text = (summaryCorrectionData.NotesRejected4 + summaryCorrectionData.NotesRemaining4).ToString();

                                            Label_CCMSRejected1.Text = summaryCorrectionData.NotesRejected1.ToString();
                                            Label_CCMSRejected2.Text = summaryCorrectionData.NotesRejected2.ToString();
                                            Label_CCMSRejected3.Text = summaryCorrectionData.NotesRejected3.ToString();
                                            Label_CCMSRejected4.Text = summaryCorrectionData.NotesRejected4.ToString();

                                            Label_CCMSDispensed1.Text = summaryCorrectionData.NotesDispensed1.ToString();
                                            Label_CCMSDispensed2.Text = summaryCorrectionData.NotesDispensed2.ToString();
                                            Label_CCMSDispensed3.Text = summaryCorrectionData.NotesDispensed3.ToString();
                                            Label_CCMSDispensed4.Text = summaryCorrectionData.NotesDispensed4.ToString();

                                            Label_CCMSTotalLeftAmount.Text = (int.Parse(Label_CCMSReturned1.Text) * noteSetType.DenominationType1 +
                                                                             int.Parse(Label_CCMSReturned2.Text) * noteSetType.DenominationType2 +
                                                                             int.Parse(Label_CCMSReturned3.Text) * noteSetType.DenominationType3 +
                                                                             int.Parse(Label_CCMSReturned4.Text) * noteSetType.DenominationType4).ToString();


                                            Label_DiffType1.Text = (int.Parse(Label_Returned1.Text) - int.Parse(Label_CCMSReturned1.Text)).ToString();
                                            Label_DiffType2.Text = (int.Parse(Label_Returned2.Text) - int.Parse(Label_CCMSReturned2.Text)).ToString();
                                            Label_DiffType3.Text = (int.Parse(Label_Returned3.Text) - int.Parse(Label_CCMSReturned3.Text)).ToString();
                                            Label_DiffType4.Text = (int.Parse(Label_Returned4.Text) - int.Parse(Label_CCMSReturned4.Text)).ToString();

                                            Label_DiffTotalAmount.Text = (int.Parse(Label_DiffType1.Text) * noteSetType.DenominationType1 +
                                                int.Parse(Label_DiffType2.Text) * noteSetType.DenominationType2 +
                                                int.Parse(Label_DiffType3.Text) * noteSetType.DenominationType3 +
                                                int.Parse(Label_DiffType4.Text) * noteSetType.DenominationType4).ToString();

                                        }

                                    }
                                }
                                else
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Latest replenishment not found for atm " + atm.Title);
                                //GetDispensedAmount(atm.ATMId, noteSetType, atmSettlement.PreviousRepDate.Value, atmSettlement.RepDatetime.Value);















                            }
                            else
                            {
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "previous replenishment not found in CCMS for ATM title " + atm.Title);
                                SummaryCorrectionData summaryCorrectionData = SummaryCorrectionData.LoadSummaryCorrectionData("atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + "',103) and rep_datetime<=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + " 23:59:59',103)   ");
                                if (summaryCorrectionData != null)
                                {
                                    Label_CCMSLeft1.Text = summaryCorrectionData.NotesRemaining1.ToString();
                                    Label_CCMSLeft2.Text = summaryCorrectionData.NotesRemaining2.ToString();
                                    Label_CCMSLeft3.Text = summaryCorrectionData.NotesRemaining3.ToString();
                                    Label_CCMSLeft4.Text = summaryCorrectionData.NotesRemaining4.ToString();

                                    Label_CCMSReturned1.Text = (summaryCorrectionData.NotesRejected1 + summaryCorrectionData.NotesRemaining1).ToString();
                                    Label_CCMSReturned2.Text = (summaryCorrectionData.NotesRejected2 + summaryCorrectionData.NotesRemaining2).ToString();
                                    Label_CCMSReturned3.Text = (summaryCorrectionData.NotesRejected3 + summaryCorrectionData.NotesRemaining3).ToString();
                                    Label_CCMSReturned4.Text = (summaryCorrectionData.NotesRejected4 + summaryCorrectionData.NotesRemaining4).ToString();

                                    Label_CCMSRejected1.Text = summaryCorrectionData.NotesRejected1.ToString();
                                    Label_CCMSRejected2.Text = summaryCorrectionData.NotesRejected2.ToString();
                                    Label_CCMSRejected3.Text = summaryCorrectionData.NotesRejected3.ToString();
                                    Label_CCMSRejected4.Text = summaryCorrectionData.NotesRejected4.ToString();

                                    Label_CCMSDispensed1.Text = summaryCorrectionData.NotesDispensed1.ToString();
                                    Label_CCMSDispensed2.Text = summaryCorrectionData.NotesDispensed2.ToString();
                                    Label_CCMSDispensed3.Text = summaryCorrectionData.NotesDispensed3.ToString();
                                    Label_CCMSDispensed4.Text = summaryCorrectionData.NotesDispensed4.ToString();

                                    Label_CCMSTotalLeftAmount.Text = (int.Parse(Label_CCMSReturned1.Text) * noteSetType.DenominationType1 +
                                                                     int.Parse(Label_CCMSReturned2.Text) * noteSetType.DenominationType2 +
                                                                     int.Parse(Label_CCMSReturned3.Text) * noteSetType.DenominationType3 +
                                                                     int.Parse(Label_CCMSReturned4.Text) * noteSetType.DenominationType4).ToString();


                                    Label_DiffType1.Text = (int.Parse(Label_Returned1.Text) - int.Parse(Label_CCMSReturned1.Text)).ToString();
                                    Label_DiffType2.Text = (int.Parse(Label_Returned2.Text) - int.Parse(Label_CCMSReturned2.Text)).ToString();
                                    Label_DiffType3.Text = (int.Parse(Label_Returned3.Text) - int.Parse(Label_CCMSReturned3.Text)).ToString();
                                    Label_DiffType4.Text = (int.Parse(Label_Returned4.Text) - int.Parse(Label_CCMSReturned4.Text)).ToString();

                                    Label_DiffTotalAmount.Text = (int.Parse(Label_DiffType1.Text) * noteSetType.DenominationType1 +
                                        int.Parse(Label_DiffType2.Text) * noteSetType.DenominationType2 +
                                        int.Parse(Label_DiffType3.Text) * noteSetType.DenominationType3 +
                                        int.Parse(Label_DiffType4.Text) * noteSetType.DenominationType4).ToString();

                                }
                            }
                        }
                        //}
                        AtmSettlementOtherDetails atmSettlementOtherDetails = AtmSettlementOtherDetails.LoadAtmSettlementOtherDetails("atm_settlement_id=" + atmSettlementID);
                        if (atmSettlementOtherDetails != null)
                        {
                            TextBox_NCRComments.Text = atmSettlementOtherDetails.Comments;
                            TextBox_NoOfReplenishments.Text = atmSettlementOtherDetails.NoOfReplenishment.ToString();
                            TextBox_Inventory.Text = atmSettlementOtherDetails.Inventory;
                        }
                    }//
                    else
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "atm settlement not found. Atm Settlement ID : " + atmSettlementID);
                    //oldTableSettlementDetail.Visible = false;
                    //iframeEnvelop.Attributes.Add("src", "PrintEnvelop.aspx?s=" + atmSettlementID+"&i=1");

                    //AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlementByPk(atmSettlementID);
                    //if (atmSettlement != null)
                    //{
                    //    iframeEnvelop.Attributes.Add("src", "PrintEnvelop.aspx?s=" + atmSettlement.AtmSettlementId);

                    //    Atm atm = Atm.LoadAtm("cit_atm_title = '" + atmSettlement.AtmNo + "'");
                    //    if (atm != null)
                    //        this.Label_Title.Text = atm.Title;

                    //    this.Label_ATMSiteAndNumber.Text = atmSettlement.AtmSiteAndNumber;
                    //    this.Label_SealNo.Text = atmSettlement.SealNumber;
                    //    this.Label_GLNo.Text = atmSettlement.GlNo;
                    //    this.Label_ReplenishmentDate.Text = atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy");

                    //    //Atm atm = Atm.LoadAtm("cit_atm_title='" + atmSettlement.AtmNo + "'");
                    //    //Atm atm = Atm.LoadAtm("gl_number='" + atmSettlement.GlNo + "'");
                    //    if (atm != null)
                    //    {
                    //        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                    //        if (noteSetType != null)
                    //        {
                    //            if (noteSetType.DenominationType1Title.Length > 0)
                    //            {
                    //                this.Label_type1.Text = noteSetType.DenominationType1Title;
                    //                this.Label_ctype1.Text = noteSetType.DenominationType1Title;
                    //                //this.Label_EJSummaryType1.Text = noteSetType.DenominationType1Title;
                    //                //this.Label_EJType1.Text = noteSetType.DenominationType1Title;

                    //            }
                    //            if (noteSetType.DenominationType2Title.Length > 0)
                    //            {
                    //                this.Label_type2.Text = noteSetType.DenominationType2Title;
                    //                this.Label_ctype2.Text = noteSetType.DenominationType2Title;
                    //                //this.Label_EJSummaryType2.Text = noteSetType.DenominationType2Title;
                    //                //this.Label_EJType2.Text = noteSetType.DenominationType2Title;
                    //            }
                    //            if (noteSetType.DenominationType2Title.Length > 0)
                    //            {
                    //                this.Label_type3.Text = noteSetType.DenominationType3Title;
                    //                this.Label_ctype3.Text = noteSetType.DenominationType3Title;
                    //                //this.Label_EJSummaryType3.Text = noteSetType.DenominationType3Title;
                    //                //this.Label_EJType3.Text = noteSetType.DenominationType3Title;
                    //            }
                    //            if (noteSetType.DenominationType4Title.Length > 0)
                    //            {
                    //                this.Label_type4.Text = noteSetType.DenominationType4Title;
                    //                this.Label_ctype4.Text = noteSetType.DenominationType4Title;
                    //                //this.Label_EJSummaryType4.Text = noteSetType.DenominationType4Title;
                    //                //this.Label_EJType4.Text = noteSetType.DenominationType4Title;
                    //            }
                    //            PopulateGrid(atmSettlement, noteSetType);
                    //        }
                    //        Avanza.CCMS.DAL.Summary summary = Avanza.CCMS.DAL.Summary.LoadSummary(string.Concat(new object[] { "atm_id=", atm.ATMId, " and trxn_datetime=convert(datetime,'", atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy"), "',103)" }));
                    //        if (summary != null)
                    //        {
                    //            added = 0;
                    //            //dispensed = 0;
                    //            returned = 0;


                    //            if (summary.CashAdded1.HasValue)
                    //            {
                    //                this.Label_Added1.Text = summary.CashAdded1.Value.ToString();
                    //                added += summary.CashAdded1.Value * noteSetType.DenominationType1.Value;
                    //            }
                    //            if (summary.CashAdded2.HasValue)
                    //            {
                    //                this.Label_Added2.Text = summary.CashAdded2.Value.ToString();
                    //                added += summary.CashAdded2.Value * noteSetType.DenominationType2.Value;
                    //            }
                    //            if (summary.CashAdded3.HasValue)
                    //            {
                    //                this.Label_Added3.Text = summary.CashAdded3.Value.ToString();
                    //                added += summary.CashAdded3.Value * noteSetType.DenominationType3.Value;
                    //            }
                    //            if (summary.CashAdded4.HasValue)
                    //            {
                    //                this.Label_Added4.Text = summary.CashAdded4.Value.ToString();
                    //                added += summary.CashAdded4.Value * noteSetType.DenominationType4.Value;
                    //            }
                    //            if (summary.ReturnType1.HasValue)
                    //            {
                    //                this.Label_Returned1.Text = summary.ReturnType1.Value.ToString();
                    //                returned += summary.ReturnType1.Value * noteSetType.DenominationType1.Value;
                    //            }
                    //            if (summary.ReturnType2.HasValue)
                    //            {
                    //                this.Label_Returned2.Text = summary.ReturnType2.Value.ToString();
                    //                returned += summary.ReturnType2.Value * noteSetType.DenominationType2.Value;
                    //            }
                    //            if (summary.ReturnType3.HasValue)
                    //            {
                    //                this.Label_Returned3.Text = summary.ReturnType3.Value.ToString();
                    //                returned += summary.ReturnType3.Value * noteSetType.DenominationType3.Value;
                    //            }
                    //            if (summary.ReturnType4.HasValue)
                    //            {
                    //                this.Label_Returned4.Text = summary.ReturnType4.Value.ToString();
                    //                returned += summary.ReturnType4.Value * noteSetType.DenominationType4.Value;
                    //            }

                    //            Label_TotalReturned.Text = returned.ToString();
                    //            Label_TotalAdded.Text = added.ToString();
                    //            GetDispensedAmount(atm.ATMId, noteSetType);

                    //        }
                    //        //Getting data from ej summary table.
                    //        Avanza.CCMS.DAL.EjSummary ejSummary = Avanza.CCMS.DAL.EjSummary.LoadEjSummary(string.Concat(new object[] { "atm_id=", atm.ATMId, " and trxn_datetime=convert(datetime,'", atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy"), "',103)" }));
                    //        if (ejSummary != null)
                    //        {
                    //            //added = 0;
                    //            //returned = 0;


                    //            //if (ejSummary.CashAdded1.HasValue)
                    //            //{
                    //            //    this.Label_EJSummaryAdded1.Text = ejSummary.CashAdded1.Value.ToString();
                    //            //    added += ejSummary.CashAdded1.Value * noteSetType.DenominationType1.Value;
                    //            //}
                    //            //if (ejSummary.CashAdded2.HasValue)
                    //            //{
                    //            //    this.Label_EJSummaryAdded2.Text = ejSummary.CashAdded2.Value.ToString();
                    //            //    added += ejSummary.CashAdded2.Value * noteSetType.DenominationType2.Value;
                    //            //}
                    //            //if (ejSummary.CashAdded3.HasValue)
                    //            //{
                    //            //    this.Label_EJSummaryAdded3.Text = ejSummary.CashAdded3.Value.ToString();
                    //            //    added += ejSummary.CashAdded3.Value * noteSetType.DenominationType3.Value;
                    //            //}
                    //            //if (ejSummary.CashAdded4.HasValue)
                    //            //{
                    //            //    this.Label_EJSummaryAdded4.Text = ejSummary.CashAdded4.Value.ToString();
                    //            //    added += ejSummary.CashAdded4.Value * noteSetType.DenominationType4.Value;
                    //            //}
                    //            //if (ejSummary.ReturnType1.HasValue)
                    //            //{
                    //            //    this.Label_EJSummaryReturned1.Text = ejSummary.ReturnType1.Value.ToString();
                    //            //    returned += ejSummary.ReturnType1.Value * noteSetType.DenominationType1.Value;
                    //            //}
                    //            //if (ejSummary.ReturnType2.HasValue)
                    //            //{
                    //            //    this.Label_EJSummaryReturned2.Text = ejSummary.ReturnType2.Value.ToString();
                    //            //    returned += ejSummary.ReturnType2.Value * noteSetType.DenominationType2.Value;
                    //            //}
                    //            //if (ejSummary.ReturnType3.HasValue)
                    //            //{
                    //            //    this.Label_EJSummaryReturned3.Text = ejSummary.ReturnType3.Value.ToString();
                    //            //    returned += ejSummary.ReturnType3.Value * noteSetType.DenominationType3.Value;
                    //            //}
                    //            //if (ejSummary.ReturnType4.HasValue)
                    //            //{
                    //            //    this.Label_EJSummaryReturned4.Text = ejSummary.ReturnType4.Value.ToString();
                    //            //    returned += ejSummary.ReturnType4.Value * noteSetType.DenominationType4.Value;
                    //            //}

                    //            //Label_EJSummaryTotalReturned.Text = returned.ToString();
                    //            //Label_EJSummaryTotalAdded.Text = added.ToString();
                    //            //GetDispensedAmount(atm.ATMId, noteSetType);
                    //        }

                    //    }
                    //}
                }

                catch (Exception ex)
                {
                    Session[SessionVars.notice.ToString()] = ex.Message;
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
                }
                finally
                {
                    task.EndTask();
                }
            }
        }



        private void ReloadComments(int settlementID)
        {
            AtmSettlement settlement = AtmSettlement.LoadAtmSettlementByPk(settlementID);
            Avanza.CCMS.DAL.UserTask task = Avanza.CCMS.DAL.UserTask.LoadUserTask("atm_settlement_id=" + settlement.AtmSettlementId);
            UserTaskComments.UserTaskCommentsReader reader = UserTaskComments.ExecuteReader(" user_task_id=" + task.UserTaskId);
            StringBuilder builder = new StringBuilder();
            while (reader.Read())
            {
                builder.Append("<div style='width:100%'><table style='width:100%' border='0'><tr><td style='width:10%'><b>" + AppUser.LoadAppUserByPk(reader.CurrentUserTaskComments.UserId).UserFullName + ":</b></td><td style='width:80%'>" + reader.CurrentUserTaskComments.Comments + "</td><td align='right' width='10%'>" + reader.CurrentUserTaskComments.CreationTime.ToString("dd/MM/yyyy HH:mm") + "</td></table></div><br/>");
            }
            reader.Close();
            this.Literal_Comments.Text = builder.ToString();
        }

      
    }


}