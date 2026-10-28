using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

using Avanza.CCMS.DAL;


namespace Avanza.iSuite.UI
{
    public partial class VaultAdjustments : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewVaultAdjustment, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (!CheckRight((int)Permissions.EditVaultAdjustment))
                    GridView_VaultAdj.Columns[5].Visible = false;

                if (!CheckRight((int)Permissions.DeleteVaultAdjustment))
                    GridView_VaultAdj.Columns[6].Visible = false;

                if (!CheckRight((int)Permissions.CreateVaultAdjustment))
                    this.HyperLink_newVaultAdj.Visible = false;

                PopulateCIT();
                ResetAllControls();
                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = "created_on";

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        private void PopulateCIT()
        {
            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add("Select");
            CcmsCit.CcmsCitReader reader = CcmsCit.ExecuteReader("1=1");
            while (reader.Read())
                DropDownList_CIT.Items.Add(new ListItem(reader.CurrentCcmsCit.Name, reader.CurrentCcmsCit.Id.ToString()));
            reader.Close();
        }

        string Filter()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and ccms_vault_adjustments.created_on >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and ccms_vault_adjustments.created_on <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";


            if (DropDownList_CIT.SelectedIndex > 0)
                filter += " and ccms_cheque.cit_id = " + DropDownList_CIT.SelectedValue;

            //if (DropDownList_Vault.SelectedIndex > 0)
            //    filter += " and vault_id = " + DropDownList_Vault.SelectedValue;

            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT amount,name,title,ccms_vault_adjustments.id,ccms_vault_adjustments.created_on,ccms_vault_adjustments.modified_on
                                    FROM ccms_vault_adjustments inner join ccms_cit on ccms_vault_adjustments.cit_id= ccms_cit.id 
                                    inner join ccms_vault on ccms_vault.id = ccms_vault_adjustments.vault_id where ccms_vault_adjustments.created_by = " + ((AppUser)Session[SessionVars.user.ToString()]).UserId + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtCheques = new DataTable();
                adapter.Fill(dtCheques);
                cmd.Connection.Close();
                this.GridView_VaultAdj.DataSource = dtCheques;
                this.GridView_VaultAdj.DataBind();
                Literal_count.Text = dtCheques.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_VaultAdj_Sorting(object sender, GridViewSortEventArgs e)
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


        protected void GridView_VaultAdj_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteVaultAdj")
            {
                try
                {
                    task = LogableTask.NewTask("delete vault adjustemnt");
                    CcmsVaultAdjustments vaultAdj = CcmsVaultAdjustments.LoadCcmsVaultAdjustmentsByPk(int.Parse(e.CommandArgument.ToString()));
                    ConnectionFactory.ExecuteQuery("delete ccms_vault_adjustments_detail where ccms_vault_adjustments_id = " + vaultAdj.Id + ";delete ccms_vault_adjustments where id = " + vaultAdj.Id + ";delete user_task where entity_id = " + vaultAdj.Id);
                    if (vaultAdj != null)
                    {
                        Utility.BuildAuditLog(vaultAdj.Id + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteCheque);
                        Session[SessionVars.notice.ToString()] = "Selected vault adjustment deleted";
                    }

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Vault adjustment deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting vault adjustment. " + ex.Message + "</li>";
                }
                finally
                {
                    task.EndTask();
                }
            }
        }

        protected void GridView_VaultAdj_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_VaultAdj.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_VaultAdj_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteVaultAdjustment") != null)

                ((ImageButton)e.Row.FindControl("Button_deleteVaultAdjustment")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete vault adjustment for vault" + ((DataRowView)e.Row.DataItem).Row["title"] + "?','CCMS')");
         }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        private void ResetAllControls()
        {
            TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            DropDownList_CIT.SelectedIndex = 0;
            //DropDownList_Vault.SelectedIndex = 0;
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            ResetAllControls();
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


    }
}