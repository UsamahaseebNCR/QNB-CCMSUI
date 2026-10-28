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
using System.Text;
using System.Collections.Generic;
using Avanza.CCMS.DAL;

using ComponentArt.Web.UI;
using Newtonsoft.Json;

namespace Avanza.iSuite.UI
{
    public partial class EditAlertTemplate : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            try
            {

                if (!Page.IsPostBack)
                {
                    //if (Session["alert_type_id"] != null)
                    //    Session["alert_type_id"] = Request.QueryString["alert_type_id"];


                    if (Session["alert_type_id"] != null)
                    {
                        PopulateOrganizations();
                        DropDownList_Organization_SelectedIndexChanged(this, null);

                        cmd = ConnectionFactory.GetNewCommand(true);
                        cmd.CommandText = @"select alert_type_name,alert_additional_text,alert_default_text,send_email_notification,open_ticket_in_gasper,tpa_code,tpa_value
                                from alert_type
                                where alert_type_id=" + (string)Session["alert_type_id"];
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            Label_AlertType.Text = reader[0].ToString();// +": ";
                            TextBox_additional.Text = reader[1].ToString();
                            TextBox_default.Text = reader[2].ToString();

                            if (reader[4] != null)
                                CheckBox_OpenTicket.Checked = reader.GetBoolean(4);

                            if (reader[3] != null)
                                CheckBox_EmailNotification.Checked = reader.GetBoolean(3);


                            if (reader[5] != null)
                                TextBox_TPACode.Text = reader[5].ToString();

                            if (reader[6] != null)
                                TextBox_TPAValue.Text = reader[6].ToString();
                        }
                    }
                    else
                    {
                        Response.Redirect("AccessDenied.aspx");
                    }
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("LoadAlertTemplates", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }

        }


        protected void Button_SaveUpdateEditAlertTemplate_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("SaveUpdateAlertTemplate");
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlTransaction trx = null;
            bool userSelected = false;
            string SQL = null;
            bool result = false;

            try
            {
                ValidateInputs();

                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                long selectedAlertTypeId = long.Parse(Session["alert_type_id"].ToString());
                long selectedOrganizationId = long.Parse(DropDownList_Organization.SelectedValue);
                //SQL = "update alert_type set alert_additional_text='" + TextBox_additional.Text + "' where alert_type_id=" + selectedAlertTypeId;

                ConnectionFactory.ExecuteQuery(String.Format("delete from ccms_alert_notification where alert_type_id = {0} and organization_id = {1}", selectedAlertTypeId, selectedOrganizationId), trx);

                //foreach (TreeViewNode currentNode in AlertNotificationUsersTreeView.CheckedNodes)
                //{
                //    if (!currentNode.Value.Equals("-1"))
                //    {
                for (int i = 0; i < ListBox1.Items.Count; i++)
                    if (ListBox1.Items[i].Selected)
                        new CcmsAlertNotification(selectedAlertTypeId, selectedOrganizationId, long.Parse(ListBox1.Items[i].Value)).Save(conn, trx);
                //        userSelected = true;
                //    }
                //}

                AlertType alertType = AlertType.LoadAlertTypeByPk(int.Parse(selectedAlertTypeId.ToString()));
                AlertType oldAlertType = AlertType.LoadAlertTypeByPk(int.Parse(selectedAlertTypeId.ToString()));

                if (alertType != null)
                {
                    alertType.AlertAdditionalText = TextBox_additional.Text;
                    alertType.OpenTicketInGasper = CheckBox_OpenTicket.Checked;
                    alertType.SendEmailNotification = CheckBox_EmailNotification.Checked;
                    if (TextBox_TPACode.Text.Length > 0)
                        alertType.TpaCode = TextBox_TPACode.Text;
                    if (TextBox_TPAValue.Text.Length > 0)
                        alertType.TpaValue = TextBox_TPAValue.Text;
                    alertType.Save(trx.Connection, trx);
                }

                //if (userSelected)
                //    SQL = "update alert_type set alert_additional_text='" + TextBox_additional.Text + "',send_email_notification=1 where alert_type_id=" + selectedAlertTypeId;
                //else
                //    SQL = "update alert_type set alert_additional_text='" + TextBox_additional.Text + "',send_email_notification=null where alert_type_id=" + selectedAlertTypeId;
                //ConnectionFactory.ExecuteQuery(SQL, trx);


                Literal_error.Text = "<li>" + "Alert Templates Set Successfully." + "</li>";
                //Utility.BuildAuditLog("Alert: "+Label_AlertType.Text + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyAlertTemplate);
                editAuditLog(alertType, oldAlertType, trx);

                Session[SessionVars.notice.ToString()] = "Alert updated successfully.";
                Response.Redirect("alertTemplate.aspx", false);

                if (trx != null)
                {
                    trx.Commit();
                }


            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }

