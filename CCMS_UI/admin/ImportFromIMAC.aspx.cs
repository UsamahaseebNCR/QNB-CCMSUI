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

using System.Data.SqlClient;
using System.Text;
using System.Linq;

namespace CCMSUI.admin
{
    public partial class ImportFromIMAC : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");

            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ImportATMsFromIMAC))
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

            string excelContentType = System.Configuration.ConfigurationManager.AppSettings["excelContentType"];
            string[] excelContentTypeArray = string.IsNullOrEmpty(excelContentType) ? 
                new string[]  { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } : 
                excelContentType.Split(';');

            try
            {
                if (FileUpload_imac.HasFile)
                {
                    if ((FileUpload_imac.PostedFile.FileName.Contains(".xlsx") || (FileUpload_imac.PostedFile.FileName.Contains(".xls"))))
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, FileUpload_imac.PostedFile.ContentType);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, excelContentTypeArray.Length.ToString());
                        if (excelContentTypeArray.Contains(FileUpload_imac.PostedFile.ContentType))
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Here");
                            AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                            string extention = FileUpload_imac.FileName.Contains("xlsx") ? "xlsx" : "xls";

                            File.WriteAllBytes(appSetting.TemporaryFolder + "\\IMACFile." + extention, FileUpload_imac.FileBytes);
                            File.WriteAllBytes(appSetting.TemporaryFolder + "\\IMACFile_Copy." + extention, FileUpload_imac.FileBytes);
                            SaveATMs(GetDataSetFromIMACSheet(appSetting.TemporaryFolder + "\\IMACFile_Copy." + extention));
                            File.Delete(appSetting.TemporaryFolder + "\\IMACFile_Copy." + extention);

                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "ATMs imported successfully from IMAC sheet");
                            Session[SessionVars.notice.ToString()] = "<script>parent.window.location.reload();</script>";
                            Session[SessionVars.noticeForMain.ToString()] = "ATMs imported successfully from IMAC sheet.";
                        }
                        else
                            throw new Exception("invalid file type uploaded on server");
                    }
                    else
                    {
                        Literal_error.Text = "<li>Please only select .xlsx and xls IMAC file to upload in the system</li>";
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An error occured while importing ATMs [" + ex.Message + "]</li>";
                if (ex.Message.Contains("invalid"))
                    throw;
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
        protected DataSet GetDataSetFromIMACSheet(string filepath)
        {
            OleDbCommand command = null;
            OleDbDataAdapter adapter = null;
            DataSet dsIMACSheet = null;

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
                if (filepath.Contains("xlsx"))
                    command = new OleDbCommand(@"select * from [IMAC$]", new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\""));
                else
                    command = new OleDbCommand(@"select * from [IMAC$]", new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=\"Excel 8.0;IMEX=1\""));



                //command = new OleDbCommand(@"select * from [IMAC$]", new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=\"Excel 8.0;IMEX=1\""));
                adapter = new OleDbDataAdapter(command);
                dsIMACSheet = new DataSet();

                command.Connection.Open();
                adapter.Fill(dsIMACSheet);
                //dsIMACSheet.Tables[0].Rows.RemoveAt(0);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read data from IMAC sheet. " + ex.Message);
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

            return dsIMACSheet;
        }

        /// <summary>
        /// Reads each row of IMAC dataset and adds, changes or removes the ATM
        /// </summary>
        /// <param name="ds">Dataset containing data of selected columns from IMAC sheet</param>
        protected void SaveATMs(DataSet ds)
        {
            string lastcol = string.Empty;
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            int rowCount = 0;
            try
            {
                connection = ConnectionFactory.GetNewConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    rowCount++;
                    Region region = null;
                    Region organization = null;
                    UserOrganization userOrganization = null;
                    Atm atm = null;
                    McnAtms mcnAtms = null;

                    if (isComplete(row))
                    {
                        switch (row["ChangeType"].ToString())
                        {
                            #region Add
                            case "A":

                                object res = ConnectionFactory.ExecuteScalar("select is_active from atm with (nolock) where atm.title = '" + row["ATM-ID"] + "'");
                                if (res != null)
                                {
                                    if (bool.Parse(res.ToString()))
                                        throw new Exception("ATM with friendly name \"" + row["ATM-ID"] + "\" already exists");
                                    else
                                        ConnectionFactory.ExecuteQuery("update atm set is_active=1 where title ='" + row["ATM-ID"] + "'", transaction);
                                    continue;

                                }
                                //if (ConnectionFactory.ExecuteScalar("select is_active from atm where atm.title = '" + row["ATM-ID"] + "'") != null)
                                //{

                                //    throw new Exception("ATM with friendly name \"" + row["ATM-ID"] + "\" already exists");

                                //}

                                if (ConnectionFactory.ExecuteScalar("select top 1 title from atm with (nolock) where atm.ip = '" + row["IP Address"] + "'") != null)
                                    throw new Exception("ATM with IP \"" + row["IP Address"] + "\" already exists");

                                organization = Region.LoadRegion("MCN like '" + row["ID4"] + "'");
                                if (organization == null)
                                    throw new Exception("Organization \"" + row["ID4"] + "\" does not exists");

                                if ((int)ConnectionFactory.ExecuteScalar("select count(*) from note_set_type with (nolock) where note_set_type_name='" + row["note_set_type"] + "' and region_id = " + organization.RegionId) == 0)
                                    throw new Exception("Note Set Type \"" + row["note_set_type"] + "\" does not exists for organization " + organization.RegionName);



                                if (organization == null)
                                {
                                    organization = new Region();
                                    organization.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                    organization.CreationTime = DateTime.Now;
                                    organization.IsActive = true;
                                    organization.MCN = row["ID4"].ToString();
                                    organization.RegionName = row["ID4"].ToString();
                                    organization.IsOrganization = true;
                                    organization.ParentRegionId = 1;
                                    organization.Save(connection, transaction);
                                }

                                DataTable dtUserOrgs = ((DataTable)Session[SessionVars.dtUserOrgs.ToString()]);
                                if (!dtUserOrgs.Rows.Contains(organization.MCN))
                                {
                                    dtUserOrgs.Rows.Add(new object[] { organization.MCN });
                                    Session[SessionVars.dtUserOrgs.ToString()] = dtUserOrgs;
                                }

                                region = Region.LoadRegion("region_name like '" + row["Region ID"] + "' and parent_region_id = " + organization.RegionId);
                                if (region == null)
                                {
                                    region = new Region();
                                    region.RegionName = row["Region ID"].ToString();
                                    region.ParentRegionId = organization.RegionId;
                                    region.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                    region.IsActive = true;
                                    region.CreationTime = DateTime.Now;
                                    region.IsOrganization = false;
                                    region.Save(connection, transaction);
                                }

                                userOrganization = UserOrganization.LoadUserOrganization("org_mcn like '" + row["ID4"] + "' and user_id = " + ((AppUser)Session[SessionVars.user.ToString()]).UserId);
                                if (userOrganization == null)
                                {
                                    userOrganization = new UserOrganization();
                                    userOrganization.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                    userOrganization.OrgMcn = row["ID4"].ToString();
                                    userOrganization.Save(connection, transaction);
                                }

                                atm = new Atm();

                                //atm.AtmFriendlyName = row["ATM-ID"].ToString();
                                atm.Title = row["ATM-ID"].ToString();
                                atm.RegionId = region.RegionId;
                                atm.NoteSetTypeId = Avanza.CCMS.DAL.NoteSetType.LoadNoteSetType("note_set_type_name='" + row["note_set_type"] + "'").NoteSetTypeId;
                                //atm.RegionName = row["Region ID"].ToString();
                                atm.IP = row["IP Address"].ToString();
                                atm.AtmType = "NCR_ATM";
                                atm.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                atm.CreationTime = DateTime.Now;
                                atm.LastWincorSent = DateTime.Now.AddDays(-2);
                                atm.ModifiedBy = null;
                                atm.Port = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DefaltAtmPort;
                                atm.IsActive = true;
                                atm.Cassette1Capacity = 0;
                                atm.Cassette1Denomination = 0;
                                atm.Cassette1SplitPercentage = 0;
                                atm.Cassette2Capacity = 0;
                                atm.Cassette2Denomination = 0;
                                atm.Cassette2SplitPercentage = 0;
                                atm.Cassette3Capacity = 0;
                                atm.Cassette3Denomination = 0;
                                atm.Cassette3SplitPercentage = 0;
                                atm.Cassette4Capacity = 0;
                                atm.Cassette4Denomination = 0;
                                atm.Cassette4SplitPercentage = 0;
                                atm.Cassette5Capacity = 0;
                                atm.Cassette5Denomination = 0;
                                atm.Cassette5SplitPercentage = 0;
                                atm.Cassette6Capacity = 0;
                                atm.Cassette6Denomination = 0;
                                atm.Cassette6SplitPercentage = 0;
                                atm.Cassette7Capacity = 0;
                                atm.Cassette7Denomination = 0;
                                atm.Cassette7SplitPercentage = 0;
                                atm.SuspendCashOrder = false;
                                atm.AllowedInactivityPeriod = null;
                                atm.BnaAllowedInactivityPeriod = null;
                                atm.ChequeAllowedInactivityPeriod = null;
                                atm.IsAtm = false;
                                atm.IsCdm = false;
                                atm.IsCcdm = false;
                                decimal result;

                                for (int i = 0; i < CheckBoxList_columns.Items.Count; i++)
                                {
                                    //try
                                    //{
                                    //
                                    ListItem checkBox = (ListItem)CheckBoxList_columns.Items[i];

                                    if (checkBox.Selected)
                                    {

                                        object cellValue = row[checkBox.Value];

                                        if (isValid(cellValue))
                                        {
                                            lastcol = checkBox.Value;
                                            switch (checkBox.Value)
                                            {
                                                case "Effective Date":
                                                    if (cellValue.ToString().Length > 0)
                                                    {
                                                        //atm.EffectiveDate = DateTime.ParseExact(cellValue.ToString(), "yyyyMMdd", null);
                                                        atm.EffectiveDate = Convert.ToDateTime(cellValue);
                                                        //DateTime.ParseExact(cellValue.ToString(), "MM/dd/yyyy HH:mm:ss", null);
                                                    }
                                                    break;
                                                case "ATM location name":
                                                    atm.Location = Convert.ToString(cellValue).Replace("'", "''"); ;
                                                    break;
                                                case "ATM address 1":
                                                    atm.Address1 = Convert.ToString(cellValue).Replace("'", "''");
                                                    break;
                                                case "ATM address 2_(optional)":
                                                    atm.Address2 = Convert.ToString(cellValue).Replace("'", "''"); ; ;
                                                    break;
                                                case "ATM city":
                                                    atm.City = Convert.ToString(cellValue).Replace("'", "''");
                                                    break;
                                                case "ATM country":
                                                    atm.Country = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM postal/zip code":
                                                    atm.ZipCode = Convert.ToString(cellValue);
                                                    break;
                                                case "Location Type":
                                                    atm.LocationType = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM CM Service Status":
                                                    atm.ServiceStatus = Convert.ToString(cellValue);
                                                    break;
                                                case "Holiday Status":
                                                    atm.HolidayStatus = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM Business-Days":
                                                    atm.BusinessDays = Convert.ToString(cellValue);
                                                    break;
                                                case "Time-Zone-Offset":
                                                    atm.TimeZone = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Maximum_Number of Notes per Cassette":
                                                    atm.MaxNotesPerCassette = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Denomination 1":
                                                    atm.Cassette1Denomination = Convert.ToInt32(cellValue.ToString().Substring(3));
                                                    break;
                                                case "Capacity Denomination 1":
                                                    if (atm.Cassette1Denomination != 0)
                                                        atm.Cassette1Capacity = Convert.ToInt32(cellValue) / atm.Cassette1Denomination;
                                                    break;
                                                case "Split percentage 1":
                                                    atm.Cassette1SplitPercentage = Convert.ToInt32(Convert.ToDecimal(cellValue) * 100);
                                                    break;
                                                case "Denomination 2":
                                                    atm.Cassette2Denomination = Convert.ToInt32(cellValue.ToString().Substring(3));
                                                    break;
                                                case "Capacity Denomination 2":
                                                    if (atm.Cassette2Denomination != 0)
                                                        atm.Cassette2Capacity = Convert.ToInt32(cellValue) / atm.Cassette2Denomination;
                                                    break;
                                                case "Split percentage 2":
                                                    atm.Cassette2SplitPercentage = Convert.ToInt32(Convert.ToDecimal(cellValue) * 100);
                                                    break;
                                                case "Denomination 3":
                                                    atm.Cassette3Denomination = Convert.ToInt32(cellValue.ToString().Substring(3));
                                                    break;
                                                case "Capacity Denomination 3":
                                                    if (atm.Cassette3Denomination != 0)
                                                        atm.Cassette3Capacity = Convert.ToInt32(cellValue) / atm.Cassette3Denomination;
                                                    break;
                                                case "Split percentage 3":
                                                    atm.Cassette3SplitPercentage = Convert.ToInt32(Convert.ToDecimal(cellValue) * 100);
                                                    break;
                                                case "Denomination 4":
                                                    atm.Cassette4Denomination = Convert.ToInt32(cellValue.ToString().Substring(3));
                                                    break;
                                                case "Capacity Denomination 4":
                                                    if (atm.Cassette4Denomination != 0)
                                                        atm.Cassette4Capacity = Convert.ToInt32(cellValue) / atm.Cassette4Denomination;
                                                    break;
                                                case "Split percentage 4":
                                                    atm.Cassette4SplitPercentage = Convert.ToInt32(Convert.ToDecimal(cellValue) * 100);
                                                    break;
                                                case "Interest rate":
                                                    atm.InterestRate = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Insurance rate":
                                                    if (cellValue.ToString().Length > 0)
                                                        atm.InsuranceRate = Convert.ToDecimal(cellValue.ToString());
                                                    break;
                                                case "Max- Holding Amount":
                                                    atm.MaxHoldingAmount = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Min Operating Balance _(optional)":
                                                    atm.MinOperatingBalance = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Min-amount for normal Delivery _(optional)":
                                                    atm.MinAmountForNormalDelivery = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Bank Cash Center ID":
                                                    atm.BankCashCenterId = Convert.ToString(cellValue);
                                                    break;
                                                case "CIT Cash Center / Servicer":
                                                    atm.CITCashCenterServicer = Convert.ToString(cellValue);
                                                    break;
                                                case "Depot ID":
                                                    atm.DepotId = Convert.ToString(cellValue);
                                                    break;
                                                case "secondary depot vault id":
                                                    atm.SecondaryDepotVaultId = Convert.ToString(cellValue);
                                                    break;
                                                case "New ATM Scenario":
                                                    atm.NewAtmScenario = Convert.ToString(cellValue);
                                                    break;
                                                case "Cash Swap days":
                                                    atm.CashSwapDays = Convert.ToString(cellValue);
                                                    break;
                                                case "Mandatory Cash Swap days":
                                                    atm.MandatoryCashSwapDays = Convert.ToString(cellValue);
                                                    break;
                                                case "Cash Swap cycle":
                                                    atm.CashSwapCycle = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Cash Swap Lead-Time":
                                                    atm.CashSwapLeadTime = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Cash Swap Start Date":
                                                    atm.CashSwapStartDate = Convert.ToDateTime(cellValue);
                                                    break;
                                                case "Cash Swap Handling Costs":

                                                    if (cellValue.ToString().Length > 0 && cellValue.ToString() != "0")
                                                    {

                                                        if (decimal.TryParse(cellValue.ToString(), out result))
                                                            atm.CashSwapHandlingCost = result;
                                                    }

                                                    break;
                                                case "Cash Swap Costs":

                                                    if (cellValue.ToString().Length > 0 && cellValue.ToString() != "0")
                                                    {
                                                        if (decimal.TryParse(cellValue.ToString(), out result))
                                                            atm.CashSwapCosts = result;
                                                    }

                                                    //atm.CashSwapCosts = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Emergency days":
                                                    atm.EmergencyDays = Convert.ToString(cellValue);
                                                    break;
                                                case "Emergency Lead-Time":
                                                    atm.EmergencyLeadTime = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Emergency Costs":
                                                    atm.EmergencyCost = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Contact A":
                                                    atm.Contact1Email = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact A Phone":
                                                    atm.Contact1Phone = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact B":
                                                    atm.Contact2Email = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact B Phone":
                                                    atm.Contact2Phone = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact C":
                                                    atm.Contact3Email = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact C Phone":
                                                    atm.Contact3Phone = Convert.ToString(cellValue);
                                                    break;
                                                case "Longitude":
                                                    atm.Longitude = Convert.ToString(cellValue);
                                                    break;
                                                case "Latitude":
                                                    atm.Latitude = Convert.ToString(cellValue);
                                                    break;
                                                case "Distribution Port":
                                                    atm.DistributionPort = string.IsNullOrEmpty(Convert.ToString(cellValue)) ? 0 : Convert.ToInt32(cellValue);
                                                    break;
                                                case "ATM Type":
                                                    string atmType = Convert.ToString(cellValue);

                                                    if (atmType == "2in1")
                                                    {
                                                        atm.IsAtm = true;
                                                        atm.IsCdm = true;
                                                        atm.BnaAllowedInactivityPeriod= atm.AllowedInactivityPeriod = 12600;

                                                    }
                                                    else if (atmType == "3in1")
                                                    {
                                                        atm.IsAtm = true;
                                                        atm.IsCdm = true;
                                                        atm.IsCcdm = true;
                                                       atm.ChequeAllowedInactivityPeriod= atm.BnaAllowedInactivityPeriod = atm.AllowedInactivityPeriod = 12600;

                                                    }
                                                    else if (atmType.ToLower() == "atm")
                                                    {
                                                        atm.IsAtm = true;
                                                        atm.AllowedInactivityPeriod = 12600;
                                                    }
                                                    else if (atmType.ToLower() == "deposit")
                                                    {
                                                        atm.IsCdm = true;
                                                        atm.BnaAllowedInactivityPeriod = 12600;
                                                    }
                                                    break;
                                                case "Vendor":
                                                    atm.AtmType = Convert.ToString(cellValue);
                                                    break;
                                            }
                                        }
                                    }
                                    //}
                                    //catch (Exception ex)
                                    //{

                                    //}
                                }

                                atm.IsCounterEnabled = true;
                                if (!atm.MinOperatingBalance.HasValue)
                                    atm.MinOperatingBalance = 100000;

                                atm.StartupSleepInterval = 300;
                                atm.CountsClearRetries = 3;
                                atm.TCPTimeout = 2000;
                                atm.SleepInterval = 1000;
                                atm.CPMCommandWait = 30000;
                                atm.CPMCommandSleep = 60000;
                                atm.AANDCApplications1 = "SSDSAPP_applicationcore,";
                                atm.AANDCApplications2 = "SSDSAPP_applicationcore,";
                                atm.AANDCApplications3 = "SSDSAPP_applicationcore,";
                                atm.AANDCApplications4 = "SSDSAPP_applicationcore,";
                                atm.AANDCApplications5 = "SSDSAPP_applicationcore,";
                                atm.MonitoringRetries = 5;
                                atm.WindowSwitchSleep = 500;
                                atm.AppSwitchSleep = 60000;
                                atm.MonitoringCycleSleep = 5;
                                atm.CPMLogLevel = 4;
                                atm.IsDispenserRealTimeNotificationEnabled = false;
                                atm.IsBNARealTimeNotificationEnabled = false;
                                atm.IsCPMRealTimeNotificationEnabled = false;
                                atm.IsReplenishmentRealTimeNotificationEnabled = false;
                                atm.IsOutOfCashRealTimeNotificationEnabled = false;
                                atm.IsDispenserMismatchRealTimeNotificationEnabled = false;
                                atm.IsBNAMismatchRealTimeNotificationEnabled = false;
                                atm.IsCPMMismatchRealTimeNotificationEnabled = false;
                                atm.IsCounterExplodedRealTimeNotificationEnabled = false;
                                atm.Type1MinimumNotes = 500;
                                atm.Type2MinimumNotes = 500;
                                atm.Type3MinimumNotes = 500;
                                atm.Type4MinimumNotes = 500;
                                atm.Type5MinimumNotes = 500;
                                atm.Type6MinimumNotes = 500;
                                atm.Type7MinimumNotes = 500;

                                AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                                atm.RetryCountCounterFile = appSetting.RetryCountCounterFile;
                                atm.RetryCountDatetimeSchedule = appSetting.RetryCountDatetimeSchedule;
                                atm.RetryCountRestartSchedule = appSetting.RetryCountRestartSchedule;
                                //atm.RetryCountCashOrderDownload = int.Parse(TextBox_NumberOfRetryCashOrderDownload.Text);
                                atm.RetryCountCashOrderUpload = appSetting.RetryCountCashOrderUpload;
                                atm.RetryCountConfUpload = appSetting.RetryCountConfUpload;
                                atm.Save(connection, transaction);

                                new UserATMs(((AppUser)Session[SessionVars.user.ToString()]).UserId, atm.ATMId).Save(connection, transaction);
                                if (((AppUser)Session[SessionVars.user.ToString()]).UserId != 2)
                                    new UserATMs(2, atm.ATMId).Save(connection, transaction);

                                mcnAtms = McnAtms.LoadMcnAtms("atm_id = " + atm.ATMId + " and region_id = " + region.RegionId);
                                if (mcnAtms == null)
                                {
                                    mcnAtms = new McnAtms();
                                    mcnAtms.AtmId = atm.ATMId;
                                    //mcnAtms.RegionId = region.RegionId;
                                    mcnAtms.RegionId = organization.RegionId;
                                    mcnAtms.Mcn = organization.MCN;
                                    mcnAtms.Save(connection, transaction);
                                }

                                if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task with (nolock) where task_type_id = " + (int)EnumTaskType.Configuration + " and status in ('scheduled','uploadingDisconnected') and atm_id = " + atm.ATMId) == 0)
                                {
                                    Task configTask = new Task();
                                    configTask.ATMId = atm.ATMId;
                                    configTask.BytesTransferred = 0;
                                    configTask.CreatedBy = 1;
                                    configTask.CreationTime = DateTime.Now;
                                    configTask.Parsed = false;
                                    configTask.RetryRemaining = 10;
                                    configTask.Status = UploadStates.scheduled.ToString();
                                    configTask.TaskTypeId = (int)EnumTaskType.Configuration;
                                    configTask.UploadTime = new DateTime(1900, 1, 1);
                                    configTask.Save(connection, transaction);
                                }

                                if ((int)ConnectionFactory.ExecuteScalar("select count(*) from scheduled_downloads with (nolock) where atm_id = " + atm.ATMId + " and file_type_id = 1") == 0)
                                {
                                    ScheduledDownloads objScheduledDwnlds = new ScheduledDownloads();
                                    objScheduledDwnlds.ATMId = atm.ATMId;
                                    objScheduledDwnlds.FileTypeId = 1;
                                    objScheduledDwnlds.Save(connection, transaction);
                                }

                                Utility.BuildAuditLog("ATM \"" + row["ATM-ID"] + "\" created through IMAC sheet", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ImportATMsFromIMAC, connection, transaction);
                                break;
                            #endregion

                            #region Change
                            case "C":
                                bool isCapacityChanged = false, isDenominationChanged = false;
                                int? oldCapacity, oldDenomination;

                                atm = Atm.LoadAtm("title like '" + row["ATM-ID"] + "'");

                                if (atm == null)
                                    throw new Exception("ATM with friendly name \"" + row["ATM-ID"] + "\" does not exist");

                                organization = Region.LoadRegion("MCN like '" + row["ID4"] + "'");
                                if (organization == null)
                                {
                                    organization = new Region();
                                    organization.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                    organization.CreationTime = DateTime.Now;
                                    organization.IsActive = true;
                                    organization.MCN = row["ID4"].ToString();
                                    organization.RegionName = row["ID4"].ToString();
                                    organization.IsOrganization = true;
                                    organization.ParentRegionId = 1;
                                    organization.Save(connection, transaction);
                                }

                                region = Region.LoadRegion("region_name like '" + row["Region ID"] + "'");
                                if (region == null)
                                {
                                    region = new Region();
                                    region.RegionName = row["Region ID"].ToString();
                                    region.ParentRegionId = organization.RegionId;
                                    region.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                    region.IsActive = true;
                                    region.CreationTime = DateTime.Now;
                                    region.IsOrganization = false;
                                    region.Save(connection, transaction);
                                }

                                userOrganization = UserOrganization.LoadUserOrganization("org_mcn like '" + row["ID4"] + "' and user_id = " + ((AppUser)Session[SessionVars.user.ToString()]).UserId);
                                if (userOrganization == null)
                                {
                                    userOrganization = new UserOrganization();
                                    userOrganization.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                    userOrganization.OrgMcn = row["ID4"].ToString();
                                    userOrganization.Save(connection, transaction);
                                }

                                //atm.AtmFriendlyName = row["ATM-ID"].ToString();
                                atm.Title = row["ATM-ID"].ToString();
                                atm.RegionId = region.RegionId;
                                atm.NoteSetTypeId = Avanza.CCMS.DAL.NoteSetType.LoadNoteSetType("note_set_type_name='" + row["note_set_type"] + "'").NoteSetTypeId;
                                //atm.RegionName = row["Region ID"].ToString();
                                atm.IP = row["IP Address"].ToString();
                                atm.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                                atm.IsActive = true;

                                for (int i = 0; i < CheckBoxList_columns.Items.Count; i++)
                                {
                                    ListItem checkBox = (ListItem)CheckBoxList_columns.Items[i];

                                    if (checkBox.Selected)
                                    {
                                        object cellValue = DetermineValue(row[checkBox.Value]);

                                        if (isValid(cellValue))
                                        {
                                            switch (checkBox.Value)
                                            {
                                                case "Effective Date":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.EffectiveDate = null;
                                                    else
                                                        atm.EffectiveDate = Convert.ToDateTime(cellValue);
                                                    break;
                                                case "ATM location name":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Location = null;
                                                    else
                                                        atm.Location = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM address 1":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Address1 = null;
                                                    else
                                                        atm.Address1 = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM address 2_(optional)":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Address2 = null;
                                                    else
                                                        atm.Address2 = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM city":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.City = null;
                                                    else
                                                        atm.City = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM country":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Country = null;
                                                    else
                                                        atm.Country = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM postal/zip code":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.ZipCode = null;
                                                    else
                                                        atm.ZipCode = Convert.ToString(cellValue);
                                                    break;
                                                case "Location Type":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.LocationType = null;
                                                    else
                                                        atm.LocationType = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM CM Service Status":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.ServiceStatus = null;
                                                    else
                                                        atm.ServiceStatus = Convert.ToString(cellValue);
                                                    break;
                                                case "Holiday Status":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.HolidayStatus = null;
                                                    else
                                                        atm.HolidayStatus = Convert.ToString(cellValue);
                                                    break;
                                                case "ATM Business-Days":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.BusinessDays = null;
                                                    else
                                                        atm.BusinessDays = Convert.ToString(cellValue);
                                                    break;
                                                case "Time-Zone-Offset":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.TimeZone = null;
                                                    else
                                                        atm.TimeZone = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Maximum_Number of Notes per Cassette":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.MaxNotesPerCassette = null;
                                                    else
                                                        atm.MaxNotesPerCassette = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Denomination 1":
                                                    oldDenomination = atm.Cassette1Denomination;
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette1Denomination = 0;
                                                    else
                                                        atm.Cassette1Denomination = Convert.ToInt32(cellValue.ToString().Substring(3));
                                                    if (oldDenomination != atm.Cassette1Denomination)
                                                        isDenominationChanged = true;
                                                    break;
                                                case "Capacity Denomination 1":
                                                    oldCapacity = atm.Cassette1Capacity;
                                                    if (atm.Cassette1Denomination == 0 || cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette1Capacity = 0;
                                                    else
                                                        atm.Cassette1Capacity = Convert.ToInt32(cellValue) / atm.Cassette1Denomination;
                                                    if (oldCapacity != atm.Cassette1Capacity)
                                                        isCapacityChanged = true;
                                                    break;
                                                case "Split percentage 1":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette1SplitPercentage = 0;
                                                    else
                                                        atm.Cassette1SplitPercentage = Convert.ToInt32(Convert.ToDecimal(cellValue) * 100);
                                                    break;
                                                case "Denomination 2":
                                                    oldDenomination = atm.Cassette2Denomination;
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette2Denomination = 0;
                                                    else
                                                        atm.Cassette2Denomination = Convert.ToInt32(cellValue.ToString().Substring(3));
                                                    if (oldDenomination != atm.Cassette2Denomination)
                                                        isDenominationChanged = true;
                                                    break;
                                                case "Capacity Denomination 2":
                                                    oldCapacity = atm.Cassette2Capacity;
                                                    if (atm.Cassette2Denomination == 0 || cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette2Capacity = 0;
                                                    else
                                                        atm.Cassette2Capacity = Convert.ToInt32(cellValue) / atm.Cassette2Denomination;
                                                    if (oldCapacity != atm.Cassette2Capacity)
                                                        isCapacityChanged = true;
                                                    break;
                                                case "Split percentage 2":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette2SplitPercentage = 0;
                                                    else
                                                        atm.Cassette2SplitPercentage = Convert.ToInt32(Convert.ToDecimal(cellValue) * 100);
                                                    break;
                                                case "Denomination 3":
                                                    oldDenomination = atm.Cassette3Denomination;
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette3Denomination = 0;
                                                    else
                                                        atm.Cassette3Denomination = Convert.ToInt32(cellValue.ToString().Substring(3));
                                                    if (oldDenomination != atm.Cassette3Denomination)
                                                        isDenominationChanged = true;
                                                    break;
                                                case "Capacity Denomination 3":
                                                    oldCapacity = atm.Cassette3Capacity;
                                                    if (atm.Cassette3Denomination == 0 || cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette3Capacity = 0;
                                                    else
                                                        atm.Cassette3Capacity = Convert.ToInt32(cellValue) / atm.Cassette3Denomination;
                                                    if (oldCapacity != atm.Cassette3Capacity)
                                                        isCapacityChanged = true;
                                                    break;
                                                case "Split percentage 3":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette3SplitPercentage = 0;
                                                    else
                                                        atm.Cassette3SplitPercentage = Convert.ToInt32(Convert.ToDecimal(cellValue) * 100);
                                                    break;
                                                case "Denomination 4":
                                                    oldDenomination = atm.Cassette4Denomination;
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette4Denomination = 0;
                                                    else
                                                        atm.Cassette4Denomination = Convert.ToInt32(cellValue.ToString().Substring(3));
                                                    if (oldDenomination != atm.Cassette4Denomination)
                                                        isDenominationChanged = true;
                                                    break;
                                                case "Capacity Denomination 4":
                                                    oldCapacity = atm.Cassette4Capacity;
                                                    if (atm.Cassette4Denomination == 0 || cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette4Capacity = 0;
                                                    else
                                                        atm.Cassette4Capacity = Convert.ToInt32(cellValue) / atm.Cassette4Denomination;
                                                    if (oldCapacity != atm.Cassette4Capacity)
                                                        isCapacityChanged = true;
                                                    break;
                                                case "Split percentage 4":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Cassette4SplitPercentage = 0;
                                                    else
                                                        atm.Cassette4SplitPercentage = Convert.ToInt32(Convert.ToDecimal(cellValue) * 100);
                                                    break;
                                                case "Interest rate":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.InterestRate = null;
                                                    else
                                                        atm.InterestRate = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Insurance rate":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.InsuranceRate = null;
                                                    else
                                                        atm.InsuranceRate = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Max- Holding Amount":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.MaxHoldingAmount = null;
                                                    else
                                                        atm.MaxHoldingAmount = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Min Operating Balance _(optional)":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.MinOperatingBalance = null;
                                                    else
                                                        atm.MinOperatingBalance = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Min-amount for normal Delivery _(optional)":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.MinAmountForNormalDelivery = null;
                                                    else
                                                        atm.MinAmountForNormalDelivery = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Bank Cash Center ID":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.BankCashCenterId = null;
                                                    else
                                                        atm.BankCashCenterId = Convert.ToString(cellValue);
                                                    break;
                                                case "CIT Cash Center / Servicer":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.CITCashCenterServicer = null;
                                                    else
                                                        atm.CITCashCenterServicer = Convert.ToString(cellValue);
                                                    break;
                                                case "Depot ID":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.DepotId = null;
                                                    else
                                                        atm.DepotId = Convert.ToString(cellValue);
                                                    break;
                                                case "secondary depot vault id":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.SecondaryDepotVaultId = null;
                                                    else
                                                        atm.SecondaryDepotVaultId = Convert.ToString(cellValue);
                                                    break;
                                                case "New ATM Scenario":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.NewAtmScenario = null;
                                                    else
                                                        atm.NewAtmScenario = Convert.ToString(cellValue);
                                                    break;
                                                case "Cash Swap days":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.CashSwapDays = null;
                                                    else
                                                        atm.CashSwapDays = Convert.ToString(cellValue);
                                                    break;
                                                case "Mandatory Cash Swap days":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.MandatoryCashSwapDays = null;
                                                    else
                                                        atm.MandatoryCashSwapDays = Convert.ToString(cellValue);
                                                    break;
                                                case "Cash Swap cycle":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.CashSwapCycle = null;
                                                    else
                                                        atm.CashSwapCycle = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Cash Swap Lead-Time":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.CashSwapLeadTime = null;
                                                    else
                                                        atm.CashSwapLeadTime = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Cash Swap Start Date":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.CashSwapStartDate = null;
                                                    else
                                                        atm.CashSwapStartDate = Convert.ToDateTime(cellValue);
                                                    break;
                                                case "Cash Swap Handling Costs":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.CashSwapHandlingCost = null;
                                                    else
                                                    {
                                                        //   atm.CashSwapHandlingCost = Convert.ToDecimal(cellValue);
                                                        if (cellValue.ToString().Length > 0 && cellValue.ToString() != "0")
                                                        {
                                                            if (decimal.TryParse(cellValue.ToString(), out result))
                                                                atm.CashSwapHandlingCost = result;
                                                        }
                                                    }
                                                    break;
                                                case "Cash Swap Costs":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.CashSwapCosts = null;
                                                    else
                                                    {
                                                        if (cellValue.ToString().Length > 0 && cellValue.ToString() != "0")
                                                        {
                                                            if (decimal.TryParse(cellValue.ToString(), out result))
                                                                atm.CashSwapCosts = result;
                                                        }

                                                        //atm.CashSwapCosts = Convert.ToDecimal(cellValue);

                                                    }
                                                    break;
                                                case "Emergency days":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.EmergencyDays = null;
                                                    else
                                                        atm.EmergencyDays = Convert.ToString(cellValue);
                                                    break;
                                                case "Emergency Lead-Time":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.EmergencyLeadTime = null;
                                                    else
                                                        atm.EmergencyLeadTime = Convert.ToInt32(cellValue);
                                                    break;
                                                case "Emergency Costs":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.EmergencyCost = null;
                                                    else
                                                        atm.EmergencyCost = Convert.ToDecimal(cellValue);
                                                    break;
                                                case "Contact A":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Contact1Email = null;
                                                    else
                                                        atm.Contact1Email = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact A Phone":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Contact1Phone = null;
                                                    else
                                                        atm.Contact1Phone = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact B":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Contact2Email = null;
                                                    else
                                                        atm.Contact2Email = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact B Phone":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Contact2Phone = null;
                                                    else
                                                        atm.Contact2Phone = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact C":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Contact3Email = null;
                                                    else
                                                        atm.Contact3Email = Convert.ToString(cellValue);
                                                    break;
                                                case "Contact C Phone":
                                                    if (cellValue.ToString() == DropDownList_specialCharacter.SelectedItem.Value)
                                                        atm.Contact3Phone = null;
                                                    else
                                                        atm.Contact3Phone = Convert.ToString(cellValue);
                                                    break;

                                                case "Longitude":
                                                    atm.Longitude = Convert.ToString(cellValue);
                                                    break;
                                                case "Latitude":
                                                    atm.Latitude = Convert.ToString(cellValue);
                                                    break;
                                                case "Distribution Port":
                                                    atm.DistributionPort = string.IsNullOrEmpty(Convert.ToString(cellValue)) ? 0 : Convert.ToInt32(cellValue);
                                                    break;
                                                case "ATM Type":
                                                    string atmType = Convert.ToString(cellValue);

                                                    if (atmType == "2in1")
                                                    {
                                                        atm.IsAtm = true;
                                                        atm.IsCdm = true;
                                                        atm.BnaAllowedInactivityPeriod = atm.AllowedInactivityPeriod = 12600;

                                                    }
                                                    else if (atmType == "3in1")
                                                    {
                                                        atm.IsAtm = true;
                                                        atm.IsCdm = true;
                                                        atm.IsCcdm = true;
                                                        atm.ChequeAllowedInactivityPeriod = atm.BnaAllowedInactivityPeriod = atm.AllowedInactivityPeriod = 12600;

                                                    }
                                                    else if (atmType.ToLower() == "atm")
                                                    {
                                                        atm.IsAtm = true;
                                                        atm.AllowedInactivityPeriod = 12600;
                                                    }
                                                    else if (atmType.ToLower() == "deposit")
                                                    {
                                                        atm.IsCdm = true;
                                                        atm.BnaAllowedInactivityPeriod = 12600;
                                                    }
                                                    break;
                                                case "Vendor":
                                                    atm.AtmType = Convert.ToString(cellValue);
                                                    break;
                                            }
                                        }
                                    }
                                }

                                atm.Save(connection, transaction);
                                mcnAtms = McnAtms.LoadMcnAtms("atm_id = " + atm.ATMId + " and region_id = " + region.RegionId);
                                if (mcnAtms == null)
                                {
                                    mcnAtms = new McnAtms();
                                    mcnAtms.AtmId = atm.ATMId;
                                    mcnAtms.RegionId = region.RegionId;
                                    mcnAtms.Mcn = organization.MCN;
                                    mcnAtms.Save(connection, transaction);
                                }

                                if (isCapacityChanged || isDenominationChanged)
                                {
                                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task with (nolock) where task_type_id = " + (int)EnumTaskType.Configuration + " and status in ('scheduled','uploadingDisconnected') and atm_id = " + atm.ATMId) == 0)
                                    {
                                        Task configTask = new Task();
                                        configTask.ATMId = atm.ATMId;
                                        configTask.BytesTransferred = 0;
                                        configTask.CreatedBy = 1;
                                        configTask.CreationTime = DateTime.Now;
                                        configTask.Parsed = false;
                                        configTask.RetryRemaining = 10;
                                        configTask.Status = UploadStates.scheduled.ToString();
                                        configTask.TaskTypeId = (int)EnumTaskType.Configuration;
                                        configTask.UploadTime = new DateTime(1900, 1, 1);
                                        configTask.Save(connection, transaction);
                                    }
                                }

                                Utility.BuildAuditLog("ATM \"" + row["ATM-ID"] + "\" edited through IMAC sheet", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyATM, connection, transaction);
                                break;
                            #endregion

                            #region Remove
                            case "R":
                                atm = Atm.LoadAtm("title like '" + row["ATM-ID"] + "'");

                                if (atm != null)
                                {
                                    atm.IsActive = false;
                                    atm.Save(connection, transaction);
                                    Utility.BuildAuditLog("ATM \"" + row["ATM-ID"] + "\" removed through IMAC sheet", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteATM, connection, transaction);
                                }
                                else
                                    throw new Exception("Atm \"" + row["ATM-ID"] + "\" does not exist in database");
                                break;
                            #endregion
                        }
                    }
                }

                if (transaction != null)
                {
                    transaction.Commit();
                    Session["currentPage"] = "admin/ATMs.aspx";
                }

            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                LogableTask.LogMonoActivityTask("test", MethodBase.GetCurrentMethod(), TraceLevel.Info, "error in processing col " + lastcol + " of row " + rowCount);
                throw;// new Exception("Unable to save ATMS. " + ex.Message);
            }
            finally
            {
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
        protected bool isComplete(DataRow row)
        {
            if (row["ChangeType"] == null)
                throw new Exception("Change Type cannot be null");

            if (row["ChangeType"].ToString().Replace(" ", "").Equals(String.Empty))
                throw new Exception("Change Type cannot be empty");

            if (!row["ChangeType"].ToString().Equals("A") && !row["ChangeType"].ToString().Equals("C") && !row["ChangeType"].ToString().Equals("R"))
                throw new Exception("Change Type not supported");

            if (row["ATM-ID"] == null)
                throw new Exception("ATM Name cannot be null");

            if (!row["ChangeType"].ToString().Equals("C"))
                if (row["ATM-ID"].ToString().Replace(" ", "").Equals(String.Empty))
                    throw new Exception("ATM Name cannot be empty");

            if (row["ATM-ID"].ToString().Trim().Equals(DropDownList_specialCharacter.SelectedItem.Value))
                throw new Exception("ATM Name cannot be set as null");

            if (row["Region ID"] == null)
                throw new Exception("Region Name cannot be null");

            if (!row["ChangeType"].ToString().Equals("C"))
                if (row["Region ID"].ToString().Replace(" ", "").Equals(String.Empty))
                    throw new Exception("Region Name cannot be empty");

            if (row["Region ID"].ToString().Trim().Equals(DropDownList_specialCharacter.SelectedItem.Value))
                throw new Exception("Region Name cannot be set as null");

            if (row["ID4"] == null)
                throw new Exception("MCN cannot be null");

            if (!row["ChangeType"].ToString().Equals("C"))
                if (row["ID4"].ToString().Replace(" ", "").Equals(String.Empty))
                    throw new Exception("MCN cannot be empty");

            if (row["ID4"].ToString().Trim().Equals(DropDownList_specialCharacter.SelectedItem.Value))
                throw new Exception("MCN cannot be set as null");

            if (row["IP Address"] == null)
                throw new Exception("ATM IP Address cannot be null");

            if (!row["ChangeType"].ToString().Equals("C"))
                if (row["IP Address"].ToString().Replace(" ", "").Equals(String.Empty))
                    throw new Exception("ATM IP cannot be empty");

            if (row["IP Address"].ToString().Trim().Equals(DropDownList_specialCharacter.SelectedItem.Value))
                throw new Exception("ATM IP cannot be set as null");

            return true;
        }

        protected bool isValid(object value)
        {
            if (value == null)
                return false;
            if (value.ToString().Replace(" ", "").Equals(String.Empty))
                return false;
            return true;
        }

        protected object DetermineValue(object newValue)
        {
            //only for Change ATM case

            if (newValue == null) //if null, do nothing
                return null;
            else if (newValue.ToString().Replace(" ", "").Equals(String.Empty)) //if empty, do nothing
                return null;
            else if (newValue.ToString().Trim().Equals(DropDownList_specialCharacter.SelectedItem.Value)) //if special character, make null
                return DropDownList_specialCharacter.SelectedItem.Value;
            else //if valid, change old value with new value
                return newValue;
        }
    }
}