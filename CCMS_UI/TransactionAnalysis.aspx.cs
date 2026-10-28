using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

//using Microsoft.Office.Interop.Excel;
using Avanza.iSuite.UI;
using System.Text;
using System.Web.UI.DataVisualization.Charting;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using ComponentArt.Web.UI;
using System.Drawing;

namespace CCMSUI
{
    public partial class TransactionAnalysis : BasePage
    {
        string[] dateFormats = new string[] { "M/d/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "MM/dd/yyyy HH:mm:ss", "MM/d/yyyy h:mm:ss tt" };

        protected void Chart_ShowGraph_Load(object sender, EventArgs e)
        {
            Chart_ShowGraph.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart_ShowGraph.Height = int.Parse(Session["height"].ToString()) - 100;

        }
        protected void Chart1_Load(object sender, EventArgs e)
        {
            Chart1.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart1.Height = (int.Parse(Session["height"].ToString())) - 500;
        }
        protected void Chart2_Load(object sender, EventArgs e)
        {
            Chart2.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart2.Height = (int.Parse(Session["height"].ToString())) - 500;
        }
        protected void Chart3_Load(object sender, EventArgs e)
        {
            Chart3.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart3.Height = int.Parse(Session["height"].ToString()) - 100;
        }
        protected void Chart4_Load(object sender, EventArgs e)
        {
            Chart4.Width = int.Parse(Session["width"].ToString());
            Chart4.Height = (int.Parse(Session["height"].ToString()) / 3); //-200
        }
        protected void Chart5_Load(object sender, EventArgs e)
        {
            Chart5.Width = int.Parse(Session["width"].ToString());
            Chart5.Height = int.Parse(Session["height"].ToString()) / 3;
        }
        protected void Chart6_Load(object sender, EventArgs e)
        {
            Chart6.Width = int.Parse(Session["width"].ToString());
            Chart6.Height = int.Parse(Session["height"].ToString()) / 3;
        }
        protected void Chart7_Load(object sender, EventArgs e)
        {
            Chart7.Width = int.Parse(Session["width"].ToString()) / 2;
            Chart7.Height = int.Parse(Session["height"].ToString()) / 2;
            divRegionComparison.Style.Add("height", Chart7.Height.ToString());
        }

        //protected void Chart8_Load(object sender, EventArgs e)
        //{
        //    Chart8.Width = int.Parse(Session["width"].ToString()) / 3;
        //    Chart8.Height = int.Parse(Session["height"].ToString());
        //}
        protected void Chart9_Load(object sender, EventArgs e)
        {
            Chart9.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart9.Height = int.Parse(Session["height"].ToString());
        }
        protected void Chart10_Load(object sender, EventArgs e)
        {
            Chart10.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart10.Height = int.Parse(Session["height"].ToString());
        }
        protected void Chart11_Load(object sender, EventArgs e)
        {
            Chart11.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart11.Height = int.Parse(Session["height"].ToString());
        }
        protected void Chart12_Load(object sender, EventArgs e)
        {
            Chart12.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart12.Height = int.Parse(Session["height"].ToString()) - 200;
        }
        protected void Chart13_Load(object sender, EventArgs e)
        {
            Chart13.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart13.Height = int.Parse(Session["height"].ToString()) - 200;
        }
        protected void Chart14_Load(object sender, EventArgs e)
        {
            Chart14.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart14.Height = int.Parse(Session["height"].ToString()) - 200;
        }
        protected void Chart15_Load(object sender, EventArgs e)
        {
            Chart15.Width = int.Parse(Session["width"].ToString());
            Chart15.Height = int.Parse(Session["height"].ToString()) / 2;
        }

        private DataTable GetIncidents()
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select title,location, generated_at , '' outage,alert_type
                                from vCcmsIntegratedAlert inner join atm
                                on vCcmsIntegratedAlert.entity_id = atm.atm_id
                                where generated_at >= CONVERT(datetime,'" + TextBox13.Text + "',103) " +
                                " and generated_at <=CONVERT(datetime,' " + TextBox14.Text + " 23:59:59',103) and atm.atm_id in ("
                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                                " and atm.is_active = 1 and atm.is_ej_enabled = 1 and resolved_at is null";

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }



