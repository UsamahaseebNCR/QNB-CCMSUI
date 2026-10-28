using Avanza.CCMS.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;


public static class Utility
{
    public static string GetFormattedPANString(string inputFromUser)
    {
        if (inputFromUser.Length >= 16 && bool.Parse(System.Configuration.ConfigurationSettings.AppSettings["MaskedPANEnabled"]))
            return inputFromUser.Substring(0, 6) + "%" + inputFromUser.Substring(inputFromUser.Length - 4, 4);
        else
            return inputFromUser;
    }

    public static string GetDateTime(string date)
    {
        return DateTime.Parse(date).ToString("MM/dd/yyyy");
    }

    public static string GetUserIP()
    {
        return HttpContext.Current.Request.UserHostAddress;

    }
    public static string GetIpAddress()
    {
        string ipAdd;
        ipAdd = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (string.IsNullOrEmpty(ipAdd))
        {
            ipAdd = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        return ipAdd;
    }

    public static string GetFormatRegex()
    {
        return @"([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]\D)|([(]\d)|([(]C)|([[]040)|([[]000p)|(/)|(q[(]I)|(q[(]1)|([(]7)|([(]>)";
    }

    public static void editCaseAuditLogInserts(dynamic newObject, dynamic oldObject, string message, int userId, int rightId, SqlTransaction transaction)
    {
        List<AuditLog> auditLogs = new List<AuditLog>();
        var objType = oldObject.GetType();
        foreach (var objProperty in objType.GetProperties())
        {
            var oOldValue = objProperty.GetValue(oldObject, null);
            var oNewValue = objProperty.GetValue(newObject, null);
            if (!object.Equals(oOldValue, oNewValue))
            {
                AuditLog auditLog = new AuditLog();
                auditLog.Message = message;
                auditLog.ActivityTime = DateTime.Now;
                auditLog.UserId = userId;
                auditLog.RightId = rightId;
                auditLog.ipAddress = GetIpAddress();
                auditLog.fieldName = objProperty.Name;

                if (oNewValue == null)
                    oNewValue = "";
                if (oOldValue == null)
                    oOldValue = "";

                auditLog.oldValue = !string.IsNullOrEmpty(Convert.ToString(oOldValue)) ? oOldValue.ToString() : "";               
                auditLog.newValue = !string.IsNullOrEmpty(Convert.ToString(oNewValue)) ? oNewValue.ToString() : "";
                auditLogs.Add(auditLog);
                //Utility.BuildAuditLog("Node Set Type " + objCurrent.NoteSetTypeName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyNoteSetType);
            }
        }
        auditLogs[0].BulkSave(auditLogs, transaction);
    }

    public static void BuildAuditLog(string message, int user_id, int right_id, string fieldname = null, string oldval = null, string newval = null)
    {
        AuditLog auditLog = new AuditLog();
        auditLog.ActivityTime = DateTime.Now;
        auditLog.Message = message;
        auditLog.UserId = user_id;
        auditLog.RightId = right_id;
        auditLog.ipAddress = GetIpAddress();
        auditLog.fieldName = fieldname;
        auditLog.oldValue = oldval;
        auditLog.newValue = newval;
        auditLog.Save();
    }

    public static void BuildAuditLog(string message, int user_id, int right_id, /*Entity entity, Action action,*/ SqlConnection conn, SqlTransaction trx)
    {
        AuditLog auditLog = new AuditLog();
        auditLog.ActivityTime = DateTime.Now;
        auditLog.Message = message;
        auditLog.UserId = user_id;
        auditLog.RightId = right_id;
        //auditLog.Action = action.ToString();
        //auditLog.Entity = entity.ToString();
        auditLog.Save(conn, trx);
    }
}
public enum ApplicationVars
{
    DBConnected,
    AppSettings,
    initError
}

public enum SessionVars
{
    user = 0,
    dtUserRights = 1,
    UserSettings,
    SelectedATMs,
    /// <summary>
    /// bool 
    /// </summary>
    isRegionSelected,
    /// <summary>
    /// contains string with region id , check <paramref name="isRegionSelected"/> before using
    /// </summary>
    SelectedRegion,
    SelectedRegions,
    notice,
    noticeForMain,
    dtReportDataSet,
    dtAppUser,
    dtADUsers,
    ADUserName,
    ADPassword,
    LoginToken,
    CSVUsers,
    dsFIT,
    dtUserOrgs,
    InactiveATMs,
    FileName,
    ADSearch
}

public enum CacheVars
{
    dtGroupsRight,
    dtUsers,
    dtATMs,
    dtMStatus,
    dtRegion,
    dtEJTrxn,
    dtDownloadedFile,
    dtAuditLog,
    dtAppUser,
    dtSwitch,
    dtBank,
    dtCashOrders,
    dtGroups
}

public enum Permissions : int
{
    Login = 0,
    CreateGroup = 1,
    ViewGroupList = 2,
    ModifyGroup = 3,
    DeleteGroup = 4,
    CreateUser = 5,
    ViewUserList = 6,
    ModifyUser = 7,
    DeleteUser = 8,
    CreateATMs = 9,
    ViewATMsList = 10,
    ModifyATM = 11,
    DeleteATM = 12,
    CreateRegion = 14,
    ViewRegionList = 15,
    ModifyRegion = 16,
    DeleteRegion = 17,
    DownloadFileFromATM = 29,
    ScheduleFileDownload = 31,
    ViewConfiguration = 32,
    ChangeConfiguration = 33,
    ViewReplenishments = 34,
    ViewAuditLog = 42,
    ChangePassword = 56,
    AlertMonitoringHistory = 59,
    TaskStatusMonitoringHistory = 60,
    ViewCashOrderMonitoringHistory = 61,
    ViewOutOfBalanceInvestigation = 62,
    CashOrderReport = 63,
    FTPFileMonitoringReport = 64,
    TaskStatusReport = 65,
    TerminalDowntimeReport = 66,
    ScheduleTrackingReport = 67,
    SuspiciousReplenishmentReport = 68,
    ViewAlertDashBoard = 69,
    CreateOrganization = 70,
    ViewOrganization = 71,
    ModifyOrganization = 72,
    DeleteOrganization = 73,
    CreateCIT = 74,
    ViewCIT = 75,
    ModifyCIT = 76,
    DeleteCIT = 77,
    ViewDailyFeedSchedule = 78,
    ViewAgentRemoteAccess = 79,
    ViewOrderDownloadAndDailyFeedStatus = 80,
    ViewOrderDownloadAndDailyFeedStatusDashBoard = 81,
    ViewTaskDashBoard = 82,
    ViewCashOrderDashBoard = 83,
    ViewTransactions = 84,
    ViewCashOrders = 85,
    ImportATMsFromIMAC = 86,
    CashUtilizationReport = 87,
    CreateDailyFeedSchedule = 88,
    DeleteDailyFeedSchedule = 89,
    ViewAlertTemplate = 90,
    ModifyAlertTemplate = 91,
    CreateCashOrders = 92,
    ModifyCashOrders = 93,
    ModifyDailyFeedGenerationScheme = 94,
    ViewSummary = 95,
    ViewConfigureDailyFeed = 96,
    ViewCashPositionsDashBoard = 97,
    CreateNoteSetType = 98,
    ViewNoteSetType = 99,
    ModifyNoteSetType = 100,
    DeleteNoteSetType = 101,
    ViewDepositDashboard = 102,
    BNACounterReport = 103,
    CPMCounterReport = 104,
    CashWithdrawalReport = 105,
    RecentActivitySummary = 106,
    ScheduledSummary = 107,
    Top20Alerts = 108,
    Top20TransactingATMs = 109,
    Top20LeastUsedATMs = 110,
    ChequeCollectionSummary = 111,
    DepositCollectionSummary = 112,
    LowBalanceSummary = 113,
    AlertMonitoringReport = 114,
    NoCashWithdrawalReport = 115,
    ViewCashPositions = 116,
    OutOfCashReport = 117,
    CreateVault = 500,
    ViewVault = 501,
    ModifyVault = 502,
    DeleteVault = 503,
    PurgedTransactionCount = 3000,
    DFFInformation = 3001,
    ViewMinThresholdDashboard = 3002,
    ViewGraphList = 3003,
    PurgeBinAlertsSummary = 3004,
    ScheduleRestart = 3005,
    ScheduleDateTimeSync = 3006,
    ViewScheduleReports = 3007,
    ReportTaskMonitoring = 3008,
    EditScheduleReports = 3009,
    ViewReportTask = 3010,
    ViewCashPositionVsDFF = 3011,
    ScheduleConfiguration = 3012,
    ScheduleHeartBeatConfiguration = 3013,
    SchedulBatchConfiguration = 3014,
    DenominationUtilizationReport = 3015,
    SwitchDispensingReport = 3016,
    CreateFileType = 3017,
    EditFileType = 3018,
    DeleteFileType = 3019,
    ViewFileType = 3020,
    CounterMismatch = 3021,
    Replenishments = 3022,
    DirectoryLookupAndFileDownloader = 3023,
    ViewEJTransactions = 3024,
    ATMSummaryReport = 3025,
    DeadATMsReport = 3026,
    ATMWithoutTransaction24Hour = 3027,
    LowBalanceReport = 3028,
    PurgeBinAlertsReport = 3030,
    CassetteDispensingReport = 3031,
    ImportFLMRecord = 3032,
    ViewCardCaptured = 3033,
    ViewEJReplenishments = 3034,
    EJBalanceInvestigation = 3035,
    ViewFITMapping = 3036,
    EditFITMapping = 3037,
    DeleteFITMapping = 3038,
    UpdateFITMapping = 3039,
    MemberBankTransactionReport = 3040,
    ImportATMSettlement = 3041,
    ViewCardCaptureReport = 3042,
    ViewCallOutList = 3043,
    ViewATMSettlement = 3044,
    ViewCallOutSummary = 3045,
    DeleteReportSchedule = 3046,
    ViewBNATransactions = 3047,
    ViewCPMTransactions = 3048,
    ViewTransactionsHourlyAnalysis = 3049,
    ViewEJBNATransactions = 3050,
    CallOutReport = 3051,
    UserTask = 3052,
    ViewCCMSAndCiTSettlement = 3053,
    ApproveCCMSAndCiTSettlement = 3054,
    RejectCCMSAndCiTSettlement = 3055,
    CassetteFaultySummaryReport = 3056,
    ReplenishmentReturnedReport = 3057,
    FinalizeSuspectedReplenishments = 3058,
    AddActiveDirectoryUsers = 3059,
    ATMTransactionAnalysis = 3060,
    ViewATMTransactionForecast = 3061,
    ViewForcedReplenishments = 3062,
    ViewFaultyReplenishments = 3063,
    ViewSuggestedReplenishments = 3064,
    ViewATMsWithNoTransactionOnCurrentDay = 3065,
    ViewATMsHavingThresholdReached = 3066,
    ViewATMsHavingLastTransaction = 3067,
    ViewATMsHavingNoLastTransaction = 3068,
    ViewATMsThresholdHourlyAnalysis = 3069,
    ViewDeadATMs = 3070,
    ViewDateTimeSync = 3071,
    ViewDisputeManagement = 3072,
    ViewEJCashPositionsDashBoard = 3084,
    ViewEJCashPositions = 3085,
    ViewReconciliation = 3086,
    EJCassetteDispensingReport = 3087,
    EJATMWithoutTransaction24Hour = 3088,
    EJSwitchDispensingReport = 3089,
    EJReplenishmentReport = 3090,
    EJCashUtilizationReport = 3091,
    EJReplenishmentReturnedReport = 3092,
    EJCashWithdrawalReport = 3093,
    ViewTransactionAnalysisGraph = 3094,
    ViewReplenishmentAnalysisGraph = 3095,
    ViewCashUtilizationAnalysisGraph = 3096,
    ViewCallOutAnalysisGraph = 3097,
    ViewDenominationUtilizationGraph = 3098,
    ViewEJTransactionAnalysis = 3099,
    ViewGoogleMaps = 3100,
    ViewFraudTransactionsDashBoard = 3101,
    AddEditRules = 3102,
    ViewFraudTransactions = 3103,
    FraudTransactionsReport = 3104,
    ViewRulesList = 3105,
    EditRulesList = 3106,
    DeactivateRules = 3107,
    ViewUploadedData = 3108,
    ViewReconData = 3109,
    ViewGLAccount = 3110,
    CreateGLAccount = 3111,
    EditGLAccount = 3112,
    DeleteGLAccount = 3113,
    ReconciliationReport = 3114,
    GLReport = 3115,
    ModifyInvoice = 3116,
    DeleteInvoice = 3117,
    CreateInvoice = 3118,
    ViewInvoice = 3119,
    ViewInvoiceDetailReport = 3120,
    ViewInvoiceSummaryReport = 3121,
    IssueNewCheque = 3122,
    EditCheque = 3123,
    DeleteCheque = 3124,
    ViewCheque = 3125,
    ViewLedger = 3126,
    ApproveCheque = 3127,
    VaultBalanceInvestigation = 3128,
    ViewVaultSummary = 3129,
    ViewVaultSummaryDashboard = 3130,
    ScheduleInventory = 3131,
    ReplenishmentExceptionReport = 3132,
    CreateVaultAdjustment = 3133,
    EditVaultAdjustment = 3134,
    DeleteVaultAdjustment = 3135,
    ViewVaultAdjustment = 3136,
    ApproveVaultAdjustment = 3137,
    VaultReport = 3138,
    ViewVaultUtilizationAnalysisGraph = 3139,
    ViewPerformanceDashboard = 3140,
    OrderCancelled = 3141,
    ApproveVaultSettlement = 3142,
    MissingSettlementReport = 3143,
    LateSettlementReport = 3144,
    CreateInvestigation = 3145,
    ViewInvestigation = 3146,
    EditInvestigation = 3147,
    ATMNetworkInfo = 3148,
    EditAlertEscalation = 3149,
    EditAlertTemplate = 3150,
    ViewAlertTemplatesList = 3151,
    AddAlertTemplate = 3152,
    DeleteAlertTemplate = 3153,
    AssignTemplate = 3154,
    DeviceTemplate = 3155,
    Devices = 3156,
    EditDevices = 3157,
    AddDeviceTemplate = 3158,
    EditDeviceTemplate = 3159,
    DeleteDeviceTemplate = 3160,
    ViewAlertTemplates = 3161,
    ViewATMInventory = 3162,
    ViewCashPositionReport = 3167,
    ViewPNCAnalysisForCPM = 3168,
    ViewPNCAnalysisForBNA = 3169,
    ViewPNCAnalysisForTop5Incidents = 3170,
    ViewPNCBNATransactions = 3171,
    ViewPNCCPMTransactions = 3172,
    ViewMeanTimeToFailureAnalysis = 3173,
    ViewAvgResponseTimeAnalysis = 3174,
    ViewPNCMeanTimeForBNA = 3175,
    ViewPNCMeanTimeForCPM = 3176,
    ModifyATMSettlement = 3177,
    PNCDepositSummary = 3178,
    PNCIncidentAnalysis = 3179,
    PNCWeeklyAnalysis = 3180,
    PNCDepositDetail = 3181,
    PNCMeanTimeToFailureAnalysisReport = 3182,
    CaptureScreen = 3183,
    TaskbarRunningApps = 3184,
    ServiceStart = 3185,
    ServiceStop = 3186,
    ServiceStatus = 3187,
    ExecInitEJ = 3188,
    ViewSummaryDashboard = 3189,
    ViewATMDeviceStatus = 3190,
    ViewCountsClearedHistory = 3191,
    ViewEJCountsClearedHistory = 3192,
    ViewPNCMeanTimeIncidentForBNA = 3193,
    ViewPNCMeanTimeIncidentForCPM = 3194,
    ViewPNCMeanTimeIncidentForSDM = 3195,
    ViewPNCMeanTimeForSDM = 3196,
    ViewPNCAnalysisForSDM = 3197,
    ViewPNCMeanTimePerIncidentAndMediaForBNA = 3198,
    ViewPNCMeanTimePerIncidentAndMediaForCPM = 3199,
    ViewPNCMeanTimePerIncidentAndMediaForSDM = 3200,
    ViewBankSettlementReport = 3201,
    ViewCITCCMSCounterReport = 3202,
    PostReplenishment = 3203,
    EditReplenishment = 3204,
    DeleteReplenishment = 3205,
    ViewCCMSReplenishment = 3206,
    UploadResource = 3207,
    UploadStatus = 3208,
    ViewPowerMonitoringDashboard = 3209,
    ViewEjModule = 3210,
    ViewCounterModule = 3211,
    ViewSoftwareDistributionModule = 3212,
    ViewPowerMonitoringModule = 3213,
    ViewSensorMonitoringHistory = 3214,
    ViewLinkMonitoringHistory = 3215,
    ViewGasperTicket = 3216,
    ViewReplenishedRemainingNotesReport = 3217,
    ViewMinNotesThresholdReport = 3218,
    ImportBWAC = 3219,
    ImportGasperTicket = 3220,
    ViewBWACCashPositions = 3221,
    ImportQNBExcelSheetTemplate = 3222,
    ViewQNBBankSettlementReport = 3223,
    DeleteATMSettlement = 3224,
    CreateATMClassification = 3225,
    ViewATMClassification = 3226,
    ModifyATMClassification = 3227,
    DeleteATMClassification = 3228,
    PrepareCashOrders = 3229,
    ImportCashOrders = 3230,
    ViewEventsManagement = 3231,
    ViewExceptionHandlingReport = 3232,
    //*****
    //edited by shariq 9sep15
    ViewSmsTemplate = 3233,
    //*****
    //edited by shariq 10sep15
    ViewSmsConfiguration = 3234,
    //edited by shariq 
    ViewSMSTaskStatusMonitoring = 3235,
    ViewExceptionHandlingCriteria = 3236,
    DeleteSMSTask = 3237,
    ViewEjBNADisputedTransactions = 3238,
    ViewEjBNAFailedTransactions = 3239,
    ViewEjCPMDisputedTransactions = 3240,
    ViewEjCPMFailedTransactions = 3241,
    ViewEjCapturedCardTransactions = 3242,
    ViewEjNormalTransactions = 3243,
    ViewIMCCMSDashboard = 3244,
    CalculateInvoiceDeductions = 3245,
    ViewPowerMonitoringReport = 3246,
    ViewEjViewerWithOptions = 3247,
    ViewDistribution = 3248,
    ViewATMTransactions = 3249,
    ViewPerformance = 3250,
    ViewExceptions = 3251,
    ATMProfitability = 3252,
    ViewCashManagementTab = 3253,
    ViewProfitabilityParam = 3254,
    ViewCommsVsPowerMonitoringDashboard = 3255,
    ViewPowerNetworkDownReport = 3256,
    //Added By Aadil
    ReplenishmentExceptionWithoutOrderReport = 3257,
    ReplenishmentExceptionByAmountReport = 3258,
    ReplenishmentExceptionByTimeReport = 3259,

