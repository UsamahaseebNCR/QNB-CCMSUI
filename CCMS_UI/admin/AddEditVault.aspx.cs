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
using ComponentArt.Web.UI;

namespace CCMSUI.admin
{
    public partial class AddEditVault : BasePage
    {
        DataTable dt = null;
        protected void Page_Load(object sender, EventArgs e)
        {

            Page.SetFocus(TextBox_Title);
            if (Page.IsPostBack)
                return;




            if (Request.QueryString["id"] == null)
            {
                if (!CheckRight((int)Permissions.CreateVault))
                    Response.Redirect("AccessDenied.aspx");

            }
            else
            {
                if (!CheckRight((int)Permissions.ModifyVault))
                    Response.Redirect("AccessDenied.aspx");
            }

            //Organization
            DropDownList_Organization.Items.Add(new ListItem("Select", "Select"));
            Region.RegionReader OrganizationReader = Region.ExecuteReader("region_id > 1 and is_organization=1");
            while (OrganizationReader.Read())
                DropDownList_Organization.Items.Add(new ListItem(OrganizationReader.CurrentRegion.RegionName, OrganizationReader.CurrentRegion.RegionId.ToString()));
            OrganizationReader.Close();

            //NoteSetType
            DropDownList_NoteSetType.Items.Add(new ListItem("Select", "Select"));

            //CIT 
            //DropDownList_CIT.Items.Add(new ListItem("Select", "Select"));
            //CcmsCit.CcmsCitReader CitReader = CcmsCit.ExecuteReader("1=1");
            //while (CitReader.Read())
            //    DropDownList_CIT.Items.Add(new ListItem(CitReader.CurrentCcmsCit.Name, CitReader.CurrentCcmsCit.Id.ToString()));
            //CitReader.Close();


            if (Request.QueryString["id"] != null)
            {
                CcmsVault vault = Avanza.CCMS.DAL.CcmsVault.LoadCcmsVaultByPk(int.Parse(Request.QueryString["id"]));
                TextBox_Title.Text = vault.Title;
                TextBox_Description.Text = vault.Description;
                DropDownList_Organization.SelectedValue = vault.OrganizationId.ToString();
                

                DropDownList_Organization_SelectedIndexChanged(this, null);
                DropDownList_CIT.SelectedValue = vault.CitId.ToString();
                //DropDownList_CIT_SelectedIndexChanged(this, null);

                DropDownList_NoteSetType.SelectedValue = vault.NoteSetTypeId.ToString();
                //DropDownList_Region.SelectedValue = vault.RegionId.ToString();
                CheckBox_Branch.Checked = vault.IsBranch;

                DropDownList_NoteSetType.Enabled = false;

                dt = GetGridDataTable();
                CcmsVaultNoteType.CcmsVaultNoteTypeReader reader = CcmsVaultNoteType.ExecuteReader("vault_id = " + vault.Id);
                while (reader.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = reader.CurrentCcmsVaultNoteType.DenominationName;
                    //dr[1] = reader.CurrentCcmsVaultNoteType.EmergencyCash;
                    //dr[2] = reader.CurrentCcmsVaultNoteType.ThrasholdQty;
                    dr[3] = reader.CurrentCcmsVaultNoteType.Id;
                    dt.Rows.Add(dr);
                }
                ArrayList atmList = new ArrayList();
                VaultAtm.VaultAtmReader vaultAtmReader = VaultAtm.ExecuteReader("vault_id = " + vault.Id);
                while (vaultAtmReader.Read())
                {
                    atmList.Add(vaultAtmReader.CurrentVaultAtm.AtmId);

                }
                vaultAtmReader.Close();
                if (atmList.Count > 0)
                    for (int i = 0; i < UserATMsTreeView.Nodes.Count; i++)
                        SetCheck(UserATMsTreeView.Nodes[i], atmList);


            }
            else
            {
                dt = GetGridDataTable();

            }
            GridView_VaultNoteDetail.DataSource = dt;
            GridView_VaultNoteDetail.DataBind();

            GridView_VaultNoteDetail.Columns[3].Visible = false;

        }
        protected void SetCheck(TreeViewNode node, ArrayList selectedItem)
        {
            TreeViewNodeCollection nodeCol = node.Nodes;
            int regions = 0;

            for (int counter = 0; counter < nodeCol.Count; counter++)
            {

                if (selectedItem.IndexOf(int.Parse(nodeCol[counter].ID.Substring(1))) >= 0)
                {
                    nodeCol[counter].Checked = true;
                    regions++;
                }

                if (nodeCol[counter].Nodes.Count > 0)
                {
                    SetCheck(nodeCol[counter], selectedItem);
                    //if (regions == nodeCol.Count && regions != 0)
                    //    nodeCol[0].ParentNode.Checked = true;
                    if (nodeCol[counter].Checked)
                        regions++;
                }
                //else
                //{
                //    nodeCol[counter].Checked = true;
                //    regions++;
                //}
            }

            if (regions == nodeCol.Count && regions != 0)
                nodeCol[0].ParentNode.Checked = true;
        }

