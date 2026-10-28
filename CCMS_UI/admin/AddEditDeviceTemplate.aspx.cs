using Avanza.iSuite.UI;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Avanza.iSuite.DAL;
using Avanza.CCMS.DAL;
using System;
using System.Collections;
using System.Text;
namespace CCMSUI.admin
{
    public partial class AddEditDeviceTemplate : BasePage
    {
        // Fields

        // Methods
        DataSet deviceDS = null;
        protected void BuildSubTree(DataRow parentDeviceRow, TreeNode node)
        {
            foreach (DataRow row in this.deviceDS.Tables[1].Select("module_number = " + parentDeviceRow["device_number"]))
            {
                TreeNode child = this.CreateParentDeviceNode(row["device_name"].ToString(), true, (int)row["device_id"]);
                node.ChildNodes.Add(child);
                this.BuildSubTree(row, child);
            }
            foreach (DataRow row2 in parentDeviceRow.GetChildRows("ParentChildRelation"))
            {
                TreeNode node3 = this.CreateChildDeviceNode(row2["device_name"].ToString(), (int)row2["device_id"]);
                node.ChildNodes.Add(node3);
            }
        }

        protected void BuildTree(string vendorType)
        {
            SqlCommand selectCommand = null;
            SqlDataAdapter adapter = null;
            try
            {
                //BasePage.\\UILogger.StepInContext("BuildTree");
                this.TreeView_Device.Nodes.Clear();
                selectCommand = ConnectionFactory.GetNewCommand(false);
                selectCommand.CommandText = "select device_id, device_name, device_number, module_number from device where vendor_name = @VendorType and module_number is not null and is_active = 1 order by device_name; select device_id, device_name, device_number, module_number from device where vendor_name = @VendorType and module_number is null and is_active = 1 order by device_name";
                selectCommand.Parameters.AddWithValue("@VendorType", vendorType);
                adapter = new SqlDataAdapter(selectCommand);
                this.deviceDS = new DataSet();
                adapter.Fill(this.deviceDS);
                this.deviceDS.Relations.Add("ParentChildRelation", this.deviceDS.Tables[1].Columns["device_number"], this.deviceDS.Tables[0].Columns["device_number"]);
                foreach (DataRow row in this.deviceDS.Tables[1].Rows)
                {
                    if (row.IsNull("module_number"))
                    {
                        TreeNode child = this.CreateParentDeviceNode(row["device_name"].ToString(), true, (int)row["device_id"]);
                        this.TreeView_Device.Nodes.Add(child);
                        this.BuildSubTree(row, child);
                    }
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while building tree.", exception);
                this.Literal_error.Text = "<li>An error occurred while building tree: [" + exception.Message + "]</li>";
            }
            finally
            {
                if (selectCommand != null)
                {
                    selectCommand.Connection.Close();
                    selectCommand.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();
                }
                //BasePage.\\UILogger.StepOutContext();
            }
        }

        protected void Button_save_Click(object sender, EventArgs e)
        {
            DeviceTemplate deviceTemplate = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            bool flag = false;
            try
            {
                if (base.Request.QueryString["device_template_id"] == null)
                {
                    flag = false;
                    if ((int)ConnectionFactory.ExecuteScalar("select count(*) from device_Template where device_template_name='" + TextBox_Type.Text + "'") > 0)
                    {
                        this.Literal_error.Text = "<li>Device Template name already exists.</li>";
                        return;
                    }
                    deviceTemplate = new DeviceTemplate();
                }
                else
                {
                    flag = true;
                    int deviceTemplateId = int.Parse(Request.QueryString["device_template_id"]);
                    if ((int)ConnectionFactory.ExecuteScalar("select count(*) from device_Template where device_template_name='" + TextBox_Type.Text + "' and device_template_id <>" + deviceTemplateId) > 0)
                    {
                        this.Literal_error.Text = "<li>Device Template name already exists.</li>";
                        return;
                    }
                    deviceTemplate = DeviceTemplate.LoadDeviceTemplateByPk(deviceTemplateId);
                }
                deviceTemplate.DeviceTemplateName = this.TextBox_Type.Text;
                deviceTemplate.DeviceTemplateDesc = this.TextBox_Description.Text;
                deviceTemplate.IsActive = new bool?(this.DropDownList_status.SelectedValue == "1");
                deviceTemplate.Save();

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = "select atm_id from atm_devices where device_template_id = " + deviceTemplate.DeviceTemplateId;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                string command = string.Format("delete from atm_devices where device_template_id ={0}; delete from device_template_devices where device_template_id={0};  ", deviceTemplate.DeviceTemplateId);
                StringBuilder builder = new StringBuilder();
                foreach (TreeNode node in this.TreeView_Device.CheckedNodes)
                {
                    builder.AppendFormat("insert device_template_devices(device_template_id,device_id,is_present) values({0},{1},1);", deviceTemplate.DeviceTemplateId, int.Parse(node.Value.Substring(1)));
                }
                StringBuilder builder2 = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    builder2.AppendFormat(@"insert atm_devices(atm_id,device_template_id,device_id,is_active,is_critical,is_present)                                                 
                    select {0},device_template_id,device_id,1,1,is_present from device_template_devices where device_template_id={1};",
                    dr[0], deviceTemplate.DeviceTemplateId);
                }
                cmd.Connection.Open();
                trxn = cmd.Connection.BeginTransaction();
                cmd.Transaction = trxn;

                ConnectionFactory.ExecuteQuery(command, trxn);
                ConnectionFactory.ExecuteQuery(builder.ToString(), trxn);

                if (dt.Rows.Count > 0)
                    ConnectionFactory.ExecuteQuery(builder2.ToString(), trxn);
                trxn.Commit();

                if (flag)
                {
                    Utility.BuildAuditLog("Device Template " + deviceTemplate.DeviceTemplateName + " edited", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, 0xb1);
                    //Session[SessionVars.notice.ToString()] = "Device Template Updated Successfully";
                    Session["msg"] = "Device Template Updated Successfully";
                }
                else
                {
                    Utility.BuildAuditLog("Device Template " + deviceTemplate.DeviceTemplateName + " created", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, 0xb0);
                    //Session[SessionVars.notice.ToString()] = "Device Template Added Successfully";
                    Session["msg"] = "Device Template Added Successfully";
                }
                //BasePage.\\UILogger.Log(SeverityLevel.Information, "Device Template Saved.");
                base.Response.Redirect("~/ccms/DeviceTemplates.aspx", false);

            }
            catch (Exception exception)
            {
                trxn.Rollback();
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while saving device template.", exception);
                this.Literal_error.Text = "<li>An error occurred while saving device template: [" + exception.Message + "]</li>";
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();

                //BasePage.\\UILogger.CloseContext();
            }
        }

