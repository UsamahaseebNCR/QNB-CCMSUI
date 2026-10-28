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

using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;

using ComponentArt.Web.UI;
using Newtonsoft.Json;

namespace CCMSUI.admin
{
    public partial class AddEditCIT : BasePage
    {
        //DataSet atmsDS;
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.SetFocus(TextBox_Name);
            if (Page.IsPostBack)
                return;

            
            for (int i = 0; i < 24; i++)
                DropDownList_DispatchHour.Items.Add(new ListItem(i.ToString(), i.ToString()));

            for (int i = 0; i < 60; i++)
                DropDownList_DispatchMin.Items.Add(new ListItem(i.ToString(), i.ToString()));

            //BuildTree();

            if (Request.QueryString["id"] == null)
            {
   
                if (!CheckRight((int)Permissions.CreateCIT))
                    Response.Redirect("AccessDenied.aspx");
                RadioButton_Active.SelectedIndex = 0;
            }
            else
            {
                if (!CheckRight((int)Permissions.ModifyCIT))
                    Response.Redirect("AccessDenied.aspx");

                CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(int.Parse(Request.QueryString["id"]));
                TextBox_Name.Text = ccmsCIT.Name;
                TextBox_Code.Text = ccmsCIT.Code;
                TextBox_Email.Text = ccmsCIT.EmailId;

                DropDownList_DispatchHour.SelectedValue = ccmsCIT.OrderDispatchTime.Value.Hour.ToString();
                DropDownList_DispatchMin.SelectedValue = ccmsCIT.OrderDispatchTime.Value.Minute.ToString();
                RadioButton_Active.SelectedIndex = ccmsCIT.IsActive.Value ? 0 : 1;
                //ArrayList regionList = new ArrayList();
                //RegionCit.RegionCitReader regionCITReader = RegionCit.ExecuteReader("cit_id = " + ccmsCIT.Id);
                //while (regionCITReader.Read())
                //{
                //    regionList.Add(regionCITReader.CurrentRegionCit.RegionId);

                //}
                //regionCITReader.Close();
                //ViewState["AssignedRegions"] = regionList;
                //if (regionList.Count > 0)
                //    for (int i = 0; i < regionTreeView.Nodes.Count; i++)
                //        SetCheck(regionTreeView.Nodes[i], regionList);
            }
        }
        //protected void SetCheck(TreeViewNode node, ArrayList selectedItem)
        //{
        //    TreeViewNodeCollection nodeCol = node.Nodes;
        //    int regions = 0;

        //    for (int counter = 0; counter < nodeCol.Count; counter++)
        //    {

        //        if (selectedItem.IndexOf(int.Parse(nodeCol[counter].ID.Substring(1))) >= 0)
        //        {
        //            nodeCol[counter].Checked = true;
        //            regions++;
        //        }

        //        if (nodeCol[counter].Nodes.Count > 0)
        //        {
        //            SetCheck(nodeCol[counter], selectedItem);
        //            //if (regions == nodeCol.Count && regions != 0)
        //            //    nodeCol[0].ParentNode.Checked = true;
        //            if (nodeCol[counter].Checked)
        //                regions++;
        //        }
        //        //else
        //        //{
        //        //    nodeCol[counter].Checked = true;
        //        //    regions++;
        //        //}
        //    }

        //    if (regions == nodeCol.Count && regions != 0)
        //        nodeCol[0].ParentNode.Checked = true;
        //}
        //protected void BuildTree()
        //{
        //    //regionTreeView.Nodes.Clear();

        //    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
        //    cmd.CommandText = @"select region_name, region_id, parent_region_id, is_organization, mcn from region where is_active = 1 order by region_name";
        //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //    atmsDS = new DataSet();
        //    adapter.Fill(atmsDS);
        //    atmsDS.Relations.Add("RegionRegionRelation", atmsDS.Tables[0].Columns["parent_region_id"], atmsDS.Tables[0].Columns["region_id"], false);

        //    foreach (DataRow dbRow in atmsDS.Tables[0].Rows)
        //    {
        //        if (dbRow.IsNull("parent_region_id"))
        //        {
        //            //ComponentArt.Web.UI.TreeViewNode newNode = CreateRegionNode(dbRow["region_name"].ToString(), true, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
        //            //regionTreeView.Nodes.Add(newNode);
        //            //PopulateSubTree(dbRow, newNode, false, false, dbRow["mcn"].ToString());
        //        }
        //    }


        //}

        //protected void PopulateSubTree(DataRow parentRegionRow, ComponentArt.Web.UI.TreeViewNode node, bool allowEditingRegion, bool allowEditingATM, string parentMCN)
        //{
        //    //ArrayList selectedOrganizations = new ArrayList();
        //    //foreach (System.Web.UI.WebControls.ListItem listItem in CheckBoxList_Organization.Items)
        //    //    if (listItem.Selected)
        //    //        selectedOrganizations.Add(listItem.Value);

