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
    public partial class ImportGasperTickets : BasePage
    {
        string[] dateFormats = new string[] { "M/d/yyyy h:mm:ss tt" ,"MM/dd/yyyy h:mm:ss tt" ,"M/dd/yyyy h:mm:ss tt" ,"MM/d/yyyy h:mm:ss tt",
            "M/d/yyyy HH:mm:ss" ,"MM/dd/yyyy HH:mm:ss" ,"M/dd/yyyy HH:mm:ss" ,"MM/d/yyyy HH:mm:ss",
        "d/M/yyyy h:mm:ss tt" ,"dd/MM/yyyy h:mm:ss tt" ,"dd/M/yyyy h:mm:ss tt" ,"d/MM/yyyy h:mm:ss tt" ,
        "d/M/yyyy HH:mm:ss" ,"dd/MM/yyyy HH:mm:ss" ,"dd/M/yyyy HH:mm:ss" ,"d/MM/yyyy HH:mm:ss" ,"dd-MMM-yy HH:mm:ss" };

        //protected void Page_PreInit(object sender, EventArgs e)
        //{
        //    this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        //}


        //protected void refresh_ServerClick(object sender, EventArgs e)
        //{
        //    string regionID = Session[SessionVars.SelectedRegions.ToString()].ToString();
        //    if (regionID != null /*&& !regionID.Contains(",")*/
        //        && !regionID.StartsWith("1,"))
        //    {

        //        Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
        //        if (region.IsOrganization)
        //        {
        //            Label_SelectedOrganization.Text = region.RegionName;
        //            Session[SessionVars.SelectedRegions.ToString()] = region.RegionId.ToString();
        //        }
        //        DropDownList_Process.Items.Clear();
        //        OrganizationSettlementsProcess.OrganizationSettlementsProcessReader reader = OrganizationSettlementsProcess.ExecuteReader("organization_id=" + region.RegionId);
        //        while (reader.Read())
        //        {
        //            SettlementProcess process = SettlementProcess.LoadSettlementProcessByPk(reader.CurrentOrganizationSettlementsProcess.SettlementProcessId.Value);
        //            DropDownList_Process.Items.Add(new ListItem(process.SettlementProcessName, process.SettlementProcessId.ToString()));
        //        }
        //        DropDownList_Process.DataBind();
        //        if (DropDownList_Process.Items.Count>0)
        //            DropDownList_Process_Idx_Chg(this, null);
        //    }
        //    else
        //        Label_SelectedOrganization.Text = "";

        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");

            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ImportATMSettlement))
                        Response.Redirect("AccessDenied.aspx");
                    //refresh_ServerClick(this, null);
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

                    string fileName = "";
                    //if (FileUpload_imac.FileName.Contains("xlsx"))
                    fileName = appSetting.TemporaryFolder + "\\" + FileUpload_imac.FileName;
                    //else
                    //    fileName = appSetting.TemporaryFolder + "\\ATMSETTLEMENT.xls";


                    File.WriteAllBytes(fileName, FileUpload_imac.FileBytes);


                    //File.WriteAllBytes(appSetting.TemporaryFolder + "\\IMACFile_Copy.xls", FileUpload_imac.FileBytes);

                    //SaveSettlement(GetDataSetFromSheet(fileName, TextBox_SheetName.Text));
                    SaveGasperTicket(fileName, TextBox_SheetName.Text);


                    File.Delete(fileName);

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Gasper Tickets uploaded successfully");
                    Session[SessionVars.noticeForMain.ToString()] = "Gasper Tickets uploaded successfully";
                }
                else
                    Literal_error.Text = "<li>Please select the ticket file to upload in the system</li>";

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An error occured while importing gasper tickets [" + ex.Message + "]</li>";
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
        protected static DataSet GetDataSetFromSheet(string filepath, string sheetName)
        {
            OleDbCommand command = null;
            OleDbDataAdapter adapter = null;
            DataSet dsSheet = null;

            try
            {
                if (filepath.Contains("xlsx"))
                    command = new OleDbCommand(@"select * from [" + sheetName + "$]", new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0 Xml;\""));
                else
                    command = new OleDbCommand(@"select *  from [" + sheetName + "$]", new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=\"Excel 8.0;\""));
                adapter = new OleDbDataAdapter(command);
                dsSheet = new DataSet();
                adapter.Fill(dsSheet);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read data from excel sheet. " + ex.Message);
            }
            finally
            {
                if (adapter != null)
                    adapter.Dispose();


            }
            return dsSheet;
        }

        /// <summary>
        /// Reads each row of IMAC dataset and adds, changes or removes the ATM
        /// </summary>
        /// <param name="ds">Dataset containing data of selected columns from IMAC sheet</param>
        protected void SaveGasperTicket(string filePath, string sheetName)
        {
            SqlCommand cmd = null;
            SqlTransaction transaction = null;
            try
            {
                //Variable added by Ali Shah on 16 Feb, 2016
                //Change in Import Gasper as no. of new fields added in gasper_ticket table
                DateTime datetimeStartTime;
                DateTime datetimeEndTime;
                string address = "";
                //string[] parts = dsSheet.Tables[0].Rows[6][3].ToString().Split('#');

                //if (DropDownList_Process.SelectedItem.Value == "2")
                //{
                //    StringBuilder builder = new StringBuilder();
                //    Settlements.Settlements settlements = new Settlements.Settlements();
                //    int noOfSettlementsImported = settlements.ImportSettlements(filePath, sheetName, TextBox_VaultBalancingSheetName.Text, ((AppUser)Session[SessionVars.user.ToString()]).UserId,builder);
                //    if (noOfSettlementsImported > 0)
                //    {
                //        Session[SessionVars.notice.ToString()] = noOfSettlementsImported + " ATM Settlements imported Successfully!";
                //        Response.Redirect("~/ccms/ATMSettlementList.aspx");

                //    }
                //    else
                //        Literal_ok.Text = "Settlements alreadys exists for all the ATMs.";
                //} //For QIB use the below code.
                //else if (DropDownList_Process.SelectedItem.Value == "4")
                //{
                DateTime datetime;
                DataSet result = GetDataSetFromSheet(filePath, sheetName);
                LogableTask.LogMonoActivityTask("i", MethodBase.GetCurrentMethod(), TraceLevel.Error, "total rows: " +result.Tables[0].Rows.Count);
                    
                if (result.Tables[0].Rows.Count > 0)
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    transaction = cmd.Connection.BeginTransaction();

                    foreach (DataRow dr in result.Tables[0].Rows)
                    {/*dr[1]
                    "MRQDBU0291"
                    dr[7]
                    "2014-Sep-14"
                    dr[8]
                    "00:20:53"
                    */
                        LogableTask.LogMonoActivityTask("i", MethodBase.GetCurrentMethod(), TraceLevel.Error, "processing " + dr[0] + " " + dr["Ticket Problem Bucket TH"].ToString());
                        if (dr["Ticket Problem Bucket TH"].ToString() == "Communication Failure")
                        {
                            LogableTask.LogMonoActivityTask("i", MethodBase.GetCurrentMethod(), TraceLevel.Error, "matched");
                            GasperTicket newTicket = new GasperTicket();
                            Atm atm = Atm.LoadAtm("title = '" + dr["ATM ID"] + "'");
                            if (atm != null)
                            {
                                LogableTask.LogMonoActivityTask("i", MethodBase.GetCurrentMethod(), TraceLevel.Error, "atm exists");
                                newTicket.AtmId = atm.ATMId;
                                newTicket.Title = dr["ATM ID"].ToString();
                                DateTime.TryParseExact(Convert.ToDateTime(dr["Start Date Local"]).ToShortDateString() + " " + dr["Start Time  Local"].ToString(), dateFormats, null, System.Globalization.DateTimeStyles.None, out datetimeStartTime);
                                newTicket.StartDate = datetimeStartTime;
                                if (dr["End Date Local"].ToString() != "" && dr["End Time  Local"].ToString() != "")
                                {
                                    DateTime.TryParseExact(Convert.ToDateTime(dr["End Date Local"]).ToShortDateString().Split(' ')[0] + " " + dr["End Time  Local"].ToString(), dateFormats, null, System.Globalization.DateTimeStyles.None, out datetimeEndTime);
                                    newTicket.EndDate = datetimeEndTime;
                                }

                                //Edited by Ali Shah on 11th Feb, 2016
                                newTicket.Reason = dr["Ticket Problem Bucket TH"].ToString();
                                newTicket.TicketId = int.Parse(dr["Ticket ID"].ToString());
                                newTicket.BranchName = dr["Branch Name TH"].ToString();
                                address = dr["Address 1"].ToString();
                                if (address.Contains("'"))
                                    address = address.Replace("'", "''");
                                newTicket.Address1 = address;   //dr["Address 1"].ToString();
                                newTicket.City = dr["City"].ToString();
                                newTicket.ObjectTypeDescription = dr["Object Type Description"].ToString();
                                newTicket.LocationType = dr["Location Type"].ToString();
                                newTicket.StatusCodeKey = int.Parse(dr["Status Code Key"].ToString());
                                newTicket.StatusDescription = dr["Status Desc"].ToString();
                                newTicket.SharedComment = dr["Shared Comment"].ToString();
                                newTicket.ServiceTeam = dr["Service Team"].ToString();
                                newTicket.TotalTicketDuration = Convert.ToDecimal(dr["Total Ticket Duration"]);
                                //
                                newTicket.Save(transaction.Connection, transaction);
                                LogableTask.LogMonoActivityTask("ImportGasperTickets", MethodBase.GetCurrentMethod(), TraceLevel.Error, "ticket saved.");
                            }
                            else
                                LogableTask.LogMonoActivityTask("ImportGasperTickets", MethodBase.GetCurrentMethod(), TraceLevel.Error, "cit atm title is not defined " + dr[1]);
                        }
                    }
                    transaction.Commit();
                    Literal_ok.Text = "Gasper Tickets Imported successfully!";
                }
                else
                    Literal_error.Text = "No rows found";
                
                //                } //For QIB use the below code.
                //else
                //{
                //    AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlement(
                //        string.Format("gl_no='{0}' and rep_datetime=convert(datetime,'{1}',103) and atm_site_and_number='{2}'",
                //        dsSheet.Tables[0].Rows[7][2], DateTime.ParseExact(dsSheet.Tables[0].Rows[8][3].ToString(), "dd.MM.yyyy", null).ToString("dd/MM/yyyy HH:mm:ss"),
                //        dsSheet.Tables[0].Rows[6][3].ToString()));

                //    if (atmSettlement == null)
                //    {
                //        connection = ConnectionFactory.GetNewConnection();
                //        connection.Open();
                //        transaction = connection.BeginTransaction();
                //        DateTime dt = DateTime.Now;
                //        atmSettlement = new AtmSettlement();
                //        atmSettlement.AtmSiteAndNumber = dsSheet.Tables[0].Rows[6][3].ToString();
                //        //atmSettlement.AtmNumber = parts[1];
                //        atmSettlement.GlNo = dsSheet.Tables[0].Rows[7][2].ToString();
                //        atmSettlement.RepDatetime = DateTime.ParseExact(dsSheet.Tables[0].Rows[8][3].ToString(), "dd.MM.yyyy", null);
                //        atmSettlement.SealNumber = dsSheet.Tables[0].Rows[9][2].ToString();



                //        atmSettlement.CashRepDenominationType1 = int.Parse(dsSheet.Tables[0].Rows[12][1].ToString());
                //        atmSettlement.CashRepCountersType1 = int.Parse(dsSheet.Tables[0].Rows[12][2].ToString());
                //        atmSettlement.CashRepDenominationType2 = int.Parse(dsSheet.Tables[0].Rows[13][1].ToString());
                //        atmSettlement.CashRepCountersType2 = int.Parse(dsSheet.Tables[0].Rows[13][2].ToString());
                //        atmSettlement.CashRepDenominationType3 = int.Parse(dsSheet.Tables[0].Rows[14][1].ToString());
                //        atmSettlement.CashRepCountersType3 = int.Parse(dsSheet.Tables[0].Rows[14][2].ToString());

                //        atmSettlement.CashReturnDenominationType1 = int.Parse(dsSheet.Tables[0].Rows[12][5].ToString());
                //        atmSettlement.CashReturnCountersType1 = int.Parse(dsSheet.Tables[0].Rows[12][6].ToString());
                //        atmSettlement.CashReturnDenominationType2 = int.Parse(dsSheet.Tables[0].Rows[13][5].ToString());
                //        atmSettlement.CashReturnCountersType2 = int.Parse(dsSheet.Tables[0].Rows[13][6].ToString());
                //        atmSettlement.CashReturnDenominationType3 = int.Parse(dsSheet.Tables[0].Rows[14][5].ToString());
                //        atmSettlement.CashReturnCountersType3 = int.Parse(dsSheet.Tables[0].Rows[14][6].ToString());

                //        atmSettlement.CashDispensedDenominationType1 = int.Parse(dsSheet.Tables[0].Rows[20][1].ToString());
                //        atmSettlement.CashDispensedCountersType1 = int.Parse(dsSheet.Tables[0].Rows[20][2].ToString());
                //        atmSettlement.CashDispensedDenominationType2 = int.Parse(dsSheet.Tables[0].Rows[21][1].ToString());
                //        atmSettlement.CashDispensedCountersType2 = int.Parse(dsSheet.Tables[0].Rows[21][2].ToString());
                //        atmSettlement.CashDispensedDenominationType3 = int.Parse(dsSheet.Tables[0].Rows[22][1].ToString());
                //        atmSettlement.CashDispensedCountersType3 = int.Parse(dsSheet.Tables[0].Rows[22][2].ToString());

                //        atmSettlement.CashRejectedDenominationType1 = int.Parse(dsSheet.Tables[0].Rows[20][5].ToString());
                //        atmSettlement.CashRejectedCountersType1 = int.Parse(dsSheet.Tables[0].Rows[20][6].ToString());
                //        atmSettlement.CashRejectedDenominationType2 = int.Parse(dsSheet.Tables[0].Rows[21][5].ToString());
                //        atmSettlement.CashRejectedCountersType2 = int.Parse(dsSheet.Tables[0].Rows[21][6].ToString());
                //        atmSettlement.CashRejectedDenominationType3 = int.Parse(dsSheet.Tables[0].Rows[22][5].ToString());
                //        atmSettlement.CashRejectedCountersType3 = int.Parse(dsSheet.Tables[0].Rows[22][6].ToString());

                //        atmSettlement.UploadDatetime = DateTime.Now;
                //        atmSettlement.UploadedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

                //        atmSettlement.Save(transaction.Connection, transaction);



                //        Atm atm = Atm.LoadAtm("gl_number='" + atmSettlement.GlNo + "'");
                //        if (atm != null)
                //        {
                //            UserTask userTask = new UserTask();
                //            userTask.AtmSettlementId = atmSettlement.AtmSettlementId;
                //            userTask.CreatedBy = atmSettlement.UploadedBy.Value;
                //            userTask.CreationTime = DateTime.Now;
                //            userTask.Status = ApprovalStatus.Pending.ToString();
                //            userTask.TaskTypeId = (int)EnumTaskType.ApproveSettlement;
                //            userTask.AtmId = atm.ATMId;
                //            userTask.Save(transaction.Connection, transaction);



                //            UserATMs.UserATMsReader reader = UserATMs.ExecuteReader("atm_id = " + atm.ATMId);
                //            try
                //            {
                //                while (reader.Read())
                //                {


                //                    UserTaskAssignment taskAssignment = new UserTaskAssignment();
                //                    taskAssignment.UserTaskId = userTask.UserTaskId;
                //                    taskAssignment.UserId = reader.CurrentUserATMs.UserId;
                //                    taskAssignment.Save(transaction.Connection, transaction);

                //                }
                //            }
                //            finally
                //            {
                //                if (reader != null)
                //                    reader.Close();
                //            }

                //        }
                //        else
                //        {
                //            UserTask userTask = new UserTask();
                //            userTask.AtmSettlementId = atmSettlement.AtmSettlementId;
                //            userTask.CreatedBy = atmSettlement.UploadedBy.Value;
                //            userTask.CreationTime = DateTime.Now;
                //            userTask.Status = ApprovalStatus.Pending.ToString();
                //            userTask.TaskTypeId = (int)EnumTaskType.ApproveSettlement;
                //            userTask.Save(transaction.Connection, transaction);

                //            UserTaskAssignment taskAssignment = new UserTaskAssignment();
                //            taskAssignment.UserTaskId = userTask.UserTaskId;
                //            taskAssignment.UserId = 2;
                //            taskAssignment.Save(transaction.Connection, transaction);
                //        }

                //        if (transaction != null)
                //            transaction.Commit();

                //        Literal_ok.Text = "ATM Settlement Imported Successfully.";
                //    }
                //    else
                //    {
                //        Literal_error.Text = "ATM Settlement already exists.";
                //    }
                //}
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                //LogableTask.LogMonoActivityTask("test", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex.Message + " " + ex.StackTrace);
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