        private void DrawGoogleGaugeChart()
        {
            decimal success = 0;
            decimal failed = 0;
            decimal suspicious = 0;
            decimal total = 0;
            decimal chartData = 0;
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"
                                select status,count(ej_parsed_transactions_id) from vEjParsedTransactions 
                                inner join atm on vEjParsedTransactions.atm_id = atm.atm_id 
                                inner join region on atm.region_id = region.region_id  " +
                                " where trxn_datetime >= CONVERT(datetime,'" + TextBox13.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox14.Text + " 23:59:59',103) and atm.atm_id in ("
                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                               "  and atm.is_active = 1 and atm.is_ej_enabled=1 " +
                                " group by status " +
                                " order by status";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);


            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0].ToString() == "0")
                    success = decimal.Parse(dr[1].ToString());
                if (dr[0].ToString() == "1")
                    failed = decimal.Parse(dr[1].ToString());
                if (dr[0].ToString() == "2")
                    suspicious = decimal.Parse(dr[1].ToString());

            }
            total = success + failed + suspicious;
            //this.ChartPIE.Series["Default"].ChartType = SeriesChartType.Pie;
            //this.ChartPIE.Series["Default"]["PieLabelStyle"] = "Outside";
            //this.ChartPIE.Titles[0].Text = "Overall Service Analysis";
            //this.ChartPIE.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            //this.ChartPIE.Series[0]["PieDrawingStyle"] = "SoftEdge";
            //this.ChartPIE.Legends[0].Enabled = true;
            if (total > 0)
                chartData = Math.Round((success / total) * 100);


            Literal_Script.Text = @"<script type='text/javascript'>google.load('visualization', '1', { packages: ['gauge'] });
        google.setOnLoadCallback(drawChart);
        function drawChart() {

            var data = google.visualization.arrayToDataTable([['Label', 'Value'], ['KPI', " + chartData + "]]);" +
            " var options = { width:300, height: 120, redFrom: 0, redTo: 50, yellowFrom: 51, yellowTo: 75, minorTicks: 5,greenFrom:76,greenTo:100 }; " +
            " var chart = new google.visualization.Gauge(document.getElementById('chart_div')); " +
            " chart.draw(data, options);}</script>";


            //foreach (DataRow dr in status.Rows)
            //{
            //    seriesName = dr[0].ToString();
            //    Series series = new Series(
            //        seriesName == "0" ? "Successful" :
            //        seriesName == "1" ? "Failed" :
            //        seriesName == "2" ? "Suspicious" : "-1");
            //    series.ChartType = SeriesChartType.StackedBar100;
            //    series.ChartArea = "ChartArea1";

            //    ChartKPISummary.Series.Add(series);



            //}








        }



        protected void GridView_EventDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime transactionDateTime;
                DateTime.TryParseExact(e.Row.Cells[2].Text, dateFormats, null, System.Globalization.DateTimeStyles.None, out transactionDateTime);
                TimeSpan t = DateTime.Now - transactionDateTime;
                e.Row.Cells[3].Text = t.Days + " days " + t.Hours + " hr " + t.Minutes + " min";

            }
        }

        protected void GridView_ATMIncidents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime transactionDateTime;
                DateTime.TryParseExact(e.Row.Cells[4].Text, dateFormats, null, System.Globalization.DateTimeStyles.None, out transactionDateTime);
                TimeSpan t = DateTime.Now - transactionDateTime;
                e.Row.Cells[5].Text = t.Days + " days " + t.Hours + " hr " + t.Minutes + " min";

            }
        }
        protected void GridView_ServiceAvailability_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "1")
                        e.Row.Cells[i].Text = "<img src='images/working.png' >";
                    else
                        e.Row.Cells[i].Text = "<img src='images/notWorking.png' >";
                }
            }
        }




        private void DrawKPIChart(string from, string to)
        {

            ChartKPISummary.Series[0].Points.Clear();
            ChartKPISummary.Series[1].Points.Clear();
            ChartKPISummary.Series[2].Points.Clear();


            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"
                                select status,convert(varchar,trxn_datetime,103) trxn_datetime,count(ej_parsed_transactions_id) from vEjParsedTransactions 
                                inner join atm on vEjParsedTransactions.atm_id = atm.atm_id 
                                inner join region on atm.region_id = region.region_id  " +
                                " where trxn_datetime >= CONVERT(datetime,'" + from + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + to + " 23:59:59',103) and atm.atm_id in ("
                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                               "  and atm.is_active = 1 and atm.is_ej_enabled=1 " +
                                " group by status,convert(varchar,trxn_datetime,103) " +
                                " order by convert(varchar,trxn_datetime,103)";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            DataTable status = dt.DefaultView.ToTable(true, new string[] { "status" });
            //foreach (DataRow dr in status.Rows)
            //{
            //    seriesName = dr[0].ToString();
            //    Series series = new Series(
            //        seriesName == "0" ? "Successful" :
            //        seriesName == "1" ? "Failed" :
            //        seriesName == "2" ? "Suspicious" : "-1");
            //    series.ChartType = SeriesChartType.StackedBar100;
            //    series.ChartArea = "ChartArea1";

            //    ChartKPISummary.Series.Add(series);



            //}
            DataTable trxnDatetime = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
            bool series1DataPointAdded = false;
            bool series2DataPointAdded = false;
            bool series3DataPointAdded = false;
            foreach (DataRow dr in trxnDatetime.Rows)
            {
                series1DataPointAdded = false;
                series2DataPointAdded = false;
                series3DataPointAdded = false;
                DataRow[] drArray = dt.Select("trxn_datetime='" + dr[0] + "'", "status");
                for (int j = 0; j < drArray.Length; j++)
                {
                    if (drArray[j][0].ToString() == "0")
                    {
                        ChartKPISummary.Series[0].Points.AddXY(dr[0], drArray[j][2]);
                        series1DataPointAdded = true;
                    }

                    if (drArray[j][0].ToString() == "1")
                    {
                        ChartKPISummary.Series[1].Points.AddXY(dr[0], drArray[j][2]);
                        series2DataPointAdded = true;
                    }

                    if (drArray[j][0].ToString() == "2")
                    {
                        ChartKPISummary.Series[2].Points.AddXY(dr[0], drArray[j][2]);
                        series3DataPointAdded = true;
                    }
                }

                if (!series1DataPointAdded)
                    ChartKPISummary.Series[0].Points.AddXY(dr[0], 0);
                if (!series2DataPointAdded)
                    ChartKPISummary.Series[1].Points.AddXY(dr[0], 0);
                if (!series3DataPointAdded)
                    ChartKPISummary.Series[2].Points.AddXY(dr[0], 0);



            }

            //for (int j = 0; j < ChartKPISummary.Series.Count; j++)
            //{
            //    ChartKPISummary.Series[j]["StackedGroupName"] = "Group1";
            //    ChartKPISummary.Series[j].IsValueShownAsLabel = true;


            //}

            ChartKPISummary.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            ChartKPISummary.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;
            ChartKPISummary.Legends[0].Enabled = false;
            ChartKPISummary.ResetAutoValues();







        }
        private DataTable GetServiceAvailability()
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("Title");


            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"
                                select title,transaction_type_name,count(ej_parsed_transactions_id) total 
						from atm 
                                left join vEjParsedTransactions  on vEjParsedTransactions.atm_id = atm.atm_id 
                                inner join region on atm.region_id = region.region_id 
                                left join transaction_type on transaction_type.transaction_type_id = vEjParsedTransactions.transaction_type_id " +
                                " where trxn_datetime >= CONVERT(datetime,'" + TextBox13.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox14.Text + " 23:59:59',103) and atm.atm_id in ("
                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                               "  and atm.is_active = 1 and atm.is_ej_enabled=1 group by title,transaction_type_name ";

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            DataTable trxn = dt.DefaultView.ToTable(true, new string[] { "transaction_type_name" });
            //bool columnAdded = false;
            foreach (DataRow dr in trxn.Rows)
            {
                if (dr[0] != DBNull.Value)
                {
                    dtGrid.Columns.Add(dr[0].ToString());
                    //columnAdded = true;
                }
            }
            //if (!columnAdded)
            //{
            //    dtGrid.Columns.Add("BALANCE");
            //    dtGrid.Columns.Add("FAST CASH");
            //    dtGrid.Columns.Add("MINISTMT");
            //    dtGrid.Columns.Add("WITHDRAW");

            //}

            //cmd.CommandText = @"select title from atm where is_active = 1 and is_ej_enabled=1";
            //adapter = new SqlDataAdapter(cmd);
            //DataTable dtATM = new DataTable();
            //adapter.Fill(dtATM);

            DataTable dtATM = dt.DefaultView.ToTable(true, new string[] { "title" });

            int idx = 1;
            foreach (DataRow dr in dtATM.Rows)
            {
                idx = 1;
                DataRow newRow = dtGrid.NewRow();

                DataRow[] drArray = dt.Select("title='" + dr[0] + "'");
                newRow[0] = dr[0];
                for (int i = 1; i < dtGrid.Columns.Count; i++)
                    newRow[i] = 0;

                foreach (DataRow dr1 in drArray)
                {
                    if (dr1[1].ToString().Length > 0)
                    {
                        newRow[idx] = decimal.Parse(dr1["total"].ToString()) > 0 ? 1 : 0;
                        idx++;
                    }
                }
                dtGrid.Rows.Add(newRow);
            }

            return dtGrid;
        }

        private DataTable GetServiceUtilization()
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("Region");


            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select region_name,transaction_type_name,count(ej_parsed_transactions_id) total from vEjParsedTransactions 
                                inner join atm on vEjParsedTransactions.atm_id = atm.atm_id 
                                inner join region on atm.region_id = region.region_id 
                                inner join transaction_type on transaction_type.transaction_type_id = vEjParsedTransactions.transaction_type_id " +
                               " where trxn_datetime >= CONVERT(datetime,'" + TextBox13.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox14.Text + " 23:59:59',103) and atm.atm_id in ("
                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                               "  and atm.is_active = 1 and atm.is_ej_enabled=1 " +
                               " group by region_name,transaction_type_name";

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            DataTable trxn = dt.DefaultView.ToTable(true, new string[] { "transaction_type_name" });
            //bool columnAdded = false;
            foreach (DataRow dr in trxn.Rows)
            {
                if (dr[0] != DBNull.Value)
                {
                    dtGrid.Columns.Add(dr[0].ToString());
                    //      columnAdded = true;
                }
            }
            //if (!columnAdded)
            //{
            //    dtGrid.Columns.Add("BALANCE");
            //    dtGrid.Columns.Add("FAST CASH");
            //    dtGrid.Columns.Add("MINISTMT");
            //    dtGrid.Columns.Add("WITHDRAW");

            //}


            //            cmd.CommandText = @"select region_name from region where region_id in (
            //                                select distinct(region_id) from atm 
            //                                where is_ej_enabled = 1 and is_active=1)";
            //            adapter = new SqlDataAdapter(cmd);
            //            DataTable dtRegion = new DataTable();
            //            adapter.Fill(dtRegion);





            DataTable dtRegion = dt.DefaultView.ToTable(true, new string[] { "region_name" });
            decimal total = 0;
            int idx = 1;
            foreach (DataRow dr in dtRegion.Rows)
            {
                idx = 1;
                DataRow newRow = dtGrid.NewRow();

                DataRow[] drArray = dt.Select("region_name='" + dr[0] + "'");
                newRow[0] = dr[0];
                for (int i = 1; i < dtGrid.Columns.Count; i++)
                    newRow[i] = "0%";
                foreach (DataRow dr1 in drArray)
                {
                    total += decimal.Parse(dr1["total"].ToString());
                }
                foreach (DataRow dr1 in drArray)
                {
                    newRow[idx] = Math.Round((decimal.Parse(dr1["total"].ToString()) / total) * 100, 2) + "%";
                    idx++;
                }
                dtGrid.Rows.Add(newRow);
            }

            return dtGrid;
        }
        private void DrawHourlyAnalysisLineChart(string id)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            //            cmd.CommandText = @"select substring(convert(varchar,generated_at,108),1,2) hr,count(id) from ccms_integrated_alert
            //            inner join atm on ccms_integrated_alert.entity_id = atm.atm_id" +
            //" where generated_at >= CONVERT(datetime,'" + TextBox15.Text + "',103) " +
            //                                " and generated_at <=CONVERT(datetime,' " + TextBox16.Text + " 23:59:59',103) and atm.atm_id in ("
            //                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
            //                               "  and atm.is_active = 1 and atm.is_ej_enabled=1 and alert_type_id = " + alertTypeID + " group by substring(convert(varchar,generated_at,108),1,2)";



            cmd.CommandText = @"select substring(convert(varchar,captured_at,108),1,2) hr,count(*) from captured_event  inner join keylookup
                                on captured_event.captured_keylookup_id = keylookup.keylookup_id
            inner join atm on atm.ATM_id = captured_event.atm_id " +