        protected TreeNode CreateChildDeviceNode(string text, int id)
        {
            TreeNode node = new TreeNode();
            node.Text = text;
            node.Value = "a" + id;
            node.ShowCheckBox = true;
            node.SelectAction = TreeNodeSelectAction.None;
            return node;
        }

        protected TreeNode CreateParentDeviceNode(string text, bool expanded, int id)
        {
            TreeNode node = new TreeNode();
            node.Text = text;
            node.Value = "r" + id;
            node.Expanded = new bool?(expanded);
            node.ShowCheckBox = true;
            node.SelectAction = TreeNodeSelectAction.None;
            return node;
        }

        protected void DropDownList_VendorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Convert.ToInt32(this.DropDownList_VendorType.SelectedValue))
            {
                case 0:
                    this.BuildTree("NCR");
                    return;

                case 1:
                    this.BuildTree("Wincor");
                    return;

                case 2:
                    this.BuildTree("Diebold");
                    return;
            }
            this.BuildTree("Dummy");
        }

        private string getVendorType(int deviceTemplateID)
        {
            string str;
            try
            {
                SqlCommand newCommand = null;
                newCommand = ConnectionFactory.GetNewCommand(true);
                newCommand.CommandText = "select vendor_name from device where device_id=(select top 1 device_id from device_template_devices where device_template_id = @deviceTemplateID)";
                newCommand.Parameters.AddWithValue("@deviceTemplateID", deviceTemplateID);
                str = newCommand.ExecuteScalar().ToString();
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "Error occured while getting vendor_name for template", exception);
                throw new Exception("This template has no device associated with it");
            }
            return str;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("AddEditDeviceTemplate");
                //BasePage.\\UILogger.StepInContext("Page_Load");
                if (!base.IsPostBack)
                {
                    if (base.Request.QueryString["device_template_id"] == null)
                    {
                        //if (!base.CheckRight(0xb0))
                        //{
                        //base.Response.Redirect("AccessDenied.aspx");
                        //}
                        this.SetDefaultValues();
                    }
                    else
                    {
                        //if (!base.CheckRight(0xb1))
                        //{
                        //base.Response.Redirect("AccessDenied.aspx");
                        //}
                        this.SetCurrentValues();
                    }
                    this.Page.SetFocus(this.Button_save);
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while loading page.", exception);
                this.Literal_error.Text = "<li>An error occurred while loading page: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
            }
        }

        protected void SetCheck(TreeNode node, ArrayList selectedItem)
        {
            if (selectedItem.IndexOf(int.Parse(node.Value.Substring(1))) >= 0)
            {
                node.Checked = true;
            }
            TreeNodeCollection childNodes = node.ChildNodes;
            int num = 0;
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i].Value.IndexOf('r') < 0)
                {
                    if (selectedItem.IndexOf(int.Parse(childNodes[i].Value.Substring(1))) >= 0)
                    {
                        childNodes[i].Checked = true;
                        num++;
                    }
                }
                else if (childNodes[i].ChildNodes.Count > 0)
                {
                    this.SetCheck(childNodes[i], selectedItem);
                    if (childNodes[i].Checked)
                    {
                        num++;
                    }
                }
                else
                {
                    childNodes[i].Checked = true;
                    num++;
                }
            }
        }

        protected void SetCurrentValues()
        {
            DeviceTemplate template = null;
            DeviceTemplateDevices.DeviceTemplateDevicesReader reader = null;
            ArrayList selectedItem = null;
            try
            {
                //BasePage.\\UILogger.StepInContext("SetCurrentValues");
                int num = int.Parse(base.Request.QueryString["device_template_id"]);
                template = DeviceTemplate.LoadDeviceTemplateByPk(num);
                reader = DeviceTemplateDevices.ExecuteReader("device_template_id = " + num);
                selectedItem = new ArrayList();
                this.TextBox_Type.Text = template.DeviceTemplateName;
                this.TextBox_Description.Text = template.DeviceTemplateDesc;
                if (template.IsActive.HasValue)
                {
                    this.DropDownList_status.SelectedValue = template.IsActive.Value ? "1" : "0";
                }
                string str = this.getVendorType(template.DeviceTemplateId);
                if (str != null)
                {
                    if (!(str == "NCR"))
                    {
                        if (str == "Wincor")
                        {
                            goto Label_010D;
                        }
                        if (str == "Diebold")
                        {
                            goto Label_012A;
                        }
                    }
                    else
                    {
                        this.DropDownList_VendorType.SelectedValue = "0";
                        this.BuildTree("NCR");
                    }
                }
                goto Label_015E;
            Label_010D:
                this.DropDownList_VendorType.SelectedValue = "1";
                this.BuildTree("Wincor");
                goto Label_015E;
            Label_012A:
                this.DropDownList_VendorType.SelectedValue = "2";
                this.BuildTree("Diebold");
            Label_015E:
                while (reader.Read())
                {
                    selectedItem.Add(reader.CurrentDeviceTemplateDevices.DeviceId);
                }
                if (selectedItem.Count > 0)
                {
                    for (int i = 0; i < this.TreeView_Device.Nodes.Count; i++)
                    {
                        this.SetCheck(this.TreeView_Device.Nodes[i], selectedItem);
                    }
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while setting current values.", exception);
                this.Literal_error.Text = "<li>An error occurred while setting current values: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.StepOutContext();
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        protected void SetDefaultValues()
        {
            try
            {
                //BasePage.\\UILogger.StepInContext("SetDefaultValues");
                this.TextBox_Type.Text = "";
                this.TextBox_Description.Text = "";
                this.DropDownList_status.ClearSelection();
                this.DropDownList_status.SelectedIndex = 0;
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while setting default values.", exception);
                this.Literal_error.Text = "<li>An error occurred while setting default values: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.StepOutContext();
            }
        }
    }




}