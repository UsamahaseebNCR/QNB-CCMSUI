using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using ClosedXML.Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using System.Web.UI;

namespace CCMSUI.CCMS
{
    public partial class CPMCountClearedReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                Literal_errorMsg.Text = "";
                Literal_ok.Text = "";
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ViewCPMCountClearedReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                   
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        private string buildSearchCriteria()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and counts_cleared_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103) ";


            if (TextBox_ToDate.Text.Length > 0)
                filter += " and counts_cleared_at <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";



            return filter;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Literal_errorMsg.Text = "";
            Literal_ok.Text = "";
            LogableTask task = LogableTask.NewTask("Search");
            try
            {

                DataTable dt = new DataTable();
                using (SqlCommand cmd = ConnectionFactory.GetNewCommand(true))
                {
                    cmd.CommandTimeout = 100;

                    cmd.CommandText = $@"select a.title [ATM Title] , counts_cleared_at [Counts Cleared Date] , bin1 [Bin 1] , bin2 [Bin 2], bin3 [Bin 3],bin4 [Bin 4]  
                        from cpm_counts_cleared c
                        inner join atm a on c.atm_id = a.ATM_id where a.atm_id in ( {Session[SessionVars.SelectedATMs.ToString()]}) {buildSearchCriteria()} ;";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        adapter.Fill(dt);
                    cmd.Connection.Close();
                }

               


                if (dt.Rows.Count > 0)
                {
                    ExportToExcel(dt, "CPMCountClearedReport");
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }
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
                Literal_errorMsg.Text = "<li>No data available to export.</li>";
            }
        }
    }
}