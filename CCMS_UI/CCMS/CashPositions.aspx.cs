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
using System.Data.SqlClient;

using System.Diagnostics;
using System.Reflection;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Text;




namespace CCMSUI.CCMS
{
    public partial class CashPositions : BasePage
    {
        protected void CheckBox_Current_CheckedChanged(object sender, EventArgs e)
        {
            TextBox_FromDate.Enabled = (!TextBox_FromDate.Enabled);
            TextBox_ToDate.Enabled = (!TextBox_ToDate.Enabled);
        }
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
                var dt = new DataTable();
                dt.Columns.Add("note_set_type_name");
                dt.Columns.Add("region_id");
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                while (reader.Read())
                {
                    // DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                    dt.Rows.Add(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString());

                }

                if (dt.Rows.Count > 0)
                {
                    groupList.DataSource = dt;
                    groupList.DataTextField = "note_set_type_name";
                    groupList.DataValueField = "region_id";
                    groupList.DataBind();
                }

                reader.Close();
            }
        }
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindNoteSetType();

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";

                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
        }



        protected void Button_ExportToExcel_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToPDF_Click");
            string trxnTableName = null;
            string cashPosTableName = "";
            string replenishmentTableName = "";
            try
            {
                // Data Archiving Work
                if (Request.QueryString["ej"] == null)
                {
                    trxnTableName = "vParsedTransaction";
                    cashPosTableName = "vCashPosition";
                    replenishmentTableName = "vReplenishment";
                }
                else
                {
                    trxnTableName = "vEjParsedTransactions";
                    cashPosTableName = "vEjCashPosition";
                    replenishmentTableName = "vEjParsedReplenishments";
                }
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandText = @"select  note_set_type_name,outerATM.description ,outerATM.location,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
                                    p.cassette1_notes, p.cassette2_notes,
                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
                                    p.cassette7_notes,p.purge_cassette1_notes,p.purge_cassette2_notes, p.purge_cassette3_notes,p.purge_cassette4_notes, (select max(trxn_datetime) from " + trxnTableName + " where atm_id = outerATM.atm_id and amount>0 )last_successful_trxn_at,last_trxn_at, " +
" note_set_type.denomination_type_1 * p.cassette1_notes +  " +
" note_set_type.denomination_type_2 * p.cassette2_notes + " +
" note_set_type.denomination_type_3 * p.cassette3_notes + " +
" note_set_type.denomination_type_4 * p.cassette4_notes + " +
" note_set_type.denomination_type_5 * p.cassette5_notes + " +
" note_set_type.denomination_type_6 * p.cassette6_notes + " +
" note_set_type.denomination_type_7 * p.cassette7_notes total ," +
" note_set_type.denomination_type_1 * p.purge_cassette1_notes +  " +
" note_set_type.denomination_type_2 * p.purge_cassette2_notes + " +
" note_set_type.denomination_type_3 * p.purge_cassette3_notes + " +
" note_set_type.denomination_type_4 * p.purge_cassette4_notes totalPurged ," +

" ( select max(rep_datetime) from " + replenishmentTableName + " where atm_id = outerATM.atm_id)last_replenishment_at,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id,r.parent_region_id,bank_logo " +
" ,  ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    " +
" and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenishment_at, " +

" ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
" and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount" +
 " from atm outerATM , " + cashPosTableName + " p ,note_set_type,region r" +
" where outerATM.atm_id = p.atm_id and outerATM.is_active = 1  and outerATM.is_atm = 1 " +
" and outerATM.note_set_type_id = note_set_type.note_set_type_id " + Filter() +
"  and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.region_id = r.region_id";


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dsCashPositions dsTrxn = new dsCashPositions();
                adapter.Fill(dsTrxn.Tables[0]);
                PopulateDataTable(dsTrxn.Tables[0]);
                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\CashPositionsRpt.rpt");
                rptDoc.SetDataSource(dsTrxn.Tables[0]);
                rptDoc.SummaryInfo.ReportTitle = "Cash Positions Report";
                rptDoc.SetParameterValue("FromDate", DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy HH:mm:ss", null));
                rptDoc.SetParameterValue("ToDate", DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy HH:mm:ss", null));
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dsTrxn.Tables[0].Rows.Count);
                rptDoc.SetParameterValue("suppressDetail", "0");
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\CashPositions_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";
                rptDoc.ExportToDisk(ExportFormatType.ExcelRecord, path);
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

        protected void Button_ExportToPDF_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExportToPDF_Click");
            string trxnTableName = null;
            string cashPosTableName = "";
            string replenishmentTableName = "";
            try
            {
                // Data Archiving Work
                if (Request.QueryString["ej"] == null)
                {
                    trxnTableName = "vParsedTransaction";
                    cashPosTableName = "vCashPosition";
                    replenishmentTableName = "vReplenishment";
                }
                else
                {
                    trxnTableName = "vEjParsedTransactions";
                    cashPosTableName = "vEjCashPosition";
                    replenishmentTableName = "vEjParsedReplenishments";
                }
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandText = @"select note_set_type_name,outerATM.description ,outerATM.location,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
                                    p.cassette1_notes, p.cassette2_notes,
                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
                                    p.cassette7_notes, (select max(trxn_datetime) from " + trxnTableName + " where atm_id = outerATM.atm_id and amount>0 )last_successful_trxn_at,last_trxn_at, " +
" note_set_type.denomination_type_1 * p.cassette1_notes +  " +
" note_set_type.denomination_type_2 * p.cassette2_notes + " +
" note_set_type.denomination_type_3 * p.cassette3_notes + " +
" note_set_type.denomination_type_4 * p.cassette4_notes + " +
" note_set_type.denomination_type_5 * p.cassette5_notes + " +
" note_set_type.denomination_type_6 * p.cassette6_notes + " +
" note_set_type.denomination_type_7 * p.cassette7_notes total ," +
" ( select max(rep_datetime) from " + replenishmentTableName + " where atm_id = outerATM.atm_id)last_replenishment_at,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id,r.parent_region_id,bank_logo " +
" ,  ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    " +
" and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenishment_at, " +

" ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
" and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount" +
 " from atm outerATM , " + cashPosTableName + " p ,note_set_type,region r " +
" where outerATM.atm_id = p.atm_id and outerATM.is_active = 1  and outerATM.is_atm = 1" +
" and outerATM.note_set_type_id = note_set_type.note_set_type_id " + Filter() +
"  and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.region_id = r.region_id";


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dsCashPositions dsTrxn = new dsCashPositions();
                adapter.Fill(dsTrxn.Tables[0]);
                PopulateDataTable(dsTrxn.Tables[0]);
                ReportDocument rptDoc = new ReportDocument();
                rptDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\CCMS\\CashPositionsRpt.rpt");
                rptDoc.SetDataSource(dsTrxn.Tables[0]);
                rptDoc.SummaryInfo.ReportTitle = "Cash Positions Report";
                rptDoc.SetParameterValue("FromDate", DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy HH:mm:ss", null));
                rptDoc.SetParameterValue("ToDate", DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy HH:mm:ss", null));
                rptDoc.SetParameterValue("GeneratedBy", ((AppUser)Session[SessionVars.user.ToString()]).UserLogin);
                rptDoc.SetParameterValue("total", dsTrxn.Tables[0].Rows.Count);
                rptDoc.SetParameterValue("suppressDetail", "0");
                string path = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\CashPositions_" + DateTime.Now.ToString("ddMMyyyy") + ".pdf";
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
        string Filter()
        {
            string filter = "";
            
            StringBuilder groupbuilder = new StringBuilder();
            foreach (ListItem item in groupList.Items)
            {
                if (item.Selected)
                {
                    if (groupbuilder.Length > 0)
                        groupbuilder.Append(",");
                    groupbuilder.Append(item.Value);
                }

            }
            if (groupbuilder.Length > 0)
                filter += " and note_set_type.note_set_type_id  in ( " + groupbuilder.ToString() + " ) ";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and last_trxn_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and last_trxn_at<= convert(datetime,'" + TextBox_ToDate.Text + "',103)";



            return filter;
        }
        private void BindGrid(string orderBy)
        {
            string trxnTableName = null;
            string cashPosTableName = "";
            string replenishmentTableName = "";
            try
            {
                ///<Summary>Edited by Ali Shah on 09th Oct, 2019
                ///To dynamically handling of cassettes more than 4
                EnableGridRespectiveColumns();

                // Data Archiving Work
                if (Request.QueryString["ej"] == null)
                {
                    trxnTableName = "vParsedTransaction";
                    cashPosTableName = "vCashPosition";
                    replenishmentTableName = "vReplenishment";
                }
                else
                {
                    trxnTableName = "vEjParsedTransactions";
                    cashPosTableName = "vEjCashPosition";
                    replenishmentTableName = "vEjParsedReplenishments";
                }
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                ///<Summary>
                ///Edited by Ali Shah on 09th May, 2019
                ///To apply multicurrency feature on column 'total'

//                cmd.CommandText = @"select note_set_type_name,outerATM.description ,outerATM.location,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
//                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
//                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
//                                    p.cassette1_notes, p.cassette2_notes,
//                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
//                                    p.cassette7_notes, p.purge_cassette1_notes,p.purge_cassette2_notes, p.purge_cassette3_notes,p.purge_cassette4_notes,(select max(trxn_datetime) from " + trxnTableName + " where atm_id = outerATM.atm_id and amount>0 )last_successful_trxn_at,last_trxn_at, " +
//                " note_set_type.denomination_type_1 * p.cassette1_notes +  " +
//                " note_set_type.denomination_type_2 * p.cassette2_notes + " +
//                " note_set_type.denomination_type_3 * p.cassette3_notes + " +
//                " note_set_type.denomination_type_4 * p.cassette4_notes + " +
//                " note_set_type.denomination_type_5 * p.cassette5_notes + " +
//                " note_set_type.denomination_type_6 * p.cassette6_notes + " +
//                " note_set_type.denomination_type_7 * p.cassette7_notes total ," +
//" note_set_type.denomination_type_1 * p.purge_cassette1_notes +  " +
//" note_set_type.denomination_type_2 * p.purge_cassette2_notes + " +
//" note_set_type.denomination_type_3 * p.purge_cassette3_notes + " +
//" note_set_type.denomination_type_4 * p.purge_cassette4_notes totalPurged ," +

//                " ( select max(rep_datetime) from " + replenishmentTableName + " where atm_id = outerATM.atm_id)last_replenishment_at,region_name,region.region_id,parent_region_id,bank_logo " +
//                " ,  ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    " +
//                " and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenishment_at, " +

//                " ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
//                " and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount, outerATM.location " +
//                 " from atm outerATM , " + cashPosTableName + " p ,note_set_type,region " +
//                " where outerATM.atm_id = p.atm_id and outerATM.is_active = 1  and outerATM.is_atm = 1 " +
//                " and outerATM.note_set_type_id = note_set_type.note_set_type_id " + Filter() +
//                "  and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.region_id = region.region_id" +
//                " order by " + orderBy;
                
                
                ///<Summary>
                ///Edited by Ali Shah.
                ///To dynamically handling of cassettes more than 4
//                cmd.CommandText = @"select note_set_type_name,outerATM.description ,outerATM.location,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
//                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
//                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
//                                    p.cassette1_notes, p.cassette2_notes,
//                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
//                                    p.cassette7_notes, p.purge_cassette1_notes,p.purge_cassette2_notes, p.purge_cassette3_notes,p.purge_cassette4_notes,(select max(trxn_datetime) from " + trxnTableName + " where atm_id = outerATM.atm_id and amount>0 )last_successful_trxn_at,last_trxn_at, " +
//                " note_set_type.denomination_type_1 * p.cassette1_notes +  " +
//                " note_set_type.denomination_type_2 * p.cassette2_notes + " +
//                " note_set_type.denomination_type_3 * p.cassette3_notes + " +
//                " note_set_type.denomination_type_4 * p.cassette4_notes + " +
//                " note_set_type.denomination_type_5 * p.cassette5_notes + " +
//                " note_set_type.denomination_type_6 * p.cassette6_notes + " +
//                " note_set_type.denomination_type_7 * p.cassette7_notes total ," +
//" note_set_type.denomination_type_1 * p.purge_cassette1_notes +  " +
//" note_set_type.denomination_type_2 * p.purge_cassette2_notes + " +
//" note_set_type.denomination_type_3 * p.purge_cassette3_notes + " +
//" note_set_type.denomination_type_4 * p.purge_cassette4_notes totalPurged ," +

//                " ( select max(rep_datetime) from " + replenishmentTableName + " where atm_id = outerATM.atm_id)last_replenishment_at,region_name,region.region_id,parent_region_id,bank_logo " +
//                " ,  ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    " +
//                " and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenishment_at, " +

//                " ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
//                " and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount, outerATM.location " + "," +
//                "dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title," +
//                  "note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4," +
//                  "p.cassette1_notes, p.cassette2_notes, p.cassette3_notes, p.cassette4_notes) total_text" +  
//                " from atm outerATM , " + cashPosTableName + " p ,note_set_type,region " +
//                " where outerATM.atm_id = p.atm_id and outerATM.is_active = 1  and outerATM.is_atm = 1 " +
//                " and outerATM.note_set_type_id = note_set_type.note_set_type_id " + Filter() +
//                "  and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.region_id = region.region_id" +
//                " order by " + orderBy;

                cmd.CommandText = @"select note_set_type_name,outerATM.description ,outerATM.location,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
                                    p.cassette1_notes, p.cassette2_notes,
                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
                                    p.cassette7_notes, p.purge_cassette1_notes,p.purge_cassette2_notes, p.purge_cassette3_notes,p.purge_cassette4_notes,p.purge_cassette5_notes, p.purge_cassette6_notes,p.purge_cassette7_notes,(select max(trxn_datetime) from " + trxnTableName + " where atm_id = outerATM.atm_id and amount>0 )last_successful_trxn_at,last_trxn_at, " +
                " note_set_type.denomination_type_1 * p.cassette1_notes +  " +
                " note_set_type.denomination_type_2 * p.cassette2_notes + " +
                " note_set_type.denomination_type_3 * p.cassette3_notes + " +
                " note_set_type.denomination_type_4 * p.cassette4_notes + " +
                " note_set_type.denomination_type_5 * p.cassette5_notes + " +
                " note_set_type.denomination_type_6 * p.cassette6_notes + " +
                " note_set_type.denomination_type_7 * p.cassette7_notes total ," +
" note_set_type.denomination_type_1 * p.purge_cassette1_notes +  " +
" note_set_type.denomination_type_2 * p.purge_cassette2_notes + " +
" note_set_type.denomination_type_3 * p.purge_cassette3_notes + " +
" note_set_type.denomination_type_4 * p.purge_cassette4_notes +  " +
" note_set_type.denomination_type_5 * p.purge_cassette5_notes + " +
" note_set_type.denomination_type_6 * p.purge_cassette6_notes + " +
" note_set_type.denomination_type_7 * p.purge_cassette7_notes totalPurged ," +

                " ( select max(rep_datetime) from " + replenishmentTableName + " where atm_id = outerATM.atm_id)last_replenishment_at,region_name,region.region_id,parent_region_id,bank_logo " +
                " ,  ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    " +
                " and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenishment_at, " +

                " ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
                " and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount, outerATM.location " + "," +
                "dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title," +
                  "note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4," +
                  "p.cassette1_notes, p.cassette2_notes, p.cassette3_notes, p.cassette4_notes) total_text" +
                " from atm outerATM , " + cashPosTableName + " p ,note_set_type,region " +
                " where outerATM.atm_id = p.atm_id and outerATM.is_active = 1  and outerATM.is_atm = 1 " +
                " and outerATM.note_set_type_id = note_set_type.note_set_type_id " + Filter() +
                "  and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.region_id = region.region_id" +
                " order by " + orderBy;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataTable dtCashPositions = new DataTable();
                adapter.Fill(dtCashPositions);

                Cache["CashPositions"] = dtCashPositions;
                //cmd.Connection.Close();
                this.GridView_CashPositions.DataSource = dtCashPositions;
                this.GridView_CashPositions.DataBind();
                Literal_count.Text = dtCashPositions.Rows.Count + " records.";

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("bind cash position grid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }
        private void PopulateDataTable(DataTable dt)
        {
            int region_id = 0;
            int id = -1;

            DataTable dtRegions = new DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region where parent_region_id is not null";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtRegions);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                region_id = int.Parse(dt.Rows[i][14].ToString()); //parent

                while (true)
                {
                    DataRow[] innerArray = dtRegions.Select("region_id=" + region_id);
                    if (bool.Parse(innerArray[0][2].ToString()))
                    {
                        id = int.Parse(innerArray[0][0].ToString());
                        break;
                    }
                    else
                        region_id = int.Parse(innerArray[0][1].ToString());

                }
                //organization id found
                Region region = Region.LoadRegionByPk(id);
                dt.Rows[i][15] = region.RegionName;
                dt.Rows[i][17] = region.BankLogo;

            }

        }
        public System.Web.UI.WebControls.SortDirection GridViewSortDirection
        {
            get
            {



                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = System.Web.UI.WebControls.SortDirection.Ascending;

                //return (SortDirection)ViewState["sortDirection"];
                return (ViewState["sortDirection"].ToString() == " asc" ? System.Web.UI.WebControls.SortDirection.Ascending : System.Web.UI.WebControls.SortDirection.Descending);
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected void GridView_CashPositions_Sorting(object sender, GridViewSortEventArgs e)
        {


            if (ViewState["SortExpression"].ToString() == e.SortExpression)
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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            //System.Data.DataTable dt = (System.Data.DataTable)Cache[CacheVars.dtCashOrders.ToString()];
            //DataView dv = new DataView(dt);
            //GridViewSortDirection = (GridViewSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
            //dv.Sort = e.SortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            //GridView_CashOrders.DataSource = dv;
            //GridView_CashOrders.DataBind();

        }




        protected void GridView_CashPositions_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            System.Data.DataTable dt = (System.Data.DataTable)Cache["CashPositions"];
            DataView dv = new DataView(dt);
            if (GridViewSortExpression != "")
                dv.Sort = GridViewSortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            this.GridView_CashPositions.DataSource = dv;

            this.GridView_CashPositions.PageIndex = e.NewPageIndex;
            this.GridView_CashPositions.DataBind();

        }
        private string GridViewSortExpression
        {
            get
            { return ViewState["SortExpression"] as string ?? string.Empty; }
            set
            { ViewState["SortExpression"] = value; }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindNoteSetType();
            Button_search_Click(this, null);
        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void EnableGridRespectiveColumns()
        {
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexDenomination = -1;
            int columnIndexRemaining = -1;
            int columnIndexPurged = -1;
            columnIndexDenomination = GetColumnIndexByName(this.GridView_CashPositions, "Type 5");
            columnIndexRemaining = columnIndexDenomination >=0 ? (columnIndexDenomination + 1) : -1;     // GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            columnIndexPurged = columnIndexDenomination >=0 ? (columnIndexDenomination + 2) : -1;// GetColumnIndexByName(this.GridView_CashPositions, "Remaining");
            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexDenomination >= 0)
                {
                    this.GridView_CashPositions.Columns[columnIndexDenomination].Visible = true;
                    columnIndexDenomination += 3;
                }

                if (columnIndexRemaining >= 0)
                {
                    this.GridView_CashPositions.Columns[columnIndexRemaining].Visible = true;
                    columnIndexRemaining += 3;
                }

                if (columnIndexPurged >= 0)
                {
                    this.GridView_CashPositions.Columns[columnIndexPurged].Visible = true;
                    columnIndexPurged += 3;
                }
            }
        }



    }
}