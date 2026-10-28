using Avanza.iSuite.UI;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Avanza.iSuite.DAL;
using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI;
namespace CCMSUI.CCMS
{
    public partial class DeviceTemplates : BasePage
    {
        protected void BindGrid()
        {
            SqlDataAdapter adapter = null;
            SqlCommand selectCommand = null;
            DataTable dataTable = null;
            StringBuilder builder = null;
            try
            {
                ////BasePage.\\UILogger.StepInContext("BindGrid");
                selectCommand = ConnectionFactory.GetNewCommand(false);
                builder = new StringBuilder("select * from device_template where ");
                if (this.TextBox_name.Text != string.Empty)
                {
                    builder.Append("device_template_name like @template_name and ");
                    selectCommand.Parameters.AddWithValue("@template_name", "%" + this.TextBox_name.Text + "%");
                }
                builder.Append("is_active in (" + this.DropDownList_status.SelectedValue + ") ");
                builder.Append("order by " + (this.ViewState["SortExpression"].ToString() + " " + this.ViewState["SortDirection"].ToString()));
                selectCommand.CommandText = builder.ToString();
                adapter = new SqlDataAdapter(selectCommand);
                dataTable = new DataTable();
                adapter.Fill(dataTable);
                this.GridView_DeviceTemplates.DataSource = dataTable;
                this.GridView_DeviceTemplates.DataBind();
                this.GridView_DeviceTemplates.Visible = true;
                this.Literal_count.Text = dataTable.Rows.Count + " records.";
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding grid.", exception);
                this.Literal_error.Text = "<li>An error occurred while binding grid: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.StepOutContext();
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
            this.ViewState["SortExpression"] = "device_template_name";
            this.BindGrid();
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        //private void dbAccess_ExceptionOccured(ArrayList Exceptions)
        //{
        //    string str = null;
        //    try
        //    {
        //        //BasePage.\\UILogger.OpenContext("DeviceTemplates");
        //        //BasePage.\\UILogger.StepInContext("dbAccess_ExceptionOccured");
        //        this.Literal_error.Text = "<li> An error occurred while loading data ";
        //        foreach (MultiserverException exception in Exceptions)
        //        {
        //            str = str + exception.ServerName + ": " + exception.ExceptionRaised.Message;
        //            this.Literal_error.Text = this.Literal_error.Text + "<br />Unable to connect to " + exception.ServerName;
        //        }
        //        //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while loading data. Error[" + str + "]");
        //        this.Literal_error.Text = this.Literal_error.Text + "</li>";
        //    }
        //    finally
        //    {
        //        //BasePage.\\UILogger.CloseContext();
        //    }
        //}

        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("DeviceTemplates");
                //BasePage.\\UILogger.StepInContext("GridView_PageIndexChanging");
                this.GridView_DeviceTemplates.PageIndex = e.NewPageIndex;
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

        protected void GridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            SqlCommand newCommand = null;
            SqlTransaction trx = null;
            try
            {
                //BasePage.\\UILogger.OpenContext("DeviceTemplates");
                //BasePage.\\UILogger.StepInContext("GridView_RowCommand");
                if (e.CommandName == "Deactivate")
                {
                    //if (base.CheckRight(0xb1))
                    //{
                    newCommand = ConnectionFactory.GetNewCommand(true);
                    trx = newCommand.Connection.BeginTransaction();
                    newCommand.Transaction = trx;
                    ConnectionFactory.ExecuteQuery("update device_template set is_active = 0 where device_template_id = " + e.CommandArgument.ToString(), trx);
                    ConnectionFactory.ExecuteQuery("update atm_devices set is_active = 0 where device_template_id = " + e.CommandArgument.ToString(), trx);
                    this.Literal_ok.Text = "<li>Selected Device Template was deactivated!</li>";
                    trx.Commit();
                    this.BindGrid();
                    //}
                }
                else if (e.CommandName == "Activate")
                {
                    //if (base.CheckRight(0xb1))
                    //{
                    newCommand = ConnectionFactory.GetNewCommand(true);
                    trx = newCommand.Connection.BeginTransaction();
                    newCommand.Transaction = trx;
                    ConnectionFactory.ExecuteQuery("update device_template set is_active = 1 where device_template_id = " + e.CommandArgument.ToString(), trx);
                    ConnectionFactory.ExecuteQuery("update atm_devices set is_active = 1 where device_template_id = " + e.CommandArgument.ToString(), trx);
                    this.Literal_ok.Text = "<li>Selected Device Template was activated!</li>";
                    trx.Commit();
                    this.BindGrid();
                    //}
                }//&& base.CheckRight(0xcf)
                else if ((e.CommandName == "deleteDeviceTemplate"))
                {
                    if (((int)ConnectionFactory.ExecuteScalar("select isnull(count(atm_id),0) from atm_devices where device_template_id = " + e.CommandArgument.ToString())) == 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendFormat("delete device_template_devices where device_template_id = {0};", e.CommandArgument.ToString());
                        builder.AppendFormat("delete device_template where device_template_id = {0};", e.CommandArgument.ToString());
                        ConnectionFactory.ExecuteQuery(builder.ToString());
                        //BasePage.\\UILogger.Log(SeverityLevel.Information, string.Format("Device Template deleted. {0}", e.CommandArgument.ToString()));
                        this.Literal_ok.Text = "<li>Selected Device Template was deleted!</li>";
                    }
                    else
                    {
                        //BasePage.\\UILogger.Log(SeverityLevel.Information, "Device Template cannot be deleted as it is assigned to some atm(s).");
                        this.Literal_error.Text = "<li>Device Template cannot be deleted as it is assigned to some atm(s).</li>";
                    }
                    this.BindGrid();
                }
            }
            catch (Exception exception)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while executing row command.", exception);
                if (exception.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    this.Literal_error.Text = "<li>Cannot delete device template that has atm associated with it</li>";
                }
                else
                {
                    this.Literal_error.Text = "<li>An error occurred while executing row command: [" + exception.Message + "]</li>";
                }
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
                if (newCommand != null)
                {
                    newCommand.Connection.Close();
                    newCommand.Dispose();
                }
            }
        }