        //    foreach (DataRow childRow in atmsDS.Tables[0].Select("parent_region_id=" + parentRegionRow["region_id"]))
        //    {
        //        //if (bool.Parse(childRow["is_organization"].ToString()))
        //        //{
        //        ComponentArt.Web.UI.TreeViewNode childNode = CreateRegionNode(childRow["region_name"].ToString(), false, (int)childRow["region_id"], bool.Parse(childRow["is_organization"].ToString()));
        //        node.Nodes.Add(childNode);
        //        PopulateSubTree(childRow, childNode, false, false, parentMCN);

        //        //                }
        //        //else
        //        //{
        //        //    if (selectedOrganizations.Contains(TraceOrganizationMCN(int.Parse(childRow["region_id"].ToString()))) || selectedOrganizations.Contains(parentMCN))
        //        //    {
        //        //        ComponentArt.Web.UI.TreeViewNode childNode = CreateRegionNode(childRow["region_name"].ToString(), false, (int)childRow["region_id"], bool.Parse(childRow["is_organization"].ToString()));
        //        //        node.Nodes.Add(childNode);
        //        //        PopulateSubTree(childRow, childNode, false, false, parentMCN);
        //        //    }
        //        //}
        //    }

        //    //foreach (DataRow childRow in parentRegionRow.GetChildRows("ATMRegionRelation"))
        //    //{
        //    //    ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(childRow["title"].ToString(), (int)childRow["atm_id"]);
        //    //    node.Nodes.Add(childNode);
        //    //}
        //}

        //protected ComponentArt.Web.UI.TreeViewNode CreateRegionNode(string text, bool expanded, int id, bool isOrganization)
        //{
        //    ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();

        //    node.Text = text;
        //    if (isOrganization)
        //    {
        //        node.ID = "o" + id;
        //        node.ImageUrl = "icon_organization.png";
        //    }
        //    else
        //    {
        //        node.ID = "r" + id;
        //        node.ImageUrl = "icon_ATMRegionList.gif";
        //    }

        //    node.DroppingEnabled = false;
        //    node.Expanded = expanded;
        //    node.ShowCheckBox = true;
        //    return node;
        //}


        protected void Button_SaveUpdateCIT_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddCIT");
            SqlConnection conn = null;
            SqlTransaction trx = null;
            CcmsCit cit;
            CcmsCit oldCit = null;
            try
            {

                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                if (Request.QueryString["id"] != null)
                {
                    oldCit = Avanza.CCMS.DAL.CcmsCit.LoadCcmsCitByPk(int.Parse(Request.QueryString["id"]));
                    cit = Avanza.CCMS.DAL.CcmsCit.LoadCcmsCitByPk(int.Parse(Request.QueryString["id"]));
                    cit.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    cit.ModifiedOn = DateTime.Now;
                }
                else
                {
                    cit = new CcmsCit();
                    cit.CreatedOn = DateTime.Now;
                    cit.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

                }
                // change by Wajahat on 27/6/2018
                ValidateInput();

                object res = ConnectionFactory.ExecuteScalar("select top 1 name from ccms_cit where name='" + TextBox_Name.Text + "' or code = '" + TextBox_Code.Text + "' or email_id= '"+ TextBox_Email.Text + "' "
                                           + ((Request.QueryString["id"] != null) ? "and id != " + cit.Id : ""));
                if (res != null)
                {
                    Literal_error.Text = "<li>This detail is already defined for cit '" + res + "'. Please, provide different details.</li>";
                    return;
                }

                cit.Name = TextBox_Name.Text;
                cit.Code = TextBox_Code.Text;
                cit.EmailId = TextBox_Email.Text;
                cit.OrderDispatchTime = new DateTime(1900, 1, 1, int.Parse(DropDownList_DispatchHour.SelectedValue), int.Parse(DropDownList_DispatchMin.SelectedValue), 0);
                cit.IsActive = (RadioButton_Active.SelectedIndex == 0) ? true : false;
                cit.IsDeleted = false;

                cit.Save(conn, trx);
                ConnectionFactory.ExecuteScalar("delete region_cit where cit_id = " + cit.Id, trx);
                //foreach (ComponentArt.Web.UI.TreeViewNode node in regionTreeView.CheckedNodes)
                //{
                //    //if (node.ID.Contains("r"))
                //    //{
                //    RegionCit regionCIT = new RegionCit();
                //    regionCIT.RegionId = int.Parse(node.ID.Substring(1));
                //    regionCIT.CitId = cit.Id;
                //    regionCIT.Save(conn, trx);
                //    //}
                //}

                //RemoveCITUserNotifications(cit.Id, conn, trx);
                //RemoveCITUserAtms(cit.Id, conn, trx);
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
                    Utility.BuildAuditLog("CIT " + cit.Name + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateCIT);
                else
                    editAuditLog(cit, oldCit, trx);
                    //Utility.BuildAuditLog("CIT "+ cit.Name + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyCIT);

                if (trx != null)
                {
                    trx.Commit();
                }

                Response.Redirect("cit.aspx", false);


            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }

                Literal_error.Text = "<li>An error occured while saving CIT." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        public void editAuditLog(Avanza.CCMS.DAL.CcmsCit objCurrent, Avanza.CCMS.DAL.CcmsCit objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Organization " + objPrevious.Name + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyCIT, trx);
            }

        }

        //        private void RemoveCITUserAtms(long citId, SqlConnection conn, SqlTransaction trx)
        //        {

        //            ArrayList previousSelectedRegions = (ArrayList)ViewState["AssignedRegions"];
        //            if (previousSelectedRegions == null || previousSelectedRegions.Count == 0)
        //                return;

        //            DataTable dtCitUsers = GetCITUsers(citId, conn, trx);
        //            string strCitUserIds = "";
        //            foreach (DataRow drCitUser in dtCitUsers.Rows)
        //            {
        //                strCitUserIds += drCitUser["user_id"].ToString() + ",";
        //            }

        //            if (strCitUserIds == String.Empty)
        //                return;

        //            strCitUserIds = strCitUserIds.Substring(0, strCitUserIds.Length - 1);

        ////            for (int index = 0; index < previousSelectedRegions.Count; index++)
        ////            {
        ////                int regionId = (int)previousSelectedRegions[index];

        ////                if (IsRegionRevoked(regionId))
        ////                {
        ////                    ConnectionFactory.ExecuteQuery(String.Format(@"delete from user_atms
        ////                                                    where atm_id in (select atm_id from atm where region_id = {0})
        ////                                                    and user_id in ({1})", regionId, strCitUserIds), trx);
        ////                }
        ////            }


        //        }

        //private bool IsRegionRevoked(int regionId)
        //{
        //    bool isRevoked = false;
        //    TreeViewNode searchNode = regionTreeView.FindNodeById("r" + regionId);
        //    if (searchNode != null)
        //    {
        //        if (!searchNode.IsSelected)
        //            isRevoked = true;
        //    }

        //    return isRevoked;
        //}

        // Remove organization from "CIT User(s)" notification list if CIT is revoked permissions on that organization
        private void RemoveCITUserNotifications(long citId, SqlConnection conn, SqlTransaction trx)
        {
            DataTable dtOrganizations = GetCITAssignedOrganizations(citId, conn, trx);
            DataTable dtUsers = GetCITUsers(citId, conn, trx);

            foreach (DataRow drUser in dtUsers.Rows)
            {
                int userId = (int)drUser["user_id"];
                DataTable dtUserOrganizations = GetUserOrganizations(userId, conn, trx);

                foreach (DataRow drUserOrganization in dtUserOrganizations.Rows)
                {
                    int userOrganizationId = (int)drUserOrganization["region_id"];
                    String filterExpression = String.Format("region_id = {0}", userOrganizationId);
                    DataRow[] filteredRows = dtOrganizations.Select(filterExpression);

                    if (filteredRows.Length == 0)
                    {
                        ConnectionFactory.ExecuteQuery(String.Format(@"delete from ccms_alert_notification where organization_id={0}
                                                                       and user_id={1}", userOrganizationId, userId), trx);
                    }
                }
            }

        }

        private DataTable GetUserOrganizations(long userId, SqlConnection conn, SqlTransaction trx)
        {
            DataTable dtUserOrganizations = new DataTable();

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.Connection = conn;
            cmd.Transaction = trx;
            cmd.CommandText = String.Format(@"select region_id, region_name from region
                                inner join ccms_alert_notification on region.region_id = ccms_alert_notification.organization_id
                                where user_id = {0}", userId);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtUserOrganizations);

            return dtUserOrganizations;
        }

        private DataTable GetCITUsers(long citId, SqlConnection conn, SqlTransaction trx)
        {
            DataTable dtUsers = new DataTable();

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.Connection = conn;
            cmd.Transaction = trx;
            cmd.CommandText = String.Format(@"select * from app_user inner join ccms_user_cit
                                                on app_user.user_id = ccms_user_cit.user_id
                                                where ccms_user_cit.cit_id = {0}", citId);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtUsers);

            return dtUsers;
        }

        private DataTable GetCITAssignedOrganizations(long citId, SqlConnection conn, SqlTransaction trx)
        {
            DataTable dtOrganizations = new DataTable();

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.Connection = conn;
            cmd.Transaction = trx;
            cmd.CommandText = String.Format(@"select distinct a.region_id, a.region_name from region a
                                        inner join region b on a.region_id = b.parent_region_id
                                        inner join region_cit on b.region_id = region_cit.region_id
                                        where region_cit.cit_id = {0}", citId);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtOrganizations);

            return dtOrganizations;
        }

    

        public void ValidateInput()
        {
            try
            {
                //bool result = true;
                bool isNameContainSpecialChars = CheckSpecialChars(TextBox_Name.Text);
                if (isNameContainSpecialChars == true)
                {
                    throw new Exception("Name contain invalid character");
                    //return result = false;
                }
                bool isCodeContainSpecialChars = CheckSpecialChars(TextBox_Code.Text);
                if (isCodeContainSpecialChars == true)
                {
                    throw new Exception("Code contain invalid character");
                    //return result = false;
                }
                //return result;
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("Err", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = ex.Message;
            }

        }
    }
}