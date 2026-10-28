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
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using Avanza.iSuite.UI;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class FinalizeSuspectedReplenishments : BasePage
    {
        protected void LoadCounterFileOnDemand_Click(object sender, EventArgs e)
        {
            object alertID = Request.QueryString["aid"];
            AtmAlert atmAlert = AtmAlert.LoadAtmAlertByPk(int.Parse(alertID.ToString()));
            if (atmAlert != null)
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 600;
                cmd.CommandText = "select * from ccms_atm_log where task_id = " + atmAlert.TaskId;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                GridView_CounterFile.DataSource = dt;
                GridView_CounterFile.DataBind();
            }
        }
        protected void Button_Approve_Click(object sender, EventArgs e)
        {
            this.ProcessReplenishment();
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("~/ccms/UserTask.aspx");
        }

        protected void Button_Decline_Click(object sender, EventArgs e)
        {
            Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(base.Request.QueryString["tid"]));
            userTask.Status = ApprovalStatus.Rejected.ToString();
            userTask.Save();
            base.Response.Redirect("~/ccms/UserTask.aspx");
        }

        public static void GenerateIntegratedAlert(int alertTypeID, string msg, int entityID, EntityType entityType, Event_Type eventType, int? ftpFileInfoId, SqlTransaction trxn, int? atmAlertID)
        {
            LogableTask task = LogableTask.NewTask("GenerateIntegratedAlert");
            int orgID = -1;
            try
            {
                CcmsIntegratedAlert alert = new CcmsIntegratedAlert();
                if (atmAlertID != null)
                    alert.AtmAlertId = atmAlertID;
                alert.AlertTypeId = alertTypeID;
                alert.AlertType = AlertType.LoadAlertTypeByPk(alertTypeID).AlertTypeName;
                alert.EntityId = entityID;
                alert.EntityType = entityType.ToString();
                alert.AlertLevel = eventType.ToString();
                alert.AlertStatus = "Unread";
                alert.GeneratedAt = new DateTime?(DateTime.Now);
                alert.AlertText = msg;
                alert.ExpirationTime = new DateTime?(DateTime.Now.AddDays(2.0));
                alert.GenerateNotificationSent = false;
                alert.ResolveNotificationSent = false;
                if (alert.EntityType == EntityType.ATM.ToString())
                {
                    Atm atm = Atm.LoadAtmByPk(entityID);
                    alert.GenerateRetryRemaining = new int?(atm.RetryCountAlert);
                    alert.ResolveRetryRemaining = new int?(atm.RetryCountAlert);
                    orgID = GetOrganization(atm.RegionId);
                }
                else if (alert.EntityType == EntityType.Organization.ToString())
                {
                    alert.FtpFileInfoId = ftpFileInfoId;
                    Region region = Region.LoadRegionByPk(FtpFileInfo.LoadFtpFileInfoByPk(ftpFileInfoId.Value).RegionId);
                    alert.GenerateRetryRemaining = new int?(region.RetryCountAlert);
                    alert.ResolveRetryRemaining = new int?(region.RetryCountAlert);
                    orgID = FtpFileInfo.LoadFtpFileInfoByPk(ftpFileInfoId.Value).RegionId;
                }
                alert.ModuleType = "CURRENCY";
                alert.OrganizationId = orgID;
                if (trxn != null)
                {
                    alert.Save(trxn.Connection, trxn);
                }
                else
                {
                    alert.Save();
                }
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Alert added of type " + alertTypeID);
            }
            finally
            {
                task.EndTask();
            }
        }

        public static void GenerateTerminalAlert(int atm_id, int alertTypeID, string msg, SqlTransaction trxn, Event_Type eventType, int taskID, int? entityID, string entityType)
        {

            LogableTask task = LogableTask.NewTask("GenerateTerminalAlert");
            try
            {
                AtmAlert alert = new AtmAlert
                {
                    AtmId = atm_id,
                    GeneratedAt = DateTime.Now,
                    AlertTypeId = alertTypeID,
                    GenerateNotificationSent = false,
                    ResolveNotificationSent = null,
                    GenerateAtRetryRemaining = 10,
                    ResolveAtRetryRemaining = 0
                };
                alert.TaskId = taskID;
                alert.AlertMsg = msg;
                if (entityID.HasValue)
                {
                    alert.EntityId = entityID;
                }
                if (entityType != null)
                {
                    alert.EntityType = entityType;
                }
                if (alert.AlertTypeId == 11 || alert.AlertTypeId == 10 || alert.AlertTypeId == 8 ||
                    alert.AlertTypeId == 12 || alert.AlertTypeId == 7 || alert.AlertTypeId == 9 || alert.AlertTypeId == 13)
                {
                    alert.ExpirationTime = DateTime.Now.AddDays(2);
                }
                alert.Save(trxn.Connection, trxn);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Alert added for terminal " + atm_id);
                GenerateIntegratedAlert(alertTypeID, msg, alert.AtmId.Value, EntityType.ATM, eventType, null, trxn, alert.AtmAlertId);

            }
            finally
            {
                task.EndTask();
            }

        }

        public static int GetOrganization(int region_id)
        {
            Region region = Region.LoadRegionByPk(region_id);
            if (region.IsOrganization)
            {
                return region.RegionId;
            }
            return GetOrganization(region.ParentRegionId.Value);
        }

        private static int MakeNumberDivisibleBy10(int number)
        {
            while ((number % 10) != 0)
            {
                number++;
            }
            return number;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!base.IsPostBack)
            {
                object usertTaskID = Request.QueryString["tid"];
                object alertID = Request.QueryString["aid"];
                if (usertTaskID != null)
                {
                    Session["userTaskID"] = usertTaskID;
                    Session["alertID"] = alertID;

                }
                else
                {
                    usertTaskID = Session["userTaskID"];
                    alertID = Session["alertID"];
                }

                if (!base.CheckRight(0xbf2))
                    Response.Redirect("../admin/AccessDenied.aspx");

                if (Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(usertTaskID.ToString())).Status != ((ApprovalStatus)0).ToString())
                {
                    this.Button_Approve.Enabled = false;
                    this.Button_Decline.Enabled = false;
                }
                AtmAlert atmAlert = AtmAlert.LoadAtmAlertByPk(int.Parse(alertID.ToString()));
                if (atmAlert != null)
                {

                    string[] parts = atmAlert.AlertMsg.Split(new char[] { '|' });
                    this.Label_Title.Text = Atm.LoadAtmByPk(atmAlert.AtmId.Value).Title;
                    this.Label_date.Text = DateTime.ParseExact(parts[0], "MM/dd/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy HH:mm:ss");
                    TextBox_RepDate.Text = Label_date.Text;
                    BindGrid(GridView_PostWithdrawals, 1, atmAlert.AtmId.Value);
                    BindGrid(GridView_PreWithdrawals, 2, atmAlert.AtmId.Value);
                    BindReplenishmentGrid(atmAlert.AtmId.Value);

                    if (atmAlert.AlertTypeId == (int)EnumAlertType.AddCashReplenishmentDetected)
                    {

                        Label_Counters.Text = "Add Cash Counters";
                        currentCounterSnapshot.Visible = false;
                        tblServiceStateAndCurentModeInfo.Visible = false;
                        //05/17/2012 09:35:36|Replenishment|ReplenishmentWithoutTestCash|20120517093536|20120517093536|-1|2000|4000|0|2000|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|575|1285|0|677|0|0|0|Swap
                        this.Label_Remaining1.Text = parts[6];
                        this.Label_Remaining2.Text = parts[7];
                        this.Label_Remaining3.Text = parts[8];
                        this.Label_Remaining4.Text = parts[9];

                        this.Label_Dispensed1.Text = parts[13];
                        this.Label_Dispensed2.Text = parts[14];
                        this.Label_Dispensed3.Text = parts[15];
                        this.Label_Dispensed4.Text = parts[16];

                        this.Label_Purged1.Text = parts[20];
                        this.Label_Purged2.Text = parts[21];
                        this.Label_Purged3.Text = parts[22];
                        this.Label_Purged4.Text = parts[23];

                        this.Label_LastTotalType1.Text = parts[27];
                        this.Label_LastTotalType2.Text = parts[28];
                        this.Label_LastTotalType3.Text = parts[29];
                        this.Label_LastTotalType4.Text = parts[30];
                    }
                    else
                    {

                        this.Label_ServiceState.Text = parts[2];
                        this.Label_CurrentMode.Text = parts[3];
                        this.Label_LastTotalType1.Text = parts[4];
                        this.Label_LastTotalType2.Text = parts[5];
                        this.Label_LastTotalType3.Text = parts[6];
                        this.Label_LastTotalType4.Text = parts[7];
                        this.Label_Dispensed1.Text = parts[0x12];
                        this.Label_Dispensed2.Text = parts[0x13];
                        this.Label_Dispensed3.Text = parts[20];
                        this.Label_Dispensed4.Text = parts[0x15];
                        this.Label_Remaining1.Text = parts[11];
                        this.Label_Remaining2.Text = parts[12];
                        this.Label_Remaining3.Text = parts[13];
                        this.Label_Remaining4.Text = parts[14];
                        this.Label_Purged1.Text = parts[0x19];
                        this.Label_Purged2.Text = parts[0x1a];
                        this.Label_Purged3.Text = parts[0x1b];
                        this.Label_Purged4.Text = parts[0x1c];
                        this.Label_CurrentTotalType1.Text = parts[0x20];
                        this.Label_CurrentTotalType2.Text = parts[0x21];
                        this.Label_CurrentTotalType3.Text = parts[0x22];
                        this.Label_CurrentTotalType4.Text = parts[0x23];
                        this.Label_currentRemaining1.Text = parts[0x27];
                        this.Label_currentRemaining2.Text = parts[40];
                        this.Label_currentRemaining3.Text = parts[0x29];
                        this.Label_currentRemaining4.Text = parts[0x2a];
                        this.Label_currentDispensed1.Text = parts[0x2e];
                        this.Label_currentDispensed2.Text = parts[0x2f];
                        this.Label_currentDispensed3.Text = parts[0x30];
                        this.Label_currentDispensed4.Text = parts[0x31];
                        this.Label_currentPurged1.Text = parts[0x35];
                        this.Label_currentPurged2.Text = parts[0x36];
                        this.Label_currentPurged3.Text = parts[0x37];
                        this.Label_currentPurged4.Text = parts[0x38];


                        if (int.Parse(Label_currentDispensed1.Text) == 0 && int.Parse(Label_currentDispensed2.Text) == 0
                            && int.Parse(Label_currentDispensed3.Text) == 0 && int.Parse(Label_currentDispensed4.Text) == 0
                            && int.Parse(Label_currentPurged1.Text) == 1 && int.Parse(Label_currentPurged2.Text) == 1
                            && int.Parse(Label_currentPurged3.Text) == 1 && int.Parse(Label_currentPurged4.Text) == 1)
                        {
                            TextBox_Total1.Text = (int.Parse(Label_currentRemaining1.Text) + int.Parse(Label_currentDispensed1.Text) + int.Parse(Label_currentPurged1.Text)).ToString();
                            TextBox_Total2.Text = (int.Parse(Label_currentRemaining2.Text) + int.Parse(Label_currentDispensed2.Text) + int.Parse(Label_currentPurged2.Text)).ToString();
                            TextBox_Total3.Text = (int.Parse(Label_currentRemaining3.Text) + int.Parse(Label_currentDispensed3.Text) + int.Parse(Label_currentPurged3.Text)).ToString();
                            TextBox_Total4.Text = (int.Parse(Label_currentRemaining4.Text) + int.Parse(Label_currentDispensed4.Text) + int.Parse(Label_currentPurged4.Text)).ToString();

                            TextBox_Rem1.Text = Label_currentRemaining1.Text;
                            TextBox_Rem2.Text = Label_currentRemaining2.Text;
                            TextBox_Rem3.Text = Label_currentRemaining3.Text;
                            TextBox_Rem4.Text = Label_currentRemaining4.Text;

                            TextBox_Dis1.Text = Label_currentDispensed1.Text;
                            TextBox_Dis2.Text = Label_currentDispensed2.Text;
                            TextBox_Dis3.Text = Label_currentDispensed3.Text;
                            TextBox_Dis4.Text = Label_currentDispensed4.Text;

                            TextBox_Purg1.Text = Label_currentPurged1.Text;
                            TextBox_Purg2.Text = Label_currentPurged2.Text;
                            TextBox_Purg3.Text = Label_currentPurged3.Text;
                            TextBox_Purg4.Text = Label_currentPurged4.Text;

                            Label_Notes.Text = "Note : CCMS recommends to overwrite counters with user counters.If you agree with it then please select checkbox \"Forcefully overwrite counters\" ";




                        }


                    }
                    
                }
            }
        }

        private void ProcessReplenishment()
        {
            bool isTrxnCommited = false;
            LogableTask task = LogableTask.NewTask("ProcessReplenishments");
            SqlTransaction trx = null;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {   //Validate replenishment data 
                if (CheckBox_Override.Checked)
                {
                    if (TextBox_Total1.Text=="0" && TextBox_Total2.Text=="0" && TextBox_Total3.Text=="0"
                        && TextBox_Total4.Text=="0")
                        throw new Exception ("Replenishment amount should be greater than zero");
                }
                else
                {
                    if (Label_CurrentTotalType1.Text == "0" && Label_CurrentTotalType2.Text == "0" && Label_CurrentTotalType3.Text == "0"
                        && Label_CurrentTotalType4.Text == "0")
                        throw new Exception ("Replenishment amount should be greater than zero");
                    
                }


                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                AtmAlert atmAlert = AtmAlert.LoadAtmAlertByPk(int.Parse(base.Request.QueryString["aid"]));
                Atm atm = Atm.LoadAtmByPk(atmAlert.AtmId.Value);
                NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                Replenishment replenishment;
                AtmAlert atmAlert1;


                if (atmAlert.AlertTypeId == (int)EnumAlertType.AddCashReplenishmentDetected)
                {
                    if (CheckBox_Override.Checked)
                    {
                        replenishment = new Replenishment
                        {
                            AtmId = atm.ATMId,
                            RepDatetime = DateTime.ParseExact(TextBox_RepDate.Text, "dd/MM/yyyy HH:mm:ss", null),
                            CashAdded1 = MakeNumberDivisibleBy10(int.Parse(this.TextBox_Total1.Text)),
                            CashAdded2 = MakeNumberDivisibleBy10(int.Parse(this.TextBox_Total2.Text)),
                            CashAdded3 = MakeNumberDivisibleBy10(int.Parse(this.TextBox_Total3.Text)),
                            CashAdded4 = MakeNumberDivisibleBy10(int.Parse(this.TextBox_Total4.Text)),
                            CashAdded5 = 0,
                            CashAdded6 = 0,
                            CashAdded7 = 0,
                            RepStatus = "OrderMissingForceFully",
                            IsSwap = (this.RadioButtonList_RepType.SelectedIndex == 0) ? false : true,
                            TaskId = atmAlert.TaskId.Value,
                            CashOrderId = -1,
                        };
                    }
                    else
                    {
                        replenishment = new Replenishment
                        {
                            AtmId = atm.ATMId,
                            RepDatetime = DateTime.ParseExact(this.Label_date.Text, "dd/MM/yyyy HH:mm:ss", null),
                            CashAdded1 = MakeNumberDivisibleBy10(int.Parse(this.Label_LastTotalType1.Text)),
                            CashAdded2 = MakeNumberDivisibleBy10(int.Parse(this.Label_LastTotalType2.Text)),
                            CashAdded3 = MakeNumberDivisibleBy10(int.Parse(this.Label_LastTotalType3.Text)),
                            CashAdded4 = MakeNumberDivisibleBy10(int.Parse(this.Label_LastTotalType4.Text)),
                            CashAdded5 = 0,
                            CashAdded6 = 0,
                            CashAdded7 = 0,
                            RepStatus = "OrderMissing",
                            IsSwap = (this.RadioButtonList_RepType.SelectedIndex == 0) ? false : true,
                            TaskId = atmAlert.TaskId.Value,
                            CashOrderId = -1,
                        };
                    }
                }

                else
                {

                    if (CheckBox_Override.Checked)
                    {
                        replenishment = new Replenishment
                        {
                            AtmId = atm.ATMId,
                            RepDatetime = DateTime.ParseExact(TextBox_RepDate.Text, "dd/MM/yyyy HH:mm:ss", null),
                            CashAdded1 = MakeNumberDivisibleBy10(int.Parse(this.TextBox_Total1.Text)),
                            CashAdded2 = MakeNumberDivisibleBy10(int.Parse(this.TextBox_Total2.Text)),
                            CashAdded3 = MakeNumberDivisibleBy10(int.Parse(this.TextBox_Total3.Text)),
                            CashAdded4 = MakeNumberDivisibleBy10(int.Parse(this.TextBox_Total4.Text)),
                            CashAdded5 = 0,
                            CashAdded6 = 0,
                            CashAdded7 = 0,
                            RepStatus = "OrderMissingForceFully",
                            IsSwap = (this.RadioButtonList_RepType.SelectedIndex == 0) ? false : true,
                            TaskId = atmAlert.TaskId.Value,
                            CashOrderId = -1,
                            GeneratedAt = DateTime.Now,
                            IsUpdated = false

                        };
                        replenishment.Save(trx.Connection, trx);

                        TestCashPurgedNotes testCashPurgedNotes = new TestCashPurgedNotes();
                        testCashPurgedNotes.ReplenishmentId = replenishment.ReplenishmentId;
                        testCashPurgedNotes.TaskId = atmAlert.TaskId.Value;
                        testCashPurgedNotes.TestCashDatetime = replenishment.RepDatetime;
                        testCashPurgedNotes.CashPurged1 = int.Parse(TextBox_Purg1.Text);
                        testCashPurgedNotes.CashPurged2 = int.Parse(TextBox_Purg2.Text);
                        testCashPurgedNotes.CashPurged3 = int.Parse(TextBox_Purg3.Text);
                        testCashPurgedNotes.CashPurged4 = int.Parse(TextBox_Purg4.Text);
                        testCashPurgedNotes.CashPurged5 = 0;
                        testCashPurgedNotes.CashPurged6 = 0;
                        testCashPurgedNotes.CashPurged7 = 0;
                        testCashPurgedNotes.AtmId = replenishment.AtmId;
                        testCashPurgedNotes.Save(trx.Connection, trx);
                    }
                    else
                    {

                        replenishment = new Replenishment
                        {
                            AtmId = atm.ATMId,
                            RepDatetime = DateTime.ParseExact(this.Label_date.Text, "dd/MM/yyyy HH:mm:ss", null),
                            CashAdded1 = MakeNumberDivisibleBy10(int.Parse(this.Label_CurrentTotalType1.Text)),
                            CashAdded2 = MakeNumberDivisibleBy10(int.Parse(this.Label_CurrentTotalType2.Text)),
                            CashAdded3 = MakeNumberDivisibleBy10(int.Parse(this.Label_CurrentTotalType3.Text)),
                            CashAdded4 = MakeNumberDivisibleBy10(int.Parse(this.Label_CurrentTotalType4.Text)),
                            CashAdded5 = 0,
                            CashAdded6 = 0,
                            CashAdded7 = 0,
                            RepStatus = "OrderMissing",
                            IsSwap = (this.RadioButtonList_RepType.SelectedIndex == 0) ? false : true,
                            TaskId = atmAlert.TaskId.Value,
                            CashOrderId = -1,
                            GeneratedAt = DateTime.Now,
                            IsUpdated = false
                        };
                        replenishment.Save(trx.Connection, trx);
                        ///Changes done on 13/Mar 2014 to add notes in purged bin
                        TestCashPurgedNotes testCashPurgedNotes = new TestCashPurgedNotes();
                        testCashPurgedNotes.ReplenishmentId = replenishment.ReplenishmentId;
                        testCashPurgedNotes.TaskId = atmAlert.TaskId.Value;
                        testCashPurgedNotes.TestCashDatetime = replenishment.RepDatetime;
                        testCashPurgedNotes.CashPurged1 = int.Parse(Label_currentPurged1.Text);
                        testCashPurgedNotes.CashPurged2 = int.Parse(Label_currentPurged2.Text);
                        testCashPurgedNotes.CashPurged3 = int.Parse(Label_currentPurged3.Text);
                        testCashPurgedNotes.CashPurged4 = int.Parse(Label_currentPurged4.Text);
                        testCashPurgedNotes.CashPurged5 = 0;
                        testCashPurgedNotes.CashPurged6 = 0;
                        testCashPurgedNotes.CashPurged7 = 0;
                        testCashPurgedNotes.AtmId = replenishment.AtmId;
                        testCashPurgedNotes.Save(trx.Connection, trx);

                    }
                }
                


                GenerateTerminalAlert(atm.ATMId, (int)EnumAlertType.ReplenishmentAtATM, "Repenishment At ATM", trx, Event_Type.Information, atmAlert.TaskId.Value, replenishment.ReplenishmentId, "Replenishment");
                if (this.CheckBox_PurgeBinThreshold.Checked && replenishment.IsSwap)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Going to fetch purge bin alert for atm_id = " + atm.ATMId);
                    atmAlert1 = AtmAlert.LoadAtmAlert(string.Concat(new object[] { "alert_type_id=", 0x16, " and atm_id=", atm.ATMId, " and resolve_at is null" }));
                    if (atmAlert1 != null)
                    {
                        atmAlert1.ResolveAt = new DateTime?(DateTime.Now);
                        atmAlert1.Save(trx.Connection, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Purge bin alert resolved for atm_id = " + atm.ATMId);
                        CcmsIntegratedAlert ccmsIntAlert = CcmsIntegratedAlert.LoadCcmsIntegratedAlert("atm_alert_id=" + atmAlert1.AtmAlertId);
                        if (ccmsIntAlert != null)
                        {

                            cmd = conn.CreateCommand();
                            cmd.Transaction = trx;
                            cmd.CommandText = "update Ccms_integrated_alert set resolved_at= convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103) where id=" + ccmsIntAlert.Id;
                            cmd.ExecuteNonQuery();

                            //ccmsIntAlert.ResolvedAt = DateTime.Now;
                            //ccmsIntAlert.Save(trxn.Connection, trxn);
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Purge bin alert from ccms integrated alert resolved for atm_id = " + atm.ATMId);
                        }
                    }
                }
                decimal replenishedAmount = (decimal)(replenishment.CashAdded1 * noteSetType.DenominationType1 +
                    replenishment.CashAdded2 * noteSetType.DenominationType2 +
                    replenishment.CashAdded3 * noteSetType.DenominationType3 +
                    replenishment.CashAdded4 * noteSetType.DenominationType4 +
                    replenishment.CashAdded5 * noteSetType.DenominationType5 +
                    replenishment.CashAdded6 * noteSetType.DenominationType6 +
                    replenishment.CashAdded7 * noteSetType.DenominationType7);

                if ((replenishedAmount > atm.MinOperatingBalance) && this.CheckBox_LowBalanceReport.Checked)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Going to fetch low balance alert for atm_id = " + atm.ATMId);
                    atmAlert1 = AtmAlert.LoadAtmAlert(string.Concat(new object[] { "alert_type_id=", 20, " and atm_id=", atm.ATMId, " and resolve_at is null" }));
                    if (atmAlert1 != null)
                    {
                        atmAlert1.ResolveAt = DateTime.Now;
                        atmAlert1.Save(trx.Connection, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Low balance alert resolved for atm_id = " + atm.ATMId);
                        CcmsIntegratedAlert ccmsIntAlert = CcmsIntegratedAlert.LoadCcmsIntegratedAlert("atm_alert_id=" + atmAlert1.AtmAlertId);
                        if (ccmsIntAlert != null)
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Transaction Object is null=" + (trx.Connection == null ? "true" : "false"));


                            //ConnectionFactory.ExecuteQuery("update Ccms_integrated_alert set resolved_at= convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103) where id=" + ccmsIntAlert.Id, trxn);
                            cmd = conn.CreateCommand();
                            cmd.Transaction = trx;
                            cmd.CommandText = "update Ccms_integrated_alert set resolved_at= convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103) where id=" + ccmsIntAlert.Id;
                            cmd.ExecuteNonQuery();


                            //ccmsIntAlert.ResolvedAt = DateTime.Now;
                            //ccmsIntAlert.Save(conn, trxn);
                            // ccmsIntAlert.Save(trxn.Connection, trxn);
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Low balance alert from ccms integrated alert resolved for atm_id = " + atm.ATMId);
                        }
                    }
                }

                if ((replenishedAmount > atm.MinOperatingBalance) && this.CheckBox_ResolveOutOfCashAlert.Checked)
                {
                    atmAlert1 = AtmAlert.LoadAtmAlert(string.Concat(new object[] { "alert_type_id=", 0x15, " and atm_id=", atm.ATMId, " and resolve_at is null" }));
                    if (atmAlert1 != null)
                    {
                        atmAlert1.ResolveAt = DateTime.Now;
                        atmAlert1.Save(trx.Connection, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Out Of Cash alert resolved for atm_id = " + atm.ATMId);
                        CcmsIntegratedAlert ccmsIntAlert = CcmsIntegratedAlert.LoadCcmsIntegratedAlert("atm_alert_id=" + atmAlert1.AtmAlertId);
                        if (ccmsIntAlert != null)
                        {
                            //ccmsIntAlert.ResolvedAt = DateTime.Now;
                            //ConnectionFactory.ExecuteQuery("update Ccms_integrated_alert set resolved_at= convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103) where id=" + ccmsIntAlert.Id,trxn);
                            cmd = conn.CreateCommand();
                            cmd.Transaction = trx;
                            cmd.CommandText = "update Ccms_integrated_alert set resolved_at= convert(datetime,'" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "',103) where id=" + ccmsIntAlert.Id;
                            cmd.ExecuteNonQuery();
                            //ccmsIntAlert.Save(conn,trxn);
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Out Of Cash alert from ccms integrated alert resolved for atm_id = " + atm.ATMId);
                        }
                    }
                }
                Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(base.Request.QueryString["tid"]));
                userTask.Status = ApprovalStatus.Approved.ToString();
                userTask.ResolutionTime = DateTime.Now;
                userTask.ResolvedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                userTask.Save(trx.Connection, trx);
                
                trx.Commit();
                isTrxnCommited = true;
                base.Response.Redirect("~/ccms/UserTask.aspx");


            }
            catch (Exception ex)
            {
                if (!isTrxnCommited && trx!=null)
                    trx.Rollback();
                this.Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }


        string GetFilter(int index, DateTime doubtfulRepDate)
        {
            string filter = "";

            if (index == 1)
            {
                filter += " and trxn_datetime>convert(datetime,'" + doubtfulRepDate.ToString("dd/MM/yyyy HH:mm:ss") + "',103)" +
                " and trxn_datetime<convert(datetime,'" + doubtfulRepDate.AddDays(60).ToString("dd/MM/yyyy HH:mm:ss") + "',103)";
                //till 10 days to smartly search records.



            }
            else if (index == 2)
            {
                filter += " and trxn_datetime>convert(datetime,'" + doubtfulRepDate.AddDays(-60).ToString("dd/MM/yyyy HH:mm:ss") + "',103)" +
                    " and trxn_datetime<convert(datetime,'" + doubtfulRepDate.ToString("dd/MM/yyyy HH:mm:ss") + "',103)";
            }


            return filter;
        }

        void BindReplenishmentGrid(int atmID)
        {
            LogableTask task = LogableTask.NewTask("BindReplenishmentGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandText = @"SELECT outerATM.TITLE, rep_datetime, cash_added1,cash_added2,cash_added3,cash_added4 " +
                    " from replenishment inner join atm outerATM on replenishment.atm_id = outerATM.atm_id and outerATM.IS_ACTIVE =1 and outerATM.atm_id=" + atmID +
                    " and rep_datetime >=convert(datetime,'" + DateTime.ParseExact(Label_date.Text, "dd/MM/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy") + "',103) order by rep_datetime desc";


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                GridView_rep.DataSource = dtTrxn;
                GridView_rep.DataBind();

            }



            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }
        void BindGrid(GridView obj, int index, int atmID)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandText = @"SELECT top 5 outerATM.TITLE, trxn_datetime, outerATM.atm_id,cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4
                    from parsed_transaction,atm outerATM where parsed_transaction.atm_id = outerATM.atm_id and "
                + " outerATM.atm_id =" + atmID + "  and outerATM.IS_ACTIVE =1 " + GetFilter(index, DateTime.ParseExact(Label_date.Text, "dd/MM/yyyy HH:mm:ss", null))
                + " order by trxn_datetime " + (index == 1 ? "asc" : "desc");

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                obj.DataSource = dtTrxn;
                obj.DataBind();

            }



            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }
    }
}