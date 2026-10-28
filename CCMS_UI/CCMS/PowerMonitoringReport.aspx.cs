using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data;
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Web.Configuration;
using System.Data.OleDb;
using System.Configuration;

namespace CCMSUI.CCMS
{
    public partial class PowerMonitoringReport : BasePage
    {
        static string ticketsToCheck = "Communication Failure";
        enum columnsMain
        {
            Title = 0,
            TicketId = 1,
            IP = 2,
            Location = 3,
            Region = 4,
            StatusDescription = 5,
            FromDate = 6,
            FromTime = 7,
            ToDate = 8,
            ToTime = 9,
            TotalDowntime = 10,
            Reason = 11,
            Organization = 12
        };
        private void BindNoteSetType()
        {
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && regionID != "1")
            {
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                while (reader.Read())
                {
                    //DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                }
                reader.Close();
            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindNoteSetType();
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewPowerMonitoringReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00";
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59";
                BindNoteSetType();
               
            }
        }
        private void PopulateDataTable(DataTable dt)
        {
            int region_id = 0;
            int id = -1;

            DataTable dtRegions = new DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region where parent_region_id is not null";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtRegions);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                region_id = int.Parse(dt.Rows[i][9].ToString()); //parent


                while (true)
                {
                    DataRow[] innerArray = dtRegions.Select("region_id=" + region_id);
                    if (bool.Parse(innerArray[0][2].ToString()))
                    {
                        id = int.Parse(innerArray[0][0].ToString());
                        break;
                    }
                    else
                        region_id = int.Parse(innerArray[0][1].ToString());

                }
                //organization id found
                Region region = Region.LoadRegionByPk(id);
                dt.Rows[i][8] = region.RegionName;
                //dt.Rows[i][4] = region.BankLogo;

            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Generate Report");
            try
            {
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;

                //string filePathTicketHistorySheet = @"C:\CCMSLiveData\Temp\Guesspar\AIMA04 - Ticket History - ALL DATA - monthly -NPE BNP Paribas Egypt.xlsx";
                DateTime dtFrom = DateTime.Parse(TextBox_FromDate.Text);      //new DateTime(2015, 7, 30, 0, 0, 0);
                DateTime dtTo = DateTime.Parse(TextBox_ToDate.Text);    //toDate;     //new DateTime(2015, 7, 31, 23, 59, 0);
                DataSetPowerMonitoring ds = new DataSetPowerMonitoring();
                //DataRow[] dataRow = CollectRequiredDataFromExcelSheet(filePathTicketHistorySheet, "Report 1", "[Ticket Problem Bucket TH] = 'Communication Failure'", dtFrom, dtTo);
                ////DataTable testTable = CreateDateTable(dataRow);     //Only to test what is inside in dataRow Array
                //ExtractInformationFromExcelSheet(dataRow, dtFrom, dtTo, ds);
                DataTable dataTableTickets = CollectGasperTicketsFromDatabase(dtFrom, dtTo);
                ExtractInformationFromDB(dataTableTickets, dtFrom, dtTo, ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Session["subReport"] = ds.Tables[1].DataSet;
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=PowerMonitoringRpt.rpt&ReportTitle=Power Monitoring Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["dtMain"].Rows.Count;
                    Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }
        }
         protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00";
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59";
        }

