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
using System.Diagnostics;
using System.Reflection;
using System.Web.Caching;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CCMSUI.CCMS
{
    public partial class ViewTransactions : BasePage
    {
        Hashtable ht = new Hashtable(5);

        decimal totalPurgedAmount = 0;
        decimal totalDispensedAmount = 0;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
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


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                //if (!CheckRight((int)Permissions.TaskStatusMonitoringHistory, true))
                //    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["SortExpression"] = "trxn_datetime";
                ViewState["sortDirection"] = " asc ";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                //BindGrid(0);
                BindNoteSetType();
                RadioButtonList_AnaylsisType.SelectedIndex = 0;
                RadioButtonList_AnaylsisType_SelectedIndexChanged(this, e);

                if (System.Configuration.ConfigurationManager.AppSettings["isPurgeCountsVisibleOnTrxnScreen"] == "1")
                {
                    GridView_Trxn.Columns[14].Visible = true;
                    GridView_Trxn.Columns[15].Visible = true;
                    GridView_Trxn.Columns[16].Visible = true;
                    GridView_Trxn.Columns[17].Visible = true;
                }
                else
                {
                    GridView_Trxn.Columns[14].Visible = false;
                    GridView_Trxn.Columns[15].Visible = false;
                    GridView_Trxn.Columns[16].Visible = false;
                    GridView_Trxn.Columns[17].Visible = false;
                }

            }
        }


        protected void GridView_Trxn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Trxn.PageIndex = e.NewPageIndex;
            BindGrid(RadioButtonList_AnaylsisType.SelectedIndex);
            //BindGridWithCache();
        }

        protected void GridView_Trxn_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
            //if (ViewState["SortExpression"].ToString() == "creation_time")
            //    ViewState["SortExpression"] = "f.creation_time";

            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            BindGrid(RadioButtonList_AnaylsisType.SelectedIndex);
            //BindGridWithCache();
        }


        private void BindGridWithCache()
        {
            DataTable dtTrxn = (DataTable)Cache["dtTrxn"];
            dtTrxn.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
            this.GridView_Trxn.DataSource = dtTrxn;
            this.GridView_Trxn.DataBind();
            Literal_count.Text = dtTrxn.Rows.Count + " records.";

        }
        void BindGrid(int selectedIndex)
        {
            totalPurgedAmount = 0;
            totalDispensedAmount = 0;

            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                ///<Summary>Edited by Ali Shah on 16th Oct, 2019
                ///To dynamically handling of cassettes more than 4
                EnableGridRespectiveColumns();

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[GetATMTransactions]";
                cmd.Parameters.Add(new SqlParameter("AtmId", SqlDbType.VarChar));
                cmd.Parameters[0].Value = Session[SessionVars.SelectedATMs.ToString()];
                cmd.Parameters.Add(new SqlParameter("Filter", SqlDbType.VarChar));
                cmd.Parameters[1].Value = GetFilter(selectedIndex, cmd);
                cmd.Parameters.Add(new SqlParameter("OrderBy", SqlDbType.VarChar));
                cmd.Parameters[2].Value = ViewState["orderby"];


                

//                cmd.CommandText = @"SELECT outerATM.TITLE, trxn_datetime, amount,
//cash_purged1,cash_purged2,cash_purged3,cash_purged4,cash_purged5,cash_purged6,cash_purged7,
//cash_dispensed1,cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,
//cash_dispensed7 ,outerATM.atm_id,cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4,cash_remaining5,cash_remaining6,cash_remaining7,outerATM.location,
//cash_purged1+cash_purged2+cash_purged3+cash_purged4+cash_purged5+cash_purged6+cash_purged7
//cash_purgedTotal,note_set_type_name
//
//                    from parsed_transaction,atm outerATM,note_set_type where parsed_transaction.atm_id = outerATM.atm_id and note_set_type.note_set_type_id=outerATM.note_set_type_id and "
//                + " outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.IS_ACTIVE =1 " + GetFilter(selectedIndex, cmd)
//                + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                //object totalDispensedAmount;
                //object totalPurgedAmount;
                object temp = dtTrxn.Compute("Sum(amount)", "");
                if (temp.ToString().Length>0)
                    totalDispensedAmount = decimal.Parse(temp.ToString());

                temp = dtTrxn.Compute("Sum(purged_amount)", "");
                if (temp.ToString().Length > 0)
                    totalPurgedAmount = decimal.Parse(temp.ToString());

                //totalDispensedAmount = decimal.Parse(dtTrxn.Compute("Sum(amount)", "").ToString());
                //totalPurgedAmount = decimal.Parse(dtTrxn.Compute("Sum(purged_amount)", "").ToString());


                //dtTrxn.Compute("sum",
                //for (int i = 0; i < dtTrxn.Rows.Count; i++)
                //{
                //    totalDispensedAmount += decimal.Parse(dtTrxn.Rows[i][2].ToString());

                //    int atm_id = Convert.ToInt32(dtTrxn.Rows[i][17].ToString());
                //    NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(Atm.LoadAtmByPk(atm_id).NoteSetTypeId);

                //    totalPurgedAmount += (decimal)(noteSetType.DenominationType1 * Convert.ToInt32(dtTrxn.Rows[i][3].ToString()) +
                //        noteSetType.DenominationType2 * Convert.ToInt32(dtTrxn.Rows[i][4].ToString()) +
                //        noteSetType.DenominationType3 * Convert.ToInt32(dtTrxn.Rows[i][5].ToString()) +
                //        noteSetType.DenominationType4 * Convert.ToInt32(dtTrxn.Rows[i][6].ToString()) +
                //        noteSetType.DenominationType5 * Convert.ToInt32(dtTrxn.Rows[i][7].ToString()) +
                //        noteSetType.DenominationType6 * Convert.ToInt32(dtTrxn.Rows[i][8].ToString()) +
                //        noteSetType.DenominationType7 * Convert.ToInt32(dtTrxn.Rows[i][9].ToString()));
                //}

                Cache["dtTrxn"] = dtTrxn;
                Literal_count.Text = dtTrxn.Rows.Count + " records.";
                this.GridView_Trxn.DataSource = dtTrxn;
                this.GridView_Trxn.DataBind();

            }



            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Trxn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SqlCommand cmd = null;
            string body = null;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                int cashPurged1 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged1"].ToString());
                int cashPurged2 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged2"].ToString());
                int cashPurged3 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged3"].ToString());
                int cashPurged4 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged4"].ToString());
                int cashPurged5 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged5"].ToString());
                int cashPurged6 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged6"].ToString());
                int cashPurged7 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged7"].ToString());
                //int atm_id = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["atm_id"].ToString());
                //NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(Atm.LoadAtmByPk(atm_id).NoteSetTypeId);

                //totalPurgedAmount += (decimal)(noteSetType.DenominationType1 * cashPurged1 +
                //    noteSetType.DenominationType2 * cashPurged2 +
                //    noteSetType.DenominationType3 * cashPurged3 +
                //    noteSetType.DenominationType4 * cashPurged4 +
                //    noteSetType.DenominationType5 * cashPurged5 +
                //    noteSetType.DenominationType6 * cashPurged6 +
                //    noteSetType.DenominationType7 * cashPurged7);
                //totalDispensedAmount += Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["amount"].ToString());


                if (cashPurged1 > 0 || cashPurged2 > 0 || cashPurged3 > 0 || cashPurged4 > 0 || cashPurged5 > 0 || cashPurged6 > 0 || cashPurged7 > 0)
                {
                    e.Row.BackColor = Color.LightGreen;
                }
                try
                {
                    Atm atm = Atm.LoadAtm("title='" + ((DataRowView)e.Row.DataItem).Row["title"].ToString() + "'");
                    if (!ht.Contains(atm.Title))
                    {
                        // Data Archiving Work
                        cmd = ConnectionFactory.GetNewCommand(true);
                        cmd.CommandText = "select top 10 status from vTask where atm_id = " + atm.ATMId + " order by creation_time desc";
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            body += reader.GetString(0) + "<br/>";
                        }
                        reader.Close();
                        ht.Add(atm.Title, body);
                    }
                    else
                    {
                        body += (string)ht[atm.Title];
                    }
                    ((System.Web.UI.WebControls.Image)e.Row.FindControl("image_history")).Attributes.Add("title", "header=[ATM Top 10 File Download Status] body=[" + body + ".]");
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (cmd != null)
                        cmd.Connection.Close();
                }

            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                Literal_count.Text += " &nbsp;,Total Purged Amount = " + totalPurgedAmount.ToString() + ",Total Dispensed Amount=" + totalDispensedAmount.ToString();
            }
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_Dispensed1.Text = "";
            TextBox_Dispensed2.Text = "";
            TextBox_Dispensed3.Text = "";
            TextBox_Dispensed4.Text = "";
            TextBox_FromAmount.Text = "";
            TextBox_ToAmount.Text = "";
            TextBox_purgeFrom.Text = "";
            TextBox_purge_to.Text = "";
            BindGrid(0);
 
        }
        protected void Button_ExportToExcel_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToExcel_Click");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"SELECT TITLE, ip, trxn_datetime,processing_datetime, amount,
