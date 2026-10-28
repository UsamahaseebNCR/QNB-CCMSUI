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
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using Avanza.iSuite.UI;
using System.IO;
using System.Data.OleDb;
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;


namespace CCMSUI.CCMS
{
    public partial class CalculateInvoiceDeductions : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");

            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.CalculateInvoiceDeductions))
                        Response.Redirect("AccessDenied.aspx");
                    
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An error occured while loading page [" + ex.Message + "]</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void Button_Process_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_Process_Click");

            try
            {
                if (FileUpload_DelaysDetailSheet.HasFile && FileUpload_InvoiceSheet.HasFile)
                {
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                    string consoleExecutionFile = @"C:\NCR Projects\ConsoleAIMADeductionSheet\ConsoleAIMADeductionSheet\bin\Debug\ConsoleAIMADeductionSheet.exe";
                    string fileNameDelaysDetail = "";
                    string fileNameInvoice = "";
                    string sheetNameDelaysDetail = "";
                    string sheetNameInvoice = "";
                    //if (FileUpload_imac.FileName.Contains("xlsx"))
                    //shortageAmount = double.Parse(TextBox_ShortageAmount.Text);
                    fileNameDelaysDetail = appSetting.TemporaryFolder + "\\" + FileUpload_DelaysDetailSheet.FileName;
                    sheetNameDelaysDetail = !string.IsNullOrEmpty(TextBox_DelaysSheetName.Text) ? TextBox_DelaysSheetName.Text : "Sheet1";
                    
                    fileNameInvoice = appSetting.TemporaryFolder + "\\" + FileUpload_InvoiceSheet.FileName;
                    sheetNameInvoice = !string.IsNullOrEmpty(TextBox_InvoiceSheetName.Text) ? TextBox_InvoiceSheetName.Text : "ورقة1";
                    

                    File.WriteAllBytes(fileNameDelaysDetail, FileUpload_DelaysDetailSheet.FileBytes);
                    File.WriteAllBytes(fileNameInvoice, FileUpload_InvoiceSheet.FileBytes);

                    //SaveGasperTicket(fileName, TextBox_ShortageAmount.Text);
                    //ProcessExcelSheet(fileNameDelaysDetail, sheetNameDelaysDetail, fileNameInvoice, sheetNameInvoice, TextBox_ShortageAmount.Text, appSetting.TemporaryFolder);
                    ExecuteConsoleApp(fileNameDelaysDetail, sheetNameDelaysDetail, fileNameInvoice, sheetNameInvoice, TextBox_ShortageAmount.Text, appSetting.TemporaryFolder, consoleExecutionFile);

                    //File.Delete(fileNameDelaysDetail);
                    //File.Delete(fileNameInvoice);
                    //Literal_error.Text = "<li>Invoice and Delay Details Sheets uploaded successfully</li>";
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Deductions Sheet created successfully");
                    //Session[SessionVars.noticeForMain.ToString()] = "Invoice and Delay Details Sheets uploaded successfully";
                    Session[SessionVars.notice.ToString()] = "Deductions Sheet created successfully";
                    
                }
                else
                {
                    if (!FileUpload_DelaysDetailSheet.HasFile)
                        Literal_error.Text = "<li>Please select the Delays Detail Sheet to upload in the system</li>";
                    else
                        Literal_error.Text = "<li>Please select the Invoice Sheet to upload in the system</li>";
                }

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An error occured while importing Invoice and Delay Details Sheets [" + ex.Message + "]</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private void ExecuteConsoleApp(string fileNameDelaysDetail, string sheetNameDelaysDetail, string fileNameInvoice, string sheetNameInvoice, string shortageAmount, string tempFolder, string consoleExecutionFile)
        {
            string tempFolderForConsole="";
            //string clientMachineFilePath = "";
            fileNameDelaysDetail = "\"" + fileNameDelaysDetail + "\"";
            fileNameInvoice = "\"" + fileNameInvoice + "\"";
            shortageAmount = "\"" + shortageAmount + "\"";
            tempFolderForConsole = "\"" + tempFolder + "\"";
            sheetNameDelaysDetail = "\"" + sheetNameDelaysDetail + "\"";
            sheetNameInvoice = "\"" + sheetNameInvoice + "\"";

            //Process process = new Process();
            //process.StartInfo.FileName = @"C:\NCR Projects\TestConsoleApp\TestConsoleApp\bin\Debug\TestConsoleApp.exe";
            //process.Start();
            //process.WaitForExit();
            //int exitCode = process.ExitCode;

            Process process = new Process();
            process.StartInfo.FileName = consoleExecutionFile;
            process.StartInfo.Arguments = fileNameDelaysDetail + " " + fileNameInvoice + " " + shortageAmount + " " + tempFolderForConsole + " " + sheetNameDelaysDetail + " " + sheetNameInvoice;
            process.Start();
            process.WaitForExit();
            int exitcode = process.ExitCode;
            if (process.HasExited)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + @"\OutputEmirates.xls");
                Response.TransmitFile(tempFolder + @"\OutputEmirates.xls");
                Response.Flush();
                Response.Close();
            }
            

        }
    }
}