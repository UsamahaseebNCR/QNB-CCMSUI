using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avanza.iSuite.UI;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;

using System.Drawing;
using DBauer.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class ATMDetail : BasePage
    {
        // Fields
        private static string atmId;
        private static string page;
        private static long runningTotalDispensed;
        private static long runningTotalJammed;
        private static long runningTotalRejected;
        private static long runningTotalRemaining;



        // Methods
        protected void BindGrid()
        {
            //SqlCommand newCommand = null;
            //SqlDataReader reader = null;
            DataTable table = null;
            DataSet set = null;
            DataTable table2 = null;
            string str = "";
            string query = "";
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //BasePage.\\UILogger.StepInContext("BindGrid");
                //access = new Db//access();
                //access.ExceptionOccured += new Db//access.Exceptionhandler(this.db//access_ExceptionOccured);
                //queryParams = new //queryParams();
                query = string.Format("select * from vGetATMDetail where atm_id = {0}", atmId);
                cmd.CommandText = query;
                table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
                //queryParams.ProductType = DatabaseType.iWatch;
                //queryParams.AtmIds = atmId;
                //table = //access.ExecuteQuery(query, //queryParams);
                if (table.Rows.Count > 0)
                {
                    this.Label_Title.Text = table.Rows[0]["title"].ToString();
                    this.Label_IP.Text = table.Rows[0]["ip"].ToString();
                    this.Label_Type.Text = table.Rows[0]["atm_type"].ToString();
                    this.Label_Region.Text = table.Rows[0]["region_name"].ToString();
                    this.Label_AggregateState.Text = table.Rows[0]["aggregate_state_desc"].ToString();
                    query = string.Format("select calculated_state from vGetATMSummary where atm_id = {0}", atmId);

                    cmd.CommandText = query;
                    adapter = new SqlDataAdapter(cmd);
                    DataTable table3 = new DataTable();
                    adapter.Fill(table3);

                    this.Label_CalcState.Text = table3.Rows[0]["calculated_state"].ToString();
                    this.Label_CurrentMode.Text = table.Rows[0]["current_mode_desc"].ToString();
                    this.Label_LastBootTime.Text = table.Rows[0]["last_boot_time"].ToString();
                    this.Label_LastScanTime.Text = table.Rows[0]["last_scan_time"].ToString();
                    this.Label_DiscoveredOn.Text = table.Rows[0]["discovery_time"].ToString();
                    query = string.Format("select * from vGetATMSupplyState where atm_id = {0}", atmId);
                    cmd.CommandText = query;
                    adapter = new SqlDataAdapter(cmd);
                    table2 = new DataTable();
                    adapter.Fill(table2);
                    //table2 = //access.ExecuteQuery(query, //queryParams);
                    if (table2.Rows.Count > 0)
                    {
                        this.Label_SupplyState.Text = table2.Rows[0]["supply_state_desc"].ToString();
                    }
                    else
                    {
                        this.Label_SupplyState.Text = "";
                    }
                    //newCommand = ConnectionFactory.GetNewCommand(true);
                    //newCommand.CommandText = string.Format("select user_full_name from app_user inner join user_atms on app_user.user_id = user_atms.user_id where atm_id = {0} ", atmId);
                    //reader = newCommand.ExecuteReader();
                    //this.Label_ServiceEngineers.Text = "";
                    //while (reader.Read())
                    //{
                    //    this.Label_ServiceEngineers.Text = this.Label_ServiceEngineers.Text + reader.GetValue(0) + " | ";
                    //}
                    this.Img_mode.ImageUrl = Extension.getCurrentModeImageUrl(this.Label_CurrentMode.Text);
                    this.img_aggregate.ImageUrl = Extension.getAggregateStateImageUrl(this.Label_AggregateState.Text);
                    this.img_calcstate.ImageUrl = Extension.getCalculatedStateImageUrl(this.Label_CalcState.Text);
                    this.img_supply.ImageUrl = Extension.getSupplyStateImageUrl(this.Label_SupplyState.Text);
                }
                str = this.ViewState["SortExpression"].ToString() + " " + this.ViewState["SortDirection"].ToString();
                //queryParams.OrderBy = str;
                set = new DataSet();
                query = "select * from vGetATMParentDevices where atm_id = " + atmId + " order by " + str;
                cmd.CommandText = query;
                adapter = new SqlDataAdapter(cmd);
                set.Tables.Add("ParentDevices");
                adapter.Fill(set.Tables[0]);
                //set.Tables.Add(//access.ExecuteQuery(query, //queryParams).Copy());
                //set.Tables[0].TableName = "ParentDevices";
                query = "select * from (select atm_devices.atm_id, atm_devices.device_id, device_name, vendor_name,  device_state as device_state_str, device_state,device_state_desc,   device_number, module_number, is_replenishable  from atm_devices  inner join device on atm_devices.device_id = device.device_id  left join atm_device_state on atm_devices.device_id = atm_device_state.device_id and atm_devices.atm_id = atm_device_state.atm_id  where atm_devices.is_active = 1 and device.is_active = 1 and is_abstract = 0  and module_number is not null ) v where atm_id = " + atmId + " order by " + str;
                cmd.CommandText = query;
                adapter = new SqlDataAdapter(cmd);
                set.Tables.Add("ChildDevices");
                adapter.Fill(set.Tables[1]);

                //            set.Tables.Add(//access.ExecuteQuery(query, //queryParams).Copy());
                //set.Tables[1].TableName = "ChildDevices";
                set.Relations.Add("ParentAndChildDevices", set.Tables["ParentDevices"].Columns["device_number"], set.Tables["ChildDevices"].Columns["device_number"], false);
                this.Grid_Devices.DataSource = set;
                this.Grid_Devices.DataMember = "ParentDevices";
                this.Grid_Devices.DataBind();
                this.Grid_Devices.RowExpanded.CollapseAll();
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding grid.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding grid: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.StepOutContext();
                //if (reader != null)
                //{
                //    reader.Close();
                //}
                //if (newCommand != null)
                //{
                //    newCommand.Connection.Close();
                //    newCommand.Dispose();
                //}
            }
        }

        protected void BindGridCashAndSupply()
        {
            SqlCommand command = null;
            SqlDataReader reader = null;
            //Db//access db//access = null;
            //queryParams //queryParams = null;
            DataTable dtATM = null;
            DataSet set = null;
            DataTable dtSupplyState = null;
            string selectQuery = "";
            set = new DataSet();
            try
            {
                //BasePage.\\UILogger.StepInContext("BindGrid");
                this.DisplayHeaderInfo(ref dtATM);//ref db//access, ref //queryParams, ref dtATM, ref dtSupplyState, ref selectQuery);
                selectQuery = "select * from vGetCashPosition    where atm_id=" + atmId + "  Order By cassette_name";
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = selectQuery;
                set.Tables.Add("CashStatus");
                set.Tables.Add("SupplyStatus");
                //set.Tables.Add(db//access.ExecuteQuery(selectQuery, //queryParams).Copy());
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(set.Tables[0]);
                //set.Tables[0].TableName = "CashStatus";
                this.GridView_CashState.DataSource = set;
                this.GridView_CashState.DataMember = "CashStatus";
                this.GridView_CashState.DataBind();
                selectQuery = "select device_name,convert(varchar,device_state) as device_state,device_state_desc\r\n                                from dbo.vGetATMChildDevices where is_replenishable=1 \r\n                                and device_state is not null\r\n                                and atm_id='" + atmId + "' order by device_state_desc";
                cmd.CommandText = selectQuery;
                //set.Tables.Add("SupplyStatus");
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(set.Tables[1]);
                //set.Tables.Add(db//access.ExecuteQuery(selectQuery, //queryParams).Copy());
                //set.Tables[1].TableName = "SupplyStatus";
                foreach (DataRow row in set.Tables["SupplyStatus"].Rows)
                {
                    row["device_state"] = Extension.GetSupplyStateDescription((string)row["device_state"]);
                }
                this.GridView_SupplyState.DataSource = set;
                this.GridView_SupplyState.DataMember = "SupplyStatus";
                this.GridView_SupplyState.DataBind();
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding Cash and Supply grid.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding grid: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.StepOutContext();
                if (reader != null)
                {
                    reader.Close();
                }
                if (command != null)
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
        }

        protected void Button_back_Click(object sender, EventArgs e)
        {
            if (page.ToLower().Equals("dashboardiconview"))
            {
                page = null;
                atmId = null;
                base.Response.Redirect("../iwatch/DashboardIconView.aspx");
            }
            else if (page.ToLower().Equals("dashboardlistview"))
            {
                page = null;
                atmId = null;
                base.Response.Redirect("../iwatch/DashboardListView.aspx");
            }
        }



        private void DisplayHeaderInfo(ref DataTable dtATM)
        {
            //db//access = new Db//access();
            // db//access.ExceptionOccured += new Db//access.Exceptionhandler(this.db//access_ExceptionOccured);
            //queryParams = new //queryParams();
            string selectQuery = string.Format("select * from vGetATMDetail where atm_id = {0}", atmId);
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = selectQuery;

            //queryParams.ProductType = DatabaseType.iWatch;
            //queryParams.AtmIds = atmId;
            dtATM = new DataTable(); //access.ExecuteQuery(selectQuery, //queryParams);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtATM);
            if (dtATM.Rows.Count > 0)
            {
                this.Label_Title.Text = dtATM.Rows[0]["title"].ToString();
                this.Label_IP.Text = dtATM.Rows[0]["ip"].ToString();
                this.Label_Type.Text = dtATM.Rows[0]["atm_type"].ToString();
                this.Label_Region.Text = dtATM.Rows[0]["region_name"].ToString();
                this.Label_AggregateState.Text = dtATM.Rows[0]["aggregate_state_desc"].ToString();
                this.Label_CurrentMode.Text = dtATM.Rows[0]["current_mode_desc"].ToString();
                this.Label_LastBootTime.Text = dtATM.Rows[0]["last_boot_time"].ToString();
                this.Label_LastScanTime.Text = dtATM.Rows[0]["last_scan_time"].ToString();
                this.Label_DiscoveredOn.Text = dtATM.Rows[0]["discovery_time"].ToString();
                selectQuery = string.Format("select * from vGetATMSupplyState where atm_id = {0}", atmId);
                cmd.CommandText = selectQuery;
                DataTable dtSupplyState = new DataTable();//= db//access.ExecuteQuery(selectQuery, //queryParams);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dtSupplyState);
                if (dtSupplyState.Rows.Count > 0)
                {
                    this.Label_SupplyState.Text = dtSupplyState.Rows[0]["supply_state_desc"].ToString();
                }
                else
                {
                    this.Label_SupplyState.Text = "";
                }
                this.Img_mode.ImageUrl = Extension.getCurrentModeImageUrl(this.Label_CurrentMode.Text);
                selectQuery = string.Format("SELECT  dbo.GetCurrentMode  ( service_state) from dbo.atm_history \r\n             where (from_time   in (select max(from_time) from atm_history where from_time not in( select MAX(from_time ) FROM [atm_history] where atm_id= {0})) and atm_id ={0})", atmId);
                cmd.CommandText = selectQuery;
                //queryParams.ProductType = DatabaseType.iWatch;
                //queryParams.AtmIds = atmId;
                DataTable table = new DataTable();// db//access.ExecuteQuery(selectQuery, //queryParams);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    this.Label_Last_CurrentMode.Text = table.Rows[0][0].ToString();
                    this.img_last_currentmode.ImageUrl = Extension.getCurrentModeImageUrl(this.Label_Last_CurrentMode.Text);
                }
                this.img_aggregate.ImageUrl = Extension.getAggregateStateImageUrl(this.Label_AggregateState.Text);
                this.img_supply.ImageUrl = Extension.getSupplyStateImageUrl(this.Label_SupplyState.Text);
            }
        }

        protected void Grid_Devices_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            System.Web.UI.WebControls.Image image = (System.Web.UI.WebControls.Image)e.Item.Cells[5].FindControl("Image1");
            if (e.Item.ItemIndex > -1)
            {
                image.ImageUrl = Extension.getParentDeviceStatusImages(e.Item.Cells[4].Text);
                e.Item.Cells[4].Text = Extension.ParentDeviceStatusToStr(e.Item.Cells[4].Text);
            }
        }

        protected void Grid_Devices_TemplateSelection(object sender, HierarGridTemplateSelectionEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("ATMDetail");
                //BasePage.\\UILogger.StepInContext("Grid_Devices_TemplateSelection");
                e.TemplateFilename = "~/admin/ChildDevicesDetail.ascx";
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

        protected void GridView_CashState_RowCreated(object sender, GridViewRowEventArgs e)
        {
        }

        protected void GridView_CashState_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("CashAndSupplyStatus");
                //BasePage.\\UILogger.StepInContext("GridView_RowDataBound");
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.RowIndex == 0)
                    {
                        this.GridView_CashState.Columns[1].FooterText = e.Row.Cells[1].Text;
                    }
                    else if (this.GridView_CashState.Columns[1].FooterText != e.Row.Cells[1].Text)
                    {
                        this.GridView_CashState.Columns[1].FooterText = "";
                    }
                    int result = 0;
                    int num2 = 0;
                    int num3 = 0;
                    int num4 = 0;
                    int num5 = 0;
                    int.TryParse(e.Row.Cells[2].Text, out result);
                    int.TryParse(e.Row.Cells[3].Text, out num2);
                    int.TryParse(e.Row.Cells[4].Text, out num3);
                    int.TryParse(e.Row.Cells[5].Text, out num4);
                    int.TryParse(e.Row.Cells[6].Text, out num5);
                    if (result == -1)
                    {
                        result = 0;
                    }
                    if (num2 == -1)
                    {
                        num2 = 0;
                    }
                    if (num3 == -1)
                    {
                        num3 = 0;
                    }
                    if (num4 == -1)
                    {
                        num4 = 0;
                    }
                    if (num5 == -1)
                    {
                        num5 = 0;
                    }
                    runningTotalRemaining += result * num2;
                    runningTotalRejected += result * num3;
                    runningTotalDispensed += result * num4;
                    runningTotalJammed += result * num5;
                    this.GridView_CashState.Columns[0].FooterText = "Total Amount";
                    this.GridView_CashState.Columns[3].FooterText = runningTotalRemaining.ToString("#,###,##0");
                    this.GridView_CashState.Columns[4].FooterText = runningTotalRejected.ToString("#,###,##0");
                    this.GridView_CashState.Columns[5].FooterText = runningTotalDispensed.ToString("#,###,##0");
                    this.GridView_CashState.Columns[6].FooterText = runningTotalJammed.ToString("#,###,##0");
                    this.GridView_CashState.FooterStyle.CssClass = "bold";
                    this.GridView_CashState.Columns[0].FooterStyle.CssClass = "GridRowCellCenteredClass";
                    this.GridView_CashState.Columns[1].FooterStyle.CssClass = "GridRowCellCenteredClass";
                    this.GridView_CashState.Columns[2].FooterStyle.CssClass = "GridRowCellLeftAlignedClass";
                    this.GridView_CashState.Columns[3].FooterStyle.CssClass = "GridRowCellRightAlignedClass";
                    this.GridView_CashState.Columns[4].FooterStyle.CssClass = "GridRowCellRightAlignedClass";
                    this.GridView_CashState.Columns[5].FooterStyle.CssClass = "GridRowCellRightAlignedClass";
                    this.GridView_CashState.Columns[6].FooterStyle.CssClass = "GridRowCellRightAlignedClass";
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding row.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding row: [" + exception.Message + "]</li>";
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
                //BasePage.\\UILogger.OpenContext("ATMDetail");
                //BasePage.\\UILogger.StepInContext("GridView_PageIndexChanging");
                this.Grid_Devices.CurrentPageIndex = e.NewPageIndex;
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
                //BasePage.\\UILogger.OpenContext("ATMDetail");
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
                    if (string.IsNullOrEmpty((string)this.ViewState["SortDirection"]))
                    {
                        this.ViewState["SortDirection"] = "asc";
                    }
                    else if (((string)this.ViewState["SortDirection"]) == "asc")
                    {
                        this.ViewState["SortDirection"] = "desc";
                    }
                    else
                    {
                        this.ViewState["SortDirection"] = "asc";
                    }
                }
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

        protected void GridView2_SupplyState_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                string str;
                //BasePage.\\UILogger.OpenContext("CashAndSupplyStatus.aspx");
                //BasePage.\\UILogger.StepInContext("GridView2_RowDataBound");
                if ((e.Row.RowType == DataControlRowType.DataRow) && ((str = e.Row.Cells[1].Text) != null))
                {
                    if (!(str == "Healthy"))
                    {
                        if (str == "Attention Soon")
                        {
                            goto Label_00B0;
                        }
                        if (str == "Attention Now")
                        {
                            goto Label_00CE;
                        }
                        if (str == "Not Configured")
                        {
                            goto Label_00E9;
                        }
                        if (str == "Unknown")
                        {
                        }
                    }
                    else
                    {
                        e.Row.BackColor = Color.FromArgb(0x98, 0xfb, 0x98);
                    }
                }
                return;
            Label_00B0:
                e.Row.BackColor = Color.FromArgb(0xff, 0xaf, 0x5f);
                return;
            Label_00CE:
                e.Row.BackColor = Color.FromArgb(0xff, 0x7d, 0x7d);
                return;
            Label_00E9:
                e.Row.BackColor = Color.FromArgb(0xff, 0xff, 0);
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding row.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding row: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("ATMDetail");
                //BasePage.\\UILogger.StepInContext("Page_Load");
                //if (!base.CheckRight(0xd3))
                //{
                //   // base.Response.Redirect("//accessDenied.aspx");
                //}
                //else
                //{
                page = base.Request.QueryString["page"];
                //this.showAtmStateCounts();
                if (base.Request.QueryString["atm_id"] == null)
                {
                    this.Button_back.Visible = false;
                    if ((((bool)this.Session[SessionVars.isRegionSelected.ToString()]) || (this.Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1)) || (this.Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0))
                    {
                        this.img_aggregate.Visible = false;
                        this.Img_mode.Visible = false;
                        this.img_supply.Visible = false;
                        this.rdball.Visible = false;
                        this.rdbfatal.Visible = false;
                        this.Rdbwarning.Visible = false;
                        this.Literal_error.Text = "<li>ATM not selected. Please Select ATM.</li>";
                        return;
                    }
                    if (((string)this.Session[SessionVars.SelectedATMs.ToString()]) == "-1")
                    {
                        this.img_aggregate.Visible = false;
                        this.Img_mode.Visible = false;
                        this.img_supply.Visible = false;
                        this.rdball.Visible = false;
                        this.rdbfatal.Visible = false;
                        this.Rdbwarning.Visible = false;
                        this.Literal_error.Text = "<li>ATM not selected. Please Select ATM.</li>";
                        return;
                    }
                    atmId = this.Session[SessionVars.SelectedATMs.ToString()].ToString();
                    this.img_aggregate.Visible = true;
                    this.Img_mode.Visible = true;
                    this.img_supply.Visible = true;
                    this.rdball.Visible = true;
                    this.rdbfatal.Visible = true;
                    this.Rdbwarning.Visible = true;
                }
                else
                {
                    atmId = base.Request.QueryString["atm_id"];
                }
                if (string.IsNullOrEmpty((string)this.ViewState["SortDirection"]))
                {
                    this.ViewState["SortDirection"] = "asc";
                }
                if (string.IsNullOrEmpty((string)this.ViewState["SortExpression"]))
                {
                    this.ViewState["SortExpression"] = "device_name";
                }
                this.BindGrid();
                this.Page.SetFocus(this.Button_back);
                runningTotalRemaining = 0L;
                runningTotalRejected = 0L;
                runningTotalDispensed = 0L;
                runningTotalJammed = 0L;
                this.BindGridCashAndSupply();
                //}
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

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new EventHandler(this.refresh_ServerClick);
        }

        protected void rdball_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.rdball.Checked)
                {
                    this.BindGrid();
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding grid.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding grid: [" + exception.Message + "]</li>";
            }
        }

        protected void rdbfatal_CheckedChanged(object sender, EventArgs e)
        {
            this.Grid_Devices.CurrentPageIndex = 0;
            try
            {
                if (this.rdbfatal.Checked)
                {
                    //Db//access //access = null;
                    //queryParams //queryParams = null;
                    DataSet set = null;
                    string str = "";
                    string query = "";
                    //BasePage.\\UILogger.StepInContext("BindGrid");
                    //access = new Db//access();
                    //access.ExceptionOccured += new Db//access.Exceptionhandler(this.db//access_ExceptionOccured);
                    //queryParams = new //queryParams();
                    //query = string.Format("select * from vGetATMDetail where atm_id = {0}", atmId);


                    //queryParams.ProductType = DatabaseType.iWatch;
                    //queryParams.AtmIds = atmId;
                    //access.ExecuteQuery(query, //queryParams);
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    query = string.Format("select * from vGetATMDetail where atm_id = {0}", atmId);
                    cmd.CommandText = query;
                    DataTable table = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    str = this.ViewState["SortExpression"].ToString() + " " + this.ViewState["SortDirection"].ToString();
                    //queryParams.OrderBy = str;
                    set = new DataSet();
                    set.Tables.Add("ParentDevices");
                    set.Tables.Add("ChildDevices");
                    query = "select * from vGetATMParentDevices where atm_id = " + atmId + " and device_state = 4 order by " + str;

                    cmd.CommandText = query;
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(set.Tables[0]);
                    //set.Tables.Add(//access.ExecuteQuery(query, //queryParams).Copy());
                    //set.Tables[0].TableName = "ParentDevices";
                    query = "select * from (select atm_devices.atm_id, atm_devices.device_id, device_name, vendor_name,  device_state as device_state_str, device_state,device_state_desc,   device_number, module_number, is_replenishable  from atm_devices  inner join device on atm_devices.device_id = device.device_id  left join atm_device_state on atm_devices.device_id = atm_device_state.device_id and atm_devices.atm_id = atm_device_state.atm_id  where atm_devices.is_active = 1 and device.is_active = 1 and is_abstract = 0  and module_number is not null ) v where atm_id = " + atmId + " order by " + str;
                    cmd.CommandText = query;
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(set.Tables[1]);
                    //set.Tables.Add(//access.ExecuteQuery(query, //queryParams).Copy());
                    //set.Tables[1].TableName = "ChildDevices";
                    set.Relations.Add("ParentAndChildDevices", set.Tables["ParentDevices"].Columns["device_number"], set.Tables["ChildDevices"].Columns["device_number"], false);
                    this.Grid_Devices.DataSource = set;
                    this.Grid_Devices.DataMember = "ParentDevices";
                    this.Grid_Devices.DataBind();
                    this.Grid_Devices.RowExpanded.CollapseAll();
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding grid.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding grid: [" + exception.Message + "]</li>";
            }
        }

        protected void Rdbwarning_CheckedChanged(object sender, EventArgs e)
        {
            this.Grid_Devices.CurrentPageIndex = 0;
            try
            {
                if (this.Rdbwarning.Checked)
                {
                    // Db//access //access = null;
                    //queryParams //queryParams = null;
                    DataSet set = null;
                    string str = "";
                    string query = "";
                    //BasePage.\\UILogger.StepInContext("BindGrid");
                    //access = new Db//access();
                    //access.ExceptionOccured += new Db//access.Exceptionhandler(this.db//access_ExceptionOccured);
                    //queryParams = new //queryParams();
//                    query = string.Format("select * from vGetATMDetail where atm_id = {0}", atmId);
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    query = string.Format("select * from vGetATMDetail where atm_id = {0}", atmId);
                    cmd.CommandText = query;
                    DataTable table = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(table);

                    //queryParams.ProductType = DatabaseType.iWatch;
                    //queryParams.AtmIds = atmId;
                    //access.ExecuteQuery(query, //queryParams);
                    str = this.ViewState["SortExpression"].ToString() + " " + this.ViewState["SortDirection"].ToString();
                    //queryParams.OrderBy = str;
                    set = new DataSet();
                    set.Tables.Add("ParentDevices");
                    set.Tables.Add("ChildDevices");
                    query = "select * from vGetATMParentDevices where atm_id = " + atmId + " and device_state <> 0 order by " + str;
                    
                    cmd.CommandText = query;
                    adapter = new SqlDataAdapter(cmd);
                    
                    adapter.Fill(set.Tables[0]);
                    //set.Tables.Add(//access.ExecuteQuery(query, //queryParams).Copy());
                    //set.Tables[0].TableName = "ParentDevices";
                    query = "select * from (select atm_devices.atm_id, atm_devices.device_id, device_name, vendor_name,  device_state as device_state_str, device_state,device_state_desc,   device_number, module_number, is_replenishable  from atm_devices  inner join device on atm_devices.device_id = device.device_id  left join atm_device_state on atm_devices.device_id = atm_device_state.device_id and atm_devices.atm_id = atm_device_state.atm_id  where atm_devices.is_active = 1 and device.is_active = 1 and is_abstract = 0  and module_number is not null ) v where atm_id = " + atmId + " order by " + str;
                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandText = query;
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(set.Tables[1]);
                    //set.Tables.Add(//access.ExecuteQuery(query, //queryParams).Copy());
                    //set.Tables[1].TableName = "ChildDevices";
                    set.Relations.Add("ParentAndChildDevices", set.Tables["ParentDevices"].Columns["device_number"], set.Tables["ChildDevices"].Columns["device_number"], false);
                    this.Grid_Devices.DataSource = set;
                    this.Grid_Devices.DataMember = "ParentDevices";
                    this.Grid_Devices.DataBind();
                    this.Grid_Devices.RowExpanded.CollapseAll();
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding grid.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding grid: [" + exception.Message + "]</li>";
            }
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
        }

        private void showAtmStateCounts()
        {
            string str = "";
            string query = "";
            //Db//access //access = new Db//access();
            //access.ExceptionOccured += new Db//access.Exceptionhandler(this.db//access_ExceptionOccured);
            //queryParams //queryParams = new //queryParams();
            query = string.Format("select * from vGetATMSummary order by current_mode_code", new object[0]);
            //queryParams.ProductType = DatabaseType.iWatch;
            //queryParams.AtmIds = this.Session[SessionVars.SelectedATMs.ToString()].ToString();
            //queryParams.OrderBy = str;

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = query;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            //this.Span_InService.InnerText = "In Service : " + table.Select("current_mode_code = 4").Length.ToString();
            //this.Span_Supervisor.InnerText = "Supervisor : " + table.Select("current_mode_code = 3").Length.ToString();
            //this.Span_OutService.InnerText = "Not In Service : " + table.Select("NOT(current_mode_code = 4 OR aggregate_state = 3)").Length.ToString();
        }
    }



}