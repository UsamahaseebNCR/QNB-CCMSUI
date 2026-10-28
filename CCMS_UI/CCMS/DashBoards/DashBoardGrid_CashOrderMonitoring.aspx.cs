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

using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Drawing;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_CashOrderMonitoring : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCashOrderDashBoard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "title ";
                ViewState["sortDirection"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }

        }

        private DataTable MergeDataTables(DataTable dtPreviousDay, DataTable dtCurrentDay)
        {            
            DataTable dtCashOrderMonitoring = new DataTable();

            dtCashOrderMonitoring.Columns.Add("title");
            dtCashOrderMonitoring.Columns.Add("Status");
            dtCashOrderMonitoring.Columns.Add("last_order_received_at", typeof(DateTime));
            dtCashOrderMonitoring.Columns.Add("last_order_delivered_at", typeof(DateTime));
            dtCashOrderMonitoring.Columns.Add("last_order_executed_at", typeof(DateTime));
            dtCashOrderMonitoring.Columns.Add("last_order_remaining_amount");
            dtCashOrderMonitoring.Columns.Add("last_order_suggested_amount");
            dtCashOrderMonitoring.Columns.Add("last_order_status");
            

            dtCashOrderMonitoring.Columns.Add("current_order_received_at", typeof(DateTime));
            dtCashOrderMonitoring.Columns.Add("current_order_delivered_at", typeof(DateTime));
            dtCashOrderMonitoring.Columns.Add("current_order_executed_at", typeof(DateTime));
            dtCashOrderMonitoring.Columns.Add("current_order_remaining_amount");
            dtCashOrderMonitoring.Columns.Add("current_order_suggested_amount");
            dtCashOrderMonitoring.Columns.Add("current_order_status");
            

            dtCashOrderMonitoring.Columns.Add("current_cash_positions_remaining_amount");
            dtCashOrderMonitoring.Columns.Add("last_order_current_order_id");
            dtCashOrderMonitoring.Columns.Add("current_order_current_order_id");

            

            for (int i = 0; i < dtCurrentDay.Rows.Count; i++)
            {
                //if (int.Parse(dtCurrentDay.Rows[i][6].ToString()) == int.Parse(dtCurrentDay.Rows[i][7].ToString()))
                //    orderPending++;
                //else if (int.Parse(dtCurrentDay.Rows[i][6].ToString()) == 0)
                //    orderExecuted++;
                //else if (int.Parse(dtCurrentDay.Rows[i][6].ToString()) < 0)
                //    orderOverConsumed++;
                //else if (int.Parse(dtCurrentDay.Rows[i][6].ToString()) != int.Parse(dtCurrentDay.Rows[i][7].ToString()))
                //    orderPartiallyExecuted++;

                DataRow[] lastRecords = dtPreviousDay.Select("atm_id = " + dtCurrentDay.Rows[i][0]);
               
                
                
                DataRow newDataRow = dtCashOrderMonitoring.NewRow();
                newDataRow[0] = dtCurrentDay.Rows[i][1];
                newDataRow[1] = dtCurrentDay.Rows[i][2];
                newDataRow[16] =dtCurrentDay.Rows[i][8];

                if (int.Parse(dtCurrentDay.Rows[i][7].ToString()) == int.Parse(dtCurrentDay.Rows[i][6].ToString()))
                    newDataRow[13] = "Pending";

                else if (int.Parse(dtCurrentDay.Rows[i][7].ToString()) < 0)
                    newDataRow[13] = "Negative Suspect";

                else if (int.Parse(dtCurrentDay.Rows[i][7].ToString()) == 0)
                    newDataRow[13] = "Completed";

                else if (int.Parse(dtCurrentDay.Rows[i][7].ToString()) > 0)
                    newDataRow[13] = "Positive Suspect";


                    



                newDataRow[8] = dtCurrentDay.Rows[i][3];
                newDataRow[9] = dtCurrentDay.Rows[i][4];
                newDataRow[10] = dtCurrentDay.Rows[i][5];

                newDataRow[11] = dtCurrentDay.Rows[i][7];
                newDataRow[12] = dtCurrentDay.Rows[i][6];
                //6 =sug 7 = rem DT
                // 10 = rem 11 sugg

                CashPosition cashPosition = CashPosition.LoadCashPosition("atm_id =" + dtCurrentDay.Rows[i][0] + " and last_trxn_at >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) " +
               " and last_trxn_at <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)");

                if (cashPosition != null)
                {
                    NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(Atm.LoadAtmByPk(int.Parse(dtCurrentDay.Rows[i][0].ToString())).NoteSetTypeId);

                    newDataRow[14] = cashPosition.Cassette1Notes.Value * noteSetType.DenominationType1.Value +
                    cashPosition.Cassette2Notes.Value * noteSetType.DenominationType2.Value +
                    cashPosition.Cassette3Notes.Value * noteSetType.DenominationType3.Value +
                    cashPosition.Cassette4Notes.Value * noteSetType.DenominationType4.Value +
                    cashPosition.Cassette5Notes.Value * noteSetType.DenominationType5.Value +
                    cashPosition.Cassette6Notes.Value * noteSetType.DenominationType6.Value +
                    cashPosition.Cassette7Notes.Value * noteSetType.DenominationType7.Value;
                }    





                if (lastRecords.Length > 0)
                {
                    //for (int j = 0; j < lastRecords.Length; j++)
                    //{
                    newDataRow[2] = lastRecords[lastRecords.Length - 1].ItemArray[3];
                    newDataRow[3] = lastRecords[lastRecords.Length - 1].ItemArray[4];
                    newDataRow[4] = lastRecords[lastRecords.Length - 1].ItemArray[5];
                    newDataRow[5] = lastRecords[lastRecords.Length - 1].ItemArray[7];
                    newDataRow[6] = lastRecords[lastRecords.Length - 1].ItemArray[6];
                    newDataRow[15] = lastRecords[lastRecords.Length - 1].ItemArray[8];

                    if (int.Parse(newDataRow[5].ToString()) == int.Parse(newDataRow[6].ToString()))
                        newDataRow[7] = "Pending";

                    else if (int.Parse(newDataRow[5].ToString()) < 0)
                        newDataRow[7] = "Negative Suspect";

                    else if (int.Parse(newDataRow[5].ToString()) == 0)
                        newDataRow[7] = "Completed";

                    else if (int.Parse(newDataRow[5].ToString()) > 0)
                        newDataRow[7] = "Positive Suspect";



                    //5 = rem, sug =6  New DT 
                }

                //}
                dtCashOrderMonitoring.Rows.Add(newDataRow);

            }

            //Literal_executed.Text = orderExecuted + " records.";
            //Literal_partial.Text = orderPartiallyExecuted + " records.";
            //Literal_not.Text = orderPending + " records.";
            //Literal_over.Text = orderOverConsumed + " records.";
            return dtCashOrderMonitoring;
        }
        //private DataTable MergeDataTables(DataTable dtPreviousDay, DataTable dtCurrentDay) on 25th march 2011
        //{
        //    int orderPending = 0;
        //    int orderExecuted = 0;
        //    int orderOverConsumed = 0;
        //    int orderPartiallyExecuted = 0;
        //    DataTable dtCashOrderMonitoring = new DataTable();

        //    dtCashOrderMonitoring.Columns.Add("title");
        //    dtCashOrderMonitoring.Columns.Add("Status");
        //    dtCashOrderMonitoring.Columns.Add("last_order_received_at", typeof(DateTime));
        //    dtCashOrderMonitoring.Columns.Add("last_order_delivered_at", typeof(DateTime));
        //    dtCashOrderMonitoring.Columns.Add("last_order_executed_at", typeof(DateTime));
        //    dtCashOrderMonitoring.Columns.Add("last_order_remaining_amount");
        //    dtCashOrderMonitoring.Columns.Add("current_order_received_at", typeof(DateTime));
        //    dtCashOrderMonitoring.Columns.Add("current_order_delivered_at", typeof(DateTime));
        //    dtCashOrderMonitoring.Columns.Add("current_order_executed_at", typeof(DateTime));
        //    dtCashOrderMonitoring.Columns.Add("current_order_remaining_amount");
        //    dtCashOrderMonitoring.Columns.Add("current_order_suggested_amount");


        //    for (int i = 0; i < dtCurrentDay.Rows.Count; i++)
        //    {
        //        if (int.Parse(dtCurrentDay.Rows[i][6].ToString()) == int.Parse(dtCurrentDay.Rows[i][7].ToString()))
        //            orderPending++;
        //        else if (int.Parse(dtCurrentDay.Rows[i][6].ToString()) == 0)
        //            orderExecuted++;
        //        else if (int.Parse(dtCurrentDay.Rows[i][6].ToString()) < 0)
        //            orderOverConsumed++;
        //        else if (int.Parse(dtCurrentDay.Rows[i][6].ToString()) != int.Parse(dtCurrentDay.Rows[i][7].ToString()))
        //            orderPartiallyExecuted++;



        //        DataRow[] lastRecords = dtPreviousDay.Select("atm_id = " + dtCurrentDay.Rows[i][0]);
        //        DataRow newDataRow = dtCashOrderMonitoring.NewRow();
        //        newDataRow[0] = dtCurrentDay.Rows[i][1];
        //        newDataRow[1] = dtCurrentDay.Rows[i][2];
        //        newDataRow[6] = dtCurrentDay.Rows[i][3];
        //        newDataRow[7] = dtCurrentDay.Rows[i][4];
        //        newDataRow[8] = dtCurrentDay.Rows[i][5];
        //        newDataRow[9] = dtCurrentDay.Rows[i][6];
        //        newDataRow[10] = dtCurrentDay.Rows[i][7];
        //        if (lastRecords.Length > 0)
        //        {
        //            //for (int j = 0; j < lastRecords.Length; j++)
        //            //{
        //            newDataRow[2] = lastRecords[lastRecords.Length - 1].ItemArray[3];
        //            newDataRow[3] = lastRecords[lastRecords.Length - 1].ItemArray[4];
        //            newDataRow[4] = lastRecords[lastRecords.Length - 1].ItemArray[5];
        //            newDataRow[5] = lastRecords[lastRecords.Length - 1].ItemArray[6];
        //        }

        //        //}
        //        dtCashOrderMonitoring.Rows.Add(newDataRow);

        //    }

        //    //Literal_executed.Text = orderExecuted + " records.";
        //    //Literal_partial.Text = orderPartiallyExecuted + " records.";
        //    //Literal_not.Text = orderPending + " records.";
        //    //Literal_over.Text = orderOverConsumed + " records.";
        //    return dtCashOrderMonitoring;
        //}

        private string Filter()
        {
            //AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
            //if (appSetting.ThresholdForCashorder == -1)
            //    return " and cash_order_datetime <convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)";
            //else

            return " and vCashOrdersMonitoring.replenishment_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and vCashOrdersMonitoring.replenishment_datetime <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)";
      
//            return @" and replenishment_datetime in 
//      
//                              (
//										select MAX(replenishment_datetime) 
//										from cash_order_monitoring 
//										where atm_id = outerATM.atm_id
//										and replenishment_datetime < convert(datetime,'" + DateTime.Today.AddDays( (appSetting.ThresholdForCashorder==-1?0:-appSetting.ThresholdForCashorder.Value)).ToString("dd/MM/yyyy")+"',103))";


            //Changed criteria as per ADCB replenishment datetime





                //return " and cash_order_datetime < convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForCashorder.Value).ToString("dd/MM/yyyy")+"',103)"; 

        }

        private string Filter1()
        {
            AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
            if (appSetting.ThresholdForCashorder == -1)
                return " and replenishment_datetime >=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) and replenishment_datetime <=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103)";
            else
                return " and replenishment_datetime >=convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForCashorder.Value).ToString("dd/MM/yyyy") + "',103) and replenishment_datetime <=convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForCashorder.Value).ToString("dd/MM/yyyy") + " 23:59:59',103)"; 

        }
 
        protected void GridView_CashOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //if (((DataRowView)e.Row.DataItem).Row["last_order_status"].ToString() == "Pending")
                //{
                //    e.Row.Cells[8].ForeColor = Color.White;
                //    e.Row.Cells[8].BackColor = Color.Orange;
                //}
                //else if (((DataRowView)e.Row.DataItem).Row["last_order_status"].ToString() == "Completed")
                //{
                //    e.Row.Cells[8].ForeColor = Color.White;
                //    e.Row.Cells[8].BackColor = Color.Green;
                //}
                //else if (((DataRowView)e.Row.DataItem).Row["last_order_status"].ToString().Contains("Suspect"))
                //{
                //    e.Row.Cells[8].ForeColor = Color.White;
                //    e.Row.Cells[8].BackColor = Color.Red;
                //}
                //if (((DataRowView)e.Row.DataItem).Row["current_order_status"].ToString() == "Pending")
                //{
                //    e.Row.Cells[e.Row.Cells.Count-1].ForeColor = Color.White;
                //    e.Row.Cells[e.Row.Cells.Count - 1].BackColor = Color.Orange;
                //}
                //else if (((DataRowView)e.Row.DataItem).Row["current_order_status"].ToString() == "Completed")
                //{
                //    e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = Color.White;
                //    e.Row.Cells[e.Row.Cells.Count - 1].BackColor = Color.Green;
                //}
                //else if (((DataRowView)e.Row.DataItem).Row["current_order_status"].ToString().Contains("Suspect"))
                //{
                //    e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = Color.White;
                //    e.Row.Cells[e.Row.Cells.Count - 1].BackColor = Color.Red;
                //}

                string remainingAmount = ((DataRowView)e.Row.DataItem).Row["current_order_remaining_amount"].ToString();
                string suggestedAmount = ((DataRowView)e.Row.DataItem).Row["current_order_suggested_amount"].ToString();

                if (remainingAmount == suggestedAmount)
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "Pending";
                    e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = Color.White;
                    e.Row.Cells[e.Row.Cells.Count - 1].BackColor = Color.Orange;
                }

                else if (decimal.Parse(remainingAmount) < 0)
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "Negative Suspect";
                    e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = Color.White;
                    e.Row.Cells[e.Row.Cells.Count - 1].BackColor = Color.Red;
                }
                else if (decimal.Parse(remainingAmount) == 0)
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "Completed";
                    e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = Color.White;
                    e.Row.Cells[e.Row.Cells.Count - 1].BackColor = Color.Green;
            
                }
                else if (decimal.Parse(remainingAmount) > 0)
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "Positive Suspect";
                    e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = Color.White;
                    e.Row.Cells[e.Row.Cells.Count - 1].BackColor = Color.Red;
                }
                

            







                //Task existingTask = null;
                //string currentOrderID = this.GridView_CashOrder.DataKeys[e.Row.DataItemIndex].Values["current_order_current_order_id"].ToString();
                //string lastOrderID = this.GridView_CashOrder.DataKeys[e.Row.DataItemIndex].Values["last_order_current_order_id"].ToString();
                //if (currentOrderID.Length > 0)
                //{
                //    existingTask = Task.LoadTask("cash_order_id=" + currentOrderID);
                //    if ((existingTask != null) && existingTask.Status.Contains("cancelled"))
                //    {
                //        e.Row.Cells[10].BackColor = Color.Red;
                //        e.Row.Cells[3].Text = "";
                //        e.Row.Cells[4].Text = "";
                //        e.Row.Cells[5].Text = "";
                //        e.Row.Cells[6].Text = "";
                //        e.Row.Cells[7].Text = "";
                //        e.Row.Cells[8].Text = "";
                //    }
                //}
                //if (lastOrderID.Length > 0)
                //{
                //    existingTask = Task.LoadTask("cash_order_id=" + lastOrderID);
                //    if ((existingTask != null) && existingTask.Status.Contains("cancelled"))
                //    {
                //        ((System.Web.UI.WebControls.Image)e.Row.FindControl("Image_OrderCancelled")).Attributes.Add("title", "header=[Order Task Details] body=[OC order cancelled because another order downloaded for the same day.]");
                //    }
                //}
            }

        }



        private void BindGrid(string orderBy)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {


                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select top 1000 outerATM.atm_id,outerATM.title, case outerATM.is_healthy when 1 then 'Up' else 'Down' end as Status, current_order_received_at,current_order_executed_at,
current_order_suggested_amount, current_order_remaining_amount, current_order_id, (SELECT TOP 1 last_tsn from vEjParsedReplenishments r WHERE r.atm_id = vCashOrders.atm_id AND r.rep_datetime < vCashOrders.cash_order_datetime order by r.rep_datetime desc) tsn, 
(dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4, cassette1_suggested_notes , cassette2_suggested_notes, cassette3_suggested_notes, cassette4_suggested_notes)) current_order_suggested_amount_text,
(dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4, cassette1_remaining_notes , cassette2_suggested_notes, cassette3_suggested_notes, cassette4_suggested_notes)) current_order_remaining_amount_text
from vCashOrdersMonitoring inner join ATM outerATM on vCashOrdersMonitoring.atm_id = outerATM.atm_id 
inner join vCashOrders on vCashOrders.cash_order_id = vCashOrdersMonitoring.current_order_id
inner join note_set_type on outerATM.note_set_type_id = note_set_type.note_set_type_id
    where outerATM.is_active = 1 and vCashOrders.is_cancelled=0 " + Filter() +
                                     " order by outerATM.title";
                

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt1 = new DataTable();
                adapter.Fill(dt1);

