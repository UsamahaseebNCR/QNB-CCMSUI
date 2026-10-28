using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Text;

using Avanza.CCMS.DAL;
using Newtonsoft.Json;

namespace Avanza.iSuite.UI
{
    public partial class AddEditGroup : BasePage
    {
        private static String SELECTED_ORGANIZATION = "SelectedOrganization";
        string enablemakeachecker = System.Web.Configuration.WebConfigurationManager.AppSettings["enablemakeachecker"];
        private string GetUserRightIDs()
        {
            StringBuilder builder = new StringBuilder();
            DataTable dt = (DataTable)Session[SessionVars.dtUserRights.ToString()];
            foreach (DataRow dr in dt.Rows)
                builder.Append(dr[1] + ",");
            builder.Append("-1");
            return builder.ToString();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new EventHandler(refresh_ServerClick);
        }

        void refresh_ServerClick(object sender, EventArgs e)
        {
            SqlCommand cmd = null;
            try
            {
                if (Request.QueryString["group_id"] != null)
                {
                    Groups currentGroup = Groups.LoadGroupsByPk(int.Parse(Request.QueryString["group_id"].ToString()));
                    if (currentGroup != null && currentGroup.OrganizationId > 0)
                    {
                        Session[SELECTED_ORGANIZATION] = currentGroup.OrganizationId;
                        Region region = Region.LoadRegionByPk(currentGroup.OrganizationId.Value);
                        Label_SelectedOrganization.Text = region.RegionName;
                        return;
                    }
                }
                if (Session["selectedOrgID"] != null)
                {
                    Region region = Region.LoadRegionByPk(int.Parse(Session["selectedOrgID"].ToString()));
                    if (region != null)
                    {
                        Label_SelectedOrganization.Text = region.RegionName;
                        Session[SELECTED_ORGANIZATION] = region.RegionId;
                    }
                }
                //Changed on 4/6/2015

                //string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
                //if (regionID != null
                //    && !regionID.StartsWith("1,"))
                //{
                //    Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
                //    if (region.IsOrganization)
                //    {
                //        Label_SelectedOrganization.Text = region.RegionName;
                //        Session[SELECTED_ORGANIZATION] = region.RegionId.ToString();
                //    }
                //    else
                //    {
                //        cmd = ConnectionFactory.GetNewCommand(true);

                //        cmd.CommandText = String.Format("select b.region_id,b.region_name from region a inner join region b  "
                //                                        + " on a.parent_region_id = b.region_id where a.region_id = {0}", region.RegionId);

                //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //        DataTable dtRegion = new DataTable();
                //        adapter.Fill(dtRegion);

                //        if (dtRegion != null && dtRegion.Rows.Count > 0)
                //        {
                //            Label_SelectedOrganization.Text = dtRegion.Rows[0]["region_name"].ToString();
                //            Session[SELECTED_ORGANIZATION] = dtRegion.Rows[0]["region_id"].ToString();
                //        }
                //        else
                //        {
                //            Label_SelectedOrganization.Text = String.Empty;
                //            Session[SELECTED_ORGANIZATION] = null;
                //        }

                //        cmd.Connection.Close();
                //        adapter.Dispose();
                //    }

                //}
                //else
                //{
                //    Label_SelectedOrganization.Text = String.Empty;
                //    Session[SELECTED_ORGANIZATION] = null;
                //}
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("refresh_ServerClick", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.SetFocus(TextBox_GroupName);
            if (Page.IsPostBack)
                return;

            refresh_ServerClick(this, null);
            DropDownList_Group_Type.DataSource = GetGroupTypes();
            DropDownList_Group_Type.DataTextField = "Group_Type";
            DropDownList_Group_Type.DataValueField = "Group_Type";
            DropDownList_Group_Type.DataBind();

            if (Request.QueryString["group_id"] == null && Session["group"] == null)
            {
                if (!CheckRight((int)Permissions.CreateGroup))
                    Response.Redirect("AccessDenied.aspx");
                DropDownList_Group_Type_SelectedIndexChanged(sender, null);
            }
            else if (Session["group"] != null)
            {
                if (!CheckRight((int)Permissions.ApproveGroup))
                    Response.Redirect("AccessDenied.aspx");
                GetGroupForEditing();
            }
            else
            {
                if (!CheckRight((int)Permissions.ModifyGroup))
                    Response.Redirect("AccessDenied.aspx");
                DropDownList_Group_Type.Enabled = false;
                GetGroupForEditing();
            }
        }

        private void GetGroupForEditing()
        {
            LogableTask task = LogableTask.NewTask("AddGroup");
            SqlCommand cmd = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                Groups group = null;
                GroupsApprover groups_approver = null;
                if (Request.QueryString["group_id"] != null)
                {
                    group = Groups.LoadGroupsByPk(int.Parse(Request.QueryString["group_id"]));
                    TextBox_GroupName.Text = group.GroupName;
                    TextBox_GroupDescription.Text = group.Description;
                    DropDownList_Group_Type.SelectedValue = group.EntityType;
                    TextBox_GroupEmail.Text = group.GroupEmail;
                    CheckBox_IndividualAlert.Checked = group.SendIndividualAlert.Value;

                    Region organization = Region.LoadRegionByPk(group.OrganizationId.Value);
                    if (organization != null)
                    {
                        Label_SelectedOrganization.Text = organization.RegionName;
                        Session[SELECTED_ORGANIZATION] = organization.RegionId;
                    }

                    cmd.CommandText = "select rights.right_id, name,group_id,right_type,rights.entity_type from rights "
                                        + "left join group_rights on (rights.right_id = group_rights.right_id "
                                        + "and group_rights.group_id =" + Request.QueryString["group_id"] + ") "
                                        + "where rights.entity_type = '" + group.EntityType + "' order by name ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (int.Parse(reader["right_id"].ToString()) > 0)
                        {
                            ListItem li = new ListItem(reader["name"].ToString(), reader["right_id"].ToString());
                            if (!reader.IsDBNull(2))
                                li.Selected = true;
                            if (reader.GetString(3) == "Administration")
                                CheckBoxList_GroupRights_Administration.Items.Add(li);
                            else if (reader.GetString(3) == "Operations")
                                CheckBoxList_GroupRights_Operations.Items.Add(li);
                            else if (reader.GetString(3) == "Reports")
                                CheckBoxList_GroupRights_Reports.Items.Add(li);
                        }
                    }
                    reader.Close();

                    Button_ApproveGroup.Visible = false;
                    Button_DeclineGroup.Visible = false;
                    Button_SaveUpdateGroup.Visible = true;
                    Button_Cancel.Visible = true;
                    TextBox_GroupDescription.Enabled = true;
                    TextBox_GroupEmail.Enabled = true;
                    TextBox_GroupName.Enabled = true;
                    CheckBox_IndividualAlert.Enabled = true;
                    CheckBoxList_GroupRights_Administration.Enabled = true;
                    CheckBoxList_GroupRights_Operations.Enabled = true;
                    CheckBoxList_GroupRights_Reports.Enabled = true;
                }
                else if (Session["group"] != null)
                {
                    groups_approver = GroupsApprover.LoadAppUseApproverrByGroupName(Session["group"].ToString());
                    Groups _group = Groups.LoadGroupsByPk(groups_approver.GroupId);

                    TextBox_GroupName.Text = groups_approver.GroupName;
                    TextBox_GroupDescription.Text = groups_approver.Description;
                    DropDownList_Group_Type.SelectedValue = groups_approver.EntityType;
                    TextBox_GroupEmail.Text = groups_approver.GroupEmail;
                    CheckBox_IndividualAlert.Checked = groups_approver.SendIndividualAlert.Value;

                    Region organization = Region.LoadRegionByPk(groups_approver.OrganizationId.Value);
                    if (organization != null)
                    {
                        Label_SelectedOrganization.Text = organization.RegionName;
                        Session[SELECTED_ORGANIZATION] = organization.RegionId;
                    }

                    cmd.CommandText = "select rights.right_id, name,group_id,right_type,rights.entity_type from rights "
                                        + "left join group_rights_approver on (rights.right_id = group_rights_approver.right_id "
                                        + "and group_rights_approver.group_id =" + _group.GroupId + ") "
                                        + "where rights.entity_type = '" + groups_approver.EntityType + "' order by name ";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (int.Parse(reader["right_id"].ToString()) > 0)
                        {
                            ListItem li = new ListItem(reader["name"].ToString(), reader["right_id"].ToString());
                            if (!reader.IsDBNull(2))
                                li.Selected = true;
                            if (reader.GetString(3) == "Administration")
                                CheckBoxList_GroupRights_Administration.Items.Add(li);
                            else if (reader.GetString(3) == "Operations")
                                CheckBoxList_GroupRights_Operations.Items.Add(li);
                            else if (reader.GetString(3) == "Reports")
                                CheckBoxList_GroupRights_Reports.Items.Add(li);
                        }
                    }
                    reader.Close();
                    Button_ApproveGroup.Visible = true;
                    Button_DeclineGroup.Visible = true;
                    Button_SaveUpdateGroup.Visible = false;
                    Button_Cancel.Visible = false;
                    TextBox_GroupDescription.Enabled = false;
                    TextBox_GroupEmail.Enabled = false;
                    TextBox_GroupName.Enabled = false;
                    CheckBox_IndividualAlert.Enabled = false;
                    CheckBoxList_GroupRights_Administration.Enabled = false;
                    CheckBoxList_GroupRights_Operations.Enabled = false;
                    CheckBoxList_GroupRights_Reports.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally 
            { 
                task.EndTask(); cmd.Connection.Close();
                if (Session["group"] != null)
                {
                    string tempSession = Session["group"].ToString();
                    Session["groupAction"] = tempSession;
                    Session["group"] = null;
                }
            }
        }

        protected void Button_SaveUpdate_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddGroup");
            SqlCommand cmd = null;
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                if (TextBox_GroupName.Text == String.Empty)
                {
                    throw new Exception("Please provide group name.");
                }
                                
                if (Label_SelectedOrganization.Text == String.Empty || Session[SELECTED_ORGANIZATION] == null)
                {
                    throw new Exception("Please select organization from the tree.");
                }

                if (!CheckBox_IndividualAlert.Checked && String.IsNullOrEmpty(TextBox_GroupEmail.Text))
                {
                    throw new Exception("Please provide group email address.");
                }
                bool isGroupNameContainSpecialChars = CheckSpecialChars(TextBox_GroupName.Text);
                if (isGroupNameContainSpecialChars == true)
                {
                    throw new Exception("group Name contain invalid character</img> </li>");
                }
                bool isGroupEmailContainSpecialChars = CheckSpecialChars(TextBox_GroupEmail.Text);
                if (isGroupEmailContainSpecialChars == true)
                {
                    throw new Exception("group Email contain invalid character</img> </li>");
                }
                bool isGroupDescriptionContainSpecialChars = CheckSpecialChars(TextBox_GroupDescription.Text);
                if (isGroupDescriptionContainSpecialChars == true)
                {
                    throw new Exception("group Description contain invalid character</img> </li>");
                }

                Groups newGroup = null;
                GroupsApprover newGroupApprover = null;
                if (enablemakeachecker == "false")
                {
                    Groups oldGroup = null;
                    newGroup = new Groups();
                    AppUser currentUser = ((AppUser)Session[SessionVars.user.ToString()]);
                    string whereClause = "GROUP_NAME='" + TextBox_GroupName.Text + "'";

                    if (Request.QueryString["group_id"] != null)
                    {
                        newGroup = Groups.LoadGroupsByPk(int.Parse(Request.QueryString["group_id"]));
                        oldGroup = Groups.LoadGroupsByPk(int.Parse(Request.QueryString["group_id"]));
                        whereClause += " and group_id <> " + newGroup.GroupId;
                    }
                    
                    if (Groups.LoadGroups(whereClause) != null)
                        Literal_error.Text = "<li>The group name '" + TextBox_GroupName.Text + "' already exists!";
                    else
                    {
                        newGroup.GroupName = TextBox_GroupName.Text;
                        newGroup.Description = TextBox_GroupDescription.Text;
                        newGroup.EntityType = DropDownList_Group_Type.SelectedValue;
                        newGroup.CreatedBy = currentUser.UserId;
                        newGroup.OrganizationId = int.Parse(Session[SELECTED_ORGANIZATION].ToString());
                        newGroup.GroupEmail = TextBox_GroupEmail.Text;
                        newGroup.SendIndividualAlert = CheckBox_IndividualAlert.Checked;
                        newGroup.Save();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Group added");

                        cmd = ConnectionFactory.GetNewCommand(true);
                        cmd.CommandText = "delete from group_rights where group_id = " + newGroup.GroupId;
                        cmd.ExecuteNonQuery();
                        foreach (System.Web.UI.WebControls.ListItem right in this.CheckBoxList_GroupRights_Administration.Items)
                        {
                            if (right.Selected)
                            {
                                if (Request.QueryString["group_id"] == null)
                                    (new GroupRights(
                                        newGroup.GroupId,
                                        int.Parse(right.Value)))
                                        .Save();
                                else
                                    (new GroupRights(
                                    int.Parse(Request.QueryString["group_id"]),
                                    int.Parse(right.Value)))
                                    .Save();
                            }
                        }
                        foreach (System.Web.UI.WebControls.ListItem right in this.CheckBoxList_GroupRights_Operations.Items)
                        {
                            if (right.Selected)
                            {
                                if (Request.QueryString["group_id"] == null)
                                    (new GroupRights(
                                        newGroup.GroupId,
                                        int.Parse(right.Value)))
                                        .Save();
                                else
                                    (new GroupRights(
                                    int.Parse(Request.QueryString["group_id"]),
                                    int.Parse(right.Value)))
                                    .Save();
                            }
                        }

                        foreach (System.Web.UI.WebControls.ListItem right in this.CheckBoxList_GroupRights_Reports.Items)
                        {
                            if (right.Selected)
                            {
                                if (Request.QueryString["group_id"] == null)
                                    (new GroupRights(
                                        newGroup.GroupId,
                                        int.Parse(right.Value)))
                                        .Save();
                                else
                                    (new GroupRights(
                                    int.Parse(Request.QueryString["group_id"]),
                                    int.Parse(right.Value)))
                                    .Save();
                            }
                        }

                        cmd.Connection.Close();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "rights for the group added");

                        Session[SessionVars.notice.ToString()] = "Group [" + newGroup.GroupName + "] was saved successfully";
                        if (Request.QueryString["group_id"] == null)
                            Utility.BuildAuditLog("Group [" + newGroup.GroupName + "] was created successfully", currentUser.UserId, (int)Permissions.CreateGroup);
                        else
                            editAuditLog(newGroup, oldGroup, trx);
                        //Utility.BuildAuditLog("Group [" + newGroup.GroupName + "] was modified successfully", currentUser.UserId, (int)Permissions.ModifyGroup);
                        if (trx != null)
                        {
                            trx.Commit();
                        }
                        Response.Redirect("GroupUser.aspx", false);
                    }
                }
                else
                {
                    AppUser currentUser = ((AppUser)Session[SessionVars.user.ToString()]);
                    string whereClause = "GROUP_NAME='" + TextBox_GroupName.Text + "'";

                    newGroupApprover = new GroupsApprover();

                    if (Request.QueryString["group_id"] != null)
                    {
                        newGroup = Groups.LoadGroupsByPk(int.Parse(Request.QueryString["group_id"]));
                        whereClause += " and group_id <> " + newGroup.GroupId;

                        newGroupApprover.IsAdded = false;
                        newGroupApprover.IsDeleted = false;
                        newGroupApprover.IsEditied = true;
                        newGroupApprover.Status = "Pending";

                    }
                    else
                    {
                        newGroupApprover.IsAdded = true;
                        newGroupApprover.IsDeleted = false;
                        newGroupApprover.IsEditied = false;
                        newGroupApprover.Status = "Pending";
                    }
                    if (Groups.LoadGroups(whereClause) != null)
                        Literal_error.Text = "<li>The group name '" + TextBox_GroupName.Text + "' already exists!";
                    else
                    {
                        newGroupApprover.GroupName = TextBox_GroupName.Text;
                        newGroupApprover.Description = TextBox_GroupDescription.Text;
                        newGroupApprover.EntityType = DropDownList_Group_Type.SelectedValue;
                        newGroupApprover.CreatedBy = currentUser.UserId;
                        newGroupApprover.OrganizationId = int.Parse(Session[SELECTED_ORGANIZATION].ToString());
                        newGroupApprover.GroupEmail = TextBox_GroupEmail.Text;
                        newGroupApprover.SendIndividualAlert = CheckBox_IndividualAlert.Checked;


                        if (newGroupApprover.IsAdded == true)
                        {
                            newGroup = new Groups();
                            newGroup.GroupName = newGroupApprover.GroupName;
                            newGroup.Description = newGroupApprover.Description;
                            newGroup.EntityType = newGroupApprover.EntityType;
                            newGroup.CreatedBy = newGroupApprover.CreatedBy;
                            newGroup.OrganizationId = newGroupApprover.OrganizationId;
                            newGroup.GroupEmail = newGroupApprover.GroupEmail;
                            newGroup.SendIndividualAlert = newGroupApprover.SendIndividualAlert;
                            newGroup.IsAdded = true;
                            newGroup.IsDeleted = false;
                            newGroup.IsEditied = false;
                            newGroup.Status = "Pending";
                            newGroup.Save();
                        }

                        if (newGroupApprover.IsEditied == true)
                        {
                            newGroup.IsAdded = false;
                            newGroup.IsDeleted = false;
                            newGroup.IsEditied = true;
                            newGroup.Status = "Pending";
                            newGroup.Save();
                        }

                        newGroupApprover.GroupId = newGroup.GroupId;
                        newGroupApprover.Save();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Group added");

                        cmd = ConnectionFactory.GetNewCommand(true);
                        cmd.CommandText = "delete from group_rights_approver where group_id = " + newGroup.GroupId;
                        cmd.ExecuteNonQuery();
                        foreach (System.Web.UI.WebControls.ListItem right in this.CheckBoxList_GroupRights_Administration.Items)
                        {
                            if (right.Selected)
                            {
                                if (Request.QueryString["group_id"] == null)
                                    (new GroupRightsApprover(
                                        newGroup.GroupId,
                                        int.Parse(right.Value)))
                                        .Save();
                                else
                                    (new GroupRightsApprover(
                                    int.Parse(Request.QueryString["group_id"]),
                                    int.Parse(right.Value)))
                                    .Save();
                            }
                        }
                        foreach (System.Web.UI.WebControls.ListItem right in this.CheckBoxList_GroupRights_Operations.Items)
                        {
                            if (right.Selected)
                            {
                                if (Request.QueryString["group_id"] == null)
                                    (new GroupRightsApprover(
                                        newGroup.GroupId,
                                        int.Parse(right.Value)))
                                        .Save();
                                else
                                    (new GroupRightsApprover(
                                    int.Parse(Request.QueryString["group_id"]),
                                    int.Parse(right.Value)))
                                    .Save();
                            }
                        }

                        foreach (System.Web.UI.WebControls.ListItem right in this.CheckBoxList_GroupRights_Reports.Items)
                        {
                            if (right.Selected)
                            {
                                if (Request.QueryString["group_id"] == null)
                                    (new GroupRightsApprover(
                                        newGroup.GroupId,
                                        int.Parse(right.Value)))
                                        .Save();
                                else
                                    (new GroupRightsApprover(
                                    int.Parse(Request.QueryString["group_id"]),
                                    int.Parse(right.Value)))
                                    .Save();
                            }
                        }

                        cmd.Connection.Close();
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "rights for the group added");

                        Session[SessionVars.notice.ToString()] = "Group [" + newGroup.GroupName + "] was saved successfully and request sent for approval.";
                        if (Request.QueryString["group_id"] == null)
                            Utility.BuildAuditLog("Request to create group [" + newGroup.GroupName + "] has been sent for approval.", currentUser.UserId, (int)Permissions.CreateGroup);
                        else
                            Utility.BuildAuditLog("Request to modify group [" + newGroup.GroupName + "] has been sent for approval.", currentUser.UserId, (int)Permissions.ModifyGroup);
                        Response.Redirect("GroupUser.aspx", false);
                    }
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                if (ex.Message.Contains("invalid"))
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    Literal_error.Text = "<li>Group was not saved! " + ex.Message + "</li>";
                }
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask();
            }
        }

        public void editAuditLog(Avanza.CCMS.DAL.Groups objCurrent, Avanza.CCMS.DAL.Groups objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Group [" + objPrevious.GroupName + "] was modified successfully", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyGroup, trx);
            }

        }

        protected void DropDownList_Group_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGroupType = String.Empty;

            if (!IsPostBack)
            {
                AppUser currentUser = (AppUser)Session[SessionVars.user.ToString()];
                if (currentUser != null)
                {
                    if (!currentUser.UserType.Equals("NCR"))
                    {
                        DropDownList_Group_Type.SelectedValue = currentUser.UserType;
                        DropDownList_Group_Type.Enabled = false;
                    }
                }
            }

            selectedGroupType = DropDownList_Group_Type.SelectedValue;

            Rights.RightsReader rightsReader = Rights.ExecuteReader("right_id > 0 and right_type='Administration' and entity_type='" + selectedGroupType + "' order by name");
            CheckBoxList_GroupRights_Administration.DataSource = rightsReader;
            CheckBoxList_GroupRights_Administration.DataTextField = "Name";
            CheckBoxList_GroupRights_Administration.DataValueField = "RightId";
            CheckBoxList_GroupRights_Administration.DataBind();
            rightsReader.Close();

            rightsReader = Rights.ExecuteReader("right_id > 0 and right_type='Operations' and entity_type='" + selectedGroupType + "' order by name");
            CheckBoxList_GroupRights_Operations.DataSource = rightsReader;
            CheckBoxList_GroupRights_Operations.DataTextField = "Name";
            CheckBoxList_GroupRights_Operations.DataValueField = "RightId";
            CheckBoxList_GroupRights_Operations.DataBind();
            rightsReader.Close();

            rightsReader = Rights.ExecuteReader("right_id > 0 and right_type='Reports' and entity_type='" + selectedGroupType + "' order by name");
            CheckBoxList_GroupRights_Reports.DataSource = rightsReader;
            CheckBoxList_GroupRights_Reports.DataTextField = "Name";
            CheckBoxList_GroupRights_Reports.DataValueField = "RightId";
            CheckBoxList_GroupRights_Reports.DataBind();
            rightsReader.Close();
        }

        private DataTable GetGroupTypes()
        {
            DataRow groupTypeRow = null;

            DataTable dtGroupTypes = new DataTable();
            dtGroupTypes.Columns.Add("Group_Type", typeof(String));

            groupTypeRow = dtGroupTypes.NewRow();
            groupTypeRow["Group_Type"] = "NCR";

            dtGroupTypes.Rows.Add(groupTypeRow);

            groupTypeRow = dtGroupTypes.NewRow();
            groupTypeRow["Group_Type"] = "BANK";

            dtGroupTypes.Rows.Add(groupTypeRow);

            groupTypeRow = dtGroupTypes.NewRow();
            groupTypeRow["Group_Type"] = "CIT";

            dtGroupTypes.Rows.Add(groupTypeRow);

            return dtGroupTypes;
        }

        protected void Button_ApproveGroup_Click(object sender, EventArgs e)
        {

            SqlTransaction trx = null;
            SqlCommand cmd = null;

            GroupsApprover approveGroup = GroupsApprover.LoadAppUseApproverrByGroupName(Session["groupAction"].ToString());
            Groups groups = Groups.LoadGroupsByPk(approveGroup.GroupId);
            Groups oldgroups = Groups.LoadGroupsByPk(approveGroup.GroupId);
            string msg = null;
            try
            {
                if (groups.IsDeleted == true)
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trx = cmd.Connection.BeginTransaction();
                    cmd.Transaction = trx;

                    cmd.CommandText = "Delete_group";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@group_id", SqlDbType.Int);
                    cmd.Parameters[0].Value = groups.GroupId;
                    cmd.ExecuteNonQuery();
                    trx.Commit();

                    ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_id = '" + groups.GroupId + "'");
                    ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights_approver where group_id = " + groups.GroupId);

                    Utility.BuildAuditLog(groups.GroupName + "is Deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteGroup);
                    msg = "Request to delete group [" + groups.GroupName + "] has been approved successfully.";
                }
                if (groups.IsEditied == true)
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trx = cmd.Connection.BeginTransaction();
                    cmd.Transaction = trx;
                    groups.GroupName = approveGroup.GroupName;
                    groups.Description = approveGroup.Description;
                    groups.EntityType = approveGroup.EntityType;
                    groups.CreatedBy = approveGroup.CreatedBy;
                    groups.OrganizationId = approveGroup.OrganizationId;
                    groups.GroupEmail = approveGroup.GroupEmail;
                    groups.SendIndividualAlert = approveGroup.SendIndividualAlert;
                    groups.IsAdded = false;
                    groups.IsDeleted = false;
                    groups.IsEditied = false;
                    groups.Status = "Approved";
                    groups.Save();
                    editAuditLog(groups, oldgroups, trx);
                    trx.Commit();
                    ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights where group_id = " + groups.GroupId);
                    ConnectionFactory.ExecuteQuery("insert into Group_rights select * from Group_rights_approver where group_id = " + groups.GroupId);
                    ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights_approver where group_id = " + groups.GroupId);
                    ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_id = '" + groups.GroupId + "'");
                    msg = "Request to update group [" + groups.GroupName + "] has been approved successfully.";
                }
                if (groups.IsAdded == true)
                {
                    groups.IsAdded = false;
                    groups.IsDeleted = false;
                    groups.IsEditied = false;
                    groups.Status = "Approved";
                    groups.Save();

                    ConnectionFactory.ExecuteQuery("insert into Group_rights select * from Group_rights_approver where group_id = " + groups.GroupId);
                    ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights_approver where group_id = " + groups.GroupId);
                    ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_id = '" + groups.GroupId + "'");
                    msg = "Request to create group [" + groups.GroupName + "] has been approved successfully.";
                }
                Session[SessionVars.notice.ToString()] = msg;
                Utility.BuildAuditLog(msg, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ApproveGroup);
                Response.Redirect("ApproveGroup.aspx",false);
            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();
                Literal_error.Text = "<li>An error occured while Approving Group! " + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("Approve group", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally {
                Session["group"] = null; 
                if (cmd != null) cmd.Connection.Close();
            }
        }

        protected void Button_DeclineGroup_Click(object sender, EventArgs e)
        {
            SqlTransaction trx = null;
            SqlCommand cmd = null;

            GroupsApprover approveGroup = GroupsApprover.LoadAppUseApproverrByGroupName(Session["groupAction"].ToString());
            Groups groups = Groups.LoadGroupsByPk(approveGroup.GroupId);
            string msg = null;
            try
            {
                if (groups.IsDeleted == true)
                {
                    groups.Status = "Decline";
                    groups.IsDeleted = false;
                    groups.Save();

                    ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights_approver where group_id = " + groups.GroupId);
                    ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_id = '" + groups.GroupId + "'");
                    msg = "Request to delete group [" + groups.GroupName + "] has been declined.";
                }
                if (groups.IsAdded == true)
                {
                    //ConnectionFactory.ExecuteQuery("insert into Group_rights select * from Group_rights_approver where group_id = " + groups.GroupId);
                    ConnectionFactory.ExecuteQuery("DELETE FROM Group_rights_approver where group_id = " + groups.GroupId);
                    ConnectionFactory.ExecuteQuery("DELETE FROM Groups where group_id  = '" + groups.GroupId + "'");
                    ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_id = '" + groups.GroupId + "'");
                    msg = "Request to create group [" + groups.GroupName + "] has been declined.";
                }
                if (groups.IsEditied == true)
                {
                    groups.Status = "Decline";
                    groups.IsEditied = false;
                    groups.Save();

                    ConnectionFactory.ExecuteQuery("DELETE FROM groups_approver where group_id = '" + groups.GroupId + "'");
                    ConnectionFactory.ExecuteQuery("delete from Group_rights_approver where group_id  = '" + groups.GroupId + "'");
                    msg = "Request to update group [" + groups.GroupName + "] has been declined.";
                }
                Session[SessionVars.notice.ToString()] = msg;
                Utility.BuildAuditLog(msg, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ApproveGroup);
                Response.Redirect("ApproveGroup.aspx", false);
            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();
                Literal_error.Text = "<li>An error occured while Approving Group! " + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("Approve group", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally { Session["group"] = null; 
                
                if (cmd != null) cmd.Connection.Close(); }
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("GroupUser.aspx", false);

        }

     
    }
}