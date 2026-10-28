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
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;

namespace CCMSUI.admin
{
    public partial class AddEditVaultAdj : BasePage
    {
        DataTable dt = null;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            string regionID = Session[SessionVars.SelectedRegions.ToString()].ToString();
            if (regionID != null /*&& !regionID.Contains(",")*/
                && !regionID.StartsWith("1,"))
            {

                Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
                if (region.IsOrganization)
                {
                    Label_Org.Text = region.RegionName;
                    Session[SessionVars.SelectedRegions.ToString()] = region.RegionId.ToString();
                    PopulateCIT(region.RegionId);

                }

            }
            else
                Label_Org.Text = "Please select Organization";

        }
        private void PopulateCIT(int orgID)
        {
            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add("Select");
            RegionCit.RegionCitReader reader = RegionCit.ExecuteReader("region_id = " + orgID);
            while (reader.Read())
            {
                CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(reader.CurrentRegionCit.CitId);
                if (ccmsCIT != null)
                    DropDownList_CIT.Items.Add(new ListItem(ccmsCIT.Name, ccmsCIT.Id.ToString()));
            }
            reader.Close();
        }

        protected void DropDownList_CIT_Change(object sender, EventArgs e)
        {
            DropDownList_Vault.Items.Clear();
            DropDownList_Vault.Items.Add("Select");
            CcmsVault.CcmsVaultReader reader = CcmsVault.ExecuteReader("cit_id = " + long.Parse(DropDownList_CIT.SelectedItem.Value) + "  and is_deleted=0");
            while (reader.Read())
            {
                DropDownList_Vault.Items.Add(new ListItem(reader.CurrentCcmsVault.Title, reader.CurrentCcmsVault.Id.ToString()));
            }
            reader.Close();
        }



        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.IsPostBack)
                return;

            //TextBox_Amount.Attributes.Add("readonly", "readonly");
            //TextBox_Amount.Style.Add("color", "gray");

            if (Request.QueryString["id"] == null)
            {
                if (!CheckRight((int)Permissions.CreateVaultAdjustment))
                    Response.Redirect("AccessDenied.aspx");
                refresh_ServerClick(this, null);
                trApproveRejectVaultAdj.Visible = false;
            }
            else
            {
                if (!CheckRight((int)Permissions.EditVaultAdjustment))
                    Response.Redirect("AccessDenied.aspx");
            }


            if (Request.QueryString["id"] != null)
            {
                CcmsVaultAdjustments vaultAdj = CcmsVaultAdjustments.LoadCcmsVaultAdjustmentsByPk(int.Parse(Request.QueryString["id"]));

                Session[SessionVars.SelectedRegions.ToString()] = vaultAdj.OrganizationId;
                Region org = Region.LoadRegionByPk(vaultAdj.OrganizationId);
                Label_Org.Text = org.RegionName;

                PopulateCIT(vaultAdj.OrganizationId);
                DropDownList_CIT.SelectedValue = vaultAdj.CitId.ToString();
                DropDownList_CIT_Change(null, null);

                DropDownList_Vault.SelectedValue = vaultAdj.VaultId.ToString();
                DropDownList_Vault_SelectedIndexChanged(null, null);
                DropDownList_TrxnType.SelectedValue = vaultAdj.TrxnType;

                Label_Amount.Text = vaultAdj.Amount.ToString();
                amt.Value = vaultAdj.Amount.ToString();
                dt = GetGridDataTable();
                CcmsVaultNoteType.CcmsVaultNoteTypeReader reader = CcmsVaultNoteType.ExecuteReader("vault_id = " + vaultAdj.VaultId);
                while (reader.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = reader.CurrentCcmsVaultNoteType.DenominationName;

                    dr[2] = vaultAdj.Id;
                    dt.Rows.Add(dr);
                }
                reader.Close();

                if (Request.QueryString["s"] != null)
                {
                    trAddEditVaultAdj.Visible = false;
                    trApproveRejectVaultAdj.Visible = true;


                    if (Request.QueryString["s"].ToString() == "1")
                    {
                        Button_ApproveVaultAdj.Enabled = false;
                        Button_RejectVaultAdj.Enabled = false;
                    }
                    else if (Request.QueryString["s"].ToString() == "2")
                    {
                        Button_ApproveVaultAdj.Enabled = false;
                        Button_RejectVaultAdj.Enabled = false;
                    }
                }
                else
                {
                    trApproveRejectVaultAdj.Visible = false;
                    trAddEditVaultAdj.Visible = true;
                }
                UserTask userTask = UserTask.LoadUserTask("entity_id = " + vaultAdj.Id);
                if (userTask != null)
                {
                    if (userTask.Status == "Pending" || userTask.Status == "Rejected")
                        Button_SaveUpdateVaultAdj.Enabled = true;
                    else
                        Button_SaveUpdateVaultAdj.Enabled = false;
                }
            }
            else
            {
                dt = GetGridDataTable();

            }
            GridView_VaultNoteDetail.DataSource = dt;
            GridView_VaultNoteDetail.DataBind();
            GridView_VaultNoteDetail.Columns[2].Visible = false;
        }


        protected void GridView_VaultNoteDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[2].Text.Length > 0)
                {
                    CcmsVaultAdjustmentsDetail detail = CcmsVaultAdjustmentsDetail.LoadCcmsVaultAdjustmentsDetail("ccms_vault_adjustments_id=" + e.Row.Cells[2].Text
                        + " and denomination_name = '" + e.Row.Cells[0].Text + "'");

                    if (detail != null)
                    {
                        HtmlInputText TextBox_Qty = (HtmlInputText)e.Row.FindControl("TextBox_Quantity");
                        TextBox_Qty.Value = detail.DenominationQty.ToString();

                    }
                }
            }
        }


        public class VaultInfo
        {
            public int newVaultId = 0;
            public int existingVaultId = 0;
        }


        private VaultInfo CreateVaultLedger(int vaultId, decimal amount, int id, SqlTransaction trxn,string trxnType)
        {
            VaultInfo vaultInfo = new VaultInfo();
            CcmsVaultLedger vaultLedger = new CcmsVaultLedger();
            vaultLedger.TransactionDate = DateTime.Now;
            vaultLedger.TransactionType = trxnType;
            vaultLedger.Description = "Adjustment of amount " + amount + " posted on Vault";
            vaultLedger.LedgerAmount = amount;
            vaultLedger.PostedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
            vaultLedger.VaultId = vaultId;
            if (trxnType == "Cr") 
                vaultLedger.VaultTransactionType = "Adjustment Credit";
            else
                vaultLedger.VaultTransactionType = "Adjustment Debit";
            vaultLedger.VaultAdjustmentId = id;
            
            object result = ConnectionFactory.ExecuteScalar("select max(id) from ccms_vault_ledger where vault_id = "+vaultId);
            if (result != DBNull.Value)
            {
                CcmsVaultLedger existingLedger = CcmsVaultLedger.LoadCcmsVaultLedgerByPk(int.Parse(result.ToString()));
                if (existingLedger != null)
                {
                    if (trxnType == "Cr") 
                        vaultLedger.Balance = existingLedger.Balance + vaultLedger.LedgerAmount;
                    else
                        vaultLedger.Balance = existingLedger.Balance - vaultLedger.LedgerAmount;
                    vaultInfo.existingVaultId = existingLedger.Id;
                }
            }
            else
                vaultLedger.Balance = vaultLedger.LedgerAmount;

            vaultLedger.Save(trxn.Connection, trxn);
            vaultInfo.newVaultId = vaultLedger.Id;
            return vaultInfo;
        }

        private void CreateVaultLedgerDetail(VaultInfo vaultInfo, int qty, int vaultNoteTypeId, string trxnType, SqlTransaction trxn)
        {
            CcmsVaultLedgerDetail vaultLedgerDetail = new CcmsVaultLedgerDetail();
            vaultLedgerDetail.LedgerId = vaultInfo.newVaultId;
            vaultLedgerDetail.Quantity = qty;
            vaultLedgerDetail.VaultNoteTypeId = vaultNoteTypeId;

            CcmsVaultLedgerDetail existingVaultLedgerDetail = CcmsVaultLedgerDetail.LoadCcmsVaultLedgerDetail(
                string.Format("ledger_id = {0} and vault_note_type_id={1}", vaultInfo.existingVaultId, vaultNoteTypeId));
            if (existingVaultLedgerDetail != null)
            {
                if (vaultLedgerDetail.Balance==null)
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
            vaultLedgerDetail.Save(trxn.Connection, trxn);
        }

        protected void Button_ApproveVaultAdj_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                trxn = conn.BeginTransaction();

                CcmsVaultAdjustments vaultAdj = CcmsVaultAdjustments.LoadCcmsVaultAdjustmentsByPk(int.Parse(Request.QueryString["id"]));
                VaultInfo vaultInfo = CreateVaultLedger(vaultAdj.VaultId, vaultAdj.Amount, vaultAdj.Id, trxn, vaultAdj.TrxnType);

                DataTable dtVaultNotesDetail = new DataTable();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = "select id, denomination_id, denomination_name from ccms_vault_note_type where vault_id = " + vaultAdj.VaultId;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dtVaultNotesDetail);


                foreach (GridViewRow row in GridView_VaultNoteDetail.Rows)
                {
                    string type = row.Cells[0].Text;
                    HtmlInputText quantity = (HtmlInputText)row.FindControl("TextBox_Quantity");
                    DataRow[] dr = dtVaultNotesDetail.Select("denomination_name='" + type + "'");
                    CreateVaultLedgerDetail( vaultInfo, int.Parse(quantity.Value), int.Parse(dr[0]["id"].ToString()), vaultAdj.TrxnType, trxn);
                }

            //Do not enter in temp vault....Forgot reason thats uncommenting it..
                ConnectionFactory.ExecuteQuery(string.Format(@"insert into ccms_temp_vault_ledger(
                                                  [transaction_date] ,[description] ,[transaction_type] ,[ledger_amount] ,[balance] ,[posted_by]
                                                  ,[vault_id] ,[vault_transaction_type] ,[cheque_id] ,[atm_id]) 
                                                    select [transaction_date] ,[description] ,[transaction_type] ,[ledger_amount] ,[balance] ,[posted_by]
                                                  ,[vault_id] ,[vault_transaction_type] ,[cheque_id] ,[atm_id] from ccms_vault_ledger
                                                   where id = {0}; insert into ccms_temp_vault_ledger_detail(
                                                   [ledger_id],[quantity] ,[vault_note_type_id],balance) 
                                                    select scope_identity(),[quantity] ,[vault_note_type_id],balance from ccms_vault_ledger_detail
                                                    where ledger_id = {0}", vaultInfo.newVaultId), trxn);

                Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(base.Request.QueryString["tid"]));
                userTask.Status = ApprovalStatus.Approved.ToString();
                userTask.Save(trxn.Connection, trxn);
                trxn.Commit();
                Utility.BuildAuditLog("Vault Adjustment Approved " + CcmsVault.LoadCcmsVaultByPk(vaultAdj.VaultId).Title + " approved", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ApproveVaultAdjustment);
                Response.Redirect("~/ccms/usertask.aspx");
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                LogableTask.LogMonoActivityTask("ApproveVaultAdjustment", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

        }
        protected void Button_Reject_Click(object sender, EventArgs e)
        {
            Avanza.CCMS.DAL.UserTask userTask = Avanza.CCMS.DAL.UserTask.LoadUserTaskByPk(int.Parse(base.Request.QueryString["tid"]));
            userTask.Status = ApprovalStatus.Rejected.ToString();
            userTask.Save();
            base.Response.Redirect("~/ccms/UserTask.aspx");
        }

        protected void Button_SaveUpdateVaultAdj_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddEditVaultAdjustment");
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            CcmsVaultAdjustments vaultAdj;
            CcmsVaultAdjustmentsDetail detail;
            bool isTransactionStarted = false;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trxn = conn.BeginTransaction();
                isTransactionStarted = true;
                if (Request.QueryString["id"] != null)
                {
                    vaultAdj = CcmsVaultAdjustments.LoadCcmsVaultAdjustmentsByPk(int.Parse(Request.QueryString["id"]));
                    vaultAdj.ModifiedOn = DateTime.Now;
                    vaultAdj.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    
                }
                else
                {
                    vaultAdj = new CcmsVaultAdjustments();
                    vaultAdj.CreatedOn = DateTime.Now;
                    vaultAdj.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    
                }
                vaultAdj.Amount = decimal.Parse(amt.Value);
                
                vaultAdj.CitId = int.Parse(DropDownList_CIT.SelectedItem.Value);
                vaultAdj.VaultId = int.Parse(DropDownList_Vault.SelectedItem.Value);
                vaultAdj.OrganizationId = int.Parse(Session[SessionVars.SelectedRegions.ToString()].ToString());
                vaultAdj.TrxnType = DropDownList_TrxnType.SelectedItem.Text;
                vaultAdj.Save(trxn.Connection, trxn);
                ConnectionFactory.ExecuteQuery("delete ccms_vault_adjustments_detail where ccms_vault_adjustments_id = " + vaultAdj.Id, trxn);


                 
                DataTable dtVaultNotesDetail = new DataTable();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = "select id, denomination_id, denomination_name from ccms_vault_note_type where vault_id = " + vaultAdj.VaultId;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dtVaultNotesDetail);


                foreach (GridViewRow row in GridView_VaultNoteDetail.Rows)
                {
                    string type = row.Cells[0].Text;
                    HtmlInputText quantity = (HtmlInputText)row.FindControl("TextBox_Quantity");
                    detail = new CcmsVaultAdjustmentsDetail();
                    DataRow[] dr = dtVaultNotesDetail.Select("denomination_name='" + type + "'");

                    detail.DenominationId = int.Parse(dr[0]["denomination_id"].ToString());
                    detail.DenominationName = dr[0]["denomination_name"].ToString();
                    detail.DenominationQty = int.Parse(quantity.Value);
                    detail.VaultNoteTypeId = int.Parse(dr[0]["id"].ToString());
                    detail.CcmsVaultAdjustmentsId = vaultAdj.Id;
                    detail.Save(trxn.Connection, trxn);
                    //  CreateVaultLedgerDetail(vaultLedgerId, detail.DenominationQty, detail.VaultNoteTypeId, trxn);

                }

                if (Request.QueryString["id"] == null)
                    CCMSUI.CCMS.Investigation.CreateUserAndAssignmentTasks(
                        ((AppUser)Session[SessionVars.user.ToString()]).UserId,
                        "Vault Adjustment posted for approval", vaultAdj.Id, (int)EnumTaskType.ApproveVaultAdjustment, trxn);
                else
                {
                    ConnectionFactory.ExecuteQuery("update user_task set status = 'Pending' where entity_id = " + vaultAdj.Id, trxn);
                }
                //Create User Task...