//                cmd.CommandText = @"select top 1000 ATM.atm_id,ATM.title, case ATM.is_healthy when 1 then 'Up' else 'Down' end as Status, 
//                                current_order_received_at,current_order_delivered_at,current_order_executed_at,
//                                    current_order_suggested_amount , current_order_remaining_amount, current_order_id 
//                                    from cash_order_monitoring,ATM 
//                                    where is_active = 1 and cash_order_monitoring.atm_id = ATM.atm_id " + Filter1() +
//                                    " order by ATM.atm_id";

//                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd);
//                DataTable dt2 = new DataTable();
//                adapter2.Fill(dt2);


                //DataTable dtCashOrderMonitoring = dt1;// MergeDataTables(dt1, dt2);
                //dtCashOrderMonitoring.DefaultView.Sort = orderBy;



                //DataTable dtMonitoring = new DataTable();


                //dtMonitoring.Columns.Add("title");
                //dtMonitoring.Columns.Add("Status");
                //dtMonitoring.Columns.Add("last_order_received_at", typeof(DateTime));
                //dtMonitoring.Columns.Add("last_order_delivered_at", typeof(DateTime));
                //dtMonitoring.Columns.Add("last_order_executed_at", typeof(DateTime));
                //dtMonitoring.Columns.Add("last_order_remaining_amount");
                //dtMonitoring.Columns.Add("current_order_received_at", typeof(DateTime));
                //dtMonitoring.Columns.Add("current_order_delivered_at", typeof(DateTime));
                //dtMonitoring.Columns.Add("current_order_executed_at", typeof(DateTime));
                //dtMonitoring.Columns.Add("current_order_remaining_amount");
                //dtMonitoring.Columns.Add("current_order_suggested_amount");





                //dt2 = new DataTable();
                //dt2 = dtMonitoring.Clone();
                //for (int i = 0; i < dtCashOrderMonitoring.Rows.Count / 2; i++)
                //{
                //    DataRow dr = dtMonitoring.NewRow();
                //    dr[0] = dtCashOrderMonitoring.Rows[i][0];
                //    dr[1] = dtCashOrderMonitoring.Rows[i][1];
                //    dr[2] = dtCashOrderMonitoring.Rows[i][2];
                //    dr[3] = dtCashOrderMonitoring.Rows[i][3];
                //    dr[4] = dtCashOrderMonitoring.Rows[i][4];
                //    dr[5] = dtCashOrderMonitoring.Rows[i][5];
                //    dr[6] = dtCashOrderMonitoring.Rows[i][6];
                //    dr[7] = dtCashOrderMonitoring.Rows[i][7];
                //    dr[8] = dtCashOrderMonitoring.Rows[i][8];
                //    dr[9] = dtCashOrderMonitoring.Rows[i][9];
                //    dr[10] = dtCashOrderMonitoring.Rows[i][10];


                //    dtMonitoring.Rows.Add(dr);

                //}
                //for (int i = dtCashOrderMonitoring.Rows.Count / 2; i < dtCashOrderMonitoring.Rows.Count; i++)
                //{
                //    DataRow dr = dt2.NewRow();
                //    dr[0] = dtCashOrderMonitoring.Rows[i][0];
                //    dr[1] = dtCashOrderMonitoring.Rows[i][1];
                //    dr[2] = dtCashOrderMonitoring.Rows[i][2];
                //    dr[3] = dtCashOrderMonitoring.Rows[i][3];
                //    dr[4] = dtCashOrderMonitoring.Rows[i][4];
                //    dr[5] = dtCashOrderMonitoring.Rows[i][5];
                //    dr[6] = dtCashOrderMonitoring.Rows[i][6];
                //    dr[7] = dtCashOrderMonitoring.Rows[i][7];
                //    dr[8] = dtCashOrderMonitoring.Rows[i][8];
                //    dr[9] = dtCashOrderMonitoring.Rows[i][9];
                //    dr[10] = dtCashOrderMonitoring.Rows[i][10];

                //    dt2.Rows.Add(dr);

                //}


                GridView_CashOrder.DataSource = dt1;
                //GridView2.DataSource = dt2;
                GridView_CashOrder.DataBind();
                //GridView2.DataBind();


            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";

            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();

                task.EndTask();

            }


        }
        protected void GridView_CashOrder_Sorting(object sender, GridViewSortEventArgs e)
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

        }
    }
}