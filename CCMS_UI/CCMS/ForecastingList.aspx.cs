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
using iSuite;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Data.SqlClient;
using System.Collections.Generic;
using ComponentArt.Web.UI;
using System.Text;

namespace CCMSUI.CCMS
{
    public class TransactionHistory
    {
        public DateTime trxnDatetime;
        public decimal amount;
    }
    public partial class ForecastingList : BasePage
    {

        int expectedTrips = 0;
        decimal expectedAmount = 0;

        string[] color = { "#93bb49", "#ffc83c", "#fe4b4b", "#ffffff" };
        Random r = new Random();
        //List<DateTime> events = new List<DateTime>();
        static List<DateTime> events = new List<DateTime>();
        protected void Page_PreInit(object sender, EventArgs e)
        {

            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        private void DrawGraph()
        {
            int[] data1 = { 110450, 91450, 76900, 118950, 101800, 109950, 172900 };
            int[] data2 = { 835250, 595500, 337200, 335850, 141700, 214400, 220000 };

            int[] data3 = { 15, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, };


            this.Chart4.Series[0].Points.AddXY(DateTime.Now.AddDays(0).DayOfWeek.ToString(), data1[0]);
            this.Chart4.Series[0].Points.AddXY(DateTime.Now.AddDays(-1).DayOfWeek.ToString(), data1[1]);
            this.Chart4.Series[0].Points.AddXY(DateTime.Now.AddDays(-2).DayOfWeek.ToString(), data1[2]);
            this.Chart4.Series[0].Points.AddXY(DateTime.Now.AddDays(-3).DayOfWeek.ToString(), data1[3]);
            this.Chart4.Series[0].Points.AddXY(DateTime.Now.AddDays(-4).DayOfWeek.ToString(), data1[4]);
            this.Chart4.Series[0].Points.AddXY(DateTime.Now.AddDays(-5).DayOfWeek.ToString(), data1[5]);
            this.Chart4.Series[0].Points.AddXY(DateTime.Now.AddDays(-6).DayOfWeek.ToString(), data1[6]);

            this.Chart4.Series[1].Points.AddXY(DateTime.Now.AddDays(0).DayOfWeek.ToString(), data2[0]);
            this.Chart4.Series[1].Points.AddXY(DateTime.Now.AddDays(-1).DayOfWeek.ToString(), data2[1]);
            this.Chart4.Series[1].Points.AddXY(DateTime.Now.AddDays(-2).DayOfWeek.ToString(), data2[2]);
            this.Chart4.Series[1].Points.AddXY(DateTime.Now.AddDays(-3).DayOfWeek.ToString(), data2[3]);
            this.Chart4.Series[1].Points.AddXY(DateTime.Now.AddDays(-4).DayOfWeek.ToString(), data2[4]);
            this.Chart4.Series[1].Points.AddXY(DateTime.Now.AddDays(-5).DayOfWeek.ToString(), data2[5]);
            this.Chart4.Series[1].Points.AddXY(DateTime.Now.AddDays(-6).DayOfWeek.ToString(), data2[6]);













            this.Chart5.Series[0].Points.AddXY("Jan 2012", data3[0]);
            this.Chart5.Series[0].Points.AddXY("Feb 2012", data3[1]);
            this.Chart5.Series[0].Points.AddXY("Mar 2012", data3[2]);
            this.Chart5.Series[0].Points.AddXY("Apr 2012", data3[3]);
            this.Chart5.Series[0].Points.AddXY("May 2012", data3[4]);
            this.Chart5.Series[0].Points.AddXY("Jun 2012", data3[5]);
            this.Chart5.Series[0].Points.AddXY("Jul 2012", data3[6]);
            this.Chart5.Series[0].Points.AddXY("Aug 2012", data3[7]);
            this.Chart5.Series[0].Points.AddXY("Sep 2012", data3[8]);
            //this.Chart5.Series[0].Points.AddXY("Oct 2012", data3[9]);
            //this.Chart5.Series[0].Points.AddXY("Nov 2012", data3[10]);
            //this.Chart5.Series[0].Points.AddXY("Dec 2012", data3[11]);
        }
        private void AnalyzeATMsHistory(DateTime historyStartDateTime)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("missingCount");
            dt.Columns.Add("atm_id");
            dt.Columns.Add("location");

            int gridCount = 0;
            int missingCount = 0;
            List<DateTime> listATMsHistory = null;
            Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1 and is_active=1");
            DateTime tempDateTime = historyStartDateTime;
            while (atmReader.Read())
            {
                listATMsHistory = new List<DateTime>();
                //historyStartDateTime = tempDateTime;
                missingCount = 0;
                Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atmReader.CurrentAtm.ATMId);
                while (summaryReader.Read())
                    listATMsHistory.Add(summaryReader.CurrentSummary.TrxnDatetime);
                summaryReader.Close();

                DateTime to = historyStartDateTime.AddMonths(13);
                int count = (to - historyStartDateTime).Days;
                for (int j = 0; j < count; j++)
                {
                    tempDateTime = historyStartDateTime.AddDays(j);
                    if (!listATMsHistory.Contains(tempDateTime))
                        missingCount++;
                }
                DataRow dr = dt.NewRow();
                dr[0] = atmReader.CurrentAtm.Title;
                dr[1] = missingCount.ToString();
                dr[2] = atmReader.CurrentAtm.ATMId;
                dr[3] = atmReader.CurrentAtm.Location;

                dt.Rows.Add(dr);
                gridCount++;
                if (gridCount == 30)
                {
                    GridView_MissingHistory.DataSource = dt;
                    GridView_MissingHistory.DataBind();
                    dt.Rows.Clear();
                }
                else if (gridCount == 60)
                {
                    GridView_MissingHistory_1.DataSource = dt;
                    GridView_MissingHistory_1.DataBind();
                    dt.Rows.Clear();
                }
                else if (gridCount == 90)
                {
                    GridView_MissingHistory_2.DataSource = dt;
                    GridView_MissingHistory_2.DataBind();
                    dt.Rows.Clear();
                }

            }
            atmReader.Close();
            GridView_MissingHistory_3.DataSource = dt;
            GridView_MissingHistory_3.DataBind();
        }



