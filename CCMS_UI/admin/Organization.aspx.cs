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
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.admin
{
    public partial class Organization : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewOrganization))
                {
                    Response.Redirect("AccessDenied.aspx");
                    Response.StatusCode = 401;
                }

                if (!CheckRight((int)Permissions.ModifyOrganization))
                    GridView_Organization.Columns[5].Visible = false;

                if (!CheckRight((int)Permissions.DeleteOrganization))
                    GridView_Organization.Columns[6].Visible = false;

                if (!CheckRight((int)Permissions.CreateOrganization))
                    HyperLink_newOrganization.Visible = false;

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "region_name";
                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                //   AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                //if (!objappsetting.IsGasperexchangeConfigured.Value)
                //  GridView_ATMs.Columns[3].Visible = true;
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();


                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        string Filter()
        {
            string filter = "";
            if (TextBox_name.Text.Length > 0)
                filter += "  region_name like '%" + TextBox_name.Text + "%' and ";
            if (TextBox_location.Text.Length > 0)
                filter += "  location like '%" + TextBox_location.Text + "%' and ";

            if (TextBox_country.Text.Length > 0)
                filter += " country like '%" + TextBox_country.Text + "%' and ";

            if (TextBox_MCN.Text.Length > 0)
                filter += "  mcn like '%" + TextBox_MCN.Text + "%' and ";

            if (DropDownList_creator.SelectedIndex > 0)
                filter += "  created_by = " + DropDownList_creator.SelectedValue + " and ";



            string selectedRegion = null;
            if (Session[SessionVars.SelectedRegions.ToString()] == null)
                selectedRegion = "-1";
            else
                selectedRegion = (string)Session[SessionVars.SelectedRegions.ToString()];

            filter += " is_active in (" + DropDownList_status.SelectedValue + ") and ";
            if (filter.Length > 0)
                return "where is_organization = 1 and region_id > 1 and region_id in (" + selectedRegion + ") and " + filter.Substring(0, filter.LastIndexOf("and "));
            else
                return "where is_organization = 1 and region_id > 1 and region_id in (" + selectedRegion + ")";
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                ValidateInputs();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                cmd.CommandText = @"select [region_id],[region_name] ,[location],[country],	[MCN] ,
	            is_active  from region  " + Filter() + "  order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtOrganization = new DataTable();
                adapter.Fill(dtOrganization);
                cmd.Connection.Close();
                this.GridView_Organization.DataSource = dtOrganization;
                this.GridView_Organization.DataBind();
                Literal_count.Text = dtOrganization.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Organization_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Organization_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteOrganization")
            {

                task = LogableTask.NewTask("delete organization");
                SqlConnection conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                SqlTransaction trxn = conn.BeginTransaction();
                try
                {

                    if ((int)ConnectionFactory.ExecuteScalar("select count(region_id) from region  " +
                        " where  parent_region_id = " + e.CommandArgument.ToString()) != 0)
                        throw new Exception("Cannot delete selected organization as it has one or more child objects");


                    int id = int.Parse(e.CommandArgument.ToString());
                    Avanza.CCMS.DAL.Region organization = Avanza.CCMS.DAL.Region.LoadRegionByPk(id);
                    ConnectionFactory.ExecuteQuery("delete region where region_id = " + id + ";update ccms_organization set is_deleted = 1 where region_id=" + id + ";delete mcn_atms where mcn='" + organization.MCN + "';delete region_receipients where region_id =" + id + ";delete region_cit where region_id =" + id, trxn);



                    //organization.Delete(conn);
                    //CcmsOrganization ccmsOrganization = CcmsOrganization.LoadCcmsOrganization("region_id=" + organization.RegionId);
                    //ccmsOrganization.IsDeleted = true;
                    //ccmsOrganization.Save(conn,trxn);



                    Session[SessionVars.SelectedRegion.ToString()] = "1";
                    if (organization != null)
                        Utility.BuildAuditLog("Organization "+ organization.RegionName + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteOrganization);

                    //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                    Session[SessionVars.notice.ToString()] = "<script>parent.window.ShowSuccess('Organization deleted successfully!');parent.window.DeleteNode(" + organization.RegionId + ");window.location='organization.aspx' </script>";


                    DataTable dt = ((DataTable)Session[SessionVars.dtUserOrgs.ToString()]);
                    if (dt != null)
                        dt.Rows.Remove(dt.Select("MCN = '" + organization.MCN + "'")[0]);
                    trxn.Commit();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "organization deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    trxn.Rollback();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting Organization. " + ex.Message + "</li>";
                }
                finally
                {
                    if (conn != null)
                        conn.Close();

                    task.EndTask();

                }
            }
        }

        protected void GridView_Organization_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Organization.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Organization_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteOrganization") != null)


                ((ImageButton)e.Row.FindControl("Button_deleteOrganization")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete Organization " + ((DataRowView)e.Row.DataItem).Row["region_name"] + "?','CCMS')");


        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            try
            {
                   BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
              
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }


        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_name.Text = "";
            DropDownList_creator.SelectedIndex = 0;
            DropDownList_status.SelectedIndex = 0;
            TextBox_location.Text = "";
            TextBox_MCN.Text = "";
            TextBox_country.Text = "";
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }

       

        public void ValidateInputs()
        {
            
            bool isNameContainSpecialChars = CheckSpecialChars(TextBox_name.Text);
            if (isNameContainSpecialChars == true)
            {
                throw new Exception ("Name contain invalid character");
                
            }
            bool isLocationContainSpecialChars = CheckSpecialChars(TextBox_location.Text);
            if (isLocationContainSpecialChars == true)
            {
                throw new Exception ("Location contain invalid character");
                
            }
            bool isMCNContainSpecialChars = CheckSpecialChars(TextBox_MCN.Text);
            if (isMCNContainSpecialChars == true)
            {
                throw new Exception ("MCN contain invalid character");
                
            }
            bool isCountryContainSpecialChars = CheckSpecialChars(TextBox_country.Text);
            if (isCountryContainSpecialChars == true)
            {
                throw new Exception ("Country contain invalid character");
                
            }
            
        }
    }
}