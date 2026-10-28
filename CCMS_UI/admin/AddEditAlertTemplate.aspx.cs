using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

using Avanza.CCMS.DAL;


#region Page Information
/*
 * Navigation Summary
 * ------------------
 * Redirected From: AlertTemplates.aspx [alert_template_id]
 * Redirected To:   AlertTemplates.aspx []
 *
 * Control Flow
 * ------------
 * OnLoad
 *		AlertTemplates.aspx [alert_template_id]
 *			- Build tree.
 *			- Display information about the selected alert template.
 * 
 * Save_Click
 *		- Add/edit information about alert template.
 *
 * Dialog_Save_Click
 *		- Add/edit information about alert escalation.
*/
#endregion

namespace Avanza.iSuite.UI
{
    /// <summary>
    /// Add/Edit alert template.
    /// </summary>
    public partial class AddEditAlertTemplate : BasePage
    {
        #region Member Variables

        /// <summary>
        /// DataSet representing Devices to be displayed in TreeView.
        /// </summary>
        protected DataSet deviceDS = null;

        /// <summary>
        /// Dataset representing Alert Escalations for the alert template.
        /// </summary>
        protected static DataTable alertEscalationDT = null;

        /// <summary>
        /// String representing Alert Template Id.
        /// </summary>
        protected static string alertTemplateId = null;

        #endregion

        #region Member Functions

        /// <summary>
        /// Set default values to all the controls on the page.
        /// </summary>
        protected void SetDefaultValues(int alertTemplateId)
        {
            Avanza.CCMS.DAL.AlertTemplate alertTemplate = null;
            Avanza.CCMS.DAL.AlertTemplateDevices.AlertTemplateDevicesReader deviceReader = null;
            ArrayList deviceList = null;
            Avanza.CCMS.DAL.AlertEscalation.AlertEscalationReader escalations = null;
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dtSeverityLevels = null;

            try
            {

                alertTemplate = Avanza.CCMS.DAL.AlertTemplate.LoadAlertTemplateByPk(alertTemplateId);
                deviceReader = Avanza.CCMS.DAL.AlertTemplateDevices.ExecuteReader("alert_template_id = " + alertTemplateId);
                deviceList = new ArrayList();
                escalations = Avanza.CCMS.DAL.AlertEscalation.ExecuteReader("alert_template_id = " + alertTemplateId);

                TextBox_Type.Text = alertTemplate.AlertTemplateName;
                TextBox_Description.Text = alertTemplate.AlertTemplateDesc;

                while (deviceReader.Read())
                    deviceList.Add(deviceReader.CurrentAlertTemplateDevices.DeviceId);

                if (deviceList.Count > 0)
                    for (int i = 0; i < TreeView_Device.Nodes.Count; i++)
                        SetCheck(TreeView_Device.Nodes[i], deviceList);

                while (escalations.Read())
                    alertEscalationDT.Rows.Add(new object[] { escalations.CurrentAlertEscalation.AlertTemplateId, 
                                                            escalations.CurrentAlertEscalation.SeverityId, 
                                                            escalations.CurrentAlertEscalation.Level0Duration, 
                                                            escalations.CurrentAlertEscalation.Level1Duration, 
                                                            escalations.CurrentAlertEscalation.Level2Duration,
                                                            escalations.CurrentAlertEscalation.Level3Duration, 
                                                            escalations.CurrentAlertEscalation.Level4Duration,
                                                            escalations.CurrentAlertEscalation.Level1NotificationCount, 
                                                            escalations.CurrentAlertEscalation.Level2NotificationCount,
                                                            escalations.CurrentAlertEscalation.Level3NotificationCount, 
                                                            escalations.CurrentAlertEscalation.Level4NotificationCount, 
                                                            escalations.CurrentAlertEscalation.Level5NotificationCount, 
                                                            escalations.CurrentAlertEscalation.Level1ReminderInterval, 
                                                            escalations.CurrentAlertEscalation.Level2ReminderInterval, 
                                                            escalations.CurrentAlertEscalation.Level3ReminderInterval,
                                                            escalations.CurrentAlertEscalation.Level4ReminderInterval,
                                                            escalations.CurrentAlertEscalation.Level5ReminderInterval, 
                                                            escalations.CurrentAlertEscalation.Level1EmailEnabled,
                                                            escalations.CurrentAlertEscalation.Level2EmailEnabled, 
                                                            escalations.CurrentAlertEscalation.Level3EmailEnabled,
                                                            escalations.CurrentAlertEscalation.Level4EmailEnabled,
                                                            escalations.CurrentAlertEscalation.Level5EmailEnabled, 
                                                            escalations.CurrentAlertEscalation.Level1SmsEnabled,
                                                            escalations.CurrentAlertEscalation.Level2SmsEnabled,
                                                            escalations.CurrentAlertEscalation.Level3SmsEnabled, 
                                                            escalations.CurrentAlertEscalation.Level4SmsEnabled, 
                                                            escalations.CurrentAlertEscalation.Level5SmsEnabled });

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = "select * from severity_level order by severity_id asc";
                adapter = new SqlDataAdapter(cmd);
                dtSeverityLevels = new DataTable();
                adapter.Fill(dtSeverityLevels);

                GridView_AlertEscalation.DataSource = dtSeverityLevels;
                GridView_AlertEscalation.DataBind();
                GridView_AlertEscalation.Visible = true;
            }
            catch (Exception ex)
            {
                //\\UILogger.Log(SeverityLevel.Error, "An error occurred while setting default values.", ex);
                Literal_error.Text = "<li>An error occurred while setting default values: [" + ex.Message + "]</li>";
            }
            finally
            {
                //\\UILogger.StepOutContext();

                if (deviceReader != null)
                    deviceReader.Close();

                if (escalations != null)
                    escalations.Close();

                if (adapter != null)
                    adapter.Dispose();

                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Dispose();
                }
            }
        }

