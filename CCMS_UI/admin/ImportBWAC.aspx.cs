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
using System.Text.RegularExpressions;


namespace CCMSUI.admin
{
    public partial class ImportBWAC : BasePage
    {
        Regex regex = new Regex(@"(Printed[ ]on[ ](?<PrintDate>\d+/\d+/\d+))|(ATM[ ]Id[ ]:[ ](?<ATMId>\d+))|(Last[ ]Start[ ]Of[ ]Day[ ]/[ ]End[ ]Of[ ]Day[ ]:\r?\n?[ ](?<RepDateTime>\d+/\d+/\d+[ ]+\d+:\d+:\d+))|(Local[ ]Currency	+(?<Loaded>[\d,]+)	+(?<Dispensed>[\d,]+)	+(?<Remaining>[\d,]+))");

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");

            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ImportBWAC))
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

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_Submit_Click");

            try
            {
                if (FileUpload_bwac.HasFile)
                {
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                    string fileName = "";
                    //if (FileUpload_imac.FileName.Contains("xlsx"))
                    fileName = appSetting.TemporaryFolder + "\\" + FileUpload_bwac.FileName;
                    //else
                    //    fileName = appSetting.TemporaryFolder + "\\ATMSETTLEMENT.xls";


                    File.WriteAllBytes(fileName, FileUpload_bwac.FileBytes);


                    //File.WriteAllBytes(appSetting.TemporaryFolder + "\\IMACFile_Copy.xls", FileUpload_imac.FileBytes);

                    //SaveSettlement(GetDataSetFromSheet(fileName, TextBox_SheetName.Text));
                    DoImport(fileName,task);


                    //File.Delete(fileName);

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "File uploaded successfully");
                    Session[SessionVars.noticeForMain.ToString()] = "File uploaded successfully";
                }
                else
                    Literal_error.Text = "<li>Please select the bwac text file to upload in the system</li>";

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An error occured while importing bwac text file [" + ex.Message + "]</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        /// <summary>
        /// Returns the set of data from columns that are selected by the user
        /// </summary>
        /// <param name="filepath">Complete path where IMAC sheet resides</param>
        /// <returns></returns>

        /// <summary>
        /// Reads each row of IMAC dataset and adds, changes or removes the ATM
        /// </summary>
        /// <param name="ds">Dataset containing data of selected columns from IMAC sheet</param>
        protected void DoImport(string filePath,LogableTask task)
        {
            SqlCommand cmd = null;
            SqlTransaction transaction = null;
            try
            {
                BwacUploadProcess bwacUploadProcess = null;
                BwacCashPosition bwacCashPosition = null;
                cmd = ConnectionFactory.GetNewCommand(true);
                transaction = cmd.Connection.BeginTransaction();

                string content = File.ReadAllText(filePath);
                content = content.Replace("\"", "");
                Match match = regex.Match(content);
                while (match.Success)
                {
                    if (match.Groups["PrintDate"].Success)
                    {
                        bwacUploadProcess = BwacUploadProcess.LoadBwacUploadProcess("upload_filename = '" + filePath + "'");
                        if (bwacUploadProcess != null)
                        {
                            ConnectionFactory.ExecuteQuery("delete bwac_cash_position where bwac_upload_process_id = " + bwacUploadProcess.BwacUploadProcessId, transaction);
                        }
                        else
                            bwacUploadProcess = new BwacUploadProcess();
                        
                        bwacUploadProcess.UploadDatetime = DateTime.Now;
                        bwacUploadProcess.UploadedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        bwacUploadProcess.UploadFilename = filePath;
                        bwacUploadProcess.PrintDate = DateTime.ParseExact(match.Groups["PrintDate"].Value, "dd/MM/yyyy", null);
                        bwacUploadProcess.Save(transaction.Connection, transaction);
                    }
                    else
                    {
                        if (match.Groups["ATMId"].Success)
                        {
                            bwacCashPosition = new BwacCashPosition();
                            Atm atm = Atm.LoadAtm("title = 'QNB0" + match.Groups["ATMId"].Value + "'");
                            if (atm != null)
                            {
                                bwacCashPosition.AtmId = atm.ATMId;
                            }
                            else
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "ATM ID does not exists" + match.Groups["ATMId"].Value);


                        }
                        else
                        {
                            if (bwacCashPosition != null)
                            {

                                if (bwacCashPosition.AtmId != 0)
                                {
                                    if (match.Groups["RepDateTime"].Success)
                                        bwacCashPosition.RepDatetime = DateTime.ParseExact(match.Groups["RepDateTime"].Value, "dd/MM/yyyy  HH:mm:ss", null);

                                    if (match.Groups["Loaded"].Success)
                                        bwacCashPosition.Loaded = decimal.Parse(match.Groups["Loaded"].Value);

                                    if (match.Groups["Dispensed"].Success)
                                        bwacCashPosition.Dispensed = decimal.Parse(match.Groups["Dispensed"].Value);

                                    if (match.Groups["Remaining"].Success)
                                    {
                                        bwacCashPosition.Remaining = decimal.Parse(match.Groups["Remaining"].Value);
                                        bwacCashPosition.BwacUploadProcessId = bwacUploadProcess.BwacUploadProcessId;
                                        bwacCashPosition.Save(transaction.Connection, transaction);
                                    }
                                }
                            }
                        }
                    }


                    match = match.NextMatch();

                }

                transaction.Commit();
                Literal_ok.Text = "File uploaded successfully!";


            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();

                throw;// new Exception("Unable to save ATMS. " + ex.Message);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Determines if the necessary columns (ChangeType, ATM-ID, Region ID, IP Address and ID4) in the row are valid
        /// </summary>
        /// <param name="row">Row of the dataset from IMAC sheet</param>
        /// <returns></returns>


        protected bool isValid(object value)
        {
            if (value == null)
                return false;
            if (value.ToString().Replace(" ", "").Equals(String.Empty))
                return false;
            return true;
        }


    }
}