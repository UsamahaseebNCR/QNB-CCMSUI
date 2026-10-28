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

using ComponentArt.Web.UI;
using Avanza.CCMS.DAL;
using System.Linq;
using System.Collections.Generic;
using Encryption;
using Newtonsoft.Json;
using Avanza.iSuite.DAL;
//using Avanza.iSuite.DAL;
//using System.Web.Helpers;

namespace Avanza.iSuite.UI
{
    public partial class AddEditUser : BasePage
    {
        DataSet atmsDS;
        StringBuilder jsStr = new StringBuilder(5000);
        StringBuilder jsStrLocation = new StringBuilder(5000);
        bool isTreeviewNodeExpanded = System.Web.Configuration.WebConfigurationManager.AppSettings["isTreeviewNodeExpanded"] == "1" ? true : false;
        string traceOrganizationMCN = System.Web.Configuration.WebConfigurationManager.AppSettings["traceOrganizationMCN"];
        string enablemakeachecker = System.Web.Configuration.WebConfigurationManager.AppSettings["enablemakeachecker"];

        /// <summary>
        /// Edited by Ali Faiyaz Shah on 07th Juy, 2020.
        /// To apply Anti Forgery Token to avoid Cross-site forgery attacks
        /// </summary>
        protected void Page_Init(object sender, EventArgs e)
        {
            //UserDiv.InnerHtml = AntiForgery.GetHtml().ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.SetFocus(TextBox_Login_Id);
                Literal_error.Text = string.Empty;

                if (!Page.IsPostBack)
                {
                    if (Request.QueryString["u"] != null)
                    {
                        try
                        {
                            string input = Encryption.Cryptic.DecryptString(Request.QueryString["u"].ToString());
                            if (input.Contains(","))
                            {
                                if (input.Contains("\0"))
                                {
                                    int charLocation = input.IndexOf("\0", StringComparison.Ordinal);
                                    if (charLocation > 0)
                                    {
                                        input = input.Substring(0, charLocation);
                                    }
                                }
                                string[] words = input.Split(',');
                                AppUser result = AppUser.LoadAppUser(" user_id = " + int.Parse(words[0]) + " and user_full_name = '" + words[1].ToString() + "'");
                                if (result == null)
                                    Response.Redirect("AccessDenied.aspx");
                                else
                                    Session["u"] = result.UserId.ToString();
                            }
                            else
                                Response.Redirect("AccessDenied.aspx");
                        }
                        catch (Exception ex)
                        {
                            LogableTask.LogMonoActivityTask("Page_Load", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                            Response.Redirect("AccessDenied.aspx");
                        }
                    }

                    ViewState["PostBackCounter"] = 1;
                    DropDownList_User_Type.DataSource = GetUserTypes();
                    DropDownList_User_Type.DataTextField = "User_Type";
                    DropDownList_User_Type.DataValueField = "User_Type";
                    DropDownList_User_Type.DataBind();
                    DropDownList_User_Type_SelectedIndexChanged(sender, null);
                    Avanza.CCMS.DAL.AlertType.AlertTypeReader reader = Avanza.CCMS.DAL.AlertType.ExecuteReader("1=1");
                    checkbox_list.DataSource = reader;
                    checkbox_list.DataBind();
                    reader.Close();
                    if (Session["u"] != null)
                    {
                        if (!CheckRight((int)Permissions.ModifyUser))
                            Response.Redirect("AccessDenied.aspx");

                        AppUser user = AppUser.LoadAppUserByPk(int.Parse(Session["u"].ToString()));
                        TextBox_Login_Id.Enabled = false;
                        DropDownList_User_Type.Enabled = false;
                        GetUserForEditing(user.UserId);
                        SetAssignedAlerts();

                        if (user.UserType.Equals("CIT"))
                            Button_Load_CIT_Click(sender, null);
                        else
                            Button_Load_Click(sender, null);
                    }
                    else if (Session["userApprover"] != null)
                    {
                        if (!CheckRight((int)Permissions.ApproveUser))
                            Response.Redirect("AccessDenied.aspx");

                        AppUserApprover user = AppUserApprover.LoadAppUseApproverrByuserLogin(Session["userApprover"].ToString());
                        TextBox_Login_Id.Enabled = false;
                        DropDownList_User_Type.Enabled = false;
                        GetUserForView(user.UserLogin);
                        SetAssignedAlertsForView();

                        if (user.UserType.Equals("CIT"))
                            Button_Load_CIT_Click(sender, null);
                        else
                            Button_Load_Click(sender, null);
                    }
                    else
                    {
                        if (!CheckRight((int)Permissions.CreateUser))
                            Response.Redirect("AccessDenied.aspx");

                        Button_Load_Click(sender, null);
                    }
                    if (Session["u"] != null && Session["u"].ToString() == "2")
                    {
                        Response.Redirect("AccessDenied.aspx");
                    }
                }
                else
                    ViewState["PostBackCounter"] = (int)ViewState["PostBackCounter"] + 1;
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("Page_Load", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>Error while loading page. [" + ex.Message + "]</li>";
            }
        }

        private void SetAssignedAlertsForView()
        {
            AppUserApprover user = AppUserApprover.LoadAppUseApproverrByuserLogin(Session["userApprover"].ToString());
            AppUser _appUser = AppUser.LoadAppUser("user_login='"+user.UserLogin+"'");

            System.Collections.Generic.List<long> list = new System.Collections.Generic.List<long>();

            foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_Organization.Items)
            {
                if (SelectedItem.Selected)
                {
                    //CcmsAlertNotificationApprover.CcmsAlertNotificationApproverReader alertNotificationReader = CcmsAlertNotificationApprover.ExecuteReader(
                    //    "user_id=" + _appUser.UserId.ToString() + " and organization_id= " + GetOrganizationIdFromMCN(SelectedItem.Value));
                    SqlCommand cmd = null;
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "Select id,alert_type_id,organization_id,user_id from ccms_alert_notification_approver where user_id = " + _appUser.UserId + " and organization_id =" + GetOrganizationIdFromMCN(SelectedItem.Value);
                    //cmd.ExecuteReader();

                    //var result = alertNotificationReader.Read();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var x = rdr.IsDBNull(0) ? 0 : rdr.GetInt32(0);
                            //int a = Convert.ToInt32(rdr.ToString());
                            list.Add((long)rdr[1]);
                        }
                    }
                    //while (cmd.Read())
                    //{
                    //    list.Add((int)alertNotificationReader.CurrentCcmsAlertNotificationApprover.AlertTypeId.Value);
                    //}
                    //alertNotificationReader.Close();
                    foreach (ListItem item in checkbox_list.Items)
                    {
                        if (list.IndexOf(int.Parse(item.Value)) != -1)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
        }

        private void SetAssignedAlerts()
        {
            System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();

            foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_Organization.Items)
            {
                if (SelectedItem.Selected)
                {
                    CcmsAlertNotification.CcmsAlertNotificationReader alertNotificationReader = CcmsAlertNotification.ExecuteReader(
                        "user_id=" + Session["u"].ToString() + " and organization_id=" + GetOrganizationIdFromMCN(SelectedItem.Value));

                    while (alertNotificationReader.Read())
                    {
                        list.Add((int)alertNotificationReader.CurrentCcmsAlertNotification.AlertTypeId.Value);
                    }
                    alertNotificationReader.Close();
                    foreach (ListItem item in checkbox_list.Items)
                    {
                        if (list.IndexOf(int.Parse(item.Value)) != -1)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
        }

        protected void GetUserForView(string userLogin)
        {
            SqlCommand cmd = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                AppUserApprover user = AppUserApprover.LoadAppUseApproverrByuserLogin(userLogin);
                AppUser _appUser = AppUser.LoadAppUser("user_login='"+user.UserLogin+"'");

                if (!user.IsActiveDirectoryUser)
                {
                    TextBox_Password.Attributes.Add("value", user.UserPassword);
                    TextBox_Password.Attributes.Add("onchange", "setDirtyFlagToUpdatePasswordInDb();");

                    TextBox_confirmpass.Attributes.Add("value", user.UserPassword);
                }
                else
                {
                    TextBox_Password.Enabled = false;
                    TextBox_confirmpass.Enabled = false;
                    //RequiredFieldValidator_TextBox_Password.EnableClientScript = false;
                    //CompareValidator1.EnableClientScript = false;
                    //RequiredFieldValidator1.EnableClientScript = false;
                }

                TextBox_MobileNo.Text = user.MobileNumber;
                TextBox_Login_Id.Text = user.UserLogin;
                //TextBox_Password.Attributes.Add("value", Cryptic.DecryptString(user.UserPassword));
                //TextBox_confirmpass.Attributes.Add("value", Cryptic.DecryptString(user.UserPassword));
                TextBox_Fullname.Text = user.UserFullName;
                TextBox_Email.Text = user.UserEmail;
                DropDownList_User_Type.SelectedValue = user.UserType;

                cmd.CommandText = "SELECT GROUP_ID FROM group_users_approver WHERE USER_ID =" + _appUser.UserId;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_UserGroups.Items)
                    {
                        if (SelectedItem.Value == reader[0].ToString())
                            SelectedItem.Selected = true;
                        if (user.UserId == 2)
                        {
                            if (SelectedItem.Value == "1")
                                SelectedItem.Enabled = false;
                        }
                    }
                }
                reader.Close();


                if (user.UserType.Equals("NCR"))
                {
                    cmd.CommandText = "select org_mcn from user_organization_approver where user_id = " + _appUser.UserId;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_Organization.Items)
                        {
                            if (SelectedItem.Value == reader[0].ToString())
                                SelectedItem.Selected = true;
                        }
                    }
                    reader.Close();
                }