         public static DataTable CollectGasperTicketsFromDatabase(DateTime dtFrom, DateTime dtTo)
         {
             LogableTask.LogMonoActivityTask("Collecting Data", MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Collect Data  from DB");
             DataTable dtTicketHistory = new DataTable();
             try
             {
                 //        string queryGetGasperData = "select title as [ATM ID], atm_id, ticket_id as [Ticket ID], start_date, end_date, address_1 as [Address 1],Reason from gasper_ticket where (end_date >= convert(datetime,'" + dtFrom + "',103) or start_date <= convert(datetime,'" + dtTo.ToString("dd/MM/yyyy") + " 23:59:59',103))  And Reason = '" + ticketsToCheck + "'";

                 //Change in query by Ali Shah on 21 Feb, 16
                 string queryGetGasperData = "select title as [ATM ID], atm_id, ticket_id as [Ticket ID], start_date, " +
                                             " end_date, address_1 as [Address 1],Reason from gasper_ticket " +
                     //                            " where start_date >= convert(datetime,'" + dtFrom + "',103) and end_date <= convert(datetime,'" + dtTo.ToString("dd/MM/yyyy") + " 23:59:59',103)  And Reason = '" + ticketsToCheck + "'";
                                              " where start_date <= CONVERT(datetime,'" + dtTo + "',103) And (end_date is null OR end_date >= convert(datetime,'" + dtFrom + "',103)) And Reason = '" + ticketsToCheck + "'";

                 //string queryGetGasperData = "select title as [ATM ID], atm_id, ticket_id as [Ticket ID], start_date, " +
                 //                            " end_date, address_1 as [Address 1],Reason from gasper_ticket " +
                 //    //                            " where start_date >= convert(datetime,'" + dtFrom + "',103) and end_date <= convert(datetime,'" + dtTo.ToString("dd/MM/yyyy") + " 23:59:59',103)  And Reason = '" + ticketsToCheck + "'";
                 //                             " where ((start_date >= convert(datetime,'" + dtFrom + "',103) And start_date < CONVERT(datetime,'" + dtTo + "',103)) OR (end_date >= convert(datetime,'" + dtFrom + "',103) And end_date < CONVERT(datetime,'" + dtTo + "',103))) And Reason = '" + ticketsToCheck + "'";

                 LogableTask.LogMonoActivityTask("Error occured while collecting tickets from Database", MethodBase.GetCurrentMethod(), TraceLevel.Error, queryGetGasperData);
                 SqlCommand cmdGetGasperData = ConnectionFactory.GetNewCommand(false);
                 cmdGetGasperData.CommandText = queryGetGasperData;
                 SqlDataAdapter adptrGasperData = new SqlDataAdapter(cmdGetGasperData);
                 adptrGasperData.Fill(dtTicketHistory);
             }
             catch (Exception ex)
             {
                 LogableTask.LogMonoActivityTask("Error occured while collecting tickets from Database", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex.Message);
             }
             return dtTicketHistory;
         }

         public void ExtractInformationFromDB(DataTable dataTableTickets, DateTime dtFrom, DateTime dtTo, DataSet dsPowerMonitoring)
         {
             LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Info, "Going to Extract Information");
             SqlConnection conn = ConnectionFactory.GetNewConnection();
             DataTable dtDownTimeDetail = new DataTable();
             DataTable dtAtms = new DataTable();
             DataTable dtPowerupEvents = new DataTable();
             //Edited by Ali Shah on 29 Mar, 2016
             //Variable added to facilitate 'Power Down only' events as per MCB Farhan Request
             string reasonDescription = "";
             //Change done by Ali Shah
             //As there is a chance that Key is not included in config file.
             string strInterfaceDetailOffset = ConfigurationManager.AppSettings["InterfaceDetailOffsetTimeInMin"];
             if (string.IsNullOrEmpty(strInterfaceDetailOffset))
             {
                 LogableTask.LogMonoActivityTask("Extracting InterfaceDetailOffsetTimeInMin Key", MethodBase.GetCurrentMethod(), TraceLevel.Error, "InterfaceDetailOffsetTimeInMin key is not available in Config file. Initialized with 30");
                 strInterfaceDetailOffset = "30";
             }
             double interfaceDetailOffsetTimeInMin = double.Parse(strInterfaceDetailOffset);

             //       conn.Open();
             //string queryDownTimeDetail = "select title, interface_friendly_name, atmInfo.ip as ip, from_date, to_date, interface_status from atm_interface_info as atmInfo inner join atm_interface_info_history as infoHistory on atmInfo.atm_interface_info_id = infoHistory.atm_interface_info_id inner join atm on atmInfo.atm_id = atm.ATM_id where to_date >= convert(datetime,'" + dtFrom + "',103) or from_date <= convert(datetime,'" + dtTo.ToString("dd/MM/yyyy") + " 23:59:59',103)";
             //string queryDownTimeDetail = "select title, interface_friendly_name, atmInfo.ip as ip, from_date, to_date, interface_status " +
             //                             " from atm_interface_info as atmInfo inner join atm_interface_info_history as infoHistory " +
             //                             " on atmInfo.atm_interface_info_id = infoHistory.atm_interface_info_id " +
             //                             " inner join atm on atmInfo.atm_id = atm.ATM_id " +
             //    //" where from_date >= convert(datetime,'" + dtFrom + "',103) and to_date <= convert(datetime,'" + dtTo.ToString("dd/MM/yyyy") + " 23:59:59',103)";
             //                             " where from_date >= convert(datetime,'" + dtFrom + "',103) ";

             //Change in query by Ali Shah on 22 Feb, 16
             string queryDownTimeDetail = "select title, interface_friendly_name, atmInfo.ip as ip, from_date, to_date, interface_status " +
                                          " from atm_interface_info as atmInfo inner join atm_interface_info_history as infoHistory " +
                                          " on atmInfo.atm_interface_info_id = infoHistory.atm_interface_info_id " +
                                          " inner join atm on atmInfo.atm_id = atm.ATM_id " +
                                          " where to_date > convert(datetime,'" + dtFrom + "',103) and from_date < convert(datetime,'" + dtTo + "',103)";



             SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
             cmd.CommandText = queryDownTimeDetail;
             LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Error, cmd.CommandText);
             SqlDataAdapter adpter = new SqlDataAdapter(cmd);
             adpter.Fill(dtDownTimeDetail);