        /// <summary>
        /// Builds tree containing parent and child Devices.
        /// </summary>
        protected void BuildTree()
        {
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;

            try
            {
                //\\UILogger.StepInContext("BuildTree");

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select device_id, device_friendly_name, device_number, module_number from device where module_number is not null and is_active = 1 order by device_friendly_name; select device_id, device_friendly_name, device_number, module_number from device where module_number is null and is_active = 1 order by device_friendly_name";
                adapter = new SqlDataAdapter(cmd);
                deviceDS = new DataSet();
                adapter.Fill(deviceDS);

                deviceDS.Relations.Add("ParentChildRelation", deviceDS.Tables[1].Columns["device_number"], deviceDS.Tables[0].Columns["device_number"]);

                foreach (DataRow dbRow in deviceDS.Tables[1].Rows)
                {
                    if (dbRow.IsNull("module_number"))
                    {
                        TreeNode newNode = CreateParentDeviceNode(dbRow["device_friendly_name"].ToString(), true, (int)dbRow["device_id"]);
                        TreeView_Device.Nodes.Add(newNode);

                        BuildSubTree(dbRow, newNode);
                    }
                }
            }
            catch (Exception ex)
            {
                //	\\UILogger.Log(SeverityLevel.Error, "An error occurred while building tree.", ex);
                Literal_error.Text = "<li>An error occurred while building tree: [" + ex.Message + "]</li>";
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Connection.Close();
                    cmd.Dispose();
                }

                if (adapter != null)
                    adapter.Dispose();

                //\\UILogger.StepOutContext();
            }
        }

        /// <summary>
        /// Recursive function to build sub-tree for a Parent Device containing Child Devices.
        /// </summary>
        /// <param name="parentDeviceRow">DataRow containing Parent Device information.</param>
        /// <param name="node">Node representing Device.</param>
        protected void BuildSubTree(DataRow parentDeviceRow, TreeNode node)
        {
            foreach (DataRow childRow in deviceDS.Tables[1].Select("module_number = " + parentDeviceRow["device_number"]))
            {
                TreeNode childNode = CreateParentDeviceNode(childRow["device_friendly_name"].ToString(), true, (int)childRow["device_id"]);
                node.ChildNodes.Add(childNode);

                BuildSubTree(childRow, childNode);
            }

            foreach (DataRow childRow in parentDeviceRow.GetChildRows("ParentChildRelation"))
            {
                TreeNode childNode = CreateChildDeviceNode(childRow["device_friendly_name"].ToString(), (int)childRow["device_id"]);
                node.ChildNodes.Add(childNode);
            }
        }

        /// <summary>
        /// Creates a node representing a Parent Device.
        /// </summary>
        /// <param name="text">Text of the node.</param>
        /// <param name="expanded">Value to determine whether the node will be expanded.</param>
        /// <param name="id">Value of the node.</param>
        /// <returns>A TreeNode representing a Parent Device.</returns>
        protected TreeNode CreateParentDeviceNode(string text, bool expanded, int id)
        {
            TreeNode node = new TreeNode();
            node.Text = text;
            node.Value = "r" + id;
            node.Expanded = expanded;
            node.ShowCheckBox = true;
            node.SelectAction = TreeNodeSelectAction.None;

            return node;
        }

        /// <summary>
        /// Creates a node representing a Child Device.
        /// </summary>
        /// <param name="text">Text of the node.</param>
        /// <param name="id">Value of the node.</param>
        /// <returns>A TreeNode representing a Child Device.</returns>
        protected TreeNode CreateChildDeviceNode(string text, int id)
        {
            TreeNode node = new TreeNode();
            node.Text = text;
            node.Value = "a" + id;
            node.ShowCheckBox = true;
            node.SelectAction = TreeNodeSelectAction.None;

            return node;
        }

        /// <summary>
        /// Automatically checks nodes of a sub-tree when its parent node is checked.
        /// </summary>
        /// <param name="node">Node that is checked.</param>
        /// <param name="selectedItem">Collection of nodes to be checked.</param>
        protected void SetCheck(TreeNode node, ArrayList selectedItem)
        {
            if (selectedItem.IndexOf(int.Parse(node.Value.Substring(1))) >= 0)
                node.Checked = true;

            TreeNodeCollection nodeCol = node.ChildNodes;
            int devices = 0;

            for (int counter = 0; counter < nodeCol.Count; counter++)
            {
                if (nodeCol[counter].Value.IndexOf('r') < 0)
                {
                    if (selectedItem.IndexOf(int.Parse(nodeCol[counter].Value.Substring(1))) >= 0)
                    {
                        nodeCol[counter].Checked = true;
                        devices++;
                    }
                }
                else if (nodeCol[counter].ChildNodes.Count > 0)
                {
                    SetCheck(nodeCol[counter], selectedItem);

                    if (nodeCol[counter].Checked)
                        devices++;
                }
                else
                {
                    nodeCol[counter].Checked = true;
                    devices++;
                }
            }
        }

        /// <summary>
        /// Gets all devices from the database that are already assigned to an alert template other than the template specified.
        /// </summary>
        /// <param name="alertTemplateId">Alert Template Id.</param>
        /// <returns>A list of devices.</returns>
        protected List<int> FillAddedDevices(string alertTemplateId)
        {
            List<int> addedDevices = new List<int>();
            Avanza.CCMS.DAL.AlertTemplateDevices.AlertTemplateDevicesReader reader = null;

            try
            {
                reader = AlertTemplateDevices.ExecuteReader("alert_template_id != " + alertTemplateId);
                while (reader.Read())
                    addedDevices.Add(reader.CurrentAlertTemplateDevices.DeviceId);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return addedDevices;
        }

        #endregion

        #region Event Handlers

        #region Page

        /// <summary>
        /// Raised on the Load event of the Page.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //\\UILogger.OpenContext("AddEditAlertTemplate");
                //\\UILogger.StepInContext("Page_Load");

                if (!CheckRight((int)Permissions.EditAlertTemplate))
                    Response.Redirect("AccessDenied.aspx");
                else if (Callback_LoadEditAlertEscalationDialog.CausedCallback || Callback_EditAlertEscalation.CausedCallback)
                    return;
                else if (!Page.IsPostBack)
                {
                    alertTemplateId = Request.QueryString["alert_template_id"];

                    alertEscalationDT = new DataTable();

                    alertEscalationDT.Columns.Add("alert_template_id", typeof(int));
                    alertEscalationDT.Columns.Add("severity_id", typeof(int));
                    alertEscalationDT.Columns.Add("level0_duration", typeof(int));
                    alertEscalationDT.Columns.Add("level1_duration", typeof(int));
                    alertEscalationDT.Columns.Add("level2_duration", typeof(int));
                    alertEscalationDT.Columns.Add("level3_duration", typeof(int));
                    alertEscalationDT.Columns.Add("level4_duration", typeof(int));
                    alertEscalationDT.Columns.Add("level1_notification_count", typeof(int));
                    alertEscalationDT.Columns.Add("level2_notification_count", typeof(int));
                    alertEscalationDT.Columns.Add("level3_notification_count", typeof(int));
                    alertEscalationDT.Columns.Add("level4_notification_count", typeof(int));
                    alertEscalationDT.Columns.Add("level5_notification_count", typeof(int));
                    alertEscalationDT.Columns.Add("level1_reminder_interval", typeof(int));
                    alertEscalationDT.Columns.Add("level2_reminder_interval", typeof(int));
                    alertEscalationDT.Columns.Add("level3_reminder_interval", typeof(int));
                    alertEscalationDT.Columns.Add("level4_reminder_interval", typeof(int));
                    alertEscalationDT.Columns.Add("level5_reminder_interval", typeof(int));
                    alertEscalationDT.Columns.Add("level1_email_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level2_email_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level3_email_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level4_email_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level5_email_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level1_sms_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level2_sms_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level3_sms_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level4_sms_enabled", typeof(bool));
                    alertEscalationDT.Columns.Add("level5_sms_enabled", typeof(bool));

                    alertEscalationDT.PrimaryKey = new DataColumn[] { alertEscalationDT.Columns["alert_template_id"], alertEscalationDT.Columns["severity_id"] };

                    BuildTree();

                    if (!CheckRight((int)Permissions.EditAlertEscalation))
                        GridView_AlertEscalation.Columns[1].Visible = false;

                    SetDefaultValues(int.Parse(alertTemplateId));

                    Page.SetFocus(Button_Save);
                }
            }
            catch (Exception ex)
            {
                //\\UILogger.Log(SeverityLevel.Error, "An error occurred while loading page.", ex);
                Literal_error.Text = "<li>An error occurred while loading page: [" + ex.Message + "]</li>";
            }
            finally
            {
                //\\UILogger.CloseContext();
            }
        }

        #endregion

        #region Button

        /// <summary>
        /// Raised on the Click event of the Button_Save.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Button_Save_Click(object sender, EventArgs e)
		{
			SqlConnection conn = null;
			SqlTransaction trx = null;
			Avanza.CCMS.DAL.AlertTemplate alertTemplate = null;
			AlertEscalation alertEscalation = null;
			int severityId = 0;
			DataRow row = null;
			object result = null;
			List<int> addedDevices = null;

			try
			{
				//\\UILogger.StepInContext("Button_Save_Click");

                //if (pageSqlIjectionFree)
                //{
					conn = ConnectionFactory.GetNewConnection();
					conn.Open();
					trx = conn.BeginTransaction();

					if (alertTemplateId == null)
						alertTemplate = new Avanza.CCMS.DAL.AlertTemplate();
					else
						alertTemplate = Avanza.CCMS.DAL.AlertTemplate.LoadAlertTemplateByPk(Convert.ToInt32(alertTemplateId));

					result = ConnectionFactory.ExecuteScalar("select top 1 alert_template_id from alert_template where alert_template_name like '" + TextBox_Type.Text + "' " + ((alertTemplateId != null) ? " and alert_template_id != " + alertTemplateId : ""));
					if (result != null)
					{
						if (trx != null)
							trx.Rollback();

						Literal_error.Text = "<li>Alert Template name already exists</li>";
						return;
					}

					alertTemplate.AlertTemplateName = TextBox_Type.Text;
					alertTemplate.AlertTemplateDesc = TextBox_Description.Text;
					alertTemplate.Save(conn, trx);

					addedDevices = FillAddedDevices(alertTemplateId);

					ConnectionFactory.ExecuteQuery("delete from alert_template_devices where alert_template_id = " + alertTemplateId, trx);
					foreach (TreeNode node in TreeView_Device.CheckedNodes)
					{
						if (!addedDevices.Contains(Convert.ToInt32(node.Value.Substring(1)))) {
							AlertTemplateDevices alertTemplateDevices= new  AlertTemplateDevices();
                            alertTemplateDevices.DeviceId = int.Parse(node.Value.Substring(1));
                            alertTemplateDevices.AlertTemplateId = alertTemplate.AlertTemplateId;
                            alertTemplateDevices.Save(conn, trx);
                        
                        }
						else
						{
							if (trx != null)
								trx.Rollback();

							Literal_error.Text = "<li>Device " + node.Text + " already assigned to another template</li>";
							return;
						}
					}

					for (int i = 0; i < GridView_AlertEscalation.Rows.Count; i++)
					{
						severityId = SeverityLevel.LoadSeverityLevel("level_name like '" + GridView_AlertEscalation.Rows[i].Cells[0].Text + "'").SeverityId;
						alertEscalation = AlertEscalation.LoadAlertEscalation("alert_template_id="+ alertTemplate.AlertTemplateId + " and severity_id="+ severityId);
						row = alertEscalationDT.Select("alert_template_id = " + alertTemplate.AlertTemplateId + " and severity_id = " + severityId)[0];

						alertEscalation.Level0Duration = Convert.ToInt32(row["level0_duration"]);
						alertEscalation.Level1Duration = Convert.ToInt32(row["level1_duration"]);
						alertEscalation.Level2Duration = Convert.ToInt32(row["level2_duration"]);
						alertEscalation.Level3Duration = Convert.ToInt32(row["level3_duration"]);
						alertEscalation.Level4Duration = Convert.ToInt32(row["level4_duration"]);

						alertEscalation.Level1NotificationCount = Convert.ToByte(row["level1_notification_count"]);
						alertEscalation.Level2NotificationCount = Convert.ToByte(row["level2_notification_count"]);
						alertEscalation.Level3NotificationCount = Convert.ToByte(row["level3_notification_count"]);
						alertEscalation.Level4NotificationCount = Convert.ToByte(row["level4_notification_count"]);
						alertEscalation.Level5NotificationCount = Convert.ToByte(row["level5_notification_count"]);

						alertEscalation.Level1ReminderInterval = Convert.ToInt32(row["level1_reminder_interval"]);
						alertEscalation.Level2ReminderInterval = Convert.ToInt32(row["level2_reminder_interval"]);
						alertEscalation.Level3ReminderInterval = Convert.ToInt32(row["level3_reminder_interval"]);
						alertEscalation.Level4ReminderInterval = Convert.ToInt32(row["level4_reminder_interval"]);
						alertEscalation.Level5ReminderInterval = Convert.ToInt32(row["level5_reminder_interval"]);

						alertEscalation.Level1EmailEnabled = (bool)row["level1_email_enabled"];
						alertEscalation.Level2EmailEnabled = (bool)row["level2_email_enabled"];
						alertEscalation.Level3EmailEnabled = (bool)row["level3_email_enabled"];
						alertEscalation.Level4EmailEnabled = (bool)row["level4_email_enabled"];
						alertEscalation.Level5EmailEnabled = (bool)row["level5_email_enabled"];

						alertEscalation.Level1SmsEnabled = (bool)row["level1_sms_enabled"];
						alertEscalation.Level2SmsEnabled = (bool)row["level2_sms_enabled"];
						alertEscalation.Level3SmsEnabled = (bool)row["level3_sms_enabled"];
						alertEscalation.Level4SmsEnabled = (bool)row["level4_sms_enabled"];
						alertEscalation.Level5SmsEnabled = (bool)row["level5_sms_enabled"];

						alertEscalation.Save(conn, trx);
					}

					trx.Commit();

					Utility.BuildAuditLog("Alert Template " + alertTemplate.AlertTemplateName + " edited", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditAlertTemplate);

					//\\UILogger.Log(SeverityLevel.Information, "Alert Template " + alertTemplate.AlertTemplateName + " saved.");

					alertEscalationDT = null;

					Response.Redirect("AlertTemplates.aspx", false);
				//}
                //else
                //    Literal_error.Text = "<li>Invalid characters entered.</li>";
			}
			catch (Exception ex)
			{
				if (trx != null)
					trx.Rollback();

				//\\UILogger.Log(SeverityLevel.Error, "An error occurred while saving alert template.", ex);
				Literal_error.Text = "<li>An error occurred while saving alert template: [" + ex.Message + "]</li>";
			}
			finally
			{
				//\\UILogger.StepOutContext();

				if (conn != null)
					conn.Close();
			}
		}

        #endregion

        #endregion

        #region Callbacks

        /// <summary>
        /// Opens Component Art Dialog for editing selected alert escalation.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void LoadEditAlertEscalationDialog(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            DataRow row = null;
            string severityLevelName = "", script = "";

            try
            {
                //..\\UILogger.StepInContext("LoadEditAlertEscalationDialog");

                for (int i = 0; i < 24; i++)
                {
                    script += string.Format(@"
					document.form1.TextBox_Level0_Duration_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level1_Duration_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level2_Duration_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level3_Duration_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level4_Duration_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level5_Duration_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level0_ReminderInterval_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level1_ReminderInterval_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level2_ReminderInterval_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level3_ReminderInterval_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level4_ReminderInterval_Hours.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level5_ReminderInterval_Hours.options[{0}] = new Option('{1}', '{1}');", i, i.ToString());
                }

                for (int i = 0; i < 60; i++)
                {
                    script += string.Format(@"
					document.form1.TextBox_Level0_Duration_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level1_Duration_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level2_Duration_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level3_Duration_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level4_Duration_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level5_Duration_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level0_ReminderInterval_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level1_ReminderInterval_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level2_ReminderInterval_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level3_ReminderInterval_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level4_ReminderInterval_Mins.options[{0}] = new Option('{1}', '{1}');
					document.form1.TextBox_Level5_ReminderInterval_Mins.options[{0}] = new Option('{1}', '{1}');", i, i.ToString());
                }

                e.Output.Write("<script>" + script + "</script>");
                e.Output.Flush();

                severityLevelName = SeverityLevel.LoadSeverityLevelByPk(int.Parse(e.Parameters[0])).LevelName;
                DataRow[] drArray = alertEscalationDT.Select("alert_template_id = " + int.Parse(alertTemplateId) + " and severity_id = " + int.Parse(e.Parameters[0]));
                if (drArray.Length>0)
                    row = alertEscalationDT.Select("alert_template_id = " + int.Parse(alertTemplateId) + " and severity_id = " + int.Parse(e.Parameters[0]))[0];

                if (row != null)
                {
                    TimeSpan level0_duration = TimeSpan.FromSeconds(Convert.ToDouble(row["level0_duration"]));
                    TimeSpan level1_duration = TimeSpan.FromSeconds(Convert.ToDouble(row["level1_duration"]));
                    TimeSpan level2_duration = TimeSpan.FromSeconds(Convert.ToDouble(row["level2_duration"]));
                    TimeSpan level3_duration = TimeSpan.FromSeconds(Convert.ToDouble(row["level3_duration"]));
                    TimeSpan level4_duration = TimeSpan.FromSeconds(Convert.ToDouble(row["level4_duration"]));
                    TimeSpan level1_reminder_interval = TimeSpan.FromSeconds(Convert.ToDouble(row["level1_reminder_interval"]));
                    TimeSpan level2_reminder_interval = TimeSpan.FromSeconds(Convert.ToDouble(row["level2_reminder_interval"]));
                    TimeSpan level3_reminder_interval = TimeSpan.FromSeconds(Convert.ToDouble(row["level3_reminder_interval"]));
                    TimeSpan level4_reminder_interval = TimeSpan.FromSeconds(Convert.ToDouble(row["level4_reminder_interval"]));
                    TimeSpan level5_reminder_interval = TimeSpan.FromSeconds(Convert.ToDouble(row["level5_reminder_interval"]));

                    script = string.Format(@"<script>
                        document.getElementById('TextBox_Dialog_SeverityId').innerText = '{0}';
						document.getElementById('Label_Name').innerText = '{1}';
                        
						document.getElementById('TextBox_Level0_Duration_Days').innerText = '{2}';
                        for(i = 0; i < window.form1.TextBox_Level0_Duration_Hours.length; i++)        
                            if(window.form1.TextBox_Level0_Duration_Hours.options[i].value == '{3}')
                                window.form1.TextBox_Level0_Duration_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level0_Duration_Mins.length; i++)        
                            if(window.form1.TextBox_Level0_Duration_Mins.options[i].value == '{4}')
                                window.form1.TextBox_Level0_Duration_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level1_Duration_Days').innerText = '{5}';
                        for(i = 0; i < window.form1.TextBox_Level1_Duration_Hours.length; i++)        
                            if(window.form1.TextBox_Level1_Duration_Hours.options[i].value == '{6}')
                                window.form1.TextBox_Level1_Duration_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level1_Duration_Mins.length; i++)        
                            if(window.form1.TextBox_Level1_Duration_Mins.options[i].value == '{7}')
                                window.form1.TextBox_Level1_Duration_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level2_Duration_Days').innerText = '{8}';
                        for(i = 0; i < window.form1.TextBox_Level2_Duration_Hours.length; i++)        
                            if(window.form1.TextBox_Level2_Duration_Hours.options[i].value == '{9}')
                                window.form1.TextBox_Level2_Duration_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level2_Duration_Mins.length; i++)        
                            if(window.form1.TextBox_Level2_Duration_Mins.options[i].value == '{10}')
                                window.form1.TextBox_Level2_Duration_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level3_Duration_Days').innerText = '{11}';
                        for(i = 0; i < window.form1.TextBox_Level3_Duration_Hours.length; i++)        
                            if(window.form1.TextBox_Level3_Duration_Hours.options[i].value == '{12}')
                                window.form1.TextBox_Level3_Duration_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level3_Duration_Mins.length; i++)        
                            if(window.form1.TextBox_Level3_Duration_Mins.options[i].value == '{13}')
                                window.form1.TextBox_Level3_Duration_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level4_Duration_Days').innerText = '{14}';
                        for(i = 0; i < window.form1.TextBox_Level4_Duration_Hours.length; i++)        
                            if(window.form1.TextBox_Level4_Duration_Hours.options[i].value == '{15}')
                                window.form1.TextBox_Level4_Duration_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level4_Duration_Mins.length; i++)        
                            if(window.form1.TextBox_Level4_Duration_Mins.options[i].value == '{16}')
                                window.form1.TextBox_Level4_Duration_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level1_Notifications').innerText = '{17}';
						document.getElementById('TextBox_Level2_Notifications').innerText = '{18}';
						document.getElementById('TextBox_Level3_Notifications').innerText = '{19}';
						document.getElementById('TextBox_Level4_Notifications').innerText = '{20}';
						document.getElementById('TextBox_Level5_Notifications').innerText = '{21}';

						document.getElementById('TextBox_Level1_ReminderInterval_Days').innerText = '{22}';
                        for(i = 0; i < window.form1.TextBox_Level1_ReminderInterval_Hours.length; i++)        
                            if(window.form1.TextBox_Level1_ReminderInterval_Hours.options[i].value == '{23}')
                                window.form1.TextBox_Level1_ReminderInterval_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level1_ReminderInterval_Mins.length; i++)        
                            if(window.form1.TextBox_Level1_ReminderInterval_Mins.options[i].value == '{24}')
                                window.form1.TextBox_Level1_ReminderInterval_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level2_ReminderInterval_Days').innerText = '{25}';
                        for(i = 0; i < window.form1.TextBox_Level2_ReminderInterval_Hours.length; i++)        
                            if(window.form1.TextBox_Level2_ReminderInterval_Hours.options[i].value == '{26}')
                                window.form1.TextBox_Level2_ReminderInterval_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level2_ReminderInterval_Mins.length; i++)        
                            if(window.form1.TextBox_Level2_ReminderInterval_Mins.options[i].value == '{27}')
                                window.form1.TextBox_Level2_ReminderInterval_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level3_ReminderInterval_Days').innerText = '{28}';
                        for(i = 0; i < window.form1.TextBox_Level3_ReminderInterval_Hours.length; i++)        
                            if(window.form1.TextBox_Level3_ReminderInterval_Hours.options[i].value == '{29}')
                                window.form1.TextBox_Level3_ReminderInterval_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level3_ReminderInterval_Mins.length; i++)        
                            if(window.form1.TextBox_Level3_ReminderInterval_Mins.options[i].value == '{30}')
                                window.form1.TextBox_Level3_ReminderInterval_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level4_ReminderInterval_Days').innerText = '{31}';
                        for(i = 0; i < window.form1.TextBox_Level4_ReminderInterval_Hours.length; i++)        
                            if(window.form1.TextBox_Level4_ReminderInterval_Hours.options[i].value == '{32}')
                                window.form1.TextBox_Level4_ReminderInterval_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level4_ReminderInterval_Mins.length; i++)        
                            if(window.form1.TextBox_Level4_ReminderInterval_Mins.options[i].value == '{33}')
                                window.form1.TextBox_Level4_ReminderInterval_Mins.options[i].selected = true;

						document.getElementById('TextBox_Level5_ReminderInterval_Days').innerText = '{34}';
                        for(i = 0; i < window.form1.TextBox_Level5_ReminderInterval_Hours.length; i++)        
                            if(window.form1.TextBox_Level5_ReminderInterval_Hours.options[i].value == '{35}')
                                window.form1.TextBox_Level5_ReminderInterval_Hours.options[i].selected = true;
						for(i = 0; i < window.form1.TextBox_Level5_ReminderInterval_Mins.length; i++)        
                            if(window.form1.TextBox_Level5_ReminderInterval_Mins.options[i].value == '{36}')
                                window.form1.TextBox_Level5_ReminderInterval_Mins.options[i].selected = true;

						window.form1.CheckBox_Level1_Email.checked = {37};
						window.form1.CheckBox_Level2_Email.checked = {38};
						window.form1.CheckBox_Level3_Email.checked = {39};
						window.form1.CheckBox_Level4_Email.checked = {40};
						window.form1.CheckBox_Level5_Email.checked = {41};

						window.form1.CheckBox_Level1_Sms.checked = {42};
						window.form1.CheckBox_Level2_Sms.checked = {43};
						window.form1.CheckBox_Level3_Sms.checked = {44};
						window.form1.CheckBox_Level4_Sms.checked = {45};
						window.form1.CheckBox_Level5_Sms.checked = {46};
						</script>", e.Parameters[0], severityLevelName, level0_duration.Days.ToString(), level0_duration.Hours.ToString(), level0_duration.Minutes.ToString(), level1_duration.Days.ToString(), level1_duration.Hours.ToString(), level1_duration.Minutes.ToString(), level2_duration.Days.ToString(), level2_duration.Hours.ToString(), level2_duration.Minutes.ToString(), level3_duration.Days.ToString(), level3_duration.Hours.ToString(), level3_duration.Minutes.ToString(), level4_duration.Days.ToString(), level4_duration.Hours.ToString(), level4_duration.Minutes.ToString(), row["level1_notification_count"].ToString(), row["level2_notification_count"].ToString(), row["level3_notification_count"].ToString(), row["level4_notification_count"].ToString(), row["level5_notification_count"].ToString(), level1_reminder_interval.Days.ToString(), level1_reminder_interval.Hours.ToString(), level1_reminder_interval.Minutes.ToString(), level2_reminder_interval.Days.ToString(), level2_reminder_interval.Hours.ToString(), level2_reminder_interval.Minutes.ToString(), level3_reminder_interval.Days.ToString(), level3_reminder_interval.Hours.ToString(), level3_reminder_interval.Minutes.ToString(), level4_reminder_interval.Days.ToString(), level4_reminder_interval.Hours.ToString(), level4_reminder_interval.Minutes.ToString(), level5_reminder_interval.Days.ToString(), level5_reminder_interval.Hours.ToString(), level5_reminder_interval.Minutes.ToString(), row["level1_email_enabled"].ToString().ToLower(), row["level2_email_enabled"].ToString().ToLower(), row["level3_email_enabled"].ToString().ToLower(), row["level4_email_enabled"].ToString().ToLower(), row["level5_email_enabled"].ToString().ToLower(), row["level1_sms_enabled"].ToString().ToLower(), row["level2_sms_enabled"].ToString().ToLower(), row["level3_sms_enabled"].ToString().ToLower(), row["level4_sms_enabled"].ToString().ToLower(), row["level5_sms_enabled"].ToString().ToLower());

                    e.Output.Write(script);
                }
            }
            catch (Exception ex)
            {
                //\\UILogger.Log(SeverityLevel.Error, "An error occurred while loading alert escalation.", ex);
                e.Output.Write("<script>ShowErrorMessage('An error occurred while loading alert escalation. " + ex.Message.Replace("'", "") + "')</script>");
            }
            finally
            {
                //\\UILogger.StepOutContext();
            }
        }

        /// <summary>
        /// Edit alert escalation.
        /// </summary>
        protected void EditAlertEscalation(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            DataRow row = null;

            try
            {
                //\\UILogger.StepInContext("EditAlertEscalation");
                
                //if (!BasePage.isSqlInjectionFree(e.Parameters[1]) || !BasePage.isSqlInjectionFree(e.Parameters[4]) || !BasePage.isSqlInjectionFree(e.Parameters[7]) || !BasePage.isSqlInjectionFree(e.Parameters[10]) || !BasePage.isSqlInjectionFree(e.Parameters[13]) || !BasePage.isSqlInjectionFree(e.Parameters[16]) || !BasePage.isSqlInjectionFree(e.Parameters[17]) || !BasePage.isSqlInjectionFree(e.Parameters[18]) || !BasePage.isSqlInjectionFree(e.Parameters[19]) || !BasePage.isSqlInjectionFree(e.Parameters[20]) || !BasePage.isSqlInjectionFree(e.Parameters[21]) || !BasePage.isSqlInjectionFree(e.Parameters[24]) || !BasePage.isSqlInjectionFree(e.Parameters[27]) || !BasePage.isSqlInjectionFree(e.Parameters[30]) || !BasePage.isSqlInjectionFree(e.Parameters[33]))
                //{
                //    e.Output.Write("<script>ShowErrorMessage('Invalid characters entered.')</script>");
                //    return;
                //}

                row = alertEscalationDT.Select("alert_template_id = " + int.Parse(alertTemplateId) + " and severity_id = " + int.Parse(e.Parameters[0]))[0];

                alertEscalationDT.Rows.Remove(row);

                row["alert_template_id"] = int.Parse(alertTemplateId);
                row["severity_id"] = int.Parse(e.Parameters[0]);

                row["level0_duration"] = ((Convert.ToInt32(e.Parameters[1]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[2]) * 60 * 60) + (Convert.ToInt32(e.Parameters[3]) * 60));
                row["level1_duration"] = ((Convert.ToInt32(e.Parameters[4]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[5]) * 60 * 60) + (Convert.ToInt32(e.Parameters[6]) * 60));
                row["level2_duration"] = ((Convert.ToInt32(e.Parameters[7]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[8]) * 60 * 60) + (Convert.ToInt32(e.Parameters[9]) * 60));
                row["level3_duration"] = ((Convert.ToInt32(e.Parameters[10]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[11]) * 60 * 60) + (Convert.ToInt32(e.Parameters[12]) * 60));
                row["level4_duration"] = ((Convert.ToInt32(e.Parameters[13]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[14]) * 60 * 60) + (Convert.ToInt32(e.Parameters[15]) * 60));

                row["level1_notification_count"] = Convert.ToByte(e.Parameters[16]);
                row["level2_notification_count"] = Convert.ToByte(e.Parameters[17]);
                row["level3_notification_count"] = Convert.ToByte(e.Parameters[18]);
                row["level4_notification_count"] = Convert.ToByte(e.Parameters[19]);
                row["level5_notification_count"] = Convert.ToByte(e.Parameters[20]);

                row["level1_reminder_interval"] = ((Convert.ToInt32(e.Parameters[21]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[22]) * 60 * 60) + (Convert.ToInt32(e.Parameters[23]) * 60));
                row["level2_reminder_interval"] = ((Convert.ToInt32(e.Parameters[24]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[25]) * 60 * 60) + (Convert.ToInt32(e.Parameters[26]) * 60));
                row["level3_reminder_interval"] = ((Convert.ToInt32(e.Parameters[27]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[28]) * 60 * 60) + (Convert.ToInt32(e.Parameters[29]) * 60));
                row["level4_reminder_interval"] = ((Convert.ToInt32(e.Parameters[30]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[31]) * 60 * 60) + (Convert.ToInt32(e.Parameters[32]) * 60));
                row["level5_reminder_interval"] = ((Convert.ToInt32(e.Parameters[33]) * 60 * 60 * 24) + (Convert.ToInt32(e.Parameters[34]) * 60 * 60) + (Convert.ToInt32(e.Parameters[35]) * 60));

                row["level1_email_enabled"] = Convert.ToBoolean(e.Parameters[36]);
                row["level2_email_enabled"] = Convert.ToBoolean(e.Parameters[37]);
                row["level3_email_enabled"] = Convert.ToBoolean(e.Parameters[38]);
                row["level4_email_enabled"] = Convert.ToBoolean(e.Parameters[39]);
                row["level5_email_enabled"] = Convert.ToBoolean(e.Parameters[40]);

                row["level1_sms_enabled"] = Convert.ToBoolean(e.Parameters[41]);
                row["level2_sms_enabled"] = Convert.ToBoolean(e.Parameters[42]);
                row["level3_sms_enabled"] = Convert.ToBoolean(e.Parameters[43]);
                row["level4_sms_enabled"] = Convert.ToBoolean(e.Parameters[44]);
                row["level5_sms_enabled"] = Convert.ToBoolean(e.Parameters[45]);

                alertEscalationDT.Rows.Add(row);

                //\\UILogger.Log(SeverityLevel.Information, "Alert Escalation with Id " + alertTemplateId + " and Severity " + e.Parameters[0] + " saved temporarily.");

                e.Output.Write("<script>Dialog_EditAlertEscalation.Close();document.body.scroll='no';</script>");
            }
            catch (Exception ex)
            {
                //\\UILogger.Log(SeverityLevel.Error, "An error occurred while saving alert escalation.", ex);
                e.Output.Write("<script>ShowErrorMessage('An error occurred while saving alert escalation. " + ex.Message.Replace("'", "") + "');</script>");
            }
            finally
            {
                //\\UILogger.StepOutContext();
            }
        }

        #endregion
    }
}