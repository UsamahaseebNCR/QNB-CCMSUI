using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;
using Encryption;

using ComponentArt.Web.UI;
using Avanza.CCMS.DAL;
using System.Collections.Generic;
using System.Linq;
using Avanza.iSuite.DAL;
//using Avanza.iSuite.DAL;

namespace Avanza.iSuite.UI
{
    public partial class AssignATMsToADUsers : BasePage
    {
        bool isTreeviewNodeExpanded = System.Web.Configuration.WebConfigurationManager.AppSettings["isTreeviewNodeExpanded"] == "1" ? true : false;
        string enablemakeachecker = System.Web.Configuration.WebConfigurationManager.AppSettings["enablemakeachecker"];
        StringBuilder jsStr = new StringBuilder(5000);
        StringBuilder jsStrLocation = new StringBuilder(5000);
        private DataTable GetUserTypes()
        {
            DataRow userTypeRow = null;

            DataTable dtUserTypes = new DataTable();
            dtUserTypes.Columns.Add("User_Type", typeof(String));

            userTypeRow = dtUserTypes.NewRow();
            userTypeRow["User_Type"] = "NCR";

            dtUserTypes.Rows.Add(userTypeRow);

            userTypeRow = dtUserTypes.NewRow();
            userTypeRow["User_Type"] = "BANK";

            dtUserTypes.Rows.Add(userTypeRow);

            userTypeRow = dtUserTypes.NewRow();
            userTypeRow["User_Type"] = "CIT";

            dtUserTypes.Rows.Add(userTypeRow);

            return dtUserTypes;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            if (!Page.IsPostBack)
            {
                try
                {

                    if (!CheckRight((int)Permissions.AddActiveDirectoryUsers))
                        Response.Redirect("AccessDenied.aspx");
                    DataTable dtADUsers = (DataTable)Session[SessionVars.dtAppUser.ToString()];

                    for (int counter = 0; counter < dtADUsers.Rows.Count; counter++)
                    {
                        ListItem listItem = new ListItem();

                        listItem.Text = dtADUsers.Rows[counter].ItemArray[2].ToString() + " (" + dtADUsers.Rows[counter].ItemArray[0] + ")";
                        listItem.Value = dtADUsers.Rows[counter].ItemArray[0] + ";" + dtADUsers.Rows[counter].ItemArray[1];
                        ListBox_ADUsers.Items.Add(listItem);
                    }
                    ListBox_ADUsers.SelectedIndex = 0;
                    Groups.GroupsReader groupsReader = Groups.ExecuteReader("1=1");
                    CheckBoxList_UserGroups.DataSource = groupsReader;
                    CheckBoxList_UserGroups.DataTextField = "GroupName";
                    CheckBoxList_UserGroups.DataValueField = "GroupId";
                    CheckBoxList_UserGroups.DataBind();
                    groupsReader.Close();

                    DropDownList_User_Type.DataSource = GetUserTypes();
                    DropDownList_User_Type.DataTextField = "User_Type";
                    DropDownList_User_Type.DataValueField = "User_Type";
                    DropDownList_User_Type.DataBind();

                    DropDownList_User_Type_SelectedIndexChanged(sender, null);

                    //BuildTree();
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                }
                finally
                {
                    task.EndTask();
                }
            }

        }
        protected void DropDownList_User_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            string selectedUserType = String.Empty;
            int userId = 0;

            AppUser loggedInUser = (AppUser)Session[SessionVars.user.ToString()];

            if (loggedInUser == null)
                return;

            if (!loggedInUser.UserType.Equals("NCR"))
            {
                DropDownList_User_Type.SelectedValue = loggedInUser.UserType;
                DropDownList_User_Type.Enabled = false;
            }

            if (!IsPostBack)
            {
                if (Session["u"] != null)
                {
                    userId = int.Parse(Session["u"].ToString());
                    AppUser user = AppUser.LoadAppUserByPk(userId);
                    DropDownList_User_Type.SelectedValue = user.UserType;
                    builder.Append(" and user_id <> " + user.UserId);

                }
            }

            selectedUserType = DropDownList_User_Type.SelectedValue;

            DropDownList_Manager.Items.Clear();
            DropDownList_Manager.Items.Add("Select");
            AppUser.AppUserReader userReader = AppUser.ExecuteReader("user_type = '" + selectedUserType + "' and user_id>2 " + builder.ToString());
            while (userReader.Read())
            {
                DropDownList_Manager.Items.Add(new ListItem(userReader.CurrentAppUser.UserFullName, userReader.CurrentAppUser.UserId.ToString()));
            }
            userReader.Close();


