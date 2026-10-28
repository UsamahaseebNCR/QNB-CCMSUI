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
    public partial class ParamAvgResponseTimeAnalysis : BasePage
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
                Literal_Err.Text = ex.Message;
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
            return " and transaction_type_id in (" + builder.ToString()+ ")";
        }
        private void DrawGraph(string from, string to)
        {
            Chart_AvgResponseTime.Series[0].Points.Clear();
            string SQL = @"select convert(datetime,transaction_end_time,108) - convert(datetime,convert(varchar,transaction_start_time,108),108),convert(datetime,transaction_end_time,108),convert(datetime,transaction_start_time,108),tsn
                          from ej_parsed_transactions where transaction_start_time is not null and transaction_end_time is not null " +
                " and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) "+ Filter() + " order by trxn_datetime ";
            DataTable dt = GetDataTable(SQL);

            DateTime responseTime;            
            int totalSeconds =0;
            int seconds = 0;
            int counter = 0;
            foreach (DataRow dr in dt.Rows)
            {
                responseTime = DateTime.Parse(dr[0].ToString());
                seconds = responseTime.Minute * 60 + responseTime.Second;
                totalSeconds += seconds;
                counter++;
                Chart_AvgResponseTime.Series[0].Points.AddXY(counter, seconds);
                Chart_AvgResponseTime.Series[0].Points[Chart_AvgResponseTime.Series[0].Points.Count - 1].ToolTip = "[End Time] = "+dr[2].ToString() +"[Start Time] = "+dr[1].ToString()+"[TSN] =" + dr[3].ToString();
                    responseTime.ToString("mm:ss");
            }
            Label_AvgResponseTime.Text = Math.Round((double)totalSeconds / counter, 2).ToString();
            Label_TrxnProcessed.Text = counter.ToString();
            

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