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
    public partial class PNCMeanTimeAnalysisForSDM : BasePage
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
                if (Page.IsValid)
                {
                    string from = TextBox_From.Text;
                    string to = TextBox_To.Text;
                    DrawGraph(from, to);
                }
            }
            catch (Exception ex)
            {
                Literal_Message.Text = ex.Message;
            }
        }

        private void DrawGraph(string from, string to)
        {

            string filter = " and (dispute_status is null or dispute_status = '000') ";
            int successfulTrxns = 0;
            int samplingSize = int.Parse(TextBox_SamplingSize.Text);
            int failureCount = 0;
            filter += " and is_sdm = 1 ";
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("DateTime");
            dtGrid.Columns.Add("TotalDeposits");
            dtGrid.Columns.Add("TotalTrxns");
            dtGrid.Columns.Add("FailureCount");

            if (!CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
                Chart_MeanTimeToFailure1.Titles[0].Text = "Mean Time To Failure Analysis for SDM BNA";
                Chart_MeanTimeToFailure1.ChartAreas[0].AxisY.Title = "Faults Per "+samplingSize + " Deposits";
                List<int> parsedBNATransactions = new List<int>();
                int count = 0;
                int successfulChqDeposit = 0;
                int id = 0; Chart_MeanTimeToFailure1.Series[0].Points.Clear();
                Chart_MeanTimeToFailure1.Series[1].Points.Clear();

                string SQL = "select * from vEjParsedBNATransaction left join ej_parsed_bna_transaction_detail on vEjParsedBNATransaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                    " inner join atm on atm.atm_id = vEjParsedBNATransaction.atm_id " +
                    " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                    " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + filter + " and is_eligible=1  order by trxn_datetime ";
                DataTable dt = GetDataTable(SQL);
                DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
                for (int i = 0; i < dtTrxns.Rows.Count; i++)
                {
                    if (count > samplingSize)
                        break;
                    DataRow[] drArray = dt.Select("trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] + "#");
                    foreach (DataRow dr in drArray)
                    {
                        if (count > samplingSize)
                            break;

                        id = int.Parse(dr["ej_parsed_bna_transaction_id"].ToString());
                        if (parsedBNATransactions.Contains(id))
                            continue;
                        else
                            parsedBNATransactions.Add(id);
                        count++;
                        if (dr["status"].ToString() == "Successful")
                        {
                            successfulTrxns++;
                            DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id = " + dr["ej_parsed_bna_transaction_id"]);
                            if (temp.Length > 0)
                            {
                                DataTable dtUniqueNoteType = temp.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                {
                                    DataRow[] innerTemp = dt.Select("ej_parsed_bna_transaction_id=" + dr["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"]);
                                    successfulChqDeposit += int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString());
                                }

                            }
                        }
                        else if (dr["status"].ToString() == "Failed")
                            failureCount++;

                        if (successfulChqDeposit >= samplingSize)
                        {
                            if (failureCount > 0)
                            {
                                Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), failureCount);
                                Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = failureCount + " deposits failed";

                                Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulTrxns);
                                Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = successfulTrxns + " transactions processed";
                            }
                            DataRow newRow = dtGrid.NewRow();
                            newRow[0] = dtTrxns.Rows[i]["trxn_datetime"].ToString();
                            newRow[1] = successfulChqDeposit;
                            newRow[2] = successfulTrxns;
                            newRow[3] = failureCount;
                            dtGrid.Rows.Add(newRow);

                            successfulChqDeposit = 0;
                            successfulTrxns = 0;
                            count = 0;
                            failureCount = 0;

                        }
                    }
                }
                GridView_MeanTime.DataSource = dtGrid;
                GridView_MeanTime.DataBind();
            }
            else if (CheckBoxList_Module.Items[0].Selected && !CheckBoxList_Module.Items[1].Selected)
            {
                Chart_MeanTimeToFailure1.Titles[0].Text = "Mean Time To Failure Analysis for SDM CPM";
                Chart_MeanTimeToFailure1.ChartAreas[0].AxisY.Title = "Faults Per " + samplingSize + " Deposits";
                List<int> parsedCPMTransactions = new List<int>();
                int count = 0;
                int successfulChqDeposit = 0;
                int id = 0;
                Chart_MeanTimeToFailure1.Series[0].Points.Clear();
                Chart_MeanTimeToFailure1.Series[1].Points.Clear();

                string SQL = "select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                    " inner join atm on atm.atm_id = ej_parsed_cpm_transaction.atm_id " +
                    " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                    " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) and is_eligible=1 " + filter + "order by trxn_datetime ";
                DataTable dt = GetDataTable(SQL);
                DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
                for (int i = 0; i < dtTrxns.Rows.Count; i++)
                {
                    if (count > samplingSize)
                        break;
                    DataRow[] drArray = dt.Select("trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] + "#");
                    foreach (DataRow dr in drArray)
                    {
                        if (count > samplingSize)
                            break;

                        id = int.Parse(dr["ej_parsed_cpm_transaction_id"].ToString());
                        if (parsedCPMTransactions.Contains(id))
                            continue;
                        else
                            parsedCPMTransactions.Add(id);
                        count++;
                        if (dr["status"].ToString() == "Successful")
                        {
                            successfulTrxns++;
                            DataRow[] temp = dt.Select("ej_parsed_cpm_transaction_id = " + dr["ej_parsed_cpm_transaction_id"]);
                            if (temp != null)
                                successfulChqDeposit += temp.Length; // Successful cheque deposit count;
                        }
                        else if (dr["status"].ToString() == "Failed")
                            failureCount++;

                        if (successfulChqDeposit >= samplingSize)
                        {
                            if (failureCount > 0)
                            {
                                Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), failureCount);
                                Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = failureCount + " deposits failed ";

                                Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulTrxns);
                                Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = successfulTrxns + " transactions processed";
                            }
                            DataRow newRow = dtGrid.NewRow();
                            newRow[0] = dtTrxns.Rows[i]["trxn_datetime"].ToString();
                            newRow[1] = successfulChqDeposit;
                            newRow[2] = successfulTrxns;
                            newRow[3] = failureCount;
                            dtGrid.Rows.Add(newRow);
                            successfulChqDeposit = 0;
                            successfulTrxns = 0;
                            count = 0;
                            failureCount = 0;


                        }
                    }
                }
                GridView_MeanTime.DataSource = dtGrid;
                GridView_MeanTime.DataBind();
            }
            else if (CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
                Chart_MeanTimeToFailure1.Titles[0].Text = "Mean Time To Failure Analysis for SDM";
                
                Chart_MeanTimeToFailure1.ChartAreas[0].AxisY.Title = "Faults Per " + samplingSize + " Deposits";
                Hashtable ht = new Hashtable();
                List<int> parsedCPMTransactions = new List<int>();
                List<int> parsedBNATransactions = new List<int>();
                int count = 0;
                int successfulChqDeposit = 0;
                int id = 0; Chart_MeanTimeToFailure1.Series[0].Points.Clear();
                Chart_MeanTimeToFailure1.Series[1].Points.Clear();

                string SQL = "select * from vEjParsedBNATransaction left join ej_parsed_bna_transaction_detail on vEjParsedBNATransaction.ej_parsed_bna_transaction_id = ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id " +
                    " inner join atm on atm.atm_id = vEjParsedBNATransaction.atm_id " +
                    " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                    " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) " + filter + " and is_eligible=1  order by trxn_datetime ";
                DataTable dt = GetDataTable(SQL);

                //Merging...
                SQL = "select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                 " inner join atm on atm.atm_id = ej_parsed_cpm_transaction.atm_id " +
                 " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                 " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) and is_eligible=1 " + filter + "order by trxn_datetime ";
                DataTable dt1 = GetDataTable(SQL);

                DataTable merged = new DataTable();
                merged.Columns.Add("ej_parsed_bna_transaction_id", typeof(int));
                merged.Columns.Add("ej_parsed_cpm_transaction_id", typeof(int));
                merged.Columns.Add("dispute_status");
                merged.Columns.Add("status");
                merged.Columns.Add("trxn_datetime", typeof(DateTime));
                //

                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newRow = merged.NewRow();
                    newRow["ej_parsed_bna_transaction_id"] = dr["ej_parsed_bna_transaction_id"];
                    newRow["dispute_status"] = dr["dispute_status"];
                    newRow["status"] = dr["status"];
                    newRow["trxn_datetime"] = dr["trxn_datetime"];
                    merged.Rows.Add(newRow);
                }
                foreach (DataRow dr in dt1.Rows)
                {
                    DataRow newRow = merged.NewRow();
                    newRow["ej_parsed_cpm_transaction_id"] = dr["ej_parsed_cpm_transaction_id"];
                    newRow["dispute_status"] = dr["dispute_status"];
                    newRow["status"] = dr["status"];
                    newRow["trxn_datetime"] = dr["trxn_datetime"];
                    merged.Rows.Add(newRow);
                }


                //DataTable dtTrxns = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
                DataTable dtTrxns = merged.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
                dtTrxns.DefaultView.Sort = "trxn_datetime";
                dtTrxns = dtTrxns.DefaultView.ToTable();
                for (int i = 0; i < dtTrxns.Rows.Count; i++)
                {
                    if (count > samplingSize)
                        break;
                    DataRow[] drArray = merged.Select("trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] + "#");
                    foreach (DataRow dr in drArray)
                    {
                        if (count > samplingSize)
                            break;

                        if (dr["ej_parsed_bna_transaction_id"].ToString().Length > 0)
                        {
                            id = int.Parse(dr["ej_parsed_bna_transaction_id"].ToString());
                            if (parsedBNATransactions.Contains(id))
                                continue;
                            else
                                parsedBNATransactions.Add(id);
                            count++;

                            if (dr["status"].ToString() == "Successful")
                            {
                                successfulTrxns++;

                                DataRow[] temp = dt.Select("ej_parsed_bna_transaction_id = " + dr["ej_parsed_bna_transaction_id"]);
                                if (temp.Length > 0)
                                {
                                    DataTable dtUniqueNoteType = temp.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                    foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                    {
                                        DataRow[] innerTemp = dt.Select("ej_parsed_bna_transaction_id=" + dr["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"]);
                                        successfulChqDeposit += int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString());
                                    }

                                }
                            }
                            else if (dr["status"].ToString() == "Failed")
                                failureCount++;

                            if (successfulChqDeposit >= samplingSize)
                            {
                                if (failureCount > 0)
                                {
                                    Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), failureCount);
                                    Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = failureCount + " deposits failed";

                                    Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulTrxns);
                                    Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = successfulTrxns + " transactions processed";
                                }
                                DataRow newRow = dtGrid.NewRow();
                                newRow[0] = dtTrxns.Rows[i]["trxn_datetime"].ToString();
                                newRow[1] = successfulChqDeposit;
                                newRow[2] = successfulTrxns;
                                newRow[3] = failureCount;
                                dtGrid.Rows.Add(newRow);
                                successfulChqDeposit = 0;
                                successfulTrxns = 0;
                                count = 0;
                                failureCount = 0;


                            }
                        }
                        if (dr["ej_parsed_cpm_transaction_id"].ToString().Length > 0)
                        {
                            id = int.Parse(dr["ej_parsed_cpm_transaction_id"].ToString());
                            if (parsedCPMTransactions.Contains(id))
                                continue;
                            else
                                parsedCPMTransactions.Add(id);
                            count++;

                            if (dr["status"].ToString() == "Successful")
                            {
                                successfulTrxns++;
                                DataRow[] temp = dt1.Select("ej_parsed_cpm_transaction_id = " + dr["ej_parsed_cpm_transaction_id"]);
                                if (temp != null)
                                    successfulChqDeposit += temp.Length; // Successful cheque deposit count;
                            }
                            else if (dr["status"].ToString() == "Failed")
                                failureCount++;

                            if (successfulChqDeposit >= samplingSize)
                            {
                                if (failureCount > 0)
                                {

                                    Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), failureCount);
                                    Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = failureCount + " deposits failed";

                                    Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulTrxns);
                                    Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = successfulTrxns + " transactions processed";

                                }
                                DataRow newRow = dtGrid.NewRow();
                                newRow[0] = dtTrxns.Rows[i]["trxn_datetime"].ToString();
                                newRow[1] = successfulChqDeposit;
                                newRow[2] = successfulTrxns;
                                newRow[3] = failureCount;
                                dtGrid.Rows.Add(newRow);
                                successfulChqDeposit = 0;
                                successfulTrxns = 0;
                                count = 0;
                                failureCount = 0;

                            }
                        }



                    }
                    //CPM
                    //List<int> parsedCPMTransactions = new List<int>();
                    //count = 0;
                    //successfulChqDeposit = 0;
                    //id = 0;
                    //Chart_MeanTimeToFailure1.Series[0].Points.Clear();
                    //Chart_MeanTimeToFailure1.Series[1].Points.Clear();

                    //SQL = "select * from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                    //     " inner join atm on atm.atm_id = ej_parsed_cpm_transaction.atm_id " +
                    //     " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + from + "',103)  " +
                    //     " and trxn_datetime<=convert(datetime,'" + to + " 23:59:59',103) and is_eligible=1 " + filter + "order by trxn_datetime ";
                    //dt = GetDataTable(SQL);
                    //dtTrxns = dt.DefaultView.ToTable(true, new string[] { "trxn_datetime" });
                    //for (int i = 0; i < dtTrxns.Rows.Count; i++)
                    //{
                    //    count++;
                    //    DataRow[] drArray = dt.Select("trxn_datetime = #" + dtTrxns.Rows[i]["trxn_datetime"] + "#");
                    //    foreach (DataRow dr in drArray)
                    //    {
                    //        id = int.Parse(dr["ej_parsed_cpm_transaction_id"].ToString());
                    //        if (parsedCPMTransactions.Contains(id))
                    //            continue;
                    //        else
                    //            parsedCPMTransactions.Add(id);

                    //        if (dr["status"].ToString() == "Successful")
                    //        {
                    //            DataRow[] temp = dt.Select("ej_parsed_cpm_transaction_id = " + dr["ej_parsed_cpm_transaction_id"]);
                    //            if (temp != null)
                    //                successfulChqDeposit += temp.Length; // Successful cheque deposit count;
                    //        }
                    //        else if (dr["status"].ToString() == "Failed" && successfulChqDeposit > 0)
                    //        {
                    //            if (dr["dispute_status"] != DBNull.Value) // Business Rules
                    //            {
                    //                if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
                    //                {
                    //                    if (ht.Contains(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString())))
                    //                    {
                    //                        string val = ht[DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString())].ToString();
                    //                        Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit + int.Parse(val));
                    //                        Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = (successfulChqDeposit + int.Parse(val)) + " successful deposits before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                    //                    }
                    //                    else
                    //                    {
                    //                        Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit);
                    //                        Chart_MeanTimeToFailure1.Series[0].Points[Chart_MeanTimeToFailure1.Series[0].Points.Count - 1].ToolTip = successfulChqDeposit + " successful deposits before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                    //                    }
                    //                }
                    //                else
                    //                    Chart_MeanTimeToFailure1.Series[0].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), 0);

                    //                successfulChqDeposit = 0;
                    //            }
                    //            else
                    //            {
                    //                if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
                    //                {
                    //                    if (ht.Contains(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString())))
                    //                    {
                    //                        string val = ht[DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString())].ToString();
                    //                        Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit + int.Parse(val));
                    //                        Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[1].Points.Count - 1].ToolTip = (successfulChqDeposit + int.Parse(val)) + " successful deposits before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                    //                    }
                    //                    else
                    //                    {
                    //                        Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), successfulChqDeposit);
                    //                        Chart_MeanTimeToFailure1.Series[1].Points[Chart_MeanTimeToFailure1.Series[1].Points.Count - 1].ToolTip = successfulChqDeposit + " successful deposits before getting error on " + dtTrxns.Rows[i]["trxn_datetime"];
                    //                    }
                    //                }
                    //                else
                    //                    Chart_MeanTimeToFailure1.Series[1].Points.AddXY(DateTime.Parse(dtTrxns.Rows[i]["trxn_datetime"].ToString()), 0);
                    //                successfulChqDeposit = 0;
                    //            }
                    //        }
                    //}//
                }
                //
            }
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