cash_purged1,cash_purged2,cash_purged3,cash_purged4,cash_purged5,cash_purged6,cash_purged7,

cash_dispensed1,cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,
cash_dispensed7 ,atm.atm_id,cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4,cash_remaining5,cash_remaining6,cash_remaining7

                    from vParsedTransaction parsed_transaction,atm where parsed_transaction.atm_id = atm.atm_id and "
                + " ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and atm.IS_ACTIVE =1 " + GetFilter(RadioButtonList_AnaylsisType.SelectedIndex, cmd)
                + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\ExportTransactions.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "Cash Withdrawal";
                rptDoc.SetParameterValue("FromDate", DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null));
                rptDoc.SetParameterValue("ToDate", DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null));
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dtTrxn.Rows.Count);
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\Transactions_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
                rptDoc.ExportToDisk(ExportFormatType.Excel, path);
                rptDoc.Close();
                Response.AddHeader("Content-Type", "application/vnd.ms-excel");
                Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(path));
                Response.AddHeader("Content-Length", new System.IO.FileInfo(path).Length.ToString());
                Response.BufferOutput = false;
                Response.WriteFile(path);
                Response.Flush();
                Response.Close();

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }


        }

        protected void Button_ExportToPDF_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToPDF_Click");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"SELECT TITLE, ip, trxn_datetime,processing_datetime, amount,