//                UserTask userTask = new UserTask();
//                userTask.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
//                userTask.CreationTime = DateTime.Now;
//                userTask.Status = ApprovalStatus.Pending.ToString();
//                userTask.TaskTypeId = (int)EnumTaskType.ApproveVaultAdjustment;
//                userTask.TaskDesc = "Vault Adjustment posted for approval";
//                userTask.EntityType = "Vault Adjustment";
//                userTask.EntityId = vaultAdj.Id;
//                //userTask.AtmId = atm.ATMId;
//                userTask.Save(trxn.Connection, trxn);

//                //User that can view this task..
//                StringBuilder builder = new StringBuilder();
//                cmd.CommandText = @"select user_id , employee_manager_id from app_user
//                                   where employee_manager_id is not null";

//                DataTable dtUser = new DataTable();
//                adapter = new SqlDataAdapter(cmd);
//                adapter.Fill(dtUser);
//                int id = userTask.CreatedBy;
//                while (true)
//                {
//                    DataRow[] dataRow = dtUser.Select("user_id = " + id);
//                    if (dataRow.Length > 0)
//                    {
//                        //Create task Assignment
//                        id = int.Parse(dataRow[0]["employee_manager_id"].ToString());
//                        UserTaskAssignment taskAssignment = new UserTaskAssignment();
//                        taskAssignment.UserTaskId = userTask.UserTaskId;
//                        taskAssignment.UserId = id;
//                        taskAssignment.Save(trxn.Connection, trxn);
//                    }
//                    else
//                        break;
//                }



                ///////////////////////////////

                trxn.Commit();


                if (Request.QueryString["id"] == null) 
                    Utility.BuildAuditLog("New Vault Adjustment " +CcmsVault.LoadCcmsVaultByPk(vaultAdj.VaultId).Title + " posted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateVaultAdjustment);
                else
                    Utility.BuildAuditLog("Vault Adjustment " + CcmsVault.LoadCcmsVaultByPk(vaultAdj.VaultId).Title + " edited", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditVaultAdjustment);

                Response.Redirect("VaultAdjustments.aspx", false);
            }
            catch (Exception ex)
            {
                if (isTransactionStarted)
                    trxn.Rollback();
                Literal_error.Text = "<li>An error occured while saving vault adjustments." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }



        private DataTable GetGridDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("type");
            dt.Columns.Add("quantity");
            dt.Columns.Add("vault_adjustment_id");
            return dt;
        }


        protected void DropDownList_Vault_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = GetGridDataTable();
            //CcmsVault ccmsVault = CcmsVault.LoadCcmsVaultByPk(int.Parse(DropDownList_Vault.SelectedItem.Value));


            CcmsVaultNoteType.CcmsVaultNoteTypeReader reader = CcmsVaultNoteType.ExecuteReader("vault_id = " + DropDownList_Vault.SelectedItem.Value);
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                dr[0] = reader.CurrentCcmsVaultNoteType.DenominationName;
                dr[1] = 0;
                dr[2] = 0;
                dt.Rows.Add(dr);
            }
            reader.Close();

            //CcmsOrgNoteSet ccmsOrgNoteSet = CcmsOrgNoteSet.LoadCcmsOrgNoteSet("name='" + Label_Org.Text + "'");
            //if (ccmsOrgNoteSet != null)
            //{
            //    CcmsOrgNoteSetItem.CcmsOrgNoteSetItemReader ccmsOrgNoteSetItemReader = CcmsOrgNoteSetItem.ExecuteReader("org_note_set_id=" + ccmsOrgNoteSet.Id);
            //    while (ccmsOrgNoteSetItemReader.Read())
            //    {
            //        DataRow dr = dt.NewRow();
            //        dr[0] = ccmsOrgNoteSetItemReader.CurrentCcmsOrgNoteSetItem.DenominationName;
            //        dr[1] = 0;
            //        dr[2] = 0;
            //        dt.Rows.Add(dr);
            //    }
            //}
            GridView_VaultNoteDetail.DataSource = dt;
            GridView_VaultNoteDetail.DataBind();
            Page.SetFocus(GridView_VaultNoteDetail);
        }


    }
}