using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.CCMS.DAL;
using System.Data.SqlClient;

using System.Diagnostics;
using System.Data;
using System.Reflection;
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class PrintEnvelop : BasePage
    {
        private void GetDispensedAmount(int atmID, NoteSetType noteSetType, DateTime lastRepDateTime, DateTime currentRepDateTime)
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();

            try
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                //cmd.CommandText = "select max(rep_datetime) from replenishment where atm_id=" + atmID + " and rep_datetime<convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).AddDays(-1).ToString("dd/MM/yyyy") + "',103)";
                //object repDate = cmd.ExecuteScalar();
                //if (repDate != DBNull.Value)
                //{

                //cmd.CommandText = "select max(rep_datetime) from replenishment where atm_id=" + atmID +
                //    " and rep_datetime>=convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy") + "',103)" +
                //    " and rep_datetime<=convert(datetime,'" + DateTime.ParseExact(Label_ReplenishmentDate.Text, "dd/MM/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy") + " 23:59:59',103)";

                //object currentRepDate = cmd.ExecuteScalar();

                cmd.CommandText = @"select isnull(sum(cash_dispensed1),0),
                                                           isnull(sum(cash_dispensed2),0),
                                                           isnull(sum(cash_dispensed3),0),
                                                           isnull(sum(cash_dispensed4),0),
                                                           isnull(sum(cash_purged1),0),
                                                           isnull(sum(cash_purged2),0),
                                                           isnull(sum(cash_purged3),0),
                                                           isnull(sum(cash_purged4),0)

                                            from parsed_transaction 
                                            where parsed_transaction.atm_id = " + atmID +
                                    "and trxn_datetime>=convert(datetime,'" + lastRepDateTime.ToString("dd/MM/yyyy") + "',103) " +
                                    " and trxn_datetime<=convert(datetime,'" + currentRepDateTime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) ";


                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Label_CCMSDispensed1.Text = reader[0].ToString();
                    Label_CCMSDispensed2.Text = reader[1].ToString();
                    Label_CCMSDispensed3.Text = reader[2].ToString();
                    Label_CCMSDispensed4.Text = reader[3].ToString();

                    Label_CCMSRejected1.Text = reader[4].ToString();
                    Label_CCMSRejected2.Text = reader[5].ToString();
                    Label_CCMSRejected3.Text = reader[6].ToString();
                    Label_CCMSRejected4.Text = reader[7].ToString();


                }
                reader.Close();

                //}
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {

            Atm atm = null;
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




            if (!IsPostBack)
            {
                int atmSettlementID = int.Parse(Request.QueryString["s"]);

                AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlementByPk(atmSettlementID);
                if (atmSettlement != null)
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["citIdRequiresPadding"] == "1")
                        atm = Atm.LoadAtm("cit_atm_title = '" + atmSettlement.AtmNo.Value.ToString().PadLeft(8, '0') + "'");
                    else
                        atm = Atm.LoadAtm("cit_atm_title = '" + atmSettlement.AtmNo.Value + "'");
                    NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                    Label_DiffNoteSetType1.Text = Label_NoteSetType1.Text = noteSetType.DenominationType1.Value.ToString();
                    Label_DiffNoteSetType2.Text = Label_NoteSetType2.Text = noteSetType.DenominationType2.Value.ToString();
                    Label_DiffNoteSetType3.Text = Label_NoteSetType3.Text = noteSetType.DenominationType3.Value.ToString();
                    Label_DiffNoteSetType4.Text = Label_NoteSetType4.Text = noteSetType.DenominationType4.Value.ToString();

                    Label_ATMID.Text = atm.CitAtmTitle;
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


                    //AtmSettlement lastSettlement = AtmSettlement.LoadAtmSettlement(@"atm_no = '" + atmSettlement.AtmNo + "' and rep_datetime = convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + "',103)");
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
                        string SQL = "atm_id=" + atm.ATMId + " and rep_datetime>=convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + "',103)  " +
                            " and rep_datetime<=convert(datetime,'" + atmSettlement.PreviousRepDate.Value.ToString("dd/MM/yyyy") + " 23:59:59',103) order by rep_datetime desc";
                        LogableTask.LogMonoActivityTask("CheckSQL", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, SQL);
                        Avanza.CCMS.DAL.Replenishment replenishment = Avanza.CCMS.DAL.Replenishment.LoadReplenishment(SQL
                             );
                        if (replenishment != null)
                        {
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
                            //GetDispensedAmount(atm.ATMId, noteSetType, atmSettlement.PreviousRepDate.Value, atmSettlement.RepDatetime.Value);















                        }
                        else
                        {
                            //   task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "previous replenishment not found in CCMS for ATM title " + atm.Title);
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
                }//


            }
        }
    }
}