" where captured_at >= CONVERT(datetime,'" + TextBox15.Text + "',103) " +
                                " and captured_at <=CONVERT(datetime,' " + TextBox16.Text + " 23:59:59',103) and atm.atm_id in ("
                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                               "  and atm.is_active = 1 and atm.is_ej_enabled=1 and captured_keylookup_id = " + id + " group by substring(convert(varchar,captured_at,108),1,2)";



            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            for (int k = 0; k < 24; k++)
            {
                DataRow[] drArray = dt.Select("hr='" + k.ToString().PadLeft(2, '0') + "'");
                if (drArray.Length > 0)
                    ChartAlertCountsByHour.Series[0].Points.AddXY(k, drArray[0][1]);

            }

        }


        protected void GridView_EventsSummary_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "viewDetails")
            {
                SqlCommand cmd = null;
                try
                {
                    task = LogableTask.NewTask("viewDetails");

                    cmd = ConnectionFactory.GetNewCommand(false);
                    //                    cmd.CommandText = @"
                    //select title,alert_type_name,generated_at,'' outage 
                    //from ccms_integrated_alert inner join atm on ccms_integrated_alert.entity_id = atm.atm_id
                    // inner join alerT_type
                    //                    on ccms_integrated_alert.alert_type_id = alert_type.alert_type_id " +
                    //" where generated_at >= CONVERT(datetime,'" + TextBox15.Text + "',103) " +
                    //                                " and generated_at <=CONVERT(datetime,' " + TextBox16.Text + " 23:59:59',103) and atm.atm_id in ("
                    //                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                    //                               "  and atm.is_active = 1 and atm.is_ej_enabled=1 " +
                    //                    "  and alert_type.alert_type_id= " + e.CommandArgument.ToString();

                    cmd.CommandText = @"
select title,key_name,captured_at generated_at,'' outage 
from captured_event inner join keylookup on captured_event.captured_keylookup_id = keylookup.keylookup_id
inner join atm on atm.ATM_id = captured_event.atm_id " +
" where captured_at >= CONVERT(datetime,'" + TextBox15.Text + "',103) " +
                       " and captured_at <=CONVERT(datetime,' " + TextBox16.Text + " 23:59:59',103) and atm.atm_id in ("
                       + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                      "  and atm.is_active = 1 and atm.is_ej_enabled=1 " +
           "  and captured_keylookup_id = " + e.CommandArgument.ToString();


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);


                    GridView_EventDetails.DataSource = dt;
                    GridView_EventDetails.DataBind();

                    DrawHourlyAnalysisLineChart(e.CommandArgument.ToString());

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
        }
        private void GetTransactionAnalysis()
        {
            LogableTask task = LogableTask.NewTask("GetTransactionAnalysis");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetTrasactionsSummary";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox11.Text, "dd/MM/yyyy", null);
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox12.Text, "dd/MM/yyyy", null);
                cmd.Parameters.Add(new SqlParameter("AtmId", SqlDbType.VarChar));
                cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                this.GridView_TrxnStatus.DataSource = dt;
                this.GridView_TrxnStatus.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_Message.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }
        private void GetEventsSummary()
        {

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            //            cmd.CommandText = @"select ccms_integrated_alert.alert_type_id,alert_type_name,COUNT(*) total from ccms_integrated_alert inner join alerT_type
            //on ccms_integrated_alert.alert_type_id = alerT_type.alert_type_id inner join atm on ccms_integrated_alert.entity_id = atm.atm_id " +
            //" where generated_at >= CONVERT(datetime,'" + TextBox15.Text + "',103) " +
            //                                " and generated_at <=CONVERT(datetime,' " + TextBox16.Text + " 23:59:59',103) and atm.atm_id in ("
            //                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
            //                               "  and atm.is_active = 1 and atm.is_ej_enabled=1 " +
            //" group by ccms_integrated_alert.alert_type_id,alert_type_name";

            cmd.CommandText = @"select key_name, keylookup_id,count(*)  total
from captured_event inner join keylookup
on captured_event.captured_keylookup_id = keylookup.keylookup_id
inner join atm on atm.ATM_id = captured_event.atm_id " +
" where captured_at >= CONVERT(datetime,'" + TextBox15.Text + "',103) " +
                              " and captured_at <=CONVERT(datetime,' " + TextBox16.Text + " 23:59:59',103) and atm.atm_id in ("
                              + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                             "  and atm.is_active = 1 and atm.is_ej_enabled=1 " +
" group by key_name,keylookup_id";



            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            GridView_EventsSummary.DataSource = dt;
            GridView_EventsSummary.DataBind();




        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["isRunningForEj"] == null)
                    Session["isRunningForEj"] = Request.QueryString["ej"];

                ViewState["SortExpression"] = "title ";
                ViewState["sortDirection"] = "asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
                TextBox16.Text = TextBox14.Text = TextBox12.Text = TextBox_To.Text = TextBox2.Text = TextBox4.Text = TextBox6.Text = TextBox8.Text = TextBox10.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox15.Text = TextBox13.Text = TextBox11.Text = TextBox_From.Text = TextBox1.Text = TextBox3.Text = TextBox5.Text = TextBox7.Text = TextBox9.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                if (Session["isRunningForEj"] == null)
                {
                    TabStripTab3.Visible = TabStripTab4.Visible = TabStripTab2.Visible = false;
                    PageView7.Visible = false;
                    PageView9.Visible = false;
                    PageView8.Visible = false;
                    PageView1.Focus();

                }
                //ada
                if (Session[SessionVars.isRegionSelected.ToString()] != null)
                {
                    int regionID = int.Parse(Session[SessionVars.SelectedRegion.ToString()].ToString());
                    //if (regionID == 1) throw new Exception("Organization not selected.");
                    //Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(regionID);
                    //if (!region.IsOrganization)
                    //    throw new Exception("Organization not selected.");
                    //else
                    //{
                    //    var regionName = new List<string>();
                    Avanza.CCMS.DAL.Region.RegionReader reader = Avanza.CCMS.DAL.Region.ExecuteReader("parent_region_id = " + regionID);
                    while (reader.Read())
                    {
                        ListItem li = new ListItem(reader.CurrentRegion.RegionName.ToString(), reader.CurrentRegion.RegionId.ToString());
                        Checkbox_region.Items.Add(li);

                    }
                    reader.Close();

                    //                        }
                }
            }
        }
        private void GenerateGraph(string id)
        {
            LogableTask task = LogableTask.NewTask("DrawGraph");
            Response.CacheControl = "no-cache";
            SqlCommand cmd = null;
            string isEjTrxnSelected = Request.QueryString["ej"] == null ? "0" : "1";
            try
            {
                if (id == "Button1")
                {
                    task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "Graph Id = " + Request.QueryString["gid"]);

                    //Chart_ShowGraph.Url = "Data.aspx?param=1@" + DateTime.Today.ToString("dd/MM/yyyy") + "@" + DateTime.Today.ToString("dd/MM/yyyy") + "@2@167AD4@" + isEjTrxnSelected;
                    Chart_ShowGraph.ChartAreas[0].AxisX.Title = "Hour";
                    Chart_ShowGraph.ChartAreas[0].AxisY.Title = "No of Transactions";
                    Chart_ShowGraph.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart_ShowGraph.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJTransactionVersusHour" : "TransactionVersusHour";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    DataTable ChartData = dtTrxn.Tables[0];
                    int[] XPointMember = new int[ChartData.Rows.Count];
                    int[] YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["hr"]);
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart_ShowGraph.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                    //Setting width of line  
                    //Chart3.Series[0].BorderWidth = 1;
                    //setting Chart type   
                    //Chart_ShowGraph.Series[0]["PieLabelStyle"] = "Outside";
                    //Chart_ShowGraph.Series[0].BorderWidth = 1;
                    //Chart_ShowGraph.Series[0].BorderDashStyle = ChartDashStyle.Solid;
                    //Chart_ShowGraph.Series[0].BorderColor = System.Drawing.Color.FromArgb(200, 26, 59, 105);
                    //this.Chart_ShowGraph.Titles[0].Text = "Overall Denomination Analysis";
                    this.Chart_ShowGraph.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart_ShowGraph.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart_ShowGraph.Series[0]["PointWidth"] = "0.8";
                    Chart_ShowGraph.Series[0].ChartType = SeriesChartType.Column;
                    //foreach (Series charts in Chart_ShowGraph.Series)
                    //{
                    //    foreach (DataPoint point in charts.Points)
                    //    {
                    //        switch (point.AxisLabel)
                    //        {
                    //            case "1": point.Color = Color.RoyalBlue; break;
                    //            case "5": point.Color = Color.SaddleBrown; break;
                    //            case "10": point.Color = Color.SpringGreen; break;
                    //        }
                    //        point.Label = string.Format("{0:0} - {1} Hr", point.YValues[0], point.XValue);

                    //    }
                    //}


                    //Chart1.Url = "Data.aspx?param=1@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@167AD4@" + isEjTrxnSelected;
                    //Chart2.Url = "Data.aspx?param=1@" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "@0@2A9c0A@" + isEjTrxnSelected;
                    //Chart3.Url = "Data.aspx?param=1@" + TextBox_From.Text + "@" + TextBox_To.Text + "@1@2A9c0A@" + isEjTrxnSelected;
                    Chart1.ChartAreas[0].AxisX.Title = "Hour";
                    Chart1.ChartAreas[0].AxisY.Title = "Number Of Transactions";
                    Chart1.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart1.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);


                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJTransactionVersusHour" : "TransactionVersusHour";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();
                    adapter = new SqlDataAdapter(cmd);
                    dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    ChartData = dtTrxn.Tables[0];
                    XPointMember = new int[ChartData.Rows.Count];
                    YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["hr"]);
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart1.Series[0].Points.DataBindXY(XPointMember, YPointMember);
                    Chart1.ChartAreas[0].AxisX.Interval = 1;
                    //Setting width of line  
                    //Chart3.Series[0].BorderWidth = 1;
                    //setting Chart type   
                    this.Chart1.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart1.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart1.Series[0]["PointWidth"] = "0.8";
                    Chart1.Series[0].ChartType = SeriesChartType.Column;



                    Chart2.ChartAreas[0].AxisX.Title = "Hour";
                    Chart2.ChartAreas[0].AxisY.Title = "Number Of Transactions";
                    Chart2.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart2.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJTransactionVersusHour" : "TransactionVersusHour";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();
                    adapter = new SqlDataAdapter(cmd);
                    dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    ChartData = dtTrxn.Tables[0];
                    XPointMember = new int[ChartData.Rows.Count];
                    YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["hr"]);
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart2.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                    //Setting width of line  
                    //Chart3.Series[0].BorderWidth = 1;
                    //setting Chart type   
                    this.Chart2.Series[0]["ShowMarkerLines"] = "true";
                    // this.Chart2.Series[0]["DrawingStyle"] = "Cylinder";
                    // this.Chart2.Series[0]["PointWidth"] = "0.8";
                    Chart2.Series[0].ChartType = SeriesChartType.Column;


                    Chart3.ChartAreas[0].AxisX.Title = "Hour";
                    Chart3.ChartAreas[0].AxisY.Title = "Number Of Transactions";
                    Chart3.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart3.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJTransactionVersusHour" : "TransactionVersusHour";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(TextBox_From.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(TextBox_To.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();
                    adapter = new SqlDataAdapter(cmd);
                    dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    ChartData = dtTrxn.Tables[0];
                    XPointMember = new int[ChartData.Rows.Count];
                    YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["hr"]);
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart3.Series[0].Points.DataBindXY(XPointMember, YPointMember);
                    Chart3.ChartAreas[0].AxisX.Interval = 1;
                    //Setting width of line  
                    //Chart3.Series[0].BorderWidth = 1;
                    //setting Chart type   
                    this.Chart3.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart3.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart3.Series[0]["PointWidth"] = "0.8";
                    Chart3.Series[0].ChartType = SeriesChartType.Column;





                    Literal1.Text = "<b>Today [" + DateTime.Today.ToString("dd/MM/yyyy") + "]</b><br/>ATM transactions for today";
                    Literal2.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>ATM transactions for last week";
                    Literal3.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>ATM transactions for previous week"; ;
                    Literal4.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox_To.Text + "]</b><br/>ATM transactions for specified duration";

                }
                else if (id == "Button2")
                {
                    //Chart4.Url = "Data.aspx?param=20@" + TextBox1.Text + "@" + TextBox2.Text + "@0@167AD4@500@" + Int32.MaxValue + "@" + isEjTrxnSelected;

                    Chart4.ChartAreas[0].AxisX.Title = "ATM";
                    Chart4.ChartAreas[0].AxisY.Title = "Transactions";
                    Chart4.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart4.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJATMTransactions" : "ATMTransactions";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(TextBox1.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(TextBox2.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();

                    cmd.Parameters.Add("@LowerBound", SqlDbType.Int);
                    cmd.Parameters[3].Value = 500;

                    cmd.Parameters.Add("@UpperBound", SqlDbType.Int);
                    cmd.Parameters[4].Value = Int32.MaxValue;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    DataTable ChartData = dtTrxn.Tables[0];
                    string[] XPointMember = new string[ChartData.Rows.Count];
                    int[] YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = (ChartData.Rows[count]["Title"]).ToString();
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart4.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                    //Setting width of line  
                    //Chart3.Series[0].BorderWidth = 1;
                    //setting Chart type   
                    //this.Chart4.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart4.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart4.Series[0]["PointWidth"] = "0.8";
                    Chart4.Series[0].ChartType = SeriesChartType.Column;

                    //Chart5.Url = "Data.aspx?param=20@" + TextBox1.Text + "@" + TextBox2.Text + "@0@2A9c0A@201@500@" + isEjTrxnSelected;
                    Chart5.ChartAreas[0].AxisX.Title = "ATM";
                    Chart5.ChartAreas[0].AxisY.Title = "Transactions";
                    Chart5.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart5.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJATMTransactions" : "ATMTransactions";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(TextBox1.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(TextBox2.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();

                    cmd.Parameters.Add("@LowerBound", SqlDbType.Int);
                    cmd.Parameters[3].Value = 201;

                    cmd.Parameters.Add("@UpperBound", SqlDbType.Int);
                    cmd.Parameters[4].Value = 500;

                    adapter = new SqlDataAdapter(cmd);
                    dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    ChartData = dtTrxn.Tables[0];
                    XPointMember = new string[ChartData.Rows.Count];
                    YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = (ChartData.Rows[count]["Title"]).ToString();
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart5.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                    //Setting width of line  
                    //Chart3.Series[0].BorderWidth = 1;
                    //setting Chart type
                    //this.Chart5.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart5.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart5.Series[0]["PointWidth"] = "0.8";
                    Chart5.Series[0].ChartType = SeriesChartType.Column;

                    //Chart6.Url = "Data.aspx?param=20@" + TextBox1.Text + "@" + TextBox2.Text + "@0@F59C0A@0@199@" + isEjTrxnSelected;

                    Chart6.ChartAreas[0].AxisX.Title = "ATM";
                    Chart6.ChartAreas[0].AxisY.Title = "Transactions";
                    Chart6.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart6.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJATMTransactions" : "ATMTransactions";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(TextBox1.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(TextBox2.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();

                    cmd.Parameters.Add("@LowerBound", SqlDbType.Int);
                    cmd.Parameters[3].Value = 0;

                    cmd.Parameters.Add("@UpperBound", SqlDbType.Int);
                    cmd.Parameters[4].Value = 200;

                    adapter = new SqlDataAdapter(cmd);
                    dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    ChartData = dtTrxn.Tables[0];
                    XPointMember = new string[ChartData.Rows.Count];
                    YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = (ChartData.Rows[count]["Title"]).ToString();
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart6.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                    //Setting width of line  
                    //Chart3.Series[0].BorderWidth = 1;
                    //setting Chart type   
                    //this.Chart6.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart6.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart6.Series[0]["PointWidth"] = "0.8";
                    Chart6.Series[0].ChartType = SeriesChartType.Column;

                    Literal9.Text = "<b>Selected Criteria [" + TextBox1.Text + "-" + TextBox2.Text + "]</b><br/>ATM transactions more than 500 for specified duration";
                    Literal10.Text = "<b>Selected Criteria [" + TextBox1.Text + "-" + TextBox2.Text + "]</b><br/>ATM transactions more than 201 and less than 500 for specified duration";
                    Literal11.Text = "<b>Selected Criteria [" + TextBox1.Text + "-" + TextBox2.Text + "]</b><br/>ATM transactions less than 200 for specified duration";
                }
                else if (id == "Button4")
                {
                    //                    Chart8.Url = "Data.aspx?param=26@" + DateTime.Today.ToString("dd/MM/yyyy") + "@" + DateTime.Today.ToString("dd/MM/yyyy") + "@2@167AD4";
                    //Chart9.Url = "Data.aspx?param=26@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@167AD4@" + isEjTrxnSelected;
                    Chart9.ChartAreas[0].AxisX.Title = "Day";
                    Chart9.ChartAreas[0].AxisY.Title = "Number Of Transactions";
                    Chart9.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart9.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJWeeklyATMTransactions" : "WeeklyATMTransactions";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    DataTable ChartData = dtTrxn.Tables[0];
                    string[] XPointMember = new string[ChartData.Rows.Count];
                    int[] YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = ChartData.Rows[count]["day"].ToString();
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart9.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                    this.Chart9.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart9.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart9.Series[0]["PointWidth"] = "0.8";
                    Chart9.Series[0].ChartType = SeriesChartType.Column;
                    //Chart10.Url = "Data.aspx?param=26@" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "@0@2A9c0A@" + isEjTrxnSelected;
                    Chart10.ChartAreas[0].AxisX.Title = "Day";
                    Chart10.ChartAreas[0].AxisY.Title = "Number Of Transactions";
                    Chart10.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart10.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJWeeklyATMTransactions" : "WeeklyATMTransactions";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();

                    adapter = new SqlDataAdapter(cmd);
                    dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    ChartData = dtTrxn.Tables[0];
                    XPointMember = new string[ChartData.Rows.Count];
                    YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = ChartData.Rows[count]["day"].ToString();
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart10.Series[0].Points.DataBindXY(XPointMember, YPointMember);


                    this.Chart10.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart10.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart10.Series[0]["PointWidth"] = "0.8";
                    Chart10.Series[0].ChartType = SeriesChartType.Column;
                    //Chart11.Url = "Data.aspx?param=26@" + TextBox5.Text + "@" + TextBox6.Text + "@1@2A9c0A@" + isEjTrxnSelected;
                    Chart11.ChartAreas[0].AxisX.Title = "Day";
                    Chart11.ChartAreas[0].AxisY.Title = "Number Of Transactions";
                    Chart11.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart11.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = isEjTrxnSelected == "1" ? "EJWeeklyATMTransactions" : "WeeklyATMTransactions";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(TextBox5.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(TextBox6.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();

                    adapter = new SqlDataAdapter(cmd);
                    dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    ChartData = dtTrxn.Tables[0];
                    XPointMember = new string[ChartData.Rows.Count];
                    YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = ChartData.Rows[count]["day"].ToString();
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart11.Series[0].Points.DataBindXY(XPointMember, YPointMember);


                    this.Chart11.Series[0]["ShowMarkerLines"] = "true";
                    //this.Chart11.Series[0]["DrawingStyle"] = "Cylinder";
                    //this.Chart11.Series[0]["PointWidth"] = "0.8";
                    Chart11.Series[0].ChartType = SeriesChartType.Column;
                    Chart10.Titles[0].Text = "Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]\n ATM transactions for previous week";
                    Chart10.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                    Chart10.Titles[0].ForeColor = Color.Black;

                    Chart9.Titles[0].Text = "Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]\n ATM transactions for last week";
                    Chart9.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                    Chart9.Titles[0].ForeColor = Color.Black;

                    Chart11.Titles[0].Text = "Selected Criteria [" + TextBox5.Text + "-" + TextBox6.Text + "]\n ATM transactions for specified duration";
                    Chart11.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                    Chart11.Titles[0].ForeColor = Color.Black;

                    //                  Literal5.Text = "<b>Today [" + DateTime.Today.ToString("dd/MM/yyyy") + "]</b><br/>ATM transactions for today";
                    //Literal6.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]\n ATM transactions for last week";
                    //Literal7.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>ATM transactions for previous week"; ;
                    //Literal8.Text = "<b>Selected Criteria [" + TextBox5.Text + "-" + TextBox6.Text + "]</b><br/>ATM transactions for specified duration"; ;
                }
                else if (id == "Button6")
                {


                    if (Session[SessionVars.isRegionSelected.ToString()] != null)
                    {
                        int regionID = int.Parse(Session[SessionVars.SelectedRegion.ToString()].ToString());
                        if (regionID == 1) throw new Exception("Organization not selected.");
                        Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(regionID);
                        if (!region.IsOrganization)
                            throw new Exception("Organization not selected.");
                        else
                        {
                            Avanza.CCMS.DAL.Region.RegionReader reader = Avanza.CCMS.DAL.Region.ExecuteReader("parent_region_id = " + region.RegionId);

                            if (reader.Read())
                            {
                                //Chart12.Url = "Data.aspx?param=27@" + TextBox7.Text + "@" + TextBox8.Text + "@0@167AD4@" + reader.CurrentRegion.RegionId + "@" + isEjTrxnSelected;
                                Chart12.ChartAreas[0].AxisX.Title = "Hour";
                                Chart12.ChartAreas[0].AxisY.Title = "Transactions";
                                Chart12.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                                Chart12.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                                cmd = ConnectionFactory.GetNewCommand(false);
                                cmd.CommandTimeout = 180;
                                cmd.CommandText = isEjTrxnSelected == "1" ? "EJRegionTransactionsHourAnalysis" : "RegionTransactionsHourAnalysis";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox7.Text, "dd/MM/yyyy", null);

                                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox8.Text, "dd/MM/yyyy", null);

                                cmd.Parameters.Add("@RegionId", SqlDbType.Int);
                                cmd.Parameters[2].Value = reader.CurrentRegion.RegionId;
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                DataSet dtTrxn = new DataSet();
                                adapter.Fill(dtTrxn);
                                DataTable ChartData = dtTrxn.Tables[0];
                                int[] XPointMember = new int[ChartData.Rows.Count];
                                int[] YPointMember = new int[ChartData.Rows.Count];
                                for (int count = 0; count < ChartData.Rows.Count; count++)
                                {
                                    //storing Values for X axis  
                                    XPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["hr"]);
                                    //storing values for Y Axis  
                                    YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                                }
                                Chart12.ChartAreas[0].AxisX.Interval = 1;
                                //binding chart control  
                                Chart12.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                                //Setting width of line  
                                //Chart3.Series[0].BorderWidth = 1;
                                //setting Chart type   

                                Chart12.Series[0].ChartType = SeriesChartType.Column;
                                Chart12.Titles[0].Text = "Selected Criteria [" + TextBox7.Text + "-" + TextBox8.Text + "]\n Region wise hourly analysis for " + reader.CurrentRegion.RegionName + " Region";
                                Chart12.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                                Chart12.Titles[0].ForeColor = Color.Black;
                                //Literal12.Text = "<b>Selected Criteria [" + TextBox7.Text + "-" + TextBox8.Text + "]</b><br/>Region wise hourly analysis for " + reader.CurrentRegion.RegionName + " Region";
                            }

                            if (reader.Read())
                            {
                                //Chart13.Url = "Data.aspx?param=27@" + TextBox7.Text + "@" + TextBox8.Text + "@0@2A9c0A@" + reader.CurrentRegion.RegionId + "@" + isEjTrxnSelected;
                                Chart13.ChartAreas[0].AxisX.Title = "Hour";
                                Chart13.ChartAreas[0].AxisY.Title = "Transactions";
                                Chart13.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                                Chart13.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                                cmd = ConnectionFactory.GetNewCommand(false);
                                cmd.CommandTimeout = 180;
                                cmd.CommandText = isEjTrxnSelected == "1" ? "EJRegionTransactionsHourAnalysis" : "RegionTransactionsHourAnalysis";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox7.Text, "dd/MM/yyyy", null);

                                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox8.Text, "dd/MM/yyyy", null);

                                cmd.Parameters.Add("@RegionId", SqlDbType.Int);
                                cmd.Parameters[2].Value = reader.CurrentRegion.RegionId;
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                DataSet dtTrxn = new DataSet();
                                adapter.Fill(dtTrxn);
                                DataTable ChartData = dtTrxn.Tables[0];
                                string[] XPointMember = new string[ChartData.Rows.Count];
                                int[] YPointMember = new int[ChartData.Rows.Count];
                                for (int count = 0; count < ChartData.Rows.Count; count++)
                                {
                                    //storing Values for X axis  
                                    XPointMember[count] = ChartData.Rows[count]["hr"].ToString();
                                    //storing values for Y Axis  
                                    YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                                }
                                Chart13.ChartAreas[0].AxisX.Interval = 1;
                                //binding chart control  
                                Chart13.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                                //Setting width of line  
                                //Chart3.Series[0].BorderWidth = 1;
                                //setting Chart type   

                                Chart13.Series[0].ChartType = SeriesChartType.Column;

                                Chart13.Titles[0].Text = "Selected Criteria [" + TextBox7.Text + "-" + TextBox8.Text + "]\n Region wise hourly analysis for " + reader.CurrentRegion.RegionName + " Region";
                                Chart13.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                                Chart13.Titles[0].ForeColor = Color.Black;

                                //Literal13.Text = "<b>Selected Criteria [" + TextBox7.Text + "-" + TextBox8.Text + "]</b><br/>Region wise hourly analysis for " + reader.CurrentRegion.RegionName + " Region";
                            }

                            if (reader.Read())
                            {
                                //Chart14.Url = "Data.aspx?param=27@" + TextBox7.Text + "@" + TextBox8.Text + "@0@F59C0A@" + reader.CurrentRegion.RegionId + "@" + isEjTrxnSelected;
                                Chart14.ChartAreas[0].AxisX.Title = "Hour";
                                Chart14.ChartAreas[0].AxisY.Title = "Transactions";
                                Chart14.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                                Chart14.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);

                                cmd = ConnectionFactory.GetNewCommand(false);
                                cmd.CommandTimeout = 180;
                                cmd.CommandText = isEjTrxnSelected == "1" ? "EJRegionTransactionsHourAnalysis" : "RegionTransactionsHourAnalysis";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox7.Text, "dd/MM/yyyy", null);

                                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox8.Text, "dd/MM/yyyy", null);

                                cmd.Parameters.Add("@RegionId", SqlDbType.Int);
                                cmd.Parameters[2].Value = reader.CurrentRegion.RegionId;
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                DataSet dtTrxn = new DataSet();
                                adapter.Fill(dtTrxn);
                                DataTable ChartData = dtTrxn.Tables[0];
                                int[] XPointMember = new int[ChartData.Rows.Count];
                                int[] YPointMember = new int[ChartData.Rows.Count];
                                for (int count = 0; count < ChartData.Rows.Count; count++)
                                {
                                    //storing Values for X axis  
                                    XPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["hr"]);
                                    //storing values for Y Axis  
                                    YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                                }
                                Chart14.ChartAreas[0].AxisX.Interval = 1;
                                //binding chart control  
                                Chart14.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                                //Setting width of line  
                                //Chart3.Series[0].BorderWidth = 1;
                                //setting Chart type   

                                Chart14.Series[0].ChartType = SeriesChartType.Column;


                                Chart14.Titles[0].Text = "Selected Criteria [" + TextBox7.Text + "-" + TextBox8.Text + "]\n Region wise hourly analysis for " + reader.CurrentRegion.RegionName + " Region";
                                Chart14.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                                Chart14.Titles[0].ForeColor = Color.Black;
                                //Literal14.Text = "<b>Selected Criteria [" + TextBox7.Text + "-" + TextBox8.Text + "]</b><br/>Region wise hourly analysis for " + reader.CurrentRegion.RegionName + " Region";
                            }

                            reader.Close();
                        }
                    }
                    else
                        throw new Exception("Organization not selected.");


                }
                else if (id == "Button7")
                {
                    StringBuilder builder = new StringBuilder();
                    if (Session[SessionVars.isRegionSelected.ToString()] != null)
                    {
                        int regionID = int.Parse(Session[SessionVars.SelectedRegion.ToString()].ToString());
                        if (regionID == 1) throw new Exception("Organization not selected.");
                        Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(regionID);
                        if (!region.IsOrganization)
                            throw new Exception("Organization not selected.");
                        else
                        {
                            //var regionName = new List<string>();
                            //Avanza.CCMS.DAL.Region.RegionReader reader = Avanza.CCMS.DAL.Region.ExecuteReader("parent_region_id = " + region.RegionId);
                            //while (reader.Read())
                            //{
                            //    builder.Append(reader.CurrentRegion.RegionId + ",");
                            //    regionName.Add(reader.CurrentRegion.RegionName + ",");

                            //}
                            //Session["regionIds"] = builder.ToString();
                            //Chart15.Url = "Data.aspx?param=28@" + TextBox9.Text + "@" + TextBox10.Text + "@0@167AD4@" + isEjTrxnSelected;

                            List<string> regionIds = new List<string>();
                            List<string> regionNames = new List<string>();
                            string[] colors = { "#00A77E", "#008D98", "#0099BF", "#31A9E0", "#ED9A22", "#F1851C", "#E0D120", "#A6CE39" };
                            //System.Drawing.ColorTranslator.FromHtml("#00A77E")





                            foreach (System.Web.UI.WebControls.ListItem reg in Checkbox_region.Items)
                            {
                                if (reg.Selected)
                                {
                                    regionIds.Add(reg.Value);
                                    regionNames.Add(reg.Text);
                                }

                            }
                            //if(regionIds.Count < 2)
                            //{
                            //    throw new Exception("Minimum 2 regions should be selected.");
                            //}
                            if (regionIds.Count > 8)
                            {
                                throw new Exception("Maximum 8 regions should be selected.");
                            }

                            Chart15.Titles[0].Text = "Multi Region Analysis";
                            Chart15.ChartAreas[0].AxisY.Title = "Transaction";
                            Chart15.ChartAreas[0].AxisX.Title = "Dates";
                            Chart15.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                            Chart15.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                            //string[] regionIds = Session["regionIds"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            this.Chart15.Legends[0].Enabled = true;
                            Chart15.ChartAreas[0].AxisX.Interval = 6;
                            int totalcount = 0;
                            for (int regionid = 0; regionid < regionIds.Count; regionid++)
                            {


                                cmd = ConnectionFactory.GetNewCommand(false);
                                cmd.CommandTimeout = 180;
                                cmd.CommandText = isEjTrxnSelected == "1" ? "EJMultiRegionTransactionAnalysis" : "MultiRegionTransactionAnalysis";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox9.Text, "dd/MM/yyyy", null);

                                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox10.Text, "dd/MM/yyyy", null);

                                cmd.Parameters.Add("@RegionId", SqlDbType.Int);
                                cmd.Parameters[2].Value = int.Parse(regionIds[regionid]);
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                DataSet dtTrxn = new DataSet();
                                adapter.Fill(dtTrxn);
                                DataTable ChartData = dtTrxn.Tables[0];
                                string[] XPointMember = new string[ChartData.Rows.Count];
                                int[] YPointMember = new int[ChartData.Rows.Count];

                                for (int count = 0; count < ChartData.Rows.Count; count++)
                                {
                                    //storing Values for X axis  
                                    XPointMember[count] = ChartData.Rows[count][1].ToString();
                                    //storing values for Y Axis  
                                    YPointMember[count] = Convert.ToInt32(ChartData.Rows[count][0]);
                                    totalcount += Convert.ToInt32(ChartData.Rows[count][0]);


                                }
                                if (ChartData.Rows.Count > 0)
                                {

                                    Series temp = new Series();
                                    temp.Points.DataBindXY(XPointMember, YPointMember);
                                    temp.ChartType = SeriesChartType.Column;
                                    temp.Name = regionNames[regionid];
                                    temp.Color = System.Drawing.ColorTranslator.FromHtml(colors[regionid]);
                                    Chart15.Series.Add(temp);


                                }

                            }
                            Chart15.Titles[0].Text = "Selected Criteria [" + TextBox9.Text + "-" + TextBox10.Text + "]\n Multi Region Analysis";
                            Chart15.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                            Chart15.Titles[0].ForeColor = Color.Black;

                            Literal15.Text = "Total Transactions count is " + totalcount;
                        }
                    }
                    else
                        throw new Exception("Organization not selected.");
                }
                else if (id == "Button12")
                {
                    GetEventsSummary();
                }

                else if (id == "Button10")
                {
                    GridView_ATMIncidents.DataSource = GetIncidents();
                    GridView_ATMIncidents.DataBind();
                    GridView_ServiceUtil.DataSource = GetServiceUtilization();
                    GridView_ServiceUtil.DataBind();
                    GridView_ServiceAvailability.DataSource = GetServiceAvailability();
                    GridView_ServiceAvailability.DataBind();

                    DrawKPIChart(TextBox13.Text, TextBox14.Text);
                    DrawGoogleGaugeChart();




                }
                else if (id == "Button8")
                {
                    ChartTransactionTypeBar.Series[0].Points.Clear();
                    ChartFailureReasons.Series[0].Points.Clear();

                    System.Data.DataTable dt = GetTransactionType();
                    foreach (System.Data.DataRow dr in dt.Rows)
                        ChartTransactionTypeBar.Series[0].Points.AddXY(dr[0], dr[1]);

                    this.ChartTransactionTypeBar.Titles[0].Text = "Transaction Type Analysis";
                    this.ChartTransactionTypeBar.Series["Default"]["ShowMarkerLines"] = "true";
                    this.ChartTransactionTypeBar.Series["Default"]["DrawingStyle"] = "Cylinder";
                    this.ChartTransactionTypeBar.Series["Default"]["PointWidth"] = "0.8";

                    dt = GetFailureReasons();
                    foreach (System.Data.DataRow dr in dt.Rows)
                        ChartFailureReasons.Series[0].Points.AddXY(dr[0], dr[1]);


                    this.ChartFailureReasons.Titles[0].Text = "Failed Transactions Analysis";
                    this.ChartFailureReasons.Series["Default"]["ShowMarkerLines"] = "true";
                    this.ChartFailureReasons.Series["Default"]["DrawingStyle"] = "Cylinder";
                    this.ChartFailureReasons.Series["Default"]["PointWidth"] = "0.8";


                    //this.ChartFailureReasons.Titles[0].Text = "Error Analsis";
                    //this.ChartFailureReasons.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                    //this.ChartFailureReasons.Series[0]["PieDrawingStyle"] = "SoftEdge";
                    //this.ChartFailureReasons.Legends[0].Enabled = true;
                    //this.ChartFailureReasons.Series["Default"].ChartType = SeriesChartType.Pie;
                    //this.ChartFailureReasons.Series["Default"]["PieLabelStyle"] = "Outside";

                    try
                    {
                        GetTop10LowTransactingATM();
                    }
                    catch (Exception ex)
                    {
                        task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                    }

                    try
                    {
                        string atmTitles = GetTop10TransactingATM();
                        GetOnUsNotOnUsTransactionsBreakup(atmTitles);
                    }
                    catch (Exception ex)
                    {
                        task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                    }
                    try
                    {
                        PopulateNetworkStatistics();
                    }
                    catch (Exception ex)
                    {
                        task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                    }

                    try
                    {
                        PopulateRegionWiseNetworkGrid();
                    }
                    catch (Exception ex)
                    {
                        task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                    }
                    try
                    {
                        PopulateSummaryGrid();
                    }
                    catch (Exception ex)
                    {
                        task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                    }
                    try
                    {
                        GetTransactionAnalysis();
                    }
                    catch (Exception ex)
                    {
                        task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                    }
                }

                else
                {
                    // Literal_GraphTitle.Text = "<b>Selected Criteria [" + TextBox3.Text + "-" + TextBox4.Text + "]</b><br/>Region wise breakup of transactions for specified period"; ;
                    //Chart7.Url = "Data.aspx?param=21@" + TextBox3.Text + "@" + TextBox4.Text + "@0@0@167AD4@" + isEjTrxnSelected;
                    Chart7.ChartAreas[0].AxisX.Title = "Transactions";
                    Chart7.ChartAreas[0].AxisY.Title = "Atms";
                    Chart7.ChartAreas[0].AxisY.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart7.ChartAreas[0].AxisX.TitleFont = new Font("Verdana", 11, FontStyle.Regular);
                    Chart7.Titles[0].Text = "Selected Criteria [" + TextBox3.Text + "-" + TextBox4.Text + "]\n Region wise breakup of transactions for specified period";
                    Chart7.Titles[0].Alignment = ContentAlignment.MiddleCenter;
                    Chart7.Titles[0].ForeColor = Color.Black;

                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 180;
                    cmd.CommandText = "RegionWiseTransaction";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(TextBox3.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(TextBox4.Text, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@OrganizationId", SqlDbType.Int);
                    cmd.Parameters[2].Value = 1;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    DataTable ChartData = dtTrxn.Tables[0];
                    string[] XPointMember = new string[ChartData.Rows.Count];
                    int[] YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        //storing Values for X axis  
                        XPointMember[count] = ChartData.Rows[count]["region_name"].ToString();
                        //storing values for Y Axis  
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count]["counter"]);


                    }
                    //binding chart control  
                    Chart7.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                    //Setting width of line  
                    //Chart3.Series[0].BorderWidth = 1;
                    //setting Chart type   
                    Chart7.Series[0]["PieLabelStyle"] = "Outside";
                    Chart7.Series[0].BorderWidth = 1;
                    Chart7.Series[0].BorderDashStyle = ChartDashStyle.Solid;
                    this.Chart7.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                    this.Chart7.Series[0]["PieDrawingStyle"] = "SoftEdge";
                    this.Chart7.Legends[0].Enabled = true;
                    Chart7.Series[0].ChartType = SeriesChartType.Pie;
                    foreach (Series charts in Chart7.Series)
                    {
                        int colorCount = 0;
                        foreach (DataPoint point in charts.Points)
                        {
                            //BackdropItem bd = default(BackdropItem);  
                            switch (colorCount)
                            {
                                case 1: point.Color = System.Drawing.ColorTranslator.FromHtml("#00A77E"); break;
                                case 2: point.Color = System.Drawing.ColorTranslator.FromHtml("#008D98"); break;
                                case 3: point.Color = System.Drawing.ColorTranslator.FromHtml("#0099BF"); break;
                                case 4: point.Color = System.Drawing.ColorTranslator.FromHtml("#31A9E0"); break;
                                case 5: point.Color = System.Drawing.ColorTranslator.FromHtml("#ED9A22"); break;
                                case 6: point.Color = System.Drawing.ColorTranslator.FromHtml("#F1851C"); break;
                                case 7: point.Color = System.Drawing.ColorTranslator.FromHtml("#E0D120"); break;
                                case 8: point.Color = System.Drawing.ColorTranslator.FromHtml("#A6CE39"); break;
                            }

                            colorCount = colorCount + 1;
                            //point.Properties["Backdrop"] = bd;
                            point.Label = string.Format("{0:0} - {1}", point.YValues[0], point.AxisLabel);

                        }
                    }
                    BindRegionAnalysisGrid((string)ViewState["orderby"]);

                }
            }
            catch (Exception ex)
            {

                task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                Literal_Message.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }

        }
        private void GetTop10LowTransactingATM()
        {
            LogableTask task = LogableTask.NewTask("GetTop10LowTransactingATM");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetEJTop10ATMs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox11.Text, "dd/MM/yyyy", null);
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox12.Text, "dd/MM/yyyy", null);
                cmd.Parameters.Add(new SqlParameter("order", SqlDbType.Int));
                cmd.Parameters[2].Value = 0;
                cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                cmd.Parameters[3].Value = Session[SessionVars.SelectedATMs.ToString()];
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                this.GridView_NoTransactingATM.DataSource = dt;
                this.GridView_NoTransactingATM.DataBind();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_Message.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private void PopulateSummaryGrid()
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("type");
            dtGrid.Columns.Add("val");

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select * from vEjParsedTransactions 
                                inner join transaction_type on transaction_type.transaction_type_id = vEjParsedTransactions.transaction_type_id  " +
                                " inner join atm on vEjParsedTransactions.atm_id = atm.atm_id " +
                                " where trxn_datetime >= CONVERT(datetime,'" + TextBox11.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox12.Text + " 23:59:59',103) and atm.atm_id in ("
                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                               "  and atm.is_active = 1  and status =0 ";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);

            DataRow newRow = dtGrid.NewRow();
            newRow[0] = "Transaction Count";
            newRow[1] = dt.Rows.Count;
            dtGrid.Rows.Add(newRow);

            newRow = dtGrid.NewRow();
            newRow[0] = "Withdrawal";
            DataRow[] drArray = dt.Select("transaction_type_name like '%withdraw%' or transaction_type_name like '%fast%'");
            newRow[1] = drArray.Length;
            dtGrid.Rows.Add(newRow);

            decimal total = 0;
            newRow = dtGrid.NewRow();
            newRow[0] = "Total Cash Out";
            drArray = dt.Select("transaction_type_name like '%withdraw%' or transaction_type_name like '%fast%'");
            foreach (DataRow dr in drArray)
            {
                if (dr["amount"] != DBNull.Value)
                    total += decimal.Parse(dr["amount"].ToString());
            }
            newRow[1] = total;
            dtGrid.Rows.Add(newRow);

            GridView_Summary.DataSource = dtGrid;
            GridView_Summary.DataBind();
        }

        private void PopulateRegionWiseNetworkGrid()
        {
            ChartNetworkStats.Series.Clear();
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("Region");




            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select  region_name,FITMapping.bank_name,COUNT(ej_parsed_transactions_id) total
                                from vEjParsedTransactions inner join atm
                               
                                on vEjParsedTransactions.atm_id = atm.atm_id  
                                inner join FITMapping on  substring(vEjParsedTransactions.pan,1,6) = 
                                FITMapping.PAN_prefix
                                inner join region on region.region_id = atm.region_id " +
" where trxn_datetime >= CONVERT(datetime,'" + TextBox11.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox12.Text + " 23:59:59',103) and atm.atm_id in ("
                                + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                               "  and atm.is_active = 1 and status=0" +
                                " group by FITMapping.bank_name,region_name order by COUNT(ej_parsed_transactions_id) ";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);

            DataTable trxn = dt.DefaultView.ToTable(true, new string[] { "bank_name" });
            foreach (DataRow dr in trxn.Rows)
            {
                dtGrid.Columns.Add(dr[0].ToString());
                Series series = new Series(dr[0].ToString());
                series.BorderWidth = 10;
                series.MarkerStyle = MarkerStyle.Circle;
                series.ChartType = SeriesChartType.StackedColumn;
                series.MarkerSize = 10;


                //series.BorderColor = System.Drawing.Color.Navy;
                //series.Color = System.Drawing.Color.OldLace;
                ChartNetworkStats.Series.Add(series);
            }


            DataTable dtRegion = dt.DefaultView.ToTable(true, new string[] { "region_name" });
            decimal total = 0;
            int idx = 1;
            foreach (DataRow dr in dtRegion.Rows)
            {

                DataRow[] drArray = dt.Select("region_name='" + dr[0] + "'");
                if (drArray.Length > 0)
                {
                    DataRow newRow = dtGrid.NewRow();
                    newRow[0] = drArray[0][0];
                    foreach (DataRow dr1 in drArray)
                    {
                        total += decimal.Parse(dr1["total"].ToString());
                    }
                    foreach (DataRow dr1 in drArray)
                    {
                        newRow[idx] = Math.Round((decimal.Parse(dr1["total"].ToString()) / total) * 100, 2);
                        ChartNetworkStats.Series[idx - 1].Points.AddXY(dr[0], newRow[idx]);
                        newRow[idx] += "%";
                        idx++;

                    }
                    dtGrid.Rows.Add(newRow);
                }
            }
            GridView_NetworkStatisticsDetails.DataSource = dtGrid;
            GridView_NetworkStatisticsDetails.DataBind();



            //for (int j = 0; j < ChartNetworkStats.Series.Count; j++)
            //{
            //    //ChartNetworkStats.Series[j]["StackedGroupName"] = "Group1";
            //    ChartNetworkStats.Series[j].IsValueShownAsLabel = true;


            //}

            ChartNetworkStats.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = true;
            ChartNetworkStats.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;
            ChartNetworkStats.Legends[0].Enabled = true;
            ChartNetworkStats.ResetAutoValues();






        }
        private void PopulateNetworkStatistics()
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("pan_prefix");
            dtGrid.Columns.Add("total");

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select  FITMapping.bank_name,COUNT(ej_parsed_transactions_id) total
                                from vEjParsedTransactions inner join atm
                               
                                on vEjParsedTransactions.atm_id = atm.atm_id  
                                inner join FITMapping on  substring(vEjParsedTransactions.pan,1,6) = 
                                FITMapping.PAN_prefix " +

 " where trxn_datetime >= CONVERT(datetime,'" + TextBox11.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox12.Text + " 23:59:59',103)  " +
                                " and atm.atm_id in ("
                    + Session[SessionVars.SelectedATMs.ToString()] + ") and status =0  group by FITMapping.bank_name";

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);

            decimal total = 0;
            foreach (DataRow dr in dt.Rows)
                total += decimal.Parse(dr["total"].ToString());

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = dtGrid.NewRow();
                newRow[0] = dr["bank_name"];
                newRow[1] = Math.Round((decimal.Parse(dr["total"].ToString()) / total) * 100, 2) + "%";
                dtGrid.Rows.Add(newRow);
            }


            GridView_NetworkStatistics.DataSource = dtGrid;
            GridView_NetworkStatistics.DataBind();
        }

        private void GetOnUsNotOnUsTransactionsBreakup(string atmTitles)
        {
            LogableTask task = LogableTask.NewTask("GetOnUsNotOnUsTransactionsBreakup");
            try
            {
                double onUsTotal = 0;
                double notOnUsTotal = 0;

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select  COUNT(ej_parsed_transactions_id) total,title,isnull(on_us,0)
                                from vEjParsedTransactions inner join atm
                               
                                on vEjParsedTransactions.atm_id = atm.atm_id  
                                inner join FITMapping on  substring(vEjParsedTransactions.pan,1,6) = 
                                FITMapping.PAN_prefix
                                where trxn_datetime >= CONVERT(datetime,'" + TextBox11.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox12.Text + " 23:59:59',103) and title in (" + atmTitles + ") and status =0" +
                                " group by title,on_us " +
                                 " order by  title,COUNT(ej_parsed_transactions_id)";


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new System.Data.DataTable();
                adapter.Fill(dt);

                DataTable dtTitles = dt.DefaultView.ToTable(true, new string[] { "title" });

                foreach (DataRow dr in dtTitles.Rows)
                {

                    DataRow[] drArray = dt.Select("title = '" + dr[0] + "'");

                    if (drArray.Length > 0)
                    {

                        if (drArray[0][2].ToString() == "False")
                        {

                            notOnUsTotal += double.Parse(drArray[0][0].ToString());
                        }
                        else
                        {

                            onUsTotal += double.Parse(drArray[0][0].ToString());
                        }
                    }
                    if (drArray.Length > 1)
                    {
                        if (drArray[1][2].ToString() == "False")
                        {

                            notOnUsTotal += double.Parse(drArray[1][0].ToString());
                        }
                        else
                        {

                            onUsTotal += double.Parse(drArray[1][0].ToString());
                        }
                    }

                }

                DrawProfitabilityAnalysisGraph(onUsTotal, notOnUsTotal);
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_Message.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private void DrawProfitabilityAnalysisGraph(double onUsTotal, double notOnUsTotal)
        {
            LogableTask task = LogableTask.NewTask("DrawProfitabilityAnalysisGraph");
            try
            {
                ChartProfitablityAnalysis.Series[0].Points.Clear();
                double total = onUsTotal + notOnUsTotal;
                double bar1 = Math.Round((onUsTotal / total) * 100, 2);
                double bar2 = Math.Round((notOnUsTotal / total) * 100, 2);


                ChartProfitablityAnalysis.Series[0].Points.AddXY("On Us", bar1);
                ChartProfitablityAnalysis.Series[0].Points.AddXY("Not On Us", bar2);

                this.ChartProfitablityAnalysis.Titles[0].Text = "Profitability Analysis";
                //this.ChartProfitablityAnalysis.Series["Default"]["ShowMarkerLines"] = "true";
                //this.ChartProfitablityAnalysis.Series["Default"]["DrawingStyle"] = "Cylinder";
                //this.ChartProfitablityAnalysis.Series["Default"]["PointWidth"] = "0.8";


                this.ChartProfitablityAnalysis.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                this.ChartProfitablityAnalysis.Series[0]["PieDrawingStyle"] = "SoftEdge";
                this.ChartProfitablityAnalysis.Legends[0].Enabled = true;
                this.ChartProfitablityAnalysis.Series["Default"].ChartType = SeriesChartType.Pie;
                this.ChartProfitablityAnalysis.Series["Default"]["PieLabelStyle"] = "Outside";

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_Message.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
        }




        private string GetTop10TransactingATM()
        {
            LogableTask task = LogableTask.NewTask("GetTop10TransactingATM");
            StringBuilder builder = new StringBuilder();
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "GetEJTop10ATMs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.ParseExact(TextBox11.Text, "dd/MM/yyyy", null);
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = DateTime.ParseExact(TextBox12.Text, "dd/MM/yyyy", null);
                cmd.Parameters.Add(new SqlParameter("order", SqlDbType.Int));
                cmd.Parameters[2].Value = 1;
                cmd.Parameters.Add(new SqlParameter("atmIDs", SqlDbType.VarChar));
                cmd.Parameters[3].Value = Session[SessionVars.SelectedATMs.ToString()];


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new System.Data.DataTable();
                adapter.Fill(dt);
                this.GridView_Transacting.DataSource = dt;
                this.GridView_Transacting.DataBind();
                DataTable dtTitles = dt.DefaultView.ToTable(true, new string[] { "title" });
                foreach (DataRow dr in dtTitles.Rows)
                    builder.Append("'" + dr[0] + "',");
                builder.Append("'-1'");

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                this.Literal_Message.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                task.EndTask();
            }
            return builder.ToString();
        }

        private System.Data.DataTable GetTransactionType()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select transaction_type_name,COUNT(ej_parsed_transactions_id)
                                from vEjParsedTransactions inner join transaction_type
                                on vEjParsedTransactions.transaction_type_id = transaction_type.transaction_type_id 
                                where trxn_datetime >= CONVERT(datetime,'" + TextBox11.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox12.Text + " 23:59:59',103) " +
                                " and vEjParsedTransactions.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()].ToString() + ") and status = 0" +
                                " group by transaction_type_name";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }
        private System.Data.DataTable GetFailureReasons()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select comment_text,COUNT(ej_parsed_transactions_id)
                                from vEjParsedTransactions inner join comment
                                on vEjParsedTransactions.comment_id = comment.comment_id 
                                where trxn_datetime >= CONVERT(datetime,'" + TextBox11.Text + "',103) " +
                                " and trxn_datetime <=CONVERT(datetime,' " + TextBox12.Text + " 23:59:59',103) " +
                                " and vEjParsedTransactions.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()].ToString() + ") and status = 1" +
                                " group by comment_text";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        private void BindRegionAnalysisGrid(string orderBy)
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlCommand cmd = conn.CreateCommand();
            string isEjTrxnSelected = Request.QueryString["ej"] == null ? "0" : "1";

            if (isEjTrxnSelected == "1")
            {
                cmd.CommandText = @"select title, region_name, count(*) as totalTrxn
from vEjParsedTransactions inner join atm on vEjParsedTransactions.atm_id = atm.atm_id 
inner join region on atm.region_id = region.region_id where trxn_datetime>=convert(datetime,'" + TextBox3.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox4.Text + " 23:59:59',103) " +
    " group by title,region_name order by " + orderBy;
            }
            else
                cmd.CommandText = @"select title, region_name, count(*) as totalTrxn
