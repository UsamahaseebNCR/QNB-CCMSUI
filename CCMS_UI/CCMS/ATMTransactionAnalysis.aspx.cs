using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;

using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Collections;
using System.Configuration;

namespace CCMSUI.CCMS
{
    public partial class ATMTransactionAnalysis : BasePage
    {
        private void BindNoteSetType()
        {
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && regionID != "1")
            {
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                while (reader.Read())
                {
                    DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                }
                reader.Close();
            }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ATMTransactionAnalysis))
                    Response.Redirect("~/admin/AccessDenied.aspx");
                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                BindNoteSetType();
            }
        }

        private void AnalysisByDenomination()
        {

            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select sum(cash_dispensed1), sum(cash_dispensed2),sum(cash_dispensed3),sum(cash_dispensed4) from parsed_transaction inner join atm on atm.atm_id =parsed_transaction.atm_id where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)  " + (DropDownList_NoteSetTypeID.SelectedIndex > 0 ? "and atm.note_set_type_id =" + DropDownList_NoteSetTypeID.SelectedItem.Value : "");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            this.Chart3.Titles[0].Text = "ATM Demand Analysis By Denomination";
            this.Chart3.ChartAreas[0].AxisY.Title = "No.Of Transactions";
            this.Chart3.ChartAreas[0].AxisX.Title = "Denomination";

            //string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettesDenomination"].Split(new char[] { ',' });
            
            this.Chart3.Series[0].Points.AddXY("Type 1", dt.Rows[0][0]);
            this.Chart3.Series[0].Points.AddXY("Type 2", dt.Rows[0][1]);
            this.Chart3.Series[0].Points.AddXY("Type 3", dt.Rows[0][2]);
            this.Chart3.Series[0].Points.AddXY("Type 4", dt.Rows[0][3]);

            //this.Chart2.Series["Default"]["ShowMarkerLines"] = "true";
            this.Chart3.Series["Default"]["DrawingStyle"] = "Cylinder";
            this.Chart3.Series["Default"]["PointWidth"] = "0.8";

        }
        private void TransactionAmountAnalysis()
        {

            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select count(parsed_transaction_id), amount from parsed_transaction inner join atm on atm.atm_id =parsed_transaction.atm_id where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)" + (DropDownList_NoteSetTypeID.SelectedIndex > 0 ? "and atm.note_set_type_id =" + DropDownList_NoteSetTypeID.SelectedItem.Value : "") + " group by amount order by amount ";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            this.Chart2.Titles[0].Text = "ATM Demand Analysis By Amount";
            this.Chart2.ChartAreas[0].AxisY.Title = "No.Of Transactions";
            this.Chart2.ChartAreas[0].AxisX.Title = "Amount";
            //string[] parts = ConfigurationManager.AppSettings["ConfiguredCassettesDenomination"].Split(new char[] { ',' });
            //string currency = parts[0].Substring(0, 3);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.Chart2.Series[0].Points.AddXY(dt.Rows[i][1].ToString(), dt.Rows[i][0]);
            }
            //this.Chart2.Series["Default"]["ShowMarkerLines"] = "true";
            this.Chart2.Series["Default"]["DrawingStyle"] = "Cylinder";
            this.Chart2.Series["Default"]["PointWidth"] = "0.8";

        }
        private void ComputeDemand()
        {
            SqlConnection conn = null;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select count(parsed_transaction_id)/count(distinct(day(trxn_datetime))),day(trxn_datetime) from parsed_transaction inner join atm on atm.atm_id =parsed_transaction.atm_id where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " + (DropDownList_NoteSetTypeID.SelectedIndex > 0 ? "and atm.note_set_type_id =" + DropDownList_NoteSetTypeID.SelectedItem.Value : "")+"group by day(trxn_datetime) order by day(trxn_datetime) ";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                this.Chart1.Titles[0].Text = "ATM Demand Analysis By Transaction Count";
                this.Chart1.ChartAreas[0].AxisY.Title = "No.Of Transactions";
                this.Chart1.ChartAreas[0].AxisX.Title = "Dates";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    this.Chart1.Series[0].Points.AddXY(dt.Rows[i][1], dt.Rows[i][0]);
                }
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

        }


        private decimal ComputeUtilization()
        {
            decimal rep = 0;
            decimal ret = 0;
            decimal util = 0;
            decimal totalUtil = 0;
            decimal gTotalUtil = 0;
            int gTotalCount = 0;
            int count = 0;
            SqlConnection conn = null;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select replenishment_amount,return_amount,atm.atm_id,trxn_datetime from summary inner join atm on atm.atm_id =summary.atm_id where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " + (DropDownList_NoteSetTypeID.SelectedIndex > 0 ? "and atm.note_set_type_id =" + DropDownList_NoteSetTypeID.SelectedItem.Value : "") + "order by trxn_datetime";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                List<int> atmIDs = new List<int>();
                //List<decimal> utils = new List<decimal>();
                int atmID = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    atmID = int.Parse(dr[2].ToString());
                    if (!atmIDs.Contains(atmID))
                        atmIDs.Add(atmID);
                }
                foreach (int id in atmIDs)
                {
                    DataRow[] drArray = dt.Select("atm_id=" + id, "trxn_datetime asc");
                    if (drArray.Length > 1)
                    {
                        for (int k = 0; k < drArray.Length; k++)
                        {
                            if (k + 1 < drArray.Length)
                            {
                                rep = decimal.Parse(drArray[k][0].ToString());
                                ret = decimal.Parse(drArray[k + 1][1].ToString());
                                if (rep != 0)
                                {
                                    util = ((rep - ret) / rep) * 100;
                                    totalUtil += util;
                                    count++;
                                }
                            }
                        }
                    }
                    if (count != 0)
                    {
                        gTotalUtil += totalUtil / count;
                        gTotalCount++;
                    }
                    totalUtil = 0;
                    count = 0;
                }
                if (gTotalCount != 0)
                    return Math.Round(gTotalUtil / gTotalCount, 2);
                else
                    return 0;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

        }

        private string GetCITATMTitle()
        {
            StringBuilder builder = new StringBuilder();
            SqlConnection conn = null;

            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select cit_atm_title from atm where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and cit_atm_title is not null";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    builder.Append("'" + reader[0] + "',");
                builder.Append("'-1'");
                return builder.ToString();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            string totalTrxnSQL = "select count(parsed_transaction_id) from parsed_transaction inner join atm on atm.atm_id =parsed_transaction.atm_id  where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) "+ (DropDownList_NoteSetTypeID.SelectedIndex>0?"and atm.note_set_type_id ="+DropDownList_NoteSetTypeID.SelectedItem.Value:"");
            string totalRepSQL = "select count(replenishment_id) from replenishment inner join atm on atm.atm_id =replenishment.atm_id where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and rep_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and rep_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)" + (DropDownList_NoteSetTypeID.SelectedIndex > 0 ? "and atm.note_set_type_id =" + DropDownList_NoteSetTypeID.SelectedItem.Value : "");
            string totalCashOutSQL = "select isnull(sum(amount),0) from parsed_transaction inner join atm on atm.atm_id =parsed_transaction.atm_id where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " + (DropDownList_NoteSetTypeID.SelectedIndex > 0 ? "and atm.note_set_type_id =" + DropDownList_NoteSetTypeID.SelectedItem.Value : "");
            string utilizationSQL = "select replenishment_amount,return_amount,atm_id from summary inner join atm on atm.atm_id =summary.atm_id where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and trxn_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " + (DropDownList_NoteSetTypeID.SelectedIndex > 0 ? "and atm.note_set_type_id =" + DropDownList_NoteSetTypeID.SelectedItem.Value : "") + " order by trxn_datetime";

            string callOutSQL = "select count(flm_monitoring_id) from flm_monitoring inner join atm on atm.cit_atm_title =flm_monitoring.atm_id_no where atm_id_no in (" + GetCITATMTitle() + ") and time_of_info>=convert(datetime,'" + TextBox_FromDate.Text + "',103) and time_of_info<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) " + (DropDownList_NoteSetTypeID.SelectedIndex > 0 ? "and atm.note_set_type_id =" + DropDownList_NoteSetTypeID.SelectedItem.Value : ""); 


            Label_TotalTrxn.Text = ((int)ConnectionFactory.ExecuteScalar(totalTrxnSQL)).ToString();
            Label_TotalReplenishments.Text = ((int)ConnectionFactory.ExecuteScalar(totalRepSQL)).ToString();
            Label_TotalCashOut.Text = ConnectionFactory.ExecuteScalar(totalCashOutSQL).ToString();
            Label_CashUtilization.Text = ComputeUtilization().ToString();
            Label_CallOut.Text = ((int)ConnectionFactory.ExecuteScalar(callOutSQL)).ToString();
            ComputeDemand();
            TransactionAmountAnalysis();
            AnalysisByDenomination();
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindNoteSetType();
            //Button_search_Click(this, null);
        }


    }
}