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
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;

namespace CCMSUI
{
    public partial class ParamPNCDepositReportingForCPM : Page
    {
        string[] colorArr = { "#d8422D", "#ffba04", "#1d71ef", "#01A05b", "#d8422D" };
       
        int id = 1;
        int reportTaskId = 0;
        private string Filter()
        {
            string filter = "";

            ReportSchedule reportSchedule = ReportSchedule.LoadReportScheduleByPk(id);
            ReportTask reportTask = ReportTask.LoadReportTaskByPk(reportTaskId);
            Literal_Title.Text = reportSchedule.ReportFriendlyName;
            Label_currentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            if (reportSchedule.ReportName.ToLower().Contains("sdm"))
                filter += " and is_sdm = 1 ";
            else
                filter += " and is_ccdm = 1 ";
            //if (DropDownList_Comment.SelectedIndex>0)
            //    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";
            if (reportSchedule.CriteriaId.HasValue)
            {
                if (reportSchedule.CriteriaId == 1)
                    filter += " and dispute_status is not null ";
                else if (reportSchedule.CriteriaId == 2)
                    filter += " and (dispute_status is null or dispute_status = '000') ";
            }
            filter += " and is_eligible = 1";
            //if (CheckBoxList_ErrorAnalysisType.Items[0].Selected && CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            //    criteria = "";
            //else if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
            //    criteria = " and dispute_status is not null ";
            //else if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            //    criteria = " and dispute_status is null ";
            return filter;
        }
        
        private void DrawCheckRejectRateGraph(string fromDate,string toDate)
        {

          
           
            Chart_RejectRate.Series[0].Points.Clear();
            Chart_RejectRate.Series[1].Points.Clear();
            Chart_RejectRate.Series[2].Points.Clear();

            //string SQL = "select top 7 status,CONVERT(varchar,trxn_datetime,103) trxn_datetime,COUNT(*) from ej_parsed_cpm_transaction " +
            //    " inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id where atm.is_active=1  and  trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
            //    " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " +
            //    " group by status,CONVERT(varchar,trxn_datetime,103)  " +
            //    " order by convert(datetime,CONVERT(varchar,trxn_datetime,103),103)" ;

            string SQL = "select status,CONVERT(varchar,trxn_datetime,103) trxn_datetime from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                        " inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id where atm.is_active=1  and  trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                        " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " + Filter();


            DataTable dt = GetDataTable(SQL);

            //DataRow[] drReject = dt.Select(" status = 'Failed'");
            //DataRow[] drSuccess = dt.Select(" status = 'Sucessful'");
            //DataRow[] drNonBusinessRuleReject = dt.Select("dispute_status is null and status = 'Failed'");
            //DataRow[] drAccepted = dt.Select("dispute_status ='000' and status = 'Successful'");





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
            //bool series3DataPointAdded = false;
            int series1DataPointVal = 0;
            int series2DataPointVal = 0;
            string xAxisLable = "";
            int counter = 0;
            foreach (DataRow dr in trxnDatetime.Rows)
            {
                //counter++;
                if (counter == 7)
                    break;
                series1DataPointAdded = false;
                series2DataPointAdded = false;
                //series3DataPointAdded = false;
                series1DataPointVal = 0;
                series2DataPointVal = 0;

                DataRow[] drArray = dt.Select("trxn_datetime='" + dr[0] + "'", "status");
                xAxisLable = dr[0].ToString();

                for (int j = 0; j < drArray.Length; j++)
                {
                    if (drArray[j]["status"].ToString() == "Successful")
                    {

                        series1DataPointAdded = true;
                        series1DataPointVal++;// = int.Parse(drArray[j][2].ToString());
                    }

                    if (drArray[j]["status"].ToString() == "Failed")
                    {

                        series2DataPointAdded = true;
                        series2DataPointVal++;// = int.Parse(drArray[j][2].ToString());
                    }


                }
                if (series1DataPointAdded && series2DataPointAdded)
                {
                    Chart_RejectRate.Series[0].Points.AddXY(xAxisLable, series1DataPointVal);
                    Chart_RejectRate.Series[1].Points.AddXY(xAxisLable, series2DataPointVal);
                    Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round(((decimal)series2DataPointVal / (series2DataPointVal + series1DataPointVal)) * 100, 2));
                    counter++;
                    //series3DataPointAdded = true;
                }

                //if (!series1DataPointAdded)
                //    Chart_RejectRate.Series[0].Points.AddXY(dr[0], 0);
                //if (!series2DataPointAdded)
                //    Chart_RejectRate.Series[1].Points.AddXY(dr[0], 0);
                //if (!series3DataPointAdded)
                //    Chart_RejectRate.Series[2].Points.AddXY(dr[0], 0);


            }

            //for (int j = 0; j < ChartKPISummary.Series.Count; j++)
            //{
            //    ChartKPISummary.Series[j]["StackedGroupName"] = "Group1";
            //    ChartKPISummary.Series[j].IsValueShownAsLabel = true;


            //}

