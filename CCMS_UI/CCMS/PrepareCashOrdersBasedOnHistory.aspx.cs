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
using System.Data.SqlClient;

using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using ceTe.DynamicPDF;
using System.Collections.Generic;
using System.Web.Services;

namespace CCMSUI.CCMS
{
    public partial class PrepareCashOrdersBasedOnHistory : BasePage
    {
        [WebMethod]
        public static string GetOrderDetail(int atmID, int type)
        {
            StringBuilder builder = new StringBuilder();
            Atm atm = Atm.LoadAtmByPk(atmID);
            if (atm != null)
            {
                if (type == 1)
                    builder.Append(atm.Type1SuggestedNotesNormalDays + "|" + atm.Type2SuggestedNotesNormalDays + "|" + atm.Type3SuggestedNotesNormalDays + "|" + atm.Type4SuggestedNotesNormalDays);
                else if (type == 2)
                    builder.Append(atm.Type1SuggestedNotesSalaryDays + "|" + atm.Type2SuggestedNotesSalaryDays + "|" + atm.Type3SuggestedNotesSalaryDays + "|" + atm.Type4SuggestedNotesSalaryDays);
            }
            return builder.ToString();
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            //Button_search_Click(this, null);
        }

        Notifications objNotifications = null;
        Classification.ClassificationReader classificationReader = null;
        private class Notifications
        {
            public string type;
            public string title;
        }
        private int isATMExists(List<Notifications> list, string title)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].title == title)
                    return i;
            return -1;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["GetMsgs"] != null)
                {
                    List<Notifications> list = new List<Notifications>();

                    //n.id = 1;
                    //n.msg = "hello world";

                    classificationReader = Classification.ExecuteReader("");
                    while (classificationReader.Read())
                    {
                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                        cmd.CommandText = @"select title,classification_name  
                                            from atm_classification inner join classification on atm_classification.classification_id = classification.classification_id
                                            inner join atm on atm.atm_id = atm_classification.atm_id
                                            where classification.classification_id = " + classificationReader.CurrentClassification.ClassificationId.ToString();

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        foreach (DataRow dr in dt.Rows)
                        {
                            int idx = isATMExists(list, dr["title"].ToString());
                            if (idx == -1)
                            {
                                objNotifications = new Notifications();
                                objNotifications.title = dr["title"].ToString();
                                objNotifications.type = dr["classification_name"].ToString();
                                list.Add(objNotifications);
                            }
                            else
                            {
                                objNotifications = list[idx];
                                objNotifications.type = objNotifications.type + "," + dr["classification_name"].ToString();
                            }

                        }
                    }

                    classificationReader.Close();

                    Response.ContentType = "json";
                    System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    Response.Write(jsSerializer.Serialize(list));
                    Response.End();
                }
                if (!CheckRight((int)Permissions.PrepareCashOrders, true))
                    Response.Redirect("../admin/AccessDenied.aspx");

                classificationReader = Classification.ExecuteReader("");

                CheckBoxList_Classification.DataSource = classificationReader;
                CheckBoxList_Classification.DataTextField = "ClassificationName";
                CheckBoxList_Classification.DataValueField = "ClassificationId";
                CheckBoxList_Classification.DataBind();
                classificationReader.Close();


                ViewState["SortExpression"] = "total ";
                ViewState["sortDirection"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
        }

        public class VaultInfo
        {
            public int newVaultId = 0;
            public int existingVaultId = 0;
        }
        private VaultInfo CreateTempVaultLedger(int vaultId, decimal amount, int atmId, string transactionType, string desc, string vaultTrxnType, int orderId, SqlTransaction trxn)
        {
            VaultInfo vaultInfo = new VaultInfo();
            CcmsTempVaultLedger vaultLedger = new CcmsTempVaultLedger();
            vaultLedger.TransactionDate = DateTime.Now;
            vaultLedger.TransactionType = transactionType;
            vaultLedger.Description = desc;
            vaultLedger.LedgerAmount = amount;
            vaultLedger.PostedBy = 1;
            vaultLedger.VaultId = vaultId;
            vaultLedger.VaultTransactionType = vaultTrxnType;
            vaultLedger.AtmId = atmId;
            vaultLedger.OrderId = orderId;
            object result = ConnectionFactory.ExecuteScalar("select max(id) from ccms_temp_vault_ledger where vault_id = " + vaultId);
            if (result != DBNull.Value)
            {
                CcmsTempVaultLedger existingLedger = CcmsTempVaultLedger.LoadCcmsTempVaultLedgerByPk(int.Parse(result.ToString()));
                if (existingLedger != null)
                {
                    vaultInfo.existingVaultId = existingLedger.Id;
                    if (transactionType == "Cr")
                        vaultLedger.Balance = existingLedger.Balance + vaultLedger.LedgerAmount;
                    else
                        vaultLedger.Balance = existingLedger.Balance - vaultLedger.LedgerAmount;

                }

            }
            else
            {
                if (transactionType == "Cr")
                    vaultLedger.Balance = vaultLedger.LedgerAmount;
                else
                    vaultLedger.Balance = -vaultLedger.LedgerAmount;
            }

            vaultLedger.Save(trxn.Connection, trxn);
            vaultInfo.newVaultId = vaultLedger.Id;
            return vaultInfo;
        }

        private void CreateTempVaultLedgerDetail(VaultInfo vaultInfo, int qty, int vaultNoteTypeId, string trxnType, bool isContraEntry, SqlTransaction trxn)
        {
            CcmsTempVaultLedgerDetail vaultLedgerDetail = new CcmsTempVaultLedgerDetail();
            vaultLedgerDetail.LedgerId = vaultInfo.newVaultId;
            vaultLedgerDetail.Quantity = qty;
            vaultLedgerDetail.VaultNoteTypeId = vaultNoteTypeId;
            CcmsTempVaultLedgerDetail existingVaultLedgerDetail = CcmsTempVaultLedgerDetail.LoadCcmsTempVaultLedgerDetail(
                string.Format("ledger_id = {0} and vault_note_type_id={1}", vaultInfo.existingVaultId, vaultNoteTypeId));
            if (existingVaultLedgerDetail != null)
            {
                if (vaultLedgerDetail.Balance == null)
                    vaultLedgerDetail.Balance = 0;

                if (existingVaultLedgerDetail.Balance == null)
                    existingVaultLedgerDetail.Balance = 0;



                if (trxnType == "Cr")
                    vaultLedgerDetail.Balance = existingVaultLedgerDetail.Balance + qty;
                else if (trxnType == "Dr")
                    vaultLedgerDetail.Balance = existingVaultLedgerDetail.Balance - qty;
            }
            else
            {
                if (trxnType == "Cr")
                    vaultLedgerDetail.Balance = qty;
                else
                    vaultLedgerDetail.Balance = -qty;
            }
            if (!isContraEntry)
            {
                CcmsVaultNoteType ccmsVaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteTypeByPk(vaultNoteTypeId);
                if (vaultLedgerDetail.Balance <= ccmsVaultNoteType.ThrasholdQty)
                {
                    CcmsVault ccmsVault = CcmsVault.LoadCcmsVaultByPk(ccmsVaultNoteType.VaultId);
                    GenerateGeneralAlert((int)EnumAlertType.VaultLowBalanceThresholdReached, "Vault[" + ccmsVault.Title + "] Threshold reached for denomination " + ccmsVaultNoteType.DenominationName, vaultLedgerDetail.LedgerId, EntityType.VaultLedger, trxn, Event_Type.Warning);
                }
            }

            vaultLedgerDetail.Save(trxn.Connection, trxn);
        }


        public void GenerateGeneralAlert(int alertTypeID, string msg, int entityId, EntityType entityType, SqlTransaction trxn, Event_Type eventType)
        {
            LogableTask task = LogableTask.NewTask("GenerateGeneralAlert");
            try
            {

                GeneralAlert alert = new GeneralAlert();
                alert.GeneratedAt = DateTime.Now;
                alert.AlertTypeId = alertTypeID;
                alert.ExpirationTime = DateTime.Now.AddDays(((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).AlertExpirationTime.Value);

                //                alert.ExpirationTime = DateTime.Now.AddDays(int.Parse(ConfigurationManager.AppSettings["AlertExpirationTime"]));
                alert.AlertMsg = msg;
                alert.RetryRemaining = 10;
                alert.GenerateNotificationSent = false;
                alert.EntityId = entityId;
                alert.EntityType = entityType.ToString();
                alert.Save(trxn.Connection, trxn);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Alert added of type " + alertTypeID);
                GenerateIntegratedAlert(alertTypeID, msg, entityId, entityType,
                    eventType, null, trxn);
            }

            finally
            {
                task.EndTask();
            }
        }
        public void GenerateIntegratedAlert(int alertTypeID, string msg,
           int entityID, EntityType entityType, Event_Type eventType, int? ftpFileInfoId, SqlTransaction trxn)
        {
            LogableTask task = LogableTask.NewTask("GenerateIntegratedAlert");
            int orgID = -1;
            try
            {
                CcmsIntegratedAlert alert = new CcmsIntegratedAlert();
                alert.AlertTypeId = alertTypeID;
                alert.AlertType = AlertType.LoadAlertTypeByPk(alertTypeID).AlertTypeName;
                alert.EntityId = entityID;
                alert.EntityType = entityType.ToString();
                alert.AlertLevel = eventType.ToString();
                alert.AlertStatus = "Unread";
                alert.GeneratedAt = DateTime.Now;
                alert.AlertText = msg;
                alert.ExpirationTime = DateTime.Now.AddDays(((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).AlertExpirationTime.Value);
                alert.GenerateNotificationSent = false;
                alert.ResolveNotificationSent = false;
                if (alert.EntityType == EntityType.ATM.ToString())
                {
                    Atm atm = Atm.LoadAtmByPk(entityID);
                    alert.GenerateRetryRemaining = atm.RetryCountAlert;
                    alert.ResolveRetryRemaining = atm.RetryCountAlert;
                    orgID = GetOrganization(atm.RegionId);
                }
                else if (alert.EntityType == EntityType.Organization.ToString())
                {
                    alert.FtpFileInfoId = ftpFileInfoId;
                    FtpFileInfo ftpFileInfo = FtpFileInfo.LoadFtpFileInfoByPk(ftpFileInfoId.Value);
                    Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(ftpFileInfo.RegionId);
                    alert.GenerateRetryRemaining = region.RetryCountAlert; // add field in region table for this....
                    alert.ResolveRetryRemaining = region.RetryCountAlert;
                    orgID = FtpFileInfo.LoadFtpFileInfoByPk(ftpFileInfoId.Value).RegionId;
                }
                alert.ModuleType = "CURRENCY";
                alert.OrganizationId = orgID;

                if (trxn != null)
                    alert.Save(trxn.Connection, trxn);
                else
                    alert.Save();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Alert added of type " + alertTypeID);
            }

            finally
            {
                task.EndTask();
            }
        }
        public int GetOrganization(int region_id)
        {
            Avanza.CCMS.DAL.Region region = Avanza.CCMS.DAL.Region.LoadRegionByPk(region_id);
            if (region.IsOrganization)
            {
                return region.RegionId;
            }
            else
                return GetOrganization(region.ParentRegionId.Value);
        }

        private void BindGrid(string orderBy)
        {
            DataTable dtGrid = null;
            StringBuilder builder = new StringBuilder();
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");
            //string atmTitle = null;
            int matched = 0;
            int selected = 0;
            try
            {
                //this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                //this.Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(this.metaRefresh.Content) / 60) + "\");} init();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetCashPositions";
                cmd.Parameters.Add("@Date", SqlDbType.VarChar);
                cmd.Parameters.Add("@NoteSetTypeId", SqlDbType.Int);
                cmd.Parameters.Add("@MinNotesAlertExists", SqlDbType.Int);
                cmd.Parameters.Add("@OrderBy", SqlDbType.VarChar);
                cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);

                cmd.Parameters[0].Value = DateTime.Today.ToString("dd/MM/yyyy");

                if (Request.QueryString["n"] != null)
                    cmd.Parameters[1].Value = int.Parse(Request.QueryString["n"]);
                else
                    cmd.Parameters[1].Value = 0;

                if (Request.QueryString["f"] != null)
                    cmd.Parameters[2].Value = int.Parse(Request.QueryString["f"]);
                else
                    cmd.Parameters[2].Value = 0;

                cmd.Parameters[3].Value = orderBy;
                cmd.Parameters[4].Value = Session[SessionVars.SelectedATMs.ToString()];


                //cmd.CommandText = "select top 1000 outerATM.atm_id,outerATM.title title, note_set_type.denomination_type_1, " +
                //    " note_set_type.denomination_type_2, note_set_type.denomination_type_3,note_set_type.denomination_type_4 " +
                //" , note_set_type.denomination_type_5, note_set_type.denomination_type_6, note_set_type.denomination_type_7, " +
                //" (select max(trxn_datetime) from parsed_transaction where atm_id = outerATM.atm_id and amount>0 )last_trxn_at, " +
                //" p.cassette1_notes, p.cassette2_notes, " +
                // " p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes, " +
                //" p.cassette7_notes, note_set_type.denomination_type_1 * p.cassette1_notes +  " +
                //" note_set_type.denomination_type_2 * p.cassette2_notes + note_set_type.denomination_type_3 * p.cassette3_notes  " +
                //" + note_set_type.denomination_type_4 * p.cassette4_notes + note_set_type.denomination_type_5 * p.cassette5_notes + " +
                //" note_set_type.denomination_type_6 * p.cassette6_notes + note_set_type.denomination_type_7 * p.cassette7_notes total ," +
                //" (p.purge_cassette1_notes+p.purge_cassette2_notes+p.purge_cassette3_notes+p.purge_cassette4_notes+p.purge_cassette5_notes+" +
                //" p.purge_cassette6_notes+p.purge_cassette7_notes) purged_counts," +
                //" (p.purge_cassette1_notes*note_set_type.denomination_type_1+p.purge_cassette2_notes*note_set_type.denomination_type_2+" +
                //" p.purge_cassette3_notes*note_set_type.denomination_type_3+p.purge_cassette4_notes*note_set_type.denomination_type_4 " +
                //" +p.purge_cassette5_notes*note_set_type.denomination_type_5+p.purge_cassette6_notes*note_set_type.denomination_type_6 " +
                //" +p.purge_cassette7_notes*note_set_type.denomination_type_7) purged_amount,( select max(rep_datetime) from replenishment where atm_id = outerATM.atm_id )last_replenished_at, " +
                //" ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenished_at,  " +
                //" ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id   and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount from atm outerATM , cash_position p ,note_set_type where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 " +

                //" and outerATM.exclude_dff = 0 and outerATM.note_set_type_id = note_set_type.note_set_type_id " +
                //(Request.QueryString["n"] != null ? " and outerATM.note_set_type_id = " + Request.QueryString["n"] : "") +
                //" and last_trxn_at = (select max(last_trxn_at)from cash_position where cash_position.atm_id = outerATM.atm_id)  order by " + orderBy;



                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                //Handling filters
                dt.Columns.Add("is_rep");
                dt.Columns.Add("is_min_threshold_reached1");
                dt.Columns.Add("is_min_threshold_reached2");
                dt.Columns.Add("is_min_threshold_reached3");
                dt.Columns.Add("is_min_threshold_reached4");
                dt.Columns.Add("is_cassette_faulty");
                dt.Columns.Add("is_dispensing_failed");


                if (CheckBoxList_Classification.SelectedItem != null)
                {
                    DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id" });
                    foreach (DataRow dr in dtATMs.Rows)
                        builder.Append(dr[0] + ",");
                    builder.Append("-1");
                    cmd.CommandType = CommandType.Text;
                    //Get Classification
                    cmd.CommandText = "select classification_id,atm_id from atm_classification where atm_id in (" + builder.ToString() + ")";
                    DataTable dtClassification = new DataTable();
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dtClassification);

                    if (dtClassification.Rows.Count > 0)
                    {
                        foreach (ListItem li in CheckBoxList_Classification.Items)
                        {
                            if (li.Selected)
                                selected++;
                        }

                        dtGrid = dt.Clone();
                        foreach (DataRow dr in dt.Rows)
                        {
                            DataRow[] drArray = dtClassification.Select("atm_id = " + dr["atm_id"]);
                            if (drArray.Length > 0)
                            {
                                matched = 0;
                                foreach (DataRow drClassification in drArray)
                                {
                                    foreach (ListItem li in CheckBoxList_Classification.Items)
                                    {
                                        if (li.Value == drClassification["classification_id"].ToString())
                                        {
                                            matched++;
                                            break;
                                        }
                                    }
                                }
                                if (matched == selected)
                                {
                                    AddToGridTable(dtGrid, dr);
                                }
                            }
                        }
                        dt = dtGrid;
                    }

                    //Handling filters

                }

                //Filtering on the basis of last replenishment;
                //////////////////////////////////////
                if (TextBox_LastReplenishment.Text.Length > 0)
                {
                    //dtGrid.Rows.Clear();
                    builder = new StringBuilder();
                    DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id" });
                    foreach (DataRow dr in dtATMs.Rows)
                        builder.Append(dr[0] + ",");
                    builder.Append("-1");
                    cmd.CommandText = "select atm_id, max(rep_datetime) from replenishment where atm_id in (" + builder.ToString() + ") group by atm_id";
                    DataTable dtReplenishment = new DataTable();
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dtReplenishment);
                    if (dtReplenishment.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtATMs.Rows)
                        {
                            DataRow[] drArray = dtReplenishment.Select("atm_id = " + dr["atm_id"]);
                            if (drArray.Length > 0)
                            {
                                if (!(DateTime.Parse(drArray[0][1].ToString()) < DateTime.Now.AddDays(-double.Parse(TextBox_LastReplenishment.Text))))
                                {
                                    dt.Rows.Remove(dt.Select("atm_id = " + dr["atm_id"])[0]);
                                    //AddToGridTable(dtGrid, dr);
                                }
                            }
                        }

                    }
                }
                //Handling period 

                if (TextBox_Period.Text.Length > 0)
                {
                    decimal avgDispensedNormalDays = 0;
                    decimal avgDispensedSalaryDays = 0;
                    decimal minOperatingBalanceNormalDays = 0;
                    decimal minOperatingBalanceSalaryDays = 0;


                    decimal spareCash = 0;
                    decimal balance = 0;
                    decimal amountToDeduct = 0;
                    int iPeriod = int.Parse(TextBox_Period.Text);
                    List<DateTime> dtEvents = Avanza.CCMS.DAL.Utility.GetEvents("Salary");
                    foreach (DataRow dr in dt.Rows)
                    {
                        Atm atm = Atm.LoadAtmByPk(int.Parse(dr["atm_id"].ToString()));
                        if (atm != null)
                        {
                            if (atm.SpareCash.HasValue)
                            {
                                spareCash = atm.SpareCash.Value;
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "spare cash : " + spareCash);
                            }

                            if (atm.AvgDispensed.HasValue)
                            {
                                avgDispensedNormalDays = atm.AvgDispensed.Value;
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "avgDispensedNormalDays : " + avgDispensedNormalDays);
                            }

                            if (atm.AvgDispensedSalaryDays.HasValue)
                            {
                                avgDispensedSalaryDays = atm.AvgDispensedSalaryDays.Value;
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "avgDispensedSalaryDays : " + avgDispensedSalaryDays);
                            }

                            if (atm.MinOperatingBalanceSalaryDays.HasValue)
                            {
                                minOperatingBalanceSalaryDays = atm.MinOperatingBalanceSalaryDays.Value;
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "minOperatingBalanceSalaryDays : " + minOperatingBalanceSalaryDays);
                            }

                            if (atm.MinOperatingBalanceNormalDays.HasValue)
                            {
                                minOperatingBalanceNormalDays = atm.MinOperatingBalanceNormalDays.Value;
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "minOperatingBalanceNormalDays : " + minOperatingBalanceNormalDays);
                            }



                            for (int i = 0; i < iPeriod; i++)
                            {
                                if (dtEvents.Contains(DateTime.Today.AddDays(i + 1)))
                                {
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, DateTime.Now.AddDays(i + 1) + " is defined as salary days");
                                    amountToDeduct += avgDispensedSalaryDays;
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "amountToDeduct : " + amountToDeduct);
                                }
                                else
                                {
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, DateTime.Now.AddDays(i + 1) + " is defined as normal days");
                                    amountToDeduct += avgDispensedNormalDays;
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "amountToDeduct : " + amountToDeduct);
                                }
                            }
                            balance = decimal.Parse(dr["total"].ToString()) - amountToDeduct - spareCash;
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, string.Format("current balance ={0} - amountToDeduct={1} - spareCash={2}=balance={3} ", dr["total"], amountToDeduct, spareCash, balance));

                            if (dtEvents.Contains(DateTime.Today.AddDays(iPeriod)))
                            {
                                if (balance <= minOperatingBalanceSalaryDays)
                                {
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "balance < MinOperatingBalanceSalaryDays : " + balance);
                                    dr["is_rep"] = "1";

                                }
                            }
                            else
                            {
                                if (balance <= minOperatingBalanceNormalDays)
                                {
                                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "balance < MinOperatingBalanceNormalDays : " + balance);
                                    dr["is_rep"] = "1";
                                }

                            }
                        }
                    }
                }


                //Handling Type minimum threshold 
                ///////////////////////////////////////
                //dtGrid.Rows.Clear();
                int foundItemCount = 0;
                builder = new StringBuilder();
                DataTable dataTableATM = dt.DefaultView.ToTable(true, new string[] { "atm_id" });
                foreach (DataRow dr in dataTableATM.Rows)
                    builder.Append(dr[0] + ",");
                builder.Append("-1");
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select alert_type_id,atm_id from atm_alert where alert_type_id in (" + DropDownList_Types.SelectedItem.Value + ") and resolve_at is null and atm_id in (" + builder.ToString() + ")";
                adapter = new SqlDataAdapter(cmd);
                DataTable dtAlert = new DataTable();
                adapter.Fill(dtAlert);

                if (dtAlert.Rows.Count > 0)
                {
                    //is_min_threshold_reached1
                    foreach (DataRow dr in dataTableATM.Rows)
                    {
                        DataRow[] drArray = dtAlert.Select("atm_id = " + dr["atm_id"]);
                        //All alerts for this atm found...
                        if (drArray.Length == 0)
                            dt.Rows.Remove(dt.Select("atm_id = " + dr["atm_id"])[0]);
                        else
                        {
                            DataRow drGrid = dt.Select("atm_id = " + dr["atm_id"])[0];
                            string[] parts = DropDownList_Types.SelectedItem.Value.Split(',');
                            for (int i = 0; i < parts.Length; i++)
                            {
                                foreach (DataRow drInner in drArray)
                                {
                                    if (drInner["alert_type_id"].ToString() == parts[i])
                                    {
                                        foundItemCount++;
                                        if (parts[i] == "46")
                                            drGrid["is_min_threshold_reached1"] = 1;
                                        else if (parts[i] == "47")
                                            drGrid["is_min_threshold_reached2"] = 1;
                                        else if (parts[i] == "48")
                                            drGrid["is_min_threshold_reached3"] = 1;
                                        else if (parts[i] == "49")
                                            drGrid["is_min_threshold_reached4"] = 1;
                                        break;
                                    }
                                }
                            }
                            if (foundItemCount != parts.Length && DropDownList_Types.SelectedIndex > 0)
                                dt.Rows.Remove(dt.Select("atm_id = " + dr["atm_id"])[0]);

                        }
                    }
                }
                else
                {
                    if (DropDownList_Types.SelectedIndex > 0)
                        dt = new DataTable();
                }


                //Handling cassette faulty and cassette dispensing cases
                if (dt.Rows.Count > 0)
                {
                    builder = new StringBuilder();
                    dataTableATM = dt.DefaultView.ToTable(true, new string[] { "atm_id" });
                    foreach (DataRow dr in dataTableATM.Rows)
                        builder.Append(dr[0] + ",");
                    builder.Append("-1");
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select alert_type_id,atm_id from atm_alert where alert_type_id in (32,36) and resolve_at is null and atm_id in (" + builder.ToString() + ")";
                    adapter = new SqlDataAdapter(cmd);
                    dtAlert = new DataTable();
                    adapter.Fill(dtAlert);

                    if (dtAlert.Rows.Count > 0)
                    {
                        //is_min_threshold_reached1
                        foreach (DataRow dr in dataTableATM.Rows)
                        {
                            DataRow[] drArray = dtAlert.Select("atm_id = " + dr["atm_id"]);
                            if (drArray.Length > 0)
                            {
                                DataRow temp = dt.Select("atm_id = " + dr["atm_id"])[0];
                                foreach (DataRow drTemp in drArray)
                                {
                                    if (drTemp["alert_type_id"].ToString() == "32")
                                        temp["is_cassette_faulty"] = 1;
                                    if (drTemp["alert_type_id"].ToString() == "36")
                                        temp["is_dispensing_failed"] = 1;

                                }
                            }

                        }
                    }
                }



                //foreach (DataRow dr in dt.Rows)
                //{
                //    if (int.Parse(dr["alert_type_id"].ToString()) == 46)
                //    {
                //        e.Row.Cells[5].ForeColor = System.Drawing.Color.Yellow;

                //    }
                //    else if (int.Parse(dr["alert_type_id"].ToString()) == 47)
                //    {
                //        e.Row.Cells[7].ForeColor = System.Drawing.Color.Yellow;
                //    }
                //    else if (int.Parse(dr["alert_type_id"].ToString()) == 48)
                //    {
                //        e.Row.Cells[9].ForeColor = System.Drawing.Color.Yellow;
                //    }
                //    else if (int.Parse(dr["alert_type_id"].ToString()) == 49)
                //    {
                //        e.Row.Cells[11].ForeColor = System.Drawing.Color.Yellow;
                //    }

                //}



                // cmd.CommandText = "select atm_id, max(rep_datetime) from replenishment where atm_id in (" + builder.ToString() + ") group by atm_id";


                ///////////////////////////////////////


                this.GridView_CashPositions.DataSource = dt;
                this.GridView_CashPositions.DataBind();
                Literal_AddHeading.Text = "NCR Cash Positions Monitoring DashBoard - Total No.Of ATMs : " + dt.Rows.Count;
            }


