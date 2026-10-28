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

using System.Diagnostics;
using System.Reflection;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Xml;
using System.Text;



namespace CCMSUI.CCMS
{
    public partial class CashOrders : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                HyperLink_CreateCashOrders.Visible = CheckRight((int)Permissions.CreateCashOrders);
                if (!CheckRight((int)Permissions.ModifyCashOrders))
                    GridView_CashOrders.Columns[23].Visible = false;

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();


                BindGrid("title asc");

            }
        }

        string Filter()
        {
            string filter = "";
            //Changed to creation time instead of cash_order_datetime
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and c.creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and c.creation_time<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            if (DropDownList_creator.SelectedIndex > 0)
                filter += " and c.created_by = " + DropDownList_creator.SelectedValue;

            if (DropDownList_repType.SelectedIndex > 0)
                filter += " and cash_order_type= '" + DropDownList_repType.SelectedValue + "'";

            if (DropDownList_OrderStatus.SelectedIndex == 1)
                filter += " and is_cancelled= 1 ";

            else if (DropDownList_OrderStatus.SelectedIndex == 2)
                filter += " and is_cancelled= 0 ";


            return filter;
        }
        private void BindGrid(string orderBy)
        {
            try
            {
                ///<Summary>Edited by Ali Shah on 10th Oct, 2019
                ///To dynamically handling of cassettes more than 4
                EnableGridRespectiveColumns();

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select c.order_number,c.cassette1_denomination,c.cassette2_denomination,
                                    c.cassette3_denomination,c.cassette4_denomination,c.cassette5_denomination,c.cassette6_denomination,
                                    c.cassette7_denomination,title, ip , cash_order_id,cash_order_type,cash_order_datetime,
                                    cassette1_suggested_notes,cassette2_suggested_notes,cassette3_suggested_notes,
                                    cassette4_suggested_notes,cassette5_suggested_notes,cassette6_suggested_notes,cassette7_suggested_notes,
                                    c.cassette1_denomination*cassette1_suggested_notes+
                                    c.cassette2_denomination*cassette2_suggested_notes+
                                    c.cassette3_denomination*cassette3_suggested_notes+
                                    c.cassette4_denomination*cassette4_suggested_notes+
                                    c.cassette5_denomination*cassette5_suggested_notes+
                                    c.cassette6_denomination*cassette6_suggested_notes+
                                    c.cassette7_denomination*cassette7_suggested_notes sum,c.creation_time,c.replenishment_datetime,c.is_hold,cit_atm_title,gl_number,location,city,
                                    dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4, cassette1_suggested_notes , cassette2_suggested_notes, cassette3_suggested_notes, cassette4_suggested_notes) sum_text
                                      from cash_orders c, atm, app_user, note_set_type where app_user.user_id = c.created_by and c.atm_id = atm.atm_id and note_set_type.note_set_type_id = atm.note_set_type_id 
                                     and atm.atm_id in ("
                    + Session[SessionVars.SelectedATMs.ToString()] + ") " + Filter() + " order by " + orderBy;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                System.Data.DataTable dtCashOrders = new System.Data.DataTable();
                adapter.Fill(dtCashOrders);
                Cache["dtCashOrders"] = dtCashOrders;
                this.GridView_CashOrders.DataSource = dtCashOrders;
                this.GridView_CashOrders.DataBind();
                Literal_count.Text = dtCashOrders.Rows.Count + " records.";

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("bind cash order grid", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

        public SortDirection GridViewSortDirection
        {
            get
            {



                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;

                //return (SortDirection)ViewState["sortDirection"];
                return (ViewState["sortDirection"].ToString() == " asc" ? SortDirection.Ascending : SortDirection.Descending);
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected void GridView_CashOrders_Sorting(object sender, GridViewSortEventArgs e)
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

            //System.Data.DataTable dt = (System.Data.DataTable)Cache[CacheVars.dtCashOrders.ToString()];
            //DataView dv = new DataView(dt);
            //GridViewSortDirection = (GridViewSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
            //dv.Sort = e.SortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            //GridView_CashOrders.DataSource = dv;
            //GridView_CashOrders.DataBind();

        }




        protected void GridView_CashOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_CashOrders.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            //System.Data.DataTable dt = (System.Data.DataTable)Cache[CacheVars.dtCashOrders.ToString()];
            //DataView dv = new DataView(dt);
            //if (GridViewSortExpression != "")
            //    dv.Sort = GridViewSortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            //this.GridView_CashOrders.DataSource = dv;

            //this.GridView_CashOrders.PageIndex = e.NewPageIndex;
            //this.GridView_CashOrders.DataBind();

        }
        private string GridViewSortExpression
        {
            get
            { return ViewState["SortExpression"] as string ?? string.Empty; }
            set
            { ViewState["SortExpression"] = value; }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }


        protected void GridView_CashOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            //holdCashOrder

            if (e.CommandName == "holdCashOrder")
            {
                // SqlCommand cmd = null;
                try
                {
                    task = LogableTask.NewTask("hold cash order");
                    // cmd = ConnectionFactory.GetNewCommand(true);

                    Avanza.CCMS.DAL.CashOrders cashOrder = Avanza.CCMS.DAL.CashOrders.LoadCashOrdersByPk(int.Parse(e.CommandArgument.ToString()));
                    if (cashOrder != null)
                    {
                        cashOrder.IsHold = true;
                        cashOrder.Save();
                        Session[SessionVars.notice.ToString()] = "Selected order put on hold!";
                    }
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "order hold");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while holding cash order. " + ex.Message + "</li>";

                }
                finally
                {
                    //  cmd.Connection.Close();
                    task.EndTask();
                }
            }
            else if (e.CommandName == "cancelCashOrder")
            {
                SqlCommand cmd = null;
                SqlTransaction trx = null;
                try
                {
                    task = LogableTask.NewTask("cancel cash order");
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trx = cmd.Connection.BeginTransaction();
                    cmd.Transaction = trx;

                    Avanza.CCMS.DAL.CashOrders cashOrder = Avanza.CCMS.DAL.CashOrders.LoadCashOrdersByPk(int.Parse(e.CommandArgument.ToString()));
                    if (cashOrder != null)
                    {
                        cashOrder.IsCancelled = true;
                        cashOrder.Save(trx.Connection, trx);

                        Task existingTask = Task.LoadTask("cash_order_id=" + cashOrder.CashOrderId);
                        if (existingTask != null)
                        {
                            existingTask.Status = UploadStates.cancelledAnotherOrderPosted.ToString();
                            existingTask.Save(trx.Connection, trx);
                        }
                        VaultAtm vaultAtm = VaultAtm.LoadVaultAtm("atm_id = " + cashOrder.AtmId);
                        if (vaultAtm != null)
                        {
                            CcmsTempVaultLedger tempVaultLedger = CcmsTempVaultLedger.LoadCcmsTempVaultLedger("order_id = " + cashOrder.CashOrderId);
                            if (tempVaultLedger != null)
                            {
                                VaultInfo vaultInfo = CreateTempVaultLedger(vaultAtm.VaultId, tempVaultLedger.LedgerAmount, cashOrder.AtmId, "Cr", "Vault Adjusted because order cancelled.Order ref # " + cashOrder.OrderNumber, "Adjustment Credit", -1, trx);
                                //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                                cmd.CommandText = "select quantity,vault_note_type_id from ccms_temp_vault_ledger_detail where ledger_id = " + tempVaultLedger.Id;
                                System.Data.DataTable dt = new System.Data.DataTable();
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                adapter.Fill(dt);

                                foreach (DataRow dr in dt.Rows)
                                {
                                    int Quantity = int.Parse(dr["quantity"].ToString());
                                    CreateTempVaultLedgerDetail(vaultInfo, Quantity, int.Parse(dr["vault_note_type_id"].ToString()), "Cr", true, trx);
                                }
                                ConnectionFactory.ExecuteQuery("delete general_alert where entity_id = " + tempVaultLedger.Id, trx);
                            }
                        }
                        trx.Commit();
                        if (cashOrder != null)
                        {
                            Utility.BuildAuditLog(cashOrder.OrderNumber + " cancelled", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.OrderCancelled);

                            //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                            Session[SessionVars.notice.ToString()] = "Selected order cancelled!";
                        }

                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "order cancelled");
                        BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while cancelling cash order. " + ex.Message + "</li>";
                    trx.Rollback();
                }
                finally
                {
                    cmd.Connection.Close();
                    task.EndTask();

                }
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

        protected void GridView_CashOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int id = int.Parse(GridView_CashOrders.DataKeys[e.Row.RowIndex].Values["cash_order_id"].ToString());
                Avanza.CCMS.DAL.CashOrders cashOrder = Avanza.CCMS.DAL.CashOrders.LoadCashOrdersByPk(id);
                if (cashOrder.IsCancelled)
                {
                    ((HyperLink)e.Row.FindControl("HyperLink_EditCashOrders")).Visible = false;
                    ((ImageButton)e.Row.FindControl("Button_CancelCashOrder")).Visible = false;

                }

                //Task existingTask = Task.LoadTask("cash_order_id=" + id);
                //if (existingTask != null)
                //{
                //    if (existingTask.Status != DownloadStates.scheduled.ToString())
                //    {
                //        ((HyperLink)e.Row.FindControl("HyperLink_EditCashOrders")).Visible = false;
                //        ((ImageButton)e.Row.FindControl("Button_CancelCashOrder")).Visible = false;

                //    }

                //}
                else
                    if (e.Row.FindControl("Button_CancelCashOrder") != null)
                        ((ImageButton)e.Row.FindControl("Button_CancelCashOrder")).Attributes.Add("onClick",
                            "return confirm('Are you sure you want to cancel order for ATM " + ((DataRowView)e.Row.DataItem).Row["Title"] + "?','CCMS')");
            }
        }

        private void EnableGridRespectiveColumns()
        {
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexDenomination = -1;
            //int columnIndexDispensed = -1;
            int columnIndexSuggestion = -1;
            columnIndexDenomination = GetColumnIndexByName(this.GridView_CashOrders, "Den 5");
            //columnIndexDispensed = GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            columnIndexSuggestion = GetColumnIndexByName(this.GridView_CashOrders, "Sugg 5");
            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexDenomination >= 0)
                {
                    this.GridView_CashOrders.Columns[columnIndexDenomination].Visible = true;
                    columnIndexDenomination += 2;
                }

                //if (columnIndexDispensed >= 0)
                //{
                //    this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
                //    columnIndexDispensed++;
                //}
                if (columnIndexSuggestion >= 0)
                {
                    this.GridView_CashOrders.Columns[columnIndexSuggestion].Visible = true;
                    columnIndexSuggestion += 2;
                }
            }
        }

        //private string GenerateSheetHeaderAndDefaultSheetData(DateTime orderDate)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        //    builder.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");
        //    builder.Append("    <dimension ref=\"A1:L40\"/>");
        //    builder.Append("    <sheetViews>");
        //    builder.Append("        <sheetView tabSelected=\"1\" view=\"pageBreakPreview\" topLeftCell=\"A2\" zoomScale=\"60\" zoomScaleNormal=\"100\" workbookViewId=\"0\">");
        //    builder.Append("		            <selection activeCell=\"K38\" sqref=\"K38\"/>");
        //    builder.Append("        </sheetView>");
        //    builder.Append("    </sheetViews>");
        //    builder.Append("    <sheetFormatPr defaultRowHeight=\"13.5\"/>");
        //    builder.Append("    <cols>");
        //    builder.Append("    <col min=\"1\" max=\"1\" width=\"7.7109375\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("    <col min=\"2\" max=\"2\" width=\"9.140625\" style=\"140\"/>");

        //    builder.Append("            <col min=\"3\" max=\"3\" width=\"11.140625\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("    <col min=\"4\" max=\"4\" width=\"21.7109375\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("             <col min=\"5\" max=\"5\" width=\"11.7109375\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("            <col min=\"6\" max=\"6\" width=\"10.85546875\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("            <col min=\"7\" max=\"7\" width=\"14.28515625\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("            <col min=\"8\" max=\"8\" width=\"10.5703125\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("            <col min=\"9\" max=\"9\" width=\"14\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("           <col min=\"10\" max=\"10\" width=\"14.140625\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("           <col min=\"11\" max=\"11\" width=\"21.7109375\" style=\"140\" customWidth=\"1\"/>");
        //    builder.Append("           <col min=\"12\" max=\"16384\" width=\"9.140625\" style=\"140\"/>");
        //    builder.Append("       </cols>");
        //    builder.Append("       <sheetData>");
        //    builder.Append("           <row r=\"1\" spans=\"1:12\">");
        //    builder.Append("              <c r=\"K1\" s=\"141\"/>");
        //    builder.Append("          </row>");
        //    builder.Append("          <row r=\"2\" spans=\"1:12\">");
        //    builder.Append("              <c r=\"F2\" s=\"142\" t=\"s\">");
        //    builder.Append("                <v>7</v>");
        //    builder.Append("              </c><c r=\"G2\" s=\"143\"/>");
        //    builder.Append("              <c r=\"H2\" s=\"143\"/>");
        //    builder.Append("             <c r=\"I2\" s=\"144\"/>");
        //    builder.Append("              <c r=\"J2\" s=\"145\" t=\"s\">");
        //    builder.Append("                <v>0</v>");
        //    builder.Append("              </c><c r=\"K2\" s=\"174\">");
        //    builder.Append("                <v>" + orderDate.ToOADate() + "</v>");
        //    builder.Append("              </c>");
        //    builder.Append("          </row>");
        //    builder.Append("          <row r=\"3\" spans=\"1:12\">");
        //    builder.Append("              <c r=\"I3\" s=\"147\"/>");
        //    builder.Append("              <c r=\"J3\" s=\"147\" t=\"s\">");
        //    builder.Append("                <v>1</v>");
        //    builder.Append("              </c>");

        //    builder.Append("              <c r=\"K3\" s=\"175\" t=\"inlineStr\">");
        //    builder.Append("                 <is>");
        //    builder.Append("                     <t>" + orderDate.DayOfWeek + "</t>");
        //    builder.Append("                  </is>");
        //    builder.Append("             </c>");
        //    builder.Append("          </row>");
        //    builder.Append("          <row r=\"4\" spans=\"1:12\">");
        //    builder.Append("              <c r=\"J4\" s=\"148\"/>");
        //    builder.Append("          </row>");
        //    builder.Append("          <row r=\"5\" spans=\"1:12\">");
        //    builder.Append("              <c r=\"A5\" s=\"141\"/>");
        //    builder.Append("              <c r=\"F5\" s=\"149\" t=\"s\">");
        //    builder.Append("                <v>5</v>");
        //    builder.Append("             </c><c r=\"G5\" s=\"150\"/>");
        //    builder.Append("              <c r=\"J5\" s=\"151\"/>");
        //    builder.Append("              <c r=\"K5\" s=\"152\"/>");
        //    builder.Append("              <c r=\"L5\" s=\"141\"/>");
        //    builder.Append("          </row>");
        //    builder.Append("          <row r=\"6\" spans=\"1:12\">");
        //    builder.Append("              <c r=\"A6\" s=\"153\" t=\"s\">");
        //    builder.Append("                <v>90</v>");
        //    builder.Append("             </c><c r=\"B6\" s=\"154\" t=\"s\">");
        //    builder.Append("                <v>2</v>");
        //    builder.Append("             </c><c r=\"C6\" s=\"154\" t=\"s\">");
        //    builder.Append("                <v>3</v>");
        //    builder.Append("             </c><c r=\"D6\" s=\"154\" t=\"s\">");
        //    builder.Append("                <v>4</v>");
        //    builder.Append("            </c><c r=\"E6\" s=\"150\">");
        //    builder.Append("                <v>1000</v>");
        //    builder.Append("            </c><c r=\"F6\" s=\"154\">");
        //    builder.Append("               <v>500</v>");
        //    builder.Append("           </c><c r=\"G6\" s=\"154\">");
        //    builder.Append("               <v>200</v>");
        //    builder.Append("          </c><c r=\"H6\" s=\"145\">");
        //    builder.Append("              <v>100</v>");
        //    builder.Append("          </c><c r=\"I6\" s=\"150\" t=\"s\">");
        //    builder.Append("              <v>6</v>");
        //    builder.Append("          </c><c r=\"J6\" s=\"144\" t=\"s\">");
        //    builder.Append("              <v>24</v>");
        //    builder.Append("          </c><c r=\"K6\" s=\"146\" t=\"s\">");
        //    builder.Append("              <v>301</v>");
        //    builder.Append("          </c><c r=\"L6\" s=\"145\" t=\"s\">");
        //    builder.Append("               <v>423</v>");
        //    builder.Append("           </c>");
        //    builder.Append("       </row>");
        //    return builder.ToString();
        //}
        //private string GenerateFormulaRowAndSheetEndTag(int idx)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(string.Format("<row r=\"{0}\" spans=\"1:12\" s=\"160\" customFormat=\"1\">", idx, idx - 1));
        //    builder.Append(string.Format("<c r=\"E{0}\" s=\"176\"><f>SUM(E7:E{1})</f><v>25100000</v></c>", idx, idx - 1));
        //    builder.Append(string.Format("<c r=\"F{0}\" s=\"176\"><f>SUM(F7:F{1})</f><v>25100000</v></c>", idx, idx - 1));
        //    builder.Append(string.Format("<c r=\"G{0}\" s=\"176\"><f>SUM(G7:G{1})</f><v>25100000</v></c>", idx, idx - 1));
        //    builder.Append(string.Format("<c r=\"H{0}\" s=\"176\"><f>SUM(H7:H{1})</f><v>25100000</v></c>", idx, idx - 1));
        //    builder.Append(string.Format("<c r=\"I{0}\" s=\"176\"><f>SUM(I7:I{1})</f><v>25100000</v></c>", idx, idx - 1));
        //    builder.Append("</row>");
        //    builder.Append(string.Format("<row r=\"{0}\" spans=\"1:12\" s=\"160\" customFormat=\"1\"/>", idx + 1));
        //    builder.Append(string.Format("<row r=\"{0}\" spans=\"1:12\" s=\"160\" customFormat=\"1\"/>", idx + 2));
        //    builder.Append("</sheetData>");
        //    return builder.ToString();
        //}
        //private string GenerateFooter()
        //{
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append("<sortState ref=\"A7:M422\">");
        //    builder.Append("                <sortCondition ref=\"B7\"/>");
        //    builder.Append("            </sortState>");
        //    builder.Append("<pageMargins left=\"0.75\" right=\"0.75\" top=\"1\" bottom=\"1\" header=\"0.5\" footer=\"0.5\"/>");
        //    builder.Append("<pageSetup scale=\"79\" orientation=\"landscape\" horizontalDpi=\"4294967293\" r:id=\"rId1\"/>");
        //    builder.Append("<headerFooter alignWithMargins=\"0\"/>");
        //    builder.Append("<drawing r:id=\"rId2\"/>");
        //    builder.Append("</worksheet>");
        //    return builder.ToString();
        //}
        //public void UpdateSheet3(string filePath, DateTime orderDate, System.Data.DataTable dt)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    string headerWithDefaultRow = GenerateSheetHeaderAndDefaultSheetData(orderDate);
        //    string footer = GenerateFooter();
        //    int idx = 7;
        //    foreach (DataRow dr in dt.Rows)
        //    {//c.cassette1_denomination,c.cassette1_suggested_notes
                
                 
                
        //        builder.Append(string.Format("<row r=\"{0}\" spans=\"1:12\" s=\"160\" customFormat=\"1\">", idx));
        //        builder.Append(string.Format("<c r=\"A{0}\" s=\"155\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["cit_atm_title"]));
        //        builder.Append(string.Format("<c r=\"B{0}\" s=\"156\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["title"]));
        //        builder.Append(string.Format("<c r=\"C{0}\" s=\"156\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["gl_number"]));
        //        builder.Append(string.Format("<c r=\"D{0}\" s=\"156\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["location"]));
        //        builder.Append(string.Format("<c r=\"E{0}\" s=\"157\"><v>{1}</v></c>", idx, int.Parse(dr["cassette1_denomination"].ToString()) * int.Parse(dr["cassette1_suggested_notes"].ToString())));
        //        builder.Append(string.Format("<c r=\"F{0}\" s=\"157\"><v>{1}</v></c>", idx, int.Parse(dr["cassette2_denomination"].ToString()) * int.Parse(dr["cassette2_suggested_notes"].ToString())));
        //        builder.Append(string.Format("<c r=\"G{0}\" s=\"157\"><v>{1}</v></c>", idx, int.Parse(dr["cassette3_denomination"].ToString()) * int.Parse(dr["cassette3_suggested_notes"].ToString())));
        //        builder.Append(string.Format("<c r=\"H{0}\" s=\"157\"><v>{1}</v></c>", idx, int.Parse(dr["cassette4_denomination"].ToString()) * int.Parse(dr["cassette4_suggested_notes"].ToString())));
        //        builder.Append(string.Format("<c r=\"I{0}\" s=\"158\"><f>SUM(E{0}:H{0})</f><v>2600000</v></c>", idx));
        //        builder.Append(string.Format("<c r=\"J{0}\" s=\"159\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["city"]));

        //        if (dr["cash_order_datetime"].ToString() != dr["replenishment_datetime"].ToString())
        //        {
        //            builder.Append(string.Format("<c r=\"K{0}\" s=\"156\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, DateTime.Parse(dr["replenishment_datetime"].ToString()).ToString("dd-MMM")));
        //            builder.Append(string.Format("<c r=\"L{0}\" s=\"156\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, DateTime.Parse(dr["replenishment_datetime"].ToString()).ToString("HH:mm")));
        //        }
        //        else
        //        {
        //            builder.Append(string.Format("<c r=\"K{0}\" s=\"156\"/>", idx));
        //            builder.Append(string.Format("<c r=\"L{0}\" s=\"156\"/>", idx));
        //        }

                
        //        builder.Append("</row>");
        //        idx++;
        //    }
        //    File.WriteAllText(filePath, headerWithDefaultRow + builder.ToString() + GenerateFormulaRowAndSheetEndTag(idx)+ footer);
        //}

        private void ExportCashOrder()
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select c.order_number,c.cassette1_denomination,c.cassette2_denomination,
                                    c.cassette3_denomination,c.cassette4_denomination,c.cassette5_denomination,c.cassette6_denomination,
                                    c.cassette7_denomination,title, ip , cash_order_id,cash_order_type,cash_order_datetime,
                                    cassette1_suggested_notes,cassette2_suggested_notes,cassette3_suggested_notes,
                                    cassette4_suggested_notes,cassette5_suggested_notes,cassette6_suggested_notes,cassette7_suggested_notes,
                                    c.cassette1_denomination*cassette1_suggested_notes+
                                    c.cassette2_denomination*cassette2_suggested_notes+
                                    c.cassette3_denomination*cassette3_suggested_notes+
                                    c.cassette4_denomination*cassette4_suggested_notes+
                                    c.cassette5_denomination*cassette5_suggested_notes+
                                    c.cassette6_denomination*cassette6_suggested_notes+
                                    c.cassette7_denomination*cassette7_suggested_notes sum,c.creation_time,c.replenishment_datetime,c.is_hold,cit_atm_title,gl_number,location,city,
                                    dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4, cassette1_suggested_notes , cassette2_suggested_notes, cassette3_suggested_notes, cassette4_suggested_notes) sum_text
                                      from cash_orders c, atm, app_user, note_set_type where app_user.user_id = c.created_by and c.atm_id = atm.atm_id and atm.note_set_type_id = note_set_type.note_set_type_id and c.is_cancelled=0  
                                     and atm.atm_id in ("
                + Session[SessionVars.SelectedATMs.ToString()] + ") " + Filter();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);
                


     //       System.Data.DataTable dt = (System.Data.DataTable)Cache["dtCashOrders"];
            if (dt.Rows.Count > 0)
            {
                AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                string zipFilePath = appSetting.TemporaryFolder + "\\Template.zip";
                if (!File.Exists(zipFilePath))
                    throw new Exception("Template file does not exists.Please upload template file before generating settlement report");
                string extractFilePath = appSetting.TemporaryFolder + "\\Template1";

                if (Directory.Exists(extractFilePath))
                    Directory.Delete(extractFilePath, true);

                if (!Directory.Exists(extractFilePath))
                    Directory.CreateDirectory(extractFilePath);
                // Change Sheet Utility to Utility

                SheetUtility.ExpandFolder(zipFilePath, extractFilePath);

                var sheetName = "sheet3";
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CashReportSheetName"]))
                    sheetName = ConfigurationManager.AppSettings["CashReportSheetName"];


                SheetUtility.UpdateSheet3(extractFilePath + "\\xl\\worksheets\\"+sheetName+".xml", DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null), dt);

                if (File.Exists(extractFilePath + "\\xl\\calcChain.xml"))
                    File.Delete(extractFilePath + "\\xl\\calcChain.xml");

                SheetUtility.UpdateWorkSheet(extractFilePath + "\\xl\\workbook.xml");
                SheetUtility.ZipFolder(extractFilePath);
                if (File.Exists(extractFilePath + ".xlsx"))
                    File.Delete(extractFilePath + ".xlsx");
                File.Move(extractFilePath + ".zip", extractFilePath + ".xlsx");

                Response.Clear();
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment; filename=CashOrders.xlsx");
                Response.AddHeader("Content-Length",new FileInfo(extractFilePath + ".xlsx").Length.ToString());
                Response.TransmitFile(extractFilePath + ".xlsx");
                Response.Flush();
                Response.Close();
            }
        }

        protected void Button_ExportOrder_Click(object sender, EventArgs e)
        {
            ExportCashOrder();
        }
        protected void Button_ExportCashOrder_Click(object sender, EventArgs e)
        {


            //WindowsIdentity winId = (WindowsIdentity)HttpContext.Current.User.Identity;
            //WindowsImpersonationContext ctx = null;
            try
            {
                if (Session[SessionVars.SelectedRegion.ToString()] == null)
                    throw new Exception("Please select organization from tree");

                else if (Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(Session[SessionVars.SelectedRegion.ToString()].ToString())).RegionId == 1)
                    throw new Exception("Please select only one organization from tree");
                // Start impersonating
                //ctx = winId.Impersonate();
                CreateExcelWorksheet f = new CreateExcelWorksheet();
                f.DoWork((string)Session[SessionVars.SelectedATMs.ToString()],
                    int.Parse(Session[SessionVars.SelectedRegion.ToString()].ToString()), GetFilter(), ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder);
                string filePath = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\copy.xls";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));

                Response.TransmitFile(filePath);
                Response.Flush();
                Response.Close();


                // Now impersonating
                // Access resources using the identity of the authenticated user
            }
            // Prevent exceptions from propagating
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                LogableTask.LogMonoActivityTask("ExportCashOrders", MethodBase.GetCurrentMethod(),
                        TraceLevel.Error, ex);



            }
            finally
            {
                //ctx = WindowsIdentity.Impersonate(IntPtr.Zero);

                // //Revert impersonation
                //if (ctx != null)
                //    ctx.Undo();



            }



            //f.xlApp.Save(@"C:\myfile.xlsx");
            //            SqlConnection conn = ConnectionFactory.GetNewConnection();
            //            using (SqlCommand cmd = conn.CreateCommand())
            //            {
            //                string SQL = @"select bank_cash_center_id,depot_id,title, location,address1,city,
            //cash_order_datetime, c.cassette1_denomination,c.cassette1_suggested_notes,
            //c.cassette2_denomination,c.cassette2_suggested_notes,
            //c.cassette3_denomination,c.cassette3_suggested_notes,
            //c.cassette4_denomination,c.cassette4_suggested_notes,
            //c.cassette5_denomination,c.cassette5_suggested_notes,
            //c.cassette6_denomination,c.cassette6_suggested_notes,
            //c.cassette7_denomination,c.cassette7_suggested_notes, 
            //
            //c.cassette1_denomination*c.cassette1_suggested_notes+
            //c.cassette2_denomination*c.cassette2_suggested_notes+
            //c.cassette3_denomination*c.cassette3_suggested_notes+
            //c.cassette4_denomination*c.cassette4_suggested_notes+
            //c.cassette5_denomination*c.cassette5_suggested_notes+
            //c.cassette6_denomination*c.cassette6_suggested_notes+
            //c.cassette7_denomination*c.cassette7_suggested_notes total
            //
            //from atm, cash_orders c
            //where c.atm_id = atm.atm_id";

            //                cmd.CommandText = SQL;

            //                CCMSUI.CCMS.dsExportCashorder Ds = new CCMSUI.CCMS.dsExportCashorder();
            //                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //                adapter.Fill(Ds.Tables["DataTable1"]);

            //                if (Ds.Tables[0].Rows.Count > 0)
            //                {
            //                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;
            //                    string URL = "ReportPopup.aspx?Report=ExportCashOrder.rpt&RptName=Cash Order Report&bypass=1&total=" + Ds.Tables[0].Rows.Count;

            //                    //  Literal_ok.Text = "<script> try { window.open(\"" + URL + "\"); } catch { alert('Popup cannot be opened'); }</script>";
            //                    Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
            //                }
            //                else
            //                    Literal_ok.Text = "<li>No record(s) found</li>";


            //            }



        }
        string GetFilter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and c.cash_order_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
            else
                filter += " and c.cash_order_datetime >= getdate() - 3";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and c.cash_order_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";





            if (DropDownList_creator.SelectedIndex > 0)
                filter += " and c.created_by = " + DropDownList_creator.SelectedValue;


            if (DropDownList_repType.SelectedIndex > 0)
                filter += " and cash_order_type = '" + DropDownList_repType.SelectedValue + "'";

            return filter;
        }
        public class CreateExcelWorksheet
        {
            Application xlApp = new Application();
            Workbook wb;
            Worksheet ws;
            SqlConnection conn = ConnectionFactory.GetNewConnection();//  new SqlConnection(@"initial catalog=ccms;data source=.\sqlexpress;user id = sa;pwd=sa123");
            List<int> list = new List<int>();
            void PrintBankLogo(int regionID, string tempFolder)
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string SQL = "select bank_logo from region where region_id=" + regionID;
                    cmd.CommandText = SQL;
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        byte[] buffer = (byte[])result;

                        //@"c:\ijournalLiveData\temp\img";
                        System.Drawing.Image.FromStream(new MemoryStream(buffer)).Save(tempFolder + "\\img");
                        //ws.Shapes.AddPicture(tempFolder + "\\img"
                        //    , Microsoft.Office.Core.MsoTriState.msoFalse,
                        //Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 70, 70);
                       // ws.Shapes.AddPicture(tempFolder + "\\img"                      , Microsoft.Office.Core.MsoTriState.msoFalse,                Microsoft.Office.Core.MsoTriState.msoTrue, 0, 0, 70, 70);
                    }
                }

            }
            void PrintTitle()
            {

                ws.Cells.set_Item(8, 1, "Currency Management NCR Corporation ATM Order List");
                ((Range)ws.Cells[8, 1]).Font.Size = 12;
                ((Range)ws.Cells[8, 1]).Font.Bold = true;
                ((Range)ws.Cells[8, 1]).Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                ((Range)ws.Cells[8, 1]).EntireRow.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);

            }
            void FormatHeader(int row, int col)
            {

                ((Range)ws.Cells[row, col]).EntireRow.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);
                ((Range)ws.Cells[row, col]).EntireRow.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                ((Range)ws.Cells[row, col]).EntireRow.Font.Bold = true;
                ((Range)ws.Cells[row, col]).EntireRow.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            }
            void PrintHeaders()
            {
                ws.Cells.set_Item(1, 5, "Name Report:");
                ((Range)ws.Cells[1, 5]).Font.Bold = true;
                ws.Cells.set_Item(2, 5, "Run Data:");
                ((Range)ws.Cells[2, 5]).Font.Bold = true;
                ws.Cells.set_Item(2, 6, DateTime.Now.ToString("dd-MM-yyyy hh:mm"));
                ((Range)ws.Cells[2, 6]).Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue);
                ((Range)ws.Cells[2, 6]).Font.Bold = true;


                int col1 = 1;
                ws.Cells.set_Item(10, 1, "Cash Center");
                FormatHeader(10, col1++);
                ws.Cells.set_Item(10, 2, "Branch");

                ws.Cells.set_Item(10, 3, "ATM ID");
                ws.Cells.set_Item(10, 4, "Replenishment date");
                ws.Cells.set_Item(10, 5, "Name");
                ws.Cells.set_Item(10, 6, "Street");
                ws.Cells.set_Item(10, 7, "City");

                string[] parts = new string[] { "500", "100", "50" };
                //System.Configuration.ConfigurationManager.AppSettings["Denominations"].Split(',');
                string currency = "SAR";
                //System.Configuration.ConfigurationManager.AppSettings["Currency"];
                int col = 8;
                foreach (string part in parts)
                {
                    ws.Cells.set_Item(10, col++, "# of " + currency + part);
                    //FormatHeader(10, col1++);
                    ws.Cells.set_Item(10, col++, "Value of " + currency + part);
                    //FormatHeader(10, col1++);
                    list.Add(int.Parse(part));

                }
                ws.Cells.set_Item(10, col++, "Total Amount");
                //FormatHeader(10, col1++);
            }

            void GetRecords(string ATMs, int regionID, string whereClause)
            {

                ws.Rows.Font.Name = "Ariel";
                ws.Rows.Font.Size = 8;
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string SQL = @"select bank_cash_center_id,depot_id,title, location,address1,city,
cash_order_datetime, c.cassette1_denomination,c.cassette1_suggested_notes,
c.cassette2_denomination,c.cassette2_suggested_notes,
c.cassette3_denomination,c.cassette3_suggested_notes,
c.cassette4_denomination,c.cassette4_suggested_notes,
c.cassette5_denomination,c.cassette5_suggested_notes,
c.cassette6_denomination,c.cassette6_suggested_notes,
c.cassette7_denomination,c.cassette7_suggested_notes, 

c.cassette1_denomination*c.cassette1_suggested_notes+
c.cassette2_denomination*c.cassette2_suggested_notes+
c.cassette3_denomination*c.cassette3_suggested_notes+
c.cassette4_denomination*c.cassette4_suggested_notes+
c.cassette5_denomination*c.cassette5_suggested_notes+
c.cassette6_denomination*c.cassette6_suggested_notes+
c.cassette7_denomination*c.cassette7_suggested_notes total

from atm, cash_orders c
where c.atm_id = atm.atm_id and atm.atm_id in (" + ATMs + ")" + whereClause;

                    cmd.CommandText = SQL;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                        throw new Exception("No records found!");
                    int row = 11, col = 1;
                    Hashtable ht = null;
                    Hashtable htRunningTotal = new Hashtable();
                    int total = 0;
                    while (reader.Read())
                    {
                        row++;
                        ht = new Hashtable();
                        col = 1;
                        ws.Cells.set_Item(row, col++, reader[0]);
                        ws.Cells.set_Item(row, col++, reader[1]);
                        ws.Cells.set_Item(row, col++, reader[2]);
                        ws.Cells.set_Item(row, col++, reader.GetDateTime(6).ToString("dd-MMM-yy"));
                        ws.Cells.set_Item(row, col++, reader[3]);
                        ws.Cells.set_Item(row, col++, reader[4]);
                        ws.Cells.set_Item(row, col++, reader[5]);

                        for (int i = 0; i < 13; i += 2)
                        {
                            if (int.Parse(reader[i + 7].ToString()) > 0)
                            {
                                if (ht.Contains(reader[i + 7]))
                                    ht[reader[i + 7]] = (int)ht[reader[i + 7]] + (int)reader[i + 8];

                                else
                                    ht.Add(reader[i + 7], reader[i + 8]);


                                if (htRunningTotal.ContainsKey(reader[i + 7]))
                                    htRunningTotal[reader[i + 7]] = (int)htRunningTotal[reader[i + 7]] + (int)reader[i + 8];
                                else
                                    htRunningTotal.Add(reader[i + 7], (int)reader[i + 8]);


                            }

                        }

                        for (int j = 0; j < list.Count; j++)
                        {
                            ws.Cells.set_Item(row, col++, ht[list[j]]);

                            //if (htRunningTotal.Contains(list[j]))
                            //    ht[list[j]] = (int)ht[list[j]] + int.Parse(ht[list[j]].ToString()) * list[j];
                            //else
                            //    htRunningTotal.Add(list[j], int.Parse(ht[list[j]].ToString()) * list[j]);

                            ws.Cells.set_Item(row, col++, int.Parse(ht[list[j]].ToString()) * list[j]);


                        }
                        ws.Cells.set_Item(row, col++, reader[21]);
                        total += int.Parse(reader[21].ToString());
                    }
                    ws.Cells.set_Item(11, 14, total);
                    col = 8;
                    for (int j = 0; j < list.Count; j++)
                    {

                        ws.Cells.set_Item(11, col++, htRunningTotal[list[j]]);

                        ws.Cells.set_Item(11, col++, (int)htRunningTotal[list[j]] * list[j]);

                    }
                    FormatHeader(11, 1);

                    //(8+(list.Count*2)) == N
                    Range aRange = ws.get_Range("H8", "N" + row);
                    //N16 is not fixed.
                    //Console.WriteLine(row);

                    aRange.NumberFormat = "#,##0.00";
                    //aRange = ws.get_Range("A12", "N" + row);
                    //aRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                }
            }
            public void DoWork(string ATMs, int regionID, string whereClause, string tempFolder)
            {

                try
                {
                    object missing = System.Reflection.Missing.Value;

                    conn.Open();
                    wb = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);

                    ws = (Worksheet)wb.Worksheets[1];
                    PrintBankLogo(regionID, tempFolder);
                    PrintHeaders();
                    GetRecords(ATMs, regionID, whereClause);
                    PrintTitle();
                    conn.Close();
                    //        wb.SaveAs(




                    //             "c:\\copy.xlsx", missing, missing, missing,
                    //missing, missing, missing, missing, missing, missing);
                    string filePath = tempFolder + "\\copy.xls";
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    wb.SaveAs(filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //xlApp.Visible = false;
                    wb.Close(false, false, System.Type.Missing);
                    xlApp.Quit();
                }
                finally
                {
                    try
                    {
                        while (System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp) > 0)
                        {
                            xlApp = null;
                        }

                        Process[] processes = System.Diagnostics.Process.GetProcessesByName("EXCEL");
                        foreach (Process p in processes)
                            p.Kill();
                    }
                    catch (Exception ex)
                    {
                    }

                }




            }


            //static void Main()
            //{
            //    CreateExcelWorksheet f = new CreateExcelWorksheet();
            //    f.DoWork();
            //    f.xlApp.Save(@"C:\myfile.xlsx");


            //    //if (xlApp == null)
            //    //{
            //    //    Console.WriteLine("EXCEL could not be started. Check that your office installation and project references are correct.");
            //    //    return;
            //    //}
            //    //xlApp.Visible = true;

            //    //if (ws == null)
            //    //{
            //    //    Console.WriteLine("Worksheet could not be created. Check that your office installation and project references are correct.");
            //    //}

            //    //        ws.Cells.set_Item(1, 9, "hello");
            //    //Console.WriteLine(((Range)ws.Cells[1, 5]).ColumnWidth);
            //    //((Range)ws.Cells[1, 5]).EntireColumn.ColumnWidth =
            //    //    50;
            //    //Console.WriteLine(((Range)ws.Cells[1, 5]).ColumnWidth);


            //    //((Range)ws.Columns["1,9"]).ColumnWidth =  9.2;


            //    // Select the Excel cells, in the range c1 to c7 in the worksheet.
            //    //     Range aRange = ws.get_Range("C1", "C7");

            //    //     if (aRange == null)
            //    //     {
            //    //         Console.WriteLine("Could not get a range. Check to be sure you have the correct versions of the office DLLs.");
            //    //     }
            //    //     aRange.Font.Bold = true;
            //    //     aRange.Font.Size = 8;
            //    //     //aRange.Interior.Color = 255;
            //    //     aRange.Font.Color = Color.Gray.ToArgb();
            //    //     aRange.Interior.Color = 211;




            //    //     // Fill the cells in the C1 to C7 range of the worksheet with the number 6.
            //    //     Object[] args = new Object[1];
            //    //     args[0] = 100000;

            //    //// aRange.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, aRange, args);


            //    //aRange = ws.get_Range("E1", "E7");
            //    //Console.WriteLine(aRange.NumberFormat);
            //    //aRange.NumberFormat = "#,##0.00";




            //    //aRange.GetType().InvokeMember("Value",   BindingFlags.SetProperty, null, aRange, args);
            //    //((Range)ws.Cells[1, 5]).EntireColumn.AutoFit();






            //    // Change the cells in the C1 to C7 range of the worksheet to the number 8.
            //    //aRange.Value2 = 8;
            //}
        }

    }
}