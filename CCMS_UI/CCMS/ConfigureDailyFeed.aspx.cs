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

using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class ConfigureDailyFeed : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LogableTask task = LogableTask.NewTask("ConfigureDailyFeed");
                try
                {

                    ViewState["sortDirection"] = " asc";
                    ViewState["SortExpression"] = "mcn";
                    if (!CheckRight((int)Permissions.ViewConfigureDailyFeed))
                    {
                        Response.StatusCode = 401;
                        Response.Redirect("../admin/AccessDenied.aspx");
                    }


                    AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                    DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                    while (userReader.Read())
                        DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                    userReader.Close();

                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());



                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                }
                finally
                {
                    task.EndTask();
                }
            }
        }
        string Filter()
        {
            string filter = "";
            if (TextBox_MCN.Text.Length > 0)
                filter += " and mcn ='" + TextBox_MCN.Text + "'";
            if (DropDownList_creator.SelectedIndex > 0)
                filter += " and created_by = " + DropDownList_creator.SelectedValue;

            return filter;
        }

        private void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                bool isMCNContainSpecialChars = CheckSpecialChars(TextBox_MCN.Text);
                if (isMCNContainSpecialChars == true)
                {
                    throw new Exception("MCN contain invalid character");
                }

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = "select distinct(mcn) mcn from region where region_id>1 and is_organization=1" + Filter() + " order by " + orderBy; ;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                GridView_MCNs.DataSource = dt;
                GridView_MCNs.DataBind();
                Literal_count.Text = dt.Rows.Count + " records.";
            }
            catch (Exception ex)
            { 
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); 
            }
            finally
            { task.EndTask(); }

        }
        protected void GridView_MCNs_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_MCNs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_MCNs.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
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
            TextBox_MCN.Text = "";
            DropDownList_creator.SelectedIndex = 0;
        }
    

        protected void GridView_MCNs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "editMCN")
            {
                try
                {
                    task = LogableTask.NewTask("edit MCN");

                    Session["mcn"] = e.CommandArgument.ToString();
                    Response.Redirect("ConfigureDailyFeedStep2.aspx" , false);
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while Edit MCN. " + ex.Message + "</li>";

                }
                finally
                {
                    task.EndTask();
                }
            }
        }



    }
}