        protected void GridView_VaultNoteDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[3].Text.Length > 0)
                {
                    CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteTypeByPk(int.Parse(e.Row.Cells[3].Text));
                    if (vaultNoteType != null)
                    {
                        TextBox TextBox_EmergencyCash = (TextBox)e.Row.FindControl("TextBox_EmergencyCash");
                        TextBox TextBox_ThresholdCash = (TextBox)e.Row.FindControl("TextBox_ThresholdCash");
                        TextBox_EmergencyCash.Text = vaultNoteType.EmergencyCash.ToString();
                        TextBox_ThresholdCash.Text = vaultNoteType.ThrasholdQty.ToString();
                    }
                }
            }
        }




        protected void Button_SaveUpdateVault_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddEditVault");
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            CcmsVault vault;
            CcmsVaultNoteType vaultNoteType;
            bool isTransactionStarted = false;
            try
            {

                if (Request.QueryString["id"] != null)
                {
                    vault = Avanza.CCMS.DAL.CcmsVault.LoadCcmsVaultByPk(int.Parse(Request.QueryString["id"]));
                    vault.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    vault.ModifiedOn = DateTime.Now;
                }
                else
                {
                    vault = new CcmsVault();
                    vault.CreatedOn = DateTime.Now;
                    vault.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                }
                object res = ConnectionFactory.ExecuteScalar("select top 1 title from ccms_vault where title='" + TextBox_Title.Text + "' "
                          + ((Request.QueryString["id"] != null) ? "and id != " + vault.Id : ""));
                if (res != null)
                {
                    Literal_error.Text = "<li>This title is already defined for vault '" + res + "'. Please, provide different vault title.</li>";
                    return;
                }



                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trxn = conn.BeginTransaction();
                isTransactionStarted = true;
                vault.IsBranch = CheckBox_Branch.Checked;
                vault.Title = TextBox_Title.Text;
                vault.OrganizationId = int.Parse(DropDownList_Organization.SelectedItem.Value);
                //vault.RegionId = int.Parse(DropDownList_Region.SelectedItem.Value);
                vault.NoteSetTypeId = int.Parse(DropDownList_NoteSetType.SelectedValue);
                vault.CitId = int.Parse(DropDownList_CIT.SelectedItem.Value);
                vault.Description = TextBox_Description.Text;
                vault.IsDeleted = false;
                vault.Save(conn, trxn);

                foreach (GridViewRow row in GridView_VaultNoteDetail.Rows)
                {
                    string type = row.Cells[0].Text;
                    TextBox TextBox_EmergencyCash = (TextBox)row.FindControl("TextBox_EmergencyCash");
                    TextBox TextBox_ThresholdCash = (TextBox)row.FindControl("TextBox_ThresholdCash");
                    string vaultNoteTypeId = row.Cells[3].Text;
                    vaultNoteType = GetCcmsVaultNoteType(vaultNoteTypeId);
                    vaultNoteType.DenominationName = type;
                    int id = (int)ConnectionFactory.ExecuteScalar("select id from ccms_denomination where name='" + vaultNoteType.DenominationName + "'");
                    vaultNoteType.DenominationId = id;
                    vaultNoteType.EmergencyCash = int.Parse(TextBox_EmergencyCash.Text);
                    vaultNoteType.ThrasholdQty = int.Parse(TextBox_ThresholdCash.Text);
                    vaultNoteType.VaultId = vault.Id;
                    vaultNoteType.Save(conn, trxn);

                }
                ConnectionFactory.ExecuteScalar("delete vault_atm where vault_id = " + vault.Id, trxn);
                foreach (ComponentArt.Web.UI.TreeViewNode node in UserATMsTreeView.CheckedNodes)
                {
                    if (node.ID.StartsWith("a"))
                    {
                        VaultAtm vaultAtm = new VaultAtm();
                        vaultAtm.VaultId = vault.Id;
                        vaultAtm.AtmId = int.Parse(node.ID.Substring(1));
                        vaultAtm.Save(trxn.Connection, trxn);
                    }
                
                }
                 trxn.Commit();

                //object res = ConnectionFactory.ExecuteScalar("select top 1 name from ccms_cit where name='" + TextBox_Name.Text + "' "
                //          + ((Request.QueryString["id"] != null) ? "and id != " + cit.Id : ""));
                //if (res != null)
                //{
                //    Literal_error.Text = "<li>This name is already defined for cit '" + res + "'. Please, provide different cit name.</li>";
                //    return;
                //}



                // AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                //cit.IsActive = RadioButton_active.Checked;

                //cit.Name = TextBox_Name.Text;
                //cit.Location = TextBox_Location.Text;
                //cit.Id = TextBox_ID.Text;
                //cit.TeamId = TextBox_TeamID.Text;
                //cit.CcId = TextBox_CCID.Text;
                //RadioButton_inactive.Checked = !cit.IsActive.Value;
                //cit.Save();
                if (Request.QueryString["id"] == null)
                    Utility.BuildAuditLog(vault.Title + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateVault);
                else
                    Utility.BuildAuditLog(vault.Title + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyVault);

                Response.Redirect("vault.aspx", false);
            }
            catch (Exception ex)
            {
                if (isTransactionStarted)
                    trxn.Rollback();
                Literal_error.Text = "<li>An error occured while saving Vault." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        protected void DropDownList_CIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CIT Region 

            //if (DropDownList_CIT.SelectedIndex == 0)
            //{
            //    Literal_error.Text = "<li>Please select CIT</li>";
            //    return;
            //}

            //if (DropDownList_Organization.SelectedIndex == 0)
            //{
            //    Literal_error.Text = "<li>Please select Organization</li>";
            //    return;
            //}

            Region organization = Region.LoadRegionByPk(int.Parse(DropDownList_Organization.SelectedValue));
            Region.RegionReader regionReader = Region.ExecuteReader("parent_region_id = " + organization.RegionId);
            string regionIds = "";
            while (regionReader.Read())
            {
                regionIds += regionReader.CurrentRegion.RegionId + ",";
            }

            if (regionIds.IndexOf(",") > 0)
            {
                regionIds = regionIds.Substring(0, regionIds.Length - 1);
            }
            RegionCit.RegionCitReader RegionCitReader = RegionCit.ExecuteReader("cit_id=" + DropDownList_CIT.SelectedItem.Value + ((regionIds != string.Empty) ? " and region_id in (" + regionIds + ")" : ""));

            while (RegionCitReader.Read())
            {
                Region region = Region.LoadRegionByPk(int.Parse(RegionCitReader.CurrentRegionCit.RegionId.ToString()));
                Region parentRegion = Region.LoadRegionByPk(int.Parse(region.ParentRegionId.ToString()));
                //DropDownList_Region.Items.Add(new ListItem(parentRegion.MCN + " - " + region.RegionName, region.RegionId.ToString()));
            }
            RegionCitReader.Close();
            //Page.SetFocus(DropDownList_Region);
        }

        private DataTable GetGridDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("type");
            dt.Columns.Add("emergencyCash");
            dt.Columns.Add("thresholdCash");
            dt.Columns.Add("vault_note_type_id");
            return dt;
        }
        protected void DropDownList_Organization_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (DropDownList_Organization.SelectedIndex == 0)
            {
                Literal_error.Text = "<li>Please select Organization</li>";
                return;
            }

            DropDownList_NoteSetType.Items.Clear();
            DropDownList_NoteSetType.Items.Add(new ListItem("Select", "Select"));
            Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader noteSetTypeReader = Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id =" + DropDownList_Organization.SelectedValue);
            while (noteSetTypeReader.Read())
                DropDownList_NoteSetType.Items.Add(new ListItem(noteSetTypeReader.CurrentNoteSetType.NoteSetTypeName, noteSetTypeReader.CurrentNoteSetType.NoteSetTypeId.ToString()));
            noteSetTypeReader.Close();
            Page.SetFocus(DropDownList_NoteSetType);
            BuildTreeForBank(long.Parse(DropDownList_Organization.SelectedItem.Value));

            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add(new ListItem("Select", "Select"));
            RegionCit.RegionCitReader RegionCitReader = RegionCit.ExecuteReader("region_id=" + DropDownList_Organization.SelectedItem.Value);
            while (RegionCitReader.Read())
            {
                CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(RegionCitReader.CurrentRegionCit.CitId);
                DropDownList_CIT.Items.Add(new ListItem(ccmsCIT.Name, ccmsCIT.Id.ToString()));
            }
            RegionCitReader.Close();

        }

        protected void DropDownList_NoteSetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList_NoteSetType.SelectedIndex == 0)
            {
                Literal_error.Text = "<li>Please select NoteSetType</li>";
                return;
            }

            DataTable dt = GetGridDataTable();

            CcmsOrganization ccmsOrg = CcmsOrganization.LoadCcmsOrganization("name='" + DropDownList_Organization.SelectedItem.Text + "'");
            CcmsOrgNoteSet ccmsOrgNoteSet = CcmsOrgNoteSet.LoadCcmsOrgNoteSet("organization_id=" + ccmsOrg.Id + " and name = '" + DropDownList_NoteSetType.SelectedItem.Text+"'");
            if (ccmsOrgNoteSet != null)
            {
                CcmsOrgNoteSetItem.CcmsOrgNoteSetItemReader ccmsOrgNoteSetItemReader = CcmsOrgNoteSetItem.ExecuteReader("org_note_set_id=" + ccmsOrgNoteSet.Id);
                while (ccmsOrgNoteSetItemReader.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = ccmsOrgNoteSetItemReader.CurrentCcmsOrgNoteSetItem.DenominationName;
                    dr[1] = 0;
                    dr[2] = 0;
                    dr[3] = 0;
                    dt.Rows.Add(dr);
                }
            }
            GridView_VaultNoteDetail.DataSource = dt;
            GridView_VaultNoteDetail.DataBind();
            Page.SetFocus(DropDownList_CIT);
        }

        public static CcmsVaultNoteType GetCcmsVaultNoteType(String vaultNoteTypeId)
        {
            if (vaultNoteTypeId == "")
                return new CcmsVaultNoteType();

            CcmsVaultNoteType vaultNoteType = CcmsVaultNoteType.LoadCcmsVaultNoteTypeByPk(int.Parse(vaultNoteTypeId));

            if (vaultNoteType == null)
                vaultNoteType = new CcmsVaultNoteType();

            return vaultNoteType;
        }
        protected void BuildTreeForBank(long
           organizationId)
        {
            UserATMsTreeView.Nodes.Clear();


            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select region_name, region_id, parent_region_id, is_organization, mcn from region 
                                  where region_id = " + organizationId + "; " +
                                " select region_name, region_id, parent_region_id, is_organization, mcn from region " +
                                " where parent_region_id = " + organizationId +
                                " select title, atm_id, region_id from atm " +
                                " where region_id in (select region_id from region where parent_region_id = " + organizationId + ");";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet atmsDS = new DataSet();
            adapter.Fill(atmsDS);

            //Adding Organization Node
            foreach (DataRow dbRow in atmsDS.Tables[0].Rows)
            {
                ComponentArt.Web.UI.TreeViewNode organizationNode = CreateRegionNode(dbRow["region_name"].ToString(), true, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
                UserATMsTreeView.Nodes.Add(organizationNode);

                //Adding Regions in Organization and ATMs in Region Node
                foreach (DataRow dbRegionRow in atmsDS.Tables[1].Rows)
                {
                    ComponentArt.Web.UI.TreeViewNode regionNode = CreateRegionNode(dbRegionRow["region_name"].ToString(), true, (int)dbRegionRow["region_id"], bool.Parse(dbRegionRow["is_organization"].ToString()));
                    organizationNode.Nodes.Add(regionNode);

                    foreach (DataRow dbATMRow in atmsDS.Tables[2].Select(" region_id = " + (int)dbRegionRow["region_id"]))
                    {
                        ComponentArt.Web.UI.TreeViewNode atmNode = CreateATMNode(dbATMRow["title"].ToString(), (int)dbATMRow["atm_id"]);
                        regionNode.Nodes.Add(atmNode);
                    }
                }
            }

        }
        protected ComponentArt.Web.UI.TreeViewNode CreateRegionNode(string text, bool expanded, int id, bool isOrganization)
        {
            ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();

            node.Text = text;
            node.ID = "r" + id;
            if (isOrganization)
                node.ImageUrl = "icon_organization.png";
            else
                node.ImageUrl = "icon_ATMRegionList.gif";
            node.DroppingEnabled = false;
            node.Expanded = expanded;
            node.ShowCheckBox = true;

            return node;
        }

        protected ComponentArt.Web.UI.TreeViewNode CreateATMNode(string text, int id)
        {
            ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();

            node.Text = text;
            node.ID = "a" + id;
            node.ImageUrl = "icon_atm.jpg";
            node.DroppingEnabled = false;
            node.ShowCheckBox = true;

            return node;
        }
    }
}