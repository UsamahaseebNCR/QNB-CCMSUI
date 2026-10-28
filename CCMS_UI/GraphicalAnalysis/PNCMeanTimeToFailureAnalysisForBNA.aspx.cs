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
    public partial class PNCMeanTimeToFailureAnalysisForBNA : BasePage
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
                Literal_Message.Text = ex.Message;
            }
        }

        private void DrawGraph(string from, string to)
        {


            List<int> parsedBNATransactions = new List<int>();
            int count = 0;
            int successfulChqDeposit = 0;
            int id = 0;
            //bool series1DataPointAdded = false;
            //bool series2DataPointAdded = false;
            Chart_MeanTimeToFailure1.Series[0].Points.Clear();
            Chart_MeanTimeToFailure1.Series[1].Points.Clear();
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("DateTime");
            dtGrid.Columns.Add("Total");
            dtGrid.Columns.Add("Comment");
            //DataTable dtNonBusinessRules = new DataTable();
            //dtNonBusinessRules.Columns.Add("trxn_datetime", typeof(DateTime));
            //dtNonBusinessRules.Columns.Add("successfulTrxnCount");

            //DataTable dtBusinessRuleError = dtNonBusinessRules.Clone();

            string filter = "";
            if (CheckBoxList_ErrorAnalysisType.Items[0].Selected && CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            {
                filter = "";
            }
            else if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
                filter = " and dispute_status is not null ";
            else if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
                filter = " and (dispute_status is null or dispute_status = '000') ";


            if (CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
            }
            if (CheckBoxList_Module.Items[0].Selected)
                filter += " and is_sdm = 1 ";
            else if (CheckBoxList_Module.Items[1].Selected)
                filter += " and is_cdm = 1 ";

            string SQL = "select top 1000 * from vEjParsedBNATransaction left join ej_parsed_bna_transaction_detail on vEjParsedBNATransaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                " inner join atm on atm.atm_id = vEjParsedBNATransaction.atm_id " +
                " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + filter + " and is_eligible=1  order by trxn_datetime ";
            DataTable dt = GetDataTable(SQL);
            DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
            for (int i = 0; i < dtTrxns.Rows.Count; i++)
            {
                count++;
                //DataRow[] previousDayLowBalancedRows = dt.Select("generated_at < #" + DateTime.Today + "#");
                DataRow[] drArray = dt.Select("trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] + "#");
                foreach (DataRow dr in drArray)
                {
                    id = int.Parse(dr["ej_parsed_bna_transaction_id"].ToString());
                    if (parsedBNATransactions.Contains(id))
                        continue;
                    else
                        parsedBNATransactions.Add(id);

                    if (dr["status"].ToString() == "Successful")
                    {
                        DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id = " + dr["ej_parsed_bna_transaction_id"]);
                        if (temp.Length > 0)
                        {
                            DataTable dtUniqueNoteType = temp.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                            foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                            {
                                DataRow[] innerTemp = dt.Select("ej_parsed_bna_transaction_id=" + dr["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"]);
                                //builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(temp[temp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                successfulChqDeposit += int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString());
                            }

                        }

                        //if (temp != null)
                        //{
                        //    foreach (DataRow drTemp in temp)
                        //        successfulChqDeposit += int.Parse(drTemp["notes_count"].ToString()); // Successful cheque deposit count;
                        //}
                    }
                    else if (dr["status"].ToString() == "Failed" && successfulChqDeposit > 0)
                    {
                        if (dr["dispute_status"] != DBNull.Value) // Business Rules
                        {
                            //DataRow newRow = dtBusinessRuleError.NewRow();
                            //newRow[0] = DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString());
                            //newRow[1] = successfulChqDeposit;
                            //dtBusinessRuleError.Rows.Add(newRow);

                            if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
                            {
                                Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit);
                                Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip =  successfulChqDeposit + " notes deposited before getting error on "+dtTrxns.Rows[i]["trxn_datetime"];
                                DataRow newRow =  dtGrid.NewRow();
                                newRow[0] = dtTrxns.Rows[i]["trxn_datetime"].ToString();
                                newRow[1] = successfulChqDeposit;
                                newRow[2] = dr["comment"];
                                dtGrid.Rows.Add(newRow);
                            }
                            else
                                Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), 0);
                            
                            successfulChqDeposit = 0;
                        }
                        else
                        {   //Non Business Rules
                            //DataRow newRow = dtNonBusinessRules.NewRow();
                            //newRow[0] = DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString());
                            //newRow[1] = successfulChqDeposit;
                            //dtNonBusinessRules.Rows.Add(newRow);
                            
                            if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
                            {
                                Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit);
                                Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[1].Points.Count - 1].ToolTip = successfulChqDeposit + " notes deposited before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                                DataRow newRow = dtGrid.NewRow();
                                newRow[0] = dtTrxns.Rows[i]["trxn_datetime"].ToString();
                                newRow[1] = successfulChqDeposit;
                                newRow[2] = dr["comment"];
                                dtGrid.Rows.Add(newRow);
                            }
                            else
                                Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), 0);
                            

                            successfulChqDeposit = 0;
                        }
                    }
                }
            }

            //int max = dtBusinessRuleError.Rows.Count > dtNonBusinessRules.Rows.Count ? dtBusinessRuleError.Rows.Count : dtNonBusinessRules.Rows.Count;
            //int sample = int.Parse(TextBox_Instance.Text);
            //for (int i = 0; i < max; i++)
            //{
            //    if (i < dtBusinessRuleError.Rows.Count)
            //    {
            //        if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
            //        {
            //            Chart_MeanTimeToFailure1.Series[0].Points.AddXY("Instance " + (i + 1), dtBusinessRuleError.Rows[i]["successfulTrxnCount"]);
            //            Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = "Successful Transactions[" + dtBusinessRuleError.Rows[i]["successfulTrxnCount"] + "]";
            //        }
            //        else
            //            Chart_MeanTimeToFailure1.Series[0].Points.AddXY("Instance " + (i + 1), 0);
            //    }
            //    if (i < dtNonBusinessRules.Rows.Count)
            //    {
            //        if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
            //        {
            //            Chart_MeanTimeToFailure1.Series[1].Points.AddXY("Instance " + (i + 1), dtNonBusinessRules.Rows[i]["successfulTrxnCount"]);
            //            Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[1].Points.Count - 1].ToolTip = "Successful Transactions[" + dtNonBusinessRules.Rows[i]["successfulTrxnCount"] + "]";
            //        }
            //        else
            //            Chart_MeanTimeToFailure1.Series[1].Points.AddXY("Instance " + (i + 1), 0);
            //    }
            //    if (i == sample)
            //        break;
            //}

            Chart_MeanTimeToFailure1.ResetAutoValues();
            GridView_MeanTime.DataSource = dtGrid;
            GridView_MeanTime.DataBind();
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