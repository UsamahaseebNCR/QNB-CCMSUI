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

using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

namespace CCMSUI.admin
{
    public partial class CIT : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCIT))
                {
                    Response.Redirect("AccessDenied.aspx");
                    Response.StatusCode = 401;
                }

                if (!CheckRight((int)Permissions.ModifyCIT))
                    GridView_CIT.Columns[6].Visible = false;

                if (!CheckRight((int)Permissions.DeleteCIT))
                    GridView_CIT.Columns[7].Visible = false;

                if (!CheckRight((int)Permissions.CreateCIT))
                    HyperLink_newCIT.Visible = false;

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "name";
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
                filter += "  name ='" + TextBox_name.Text + "' and ";
            if (TextBox_Code.Text.Length > 0)
                filter += "  code ='" + TextBox_Code.Text + "' and ";

            if (DropDownList_creator.SelectedIndex > 0)
                filter += "  created_by =" + DropDownList_creator.SelectedValue + " and ";

            filter += " is_active in (" + DropDownList_status.SelectedValue + ") and ";
            if (filter.Length > 0)
                return "and " + filter.Substring(0, filter.LastIndexOf("and "));
            else
                return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                string selectedRegion = null;
                if (Session[SessionVars.SelectedRegions.ToString()] == null)
                    selectedRegion = "-1";
                else
                    selectedRegion = (string)Session[SessionVars.SelectedRegions.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                cmd.CommandText = @"select id,name,code,email_id,order_dispatch_time,user_login,is_active,created_on,normal_order_cost,emergency_order_cost,modified_on from ccms_cit,app_user where ccms_cit.created_by = app_user.user_id and ccms_cit.is_deleted = 0 " + Filter() + "  order by " + orderby;


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtCIT = new DataTable();
                adapter.Fill(dtCIT);
                cmd.Connection.Close();
                this.GridView_CIT.DataSource = dtCIT; ;
                this.GridView_CIT.DataBind();
                Literal_count.Text = dtCIT.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_CIT_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_CIT_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteCIT")
            {
                task = LogableTask.NewTask("delete CIT");

                try
                {



                    //if ((int)ConnectionFactory.ExecuteScalar("select count(cit_id) from app_user where cit_id=" + e.CommandArgument.ToString()) != 0)
                    //    throw new Exception ("Cannot delete this cit as it is assigned to some users");

                    //if ((int)ConnectionFactory.ExecuteScalar("select count(cit_id) from region_cit where cit_id=" + e.CommandArgument.ToString()) != 0)
                    //    throw new Exception("Cannot delete this cit as it is assigned to some organizations");
                    AppUser user = ((AppUser)Session[SessionVars.user.ToString()]);
                    CcmsCit ccmsCit = CcmsCit.LoadCcmsCitByPk(int.Parse(e.CommandArgument.ToString()));

                    ccmsCit.IsDeleted = true;
                    ccmsCit.ModifiedOn = DateTime.Now;
                    ccmsCit.ModifiedBy = (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    ccmsCit.Save();
                    Utility.BuildAuditLog("CIT "+ ccmsCit.Name + " deleted", user.UserId, (int)Permissions.DeleteCIT);

                    //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                    Session[SessionVars.notice.ToString()] = "Selected cit was deleted!";
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "cit deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting CIT. " + ex.Message + "</li>";

                }
                finally
                {

                    task.EndTask();

                }
            }
        }

        protected void GridView_CIT_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_CIT.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_CIT_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteCIT") != null)


                ((ImageButton)e.Row.FindControl("Button_deleteCIT")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete CIT " + ((DataRowView)e.Row.DataItem).Row["name"] + "?','CCMS')");


        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            ValidateInput();

            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());


        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_name.Text = "";
            TextBox_Code.Text = "";
            DropDownList_creator.SelectedIndex = 0;
            DropDownList_status.SelectedIndex = 0;



        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


        public void ValidateInput()
        {
           
                bool isNameContainSpecialChars = CheckSpecialChars(TextBox_name.Text);
                if (isNameContainSpecialChars == true)
                {
                    throw new Exception("Name contain invalid character");
                    //return result = false;
                }
                bool isCodeContainSpecialChars = CheckSpecialChars(TextBox_Code.Text);
                if (isCodeContainSpecialChars == true)
                {
                    throw new Exception("Code contain invalid character");
                    //return result = false;
                }
           
        }

    }
}