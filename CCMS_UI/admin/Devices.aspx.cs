using Avanza.iSuite.UI;
using System.Data.SqlClient;
using Avanza.iSuite.DAL;
using System.Text;
using System.Data;
using System;
using ComponentArt.Web.UI;
using System.Web.UI.WebControls;
using Avanza.CCMS.DAL;


namespace CCMSUI.admin
{
    public partial class Devices : BasePage
    {


        //// Fields


        // Methods
        private string GetSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();
            if (TextBox_name.Text.Length > 0)
                builder.Append(" and tbl.device_name like '%" + TextBox_name.Text + "%'");

            if (TextBox_DeviceNo.Text.Length > 0)
                builder.Append("and tbl.device_number like '%" + TextBox_DeviceNo.Text + "%'");

            if (DropDownList_vendor.SelectedIndex > 0)
                builder.Append("and tbl.vendor_name ='" + DropDownList_vendor.SelectedItem.Value + "'");

            //if (DropDownList_Repl.SelectedIndex > 0)
            //    builder.Append("and tbl.is_replenishable =" + DropDownList_Repl.SelectedItem.Value);

            return builder.ToString();
        }
        protected void BindGrid()
        {
            SqlDataAdapter adapter = null;
            SqlCommand selectCommand = null;
            DataSet set = null;
            DataTable dataTable = null;
            DataTable table2 = null;
            StringBuilder builder = null;
            try
            {
                ////BasePage.\\UILogger.StepInContext("BindGrid");
                selectCommand = ConnectionFactory.GetNewCommand(false);
                builder = new StringBuilder("select device_service_1,device_service_2,device_service_3,device_id, module_number,is_replenishable,device_name, device_number, vendor_name FROM (select distinct(p.device_id),p.device_service_1,p.device_service_2,p.device_service_3, p.module_number,p.is_replenishable,p.device_name, p.device_number, p.vendor_name  from (select device_service_1,device_service_2,device_service_3,device_id, module_number,is_replenishable,device_name, device_number, vendor_name  from device where module_number is null and is_active = 1 and is_abstract = 0) p left join (select device_service_1,device_service_2,device_service_3,device_id, module_number,is_replenishable,device_name, device_number, vendor_name   from device where module_number is not null and is_active = 1 and is_abstract = 0) c on p.device_number = c.device_number) as tbl ");
                builder.Append(" WHERE (1=1) ");
                builder.Append(GetSearchCriteria());
                //(tbl.device_name like '%" + this.TextBox_name.Text + "%') ");
                //builder.Append(" AND (tbl.vendor_name like '" + this.DropDownList_vendor.SelectedValue + "' OR '" + this.DropDownList_vendor.SelectedValue + "'='All') ");
                builder.Append("ORDER BY tbl." + (this.ViewState["SortExpression"].ToString() + " " + this.ViewState["SortDirection"].ToString()));
                selectCommand.CommandText = builder.ToString();
                adapter = new SqlDataAdapter(selectCommand);
                dataTable = new DataTable("ParentDevices");
                adapter.Fill(dataTable);
                selectCommand.Parameters.Clear();
                builder = new StringBuilder("select device_service_1,device_service_2,device_service_3,device_id, module_number,is_replenishable,device_name, device_number, vendor_name FROM (select distinct(c.device_id),c.device_service_1,c.device_service_2,c.device_service_3, c.module_number,c.is_replenishable,c.device_name, c.device_number, c.vendor_name   from (select device_service_1,device_service_2,device_service_3,device_id, module_number,is_replenishable,device_name, device_number, vendor_name  from device where module_number is null and is_active = 1 and is_abstract = 0) p right join (select device_service_1,device_service_2,device_service_3,device_id, module_number,is_replenishable,device_name, device_number, vendor_name  from device where module_number is not null and is_active = 1 and is_abstract = 0) c on p.device_number = c.device_number) as tbl ");
                builder.Append(" WHERE (1=1) ");
                builder.Append(GetSearchCriteria());
                //builder.Append(" AND (tbl.device_name like '%" + this.TextBox_name.Text + "%') ");
                //builder.Append(" AND (tbl.vendor_name like '" + this.DropDownList_vendor.SelectedValue + "' OR '" + this.DropDownList_vendor.SelectedValue + "'='All') ");
                builder.Append("ORDER BY tbl." + (this.ViewState["SortExpression"].ToString() + " " + this.ViewState["SortDirection"].ToString()));
                selectCommand.CommandText = builder.ToString();
                adapter = new SqlDataAdapter(selectCommand);
                table2 = new DataTable("ChildDevices");
                adapter.Fill(table2);
                set = new DataSet();
                set.Tables.Add(dataTable);
                set.Tables.Add(table2);
                set.Relations.Add("ParentAndChildDevices", set.Tables["ParentDevices"].Columns["device_number"], set.Tables["ChildDevices"].Columns["device_number"], false);
                this.Grid_Devices.DataSource = set;
                this.Grid_Devices.DataMember = "ParentDevices";
                this.Grid_Devices.DataBind();
                this.Grid_Devices.RowExpanded.CollapseAll();
                this.Literal_count.Text = dataTable.Rows.Count + " records.";
            }
            catch (Exception exception)
            {
                ////BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding grid.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding grid: [" + exception.Message + "]</li>";
            }
            finally
            {
                ////BasePage.\\UILogger.StepOutContext();
                if (adapter != null)
                {
                    adapter.Dispose();
                }
                if (selectCommand != null)
                {
                    selectCommand.Connection.Close();
                    selectCommand.Dispose();
                }
            }
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            this.SetDefaultValues();
            this.ViewState["SortDirection"] = "asc";
            this.ViewState["SortExpression"] = "device_name";
            this.BindGrid();
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            this.Grid_Devices.CurrentPageIndex = 0;
            //if (!base.CheckRight(0xb2))
            //{
            //this.Grid_Devices.Columns[3].Visible = false;
            //            }
            this.BindGrid();
        }

