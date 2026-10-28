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
using System.Data.SqlClient;

using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Text;

namespace CCMSUI.admin
{
    public partial class Vault : BasePage
    {
        //protected void Page_PreInit(object sender, EventArgs e)
        //{
        //    this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (!CheckRight((int)Permissions.ModifyVault))
                    GridView_Vault.Columns[10].Visible = false;

                if (!CheckRight((int)Permissions.DeleteVault))
                    GridView_Vault.Columns[11].Visible = false;

                if (!CheckRight((int)Permissions.CreateVault))
                    HyperLink_newVault.Visible = false;

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";
                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                //   AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                //if (!objappsetting.IsGasperexchangeConfigured.Value)
                //  GridView_ATMs.Columns[3].Visible = true;
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();

                PopulateCIT();
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        string Filter()
        {
            string filter = "";
            if (TextBox_name.Text.Length > 0)
                filter += "  title ='" + TextBox_name.Text + "' and ";
            if (TextBox_OrganizationName.Text.Length > 0)
                filter += "  ccms_organization.name ='" + TextBox_OrganizationName.Text + "' and ";
            if (DropDownList_CIT.SelectedIndex > 0)
                filter += "  ccms_vault.cit_id  =" +DropDownList_CIT.SelectedItem.Value + " and ";
            if (DropDownList_creator.SelectedIndex > 0)
                filter += "  ccms_vault.created_by =" + DropDownList_creator.SelectedValue + " and ";

            filter += " ccms_vault.is_deleted = 0 and ";
            if (filter.Length > 0)
                return "and " + filter.Substring(0, filter.LastIndexOf("and "));
            else
                return filter;
        }

        private void PopulateCIT()
        //private void PopulateCIT(int orgID)
        {
            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add("Select");
            //RegionCit.RegionCitReader reader = RegionCit.ExecuteReader("region_id = " + orgID);
            //while (reader.Read())
            //{
            //    CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(reader.CurrentRegionCit.CitId);
            //    if (ccmsCIT != null)
            //        DropDownList_CIT.Items.Add(new ListItem(ccmsCIT.Name, ccmsCIT.Id.ToString()));
            //}
            //reader.Close();
            CcmsCit.CcmsCitReader reader = CcmsCit.ExecuteReader("1=1");
            while (reader.Read())
            {
                DropDownList_CIT.Items.Add(new ListItem(reader.CurrentCcmsCit.Name, reader.CurrentCcmsCit.Id.ToString()));
            }
            reader.Close();

        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("id",typeof(int));
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("organizationName");
                dtGrid.Columns.Add("citName");
                dtGrid.Columns.Add("created_on",typeof(DateTime));
                dtGrid.Columns.Add("modified_on", typeof(DateTime));
                dtGrid.Columns.Add("description");
                dtGrid.Columns.Add("emergency_cash_detail");
                dtGrid.Columns.Add("threshold_cash_detail");
                dtGrid.Columns.Add("is_deleted",typeof(bool));
                dtGrid.Columns.Add("is_branch", typeof(bool));
                 
                


                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                cmd.CommandText = @"select ccms_vault.id,title,region_name organizationName,ccms_cit.name citName,ccms_vault.is_deleted,is_branch,
                                    description,ccms_vault.created_on,ccms_vault.modified_on
from ccms_vault inner join region
on ccms_vault.organization_id = region.region_id
inner join ccms_cit on ccms_vault.cit_id = ccms_cit.id 
inner join app_user on ccms_vault.created_by = app_user.user_id
" + Filter() + "  order by " + orderby;


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtVault = new DataTable();
                adapter.Fill(dtVault);

                foreach (DataRow dr in dtVault.Rows)
                {
                    DataRow newRow = dtGrid.NewRow();
                    newRow["id"] = dr["id"];
                    newRow["title"] = dr["title"];
                    newRow["organizationName"] = dr["organizationName"];
                    newRow["citName"] = dr["citName"];
                    newRow["is_deleted"] = dr["is_deleted"];
                    newRow["is_branch"] = dr["is_branch"];
                    newRow["description"] = dr["description"];
                    newRow["created_on"] = dr["created_on"];
                    newRow["modified_on"] = dr["modified_on"];

                    StringBuilder builder = new StringBuilder();
                    StringBuilder builderThreshold = new StringBuilder();
                    CcmsVaultNoteType.CcmsVaultNoteTypeReader vaultNoteTypeReader = CcmsVaultNoteType.ExecuteReader("vault_id = " + dr["id"]);
                    while (vaultNoteTypeReader.Read())
                    {
                        builder.Append(vaultNoteTypeReader.CurrentCcmsVaultNoteType.DenominationName + "*" + vaultNoteTypeReader.CurrentCcmsVaultNoteType.EmergencyCash+",");
                        builderThreshold.Append(vaultNoteTypeReader.CurrentCcmsVaultNoteType.DenominationName + "*" + vaultNoteTypeReader.CurrentCcmsVaultNoteType.ThrasholdQty+ ",");
                    }
                    vaultNoteTypeReader.Close();

                    newRow["emergency_cash_detail"] = builder.ToString();
                    newRow["threshold_cash_detail"] = builderThreshold.ToString(); ;
                    dtGrid.Rows.Add(newRow);
                }

                this.GridView_Vault.DataSource = dtGrid;
                this.GridView_Vault.DataBind();
                Literal_count.Text = dtGrid.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Vault_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_Vault_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteVault")
            {
                task = LogableTask.NewTask("delete Vault");

                try
                {



                    //if ((int)ConnectionFactory.ExecuteScalar("select count(cit_id) from app_user where cit_id=" + e.CommandArgument.ToString()) != 0)
                    //    throw new Exception ("Cannot delete this cit as it is assigned to some users");

                    //if ((int)ConnectionFactory.ExecuteScalar("select count(cit_id) from region_cit where cit_id=" + e.CommandArgument.ToString()) != 0)
                    //    throw new Exception("Cannot delete this cit as it is assigned to some organizations");
                    AppUser user = ((AppUser)Session[SessionVars.user.ToString()]);
                    CcmsVault ccmsVault = CcmsVault.LoadCcmsVaultByPk(int.Parse(e.CommandArgument.ToString()));

                    ccmsVault.IsDeleted = true;
                    ccmsVault.ModifiedOn = DateTime.Now;
                    ccmsVault.ModifiedBy = (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    ccmsVault.Save();
                    Utility.BuildAuditLog(ccmsVault.Title + " deleted", user.UserId, (int)Permissions.DeleteVault);

                    //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                    Session[SessionVars.notice.ToString()] = "Selected vault was deleted!";
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "vault deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting Vault. " + ex.Message + "</li>";

                }
                finally
                {

                    task.EndTask();

                }
            }
        }

        protected void GridView_Vault_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Vault.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Vault_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteCIT") != null)


                ((ImageButton)e.Row.FindControl("Button_deleteCIT")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete CIT " + ((DataRowView)e.Row.DataItem).Row["name"] + "?','CCMS')");


        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_name.Text = "";
            DropDownList_creator.SelectedIndex = 0;
            



        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }

    }
}