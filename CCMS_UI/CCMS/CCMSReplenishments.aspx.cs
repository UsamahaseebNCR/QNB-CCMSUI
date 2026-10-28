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



namespace CCMSUI.CCMS
{
    public partial class CCMSReplenishments : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                HyperLink_PostRep.Visible = CheckRight((int)Permissions.PostReplenishment);
                if (!CheckRight((int)Permissions.EditReplenishment))
                    GridView_Rep.Columns[10].Visible = false;

                if (!CheckRight((int)Permissions.DeleteReplenishment))
                    GridView_Rep.Columns[11].Visible = false;


                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                //AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                //DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                //while (userReader.Read())
                //    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                //userReader.Close();


                BindGrid("title asc");

            }
        }

        string Filter()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and rep_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and rep_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            //if (DropDownList_creator.SelectedIndex > 0)
            //    filter += " and c.created_by = " + DropDownList_creator.SelectedValue;

            //if (DropDownList_repType.SelectedIndex > 0)
            //    filter += " and cash_order_type= '" + DropDownList_repType.SelectedValue + "'";

            //if (DropDownList_OrderStatus.SelectedIndex == 1)
            //    filter += " and is_cancelled= 1 ";

            //else if (DropDownList_OrderStatus.SelectedIndex == 2)
            //    filter += " and is_cancelled= 0 ";


            return filter;
        }

        private void BindGrid(string orderBy)
        {
            try
            {
                ///<Summary>Edited by Ali Shah on 09th Oct, 2019
                ///To dynamically handling of cassettes more than 4
                EnableGridRespectiveColumns();

                /*c.order_number,c.cassette1_denomination,c.cassette2_denomination,
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
c.cassette7_denomination*cassette7_suggested_notes sum*/
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select *,
dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4, cash_added1 , cash_added2, cash_added3, cash_added4) amt_text
from vReplenishment inner join atm on vReplenishment.atm_id = atm.atm_id
inner join note_set_type on note_set_type.note_set_type_id = atm.note_set_type_id  
left join app_user on vReplenishment.generated_by = app_user.user_id
                                                       where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + Filter() + " order by " + orderBy;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                System.Data.DataTable dt = new System.Data.DataTable();
                adapter.Fill(dt);
                this.GridView_Rep.DataSource = dt;
                this.GridView_Rep.DataBind();
                Literal_count.Text = dt.Rows.Count + " records.";

            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>An Error occured while loading replenishment. " + ex.Message + "</li>";
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

        protected void GridView_Rep_Sorting(object sender, GridViewSortEventArgs e)
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
            //GridView_Rep.DataSource = dv;
            //GridView_Rep.DataBind();

        }




        protected void GridView_Rep_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Rep.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            //System.Data.DataTable dt = (System.Data.DataTable)Cache[CacheVars.dtCashOrders.ToString()];
            //DataView dv = new DataView(dt);
            //if (GridViewSortExpression != "")
            //    dv.Sort = GridViewSortExpression + ((GridViewSortDirection == 0) ? " ASC" : " DESC");
            //this.GridView_Rep.DataSource = dv;

            //this.GridView_Rep.PageIndex = e.NewPageIndex;
            //this.GridView_Rep.DataBind();

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


        protected void GridView_Rep_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteRep")
            {
                try
                {
                    task = LogableTask.NewTask("deleteRep");
                    Avanza.CCMS.DAL.Replenishment replenishment = Avanza.CCMS.DAL.Replenishment.LoadReplenishmentByPk(int.Parse(e.CommandArgument.ToString()));

                    if (replenishment != null)
                    {
                        replenishment.Delete();
                        Utility.BuildAuditLog(replenishment.RepDatetime + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteReplenishment);

                        //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                        Session[SessionVars.notice.ToString()] = "Selected replenishment deleted!";
                    }

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "replenishment deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting replenishment. " + ex.Message + "</li>";
                }
                finally
                {
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

        protected void GridView_Rep_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.FindControl("Button_DeleteRep") != null)
                    ((ImageButton)e.Row.FindControl("Button_DeleteRep")).Attributes.Add("onClick",
                        "return confirm('Are you sure you want to delete replenishment for "+DateTime.Parse(((DataRowView)e.Row.DataItem).Row["rep_datetime"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")  + " for the ATM " + ((DataRowView)e.Row.DataItem).Row["Title"] + "?','CCMS')");
            }
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





            //if (DropDownList_creator.SelectedIndex > 0)
            //    filter += " and c.created_by = " + DropDownList_creator.SelectedValue;


            //if (DropDownList_repType.SelectedIndex > 0)
            //    filter += " and cash_order_type = '" + DropDownList_repType.SelectedValue + "'";

            return filter;
        }

        /// <summary>
        /// Edited by Ali Shah on 10th Oct, 2019.
        /// ///To dynamically handling of cassettes more than 4
        /// </summary>
        private void EnableGridRespectiveColumns()
        {
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexReplenishment = -1;
            //int columnIndexDispensed = -1;
            //int columnIndexRemaining = -1;
            columnIndexReplenishment = GetColumnIndexByName(this.GridView_Rep, "Type 5");
            //columnIndexDispensed = GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            //columnIndexRemaining = columnIndexDenomination + 1;// GetColumnIndexByName(this.GridView_CashPositions, "Remaining");
            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexReplenishment >= 0)
                {
                    this.GridView_Rep.Columns[columnIndexReplenishment].Visible = true;
                    columnIndexReplenishment++;
                }

                //if (columnIndexDispensed >= 0)
                //{
                //    this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
                //    columnIndexDispensed++;
                //}
                //if (columnIndexRemaining >= 0)
                //{
                //    this.GridView_CashPositions.Columns[columnIndexRemaining].Visible = true;
                //    columnIndexRemaining += 2;
                //}
            }
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
                        //ws.Shapes.AddPicture(tempFolder + "\\img"                      , Microsoft.Office.Core.MsoTriState.msoFalse,                Microsoft.Office.Core.MsoTriState.msoTrue, 0, 0, 70, 70);
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