        private void GroupATMsIntoZone(DateTime historyStartDateTime)
        {
            DataTable dtOrange = new DataTable();
            dtOrange.Columns.Add("title");
            dtOrange.Columns.Add("atm_id");
            dtOrange.Columns.Add("location");
            DataTable dtGreen = dtOrange.Clone();
            DataTable dtRed = dtOrange.Clone();



            List<decimal> listATMsWithdrawals = new List<decimal>();
            Hashtable ht = null;
            Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1 and is_active=1");
            int key = 0;
            DateTime tempDateTime = historyStartDateTime;
            while (atmReader.Read())
            {
                ht = new Hashtable(3);
                //historyStartDateTime = tempDate;
                Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atmReader.CurrentAtm.ATMId);

                while (summaryReader.Read())
                {

                    if (summaryReader.CurrentSummary.Withdrawals >= 0 && summaryReader.CurrentSummary.Withdrawals <= 100000)
                        key = 1;
                    else if (summaryReader.CurrentSummary.Withdrawals > 100000 && summaryReader.CurrentSummary.Withdrawals <= 200000)
                        key = 2;
                    else if (summaryReader.CurrentSummary.Withdrawals > 200000 && summaryReader.CurrentSummary.Withdrawals <= 300000)
                        key = 3;
                    else if (summaryReader.CurrentSummary.Withdrawals > 300000 && summaryReader.CurrentSummary.Withdrawals <= 400000)
                        key = 4;
                    else if (summaryReader.CurrentSummary.Withdrawals > 400000 && summaryReader.CurrentSummary.Withdrawals <= 500000)
                        key = 5;
                    else if (summaryReader.CurrentSummary.Withdrawals > 500000 && summaryReader.CurrentSummary.Withdrawals <= 600000)
                        key = 6;
                    else if (summaryReader.CurrentSummary.Withdrawals > 600000 && summaryReader.CurrentSummary.Withdrawals <= 700000)
                        key = 7;
                    else if (summaryReader.CurrentSummary.Withdrawals > 700000 && summaryReader.CurrentSummary.Withdrawals <= 800000)
                        key = 8;
                    else if (summaryReader.CurrentSummary.Withdrawals > 800000 && summaryReader.CurrentSummary.Withdrawals <= 900000)
                        key = 9;
                    else if (summaryReader.CurrentSummary.Withdrawals > 900000 && summaryReader.CurrentSummary.Withdrawals <= 1000000)
                        key = 10;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1000000 && summaryReader.CurrentSummary.Withdrawals <= 1100000)
                        key = 11;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1100000 && summaryReader.CurrentSummary.Withdrawals <= 1200000)
                        key = 12;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1200000 && summaryReader.CurrentSummary.Withdrawals <= 1300000)
                        key = 13;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1300000 && summaryReader.CurrentSummary.Withdrawals <= 1400000)
                        key = 14;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1400000 && summaryReader.CurrentSummary.Withdrawals <= 1500000)
                        key = 15;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1500000 && summaryReader.CurrentSummary.Withdrawals <= 1600000)
                        key = 16;


                    if (ht.Contains(key))
                        ht[key] = (int)ht[key] + 1;
                    else
                        ht.Add(key, 1);

                }
                summaryReader.Close();

                int sum2 = 0; int sum1 = 0; decimal temp = 0;
                foreach (DictionaryEntry de in ht)
                {
                    sum2 += int.Parse(de.Value.ToString());
                    sum1 += int.Parse(de.Key.ToString()) * int.Parse(de.Value.ToString());
                }

                if (sum2 != 0)
                {
                    temp = Math.Round((decimal)sum1 / sum2);
                    temp = temp * 100000;
                }


                DateTime to = historyStartDateTime.AddMonths(13);
                int count = (to - historyStartDateTime).Days;
                int divisor = 0;
                int tdivisor = 0;
                decimal var = 0;
                decimal result = 0;

                for (int j = 0; j < 12; j++)
                {
                    var = 0;
                    divisor = 0;
                    tempDateTime = historyStartDateTime.AddMonths(j);
                    summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atmReader.CurrentAtm.ATMId + " and trxn_datetime>=convert(datetime,'" + tempDateTime.ToString("dd/MM/yyyy") + "',103) and trxn_datetime<convert(datetime,'" + tempDateTime.AddMonths(1).ToString("dd/MM/yyyy") + " 23:59:59',103)");

                    while (summaryReader.Read())
                    {
                        if (temp != 0)
                        {
                            var += ((temp - summaryReader.CurrentSummary.Withdrawals) / temp) * 100;
                            divisor++;
                        }
                    }
                    summaryReader.Close();
                    if (divisor != 0)
                    {
                        result += Math.Round(var / divisor, 2);
                        tdivisor++;
                    }
                }
                if (tdivisor == 0)
                {
                    result = 100;
                    tdivisor = 1;
                }

                if (tdivisor != 0)
                {
                    result = Math.Round(result / tdivisor, 2);
                    if (result >= 0 && result <= 30)
                    {
                        DataRow dr = dtGreen.NewRow();
                        dr[0] = atmReader.CurrentAtm.Title;
                        dr[1] = atmReader.CurrentAtm.ATMId;
                        dr[2] = atmReader.CurrentAtm.Location;

                        dtGreen.Rows.Add(dr);

                    }
                    else if (result >= 31 && result <= 60)
                    {
                        DataRow dr = dtOrange.NewRow();
                        dr[0] = atmReader.CurrentAtm.Title;
                        dr[1] = atmReader.CurrentAtm.ATMId;
                        dr[2] = atmReader.CurrentAtm.Location;

                        dtOrange.Rows.Add(dr);

                    }
                    else if (result >= 61 && result <= 100)
                    {
                        DataRow dr = dtRed.NewRow();
                        dr[0] = atmReader.CurrentAtm.Title;
                        dr[1] = atmReader.CurrentAtm.ATMId;
                        dr[2] = atmReader.CurrentAtm.Location;

                        dtRed.Rows.Add(dr);
                    }


                }
            }
            atmReader.Close();
            GridView_Orange.DataSource = dtOrange;
            GridView_Orange.DataBind();
            GridView_RedZone.DataSource = dtRed;
            GridView_RedZone.DataBind();
            GridView_GreenZone.DataSource = dtGreen;
            GridView_GreenZone.DataBind();
            Session["dtOrange"] = dtOrange;
            Session["dtRed"] = dtRed;
            Session["dtGreen"] = dtGreen;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (DialogCallBack.CausedCallback)
                    return;

                if (!Page.IsPostBack)
                {
                    Panel1.Visible = true;
                    Panel_DisplayHistoryOfMissingATMs.Visible = false;
                    Panel_ATMZone.Visible = false;
                    Panel_RepGroup.Visible = false;

                    //fileUploadCtrlRow.Visible = false;
                    DrawGraph();
                    Button_GenerateOrderWithLeadTime.Visible = false;
                    Button_GenerateOrder.Visible = false;
                    //GetTotalReplenishments(0);
                    //BindWithdrawalGrids();
                    //DoReplenishmentAnalysis();

                    if (!CheckRight((int)Permissions.ViewATMTransactionForecast))
                        Response.Redirect("AccessDenied.aspx");

                    if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                       Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                        Label1.Text = Label_ATMTitle.Text = "Please, select an ATM";
                    else
                    {
                        if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                        {
                            Literal_error.Text = "<li>ATM not selected.</li>";
                            Button_Generate.Enabled = false;
                        }
                        else
                        {
                            Label1.Text = Label_ATMTitle.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
                            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            TextBox_FromDate.Text = from.ToString("dd/MM/yyyy");
                            TextBox_ToDate.Text = from.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");
                            TextBox_NoOfTripsPerWeek.Text = "3";
                            Textbox_type1.Text = "2000";
                            Textbox_type2.Text = "4000";
                            Textbox_type3.Text = "2000";
                            Textbox_type4.Text = "0";
                            //TextBox_WindowSize.Text = "3";
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


        protected void Button_GenerateATMTrend_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateATMTrend_Click");

            try
            {
                DisplayTrend();

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


        protected void Button_GenerateOrder_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateOrder_Click");
            int count = 0;
            try
            {
                foreach (GridViewRow row in GridView_forecasting.Rows)
                {
                    string atm_id = this.GridView_forecasting.DataKeys[row.DataItemIndex].Values["atm_id"].ToString();

                    CheckBox chk = (CheckBox)row.FindControl("chkbx");
                    if (chk.Checked)
                    {
                        string date = this.GridView_forecasting.Rows[row.RowIndex].Cells[1].Text;
                        decimal rep = decimal.Parse(this.GridView_forecasting.Rows[row.RowIndex].Cells[3].Text);
                        decimal type1 = decimal.Parse(this.GridView_forecasting.Rows[row.RowIndex].Cells[8].Text);
                        decimal type2 = decimal.Parse(this.GridView_forecasting.Rows[row.RowIndex].Cells[9].Text);
                        decimal type3 = decimal.Parse(this.GridView_forecasting.Rows[row.RowIndex].Cells[10].Text);

                        count++;
                    }


                }
                if (count > 0)
                    Literal_error.Text = "<li>" + count + " Orders and email generated successfully!</li>";

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



        protected void Button_GenerateOrderWithLeadTime_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateOrderWithLeadTime_Click");
            int count = 0;
            try
            {
                foreach (GridViewRow row in GridView4.Rows)
                {
                    string atm_id = this.GridView4.DataKeys[row.DataItemIndex].Values["atm_id"].ToString();

                    CheckBox chk = (CheckBox)row.FindControl("chkbx");
                    if (chk.Checked)
                    {

                        string date = this.GridView_forecasting.Rows[row.RowIndex].Cells[2].Text;
                        decimal rep = decimal.Parse(this.GridView_forecasting.Rows[row.RowIndex].Cells[4].Text);
                        decimal type1 = decimal.Parse(this.GridView_forecasting.Rows[row.RowIndex].Cells[9].Text);
                        decimal type2 = decimal.Parse(this.GridView_forecasting.Rows[row.RowIndex].Cells[10].Text);
                        decimal type3 = decimal.Parse(this.GridView_forecasting.Rows[row.RowIndex].Cells[11].Text);

                        count++;
                    }


                }
                if (count > 0)
                    Literal_error.Text = "<li>" + count + " Orders and email generated successfully!</li>";

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
                Label1.Text = Label_ATMTitle.Text = "Please, select an ATM";
            else
            {
                if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                {
                    Literal_error.Text = "<li>ATM not selected.</li>";
                    Button_Generate.Enabled = false;
                }
                else
                {
                    Label1.Text = Label_ATMTitle.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
                    DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    TextBox_FromDate.Text = from.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = from.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");
                    TextBox_NoOfTripsPerWeek.Text = "3";
                    Textbox_type1.Text = "2000";
                    Textbox_type2.Text = "4000";
                    Textbox_type3.Text = "2000";
                    Textbox_type4.Text = "0";
                    // TextBox_WindowSize.Text = "3";
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
        private void AddToDataTable(DataTable dt, string val1, string val2, string val3, string val4, string val5, int atmID, int count)
        {
            DataRow dr = dt.NewRow();
            dr[0] = val1;
            dr[1] = val2;
            dr[2] = val3;
            dr[3] = val4;
            dr[4] = val5;
            dr[5] = ConnectionFactory.ExecuteScalar("select count(summary_id) from summary where atm_id=" + atmID + " and withdrawals>" + val4);
            dr[6] = count;
            dt.Rows.Add(dr);
        }



        private void BindReplenishmentGrids()
        {
            Hashtable ht = null;
            int key = 0;

            DataTable dtReplenishment1 = new DataTable();
            dtReplenishment1.Columns.Add("title");
            dtReplenishment1.Columns.Add("location");
            dtReplenishment1.Columns.Add("frequency");
            dtReplenishment1.Columns.Add("amount");
            dtReplenishment1.Columns.Add("atm_id");
            dtReplenishment1.Columns.Add("efrequency");
            
            int dailyCITTrips = 0;
            int onceAWeekCITTrips = 0;
            int twiceAWeekCITTrips = 0;
            int thriceAWeekCITTrips = 0;
            int biWeeklyCITTrips = 0;
            int monthlyCITTrips = 0;


            decimal dailyAmount = 0;
            decimal onceAWeekAmount = 0;
            decimal twiceAWeekAmount = 0;
            decimal thriceAWeekAmount = 0;
            decimal biWeeklyAmount = 0;
            decimal monthlyAmount = 0;



            DataTable dtReplenishment2 = dtReplenishment1.Clone();
            DataTable dtReplenishment3 = dtReplenishment1.Clone();
            DataTable dtReplenishment4 = dtReplenishment1.Clone();
            DataTable dtReplenishment5 = dtReplenishment1.Clone();
            DataTable dtReplenishment6 = dtReplenishment1.Clone();
            DataTable dtReplenishment7 = dtReplenishment1.Clone();
            DataTable dtResult = dtReplenishment1.Clone();


            Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1 and is_active=1");
            while (atmReader.Read())
            {
                ht = new Hashtable(3);
                Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atmReader.CurrentAtm.ATMId);
                while (summaryReader.Read())
                {
                    if (summaryReader.CurrentSummary.Withdrawals >= 0 && summaryReader.CurrentSummary.Withdrawals <= 100000)
                        key = 1;
                    else if (summaryReader.CurrentSummary.Withdrawals > 100000 && summaryReader.CurrentSummary.Withdrawals <= 200000)
                        key = 2;
                    else if (summaryReader.CurrentSummary.Withdrawals > 200000 && summaryReader.CurrentSummary.Withdrawals <= 300000)
                        key = 3;
                    else if (summaryReader.CurrentSummary.Withdrawals > 300000 && summaryReader.CurrentSummary.Withdrawals <= 400000)
                        key = 4;
                    else if (summaryReader.CurrentSummary.Withdrawals > 400000 && summaryReader.CurrentSummary.Withdrawals <= 500000)
                        key = 5;
                    else if (summaryReader.CurrentSummary.Withdrawals > 500000 && summaryReader.CurrentSummary.Withdrawals <= 600000)
                        key = 6;
                    else if (summaryReader.CurrentSummary.Withdrawals > 600000 && summaryReader.CurrentSummary.Withdrawals <= 700000)
                        key = 7;
                    else if (summaryReader.CurrentSummary.Withdrawals > 700000 && summaryReader.CurrentSummary.Withdrawals <= 800000)
                        key = 8;
                    else if (summaryReader.CurrentSummary.Withdrawals > 800000 && summaryReader.CurrentSummary.Withdrawals <= 900000)
                        key = 9;
                    else if (summaryReader.CurrentSummary.Withdrawals > 900000 && summaryReader.CurrentSummary.Withdrawals <= 1000000)
                        key = 10;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1000000 && summaryReader.CurrentSummary.Withdrawals <= 1100000)
                        key = 11;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1100000 && summaryReader.CurrentSummary.Withdrawals <= 1200000)
                        key = 12;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1200000 && summaryReader.CurrentSummary.Withdrawals <= 1300000)
                        key = 13;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1300000 && summaryReader.CurrentSummary.Withdrawals <= 1400000)
                        key = 14;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1400000 && summaryReader.CurrentSummary.Withdrawals <= 1500000)
                        key = 15;
                    else if (summaryReader.CurrentSummary.Withdrawals > 1500000 && summaryReader.CurrentSummary.Withdrawals <= 1600000)
                        key = 16;


                    if (ht.Contains(key))
                        ht[key] = (int)ht[key] + 1;
                    else
                        ht.Add(key, 1);
                }
                summaryReader.Close();
                int sum2 = 0; int sum1 = 0; decimal temp = 0;
                int fixedAmount = 1350000;
                foreach (DictionaryEntry de in ht)
                {
                    sum2 += int.Parse(de.Value.ToString());
                    sum1 += int.Parse(de.Key.ToString()) * int.Parse(de.Value.ToString());
                }

                if (sum2 != 0)
                {
                    temp = Math.Round((decimal)sum1 / sum2);
                    temp = temp * 100000;
                }
                if (temp >= 600000)
                {
                    DataRow dr = dtReplenishment1.NewRow();
                    dr[0] = atmReader.CurrentAtm.Title;
                    dr[1] = atmReader.CurrentAtm.Location;
                    dr[2] = "Daily";
                    dr[3] = fixedAmount;
                    dr[4] = atmReader.CurrentAtm.ATMId;
                    dr[5] = DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId);
                    dtReplenishment1.Rows.Add(dr);
                    dailyCITTrips++;
                    dailyAmount += fixedAmount;

                }
                else if (temp >= 100000 && temp < 200000)
                {
                    DataRow dr = dtReplenishment5.NewRow();
                    dr[0] = atmReader.CurrentAtm.Title;
                    dr[1] = atmReader.CurrentAtm.Location;
                    dr[2] = "Once a Week";
                    dr[3] = fixedAmount;
                    dr[4] = atmReader.CurrentAtm.ATMId;
                    dr[5] = DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId);
                    dtReplenishment5.Rows.Add(dr);
                    onceAWeekCITTrips++;
                    onceAWeekAmount += fixedAmount;
                }
                else if (temp >= 200000 && temp < 400000)
                {
                    DataRow dr = dtReplenishment4.NewRow();
                    dr[0] = atmReader.CurrentAtm.Title;
                    dr[1] = atmReader.CurrentAtm.Location;
                    dr[2] = "Twice a week";
                    dr[3] = fixedAmount * 2;
                    dr[4] = atmReader.CurrentAtm.ATMId;
                    dr[5] = DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId);
                    dtReplenishment4.Rows.Add(dr);
                    twiceAWeekCITTrips++;
                    twiceAWeekAmount += fixedAmount * 2;

                }
                //else if (temp >= 500000 && temp < 600000)
                //{
                //    DataRow dr = dtReplenishment2.NewRow();
                //    dr[0] = atmReader.CurrentAtm.Title;
                //    dr[1] = atmReader.CurrentAtm.Location;
                //    dr[2] = "Thrice a week";
                //    dr[3] = temp;
                //    dr[4] = atmReader.CurrentAtm.ATMId;
                //    dr[5] = DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId);
                //    dtReplenishment2.Rows.Add(dr);
                //}

                else if (temp >= 400000 && temp < 600000 )//temp < 500000
                {
                    DataRow dr = dtReplenishment3.NewRow();
                    dr[0] = atmReader.CurrentAtm.Title;
                    dr[1] = atmReader.CurrentAtm.Location;
                    dr[2] = "Thrice a week";
                    dr[3] = fixedAmount * 3;
                    dr[4] = atmReader.CurrentAtm.ATMId;
                    dr[5] = DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId);
                    dtReplenishment3.Rows.Add(dr);
                    thriceAWeekCITTrips++;
                    thriceAWeekAmount += fixedAmount * 3; 

                }
                
               
                else if (temp >= 50000 && temp < 100000)
                {
                    DataRow dr = dtReplenishment6.NewRow();
                    dr[0] = atmReader.CurrentAtm.Title;
                    dr[1] = atmReader.CurrentAtm.Location;
                    dr[2] = "Biweekly";
                    dr[3] = fixedAmount * 2;
                    dr[4] = atmReader.CurrentAtm.ATMId;
                    dr[5] = DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId);
                    dtReplenishment6.Rows.Add(dr);
                    biWeeklyCITTrips++;
                    biWeeklyAmount += fixedAmount * 2; 
                }
                else if (temp < 50000)
                {
                    DataRow dr = dtReplenishment7.NewRow();
                    dr[0] = atmReader.CurrentAtm.Title;
                    dr[1] = atmReader.CurrentAtm.Location;
                    dr[2] = "Monthly";
                    dr[3] = fixedAmount;
                    dr[4] = atmReader.CurrentAtm.ATMId;
                    dr[5] = DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId);
                    dtReplenishment7.Rows.Add(dr);
                    monthlyCITTrips++;
                    monthlyAmount += fixedAmount;
                }



            }

            atmReader.Close();

            GridView_DailyRep.DataSource = dtReplenishment1;
            GridView_DailyRep.DataBind();
            
            GridView_1perweek.DataSource = dtReplenishment5;
            GridView_1perweek.DataBind();


            GridView_2perweek.DataSource = dtReplenishment4;
            GridView_2perweek.DataBind();


            GridView_3perweek.DataSource = dtReplenishment3;
            GridView_3perweek.DataBind();


          
            GridView_biweekly.DataSource = dtReplenishment6;
            GridView_biweekly.DataBind();
            GridView_monthly.DataSource = dtReplenishment7;
            GridView_monthly.DataBind();

            dtResult.Merge(dtReplenishment1);
            //dtResult.Merge(dtReplenishment2);
            dtResult.Merge(dtReplenishment3);
            dtResult.Merge(dtReplenishment4);
            dtResult.Merge(dtReplenishment5);
            dtResult.Merge(dtReplenishment6);
            dtResult.Merge(dtReplenishment6);
            dtResult.Merge(dtReplenishment7);

            GridView5.DataSource = dtResult;
            GridView5.DataBind();
            Session["dtResult"] = dtResult;


            hidden_info.Value = dailyCITTrips + "," + onceAWeekCITTrips + "," + twiceAWeekCITTrips + "," + thriceAWeekCITTrips + "," + biWeeklyCITTrips + "," + monthlyCITTrips +
                "," + dailyAmount + "," + onceAWeekAmount + "," + twiceAWeekAmount + "," + thriceAWeekAmount + "," + biWeeklyAmount + "," + monthlyAmount;


            //GridView6.DataSource = dtWithdrawals2;
            //GridView6.DataBind();

            //GridView7.DataSource = dtWithdrawals3;
            //GridView7.DataBind();

            //GridView8.DataSource = dtWithdrawals4;
            //GridView8.DataBind();

            //GridView9.DataSource = dtWithdrawals5;
            //GridView9.DataBind();

            //GridView10.DataSource = dtWithdrawals6;
            //GridView10.DataBind();

            //GridView11.DataSource = dtWithdrawals7;
            //GridView11.DataBind();

            //GridView12.DataSource = dtWithdrawals8;
            //GridView12.DataBind();

            //GridView13.DataSource = dtWithdrawals9;
            //GridView13.DataBind();

            //GridView14.DataSource = dtWithdrawals10;
            //GridView14.DataBind();

            //GridView15.DataSource = dtWithdrawals11;
            //GridView15.DataBind();

            //GridView16.DataSource = dtWithdrawals12;
            //GridView16.DataBind();

            //GridView17.DataSource = dtWithdrawals13;
            //GridView17.DataBind();

            //GridView18.DataSource = dtWithdrawals14;
            //GridView18.DataBind();

            //GridView19.DataSource = dtWithdrawals15;
            //GridView19.DataBind();

            //GridView20.DataSource = dtWithdrawals16;
            //GridView20.DataBind();

            //GridView21.DataSource = dtWithdrawals17;
            //GridView21.DataBind();

            //GridView22.DataSource = dtWithdrawals18;
            //GridView22.DataBind();

            //Label_ExpAmount.Text = expectedAmount.ToString();
            //Label_CITtrips.Text = expectedTrips.ToString();

        }


        private void BindWithdrawalGrids()
        {
            decimal sum = 0;
            decimal avg = 0;
            int count = 0;


            DataTable dtWithdrawals1 = new DataTable();
            dtWithdrawals1.Columns.Add("region");
            dtWithdrawals1.Columns.Add("title");
            dtWithdrawals1.Columns.Add("location");
            dtWithdrawals1.Columns.Add("amount");
            dtWithdrawals1.Columns.Add("cycle");
            dtWithdrawals1.Columns.Add("deviation");
            dtWithdrawals1.Columns.Add("total");
            DataTable dtWithdrawals2 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals3 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals4 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals5 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals6 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals7 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals8 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals9 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals10 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals11 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals12 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals13 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals14 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals15 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals16 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals17 = dtWithdrawals1.Clone();
            DataTable dtWithdrawals18 = dtWithdrawals1.Clone();


            Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1 and is_active=1");
            while (atmReader.Read())
            {
                sum = 0;
                count = 0;

                Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atmReader.CurrentAtm.ATMId);
                while (summaryReader.Read())
                {
                    sum += summaryReader.CurrentSummary.Withdrawals;
                    count++;
                }
                summaryReader.Close();
                if (count != 0)
                {
                    avg = sum / count;
                    if (avg >= 0 && avg < 100000)
                    {
                        AddToDataTable(dtWithdrawals1, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 100000 && avg < 200000)
                    {
                        AddToDataTable(dtWithdrawals2, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 200000 && avg < 300000)
                    {
                        AddToDataTable(dtWithdrawals3, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 300000 && avg < 400000)
                    {
                        AddToDataTable(dtWithdrawals4, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 400000 && avg < 500000)
                    {
                        AddToDataTable(dtWithdrawals5, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 500000 && avg < 600000)
                    {
                        AddToDataTable(dtWithdrawals6, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 600000 && avg < 700000)
                    {
                        AddToDataTable(dtWithdrawals7, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 700000 && avg < 800000)
                    {
                        AddToDataTable(dtWithdrawals8, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 800000 && avg < 900000)
                    {
                        AddToDataTable(dtWithdrawals9, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }

                    else if (avg >= 900000 && avg < 1000000)
                    {
                        AddToDataTable(dtWithdrawals10, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 1000000 && avg < 1100000)
                    {
                        AddToDataTable(dtWithdrawals11, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 1100000 && avg < 1200000)
                    {
                        AddToDataTable(dtWithdrawals12, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 1200000 && avg < 1300000)
                    {
                        AddToDataTable(dtWithdrawals13, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 1300000 && avg < 1400000)
                    {
                        AddToDataTable(dtWithdrawals14, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 1400000 && avg < 1500000)
                    {
                        AddToDataTable(dtWithdrawals15, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 1500000 && avg < 1600000)
                    {
                        AddToDataTable(dtWithdrawals16, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 1600000 && avg < 1700000)
                    {
                        AddToDataTable(dtWithdrawals17, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }
                    else if (avg >= 1700000 && avg < 1800000)
                    {
                        AddToDataTable(dtWithdrawals18, Region.LoadRegionByPk(atmReader.CurrentAtm.RegionId).RegionName, atmReader.CurrentAtm.Title, atmReader.CurrentAtm.Location, Math.Round(avg).ToString(), DoReplenishmentAnalysis(atmReader.CurrentAtm.ATMId).ToString(), atmReader.CurrentAtm.ATMId, count);
                    }




                }


            }

            atmReader.Close();
            GridView5.DataSource = dtWithdrawals1;
            GridView5.DataBind();

            //GridView6.DataSource = dtWithdrawals2;
            //GridView6.DataBind();

            //GridView7.DataSource = dtWithdrawals3;
            //GridView7.DataBind();

            //GridView8.DataSource = dtWithdrawals4;
            //GridView8.DataBind();

            //GridView9.DataSource = dtWithdrawals5;
            //GridView9.DataBind();

            //GridView10.DataSource = dtWithdrawals6;
            //GridView10.DataBind();

            //GridView11.DataSource = dtWithdrawals7;
            //GridView11.DataBind();

            //GridView12.DataSource = dtWithdrawals8;
            //GridView12.DataBind();

            //GridView13.DataSource = dtWithdrawals9;
            //GridView13.DataBind();

            //GridView14.DataSource = dtWithdrawals10;
            //GridView14.DataBind();

            //GridView15.DataSource = dtWithdrawals11;
            //GridView15.DataBind();

            //GridView16.DataSource = dtWithdrawals12;
            //GridView16.DataBind();

            //GridView17.DataSource = dtWithdrawals13;
            //GridView17.DataBind();

            //GridView18.DataSource = dtWithdrawals14;
            //GridView18.DataBind();

            //GridView19.DataSource = dtWithdrawals15;
            //GridView19.DataBind();

            //GridView20.DataSource = dtWithdrawals16;
            //GridView20.DataBind();

            //GridView21.DataSource = dtWithdrawals17;
            //GridView21.DataBind();

            //GridView22.DataSource = dtWithdrawals18;
            //GridView22.DataBind();

            //Label_ExpAmount.Text = expectedAmount.ToString();
            //Label_CITtrips.Text = expectedTrips.ToString();

        }

        private int DoReplenishmentAnalysis(int atmID)
        {

            int sum1 = 0;
            int sum2 = 0;
            //double result = 0;
            //DataTable dtDaily = new DataTable();
            //dtDaily.Columns.Add("region");
            //dtDaily.Columns.Add("title");
            //dtDaily.Columns.Add("location");
            //dtDaily.Columns.Add("atm_id");
            //DataTable dtWeekly = dtDaily.Clone();
            //DataTable dtMonthly = dtDaily.Clone();

            Hashtable ht = null;
            List<DateTime> replenishmentDates = null;
            //List<string> dailyRegion = new List<string>();
            //Hashtable dailyRegionCount = new Hashtable(3);

            //List<string> weeklyRegion = new List<string>();
            //Hashtable weeklyRegionCount = new Hashtable(3);

            //List<string> monthlyRegion = new List<string>();
            //Hashtable monthlyRegionCount = new Hashtable(3);

            //Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1 and is_active=1");
            Atm atm = Atm.LoadAtmByPk(atmID);
            NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
            int totalType1 = 0;
            int totalType2 = 0;
            int totalType3 = 0;
            int totalType4 = 0;
            int repDays = 0;
            decimal temp = 0;
            //while (atmReader.Read())
            //{
            sum1 = 0;
            sum2 = 0;
            //result = 0;
            repDays = 0;
            ht = new Hashtable();
            replenishmentDates = new List<DateTime>();

            Avanza.CCMS.DAL.Replenishment.ReplenishmentReader repReader = Avanza.CCMS.DAL.Replenishment.ExecuteReader("atm_id=" + atm.ATMId + " order by rep_datetime");
            while (repReader.Read())
            {
                replenishmentDates.Add(new DateTime(repReader.CurrentReplenishment.RepDatetime.Year, repReader.CurrentReplenishment.RepDatetime.Month, repReader.CurrentReplenishment.RepDatetime.Day));
                totalType1 += repReader.CurrentReplenishment.CashAdded1;
                totalType2 += repReader.CurrentReplenishment.CashAdded2;
                totalType3 += repReader.CurrentReplenishment.CashAdded3;
                totalType4 += repReader.CurrentReplenishment.CashAdded4;

            }
            repReader.Close();
            if (replenishmentDates.Count > 0)
            {
                for (int k = 1; k < replenishmentDates.Count; k++)
                {
                    repDays = (replenishmentDates[k] - replenishmentDates[k - 1]).Days;
                    if (ht.Contains(repDays))
                        ht[repDays] = (int)ht[repDays] + 1;
                    else
                        ht.Add(repDays, 1);
                }

                foreach (DictionaryEntry de in ht)
                {
                    sum2 += int.Parse(de.Value.ToString());
                    sum1 += int.Parse(de.Key.ToString()) * int.Parse(de.Value.ToString());
                }

                if (sum2 != 0)
                    temp = Math.Round((decimal)sum1 / sum2);
                //expectedTrips += (int)temp;
                //expectedAmount += (decimal)(totalType1 * noteSetType.DenominationType1 +
                //    totalType2 * noteSetType.DenominationType2 +
                //    totalType3 * noteSetType.DenominationType3 +
                //    totalType4 * noteSetType.DenominationType4);
            }
            return (int)temp;

            //}

            //atmReader.Close();
            //for (int i = 0; i < dailyRegion.Count; i++)
            //{
            //    this.Chart3.Series["Default"].Points.AddXY(dailyRegion[i], dailyRegionCount[dailyRegion[i]]);

            //}
            //this.Chart3.Series["Default"]["ShowMarkerLines"] = "true";
            //this.Chart3.Series["Default"]["DrawingStyle"] = "Cylinder";
            //this.Chart3.Series["Default"]["PointWidth"] = "0.8";

            //for (int i = 0; i < weeklyRegion.Count; i++)
            //{
            //    this.Chart1.Series["Default"].Points.AddXY(weeklyRegion[i], weeklyRegionCount[weeklyRegion[i]]);

            //}
            //this.Chart1.Series["Default"]["ShowMarkerLines"] = "true";
            //this.Chart1.Series["Default"]["DrawingStyle"] = "Cylinder";
            //this.Chart1.Series["Default"]["PointWidth"] = "0.8";

            //for (int i = 0; i < monthlyRegion.Count; i++)
            //{
            //    this.Chart2.Series["Default"].Points.AddXY(monthlyRegion[i], monthlyRegionCount[monthlyRegion[i]]);

            //}
            //this.Chart2.Series["Default"]["ShowMarkerLines"] = "true";
            //this.Chart2.Series["Default"]["DrawingStyle"] = "Cylinder";
            //this.Chart2.Series["Default"]["PointWidth"] = "0.8";

            //GridView1.DataSource = dtDaily;
            //GridView1.DataBind();

            //GridView2.DataSource = dtWeekly;
            //GridView2.DataBind();

            //GridView3.DataSource = dtMonthly;
            //GridView3.DataBind();







        }


        protected void DisplayTrend()
        {
            LogableTask task = null;

            SqlCommand cmd = null;
            SqlCommand cmd1 = null;

            try
            {
                this.Chart7.Series[0].Points.Clear();
                this.Chart8.Series[0].Points.Clear();
                this.Chart8.Series[1].Points.Clear();
                this.Chart8.Series[2].Points.Clear();
                task = LogableTask.NewTask("display Trend");
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd1 = ConnectionFactory.GetNewCommand(true);

                Atm atm = Atm.LoadAtm("title='" + Label1.Text + "'");
                Avanza.CCMS.DAL.Summary.SummaryReader reader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atm.ATMId + "and trxn_datetime>=convert(datetime,'" + TextBox_fromTrendAnalysis.Text + "',103) and  trxn_datetime<=convert(datetime,'" + TextBox_toTrendAnalysis.Text + " 23:59:59',103)  order by trxn_datetime");
                while (reader.Read())
                {
                    this.Chart7.Series["Default"].Points.AddXY(reader.CurrentSummary.TrxnDatetime, reader.CurrentSummary.Withdrawals);

                    Chart7.Series["Default"].Points[Chart7.Series["Default"].Points.Count - 1].PostBackValue = "#VALX," + atm.ATMId + "," + reader.CurrentSummary.Withdrawals;

                    cmd1.CommandText = @" select sum(cash_dispensed1),
sum(cash_dispensed2),sum(cash_dispensed3) from parsed_transaction
where atm_id =" + atm.ATMId + " and  trxn_datetime>=CONVERT(datetime,'" + reader.CurrentSummary.TrxnDatetime.ToString("dd/MM/yyyy") + "',103)" +
" and trxn_datetime<=CONVERT(datetime,'" + reader.CurrentSummary.TrxnDatetime.ToString("dd/MM/yyyy") + " 23:59:59',103)";
                    SqlDataReader reader1 = cmd1.ExecuteReader();
                    while (reader1.Read())
                    {

                        this.Chart8.Series[0].Points.AddXY(reader.CurrentSummary.TrxnDatetime, reader1[0]);
                        this.Chart8.Series[1].Points.AddXY(reader.CurrentSummary.TrxnDatetime, reader1[1]);
                        this.Chart8.Series[2].Points.AddXY(reader.CurrentSummary.TrxnDatetime, reader1[2]);
                    }
                    reader1.Close();
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured while displaying trend. " + ex.Message + "</li>";

            }
            finally
            {
                cmd.Connection.Close();
                cmd1.Connection.Close();
                task.EndTask();

            }

        }


        //protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    LogableTask task = null;
        //    if (e.CommandName == "displayTrend")
        //    {
        //        SqlCommand cmd = null;

        //        try
        //        {
        //            this.Chart6.Series[0].Points.Clear();
        //            task = LogableTask.NewTask("display Trend");
        //            cmd = ConnectionFactory.GetNewCommand(true);

        //            Atm atm = Atm.LoadAtmByPk(int.Parse(e.CommandArgument.ToString()));

        //            Avanza.CCMS.DAL.Summary.SummaryReader reader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atm.ATMId + "and trxn_datetime>=getdate()-90 order by trxn_datetime");
        //            while (reader.Read())
        //            {
        //                this.Chart6.Series["Default"].Points.AddXY(reader.CurrentSummary.TrxnDatetime, reader.CurrentSummary.Withdrawals);
        //            }
        //            reader.Close();

        //        }
        //        catch (Exception ex)
        //        {
        //            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //            Literal_error.Text = "<li>An Error occured while displaying trend. " + ex.Message + "</li>";

        //        }
        //        finally
        //        {
        //            cmd.Connection.Close();
        //            task.EndTask();

        //        }
        //    }
        //}

        //protected void GridView3_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    LogableTask task = null;
        //    if (e.CommandName == "displayTrend")
        //    {
        //        SqlCommand cmd = null;

        //        try
        //        {
        //            this.Chart4.Series[0].Points.Clear();
        //            task = LogableTask.NewTask("display Trend");
        //            cmd = ConnectionFactory.GetNewCommand(true);

        //            Atm atm = Atm.LoadAtmByPk(int.Parse(e.CommandArgument.ToString()));

        //            Avanza.CCMS.DAL.Summary.SummaryReader reader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atm.ATMId + "and trxn_datetime>=getdate()-90  order by trxn_datetime");
        //            while (reader.Read())
        //            {
        //                this.Chart4.Series["Default"].Points.AddXY(reader.CurrentSummary.TrxnDatetime, reader.CurrentSummary.Withdrawals);
        //            }
        //            reader.Close();

        //        }
        //        catch (Exception ex)
        //        {
        //            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //            Literal_error.Text = "<li>An Error occured while displaying trend. " + ex.Message + "</li>";

        //        }
        //        finally
        //        {
        //            cmd.Connection.Close();
        //            task.EndTask();

        //        }
        //    }
        //}

        private int GetMonthIndex(string month)
        {
            List<string> mon = new List<string>();
            DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
            for (int i = 0; i < 12; i++)
            {
                mon.Add(date.ToString("MMM"));
                date = date.AddMonths(1);
            }
            return mon.IndexOf(month);
        }

        protected void GridView_forecasting_rowdatabound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.FindControl("chkbx") != null)
                {
                    decimal amt = decimal.Parse(((DataRowView)e.Row.DataItem).Row["replenishment"].ToString());
                    if (amt == 0)
                        ((CheckBox)e.Row.FindControl("chkbx")).Visible = false;

                    string date = ((DataRowView)e.Row.DataItem).Row["date"].ToString();
                    string[] dateParts = date.Split(',')[1].Split('-');
                    DateTime forecastedDate = new DateTime(int.Parse(dateParts[2]), GetMonthIndex(dateParts[1]) + 1, int.Parse(dateParts[0]));
                    if (forecastedDate < DateTime.Today)
                        ///string currentDate =  DateTime.Now.DayOfWeek + "," + DateTime.Now.Day + "-" + DateTime.Now.ToString("MMM") + "-" + DateTime.Now.Year;

                        //if (date!=currentDate) {
                        e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml(color[r.Next(4)]);
                    //}
                    if (!events.Contains(forecastedDate))
                        ((System.Web.UI.HtmlControls.HtmlImage)e.Row.FindControl("imgEvent")).Visible = false;

                }
            }




        }
        DataTable GenerateForecastDataTableWithLeadtime()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
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
            dt.Columns.Add("atm_id");



            //dt.Columns.Add("date");

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
            decimal standardOrderAmount = 0;
            NoteSetType notesetType = null;
            int windowSize = 3;
            List<TransactionHistory> withdrawalHistory = null;
            List<decimal> forecastedWithdrawals = null;
            //List<decimal> withdrawals = new List<decimal>();
            List<DateTime> events = new List<DateTime>();
            //decimal forecastedWithdrawals = 0;
            TransactionHistory history = null;
            Atm.AtmReader atmReader = Atm.ExecuteReader("is_atm=1 and is_active=1");
            while (atmReader.Read())
            {
                openingBalance = 0;
                forecastedWithdrawals = new List<decimal>();
                withdrawalHistory = new List<TransactionHistory>();
                notesetType = NoteSetType.LoadNoteSetTypeByPk(atmReader.CurrentAtm.NoteSetTypeId);
                standardOrderAmount = (decimal)(notesetType.DenominationType1 * 2000 +
                    notesetType.DenominationType2 * 4000 +
                    notesetType.DenominationType3 * 2000);
                Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atmReader.CurrentAtm.ATMId + " and trxn_datetime >=getdate()-90 order by trxn_datetime");
                //Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atmReader.CurrentAtm.ATMId + " and trxn_datetime >=convert(datetime,'" + DateTime.Now.AddMonths(-1).AddDays(-windowSize).ToString("dd/MM/yyyy") + "',103) and trxn_datetime<=convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy") + "',103) order by trxn_datetime");
                while (summaryReader.Read())
                {
                    history = new TransactionHistory();
                    history.trxnDatetime = summaryReader.CurrentSummary.TrxnDatetime;
                    history.amount = summaryReader.CurrentSummary.Withdrawals;
                    //withdrawals.Add(summaryReader.CurrentSummary.Withdrawals);
                    withdrawalHistory.Add(history);
                }
                summaryReader.Close();

                if (withdrawalHistory.Count == 0)
                    continue;

                DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime to = DateTime.Now.AddDays(double.Parse(TextBox_LeadTime.Text));
                int diff1 = (to - from).Days + 1;


                //int[] dataPoints = { 119, 72, 113, 82, 82, 131, 111, 116, 89, 95, 88, 90 };
                int toLoop = withdrawalHistory.Count - windowSize;
                decimal sum = 0;
                bool added = false;
                int itemAdded = 0;
                DateTime temp = DateTime.MinValue;
                DateTime forecastedDay = DateTime.MinValue;
                DateTime forecastedDateTime = DateTime.MinValue;
                int tempCount = 0;
                for (int j = 0; j < toLoop; j++)
                {

                    tempCount = 0;
                    sum = 0;
                    added = false;
                    itemAdded = 0;
                    for (int k = j; k < (j + windowSize); k++)
                    {
                        if (tempCount == 0)
                        {
                            temp = withdrawalHistory[k].trxnDatetime;
                            forecastedDay = temp.AddDays(windowSize);
                            tempCount++;
                        }
                        if (events.Contains(withdrawalHistory[k].trxnDatetime))
                        {
                            for (int l = 0; l < withdrawalHistory.Count; l++)
                            {
                                if (withdrawalHistory[l].trxnDatetime == forecastedDay)
                                {
                                    sum = withdrawalHistory[l].amount;
                                    forecastedWithdrawals.Add(MakeNumberDivisibleBy50(sum));
                                    added = true;
                                    break;
                                }
                            }


                            //if (temp == withdrawalHistory[k].trxnDatetime) // Event Day
                            //{
                            //    sum = withdrawalHistory[k].amount;
                            //    forecastedWithdrawals.Add(MakeNumberDivisibleBy50(sum));
                            //    added = true;
                            //    break;
                            //}
                            //else
                            //{
                            //    if (forecastedWithdrawals.Count > 0)
                            //    {
                            //        forecastedWithdrawals.Add(forecastedWithdrawals[forecastedWithdrawals.Count - 1]);
                            //        added = true;
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        forecastedWithdrawals.Add(MakeNumberDivisibleBy50(Math.Round(sum / itemAdded)));
                            //        added = true;
                            //        break;
                            //    }
                            //}
                        }
                        else
                        {
                            sum += withdrawalHistory[k].amount;
                            itemAdded++;
                        }

                    }
                    if (!added)
                        forecastedWithdrawals.Add(MakeNumberDivisibleBy50(Math.Round(sum / windowSize)));
                }
                decimal expectedAmount = 0;
                for (int j = 0; j < diff1; j++)
                {
                    forecastedDateTime = from.AddDays(j);
                    expectedAmount = 0;
                    preWithdrawals = 0;
                    amountExpForType1 = 0;
                    amountExpForType2 = 0;
                    amountExpForType3 = 0;
                    amountExpForType4 = 0;

                    returns = 0;
                    //Console.WriteLine("Day " + (j + 1 + windowSize) + " -->" + Math.Round(sum / windowSize));
                    DataRow dr = dt.NewRow();
                    //DateTime forecastedDateTime = DateTime.Now.AddDays(double.Parse(TextBox_LeadTime.Text));
                    //withdrawalHistory[j].trxnDatetime.AddDays(windowSize);
                    dr[1] = forecastedDateTime.DayOfWeek + "," + forecastedDateTime.Day + "-" + forecastedDateTime.ToString("MMM") + "-" + forecastedDateTime.Year;
                    dr[0] = atmReader.CurrentAtm.Title;
                    if (openingBalance == 0)
                        openingBalance = standardOrderAmount;
                    dr[2] = openingBalance;


                    if (openingBalance < forecastedWithdrawals[j])
                    {
                        //   int  upperBound = int.Parse(TextBox12.Text);// 1; //Trips per week.
                        decimal upperBound = Math.Floor(7 / decimal.Parse(TextBox_NoOfTripsPerWeek.Text));
                        if (j + 1 + upperBound < forecastedWithdrawals.Count)
                        {
                            for (int k = 0; k < upperBound; k++)
                            {
                                expectedAmount += forecastedWithdrawals[k + j];
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
                                decimal diff = (amountExpForType1 + amountExpForType2 + amountExpForType3) - rep;

                                amountExpForType1 = amountExpForType1 - diff;
                            }


                        }
                        else
                            rep = standardOrderAmount;
                        decimal notes = openingBalance / 50;


                        returns = Math.Floor(notes / 2) * 50;
                        preWithdrawals = Math.Round(notes / 2) * 50;
                    }
                    else
                        rep = 0;

                    //dr[1] = 
                    dr[3] = rep;

                    dr[4] = preWithdrawals;
                    dr[5] = forecastedWithdrawals[j];
                    dr[6] = returns;
                    if (rep > 0)
                    {
                        balance = rep - (forecastedWithdrawals[j] - preWithdrawals);
                    }
                    else
                        balance = openingBalance - forecastedWithdrawals[j];
                    openingBalance = Math.Round(balance);
                    dr[7] = balance;
                    dr[8] = Math.Round(amountExpForType1 / 50);
                    dr[9] = Math.Round(amountExpForType2 / 100);
                    dr[10] = Math.Round(amountExpForType3 / 500);
                    dr[11] = amountExpForType4;
                    dr[12] = atmReader.CurrentAtm.ATMId;
                    //dr[7] = atmReader.CurrentAtm.Location;
                    //dr[8] = MakeNumberDivisibleBy50(Math.Round((decimal)(decimal.Parse(dr[3].ToString()) * notesetType.DenominationType1 +
                    //    decimal.Parse(dr[4].ToString()) * notesetType.DenominationType2 +
                    //    decimal.Parse(dr[5].ToString()) * notesetType.DenominationType3) / 3));
                    //dr[1] = DateTime.Now.DayOfWeek + "," + DateTime.Now.Day + "-" + DateTime.Now.///ToString("MMM") + "-" + DateTime.Now.Year;


                    if (rep > 0)
                    {
                        if (forecastedDateTime.Date == DateTime.Now.AddDays(double.Parse(TextBox_LeadTime.Text)).Date)
                        {
                            dt.Rows.Add(dr);
                            break;
                        }
                    }



                }
            }
            atmReader.Close();
            //if (dt.Rows.Count > 0)
            //    Button_GenerateOrderWithLeadTime.Visible = true;
            //else
            //    Button_GenerateOrderWithLeadTime.Visible = false;
            return dt;
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
            dt.Columns.Add("atm_id");
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

            int windowSize = 3;
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

            //Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atm.ATMId + " and trxn_datetime >=convert(datetime,'" + DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null).AddDays(-windowSize).ToString("dd/MM/yyyy") + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + "',103) order by trxn_datetime");
            Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atm.ATMId + " and trxn_datetime >=getdate()-90 order by trxn_datetime");
            while (summaryReader.Read())
            {
                history = new TransactionHistory();
                history.trxnDatetime = summaryReader.CurrentSummary.TrxnDatetime;
                history.amount = summaryReader.CurrentSummary.Withdrawals;
                withdrawals.Add(summaryReader.CurrentSummary.Withdrawals);
                withdrawalHistory.Add(history);
            }
            summaryReader.Close();
            if (withdrawalHistory.Count == 0)
                throw new Exception("ATM Withdrawal history is missing.");
            for (int i = 0; i < ListBox_Events.Items.Count; i++)
            {
                events.Add(DateTime.ParseExact(ListBox_Events.Items[i].Text, "dd/MM/yyyy", null));
            }


            //int[] dataPoints = { 119, 72, 113, 82, 82, 131, 111, 116, 89, 95, 88, 90 };
            int toLoop = withdrawals.Count - windowSize;
            decimal sum = 0;
            bool added = false;
            int itemAdded = 0;
            DateTime temp = DateTime.MinValue;
            DateTime forecastedDay = DateTime.MinValue;

            int tempCount = 0;
            for (int j = 0; j < toLoop; j++)
            {
                tempCount = 0;
                sum = 0;
                added = false;
                itemAdded = 0;
                for (int k = j; k < (j + windowSize); k++)
                {
                    if (tempCount == 0)
                    {
                        temp = withdrawalHistory[k].trxnDatetime;
                        forecastedDay = temp.AddDays(windowSize);
                        tempCount++;
                    }
                    if (events.Contains(withdrawalHistory[k].trxnDatetime))
                    {
                        for (int l = 0; l < withdrawalHistory.Count; l++)
                        {
                            if (withdrawalHistory[l].trxnDatetime == forecastedDay)
                            {
                                sum = withdrawalHistory[l].amount;
                                forecastedWithdrawals.Add(MakeNumberDivisibleBy50(sum));
                                added = true;
                                break;
                            }
                        }


                        //if (temp == withdrawalHistory[k].trxnDatetime) // Event Day
                        //{
                        //    sum = withdrawalHistory[k].amount;
                        //    forecastedWithdrawals.Add(MakeNumberDivisibleBy50(sum));
                        //    added = true;
                        //    break;
                        //}
                        //else
                        //{
                        //    if (forecastedWithdrawals.Count > 0)
                        //    {
                        //        forecastedWithdrawals.Add(forecastedWithdrawals[forecastedWithdrawals.Count - 1]);
                        //        added = true;
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        forecastedWithdrawals.Add(MakeNumberDivisibleBy50(Math.Round(sum / itemAdded)));
                        //        added = true;
                        //        break;
                        //    }
                        //}
                    }
                    else
                    {
                        sum += withdrawalHistory[k].amount;
                        itemAdded++;
                    }

                }
                if (!added)
                    forecastedWithdrawals.Add(MakeNumberDivisibleBy50(Math.Round(sum / windowSize)));
            }
            decimal expectedAmount = 0;

            DateTime from = DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null); // withdrawalHistory[j].trxnDatetime.AddDays(windowSize);//.AddYears(1);
            DateTime to = DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null); // withdrawalHistory[j].trxnDatetime.AddDays(windowSize);//.AddYears(1);

            int diff1 = (to - from).Days + 1;

            //for (int j = 0; j < forecastedWithdrawals.Count; j++)
            for (int j = 0; j < diff1; j++)
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
                DateTime forecastedDateTime = from.AddDays(j); // withdrawalHistory[j].trxnDatetime.AddDays(windowSize);//.AddYears(1);
                dr[0] = forecastedDateTime.DayOfWeek + "," + forecastedDateTime.Day + "-" + forecastedDateTime.ToString("MMM") + "-" + forecastedDateTime.Year;

                if (openingBalance == 0)
                    openingBalance = standardOrderAmount;
                dr[1] = openingBalance;

                if (openingBalance < forecastedWithdrawals[j])
                {
                    decimal upperBound = Math.Floor(7 / decimal.Parse(TextBox_NoOfTripsPerWeek.Text));
                    if (j + 1 + upperBound < forecastedWithdrawals.Count)
                    {
                        for (int k = 0; k < upperBound; k++)
                        {
                            expectedAmount += forecastedWithdrawals[k + j];
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
                            decimal diff = (amountExpForType1 + amountExpForType2 + amountExpForType3) - rep;

                            amountExpForType1 = amountExpForType1 - diff;
                        }


                    }
                    else
                        rep = standardOrderAmount;
                    decimal notes = openingBalance / 50;


                    returns = Math.Floor(notes / 2) * 50;
                    preWithdrawals = Math.Round(notes / 2) * 50;
                }
                else
                    rep = 0;

                dr[2] = rep;
                dr[3] = preWithdrawals;
                dr[4] = forecastedWithdrawals[j];
                dr[5] = returns;
                if (rep > 0)
                {
                    balance = rep - (forecastedWithdrawals[j] - preWithdrawals);
                }
                else
                    balance = openingBalance - forecastedWithdrawals[j];

                openingBalance = Math.Round(balance);
                dr[6] = openingBalance;
                dr[7] = Math.Round(amountExpForType1 / 50);
                dr[8] = Math.Round(amountExpForType2 / 100);
                dr[9] = Math.Round(amountExpForType3 / 500);
                dr[10] = amountExpForType4;
                dr[11] = atm.ATMId;
                dt.Rows.Add(dr);

            }
            if (dt.Rows.Count > 0)
                Button_GenerateOrder.Visible = true;
            return dt;
        }
        protected void Button_ForecastOrderWithLeadTime_Click(object sender, EventArgs e)
        {
            GridView4.DataSource = GenerateForecastDataTableWithLeadtime();
            GridView4.DataBind();
        }
        protected void RadioButton_AnalysisOpt_Click(object sender, EventArgs e)
        {
            if (RadioButton_AnalysisOpt.SelectedIndex == 0)
                fileUploadCtrlRow.Visible = true;
            else
                fileUploadCtrlRow.Visible = false;
        }

        protected void Button_Next_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            Panel_DisplayHistoryOfMissingATMs.Visible = true;
            Panel_ATMZone.Visible = false;
            Panel_RepGroup.Visible = false;
            AnalyzeATMsHistory(DateTime.Today.AddMonths(-13));
        }
        protected void Button_Next1_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            Panel_DisplayHistoryOfMissingATMs.Visible = false;
            Panel_ATMZone.Visible = true;
            Panel_RepGroup.Visible = false;

            GroupATMsIntoZone(DateTime.Today.AddMonths(-13));
        }

        protected void Button_Next2_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            Panel_DisplayHistoryOfMissingATMs.Visible = false;
            Panel_ATMZone.Visible = false;
            Panel_RepGroup.Visible = true;
            BindReplenishmentGrids();
            //GroupATMsIntoZone(DateTime.Today.AddMonths(-13));
        }


        protected void Button_Add_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text.Length > 0)
            {
                string[] dateParts = TextBox1.Text.Split('/');
                DateTime temp = new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]));

                if (!events.Contains(temp))
                {
                    events.Add(temp);
                    ListBox_Events.Items.Add(temp.ToString());
                }
            }
        }

        protected void Button_Remove_Click(object sender, EventArgs e)
        {
            if (ListBox_Events.SelectedIndex > -1)
            {
                string[] dateParts = ListBox_Events.Items[ListBox_Events.SelectedIndex].Text.Split('/');
                DateTime temp = new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]));
                events.Remove(temp);
                ListBox_Events.Items.RemoveAt(ListBox_Events.SelectedIndex);
            }

        }
        protected void Chart7_Click(object sender, ImageMapEventArgs e)
        {
            string postBackValue = e.PostBackValue;
            string[] parts = postBackValue.Split(',');
            Label_Date.Text = parts[0];
            Label_Amount.Text = parts[2];
            DateTime from = new DateTime(int.Parse(Label_Date.Text.Substring(6)), int.Parse(Label_Date.Text.Substring(3, 2)), 1);
            DateTime to = from.AddMonths(1).AddDays(-1);


            this.Chart10.Series["Default"].Points.Clear();
            this.Chart6.Series["Default"].Points.Clear();
            this.Chart9.Series["Default"].Points.Clear();


            Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + parts[1] + " and trxn_datetime >=convert(datetime,'" + from.ToString("dd/MM/yyyy") + "',103) and trxn_datetime <=convert(datetime,'" + to.ToString("dd/MM/yyyy") + " 23:59:59',103)  order by trxn_datetime");

            while (summaryReader.Read())
            {
                this.Chart10.Series["Default"].Points.AddXY(summaryReader.CurrentSummary.TrxnDatetime, summaryReader.CurrentSummary.Withdrawals);
            }
            summaryReader.Close();

            summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + parts[1] + " and trxn_datetime >=convert(datetime,'" + from.AddDays(-7).ToString("dd/MM/yyyy") + "',103) and trxn_datetime <=convert(datetime,'" + from.AddDays(-1).ToString("dd/MM/yyyy") + " 23:59:59',103)  order by trxn_datetime");
            while (summaryReader.Read())
            {
                this.Chart6.Series["Default"].Points.AddXY(summaryReader.CurrentSummary.TrxnDatetime, summaryReader.CurrentSummary.Withdrawals);
            }
            summaryReader.Close();

            summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + parts[1] + " and trxn_datetime >=convert(datetime,'" + to.AddDays(1).ToString("dd/MM/yyyy") + "',103) and trxn_datetime <=convert(datetime,'" + to.AddDays(7).ToString("dd/MM/yyyy") + " 23:59:59',103)  order by trxn_datetime");
            while (summaryReader.Read())
            {
                this.Chart9.Series["Default"].Points.AddXY(summaryReader.CurrentSummary.TrxnDatetime, summaryReader.CurrentSummary.Withdrawals);
            }
            this.Chart6.Series["Default"]["DrawingStyle"] = "Cylinder";
            this.Chart6.Series["Default"]["PointWidth"] = "0.8";
            this.Chart9.Series["Default"]["DrawingStyle"] = "Cylinder";
            this.Chart9.Series["Default"]["PointWidth"] = "0.8";
            summaryReader.Close();





        }
        protected void DialogCallBack_Callback(object sender, CallBackEventArgs e)
        {
            LogableTask task = null;
            try
            {
                DateTime tempDateTime;
                task = LogableTask.NewTask("DialogCallBack_Callback");
                StringBuilder builder = new StringBuilder();

                List<DateTime> listATMsHistory = new List<DateTime>();
                string[] paramParts = e.Parameter.Split(',');

                Atm atm = Atm.LoadAtmByPk(int.Parse(paramParts[0]));
                builder.Append("<table border=1 width=100%><tr><td width=20%><b>Title</b></td><td><b>" + atm.Title + "</b></td></tr><tr><td><b>Location</b></td><td><b>" + atm.Location + "</b></td></tr></table><br/>");


                if (paramParts[1] == "0")
                {
                    builder.Append("<b>The selected ATM has history missing for the following days</b><br/><br/>");
                    builder.Append("<table border=1 width=40%>");
                    builder.Append("<tr><td width=2%><b>S.No.</b></td><td width=20%><b>Missing History Date</b></td></tr>");
                    Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atm.ATMId);
                    while (summaryReader.Read())
                        listATMsHistory.Add(summaryReader.CurrentSummary.TrxnDatetime);
                    summaryReader.Close();
                    DateTime historyStartDateTime = DateTime.Today.AddMonths(-13);
                    DateTime to = historyStartDateTime.AddMonths(13);
                    int count = (to - historyStartDateTime).Days;
                    int printCounter = 1;
                    for (int j = 0; j < count; j++)
                    {
                        tempDateTime = historyStartDateTime.AddDays(j);
                        if (!listATMsHistory.Contains(tempDateTime))
                        {
                            builder.Append("<tr><td width=2%>" + printCounter + "</td><td width=20%>" + tempDateTime.ToString("dd/MM/yyyy") + "</td></tr>");
                            printCounter++;
                        }
                    }


                }
                else if (paramParts[1] == "1")
                {
                    builder.Append("<b>Transactional History Of Selected ATM</b><br/><br/>");
                    builder.Append("<table border=1 width=45%>");
                    builder.Append("<tr><td width=2%><b>S.No.</b></td><td width=20%><b>Transaction Date</b></td><td><b>Withdrawals</b></td></tr>");
                    Avanza.CCMS.DAL.Summary.SummaryReader summaryReader = Avanza.CCMS.DAL.Summary.ExecuteReader("atm_id=" + atm.ATMId);
                    int printCounter = 1;
                    while (summaryReader.Read())
                    {
                        builder.Append("<tr><td width=2%>" + printCounter + "</td><td width=30%>" + summaryReader.CurrentSummary.TrxnDatetime.ToString("dd/MM/yyyy") + "</td><td>"
                            + summaryReader.CurrentSummary.Withdrawals + "</td></tr>");
                        //listATMsHistory.Add(summaryReader.CurrentSummary.TrxnDatetime);
                        printCounter++;
                    }
                    summaryReader.Close();
                }
                else if (paramParts[1] == "2")
                {
                    builder.Append("<b>Replenishment History Of Selected ATM</b><br/><br/>");
                    builder.Append("<table border=1 width=65%>");
                    builder.Append("<tr><td width=2%><b>S.No.</b></td><td width=20%><b>Replenished Date</b></td><td><b>Type 1</b><td><b>Type 2</b></td><td><b>Type 3</b></td><td><b>Type 4</b></td><td><b>Replenished Amount</b></td></tr>");

                    NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                    int printCounter = 1;
                    Avanza.CCMS.DAL.Replenishment.ReplenishmentReader repReader = Avanza.CCMS.DAL.Replenishment.ExecuteReader("atm_id=" + atm.ATMId + " order by rep_datetime");
                    decimal amount = 0;
                    while (repReader.Read())
                    {
                        amount = (decimal)(repReader.CurrentReplenishment.CashAdded1 * noteSetType.DenominationType1 +
                        repReader.CurrentReplenishment.CashAdded2 * noteSetType.DenominationType2 +
                        repReader.CurrentReplenishment.CashAdded3 * noteSetType.DenominationType3 +
                        repReader.CurrentReplenishment.CashAdded4 * noteSetType.DenominationType4);
                        builder.Append("<tr><td width=2%>" + printCounter + "</td><td width=30%>" + repReader.CurrentReplenishment.RepDatetime.ToString("dd/MM/yyyy") + "</td><td>"
                            + repReader.CurrentReplenishment.CashAdded1 + "</td><td>" + repReader.CurrentReplenishment.CashAdded2 + "</td><td>" + repReader.CurrentReplenishment.CashAdded3 + "</td><td>" + repReader.CurrentReplenishment.CashAdded4 + "</td><td>" + amount + "</td></tr>");
                        //listATMsHistory.Add(summaryReader.CurrentSummary.TrxnDatetime);
                        printCounter++;
                    }
                    repReader.Close();






                }
                builder.Append("</table>");
                e.Output.Write("<script> receiveCallBackResult('" + builder.ToString() + "');</script>");
            }
            catch (Exception ex)
            {

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }



        protected void GridView_RedZone_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_GreenZoneFromRedZone") != null)
                ((ImageButton)e.Row.FindControl("Button_GreenZoneFromRedZone")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to Move " + ((DataRowView)e.Row.DataItem).Row["Title"] + " to Green Zone ?','CCMS')");

            if (e.Row.FindControl("Button_OrangeZoneFromRedZone") != null)
                ((ImageButton)e.Row.FindControl("Button_OrangeZoneFromRedZone")).Attributes.Add("onClick",
                   "return confirm('Are you sure you want to Move " + ((DataRowView)e.Row.DataItem).Row["Title"] + " to Orange Zone ?','CCMS')");


        }
        protected void GridView_OrangeZone_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_GreenZoneFromOrangeZone") != null)
                ((ImageButton)e.Row.FindControl("Button_GreenZoneFromOrangeZone")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to Move " + ((DataRowView)e.Row.DataItem).Row["Title"] + " to Green Zone ?','CCMS')");

            if (e.Row.FindControl("Button_RedZoneFromOrangeZone") != null)
                ((ImageButton)e.Row.FindControl("Button_RedZoneFromOrangeZone")).Attributes.Add("onClick",
                   "return confirm('Are you sure you want to Move " + ((DataRowView)e.Row.DataItem).Row["Title"] + " to Red Zone ?','CCMS')");


        }
        protected void GridView_GreenZone_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_OrangeZoneFromGreenZone") != null)
                ((ImageButton)e.Row.FindControl("Button_OrangeZoneFromGreenZone")).Attributes.Add("onClick",
                   "return confirm('Are you sure you want to Move " + ((DataRowView)e.Row.DataItem).Row["Title"] + " to Orange Zone ?','CCMS')");

            if (e.Row.FindControl("Button_RedZoneFromGreenZone") != null)
                ((ImageButton)e.Row.FindControl("Button_RedZoneFromGreenZone")).Attributes.Add("onClick",
                   "return confirm('Are you sure you want to Move " + ((DataRowView)e.Row.DataItem).Row["Title"] + " to Red Zone ?','CCMS')");


        }

        private void MoveATM(DataTable fromDataTable, DataTable toDataTable, string atmID)
        {
            DataRow[] drATM = fromDataTable.Select("atm_id='" + atmID + "'");

            if (toDataTable == null)
                toDataTable = new DataTable();
            DataRow dr = toDataTable.NewRow();
            dr[0] = drATM[0][0];
            dr[1] = drATM[0][1];
            dr[2] = drATM[0][2];
            toDataTable.Rows.Add(dr);
            fromDataTable.Rows.Remove(drATM[0]);

        }
        protected void GridView_Zone_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string msg = "";
            LogableTask task = LogableTask.NewTask("GridView_Zone_RowCommand");
            try
            {
                DataTable dtOrange = (DataTable)Session["dtOrange"];
                DataTable dtGreen = (DataTable)Session["dtGreen"];
                DataTable dtRed = (DataTable)Session["dtRed"];
                Atm atm = Atm.LoadAtmByPk(int.Parse(e.CommandArgument.ToString()));
                msg = atm.Title;
                if (e.CommandName == "MoveToGreenZoneFromRedZone")
                {
                    MoveATM(dtRed, dtGreen, e.CommandArgument.ToString());
                    msg += " Moved Successfully From Red Zone To Green Zone!";
                }
                else if (e.CommandName == "MoveToOrangeZoneFromRedZone")
                {
                    MoveATM(dtRed, dtOrange, e.CommandArgument.ToString());
                    msg += " Moved Successfully From Red Zone To Orange Zone!";
                }
                else if (e.CommandName == "MoveToGreenZoneFromOrangeZone")
                {
                    MoveATM(dtOrange, dtGreen, e.CommandArgument.ToString());
                    msg += " Moved Successfully From Orange Zone To Green Zone!";
                }
                else if (e.CommandName == "MoveToRedZoneFromOrangeZone")
                {
                    MoveATM(dtOrange, dtRed, e.CommandArgument.ToString());
                    msg += " Moved Successfully From Red Zone To Orange Zone!";
                }
                else if (e.CommandName == "MoveToOrangeZoneFromGreenZone")
                {
                    MoveATM(dtGreen, dtOrange, e.CommandArgument.ToString());
                    msg += " Moved Successfully From Green Zone To Orange Zone!";
                }
                else if (e.CommandName == "MoveToRedZoneFromGreenZone")
                {
                    MoveATM(dtGreen, dtRed, e.CommandArgument.ToString());
                    msg += " Moved Successfully From Green Zone To Red Zone!";
                }
                GridView_Orange.DataSource = dtOrange;
                GridView_Orange.DataBind();
                GridView_RedZone.DataSource = dtRed;
                GridView_RedZone.DataBind();
                GridView_GreenZone.DataSource = dtGreen;
                GridView_GreenZone.DataBind();

                Session["dtOrange"] = dtOrange;
                Session["dtGreen"] = dtGreen;
                Session["dtRed"] = dtRed;

                Session[SessionVars.notice.ToString()] = msg;
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured moving ATM. " + ex.Message + "</li>";

            }
            finally
            {
                task.EndTask();
            }

        }
        protected void GridView5_RowCanceling(object sender, GridViewCancelEditEventArgs e)
        {
            this.GridView5.EditIndex = -1;
            DataTable dtResult = (DataTable)Session["dtResult"];
            GridView5.DataSource = dtResult;
            GridView5.DataBind();
        }
        protected void GridView5_RowEditing(object sender, GridViewEditEventArgs e)
        {
            this.GridView5.EditIndex = e.NewEditIndex;
            DataTable dtResult = (DataTable)Session["dtResult"];
            GridView5.DataSource = dtResult;
            GridView5.DataBind();
        }
        protected void GridView5_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string atm_id = e.Keys[e.RowIndex].ToString();

            // old values for the current row 

            //var oldValues = e.OldValues;

            // new (updated) values for the current row 
            //var newvalues = 

            // Exit edit mode 
            this.GridView5.EditIndex = -1;

            // Update the grid 
            DataTable dtResult = (DataTable)Session["dtResult"];
            DataRow[] dr = dtResult.Select("atm_id='" + atm_id + "'");
            dr[0][3] = e.NewValues[3];


            GridView5.DataSource = dtResult;
            GridView5.DataBind();
            Session["dtResult"] = dtResult;


        }

    }
}