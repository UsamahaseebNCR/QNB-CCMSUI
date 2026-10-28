using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Data;
namespace CCMSUI.admin
{
    public partial class ChildDevices : UserControl
    {
        // Fields
        protected GridView Grid_Devices;

        // Methods
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("ChildDevices");
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //BasePage.\\UILogger.OpenContext("ChildDevices");
                //BasePage.\\UILogger.StepInContext("Page_Load");
                //if (base.Session[SessionVars.dtUserRights.ToString()] == null)
                //{
                //    base.Response.Redirect("../signin.aspx");
                //}
                //else if (!((DataTable)base.Session[SessionVars.dtUserRights.ToString()]).Rows.Contains(0xb2))
                //{
                //    this.Grid_Devices.Columns[2].Visible = false;
                //}
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