            CheckBoxList_UserGroups.Items.Clear();
            CheckBoxList_Organization.Items.Clear();
            CheckBoxList_CIT.Items.Clear();
            UserATMsTreeView.Nodes.Clear();

            PopulateGroups(selectedUserType, loggedInUser);
            PopulateOrganizations(selectedUserType, loggedInUser);

        }
        private void PopulateGroups(string userType, AppUser loggedInUser)
        {
            String whereExpression = String.Format("entity_type = '{0}'", userType);

            if (!loggedInUser.UserType.Equals("NCR"))
            {
                whereExpression += String.Format(" and created_by = {0}", loggedInUser.UserId);
            }

            Groups.GroupsReader groupsReader = Groups.ExecuteReader(whereExpression);
            CheckBoxList_UserGroups.DataSource = groupsReader;
            CheckBoxList_UserGroups.DataTextField = "GroupName";
            CheckBoxList_UserGroups.DataValueField = "GroupId";
            CheckBoxList_UserGroups.DataBind();
            groupsReader.Close();

            //Disabling default "Administration" Group if User is not "Admin"
            if (loggedInUser.UserId != 2)
            {
                DisableAdministrationGroup(userType);
            }
        }
        private void DisableAdministrationGroup(string userType)
        {
            if (userType.Equals("NCR"))
            {
                foreach (ListItem item in CheckBoxList_UserGroups.Items)
                {
                    if (item.Value.Equals("Administration"))
                    {
                        item.Enabled = false;
                        break;
                    }
                }
            }
        }
        private void ShowOrganizationForUserType(string userType)
        {
            if (userType.Equals("CIT"))
            {
                Organization_Row.Visible = false;
                CIT_Row.Visible = true;
            }
            else
            {
                Organization_Row.Visible = true;
                CIT_Row.Visible = false;
            }
        }
        private void PopulateOrganizations(string userType, AppUser loggedInUser)
        {
            ShowOrganizationForUserType(userType);

            if (userType.Equals("CIT"))
            {
                LoadCIT(loggedInUser);
            }
            else
            {
                LoadOrganization(loggedInUser);
            }


        }
        private void LoadCIT(AppUser loggedInUser)
        {
            CheckBoxList_CIT.Items.Clear();
            SqlCommand cmd = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                String commandText = "select id, name from ccms_cit ";

                if (!loggedInUser.UserType.Equals("NCR"))
                {
                    CcmsUserCit.CcmsUserCitReader reader = CcmsUserCit.ExecuteReader(String.Format("user_id = {0}", loggedInUser.UserId));
                    if (reader.Read())
                    {
                        commandText += String.Format(" where id = {0}", reader.CurrentCcmsUserCit.CitId);
                    }
                    reader.Close();
                }

                cmd.CommandText = commandText;
                SqlDataReader SqlReader = cmd.ExecuteReader();
                while (SqlReader.Read())
                    CheckBoxList_CIT.Items.Add(new ListItem("[" + SqlReader.GetString(1) + "]", SqlReader.GetInt64(0).ToString()));
                SqlReader.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        private void LoadOrganization(AppUser loggedInUser)
        {
            SqlCommand cmd = null;
            try
            {
                CheckBoxList_Organization.Items.Clear();

                String commandText = "select mcn,region_name from region where is_organization = 1 and parent_region_id is not null";

                if (!loggedInUser.UserType.Equals("NCR"))
                {
                    CcmsUserOrganization.CcmsUserOrganizationReader reader = CcmsUserOrganization.ExecuteReader(String.Format("user_id = {0}", loggedInUser.UserId));
                    if (reader.Read())
                    {
                        commandText += String.Format(" and region_id = {0}", reader.CurrentCcmsUserOrganization.OrganizationId);
                    }
                    reader.Close();
                }

                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandText = commandText;
                SqlDataReader SqlReader = cmd.ExecuteReader();
                while (SqlReader.Read())
                    CheckBoxList_Organization.Items.Add(new ListItem(SqlReader.GetString(0) + "[" + SqlReader.GetString(1) + "]", SqlReader.GetString(0)));
                SqlReader.Close();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }
        protected void Button_Load_CIT_Click(object sender, EventArgs e)
        {
            int selectedCITCount = 0;
            int selectedCITId = 0;

            UserATMsTreeView.Nodes.Clear();

            foreach (System.Web.UI.WebControls.ListItem listItem in CheckBoxList_CIT.Items)
            {
                if (listItem.Selected)
                    selectedCITCount++;
            }

            if (selectedCITCount == 0)
            {
                Literal_error.Text = "<li>No CIT selected.</li>";
                return;
            }
            else if (selectedCITCount > 1)
            {
                Literal_error.Text = "<li>Please select only one CIT.</li>";
                return;
            }

            selectedCITId = GetSelectedCIT();
            BuildTreeFromCIT(selectedCITId);

            SelectTreeNodesFromUserState();
        }
        private int GetSelectedCIT()
        {
            int selectedCITId = 0;

            foreach (System.Web.UI.WebControls.ListItem listItem in CheckBoxList_CIT.Items)
            {
                if (listItem.Selected)
                {
                    selectedCITId = int.Parse(listItem.Value);
                    break;
                }
            }

            return selectedCITId;
        }
        private void BuildTreeFromCIT(int selectedCitId)
        {
            UserATMsTreeView.Nodes.Clear();


            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select region_name, region_id, parent_region_id, is_organization, mcn from region 
                                where region_id in (select b.region_id from region a 
						                        inner join region b on a.parent_region_id = b.region_id 
						                        inner join region_cit on a.region_id = region_cit.region_id
						                        where region_cit.cit_id = " + selectedCitId + "); " +
                                "select a.region_name, a.region_id, a.parent_region_id, a.is_organization, a.mcn " +
                                                "from region a inner join region_cit on a.region_id = region_cit.region_id " +
                                                "where region_cit.cit_id = " + selectedCitId + "; " +
                                "select title, atm_id, region_id from atm " +
                                "where region_id in (select a.region_id " +
                                                        "from region a inner join region_cit on a.region_id = region_cit.region_id " +
                                                        "where region_cit.cit_id = " + selectedCitId + ");";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            atmsDS = new DataSet();
            adapter.Fill(atmsDS);

            //Adding Organization Node
            foreach (DataRow dbRow in atmsDS.Tables[0].Rows)
            {
                ComponentArt.Web.UI.TreeViewNode organizationNode = CreateRegionNode(dbRow["region_name"].ToString(), true, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
                UserATMsTreeView.Nodes.Add(organizationNode);

                //Adding Regions in Organization and ATMs in Region Node
                foreach (DataRow dbRegionRow in atmsDS.Tables[1].Select("parent_region_id = " + (int)dbRow["region_id"]))
                {
                    ComponentArt.Web.UI.TreeViewNode regionNode = CreateRegionNode(dbRegionRow["region_name"].ToString(), true, (int)dbRegionRow["region_id"], bool.Parse(dbRegionRow["is_organization"].ToString()));
                    organizationNode.Nodes.Add(regionNode);

                    foreach (DataRow dbATMRow in atmsDS.Tables[2].Select(" region_id = " + (int)dbRegionRow["region_id"]))
                    {
                        ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(dbATMRow["title"].ToString(), (int)dbATMRow["atm_id"]);
                        regionNode.Nodes.Add(childNode);
                    }
                }
            }


        }

        protected void Button_SaveUpdate_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddADUser");
            try
            {
                List<string> currGroups = new List<string>();
                List<string> addedGroups = new List<string>();
                List<string> removedGroup = new List<string>();
                string[] split = ListBox_ADUsers.SelectedItem.Value.Split(';');

                if ((int)ConnectionFactory.ExecuteScalar("select count(user_id) from app_user where user_login = '" + split[0] + "'") > 0)
                    throw new Exception("User with the same ID already exists!");

                AppUser newUser;
                AppUserApprover _newApproverUser;

                SqlConnection conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                SqlTransaction trx = conn.BeginTransaction();

                _newApproverUser = new AppUserApprover();
                newUser = new AppUser();
                if (enablemakeachecker == "false")
                {
                    newUser = new AppUser();
                    newUser.UserIsActive = true;
                    newUser.UserCreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    newUser.UserCreationTime = DateTime.Now;
                    newUser.UserType = DropDownList_User_Type.SelectedValue;
                    newUser.IsAdded = true;

                    if (DropDownList_Manager.SelectedIndex > 0)
                        newUser.EmployeeManagerId = int.Parse(DropDownList_Manager.SelectedItem.Value);

                    newUser.UserLogin = split[0];
                    newUser.IsActiveDirectoryUser = true;
                    newUser.UserFullName = ListBox_ADUsers.SelectedItem.Text.Replace("'", "''");
                    newUser.UserEmail = split[1];

                    newUser.Save(conn, trx);
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "User saved");

                    ConnectionFactory.ExecuteQuery("delete from group_users where user_id = " + newUser.UserId, trx);

                    foreach (System.Web.UI.WebControls.ListItem SelectedGroups in CheckBoxList_UserGroups.Items)
                        if (SelectedGroups.Selected)
                        {
                            if (newUser.IsEditied)
                            {
                                if (!currGroups.Contains(SelectedGroups.Text))
                                    addedGroups.Add(SelectedGroups.Text);
                                else
                                    currGroups.Remove(SelectedGroups.Text);
                            }
                            (new GroupUsers(newUser.UserId, int.Parse(SelectedGroups.Value))).Save(conn, trx);
                        }
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "groups for the user saved");

                    ConnectionFactory.ExecuteQuery("delete from user_ATMs where user_id = " + newUser.UserId, trx);
                    foreach (ComponentArt.Web.UI.TreeViewNode node in UserATMsTreeView.CheckedNodes)
                        if (node.ID.Substring(0, 1) != "r")
                            (new UserATMs(newUser.UserId, int.Parse(node.ID.Substring(1)))).Save(conn, trx);
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "atms for the user saved");

                    if (newUser.UserType.Equals("NCR"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from user_organization where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                            if (SelectedOrganizations.Selected)
                                (new UserOrganization(newUser.UserId, SelectedOrganizations.Value)).Save(conn, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "organization(s) for NCR user saved");
                    }
                    else if (newUser.UserType.Equals("BANK"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from user_organization where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                            if (SelectedOrganizations.Selected)
                            {
                                (new UserOrganization(newUser.UserId, SelectedOrganizations.Value)).Save(conn, trx);
                            }
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "organization for Bank user saved");
                    }
                    else if (newUser.UserType.Equals("CIT"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from ccms_user_cit where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem selectedCIT in CheckBoxList_CIT.Items)
                            if (selectedCIT.Selected)
                                (new CcmsUserCit((long)newUser.UserId, long.Parse(selectedCIT.Value))).Save(conn, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "cit for user saved");
                    }

                     Utility.BuildAuditLog("User [" + newUser.UserFullName + "] has been created successfully", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateUser);

                    if (trx != null)
                        trx.Commit();

                    Session[SessionVars.notice.ToString()] = "User [" + newUser.UserFullName + "] was saved successfully.";
                }
                else
                {
                    _newApproverUser.UserCreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    _newApproverUser.UserCreationTime = DateTime.Now;
                    _newApproverUser.UserIsActive = true;
                    _newApproverUser.UserType = DropDownList_User_Type.SelectedValue;
                    _newApproverUser.IsActiveDirectoryUser = true;

                    _newApproverUser.UserLogin = split[0];
                    _newApproverUser.UserFullName = ListBox_ADUsers.SelectedItem.Text.Replace("'", "''");
                    _newApproverUser.UserEmail = split[1];
                    if (DropDownList_Manager.SelectedIndex > 0)
                        _newApproverUser.EmployeeManagerId = int.Parse(DropDownList_Manager.SelectedItem.Value);
                    _newApproverUser.IsAdded = true;
                    _newApproverUser.IsDeleted = false;
                    _newApproverUser.IsEditied = false;
                    _newApproverUser.ApprovalStatus = "Pending";
                    _newApproverUser.Save(conn, trx);

                    newUser.UserFullName = _newApproverUser.UserFullName;
                    newUser.CitId = _newApproverUser.CitId;
                    newUser.EmployeeManagerId = _newApproverUser.EmployeeManagerId;
                    newUser.IsActiveDirectoryUser = _newApproverUser.IsActiveDirectoryUser;
                    newUser.ManagerId = _newApproverUser.ManagerId;
                    newUser.MobileNumber = _newApproverUser.MobileNumber;
                    newUser.RetryAttempt = _newApproverUser.RetryAttempt;
                    newUser.UserCreatedBy = _newApproverUser.UserCreatedBy;
                    newUser.UserCreationTime = _newApproverUser.UserCreationTime;
                    newUser.UserEmail = _newApproverUser.UserEmail;
                    newUser.UserIsActive = false;
                    newUser.UserLastLoginTime = _newApproverUser.UserLastLoginTime;
                    newUser.UserLogin = _newApproverUser.UserLogin;
                    newUser.UserModificationTime = _newApproverUser.UserModificationTime;
                    newUser.UserModifiedBy = _newApproverUser.UserModifiedBy;

                    newUser.UserType = _newApproverUser.UserType;
                    newUser.ApprovalStatus = _newApproverUser.ApprovalStatus;
                    newUser.IsAdded = _newApproverUser.IsAdded;
                    newUser.IsDeleted = _newApproverUser.IsDeleted;
                    newUser.IsEditied = _newApproverUser.IsEditied;
                    newUser.Save(conn, trx);

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "AD User added on App User Approval");

                    if (_newApproverUser.UserType.Equals("NCR"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                            if (SelectedOrganizations.Selected)
                                (new UserOrganizationApprover(newUser.UserId, SelectedOrganizations.Value)).Save(conn, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "organization(s) for NCR user saved");
                    }
                    else if (_newApproverUser.UserType.Equals("BANK"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                            if (SelectedOrganizations.Selected)
                            {
                                (new UserOrganizationApprover(newUser.UserId, SelectedOrganizations.Value)).Save(conn, trx);
                            }
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "organization for Bank user saved");
                    }
                    else if (_newApproverUser.UserType.Equals("CIT"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem selectedCIT in CheckBoxList_CIT.Items)
                            if (selectedCIT.Selected)
                                (new CcmsUserCitApprover((long)newUser.UserId, long.Parse(selectedCIT.Value))).Save(conn, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "cit for user saved");
                    }
                    try
                    {
                        foreach (System.Web.UI.WebControls.ListItem SelectedGroups in this.CheckBoxList_UserGroups.Items)
                        {
                            if (SelectedGroups.Selected)
                                (new GroupUsersApprover(newUser.UserId, int.Parse(SelectedGroups.Value))).Save(conn, trx);
                        }
                        foreach (ComponentArt.Web.UI.TreeViewNode node in UserATMsTreeView.CheckedNodes)
                        {
                            if (node.ID.Substring(0, 1) != "r")
                                (new UserATMsApprover(newUser.UserId, int.Parse(node.ID.Substring(1)))).Save(conn, trx);
                        }

                        trx.Commit();
                        Session[SessionVars.notice.ToString()] = "Request to add user [" + newUser.UserFullName + "] has been sent for approval.";
                        Utility.BuildAuditLog("Request to add user [" + newUser.UserFullName + "] has been sent for approval.", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateUser);
                    }
                    catch (Exception ex)
                    {
                        trx.Rollback();
                        Literal_error.Text = "<li>An error occured while assigning users/atms! " + ex.Message + "</li>";
                        throw new Exception("error while assigning users/atms", ex);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                ListBox_ADUsers.Items.Remove(ListBox_ADUsers.SelectedItem);
                if (ListBox_ADUsers.Items.Count == 0)
                    Response.Redirect("Users.aspx");
                else
                    ListBox_ADUsers.SelectedIndex = 0;

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "groups for the user added");
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        private bool CheckMultipleOrganizationSelection(string userType)
        {
            bool bValue = false;
            int selectedOrganizationCount = 0;

            foreach (System.Web.UI.WebControls.ListItem listItem in CheckBoxList_Organization.Items)
                if (listItem.Selected)
                    selectedOrganizationCount++;

            //if (selectedOrganizationCount == 0)
            //{
            //    Literal_error.Text = "<li>No organization selected.</li>";
            //}

            if (userType.Equals("Bank") && selectedOrganizationCount > 1)
                return true;


            return bValue;
        }

        private long GetSelectedOrganizationId()
        {
            long selectedOrganizationId = 0;
            string organizationMCN = null;

            // Add user case
            if (Session["u"] != null)
            {
                AppUser user = AppUser.LoadAppUserByPk(int.Parse(Session["u"].ToString()));

                CcmsUserOrganization.CcmsUserOrganizationReader reader = CcmsUserOrganization.ExecuteReader("user_id = " + user.UserId);

                if (reader.Read())
                {
                    selectedOrganizationId = reader.CurrentCcmsUserOrganization.OrganizationId;
                }

                reader.Close();

            }
            else
            {
                int selectedOrganiztaionCount = 1;

                foreach (System.Web.UI.WebControls.ListItem listItem in CheckBoxList_Organization.Items)
                {
                    if (listItem.Selected)
                    {
                        if (selectedOrganiztaionCount > 1)
                        {
                            throw new Exception("Multiple organizations selected, Please select only one organization.");
                        }
                        organizationMCN = listItem.Value;
                        selectedOrganiztaionCount++;
                    }
                }

                if (organizationMCN == null)
                {
                    throw new Exception("Please select an organization.");
                }

                selectedOrganizationId = GetOrganizationIdFromMCN(organizationMCN);
            }
            return selectedOrganizationId;
        }
        
        private long GetOrganizationIdFromMCN(string organizationMCN)
        {
            long organizationId = 0;
            SqlCommand cmd = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandText = "select region_id from region where mcn = '" + organizationMCN + "'";
                object id = cmd.ExecuteScalar();
                if (id != null)
                {
                    organizationId = long.Parse(id.ToString());
                }

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("GetOrganizationIdFromMCN", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }

            return organizationId;

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


        protected void BuildTreeForBank(long organizationId)
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
            atmsDS = new DataSet();
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

        protected void Button_Load_Click(object sender, EventArgs e)
        {
            string selectedUserType = DropDownList_User_Type.SelectedValue;
            long selectedOrganizationId = 0;

            if (CheckMultipleOrganizationSelection(selectedUserType))
            {
                Literal_error.Text = "<li>Please select only one organization.</li>";
                return;
            }

            LogableTask task = LogableTask.NewTask("LoadTree");

            try
            {
                if (selectedUserType.Equals("BANK"))
                {
                    selectedOrganizationId = GetSelectedOrganizationId();
                    BuildTreeForBank(selectedOrganizationId);
                }
                else
                {
                    BuildTree();
                }


                SelectTreeNodesFromUserState();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>Error while loading tree. [" + ex.Message + "]</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
        private void SelectTreeNodesFromUserState()
        {
            if (Session["u"] != null)
            {
                SqlCommand cmd = null;
                try
                {
                    ArrayList atmsList = new ArrayList();
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "SELECT atm_ID FROM USER_atms WHERE USER_ID = " + Session["u"];
                    SqlDataReader SqlReader = cmd.ExecuteReader();
                    while (SqlReader.Read())
                        atmsList.Add(SqlReader.GetInt32(0));
                    SqlReader.Close();
                    cmd.Connection.Close();

                    if (atmsList.Count > 0)
                        for (int i = 0; i < UserATMsTreeView.Nodes.Count; i++)
                            SetCheck(UserATMsTreeView.Nodes[i], atmsList);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (cmd != null)
                        if (cmd.Connection != null)
                            cmd.Connection.Close();
                }
            }
        }
        protected void SetCheck(TreeViewNode node, ArrayList selectedItem)
        {
            TreeViewNodeCollection nodeCol = node.Nodes;
            int atms = 0;

            for (int counter = 0; counter < nodeCol.Count; counter++)
            {
                if (nodeCol[counter].ID.IndexOf('r') < 0)
                {
                    if (selectedItem.IndexOf(int.Parse(nodeCol[counter].ID.Substring(1))) >= 0)
                    {
                        nodeCol[counter].Checked = true;
                        atms++;
                    }
                }
                else if (nodeCol[counter].Nodes.Count > 0)
                {
                    SetCheck(nodeCol[counter], selectedItem);

                    if (nodeCol[counter].Checked)
                        atms++;
                }
                else
                {
                    nodeCol[counter].Checked = true;
                    atms++;
                }
            }

            if (atms == nodeCol.Count && atms != 0)
                nodeCol[0].ParentNode.Checked = true;
        }
        private void setCheck(TreeViewNode node, ArrayList selectedItem)
        {

            TreeViewNodeCollection nodeCol = node.Nodes;
            int atms = 0;

            for (int counter = 0; counter < nodeCol.Count; counter++)
            {
                if (nodeCol[counter].ID.IndexOf('r') < 0)
                {
                    if (selectedItem.IndexOf(int.Parse(nodeCol[counter].ID.Substring(1))) >= 0)
                    {
                        nodeCol[counter].Checked = true;
                        atms++;
                    }
                }
                if (nodeCol[counter].Nodes.Count > 0)
                {
                    setCheck(nodeCol[counter], selectedItem);
                    if (nodeCol[counter].Checked) atms++;
                }
            }

            if (atms == nodeCol.Count && atms != 0)
                nodeCol[0].ParentNode.Checked = true;

        }
        DataSet atmsDS;
        private void BuildTree()
        {
            ComponentArt.Web.UI.TreeViewNode childNode = null;
            UserATMsTreeView.Nodes.Clear();
            bool noRegion = true;
            jsStr.Append("<script>var ary1 = [");
            //if (Session["LoadTreeCheck"] != null && Session["LoadTreeCheck"].ToString() == "1")
            //{
            //    return;
            //}
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select title,atm_id,region_id from atm where is_active = 1 order by title;select region_name, region_id, parent_region_id, is_organization, mcn from region where is_active = 1 order by region_name";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            atmsDS = new DataSet();
            adapter.Fill(atmsDS);
            atmsDS.Relations.Add("ATMRegionRelation", atmsDS.Tables[1].Columns["region_id"], atmsDS.Tables[0].Columns["region_id"]);
            atmsDS.Relations.Add("RegionRegionRelation", atmsDS.Tables[1].Columns["parent_region_id"], atmsDS.Tables[1].Columns["region_id"], false);

            foreach (DataRow dbRow in atmsDS.Tables[1].Rows)
            {
                if (dbRow.IsNull("parent_region_id"))
                {
                    ComponentArt.Web.UI.TreeViewNode newNode = CreateRegionNode(dbRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
                    newNode.EditingEnabled = newNode.DraggingEnabled = false;
                    UserATMsTreeView.Nodes.Add(newNode);
                    jsStr.Append("['" + newNode.ID + "','" + newNode.Text + "'],");
                    PopulateSubTree(dbRow, newNode, false, false, dbRow["mcn"].ToString());
                    noRegion = false;
                }
            }

            foreach (DataRow dbRow in atmsDS.Tables[0].Rows)
            {
                if (dbRow.IsNull("region_id"))
                {
                    childNode = CreateATMNode(dbRow["title"].ToString(), (int)dbRow["atm_id"]);
                    childNode.EditingEnabled = childNode.DraggingEnabled = false;
                    childNode.Nodes.Add(childNode);
                    jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                }
            }
            jsStr[jsStr.Length - 1] = ']';
            jsStr.Append("</script>");
            if (atmsDS.Tables[1].Rows.Count == 0)
            {
                literal_jsArray1.Text = "<script>var ary1 = new Array();var aryLocation1 = new Array(); </script>";
            }
            else
            {
                literal_jsArray1.Text = jsStr.ToString() + " " + jsStrLocation.ToString();
            }

            if (noRegion)
                literal_jsArray1.Text = "<script>var ary1 = new Array();var aryLocation1 = new Array(); </script>";
            //Session["LoadTreeCheck"] = "1";
        }

        private void PopulateSubTree(DataRow parentRegionRow, ComponentArt.Web.UI.TreeViewNode node, bool allowEditingRegion, bool allowEditingATM, string parentMCN)
        {
            foreach (DataRow childRow in atmsDS.Tables[1].Select("parent_region_id=" + parentRegionRow["region_id"]))
            {
                ComponentArt.Web.UI.TreeViewNode childNode = CreateRegionNode(childRow["region_name"].ToString(), false, (int)childRow["region_id"]);
                node.Nodes.Add(childNode);
                jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                childNode.EditingEnabled = childNode.DraggingEnabled = allowEditingRegion;
                PopulateSubTree(childRow, childNode, allowEditingRegion, allowEditingATM, parentMCN);
            }
            foreach (DataRow childRow in parentRegionRow.GetChildRows("ATMRegionRelation"))
            {
                ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(childRow["title"].ToString(), (int)childRow["atm_id"]);
                node.Nodes.Add(childNode);
                jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                childNode.EditingEnabled = childNode.DraggingEnabled = allowEditingATM;
            }
        }

        private ComponentArt.Web.UI.TreeViewNode CreateRegionNode(string text, bool expanded, int id)
        {
            ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();
            node.Text = text;
            node.ID = "r" + id;
            node.ImageUrl = "treeRegion.jpg";
            node.Expanded = expanded;
            node.ShowCheckBox = true;
            return node;
        }

        private ComponentArt.Web.UI.TreeViewNode CreateATMNode(string text, int id)
        {
            ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();
            node.Text = text;
            node.ID = "a" + id;
            node.ImageUrl = "treeATM.jpg";
            node.DroppingEnabled = false;
            node.ShowCheckBox = true;
            return node;
        }



    }
}