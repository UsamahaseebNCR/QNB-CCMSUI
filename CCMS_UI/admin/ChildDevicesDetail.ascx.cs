using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Data;
namespace CCMSUI.admin
{
    public partial class ChildDevicesDetail : UserControl
    {

        // Fields
        protected GridView Grid_Devices;

        // Methods
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("ChildDevicesDetail");
                //BasePage.\\UILogger.StepInContext("Grid_DataBinding");
                DataGridItem bindingContainer = (DataGridItem)base.BindingContainer;
                DataSet dataItem = (DataSet)bindingContainer.DataItem;
                this.Grid_Devices.DataSource = dataItem;
                this.Grid_Devices.DataMember = "ChildDevices";
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while binding grid.", exception);
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
            }
        }

        protected void Grid_Devices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Image image = (Image)e.Row.Cells[5].FindControl("Image1");
            if (e.Row.RowIndex > -1)
            {
                image.ImageUrl = Extension.getChildDeviceStatusImages(e.Row.Cells[4].Text);
                e.Row.Cells[4].Text = Extension.ChildDeviceStatusToStr(e.Row.Cells[4].Text);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("ChildDevicesDetail");
                //BasePage.\\UILogger.StepInContext("Page_Load");
                if (base.Session[SessionVars.dtUserRights.ToString()] == null)
                {
                    base.Response.Redirect("../signin.aspx");
                }
            }
            catch (Exception exception)
            {
                //BasePage.\\UILogger.Log(SeverityLevel.Error, "An error occurred while loading page.", exception);
            }
            finally
            {
                //BasePage.\\UILogger.CloseContext();
            }
        }
    }




}