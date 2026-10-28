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
    public partial class PNCDepositReportingForBNA : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            //string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            //if (regionID != null /*&& !regionID.Contains(",")*/
            //    && regionID != "1")
            //{
            //    DropDownList_NoteSetType.Items.Clear();
            //    Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
            //    Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
            //    while (reader.Read())
            //    {
            //        DropDownList_NoteSetType.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
            //    }
            //    reader.Close();

            //}
            //else
            //    Literal_Message.Text = "<span style='color:red'><b>Please select only one organization to do denomination analysis</b></span>";

        }



        private string Filter()
        {
            string criteria = null;

            if (CheckBoxList_ErrorAnalysisType.Items[0].Selected && CheckBoxList_ErrorAnalysisType.Items[1].Selected)
                criteria = "";
            else if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
                criteria = " and dispute_status is not null ";
            else if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
                criteria = " and (dispute_status is null or dispute_status = '000') ";

            if (CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
            }
            else if (CheckBoxList_Module.Items[0].Selected)
                criteria += " and is_sdm = 1 ";
            else if (CheckBoxList_Module.Items[1].Selected)
                criteria += " and is_cdm = 1 ";


            criteria += "  and is_eligible = 1";
            return criteria;
        }
        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            DrawAcceptanceByNoteCountGraph();
            DrawNoteRejectReasonsGraph();
            DrawRejectRateGraph();
            //DrawBusinessRuleRejectCausesGraph();
            //DrawNonBusinessRuleRejectCausesGraph();
            //DrawCheckRejectRateGraph();
        }

        private void DrawNoteRejectReasonsGraph()
        {
            string comment = null;
            string SQL = "select comment,count(*) total from vEjParsedBNATransaction " +
                " inner join atm on atm.atm_id=vEjParsedBNATransaction.atm_id " +
                       " where status = 'Failed' and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter() +
                       " and comment is not null and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
            {
                comment = dr[0].ToString();
                if (comment.Length > 30)
                    comment = comment.Substring(0, 30);

                Chart_NoteRejectReasons.Series[0].Points.AddXY(comment, dr[1]);
            }
            Chart_NoteRejectReasons.Titles[0].Text = "NOTE REJECT REASONS";
            //Chart_NoteRejectReasons.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart_NoteRejectReasons.Series[0]["PieDrawingStyle"] = "SoftEdge";
            //Chart_NoteRejectReasons.Legends[0].Enabled = true;

            Chart_NoteRejectReasons.Series["Default"]["ShowMarkerLines"] = "true";
            Chart_NoteRejectReasons.Series["Default"]["DrawingStyle"] = "Cylinder";
            Chart_NoteRejectReasons.Series["Default"]["PointWidth"] = "0.8";

            GridView_RuleRejectCauses.DataSource = dt;
            GridView_RuleRejectCauses.DataBind();

        }
        private void DrawRejectRateGraph()
        {

            Chart_RejectRate.Series[0].Points.Clear();
            Chart_RejectRate.Series[1].Points.Clear();
            Chart_RejectRate.Series[2].Points.Clear();

            int counter = 0;

            string SQL = @"select  atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,vEjParsedBNATransaction.ej_parsed_bna_transaction_Id
                                    from vEjParsedBNATransaction inner join atm on vEjParsedBNATransaction.atm_id = atm.atm_id
                                    inner join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    vEjParsedBNATransaction.ej_parsed_bna_transaction_id " + 
                                 
                                   " where atm.is_active=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_From.Text + "',103)  " +
                         " and trxn_datetime<=convert(datetime,'" + TextBox_To.Text + " 23:59:59',103) " + Filter();


            //string SQL = "select atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,dispute_status,status from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
            //             " inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id  where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
            //             " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) ";

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






            //string SQL = "select top 7 status,CONVERT(varchar,trxn_datetime,103) trxn_datetime,COUNT(*) from ej_parsed_bna_transaction " +
            //    " where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
            //    " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter() + 
            //    " group by status,CONVERT(varchar,trxn_datetime,103)  " +
            //    " order by convert(Datetime,CONVERT(varchar,trxn_datetime,103),103)";

            //DataTable dt = GetDataTable(SQL);

            //DataRow[] drReject = dt.Select(" status = 'Failed'");
            //DataRow[] drSuccess = dt.Select(" status = 'Sucessful'");
            //DataRow[] drNonBusinessRuleReject = dt.Select("dispute_status is null and status = 'Failed'");
            //DataRow[] drAccepted = dt.Select("dispute_status ='000' and status = 'Successful'");





            //DataTable status = dt.DefaultView.ToTable(true, new string[] { "status" });
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
            //DataTable trxnDatetime = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
            //bool series1DataPointAdded = false;
            //bool series2DataPointAdded = false;
            //bool series3DataPointAdded = false;
            //int series1DataPointVal = 0;
            //int series2DataPointVal = 0;
            //string xAxisLable = "";
            //foreach (DataRow dr in trxnDatetime.Rows)
            //{
            //    series1DataPointAdded = false;
            //    series2DataPointAdded = false;
            //    series3DataPointAdded = false;
            //    series1DataPointVal = 0;
            //    series2DataPointVal = 0;

            //    DataRow[] drArray = dt.Select("trxn_datetime='" + dr[0] + "'", "status");
            //    for (int j = 0; j < drArray.Length; j++)
            //    {
            //        if (drArray[j][0].ToString() == "Successful")
            //        {
            //            xAxisLable = dr[0].ToString();
            //            Chart_RejectRate.Series[0].Points.AddXY(dr[0], drArray[j][2]);
            //            series1DataPointAdded = true;
            //            series1DataPointVal = int.Parse(drArray[j][2].ToString());
            //        }

            //        if (drArray[j][0].ToString() == "Failed")
            //        {
            //            Chart_RejectRate.Series[1].Points.AddXY(dr[0], drArray[j][2]);
            //            series2DataPointAdded = true;
            //            series2DataPointVal = int.Parse(drArray[j][2].ToString());
            //        }
            //        if (series1DataPointAdded && series2DataPointAdded)
            //        {
            //            Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round(((decimal)series2DataPointVal / (series2DataPointVal + series1DataPointVal)) * 100));
            //            series3DataPointAdded = true;
            //        }

            //    }

            //    if (!series1DataPointAdded)
            //        Chart_RejectRate.Series[0].Points.AddXY(dr[0], 0);
            //    if (!series2DataPointAdded)
            //        Chart_RejectRate.Series[1].Points.AddXY(dr[0], 0);
            //    if (!series3DataPointAdded)
            //        Chart_RejectRate.Series[2].Points.AddXY(dr[0], 0);
            //}

            //for (int j = 0; j < ChartKPISummary.Series.Count; j++)
            //{
            //    ChartKPISummary.Series[j]["StackedGroupName"] = "Group1";
            //    ChartKPISummary.Series[j].IsValueShownAsLabel = true;


            //}
            Chart_RejectRate.Titles[0].Text = "PNC NOTE REJECT RATE";
            Chart_RejectRate.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            //Chart_RejectRate.ChartAreas["ChartArea1"].Area3DStyle.  = false;
            //Chart_RejectRate.Legends[0].Enabled = false;
            Chart_RejectRate.ResetAutoValues();







        }



        private void DrawAcceptanceByNoteCountGraph()
        {
            int notesAcceptedCount = 0;
            int notesRejectedCount = 0;



            string SQL = @"select  vEjParsedBNATransaction.ej_parsed_bna_transaction_id,status,note_type,notes_count from vEjParsedBNATransaction inner join ej_parsed_bna_transaction_detail 
                          on vEjParsedBNATransaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                         " inner join atm on atm.atm_id=vEjParsedBNATransaction.atm_id " +
                        " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter();


            //            string SQL = @"select  status,isnull(sum(notes_count),0) from ej_parsed_bna_transaction inner join ej_parsed_bna_transaction_detail 
            //            on ej_parsed_bna_transaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
            //                       " where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
            //                       " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) group by status";

            DataTable dt = GetDataTable(SQL);

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
            if (!base.IsPostBack)
            {
                this.TextBox_To.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.TextBox_From.Text = DateTime.Now.AddDays(-7.0).ToString("dd/MM/yyyy");
                Button1.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(Button1, null) + ";");
            }
        }
    }


}