from vParsedTransaction inner join atm on vParsedTransaction.atm_id = atm.atm_id 
inner join region on atm.region_id = region.region_id where trxn_datetime>=convert(datetime,'" + TextBox3.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox4.Text + " 23:59:59',103) " +
    " group by title,region_name order by " + orderBy;

            SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);
            GridView_RegionTrxnDetail.DataSource = dt;
            GridView_RegionTrxnDetail.DataBind();
        }

        protected void GridView_RegionTrxnDetail_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }


            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            ///BindGrid(null);
            BindRegionAnalysisGrid((string)ViewState["orderby"]);
        }





        protected void Button_ShowData_Click(object sender, EventArgs e)
        {

        }
        protected void DropDownList_Criteria_Changed(object sender, EventArgs e)
        {

            if (DropDownList_Criteria.SelectedIndex == 0)
                DrawKPIChart(TextBox13.Text, TextBox14.Text);
            else if (DropDownList_Criteria.SelectedIndex == 1)
                DrawKPIChart(DateTime.Now.AddDays(-10).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));
            else if (DropDownList_Criteria.SelectedIndex == 2)
                DrawKPIChart(DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));
            else if (DropDownList_Criteria.SelectedIndex == 3)
                DrawKPIChart(DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));
            else if (DropDownList_Criteria.SelectedIndex == 3)
                DrawKPIChart(DateTime.Now.AddYears(-1).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));

        }



        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            //Session[SessionVars.SelectedATMs.ToString()] = e.Parameter.Substring(1);

            //((Button)sender).ID
            //if (Session[SessionVars.SelectedATMs.ToString()] == null)
            //    Literal_Message.Text = "Please select an ATM or region from the tree";
            //else
            //{
            GenerateGraph(((System.Web.UI.WebControls.Button)sender).ID);
            //}


        }
        protected void DialogCallBack_Callback(object sender, CallBackEventArgs e)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
            try
            {


                StringBuilder builder = new StringBuilder();
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 600;
                DateTime fromDate = DateTime.ParseExact(e.Parameters[0], "dd/MM/yyyy", null);
                DateTime toDate = DateTime.ParseExact(e.Parameters[1], "dd/MM/yyyy", null);
                string atmID = e.Parameters[2];
                string taskID = e.Parameters[3];
                int serialNo = 1;
                decimal amount = 0;
                string comment = "";

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTrasactionsSummaryDetailByTaskID";
                cmd.Parameters.Add(new SqlParameter("FromDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = fromDate;
                cmd.Parameters.Add(new SqlParameter("ToDate", SqlDbType.DateTime));
                cmd.Parameters[1].Value = toDate;
                cmd.Parameters.Add(new SqlParameter("AtmId", SqlDbType.Int));
                cmd.Parameters[2].Value = atmID;
                cmd.Parameters.Add(new SqlParameter("TaskId", SqlDbType.Int));
                cmd.Parameters[3].Value = taskID;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                builder.Append("<tr><td><b>S.No</b></td><td><b>Title</b></td><td><b>Transaction Type</b></td><td><b>Datetime</b></td><td><b>Amount</b></td><td><b>Reason</b></td></tr>"); // Header..
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["comment_text"] != DBNull.Value)
                        comment = dr["comment_text"].ToString().Replace("\r\n", "").Replace("\r", "");
                    else
                        comment = "&nbsp";
                    builder.Append("<tr><td><b>" + serialNo++ + "</b></td><td><b>" + dr[0] + "</b></td><td><b>" + dr[3] + "</b></td><td><b>" + DateTime.Parse(dr[1].ToString()).ToString("dd/MM/yyyy HH:mm:ss") + "</b></td><td><b>" + dr[2] + "</b></td><td><b>" + comment + "</b></td></tr>");
                    if (dr[2] != DBNull.Value)
                        amount += decimal.Parse(dr[2].ToString());
                }
                builder.Append("<tr><td></td><td></td><td></td><td><b>Total Amount</b></td><td><b>" + amount + "</b></td></tr>");
                e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>');</script>");


            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask();
            }
        }

    }
}