            Chart_RejectRate.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            Chart_RejectRate.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;
            //Chart_RejectRate.Legends[0].Enabled = false;
            Chart_RejectRate.ResetAutoValues();

        }
        private void DrawNonBusinessRuleRejectCausesGraph(string fromDate, string toDate)
        {
            string SQL = "select top 10 comment,count(*) from ej_parsed_cpm_transaction inner join atm on atm.atm_id =ej_parsed_cpm_transaction.atm_id " +
                       " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " +
                       " and dispute_status is null and status = 'Failed' "+Filter() +"  and len(comment)>1 group by comment order by count(*) desc";


           

            
            DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
                Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(dr[0], dr[1]);

            for (int i = 0; i < Chart_NonBusinessRuleRejectCauses.Series["Default"].Points.Count; i++)
                Chart_NonBusinessRuleRejectCauses.Series["Default"].Points[i].Color = System.Drawing.ColorTranslator.FromHtml(colorArr[i % 5]);

            Chart_NonBusinessRuleRejectCauses.Titles[0].Text = "TOP 10 NON-BUSINESS RULE REJECT CAUSES";
            //Chart_NonBusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart_NonBusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
            //Chart_NonBusinessRuleRejectCauses.Legends[0].Enabled = true;

            Chart_NonBusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
            Chart_NonBusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
            Chart_NonBusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";


        }

        private void DrawBusinessRuleRejectCausesGraph(string fromDate, string toDate)
        {
            string comment = null; 
            string SQL = "select top 10 comment,count(*) from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id" +
                       " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " +
                       " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' "+ Filter() +"  and len(comment)>1 group by comment order by count(*) desc";
            DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
            {
                comment = dr[0].ToString();
                //if (comment.Length > 30)
                //    comment = comment.Substring(0, 30);
                //
                Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);

            }
            for (int i = 0; i < Chart_BusinessRuleRejectCauses.Series["Default"].Points.Count; i++)
                Chart_BusinessRuleRejectCauses.Series["Default"].Points[i].Color = System.Drawing.ColorTranslator.FromHtml(colorArr[i % 5]);

            Chart_BusinessRuleRejectCauses.Titles[0].Text = "TOP 10 BUSINESS RULE REJECT CAUSES";
            //Chart_BusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart_BusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
            //Chart_BusinessRuleRejectCauses.Legends[0].Enabled = true;

            Chart_BusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
            Chart_BusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
            Chart_BusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";



        }

        private void  DrawAcceptanceByCheckCountGraph(string fromDate,string toDate)
        {

            string SQL = "select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                       " inner join atm on atm.atm_id =ej_parsed_cpm_transaction.atm_id  where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " + Filter();

          

            DataTable dt = GetDataTable(SQL);
            Label_totalATMs.Text = dt.DefaultView.ToTable(true, new string[] { "atm_id" }).Rows.Count.ToString();
           
            DataRow[] drBusinessRuleReject = dt.Select("dispute_status <>'000' and dispute_status is not null and status = 'Failed'");
            DataRow[] drNonBusinessRuleReject = dt.Select("dispute_status is null and status = 'Failed'");
            DataRow[] drAccepted = dt.Select("dispute_status ='000' and status = 'Successful'");

            int totalChecks = drBusinessRuleReject.Length + drNonBusinessRuleReject.Length + drAccepted.Length;
            if (totalChecks > 0)
                Label_AccPerc.Text = Math.Round((decimal)drAccepted.Length / totalChecks * 100,2).ToString();
            else
                Label_AccPerc.Text = "0";
            Label_RejPerc.Text = (100 - decimal.Parse(Label_AccPerc.Text)).ToString();
            Label_ItemsProcessed.Text = totalChecks.ToString();
            
            Chart_AccCount.Series["Default"].Points.AddXY("BUSINESS RULES REJECTS", drBusinessRuleReject.Length);
            Chart_AccCount.Series["Default"].Points.AddXY("NON-BUSINESS RULES REJECTS", drNonBusinessRuleReject.Length);
            Chart_AccCount.Series["Default"].Points.AddXY("CHECKS ACCEPTED", drAccepted.Length);
            Chart_AccCount.Series["Default"].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#E0400A");
            Chart_AccCount.Series["Default"].Points[2].Color = System.Drawing.ColorTranslator.FromHtml("#32BE5F");

            Chart_AccCount.Series["Default"].ChartType = SeriesChartType.Pie;
            Chart_AccCount.Series["Default"]["PieLabelStyle"] = "Outside";
            Chart_AccCount.Titles[0].Text = "ACCEPTANCE BY CHECK COUNT";
            Chart_AccCount.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart_AccCount.Series[0]["PieDrawingStyle"] = "SoftEdge";
            Chart_AccCount.Legends[0].Enabled = true;


        }

        private DataTable GetDataTable(string SQL)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = SQL;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            string fromDate = Request.Params["from"];
            string toDate = Request.Params["to"];
            id = int.Parse(Request.Params["id"]);
            reportTaskId = int.Parse(Request.Params["tid"]);
            //http://localhost:82/CCMSInt/GraphicalAnalysis/Reports/ParamPNCDepositReportingForCPM.aspx?from=05/05/2000&to=12/05/2014

            Label_FromDate.Text = fromDate;
            Label_ToDate.Text = toDate;

            DrawAcceptanceByCheckCountGraph(fromDate,toDate);
            DrawBusinessRuleRejectCausesGraph(fromDate, toDate);
            DrawNonBusinessRuleRejectCausesGraph(fromDate, toDate);
            DrawCheckRejectRateGraph(fromDate, toDate);
        }
    }


}