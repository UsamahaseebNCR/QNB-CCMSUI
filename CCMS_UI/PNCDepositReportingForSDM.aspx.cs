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
    public partial class PNCDepositReportingForSDM : BasePage
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

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            DrawAcceptanceByCheckCountGraph();
            DrawBusinessRuleRejectCausesGraph();
            DrawNonBusinessRuleRejectCausesGraph();
            DrawCheckRejectRateGraph();
        }

        private void DrawCheckRejectRateGraph()
        {
            if (CheckBoxList_Module.Items[0].Selected && !CheckBoxList_Module.Items[1].Selected)
            {

                Chart_RejectRate.Series[0].Points.Clear();
                Chart_RejectRate.Series[1].Points.Clear();
                Chart_RejectRate.Series[2].Points.Clear();


                string SQL = @"select  status,CONVERT(varchar,trxn_datetime,103) trxn_datetime 
                        from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                        inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +

                         " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                         " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter();


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
            else if (!CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
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
            else if (CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
                Hashtable hashtable = new Hashtable();
                Chart_RejectRate.Series[0].Points.Clear();
                Chart_RejectRate.Series[1].Points.Clear();
                Chart_RejectRate.Series[2].Points.Clear();


                string SQL = @"select  status,CONVERT(varchar,trxn_datetime,103) trxn_datetime 
                        from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                        inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +

                         " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                         " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter();


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

                SQL = @"select  atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,vEjParsedBNATransaction.ej_parsed_bna_transaction_Id
                                    from vEjParsedBNATransaction inner join atm on vEjParsedBNATransaction.atm_id = atm.atm_id
                                    inner join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    vEjParsedBNATransaction.ej_parsed_bna_transaction_id " +

                                       " where atm.is_active=1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_From.Text + "',103)  " +
                             " and trxn_datetime<=convert(datetime,'" + TextBox_To.Text + " 23:59:59',103) " + Filter();


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
                                Chart_RejectRate.Series[0].Points.AddXY(xAxisLable, depositCount+int.Parse(valParts[0]));
                                Chart_RejectRate.Series[1].Points.AddXY(xAxisLable, rej1 + rej2 + int.Parse(valParts[1]));
                                total = rej1 + rej2 + depositCount+int.Parse(valParts[0])+int.Parse(valParts[1]);
                                if (total != 0)
                                    Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round((((decimal)rej1 + rej2+int.Parse(valParts[1])) / total) * 100, 2));
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
        private void DrawNonBusinessRuleRejectCausesGraph()
        {
            string comment = null;
            if (CheckBoxList_Module.Items[0].Selected && !CheckBoxList_Module.Items[1].Selected)
            {
                string SQL = "select comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                           " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                           " and dispute_status is null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt = GetDataTable(SQL);

                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    if (comment.Length > 30)
                        comment = comment.Substring(0, 30);

                    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);

                }
                Chart_NonBusinessRuleRejectCauses.Titles[0].Text = "NON-BUSINESS RULE REJECT CAUSES";
                //Chart_NonBusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart_NonBusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
                //Chart_NonBusinessRuleRejectCauses.Legends[0].Enabled = true;

                Chart_NonBusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
                Chart_NonBusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
                Chart_NonBusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";

                GridView_NonBusinessRuleRejectCauses.DataSource = dt;
                GridView_NonBusinessRuleRejectCauses.DataBind();
            }
            else if (!CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
                string SQL = "select comment  ,count(*) total from vEjParsedBNATransaction inner join atm on  vEjParsedBNATransaction.atm_id = atm.atm_id" +
                            " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                            " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                            " and dispute_status is null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt = GetDataTable(SQL);
                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    if (comment.Length > 30)
                        comment = comment.Substring(0, 30);

                    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                }


                Chart_NonBusinessRuleRejectCauses.Titles[0].Text = "NON-BUSINESS RULE REJECT CAUSES";
                //Chart_NonBusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart_NonBusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
                //Chart_NonBusinessRuleRejectCauses.Legends[0].Enabled = true;

                Chart_NonBusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
                Chart_NonBusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
                Chart_NonBusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";
                GridView_NonBusinessRuleRejectCauses.DataSource = dt;
                GridView_NonBusinessRuleRejectCauses.DataBind();

            }
            else if (CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
                string SQL = "select comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                           " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                           " and dispute_status is null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt = GetDataTable(SQL);

                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    if (comment.Length > 30)
                        comment = comment.Substring(0, 30);

                    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                }
                //
                SQL = "select comment ,count(*) total  from vEjParsedBNATransaction inner join atm on  vEjParsedBNATransaction.atm_id = atm.atm_id" +
            " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
            " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
            " and dispute_status is null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt1 = GetDataTable(SQL);
                foreach (DataRow dr in dt1.Rows)
                {
                    comment = dr[0].ToString();
                    if (comment.Length > 30)
                        comment = comment.Substring(0, 30);

                    Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                }
                //
                Chart_NonBusinessRuleRejectCauses.Titles[0].Text = "NON-BUSINESS RULE REJECT CAUSES";
                //Chart_NonBusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart_NonBusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
                //Chart_NonBusinessRuleRejectCauses.Legends[0].Enabled = true;

                Chart_NonBusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
                Chart_NonBusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
                Chart_NonBusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";

                dt.Merge(dt1);
                GridView_NonBusinessRuleRejectCauses.DataSource = dt;
                GridView_NonBusinessRuleRejectCauses.DataBind();

            }
        }

        private void DrawBusinessRuleRejectCausesGraph()
        {
            string comment = null;
            if (CheckBoxList_Module.Items[0].Selected && !CheckBoxList_Module.Items[1].Selected)
            {
                
                string SQL = "select comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                    " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                    " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                    " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt = GetDataTable(SQL);

                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    if (comment.Length > 30)
                        comment = comment.Substring(0, 30);

                    Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                }
                Chart_BusinessRuleRejectCauses.Titles[0].Text = "BUSINESS RULE REJECT CAUSES";
                //Chart_BusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart_BusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
                //Chart_BusinessRuleRejectCauses.Legends[0].Enabled = true;

                Chart_BusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
                Chart_BusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
                Chart_BusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";

                GridView_BusinessRuleRejectCauses.DataSource = dt;
                GridView_BusinessRuleRejectCauses.DataBind();
            }
            else if (!CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {

                string SQL = "select comment ,count(*) total from vEjParsedBNATransaction inner join atm on  vEjParsedBNATransaction.atm_id = atm.atm_id" +
                    " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                    " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                    " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt = GetDataTable(SQL);

                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    if (comment.Length > 30)
                        comment = comment.Substring(0, 30);


                    Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                }
                Chart_BusinessRuleRejectCauses.Titles[0].Text = "BUSINESS RULE REJECT CAUSES";
                //Chart_BusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart_BusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
                //Chart_BusinessRuleRejectCauses.Legends[0].Enabled = true;

                Chart_BusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
                Chart_BusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
                Chart_BusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";

                GridView_BusinessRuleRejectCauses.DataSource = dt;
                GridView_BusinessRuleRejectCauses.DataBind();
      
            }
            else if (CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
                string SQL = "select comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                 " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                 " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                 " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt = GetDataTable(SQL);
                foreach (DataRow dr in dt.Rows)
                {
                    comment = dr[0].ToString();
                    if (comment.Length > 30)
                        comment = comment.Substring(0, 30);
       
                    Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                }

                SQL = "select comment ,count(*) total  from vEjParsedBNATransaction inner join atm on  vEjParsedBNATransaction.atm_id = atm.atm_id" +
                            " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                            " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                            " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + Filter() + " and len(comment)>1 group by comment";
                DataTable dt1 = GetDataTable(SQL);

                foreach (DataRow dr in dt1.Rows)
                {
                    comment = dr[0].ToString();
                    if (comment.Length > 30)
                        comment = comment.Substring(0, 30);
                    Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment , dr[1]);
                }
                Chart_BusinessRuleRejectCauses.Titles[0].Text = "BUSINESS RULE REJECT CAUSES";
                //Chart_BusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart_BusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
                //Chart_BusinessRuleRejectCauses.Legends[0].Enabled = true;

                Chart_BusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
                Chart_BusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
                Chart_BusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";
                dt.Merge(dt1);
                GridView_BusinessRuleRejectCauses.DataSource = dt;
                GridView_BusinessRuleRejectCauses.DataBind();
      
            }
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

            //if (CheckBoxList_Module.Items[0].Selected)
            //    criteria += " and is_sdm = 1 ";
            //else if (CheckBoxList_Module.Items[1].Selected)
            //    criteria += " and is_cdm = 1 ";

            criteria += " and is_sdm = 1 and is_eligible = 1";
            return criteria;
        }

        private void DrawAcceptanceByCheckCountGraph()
        {

            if (CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
                decimal chequeAcceptance = 0;
                decimal notesAcceptance = 0;
                string SQL = @"select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                          inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                           " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter();

                DataTable dt = GetDataTable(SQL);
                LogableTask.LogMonoActivityTask("RowCount", MethodBase.GetCurrentMethod(), TraceLevel.Info, "Row count from first query: "+dt.Rows.Count);
                DataRow[] drBusinessRuleReject = dt.Select("dispute_status <>'000' and dispute_status is not null and status = 'Failed'");
                DataRow[] drNonBusinessRuleReject = dt.Select("dispute_status is null and status = 'Failed'");
                DataRow[] drAccepted = dt.Select("dispute_status ='000' and status = 'Successful'");
                LogableTask.LogMonoActivityTask("RowCount", MethodBase.GetCurrentMethod(), TraceLevel.Info, "Total cheques acc[successful]:" + drAccepted.Length);
                

                int totalChecks = drBusinessRuleReject.Length + drNonBusinessRuleReject.Length + drAccepted.Length;
                LogableTask.LogMonoActivityTask("RowCount", MethodBase.GetCurrentMethod(), TraceLevel.Info, "Total cheques processed (br + nbr + acc)" + totalChecks);

                int totalRejects = drBusinessRuleReject.Length + drNonBusinessRuleReject.Length;

                //if (totalChecks > 0)
                //    chequeAcceptance = Math.Round((decimal)drAccepted.Length / totalChecks * 100, 2);

                int notesAcceptedCount = 0;
                int notesRejectedCount = 0;

                SQL = @"select  vEjParsedBNATransaction.ej_parsed_bna_transaction_id,status,note_type,notes_count from vEjParsedBNATransaction inner join ej_parsed_bna_transaction_detail 
                          on vEjParsedBNATransaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                             " inner join atm on atm.atm_id=vEjParsedBNATransaction.atm_id " +
                            " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter();

                dt = GetDataTable(SQL);
                LogableTask.LogMonoActivityTask("RowCount", MethodBase.GetCurrentMethod(), TraceLevel.Info, "Row count from second query " + dt.Rows.Count);
                
                DataTable dtBNATrxnIds = dt.DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });
                foreach (DataRow drRow in dtBNATrxnIds.Rows)
                {
                    DataRow[] dr = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                    if (dr.Length > 0)
                    {
                        DataTable dtUniqueNoteType = dr.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                        foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                        {
                            DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"],"notes_count");
                            if (temp[0]["status"].ToString() == "Successful")
                            {
                                notesAcceptedCount += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                            }
                            else
                            {
                                notesRejectedCount += int.Parse(temp[temp.Length - 1]["notes_count"].ToString());
                            }
                        }

                    }
                }

                int totalNotes = notesAcceptedCount + notesRejectedCount;
                LogableTask.LogMonoActivityTask("RowCount", MethodBase.GetCurrentMethod(), TraceLevel.Info, "notesAcceptedCount +  notesRejectedCount " + totalNotes);
               
                //if (totalNotes > 0)
                //    notesAcceptance = Math.Round((decimal)notesAcceptedCount / totalNotes * 100, 2);
                ////chequeAcceptance = Math.Round((decimal)drAccepted.Length / totalChecks * 100, 2);
                // (drAccepted.Length  + notesAcceptedCount)/(totalNotes+totalChecks)

                //Label_AccPerc.Text = ((chequeAcceptance + notesAcceptance) / 2).ToString();
                int total = totalNotes + totalChecks;
                LogableTask.LogMonoActivityTask("RowCount", MethodBase.GetCurrentMethod(), TraceLevel.Info, "totalNotes + totalChecks: " + totalNotes + totalChecks);
              
                if (total>0)
                Label_AccPerc.Text = Math.Round((((decimal)drAccepted.Length  + notesAcceptedCount)/total)*100,2).ToString();

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

            }
            else if (CheckBoxList_Module.Items[0].Selected && !CheckBoxList_Module.Items[1].Selected)
            {
                string SQL = @"select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                          inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                           " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter();

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
            else if (!CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
                int notesAcceptedCount = 0;
                int notesRejectedCount = 0;

                string SQL = @"select  vEjParsedBNATransaction.ej_parsed_bna_transaction_id,status,note_type,notes_count from vEjParsedBNATransaction inner join ej_parsed_bna_transaction_detail 
                          on vEjParsedBNATransaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                             " inner join atm on atm.atm_id=vEjParsedBNATransaction.atm_id " +
                            " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                           " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter();

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
                            DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"],"notes_count");
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
            if (!base.IsPostBack)
            {
                this.TextBox_To.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.TextBox_From.Text = DateTime.Now.AddDays(-7.0).ToString("dd/MM/yyyy");
                Button1.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(Button1, null) + ";");
            }
        }
    }


}