        protected void EditDevice(object sender, CallBackEventArgs e)
        {
            SqlCommand newCommand = null;
            SqlTransaction trx = null;
            try
            {
                //BasePage.\\UILogger.StepInContext("EditDevice");

                newCommand = ConnectionFactory.GetNewCommand(true);
                trx = newCommand.Connection.BeginTransaction();
                newCommand.Transaction = trx;
                newCommand.CommandText = "update device set vendor_name = '" + e.Parameters[2] + "' where device_number = " + e.Parameters[3];
                newCommand.ExecuteNonQuery();
                Device device = Device.LoadDevice(" device_id = " + e.Parameters[0]);
                device.DeviceName = e.Parameters[1];
                device.VendorName = e.Parameters[2];
                device.Save(newCommand.Connection, trx);
                trx.Commit();
                Utility.BuildAuditLog("Device " + device.DeviceName + " edited", ((AppUser)this.Session[SessionVars.user.ToString()]).UserId, 0xb2);
                //BasePage.\\UILogger.Log(SeverityLevel.Information, "Device " + device.DeviceName + " saved.");
                e.Output.Write("<script>Dialog_EditDevice.Close();window.location = 'Devices.aspx';parent.window.ShowSuccess('Device updated successfully');</script>");

            }
            catch (Exception exception)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while editing device.", exception);
                e.Output.Write("<script>ShowErrorMessage('An error occurred while editing device. " + exception.Message.Replace("'", "") + "')</script>");
            }
            finally
            {
                //BasePage.\\UILogger.StepOutContext();
                if (newCommand != null)
                {
                    newCommand.Connection.Close();
                    newCommand.Dispose();
                }
            }
        }

        protected void Grid_Devices_TemplateSelection(object sender, DBauer.Web.UI.WebControls.HierarGridTemplateSelectionEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("Devices");
                //BasePage.\\UILogger.StepInContext("Grid_Devices_TemplateSelection");
                e.TemplateFilename = "ChildDevices.ascx";
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while selecting template.", exception);
                this.Literal_error.Text = "<li>An error occurred while selecting template: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
            }
        }

        protected void GridView_PageIndexChanging(object source, DataGridPageChangedEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("Devices");
                //BasePage.\\UILogger.StepInContext("GridView_PageIndexChanging");
                this.Grid_Devices.CurrentPageIndex = e.NewPageIndex;
                //if (!base.CheckRight(0xb2))
                //{
                //  this.Grid_Devices.Columns[4].Visible = false;
                //                }
                this.BindGrid();
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while changing page index.", exception);
                this.Literal_error.Text = "<li>An error occurred while changing page index: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
            }
        }

        protected void GridView_Sorting(object source, DataGridSortCommandEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("Devices");
                //BasePage.\\UILogger.StepInContext("GridView_Sorting");
                if (this.ViewState["SortExpression"].ToString() == e.SortExpression)
                {
                    if (this.ViewState["SortDirection"].ToString() == "asc")
                    {
                        this.ViewState["SortDirection"] = "desc";
                    }
                    else
                    {
                        this.ViewState["SortDirection"] = "asc";
                    }
                }
                else
                {
                    this.ViewState["SortExpression"] = e.SortExpression;
                    this.ViewState["SortDirection"] = "asc";
                }
                //if (!base.CheckRight(0xb2))
                //{
                //this.Grid_Devices.Columns[3].Visible = false;
                //                }
                this.BindGrid();
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while sorting grid.", exception);
                this.Literal_error.Text = "<li>An error occurred while sorting grid: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
            }
        }

        protected void LoadEditDeviceDialog(object sender, CallBackEventArgs e)
        {
            SqlCommand newCommand = null;
            SqlDataReader reader = null;
            TerminalType.TerminalTypeReader reader2 = null;
            string str = "";
            int num = 0;
            try
            {
                //BasePage.\\UILogger.StepInContext("LoadEditDeviceDialog");
                reader2 = TerminalType.ExecuteReader("1=1");
                while (reader2.Read())
                {
                    str = string.Format("<script>document.form1.DropDownList_Dialog_Vendor.options[{0}] = new Option('{1}');</script>", num++, reader2.CurrentTerminalType.AtmType);
                    e.Output.Write(str);
                    e.Output.Flush();
                }
                newCommand = ConnectionFactory.GetNewCommand(true);
                newCommand.CommandText = "select device_number, module_number, device_name, vendor_name, device_internal_name, is_replenishable from device where device_id = " + e.Parameter;
                reader = newCommand.ExecuteReader();
                if (reader.Read())
                {
                    str = string.Format("<script>\r\n                        document.getElementById('Label_Dialog_DeviceId').innerHTML = '{0}';\r\n                        document.getElementById('Label_Dialog_DeviceNumber').innerHTML = '{1}';\r\n                        document.getElementById('Label_Dialog_ModuleNumber').innerHTML = '{2}';                        \r\n                        document.getElementById('TextBox_Dialog_Name').value = '{3}';\r\n                        for(i = 0; i < document.form1.DropDownList_Dialog_Vendor.options.length; i++)        \r\n                            if(document.form1.DropDownList_Dialog_Vendor.options[i].text == '{4}')\r\n                                document.form1.DropDownList_Dialog_Vendor.options[i].selected = 'selected';\r\n                        document.getElementById('Label_Dialog_InternalName').innerHTML = '{5}';\r\n                        document.getElementById('Label_Dialog_Replenishable').innerHTML = '{6}';\r\n                        </script>", new object[] { e.Parameter, reader.GetValue(0), (reader.GetValue(1) == DBNull.Value) ? "" : reader.GetValue(1), reader.GetValue(2), (reader.GetValue(3) == DBNull.Value) ? "" : reader.GetValue(3), (reader.GetValue(4) == DBNull.Value) ? "" : reader.GetValue(4), (reader.GetValue(5) == DBNull.Value) ? "" : reader.GetValue(5).ToString() });
                    if (reader.GetValue(1) == DBNull.Value)
                    {
                        e.Output.Write("<script>document.getElementById('DropDownList_Dialog_Vendor').removeAttribute('disabled');</script>");
                    }
                    else
                    {
                        e.Output.Write("<script>document.getElementById('DropDownList_Dialog_Vendor').setAttribute('disabled', 'disabled');</script>");
                    }
                    e.Output.Flush();
                }
                e.Output.Write(str);
                e.Output.Flush();
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while loading device.", exception);
                e.Output.Write("<script>ShowErrorMessage('An error occurred while loading device. " + exception.Message.Replace("'", "") + "')</script>");
            }
            finally
            {
                //BasePage.\\UILogger.StepOutContext();
                if (reader2 != null)
                {
                    reader2.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                if (newCommand != null)
                {
                    newCommand.Connection.Close();
                    newCommand.Dispose();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TerminalType.TerminalTypeReader reader = null;
            try
            {
                //BasePage.\\UILogger.OpenContext("Devices");
                //BasePage.\\UILogger.StepInContext("Page_Load");
                if ((!this.Callback_LoadEditDeviceDialog.CausedCallback && !this.Callback_EditDevice.CausedCallback) && !this.Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.EditDevices))
                    {
                        this.Grid_Devices.Columns[8].Visible = false;
                    }
                    this.DropDownList_vendor.Items.Clear();
                    reader = TerminalType.ExecuteReader("1=1");
                    this.DropDownList_vendor.Items.Add(new ListItem("All", "All"));
                    while (reader.Read())
                    {
                        this.DropDownList_vendor.Items.Add(new ListItem(reader.CurrentTerminalType.AtmType, reader.CurrentTerminalType.AtmType));
                    }
                    this.SetDefaultValues();
                    this.ViewState["SortDirection"] = "asc";
                    this.ViewState["SortExpression"] = "device_name";
                    this.BindGrid();
                    this.Page.SetFocus(this.Button_search);
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
                this.TextBox_name.Text = "";
                this.DropDownList_vendor.ClearSelection();
                this.DropDownList_vendor.SelectedIndex = 0;
                this.Grid_Devices.DataSource = null;
                this.Grid_Devices.DataBind();
                this.Literal_count.Text = "0 records.";
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