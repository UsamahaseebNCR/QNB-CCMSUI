using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

namespace CCMSUI.admin
{
    public partial class ATMBandwidth : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                
                if (!CheckRight((int)Permissions.ModifyATMBandwidth))
                    GridView_Bandwidth.Columns[5].Visible = false;

                if (!CheckRight((int)Permissions.DeleteATMBandwidth))
                    GridView_Bandwidth.Columns[6].Visible = false;

                if (!CheckRight((int)Permissions.CreateATMBandwidth))
                    this.HyperLink_newBandwidth.Visible = false;

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "atm_Bandwidth_name";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }


        }
        string Filter()
        {
            string filter = "";
            if (TextBox_name.Text.Length > 0)
                filter += " atm_Bandwidth_name like '%" + TextBox_name.Text + "%' and ";
        
            if (filter.Length > 0)
                return "where " + filter.Substring(0, filter.LastIndexOf("and"));
            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT * FROM atm_Bandwidth inner join app_user on app_user.user_id =atm_Bandwidth.created_by  " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtFileType = new DataTable();
                adapter.Fill(dtFileType);
                cmd.Connection.Close();
                this.GridView_Bandwidth.DataSource = dtFileType;
                this.GridView_Bandwidth.DataBind();
                Literal_count.Text = GridView_Bandwidth.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = ex.Message;

            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Bandwidth_Sorting(object sender, GridViewSortEventArgs e)
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


        protected void GridView_Bandwidth_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteBandwidth") != null)


                ((ImageButton)e.Row.FindControl("Button_deleteBandwidth")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete ATM Bandwidth " + ((DataRowView)e.Row.DataItem).Row["atm_Bandwidth_name"] + "?','CCMS')");


        }



        protected void GridView_Bandwidth_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteBandwidth")
            {

                try
                {
                    task = LogableTask.NewTask("delete atm Bandwidth");
                    Avanza.CCMS.DAL.AtmBandwidth Bandwidth = Avanza.CCMS.DAL.AtmBandwidth.LoadAtmBandwidthByPk(int.Parse(e.CommandArgument.ToString()));
                    Bandwidth.Delete();

                    if (Bandwidth != null)
                    {
                        Utility.BuildAuditLog("ATM Bandwidth: "+Bandwidth.AtmBandwidthName + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteATMBandwidth);

                        //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                        Session[SessionVars.notice.ToString()] = "Selected atm Bandwidth deleted!";
                    }

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Selected atm Bandwidth deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting atm Bandwidth. " + ex.Message + "</li>";

                }
                finally
                {

                    task.EndTask();

                }
            }
        }

        protected void GridView_Bandwidth_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Bandwidth.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }



        protected void Button_search_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNameContainSpecialChars = CheckSpecialChars(TextBox_name.Text);
                if (isNameContainSpecialChars == true)
                {
                    throw new Exception("Name contain invalid character");
                    
                }
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("invalid"))
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    Literal_error.Text = ex.Message;
                }
            }
           
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_name.Text = "";

        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }
     
    }
}