cash_purged1,cash_purged2,cash_purged3,cash_purged4,cash_purged5,cash_purged6,cash_purged7,
cash_dispensed1,cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,
cash_dispensed7 ,atm.atm_id,cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4,cash_remaining5,cash_remaining6,cash_remaining7

                    from vParsedTransaction parsed_transaction,atm where parsed_transaction.atm_id = atm.atm_id and "
                + " ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and atm.IS_ACTIVE =1 " + GetFilter(RadioButtonList_AnaylsisType.SelectedIndex, cmd)
                + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\ExportTransactions.rpt");
                rptDoc.SetDataSource(dtTrxn);
                rptDoc.SummaryInfo.ReportTitle = "Cash Withdrawal";
                rptDoc.SetParameterValue("FromDate", DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null));
                rptDoc.SetParameterValue("ToDate", DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null));
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dtTrxn.Rows.Count);
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\Transactions_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";
                rptDoc.ExportToDisk(ExportFormatType.PortableDocFormat, path);
                rptDoc.Close();
                Response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(path));
                Response.WriteFile(path);
                Response.Flush();
                Response.Close();

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }


        }
        string GetFilter(int index, SqlCommand cmd)
        {
            string filter = "";

            if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                filter += " and note_set_type.note_set_type_id = " + DropDownList_NoteSetTypeID.SelectedItem.Value;
                
            if (index == 0)
            {
                if (TextBox_FromDate.Text.Length > 0)
                    filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

                if (TextBox_ToDate.Text.Length > 0)
                    filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";
            }
            else if (index == 1)
            {
                filter += " and trxn_datetime>=(select max(rep_datetime) from replenishment where atm_id=outerATM.atm_id)";
            }
            else if (index == 2)
            {

                Atm atm = Atm.LoadAtm("title='" + Label_SelectedATM.Text + "'");
                int cycle = int.Parse(TextBox_NoOfCycles.Text);
                SqlCommand repCmd = ConnectionFactory.GetNewCommand(false);
                try
                {
                    
                    repCmd.CommandText = "select top " + cycle + " rep_datetime from replenishment where atm_id=" + atm.ATMId + " order by rep_datetime " + (RadioButtonList_Order.SelectedIndex == 0 ? " asc" : "desc");
                    repCmd.Connection.Open();
                    SqlDataReader reader = repCmd.ExecuteReader();
                    DateTime upperLimit = new DateTime(1900, 1, 1);
                    DateTime lowerLimit = new DateTime(1900, 1, 1);
                    int count = 0;
                    while (reader.Read())
                    {
                        if (count == 0)
                        {
                            //upperLimit = reader.GetDateTime(0);
                            lowerLimit = reader.GetDateTime(0);
                            count++;
                        }

                        upperLimit = reader.GetDateTime(0);
                    }
                    reader.Close();
                    if (RadioButtonList_Order.SelectedIndex == 0)
                        filter += " and trxn_datetime >= convert(datetime,'" + lowerLimit.ToString("dd/MM/yyyy HH:mm:ss") + "',103) " +
                        " and trxn_datetime<= convert(datetime,'" + upperLimit.ToString("dd/MM/yyyy HH:mm:ss") + "',103) ";
                    else
                        filter += " and trxn_datetime >= convert(datetime,'" + upperLimit.ToString("dd/MM/yyyy HH:mm:ss") + "',103) " +
                    " and trxn_datetime<= convert(datetime,'" + lowerLimit.ToString("dd/MM/yyyy HH:mm:ss") + "',103) ";

                }
                finally
                {
                    repCmd.Connection.Close();
                }



            }

            if (TextBox_purgeFrom.Text.Length > 0)
                filter += " and (cash_purged1+cash_purged2+cash_purged3+cash_purged4+cash_purged5+cash_purged6+cash_purged7)>=" + int.Parse(TextBox_purgeFrom.Text);

            if (TextBox_purge_to.Text.Length > 0)
                filter += " and (cash_purged1+cash_purged2+cash_purged3+cash_purged4+cash_purged5+cash_purged6+cash_purged7)<=" + int.Parse(TextBox_purge_to.Text);

            if (TextBox_FromAmount.Text.Length > 0)
                filter += " and amount >= " + int.Parse(TextBox_FromAmount.Text);

            if (TextBox_ToAmount.Text.Length > 0)
                filter += " and amount <= " + int.Parse(TextBox_ToAmount.Text);

            if (TextBox_Dispensed1.Text.Length > 0)
                filter += " and cash_dispensed1 = " + int.Parse(TextBox_Dispensed1.Text);

            if (TextBox_Dispensed2.Text.Length > 0)
                filter += " and cash_dispensed2 = " + int.Parse(TextBox_Dispensed2.Text);

            if (TextBox_Dispensed3.Text.Length > 0)
                filter += " and cash_dispensed3 = " + int.Parse(TextBox_Dispensed3.Text);

            if (TextBox_Dispensed4.Text.Length > 0)
                filter += " and cash_dispensed4 = " + int.Parse(TextBox_Dispensed4.Text);

            return filter;
        }
        protected void button_search_Click(object sender, EventArgs e)
        {
            try
            {
                if (RadioButtonList_AnaylsisType.SelectedIndex == 2)
                {
                    if (TextBox_NoOfCycles.Text.Length == 0)
                        throw new Exception("Invalid number format");

                    if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                    Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                    {
                        throw new Exception("Please, select an ATM");
                        //Label_SelectedATM.Text = "Please, select an ATM";
                    }
                    else
                    {
                        if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                        {
                            throw new Exception("<li>ATM not selected.</li>");
                        }
                        else
                            Label_SelectedATM.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
                    }
                }
                BindGrid(RadioButtonList_AnaylsisType.SelectedIndex);
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }

        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindNoteSetType();
            button_search_Click(this, null);
        }

        protected void RadioButtonList_AnaylsisType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList_AnaylsisType.SelectedIndex == 0)
            {
                trReplenishmentRow.Visible = false;
                trTransactionRow.Visible = true;
                trCycles.Visible = false;
            }
            else if (RadioButtonList_AnaylsisType.SelectedIndex == 1)
            {
                trReplenishmentRow.Visible = false;
                trTransactionRow.Visible = false;
                trCycles.Visible = false;
            }
            else if (RadioButtonList_AnaylsisType.SelectedIndex == 2)
            {
                trCycles.Visible = true;
                trReplenishmentRow.Visible = true;
                trTransactionRow.Visible = false;

                if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                    Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                {
                    //throw new Exception("Please, select an ATM");
                    Label_SelectedATM.Text = "Please, select an ATM";
                }
                else
                {
                    if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                    {
                        Label_SelectedATM.Text = "<li>ATM not selected.</li>";
                    }
                    else
                        Label_SelectedATM.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
                }
            }

        }

        private void EnableGridRespectiveColumns()
        {
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexDispensed = -1;
            int columnIndexPurged = -1;
            int columnIndexRemaining = -1;
            columnIndexDispensed = GetColumnIndexByName(this.GridView_Trxn, "Dispensed 5");
            columnIndexPurged = GetColumnIndexByName(this.GridView_Trxn, "Purged 5");
            columnIndexRemaining = GetColumnIndexByName(this.GridView_Trxn, "Remaining 5");

            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexDispensed >= 0)
                {
                    this.GridView_Trxn.Columns[columnIndexDispensed].Visible = true;
                    columnIndexDispensed++;
                }

                if (columnIndexPurged >= 0)
                {
                    this.GridView_Trxn.Columns[columnIndexPurged].Visible = true;
                    columnIndexPurged++;
                }
                if (columnIndexRemaining >= 0)
                {
                    this.GridView_Trxn.Columns[columnIndexRemaining].Visible = true;
                    columnIndexRemaining++;
                }
            }

            
        }
    }
}