    //Added By Aadil
    ReportTemplate = 3260,
    DashboardTemplate = 3261,
    CustomReports = 3262,
    CustomDashboard = 3263,
    ExecutiveDashboard = 3264,
    TransactionMonitoringDashboard = 3265,
    CashManagementDashboard = 3266,

    // Added by wajahat
    ApproveUser = 50569,
    ApproveGroup = 50570,
    DeletePackage = 3267,
    ViewPackageList = 3268,
    CreatePackage = 3269,
    ViewUploadStatusReport = 3270,
    ViewPackageAnalytics = 3271,
    CreateATMBandwidth = 3272,
    ModifyATMBandwidth = 3273,
    DeleteATMBandwidth = 3274,
    ViewATMBandwidth = 3275,
    DeletePackageScheduledJob = 3276,
    ViewTransactionAnalysisByAmountGraph = 3277,
    AddException = 3278,
    ModifyException = 3279,
    DeleteException = 3280,
    DeactivateUser = 5000,
    ActivateUser = 5001,
    AdvancedATMConfiguration = 5002,
    ViewCPMCountClearedReport = 50600,

    ViewSwitchDispenseTrxn = 60001,
    ViewSwitchBnaTrxn = 60002,
    ViewHostDispenseTrxn = 60003,
    ViewHostBnaTrxn = 60004,
    ViewCCMSvsEjTrxnReconciliation = 60005,
    ViewReconFileImportHistory = 60006,
    ViewReconciliationModule = 60007,


}

/// <summary>
/// Contains all the possible types of CCMS counters file entries.
/// </summary>
public enum EntryTypes : int
{
    /// <summary>
    /// Value designates to the unknown entries.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Value designates to the entries determining change in service state.
    /// </summary>
    ServiceState = 1,

