using Avanza.iSuite.DAL;
using Avanza.iSuite.UI;
using ClosedXML.Excel;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTable = System.Data.DataTable;

namespace CCMS
{
    public partial class ReplenishmentReturn : BasePage
    {
        static DataTable dtRepReturn = new DataTable();
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!Page.IsPostBack)
            {
                dtRepReturn = new DataTable();
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";
                ViewState["sortDirectionCC"] = " asc";
                ViewState["SortExpressionCC"] = "title";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");


                BindGrid();

            }
        }

        string Filter()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and rep_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and rep_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            return filter;
        }




        private void BindGrid()
        {
            try
            {
                string orderBy = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
                string orderByCC = ViewState["SortExpressionCC"].ToString() + ViewState["sortDirectionCC"].ToString();

                string ATMIds = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);


                using (SqlCommand cmd = ConnectionFactory.GetNewCommand(true))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"SELECT 
    rep_datetime AS ""Replenishment Date and Time"",
    title AS ""ATM Title"",

    denomination_type_1 AS ""Denomination Type 1"",
    cash_added_type_1 AS ""Cash Added Type 1"", 
    remaining_type_1 AS ""Remaining Type 1"",
    purged_type_1 AS ""Purged Type 1"",
    return_type_1 AS ""Return Type 1"",
    return_amount_type_1 AS ""Return Amount Type 1"",

    cash_added_rc_type_1 AS ""Cash Added RC Type 1"", 
    remaining_rc_type_1 AS ""Remaining RC Type 1"",
    purged_rc_type_1 AS ""Purged RC Type 1"",
    return_rc_type_1 AS ""Return RC Type 1"",
    return_rc_amount_type_1 AS ""Return RC Amount Type 1"",

    denomination_type_2 AS ""Denomination Type 2"",
    cash_added_type_2 AS ""Cash Added Type 2"", 
    remaining_type_2 AS ""Remaining Type 2"",
    purged_type_2 AS ""Purged Type 2"",
    return_type_2 AS ""Return Type 2"",
    return_amount_type_2 AS ""Return Amount Type 2"",

    cash_added_rc_type_2 AS ""Cash Added RC Type 2"", 
    remaining_rc_type_2 AS ""Remaining RC Type 2"",
    purged_rc_type_2 AS ""Purged RC Type 2"",
    return_rc_type_2 AS ""Return RC Type 2"",
    return_rc_amount_type_2 AS ""Return RC Amount Type 2"",

    denomination_type_3 AS ""Denomination Type 3"",
    cash_added_type_3 AS ""Cash Added Type 3"", 
    remaining_type_3 AS ""Remaining Type 3"",
    purged_type_3 AS ""Purged Type 3"",
    return_type_3 AS ""Return Type 3"",
    return_amount_type_3 AS ""Return Amount Type 3"",

    cash_added_rc_type_3 AS ""Cash Added RC Type 3"", 
    remaining_rc_type_3 AS ""Remaining RC Type 3"",
    purged_rc_type_3 AS ""Purged RC Type 3"",
    return_rc_type_3 AS ""Return RC Type 3"",
    return_rc_amount_type_3 AS ""Return RC Amount Type 3"",

    denomination_type_4 AS ""Denomination Type 4"",
    cash_added_type_4 AS ""Cash Added Type 4"", 
    remaining_type_4 AS ""Remaining Type 4"",
    purged_type_4 AS ""Purged Type 4"",
    return_type_4 AS ""Return Type 4"",
    return_amount_type_4 AS ""Return Amount Type 4"",

    cash_added_rc_type_4 AS ""Cash Added RC Type 4"", 
    remaining_rc_type_4 AS ""Remaining RC Type 4"",
    purged_rc_type_4 AS ""Purged RC Type 4"",
    return_rc_type_4 AS ""Return RC Type 4"",
    return_rc_amount_type_4 AS ""Return RC Amount Type 4"",

	 ((cash_added_type_1 * denomination_type_1) + (cash_added_type_2 * denomination_type_2) + (cash_added_type_3 * denomination_type_3) + (cash_added_type_4 * denomination_type_4)) AS ""Total Cash Added Amount"", 
     ((cash_added_rc_type_1 * denomination_type_1) +  (cash_added_rc_type_2 * denomination_type_2) + (cash_added_rc_type_3 * denomination_type_3) + (cash_added_rc_type_4 * denomination_type_4)) AS ""Total RC Cash Added Amount"",

    return_amount_cd AS ""Return Amount CD"",
    return_amount_rc AS ""Return Amount RC"",
    total_return_amount AS ""Total Return Amount""
FROM replenishment_return where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + Filter() + " order by " + orderBy;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    dtRepReturn = new DataTable();
                    adapter.Fill(dtRepReturn);
                    this.GridView_Rep.DataSource = dtRepReturn;
                    this.GridView_Rep.DataBind();
                    Literal_count.Text = dtRepReturn.Rows.Count + " records.";
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>An Error occured while loading replenishment Return. " + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("Bind Replenishment Return grid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

        static void DeleteFile(string key, object val, CacheItemRemovedReason reas)
        {
            File.Delete(key);
        }
        public SortDirection GridViewSortDirection
        {
            get
            {

                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;

                //return (SortDirection)ViewState["sortDirection"];
                return (ViewState["sortDirection"].ToString() == " asc" ? SortDirection.Ascending : SortDirection.Descending);
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected void GridView_Rep_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGrid();


        }




        protected void GridView_Rep_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Rep.PageIndex = e.NewPageIndex;
            BindGrid();


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
            BindGrid();
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        string GetFilter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and c.cash_order_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
            else
                filter += " and c.cash_order_datetime >= getdate() - 3";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and c.cash_order_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            return filter;
        }


        private void EnableGridRespectiveColumns()
        {
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexReplenishment = -1;
            //int columnIndexDispensed = -1;
            //int columnIndexRemaining = -1;
            columnIndexReplenishment = GetColumnIndexByName(this.GridView_Rep, "Type 5");
            //columnIndexDispensed = GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            //columnIndexRemaining = columnIndexDenomination + 1;// GetColumnIndexByName(this.GridView_CashPositions, "Remaining");
            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexReplenishment >= 0)
                {
                    this.GridView_Rep.Columns[columnIndexReplenishment].Visible = true;
                    columnIndexReplenishment++;
                }

                //if (columnIndexDispensed >= 0)
                //{
                //    this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
                //    columnIndexDispensed++;
                //}
                //if (columnIndexRemaining >= 0)
                //{
                //    this.GridView_CashPositions.Columns[columnIndexRemaining].Visible = true;
                //    columnIndexRemaining += 2;
                //}
            }
        }


        protected void btnExportReplenishments_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("btnExportReplenishments_Click");
            try
            {
                ExportToExcel();
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

        protected void ExportToExcel()
        {
           
            if (dtRepReturn != null && dtRepReturn.Rows.Count > 0)
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    string fileName = "ReplenishmentReturnReport";
                    var worksheet = workbook.Worksheets.Add(fileName);
                    worksheet.Cell(1, 1).InsertTable(dtRepReturn); // Insert DataTable starting from Cell A1

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

    }
}