                else if (user.UserType.Equals("BANK"))
                {
                    cmd.CommandText = String.Format(@"select region.mcn from ccms_user_organization inner join region on ccms_user_organization.organization_id = region.region_id
                                                        where ccms_user_organization.user_id = {0}", _appUser.UserId);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_Organization.Items)
                        {
                            if (SelectedItem.Value == reader[0].ToString())
                                SelectedItem.Selected = true;
                        }
                    }
                    reader.Close();

                    // Disabling Organization Tree; 
                    CheckBoxList_Organization.Enabled = false;

                }
                else if (user.UserType.Equals("CIT"))
                {
                    cmd.CommandText = String.Format(@"select cit_id from ccms_user_cit_approver where user_id = {0}", _appUser.UserId);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_CIT.Items)
                        {
                            if (SelectedItem.Value == reader[0].ToString())
                                SelectedItem.Selected = true;
                        }
                    }
                    reader.Close();

                    // Disabling Cit Tree;
                    CheckBoxList_CIT.Enabled = false;
                }

                if (user.EmployeeManagerId.HasValue)
                    DropDownList_Manager.SelectedValue = user.EmployeeManagerId.Value.ToString();

                Button_ApproveUser.Visible = true;
                Button_DeclineUser.Visible = true;
                Button_SaveUpdateUser.Visible = false;
                Button_Cancel.Visible = false;
                TextBox_confirmpass.Enabled = false;
                TextBox_Email.Enabled = false;
                TextBox_Fullname.Enabled = false;
                TextBox_Login_Id.Enabled = false;
                TextBox_MobileNo.Enabled = false;
                TextBox_Password.Enabled = false;
                checkbox_list.Enabled = false;
                UserATMsTreeView.Enabled = false;
                UserATMsTreeView.NodeEditingEnabled = false;
                Button_Load.Enabled = false;
                CheckBoxList_CIT.Enabled = false;
                CheckBoxList_Organization.Enabled = false;
                CheckBoxList_UserGroups.Enabled = false;
                DropDownList_Manager.Enabled = false;
                DropDownList_User_Type.Enabled = false;
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("GetUserToView", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        protected void GetUserForEditing(int userId)
        {
            SqlCommand cmd = null;
            try
            {
                TextBox_confirmpass.Enabled = true;
                TextBox_Email.Enabled = true;
                TextBox_Fullname.Enabled = true;
                TextBox_Login_Id.Enabled = false;
                TextBox_MobileNo.Enabled = true;
                TextBox_Password.Enabled = true;
                checkbox_list.Enabled = true;
                CheckBoxList_CIT.Enabled = true;
                CheckBoxList_Organization.Enabled = true;
                CheckBoxList_UserGroups.Enabled = true;
                DropDownList_Manager.Enabled = true;
                DropDownList_User_Type.Enabled = true;

                cmd = ConnectionFactory.GetNewCommand(true);
                AppUser user = AppUser.LoadAppUserByPk(userId);
                if (!user.IsActiveDirectoryUser)
                {
                    TextBox_Password.Attributes.Add("value", user.UserPassword);
                    TextBox_Password.Attributes.Add("onchange", "setDirtyFlagToUpdatePasswordInDb();");

                    TextBox_confirmpass.Attributes.Add("value", user.UserPassword);
                }
                else
                {
                    TextBox_Password.Enabled = false;
                    TextBox_confirmpass.Enabled = false;
                    //RequiredFieldValidator_TextBox_Password.EnableClientScript = false;
                    //CompareValidator1.EnableClientScript = false;
                    //RequiredFieldValidator1.EnableClientScript = false;
                }

                TextBox_MobileNo.Text = user.MobileNumber;
                TextBox_Login_Id.Text = user.UserLogin;
                //TextBox_Password.Attributes.Add("value", Cryptic.DecryptString(user.UserPassword));
                //TextBox_confirmpass.Attributes.Add("value", Cryptic.DecryptString(user.UserPassword));
                TextBox_Fullname.Text = user.UserFullName;
                TextBox_Email.Text = user.UserEmail;
                DropDownList_User_Type.SelectedValue = user.UserType;

                cmd.CommandText = "SELECT GROUP_ID FROM GROUP_USERS WHERE USER_ID =" + userId;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_UserGroups.Items)
                    {
                        if (SelectedItem.Value == reader[0].ToString())
                            SelectedItem.Selected = true;
                        if (user.UserId == 2)
                        {
                            if (SelectedItem.Value == "1")
                                SelectedItem.Enabled = false;
                        }
                    }
                }
                reader.Close();


                if (user.UserType.Equals("NCR"))
                {
                    cmd.CommandText = "select org_mcn from user_organization where user_id = " + userId;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_Organization.Items)
                        {
                            if (SelectedItem.Value == reader[0].ToString())
                                SelectedItem.Selected = true;
                        }
                    }
                    reader.Close();
                }
                else if (user.UserType.Equals("BANK"))
                {
                    cmd.CommandText = String.Format(@"select region.mcn from ccms_user_organization inner join region on ccms_user_organization.organization_id = region.region_id
                                                        where ccms_user_organization.user_id = {0}", user.UserId);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_Organization.Items)
                        {
                            if (SelectedItem.Value == reader[0].ToString())
                                SelectedItem.Selected = true;
                        }
                    }
                    reader.Close();

                    // Disabling Organization Tree; 
                    CheckBoxList_Organization.Enabled = false;

                }
                else if (user.UserType.Equals("CIT"))
                {
                    cmd.CommandText = String.Format(@"select cit_id from ccms_user_cit where user_id = {0}", user.UserId);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (System.Web.UI.WebControls.ListItem SelectedItem in CheckBoxList_CIT.Items)
                        {
                            if (SelectedItem.Value == reader[0].ToString())
                                SelectedItem.Selected = true;
                        }
                    }
                    reader.Close();

                    // Disabling Cit Tree;
                    CheckBoxList_CIT.Enabled = false;
                }

                if (user.EmployeeManagerId.HasValue)
                    DropDownList_Manager.SelectedValue = user.EmployeeManagerId.Value.ToString();

                Button_ApproveUser.Visible = false;
                Button_DeclineUser.Visible = false;
                Button_SaveUpdateUser.Visible = true;
                Button_Cancel.Visible = true;
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("GetUserForEditing", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                throw ex;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();

            }
        }

        public static bool CheckPasswordPolicy(string password, Literal msg)
        {
            const int passwordLength = 8;
            int numberOfCaps = 0;
            int numberOfSmall = 0;
            int numberOfDigit = 0;

            for (int counter = 0; counter < password.Length; counter++)
            {
                if (char.IsNumber(password, counter))
                    numberOfDigit++;
                if (char.IsUpper(password, counter))
                    numberOfCaps++;
                if (char.IsLower(password, counter))
                    numberOfSmall++;
            }

            if (password.Length < passwordLength)
            {
                msg.Text = "Password length cannot be less than " + passwordLength + " characters";
                throw new Exception("Password length cannot be less than " + passwordLength + " characters");
            }
            else if (numberOfDigit == 0)
            {
                msg.Text = "Password must contain at least 1 digit";
                throw new Exception("Password must contain at least 1 digit");
            }
            else if (numberOfCaps == 0)
            {
                msg.Text = "Password must have atleast one upper case character";
                throw new Exception("Password must have atleast one upper case character");
            }
            else if (numberOfSmall == 0)
            {
                msg.Text = "Password must have atleast one lower case character";
                throw new Exception("Password must have atleast one lower case character");
            }

            return true;
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

        protected void Button_SaveUpdate_Click(object sender, EventArgs e)
        {
            
            LogableTask task = LogableTask.NewTask("SaveUser");
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                /// <summary>
                /// Edited by Ali Faiyaz Shah on 07th Juy, 2020.
                /// To apply Anti Forgery Token to avoid Cross-site forgery attacks
                /// </summary>
                //AntiForgery.Validate();

                int selectedCitCount = 0;
                int selectedOrgCount = 0;
                int selectedGroupCount = 0;
                List<string> currGroups = new List<string>();
                List<string> addedGroups = new List<string>();
                List<string> removedGroup = new List<string>();
                string selectedUserType = DropDownList_User_Type.SelectedValue;

                if (CheckBoxList_UserGroups.Items.Count == 0)
                {
                    throw new Exception("No group(s) found for the user type. Please add group for this user type.");
                }

                if (!selectedUserType.Equals("CIT")
                        && CheckBoxList_Organization.Items.Count == 0)
                {
                    throw new Exception("No organization(s) found. Please add organization.");
                }

                if (selectedUserType.Equals("CIT")
                        && CheckBoxList_CIT.Items.Count == 0)
                {
                    throw new Exception("No cit(s) found. Please add cit.");
                }

                foreach (System.Web.UI.WebControls.ListItem SelectedGroups in CheckBoxList_UserGroups.Items)
                {
                    if (SelectedGroups.Selected)
                        selectedGroupCount++;
                }

                if (selectedGroupCount == 0)
                    throw new Exception("Please select group(s) for this user.");

                foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                {
                    if (SelectedOrganizations.Selected)
                        selectedOrgCount++;

                }
                if (!selectedUserType.Equals("CIT") && selectedOrgCount == 0)
                    throw new Exception("Please select organization for this user.");

                foreach (System.Web.UI.WebControls.ListItem SelectedCIT in CheckBoxList_CIT.Items)
                {
                    if (SelectedCIT.Selected)
                        selectedCitCount++;
                }
                if (selectedUserType.Equals("CIT") && selectedCitCount == 0)
                    throw new Exception("Please select cit for this user.");

                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                AppUser newUser = null;
                AppUser oldUser = null;

                trx = conn.BeginTransaction();
                if (enablemakeachecker == "false")
                {
                    //AppUserApprover editUserApprover = null;
                    if (Session["u"] != null)
                    {
                        if (AppUser.LoadAppUser("user_login like '" + TextBox_Login_Id.Text + "' and user_id != " + Session["u"].ToString()) != null)
                            throw new Exception("The user id '" + TextBox_Login_Id.Text + "' already exists!");
                        else
                        {
                            //editUserApprover = new AppUserApprover();
                            newUser = AppUser.LoadAppUserByPk(int.Parse(Session["u"].ToString()));
                            oldUser = AppUser.LoadAppUserByPk(int.Parse(Session["u"].ToString()));
                            int userid = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

                            newUser.UserModifiedBy = userid;
                            newUser.UserCreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                            newUser.UserModificationTime = DateTime.Now;
                            newUser.UserType = DropDownList_User_Type.SelectedValue;
                            newUser.IsEditied = true;
                        }
                    }
                    else
                    {
                        if (AppUser.LoadAppUser("user_login like '" + TextBox_Login_Id.Text + "'") != null)
                            throw new Exception("The user id '" + TextBox_Login_Id.Text + "' already exists!");
                        else
                        {
                            newUser = new AppUser();

                            newUser.UserIsActive = true;
                            newUser.UserCreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                            newUser.UserCreationTime = DateTime.Now;
                            newUser.UserType = DropDownList_User_Type.SelectedValue;
                            newUser.IsAdded = true;
                        }
                    }
                    if (DropDownList_Manager.SelectedIndex > 0)
                        newUser.EmployeeManagerId = int.Parse(DropDownList_Manager.SelectedItem.Value);
                    newUser.UserLogin = TextBox_Login_Id.Text;

                    if ((!newUser.IsActiveDirectoryUser && hidden_dirtyFlagForUserPassword.Value == "1") || Session["u"] == null)
                        newUser.UserPassword = Encryption.Cryptic.EncryptString(TextBox_Password.Text);
                    else
                        newUser.UserPassword = TextBox_Password.Text;

                    if (!newUser.IsActiveDirectoryUser)
                    {
                        if (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).ApplyPasswordPolicy == true)
                            if (!CheckPasswordPolicy(TextBox_Password.Text, Literal_error))
                                return;

                        if (TextBox_Password.Text == string.Empty)
                            throw new Exception("Please Enter Password");
                        if (TextBox_Password.Text != TextBox_confirmpass.Text)
                            throw new Exception("The two passwords do not match");
                    }

                    newUser.UserFullName = TextBox_Fullname.Text.Replace("'", "''");
                    newUser.UserEmail = TextBox_Email.Text;
                    newUser.MobileNumber = TextBox_MobileNo.Text;

                    //editUserApprover.Save(conn, trx);
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "User saved");
                    if (newUser.IsAdded == true)
                    {
                        newUser.IsAdded = false;
                        newUser.Save(conn, trx);
                    }
                    if (newUser.IsEditied == true)
                    {
                        //newUser = AppUser.LoadAppUser("user_id = '" + int.Parse(Session["u"].ToString()) + "'");

                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                        cmd.CommandText = @"SELECT g.group_name FROM group_users gu INNER JOIN groups g ON gu.group_id = g.group_id  WHERE user_id = " + newUser.UserId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        currGroups = dt.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToList();
                        newUser.Save(conn, trx);
                    }

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

                    foreach (System.Web.UI.WebControls.ListItem selectedAlert in checkbox_list.Items)
                    {
                        if (selectedAlert.Selected)
                        {
                            foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                            {
                                if (SelectedOrganizations.Selected)
                                {
                                    ConnectionFactory.ExecuteQuery(String.Format("delete from ccms_alert_notification where alert_type_id = {0} and organization_id = {1} and user_id={2}", selectedAlert.Value, GetOrganizationIdFromMCN(SelectedOrganizations.Value), newUser.UserId), trx);
                                    new CcmsAlertNotification(long.Parse(selectedAlert.Value), GetOrganizationIdFromMCN(SelectedOrganizations.Value), newUser.UserId).Save(conn, trx);
                                }
                            }
                        }
                    }

                    if (Session["u"] != null)
                    {
                        editAuditLog(newUser, oldUser, trx);
                        var msg = "User [" + newUser.UserFullName.Replace("'", "''") + "] has been modified successfully";
                        if (addedGroups.Any())
                            msg += string.Format(" New Groups assigned to user are {0}.", string.Join(", ", addedGroups));

                        if (currGroups.Any())
                            msg += string.Format(" Groups removed from user are {0}.", string.Join(", ", currGroups));
                        Utility.BuildAuditLog(msg, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyUser);

                    }
                    else
                        Utility.BuildAuditLog("User [" + newUser.UserFullName.Replace("'", "''") + "] has been created successfully", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateUser);

                    if (trx != null)
                        trx.Commit();

                    Session[SessionVars.notice.ToString()] = "User [" + newUser.UserFullName + "] was saved successfully.";
                }
                else
                {
                    AppUserApprover editUserApprover = null;
                    if (Session["u"] != null)
                    {
                        if (AppUser.LoadAppUser("user_login like '" + TextBox_Login_Id.Text + "' and user_id != " + Session["u"].ToString()) != null)
                            throw new Exception("The user id '" + TextBox_Login_Id.Text + "' already exists!");
                        else
                        {
                            editUserApprover = new AppUserApprover();
                            newUser = AppUser.LoadAppUserByPk(int.Parse(Session["u"].ToString()));
                            int userid = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

                            editUserApprover.UserModifiedBy = userid;
                            editUserApprover.UserCreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                            editUserApprover.UserCreationTime = newUser.UserCreationTime;
                            editUserApprover.IsActiveDirectoryUser = newUser.IsActiveDirectoryUser;
                            editUserApprover.UserModificationTime = DateTime.Now;
                            editUserApprover.UserType = DropDownList_User_Type.SelectedValue;
                            editUserApprover.IsEditied = true;
                            editUserApprover.ApprovalStatus = "Pending";
                        }
                    }
                    else
                    {
                        if (AppUser.LoadAppUser("user_login like '" + TextBox_Login_Id.Text + "'") != null)
                            throw new Exception("The user id '" + TextBox_Login_Id.Text + "' already exists!");
                        else
                        {
                            editUserApprover = new AppUserApprover();

                            editUserApprover.UserIsActive = true;
                            editUserApprover.UserCreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                            editUserApprover.UserCreationTime = DateTime.Now;
                            editUserApprover.UserType = DropDownList_User_Type.SelectedValue;
                            editUserApprover.IsAdded = true;
                            editUserApprover.ApprovalStatus = "Pending";
                        }
                    }
                    if (DropDownList_Manager.SelectedIndex > 0)
                        editUserApprover.EmployeeManagerId = int.Parse(DropDownList_Manager.SelectedItem.Value);
                    editUserApprover.UserLogin = TextBox_Login_Id.Text;

                    if ((!editUserApprover.IsActiveDirectoryUser && hidden_dirtyFlagForUserPassword.Value == "1") || Session["u"] == null)
                        editUserApprover.UserPassword = Encryption.Cryptic.EncryptString(TextBox_Password.Text);
                    else
                    {
                        editUserApprover.UserPassword = TextBox_Password.Text;
                    }

                    if (!editUserApprover.IsActiveDirectoryUser)
                    {
                        if (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).ApplyPasswordPolicy == true)
                            if (!CheckPasswordPolicy(TextBox_Password.Text, Literal_error))
                                return;

                        if (TextBox_Password.Text == string.Empty)
                        {
                            throw new Exception("Please Enter Password");
                        }
                        if (TextBox_Password.Text != TextBox_confirmpass.Text)
                        {
                            throw new Exception("The two passwords do not match");
                        }
                    }

                    editUserApprover.UserFullName = TextBox_Fullname.Text.Replace("'", "''");
                    editUserApprover.UserEmail = TextBox_Email.Text;
                    editUserApprover.MobileNumber = TextBox_MobileNo.Text;

                    editUserApprover.Save(conn, trx);
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "User saved");
                    if (editUserApprover.IsAdded == true)
                    {
                        newUser = new AppUser();
                        newUser.UserFullName = editUserApprover.UserFullName.Replace("'", "''");
                        newUser.CitId = editUserApprover.CitId;
                        newUser.EmployeeManagerId = editUserApprover.EmployeeManagerId;
                        newUser.IsActiveDirectoryUser = editUserApprover.IsActiveDirectoryUser;
                        newUser.ManagerId = editUserApprover.ManagerId;
                        newUser.MobileNumber = editUserApprover.MobileNumber;
                        newUser.RetryAttempt = editUserApprover.RetryAttempt;
                        newUser.UserCreatedBy = editUserApprover.UserCreatedBy;
                        newUser.UserCreationTime = editUserApprover.UserCreationTime;
                        newUser.UserEmail = editUserApprover.UserEmail;
                        newUser.UserIsActive = false;
                        newUser.UserLastLoginTime = editUserApprover.UserLastLoginTime;
                        newUser.UserLogin = editUserApprover.UserLogin;
                        newUser.UserModificationTime = editUserApprover.UserModificationTime;
                        newUser.UserModifiedBy = editUserApprover.UserModifiedBy;
                        newUser.UserPassword = editUserApprover.UserPassword;
                        newUser.UserType = editUserApprover.UserType;
                        newUser.ApprovalStatus = editUserApprover.ApprovalStatus;
                        newUser.IsAdded = editUserApprover.IsAdded;
                        newUser.IsDeleted = editUserApprover.IsDeleted;
                        newUser.IsEditied = editUserApprover.IsEditied;
                        newUser.Save(conn, trx);
                    }
                    if (editUserApprover.IsEditied == true)
                    {
                        newUser = AppUser.LoadAppUser("user_id = '" + int.Parse(Session["u"].ToString()) + "'");
                        newUser.IsEditied = editUserApprover.IsEditied;
                        newUser.ApprovalStatus = editUserApprover.ApprovalStatus;
                        newUser.UserModificationTime = DateTime.Now;
                        newUser.UserModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;

                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                        cmd.CommandText = @"SELECT g.group_name FROM group_users gu INNER JOIN groups g ON gu.group_id = g.group_id  WHERE user_id = " + newUser.UserId;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        currGroups = dt.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToList();

                        newUser.Save(conn, trx);
                    }

                    ConnectionFactory.ExecuteQuery("delete from group_users_approver where user_id = " + newUser.UserId, trx);


                    foreach (System.Web.UI.WebControls.ListItem SelectedGroups in CheckBoxList_UserGroups.Items)
                        if (SelectedGroups.Selected)
                        {
                            if (editUserApprover.IsEditied)
                            {
                                if (!currGroups.Contains(SelectedGroups.Text))
                                    addedGroups.Add(SelectedGroups.Text);
                                else
                                    currGroups.Remove(SelectedGroups.Text);
                            }
                            (new GroupUsersApprover(newUser.UserId, int.Parse(SelectedGroups.Value))).Save(conn, trx);
                        }
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "groups for the user saved");

                    ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + newUser.UserId, trx);
                    foreach (ComponentArt.Web.UI.TreeViewNode node in UserATMsTreeView.CheckedNodes)
                        if (node.ID.Substring(0, 1) != "r")
                            (new UserATMsApprover(newUser.UserId, int.Parse(node.ID.Substring(1)))).Save(conn, trx);
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "atms for the user saved");

                    if (editUserApprover.UserType.Equals("NCR"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                            if (SelectedOrganizations.Selected)
                                (new UserOrganizationApprover(newUser.UserId, SelectedOrganizations.Value)).Save(conn, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "organization(s) for NCR user saved");
                    }
                    else if (editUserApprover.UserType.Equals("BANK"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                            if (SelectedOrganizations.Selected)
                            {
                                (new UserOrganizationApprover(newUser.UserId, SelectedOrganizations.Value)).Save(conn, trx);
                            }
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "organization for Bank user saved");
                    }
                    else if (editUserApprover.UserType.Equals("CIT"))
                    {
                        ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + newUser.UserId, trx);
                        foreach (System.Web.UI.WebControls.ListItem selectedCIT in CheckBoxList_CIT.Items)
                            if (selectedCIT.Selected)
                                (new CcmsUserCitApprover((long)newUser.UserId, long.Parse(selectedCIT.Value))).Save(conn, trx);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "cit for user saved");
                    }

                    //ConnectionFactory.ExecuteQuery("delete from ccms_organization_user where user_id = " + newUser.UserId, trx);

                    //foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                    //    if (SelectedOrganizations.Selected)
                    //    {
                    //        CcmsOrganization ccmsOrg = CcmsOrganization.LoadCcmsOrganization("mcn='" + SelectedOrganizations.Value + "'");
                    //        (new CcmsOrganizationUser(ccmsOrg.Id, newUser.UserId)).Save(conn, trx);
                    //    }
                    //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "organizations for the user saved in ccms table");
                    foreach (System.Web.UI.WebControls.ListItem selectedAlert in checkbox_list.Items)
                    {
                        if (selectedAlert.Selected)
                        {
                            foreach (System.Web.UI.WebControls.ListItem SelectedOrganizations in CheckBoxList_Organization.Items)
                            {
                                if (SelectedOrganizations.Selected)
                                {
                                    ConnectionFactory.ExecuteQuery(String.Format("delete from ccms_alert_notification_approver where alert_type_id = {0} and organization_id = {1} and user_id={2}", selectedAlert.Value, GetOrganizationIdFromMCN(SelectedOrganizations.Value), newUser.UserId), trx);
                                    new CcmsAlertNotificationApprover(long.Parse(selectedAlert.Value), GetOrganizationIdFromMCN(SelectedOrganizations.Value), newUser.UserId).Save(conn, trx);
                                }
                            }

                        }
                    }



                    if (Session["u"] != null)
                    {
                        var msg = "Request to modify user [" + newUser.UserFullName.Replace("'", "''") + "] has been sent for approval.";
                        if (addedGroups.Any())
                            msg += string.Format(" New Groups assigned to user are {0}.", string.Join(", ", addedGroups));

                        if (currGroups.Any())
                            msg += string.Format(" Groups removed from user are {0}.", string.Join(", ", currGroups));
                        Utility.BuildAuditLog(msg, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyUser);

                    }
                    else
                        Utility.BuildAuditLog("Request to create user [" + newUser.UserFullName.Replace("'", "''") + "] has been sent for approval.", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateUser);

                    if (trx != null)
                        trx.Commit();

                    Session[SessionVars.notice.ToString()] = "User [" + newUser.UserFullName + "] was saved successfully and request sent for approval.";
                }
                Response.Redirect("Users.aspx", false);
            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>Error while saving user. [" + ex.Message + "]</li>";
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                task.EndTask();
            }
        }

        public void editAuditLog(AppUser objCurrent, AppUser objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "User [" + objPrevious.UserFullName.Replace("'", "''") + "] has been modified successfully", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyUser, trx);
            }

        }

        protected void BuildTree()
        {
            UserATMsTreeView.Nodes.Clear();
            bool noRegion = true;

            jsStr.Append("<script>var ary1 = [");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select title, atm_id, region_id from atm where is_active = 1 and atm_id in 
            (select atm_id from user_atms where user_id = " + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ") order by title;" +
            "select region_name, region_id, parent_region_id, is_organization, mcn from region where is_active = 1 order by region_name";
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
                    ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(dbRow["title"].ToString(), (int)dbRow["atm_id"]);
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
            atmsDS = new DataSet();
            adapter.Fill(atmsDS);

            //Adding Organization Node
            foreach (DataRow dbRow in atmsDS.Tables[0].Rows)
            {
                ComponentArt.Web.UI.TreeViewNode organizationNode = CreateRegionNode(dbRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
                UserATMsTreeView.Nodes.Add(organizationNode);

                //Adding Regions in Organization and ATMs in Region Node
                foreach (DataRow dbRegionRow in atmsDS.Tables[1].Rows)
                {
                    ComponentArt.Web.UI.TreeViewNode regionNode = CreateRegionNode(dbRegionRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)dbRegionRow["region_id"], bool.Parse(dbRegionRow["is_organization"].ToString()));
                    organizationNode.Nodes.Add(regionNode);

                    foreach (DataRow dbATMRow in atmsDS.Tables[2].Select(" region_id = " + (int)dbRegionRow["region_id"]))
                    {
                        ComponentArt.Web.UI.TreeViewNode atmNode = CreateATMNode(dbATMRow["title"].ToString(), (int)dbATMRow["atm_id"]);
                        regionNode.Nodes.Add(atmNode);
                    }
                }
            }
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
                ComponentArt.Web.UI.TreeViewNode organizationNode = CreateRegionNode(dbRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
                UserATMsTreeView.Nodes.Add(organizationNode);

                //Adding Regions in Organization and ATMs in Region Node
                foreach (DataRow dbRegionRow in atmsDS.Tables[1].Select("parent_region_id = " + (int)dbRow["region_id"]))
                {
                    ComponentArt.Web.UI.TreeViewNode regionNode = CreateRegionNode(dbRegionRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)dbRegionRow["region_id"], bool.Parse(dbRegionRow["is_organization"].ToString()));
                    organizationNode.Nodes.Add(regionNode);

                    foreach (DataRow dbATMRow in atmsDS.Tables[2].Select(" region_id = " + (int)dbRegionRow["region_id"]))
                    {
                        ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(dbATMRow["title"].ToString(), (int)dbATMRow["atm_id"]);
                        regionNode.Nodes.Add(childNode);
                    }
                }
            }


        }

        protected void PopulateSubTree(DataRow parentRegionRow, ComponentArt.Web.UI.TreeViewNode node, bool allowEditingRegion, bool allowEditingATM, string parentMCN)
        {
            ArrayList selectedOrganizations = new ArrayList();
            foreach (System.Web.UI.WebControls.ListItem listItem in CheckBoxList_Organization.Items)
                if (listItem.Selected)
                    selectedOrganizations.Add(listItem.Value);

            foreach (DataRow childRow in atmsDS.Tables[1].Select("parent_region_id=" + parentRegionRow["region_id"]))
            {
                if (bool.Parse(childRow["is_organization"].ToString()))
                {
                    if (selectedOrganizations.Contains(childRow["mcn"].ToString()) || selectedOrganizations.Contains(parentMCN))
                    {
                        ComponentArt.Web.UI.TreeViewNode childNode = CreateRegionNode(childRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)childRow["region_id"], bool.Parse(childRow["is_organization"].ToString()));
                        node.Nodes.Add(childNode);
                        jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                        PopulateSubTree(childRow, childNode, false, false, parentMCN);
                    }
                }
                else
                {
                    if (selectedOrganizations.Contains(TraceOrganizationMCN(int.Parse(childRow["region_id"].ToString()))) || selectedOrganizations.Contains(parentMCN))
                    {
                        ComponentArt.Web.UI.TreeViewNode childNode = CreateRegionNode(childRow["region_name"].ToString(), isTreeviewNodeExpanded, (int)childRow["region_id"], bool.Parse(childRow["is_organization"].ToString()));
                        node.Nodes.Add(childNode);
                        jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
                        PopulateSubTree(childRow, childNode, false, false, parentMCN);
                    }
                }
            }

            foreach (DataRow childRow in parentRegionRow.GetChildRows("ATMRegionRelation"))
            {
                ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(childRow["title"].ToString(), (int)childRow["atm_id"]);
                node.Nodes.Add(childNode);
                jsStr.Append("['" + childNode.ID + "','" + childNode.Text + "'],");
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

        protected string TraceOrganizationMCN(int region_id)
        {
            if (traceOrganizationMCN.Length == 0)
            {

                if (Region.LoadRegionByPk(region_id).IsOrganization == true)
                    return Region.LoadRegionByPk(region_id).MCN;
                else
                    return TraceOrganizationMCN(Region.LoadRegionByPk(region_id).ParentRegionId.Value);
            }
            else
                return traceOrganizationMCN;
        }

        protected void DropDownList_Organization_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildTree();
        }

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

                if (Session["userApprover"] != null)
                {
                    AppUserApprover user = AppUserApprover.LoadAppUseApproverrByuserLogin(Session["userApprover"].ToString());
                    AppUser _appUser = AppUser.LoadAppUser("user_login = '"+user.UserLogin+"'");

                    //userId = int.Parse(Session["u"].ToString());
                    //AppUser user = AppUser.LoadAppUserByPk(userId);
                    DropDownList_User_Type.SelectedValue = _appUser.UserType;
                    builder.Append(" and user_id <> " + _appUser.UserId);
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
                {
                    CheckBoxList_Organization.Items.Add(new ListItem(SqlReader.GetString(0) + "[" + SqlReader.GetString(1) + "]", SqlReader.GetString(0)));

                }
                if (CheckBoxList_Organization.Items.Count > 0)
                    CheckBoxList_Organization.Items[0].Selected = true;
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

        /*
         * If multiple organizations are selected for user type "Bank" returns false;
         */
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

        private void SelectTreeNodesFromUserState()
        {


            if (Session["u"] != null)
            {
                SqlCommand cmd = null;
                try
                {
                    ArrayList atmsList = new ArrayList();
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "SELECT atm_ID FROM USER_atms WHERE USER_ID = " + Session["u"].ToString();
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
            if (Session["userApprover"] != null)
            {
                AppUserApprover user = AppUserApprover.LoadAppUseApproverrByuserLogin(Session["userApprover"].ToString());
                AppUser _appUser = AppUser.LoadAppUser("user_login = '"+user.UserLogin+"'");
                SqlCommand cmd = null;
                try
                {
                    ArrayList atmsList = new ArrayList();
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "SELECT atm_ID FROM USER_atms_approver WHERE USER_ID = " + _appUser.UserId;
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

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            //RequiredFieldValidator_TextBox_Login_Id.EnableClientScript = false;
            //RequiredFieldValidator_TextBox_Login_Id.EnableClientScript = false;
            Response.Redirect("Users.aspx", false);
        }

        protected void Button_ApproveUser_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trx = null;
            conn = ConnectionFactory.GetNewConnection();
            try
            {
                string msg = null;
                AppUserApprover userApprover = AppUserApprover.LoadAppUseApproverrByuserLogin(Session["userApprover"].ToString());
                AppUser _user = AppUser.LoadAppUser("user_login = '"+userApprover.UserLogin+"'");
                AppUser _olduser = AppUser.LoadAppUser("user_login = '"+userApprover.UserLogin+"'");
                conn.Open();
                trx = conn.BeginTransaction();
                if (_user.IsAdded == true)
                {
                    _user.ApprovalStatus = "Approved";
                    _user.IsAdded = false;
                    _user.UserIsActive = true;
                    _user.Save(conn, trx);



                    ConnectionFactory.ExecuteQuery("insert into group_users select * from group_users_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);

                    ConnectionFactory.ExecuteQuery("insert into user_ATMs select * from user_ATMs_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);

                    UserOrganizationApprover _UserOrganizationApprover = UserOrganizationApprover.LoadUserOrganizationApprover("user_id = " + _user.UserId);
                    //ConnectionFactory.ExecuteQuery(" insert into user_organization (user_id, org_mcn) select user_id, org_mcn from user_organization_approver where user_id = " + _user.UserId, trx);
                    if (_UserOrganizationApprover != null)
                    {
                        UserOrganization userOrganization = new UserOrganization();
                        userOrganization.UserId = _UserOrganizationApprover.UserId;
                        userOrganization.OrgMcn = _UserOrganizationApprover.OrgMcn;
                        userOrganization.Save(conn, trx);
                        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);
                    }

                    ConnectionFactory.ExecuteQuery("insert into ccms_user_cit (user_id,cit_id) select user_id,cit_id from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);

                    ConnectionFactory.ExecuteQuery("insert into ccms_alert_notification (alert_type_id,organization_id,user_id) select alert_type_id,organization_id,user_id from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);

                    ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login = '" + _user.UserLogin + "'", trx);

                    trx.Commit();
                    msg = "Create request for user [" + _user.UserLogin + "] has been Approved successfully.";

                }
                if (_user.IsDeleted == true)
                {
                    SqlCommand cmd = null;
                    //AppUser user = AppUser.LoadAppUserByPk(int.Parse(e.CommandArgument.ToString()));
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trx = cmd.Connection.BeginTransaction();
                    cmd.CommandText = "DeleteUser";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@user_id", SqlDbType.Int);
                    cmd.Parameters[0].Value = _user.UserId;
                    cmd.Transaction = trx;
                    cmd.ExecuteNonQuery();

                    ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login  = '" + _user.UserLogin + "'");
                    trx.Commit();
                    Utility.BuildAuditLog(_user.UserFullName.Replace("'", "''") + " is Deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteUser);
                    msg = "Delete request for user [" + _user.UserLogin + "] has been Approved successfully.";
                }
                if (_user.IsEditied == true)
                {
                    _user.UserFullName = userApprover.UserFullName.Replace("'", "''");
                    _user.CitId = userApprover.CitId;
                    _user.EmployeeManagerId = userApprover.EmployeeManagerId;
                    _user.IsActiveDirectoryUser = userApprover.IsActiveDirectoryUser;
                    _user.ManagerId = userApprover.ManagerId;
                    _user.MobileNumber = userApprover.MobileNumber;
                    _user.RetryAttempt = userApprover.RetryAttempt;
                    _user.UserCreatedBy = userApprover.UserCreatedBy;
                    _user.UserCreationTime = userApprover.UserCreationTime;
                    _user.UserEmail = userApprover.UserEmail;
                    _user.UserLastLoginTime = userApprover.UserLastLoginTime;
                    _user.UserLogin = userApprover.UserLogin;
                    _user.UserModificationTime = userApprover.UserModificationTime;
                    _user.UserModifiedBy = userApprover.UserModifiedBy;
                    if (userApprover.IsActiveDirectoryUser == false)
                    {
                        _user.UserPassword = userApprover.UserPassword;
                    }
                    _user.UserType = userApprover.UserType;
                    _user.ApprovalStatus = "Approved";
                    _user.IsAdded = false;
                    _user.IsDeleted = false;
                    _user.IsEditied = false;
                    _user.UserIsActive = true;
                    _user.Save(conn, trx);

                    editAuditLog(_user, _olduser, trx);
                    ConnectionFactory.ExecuteQuery("DELETE FROM group_users where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("insert into group_users select * from group_users_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);

                    ConnectionFactory.ExecuteQuery("delete from user_ATMs where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("insert into user_ATMs select * from user_ATMs_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);

                    ConnectionFactory.ExecuteQuery("delete from user_organization where user_id = " + _user.UserId, trx);
                    //ConnectionFactory.ExecuteQuery("SET IDENTITY_INSERT ccms_organization ON");
                    //ConnectionFactory.ExecuteQuery("insert into user_organization (user_id, org_mcn) select user_id, org_mcn from user_organization_approver where user_id = " + _user.UserId, trx);

                    UserOrganizationApprover _UserOrganizationApprover = UserOrganizationApprover.LoadUserOrganizationApprover("user_id = " + _user.UserId);

                    if (_UserOrganizationApprover != null)
                    {
                        UserOrganization userOrganization = new UserOrganization();
                        userOrganization.UserId = _UserOrganizationApprover.UserId;
                        userOrganization.OrgMcn = _UserOrganizationApprover.OrgMcn;
                        userOrganization.Save(conn, trx);
                        ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);
                    }

                    ConnectionFactory.ExecuteQuery("delete from ccms_user_cit where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("insert into ccms_user_cit(user_id,cit_id) select user_id,cit_id from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);

                    SqlCommand cmd = null;
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "delete from ccms_alert_notification where user_id = " + _user.UserId;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "insert into ccms_alert_notification (alert_type_id,organization_id,user_id) select alert_type_id,organization_id,user_id from ccms_alert_notification_approver where user_id = " + _user.UserId;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from ccms_alert_notification_approver where user_id = " + _user.UserId;
                    cmd.ExecuteNonQuery();

                    ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login = '" + _user.UserLogin + "'", trx);
                    
                    trx.Commit();
                    msg = "Update request for User [" + _user.UserLogin + "] has been Approved successfully.";
                }

                Session[SessionVars.notice.ToString()] = msg;
                Utility.BuildAuditLog(msg, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ApproveUser);
            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();
                Literal_error.Text = "<li>An error occured while Approving User! " + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("Approve group", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                Session["userApprover"] = null;

                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                Response.Redirect("ApproveUser.aspx", false);

            }

        }

        protected void Button_DeclineUser_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trx = null;
            conn = ConnectionFactory.GetNewConnection();
            try
            {
                string msg = null;
                AppUserApprover userApprover = AppUserApprover.LoadAppUseApproverrByuserLogin(Session["userApprover"].ToString());
                AppUser _user = AppUser.LoadAppUser("user_login='"+userApprover.UserLogin+"'");
                conn.Open();
                trx = conn.BeginTransaction();
                if (_user.IsAdded == true)
                {
                    //ConnectionFactory.ExecuteQuery("insert into group_users select * from group_users_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);

                    //ConnectionFactory.ExecuteQuery("insert into user_ATMs select * from user_ATMs_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);

                    //UserOrganizationApprover.UserOrganizationApproverReader reader =  UserOrganizationApprover.ExecuteReader("user_id = " + _user.UserId);
                    //while (reader.Read())
                    //    (new UserOrganization(reader.CurrentUserOrganizationApprover.UserId, reader.CurrentUserOrganizationApprover.OrgMcn)).Save(conn, trx);
                    ////ConnectionFactory.ExecuteQuery("insert into user_organization (user_id, org_mcn) select user_id, org_mcn from user_organization_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);

                    //ConnectionFactory.ExecuteQuery("insert into ccms_user_cit(user_id,cit_id) select user_id,cit_id from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);

                    //ConnectionFactory.ExecuteQuery("insert into ccms_alert_notification (alert_type_id,organization_id,user_id) select alert_type_id,organization_id,user_id from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
                    ////////////
                    ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login  = '" + _user.UserLogin + "'");

                    //ConnectionFactory.ExecuteQuery("update app_user set approval_status = 'Decline', is_added = 0, user_is_active = 0 where user_login  = '" + _user.UserLogin + "'");
                    ConnectionFactory.ExecuteQuery("delete from app_user where user_login  = '" + _user.UserLogin + "'");

                    trx.Commit();
                    msg = "Request to create user [" + _user.UserLogin + "] has been Declined.";
                }
                if (_user.IsDeleted == true)
                {
                    _user.ApprovalStatus = "Decline";
                    _user.IsDeleted = false;
                    //_user.UserIsActive = true;
                    _user.Save(conn, trx);
                    ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login  = '" + _user.UserLogin + "'");
                    ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);
                    trx.Commit();
                    msg = "Request to delete user [" + _user.UserLogin + "] has been Declined.";
                }
                if (_user.IsEditied == true)
                {
                    _user.ApprovalStatus = "Decline";
                    _user.IsEditied = false;
                    _user.UserIsActive = true;
                    _user.Save(conn, trx);
                    ConnectionFactory.ExecuteQuery("delete from app_user_approver where user_login  = '" + _user.UserLogin + "'");
                    ConnectionFactory.ExecuteQuery("DELETE FROM group_users_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from user_ATMs_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from user_organization_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_user_cit_approver where user_id = " + _user.UserId, trx);
                    ConnectionFactory.ExecuteQuery("delete from ccms_alert_notification_approver where user_id = " + _user.UserId, trx);

                    trx.Commit();
                    msg = "Request to update user [" + _user.UserLogin + "] has been Decline.";
                }

                Session[SessionVars.notice.ToString()] = msg;
                Utility.BuildAuditLog(msg, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ApproveUser);
            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();
                Literal_error.Text = "<li>An error occured while Decline User! " + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("Approve group", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                Session["userApprover"] = null;

                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                Response.Redirect("ApproveUser.aspx", false);
                Literal_error.Text = "<li>User Approval Decline!";
            }

        }
    }
}