    /// <summary>
    /// Value designates to the entries determining current mode of the ATM.
    /// </summary>
    CurrentMode = 2,

    /// <summary>
    /// Value designates to the entries determining current cassette mode of the ATM.
    /// </summary>
    CassetteMode = 3,

    /// <summary>
    /// Value designates to the entries determining note counts mismatch between last saved in configuration and currently loaded from CDI stores.
    /// </summary>
    Mismatch = 4,

    /// <summary>
    /// Value designates to the entries determining change in current mode.
    /// </summary>
    ModeChanged = 5,

    /// <summary>
    /// Value designates to the cash withdrawal transaction entries.
    /// </summary>
    CashWithdrawal = 6,

    /// <summary>
    /// Value designates to the counts clear transaction entries.
    /// </summary>
    CountsCleared = 7,

    /// <summary>
    /// Value designates to the counts set transaction entries in case of order is in place.
    /// </summary>
    CountsSet = 8,

    /// <summary>
    /// Value designates to the counts set transaction entries in case of order is missing.
    /// </summary>
    OrderMissing = 9,

    /// <summary>
    /// Value designates to the counts set transaction entries in case of counts clear retries exhausted.
    /// </summary>
    RetriesExhausted = 10,

    /// <summary>
    /// Value designates to the add cash transaction entries determining.
    /// </summary>
    AddCash = 11,

