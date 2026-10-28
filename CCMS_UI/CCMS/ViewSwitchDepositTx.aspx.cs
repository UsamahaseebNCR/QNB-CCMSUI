using Avanza.iSuite.DAL;
using Avanza.iSuite.UI;
using ClosedXML.Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class ViewSwitchDepositTx : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (!CheckRight((int)Permissions.ViewUploadedData))
                    Response.Redirect("../admin/aspx/AccessDenied.aspx");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "transaction_date";


                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");

                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        string Filter()
        {
            string filter = "";
            if (TextBox_ATMID.Text.Length > 0)
                filter += " and sd.atm_id like '%" + TextBox_ATMID.Text + "%'";
            if (TextBox_PAN.Text.Length > 0)
                filter += " and card_number like '%" + TextBox_PAN.Text + "%'";
            if (TextBox_AcctNo.Text.Length > 0)
                filter += " and customer_account_no = '" + TextBox_AcctNo.Text + "'";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and convert(datetime,transaction_date,103) >= convert(datetime, '" + DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null).ToString("yyyyMMdd") + "', 103)  ";
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and convert(datetime,transaction_date,103) <= convert(datetime, '" + DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null).ToString("yyyyMMdd") + "', 103) ";
            if (TextBox_Amount.Text.Length > 0)
                filter += " and transaction_amount = '" + TextBox_Amount.Text + "'";
            if (TextBox_TSN.Text.Length > 0)
                filter += " and transaction_sequence = '" + TextBox_TSN.Text + "'";
            if (TextBox_Response.Text.Length > 0)
                filter += " and transaction_response like '%" + TextBox_Response.Text + "%'";

            if (Session[SessionVars.SelectedATMs.ToString()] != "-1")
                filter += "and a.atm_id in ( " + Session[SessionVars.SelectedATMs.ToString()] + ")";


            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = "select sd.atm_id [ATM ID] , sd.card_number [Card Number], sd.customer_account_no [Account No.] , sd.transaction_date [Date], sd.transaction_time [Time], sd.transaction_amount [Amount], sd.transaction_currency [Currency],sd.transaction_sequence [Seq] , sd.transaction_type [Type] , sd.transaction_response [Response Code], sd.card_network [Card Network], sd.card_issuer [Card Issuer] from VReconciliationBnaSwitchData sd inner join atm a on  sd.atm_id = a.title  where 1=1 " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtGrid = new DataTable();
                adapter.Fill(dtGrid);
                cmd.Connection.Close();
                GridView_Trxn.DataSource = dtGrid;
                GridView_Trxn.DataBind();
                Literal_count.Text = dtGrid.Rows.Count + " records.";

                if (Cache["dataSwitchDepositsTx"] != null)
                    Cache.Remove("dataSwitchDepositsTx");

                this.Cache.Add("dataSwitchDepositsTx", dtGrid, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
               new TimeSpan(0, 1, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(DeleteFile));

            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>An error occured." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }
        static void DeleteFile(string key, object val, CacheItemRemovedReason reas)
        {
            File.Delete(key);
        }
        protected void GridView_Trxn_Sorting(object sender, GridViewSortEventArgs e)
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




        protected void GridView_Trxn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Trxn.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_ATMID.Text = "";
            TextBox_AcctNo.Text = "";
            // DropDownList_Source.SelectedIndex = 0;
            TextBox_Amount.Text = "";
            TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_TSN.Text = "";
            TextBox_Response.Text = "";
            TextBox_PAN.Text = "";
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }

        protected void ExportToExcel(DataTable dt, string fileName)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(fileName);
                    worksheet.Cell(1, 1).InsertTable(dt); // Insert DataTable starting from Cell A1

                    worksheet.Columns().AdjustToContents(); // Auto-fit the columns

                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment; filename={fileName}.xlsx");

                    // Write the Excel workbook to the response stream
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.WriteTo(Response.OutputStream);
                        Response.AppendHeader("Set-Cookie", "fileDownload=true; path=/");
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else
            {
                Literal_ok.Text = "<li>No data available to export.</li>";
            }
        }


        protected void Button_ExporttoCSV_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_ExporttoCSV_Click");
            try
            {
                ExportToExcel((DataTable)Cache["dataSwitchDepositsTx"], "SwitchDepositsTx");
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
    }
}