             //Collect IP information by passing ATM Title.
             string queryGetAtmIP = "Select atm.title, atm.ip, atm.atm_id, atm.region_id, region.region_name " +
                                     " from atm inner join region on atm.region_id = region.region_id " +
                                     " where atm.is_active =1";
             cmd = ConnectionFactory.GetNewCommand(false);
             cmd.CommandText = queryGetAtmIP;
             LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Error, cmd.CommandText);
             adpter.SelectCommand = cmd;
             adpter.Fill(dtAtms);

             //Collect Powerup Events
             string queryGetPowerupEvents = "Select atm_id, powerup_time from ej_powerup_events " +
                                            " where powerup_time >= convert(datetime,'" + dtFrom + "',103) " +
                                             " and powerup_time <= convert(datetime,'" + dtTo.ToString("dd/MM/yyyy") + " 23:59:59',103)";
             cmd = ConnectionFactory.GetNewCommand(false);
             cmd.CommandText = queryGetPowerupEvents;
             adpter.SelectCommand = cmd;
             LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Error, cmd.CommandText);
             adpter.Fill(dtPowerupEvents);

             foreach (DataRow row in dataTableTickets.Rows)
             {
                 DateTime atmDownTime = Convert.ToDateTime(row["start_date"]);  // DateTime.Parse(row["Start Date Local"].ToString()) + TimeSpan.Parse(row["Start Time Local"].ToString());
                 DateTime atmUpTime = DateTime.Now;
                 if (row["end_date"] != DBNull.Value)
                     atmUpTime = Convert.ToDateTime(row["end_date"]);  //DateTime.Parse(row["End Date Local"].ToString()) + TimeSpan.Parse(row["End Time Local"].ToString());
                 //Only for testing
                 //DataRow[] tempDetailArray = dtDownTimeDetail.Select(" title = '" + row["ATM ID"] + "'");

                 //Change in selection query by Ali Shah on 22 Feb, 16
                 //Purpose is to identify those interface details which relevant to the gasper ticket
                 DataRow[] downtimeDetailArray = dtDownTimeDetail.Select(" title = '" + row["ATM ID"] + "' And from_date > '" + atmDownTime.AddMinutes(-interfaceDetailOffsetTimeInMin) + "' And from_date < '" + atmDownTime.AddMinutes(interfaceDetailOffsetTimeInMin) + "'");
                 //DataTable dtTesting = ConvertToDataTable(downtimeDetailArray);

                 DataRow rowPowerMonitoring = dsPowerMonitoring.Tables["dtMain"].NewRow();   // dtMain.NewRow();


                 AtmRequiredInfo atmInfo = GetAtmInfoByTitle(dtAtms, row["ATM ID"].ToString());

                 ///<Summary>
                 ///This is because to handle those Tickets whose details are not present in our CCMS UI. 
                 ///Those tickets should be in report.
                 ///</Summary>
                 if (atmInfo == null)
                 {
                     LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Error, "atm info not found:" + row["ATM ID"]);
                     continue;
                 }
                 ///<Summary>
                 ///Edited by Ali Shah on 29 Mar, 2016
                 //To facilitate 'Power Down only' events as per MCB Farhan Request
                 ///</Summary>
                 reasonDescription = InvestigateCommDownProblem(dtPowerupEvents, atmInfo.Atm_Id, atmDownTime, atmUpTime, dtTo);
                 if (CheckBox_PowerOnly.Checked && !reasonDescription.Contains("Power Down"))
                 {
                     LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Info, "Communication Down Event: " + row["ATM ID"]);
                     continue;
                 }
                 //
                 rowPowerMonitoring[(int)columnsMain.Title] = row["ATM ID"];
                 rowPowerMonitoring[(int)columnsMain.TicketId] = row["Ticket ID"];
                 rowPowerMonitoring[(int)columnsMain.IP] = atmInfo.IP;
                 rowPowerMonitoring[(int)columnsMain.Location] = row["Address 1"];
                 rowPowerMonitoring[(int)columnsMain.Region] = atmInfo.RegionName;   // row["Branch Name TH"];
                 rowPowerMonitoring[(int)columnsMain.StatusDescription] = "";

                 rowPowerMonitoring[(int)columnsMain.FromDate] = atmDownTime.ToShortDateString();
                 rowPowerMonitoring[(int)columnsMain.FromTime] = atmDownTime.ToShortTimeString();
                 if (row["end_date"] != DBNull.Value)
                 {
                     rowPowerMonitoring[(int)columnsMain.ToDate] = atmUpTime.ToShortDateString();
                     rowPowerMonitoring[(int)columnsMain.ToTime] = atmUpTime.ToShortTimeString();
                 }
                 rowPowerMonitoring[(int)columnsMain.TotalDowntime] = GetDowntime(atmDownTime, atmUpTime, dtFrom, dtTo);
                 //rowPowerMonitoring[(int)columnsMain.Reason] = row["Ticket Problem Bucket TH"];

                 ///<Summary>
                 ///Edited by Ali Shah on 29 Mar, 2016
                 //To facilitate 'Power Down only' events as per MCB Farhan Request
                 ///</Summary>
                 rowPowerMonitoring[(int)columnsMain.Reason] = reasonDescription;
                 //
                 rowPowerMonitoring[(int)columnsMain.Organization] = GetOrganization(atmInfo.Region_id);

                 dsPowerMonitoring.Tables["dtMain"].Rows.Add(rowPowerMonitoring);
                 LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Error, "rpw added:" + row["ATM ID"]);

                 ///<Summary>
                 ///Edited by Ali Shah on 29 Mar, 2016
                 //To facilitate 'Power Down only' events as per MCB Farhan Request
                 ///</Summary>
                 if (!CheckBox_PowerOnly.Checked)   //To ensure 'Power Down only' events should print with interface details as per MCB Farhan Request
                 {
                 //
                     foreach (DataRow rowDetail in downtimeDetailArray)  //Seb Report...Use to tell detail about Atm Down
                     {
                         DataRow rowPowerMonitoringDetail = dsPowerMonitoring.Tables["dtSubReport"].NewRow();

                         rowPowerMonitoringDetail["Title"] = rowDetail["Title"];
                         rowPowerMonitoringDetail["TicketId"] = row["Ticket ID"];
                         rowPowerMonitoringDetail["InterfaceName"] = rowDetail["interface_friendly_name"];
                         rowPowerMonitoringDetail["IP"] = rowDetail["ip"];

                         LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Error, "from date:" + rowDetail["from_date"] + " , to date:" + rowDetail["to_date"]);

                         rowPowerMonitoringDetail["InterfaceFromDate"] = DateTime.Parse(rowDetail["from_date"].ToString()).ToShortDateString();
                         rowPowerMonitoringDetail["InterfaceFromTime"] = DateTime.Parse(rowDetail["from_date"].ToString()).TimeOfDay.ToString();
                         string toDate = rowDetail["to_date"] != DBNull.Value ? rowDetail["to_date"].ToString() : "";
                         if (toDate.Length == 0)
                         {
                             rowPowerMonitoringDetail["InterfaceToDate"] = DateTime.Now.ToShortDateString();
                             rowPowerMonitoringDetail["InterfaceToTime"] = DateTime.Now.TimeOfDay.ToString();
                             rowPowerMonitoringDetail["InterfaceDowntime"] = GetDowntime(Convert.ToDateTime(rowDetail["from_date"]), dtTo, dtFrom, dtTo);

                         }
                         else
                         {
                             rowPowerMonitoringDetail["InterfaceToDate"] = DateTime.Parse(rowDetail["to_date"].ToString()).ToShortDateString();
                             rowPowerMonitoringDetail["InterfaceToTime"] = DateTime.Parse(rowDetail["to_date"].ToString()).TimeOfDay.ToString();
                             rowPowerMonitoringDetail["InterfaceDowntime"] = GetDowntime(Convert.ToDateTime(rowDetail["from_date"]), Convert.ToDateTime(rowDetail["to_date"]), dtFrom, dtTo);
                         }
                         rowPowerMonitoringDetail["InterfaceLastStatus"] = int.Parse(rowDetail["interface_status"].ToString()) == 1 ? "Up" : "Down";
                         dsPowerMonitoring.Tables["dtSubReport"].Rows.Add(rowPowerMonitoringDetail);
                         LogableTask.LogMonoActivityTask("Extracting Information", MethodBase.GetCurrentMethod(), TraceLevel.Error, "sub report detail added:" + row["ATM ID"]);

                     }
                 }//for
             }//if checkbox_powerdown.checked
         }//ExtractingInformationFromDB

         //Checking Link down and up time w.r.t. Report Start and End Time.
         private static double GetDowntime(DateTime downTime, DateTime upTime, DateTime fromTime, DateTime toTime)
         {
             DateTime startTime;
             DateTime endTime;
             startTime = fromTime > downTime ? fromTime : downTime;
             endTime = (upTime == null || toTime < upTime) ? toTime : upTime;
             if (startTime > endTime)
             {
                 LogableTask.LogMonoActivityTask("Error while calculating Downtime", MethodBase.GetCurrentMethod(), TraceLevel.Warning, "Start Time: " + startTime + " and End Time: " + endTime);
                 return 0;
             }
             //Changed to zero from one, as dont want to get time in decimal numbers.
             return (Math.Round(endTime.Subtract(startTime).TotalMinutes, 0));
             //return (upTime.Subtract(downTime).TotalMinutes);
         }

         public static string GetOrganization(int region_id)
         {
             Region region = Region.LoadRegionByPk(region_id);
             if (region.IsOrganization)
             {
                 return region.RegionName;
             }
             else
                 return GetOrganization(region.ParentRegionId.Value);
         }

        private static AtmRequiredInfo GetAtmInfoByTitle(DataTable dtAtms, string atmTitle)
        {
            AtmRequiredInfo atmInfo = null;
            DataRow[] rowArray = dtAtms.Select(" title = '" + atmTitle + "'");
            if (rowArray.Length > 0)
            {
                atmInfo = new AtmRequiredInfo();
                atmInfo.Title = rowArray[0][0].ToString();
                atmInfo.IP = rowArray[0][1].ToString();
                atmInfo.Atm_Id = int.Parse(rowArray[0][2].ToString());
                atmInfo.Region_id = int.Parse(rowArray[0][3].ToString());
                atmInfo.RegionName = rowArray[0][4].ToString();
            }
            return atmInfo;
        }

        private static string InvestigateCommDownProblem(DataTable dtPowerupEvents, int Atm_Id, DateTime atmDownTime, DateTime atmUpTime, DateTime dtTo)
        {
            string reason = "Communication Failure";
            //Change done by Ali Shah on 30th Mar, 2016
            //ATM down fr more than 24 hrs must be due to Power down as Link down never be for more than 24 hrs as per MCB Farhan statement
            if (atmUpTime.Subtract(atmDownTime).TotalMinutes > 1440)
            {
                reason = "Power Down";
                return reason;
            }
            //Change done by Ali Shah
            //PowerupOffsetTimeInMin Key may forgot to provide in config file
            string strPowerupOffset = ConfigurationManager.AppSettings["PowerupOffsetTimeInMin"];
            if (string.IsNullOrEmpty(strPowerupOffset))
            {
                LogableTask.LogMonoActivityTask("Extracting PowerupOffsetTimeInMin Key", MethodBase.GetCurrentMethod(), TraceLevel.Error, "PowerupOffsetTimeInMin key is not available in Config file. Initialized with 5");
                strPowerupOffset = "5";
            }
            double powerupOffsetTimeInMin = double.Parse(strPowerupOffset);
            DataRow[] drArrayPowerupEvents = dtPowerupEvents.Select("atm_id = " + Atm_Id);
            foreach (DataRow row in drArrayPowerupEvents)
            {
                DateTime dtPowerupTime = Convert.ToDateTime(row["powerup_time"]);
                if (dtPowerupTime > atmDownTime && dtPowerupTime < atmUpTime.AddMinutes(-powerupOffsetTimeInMin))
                {
                    reason = "Power Down till " + dtPowerupTime.ToString() + " then Link Down";
                }
                //else if (dtPowerupTime >= atmUpTime.AddMinutes(-powerupOffsetTimeInMin) && dtPowerupTime <= atmUpTime.AddMinutes(powerupOffsetTimeInMin))
                else if ((dtPowerupTime >= atmUpTime.AddMinutes(-powerupOffsetTimeInMin) && dtPowerupTime <= atmUpTime.AddMinutes(powerupOffsetTimeInMin)) || atmUpTime.Subtract(atmDownTime).TotalMinutes > 1440)
                {
                    reason = "Power Down";
                }
                else //if ((dtPowerupTime < atmDownTime || dtPowerupTime > atmUpTime) && dtPowerupTime < dtTo)
                {
                    reason = "Link Down";
                }
            }
            return reason;
        }

        protected void CheckBox_PowerOnly_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    class AtmRequiredInfo
    {
        int atm_Id;
        string title;
        int region_id;
        string ip;
        string region_name;

        public int Atm_Id
        {
            get
            {
                return atm_Id;
            }
            set
            {
                atm_Id = value;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        public int Region_id
        {
            get
            {
                return region_id;
            }
            set
            {
                region_id = value;
            }
        }

        public string IP
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
            }
        }

        public string RegionName
        {
            get
            {
                return region_name;
            }
            set
            {
                region_name = value;
            }
        }
    }
}