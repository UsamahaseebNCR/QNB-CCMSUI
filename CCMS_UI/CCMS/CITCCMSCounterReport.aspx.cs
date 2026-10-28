using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;

using System.Data;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class CITCCMSCounterReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCITCCMSCounterReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");
                //if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                //    Button_GenerateReportSchedule.Enabled = false;


                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        string Filter()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and rep_datetime >= convert(datetime, '" + TextBox_FromDate.Text + "', 103) ";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and rep_datetime <= convert(datetime, '" + TextBox_ToDate.Text + " 23:59:59', 103) ";

            filter += " and atm.is_active =1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";

            return filter;
        }
        
        protected void Button_search_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                decimal creditAmount = 0;
                decimal debitAmount = 0;
                dsCITCCMSCounter Ds = new dsCITCCMSCounter();
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"
                                    select *
                                    from atm_settlement 
                                    inner join atm on atm.cit_atm_title = atm_settlement.atm_site_and_number
                                    " + Filter();

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    int atmSettlementID = int.Parse(dr["atm_settlement_id"].ToString());
                    AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlementByPk(atmSettlementID);
                    
                    if (atmSettlement != null)
                    {
                        DataRow newRow = Ds.Tables[0].NewRow();
                        Atm atm = Atm.LoadAtm("cit_atm_title = '" + atmSettlement.AtmSiteAndNumber + "'");
                        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                        newRow["denom1"] = noteSetType.DenominationType1Title;
                        newRow["denom2"] = noteSetType.DenominationType2Title;
                        newRow["denom3"] = noteSetType.DenominationType3Title;
                        newRow["denom4"] = noteSetType.DenominationType4Title;
                        //Label_DiffNoteSetType1.Text = Label_NoteSetType1.Text = noteSetType.DenominationType1.Value.ToString();
                        //Label_DiffNoteSetType2.Text = Label_NoteSetType2.Text = noteSetType.DenominationType2.Value.ToString();
                        //Label_DiffNoteSetType3.Text = Label_NoteSetType3.Text = noteSetType.DenominationType3.Value.ToString();
                        //Label_DiffNoteSetType4.Text = Label_NoteSetType4.Text = noteSetType.DenominationType4.Value.ToString();
                        newRow["title"] = atm.Title;
                        //Label_Title.Text = atm.Title;
                        //Label_ATMCITTitle.Text = atm.CitAtmTitle;
                        newRow["cit_atm_title"] = atm.CitAtmTitle;
                        newRow["location"] = atm.Location;
                        //Label_ATMLocation.Text = atm.Location;
                        ////CIT counters
                        //Label_TotalAdded.Text = atmSettlement.TotalReplenished.ToString();
                        /////////////////////////////New Loading////////////////////////////////////
                        if (atmSettlement.CashRepCountersType1.HasValue)
                            newRow["cit_loaded1"] = atmSettlement.CashRepCountersType1.ToString();
                        else
                            newRow["cit_loaded1"] = 0;
                        
                        if (atmSettlement.CashRepCountersType2.HasValue)
                            newRow["cit_loaded2"] = atmSettlement.CashRepCountersType2.ToString();
                        else
                            newRow["cit_loaded2"] = 0;
                        
                        if (atmSettlement.CashRepCountersType3.HasValue)
                            newRow["cit_loaded3"] = atmSettlement.CashRepCountersType3.ToString();
                        else
                            newRow["cit_loaded3"] = 0;
                        
                        if (atmSettlement.CashRepCountersType4.HasValue)
                            newRow["cit_loaded4"] = atmSettlement.CashRepCountersType4.ToString();
                        else
                            newRow["cit_loaded4"] = 0;
                        /////////////////////Remaining in Cassettes
                        if (atmSettlement.CashRemCountersType1.HasValue)
                            newRow["cit_left1"] = atmSettlement.CashRemCountersType1.Value.ToString();
                        else
                            newRow["cit_left1"] = 0;

                        if (atmSettlement.CashRemCountersType2.HasValue)
                            newRow["cit_left2"] = atmSettlement.CashRemCountersType2.Value.ToString();
                        else
                            newRow["cit_left2"] = 0;

                        if (atmSettlement.CashRemCountersType3.HasValue)
                            newRow["cit_left3"] = atmSettlement.CashRemCountersType3.Value.ToString();
                        else
                            newRow["cit_left3"] = 0;

                        if (atmSettlement.CashRemCountersType4.HasValue)
                            newRow["cit_left4"] = atmSettlement.CashRemCountersType4.Value.ToString();
                        else
                           newRow["cit_left4"]= 0;
                        /////////////////////Returned

                        //////////////////////Rejected
                        if (atmSettlement.CashRejectedCountersType1.HasValue)
                            newRow["cit_rejected1"] = atmSettlement.CashRejectedCountersType1.ToString();
                        else
                            newRow["cit_rejected1"]= 0;

                        if (atmSettlement.CashRejectedCountersType2.HasValue)
                            newRow["cit_rejected2"] = atmSettlement.CashRejectedCountersType2.ToString();
                        else
                            newRow["cit_rejected2"]=0;

                        if (atmSettlement.CashRejectedCountersType3.HasValue)
                            newRow["cit_rejected3"] = atmSettlement.CashRejectedCountersType3.ToString();
                        else
                            newRow["cit_rejected3"]= 0;

                        if (atmSettlement.CashRejectedCountersType4.HasValue)
                            newRow["cit_rejected4"] = atmSettlement.CashRejectedCountersType4.ToString();
                        else
                            newRow["cit_rejected4"]  =0;

                        newRow["cit_returned1"] = (int.Parse(newRow["cit_left1"].ToString()) + int.Parse(newRow["cit_rejected1"].ToString())).ToString();
                        newRow["cit_returned2"] = (int.Parse(newRow["cit_left2"].ToString()) + int.Parse(newRow["cit_rejected2"].ToString())).ToString();
                        newRow["cit_returned3"] = (int.Parse(newRow["cit_left3"].ToString()) + int.Parse(newRow["cit_rejected3"].ToString())).ToString();
                        newRow["cit_returned4"] = (int.Parse(newRow["cit_left4"].ToString()) + int.Parse(newRow["cit_rejected4"].ToString())).ToString();

                        newRow["cit_returned_amount_text"] = ConnectionFactory.ExecuteScalar(string.Format(@"SELECT dbo.GetTotalText(
                            denomination_type_1_title, denomination_type_2_title, denomination_type_3_title, denomination_type_4_title, 
                            denomination_type_1, denomination_type_2, denomination_type_3, denomination_type_4,
                            {0}, {1}, {2}, {3}) total_remaining_text FROM note_set_type where note_set_type_id = {4}",
                            int.Parse(newRow["cit_returned1"].ToString()), int.Parse(newRow["cit_returned2"].ToString()), int.Parse(newRow["cit_returned3"].ToString()), int.Parse(newRow["cit_returned4"].ToString()), noteSetType.NoteSetTypeId)).ToString();

                        ////At the time of new loading..
                        newRow["atm_closed_date"] = atmSettlement.RepDatetime.Value.ToString("dd-MM-yy");
                        newRow["atm_closed_time"] = atmSettlement.RepDatetime.Value.ToString("HH:mm");

                        //Label_ATMClosedDate.Text = atmSettlement.RepDatetime.Value.ToString("dd-MMM-yy");
                        //Label_ATMClosedTime.Text = atmSettlement.RepDatetime.Value.ToString("HH:mm");

                        newRow["cit_old_loaded1"] = 0;
                        newRow["cit_old_loaded2"] = 0;
                        newRow["cit_old_loaded3"] = 0;
                        newRow["cit_old_loaded4"] = 0;
                        if (atmSettlement.PreviousRepDate.HasValue)
                        {
                            AtmSettlement lastSettlement = AtmSettlement.LoadAtmSettlement(@"atm_no = '" + atmSettlement.AtmSiteAndNumber + "' and rep_datetime = convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + "',103)");
                            if (lastSettlement != null)
                            {
                                newRow["atm_open_date"] = lastSettlement.RepDatetime.Value.ToString("dd-MM-yy");
                                newRow["atm_open_time"] = lastSettlement.RepDatetime.Value.ToString("HH:mm");
                                newRow["cit_old_loaded1"] = lastSettlement.CashRepCountersType1 ?? 0;
                                newRow["cit_old_loaded2"] = lastSettlement.CashRepCountersType2 ?? 0;
                                newRow["cit_old_loaded3"] = lastSettlement.CashRepCountersType3 ?? 0;
                                newRow["cit_old_loaded4"] = lastSettlement.CashRepCountersType4 ?? 0;
                            }
                        }
                        
                        if (atm.IsEjEnabled)
                        {
                            LogableTask.LogMonoActivityTask("CheckTime", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy"));
                            Dispensed dispensed = Dispensed.LoadDispensed("atm_id=" + atm.ATMId + " and clearing_datetime=convert(datetime,'" + atmSettlement.RepDatetime.Value.ToString("dd/MM/yyyy") + "',103)");
                            if (dispensed != null)
                            {
                                LogableTask.LogMonoActivityTask("CheckTime", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, "dispensed id= " + dispensed.DispensedId);
                            }
                        }
                        else
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

                            if (atmSettlement.PreviousRepDate.HasValue)
                            {
                                string SQL = "atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + "',103)  " +
                                " and rep_datetime<=convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + " 23:59:59',103) order by rep_datetime desc";
                                LogableTask.LogMonoActivityTask("CheckSQL", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, SQL);
                                Avanza.CCMS.DAL.Replenishment replenishment = Avanza.CCMS.DAL.Replenishment.LoadReplenishment(SQL
                                     );
                                if (replenishment != null)
                                {
                                    newRow["ccms_loaded1"] = replenishment.CashAdded1.ToString();
                                    newRow["ccms_loaded2"] = replenishment.CashAdded2.ToString();
                                    newRow["ccms_loaded3"] = replenishment.CashAdded3.ToString();
                                    newRow["ccms_loaded4"] = replenishment.CashAdded4.ToString();

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

                                            cmd = ConnectionFactory.GetNewCommand(false);
                                            cmd.CommandText = @"select isnull(sum(cash_purged1),0),isnull(sum(cash_purged2),0),isnull(sum(cash_purged3),0),isnull(sum(cash_purged4),0)  
                                                    from test_cash_purged_notes 
                                                    where atm_id = " + atm.ATMId + " and test_cash_datetime>=convert(datetime,'" + replenishment.RepDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) " +
                                                              " and test_cash_datetime<convert(datetime,'" + parsedTrxn.TrxnDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) ";
                                            adapter = new SqlDataAdapter(cmd);
                                            dt = new DataTable();
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
                                                newRow["ccms_left1"] = summaryCorrectionData.NotesRemaining1.ToString();
                                                newRow["ccms_left2"] = summaryCorrectionData.NotesRemaining2.ToString();
                                                newRow["ccms_left3"] = summaryCorrectionData.NotesRemaining3.ToString();
                                                newRow["ccms_left4"] = summaryCorrectionData.NotesRemaining4.ToString();
                                            }
                                            else
                                            {
                                                newRow["ccms_left1"] = (parsedTrxn.CashRemaining1 - parsedTrxn.CashDispensed1 - parsedTrxn.CashPurged1 + notesPurgedInSupBeforeRep1).ToString();

                                                if (int.Parse(newRow["ccms_left1"].ToString()) < 0)
                                                    newRow["ccms_left1"] = 0;

                                                newRow["ccms_left2"] = (parsedTrxn.CashRemaining2 - parsedTrxn.CashDispensed2 - parsedTrxn.CashPurged2 + notesPurgedInSupBeforeRep2).ToString();
                                                if (int.Parse(newRow["ccms_left2"].ToString()) < 0)
                                                    newRow["ccms_left2"] = 0;

                                                newRow["ccms_left3"] = (parsedTrxn.CashRemaining3 - parsedTrxn.CashDispensed3 - parsedTrxn.CashPurged3 + notesPurgedInSupBeforeRep3).ToString();
                                                if (int.Parse(newRow["ccms_left3"].ToString()) < 0)
                                                    newRow["ccms_left3"] = 0;

                                                newRow["ccms_left4"] = (parsedTrxn.CashRemaining4 - parsedTrxn.CashDispensed4 - parsedTrxn.CashPurged4 + notesPurgedInSupBeforeRep4).ToString();
                                                if (int.Parse(newRow["ccms_left4"].ToString()) < 0)
                                                    newRow["ccms_left4"] = 0;

                                            }
                                            if (summaryCorrectionData != null)
                                            {
                                                newRow["ccms_returned1"] = (summaryCorrectionData.NotesRejected1 + summaryCorrectionData.NotesRemaining1).ToString();
                                                newRow["ccms_returned2"] = (summaryCorrectionData.NotesRejected2 + summaryCorrectionData.NotesRemaining2).ToString();
                                                newRow["ccms_returned3"] = (summaryCorrectionData.NotesRejected3 + summaryCorrectionData.NotesRemaining3).ToString();
                                                newRow["ccms_returned4"] = (summaryCorrectionData.NotesRejected4 + summaryCorrectionData.NotesRemaining4).ToString();
                                            }
                                            else
                                            {
                                                newRow["ccms_returned1"] = (int.Parse(newRow["ccms_left1"].ToString()) + CassettePurgedBin1 + testCashPurgedBin1).ToString();
                                                newRow["ccms_returned2"] = (int.Parse(newRow["ccms_left2"].ToString()) + CassettePurgedBin2 + testCashPurgedBin2).ToString();
                                                newRow["ccms_returned3"] = (int.Parse(newRow["ccms_left3"].ToString()) + CassettePurgedBin3 + testCashPurgedBin3).ToString();
                                                newRow["ccms_returned4"] = (int.Parse(newRow["ccms_left4"].ToString()) + CassettePurgedBin4 + testCashPurgedBin4).ToString();
                                            }
                                            if (summaryCorrectionData != null)
                                            {
                                                newRow["ccms_rejected1"] = summaryCorrectionData.NotesRejected1.ToString();
                                                newRow["ccms_rejected2"] = summaryCorrectionData.NotesRejected2.ToString();
                                                newRow["ccms_rejected3"] = summaryCorrectionData.NotesRejected3.ToString();
                                                newRow["ccms_rejected4"] = summaryCorrectionData.NotesRejected4.ToString();
                                            }
                                            else
                                            {
                                                newRow["ccms_rejected1"] = (CassettePurgedBin1 + testCashPurgedBin1).ToString();
                                                newRow["ccms_rejected2"] = (CassettePurgedBin2 + testCashPurgedBin2).ToString();
                                                newRow["ccms_rejected3"] = (CassettePurgedBin3 + testCashPurgedBin3).ToString();
                                                newRow["ccms_rejected4"] = (CassettePurgedBin4 + testCashPurgedBin4).ToString();
                                            }
                                            if (summaryCorrectionData != null)
                                            {
                                                newRow["ccms_dispensed1"] = summaryCorrectionData.NotesDispensed1.ToString();
                                                newRow["ccms_dispensed2"] = summaryCorrectionData.NotesDispensed2.ToString();
                                                newRow["ccms_dispensed3"] = summaryCorrectionData.NotesDispensed3.ToString();
                                                newRow["ccms_dispensed4"] = summaryCorrectionData.NotesDispensed4.ToString();
                                            }
                                            else
                                            {
                                                newRow["ccms_dispensed1"] = (replenishment.CashAdded1 - int.Parse(newRow["ccms_returned1"].ToString()) + Math.Abs(notesPurgedInSupBeforeRep1)).ToString();
                                                newRow["ccms_dispensed2"] = (replenishment.CashAdded2 - int.Parse(newRow["ccms_returned2"].ToString()) + Math.Abs(notesPurgedInSupBeforeRep2)).ToString();
                                                newRow["ccms_dispensed3"] = (replenishment.CashAdded3 - int.Parse(newRow["ccms_returned3"].ToString()) + Math.Abs(notesPurgedInSupBeforeRep3)).ToString();
                                                newRow["ccms_dispensed4"] = (replenishment.CashAdded4 - int.Parse(newRow["ccms_returned4"].ToString()) + Math.Abs(notesPurgedInSupBeforeRep4)).ToString();
                                            }

                                            newRow["ccms_returned_amount_text"] = ConnectionFactory.ExecuteScalar(string.Format(@"SELECT dbo.GetTotalText(
                                                denomination_type_1_title, denomination_type_2_title, denomination_type_3_title, denomination_type_4_title, 
                                                denomination_type_1, denomination_type_2, denomination_type_3, denomination_type_4,
                                                {0}, {1}, {2}, {3}) total_remaining_text FROM note_set_type where note_set_type_id = {4}",
                                                int.Parse(newRow["ccms_returned1"].ToString()), int.Parse(newRow["ccms_returned2"].ToString()), int.Parse(newRow["ccms_returned3"].ToString()), int.Parse(newRow["ccms_returned4"].ToString()), noteSetType.NoteSetTypeId)).ToString();

                                            newRow["diff1"] = (int.Parse(newRow["cit_returned1"].ToString()) - int.Parse(newRow["ccms_returned1"].ToString())).ToString();
                                            newRow["diff2"] = (int.Parse(newRow["cit_returned2"].ToString()) - int.Parse(newRow["ccms_returned2"].ToString())).ToString();
                                            newRow["diff3"] = (int.Parse(newRow["cit_returned3"].ToString()) - int.Parse(newRow["ccms_returned3"].ToString())).ToString();
                                            newRow["diff4"] = (int.Parse(newRow["cit_returned4"].ToString()) - int.Parse(newRow["ccms_returned4"].ToString())).ToString();
                                            newRow["diff_amount_text"] = ConnectionFactory.ExecuteScalar(string.Format(@"SELECT dbo.GetTotalText(
                                                denomination_type_1_title, denomination_type_2_title, denomination_type_3_title, denomination_type_4_title, 
                                                denomination_type_1, denomination_type_2, denomination_type_3, denomination_type_4,
                                                {0}, {1}, {2}, {3}) total_remaining_text FROM note_set_type where note_set_type_id = {4}",
                                                int.Parse(newRow["diff1"].ToString()), int.Parse(newRow["diff2"].ToString()), int.Parse(newRow["diff3"].ToString()), int.Parse(newRow["diff4"].ToString()), noteSetType.NoteSetTypeId)).ToString();
                                        }
                                    }
                                    //GetDispensedAmount(atm.ATMId, noteSetType, atmSettlement.PreviousRepDate.Value, atmSettlement.RepDatetime.Value);
                                }
                            }
                            
                        }
                        //}
                        Ds.Tables[0].Rows.Add(newRow);
                    }   
                }
                if (Ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                    string URL = "ReportPopup.aspx?Report=CITCCMSCounterRpt.rpt&ReportTitle=CIT Vs CCMS Counter Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count;
                    Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

        }

        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            ReportSchedule reportSchedule = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            try
            {
                if (Session["isOrgSelected"] != null)
                {
                    if (bool.Parse(Session["isOrgSelected"].ToString()))
                    {
                        cmd = ConnectionFactory.GetNewCommand(true);
                        trxn = cmd.Connection.BeginTransaction();
                        string orgID = Session["selectedOrgID"].ToString();
                        ConnectionFactory.ExecuteScalar(@"delete report_generation_schedule
                                                          where report_schedule_id in
                                                          (select  report_schedule_id 
                                                          from report_schedule 
                                                          where report_name='CITCCMSCounterReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='CITCCMSCounterReport' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "CITCCMSCounterReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"ATMSummaryRpt.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "CIT CCMS Counter Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = 1;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        reportSchedule.IsEjEnabled = false;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_error.Text = "";
                        Utility.BuildAuditLog("Schedule added for CIT CCMS Counter Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewCITCCMSCounterReport);
                    }
                    else
                        Literal_error.Text = "Please select organization from treeview.";
                }
                else
                    Literal_error.Text = "Please select organization from treeview.";
            }

            catch (Exception ex)
            {
                if (trxn != null)
                    trxn.Rollback();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);

            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }
    }
}