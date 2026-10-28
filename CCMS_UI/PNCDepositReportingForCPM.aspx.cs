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
    public partial class PNCDepositReportingForCPM : BasePage
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

            Chart_RejectRate.Series[0].Points.Clear();
            Chart_RejectRate.Series[1].Points.Clear();
            Chart_RejectRate.Series[2].Points.Clear();


            string SQL = @"select  status,CONVERT(varchar,trxn_datetime,103) trxn_datetime 
                        from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id 
                        inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +

                     " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                     " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " + Filter();
          

            //string SQL = "select top 7 status,CONVERT(varchar,trxn_datetime,103) trxn_datetime,COUNT(*) from ej_parsed_cpm_transaction " +
            //    " where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and  trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
            //    " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) "+
            //    " group by status,CONVERT(varchar,trxn_datetime,103)  " +
            //    " order by convert(datetime,CONVERT(varchar,trxn_datetime,103),103)" ;

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
                    Chart_RejectRate.Series[2].Points.AddXY(xAxisLable, Math.Round(((decimal)series2DataPointVal / (series2DataPointVal + series1DataPointVal)) * 100,2));
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
        private void DrawNonBusinessRuleRejectCausesGraph()
        {
            string SQL = "select comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                       " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                       " and dispute_status is null and status = 'Failed' "+Filter()+" and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
                Chart_NonBusinessRuleRejectCauses.Series[0].Points.AddXY(dr[0], dr[1]);

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
        
        private void DrawBusinessRuleRejectCausesGraph()
        {
            string comment = null; 
            string SQL = "select comment,count(*) total from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                       " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +
                       " and dispute_status <>'000' and dispute_status is not null and status = 'Failed' "+Filter()+" and len(comment)>1 group by comment";
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
            //Chart_BusinessRuleRejectCauses.Width = 1500;

            GridView_BusinessRuleRejectCauses.DataSource = dt;
            GridView_BusinessRuleRejectCauses.DataBind();
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
                criteria += " and is_ccdm = 1 ";

             criteria += " and is_eligible = 1";
            return criteria;
        }

        private void  DrawAcceptanceByCheckCountGraph()
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
                Label_AccPerc.Text = Math.Round((decimal)drAccepted.Length / totalChecks * 100,2).ToString();
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