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
using Avanza.iSuite.DAL;
using System.Reflection;
using System.Diagnostics;
using iSuite;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace CCMSUI.CCMS
{
    public class TransactionHistory
    {
        public DateTime trxnDatetime;
        public decimal amount;
    }
    public partial class ForecastingList : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ViewATMTransactionForecast))
                        Response.Redirect("AccessDenied.aspx");
                    
                    if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                       Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                        Label_ATMTitle.Text = "Please, select an ATM";
                    else
                    {
                        if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                        {
                            Literal_error.Text = "<li>ATM not selected.</li>";
                            Button_Generate.Enabled = false;
                        }
                        else
                        {
                            Label_ATMTitle.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
                            TextBox_FromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy");
                            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                            TextBox_NoOfTripsPerWeek.Text = "3";
                            Textbox_type1.Text = "2000";
                            Textbox_type2.Text = "4000";
                            Textbox_type3.Text = "2000";
                            Textbox_type4.Text = "0";
                            TextBox_WindowSize.Text = "3";
                            Textbox_Den1.Text = "20";
                            Textbox_Den2.Text = "20";
                            Textbox_Den3.Text = "60";
                            Textbox_Den4.Text = "0";



                        }
                    }
                }

               
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask task = LogableTask.NewTask("ForecastingList");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
            }


        }



        protected void Button_Generate_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_Generate_Click");
            try
            {
                if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                {
                    Label_ATMTitle.Text = "Please, select an ATM";
                    Literal_error.Text = "<li>please select an ATM from the tree.</li>";
                    return;
                }
                else
                {
                    if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1
                        || Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                    {
                        Literal_error.Text = "<li>please select an ATM from the tree.</li>";
                        return;
                    }

                    GridView_forecasting.DataSource = GenerateForecastDataTable();
                    GridView_forecasting.DataBind();
                }
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
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                       Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                Label_ATMTitle.Text = "Please, select an ATM";
            else
            {
                if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                {
                    Literal_error.Text = "<li>ATM not selected.</li>";
                    Button_Generate.Enabled = false;
                }
                else
                {
                    Label_ATMTitle.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
                    TextBox_FromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    TextBox_NoOfTripsPerWeek.Text = "3";
                    Textbox_type1.Text = "2000";
                    Textbox_type2.Text = "4000";
                    Textbox_type3.Text = "2000";
                    Textbox_type4.Text = "0";
                    TextBox_WindowSize.Text = "3";
                    Textbox_Den1.Text = "20";
                    Textbox_Den2.Text = "20";
                    Textbox_Den3.Text = "60";
                    Textbox_Den4.Text = "0";



                }
            }
        }
        private static decimal MakeNumberDivisibleBy50(decimal number)
        {
            while (number % 50 != 0)
                number++;

            return number;
        }
        private static decimal MakeNumberDivisibleBy500(decimal number)
        {
            while (number % 500 != 0)
                number++;

            return number;
        }
        private static decimal MakeNumberDivisibleBy100(decimal number)
        {
            while (number % 100 != 0)
                number++;

            return number;
        }


        private static decimal MakeNumberDivisibleBy10(decimal number)
        {
            while (number % 10 != 0)
                number++;

            return number;
        }
        DataTable GenerateForecastDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("date");
            dt.Columns.Add("opening_balance");
            dt.Columns.Add("replenishment");
            dt.Columns.Add("pre_withdrawals");
            dt.Columns.Add("withdrawals");
            dt.Columns.Add("returns");
            dt.Columns.Add("closing_balance");
            dt.Columns.Add("type1");
            dt.Columns.Add("type2");
            dt.Columns.Add("type3");
            dt.Columns.Add("type4");
            // decimal standardOrderAmount = 50*1000+100*1000+500*2000;
            decimal openingBalance = 0;
            decimal rep = 0;
            decimal balance = 0;
            decimal preWithdrawals = 0;
            decimal returns = 0;

            decimal amountExpForType1 = 0;
            decimal amountExpForType2 = 0;
            decimal amountExpForType3 = 0;
            decimal amountExpForType4 = 0;

            int windowSize = int.Parse(TextBox_WindowSize.Text);
            List<TransactionHistory> withdrawalHistory = new List<TransactionHistory>();
            List<decimal> forecastedWithdrawals = new List<decimal>();
            List<decimal> withdrawals = new List<decimal>();
            //decimal forecastedWithdrawals = 0;
            TransactionHistory history = null;
            Atm atm = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString());
            NoteSetType notesetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
            decimal standardOrderAmount = (decimal)(notesetType.DenominationType1 * int.Parse(Textbox_type1.Text) +
                notesetType.DenominationType2 * int.Parse(Textbox_type2.Text) +
                notesetType.DenominationType3 * int.Parse(Textbox_type3.Text) + notesetType.DenominationType4 + int.Parse(Textbox_type4.Text));

            Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atm.ATMId + " and trxn_datetime >=convert(datetime,'" + DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null).AddDays(-windowSize).ToString("dd/MM/yyyy") + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + "',103) order by trxn_datetime");
            while (summaryReader.Read())
            {
                history = new TransactionHistory();
                history.trxnDatetime = summaryReader.CurrentSummary.TrxnDatetime;
                history.amount = summaryReader.CurrentSummary.Withdrawals;
                withdrawals.Add(summaryReader.CurrentSummary.Withdrawals);
                withdrawalHistory.Add(history);
            }
            summaryReader.Close();


            //int[] dataPoints = { 119, 72, 113, 82, 82, 131, 111, 116, 89, 95, 88, 90 };
            int toLoop = withdrawals.Count - windowSize;
            decimal sum = 0;
            for (int j = 0; j < toLoop ; j++)
            {
                sum = 0;
                for (int k = j; k < (j + windowSize); k++)
                    sum += withdrawals[k];
                forecastedWithdrawals.Add(MakeNumberDivisibleBy50(Math.Round(sum / windowSize)));
            }
            decimal expectedAmount=0;
            for (int j = 0; j < forecastedWithdrawals.Count; j++)
            {
                expectedAmount = 0;
                preWithdrawals = 0;
                amountExpForType1 = 0;
                amountExpForType2 = 0;
                amountExpForType3 = 0;
                amountExpForType4 = 0;

                returns = 0;
                //Console.WriteLine("Day " + (j + 1 + windowSize) + " -->" + Math.Round(sum / windowSize));
                DataRow dr = dt.NewRow();
                DateTime forecastedDateTime = withdrawalHistory[j].trxnDatetime.AddDays(windowSize).AddYears(1);
                dr[0] = forecastedDateTime.DayOfWeek + "," + forecastedDateTime.Day + "-" + forecastedDateTime.ToString("MMM") + "-" + forecastedDateTime.Year;

                if (openingBalance == 0)
                    openingBalance = standardOrderAmount;
                dr[1] = openingBalance;

                if (openingBalance < forecastedWithdrawals[j])
                {
                    decimal upperBound = Math.Floor(7 / decimal.Parse(TextBox_NoOfTripsPerWeek.Text));
                    if (j +1+ upperBound < forecastedWithdrawals.Count)
                    {
                        for (int k = 0; k < upperBound; k++)
                        {
                            expectedAmount += forecastedWithdrawals[k+j];
                        }
                        expectedAmount += forecastedWithdrawals[j + (int)upperBound] / 3;
                        rep = Math.Round(expectedAmount);
                        rep = MakeNumberDivisibleBy50(rep);
                        amountExpForType1 = Math.Round(rep * (decimal.Parse(Textbox_Den1.Text) / 100));
                        amountExpForType2 = Math.Floor(rep * (decimal.Parse(Textbox_Den2.Text) / 100));
                        amountExpForType3 = Math.Round(rep * (decimal.Parse(Textbox_Den3.Text) / 100));
                        amountExpForType4 = Math.Floor(rep * (decimal.Parse(Textbox_Den4.Text) / 100));

                        amountExpForType1 = MakeNumberDivisibleBy50(amountExpForType1);
                        amountExpForType2 = MakeNumberDivisibleBy100(amountExpForType2);
                        amountExpForType3 = MakeNumberDivisibleBy500(amountExpForType3);
                        amountExpForType4 = 0;

                        if (amountExpForType1 + amountExpForType2 + amountExpForType3 > rep)
                        {
                            decimal diff =  (amountExpForType1 + amountExpForType2 + amountExpForType3)-rep;

                            amountExpForType1 = amountExpForType1 - diff;
                        }

                            
                    }
                    else
                         rep = standardOrderAmount;
                    decimal notes = openingBalance / 50;


                    returns = Math.Floor(notes / 2)*50;   
                    preWithdrawals =Math.Round (notes / 2)*50;
                }
                else
                    rep = 0;
                
                dr[2] = rep;
                dr[3] = preWithdrawals;
                dr[4] = forecastedWithdrawals[j];
                dr[5] = returns;
                if (rep > 0)
                {
                    balance = rep - (forecastedWithdrawals[j]-preWithdrawals);
                }
                else
                    balance = openingBalance - forecastedWithdrawals[j];
                openingBalance = Math.Round(balance);
                dr[6] = openingBalance;
                dr[7] =Math.Round(amountExpForType1/50);
                dr[8] =Math.Round( amountExpForType2/100);
                dr[9] = Math.Round(amountExpForType3 / 500);
                dr[10] = amountExpForType4;

                dt.Rows.Add(dr);

            }
            return dt;
        }

        protected void Button_Add_Click(object sender, EventArgs e)
        {
            ListBox_Events.Items.Add(TextBox1.Text);
        }

          protected void Button_Remove_Click(object sender, EventArgs e)
        {
            if (ListBox_Events.SelectedIndex > -1)
                ListBox_Events.Items.RemoveAt(ListBox_Events.SelectedIndex);

        }
    }
}