                Literal_error.Text = "<li>An error occured while saving Alert Template." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();

            }
        }

        public void editAuditLog(AlertType objCurrent, AlertType objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Alert: " + objPrevious.AlertTypeName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyAlertTemplate, trx);
            }

        }

        protected void DropDownList_Organization_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlCommand cmd = null;
            try
            {
                //AlertNotificationUsersTreeView.Nodes.Clear();

                int alertTypeId = int.Parse(Session["alert_type_id"].ToString());
                int organizationId = int.Parse(DropDownList_Organization.SelectedValue);

                //TreeViewNode ncrNode = new TreeViewNode();
                //ncrNode.Text = "NCR User(s)";
                //ncrNode.Value = "-1";
                //ncrNode.ShowCheckBox = true;

                //TreeViewNode bankNode = new TreeViewNode();
                //bankNode.Text = "Bank User(s)";
                //bankNode.Value = "-1";
                //bankNode.ShowCheckBox = true;

                //TreeViewNode citNode = new TreeViewNode();
                //citNode.Text = "CIT User(s)";
                //citNode.Value = "-1";
                //citNode.ShowCheckBox = true;

                cmd = ConnectionFactory.GetNewCommand(true);

                AppUser loggedInUser = (AppUser)Session[SessionVars.user.ToString()];

                if (loggedInUser.UserType.Equals("NCR"))
                {
                    cmd.CommandText = "GetUserListForNCRUser";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@userId", SqlDbType.BigInt).Value = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    cmd.Parameters.Add("@organizationId", SqlDbType.BigInt).Value = organizationId;
                }
                else if (loggedInUser.UserType.Equals("BANK"))
                {
                    cmd.CommandText = String.Format(@"select 
	                                app_user.*
                                from 
	                                app_user inner join ccms_user_organization on app_user.user_id = ccms_user_organization.user_id
                                where ccms_user_organization.organization_id in (select organization_id from ccms_user_organization where user_id = {0})", loggedInUser.UserId);
                }
                else if (loggedInUser.UserType.Equals("CIT"))
                {
                    cmd.CommandText = String.Format(@"select 
	                            app_user.*
                            from	
	                            app_user inner join ccms_user_cit on app_user.user_id = ccms_user_cit.user_id
                            where ccms_user_cit.cit_id in (select cit_id from ccms_user_cit where user_id = {0})", loggedInUser.UserId);
                }

                DataTable dtUser = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtUser);

                if (dtUser != null && dtUser.Rows.Count > 0)
                {

                    //if (loggedInUser.UserType.Equals("NCR"))
                    //{

                    //    //AlertNotificationUsersTreeView.Nodes.Add(ncrNode);
                    //    //AlertNotificationUsersTreeView.Nodes.Add(bankNode);
                    //    //AlertNotificationUsersTreeView.Nodes.Add(citNode);

                    //    //PopulateUserNodes(ncrNode, dtUser, "user_type = 'NCR'");
                    //    //PopulateUserNodes(bankNode, dtUser, "user_type = 'BANK'");
                    //    //PopulateUserNodes(citNode, dtUser, "user_type = 'CIT'");

                    //}
                    //else if (loggedInUser.UserType.Equals("BANK"))
                    //{
                    //    //AlertNotificationUsersTreeView.Nodes.Add(bankNode);

                    //    //PopulateUserNodes(bankNode, dtUser, "user_type = 'BANK'");

                    //}
                    //else if (loggedInUser.UserType.Equals("CIT"))
                    //{
                    //    //AlertNotificationUsersTreeView.Nodes.Add(citNode);

                    //    //PopulateUserNodes(citNode, dtUser, "user_type = 'CIT'");

                    //}

                    ListBox1.DataSource = dtUser;
                    ListBox1.DataTextField = "user_full_name";
                    ListBox1.DataValueField = "user_id";
                    ListBox1.DataBind();
                    ListBox1.SelectionMode = ListSelectionMode.Multiple;

                    CcmsAlertNotification.CcmsAlertNotificationReader reader = CcmsAlertNotification.ExecuteReader("alert_type_id=" + alertTypeId);
                    while (reader.Read())
                    {
                        for (int i = 0; i < ListBox1.Items.Count; i++)
                            if (ListBox1.Items[i].Value == reader.CurrentCcmsAlertNotification.UserId.Value.ToString())
                                ListBox1.Items[i].Selected = true;
                    }
                    reader.Close();
                    //ListBox1. = "Name";
                    //AlertNotificationUsersTreeView.ExpandAll();

                    //PopulateAssignedUsers(alertTypeId, organizationId);

                }

                cmd.Connection.Close();

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("Err", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        private void PopulateAssignedUsers(int alertTypeId, int organizationId)
        {
            String whereExpression = String.Format("alert_type_id = {0} and organization_id = {1}", alertTypeId, organizationId);

            CcmsAlertNotification.CcmsAlertNotificationReader reader = CcmsAlertNotification.ExecuteReader(whereExpression);
            while (reader.Read())
            {
                //foreach (TreeViewNode currentNode in AlertNotificationUsersTreeView.Nodes)
                //{
                //    CheckUser(currentNode, reader.CurrentCcmsAlertNotification.UserId.Value);
                //}
            }

            reader.Close();

        }

        //private void CheckUser(TreeViewNode node, long userId)
        //{
        //    if (node == null)
        //        return;

        //    if (node.Value.Equals(userId.ToString()))
        //        node.Checked = true;

        //    foreach (TreeViewNode childNode in node.Nodes)
        //    {
        //        if (childNode.Nodes.Count > 0)
        //            CheckUser(childNode, userId);

        //        if (childNode.Value.Equals(userId.ToString()))
        //        {
        //            childNode.Checked = true;
        //        }
        //    }
        //}

        //private void PopulateUserNodes(TreeViewNode parentNode, DataTable dtUser, String filterExpression)
        //{
        //    DataRow[] filteredUsers = dtUser.Select(filterExpression);
        //    foreach (DataRow user in filteredUsers)
        //    {
        //        TreeViewNode userNode = new TreeViewNode();
        //        userNode.Text = user["user_login"].ToString() + " [" + user["user_full_name"].ToString() + "] ";
        //        userNode.ID = user["user_id"].ToString();
        //        userNode.Value = user["user_id"].ToString();
        //        userNode.ShowCheckBox = true;

        //        parentNode.Nodes.Add(userNode);
        //    }
        //}

        private void PopulateOrganizations()
        {

            DropDownList_Organization.Items.Clear();

            AppUser loggedInUser = (AppUser)Session[SessionVars.user.ToString()];

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

            if (loggedInUser.UserType.Equals("NCR"))
            {
                cmd.CommandText = String.Format("select region_id, region_name from region "
                                    + " inner join user_organization on region.mcn = user_organization.org_mcn "
                                    + " where user_organization.user_id = {0}", loggedInUser.UserId);

            }
            else if (loggedInUser.UserType.Equals("BANK"))
            {
                cmd.CommandText = String.Format("select region_id, region_name from region "
                                            + " inner join ccms_user_organization on region.region_id = ccms_user_organization.organization_id "
                                            + " where ccms_user_organization.user_id = {0} ", loggedInUser.UserId);

            }
            else if (loggedInUser.UserType.Equals("CIT"))
            {
                cmd.CommandText = String.Format("select distinct a.region_id, a.region_name from region a "
                                                    + " inner join region b on a.region_id = b.parent_region_id "
                                                    + " inner join region_cit on b.region_id = region_cit.region_id "
                                                    + " inner join ccms_user_cit on region_cit.cit_id = ccms_user_cit.cit_id "
                                                    + " where ccms_user_cit.user_id = {0} ", loggedInUser.UserId);

            }


            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dtOrganizations = new DataTable();

            adapter.Fill(dtOrganizations);

            DropDownList_Organization.DataSource = dtOrganizations;
            DropDownList_Organization.DataTextField = "region_name";
            DropDownList_Organization.DataValueField = "region_id";
            DropDownList_Organization.DataBind();

            if (DropDownList_Organization.Items.Count > 0)
            {
                DropDownList_Organization.Items[0].Selected = true;
            }

            //cmd.Connection.Close();
            //adapter.Dispose();
        }


        public void ValidateInputs()
        {

            bool isadditionalContainSpecialChars = CheckSpecialChars(TextBox_additional.Text);
            if (isadditionalContainSpecialChars == true)
            {
                throw new Exception("Additional has invalid character");

            }
            bool isdefaultContainSpecialChars = CheckSpecialChars(TextBox_default.Text);
            if (isdefaultContainSpecialChars == true)
            {
                throw new Exception("Default has invalid character");

            }
            bool isTPACodeContainSpecialChars = CheckSpecialChars(TextBox_TPACode.Text);
            if (isTPACodeContainSpecialChars == true)
            {
                throw new Exception("TPA Code has invalid character");

            }
            bool isTPAValueContainSpecialChars = CheckSpecialChars(TextBox_TPAValue.Text);
            if (isTPAValueContainSpecialChars == true)
            {
                throw new Exception("TPA Value has invalid character");

            }


        }
    }
}