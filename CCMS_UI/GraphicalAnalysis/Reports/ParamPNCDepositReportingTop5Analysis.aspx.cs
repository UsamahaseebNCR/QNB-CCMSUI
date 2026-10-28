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
    public partial class ParamPNCDepositReportingTop5Analysis : Page
    {
        int id = 1;
        int reportTaskId = 0;
        private string Filter()
        {
            string filter = "";

            ReportSchedule reportSchedule = ReportSchedule.LoadReportScheduleByPk(id);
            ReportTask reportTask = ReportTask.LoadReportTaskByPk(reportTaskId);
            Literal_Title.Text = reportSchedule.ReportFriendlyName;
            Label_currentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Label_scheduleDate.Text = reportTask.ScheduleDate.ToString("dd/MM/yyyy");
            if (reportSchedule.ReportName.ToLower().Contains("sdm"))
                filter += " and is_sdm = 1 ";
            else
                filter += " and is_cdm = 1 ";
            //if (DropDownList_Comment.SelectedIndex>0)
            //    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";
            if (reportSchedule.CriteriaId.HasValue)
            {
                if (reportSchedule.CriteriaId == 1)
                    filter += " and dispute_status is not null ";
                else if (reportSchedule.CriteriaId == 2)
                    filter += " and (dispute_status is null or dispute_status = '000') ";
            }
            filter += " and is_eligible = 1 and status = 'Failed'";
            //if (CheckBoxList_ErrorAnalysisType.Items[0].Selected && CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            //    criteria = "";
            //else if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
            //    criteria = " and dispute_status is not null ";
            //else if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            //    criteria = " and dispute_status is null ";
            return filter;
        }

        private void DrawCashAcceptorGraph(string fromDate,string toDate)
        {
            string SQL = "select top 5 comment,count(*) from ej_parsed_bna_transaction inner join atm on atm.atm_id =ej_parsed_bna_transaction.atm_id " +
                       " where is_cdm=1 and atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " + Filter() +
                       " and comment is not null and len(comment)>1 and is_eligible=1  group by comment order by count(*) desc";
            DataTable dt = GetDataTable(SQL);

            //string SQL = "select top 5 comment,count(*) from ej_parsed_bna_transaction inner join atm on atm.atm_id = ej_parsed_bna_transaction.atm_id" +
            //           " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
            //           " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " +
            //           " and comment is not null and len(comment)>1 group by comment";
            //DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
                Chart_Top5CashAcceptorIncident.Series[0].Points.AddXY(dr[0], dr[1]);

            Chart_Top5CashAcceptorIncident.Titles[0].Text = "TOP 5 INCIDENTS RELATED TO CASH ACCEPTOR";
            Chart_Top5CashAcceptorIncident.Series[0]["PieDrawingStyle"] = "SoftEdge";


            Chart_Top5CashAcceptorIncident.Series["Default"]["ShowMarkerLines"] = "true";
            Chart_Top5CashAcceptorIncident.Series["Default"]["DrawingStyle"] = "Cylinder";
            Chart_Top5CashAcceptorIncident.Series["Default"]["PointWidth"] = "0.8";


        }
        private void DrawCheckAcceptorGraph(string fromDate, string toDate)
        {
            string SQL = "select top 5 comment,count(*) from ej_parsed_cpm_transaction inner join atm on atm.atm_id =ej_parsed_cpm_transaction.atm_id" +
                         " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)   " +
                         " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " + Filter() +
                         " and comment is not null and len(comment)>1 and is_eligible=1  group by comment order by count(*) desc";
          

            //string SQL = "select top 5 comment,count(*) from ej_parsed_cpm_transaction inner join atm on atm.atm_id = ej_parsed_cpm_transaction.atm_id" +
            //           " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate+ "',103)  " +
            //           " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " +
            //           " and comment is not null and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
                Chart_Top5CheckAcceptorIncident.Series[0].Points.AddXY(dr[0], dr[1]);

            Chart_Top5CheckAcceptorIncident.Titles[0].Text = "TOP 5 INCIDENTS RELATED TO CHECK ACCEPTOR";
            Chart_Top5CheckAcceptorIncident.Series[0]["PieDrawingStyle"] = "SoftEdge";

            Chart_Top5CheckAcceptorIncident.Series["Default"]["ShowMarkerLines"] = "true";
            Chart_Top5CheckAcceptorIncident.Series["Default"]["DrawingStyle"] = "Cylinder";
            Chart_Top5CheckAcceptorIncident.Series["Default"]["PointWidth"] = "0.8";

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
            //http://localhost:82/CCMSInt/GraphicalAnalysis/Reports/ParamPNCDepositReportingTop5Analysis.aspx?from=05/05/2000&to=12/05/2014

            DrawCashAcceptorGraph(fromDate, toDate);
            DrawCheckAcceptorGraph(fromDate, toDate);
 
        }
    }


}