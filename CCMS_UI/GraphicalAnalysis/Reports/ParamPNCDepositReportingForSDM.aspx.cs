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
    public partial class ParamPNCDepositReportingForSDM : Page
    {
        string[] colorArr = { "#d8422D", "#ffba04", "#1d71ef", "#01A05b", "#d8422D" };

        int id = 1;
        int reportTaskId = 0;

        private void DrawCheckRejectRateGraph(string from, string to, int moduleType)
        {
            if (moduleType == 1)
            {

                Chart_RejectRate.Series[0].Points.Clear();
                Chart_RejectRate.Series[1].Points.Clear();
                Chart_RejectRate.Series[2].Points.Clear();


                string SQL = @"select  status,CONVERT(varchar,trxn_datetime,103) trxn_datetime 
                        from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                        inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +

                         " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                         " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + Filter();


                DataTable dt = GetDataTable(SQL);
                DataTable status = dt.DefaultView.ToTable(true, new string[] { "status" });
                DataTable trxnDatetime = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
                bool series1DataPointAdded = false;
                bool series2DataPointAdded = false;
                // bool series3DataPointAdded = false;
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
                    //if (series1DataPointVal > 0 &&  series2DataPointVal > 0)

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


                }


                Chart_RejectRate.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
                Chart_RejectRate.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;
                Chart_RejectRate.ResetAutoValues();


            }
            else if (moduleType == 0)
            {
                Chart_RejectRate.Series[0].Points.Clear();
                Chart_RejectRate.Series[1].Points.Clear();
                Chart_RejectRate.Series[2].Points.Clear();

                int counter = 0;

                string SQL = @"select  atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,ej_parsed_bna_transaction.ej_parsed_bna_transaction_Id
                                    from ej_parsed_bna_transaction inner join atm on ej_parsed_bna_transaction.atm_id = atm.atm_id
                                    inner join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    ej_parsed_bna_transaction.ej_parsed_bna_transaction_id " +

                                       " where atm.is_active=1 and atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                             " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + Filter();


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
                Chart_RejectRate.ResetAutoValues();

            }
            else if (moduleType == 2)
            {
                Hashtable hashtable = new Hashtable();
                Chart_RejectRate.Series[0].Points.Clear();
                Chart_RejectRate.Series[1].Points.Clear();
                Chart_RejectRate.Series[2].Points.Clear();


                string SQL = @"select  status,CONVERT(varchar,trxn_datetime,103) trxn_datetime 
                        from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                        inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +

                         " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                         " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + Filter();


                DataTable dt = GetDataTable(SQL);
                DataTable status = dt.DefaultView.ToTable(true, new string[] { "status" });
                DataTable trxnDatetime = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
                bool series1DataPointAdded = false;
                bool series2DataPointAdded = false;
                bool series3DataPointAdded = false;
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
                    series3DataPointAdded = false;
                    //if (series1DataPointVal > 0 &&  series2DataPointVal > 0)

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
                        hashtable.Add(xAxisLable, series1DataPointVal + "|" + series2DataPointVal + "|" + Math.Round(((decimal)series2DataPointVal / (series2DataPointVal + series1DataPointVal)) * 100, 2));
                        //Chart_RejectRate.Series[0].Points.AddXY(xAxisLable, series1DataPointVal);
                        //Chart_RejectRate.Series[1].Points.AddXY(xAxisLable, series2DataPointVal);
                        //Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round(((decimal)series2DataPointVal / (series2DataPointVal + series1DataPointVal)) * 100, 2));
                        counter++;
                        //series3DataPointAdded = true;
                    }


                }

                //BNA
                Chart_RejectRate.Series[0].Points.Clear();
                Chart_RejectRate.Series[1].Points.Clear();
                Chart_RejectRate.Series[2].Points.Clear();

                counter = 0;

                SQL = @"select  atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,ej_parsed_bna_transaction.ej_parsed_bna_transaction_Id
                                    from ej_parsed_bna_transaction inner join atm on ej_parsed_bna_transaction.atm_id = atm.atm_id
                                    inner join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    ej_parsed_bna_transaction.ej_parsed_bna_transaction_id " +

                                       " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                             " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + Filter();


                dt = GetDataTable(SQL);

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
                            xAxisLable = drInner["trxn_datetime"].ToString();
                            if (hashtable.Contains(xAxisLable))
                            {
                                string val = hashtable[xAxisLable].ToString();
                                string[] valParts = val.Split('|');
                                //Chart_RejectRate.Series[0].Points.AddXY(xAxisLable, series1DataPointVal);
                                //Chart_RejectRate.Series[1].Points.AddXY(xAxisLable, series2DataPointVal);
                                //Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round(((decimal)series2DataPointVal / (series2DataPointVal + series1DataPointVal)) * 100, 2));
                                Chart_RejectRate.Series[0].Points.AddXY(xAxisLable, depositCount + int.Parse(valParts[0]));
                                Chart_RejectRate.Series[1].Points.AddXY(xAxisLable, rej1 + rej2 + int.Parse(valParts[1]));
                                total = rej1 + rej2 + depositCount + int.Parse(valParts[0]) + int.Parse(valParts[1]);
                                if (total != 0)
                                    Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round((((decimal)rej1 + rej2 + int.Parse(valParts[1])) / total) * 100, 2));
                            }
                            else
                            {
                                Chart_RejectRate.Series[0].Points.AddXY(xAxisLable, depositCount);
                                Chart_RejectRate.Series[1].Points.AddXY(xAxisLable, rej1 + rej2);
                                total = rej1 + rej2 + depositCount;
                                if (total != 0)
                                    Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round((((decimal)rej1 + rej2) / total) * 100, 2));

                            }
                            counter++;
                        }
                    }


                }

                //


                Chart_RejectRate.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
                Chart_RejectRate.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;
                Chart_RejectRate.ResetAutoValues();

            }



        }
        private void DrawNonBusinessRuleRejectCausesGraph(string from, string to, int moduleType)
        {
            string comment = null;
            if (moduleType == 1)
            {
                string SQL = "select top 10 comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                           " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " +
                           " and dispute_status is null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment order by count(*) desc";
                DataTable dt = GetDataTable(SQL);

                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    //if (comment.Length > 30)
                    //    comment = comment.Substring(0, 30);

                    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);

                }
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
            else if (moduleType == 0)
            {
                string SQL = "select top 10 comment  ,count(*) total from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
                            " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                            " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " +
                            " and dispute_status is null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment order by count(*) desc";
                DataTable dt = GetDataTable(SQL);
                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    //if (comment.Length > 30)
                    //    comment = comment.Substring(0, 30);

                    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                }
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
            else if (moduleType == 2)
            {
                string SQL = "select top 10 comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                           " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " +
                           " and dispute_status is null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt = GetDataTable(SQL);

                //foreach (DataRow dr in dt.Rows)
                //{
                //    comment = dr[0].ToString();
                //    if (comment.Length > 30)
                //        comment = comment.Substring(0, 30);

                //    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                //}
                //
                SQL = "select comment ,count(*) total  from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
            " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
            " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " +
            " and dispute_status is null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt1 = GetDataTable(SQL);


                dt.Merge(dt1);
                dt.DefaultView.Sort = "total desc";
                DataView dataview = dt.DefaultView;
                int count = dt.Rows.Count > 10 ? 10 : dt.Rows.Count;

                for (int i = 0; i < count; i++)
                {
                    comment = dataview[i]["comment"].ToString();
                    //if (comment.Length > 30)
                    //    comment = comment.Substring(0, 30);
                    //
                    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dataview[i]["total"]);
                }

                for (int i = 0; i < Chart_NonBusinessRuleRejectCauses.Series["Default"].Points.Count; i++)
                    Chart_NonBusinessRuleRejectCauses.Series["Default"].Points[i].Color = System.Drawing.ColorTranslator.FromHtml(colorArr[i % 5]);

                //foreach (DataRow dr in dt1.Rows)
                //{
                //    comment = dr[0].ToString();
                //    if (comment.Length > 30)
                //        comment = comment.Substring(0, 30);

                //    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                //}
                //
                Chart_NonBusinessRuleRejectCauses.Titles[0].Text = "TOP 10 NON-BUSINESS RULE REJECT CAUSES";
                //Chart_NonBusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart_NonBusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
                //Chart_NonBusinessRuleRejectCauses.Legends[0].Enabled = true;

                Chart_NonBusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
                Chart_NonBusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
                Chart_NonBusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";

                //      dt.Merge(dt1);

            }
        }

        private void DrawBusinessRuleRejectCausesGraph(string from, string to, int moduleType)
        {
            string comment = null;
            if (moduleType == 1)
            {

                string SQL = "select top 10 ,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                    " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                    " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " +
                    " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment order by count(*) desc";
                DataTable dt = GetDataTable(SQL);

                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    //if (comment.Length > 30)
                    //    comment = comment.Substring(0, 30);

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
            else if (moduleType == 0)
            {

                string SQL = "select top 10 comment ,count(*) total from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
                    " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                    " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " +
                    " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment order by count(*) desc";
                DataTable dt = GetDataTable(SQL);

                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    //if (comment.Length > 30)
                    //    comment = comment.Substring(0, 30);


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
            else if (moduleType == 2)
            {
                string SQL = "select comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                 " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                 " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " +
                 " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt = GetDataTable(SQL);
                //foreach (DataRow dr in dt.Rows)
                //{
                //    comment = dr[0].ToString();
                //    if (comment.Length > 30)
                //        comment = comment.Substring(0, 30);

                //    Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                //}

                SQL = "select comment ,count(*) total  from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
                            " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                            " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " +
                            " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment ";
                DataTable dt1 = GetDataTable(SQL);
                dt.Merge(dt1);
                dt.DefaultView.Sort = "total desc";
                DataView dataview = dt.DefaultView;

                int count = dt.Rows.Count > 10 ? 10 : dt.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    comment = dataview[i]["comment"].ToString();
                    //if (comment.Length > 30)
                    //    comment = comment.Substring(0, 30);
                    Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dataview[i]["total"]);
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
        }
        private string Filter()
        {
            string filter = "";

            ReportSchedule reportSchedule = ReportSchedule.LoadReportScheduleByPk(id);
            ReportTask reportTask = ReportTask.LoadReportTaskByPk(reportTaskId);
            Literal_Title.Text = reportSchedule.ReportFriendlyName;
            Label_currentTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");


            if (reportSchedule.CriteriaId.HasValue)
            {
                if (reportSchedule.CriteriaId == 1)
                    filter += " and dispute_status is not null ";
                else if (reportSchedule.CriteriaId == 2)
                    filter += " and (dispute_status is null or dispute_status = '000') ";
            }

            //if (CheckBoxList_Module.Items[0].Selected)
            //    criteria += " and is_sdm = 1 ";
            //else if (CheckBoxList_Module.Items[1].Selected)
            //    criteria += " and is_cdm = 1 ";

            filter += " and is_sdm = 1 and is_eligible = 1";
            return filter;
        }

        private void DrawAcceptanceByCheckCountGraph(string from, string to, int moduleType)
        {

            if (moduleType == 2)
            {
                string SQL = @"select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                          inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                           " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + Filter();

                DataTable dt = GetDataTable(SQL);
                DataTable dtCPM = dt.DefaultView.ToTable(true, new string[] { "atm_id" });

                DataRow[] drBusinessRuleReject = dt.Select("dispute_status <>'000' and dispute_status is not null and status = 'Failed'");
                DataRow[] drNonBusinessRuleReject = dt.Select("dispute_status is null and status = 'Failed'");
                DataRow[] drAccepted = dt.Select("dispute_status ='000' and status = 'Successful'");

                int totalChecks = drBusinessRuleReject.Length + drNonBusinessRuleReject.Length + drAccepted.Length;
                int totalRejects = drBusinessRuleReject.Length + drNonBusinessRuleReject.Length;
                //if (totalChecks > 0)
                //    chequeAcceptance = Math.Round((decimal)drAccepted.Length / totalChecks * 100, 2);

                int notesAcceptedCount = 0;
                int notesRejectedCount = 0;

                SQL = @"select  ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,status,note_type,notes_count,atm.atm_id from ej_parsed_bna_transaction inner join ej_parsed_bna_transaction_detail 
                          on ej_parsed_bna_transaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                             " inner join atm on atm.atm_id=ej_parsed_bna_transaction.atm_id " +
                            " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + Filter();

                dt = GetDataTable(SQL);
                DataTable dtBNA = dt.DefaultView.ToTable(true, new string[] { "atm_id" });

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

                int totalNotes = notesAcceptedCount + notesRejectedCount;
                //if (totalNotes > 0)
                //    notesAcceptance = Math.Round((decimal)notesAcceptedCount / totalNotes * 100, 2);
                ////chequeAcceptance = Math.Round((decimal)drAccepted.Length / totalChecks * 100, 2);
                // (drAccepted.Length  + notesAcceptedCount)/(totalNotes+totalChecks)

                //Label_AccPerc.Text = ((chequeAcceptance + notesAcceptance) / 2).ToString();
                int total = totalNotes + totalChecks;
                if (total > 0)
                {
                    Label_AccPerc.Text = Math.Round((((decimal)drAccepted.Length + notesAcceptedCount) / total) * 100, 2).ToString();
                    Label_RejPerc.Text = (100 - decimal.Parse(Label_AccPerc.Text)).ToString();
                }
                Label_ItemsProcessed.Text = total.ToString(); //(notesRejectedCount + totalRejects + notesAcceptedCount + drAccepted.Length).ToString();
                Chart_AccCount.Series["Default"].Points.AddXY("TOTAL REJECTS", notesRejectedCount + totalRejects);
                Chart_AccCount.Series["Default"].Points.AddXY("ACCEPTED", notesAcceptedCount + drAccepted.Length);

                Chart_AccCount.Series["Default"].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#E0400A");
                Chart_AccCount.Series["Default"].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#32BE5F");

                Chart_AccCount.Series["Default"].ChartType = SeriesChartType.Pie;
                Chart_AccCount.Series["Default"]["PieLabelStyle"] = "Outside";
                Chart_AccCount.Titles[0].Text = "ACCEPTANCE";
                Chart_AccCount.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart_AccCount.Series[0]["PieDrawingStyle"] = "SoftEdge";
                Chart_AccCount.Legends[0].Enabled = true;

                int totalUnits = dtCPM.Rows.Count;
                foreach (DataRow dr in dtBNA.Rows)
                {
                    if (dtCPM.Select("atm_id=" + dr["atm_id"]).Length == 0)
                        totalUnits++;
                }
                Label_totalATMs.Text = totalUnits.ToString();
            }
            else if (moduleType == 1)
            {
                string SQL = @"select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                          inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                           " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + Filter();

                DataTable dt = GetDataTable(SQL);

                DataRow[] drBusinessRuleReject = dt.Select("dispute_status <>'000' and dispute_status is not null and status = 'Failed'");
                DataRow[] drNonBusinessRuleReject = dt.Select("dispute_status is null and status = 'Failed'");
                DataRow[] drAccepted = dt.Select("dispute_status ='000' and status = 'Successful'");

                int totalChecks = drBusinessRuleReject.Length + drNonBusinessRuleReject.Length + drAccepted.Length;
                if (totalChecks > 0)
                    Label_AccPerc.Text = Math.Round((decimal)drAccepted.Length / totalChecks * 100, 2).ToString();
                else
                    Label_AccPerc.Text = "0";
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
            else if (moduleType == 0)
            {
                int notesAcceptedCount = 0;
                int notesRejectedCount = 0;

                string SQL = @"select  ej_parsed_bna_transaction.ej_parsed_bna_transaction_id,status,note_type,notes_count from ej_parsed_bna_transaction inner join ej_parsed_bna_transaction_detail 
                          on ej_parsed_bna_transaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                             " inner join atm on atm.atm_id=ej_parsed_bna_transaction.atm_id " +
                            " where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + Filter();

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
                    Label_AccPerc.Text = Math.Round((decimal)notesAcceptedCount / totalNotes * 100, 2).ToString();
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
            string from = Request.Params["from"];
            string to = Request.Params["to"];
            id = int.Parse(Request.Params["id"]);
            reportTaskId = int.Parse(Request.Params["tid"]);
            //http://localhost:82/CCMSInt/GraphicalAnalysis/Reports/ParamPNCDepositReportingForCPM.aspx?from=05/05/2000&to=12/05/2014
            Label_FromDate.Text = from;
            Label_ToDate.Text = to;

            int moduleType = 2;
            DrawAcceptanceByCheckCountGraph(from, to, moduleType);
            DrawBusinessRuleRejectCausesGraph(from, to, moduleType);
            DrawNonBusinessRuleRejectCausesGraph(from, to, moduleType);
            DrawCheckRejectRateGraph(from, to, moduleType);


        }
    }


}