        protected void GridView_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("DeviceTemplates");
                //BasePage.\\UILogger.StepInContext("GridView_RowCreated");
                if (((this.ViewState["SortDirection"] != null) && (this.ViewState["SortExpression"] != null)) && (e.Row.RowType == DataControlRowType.Header))
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        if (cell.HasControls())
                        {
                            LinkButton button = (LinkButton)cell.Controls[0];
                            if (button != null)
                            {
                                Image child = new Image();
                                child.ImageUrl = "../images/gridimages/" + this.ViewState["SortDirection"].ToString().Trim() + ".gif";
                                if (this.ViewState["SortExpression"].ToString().Trim() == button.CommandArgument)
                                {
                                    cell.Controls.Add(new LiteralControl(" "));
                                    cell.Controls.Add(child);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while creating row.", exception);
                this.Literal_error.Text = "<li>An error occurred while creating row: [" + exception.Message + "]</li>";
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
            }
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("DeviceTemplates");
                //BasePage.\\UILogger.StepInContext("GridView_RowDataBound");
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton button = (ImageButton)e.Row.FindControl("Button_Deactivate");
                    ImageButton button2 = (ImageButton)e.Row.FindControl("Button_Activate");
                    if ((bool)((DataRowView)e.Row.DataItem)["is_active"])
                    {
                        button2.Visible = false;
                    }
                    else
                    {
                        button.Visible = false;
                    }
                    if (e.Row.FindControl("Button_Activate") != null)
                    {
                        ((ImageButton)e.Row.FindControl("Button_Activate")).Attributes.Add("onClick", "return confirm('Are you sure you want to Activate Template " + ((DataRowView)e.Row.DataItem).Row["device_template_name"] + " as it might be associated with an atm?', 'CCMS')");
                    }
                    if (e.Row.FindControl("Button_Deactivate") != null)
                    {
                        ((ImageButton)e.Row.FindControl("Button_Deactivate")).Attributes.Add("onClick", "return confirm('Are you sure you want to Deactivate Template " + ((DataRowView)e.Row.DataItem).Row["device_template_name"] + " as it might be associated with an atm?', 'CCMS')");
                    }
                    if (e.Row.FindControl("img_delete") != null)
                    {
                        ((ImageButton)e.Row.FindControl("img_delete")).Attributes.Add("onClick", "return confirm('Are you sure you want to delete Template " + ((DataRowView)e.Row.DataItem).Row["device_template_name"] + "?', 'CCMS')");
                    }
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

        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("DeviceTemplates");
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("DeviceTemplates");
                //BasePage.\\UILogger.StepInContext("Page_Load");
                
                //else 
                if (!this.Page.IsPostBack)
                {
                    if (base.CheckRight((int)Permissions.DeviceTemplate))
                    {
                        base.Response.Redirect("~/admin/AccessDenied.aspx");
                    }

                    if (!base.CheckRight((int)Permissions.AddDeviceTemplate))
                    {
                        this.HyperLink_newDeviceTemplate.Visible = false;
                    }
                    if (!base.CheckRight((int)Permissions.EditDeviceTemplate))
                    {
                        this.GridView_DeviceTemplates.Columns[3].Visible = false;
                    }
                    if (!base.CheckRight((int)Permissions.DeleteDeviceTemplate))
                    {
                        this.GridView_DeviceTemplates.Columns[4].Visible = false;
                    }
                    this.SetDefaultValues();
                    this.ViewState["SortDirection"] = "asc";
                    this.ViewState["SortExpression"] = "device_template_name";
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
            }
        }

        protected void SetDefaultValues()
        {
            try
            {
                //BasePage.\\UILogger.StepInContext("SetDefaultValues");
                this.TextBox_name.Text = "";
                this.DropDownList_status.ClearSelection();
                this.DropDownList_status.SelectedIndex = 0;
                this.GridView_DeviceTemplates.DataSource = new DataTable();
                this.GridView_DeviceTemplates.DataBind();
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