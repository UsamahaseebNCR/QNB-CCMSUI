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
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;

using System.Text;
using System.Drawing;

namespace CCMSUI.CCMS
{
    public partial class CashOrderMonitoring : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCashOrderMonitoringHistory))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";

                TextBox_Date.Text = DateTime.Now.ToString("dd/MM/yyyy");
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                //  BindGrid("atm_id asc");
            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }

        protected void GridView_CashOrderMonitoring_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_CashOrderMonitoring.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            // BindGridWithCache();
        }

        protected void GridView_CashOrderMonitoring_Sorting(object sender, GridViewSortEventArgs e)
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
            //BindGridWithCache();
        }

        string Filter()
        {
            string filter = "";

            if (DropDownList_status.SelectedIndex == 1)
                filter += " and current_order_remaining_amount =0 ";

            if (DropDownList_status.SelectedIndex == 2)
                filter += " and current_order_remaining_amount >0 and current_order_suggested_amount <> current_order_remaining_amount";
            if (DropDownList_status.SelectedIndex == 3)
                filter += " and current_order_remaining_amount =current_order_suggested_amount ";
            if (DropDownList_status.SelectedIndex == 4)
                filter += " and current_order_remaining_amount <0 ";

            return filter;
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

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select top 1000 outerATM.atm_id,outerATM.title, case outerATM.is_healthy when 1 then 'Up' else 'Down' end as Status, 
                                    current_order_received_at,current_order_executed_at,
                                        current_order_suggested_amount, current_order_remaining_amount, current_order_id
                                         from cash_order_monitoring inner join ATM outerATM on cash_order_monitoring.atm_id = outerATM.atm_id 
                                         inner join cash_orders on cash_orders.cash_order_id = cash_order_monitoring.current_order_id 
                                        where outerATM.is_active = 1 and cash_orders.is_cancelled=0  " +
                                    " and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " +
                                    " and cash_order_monitoring.replenishment_datetime >=convert(datetime,'" + TextBox_Date.Text  + "',103) and cash_order_monitoring.replenishment_datetime <=convert(datetime,'" + TextBox_Date.Text  + " 23:59:59',103) " +
                                     " order by title";


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

//                cmd.CommandText = @"select top 1000 ATM.atm_id,ATM.title, case ATM.is_healthy when 1 then 'Up' else 'Down' end as Status, 
//                                current_order_received_at,current_order_delivered_at,current_order_executed_at,
//                                    current_order_suggested_amount , current_order_remaining_amount
//                                    from cash_order_monitoring,ATM 
//                                    where is_active = 1 and cash_order_monitoring.atm_id = ATM.atm_id
//                                    and cash_order_datetime = CONVERT(datetime,'" + TextBox_Date.Text + "',103) " +
//                                    " and ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + Filter() + " order by ATM.atm_id";

//                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd);
//                DataTable dt2 = new DataTable();
//                adapter2.Fill(dt2);


//                DataTable dtCashOrderMonitoring = MergeDataTables(dt1, dt2);
//                dtCashOrderMonitoring.DefaultView.Sort = orderby;
//                //Cache["dtMonitoring"] = dtCashOrderMonitoring;
                this.GridView_CashOrderMonitoring.DataSource = dt;
                this.GridView_CashOrderMonitoring.DataBind();

                Literal_count.Text = dt.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            { task.EndTask(); }
        }
        private void BindGridWithCache()
        {
            DataTable dtCashOrderMonitoring = (DataTable)Cache["dtMonitoring"];
            dtCashOrderMonitoring.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
            this.GridView_CashOrderMonitoring.DataSource = dtCashOrderMonitoring;
            this.GridView_CashOrderMonitoring.DataBind();
            Literal_count.Text = dtCashOrderMonitoring.Rows.Count + " records.";

        }


        private DataTable MergeDataTables(DataTable dtPreviousDay, DataTable dtCurrentDay)
        {
            int orderPending = 0;
            int orderExecuted = 0;
            int orderOverConsumed = 0;
            int orderPartiallyExecuted = 0;
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
                
                if (int.Parse(dtCurrentDay.Rows[i][7].ToString()) == int.Parse(dtCurrentDay.Rows[i][6].ToString()))
                    newDataRow[13] = "Pending";

                else if (int.Parse(dtCurrentDay.Rows[i][7].ToString()) < 0)
                    newDataRow[13] = "Negative Suspect";

                else if (int.Parse(dtCurrentDay.Rows[i][7].ToString()) ==0)
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


                if (lastRecords.Length > 0)
                {
                    //for (int j = 0; j < lastRecords.Length; j++)
                    //{
                    newDataRow[2] = lastRecords[lastRecords.Length - 1].ItemArray[3];
                    newDataRow[3] = lastRecords[lastRecords.Length - 1].ItemArray[4];
                    newDataRow[4] = lastRecords[lastRecords.Length - 1].ItemArray[5];
                    newDataRow[5] = lastRecords[lastRecords.Length - 1].ItemArray[7];
                    newDataRow[6] = lastRecords[lastRecords.Length - 1].ItemArray[6];


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
        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        }

    }
}