//            try
            //            {
            //                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
            //                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

//                cmd = ConnectionFactory.GetNewCommand(false);
            //                cmd.CommandTimeout = 500;
            //                //                cmd.CommandText = @"select top 1000 atm.title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
            //                //                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
            //                //                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
            //                //                                    p.last_trxn_at, p.cassette1_notes, p.cassette2_notes,
            //                //                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
            //                //                                    p.cassette7_notes, task.last_invoked ,
            //                //note_set_type.denomination_type_1 * p.cassette1_notes + 
            //                //note_set_type.denomination_type_2 * p.cassette2_notes + 
            //                //note_set_type.denomination_type_3 * p.cassette3_notes + 
            //                //note_set_type.denomination_type_4 * p.cassette4_notes + 
            //                //note_set_type.denomination_type_5 * p.cassette5_notes + 
            //                //note_set_type.denomination_type_6 * p.cassette6_notes + 
            //                //note_set_type.denomination_type_7 * p.cassette7_notes total 
            //                //                                    from atm inner join cash_position p on atm.ATM_id = p.atm_id inner join task on p.task_id = task.task_id
            //                //                                    inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id where atm.IS_ACTIVE = 1 order by atm.title";


//                cmd.CommandText = @"select top 1000 outerATM.atm_id,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
            //                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
            //                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
            //                                    p.last_trxn_at, p.cassette1_notes, p.cassette2_notes,
            //                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
            //                                    p.cassette7_notes, task.last_invoked ,
            //note_set_type.denomination_type_1 * p.cassette1_notes + 
            //note_set_type.denomination_type_2 * p.cassette2_notes + 
            //note_set_type.denomination_type_3 * p.cassette3_notes + 
            //note_set_type.denomination_type_4 * p.cassette4_notes + 
            //note_set_type.denomination_type_5 * p.cassette5_notes + 
            //note_set_type.denomination_type_6 * p.cassette6_notes + 
            //note_set_type.denomination_type_7 * p.cassette7_notes total ,
            //(p.purge_cassette1_notes+p.purge_cassette2_notes+p.purge_cassette3_notes+p.purge_cassette4_notes+p.purge_cassette5_notes+
            //p.purge_cassette6_notes+p.purge_cassette7_notes) purged_counts,
            //
            //(p.purge_cassette1_notes*note_set_type.denomination_type_1+p.purge_cassette2_notes*note_set_type.denomination_type_2+
            //p.purge_cassette3_notes*note_set_type.denomination_type_3+p.purge_cassette4_notes*note_set_type.denomination_type_4
            //+p.purge_cassette5_notes*note_set_type.denomination_type_5+p.purge_cassette6_notes*note_set_type.denomination_type_6
            //+p.purge_cassette7_notes*note_set_type.denomination_type_7) purged_amount,
            //( select max(rep_datetime) from replenishment where atm_id = outerATM.atm_id )last_replenished_at,
            //
            //( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    
            //and cash_order_datetime>=convert(datetime,'"+DateTime.Today.ToString("dd/MM/yyyy")+"',103))next_replenished_at, " +

//" ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
            //" and cash_order_datetime>=convert(datetime,'"+DateTime.Today.ToString("dd/MM/yyyy")+"',103))amount"+

//" from atm outerATM , cash_position p ,task,note_set_type " +
            //"where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 and outerATM.exclude_dff = 0 " +
            //"and p.task_id = task.task_id " +
            //"and outerATM.note_set_type_id = note_set_type.note_set_type_id " +
            //"and last_trxn_at = (select max(last_trxn_at)" +
            //"from cash_position " +
            //"where cash_position.atm_id = outerATM.atm_id)  order by " + orderBy;

//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //                DataTable dt = new DataTable();
            //                adapter.Fill(dt);

//                GridView_CashPositions.DataSource = dt;
            //                GridView_CashPositions.DataBind();
            //            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();

                task.EndTask();
            }
        }

        private static void AddToGridTable(DataTable dtGrid, DataRow dr)
        {
            DataRow newRow = dtGrid.NewRow();
            newRow["title"] = dr["title"];
            newRow["last_trxn_at"] = dr["last_trxn_at"];
            newRow["last_replenished_at"] = dr["last_replenished_at"];
            newRow["denomination_type_1"] = dr["denomination_type_1"];
            newRow["cassette1_notes"] = dr["cassette1_notes"];
            newRow["denomination_type_2"] = dr["denomination_type_2"];
            newRow["cassette2_notes"] = dr["cassette2_notes"];
            newRow["denomination_type_3"] = dr["denomination_type_3"];
            newRow["cassette3_notes"] = dr["cassette3_notes"];
            newRow["denomination_type_4"] = dr["denomination_type_4"];
            newRow["cassette4_notes"] = dr["cassette4_notes"];
            newRow["total"] = dr["total"];
            newRow["next_replenished_at"] = dr["next_replenished_at"];
            newRow["atm_id"] = dr["atm_id"];


            dtGrid.Rows.Add(newRow);
        }
        protected void Button_Search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        }
        protected void GridView_CashPositions_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        }

        protected void DropDownList_OrderType_IndexChanged(object sender, EventArgs e)
        {
            GridViewRow gvr = (GridViewRow)(((Control)sender).NamingContainer);
            string atmTitle = gvr.Cells[2].Text;
            DropDownList dropdownlist1 = (DropDownList)gvr.FindControl("DropDownList_OrderType");
            if (dropdownlist1.SelectedItem.Text == "N")
            {
                Atm atm = Atm.LoadAtm("title='" + atmTitle + "'");
                if (atm != null)
                {
                    if (atm.Type1SuggestedNotesNormalDays.HasValue)
                        ((TextBox)gvr.FindControl("TextBox_Type1")).Text = atm.Type1SuggestedNotesNormalDays.Value.ToString();

                    if (atm.Type2SuggestedNotesNormalDays.HasValue)
                        ((TextBox)gvr.FindControl("TextBox_Type2")).Text = atm.Type2SuggestedNotesNormalDays.Value.ToString();

                    if (atm.Type3SuggestedNotesNormalDays.HasValue)
                        ((TextBox)gvr.FindControl("TextBox_Type3")).Text = atm.Type3SuggestedNotesNormalDays.Value.ToString();

                    if (atm.Type4SuggestedNotesNormalDays.HasValue)
                        ((TextBox)gvr.FindControl("TextBox_Type4")).Text = atm.Type4SuggestedNotesNormalDays.Value.ToString();
                }
            }
            else if (dropdownlist1.SelectedItem.Text == "S")
            {
                Atm atm = Atm.LoadAtm("title='" + atmTitle + "'");
                if (atm != null)
                {
                    if (atm.Type1SuggestedNotesSalaryDays.HasValue)
                        ((TextBox)gvr.FindControl("TextBox_Type1")).Text = atm.Type1SuggestedNotesSalaryDays.Value.ToString();

                    if (atm.Type2SuggestedNotesSalaryDays.HasValue)
                        ((TextBox)gvr.FindControl("TextBox_Type2")).Text = atm.Type2SuggestedNotesSalaryDays.Value.ToString();

                    if (atm.Type3SuggestedNotesSalaryDays.HasValue)
                        ((TextBox)gvr.FindControl("TextBox_Type3")).Text = atm.Type3SuggestedNotesSalaryDays.Value.ToString();

                    if (atm.Type4SuggestedNotesSalaryDays.HasValue)
                        ((TextBox)gvr.FindControl("TextBox_Type4")).Text = atm.Type4SuggestedNotesSalaryDays.Value.ToString();
                }
            }
        }
        protected void GridView_CashPositions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool isMinthresholdReachedForAnyType = false;
                bool isCassetteFaulty = false;
                bool isDispensingFailed = false;
                Image img = (Image)e.Row.FindControl("img");
                if (((DataRowView)e.Row.DataItem).Row["is_rep"].ToString() == "1")
                {
                    img.Attributes.Add("src", "../images/attention_now.jpg");
                    img.Visible = true;
                }
                else
                    img.Visible = false;


                if (((DataRowView)e.Row.DataItem).Row["is_min_threshold_reached1"].ToString() == "1")
                {
                    e.Row.Cells[6].ForeColor = System.Drawing.Color.Red;
                    isMinthresholdReachedForAnyType = true;
                }
                if (((DataRowView)e.Row.DataItem).Row["is_min_threshold_reached2"].ToString() == "1")
                {
                    e.Row.Cells[8].ForeColor = System.Drawing.Color.Red;
                    isMinthresholdReachedForAnyType = true;
                }
                if (((DataRowView)e.Row.DataItem).Row["is_min_threshold_reached3"].ToString() == "1")
                {
                    e.Row.Cells[10].ForeColor = System.Drawing.Color.Red;
                    isMinthresholdReachedForAnyType = true;
                }
                if (((DataRowView)e.Row.DataItem).Row["is_min_threshold_reached4"].ToString() == "1")
                {
                    e.Row.Cells[12].ForeColor = System.Drawing.Color.Red;
                    isMinthresholdReachedForAnyType = true;
                }



                if (((DataRowView)e.Row.DataItem).Row["is_cassette_faulty"].ToString() == "1")
                {
                    e.Row.Cells[2].BackColor = System.Drawing.Color.Orange;
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    isCassetteFaulty = true;
                }
                if (((DataRowView)e.Row.DataItem).Row["is_dispensing_failed"].ToString() == "1")
                {
                    e.Row.Cells[2].BackColor = System.Drawing.Color.Red;
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    isDispensingFailed = true;
                }

                if (isMinthresholdReachedForAnyType == false && isDispensingFailed == false && isCassetteFaulty == false)
                {
                    e.Row.Cells[2].BackColor = System.Drawing.Color.Green;
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;

                }



                //string atm_id = this.GridView_CashPositions.DataKeys[e.Row.DataItemIndex].Values["atm_id"].ToString();
                //Atm atm = Atm.LoadAtmByPk(int.Parse(atm_id));
                //if (!atm.IsHealthy)
                //    e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                //if (atm.MinOperatingBalance == null)
                //{
                //    img.Attributes.Add("src", "../../images/help.jpg");
                //    img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Minimum operating balance is not defined]");
                //}
                //else if (decimal.Parse(e.Row.Cells[18].Text) <= atm.MinOperatingBalance)
                //{
                //    if (decimal.Parse(e.Row.Cells[18].Text) <= 50000)
                //    {
                //        img.Attributes.Add("src", "../../images/attention_now.jpg");

                //        img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Current balance is less than equal to 50K.Minimum Operating balance (" + atm.MinOperatingBalance + ")]");
                //    }
                //    else
                //    {
                //        img.Attributes.Add("src", "../../images/attention_soon.jpg");

                //        img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Current balance is less then min operating balance.Minimum operating balance (" + atm.MinOperatingBalance + ")]");
                //    }
                //}
                //else
                //{
                //    img.Attributes.Add("src", "../../images/healthy.jpg");

                //    img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[ATM is in healthy state.Minimum operating balance (" + atm.MinOperatingBalance + ")]");
                //}


            }

        }

        protected void Button_GenerateOrder_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            LogableTask task = LogableTask.NewTask("GenerateOrder");
            int orderGeneratedCounter = 0;
            StringBuilder builder = new StringBuilder();
            try
            {
                Avanza.CCMS.DAL.CashOrders cashOrders = null;
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trxn = conn.BeginTransaction();

                foreach (GridViewRow row in GridView_CashPositions.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("CheckBox_Select");
                    if (chk.Checked)
                    {
                        string atm_id = this.GridView_CashPositions.DataKeys[row.DataItemIndex].Values["atm_id"].ToString();
                        Atm atm = Atm.LoadAtmByPk(int.Parse(atm_id));

                        TextBox type1 = (TextBox)row.FindControl("TextBox_Type1");
                        TextBox type2 = (TextBox)row.FindControl("TextBox_Type2");
                        TextBox type3 = (TextBox)row.FindControl("TextBox_Type3");
                        TextBox type4 = (TextBox)row.FindControl("TextBox_Type4");

                        TextBox orderDate = (TextBox)row.FindControl("TextBox_OrderDate");
                        TextBox repDate = (TextBox)row.FindControl("TextBox_ReplenishmentDate");

                        cashOrders = Avanza.CCMS.DAL.CashOrders.LoadCashOrders("atm_id = " + atm_id + " and replenishment_datetime = convert(datetime,'" + orderDate.Text + "',103) and is_cancelled=0");
                        if (cashOrders != null)
                        {
                            string temp = string.Format("Cash order already exists for {0} for the day {1} ", atm.Title, orderDate.Text);
                            builder.Append(temp + "<br/>");
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, temp);
                            continue;
                        }
                        else
                            cashOrders = new Avanza.CCMS.DAL.CashOrders();
                        //cashOrders.CashOrderDatetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(atm.NoteSetTypeId);
                        cashOrders.AtmId = atm.ATMId;
                        cashOrders.IsUploaded = false;
                        cashOrders.IsCancelled = false;
                        cashOrders.IsHold = false;
                        cashOrders.CreationTime = DateTime.Now;
                        cashOrders.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

                        AppSetting appSettings = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]);
                        cashOrders.DispatchTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, appSettings.CashOrderExecutionTime.Hour, appSettings.CashOrderExecutionTime.Minute, appSettings.CashOrderExecutionTime.Second);

                        cashOrders.ReplenishmentDatetime = DateTime.ParseExact(repDate.Text, "dd/MM/yyyy HH:mm:ss", null);
                        cashOrders.CashOrderDatetime = DateTime.ParseExact(orderDate.Text, "dd/MM/yyyy HH:mm:ss", null);
                        cashOrders.Cassette1Denomination = noteSetType.DenominationType1.Value;
                        cashOrders.Cassette2Denomination = noteSetType.DenominationType2.Value;
                        cashOrders.Cassette3Denomination = noteSetType.DenominationType3.Value;
                        cashOrders.Cassette4Denomination = noteSetType.DenominationType4.Value;
                        cashOrders.Cassette5Denomination = noteSetType.DenominationType5.Value;
                        cashOrders.Cassette6Denomination = noteSetType.DenominationType6.Value;
                        cashOrders.Cassette7Denomination = noteSetType.DenominationType7.Value;

                        if (type1.Text.Length > 0)
                            cashOrders.Cassette1SuggestedNotes = int.Parse(type1.Text);
                        else
                            cashOrders.Cassette1SuggestedNotes = 0;


                        if (type2.Text.Length > 0)
                            cashOrders.Cassette2SuggestedNotes = int.Parse(type2.Text);
                        else
                            cashOrders.Cassette2SuggestedNotes = 0;


                        if (type3.Text.Length > 0)
                            cashOrders.Cassette3SuggestedNotes = int.Parse(type3.Text);
                        else
                            cashOrders.Cassette3SuggestedNotes = 0;

                        if (type4.Text.Length > 0)
                            cashOrders.Cassette4SuggestedNotes = int.Parse(type4.Text);
                        else
                            cashOrders.Cassette4SuggestedNotes = 0;
                        cashOrders.Cassette5SuggestedNotes = 0;
                        cashOrders.Cassette6SuggestedNotes = 0;
                        cashOrders.Cassette7SuggestedNotes = 0;



                        cashOrders.CashOrderType = "RS";
                        cashOrders.OrderNumber = cashOrders.CashOrderType + "-" + atm.Title + "-" + cashOrders.CashOrderDatetime.ToString("ddMMyy");
                        cashOrders.Cassette1RemainingNotes = cashOrders.Cassette1SuggestedNotes;
                        cashOrders.Cassette2RemainingNotes = cashOrders.Cassette2SuggestedNotes;
                        cashOrders.Cassette3RemainingNotes = cashOrders.Cassette3SuggestedNotes;
                        cashOrders.Cassette4RemainingNotes = cashOrders.Cassette4SuggestedNotes;
                        cashOrders.Cassette5RemainingNotes = cashOrders.Cassette5SuggestedNotes;
                        cashOrders.Cassette6RemainingNotes = cashOrders.Cassette6SuggestedNotes;
                        cashOrders.Cassette7RemainingNotes = cashOrders.Cassette7SuggestedNotes;

                        cashOrders.Save(trxn.Connection, trxn);
                        orderGeneratedCounter++;
                        //Task cashOrderUploadTask = null;

                        Avanza.CCMS.DAL.CashOrderMonitoring cashOrderMonitoring = new Avanza.CCMS.DAL.CashOrderMonitoring();
                        cashOrderMonitoring.CashOrderDatetime = cashOrders.CashOrderDatetime;
                        cashOrderMonitoring.ReplenishmentDatetime = cashOrders.ReplenishmentDatetime;
                        cashOrderMonitoring.AtmId = atm.ATMId;
                        cashOrderMonitoring.CurrentOrderReceivedAt = DateTime.Now;
                        cashOrderMonitoring.CurrentOrderId = cashOrders.CashOrderId;

                        cashOrderMonitoring.CurrentOrderRemainingAmount = noteSetType.DenominationType1.Value * cashOrders.Cassette1SuggestedNotes.Value
                                    + noteSetType.DenominationType2.Value * cashOrders.Cassette2SuggestedNotes.Value
                                  + noteSetType.DenominationType3.Value * cashOrders.Cassette3SuggestedNotes.Value
                                    + noteSetType.DenominationType4.Value * cashOrders.Cassette4SuggestedNotes.Value
                                     + noteSetType.DenominationType5.Value * cashOrders.Cassette5SuggestedNotes.Value
                                      + noteSetType.DenominationType6.Value * cashOrders.Cassette6SuggestedNotes.Value
                                        + noteSetType.DenominationType7.Value * cashOrders.Cassette7SuggestedNotes.Value;

                        cashOrderMonitoring.CurrentOrderSuggestedAmount = cashOrderMonitoring.CurrentOrderRemainingAmount;
                        cashOrderMonitoring.Save(trxn.Connection, trxn);

                        VaultAtm vaultAtm = VaultAtm.LoadVaultAtm("atm_id = " + cashOrders.AtmId);
                        if (vaultAtm != null)
                        {
                            VaultInfo vaultInfo = null;
                            if (Request.QueryString["cash_order_id"] != null)
                            {
                                CcmsTempVaultLedger tempVaultLedger = CcmsTempVaultLedger.LoadCcmsTempVaultLedger("order_id = " + cashOrders.CashOrderId);
                                if (tempVaultLedger != null)
                                {
                                    vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, tempVaultLedger.LedgerAmount, cashOrders.AtmId, "Cr", "Vault adjusted because order cancelled.Order ref # " + cashOrders.OrderNumber, "Adjustment Credit", -1, trxn);
                                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                                    cmd.CommandText = "select quantity,vault_note_type_id from ccms_temp_vault_ledger_detail where ledger_id = " + tempVaultLedger.Id;
                                    System.Data.DataTable dt = new System.Data.DataTable();
                                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                    adapter.Fill(dt);

                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        int Quantity = int.Parse(dr["quantity"].ToString());
                                        CreateTempVaultLedgerDetail(vaultInfo, Quantity, int.Parse(dr["vault_note_type_id"].ToString()), "Cr", true, trxn);
                                    }
                                    ConnectionFactory.ExecuteQuery("delete general_alert where entity_id = " + tempVaultLedger.Id, trxn);
                                }
                            }

                            vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, cashOrderMonitoring.CurrentOrderRemainingAmount, cashOrders.AtmId, "Dr", "Vault deducted with amount " + cashOrderMonitoring.CurrentOrderRemainingAmount + " order ref # " + cashOrders.OrderNumber, "Outbound", cashOrders.CashOrderId, trxn);

                            if (cashOrders.Cassette1SuggestedNotes > 0)
                            {
                                CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType1Title + "'");
                                if (vaultNoteType != null)
                                    CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette1SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                            }
                            if (cashOrders.Cassette2SuggestedNotes > 0)
                            {
                                CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType2Title + "'");
                                if (vaultNoteType != null)
                                    CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette2SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                            }
                            if (cashOrders.Cassette3SuggestedNotes > 0)
                            {
                                CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType3Title + "'");
                                if (vaultNoteType != null)
                                    CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette3SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                            }
                            if (cashOrders.Cassette4SuggestedNotes > 0)
                            {
                                CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType4Title + "'");
                                if (vaultNoteType != null)
                                    CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette4SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                            }
                            if (cashOrders.Cassette5SuggestedNotes > 0)
                            {
                                CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType5Title + "'");
                                if (vaultNoteType != null)
                                    CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette5SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                            }

                            if (cashOrders.Cassette6SuggestedNotes > 0)
                            {
                                CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType6Title + "'");
                                if (vaultNoteType != null)
                                    CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette6SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                            }
                            if (cashOrders.Cassette7SuggestedNotes > 0)
                            {
                                CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteType("vault_id = " + vaultAtm.VaultId + " and denomination_name='" + noteSetType.DenominationType7Title + "'");
                                if (vaultNoteType != null)
                                    CreateTempVaultLedgerDetail(vaultInfo, cashOrders.Cassette7SuggestedNotes.Value, vaultNoteType.Id, "Dr", false, trxn);
                            }





                        }
                        else
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Vault is not defined for the atm =" + cashOrders.AtmId);


                        Utility.BuildAuditLog(string.Format("Cash order created for atm {0} for the day {1}", atm.Title, cashOrders.CashOrderDatetime.ToString("dd/MM/yyyy")), (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateCashOrders);

                    }

                }
                trxn.Commit();
                if (orderGeneratedCounter > 0)
                {
                    Response.Redirect("CashOrders.aspx");
                    Session[SessionVars.notice.ToString()] = "Orders successfully generated";
                }
                else
                    Session[SessionVars.notice.ToString()] = builder.ToString();
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                if (trxn != null)
                    trxn.Rollback();
            }
            finally
            {
                task.EndTask();
            }

        }
    }

}