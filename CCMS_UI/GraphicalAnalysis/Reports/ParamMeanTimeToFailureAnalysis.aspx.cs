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
using System.Text;

namespace CCMSUI
{
    public partial class ParamMeanTimeToFailureAnalysis : BasePage
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
            try
            {
                string from = TextBox_From.Text;
                string to = TextBox_To.Text;
                DrawGraph(from, to);
            }
            catch (Exception ex)
            {

            }
        }

        private string Filter()
        {
            StringBuilder builder = new StringBuilder();
            foreach(ListItem listItem in CheckBoxList_TrxnType.Items)
            {
                if (listItem.Selected)
                    builder.Append(listItem.Value + ",");
            }
            builder.Append("-1");
            return " and transaction_type_id in (" + builder.ToString()+ ") ";
        }
        private void DrawGraph(string from, string to)
        {
            int count= 0;
            int successfulCount = 0;
            //bool series1DataPointAdded = false;
            //bool series2DataPointAdded = false;
            Chart_MeanTimeToFailure1.Series[0].Points.Clear();
            Chart_MeanTimeToFailure1.Series[1].Points.Clear();

            DataTable dtDeviceError = new DataTable();
            dtDeviceError.Columns.Add("trxn_datetime", typeof(DateTime));
            dtDeviceError.Columns.Add("successfulTrxnCount");

            DataTable dtBusinessRuleError = dtDeviceError.Clone();

            
            string SQL = "select status,trxn_datetime,mstate_id from ej_parsed_transactions " +
                " where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) "+ Filter() + " order by trxn_datetime ";
            DataTable dt = GetDataTable(SQL);
            DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
            for (int i = 0; i < dtTrxns.Rows.Count; i++)
            {
                count++;
                //DataRow[] previousDayLowBalancedRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                DataRow[] drArray = dt.Select("trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] + "#");
                foreach (DataRow dr in drArray)
                {
                    if (dr["status"].ToString() == "0")
                        successfulCount++;
                    else if (dr["status"].ToString() == "1" && successfulCount>0)
                    {
                        if (dr["mstate_id"] == DBNull.Value)
                        {
                            DataRow newRow = dtBusinessRuleError.NewRow();
                            newRow[0] = DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString());
                            newRow[1] = successfulCount;
                            dtBusinessRuleError.Rows.Add(newRow);
                            successfulCount = 0;
                        }
                        else
                        {
                            DataRow newRow = dtDeviceError.NewRow();
                            newRow[0] = DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString());
                            newRow[1] = successfulCount;
                            dtDeviceError.Rows.Add(newRow);
                            successfulCount = 0;
                        }
                    }
                }
            }

            int max = dtBusinessRuleError.Rows.Count > dtDeviceError.Rows.Count ? dtBusinessRuleError.Rows.Count : dtDeviceError.Rows.Count;
            int sample = int.Parse(TextBox_Instance.Text);
            for (int i = 0; i < max; i++)
            {
                if (i < dtBusinessRuleError.Rows.Count)
                {
                    if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
                    {
                        Chart_MeanTimeToFailure1.Series[0].Points.AddXY("Instance " + (i + 1), dtBusinessRuleError.Rows[i]["successfulTrxnCount"]);
                        Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = "Successful Transactions[" + dtBusinessRuleError.Rows[i]["successfulTrxnCount"] + "]"; 
                    }
                    else
                        Chart_MeanTimeToFailure1.Series[0].Points.AddXY("Instance " + (i + 1), 0);
                }
                if (i < dtDeviceError.Rows.Count)
                {
                    if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
                    {
                        Chart_MeanTimeToFailure1.Series[1].Points.AddXY("Instance " + (i + 1), dtDeviceError.Rows[i]["successfulTrxnCount"]);
                        Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[1].Points.Count - 1].ToolTip = "Successful Transactions[" + dtDeviceError.Rows[i]["successfulTrxnCount"] + "]"; 
                    }
                    else
                        Chart_MeanTimeToFailure1.Series[1].Points.AddXY("Instance " + (i + 1), 0);
                }
                if (i == sample)
                    break;
             }

            Chart_MeanTimeToFailure1.ResetAutoValues();
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

                TransactionType.TransactionTypeReader reader = TransactionType.ExecuteReader("transaction_type_id>0");
                while (reader.Read()){
                    CheckBoxList_TrxnType.Items.Add(new ListItem(reader.CurrentTransactionType.TransactionTypeName,reader.CurrentTransactionType.TransactionTypeId.ToString()));
                }
                reader.Close();

            }
        }
    }


}