    /// <summary>
    /// Value designates to the test cash transaction entries.
    /// </summary>
    TestCash = 12,

    /// <summary>
    /// Value designates to the replenishment entries.
    /// </summary>
    Replenishment = 13,

    /// <summary>
    /// Value designates to the end of day balance entries.
    /// </summary>
    EODBalance = 14,

    /// <summary>
    /// Value designates to the entries determining BNA note counts mismatch between last saved in configuration and currently loaded from CDI stores.
    /// </summary>
    BNAMismatch = 15,

    /// <summary>
    /// Value designates to the entries determining CPM cheque counts mismatch between last saved in configuration and currently loaded from CDI stores.
    /// </summary>
    CPMMismatch = 16,

    /// <summary>
    /// Value designates to the cash deposit transaction entries.
    /// </summary>
    CashDeposit = 17,

    /// <summary>
    /// Value designates to the cheque deposit transaction entries.
    /// </summary>
    ChequeDeposit = 18,

    /// <summary>
    /// Value designates to the BNA counts clear transaction entries.
    /// </summary>
    BNACountsCleared = 19,

    /// <summary>
    /// Value designates to the CPM counts clear transaction entries.
    /// </summary>
    CPMCountsCleared = 20,

    /// <summary>
    /// Value designates to the BNA end of day balance entries.
    /// </summary>
    BNAEODBalance = 21,

    /// <summary>
    /// Value designates to the CPM end of day balance entries.
    /// </summary>
    CPMEODBalance = 22,
    OrderId = 23
}