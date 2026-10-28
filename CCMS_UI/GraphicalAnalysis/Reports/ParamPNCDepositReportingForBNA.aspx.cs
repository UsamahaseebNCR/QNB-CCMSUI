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
    public partial class ParamPNCDepositReportingForBNA : Page
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
            filter += " and is_eligible = 1";
            //if (CheckBoxList_ErrorAnalysisType.Items[0].Selected && CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            //    criteria = "";
            //else if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
            //    criteria = " and dispute_status is not null ";
            //else if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            //    criteria = " and dispute_status is null ";
            return filter;
        }

        private void DrawNoteRejectReasonsGraph(string fromDate, string toDate)
        {
            //string SQL = "select comment,count(*) from ej_parsed_bna_transaction inner join atm on atm.atm_id =ej_parsed_bna_transaction.atm_id  " +
            //           " where is_cdm = 1 and atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
            //           " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " + Filter() +
            //           " and comment is not null and len(comment)>1 group by comment";

            string comment = null;
            string SQL = "select top 10 comment,count(*) total from ej_parsed_bna_transaction inner join atm on atm.atm_id=ej_parsed_bna_transaction.atm_id " +
                       " where status = 'Failed' and atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + toDate+ " 23:59:59',103) " + Filter() +
                       " and comment is not null and len(comment)>1 group by comment order by count(*) desc";

            DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
            {
                comment = dr[0].ToString();
                //if (comment.Length > 30)
                //    comment = comment.Substring(0, 30);

                Chart_NoteRejectReasons.Series[0].Points.AddXY(comment, dr[1]);
            }
       
            for (int i = 0; i < Chart_NoteRejectReasons.Series["Default"].Points.Count; i++)
                Chart_NoteRejectReasons.Series["Default"].Points[i].Color = System.Drawing.ColorTranslator.FromHtml(colorArr[i % 5]);


            Chart_NoteRejectReasons.Titles[0].Text = "TOP 10 NOTE REJECT REASONS";
            //Chart_NoteRejectReasons.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart_NoteRejectReasons.Series[0]["PieDrawingStyle"] = "SoftEdge";
            //Chart_NoteRejectReasons.Legends[0].Enabled = true;

            Chart_NoteRejectReasons.Series["Default"]["ShowMarkerLines"] = "true";
            Chart_NoteRejectReasons.Series["Default"]["DrawingStyle"] = "Cylinder";
            Chart_NoteRejectReasons.Series["Default"]["PointWidth"] = "0.8";


        }
        private void DrawRejectRateGraph(string fromDate, string toDate)
        {
            int counter = 0;
            Chart_RejectRate.Series[0].Points.Clear();
            Chart_RejectRate.Series[1].Points.Clear();
            Chart_RejectRate.Series[2].Points.Clear();

            //string SQL = "select top 7 status,CONVERT(varchar,trxn_datetime,103) trxn_datetime,COUNT(*) from ej_parsed_bna_transaction " +
            //    " inner join atm on atm.atm_id =ej_parsed_bna_transaction.atm_id  where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
            //    " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " +
            //    " group by status,CONVERT(varchar,trxn_datetime,103)  " +
            //    " order by convert(Datetime,CONVERT(varchar,trxn_datetime,103),103)";

            string SQL = @"select atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,ej_parsed_bna_transaction.ej_parsed_bna_transaction_Id
                                    from ej_parsed_bna_transaction inner join atm on ej_parsed_bna_transaction.atm_id = atm.atm_id
                                    inner join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    ej_parsed_bna_transaction.ej_parsed_bna_transaction_id
                                     where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                        " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " + Filter();



            
            DataTable dt = GetDataTable(SQL);

            DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id", "title" });
            DataTable dtATMsTrxnDatetime = dt.DefaultView.ToTable(true, new string[] { "atm_id", "trxn_datetime" });
            int depositCount = 0;
            int rej1 = 0;
            int rej2 = 0;
            int total = 0;
            foreach (DataRow dr in dtATMs.Rows)
            {
                if (counter == 7) break;
                DataRow[] drTrxnDates = dtATMsTrxnDatetime.Select("atm_id = " + dr["atm_id"]);

                foreach (DataRow drInner in drTrxnDates)
                {

                    if (counter == 7) break;
                    rej1 = 0;
                    rej2 = 0;
                    total = 0;
                    depositCount = 0;

                    DataRow[] drAllTrxnForTheDay = dt.Select("atm_id = " + dr["atm_id"] + " and trxn_datetime='" + drInner["trxn_datetime"] + "' and status = 'Successful'");
                    if (drAllTrxnForTheDay.Length > 0)
                    {

                        DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                        foreach (DataRow drRow in dtBNATrxnIds.Rows)
                        {


                            DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                            if (dr1.Length > 0)
                            {
                                DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                {
                                    DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");

                                    depositCount += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                                }

                            }
                        }
                    }



                    drAllTrxnForTheDay = dt.Select("atm_id = " + dr["atm_id"] + " and trxn_datetime='" + drInner["trxn_datetime"] + "' and status = 'Failed' and dispute_status is not null");
                    if (drAllTrxnForTheDay.Length > 0)
                    {
                        DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                        foreach (DataRow drRow in dtBNATrxnIds.Rows)
                        {

                            DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                            if (dr1.Length > 0)
                            {
                                DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                {
                                    DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                                    rej1 += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                                }

                            }
                        }
                    }




                    drAllTrxnForTheDay = dt.Select("atm_id = " + dr["atm_id"] + " and trxn_datetime='" + drInner["trxn_datetime"] + "' and status = 'Failed' and dispute_status is null");
                    if (drAllTrxnForTheDay.Length > 0)
                    {
                        DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                        foreach (DataRow drRow in dtBNATrxnIds.Rows)
                        {

                            DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                            if (dr1.Length > 0)
                            {
                                DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                {
                                    DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                                    rej2 += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                                }

                            }
                        }


                    }


                    if (depositCount > 0 && (rej1 + rej2) > 0)
                    {

                        string xAxisLable = drInner["trxn_datetime"].ToString();
                        Chart_RejectRate.Series[0].Points.AddXY(xAxisLable, depositCount);
                        Chart_RejectRate.Series[1].Points.AddXY(xAxisLable, rej1 + rej2);
                        total = rej1 + rej2 + depositCount;
                        if (total != 0)
                            Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round((((decimal)rej1 + rej2) / total) * 100, 2));
                        counter++;
                    }
                }


            }





            Chart_RejectRate.Titles[0].Text = "PNC NOTE REJECT RATE";
            Chart_RejectRate.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            //Chart_RejectRate.ChartAreas["ChartArea1"].Area3DStyle.  = false;
            //Chart_RejectRate.Legends[0].Enabled = false;
            Chart_RejectRate.ResetAutoValues();







        }



        private void DrawAcceptanceByNoteCountGraph(string fromDate, string toDate)
        {
            int notesAcceptedCount = 0;
            int notesRejectedCount = 0;
            //            string SQL = @"select  status,isnull(sum(notes_count),0) from ej_parsed_bna_transaction inner join ej_parsed_bna_transaction_detail 
            //            on ej_parsed_bna_transaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
            //                       "inner join atm on atm.atm_id =ej_parsed_bna_transaction.atm_id  where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
            //                       " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " + Filter();


            string SQL = @"select  ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,status,note_type,notes_count,atm.atm_id from ej_parsed_bna_transaction inner join ej_parsed_bna_transaction_detail 
                          on ej_parsed_bna_transaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                         "inner join atm on atm.atm_id =ej_parsed_bna_transaction.atm_id  where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                        " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) " + Filter();


            
            DataTable dt = GetDataTable(SQL);
            Label_totalATMs.Text = dt.DefaultView.ToTable(true, new string[] { "atm_id" }).Rows.Count.ToString();
            DataTable dtBNATrxnIds = dt.DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });
            foreach (DataRow drRow in dtBNATrxnIds.Rows)
            {
                DataRow[] dr = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                if (dr.Length > 0)
                {
                    DataTable dtUniqueNoteType = dr.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                    foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                    {
                        DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                        if (temp[0]["status"].ToString() == "Successful")
                            notesAcceptedCount += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                        else
                            notesRejectedCount += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());

                    }

                }
            }

            //DataRow[] drRejected = dt.Select(" status = 'Failed'");
            //DataRow[] drAccepted = dt.Select(" status = 'Successful'");

            //if (drAccepted.Length>0)
            //    notesAcceptedCount = int.Parse(drAccepted[0][1].ToString());

            //if (drRejected.Length>0)
            //    notesRejectedCount = int.Parse(drRejected[0][1].ToString());

            int totalNotes = notesAcceptedCount + notesRejectedCount;
            if (totalNotes > 0)
                Label_AccPerc.Text = Math.Round((decimal)notesAcceptedCount / totalNotes * 100,2).ToString();
            else
                Label_AccPerc.Text = "0";
            Label_RejPerc.Text = (100 - decimal.Parse(Label_AccPerc.Text)).ToString();
            Label_ItemsProcessed.Text = totalNotes.ToString();
                
            Chart_AccCount.Series["Default"].Points.AddXY("TOTAL REJECTS", notesRejectedCount);
            Chart_AccCount.Series["Default"].Points.AddXY("NOTES-ACCEPTED", notesAcceptedCount);

            Chart_AccCount.Series["Default"].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#E0400A");
            Chart_AccCount.Series["Default"].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#32BE5F");

            Chart_AccCount.Series["Default"].ChartType = SeriesChartType.Pie;
            Chart_AccCount.Series["Default"]["PieLabelStyle"] = "Outside";
            Chart_AccCount.Titles[0].Text = "ACCEPTANCE BY NOTE COUNT";
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
            //http://localhost:82/CCMSInt/GraphicalAnalysis/Reports/ParamPNCDepositReportingForBNA.aspx?from=05/05/2000&to=12/05/2014
            Label_FromDate.Text = fromDate;
            Label_ToDate.Text = toDate;


            DrawAcceptanceByNoteCountGraph(fromDate, toDate);
            DrawNoteRejectReasonsGraph(fromDate, toDate);
            DrawRejectRateGraph(fromDate, toDate);

        }

       
    }


}