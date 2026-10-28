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

namespace CCMSUI.admin
{
    public partial class ImportFLMMonitoringRecords : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");

            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ImportFLMRecord))
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
                if (FileUpload_imac.HasFile)
                {
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                    File.WriteAllBytes(appSetting.TemporaryFolder + "\\FLMReport.xls", FileUpload_imac.FileBytes);
                    //File.WriteAllBytes(appSetting.TemporaryFolder + "\\IMACFile_Copy.xls", FileUpload_imac.FileBytes);

                    SaveFLM(GetDataSetFromSheet(appSetting.TemporaryFolder + "\\FLMReport.xls"));

                    File.Delete(appSetting.TemporaryFolder + "\\FLMReport.xls");

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Records imported successfully");
                    Session[SessionVars.noticeForMain.ToString()] = "Records imported successfully";
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An error occured while importing Call Out By NCR [" + ex.Message + "]</li>";

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
        protected DataSet GetDataSetFromSheet(string filepath)
        {
            OleDbCommand command = null;
            OleDbDataAdapter adapter = null;
            DataSet dsSheet = null;

            try
            {
                StringBuilder columnString = new StringBuilder();
                for (int i = 0; i < CheckBoxList_columns.Items.Count; i++)
                {
                    ListItem checkBox = (ListItem)CheckBoxList_columns.Items[i];
                    if (checkBox.Selected == true)
                        columnString.Append(String.Format(",[{0}]", checkBox.Value));
                }

                //command = new OleDbCommand(@"select note_set_type , [ChangeType],[ATM-ID],[ID4],[Region ID],[IP Address]" + columnString.ToString() + " from [IMAC$]", new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=\"Excel 8.0;IMEX=1\""));
                command = new OleDbCommand(@"select * from [NCR MONITORING$]", new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=\"Excel 8.0;IMEX=1\""));
                adapter = new OleDbDataAdapter(command);
                dsSheet = new DataSet();

                command.Connection.Open();
                adapter.Fill(dsSheet);
                //dsIMACSheet.Tables[0].Rows.RemoveAt(0);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read data from NCR Monitoring Sheet. " + ex.Message);
            }
            finally
            {
                if (adapter != null)
                    adapter.Dispose();

                if (command != null)
                    if (command.Connection != null)
                        if (command.Connection.State == ConnectionState.Open)
                            command.Connection.Close();
            }

            return dsSheet;
        }

        /// <summary>
        /// Reads each row of IMAC dataset and adds, changes or removes the ATM
        /// </summary>
        /// <param name="ds">Dataset containing data of selected columns from IMAC sheet</param>
        protected void SaveFLM(DataSet ds)
        {

            LogableTask task = LogableTask.NewTask("SaveFLM");
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            int rowCount = 0;
            try
            {


                connection = ConnectionFactory.GetNewConnection();
                connection.Open();
                transaction = connection.BeginTransaction();
                DateTime dt = DateTime.Now;
                string[] parts = null;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    rowCount++;
                    parts = null;
                    string time = row["Time"].ToString();
                    if (time.Length > 0)
                    {
                        if (time.Contains(" "))
                            parts = time.Split(' ')[1].Split(':');
                        else
                            parts = time.Split(':');
                    }


                    dt = DateTime.ParseExact(row["Date"].ToString(), "dd.MM.yyyy", null);
                    if (parts != null)
                        dt = dt.AddHours(int.Parse(parts[0])).AddMinutes(int.Parse(parts[1]));

                    FlmMonitoring flm = FlmMonitoring.LoadFlmMonitoring("sno=" + row[0]
                        + " and activity_datetime=convert(Datetime,'" + dt.ToString("dd/MM/yyyy HH:mm:ss") + "',103)");


                    if (flm != null)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Record already exists with serial no= " + row[0] + " and activity date=" + row["Date"]);
                        continue;
                    }


                    flm = new FlmMonitoring();
                    flm.Sno = int.Parse(row[0].ToString());
                    flm.ActivityDatetime = dt;

                    for (int i = 0; i < CheckBoxList_columns.Items.Count; i++)
                    {
                        ListItem checkBox = (ListItem)CheckBoxList_columns.Items[i];

                        if (checkBox.Selected)
                        {
                            object cellValue = row[checkBox.Value];
                            if (isValid(cellValue))
                            {
                                switch (checkBox.Value)
                                {


                                    case "log":
                                        flm.Log = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;

                                    case "Call Reported By":
                                        flm.CallReportedBy = Convert.ToString(cellValue).Replace("'", "''");
                                        break;
                                    case "ATM ID No#":
                                        flm.AtmIdNo = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "ATM LOCATION":
                                        flm.AtmLocation = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "TYPE OF PROBLEM":
                                        flm.TypeOfProblem = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "CODE":
                                        flm.Code = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "INFO PASSED TO":
                                        flm.InfoPassedTo = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "NCR TICKET NUMBER":
                                        flm.TicketNumber = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "Type Of Call":
                                        flm.TypeOfCall = Convert.ToString(cellValue);
                                        break;

                                    case "Time Of Info":
                                        if (cellValue.ToString().Length > 0)
                                        {
                                            parts = cellValue.ToString().Split(' ')[1].Split(':');
                                            flm.TimeOfInfo = dt.AddHours(int.Parse(parts[0])).AddMinutes(int.Parse(parts[1]));

                                        }
                                        break;
                                    case "Time Of Reaching Site":
                                        if (cellValue.ToString().Length > 0)
                                        {
                                            parts = cellValue.ToString().Split(' ')[1].Split(':');
                                            flm.TimeOfReachingSite = dt.AddHours(int.Parse(parts[0])).AddMinutes(int.Parse(parts[1]));

                                        }
                                        break;
                                    case "Time Of Solving":
                                        if (cellValue.ToString().Length > 0)
                                        {
                                            parts = cellValue.ToString().Split(' ')[1].Split(':');
                                            flm.TimeOfSolving = dt.AddHours(int.Parse(parts[0])).AddMinutes(int.Parse(parts[1]));

                                        }
                                        break;
                                    case "Attended By":
                                        flm.AttendedBy = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "Reject Receipt Counter Type 1":
                                        flm.RejectReceiptCounterType1 = Convert.ToInt32(cellValue);
                                        break;
                                    case "Reject Receipt Counter Type 2":
                                        flm.RejectReceiptCounterType2 = Convert.ToInt32(cellValue);
                                        break;
                                    case "Reject Receipt Counter Type 3":
                                        flm.RejectReceiptCounterType3 = Convert.ToInt32(cellValue);
                                        break;
                                    case "Physical Reject Counter Type 1":
                                        flm.PhysicalRejectCounterType1 = Convert.ToInt32(cellValue);
                                        break;
                                    case "Physical Reject Counter Type 2":
                                        flm.PhysicalRejectCounterType2 = Convert.ToInt32(cellValue);
                                        break;
                                    case "Physical Reject Counter Type 3":
                                        flm.PhysicalRejectCounterType3 = Convert.ToInt32(cellValue);
                                        break;
                                    case "Actual Problem Found":
                                        flm.ActualProblemFound = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "Action Taken":
                                        flm.ActionTaken = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "BNA(CDM)Jammed Counter":
                                        flm.BnaJammedCounter = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "TE#BAG NUMBER & COUNT":
                                        flm.TeBagNoAndCount = Convert.ToString(cellValue).Replace("'", "''"); ;
                                        break;
                                    case "Time Of Call To Bank":
                                        if (cellValue.ToString().Length > 0)
                                        {
                                            parts = cellValue.ToString().Split(' ')[1].Split(':');
                                            flm.TimeOfCallToBank = dt.AddHours(int.Parse(parts[0])).AddMinutes(int.Parse(parts[1]));

                                        }
                                        break;
                                }
                            }
                        }
                    }
                    flm.UploadDatetime = DateTime.Now;
                    flm.UploadedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    flm.Save(transaction.Connection, transaction);
                }

                if (transaction != null)
                {
                    transaction.Commit();

                }
                //Session[SessionVars.notice.ToString()] = "Records imported successfully";
                Literal_ok.Text = "Records imported successfully.";

            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, "error in processing row " + rowCount);
                throw new Exception("Unable to save Call out. An error ocurred while processing row :" + rowCount + " ," + ex.Message);
            }
            finally
            {
                task.EndTask();
                if (connection != null)
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
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